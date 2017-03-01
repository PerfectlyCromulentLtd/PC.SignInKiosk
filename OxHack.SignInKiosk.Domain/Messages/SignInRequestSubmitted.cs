using OxHack.SignInKiosk.Domanin.Messages.Models;
using System.Runtime.Serialization;

namespace OxHack.SignInKiosk.Domanin.Messages
{
	[DataContract]
	public class SignInRequestSubmitted
	{
		public SignInRequestSubmitted(Person person)
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