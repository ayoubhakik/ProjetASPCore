using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjetASPCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetASPCore.Services
{
    public interface IEtudiantService
    {
        byte[] ExporterExcel();

        bool Modification(Etudiant etudiant,string Update, String choix1, String choix2, String choix3,IFormFile file);

        Etudiant FindEtudiant(string id);



    }
}
