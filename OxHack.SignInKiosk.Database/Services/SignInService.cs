using OxHack.SignInKiosk.Database.Models;
using System;
using System.Linq;

namespace OxHack.SignInKiosk.Database.Services
{
	public class SignInService
	{
		private readonly IDbConfig dbConfig;

		public static object LogManager
		{
			get;
			private set;
		}

		public SignInService(IDbConfig dbConfig)
		{
			this.dbConfig = dbConfig;
		}

		private SignInContext GetDbContext()
		{
			return new SignInContext(this.dbConfig);
		}

		public DateTime SignIn(string displayName, bool isVisitor, string tokenId = null)
		{
			if (String.IsNullOrWhiteSpace(displayName))
			{
				throw new ArgumentException(nameof(displayName));
			}

			var signInTime = DateTime.Now;

			using (var context = this.GetDbContext())
			{
				var isAlreadySignedIn =
					context.CurrentlySignedIn.Any(item => tokenId != null && item.TokenId == tokenId);

				if (isAlreadySignedIn)
				{
					throw new InvalidOperationException("Duplicate Sign-In detected.");
				}

				var auditRecord = new AuditRecord()
				{
					PersonDisplayName = displayName,
					PersonTokenId = tokenId,
					PersonIsVisitor = isVisitor,
					RecordType = AuditRecord.SignInRecordType,
					Time = signInTime
				};

				var signedInRecord = new SignedInRecord()
				{
					DisplayName = displayName,
					IsVisitor = isVisitor,
					SignInTime = signInTime,
					TokenId = tokenId
				};

				context.AuditRecords.Add(auditRecord);
				context.CurrentlySignedIn.Add(signedInRecord);

				context.SaveChanges();
			}

			return signInTime;
		}

		public DateTime SignOut(string displayName, DateTime signInTime, bool isVisitor, string tokenId = null)
		{
			if (String.IsNullOrWhiteSpace(displayName))
			{
				throw new ArgumentException(nameof(displayName));
			}

			var signOutTime = DateTime.Now;

			using (var context = this.GetDbContext())
			{
				var signedInRecord =
					context.CurrentlySignedIn.FirstOrDefault(item => item.DisplayName == displayName && item.SignInTime == signInTime);

				if (signedInRecord == null)
				{
					throw new InvalidOperationException("Can't find matching Signed-In record.  Are you sure this user is signed in?");
				}

				var auditRecord = new AuditRecord()
				{
					PersonDisplayName = displayName,
					PersonTokenId = tokenId,
					PersonIsVisitor = isVisitor,
					RecordType = AuditRecord.SignOutRecordType,
					Time = signOutTime
				};

				context.AuditRecords.Add(auditRecord);
				context.CurrentlySignedIn.Remove(signedInRecord);

				context.SaveChanges();
			}

			return signOutTime;
		}
	}
}
