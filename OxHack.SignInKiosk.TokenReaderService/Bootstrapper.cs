using OxHack.SignInKiosk.Messaging;
using OxHack.SignInKiosk.TokenReaderService.SubServices;
using Prism.Events;
using System.Threading.Tasks;

namespace OxHack.SignInKiosk.TokenReaderService
{
	class Bootstrapper
	{
		private readonly MessageRelayer messageRelayer;
		private readonly TokenReader tokenReader;

		public Bootstrapper()
		{
			var eventAggregator = new EventAggregator();

			this.tokenReader = new TokenReader(eventAggregator);
			this.messageRelayer = new MessageRelayer(eventAggregator, new MessagingClient());
		}

		public async Task Start()
		{
			//await this.messageRelayer.Start();
			await this.tokenReader.Start();
		}

		public async Task Stop()
		{
			await this.tokenReader.Stop();
			//await this.messageRelayer.Stop();
		}
	}
}
