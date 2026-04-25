using Riok.Mapperly.Abstractions;
using Volo.Abp.ObjectMapping;
using Volo.Abp.DependencyInjection;
using LTC.AdministrationService.Entities;
using LTC.AdministrationService.Admin.Cinemas.Dtos.Input;
using LTC.AdministrationService.Admin.Cinemas.Dtos.Output;
using LTC.AdministrationService.Admin.Screens.Dtos.Input;
using LTC.AdministrationService.Admin.Screens.Dtos.Output;
using LTC.AdministrationService.Admin.SeatTypes.Dtos.Input;
using LTC.AdministrationService.Admin.SeatTypes.Dtos.Output;
using LTC.AdministrationService.PricingRules.Dtos;
using LTC.AdministrationService.Employee.Dtos.Output;
using LTC.AdministrationService.Employee.Dtos.Input;
using LTC.AdministrationService.NewsAndOffers.Dtos.Input;
using LTC.AdministrationService.NewsAndOffers.Dtos.Output;

namespace LTC.AdministrationService;

[Mapper]
public partial class CinemaOutputMapper : IObjectMapper<Entities.Cinema, CinemasOutputDto>, ITransientDependency
{
    public partial CinemasOutputDto Map(Entities.Cinema source);
    public CinemasOutputDto Map(Entities.Cinema source, CinemasOutputDto destination) => Map(source);
}

[Mapper]
public partial class CreateCinemaInputMapper : IObjectMapper<CreateCinemasInputDto, Entities.Cinema>, ITransientDependency
{
    public partial Entities.Cinema Map(CreateCinemasInputDto source);
    public Entities.Cinema Map(CreateCinemasInputDto source, Entities.Cinema destination) => null;
}

[Mapper]
public partial class UpdateCinemaInputMapper : IObjectMapper<UpdateCinemasInputDto, Entities.Cinema>, ITransientDependency
{
    public partial Entities.Cinema Map(UpdateCinemasInputDto source);
    public Entities.Cinema Map(UpdateCinemasInputDto source, Entities.Cinema destination) { MapUpdate(source, destination); return destination; }
    [MapperIgnoreTarget(nameof(Entities.Cinema.Id))]
    [MapperIgnoreTarget(nameof(Entities.Cinema.TenantId))]
    public partial void MapUpdate(UpdateCinemasInputDto source, Entities.Cinema target);
}

[Mapper]
public partial class ScreenOutputMapper : IObjectMapper<Entities.Screen, ScreenOutputDto>, ITransientDependency
{
    [MapperIgnoreTarget(nameof(ScreenOutputDto.SeatLayout))]
    [MapperIgnoreTarget(nameof(ScreenOutputDto.SeatCount))]
    public partial ScreenOutputDto Map(Entities.Screen source);
    public ScreenOutputDto Map(Entities.Screen source, ScreenOutputDto destination) => Map(source);
}

[Mapper]
public partial class CreateScreenInputMapper : IObjectMapper<CreateScreenInputDto, Entities.Screen>, ITransientDependency
{
    [MapperIgnoreTarget(nameof(Entities.Screen.SeatMap))]
    public partial Entities.Screen Map(CreateScreenInputDto source);
    public Entities.Screen Map(CreateScreenInputDto source, Entities.Screen destination) => null;
}

[Mapper]
public partial class UpdateScreenInputMapper : IObjectMapper<UpdateScreenInputDto, Entities.Screen>, ITransientDependency
{
    public partial Entities.Screen Map(UpdateScreenInputDto source);
    public Entities.Screen Map(UpdateScreenInputDto source, Entities.Screen destination) { MapUpdate(source, destination); return destination; }
    [MapperIgnoreTarget(nameof(Entities.Screen.Id))]
    [MapperIgnoreTarget(nameof(Entities.Screen.TenantId))]
    [MapperIgnoreTarget(nameof(Entities.Screen.SeatMap))]
    public partial void MapUpdate(UpdateScreenInputDto source, Entities.Screen target);
}

