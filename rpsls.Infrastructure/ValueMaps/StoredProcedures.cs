namespace rpsls.Infrastructure.ValueMaps
{
    public class StoredProcedures
    {
#pragma warning disable IDE1006 // Naming Styles
        internal static readonly StoredProcedures CreateMatchResult = new StoredProcedures("dbo.CreateMatchResult");
#pragma warning restore IDE1006 // Naming Styles

        private StoredProcedures(string name)
        {
            Name = name;
        }

        internal string Name { get; }
    }
}