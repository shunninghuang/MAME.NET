using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace mame
{
    public partial class Capcom
    {
        public static void tilemap_init()
        {
            switch (Machine.sName)
            {
                case "gng":
                case "gnga":
                case "gngbl":
                case "gngprot":
                case "gngblita":
                case "gngc":
                case "gngt":
                case "makaimur":
                case "makaimurc":
                case "makaimurg":
                case "diamond":
                    tilemap_init_gng();
                    break;
                case "sf":
                case "sfua":
                case "sfj":
                case "sfjan":
                case "sfan":
                case "sfp":
                    tilemap_init_sf();
                    break;
            }
        }
        public static void tilemap_init_gng()
        {
            int i;
            fg_tilemap = new Tmap();
            fg_tilemap.cols = 0x20;
            fg_tilemap.rows = 0x20;
            fg_tilemap.tilewidth = 8;
            fg_tilemap.tileheight = 8;
            fg_tilemap.width = 0x100;
            fg_tilemap.height = 0x100;
            fg_tilemap.enable = true;
            fg_tilemap.all_tiles_dirty = true;
            fg_tilemap.total_elements = gfx1rom.Length / 0x40;
            fg_tilemap.pixmap = new ushort[0x100 * 0x100];
            fg_tilemap.flagsmap = new byte[0x100, 0x100];
            fg_tilemap.tileflags = new byte[0x20, 0x20];
            fg_tilemap.pen_data = new byte[0x40];
            fg_tilemap.pen_to_flags = new byte[1, 16];
            for (i = 0; i < 16; i++)
            {
                fg_tilemap.pen_to_flags[0, i] = 0x10;
            }
            fg_tilemap.pen_to_flags[0, 3] = 0;
            fg_tilemap.scrollrows = 1;
            fg_tilemap.scrollcols = 1;
            fg_tilemap.rowscroll = new int[fg_tilemap.scrollrows];
            fg_tilemap.colscroll = new int[fg_tilemap.scrollcols];
            fg_tilemap.tilemap_draw_instance3 = fg_tilemap.tilemap_draw_instance_capcom_gng;
            fg_tilemap.tile_update3 = fg_tilemap.tile_update_capcom_fg_gng;
            bg_tilemap = new Tmap();
            bg_tilemap.cols = 0x20;
            bg_tilemap.rows = 0x20;
            bg_tilemap.tilewidth = 0x10;
            bg_tilemap.tileheight = 0x10;
            bg_tilemap.width = 0x200;
            bg_tilemap.height = 0x200;
            bg_tilemap.enable = true;
            bg_tilemap.all_tiles_dirty = true;
            bg_tilemap.total_elements = gfx2rom.Length / 0x100;
            bg_tilemap.pixmap = new ushort[0x200 * 0x200];
            bg_tilemap.flagsmap = new byte[0x200, 0x200];
            bg_tilemap.tileflags = new byte[0x20, 0x20];
            bg_tilemap.pen_data = new byte[0x100];
            bg_tilemap.pen_to_flags = new byte[2, 16];
            for (i = 0; i < 8; i++)
            {
                bg_tilemap.pen_to_flags[0, i] = 0x20;
            }
            for (i = 8; i < 0x10; i++)
            {
                bg_tilemap.pen_to_flags[0, i] = 0x30;
            }
            bg_tilemap.pen_to_flags[1, 0] = 0x20;
            for (i = 1; i < 8; i++)
            {
                bg_tilemap.pen_to_flags[1, i] = 0x10;
            }
            for (i = 8; i < 0x10; i++)
            {
                bg_tilemap.pen_to_flags[1, i] = 0x30;
            }
            bg_tilemap.scrollrows = 1;
            bg_tilemap.scrollcols = 1;
            bg_tilemap.rowscroll = new int[bg_tilemap.scrollrows];
            bg_tilemap.colscroll = new int[bg_tilemap.scrollcols];
            bg_tilemap.tilemap_draw_instance3 = bg_tilemap.tilemap_draw_instance_capcom_gng;
            bg_tilemap.tile_update3 = bg_tilemap.tile_update_capcom_bg_gng;
        }
        public static void tilemap_init_sf()
        {
            int i;
            bg_tilemap = new Tmap();
            bg_tilemap.cols = 0x800;
            bg_tilemap.rows = 0x10;
            bg_tilemap.tilewidth = 0x10;
            bg_tilemap.tileheight = 0x10;
            bg_tilemap.width = 0x8000;
            bg_tilemap.height = 0x100;
            bg_tilemap.enable = true;
            bg_tilemap.all_tiles_dirty = true;
            bg_tilemap.total_elements = gfx1rom.Length / 0x100;
            bg_tilemap.pixmap = new ushort[0x100 * 0x8000];
            bg_tilemap.flagsmap = new byte[0x100, 0x8000];
            bg_tilemap.tileflags = new byte[0x10, 0x800];
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
            bg_tilemap.tilemap_draw_instance3 = bg_tilemap.tilemap_draw_instance_capcom_sf;
            bg_tilemap.tile_update3 = bg_tilemap.tile_update_capcom_bg;

            fg_tilemap = new Tmap();
            fg_tilemap.cols = 0x800;
            fg_tilemap.rows = 0x10;
            fg_tilemap.tilewidth = 0x10;
            fg_tilemap.tileheight = 0x10;
            fg_tilemap.width = 0x8000;
            fg_tilemap.height = 0x100;
            fg_tilemap.enable = true;
            fg_tilemap.all_tiles_dirty = true;
            fg_tilemap.total_elements = gfx2rom.Length / 0x100;
            fg_tilemap.pixmap = new ushort[0x100 * 0x8000];
            fg_tilemap.flagsmap = new byte[0x100, 0x8000];
            fg_tilemap.tileflags = new byte[0x10, 0x800];
            fg_tilemap.pen_data = new byte[0x100];
            fg_tilemap.pen_to_flags = new byte[1, 16];
            for (i = 0; i < 15; i++)
            {
                fg_tilemap.pen_to_flags[0, i] = 0x10;
            }
            fg_tilemap.pen_to_flags[0, 3] = 0;
            fg_tilemap.scrollrows = 1;
            fg_tilemap.scrollcols = 1;
            fg_tilemap.rowscroll = new int[bg_tilemap.scrollrows];
            fg_tilemap.colscroll = new int[bg_tilemap.scrollcols];
            fg_tilemap.tilemap_draw_instance3 = fg_tilemap.tilemap_draw_instance_capcom_sf;
            fg_tilemap.tile_update3 = fg_tilemap.tile_update_capcom_fg;

            tx_tilemap = new Tmap();
            tx_tilemap.cols = 0x40;
            tx_tilemap.rows = 0x20;
            tx_tilemap.tilewidth = 8;
            tx_tilemap.tileheight = 8;
            tx_tilemap.width = 0x200;
            tx_tilemap.height = 0x100;
            tx_tilemap.enable = true;
            tx_tilemap.all_tiles_dirty = true;
            tx_tilemap.total_elements = gfx4rom.Length / 0x40;
            tx_tilemap.pixmap = new ushort[0x100 * 0x200];
            tx_tilemap.flagsmap = new byte[0x100, 0x200];
            tx_tilemap.tileflags = new byte[0x20, 0x40];
            tx_tilemap.pen_data = new byte[0x40];
            tx_tilemap.pen_to_flags = new byte[1, 16];
            for (i = 0; i < 16; i++)
            {
                tx_tilemap.pen_to_flags[0, i] = 0x10;
            }
            tx_tilemap.pen_to_flags[0, 3] = 0;
            tx_tilemap.scrollrows = 1;
            tx_tilemap.scrollcols = 1;
            tx_tilemap.rowscroll = new int[bg_tilemap.scrollrows];
            tx_tilemap.colscroll = new int[bg_tilemap.scrollcols];
            tx_tilemap.tilemap_draw_instance3 = tx_tilemap.tilemap_draw_instance_capcom_sf;
            tx_tilemap.tile_update3 = tx_tilemap.tile_update_capcom_tx;
            Tilemap.lsTmap = new List<Tmap>();
            Tilemap.lsTmap.Add(bg_tilemap);
            Tilemap.lsTmap.Add(fg_tilemap);
            Tilemap.lsTmap.Add(tx_tilemap);
        }
    }
    public partial class Tmap
    {
        public void tilemap_draw_instance_capcom_gng(RECT cliprect, int xpos, int ypos)
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
                                Array.Copy(pixmap, offsety2 * width + x_start, Video.bitmapbase[Video.curbitmap], (offsety2 + ypos) * 0x100 + xpos + x_start, x_end - x_start);
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
                                        Video.bitmapbase[Video.curbitmap][(offsety2 + ypos) * 0x100 + i] = pixmap[offsety2 * width + i - xpos];
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
        public void tilemap_draw_instance_capcom_sf(RECT cliprect, int xpos, int ypos)
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
        public void tile_update_capcom_bg(int col, int row)
        {
            int x0 = tilewidth * col;
            int y0 = tileheight * row;
            byte flags;
            int tile_index;
            int code, color;
            int pen_data_offset, palette_base;
            tile_index = col * rows + row;
            int base_offset = 2 * tile_index;
            int attr = Capcom.gfx5rom[base_offset + 0x10000];
            color = Capcom.gfx5rom[base_offset];
            code = (Capcom.gfx5rom[base_offset + 0x10000 + 1] << 8) | Capcom.gfx5rom[base_offset + 1];
            code = code % total_elements;
            pen_data_offset = code * 0x100;
            palette_base = color * 0x10;
            flags = (byte)((attr & 0x03) ^ (attributes & 0x03));
            tileflags[row, col] = tile_draw(Capcom.gfx1rom, pen_data_offset, x0, y0, palette_base, 0, 0, flags);
        }
        public void tile_update_capcom_fg(int col, int row)
        {
            int x0 = tilewidth * col;
            int y0 = tileheight * row;
            byte flags;
            int tile_index;
            int code, color;
            int pen_data_offset, palette_base;
            tile_index = col * rows + row;
            int base_offset = 0x20000 + 2 * tile_index;
            int attr = Capcom.gfx5rom[base_offset + 0x10000];
            color = Capcom.gfx5rom[base_offset];
            code = (Capcom.gfx5rom[base_offset + 0x10000 + 1] << 8) | Capcom.gfx5rom[base_offset + 1];
            code = code % total_elements;
            pen_data_offset = code * 0x100;
            palette_base = 0x100 + (color * 0x10);
            flags = (byte)((attr & 0x03) ^ (attributes & 0x03));
            tileflags[row, col] = tile_draw(Capcom.gfx2rom, pen_data_offset, x0, y0, palette_base, 0, 0, flags);
        }
        public void tile_update_capcom_tx(int col, int row)
        {
            int x0 = tilewidth * col;
            int y0 = tileheight * row;
            byte flags;
            int tile_index;
            int code, color;
            int pen_data_offset, palette_base;
            tile_index = row * cols + col;
            code = Capcom.sf_videoram[tile_index];
            color = code >> 12;
            flags = (byte)((((code & 0xc00) >> 10) & 0x03) ^ (attributes & 0x03));
            code = (code & 0x3ff) % total_elements;
            pen_data_offset = code * 0x40;
            palette_base = 0x300 + (color * 0x4);
            tileflags[row, col] = tile_draw(Capcom.gfx4rom, pen_data_offset, x0, y0, palette_base, 0, 0, flags);
        }
        public void tile_update_capcom_bg_gng(int col, int row)
        {
            int x0 = tilewidth * col;
            int y0 = tileheight * row;
            byte flags;
            int tile_index;
            int code, color;
            int pen_data_offset, palette_base;
            byte group;
            tile_index = col * rows + row;
            int base_offset = 2 * tile_index;
            int attr = Capcom.gng_bgvideoram[tile_index + 0x400];
            color = attr & 0x07;
            code = Capcom.gng_bgvideoram[tile_index] + ((attr & 0xc0) << 2);
            code = code % total_elements;
            pen_data_offset = code * 0x100;
            palette_base = color * 8;
            flags = (byte)((((attr & 0x30) >> 4) & 0x03) ^ (attributes & 0x03));
            group = (byte)((attr & 0x08) >> 3);
            tileflags[row, col] = tile_draw(Capcom.gfx2rom, pen_data_offset, x0, y0, palette_base, 0, group, flags);
        }
        public void tile_update_capcom_fg_gng(int col, int row)
        {
            int x0 = tilewidth * col;
            int y0 = tileheight * row;
            byte flags;
            int tile_index;
            int code, color;
            int pen_data_offset, palette_base;
            tile_index = row * cols + col;
            int base_offset = 2 * tile_index;
            int attr = Capcom.gng_fgvideoram[tile_index + 0x400];
            color = attr & 0x0f;
            code = Capcom.gng_fgvideoram[tile_index] + ((attr & 0xc0) << 2);
            code = code % total_elements;
            pen_data_offset = code * 0x40;
            palette_base = 0x80 + color * 4;
            flags = (byte)((((attr & 0x30) >> 4) & 0x03) ^ (attributes & 0x03));
            tileflags[row, col] = tile_draw(Capcom.gfx1rom, pen_data_offset, x0, y0, palette_base, 0, 0, flags);
        }
    }
}
