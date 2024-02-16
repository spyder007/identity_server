using System;

using Microsoft.EntityFrameworkCore.Migrations;

namespace Spydersoft.Identity.Data.Migrations.ConfigurationDb
{
    /// <summary>
    /// Class IdentityServer3to4.
    /// Implements the <see cref="Migration" />
    /// </summary>
    /// <seealso cref="Migration" />
    public partial class IdentityServer3to4 : Migration
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
            _ = migrationBuilder.DropForeignKey(
                name: "FK_ApiClaims_ApiResources_ApiResourceId",
                table: "ApiClaims");

            _ = migrationBuilder.DropForeignKey(
                name: "FK_ApiProperties_ApiResources_ApiResourceId",
                table: "ApiProperties");

            _ = migrationBuilder.DropForeignKey(
                name: "FK_ApiScopeClaims_ApiScopes_ApiScopeId",
                table: "ApiScopeClaims");

            _ = migrationBuilder.DropForeignKey(
                name: "FK_ApiScopes_ApiResources_ApiResourceId",
                table: "ApiScopes");

            _ = migrationBuilder.DropForeignKey(
                name: "FK_IdentityProperties_IdentityResources_IdentityResourceId",
                table: "IdentityProperties");

            _ = migrationBuilder.DropTable(
                name: "ApiSecrets");

            _ = migrationBuilder.DropTable(
                name: "IdentityClaims");

            _ = migrationBuilder.DropIndex(
                name: "IX_ApiScopes_ApiResourceId",
                table: "ApiScopes");

            _ = migrationBuilder.DropIndex(
                name: "IX_ApiScopeClaims_ApiScopeId",
                table: "ApiScopeClaims");

            _ = migrationBuilder.DropPrimaryKey(
                name: "PK_IdentityProperties",
                table: "IdentityProperties");

            _ = migrationBuilder.DropPrimaryKey(
                name: "PK_ApiProperties",
                table: "ApiProperties");

            _ = migrationBuilder.DropPrimaryKey(
                name: "PK_ApiClaims",
                table: "ApiClaims");

            _ = migrationBuilder.DropColumn(
                name: "ApiResourceId",
                table: "ApiScopes");

            _ = migrationBuilder.DropColumn(
                name: "ApiScopeId",
                table: "ApiScopeClaims");

            _ = migrationBuilder.RenameTable(
                name: "IdentityProperties",
                newName: "IdentityResourceProperties");

            _ = migrationBuilder.RenameTable(
                name: "ApiProperties",
                newName: "ApiResourceProperties");

            _ = migrationBuilder.RenameTable(
                name: "ApiClaims",
                newName: "ApiResourceClaims");

            _ = migrationBuilder.RenameIndex(
                name: "IX_IdentityProperties_IdentityResourceId",
                table: "IdentityResourceProperties",
                newName: "IX_IdentityResourceProperties_IdentityResourceId");

            _ = migrationBuilder.RenameIndex(
                name: "IX_ApiProperties_ApiResourceId",
                table: "ApiResourceProperties",
                newName: "IX_ApiResourceProperties_ApiResourceId");

            _ = migrationBuilder.RenameIndex(
                name: "IX_ApiClaims_ApiResourceId",
                table: "ApiResourceClaims",
                newName: "IX_ApiResourceClaims_ApiResourceId");

            _ = migrationBuilder.AddColumn<string>(
                name: "AllowedIdentityTokenSigningAlgorithms",
                table: "Clients",
                maxLength: 100,
                nullable: true);

            _ = migrationBuilder.AddColumn<bool>(
                name: "RequireRequestObject",
                table: "Clients",
                nullable: false,
                defaultValue: false);

            _ = migrationBuilder.AddColumn<bool>(
                name: "Enabled",
                table: "ApiScopes",
                nullable: false,
                defaultValue: false);

            _ = migrationBuilder.AddColumn<int>(
                name: "ScopeId",
                table: "ApiScopeClaims",
                nullable: false,
                defaultValue: 0);

            _ = migrationBuilder.AddColumn<string>(
                name: "AllowedAccessTokenSigningAlgorithms",
                table: "ApiResources",
                maxLength: 100,
                nullable: true);

            _ = migrationBuilder.AddColumn<bool>(
                name: "ShowInDiscoveryDocument",
                table: "ApiResources",
                nullable: false,
                defaultValue: false);

            _ = migrationBuilder.AddPrimaryKey(
                name: "PK_IdentityResourceProperties",
                table: "IdentityResourceProperties",
                column: "Id");

            _ = migrationBuilder.AddPrimaryKey(
                name: "PK_ApiResourceProperties",
                table: "ApiResourceProperties",
                column: "Id");

            _ = migrationBuilder.AddPrimaryKey(
                name: "PK_ApiResourceClaims",
                table: "ApiResourceClaims",
                column: "Id");

