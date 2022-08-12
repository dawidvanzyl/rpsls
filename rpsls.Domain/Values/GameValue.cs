using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace rpsls.Domain.Values
{
    public sealed class GameValue : IEquatable<GameValue>
    {
        private GameValue(string name, IList<ChallangeValue> challanges)
        {
            Name = name;
            Challanges = challanges;
        }

        public IList<ChallangeValue> Challanges { get; }
        public string Name { get; }

        public static GameValue From(string name, IList<ChallangeValue> challanges)
        {
            return new GameValue(name, challanges);
        }

        public bool Equals([AllowNull] GameValue other)
        {
            return other != null
                && Name.Equals(other.Name)
                && Challanges.All(challange => other.Challanges.Contains(challange));
        }

        public override string ToString()
        {
            return Name;
        }
    }
}