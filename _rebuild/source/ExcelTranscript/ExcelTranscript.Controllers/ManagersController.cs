using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using ExcelTranscript.Models;
using ExcelTranscript.Models.DTO;
using ExcelTranscript.Models.Helper;
using Microsoft.Ajax.Utilities;

namespace ExcelTranscript.Controllers
{
	public class ManagersController : Controller
	{
		private readonly db_ExcelTransEntities db;

		public ManagersController()
		{
			db = new db_ExcelTransEntities();
		}

		public ActionResult Index(string message)
		{
			if (!string.IsNullOrWhiteSpace(message))
			{
				base.ViewBag.message = message;
			}
			List<GetManagers_Result> model = db.GetManagers(0).DistinctBy((GetManagers_Result mm) => mm.ManagerID).ToList();
			return View(model);
		}

		public async Task<ActionResult> Details(int? id)
		{
			if (!id.HasValue)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Manager manager = await db.Managers.FindAsync(id);
			if (manager == null)
			{
				return HttpNotFound();
			}
			return View(manager);
		}

		public ActionResult Create()
		{
			base.ViewBag.Clinics = (from sa in db.Clinics
				select new
				{
					id = sa.ClinicID,
					text = sa.Name
				} into s
				orderby s.text
				select s).ToList();
			return View();
		}

		[HttpPost]
		public async Task<ActionResult> Create(ManagerDTO manager)
		{
			base.ViewBag.Clinics = (from sa in db.Clinics
				select new
				{
					id = sa.ClinicID,
					text = sa.Name
				} into s
				orderby s.text
				select s).ToList();
			if (base.ModelState.IsValid)
			{
				using (DbContextTransaction transaction = db.Database.BeginTransaction())
				{
					try
					{
						if (await db.AspNetUsers.AnyAsync((AspNetUser m) => m.UserName == manager.Username))
						{
							transaction.Rollback();
							return Json(new
							{
								result = "Username already exists! Please enter unique username."
							}, JsonRequestBehavior.AllowGet);
						}
						Manager newManager = new Manager
						{
							Username = manager.Username,
							Password = manager.Password,
							isActive = manager.isActive,
							AddedOn = CustomTimeZone.Get_US_UTC_Time(),
							AddedBy = 4001
						};
						db.Managers.Add(newManager);
						await db.SaveChangesAsync();
						List<Dictator> dictators = await db.Dictators.ToListAsync();
						foreach (int dictatorID in manager.DictatorIds)
						{
							ManagerClinic managerClinic = new ManagerClinic
							{
								DictatorID = dictatorID,
								ManagerID = newManager.ManagerID
							};
							Dictator associatedDictator = dictators.Where((Dictator d) => d.DictatorID == dictatorID).FirstOrDefault();
							managerClinic.ClinicID = associatedDictator.AccountID;
							db.ManagerClinics.Add(managerClinic);
						}
						await db.SaveChangesAsync();
						ExternalRegisterViewModel data = new ExternalRegisterViewModel
						{
							UserName = manager.Username,
							Password = manager.Password,
							ConfirmPassword = manager.Password,
							UserRoles = "Manager",
							Email = manager.Username + "@exceltrans.com",
							RedirectToControllerName = "Managers"
						};
						transaction.Commit();
						return RedirectToAction("ExternalRegister", "Account", data);
					}
					catch (Exception)
					{
						transaction.Rollback();
						return Json(new
						{
							result = "Exception occured. Please try again later."
						}, JsonRequestBehavior.AllowGet);
					}
				}
			}
			return Json(new
			{
				result = "Invalid Data found. Please check your entries."
			}, JsonRequestBehavior.AllowGet);
		}

		public ActionResult Edit(int? id)
		{
			if (!id.HasValue)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			GetManagers_Result getManagers_Result = db.GetManagers(id).DistinctBy((GetManagers_Result m) => m.ManagerID).FirstOrDefault();
			if (getManagers_Result == null)
			{
				return HttpNotFound();
			}
			base.ViewBag.Clinics = (from sa in db.Clinics
				select new
				{
					id = sa.ClinicID,
					text = sa.Name
				} into s
				orderby s.text
				select s).ToList();
			base.ViewBag.Dictators = (from sa in db.Dictators
				select new
				{
					id = sa.DictatorID,
					text = sa.LoginID
				} into s
				orderby s.text
				select s).ToList();
			return View(getManagers_Result);
		}

