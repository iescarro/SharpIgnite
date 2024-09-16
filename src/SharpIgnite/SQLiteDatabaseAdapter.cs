using System.Data;
using System.Data.SQLite;

namespace SharpIgnite
{
    public class SQLiteDatabaseAdapter : DatabaseAdapter
    {
        public SQLiteDatabaseAdapter() : this("")
        {
        }

        public SQLiteDatabaseAdapter(string connectionString) : this(connectionString, new SQLiteQueryBuilder())
        {
        }

        public SQLiteDatabaseAdapter(string connectionString, ISqlQueryBuilder queryBuilder)
        {
            this.ConnectionString = connectionString;
            this.QueryBuilder = queryBuilder;
        }

        public override IDbConnection CreateConnection(string connectionString)
        {
            return new SQLiteConnection(connectionString);
        }
    }
}
