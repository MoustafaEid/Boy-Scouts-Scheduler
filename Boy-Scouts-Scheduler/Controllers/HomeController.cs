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
