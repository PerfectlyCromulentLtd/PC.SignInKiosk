using OxHack.SignInKiosk.Services;
using OxHack.SignInKiosk.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace OxHack.SignInKiosk.Views
{
	public sealed partial class ManualSignOutView : Page
	{
		private ManualSignOutViewModel viewModel;

		public ManualSignOutView()
		{
			this.InitializeComponent();
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			this.viewModel = this.DataContext as ManualSignOutViewModel;

			this.viewModel.LoadSignOutList(e.Parameter as string);
		}
	}
}
