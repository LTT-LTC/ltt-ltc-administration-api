# LTC Administration API

## 1) What this project is

`ltt-ltc-administration-api` is an ABP-based service for administration features.

It includes:

- `LTC.AdministrationService.HttpApi.Host`: runnable API host.
- `src/LTC.AdministrationService.Application`: application services (use cases).
- `src/LTC.AdministrationService.HttpApi`: HTTP API layer (controllers/contracts exposure).
- `src/LTC.AdministrationService.EntityFrameworkCore`: EF Core DbContext + migrations.
- `src/LTC.AdministrationService.DbMigrator`: migration/seeding console app.
- `shared/*`: shared hosting/auth/openapi utilities.

Current permission model is ABP Permission Management (permission-based), with data in `ADM.AbpPermissions` and `ADM.AbpPermissionGrants`.

## 2) Prerequisites

- .NET SDK 10+
- SQL Server (LocalDB or full SQL Server)
- Redis

Optional:

- MongoDB (used by mail template flow)

## 3) Configure and run

### 3.1 Connection strings

Set your DB connection string in:

- `LTC.AdministrationService.HttpApi.Host/appsettings.json`
- `src/LTC.AdministrationService.DbMigrator/appsettings.json`

Example:

```json
{
  "ConnectionStrings": {
	 "Default": "Server=(LocalDb)\\MSSQLLocalDB;Database=AdministrationService;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

### 3.2 Apply migrations

From repository root:

```bash
dotnet run --project src/LTC.AdministrationService.DbMigrator/LTC.AdministrationService.DbMigrator.csproj
```

### 3.3 Run API host

```bash
dotnet run --project LTC.AdministrationService.HttpApi.Host/LTC.AdministrationService.HttpApi.Host.csproj
```

## 4) Route to code: API -> Application -> DB

Use this path when debugging or adding features:

1. API host entry and middleware:
	- `LTC.AdministrationService.HttpApi.Host/Program.cs`
	- `LTC.AdministrationService.HttpApi.Host/AdministrationServiceHttpApiHostModule.cs`
2. HTTP API layer:
	- `src/LTC.AdministrationService.HttpApi/*`
3. Application service layer:
	- `src/LTC.AdministrationService.Application/*`
4. Domain entities/rules:
	- `src/LTC.AdministrationService.Domain/*`
5. Persistence:
	- `src/LTC.AdministrationService.EntityFrameworkCore/EntityFrameworkCore/AdministrationServiceDbContext.cs`
	- `src/LTC.AdministrationService.EntityFrameworkCore/Migrations/*`

## 5) How to create a new API (end-to-end)

1. Add or update entity/domain object in `src/LTC.AdministrationService.Domain`.
2. Add DTOs and service contracts in `src/LTC.AdministrationService.Application.Contracts`.
3. Implement application logic in `src/LTC.AdministrationService.Application`.
4. Expose endpoint in `src/LTC.AdministrationService.HttpApi` (or conventional ABP controller flow if enabled).
5. Register permissions in:
	- `src/LTC.AdministrationService.Application.Contracts/Permissions/AdministrationServicePermissions.cs`
	- `src/LTC.AdministrationService.Application.Contracts/Permissions/AdministrationServicePermissionDefinitionProvider.cs`
6. Add/adjust EF mapping in DbContext if needed.
7. Create migration:

```bash
dotnet ef migrations add <MigrationName> --project src/LTC.AdministrationService.EntityFrameworkCore --startup-project LTC.AdministrationService.HttpApi.Host
```

8. Apply migration via DbMigrator.
9. Run host and test via Swagger.

## 6) How to add a new table

Always use EF Core migrations — **never add tables directly via raw SQL**. If you create a table manually, EF's model snapshot won't know about it and the next migration will attempt to drop it.

### Step 1 — Define the entity

In `src/LTC.AdministrationService.Domain/`, create your entity class:

```csharp
public class YourEntity : Entity<Guid>
{
    public string Name { get; set; }
    // add other properties...
}
```

### Step 2 — Add a DbSet to the DbContext

In `src/LTC.AdministrationService.EntityFrameworkCore/EntityFrameworkCore/AdministrationServiceDbContext.cs`:

```csharp
public DbSet<YourEntity> YourEntities { get; set; }
```

### Step 3 — Configure the table mapping

Inside `OnModelCreating` in the same file:

```csharp
builder.Entity<YourEntity>(b =>
{
    b.ToTable("YourEntities", AdministrationServiceConsts.DbSchema); // schema = "ADM"
    b.ConfigureByConvention();
    b.Property(x => x.Name).HasMaxLength(256).IsRequired();
});
```

### Step 4 — Generate the migration

```powershell
cd src/LTC.AdministrationService.EntityFrameworkCore
dotnet ef migrations add Add_YourEntity --no-build
```

### Step 5 — Apply the migration

```powershell
cd src/LTC.AdministrationService.DbMigrator
dotnet run
```

> **Seed data**: If the new table needs initial data, add INSERT statements to `LTTLTC.sql` **after** the migration has been applied. Schema changes always go through migrations; only row-level seed data belongs in SQL scripts.

## 7) Data stores currently used

- SQL Server schema: `ADM` (ABP/OpenIddict tables).
- Mongo collection for mail template: `adm.mail_template`.

## 7) Troubleshooting

- Error: `Invalid column name 'ManagementPermissionName'`.
  - Cause: DB schema is behind current ABP version/migrations.
  - Fix: run DbMigrator against the same DB used by HttpApi.Host.
