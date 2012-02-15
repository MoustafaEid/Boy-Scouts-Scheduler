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
    public class SchedulingConstraintController : Controller
    {
        private SchedulingContext db = new SchedulingContext();

        //
        // GET: /SchedulingConstraint/

        public ViewResult Index(int start = 0, int itemsPerPage = 20, string orderBy = "ID", bool desc = false)
        {
            ViewBag.Count = db.SchedulingConstraints.Count();
            ViewBag.Start = start;
            ViewBag.ItemsPerPage = itemsPerPage;
            ViewBag.OrderBy = orderBy;
            ViewBag.Desc = desc;

            return View();
        }

        public ActionResult GridData(int start = 0, int itemsPerPage = 20, string orderBy = "ID", bool desc = false)
        {
            Response.AppendHeader("X-Total-Row-Count", db.SchedulingConstraints.Count().ToString());
            ObjectQuery<SchedulingConstraint> schedulingconstraints = (db as IObjectContextAdapter).ObjectContext.CreateObjectSet<SchedulingConstraint>();
            schedulingconstraints = schedulingconstraints.OrderBy("it." + orderBy + (desc ? " desc" : ""));

            return PartialView(schedulingconstraints.Skip(start).Take(itemsPerPage));
        }

        //
        // GET: /SchedulingConstraint/Details/5

        public ViewResult Details(int id)
        {
            SchedulingConstraint schedulingconstraint = db.SchedulingConstraints.Find(id);
            return View(schedulingconstraint);
        }

        //
        // GET: /SchedulingConstraint/Create

        public ActionResult Create()
        {
            ViewBag.Groups = db.Groups.ToList();
            ViewBag.Stations = db.Stations.ToList();
            return View();
        } 

        //
        // POST: /SchedulingConstraint/Create

        [HttpPost]
        public ActionResult Create(SchedulingConstraint schedulingconstraint)
        {
            if (ModelState.IsValid)
            {
                schedulingconstraint.Station = db.Stations.Find(schedulingconstraint.Station.ID);
                schedulingconstraint.Group = db.Groups.Find(schedulingconstraint.Group.ID);
                db.SchedulingConstraints.Add(schedulingconstraint);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            return View(schedulingconstraint);
        }
        
        //
        // GET: /SchedulingConstraint/Edit/5
 
        public ActionResult Edit(int id)
        {
            SchedulingConstraint schedulingconstraint = db.SchedulingConstraints.Find(id);
            return View(schedulingconstraint);
        }

        //
        // POST: /SchedulingConstraint/Edit/5

        [HttpPost]
        public ActionResult Edit(SchedulingConstraint schedulingconstraint)
        {
            if (ModelState.IsValid)
            {
                schedulingconstraint.Station = db.Stations.Find(schedulingconstraint.Station.ID);
                schedulingconstraint.Group = db.Groups.Find(schedulingconstraint.Group.ID);
                db.Entry(schedulingconstraint).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(schedulingconstraint);
        }

        //
        // GET: /SchedulingConstraint/Delete/5
 
        public ActionResult Delete(int id)
        {
            SchedulingConstraint schedulingconstraint = db.SchedulingConstraints.Find(id);
            return View(schedulingconstraint);
        }

        //
        // POST: /SchedulingConstraint/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            SchedulingConstraint schedulingconstraint = db.SchedulingConstraints.Find(id);
            db.SchedulingConstraints.Remove(schedulingconstraint);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}