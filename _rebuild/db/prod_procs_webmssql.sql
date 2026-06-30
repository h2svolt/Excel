-- Stored procedures for Excel on Work - WebMSSQL friendly (NO GO).
-- Each CREATE is wrapped in EXEC(N'...') so the whole file runs as ONE batch.
-- Safe to paste into MonsterASP WebMSSQL and run once.

-- ============================================================================
-- Reconstructed stored procedures for Excel on Work.
-- Column lists match the EF *_Result types EXACTLY (EF maps by column name and
-- throws if an expected column is missing). Proc bodies are best-effort
-- reconstructions (original bodies were not recoverable).
-- ============================================================================

IF OBJECT_ID('dbo.STRINGSplit_Fun','IF') IS NOT NULL DROP FUNCTION dbo.STRINGSplit_Fun;
EXEC(N'CREATE FUNCTION dbo.STRINGSplit_Fun (@pString nvarchar(max), @pDelimiter char(1))
RETURNS TABLE AS RETURN
(
    SELECT LTRIM(RTRIM(value)) AS Item
    FROM STRING_SPLIT(ISNULL(@pString,''''), @pDelimiter)
    WHERE LTRIM(RTRIM(value)) <> ''''
);');
-- ---- stp_GetTypist : (UserId, UserName) ------------------------------------
IF OBJECT_ID('dbo.stp_GetTypist','P') IS NOT NULL DROP PROCEDURE dbo.stp_GetTypist;
EXEC(N'CREATE PROCEDURE dbo.stp_GetTypist AS
BEGIN
    SET NOCOUNT ON;
    SELECT u.UserId AS UserId, u.UserName AS UserName
    FROM AspNetUsers u
    INNER JOIN AspNetUserRoles ur ON ur.UserId = u.Id
    INNER JOIN AspNetRoles r ON r.Id = ur.RoleId
    WHERE r.Name = ''Typist''
    ORDER BY u.UserName;
END');
-- ---- stp_GetDocuments : document list --------------------------------------
IF OBJECT_ID('dbo.stp_GetDocuments','P') IS NOT NULL DROP PROCEDURE dbo.stp_GetDocuments;
EXEC(N'CREATE PROCEDURE dbo.stp_GetDocuments
    @DictatorIDs int = NULL, @ClinicID int = NULL, @TypistID int = NULL,
    @StatusId int = NULL, @FromDate datetime = NULL, @ToDate datetime = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        uf.FileName AS AudioName, s.Name AS Status, uf.TypistAssignID AS TypistAssignID,
        (ISNULL(d.FirstName,'''') + '' '' + ISNULL(d.LastName,'''')) AS Provider, c.Name AS ClinicName,
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
      AND (@ToDate   IS NULL OR uf.UploadOn < DATEADD(DAY,1,@ToDate))
    ORDER BY ud.Id DESC;
END');
-- ---- stp_GetDictations : dictation list ------------------------------------
IF OBJECT_ID('dbo.stp_GetDictations','P') IS NOT NULL DROP PROCEDURE dbo.stp_GetDictations;
EXEC(N'CREATE PROCEDURE dbo.stp_GetDictations
    @DictatorIDs nvarchar(max) = NULL, @ClinicIDs nvarchar(max) = NULL,
    @TypistID int = NULL, @StatusId int = NULL, @FromDate datetime = NULL, @ToDate datetime = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        uf.TypistAssignID AS TypistAssignID, uf.UploadBy AS UploadBy, uf.FilePath AS FilePath,
        uf.Duration AS Duration, uf.FileID AS FileID, uf.FileName AS FileName, uf.UploadOn AS UploadOn,
        uf.DictatorID AS DictatorID, ISNULL(uf.IsDownloaded,0) AS isDownloaded,
        (ISNULL(d.FirstName,'''') + '' '' + ISNULL(d.LastName,'''')) AS Provider, c.Name AS ClinicName
    FROM UploadedFile uf
    LEFT JOIN Dictator d ON uf.DictatorID = d.DictatorID
    LEFT JOIN Clinic c ON d.AccountID = c.ClinicID
    WHERE ISNULL(uf.IsActive,1) = 1
      AND (@DictatorIDs IS NULL OR @DictatorIDs='''' OR uf.DictatorID IN (SELECT TRY_CONVERT(int,Item) FROM dbo.STRINGSplit_Fun(@DictatorIDs,'','')))
      AND (@ClinicIDs   IS NULL OR @ClinicIDs=''''   OR d.AccountID  IN (SELECT TRY_CONVERT(int,Item) FROM dbo.STRINGSplit_Fun(@ClinicIDs,'','')))
      AND (@TypistID    IS NULL OR uf.TypistAssignID = @TypistID)
      AND (@StatusId    IS NULL OR uf.StatusId = @StatusId)
      AND (@FromDate    IS NULL OR uf.UploadOn >= @FromDate)
      AND (@ToDate      IS NULL OR uf.UploadOn < DATEADD(DAY,1,@ToDate))
    ORDER BY uf.FileID DESC;
END');
-- ---- stp_GetDocumentsDataNew : Home grid (DOT/DOS/DOB ranges) ----------------
IF OBJECT_ID('dbo.stp_GetDocumentsDataNew','P') IS NOT NULL DROP PROCEDURE dbo.stp_GetDocumentsDataNew;
EXEC(N'CREATE PROCEDURE dbo.stp_GetDocumentsDataNew
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
        (ISNULL(d.FirstName,'''') + '' '' + ISNULL(d.LastName,'''')) AS Provider,
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
      AND (@DictatorIDs IS NULL OR @DictatorIDs='''' OR uf.DictatorID IN (SELECT TRY_CONVERT(int,Item) FROM dbo.STRINGSplit_Fun(@DictatorIDs,'','')))
      AND (@ClinicIDs   IS NULL OR @ClinicIDs=''''   OR d.AccountID  IN (SELECT TRY_CONVERT(int,Item) FROM dbo.STRINGSplit_Fun(@ClinicIDs,'','')))
      AND (@TypistID    IS NULL OR uf.TypistAssignID = @TypistID)
      AND (@StatusId    IS NULL OR uf.StatusId = @StatusId)
      AND (@DotFromDate IS NULL OR uf.UploadOn >= @DotFromDate)
      AND (@DotToDate   IS NULL OR uf.UploadOn < DATEADD(DAY,1,@DotToDate))
      AND (@DosFromDate IS NULL OR TRY_CONVERT(datetime, ud.DOS) >= @DosFromDate)
      AND (@DosToDate   IS NULL OR TRY_CONVERT(datetime, ud.DOS) < DATEADD(DAY,1,@DosToDate))
      AND (@DoDFromDate IS NULL OR TRY_CONVERT(datetime, ud.DOB) >= @DoDFromDate)
      AND (@DoDToDate   IS NULL OR TRY_CONVERT(datetime, ud.DOB) < DATEADD(DAY,1,@DoDToDate))
    ORDER BY ISNULL(uf.StackMark,0) DESC, uf.FileID DESC;
END');
-- ---- stp_GetDocumentsData : same shape, single DOB naming -------------------
IF OBJECT_ID('dbo.stp_GetDocumentsData','P') IS NOT NULL DROP PROCEDURE dbo.stp_GetDocumentsData;
EXEC(N'CREATE PROCEDURE dbo.stp_GetDocumentsData
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
END');
-- ---- GetFaxStatusFilterWise : FAX page (+ NEW DOT/DOS/DOB params) ------------
IF OBJECT_ID('dbo.GetFaxStatusFilterWise','P') IS NOT NULL DROP PROCEDURE dbo.GetFaxStatusFilterWise;
EXEC(N'CREATE PROCEDURE dbo.GetFaxStatusFilterWise
    @ClinicIDs nvarchar(max) = NULL, @fromDate datetime = NULL, @toDate datetime = NULL,
    @ProviderIDs nvarchar(max) = NULL, @TypistID int = NULL, @FileName nvarchar(50) = NULL,
    @DotFromDate datetime = NULL, @DotToDate datetime = NULL,
    @DosFromDate datetime = NULL, @DosToDate datetime = NULL,
    @DobFromDate datetime = NULL, @DobToDate datetime = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        c.Name AS Clinic, (ISNULL(d.FirstName,'''') + '' '' + ISNULL(d.LastName,'''')) AS Provider,
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
      AND (@ClinicIDs   IS NULL OR @ClinicIDs=''''   OR d.AccountID  IN (SELECT TRY_CONVERT(int,Item) FROM dbo.STRINGSplit_Fun(@ClinicIDs,'','')))
      AND (@ProviderIDs IS NULL OR @ProviderIDs='''' OR uf.DictatorID IN (SELECT TRY_CONVERT(int,Item) FROM dbo.STRINGSplit_Fun(@ProviderIDs,'','')))
      AND (@TypistID    IS NULL OR uf.TypistAssignID = @TypistID)
      AND (@FileName    IS NULL OR @FileName='''' OR ffs.Status = @FileName)
      AND (@fromDate    IS NULL OR ffs.AddedOn >= @fromDate)
      AND (@toDate      IS NULL OR ffs.AddedOn < DATEADD(DAY,1,@toDate))
      AND (@DotFromDate IS NULL OR uf.UploadOn >= @DotFromDate)
      AND (@DotToDate   IS NULL OR uf.UploadOn < DATEADD(DAY,1,@DotToDate))
      AND (@DosFromDate IS NULL OR TRY_CONVERT(datetime, ud.DOS) >= @DosFromDate)
      AND (@DosToDate   IS NULL OR TRY_CONVERT(datetime, ud.DOS) < DATEADD(DAY,1,@DosToDate))
      AND (@DobFromDate IS NULL OR TRY_CONVERT(datetime, ud.DOB) >= @DobFromDate)
      AND (@DobToDate   IS NULL OR TRY_CONVERT(datetime, ud.DOB) < DATEADD(DAY,1,@DobToDate))
    ORDER BY ffs.ID DESC;
END');
-- ---- GetManagers ------------------------------------------------------------
IF OBJECT_ID('dbo.GetManagers','P') IS NOT NULL DROP PROCEDURE dbo.GetManagers;
EXEC(N'CREATE PROCEDURE dbo.GetManagers @ManagerID int = NULL AS
BEGIN
    SET NOCOUNT ON;
    SELECT m.ManagerID AS ManagerID, m.Username AS Username, m.Password AS Password, m.isActive AS isActive,
        m.AddedOn AS AddedOn, m.AccountID AS AccountID,
        (ISNULL(d.FirstName,'''') + '' '' + ISNULL(d.LastName,'''')) AS DictatorName,
        ISNULL(mc.DictatorID,0) AS DictatorID, c.Name AS ClinicName, ISNULL(mc.ClinicID,0) AS ClinicID
    FROM Manager m
    LEFT JOIN ManagerClinic mc ON mc.ManagerID = m.ManagerID
    LEFT JOIN Dictator d ON mc.DictatorID = d.DictatorID
    LEFT JOIN Clinic c ON mc.ClinicID = c.ClinicID
    WHERE (@ManagerID IS NULL OR @ManagerID = 0 OR m.ManagerID = @ManagerID);
END');
-- ---- stp_GetActivityLogs ----------------------------------------------------
IF OBJECT_ID('dbo.stp_GetActivityLogs','P') IS NOT NULL DROP PROCEDURE dbo.stp_GetActivityLogs;
EXEC(N'CREATE PROCEDURE dbo.stp_GetActivityLogs
    @DictatorID int = NULL, @StatusId int = NULL, @FromDate datetime = NULL, @ToDate datetime = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        uf.FileID AS AudioID, CAST(NULL AS int) AS Action, s.Name AS Status,
        (ISNULL(d.FirstName,'''') + '' '' + ISNULL(d.LastName,'''')) AS Provider,
        uf.UploadOn AS ActivityDate, uf.FileName AS AudioFile, uf.Duration AS Duration, s.Color AS StatusColor
    FROM UploadedFile uf
    LEFT JOIN Dictator d ON uf.DictatorID = d.DictatorID
    LEFT JOIN Status s ON uf.StatusId = s.Id
    WHERE (@DictatorID IS NULL OR uf.DictatorID = @DictatorID)
      AND (@StatusId   IS NULL OR uf.StatusId = @StatusId)
      AND (@FromDate   IS NULL OR uf.UploadOn >= @FromDate)
      AND (@ToDate     IS NULL OR uf.UploadOn < DATEADD(DAY,1,@ToDate))
    ORDER BY uf.FileID DESC;
END');
-- ---- stp_GetUploadedAudio ---------------------------------------------------
IF OBJECT_ID('dbo.stp_GetUploadedAudio','P') IS NOT NULL DROP PROCEDURE dbo.stp_GetUploadedAudio;
EXEC(N'CREATE PROCEDURE dbo.stp_GetUploadedAudio
    @DictatorID int = NULL, @ClinicID int = NULL, @StatusId int = NULL,
    @FromDate datetime = NULL, @ToDate datetime = NULL, @TypistAssignId int = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        uf.FileID AS AudioID, s.Name AS Status, c.Name AS Clinic,
        (ISNULL(d.FirstName,'''') + '' '' + ISNULL(d.LastName,'''')) AS Provider, uf.UploadOn AS Uploaded,
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
      AND (@ToDate     IS NULL OR uf.UploadOn < DATEADD(DAY,1,@ToDate))
    ORDER BY uf.FileID DESC;
END');
-- ---- stp_GetUploadedAudioInfo (superset + IsActive/DocumentName/counts) ------
IF OBJECT_ID('dbo.stp_GetUploadedAudioInfo','P') IS NOT NULL DROP PROCEDURE dbo.stp_GetUploadedAudioInfo;
EXEC(N'CREATE PROCEDURE dbo.stp_GetUploadedAudioInfo
    @DictatorID int = NULL, @ClinicID int = NULL, @StatusId int = NULL,
    @FromDate datetime = NULL, @ToDate datetime = NULL, @TypistAssignId int = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        uf.FileID AS AudioID, s.Name AS Status, c.Name AS Clinic,
        (ISNULL(d.FirstName,'''') + '' '' + ISNULL(d.LastName,'''')) AS Provider, uf.UploadOn AS Uploaded,
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
      AND (@ToDate     IS NULL OR uf.UploadOn < DATEADD(DAY,1,@ToDate))
    ORDER BY uf.FileID DESC;
END');
-- ---- stp_DocumentReport -----------------------------------------------------
IF OBJECT_ID('dbo.stp_DocumentReport','P') IS NOT NULL DROP PROCEDURE dbo.stp_DocumentReport;
EXEC(N'CREATE PROCEDURE dbo.stp_DocumentReport
    @DictatorID int = NULL, @ClinicID int = NULL, @StatusId int = NULL,
    @FromDate datetime = NULL, @ToDate datetime = NULL, @TypistAssignId int = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        uf.DictatorID AS ProviderId, (ISNULL(d.FirstName,'''') + '' '' + ISNULL(d.LastName,'''')) AS Provider,
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
      AND (@ToDate     IS NULL OR uf.UploadOn < DATEADD(DAY,1,@ToDate))
    ORDER BY ud.Id DESC;
END');
-- ---- stp_GetUserLog ---------------------------------------------------------
IF OBJECT_ID('dbo.stp_GetUserLog','P') IS NOT NULL DROP PROCEDURE dbo.stp_GetUserLog;
EXEC(N'CREATE PROCEDURE dbo.stp_GetUserLog @AudioID int = NULL AS
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
END');
-- ---- stp_GetUserLogByDocumentId (same shape) --------------------------------
IF OBJECT_ID('dbo.stp_GetUserLogByDocumentId','P') IS NOT NULL DROP PROCEDURE dbo.stp_GetUserLogByDocumentId;
EXEC(N'CREATE PROCEDURE dbo.stp_GetUserLogByDocumentId @DocID int = NULL AS
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
END');
-- ---- Proc_InsertErrorDetails : logging stub ---------------------------------
IF OBJECT_ID('dbo.Proc_InsertErrorDetails','P') IS NOT NULL DROP PROCEDURE dbo.Proc_InsertErrorDetails;
EXEC(N'CREATE PROCEDURE dbo.Proc_InsertErrorDetails AS BEGIN SET NOCOUNT ON; RETURN 0; END');
-- ---- stp_Dictator : insert/update a provider --------------------------------
IF OBJECT_ID('dbo.stp_Dictator','P') IS NOT NULL DROP PROCEDURE dbo.stp_Dictator;
EXEC(N'CREATE PROCEDURE dbo.stp_Dictator
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
END');
PRINT 'Stored procedures created.';
