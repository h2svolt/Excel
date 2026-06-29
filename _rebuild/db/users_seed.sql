USE db_StagingExcel;
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
