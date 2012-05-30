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
using System.Web.Security;

namespace MvcApplication1.Controllers
{

	[Authorize]
	public class AccountController : Controller
	{

		//
		// GET: /Account/LogOn

		[AllowAnonymous]
		public ActionResult LogOn()
		{
			return ContextDependentView();
		}

		//
		// POST: /Account/JsonLogOn
		/*
		[AllowAnonymous]
		[HttpPost]
		public JsonResult JsonLogOn(string x, string y, string returnUrl)
		{
			if (ModelState.IsValid)
			{
				if (Membership.ValidateUser(model.UserName, model.Password))
				{
					FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
					return Json(new { success = true, redirect = returnUrl });
				}
				else
				{
					ModelState.AddModelError("", "The user name or password provided is incorrect.");
				}
			}

			// If we got this far, something failed
			return Json(new { errors = GetErrorsFromModelState() });
		}

		//
		// POST: /Account/LogOn
		*/
		[AllowAnonymous]
		[HttpPost]
		public ActionResult LogOn(string x)
		{
			string user = Request.Params["username"];
			string pass = Request.Params["password"];

			string hardCodedUser = "admin";
			string hardCodedPass = "Camp@Lazerus";

			if (user == hardCodedUser && pass == hardCodedPass)
			{
				FormsAuthentication.SetAuthCookie(user, false);
				return RedirectToAction("Welcome", "Home");
			}
			else
			{
				return Redirect("./");
			}
			
			/*
			if (ModelState.IsValid)
			{
				if (Membership.ValidateUser(model.UserName, model.Password))
				{
					FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
					if (Url.IsLocalUrl(returnUrl))
					{
						return Redirect(returnUrl);
					}
					else
					{
						return RedirectToAction("Index", "Home");
					}
				}
				else
				{
					ModelState.AddModelError("", "The user name or password provided is incorrect.");
				}
			}
			*/
			// If we got this far, something failed, redisplay form
			return View();
		}
		/*
		//
		// GET: /Account/LogOff

		public ActionResult LogOff()
		{
			FormsAuthentication.SignOut();

			return RedirectToAction("Index", "Home");
		}

		private ActionResult ContextDependentView()
		{
			string actionName = ControllerContext.RouteData.GetRequiredString("action");
			if (Request.QueryString["content"] != null)
			{
				ViewBag.FormAction = "Json" + actionName;
				return PartialView();
			}
			else
			{
				ViewBag.FormAction = actionName;
				return View();
			}
		}
	*/
		private ActionResult ContextDependentView()
		{
			string actionName = ControllerContext.RouteData.GetRequiredString("action");
			if (Request.QueryString["content"] != null)
			{
				ViewBag.FormAction = "Json" + actionName;
				return PartialView();
			}
			else
			{
				ViewBag.FormAction = actionName;
				return View();
			}
		}
	}
}
