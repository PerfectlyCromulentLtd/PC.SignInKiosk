using Microsoft.EntityFrameworkCore;
using OxHack.SignInKiosk.Domanin.Models;

namespace OxHack.SignInKiosk.Database
{
	internal partial class SignInContext : DbContext
	{
		private readonly IDbConfig dbConfig;

		public SignInContext()
		{
			this.dbConfig = new DummyConfig();
		}

		public SignInContext(IDbConfig dbConfig)
		{
			this.dbConfig = dbConfig;
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			base.OnConfiguring(optionsBuilder);

			optionsBuilder.UseSqlServer(this.dbConfig.ConnectionString);
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<TokenHolder>()
				.HasKey(item => item.TokenId);

			modelBuilder.Entity<AuditRecord>()
				.HasKey(item => item.SequenceNumber);

			modelBuilder.Entity<AuditRecord>()
				.Property(item => item.SequenceNumber)
				.ValueGeneratedOnAdd();

			modelBuilder.Entity<SignedInRecord>()
				.HasKey(item => nameof(item.Id));

			modelBuilder.Entity<SignedInRecord>()
				.Property(item => nameof(item.Id))
				.ValueGeneratedOnAdd();
		}
	}
}
