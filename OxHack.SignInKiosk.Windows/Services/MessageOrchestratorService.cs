using Caliburn.Micro;
using OxHack.SignInKiosk.Domain.Messages;
using OxHack.SignInKiosk.Domain.Models;
using OxHack.SignInKiosk.Events;
using OxHack.SignInKiosk.ViewModels;
using OxHack.SignInKiosk.Views;
using System;

namespace OxHack.SignInKiosk.Services
{
	class MessageOrchestratorService :
		IHandle<PersonSignedIn>,
		IHandle<PersonSignedOut>,
		IHandle<ConnectionFaulted>,
		IHandle<Connected>,
		IHandle<TokenRead>
	{
		private readonly INavigationService navigationService;
		private readonly SignInService signInService;
		private readonly TokenHolderService tokenHolderService;
		private readonly ToastService toastService;

		public MessageOrchestratorService(
			INavigationService navigationService,
			SignInService signInService,
			TokenHolderService tokenHolderService,
			ToastService toastService,
			IEventAggregator eventAggregator)
		{
			this.navigationService = navigationService;
			this.signInService = signInService;
			this.tokenHolderService = tokenHolderService;
			this.toastService = toastService;

			eventAggregator.Subscribe(this);
		}

		public async void Handle(TokenRead message)
		{
			// TODO: Play success beep

			try
			{
				if (this.navigationService.CurrentSourcePageType != typeof(StartView))
				{
					this.toastService.Show("A fob was read and ignored.", "To sign-in with it go BACK to the first screen and try again.");
				}
				else
				{
					var tokenHolder = await this.tokenHolderService.GetTokenHolderByTokenId(message.Id);

					if (tokenHolder == null)
					{
						this.navigationService.NavigateToViewModel<NameEntryViewModel>(message.Id);
					}
					else
					{
						SignedInRecord signedInRecord;
						if (this.signInService.TryGetSignedInRecord(tokenHolder.TokenId, out signedInRecord))
						{
							await this.signInService.RequestSignOut(signedInRecord);
						}
						else
						{
							await this.signInService.RequestSignIn(tokenHolder);
						}
					}
				}
			}
			catch (Exception e)
			{
				// TODO: log error
				this.toastService.ShowGenericError();
			}
		}

		public void Handle(Connected message)
		{
			if (this.navigationService.CurrentSourcePageType == typeof(DisconnectedView))
			{
				this.navigationService.GoBack();
			}
		}

		public void Handle(ConnectionFaulted message)
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

		public void Handle(PersonSignedOut message)
		{
			this.navigationService.NavigateToViewModel<SignedOutFarewellViewModel>(message.Person.DisplayName);
		}
	}
}