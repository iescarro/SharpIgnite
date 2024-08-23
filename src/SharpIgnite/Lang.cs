using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using System.Text;
using System.Web;
using System.Resources;
using System.IO;

namespace SharpIgnite
{
    public class Lang
    {
        ResXResourceReader resource;
    
        public void Load(string filename)
        {
            var propertiesDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Properties");
            resource = new ResXResourceReader(Path.Combine(propertiesDir, filename));
        }
    
        public string Line(string key)
        {
            return Line(key, "");
        }
    
        public string Line(string key, string defaultValue)
        {
            IDictionaryEnumerator d = resource.GetEnumerator();
            while (d.MoveNext()) {
                if (key == (string)d.Key) {
                    return d.Value as string;
                }
            }
            return defaultValue;
        }
    }
}
