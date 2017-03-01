using Caliburn.Micro;
using System;
using System.Threading.Tasks;

namespace OxHack.SignInKiosk.ViewModels
{
	public class SignedOutFarewellViewModel : Screen
	{
		private readonly INavigationService navigationService;
		private string name;

		public SignedOutFarewellViewModel(INavigationService navigationService)
		{
			this.navigationService = navigationService;
		}

		protected override async void OnActivate()
		{
			base.OnActivate();
			await Task.Delay(TimeSpan.FromSeconds(2));
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
