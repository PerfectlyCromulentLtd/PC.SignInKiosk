using OxHack.SignInKiosk.Messaging.Models;
using System;
using System.Runtime.Serialization;

namespace OxHack.SignInKiosk.Messaging.Messages
{
	[DataContract]
	public class PersonSignedIn
	{
		public PersonSignedIn(DateTime time, Person person)
		{
			this.Time = time;
			this.Person = person;
		}

		[DataMember]
		public DateTime Time
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
