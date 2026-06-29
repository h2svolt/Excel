using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Web.Mvc;
using ExcelTranscript.Models;
using ExcelTranscript.Models.Helper;
using Microsoft.AspNet.Identity;

namespace ExcelTranscript.Controllers
{
	public class DictatorsController : Controller
	{
		private db_ExcelTransEntities db = new db_ExcelTransEntities();

		public static int CurUserId { get; set; }

		public static string CurUserName { get; set; }

		private string GetCurrentUser()
		{
			IIdentity identity = base.User.Identity;
			string currentUserId = base.User.Identity.GetUserId();
			AspNetUser aspNetUser = db.AspNetUsers.FirstOrDefault((AspNetUser x) => x.Id == currentUserId);
			CurUserName = aspNetUser.UserName;
			CurUserId = aspNetUser.UserId;
			return CurUserName;
		}

		public ActionResult Index(string message)
		{
			if (!string.IsNullOrEmpty(message))
			{
				base.ViewBag.message = message;
			}
			base.ViewBag.AccountID = (from sa in db.Clinics
				where sa.AddedBy == CurUserId
				select sa into x
				select new
				{
					Value = x.ClinicID,
					Name = x.Name
				}).ToList();
			List<Dictator> model = ((!base.User.IsInRole("Provider") && !base.User.IsInRole("Typist")) ? db.Dictators.Where((Dictator s) => s.LoginID != "admin").ToList() : db.Dictators.Where((Dictator sa) => sa.AddedBy == CurUserId).ToList());
			return View(model);
		}

		public ActionResult Details(int? id)
		{
			if (!id.HasValue)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Dictator dictator = db.Dictators.Find(id);
			if (dictator == null)
			{
				return HttpNotFound();
			}
			return View(dictator);
		}

