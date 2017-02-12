using OxHack.SignInKiosk.MessageBrokerProxy;
using System.Collections.Generic;

namespace OxHack.SignInKiosk.Services
{
	class UserInfoService
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
	}
}
