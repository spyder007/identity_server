using System;

using Microsoft.EntityFrameworkCore.Migrations;


namespace Spydersoft.Identity.Data.Migrations.ConfigurationDb
{
    public partial class Duende_63_AppDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.AddColumn<TimeSpan>(
                name: "DPoPClockSkew",
                table: "Clients",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            _ = migrationBuilder.AddColumn<int>(
                name: "DPoPValidationMode",
                table: "Clients",
                type: "int",
                nullable: false,
                defaultValue: 0);

            _ = migrationBuilder.AddColumn<string>(
                name: "InitiateLoginUri",
                table: "Clients",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            _ = migrationBuilder.AddColumn<bool>(
                name: "RequireDPoP",
                table: "Clients",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.DropColumn(
                name: "DPoPClockSkew",
                table: "Clients");

            _ = migrationBuilder.DropColumn(
                name: "DPoPValidationMode",
                table: "Clients");

            _ = migrationBuilder.DropColumn(
                name: "InitiateLoginUri",
                table: "Clients");

            _ = migrationBuilder.DropColumn(
                name: "RequireDPoP",
                table: "Clients");
        }
    }
}