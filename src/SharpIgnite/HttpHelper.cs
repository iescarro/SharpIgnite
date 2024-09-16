using System.Collections.Specialized;
using System.Web;

namespace SharpIgnite
{
    public static class HttpHelper
    {
        public static string BuildQuery(Array array)
        {
            string query = "";
            int i = 0;
            foreach (var key in array.Keys) {
                if (i++ > 0) {
                    query += "&";
                }
                var value = array[key] ?? "";
                query += HttpUtility.UrlEncode(key.ToString()) + "=" + HttpUtility.UrlEncode(value.ToString());
            }
            return query;
        }

        public static string BuildQuery(NameValueCollection array)
        {
            string query = "";
            int i = 0;
            foreach (var key in array.AllKeys) {
                if (i++ > 0) {
                    query += "&";
                }
                var value = array[key];
                query += HttpUtility.UrlEncode(key.ToString()) + "=" + HttpUtility.UrlEncode(value.ToString());
            }
            return query;
        }
    }
}
