using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mame
{
    public partial class Gaelco
    {
        public static Tmap[] gaelco_tilemap;
        public static void tilemap_init()
        {
            int i,j;
            gaelco_tilemap = new Tmap[2];
            for (i = 0; i < 2; i++)
            {
                gaelco_tilemap[i] = new Tmap();
                gaelco_tilemap[i].cols = 32;
                gaelco_tilemap[i].rows = 32;
                gaelco_tilemap[i].tilewidth = 16;
                gaelco_tilemap[i].tileheight = 16;
                gaelco_tilemap[i].width = 0x200;
                gaelco_tilemap[i].height = 0x200;
                gaelco_tilemap[i].enable = true;
                gaelco_tilemap[i].all_tiles_dirty = true;
                gaelco_tilemap[i].total_elements = gfx2rom.Length / 0x100;
                gaelco_tilemap[i].pixmap = new ushort[0x200 * 0x200];
                gaelco_tilemap[i].flagsmap = new byte[0x200, 0x200];
                gaelco_tilemap[i].tileflags = new byte[32, 32];
                gaelco_tilemap[i].pen_data = new byte[0x100];
                gaelco_tilemap[i].pen_to_flags = new byte[1, 16];                
                gaelco_tilemap[i].scrollrows = 1;
                gaelco_tilemap[i].scrollcols = 1;
                gaelco_tilemap[i].rowscroll = new int[gaelco_tilemap[i].scrollrows];
                gaelco_tilemap[i].colscroll = new int[gaelco_tilemap[i].scrollcols];
                gaelco_tilemap[i].tilemap_draw_instance3 = gaelco_tilemap[i].tilemap_draw_instance_gaelco;
                
            }
            gaelco_tilemap[0].tile_update3 = gaelco_tilemap[0].tile_update_gaelco_screen0;
            gaelco_tilemap[1].tile_update3 = gaelco_tilemap[1].tile_update_gaelco_screen1;
            switch (Machine.sName)
            {
                case "bigkarnk":
                    for (i = 0; i < 2; i++)
                    {
                        gaelco_tilemap[i].pen_to_flags[0, 0] = 0;
                        for (j = 1; j < 8; j++)
                        {
                            gaelco_tilemap[i].pen_to_flags[0, j] = 0x10;
                        }
                        for (j = 8; j < 16; j++)
                        {
                            gaelco_tilemap[i].pen_to_flags[0, j] = 0x20;
                        }
                    }
                    break;
                case "biomtoy":
                case "biomtoya":
                case "biomtoyb":
                case "biomtoyc":
                case "bioplayc":
                case "maniacsp":
                case "lastkm":
                case "squash":
                case "thoop":
                    for (i = 0; i < 2; i++)
                    {
                        gaelco_tilemap[i].pen_to_flags[0, 0] = 0;
                        for (j = 1; j < 16; j++)
                        {
                            gaelco_tilemap[i].pen_to_flags[0, j] = 0x10;
                        }
                    }
                    break;
            }
        }
    }
    public partial class Tmap
    {
        public void tile_update_gaelco_screen0(int col, int row)
        {
            int x0 = tilewidth * col;
            int y0 = tileheight * row;
            int tile_index;
            int code, color;
            byte category, flags;
            int pen_data_offset, palette_base;
            tile_index = row * cols + col;
            int data = Gaelco.gaelco_videoram[tile_index << 1];
            int data2 = Gaelco.gaelco_videoram[(tile_index << 1) + 1];
            code = ((data & 0xfffc) >> 2);
            color = data2 & 0x3f;
            pen_data_offset = (0x4000 + code) * 0x100;
            palette_base = 0x10 * color;
            category = (byte)((data2 >> 6) & 0x03);
            flags = (byte)(data & 0x03);
            tileflags[row, col] = tile_draw(Gaelco.gfx2rom, pen_data_offset, x0, y0, palette_base, category, 0, flags);
        }
        public void tile_update_gaelco_screen1(int col, int row)
        {
            int x0 = tilewidth * col;
            int y0 = tileheight * row;
            int tile_index;
            int code, color;
            byte category, flags;
            int pen_data_offset, palette_base;
            tile_index = row * cols + col;
            int data = Gaelco.gaelco_videoram[(0x1000 / 2) + (tile_index << 1)];
            int data2 = Gaelco.gaelco_videoram[(0x1000 / 2) + (tile_index << 1) + 1];
            code = ((data & 0xfffc) >> 2);
            color = data2 & 0x3f;
            pen_data_offset = (0x4000 + code) * 0x100;
            palette_base = 0x10 * color;
            category = (byte)((data2 >> 6) & 0x03);
            flags = (byte)(data & 0x03);
            tileflags[row, col] = tile_draw(Gaelco.gfx2rom, pen_data_offset, x0, y0, palette_base, category, 0, flags);
        }
        public void tilemap_draw_instance_gaelco(RECT cliprect, int xpos, int ypos)
        {
            int mincol, maxcol;
            int x1, y1, x2, y2;
            int y, nexty;
            int offsety1, offsety2;
            int i;
            x1 = Math.Max(xpos, cliprect.min_x);
            x2 = Math.Min(xpos + width, cliprect.max_x + 1);
            y1 = Math.Max(ypos, cliprect.min_y);
            y2 = Math.Min(ypos + height, cliprect.max_y + 1);
            if (x1 >= x2 || y1 >= y2)
                return;
            x1 -= xpos;
            y1 -= ypos;
            x2 -= xpos;
            y2 -= ypos;
            offsety1 = y1;
            mincol = x1 / tilewidth;
            maxcol = (x2 + tilewidth - 1) / tilewidth;
            y = y1;
            nexty = tileheight * (y1 / tileheight) + tileheight;
            nexty = Math.Min(nexty, y2);
            for (; ; )
            {
                int row = y / tileheight;
                trans_t prev_trans = trans_t.WHOLLY_TRANSPARENT;
                trans_t cur_trans;
                int x_start = x1;
                int column;
                for (column = mincol; column <= maxcol; column++)
                {
                    int x_end;
                    if (column == maxcol)
                    {
                        cur_trans = trans_t.WHOLLY_TRANSPARENT;
                    }
                    else
                    {
                        if (tileflags[row, column] == Tilemap.TILE_FLAG_DIRTY)
                        {
                            tile_update3(column, row);
                        }
                        if ((tileflags[row, column] & mask) != 0)
                        {
                            cur_trans = trans_t.MASKED;
                        }
                        else
                        {
                            cur_trans = ((flagsmap[offsety1, column * tilewidth] & mask) == value) ? trans_t.WHOLLY_OPAQUE : trans_t.WHOLLY_TRANSPARENT;
                        }
                    }
                    if (cur_trans == prev_trans)
                    {
                        continue;
                    }
                    x_end = column * tilewidth;
                    x_end = Math.Max(x_end, x1);
                    x_end = Math.Min(x_end, x2);
                    if (prev_trans != trans_t.WHOLLY_TRANSPARENT)
                    {
                        int cury;
                        offsety2 = offsety1;
                        if (prev_trans == trans_t.WHOLLY_OPAQUE)
                        {
                            for (cury = y; cury < nexty; cury++)
                            {
                                Array.Copy(pixmap, offsety2 * width + x_start, Video.bitmapbase[Video.curbitmap], (offsety2 + ypos) * 0x200 + xpos + x_start, x_end - x_start);
                                offsety2++;
                            }
                        }
                        else if (prev_trans == trans_t.MASKED)
                        {
                            for (cury = y; cury < nexty; cury++)
                            {
                                for (i = xpos + x_start; i < xpos + x_end; i++)
                                {
                                    if ((flagsmap[offsety2, i - xpos] & mask) == value)
                                    {
                                        Video.bitmapbase[Video.curbitmap][(offsety2 + ypos) * 0x200 + i] = pixmap[offsety2 * width + i - xpos];
                                    }
                                }
                                offsety2++;
                            }
                        }
                    }
                    x_start = x_end;
                    prev_trans = cur_trans;
                }
                if (nexty == y2)
                {
                    break;
                }
                offsety1 += (nexty - y);
                y = nexty;
                nexty += tileheight;
                nexty = Math.Min(nexty, y2);
            }
        }
    }
}
