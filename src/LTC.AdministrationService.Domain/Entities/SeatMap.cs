using System;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace LTC.AdministrationService.Entities;

/// <summary>
/// Reusable seat layout template for a cinema. The chosen template's
/// <see cref="SeatLayout"/> is snapshotted into <see cref="Screen.SeatLayout"/>
/// at create/update time.
/// </summary>
public class SeatMap : Entity<Guid>, IMultiTenant
{
    protected SeatMap()
    {
    }

    public SeatMap(Guid id)
        : base(id)
    {
    }

    public Guid? TenantId { get; set; }
    public Guid CinemaId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? SeatLayout { get; set; }
    public int SeatCount { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
