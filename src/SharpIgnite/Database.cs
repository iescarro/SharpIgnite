using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace SharpIgnite
{
    public static class DatabaseHelper
    {
        public static string ToString(List<Database.WhereClause> _where)
        {
            string s = "";
            int i = 0;
            foreach (var w in _where) {
                if (i++ > 0) {
                    s += " AND ";
                }
                s += w.ToString();
            }
            return s;
        }
    }

    public class Database
    {
        internal string tableName;
        internal string groupByClause;
        internal List<JoinClause> joinClauses = new List<JoinClause>();
        internal List<string> selectClauses = new List<string>();
        internal List<string> whereClauses = new List<string>();
        internal List<OrderByClause> orderByClauses = new List<OrderByClause>();
        internal List<DatabaseColumn> columns = new List<DatabaseColumn>();
        public Dictionary<string, object> Data { get; set; }
        internal string lastQuery;
        internal string styledLastQuery;
        internal int _limit;
        internal int insertedId;

        public string LastQuery {
            get { return lastQuery; }
            internal set {
                lastQuery = value;
                ClearQuery();
                OnQueryChanged(new DatabaseEventArgs(lastQuery, styledLastQuery));
            }
        }

        public event EventHandler<DatabaseEventArgs> QueryChanged;

        protected virtual void OnQueryChanged(DatabaseEventArgs e)
        {
            if (QueryChanged != null) {
                QueryChanged(this, e);
            }
        }

        IDatabaseDriver databaseDriver;

        public Database() : this(ConfigurationManager.AppSettings["SqlConnection"])
        {
        }

        public Database(string connectionString) : this(new SqlDatabaseDriver(connectionString))
        {
        }

        public Database(IDatabaseDriver databaseDriver)
        {
            this.databaseDriver = databaseDriver;
            this.databaseDriver.Database = this;

            this.Data = new Dictionary<string, object>();
        }

        public Database Load(string connectionName)
        {
            this.databaseDriver.ConnectionString = ConfigurationManager.AppSettings[connectionName];
            return this;
        }

        public Database LoadConnectionString(string connectionString)
        {
            this.databaseDriver.ConnectionString = connectionString;
            return this;
        }

        public Database Adapter(IDatabaseDriver databaseDriver)
        {
            this.databaseDriver = databaseDriver;
            this.databaseDriver.Database = this;
            return this;
        }

        public Database Select(string selectClause)
        {
            this.selectClauses.Add(selectClause);
            return this;
        }

        public Database From(string tableName)
        {
            this.tableName = tableName;
            return this;
        }

        public Database Join(string tableName, string _join)
        {
            return Join(tableName, _join, "INNER JOIN");
        }

        public Database Join(string tableName, string _join, string type)
        {
            this.joinClauses.Add(new JoinClause(tableName, _join, type));
            return this;
        }

        public Database Where(string _where)
        {
            this.whereClauses.Add(_where);
            return this;
        }

        public Database Where(Array array)
        {
            foreach (var key in array.Keys) {
                var value = array[key];
                this.whereClauses.Add(new WhereClause(key.ToString(), value).ToString());
            }
            return this;
        }

        public Database Where(WhereClause _where)
        {
            this.whereClauses.Add(_where.ToString());
            return this;
        }

        public Database Where(string columnName, object _value)
        {
            return Where(columnName, _value, "=");
        }

        public Database Where(string columnName, object _value, string _operator)
        {
            var w = new WhereClause(columnName, _value, _operator);
            return Where(w);
        }

        public Database OrderBy(string columnName)
        {
            return OrderBy(columnName, "ASC");
        }

        public Database OrderBy(string columnName, string order)
        {
            this.orderByClauses.Add(new OrderByClause(columnName, order));
            return this;
        }

        public Database GroupBy(string groupBy)
        {
            this.groupByClause = groupBy;
            return this;
        }

        public Database Set(string column, object value)
        {
            this.Data.Add(column, value);
            return this;
        }

        public Database Limit(int limit)
        {
            this._limit = limit;
            return this;
        }

        public Database Get(string tableName)
        {
            this.tableName = tableName;
            return this;
        }

        public Database GetWhere(string tableName, Array _where)
        {
            foreach (var key in _where.Keys) {
                var value = _where[key];
                this.whereClauses.Add(new WhereClause(key.ToString(), value).ToString());
            }
            return this;
        }

        public Database GetWhere(string tableName, WhereClause _where)
        {
            this.tableName = tableName;
            this.whereClauses.Add(_where.ToString());
            return this;
        }

        public Database GetWhere(string tableName, string _where)
        {
            this.tableName = tableName;
            this.whereClauses.Add(_where);
            return this;
        }

        internal void ClearQuery()
        {
            selectClauses.Clear();
            joinClauses.Clear();
            whereClauses.Clear();
            columns.Clear();
        }

        public int Drop(string tableName)
        {
            return databaseDriver.Drop(tableName);
        }

        public int Create(string tableName)
        {
            return databaseDriver.Create(tableName, this.columns.ToArray());
        }

        public Database Columns(params DatabaseColumn[] columns)
        {
            this.columns.AddRange(columns);
            return this;
        }

        public Database Column(string name)
        {
            return Column(name, DbType.String);
        }

        public Database Column(string name, DbType type)
        {
            return Column(name, type, false, false);
        }

        public Database Column(string name, DbType type, bool isPrimaryKey, bool isAutoIncrement)
        {
            this.columns.Add(new DatabaseColumn { Name = name, Type = type, IsPrimaryKey = isPrimaryKey, IsAutoIncrement = isAutoIncrement });
            return this;
        }

        /*public int Insert(string tableName, Dictionary<string, object> data)
        {
            return databaseDriver.Insert(tableName, data);
        }*/

        public int Insert<T>(string tableName, T data)
        {
            insertedId = databaseDriver.Insert<T>(tableName, data);
            return insertedId;
        }

        public int Insert<T>(string tableName, List<T> data)
        {
            return databaseDriver.Insert<T>(tableName, data);
        }

        public int Insert(string tableName, Array data)
        {
            return databaseDriver.Insert(tableName, data);
        }

        public int InsertedId()
        {
            return insertedId;
        }

        public List<T> Result<T>()
        {
            return databaseDriver.Result<T>();
        }

        public List<T> Query<T>(string query)
        {
            return databaseDriver.Query<T>(query);
        }

        public T Row<T>()
        {
            return databaseDriver.Row<T>();
        }

        public T QueryFirst<T>(string query)
        {
            return databaseDriver.QueryFirst<T>(query);
        }

        public int Count()
        {
            return Count("");
        }

        public int Count(string tableName)
        {
            return this.databaseDriver.Count(tableName);
        }

        public int Update(string tableName)
        {
            this.tableName = tableName;
            return databaseDriver.Update(tableName);
        }

        public int Update<T>(string tableName, T data, Array _where)
        {
            this.tableName = tableName;
            return databaseDriver.Update(tableName, data, _where);
        }

        public int Update<T>(string tableName, T data, string _where)
        {
            this.tableName = tableName;
            return databaseDriver.Update(tableName, data, _where);
        }

        public int Update(string tableName, object data, Array _where)
        {
            this.tableName = tableName;
            return databaseDriver.Update(tableName, data, _where);
        }

        public int Delete(string tableName, Array _where)
        {
            return databaseDriver.Delete(tableName, _where);
        }

        public int Delete(string tableName, string _where)
        {
            return databaseDriver.Delete(tableName, _where);
        }

        public int Truncate(string tableName)
        {
            return databaseDriver.Truncate(tableName);
        }

        public static string SqlSanitize(object value)
        {
            string sanitizedStr = value.ToString().Replace("'", "");
            return sanitizedStr;
        }

        public void QueryFromFile(string filePath)
        {
            databaseDriver.QueryFromFile(filePath);
        }

        public class WhereClause
        {
            public string ColumnName { get; set; }
            public object Value { get; set; }
            public string Operator { get; set; }

            public WhereClause(string columnName, object _value) : this(columnName, _value, " = ")
            {
            }

            public WhereClause(string columnName, object _value, string _operator)
            {
                this.ColumnName = columnName;
                this.Value = _value;
                this.Operator = _operator;
            }

            public override string ToString()
            {
                string value = "";
                if (Value is string || Value is DateTime) {
                    value += "'" + Database.SqlSanitize(Value) + "'";
                } else {
                    value = Value.ToString();
                }
                return ColumnName + Operator + value;
            }

            public static List<WhereClause> New(Array array)
            {
                var w = new List<WhereClause>();
                foreach (var key in array.Keys) {
                    var value = array[key];
                    w.Add(new WhereClause(key.ToString(), value));
                }
                return w;
            }
        }

        public class JoinClause
        {
            public string Type { get; set; }
            public string Join { get; set; }
            public string TableName { get; set; }

            public JoinClause(string tableName, string _join, string type)
            {
                this.TableName = tableName;
                this.Join = _join;
                this.Type = type;
            }
        }

        public class OrderByClause
        {
            public string ColumnName { get; set; }
            public string Order { get; set; }

            public OrderByClause(string columnName, string order)
            {
                this.ColumnName = columnName;
                this.Order = order;
            }
        }
    }

    public class DatabaseColumn
    {
        public string Name { get; set; }
        public DbType Type { get; set; }
        public int? Size { get; set; }
        public bool IsPrimaryKey { get; set; }
        public bool IsAutoIncrement { get; set; }
    }

    public class DatabaseEventArgs : EventArgs
    {
        public string Query { get; set; }
        public string StyledQuery { get; set; }
        public string Connection { get; set; }

        public DatabaseEventArgs(string query, string styledQuery)
        {
            this.Query = query;
            this.StyledQuery = styledQuery;
        }

        public DatabaseEventArgs()
        {
        }
    }
}