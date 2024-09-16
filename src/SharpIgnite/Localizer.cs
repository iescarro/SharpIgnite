using System;
using System.Collections;
using System.IO;
using System.Resources;

namespace SharpIgnite
{
    public class Localizer
    {
        ResXResourceReader resource;

        public void Set(string filename)
        {
            var propertiesDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Properties");
            resource = new ResXResourceReader(Path.Combine(propertiesDir, filename));
        }

        string Line(string key)
        {
            return Line(key, null);
        }

        string Line(string key, string defaultValue)
        {
            try {
                IDictionaryEnumerator d = resource.GetEnumerator();
                while (d.MoveNext()) {
                    if (key == (string)d.Key) {
                        return d.Value as string;
                    }
                }
                return defaultValue;
            } catch {
                return defaultValue;
            }
        }

        public string this[string key] {
            get {
                return Line(key);
            }
        }
    }
}
