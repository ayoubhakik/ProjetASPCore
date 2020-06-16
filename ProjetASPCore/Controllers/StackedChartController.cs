using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjetASPCore.Models;

namespace ProjetASPCore.Controllers
{
    public class StackedChartController : Controller
    {
        public IActionResult Index()
        {
            var h = HttpContext.Session.GetInt32("userId");
            var h1 = HttpContext.Session.GetString("role");

            if (h != null && h1.Equals("Departement"))
            {
                return View();
            }
            else
                
                    return RedirectToAction("Authentification", "User");

        }
        [HttpGet]
        public JsonResult PopulationChart()
        {
            var populationList = ListStatic.GetCityPopulationList();
            return Json(populationList);
        }
    }
}