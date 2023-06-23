using Microsoft.EntityFrameworkCore.Migrations;

namespace Spydersoft.Identity.Data.Migrations.PersistedGrantDb
{
#pragma warning disable IDE1006 // Naming Styles
    public partial class aspnetcore_31_update : Migration
#pragma warning restore IDE1006 // Naming Styles
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.DropIndex(
                name: "IX_PersistedGrants_SubjectId_ClientId_Type_Expiration",
                table: "PersistedGrants");

            _ = migrationBuilder.CreateIndex(
                name: "IX_PersistedGrants_Expiration",
                table: "PersistedGrants",
                column: "Expiration");

            _ = migrationBuilder.CreateIndex(
                name: "IX_PersistedGrants_SubjectId_ClientId_Type",
                table: "PersistedGrants",
                columns: new[] { "SubjectId", "ClientId", "Type" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.DropIndex(
                name: "IX_PersistedGrants_Expiration",
                table: "PersistedGrants");

            _ = migrationBuilder.DropIndex(
                name: "IX_PersistedGrants_SubjectId_ClientId_Type",
                table: "PersistedGrants");

            _ = migrationBuilder.CreateIndex(
                name: "IX_PersistedGrants_SubjectId_ClientId_Type_Expiration",
                table: "PersistedGrants",
                columns: new[] { "SubjectId", "ClientId", "Type", "Expiration" });
        }
    }
}