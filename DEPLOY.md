# Excel on Work — Deployment & Rebuild Guide

This is an **ASP.NET MVC 5 / .NET Framework 4.8** application (Windows + IIS + **SQL Server**).

> ⚠️ **It cannot run on Vercel / Netlify / any Linux serverless host.** Those run
> Node/Python/Go on Linux; this app needs Windows, IIS, and SQL Server. Use a
> Windows .NET host instead (see below).

## Recommended free hosting (app + SQL Server)

| Platform | App | Database |
|----------|-----|----------|
| **MonsterASP.NET** (free) | ASP.NET / .NET Framework | free MSSQL — easiest match |
| **Azure** | App Service (Windows, free F1) | Azure SQL (free tier) |
| **SmarterASP.NET** | free trial | MSSQL |

Deploy the contents of this folder (the published app, including `bin/`) to the
host's web root, then point `web.config` at the host's SQL Server.

## Database setup

The original stored-procedure logic was not shipped with this published app, so
the database here is **reconstructed**. Run these scripts (in `_rebuild/db/`) in
order against a fresh SQL Server database named `db_StagingExcel`:

1. `schema_tables.sql`  — 26 tables (generated from the EF model)
2. `schema_procs.sql`   — 16 stored procedures (best-effort reconstructions)
3. `seed.sql`           — lookup data, a clinic/provider, sample documents + fax rows
4. `users_seed.sql`     — login users for each role + a `PlainPassword` column

### Seeded logins (all password `Admin@123`)

| Username | Role |
|----------|------|
| `admin` | SuperAdmin + Admin |
| `drsmith` | Provider (doctor) |
| `manager1` | Manager |
| `typist1` | Typist |

## Connection string

`web.config` → `connectionStrings`. Update `Data Source`, `User ID`, `Password`
for your host's SQL Server. (SQL auth shown; mixed-mode must be enabled on the
server.)

## Rebuilding the app DLL

The editable source recovered from `bin/ExcelTranscript.dll` is in
`_rebuild/source/ExcelTranscript/` (a buildable .NET Framework 4.8 project).
Build with MSBuild + the Roslyn compiler in `bin/roslyn/`:

```
MSBuild _rebuild/source/ExcelTranscript/build.csproj /p:Configuration=Release \
  /p:CscToolPath=<path>\bin\roslyn
```

The build embeds the EF metadata (`Models.dBModel.csdl/ssdl/msl`). Copy the
resulting `ExcelTranscript.dll` into `bin/`.

## Features added on top of the original

- **Fax Status page** — DOT / DOS / DOB date filters
- **Historic Data Entry** — records stored with `MRN = "Historic"`
- **Admin → User Management** — list users, view/change passwords, delete users
- **Typist List** nav link restored

> **Note:** runtime patient data (`Content/Transcripts`, `Temp`, `TempDownload`)
> is intentionally excluded from this repo for privacy. Those folders are created
> at runtime.
