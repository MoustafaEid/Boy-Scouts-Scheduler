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
    public class EventController : Controller
    {
        private SchedulingContext db = new SchedulingContext();

        //
        // GET: /Event/
        public JsonResult Index()
        {
            return Json(db.Events.ToList());
        }

        //
        // GET: /Event/5
        [HttpPost]
        public void Select(int id)
        {
            Response.AppendCookie(new HttpCookie("event", id.ToString()));
        }

        //
        // POST: /Event/Create

        [HttpPost]
        public JsonResult Create(Event e)
        {
            if (ModelState.IsValid)
            {
                db.Events.Add(e);
                db.SaveChanges();

                Response.AppendCookie(new HttpCookie("event", e.ID.ToString()));

                return Json(e);
            } else {
                throw new Exception("Could not model bind event.");
            }
        }

        //
        // POST: /Event/Edit/5

        [HttpPost]
        public JsonResult Edit(Event e)
        {
            if (ModelState.IsValid)
            {
                db.Entry(e).State = EntityState.Modified;
                db.SaveChanges();
                return Json(e);
            } else {
                throw new Exception("Could not model bind event.");
            }
        }

        //
        // POST: /Event/Delete/5

        [HttpPost]
        public void Delete(int id)
        {
            Event e = db.Events.Find(id);
            db.Events.Remove(e);
            db.SaveChanges();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
