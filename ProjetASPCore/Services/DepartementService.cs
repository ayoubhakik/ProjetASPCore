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
using System.Net.Mail;
using System.Net;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace ProjetASPCore.Services
{
    public class DepartementService:IDepartementService
    {
        private EtudiantContext db = new EtudiantContext();
        private readonly IConfiguration _configuration;
        public DepartementService(IConfiguration configuration)
        {
            _configuration = configuration;

        }

        public Boolean AttributionFiliere(string infoMax, string indusMax, string gtrMax, string gpmcMax)
        {
            List<Etudiant> list = db.Etudiants.ToList();

            //calculer le nbr total sans les etudiants redoublants
            int total = 0;
            for (int i = 0; i < db.Etudiants.ToList().Count; i++)
            {
                if (!db.Etudiants.ToList()[i].Redoubler)
                {
                    total++;
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

            //diviser le diff partout
            Dictionary<string, int> dr = new Dictionary<string, int>();
            dr.Add("info", info);
            dr.Add("gpmc", gpmc);
            dr.Add("gtr", gtr);
            dr.Add("indus", indus);

            //creer une copie de dictionnaire
            Dictionary<string, int> copyDr = new Dictionary<string, int>();

            for (int i = 0; i < dr.Count; i++)
            {
                copyDr.Add(dr.ElementAt(i).Key, dr.ElementAt(i).Value);
            }
            int index1 = 0;
            while (index1 < diff && diff != 0)
            {
                switch (copyDr.FirstOrDefault(x => x.Value == copyDr.Values.Max()).Key)
                {
                    case "info":
                        maxInfo++;
                        index1++;
                        copyDr.Remove("info"); break;
                    case "indus":
                        maxIndus++;
                        index1++;
                        copyDr.Remove("indus"); break;
                    case "gpmc":
                        maxGtr++;
                        index1++;
                        copyDr.Remove("gpmc"); break;
                    case "gtr":
                        maxGpmc++;
                        index1++;
                        copyDr.Remove("gtr"); break;
                }

            }
            copyDr.Clear();
            for (int i = 0; i < dr.Count; i++)
            {
                copyDr.Add(dr.ElementAt(i).Key, dr.ElementAt(i).Value);
            }
            //changer les maxs si la departement a saisi des valeurs
            if (infoMax != null && indusMax != null && gpmcMax != null && gtrMax != null)
            {
                try
                {

                    maxInfo = Convert.ToInt32(infoMax);
                    maxIndus = Convert.ToInt32(indusMax);
                    maxGtr = Convert.ToInt32(gtrMax);
                    maxGpmc = Convert.ToInt32(gpmcMax);
                    if (maxInfo + maxIndus + maxGtr + maxGpmc != total)
                    {
                        return  true;
                    }

                }
                catch (Exception e)
                {

                }

            }

            int indexInfo = 0;
            int indexGtr = 0;
            int indexIndus = 0;
            int indexGpmc = 0;
            for (int i = 0; i < list.Count; i++)
            {

                //verification de l'etudiant si deja a choisi une filiere sinon on va lui attribuer la derniere filiere (gpmc->indus->gtr->info)
                if (!list[i].Redoubler)
                {
                    if (list[i].Validated)
                    {
                        //parse to a table of chars
                        char[] choice = list[i].Choix.ToCharArray();
                        //verify the frst case which is if we have F=info
                        Boolean choosen = false;

                        for (int j = 0; j < 3; j++)
                        {

                            if (choice[j] == 'F')
                            {
                                if (indexInfo < maxInfo)
                                {
                                    list[i].idFil = 1;
                                    choosen = true;
                                    indexInfo++; break;
                                }
                            }
                            if (choice[j] == 'T')
                            {
                                if (indexGtr < maxGtr)
                                {
                                    list[i].idFil = 2;
                                    choosen = true;

                                    indexGtr++; break;
                                }
                            }
                            if (choice[j] == 'D')
                            {
                                if (indexIndus < maxIndus)
                                {
                                    list[i].idFil = 3;
                                    choosen = true;

                                    indexIndus++; break;
                                }

                            }
                            if (choice[j] == 'P')
                            {
                                if (indexGpmc < maxGpmc)
                                {
                                    list[i].idFil = 4;
                                    choosen = true;
                                    indexGpmc++; break;
                                }
                            }
                            //si l'etudiant est plac'e dans une filiere en va sortir de la boucle
                            if (choosen)
                            {
                                j = 3;
                            }
                            //si l'etudiant n'est pas place dans une filiere alors (trois filiere qu il a choisi sont pleins), il va prendre une place dans la filiere disponible
                            if (!choosen && j == 2)
                            {
                                if (indexInfo < maxInfo)
                                {
                                    list[i].idFil = 1;
                                    choosen = true;
                                    indexInfo++; break;
                                }
                                if (indexGtr < maxGtr)
                                {
                                    list[i].idFil = 2;
                                    choosen = true;
                                    indexGtr++; break;
                                }
                                if (indexIndus < maxIndus)
                                {
                                    list[i].idFil = 3;
                                    choosen = true;
                                    indexIndus++; break;
                                }
                                if (indexGpmc < maxGpmc)
                                {
                                    list[i].idFil = 4;
                                    choosen = true;
                                    indexGpmc++; break;
                                }
                            }
                        }
                    }
                    else
                    {
                        copyDr.Clear();
                        for (int j = 0; j < dr.Count; j++)
                        {
                            copyDr.Add(dr.ElementAt(j).Key, dr.ElementAt(j).Value);
                        }
                        Boolean choosen = false;
                        while (!choosen)
                        {
                            switch (copyDr.FirstOrDefault(x => x.Value == copyDr.Values.Max()).Key)
                            {
                                case "info":
                                    if (indexInfo < maxInfo)
                                    {
                                        list[i].idFil = 1;
                                        choosen = true;
                                        indexInfo++;
                                    }
                                    copyDr.Remove("info");
                                    break;
                                case "gtr":
                                    if (indexGtr < maxGtr)
                                    {
                                        list[i].idFil = 2;
                                        choosen = true;
                                        indexGtr++;
                                    }
                                    copyDr.Remove("gtr");
                                    break;
                                case "indus":
                                    if (indexIndus < maxIndus)
                                    {
                                        list[i].idFil = 3;
                                        choosen = true;
                                        indexIndus++;
                                    }
                                    copyDr.Remove("indus");
                                    break;
                                case "gpmc":
                                    if (indexGpmc < maxGpmc)
                                    {
                                        list[i].idFil = 4;
                                        choosen = true;
                                        indexGpmc++;
                                    }
                                    copyDr.Remove("gpmc");
                                    break;
                            }
                        }

                    }
                }
            }

            db.Settings.First().Attributted = true;
        //envoi d'un msg qui contient la filiere attribuer pour  chaque etudiant
            db.SaveChanges();
            return false;
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
                    Boolean Result = SendEmail(db.Etudiants.ToList()[i].email, "Information a propos la filiere attribuer ", body);
                    if (Result == true)
                    {
                       // Json(Result, new Newtonsoft.Json.JsonSerializerSettings());
                    }
                }


            }
        }
        public bool SendEmail(String toEmail, string subject, string EmailBody)
        {
            try
            {
                String senderEmail = _configuration["Email:Email"];
                String senderPassword = _configuration["Email:Password"];
                /* WebMail.SmtpServer = "smtp.gmail.com";
                 WebMail.SmtpPort = 587;
                 WebMail.SmtpUseDefaultCredentials = true;
                 WebMail.UserName = sendereEmail;
                 WebMail.Password = senderPassword;
                 WebMail.Send(to: toEmail, subject: subject, body: EmailBody, isBodyHtml: true);*/
                System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient("smtp.gmail.com", 587);
                client.EnableSsl = true;
                client.Timeout = 100000;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(senderEmail, senderPassword);
                MailMessage Message = new MailMessage(senderEmail, toEmail, subject, EmailBody);
                Message.IsBodyHtml = true;
                Message.BodyEncoding = UTF8Encoding.UTF8;
                client.Send(Message);
                return true;
            }
            catch (Exception ex)
            {
                return false;
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

            return new EtudiantContext().Etudiants.ToList();

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
