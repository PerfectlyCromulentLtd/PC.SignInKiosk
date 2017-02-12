using OxHack.SignInKiosk.Messaging;
using OxHack.SignInKiosk.Messaging.Messages;
using System;
using System.Threading.Tasks;

namespace OxHack.SignInKiosk.TokenReaderService.SubServices
{
	class TokenReader
	{
		private bool isRunning;
		private readonly MessagingClient messagingClient;
		private Task worker;

		public TokenReader(MessagingClient messagingClient)
		{
			this.messagingClient = messagingClient;
		}

		public async Task Start()
		{
			await this.messagingClient.Connect();

			this.worker = Task.Run(() => WorkerLoop());
		}

		public async Task Stop()
		{
			this.isRunning = await Task.FromResult(false);
			await this.worker;
			await this.messagingClient.Disconnect();
		}

		private async Task WorkerLoop()
		{
			this.isRunning = true;

			while (this.isRunning)
			{
				await this.messagingClient.Publish(new TokenRead(DateTime.Now.Ticks.ToString()));
				await Task.Delay(TimeSpan.FromSeconds(5));
			}
		}
	}
}
