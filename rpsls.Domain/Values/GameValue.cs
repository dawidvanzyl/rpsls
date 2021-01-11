using rpsls.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace rpsls.Domain.Values
{
    public class GameValue : IEquatable<GameValue>
    {
        private GameValue(string configuration, IList<Attack> attacks, short rounds)
        {
            Configuration = configuration;
            Attacks = attacks;
            Rounds = rounds;
        }

        public string Configuration { get; }
        public IList<Attack> Attacks { get; }
        public short Rounds { get; }

        public bool Equals([AllowNull] GameValue other)
        {
            if (other == null) return false;

            return Configuration.Equals(other.Configuration)
                && Rounds.Equals(other.Rounds)
                && Attacks.All(attack => other.Attacks.Contains(attack));
        }

        public static GameValue From(string configuration, IList<Attack> attacks, short rounds)
        {
            return new GameValue(configuration, attacks, rounds);
        }
    }
}
