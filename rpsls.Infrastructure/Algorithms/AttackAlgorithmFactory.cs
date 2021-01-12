using rpsls.Application;
using rpsls.Application.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace rpsls.Infrastructure.Algorithms
{
    public class AttackAlgorithmFactory : IAttackStrategyFactory
    {
        private readonly IDictionary<string, IAttackStrategy> attackStrategies;

        public AttackAlgorithmFactory(ISettingsRepository settingsRepository)
        {
            var gameValues = settingsRepository.GetGameValues();

            attackStrategies = new Dictionary<string, IAttackStrategy>();
            foreach (var gameValue in gameValues)
            {
                attackStrategies.Add($"{gameValue.Name}_{nameof(RandomAttackAlgorithm)}", new RandomAttackAlgorithm(gameValue.Attacks.ToArray()));
                attackStrategies.Add($"{gameValue.Name}_{nameof(MLAttackAlgorithm)}", new MLAttackAlgorithm(gameValue.Attacks.ToArray()));
            }
        }

        public IAttackStrategy CreateMLAttackAlgorithm(Domain.Values.GameValue gameValue)
        {
            string mlAttackAlgorithmKey = $"{gameValue.Name}_{nameof(MLAttackAlgorithm)}";

            bool gameServiceRegistered = attackStrategies.ContainsKey(mlAttackAlgorithmKey);
            if (!gameServiceRegistered)
            {
                throw new InvalidOperationException($"Game value {mlAttackAlgorithmKey} has not been registered.");
            }

            return attackStrategies[mlAttackAlgorithmKey];
        }

        public IAttackStrategy CreateRandomAttackAlgorithm(Domain.Values.GameValue gameValue)
        {
            string randomAttackAlgorithmKey = $"{gameValue.Name}_{nameof(RandomAttackAlgorithm)}";

            bool gameServiceRegistered = attackStrategies.ContainsKey(randomAttackAlgorithmKey);
            if (!gameServiceRegistered)
            {
                throw new InvalidOperationException($"Game value {randomAttackAlgorithmKey} has not been registered.");
            }

            return attackStrategies[randomAttackAlgorithmKey];
        }
    }
}
