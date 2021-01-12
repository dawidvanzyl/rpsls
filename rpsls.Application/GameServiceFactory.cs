using rpsls.Application.Repositories;
using rpsls.Domain.Values;
using System;
using System.Collections.Generic;

namespace rpsls.Application
{
    public class GameServiceFactory
    {
        private readonly IDictionary<string, GameService> gameServices;

        public GameServiceFactory(ISettingsRepository settingsRepository, IMatchRepository matchRepository, IAttackStrategyFactory attackStrategyFactory)
        {
            var gameValues = settingsRepository.GetGameValues();

            gameServices = new Dictionary<string, GameService>();
            foreach (var gameValue in gameValues)
            {
                gameServices.Add(gameValue.Name, new GameService(gameValue, matchRepository, attackStrategyFactory));
            }
        }

        public GameService Create(GameValue gameValue)
        {
            bool gameServiceRegistered = gameServices.ContainsKey(gameValue.Name);
            if (!gameServiceRegistered)
            {
                throw new InvalidOperationException($"Game value {gameValue.Name} has not been registered.");
            }

            return gameServices[gameValue.Name];
        }
    }
}
