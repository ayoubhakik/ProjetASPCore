using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using ProjetASPCore.Context;
using ProjetASPCore.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetASPCore.Services
{
    public class EtudiantService : IEtudiantService
    {
        private readonly string[] ImageEx = new string[] { ".png", ".jpg", ".jpeg", ".jfif", ".svg" };
        private EtudiantContext db = new EtudiantContext();
        IHostingEnvironment _env;
        public EtudiantService(IHostingEnvironment environment)
        {
            _env = environment;

        }

        public byte[] ExporterExcel()
        {

            string[] choixTab = new string[3];
            string choixAffecte;

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
            foreach (var student in db.Etudiants.ToList())
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
                    choixAffecte = db.Filieres.Find(student.idFil).nomFil;
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

        public bool Modification(Etudiant etudiant, string Update, string choix1, string choix2, string choix3,IFormFile file)
        {
            try
            {
                Etudiant etudiants = FindEtudiant("R132580560");
                
                if(file!=null && file.Length > 0 && Update== "Upload")
                {
                   
                    var imagePath = @"\Images\";
                    var uploadPath = _env.WebRootPath + imagePath;

                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }
                    //Create uniq file name 
                    var uniqFileName = Guid.NewGuid().ToString();
                    var filename = Path.GetFileName(uniqFileName + "." + file.FileName.Split(".")[1].ToLower());
                    var extension = Path.GetExtension(filename);
                    string fullPath = uploadPath + filename;
                    if (filename != "" && ImageEx.Contains(extension) == true)
                    {
                        imagePath = imagePath + @"\";
                        var filePath = Path.Combine(imagePath, filename);

                        using (var fileStream = new FileStream(fullPath, FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }
                        etudiants.photo_link = filePath;
                    }
                    else
                    {
                        return false;
                    }
                   
                }

                if (Update == "Save")
                {
                    etudiants.nom = etudiant.nom;
                    etudiants.Choix = choix1+choix2+choix3;
                    etudiants.nationalite = etudiant.nationalite;
                    etudiants.email = etudiant.email;
                    etudiants.phone = etudiant.phone;
                    etudiants.address = etudiant.address;
                    etudiants.gsm = etudiant.gsm;
                    etudiants.address = etudiant.address;
                    etudiants.ville = etudiant.ville;
                    etudiants.dateNaiss = etudiant.dateNaiss;
                    etudiants.lieuNaiss = etudiant.lieuNaiss;
                  
                }
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (FindEtudiant(etudiant.cne)==null)
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }
            return true;
        }

        public Etudiant FindEtudiant(string id)
        {
            return db.Etudiants.Find(id);
        }
    }
}
