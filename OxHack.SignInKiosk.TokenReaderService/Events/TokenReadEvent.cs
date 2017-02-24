using OxHack.SignInKiosk.Messaging.Messages;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OxHack.SignInKiosk.TokenReaderService.Events
{
	class TokenReadEvent : PubSubEvent<uint>
	{
	}
}
