using System;
using System.Configuration;

namespace SharpIgnite
{
    public abstract class Seeder
    {
        IDatabaseAdapter databaseAdapter;

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

        public Seeder(string connectionString)
        {
            var databaseAdapter = new SqlDatabaseAdapter(connectionString); // TODO: Make this configurable!
            this.db = Database.Instance.Adapter(databaseAdapter); // new Database(databaseDriver);
        }

        public Seeder()
        {
            var connectionString = ConfigurationManager.AppSettings["SqlConnection"];
            var databaseAdapter = new SqlDatabaseAdapter(connectionString); // TODO: Make this configurable!
            this.db = Database.Instance.Adapter(databaseAdapter); // new Database(databaseDriver);
        }

        public Seeder(IDatabaseAdapter databaseAdapter)
        {
            this.databaseAdapter = databaseAdapter;
            this.db = Database.Instance.Adapter(databaseAdapter);// new Database(databaseDriver);
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
