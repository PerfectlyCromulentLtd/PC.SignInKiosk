using System;
using System.Collections.Generic;
using System.Text;

namespace OxHack.SignInKiosk.Domanin.Models
{
    public class TokenHolder
    {
		public TokenHolder(string tokenId, string displayName, bool isVisitor)
		{
			this.TokenId = tokenId;
			this.DisplayName = displayName;
			this.IsVisitor = isVisitor;
		}

		public string TokenId
		{
			get;
			private set;
		}

		public string DisplayName
		{
			get;
			private set;
		}

		public bool IsVisitor
		{
			get;
			private set;
		}
	}
}
