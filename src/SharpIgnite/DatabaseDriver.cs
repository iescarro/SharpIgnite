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
    public interface IDatabaseDriver
    {
        string ConnectionString { get; set; }
        Database Database { get; set; }
        ISqlQueryBuilder QueryBuilder { get;set; }
        int Drop(string tableName);
        int Create(string tableName, params DatabaseColumn[] columns);
        int Insert<T>(string tableName, List<T> data);
        int Insert<T>(string tableName, T data);
        int Insert(string tableName, Array data);
        List<T> Result<T>();
        List<T> Query<T>(string query);
        T Row<T>();
        T QueryFirst<T>(string query);
        int Count(string tableName);
        int Update(string tableName);
        int Update<T>(string tableName, T data, Array _where);
        int Update<T>(string tableName, T data, string _where);
        int Delete(string tableName, Array _where);
        int Delete(string tableName, string _where);
        int Truncate(string tableName);
        void QueryFromFile(string filePath);
    }
    
    public abstract class DatabaseDriver : IDatabaseDriver
    {
        public const int SqlServer = 1;
        public const int MySql = 2;
        public const int SQLite = 3;
        
        public string ConnectionString { get; set; }
        public Database Database { get; set; }
        
        ISqlQueryBuilder queryBuilder;
        
        public ISqlQueryBuilder QueryBuilder {
            get { return queryBuilder; }
            set { queryBuilder = value; }
        }
        
        public int Drop(string tableName)
        {
            string query = queryBuilder.Drop(this.Database, tableName);
            Database.LastQuery = query;
            return ExecuteNonQuery(query);
        }
        
        public int Create(string tableName, params DatabaseColumn[] columns)
        {
            string query = queryBuilder.Create(this.Database, tableName, columns);
            Database.LastQuery = query;
            return ExecuteNonQuery(query);
        }
        
        public int Insert<T>(string tableName, List<T> data)
        {
            string query = queryBuilder.Insert<T>(this.Database, tableName, data);
            Database.LastQuery = query;
            return ExecuteNonQuery(query);
        }
        
        public int Insert(string tableName, Array data)
        {
            string query = queryBuilder.Insert(this.Database, tableName, data);
            Database.LastQuery = query;
            Database.insertedId = ExecuteScalar<int>(query);
            return Database.insertedId;
        }
        
        public int Insert<T>(string tableName, T data)
        {
            string query = queryBuilder.Insert<T>(this.Database, tableName, data);
            Database.LastQuery = query;
            Database.insertedId = ExecuteScalar<int>(query);
            return InsertedId();
        }
        
        public int InsertedId()
        {
            return Database.insertedId;
        }
        
        public List<T> Result<T>()
        {
            string query = queryBuilder.Result(this.Database);
            
            List<T> results = new List<T>();
            using (var rs = ExecuteReader(query)) {
                while (rs.Read()) {
                    T item = CreateItem<T>(rs);
                    results.Add(item);
                }
            }
            return results;
        }
        
        public List<T> Query<T>(string query)
        {
            //string query = queryBuilder.Result(this.Database); // Not needed, we'll execute the query directly
            this.Database.LastQuery = query;
            List<T> results = new List<T>();
            using (var rs = ExecuteReader(query)) {
                while (rs.Read()) {
                    T item = CreateItem<T>(rs);
                    results.Add(item);
                }
            }
            return results;
        }
        
        public T Row<T>()
        {
            string query = queryBuilder.Row(this.Database);
            
            T item = default(T);
            using (var rs = ExecuteReader(query)) {
                if (rs.Read()) {
                    item = CreateItem<T>(rs);
                }
            }
            return item;
        }
        
        public T QueryFirst<T>(string query)
        {
            //string query = queryBuilder.Row(this.Database); // Not needed, we'll execute the query directly
            this.Database.LastQuery = query;
            if (typeof(T).IsPrimitive) {
                return ExecuteScalar<T>(query);
            } else {
                T item = default(T);
                using (var rs = ExecuteReader(query)) {
                    if (rs.Read()) {
                        item = CreateItem<T>(rs);
                    }
                }
                return item;
            }
        }
        
        protected virtual DbType GetDbTypeValue(string value)
        {
            value = value.ToUpper();
            if (value == "INT") {
                return DbType.Int32;
            }
            return DbType.String;
        }
        
        T CreateItem<T>(IDataReader rs)
        {
            T item = Activator.CreateInstance<T>();
            Type type = typeof(T);
            foreach (PropertyInfo property in type.GetProperties()) {
                try {
                    string propertyName;
                    //if (property.GetCustomAttribute<ColumnAttribute>() != null) {
                        ColumnAttribute column = (ColumnAttribute)property.GetCustomAttribute(typeof(ColumnAttribute));
                        if (column != null && column.Name != null) {
                            propertyName = column.Name;
                        } else {
                            propertyName = property.Name;
                        }
                        var index = rs.GetOrdinal(propertyName);
                        object value = rs[index];
                        if (value == DBNull.Value) {
                            if (property.PropertyType == typeof(string) || property.PropertyType == typeof(Guid)) {
                                property.SetValue(item, null);
                            } else if (property.PropertyType.IsValueType) {
                                property.SetValue(item, Activator.CreateInstance(property.PropertyType));
                            }
                        } else {
                            if (property.PropertyType == typeof(int) && value.GetType() == typeof(Int64)) {
                                // Convert Int64 to int
                                // HACK: For default int in SQLite that's Int64
                                property.SetValue(item, Convert.ToInt32((Int64)value));
                            } else if (property.PropertyType == typeof(string)) {
                                property.SetValue(item, value.ToString());
                            }  else if (property.PropertyType == typeof(Guid)) {
                                property.SetValue(item, (Guid)value);
                            } else if (property.PropertyType == typeof(DbType)) { // HACK: This is for migration when property if of type DbType
                                property.SetValue(item, GetDbTypeValue(value.ToString()));
                            } else {
                                property.SetValue(item, value);
                            }
                        }
                    //}
                    /*if (property.GetCustomAttribute<ColumnAttribute>() != null) {
                        ColumnAttribute column = (ColumnAttribute)property.GetCustomAttribute(typeof(ColumnAttribute));
                        if (column.Name != null) {
                            propertyName = column.Name;
                        } else {
                            propertyName = property.Name;
                        }
                    } else {
                        propertyName = property.Name;
                    }
                    var index = rs.GetOrdinal(propertyName);
                    object value = rs[index];
                    if (value == DBNull.Value) {
                        if (property.PropertyType == typeof(string)) {
                            property.SetValue(item, null);
                        } else if (property.PropertyType.IsValueType) {
                            property.SetValue(item, Activator.CreateInstance(property.PropertyType));
                        }
                    } else {
                        property.SetValue(item, value);
                    }*/
                } catch (IndexOutOfRangeException ex) {}
            }
            return item;
        }
        
        public int Count(string tableName)
        {
            string query = queryBuilder.Count(Database, tableName);
            return ExecuteScalar<int>(query);
        }
        
        public int Update(string tableName)
        {
            string query = queryBuilder.Update(Database, tableName);
            return ExecuteNonQuery(query);
        }
        
        public int Update<T>(string tableName, T data, Array _where)
        {
            string query = queryBuilder.Update(Database, tableName, data, _where);
            return ExecuteNonQuery(query);
        }
        
        public int Update<T>(string tableName, T data, string _where)
        {
            string query = queryBuilder.Update(Database, tableName, data, _where);
            return ExecuteNonQuery(query);
        }
        
        public int Delete(string tableName, Array _where)
        {
            string query = queryBuilder.Delete(this.Database, tableName, _where);
            return ExecuteNonQuery(query);
        }
        
        public int Delete(string tableName, string _where)
        {
            string query = queryBuilder.Delete(this.Database, tableName, _where);
            return ExecuteNonQuery(query);
        }
        
        public int Truncate(string tableName)
        {
            string query = queryBuilder.Truncate(this.Database, tableName);
            return ExecuteNonQuery(query);
        }
        
        public void QueryFromFile(string filePath)
        {
            string fileContent = "";
            using (var sr = new StreamReader(filePath)) {
                fileContent = sr.ReadToEnd();
            }
            string preQuery = "";
            var queries = fileContent.Split(new string[] { "GO" }, StringSplitOptions.RemoveEmptyEntries);
            int i = 0;
            foreach (var q in queries) {
                string[] lines = q.Split('\n');
                if (lines.Length > 0 && i == 0) {
                    preQuery = lines[0];
                }
                string query = i > 0 ? preQuery + q : q;
                Database.LastQuery = query;
                ExecuteNonQuery(query);
                i++;
            }
        }
        
        protected virtual IDataReader ExecuteReader(string query)
        {
            var connection = CreateConnection(ConnectionString);
            OpenConnection(connection);
            var cmd = connection.CreateCommand();
            cmd.CommandText = query;
            var reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            return reader;
        }
        
        protected virtual T ExecuteScalar<T>(string query)
        {
            return ExecuteScalar<T>(query, default(T));
        }
        
        public abstract IDbConnection CreateConnection(string connectionString);
        
        protected T ExecuteScalar<T>(string query, T defaultValue)
        {
            var connection = CreateConnection(ConnectionString);
            OpenConnection(connection);
            var cmd = connection.CreateCommand();
            cmd.CommandText = query;
            T returnValue = defaultValue;
            var o = cmd.ExecuteScalar();
            if (o != null && o != DBNull.Value) {
                returnValue = (T)Convert.ChangeType(o, typeof(T));
            }
            CloseConnection(connection);
            return returnValue;
        }
        
        protected virtual int ExecuteNonQuery(string query)
        {
            var connection = CreateConnection(ConnectionString);
            OpenConnection(connection);
            var cmd = connection.CreateCommand();
            cmd.CommandText = query;
            int rowsAffected = cmd.ExecuteNonQuery();
            CloseConnection(connection);
            return rowsAffected;
        }
        
        void OpenConnection(IDbConnection connection)
        {
            if (connection.State == ConnectionState.Closed) {
                connection.Open();
            }
        }
        
        void CloseConnection(IDbConnection connection)
        {
            if (connection.State == ConnectionState.Open) {
                connection.Close();
                connection.Dispose();
            }
        }
        
        public static IDatabaseDriver GetDatabaseDriver(int driver, string connectionString)
        {
            if (driver == DatabaseDriver.MySql) {
                return new MySqlDatabaseDriver(connectionString);
            } else if (driver == DatabaseDriver.SQLite) {
                return new SQLiteDatabaseDriver(connectionString);
            } else {
                return new SqlDatabaseDriver(connectionString);
            }
        }
    }
}
