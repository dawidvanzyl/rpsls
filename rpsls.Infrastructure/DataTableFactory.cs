using rpsls.Entities;
using System.Data;

namespace rpsls.Infrastructure;

internal static class DataTableFactory
{
    internal static DataTable CreateMatchResultsTable(long gameId, IEnumerable<Round> rounds)
    {
        var tvpMatchResult = new DataTable();
        tvpMatchResult.Columns.Add("FkGameId", typeof(long));
        tvpMatchResult.Columns.Add("Player1", typeof(int));
        tvpMatchResult.Columns.Add("Player2", typeof(int));
        tvpMatchResult.Columns.Add("Result", typeof(int));

        foreach (var round in rounds)
        {
            tvpMatchResult.Rows.Add(
                gameId,
                round.P1Attack,
                round.P2Attack,
                round.Result);
        }

        return tvpMatchResult;
    }
}