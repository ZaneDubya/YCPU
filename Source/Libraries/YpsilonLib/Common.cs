using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Ypsilon
{
    public static class Common
    {
        public static byte[] GetBytesFromFile(string path)
        {
            try
            {
                byte[] data = File.ReadAllBytes(path);
                return data;
            }
            catch
            {
                return null;
            }
        }
    }
}
