using System;
using System.Collections.Generic;
using System.Text;

namespace OxHack.SignInKiosk.Database.Models
{
    class SignedInRecord
	{
		public string DisplayName
		{
			get;
			set;
		}

		public DateTime SignInTime
		{
			get;
			set;
		}

		public string TokenId
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
