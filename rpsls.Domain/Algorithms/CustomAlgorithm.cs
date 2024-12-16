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
            var playerAttacks = _gameModule.GetPlayer1Attacks();

            var attackPercentages = playerAttacks
                .GroupBy((attackType) => attackType)
                .Select(attackGroup => new { AttackType = attackGroup.Key, Percentage = Math.Round(100.0m * attackGroup.Count() / playerAttacks.Count(), 2) })
                .OrderByDescending(a => a.Percentage);

            var player1Prediction = attackPercentages.First().AttackType;

            return _ruleSet
                .First(rs => rs.Beats == player1Prediction)
                .Attack;
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