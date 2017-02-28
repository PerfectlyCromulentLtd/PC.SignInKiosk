using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace OxHack.SignInKiosk.Database.Migrations
{
    public partial class InitialSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditRecords",
                columns: table => new
                {
                    SequenceNumber = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PersonDisplayName = table.Column<string>(nullable: true),
                    PersonIsVisitor = table.Column<bool>(nullable: false),
                    PersonTokenId = table.Column<string>(nullable: true),
                    RecordType = table.Column<string>(nullable: true),
                    Time = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditRecords", x => x.SequenceNumber);
                });

            migrationBuilder.CreateTable(
                name: "CurrentlySignedIn",
                columns: table => new
                {
                    DisplayName = table.Column<string>(nullable: false),
                    SignInTime = table.Column<DateTime>(nullable: false),
                    IsVisitor = table.Column<bool>(nullable: false),
                    TokenId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrentlySignedIn", x => new { x.DisplayName, x.SignInTime });
                });

            migrationBuilder.CreateTable(
                name: "TokenHolders",
                columns: table => new
                {
                    TokenId = table.Column<string>(nullable: false),
                    DisplayName = table.Column<string>(nullable: true),
                    IsVisitor = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TokenHolders", x => x.TokenId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditRecords");

            migrationBuilder.DropTable(
                name: "CurrentlySignedIn");

            migrationBuilder.DropTable(
                name: "TokenHolders");
        }
    }
}
