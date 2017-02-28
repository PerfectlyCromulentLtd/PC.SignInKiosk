using Caliburn.Micro;
using OxHack.SignInKiosk.MessageBrokerProxy;
using OxHack.SignInKiosk.Services;
using OxHack.SignInKiosk.Views;
using Prism.Commands;
using System;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml.Navigation;

namespace OxHack.SignInKiosk.ViewModels
{
	public class NameEntryViewModel : Screen
	{
		private readonly INavigationService navigationService;
		private readonly SignInService signInService;

		private string tokenId;
		private string name;
		private bool showMembershipStatusPicker;
		private bool isVisitor;
		private bool isMember;
		private bool isBusy;

		public NameEntryViewModel(INavigationService navigationService, SignInService signInService)
		{
			this.navigationService = navigationService;
			this.signInService = signInService;

			this.SubmitCommand = new DelegateCommand(this.Submit, this.CanSubmit);
		}

		public void GoBack()
		{
			this.navigationService.GoBack();
		}

		public bool CanSubmitName
		{
			get
			{
				return !String.IsNullOrWhiteSpace(this.Name) && !this.ShowMembershipStatusPicker && !this.IsBusy;
			}
		}

		internal void SubmitName()
		{
			if (this.CanSubmitName)
			{
				this.ShowMembershipStatusPicker = true;
			}
		}

		public DelegateCommand SubmitCommand
		{
			get;
		}

		public bool CanSubmit()
		{
			return (this.IsVisitor || this.IsMember) && !this.IsBusy;
		}

		public async void Submit()
		{
			this.IsBusy = true;
			this.ShowMembershipStatusPicker = false;

			var person = new Person()
			{
				TokenId = this.tokenId,
				DisplayName = this.Name,
				IsVisitor = this.IsVisitor
			};

			await this.signInService.RequestSignIn(person);
		}

		internal async Task Reset(string tokenId)
		{
			bool popUpRequired = (tokenId != null);
			this.tokenId = tokenId;

			if (popUpRequired)
			{
				// sacrilege:
				var dialog = new MessageDialog("It doesn't look like we've seen you before.\n\nPlease take a moment to introduce yourself.", "Are you new?");
				await dialog.ShowAsync();
			}

			this.Name = String.Empty;
		}

		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				if (this.name != value)
				{
					this.name = (value ?? String.Empty).Trim();
					this.NotifyOfPropertyChange();
					this.NotifyOfPropertyChange(nameof(this.CanSubmitName));
				}
			}
		}

		public bool IsVisitor
		{
			get
			{
				return this.isVisitor;
			}
			set
			{
				this.isVisitor = value;
				this.NotifyOfPropertyChange();
				this.SubmitCommand.RaiseCanExecuteChanged();
			}
		}

		public bool IsMember
		{
			get
			{
				return this.isMember;
			}
			set
			{
				this.isMember = value;
				this.NotifyOfPropertyChange();
				this.SubmitCommand.RaiseCanExecuteChanged();
			}
		}

		public bool IsBusy
		{
			get
			{
				return this.isBusy;
			}
			set
			{
				this.isBusy = value;
				this.NotifyOfPropertyChange();
				this.NotifyOfPropertyChange(nameof(this.IsNotBusy));
				this.NotifyOfPropertyChange(nameof(this.CanSubmitName));
				this.SubmitCommand.RaiseCanExecuteChanged();
			}
		}

		public bool IsNotBusy
			=> !this.IsBusy;

		public bool ShowMembershipStatusPicker
		{
			get
			{
				return this.showMembershipStatusPicker;
			}
			set
			{
				if (value != this.ShowMembershipStatusPicker)
				{
					this.showMembershipStatusPicker = value;
					this.NotifyOfPropertyChange();
					this.NotifyOfPropertyChange(nameof(this.CanSubmitName));
				}
			}
		}
	}
}
