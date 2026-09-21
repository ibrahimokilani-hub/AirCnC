using HotelBooking.Api.Extensions;
using HotelBooking.Core;
using HotelBooking.Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddSerilogLogging();

builder.Services.AddPresentation()
                .AddInfrastructure(builder.Configuration)
                .AddCore();

var app = builder.Build();

app.UseApiPipeline();

app.Run();
