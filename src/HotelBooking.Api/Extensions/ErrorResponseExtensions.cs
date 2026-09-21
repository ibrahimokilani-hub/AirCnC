using HotelBooking.Contracts.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace HotelBooking.Api.Extensions;

public static class ErrorResponseExtensions
{
    private const string ValidationMessage = "One or more validation errors occurred.";

    public static IMvcBuilder AddErrorResponses(this IMvcBuilder builder)
    {
        builder.ConfigureApiBehaviorOptions(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState
                    .Where(entry => entry.Value is { Errors.Count: > 0 })
                    .ToDictionary(
                        entry => entry.Key,
                        entry => entry.Value!.Errors
                            .Select(error => string.IsNullOrWhiteSpace(error.ErrorMessage)
                                ? "The value is invalid."
                                : error.ErrorMessage)
                            .ToArray());

                var errorResponse = new ErrorResponse(
                    StatusCodes.Status400BadRequest,
                    ValidationMessage,
                    errors
                );

                return new BadRequestObjectResult(errorResponse);
            };
        });

        return builder;
    }

   public static IApplicationBuilder UseErrorResponsesForEmptyStatusCodes(this IApplicationBuilder app)
    {
        return app.UseStatusCodePages(async context =>
        {
            var response = context.HttpContext.Response;
            var message = ReasonPhrases.GetReasonPhrase(response.StatusCode);

            await response.WriteAsJsonAsync(new ErrorResponse(response.StatusCode, message));
        });
    }
}
