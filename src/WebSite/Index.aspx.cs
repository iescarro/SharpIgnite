using SharpIgnite;
using System;
using System.Reflection;

namespace WebSite
{
    public partial class Index : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SharpIgnite.Application.Instance.Routes
                .Add("/", "posts");

            SharpIgnite.Application.Run(Request.Url.ToString(),
                this,
                Assembly.GetExecutingAssembly());
        }
    }
}

namespace WebSite.Controllers
{
    public class HomeController : Controller
    {
        public void Hello()
        {
            Write("Hello world");
        }
    }

}