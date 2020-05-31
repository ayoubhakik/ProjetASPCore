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
    }
}
