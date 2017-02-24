using Caliburn.Micro;
using System;
using System.Threading.Tasks;

namespace OxHack.SignInKiosk.ViewModels
{
	public class SignedInGreetingViewModel : PropertyChangedBase
	{
		private readonly INavigationService navigationService;
		private string name;

		public SignedInGreetingViewModel(INavigationService navigationService)
		{
			this.navigationService = navigationService;
		}

		public async Task BeginDelayedNavigation()
		{
			await Task.Delay(TimeSpan.FromSeconds(3));
			this.navigationService.NavigateToViewModel<StartViewModel>();
		}

		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
				this.NotifyOfPropertyChange();
			}
		}
	}
}
