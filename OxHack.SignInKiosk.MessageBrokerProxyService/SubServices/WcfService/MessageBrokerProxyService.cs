using NLog;
using OxHack.SignInKiosk.Messaging;
using OxHack.SignInKiosk.Messaging.Messages;
using System;
using System.ServiceModel;

namespace OxHack.SignInKiosk.MessageBrokerProxyService.SubServices.WcfService
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Single)]
	public class MessageBrokerProxyService : IMessageBrokerProxyService
	{
		private readonly ILogger logger = LogManager.GetCurrentClassLogger();
		private IMessageBrokerProxyServiceCallback callBack;
		private readonly MessagingClient messagingClient;

		public MessageBrokerProxyService(MessagingClient messagingClient)
		{
			this.messagingClient = messagingClient;

			this.messagingClient.PersonSignedIn += this.RelayPersonSignedInMessage;
			this.messagingClient.PersonSignedOut += this.RelayPersonSignedOutMessage;
			this.messagingClient.SignInRequestSubmitted += this.RelaySignInRequestSubmitted;
			this.messagingClient.TokenRead += this.RelayTokenReadMessage;
		}

		private void RelayPersonSignedInMessage(Object sender, PersonSignedIn message)
		{
			try
			{
				this.callBack?.OnPersonSignedInPublished(message);
			}
			catch (Exception exception)
			{
				this.logger.Error(exception);
			}
		}

		private void RelayPersonSignedOutMessage(Object sender, PersonSignedOut message)
		{
			try
			{
				this.logger.Debug($"Relaying {nameof(PersonSignedOut)} message: {message.Person.TokenId}");

				this.callBack?.OnPersonSignedOutPublished(message);
			}
			catch (Exception exception)
			{
				this.logger.Error(exception);
			}
		}

		private void RelaySignInRequestSubmitted(Object sender, SignInRequestSubmitted message)
		{
			try
			{
				this.logger.Debug($"Relaying {nameof(SignInRequestSubmitted)} message: {message.Person.TokenId}");

				this.callBack?.OnSignInRequestSubmittedPublished(message);
			}
			catch (Exception exception)
			{
				this.logger.Error(exception);
			}
		}

		private void RelayTokenReadMessage(Object sender, TokenRead message)
		{
			try
			{
				this.logger.Debug($"Relaying {nameof(TokenRead)} message: {message.Id}");

				this.callBack?.OnTokenReadPublished(message);
			}
			catch (Exception exception)
			{
				this.logger.Error(exception);
			}
		}

		public async void PublishSignInRequestSubmitted(SignInRequestSubmitted message)
		{
			try
			{
				this.logger.Debug($"Publishing {nameof(SignInRequestSubmitted)} message: {message.Person.DisplayName}");

				await this.messagingClient.Publish(message);

				// HACK: temporary
				await this.messagingClient.Publish(new PersonSignedIn(message.Person));
			}
			catch (Exception exception)
			{
				this.logger.Error(exception);
			}
		}

		public void Subscribe()
		{
			try
			{
				OperationContext ctx = OperationContext.Current;
				this.callBack = ctx.GetCallbackChannel<IMessageBrokerProxyServiceCallback>();

				this.logger.Info("Client subscribed.");
			}
			catch (Exception exception)
			{
				this.logger.Error(exception);
			}
		}

		public void Unsubscribe()
		{
			try
			{
				this.logger.Info("Client unsubscribed.");
				this.callBack = null;
			}
			catch (Exception exception)
			{
				this.logger.Error(exception);
			}
		}
	}
}
