using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjetASPCore.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetASPCore.Services
{
    public interface IDepartementService
    {
        void EnvoyerLesFilieres();
        //Task<IActionResult> SendEmailAsync(string email, string subject, string message);
        void DeleteAllStudents();
        List<Etudiant> Search(string searchBy, string cne);
        void DeleteImportedStudents();
        void SupprimerEtudiant(string id);
        void ImporterEtudiantExcel(IFormFile excelFile);
        List<Etudiant> students();
        void ImporterNoteExcel(IFormFile excelFile);
        void AttributionFiliere(string infoMax, string indusMax, string gtrMax, string gpmcMax);
        byte[] ExtraireNonValide();
        byte[] ExportExcel();
        byte[] ExportExcelAttributed();
        void Setting(DateTime dateNotification, DateTime dateAttribution);
        Dictionary<string, long> statistiques();
       
        }
}
