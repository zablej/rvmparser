using System;
using System.Collections.Generic;
using System.Text;

namespace Lib.Api
{
    public static class CommonHelper
    {
        public static void swap<T>(ref T x, ref T y)
        {
            var temp = x;
            x = y;
            y = temp;
        }
    }
}
