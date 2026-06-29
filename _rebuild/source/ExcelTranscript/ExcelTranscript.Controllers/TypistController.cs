using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web.Mvc;
using ExcelTranscript.Models;
using ExcelTranscript.Models.Helper;
using Microsoft.AspNet.Identity;

namespace ExcelTranscript.Controllers
{
	public class TypistController : Controller
	{
		private db_ExcelTransEntities db = new db_ExcelTransEntities();

		public static int CurUserId { get; set; }

		private int GetCurrentUserId()
		{
			IIdentity identity = base.User.Identity;
			string currentUserId = base.User.Identity.GetUserId();
			AspNetUser aspNetUser = db.AspNetUsers.FirstOrDefault((AspNetUser x) => x.Id == currentUserId);
			CurUserId = aspNetUser.UserId;
			return aspNetUser.UserId;
		}

		public ActionResult Index()
		{
			GetCurrentUserId();
			List<Typist> list = new List<Typist>();
			foreach (stp_GetTypist_Result item in db.stp_GetTypist())
			{
				list.Add(new Typist
				{
					Id = item.UserId.GetValueOrDefault(),
					TypistName = item.UserName
				});
			}
			return View(list);
		}

		public ActionResult Create(string message)
		{
			if (!string.IsNullOrWhiteSpace(message))
			{
				base.ViewBag.message = message;
			}
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(RegisterViewModel model)
		{
			try
			{
				if (db.AspNetUsers.Any((AspNetUser u) => u.UserName == model.UserName))
				{
					base.ModelState.AddModelError("TypistName", "User Name already exist. Please enter any other username.");
					return View(model);
				}
				ExternalRegisterViewModel routeValues = new ExternalRegisterViewModel
				{
					UserName = model.UserName,
					Password = model.Password,
					ConfirmPassword = model.Password,
					UserRoles = "Typist",
					Email = model.UserName + ".typist@gmail.com"
				};
				return RedirectToAction("ExternalRegisterTypist", "Account", routeValues);
			}
			catch (Exception error)
			{
				LogHelper.PrintError(error);
				return View("Create");
			}
		}
	}
}
