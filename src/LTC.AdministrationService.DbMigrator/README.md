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

## How to Run & When to Use

### 1. First time pulling the source (Local Windows Development)
When you clone the repo to a new Windows machine (running SQL Server and Redis locally on your host):
1. **Restore & Build:** `dotnet build`
2. **Run the Migrator:**
   ```bash
   dotnet run --project src/LTC.AdministrationService.DbMigrator
   ```
   **Why?** This single command connects to your local SQL Server, executes all migrations to build the tables, **AND** triggers our `AdministrationServiceInitialDataSeeder` to automatically insert the `LTCINEMA` tenant, `ADMIN` user, and permissions!

---

### 2. Updating tables in the database (Day-to-day Development)
When you add a new entity/table to your `AdministrationServiceDbContext.cs`:

1. **Generate the Migration file:**
   ```bash
   dotnet ef migrations add Add_Your_Table_Name -c AdministrationServiceDbContext -p src\LTC.AdministrationService.EntityFrameworkCore -s src\LTC.AdministrationService.EntityFrameworkCore
   ```
2. **Apply the schema to your database:**
   ```bash
   dotnet ef database update -c AdministrationServiceDbContext -p src\LTC.AdministrationService.EntityFrameworkCore -s src\LTC.AdministrationService.EntityFrameworkCore
   ```
   **Why not run DbMigrator?** Because `dotnet ef database update` is much faster. It strictly pushes your new SQL schemas without booting up the entire ABP dependency injection framework just to check the data seeders. 

---

### 3. Initializing when using Docker-Compose
When deploying to a server or running your entire stack inside Docker, the SQL server hostname is `ltt-ltc-sqlserver` instead of `localhost`.

1. **Let Docker handle it:**
   Start your infrastructure (SQL, Redis, Mongo):
   ```bash
   docker-compose up -d ltt-ltc-sqlserver ltt-ltc-redis
   ```
2. **Run the DbMigrator Container:**
   ABP projects usually have the DbMigrator configured in the `docker-compose.yml` as a standalone, short-lived setup container. Just turn it on!
   ```bash
   docker-compose up administration-service-dbmigrator
   ```
   *(Check your docker-compose for the exact service name. Once that container finishes running, it applies the schemas and data seeders directly inside your Docker network. You can then safely start your api host container!)*