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

        public static DefeatedByValue From(AttackValue attackValue, string message)
        {
            return new DefeatedByValue(attackValue, message);
        }

        public bool Equals([AllowNull] DefeatedByValue other)
        {
            return other != null
                && AttackValue.Equals(other.AttackValue)
                && Message.Equals(other.Message);
        }
    }
}