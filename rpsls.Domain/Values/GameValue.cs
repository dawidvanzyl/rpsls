using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace rpsls.Domain.Values
{
    public sealed class GameValue : IEquatable<GameValue>
    {
        private GameValue(string name, IList<Attack> attacks)
        {
            Name = name;
            Attacks = attacks;
        }

        public IList<Attack> Attacks { get; }
        public string Name { get; }

        public static GameValue From(string name, IList<Attack> attacks)
        {
            return new GameValue(name, attacks);
        }

        public bool Equals([AllowNull] GameValue other)
        {
            return other != null
                && Name.Equals(other.Name)
                && Attacks.All(attack => other.Attacks.Contains(attack));
        }

        public override string ToString()
        {
            return Name;
        }
    }
}