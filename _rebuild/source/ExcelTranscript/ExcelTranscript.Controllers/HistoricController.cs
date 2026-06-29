using System;
using System.Linq;
using System.Web.Mvc;
using ExcelTranscript.Models;
using Microsoft.AspNet.Identity;

namespace ExcelTranscript.Controllers
{
	// MoM: one-time entry of old/historic data. Records are stored with
	// MRN = "Historic" so they can be identified and reported on later.
	[Authorize(Roles = "Admin,SuperAdmin")]
	public class HistoricController : Controller
	{
		private db_ExcelTransEntities db = new db_ExcelTransEntities();

		private int CurrentUserId()
		{
			string uid = User.Identity.GetUserId();
			AspNetUser u = db.AspNetUsers.FirstOrDefault((AspNetUser x) => x.Id == uid);
			return (u != null) ? u.UserId : 1;
		}

		public ActionResult Index()
		{
			ViewBag.DictatorID = (from d in db.Dictators
				where d.IsActive
				orderby d.LoginID
				select new { value = d.DictatorID, text = (d.FirstName + " " + d.LastName) }).ToList();
			ViewBag.StatusId = db.Status.OrderBy((Status s) => s.Id)
				.Select(s => new { value = s.Id, text = s.Name }).ToList();
			return View();
		}

		[HttpPost]
		public ActionResult Save(int DictatorID, string PatientName, string DocumentName, DateTime? DOS, DateTime? DOT, int? StatusId)
		{
			try
			{
				int uid = CurrentUserId();
				Dictator dict = db.Dictators.FirstOrDefault((Dictator d) => d.DictatorID == DictatorID);
				int? clinicId = (dict != null) ? (int?)dict.AccountID : null;
				DateTime when = DOT ?? DateTime.Now;
				string name = string.IsNullOrEmpty(DocumentName)
					? ("historic_" + Guid.NewGuid().ToString("N").Substring(0, 8))
					: DocumentName;

				UploadedFile file = new UploadedFile
				{
					FileName = name,
					Extension = "docx",
					DictatorID = DictatorID,
					ClinicID = clinicId,
					UploadBy = uid,
					UploadOn = when,
					IsActive = true,
					IsDownloaded = false,
					StatusId = StatusId ?? 5
				};
				db.UploadedFiles.Add(file);
				db.SaveChanges();

				UploadedDocument doc = new UploadedDocument
				{
					FileID = file.FileID,
					FileName = name,
					Extension = "docx",
					StatusId = StatusId ?? 5,
					IsApproved = true,
					UploadBy = uid,
					UploadOn = when,
					PatientName = PatientName,
					MRN = "Historic",
					DOS = DOS.HasValue ? DOS.Value.ToString("MM/dd/yyyy") : null
				};
				db.UploadedDocuments.Add(doc);
				db.SaveChanges();

				TempData["HistoricMsg"] = "Historic record saved successfully (MRN = Historic).";
			}
			catch (Exception ex)
			{
				TempData["HistoricErr"] = "Error saving historic record: " + ex.Message;
			}
			return RedirectToAction("Index");
		}
	}
}
