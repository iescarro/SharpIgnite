using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Web;

namespace SharpIgnite
{
    public class Input
    {
        IHttpRequest request;

        public Input()
        {
            this.request = new MyHttpRequest();
        }

        public Input(IHttpRequest request)
        {
            this.request = request;
        }

        public string RequestType {
            get { return request.RequestType; }
        }

        public string AcceptLanguage {
            get {
                string[] languages = request.UserLanguages;
                if (languages == null || languages.Length == 0) {
                    return null;
                }
                try {
                    string language = languages[0].ToLowerInvariant().Trim();
                    var culture = CultureInfo.CreateSpecificCulture(language);
                    return culture.Name;
                } catch (ArgumentException) {
                    return null;
                }
            }
        }

        public bool Post()
        {
            return request.Form.Count > 0;
        }

        public string Post(string name)
        {
            return Post(name, null);
        }

        public string Post(string name, string defaultValue)
        {
            var request = HttpContext.Current.Request;
            if (request.Form[name] != null) {
                return request.Form[name];
            }
            return defaultValue;
        }

        public T Post<T>(string name)
        {
            return Post<T>(name, default(T));
        }

        public T Post<T>(string name, T defaultValue)
        {
            if (request.Form[name] != null) {
                return (T)Convert.ChangeType(request.Form[name], typeof(T));
            }
            return defaultValue;
        }

        public string Get(string key)
        {
            return request.QueryString[key];
        }

        public string Get(string key, object defaultValue)
        {
            if (request.QueryString[key] != null) {
                return request.QueryString[key];
            }
            return defaultValue.ToString();
        }

        public T Get<T>(string key)
        {
            return Get<T>(key, default(T));
        }

        public T Get<T>(string key, T defaultValue)
        {
            if (request.QueryString[key] != null) {
                return (T)Convert.ChangeType(request.QueryString[key], typeof(T));
            }
            return defaultValue;
        }

        public object Cookie(string name)
        {
            return request.Cookies[name];
        }

        // Server

        // IP Address
        public string GetIPAddress()
        {
            string ip = (
                request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null
                && request.ServerVariables["HTTP_X_FORWARDED_FOR"] != ""
                ? request.ServerVariables["HTTP_X_FORWARDED_FOR"]
                : request.ServerVariables["REMOTE_ADDR"]
               );

            if (ip.IndexOf(",") >= 0) {
                string[] ips = ip.Split(',');
                ip = ips[ips.Length - 1];
            }
            if (ip.IndexOf(":") >= 0) {
                string firstSplit = ip.Split(':')[0];
                if (firstSplit == "") {
                    ip = "127.0.0.1";
                } else {
                    ip = firstSplit;
                }
            }
            return ip.Trim().Replace("'", "");
        }
    }

    public interface IHttpRequest
    {
        NameValueCollection ServerVariables { get; }
        NameValueCollection Form { get; }
        NameValueCollection QueryString { get; }
        HttpCookieCollection Cookies { get; }
        string[] UserLanguages { get; }
        string RequestType { get; }
    }

    public class MyHttpRequest : IHttpRequest
    {
        public MyHttpRequest()
        {
        }

        public NameValueCollection Form {
            get { return HttpContext.Current.Request.Form; }
        }

        public NameValueCollection QueryString {
            get { return HttpContext.Current.Request.QueryString; }
        }

        public NameValueCollection ServerVariables {
            get { return HttpContext.Current.Request.ServerVariables; }
        }

        public HttpCookieCollection Cookies {
            get { return HttpContext.Current.Request.Cookies; }
        }

        public string[] UserLanguages {
            get { return HttpContext.Current.Request.UserLanguages; }
        }

        public string RequestType {
            get { return HttpContext.Current.Request.RequestType; }
        }
    }
}
