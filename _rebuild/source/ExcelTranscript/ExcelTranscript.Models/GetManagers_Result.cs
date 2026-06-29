using System;

namespace ExcelTranscript.Models
{
	public class GetManagers_Result
	{
		public int ManagerID { get; set; }

		public string Username { get; set; }

		public string Password { get; set; }

		public bool isActive { get; set; }

		public DateTime? AddedOn { get; set; }

		public int AccountID { get; set; }

		public string DictatorName { get; set; }

		public int DictatorID { get; set; }

		public string ClinicName { get; set; }

		public int ClinicID { get; set; }
	}
}
