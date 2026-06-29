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
	public class ModalitiesController : Controller
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
			List<Modality> model = db.Modalities.ToList();
			return View(model);
		}

		public ActionResult Details(int? id)
		{
			if (!id.HasValue)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Modality modality = db.Modalities.Find(id);
			if (modality == null)
			{
				return HttpNotFound();
			}
			return View(modality);
		}

		public ActionResult Create()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(Modality modality)
		{
			try
			{
				GetCurrentUserId();
				modality.AddedBy = CurUserId;
				modality.AddedOn = DateTime.UtcNow.AddHours(5.0);
				if (base.ModelState.IsValid)
				{
					if (db.Modalities.Any((Modality c) => c.ModalityName == modality.ModalityName))
					{
						base.ModelState.AddModelError("", "Already Exists!");
						return View(modality);
					}
					db.Modalities.Add(modality);
					db.SaveChanges();
					return RedirectToAction("Index", new
					{
						message = "Added"
					});
				}
				return View(modality);
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
			Modality modality = db.Modalities.Find(id);
			if (modality == null)
			{
				return HttpNotFound();
			}
			return View(modality);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(Modality modality)
		{
			if (base.ModelState.IsValid)
			{
				Modality modality2 = db.Modalities.Where((Modality m) => m.Id == modality.Id).FirstOrDefault();
				if (modality2 != null)
				{
					if (db.Modalities.Any((Modality c) => c.ModalityName == modality.ModalityName))
					{
						base.ModelState.AddModelError("", "Modality Already Exists!");
						return View(modality);
					}
					modality2.UpdatedOn = DateTime.UtcNow.AddHours(5.0);
					modality2.UpdatedBy = CurUserId;
					modality2.ModalityName = modality.ModalityName;
					db.Entry(modality2).State = EntityState.Modified;
					db.SaveChanges();
					return RedirectToAction("Index", new
					{
						message = "Edited"
					});
				}
			}
			base.ModelState.AddModelError("", "Modality doesn't exists!");
			return View(modality);
		}

		public ActionResult Delete(int? id)
		{
			if (!id.HasValue)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Modality modality = db.Modalities.Find(id);
			if (modality == null)
			{
				return HttpNotFound();
			}
			return View(modality);
		}

		[HttpPost]
		[ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id)
		{
			Modality entity = db.Modalities.Find(id);
			db.Modalities.Remove(entity);
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
