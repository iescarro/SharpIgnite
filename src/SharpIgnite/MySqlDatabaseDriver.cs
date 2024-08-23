using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Reflection;
using MySql.Data.MySqlClient;

namespace SharpIgnite
{
    public class MySqlDatabaseDriver : DatabaseDriver
    {
        public MySqlDatabaseDriver() : this("")
        {
        }
        
        public MySqlDatabaseDriver(string connectionString) : this(connectionString, new MySqlQueryBuilder())
        {
        }
        
        public MySqlDatabaseDriver(string connectionString, ISqlQueryBuilder queryBuilder)
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
