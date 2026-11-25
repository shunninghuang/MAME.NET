using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace mame
{
    public partial class Tmap
    {
        public void tile_update_sega_16b_text(int logindex, int col, int row)
        {
            int x0 = tilewidth * col;
            int y0 = tileheight * row;
            int memindex;
            int code, color;
            byte category;
            int pen_data_offset, palette_base;
            memindex = logical_to_memory[logindex];
            ushort data = sega.bg_tilemap[0].textram[memindex];
            int bank = sega.bg_tilemap[0].textmap_info.bank[0];
            color = (data >> 9) & 0x07;
            code = bank*sega.bg_tilemap[0].textmap_info.banksize+ (data & 0x1ff);
            category = (byte)((data >> 15) & 1);
            //code = code % 0x8000;
            pen_data_offset = code * 0x40;
            palette_base = 8 * color;
            tileflags[logindex] = tile_draw(sega.tilesrom, pen_data_offset, x0, y0, palette_base,category, 0, 0);
        }
        public void tile_update_sega_16b_tile(int logindex, int col, int row,int pagenum)
        {
            int x0 = tilewidth * col;
            int y0 = tileheight * row;
            int memindex;
            int code, color;
            byte category;
            int pen_data_offset, palette_base;
            memindex = logical_to_memory[logindex];
            ushort data = sega.bg_tilemap[0].tileram[pagenum * 64 * 32 + memindex];
            color = (data >> 6) & 0x7f;
            code = data & 0x1fff;
            code = sega.bg_tilemap[0].tilemap_info1[pagenum].bank[code / sega.bg_tilemap[0].tilemap_info1[pagenum].banksize] * sega.bg_tilemap[0].tilemap_info1[pagenum].banksize + code % sega.bg_tilemap[0].tilemap_info1[pagenum].banksize;
            //code = code % 0x8000;
            category = (byte)((data >> 15) & 1);
            pen_data_offset = code * 0x40;
            palette_base = 8 * color;
            tileflags[logindex] = tile_draw(sega.tilesrom, pen_data_offset, x0, y0, palette_base, category, 0, 0);
        }
        public void tile_update_sega_16b_tile_0(int logindex, int col, int row)
        {
            tile_update_sega_16b_tile(logindex, col, row, 0x0);
        }
        public void tile_update_sega_16b_tile_1(int logindex, int col, int row)
        {
            tile_update_sega_16b_tile(logindex, col, row, 0x1);
        }
        public void tile_update_sega_16b_tile_2(int logindex, int col, int row)
        {
            tile_update_sega_16b_tile(logindex, col, row, 0x2);
        }
        public void tile_update_sega_16b_tile_3(int logindex, int col, int row)
        {
            tile_update_sega_16b_tile(logindex, col, row, 0x3);
        }
        public void tile_update_sega_16b_tile_4(int logindex, int col, int row)
        {
            tile_update_sega_16b_tile(logindex, col, row, 0x4);
        }
        public void tile_update_sega_16b_tile_5(int logindex, int col, int row)
        {
            tile_update_sega_16b_tile(logindex, col, row, 0x5);
        }
        public void tile_update_sega_16b_tile_6(int logindex, int col, int row)
        {
            tile_update_sega_16b_tile(logindex, col, row, 0x6);
        }
        public void tile_update_sega_16b_tile_7(int logindex, int col, int row)
        {
            tile_update_sega_16b_tile(logindex, col, row, 0x7);
        }
        public void tile_update_sega_16b_tile_8(int logindex, int col, int row)
        {
            tile_update_sega_16b_tile(logindex, col, row, 0x8);
        }
        public void tile_update_sega_16b_tile_9(int logindex, int col, int row)
        {
            tile_update_sega_16b_tile(logindex, col, row, 0x9);
        }
        public void tile_update_sega_16b_tile_a(int logindex, int col, int row)
        {
            tile_update_sega_16b_tile(logindex, col, row, 0xa);
        }
        public void tile_update_sega_16b_tile_b(int logindex, int col, int row)
        {
            tile_update_sega_16b_tile(logindex, col, row, 0xb);
        }
        public void tile_update_sega_16b_tile_c(int logindex, int col, int row)
        {
            tile_update_sega_16b_tile(logindex, col, row, 0xc);
        }
        public void tile_update_sega_16b_tile_d(int logindex, int col, int row)
        {
            tile_update_sega_16b_tile(logindex, col, row, 0xd);
        }
        public void tile_update_sega_16b_tile_e(int logindex, int col, int row)
        {
            tile_update_sega_16b_tile(logindex, col, row, 0xe);
        }
        public void tile_update_sega_16b_tile_f(int logindex, int col, int row)
        {
            tile_update_sega_16b_tile(logindex, col, row, 0xf);
        }
        public void tilemap_draw_instance_sega_system18(RECT cliprect, int xpos, int ypos)
        {
            int mincol, maxcol, logindex;
            int x1, y1, x2, y2;
            int y, nexty;
            int offsety1, offsety2;
            int i;
            x1 = Math.Max(xpos, cliprect.min_x);
            x2 = Math.Min(xpos + width, cliprect.max_x + 1);
            y1 = Math.Max(ypos, cliprect.min_y);
            y2 = Math.Min(ypos + height, cliprect.max_y + 1);
            if (x1 >= x2 || y1 >= y2)
            {
                return;
            }
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
                        logindex = row * cols + column;
                        if (tileflags[logindex] == Tilemap.TILE_FLAG_DIRTY)
                        {
                            tile_update3(logindex, column, row);
                        }
                        if ((tileflags[logindex] & mask) != 0)
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
                                Array.Copy(pixmap, offsety2 * width + x_start, Palette.bbitmap[Video.curbitmap].uu1, (offsety2 + ypos) * Video.fullwidth + xpos + x_start, x_end - x_start);
                                if (priority != 0)
                                {
                                    for (i = xpos + x_start; i < xpos + x_end; i++)
                                    {
                                        Tilemap.priority_bitmap[(offsety2 + ypos) * Tilemap.screen_width + i] = (byte)(Tilemap.priority_bitmap[(offsety2 + ypos) * Tilemap.screen_width + i] | priority);
                                    }
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
                                        Palette.bbitmap[Video.curbitmap].uu1[(offsety2 + ypos) * Video.fullwidth + i] = pixmap[offsety2 * width + i - xpos];
                                        Tilemap.priority_bitmap[(offsety2 + ypos) * Tilemap.screen_width + i] = (byte)(Tilemap.priority_bitmap[(offsety2 + ypos) * Tilemap.screen_width + i] | priority);
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
