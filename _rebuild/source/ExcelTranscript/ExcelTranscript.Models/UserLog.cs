using System;

namespace ExcelTranscript.Models
{
	public class UserLog
	{
		public int Id { get; set; }

		public int? AudioID { get; set; }

		public int? DocID { get; set; }

		public string FileName { get; set; }

		public string FileType { get; set; }

		public int? Action { get; set; }

		public string UserLogName { get; set; }

		public DateTime? Date { get; set; }

		public int? UserLogId { get; set; }

		public int? StatusId { get; set; }

		public int? ProviderId { get; set; }

		public virtual Status Status { get; set; }

		public virtual UploadedFile UploadedFile { get; set; }

		public virtual UploadedDocument UploadedDocument { get; set; }

		public virtual Dictator Dictator { get; set; }
	}
}
