using System;
using System.Threading.Tasks;
using LTC.AdministrationService.Showtimes;
using LTC.AdministrationService.Showtimes.Dtos;
using Shouldly;
using Volo.Abp;
using Xunit;

namespace LTC.AdministrationService.Showtimes;

public class ShowtimeAppServiceTests : AdministrationServiceApplicationTestBase<AdministrationServiceApplicationTestModule>
{
    private readonly IShowtimeAppService _showtimeAppService;

    public ShowtimeAppServiceTests()
    {
        _showtimeAppService = GetRequiredService<IShowtimeAppService>();
    }

    [Fact]
    public async Task Create_Should_Throw_When_Movie_Does_Not_Exist()
    {
        var input = new CreateShowtimeDto
        {
            MovieId = Guid.NewGuid(),
            CinemaId = Guid.NewGuid(),
            DistributionId = Guid.NewGuid(),
            ScreenId = Guid.NewGuid(),
            ShowDate = DateTime.UtcNow.Date.AddDays(1),
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(11, 0, 0),
            BasePrice = 100000
        };

        await Should.ThrowAsync<BusinessException>(async () => await _showtimeAppService.CreateAsync(input));
    }
}
