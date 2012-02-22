using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Boy_Scouts_Scheduler.Controllers
{
    public class HelpController : Controller
    {
        //
        // GET: /Help/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ImportHelp()
        {
            return View();
        }

        public ActionResult TimeSlotHelp()
        {
            return View();
        }

        public ActionResult GroupHelp()
        {
            return View();
        }

        public ActionResult StationHelp()
        {
            return View();
        }

        public ActionResult ConstraintHelp()
        {
            return View();
        }

        public ActionResult ScheduleHelp()
        {
            return View();
        }

    }
}
