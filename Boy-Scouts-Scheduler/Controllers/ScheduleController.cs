﻿using System;
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

        public ActionResult Generate(TimeSlot startSlot)
        {
            db.Database.SqlQuery<object>("TRUNCATE TABLE [Boy_Scouts_Scheduler.Models.SchedulingContext].[dbo].[Activities]").ToList();

			IEnumerable<Activity> schedule;
            IEnumerator<Activity> enumerator;

            IEnumerable<Group> groupData = 
                from item in db.Groups
                select item;

            IEnumerable<Station> stationData =
                from item in db.Stations
                select item;

            IEnumerable<TimeSlot> timeslotData =
                from item in db.TimeSlots
                orderby item.Start ascending
                select item;

            IEnumerable<SchedulingConstraint> constraintData =
                from item in db.SchedulingConstraints
                select item;

            IEnumerable<Activity> activityData =
                from item in db.Activities
                select item;

           // call algorithm to generate schedule
           schedule = Boy_Scouts_Scheduler.Algorithm.Scheduler.Schedule(groupData, stationData, constraintData, timeslotData, activityData, startSlot);

           enumerator = schedule.GetEnumerator();
           while (enumerator.MoveNext())
           {
               db.Activities.Add(new Activity
               {
                   Group = enumerator.Current.Group,
                   TimeSlot = enumerator.Current.TimeSlot,
                   Station = enumerator.Current.Station
               });

               db.SaveChanges();
           }

           return RedirectToAction("Index");
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

            ViewBag.StartDate = db.Events.First().Start; // TODO: Deal with multiple events
            ViewBag.Groups = db.Groups;
            ViewBag.Stations = db.Stations;
            ViewBag.TimeSlots = db.TimeSlots;

            return View();
        }

        public JsonResult GroupActivities(int ID)
        {
            return Json((from activity in db.Activities
                        where activity.Group.ID == ID
                        select new {
                            ID = activity.ID,
                            StationName = activity.Station.Name,
                            Start = activity.TimeSlot.Start,
                            End = activity.TimeSlot.End
                        }).AsEnumerable().Select(a => new {
                            ID = a.ID,
                            Name = a.StationName,
                            Start = a.Start.ToString(),
                            End = a.End.ToString()
                        }));
        }

        public JsonResult StationActivities(int ID)
        {
            return Json((from activity in db.Activities
                        where activity.Station.ID == ID
                        select new
                        {
                            ID = activity.ID,
                            GroupName = activity.Group.Name,
                            Start = activity.TimeSlot.Start,
                            End = activity.TimeSlot.End
                        }).AsEnumerable().Select(a => new {
                            ID = a.ID,
                            Name = a.GroupName,
                            Start = a.Start.ToString(),
                            End = a.End.ToString()
                        }));
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
