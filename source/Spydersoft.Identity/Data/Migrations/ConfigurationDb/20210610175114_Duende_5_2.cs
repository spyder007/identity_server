﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace Spydersoft.Identity.Data.Migrations.ConfigurationDb
{
    public partial class Duende_5_2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.AddColumn<bool>(
                name: "RequireResourceIndicator",
                table: "ApiResources",
                type: "bit",
                nullable: false,
                defaultValue: false);

            _ = migrationBuilder.CreateTable(
                name: "IdentityProviders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Scheme = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Enabled = table.Column<bool>(type: "bit", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Properties = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table => _ = table.PrimaryKey("PK_IdentityProviders", x => x.Id));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.DropTable(
                name: "IdentityProviders");

            _ = migrationBuilder.DropColumn(
                name: "RequireResourceIndicator",
                table: "ApiResources");
        }
    }
}