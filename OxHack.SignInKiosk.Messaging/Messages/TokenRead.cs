using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace OxHack.SignInKiosk.Messaging.Messages
{
	[DataContract]
	public class TokenRead
	{
		public TokenRead(string id)
		{
			this.Id = id;
		}

		[DataMember]
		public string Id
		{
			get;
			private set;
		}
	}
}
