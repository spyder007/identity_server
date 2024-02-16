using System;

using Microsoft.EntityFrameworkCore.Migrations;


namespace Spydersoft.Identity.Data.Migrations.ConfigurationDb
{
    /// <summary>
    /// Class Duende60.
    /// Implements the <see cref="Migration" />
    /// </summary>
    /// <seealso cref="Migration" />
    public partial class Duende60 : Migration
    {
        /// <summary>
        /// Ups the specified migration builder.
        /// </summary>
        /// <param name="migrationBuilder">The migration builder.</param>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.DropIndex(
                name: "IX_IdentityResourceProperties_IdentityResourceId",
                table: "IdentityResourceProperties");

            _ = migrationBuilder.DropIndex(
                name: "IX_IdentityResourceClaims_IdentityResourceId",
                table: "IdentityResourceClaims");

            _ = migrationBuilder.DropIndex(
                name: "IX_ClientScopes_ClientId",
                table: "ClientScopes");

            _ = migrationBuilder.DropIndex(
                name: "IX_ClientRedirectUris_ClientId",
                table: "ClientRedirectUris");

            _ = migrationBuilder.DropIndex(
                name: "IX_ClientProperties_ClientId",
                table: "ClientProperties");

            _ = migrationBuilder.DropIndex(
                name: "IX_ClientPostLogoutRedirectUris_ClientId",
                table: "ClientPostLogoutRedirectUris");

            _ = migrationBuilder.DropIndex(
                name: "IX_ClientIdPRestrictions_ClientId",
                table: "ClientIdPRestrictions");

            _ = migrationBuilder.DropIndex(
                name: "IX_ClientGrantTypes_ClientId",
                table: "ClientGrantTypes");

            _ = migrationBuilder.DropIndex(
                name: "IX_ClientCorsOrigins_ClientId",
                table: "ClientCorsOrigins");

            _ = migrationBuilder.DropIndex(
                name: "IX_ClientClaims_ClientId",
                table: "ClientClaims");

            _ = migrationBuilder.DropIndex(
                name: "IX_ApiScopeProperties_ScopeId",
                table: "ApiScopeProperties");

            _ = migrationBuilder.DropIndex(
                name: "IX_ApiScopeClaims_ScopeId",
                table: "ApiScopeClaims");

            _ = migrationBuilder.DropIndex(
                name: "IX_ApiResourceScopes_ApiResourceId",
                table: "ApiResourceScopes");

            _ = migrationBuilder.DropIndex(
                name: "IX_ApiResourceProperties_ApiResourceId",
                table: "ApiResourceProperties");

            _ = migrationBuilder.DropIndex(
                name: "IX_ApiResourceClaims_ApiResourceId",
                table: "ApiResourceClaims");

            _ = migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "IdentityProviders",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            _ = migrationBuilder.AddColumn<DateTime>(
                name: "LastAccessed",
                table: "IdentityProviders",
                type: "datetime2",
                nullable: true);

            _ = migrationBuilder.AddColumn<bool>(
                name: "NonEditable",
                table: "IdentityProviders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            _ = migrationBuilder.AddColumn<DateTime>(
                name: "Updated",
                table: "IdentityProviders",
                type: "datetime2",
                nullable: true);

            _ = migrationBuilder.AddColumn<int>(
                name: "CibaLifetime",
                table: "Clients",
                type: "int",
                nullable: true);

            _ = migrationBuilder.AddColumn<int>(
                name: "PollingInterval",
                table: "Clients",
                type: "int",
                nullable: true);

            _ = migrationBuilder.AlterColumn<string>(
                name: "RedirectUri",
                table: "ClientRedirectUris",
                type: "nvarchar(400)",
                maxLength: 400,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000);

            _ = migrationBuilder.AlterColumn<string>(
                name: "PostLogoutRedirectUri",
                table: "ClientPostLogoutRedirectUris",
                type: "nvarchar(400)",
                maxLength: 400,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000);

