using LTC.AdministrationService.Entities;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.BlobStoring.Database.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.EntityFrameworkCore;

namespace LTC.AdministrationService.EntityFrameworkCore;

[ConnectionStringName(AdministrationServiceConsts.ConnectionStringName)]
public class AdministrationServiceDbContext :
    AbpDbContext<AdministrationServiceDbContext>
{
    public AdministrationServiceDbContext(DbContextOptions<AdministrationServiceDbContext> options)
    : base(options)
    {

    }

    #region Entities from the modules

    // Identity
    public DbSet<IdentityUser> Users { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }

    // Tenant Management
    public DbSet<Tenant> Tenants { get; set; }

    #endregion

    #region Administration Entities

    public DbSet<Entities.Employee> Employees { get; set; }
    public DbSet<MediaFile> MediaFiles { get; set; }
    public DbSet<Cinema> Cinemas { get; set; }
    public DbSet<Screen> Screens { get; set; }
    public DbSet<SeatType> SeatTypes { get; set; }
    public DbSet<Showtime> Showtimes { get; set; }
    public DbSet<PricingRule> PricingRules { get; set; }
    public DbSet<CinemaAmenity> CinemaAmenities { get; set; }
    public DbSet<CinemaAmenityType> CinemaAmenityTypes { get; set; }
    public DbSet<GiftCode> GiftCodes { get; set; }
    public DbSet<RevenueSnapshot> RevenueSnapshots { get; set; }

    #endregion

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.HasDefaultSchema(AdministrationServiceConsts.DbSchema);

        /* Include modules to your migration db context */

        builder.ConfigurePermissionManagement();
        builder.ConfigureIdentity();
        builder.ConfigureTenantManagement();
        builder.ConfigureFeatureManagement();
        builder.ConfigureSettingManagement();
        
        // Remove unused Identity tables
        builder.Ignore<IdentityUserClaim>();
        builder.Ignore<IdentityRoleClaim>();
        builder.Ignore<IdentityUserLogin>();
        builder.Ignore<IdentityUserToken>();
        builder.Ignore<IdentitySecurityLog>();
        builder.Ignore<IdentityLinkUser>();
        builder.Ignore<OrganizationUnitRole>();
        builder.Ignore<IdentitySession>();

        // Remove unused TenantManagement tables
        builder.Ignore<TenantConnectionString>();

        /* Configure custom entities */

        builder.Entity<Entities.Employee>(b =>
        {
            b.ToTable("Employees", AdministrationServiceConsts.DbSchema);
            b.ConfigureByConvention(); 
        });

        builder.Entity<MediaFile>(b =>
        {
            b.ToTable("MediaFiles", AdministrationServiceConsts.DbSchema);
            b.ConfigureByConvention(); 
        });

        builder.Entity<Cinema>(b =>
        {
            b.ToTable("Cinemas", AdministrationServiceConsts.DbSchema);
            b.ConfigureByConvention();
        });

        builder.Entity<Screen>(b =>
        {
            b.ToTable("Screens", AdministrationServiceConsts.DbSchema);
            b.ConfigureByConvention();
        });

        builder.Entity<SeatType>(b =>
        {
            b.ToTable("SeatTypes", AdministrationServiceConsts.DbSchema);
            b.ConfigureByConvention();
        });

        builder.Entity<Showtime>(b =>
        {
            b.ToTable("Showtimes", AdministrationServiceConsts.DbSchema);
            b.ConfigureByConvention();
        });

        builder.Entity<PricingRule>(b =>
        {
            b.ToTable("PricingRules", AdministrationServiceConsts.DbSchema);
            b.ConfigureByConvention();
        });

        builder.Entity<CinemaAmenity>(b =>
        {
            b.ToTable("CinemaAmenities", AdministrationServiceConsts.DbSchema);
            b.ConfigureByConvention();
        });

        builder.Entity<CinemaAmenityType>(b =>
        {
            b.ToTable("CinemaAmenityTypes", AdministrationServiceConsts.DbSchema);
            b.ConfigureByConvention();
        });

        builder.Entity<GiftCode>(b =>
        {
            b.ToTable("GiftCodes", AdministrationServiceConsts.DbSchema);
            b.ConfigureByConvention();
        });

        builder.Entity<RevenueSnapshot>(b =>
        {
            b.ToTable("RevenueSnapshots", AdministrationServiceConsts.DbSchema);
            b.ConfigureByConvention();
        });

        #region Preset Data (Data Seeding)
        // [Insert your preset data/seeding logic here]
        #endregion
    }
}
