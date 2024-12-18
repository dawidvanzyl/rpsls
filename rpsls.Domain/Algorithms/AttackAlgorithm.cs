using Microsoft.Extensions.Logging;
using rpsls.Domain.Algorithms.Models;
using rpsls.Domain.Modules;
using rpsls.Entities.Enums;
using System.Collections.Immutable;

namespace rpsls.Domain.Algorithms
{
    public class FullHistoryAlgorithm : IAlgorithm
    {
        private readonly IGameModule _gameModule;
        private readonly ILogger<FullHistoryAlgorithm> _logger;
        private ImmutableHashSet<RuleSet> _ruleSet;

        public FullHistoryAlgorithm(IGameModule gameModule, ILogger<FullHistoryAlgorithm> logger)
        {
            _gameModule = gameModule;
            _logger = logger;
        }

        public AttackTypes CalculateAttack()
        {
            var fullHistory = _gameModule.GetFullHistory();

            if (!fullHistory.Any())
            {
                return (AttackTypes)Random.Shared.Next(1, 3);
            }

            var attackPercentages = fullHistory
                .AsParallel()
                .GroupBy((matchResult) => matchResult.P1Attack)
                .Select(attackGroup =>
                {
                    var totalCount = fullHistory.Count();
                    var consecutiveRepeatsSum = attackGroup.Sum(pa => pa.ConsecutiveRepeats);
                    var count = attackGroup.Count();

                    var modifier = CalculateModifier(consecutiveRepeatsSum, count);
                    var percentage = CalculatePercentage(totalCount, count);
                    var modifiedPercentage = Math.Round(percentage * modifier, 2);

                    _logger.LogDebug($"Attack: {attackGroup.Key}, Count: {count}, Modifier: {modifier}, Percentage: {modifiedPercentage}");

                    return new { Attack = attackGroup.Key, Percentage = modifiedPercentage };
                })
                .OrderByDescending(a => a.Percentage)
                .ToList();

            var player1Prediction = attackPercentages[0].Attack;

            var winningRule = _ruleSet.FirstOrDefault(rs => rs.Beats == player1Prediction);
            return winningRule == null
                ? (AttackTypes)Random.Shared.Next(1, 3)
                : winningRule.Attack;
        }

        public void SetupRuleSet()
        {
            var matchResults = _gameModule.GetFullHistory();

            _ruleSet = matchResults
                .Where(matchResult => matchResult.Result != ResultTypes.Draw)
                .Select(matchResult =>
                {
                    return matchResult.Result == ResultTypes.Win
                        ? new { p1 = matchResult.P1Attack, p2 = matchResult.P2Attack }
                        : new { p1 = matchResult.P2Attack, p2 = matchResult.P1Attack };
                })
                .Distinct()
                .Select(a => new RuleSet { Attack = a.p1, Beats = a.p2 })
                .ToImmutableHashSet();
        }

        private static decimal CalculateModifier(int consecutiveRepeatsSum, int count)
        {
            return consecutiveRepeatsSum == 0 ? 1m : Math.Round(1m * count / consecutiveRepeatsSum, 2);
        }

        private static decimal CalculatePercentage(int totalCount, int count)
        {
            return Math.Round(100.0m * count / totalCount, 2);
        }
    }
}