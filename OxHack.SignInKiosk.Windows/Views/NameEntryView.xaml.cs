using Caliburn.Micro;
using OxHack.SignInKiosk.Services;
using OxHack.SignInKiosk.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace OxHack.SignInKiosk.Views
{
	public sealed partial class NameEntryView : Page
	{
		public NameEntryView()
		{
			this.InitializeComponent();

			this.Loaded += (s, e) =>
			{
				this.nameInput.Focus(FocusState.Programmatic);
			};
		}

		protected async override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			this.ViewModel = this.DataContext as NameEntryViewModel;

			var tokenId = e.Parameter as string;
			await this.ViewModel.Reset(tokenId);
		}

		public NameEntryViewModel ViewModel
		{
			get;
			private set;
		}
	}
}
