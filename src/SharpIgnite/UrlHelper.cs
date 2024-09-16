using System;
using System.Web;

namespace SharpIgnite
{
    public static class UrlHelper
    {
        /*public static Dictionary<string, object> ToDictionary(this NameValueCollection c)
        {
            var d = new Dictionary<string, object>();
            foreach (var k in c) {
                if (k != null) {
                    d.Add(k.ToString(), c[k.ToString()]);
                }
            }
            return d;
        }

        public static bool IsValidXForwardFor(string domain)
        {
            bool isValidDomain = false;

            IPAddress ipAddress;
            System.Net.IPAddress.TryParse(domain, out ipAddress);
            if (ipAddress != null) {
                isValidDomain = true;
            } else {
                string pattern = @"^(?!:\/\/)(?:(?:[a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9\-]*[a-zA-Z0-9])\.)*(?:[A-Za-z0-9]|[A-Za-z0-9][A-Za-z0-9\-]*[A-Za-z0-9])$";
                isValidDomain = Regex.IsMatch(domain, pattern);
            }
            return isValidDomain;
        }*/

        public static void RedirectIf(bool condition, string url)
        {
            RedirectIf(condition, url, false);
        }

        public static void RedirectIf(bool condition, string url, bool random)
        {
            if (condition) {
                if (random) {
                    HttpContext.Current.Response.Redirect(string.Format("{0}?Rnd={1}", url, NumberHelper.Random()), true);
                } else {
                    HttpContext.Current.Response.Redirect(url, true);
                }
            }
        }

        public static void Redirect(string url)
        {
            Redirect(url, false);
        }

        public static void Redirect(string url, bool random)
        {
            if (random) {
                HttpContext.Current.Response.Redirect(string.Format("{0}?Rnd={1}", url, NumberHelper.Random()), true);
            } else {
                HttpContext.Current.Response.Redirect(url, true);
            }
        }
    }

    public class UrlEventArgs : EventArgs
    {
        public string Url { get; set; }

        public UrlEventArgs(string url)
        {
            this.Url = url;
        }

        public UrlEventArgs() : this("")
        {
        }
    }
}
