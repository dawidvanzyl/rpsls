using rpsls.Domain.Values;

namespace rpsls.Application
{
    public interface IAttackStrategyFactory
    {
        IAttackStrategy CreateRandomAttackAlgorithm(GameValue gameValue);
        IAttackStrategy CreateMLAttackAlgorithm(GameValue gameValue);
    }
}
