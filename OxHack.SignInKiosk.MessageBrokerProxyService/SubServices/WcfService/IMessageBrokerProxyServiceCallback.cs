using OxHack.SignInKiosk.Domain.Messages;
using System.ServiceModel;

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

		[OperationContract]
		void OnSignOutRequestSubmittedPublished(SignOutRequestSubmitted message);

		[OperationContract]
		void KeepCallbackAlive();
	}
}
