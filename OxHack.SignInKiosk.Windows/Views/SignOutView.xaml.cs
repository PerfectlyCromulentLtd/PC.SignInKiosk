using OxHack.SignInKiosk.Services;
using OxHack.SignInKiosk.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace OxHack.SignInKiosk.Views
{
	public sealed partial class SignOutView : Page
	{
		private SignOutViewModel viewModel;

		public SignOutView()
		{
			this.InitializeComponent();
		}

		protected async override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			this.viewModel = this.DataContext as SignOutViewModel;

			await this.viewModel.LoadSignOutList(e.Parameter as string);
		}
	}
}
