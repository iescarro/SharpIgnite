using System;
using System.Collections.Generic;
using System.Reflection;

namespace SharpIgnite
{
    public class WebApplication
    {
        static WebApplication instance;
        string entryPageName;
        public Input Input { get; set; }
        public Output Output { get; set; }
        public Router Routes { get; set; }
        public Lang Lang { get; set; }
        public Session Session { get; set; }
        static System.Web.UI.Page entryPage;
        Database database;
        ServiceCollection serviceCollection;

        public Database Database {
            get { return database; }
        }

        public string BaseUrl(string url)
        {
            return BaseUrl(url, entryPageName != null);
        }

        public string BaseUrl(string url, bool includeEntryPoint)
        {
            if (includeEntryPoint) {
                return "/" + entryPageName + "/" + url;
            }
            return "/" + url;
        }

        public static WebApplication Instance {
            get {
                if (instance == null) {
                    instance = new WebApplication();
                }
                return instance;
            }
        }

        private WebApplication()
        {
            Input = new Input();
            Output = new Output();
            Routes = new Router();
            Lang = new Lang();
            Session = new Session();

            var connectionString = Config.Item("DbConnection"); ;
            var databaseDriver = GetDatabaseDriver(connectionString);
            database = new Database(databaseDriver);

            serviceCollection = new ServiceCollection();
        }

        public T GetSession<T>(string name)
        {
            return GetSession(name, default(T));
        }

        public T GetSession<T>(string name, T defaultValue)
        {
            return Session.Get<T>(name, defaultValue);
        }

        public object GetSession(string name)
        {
            return GetSession(name, null);
        }

        public object GetSession(string name, object defaultValue)
        {
            return Session.Get(name, defaultValue);
        }

        public void SetSession(string name, object value)
        {
            Session.Set(name, value);
        }

        public void SetSessionIf(bool condition, string name, object value)
        {
            if (condition) {
                Session.Set(name, value);
            }
        }

        public void RemoveSession(params string[] sessions)
        {
            foreach (var session in sessions) {
                Session.Remove(session);
            }
        }

        IDatabaseDriver GetDatabaseDriver(string connectionString)
        {
            string dbDriver = Config.Item("DbAdapter");
            switch (dbDriver) {
                case "MySql.Data.MySqlClient":
                    return new MySqlDatabaseDriver(connectionString, new MySqlQueryBuilder());
                case "System.Data.SQLite":
                    return new SQLiteDatabaseDriver(connectionString, new SQLiteQueryBuilder());
                default:
                    return new SqlDatabaseDriver(connectionString, new SqlQueryBuilder());
            }
        }

        public static void Run(string url, System.Web.UI.Page entryPage, Assembly assembly)
        {
            var app = Instance;

            var route = new Route(new Uri(url));

            WebApplication.entryPage = entryPage;
            app.entryPageName = route.PageName;

            if (app.Routes.ContainsKey(route.Name)) {
                var mappedRoute = app.Routes[route.Name];
                mappedRoute.Execute(entryPage, assembly);
            } else {
                route.Execute(entryPage, assembly);
            }
        }

        //static void Write(string text)
        //{
        //    entryPage.Response.Write(text);
        //}

        //static void WriteLine(string text)
        //{
        //    entryPage.Response.Write(text + "<br>");
        //}

        public ServiceCollection ServiceCollection {
            get { return serviceCollection; }
        }
    }

    public class ServiceCollection
    {
        public ServiceProvider BuildServiceProvider()
        {
            return new ServiceProvider(this);
        }

        internal List<object> services = new List<object>();

        public void AddTransient<I, O>() where O : I, new()
        {
            // Ensure that O implements I
            if (!typeof(I).IsAssignableFrom(typeof(O))) {
                throw new InvalidOperationException(typeof(O).Name + " does not implement " + typeof(I).Name);
            }

            // Instantiate O
            I instance = new O();
            this.services.Add(instance);
        }
    }

    public class ServiceProvider
    {
        ServiceCollection serviceCollection;

        public ServiceProvider(ServiceCollection serviceCollection)
        {
            this.serviceCollection = serviceCollection;
        }

        public T GetService<T>()
        {
            foreach (var s in serviceCollection.services) {
                if (s is T) {
                    return (T)s;
                }
            }
            return default(T);
        }
    }
}
