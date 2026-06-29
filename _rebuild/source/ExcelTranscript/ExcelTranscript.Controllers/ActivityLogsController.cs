using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web.Mvc;
using ExcelTranscript.Models;
using ExcelTranscript.Models.Helper;
using Microsoft.AspNet.Identity;
using Microsoft.CSharp.RuntimeBinder;

namespace ExcelTranscript.Controllers
{
	public class ActivityLogsController : Controller
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
			return aspNetUser.UserName;
		}

		public async Task<ActionResult> Index(bool? btn, string DictatorID, int? StatusId, DateTime? fromDate, DateTime? toDate)
		{
			if (btn.HasValue)
			{
				new List<stp_GetDocumentsDataNew_Result>();
				List<stp_GetDocumentsDataNew_Result> obj = (from sa in db.stp_GetDocumentsDataNew(DictatorID, null, null, StatusId, fromDate, toDate, null, null, null, null)
					orderby sa.StackMark descending, sa.DOT
					select sa).ToList();
				return PartialView("_IndexPartial", obj);
			}
			GetCurrentUser();
			if (base.User.IsInRole(UserRoles.Provider))
			{
				base.ViewBag.DictatorID = await (from x in db.Dictators
					where x.LoginID == CurUserName
					select x into sa
					select new
					{
						value = sa.DictatorID,
						text = sa.LoginID
					} into s
					orderby s.text
					select s).ToListAsync();
			}
			else if (base.User.IsInRole(UserRoles.Manager))
			{
				int managerID = db.Managers.Where((Manager m) => m.Username == CurUserName).FirstOrDefault().ManagerID;
				List<int> managerProviders = (from m in db.ManagerClinics
					where m.ManagerID == managerID
					select m into x
					select x.DictatorID).ToList();
				base.ViewBag.DictatorID = await (from sa in db.Dictators
					where managerProviders.Contains(sa.DictatorID)
					select new
					{
						value = sa.DictatorID,
						text = sa.LoginID
					} into s
					orderby s.text
					select s).ToListAsync();
			}
			else
			{
				base.ViewBag.DictatorID = await (from sa in db.Dictators
					select new
					{
						value = sa.DictatorID,
						text = sa.LoginID
					} into s
					orderby s.text
					select s).ToListAsync();
			}
			base.ViewBag.StatusId = from s in db.Status.Select((Status sa) => new
				{
					value = sa.Id,
					text = sa.Name
				}).ToList()
				orderby s.value == 4, s.value
				select s;
			return View();
		}

		[HttpGet]
		public JsonResult GetActivityLog(int? DocID, int? AudioID)
		{
			GetCurrentUser();
			List<stp_GetUserLog_Result> list = new List<stp_GetUserLog_Result>();
			list = db.stp_GetUserLog(AudioID).ToList();
			List<stp_GetUserLog_Result> source = list.Where((stp_GetUserLog_Result x) => x.FileType != "doc" && x.FileType != "docx").ToList();
			var list2 = (from a in source
				select new
				{
					UserName = a.UserLogName,
					UserRole = from r in UserList()
						where r.UserName == a.UserLogName
						select r into u
						select u.RoleName,
					Date = $"{a.Date:MM/dd/yyyy}",
					Time = $"{a.Date:hh:mm:ss tt}",
					Action = ((a.Action == 1) ? "Dictation Uploaded" : ((a.Action == 0) ? "Downloaded" : ((a.Action == 2) ? "Assigned" : ((a.Action == 7) ? "Faxed" : ((a.Action == 4) ? "Archived" : ((a.Action == 5) ? "Approved" : ((a.Action == 8) ? "Document Uploaded" : ""))))))),
					FileType = a.FileType
				} into sa
				orderby sa.Date
				select sa).ToList();
			List<stp_GetUserLogByDocumentId_Result> list3 = new List<stp_GetUserLogByDocumentId_Result>();
			list3 = db.stp_GetUserLogByDocumentId(DocID).ToList();
			var collection = (from a in list3
				select new
				{
					UserName = a.UserLogName,
					UserRole = from r in UserList()
						where r.UserName == a.UserLogName
						select r into u
						select u.RoleName,
					Date = $"{a.Date:MM/dd/yyyy}",
					Time = $"{a.Date:hh:mm:ss tt}",
					Action = ((a.Action == 1) ? "Dictation Uploaded" : ((a.Action == 0) ? "Downloaded" : ((a.Action == 2) ? "Assigned" : ((a.Action == 7) ? "Faxed" : ((a.Action == 4) ? "Archived" : ((a.Action == 5) ? "Approved" : ((a.Action == 8) ? "Document Uploaded" : ""))))))),
					FileType = a.FileType
				} into sa
				orderby sa.Date
				select sa).ToList();
			list2.AddRange(collection);
			return Json(list2, JsonRequestBehavior.AllowGet);
		}

		private List<UserWithRole> UserList()
		{
			return (from user in db.AspNetUsers
				from roles in user.AspNetUserRoles
				join role in db.AspNetRoles on roles.RoleId equals role.Id
				select new UserWithRole
				{
					UserName = user.UserName,
					RoleName = role.Name
				}).ToList();
		}
	}
}
