using rpsls.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace rpsls.Domain.Values
{
    public sealed class ChallengeValue : IComparable<ChallengeValue>, IEquatable<ChallengeValue>
    {
        private readonly IList<DefeatedByValue> defeatedByValues;

        private ChallengeValue(AttackValue attack, IList<DefeatedByValue> defeatedByValues)
        {
            Attack = attack;
            this.defeatedByValues = defeatedByValues;
        }

        public AttackValue Attack { get; }

        public static ChallengeValue From(AttackValue attack, IList<DefeatedByValue> defeatedByValues)
        {
            return new ChallengeValue(attack, defeatedByValues);
        }

        public static bool operator !=(ChallengeValue left, ChallengeValue right)
        {
            return !(left == right);
        }

        public static bool operator <(ChallengeValue left, ChallengeValue right)
        {
            return left is null
                ? right is null
                : left.CompareTo(right) < 0;
        }

        public static bool operator <=(ChallengeValue left, ChallengeValue right)
        {
            return left is null
                ? right is null
                : left.Equals(right) || left.CompareTo(right) < 0;
        }

        public static bool operator ==(ChallengeValue left, ChallengeValue right)
        {
            return left is null
                ? right is null
                : left.Equals(right);
        }

        public static bool operator >(ChallengeValue left, ChallengeValue right)
        {
            return left is null
                ? right is null
                : left.CompareTo(right) > 0;
        }

        public static bool operator >=(ChallengeValue left, ChallengeValue right)
        {
            return left is null
                ? right is null
                : left.Equals(right) || left.CompareTo(right) > 0;
        }

        public int CompareTo(ChallengeValue other)
        {
            var isTied = Attack.Equals(other.Attack);
            if (isTied)
            {
                return (int)ChallengeResult.Tied;
            }

            var hasLost = defeatedByValues.Any(defeatConfig => defeatConfig.Attack.Equals(other.Attack));
            return hasLost
                ? (int)ChallengeResult.Lost
                : (int)ChallengeResult.Won;
        }

        public bool Equals([AllowNull] ChallengeValue other)
        {
            return other != null
                && Attack.Equals(other.Attack)
                && defeatedByValues.All(defeatedByValue => other.defeatedByValues.Contains(defeatedByValue));
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ChallengeValue);
        }

        public string GetDefeatMessage(AttackValue attack)
        {
            if (!defeatedByValues.Any(defeatConfig => defeatConfig.Attack.Equals(attack)))
            {
                throw new ArgumentOutOfRangeException(attack.Name, "Defeated by value not found");
            }

            var message = defeatedByValues
                .First(defeatConfig => defeatConfig.Attack.Equals(attack))
                .Message;

            return message;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                Attack.GetHashCode(),
                defeatedByValues.GetHashCode());
        }

        public override string ToString()
        {
            return Attack.Name;
        }
    }
}