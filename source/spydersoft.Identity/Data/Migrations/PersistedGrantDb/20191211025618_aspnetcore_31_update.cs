using Microsoft.EntityFrameworkCore.Migrations;

namespace spydersoft.Identity.Data.Migrations.PersistedGrantDb
{
    public partial class aspnetcore_31_update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PersistedGrants_SubjectId_ClientId_Type_Expiration",
                table: "PersistedGrants");

            migrationBuilder.CreateIndex(
                name: "IX_PersistedGrants_Expiration",
                table: "PersistedGrants",
                column: "Expiration");

            migrationBuilder.CreateIndex(
                name: "IX_PersistedGrants_SubjectId_ClientId_Type",
                table: "PersistedGrants",
                columns: new[] { "SubjectId", "ClientId", "Type" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PersistedGrants_Expiration",
                table: "PersistedGrants");

            migrationBuilder.DropIndex(
                name: "IX_PersistedGrants_SubjectId_ClientId_Type",
                table: "PersistedGrants");

            migrationBuilder.CreateIndex(
                name: "IX_PersistedGrants_SubjectId_ClientId_Type_Expiration",
                table: "PersistedGrants",
                columns: new[] { "SubjectId", "ClientId", "Type", "Expiration" });
        }
    }
}
