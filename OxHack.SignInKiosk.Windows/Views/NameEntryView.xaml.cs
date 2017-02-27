using Caliburn.Micro;
using OxHack.SignInKiosk.Services;
using OxHack.SignInKiosk.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
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

				var binding = new Binding()
				{
					Path = new PropertyPath(nameof(NameEntryViewModel.ShowMembershipStatusPicker)),
					Mode = BindingMode.TwoWay					
				};

				BindingOperations.SetBinding(
					this,
					NameEntryView.ShowMembershipStatusPickerProperty,
					binding);
			};

			this.nameInput.KeyDown += (s, e) =>
			{
				if (e.Key == Windows.System.VirtualKey.Enter)
				{
					this.Focus(FocusState.Programmatic);
					this.ViewModel.SubmitName();
				}
			};

			this.membershipStatusPicker.Closed += (s, e) => this.ShowMembershipStatusPicker = false;
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

		public bool ShowMembershipStatusPicker
		{
			get { return (bool)GetValue(ShowMembershipStatusPickerProperty); }
			set { SetValue(ShowMembershipStatusPickerProperty, value); }
		}

		public static readonly DependencyProperty ShowMembershipStatusPickerProperty =
			DependencyProperty.Register(
				nameof(NameEntryView.ShowMembershipStatusPicker),
				typeof(bool),
				typeof(NameEntryView),
				new PropertyMetadata(
					false,
					(d, e) =>
					{
						var sender = ((NameEntryView)d);
						if ((bool)e.NewValue)
						{
							FlyoutBase.ShowAttachedFlyout(sender);
						}
						else
						{
							sender.membershipStatusPicker.Hide();
						};
					}
					));
	}
}
