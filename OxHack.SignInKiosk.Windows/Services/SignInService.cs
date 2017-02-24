using Caliburn.Micro;
using OxHack.SignInKiosk.MessageBrokerProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OxHack.SignInKiosk.Services
{
	public class SignInService :
		IHandle<PersonSignedIn>,
		IHandle<PersonSignedOut>
	{
		private object syncLock = new object();
		private readonly MessageBrokerService messageBrokerService;
		private readonly Dictionary<string, SignInRecord> signInRecordsByTokenId;

		public SignInService(MessageBrokerService messageBrokerService, IEventAggregator eventAggregator)
		{
			this.messageBrokerService = messageBrokerService;
			eventAggregator.Subscribe(this);

			this.signInRecordsByTokenId = new Dictionary<string, SignInRecord>();
		}

		public async Task RequestSignIn(Person person)
		{
			await this.messageBrokerService.PublishSignInRequestSubmitted(new SignInRequestSubmitted() { Person = person });
		}

		public bool IsSignedIn(Person person)
		{
			return this.signInRecordsByTokenId.ContainsKey(person.TokenId);
		}

		public IReadOnlyCollection<SignInRecord> GetPeopleSignedIn()
		{
			return this.signInRecordsByTokenId.Values.ToList();
		}

		public void Handle(PersonSignedIn message)
		{
			// TODO: Remove all this once the sign-in server is up
			var person = message.Person;
			lock (this.syncLock)
			{
				if (!this.signInRecordsByTokenId.ContainsKey(person.TokenId))
				{
					//this.signInRecordsByTokenId.Add(person.TokenId, new SignInRecord(message.Time, message.Person));
				}
			}
		}

		public void Handle(PersonSignedOut message)
		{
			// TODO: Remove all this once the sign-in server is up
			var person = message.Person;
			lock (this.syncLock)
			{
				if (this.signInRecordsByTokenId.ContainsKey(person.TokenId))
				{
					this.signInRecordsByTokenId.Remove(person.TokenId);
				}
			}
		}

		public class SignInRecord
		{
			public SignInRecord(DateTime time, Person person)
			{
				this.Time = time;
				this.Person = person;
			}

			public DateTime Time
			{
				get;
			}

			public Person Person
			{
				get;
			}
		}
	}
}
