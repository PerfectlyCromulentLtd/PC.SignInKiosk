using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using OxHack.SignInKiosk.Database;

namespace OxHack.SignInKiosk.Database.Migrations
{
    [DbContext(typeof(SignInContext))]
    [Migration("20170227210313_InitialSchema")]
    partial class InitialSchema
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("OxHack.SignInKiosk.Database.Models.AuditRecord", b =>
                {
                    b.Property<int>("SequenceNumber")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("PersonDisplayName");

                    b.Property<bool>("PersonIsVisitor");

                    b.Property<string>("PersonTokenId");

                    b.Property<string>("RecordType");

                    b.Property<DateTime>("Time");

                    b.HasKey("SequenceNumber");

                    b.ToTable("AuditRecords");
                });

            modelBuilder.Entity("OxHack.SignInKiosk.Database.Models.SignedInRecord", b =>
                {
                    b.Property<string>("DisplayName");

                    b.Property<DateTime>("SignInTime");

                    b.Property<bool>("IsVisitor");

                    b.Property<string>("TokenId");

                    b.HasKey("DisplayName", "SignInTime");

                    b.ToTable("CurrentlySignedIn");
                });

            modelBuilder.Entity("OxHack.SignInKiosk.Database.Models.TokenHolder", b =>
                {
                    b.Property<string>("TokenId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("DisplayName");

                    b.Property<bool>("IsVisitor");

                    b.HasKey("TokenId");

                    b.ToTable("TokenHolders");
                });
        }
    }
}
