# Change Log — Excel on Work

This folder is the **published/precompiled** output of the ExcelTranscript ASP.NET MVC app
(original source project: `D:\1 Net\Excel Most Latest August\ExcelTranscript\ExcelTranscript\`).

All changes below were made to the **published `.cshtml` views** (which compile at runtime, so
they are already live) and to two **C# source files** (which are NOT live here — they require the
original Visual Studio project to be rebuilt; see "Needs Rebuild" section).

---

## 1. Branding — Mangotech → H2S Volt  (LIVE)

- **Footer** `Views/Shared/_Layout.cshtml`
  - Old: `Copyright © {year} Mangotech Solutions` (linked to mangotechsolutions.com)
  - New: `Powered by H2S Volt` (links to https://www.h2svolt.com/)
- **Print/Excel export footers** — 24 occurrences across 12 views changed from
  `Developed By : MangoTech Solutions` → `Powered by H2S Volt`
  (Managers, Clinics, ActivityLog, ActivityLogs, Home, FaxStatus, UploadFiles/AddTypistRole,
   WordDocUpload/Index, WordDocUpload/DocumentReport, Dictators, WordDocument/FaxStatusReport,
   UploadFiles/Index)
- **Report contact email** `Models/Reports/SaleOrderListDetails.rdlc`
  - `info@mangotechsolutions.com` → `info.h2svolt@gmail.com`

## 2. Add Provider page — Add Signature / IsReview fix  (LIVE)

- `Views/Dictators/Create.cshtml` and `Views/Dictators/Edit.cshtml`
  - The "Add Sign" checkbox was incorrectly bound to the `IsReview` property.
  - Now bound to the correct `AddSign` property via `@Html.CheckBox("AddSign", ...)`
    (uses CheckBox not CheckBoxFor because `AddSign` is a nullable bool).
  - This also satisfies "remove/hide IsReview" — there is no longer any control bound to IsReview.

## 3. Home page — Typist filter & column  (LIVE)

- `Views/Home/Index.cshtml` — uncommented the Typist filter dropdown.
- `Views/Home/_Index.cshtml` — uncommented the Typist column header and body cell.
- `Views/Home/Index.cshtml` — updated DataTables print export column indices `[0,2,3,4,5,6,7,8,9,10,11]`
  → `[0,2,3,4,5,6,7,8,9,10,11,12]` to account for the inserted Typist column.

## 4. Return to same page after viewing a doc  (LIVE)

- Added `stateSave: true` to the DataTables init on:
  Home/Index, WordDocUpload/Index, UploadFiles/Index, FaxStatus/Index.
- Remembers current page number, search term, and page length when navigating away and back.

## 5. Refresh button on Document List & Dictation List  (LIVE)

- `Views/WordDocUpload/Index.cshtml` and `Views/UploadFiles/Index.cshtml`
  - Added a DataTables "Refresh" button that re-triggers `#btnSearch` to reload the list with
    the latest fax statuses.

---

## Needs Rebuild (C# source — NOT live in this published folder)

- `Models/Helper.cs` — notification recipient changed
  `farooq.mangotech@gmail.com` → `info.h2svolt@gmail.com`.
  This is compiled into `bin/ExcelTranscript.dll`; the edit only takes effect after the original
  Visual Studio project is recompiled and the new DLL deployed.

---

## Still NOT implemented (require controller / stored-procedure source — not present in this published folder)

These items from the Minutes of Meeting could not be done because the server-side C# logic lives in
the compiled `bin/*.dll` and is not editable without the original source project:

1. One-time historic data entry (MRN = "Historic") — needs a new controller action + import logic.
2. Single clinic user → multiple facilities/doctors — needs multi-select UI + controller save logic.
3. DOT/DOS/DOB date filters on the Fax Status page — the `GetFaxStatusFilterWise` stored procedure
   has no parameters for these dates; needs sproc + controller changes.
4. Delete-rule enforcement (block delete of approved docs/dictations; full delete otherwise) —
   logic lives in the controller, could not be verified or changed.

---

## Database note

This published folder shipped **without** a database backup. The database (`db_StagingExcel`) was
reconstructed from `Models/dBModel.edmx`. See the accompanying SQL scripts (schema_tables.sql,
schema_procs.sql, schema_proc_dictator.sql, seed_admin.sql, fix_useridentity.sql) in the parent
folder. NOTE: the 15 stored procedures were created as **stubs** (correct output columns, but no real
query logic) except `stp_Dictator` (real insert/update) and `STRINGSplit_Fun` (real). The original
stored-procedure logic only existed inside the live SQL Server database and is NOT recoverable from
this folder — the recipient must supply the real database or the real stored procedures.
