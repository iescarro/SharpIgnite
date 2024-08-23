using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.SessionState;
using System.Web.UI;
using System.Reflection;
using System.Web;

namespace SharpIgnite
{
    public class Loader
    {
        Controller controller;
    
        public Loader(Controller controller)
        {
            this.controller = controller;
        }
    
        public void View(string view)
        {
            var viewControl = controller.Page.LoadControl("Views/" + view + ".ascx") as SharpIgnite.WebView;
            controller.Page.Controls.Add(viewControl);
        }
    
        public void View(string view, SharpIgnite.Array data)
        {
            var viewControl = controller.Page.LoadControl("Views/" + view + ".ascx") as SharpIgnite.WebView;
            viewControl.Data = data;
            controller.Page.Controls.Add(viewControl);
        }
    }
}
