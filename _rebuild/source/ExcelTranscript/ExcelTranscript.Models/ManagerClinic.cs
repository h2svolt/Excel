namespace ExcelTranscript.Models
{
	public class ManagerClinic
	{
		public int Id { get; set; }

		public int ManagerID { get; set; }

		public int ClinicID { get; set; }

		public int DictatorID { get; set; }

		public virtual Clinic Clinic { get; set; }

		public virtual Dictator Dictator { get; set; }

		public virtual Manager Manager { get; set; }
	}
}
