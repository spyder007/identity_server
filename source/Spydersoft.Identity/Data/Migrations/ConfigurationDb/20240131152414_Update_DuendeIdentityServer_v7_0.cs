using Microsoft.EntityFrameworkCore.Migrations;

namespace Spydersoft.Identity.Data.Migrations.ConfigurationDb
{
    /// <inheritdoc />
    public partial class Update_DuendeIdentityServer_v7_0 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.AddColumn<int>(
                name: "PushedAuthorizationLifetime",
                table: "Clients",
                type: "int",
                nullable: true);

            _ = migrationBuilder.AddColumn<bool>(
                name: "RequirePushedAuthorization",
                table: "Clients",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.DropColumn(
                name: "PushedAuthorizationLifetime",
                table: "Clients");

            _ = migrationBuilder.DropColumn(
                name: "RequirePushedAuthorization",
                table: "Clients");
        }
    }
}