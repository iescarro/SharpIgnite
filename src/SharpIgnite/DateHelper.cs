using System;

namespace SharpIgnite
{
    public class DateHelper
    {
        public DateHelper()
        {
        }

        public static Array Months()
        {
            return Months("MMM");
        }

        public static Array Months(string format)
        {
            var months = new Array();
            var now = DateTime.Now;
            for (int i = 1; i <= 12; i++) {
                var d = new DateTime(now.Year, i, 1);
                months.Add(i, d.ToString(format));
            }
            return months;
        }
    }
}
