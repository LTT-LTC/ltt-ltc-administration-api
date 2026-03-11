using System;
using System.IO;
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
        
        return new AdministrationServiceDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../LTC.AdministrationService.DbMigrator/"))
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}
