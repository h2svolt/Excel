using System.ComponentModel.DataAnnotations;

namespace ExcelTranscript.Models
{
	public class ForgotViewModel
	{
		[Required]
		[Display(Name = "Email")]
		public string Email { get; set; }
	}
}
