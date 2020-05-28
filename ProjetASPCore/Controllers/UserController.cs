using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjetASPCore.Models;
using ProjetASPCore.Context;
using Microsoft.AspNetCore.Session;

namespace ProjetASPCore.Controllers
{

    public class UserController : Controller
    {

        [HttpGet]
        public ActionResult Authentification()
        {

            if (UserValide.IsValid())
            {

                return RedirectToAction("Index", "Departement");
            }
            else
                return View();

        }
        public ActionResult Resultat()
        {
            EtudiantContext db = new EtudiantContext();
            if (db.Settings.FirstOrDefault().Attributted)
            {
                return View(db.Etudiants.OrderBy(e => e.nom).ToList());
            }
            return RedirectToAction("Authentification1");

        }
        [HttpPost]
        public ActionResult Authentification(Departement login, string ReturnUrl = "")
        {
            String button = HttpContext.Session.GetString("loginBtn");
            string message = "";
            if (button == "Login")
            {
                string userName = HttpContext.Session.GetString("userName");
                string mdp = HttpContext.Session.GetString("mdp");
                EtudiantContext dbset = new EtudiantContext();
                var userLogin = (from data in dbset.Departement
                                 where data.username == userName && data.password == mdp
                                 select data).FirstOrDefault();
                if (userLogin != null)
                {
                    //HttpContext.Session["userName"] = userLogin.username;
                    HttpContext.Session.SetString("userName", userLogin.username);
                    HttpContext.Session.SetString("NomDep", userLogin.nom_departement);
                    HttpContext.Session.SetString("EmailDep", userLogin.email);
                    HttpContext.Session.SetInt32("userId", userLogin.id_departement);
                    HttpContext.Session.SetString("role", "Departement");
                    return RedirectToAction("Index", "Departement");
                }
                else if (userLogin == null)
                {

                    message = "Invalid username or password";
                    ViewBag.Message = message;
                    return View();
                }

            }
            return View();

        }
        public ActionResult Logout()
        {
            HttpContext.Session.SetString("userName", null);
            HttpContext.Session.SetString("NomDep", null);
            HttpContext.Session.SetString("EmailDep", null);
            HttpContext.Session.SetString("userId", null);

            return RedirectToAction("Authentification", "User");
        }





        [HttpGet]
        public ActionResult Authentification1()
        {
            EtudiantContext db = new EtudiantContext();
            ViewBag.Delai = db.Settings.FirstOrDefault().Delai;
            ViewBag.Attributted = db.Settings.FirstOrDefault().Attributted;
            ViewBag.DatedeRappel = db.Settings.FirstOrDefault().DatedeRappel;
            if (UserValide.IsValid())
            {

                return RedirectToAction("Index", "Etudiant");
            }
            else
                return View();

        }

        [HttpPost]
        public ActionResult Authentification1(Etudiant login, string ReturnUrl = "")
        {
            String button = HttpContext.Session.GetString("loginBtn");
            string message = "";
            if (button == "Login")
            {
                string cne = HttpContext.Session.GetString("cne");
                string cin = HttpContext.Session.GetString("cin");
                string mdp = HttpContext.Session.GetString("mdp");
                EtudiantContext dbset = new EtudiantContext();
                ViewBag.Delai = dbset.Settings.FirstOrDefault().Delai;
                ViewBag.Attributted = dbset.Settings.FirstOrDefault().Attributted;
                ViewBag.DatedeRappel = dbset.Settings.FirstOrDefault().DatedeRappel;
                var userLogin = (from data in dbset.Etudiants
                                 where data.cne == cne && data.password == mdp && data.cin == cin && data.Validated == true
                                 select data).FirstOrDefault();
                if (userLogin != null)
                {
                    HttpContext.Session.SetString("cin", userLogin.cin);
                    HttpContext.Session.SetString("userId", userLogin.cne);
                    HttpContext.Session.SetString("nom", userLogin.nom);
                    HttpContext.Session.SetString("prenom", userLogin.prenom);
                    /* Session["nationalite"] = userLogin.nationalite;
                     Session["email"] = userLogin.email;
                     Session["phone"] = userLogin.phone;
                     Session["gsm"] = userLogin.gsm;
                     Session["address"] = userLogin.address;
                     Session["ville"] = userLogin.ville;
                     Session["typeBac"] = userLogin.prenom;
                     Session["anneeBac"] = userLogin.anneeBac;
                     Session["noteBac"] = userLogin.noteBac;
                     Session["mentionBac"] = userLogin.mentionBac;
                     Session["noteFstYear"] = userLogin.noteFstYear;
                     Session["noteSndYear"] = userLogin.noteSndYear;
                     Session["dateNaiss"] = userLogin.dateNaiss;
                     Session["lieuNaiss"] = userLogin.lieuNaiss;
                     Session["photo_link"] = userLogin.photo_link;
                     Session["choix"] = userLogin.choix;*/
                    HttpContext.Session.SetString("role", "Etudiant");
                    return RedirectToAction("Index", "Etudiant");
                }
                else if (userLogin == null)
                {

                    message = "Invalid cin or cne or password";
                    ViewBag.Message = message;
                    return View();
                }

            }
            return View();

        }

    }

}
