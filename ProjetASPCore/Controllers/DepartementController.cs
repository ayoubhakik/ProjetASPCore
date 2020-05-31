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
        public IActionResult ExportExcel()
        {
            return File(departementService.ExportExcel(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "testing.xlsx");

        }

        ///fonction pour les info
        [HttpGet]
        public IActionResult ExportExcelAttributed()
        {
            //Données à exporter
            return File(departementService.ExportExcelAttributed(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "listeAttribution.xlsx");
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

        public ActionResult PrintConsultation()
        {
            EtudiantContext db = new EtudiantContext();

            var q = new ViewAsPdf("ImprimerEtudiant", db.Etudiants.ToList());

            var h = HttpContext.Session.GetInt32("userId");
            var h1 = HttpContext.Session.GetString("role");

            if (h != null && h1.Equals("Departement"))
            {
                return q;
            }
            else
            {
                return RedirectToAction("Authentification", "User");
            }
        }





    }
}
