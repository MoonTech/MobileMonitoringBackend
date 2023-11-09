﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WatchTowerAPI.DataAccess.DbContexts;

#nullable disable

namespace WatchTowerAPI.DataAccess.Migrations
{
    [DbContext(typeof(WatchTowerDbContext))]
    partial class WatchTowerDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.12")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("WatchTowerAPI.Domain.Models.CameraModel", b =>
                {
                    b.Property<string>("CameraToken")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool?>("AcceptationState")
                        .HasColumnType("bit");

                    b.Property<string>("RoomName")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("CameraToken");

                    b.HasIndex("RoomName");

                    b.ToTable("Cameras");
                });

            modelBuilder.Entity("WatchTowerAPI.Domain.Models.RoomModel", b =>
                {
                    b.Property<string>("RoomName")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("OwnerLogin")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Password")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("RoomName");

                    b.HasIndex("OwnerLogin");

                    b.ToTable("Rooms");
                });

            modelBuilder.Entity("WatchTowerAPI.Domain.Models.UserModel", b =>
                {
                    b.Property<string>("Login")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Login");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("WatchTowerAPI.Domain.Models.CameraModel", b =>
                {
                    b.HasOne("WatchTowerAPI.Domain.Models.RoomModel", "Room")
                        .WithMany("Cameras")
                        .HasForeignKey("RoomName")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Room");
                });

            modelBuilder.Entity("WatchTowerAPI.Domain.Models.RoomModel", b =>
                {
                    b.HasOne("WatchTowerAPI.Domain.Models.UserModel", "Owner")
                        .WithMany("Rooms")
                        .HasForeignKey("OwnerLogin")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("WatchTowerAPI.Domain.Models.RoomModel", b =>
                {
                    b.Navigation("Cameras");
                });

            modelBuilder.Entity("WatchTowerAPI.Domain.Models.UserModel", b =>
                {
                    b.Navigation("Rooms");
                });
#pragma warning restore 612, 618
        }
    }
}
