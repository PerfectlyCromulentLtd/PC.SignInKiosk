using Caliburn.Micro;

namespace OxHack.SignInKiosk.ViewModels
{
	public class StartViewModel : Screen
	{
		private readonly INavigationService navigationService;

		public StartViewModel(INavigationService navigationService)
		{
			this.navigationService = navigationService;
		}

		protected override void OnActivate()
		{
			base.OnActivate();
			this.navigationService.BackStack.Clear();
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
