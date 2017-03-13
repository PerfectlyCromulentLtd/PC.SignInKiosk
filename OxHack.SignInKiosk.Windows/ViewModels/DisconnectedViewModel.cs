using Caliburn.Micro;
using OxHack.SignInKiosk.Domain.Models;
using OxHack.SignInKiosk.Events;
using OxHack.SignInKiosk.Services;
using Prism.Commands;
using System;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace OxHack.SignInKiosk.ViewModels
{
	public class DisconnectedViewModel : Screen, IHandle<Connected>
	{
		private readonly MessageBrokerService messageBrokerService;
		private bool isConnected;
		private Task worker;

		public DisconnectedViewModel(MessageBrokerService messageBrokerService)
		{
			this.messageBrokerService = messageBrokerService;
		}

		protected override void OnViewLoaded(object view)
		{
			base.OnViewLoaded(view);

			this.isConnected = false;
			this.worker = Task.Run(async () =>
			{
				while (!this.isConnected)
				{
					await Task.Delay(TimeSpan.FromSeconds(5));
					try
					{
						await this.messageBrokerService.ConnectIfNeeded();
					}
					catch
					{
						// do nothing
					}
				}
			});
		}

		public void Handle(Connected message)
		{
			this.isConnected = true;
		}
	}
}
