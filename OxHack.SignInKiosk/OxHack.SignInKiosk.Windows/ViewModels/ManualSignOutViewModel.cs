using Caliburn.Micro;

namespace OxHack.SignInKiosk.ViewModels
{
	public class ManualSignOutViewModel : Screen
	{
		private readonly INavigationService navigationService;

		public ManualSignOutViewModel(INavigationService navigationService)
		{
			this.navigationService = navigationService;
		}

		protected override void OnActivate()
		{
			base.OnActivate();
		}

		public void GoBack()
		{
			this.navigationService.GoBack();
		}

		public void SignOut()
		{
		}
	}
}
