﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Neru.Context;

#nullable disable

namespace Neru.Migrations
{
    [DbContext(typeof(SqliteContext))]
    [Migration("20240624052308_InitializeSqliteDB")]
    partial class InitializeSqliteDB
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.6");

            modelBuilder.Entity("Neru.Models.UserRemembrance", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateOnly>("UserBirthday")
                        .HasColumnType("TEXT");

                    b.Property<string>("UserFavouriteWord")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("UserHP")
                        .HasColumnType("INTEGER");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<float>("UserLove")
                        .HasColumnType("REAL");

                    b.Property<string>("UserNickname")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("UserRemembrances");
                });
#pragma warning restore 612, 618
        }
    }
}