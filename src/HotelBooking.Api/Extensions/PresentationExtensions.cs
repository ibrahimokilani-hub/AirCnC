using HotelBooking.Api.ExceptionHandling;

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
      services.AddSwaggerDocumentation();

      return services;
   }
}