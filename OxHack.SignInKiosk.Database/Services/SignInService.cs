using OxHack.SignInKiosk.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OxHack.SignInKiosk.Database.Services
{
	public class SignInService
	{
		private readonly IDbConfig dbConfig;

		public SignInService(IDbConfig dbConfig)
		{
			this.dbConfig = dbConfig;
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

		public DateTime SignOut(SignedInRecord signedInRecord)
		{
			if (signedInRecord == null)
			{
				throw new ArgumentNullException();
			}

			var signOutTime = DateTime.Now;

			using (var context = this.GetDbContext())
			{
				var originalSignedInRecord =
					context.CurrentlySignedIn.SingleOrDefault(item => item.Id == signedInRecord.Id);

				if (originalSignedInRecord == null)
				{
					throw new InvalidOperationException("Can't find matching Signed-In record.  Are you sure this user is signed in?");
				}

				var auditRecord = new AuditRecord()
				{
					PersonDisplayName = originalSignedInRecord.DisplayName,
					PersonTokenId = originalSignedInRecord.TokenId,
					PersonIsVisitor = originalSignedInRecord.IsVisitor,
					RecordType = AuditRecord.SignOutRecordType,
					Time = signOutTime
				};

				context.AuditRecords.Add(auditRecord);
				context.CurrentlySignedIn.Remove(originalSignedInRecord);

				context.SaveChanges();
			}

			return signOutTime;
		}

		public IReadOnlyList<SignedInRecord> GetCurrentlySignedIn()
		{
			using (var context = this.GetDbContext())
			{
				return context.CurrentlySignedIn.ToList();
			}
		}

		public IReadOnlyList<AuditRecord> GetPast24Hours()
		{
			using (var context = this.GetDbContext())
			{
				return context.AuditRecords.Where(item => item.Time >= DateTime.Now.Subtract(TimeSpan.FromDays(1))).ToList();
			}
		}

		private SignInContext GetDbContext()
		{
			return new SignInContext(this.dbConfig);
		}
	}
}
