using Microsoft.EntityFrameworkCore.Migrations;


namespace Spydersoft.Identity.Data.Migrations.DataProtectionDb
{
    public partial class DataProtect_Create : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.CreateTable(
                name: "DataProtectionKeys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FriendlyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Xml = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table => _ = table.PrimaryKey("PK_DataProtectionKeys", x => x.Id));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.DropTable(
                name: "DataProtectionKeys");
        }
    }
}