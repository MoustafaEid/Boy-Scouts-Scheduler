﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Data.Entity;
using Boy_Scouts_Scheduler.Models;
using System.Web.Security;

namespace Boy_Scouts_Scheduler
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        protected void Application_Start()
        {
#if DEBUG
            Database.SetInitializer(new DevInitializer());
#else
            Database.SetInitializer(new ReleaseInitializer());
#endif
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
        }

        protected void Application_BeginRequest(object sender, EventArgs args)
		{
				HttpCookie cookie = HttpContext.Current.Request.Cookies["Event"];
				if ((cookie == null || string.IsNullOrEmpty(cookie.Value)) &&
					(Request.Path.StartsWith("/Group") || Request.Path.StartsWith("/Schedule")
					|| Request.Path.StartsWith("/SchedulingConstraint") || Request.Path.StartsWith("/Station")
					|| Request.Path.StartsWith("/TimeSlot")))
				{
					if (HttpContext.Current.Request.Cookies[".ASPXAUTH"] != null)
					{
						
						HttpContext.Current.Response.Redirect("/home/welcome");
					}
					else
					{
						HttpContext.Current.Response.Redirect("/Account/LogOn");
					}
				}
		}
    }
}
