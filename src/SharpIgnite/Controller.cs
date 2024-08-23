using System.Web.UI;

namespace SharpIgnite
{
    public class Controller
    {
        public Loader Load { get; set; }
        public Input Input { get; set; }
        public Output Output { get; set; }
        public Lang Lang { get; set; }
        public Session Session { get; set; }
        public System.Web.UI.Page Page { get; set; }
        public bool IsPostBack {
            get { return Page.IsPostBack; }
        }
        public string BaseUrl(string url)
        {
            return Page.Server.MapPath(url);
        }

        public Controller()
        {
            Load = new Loader(this);
            Input = new Input();
            Output = new Output();
            Lang = new Lang();
            Session = new Session();
        }

        public Database Database {
            get { return WebApplication.Instance.Database; }
        }

        public void Write(string text)
        {
            Output.Write(text);
        }

        public void WriteLine()
        {
            WriteLine("");
        }

        public void WriteLine(string text)
        {
            Output.Write(text + "<br>");
        }

        public void Redirect(string url)
        {
            var obj = WebApplication.Instance;
            Page.Response.Redirect(obj.BaseUrl(url));
        }
    }
}
