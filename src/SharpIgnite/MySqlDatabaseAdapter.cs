using MySql.Data.MySqlClient;
using System.Data;

namespace SharpIgnite
{
    public class MySqlDatabaseAdapter : DatabaseAdapter
    {
        public MySqlDatabaseAdapter() : this("")
        {
        }

        public MySqlDatabaseAdapter(string connectionString) : this(connectionString, new MySqlQueryBuilder())
        {
        }

        public MySqlDatabaseAdapter(string connectionString, ISqlQueryBuilder queryBuilder)
        {
            this.ConnectionString = connectionString;
            this.QueryBuilder = queryBuilder;
        }

        public override IDbConnection CreateConnection(string connectionString)
        {
            return new MySqlConnection(connectionString);
        }
    }
}
