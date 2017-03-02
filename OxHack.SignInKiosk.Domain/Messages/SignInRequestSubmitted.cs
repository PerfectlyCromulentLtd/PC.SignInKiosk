using OxHack.SignInKiosk.Domain.Messages.Models;
using System.Runtime.Serialization;

namespace OxHack.SignInKiosk.Domain.Messages
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