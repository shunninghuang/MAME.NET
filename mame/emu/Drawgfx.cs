using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace mame
{
    public class bitmap_t
    {
        //void *alloc;
        public ushort[] uu1;
        public int rowpixels;
        public int width;
        public int height;
        //bitmap_format	format;
        //int bpp;
        //palette_t *palette;
    }
    public partial class Drawgfx
    {
        public static int[] gfx_drawmode_table = new int[256];
        public static int afterdrawmask;
        public static int[][] shadow_table = new int[4][];
        public static int imode;
        public static int spritecount;
        public static void copybitmap_core16(ushort[] uu1, bitmap_t dest, bitmap_t src, int flipx, int flipy, int sx, int sy, RECT clip, int transparency, int transparent_color)
        {
            int ox;
            int oy;
            int ex;
            int ey;
            int offsetx, offsety, xdir, ydir;
            ox = sx;
            oy = sy;
            ex = sx + src.width - 1;
            if (sx < 0)
            {
                sx = 0;
            }
            if (sx < clip.min_x)
            {
                sx = clip.min_x;
            }
            if (ex >= dest.width)
            {
                ex = dest.width - 1;
            }
            if (ex > clip.max_x)
            {
                ex = clip.max_x;
            }
            if (sx > ex)
            {
                return;
            }
            ey = sy + src.height - 1;
            if (sy < 0)
            {
                sy = 0;
            }
            if (sy < clip.min_y)
            {
                sy = clip.min_y;
            }
            if (ey >= dest.height)
            {
                ey = dest.height - 1;
            }
            if (ey > clip.max_y)
            {
                ey = clip.max_y;
            }
            if (sy > ey)
            {
                return;
            }
            //UINT16 *sd = (UINT16 *)src.base;
            int sw = ex - sx + 1;
            int sh = ey - sy + 1;
            int sm = src.rowpixels;
            //UINT16 *dd1 = BITMAP_ADDR(dest, UINT16, 0, 0);
            int dm = dest.rowpixels;
            if (flipx!=0)
            {
                offsetx = src.width - 1 - (sx - ox);
                xdir = -1;
            }
            else
            {
                offsetx = sx - ox;
                xdir = 1;
            }
            if (flipy!=0)
            {
                offsety = (src.height - 1 - (sy - oy));
                ydir = -1;
            }
            else
            {
                offsety = sy - oy;
                ydir = 1;
            }
            switch (transparency)
            {
                case 0:
                    break;
                case 1:
                    //blockmove_NtoN_transpen_noremap16_1(sd, offsetx, offsety, xdir, ydir, sx, sy, sw, sh, sm, dd1, dm, transparent_color);
                    int i, j;
                    for (i = 0; i < sh; i++)
                    {
                        for (j = 0; j < sw; j++)
                        {
                            int col;
                            col = uu1[(offsety + ydir * i) * sm + offsetx + xdir * j];
                            if (col != transparent_color)
                            {
                                Video.bitmapbase[Video.curbitmap][(sy + i) * dm + sx + j] = (ushort)col;
                            }
                        }
                    }
                    break;
            }
        }
        public static void copybitmap_common(ushort[] uu1, bitmap_t dest, bitmap_t src, int flipx, int flipy, int sx, int sy, RECT clip, int transparency, int transparent_color)
        {
            copybitmap_core16(uu1, dest, src, flipx, flipy, sx, sy, clip, transparency, transparent_color);
        }
        public static void copybitmap_trans(ushort[] uu1, bitmap_t dest, bitmap_t src, int flipx, int flipy, int sx, int sy, RECT clip, int transparent_pen)
        {
            copybitmap_common(uu1, dest, src, flipx, flipy, sx, sy, clip, 1, transparent_pen);
        }
        public static void SaveStateBinary(BinaryWriter writer)
        {
            writer.Write(imode);
        }
        public static void LoadStateBinary(BinaryReader reader)
        {
            imode = reader.ReadInt32();
        }
    }
}
