using System;
using System.Collections.Generic;

namespace SharpIgnite
{
    public class Array
    {
        Dictionary<object, object> data = new Dictionary<object, object>();

        public IEnumerable<object> Keys {
            get { return data.Keys; }
        }

        public object this[object key] {
            get {
                if (data.ContainsKey(key)) {
                    return data[key];
                }
                return null;
            }
            set {
                data[key] = value;
            }
        }

        public Array()
        {
        }

        public Array(string key, object value)
        {
            Add(key, value);
        }

        public int Count {
            get {
                return data.Count;
            }
        }

        public Array AddRange(Array array)
        {
            foreach (var key in array.Keys) {
                var value = array[key];
                data.Add(key, value);
            }
            return this;
        }

        public static Array New()
        {
            return new Array();
        }

        public static Array New(string key, object value)
        {
            return new Array(key, value);
        }

        public static Array Range(int from, int to)
        {
            var array = new Array();
            for (int i = from; i <= to; i++) {
                array.Add(i, i);
            }
            return array;
        }

        public Array Add(object key, object value)
        {
            data.Add(key, value);
            return this;
        }

        public Array AddIf(bool condition, object key, object value)
        {
            if (condition) {
                data.Add(key, value);
            }
            return this;
        }

        public override string ToString()
        {
            string str = "";
            int i = 0;
            foreach (var key in this.Keys) {
                if (i++ > 0) {
                    str += ", ";
                }
                var value = this[key];
                if (value is string || value is DateTime) {
                    str += key.ToString() + " = '" + value.ToString() + "'";
                } else {
                    str += key.ToString() + " = " + value.ToString();
                }
            }
            return str;
        }
    }
}
