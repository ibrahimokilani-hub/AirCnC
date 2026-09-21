using HotelBooking.Api.ExceptionHandling;
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
      services.AddScoped<ICurrentUser, ICurrentUser>();
      
      services.AddSwaggerDocumentation();

      return services;
   }
}