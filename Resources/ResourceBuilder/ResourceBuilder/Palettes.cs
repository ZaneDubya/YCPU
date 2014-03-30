using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace ResourceBuilder
{
    class Palettes
    {
        public static void Palette_ToRGB444(string path_in, string path_out, string[] param = null)
        {
            string[] lines = System.IO.File.ReadAllLines(path_in);
            byte[][] palette = new byte[0x10][];
            int current = 0;

            for (int i = 0; i < lines.Length; i++)
            {
                if ((lines[i] == string.Empty) || (lines[i].Trim()[0] == ';'))
                {
                    // ignore this line - empty or comment
                }
                else
                {
                    string line = lines[i].Trim();
                    string[] rgb = line.Split(',');
                    if (rgb.Length != 3)
                        continue; // should only have three elements.
                    byte[] pal_entry = new byte[4];
                    pal_entry[0] = 0xFF;
                    pal_entry[1] = byte.Parse(rgb[0].Trim());
                    pal_entry[2] = byte.Parse(rgb[1].Trim());
                    pal_entry[3] = byte.Parse(rgb[2].Trim());
                    palette[current++] = pal_entry;
                }
                if (current == 16)
                    break;
            }

            ushort[] pal_16bit = new ushort[0x10];
            for (int i = 0; i < 0x10; i++)
            {
                int r = palette[i][1] >> 4;
                int g = palette[i][2] >> 4;
                int b = palette[i][3] >> 4;
                pal_16bit[i] = (ushort)((r << 8) | (g << 4) | b);
            }

            System.IO.BinaryWriter writer = new System.IO.BinaryWriter(System.IO.File.Open(path_out, System.IO.FileMode.Create));
            for (int i = 0; i < 0x10; i++)
                writer.Write(pal_16bit[i]);
            writer.Close();
        }
    }
}
