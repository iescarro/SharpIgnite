using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SharpIgnite
{
    public class ValidationError
    {
        public string Message { get; set; }

        public ValidationError() { }

        public ValidationError(string message)
        {
            this.Message = message;
        }
    }

    public partial class Model<T> : Validator
    {
        public Model()
        {
            Errors = new List<string>();
            validators = new List<IValidator>();
        }

        // TODO: Remove me, not needed, please refer to Id property of specific model!
        //public virtual int Id { get; set; }

        public override bool IsValid {
            get {
                Validate(this);
                return Errors.Count <= 0;
            }
        }

        public override void Validate(object obj)
        {
            Errors.Clear();
            ExecuteValidators();
            var type = obj.GetType();
            var properties = type.GetProperties();
            foreach (var property in properties) {
                var validatorAttributes = property
                    .GetCustomAttributes<ValidatorAttribute>()
                    .ToList();
                if (validatorAttributes.Any()) {
                    // Iterate over each ValidatorAttribute (e.g., Required, IsEmail)
                    foreach (var attribute in validatorAttributes) {
                        var validator = attribute.Validator;
                        object value = property.GetValue(obj);
                        validator.Validate(value);
                        AddErrorIf(!validator.IsValid, attribute.Message);
                    }
                }
            }
        }

        void AddErrorIf(bool condition, string message)
        {
            if (condition) {
                Errors.Add(message);
            }
        }

        public void ExecuteValidators()
        {
            foreach (var validator in validators) {
                validator.Validate(this);
            }
        }

        public void ValidatesWith(IValidator validator)
        {
            this.validators.Add(validator);
        }

        public List<string> Errors { get; set; }
        List<IValidator> validators;

        public Database Database {
            get {
                // TODO: Doesn't need to be dependent on the whole SharpIgnite Application
                return Application.Instance.Database;
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

        //public static T Read(Array array)
        public static T Read(object array)
        {
            var t = GetInstance();
            return t.Database
                .From(t.TableName)
                .Where(array)
                .Row<T>();
        }

        public static List<T> Find()
        {
            return Find(new { });
        }

        public static List<T> Find(object where)
        {
            var t = GetInstance();
            return t.Database
                .From(t.TableName)
                .Where(where)
                .Result<T>();
        }

        public static Database Where(object where)
        {
            var t = GetInstance();
            return t.Database
                .From(t.TableName)
                .Where(where);
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
