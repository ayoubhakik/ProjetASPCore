using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjetASPCore.Models;
using ProjetASPCore.Context;
using Microsoft.AspNetCore.Session;
using Microsoft.AspNetCore.Identity;

namespace ProjetASPCore.Controllers
{

    public class UserController : Controller
    {

        [HttpGet]
        public ActionResult Authentification()
        {
            if (HttpContext.Session.GetInt32("userId") != null)
            {
                return RedirectToAction("Index", "Departement");
            }
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

            String button = Request.Form["loginBtn"];
            string message = "";
            if (button == "Login")
            {
                string userName = Request.Form["userName"];
                string mdp = Request.Form["mdp"];

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
            HttpContext.Session.Remove("userName");

            HttpContext.Session.Remove("NomDep");

            HttpContext.Session.Remove("EmailDep");
            HttpContext.Session.Remove("userId");


            return RedirectToAction("Authentification", "User");
        }





        [HttpGet]
        public ActionResult Authentification1()
        {
            EtudiantContext db = new EtudiantContext();
            ViewBag.Delai = db.Settings.FirstOrDefault().Delai;
            ViewBag.Attributted = db.Settings.FirstOrDefault().Attributted;
            ViewBag.DatedeRappel = db.Settings.FirstOrDefault().DatedeRappel;
            if (HttpContext.Session.GetString("userId") == null)
            {
                return View();

            }
            else
                return RedirectToAction("Index", "Etudiant");

        }

        [HttpPost]
        public ActionResult Authentification1(Etudiant login, string ReturnUrl = "")
        {


            String button = Request.Form["loginBtn"];
            string message = "";
            if (button == "Login")
            {
                string cne = Request.Form["cne"];
                string cin = Request.Form["cin"];
                string mdp = Request.Form["mdp"];
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
