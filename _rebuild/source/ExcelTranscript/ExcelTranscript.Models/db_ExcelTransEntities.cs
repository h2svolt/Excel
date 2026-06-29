using System;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace ExcelTranscript.Models
{
	public class db_ExcelTransEntities : DbContext
	{
		public virtual DbSet<AppUserProfile> AppUserProfiles { get; set; }

		public virtual DbSet<AspNetRole> AspNetRoles { get; set; }

		public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }

		public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }

		public virtual DbSet<AspNetUserRole> AspNetUserRoles { get; set; }

		public virtual DbSet<AspNetUser> AspNetUsers { get; set; }

		public virtual DbSet<AccountType> AccountTypes { get; set; }

		public virtual DbSet<City> Cities { get; set; }

		public virtual DbSet<Clinic> Clinics { get; set; }

		public virtual DbSet<ContactPerson> ContactPersons { get; set; }

		public virtual DbSet<Country> Countries { get; set; }

		public virtual DbSet<Speciality> Specialities { get; set; }

		public virtual DbSet<FileLog> FileLogs { get; set; }

		public virtual DbSet<LogType> LogTypes { get; set; }

		public virtual DbSet<Secretary> Secretaries { get; set; }

		public virtual DbSet<Status> Status { get; set; }

		public virtual DbSet<Typist> Typists { get; set; }

		public virtual DbSet<UserLog> UserLogs { get; set; }

		public virtual DbSet<UploadedFile> UploadedFiles { get; set; }

		public virtual DbSet<Modality> Modalities { get; set; }

		public virtual DbSet<FileFaxStatu> FileFaxStatus { get; set; }

		public virtual DbSet<DictatorSecretary> DictatorSecretaries { get; set; }

		public virtual DbSet<UploadedDocument> UploadedDocuments { get; set; }

		public virtual DbSet<Dictator> Dictators { get; set; }

		public virtual DbSet<Manager> Managers { get; set; }

		public virtual DbSet<ManagerClinic> ManagerClinics { get; set; }

		public db_ExcelTransEntities()
			: base("name=db_ExcelTransEntities")
		{
		}

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			throw new UnintentionalCodeFirstException();
		}

		public virtual int stp_SecretaryUpdate(string secretaryName, int? dictatorID, int? userId)
		{
			ObjectParameter objectParameter = ((secretaryName != null) ? new ObjectParameter("SecretaryName", secretaryName) : new ObjectParameter("SecretaryName", typeof(string)));
			ObjectParameter objectParameter2 = (dictatorID.HasValue ? new ObjectParameter("DictatorID", dictatorID) : new ObjectParameter("DictatorID", typeof(int)));
			ObjectParameter objectParameter3 = (userId.HasValue ? new ObjectParameter("UserId", userId) : new ObjectParameter("UserId", typeof(int)));
			return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("stp_SecretaryUpdate", objectParameter, objectParameter2, objectParameter3);
		}

		public virtual int Proc_InsertErrorDetails()
		{
			return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("Proc_InsertErrorDetails");
		}

		public virtual ObjectResult<stp_GetTypist_Result> stp_GetTypist()
		{
			return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<stp_GetTypist_Result>("stp_GetTypist", Array.Empty<ObjectParameter>());
		}

		public virtual ObjectResult<stp_GetUserLog_Result> stp_GetUserLog(int? audioID)
		{
			ObjectParameter objectParameter = (audioID.HasValue ? new ObjectParameter("AudioID", audioID) : new ObjectParameter("AudioID", typeof(int)));
			return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<stp_GetUserLog_Result>("stp_GetUserLog", new ObjectParameter[1] { objectParameter });
		}

		public virtual ObjectResult<stp_DocumentReport_Result> stp_DocumentReport(int? dictatorID, int? clinicID, int? statusId, DateTime? fromDate, DateTime? toDate, int? typistAssignId)
		{
			ObjectParameter objectParameter = (dictatorID.HasValue ? new ObjectParameter("DictatorID", dictatorID) : new ObjectParameter("DictatorID", typeof(int)));
			ObjectParameter objectParameter2 = (clinicID.HasValue ? new ObjectParameter("ClinicID", clinicID) : new ObjectParameter("ClinicID", typeof(int)));
			ObjectParameter objectParameter3 = (statusId.HasValue ? new ObjectParameter("StatusId", statusId) : new ObjectParameter("StatusId", typeof(int)));
			ObjectParameter objectParameter4 = (fromDate.HasValue ? new ObjectParameter("FromDate", fromDate) : new ObjectParameter("FromDate", typeof(DateTime)));
			ObjectParameter objectParameter5 = (toDate.HasValue ? new ObjectParameter("ToDate", toDate) : new ObjectParameter("ToDate", typeof(DateTime)));
			ObjectParameter objectParameter6 = (typistAssignId.HasValue ? new ObjectParameter("TypistAssignId", typistAssignId) : new ObjectParameter("TypistAssignId", typeof(int)));
			return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<stp_DocumentReport_Result>("stp_DocumentReport", new ObjectParameter[6] { objectParameter, objectParameter2, objectParameter3, objectParameter4, objectParameter5, objectParameter6 });
		}

		public virtual int sp_alterdiagram(string diagramname, int? owner_id, int? version, byte[] definition)
		{
			ObjectParameter objectParameter = ((diagramname != null) ? new ObjectParameter("diagramname", diagramname) : new ObjectParameter("diagramname", typeof(string)));
			ObjectParameter objectParameter2 = (owner_id.HasValue ? new ObjectParameter("owner_id", owner_id) : new ObjectParameter("owner_id", typeof(int)));
			ObjectParameter objectParameter3 = (version.HasValue ? new ObjectParameter("version", version) : new ObjectParameter("version", typeof(int)));
			ObjectParameter objectParameter4 = ((definition != null) ? new ObjectParameter("definition", definition) : new ObjectParameter("definition", typeof(byte[])));
			return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_alterdiagram", objectParameter, objectParameter2, objectParameter3, objectParameter4);
		}

		public virtual int sp_creatediagram(string diagramname, int? owner_id, int? version, byte[] definition)
		{
			ObjectParameter objectParameter = ((diagramname != null) ? new ObjectParameter("diagramname", diagramname) : new ObjectParameter("diagramname", typeof(string)));
			ObjectParameter objectParameter2 = (owner_id.HasValue ? new ObjectParameter("owner_id", owner_id) : new ObjectParameter("owner_id", typeof(int)));
			ObjectParameter objectParameter3 = (version.HasValue ? new ObjectParameter("version", version) : new ObjectParameter("version", typeof(int)));
			ObjectParameter objectParameter4 = ((definition != null) ? new ObjectParameter("definition", definition) : new ObjectParameter("definition", typeof(byte[])));
			return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_creatediagram", objectParameter, objectParameter2, objectParameter3, objectParameter4);
		}

		public virtual int sp_dropdiagram(string diagramname, int? owner_id)
		{
			ObjectParameter objectParameter = ((diagramname != null) ? new ObjectParameter("diagramname", diagramname) : new ObjectParameter("diagramname", typeof(string)));
			ObjectParameter objectParameter2 = (owner_id.HasValue ? new ObjectParameter("owner_id", owner_id) : new ObjectParameter("owner_id", typeof(int)));
			return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_dropdiagram", objectParameter, objectParameter2);
		}

		public virtual ObjectResult<sp_helpdiagramdefinition_Result> sp_helpdiagramdefinition(string diagramname, int? owner_id)
		{
			ObjectParameter objectParameter = ((diagramname != null) ? new ObjectParameter("diagramname", diagramname) : new ObjectParameter("diagramname", typeof(string)));
			ObjectParameter objectParameter2 = (owner_id.HasValue ? new ObjectParameter("owner_id", owner_id) : new ObjectParameter("owner_id", typeof(int)));
			return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_helpdiagramdefinition_Result>("sp_helpdiagramdefinition", new ObjectParameter[2] { objectParameter, objectParameter2 });
		}

		public virtual ObjectResult<sp_helpdiagrams_Result> sp_helpdiagrams(string diagramname, int? owner_id)
		{
			ObjectParameter objectParameter = ((diagramname != null) ? new ObjectParameter("diagramname", diagramname) : new ObjectParameter("diagramname", typeof(string)));
			ObjectParameter objectParameter2 = (owner_id.HasValue ? new ObjectParameter("owner_id", owner_id) : new ObjectParameter("owner_id", typeof(int)));
			return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_helpdiagrams_Result>("sp_helpdiagrams", new ObjectParameter[2] { objectParameter, objectParameter2 });
		}

		public virtual int sp_renamediagram(string diagramname, int? owner_id, string new_diagramname)
		{
			ObjectParameter objectParameter = ((diagramname != null) ? new ObjectParameter("diagramname", diagramname) : new ObjectParameter("diagramname", typeof(string)));
			ObjectParameter objectParameter2 = (owner_id.HasValue ? new ObjectParameter("owner_id", owner_id) : new ObjectParameter("owner_id", typeof(int)));
			ObjectParameter objectParameter3 = ((new_diagramname != null) ? new ObjectParameter("new_diagramname", new_diagramname) : new ObjectParameter("new_diagramname", typeof(string)));
			return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_renamediagram", objectParameter, objectParameter2, objectParameter3);
		}

		public virtual int sp_upgraddiagrams()
		{
			return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_upgraddiagrams");
		}

		public virtual ObjectResult<stp_GetUploadedAudio_Result> stp_GetUploadedAudio(int? dictatorID, int? clinicID, int? statusId, DateTime? fromDate, DateTime? toDate, int? typistAssignId)
		{
			ObjectParameter objectParameter = (dictatorID.HasValue ? new ObjectParameter("DictatorID", dictatorID) : new ObjectParameter("DictatorID", typeof(int)));
			ObjectParameter objectParameter2 = (clinicID.HasValue ? new ObjectParameter("ClinicID", clinicID) : new ObjectParameter("ClinicID", typeof(int)));
			ObjectParameter objectParameter3 = (statusId.HasValue ? new ObjectParameter("StatusId", statusId) : new ObjectParameter("StatusId", typeof(int)));
			ObjectParameter objectParameter4 = (fromDate.HasValue ? new ObjectParameter("FromDate", fromDate) : new ObjectParameter("FromDate", typeof(DateTime)));
			ObjectParameter objectParameter5 = (toDate.HasValue ? new ObjectParameter("ToDate", toDate) : new ObjectParameter("ToDate", typeof(DateTime)));
			ObjectParameter objectParameter6 = (typistAssignId.HasValue ? new ObjectParameter("TypistAssignId", typistAssignId) : new ObjectParameter("TypistAssignId", typeof(int)));
			return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<stp_GetUploadedAudio_Result>("stp_GetUploadedAudio", new ObjectParameter[6] { objectParameter, objectParameter2, objectParameter3, objectParameter4, objectParameter5, objectParameter6 });
		}

		public virtual ObjectResult<stp_GetUploadedAudioInfo_Result> stp_GetUploadedAudioInfo(int? dictatorID, int? clinicID, int? statusId, DateTime? fromDate, DateTime? toDate, int? typistAssignId)
		{
			ObjectParameter objectParameter = (dictatorID.HasValue ? new ObjectParameter("DictatorID", dictatorID) : new ObjectParameter("DictatorID", typeof(int)));
			ObjectParameter objectParameter2 = (clinicID.HasValue ? new ObjectParameter("ClinicID", clinicID) : new ObjectParameter("ClinicID", typeof(int)));
			ObjectParameter objectParameter3 = (statusId.HasValue ? new ObjectParameter("StatusId", statusId) : new ObjectParameter("StatusId", typeof(int)));
			ObjectParameter objectParameter4 = (fromDate.HasValue ? new ObjectParameter("FromDate", fromDate) : new ObjectParameter("FromDate", typeof(DateTime)));
			ObjectParameter objectParameter5 = (toDate.HasValue ? new ObjectParameter("ToDate", toDate) : new ObjectParameter("ToDate", typeof(DateTime)));
			ObjectParameter objectParameter6 = (typistAssignId.HasValue ? new ObjectParameter("TypistAssignId", typistAssignId) : new ObjectParameter("TypistAssignId", typeof(int)));
			return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<stp_GetUploadedAudioInfo_Result>("stp_GetUploadedAudioInfo", new ObjectParameter[6] { objectParameter, objectParameter2, objectParameter3, objectParameter4, objectParameter5, objectParameter6 });
		}

		public virtual ObjectResult<stp_GetActivityLogs_Result> stp_GetActivityLogs(int? dictatorID, int? statusId, DateTime? fromDate, DateTime? toDate)
		{
			ObjectParameter objectParameter = (dictatorID.HasValue ? new ObjectParameter("DictatorID", dictatorID) : new ObjectParameter("DictatorID", typeof(int)));
			ObjectParameter objectParameter2 = (statusId.HasValue ? new ObjectParameter("StatusId", statusId) : new ObjectParameter("StatusId", typeof(int)));
			ObjectParameter objectParameter3 = (fromDate.HasValue ? new ObjectParameter("FromDate", fromDate) : new ObjectParameter("FromDate", typeof(DateTime)));
			ObjectParameter objectParameter4 = (toDate.HasValue ? new ObjectParameter("ToDate", toDate) : new ObjectParameter("ToDate", typeof(DateTime)));
			return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<stp_GetActivityLogs_Result>("stp_GetActivityLogs", new ObjectParameter[4] { objectParameter, objectParameter2, objectParameter3, objectParameter4 });
		}

		public virtual ObjectResult<stp_GetUserLogByDocumentId_Result> stp_GetUserLogByDocumentId(int? docID)
		{
			ObjectParameter objectParameter = (docID.HasValue ? new ObjectParameter("DocID", docID) : new ObjectParameter("DocID", typeof(int)));
			return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<stp_GetUserLogByDocumentId_Result>("stp_GetUserLogByDocumentId", new ObjectParameter[1] { objectParameter });
		}

		public virtual ObjectResult<stp_GetDocuments_Result> stp_GetDocuments(int? dictatorIDs, int? clinicID, int? typistID, int? statusId, DateTime? fromDate, DateTime? toDate)
		{
			ObjectParameter objectParameter = (dictatorIDs.HasValue ? new ObjectParameter("DictatorIDs", dictatorIDs) : new ObjectParameter("DictatorIDs", typeof(int)));
			ObjectParameter objectParameter2 = (clinicID.HasValue ? new ObjectParameter("ClinicID", clinicID) : new ObjectParameter("ClinicID", typeof(int)));
			ObjectParameter objectParameter3 = (typistID.HasValue ? new ObjectParameter("TypistID", typistID) : new ObjectParameter("TypistID", typeof(int)));
			ObjectParameter objectParameter4 = (statusId.HasValue ? new ObjectParameter("StatusId", statusId) : new ObjectParameter("StatusId", typeof(int)));
			ObjectParameter objectParameter5 = (fromDate.HasValue ? new ObjectParameter("FromDate", fromDate) : new ObjectParameter("FromDate", typeof(DateTime)));
			ObjectParameter objectParameter6 = (toDate.HasValue ? new ObjectParameter("ToDate", toDate) : new ObjectParameter("ToDate", typeof(DateTime)));
			return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<stp_GetDocuments_Result>("stp_GetDocuments", new ObjectParameter[6] { objectParameter, objectParameter2, objectParameter3, objectParameter4, objectParameter5, objectParameter6 });
		}

		public virtual ObjectResult<stp_GetDocumentsData_Result> stp_GetDocumentsData(int? dictatorID, int? clinicID, int? typistID, int? statusId, DateTime? dotFromDate, DateTime? dotToDate, DateTime? dosFromDate, DateTime? dosToDate, DateTime? dobFromDate, DateTime? dobToDate)
		{
			ObjectParameter objectParameter = (dictatorID.HasValue ? new ObjectParameter("DictatorID", dictatorID) : new ObjectParameter("DictatorID", typeof(int)));
			ObjectParameter objectParameter2 = (clinicID.HasValue ? new ObjectParameter("ClinicID", clinicID) : new ObjectParameter("ClinicID", typeof(int)));
			ObjectParameter objectParameter3 = (typistID.HasValue ? new ObjectParameter("TypistID", typistID) : new ObjectParameter("TypistID", typeof(int)));
			ObjectParameter objectParameter4 = (statusId.HasValue ? new ObjectParameter("StatusId", statusId) : new ObjectParameter("StatusId", typeof(int)));
			ObjectParameter objectParameter5 = (dotFromDate.HasValue ? new ObjectParameter("DotFromDate", dotFromDate) : new ObjectParameter("DotFromDate", typeof(DateTime)));
			ObjectParameter objectParameter6 = (dotToDate.HasValue ? new ObjectParameter("DotToDate", dotToDate) : new ObjectParameter("DotToDate", typeof(DateTime)));
			ObjectParameter objectParameter7 = (dosFromDate.HasValue ? new ObjectParameter("DosFromDate", dosFromDate) : new ObjectParameter("DosFromDate", typeof(DateTime)));
			ObjectParameter objectParameter8 = (dosToDate.HasValue ? new ObjectParameter("DosToDate", dosToDate) : new ObjectParameter("DosToDate", typeof(DateTime)));
			ObjectParameter objectParameter9 = (dobFromDate.HasValue ? new ObjectParameter("DobFromDate", dobFromDate) : new ObjectParameter("DobFromDate", typeof(DateTime)));
			ObjectParameter objectParameter10 = (dobToDate.HasValue ? new ObjectParameter("DobToDate", dobToDate) : new ObjectParameter("DobToDate", typeof(DateTime)));
			return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<stp_GetDocumentsData_Result>("stp_GetDocumentsData", new ObjectParameter[10] { objectParameter, objectParameter2, objectParameter3, objectParameter4, objectParameter5, objectParameter6, objectParameter7, objectParameter8, objectParameter9, objectParameter10 });
		}

		public virtual int stp_Dictator(int? id, int? accountID, string systemID, string loginID, string password, bool? allClientAccess, string prefix, string firstName, string middleName, string lastName, string gender, int? specialityID, bool? isSecretary, bool? isFax, bool? isReview, DateTime? addedOn, int? addedBy, DateTime? updatedOn, int? updatedBy, bool? isActive, string cP_Name, string cP_Phone, string cP_Cell, string cP_Address, string cP_Email, string cP_Pager, string secretaryName, int? noOfLine, decimal? rateOfLine, bool? addSign)
		{
			ObjectParameter objectParameter = (id.HasValue ? new ObjectParameter("Id", id) : new ObjectParameter("Id", typeof(int)));
			ObjectParameter objectParameter2 = (accountID.HasValue ? new ObjectParameter("AccountID", accountID) : new ObjectParameter("AccountID", typeof(int)));
			ObjectParameter objectParameter3 = ((systemID != null) ? new ObjectParameter("SystemID", systemID) : new ObjectParameter("SystemID", typeof(string)));
			ObjectParameter objectParameter4 = ((loginID != null) ? new ObjectParameter("LoginID", loginID) : new ObjectParameter("LoginID", typeof(string)));
			ObjectParameter objectParameter5 = ((password != null) ? new ObjectParameter("Password", password) : new ObjectParameter("Password", typeof(string)));
			ObjectParameter objectParameter6 = (allClientAccess.HasValue ? new ObjectParameter("AllClientAccess", allClientAccess) : new ObjectParameter("AllClientAccess", typeof(bool)));
			ObjectParameter objectParameter7 = ((prefix != null) ? new ObjectParameter("Prefix", prefix) : new ObjectParameter("Prefix", typeof(string)));
			ObjectParameter objectParameter8 = ((firstName != null) ? new ObjectParameter("FirstName", firstName) : new ObjectParameter("FirstName", typeof(string)));
			ObjectParameter objectParameter9 = ((middleName != null) ? new ObjectParameter("MiddleName", middleName) : new ObjectParameter("MiddleName", typeof(string)));
			ObjectParameter objectParameter10 = ((lastName != null) ? new ObjectParameter("LastName", lastName) : new ObjectParameter("LastName", typeof(string)));
			ObjectParameter objectParameter11 = ((gender != null) ? new ObjectParameter("Gender", gender) : new ObjectParameter("Gender", typeof(string)));
			ObjectParameter objectParameter12 = (specialityID.HasValue ? new ObjectParameter("SpecialityID", specialityID) : new ObjectParameter("SpecialityID", typeof(int)));
			ObjectParameter objectParameter13 = (isSecretary.HasValue ? new ObjectParameter("IsSecretary", isSecretary) : new ObjectParameter("IsSecretary", typeof(bool)));
			ObjectParameter objectParameter14 = (isFax.HasValue ? new ObjectParameter("IsFax", isFax) : new ObjectParameter("IsFax", typeof(bool)));
			ObjectParameter objectParameter15 = (isReview.HasValue ? new ObjectParameter("IsReview", isReview) : new ObjectParameter("IsReview", typeof(bool)));
			ObjectParameter objectParameter16 = (addedOn.HasValue ? new ObjectParameter("AddedOn", addedOn) : new ObjectParameter("AddedOn", typeof(DateTime)));
			ObjectParameter objectParameter17 = (addedBy.HasValue ? new ObjectParameter("AddedBy", addedBy) : new ObjectParameter("AddedBy", typeof(int)));
			ObjectParameter objectParameter18 = (updatedOn.HasValue ? new ObjectParameter("UpdatedOn", updatedOn) : new ObjectParameter("UpdatedOn", typeof(DateTime)));
			ObjectParameter objectParameter19 = (updatedBy.HasValue ? new ObjectParameter("UpdatedBy", updatedBy) : new ObjectParameter("UpdatedBy", typeof(int)));
			ObjectParameter objectParameter20 = (isActive.HasValue ? new ObjectParameter("IsActive", isActive) : new ObjectParameter("IsActive", typeof(bool)));
			ObjectParameter objectParameter21 = ((cP_Name != null) ? new ObjectParameter("CP_Name", cP_Name) : new ObjectParameter("CP_Name", typeof(string)));
			ObjectParameter objectParameter22 = ((cP_Phone != null) ? new ObjectParameter("CP_Phone", cP_Phone) : new ObjectParameter("CP_Phone", typeof(string)));
			ObjectParameter objectParameter23 = ((cP_Cell != null) ? new ObjectParameter("CP_Cell", cP_Cell) : new ObjectParameter("CP_Cell", typeof(string)));
			ObjectParameter objectParameter24 = ((cP_Address != null) ? new ObjectParameter("CP_Address", cP_Address) : new ObjectParameter("CP_Address", typeof(string)));
			ObjectParameter objectParameter25 = ((cP_Email != null) ? new ObjectParameter("CP_Email", cP_Email) : new ObjectParameter("CP_Email", typeof(string)));
			ObjectParameter objectParameter26 = ((cP_Pager != null) ? new ObjectParameter("CP_Pager", cP_Pager) : new ObjectParameter("CP_Pager", typeof(string)));
			ObjectParameter objectParameter27 = ((secretaryName != null) ? new ObjectParameter("SecretaryName", secretaryName) : new ObjectParameter("SecretaryName", typeof(string)));
			ObjectParameter objectParameter28 = (noOfLine.HasValue ? new ObjectParameter("NoOfLine", noOfLine) : new ObjectParameter("NoOfLine", typeof(int)));
			ObjectParameter objectParameter29 = (rateOfLine.HasValue ? new ObjectParameter("RateOfLine", rateOfLine) : new ObjectParameter("RateOfLine", typeof(decimal)));
			ObjectParameter objectParameter30 = (addSign.HasValue ? new ObjectParameter("AddSign", addSign) : new ObjectParameter("AddSign", typeof(bool)));
			return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("stp_Dictator", objectParameter, objectParameter2, objectParameter3, objectParameter4, objectParameter5, objectParameter6, objectParameter7, objectParameter8, objectParameter9, objectParameter10, objectParameter11, objectParameter12, objectParameter13, objectParameter14, objectParameter15, objectParameter16, objectParameter17, objectParameter18, objectParameter19, objectParameter20, objectParameter21, objectParameter22, objectParameter23, objectParameter24, objectParameter25, objectParameter26, objectParameter27, objectParameter28, objectParameter29, objectParameter30);
		}

		public virtual ObjectResult<GetManagers_Result> GetManagers(int? managerID)
		{
			ObjectParameter objectParameter = (managerID.HasValue ? new ObjectParameter("ManagerID", managerID) : new ObjectParameter("ManagerID", typeof(int)));
			return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetManagers_Result>("GetManagers", new ObjectParameter[1] { objectParameter });
		}

		public virtual ObjectResult<GetFaxStatusFilterWise_Result> GetFaxStatusFilterWise(string clinicIDs, DateTime? fromDate, DateTime? toDate, string providerIDs, int? typistID, string fileName)
		{
			ObjectParameter objectParameter = ((clinicIDs != null) ? new ObjectParameter("ClinicIDs", clinicIDs) : new ObjectParameter("ClinicIDs", typeof(string)));
			ObjectParameter objectParameter2 = (fromDate.HasValue ? new ObjectParameter("fromDate", fromDate) : new ObjectParameter("fromDate", typeof(DateTime)));
			ObjectParameter objectParameter3 = (toDate.HasValue ? new ObjectParameter("toDate", toDate) : new ObjectParameter("toDate", typeof(DateTime)));
			ObjectParameter objectParameter4 = ((providerIDs != null) ? new ObjectParameter("ProviderIDs", providerIDs) : new ObjectParameter("ProviderIDs", typeof(string)));
			ObjectParameter objectParameter5 = (typistID.HasValue ? new ObjectParameter("TypistID", typistID) : new ObjectParameter("TypistID", typeof(int)));
			ObjectParameter objectParameter6 = ((fileName != null) ? new ObjectParameter("FileName", fileName) : new ObjectParameter("FileName", typeof(string)));
			return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetFaxStatusFilterWise_Result>("GetFaxStatusFilterWise", new ObjectParameter[6] { objectParameter, objectParameter2, objectParameter3, objectParameter4, objectParameter5, objectParameter6 });
		}

		// MoM: Fax Status page DOT/DOS/DOB date filters. Calls the proc directly
		// (Database.SqlQuery maps by column name) so the extra date params do not
		// require changes to the EF .edmx function metadata.
		public virtual System.Collections.Generic.List<GetFaxStatusFilterWise_Result> GetFaxStatusFilterWiseEx(string clinicIDs, DateTime? fromDate, DateTime? toDate, string providerIDs, int? typistID, string fileName, DateTime? dotFromDate, DateTime? dotToDate, DateTime? dosFromDate, DateTime? dosToDate, DateTime? dobFromDate, DateTime? dobToDate)
		{
			var ps = new System.Data.SqlClient.SqlParameter[12]
			{
				new System.Data.SqlClient.SqlParameter("@ClinicIDs", (object)clinicIDs ?? DBNull.Value),
				new System.Data.SqlClient.SqlParameter("@fromDate", (object)fromDate ?? DBNull.Value),
				new System.Data.SqlClient.SqlParameter("@toDate", (object)toDate ?? DBNull.Value),
				new System.Data.SqlClient.SqlParameter("@ProviderIDs", (object)providerIDs ?? DBNull.Value),
				new System.Data.SqlClient.SqlParameter("@TypistID", (object)typistID ?? DBNull.Value),
				new System.Data.SqlClient.SqlParameter("@FileName", (object)fileName ?? DBNull.Value),
				new System.Data.SqlClient.SqlParameter("@DotFromDate", (object)dotFromDate ?? DBNull.Value),
				new System.Data.SqlClient.SqlParameter("@DotToDate", (object)dotToDate ?? DBNull.Value),
				new System.Data.SqlClient.SqlParameter("@DosFromDate", (object)dosFromDate ?? DBNull.Value),
				new System.Data.SqlClient.SqlParameter("@DosToDate", (object)dosToDate ?? DBNull.Value),
				new System.Data.SqlClient.SqlParameter("@DobFromDate", (object)dobFromDate ?? DBNull.Value),
				new System.Data.SqlClient.SqlParameter("@DobToDate", (object)dobToDate ?? DBNull.Value)
			};
			string sql = "EXEC dbo.GetFaxStatusFilterWise @ClinicIDs, @fromDate, @toDate, @ProviderIDs, @TypistID, @FileName, @DotFromDate, @DotToDate, @DosFromDate, @DosToDate, @DobFromDate, @DobToDate";
			return base.Database.SqlQuery<GetFaxStatusFilterWise_Result>(sql, ps).ToList();
		}

		public virtual ObjectResult<stp_GetDictations_Result> stp_GetDictations(string dictatorIDs, string clinicIDs, int? typistID, int? statusId, DateTime? fromDate, DateTime? toDate)
		{
			ObjectParameter objectParameter = ((dictatorIDs != null) ? new ObjectParameter("DictatorIDs", dictatorIDs) : new ObjectParameter("DictatorIDs", typeof(string)));
			ObjectParameter objectParameter2 = ((clinicIDs != null) ? new ObjectParameter("ClinicIDs", clinicIDs) : new ObjectParameter("ClinicIDs", typeof(string)));
			ObjectParameter objectParameter3 = (typistID.HasValue ? new ObjectParameter("TypistID", typistID) : new ObjectParameter("TypistID", typeof(int)));
			ObjectParameter objectParameter4 = (statusId.HasValue ? new ObjectParameter("StatusId", statusId) : new ObjectParameter("StatusId", typeof(int)));
			ObjectParameter objectParameter5 = (fromDate.HasValue ? new ObjectParameter("FromDate", fromDate) : new ObjectParameter("FromDate", typeof(DateTime)));
			ObjectParameter objectParameter6 = (toDate.HasValue ? new ObjectParameter("ToDate", toDate) : new ObjectParameter("ToDate", typeof(DateTime)));
			return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<stp_GetDictations_Result>("stp_GetDictations", new ObjectParameter[6] { objectParameter, objectParameter2, objectParameter3, objectParameter4, objectParameter5, objectParameter6 });
		}

		[DbFunction("db_ExcelTransEntities", "STRINGSplit_Fun")]
		public virtual IQueryable<STRINGSplit_Fun_Result> STRINGSplit_Fun(string pString, string pDelimiter)
		{
			ObjectParameter objectParameter = ((pString != null) ? new ObjectParameter("pString", pString) : new ObjectParameter("pString", typeof(string)));
			ObjectParameter objectParameter2 = ((pDelimiter != null) ? new ObjectParameter("pDelimiter", pDelimiter) : new ObjectParameter("pDelimiter", typeof(string)));
			return ((IObjectContextAdapter)this).ObjectContext.CreateQuery<STRINGSplit_Fun_Result>("[db_ExcelTransEntities].[STRINGSplit_Fun](@pString, @pDelimiter)", new ObjectParameter[2] { objectParameter, objectParameter2 });
		}

		public virtual ObjectResult<stp_GetDocumentsDataNew_Result> stp_GetDocumentsDataNew(string dictatorIDs, string clinicIDs, int? typistID, int? statusId, DateTime? dotFromDate, DateTime? dotToDate, DateTime? dosFromDate, DateTime? dosToDate, DateTime? doDFromDate, DateTime? doDToDate)
		{
			ObjectParameter objectParameter = ((dictatorIDs != null) ? new ObjectParameter("DictatorIDs", dictatorIDs) : new ObjectParameter("DictatorIDs", typeof(string)));
			ObjectParameter objectParameter2 = ((clinicIDs != null) ? new ObjectParameter("ClinicIDs", clinicIDs) : new ObjectParameter("ClinicIDs", typeof(string)));
			ObjectParameter objectParameter3 = (typistID.HasValue ? new ObjectParameter("TypistID", typistID) : new ObjectParameter("TypistID", typeof(int)));
			ObjectParameter objectParameter4 = (statusId.HasValue ? new ObjectParameter("StatusId", statusId) : new ObjectParameter("StatusId", typeof(int)));
			ObjectParameter objectParameter5 = (dotFromDate.HasValue ? new ObjectParameter("DotFromDate", dotFromDate) : new ObjectParameter("DotFromDate", typeof(DateTime)));
			ObjectParameter objectParameter6 = (dotToDate.HasValue ? new ObjectParameter("DotToDate", dotToDate) : new ObjectParameter("DotToDate", typeof(DateTime)));
			ObjectParameter objectParameter7 = (dosFromDate.HasValue ? new ObjectParameter("DosFromDate", dosFromDate) : new ObjectParameter("DosFromDate", typeof(DateTime)));
			ObjectParameter objectParameter8 = (dosToDate.HasValue ? new ObjectParameter("DosToDate", dosToDate) : new ObjectParameter("DosToDate", typeof(DateTime)));
			ObjectParameter objectParameter9 = (doDFromDate.HasValue ? new ObjectParameter("DoDFromDate", doDFromDate) : new ObjectParameter("DoDFromDate", typeof(DateTime)));
			ObjectParameter objectParameter10 = (doDToDate.HasValue ? new ObjectParameter("DoDToDate", doDToDate) : new ObjectParameter("DoDToDate", typeof(DateTime)));
			return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<stp_GetDocumentsDataNew_Result>("stp_GetDocumentsDataNew", new ObjectParameter[10] { objectParameter, objectParameter2, objectParameter3, objectParameter4, objectParameter5, objectParameter6, objectParameter7, objectParameter8, objectParameter9, objectParameter10 });
		}
	}
}
