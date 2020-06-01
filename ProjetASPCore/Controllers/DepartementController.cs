using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using ProjetASPCore.Models;
using ProjetASPCore.Context;
using ProjetASPCore.Services;
using Rotativa.AspNetCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using System.Threading.Tasks;

using HttpPostAttribute = Microsoft.AspNetCore.Mvc.HttpPostAttribute;
using HttpGetAttribute = Microsoft.AspNetCore.Mvc.HttpGetAttribute;
using ClosedXML.Excel;

namespace ProjetASPCore.Controllers
{

    public class DepartementController : Controller
    {

        private readonly IEtudiantService etudiantService;
        private readonly IDepartementService departementService;




        public DepartementController(IEtudiantService e, IDepartementService f)
        {
            etudiantService = e;
            departementService = f;
        }

        public ActionResult Index()
        {
            var h = HttpContext.Session.GetInt32("userId");
            var h1 = HttpContext.Session.GetString("role");

            if (h != null && h1.Equals("Departement"))
            {
                return View(departementService.students());
            }
            else
            {
                return RedirectToAction("Authentification", "User");
            }

        }



        public void EnvoyerLesFilieres()
        {
            var h = HttpContext.Session.GetInt32("userId");
            var h1 = HttpContext.Session.GetString("role");

            if (h != null && h1.Equals("Departement"))
            {
                departementService.EnvoyerLesFilieres();
            }


        }


        public ActionResult DeleteAllStudents()
        {
            var h = HttpContext.Session.GetInt32("userId");
            var h1 = HttpContext.Session.GetString("role");

            if (h != null && h1.Equals("Departement"))
            {
                departementService.DeleteAllStudents();
                return RedirectToAction("Index");
            }
            else
                return RedirectToAction("Authentification", "User");
        }

        public ActionResult Search(string searchBy, string cne)
        {

            var h = HttpContext.Session.GetInt32("userId");
            var h1 = HttpContext.Session.GetString("role");

            if (h != null && h1.Equals("Departement"))
            {
                if (departementService.Search(searchBy, cne).Count == 0)
                {
                    ViewBag.error = true;
                    return View();

                }
                ViewBag.error = false;

                return View(departementService.Search(searchBy, cne));

            }
            else
                return RedirectToAction("Authentification", "User");
        }


        //suppression des etudiants importes mais pas les redoublants
        public ActionResult DeleteImportedStudents()
        {
            var h = HttpContext.Session.GetInt32("userId");
            var h1 = HttpContext.Session.GetString("role");

            if (h != null && h1.Equals("Departement"))
            {

                departementService.DeleteImportedStudents();

                return RedirectToAction("Index");
            }
            else
                return RedirectToAction("Authentification", "User");

        }

        //suppression des etudiants (placer les etudiants redoublants dans la corbeille)
        public ActionResult SupprimerEtudiant(string id)
        {
            var h = HttpContext.Session.GetInt32("userId");
            var h1 = HttpContext.Session.GetString("role");

            if (h != null && h1.Equals("Departement"))
            {
                if (id != null)
                {

                    ViewBag.Current = "Index";

                    return RedirectToAction("Index");
                }
                else return RedirectToAction("Authentification", "User");

            }
            else
                return RedirectToAction("Authentification", "User");
        }



        public ActionResult Corbeille()
        {
            var h = HttpContext.Session.GetInt32("userId");
            var h1 = HttpContext.Session.GetString("role");

            if (h != null && h1.Equals("Departement"))
            {
                EtudiantContext db = new EtudiantContext();


                ViewBag.Current = "Corbeille";

                return View(departementService.students());
            }
            else
                return RedirectToAction("Authentification", "User");

        }

        public ActionResult Setting()
        {

            var h = HttpContext.Session.GetInt32("userId");
            var h1 = HttpContext.Session.GetString("role");

            if (h != null && h1.Equals("Departement"))
            {
                ///must be treated after
                EtudiantContext db = new EtudiantContext();
                ViewBag.Delai = db.Settings.FirstOrDefault().Delai;
                ViewBag.DatedeRappel = db.Settings.FirstOrDefault().DatedeRappel;
                db.SaveChanges();
                ViewBag.Current = "Setting";

                return View("Setting");
            }
            else
                return RedirectToAction("Authentification", "User");

        }

        // GET: Departement

