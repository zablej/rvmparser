using System;
using System.Collections.Generic;
using System.IO;
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

        public static void fprintf(StreamWriter stream,string message, params object[] args)
        {
            stream.Write(string.Format(message, args));
            stream.Flush();
        }

        public static void fclose(StreamWriter stream)
        {
            stream.Close();
        }

        public static StreamWriter fopen(string path, string fileAccessMode)
        // r - read
        // w - write
        // a - append
        // r+ - read update
        // w+ - write update
        // a+ - append update
        {
            switch (fileAccessMode)
            {
                case "w":
                    return new StreamWriter(File.OpenWrite(path));
                default:
                    throw new NotImplementedException($"Opening file with access mode '{fileAccessMode}' not implemented.");
            }
        }
    }
}
