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
    public class TimeSlotController : Controller
    {
        private SchedulingContext db = new SchedulingContext();

        //
        // GET: /TimeSlot/

        public ViewResult Index(int start = 0, int itemsPerPage = 20, string orderBy = "ID", bool desc = false)
        {
            ViewBag.Count = db.TimeSlots.Count();
            ViewBag.Start = start;
            ViewBag.ItemsPerPage = itemsPerPage;
            ViewBag.OrderBy = orderBy;
            ViewBag.Desc = desc;

            return View();
        }

        //
        // GET: /TimeSlot/GridData/?start=0&itemsPerPage=20&orderBy=ID&desc=true

        public ActionResult GridData(int start = 0, int itemsPerPage = 20, string orderBy = "ID", bool desc = false)
        {
            Response.AppendHeader("X-Total-Row-Count", db.TimeSlots.Count().ToString());
            ObjectQuery<TimeSlot> timeslots = (db as IObjectContextAdapter).ObjectContext.CreateObjectSet<TimeSlot>();
            timeslots = timeslots.OrderBy("it." + orderBy + (desc ? " desc" : ""));

            return PartialView(timeslots.Skip(start).Take(itemsPerPage));
        }

        //
        // GET: /Default5/RowData/5

        public ActionResult RowData(int id)
        {
            TimeSlot timeslot = db.TimeSlots.Find(id);
            return PartialView("GridData", new TimeSlot[] { timeslot });
        }

        //
        // GET: /TimeSlot/Create

        public ActionResult Create()
        {
            return PartialView("Edit");
        }

        //
        // POST: /TimeSlot/Create

        [HttpPost]
        public ActionResult Create(TimeSlot timeslot)
        {
            if (ModelState.IsValid)
            {
                db.TimeSlots.Add(timeslot);
                db.SaveChanges();
                return PartialView("GridData", new TimeSlot[] { timeslot });
            }

            return PartialView("Edit", timeslot);
        }

        //
        // GET: /TimeSlot/Edit/5

        public ActionResult Edit(int id)
        {
            TimeSlot timeslot = db.TimeSlots.Find(id);
            return PartialView(timeslot);
        }

        //
        // POST: /TimeSlot/Edit/5

        [HttpPost]
        public ActionResult Edit(TimeSlot timeslot)
        {
            if (ModelState.IsValid)
            {
                db.Entry(timeslot).State = EntityState.Modified;
                db.SaveChanges();
                return PartialView("GridData", new TimeSlot[] { timeslot });
            }

            return PartialView(timeslot);
        }

        //
        // POST: /TimeSlot/Delete/5

        [HttpPost]
        public void Delete(int id)
        {
            TimeSlot timeslot = db.TimeSlots.Find(id);
            db.TimeSlots.Remove(timeslot);
            db.SaveChanges();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
