using System;
using System.Data;

namespace SharpIgnite
{
    [Table("SchemaMigration")]
    public abstract class Migration
    {
        public string Version { get; set; }
        public Database Database { get; set; }
        
        public Migration(string version)
        {
            this.Version = version;
            this.Database = WebApplication.Instance.Database;
        }

        public virtual void Initialize()
        {
            
        }
        
        public bool SchemaExists {
            get {
                // HACK: Improve this!
                var migrationTable = Database.QueryFirst<int>(@"
SELECT 1
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_TYPE='BASE TABLE'
    AND TABLE_NAME != 'dtproperties'
    AND TABLE_NAME != 'sysdiagrams'
    AND TABLE_NAME = 'SchemaMigration'");
                return migrationTable != 0;
            }
        }

        public bool VersionExists {
            get {
                // HACK: Improve this!
                var version = Database.QueryFirst<int>(@"
SELECT 1
FROM SchemaMigration
WHERE Version = '" + Version + "'");
                return version != 0;
            }
        }

        public virtual void Migrate()
        {
        }
        
        public virtual void Rollback()
        {
        }
        
        public void DropTable(string tableName)
        {
            OnMigrating("Dropping table " + tableName + "...");
            Database.Drop(tableName);
            OnMigrated("OK");
        }
        
        public void CreateTable(string tableName, params DatabaseColumn[] columns)
        {
            OnMigrating(new MigrationEventArgs("Creating table " + tableName + "..."));
            Database.Columns(columns)
                .Create(tableName);
            OnMigrated("OK");
        }
        
        public DatabaseColumn Column(string name)
        {
            return Column(name,  DbType.String);
        }
        
        public DatabaseColumn Column(string name, DbType type)
        {
            return Column(name, type, false, false);
        }

        public DatabaseColumn Column(string name, DbType type, int size)
        {
            return new DatabaseColumn { Name = name, Type = type, Size = size };
        }

        public DatabaseColumn Column(string name, DbType type, bool primaryKey)
        {
            return Column(name, type, primaryKey, false);
        }
        
        public DatabaseColumn Column(string name, DbType type, bool isPrimaryKey, bool isAutoIncrement)
        {
            return new DatabaseColumn { Name = name, Type = type, IsPrimaryKey = isPrimaryKey, IsAutoIncrement = isAutoIncrement };
        }
        
        public event EventHandler<MigrationEventArgs> Migrating;
        
        protected virtual void OnMigrating(string message)
        {
            OnMigrating(new MigrationEventArgs(message));
        }
        
        protected virtual void OnMigrating(MigrationEventArgs e)
        {
            if (Migrating != null) {
                Migrating(this, e);
            }
        }
        
        public event EventHandler<MigrationEventArgs> Migrated;

        public void LoadDatabaseConnection(string connection)
        {
            OnDatabaseConnectionLoad(new DatabaseEventArgs { Connection = connection });
        }

        public event EventHandler<DatabaseEventArgs> DatabaseConnectionLoad;

        protected virtual void OnDatabaseConnectionLoad(DatabaseEventArgs e)
        {
            if (DatabaseConnectionLoad != null) {
                DatabaseConnectionLoad(this, e);
            }
        }

        protected virtual void OnMigrated(string message)
        {
            OnMigrated(new MigrationEventArgs(message));
        }
        
        protected virtual void OnMigrated(MigrationEventArgs e)
        {
            if (Migrated != null) {
                Migrated(this, e);
            }
        }
    }
    
    public class MigrationEventArgs : EventArgs
    {
        public Migration Migration { get; set; }
        public string Message { get; set; }
        
        public MigrationEventArgs(string message)
        {
            this.Message = message;
        }
    }
}
