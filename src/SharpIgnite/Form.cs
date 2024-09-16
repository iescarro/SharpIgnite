using System.Web;
using System.Web.UI.WebControls;

namespace SharpIgnite
{
    public static class Form
    {
        public static string Open(string action)
        {
            return Open(action, "POST");
        }

        public static string Open(string action, string method)
        {
            var obj = Application.Instance;
            var actionUrl = obj.BaseUrl(action);
            return $"<form action='{actionUrl}' method='{method}'>";
        }

        public static string Hidden(string name, string value)
        {
            return Hidden(name, value, "");
        }

        public static string Hidden(string name, string value, string extra)
        {
            return $"<input type='hidden' name='{name}' value='{value}' {extra}>";
        }

        public static string Input(string name)
        {
            return Input(name, "");
        }

        public static string Input(string name, string value)
        {
            return Input(name, value, "");
        }

        public static string Input(string name, string value, string extra)
        {
            return $"<input type='text' name='{name}' value='{value}' {extra}/>";
        }

        public static string Password(string name)
        {
            return Password(name, "");
        }

        public static string Password(string name, string value)
        {
            return Password(name, value, "");
        }

        public static string Password(string name, string value, string extra)
        {
            return $"<input type='password' name='{name}' value='{value}' {extra}/>";
        }

        public static string Label(string text)
        {
            return Label(text, "");
        }

        public static string Label(string label, string _for)
        {
            return $"<label for='{_for}'>{label}</label>";
        }

        public static string CheckBox(string name, object value)
        {
            return CheckBox(name, value, false);
        }

        public static string CheckBox(string name, object value, bool isChecked)
        {
            return CheckBox(name, value, isChecked, "");
        }

        public static string CheckBox(string name, object value, bool isChecked, string extra)
        {
            var c = isChecked ? "checked" : "";
            return $"<input type='checkbox' name='{name}' value='{value}' {c} {extra}/>";
        }

        public static string Submit(string name)
        {
            return Submit(name, "");
        }

        public static string Submit(string name, string value)
        {
            return Submit(name, value, "");
        }

        public static string Submit(string name, string value, string extra)
        {
            return $"<input type='submit' value='{value}' name='{name}' {extra}/>";
        }

        public static string DropDown(string name, Array options)
        {
            return DropDown(name, options, "");
        }

        static string Encode(string str)
        {
            return HttpUtility.HtmlEncode(str);
        }

        public static string DropDown(string name, Array options, string selected)
        {
            return DropDown(name, options, selected, "");
        }

        public static string DropDown(string name, Array options, string selected, string extra)
        {
            string form = "<select name='" + name + "' " + extra + ">";
            if (options != null) {
                foreach (var key in options.Keys) {
                    var value = options[key];
                    var s = selected.Equals(key) ? "selected" : "";
                    form += "<option value='" + key + "' " + s + ">" + value + "</option>";
                }
            }
            form += "</select>";
            return form;
        }

        public static string Radio(string name, string value, bool _checked, string extra)
        {
            var c = _checked ? "checked" : "";
            return $"<input type='radio' name='{name}' value='{value}' {c} {extra}/>";
        }

        public static string Email(string name)
        {
            return Email(name, "");
        }

        public static string Email(string name, string value)
        {
            return Email(name, value, "");
        }

        public static string Email(string name, string value, string extra)
        {
            return $"<input type='email' name='{name}' value='{value}' {extra}/>";
        }

        public static string TextArea(string name)
        {
            return TextArea(name, "");
        }

        public static string TextArea(string name, string value)
        {
            return TextArea(name, value, "");
        }

        public static string TextArea(string name, string value, string extra)
        {
            return $"<textarea name='{name}' {extra}/>{value}</textarea>";
        }

        public static string Close()
        {
            return "</form>";
        }

        // Below are extension methods for ASP.NET controls

        public static void SetTextIfEmpty(this TextBox textBox, string text)
        {
            if (textBox.Text == "") {
                textBox.Text = text;
            }
        }

        public static DropDownList ClearItems(this DropDownList dropDownList)
        {
            dropDownList.Items.Clear();
            return dropDownList;
        }

        public static DropDownList AddItem(this DropDownList dropDownList, string text, string value)
        {
            return AddItem(dropDownList, new ListItem(text, value));
        }

        public static DropDownList AddItem(this DropDownList dropDownList, ListItem item)
        {
            dropDownList.Items.Add(item);
            return dropDownList;
        }
    }
}
