using System;
using System.Collections.Generic;

namespace ExcelTranscript.Models
{
	public class Typist
	{
		public int Id { get; set; }

		public string TypistName { get; set; }

		public DateTime AddedOn { get; set; }

		public int AddedBy { get; set; }

		public DateTime UpdatedOn { get; set; }

		public int UpdatedBy { get; set; }

		public virtual ICollection<UploadedFile> UploadedFiles { get; set; }

		public Typist()
		{
			UploadedFiles = new HashSet<UploadedFile>();
		}
	}
}
