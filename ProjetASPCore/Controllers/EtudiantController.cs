using ProjetASPCore.Models;
using Rotativa;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;

using MailKit.Net.Smtp;
using MailKit;
using MimeKit;

using Microsoft.AspNetCore.Session;

using ProjetASPCore.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rotativa.AspNetCore;
using ProjetASPCore.Services;

using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;

namespace ProjetASPCore.Controllers
{
    
    public class EtudiantController : Controller
    {

        private readonly IEmailService _emailService;
        IHostingEnvironment _env;
        private  IEtudiantService etudiantService;
        private readonly IDepartementService departementService;
       


        public EtudiantController(IEtudiantService etudiantService,IHostingEnvironment environment)
        {
            _env = environment;
            this.etudiantService = etudiantService;
         
        }

        // GET: Etudiant
        EtudiantContext etudiantContext = new EtudiantContext();
        private readonly string[] ImageEx = new string[] { ".png", ".jpg", ".jpeg", ".jfif", ".svg" };

        public ActionResult Index()
        {
            return View();

        }


        //--------------------------------------------------------------------------------------------------------------------------
        //Modification 
        public IActionResult Modification()
        {
            ViewBag.Current = "Modification";
            ViewBag.err = "";
          
            /* if (id == null)
             {
                 return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
             }*/
            Etudiant etudiant = etudiantService.FindEtudiant("R132580560");

            return View(etudiant);
           
        }
     

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Modification([Bind("cne,nom,prenom,nationalite,cin,email,phone,gsm,address,ville,dateNaiss,lieuNaiss,photo_link,Choix")]Etudiant etudiant, string Upload, String choix1, String choix2, String choix3,IFormFile file)
        {
            if (ModelState.IsValid)
            {
                if (etudiantService.Modification(etudiant, Upload, choix1, choix2, choix3, file))
                {
                    return RedirectToAction("Modification");
                }
                else
                {
                    if (Upload == "Upload")
                    {
                        ViewBag.err = " vous devez selectionner une image";
                        return View(etudiant);
                    }


                }
            }
            else
            {
                return View(etudiant);
            }
            return NotFound();
         

        }
        //****************************************************************************************************************************




        public ActionResult Consulter()
        {
            ViewBag.Current = "Consulter";
            Etudiant etudiant = etudiantService.FindEtudiant("R132580560");

            return View(etudiant);
            
        }
        public ActionResult Recu()
        {
            ViewBag.Current = "Consulter";
            Etudiant etudiant = etudiantService.FindEtudiant("R132580560");

            return new ViewAsPdf("Recu",etudiant);

        }

        public ActionResult Deconnecter()
        {
            HttpContext.Session.SetString("userId", null);
            HttpContext.Session.SetString("cin",null);
            HttpContext.Session.SetString("nom", null);
            HttpContext.Session.SetString("prenom", null);
            HttpContext.Session.SetString("role",null);
            HttpContext.Session.Clear();
            return RedirectToAction("Authentification1", "User");

        }

      



        [HttpGet]
        public ActionResult Inscription()
        {
            //EtudiantContext db = new EtudiantContext();
            //ViewBag.Delai = db.Settings.FirstOrDefault().Delai;
            //ViewBag.DatedeRappel = db.Settings.FirstOrDefault().DatedeRappel;
            ViewBag.prenom = new SelectList(etudiantContext.Etudiants, "cne", "prenom");
            ViewBag.nom = new SelectList(etudiantContext.Etudiants, "cne", "nom");

            ViewBag.typeBac = new List<SelectListItem>
            {
                new SelectListItem {Text="Sciences Physiques et Chimiques", Value="1" },
                new SelectListItem {Text="Sciences Maths A", Value="2" },
                new SelectListItem {Text="Sciences Maths B", Value="3" },
                new SelectListItem {Text="Sciences et Technologies Electriques", Value="4" },
                new SelectListItem {Text="Sciences et Technologies Mécaniques", Value="5" }
            };
            ViewBag.mentionBac = new List<SelectListItem>
            {
                new SelectListItem {Text="Passable", Value="1" },
                new SelectListItem {Text="Assez bien", Value="2" },
                new SelectListItem {Text="Bien", Value="3" },
                new SelectListItem {Text="Très bien", Value="4" },
            };

            Etudiant student = new Etudiant();
            return View(student);
        }

