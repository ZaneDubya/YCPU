using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Ypsilon
{
    public static partial class Utility
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

        public static List<string> SplitString(string s, params string[] separators)
        {
            List<string> values = new List<string>();

            int lastPosition = 0;

            for (int i = 0; i < s.Length; i++)
            {
                for (int j = 0; j < separators.Length; j++)
                {
                    if (s[i] == (int)separators[j][0])
                    {
                        string ss = s.Substring(lastPosition, i - lastPosition);
                        if (ss != string.Empty)
                            values.Add(s.Substring(lastPosition, i - lastPosition));

                        lastPosition = i + 1;
                        break;
                    }
                }
            }

            if (s.Length != lastPosition)
                values.Add(s.Substring(lastPosition, s.Length - lastPosition));

            return values;
        }
    }
}
