using rpsls.Domain.Algorithms.Models;
using rpsls.Domain.Modules;
using rpsls.Entities.Enums;
using System.Collections.Immutable;

namespace rpsls.Domain.Algorithms
{
    public class CustomAlgorithm : IAlgorithm
    {
        private readonly IGameModule _gameModule;
        private ImmutableHashSet<RuleSet> _ruleSet;

        public CustomAlgorithm(IGameModule gameModule)
        {
            _gameModule = gameModule;
        }

        public AttackTypes CalculateAttack()
        {
            var playerAttacks = _gameModule.GetAll();

            if (!playerAttacks.Any())
            {
                return (AttackTypes)Random.Shared.Next(1, 3);
            }

            var attackPercentages = playerAttacks
                .GroupBy((matchResult) => matchResult.Player1)
                .Select(attackGroup =>
                {
                    var keyAttackCount = playerAttacks
                        .Where(pa => pa.Player1 == attackGroup.Key)
                        .Sum(pa => pa.AttackCount);

                    var modifier = Math.Round(1m * attackGroup.Count() / keyAttackCount, 2);
                    if (playerAttacks.LastOrDefault()?.Player1 == attackGroup.Key)
                    {
                        modifier += 0.2m;
                    }

                    var percentage = 100.0m * attackGroup.Count() / playerAttacks.Count();
                    var modifiedPercentage = Math.Round(percentage * modifier, 2);

                    return new { attackGroup.Key, Percentage = modifiedPercentage };
                })
                .OrderByDescending(a => a.Percentage);

            var player1Prediction = attackPercentages.First().Key;

            var winningRule = _ruleSet.FirstOrDefault(rs => rs.Beats == player1Prediction);
            return winningRule == null
                ? (AttackTypes)Random.Shared.Next(1, 3)
                : winningRule.Attack;
        }

        public void SetupRuleSet()
        {
            var matchResults = _gameModule.GetAll();

            _ruleSet = matchResults
                .Where(matchResult => matchResult.Result != ResultTypes.Draw)
                .Select(matchResult =>
                {
                    return matchResult.Result == ResultTypes.Win
                        ? new { p1 = matchResult.Player1, p2 = matchResult.Player2 }
                        : new { p1 = matchResult.Player2, p2 = matchResult.Player1 };
                })
                .Distinct()
                .Select(a => new RuleSet { Attack = a.p1, Beats = a.p2 })
                .ToImmutableHashSet();
        }
    }
}