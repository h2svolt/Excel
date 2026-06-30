-- ================================================================
-- Excel on Work — run ONCE against your MonsterASP.NET MSSQL DB.
-- The host already created the database for you; do NOT create one.
-- In MonsterASP: Databases -> your MSSQL DB -> 'Run SQL query' (or
-- connect with SSMS using the external connection string they give).
-- Order inside: tables -> procedures -> seed -> users. Re-runnable.
-- ================================================================

-- ===================== schema_tables.sql =====================
GO
GO
IF OBJECT_ID('[dbo].[AccountType]','U') IS NULL
CREATE TABLE [dbo].[AccountType] (
    [AccountTypeID] int NOT NULL,
    [Title] nvarchar(50) NOT NULL,
    CONSTRAINT [PK_AccountType] PRIMARY KEY ([AccountTypeID])
);
GO
IF OBJECT_ID('[dbo].[AppUserProfile]','U') IS NULL
CREATE TABLE [dbo].[AppUserProfile] (
    [AppUserID] int NOT NULL,
    [LoginID] nvarchar(128) NOT NULL,
    [Name] nvarchar(150) NOT NULL,
    [Email] nvarchar(50) NULL,
    [Cell] varchar(20) NOT NULL,
    [OtherPhone] varchar(20) NULL,
    [CNICNo] varchar(20) NULL,
    [Description] nvarchar(200) NULL,
    [IsActive] bit NOT NULL,
    [AddOn] smalldatetime NOT NULL,
    [AddBy] int NOT NULL,
    [UpdateOn] smalldatetime NULL,
    [UpdateBy] int NULL,
    CONSTRAINT [PK_AppUserProfile] PRIMARY KEY ([AppUserID])
);
GO
IF OBJECT_ID('[dbo].[AspNetRoles]','U') IS NULL
CREATE TABLE [dbo].[AspNetRoles] (
    [Id] nvarchar(128) NOT NULL,
    [Name] nvarchar(256) NOT NULL,
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
);
GO
IF OBJECT_ID('[dbo].[AspNetUserClaims]','U') IS NULL
CREATE TABLE [dbo].[AspNetUserClaims] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [UserId] nvarchar(128) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id])
);
GO
IF OBJECT_ID('[dbo].[AspNetUserLogins]','U') IS NULL
CREATE TABLE [dbo].[AspNetUserLogins] (
    [LoginProvider] nvarchar(128) NOT NULL,
    [ProviderKey] nvarchar(128) NOT NULL,
    [UserId] nvarchar(128) NOT NULL,
    CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey], [UserId])
);
GO
IF OBJECT_ID('[dbo].[AspNetUserRoles]','U') IS NULL
CREATE TABLE [dbo].[AspNetUserRoles] (
    [UserId] nvarchar(128) NOT NULL,
    [RoleId] nvarchar(128) NOT NULL,
    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId])
);
GO
IF OBJECT_ID('[dbo].[AspNetUsers]','U') IS NULL
CREATE TABLE [dbo].[AspNetUsers] (
    [Id] nvarchar(128) NOT NULL,
    [Email] nvarchar(256) NULL,
    [EmailConfirmed] bit NOT NULL,
    [PasswordHash] nvarchar(max) NULL,
    [SecurityStamp] nvarchar(max) NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [PhoneNumberConfirmed] bit NOT NULL,
    [TwoFactorEnabled] bit NOT NULL,
    [LockoutEndDateUtc] datetime NULL,
    [LockoutEnabled] bit NOT NULL,
    [AccessFailedCount] int NOT NULL,
    [UserName] nvarchar(256) NOT NULL,
    [UserId] int IDENTITY(1,1) NOT NULL,
    [IsApproved] bit NOT NULL,
    [IsActive] bit NOT NULL,
    [UserType] nvarchar(50) NULL,
    [DistrictID] int NULL,
    [Remarks] nvarchar(100) NULL,
    [AgentID] int NULL,
    [DeviceID] nvarchar(100) NULL,
    [Name] nvarchar(256) NULL,
    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
);
GO
IF OBJECT_ID('[dbo].[City]','U') IS NULL
CREATE TABLE [dbo].[City] (
    [CityID] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(50) NOT NULL,
    [ShortCode] nvarchar(50) NULL,
    [AddedOn] datetime NOT NULL,
    [AddedBy] int NOT NULL,
    [UpdatedOn] datetime NULL,
    [UpdatedBy] int NULL,
    [IsActive] bit NOT NULL,
    [CountryID] int NOT NULL,
    CONSTRAINT [PK_City] PRIMARY KEY ([CityID])
);
GO
IF OBJECT_ID('[dbo].[Clinic]','U') IS NULL
CREATE TABLE [dbo].[Clinic] (
    [ClinicID] int IDENTITY(1,1) NOT NULL,
    [AccountID] nvarchar(50) NOT NULL,
    [Name] nvarchar(100) NULL,
    [Address] nvarchar(max) NULL,
    [City] nvarchar(50) NULL,
    [State] nvarchar(50) NULL,
    [Country] nvarchar(50) NULL,
    [ZipCode] nvarchar(50) NULL,
    [Phone] nvarchar(50) NULL,
    [Cell] nvarchar(50) NULL,
    [FaxNo] nvarchar(50) NULL,
    [Email] nvarchar(50) NULL,
    [URL] nvarchar(100) NULL,
    [AddedOn] datetime NOT NULL,
    [AddedBy] int NOT NULL,
    [UpdatedOn] datetime NULL,
    [UpdatedBy] int NULL,
    [IsActive] bit NOT NULL,
    [CP_Name] nvarchar(100) NULL,
    [CP_Phone] nvarchar(100) NULL,
    [CP_Cell] nvarchar(20) NULL,
    [CP_Address] nvarchar(120) NULL,
    [CP_Email] nvarchar(100) NULL,
    [CP_Pager] nvarchar(100) NULL,
    [SystemID] nvarchar(100) NULL,
    CONSTRAINT [PK_Clinic] PRIMARY KEY ([ClinicID])
);
GO
IF OBJECT_ID('[dbo].[ContactPerson]','U') IS NULL
CREATE TABLE [dbo].[ContactPerson] (
    [CPID] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(100) NOT NULL,
    [Phone] nvarchar(50) NULL,
    [Cell] nvarchar(50) NULL,
    [Address] nvarchar(max) NULL,
    [Email] nvarchar(50) NULL,
    [Pager] nvarchar(50) NULL,
    [AccountTypeID] int NOT NULL,
    [AccountID] int NOT NULL,
    [AddedOn] datetime NOT NULL,
    [AddedBy] int NOT NULL,
    [UpdatedOn] datetime NULL,
    [UpdatedBy] int NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_ContactPerson] PRIMARY KEY ([CPID])
);
GO
IF OBJECT_ID('[dbo].[Country]','U') IS NULL
CREATE TABLE [dbo].[Country] (
    [CountryID] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(100) NOT NULL,
    [ShortCode] nvarchar(50) NULL,
    [AddedOn] datetime NOT NULL,
    [AddedBy] int NOT NULL,
    [UpdatedOn] datetime NULL,
    [UpdatedBy] int NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_Country] PRIMARY KEY ([CountryID])
);
GO
IF OBJECT_ID('[dbo].[Dictator]','U') IS NULL
CREATE TABLE [dbo].[Dictator] (
    [DictatorID] int IDENTITY(1,1) NOT NULL,
    [AccountID] int NOT NULL,
    [SystemID] nvarchar(50) NULL,
    [LoginID] nvarchar(128) NULL,
    [Password] nvarchar(50) NULL,
    [AllClientAccess] bit NOT NULL,
    [Prefix] nvarchar(50) NULL,
    [FirstName] nvarchar(50) NULL,
    [MiddleName] nvarchar(50) NULL,
    [LastName] nvarchar(50) NULL,
    [Gender] nvarchar(10) NULL,
    [SpecialityID] int NOT NULL,
    [IsSecretary] bit NOT NULL,
    [IsFax] bit NOT NULL,
    [IsReview] bit NOT NULL,
    [AddedOn] datetime NOT NULL,
    [AddedBy] int NOT NULL,
    [UpdatedOn] datetime NULL,
    [UpdatedBy] int NULL,
    [IsActive] bit NOT NULL,
    [CP_Name] nvarchar(100) NULL,
    [CP_Phone] nvarchar(100) NULL,
    [CP_Cell] nvarchar(20) NULL,
    [CP_Address] nvarchar(120) NULL,
    [CP_Email] nvarchar(100) NULL,
    [CP_Pager] nvarchar(100) NULL,
    [SecretaryName] nvarchar(100) NULL,
    [NoOfLine] int NULL,
    [RateOfLine] decimal(18,2) NULL,
    [AddSign] bit NULL,
    CONSTRAINT [PK_Dictator] PRIMARY KEY ([DictatorID])
);
GO
IF OBJECT_ID('[dbo].[DictatorSecretary]','U') IS NULL
CREATE TABLE [dbo].[DictatorSecretary] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [DictatorId] int NULL,
    [SecretaryId] int NULL,
    CONSTRAINT [PK_DictatorSecretary] PRIMARY KEY ([Id])
);
GO
IF OBJECT_ID('[dbo].[FileFaxStatus]','U') IS NULL
CREATE TABLE [dbo].[FileFaxStatus] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [FaxID] nvarchar(100) NOT NULL,
    [UpDocID] int NOT NULL,
    [FileID] int NOT NULL,
    [Status] nvarchar(50) NULL,
    [AddedOn] datetime NOT NULL,
    [ToNumber] nvarchar(50) NULL,
    [DateCreated] nvarchar(50) NULL,
    [PageCount] nvarchar(50) NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_FileFaxStatus] PRIMARY KEY ([ID])
);
GO
IF OBJECT_ID('[dbo].[FileLog]','U') IS NULL
CREATE TABLE [dbo].[FileLog] (
    [LogID] int IDENTITY(1,1) NOT NULL,
    [FileID] int NOT NULL,
    [LogDate] datetime NULL,
    [LogTypeID] int NULL,
    [UserID] int NOT NULL,
    CONSTRAINT [PK_FileLog] PRIMARY KEY ([LogID])
);
GO
IF OBJECT_ID('[dbo].[LogType]','U') IS NULL
CREATE TABLE [dbo].[LogType] (
    [LogTypeID] int IDENTITY(1,1) NOT NULL,
    [LogTypeTitle] nvarchar(50) NOT NULL,
    CONSTRAINT [PK_LogType] PRIMARY KEY ([LogTypeID])
);
GO
IF OBJECT_ID('[dbo].[Manager]','U') IS NULL
CREATE TABLE [dbo].[Manager] (
    [ManagerID] int IDENTITY(1,1) NOT NULL,
    [AccountID] int NOT NULL,
    [Username] nvarchar(500) NOT NULL,
    [Password] nvarchar(500) NOT NULL,
    [AddedOn] datetime NULL,
    [AddedBy] int NULL,
    [UpdatedOn] datetime NULL,
    [UpdatedBy] int NULL,
    [isActive] bit NOT NULL,
    CONSTRAINT [PK_Manager] PRIMARY KEY ([ManagerID])
);
GO
IF OBJECT_ID('[dbo].[ManagerClinic]','U') IS NULL
CREATE TABLE [dbo].[ManagerClinic] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [ManagerID] int NOT NULL,
    [ClinicID] int NOT NULL,
    [DictatorID] int NOT NULL,
    CONSTRAINT [PK_ManagerClinic] PRIMARY KEY ([Id])
);
GO
IF OBJECT_ID('[dbo].[Modality]','U') IS NULL
CREATE TABLE [dbo].[Modality] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [ModalityName] varchar(255) NULL,
    [AddedOn] datetime NOT NULL,
    [AddedBy] int NOT NULL,
    [UpdatedOn] datetime NULL,
    [UpdatedBy] int NULL,
    CONSTRAINT [PK_Modality] PRIMARY KEY ([Id])
);
GO
IF OBJECT_ID('[dbo].[Secretary]','U') IS NULL
CREATE TABLE [dbo].[Secretary] (
    [SecretaryID] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(150) NOT NULL,
    [ShortCode] nvarchar(50) NULL,
    [AddedOn] datetime NOT NULL,
    [AddedBy] int NOT NULL,
    [UpdatedOn] datetime NULL,
    [UpdatedBy] int NULL,
    [IsActive] bit NOT NULL,
    [DictatorID] int NULL,
    CONSTRAINT [PK_Secretary] PRIMARY KEY ([SecretaryID])
);
GO
IF OBJECT_ID('[dbo].[Speciality]','U') IS NULL
CREATE TABLE [dbo].[Speciality] (
    [SpecialityID] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(200) NOT NULL,
    [AddedOn] datetime NOT NULL,
    [AddedBy] int NOT NULL,
    [UpdatedOn] datetime NULL,
    [UpdatedBy] int NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_Speciality] PRIMARY KEY ([SpecialityID])
);
GO
IF OBJECT_ID('[dbo].[Status]','U') IS NULL
CREATE TABLE [dbo].[Status] (
    [Id] int NOT NULL,
    [Name] varchar(255) NULL,
    [Color] varchar(255) NULL,
    CONSTRAINT [PK_Status] PRIMARY KEY ([Id])
);
GO
IF OBJECT_ID('[dbo].[Typist]','U') IS NULL
CREATE TABLE [dbo].[Typist] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [TypistName] nvarchar(100) NULL,
    [AddedOn] datetime NOT NULL,
    [AddedBy] int NOT NULL,
    [UpdatedOn] datetime NOT NULL,
    [UpdatedBy] int NOT NULL,
    CONSTRAINT [PK_Typist] PRIMARY KEY ([Id])
);
GO
IF OBJECT_ID('[dbo].[UploadedDocument]','U') IS NULL
CREATE TABLE [dbo].[UploadedDocument] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [FileID] int NULL,
    [FileName] nvarchar(400) NULL,
    [FilePath] nvarchar(800) NULL,
    [Extension] nvarchar(50) NULL,
    [Size] nvarchar(100) NULL,
    [Decription] nvarchar(max) NULL,
    [WordCount] int NULL,
    [IsApproved] bit NULL,
    [StatusId] int NULL,
    [UploadBy] int NOT NULL,
    [UploadOn] datetime NOT NULL,
    [Duration] nvarchar(50) NULL,
    [DocChildID] int NULL,
    [CharCount] int NULL,
    [CharCountWithSpace] int NULL,
    [LineCount] decimal(18,2) NULL,
    [PatientName] varchar(200) NULL,
    [ModalityId] int NULL,
    [MRN] varchar(150) NULL,
    [DOB] varchar(50) NULL,
    [DOS] varchar(50) NULL,
    [RefDoctor] varchar(255) NULL,
    [FaxNumber] varchar(200) NULL,
    CONSTRAINT [PK_UploadedDocument] PRIMARY KEY ([Id])
);
GO
IF OBJECT_ID('[dbo].[UploadedFile]','U') IS NULL
CREATE TABLE [dbo].[UploadedFile] (
    [FileID] int IDENTITY(1,1) NOT NULL,
    [FileName] nvarchar(200) NOT NULL,
    [FilePath] nvarchar(400) NULL,
    [Extension] nvarchar(50) NULL,
    [Size] nvarchar(50) NULL,
    [DictatorID] int NULL,
    [ClinicID] int NULL,
    [UploadBy] int NOT NULL,
    [UploadOn] datetime NOT NULL,
    [IsActive] bit NOT NULL,
    [IsDownloaded] bit NOT NULL,
    [StatusId] int NULL,
    [Duration] nvarchar(50) NULL,
    [TypistId] int NULL,
    [TypistAssignID] int NULL,
    [StackMark] int NULL,
    CONSTRAINT [PK_UploadedFile] PRIMARY KEY ([FileID])
);
GO
IF OBJECT_ID('[dbo].[UserLog]','U') IS NULL
CREATE TABLE [dbo].[UserLog] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [AudioID] int NULL,
    [DocID] int NULL,
    [FileName] nvarchar(300) NULL,
    [FileType] nvarchar(50) NULL,
    [Action] int NULL,
    [UserLogName] nvarchar(150) NULL,
    [Date] datetime NULL,
    [UserLogId] int NULL,
    [StatusId] int NULL,
    [ProviderId] int NULL,
    CONSTRAINT [PK_UserLog] PRIMARY KEY ([Id])
);
GO

