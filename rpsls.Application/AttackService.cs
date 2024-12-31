using Microsoft.Extensions.Logging;
using rpsls.Domain.Algorithms.Contexts;
using rpsls.Domain.Algorithms.Modifiers;
using rpsls.Domain.Algorithms.RecencyBiases;
using rpsls.Domain.Modules;
using rpsls.Entities.Enums;

namespace rpsls.Application;

public interface IAttackService
{
    AttackTypes CalculateAttack();
}

public class AttackService(
    IMatchModule matchModule,
    IRuleModule ruleModule,
    IModifierAlgorithm modifierAlgorithm,
    IRecencyBiasAlgorithm recencyBiasAlgorithm,
    ILogger<AttackService> logger) : IAttackService
{
    public AttackTypes CalculateAttack()
    {
        var matchHistory = matchModule.GetAll();

        logger.LogDebug("Total Count: {TotalCount}", matchHistory.Count);

        if (!matchHistory.Any())
        {
            return (AttackTypes)Random.Shared.Next(1, 4);
        }

        var context = new AlgorithmContext
        {
            LastMatch = matchHistory[matchHistory.Count - 1],
            MatchHistory = matchHistory,
            TotalCount = matchHistory.Count
        };

        var attackPercentages = recencyBiasAlgorithm.ApplyRecencyBias(
            modifierAlgorithm.CalculatePercentages(matchHistory, context),
            context);

        var player1Prediction = attackPercentages
            .OrderByDescending(kv => kv.Value)
            .First();

        return ruleModule.GetAttackToBeat(player1Prediction.Key);
    }
}