using FluentValidation;
using MediatR;
using TaskManager.Application.Common.Behaviors;
using TaskManager.Application.Features.Auth.Commands;

namespace TaskManager.API.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(RegisterCommand).Assembly));

        services.AddValidatorsFromAssembly(typeof(RegisterCommand).Assembly);

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }
}