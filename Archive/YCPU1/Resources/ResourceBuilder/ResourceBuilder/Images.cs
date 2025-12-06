using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace ResourceBuilder
{
    class Images
    {
        public static void PNGtoBIN(string path_in, string path_out, string[] param = null)
        {
            Image png = Image.FromFile(path_in);
            Bitmap bmp = new Bitmap(png);
            if ((png.Width != 128) && (png.Height != 32))
                return;

            int tiles_w = png.Width / 4;
            int tiles_h = png.Height / 8;
            ushort[] binary = new ushort[256];
            for (int y = 0; y < tiles_h; y++)
            {
                for (int x = 0; x < tiles_w; x++)
                {
                    ushort[] tile = new ushort[2];
                    for (int iY = 0; iY < 8; iY++)
                    {
                        for (int iX = 0; iX < 4; iX++)
                        {
                            Color color = bmp.GetPixel(x * 4 + iX, y * 8 + iY);
                            if (color.R + color.G + color.B > 0x0100)
                                tile[(iY / 4)] |= (ushort)(1 << ((iY % 4) * 4 + iX));
                        }
                    }
                    binary[y * tiles_w * 2 + x * 2 + 0] = tile[0];
                    binary[y * tiles_w * 2 + x * 2 + 1] = tile[1];
                }
            }

            System.IO.BinaryWriter writer = new System.IO.BinaryWriter(System.IO.File.Open(path_out, System.IO.FileMode.Create));
            for (int i = 0; i < 256; i++)
                writer.Write(binary[i]);
            writer.Close();
        }
    }
}
