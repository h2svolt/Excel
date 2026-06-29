using System;
using ExcelTranscript.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.DataProtection;

namespace ExcelTranscript
{
	public class ApplicationUserManager : UserManager<ApplicationUser>
	{
		public ApplicationUserManager(IUserStore<ApplicationUser> store)
			: base(store)
		{
		}

		public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
		{
			ApplicationUserManager applicationUserManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
			applicationUserManager.UserValidator = new UserValidator<ApplicationUser>(applicationUserManager)
			{
				AllowOnlyAlphanumericUserNames = false,
				RequireUniqueEmail = false
			};
			applicationUserManager.PasswordValidator = new PasswordValidator();
			applicationUserManager.UserLockoutEnabledByDefault = true;
			applicationUserManager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5.0);
			applicationUserManager.MaxFailedAccessAttemptsBeforeLockout = 5;
			applicationUserManager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<ApplicationUser>
			{
				MessageFormat = "Your security code is {0}"
			});
			applicationUserManager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<ApplicationUser>
			{
				Subject = "Security Code",
				BodyFormat = "Your security code is {0}"
			});
			applicationUserManager.EmailService = new EmailService();
			applicationUserManager.SmsService = new SmsService();
			IDataProtectionProvider dataProtectionProvider = options.DataProtectionProvider;
			if (dataProtectionProvider != null)
			{
				applicationUserManager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
			}
			return applicationUserManager;
		}
	}
}