-- ===================== schema_procs.sql =====================
GO
-- ============================================================================
-- Reconstructed stored procedures for Excel on Work.
-- Column lists match the EF *_Result types EXACTLY (EF maps by column name and
-- throws if an expected column is missing). Proc bodies are best-effort
-- reconstructions (original bodies were not recoverable).
-- ============================================================================

IF OBJECT_ID('dbo.STRINGSplit_Fun','IF') IS NOT NULL DROP FUNCTION dbo.STRINGSplit_Fun;
GO
CREATE FUNCTION dbo.STRINGSplit_Fun (@pString nvarchar(max), @pDelimiter char(1))
RETURNS TABLE AS RETURN
(
    SELECT LTRIM(RTRIM(value)) AS Item
    FROM STRING_SPLIT(ISNULL(@pString,''), @pDelimiter)
    WHERE LTRIM(RTRIM(value)) <> ''
);
GO

-- ---- stp_GetTypist : (UserId, UserName) ------------------------------------
IF OBJECT_ID('dbo.stp_GetTypist','P') IS NOT NULL DROP PROCEDURE dbo.stp_GetTypist;
GO
CREATE PROCEDURE dbo.stp_GetTypist AS
BEGIN
    SET NOCOUNT ON;
    SELECT u.UserId AS UserId, u.UserName AS UserName
    FROM AspNetUsers u
    INNER JOIN AspNetUserRoles ur ON ur.UserId = u.Id
    INNER JOIN AspNetRoles r ON r.Id = ur.RoleId
    WHERE r.Name = 'Typist'
    ORDER BY u.UserName;
