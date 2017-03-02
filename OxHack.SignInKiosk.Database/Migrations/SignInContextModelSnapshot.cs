using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using OxHack.SignInKiosk.Database;

namespace OxHack.SignInKiosk.Database.Migrations
{
    [DbContext(typeof(SignInContext))]
    partial class SignInContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("OxHack.SignInKiosk.Domain.Models.AuditRecord", b =>
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

            modelBuilder.Entity("OxHack.SignInKiosk.Domain.Models.SignedInRecord", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("DisplayName");

                    b.Property<bool>("IsVisitor");

                    b.Property<DateTime>("SignInTime")
                        .HasAnnotation("SqlServer:ColumnType", "datetime2(7)");

                    b.Property<string>("TokenId");

                    b.HasKey("Id");

                    b.ToTable("CurrentlySignedIn");
                });

            modelBuilder.Entity("OxHack.SignInKiosk.Domain.Models.TokenHolder", b =>
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
