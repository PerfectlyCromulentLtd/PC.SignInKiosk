using OxHack.SignInKiosk.CoreService.SubServices;
using System;
using System.Threading.Tasks;

namespace OxHack.SignInKiosk.CoreService
{
	class Bootstrapper
	{
		private readonly SignInEventProcessor eventProcessor;

		public Bootstrapper(SignInEventProcessor eventProcessor)
		{
			this.eventProcessor = eventProcessor;
		}

		public async Task Start()
		{
			await this.eventProcessor.Start();
		}

		public async Task Stop()
		{
			await this.eventProcessor.Stop();
		}
	}
}