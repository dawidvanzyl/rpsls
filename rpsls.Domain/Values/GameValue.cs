using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace rpsls.Domain.Values
{
    public class GameValue : IEquatable<GameValue>
    {
        private GameValue(string name, IList<Attack> attacks)
        {
            Name = name;
            Attacks = attacks;
        }

        public string Name { get; }
        public IList<Attack> Attacks { get; }

        public bool Equals([AllowNull] GameValue other)
        {
            if (other == null) return false;

            return Name.Equals(other.Name)
                && Attacks.All(attack => other.Attacks.Contains(attack));
        }

        public static GameValue From(string name, IList<Attack> attacks)
        {
            return new GameValue(name, attacks);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
