using System;
using System.Diagnostics.CodeAnalysis;

namespace rpsls.Domain.Values
{
    public sealed class AttackValue : IEquatable<AttackValue>
    {
        private AttackValue(string name, byte value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }
        public byte Value { get; }

        public static AttackValue From(string name, byte value)
        {
            return new AttackValue(name, value);
        }

        public bool Equals([AllowNull] AttackValue other)
        {
            return other != null
                && Name.Equals(other.Name)
                && Value.Equals(other.Value);
        }
    }
}