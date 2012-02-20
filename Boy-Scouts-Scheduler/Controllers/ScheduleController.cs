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
    public class ScheduleController : Controller
    {
        private SchedulingContext db = new SchedulingContext();

        public ActionResult Generate()
        {
            List<Activity> schedule;
            IEnumerator<Activity> enumerator;

            IEnumerable<Group> groupData = 
                from item in db.Groups
                select item;

            IEnumerable<Station> stationData =
                from item in db.Stations
                select item;

            IEnumerable<TimeSlot> timeslotData =
                from item in db.TimeSlots
                select item;

            IEnumerable<SchedulingConstraint> constraintData =
                from item in db.SchedulingConstraints
                select item;
                
           //schedule = Boy-Scouts-Scheduler.Scheduler.Schedule(groupData, stationData, timeslotData, constraintData);
           //enumerator = schedule.GetEnumerator();

            //while (enumerator.MoveNext())
            //{
            //    db.Activities.Add(enumerator.Current);
            //}

            return View(); //ScheduleView(schedule);
        }

        //
        // GET: /Schedule/

        public ViewResult Index(int start = 0, int itemsPerPage = 20, string orderBy = "ID", bool desc = false)
        {
            ViewBag.Count = db.Activities.Count();
            ViewBag.Start = start;
            ViewBag.ItemsPerPage = itemsPerPage;
            ViewBag.OrderBy = orderBy;
            ViewBag.Desc = desc;

            return View();
        }

        //
        // GET: /Schedule/GridData/?start=0&itemsPerPage=20&orderBy=ID&desc=true

        public ActionResult GridData(int start = 0, int itemsPerPage = 20, string orderBy = "ID", bool desc = false)
        {
            Response.AppendHeader("X-Total-Row-Count", db.Activities.Count().ToString());
            ObjectQuery<Activity> activities = (db as IObjectContextAdapter).ObjectContext.CreateObjectSet<Activity>();
            activities = activities.OrderBy("it." + orderBy + (desc ? " desc" : ""));

            return PartialView(activities.Skip(start).Take(itemsPerPage));
        }

        //
        // GET: /Default5/RowData/5

        public ActionResult RowData(int id)
        {
            Activity activity = db.Activities.Find(id);
            return PartialView("GridData", new Activity[] { activity });
        }

        //
        // GET: /Schedule/Create

        public ActionResult Create()
        {
            return PartialView("Edit");
        }

        //
        // POST: /Schedule/Create

        [HttpPost]
        public ActionResult Create(Activity activity)
        {
            if (ModelState.IsValid)
            {
                db.Activities.Add(activity);
                db.SaveChanges();
                return PartialView("GridData", new Activity[] { activity });
            }

            return PartialView("Edit", activity);
        }

        //
        // GET: /Schedule/Edit/5

        public ActionResult Edit(int id)
        {
            Activity activity = db.Activities.Find(id);
            return PartialView(activity);
        }

        //
        // POST: /Schedule/Edit/5

        [HttpPost]
        public ActionResult Edit(Activity activity)
        {
            if (ModelState.IsValid)
            {
                db.Entry(activity).State = EntityState.Modified;
                db.SaveChanges();
                return PartialView("GridData", new Activity[] { activity });
            }

            return PartialView(activity);
        }

        //
        // POST: /Schedule/Delete/5

        [HttpPost]
        public void Delete(int id)
        {
            Activity activity = db.Activities.Find(id);
            db.Activities.Remove(activity);
            db.SaveChanges();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
