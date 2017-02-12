using OxHack.SignInKiosk.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace OxHack.SignInKiosk.MessageBrokerProxyService.SubServices.WcfService
{
	[ServiceContract(CallbackContract = typeof(IMessageBrokerProxyServiceCallback))]
	public interface IMessageBrokerProxyService
	{
		[OperationContract]
		void PublishSignInRequestSubmitted(SignInRequestSubmitted message);

		[OperationContract]
		void Subscribe();

		[OperationContract]
		void Unsubscribe();
	}
}
