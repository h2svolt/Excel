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
	public class SpecialitiesController : Controller
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

		public ActionResult Index(string message)
		{
			if (!string.IsNullOrEmpty(message))
			{
				base.ViewBag.message = message;
			}
			GetCurrentUserId();
			List<Speciality> model = ((!base.User.IsInRole("Provider") && !base.User.IsInRole("Typist")) ? db.Specialities.ToList() : db.Specialities.Where((Speciality sa) => sa.AddedBy == CurUserId).ToList());
			return View(model);
		}

		public ActionResult Details(int? id)
		{
			if (!id.HasValue)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Speciality speciality = db.Specialities.Find(id);
			if (speciality == null)
			{
				return HttpNotFound();
			}
			return View(speciality);
		}

		public ActionResult Create()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(Speciality speciality)
		{
			try
			{
				GetCurrentUserId();
				speciality.AddedBy = CurUserId;
				speciality.AddedOn = CustomTimeZone.Get_US_UTC_Time();
				speciality.IsActive = true;
				if (base.ModelState.IsValid)
				{
					if (db.Specialities.Any((Speciality c) => c.Name == speciality.Name))
					{
						base.ModelState.AddModelError("", "Speciality name already exists!");
						return View("Create");
					}
					db.Specialities.Add(speciality);
					db.SaveChanges();
					return RedirectToAction("Index", new
					{
						message = "Added"
					});
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
			Speciality speciality = db.Specialities.Find(id);
			if (speciality == null)
			{
				return HttpNotFound();
			}
			return View(speciality);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(Speciality speciality)
		{
			if (base.ModelState.IsValid)
			{
				Speciality speciality2 = db.Specialities.Where((Speciality s) => s.SpecialityID == speciality.SpecialityID).FirstOrDefault();
				if (speciality2 != null)
				{
					if (db.Specialities.Any((Speciality c) => c.Name == speciality.Name))
					{
						base.ModelState.AddModelError("", "Speciality name already exists!");
						return View(speciality);
					}
					GetCurrentUserId();
					speciality2.UpdatedOn = CustomTimeZone.Get_US_UTC_Time();
					speciality2.UpdatedBy = CurUserId;
					speciality2.Name = speciality.Name;
					db.Entry(speciality2).State = EntityState.Modified;
					db.SaveChanges();
					return RedirectToAction("Index", new
					{
						message = "Edited"
					});
				}
			}
			base.ModelState.AddModelError("", "Speciality record doesn't exists!");
			return View(speciality);
		}

		public ActionResult Delete(int? id)
		{
			if (!id.HasValue)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Speciality speciality = db.Specialities.Find(id);
			if (speciality == null)
			{
				return HttpNotFound();
			}
			return View(speciality);
		}

		[HttpPost]
		[ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id)
		{
			Speciality entity = db.Specialities.Find(id);
			db.Specialities.Remove(entity);
			db.SaveChanges();
			return RedirectToAction("Index", new
			{
				message = "Deleted"
			});
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
