using NLog;
using OxHack.SignInKiosk.Database.Services;
using OxHack.SignInKiosk.Domain.Messages;
using OxHack.SignInKiosk.Domain.Messages.Models;
using OxHack.SignInKiosk.Domain.Models;
using OxHack.SignInKiosk.Messaging;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace OxHack.SignInKiosk.CoreService.SubServices
{
	class SignInEventProcessor
	{
		private readonly ILogger logger = LogManager.GetCurrentClassLogger();

		private readonly MessagingClient messagingClient;
		private readonly SignInService signInService;
		private readonly TokenHolderService tokenHolderService;

		private BlockingCollection<Func<Task>> eventProcessingQueue;
		private Task worker;

		public SignInEventProcessor(
			MessagingClient messagingClient,
			SignInService signInService,
			TokenHolderService tokenHolderService)
		{
			this.messagingClient = messagingClient;
			this.signInService = signInService;
			this.tokenHolderService = tokenHolderService;

			this.messagingClient.SignInRequestSubmitted += (s, e) => this.eventProcessingQueue.Add(async () => await this.StoreSignIn(e));
			this.messagingClient.SignOutRequestSubmitted += (s, e) => this.eventProcessingQueue.Add(async () => await this.StoreSignOut(e));
		}

		internal async Task Start()
		{
			this.eventProcessingQueue = new BlockingCollection<Func<Task>>();
			this.worker = Task.Run(() => this.EventProcessingLoop());

			await this.messagingClient.Connect();
		}

		internal async Task Stop()
		{
			await this.messagingClient.Disconnect();

			this.eventProcessingQueue.CompleteAdding();
			this.worker = null;
		}

		private async Task EventProcessingLoop()
		{
			foreach (var processingAction in this.eventProcessingQueue.GetConsumingEnumerable())
			{
				this.logger.Debug("Processing event...");
				try
				{
					await processingAction();
				}
				catch (Exception exception)
				{
					this.logger.Error(exception);
				}
				this.logger.Debug("... finished processing event.");
			}
		}

		private async Task StoreSignIn(SignInRequestSubmitted e)
		{
			var signInTime = this.signInService.SignIn(e.Person.DisplayName, e.Person.IsVisitor, e.Person.TokenId);
			await this.messagingClient.Publish(new PersonSignedIn(signInTime, e.Person));

			if (!String.IsNullOrWhiteSpace(e.Person.TokenId))
			{
				var tokenHolder = new TokenHolder()
				{ 
					DisplayName = e.Person.DisplayName,
					IsVisitor = e.Person.IsVisitor,
					TokenId = e.Person.TokenId
			};

				var existingTokenHolder = this.tokenHolderService.GetTokenHolderByTokenId(tokenHolder.TokenId);

				if (existingTokenHolder == null)
				{
					this.tokenHolderService.AddTokenHolder(tokenHolder);
				}
			}
		}

		private async Task StoreSignOut(SignOutRequestSubmitted e)
		{
			var signOutTime = this.signInService.SignOut(e.SignedInRecord);
			await this.messagingClient.Publish(
				new PersonSignedOut(
					e.SignedInRecord.SignInTime,
					signOutTime,
					new Person(e.SignedInRecord.TokenId, e.SignedInRecord.DisplayName, e.SignedInRecord.IsVisitor)));
		}
	}
}
