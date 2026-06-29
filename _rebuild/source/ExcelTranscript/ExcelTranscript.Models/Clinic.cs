using System;
using System.Collections.Generic;

namespace ExcelTranscript.Models
{
	public class Clinic
	{
		public int ClinicID { get; set; }

		public string AccountID { get; set; }

		public string Name { get; set; }

		public string Address { get; set; }

		public string City { get; set; }

		public string State { get; set; }

		public string Country { get; set; }

		public string ZipCode { get; set; }

		public string Phone { get; set; }

		public string Cell { get; set; }

		public string FaxNo { get; set; }

		public string Email { get; set; }

		public string URL { get; set; }

		public DateTime AddedOn { get; set; }

		public int AddedBy { get; set; }

		public DateTime? UpdatedOn { get; set; }

		public int? UpdatedBy { get; set; }

		public bool IsActive { get; set; }

		public string CP_Name { get; set; }

		public string CP_Phone { get; set; }

		public string CP_Cell { get; set; }

		public string CP_Address { get; set; }

		public string CP_Email { get; set; }

		public string CP_Pager { get; set; }

		public string SystemID { get; set; }

		public virtual ICollection<UploadedFile> UploadedFiles { get; set; }

		public virtual ICollection<Dictator> Dictators { get; set; }

		public virtual ICollection<ManagerClinic> ManagerClinics { get; set; }

		public Clinic()
		{
			UploadedFiles = new HashSet<UploadedFile>();
			Dictators = new HashSet<Dictator>();
			ManagerClinics = new HashSet<ManagerClinic>();
		}
	}
}
