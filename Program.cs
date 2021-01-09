using Microsoft.Extensions.Configuration;
using rpsls.Attacks;
using System;
using System.IO;
using System.Linq;

namespace rpsls
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            var configuration = builder.Build();

            var attackFactory = new AttackFactory(configuration);
            
            var attacks = attackFactory.GetAttacks();
            var playerAttack = attacks.First();

            var gameFactory = new GameFactory(configuration);
            var game = gameFactory.GetGame(playerAttack, attacks.ToArray());
            game.Play();
            game.PrintStats();
            Console.Read();
        }
    }
}
