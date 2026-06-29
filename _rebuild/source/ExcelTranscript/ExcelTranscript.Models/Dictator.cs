using System;
using System.Collections.Generic;

namespace ExcelTranscript.Models
{
	public class Dictator
	{
		public int DictatorID { get; set; }

		public int AccountID { get; set; }

		public string SystemID { get; set; }

		public string LoginID { get; set; }

		public string Password { get; set; }

		public bool AllClientAccess { get; set; }

		public string Prefix { get; set; }

		public string FirstName { get; set; }

		public string MiddleName { get; set; }

		public string LastName { get; set; }

		public string Gender { get; set; }

		public int SpecialityID { get; set; }

		public bool IsSecretary { get; set; }

		public bool IsFax { get; set; }

		public bool IsReview { get; set; }

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

		public string SecretaryName { get; set; }

		public int? NoOfLine { get; set; }

		public decimal? RateOfLine { get; set; }

		public bool? AddSign { get; set; }

		public virtual Clinic Clinic { get; set; }

		public virtual Speciality Speciality { get; set; }

		public virtual ICollection<Secretary> Secretaries { get; set; }

		public virtual ICollection<UploadedFile> UploadedFiles { get; set; }

		public virtual ICollection<UserLog> UserLogs { get; set; }

		public virtual ICollection<ManagerClinic> ManagerClinics { get; set; }

		public Dictator()
		{
			Secretaries = new HashSet<Secretary>();
			UploadedFiles = new HashSet<UploadedFile>();
			UserLogs = new HashSet<UserLog>();
			ManagerClinics = new HashSet<ManagerClinic>();
		}
	}
}
