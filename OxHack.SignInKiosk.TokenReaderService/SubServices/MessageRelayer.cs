using OxHack.SignInKiosk.Messaging;
using OxHack.SignInKiosk.Messaging.Messages;
using System;
using System.Threading.Tasks;
using Prism.Events;
using OxHack.SignInKiosk.TokenReaderService.Events;

namespace OxHack.SignInKiosk.TokenReaderService.SubServices
{
	class MessageRelayer
	{
		private readonly MessagingClient messagingClient;
		private readonly IEventAggregator eventAggregator;
		

		public MessageRelayer(IEventAggregator eventAggregator, MessagingClient messagingClient)
		{
			this.eventAggregator = eventAggregator;
			this.messagingClient = messagingClient;

			this.eventAggregator.GetEvent<TokenReadEvent>().Subscribe(async message => await this.messagingClient.Publish(message));
		}

		public async Task Start()
		{
			await this.messagingClient.Connect();
		}

		public async Task Stop()
		{
			await this.messagingClient.Disconnect();
		}
	}
}
