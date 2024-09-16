using System.Data;
using System.Data.SqlClient;

namespace SharpIgnite
{
    public class SqlDatabaseAdapter : DatabaseAdapter
    {
        public SqlDatabaseAdapter() : this("")
        {
        }

        public SqlDatabaseAdapter(string connectionString) : this(connectionString, new SqlQueryBuilder())
        {
        }

        public SqlDatabaseAdapter(string connectionString, ISqlQueryBuilder queryBuilder)
        {
            this.ConnectionString = connectionString;
            this.QueryBuilder = queryBuilder;
        }

        public override IDbConnection CreateConnection(string connectionString)
        {
            return new SqlConnection(connectionString);
        }
    }
}