END
GO

-- ---- stp_GetDocuments : document list --------------------------------------
IF OBJECT_ID('dbo.stp_GetDocuments','P') IS NOT NULL DROP PROCEDURE dbo.stp_GetDocuments;
GO
CREATE PROCEDURE dbo.stp_GetDocuments
    @DictatorIDs int = NULL, @ClinicID int = NULL, @TypistID int = NULL,
    @StatusId int = NULL, @FromDate datetime = NULL, @ToDate datetime = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        uf.FileName AS AudioName, s.Name AS Status, uf.TypistAssignID AS TypistAssignID,
        (ISNULL(d.FirstName,'') + ' ' + ISNULL(d.LastName,'')) AS Provider, c.Name AS ClinicName,
        ud.ModalityId AS ModalityId, ISNULL(uf.FileID,0) AS AudioFileID, ud.FileName AS FileName,
        ud.FilePath AS FilePath, ud.Id AS Id, ud.PatientName AS PatientName,
        ud.DOS AS DOS, ud.DOB AS DOB, ud.MRN AS MRN,
        ISNULL(ud.UploadOn, uf.UploadOn) AS UploadOn, ud.UploadBy AS UploadBy
    FROM UploadedDocument ud
    LEFT JOIN UploadedFile uf ON ud.FileID = uf.FileID
    LEFT JOIN Dictator d ON uf.DictatorID = d.DictatorID
    LEFT JOIN Clinic c ON d.AccountID = c.ClinicID
    LEFT JOIN Status s ON ud.StatusId = s.Id
    WHERE (@DictatorIDs IS NULL OR uf.DictatorID = @DictatorIDs)
      AND (@ClinicID IS NULL OR d.AccountID = @ClinicID)
      AND (@TypistID IS NULL OR uf.TypistAssignID = @TypistID)
      AND (@StatusId IS NULL OR ud.StatusId = @StatusId)
      AND (@FromDate IS NULL OR uf.UploadOn >= @FromDate)
      AND (@ToDate   IS NULL OR uf.UploadOn <= @ToDate)
    ORDER BY ud.Id DESC;
END
GO

-- ---- stp_GetDictations : dictation list ------------------------------------
IF OBJECT_ID('dbo.stp_GetDictations','P') IS NOT NULL DROP PROCEDURE dbo.stp_GetDictations;
GO
CREATE PROCEDURE dbo.stp_GetDictations
    @DictatorIDs nvarchar(max) = NULL, @ClinicIDs nvarchar(max) = NULL,
    @TypistID int = NULL, @StatusId int = NULL, @FromDate datetime = NULL, @ToDate datetime = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        uf.TypistAssignID AS TypistAssignID, uf.UploadBy AS UploadBy, uf.FilePath AS FilePath,
        uf.Duration AS Duration, uf.FileID AS FileID, uf.FileName AS FileName, uf.UploadOn AS UploadOn,
        uf.DictatorID AS DictatorID, ISNULL(uf.IsDownloaded,0) AS isDownloaded,
        (ISNULL(d.FirstName,'') + ' ' + ISNULL(d.LastName,'')) AS Provider, c.Name AS ClinicName
    FROM UploadedFile uf
    LEFT JOIN Dictator d ON uf.DictatorID = d.DictatorID
    LEFT JOIN Clinic c ON d.AccountID = c.ClinicID
    WHERE ISNULL(uf.IsActive,1) = 1
      AND (@DictatorIDs IS NULL OR @DictatorIDs='' OR uf.DictatorID IN (SELECT TRY_CONVERT(int,Item) FROM dbo.STRINGSplit_Fun(@DictatorIDs,',')))
      AND (@ClinicIDs   IS NULL OR @ClinicIDs=''   OR d.AccountID  IN (SELECT TRY_CONVERT(int,Item) FROM dbo.STRINGSplit_Fun(@ClinicIDs,',')))
      AND (@TypistID    IS NULL OR uf.TypistAssignID = @TypistID)
      AND (@StatusId    IS NULL OR uf.StatusId = @StatusId)
      AND (@FromDate    IS NULL OR uf.UploadOn >= @FromDate)
      AND (@ToDate      IS NULL OR uf.UploadOn <= @ToDate)
    ORDER BY uf.FileID DESC;
END
GO

