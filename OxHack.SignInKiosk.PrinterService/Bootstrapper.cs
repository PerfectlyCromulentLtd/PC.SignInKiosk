using OxHack.SignInKiosk.Messaging;
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
		private readonly SignInEventPrinter messageListener;

		public Bootstrapper()
		{
			this.messageListener = new SignInEventPrinter(new MessagingClient());
		}

		public async Task Start()
		{
			await this.messageListener.Start();
		}

		public async Task Stop()
		{
			await this.messageListener.Stop();
		}
	}
}
