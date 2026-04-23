using System;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace LTC.AdministrationService.Entities;

/// <summary>
/// Seat layout payload for a screen; referenced by <see cref="Screen.SeatMapId"/>.
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
    public string? SeatLayout { get; set; }
    public int SeatCount { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
