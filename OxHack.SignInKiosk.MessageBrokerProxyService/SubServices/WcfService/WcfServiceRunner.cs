using NLog;
using OxHack.SignInKiosk.Messaging;
using OxHack.SignInKiosk.Messaging.Messages;
using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Threading.Tasks;

namespace OxHack.SignInKiosk.MessageBrokerProxyService.SubServices.WcfService
{
	class WcfServiceRunner
	{
		private readonly ILogger logger = LogManager.GetCurrentClassLogger();
		private readonly MessageReceiver messageReceiver;
		private readonly MessagePublisher messageSender;
		private ServiceHost serviceHost;

		public WcfServiceRunner(MessageReceiver messageReceiver, MessagePublisher messageSender)
		{
			this.messageReceiver = messageReceiver;
			this.messageSender = messageSender;
		}

		public async Task Start()
		{
			await this.messageReceiver.Connect();
			await this.messageSender.Connect();
			await this.StartService();
		}

		public async Task Stop()
		{
			await this.StopService();
			await this.messageSender.Disconnect();
			await this.messageReceiver.Disconnect();
		}

		private async Task StartService()
		{
			try
			{
				this.serviceHost = new ServiceHost(new MessageBrokerProxyService(this.messageReceiver, this.messageSender));
				this.serviceHost.Opened += (s, e) => this.logger.Info("Service Opened.");
				this.serviceHost.Closed += (s, e) => this.logger.Info("Service Closed.");
				this.serviceHost.Faulted += (s, e) => this.logger.Error("Shit!  Service Faulted.");
				this.serviceHost.UnknownMessageReceived += (s, e) => this.logger.Warn("Huh?  Service received unknown message.");

				await Task.Factory.FromAsync(
					(callback, state) => this.serviceHost.BeginOpen(callback, state),
					(ar) => this.logger.Info(nameof(MessageBrokerProxyService) + " started."),
					null);
			}
			catch (Exception exception)
			{
				this.logger.Error(exception);
			}
		}

		private async Task StopService()
		{
			try
			{
				if (this.serviceHost != null)
				{
					await Task.Factory.FromAsync(
						(callback, state) => this.serviceHost.BeginClose(callback, state),
						(ar) => this.logger.Info(nameof(MessageBrokerProxyService) + " stopped."),
						null);
				}
			}
			catch (Exception exception)
			{
				this.logger.Error(exception);
			}
		}
	}
}
