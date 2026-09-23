using FluentValidation;
using HotelBooking.Core.Application.Common.Paging;

namespace HotelBooking.Core.Application.Features.Rooms.Queries.GetRoomsList;

public sealed class GetRoomsListQueryValidator : AbstractValidator<GetRoomsListQuery>
{
    public GetRoomsListQueryValidator()
    {
        RuleFor(query => query.Page).ValidPage();
        RuleFor(query => query.PageSize).ValidPageSize();
        RuleFor(query => query.Search).MaximumLength(20);
        RuleFor(query => query.SortBy).OneOf(GetRoomsListQuery.SortableColumns);
        RuleFor(query => query.SortDirection).ValidSortDirection();
    }
}