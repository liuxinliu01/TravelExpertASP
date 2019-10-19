//Modified by John

using PurchaseList.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PurchaseList.Controllers
{
    public class HomeController : Controller
    {
        private TravelExpertsEntities3 db = new TravelExpertsEntities3();
        public ActionResult Index()
        {
            List<Package> packages = db.Packages.ToList();
            return View(packages);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Travel Experts";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Contact Us";

            return View();
        }
    }
}