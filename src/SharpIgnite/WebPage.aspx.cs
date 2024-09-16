using System;
using System.IO;
using System.Web.UI;

namespace SharpIgnite
{
    public partial class WebPage : System.Web.UI.Page
    {
        public WebPage() : base()
        {
            var database = this.Database;

            database.QueryChanged += new EventHandler<DatabaseEventArgs>(DatabaseQueryChanged);
        }

        string databaseOutput = "";

        void DatabaseQueryChanged(object sender, DatabaseEventArgs e)
        {
            var s = @"<code>" + e.StyledQuery + "</code>";
            databaseOutput += s;
        }

        public void Redirect(string url)
        {
            UrlHelper.Redirect(url, false);
        }

        public Database Database {
            get { return SharpIgnite.Application.Instance.Database; }
        }

        public Input Input {
            get { return SharpIgnite.Application.Instance.Input; }
        }

        public Output Output {
            get { return SharpIgnite.Application.Instance.Output; }
        }

        public Localizer @Localizer {
            get { return SharpIgnite.Application.Instance.Localizer; }
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
                var services = SharpIgnite.Application.Instance.ServiceCollection;
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
            return SharpIgnite.Application.Instance.GetSession<T>(name, defaultValue);
        }

        public object GetSession(string name)
        {
            return GetSession(name, null);
        }

        public object GetSession(string name, object defaultValue)
        {
            return SharpIgnite.Application.Instance.GetSession(name, defaultValue);
        }

        [Obsolete()]
        public void SetSession(string name, object value)
        {
            SharpIgnite.Application.Instance.SetSession(name, value);
        }

        public void SessionSet(string name, object value)
        {
            SharpIgnite.Application.Instance.SessionSet(name, value);
        }

        public void SetSessionIf(bool condition, string name, object value)
        {
            if (condition) {
                SharpIgnite.Application.Instance.SetSession(name, value);
            }
        }

        public void RemoveSession(params string[] sessions)
        {
            foreach (var session in sessions) {
                SharpIgnite.Application.Instance.RemoveSession(sessions);
            }
        }

        public void RemoveSessionIf(bool condition, string name)
        {
            if (condition) {
                SharpIgnite.Application.Instance.RemoveSession(name);
            }
        }

        public string PropertiesDirectory {
            get {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Properties");
            }
        }
    }
}