            _ = migrationBuilder.CreateTable(
                name: "ApiResourceScopes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Scope = table.Column<string>(maxLength: 200, nullable: false),
                    ApiResourceId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_ApiResourceScopes", x => x.Id);
                    _ = table.ForeignKey(
                        name: "FK_ApiResourceScopes_ApiResources_ApiResourceId",
                        column: x => x.ApiResourceId,
                        principalTable: "ApiResources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            _ = migrationBuilder.CreateTable(
                name: "ApiResourceSecrets",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(maxLength: 1000, nullable: true),
                    Value = table.Column<string>(maxLength: 4000, nullable: false),
                    Expiration = table.Column<DateTime>(nullable: true),
                    Type = table.Column<string>(maxLength: 250, nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    ApiResourceId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_ApiResourceSecrets", x => x.Id);
                    _ = table.ForeignKey(
                        name: "FK_ApiResourceSecrets_ApiResources_ApiResourceId",
                        column: x => x.ApiResourceId,
                        principalTable: "ApiResources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            _ = migrationBuilder.CreateTable(
                name: "ApiScopeProperties",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Key = table.Column<string>(maxLength: 250, nullable: false),
                    Value = table.Column<string>(maxLength: 2000, nullable: false),
                    ScopeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_ApiScopeProperties", x => x.Id);
                    _ = table.ForeignKey(
                        name: "FK_ApiScopeProperties_ApiScopes_ScopeId",
                        column: x => x.ScopeId,
                        principalTable: "ApiScopes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            _ = migrationBuilder.CreateTable(
                name: "IdentityResourceClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(maxLength: 200, nullable: false),
                    IdentityResourceId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_IdentityResourceClaims", x => x.Id);
                    _ = table.ForeignKey(
                        name: "FK_IdentityResourceClaims_IdentityResources_IdentityResourceId",
                        column: x => x.IdentityResourceId,
                        principalTable: "IdentityResources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            _ = migrationBuilder.CreateIndex(
                name: "IX_ApiScopeClaims_ScopeId",
                table: "ApiScopeClaims",
                column: "ScopeId");

            _ = migrationBuilder.CreateIndex(
                name: "IX_ApiResourceScopes_ApiResourceId",
                table: "ApiResourceScopes",
                column: "ApiResourceId");

            _ = migrationBuilder.CreateIndex(
                name: "IX_ApiResourceSecrets_ApiResourceId",
                table: "ApiResourceSecrets",
                column: "ApiResourceId");

            _ = migrationBuilder.CreateIndex(
                name: "IX_ApiScopeProperties_ScopeId",
                table: "ApiScopeProperties",
                column: "ScopeId");

            _ = migrationBuilder.CreateIndex(
                name: "IX_IdentityResourceClaims_IdentityResourceId",
                table: "IdentityResourceClaims",
                column: "IdentityResourceId");

            _ = migrationBuilder.AddForeignKey(
                name: "FK_ApiResourceClaims_ApiResources_ApiResourceId",
                table: "ApiResourceClaims",
                column: "ApiResourceId",
                principalTable: "ApiResources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            _ = migrationBuilder.AddForeignKey(
                name: "FK_ApiResourceProperties_ApiResources_ApiResourceId",
                table: "ApiResourceProperties",
                column: "ApiResourceId",
                principalTable: "ApiResources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            _ = migrationBuilder.AddForeignKey(
                name: "FK_ApiScopeClaims_ApiScopes_ScopeId",
                table: "ApiScopeClaims",
                column: "ScopeId",
                principalTable: "ApiScopes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            _ = migrationBuilder.AddForeignKey(
                name: "FK_IdentityResourceProperties_IdentityResources_IdentityResourceId",
                table: "IdentityResourceProperties",
                column: "IdentityResourceId",
                principalTable: "IdentityResources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <summary>
        /// Downs the specified migration builder.
        /// </summary>
        /// <param name="migrationBuilder">The migration builder.</param>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.DropForeignKey(
                name: "FK_ApiResourceClaims_ApiResources_ApiResourceId",
                table: "ApiResourceClaims");

            _ = migrationBuilder.DropForeignKey(
                name: "FK_ApiResourceProperties_ApiResources_ApiResourceId",
                table: "ApiResourceProperties");

            _ = migrationBuilder.DropForeignKey(
                name: "FK_ApiScopeClaims_ApiScopes_ScopeId",
                table: "ApiScopeClaims");

            _ = migrationBuilder.DropForeignKey(
                name: "FK_IdentityResourceProperties_IdentityResources_IdentityResourceId",
                table: "IdentityResourceProperties");

            _ = migrationBuilder.DropTable(
                name: "ApiResourceScopes");

            _ = migrationBuilder.DropTable(
                name: "ApiResourceSecrets");

            _ = migrationBuilder.DropTable(
                name: "ApiScopeProperties");

            _ = migrationBuilder.DropTable(
                name: "IdentityResourceClaims");

            _ = migrationBuilder.DropIndex(
                name: "IX_ApiScopeClaims_ScopeId",
                table: "ApiScopeClaims");

            _ = migrationBuilder.DropPrimaryKey(
                name: "PK_IdentityResourceProperties",
                table: "IdentityResourceProperties");

            _ = migrationBuilder.DropPrimaryKey(
                name: "PK_ApiResourceProperties",
                table: "ApiResourceProperties");

            _ = migrationBuilder.DropPrimaryKey(
                name: "PK_ApiResourceClaims",
                table: "ApiResourceClaims");

            _ = migrationBuilder.DropColumn(
                name: "AllowedIdentityTokenSigningAlgorithms",
                table: "Clients");

            _ = migrationBuilder.DropColumn(
                name: "RequireRequestObject",
                table: "Clients");

            _ = migrationBuilder.DropColumn(
                name: "Enabled",
                table: "ApiScopes");

            _ = migrationBuilder.DropColumn(
                name: "ScopeId",
                table: "ApiScopeClaims");

            _ = migrationBuilder.DropColumn(
                name: "AllowedAccessTokenSigningAlgorithms",
                table: "ApiResources");

            _ = migrationBuilder.DropColumn(
                name: "ShowInDiscoveryDocument",
                table: "ApiResources");

            _ = migrationBuilder.RenameTable(
                name: "IdentityResourceProperties",
                newName: "IdentityProperties");

            _ = migrationBuilder.RenameTable(
                name: "ApiResourceProperties",
                newName: "ApiProperties");

            _ = migrationBuilder.RenameTable(
                name: "ApiResourceClaims",
                newName: "ApiClaims");

            _ = migrationBuilder.RenameIndex(
                name: "IX_IdentityResourceProperties_IdentityResourceId",
                table: "IdentityProperties",
                newName: "IX_IdentityProperties_IdentityResourceId");

            _ = migrationBuilder.RenameIndex(
                name: "IX_ApiResourceProperties_ApiResourceId",
                table: "ApiProperties",
                newName: "IX_ApiProperties_ApiResourceId");

            _ = migrationBuilder.RenameIndex(
                name: "IX_ApiResourceClaims_ApiResourceId",
                table: "ApiClaims",
                newName: "IX_ApiClaims_ApiResourceId");

            _ = migrationBuilder.AddColumn<int>(
                name: "ApiResourceId",
                table: "ApiScopes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            _ = migrationBuilder.AddColumn<int>(
                name: "ApiScopeId",
                table: "ApiScopeClaims",
                type: "int",
                nullable: false,
                defaultValue: 0);

            _ = migrationBuilder.AddPrimaryKey(
                name: "PK_IdentityProperties",
                table: "IdentityProperties",
                column: "Id");

            _ = migrationBuilder.AddPrimaryKey(
                name: "PK_ApiProperties",
                table: "ApiProperties",
                column: "Id");

            _ = migrationBuilder.AddPrimaryKey(
                name: "PK_ApiClaims",
                table: "ApiClaims",
                column: "Id");

            _ = migrationBuilder.CreateTable(
                name: "ApiSecrets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApiResourceId = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Expiration = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_ApiSecrets", x => x.Id);
                    _ = table.ForeignKey(
                        name: "FK_ApiSecrets_ApiResources_ApiResourceId",
                        column: x => x.ApiResourceId,
                        principalTable: "ApiResources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            _ = migrationBuilder.CreateTable(
                name: "IdentityClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdentityResourceId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_IdentityClaims", x => x.Id);
                    _ = table.ForeignKey(
                        name: "FK_IdentityClaims_IdentityResources_IdentityResourceId",
                        column: x => x.IdentityResourceId,
                        principalTable: "IdentityResources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            _ = migrationBuilder.CreateIndex(
                name: "IX_ApiScopes_ApiResourceId",
                table: "ApiScopes",
                column: "ApiResourceId");

            _ = migrationBuilder.CreateIndex(
                name: "IX_ApiScopeClaims_ApiScopeId",
                table: "ApiScopeClaims",
                column: "ApiScopeId");

            _ = migrationBuilder.CreateIndex(
                name: "IX_ApiSecrets_ApiResourceId",
                table: "ApiSecrets",
                column: "ApiResourceId");

            _ = migrationBuilder.CreateIndex(
                name: "IX_IdentityClaims_IdentityResourceId",
                table: "IdentityClaims",
                column: "IdentityResourceId");

            _ = migrationBuilder.AddForeignKey(
                name: "FK_ApiClaims_ApiResources_ApiResourceId",
                table: "ApiClaims",
                column: "ApiResourceId",
                principalTable: "ApiResources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            _ = migrationBuilder.AddForeignKey(
                name: "FK_ApiProperties_ApiResources_ApiResourceId",
                table: "ApiProperties",
                column: "ApiResourceId",
                principalTable: "ApiResources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            _ = migrationBuilder.AddForeignKey(
                name: "FK_ApiScopeClaims_ApiScopes_ApiScopeId",
                table: "ApiScopeClaims",
                column: "ApiScopeId",
                principalTable: "ApiScopes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            _ = migrationBuilder.AddForeignKey(
                name: "FK_ApiScopes_ApiResources_ApiResourceId",
                table: "ApiScopes",
                column: "ApiResourceId",
                principalTable: "ApiResources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            _ = migrationBuilder.AddForeignKey(
                name: "FK_IdentityProperties_IdentityResources_IdentityResourceId",
                table: "IdentityProperties",
                column: "IdentityResourceId",
                principalTable: "IdentityResources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}