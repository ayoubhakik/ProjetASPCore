using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using ProjetASPCore.Context;
using ProjetASPCore.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HttpPostAttribute = Microsoft.AspNetCore.Mvc.HttpPostAttribute;
using HttpGetAttribute = Microsoft.AspNetCore.Mvc.HttpGetAttribute;

namespace ProjetASPCore.Services
{
    public class DepartementService:IDepartementService
    {
        private EtudiantContext db = new EtudiantContext();
        private readonly IEmailService _emailService;

        public void AttributionFiliere(string infoMax, string indusMax, string gtrMax, string gpmcMax)
        {
           
        }

        public void DeleteAllStudents()
        {
            for (int i = 0; i < db.Etudiants.ToList().Count; i++)
            {
                db.Etudiants.Remove(db.Etudiants.ToList()[i]);
            }
            db.Settings.First().Attributted = false;
            db.Settings.First().importEtudiant = false;
            db.Settings.First().importNote = false;
            db.SaveChanges();
        }

        public void DeleteImportedStudents()
        {
            for (int i = 0; i < db.Etudiants.ToList().Count; i++)
            {
                if (db.Etudiants.ToList()[i].Redoubler == false)
                {
                    db.Etudiants.Remove(db.Etudiants.ToList()[i]);
                }
            }

            db.Settings.First().Attributted = false;
            db.Settings.First().importEtudiant = false;
            db.Settings.First().importNote = false;

            db.SaveChanges();
        }
        public async Task<IActionResult> SendEmailAsync(string email, string subject, string message)
        {
            await _emailService.SendEmailAsync(email, subject, message);
            return null;
        }
        public void EnvoyerLesFilieres()
        {
            for (int i = 0; i < db.Etudiants.ToList().Count; i++)
            {
                if (db.Etudiants.ToList()[i].email != null)
                {
                    string body = "<div border='2px black solid'><h1 color='red'>Bonjour Mr/Mme " + db.Etudiants.ToList()[i].nom + " " + db.Etudiants.ToList()[i].prenom + "</h1>" +
                                                "<p>Apres avoir faire l'attribution des filieres, on vient de vous informer que votre filiere est : " + db.Filieres.Find(db.Etudiants.ToList()[i].idFil).nomFil + "</p><br/>" +
                                                "<button color='blue'><a href='localhost:localhost:52252/User/Authentification1'>Cliquer ici!</a></button>" +
                                                "</div>";
                    var Result = SendEmailAsync(db.Etudiants.ToList()[i].email, "Information a propos la filiere attribuer ", body);
                    if (Result.AsyncState != null)
                    {
                        //Json(Result, JsonRequestBehavior.AllowGet);
                    }
                }


            }
        }


        public byte[] ExportExcel()
        {
            string[] choixTab = new string[3];
            string choixAffecte;

            //Données à exporter
            EtudiantContext students = new EtudiantContext();

            //Création de la page excel
            ExcelPackage excel = new ExcelPackage();
            ExcelWorksheet worksheet = excel.Workbook.Worksheets.Add("Sheet1");

            //Style des noms de colonnes
            worksheet.Row(1).Style.Font.Bold = true;
            worksheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            //Noms des colonnes
            worksheet.Cells[1, 1].Value = "Nom";
            worksheet.Cells[1, 2].Value = "Prenom";
            worksheet.Cells[1, 3].Value = "CIN";
            worksheet.Cells[1, 4].Value = "CNE";
            worksheet.Cells[1, 5].Value = "Email";
            worksheet.Cells[1, 6].Value = "Date de naissance";
            worksheet.Cells[1, 7].Value = "Lieu de naissance";
            worksheet.Cells[1, 8].Value = "Nationalite";
            worksheet.Cells[1, 9].Value = "GSM";
            worksheet.Cells[1, 10].Value = "Tel fixe";
            worksheet.Cells[1, 11].Value = "Adresse";
            worksheet.Cells[1, 12].Value = "Ville";
            worksheet.Cells[1, 13].Value = "Type de bac";
            worksheet.Cells[1, 14].Value = "Annee de bac";
            worksheet.Cells[1, 15].Value = "Note de bac";
            worksheet.Cells[1, 16].Value = "Note de premiere annee";
            worksheet.Cells[1, 17].Value = "Note de deuxieme annee";
            worksheet.Cells[1, 18].Value = "Premier Choix";
            worksheet.Cells[1, 19].Value = "Deuxieme Choix";
            worksheet.Cells[1, 20].Value = "Troisieme Choix";
            worksheet.Cells[1, 21].Value = "Filiere affectee";
            worksheet.Cells[1, 22].Value = "Redoublant";

            //Remplissage des cellules
            int rowIndex = 2;
            foreach (var student in students.Etudiants.ToList())
            {

                //Separation des choix
                if (student.Validated)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        switch (student.Choix.ToCharArray()[i])
                        {
                            case 'F':
                                choixTab[i] = "Informatique";
                                break;
                            case 'D':
                                choixTab[i] = "Industriel";
                                break;
                            case 'T':
                                choixTab[i] = "Reseau et telecom";
                                break;
                            case 'P':
                                choixTab[i] = "Procedes";
                                break;
                        }
                    }
                }
                if (student.idFil != null)
                {
                    choixAffecte = students.Filieres.Find(student.idFil).nomFil;
                    worksheet.Cells[rowIndex, 21].Value = choixAffecte;
                }
                else
                    worksheet.Cells[rowIndex, 21].Value = null;

