using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace mame
{
    public partial class sega
    {
        public static void GDIInit()
        {
            
        }
        public static Bitmap GetLayer(int n)
        {
            int i, j, i1;
            uint u1;
            Color c1;
            Bitmap bm1 = new Bitmap(0x200, 0x200);
            for (i = 0; i < bg_tilemap[0].tilemaps[n].width; i++)
            {
                for (j = 0; j < bg_tilemap[0].tilemaps[n].height; j++)
                {
                    i1 = bg_tilemap[0].tilemaps[n].pixmap[i + j * bg_tilemap[0].tilemaps[n].width];
                    u1 = Palette.entry_color[i1];
                    c1 = Color.FromArgb((int)u1);
                    bm1.SetPixel(i, j, c1);
                }
            }
            return bm1;
        }
        public static Bitmap gettempbitmap()
        {
            int i,j,i1;
            uint u1;
            Color c1;
            Bitmap bm1 = new Bitmap(0x140, 0x106);
            for (i = 0; i < 0x140; i++)
            {
                for (j = 0; j < 0x106; j++)
                {
                    i1 = tempbitmap.uu1[j * 0x140 + i];
                    if (i1 != 0xffff)
                    {
                        u1 = Palette.entry_color[i1];
                        c1 = Color.FromArgb((int)u1);
                        bm1.SetPixel(i, j, c1);
                    }
                }
            }
            return bm1;
        }
    }
}
