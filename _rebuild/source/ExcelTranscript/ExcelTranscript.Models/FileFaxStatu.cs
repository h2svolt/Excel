using System;

namespace ExcelTranscript.Models
{
	public class FileFaxStatu
	{
		public int ID { get; set; }

		public string FaxID { get; set; }

		public int UpDocID { get; set; }

		public int FileID { get; set; }

		public string Status { get; set; }

		public DateTime AddedOn { get; set; }

		public string ToNumber { get; set; }

		public string DateCreated { get; set; }

		public string PageCount { get; set; }

		public bool IsActive { get; set; }
	}
}
