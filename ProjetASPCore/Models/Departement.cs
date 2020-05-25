using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace projetASP.Models
{
    public class Departement
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_departement { get; set; }
        public string nom_departement { get; set; }
        public string email { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string phone { get; set; }
    }
}