using System;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Spydersoft.Identity.Data.Migrations.PersistedGrantDb
{
    /// <summary>
    /// Class InitialIdentityServerPersistedGrantDbMigration.
    /// Implements the <see cref="Migration" />
    /// </summary>
    /// <seealso cref="Migration" />
    [DbContext(typeof(PersistedGrantDbContext))]
    [Migration("20161208145658_InitialIdentityServerPersistedGrantDbMigration")]
    partial class InitialIdentityServerPersistedGrantDbMigration
    {
        /// <summary>
        /// Builds the target model.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Duende.IdentityServer.EntityFramework.Entities.PersistedGrant", b =>
                {
                    b.Property<string>("Key")
                        .HasAnnotation("MaxLength", 200);

                    b.Property<string>("Type")
                        .HasAnnotation("MaxLength", 50);

                    b.Property<string>("ClientId")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 200);

                    b.Property<DateTime>("CreationTime");

                    b.Property<string>("Data")
                        .IsRequired();

                    b.Property<DateTime>("Expiration");

                    b.Property<string>("SubjectId")
                        .HasAnnotation("MaxLength", 200);

                    b.HasKey("Key", "Type");

                    b.HasIndex("SubjectId");

                    b.HasIndex("SubjectId", "ClientId");

                    b.HasIndex("SubjectId", "ClientId", "Type");

                    b.ToTable("PersistedGrants");
                });
        }
    }
}
