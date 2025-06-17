﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mame
{
    public partial class Gaelco
    {
        public static ushort[] gaelco_vregs, gaelco_videoram, gaelco_spriteram, gaelco_screen;
        public static int[] x_offset = new int[2] { 0x0, 0x2 };
        public static int[] y_offset = new int[2] { 0x0, 0x1 };
        public static void gaelco_vram_w(int offset, ushort data)
        {
            gaelco_videoram[offset] = data;
            int tile_index, row, col;
            tile_index = ((offset << 1) & 0x0fff) >> 2;
            col = tile_index % 0x20;
            row = tile_index / 0x20;
            gaelco_tilemap[offset >> 11].tilemap_mark_tile_dirty(row, col);
        }
        public static void gaelco_vram_w1(int offset, byte data)
        {
            gaelco_videoram[offset] = (ushort)((data << 8) | (gaelco_videoram[offset] & 0xff));
            int tile_index, row, col;
            tile_index = ((offset << 1) & 0x0fff) >> 2;
            col = tile_index % 0x20;
            row = tile_index / 0x20;
            gaelco_tilemap[offset >> 11].tilemap_mark_tile_dirty(row, col);
        }
        public static void gaelco_vram_w2(int offset,byte data)
        {
            gaelco_videoram[offset]=(ushort)((gaelco_videoram[offset]&0xff00)|data);
            int tile_index, row, col;
            tile_index = ((offset << 1) & 0x0fff) >> 2;
            col = tile_index % 0x20;
            row = tile_index / 0x20;
            gaelco_tilemap[offset >> 11].tilemap_mark_tile_dirty(row, col);
        }
        public static void draw_sprites(RECT cliprect)
        {
            int i, x, y, ex, ey;
            for (i = 0x800 - 4 - 1; i >= 3; i -= 4)
            {
                int sx = gaelco_spriteram[i + 2] & 0x01ff;
                int sy = (240 - (gaelco_spriteram[i] & 0x00ff)) & 0x00ff;
                int number = gaelco_spriteram[i + 3];
                int color = (gaelco_spriteram[i + 2] & 0x7e00) >> 9;
                int attr = (gaelco_spriteram[i] & 0xfe00) >> 9;
                int priority = (gaelco_spriteram[i] & 0x3000) >> 12;
                int xflip = attr & 0x20;
                int yflip = attr & 0x40;
                int spr_size, pri_mask;
                if (color >= 0x38)
                {
                    priority = 4;
                }
                switch (priority)
                {
                    case 0: pri_mask = 0xff00; break;
                    case 1: pri_mask = 0xff00 | 0xf0f0; break;
                    case 2: pri_mask = 0xff00 | 0xf0f0 | 0xcccc; break;
                    case 3: pri_mask = 0xff00 | 0xf0f0 | 0xcccc | 0xaaaa; break;
                    default:
                    case 4: pri_mask = 0; break;
                }
                if ((attr & 0x04)!=0)
                {
                    spr_size = 1;
                }
                else
                {
                    spr_size = 2;
                    number &= (~3);
                }
                for (y = 0; y < spr_size; y++)
                {
                    for (x = 0; x < spr_size; x++)
                    {
                        ex = xflip != 0 ? (spr_size - 1 - x) : x;
                        ey = yflip != 0 ? (spr_size - 1 - y) : y;
                        Drawgfx.common_drawgfx_gaelco(gfx1rom, number + x_offset[ex] + y_offset[ey], color, xflip, yflip, sx - 0x0f + x * 8, sy + y * 8, cliprect, (uint)(pri_mask | (1 << 31)));
                    }
                }
            }
        }
        public static void video_update_maniacsq()
        {
            gaelco_tilemap[0].tilemap_set_scrolly(0, gaelco_vregs[0]);
            gaelco_tilemap[0].tilemap_set_scrollx(0, gaelco_vregs[1] + 4);
            gaelco_tilemap[1].tilemap_set_scrolly(0, gaelco_vregs[2]);
            gaelco_tilemap[1].tilemap_set_scrollx(0, gaelco_vregs[3]);
            Array.Clear(Tilemap.priority_bitmap, 0, 0x40000);
            Array.Clear(Video.bitmapbase[Video.curbitmap], 0, 0x40000);
            gaelco_tilemap[1].tilemap_draw_primask(Video.new_clip, 0x13, 0);
            gaelco_tilemap[0].tilemap_draw_primask(Video.new_clip, 0x13, 0);
            gaelco_tilemap[1].tilemap_draw_primask(Video.new_clip, 0x12, 1);
            gaelco_tilemap[0].tilemap_draw_primask(Video.new_clip, 0x12, 1);
            gaelco_tilemap[1].tilemap_draw_primask(Video.new_clip, 0x11, 2);
            gaelco_tilemap[0].tilemap_draw_primask(Video.new_clip, 0x11, 2);
            gaelco_tilemap[1].tilemap_draw_primask(Video.new_clip, 0x10, 4);
            gaelco_tilemap[0].tilemap_draw_primask(Video.new_clip, 0x10, 4);
            draw_sprites(Video.new_clip);
        }
        public static void video_update_bigkarnk()
        {
            gaelco_tilemap[0].tilemap_set_scrolly(0, gaelco_vregs[0]);
            gaelco_tilemap[0].tilemap_set_scrollx(0, gaelco_vregs[1] + 4);
            gaelco_tilemap[1].tilemap_set_scrolly(0, gaelco_vregs[2]);
            gaelco_tilemap[1].tilemap_set_scrollx(0, gaelco_vregs[3]);
            Array.Clear(Tilemap.priority_bitmap, 0, 0x40000);
            Array.Clear(Video.bitmapbase[Video.curbitmap], 0, 0x40000);
            gaelco_tilemap[1].tilemap_draw_primask(Video.new_clip, 0x23, 0);
            gaelco_tilemap[0].tilemap_draw_primask(Video.new_clip, 0x23, 0);
            gaelco_tilemap[1].tilemap_draw_primask(Video.new_clip, 0x13, 1);
            gaelco_tilemap[0].tilemap_draw_primask(Video.new_clip, 0x13, 1);
            gaelco_tilemap[1].tilemap_draw_primask(Video.new_clip, 0x22, 1);
            gaelco_tilemap[0].tilemap_draw_primask(Video.new_clip, 0x22, 1);
            gaelco_tilemap[1].tilemap_draw_primask(Video.new_clip, 0x12, 2);
            gaelco_tilemap[0].tilemap_draw_primask(Video.new_clip, 0x12, 2);
            gaelco_tilemap[1].tilemap_draw_primask(Video.new_clip, 0x21, 2);
            gaelco_tilemap[0].tilemap_draw_primask(Video.new_clip, 0x21, 2);
            gaelco_tilemap[1].tilemap_draw_primask(Video.new_clip, 0x11, 4);
            gaelco_tilemap[0].tilemap_draw_primask(Video.new_clip, 0x11, 4);
            gaelco_tilemap[1].tilemap_draw_primask(Video.new_clip, 0x20, 4);
            gaelco_tilemap[0].tilemap_draw_primask(Video.new_clip, 0x20, 4);
            gaelco_tilemap[1].tilemap_draw_primask(Video.new_clip, 0x10, 8);
            gaelco_tilemap[0].tilemap_draw_primask(Video.new_clip, 0x10, 8);
            draw_sprites(Video.new_clip);
        }
        public static void video_eof_bigkarnk()
        {

        }
    }
}
