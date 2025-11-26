using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mame
{
    public partial class Technos
    {
        public static Tmap bg_tilemap, fg_tilemap;
        public static void tilemap_init()
        {
            int i;
            bg_tilemap = new Tmap();
            bg_tilemap.cols = 32;
            bg_tilemap.rows = 32;
            bg_tilemap.tilewidth = 16;
            bg_tilemap.tileheight = 16;
            bg_tilemap.width = 0x200;
            bg_tilemap.height = 0x200;
            bg_tilemap.enable = true;
            bg_tilemap.all_tiles_dirty = true;
            bg_tilemap.total_elements = gfx2rom.Length / 0x40;
            bg_tilemap.pixmap = new ushort[0x200 * 0x200];
            bg_tilemap.flagsmap = new byte[0x200, 0x200];
            bg_tilemap.tileflags = new byte[32, 32];
            bg_tilemap.pen_data = new byte[0x100];
            bg_tilemap.pen_to_flags = new byte[1, 16];
            for (i = 0; i < 16; i++)
            {
                bg_tilemap.pen_to_flags[0, i] = 0x10;
            }
            bg_tilemap.scrollrows = 1;
            bg_tilemap.scrollcols = 1;
            bg_tilemap.rowscroll = new int[bg_tilemap.scrollrows];
            bg_tilemap.colscroll = new int[bg_tilemap.scrollcols];
            bg_tilemap.tilemap_draw_instance3 = bg_tilemap.tilemap_draw_instance_technos_ddragon;
            bg_tilemap.tile_update3 = bg_tilemap.tile_update_ddragon_bg;

            fg_tilemap = new Tmap();
            fg_tilemap.cols = 32;
            fg_tilemap.rows = 32;
            fg_tilemap.tilewidth = 8;
            fg_tilemap.tileheight = 8;
            fg_tilemap.width = 0x100;
            fg_tilemap.height = 0x100;
            fg_tilemap.enable = true;
            fg_tilemap.all_tiles_dirty = true;
            fg_tilemap.total_elements = gfx1rom.Length / 0x40;
            fg_tilemap.pixmap = new ushort[0x100 * 0x100];
            fg_tilemap.flagsmap = new byte[0x100, 0x100];
            fg_tilemap.tileflags = new byte[32, 32];
            fg_tilemap.pen_data = new byte[0x100];
            fg_tilemap.pen_to_flags = new byte[1, 16];
            fg_tilemap.pen_to_flags[0, 0] = 0;
            for (i = 1; i < 16; i++)
            {
                fg_tilemap.pen_to_flags[0, i] = 0x10;
            }
            fg_tilemap.scrollrows = 1;
            fg_tilemap.scrollcols = 1;
            fg_tilemap.rowscroll = new int[fg_tilemap.scrollrows];
            fg_tilemap.colscroll = new int[fg_tilemap.scrollcols];
            fg_tilemap.tilemap_draw_instance3 = fg_tilemap.tilemap_draw_instance_technos_ddragon;
            fg_tilemap.tile_update3 = fg_tilemap.tile_update_ddragon_fg;
            Tilemap.lsTmap = new List<Tmap>();
            Tilemap.lsTmap.Add(bg_tilemap);
            Tilemap.lsTmap.Add(fg_tilemap);
        }
        public static int background_scan(int col,int row)
        {
            return (col & 0x0f) + ((row & 0x0f) << 4) + ((col & 0x10) << 4) + ((row & 0x10) << 5);
        }
    }
    public partial class Tmap
    {
        public void tile_update_ddragon_bg(int col, int row)
        {
            int x0 = tilewidth * col;
            int y0 = tileheight * row;
            byte flags;
            int tile_index=0;
            int code, attr, color;
            int pen_data_offset, palette_base;
            if (attributes == 0)
            {
                tile_index = Technos.background_scan(col, row);
            }
            else if (attributes == Tilemap.TILEMAP_FLIPX)
            {
                tile_index = Technos.background_scan(0x1f - col, row);
            }
            else if (attributes == Tilemap.TILEMAP_FLIPY)
            {
                tile_index = Technos.background_scan(col, 0x1f - row);
            }
            else if (attributes == (Tilemap.TILEMAP_FLIPX | Tilemap.TILEMAP_FLIPY))
            {
                tile_index = Technos.background_scan(0x1f - col, 0x1f - row);
            }
            attr = Technos.ddragon_bgvideoram[2 * tile_index];
            code = (Technos.ddragon_bgvideoram[2 * tile_index + 1] + ((attr & 0x07) << 8)) % total_elements;
            color = (attr >> 3) & 0x07;
            flags = (byte)(((attr & 0xc0) >> 6) & 3);
            pen_data_offset = code * 0x100;
            palette_base = 0x100 + 0x10 * color;
            tileflags[row, col] = tile_draw(Technos.gfx3rom, pen_data_offset, x0, y0, palette_base, 0,0, flags);
        }
        public void tile_update_ddragon_fg(int col, int row)
        {
            int x0 = tilewidth * col;
            int y0 = tileheight * row;
            int tile_index = 0;
            int code, attr, color;
            int pen_data_offset, palette_base;
            if (attributes == 0)
            {
                tile_index = row * 0x20 + col;
            }
            else if (attributes == Tilemap.TILEMAP_FLIPX)
            {
                tile_index = row * 0x20 + 0x1f - col;
            }
            else if (attributes == Tilemap.TILEMAP_FLIPY)
            {
                tile_index = (0x1f - row) * 0x20 + col;
            }
            else if (attributes == (Tilemap.TILEMAP_FLIPX | Tilemap.TILEMAP_FLIPY))
            {
                tile_index = (0x1f - row) * 0x20 + 0x1f - col;
            }
            attr = Technos.ddragon_fgvideoram[2 * tile_index];
            code = (Technos.ddragon_fgvideoram[2 * tile_index + 1] + ((attr & 0x07) << 8)) % total_elements;
            color = attr >> 5;
            pen_data_offset = code * 0x40;
            palette_base = 0x10 * color;
            tileflags[row, col] = tile_draw(Technos.gfx1rom, pen_data_offset, x0, y0, palette_base, 0, 0, 0);
        }
        public void tilemap_draw_instance_technos_ddragon(RECT cliprect, int xpos, int ypos)
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
