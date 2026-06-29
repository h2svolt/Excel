using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web.Mvc;
using ExcelTranscript.Models;
using InterFAX.Api;
using Microsoft.AspNet.Identity;

namespace ExcelTranscript.Controllers
{
	public class WebAPIController : Controller
	{
		private db_ExcelTransEntities db = new db_ExcelTransEntities();

		public int CurUserId { get; set; }

		public string CurUserName { get; set; }

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

		public ActionResult Index()
		{
			return View();
		}

		public async Task FaxAsync()
		{
			FaxClient interfax = new FaxClient("username", "password");
			IFaxDocument file = interfax.Documents.BuildFaxDocument("D:/Plesk.docx");
			SendOptions options = new SendOptions
			{
				FaxNumber = "+11111111112"
			};
			long faxId = await interfax.Outbound.SendFax(file, options);
			interfax.Outbound.GetFaxRecord(faxId);
		}
	}
}
