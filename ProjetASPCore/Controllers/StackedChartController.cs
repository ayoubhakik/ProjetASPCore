using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProjetASPCore.Models;

namespace ProjetASPCore.Controllers
{
    public class StackedChartController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public JsonResult PopulationChart()
        {
            var populationList = ListStatic.GetCityPopulationList();
            return Json(populationList);
        }
    }
}