using System;
using ToastHelper;
using Windows.UI.Notifications;

namespace OxHack.SignInKiosk.Services
{
	public class ToastService
	{
		private readonly ToastNotifier toastNotifier;

		public ToastService(ToastNotifier toastNotifier)
		{
			this.toastNotifier = toastNotifier;
		}

		internal void Show(string title, string text)
		{
			var content = new ToastContent.Text02()
			{
				Title = title,
				Text = text,
			};
			this.toastNotifier.Show(content.CreateNotification());
		}

		internal void ShowGenericError()
		{
			this.Show("Something went wrong.", "Please try again, or speak with the KeyHolder about signing in or out manually.");
		}
	}
}
