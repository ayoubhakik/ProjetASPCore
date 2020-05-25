﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace projetASP.Models
{
    public class Settings
    {
        [Key]
        public int idSettings { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime Delai { get; set; }

        public Boolean importEtudiant { get; set; }

        public Boolean importNote { get; set; }

        public Boolean Attributted { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DatedeRappel { get; set; }
    }
}