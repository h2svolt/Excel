using System;

namespace ExcelTranscript.Models
{
	public class stp_GetDictations_Result
	{
		public int? TypistAssignID { get; set; }

		public int UploadBy { get; set; }

		public string FilePath { get; set; }

		public string Duration { get; set; }

		public int FileID { get; set; }

		public string FileName { get; set; }

		public DateTime UploadOn { get; set; }

		public int? DictatorID { get; set; }

		public bool isDownloaded { get; set; }

		public string Provider { get; set; }

		public string ClinicName { get; set; }
	}
}
