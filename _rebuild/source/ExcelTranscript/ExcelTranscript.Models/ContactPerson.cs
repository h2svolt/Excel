using System;

namespace ExcelTranscript.Models
{
	public class ContactPerson
	{
		public int CPID { get; set; }

		public string Name { get; set; }

		public string Phone { get; set; }

		public string Cell { get; set; }

		public string Address { get; set; }

		public string Email { get; set; }

		public string Pager { get; set; }

		public int AccountTypeID { get; set; }

		public int AccountID { get; set; }

		public DateTime AddedOn { get; set; }

		public int AddedBy { get; set; }

		public DateTime? UpdatedOn { get; set; }

		public int? UpdatedBy { get; set; }

		public bool IsActive { get; set; }
	}
}
