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
			//TODO: properly implement this
			Person result = null;
			this.usersByTokenId.TryGetValue(tokenId, out result);

			return result;
		}

		public void AddUser(Person newUser)
		{
			//TODO: properly implement this
			if (newUser == null || string.IsNullOrWhiteSpace(newUser.TokenId) || this.usersByTokenId.ContainsKey(newUser.TokenId))
			{
				throw new ArgumentException();
			}

			this.usersByTokenId.Add(newUser.TokenId, newUser);
		}
	}
}
