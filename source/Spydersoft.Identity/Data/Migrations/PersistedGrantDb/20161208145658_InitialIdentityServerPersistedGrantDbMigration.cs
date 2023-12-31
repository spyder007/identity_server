﻿using System;

using Microsoft.EntityFrameworkCore.Migrations;

namespace Spydersoft.Identity.Data.Migrations.PersistedGrantDb
{
    public partial class InitialIdentityServerPersistedGrantDbMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.CreateTable(
                name: "PersistedGrants",
                columns: table => new
                {
                    Key = table.Column<string>(maxLength: 200, nullable: false),
                    Type = table.Column<string>(maxLength: 50, nullable: false),
                    ClientId = table.Column<string>(maxLength: 200, nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    Data = table.Column<string>(nullable: false),
                    Expiration = table.Column<DateTime>(nullable: false),
                    SubjectId = table.Column<string>(maxLength: 200, nullable: true)
                },
                constraints: table => _ = table.PrimaryKey("PK_PersistedGrants", x => new { x.Key, x.Type }));

            _ = migrationBuilder.CreateIndex(
                name: "IX_PersistedGrants_SubjectId",
                table: "PersistedGrants",
                column: "SubjectId");

            _ = migrationBuilder.CreateIndex(
                name: "IX_PersistedGrants_SubjectId_ClientId",
                table: "PersistedGrants",
                columns: new[] { "SubjectId", "ClientId" });

            _ = migrationBuilder.CreateIndex(
                name: "IX_PersistedGrants_SubjectId_ClientId_Type",
                table: "PersistedGrants",
                columns: new[] { "SubjectId", "ClientId", "Type" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.DropTable(
                name: "PersistedGrants");
        }
    }
}