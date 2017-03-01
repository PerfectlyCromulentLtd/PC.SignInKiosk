using Caliburn.Micro;
using OxHack.SignInKiosk.Domanin.Messages;
using OxHack.SignInKiosk.Domanin.Messages.Models;
using OxHack.SignInKiosk.Domanin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OxHack.SignInKiosk.Services
{
	public class SignInService :
		IHandle<PersonSignedIn>,
		IHandle<PersonSignedOut>
	{
		private object syncLock = new object();
		private readonly MessageBrokerService messageBrokerService;
		private readonly TokenHolderInfoService tokenHolderInfoService;

		private readonly List<SignedInRecord> signedInRecords;
		private readonly SignInApiWrapper signInApiWrapper;

		public SignInService(
			MessageBrokerService messageBrokerService,
			TokenHolderInfoService tokenHolderInfoService,
			SignInApiWrapper signInApiWrapper,
			IEventAggregator eventAggregator)
		{
			this.messageBrokerService = messageBrokerService;
			this.tokenHolderInfoService = tokenHolderInfoService;
			this.signInApiWrapper = signInApiWrapper;
			eventAggregator.Subscribe(this);

			this.signedInRecords = new List<SignedInRecord>();
		}

		public async Task RequestSignIn(string tokenId, string displayName, bool isVisitor)
		{
			await this.RequestSignIn(new TokenHolder(tokenId, displayName, isVisitor));
		}

		public async Task RequestSignIn(TokenHolder tokenHolder)
		{
			this.tokenHolderInfoService.AddOrUpdateUser(tokenHolder);
			await this.messageBrokerService.Publish(new SignInRequestSubmitted(new Person(tokenHolder.TokenId, tokenHolder.DisplayName, tokenHolder.IsVisitor)));
		}

		public async Task RequestSignOut(SignedInRecord signedInRecord)
		{
			await this.messageBrokerService.Publish(new SignOutRequestSubmitted(signedInRecord));
		}

		public async Task<bool> IsSignedIn(string tokenId)
		{
			return (tokenId != null) && (await this.signInApiWrapper.GetCurrentlySignedIn()).Any(item => item.TokenId == tokenId);
		}

		public async Task<IReadOnlyList<SignedInRecord>> GetCurrentlySignedIn()
		{
			return await this.signInApiWrapper.GetCurrentlySignedIn();
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
	}
}
