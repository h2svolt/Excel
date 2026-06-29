using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Web.Mvc;
using ExcelTranscript.Models;
using ExcelTranscript.Models.Helper;
using Microsoft.AspNet.Identity;

namespace ExcelTranscript.Controllers
{
	public class SecretariesController : Controller
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
			List<Secretary> model = ((!base.User.IsInRole("Provider") && !base.User.IsInRole("Typist")) ? db.Secretaries.ToList() : db.Secretaries.Where((Secretary sa) => sa.AddedBy == CurUserId).ToList());
			return View(model);
		}

		public ActionResult Details(int? id)
		{
			if (!id.HasValue)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Secretary secretary = db.Secretaries.Find(id);
			if (secretary == null)
			{
				return HttpNotFound();
			}
			return View(secretary);
		}

		public ActionResult Create()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(Secretary secretary)
		{
			try
			{
				GetCurrentUserId();
				secretary.AddedBy = CurUserId;
				secretary.AddedOn = CustomTimeZone.Get_US_UTC_Time();
				secretary.IsActive = true;
				if (base.ModelState.IsValid)
				{
					if (db.Specialities.Any((Speciality c) => c.Name == secretary.Name))
					{
						base.ModelState.AddModelError("Name", "Already Exists!");
						return View("Create");
					}
					db.Secretaries.Add(secretary);
					db.SaveChanges();
					return RedirectToAction("Index");
				}
				return View("Create");
			}
			catch (Exception error)
			{
				LogHelper.PrintError(error);
				return View("Create");
			}
		}

		public ActionResult Edit(int? id)
		{
			if (!id.HasValue)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Secretary secretary = db.Secretaries.Find(id);
			if (secretary == null)
			{
				return HttpNotFound();
			}
			return View(secretary);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "SecretaryID,Name,ShortCode,AddedOn,AddedBy,UpdatedOn,UpdatedBy,IsActive")] Secretary secretary)
		{
			if (base.ModelState.IsValid)
			{
				db.Entry(secretary).State = EntityState.Modified;
				db.SaveChanges();
				return RedirectToAction("Index");
			}
			return View(secretary);
		}

		public ActionResult Delete(int? id)
		{
			if (!id.HasValue)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Secretary secretary = db.Secretaries.Find(id);
			if (secretary == null)
			{
				return HttpNotFound();
			}
			return View(secretary);
		}

		[HttpPost]
		[ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id)
		{
			Secretary entity = db.Secretaries.Find(id);
			db.Secretaries.Remove(entity);
			db.SaveChanges();
			return RedirectToAction("Index");
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				db.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}
