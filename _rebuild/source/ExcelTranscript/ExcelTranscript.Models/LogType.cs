using System.Collections.Generic;

namespace ExcelTranscript.Models
{
	public class LogType
	{
		public int LogTypeID { get; set; }

		public string LogTypeTitle { get; set; }

		public virtual ICollection<FileLog> FileLogs { get; set; }

		public LogType()
		{
			FileLogs = new HashSet<FileLog>();
		}
	}
}
