using System;
using System.Collections.Generic;

namespace SharpIgnite
{
    public static class HtmlHelper
    {

        public static string MailTo(string email, string text, string extra)
        {
            return string.Format("<a href='mailto:{0}' {2}>{1}</a>", email, text, extra);
        }

        public static string Anchor(string url, string text)
        {
            return Anchor(url, text, "");
        }

        public static string Anchor(string url, string text, string attributes)
        {
            return Anchor(url, text, attributes, false);
        }

        public static string Anchor(string url, string text, string attributes, bool random)
        {
            var obj = WebApplication.Instance;
            if (random) {
                //url = string.Format("{0}?Rnd={1}", url, NumberHelper.Random());
                url = string.Format("{0}Rnd={1}", url + (url.Contains("?") ? "&" : "?"), NumberHelper.Random());
                return string.Format("<a href='{0}' {1}>{2}</a>", url, attributes, text);
            } else {
                return string.Format("<a href='{0}' {1}>{2}</a>", obj.BaseUrl(url), attributes, text);
            }
        }
        
        public static string Image(string url)
        {
        	var obj = WebApplication.Instance;
            return Image(obj.BaseUrl(url, false), "");
        }

        public static string Image(string url, string alt)
        {
            return string.Format("<img src='{0}' alt='{1}'/>", url, alt);
        }

        public static string Image(string url, string alt, int width, int height)
        {
            return string.Format("<img src='{0}' width='{1}' height='{2}' alt='{3}'/>", url, width, height, alt);
        }
    }
}
