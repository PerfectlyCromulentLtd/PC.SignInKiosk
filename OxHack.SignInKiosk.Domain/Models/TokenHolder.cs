using System;
using System.Collections.Generic;
using System.Text;

namespace OxHack.SignInKiosk.Domain.Models
{
    public class TokenHolder
    {
		public string TokenId
		{
			get;
			set;
		}

		public string DisplayName
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
