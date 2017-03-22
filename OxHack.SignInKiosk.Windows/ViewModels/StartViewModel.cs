using Caliburn.Micro;
using OxHack.SignInKiosk.Events;
using OxHack.SignInKiosk.Services;
using System;

namespace OxHack.SignInKiosk.ViewModels
{
	public class StartViewModel : Screen
	{
		private readonly INavigationService navigationService;
		private readonly MessageBrokerService messageBrokerService;
		private readonly IEventAggregator eventAggregator;

		public StartViewModel(INavigationService navigationService, MessageBrokerService messageBrokerService, IEventAggregator eventAggregator)
		{
			this.navigationService = navigationService;
			this.messageBrokerService = messageBrokerService;
			this.eventAggregator = eventAggregator;
		}

		protected override async void OnActivate()
		{
			base.OnActivate();
			this.navigationService.BackStack.Clear();

			try
			{
				await this.messageBrokerService.ConnectIfNeeded();
			}
			catch (Exception e)
			{
				// TODO: log error
				this.eventAggregator.PublishOnUIThread(new Disconnected());
			}
		}

		public void SignIn()
		{
			this.navigationService.NavigateToViewModel<NameEntryViewModel>();
		}

		public void SignOut()
		{
			this.navigationService.NavigateToViewModel<SignOutViewModel>();
		}
	}
}