-- ---- stp_GetDocumentsDataNew : Home grid (DOT/DOS/DOB ranges) ----------------
IF OBJECT_ID('dbo.stp_GetDocumentsDataNew','P') IS NOT NULL DROP PROCEDURE dbo.stp_GetDocumentsDataNew;
GO
CREATE PROCEDURE dbo.stp_GetDocumentsDataNew
    @DictatorIDs nvarchar(max) = NULL, @ClinicIDs nvarchar(max) = NULL,
    @TypistID int = NULL, @StatusId int = NULL,
    @DotFromDate datetime = NULL, @DotToDate datetime = NULL,
    @DosFromDate datetime = NULL, @DosToDate datetime = NULL,
    @DoDFromDate datetime = NULL, @DoDToDate datetime = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        uf.FileID AS AudioID, s.Name AS Status, c.Name AS Clinic,
        (ISNULL(d.FirstName,'') + ' ' + ISNULL(d.LastName,'')) AS Provider,
        uf.FileName AS AudioFile, ud.FileName AS DocumentName, uf.Duration AS Duration,
        (SELECT COUNT(1) FROM UploadedDocument x WHERE x.FileID = uf.FileID) AS DocCounts,
        s.Color AS StatusColor, uf.TypistAssignID AS TypistAssignID, ISNULL(uf.StackMark,0) AS StackMark,
        ISNULL(uf.IsActive,1) AS IsActive, uf.UploadOn AS DOT, m.ModalityName AS ModalityName,
        ud.Id AS DocumentId, ud.PatientName AS PatientName, ud.MRN AS MRN, ud.DOS AS DOS, ud.DOB AS DOB,
        ud.RefDoctor AS RefDoctor, ud.FaxNumber AS FaxNumber, ud.WordCount AS WordCount,
        ud.CharCount AS CharCount, ud.CharCountWithSpace AS CharCountWithSpace
    FROM UploadedFile uf
    LEFT JOIN Dictator d ON uf.DictatorID = d.DictatorID
    LEFT JOIN Clinic c ON d.AccountID = c.ClinicID
    LEFT JOIN Status s ON uf.StatusId = s.Id
    OUTER APPLY (SELECT TOP 1 * FROM UploadedDocument ud2 WHERE ud2.FileID = uf.FileID ORDER BY ud2.Id DESC) ud
    LEFT JOIN Modality m ON ud.ModalityId = m.Id
    WHERE ISNULL(uf.IsActive,1) = 1
      AND (@DictatorIDs IS NULL OR @DictatorIDs='' OR uf.DictatorID IN (SELECT TRY_CONVERT(int,Item) FROM dbo.STRINGSplit_Fun(@DictatorIDs,',')))
      AND (@ClinicIDs   IS NULL OR @ClinicIDs=''   OR d.AccountID  IN (SELECT TRY_CONVERT(int,Item) FROM dbo.STRINGSplit_Fun(@ClinicIDs,',')))
      AND (@TypistID    IS NULL OR uf.TypistAssignID = @TypistID)
      AND (@StatusId    IS NULL OR uf.StatusId = @StatusId)
      AND (@DotFromDate IS NULL OR uf.UploadOn >= @DotFromDate)
      AND (@DotToDate   IS NULL OR uf.UploadOn <= @DotToDate)
      AND (@DosFromDate IS NULL OR TRY_CONVERT(datetime, ud.DOS) >= @DosFromDate)
      AND (@DosToDate   IS NULL OR TRY_CONVERT(datetime, ud.DOS) <= @DosToDate)
      AND (@DoDFromDate IS NULL OR TRY_CONVERT(datetime, ud.DOB) >= @DoDFromDate)
      AND (@DoDToDate   IS NULL OR TRY_CONVERT(datetime, ud.DOB) <= @DoDToDate)
    ORDER BY ISNULL(uf.StackMark,0) DESC, uf.FileID DESC;
END
GO

-- ---- stp_GetDocumentsData : same shape, single DOB naming -------------------
IF OBJECT_ID('dbo.stp_GetDocumentsData','P') IS NOT NULL DROP PROCEDURE dbo.stp_GetDocumentsData;
GO
CREATE PROCEDURE dbo.stp_GetDocumentsData
    @DictatorID int = NULL, @ClinicID int = NULL, @TypistID int = NULL, @StatusId int = NULL,
    @DotFromDate datetime = NULL, @DotToDate datetime = NULL,
    @DosFromDate datetime = NULL, @DosToDate datetime = NULL,
    @DobFromDate datetime = NULL, @DobToDate datetime = NULL
AS
BEGIN
    SET NOCOUNT ON;
    EXEC dbo.stp_GetDocumentsDataNew @DictatorIDs=NULL, @ClinicIDs=NULL, @TypistID=@TypistID, @StatusId=@StatusId,
        @DotFromDate=@DotFromDate, @DotToDate=@DotToDate, @DosFromDate=@DosFromDate, @DosToDate=@DosToDate,
        @DoDFromDate=@DobFromDate, @DoDToDate=@DobToDate;
END
GO

-- ---- GetFaxStatusFilterWise : FAX page (+ NEW DOT/DOS/DOB params) ------------
IF OBJECT_ID('dbo.GetFaxStatusFilterWise','P') IS NOT NULL DROP PROCEDURE dbo.GetFaxStatusFilterWise;
GO
CREATE PROCEDURE dbo.GetFaxStatusFilterWise
    @ClinicIDs nvarchar(max) = NULL, @fromDate datetime = NULL, @toDate datetime = NULL,
    @ProviderIDs nvarchar(max) = NULL, @TypistID int = NULL, @FileName nvarchar(50) = NULL,
    @DotFromDate datetime = NULL, @DotToDate datetime = NULL,
    @DosFromDate datetime = NULL, @DosToDate datetime = NULL,
    @DobFromDate datetime = NULL, @DobToDate datetime = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        c.Name AS Clinic, (ISNULL(d.FirstName,'') + ' ' + ISNULL(d.LastName,'')) AS Provider,
        ud.PatientName AS PatientName, ud.FileName AS Document, ud.MRN AS MRN,
        uf.TypistAssignID AS TypistAssignedID, ffs.ID AS ID, ffs.FaxID AS FaxID, ffs.UpDocID AS UpDocID,
        ffs.FileID AS FileID, ffs.Status AS Status, ffs.AddedOn AS AddedOn, ffs.ToNumber AS ToNumber,
        ffs.DateCreated AS DateCreated, ffs.PageCount AS PageCount, ffs.IsActive AS IsActive
    FROM FileFaxStatus ffs
    LEFT JOIN UploadedDocument ud ON ffs.UpDocID = ud.Id
    LEFT JOIN UploadedFile uf ON ffs.FileID = uf.FileID
    LEFT JOIN Dictator d ON uf.DictatorID = d.DictatorID
    LEFT JOIN Clinic c ON d.AccountID = c.ClinicID
    WHERE ISNULL(ffs.IsActive,1) = 1
      AND (@ClinicIDs   IS NULL OR @ClinicIDs=''   OR d.AccountID  IN (SELECT TRY_CONVERT(int,Item) FROM dbo.STRINGSplit_Fun(@ClinicIDs,',')))
      AND (@ProviderIDs IS NULL OR @ProviderIDs='' OR uf.DictatorID IN (SELECT TRY_CONVERT(int,Item) FROM dbo.STRINGSplit_Fun(@ProviderIDs,',')))
      AND (@TypistID    IS NULL OR uf.TypistAssignID = @TypistID)
      AND (@FileName    IS NULL OR @FileName='' OR ffs.Status = @FileName)
      AND (@fromDate    IS NULL OR ffs.AddedOn >= @fromDate)
      AND (@toDate      IS NULL OR ffs.AddedOn <= @toDate)
      AND (@DotFromDate IS NULL OR uf.UploadOn >= @DotFromDate)
      AND (@DotToDate   IS NULL OR uf.UploadOn <= @DotToDate)
      AND (@DosFromDate IS NULL OR TRY_CONVERT(datetime, ud.DOS) >= @DosFromDate)
      AND (@DosToDate   IS NULL OR TRY_CONVERT(datetime, ud.DOS) <= @DosToDate)
      AND (@DobFromDate IS NULL OR TRY_CONVERT(datetime, ud.DOB) >= @DobFromDate)
      AND (@DobToDate   IS NULL OR TRY_CONVERT(datetime, ud.DOB) <= @DobToDate)
    ORDER BY ffs.ID DESC;
