# LTC.AdministrationService.DbMigrator

## Purpose

Applies EF Core migrations and seeds initial ABP/OpenIddict data.

## Responsibilities

- Upgrade SQL schema to current migration state.
- Execute startup data seed contributors.

## Key Files

- `Program.cs`
- `AdministrationServiceDbMigratorModule.cs`
- `appsettings.json`

## Run

```bash
dotnet run --project src/LTC.AdministrationService.DbMigrator/LTC.AdministrationService.DbMigrator.csproj
```

## When to Run

- First local setup.
- After adding or pulling new migrations.
- Before starting API host if schema mismatch errors appear.