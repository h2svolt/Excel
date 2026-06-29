using System;
using System.Collections.Generic;

namespace ExcelTranscript.Models
{
	public class Speciality
	{
		public int SpecialityID { get; set; }

		public string Name { get; set; }

		public DateTime AddedOn { get; set; }

		public int AddedBy { get; set; }

		public DateTime? UpdatedOn { get; set; }

		public int? UpdatedBy { get; set; }

		public bool IsActive { get; set; }

		public virtual ICollection<Dictator> Dictators { get; set; }

		public Speciality()
		{
			Dictators = new HashSet<Dictator>();
		}
	}
}
