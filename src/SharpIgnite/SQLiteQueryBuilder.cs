using System;
using System.Collections.Generic;
using System.Reflection;

namespace SharpIgnite
{
    public class SQLiteQueryBuilder : BaseQueryBuilder
    {
        public override string Drop(Database database, string tableName)
        {
            database.tableName = tableName;
            string query = "DROP TABLE " + tableName + ";" + Endl();
            return query;
        }

        public override string Create(Database database, string tableName, params DatabaseColumn[] columns)
        {
            database.tableName = tableName;
            string cols = "";
            int i = 0;
            foreach (var c in columns) {
                if (i++ > 0) cols += ", " + Endl();
                var p = c.IsPrimaryKey ? " PRIMARY KEY" : "";
                var a = c.IsAutoIncrement ? " AUTO_INCREMENT" : "";
                cols += "  " + c.Name + " " + GetDbType(c.Type, c.Size) + p + a;
            }
            string query = "CREATE TABLE " + tableName + "(" + Endl() +
                cols + Endl() +
                ");" + Endl();
            return query;
        }

        public override string Insert<T>(Database database, string tableName, T data)
        {
            database.tableName = tableName;
            Type type = data.GetType();
            string columns = "";
            string values = "";
            string primaryKeyColumnName = "";
            foreach (PropertyInfo property in type.GetProperties()) {
                //if (property.GetCustomAttribute<ColumnAttribute>() != null) {
                object value = property.GetValue(data);
                if (property.DeclaringType == type && value != null) {
                    string columnName = "";
                    ColumnAttribute column = (ColumnAttribute)property.GetCustomAttribute(typeof(ColumnAttribute));
                    if (column != null && !column.AutoIncrement) {
                        columnName = string.IsNullOrEmpty(column.Name)
                            ? property.Name
                            : column.Name;

                        columns += columnName + ", ";
                        if (value is string || value is DateTime || value is DateTime?) {
                            values += "'" + Database.SqlSanitize(value.ToString()) + "', ";
                        } else {
                            values += value + ", ";
                        }
                    } else if (column != null && column.IsPrimaryKey) {
                        primaryKeyColumnName = string.IsNullOrEmpty(column.Name)
                            ? property.Name
                            : column.Name;
                    } else {
                        columns += property.Name + ", ";
                        if (value is string || value is DateTime || value is DateTime?) {
                            values += "'" + Database.SqlSanitize(value.ToString()) + "', ";
                        } else {
                            values += value + ", ";
                        }
                    }
                }
                //}
            }
            string primaryKeyColumn = string.IsNullOrEmpty(primaryKeyColumnName)
                ? ""
                : "";
            string query = "INSERT INTO " + tableName + "(" + columns.TrimEnd(',', ' ') + ")" + Endl() +
                primaryKeyColumn +
                "VALUES(" + values.TrimEnd(',', ' ') + ");" + Endl() +
                "SELECT LAST_INSERT_ROWID() AS LastInsertedId;" + Endl();
            return query;
        }

        public override string Insert<T>(Database database, string tableName, List<T> data)
        {
            database.tableName = tableName;
            string query = "";
            foreach (var d in data) {
                string q = Insert<T>(database, tableName, d);
                query += q + Endl();
            }
            database.LastQuery = query;
            return query;
        }

        public override string Insert(Database database, string tableName, Array data)
        {
            database.tableName = tableName;
            Type type = data.GetType();
            string columns = "";
            string values = "";
            int i = 0;
            string primaryKeyColumnName = "";
            //foreach (PropertyInfo property in type.GetProperties()) {
            foreach (var key in data.Keys) {
                //if (property.GetCustomAttribute<ColumnAttribute>() != null) {
                object value = data[key]; // property.GetValue(data);
                if (value != null) {
                    if (i > 0) {
                        columns += ", ";
                        values += ", ";
                    }
                    /*string columnName = "";
                    ColumnAttribute column = (ColumnAttribute)property.GetCustomAttribute(typeof(ColumnAttribute));
                    if (!column.IsPrimaryKey) {
                        i++;
                        columnName = string.IsNullOrEmpty(column.Name)
                            ? property.Name
                            : column.Name;

                        columns += columnName;
                        if (value is string || value is DateTime || value is DateTime?) {
                            values += "'" + Database.SqlSanitize(value.ToString()) + "'";
                        } else {
                            values += value;
                        }
                    } else {
                        primaryKeyColumnName = string.IsNullOrEmpty(column.Name)
                            ? property.Name
                            : column.Name;
                    }*/
                    i++;
                    columns += key;
                    if (value is string || value is DateTime || value is DateTime?) {
                        values += "'" + Database.SqlSanitize(value.ToString()) + "'";
                    } else {
                        values += value;
                    }
                }
                //}
            }
            string primaryKeyColumn = string.IsNullOrEmpty(primaryKeyColumnName)
                ? ""
                : "OUTPUT INSERTED." + primaryKeyColumnName + Endl();
            string query = "INSERT INTO " + tableName + "(" + columns + ")" + Endl() +
                primaryKeyColumn +
                "VALUES(" + values + ")" + Endl();
            return query;
        }

        public override string Result(Database database)
        {
            string query = GetSelectClause(database) + Endl() +
                "FROM " + database.tableName + Endl() +
                GetJoinClausesString(database) +
                GetWhereClausesString(database) +
                GetOrderByClausesString(database) +
                GetLimitString(database);

            string styledQuery = "<br>" +
                "<span style='color:#0000BB'>FROM</span ";

            database.styledLastQuery = styledQuery;
            database.LastQuery = query;
            return query;
        }

