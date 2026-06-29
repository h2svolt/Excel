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
	public class ClinicsController : Controller
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
			List<Clinic> model = ((!base.User.IsInRole("Provider") && !base.User.IsInRole("Typist")) ? db.Clinics.ToList() : db.Clinics.Where((Clinic sa) => sa.AddedBy == CurUserId).ToList());
			return View(model);
		}

		public ActionResult Details(int? id)
		{
			if (!id.HasValue)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Clinic clinic = db.Clinics.Find(id);
			if (clinic == null)
			{
				return HttpNotFound();
			}
			return View(clinic);
		}

		public ActionResult Create()
		{
			base.ViewBag.SystemID = Convert.ToString(GetSystemID());
			return View();
		}

		[HttpPost]
		public ActionResult Create(Clinic clinic)
		{
			try
			{
				clinic.AddedBy = CurUserId;
				clinic.AddedOn = CustomTimeZone.Get_US_UTC_Time();
				clinic.IsActive = true;
				clinic.SystemID = Convert.ToString(GetSystemID());
				if (base.ModelState.IsValid)
				{
					if (db.Clinics.Any((Clinic c) => c.AccountID == clinic.AccountID))
					{
						base.ModelState.AddModelError("AccountID", "Account Already Exists!");
						return View(clinic);
					}
					db.Clinics.Add(clinic);
					db.SaveChanges();
					return RedirectToAction("Index", new
					{
						message = "Added"
					});
				}
				return View(clinic);
			}
			catch (Exception error)
			{
				LogHelper.PrintError(error);
				base.ModelState.AddModelError("", "An exception occured.");
				return View(clinic);
			}
		}

		public ActionResult Edit(int? id)
		{
			if (!id.HasValue)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Clinic clinic = db.Clinics.Find(id);
			if (clinic == null)
			{
				return HttpNotFound();
			}
			return View(clinic);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(Clinic clinic)
		{
			if (base.ModelState.IsValid)
			{
				if (db.Clinics.Where((Clinic sa) => sa.ClinicID != clinic.ClinicID && sa.Name == clinic.Name).Count() > 1)
				{
					base.ModelState.AddModelError("Clinic", "can't be duplicate, Clinic Name already exists");
					return View("Edit");
				}
				GetCurrentUserId();
				clinic.UpdatedOn = CustomTimeZone.Get_US_UTC_Time();
				clinic.UpdatedBy = CurUserId;
				db.Entry(clinic).State = EntityState.Modified;
				db.SaveChanges();
				return RedirectToAction("Index", new
				{
					message = "Edited"
				});
			}
			return View(clinic);
		}

		public ActionResult Delete(int? id)
		{
			if (!id.HasValue)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Clinic clinic = db.Clinics.Find(id);
			if (clinic == null)
			{
				return HttpNotFound();
			}
			return View(clinic);
		}

		[HttpPost]
		[ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id)
		{
			if (db.Dictators.Any((Dictator sa) => sa.AccountID == id))
			{
				base.ModelState.AddModelError("Clinic", "can not delete, this record already used against this Dictator (" + db.Dictators.Where((Dictator sa) => sa.AccountID == id).FirstOrDefault().FirstName.ToString() + ")");
				return View("Delete");
			}
			Clinic entity = db.Clinics.Find(id);
			db.Clinics.Remove(entity);
			db.SaveChanges();
			return RedirectToAction("Index", new
			{
				message = "Deleted"
			});
		}

		public int GetSystemID()
		{
			int num = 0;
			Clinic clinic = (from sa in db.Clinics
				where sa.SystemID != null
				select sa into s
				orderby s.SystemID descending
				select s).FirstOrDefault();
			if (clinic != null)
			{
				return int.Parse(clinic.SystemID) + 1;
			}
			return 101001;
		}

		[HttpGet]
		public JsonResult GetClinicDetails(int? ClinicID)
		{
			var data = (from sa in db.Clinics
				where (int?)sa.ClinicID == ClinicID
				select sa into a
				select new
				{
					ClinicID = a.ClinicID,
					Name = a.CP_Name,
					Phone = a.CP_Phone,
					Cell = a.CP_Cell,
					Address = a.CP_Address,
					Email = a.CP_Email,
					Pager = a.CP_Pager
				}).ToList();
			return Json(data, JsonRequestBehavior.AllowGet);
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
