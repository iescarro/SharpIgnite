using System;
using System.Web.UI;

namespace SharpIgnite
{
    public class Controller
    {
        [Obsolete] public Loader Load { get; set; }
        public Input Input { get; set; }
        public Output Output { get; set; }
        public Localizer @Localizer { get; set; }
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
            @Localizer = new Localizer();
            Session = new Session();
        }

        public Database Database {
            get { return Application.Instance.Database; }
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

        public IActionResult Redirect(string url)
        {
            return new RedirectActionResult(url);
        }

        public IActionResult RedirectToAction(string controller, string action)
        {
            string url = controller + "/" + action;
            var obj = Application.Instance;
            Page.Response.Redirect(obj.BaseUrl(url));
            return null; // TODO: Make a better implementation of this!
        }

        public IActionResult View(string view)
        {
            return View(view, null);
        }

        public IActionResult View(string view, Array model)
        {
            return new ActionResult(view, model);
        }

        public IActionResult Content(string content)
        {
            return new StringActionResult(content);
        }
    }

    public interface IActionResult
    {
        void Render(Page entryPage);
    }

    public class RedirectActionResult : IActionResult
    {
        string url;

        public RedirectActionResult(string url)
        {
            this.url = url;
        }

        public void Render(Page entryPage)
        {
            var obj = Application.Instance;
            entryPage.Response.Redirect(obj.BaseUrl(url));
        }
    }

    public class StringActionResult : IActionResult
    {
        string content;

        public StringActionResult(string content)
        {
            this.content = content;
        }

        public override string ToString()
        {
            return content;
        }

        public void Render(Page entryPage)
        {
            entryPage.Response.Write(ToString());
        }
    }

    public class ActionResult : IActionResult
    {
        public string View { get; set; }
        public Array ViewData { get; set; }

        public ActionResult(string view, Array model)
        {
            this.ViewData = model;
            this.View = view;
        }

        public void Render(Page entryPage)
        {
            var viewControl = entryPage.LoadControl("Views/" + View + ".ascx") as SharpIgnite.WebView;
            viewControl.ViewData = ViewData;
            entryPage.Controls.Add(viewControl);
        }
    }
}
