using Caliburn.Micro;
using OxHack.SignInKiosk.Domain.Messages;
using OxHack.SignInKiosk.Events;
using OxHack.SignInKiosk.MessageBrokerProxy;
using System;
using System.ServiceModel;
using System.Threading.Tasks;

namespace OxHack.SignInKiosk.Services
{
	public class MessageBrokerService
	{
		private ServiceCallback serviceCallback;
		private MessageBrokerProxyServiceClient serviceClient;
		private readonly IEventAggregator eventAggregator;
		private Task keepAliveWorker;

		public MessageBrokerService(IEventAggregator eventAggregator)
		{
			this.eventAggregator = eventAggregator;
		}

		public async Task Publish(SignInRequestSubmitted message)
		{
			await this.serviceClient.PublishSignInRequestAsync(message);
		}

		public async Task Publish(SignOutRequestSubmitted message)
		{
			await this.serviceClient.PublishSignOutRequestAsync(message);
		}

		public async Task Connect()
		{
			await this.CreateNewConnection();
			this.keepAliveWorker = Task.Run(this.KeepAliveWorkerLoop);
		}

		private async Task KeepAliveWorkerLoop()
		{
			while (true)
			{
				try
				{
					await Task.Delay(TimeSpan.FromMinutes(5));
					await this.serviceClient?.KeepAliveAsync();
				}
				catch
				{
					// Do nothing
				}
			}
		}

		private async Task CreateNewConnection()
		{
			if (this.serviceClient != null)
			{
				this.serviceClient.InnerChannel.Faulted -= this.HandleServiceClientFaults;
			}

			var timeout = TimeSpan.FromSeconds(7);

			//TODO: Enable transport security.
			var binding = new NetTcpBinding()
			{
				MaxBufferSize = int.MaxValue,
				ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max,
				MaxReceivedMessageSize = int.MaxValue,
				Security = new NetTcpSecurity()
				{
					Message = new MessageSecurityOverTcp()
					{
						ClientCredentialType = MessageCredentialType.None
					},
					Transport = new TcpTransportSecurity()
					{
						ClientCredentialType = TcpClientCredentialType.None
					},
					Mode = SecurityMode.None,
				},
				CloseTimeout = timeout,
				OpenTimeout = timeout,
				ReceiveTimeout = timeout,
				SendTimeout = timeout,
			};

			var remoteAddress = new EndpointAddress(new Uri("net.tcp://MessageBrokerProxyService:8137/MessageBrokerProxyService"));

			this.serviceCallback = new ServiceCallback(this.eventAggregator);
			this.serviceClient = new MessageBrokerProxyServiceClient(new InstanceContext(this.serviceCallback), binding, remoteAddress);
			this.serviceClient.InnerChannel.Faulted += this.HandleServiceClientFaults;

			await this.serviceClient.SubscribeAsync();
			await this.eventAggregator.PublishOnUIThreadAsync(new Connected());
		}

		private async void HandleServiceClientFaults(object sender, EventArgs e)
		{
			this.eventAggregator.PublishOnUIThread(new Disconnected());

			await Task.Delay(TimeSpan.FromSeconds(5));

			try
			{
				await this.CreateNewConnection();
			}
			catch
			{
				//TODO: log error
			}
		}

		class ServiceCallback : IMessageBrokerProxyServiceCallback
		{
			private readonly IEventAggregator eventAggregator;

			public ServiceCallback(IEventAggregator eventAggregator)
			{
				this.eventAggregator = eventAggregator;
			}

			public async void OnPersonSignedInPublished(PersonSignedIn message)
			{
				await this.eventAggregator.PublishOnUIThreadAsync(message);
			}

			public async void OnPersonSignedOutPublished(PersonSignedOut message)
			{
				await this.eventAggregator.PublishOnUIThreadAsync(message);
			}

			public async void OnTokenReadPublished(TokenRead message)
			{
				await this.eventAggregator.PublishOnUIThreadAsync(message);
			}

			public async void OnSignInRequestSubmittedPublished(SignInRequestSubmitted message)
			{
				await this.eventAggregator.PublishOnUIThreadAsync(message);
			}

			public async void OnSignOutRequestSubmittedPublished(SignOutRequestSubmitted message)
			{
				await this.eventAggregator.PublishOnUIThreadAsync(message);
			}

			public void KeepCallbackAlive()
			{
				// To nothing.
			}
		}
	}
}
