using OxHack.SignInKiosk.Messaging.Models;
using System.Runtime.Serialization;

namespace OxHack.SignInKiosk.Messaging.Messages
{
	[DataContract]
	public class PersonSignedOut
	{
		public PersonSignedOut(Person person)
		{
			this.Person = person;
		}

		[DataMember]
		public Person Person
		{
			get;
			private set;
		}
	}
}
