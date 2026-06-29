using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ExcelTranscript.Models
{
	public class ApplicationUser : IdentityUser
	{
		public string Name { get; set; }

		public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
		{
			return await manager.CreateIdentityAsync(this, "ApplicationCookie");
		}
	}
}
