using System;
using Caliburn.Micro;
using OxHack.SignInKiosk.Services;
using System.Linq;
using OxHack.SignInKiosk.MessageBrokerProxy;
using System.Collections.Generic;
using static OxHack.SignInKiosk.Services.SignInService;

namespace OxHack.SignInKiosk.ViewModels
{
	public class ManualSignOutViewModel : Screen
	{
		private readonly INavigationService navigationService;
		private readonly SignInService signInService;

		public ManualSignOutViewModel(INavigationService navigationService, SignInService signInService)
		{
			this.navigationService = navigationService;
			this.signInService = signInService;
		}

		public void GoBack()
		{
			this.navigationService.GoBack();
		}

		public void SignOut()
		{
		}

		internal void LoadSignOutList(string selectedPersonTokenId = null)
		{
			var people = this.signInService.GetPeopleSignedIn();

			this.SignInRecords = people.ToList();
			this.NotifyOfPropertyChange(nameof(this.SignInRecords));

			if (selectedPersonTokenId != null)
			{
				this.SelectedSignInRecord = this.SignInRecords.FirstOrDefault(person => person.TokenId == selectedPersonTokenId);
				this.NotifyOfPropertyChange(nameof(this.SelectedSignInRecord));
			}
		}

		public List<SignInRecord> SignInRecords
		{
			get;
			private set;
		}

		public SignInRecord SelectedSignInRecord
		{
			get;
			set;
		}
	}
}