[Mapper]
public partial class SeatTypeOutputMapper : IObjectMapper<Entities.SeatType, SeatTypeOutputDto>, ITransientDependency
{
    public partial SeatTypeOutputDto Map(Entities.SeatType source);
    public SeatTypeOutputDto Map(Entities.SeatType source, SeatTypeOutputDto destination) => Map(source);
}

[Mapper]
public partial class CreateSeatTypeInputMapper : IObjectMapper<CreateSeatTypeInputDto, Entities.SeatType>, ITransientDependency
{
    public partial Entities.SeatType Map(CreateSeatTypeInputDto source);
    public Entities.SeatType Map(CreateSeatTypeInputDto source, Entities.SeatType destination) => null;
}

[Mapper]
public partial class UpdateSeatTypeInputMapper : IObjectMapper<UpdateSeatTypeInputDto, Entities.SeatType>, ITransientDependency
{
    public partial Entities.SeatType Map(UpdateSeatTypeInputDto source);
    public Entities.SeatType Map(UpdateSeatTypeInputDto source, Entities.SeatType destination) { MapUpdate(source, destination); return destination; }
    [MapperIgnoreTarget(nameof(Entities.SeatType.Id))]
    public partial void MapUpdate(UpdateSeatTypeInputDto source, Entities.SeatType target);
}

[Mapper]
public partial class PricingRuleOutputMapper : IObjectMapper<Entities.PricingRule, PricingRuleOutputDto>, ITransientDependency
{
    public partial PricingRuleOutputDto Map(Entities.PricingRule source);
    public PricingRuleOutputDto Map(Entities.PricingRule source, PricingRuleOutputDto destination) => Map(source);
}

[Mapper]
public partial class CreatePricingRuleMapper : IObjectMapper<CreatePricingRuleDto, Entities.PricingRule>, ITransientDependency
{
    public partial Entities.PricingRule Map(CreatePricingRuleDto source);
    public Entities.PricingRule Map(CreatePricingRuleDto source, Entities.PricingRule destination) => null;
}

[Mapper]
public partial class EmployeeOutputMapper : IObjectMapper<Entities.Employee, EmployeeOutputDto>, ITransientDependency
{
    public partial EmployeeOutputDto Map(Entities.Employee source);
    public EmployeeOutputDto Map(Entities.Employee source, EmployeeOutputDto destination) => Map(source);
}

[Mapper]
public partial class CreateEmployeeInputMapper : IObjectMapper<CreateEmployeeInputDto, Entities.Employee>, ITransientDependency
{
    public partial Entities.Employee Map(CreateEmployeeInputDto source);
    public Entities.Employee Map(CreateEmployeeInputDto source, Entities.Employee destination) => null;
}

[Mapper]
public partial class GiftCodeOutputMapper : IObjectMapper<Entities.GiftCode, GiftCodes.Dtos.GiftCodeOutputDto>, ITransientDependency
{
    public partial GiftCodes.Dtos.GiftCodeOutputDto Map(Entities.GiftCode source);
    public GiftCodes.Dtos.GiftCodeOutputDto Map(Entities.GiftCode source, GiftCodes.Dtos.GiftCodeOutputDto destination) => Map(source);
}

[Mapper]
public partial class CreateGiftCodeMapper : IObjectMapper<GiftCodes.Dtos.CreateGiftCodeDto, Entities.GiftCode>, ITransientDependency
{
    public partial Entities.GiftCode Map(GiftCodes.Dtos.CreateGiftCodeDto source);
    public Entities.GiftCode Map(GiftCodes.Dtos.CreateGiftCodeDto source, Entities.GiftCode destination) => null;
}

