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

        //
        // GET: /Group/

        public ViewResult Index(int start = 0, int itemsPerPage = 20, string orderBy = "ID", bool desc = false)
        {
            ViewBag.Count = db.Groups.Count();
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
            Response.AppendHeader("X-Total-Row-Count", db.Groups.Count().ToString());
            ObjectQuery<Group> groups = (db as IObjectContextAdapter).ObjectContext.CreateObjectSet<Group>();
            groups = groups.OrderBy("it." + orderBy + (desc ? " desc" : ""));

            return PartialView(groups.Skip(start).Take(itemsPerPage));
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
            return PartialView("Edit");
        }

        //
        // POST: /Group/Create

        [HttpPost]
        public ActionResult Create(Group group)
        {
            if (ModelState.IsValid)
            {
                db.Groups.Add(group);
                db.SaveChanges();
                return PartialView("GridData", new Group[] { group });
            }

            return PartialView("Edit", group);
        }

        //
        // GET: /Group/Edit/5

        public ActionResult Edit(int id)
        {
            Group group = db.Groups.Find(id);
            return PartialView(group);
        }

        //
        // POST: /Group/Edit/5

        [HttpPost]
        public ActionResult Edit(Group group)
        {
            if (ModelState.IsValid)
            {
                db.Entry(group).State = EntityState.Modified;
                db.SaveChanges();
                return PartialView("GridData", new Group[] { group });
            }

            return PartialView(group);
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

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
