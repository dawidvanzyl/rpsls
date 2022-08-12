using rpsls.Domain;
using rpsls.Domain.Enums;
using rpsls.Domain.Values;
using rpsls.Infrastructure.Algorithms;
using rpsls.Infrastructure.Repositories;

namespace rpsls.Application
{
    public class GameService
    {
        private readonly IChallangeAlgorithm challangeAlgorithm;
        private readonly IMatchRepository matchRepository;

        public GameService(GameValue gameValue, IMatchRepository matchRepository, IChallangeAlgorithmStrategy challangeAlgorithmStrategy)
        {
            this.matchRepository = matchRepository;

            challangeAlgorithm = challangeAlgorithmStrategy.CreateMLChallange(gameValue);
        }

        public ChallangeValue GetChallange()
        {
            var mlChallangeAlgorithm = challangeAlgorithm as MLChallangeAlgorithm;
            mlChallangeAlgorithm.TrainContext();
            return mlChallangeAlgorithm.GetChallange();
        }

        public void SaveMatchResult(ChallangeValue playerOne, ChallangeValue playerTwo, ChallangeResult challangeResult)
        {
            matchRepository.Add(MatchResult.From(playerOne, playerTwo, challangeResult));

            var mlChallangeAlgorithm = challangeAlgorithm as MLChallangeAlgorithm;
            mlChallangeAlgorithm.SaveChallanges(playerOne, playerTwo);
        }
    }
}