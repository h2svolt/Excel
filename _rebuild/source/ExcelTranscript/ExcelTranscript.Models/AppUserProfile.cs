using System;

namespace ExcelTranscript.Models
{
	public class AppUserProfile
	{
		public int AppUserID { get; set; }

		public string LoginID { get; set; }

		public string Name { get; set; }

		public string Email { get; set; }

		public string Cell { get; set; }

		public string OtherPhone { get; set; }

		public string CNICNo { get; set; }

		public string Description { get; set; }

		public bool IsActive { get; set; }

		public DateTime AddOn { get; set; }

		public int AddBy { get; set; }

		public DateTime? UpdateOn { get; set; }

		public int? UpdateBy { get; set; }

		public virtual AspNetUser AspNetUser { get; set; }
	}
}
