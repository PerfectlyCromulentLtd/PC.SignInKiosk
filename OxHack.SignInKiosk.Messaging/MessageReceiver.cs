using MassTransit;
using MassTransit.NLogIntegration;
using OxHack.SignInKiosk.Messaging.Messages;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace OxHack.SignInKiosk.Messaging
{
	internal sealed class MessageReceiver
	{
		public event EventHandler<PersonSignedIn> PersonSignedIn;
		public event EventHandler<PersonSignedOut> PersonSignedOut;
		public event EventHandler<SignInRequestSubmitted> SignInRequestSubmitted;
		public event EventHandler<TokenRead> TokenRead;

		private BusHandle bus;
		private readonly IBusControl busControl;

		public MessageReceiver()
		{
			var hostAddress = new Uri("rabbitmq://rampage:5672/");
			//var queueName = "SignInKiosk";
			var queueName = Assembly.GetEntryAssembly().GetName().Name;
			var username = "signInConsumer";
			var password = "gimmeDemsBeeps";

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
						receiveConfig.Consumer<DelegateConsumer<PersonSignedIn>>(() => new DelegateConsumer<PersonSignedIn>(message => this.PersonSignedIn?.Invoke(this, message)));
						receiveConfig.Consumer<DelegateConsumer<PersonSignedOut>>(() => new DelegateConsumer<PersonSignedOut>(message => this.PersonSignedOut?.Invoke(this, message)));
						receiveConfig.Consumer<DelegateConsumer<SignInRequestSubmitted>>(() => new DelegateConsumer<SignInRequestSubmitted>(message => this.SignInRequestSubmitted?.Invoke(this, message)));
						receiveConfig.Consumer<DelegateConsumer<TokenRead>>(() => new DelegateConsumer<TokenRead>(message => this.TokenRead?.Invoke(this, message)));
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
	}
}
