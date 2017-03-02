using Caliburn.Micro;
using OxHack.SignInKiosk.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OxHack.SignInKiosk.ViewModels
{
	public class SignOutViewModel : Screen
	{
		private readonly INavigationService navigationService;
		private readonly SignInService signInService;
		private readonly ToastService toastService;
		private SignedInRecordViewModel selectedSignInRecord;
		private bool isBusy;

		public SignOutViewModel(
			INavigationService navigationService,
			SignInService signInService,
			ToastService toastService)
		{
			this.navigationService = navigationService;
			this.signInService = signInService;
			this.toastService = toastService;
		}

		public void GoBack()
		{
			this.navigationService.GoBack();
		}

		public bool CanSignOut
			=> this.SelectedSignInRecord != null && !this.IsBusy;

		public async void SignOut()
		{
			if (this.CanSignOut)
			{
				this.IsBusy = true;
				try
				{
					await this.signInService.RequestSignOut(this.SelectedSignInRecord.Model);
				}
				catch (Exception e)
				{
					// TODO: Log error.
					this.toastService.ShowGenericError();
					this.IsBusy = false;
				}
			}
		}

		internal async Task LoadSignOutList(string selectedTokenId = null)
		{
			try
			{
				var currentlySignedIn = await this.signInService.GetCurrentlySignedIn();

				this.SignInRecords = currentlySignedIn.Select(item => new SignedInRecordViewModel(item)).ToList();
				this.NotifyOfPropertyChange(nameof(this.SignInRecords));

				if (selectedTokenId != null)
				{
					this.SelectedSignInRecord = this.SignInRecords.FirstOrDefault(record => record.TokenId == selectedTokenId);
					this.NotifyOfPropertyChange(nameof(this.SelectedSignInRecord));
				}
			}
			catch (Exception e)
			{
				// TODO: Log error.
				this.toastService.ShowGenericError();
				this.GoBack();
			}
		}

		public List<SignedInRecordViewModel> SignInRecords
		{
			get;
			private set;
		}

		public SignedInRecordViewModel SelectedSignInRecord
		{
			get
			{
				return this.selectedSignInRecord;
			}
			set
			{
				this.selectedSignInRecord = value;
				this.NotifyOfPropertyChange();
				this.NotifyOfPropertyChange(nameof(this.CanSignOut));
			}
		}

		public bool IsBusy
		{
			get
			{
				return this.isBusy;
			}
			private set
			{
				this.isBusy = value;
				this.NotifyOfPropertyChange();
				this.NotifyOfPropertyChange(nameof(this.CanSignOut));
			}
		}
	}
}
