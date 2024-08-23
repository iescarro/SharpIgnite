using System;

namespace SharpIgnite
{
    public static class NumberHelper
    {
        public static int Random()
        {
            return (new Random(unchecked((int)DateTime.Now.Ticks))).Next();
        }
        
        public static int Ceiling(this int i)
        {
            int d = 10;
            if (i > 100000) {
                d = 100000;
            } else if (i > 10000) {
                d = 10000;
            } else if (i > 100) {
                d = 100;
            } else {
                d = 10;
            }
            return ((int)Math.Ceiling(i / (double)d)) * d;
        }
    }
}
