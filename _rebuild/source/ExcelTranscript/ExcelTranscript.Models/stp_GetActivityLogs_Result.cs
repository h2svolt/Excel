using System;

namespace ExcelTranscript.Models
{
	public class stp_GetActivityLogs_Result
	{
		public int? AudioID { get; set; }

		public int? Action { get; set; }

		public string Status { get; set; }

		public string Provider { get; set; }

		public DateTime? ActivityDate { get; set; }

		public string AudioFile { get; set; }

		public string Duration { get; set; }

		public string StatusColor { get; set; }
	}
}