        public override string Row(Database database)
        {
            string query = GetSelectClause(database) + Endl() +
                "FROM " + database.tableName + Endl() +
                GetJoinClausesString(database) + Endl() +
                GetWhereClausesString(database);
            database.LastQuery = query;
            return query;
        }

        public override string Count(Database database, string tableName)
        {
            if (!string.IsNullOrEmpty(tableName)) {
                database.tableName = tableName;
            }
            string query = "SELECT COUNT(*)" + Endl() +
                "FROM " + database.tableName + Endl() +
                GetJoinClausesString(database) +
                GetWhereClausesString(database);
            database.LastQuery = query;
            return query;
        }

        public override string Update(Database database, string tableName)
        {
            database.tableName = tableName;
            string query = "UPDATE " + database.tableName + " SET" + Endl();
            int i = 0;
            foreach (var d in database.Data) {
                if (i++ > 0) {
                    query += "," + Endl();
                }
                var value = d.Value;
                query += d.Key + " = ";
                if (value is string) {
                    query += "'" + value + "'";
                } else {
                    query += value;
                }
            }
            query += Endl() + GetWhereClausesString(database);
            database.LastQuery = query;
            return query;
        }

        public override string Update<T>(Database database, string tableName, T data, Array _where)
        {
            var w = DatabaseHelper.ToString(Database.WhereClause.New(_where));
            return Update(database, tableName, data, w);
        }

        public override string Update<T>(Database database, string tableName, T data, string _where)
        {
            database.tableName = tableName;
            database.whereClauses.Add(_where);
            Type type = data.GetType();
            string assignments = "";
            foreach (PropertyInfo property in type.GetProperties()) {
                //if (property.GetCustomAttribute<ColumnAttribute>() != null) {
                object value = property.GetValue(data);
                if (property.DeclaringType == type && value != null) {
                    string columnName = "";
                    ColumnAttribute column = (ColumnAttribute)property.GetCustomAttribute(typeof(ColumnAttribute));
                    if (column != null && !column.IsPrimaryKey && !column.AutoIncrement) {
                        columnName = string.IsNullOrEmpty(column.Name)
                            ? property.Name
                            : column.Name;

                        assignments += columnName + " = ";
                        if (value is string || value is DateTime || value is DateTime?) {
                            assignments += "'" + Database.SqlSanitize(value.ToString()) + "'";
                        } else {
                            assignments += value;
                        }
                        assignments += "," + Endl();
                    } else if (column == null) {
                        columnName = property.Name;
                        assignments += columnName + " = ";
                        if (value is string || value is DateTime || value is DateTime?) {
                            assignments += "'" + Database.SqlSanitize(value.ToString()) + "'";
                        } else {
                            assignments += value;
                        }
                        assignments += "," + Endl();
                    }
                }
                //}
            }
            string query = "UPDATE " + database.tableName + " SET" + Endl() +
                assignments.Trim().TrimEnd(',', ' ') + Endl() +
                GetWhereClausesString(database);
            database.LastQuery = query;
            return query;
        }

        public override string Delete(Database database, string tableName, Array _where)
        {
            database.tableName = tableName;
            var w = DatabaseHelper.ToString(Database.WhereClause.New(_where));
            database.whereClauses.Add(w);
            string query = "DELETE FROM " + database.tableName + Endl() +
                GetWhereClausesString(database);
            database.LastQuery = query;
            return query;
        }

        public override string Delete(Database database, string tableName, string _where)
        {
            database.tableName = tableName;
            database.whereClauses.Add(_where);
            string query = "DELETE FROM " + database.tableName + Endl() +
                GetWhereClausesString(database);
            database.LastQuery = query;
            return query;
        }

        public override string Truncate(Database database, string tableName)
        {
            string query = "TRUNCATE TABLE " + tableName + Endl();
            database.LastQuery = query;
            return query;
        }

        string GetJoinClausesString(Database database)
        {
            string joinClause = "";
            foreach (var j in database.joinClauses) {
                joinClause += j.Type + " " + j.TableName + " ON " + j.Join + Endl();
            }
            return joinClause != "" ? joinClause + Endl() : "";
        }

        string GetWhereClausesString(Database database)
        {
            string whereClause = "";
            if (database.whereClauses.Count > 0) {
                whereClause += "WHERE ";
            }
            int i = 0;
            foreach (var w in database.whereClauses) {
                if (i++ > 0) {
                    whereClause += "AND ";
                }
                whereClause += w + Endl();
            }
            return whereClause != "" ? whereClause + Endl() : "";
        }

        string GetOrderByClausesString(Database database)
        {
            string orderByClause = "";
            if (database.orderByClauses.Count > 0) {
                orderByClause += "ORDER BY ";
            }
            int i = 0;
            foreach (var o in database.orderByClauses) {
                if (i++ > 0) {
                    orderByClause += ", ";
                }
                orderByClause += o.ColumnName + " " + o.Order;
            }
            return orderByClause != "" ? orderByClause + Endl() : "";
        }

        string GetSelectClause(Database database)
        {
            var selectClause = "";
            if (database.selectClauses.Count > 0) {
                selectClause += "SELECT  ";
            } else {
                selectClause += "SELECT * ";
            }
            int i = 0;
            foreach (var c in database.selectClauses) {
                if (i++ > 0) {
                    selectClause += ", ";
                }
                selectClause += c;
            }
            return selectClause;
        }

        string GetLimitString(Database database)
        {
            var limitClause = database._limit > 0
                ? "LIMIT " + database._limit
                : "";
            return limitClause;
        }

        string GetGroupByClauseString(string groupByClause)
        {
            return groupByClause != "" ? "GROUP BY " + groupByClause + Endl() : "";
        }
    }
}
