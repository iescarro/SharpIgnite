using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;

namespace SharpIgnite
{
    public class SqlDatabaseDriver : DatabaseDriver
    {
        public SqlDatabaseDriver() : this("")
        {
        }
        
        public SqlDatabaseDriver(string connectionString) : this(connectionString, new SqlQueryBuilder())
        {
        }
        
        public SqlDatabaseDriver(string connectionString, ISqlQueryBuilder queryBuilder)
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
