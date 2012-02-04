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
    public class ImportController : Controller
    {
        private SchedulingContext db = new SchedulingContext();

        //
        // GET: /Import/

        public ViewResult Index(int start = 0, int itemsPerPage = 20, string orderBy = "ID", bool desc = false)
        {
            ViewBag.Count = db.Attendees.Count();
            ViewBag.Start = start;
            ViewBag.ItemsPerPage = itemsPerPage;
            ViewBag.OrderBy = orderBy;
            ViewBag.Desc = desc;

            return View();
        }

        //
        // GET: /Import/GridData/?start=0&itemsPerPage=20&orderBy=ID&desc=true

        public ActionResult GridData(int start = 0, int itemsPerPage = 20, string orderBy = "ID", bool desc = false)
        {
            Response.AppendHeader("X-Total-Row-Count", db.Attendees.Count().ToString());
            ObjectQuery<Attendee> attendees = (db as IObjectContextAdapter).ObjectContext.CreateObjectSet<Attendee>();
            attendees = attendees.OrderBy("it." + orderBy + (desc ? " desc" : ""));

            return PartialView(attendees.Skip(start).Take(itemsPerPage));
        }

        //
        // GET: /Default5/RowData/5

        public ActionResult RowData(int id)
        {
            Attendee attendee = db.Attendees.Find(id);
            return PartialView("GridData", new Attendee[] { attendee });
        }

        //
        // GET: /Import/Create

        public ActionResult Create()
        {
            return PartialView("Edit");
        }

        //
        // POST: /Import/Create

        [HttpPost]
        public ActionResult Create(Attendee attendee)
        {
            if (ModelState.IsValid)
            {
                db.Attendees.Add(attendee);
                db.SaveChanges();
                return PartialView("GridData", new Attendee[] { attendee });
            }

            return PartialView("Edit", attendee);
        }

        //
        // GET: /Import/Edit/5

        public ActionResult Edit(int id)
        {
            Attendee attendee = db.Attendees.Find(id);
            return PartialView(attendee);
        }

        //
        // POST: /Import/Edit/5

        [HttpPost]
        public ActionResult Edit(Attendee attendee)
        {
            if (ModelState.IsValid)
            {
                db.Entry(attendee).State = EntityState.Modified;
                db.SaveChanges();
                return PartialView("GridData", new Attendee[] { attendee });
            }

            return PartialView(attendee);
        }

        //
        // POST: /Import/Delete/5

        [HttpPost]
        public void Delete(int id)
        {
            Attendee attendee = db.Attendees.Find(id);
            db.Attendees.Remove(attendee);
            db.SaveChanges();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
