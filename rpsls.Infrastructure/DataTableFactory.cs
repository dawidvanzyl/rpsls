using rpsls.Entities;
using System.Data;

namespace rpsls.Infrastructure
{
    internal static class DataTableFactory
    {
        internal static DataTable CreateMatchResultsTable(IEnumerable<MatchResult> matchResults)
        {
            var tvpMatchResult = new DataTable();
            tvpMatchResult.Columns.Add("Player1", typeof(int));
            tvpMatchResult.Columns.Add("Player2", typeof(int));
            tvpMatchResult.Columns.Add("Result", typeof(int));
            tvpMatchResult.Columns.Add("AttackCount", typeof(int));

            foreach (var matchResult in matchResults)
            {
                tvpMatchResult.Rows.Add(
                    matchResult.P1Attack,
                    matchResult.P2Attack,
                    matchResult.Result,
                    matchResult.ConsecutiveRepeats);
            }

            return tvpMatchResult;
        }
    }
}