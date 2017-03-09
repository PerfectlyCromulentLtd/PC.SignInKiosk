using OxHack.SignInKiosk.CoreService.SubServices;
using System;
using System.Threading.Tasks;

namespace OxHack.SignInKiosk.CoreService
{
	class Bootstrapper
	{
		private readonly SignInEventProcessor eventProcessor;
		private readonly OffsiteStorageService offsiteStorage;

		public Bootstrapper(SignInEventProcessor eventProcessor, OffsiteStorageService offsiteStorage)
		{
			this.eventProcessor = eventProcessor;
			this.offsiteStorage = offsiteStorage;
		}

		public async Task Start()
		{
			await this.offsiteStorage.Connect();
			await this.eventProcessor.Start();
		}

		public async Task Stop()
		{
			await this.eventProcessor.Stop();
			await this.offsiteStorage.Disconnect();
		}
	}
}