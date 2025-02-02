using Dapper;
using Microsoft.Extensions.Configuration;
using rpsls.Entities;
using rpsls.Infrastructure.Mappers;
using rpsls.Infrastructure.Repositories.Abstracts;

namespace rpsls.Infrastructure.Repositories;

public interface IGameRepository
{
    Task<IList<Game>> GetAllAsync();

    Task SaveAsync(int bestOf, int player1, int player2, IEnumerable<Round> rounds);
}

public class GameRepository(IConfiguration configuration)
    : AbstractRepository(configuration), IGameRepository
{
    public async Task<IList<Game>> GetAllAsync()
    {
        var results = await QueryMultipleAsync("dbo.GetGames", GameMapper.MapGridReader);
        return results.ToList();
    }

    public async Task SaveAsync(int bestOf, int player1, int player2, IEnumerable<Round> rounds)
    {
        var gameParam = new
        {
            BestOf = bestOf,
            Player1 = player1,
            Player2 = player2
        };

        var gameId = await ExecuteAsync<long>("dbo.SaveGame", gameParam);

        var tvpMatchResult = DataTableFactory
            .CreateMatchResultsTable(gameId, rounds)
            .AsTableValuedParameter("dbo.tvp_Round");

        var param = new
        {
            Rounds = tvpMatchResult
        };

        await ExecuteAsync("dbo.SaveRounds", param);
    }
}