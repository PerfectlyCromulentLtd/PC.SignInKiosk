using System;
using System.Collections.Generic;
using System.Text;

namespace OxHack.SignInKiosk.Domanin.Models
{
    public class SignedInRecord
	{
		public Guid Id
		{
			get;
			set;
		}

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
