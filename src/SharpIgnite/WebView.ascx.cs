using System;

namespace SharpIgnite
{
    public partial class WebView : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public SharpIgnite.Array Data { get; set; }
        public SharpIgnite.Array ViewData { get; set; }

        public string CsrfToken()
        {
            return Form.Hidden("__token", Guid.NewGuid().ToString());
        }
    }
}