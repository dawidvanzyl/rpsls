using System;
using System.Diagnostics.CodeAnalysis;

namespace rpsls.Domain.Values
{
    public sealed class DefeatedByValue : IEquatable<DefeatedByValue>
    {
        private DefeatedByValue(AttackValue attackValue, string message)
        {
            AttackValue = attackValue;
            Message = message;
        }

        public AttackValue AttackValue { get; }
        public string Message { get; }

        public bool Equals([AllowNull] DefeatedByValue other)
        {
            if (other == null) return false;

            return AttackValue.Equals(other.AttackValue)
                && Message.Equals(other.Message);
        }

        public static DefeatedByValue From(AttackValue attackValue, string message)
        {
            return new DefeatedByValue(attackValue, message);
        }
    }
}
