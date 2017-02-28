using System;
using Humanizer;
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

		internal void LoadSignOutList(string selectedTokenId = null)
		{
			var people = this.signInService.GetPeopleSignedIn();

			this.SignInRecords = people.Select(item => new SignedInRecordViewModel(item)).ToList();
			this.NotifyOfPropertyChange(nameof(this.SignInRecords));

			if (selectedTokenId != null)
			{
				this.SelectedSignInRecord = this.SignInRecords.FirstOrDefault(record => record.TokenId == selectedTokenId);
				this.NotifyOfPropertyChange(nameof(this.SelectedSignInRecord));
			}
		}

		public List<SignedInRecordViewModel> SignInRecords
		{
			get;
			private set;
		}

		public SignedInRecordViewModel SelectedSignInRecord
		{
			get;
			set;
		}

		public class SignedInRecordViewModel
		{
			private readonly SignedInRecord model;

			public SignedInRecordViewModel(SignedInRecord model)
			{
				this.model = model;
			}

			public string DisplayName
				=> this.model.DisplayName;

			public string SignInTime
				=> $"Signed-in @ {this.model.SignInTime.ToString("t")}{(this.model.SignInTime.DayOfWeek != DateTime.Now.DayOfWeek ? " (" + this.model.SignInTime.Humanize() + ") " : String.Empty)}";

			public string AdditionalInformation
				=> $"[ {(this.model.IsVisitor ? "visitor" : "member")} ]{(this.model.TokenId != null ? " [ signed-in with fob ]" : String.Empty)}";

			internal string TokenId
				=> this.model.TokenId;
		}
	}
}
