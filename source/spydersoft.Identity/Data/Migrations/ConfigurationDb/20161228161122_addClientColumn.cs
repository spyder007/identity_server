using Microsoft.EntityFrameworkCore.Migrations;

namespace spydersoft.Identity.Data.Migrations.ConfigurationDb
{
#pragma warning disable IDE1006 // Naming Styles
    public partial class addClientColumn : Migration
#pragma warning restore IDE1006 // Naming Styles
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.AddColumn<bool>(
                name: "AlwaysIncludeUserClaimsInIdToken",
                table: "Clients",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.DropColumn(
                name: "AlwaysIncludeUserClaimsInIdToken",
                table: "Clients");
        }
    }
}