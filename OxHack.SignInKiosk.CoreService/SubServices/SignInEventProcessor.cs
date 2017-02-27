using OxHack.SignInKiosk.Messaging;
using OxHack.SignInKiosk.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OxHack.SignInKiosk.CoreService.SubServices
{
	class SignInEventProcessor
	{
		private readonly MessagingClient messagingClient;

		public SignInEventProcessor(MessagingClient messagingClient)
		{
			this.messagingClient = messagingClient;
			this.messagingClient.SignInRequestSubmitted += this.OnSignInRequestSubmitted;
			this.messagingClient.SignOutRequestSubmitted += this.OnSignOutRequestSubmitted;
		}

		private void OnSignInRequestSubmitted(object sender, SignInRequestSubmitted e)
		{
			throw new NotImplementedException();
		}

		private void OnSignOutRequestSubmitted(object sender, SignOutRequestSubmitted e)
		{
			throw new NotImplementedException();
		}

		internal async Task Start()
		{
			await this.messagingClient.Connect();
		}

		internal async Task Stop()
		{
			await this.messagingClient.Disconnect();
		}
	}
}
