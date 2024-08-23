using System;
using System.Collections.Generic;
using System.Reflection;

namespace SharpIgnite
{
    public class Model<T>
    {
        public Model()
        {
        }

        public Database Database {
            get {
                return WebApplication.Instance.Database;
            }
        }

        string TableName {
            get {
                Type type = this.GetType();

                TableAttribute tableAttribute = (TableAttribute)Attribute.GetCustomAttribute(type, typeof(TableAttribute));
                string tableName = type.Name;
                if (tableAttribute != null && !tableAttribute.Name.IsNullOrEmpty()) {
                    tableName = tableAttribute.Name;
                }
                return tableName;
            }
        }

        public T Save()
        {
            Database.Insert(TableName, this);
            return (T)(object)this;
        }

        public int Update()
        {
            var w = ToArray(GetPrimaryKeys());
            return Database
                .Update(TableName, this, w);
        }

        public void Delete()
        {
            var w = ToArray(GetPrimaryKeys());
            Database.Delete(TableName, w);
        }

        Array ToArray(List<PropertyInfo> properties)
        {
            var array = new Array();
            foreach (var property in properties) {
                string propertyName;
                if (property.GetCustomAttribute<ColumnAttribute>() != null) {
                    ColumnAttribute column = (ColumnAttribute)property.GetCustomAttribute(typeof(ColumnAttribute));
                    if (column.Name != null) {
                        propertyName = column.Name;
                    } else {
                        propertyName = property.Name;
                    }
                    object value = property.GetValue(this);
                    array.Add(propertyName, value);
                }
            }
            return array;
        }

        List<PropertyInfo> GetPrimaryKeys()
        {
            var primaryProperties = new List<PropertyInfo>();
            foreach (var p in GetColumnProperties()) {
                ColumnAttribute column = (ColumnAttribute)p.GetCustomAttribute(typeof(ColumnAttribute));
                if (column != null && column.IsPrimaryKey) {
                    primaryProperties.Add(p);
                }
            }
            return primaryProperties;
        }

        List<PropertyInfo> GetColumnProperties()
        {
            Type type = this.GetType();
            var columnProperties = new List<PropertyInfo>();
            foreach (PropertyInfo property in type.GetProperties()) {
                if (property.GetCustomAttribute<ColumnAttribute>() != null) {
                    columnProperties.Add(property);
                }
            }
            return columnProperties;
        }

        static Model<T> GetInstance()
        {
            var t = Activator.CreateInstance<T>();
            return (t as Model<T>);
        }

        public static T Read(Array array)
        {
            var t = GetInstance();
            return t.Database
                .From(t.TableName)
                .Where(array)
                .Row<T>();
        }

        public static List<T> Find()
        {
            return Find(Array.New());
        }

        public static List<T> Find(Array where)
        {
            var t = GetInstance();
            return t.Database
                .From(t.TableName)
                .Where(where)
                .Result<T>();
        }

        public static Database OrderBy(string columnName, string order)
        {
            var t = GetInstance();
            return t.Database
                .From(t.TableName)
                .OrderBy(columnName, order);
        }

        public static Database Limit(int limit)
        {
            var t = GetInstance();
            return t.Database
                .From(t.TableName)
                .Limit(limit);
        }

        public static List<T> All()
        {
            var t = GetInstance();
            return t.Database
                .From(t.TableName)
                .Result<T>();
        }

        public override string ToString()
        {
            PropertyInfo[] properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            string result = GetType().Name + ": { ";
            foreach (PropertyInfo prop in properties) {
                if (prop.Name == "Database") continue;

                object value = prop.GetValue(this);

                if (value != null && value.GetType() == typeof(string)) {
                    result += prop.Name + ": '" + value + "', ";
                } else {
                    result += prop.Name + ": " + value + ", ";
                }
            }
            result = result.TrimEnd(',', ' ') + " }";
            return result;
        }

    }
}
