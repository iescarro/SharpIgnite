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
    public class ColumnAttribute : Attribute
    {
        public string Name { get; set; }
        public bool IsPrimaryKey { get; set; }
        public bool AutoIncrement { get; set; }
        
        public ColumnAttribute(bool isPrimaryKey)
        {
            this.IsPrimaryKey = isPrimaryKey;
        }
        
        public ColumnAttribute(string name) : this(name, false)
        {
        }
        
        public ColumnAttribute(string name, bool isPrimaryKey) : this(name, isPrimaryKey, false)
        {
        }
        
        public ColumnAttribute(string name, bool isPrimaryKey, bool autoIncrement)
        {
            this.Name = name;
            this.IsPrimaryKey = isPrimaryKey;
            this.AutoIncrement = autoIncrement;
        }
        
        public ColumnAttribute()
        {
        }
    }
    
    public class TableAttribute : Attribute
    {
        public string Name { get; set; }
        
        public TableAttribute()
        {
        }
        
        public TableAttribute(string name)
        {
            this.Name = name;
        }
    }
}
