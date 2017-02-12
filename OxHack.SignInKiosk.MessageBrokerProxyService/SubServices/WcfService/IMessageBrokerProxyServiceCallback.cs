using OxHack.SignInKiosk.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace OxHack.SignInKiosk.MessageBrokerProxyService.SubServices.WcfService
{
	public interface IMessageBrokerProxyServiceCallback
	{
		[OperationContract]
		void OnTokenReadPublished(TokenRead message);

		[OperationContract]
		void OnPersonSignedInPublished(PersonSignedIn message);

		[OperationContract]
		void OnPersonSignedOutPublished(PersonSignedOut message);

		[OperationContract]
		void OnSignInRequestSubmittedPublished(SignInRequestSubmitted message);
	}
}
