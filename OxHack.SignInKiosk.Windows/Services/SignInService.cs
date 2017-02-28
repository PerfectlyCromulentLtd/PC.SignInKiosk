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
		private readonly UserInfoService userInfoService;

		private readonly List<SignedInRecord> signedInRecords;

		public SignInService(MessageBrokerService messageBrokerService, UserInfoService userInfoService, IEventAggregator eventAggregator)
		{
			this.messageBrokerService = messageBrokerService;
			this.userInfoService = userInfoService;
			eventAggregator.Subscribe(this);

			this.signedInRecords = new List<SignedInRecord>();
		}

		public async Task RequestSignIn(Person person)
		{
			this.userInfoService.AddOrUpdateUser(person);
			await this.messageBrokerService.PublishSignInRequestSubmitted(new SignInRequestSubmitted() { Person = person });
		}

		public bool IsSignedIn(string tokenId)
		{
			// TODO: Replace this with a call to the backend system
			return (tokenId != null) && this.signedInRecords.Any(item => item.TokenId == tokenId);
		}

		public IReadOnlyCollection<SignedInRecord> GetPeopleSignedIn()
		{
			// TODO: Replace this with a call to the backend system
			return this.signedInRecords.ToList();
		}

		public void Handle(PersonSignedIn message)
		{
			// TODO: Remove all this once the sign-in server is up
			var person = message.Person;
			lock (this.syncLock)
			{
				if (!IsSignedIn(message.SignInTime, message.Person.DisplayName))
				{
					this.signedInRecords.Add(
						new SignedInRecord()
						{
							DisplayName = message.Person.DisplayName,
							IsVisitor = message.Person.IsVisitor,
							SignInTime = message.SignInTime,
							TokenId = message.Person.TokenId
						});
				}
			}
		}

		public void Handle(PersonSignedOut message)
		{
			// TODO: Remove all this once the sign-in server is up
			var person = message.Person;
			lock (this.syncLock)
			{
				this.signedInRecords.RemoveAll(item => item.SignInTime == message.SignInTime && item.DisplayName == message.Person.DisplayName);
			}
		}

		private bool IsSignedIn(DateTime signInTime, string displayName)
		{
			return this.signedInRecords.Any(item => item.SignInTime == signInTime && item.DisplayName == displayName);
		}

		public class SignedInRecord
		{
			public string DisplayName
			{
				get;
				set;
			}

			public DateTime SignInTime
			{
				get;
				set;
			}

			public string TokenId
			{
				get;
				set;
			}

			public bool IsVisitor
			{
				get;
				set;
			}
		}
	}
}
