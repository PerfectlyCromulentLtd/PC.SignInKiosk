using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace OxHack.SignInKiosk.Messaging.Models
{
	[DataContract]
	public class Person
	{
		public Person(string tokenId, string displayName)
		{
			this.TokenId = tokenId;
			this.DisplayName = displayName;
		}

		[DataMember]
		public string TokenId
		{
			get;
			private set;
		}

		[DataMember]
		public string DisplayName
		{
			get;
			private set;
		}
	}
}
