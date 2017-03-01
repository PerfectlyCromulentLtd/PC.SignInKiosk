using OxHack.SignInKiosk.Domanin.Messages.Models;
using System;
using System.Runtime.Serialization;

namespace OxHack.SignInKiosk.Domanin.Messages
{
	[DataContract]
	public class PersonSignedOut
	{
		public PersonSignedOut(DateTime signInTime, DateTime signOutTime, Person person)
		{
			this.SignInTime = signInTime;
			this.SignOutTime = signOutTime;
			this.Person = person;
		}

		[DataMember]
		public DateTime SignInTime
		{
			get;
			private set;
		}

		[DataMember]
		public DateTime SignOutTime
		{
			get;
			private set;
		}

		[DataMember]
		public Person Person
		{
			get;
			private set;
		}
	}
}
