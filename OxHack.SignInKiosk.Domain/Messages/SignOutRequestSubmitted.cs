using OxHack.SignInKiosk.Domanin.Models;
using System;
using System.Runtime.Serialization;

namespace OxHack.SignInKiosk.Domanin.Messages
{
	[DataContract]
	public class SignOutRequestSubmitted
	{
		public SignOutRequestSubmitted(SignedInRecord signedInRecord)
		{
			this.SignedInRecord = signedInRecord;
		}

		[DataMember]
		public SignedInRecord SignedInRecord
		{
			get;
			private set;
		}
	}
}
