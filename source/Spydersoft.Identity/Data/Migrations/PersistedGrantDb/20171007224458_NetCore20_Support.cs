using System;

using Microsoft.EntityFrameworkCore.Migrations;

namespace Spydersoft.Identity.Data.Migrations.PersistedGrantDb
{
#pragma warning disable IDE1006 // Naming Styles
    /// <summary>
    /// Class NetCore20_Support.
    /// Implements the <see cref="Migration" />
    /// </summary>
    /// <seealso cref="Migration" />
    public partial class NetCore20_Support : Migration
#pragma warning restore IDE1006 // Naming Styles
    {
        /// <summary>
        /// Ups the specified migration builder.
        /// </summary>
        /// <param name="migrationBuilder">The migration builder.</param>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.DropPrimaryKey(
                name: "PK_PersistedGrants",
                table: "PersistedGrants");

            _ = migrationBuilder.DropIndex(
                name: "IX_PersistedGrants_SubjectId",
                table: "PersistedGrants");

            _ = migrationBuilder.DropIndex(
                name: "IX_PersistedGrants_SubjectId_ClientId",
                table: "PersistedGrants");

            _ = migrationBuilder.AlterColumn<DateTime>(
                name: "Expiration",
                table: "PersistedGrants",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime));

            _ = migrationBuilder.AddPrimaryKey(
                name: "PK_PersistedGrants",
                table: "PersistedGrants",
                column: "Key");
        }

        /// <summary>
        /// Downs the specified migration builder.
        /// </summary>
        /// <param name="migrationBuilder">The migration builder.</param>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.DropPrimaryKey(
                name: "PK_PersistedGrants",
                table: "PersistedGrants");

            _ = migrationBuilder.AlterColumn<DateTime>(
                name: "Expiration",
                table: "PersistedGrants",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            _ = migrationBuilder.AddPrimaryKey(
                name: "PK_PersistedGrants",
                table: "PersistedGrants",
                columns: ["Key", "Type"]);

            _ = migrationBuilder.CreateIndex(
                name: "IX_PersistedGrants_SubjectId",
                table: "PersistedGrants",
                column: "SubjectId");

            _ = migrationBuilder.CreateIndex(
                name: "IX_PersistedGrants_SubjectId_ClientId",
                table: "PersistedGrants",
                columns: ["SubjectId", "ClientId"]);
        }
    }
}