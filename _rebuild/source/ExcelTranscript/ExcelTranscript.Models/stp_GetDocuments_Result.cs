using System;

namespace ExcelTranscript.Models
{
	public class stp_GetDocuments_Result
	{
		public string AudioName { get; set; }

		public string Status { get; set; }

		public int? TypistAssignID { get; set; }

		public string Provider { get; set; }

		public string ClinicName { get; set; }

		public int? ModalityId { get; set; }

		public int AudioFileID { get; set; }

		public string FileName { get; set; }

		public string FilePath { get; set; }

		public int Id { get; set; }

		public string PatientName { get; set; }

		public string DOS { get; set; }

		public string DOB { get; set; }

		public string MRN { get; set; }

		public DateTime UploadOn { get; set; }

		public int UploadBy { get; set; }
	}
}
