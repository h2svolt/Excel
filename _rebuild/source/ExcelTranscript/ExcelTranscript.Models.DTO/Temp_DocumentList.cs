using System.Collections.Generic;

namespace ExcelTranscript.Models.DTO
{
	public class Temp_DocumentList
	{
		public List<stp_DocumentReport_Result> Doclst;

		public decimal TotalLines { get; set; }

		public decimal TotalAmount { get; set; }

		public int TotalDoc { get; set; }

		public string Clinic { get; set; }

		public string Provider { get; set; }

		public decimal GTotalLines { get; set; }

		public decimal GTotalAmount { get; set; }

		public int GTotalDoc { get; set; }
	}
}
