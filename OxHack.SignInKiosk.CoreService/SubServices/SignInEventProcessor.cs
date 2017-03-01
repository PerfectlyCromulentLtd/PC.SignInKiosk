using NLog;
using OxHack.SignInKiosk.Database.Services;
using OxHack.SignInKiosk.Domanin.Messages;
using OxHack.SignInKiosk.Domanin.Messages.Models;
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

		private BlockingCollection<Action> eventProcessingQueue;
		private Task worker;

		public SignInEventProcessor(MessagingClient messagingClient, SignInService signInService)
		{
			this.messagingClient = messagingClient;
			this.signInService = signInService;

			this.messagingClient.SignInRequestSubmitted += (s, e) => this.eventProcessingQueue.Add(() => this.StoreSignIn(e));
			this.messagingClient.SignOutRequestSubmitted += (s, e) => this.eventProcessingQueue.Add(() => this.StoreSignOut(e));
		}

		internal async Task Start()
		{
			this.eventProcessingQueue = new BlockingCollection<Action>();
			this.worker = Task.Run(() => this.EventProcessingLoop());

			await this.messagingClient.Connect();
		}

		internal async Task Stop()
		{
			await this.messagingClient.Disconnect();

			this.eventProcessingQueue.CompleteAdding();
			this.worker = null;
		}

		private void EventProcessingLoop()
		{
			foreach (var processingAction in this.eventProcessingQueue.GetConsumingEnumerable())
			{
				this.logger.Debug("Processing sign-in event...");
				try
				{
					processingAction();
				}
				catch (Exception exception)
				{
					this.logger.Error(exception);
				}
				this.logger.Debug("... finished processing sign-in event.");
			}
		}

		private async Task StoreSignIn(SignInRequestSubmitted e)
		{
			var signInTime = this.signInService.SignIn(e.Person.DisplayName, e.Person.IsVisitor, e.Person.TokenId);
			await this.messagingClient.Publish(new PersonSignedIn(signInTime, e.Person));
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
