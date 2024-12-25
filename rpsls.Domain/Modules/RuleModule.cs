using rpsls.Entities;
using rpsls.Entities.Enums;
using rpsls.Infrastructure.Repositories;

namespace rpsls.Domain.Modules;

public interface IRuleModule
{
    AttackTypes GetAttackToBeat(AttackTypes attack);
}

public class RuleModule(IRuleRepository ruleRepository)
    : IRuleModule
{
    private readonly Lazy<IList<Rule>> _rules =
        new Lazy<IList<Rule>>(
            () => ruleRepository
                    .GetAllAsync()
                    .GetAwaiter()
                    .GetResult());

    public AttackTypes GetAttackToBeat(AttackTypes attack)
    {
        var rule = GetValue().Single(rs => rs.Beats == attack);
        return rule.Attack;
    }

    private IList<Rule> GetValue()
    {
        return _rules == null
            ? throw new InvalidOperationException(nameof(_rules))
            : _rules.Value;
    }
}