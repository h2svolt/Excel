using System;

namespace ExcelTranscript.Models
{
	public class stp_GetDocumentsDataNew_Result
	{
		public int AudioID { get; set; }

		public string Status { get; set; }

		public string Clinic { get; set; }

		public string Provider { get; set; }

		public string AudioFile { get; set; }

		public string DocumentName { get; set; }

		public string Duration { get; set; }

		public int? DocCounts { get; set; }

		public string StatusColor { get; set; }

		public int? TypistAssignID { get; set; }

		public int? StackMark { get; set; }

		public bool IsActive { get; set; }

		public DateTime? DOT { get; set; }

		public string ModalityName { get; set; }

		public int? DocumentId { get; set; }

		public string PatientName { get; set; }

		public string MRN { get; set; }

		public string DOS { get; set; }

		public string DOB { get; set; }

		public string RefDoctor { get; set; }

		public string FaxNumber { get; set; }

		public int? WordCount { get; set; }

		public int? CharCount { get; set; }

		public int? CharCountWithSpace { get; set; }
	}
}
