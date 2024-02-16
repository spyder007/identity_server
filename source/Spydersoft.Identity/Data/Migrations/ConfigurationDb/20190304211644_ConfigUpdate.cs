using System;

using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Spydersoft.Identity.Data.Migrations.ConfigurationDb
{
    /// <summary>
    /// Class ConfigUpdate.
    /// Implements the <see cref="Migration" />
    /// </summary>
    /// <seealso cref="Migration" />
    public partial class ConfigUpdate : Migration
    {
        /// <summary>
        /// Builds the operations that will migrate the database 'up'.
        /// </summary>
        /// <param name="migrationBuilder">The <see cref="T:Microsoft.EntityFrameworkCore.Migrations.MigrationBuilder" /> that will build the operations.</param>
        /// <remarks><para>
        /// That is, builds the operations that will take the database from the state left in by the
        /// previous migration so that it is up-to-date with regard to this migration.
        /// </para>
        /// <para>
        /// This method must be overridden in each class that inherits from <see cref="T:Microsoft.EntityFrameworkCore.Migrations.Migration" />.
        /// </para>
        /// <para>
        /// See <see href="https://aka.ms/efcore-docs-migrations">Database migrations</see> for more information and examples.
        /// </para></remarks>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "IdentityResources",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            _ = migrationBuilder.AddColumn<bool>(
                name: "NonEditable",
                table: "IdentityResources",
                nullable: false,
                defaultValue: false);

            _ = migrationBuilder.AddColumn<DateTime>(
                name: "Updated",
                table: "IdentityResources",
                nullable: true);

            _ = migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "ClientSecrets",
                maxLength: 4000,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 2000);

            _ = migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "ClientSecrets",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 250,
                oldNullable: true);

            _ = migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "ClientSecrets",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            _ = migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "Clients",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            _ = migrationBuilder.AddColumn<int>(
                name: "DeviceCodeLifetime",
                table: "Clients",
                nullable: false,
                defaultValue: 0);

            _ = migrationBuilder.AddColumn<DateTime>(
                name: "LastAccessed",
                table: "Clients",
                nullable: true);

            _ = migrationBuilder.AddColumn<bool>(
                name: "NonEditable",
                table: "Clients",
                nullable: false,
                defaultValue: false);

            _ = migrationBuilder.AddColumn<DateTime>(
                name: "Updated",
                table: "Clients",
                nullable: true);

            _ = migrationBuilder.AddColumn<string>(
                name: "UserCodeType",
                table: "Clients",
                maxLength: 100,
                nullable: true);

            _ = migrationBuilder.AddColumn<int>(
                name: "UserSsoLifetime",
                table: "Clients",
                nullable: true);

            _ = migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "ApiSecrets",
                maxLength: 4000,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 2000,
                oldNullable: true);

            _ = migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "ApiSecrets",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 250,
                oldNullable: true);

            _ = migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "ApiSecrets",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            _ = migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "ApiResources",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            _ = migrationBuilder.AddColumn<DateTime>(
                name: "LastAccessed",
                table: "ApiResources",
                nullable: true);

            _ = migrationBuilder.AddColumn<bool>(
                name: "NonEditable",
                table: "ApiResources",
                nullable: false,
                defaultValue: false);

            _ = migrationBuilder.AddColumn<DateTime>(
                name: "Updated",
                table: "ApiResources",
                nullable: true);

            _ = migrationBuilder.CreateTable(
                name: "ApiProperties",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Key = table.Column<string>(maxLength: 250, nullable: false),
                    Value = table.Column<string>(maxLength: 2000, nullable: false),
                    ApiResourceId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_ApiProperties", x => x.Id);
                    _ = table.ForeignKey(
                        name: "FK_ApiProperties_ApiResources_ApiResourceId",
                        column: x => x.ApiResourceId,
                        principalTable: "ApiResources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            _ = migrationBuilder.CreateTable(
                name: "IdentityProperties",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Key = table.Column<string>(maxLength: 250, nullable: false),
                    Value = table.Column<string>(maxLength: 2000, nullable: false),
                    IdentityResourceId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_IdentityProperties", x => x.Id);
                    _ = table.ForeignKey(
                        name: "FK_IdentityProperties_IdentityResources_IdentityResourceId",
                        column: x => x.IdentityResourceId,
                        principalTable: "IdentityResources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            _ = migrationBuilder.CreateIndex(
                name: "IX_ApiProperties_ApiResourceId",
                table: "ApiProperties",
                column: "ApiResourceId");

            _ = migrationBuilder.CreateIndex(
                name: "IX_IdentityProperties_IdentityResourceId",
                table: "IdentityProperties",
                column: "IdentityResourceId");
        }

        /// <summary>
        /// Downs the specified migration builder.
        /// </summary>
        /// <param name="migrationBuilder">The migration builder.</param>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.DropTable(
                name: "ApiProperties");

            _ = migrationBuilder.DropTable(
                name: "IdentityProperties");

            _ = migrationBuilder.DropColumn(
                name: "Created",
                table: "IdentityResources");

            _ = migrationBuilder.DropColumn(
                name: "NonEditable",
                table: "IdentityResources");

            _ = migrationBuilder.DropColumn(
                name: "Updated",
                table: "IdentityResources");

            _ = migrationBuilder.DropColumn(
                name: "Created",
                table: "ClientSecrets");

            _ = migrationBuilder.DropColumn(
                name: "Created",
                table: "Clients");

            _ = migrationBuilder.DropColumn(
                name: "DeviceCodeLifetime",
                table: "Clients");

            _ = migrationBuilder.DropColumn(
                name: "LastAccessed",
                table: "Clients");

            _ = migrationBuilder.DropColumn(
                name: "NonEditable",
                table: "Clients");

            _ = migrationBuilder.DropColumn(
                name: "Updated",
                table: "Clients");

            _ = migrationBuilder.DropColumn(
                name: "UserCodeType",
                table: "Clients");

            _ = migrationBuilder.DropColumn(
                name: "UserSsoLifetime",
                table: "Clients");

            _ = migrationBuilder.DropColumn(
                name: "Created",
                table: "ApiSecrets");

            _ = migrationBuilder.DropColumn(
                name: "Created",
                table: "ApiResources");

            _ = migrationBuilder.DropColumn(
                name: "LastAccessed",
                table: "ApiResources");

            _ = migrationBuilder.DropColumn(
                name: "NonEditable",
                table: "ApiResources");

            _ = migrationBuilder.DropColumn(
                name: "Updated",
                table: "ApiResources");

            _ = migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "ClientSecrets",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 4000);

            _ = migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "ClientSecrets",
                maxLength: 250,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 250);

            _ = migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "ApiSecrets",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 4000);

            _ = migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "ApiSecrets",
                maxLength: 250,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 250);
        }
    }
}