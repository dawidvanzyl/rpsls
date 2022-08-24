using rpsls.Application.Models;
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
        private readonly GameValue gameValue;
        private readonly IMatchRepository matchRepository;
        private readonly MLChallengeAlgorithm mlChallengeAlgorithm;
        private readonly RandomChallengeAlgorithm randomChallengeAlgorithm;
        private MatchResult matchResult;

        public GameService(GameValue gameValue, IMatchRepository matchRepository, IAlgorithmContextFactory algorithmContextFactory, IChallengeAlgorithmStrategy challengeAlgorithmStrategy)
        {
            this.gameValue = gameValue;
            this.matchRepository = matchRepository;

            algorithmContext = algorithmContextFactory.CreateMLAlgorithmContext(gameValue);
            mlChallengeAlgorithm = challengeAlgorithmStrategy.CreateMLChallenge(gameValue);
            randomChallengeAlgorithm = challengeAlgorithmStrategy.CreateRandomChallenge(gameValue);
        }

        public MatchResult EndMatch()
        {
            return matchResult;
        }

        public ChallengeValue GetMLChallenge()
        {
            return mlChallengeAlgorithm.GetChallenge(algorithmContext);
        }

        public ChallengeValue GetRandomChallenge()
        {
            return randomChallengeAlgorithm.GetChallenge(algorithmContext);
        }

        public RoundResult GetRoundResult(PlayerInput playerOne, PlayerInput playerTwo)
        {
            var challengeResult = (ChallengeResult)playerOne.Challenge.CompareTo(playerTwo.Challenge);
            var roundResult = new RoundResult { PlayerOne = playerOne.Challenge, PlayerTwo = playerTwo.Challenge, ChallengeResult = challengeResult };

            switch (challengeResult)
            {
                case ChallengeResult.Won:
                    roundResult.DefeatMessage = playerTwo.Challenge.GetDefeatMessage(playerOne.Challenge.Attack);
                    roundResult.ChallengeMessage = $"{playerOne.Name} {ChallengeResult.Won.ToString().ToUpper()}";
                    matchResult.Win(playerOne);
                    break;

                case ChallengeResult.Tied:
                    roundResult.ChallengeMessage = $"Match {ChallengeResult.Tied.ToString().ToUpper()}";
                    matchResult.Win(playerOne);
                    matchResult.Win(playerTwo);
                    break;

                case ChallengeResult.Lost:
                    roundResult.DefeatMessage = playerOne.Challenge.GetDefeatMessage(playerTwo.Challenge.Attack);
                    roundResult.ChallengeMessage = $"{playerTwo.Name} {ChallengeResult.Won.ToString().ToUpper()}";
                    matchResult.Win(playerTwo);
                    break;
            }

            return roundResult;
        }

        public void SaveRoundResult(RoundResult roundResult)
        {
            var gameRound = GameRound.From(gameValue, roundResult.PlayerOne, roundResult.PlayerTwo, roundResult.ChallengeResult);

            algorithmContext.SaveGameRound(gameRound);

            matchRepository.Add(gameRound);
        }

        public void StartMatch(byte rounds, PlayerInput playerOne, PlayerInput playerTwo)
        {
            matchResult = new MatchResult { Rounds = rounds, PlayerOne = playerOne, PlayerTwo = playerTwo };
        }
    }
}