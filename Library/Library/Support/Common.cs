using System;
using System.Net;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace YCPU.Library.Support
{
    static class Common
    {
        private static Settings m_Settings;
        private static GraphicsDevice m_Graphics;
        private static InputState m_Input;
        private static ContentManager m_Content;

        public static void Initialize(Settings settings, GraphicsDevice graphics, InputState input)
        {
            m_Settings = settings;
            m_Graphics = graphics;
            m_Input = input;
        }

        public static ContentManager Content
        {
            get
            {
                return m_Content;
            }
            set
            {
                m_Content = value;
            }
        }

        public static InputState Input
        {
            get { return m_Input; }
        }

        public static Texture2D CreateTexture(int w, int h)
        {
            return new Texture2D(m_Graphics, w, h);
        }

        private static Random m_Random;
        public static int Random(int a, int b)
        {
            if (m_Random == null)
                m_Random = new Random(0);
            return m_Random.Next(a, b + 1);
        }

        public static uint[] RandomTile()
        {
            uint[] data = new uint[64];
            for (int i = 0; i < 64; i++)
                data[i] = (uint)Random(0, 3);
            return data;
        }

        public static Matrix ProjectionMatrixScreen
        {
            get
            {
                return Matrix.CreateOrthographicOffCenter(0f,
                    m_Settings.Resolution.X,
                    m_Settings.Resolution.Y,
                    0f, -20000f, 20000f);
            }
        }

        private static Encoding m_UTF8, m_UTF8WithEncoding;

        public static Encoding UTF8
        {
            get
            {
                if (m_UTF8 == null)
                    m_UTF8 = new UTF8Encoding(false, false);

                return m_UTF8;
            }
        }

        public static Encoding UTF8WithEncoding
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
