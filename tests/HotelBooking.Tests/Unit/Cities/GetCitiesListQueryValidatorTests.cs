using HotelBooking.Core.Application.Common.Paging;
using HotelBooking.Core.Application.Features.Cities.Queries.GetCitiesList;

namespace HotelBooking.Tests.Unit.Cities;

/// <summary>
/// Every list endpoint builds its paging and sorting rules from the same ListRuleExtensions,
/// so the shared rules are tested once here, through the cities list.
/// </summary>
public sealed class GetCitiesListQueryValidatorTests
{
    private readonly GetCitiesListQueryValidator _validator = new();

    private static GetCitiesListQuery Valid() => new(1, 25, null, "name", "asc");

    private string[] FailedProperties(GetCitiesListQuery query) =>
        _validator.Validate(query).Errors.Select(error => error.PropertyName).Distinct().ToArray();

    [Fact]
    public void Validate_ValidQuery_HasNoErrors() =>
        Assert.True(_validator.Validate(Valid()).IsValid);

    /// <summary>Sorting is optional: no sortBy means the handler's default order.</summary>
    [Fact]
    public void Validate_WithoutSorting_HasNoErrors() =>
        Assert.True(_validator.Validate(Valid() with { SortBy = null, SortDirection = null }).IsValid);

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_PageBelowOne_HasPageError(int page) =>
        Assert.Contains(nameof(GetCitiesListQuery.Page), FailedProperties(Valid() with { Page = page }));

    [Theory]
    [InlineData(0)]
    [InlineData(ListRules.MaxPageSize + 1)]
    public void Validate_PageSizeOutOfRange_HasPageSizeError(int pageSize) =>
        Assert.Contains(nameof(GetCitiesListQuery.PageSize), FailedProperties(Valid() with { PageSize = pageSize }));

    [Fact]
    public void Validate_SearchOverTheLimit_HasSearchError()
    {
        var query = Valid() with { Search = new string('a', ListRules.MaxSearchLength + 1) };

        Assert.Contains(nameof(GetCitiesListQuery.Search), FailedProperties(query));
    }

    [Fact]
    public void Validate_UnknownSortColumn_HasSortByError() =>
        Assert.Contains(nameof(GetCitiesListQuery.SortBy), FailedProperties(Valid() with { SortBy = "population" }));

    /// <summary>The allowed column names are matched without case.</summary>
    [Fact]
    public void Validate_SortColumnInAnotherCase_HasNoError() =>
        Assert.DoesNotContain(nameof(GetCitiesListQuery.SortBy), FailedProperties(Valid() with { SortBy = "CreatedAt" }));

    [Fact]
    public void Validate_SortDirectionThatIsNotAscOrDesc_HasDirectionError() =>
        Assert.Contains(
            nameof(GetCitiesListQuery.SortDirection),
            FailedProperties(Valid() with { SortDirection = "sideways" }));
}