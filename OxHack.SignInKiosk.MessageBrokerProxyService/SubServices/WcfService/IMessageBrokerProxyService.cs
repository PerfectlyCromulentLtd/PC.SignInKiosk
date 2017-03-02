using OxHack.SignInKiosk.Domain.Messages;
using System.ServiceModel;

namespace OxHack.SignInKiosk.MessageBrokerProxyService.SubServices.WcfService
{
	[ServiceContract(CallbackContract = typeof(IMessageBrokerProxyServiceCallback))]
	public interface IMessageBrokerProxyService
	{
		[OperationContract]
		void PublishSignInRequest(SignInRequestSubmitted message);

		[OperationContract]
		void PublishSignOutRequest(SignOutRequestSubmitted message);

		[OperationContract]
		void Subscribe();

		[OperationContract]
		void Unsubscribe();
	}
}
