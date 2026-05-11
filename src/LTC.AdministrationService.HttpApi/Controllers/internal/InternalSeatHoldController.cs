using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using LTC.AdministrationService.Customer.Showtimes;
using LTC.AdministrationService.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Volo.Abp.AspNetCore.Mvc;

namespace LTC.AdministrationService.Controllers.Internal;

[Route("/ltc/administration-service/internal/seat-hold")]
[AllowAnonymous]
public class InternalSeatHoldController : AbpControllerBase
{
    private readonly IShowtimeSeatHoldAppService _seatHoldAppService;
    private readonly InternalApiOptions _internalApi;

    public InternalSeatHoldController(
        IShowtimeSeatHoldAppService seatHoldAppService,
        IOptions<InternalApiOptions> internalApi)
    {
        _seatHoldAppService = seatHoldAppService;
        _internalApi = internalApi.Value;
    }

    /// <summary>Service-to-service: release Redis seat holds for a completed booking.</summary>
    [HttpPost("release")]
    public virtual async Task<IActionResult> ReleaseAsync(
        [FromHeader(Name = "X-Internal-Api-Key")] string? apiKey,
        [FromBody] ReleaseSeatHoldRequestDto body)
    {
        if (string.IsNullOrEmpty(_internalApi.CustomerServiceApiKey) ||
            !string.Equals(apiKey, _internalApi.CustomerServiceApiKey, StringComparison.Ordinal))
        {
            return Unauthorized();
        }

        if (body.ShowtimeId == Guid.Empty || string.IsNullOrWhiteSpace(body.SessionKey))
        {
            return BadRequest(new { error = "ShowtimeId and SessionKey are required." });
        }

        await _seatHoldAppService.ReleaseAsync(body.ShowtimeId, body.SessionKey);
        return Ok(new { released = true });
    }
}

public class ReleaseSeatHoldRequestDto
{
    [Required]
    public Guid ShowtimeId { get; set; }

    [Required]
    public string SessionKey { get; set; } = string.Empty;
}
