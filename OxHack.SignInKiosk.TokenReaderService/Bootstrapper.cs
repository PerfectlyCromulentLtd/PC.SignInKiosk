using OxHack.SignInKiosk.Messaging;
using OxHack.SignInKiosk.TokenReaderService.SubServices;
using System.Threading.Tasks;

namespace OxHack.SignInKiosk.TokenReaderService
{
	class Bootstrapper
	{
		private readonly TokenReader tokenReader;

		public Bootstrapper()
		{
			this.tokenReader = new TokenReader(new MessagePublisher());
		}

		public async Task Start()
		{
			await this.tokenReader.Start();
		}

		public async Task Stop()
		{
			await this.tokenReader.Stop();
		}
	}
}
