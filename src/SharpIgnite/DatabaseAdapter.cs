using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;

namespace SharpIgnite
{
    public interface IDatabaseAdapter
    {
        string ConnectionString { get; set; }
        Database Database { get; set; }
        ISqlQueryBuilder QueryBuilder { get; set; }
        int Drop(string tableName);
        int Create(string tableName, params DatabaseColumn[] columns);
        int Insert<T>(string tableName, List<T> data);
        int Insert<T>(string tableName, T data);
        //int Insert(string tableName, Array data);
        List<T> Result<T>();
        List<T> Query<T>(string query);
        T Row<T>();
        Array Row();
        T QueryFirst<T>(string query);
        int Count(string tableName);
        int Update(string tableName);
        int Update<T>(string tableName, T data, Array _where);
        int Update<T>(string tableName, T data, string _where);
        int Delete(string tableName, Array _where);
        int Delete(string tableName, string _where);
        int Truncate(string tableName);
        void QueryFromFile(string filePath);

        int NonQuery(string query, params IDataParameter[] parameters);
        IDataReader Reader(string query, params IDataParameter[] parameters);
        T Scalar<T>(string query, params IDataParameter[] parameters);
    }

    public abstract class DatabaseAdapter : IDatabaseAdapter
    {
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
            return NonQuery(query);
        }

        public int Create(string tableName, params DatabaseColumn[] columns)
        {
            string query = queryBuilder.Create(this.Database, tableName, columns);
            Database.LastQuery = query;
            return NonQuery(query);
        }

        public int Insert<T>(string tableName, List<T> data)
        {
            string query = queryBuilder.Insert<T>(this.Database, tableName, data);
            Database.LastQuery = query;
            return NonQuery(query);
        }

        //public int Insert(string tableName, Array data)
        //{
        //    string query = queryBuilder.Insert(this.Database, tableName, data);
        //    Database.LastQuery = query;
        //    Database.insertedId = Scalar<int>(query);
        //    return Database.insertedId;
        //}

        public int Insert<T>(string tableName, T data)
        {
            string query = queryBuilder.Insert<T>(this.Database, tableName, data);
            Database.LastQuery = query;
            Database.insertedId = Scalar<int>(query);
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
            using (var rs = Reader(query)) {
                while (rs != null && rs.Read()) {
                    T item = CreateItem<T>(rs);
                    results.Add(item);
                }
            }
            return results;
        }

        public List<T> Query<T>(string query)
        {
            this.Database.LastQuery = query;
            List<T> results = new List<T>();
            using (var rs = Reader(query)) {
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
            using (var rs = Reader(query)) {
                if (rs != null && rs.Read()) {
                    item = CreateItem<T>(rs);
                }
            }
            return item;
        }

        public Array Row()
        {
            string query = queryBuilder.Row(this.Database);

            Array array = null;
            using (var rs = Reader(query)) {
                if (rs != null && rs.Read()) {
                    array = CreateItem(rs);
                }
            }
            return array;
        }

        public T QueryFirst<T>(string query)
        {
            this.Database.LastQuery = query;
            if (typeof(T).IsPrimitive) {
                return Scalar<T>(query);
            } else {
                T item = default(T);
                using (var rs = Reader(query)) {
                    if (rs != null && rs.Read()) {
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
                        } else if (property.PropertyType == typeof(Guid)) {
                            property.SetValue(item, (Guid)value);
                        } else if (property.PropertyType == typeof(DbType)) { // HACK: This is for migration when property is of type DbType
                            property.SetValue(item, GetDbTypeValue(value.ToString()));
                        } else {
                            property.SetValue(item, value);
                        }
                    }
                } catch (IndexOutOfRangeException ex) { }
            }
            return item;
        }

        Array CreateItem(IDataReader rs)
        {
            Array item = new Array();
            for (int i = 0; i < rs.FieldCount; i++) {
                string columnName = rs.GetName(i);
                object columnValue = rs.GetValue(i);
                item.Add(columnName, columnValue);
            }
            return item;
        }

        public int Count(string tableName)
        {
            string query = queryBuilder.Count(Database, tableName);
            return Scalar<int>(query);
        }

        public int Update(string tableName)
        {
            string query = queryBuilder.Update(Database, tableName);
            return NonQuery(query);
        }

        public int Update<T>(string tableName, T data, Array _where)
        {
            string query = queryBuilder.Update(Database, tableName, data, _where);
            return NonQuery(query);
        }

        public int Update<T>(string tableName, T data, string _where)
        {
            string query = queryBuilder.Update(Database, tableName, data, _where);
            return NonQuery(query);
        }

        public int Delete(string tableName, Array _where)
        {
            string query = queryBuilder.Delete(this.Database, tableName, _where);
            return NonQuery(query);
        }

        public int Delete(string tableName, string _where)
        {
            string query = queryBuilder.Delete(this.Database, tableName, _where);
            return NonQuery(query);
        }

        public int Truncate(string tableName)
        {
            string query = queryBuilder.Truncate(this.Database, tableName);
            return NonQuery(query);
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
                NonQuery(query);
                i++;
            }
        }

        public abstract IDbConnection CreateConnection(string connectionString);

        public virtual int NonQuery(string query, params IDataParameter[] parameters)
        {
            var connection = CreateConnection(ConnectionString);
            OpenConnection(connection);
            var cmd = connection.CreateCommand();
            cmd.CommandText = query;
            foreach (var parameter in parameters) {
                cmd.Parameters.Add(parameter);
            }
            int rowsAffected = cmd.ExecuteNonQuery();
            CloseConnection(connection);
            return rowsAffected;
        }

        public virtual IDataReader Reader(string query, params IDataParameter[] parameters)
        {
            var connection = CreateConnection(ConnectionString);
            OpenConnection(connection);
            var cmd = connection.CreateCommand();
            cmd.CommandText = query;
            foreach (var parameter in parameters) {
                cmd.Parameters.Add(parameter);
            }
            var reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            return reader;
        }

        public virtual T Scalar<T>(string query, params IDataParameter[] parameters)
        {
            return Scalar<T>(query, default(T), parameters);
        }

        public T Scalar<T>(string query, T defaultValue, params IDataParameter[] parameters)
        {
            var connection = CreateConnection(ConnectionString);
            OpenConnection(connection);
            var cmd = connection.CreateCommand();
            cmd.CommandText = query;
            foreach (var parameter in parameters) {
                cmd.Parameters.Add(parameter);
            }
            T returnValue = defaultValue;
            var o = cmd.ExecuteScalar();
            if (o != null && o != DBNull.Value) {
                returnValue = (T)Convert.ChangeType(o, typeof(T));
            }
            CloseConnection(connection);
            return returnValue;
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
    }
}
