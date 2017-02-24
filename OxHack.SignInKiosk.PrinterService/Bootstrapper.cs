using OxHack.SignInKiosk.Messaging;
using OxHack.SignInKiosk.Messaging.Messages;
using OxHack.SignInKiosk.PrinterService.SubServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OxHack.SignInKiosk.PrinterService
{
	class Bootstrapper
	{
		private readonly SignInEventPrinter signInEventPrinter;

		public Bootstrapper()
		{
			this.signInEventPrinter = 
				new SignInEventPrinter(
					new MessagingClient(subscriptions: new[] { typeof(PersonSignedIn), typeof(PersonSignedOut) }));
		}

		public async Task Start()
		{
			await this.signInEventPrinter.Start();
		}

		public async Task Stop()
		{
			await this.signInEventPrinter.Stop();
		}
	}
}
