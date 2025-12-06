namespace Ypsilon.Extensions {
    public static class StringX {
        public static string Truncate(this string value, int maxLength) {
            if (string.IsNullOrEmpty(value))
                return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }
    }
}