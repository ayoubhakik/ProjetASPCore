using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjetASPCore.Models
{
    public class Filiere
    {
        public Filiere()
        {
            this.Etudiants = new HashSet<Etudiant>();
        }
        [Key]
        public int idFil { get; set; }
        public string nomFil { get; set; }

        public ICollection<Etudiant> Etudiants { get; set; }
    }
}