                worksheet.Cells[rowIndex, 1].Value = student.nom;
                worksheet.Cells[rowIndex, 2].Value = student.prenom;
                worksheet.Cells[rowIndex, 3].Value = student.cin;
                worksheet.Cells[rowIndex, 4].Value = student.cne;
                worksheet.Cells[rowIndex, 5].Value = student.email;
                worksheet.Cells[rowIndex, 6].Value = student.dateNaiss;
                worksheet.Cells[rowIndex, 7].Value = student.lieuNaiss;
                worksheet.Cells[rowIndex, 8].Value = student.nationalite;
                worksheet.Cells[rowIndex, 9].Value = student.gsm;
                worksheet.Cells[rowIndex, 10].Value = student.phone;
                worksheet.Cells[rowIndex, 11].Value = student.address;
                worksheet.Cells[rowIndex, 12].Value = student.ville;
                worksheet.Cells[rowIndex, 13].Value = student.typeBac;
                worksheet.Cells[rowIndex, 14].Value = student.anneeBac;
                worksheet.Cells[rowIndex, 15].Value = student.noteBac;
                worksheet.Cells[rowIndex, 16].Value = student.noteFstYear;
                worksheet.Cells[rowIndex, 17].Value = student.noteSndYear;
                worksheet.Cells[rowIndex, 18].Value = choixTab[0];
                worksheet.Cells[rowIndex, 19].Value = choixTab[1];
                worksheet.Cells[rowIndex, 20].Value = choixTab[2];
                if (student.Redoubler)
                    worksheet.Cells[rowIndex, 22].Value = "Oui";
                worksheet.Cells[rowIndex, 22].Value = "";


                rowIndex++;


            }

