using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Ypsilon.Platform
{
    public static class Common
    {
        public static byte[] GetBytesFromFile(string path)
        {
            try
            {
                byte[] data = File.ReadAllBytes(path);
                return data;
                /*bool extra = false;
                if (data.Length % 2 == 1)
                    extra = true;

                ushort[] sdata = new ushort[data.Length / 2 + (extra ? 1 : 0)];
                Buffer.BlockCopy(data, 0, sdata, 0, data.Length);
                return sdata;*/
            }
            catch
            {
                return null;
            }
        }
    }
}
