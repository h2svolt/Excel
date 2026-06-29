using System.ComponentModel.DataAnnotations;

namespace ExcelTranscript.Models
{
	public class ExternalRegisterViewModel
	{
		[Required]
		[Display(Name = "UserRoles")]
		public string UserRoles { get; set; }

		[Display(Name = "Email")]
		public string Email { get; set; }

		[Required]
		[Display(Name = "UserName")]
		public string UserName { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "Password")]
		public string Password { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "Confirm password")]
		[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
		public string ConfirmPassword { get; set; }

		public string RedirectToControllerName { get; set; }
	}
}
