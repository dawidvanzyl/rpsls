using rpsls.Domain.Values;
using rpsls.Infrastructure.Algorithms;
using rpsls.Infrastructure.Repositories;
using System;
using System.Collections.Generic;

namespace rpsls.Application
{
    public class GameServiceFactory
    {
        private readonly IDictionary<string, GameService> gameServices;

        public GameServiceFactory(ISettingsRepository settingsRepository, IMatchRepository matchRepository, IChallangeAlgorithmStrategy challangeAlgorithmStrategy)
        {
            var gameValues = settingsRepository.GetGames();

            gameServices = new Dictionary<string, GameService>();
            foreach (var gameValue in gameValues)
            {
                gameServices.Add(gameValue.Name, new GameService(gameValue, matchRepository, challangeAlgorithmStrategy));
            }
        }

        public GameService Create(GameValue gameValue)
        {
            var gameServiceRegistered = gameServices.ContainsKey(gameValue.Name);
            if (!gameServiceRegistered)
            {
                throw new InvalidOperationException($"Game value {gameValue.Name} has not been registered.");
            }

            var gameService = gameServices[gameValue.Name];
            return gameService;
        }
    }
}