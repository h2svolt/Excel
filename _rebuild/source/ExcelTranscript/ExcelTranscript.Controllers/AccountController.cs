using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ExcelTranscript.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace ExcelTranscript.Controllers
{
	[Authorize]
	public class AccountController : Controller
	{
		internal class ChallengeResult : HttpUnauthorizedResult
		{
			public string LoginProvider { get; set; }

			public string RedirectUri { get; set; }

			public string UserId { get; set; }

			public ChallengeResult(string provider, string redirectUri)
				: this(provider, redirectUri, null)
			{
			}

			public ChallengeResult(string provider, string redirectUri, string userId)
			{
				LoginProvider = provider;
				RedirectUri = redirectUri;
				UserId = userId;
			}

			public override void ExecuteResult(ControllerContext context)
			{
				AuthenticationProperties authenticationProperties = new AuthenticationProperties
				{
					RedirectUri = RedirectUri
				};
				if (UserId != null)
				{
					authenticationProperties.Dictionary["XsrfId"] = UserId;
				}
				context.HttpContext.GetOwinContext().Authentication.Challenge(authenticationProperties, LoginProvider);
			}
		}

		private ApplicationSignInManager _signInManager;

		private ApplicationUserManager _userManager;

		private ApplicationDbContext context;

		private const string XsrfKey = "XsrfId";

		public string UserName { get; set; }

		public string Password { get; set; }

		public string ConfirmPassword { get; set; }

		public ApplicationSignInManager SignInManager
		{
			get
			{
				return _signInManager ?? base.HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
			}
			private set
			{
				_signInManager = value;
			}
		}

		public ApplicationUserManager UserManager
		{
			get
			{
				return _userManager ?? base.HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
			}
			private set
			{
				_userManager = value;
			}
		}

		private IAuthenticationManager AuthenticationManager => base.HttpContext.GetOwinContext().Authentication;

		public AccountController()
		{
			context = new ApplicationDbContext();
		}

		public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
		{
			UserManager = userManager;
			SignInManager = signInManager;
		}

		[AllowAnonymous]
		public ActionResult Login(string returnUrl)
		{
			base.Session["UserName"] = null;
			base.Session["Password"] = null;
			base.ViewBag.ReturnUrl = returnUrl;
			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
		{
			if (!base.ModelState.IsValid)
			{
				return View(model);
			}
			db_ExcelTransEntities db = new db_ExcelTransEntities();
			if (db.Dictators.Where((Dictator u) => u.LoginID == model.UserName && u.Password == model.Password).FirstOrDefault()?.IsActive ?? false)
			{
				base.ModelState.AddModelError("", "Login failed, Please contact system administrator.");
				return View(model);
			}
			base.Session["UserName"] = model.UserName;
			base.Session["Password"] = model.Password;
			SignInStatus result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: false);
			if (result == SignInStatus.Failure)
			{
				base.ModelState.AddModelError("", "Login failed, Please enter correct username and password!");
			}
			switch (result)
			{
			case SignInStatus.Success:
				return RedirectToLocal(returnUrl);
			case SignInStatus.LockedOut:
				return View("Lockout");
			case SignInStatus.RequiresVerification:
				return RedirectToAction("SendCode", new
				{
					ReturnUrl = returnUrl,
					RememberMe = model.RememberMe
				});
			case SignInStatus.Failure:
				return View(model);
			default:
				base.ModelState.AddModelError("", "Login failed, Something went Wrong.");
				return View(model);
			}
		}

		[AllowAnonymous]
		public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
		{
			if (!(await SignInManager.HasBeenVerifiedAsync()))
			{
				return View("Error");
			}
			return View(new VerifyCodeViewModel
			{
				Provider = provider,
				ReturnUrl = returnUrl,
				RememberMe = rememberMe
			});
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
		{
			if (!base.ModelState.IsValid)
			{
				return View(model);
			}
			switch (await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, model.RememberMe, model.RememberBrowser))
			{
			case SignInStatus.Success:
				return RedirectToLocal(model.ReturnUrl);
			case SignInStatus.LockedOut:
				return View("Lockout");
			default:
				base.ModelState.AddModelError("", "Invalid code.");
				return View(model);
			}
		}

		[Authorize]
		public ActionResult Register()
		{
			db_ExcelTransEntities db_ExcelTransEntities = new db_ExcelTransEntities();
			if (!(UserRole() == "SuperAdmin") && !(UserRole() == "Admin"))
			{
				return RedirectToAction("Index", "Home");
			}
			base.ViewBag.Name = new SelectList(context.Roles.Where((IdentityRole u) => !u.Name.Contains("SuperAdmin") && !u.Name.Contains("Provider")).ToList(), "Name", "Name");
			return View();
		}

		public string UserRole()
		{
			if (base.User.Identity.IsAuthenticated)
			{
				IIdentity identity = base.User.Identity;
				base.ViewBag.Name = identity.Name;
				string text = "";
				context = new ApplicationDbContext();
				if (base.User.Identity.IsAuthenticated)
				{
					UserManager<ApplicationUser> manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
					IList<string> roles = manager.GetRoles(identity.GetUserId());
					text = roles[0].ToString();
				}
				base.ViewBag.Role = text;
				return text;
			}
			base.ViewBag.Name = "Not Logged IN";
			return string.Empty;
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Register(RegisterViewModel model)
		{
			base.ViewBag.Name = new SelectList(context.Roles.Where((IdentityRole u) => !u.Name.Contains("SuperAdmin") && !u.Name.Contains("Provider")).ToList(), "Name", "Name");
			if (base.ModelState.IsValid)
			{
				new db_ExcelTransEntities();
				if (model.UserRoles == "Account")
				{
					model.UserRoles = "Admin";
				}
				if (!(UserRole() == "SuperAdmin") && !(UserRole() == "Admin"))
				{
					return RedirectToAction("Login", "Account");
				}
				ApplicationUser user = new ApplicationUser
				{
					UserName = model.UserName,
					Email = model.Email,
					Name = model.Name
				};
				IdentityResult result = await UserManager.CreateAsync(user, model.Password);
				if (result.Succeeded)
				{
					await UserManager.AddToRoleAsync(user.Id, model.UserRoles);
					return RedirectToAction("Index", "Home");
				}
				AddErrors(result);
			}
			return View(model);
		}

		public async Task<object> ExternalRegister(ExternalRegisterViewModel model)
		{
			if (base.ModelState.IsValid)
			{
				ApplicationUser user = new ApplicationUser
				{
					UserName = model.UserName,
					Email = model.Email
				};
				IdentityResult result = await UserManager.CreateAsync(user, model.Password);
				if (result.Succeeded)
				{
					await UserManager.AddToRoleAsync(user.Id, model.UserRoles);
					return RedirectToAction("Index", model.RedirectToControllerName, new
					{
						message = "Added"
					});
				}
				AddErrors(result);
			}
			return RedirectToAction("Login", model);
		}

		public async Task<object> ExternalRegisterTypist(ExternalRegisterViewModel model)
		{
			ApplicationUser user = new ApplicationUser
			{
				UserName = model.UserName,
				Email = model.Email
			};
			if ((await UserManager.CreateAsync(user, model.Password)).Succeeded)
			{
				await UserManager.AddToRoleAsync(user.Id, model.UserRoles);
				return RedirectToAction("Create", "Typist", new
				{
					message = "Added"
				});
			}
			return RedirectToAction("Create", "Typist", new
			{
				message = "Failed"
			});
		}

		[AllowAnonymous]
		public async Task<ActionResult> ConfirmEmail(string userId, string code)
		{
			if (userId == null || code == null)
			{
				return View("Error");
			}
			return View((await UserManager.ConfirmEmailAsync(userId, code)).Succeeded ? "ConfirmEmail" : "Error");
		}

		[AllowAnonymous]
		public ActionResult ForgotPassword()
		{
			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
		{
			if (base.ModelState.IsValid)
			{
				ApplicationUser user = await UserManager.FindByNameAsync(model.Email);
				bool flag = user == null;
				bool flag2 = flag;
				if (!flag2)
				{
					flag2 = !(await UserManager.IsEmailConfirmedAsync(user.Id));
				}
				if (flag2)
				{
					return View("ForgotPasswordConfirmation");
				}
			}
			return View(model);
		}

		[AllowAnonymous]
		public ActionResult ForgotPasswordConfirmation()
		{
			return View();
		}

		[AllowAnonymous]
		public ActionResult ResetPassword(string code)
		{
			return (code == null) ? View("Error") : View();
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
		{
			if (!base.ModelState.IsValid)
			{
				return View(model);
			}
			ApplicationUser user = await UserManager.FindByNameAsync(model.Email);
			if (user == null)
			{
				return RedirectToAction("ResetPasswordConfirmation", "Account");
			}
			IdentityResult result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
			if (result.Succeeded)
			{
				return RedirectToAction("ResetPasswordConfirmation", "Account");
			}
			AddErrors(result);
			return View();
		}

		public async Task<ActionResult> ResetUserPassword(ResetPasswordViewModel model)
		{
			if (!base.ModelState.IsValid)
			{
				return View(model);
			}
			ApplicationUser user = await UserManager.FindByNameAsync(model.Email);
			if (user == null)
			{
				return RedirectToAction("ResetPasswordConfirmation", "Account");
			}
			IdentityResult result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
			if (result.Succeeded)
			{
				return RedirectToAction("ResetPasswordConfirmation", "Account");
			}
			AddErrors(result);
			return View();
		}

		[AllowAnonymous]
		public ActionResult ResetPasswordConfirmation()
		{
			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public ActionResult ExternalLogin(string provider, string returnUrl)
		{
			return new ChallengeResult(provider, base.Url.Action("ExternalLoginCallback", "Account", new
			{
				ReturnUrl = returnUrl
			}));
		}

		[AllowAnonymous]
		public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
		{
			string userId = await SignInManager.GetVerifiedUserIdAsync();
			if (userId == null)
			{
				return View("Error");
			}
			List<SelectListItem> factorOptions = (await UserManager.GetValidTwoFactorProvidersAsync(userId)).Select((string purpose) => new SelectListItem
			{
				Text = purpose,
				Value = purpose
			}).ToList();
			return View(new SendCodeViewModel
			{
				Providers = factorOptions,
				ReturnUrl = returnUrl,
				RememberMe = rememberMe
			});
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> SendCode(SendCodeViewModel model)
		{
			if (!base.ModelState.IsValid)
			{
				return View();
			}
			if (!(await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider)))
			{
				return View("Error");
			}
			return RedirectToAction("VerifyCode", new
			{
				Provider = model.SelectedProvider,
				ReturnUrl = model.ReturnUrl,
				RememberMe = model.RememberMe
			});
		}

		[AllowAnonymous]
		public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
		{
			ExternalLoginInfo loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
			if (loginInfo == null)
			{
				return RedirectToAction("Login");
			}
			switch (await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false))
			{
			case SignInStatus.Success:
				return RedirectToLocal(returnUrl);
			case SignInStatus.LockedOut:
				return View("Lockout");
			case SignInStatus.RequiresVerification:
				return RedirectToAction("SendCode", new
				{
					ReturnUrl = returnUrl,
					RememberMe = false
				});
			default:
				base.ViewBag.ReturnUrl = returnUrl;
				base.ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
				return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel
				{
					Email = loginInfo.Email
				});
			}
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
		{
			if (base.User.Identity.IsAuthenticated)
			{
				return RedirectToAction("Index", "Manage");
			}
			if (base.ModelState.IsValid)
			{
				ExternalLoginInfo info = await AuthenticationManager.GetExternalLoginInfoAsync();
				if (info == null)
				{
					return View("ExternalLoginFailure");
				}
				ApplicationUser user = new ApplicationUser
				{
					UserName = model.Email,
					Email = model.Email
				};
				IdentityResult result = await UserManager.CreateAsync(user);
				if (result.Succeeded)
				{
					result = await UserManager.AddLoginAsync(user.Id, info.Login);
					if (result.Succeeded)
					{
						await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
						return RedirectToLocal(returnUrl);
					}
				}
				AddErrors(result);
			}
			base.ViewBag.ReturnUrl = returnUrl;
			return View(model);
		}

		[HttpGet]
		public ActionResult LogOff()
		{
			AuthenticationManager.SignOut("ApplicationCookie");
			return RedirectToAction("Login");
		}

		[AllowAnonymous]
		public ActionResult ExternalLoginFailure()
		{
			return View();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_userManager != null)
				{
					_userManager.Dispose();
					_userManager = null;
				}
				if (_signInManager != null)
				{
					_signInManager.Dispose();
					_signInManager = null;
				}
			}
			base.Dispose(disposing);
		}

		private void AddErrors(IdentityResult result)
		{
			foreach (string error in result.Errors)
			{
				base.ModelState.AddModelError("", error);
			}
		}

		private ActionResult RedirectToLocal(string returnUrl)
		{
			if (base.Url.IsLocalUrl(returnUrl))
			{
				return Redirect(returnUrl);
			}
			return RedirectToAction("Index", "Home");
		}
	}
}
