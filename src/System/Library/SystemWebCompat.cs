using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.Concurrent;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace System.Web
{
    public enum HttpCacheability
    {
        Public
    }

    public class HttpApplicationState
    {
        private readonly ConcurrentDictionary<string, object> _values = new ConcurrentDictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        public object this[string name]
        {
            get
            {
                object value;
                return _values.TryGetValue(name, out value) ? value : null;
            }
            set
            {
                _values[name] = value;
            }
        }
    }

    public class HttpSessionState
    {
        private readonly ConcurrentDictionary<string, object> _values = new ConcurrentDictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        public object this[string name]
        {
            get
            {
                object value;
                return _values.TryGetValue(name, out value) ? value : null;
            }
            set
            {
                _values[name] = value;
            }
        }

        public int LCID { get; set; } = CultureInfo.CurrentCulture.LCID;

        public void Clear()
        {
            _values.Clear();
        }
    }

    public class HttpCookie
    {
        public HttpCookie(string name)
            : this(name, string.Empty)
        {
        }

        public HttpCookie(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }
        public string Value { get; set; }
        public DateTime Expires { get; set; }
    }

    public class HttpCookieCollection
    {
        private readonly Dictionary<string, HttpCookie> _cookies = new Dictionary<string, HttpCookie>(StringComparer.OrdinalIgnoreCase);

        public HttpCookie this[string name]
        {
            get
            {
                HttpCookie cookie;
                return _cookies.TryGetValue(name, out cookie) ? cookie : null;
            }
            set
            {
                _cookies[name] = value;
            }
        }

        public NameValueCollection Keys
        {
            get
            {
                NameValueCollection keys = new NameValueCollection();
                foreach (string key in _cookies.Keys)
                {
                    keys.Add(key, key);
                }

                return keys;
            }
        }

        public void Add(HttpCookie cookie)
        {
            if (cookie != null)
            {
                _cookies[cookie.Name] = cookie;
            }
        }
    }

    public class HttpPostedFile
    {
        public string FileName { get; set; } = string.Empty;
        public int ContentLength { get; set; }
        public byte[] Content { get; set; } = Array.Empty<byte>();

        public void SaveAs(string filename)
        {
            File.WriteAllBytes(filename, Content);
        }
    }

    public class HttpFileCollection
    {
        private readonly List<HttpPostedFile> _files = new List<HttpPostedFile>();

        public int Count => _files.Count;

        public HttpPostedFile this[int index] => _files[index];

        public void Add(HttpPostedFile postedFile)
        {
            _files.Add(postedFile);
        }
    }

    public class HttpRequest
    {
        public HttpRequest()
        {
            ApplicationPath = "/";
            ServerVariables["SERVER_PROTOCOL"] = "HTTP/1.1";
            ServerVariables["SERVER_NAME"] = "localhost";
            ServerVariables["SERVER_PORT"] = "80";
            ServerVariables["HTTP_REFERER"] = string.Empty;
            ServerVariables["HTTP_USER_AGENT"] = string.Empty;
            ServerVariables["REMOTE_ADDR"] = "127.0.0.1";
        }

        public string ApplicationPath { get; set; }
        public NameValueCollection QueryString { get; } = new NameValueCollection();
        public NameValueCollection Form { get; } = new NameValueCollection();
        public NameValueCollection ServerVariables { get; } = new NameValueCollection();
        public NameValueCollection Params { get; } = new NameValueCollection();
        public HttpCookieCollection Cookies { get; } = new HttpCookieCollection();
        public HttpFileCollection Files { get; } = new HttpFileCollection();
        public Uri Url { get; set; } = new Uri("http://localhost/");
        public Uri UrlReferrer { get; set; }

        public string this[string key]
        {
            get
            {
                return Form[key] ?? QueryString[key] ?? Params[key];
            }
        }
    }

    public class HttpCachePolicy
    {
        public void SetExpires(DateTime date)
        {
        }

        public void SetCacheability(HttpCacheability cacheability)
        {
        }
    }

    public class HttpResponse
    {
        private readonly Dictionary<string, string> _headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private readonly StringBuilder _body = new StringBuilder();

        public HttpCookieCollection Cookies { get; } = new HttpCookieCollection();
        public HttpCachePolicy Cache { get; } = new HttpCachePolicy();
        public string RedirectLocation { get; private set; }
        public string Body => _body.ToString();
        public IReadOnlyDictionary<string, string> Headers => _headers;

        public void AddHeader(string name, string value)
        {
            if (!string.IsNullOrEmpty(name))
            {
                _headers[name] = value;
            }
        }

        public void Write(string value)
        {
            if (value != null)
            {
                _body.Append(value);
            }
        }

        public void WriteFile(string filename)
        {
            if (File.Exists(filename))
            {
                _body.Append(File.ReadAllText(filename));
            }
        }

        public void Clear()
        {
            _body.Clear();
        }

        public void Flush()
        {
        }

        public void End()
        {
        }

        public void Redirect(string url)
        {
            RedirectLocation = url;
        }
    }

    public class HttpServerUtility
    {
        public string MapPath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return Directory.GetCurrentDirectory();
            }

            string normalizedPath = path.Replace("~/", string.Empty).TrimStart('/', '\\');
            return Path.GetFullPath(normalizedPath);
        }

        public string UrlEncode(string value)
        {
            if (value == null)
            {
                return null;
            }

            return WebUtility.UrlEncode(value);
        }

        public string HtmlDecode(string value)
        {
            if (value == null)
            {
                return null;
            }

            return WebUtility.HtmlDecode(value);
        }
    }

    public class HttpContext
    {
        private static readonly AsyncLocal<HttpContext> CurrentContext = new AsyncLocal<HttpContext>();

        public static HttpContext Current
        {
            get
            {
                return CurrentContext.Value ?? (CurrentContext.Value = new HttpContext());
            }
            set
            {
                CurrentContext.Value = value;
            }
        }

        public HttpContext()
            : this(new HttpRequest(), new HttpResponse(), new HttpServerUtility())
        {
        }

        public HttpContext(HttpRequest request, HttpResponse response, HttpServerUtility server)
        {
            Request = request;
            Response = response;
            Server = server;
        }

        public HttpRequest Request { get; }
        public HttpResponse Response { get; }
        public HttpServerUtility Server { get; }
    }

}

namespace System.Web.UI
{
    public class Page
    {
        public Page()
        {
            HttpContext.Current = new HttpContext(Request, Response, Server);
        }

        public System.Web.HttpRequest Request { get; } = new System.Web.HttpRequest();
        public System.Web.HttpResponse Response { get; } = new System.Web.HttpResponse();
        public System.Web.HttpSessionState Session { get; } = new System.Web.HttpSessionState();
        public System.Web.HttpApplicationState Application { get; } = new System.Web.HttpApplicationState();
        public System.Web.HttpServerUtility Server { get; } = new System.Web.HttpServerUtility();
    }
}
