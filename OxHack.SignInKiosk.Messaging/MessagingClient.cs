using MassTransit;
using MassTransit.NLogIntegration;
using OxHack.SignInKiosk.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace OxHack.SignInKiosk.Messaging
{
	public sealed class MessagingClient
	{
		public event EventHandler<PersonSignedIn> PersonSignedIn;
		public event EventHandler<PersonSignedOut> PersonSignedOut;
		public event EventHandler<SignInRequestSubmitted> SignInRequestSubmitted;
		public event EventHandler<TokenRead> TokenRead;

		private BusHandle bus;
		private IBusControl busControl;

		public MessagingClient(
			string username = "signInKiosk",
			string password = "signInKiosk",
			string connectionString = "rabbitmq://rampage:5672/",
			string queueName = null
			)
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
						receiveConfig.Consumer(() => new DelegateConsumer<PersonSignedIn>(message => this.PersonSignedIn?.Invoke(this, message)));
						receiveConfig.Consumer(() => new DelegateConsumer<PersonSignedOut>(message => this.PersonSignedOut?.Invoke(this, message)));
						receiveConfig.Consumer(() => new DelegateConsumer<SignInRequestSubmitted>(message => this.SignInRequestSubmitted?.Invoke(this, message)));
						receiveConfig.Consumer(() => new DelegateConsumer<TokenRead>(message => this.TokenRead?.Invoke(this, message)));
					});
			});
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
