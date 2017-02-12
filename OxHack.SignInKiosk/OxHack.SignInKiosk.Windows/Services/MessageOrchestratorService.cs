using Caliburn.Micro;
using OxHack.SignInKiosk.Events;
using OxHack.SignInKiosk.MessageBrokerProxy;
using OxHack.SignInKiosk.ViewModels;
using OxHack.SignInKiosk.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OxHack.SignInKiosk.Services
{
	class MessageOrchestratorService :
		IHandle<PersonSignedIn>,
		IHandle<Disconnected>,
		IHandle<Connected>,
		IHandle<TokenRead>
	{
		private readonly INavigationService navigationService;
		private readonly MessageBrokerService messageBrokerService;
		private readonly UserInfoService userInfoService;

		public MessageOrchestratorService(
			INavigationService navigationService,
			MessageBrokerService messageBrokerService,
			UserInfoService userInfoService, 
			IEventAggregator eventAggregator)
		{
			this.navigationService = navigationService;
			this.messageBrokerService = messageBrokerService;
			this.userInfoService = userInfoService;

			eventAggregator.Subscribe(this);
		}

		public async void Handle(TokenRead message)
		{
			var person = this.userInfoService.GetUserByTokenId(message.Id);

			if (person == null)
			{
				this.navigationService.NavigateToViewModel<NameEntryViewModel>(message.Id);
			}
			else
			{
				await this.messageBrokerService.PublishSignInRequestSubmitted(new SignInRequestSubmitted() { Person = person });
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