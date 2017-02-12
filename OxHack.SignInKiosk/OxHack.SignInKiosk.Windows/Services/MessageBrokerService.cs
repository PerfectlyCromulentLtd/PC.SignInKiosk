using Caliburn.Micro;
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

		public MessageBrokerService(IEventAggregator eventAggregator)
		{
			this.eventAggregator = eventAggregator;

			// the nasty:
			// TODO: move this call to something that manages the subscription lifecycle
			try
			{
				this.CreateNewConnection();
			}
			catch
			{
				// close eyes.  nothing happened.
			}
		}

		public async Task PublishSignInRequestSubmitted(SignInRequestSubmitted message)
		{
			try
			{
				await this.serviceClient.PublishSignInRequestSubmittedAsync(message);
			}
			catch (Exception e)
			{
				//TODO: log error
			}
		}

		private async Task CreateNewConnection()
		{
			if (this.serviceClient != null)
			{
				this.serviceClient.InnerChannel.Faulted -= this.HandleServiceClientFaults;
			}

			var timeout = TimeSpan.FromSeconds(15);

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
			//this.serviceClient = new MessageBrokerProxyServiceClient(new InstanceContext(this.serviceCallback));
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
			catch (Exception exception)
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
		}
	}
}
