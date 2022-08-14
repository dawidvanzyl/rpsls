using rpsls.Domain;
using rpsls.Infrastructure.Repositories;
using System.Collections.Generic;

namespace rpsls.Infrastructure.Algorithms.Contexts
{
    public class AlgorithmContext
    {
        private readonly IMatchRepository matchRepository;

        public AlgorithmContext(IMatchRepository matchRepository)
        {
            this.matchRepository = matchRepository;
        }

        public IList<GameRound> GameRounds { get; private set; }

        public virtual void SaveGameRound(GameRound gameRound)
        {
            GameRounds.Add(gameRound);
        }

        internal virtual void LoadMatchResults()
        {
            GameRounds = matchRepository.GetAll();
        }
    }
}