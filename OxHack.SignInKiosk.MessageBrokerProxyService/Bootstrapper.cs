using OxHack.SignInKiosk.Messaging;
using OxHack.SignInKiosk.MessageBrokerProxyService.SubServices.WcfService;
using System.Threading.Tasks;
using NLog;

namespace OxHack.SignInKiosk.MessageBrokerProxyService
{
	class Bootstrapper
	{
		private readonly ILogger logger = LogManager.GetCurrentClassLogger();
		private readonly WcfServiceRunner wcfServiceRunner;

		public Bootstrapper()
		{
			this.wcfServiceRunner = new WcfServiceRunner(new MessagingClient());
		}

		public async Task Start()
		{
			this.logger.Info("Start WCF Service and dependencies...");
			await this.wcfServiceRunner.Start();
		}

		public async Task Stop()
		{
			this.logger.Info("Stopping WCF Service and dependencies...");
			await this.wcfServiceRunner.Stop();
		}
	}
}