END
GO

-- ---- GetManagers ------------------------------------------------------------
IF OBJECT_ID('dbo.GetManagers','P') IS NOT NULL DROP PROCEDURE dbo.GetManagers;
GO
CREATE PROCEDURE dbo.GetManagers @ManagerID int = NULL AS
BEGIN
    SET NOCOUNT ON;
    SELECT m.ManagerID AS ManagerID, m.Username AS Username, m.Password AS Password, m.isActive AS isActive,
        m.AddedOn AS AddedOn, m.AccountID AS AccountID,
        (ISNULL(d.FirstName,'') + ' ' + ISNULL(d.LastName,'')) AS DictatorName,
        ISNULL(mc.DictatorID,0) AS DictatorID, c.Name AS ClinicName, ISNULL(mc.ClinicID,0) AS ClinicID
    FROM Manager m
    LEFT JOIN ManagerClinic mc ON mc.ManagerID = m.ManagerID
    LEFT JOIN Dictator d ON mc.DictatorID = d.DictatorID
    LEFT JOIN Clinic c ON mc.ClinicID = c.ClinicID
    WHERE (@ManagerID IS NULL OR m.ManagerID = @ManagerID);
END
GO

-- ---- stp_GetActivityLogs ----------------------------------------------------
IF OBJECT_ID('dbo.stp_GetActivityLogs','P') IS NOT NULL DROP PROCEDURE dbo.stp_GetActivityLogs;
GO
CREATE PROCEDURE dbo.stp_GetActivityLogs
    @DictatorID int = NULL, @StatusId int = NULL, @FromDate datetime = NULL, @ToDate datetime = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        uf.FileID AS AudioID, CAST(NULL AS int) AS Action, s.Name AS Status,
        (ISNULL(d.FirstName,'') + ' ' + ISNULL(d.LastName,'')) AS Provider,
        uf.UploadOn AS ActivityDate, uf.FileName AS AudioFile, uf.Duration AS Duration, s.Color AS StatusColor
    FROM UploadedFile uf
    LEFT JOIN Dictator d ON uf.DictatorID = d.DictatorID
    LEFT JOIN Status s ON uf.StatusId = s.Id
    WHERE (@DictatorID IS NULL OR uf.DictatorID = @DictatorID)
      AND (@StatusId   IS NULL OR uf.StatusId = @StatusId)
      AND (@FromDate   IS NULL OR uf.UploadOn >= @FromDate)
      AND (@ToDate     IS NULL OR uf.UploadOn <= @ToDate)
    ORDER BY uf.FileID DESC;
END
GO

-- ---- stp_GetUploadedAudio ---------------------------------------------------
IF OBJECT_ID('dbo.stp_GetUploadedAudio','P') IS NOT NULL DROP PROCEDURE dbo.stp_GetUploadedAudio;
GO
CREATE PROCEDURE dbo.stp_GetUploadedAudio
    @DictatorID int = NULL, @ClinicID int = NULL, @StatusId int = NULL,
    @FromDate datetime = NULL, @ToDate datetime = NULL, @TypistAssignId int = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        uf.FileID AS AudioID, s.Name AS Status, c.Name AS Clinic,
        (ISNULL(d.FirstName,'') + ' ' + ISNULL(d.LastName,'')) AS Provider, uf.UploadOn AS Uploaded,
        uf.FileName AS AudioFile, uf.Duration AS Duration,
        (SELECT COUNT(1) FROM UploadedDocument x WHERE x.FileID = uf.FileID) AS DocCounts,
        uf.UploadBy AS UploadBy, s.Color AS StatusColor, uf.TypistAssignID AS TypistAssignID,
        ISNULL(uf.StackMark,0) AS StackMark, ud.PatientName AS PatientName, m.ModalityName AS ModalityName,
        ud.MRN AS MRN, ud.DOS AS DOS, ud.DOB AS DOB, ud.RefDoctor AS RefDoctor, ud.FaxNumber AS FaxNumber
    FROM UploadedFile uf
    LEFT JOIN Dictator d ON uf.DictatorID = d.DictatorID
    LEFT JOIN Clinic c ON d.AccountID = c.ClinicID
    LEFT JOIN Status s ON uf.StatusId = s.Id
    OUTER APPLY (SELECT TOP 1 * FROM UploadedDocument ud2 WHERE ud2.FileID = uf.FileID ORDER BY ud2.Id DESC) ud
    LEFT JOIN Modality m ON ud.ModalityId = m.Id
    WHERE ISNULL(uf.IsActive,1) = 1
      AND (@DictatorID IS NULL OR uf.DictatorID = @DictatorID)
      AND (@ClinicID   IS NULL OR d.AccountID = @ClinicID)
      AND (@StatusId   IS NULL OR uf.StatusId = @StatusId)
      AND (@TypistAssignId IS NULL OR uf.TypistAssignID = @TypistAssignId)
      AND (@FromDate   IS NULL OR uf.UploadOn >= @FromDate)
      AND (@ToDate     IS NULL OR uf.UploadOn <= @ToDate)
    ORDER BY uf.FileID DESC;
END
GO

-- ---- stp_GetUploadedAudioInfo (superset + IsActive/DocumentName/counts) ------
IF OBJECT_ID('dbo.stp_GetUploadedAudioInfo','P') IS NOT NULL DROP PROCEDURE dbo.stp_GetUploadedAudioInfo;
GO
CREATE PROCEDURE dbo.stp_GetUploadedAudioInfo
    @DictatorID int = NULL, @ClinicID int = NULL, @StatusId int = NULL,
    @FromDate datetime = NULL, @ToDate datetime = NULL, @TypistAssignId int = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        uf.FileID AS AudioID, s.Name AS Status, c.Name AS Clinic,
        (ISNULL(d.FirstName,'') + ' ' + ISNULL(d.LastName,'')) AS Provider, uf.UploadOn AS Uploaded,
        uf.FileName AS AudioFile, uf.Duration AS Duration,
        (SELECT COUNT(1) FROM UploadedDocument x WHERE x.FileID = uf.FileID) AS DocCounts,
        uf.UploadBy AS UploadBy, s.Color AS StatusColor, uf.TypistAssignID AS TypistAssignID,
        ISNULL(uf.StackMark,0) AS StackMark, ISNULL(uf.IsActive,1) AS IsActive, m.ModalityName AS ModalityName,
        ud.PatientName AS PatientName, ud.MRN AS MRN, ud.DOS AS DOS, ud.DOB AS DOB,
        ud.RefDoctor AS RefDoctor, ud.FaxNumber AS FaxNumber, ud.FileName AS DocumentName,
        ud.WordCount AS WordCount, ud.CharCount AS CharCount, ud.CharCountWithSpace AS CharCountWithSpace
    FROM UploadedFile uf
    LEFT JOIN Dictator d ON uf.DictatorID = d.DictatorID
    LEFT JOIN Clinic c ON d.AccountID = c.ClinicID
    LEFT JOIN Status s ON uf.StatusId = s.Id
    OUTER APPLY (SELECT TOP 1 * FROM UploadedDocument ud2 WHERE ud2.FileID = uf.FileID ORDER BY ud2.Id DESC) ud
    LEFT JOIN Modality m ON ud.ModalityId = m.Id
    WHERE ISNULL(uf.IsActive,1) = 1
      AND (@DictatorID IS NULL OR uf.DictatorID = @DictatorID)
      AND (@ClinicID   IS NULL OR d.AccountID = @ClinicID)
      AND (@StatusId   IS NULL OR uf.StatusId = @StatusId)
      AND (@TypistAssignId IS NULL OR uf.TypistAssignID = @TypistAssignId)
      AND (@FromDate   IS NULL OR uf.UploadOn >= @FromDate)
      AND (@ToDate     IS NULL OR uf.UploadOn <= @ToDate)
    ORDER BY uf.FileID DESC;
