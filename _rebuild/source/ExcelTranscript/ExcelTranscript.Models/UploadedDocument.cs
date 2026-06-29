using System;
using System.Collections.Generic;

namespace ExcelTranscript.Models
{
	public class UploadedDocument
	{
		public int Id { get; set; }

		public int? FileID { get; set; }

		public string FileName { get; set; }

		public string FilePath { get; set; }

		public string Extension { get; set; }

		public string Size { get; set; }

		public string Decription { get; set; }

		public int? WordCount { get; set; }

		public bool? IsApproved { get; set; }

		public int? StatusId { get; set; }

		public int UploadBy { get; set; }

		public DateTime UploadOn { get; set; }

		public string Duration { get; set; }

		public int? DocChildID { get; set; }

		public int? CharCount { get; set; }

		public int? CharCountWithSpace { get; set; }

		public decimal? LineCount { get; set; }

		public string PatientName { get; set; }

		public int? ModalityId { get; set; }

		public string MRN { get; set; }

		public string DOB { get; set; }

		public string DOS { get; set; }

		public string RefDoctor { get; set; }

		public string FaxNumber { get; set; }

		public virtual Status Status { get; set; }

		public virtual UploadedFile UploadedFile { get; set; }

		public virtual ICollection<UserLog> UserLogs { get; set; }

		public UploadedDocument()
		{
			UserLogs = new HashSet<UserLog>();
		}
	}
}
