using System;
using System.IO;
using System.Web.UI;

namespace SharpIgnite
{
    public partial class WebPage : System.Web.UI.Page
    {
        public WebPage() : base()
        {
            var database = Database;

            database.QueryChanged += new EventHandler<DatabaseEventArgs>(DatabaseQueryChanged);
        }

        string databaseOutput = "";

        void DatabaseQueryChanged(object sender, DatabaseEventArgs e)
        {
            var s = @"<code>" + e.StyledQuery + "</code>"; ;
            databaseOutput += s;
        }

        public void Redirect(string url)
        {
            UrlHelper.Redirect(url, false);
        }

        public Database Database {
            get { return WebApplication.Instance.Database; }
        }

        public Input Input {
            get { return WebApplication.Instance.Input; }
        }

        public Output Output {
            get { return WebApplication.Instance.Output; }
        }

        public Lang Lang {
            get { return WebApplication.Instance.Lang; }
        }

        public void Write(string s)
        {
            Response.Write(s);
        }

        public void WriteLine(string s)
        {
            Response.Write(s + "<br>");
        }

        protected ServiceProvider serviceProvider {
            get {
                var services = WebApplication.Instance.ServiceCollection;
                var provider = services.BuildServiceProvider();
                return provider;
            }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);

            if (Output.profiler) {
                writer.Write("<fieldset>");
                writer.Write("<legend>Database</legend>");
                writer.Write(@"
                    <table width='100%'>
                        <tr>
                            <td>0.0001</td>
                            <td>");
                writer.Write(databaseOutput);
                writer.Write(@"
                            </td>
                        </tr>
                    </table>");
                writer.Write("</fieldset>");
            }
        }

        public T GetSession<T>(string name)
        {
            return GetSession(name, default(T));
        }

        public T GetSession<T>(string name, T defaultValue)
        {
            return WebApplication.Instance.GetSession<T>(name, defaultValue);

        }

        public object GetSession(string name)
        {
            return GetSession(name, null);
        }

        public object GetSession(string name, object defaultValue)
        {
            return WebApplication.Instance.GetSession(name, defaultValue);
        }

        public void SetSession(string name, object value)
        {
            WebApplication.Instance.SetSession(name, value);
        }

        public void SetSessionIf(bool condition, string name, object value)
        {
            if (condition) {
                WebApplication.Instance.SetSession(name, value);
            }
        }

        public void RemoveSession(params string[] sessions)
        {
            foreach (var session in sessions) {
                WebApplication.Instance.RemoveSession(sessions);
            }
        }

        public void RemoveSessionIf(bool condition, string name)
        {
            if (condition) {
                WebApplication.Instance.RemoveSession(name);
            }
        }

        public string PropertiesDirectory {
            get {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Properties");
            }
        }
    }
}