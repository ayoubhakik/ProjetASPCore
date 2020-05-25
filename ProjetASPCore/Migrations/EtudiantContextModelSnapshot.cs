﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ProjetASPCore.Context;

namespace ProjetASPCore.Migrations
{
    [DbContext(typeof(EtudiantContext))]
    partial class EtudiantContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.14-servicing-32113")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("projetASP.Models.Departement", b =>
                {
                    b.Property<int>("id_departement")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("email");

                    b.Property<string>("nom_departement");

                    b.Property<string>("password");

                    b.Property<string>("phone");

                    b.Property<string>("username");

                    b.HasKey("id_departement");

                    b.ToTable("Departement");
                });

            modelBuilder.Entity("projetASP.Models.Etudiant", b =>
                {
                    b.Property<string>("cne")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(10);

                    b.Property<string>("Choix");

                    b.Property<bool>("Modified");

                    b.Property<bool>("Redoubler");

                    b.Property<bool>("Validated");

                    b.Property<string>("address");

                    b.Property<int>("anneeBac");

                    b.Property<string>("cin")
                        .IsRequired()
                        .HasMaxLength(8);

                    b.Property<DateTime?>("dateNaiss");

                    b.Property<string>("email");

                    b.Property<string>("gsm")
                        .HasMaxLength(10);

                    b.Property<int?>("idFil");

                    b.Property<string>("lieuNaiss");

                    b.Property<string>("mentionBac");

                    b.Property<string>("nationalite");

                    b.Property<string>("nom")
                        .IsRequired();

                    b.Property<double>("noteBac");

                    b.Property<double>("noteFstYear");

                    b.Property<double>("noteSndYear");

                    b.Property<string>("password");

                    b.Property<string>("phone")
                        .HasMaxLength(10);

                    b.Property<string>("photo_link");

                    b.Property<string>("prenom")
                        .IsRequired();

                    b.Property<string>("typeBac");

                    b.Property<string>("ville");

                    b.HasKey("cne");

                    b.HasIndex("idFil");

                    b.ToTable("Etudiants");
                });

            modelBuilder.Entity("projetASP.Models.Filiere", b =>
                {
                    b.Property<int>("idFil")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("nomFil");

                    b.HasKey("idFil");

                    b.ToTable("Filieres");
                });

            modelBuilder.Entity("projetASP.Models.Settings", b =>
                {
                    b.Property<int>("idSettings")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Attributted");

                    b.Property<DateTime>("DatedeRappel");

                    b.Property<DateTime>("Delai");

                    b.Property<bool>("importEtudiant");

                    b.Property<bool>("importNote");

                    b.HasKey("idSettings");

                    b.ToTable("Settings");
                });

            modelBuilder.Entity("projetASP.Models.Etudiant", b =>
                {
                    b.HasOne("projetASP.Models.Filiere", "Filiere")
                        .WithMany("Etudiants")
                        .HasForeignKey("idFil");
                });
#pragma warning restore 612, 618
        }
    }
}
