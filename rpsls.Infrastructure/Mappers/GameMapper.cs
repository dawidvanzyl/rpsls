using rpsls.Entities;
using rpsls.Entities.Enums;
using rpsls.Infrastructure.Dtos;
using static Dapper.SqlMapper;

namespace rpsls.Infrastructure.Mappers
{
    internal static class GameMapper
    {
        public static IEnumerable<GameDto> CreateGameDtos(IEnumerable<dynamic> dynamics)
        {
            return dynamics.Select(d => new GameDto
            {
                BestOf = Convert.ToInt32(d.BestOf),
                PkGameId = Convert.ToInt64(d.PkGameId),
                Player1 = Convert.ToInt32(d.Player1),
                Player2 = Convert.ToInt32(d.Player2)
            });
        }

        public static IEnumerable<RoundDto> CreateRoundDtos(IEnumerable<dynamic> dynamics)
        {
            return dynamics.Select(d => new RoundDto
            {
                ConsecutiveRepeats = Convert.ToInt32(d.ConsecutiveRepeats),
                FkGameId = Convert.ToInt64(d.FkGameId),
                Player1 = Convert.ToInt32(d.Player1),
                Player2 = Convert.ToInt32(d.Player2),
                Result = Convert.ToInt32(d.Result),
                Round = Convert.ToInt32(d.Round)
            });
        }

        public static IEnumerable<Game> MapGames(IEnumerable<GameDto> games, IEnumerable<RoundDto> rounds)
        {
            return games
                .Select(game =>
                {
                    var gameRounds = rounds.Where(round => round.FkGameId == game.PkGameId);
                    return MapGame(game, gameRounds);
                });
        }

        public static IEnumerable<Game> MapGridReader(GridReader gridReader)
        {
            return MapGames(
                CreateGameDtos(gridReader.Read<dynamic>()),
                CreateRoundDtos(gridReader.Read<dynamic>()));
        }

        private static Game MapGame(GameDto gameDto, IEnumerable<RoundDto> rounds)
        {
            return new Game(
                gameDto.BestOf,
                [gameDto.Player1, gameDto.Player2],
                rounds
                    .Select((round, number) => MapRound(round, number + 1))
                    .ToList());
        }

        private static Round MapRound(RoundDto round, int number)
        {
            return new Round(
                number,
                (AttackTypes)round.Player1,
                (AttackTypes)round.Player2,
                (ResultTypes)round.Result,
                round.ConsecutiveRepeats);
        }
    }
}