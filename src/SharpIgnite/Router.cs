using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace SharpIgnite
{
    public class Router
    {
        Dictionary<string, Route> routes = new Dictionary<string, Route>();

        public Route this[string name] {
            get { return routes[name]; }
        }

        public Router Add(string key, string value)
        {
            routes[key] = new Route(value);
            return this;
        }

        public bool ContainsKey(string key)
        {
            return routes.ContainsKey(key);
        }
    }

    public class Route
    {
        public Route()
        {
        }

        public Route(Uri uri)
        {
            this.pageName = GetPageName(uri.ToString());
            var absolutePath = uri.AbsolutePath.Trim('/').Replace(pageName, "");
            Initialize(absolutePath);
        }

        public Route(string absolutePath)
        {
            this.pageName = "Index.aspx"; // Defaults to Index.aspx
            Initialize(absolutePath);
        }

        void Initialize(string absolutePath)
        {
            var paths = absolutePath.Trim('/').Split('/');
            if (paths.Length == 2) {
                var c = paths[0];
                var m = paths[1];
                Controller = c.Capitalize() + "Controller";
                Action = m.Capitalize();
                Name = (c + "/" + m).Trim('/');
            } else if (paths.Length == 1) {
                var c = paths[0];
                Controller = c.Capitalize() + "Controller";
                Action = "Index";
                if (c != "") {
                    Name = (c).Trim('/');
                } else {
                    Name = "/";
                }
            } else {
                // Get from default route
            }
        }

        string pageName;

        public string PageName {
            get { return pageName; }
        }

        string GetPageName(string url)
        {
            var extension = ".aspx";
            int firstIndexOfExtension = url.IndexOf(extension);
            var urlWithNoExtension = url.Substring(0, firstIndexOfExtension);
            var firstIndexOfPageName = urlWithNoExtension.LastIndexOf("/");

            var pageNameLength = (firstIndexOfExtension + extension.Length) - firstIndexOfPageName;
            var pageName = url.Substring(firstIndexOfPageName, pageNameLength);

            return pageName.Trim('/');
        }

        public string Controller { get; set; }
        public string Action { get; set; }
        public string Name { get; set; }

        public void Execute(System.Web.UI.Page entryPage, Assembly assembly)
        {
            var assemblyName = assembly.GetName().Name;
            var fullClassName = assemblyName + ".Controllers." + Controller;
            Type type = assembly.GetType(fullClassName);
            object instance = null;
            if (type != null) {
                instance = Activator.CreateInstance(type);

                PropertyInfo pageProperty = type.GetProperty("Page");
                if (pageProperty != null && pageProperty.CanWrite) {
                    pageProperty.SetValue(instance, entryPage);
                } else {
                    // Handle if the type doesn't have a Data property or if data is null
                }

                MethodInfo methodInfo = type.GetMethod(Action);
                if (methodInfo != null) {
                    if (typeof(IActionResult).IsAssignableFrom(methodInfo.ReturnType)) {
                        var result = methodInfo.Invoke(instance, null) as IActionResult;
                        result.Render(entryPage);
                    } else if (methodInfo.ReturnType == typeof(string)
                        || methodInfo.ReturnType == typeof(int)
                        || methodInfo.ReturnType == typeof(bool)
                        || methodInfo.ReturnType == typeof(double)
                        || methodInfo.ReturnType == typeof(float)
                        || methodInfo.ReturnType == typeof(char)
                        || methodInfo.ReturnType == typeof(long)
                        || methodInfo.ReturnType == typeof(short)
                        || methodInfo.ReturnType == typeof(byte)
                        || methodInfo.ReturnType == typeof(decimal)) {
                        var result = methodInfo.Invoke(instance, null);
                        entryPage.Response.Write(result.ToString());
                    } else if (methodInfo.ReturnType == typeof(void)) {
                        methodInfo.Invoke(instance, null);
                    } else {
                        var result = methodInfo.Invoke(instance, null);
                        var json = JsonConvert.SerializeObject(result);
                        var contentType = "application/json";
                        entryPage.Response.ContentType = contentType;
                        entryPage.Response.Write(json);
                    } // TODO: Handle actions that has void return type
                } else {
                    WriteLine("Method '" + Action + "' not found in class '" + fullClassName + "'", entryPage);
                }
            } else {
                WriteLine("Class '" + fullClassName + "' not found", entryPage);
            }
        }

        void WriteLine(string text, System.Web.UI.Page page) // HACK: Might move this somewhere
        {
            page.Response.Write(text);
        }
    }
}
