using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web.Mvc;
using ExcelTranscript.Models;
using ExcelTranscript.Models.BLL;
using ExcelTranscript.Models.Helper;
using Microsoft.AspNet.Identity;

namespace ExcelTranscript.Controllers
{
	public class FaxStatusController : Controller
	{
		private db_ExcelTransEntities db = new db_ExcelTransEntities();

		public static string CurUserName { get; set; }

		private string GetCurrentUserName()
		{
			IIdentity identity = base.User.Identity;
			string currentUserId = base.User.Identity.GetUserId();
			AspNetUser aspNetUser = db.AspNetUsers.FirstOrDefault((AspNetUser x) => x.Id == currentUserId);
			CurUserName = aspNetUser.UserName;
			return aspNetUser.UserName;
		}

		public ActionResult Index(bool? btn, string DictatorID, string ClinicID, int? TypistID, string StatusId, DateTime? fromDate, DateTime? toDate, DateTime? DotFromDate = null, DateTime? DotToDate = null, DateTime? DosFromDate = null, DateTime? DosToDate = null, DateTime? DobFromDate = null, DateTime? DobToDate = null)
		{
			GetCurrentUserName();
			if (base.User.IsInRole(UserRoles.Provider))
			{
				base.ViewBag.DictatorID = (from sa in db.Dictators
					where sa.LoginID == CurUserName
					select new
					{
						value = sa.DictatorID,
						text = sa.LoginID
					} into s
					orderby s.text
					select s).ToList();
				int Dictator = (from a in db.Dictators
					where a.LoginID == CurUserName
					select a into s
					select s.AccountID).FirstOrDefault();
				base.ViewBag.ClinicID = (from sa in db.Clinics
					where sa.ClinicID == Dictator
					select new
					{
						value = sa.ClinicID,
						text = sa.Name
					} into s
					orderby s.text
					select s).ToList();
				base.ViewBag.GetTypist = (from x in db.stp_GetTypist()
					select new
					{
						value = x.UserId,
						text = x.UserName
					}).ToList();
			}
			else if (base.User.IsInRole(UserRoles.Manager))
			{
				int managerID = db.Managers.Where((Manager m) => m.Username == CurUserName).FirstOrDefault().ManagerID;
				List<int> managerClinics = (from m in db.ManagerClinics
					where m.ManagerID == managerID
					select m into x
					select x.ClinicID).ToList();
				List<int> managerProviders = (from m in db.ManagerClinics
					where m.ManagerID == managerID
					select m into x
					select x.DictatorID).ToList();
				base.ViewBag.DictatorID = (from sa in db.Dictators
					where managerProviders.Contains(sa.DictatorID)
					select new
					{
						value = sa.DictatorID,
						text = sa.LoginID
					} into s
					orderby s.text
					select s).ToList();
				base.ViewBag.GetTypist = (from x in db.stp_GetTypist()
					select new
					{
						value = x.UserId,
						text = x.UserName
					}).ToList();
				base.ViewBag.ClinicID = (from sa in db.Clinics
					where managerClinics.Contains(sa.ClinicID)
					select new
					{
						value = sa.ClinicID,
						text = sa.Name
					} into s
					orderby s.text
					select s).ToList();
			}
			else if (!base.User.IsInRole(UserRoles.Typist))
			{
				base.ViewBag.DictatorID = (from sa in db.Dictators
					select new
					{
						value = sa.DictatorID,
						text = sa.LoginID
					} into s
					orderby s.text
					select s).ToList();
				base.ViewBag.ClinicID = (from sa in db.Clinics
					select new
					{
						value = sa.ClinicID,
						text = sa.Name
					} into s
					orderby s.text
					select s).ToList();
				base.ViewBag.GetTypist = (from x in db.stp_GetTypist()
					select new
					{
						value = x.UserId,
						text = x.UserName
					}).ToList();
			}
			else
			{
				base.ViewBag.DictatorID = (from sa in db.Dictators
					select new
					{
						value = sa.DictatorID,
						text = sa.LoginID
					} into s
					orderby s.text
					select s).ToList();
				base.ViewBag.ClinicID = (from sa in db.Clinics
					select new
					{
						value = sa.ClinicID,
						text = sa.Name
					} into s
					orderby s.text
					select s).ToList();
				base.ViewBag.GetTypist = (from t in db.stp_GetTypist()
					where t.UserName == CurUserName
					select t into x
					select new
					{
						value = x.UserId,
						text = x.UserName
					}).ToList();
			}
			if (btn.HasValue)
			{
				List<GetFaxStatusFilterWise_Result> list = db.GetFaxStatusFilterWiseEx(ClinicID, fromDate, toDate, DictatorID, TypistID, StatusId, DotFromDate, DotToDate, DosFromDate, DosToDate, DobFromDate, DobToDate).ToList();
				if (list.Count == 0)
				{
					return PartialView("_Index");
				}
				return PartialView("_Index", list);
			}
			return View();
		}

		public async Task<JsonResult> CheckStatus(string FaxID)
		{
			try
			{
				FaxBLL faxBLL = new FaxBLL();
				return Json(await Task.Run(() => faxBLL.CheckStatus(FaxID)), JsonRequestBehavior.AllowGet);
			}
			catch (Exception)
			{
				return Json("", JsonRequestBehavior.AllowGet);
			}
		}

		public async Task<JsonResult> RefreshFaxStatus(string selectedIds)
		{
			try
			{
				FaxBLL faxBLL = new FaxBLL();
				if (!string.IsNullOrWhiteSpace(selectedIds))
				{
					List<string> ids = selectedIds.Split(',').ToList();
					foreach (string faxItemID in ids)
					{
						await faxBLL.RefreshStatusAsync(faxItemID);
					}
				}
				else
				{
					foreach (FileFaxStatu faxItem in await db.FileFaxStatus.ToListAsync())
					{
						await faxBLL.RefreshStatusAsync(faxItem.ID.ToString());
					}
				}
				return Json("success", JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				Exception ex2 = ex;
				LogHelper.PrintError(ex2);
				return Json("error", JsonRequestBehavior.AllowGet);
			}
		}
	}
}
