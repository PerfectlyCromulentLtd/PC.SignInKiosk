using System;
using System.Collections.Generic;
using System.Text;

namespace OxHack.SignInKiosk.Database.Models
{
    class AuditRecord
	{
		public string RecordType
		{
			get;
			set;
		}

		public string PersonIdentifier
		{
			get;
			set;
		}


		public bool IsGuest
		{
			get;
			set;
		}
    }
}
