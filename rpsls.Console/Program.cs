using Microsoft.Extensions.DependencyInjection;
using rpsls.IoC.Extensions;

namespace rpsls.Console
{
    public static class Program
    {
        private static void Main()
        {
            var serviceProvider = new ServiceCollection()
                .AddDefaults()
                .AddSingleton<GamePrompts>()
                .BuildServiceProvider();

            var gamePrompts = serviceProvider.GetService<GamePrompts>();
            gamePrompts.Play();
        }
    }
}