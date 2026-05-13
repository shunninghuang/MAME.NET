using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mame
{
    public partial class Drawgfx
    {
        public static void common_drawgfx_airbustr(bitmap_t dest, byte[] bb1, int code, int color, int flipx, int flipy, int sx, int sy, RECT clip)
        {
            int ox;
            int oy;
            int ex;
            int ey;
            code %= spritecount;
            ox = sx;
            oy = sy;
            ex = sx + 0x10 - 1;
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
            ey = sy + 0x10 - 1;
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
            int sw = 0x10;
            int sh = 0x10;
            int sm = 0x10;
            int ls = sx - ox;
            int ts = sy - oy;
            int dw = ex - sx + 1;
            int dh = ey - sy + 1;
            int dm = dest.rowpixels;
            int colorbase = 0x200 + 0x10 * color;
            blockmove_8toN_transpen16_kaneko(dest, bb1, code, sw, sh, sm, ls, ts, flipx, flipy, dw, dh, dm, colorbase, 0, sx, sy);
        }
        public static void blockmove_8toN_transpen16_kaneko(bitmap_t dest, byte[] bb1, int code, int srcwidth, int srcheight, int srcmodulo, int leftskip, int topskip, int flipx, int flipy, int dstwidth, int dstheight, int dstmodulo, int colorbase, int transpen, int sx, int sy)
        {
            int ydir, xdir, col, i, j;
            int offsetx = sx, offsety = sy;
            int srcdata_offset = code * srcwidth * srcheight;
            if (flipy != 0)
            {
                offsety += (dstheight - 1);
                srcdata_offset += (srcheight - dstheight - topskip) * srcmodulo;
                ydir = -1;
            }
            else
            {
                srcdata_offset += topskip * srcmodulo;
                ydir = 1;
            }
            if (flipx != 0)
            {
                offsetx += (dstwidth - 1);
                srcdata_offset += (srcwidth - dstwidth - leftskip);
                xdir = -1;
            }
            else
            {
                srcdata_offset += leftskip;
                xdir = 1;
            }
            for (i = 0; i < dstheight; i++)
            {
                for (j = 0; j < dstwidth; j++)
                {
                    col = bb1[srcdata_offset + srcmodulo * i + j];
                    if (col != transpen)
                    {
                        dest.uu1[(offsety + ydir * i) * dstmodulo + offsetx + xdir * j] = (ushort)(colorbase + col);
                    }
                }
            }
        }
    }
}
