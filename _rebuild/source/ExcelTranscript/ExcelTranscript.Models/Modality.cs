using System;

namespace ExcelTranscript.Models
{
	public class Modality
	{
		public int Id { get; set; }

		public string ModalityName { get; set; }

		public DateTime AddedOn { get; set; }

		public int AddedBy { get; set; }

		public DateTime? UpdatedOn { get; set; }

		public int? UpdatedBy { get; set; }
	}
}
