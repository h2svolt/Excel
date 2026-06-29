using System;
using System.Collections.Generic;

namespace ExcelTranscript.Models
{
	public class Country
	{
		public int CountryID { get; set; }

		public string Name { get; set; }

		public string ShortCode { get; set; }

		public DateTime AddedOn { get; set; }

		public int AddedBy { get; set; }

		public DateTime? UpdatedOn { get; set; }

		public int? UpdatedBy { get; set; }

		public bool IsActive { get; set; }

		public virtual ICollection<City> Cities { get; set; }

		public Country()
		{
			Cities = new HashSet<City>();
		}
	}
}
