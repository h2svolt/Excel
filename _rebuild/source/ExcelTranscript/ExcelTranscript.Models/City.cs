using System;

namespace ExcelTranscript.Models
{
	public class City
	{
		public int CityID { get; set; }

		public string Name { get; set; }

		public string ShortCode { get; set; }

		public DateTime AddedOn { get; set; }

		public int AddedBy { get; set; }

		public DateTime? UpdatedOn { get; set; }

		public int? UpdatedBy { get; set; }

		public bool IsActive { get; set; }

		public int CountryID { get; set; }

		public virtual Country Country { get; set; }
	}
}
