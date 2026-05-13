using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace mame
{
    public partial class Kaneko
    {
        public static byte[] pandora_spriteram;
        public static byte pandora_region;
        public static bitmap_t bitmap, pandora_sprites_bitmap;
        public static int pandora_clear_bitmap;
        public static int pandora_xoffset, pandora_yoffset;
        public static void pandora_set_clear_bitmap(int clear)
        {
            pandora_clear_bitmap = clear;
        }
        public static void pandora_update(RECT cliprect)
        {
            if (pandora_sprites_bitmap == null)
            {
                return;
            }
            Drawgfx.copybitmap_trans(pandora_sprites_bitmap.uu1, bitmap, pandora_sprites_bitmap, 0, 0, 0, 0, cliprect, 0);
        }
        public static void pandora_draw(bitmap_t bitmap, RECT cliprect)
        {
            int sx = 0, sy = 0, x = 0, y = 0, offs;
            for (offs = 0; offs < 0x1000; offs += 8)
            {
                int dx = pandora_spriteram[offs + 4];
                int dy = pandora_spriteram[offs + 5];
                int tilecolour = pandora_spriteram[offs + 3];
                int attr = pandora_spriteram[offs + 7];
                int flipx = attr & 0x80;
                int flipy = (attr & 0x40) << 1;
                int tile = ((attr & 0x3f) << 8) + (pandora_spriteram[offs + 6] & 0xff);
                if ((tilecolour & 1)!=0)
                {
                    dx |= 0x100;
                }
                if ((tilecolour & 2)!=0)
                {
                    dy |= 0x100;
                }
                if ((tilecolour & 4)!=0)
                {
                    x += dx;
                    y += dy;
                }
                else
                {
                    x = dx;
                    y = dy;
                }
                if (Generic.flip_screen_get() != 0)
                {
                    sx = 240 - x;
                    sy = 240 - y;
                    flipx = flipx == 0 ? 1 : 0;
                    flipy = flipy == 0 ? 1 : 0;
                }
                else
                {
                    sx = x;
                    sy = y;
                }
                sx += pandora_xoffset;
                sy += pandora_yoffset;
                sx &= 0x1ff;
                sy &= 0x1ff;
                if ((sx & 0x100)!=0)
                {
                    sx -= 0x200;
                }
                if ((sy & 0x100)!=0)
                {
                    sy -= 0x200;
                }
                Drawgfx.common_drawgfx_airbustr(bitmap, gfx2rom, tile, (tilecolour & 0xf0) >> 4, flipx, flipy, sx, sy, cliprect);
            }
        }
        public static void pandora_eof()
        {
            if (pandora_clear_bitmap != 0)
            {
                Array.Clear(pandora_sprites_bitmap.uu1, 0, 0x10000);
            }
            pandora_draw(pandora_sprites_bitmap, Video.new_clip);
        }
        public static void pandora_start(byte region, int x, int y)
        {
            pandora_region = region;
            pandora_xoffset = x;
            pandora_yoffset = y;
            pandora_spriteram = new byte[0x1000];
            pandora_sprites_bitmap = new bitmap_t();
            pandora_sprites_bitmap.rowpixels = 0x100;
            pandora_sprites_bitmap.width = 0x100;
            pandora_sprites_bitmap.height = 0x100;
            pandora_sprites_bitmap.uu1 = new ushort[0x10000];
            pandora_clear_bitmap = 1;
        }
        public static void pandora_spriteram_w(int offset, byte data)
        {
            offset = Gaelco.BITSWAP16(offset, 15, 14, 13, 12, 11, 7, 6, 5, 4, 3, 2, 1, 0, 10, 9, 8);
            if (pandora_spriteram == null)
            {
                return;
            }
            if (offset >= 0x1000)
            {
                return;
            }
            pandora_spriteram[offset] = data;
        }
        public static byte pandora_spriteram_r(int offset)
        {
            offset = Gaelco.BITSWAP16(offset, 15, 14, 13, 12, 11, 7, 6, 5, 4, 3, 2, 1, 0, 10, 9, 8);
            if (pandora_spriteram == null)
            {
                return 0x00;
            }
            if (offset >= 0x1000)
            {
                return 0x00;
            }
            return pandora_spriteram[offset];
        }
    }
}
