using System.Collections.Generic;

namespace ExcelTranscript.Models
{
	public class Status
	{
		public int Id { get; set; }

		public string Name { get; set; }

		public string Color { get; set; }

		public virtual ICollection<UserLog> UserLogs { get; set; }

		public virtual ICollection<UploadedFile> UploadedFiles { get; set; }

		public virtual ICollection<UploadedDocument> UploadedDocuments { get; set; }

		public Status()
		{
			UserLogs = new HashSet<UserLog>();
			UploadedFiles = new HashSet<UploadedFile>();
			UploadedDocuments = new HashSet<UploadedDocument>();
		}
	}
}
