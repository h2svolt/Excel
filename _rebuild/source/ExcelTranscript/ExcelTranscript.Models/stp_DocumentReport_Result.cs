using System;

namespace ExcelTranscript.Models
{
	public class stp_DocumentReport_Result
	{
		public int? ProviderId { get; set; }

		public string Provider { get; set; }

		public string Clinic { get; set; }

		public string AudioFile { get; set; }

		public string DocFile { get; set; }

		public string Extension { get; set; }

		public string Size { get; set; }

		public int? WordCount { get; set; }

		public int? CharacterCount { get; set; }

		public decimal? LineCount { get; set; }

		public string UploadedBy { get; set; }

		public DateTime UploadOn { get; set; }

		public string Status { get; set; }

		public string TypistAssign { get; set; }

		public int? NoOfCharacterPerLine { get; set; }

		public decimal? RatePerLine { get; set; }
	}
}
