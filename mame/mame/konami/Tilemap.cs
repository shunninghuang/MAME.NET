using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace mame
{
    public partial class Tmap
    {
        public delegate void draw_opaque_delegate(uint[] ui1, int ypos, int offsety2, int xpos, int x_start, int x_end, byte[] bb1, byte priority);
        public draw_opaque_delegate draw_opaque;
        public delegate void draw_masked_delegate(uint[] ui1, int ypos, int offsety2, int xpos, int x_start, int x_end, byte[] bb1, byte priority);
        public draw_masked_delegate draw_masked;
        public void scanline_draw_opaque_rgb32(uint[] ui1, int ypos, int offsety2, int xpos, int x_start, int x_end, byte[] bb1, byte priority)//, const UINT16 *source, int count, UINT8 *pri, UINT32 pcode)
        {
            int i;
            for (i = xpos + x_start; i < xpos + x_end; i++)
            {
                ui1[(offsety2 + ypos) * Video.fullwidth + i] = Palette.entry_color2[pixmap[offsety2 * width + i - xpos]];
            }
            if (priority != 0)
            {
                for (i = xpos + x_start; i < xpos + x_end; i++)
                {
                    bb1[(offsety2 + ypos) * 0x200 + i] = (byte)(bb1[(offsety2 + ypos) * 0x200 + i] | priority);
                }
            }
        }
        public void scanline_draw_masked_rgb32(uint[] ui1, int ypos, int offsety2, int xpos, int x_start, int x_end, byte[] bb1, byte priority)
        {
            int i;
            for (i = xpos + x_start; i < xpos + x_end; i++)
            {
                if ((flagsmap[offsety2, i - xpos] & mask) == value)
                {
                    ui1[(offsety2 + ypos) * Video.fullwidth + i] = Palette.entry_color2[pixmap[offsety2 * width + i - xpos]];
                    bb1[(offsety2 + ypos) * 0x200 + i] = (byte)(bb1[(offsety2 + ypos) * 0x200 + i] | priority);
                }
            }
        }
        public void scanline_draw_opaque_rgb32_alpha(uint[] ui1, int ypos, int offsety2, int xpos, int x_start, int x_end, byte[] bb1, byte priority)
        {
            int i;
            for (i = xpos + x_start; i < xpos + x_end; i++)
            {
                ui1[(offsety2 + ypos) * Video.fullwidth + i] = Drawgfx.alpha_blend32(ui1[(offsety2 + ypos) * Video.fullwidth + i], Palette.entry_color2[pixmap[offsety2 * width + i - xpos]]);

            }
            if (priority != 0)
            {
                for (i = xpos + x_start; i < xpos + x_end; i++)
                {
                    bb1[(offsety2 + ypos) * 0x200 + i] = (byte)(bb1[(offsety2 + ypos) * 0x200 + i] | priority);
                }
            }
        }
        public void scanline_draw_masked_rgb32_alpha(uint[] ui1, int ypos, int offsety2, int xpos, int x_start, int x_end, byte[] bb1, byte priority)
        {
            int i;
            for (i = xpos + x_start; i < xpos + x_end; i++)
            {
                if ((flagsmap[offsety2, i - xpos] & mask) == value)
                {
                    ui1[(offsety2 + ypos) * Video.fullwidth + i] = Drawgfx.alpha_blend32(ui1[(offsety2 + ypos) * Video.fullwidth + i], Palette.entry_color2[pixmap[offsety2 * width + i - xpos]]);
                    bb1[(offsety2 + ypos) * 0x200 + i] = (byte)(bb1[(offsety2 + ypos) * 0x200 + i] | priority);
                }
            }
        }
        public void tile_update_konami_k052109_0(int logindex, int col, int row)
        {
            int x0 = tilewidth * col;
            int y0 = tileheight * row;
            int memindex;
            int code, color;
            int pen_data_offset, palette_base;
            int flipy = 0;
            memindex = logical_to_memory[logindex];
            code = Konami.K052109_ram[Konami.K052109_videoram_F_offset + memindex] + 256 * Konami.K052109_ram[Konami.K052109_videoram2_F_offset + memindex];
            color = Konami.K052109_ram[Konami.K052109_colorram_F_offset + memindex];
            byte flags = 0;
            int priority = 0;
            int bank = Konami.K052109_charrombank[(color & 0x0c) >> 2];
            if (Konami.has_extra_video_ram != 0)
            {
                bank = (color & 0x0c) >> 2;
            }
            color = (color & 0xf3) | ((bank & 0x03) << 2);
            bank >>= 2;
            flipy = color & 0x02;
            int code2, color2;
            int flags2;
            Konami.K052109_callback(0, bank, code, color, flags, priority, out code2, out color2, out flags2);
            code = code2;
            color = color2;
            flags = (byte)flags2;
            if ((Konami.K052109_tileflip_enable & 1) == 0)
            {
                flags &= unchecked((byte)(~0x01));
            }
            if (flipy != 0 && (Konami.K052109_tileflip_enable & 2) != 0)
            {
                flags |= 0x02;
            }
            code = code % Konami.K052109_tilemap[0].total_elements;
            pen_data_offset = code * 0x40;
            palette_base = 0x10 * color;
            flags = (byte)(flags ^ (attributes & 0x03));
            tileflags[logindex] = tile_draw(Konami.gfx1rom, pen_data_offset, x0, y0, palette_base, 0, 0, flags);
        }
        public void tile_update_konami_k052109_1(int logindex, int col, int row)
        {
            int x0 = tilewidth * col;
            int y0 = tileheight * row;
            int memindex;
            int code, color;
            int pen_data_offset, palette_base;
            int flipy = 0;
            memindex = logical_to_memory[logindex];
            code = Konami.K052109_ram[Konami.K052109_videoram_A_offset + memindex] + 256 * Konami.K052109_ram[Konami.K052109_videoram2_A_offset + memindex];
            color = Konami.K052109_ram[Konami.K052109_colorram_A_offset + memindex];
            byte flags = 0;
            int priority = 0;
            int bank = Konami.K052109_charrombank[(color & 0x0c) >> 2];
            if (Konami.has_extra_video_ram != 0)
            {
                bank = (color & 0x0c) >> 2;
            }
            color = (color & 0xf3) | ((bank & 0x03) << 2);
            bank >>= 2;
            flipy = color & 0x02;
            int code2, color2, flags2;
            Konami.K052109_callback(1, bank, code, color, flags, priority, out code2, out color2, out flags2);
            code = code2;
            color = color2;
            flags = (byte)flags2;
            if ((Konami.K052109_tileflip_enable & 1) == 0)
            {
                flags &= unchecked((byte)~0x01);
            }
            if (flipy != 0 && (Konami.K052109_tileflip_enable & 2) != 0)
            {
                flags |= 0x02;
            }
            code = code % Konami.K052109_tilemap[1].total_elements;
            pen_data_offset = code * 0x40;
            palette_base = 0x10 * color;
            flags = (byte)(flags ^ (attributes & 0x03));
            tileflags[logindex] = tile_draw(Konami.gfx1rom, pen_data_offset, x0, y0, palette_base, 0, 0, flags);
        }
        public void tile_update_konami_k052109_2(int logindex, int col, int row)
        {
            int x0 = tilewidth * col;
            int y0 = tileheight * row;
            int memindex;
            int code, color;
            int pen_data_offset, palette_base;
            int flipy = 0;
            memindex = logical_to_memory[logindex];
            code = Konami.K052109_ram[Konami.K052109_videoram_B_offset + memindex] + 256 * Konami.K052109_ram[Konami.K052109_videoram2_B_offset + memindex];
            color = Konami.K052109_ram[Konami.K052109_colorram_B_offset + memindex];
            byte flags = 0;
            int priority = 0;
            int bank = Konami.K052109_charrombank[(color & 0x0c) >> 2];
            if (Konami.has_extra_video_ram != 0)
            {
                bank = (color & 0x0c) >> 2;
            }
            color = (color & 0xf3) | ((bank & 0x03) << 2);
            bank >>= 2;
            flipy = color & 0x02;
            int code2, color2, flags2;
            Konami.K052109_callback(2, bank, code, color, flags, priority, out code2, out color2, out flags2);
            code = code2;
            color = color2;
            flags = (byte)flags2;
            if ((Konami.K052109_tileflip_enable & 1) == 0)
            {
                flags &= unchecked((byte)~0x01);
            }
            if (flipy != 0 && (Konami.K052109_tileflip_enable & 2) != 0)
            {
                flags |= 0x02;
            }
            code = code % Konami.K052109_tilemap[2].total_elements;
            pen_data_offset = code * 0x40;
            palette_base = 0x10 * color;
            flags = (byte)(flags ^ (attributes & 0x03));
            tileflags[logindex] = tile_draw(Konami.gfx1rom, pen_data_offset, x0, y0, palette_base, 0, 0, flags);
        }
        public void tile_update_roz_glfgreat(int logindex, int col, int row)
        {
            int x0 = tilewidth * col;
            int y0 = tileheight * row;
            int memindex;
            int code1, code, color;
            int pen_data_offset, palette_base;
            memindex = logical_to_memory[logindex];
            memindex += 0x40000 * Konami.glfgreat_roz_rom_bank;
            code1 = Konami.zoomtmaprom[memindex + 0x80000] + 256 * Konami.zoomtmaprom[memindex] + 256 * 256 * ((Konami.zoomtmaprom[memindex / 4 + 0x100000] >> (2 * (memindex & 3))) & 3);
            code = code1 & 0x3fff;
            color = code1 >> 14;
            pen_data_offset = code * 0x100;
            palette_base = 0x400 + 0x10 * color;
            tileflags[logindex] = tile_draw(Konami.gfx0rom, pen_data_offset, x0, y0, palette_base, 0, 0, 0);
        }
        public void tile_update_roz_prmrsocr(int logindex, int col, int row)
        {
            int x0 = tilewidth * col;
            int y0 = tileheight * row;
            int memindex;
            int code1, code, color;
            int pen_data_offset, palette_base;
            memindex = logical_to_memory[logindex];
            code1 = Konami.zoomtmaprom[memindex + 0x20000] + 256 * Konami.zoomtmaprom[memindex];
            code = (code1 & 0x1fff) % 0x1000;
            color = code1 >> 13;
            pen_data_offset = code * 0x100;
            palette_base = 0x400 + 0x10 * color;
            tileflags[logindex] = tile_draw(Konami.gfx0rom, pen_data_offset, x0, y0, palette_base, 0, 0, 0);
        }
        public void tile_update_konami_k056832(int logindex, int col, int row, int pageIndex)
        {
            int x0 = tilewidth * col;
            int y0 = tileheight * row;
            int memindex;
            int pen_data_offset, palette_base;
            int layer, flip, fbits, attr, code, color;
            byte flags;
            int K056832_videoram_offset;
            int code2, color2, flags2;
            memindex = logical_to_memory[logindex];
            K056832_videoram_offset = (pageIndex << 12) + (memindex << 1);
            if (Konami.K056832_LayerAssociation != 0)
            {
                layer = Konami.K056832_LayerAssociatedWithPage[pageIndex];
                if (layer == -1)
                {
                    layer = 0;
                }
            }
            else
            {
                layer = Konami.K056832_ActiveLayer;
            }
            fbits = Konami.K056832_regs[3] >> 6 & 3;
            flip = Konami.K056832_regs[1] >> (layer << 1) & 0x3;
            attr = Konami.K056832_videoram[K056832_videoram_offset];
            code = Konami.K056832_videoram[K056832_videoram_offset + 1];
            flip &= attr >> Konami.K056832_shiftmasks[fbits][0] & 3;
            color = (attr & Konami.K056832_shiftmasks[fbits][1]) | (attr >> Konami.K056832_shiftmasks[fbits][2] & Konami.K056832_shiftmasks[fbits][3]);
            flags = (byte)(flip & 3);
            Konami.K056832_callback(layer, code, color, flags, out code2, out color2, out flags2);
            code = code2;
            color = color2;
            flags = (byte)flags2;
            pen_data_offset = code * 0x40;
            palette_base = 0x10 * color;
            flags = (byte)(flags ^ (attributes & 0x03));
            tileflags[logindex] = tile_draw(Konami.gfx1rom, pen_data_offset, x0, y0, palette_base, 0, 0, flags);
        }
        public void tile_update_konami_k056832_0(int logindex, int col, int row)
        {
            tile_update_konami_k056832(logindex, col, row, 0x0);
        }
        public void tile_update_konami_k056832_1(int logindex, int col, int row)
        {
            tile_update_konami_k056832(logindex, col, row, 0x1);
        }
        public void tile_update_konami_k056832_2(int logindex, int col, int row)
        {
            tile_update_konami_k056832(logindex, col, row, 0x2);
        }
        public void tile_update_konami_k056832_3(int logindex, int col, int row)
        {
            tile_update_konami_k056832(logindex, col, row, 0x3);
        }
        public void tile_update_konami_k056832_4(int logindex, int col, int row)
        {
            tile_update_konami_k056832(logindex, col, row, 0x4);
        }
        public void tile_update_konami_k056832_5(int logindex, int col, int row)
        {
            tile_update_konami_k056832(logindex, col, row, 0x5);
        }
        public void tile_update_konami_k056832_6(int logindex, int col, int row)
        {
            tile_update_konami_k056832(logindex, col, row, 0x6);
        }
        public void tile_update_konami_k056832_7(int logindex, int col, int row)
        {
            tile_update_konami_k056832(logindex, col, row, 0x7);
        }
        public void tile_update_konami_k056832_8(int logindex, int col, int row)
        {
            tile_update_konami_k056832(logindex, col, row, 0x8);
        }
        public void tile_update_konami_k056832_9(int logindex, int col, int row)
        {
            tile_update_konami_k056832(logindex, col, row, 0x9);
        }
        public void tile_update_konami_k056832_a(int logindex, int col, int row)
        {
            tile_update_konami_k056832(logindex, col, row, 0xa);
        }
        public void tile_update_konami_k056832_b(int logindex, int col, int row)
        {
            tile_update_konami_k056832(logindex, col, row, 0xb);
        }
        public void tile_update_konami_k056832_c(int logindex, int col, int row)
        {
            tile_update_konami_k056832(logindex, col, row, 0xc);
        }
        public void tile_update_konami_k056832_d(int logindex, int col, int row)
        {
            tile_update_konami_k056832(logindex, col, row, 0xd);
        }
        public void tile_update_konami_k056832_e(int logindex, int col, int row)
        {
            tile_update_konami_k056832(logindex, col, row, 0xe);
        }
        public void tile_update_konami_k056832_f(int logindex, int col, int row)
        {
            tile_update_konami_k056832(logindex, col, row, 0xf);
        }
        public void tile_update_konami_gai_936(int logindex, int col, int row)
        {
            int x0 = tilewidth * col;
            int y0 = tileheight * row;
            int memindex;
            int pen_data_offset, palette_base;
            int code, color;
            memindex = logical_to_memory[logindex];
            code = Konami.gfx4rom[0x60000 + memindex] | ((Konami.gfx4rom[0x20000 + memindex] & 0x3f) << 8);
            if ((memindex & 1) != 0)
            {
                color = Konami.gfx4rom[memindex >> 1] & 0xf;
            }
            else
            {
                color = (Konami.gfx4rom[memindex >> 1] >> 4) & 0xf;
            }
            if ((Konami.gfx4rom[0x20000 + memindex] & 0x80) != 0)
            {
                color |= 0x10;
            }
            color |= Konami.sub1_colorbase << 4;
            pen_data_offset = code * 0x100;
            palette_base = 0x10 * color;
            tileflags[logindex] = tile_draw(Konami.gfx0rom, pen_data_offset, x0, y0, palette_base, 0, 0, 0);
        }
        public void tile_update_konami_ult_936(int logindex, int col, int row)
        {
            int x0 = tilewidth * col;
            int y0 = tileheight * row;
            int memindex;
            int pen_data_offset, palette_base;
            int code, color;
            byte flags;
            memindex = logical_to_memory[logindex];
            code = Konami.gfx4rom[0x40000 + memindex] | ((Konami.gfx4rom[memindex] & 0x1f) << 8);
            color = Konami.sub1_colorbase;
            pen_data_offset = code * 0x100;
            palette_base = 0x100 * color;
            flags = (byte)((Konami.gfx4rom[memindex] & 0x40) != 0 ? Tilemap.TILEMAP_FLIPX : 0);
            tileflags[logindex] = tile_draw(Konami.gfx0rom, pen_data_offset, x0, y0, palette_base, 0, 0, flags);
        }
        public void tilemap_draw_instance_konami_mystwarr(RECT cliprect, int xpos, int ypos)
        {
            int mincol, maxcol, logindex;
            int x1, y1, x2, y2;
            int y, nexty;
            int offsety1, offsety2;
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
                                draw_opaque(Palette.bbitmap[Video.curbitmap].ui1, ypos, offsety2, xpos, x_start, x_end, Tilemap.ppriority_bitmap, priority);
                                offsety2++;
                            }
                        }
                        else if (prev_trans == trans_t.MASKED)
                        {
                            for (cury = y; cury < nexty; cury++)
                            {
                                draw_masked(Palette.bbitmap[Video.curbitmap].ui1, ypos, offsety2, xpos, x_start, x_end, Tilemap.ppriority_bitmap, priority);
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
