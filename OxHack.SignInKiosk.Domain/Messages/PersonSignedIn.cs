using OxHack.SignInKiosk.Domanin.Messages.Models;
using System;
using System.Runtime.Serialization;

namespace OxHack.SignInKiosk.Domanin.Messages
{
	[DataContract]
	public class PersonSignedIn
	{
		public PersonSignedIn(DateTime signInTime, Person person)
		{
			this.SignInTime = signInTime;
			this.Person = person;
		}

		[DataMember]
		public DateTime SignInTime
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
