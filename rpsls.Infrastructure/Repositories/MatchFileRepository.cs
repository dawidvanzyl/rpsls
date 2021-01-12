using rpsls.Application.Repositories;
using rpsls.Domain;
using rpsls.Domain.Enums;
using System.IO;

namespace rpsls.Infrastructure.Repositories
{
    public sealed class MatchFileRepository : IMatchRepository
    {
        private static string TRAIN_DATA_FILEPATH = @"MLEngines\PlayerAttackForecastML\rpsls.player.reaction.csv";

        public MatchFileRepository()
        {
            string trainDataPath = Path.GetDirectoryName(TRAIN_DATA_FILEPATH);
            if (!Directory.Exists(trainDataPath))
            {
                Directory.CreateDirectory(trainDataPath);
            }

            if (!File.Exists(TRAIN_DATA_FILEPATH))
            {
                File.Copy("rpsls.player.initial.csv", TRAIN_DATA_FILEPATH);
            }
        }

        public void Add(Attack playerReaction, AttackResult previousPlayer1AttackResult, Attack previousPlayer2Attack)
        {
            using (var fileStream = new FileStream(TRAIN_DATA_FILEPATH, FileMode.Append))
            using (var streamWriter = new StreamWriter(fileStream))
            {
                streamWriter.WriteLine($"{playerReaction.AttackValue.Value};{(int)previousPlayer1AttackResult};{previousPlayer2Attack.AttackValue.Value}");
            }
        }
    }
}
