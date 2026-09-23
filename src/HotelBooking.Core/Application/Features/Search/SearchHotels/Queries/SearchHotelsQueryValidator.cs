using FluentValidation;
using HotelBooking.Core.Application.Common.Paging;
using HotelBooking.Core.Domain.Entities;
using HotelBooking.Core.Domain.Enums;
using HotelBooking.Core.Domain.ValueObjects;

namespace HotelBooking.Core.Application.Features.Search.SearchHotels.Queries;

public sealed class SearchHotelsQueryValidator : AbstractValidator<SearchHotelsQuery>
{
    public const int MaxRooms = 10;
    public const int MaxGuests = 20;

    public SearchHotelsQueryValidator(TimeProvider timeProvider)
    {
        var today = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);

        RuleFor(query => query.CheckIn)
            .GreaterThanOrEqualTo(today).WithMessage("Check-in can't be in the past.")
            .When(query => query.CheckIn is not null);

        RuleFor(query => query.CheckOut)
            .GreaterThan(query => query.CheckIn ?? today).WithMessage("Check-out must be after check-in.")
            .When(query => query.CheckOut is not null);

        RuleFor(query => query)
            .Must(query => Nights(query, today) <= DateRange.MaxNights)
            .WithName("CheckOut")
            .WithMessage($"A stay can be at most {DateRange.MaxNights} nights.")
            .When(query => query.CheckOut is not null);

        RuleFor(query => query.Adults).InclusiveBetween(1, MaxGuests);
        RuleFor(query => query.Children).InclusiveBetween(0, MaxGuests);
        RuleFor(query => query.Rooms).InclusiveBetween(1, MaxRooms);
        RuleFor(query => query.Rooms)
            .LessThanOrEqualTo(query => query.Adults)
            .WithMessage("Every room needs at least one adult.");

        RuleFor(query => query.MinPrice).GreaterThanOrEqualTo(0).When(query => query.MinPrice is not null);
        RuleFor(query => query.MaxPrice)
            .GreaterThanOrEqualTo(query => query.MinPrice ?? 0)
            .When(query => query.MaxPrice is not null);

        RuleFor(query => query.MinStars).InclusiveBetween(Hotel.MinStars, Hotel.MaxStars).When(query => query.MinStars is not null);
        RuleFor(query => query.HotelType).IsEnumName(typeof(HotelType), caseSensitive: false).When(query => query.HotelType is not null);
        RuleFor(query => query.AmenityIds).Must(ids => ids.Count <= 20).WithMessage("At most 20 amenities.");
        RuleFor(query => query.Query).MaximumLength(100);

        RuleFor(query => query.SortBy).OneOf(SearchHotelsQuery.SortableColumns);
        RuleFor(query => query.SortDirection).ValidSortDirection();
        RuleFor(query => query.Page).ValidPage();
        RuleFor(query => query.PageSize).InclusiveBetween(1, 50);
    }

    private static int Nights(SearchHotelsQuery query, DateOnly today) =>
        query.CheckOut!.Value.DayNumber - (query.CheckIn ?? today).DayNumber;
}
