using Microsoft.EntityFrameworkCore;
using OxHack.SignInKiosk.Domanin.Models;

namespace OxHack.SignInKiosk.Database
{
	internal partial class SignInContext : DbContext
	{
		public DbSet<AuditRecord> AuditRecords
		{
			get;
			set;
		}

		public DbSet<TokenHolder> TokenHolders
		{
			get;
			set;
		}

		public DbSet<SignedInRecord> CurrentlySignedIn
		{
			get;
			set;
		}
	}
}
