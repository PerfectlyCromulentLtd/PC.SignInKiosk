using Caliburn.Micro;
using OxHack.SignInKiosk.Domain.Messages;
using OxHack.SignInKiosk.Domain.Messages.Models;
using OxHack.SignInKiosk.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OxHack.SignInKiosk.Services
{
	public class SignInService
	{
		private object syncLock = new object();
		private readonly MessageBrokerService messageBrokerService;

		private readonly SignInApiWrapper apiWrapper;

		public SignInService(
			MessageBrokerService messageBrokerService,
			SignInApiWrapper apiWrapper,
			IEventAggregator eventAggregator)
		{
			this.messageBrokerService = messageBrokerService;
			this.apiWrapper = apiWrapper;
			eventAggregator.Subscribe(this);
		}

		public async Task RequestSignIn(string tokenId, string displayName, bool isVisitor)
		{
			await this.RequestSignIn(
				new TokenHolder()
				{
					DisplayName = displayName,
					IsVisitor = isVisitor,
					TokenId = tokenId
				});
		}

		public async Task RequestSignIn(TokenHolder tokenHolder)
		{
			await this.messageBrokerService.Publish(new SignInRequestSubmitted(new Person(tokenHolder.TokenId, tokenHolder.DisplayName, tokenHolder.IsVisitor)));
		}

		public async Task RequestSignOut(SignedInRecord signedInRecord)
		{
			await this.messageBrokerService.Publish(new SignOutRequestSubmitted(signedInRecord));
		}

		public bool TryGetSignedInRecord(string tokenId, out SignedInRecord result)
		{
			result = null;
			if (String.IsNullOrWhiteSpace(tokenId))
			{
				return false;
			}
			var task = Task.Run(async () => await this.apiWrapper.GetCurrentlySignedIn());
			result = task.Result.SingleOrDefault(item => item.TokenId == tokenId);

			return result != null;
		}

		public async Task<IReadOnlyList<SignedInRecord>> GetCurrentlySignedIn()
		{
			return await this.apiWrapper.GetCurrentlySignedIn();
		}
	}
}
