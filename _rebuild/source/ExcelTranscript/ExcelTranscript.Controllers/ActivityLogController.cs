using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web.Mvc;
using ExcelTranscript.Models;
using ExcelTranscript.Models.Helper;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.CSharp.RuntimeBinder;

namespace ExcelTranscript.Controllers
{
	public class ActivityLogController : Controller
	{
		private db_ExcelTransEntities db = new db_ExcelTransEntities();

		public static int CurUserId { get; set; }

		public static string CurUserName { get; set; }

		private int GetCurrentUserId()
		{
			IIdentity identity = base.User.Identity;
			string currentUserId = base.User.Identity.GetUserId();
			AspNetUser aspNetUser = db.AspNetUsers.FirstOrDefault((AspNetUser x) => x.Id == currentUserId);
			CurUserId = aspNetUser.UserId;
			return aspNetUser.UserId;
		}

		private string GetCurrentUserName()
		{
			IIdentity identity = base.User.Identity;
			string currentUserId = base.User.Identity.GetUserId();
			AspNetUser aspNetUser = db.AspNetUsers.FirstOrDefault((AspNetUser x) => x.Id == currentUserId);
			CurUserName = aspNetUser.UserName;
			return aspNetUser.UserName;
		}

		public async Task<ActionResult> Index(bool? btn, int? DictatorID, int? StatusId, DateTime? fromDate, DateTime? toDate)
		{
			if (btn.HasValue)
			{
				new List<stp_GetActivityLogs_Result>();
				List<stp_GetActivityLogs_Result> obj = db.stp_GetActivityLogs(DictatorID, StatusId, fromDate, toDate).DistinctBy((stp_GetActivityLogs_Result d) => d.AudioID).ToList();
				return PartialView("_IndexPartial", obj);
			}
			GetCurrentUserName();
			if (base.User.IsInRole("Provider"))
			{
				base.ViewBag.DictatorID = await Task.Run(() => (from x in db.Dictators
					where x.LoginID == CurUserName
					select x into sa
					select new
					{
						value = sa.DictatorID,
						text = sa.LoginID
					} into s
					orderby s.text
					select s).ToList());
			}
			else
			{
				base.ViewBag.DictatorID = await Task.Run(() => (from sa in db.Dictators
					select new
					{
						value = sa.DictatorID,
						text = sa.LoginID
					} into s
					orderby s.text
					select s).ToList());
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
		public JsonResult GetActivityLog(int? AudioID)
		{
			GetCurrentUserId();
			GetCurrentUserName();
			List<stp_GetUserLog_Result> list = new List<stp_GetUserLog_Result>();
			list = db.stp_GetUserLog(AudioID).ToList();
			var data = (from a in list
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
			return Json(data, JsonRequestBehavior.AllowGet);
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
