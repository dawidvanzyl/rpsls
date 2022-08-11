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

        public static Attack From(AttackValue attackValue, IList<DefeatedByValue> defeatedByValues)
        {
            return new Attack(attackValue, defeatedByValues);
        }

        public int CompareTo(Attack other)
        {
            var isTied = AttackValue.Equals(other.AttackValue);
            if (isTied)
            {
                return (int)AttackResult.Tied;
            }

            var hasLost = defeatedByValues.Any(defeatConfig => defeatConfig.AttackValue.Equals(other.AttackValue));
            return hasLost
                ? (int)AttackResult.Lost
                : (int)AttackResult.Won;
        }

        public bool Equals([AllowNull] Attack other)
        {
            return other != null
                && AttackValue.Equals(other.AttackValue)
                && defeatedByValues.All(defeatedByValue => other.defeatedByValues.Contains(defeatedByValue));
        }

        public string GetDefeatMessage(AttackValue attackValue)
        {
            if (!defeatedByValues.Any(defeatConfig => defeatConfig.AttackValue.Equals(attackValue)))
            {
                throw new ArgumentOutOfRangeException(attackValue.Name, "Defeated by value not found");
            }

            var message = defeatedByValues
                .First(defeatConfig => defeatConfig.AttackValue.Equals(attackValue))
                .Message;

            return message;
        }

        public override string ToString()
        {
            return AttackValue.Name;
        }
    }
}