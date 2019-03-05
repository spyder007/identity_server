using Microsoft.EntityFrameworkCore.Migrations;

namespace one.Identity.Data.Migrations.ConfigurationDb
{
    public partial class addClientColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AlwaysIncludeUserClaimsInIdToken",
                table: "Clients",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AlwaysIncludeUserClaimsInIdToken",
                table: "Clients");
        }
    }
}
