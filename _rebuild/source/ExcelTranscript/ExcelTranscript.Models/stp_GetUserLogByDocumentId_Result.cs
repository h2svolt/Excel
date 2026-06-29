using System;

namespace ExcelTranscript.Models
{
	public class stp_GetUserLogByDocumentId_Result
	{
		public int? AudioID { get; set; }

		public string FileName { get; set; }

		public string FileType { get; set; }

		public int? Action { get; set; }

		public string UserLogName { get; set; }

		public DateTime? Date { get; set; }

		public int? UserLogId { get; set; }

		public string Name { get; set; }

		public string Color { get; set; }

		public string Provider { get; set; }
	}
}
