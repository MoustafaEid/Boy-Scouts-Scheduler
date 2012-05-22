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
    public class ScheduleController : Controller
    {
        private SchedulingContext db = new SchedulingContext();
        private int eventID
        {
            get { return Convert.ToInt32(Request.Cookies["event"].Value); }
        }

        public ActionResult Generate(int startSlotID)
        {
            var currentEvent = db.Events.Find(eventID);
            TimeSlot startSlot = db.TimeSlots.Find(startSlotID);

			IEnumerable<Activity> schedule;
            IEnumerator<Activity> scheduleEnumerator;

            IEnumerable<Group> groupData = 
                from item in db.Groups.Include(g => g.Type) // Preloading GroupType so that it isn't lazy loaded in the GreedyScheduler.
                where item.Event.ID == eventID
                select item;

            IEnumerable<Station> stationData =
                from item in db.Stations.Include(s => s.AvailableTimeSlots) // Workaround for not having MultipleActiveResultSets on AppHarbor
                                                                            // since the GreedyScheduler was lazy loading AvailableTimeSlots
                                                                            // after the DB had already been queried to clear the old schedule
                where item.Event.ID == eventID
                select item;

            IEnumerable<TimeSlot> timeslotData =
                from item in db.TimeSlots
                where item.Event.ID == eventID
                orderby item.Start ascending
                select item;

            IEnumerable<TimeSlot> generalSlots =
                from item in db.TimeSlots
                where item.Event.ID == eventID && item.isGeneral
                orderby item.Start ascending
                select item;

            IEnumerable<SchedulingConstraint> constraintData =
                from item in db.SchedulingConstraints.Include(c => c.Group).Include(c => c.GroupType).Include(c => c.Station)
                where item.Event.ID == eventID
                select item;

            IEnumerable<Activity> activityData =
                from item in db.Activities
                where item.Event.ID == eventID
                select item;

            // call algorithm to generate schedule
            schedule = Boy_Scouts_Scheduler.Algorithm.Scheduler.Schedule(groupData.ToList(), stationData, constraintData, timeslotData, activityData, startSlot);

	        ClearSchedule();

            scheduleEnumerator = schedule.GetEnumerator();
            while (scheduleEnumerator.MoveNext())
            {
                db.Activities.Add(new Activity
                {
                    Event = currentEvent,
                    Group = scheduleEnumerator.Current.Group,
                    TimeSlot = scheduleEnumerator.Current.TimeSlot,
                    Station = scheduleEnumerator.Current.Station
                });

                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        public ActionResult ClearSchedule()
        {
            db.Database.SqlQuery<object>("DELETE FROM [dbo].[Activities] WHERE Event_ID={0}", eventID).ToList();

            return RedirectToAction("Index");
        }

        //
        // GET: /Schedule/

        public ViewResult Index(int start = 0, int itemsPerPage = 20, string orderBy = "ID", bool desc = false)
        {
            ViewBag.Count = db.Activities.Count(a => a.Event.ID == eventID);
            ViewBag.Start = start;
            ViewBag.ItemsPerPage = itemsPerPage;
            ViewBag.OrderBy = orderBy;
            ViewBag.Desc = desc;

            ViewBag.StartDate = db.Events.Find(eventID).Start;
            ViewBag.Groups = db.Groups.Where(g => g.Event.ID == eventID).OrderBy(g => g.Name);
            ViewBag.Stations = db.Stations.Where(s => s.Event.ID == eventID).OrderBy(s => s.Name);
            ViewBag.TimeSlots = db.TimeSlots.Where(t => t.Event.ID == eventID);

            return View();
        }

        public JsonResult GroupActivities(int ID)
        {
            var group = db.Groups.Find(ID);
            List<Activity> activities = db.Activities.Where(a => a.Event.ID == eventID).ToList();
            foreach (var timeSlot in db.TimeSlots.Where(t => t.Event.ID == eventID && t.isGeneral))
            {
                    activities.Add(new Activity
                    {
                        Group = group,
                        TimeSlot = timeSlot,
                        Station = new Station { ID = -1, Name = timeSlot.Name }
                    });
            }

            return Json((from activity in activities
                         where activity.Group.ID == ID
                         select new
                         {
                             ID = activity.ID,
                             StationName = activity.Station.Name,
                             Start = activity.TimeSlot.Start,
                             End = activity.TimeSlot.End
                         }).AsEnumerable().OrderBy(a => a.StationName).Select(a => new
                         {
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
                        }).AsEnumerable().OrderBy(a => a.GroupName).Select(a => new {
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
            Response.AppendHeader("X-Total-Row-Count", db.Activities.Count(a => a.Event.ID == eventID).ToString());
            ObjectQuery<Activity> activities = (db as IObjectContextAdapter).ObjectContext.CreateObjectSet<Activity>();
            activities = activities.OrderBy("it." + orderBy + (desc ? " desc" : ""));

            return PartialView(activities.Where(a => a.Event.ID == eventID).Skip(start).Take(itemsPerPage));
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
                activity.Event = db.Events.Find(eventID);
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
