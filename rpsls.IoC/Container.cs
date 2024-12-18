using Microsoft.Extensions.DependencyInjection;
using rpsls.IoC.Extensions;

namespace rpsls.IoC;

public static class Container
{
    public static IServiceCollection Default(this IServiceCollection services)
    {
        return services
            .Application()
            .Domain()
            .Infrastructure();
    }
}