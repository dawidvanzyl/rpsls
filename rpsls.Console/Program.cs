using Microsoft.Extensions.DependencyInjection;
using rpsls.Infrastructure;

namespace rpsls.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = ServiceCollectionFactory
                .Create()
                .BuildServiceProvider();
        }
    }
}
