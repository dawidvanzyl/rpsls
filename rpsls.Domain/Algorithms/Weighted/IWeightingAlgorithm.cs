using rpsls.Entities;
using rpsls.Entities.Enums;

namespace rpsls.Domain.Algorithms.Weighted;

public interface IWeightingAlgorithm
{
    IDictionary<AttackTypes, decimal> CalculateWeights(IList<Round> history);
}