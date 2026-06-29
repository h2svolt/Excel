IF DB_ID('db_StagingExcel') IS NULL CREATE DATABASE db_StagingExcel;
GO
USE db_StagingExcel;
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