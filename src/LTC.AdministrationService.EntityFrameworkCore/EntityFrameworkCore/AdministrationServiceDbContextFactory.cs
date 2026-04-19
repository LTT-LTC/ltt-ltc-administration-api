using System.IO;
using System.Linq;
using LTC.AdministrationService.MultiTenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace LTC.AdministrationService.EntityFrameworkCore;

/* This class is needed for EF Core console commands
 * (like Add-Migration and Update-Database commands) */
public class AdministrationServiceDbContextFactory : IDesignTimeDbContextFactory<AdministrationServiceDbContext>
{
    public AdministrationServiceDbContext CreateDbContext(string[] args)
    {
        var configuration = BuildConfiguration();
        
        AdministrationServiceEfCoreEntityExtensionMappings.Configure();

        var builder = new DbContextOptionsBuilder<AdministrationServiceDbContext>()
            .UseSqlServer(configuration.GetConnectionString("Default"));
        
        return new AdministrationServiceDbContext(builder.Options, new DesignTimeSchemaResolver());
    }

    private class DesignTimeSchemaResolver : ITenantSchemaResolver
    {
        public string GetSchemaName() => "dbo"; // Default schema for migrations
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var candidateBasePaths = new[]
        {
            Path.Combine(currentDirectory, "../LTC.AdministrationService.DbMigrator/"),
            Path.Combine(currentDirectory, "../src/LTC.AdministrationService.DbMigrator/"),
            Path.Combine(currentDirectory, "../../src/LTC.AdministrationService.DbMigrator/")
        };

        var migratorPath = candidateBasePaths
            .Select(Path.GetFullPath)
            .FirstOrDefault(Directory.Exists);

        if (migratorPath == null)
        {
            throw new DirectoryNotFoundException("Unable to locate LTC.AdministrationService.DbMigrator directory for EF design-time configuration.");
        }

        var builder = new ConfigurationBuilder()
            .SetBasePath(migratorPath)
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}
