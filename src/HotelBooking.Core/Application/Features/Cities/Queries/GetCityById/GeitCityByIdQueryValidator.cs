using FluentValidation;

namespace HotelBooking.Core.Application.Features.Cities.Queries.GetCityById;

public class GetCityByIdQueryValidator : AbstractValidator<GetCityByIdQuery>
{
    public GetCityByIdQueryValidator()
    {
        RuleFor(c => c.Id)
            .NotEmpty().WithMessage("City Id is required");
    }   
}