        [HttpPost]
        public ActionResult Setting(DateTime dateNotification, DateTime dateAttribution)
        {
            var h = HttpContext.Session.GetInt32("userId");
            var h1 = HttpContext.Session.GetString("role");

            if (h != null && h1.Equals("Departement"))
            {
                departementService.Setting(dateNotification, dateAttribution);

                ////must be treated after
                EtudiantContext db = new EtudiantContext();
                ViewBag.Delai = db.Settings.FirstOrDefault().Delai;
                ViewBag.DatedeRappel = db.Settings.FirstOrDefault().DatedeRappel;
                ViewBag.Current = "Setting";
                return View("Setting");
            }
            else
                return RedirectToAction("Authentification", "User");



        }
        
        public ActionResult ImporterEtudiants()
        {
            ViewBag.Current = "importerEtudiants";
            var h = HttpContext.Session.GetInt32("userId");
            var h1 = HttpContext.Session.GetString("role");

            if (h != null && h1.Equals("Departement"))
            {
                //must be treated after
                EtudiantContext db = new EtudiantContext();
                if (!db.Settings.FirstOrDefault().importEtudiant)
                {
                    return View();
                }
                ViewBag.err = true;
                return View();
            }
            else
                return RedirectToAction("Authentification", "User");
        }

        //must be treated
        [HttpPost]
        public ActionResult ImporterEtudiantExcel(IFormFile excelFile)
        {


            try
            {
                if (Request != null)
                {

                    EtudiantContext db = new EtudiantContext();
                    IFormFile file = Request.Form.Files["excelfile"];
                    Byte[] contentLength = new UTF8Encoding(true).GetBytes(file.FileName);

                    if ((file != null) && (file.Length > 0) && !string.IsNullOrEmpty(file.FileName) && (file.FileName.EndsWith("xls") || file.FileName.EndsWith("xlsx")))
                    {
                        string fileName = file.FileName;
                        string fileContentType = file.ContentType;
                        byte[] fileBytes = new byte[file.Length];
                        var data = new StreamReader(file.OpenReadStream());
                        using (var package = new ExcelPackage(file.OpenReadStream()))
                        {
                            var currentSheet = package.Workbook.Worksheets;
                            var workSheet = currentSheet.First();
                            var noOfCol = workSheet.Dimension.End.Column;
                            var noOfRow = workSheet.Dimension.End.Row;

                            for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                            {
                                Etudiant e = new Etudiant();
                                e.nom = workSheet.Cells[rowIterator, 1].Value.ToString();
                                e.prenom = workSheet.Cells[rowIterator, 2].Value.ToString();
                                e.cin = workSheet.Cells[rowIterator, 3].Value.ToString();
                                e.cne = workSheet.Cells[rowIterator, 4].Value.ToString();
                                e.dateNaiss = Convert.ToDateTime(DateTime.Now);

                                db.Etudiants.Add(e);

                            }
                            db.Settings.First().importEtudiant = true;
                            db.SaveChanges();
                            for (int i = 0; i < db.Etudiants.ToList().Count; i++)
                            {
                                db.Etudiants.ToList()[i].Choix = "FDT";
                            }
                            db.SaveChanges();
                            return RedirectToAction("Index");

                        }
                    }
                    else
                    {
                        ViewBag.errI = true;
                        return View("ImporterEtudiants");
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["alertMessage"] = ex;
                ViewBag.errI = true;
                return View("ImporterEtudiants");
            }

            return View();
            //return View("Index");
        }


        public ActionResult ImporterNotes()
        {
            ViewBag.Current = "importerNotes";
            var h = HttpContext.Session.GetString("userId");
            var h1 = HttpContext.Session.GetString("role");

            if (h != null && h1.Equals("Departement"))
            {
                EtudiantContext db = new EtudiantContext();
                if (db.Settings.FirstOrDefault().importEtudiant)
                {
                    return View();
                }
                ViewBag.err = true;
                return View();
            }
            else
                return RedirectToAction("Authentification", "User");
        }
        //must be treated

        [HttpPost]
        public ActionResult ImporterNoteExcel(IFormFile excelFile)
        {
            try
            {
                if (Request != null)
                {
                    EtudiantContext db = new EtudiantContext();
                    IFormFile file = Request.Form.Files["excelfile"];
                    Byte[] contentLength = new UTF8Encoding(true).GetBytes(file.FileName);

                    if ((file != null) && (file.Length > 0) && !string.IsNullOrEmpty(file.FileName) && (file.FileName.EndsWith("xls") || file.FileName.EndsWith("xlsx")))
                    {
                        string fileName = file.FileName;
                        string fileContentType = file.ContentType;
                        byte[] fileBytes = new byte[file.Length];
                        var data = new StreamReader(file.OpenReadStream());
                        using (var package = new ExcelPackage(file.OpenReadStream()))
                        {
                            var currentSheet = package.Workbook.Worksheets;
                            var workSheet = currentSheet.First();
                            var noOfCol = workSheet.Dimension.End.Column;
                            var noOfRow = workSheet.Dimension.End.Row;
                            Console.WriteLine("before entering ......");
                            for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                            {
                                Console.WriteLine(" entering ......");
                                Etudiant e = db.Etudiants.Find(workSheet.Cells[rowIterator, 1].Value.ToString());
                                e.noteFstYear = Convert.ToDouble(workSheet.Cells[rowIterator, 2].Value);

                                e.noteSndYear = Convert.ToDouble(workSheet.Cells[rowIterator, 3].Value);



                                // db.Etudiants.Add(e);
                                Console.WriteLine(" out ......");

                            }
                            db.Settings.First().importNote = true;

                            db.SaveChanges();
                            return RedirectToAction("Index");
                        }
                    }
                    else
                    {
                        ViewBag.errI = true;
                        return View("ImporterNotes");
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["alertMessage1"] = ex;
                ViewBag.errI = true;
                return View("ImporterNotes");
            }
            return View();
        }

        public ActionResult AttributionFiliere()
        {
            ViewBag.Current = "attributionFiliere";
            var h = HttpContext.Session.GetInt32("userId");
            var h1 = HttpContext.Session.GetString("role");

            if (h != null && h1.Equals("Departement"))
            {
                EtudiantContext db = new EtudiantContext();
                List<Etudiant> list = db.Etudiants.ToList();

                if (db.Settings.FirstOrDefault().importNote)
                {


                    int total = 0;
                    for (int i = 0; i < db.Etudiants.ToList().Count; i++)
                    {
                        if (!db.Etudiants.ToList()[i].Redoubler)
                        {
                            total++;
                        }
                    }
                    ViewBag.total = total;
                    int info = 0, indus = 0, gpmc = 0, gtr = 0;
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
                    //initialisation des Maxs
                    int maxInfo = total / 4;
                    int maxGtr = total / 4;
                    int maxIndus = total / 4;
                    int maxGpmc = total / 4;

                    int diff = total % 4;
                    //diviser le diff partout
                    Dictionary<string, int> dr = new Dictionary<string, int>();

                    dr.Add("indus", indus);
                    dr.Add("gpmc", gpmc);
                    dr.Add("gtr", gtr);
                    dr.Add("info", info);

                    //creer une copie de dictionnaire
                    Dictionary<string, int> copyDr = new Dictionary<string, int>();
                    for (int i = 0; i < dr.Count; i++)
                    {
                        copyDr.Add(dr.ElementAt(i).Key, dr.ElementAt(i).Value);
                    }
                    int index1 = 0;
                    //ViewBag.test = "indus "+indus+" gtr "+gtr+" info "+info+" gpmc "+gpmc;
                    while (index1 < diff && diff != 0)
                    {
                        switch (dr.FirstOrDefault(x => x.Value == dr.Values.Max()).Key)
                        {
                            case "info":
                                maxInfo++;
                                index1++;
                                dr.Remove("info"); break;
                            case "gpmc":
                                maxGpmc++;
                                index1++;
                                dr.Remove("gpmc"); break;
                            case "gtr":
                                maxGtr++;
                                index1++;
                                dr.Remove("gtr"); break;
                            case "indus":
                                maxIndus++;
                                index1++;
                                dr.Remove("indus"); break;

                        }

                    }
                    if (db.Settings.FirstOrDefault().Attributted)
                    {
                        int i1 = 0, i2 = 0, i3 = 0, i4 = 0;
                        for (int i = 0; i < db.Etudiants.ToList().Count; i++)
                        {
                            if (!db.Etudiants.ToList()[i].Redoubler)
                            {
                                if (db.Etudiants.ToList()[i].idFil == 1) i1++;
                                if (db.Etudiants.ToList()[i].idFil == 2) i2++;
                                if (db.Etudiants.ToList()[i].idFil == 3) i3++;
                                if (db.Etudiants.ToList()[i].idFil == 4) i4++;
                            }


                        }
                        ViewBag.info = i1;
                        ViewBag.gtr = i2;
                        ViewBag.indus = i3;
                        ViewBag.gpmc = i4;
                    }
                    else
                    {
                        ViewBag.info = maxInfo;
                        ViewBag.indus = maxIndus;
                        ViewBag.gtr = maxGtr;
                        ViewBag.gpmc = maxGpmc;
                    }

                    //list =list.OrderBy(e => (e.noteFstYear+e.noteSndYear)/2);

                    return View(list);
                }
                ViewBag.err = true;
                list = list.OrderByDescending(e => e.nom).ToList();
                return View(list);
            }
            else
                return RedirectToAction("Authentification", "User");
        }
        [HttpPost]
        public ActionResult AttributionFiliere(string infoMax, string indusMax, string gtrMax, string gpmcMax)
        {

            ViewBag.Current = "attributionFiliere";
            var h = HttpContext.Session.GetInt32("userId");
            var h1 = HttpContext.Session.GetString("role");

            if (h != null && h1.Equals("Departement"))
            {
                //EtudiantContext db = new EtudiantContext();
                //return a  list sorted in a desendent way
                ViewBag.error2 = departementService.AttributionFiliere(infoMax, indusMax, gtrMax, gpmcMax);
                EnvoyerLesFilieres();
                return RedirectToAction("AttributionFiliere");
            }
            else
                return RedirectToAction("Authentification", "User");
        }

        /* public ActionResult Statistiques()
         {
             ViewBag.Current = "statistiques";

             var h = HttpContext.Session.GetInt32("userId");
             var h1 = HttpContext.Session.GetString("role");

             if (h != null && h1.Equals("Departement"))
             {
                 //essayons de retourner tous les etudiants
                 Dictionary<string, long> table = departementService.statistiques();

                 ViewBag.nbrTotal = table["total"];

                 ViewBag.nbrReste = table["nbrRest"];
                 ViewBag.info = table["info"];
                 ViewBag.gtr = table["gtr"];
                 ViewBag.gpmc = table["gpmc"];
                 ViewBag.indus = table["indus"];
                 //les pourcentages

                 return View();
             }
             else
                 return RedirectToAction("Authentification", "User");
         }


         public IActionResult Statistiques()
         {
             return View();
         }
         [HttpGet]
         public JsonResult PopulationChart()
         {
             var populationList = ListStatic.GetCityPopulationList();
             return Json(populationList);
         }*/



        [HttpPost]
        public IActionResult ExtraireNonValide()
        {

            return File(departementService.ExtraireNonValide(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "EtudiantNonValide.xlsx");

        }

        //Exporter toutes les informations des étudiants
        [HttpGet]
        public void ExportExcel()
        {
            /* string[] choixTab = new string[3];
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
             foreach (var student in departementService.students())
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
                 worksheet.Cell(currentRow, 1).Value = student.prenom;
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
             /*
             //Envoi du fichier dans par http
             using (var memoryStream = new MemoryStream())
             {
                 Response.Clear();
                 Response.ClearContent();
                 Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                 Response.AddHeader("content-disposition", "attachment; filename=testing.xlsx");
                 excel.SaveAs(memoryStream);
                 memoryStream.WriteTo(Response.OutputStream);
                 Response.Flush();
                 Response.Clear();
                 Response.End();
             }
             */

          
        }


        ///fonction pour les info
        [HttpGet]
        public IActionResult ExportExcelAttributed()
        {
            string[] choixTab = new string[3];
            string choixAffecte;
            EtudiantContext students = new EtudiantContext();
            //Données à exporter
            // return File(departementService.ExportExcelAttributed(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "listeAttribution.xlsx");
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Users");
                
                worksheet.Columns().Width = 15;

                var currentRow = 1;
                worksheet.Cell(currentRow, 1).Value = "Nom";
                worksheet.Cell(currentRow, 2).Value = "Prenom";
                worksheet.Cell(currentRow, 3).Value = "CIN";
                worksheet.Cell(currentRow, 4).Value = "CNE";
                worksheet.Cell(currentRow, 5).Value = "Email";
                worksheet.Cell(currentRow, 6).Value = "Date de naissance";
                worksheet.Cell(currentRow, 7).Value = "Lieu de naissance";
                worksheet.Cell(currentRow, 8).Value = "Nationalite";
                worksheet.Cell(currentRow, 9).Value = "GSM";
                worksheet.Cell(currentRow, 10).Value = "Tel fixe";
                worksheet.Cell(currentRow, 11).Value = "Adresse";
                worksheet.Cell(currentRow, 12).Value = "Ville";
                worksheet.Cell(currentRow, 13).Value = "Type de bac";
                worksheet.Cell(currentRow, 14).Value = "Annee de bac";
                worksheet.Cell(currentRow, 15).Value = "Note de bac";
                worksheet.Cell(currentRow, 16).Value = "Note de premiere annee";
                worksheet.Cell(currentRow, 17).Value = "Note de deuxieme annee";
                worksheet.Cell(currentRow, 18).Value = "Premier Choix";
                worksheet.Cell(currentRow, 19).Value = "Deuxieme Choix";
                worksheet.Cell(currentRow, 20).Value = "Troisieme Choix";
                worksheet.Cell(currentRow, 21).Value = "Filiere affectee";
                worksheet.Cell(currentRow, 22).Value = "Redoublant";
                int rowIndex = 1;
                foreach (var student in departementService.students())
                {
                    rowIndex++;
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
                        worksheet.Cell(rowIndex , 21).Value = choixAffecte;
                    }
                    else
                        worksheet.Cell(rowIndex, 21).Value = null;
                    

                   worksheet.Cell(rowIndex, 1).Value = student.nom;
                    worksheet.Cell(rowIndex, 2).Value = student.prenom;
                    worksheet.Cell(rowIndex, 3).Value = student.cin;
                    worksheet.Cell(rowIndex, 4).Value = student.cne;
                    worksheet.Cell(rowIndex, 4).Style.NumberFormat.SetNumberFormatId((int)XLPredefinedFormat.Number.Integer);
                    worksheet.Cell(rowIndex, 5).Value = student.email;
                    worksheet.Cell(rowIndex, 6).Value = student.dateNaiss;
                    worksheet.Cell(rowIndex, 7).Value = student.lieuNaiss;
                    worksheet.Cell(rowIndex, 8).Value = student.nationalite;
                    worksheet.Cell(rowIndex, 9).Value = student.gsm;
                    worksheet.Cell(rowIndex, 9).Style.NumberFormat.SetNumberFormatId((int)XLPredefinedFormat.Number.Integer);
                    worksheet.Cell(rowIndex, 10).Value = student.phone;
                    worksheet.Cell(rowIndex, 10).Style.NumberFormat.SetNumberFormatId((int)XLPredefinedFormat.Number.Integer);
                    worksheet.Cell(rowIndex, 11).Value = student.address;
                    worksheet.Cell(rowIndex, 12).Value = student.ville;
                    worksheet.Cell(rowIndex, 13).Value = student.typeBac;
                    worksheet.Cell(rowIndex, 14).Value = student.anneeBac;
                    worksheet.Cell(rowIndex, 15).Value = student.noteBac;
                    worksheet.Cell(rowIndex, 16).Value = student.noteFstYear;
                    worksheet.Cell(rowIndex, 17).Value = student.noteSndYear;
                    worksheet.Cell(rowIndex, 18).Value = choixTab[0];
                    worksheet.Cell(rowIndex, 19).Value = choixTab[1];
                    worksheet.Cell(rowIndex, 20).Value = choixTab[2];
                    if (student.Redoubler)
                        worksheet.Cell(rowIndex, 22).Value = "Oui";
                        worksheet.Cell(rowIndex, 23).Value = "";

                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(
                        content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "users.xlsx");
                }
            }
        }


        public ActionResult Visualiser()
        {
            ViewBag.Current = "visualiser";
            var h = HttpContext.Session.GetInt32("userId");
            var h1 = HttpContext.Session.GetString("role");

            if (h != null && h1.Equals("Departement"))
            {

                return View(departementService.students());
            }
            else
                return RedirectToAction("Authentification", "User");
        }
        //pour Imprimer le pdf

        public IActionResult PrintConsultation()
        {

            var report = new ViewAsPdf("ImprimerEtudiant", departementService.students())
            {
                PageMargins = { Left = 20, Bottom = 20, Right = 20, Top = 20 },
            };
            

            var h = HttpContext.Session.GetInt32("userId");
            var h1 = HttpContext.Session.GetString("role");

            if (h != null && h1.Equals("Departement"))
            {
                return report;
            }
            else
            {
                return RedirectToAction("Authentification", "User");
            }
        }





    }
}
