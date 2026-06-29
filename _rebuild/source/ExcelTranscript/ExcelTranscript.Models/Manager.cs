using System;
using System.Collections.Generic;

namespace ExcelTranscript.Models
{
	public class Manager
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

		public virtual ICollection<ManagerClinic> ManagerClinics { get; set; }

		public Manager()
		{
			ManagerClinics = new HashSet<ManagerClinic>();
		}
	}
}
