using System;
using System.Diagnostics.CodeAnalysis;

namespace rpsls.Domain.Values
{
    public sealed class DefeatedByValue : IEquatable<DefeatedByValue>
    {
        private DefeatedByValue(AttackValue attack, string message)
        {
            Attack = attack;
            Message = message;
        }

        public AttackValue Attack { get; }
        public string Message { get; }

        public static DefeatedByValue From(AttackValue attack, string message)
        {
            return new DefeatedByValue(attack, message);
        }

        public bool Equals([AllowNull] DefeatedByValue other)
        {
            return other != null
                && Attack.Equals(other.Attack)
                && Message.Equals(other.Message);
        }
    }
}