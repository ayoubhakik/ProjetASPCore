using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProjetASPCore.Models
{
    public class Etudiant
    {
        [Key]
        [Required(ErrorMessage = "Le CNE est obligatoire.")]
        [StringLength(10, ErrorMessage = "Le CNE doit contenir 10 caractères. ")]
        [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "Le CNE ne peut contenir que des lettres et des chiffres.")]
        public string cne { get; set; }

        [Required(ErrorMessage = "Le nom est obligatoire.")]
        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Le nom ne peut contenir que des lettres.")]
        public string nom { get; set; }

        [Required(ErrorMessage = "Le prenom est obligatoire.")]
        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Le prenom ne peut contenir que des lettres.")]
        public string prenom { get; set; }

        [MinLength(6, ErrorMessage = "Le mot de pass doit dépasser six caractères.")]
        public string password { get; set; }

        [RegularExpression("^[a-zA-Z ]*$", ErrorMessage = "La nationalite ne peut contenir que des lettres.")]
        public string nationalite { get; set; }

        [Required(ErrorMessage = "Le CIN est obligatoire.")]
        [StringLength(8, ErrorMessage = "Le CIN doit contenir 8 caractères. ")]
        [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "Le CIN ne peut contenir que des lettres et des chiffres.")]
        public string cin { get; set; }


        [EmailAddress(ErrorMessage = "Adresse email invalide.")]
        public string email { get; set; }

        [StringLength(10, ErrorMessage = "Le numéro de tel fixe doit contenir 10 chiffres")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Le numéro de tel fixe ne peut contenir que des chiffres.")]
        public string phone { get; set; }

        [StringLength(10, ErrorMessage = "Le numéro de tel portable doit contenir 10 chiffres")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Le numéro tel portable ne peut contenir que des chiffres.")]
        public string gsm { get; set; }

        [RegularExpression("^[a-zA-Z0-9, ]*$", ErrorMessage = "L'adresse ne peut contenir que des lettres et des chiffres.")]
        public string address { get; set; }

        [RegularExpression("^[a-zA-Z ]*$", ErrorMessage = "La ville ne peut contenir que des lettres.")]
        public string ville { get; set; }

        public string typeBac { get; set; }

        public int anneeBac { get; set; }

        [Range(0, 20, ErrorMessage = "La note de bac doit se situer entre 0 et 20.")]
        public double noteBac { get; set; }

        public string mentionBac { get; set; }

        public double noteFstYear { get; set; }

        public double noteSndYear { get; set; }

        /* //[Required]
         [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
         public DateTime dateNaiss { get; set; }
         //[Required]
         public string lieuNaiss { get; set; }*/


        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<DateTime> dateNaiss { get; set; }

        [RegularExpression("^[a-zA-Z ]*$", ErrorMessage = "Le lieu de naissance ne peut contenir que des lettres.")]
        public string lieuNaiss { get; set; }

        public string photo_link { get; set; }

        public string choix = "FDT";
        public string Choix { get; set; }

        public bool validated = false;
        public bool Validated { get; set; }

        public bool modified = false;
        public bool Modified { get; set; }
        public bool redoubler = false;
        public bool Redoubler { get; set; }

        [ForeignKey("Filiere")]
        public Nullable<int> idFil { get; set; }


        public virtual Filiere Filiere { get; set; }
    }
}