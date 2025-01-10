using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace mame
{
    public partial class Namcos1
    {
        public static void GDIInit()
        {

        }
        public static Bitmap GetLayer(int n)
        {
            int i, j, i1;
            uint u1;
            Color c1;
            Bitmap bm1 = new Bitmap(512, 512);
            for (i = 0; i < Namcos1.ttmap[n].width; i++)
            {
                for (j = 0; j < Namcos1.ttmap[n].height; j++)
                {
                    i1 = Namcos1.ttmap[n].pixmap[i + j * Namcos1.ttmap[n].width] + Namcos1.ttmap[n].palette_offset;
                    u1 = Palette.entry_color[i1];
                    c1 = Color.FromArgb((int)Palette.entry_color[Namcos1.ttmap[n].pixmap[i + j * Namcos1.ttmap[n].width] + Namcos1.ttmap[n].palette_offset]);
                    bm1.SetPixel(i, j, c1);
                }
            }
            return bm1;
        }
        public static Bitmap GetSprite()
        {
            Bitmap bm1 = new Bitmap(512, 512);
            int source_offset;
            int sprite_xoffs = namcos1_spriteram[0x800 + 0x07f5] + ((namcos1_spriteram[0x800 + 0x07f4] & 1) << 8);
            int sprite_yoffs = namcos1_spriteram[0x800 + 0x07f7];
            for (source_offset = 0xfe0; source_offset >= 0x800; source_offset -= 0x10)
            {
                int[] sprite_size = new int[] { 16, 8, 32, 4 };
                int attr1 = namcos1_spriteram[source_offset + 10];
                int attr2 = namcos1_spriteram[source_offset + 14];
                int color = namcos1_spriteram[source_offset + 12];
                int flipx = (attr1 & 0x20) >> 5;
                int flipy = (attr2 & 0x01);
                int sizex = sprite_size[(attr1 & 0xc0) >> 6];
                int sizey = sprite_size[(attr2 & 0x06) >> 1];
                int tx = (attr1 & 0x18) & (~(sizex - 1));
                int ty = (attr2 & 0x18) & (~(sizey - 1));
                int sx = namcos1_spriteram[source_offset + 13] + ((color & 0x01) << 8);
                int sy = -namcos1_spriteram[source_offset + 15] - sizey;
                int sprite = namcos1_spriteram[source_offset + 11];
                int sprite_bank = attr1 & 7;
                int priority = (namcos1_spriteram[source_offset + 14] & 0xe0) >> 5;
                int pri_mask = (0xff << (priority + 1)) & 0xff;
                sprite += sprite_bank * 256;
                color = color >> 1;
                sx += sprite_xoffs;
                sy -= sprite_yoffs;
                if (Video.flip_screen_get())
                {
                    sx = -sx - sizex;
                    sy = -sy - sizey;
                    flipx ^= 1;
                    flipy ^= 1;
                }
                sy++;
                //Drawgfx.common_drawgfx_namcos1(sizex, sizey, tx, ty, sprite, color, flipx, flipy, sx & 0x1ff, ((sy + 16) & 0xff) - 16, pri_mask, cliprect);
            }
            return bm1;
        }
        public static Bitmap GetPri()
        {
            int i, j;
            Color c1;
            Bitmap bm1 = new Bitmap(512, 512);
            for (i = 0; i < 0x200; i++)
            {
                for (j = 0; j < 0x200; j++)
                {
                    if (Tilemap.priority_bitmap[i, j] == 0)
                    {
                        c1 = Color.Black;
                    }
                    else if (Tilemap.priority_bitmap[i, j] == 0x01)
                    {
                        c1 = Color.Red;
                    }
                    else if (Tilemap.priority_bitmap[i, j] == 0x03)
                    {
                        c1 = Color.Blue;
                    }
                    else if (Tilemap.priority_bitmap[i, j] == 0x04)
                    {
                        c1 = Color.Yellow;
                    }
                    else if (Tilemap.priority_bitmap[i, j] == 0x07)
                    {
                        c1 = Color.Magenta;
                    }
                    else if (Tilemap.priority_bitmap[i, j] == 0x08)
                    {
                        c1 = Color.AliceBlue;
                    }
                    else if (Tilemap.priority_bitmap[i, j] == 0x1f)
                    {
                        c1 = Color.Orange;
                    }
                    else if (Tilemap.priority_bitmap[i, j] == 0x80)
                    {
                        c1 = Color.LimeGreen;
                    }
                    else if (Tilemap.priority_bitmap[i, j] == 0x9f)
                    {
                        c1 = Color.Green;
                    }
                    else
                    {
                        c1 = Color.White;
                    }
                    bm1.SetPixel(j, i, c1);
                }
            }
            return bm1;
        }
    }
}
