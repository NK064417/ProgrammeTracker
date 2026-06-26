# Programme Tracker — Developer Setup Guide

## Tech Stack
- ASP.NET Core 8 MVC
- Entity Framework Core (Code-First)
- SQL Server / LocalDB
- ASP.NET Core Identity (role-based)
- MailKit (SMTP email)
- ExcelDataReader (one-time import)
- IHostedService (background notification loop)

---

## Project Structure

```
ProgrammeTracker/
├── Controllers/
│   ├── AccountController.cs      # Login / logout
│   ├── HomeController.cs         # Dashboard (US-01)
│   ├── ProgrammesController.cs   # Phase 1 & 2 CRUD (US-02, US-03)
│   ├── DeliverablesController.cs # Deliverables CRUD + status update (US-04)
│   ├── UsersController.cs        # Admin user management (US-05, US-06)
│   ├── NotificationsController.cs# Notification log + manual trigger (US-08)
│   └── SettingsController.cs     # SMTP config (US-09)
│
├── Models/
│   ├── User.cs                   # ApplicationUser (extends IdentityUser<int>)
│   ├── ProgrammeType.cs          # Lookup table
│   ├── Programme.cs              # Central entity (Phase 1 + Phase 2)
│   ├── Deliverable.cs            # Task / milestone
│   ├── NotificationLog.cs        # Audit log + AppSettings
│   └── Constants.cs              # Roles, statuses, lookup lists
│
├── Data/
│   └── ApplicationDbContext.cs   # EF Core context, relationships, seed data
│
├── Services/
│   ├── EmailService.cs           # MailKit SMTP + NotificationBackgroundService
│   └── ExcelImportService.cs     # One-time import from Programme_Tracker.xlsx
│
├── ViewModels/
│   └── ViewModels.cs             # All view models (Dashboard, Phase1, Phase2, etc.)
│
├── Views/
│   ├── Shared/_Layout.cshtml     # Sidebar layout
│   ├── Home/Index.cshtml         # Dashboard / tracker
│   ├── Account/Login.cshtml
│   ├── Programmes/               # Phase1, Phase2, Details, Index, Delete
│   ├── Deliverables/             # Index, Create, Edit, Delete
│   ├── Users/                    # Index, Create, Edit
│   ├── Notifications/Index.cshtml
│   └── Settings/Index.cshtml
│
├── Program.cs                    # DI, Identity, seeding, middleware
├── appsettings.json
└── ProgrammeTracker.csproj
```

---

## First-Time Setup

### 1. Prerequisites
- .NET 8 SDK
- SQL Server or SQL Server Express / LocalDB
- Visual Studio 2022 or VS Code with C# Dev Kit

### 2. Clone & Configure

Update `appsettings.json` with your SQL Server connection string:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=ProgrammeTrackerDb;Trusted_Connection=True;TrustServerCertificate=True"
}
```

For LocalDB (Visual Studio default):
```json
"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ProgrammeTrackerDb;Trusted_Connection=True"
```

### 3. Create & Apply Migrations

```bash
cd ProgrammeTracker

# Restore NuGet packages
dotnet restore

# Create initial migration
dotnet ef migrations add InitialCreate

# Apply to database (also runs automatically on startup)
dotnet ef database update
```

### 4. Run the Application

```bash
dotnet run
```

On first launch, `Program.cs` will:
1. Apply any pending migrations
2. Seed all 7 Identity roles
3. Create a default admin user: `admin@programmetracker.local` / `Admin@123!`
4. Run the Excel import if `Programme_Tracker.xlsx` exists in the project root

> **Change the admin password immediately after first login.**

### 5. Excel Import (Optional)

Place your existing `Programme_Tracker.xlsx` in the project root directory before first launch. The importer reads all sheets, uses the sheet name to detect delivery type (Campus/Online/Inhouse), and marks all records as `IsLegacyImport = true`. The import runs only once and is idempotent.

---

## Roles & Permissions Summary

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

## Email Notifications

Notifications are handled by `NotificationBackgroundService` (an `IHostedService`).

- Runs on a configurable interval (default: every 3600 seconds / 1 hour)
- Sends one email per deliverable per user per calendar day
- Automatically marks deliverables as `Overdue` when due date passes
- Admin can trigger a manual check from the Notifications screen
- SMTP settings are stored in the `AppSettings` database table and managed via the UI

---

## IIS Deployment (Windows Server)

1. Publish: `dotnet publish -c Release -o ./publish`
2. Install the [ASP.NET Core Hosting Bundle](https://dotnet.microsoft.com/download) on the server
3. Create an IIS site pointing to the `publish` folder
4. Set the Application Pool to **No Managed Code**
5. Ensure the app pool identity has read/write access to the publish folder
6. Update `appsettings.json` (or use environment variables / secrets) with the production connection string

---

## Notes for Development

- All POST actions use `[ValidateAntiForgeryToken]` as required by the spec
- `Phase1Complete` and `Phase2Complete` are computed automatically on every save via `Programme.ComputePhaseCompletion()`
- The `NotificationBackgroundService` is registered as a singleton so it can be injected into `NotificationsController` for manual triggering. It uses `IServiceScopeFactory` internally for scoped DB access.
- SETA Advisor is stored as free text on the Programme record — they have no system login
- Users are never hard-deleted — only deactivated (`IsActive = false`)
