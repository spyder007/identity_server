﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Spydersoft.Identity.Data;

#nullable disable

namespace Spydersoft.Identity.Data.Migrations.DataProtectionDb
{
    /// <summary>
    /// Class DataProtectionDbContextModelSnapshot.
    /// Implements the <see cref="ModelSnapshot" />
    /// </summary>
    /// <seealso cref="ModelSnapshot" />
    [DbContext(typeof(DataProtectionDbContext))]
    partial class DataProtectionDbContextModelSnapshot : ModelSnapshot
    {
        /// <summary>
        /// Builds the model.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Microsoft.AspNetCore.DataProtection.EntityFrameworkCore.DataProtectionKey", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("FriendlyName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Xml")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("DataProtectionKeys");
                });
#pragma warning restore 612, 618
        }
    }
}
