using Caliburn.Micro;
using OxHack.SignInKiosk.Domanin.Messages;
using OxHack.SignInKiosk.Events;
using OxHack.SignInKiosk.ViewModels;
using OxHack.SignInKiosk.Views;
using System;

namespace OxHack.SignInKiosk.Services
{
	class MessageOrchestratorService :
		IHandle<PersonSignedIn>,
		IHandle<Disconnected>,
		IHandle<Connected>,
		IHandle<TokenRead>
	{
		private readonly INavigationService navigationService;
		private readonly SignInService signInService;
		private readonly TokenHolderInfoService tokenHolderInfoService;
		private readonly ToastService toastService;

		public MessageOrchestratorService(
			INavigationService navigationService,
			SignInService signInService,
			TokenHolderInfoService tokenHolderInfoService,
			ToastService toastService,
			IEventAggregator eventAggregator)
		{
			this.navigationService = navigationService;
			this.signInService = signInService;
			this.tokenHolderInfoService = tokenHolderInfoService;
			this.toastService = toastService;

			eventAggregator.Subscribe(this);
		}

		public async void Handle(TokenRead message)
		{
			// TODO: Play success beep

			if (this.navigationService.CurrentSourcePageType != typeof(StartView))
			{
				this.toastService.Show("A fob was read and ignored.", "To sign-in with it go BACK to the sign-in screen and try again.");
			}
			else
			{
				var tokenHolder = this.tokenHolderInfoService.GetUserByTokenId(message.Id);

				if (tokenHolder == null)
				{
					this.navigationService.NavigateToViewModel<NameEntryViewModel>(message.Id);
				}
				else
				{
					if (await this.signInService.IsSignedIn(tokenHolder.TokenId))
					{
						this.navigationService.NavigateToViewModel<ManualSignOutViewModel>(tokenHolder.TokenId);
					}
					else
					{
						await this.signInService.RequestSignIn(tokenHolder);
					}
				}
			}
		}

		public void Handle(Connected message)
		{
			if (this.navigationService.CurrentSourcePageType == typeof(DisconnectedView))
			{
				this.navigationService.GoBack();
			}
		}

		public void Handle(Disconnected message)
		{
			if (this.navigationService.CurrentSourcePageType != typeof(DisconnectedView))
			{
				this.navigationService.Navigate(typeof(DisconnectedView));
			}
		}

		public void Handle(PersonSignedIn message)
		{
			this.navigationService.NavigateToViewModel<SignedInGreetingViewModel>(message.Person.DisplayName);
		}
	}
}