        [HttpPost]
        public ActionResult Inscription(Etudiant student, string choix1, string choix2, string choix3)
        {
            ViewBag.prenom = new SelectList(etudiantContext.Etudiants, "cne", "prenom");
            ViewBag.nom = new SelectList(etudiantContext.Etudiants, "cne", "nom");
            EtudiantContext db = new EtudiantContext();
            ViewBag.Delai = db.Settings.FirstOrDefault().Delai;
            ViewBag.DatedeRappel = db.Settings.FirstOrDefault().DatedeRappel;
            ViewBag.typeBac = new List<SelectListItem>
            {
                new SelectListItem {Text="Sciences Physiques et Chimiques", Value="1" },
                new SelectListItem {Text="Sciences Maths A", Value="2" },
                new SelectListItem {Text="Sciences Maths B", Value="3" },
                new SelectListItem {Text="Sciences et Technologies Electriques", Value="4" },
                new SelectListItem {Text="Sciences et Technologies Mécaniques", Value="5" }
            };
            ViewBag.mentionBac = new List<SelectListItem>
            {
                new SelectListItem {Text="Passable", Value="1" },
                new SelectListItem {Text="Assez bien", Value="2" },
                new SelectListItem {Text="Bien", Value="3" },
                new SelectListItem {Text="Très bien", Value="4" },
            };

            if (ModelState.IsValid)
            {
                var e = etudiantContext.Etudiants.Where(x => x.cne == student.cne && x.nom == student.nom && x.prenom == student.prenom).FirstOrDefault();

                if (e == null)
                {
                    ViewBag.message = "Les informations que vous avez entrez ne correspondent à aucun étudiant !";
                    return View();
                }
                else if (e.Validated == true)
                {
                    ViewBag.message = "Cet étudiant est déjà inscrit.";
                    return View();
                }
                else
                {
                    e.Validated = true;
                    e.password = student.password;
                    e.nationalite = student.nationalite;
                    e.email = student.email;
                    e.phone = student.phone;
                    e.gsm = student.gsm;
                    e.address = student.address;
                    e.ville = student.ville;
                    e.typeBac = student.typeBac;
                    e.anneeBac = student.anneeBac;
                    e.noteBac = student.noteBac;
                    e.mentionBac = student.mentionBac;
                    e.dateNaiss = student.dateNaiss;
                    e.lieuNaiss = student.lieuNaiss;
                    e.Choix = choix1 + choix2 + choix3;
                    etudiantContext.SaveChanges();

                    return RedirectToAction("SendEmailToUser1", new { email = e.email.ToString(), nom = e.nom.ToString(), prenom = e.prenom.ToString() });
                }
            }

            else return View();
        }


        public ActionResult SendEmailToUser()
        {

            bool Result = false;
            Etudiant etudiants = etudiantContext.Etudiants.Find(HttpContext.Session.GetString("userId"));

            string email = etudiants.email;
            string subject = "Modification";
            ViewBag.nom = etudiants.nom;
            ViewBag.prenom = etudiants.prenom;
            var Resulta = SendEmailAsync(email, subject, "<p> Hello" + " " + @ViewBag.nom + " " + @ViewBag.prenom + ",<br/>some modifications had been done <br />Verify your account </p>" +
                "<button color='blue'><a href='localhost:localhost:52252/User/Authentification1'>Cliquer ici!</a></button>");
            if (Resulta !=null)
            {

                Json(Result, new Newtonsoft.Json.JsonSerializerSettings());

                return RedirectToAction("Modification");
            }
            return View();
        }
        public ActionResult SendEmailToUser1(String email, String nom, String prenom)
        {
            
            string subject = "Inscription";
            var Result = SendEmailAsync(email, subject, "<p> Hello" + " " + nom + " " + prenom + ",<br/>Vous avez inscrit sur la plateforme d'ensas <br />Veuillez verifier votre compte </p>" +
                "<button color='blue'><a href='localhost:localhost:52252/User/Authentification1'>Cliquer ici!</a></button>");
            if (Result.AsyncState!=null)
            {
                Json(Result, new Newtonsoft.Json.JsonSerializerSettings());

                return RedirectToAction("Authentification1", "User");
            }
            return View();
        }
        [HttpPost]
        
        public async Task<IActionResult> SendEmailAsync(string email, string subject, string message)
        {
            await _emailService.SendEmailAsync(email, subject, message);
            return Ok();
        }
    }
}