END
GO

-- ---- stp_DocumentReport -----------------------------------------------------
IF OBJECT_ID('dbo.stp_DocumentReport','P') IS NOT NULL DROP PROCEDURE dbo.stp_DocumentReport;
GO
CREATE PROCEDURE dbo.stp_DocumentReport
    @DictatorID int = NULL, @ClinicID int = NULL, @StatusId int = NULL,
    @FromDate datetime = NULL, @ToDate datetime = NULL, @TypistAssignId int = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        uf.DictatorID AS ProviderId, (ISNULL(d.FirstName,'') + ' ' + ISNULL(d.LastName,'')) AS Provider,
        c.Name AS Clinic, uf.FileName AS AudioFile, ud.FileName AS DocFile, ud.Extension AS Extension,
        ud.Size AS Size, ud.WordCount AS WordCount, ud.CharCount AS CharacterCount, ud.LineCount AS LineCount,
        CAST(uf.UploadBy AS nvarchar(50)) AS UploadedBy, ISNULL(ud.UploadOn, uf.UploadOn) AS UploadOn,
        s.Name AS Status, CAST(uf.TypistAssignID AS nvarchar(50)) AS TypistAssign,
        ud.CharCountWithSpace AS NoOfCharacterPerLine, CAST(NULL AS decimal(18,2)) AS RatePerLine
    FROM UploadedDocument ud
    LEFT JOIN UploadedFile uf ON ud.FileID = uf.FileID
    LEFT JOIN Dictator d ON uf.DictatorID = d.DictatorID
    LEFT JOIN Clinic c ON d.AccountID = c.ClinicID
    LEFT JOIN Status s ON ud.StatusId = s.Id
    WHERE (@DictatorID IS NULL OR uf.DictatorID = @DictatorID)
      AND (@ClinicID   IS NULL OR d.AccountID = @ClinicID)
      AND (@StatusId   IS NULL OR ud.StatusId = @StatusId)
      AND (@TypistAssignId IS NULL OR uf.TypistAssignID = @TypistAssignId)
      AND (@FromDate   IS NULL OR uf.UploadOn >= @FromDate)
      AND (@ToDate     IS NULL OR uf.UploadOn <= @ToDate)
    ORDER BY ud.Id DESC;
END
GO

-- ---- stp_GetUserLog ---------------------------------------------------------
IF OBJECT_ID('dbo.stp_GetUserLog','P') IS NOT NULL DROP PROCEDURE dbo.stp_GetUserLog;
GO
CREATE PROCEDURE dbo.stp_GetUserLog @AudioID int = NULL AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        l.AudioID AS AudioID, l.FileName AS FileName, l.FileType AS FileType, l.Action AS Action,
        l.UserLogName AS UserLogName, l.Date AS Date, l.UserLogId AS UserLogId,
        CAST(NULL AS nvarchar(256)) AS Name, CAST(NULL AS varchar(255)) AS Color,
        CAST(NULL AS nvarchar(100)) AS Provider
    FROM UserLog l
    WHERE (@AudioID IS NULL OR l.AudioID = @AudioID)
    ORDER BY l.Date DESC;
END
GO

-- ---- stp_GetUserLogByDocumentId (same shape) --------------------------------
IF OBJECT_ID('dbo.stp_GetUserLogByDocumentId','P') IS NOT NULL DROP PROCEDURE dbo.stp_GetUserLogByDocumentId;
GO
CREATE PROCEDURE dbo.stp_GetUserLogByDocumentId @DocID int = NULL AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        l.AudioID AS AudioID, l.FileName AS FileName, l.FileType AS FileType, l.Action AS Action,
        l.UserLogName AS UserLogName, l.Date AS Date, l.UserLogId AS UserLogId,
        CAST(NULL AS nvarchar(256)) AS Name, CAST(NULL AS varchar(255)) AS Color,
        CAST(NULL AS nvarchar(100)) AS Provider
    FROM UserLog l
    WHERE (@DocID IS NULL OR l.DocID = @DocID)
    ORDER BY l.Date DESC;
END
GO

-- ---- Proc_InsertErrorDetails : logging stub ---------------------------------
IF OBJECT_ID('dbo.Proc_InsertErrorDetails','P') IS NOT NULL DROP PROCEDURE dbo.Proc_InsertErrorDetails;
GO
CREATE PROCEDURE dbo.Proc_InsertErrorDetails AS BEGIN SET NOCOUNT ON; RETURN 0; END
GO

-- ---- stp_Dictator : insert/update a provider --------------------------------
IF OBJECT_ID('dbo.stp_Dictator','P') IS NOT NULL DROP PROCEDURE dbo.stp_Dictator;
GO
CREATE PROCEDURE dbo.stp_Dictator
    @Id int = 0, @AccountID int = NULL, @SystemID nvarchar(50) = NULL, @LoginID nvarchar(128) = NULL,
    @Password nvarchar(50) = NULL, @AllClientAccess bit = 0, @Prefix nvarchar(50) = NULL,
    @FirstName nvarchar(50) = NULL, @MiddleName nvarchar(50) = NULL, @LastName nvarchar(50) = NULL,
    @Gender nvarchar(10) = NULL, @SpecialityID int = NULL, @IsSecretary bit = 0, @IsFax bit = 0,
    @IsReview bit = 0, @AddedOn datetime = NULL, @AddedBy int = NULL, @UpdatedOn datetime = NULL,
    @UpdatedBy int = NULL, @IsActive bit = 1, @CP_Name nvarchar(100) = NULL, @CP_Phone nvarchar(100) = NULL,
    @CP_Cell nvarchar(100) = NULL, @CP_Address nvarchar(120) = NULL, @CP_Email nvarchar(100) = NULL,
    @CP_Pager nvarchar(100) = NULL, @SecretaryName nvarchar(100) = NULL, @NoOfLine int = NULL,
    @RateOfLine decimal(18,2) = NULL, @AddSign bit = 0
AS
BEGIN
    SET NOCOUNT ON;
    IF (ISNULL(@Id,0) = 0)
    BEGIN
        INSERT INTO Dictator (AccountID, SystemID, LoginID, Password, AllClientAccess, Prefix,
            FirstName, MiddleName, LastName, Gender, SpecialityID, IsSecretary, IsFax, IsReview,
            AddedOn, AddedBy, UpdatedOn, UpdatedBy, IsActive, AddSign)
        VALUES (@AccountID, @SystemID, @LoginID, @Password, @AllClientAccess, @Prefix,
            @FirstName, @MiddleName, @LastName, @Gender, @SpecialityID, @IsSecretary, @IsFax, @IsReview,
            ISNULL(@AddedOn, GETDATE()), @AddedBy, @UpdatedOn, @UpdatedBy, @IsActive, @AddSign);
        SELECT SCOPE_IDENTITY() AS DictatorID;
    END
    ELSE
    BEGIN
        UPDATE Dictator SET AccountID=@AccountID, SystemID=@SystemID, LoginID=@LoginID, Password=@Password,
            AllClientAccess=@AllClientAccess, Prefix=@Prefix, FirstName=@FirstName, MiddleName=@MiddleName,
            LastName=@LastName, Gender=@Gender, SpecialityID=@SpecialityID, IsSecretary=@IsSecretary,
            IsFax=@IsFax, IsReview=@IsReview, UpdatedOn=ISNULL(@UpdatedOn, GETDATE()), UpdatedBy=@UpdatedBy,
            IsActive=@IsActive, AddSign=@AddSign
        WHERE DictatorID=@Id;
        SELECT @Id AS DictatorID;
    END