[Mapper]
public partial class ShowtimeOutputMapper : IObjectMapper<Entities.Showtime, Showtimes.Dtos.ShowtimeOutputDto>, ITransientDependency
{
    public partial Showtimes.Dtos.ShowtimeOutputDto Map(Entities.Showtime source);
    public Showtimes.Dtos.ShowtimeOutputDto Map(Entities.Showtime source, Showtimes.Dtos.ShowtimeOutputDto destination) => Map(source);
}

[Mapper]
public partial class CreateShowtimeMapper : IObjectMapper<Showtimes.Dtos.CreateShowtimeDto, Entities.Showtime>, ITransientDependency
{
    public partial Entities.Showtime Map(Showtimes.Dtos.CreateShowtimeDto source);
    public Entities.Showtime Map(Showtimes.Dtos.CreateShowtimeDto source, Entities.Showtime destination) => null;
}

[Mapper]
public partial class NewsAndOffersOutputMapper : IObjectMapper<Entities.NewsAndOffers, NewsAndOffersOutputDto>, ITransientDependency
{
    public partial NewsAndOffersOutputDto Map(Entities.NewsAndOffers source);
    public NewsAndOffersOutputDto Map(Entities.NewsAndOffers source, NewsAndOffersOutputDto destination) => Map(source);
}

[Mapper]
public partial class CreateNewsAndOffersMapper : IObjectMapper<CreateNewsAndOffersDto, Entities.NewsAndOffers>, ITransientDependency
{
    public partial Entities.NewsAndOffers Map(CreateNewsAndOffersDto source);
    public Entities.NewsAndOffers Map(CreateNewsAndOffersDto source, Entities.NewsAndOffers destination) => null;
}

[Mapper]
public partial class UpdateNewsAndOffersMapper : IObjectMapper<UpdateNewsAndOffersDto, Entities.NewsAndOffers>, ITransientDependency
{
    public partial Entities.NewsAndOffers Map(UpdateNewsAndOffersDto source);
    public Entities.NewsAndOffers Map(UpdateNewsAndOffersDto source, Entities.NewsAndOffers destination) { MapUpdate(source, destination); return destination; }
    [MapperIgnoreTarget(nameof(Entities.NewsAndOffers.Id))]
    public partial void MapUpdate(UpdateNewsAndOffersDto source, Entities.NewsAndOffers target);
}
[Mapper]
public partial class CinemaAmenityOutputMapper : IObjectMapper<Entities.CinemaAmenity, CinemaAmenityOutputDto>, ITransientDependency
{
    public partial CinemaAmenityOutputDto Map(Entities.CinemaAmenity source);
    public CinemaAmenityOutputDto Map(Entities.CinemaAmenity source, CinemaAmenityOutputDto destination) => Map(source);
}

[Mapper]
public partial class CreateCinemaAmenityInputMapper : IObjectMapper<CreateCinemaAmenityInputDto, Entities.CinemaAmenity>, ITransientDependency
{
    public partial Entities.CinemaAmenity Map(CreateCinemaAmenityInputDto source);
    public Entities.CinemaAmenity Map(CreateCinemaAmenityInputDto source, Entities.CinemaAmenity destination) => null;
}

[Mapper]
public partial class UpdateCinemaAmenityInputMapper : IObjectMapper<UpdateCinemaAmenityInputDto, Entities.CinemaAmenity>, ITransientDependency
{
    public partial Entities.CinemaAmenity Map(UpdateCinemaAmenityInputDto source);
    public Entities.CinemaAmenity Map(UpdateCinemaAmenityInputDto source, Entities.CinemaAmenity destination) { MapUpdate(source, destination); return destination; }
    [MapperIgnoreTarget(nameof(Entities.CinemaAmenity.Id))]
    [MapperIgnoreTarget(nameof(Entities.CinemaAmenity.TenantId))]
    [MapperIgnoreTarget(nameof(Entities.CinemaAmenity.CinemaId))]
    public partial void MapUpdate(UpdateCinemaAmenityInputDto source, Entities.CinemaAmenity target);
}
