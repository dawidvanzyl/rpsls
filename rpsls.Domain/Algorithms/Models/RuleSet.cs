using rpsls.Entities.Enums;

namespace rpsls.Domain.Algorithms.Models
{
    internal class RuleSet : IEquatable<RuleSet>
    {
        internal AttackTypes Attack { get; init; }
        internal AttackTypes Beats { get; init; }

        public bool Equals(RuleSet? other)
        {
            return other != null
                && other.Attack == Attack
                && other.Beats == Beats;
        }
    }
}