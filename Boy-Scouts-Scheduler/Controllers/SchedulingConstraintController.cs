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

        //
        // GET: /SchedulingConstraint/GridData/?start=0&itemsPerPage=20&orderBy=ID&desc=true

        public ActionResult GridData(int start = 0, int itemsPerPage = 20, string orderBy = "ID", bool desc = false)
        {
            Response.AppendHeader("X-Total-Row-Count", db.SchedulingConstraints.Count().ToString());
            ObjectQuery<SchedulingConstraint> schedulingconstraints = (db as IObjectContextAdapter).ObjectContext.CreateObjectSet<SchedulingConstraint>();
            schedulingconstraints = schedulingconstraints.OrderBy("it." + orderBy + (desc ? " desc" : ""));

            return PartialView(schedulingconstraints.Skip(start).Take(itemsPerPage));
        }

        //
        // GET: /Default5/RowData/5

        public ActionResult RowData(int id)
        {
            SchedulingConstraint schedulingconstraint = db.SchedulingConstraints.Find(id);
            return PartialView("GridData", new SchedulingConstraint[] { schedulingconstraint });
        }

        //
        // GET: /SchedulingConstraint/Create

        public ActionResult Create()
        {
            //return PartialView("Edit");
            return PartialEditView();
        }

        //
        // POST: /SchedulingConstraint/Create

        [HttpPost]
        public ActionResult Create(SchedulingConstraint schedulingconstraint)
        {
            if (ModelState.IsValid)
            {
                schedulingconstraint.Group = db.Groups.Find(schedulingconstraint.Group.ID);
                schedulingconstraint.GroupType = db.GroupTypes.Find(schedulingconstraint.GroupType.ID);
                schedulingconstraint.Station = db.Stations.Find(schedulingconstraint.Station.ID);
                db.SchedulingConstraints.Add(schedulingconstraint);
                db.SaveChanges();
                return PartialView("GridData", new SchedulingConstraint[] { schedulingconstraint });
            }

            return PartialEditView(schedulingconstraint);
        }

        //
        // GET: /SchedulingConstraint/Edit/5

        public ActionResult Edit(int id)
        {
            SchedulingConstraint schedulingconstraint = db.SchedulingConstraints.Find(id);
            return PartialEditView(schedulingconstraint);
        }

        //
        // POST: /SchedulingConstraint/Edit/5

        [HttpPost]
        public ActionResult Edit(SchedulingConstraint schedulingconstraint)
        {
            if (ModelState.IsValid)
            {
                schedulingconstraint.Group = db.Groups.Find(schedulingconstraint.Group.ID);
                schedulingconstraint.GroupType = db.GroupTypes.Find(schedulingconstraint.GroupType.ID);
                schedulingconstraint.Station = db.Stations.Find(schedulingconstraint.Station.ID);
                db.Entry(schedulingconstraint).State = EntityState.Modified;
                db.SaveChanges();
                return PartialView("GridData", new SchedulingConstraint[] { schedulingconstraint });
            }

            return PartialEditView(schedulingconstraint);
        }

        //
        // POST: /SchedulingConstraint/Delete/5

        [HttpPost]
        public void Delete(int id)
        {
            SchedulingConstraint schedulingconstraint = db.SchedulingConstraints.Find(id);
            db.SchedulingConstraints.Remove(schedulingconstraint);
            db.SaveChanges();
        }

        protected PartialViewResult PartialEditView(SchedulingConstraint schedulingconstraint = null)
        {
            ViewBag.GroupTypes = db.GroupTypes.ToList();
            ViewBag.Groups = db.Groups.ToList();
            ViewBag.Stations = db.Stations.ToList();
            return PartialView("Edit", schedulingconstraint);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