		public ActionResult Create()
		{
			if (!base.User.IsInRole("Provider") && !base.User.IsInRole("Typist"))
			{
				base.ViewBag.AccountID = db.Clinics.Select((Clinic x) => new
				{
					Value = x.ClinicID,
					Name = x.Name
				}).ToList();
				base.ViewBag.SpecialityID = db.Specialities.Select((Speciality x) => new
				{
					Value = x.SpecialityID,
					Name = x.Name
				}).ToList();
			}
			else
			{
				base.ViewBag.AccountID = (from sa in db.Clinics
					where sa.AddedBy == CurUserId
					select sa into x
					select new
					{
						Value = x.ClinicID,
						Name = x.Name
					}).ToList();
				base.ViewBag.SpecialityID = (from sa in db.Specialities
					where sa.AddedBy == CurUserId
					select sa into x
					select new
					{
						Value = x.SpecialityID,
						Name = x.Name
					}).ToList();
			}
			base.ViewBag.SystemID = Convert.ToString(GetSystemID());
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(Dictator dictator)
		{
			try
			{
				GetCurrentUser();
				dictator.AddedBy = CurUserId;
				dictator.AddedOn = CustomTimeZone.Get_US_UTC_Time();
				dictator.SystemID = Convert.ToString(GetSystemID());
				dictator.AccountID = dictator.AccountID;
				dictator.IsSecretary = true;
				if (base.ModelState.IsValid)
				{
					if (dictator.IsReview)
					{
						dictator.AddSign = true;
					}
					else
					{
						dictator.AddSign = false;
					}
					if (!base.User.IsInRole("Provider") && !base.User.IsInRole("Typist"))
					{
						base.ViewBag.AccountID = db.Clinics.Select((Clinic x) => new
						{
							Value = x.ClinicID,
							Name = x.Name
						}).ToList();
						base.ViewBag.SpecialityID = db.Specialities.Select((Speciality x) => new
						{
							Value = x.SpecialityID,
							Name = x.Name
						}).ToList();
					}
					else
					{
						base.ViewBag.AccountID = (from sa in db.Clinics
							where sa.AddedBy == CurUserId
							select sa into x
							select new
							{
								Value = x.ClinicID,
								Name = x.Name
							}).ToList();
						base.ViewBag.SpecialityID = (from sa in db.Specialities
							where sa.AddedBy == CurUserId
							select sa into x
							select new
							{
								Value = x.SpecialityID,
								Name = x.Name
							}).ToList();
					}
					AspNetUser aspNetUser = db.AspNetUsers.Where((AspNetUser u) => u.UserName == dictator.LoginID || u.Email == dictator.LoginID + "@gmail.com").FirstOrDefault();
					if (aspNetUser != null)
					{
						base.ModelState.AddModelError("LoginID", "UserName '" + dictator.LoginID + "' already exists. Please try again!");
						return View(dictator);
					}
					db.stp_Dictator(0, dictator.AccountID, dictator.SystemID, dictator.LoginID, dictator.Password, dictator.AllClientAccess, dictator.Prefix, dictator.FirstName, dictator.MiddleName, dictator.LastName, dictator.Gender, dictator.SpecialityID, dictator.IsSecretary, dictator.IsFax, dictator.IsReview, dictator.AddedOn, dictator.AddedBy, dictator.UpdatedOn, dictator.UpdatedBy, dictator.IsActive, dictator.CP_Name, dictator.CP_Phone, dictator.CP_Cell, dictator.CP_Address, dictator.CP_Email, dictator.CP_Pager, dictator.SecretaryName, dictator.NoOfLine, dictator.RateOfLine, dictator.AddSign);
					ExternalRegisterViewModel routeValues = new ExternalRegisterViewModel
					{
						UserName = dictator.LoginID,
						Password = dictator.Password,
						ConfirmPassword = dictator.Password,
						UserRoles = "Provider",
						Email = dictator.LoginID + "@exceltrans.com",
						RedirectToControllerName = "Dictators"
					};
					return RedirectToAction("ExternalRegister", "Account", routeValues);
				}
				return View(dictator);
			}
			catch (Exception error)
			{
				LogHelper.PrintError(error);
				if (!base.User.IsInRole("Provider") && !base.User.IsInRole("Typist"))
				{
					base.ViewBag.AccountID = db.Clinics.Select((Clinic x) => new
					{
						Value = x.ClinicID,
						Name = x.Name
					}).ToList();
					base.ViewBag.SpecialityID = db.Specialities.Select((Speciality x) => new
					{
						Value = x.SpecialityID,
						Name = x.Name
					}).ToList();
				}
				else
				{
					base.ViewBag.AccountID = (from sa in db.Clinics
						where sa.AddedBy == CurUserId
						select sa into x
						select new
						{
							Value = x.ClinicID,
							Name = x.Name
						}).ToList();
					base.ViewBag.SpecialityID = (from sa in db.Specialities
						where sa.AddedBy == CurUserId
						select sa into x
						select new
						{
							Value = x.SpecialityID,
							Name = x.Name
						}).ToList();
				}
				return View(dictator);
			}
		}

		public ActionResult Edit(int? id)
		{
			if (!id.HasValue)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Dictator dictator = db.Dictators.Find(id);
			if (dictator == null)
			{
				return HttpNotFound();
			}
			dictator.AddSign = dictator.IsReview;
			GetAccountIDandSpecialityIDViewBagData(dictator);
			return View(dictator);
		}

		private void GetAccountIDandSpecialityIDViewBagData(Dictator dictator)
		{
			if (base.User.IsInRole("Provider") || base.User.IsInRole("Typist"))
			{
				base.ViewBag.AccountID = new SelectList(db.Clinics, "ClinicID", "Name", dictator.AccountID);
				base.ViewBag.SpecialityID = new SelectList(db.Specialities, "SpecialityID", "Name", dictator.SpecialityID);
			}
			else
			{
				base.ViewBag.AccountID = new SelectList(db.Clinics, "ClinicID", "Name", dictator.AccountID);
				base.ViewBag.SpecialityID = new SelectList(db.Specialities, "SpecialityID", "Name", dictator.SpecialityID);
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(Dictator dictator)
		{
			GetAccountIDandSpecialityIDViewBagData(dictator);
			if (base.ModelState.IsValid)
			{
				GetCurrentUser();
				dictator.UpdatedBy = CurUserId;
				dictator.UpdatedOn = CustomTimeZone.Get_US_UTC_Time();
				dictator.IsSecretary = true;
				if (dictator.IsReview)
				{
					dictator.AddSign = true;
				}
				else
				{
					dictator.AddSign = false;
				}
				List<Dictator> list = db.Dictators.Where((Dictator d) => d.LoginID == dictator.LoginID && d.DictatorID != dictator.DictatorID).ToList();
				if (list.Count >= 1)
				{
					base.ModelState.AddModelError("LoginID", "Username '" + dictator.LoginID + "' already exists. Please try again!");
					return View(dictator);
				}
				Dictator dictator2 = db.Dictators.Where((Dictator d) => d.LoginID == dictator.LoginID && d.DictatorID == dictator.DictatorID).FirstOrDefault();
				if (dictator2 != null)
				{
					if (dictator2.Password != dictator.Password && dictator.Password != null)
					{
						ResetUserPassword(dictator);
					}
					else
					{
						dictator.Password = dictator2.Password;
					}
				}
				db.stp_Dictator(dictator.DictatorID, dictator.AccountID, dictator.SystemID, dictator.LoginID, dictator.Password, dictator.AllClientAccess, dictator.Prefix, dictator.FirstName, dictator.MiddleName, dictator.LastName, dictator.Gender, dictator.SpecialityID, dictator.IsSecretary, dictator.IsFax, dictator.IsReview, dictator.AddedOn, dictator.AddedBy, dictator.UpdatedOn, dictator.UpdatedBy, dictator.IsActive, dictator.CP_Name, dictator.CP_Phone, dictator.CP_Cell, dictator.CP_Address, dictator.CP_Email, dictator.CP_Pager, dictator.SecretaryName, dictator.NoOfLine, dictator.RateOfLine, dictator.AddSign);
				AspNetUser aspNetUser = db.AspNetUsers.Where((AspNetUser u) => u.UserName == dictator.LoginID && u.Email == dictator.LoginID + "@gmail.com").FirstOrDefault();
				if (aspNetUser == null)
				{
					RegisterUser(dictator);
				}
				return RedirectToAction("Index", new
				{
					message = "Edited"
				});
			}
			return View(dictator);
		}

		private ActionResult ResetUserPassword(Dictator dictator)
		{
			ResetPasswordViewModel routeValues = new ResetPasswordViewModel
			{
				Password = dictator.Password,
				ConfirmPassword = dictator.Password,
				Email = dictator.LoginID + "@excelonwork.com",
				Code = "ResetDictatorPassword"
			};
			return RedirectToAction("ResetUserPassword", "Account", routeValues);
		}

		private ActionResult RegisterUser(Dictator dictator)
		{
			ExternalRegisterViewModel routeValues = new ExternalRegisterViewModel
			{
				UserName = dictator.LoginID,
				Password = dictator.Password,
				ConfirmPassword = dictator.Password,
				UserRoles = "Provider",
				Email = dictator.LoginID + "@excelonwork.com"
			};
			return RedirectToAction("ExternalRegister", "Account", routeValues);
		}

		public ActionResult Delete(int? id)
		{
			if (!id.HasValue)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Dictator dictator = db.Dictators.Find(id);
			if (dictator == null)
			{
				return HttpNotFound();
			}
			return View(dictator);
		}

		[HttpPost]
		[ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id)
		{
			Secretary secretary = db.Secretaries.Where((Secretary sa) => sa.DictatorID == (int?)id).FirstOrDefault();
			if (secretary != null)
			{
				db.Secretaries.Remove(secretary);
			}
			Dictator entity = db.Dictators.Find(id);
			db.Dictators.Remove(entity);
			db.SaveChanges();
			return RedirectToAction("Index");
		}

		public int GetSystemID()
		{
			int num = 0;
			Dictator dictator = db.Dictators.OrderByDescending((Dictator s) => s.SystemID).FirstOrDefault();
			if (dictator != null)
			{
				return int.Parse(dictator.SystemID) + 1;
			}
			return 101001;
		}

		[HttpGet]
		public JsonResult GetDictatorsDetails(int? DictatorID)
		{
			var data = (from sa in db.Dictators
				where (int?)sa.DictatorID == DictatorID
				select sa into a
				select new
				{
					DictatorID = a.DictatorID,
					Name = a.CP_Name,
					Phone = a.CP_Phone,
					Cell = a.CP_Cell,
					Address = a.CP_Address,
					Email = a.CP_Email,
					Pager = a.CP_Pager
				}).ToList();
			return Json(data, JsonRequestBehavior.AllowGet);
		}

		public JsonResult GetProviders(int ClinicID)
		{
			if (base.User.IsInRole(UserRoles.Manager))
			{
				GetCurrentUser();
				List<int> managerProviders = new List<int>();
				int managerID = db.Managers.Where((Manager m) => m.Username == CurUserName).FirstOrDefault().ManagerID;
				managerProviders = (from m in db.ManagerClinics
					where m.ManagerID == managerID
					select m into x
					select x.DictatorID).ToList();
				if (ClinicID > 0)
				{
					var data = (from sa in db.Dictators
						where managerProviders.Contains(sa.DictatorID) && sa.AccountID == ClinicID
						select sa into s
						select new
						{
							value = s.DictatorID,
							text = s.LoginID
						}).ToList();
					return Json(data);
				}
				var data2 = (from sa in db.Dictators
					where managerProviders.Contains(sa.DictatorID)
					select sa into s
					select new
					{
						value = s.DictatorID,
						text = s.LoginID
					}).ToList();
				return Json(data2);
			}
			if (ClinicID > 0)
			{
				var data3 = (from sa in db.Dictators
					where sa.AccountID == ClinicID
					select sa into s
					select new
					{
						value = s.DictatorID,
						text = s.LoginID
					}).ToList();
				return Json(data3);
			}
			var data4 = db.Dictators.Select((Dictator s) => new
			{
				value = s.DictatorID,
				text = s.LoginID
			}).ToList();
			return Json(data4);
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
