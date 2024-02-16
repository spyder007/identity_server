using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Spydersoft.Identity.Data.Migrations.ConfigurationDb
{
    /// <summary>
    /// Class NetCore20_Support.
    /// Implements the <see cref="Migration" />
    /// </summary>
    /// <seealso cref="Migration" />
    public partial class NetCore20_Support : Migration
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
            _ = migrationBuilder.DropColumn(
                name: "LogoutSessionRequired",
                table: "Clients");

            _ = migrationBuilder.DropColumn(
                name: "LogoutUri",
                table: "Clients");

            _ = migrationBuilder.DropColumn(
                name: "PrefixClientClaims",
                table: "Clients");

            _ = migrationBuilder.AlterColumn<string>(
                name: "LogoUri",
                table: "Clients",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            _ = migrationBuilder.AddColumn<bool>(
                name: "BackChannelLogoutSessionRequired",
                table: "Clients",
                type: "bit",
                nullable: false,
                defaultValue: false);

            _ = migrationBuilder.AddColumn<string>(
                name: "BackChannelLogoutUri",
                table: "Clients",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            _ = migrationBuilder.AddColumn<string>(
                name: "ClientClaimsPrefix",
                table: "Clients",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            _ = migrationBuilder.AddColumn<int>(
                name: "ConsentLifetime",
                table: "Clients",
                type: "int",
                nullable: true);

            _ = migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Clients",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            _ = migrationBuilder.AddColumn<bool>(
                name: "FrontChannelLogoutSessionRequired",
                table: "Clients",
                type: "bit",
                nullable: false,
                defaultValue: false);

            _ = migrationBuilder.AddColumn<string>(
                name: "FrontChannelLogoutUri",
                table: "Clients",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            _ = migrationBuilder.AddColumn<string>(
                name: "PairWiseSubjectSalt",
                table: "Clients",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            _ = migrationBuilder.CreateTable(
                name: "ClientProperties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_ClientProperties", x => x.Id);
                    _ = table.ForeignKey(
                        name: "FK_ClientProperties_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            _ = migrationBuilder.CreateIndex(
                name: "IX_ClientProperties_ClientId",
                table: "ClientProperties",
                column: "ClientId");
        }

        /// <summary>
        /// Downs the specified migration builder.
        /// </summary>
        /// <param name="migrationBuilder">The migration builder.</param>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.DropTable(
                name: "ClientProperties");

            _ = migrationBuilder.DropColumn(
                name: "BackChannelLogoutSessionRequired",
                table: "Clients");

            _ = migrationBuilder.DropColumn(
                name: "BackChannelLogoutUri",
                table: "Clients");

            _ = migrationBuilder.DropColumn(
                name: "ClientClaimsPrefix",
                table: "Clients");

            _ = migrationBuilder.DropColumn(
                name: "ConsentLifetime",
                table: "Clients");

            _ = migrationBuilder.DropColumn(
                name: "Description",
                table: "Clients");

            _ = migrationBuilder.DropColumn(
                name: "FrontChannelLogoutSessionRequired",
                table: "Clients");

            _ = migrationBuilder.DropColumn(
                name: "FrontChannelLogoutUri",
                table: "Clients");

            _ = migrationBuilder.DropColumn(
                name: "PairWiseSubjectSalt",
                table: "Clients");

            _ = migrationBuilder.AlterColumn<string>(
                name: "LogoUri",
                table: "Clients",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000,
                oldNullable: true);

            _ = migrationBuilder.AddColumn<bool>(
                name: "LogoutSessionRequired",
                table: "Clients",
                nullable: false,
                defaultValue: false);

            _ = migrationBuilder.AddColumn<string>(
                name: "LogoutUri",
                table: "Clients",
                nullable: true);

            _ = migrationBuilder.AddColumn<bool>(
                name: "PrefixClientClaims",
                table: "Clients",
                nullable: false,
                defaultValue: false);
        }
    }
}