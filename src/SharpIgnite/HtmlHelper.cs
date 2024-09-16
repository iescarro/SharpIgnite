using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;

namespace SharpIgnite
{
    public static class HtmlHelper
    {
        [Obsolete("Please use extension method SetTextIfEmpty to TextBox control")]
        public static void SetTextIfEmpty(TextBox textBox, string text)
        {
            if (textBox.Text == "") {
                textBox.Text = text;
            }
        }

        public static string StrongIf(bool condition, string text)
        {
            if (condition) {
                return "<strong>" + text + "</strong>";
            }
            return text;
        }

        public static string ToHtml(string text)
        {
            return text.Replace("<", "&lt;").Replace(">", "&gt;");
        }

        public static string ToElementID(this string id)
        {
            id = id.Replace(" ", "-");
            return id;
        }

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
            var obj = Application.Instance;
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
            var obj = Application.Instance;
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

        public static string DropDownList<T>(IList<T> lists)
        {
            StringBuilder s = new StringBuilder();
            s.AppendLine("<ul>");
            foreach (var l in lists) {
                s.AppendLine(string.Format("<li>{0}</li>", l));
            }
            s.AppendLine("</ul>");
            return s.ToString();
        }

        public static void Write(object obj, HttpResponse reponse)
        {
            if (obj is MemoryStream) {
                reponse.BinaryWrite(((MemoryStream)obj).ToArray());
                reponse.End();
            } else if (obj is string) {
                reponse.Write((string)obj);
            }
        }

        [Obsolete()]
        public static void AddHeaderIf(bool condition, string name, string value, HttpResponse response)
        {
            if (condition) {
                response.AddHeader(name, value);
            }
        }

        public static string List(Array options, string selected, string extra)
        {
            return List(options, selected, extra);
        }

        public delegate void ActionRef(ref string element, object key);

        public static string List(Array options, string selected, string extra, ActionRef handleListItems)
        {
            return List(options, selected, extra, handleListItems, false);
        }

        public static string List(Array lists, string selected, string extra, ActionRef handleListItems, bool ordered)
        {
            string e = ordered ? "ol" : "ul";
            string element = "<" + e + " " + extra + ">";
            if (lists != null) {
                foreach (var key in lists.Keys) {
                    var value = lists[key];
                    if (handleListItems != null) {
                        element += "<li ";
                        handleListItems(ref element, key);
                        element += ">" + value + "</li>";
                    } else {
                        element += "<li>" + value + "</li>";
                    }
                }
            }
            element += "</" + e + ">";
            return element;
        }
    }

    [Obsolete("Use HttpHelper Build Query for page/link construction")]
    public class P
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public Q Q { get; set; }

        public P(string name) : this("", name)
        {
        }

        public P(string path, string name)
        {
            this.Path = path;
            this.Name = name;
            this.Q = new Q();
        }

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
            s.Append(Path);
            s.Append(Name);
            s.Append("?");
            s.Append(Q.ToString());
            return s.ToString();
        }

        public void Add(Q q)
        {
            foreach (var k in q.Keys) {
                Q.Add(k, q[k]);
            }
        }
    }

    [Serializable()]
    public class Q : Dictionary<string, object>
    {
        public Q()
        {
        }

        protected Q(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public void AddIf(bool condition, string key, object val)
        {
            if (condition) {
                Add(key, val);
            }
        }

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
            int i = 1;
            foreach (var k in Keys) {
                s.Append(string.Format("{0}={1}", k, this[k]));
                if (i++ < this.Count) {
                    s.Append("&");
                }
            }
            return s.ToString();
        }
    }
}