END
GO
PRINT 'Stored procedures created.';
GO

-- ===================== seed.sql =====================
GO
SET NOCOUNT ON;

-- ---- idempotent cleanup (safe re-run) ----
DELETE FROM FileFaxStatus;
DELETE FROM UploadedDocument;
DELETE FROM UploadedFile;
DELETE FROM Dictator;
DELETE FROM Clinic;
DELETE FROM Modality;
DELETE FROM Speciality;
DELETE FROM AccountType;
DELETE FROM Status;
DELETE FROM AspNetUserRoles;
DELETE FROM AspNetUsers;
DELETE FROM AspNetRoles;

-- ---- Roles ----
INSERT INTO AspNetRoles (Id, Name) VALUES
 (NEWID(),'SuperAdmin'),(NEWID(),'Admin'),(NEWID(),'Provider'),(NEWID(),'Manager'),(NEWID(),'Typist');

-- ---- Admin user (password = Admin@123) ----
DECLARE @adminId nvarchar(128) = NEWID();
SET IDENTITY_INSERT AspNetUsers ON;
INSERT INTO AspNetUsers (Id, Email, EmailConfirmed, PasswordHash, SecurityStamp, PhoneNumberConfirmed,
    TwoFactorEnabled, LockoutEnabled, AccessFailedCount, UserName, UserId, IsApproved, IsActive, UserType, Name)
VALUES (@adminId, 'admin@local', 1,
    'AMVY6XOlD+au+xrUCXOJfThoF9khgOBdikHSeKSyE7KNGIRnqsa3tpbPurHXaEpgnA==',
    NEWID(), 0, 0, 1, 0, 'admin', 1, 1, 1, 'Admin', 'Administrator');
-- ---- Typist user (password = Admin@123) ----
DECLARE @typistId nvarchar(128) = NEWID();
INSERT INTO AspNetUsers (Id, Email, EmailConfirmed, PasswordHash, SecurityStamp, PhoneNumberConfirmed,
    TwoFactorEnabled, LockoutEnabled, AccessFailedCount, UserName, UserId, IsApproved, IsActive, UserType, Name)
VALUES (@typistId, 'typist@local', 1,
    'AMVY6XOlD+au+xrUCXOJfThoF9khgOBdikHSeKSyE7KNGIRnqsa3tpbPurHXaEpgnA==',
    NEWID(), 0, 0, 1, 0, 'typist1', 2, 1, 1, 'Typist', 'Typist One');
SET IDENTITY_INSERT AspNetUsers OFF;
INSERT INTO AspNetUserRoles (UserId, RoleId)
    SELECT @adminId, Id FROM AspNetRoles WHERE Name IN ('Admin','SuperAdmin');
INSERT INTO AspNetUserRoles (UserId, RoleId)
    SELECT @typistId, Id FROM AspNetRoles WHERE Name = 'Typist';

-- ---- Status lookup (StatusId 5 = Approved, matches controller logic) ----
INSERT INTO Status (Id, Name, Color) VALUES
 (1,'Uploaded','#6c757d'),(2,'Assigned','#0d6efd'),(3,'Typed','#fd7e14'),
 (4,'In Review','#ffc107'),(5,'Approved','#198754'),(6,'Faxed','#20c997');

-- ---- AccountType ----
INSERT INTO AccountType (AccountTypeID, Title) VALUES (1,'Clinic');

-- ---- Speciality ----
SET IDENTITY_INSERT Speciality ON;
INSERT INTO Speciality (SpecialityID, Name, AddedOn, AddedBy, IsActive) VALUES (1,'General', GETDATE(), 1, 1);
SET IDENTITY_INSERT Speciality OFF;

-- ---- Modality ----
SET IDENTITY_INSERT Modality ON;
INSERT INTO Modality (Id, ModalityName, AddedOn, AddedBy) VALUES (1,'CT', GETDATE(), 1),(2,'MRI', GETDATE(), 1);
SET IDENTITY_INSERT Modality OFF;

-- ---- Clinic ----
SET IDENTITY_INSERT Clinic ON;
INSERT INTO Clinic (ClinicID, AccountID, Name, Address, City, State, Country, ZipCode, Phone, Email,
    AddedOn, AddedBy, IsActive, SystemID)
VALUES (1,'1','Downtown Clinic','123 Main St','Metropolis','NY','USA','10001','555-1000','clinic@local',
    GETDATE(), 1, 1, 'SYS1');
SET IDENTITY_INSERT Clinic OFF;

-- ---- Dictator / Provider (AccountID -> Clinic.ClinicID) ----
SET IDENTITY_INSERT Dictator ON;
INSERT INTO Dictator (DictatorID, AccountID, SystemID, LoginID, Password, AllClientAccess, Prefix,
    FirstName, MiddleName, LastName, Gender, SpecialityID, IsSecretary, IsFax, IsReview, AddedOn, AddedBy,
    IsActive, AddSign)
VALUES (1, 1, 'SYS1', 'drsmith', 'x', 0, 'Dr', 'John', '', 'Smith', 'Male', 1, 0, 1, 0, GETDATE(), 1, 1, 1);
SET IDENTITY_INSERT Dictator OFF;

-- ---- Sample UploadedFile (dictations) ----
SET IDENTITY_INSERT UploadedFile ON;
INSERT INTO UploadedFile (FileID, FileName, FilePath, Extension, Size, DictatorID, ClinicID, UploadBy,
    UploadOn, IsActive, IsDownloaded, StatusId, Duration, TypistId, TypistAssignID, StackMark)
VALUES
 (1,'audio_001','~/Content/Audio/audio_001.wav','wav','120 KB',1,1,1, DATEADD(day,-3,GETDATE()),1,0,5,'00:02:15',2,2,0),
 (2,'audio_002','~/Content/Audio/audio_002.wav','wav','98 KB', 1,1,1, DATEADD(day,-1,GETDATE()),1,0,3,'00:01:40',2,2,0);
SET IDENTITY_INSERT UploadedFile OFF;

-- ---- Sample UploadedDocument (documents) ----
SET IDENTITY_INSERT UploadedDocument ON;
INSERT INTO UploadedDocument (Id, FileID, FileName, FilePath, Extension, StatusId, UploadBy, UploadOn,
    PatientName, ModalityId, MRN, DOB, DOS, RefDoctor, FaxNumber, WordCount, CharCount, CharCountWithSpace, LineCount)
VALUES
 (1,1,'doc_001','~/Content/Transcripts/doc_001.docx','docx',5,1, DATEADD(day,-2,GETDATE()),
    'Alice Brown',1,'MRN1001','01/15/1980','06/20/2026','Dr. Adams','555-2001',350,1800,2100,28.0),
 (2,2,'doc_002','~/Content/Transcripts/doc_002.docx','docx',3,1, DATEADD(day,-1,GETDATE()),
    'Bob Carter',2,'MRN1002','03/22/1975','06/22/2026','Dr. Baker','555-2002',420,2200,2600,33.0);
