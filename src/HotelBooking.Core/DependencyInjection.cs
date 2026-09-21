using System.Reflection;
using FluentValidation;
using HotelBooking.Core.Application.Abstractions.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace HotelBooking.Core;

public static class DependencyInjection
{
    
    public static readonly Type[] HandlerContracts = [
        
        typeof(ICommandHandler<>),
        typeof(ICommandHandler<,>),
        typeof(IQueryHandler<,>)
    ];
    
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddValidatorsFromAssembly(assembly);
        services.AddHandlersFromAssembly(assembly);

        return services;
    }

    private static IServiceCollection AddHandlersFromAssembly(
        this IServiceCollection services,
        Assembly assembly)
    {

        var implementations = assembly.GetTypes()
            .Where(t =>
                t.IsClass &&
                !t.IsAbstract &&
                !t.ContainsGenericParameters);

        foreach (var implementation in implementations)
        {
            var handlerInterfaces = implementation
                .GetInterfaces()
                .Where(i =>
                    i.IsGenericType &&
                    HandlerContracts.Contains(i.GetGenericTypeDefinition()));

            foreach (var handlerInterface in handlerInterfaces)
            {
                services.AddScoped(handlerInterface, implementation);
            }
        }

        return services;
    }
}