using OxHack.SignInKiosk.MessageBrokerProxy;
using System;
using System.Collections.Generic;

namespace OxHack.SignInKiosk.Services
{
	public class UserInfoService
	{
		Dictionary<string, Person> usersByTokenId;

		public UserInfoService()
		{
			this.usersByTokenId = new Dictionary<string, Person>();
		}

		public Person GetUserByTokenId(string tokenId)
		{
			//TODO: Replace this with a call to the backend system
			Person result = null;
			this.usersByTokenId.TryGetValue(tokenId, out result);

			return result;
		}

		[Obsolete("This is just a dummy method to help simulate real behaviour.  It will need to be deleted.")]
		public void AddOrUpdateUser(Person newUser)
		{
			if (newUser == null || string.IsNullOrWhiteSpace(newUser.TokenId))
			{
				throw new ArgumentException();
			}

			this.usersByTokenId[newUser.TokenId] = newUser;
		}
	}
}
