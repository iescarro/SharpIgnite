namespace SharpIgnite
{
    public partial class WebMasterPage : System.Web.UI.MasterPage
    {
        protected ServiceProvider serviceProvider {
            get {
                var services = SharpIgnite.Application.Instance.ServiceCollection;
                var provider = services.BuildServiceProvider();
                return provider;
            }
        }

        public Input Input {
            get { return SharpIgnite.Application.Instance.Input; }
        }

        public T GetSession<T>(string name)
        {
            return GetSession(name, default(T));
        }

        public T GetSession<T>(string name, T defaultValue)
        {
            return SharpIgnite.Application.Instance.Session.Get<T>(name, defaultValue);
        }

        public object GetSession(string name)
        {
            return GetSession(name, null);
        }

        public object GetSession(string name, object defaultValue)
        {
            return SharpIgnite.Application.Instance.Session.Get(name, defaultValue);
        }

        public void SetSession(string name, object value)
        {
            SharpIgnite.Application.Instance.Session.Set(name, value);
        }

        public void SetSessionIf(bool condition, string name, object value)
        {
            if (condition) {
                SharpIgnite.Application.Instance.Session.Set(name, value);
            }
        }
    }
}