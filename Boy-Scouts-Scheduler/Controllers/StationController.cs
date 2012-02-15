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
    public class StationController : Controller
    {
        private SchedulingContext db = new SchedulingContext();

        //
        // GET: /NewStation/

        public ViewResult Index(int start = 0, int itemsPerPage = 20, string orderBy = "ID", bool desc = false)
        {
            ViewBag.Count = db.Stations.Count();
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
            Response.AppendHeader("X-Total-Row-Count", db.Stations.Count().ToString());
            ObjectQuery<Station> stations = (db as IObjectContextAdapter).ObjectContext.CreateObjectSet<Station>();
            stations = stations.OrderBy("it." + orderBy + (desc ? " desc" : ""));

            return PartialView(stations.Skip(start).Take(itemsPerPage));
        }

        //
        // GET: /NewStation/Details/5

        public ViewResult Details(int id)
        {
            Station station = db.Stations.Find(id);
            return View(station);
        }

        //
        // GET: /NewStation/Create

        public ActionResult Create()
        {
            //ViewBag.TimeSlots = db.TimeSlots.ToList();
            return View();
        } 

        //
        // POST: /NewStation/Create

        [HttpPost]
        public ActionResult Create(Station station)
        {
            if (ModelState.IsValid)
            {
                db.Stations.Add(station);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            return View(station);
        }
        
        //
        // GET: /NewStation/Edit/5
 
        public ActionResult Edit(int id)
        {
            Station station = db.Stations.Find(id);
            return View(station);
        }

        //
        // POST: /NewStation/Edit/5

        [HttpPost]
        public ActionResult Edit(Station station)
        {
            if (ModelState.IsValid)
            {
                db.Entry(station).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(station);
        }

        //
        // GET: /NewStation/Delete/5
 
        public ActionResult Delete(int id)
        {
            Station station = db.Stations.Find(id);
            return View(station);
        }

        //
        // POST: /NewStation/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            Station station = db.Stations.Find(id);
            db.Stations.Remove(station);
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