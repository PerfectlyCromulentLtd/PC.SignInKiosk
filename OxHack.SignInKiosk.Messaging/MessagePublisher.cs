using MassTransit;
using MassTransit.NLogIntegration;
using System;
using System.Threading.Tasks;

namespace OxHack.SignInKiosk.Messaging
{
	public class MessagePublisher
	{
		private BusHandle bus;
		private readonly IBusControl busControl;
		private readonly Uri targetUri;

		public MessagePublisher()
		{
			var hostAddress = new Uri("rabbitmq://rampage:5672/");
			this.targetUri = new Uri(hostAddress, "SignInKioskEvents");
			var username = "signInProducer";
			var password = "beepBeepRibbyRibby";

			this.busControl = Bus.Factory.CreateUsingRabbitMq(busConfig =>
			{
				busConfig.UseNLog();

				busConfig.AutoDelete = true;
				busConfig.Durable = false;
				busConfig.Exclusive = true;
				busConfig.PurgeOnStartup = false;

				busConfig.Host(hostAddress, hostConfig =>
				{
					hostConfig.Username(username);
					hostConfig.Password(password);
				});
			});
		}

		public async Task Connect()
		{
			this.bus = await this.busControl.StartAsync();
		}

		public async Task Disconnect()
		{
			await this.bus.StopAsync();
		}

		public async Task Publish<T>(T message) where T : class
		{
			await this.busControl.Publish(message);
		}
	}
}
