using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mame
{
    public partial class Namcos1
    {
        public static Tmap[] ttmap;
        public static void tilemap_init()
        {
            int i, j;
            ttmap = new Tmap[6];
            ttmap[0] = new Tmap();
            ttmap[0].rows = 64;
            ttmap[0].cols = 64;
            ttmap[0].videoram_offset = 0x0000;
            ttmap[1] = new Tmap();
            ttmap[1].rows = 64;
            ttmap[1].cols = 64;
            ttmap[1].videoram_offset = 0x2000;
            ttmap[2] = new Tmap();
            ttmap[2].rows = 64;
            ttmap[2].cols = 64;
            ttmap[2].videoram_offset = 0x4000;
            ttmap[3] = new Tmap();
            ttmap[3].rows = 32;
            ttmap[3].cols = 64;
            ttmap[3].videoram_offset = 0x6000;
            ttmap[4] = new Tmap();
            ttmap[4].rows = 28;
            ttmap[4].cols = 36;
            ttmap[4].videoram_offset = 0x7010;
            ttmap[5] = new Tmap();
            ttmap[5].rows = 28;
            ttmap[5].cols = 36;
            ttmap[5].videoram_offset = 0x7810;
            for (i = 0; i < 6; i++)
            {
                ttmap[i].tilewidth = 8;
                ttmap[i].tileheight = 8;
                ttmap[i].width = ttmap[i].cols * ttmap[i].tilewidth;
                ttmap[i].height = ttmap[i].rows * ttmap[i].tileheight;
                ttmap[i].enable = true;
                ttmap[i].all_tiles_dirty = true;
                ttmap[i].scrollrows = 1;
                ttmap[i].scrollcols = 1;
                ttmap[i].rowscroll = new int[ttmap[i].scrollrows];
                ttmap[i].colscroll = new int[ttmap[i].scrollcols];
                ttmap[i].pixmap = new ushort[ttmap[i].width * ttmap[i].height];
                ttmap[i].flagsmap = new byte[ttmap[i].height, ttmap[i].width];
                ttmap[i].tileflags = new byte[ttmap[i].rows, ttmap[i].cols];
                ttmap[i].pen_data = new byte[ttmap[i].tilewidth * ttmap[i].tileheight];
                ttmap[i].pen_to_flags = new byte[1, 0x100];
                for (j = 0; j < 0x100; j++)
                {
                    ttmap[i].pen_to_flags[0, j] = 0x10;
                }
                ttmap[i].tile_update3 = ttmap[i].tile_update_namcos1;
                ttmap[i].tilemap_draw_instance3 = ttmap[i].tilemap_draw_instance_namcos1;
            }
            Tilemap.lsTmap = new List<Tmap>();
            for (i = 0; i < 6; i++)
            {
                Tilemap.lsTmap.Add(ttmap[i]);
            }
        }
    }
    public partial class Tmap
    {
        public void tile_update_namcos1(int col, int row)
        {
            int x0 = tilewidth * col;
            int y0 = tileheight * row;
            int code, tile_index, col2, row2;
            byte flags;
            if ((attributes & Tilemap.TILEMAP_FLIPX) != 0)
            {
                col2 = (cols - 1) - col;
            }
            else
            {
                col2 = col;
            }
            if ((attributes & Tilemap.TILEMAP_FLIPY) != 0)
            {
                row2 = (rows - 1) - row;
            }
            else
            {
                row2 = row;
            }
            tile_index = (row2 * cols + col2) << 1;
            code = Namcos1.namcos1_videoram[videoram_offset + tile_index + 1] + ((Namcos1.namcos1_videoram[videoram_offset + tile_index] & 0x3f) << 8);
            flags = (byte)(attributes & 0x03);
            tileflags[row, col] = tile_draw(Namcos1.gfx2rom, code * 0x40, x0, y0, 0x800, 0, 0, flags);
            tileflags[row, col] = tile_apply_bitmask(Namcos1.gfx1rom, code << 3, x0, y0, 0, flags);
        }
        public void tilemap_draw_instance_namcos1(RECT cliprect, int xpos, int ypos)
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
                        cur_trans = trans_t.WHOLLY_TRANSPARENT;
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
                        continue;
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
                                for (i = xpos + x_start; i < xpos + x_end; i++)
                                {
                                    Video.bitmapbase[Video.curbitmap][(offsety2 + ypos) * 0x200 + i] = (ushort)(pixmap[offsety2 * width + i - xpos] + palette_offset);
                                    Tilemap.priority_bitmap[offsety2 + ypos, i] = (byte)(Tilemap.priority_bitmap[offsety2 + ypos, i] | priority);
                                }
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
                                        Video.bitmapbase[Video.curbitmap][(offsety2 + ypos) * 0x200 + i] = (ushort)(pixmap[offsety2 * width + i - xpos] + palette_offset);
                                        Tilemap.priority_bitmap[offsety2 + ypos, i] = (byte)(Tilemap.priority_bitmap[offsety2 + ypos, i] | priority);
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
                    break;
                offsety1 += (nexty - y);
                y = nexty;
                nexty += tileheight;
                nexty = Math.Min(nexty, y2);
            }
        }
    }
}
