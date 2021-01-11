using rpsls.Domain.Enums;
using rpsls.Domain.Values;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace rpsls.Domain
{
    public sealed class Attack : IComparable<Attack>, IEquatable<Attack>
    {
        private readonly IList<DefeatedByValue> defeatedByValues;

        private Attack(AttackValue attackValue, IList<DefeatedByValue> defeatedByValues)
        {
            AttackValue = attackValue;
            this.defeatedByValues = defeatedByValues;
        }

        public AttackValue AttackValue { get; }

        public string GetDefeatMessage(AttackValue attackValue)
        {
            if (!defeatedByValues.Any(defeatConfig => defeatConfig.AttackValue.Equals(attackValue)))
            {
                throw new ArgumentOutOfRangeException(attackValue.Name, "Defeated by value not found");
            }

            return defeatedByValues.First(defeatConfig => defeatConfig.AttackValue.Equals(attackValue))
                .Message;
        }

        public int CompareTo(Attack other)
        {
            if (AttackValue.Equals(other.AttackValue))
            {
                return (int)AttackResult.Tie;
            }

            if (defeatedByValues.Any(defeatConfig => defeatConfig.AttackValue.Equals(other.AttackValue)))
            {
                return (int)AttackResult.Lose;
            }

            return (int)AttackResult.Win;
        }

        public bool Equals([AllowNull] Attack other)
        {
            if (other == null) return false;

            return AttackValue.Equals(other.AttackValue)
                && defeatedByValues.All(defeatedByValue => other.defeatedByValues.Contains(defeatedByValue));
        }

        public static Attack From(AttackValue attackValue, IList<DefeatedByValue> defeatedByValues)
        {
            return new Attack(attackValue, defeatedByValues);
        }
    }
}