            //Envoi du fichier  par http
            using (var memoryStream = new MemoryStream())
            {
                //Response.Clear();
                //Response.ClearContent();
                //Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                //Response.AddHeader("content-disposition", "attachment; filename=testing.xlsx");
                //excel.SaveAs(memoryStream);
                //memoryStream.WriteTo(Response.OutputStream);
                //Response.Flush();
                //Response.Clear();
                //Response.End();
                excel.SaveAs(memoryStream);
                var content = memoryStream.ToArray();
                return content;
            }
            }

        public byte[] ExtraireNonValide()
        {
            EtudiantContext students = new EtudiantContext();

            //Création de la page excel
            ExcelPackage excel = new ExcelPackage();
            ExcelWorksheet worksheet = excel.Workbook.Worksheets.Add("Sheet1");

            //Style des noms de colonnes
            worksheet.Row(1).Style.Font.Bold = true;
            worksheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            //Noms des colonnes
            worksheet.Cells[1, 1].Value = "Nom";
            worksheet.Cells[1, 2].Value = "Prenom";
            worksheet.Cells[1, 3].Value = "CIN";
            worksheet.Cells[1, 4].Value = "CNE";


            //Remplissage des cellules
            int rowIndex = 2;
            foreach (var student in students.Etudiants.ToList())
            {
                if (!student.Validated)
                {
                    worksheet.Cells[rowIndex, 1].Value = student.nom;
                    worksheet.Cells[rowIndex, 2].Value = student.prenom;
                    worksheet.Cells[rowIndex, 3].Value = student.cin;
                    worksheet.Cells[rowIndex, 4].Value = student.cne;
                }


                rowIndex++;


            }

            //Envoi du fichier dans par http
            using (var memoryStream = new MemoryStream())
            {
                //Response.Clear();
                //Response.ClearContent();
                //Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                //Response.AddHeader("content-disposition", "attachment; filename=EtudiantNonValideCompte.xlsx");
                //excel.SaveAs(memoryStream);
                //memoryStream.WriteTo(Response.OutputStream);
                //Response.Flush();
                //Response.Clear();
                //Response.End();
                excel.SaveAs(memoryStream);
                var content = memoryStream.ToArray();
                return content;
            }
        }

        public byte[] ExportExcelAttributed()
        {

            //Création de la page excel
            ExcelPackage excel = new ExcelPackage();
            ExcelWorksheet worksheet = excel.Workbook.Worksheets.Add("Sheet1");

            //Style des noms de colonnes
            worksheet.Row(1).Style.Font.Bold = true;
            worksheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            //Noms des colonnes
            worksheet.Cells[1, 1].Value = "Nom";
            worksheet.Cells[1, 2].Value = "Prenom";
            worksheet.Cells[1, 3].Value = "CIN";
            worksheet.Cells[1, 4].Value = "CNE";
            worksheet.Cells[1, 5].Value = "Choix";
            worksheet.Cells[1, 6].Value = "Filiere affectee";

            //Remplissage des cellules
            int rowIndex = 2;
            foreach (var student in db.Etudiants.ToList())
            {
                worksheet.Cells[rowIndex, 1].Value = student.nom;
                worksheet.Cells[rowIndex, 2].Value = student.prenom;
                worksheet.Cells[rowIndex, 3].Value = student.cin;
                worksheet.Cells[rowIndex, 4].Value = student.cne;

                worksheet.Cells[rowIndex, 5].Value = student.choix;
                if (student.idFil == 1)
                {
                    worksheet.Cells[rowIndex, 6].Value = "Info";

                }

                if (student.idFil == 2)
                {
                    worksheet.Cells[rowIndex, 6].Value = "GTR";

                }
                if (student.idFil == 3)
                {
                    worksheet.Cells[rowIndex, 6].Value = "Indus";

                }
                if (student.idFil == 4)
                {
                    worksheet.Cells[rowIndex, 6].Value = "GPMC";

                }
                rowIndex++;


            }

            //Envoi du fichier  par http
            using (var memoryStream = new MemoryStream())
            {
                //Response.Clear();
                //Response.ClearContent();
                //Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                //Response.AddHeader("content-disposition", "attachment; filename=listeAttribution.xlsx");
                //excel.SaveAs(memoryStream);
                //memoryStream.WriteTo(Response.OutputStream);
                //Response.Flush();
                //Response.Clear();
                //Response.End();
                excel.SaveAs(memoryStream);
                var content = memoryStream.ToArray();
                return content;
            }
        }

        public void ImporterEtudiantExcel(IFormFile excelFile)
        {
        }

        public void ImporterNoteExcel(IFormFile excelFile)
        {
        }

        public List<Etudiant> Search(string searchBy, string cne)
        {
            EtudiantContext db = new EtudiantContext();
            List<Etudiant> etudiant = new List<Etudiant>();
            int count = 0;

            foreach (var item in db.Etudiants.Distinct().ToArray())
            {


                if (searchBy == "cne")
                {
                    var etudiants = (from s in db.Etudiants
                                     where s.cne == cne
                                     select s).ToList();
                    count++;
                    etudiant = etudiants;



                }
                if (searchBy == "Name")
                {
                    var etudiants = (from s in db.Etudiants
                                     where s.nom == cne
                                     select s).ToList();

                    etudiant = etudiants;
                    count++;
                }
                else if (searchBy == "cin")
                {

                    var etudiants = (from s in db.Etudiants
                                     where s.cin == cne
                                     select s).ToList();
                    count++;
                    etudiant = etudiants;


                }
            }


            return etudiant;
        }

        public void SupprimerEtudiant(string id)
        {
            db.Etudiants.Find(id).Redoubler = true;
            db.SaveChanges();
        }

        public List<Etudiant> students()
        {
            return db.Etudiants.ToList();

        }

        public void Setting(DateTime dateNotification, DateTime dateAttribution)
        {
            if (dateNotification != null)
            {
                db.Settings.FirstOrDefault().DatedeRappel = dateNotification;
            }
            if (dateAttribution != null)
            {
                db.Settings.FirstOrDefault().Delai = dateAttribution;
            }

            db.SaveChanges();
        }

        public Dictionary<string,long> statistiques()
        {
            Dictionary<string, long> table =new Dictionary<string, long>();
            List<Etudiant> list = db.Etudiants.ToList();
            int total = 0;
            int reste = 0;

            for (int i = 0; i < db.Etudiants.ToList().Count; i++)
            {
                if (!db.Etudiants.ToList()[i].Redoubler)
                {
                    total++;
                }
                if (!db.Etudiants.ToList()[i].Validated && !db.Etudiants.ToList()[i].Redoubler)
                {
                    reste++;
                }
            }
            //initialiser les max
            int maxInfo = total / 4;
            int maxGtr = total / 4;
            int maxIndus = total / 4;
            int maxGpmc = total / 4;
            int info = 0, indus = 0, gpmc = 0, gtr = 0;
            int diff = total % 4;

            for (int i = 0; i < db.Etudiants.ToList().Count; i++)
            {
                if (!db.Etudiants.ToList()[i].Redoubler && db.Etudiants.ToList()[i].Validated)
                {
                    char[] chiffr = (list[i].Choix).ToCharArray();

                    if (chiffr[0] == 'F')
                    {
                        info++;
                    }
                    if (chiffr[0] == 'P')
                    {
                        gpmc++;
                    }
                    if (chiffr[0] == 'T')
                    {
                        gtr++;
                    }
                    if (chiffr[0] == 'D')
                    {
                        indus++;
                    }
                }
            }
            table.Add("total",total);
            table.Add("nbrRest", reste);
            table.Add("info", info);
            table.Add("gtr", gtr);
            table.Add("gpmc", gpmc);
            table.Add("indus", indus);
            return table;


        }



    }
}
