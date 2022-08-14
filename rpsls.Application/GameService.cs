using rpsls.Domain;
using rpsls.Domain.Enums;
using rpsls.Domain.Values;
using rpsls.Infrastructure.Algorithms;
using rpsls.Infrastructure.Algorithms.Contexts;
using rpsls.Infrastructure.Factories;
using rpsls.Infrastructure.Repositories;

namespace rpsls.Application
{
    public class GameService
    {
        private readonly MLAlgorithmContext algorithmContext;
        private readonly IChallengeAlgorithm<MLAlgorithmContext> challengeAlgorithm;
        private readonly IMatchRepository matchRepository;

        public GameService(GameValue gameValue, IMatchRepository matchRepository, IAlgorithmContextFactory algorithmContextFactory, IChallengeAlgorithmStrategy challengeAlgorithmStrategy)
        {
            this.matchRepository = matchRepository;

            algorithmContext = algorithmContextFactory.CreateMLAlgorithmContext();
            challengeAlgorithm = challengeAlgorithmStrategy.CreateMLChallenge(gameValue);
        }

        public ChallengeValue GetChallenge()
        {
            return challengeAlgorithm.GetChallenge(algorithmContext);
        }

        public void SaveMatchResult(ChallengeValue playerOne, ChallengeValue playerTwo, ChallengeResult challengeResult)
        {
            var gameRound = GameRound.From(playerOne, playerTwo, challengeResult);

            algorithmContext.SaveGameRound(gameRound);
            matchRepository.Add(gameRound);
        }
    }
}