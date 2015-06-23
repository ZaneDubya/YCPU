using System.Collections.Generic;
using System.IO;
using System.Linq;

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

        public static List<string> SplitString(string s, params char[] separators)
        {
            List<string> values = new List<string>();

            int lastPosition = 0;

            for (int i = 0; i < s.Length; i++)
            {
                if (separators.Contains(s[i]))
                {
                    string ss = s.Substring(lastPosition, i - lastPosition);
                    if (ss != string.Empty)
                        values.Add(s.Substring(lastPosition, i - lastPosition));

                    lastPosition = i + 1;
                }
            }

            if (s.Length != lastPosition)
                values.Add(s.Substring(lastPosition, s.Length - lastPosition));

            return values;
        }
    }
}
