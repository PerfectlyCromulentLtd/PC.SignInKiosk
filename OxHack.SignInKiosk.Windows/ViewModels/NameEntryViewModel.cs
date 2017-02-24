using Caliburn.Micro;
using OxHack.SignInKiosk.MessageBrokerProxy;
using OxHack.SignInKiosk.Services;
using OxHack.SignInKiosk.Views;
using System;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml.Navigation;

namespace OxHack.SignInKiosk.ViewModels
{
	public class NameEntryViewModel : Screen
	{
		private readonly INavigationService navigationService;
		private readonly MessageBrokerService messageBroker;
		private readonly UserInfoService userInfoService;

		private string uniqueId;
		private string name;

		public NameEntryViewModel(INavigationService navigationService, MessageBrokerService messageBroker, UserInfoService userInfoService)
		{
			this.navigationService = navigationService;
			this.messageBroker = messageBroker;
			this.userInfoService = userInfoService;

			this.CanSubmitName = true;
		}

		public void GoBack()
		{
			this.navigationService.GoBack();
		}

		public bool CanSubmitName
		{
			get;
			set;
		}

		public async void SubmitName()
		{
			var name = this.Name?.Trim();
			if (String.IsNullOrWhiteSpace(name))
			{
				// sacrilege:
				var dialog = new MessageDialog("Please try entering some characters this time.", "Bleep blorp");
				await dialog.ShowAsync();
			}
			else
			{
				this.CanSubmitName = false;
				this.NotifyOfPropertyChange(nameof(this.CanSubmitName));

				var person = new Person()
				{
					TokenId = this.uniqueId,
					DisplayName = name
				};

				await this.messageBroker.PublishSignInRequestSubmitted(
					new SignInRequestSubmitted()
					{
						Person = person
					});

				this.userInfoService.AddUser(person);
			}
		}

		internal async Task Reset(string tokenId = null)
		{
			bool popUpRequired = (tokenId != null);
			this.uniqueId = tokenId ?? Guid.NewGuid().ToString();

			if (popUpRequired)
			{
				// more sacrilege:
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

					if (value.Contains("\n"))
					{
						this.SubmitName();
					}
				}
			}
		}
	}
}
