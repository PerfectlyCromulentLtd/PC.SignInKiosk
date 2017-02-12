using Caliburn.Micro;

namespace OxHack.SignInKiosk.ViewModels
{
	public class StartViewModel : PropertyChangedBase
	{
		private readonly INavigationService navigationService;

		public StartViewModel(INavigationService navigationService)
		{
			this.navigationService = navigationService;
		}

		public void SignIn()
		{
			this.navigationService.NavigateToViewModel<NameEntryViewModel>();
		}

		public void SignOut()
		{
			this.navigationService.NavigateToViewModel<ManualSignOutViewModel>();
		}
	}
}
