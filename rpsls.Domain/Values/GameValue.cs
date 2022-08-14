using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace rpsls.Domain.Values
{
    public sealed class GameValue : IEquatable<GameValue>
    {
        private GameValue(string name, IList<ChallengeValue> challenges)
        {
            Name = name;
            Challenges = challenges;
        }

        public IList<ChallengeValue> Challenges { get; }
        public string Name { get; }

        public static GameValue From(string name, IList<ChallengeValue> challenges)
        {
            return new GameValue(name, challenges);
        }

        public bool Equals([AllowNull] GameValue other)
        {
            return other != null
                && Name.Equals(other.Name)
                && Challenges.All(challange => other.Challenges.Contains(challange));
        }

        public override string ToString()
        {
            return Name;
        }
    }
}