		[HttpPost]
		public async Task<ActionResult> Edit(ManagerDTO managerDto)
		{
			base.ViewBag.Clinics = (from sa in db.Clinics
				select new
				{
					id = sa.ClinicID,
					text = sa.Name
				} into s
				orderby s.text
				select s).ToList();
			base.ViewBag.Dictators = (from sa in db.Dictators
				select new
				{
					id = sa.DictatorID,
					text = sa.LoginID
				} into s
				orderby s.text
				select s).ToList();
			if (base.ModelState.IsValid)
			{
				using (DbContextTransaction transaction = db.Database.BeginTransaction())
				{
					try
					{
						if (await db.AspNetUsers.CountAsync((AspNetUser m) => m.UserName == managerDto.Username) > 1)
						{
							transaction.Rollback();
							return Json(new
							{
								result = "Username already exists! Please enter unique username."
							}, JsonRequestBehavior.AllowGet);
						}
						bool changePassword = false;
						Manager manager = await db.Managers.Where((Manager m) => m.ManagerID == managerDto.ManagerID).FirstOrDefaultAsync();
						manager.isActive = managerDto.isActive;
						if (manager.Username != managerDto.Username && !string.IsNullOrWhiteSpace(managerDto.Username))
						{
							AspNetUser user = await db.AspNetUsers.Where((AspNetUser u) => u.UserName == manager.Username).FirstOrDefaultAsync();
							user.UserName = managerDto.Username;
							db.Entry(user).State = EntityState.Modified;
							await db.SaveChangesAsync();
							manager.Username = managerDto.Username;
						}
						if (manager.Password != managerDto.Password && !string.IsNullOrWhiteSpace(managerDto.Password))
						{
							manager.Password = managerDto.Password;
							changePassword = true;
						}
						db.Entry(manager).State = EntityState.Modified;
						await db.SaveChangesAsync();
						List<ManagerClinic> OldManagerClinic = await db.ManagerClinics.Where((ManagerClinic m) => m.ManagerID == manager.ManagerID).ToListAsync();
						db.ManagerClinics.RemoveRange(OldManagerClinic);
						await db.SaveChangesAsync();
						List<Dictator> dictators = await db.Dictators.ToListAsync();
						foreach (int dictatorID in managerDto.DictatorIds)
						{
							Dictator associatedDictator = dictators.Where((Dictator d) => d.DictatorID == dictatorID).FirstOrDefault();
							ManagerClinic managerClinic = new ManagerClinic
							{
								DictatorID = dictatorID,
								ManagerID = manager.ManagerID,
								ClinicID = associatedDictator.AccountID
							};
							db.ManagerClinics.Add(managerClinic);
						}
						await db.SaveChangesAsync();
						transaction.Commit();
						if (changePassword)
						{
							ResetUserPassword(managerDto);
						}
						return Json(new { }, JsonRequestBehavior.AllowGet);
					}
					catch (Exception)
					{
						transaction.Rollback();
						return Json(new
						{
							result = "Exception occured. Please try again later."
						}, JsonRequestBehavior.AllowGet);
					}
				}
			}
			return View(managerDto);
		}

		private ActionResult ResetUserPassword(ManagerDTO manager)
		{
			ResetPasswordViewModel routeValues = new ResetPasswordViewModel
			{
				Password = manager.Password,
				ConfirmPassword = manager.Password,
				Email = manager.Username + "@exceltrans.com",
				Code = "ResetManagerPassword"
			};
			return RedirectToAction("ResetUserPassword", "Account", routeValues);
		}

		public async Task<JsonResult> GetClinicsAndProviders(int managerId)
		{
			if (managerId <= 0)
			{
				return Json(new { }, JsonRequestBehavior.AllowGet);
			}
			List<object> returnedList = new List<object>();
			List<GetManagers_Result> managerData = db.GetManagers(managerId).ToList();
			List<Dictator> dictators = await db.Dictators.ToListAsync();
			List<Clinic> clinics = await db.Clinics.ToListAsync();
			HashSet<object> clinicIds = new HashSet<object>();
			HashSet<object> dictatorIds = new HashSet<object>();
			managerData.ForEach(delegate(GetManagers_Result mc)
			{
				var anon = (from c in clinics
					where c.ClinicID == mc.ClinicID
					select c into sa
					select new
					{
						id = sa.ClinicID,
						text = sa.Name
					}).FirstOrDefault();
				clinicIds.Add(anon.id);
			});
			managerData.ForEach(delegate(GetManagers_Result mc)
			{
				var anon2 = (from c in dictators
					where c.DictatorID == mc.DictatorID
					select c into sa
					select new
					{
						id = sa.DictatorID,
						text = sa.LoginID
					}).FirstOrDefault();
				dictatorIds.Add(anon2.id);
			});
			returnedList.Add(new { clinicIds, dictatorIds });
			return Json(returnedList, JsonRequestBehavior.AllowGet);
		}

		public async Task<JsonResult> GetProviders(List<int> selectedIds)
		{
			if (selectedIds == null)
			{
				return Json(new { }, JsonRequestBehavior.AllowGet);
			}
			List<object> returnedList = new List<object>();
			List<Dictator> dictators = await db.Dictators.ToListAsync();
			foreach (int Id in selectedIds)
			{
				var selectedProviders = (from d in dictators
					where d.AccountID == Id
					select d into sa
					select new
					{
						id = sa.DictatorID,
						text = sa.LoginID
					}).ToList();
				foreach (var dictator in selectedProviders)
				{
					if (dictator != null)
					{
						returnedList.Add(dictator);
					}
				}
			}
			return Json(returnedList, JsonRequestBehavior.AllowGet);
		}

		public async Task<ActionResult> Delete(int? id)
		{
			if (!id.HasValue)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Manager manager = await db.Managers.FindAsync(id);
			if (manager == null)
			{
				return HttpNotFound();
			}
			return View(manager);
		}

		[HttpPost]
		[ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> DeleteConfirmed(int id)
		{
			using (DbContextTransaction trans = db.Database.BeginTransaction())
			{
				try
				{
					Manager manager = await db.Managers.FindAsync(id);
					db.Managers.Remove(manager);
					List<ManagerClinic> managerRights = await db.ManagerClinics.Where((ManagerClinic mc) => mc.ManagerID == id).ToListAsync();
					db.ManagerClinics.RemoveRange(managerRights);
					AspNetUser user = await db.AspNetUsers.Where((AspNetUser u) => u.UserName == manager.Username).FirstOrDefaultAsync();
					AspNetUserRole userRole = await db.AspNetUserRoles.Where((AspNetUserRole u) => u.UserId == user.Id).FirstOrDefaultAsync();
					db.AspNetUserRoles.Remove(userRole);
					db.AspNetUsers.Remove(user);
					await db.SaveChangesAsync();
					trans.Commit();
					return RedirectToAction("Index", new
					{
						message = "Deleted"
					});
				}
				catch (Exception ex)
				{
					Exception ex2 = ex;
					trans.Rollback();
					return View("Delete", ex2);
				}
			}
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
