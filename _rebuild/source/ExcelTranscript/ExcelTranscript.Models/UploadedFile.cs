using System;
using System.Collections.Generic;

namespace ExcelTranscript.Models
{
	public class UploadedFile
	{
		public int FileID { get; set; }

		public string FileName { get; set; }

		public string FilePath { get; set; }

		public string Extension { get; set; }

		public string Size { get; set; }

		public int? DictatorID { get; set; }

		public int? ClinicID { get; set; }

		public int UploadBy { get; set; }

		public DateTime UploadOn { get; set; }

		public bool IsActive { get; set; }

		public bool IsDownloaded { get; set; }

		public int? StatusId { get; set; }

		public string Duration { get; set; }

		public int? TypistId { get; set; }

		public int? TypistAssignID { get; set; }

		public int? StackMark { get; set; }

		public virtual Clinic Clinic { get; set; }

		public virtual ICollection<FileLog> FileLogs { get; set; }

		public virtual Status Status { get; set; }

		public virtual Typist Typist { get; set; }

		public virtual ICollection<UserLog> UserLogs { get; set; }

		public virtual ICollection<UploadedDocument> UploadedDocuments { get; set; }

		public virtual Dictator Dictator { get; set; }

		public UploadedFile()
		{
			FileLogs = new HashSet<FileLog>();
			UserLogs = new HashSet<UserLog>();
			UploadedDocuments = new HashSet<UploadedDocument>();
		}
	}
}
