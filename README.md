# Programme Tracker

Role-based web application for tracking training programmes, deliverables and
deadlines. It replaces a spreadsheet-based process with a single dashboard and
automated email reminders for overdue work.

<img width="950" height="478" alt="image" src="https://github.com/user-attachments/assets/d3e2930b-4e28-4ae2-a3a8-dd85c7790414" />


## Highlights

- **Role-based access** with 7 roles (ASP.NET Core Identity) controlling who can create, edit or view
- **Automated notifications**: a background service sends deadline emails via MailKit/SMTP and flags overdue deliverables
- **Live dashboard** showing programme status and overdue work at a glance
- **Legacy data import**: one-time, idempotent import from Excel
- **Audit trail**: notification log, and users are deactivated rather than deleted

## Built with

- ASP.NET Core 8 MVC
- Entity Framework Core (code-first)
- SQL Server / LocalDB
- ASP.NET Core Identity (role-based)
- MailKit (SMTP email)
- ExcelDataReader (one-time import)
- `IHostedService` (background notification loop)

---

## Developer Setup Guide

### Project structure

> `US-xx` in the comments below refers to the user story each controller implements.

```
ProgrammeTracker/
├── Controllers/
│   ├── AccountController.cs        # Login / logout
│   ├── HomeController.cs           # Dashboard (US-01)
│   ├── ProgrammesController.cs     # Phase 1 & 2 CRUD (US-02, US-03)
│   ├── DeliverablesController.cs   # Deliverables CRUD + status update (US-04)
│   ├── UsersController.cs          # Admin user management (US-05, US-06)
│   ├── NotificationsController.cs  # Notification log + manual trigger (US-08)
│   └── SettingsController.cs       # SMTP config (US-09)
│
├── Models/
│   ├── User.cs                     # ApplicationUser (extends IdentityUser<int>)
│   ├── ProgrammeType.cs            # Lookup table
│   ├── Programme.cs                # Central entity (Phase 1 + Phase 2)
│   ├── Deliverable.cs              # Task / milestone
│   ├── NotificationLog.cs          # Audit log + AppSettings
│   └── Constants.cs                # Roles, statuses, lookup lists
│
├── Data/
│   └── ApplicationDbContext.cs     # EF Core context, relationships, seed data
│
├── Migrations/                     # EF Core migrations (included in the repo)
│
├── Services/
│   ├── EmailService.cs             # MailKit SMTP + NotificationBackgroundService
│   └── ExcelImportService.cs       # One-time import from Programme_Tracker.xlsx
│
├── ViewModels/
│   └── ViewModels.cs               # All view models (Dashboard, Phase1, Phase2, etc.)
│
├── Views/
│   ├── Shared/_Layout.cshtml       # Sidebar layout
│   ├── Home/Index.cshtml           # Dashboard / tracker
│   ├── Account/Login.cshtml
│   ├── Programmes/                 # Phase1, Phase2, Details, Index, Delete
│   ├── Deliverables/               # Index, Create, Edit, Delete
│   ├── Users/                      # Index, Create, Edit
│   ├── Notifications/Index.cshtml
│   └── Settings/Index.cshtml
│
├── docs/                           # Screenshots used in this README
├── Program.cs                      # DI, Identity, seeding, middleware
├── appsettings.json
└── ProgrammeTracker.csproj
```

### 1. Prerequisites

- .NET 8 SDK
- SQL Server, SQL Server Express or LocalDB
- Visual Studio 2022, or VS Code with the C# Dev Kit

### 2. Clone and configure the database

```bash
git clone https://github.com/NK064417/ProgrammeTracker.git
cd ProgrammeTracker
```

Set your SQL Server connection string in `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=ProgrammeTrackerDb;Trusted_Connection=True;TrustServerCertificate=True"
}
```

For LocalDB (the Visual Studio default):

```json
"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ProgrammeTrackerDb;Trusted_Connection=True"
```

### 3. Default admin account (development only)

If the database has no users yet, the app creates a default admin on first launch
so you can sign in:

| Email | Password |
|---|---|
| `admin@programmetracker.local` | `Admin@123!` |

> **These credentials are for local development and demos only.** Change the
> password immediately after first login, and never deploy with the default.

### 4. Apply the database migrations

The migrations are already included in the repo, so you only need to restore
packages and update the database:

```bash
dotnet restore
dotnet ef database update
```

Migrations also run automatically on startup. Only add a new migration
(`dotnet ef migrations add <Name>`) when you change the data model.

### 5. Run the application

```bash
dotnet run
```

On first launch, `Program.cs` will:

1. Apply any pending migrations
2. Seed all 7 Identity roles
3. Create the default admin user (only if no users exist yet)
4. Run the Excel import if `Programme_Tracker.xlsx` exists in the project root

### 6. Excel import (optional)

To migrate data from an existing spreadsheet, place `Programme_Tracker.xlsx` in
the project root before first launch. The file is not included in this repo.
The importer reads all sheets, uses the sheet name to detect the delivery type
(Campus / Online / Inhouse), and marks all records as `IsLegacyImport = true`.
The import runs only once and is idempotent.

---

## Roles and permissions

| Role | Create Programmes | Edit Programmes | View All | Manage Users | Settings |
|---|---|---|---|---|---|
| Admin | ✅ | ✅ | ✅ | ✅ | ✅ |
| Account Manager | ✅ | ✅ | ✅ | ❌ | ❌ |
| Project Coordinator | ✅ | Own only | ✅ | ❌ | ❌ |
| Facilitator | ❌ | ❌ | Own only | ❌ | ❌ |
| Talent Manager | ❌ | ❌ | Placement dates | ❌ | ❌ |
| Payroll | ❌ | ❌ | Impl. plan only | ❌ | ❌ |
| Management | ❌ | ❌ | Dashboard only | ❌ | ❌ |

---

## Email notifications

Notifications are handled by `NotificationBackgroundService` (an `IHostedService`).

- Runs on a configurable interval (default: every 3600 seconds / 1 hour)
- Sends one email per deliverable per user per calendar day
- Automatically marks deliverables as `Overdue` when the due date passes
- Admins can trigger a manual check from the Notifications screen
- SMTP settings are stored in the `AppSettings` database table and managed through the UI

---

## IIS deployment (Windows Server)

1. Publish: `dotnet publish -c Release -o ./publish`
2. Install the [ASP.NET Core Hosting Bundle](https://dotnet.microsoft.com/download) on the server
3. Create an IIS site pointing to the `publish` folder
4. Set the Application Pool to **No Managed Code**
5. Ensure the app pool identity has read/write access to the publish folder
6. Update `appsettings.json` (or use environment variables / secrets) with the production connection string, and change the default admin password after first login

---

## Development notes

- All POST actions use `[ValidateAntiForgeryToken]`
- `Phase1Complete` and `Phase2Complete` are computed automatically on every save via `Programme.ComputePhaseCompletion()`
- `NotificationBackgroundService` is registered as a singleton so it can be injected into `NotificationsController` for manual triggering. It uses `IServiceScopeFactory` internally for scoped database access
- The SETA Advisor is stored as free text on the Programme record, since they have no system login
- Users are never hard-deleted, only deactivated (`IsActive = false`)
