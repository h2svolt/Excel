using System;
using System.Collections.Generic;

namespace ExcelTranscript.Models.DTO
{
	public class ManagerDTO
	{
		public int ManagerID { get; set; }

		public int AccountID { get; set; }

		public string Username { get; set; }

		public string Password { get; set; }

		public DateTime? AddedOn { get; set; }

		public int? AddedBy { get; set; }

		public DateTime? UpdatedOn { get; set; }

		public int? UpdatedBy { get; set; }

		public bool isActive { get; set; }

		public List<int> ClinicIds { get; set; }

		public List<int> DictatorIds { get; set; }
	}
}
