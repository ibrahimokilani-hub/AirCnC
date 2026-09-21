using HotelBooking.Api.ExceptionHandling;
using HotelBooking.Api.Services;
using HotelBooking.Core.Application.Abstractions;

namespace HotelBooking.Api.Extensions;

public static class PresentationExtensions
{
   public static IServiceCollection AddPresentation(this IServiceCollection services)
   {
      services
         .AddControllers()
         .AddErrorResponses();

      services.AddExceptionHandler<GlobalExceptionHandler>();
      
      services.AddProblemDetails();
      
      services.AddHttpContextAccessor();
      services.AddScoped<ICurrentUser, CurrentUser>();
      
      services.AddSwaggerDocumentation();

      return services;
   }
}