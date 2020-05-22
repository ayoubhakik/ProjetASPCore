using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using projetASP.Models;
using ProjetASPCore.Context;
using ProjetASPCore.Models;
using ProjetASPCore.Services;
using Rotativa.AspNetCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web.helpers;

namespace projetASP.Controllers
{

    public class DepartementController : Controller
    {
        private readonly IEtudiantService etudiantService;
        private readonly IDepartementService departementService;

        DepartementController(IEtudiantService e, IDepartementService f)
        {
            this.departementService = f;
            this.etudiantService = e;
        }
        public void EnvoyerLesFilieres()
        {

            if (UserValide.IsValid() && UserValide.IsAdmin())
            {
                EtudiantContext db = new EtudiantContext();
                for (int i = 0; i < db.Etudiants.ToList().Count; i++)
                {
                    if (db.Etudiants.ToList()[i].email != null)
                    {
                        string body = "<div border='2px black solid'><h1 color='red'>Bonjour Mr/Mme " + db.Etudiants.ToList()[i].nom + " " + db.Etudiants.ToList()[i].prenom + "</h1>" +
                                                    "<p>Apres avoir faire l'attribution des filieres, on vient de vous informer que votre filiere est : " + db.Filieres.Find(db.etudiants.ToList()[i].idFil).nomFil + "</p><br/>" +
                                                    "<button color='blue'><a href='localhost:localhost:52252/User/Authentification1'>Cliquer ici!</a></button>" +
                                                    "</div>";
                        Boolean Result = SendEmail(db.Etudiants.ToList()[i].email, "Information a propos la filiere attribuer ", body);
                        if (Result == true)
                        {
                            //Json(Result, JsonRequestBehavior.AllowGet);
                        }
                    }



                }
            }


        }

