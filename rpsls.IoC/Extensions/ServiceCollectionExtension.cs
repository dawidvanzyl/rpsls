using Microsoft.Extensions.DependencyInjection;
using rpsls.Application;
using rpsls.Infrastructure.Repositories;

namespace rpsls.IoC.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection Application(this IServiceCollection services)
    {
        services.AddTransient<IGameService, GameService>();

        return services;
    }

    public static IServiceCollection Infrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IGameRepository, GameRepository>();

        return services;
    }
}