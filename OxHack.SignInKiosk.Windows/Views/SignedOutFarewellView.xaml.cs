using OxHack.SignInKiosk.Services;
using OxHack.SignInKiosk.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace OxHack.SignInKiosk.Views
{
	public sealed partial class SignedOutFarewellView : Page
	{
		public SignedOutFarewellView()
		{
			this.InitializeComponent();
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			var viewModel = this.DataContext as SignedOutFarewellViewModel;
			viewModel.Name = e.Parameter as string;
		}
	}
}