        public bool SendEmail(String toEmail, string subject, string EmailBody)
        {
            try
            {
                String senderEmail = WebConfigurationManager.AppSettings["senderEmail"];
                String senderPassword = WebConfigurationManager.AppSettings["senderPassword"];
                /* WebMail.SmtpServer = "smtp.gmail.com";
                 WebMail.SmtpPort = 587;
                 WebMail.SmtpUseDefaultCredentials = true;
                 WebMail.UserName = sendereEmail;
                 WebMail.Password = senderPassword;
                 WebMail.Send(to: toEmail, subject: subject, body: EmailBody, isBodyHtml: true);*/
                SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
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

        public ActionResult DeleteAllStudents()
        {
            if (UserValide.IsValid() && UserValide.IsAdmin())
            {
                EtudiantContext db = new EtudiantContext();
                for (int i = 0; i < db.Etudiants.ToList().Count; i++)
                {
                    db.Etudiants.Remove(db.Etudiants.ToList()[i]);
                }
                db.Settings.First().Attributted = false;
                db.Settings.First().importEtudiant = false;
                db.Settings.First().importNote = false;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
                return RedirectToAction("Authentification", "User");
        }

        public ActionResult Search(string searchBy, string cne)
        {

            if (UserValide.IsValid() && UserValide.IsAdmin())
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


                if (count == 0)
                {
                    ViewBag.error = true;
                    return View();
                }
                ViewBag.error = false;
                return View(etudiant);

            }
            else
                return RedirectToAction("Authentification", "User");
        }
        //suppression des etudiants importes mais pas les redoublants
        public ActionResult DeleteImportedStudents()
        {
            if (UserValide.IsValid() && UserValide.IsAdmin())
            {
                EtudiantContext db = new EtudiantContext();
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

                return RedirectToAction("Index");
            }
            else
                return RedirectToAction("Authentification", "User");

        }

        //suppression des etudiants (placer les etudiants redoublants dans la corbeille)
        public ActionResult SupprimerEtudiant(string id)
        {
            if (UserValide.IsValid() && UserValide.IsAdmin())
            {
                if (id != null)
                {
                    EtudiantContext db = new EtudiantContext();
                    db.Etudiants.Find(id).Redoubler = true;
                    db.SaveChanges();
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
            if (UserValide.IsValid() && UserValide.IsAdmin())
            {
                EtudiantContext db = new EtudiantContext();


                ViewBag.Current = "Corbeille";

                return View(db.Etudiants.ToList());
            }
            else
                return RedirectToAction("Authentification", "User");

        }

        public ActionResult Setting()
        {

            if (UserValide.IsValid() && UserValide.IsAdmin())
            {
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
            if (UserValide.IsValid() && UserValide.IsAdmin())
            {
                EtudiantContext db = new EtudiantContext();


                if (dateNotification != null)
                {
                    db.Settings.FirstOrDefault().DatedeRappel = dateNotification;
                }
                if (dateAttribution != null)
                {
                    db.Settings.FirstOrDefault().Delai = dateAttribution;
                }

                db.SaveChanges();
                ViewBag.Delai = db.Settings.FirstOrDefault().Delai;
                ViewBag.DatedeRappel = db.Settings.FirstOrDefault().DatedeRappel;
                ViewBag.Current = "Setting";
                return View("Setting");
            }
            else
                return RedirectToAction("Authentification", "User");



        }
        public ActionResult Index()
        {

            if (UserValide.IsValid() && UserValide.IsAdmin())
            {
                EtudiantContext db = new EtudiantContext();


                ViewBag.Current = "index";
                List<Etudiant> list = db.Etudiants.ToList();

                return View(list);
            }
            else
                return RedirectToAction("Authentification", "User");



        }

        public ActionResult ImporterEtudiants()
        {
            ViewBag.Current = "importerEtudiants";
            if (UserValide.IsValid() && UserValide.IsAdmin())
            {

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


        [HttpPost]
        public ActionResult ImporterEtudiantExcel(HttpPostedFileBase excelFile)
        {
            try
            {
                if (Request != null)
                {

                    EtudiantContext db = new EtudiantContext();
                    HttpPostedFileBase file = Request.Files["excelfile"];
                    if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName) && (file.FileName.EndsWith("xls") || file.FileName.EndsWith("xlsx")))
                    {
                        string fileName = file.FileName;
                        string fileContentType = file.ContentType;
                        byte[] fileBytes = new byte[file.ContentLength];
                        var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
                        using (var package = new ExcelPackage(file.InputStream))
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
            if (UserValide.IsValid() && UserValide.IsAdmin())
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

        [HttpPost]
        public ActionResult ImporterNoteExcel(HttpPostedFileBase excelFile)
        {
            try
            {
                if (Request != null)
                {
                    EtudiantContext db = new EtudiantContext();
                    HttpPostedFileBase file = Request.Files["excelfile"];
                    if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName) && (file.FileName.EndsWith("xls") || file.FileName.EndsWith("xlsx")))
                    {
                        string fileName = file.FileName;
                        string fileContentType = file.ContentType;
                        byte[] fileBytes = new byte[file.ContentLength];
                        var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
                        using (var package = new ExcelPackage(file.InputStream))
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



                                //db.etudiants.Add(e);
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
            if (UserValide.IsValid() && UserValide.IsAdmin())
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
            if (UserValide.IsValid() && UserValide.IsAdmin())
            {
                EtudiantContext db = new EtudiantContext();
                //return a  list sorted in a desendent way
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
                            ViewBag.error2 = true;
                            return View();
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
                EnvoyerLesFilieres();
                return RedirectToAction("AttributionFiliere");
            }
            else
                return RedirectToAction("Authentification", "User");
        }

        public ActionResult Statistiques()
        {
            ViewBag.Current = "statistiques";

            if (UserValide.IsValid() && UserValide.IsAdmin())
            {
                //essayons de retourner tous les etudiants
                EtudiantContext db = new EtudiantContext();
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
                ViewBag.nbrTotal = total;

                ViewBag.nbrReste = reste;
                ViewBag.info = info;
                ViewBag.gtr = gtr;
                ViewBag.gpmc = gpmc;
                ViewBag.indus = indus;
                //les pourcentages

                return View();
            }
            else
                return RedirectToAction("Authentification", "User");
        }

        /* public ActionResult Chart()
         {
             //essayons de retourner tous les etudiants
             EtudiantContext db = new EtudiantContext();
             List<Etudiant> list = db.Etudiants.ToList();
             //initialisation des compteurs des filieres
             int info = 0, indus = 0, gtr = 0, gpmc = 0;

             //variable pour les nombre totale et le reste qui n'a pas choisi les filieres
             int nbrTotal = list.Count, nbrReste = 0;

             for (int i = 0; i < nbrTotal; i++)
             {
                 if (list[i].Choix == null)
                 {
                     //un etudiant avec null dans choix alors on va l'es ajouter dans le reste
                     nbrReste++;
                 }
                 //sinon on va traiter les choix comme ca
                 else
                 {
                     if (list[i].Validated)
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

             }

             //les pourcentages
             //double nbrTotalP = Convert.ToDouble(nbrTotal) / Convert.ToDouble(nbrTotal) * 100;
             //double nbrResteP = Convert.ToDouble(nbrReste) / Convert.ToDouble(nbrTotal) * 100;
             double infoP = Convert.ToDouble(info) / Convert.ToDouble(nbrTotal) * 100;
             double gtrP = Convert.ToDouble(gtr) / Convert.ToDouble(nbrTotal) * 100;
             double gpmcP = Convert.ToDouble(gpmc) / Convert.ToDouble(nbrTotal) * 100;
             double indusP = Convert.ToDouble(indus) / Convert.ToDouble(nbrTotal) * 100;


             string[] vx = { "info", "indus", "gtr", "gpmc" };
             double[] vy = { infoP, indusP, gtrP, gpmcP };
             //var chart = new google.visualization.LineChart(document);
             System.Web.Helpers.Chart chart = new System.Web.Helpers.Chart(width: 900, height: 400, theme: ChartTheme.Blue);


             chart.AddSeries(chartType: "Column", xValue: vx, yValues: vy);
             chart.Write("png");
             return null;
         }*/
        public IActionResult chart()
        {
            return View();
        }
        [HttpGet]
        public JsonResult EtudientChart()
        {
            var filiereList = ListChart.GetInfoList();
            return Json(filiereList);
        }
        [HttpPost]
        public void ExtraireNonValide()
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
                Response.Clear();
                Response.ClearContent();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment; filename=EtudiantNonValideCompte.xlsx");
                excel.SaveAs(memoryStream);
                memoryStream.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.Clear();
                Response.End();
            }
        }
        [HttpGet]
        public void ExportExcel()
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



        }

        ///fonction pour les info
        [HttpGet]
        public void ExportExcelAttributed()
        {
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
            worksheet.Cells[1, 5].Value = "Choix";
            worksheet.Cells[1, 6].Value = "Filiere affectee";

            //Remplissage des cellules
            int rowIndex = 2;
            foreach (var student in students.Etudiants.ToList())
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
                Response.Clear();
                Response.ClearContent();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment; filename=listeAttribution.xlsx");
                excel.SaveAs(memoryStream);
                memoryStream.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.Clear();
                Response.End();
            }



        }


        public ActionResult Visualiser()
        {
            ViewBag.Current = "visualiser";
            if (UserValide.IsValid() && UserValide.IsAdmin())
            {
                EtudiantContext db = new EtudiantContext();

                return View(db.Etudiants.ToList());
            }
            else
                return RedirectToAction("Authentification", "User");
        }
        //pour Imprimer le pdf

        public ActionResult PrintConsultation()
        {
            EtudiantContext db = new EtudiantContext();

            var q = new ViewAsPdf("ImprimerEtudiant", db.Etudiants.ToList());

            if (UserValide.IsValid() && UserValide.IsAdmin())
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
