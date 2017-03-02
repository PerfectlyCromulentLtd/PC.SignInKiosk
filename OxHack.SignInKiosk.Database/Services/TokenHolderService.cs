using OxHack.SignInKiosk.Domain.Models;
using System;
using System.Linq;

namespace OxHack.SignInKiosk.Database.Services
{
	public class TokenHolderService
	{
		private readonly IDbConfig dbConfig;

		public TokenHolderService(IDbConfig dbConfig)
		{
			this.dbConfig = dbConfig;
		}

		public void AddTokenHolder(TokenHolder tokenHolder)
		{
			if (tokenHolder == null)
			{
				throw new ArgumentNullException();
			}

			if (String.IsNullOrWhiteSpace(tokenHolder.TokenId) || String.IsNullOrWhiteSpace(tokenHolder.DisplayName))
			{
				throw new ArgumentException();
			}

			using (var context = this.GetDbContext())
			{
				if (!context.TokenHolders.Any(item => item.TokenId == tokenHolder.TokenId))
				{
					context.TokenHolders.Add(tokenHolder);
					context.SaveChanges();
				}
			}
		}

		public TokenHolder GetTokenHolderByTokenId(string tokenId)
		{
			if (String.IsNullOrWhiteSpace(tokenId))
			{
				throw new ArgumentException();
			}

			using (var context = this.GetDbContext())
			{
				return context.TokenHolders.SingleOrDefault(item => item.TokenId == tokenId);
			}
		}

		private SignInContext GetDbContext()
		{
			return new SignInContext(this.dbConfig);
		}
	}
}
