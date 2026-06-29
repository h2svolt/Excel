namespace ExcelTranscript.Models
{
	public class AspNetUserRole
	{
		public string UserId { get; set; }

		public string RoleId { get; set; }

		public virtual AspNetUser AspNetUser { get; set; }
	}
}
