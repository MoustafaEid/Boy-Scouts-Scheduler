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
    public class GroupController : Controller
    {
        private SchedulingContext db = new SchedulingContext();
        private int eventID
        {
            get { return Convert.ToInt32(Request.Cookies["event"].Value); }
        }

        //
        // GET: /Group/

        public ViewResult Index(int start = 0, int itemsPerPage = 20, string orderBy = "ID", bool desc = false)
        {
            ViewBag.Count = db.Groups.Count(g => g.Event.ID == eventID);
            ViewBag.Start = start;
            ViewBag.ItemsPerPage = itemsPerPage;
            ViewBag.OrderBy = orderBy;
            ViewBag.Desc = desc;

            return View();
        }

        //
        // GET: /Group/GridData/?start=0&itemsPerPage=20&orderBy=ID&desc=true

        public ActionResult GridData(int start = 0, int itemsPerPage = 20, string orderBy = "ID", bool desc = false)
        {
            Response.AppendHeader("X-Total-Row-Count", db.Groups.Count(g => g.Event.ID == eventID).ToString());
            ObjectQuery<Group> groups = (db as IObjectContextAdapter).ObjectContext.CreateObjectSet<Group>();
            groups = groups.OrderBy("it." + orderBy + (desc ? " desc" : ""));

            return PartialView(groups.Where(g => g.Event.ID == eventID).Skip(start).Take(itemsPerPage)
                                     .Include(t => t.Type));
        }

        //
        // GET: /Default5/RowData/5

        public ActionResult RowData(int id)
        {
            Group group = db.Groups.Find(id);
            return PartialView("GridData", new Group[] { group });
        }

        //
        // GET: /Group/Create

        public ActionResult Create()
        {
            return PartialEditView();
        }

        //
        // POST: /Group/Create

        [HttpPost]
        public ActionResult Create(Group group)
        {
            if (ModelState.IsValid)
            {
                group.Event = db.Events.Find(eventID);
                group.Type = db.GroupTypes.Find(group.TypeID);
                group.Preference1 = db.Stations.Find(group.Preference1.ID);
                group.Preference2 = db.Stations.Find(group.Preference2.ID);
                group.Preference3 = db.Stations.Find(group.Preference3.ID);
                group.Preference4 = db.Stations.Find(group.Preference4.ID);
                group.Preference5 = db.Stations.Find(group.Preference5.ID);
                db.Groups.Add(group);
                db.SaveChanges();
                return PartialView("GridData", new Group[] { group });
            }
            return PartialEditView(group);
        }

        //
        // GET: /Group/Edit/5

        public ActionResult Edit(int id)
        {
            Group group = db.Groups.Find(id);
            return PartialEditView(group);
        }

        //
        // POST: /Group/Edit/5

        [HttpPost]
        public ActionResult Edit(Group group)
        {
            if (ModelState.IsValid)
            {
                Group origGroup = db.Groups
                                    .Include(g => g.Preference1)
                                    .Include(g => g.Preference2)
                                    .Include(g => g.Preference3)
                                    .Include(g => g.Preference4)
                                    .Include(g => g.Preference5)
                                    .Single(g => g.ID == group.ID);
                db.Entry(origGroup).CurrentValues.SetValues(group);
                origGroup.Preference1 = db.Stations.Find(group.Preference1.ID);
                origGroup.Preference2 = db.Stations.Find(group.Preference2.ID);
                origGroup.Preference3 = db.Stations.Find(group.Preference3.ID);
                origGroup.Preference4 = db.Stations.Find(group.Preference4.ID);
                origGroup.Preference5 = db.Stations.Find(group.Preference5.ID);
                db.SaveChanges();
                return PartialView("GridData", new Group[] { origGroup });
            }
            return PartialEditView(group);
        }

        //
        // POST: /Group/Delete/5

        [HttpPost]
        public void Delete(int id)
        {
            Group group = db.Groups.Find(id);
            db.Groups.Remove(group);
            db.SaveChanges();
        }

        protected PartialViewResult PartialEditView(Group group = null)
        {
            ViewBag.GroupTypes = db.GroupTypes.ToList();
            ViewBag.Stations = db.Stations.Where(s => s.Event.ID == eventID).ToList();
            ViewBag.Stations.Insert(0, new Station { ID = -1 }); // Allow null preferences
            return PartialView("Edit", group);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
