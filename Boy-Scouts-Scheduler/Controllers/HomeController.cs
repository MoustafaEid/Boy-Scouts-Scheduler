using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Boy_Scouts_Scheduler.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to kick-start your ASP.NET MVC application.";

            return View();
        }

        public ActionResult Help()
        {
            return View();
        }

        public ActionResult Import()
        {
            return View();
        }
        
        public ActionResult Schedules()
        {
            return View();
        }
    }
}
