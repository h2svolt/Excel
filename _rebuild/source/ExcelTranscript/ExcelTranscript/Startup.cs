using System;
using System.Web.Mvc;
using ExcelTranscript.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;

namespace ExcelTranscript
{
	public class Startup
	{
		public void ConfigureAuth(IAppBuilder app)
		{
			app.CreatePerOwinContext(ApplicationDbContext.Create);
			app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
			app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);
			GlobalFilters.Filters.Add(new AuthorizeAttribute());
			app.UseCookieAuthentication(new CookieAuthenticationOptions
			{
				AuthenticationType = "ApplicationCookie",
				LoginPath = new PathString("/Account/Login"),
				Provider = new CookieAuthenticationProvider
				{
					OnValidateIdentity = SecurityStampValidator.OnValidateIdentity(TimeSpan.FromMinutes(30.0), (ApplicationUserManager manager, ApplicationUser user) => user.GenerateUserIdentityAsync(manager))
				}
			});
			app.UseExternalSignInCookie("ExternalCookie");
			app.UseTwoFactorSignInCookie("TwoFactorCookie", TimeSpan.FromMinutes(270.0));
			app.UseTwoFactorRememberBrowserCookie("TwoFactorRememberBrowser");
		}

		public void Configuration(IAppBuilder app)
		{
			ConfigureAuth(app);
		}
	}
}
