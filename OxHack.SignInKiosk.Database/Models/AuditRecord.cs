using System;
using System.Collections.Generic;
using System.Text;

namespace OxHack.SignInKiosk.Database.Models
{
    class AuditRecord
	{
		internal const string SignInRecordType = "SignIn";
		internal const string SignOutRecordType = "SignOut";

		public int SequenceNumber
		{
			get;
			set;
		}

		public string RecordType
		{
			get;
			set;
		}

		public DateTime Time
		{
			get;
			set;
		}

		public string PersonTokenId
		{
			get;
			set;
		}

		public string PersonDisplayName
		{
			get;
			set;
		}

		public bool PersonIsVisitor
		{
			get;
			set;
		}
    }
}
