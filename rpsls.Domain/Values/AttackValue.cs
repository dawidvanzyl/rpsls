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

        public bool Equals([AllowNull] AttackValue other)
        {
            if (other == null) return false;

            return Name.Equals(other.Name)
                && Value.Equals(other.Value);
        }

        public static AttackValue From(string name, byte value)
        {
            return new AttackValue(name, value);
        }
    }
}
