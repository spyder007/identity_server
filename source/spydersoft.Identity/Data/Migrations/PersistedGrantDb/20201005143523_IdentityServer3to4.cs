using System;

using Microsoft.EntityFrameworkCore.Migrations;
namespace Spydersoft.Identity.Data.Migrations.PersistedGrantDb
{
#pragma warning disable IDE1006 // Naming Styles
    public partial class IdentityServer3to4 : Migration
#pragma warning restore IDE1006 // Naming Styles
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.AddColumn<DateTime>(
                name: "ConsumedTime",
                table: "PersistedGrants",
                nullable: true);

            _ = migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "PersistedGrants",
                maxLength: 200,
                nullable: true);

            _ = migrationBuilder.AddColumn<string>(
                name: "SessionId",
                table: "PersistedGrants",
                maxLength: 100,
                nullable: true);

            _ = migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "DeviceCodes",
                maxLength: 200,
                nullable: true);

            _ = migrationBuilder.AddColumn<string>(
                name: "SessionId",
                table: "DeviceCodes",
                maxLength: 100,
                nullable: true);


            _ = migrationBuilder.CreateIndex(
                name: "IX_PersistedGrants_SubjectId_SessionId_Type",
                table: "PersistedGrants",
                columns: new[] { "SubjectId", "SessionId", "Type" });

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.DropIndex(
                name: "IX_PersistedGrants_SubjectId_SessionId_Type",
                table: "PersistedGrants");

            _ = migrationBuilder.DropColumn(
                name: "ConsumedTime",
                table: "PersistedGrants");

            _ = migrationBuilder.DropColumn(
                name: "Description",
                table: "PersistedGrants");

            _ = migrationBuilder.DropColumn(
                name: "SessionId",
                table: "PersistedGrants");

            _ = migrationBuilder.DropColumn(
                name: "Description",
                table: "DeviceCodes");

            _ = migrationBuilder.DropColumn(
                name: "SessionId",
                table: "DeviceCodes");
        }
    }
}