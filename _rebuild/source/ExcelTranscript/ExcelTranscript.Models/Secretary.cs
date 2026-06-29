using System;

namespace ExcelTranscript.Models
{
	public class Secretary
	{
		public int SecretaryID { get; set; }

		public string Name { get; set; }

		public string ShortCode { get; set; }

		public DateTime AddedOn { get; set; }

		public int AddedBy { get; set; }

		public DateTime? UpdatedOn { get; set; }

		public int? UpdatedBy { get; set; }

		public bool IsActive { get; set; }

		public int? DictatorID { get; set; }

		public virtual Dictator Dictator { get; set; }
	}
}
