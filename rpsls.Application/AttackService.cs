using Microsoft.Extensions.Logging;
using rpsls.Domain.Algorithms.Models;
using rpsls.Domain.Algorithms.RecencyBias;
using rpsls.Domain.Algorithms.Weight;
using rpsls.Domain.Modules;
using rpsls.Entities;
using rpsls.Entities.Enums;

namespace rpsls.Application;

public interface IAttackService
{
    AttackTypes CalculateAttack();
}

public class AttackService(
    IMatchResultModule matchResultModule,
    IRuleModule ruleModule,
    IWeightAlgorithm<WeightedSmoothedCountInput> weightAlgorithm,
    IRecencyBiasAlgorithm<WeightedRecencyBiasInput> recencyBiasAlgorithm,
    ILogger<AttackService> logger) : IAttackService
{
    public AttackTypes CalculateAttack()
    {
        var matchResults = matchResultModule.GetAll();

        if (!matchResults.Any())
        {
            return (AttackTypes)Random.Shared.Next(1, 4);
        }

        var totalCount = matchResults.Count;
        var lastMatchResult = matchResults[matchResults.Count - 1];

        var attackPercentages = matchResults
            .GroupBy((matchResult) => matchResult.P1Attack)
            .Select(attackGroup =>
                new
                {
                    Attack = attackGroup.Key,
                    WeighedPercentage = CalculateWeightedPercentage(attackGroup, totalCount, lastMatchResult)
                })
            .OrderByDescending(a => a.WeighedPercentage)
            .ToList();

        var player1Prediction = attackPercentages[0].Attack;

        return ruleModule.GetAttackToBeat(player1Prediction);
    }

    private decimal CalculateWeightedPercentage(IGrouping<AttackTypes, MatchResult> attackGroup, int totalCount, MatchResult lastMatchResult)
    {
        var count = attackGroup.Count();

        logger.LogDebug("Attack Group: {AttackGroup}", attackGroup.Key);
        logger.LogDebug("Total Count: {TotalCount}", totalCount);
        logger.LogDebug("Count: {Count}", count);

        var weighedPercentage = weightAlgorithm.CalculateWeightedPercentage(
            new WeightedSmoothedCountInput
            {
                GroupCount = count,
                TotalCount = totalCount
            });

        if (lastMatchResult.P1Attack == attackGroup.Key)
        {
            weighedPercentage = recencyBiasAlgorithm.ApplyRecencyBias(
                new WeightedRecencyBiasInput
                {
                    LastResult = lastMatchResult.Result,
                    WeighedPercentage = weighedPercentage
                });
        }

        return weighedPercentage;
    }
}