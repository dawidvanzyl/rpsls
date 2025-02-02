using rpsls.Entities.Enums;

namespace rpsls.Domain.Algorithms.RecencyBiases;

public interface IRecencyBiasAlgorithm
{
    public IDictionary<AttackTypes, decimal> ApplyRecencyBias(IDictionary<AttackTypes, decimal> attackPercentages);
}