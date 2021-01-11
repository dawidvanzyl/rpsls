using System;

namespace rpsls.Attacks
{
    public class AttackType : IEquatable<AttackType>
    {
        private AttackType(string name, byte value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }
        public byte Value { get; }

        public static AttackType From(string name, byte value)
        {
            return new AttackType(name, value);
        }

        public bool Equals(AttackType other)
        {
            return Name.Equals(other.Name)
                && Value.Equals(other.Value);
        }
    }
}
