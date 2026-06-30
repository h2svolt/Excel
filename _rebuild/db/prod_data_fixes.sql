-- Run this on the EXISTING production DB (db57918) to fix data-level bugs.
-- No GO batches -> safe to paste into WebMSSQL and run in one shot.

-- 1) SystemID must be numeric (app does int.Parse to generate the next one)
UPDATE Clinic   SET SystemID = CAST(1000 + ClinicID  AS varchar(50)) WHERE SystemID IS NULL OR ISNUMERIC(SystemID) = 0;
UPDATE Dictator SET SystemID = CAST(2000 + DictatorID AS varchar(50)) WHERE SystemID IS NULL OR ISNUMERIC(SystemID) = 0;
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
