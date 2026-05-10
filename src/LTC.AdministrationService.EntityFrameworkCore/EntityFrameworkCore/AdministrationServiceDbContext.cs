using LTC.AdministrationService.Entities;
using LTC.AdministrationService.MultiTenancy;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.BlobStoring.Database.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;
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
    private readonly ITenantSchemaResolver _tenantSchemaResolver;

    public AdministrationServiceDbContext(
        DbContextOptions<AdministrationServiceDbContext> options,
        ITenantSchemaResolver tenantSchemaResolver)
    : base(options)
    {
        _tenantSchemaResolver = tenantSchemaResolver;
    }

    public string GetCurrentSchema() => _tenantSchemaResolver.GetSchemaName();

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
    public DbSet<SeatMap> SeatMaps { get; set; }
    public DbSet<SeatType> SeatTypes { get; set; }
    public DbSet<Showtime> Showtimes { get; set; }
    public DbSet<PricingRule> PricingRules { get; set; }
    public DbSet<CinemaAmenity> CinemaAmenities { get; set; }
    public DbSet<CinemaAmenityType> CinemaAmenityTypes { get; set; }
    public DbSet<GiftCode> GiftCodes { get; set; }
    public DbSet<RevenueSnapshot> RevenueSnapshots { get; set; }
    public DbSet<Entities.NewsAndOffers> NewsAndOffers { get; set; }

    #endregion

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        var schema = GetCurrentSchema();
        builder.HasDefaultSchema(schema);

        /* Include modules to your migration db context */

        builder.HasDefaultSchema("dbo");
        builder.ConfigurePermissionManagement();
        builder.ConfigureIdentity();
        builder.ConfigureTenantManagement();
        builder.ConfigureFeatureManagement();
        builder.ConfigureSettingManagement();
        
        builder.HasDefaultSchema(schema);
        /* Configure custom entities */

        builder.Entity<Entities.Employee>(b =>
        {
            b.ToTable("Employees");
            b.ConfigureByConvention();
            b.HasIndex(x => x.UserId)
                .IsUnique()
                .HasFilter("[UserId] IS NOT NULL");
            b.HasOne<IdentityUser>()
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.SetNull);
            b.HasOne<Entities.Cinema>()
                .WithMany()
                .HasForeignKey(x => x.CinemaId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        builder.Entity<MediaFile>(b =>
        {
            b.ToTable("MediaFiles");
            b.ConfigureByConvention(); 
        });

        builder.Entity<Cinema>(b =>
        {
            b.ToTable("Cinemas");
            b.ConfigureByConvention();
            b.HasIndex(x => x.ManagerUserId)
                .IsUnique()
                .HasFilter("[ManagerUserId] IS NOT NULL");
            b.HasOne<IdentityUser>()
                .WithMany()
                .HasForeignKey(x => x.ManagerUserId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        builder.Entity<SeatMap>(b =>
        {
            b.ToTable("SeatMaps");
            b.ConfigureByConvention();
            b.Property(x => x.Name).HasMaxLength(256).IsRequired();
            b.Property(x => x.Description).HasMaxLength(2000);
            b.Property(x => x.SeatLayout).HasColumnType("nvarchar(max)");
            b.Property(x => x.SeatCount)
                .HasConversion(
                    v => v.ToString(),
                    v => v == null ? 0 : int.Parse(v)
                )
                .HasMaxLength(32);
        });

        builder.Entity<Screen>(b =>
        {
            b.ToTable("Screens");
            b.ConfigureByConvention();
            b.Property(x => x.SeatLayout).HasColumnType("nvarchar(max)");
        });

        builder.Entity<SeatType>(b =>
        {
            b.ToTable("SeatTypes");
            b.ConfigureByConvention();
            b.Property(x => x.SeatColor).HasMaxLength(7);
        });

        builder.Entity<Showtime>(b =>
        {
            b.ToTable("Showtimes");
            b.ConfigureByConvention();
            b.Property(x => x.SeatLayout).HasColumnType("nvarchar(max)");
        });

        builder.Entity<PricingRule>(b =>
        {
            b.ToTable("PricingRules");
            b.ConfigureByConvention();
            b.Property(x => x.DayOfWeek).HasColumnType("nvarchar(max)");
        });

        builder.Entity<CinemaAmenity>(b =>
        {
            b.ToTable("CinemaAmenities");
            b.ConfigureByConvention();
        });

        builder.Entity<Entities.NewsAndOffers>(b =>
        {
            b.ToTable("NewsAndOffers");
            b.HasKey(x => x.Id);
            b.Property(x => x.Id).HasColumnName("Id").IsRequired();
            b.Property(x => x.CinemaId).HasColumnName("CinemaId");
            b.Property(x => x.Title).HasColumnName("Title").HasMaxLength(256).IsRequired();
            b.Property(x => x.Content).HasColumnName("Content").IsRequired();
            b.Property(x => x.StartDate).HasColumnName("StartDate");
            b.Property(x => x.EndDate).HasColumnName("EndDate");
            b.Property(x => x.IsActive).HasColumnName("IsActive").IsRequired();
            b.Property(x => x.PosterUrl).HasColumnName("PosterUrl");
            b.Property(x => x.IsDeleted).HasColumnName("IsDeleted").IsRequired();
            b.Property(x => x.CreatedAt).HasColumnName("CreatedAt");
            b.Property(x => x.UpdatedAt).HasColumnName("UpdatedAt");
        });

        builder.Entity<CinemaAmenityType>(b =>
        {
            b.ToTable("CinemaAmenityTypes");
            b.ConfigureByConvention();
        });

        builder.Entity<GiftCode>(b =>
        {
            b.ToTable("GiftCodes");
            b.ConfigureByConvention();
        });

        builder.Entity<RevenueSnapshot>(b =>
        {
            b.ToTable("RevenueSnapshots");
            b.ConfigureByConvention();
        });

        #region Preset Data (Data Seeding)
        // [Insert your preset data/seeding logic here]
        #endregion
    }
}
