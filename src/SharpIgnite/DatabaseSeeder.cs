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
    public abstract class DatabaseSeeder
    {
        IDatabaseDriver databaseDriver;

        public virtual void Initialize()
        {
        }
        
        public virtual void Run()
        {
        }
        
        public virtual void Rollback()
        {
        }
        
        protected Database db;
        
        public Database Database {
            get { return db; }
            set { db = value; }
        }
        
        public DatabaseSeeder(string connectionString)
        {
            var databaseDriver = new SqlDatabaseDriver(connectionString); // TODO: Make this configurable!
            this.db = new Database(databaseDriver);
        }
        
        public DatabaseSeeder()
        {
            var connectionString = ConfigurationManager.AppSettings["SqlConnection"];
            var databaseDriver = new SqlDatabaseDriver(connectionString); // TODO: Make this configurable!
            this.db = new Database(databaseDriver);
        }
        
        public DatabaseSeeder(IDatabaseDriver databaseDriver)
        {
            this.databaseDriver = databaseDriver;
            this.db = new Database(databaseDriver);
        }
        
        public event EventHandler<SeederEventArgs> Seeding;

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

        protected virtual void OnSeeding(string message)
        {
            OnSeeding(new SeederEventArgs(message));
        }
        
        protected virtual void OnSeeding(SeederEventArgs e)
        {
            if (Seeding != null) {
                Seeding(this, e);
            }
        }
    }
    
    public class SeederEventArgs : EventArgs
    {
        public string Message { get; set; }
        
        public SeederEventArgs()
        {
        }
        
        public SeederEventArgs(string message)
        {
            this.Message = message;
        }
    }
}
