using FluentValidation;

namespace HotelBooking.Core.Application.Common.Paging;

public static class ListRuleExtensions
{
    public static IRuleBuilderOptions<T, int> ValidPage<T>(this IRuleBuilder<T, int> rule) =>
        rule.GreaterThanOrEqualTo(1)
            .WithMessage("Page must be 1 or greater.");

    public static IRuleBuilderOptions<T, int> ValidPageSize<T>(this IRuleBuilder<T, int> rule) =>
        rule.InclusiveBetween(1, ListRules.MaxPageSize);

    public static IRuleBuilderOptions<T, string?> ValidSearch<T>(this IRuleBuilder<T, string?> rule) =>
        rule.MaximumLength(ListRules.MaxSearchLength);

    public static IRuleBuilderOptions<T, string?> OneOf<T>(this IRuleBuilder<T, string?> rule, string[] allowed) =>
        rule.Must(value => value is null || allowed.Contains(value, StringComparer.OrdinalIgnoreCase))
            .WithMessage($"Must be one of: {string.Join(", ", allowed)}.");

    public static IRuleBuilderOptions<T, string?> ValidSortDirection<T>(this IRuleBuilder<T, string?> rule) =>
        rule.OneOf(ListRules.SortDirections)
            .WithMessage("Must be asc or desc.");
}