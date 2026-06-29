USE db_StagingExcel;
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
