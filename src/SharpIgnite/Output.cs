using System.Text;
using System.Web;

namespace SharpIgnite
{
    public class Output
    {
        internal bool profiler;
        string output;
        IHttpResponse response;

        public Output()
        {
            this.response = new MyHttpResponse();
        }

        public Output(IHttpResponse response)
        {
            this.response = response;
        }

        public void EnableProfiler(bool profiler)
        {
            this.profiler = profiler;
        }

        public void AppendOutput(string output)
        {
            this.output += output;
        }

        public string GetOutput()
        {
            return this.output;
        }

        public void SetOutput(string output)
        {
            this.output = output;
        }

        public Output SetCharset(string charset)
        {
            response.SetCharset(charset);
            return this;
        }

        public Output SetContentType(string contentType)
        {
            response.SetContentType(contentType);
            return this;
        }

        public Output SetContentEncoding(Encoding encoding)
        {
            response.SetContentEncoding(encoding);
            return this;
        }

        public Output AddHeader(string name, string value)
        {
            response.AddHeader(name, value);
            return this;
        }

        public Output Write(byte[] buffer, int offset, int count)
        {
            response.Write(buffer, offset, count);
            return this;
        }

        public Output Write(string text)
        {
            response.Write(text);
            return this;
        }

        public Output Clear()
        {
            response.Clear();
            return this;
        }

        public Output ClearHeaders()
        {
            response.ClearHeaders();
            return this;
        }

        public Output Flush()
        {
            response.Flush();
            return this;
        }

        public Output End()
        {
            response.End();
            return this;
        }
    }

    public interface IHttpResponse
    {
        IHttpResponse SetContentType(string contentType);
        IHttpResponse AddHeader(string name, string value);
        IHttpResponse Write(byte[] buffer, int offset, int count);
        IHttpResponse Write(string text);
        IHttpResponse Clear();
        IHttpResponse ClearHeaders();
        IHttpResponse Flush();
        IHttpResponse End();
        IHttpResponse SetCharset(string charset);
        IHttpResponse SetContentEncoding(Encoding encoding);
    }

    public class MyHttpResponse : IHttpResponse
    {
        public MyHttpResponse()
        {
        }

        public IHttpResponse SetContentType(string contentType)
        {
            HttpContext.Current.Response.ContentType = contentType;
            return this;
        }

        public IHttpResponse AddHeader(string name, string value)
        {
            HttpContext.Current.Response.AddHeader(name, value);
            return this;
        }

        public IHttpResponse Write(byte[] buffer, int offset, int count)
        {
            HttpContext.Current.Response.OutputStream.Write(buffer, offset, count);
            return this;
        }

        public IHttpResponse Write(string text)
        {
            HttpContext.Current.Response.Write(text);
            return this;
        }

        public IHttpResponse Clear()
        {
            HttpContext.Current.Response.Clear();
            return this;
        }

        public IHttpResponse ClearHeaders()
        {
            HttpContext.Current.Response.ClearHeaders();
            return this;
        }

        public IHttpResponse Flush()
        {
            HttpContext.Current.Response.Flush();
            return this;
        }

        public IHttpResponse End()
        {
            HttpContext.Current.Response.End();
            return this;
        }

        public IHttpResponse SetCharset(string charset)
        {
            HttpContext.Current.Response.Charset = charset;
            return this;
        }

        public IHttpResponse SetContentEncoding(Encoding encoding)
        {
            HttpContext.Current.Response.ContentEncoding = encoding;
            return this;
        }
    }
}
