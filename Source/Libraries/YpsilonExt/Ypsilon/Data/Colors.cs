using Microsoft.Xna.Framework;

namespace Ypsilon.Data
{
    static class Colors
    {
        /// <summary>
        /// Chris Kempson - Base16 Railscasts
        /// </summary>
        public static Color[] Railscasts = new Color[16] {
            new Color( 0x2b, 0x2b, 0x2b),
            new Color( 0x27, 0x29, 0x35),
            new Color( 0x3a, 0x40, 0x55),
            new Color( 0x5a, 0x64, 0x7e),
            new Color( 0xd4, 0xcf, 0xc9),
            new Color( 0xe6, 0xe1, 0xdc),
            new Color( 0xf4, 0xf1, 0xed),
            new Color( 0xf9, 0xf7, 0xf3),
            new Color( 0xda, 0x49, 0x39),
            new Color( 0xcc, 0x78, 0x33),
            new Color( 0xff, 0xc6, 0x6d),
            new Color( 0xa5, 0xc2, 0x61),
            new Color( 0x51, 0x9f, 0x50),
            new Color( 0x6d, 0x9c, 0xbe),
            new Color( 0xb6, 0xb3, 0xeb),
            new Color( 0xbc, 0x94, 0x58)
        };

        public static Vector4[] CreateV4CLUT(Color[] value, int count)
        {
            Vector4[] clut = new Vector4[count];
            for (int i = 0; i < value.Length; i++)
            {
                clut[i] = value[i].ToVector4();
            }
            return clut;
        }
    }
}
