using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace OxHack.SignInKiosk.Domanin.Messages.Models
{
	[DataContract]
	public class Person
	{
		public Person(string tokenId, string displayName, bool isVisitor)
		{
			this.TokenId = tokenId;
			this.DisplayName = displayName;
			this.IsVisitor = isVisitor;
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

		[DataMember]
		public bool IsVisitor
		{
			get;
			private set;
		}
	}
}
