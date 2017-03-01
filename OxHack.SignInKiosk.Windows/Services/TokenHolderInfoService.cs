using OxHack.SignInKiosk.Domanin.Models;
using System;
using System.Collections.Generic;

namespace OxHack.SignInKiosk.Services
{
	public class TokenHolderInfoService
	{
		Dictionary<string, TokenHolder> usersByTokenId;

		public TokenHolderInfoService()
		{
			this.usersByTokenId = new Dictionary<string, TokenHolder>();
		}

		public TokenHolder GetUserByTokenId(string tokenId)
		{
			//TODO: Replace this with a call to the backend system
			TokenHolder result = null;
			this.usersByTokenId.TryGetValue(tokenId, out result);

			return result;
		}

		[Obsolete("This is just a dummy method to help simulate real behaviour.  It will need to be deleted.")]
		public void AddOrUpdateUser(TokenHolder tokenHolder)
		{
			if (tokenHolder == null)
			{
				throw new ArgumentNullException();
			}

			if (!string.IsNullOrWhiteSpace(tokenHolder.TokenId))
			{
				this.usersByTokenId[tokenHolder.TokenId] = tokenHolder;
			}
		}
	}
}
