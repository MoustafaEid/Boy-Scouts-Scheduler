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
	[Authorize]
    public class StationController : Controller
    {
        private SchedulingContext db = new SchedulingContext();
        private int eventID
        {
            get { return Convert.ToInt32(Request.Cookies["event"].Value); }
        }

        //
        // GET: /NewStation/

        public ViewResult Index(int start = 0, int itemsPerPage = 20, string orderBy = "ID", bool desc = false)
        {
            ViewBag.Count = db.Stations.Count(s => s.Event.ID == eventID);
            ViewBag.Start = start;
            ViewBag.ItemsPerPage = itemsPerPage;
            ViewBag.OrderBy = orderBy;
            ViewBag.Desc = desc;

            return View();
        }

        //
        // GET: /Station/GridData/?start=0&itemsPerPage=20&orderBy=ID&desc=true

        public ActionResult GridData(int start = 0, int itemsPerPage = 20, string orderBy = "ID", bool desc = false)
        {
            Response.AppendHeader("X-Total-Row-Count", db.Stations.Count(s => s.Event.ID == eventID).ToString());
            ObjectQuery<Station> stations = (db as IObjectContextAdapter).ObjectContext.CreateObjectSet<Station>();
            stations = stations.OrderBy("it." + orderBy + (desc ? " desc" : ""));

            return PartialView(stations.Where(s => s.Event.ID == eventID).Skip(start).Take(itemsPerPage));
        }

        //
        // GET: /Station/Create

        public ActionResult Create()
        {
            PrepareTimeSlotCheckBoxes();
            return View();
        } 

        //
        // POST: /Station/Create

        [HttpPost]
        public ActionResult Create(Station station, int[] TimeSlotIDs)
        {
            if (ModelState.IsValid)
            {
                station.Event = db.Events.Find(eventID);
                station.AvailableTimeSlots = new List<TimeSlot>();
                if (TimeSlotIDs != null)
                {
                    foreach (var timeSlotID in TimeSlotIDs)
                    {
                        station.AvailableTimeSlots.Add(db.TimeSlots.Find(timeSlotID));
                    }
                }
                db.Stations.Add(station);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            PrepareTimeSlotCheckBoxes();
            return View("Edit", station);
        }
        
        //
        // GET: /Station/Edit/5
 
        public ActionResult Edit(int id)
        {
            Station station = db.Stations.Find(id);
            PrepareTimeSlotCheckBoxes();
            return View(station);
        }

        //
        // POST: /Station/Edit/5

        [HttpPost]
        public ActionResult Edit(Station station, int[] TimeSlotIDs)
        {
            if (ModelState.IsValid)
            {
                var availableTimeSlots = new List<TimeSlot>();
                if (TimeSlotIDs != null)
                {
                    foreach (var timeSlotID in TimeSlotIDs)
                    {
                        availableTimeSlots.Add(db.TimeSlots.Find(timeSlotID));
                    }
                }
                db.Stations.Attach(station);
                db.Entry(station).Collection(s => s.AvailableTimeSlots).Load();
                station.AvailableTimeSlots = availableTimeSlots;
                db.Entry(station).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            PrepareTimeSlotCheckBoxes();
            return View(station);
        }

        //
        // GET: /Station/Delete/5

        [HttpPost]
        public void Delete(int id)
        {
            Station station = db.Stations.Find(id);
            db.Stations.Remove(station);
            db.SaveChanges();
        }

        //public ActionResult Delete(int id)
        //{
        //    Station station = db.Stations.Find(id);
        //    return View(station);
        //}

        ////
        //// POST: /Station/Delete/5

        //[HttpPost, ActionName("Delete")]
        //public ActionResult DeleteConfirmed(int id)
        //{            
        //    Station station = db.Stations.Find(id);
        //    db.Stations.Remove(station);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}
        
        protected void PrepareTimeSlotCheckBoxes() {
            // TODO: select timeslots only from current event
            ViewBag.TimeSlots =
                (from slot in db.TimeSlots
                 where slot.Event.ID == eventID && slot.isGeneral == false
                group slot by new
                {
                    y = slot.Start.Year,
                    m = slot.Start.Month,
                    d = slot.Start.Day
                }).ToList().Select(g => g.OrderBy(slot => slot.Start).GetEnumerator()).ToArray();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}