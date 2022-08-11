using Microsoft.Extensions.DependencyInjection;
using rpsls.Infrastructure.Factories;

namespace rpsls.Console
{
    public static class Program
    {
        private static void Main()
        {
            var serviceProvider = ServiceCollectionFactory
                .Create()
                .AddSingleton<GamePrompts>()
                .BuildServiceProvider();

            var gamePrompts = serviceProvider.GetService<GamePrompts>();
            gamePrompts.Play();
        }
    }
}