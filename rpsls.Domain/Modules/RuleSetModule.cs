using rpsls.Entities;
using rpsls.Entities.Enums;
using rpsls.Infrastructure.Repositories;

namespace rpsls.Domain.Modules;

public interface IRuleSetModule
{
    AttackTypes GetAttackToBeat(AttackTypes attack);
}

public class RuleSetModule(IRuleSetRepository ruleSetRepository) : IRuleSetModule
{
    private readonly Lazy<IList<RuleSet>> _ruleSet =
        new Lazy<IList<RuleSet>>(
            () => ruleSetRepository
                    .GetAsync()
                    .GetAwaiter()
                    .GetResult());

    public AttackTypes GetAttackToBeat(AttackTypes attack)
    {
        var rule = GetValue().Single(rs => rs.Beats == attack);
        return rule.Attack;
    }

    private IList<RuleSet> GetValue()
    {
        return _ruleSet == null
            ? throw new InvalidOperationException(nameof(_ruleSet))
            : _ruleSet.Value;
    }
}