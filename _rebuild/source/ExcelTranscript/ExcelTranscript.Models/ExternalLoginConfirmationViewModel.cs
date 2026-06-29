using System.ComponentModel.DataAnnotations;

namespace ExcelTranscript.Models
{
	public class ExternalLoginConfirmationViewModel
	{
		[Required]
		[Display(Name = "Email")]
		public string Email { get; set; }
	}
}
