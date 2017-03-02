using OxHack.SignInKiosk.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OxHack.SignInKiosk.Services
{
	[Obsolete("This seems like a pointless abstraction.  Remove it if it is truly unneeded.")]
	public class TokenHolderService
	{
		private readonly TokenHolderApiWrapper apiWrapper;

		public TokenHolderService(TokenHolderApiWrapper apiWrapper)
		{
			this.apiWrapper = apiWrapper;
		}

		public async Task<TokenHolder> GetTokenHolderByTokenId(string tokenId)
		{
			return await this.apiWrapper.GetTokenHolderByTokenId(tokenId);
		}
	}
}