SET IDENTITY_INSERT UploadedDocument OFF;

-- ---- Sample FileFaxStatus (fax page) ----
SET IDENTITY_INSERT FileFaxStatus ON;
INSERT INTO FileFaxStatus (ID, FaxID, UpDocID, FileID, Status, AddedOn, ToNumber, DateCreated, PageCount, IsActive)
VALUES
 (1,'FAX-1001',1,1,'Sent', DATEADD(day,-2,GETDATE()),'555-3001','2026-06-23','2',1),
 (2,'FAX-1002',2,2,'SendingFailed', DATEADD(day,-1,GETDATE()),'555-3002','2026-06-24','3',1);
SET IDENTITY_INSERT FileFaxStatus OFF;

PRINT 'Seed data inserted.';
GO

-- ===================== users_seed.sql =====================
GO
SET NOCOUNT ON;

-- Add a plaintext password column so Admin can VIEW passwords (the app already
-- stores plaintext provider/manager passwords, so this matches its model).
IF COL_LENGTH('dbo.AspNetUsers','PlainPassword') IS NULL
    ALTER TABLE dbo.AspNetUsers ADD PlainPassword nvarchar(200) NULL;
GO

-- Identity v2 hash for password 'Admin@123' (PBKDF2/HMACSHA1, 1000 iters)
DECLARE @hash nvarchar(200) = 'AMVY6XOlD+au+xrUCXOJfThoF9khgOBdikHSeKSyE7KNGIRnqsa3tpbPurHXaEpgnA==';

-- ---- Doctor / Provider login: drsmith (Dictator already exists w/ Password 'x') ----
IF NOT EXISTS (SELECT 1 FROM AspNetUsers WHERE UserName='drsmith')
BEGIN
    DECLARE @docId nvarchar(128) = NEWID();
    SET IDENTITY_INSERT AspNetUsers ON;
    INSERT INTO AspNetUsers (Id, Email, EmailConfirmed, PasswordHash, SecurityStamp, PhoneNumberConfirmed,
        TwoFactorEnabled, LockoutEnabled, AccessFailedCount, UserName, UserId, IsApproved, IsActive, UserType, Name, PlainPassword)
    VALUES (@docId, 'drsmith@local', 1, @hash, NEWID(), 0, 0, 1, 0, 'drsmith', 3, 1, 1, 'Provider', 'Dr. John Smith', 'Admin@123');
    SET IDENTITY_INSERT AspNetUsers OFF;
    INSERT INTO AspNetUserRoles (UserId, RoleId) SELECT @docId, Id FROM AspNetRoles WHERE Name='Provider';
END

-- ---- Manager login: manager1 (+ Manager row + ManagerClinic link) ----
IF NOT EXISTS (SELECT 1 FROM AspNetUsers WHERE UserName='manager1')
BEGIN
    DECLARE @mgrId nvarchar(128) = NEWID();
    SET IDENTITY_INSERT AspNetUsers ON;
    INSERT INTO AspNetUsers (Id, Email, EmailConfirmed, PasswordHash, SecurityStamp, PhoneNumberConfirmed,
        TwoFactorEnabled, LockoutEnabled, AccessFailedCount, UserName, UserId, IsApproved, IsActive, UserType, Name, PlainPassword)
    VALUES (@mgrId, 'manager1@local', 1, @hash, NEWID(), 0, 0, 1, 0, 'manager1', 4, 1, 1, 'Manager', 'Manager One', 'Admin@123');
    SET IDENTITY_INSERT AspNetUsers OFF;
    INSERT INTO AspNetUserRoles (UserId, RoleId) SELECT @mgrId, Id FROM AspNetRoles WHERE Name='Manager';

    DECLARE @newMgr int;
    INSERT INTO Manager (AccountID, Username, Password, AddedOn, AddedBy, isActive)
    VALUES (1, 'manager1', 'Admin@123', GETDATE(), 1, 1);
    SET @newMgr = SCOPE_IDENTITY();
    INSERT INTO ManagerClinic (ManagerID, ClinicID, DictatorID) VALUES (@newMgr, 1, 1);
END

-- ---- Backfill plaintext passwords for the pre-existing seeded users ----
UPDATE AspNetUsers SET PlainPassword='Admin@123' WHERE UserName IN ('admin','typist1') AND PlainPassword IS NULL;

PRINT 'User seed complete.';
GO

SELECT u.UserName, u.UserId, u.PlainPassword, r.Name AS Role
FROM AspNetUsers u
LEFT JOIN AspNetUserRoles ur ON ur.UserId=u.Id
LEFT JOIN AspNetRoles r ON r.Id=ur.RoleId
ORDER BY u.UserId;
GO


-- ===================== fix_defaults.sql (column defaults so inserts succeed) =====================
-- ================================================================
-- FIX: add DEFAULT constraints to every NOT NULL column that has no
-- default and is not an IDENTITY/computed column. This lets the app's
-- insert paths (ASP.NET Identity user creation, uploads, etc.) succeed
-- when they don't explicitly set an app-specific column.
-- Idempotent: skips columns that already have a default. Runs on any DB.
-- ================================================================
SET NOCOUNT ON;

DECLARE @sql nvarchar(max);
DECLARE @schema sysname, @table sysname, @col sysname, @type sysname, @cname sysname, @def nvarchar(100);

DECLARE cur CURSOR FOR
SELECT s.name, t.name, c.name, ty.name
FROM sys.columns c
JOIN sys.tables  t ON t.object_id = c.object_id
JOIN sys.schemas s ON s.schema_id = t.schema_id
JOIN sys.types  ty ON ty.user_type_id = c.user_type_id
WHERE c.is_nullable = 0
  AND c.is_identity = 0
  AND c.is_computed = 0
  AND c.default_object_id = 0          -- no existing default
  AND t.is_ms_shipped = 0
  AND t.name NOT LIKE 'sysdiagrams'
ORDER BY t.name, c.column_id;

OPEN cur;
FETCH NEXT FROM cur INTO @schema, @table, @col, @type;
WHILE @@FETCH_STATUS = 0
BEGIN
    SET @def = CASE
        WHEN @type IN ('bit')                                   THEN '0'
        WHEN @type IN ('tinyint','smallint','int','bigint')     THEN '0'
        WHEN @type IN ('decimal','numeric','money','smallmoney','float','real') THEN '0'
        WHEN @type IN ('datetime','datetime2','smalldatetime','date') THEN 'GETDATE()'
        WHEN @type IN ('uniqueidentifier')                      THEN 'NEWID()'
        WHEN @type IN ('nvarchar','varchar','nchar','char','text','ntext') THEN ''''''
        ELSE NULL END;

    IF @def IS NOT NULL
    BEGIN
        SET @cname = 'DF_' + @table + '_' + @col;
        SET @sql = 'ALTER TABLE ' + QUOTENAME(@schema) + '.' + QUOTENAME(@table) +
                   ' ADD CONSTRAINT ' + QUOTENAME(@cname) +
                   ' DEFAULT ' + @def + ' FOR ' + QUOTENAME(@col) + ';';
        BEGIN TRY
            EXEC sp_executesql @sql;
            PRINT 'Added default on ' + @table + '.' + @col + ' = ' + @def;
        END TRY
        BEGIN CATCH
            PRINT 'SKIP ' + @table + '.' + @col + ' : ' + ERROR_MESSAGE();
        END CATCH
    END
    FETCH NEXT FROM cur INTO @schema, @table, @col, @type;
END
CLOSE cur; DEALLOCATE cur;
PRINT 'Default-constraint fix complete.';
