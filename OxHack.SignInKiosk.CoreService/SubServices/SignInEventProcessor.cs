using NLog;
using OxHack.SignInKiosk.Database.Services;
using OxHack.SignInKiosk.Messaging;
using OxHack.SignInKiosk.Messaging.Messages;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
				processingAction();
				this.logger.Debug("... finished processing sign-in event.");
			}
		}

		private async Task StoreSignIn(SignInRequestSubmitted e)
		{
			try
			{
				var signInTime = this.signInService.SignIn(e.Person.DisplayName, e.Person.IsVisitor, e.Person.TokenId);
				await this.messagingClient.Publish(new PersonSignedIn(signInTime, e.Person));
			}
			catch (Exception exception)
			{
				this.logger.Error(exception);
			}
		}

		private async Task StoreSignOut(SignOutRequestSubmitted e)
		{
			try
			{
				var signOutTime = this.signInService.SignOut(e.Person.DisplayName, e.SignInTime, e.Person.IsVisitor, e.Person.TokenId);
				await this.messagingClient.Publish(new PersonSignedOut(e.SignInTime, signOutTime, e.Person));
			}
			catch (Exception exception)
			{
				this.logger.Error(exception);
			}
		}
	}
}
