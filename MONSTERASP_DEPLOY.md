# Deploying Excel on Work to MonsterASP.NET (free)

This app is ASP.NET MVC 5 / **.NET Framework 4.8** + **SQL Server**. MonsterASP.NET
supports exactly this, for free.

---

## 1. Create the free account + resources

1. Go to **https://www.monsterasp.net/** → sign up (free plan, no card).
2. In the dashboard, **create a website** → you get a URL like `yourname.runasp.net`.
3. **Create a MSSQL database** → the dashboard shows its **connection details**:
   `Server`, `Database name`, `User`, `Password`. Copy these.

---

## 2. Load the database

Use **`_rebuild/db/deploy_database.sql`** (this repo). It does NOT create a
database (the host already made one) — it builds the tables, stored procedures,
seed data, and login users inside your assigned DB.

Run it one of these ways:
- **MonsterASP “Run SQL query” tool** in the database panel — paste the script, run.
- **SSMS** (SQL Server Management Studio): connect using the external connection
  details from step 1, open `deploy_database.sql`, Execute.

After it runs you’ll have 26 tables, 16 procs, sample data, and these logins:

| Username | Role | Password |
|----------|------|----------|
| `admin` | SuperAdmin + Admin | `Admin@123` |
| `drsmith` | Provider (doctor) | `Admin@123` |
| `manager1` | Manager | `Admin@123` |
| `typist1` | Typist | `Admin@123` |

---

## 3. Point web.config at the host database

Edit **`web.config`** → `<connectionStrings>`. Replace the two entries with these,
filling in `{SERVER} {DBNAME} {DBUSER} {DBPASS}` from step 1:

```xml
<add name="DefaultConnection"
     connectionString="Data Source={SERVER};Initial Catalog={DBNAME};User ID={DBUSER};Password={DBPASS};MultipleActiveResultSets=True;Encrypt=True;TrustServerCertificate=True"
     providerName="System.Data.SqlClient" />

<add name="db_ExcelTransEntities"
     connectionString="metadata=res://*/Models.dBModel.csdl|res://*/Models.dBModel.ssdl|res://*/Models.dBModel.msl;provider=System.Data.SqlClient;provider connection string=&quot;Data Source={SERVER};Initial Catalog={DBNAME};User ID={DBUSER};Password={DBPASS};MultipleActiveResultSets=True;Encrypt=True;TrustServerCertificate=True;App=EntityFramework&quot;"
     providerName="System.Data.EntityClient" />
```

---

## 4. Upload the app files

Upload **the contents of this folder** (the published app — `bin/`, `Views/`,
`Content/`, `Scripts/`, `web.config`, `Global.asax` etc.) to the site root
(`wwwroot/`) using MonsterASP’s **File Manager** or **FTP** (FTP host/user/pass
are in the dashboard).

- Upload `bin/` as-is (it contains the compiled app + `roslyn/`).
- You do **not** need `_rebuild/` on the server (it’s source/scripts only) — but it’s harmless.
- The `Content/Transcripts`, `Temp`, `TempDownload` folders are created at runtime;
  make sure they exist and are writable (the File Manager can create them).

---

## 5. Browse

Open `https://yourname.runasp.net` → log in as `admin` / `Admin@123`.

---

## Known caveats

- **Spire DLLs** (`Spire.Doc`, `Spire.Pdf`, …) are commercial; in free mode they
  add watermarks / page limits to generated documents. Buy a license to remove.
- The stored procedures are **reconstructions** (original logic wasn’t shipped) —
  list/report behavior may differ from the original production system.
- Free MSSQL has a size cap; the schema + seed is tiny, so you’re fine to start.
- If you see a SQL `STRING_SPLIT` error, set the DB compatibility level to 130+
  (`ALTER DATABASE {DBNAME} SET COMPATIBILITY_LEVEL = 150;`).
