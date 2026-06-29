using System;

namespace ExcelTranscript.Models
{
	public class FileLog
	{
		public int LogID { get; set; }

		public int FileID { get; set; }

		public DateTime? LogDate { get; set; }

		public int? LogTypeID { get; set; }

		public int UserID { get; set; }

		public virtual LogType LogType { get; set; }

		public virtual UploadedFile UploadedFile { get; set; }
	}
}
