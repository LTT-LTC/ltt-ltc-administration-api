# LTC.AdministrationService.EntityFrameworkCore

## Purpose

Implements persistence for SQL Server (EF Core) and includes migrations.

## Responsibilities

- Define `AdministrationServiceDbContext`.
- Configure ABP module table mappings and schema.
- Maintain migration history.

## Key Files

- `EntityFrameworkCore/AdministrationServiceDbContext.cs`
- `EntityFrameworkCore/AdministrationServiceEntityFrameworkCoreModule.cs`
- `Migrations/*`

## Common Tasks

Add migration:

```bash
dotnet ef migrations add <MigrationName> --project src/LTC.AdministrationService.EntityFrameworkCore --startup-project LTC.AdministrationService.HttpApi.Host
```

Apply migration:

```bash
dotnet run --project src/LTC.AdministrationService.DbMigrator/LTC.AdministrationService.DbMigrator.csproj
```

```bash
dotnet ef database update -c AdministrationServiceDbContext -p src\LTC.AdministrationService.EntityFrameworkCore -s src\LTC.AdministrationService.EntityFrameworkCore
```

## Notes

Current SQL schema default is `ADM` from `AdministrationServiceConsts`.