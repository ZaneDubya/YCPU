using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

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

            int currentPosition = 0;
            int lastPosition = 0;

            do
            {

                do
                {
                    if (separators.Contains(s[currentPosition]))
                        break;
                    ;
                } while (++currentPosition < s.Length);

                values.Add(s.Substring(lastPosition, currentPosition - lastPosition));

                lastPosition = ++currentPosition;

            } while (currentPosition < s.Length);

            return values;
        }
    }
}
