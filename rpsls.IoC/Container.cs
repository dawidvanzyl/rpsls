using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using rpsls.IoC.Extensions;

namespace rpsls.IoC;

public static class Container
{
    public static IServiceCollection Create()
    {
        var services = new ServiceCollection();

        IConfiguration configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        services.AddSingleton(configuration);

        return services
            .Application()
            .Infrastructure();
    }
}