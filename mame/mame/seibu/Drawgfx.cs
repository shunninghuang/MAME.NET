using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mame
{
    public partial class Drawgfx
    {
        public static void common_drawgfx_kncljoe(bitmap_t dest, byte[] bb1, int code, int color, int flipx, int flipy, int sx, int sy, RECT clip)
        {
            int ox;
            int oy;
            int ex;
            int ey;
            code %= Seibu.spritecount[Seibu.sprite_bank];
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
            int colorbase = 0x80 + 8 * color;
            blockmove_8toN_transpen16_m72(bb1, code, sw, sh, sm, ls, ts, flipx, flipy, dw, dh, dm, colorbase, 0, sx, sy);
        }
    }
}
