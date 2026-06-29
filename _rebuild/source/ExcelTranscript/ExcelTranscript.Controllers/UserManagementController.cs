using System;
using System.Linq;
using System.Security.Cryptography;
using System.Web.Mvc;
using ExcelTranscript.Models;
using Microsoft.AspNet.Identity;

namespace ExcelTranscript.Controllers
{
	// Row shape for the user-management grid (mapped from raw SQL).
	public class UserMgmtRow
	{
		public string Id { get; set; }
		public int UserId { get; set; }
		public string UserName { get; set; }
		public string Name { get; set; }
		public string Email { get; set; }
		public string PlainPassword { get; set; }
		public bool IsActive { get; set; }
		public string Roles { get; set; }
	}

	// MoM follow-up: Admin user management — list users with their roles and
	// passwords, change a user's password, and delete a user.
	[Authorize(Roles = "Admin,SuperAdmin")]
	public class UserManagementController : Controller
	{
		private db_ExcelTransEntities db = new db_ExcelTransEntities();

		public ActionResult Index()
		{
			string sql = @"SELECT u.Id, u.UserId, u.UserName, u.Name, u.Email,
				ISNULL(u.PlainPassword,'') AS PlainPassword, u.IsActive,
				ISNULL(STUFF((SELECT ', ' + r.Name FROM AspNetUserRoles ur
					JOIN AspNetRoles r ON r.Id = ur.RoleId WHERE ur.UserId = u.Id
					FOR XML PATH('')), 1, 2, ''), '') AS Roles
				FROM AspNetUsers u ORDER BY u.UserId";
			var users = db.Database.SqlQuery<UserMgmtRow>(sql).ToList();
			return View(users);
		}

		[HttpPost]
		public ActionResult ChangePassword(string id, string newPassword)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(id))
				{
					TempData["UMErr"] = "Missing user.";
					return RedirectToAction("Index");
				}
				if (string.IsNullOrWhiteSpace(newPassword))
				{
					TempData["UMErr"] = "Password cannot be empty.";
					return RedirectToAction("Index");
				}
				string hash = HashPassword(newPassword);
				db.Database.ExecuteSqlCommand(
					"UPDATE AspNetUsers SET PasswordHash = {0}, PlainPassword = {1} WHERE Id = {2}",
					hash, newPassword, id);
				TempData["UMMsg"] = "Password updated successfully.";
			}
			catch (Exception ex)
			{
				TempData["UMErr"] = "Error changing password: " + ex.Message;
			}
			return RedirectToAction("Index");
		}

		[HttpPost]
		public ActionResult Delete(string id)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(id))
				{
					TempData["UMErr"] = "Missing user.";
					return RedirectToAction("Index");
				}
				string me = User.Identity.GetUserId();
				if (me == id)
				{
					TempData["UMErr"] = "You cannot delete the account you are logged in with.";
					return RedirectToAction("Index");
				}
				db.Database.ExecuteSqlCommand("DELETE FROM AspNetUserRoles WHERE UserId = {0}", id);
				db.Database.ExecuteSqlCommand("DELETE FROM AspNetUserClaims WHERE UserId = {0}", id);
				db.Database.ExecuteSqlCommand("DELETE FROM AspNetUserLogins WHERE UserId = {0}", id);
				db.Database.ExecuteSqlCommand("DELETE FROM AspNetUsers WHERE Id = {0}", id);
				TempData["UMMsg"] = "User deleted.";
			}
			catch (Exception ex)
			{
				TempData["UMErr"] = "Error deleting user: " + ex.Message;
			}
			return RedirectToAction("Index");
		}

		// ASP.NET Identity v2 password hash (PBKDF2 / HMACSHA1, 1000 iterations).
		private static string HashPassword(string password)
		{
			byte[] salt = new byte[16];
			using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
			{
				rng.GetBytes(salt);
			}
			byte[] subkey;
			using (Rfc2898DeriveBytes deriveBytes = new Rfc2898DeriveBytes(password, salt, 1000))
			{
				subkey = deriveBytes.GetBytes(32);
			}
			byte[] outputBytes = new byte[49];
			outputBytes[0] = 0;
			Buffer.BlockCopy(salt, 0, outputBytes, 1, 16);
			Buffer.BlockCopy(subkey, 0, outputBytes, 17, 32);
			return Convert.ToBase64String(outputBytes);
		}
	}
}
