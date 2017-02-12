using Caliburn.Micro;
using OxHack.SignInKiosk.MessageBrokerProxy;
using OxHack.SignInKiosk.Services;
using System;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace OxHack.SignInKiosk.ViewModels
{
	public class NameEntryViewModel : Screen
	{
		private readonly INavigationService navigationService;
		private string uniqueId;
		private readonly MessageBrokerService messageBroker;

		public NameEntryViewModel(INavigationService navigationService, MessageBrokerService messageBroker)
		{
			this.navigationService = navigationService;
			this.messageBroker = messageBroker;
		}

		public void GoBack()
		{
			this.navigationService.GoBack();
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
				await this.messageBroker.PublishSignInRequestSubmitted(
					new SignInRequestSubmitted()
					{
						Person = new Person()
						{
							TokenId = this.uniqueId,
							DisplayName = name
						}
					});
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
			this.NotifyOfPropertyChange(nameof(this.Name));
		}

		public string Name
		{
			get;
			set;
		}
	}
}