            _ = migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "ApiScopes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            _ = migrationBuilder.AddColumn<DateTime>(
                name: "LastAccessed",
                table: "ApiScopes",
                type: "datetime2",
                nullable: true);

            _ = migrationBuilder.AddColumn<bool>(
                name: "NonEditable",
                table: "ApiScopes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            _ = migrationBuilder.AddColumn<DateTime>(
                name: "Updated",
                table: "ApiScopes",
                type: "datetime2",
                nullable: true);

            _ = migrationBuilder.CreateIndex(
                name: "IX_IdentityResourceProperties_IdentityResourceId_Key",
                table: "IdentityResourceProperties",
                columns: ["IdentityResourceId", "Key"],
                unique: true);

            _ = migrationBuilder.CreateIndex(
                name: "IX_IdentityResourceClaims_IdentityResourceId_Type",
                table: "IdentityResourceClaims",
                columns: ["IdentityResourceId", "Type"],
                unique: true);

            _ = migrationBuilder.CreateIndex(
                name: "IX_IdentityProviders_Scheme",
                table: "IdentityProviders",
                column: "Scheme",
                unique: true);

            _ = migrationBuilder.CreateIndex(
                name: "IX_ClientScopes_ClientId_Scope",
                table: "ClientScopes",
                columns: ["ClientId", "Scope"],
                unique: true);

            _ = migrationBuilder.CreateIndex(
                name: "IX_ClientRedirectUris_ClientId_RedirectUri",
                table: "ClientRedirectUris",
                columns: ["ClientId", "RedirectUri"],
                unique: true);

            _ = migrationBuilder.CreateIndex(
                name: "IX_ClientProperties_ClientId_Key",
                table: "ClientProperties",
                columns: ["ClientId", "Key"],
                unique: true);

            _ = migrationBuilder.CreateIndex(
                name: "IX_ClientPostLogoutRedirectUris_ClientId_PostLogoutRedirectUri",
                table: "ClientPostLogoutRedirectUris",
                columns: ["ClientId", "PostLogoutRedirectUri"],
                unique: true);

            _ = migrationBuilder.CreateIndex(
                name: "IX_ClientIdPRestrictions_ClientId_Provider",
                table: "ClientIdPRestrictions",
                columns: ["ClientId", "Provider"],
                unique: true);

            _ = migrationBuilder.CreateIndex(
                name: "IX_ClientGrantTypes_ClientId_GrantType",
                table: "ClientGrantTypes",
                columns: ["ClientId", "GrantType"],
                unique: true);

            _ = migrationBuilder.CreateIndex(
                name: "IX_ClientCorsOrigins_ClientId_Origin",
                table: "ClientCorsOrigins",
                columns: ["ClientId", "Origin"],
                unique: true);

            _ = migrationBuilder.CreateIndex(
                name: "IX_ClientClaims_ClientId_Type_Value",
                table: "ClientClaims",
                columns: ["ClientId", "Type", "Value"],
                unique: true);

            _ = migrationBuilder.CreateIndex(
                name: "IX_ApiScopeProperties_ScopeId_Key",
                table: "ApiScopeProperties",
                columns: ["ScopeId", "Key"],
                unique: true);

            _ = migrationBuilder.CreateIndex(
                name: "IX_ApiScopeClaims_ScopeId_Type",
                table: "ApiScopeClaims",
                columns: ["ScopeId", "Type"],
                unique: true);

            _ = migrationBuilder.CreateIndex(
                name: "IX_ApiResourceScopes_ApiResourceId_Scope",
                table: "ApiResourceScopes",
                columns: ["ApiResourceId", "Scope"],
                unique: true);

            _ = migrationBuilder.CreateIndex(
                name: "IX_ApiResourceProperties_ApiResourceId_Key",
                table: "ApiResourceProperties",
                columns: ["ApiResourceId", "Key"],
                unique: true);

            _ = migrationBuilder.CreateIndex(
                name: "IX_ApiResourceClaims_ApiResourceId_Type",
                table: "ApiResourceClaims",
                columns: ["ApiResourceId", "Type"],
                unique: true);
        }

        /// <summary>
        /// Downs the specified migration builder.
        /// </summary>
        /// <param name="migrationBuilder">The migration builder.</param>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.DropIndex(
                name: "IX_IdentityResourceProperties_IdentityResourceId_Key",
                table: "IdentityResourceProperties");

            _ = migrationBuilder.DropIndex(
                name: "IX_IdentityResourceClaims_IdentityResourceId_Type",
                table: "IdentityResourceClaims");

            _ = migrationBuilder.DropIndex(
                name: "IX_IdentityProviders_Scheme",
                table: "IdentityProviders");

            _ = migrationBuilder.DropIndex(
                name: "IX_ClientScopes_ClientId_Scope",
                table: "ClientScopes");

            _ = migrationBuilder.DropIndex(
                name: "IX_ClientRedirectUris_ClientId_RedirectUri",
                table: "ClientRedirectUris");

            _ = migrationBuilder.DropIndex(
                name: "IX_ClientProperties_ClientId_Key",
                table: "ClientProperties");

            _ = migrationBuilder.DropIndex(
                name: "IX_ClientPostLogoutRedirectUris_ClientId_PostLogoutRedirectUri",
                table: "ClientPostLogoutRedirectUris");

            _ = migrationBuilder.DropIndex(
                name: "IX_ClientIdPRestrictions_ClientId_Provider",
                table: "ClientIdPRestrictions");

            _ = migrationBuilder.DropIndex(
                name: "IX_ClientGrantTypes_ClientId_GrantType",
                table: "ClientGrantTypes");

            _ = migrationBuilder.DropIndex(
                name: "IX_ClientCorsOrigins_ClientId_Origin",
                table: "ClientCorsOrigins");

            _ = migrationBuilder.DropIndex(
                name: "IX_ClientClaims_ClientId_Type_Value",
                table: "ClientClaims");

            _ = migrationBuilder.DropIndex(
                name: "IX_ApiScopeProperties_ScopeId_Key",
                table: "ApiScopeProperties");

            _ = migrationBuilder.DropIndex(
                name: "IX_ApiScopeClaims_ScopeId_Type",
                table: "ApiScopeClaims");

            _ = migrationBuilder.DropIndex(
                name: "IX_ApiResourceScopes_ApiResourceId_Scope",
                table: "ApiResourceScopes");

            _ = migrationBuilder.DropIndex(
                name: "IX_ApiResourceProperties_ApiResourceId_Key",
                table: "ApiResourceProperties");

            _ = migrationBuilder.DropIndex(
                name: "IX_ApiResourceClaims_ApiResourceId_Type",
                table: "ApiResourceClaims");

            _ = migrationBuilder.DropColumn(
                name: "Created",
                table: "IdentityProviders");

            _ = migrationBuilder.DropColumn(
                name: "LastAccessed",
                table: "IdentityProviders");

            _ = migrationBuilder.DropColumn(
                name: "NonEditable",
                table: "IdentityProviders");

            _ = migrationBuilder.DropColumn(
                name: "Updated",
                table: "IdentityProviders");

            _ = migrationBuilder.DropColumn(
                name: "CibaLifetime",
                table: "Clients");

            _ = migrationBuilder.DropColumn(
                name: "PollingInterval",
                table: "Clients");

            _ = migrationBuilder.DropColumn(
                name: "Created",
                table: "ApiScopes");

            _ = migrationBuilder.DropColumn(
                name: "LastAccessed",
                table: "ApiScopes");

            _ = migrationBuilder.DropColumn(
                name: "NonEditable",
                table: "ApiScopes");

            _ = migrationBuilder.DropColumn(
                name: "Updated",
                table: "ApiScopes");

            _ = migrationBuilder.AlterColumn<string>(
                name: "RedirectUri",
                table: "ClientRedirectUris",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(400)",
                oldMaxLength: 400);

            _ = migrationBuilder.AlterColumn<string>(
                name: "PostLogoutRedirectUri",
                table: "ClientPostLogoutRedirectUris",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(400)",
                oldMaxLength: 400);

            _ = migrationBuilder.CreateIndex(
                name: "IX_IdentityResourceProperties_IdentityResourceId",
                table: "IdentityResourceProperties",
                column: "IdentityResourceId");

            _ = migrationBuilder.CreateIndex(
                name: "IX_IdentityResourceClaims_IdentityResourceId",
                table: "IdentityResourceClaims",
                column: "IdentityResourceId");

            _ = migrationBuilder.CreateIndex(
                name: "IX_ClientScopes_ClientId",
                table: "ClientScopes",
                column: "ClientId");

            _ = migrationBuilder.CreateIndex(
                name: "IX_ClientRedirectUris_ClientId",
                table: "ClientRedirectUris",
                column: "ClientId");

            _ = migrationBuilder.CreateIndex(
                name: "IX_ClientProperties_ClientId",
                table: "ClientProperties",
                column: "ClientId");

            _ = migrationBuilder.CreateIndex(
                name: "IX_ClientPostLogoutRedirectUris_ClientId",
                table: "ClientPostLogoutRedirectUris",
                column: "ClientId");

            _ = migrationBuilder.CreateIndex(
                name: "IX_ClientIdPRestrictions_ClientId",
                table: "ClientIdPRestrictions",
                column: "ClientId");

            _ = migrationBuilder.CreateIndex(
                name: "IX_ClientGrantTypes_ClientId",
                table: "ClientGrantTypes",
                column: "ClientId");

            _ = migrationBuilder.CreateIndex(
                name: "IX_ClientCorsOrigins_ClientId",
                table: "ClientCorsOrigins",
                column: "ClientId");

            _ = migrationBuilder.CreateIndex(
                name: "IX_ClientClaims_ClientId",
                table: "ClientClaims",
                column: "ClientId");

            _ = migrationBuilder.CreateIndex(
                name: "IX_ApiScopeProperties_ScopeId",
                table: "ApiScopeProperties",
                column: "ScopeId");

            _ = migrationBuilder.CreateIndex(
                name: "IX_ApiScopeClaims_ScopeId",
                table: "ApiScopeClaims",
                column: "ScopeId");

            _ = migrationBuilder.CreateIndex(
                name: "IX_ApiResourceScopes_ApiResourceId",
                table: "ApiResourceScopes",
                column: "ApiResourceId");

            _ = migrationBuilder.CreateIndex(
                name: "IX_ApiResourceProperties_ApiResourceId",
                table: "ApiResourceProperties",
                column: "ApiResourceId");

            _ = migrationBuilder.CreateIndex(
                name: "IX_ApiResourceClaims_ApiResourceId",
                table: "ApiResourceClaims",
                column: "ApiResourceId");
        }
    }
}