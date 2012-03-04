using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Objects;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Boy_Scouts_Scheduler.Models;

namespace Boy_Scouts_Scheduler.Controllers
{
    public class HomeController : Controller
    {
        private SchedulingContext db = new SchedulingContext();

        public ActionResult Index()
        {
            ViewBag.Events = db.Events.ToList();
            return View();
        }   
    }
}
