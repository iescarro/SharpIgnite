using System.Data;

namespace SharpIgnite
{
    public class DB
    {
        public static IDatabaseAdapter Connection()
        {
            var connector = Config.Get("DB_CONNECTION") ?? "sqlserver";
            return Connection(connector);
        }

        public static IDatabaseAdapter Connection(string connector)
        {
            var connectionString = Config.Get("DB_CONNECTION_STRING") ?? Config.Get("SqlConnection");
            return Connection(connector, connectionString);
        }

        public static IDatabaseAdapter Connection(string connector, string connectionString)
        {
            if (connector == "sqlite") {
                return Connection(new SQLiteDatabaseAdapter(connectionString));
            }
            return Connection(new SqlDatabaseAdapter(connectionString));
        }

        public static IDatabaseAdapter Connection(IDatabaseAdapter connector)
        {
            return connector;
        }

        public static Database Table(string tableName)
        {
            var connector = Connection();
            var db = Database.Instance.Adapter(connector);
            return db.From(tableName);
        }

        public static int NonQuery(string query, params IDbDataParameter[] parameters)
        {
            var connection = Connection();
            return connection.NonQuery(query, parameters);
        }

        public static IDataReader Reader(string query, params IDataParameter[] parameters)
        {
            var connection = Connection();
            return connection.Reader(query, parameters);
        }

        public static T Scalar<T>(string query, params IDataParameter[] parameters)
        {
            var connection = Connection();
            return connection.Scalar<T>(query, parameters);
        }

        public static string GetString(IDataReader reader, int index)
        {
            return GetString(reader, index, null);
        }

        public static string GetString(IDataReader reader, int index, string defaultValue)
        {
            return reader.IsDBNull(index) ? defaultValue : reader.GetString(index);
        }

        public static int GetInt32(IDataReader reader, int index)
        {
            return GetInt32(reader, index, 0);
        }

        public static int GetInt32(IDataReader reader, int index, int defaultValue)
        {
            return reader.IsDBNull(index) ? defaultValue : reader.GetInt32(index);
        }

        public static float GetFloat(IDataReader reader, int index)
        {
            return GetFloat(reader, index, 0);
        }

        public static float GetFloat(IDataReader reader, int index, float defaultValue)
        {
            return reader.IsDBNull(index) ? defaultValue : reader.GetFloat(index);
        }
    }
}