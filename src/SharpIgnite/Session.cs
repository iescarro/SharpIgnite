using System.Web;

namespace SharpIgnite
{
    public class Session
    {
        IMyHttpSession session;

        public Session() : this(new MyHttpSession())
        {
        }

        public Session(IMyHttpSession session)
        {
            this.session = session;
        }

        public void Remove(string name)
        {
            session.Remove(name);
        }

        public void RemoveAll()
        {
            session.RemoveAll();
        }

        public T Get<T>(string name)
        {
            return Get<T>(name, default(T));
        }

        public T Get<T>(string name, T defaultValue)
        {
            if (session[name] != null) {
                return (T)session[name];
            }
            return defaultValue;
        }

        public object Get(string name)
        {
            return Get(name, null);
        }

        public object Get(string name, object defaultValue)
        {
            if (session[name] != null) {
                return session[name];
            }
            return defaultValue;
        }

        public void Set(string name, object value)
        {
            HttpContext.Current.Session[name] = value;
        }

        public string FlashData(string name)
        {
            return FlashData(name, "");
        }

        public string FlashData(string name, string value)
        {
            if (!string.IsNullOrEmpty(value)) {
                session[name] = value;
            } else {
                if (session[name] != null) {
                    value = session[name].ToString();
                    session.Remove(name);
                }
            }
            return value;
        }
    }

    public interface IMyHttpSession
    {
        void RemoveAll();
        void Remove(string name);
        object this[string name] { get; set; }
    }

    public class MyHttpSession : IMyHttpSession
    {
        public MyHttpSession()
        {
        }

        public void RemoveAll()
        {
            HttpContext.Current.Session.RemoveAll();
        }

        public void Remove(string name)
        {
            HttpContext.Current.Session.Remove(name);
        }

        public object this[string name] {
            get {
                return HttpContext.Current.Session[name];
            }
            set {
                HttpContext.Current.Session[name] = value;
            }
        }

    }
}
