using Microsoft.EntityFrameworkCore;
using ProjetASPCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetASPCore.Context
{
    public class EtudiantContext: DbContext
    {
        public DbSet<Etudiant> Etudiants { get; set; }
        public DbSet<Settings> Settings { get; set; }
        public DbSet<Departement> Departement { get; set; }
        public DbSet<Filiere> Filieres { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer
                ("Server=LAPTOP-PDHSS6RI\\SQLEXPRESS;Database=ProjetASPCore;Trusted_Connection=True;");
        }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Settings>().HasData(
            new Settings
            {
                idSettings = 1,
                importEtudiant = false,
                Delai = Convert.ToDateTime("2020-06-30"),
                importNote = false,
                Attributted = false,
                DatedeRappel = Convert.ToDateTime("2020-06-27")

            }
            );
        modelBuilder.Entity<Departement>().HasData(
            new Departement
            {
                id_departement = 1,
                nom_departement = "ENSA",
                email = "assmaesafae20@gmail.com",
                username = "ENSA",
                password = "12345",
                phone = "0600000000"

            }

            );
    }
}
}
