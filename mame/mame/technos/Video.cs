using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mame
{
    public partial class Technos
    {
        public static void video_start_ddragon()
        {
            gfxtotalelement = gfx2rom.Length / 0x100;
            fg_tilemap.tilemap_set_scrolldx(0, 384 - 256);
            bg_tilemap.tilemap_set_scrolldx(0, 384 - 256);
            fg_tilemap.tilemap_set_scrolldy(-8, -8);
            bg_tilemap.tilemap_set_scrolldy(-8, -8);
        }
        public static void ddragon_bgvideoram_w(int offset, byte data)
        {
            int tile_index, row, col;
            ddragon_bgvideoram[offset] = data;
            tile_index = offset / 2;
            col = tile_index & 0x0f;
            row = (tile_index & 0xf0) >> 4;
            if ((tile_index & 0x100) != 0)
            {
                col |= 0x10;
            }
            if ((tile_index & 0x200) != 0)
            {
                row |= 0x10;
            }
            bg_tilemap.tilemap_mark_tile_dirty(row, col);
        }
        public static void ddragon_fgvideoram_w(int offset, byte data)
        {
            int tile_index, row, col;
            ddragon_fgvideoram[offset] = data;
            tile_index = offset / 2;
            col = tile_index % 0x20;
            row = tile_index / 0x20;
            fg_tilemap.tilemap_mark_tile_dirty(row, col);
        }
        public static void draw_sprites(RECT cliprect)
        {
            byte[] srcbyte = new byte[0x800];
            int i;
            if (technos_video_hw == 1)
            {
                Array.Copy(Generic.spriteram, srcbyte, 0x800);
            }
            else
            {
                Array.Copy(ddragon_spriteram, 0x800, srcbyte, 0, 0x800);
            }
            for (i = 0; i < (64 * 5); i += 5)
            {
                int attr = srcbyte[i + 1];
                if ((attr & 0x80) != 0)
                {
                    int sx = 240 - srcbyte[i + 4] + ((attr & 2) << 7);
                    int sy = 232 - srcbyte[i + 0] + ((attr & 1) << 8);
                    int size = (attr & 0x30) >> 4;
                    int flipx = (attr & 8);
                    int flipy = (attr & 4);
                    int dx = -16, dy = -16;
                    int which;
                    int color;
                    if (technos_video_hw == 2)
                    {
                        color = (srcbyte[i + 2] >> 5);
                        which = srcbyte[i + 3] + ((srcbyte[i + 2] & 0x1f) << 8);
                    }
                    else
                    {
                        if (technos_video_hw == 1)
                        {
                            if ((sx < -7) && (sx > -16))
                            {
                                sx += 256;
                            }
                            if ((sy < -7) && (sy > -16))
                            {
                                sy += 256;
                            }
                        }
                        color = (srcbyte[i + 2] >> 4) & 0x07;
                        which = srcbyte[i + 3] + ((srcbyte[i + 2] & 0x0f) << 8);
                    }
                    if (Generic.flip_screen_get() != 0)
                    {
                        sx = 240 - sx;
                        sy = 256 - sy;
                        flipx = (flipx == 0 ? 1 : 0);
                        flipy = (flipy == 0 ? 1 : 0);
                        dx = -dx;
                        dy = -dy;
                    }
                    which &= ~size;
                    switch (size)
                    {
                        case 0:
                            Drawgfx.common_drawgfx_ddragon(gfx2rom, 0x10, 0x10, 0x10, gfxtotalelement, which, color, flipx, flipy, sx, sy, cliprect);
                            break;
                        case 1:
                            Drawgfx.common_drawgfx_ddragon(gfx2rom, 0x10, 0x10, 0x10, gfxtotalelement, which, color, flipx, flipy, sx, sy + dy, cliprect);
                            Drawgfx.common_drawgfx_ddragon(gfx2rom, 0x10, 0x10, 0x10, gfxtotalelement, which + 1, color, flipx, flipy, sx, sy, cliprect);
                            break;
                        case 2:
                            Drawgfx.common_drawgfx_ddragon(gfx2rom, 0x10, 0x10, 0x10, gfxtotalelement, which, color, flipx, flipy, sx + dx, sy, cliprect);
                            Drawgfx.common_drawgfx_ddragon(gfx2rom, 0x10, 0x10, 0x10, gfxtotalelement, which + 2, color, flipx, flipy, sx, sy, cliprect);
                            break;
                        case 3:
                            Drawgfx.common_drawgfx_ddragon(gfx2rom, 0x10, 0x10, 0x10, gfxtotalelement, which, color, flipx, flipy, sx + dx, sy + dy, cliprect);
                            Drawgfx.common_drawgfx_ddragon(gfx2rom, 0x10, 0x10, 0x10, gfxtotalelement, which + 1, color, flipx, flipy, sx + dx, sy, cliprect);
                            Drawgfx.common_drawgfx_ddragon(gfx2rom, 0x10, 0x10, 0x10, gfxtotalelement, which + 2, color, flipx, flipy, sx, sy + dy, cliprect);
                            Drawgfx.common_drawgfx_ddragon(gfx2rom, 0x10, 0x10, 0x10, gfxtotalelement, which + 3, color, flipx, flipy, sx, sy, cliprect);
                            break;
                    }
                }
            }
        }
        public static void video_update_ddragon()
        {
            int scrollx = ddragon_scrollx_hi + ddragon_scrollx_lo;
            int scrolly = ddragon_scrolly_hi + ddragon_scrolly_lo;
            bg_tilemap.tilemap_set_scrollx(0, scrollx);
            bg_tilemap.tilemap_set_scrolly(0, scrolly);
            bg_tilemap.tilemap_draw_primask(Video.new_clip, 0x10, 0);
            draw_sprites(Video.new_clip);
            fg_tilemap.tilemap_draw_primask(Video.new_clip, 0x10, 0);
        }
        public static void video_eof_ddragon()
        {

        }
    }
}
