using System.Text;

namespace Ypsilon.Core {
    public static class Strings {
        public static readonly char Delim = '|';
        public static string Name = "PlayerName";

        public static string Localize(string s) {
            StringBuilder sb = new StringBuilder(s);
            sb.Replace("%name%", Name);
            return sb.ToString();
        }
    }
}