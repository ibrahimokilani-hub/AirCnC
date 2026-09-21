using HotelBooking.Api.Extensions;
using HotelBooking.Core;
using HotelBooking.Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.AddSerilogLogging();

builder.Services
    .AddControllers()
    .AddErrorResponses();

builder.Services.AddSwaggerDocumentation();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddCore();


var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseErrorResponsesForEmptyStatusCodes();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerDocumentation();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
