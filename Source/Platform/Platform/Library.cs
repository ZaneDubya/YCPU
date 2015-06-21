using Microsoft.Xna.Framework;
using System;
using System.Net;
using System.Text;

namespace Ypsilon.Platform
{
    internal static class Library
    {
        public static Settings Settings
        {
            get;
            set;
        }

        private static Random m_Random;
        public static int Random(int a, int b)
        {
            if (m_Random == null)
                m_Random = new Random(0);
            return m_Random.Next(a, b + 1);
        }

        internal static Matrix ProjectionMatrixScreen
        {
            get
            {
                return Matrix.CreateOrthographicOffCenter(0f,
                    Settings.Resolution.X,
                    Settings.Resolution.Y,
                    0f, -20000f, 20000f);
            }
        }

        private static Encoding m_UTF8, m_UTF8WithEncoding;

        internal static Encoding UTF8
        {
            get
            {
                if (m_UTF8 == null)
                    m_UTF8 = new UTF8Encoding(false, false);

                return m_UTF8;
            }
        }

        internal static Encoding UTF8WithEncoding
        {
            get
            {
                if (m_UTF8WithEncoding == null)
                    m_UTF8WithEncoding = new UTF8Encoding(true, false);

                return m_UTF8WithEncoding;
            }
        }

        public static long GetLongAddressValue(IPAddress address)
        {
#pragma warning disable 618
            return address.Address;
#pragma warning restore 618
        }

        public static int Clamp(int value, int low, int high)
        {
            if (value < low)
                value = low;
            if (value > high)
                value = high;
            return value;
        }

        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }
    }
}
