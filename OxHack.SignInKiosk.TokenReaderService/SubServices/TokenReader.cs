using OxHack.SignInKiosk.Messaging;
using OxHack.SignInKiosk.Messaging.Messages;
using System;
using System.Threading.Tasks;

namespace OxHack.SignInKiosk.TokenReaderService.SubServices
{
	class TokenReader
	{
		private bool isRunning;
		private readonly MessagePublisher messageSender;
		private Task worker;

		public TokenReader(MessagePublisher messageSender)
		{
			this.messageSender = messageSender;
		}

		public async Task Start()
		{
			await this.messageSender.Connect();

			this.worker = Task.Run(() => WorkerLoop());
		}

		public async Task Stop()
		{
			this.isRunning = await Task.FromResult(false);
			await this.worker;
			await this.messageSender.Disconnect();
		}

		private async Task WorkerLoop()
		{
			this.isRunning = true;

			while (this.isRunning)
			{
				await this.messageSender.Publish(new TokenRead(DateTime.Now.Ticks.ToString()));
				await Task.Delay(TimeSpan.FromSeconds(5));
			}
		}
	}
}
