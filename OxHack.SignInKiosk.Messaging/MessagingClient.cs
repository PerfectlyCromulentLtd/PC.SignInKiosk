using MassTransit;
using MassTransit.NLogIntegration;
using OxHack.SignInKiosk.Messaging.Messages;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace OxHack.SignInKiosk.Messaging
{
	public sealed class MessagingClient
	{
		private BusHandle bus;
		private readonly IBusControl busControl;

		public MessagingClient(string username, string password, string connectionString = "rabbitmq://rampage:5672/", string queueName = null)
		{
			var hostAddress = new Uri(connectionString);
			queueName = queueName ?? Assembly.GetEntryAssembly().GetName().Name;

			this.busControl = Bus.Factory.CreateUsingRabbitMq(busConfig =>
			{
				busConfig.UseNLog();

				busConfig.AutoDelete = true;
				busConfig.Durable = false;
				busConfig.Exclusive = true;
				busConfig.PurgeOnStartup = false;

				var host = busConfig.Host(hostAddress, hostConfig =>
				{
					hostConfig.Username(username);
					hostConfig.Password(password);
				});

				busConfig.ReceiveEndpoint(
					host,
					queueName,
					receiveConfig =>
					{
						receiveConfig.AutoDelete = true;
						receiveConfig.Durable = false;
						receiveConfig.Exclusive = true;
					});
			});
		}

		public void AddConsumer<T>(Func<T> consumerFactory) where T : class, IConsumer
		{
			this.busControl.ConnectConsumer<T>(consumerFactory);
		}

		public async Task Publish<T>(T message) where T : class
		{
			await this.busControl.Publish(message);
		}

		public async Task Connect()
		{
			this.bus = await this.busControl.StartAsync();
		}

		public async Task Disconnect()
		{
			await this.bus.StopAsync();
		}
	}
}
