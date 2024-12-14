using Microsoft.Extensions.Configuration;
using rpsls.Domain.Enums;
using rpsls.Infrastructure.Repositories.Abstracts;
using rpsls.Infrastructure.ValueMaps;

namespace rpsls.Infrastructure.Repositories
{
    public interface IGameRepository
    {
        Task CreateMatchResultAsync(AttackTypes p1, AttackTypes p2, ResultTypes matchResult);
    }

    public class GameRepository : AbstractRepository, IGameRepository
    {
        public GameRepository(IConfiguration configuration)
            : base(configuration)
        {
        }

        public async Task CreateMatchResultAsync(AttackTypes p1, AttackTypes p2, ResultTypes matchResult)
        {
            var param = new
            {
                Player1 = p1,
                Player2 = p2,
                Result = matchResult
            };

            await ExecuteAsync(StoredProcedures.CreateMatchResult, param);
        }
    }
}