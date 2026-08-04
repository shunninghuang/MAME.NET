using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace mame
{
    public partial class Konami
    {
        public static Tmap[] K052109_tilemap, K053251_tilemaps, K056832_tilemap;
        private static byte[] K052109_memory_region;
        public static int K052109_videoram_F_offset, K052109_videoram2_F_offset, K052109_colorram_F_offset, K052109_videoram_A_offset, K052109_videoram2_A_offset, K052109_colorram_A_offset, K052109_videoram_B_offset, K052109_videoram2_B_offset, K052109_colorram_B_offset;
        public static byte[] K052109_ram, K052109_charrombank, K052109_charrombank_2;
        public static LineState K052109_RMRD_line;
        public static byte K052109_romsubbank, K052109_scrollctrl, K052109_irq_enabled, has_extra_video_ram;
        public static int[] K052109_dx, K052109_dy;
        public static int K052109_tileflip_enable;

        public static byte[] K051960_memory_region;
        public static int K051960_romoffset, K051960_spriteflip, K051960_readroms;
        public static byte[] K051960_spriterombank, K051960_ram;
        public static int K051960_dx, K051960_dy;
        public static int K051960_irq_enabled, K051960_nmi_enabled;

        private static byte[][] K053245_memory_region;
        private static int K05324x_z_rejection;
        private static int[] K053244_rombank, K053245_ramsize, K053245_dx, K053245_dy;
        public static ushort[][] K053245_buffer;
        public static byte[][] K053245_ram, K053244_regs;

        public static int K053247_dx, K053247_dy, K053247_wraparound;
        public static byte[] K053246_regs;
        public static ushort[] K053247_regs;
        public static ushort[] K053247_ram;
        public static gfx_element K053247_gfx;
        public static byte K053246_OBJCHA_line;
        public static byte[] K053247_memory_region;

        public static byte[] K054000_ram;

        public static ushort[] K053936_0_ctrl, K053936_0_linectrl;
        public static ushort[] K053936_1_ctrl, K053936_1_linectrl;
        public static int[][] K053936_offset;
        public static int[] K053936_wraparound;

        public static byte[] K053251_ram;
        public static int[] K053251_palette_index;
        public static int K053251_tilemaps_set;

        public static ushort[][] K056832_pixmap;
        public static ushort[] K056832_regs, K056832_regsb;
        public static ushort[] K056832_videoram;
        public static int K056832_NumGfxBanks, K056832_CurGfxBank, K056832_gfxnum, K056832_rom_half;
        public static int[] K056832_LayerAssociatedWithPage;
        public static int[][] K056832_LayerOffset, K056832_LSRAMPage;
        public static int[] K056832_X, K056832_Y, K056832_W, K056832_H, K056832_dx, K056832_dy;
        public static uint[][] K056832_LineDirty;
        public static byte[] K056832_AllLinesDirty, K056832_PageTileMode, K056832_LayerTileMode;
        public static int K056832_DefaultLayerAssociation, K056832_LayerAssociation, K056832_ActiveLayer, K056832_SelectedPage, K056832_SelectedPagex4096, K056832_UpdateMode, K056832_linemap_enabled, K056832_use_ext_linescroll, K056832_uses_tile_banks, K056832_cur_tile_bank, K056832_djmain_hack;
        public static int[][] K056832_shiftmasks;
        public static byte[] K056832_rom;

        public static byte[] k55555_regs;

        public static ushort[] k54338_regs;
        public static int[] K054338_shdRGB;
        public static int K054338_alphainverted;

        public struct K053250_CHIPTAG
        {
            public byte[] regs;
            public byte[] rom;
            public ushort[] ram, rammax;
            public ushort[][] buffer;
            public int buffer0offset, buffer1offset,rammaxoffset;
            public uint rommask;
            public int[] page;
            public int frame, offsx, offsy;
        }
        public struct K053250_struct
        {
            public int chips;
            public K053250_CHIPTAG[] chip;
        }

        public static K053250_struct K053250_info;

        public static ushort[] K053252_regs;

        public static int counter;
        public delegate void K052109_delegate(int tmap, int bank, int code, int color, int flags, int priority, out int code2, out int color2, out int flags2);
        public static K052109_delegate K052109_callback;
        public delegate void K051960_delegate(int code, int color, int priority, int shadow, out int code2, out int color2, out int priority2);
        public static K051960_delegate K051960_callback;
        public delegate void K053245_delegate(int code, int color, out int code2, out int color2, out int priority_mask);
        public static K053245_delegate K053245_callback;
        public delegate void K053247_delegate(int code, int color, int priority, out int code2, out int color2, out int priority2);
        public static K053247_delegate K053247_callback;


        public delegate void K056832_delegate(int layer, int code, int color, int flags, out int code2, out int color2, out int flags2);
        public static K056832_delegate K056832_callback;

        public static void K052109_vh_start(K052109_delegate _K052109_callback)
        {
            int i, j;
            K052109_callback = _K052109_callback;
            K052109_RMRD_line = LineState.CLEAR_LINE;
            K052109_irq_enabled = 0;
            has_extra_video_ram = 0;
            K052109_tilemap = new Tmap[3];
            K052109_tilemap[0] = Tmap.tilemap_create(Tmap.tilemap_scan_rows, 8, 8, 64, 32);
            K052109_tilemap[1] = Tmap.tilemap_create(Tmap.tilemap_scan_rows, 8, 8, 64, 32);
            K052109_tilemap[2] = Tmap.tilemap_create(Tmap.tilemap_scan_rows, 8, 8, 64, 32);
            K052109_ram = new byte[0x6000];
            K052109_colorram_F_offset = 0x0000;
            K052109_colorram_A_offset = 0x0800;
            K052109_colorram_B_offset = 0x1000;
            K052109_videoram_F_offset = 0x2000;
            K052109_videoram_A_offset = 0x2800;
            K052109_videoram_B_offset = 0x3000;
            K052109_videoram2_F_offset = 0x4000;
            K052109_videoram2_A_offset = 0x4800;
            K052109_videoram2_B_offset = 0x5000;
            //tilemap_set_transparent_pen(K052109_tilemap[0],0);
            //tilemap_set_transparent_pen(K052109_tilemap[1],0);
            //tilemap_set_transparent_pen(K052109_tilemap[2],0);
            K052109_tilemap[0].scrollrows = 1;
            K052109_tilemap[0].scrollcols = 1;
            K052109_tilemap[1].scrollrows = 256;
            K052109_tilemap[2].scrollrows = 256;
            K052109_tilemap[1].scrollcols = 512;
            K052109_tilemap[2].scrollcols = 512;
            for (i = 0; i < 3; i++)
            {
                K052109_tilemap[i].rowscroll = new int[K052109_tilemap[i].scrollrows];
                K052109_tilemap[i].colscroll = new int[K052109_tilemap[1].scrollcols];
                K052109_tilemap[i].tilemap_draw_instance3 = K052109_tilemap[i].tilemap_draw_instance_cps;
                K052109_tilemap[i].pen_to_flags = new byte[1, 16];
                K052109_tilemap[i].pen_to_flags[0, 0] = 0;
                for (j = 1; j < 16; j++)
                {
                    K052109_tilemap[i].pen_to_flags[0, j] = 0x10;
                }
                K052109_tilemap[i].total_elements = gfx1rom.Length / 0x40;
            }
            K052109_tilemap[0].tile_update3 = K052109_tilemap[0].tile_update_konami_k052109_0;
            K052109_tilemap[1].tile_update3 = K052109_tilemap[1].tile_update_konami_k052109_1;
            K052109_tilemap[2].tile_update3 = K052109_tilemap[2].tile_update_konami_k052109_2;
            for (i = 0; i < 3; i++)
            {
                K052109_dx[i] = K052109_dy[i] = 0;
            }
        }
        public static byte K052109_r(int offset)
        {
            if (K052109_RMRD_line == LineState.CLEAR_LINE)
            {
                return K052109_ram[offset];
            }
            else
            {
                int code = (offset & 0x1fff) >> 5;
                int color = K052109_romsubbank;
                int code2, color2, flags2;
                int flags = 0;
                int priority = 0;
                int bank = K052109_charrombank[(color & 0x0c) >> 2] >> 2;
                int addr;
                bank |= (K052109_charrombank_2[(color & 0x0c) >> 2] >> 2);
                if (has_extra_video_ram != 0)
                {
                    code |= color << 8;
                }
                else
                {
                    K052109_callback(0, bank, code, color, flags, priority, out code2, out color2, out flags2);
                    code = code2;
                    color = color2;
                }
                addr = (code << 5) + (offset & 0x1f);
                addr &= K052109_memory_region.Length - 1;
                return K052109_memory_region[addr];
            }
        }
        public static void K052109_w(int offset, byte data)
        {
            if (offset == 0x90d)
            {
                int i1 = 1;
            }
            if (offset == 0x290d)
            {
                int i1 = 1;
            }
            if ((offset & 0x1fff) < 0x1800)
            {
                if (offset >= 0x4000)
                {
                    has_extra_video_ram = 1;
                }
                K052109_ram[offset] = data;
                K052109_tilemap[(offset & 0x1800) >> 11].tilemap_mark_tile_dirty(offset & 0x7ff);
            }
            else
            {
                K052109_ram[offset] = data;
                if (offset >= 0x180c && offset < 0x1834)
                {

                }
                else if (offset >= 0x1a00 && offset < 0x1c00)
                {

                }
                else if (offset == 0x1c80)
                {
                    if (K052109_scrollctrl != data)
                    {
                        K052109_scrollctrl = data;
                    }
                }
                else if (offset == 0x1d00)
                {
                    K052109_irq_enabled = (byte)(data & 0x04);
                }
                else if (offset == 0x1d80)
                {
                    int dirty = 0;
                    if (K052109_charrombank[0] != (data & 0x0f)) dirty |= 1;
                    if (K052109_charrombank[1] != ((data >> 4) & 0x0f)) dirty |= 2;
                    if (dirty != 0)
                    {
                        int i;
                        K052109_charrombank[0] = (byte)(data & 0x0f);
                        K052109_charrombank[1] = (byte)((data >> 4) & 0x0f);
                        for (i = 0; i < 0x1800; i++)
                        {
                            int bank = (K052109_ram[i] & 0x0c) >> 2;
                            if ((bank == 0 && ((dirty & 1) != 0) || (bank == 1 && ((dirty & 2) != 0))))
                            {
                                K052109_tilemap[(i & 0x1800) >> 11].tilemap_mark_tile_dirty(i & 0x7ff);
                            }
                        }
                    }
                }
                else if (offset == 0x1e00 || offset == 0x3e00)
                {
                    K052109_romsubbank = data;
                }
                else if (offset == 0x1e80)
                {
                    //tilemap_set_flip(K052109_tilemap[0], (data & 1) ? (TILEMAP_FLIPY | TILEMAP_FLIPX) : 0);
                    //tilemap_set_flip(K052109_tilemap[1], (data & 1) ? (TILEMAP_FLIPY | TILEMAP_FLIPX) : 0);
                    //tilemap_set_flip(K052109_tilemap[2], (data & 1) ? (TILEMAP_FLIPY | TILEMAP_FLIPX) : 0);
                    if (K052109_tileflip_enable != ((data & 0x06) >> 1))
                    {
                        K052109_tileflip_enable = ((data & 0x06) >> 1);
                        K052109_tilemap[0].all_tiles_dirty = true;
                        K052109_tilemap[1].all_tiles_dirty = true;
                        K052109_tilemap[2].all_tiles_dirty = true;
                    }
                }
                else if (offset == 0x1f00)
                {
                    int dirty = 0;
                    if (K052109_charrombank[2] != (data & 0x0f)) dirty |= 1;
                    if (K052109_charrombank[3] != ((data >> 4) & 0x0f)) dirty |= 2;
                    if (dirty != 0)
                    {
                        int i;
                        K052109_charrombank[2] = (byte)(data & 0x0f);
                        K052109_charrombank[3] = (byte)((data >> 4) & 0x0f);
                        for (i = 0; i < 0x1800; i++)
                        {
                            int bank = (K052109_ram[i] & 0x0c) >> 2;
                            if ((bank == 2 && ((dirty & 1) != 0)) || (bank == 3 && ((dirty & 2) != 0)))
                            {
                                K052109_tilemap[(i & 0x1800) >> 11].tilemap_mark_tile_dirty(i & 0x7ff);
                            }
                        }
                    }
                }
                else if (offset >= 0x380c && offset < 0x3834)
                {

                }
                else if (offset >= 0x3a00 && offset < 0x3c00)
                {

                }
                else if (offset == 0x3d80)
                {
                    K052109_charrombank_2[0] = (byte)(data & 0x0f);
                    K052109_charrombank_2[1] = (byte)((data >> 4) & 0x0f);
                }
                else if (offset == 0x3f00)
                {
                    K052109_charrombank_2[2] = (byte)(data & 0x0f);
                    K052109_charrombank_2[3] = (byte)((data >> 4) & 0x0f);
                }
            }
        }
        public static ushort K052109_word_r(int offset)
        {
            return (ushort)(K052109_r(offset + 0x2000) | (K052109_r(offset) << 8));
        }
        public static void K052109_word_w(int offset, ushort data)
        {
            K052109_w(offset, (byte)((data >> 8) & 0xff));
            K052109_w(offset + 0x2000, (byte)(data & 0xff));
        }
        public static void K052109_word_w1(int offset, byte data)
        {
            K052109_w(offset, data);
        }
        public static void K052109_word_w2(int offset, byte data)
        {
            K052109_w(offset + 0x2000, data);
        }
        public static void K052109_set_RMRD_line(LineState state)
        {
            K052109_RMRD_line = state;
        }
        public static int K052109_get_RMRD_line()
        {
            return (int)K052109_RMRD_line;
        }
        public static void K052109_tilemap_update()
        {
            if ((K052109_scrollctrl & 0x03) == 0x02)
            {
                int xscroll, yscroll, offs;
                int scrollram_offset = 0x1a00;
                K052109_tilemap[1].tilemap_set_scroll_rows(256);
                K052109_tilemap[1].tilemap_set_scroll_cols(1);
                yscroll = K052109_ram[0x180c];
                K052109_tilemap[1].tilemap_set_scrolly(0, yscroll + K052109_dy[1]);
                for (offs = 0; offs < 256; offs++)
                {
                    xscroll = K052109_ram[scrollram_offset + 2 * (offs & 0xfff8) + 0] + 256 * K052109_ram[scrollram_offset + 2 * (offs & 0xfff8) + 1];
                    xscroll -= 6;
                    K052109_tilemap[1].tilemap_set_scrollx((offs + yscroll) & 0xff, xscroll + K052109_dx[1]);
                }
            }
            else if ((K052109_scrollctrl & 0x03) == 0x03)
            {
                int xscroll, yscroll, offs;
                int scrollram_offset = 0x1a00;
                K052109_tilemap[1].tilemap_set_scroll_rows(256);
                K052109_tilemap[1].tilemap_set_scroll_cols(1);
                yscroll = K052109_ram[0x180c];
                K052109_tilemap[1].tilemap_set_scrolly(0, yscroll + K052109_dy[1]);
                for (offs = 0; offs < 256; offs++)
                {
                    xscroll = K052109_ram[scrollram_offset + 2 * offs + 0] + 256 * K052109_ram[scrollram_offset + 2 * offs + 1];
                    xscroll -= 6;
                    K052109_tilemap[1].tilemap_set_scrollx((offs + yscroll) & 0xff, xscroll + K052109_dx[1]);
                }
            }
            else if ((K052109_scrollctrl & 0x04) == 0x04)
            {
                int xscroll, yscroll, offs;
                int scrollram_offset = 0x1800;
                K052109_tilemap[1].tilemap_set_scroll_rows(1);
                K052109_tilemap[1].tilemap_set_scroll_cols(512);
                xscroll = K052109_ram[0x1a00] + 256 * K052109_ram[0x1a01];
                xscroll -= 6;
                K052109_tilemap[1].tilemap_set_scrollx(0, xscroll + K052109_dx[1]);
                for (offs = 0; offs < 512; offs++)
                {
                    yscroll = K052109_ram[scrollram_offset + offs / 8];
                    K052109_tilemap[1].tilemap_set_scrolly((offs + xscroll) & 0x1ff, yscroll + K052109_dy[1]);
                }
            }
            else
            {
                int xscroll, yscroll;
                int scrollram_offset = 0x1a00;
                K052109_tilemap[1].tilemap_set_scroll_rows(1);
                K052109_tilemap[1].tilemap_set_scroll_cols(1);
                xscroll = K052109_ram[scrollram_offset + 0] + 256 * K052109_ram[scrollram_offset + 1];
                xscroll -= 6;
                yscroll = K052109_ram[0x180c];
                K052109_tilemap[1].tilemap_set_scrollx(0, xscroll + K052109_dx[1]);
                K052109_tilemap[1].tilemap_set_scrolly(0, yscroll + K052109_dy[1]);
            }
            if ((K052109_scrollctrl & 0x18) == 0x10)
            {
                int xscroll, yscroll, offs;
                int scrollram_offset = 0x3a00;
                K052109_tilemap[2].tilemap_set_scroll_rows(256);
                K052109_tilemap[2].tilemap_set_scroll_cols(1);
                yscroll = K052109_ram[0x380c];
                K052109_tilemap[2].tilemap_set_scrolly(0, yscroll + K052109_dy[2]);
                for (offs = 0; offs < 256; offs++)
                {
                    xscroll = K052109_ram[scrollram_offset + 2 * (offs & 0xfff8) + 0] + 256 * K052109_ram[scrollram_offset + 2 * (offs & 0xfff8) + 1];
                    xscroll -= 6;
                    K052109_tilemap[2].tilemap_set_scrollx((offs + yscroll) & 0xff, xscroll + K052109_dx[2]);
                }
            }
            else if ((K052109_scrollctrl & 0x18) == 0x18)
            {
                int xscroll, yscroll, offs;
                int scrollram_offset = 0x3a00;
                K052109_tilemap[2].tilemap_set_scroll_rows(256);
                K052109_tilemap[2].tilemap_set_scroll_cols(1);
                yscroll = K052109_ram[0x380c];
                K052109_tilemap[2].tilemap_set_scrolly(0, yscroll + K052109_dy[2]);
                for (offs = 0; offs < 256; offs++)
                {
                    xscroll = K052109_ram[scrollram_offset + 2 * offs + 0] + 256 * K052109_ram[scrollram_offset + 2 * offs + 1];
                    xscroll -= 6;
                    K052109_tilemap[2].tilemap_set_scrollx((offs + yscroll) & 0xff, xscroll + K052109_dx[2]);
                }
            }
            else if ((K052109_scrollctrl & 0x20) == 0x20)
            {
                int xscroll, yscroll, offs;
                int scrollram_offset = 0x3800;
                K052109_tilemap[2].tilemap_set_scroll_rows(1);
                K052109_tilemap[2].tilemap_set_scroll_cols(512);
                xscroll = K052109_ram[0x3a00] + 256 * K052109_ram[0x3a01];
                xscroll -= 6;
                K052109_tilemap[2].tilemap_set_scrollx(0, xscroll + K052109_dx[2]);
                for (offs = 0; offs < 512; offs++)
                {
                    yscroll = K052109_ram[scrollram_offset + offs / 8];
                    K052109_tilemap[2].tilemap_set_scrolly((offs + xscroll) & 0x1ff, yscroll + K052109_dy[2]);
                }
            }
            else
            {
                int xscroll, yscroll;
                int scrollram_offset = 0x3a00;
                K052109_tilemap[2].tilemap_set_scroll_rows(1);
                K052109_tilemap[2].tilemap_set_scroll_cols(1);
                xscroll = K052109_ram[scrollram_offset + 0] + 256 * K052109_ram[scrollram_offset + 1];
                xscroll -= 6;
                yscroll = K052109_ram[0x380c];
                K052109_tilemap[2].tilemap_set_scrollx(0, xscroll + K052109_dx[2]);
                K052109_tilemap[2].tilemap_set_scrolly(0, yscroll + K052109_dy[2]);
            }
        }
        public static int K052109_is_IRQ_enabled()
        {
            return K052109_irq_enabled;
        }
        public static void SaveStateBinary_K052109(BinaryWriter writer)
        {
            int i;
            writer.Write(K052109_ram, 0, 0x6000);
            writer.Write((int)K052109_RMRD_line);
            writer.Write(K052109_romsubbank);
            writer.Write(K052109_scrollctrl);
            writer.Write(K052109_irq_enabled);
            writer.Write(K052109_charrombank, 0, 4);
            writer.Write(K052109_charrombank_2, 0, 4);
            for (i = 0; i < 3; i++)
            {
                writer.Write(K052109_dx[i]);
            }
            for (i = 0; i < 3; i++)
            {
                writer.Write(K052109_dy[i]);
            }
            writer.Write(has_extra_video_ram);
            writer.Write(K052109_tileflip_enable);
        }
        public static void LoadStateBinary_K052109(BinaryReader reader)
        {
            int i;
            K052109_ram = reader.ReadBytes(0x6000);
            K052109_RMRD_line = (LineState)reader.ReadInt32();
            K052109_romsubbank = reader.ReadByte();
            K052109_scrollctrl = reader.ReadByte();
            K052109_irq_enabled = reader.ReadByte();
            K052109_charrombank = reader.ReadBytes(4);
            K052109_charrombank_2 = reader.ReadBytes(4);
            for (i = 0; i < 3; i++)
            {
                K052109_dx[i] = reader.ReadInt32();
            }
            for (i = 0; i < 3; i++)
            {
                K052109_dy[i] = reader.ReadInt32();
            }
            has_extra_video_ram = reader.ReadByte();
            K052109_tileflip_enable = reader.ReadInt32();
        }
        public static void LoadStateBinary_K052109_2(BinaryReader reader)
        {
            int i;
            reader.ReadBytes(0x6000);
            reader.ReadInt32();
            reader.ReadByte();
            reader.ReadByte();
            reader.ReadByte();
            reader.ReadBytes(4);
            reader.ReadBytes(4);
            for (i = 0; i < 3; i++)
            {
                reader.ReadInt32();
            }
            for (i = 0; i < 3; i++)
            {
                reader.ReadInt32();
            }
            reader.ReadByte();
            reader.ReadInt32();
        }
        public static void K051960_vh_start(K051960_delegate _K051960_callback)
        {
            int i;
            Drawgfx.gfx_drawmode_table[0] = 0;
            for (i = 1; i < 15; i++)
            {
                Drawgfx.gfx_drawmode_table[i] = 1;
            }
            Drawgfx.gfx_drawmode_table[15] = 2;
            K051960_dx = K051960_dy = 0;
            K051960_callback = _K051960_callback;
            K051960_ram = new byte[0x400];
            K051960_spriterombank = new byte[3];
        }
        public static byte K051960_fetchromdata(int byte1)
        {
            int code, color, pri, shadow, off1, addr, code2, color2, pri2;
            addr = K051960_romoffset + (K051960_spriterombank[0] << 8) +
                    ((K051960_spriterombank[1] & 0x03) << 16);
            code = (addr & 0x3ffe0) >> 5;
            off1 = addr & 0x1f;
            color = ((K051960_spriterombank[1] & 0xfc) >> 2) + ((K051960_spriterombank[2] & 0x03) << 6);
            pri = 0;
            shadow = color & 0x80;
            K051960_callback(code, color, pri, shadow, out code2, out color2, out pri2);
            addr = (code2 << 7) | (off1 << 2) | byte1;
            addr &= K051960_memory_region.Length - 1;
            return K051960_memory_region[addr];
        }
        public static byte K051960_r(int offset)
        {
            if (K051960_readroms != 0)
            {
                K051960_romoffset = (offset & 0x3fc) >> 2;
                return K051960_fetchromdata(offset & 3);
            }
            else
            {
                return K051960_ram[offset];
            }
        }
        public static void K051960_w(int offset, byte data)
        {
            K051960_ram[offset] = data;
        }
        public static byte K051937_r(int offset)
        {
            if (K051960_readroms != 0 && offset >= 4 && offset < 8)
            {
                return K051960_fetchromdata(offset & 3);
            }
            else
            {
                if (offset == 0)
                {
                    return (byte)((counter++) & 1);
                }
                return 0;
            }
        }
        public static void K051937_w(int offset, byte data)
        {
            if (offset == 0)
            {
                K051960_irq_enabled = (data & 0x01);
                K051960_nmi_enabled = (data & 0x04);
                K051960_spriteflip = data & 0x08;
                K051960_readroms = data & 0x20;
            }
            else if (offset == 1)
            {

            }
            else if (offset >= 2 && offset < 5)
            {
                K051960_spriterombank[offset - 2] = data;
            }
            else
            {

            }
        }
        public static void K051960_sprites_draw(RECT cliprect, int min_priority, int max_priority)
        {
            int ox, oy, code, color, pri, shadow, size, w, h, x, y, flipx, flipy, zoomx, zoomy, code2, color2, pri2;
            int offs, pri_code;
            int[] sortedlist = new int[128];
            int[] xoffset = new int[] { 0, 1, 4, 5, 16, 17, 20, 21 };
            int[] yoffset = new int[] { 0, 2, 8, 10, 32, 34, 40, 42 };
            int[] width = new int[] { 1, 2, 1, 2, 4, 2, 4, 8 };
            int[] height = new int[] { 1, 1, 2, 2, 2, 4, 4, 8 };
            for (offs = 0; offs < 128; offs++)
            {
                sortedlist[offs] = -1;
            }
            for (offs = 0; offs < 0x400; offs += 8)
            {
                if ((K051960_ram[offs] & 0x80) != 0)
                {
                    if (max_priority == -1)
                    {
                        sortedlist[(K051960_ram[offs] & 0x7f) ^ 0x7f] = offs;
                    }
                    else
                    {
                        sortedlist[K051960_ram[offs] & 0x7f] = offs;
                    }
                }
            }
            for (pri_code = 0; pri_code < 128; pri_code++)
            {
                offs = sortedlist[pri_code];
                if (offs == -1)
                {
                    continue;
                }
                code = K051960_ram[offs + 2] + ((K051960_ram[offs + 1] & 0x1f) << 8);
                color = K051960_ram[offs + 3] & 0xff;
                pri = 0;
                shadow = color & 0x80;
                K051960_callback(code, color, pri, shadow, out code2, out color2, out pri2);
                code = code2;
                color = color2;
                pri = pri2;
                if (max_priority != -1)
                {
                    if (pri < min_priority || pri > max_priority)
                    {
                        continue;
                    }
                }
                size = (K051960_ram[offs + 1] & 0xe0) >> 5;
                w = width[size];
                h = height[size];
                if (w >= 2) code &= ~0x01;
                if (h >= 2) code &= ~0x02;
                if (w >= 4) code &= ~0x04;
                if (h >= 4) code &= ~0x08;
                if (w >= 8) code &= ~0x10;
                if (h >= 8) code &= ~0x20;
                ox = (256 * K051960_ram[offs + 6] + K051960_ram[offs + 7]) & 0x01ff;
                oy = 256 - ((256 * K051960_ram[offs + 4] + K051960_ram[offs + 5]) & 0x01ff);
                ox += K051960_dx;
                oy += K051960_dy;
                flipx = K051960_ram[offs + 6] & 0x02;
                flipy = K051960_ram[offs + 4] & 0x02;
                zoomx = (K051960_ram[offs + 6] & 0xfc) >> 2;
                zoomy = (K051960_ram[offs + 4] & 0xfc) >> 2;
                zoomx = 0x10000 / 128 * (128 - zoomx);
                zoomy = 0x10000 / 128 * (128 - zoomy);
                if (K051960_spriteflip != 0)
                {
                    ox = 512 - (zoomx * w >> 12) - ox;
                    oy = 256 - (zoomy * h >> 12) - oy;
                    flipx = (flipx == 0) ? 1 : 0;
                    flipy = (flipy == 0) ? 1 : 0;
                }
                if (zoomx == 0x10000 && zoomy == 0x10000)
                {
                    int sx, sy;
                    for (y = 0; y < h; y++)
                    {
                        sy = oy + 16 * y;
                        for (x = 0; x < w; x++)
                        {
                            int c = code;
                            sx = ox + 16 * x;
                            if (flipx != 0)
                            {
                                c += xoffset[(w - 1 - x)];
                            }
                            else
                            {
                                c += xoffset[x];
                            }
                            if (flipy != 0)
                            {
                                c += yoffset[(h - 1 - y)];
                            }
                            else
                            {
                                c += yoffset[y];
                            }
                            if (max_priority == -1)
                            {
                                Drawgfx.common_drawgfx_konami(gfx2rom, c, color, flipx, flipy, sx & 0x1ff, sy, cliprect, shadow, (uint)(pri | (1 << 31)));
                            }
                            else
                            {
                                Drawgfx.common_drawgfx_konami(gfx2rom, c, color, flipx, flipy, sx & 0x1ff, sy, cliprect, shadow, 0);
                            }
                        }
                    }
                }
                else
                {
                    int sx, sy, zw, zh;
                    for (y = 0; y < h; y++)
                    {
                        sy = oy + ((zoomy * y + (1 << 11)) >> 12);
                        zh = (oy + ((zoomy * (y + 1) + (1 << 11)) >> 12)) - sy;
                        for (x = 0; x < w; x++)
                        {
                            int c = code;
                            sx = ox + ((zoomx * x + (1 << 11)) >> 12);
                            zw = (ox + ((zoomx * (x + 1) + (1 << 11)) >> 12)) - sx;
                            if (flipx != 0)
                            {
                                c += xoffset[(w - 1 - x)];
                            }
                            else c += xoffset[x];
                            if (flipy != 0)
                            {
                                c += yoffset[(h - 1 - y)];
                            }
                            else
                            {
                                c += yoffset[y];
                            }
                            if (max_priority == -1)
                            {
                                Drawgfx.common_drawgfxzoom_konami(gfx2rom, c, color, flipx, flipy, sx & 0x1ff, sy, cliprect, shadow, 0, (zw << 16) / 16, (zh << 16) / 16, (uint)(pri | (1 << 31)));
                            }
                            else
                            {
                                Drawgfx.common_drawgfxzoom_konami(gfx2rom, c, color, flipx, flipy, sx & 0x1ff, sy, cliprect, shadow, 0, (zw << 16) / 16, (zh << 16) / 16);
                            }
                        }
                    }
                }
            }
        }
        public static byte K052109_051960_r(int offset)
        {
            if (K052109_RMRD_line == LineState.CLEAR_LINE)
            {
                if (offset >= 0x3800 && offset < 0x3808)
                {
                    return K051937_r(offset - 0x3800);
                }
                else if (offset < 0x3c00)
                {
                    return K052109_r(offset);
                }
                else
                {
                    return K051960_r(offset - 0x3c00);
                }
            }
            else
            {
                return K052109_r(offset);
            }
        }
        public static void K052109_051960_w(int offset, byte data)
        {
            if (offset >= 0x3800 && offset < 0x3808)
            {
                K051937_w(offset - 0x3800, data);
            }
            else if (offset < 0x3c00)
            {
                K052109_w(offset, data);
            }
            else
            {
                K051960_w(offset - 0x3c00, data);
            }
        }
        public static void SaveStateBinary_K051960(BinaryWriter writer)
        {
            writer.Write(K051960_romoffset);
            writer.Write(K051960_spriteflip);
            writer.Write(K051960_readroms);
            writer.Write(K051960_spriterombank, 0, 3);
            writer.Write(K051960_ram, 0, 0x400);
            writer.Write(K051960_dx);
            writer.Write(K051960_dy);
            writer.Write(K051960_irq_enabled);
            writer.Write(K051960_nmi_enabled);
        }
        public static void LoadStateBinary_K051960(BinaryReader reader)
        {
            K051960_romoffset = reader.ReadInt32();
            K051960_spriteflip = reader.ReadInt32();
            K051960_readroms = reader.ReadInt32();
            K051960_spriterombank = reader.ReadBytes(3);
            K051960_ram = reader.ReadBytes(0x400);
            K051960_dx = reader.ReadInt32();
            K051960_dy = reader.ReadInt32();
            K051960_irq_enabled = reader.ReadInt32();
            K051960_nmi_enabled = reader.ReadInt32();
        }
        public static void K05324x_set_z_rejection(int zcode)
        {
            K05324x_z_rejection = zcode;
        }
        public static void K053245_vh_start(K053245_delegate _K053245_callback)
        {
            int i;
            Drawgfx.gfx_drawmode_table[0] = 0;
            for (i = 1; i < 15; i++)
            {
                Drawgfx.gfx_drawmode_table[i] = 1;
            }
            Drawgfx.gfx_drawmode_table[15] = 2;
            K05324x_z_rejection = -1;
            K053245_callback = _K053245_callback;
            K053244_rombank[0] = 0;
            K053245_ramsize[0] = 0x800;
            K053245_ram[0] = new byte[K053245_ramsize[0]];
            K053245_dx[0] = K053245_dy[0] = 0;
            K053245_buffer[0] = new ushort[K053245_ramsize[0] / 2];
            for (i = 0; i < K053245_ramsize[0]; i++)
            {
                K053245_ram[0][i] = 0;
            }
            for (i = 0; i < K053245_ramsize[0] / 2; i++)
            {
                K053245_buffer[0][i] = 0;
            }
        }
        public static ushort K053245_word_r(int offset)
        {
            return (ushort)(K053245_ram[0][offset * 2] * 0x100 + K053245_ram[0][offset * 2 + 1]);
        }
        public static void K053245_word_w(int offset, ushort data)
        {
            K053245_ram[0][offset * 2] = (byte)(data >> 8);
            K053245_ram[0][offset * 2 + 1] = (byte)data;
        }
        public static void K053245_word_w2(int offset, ushort data)
        {
            K053245_ram[0][offset * 2 + 1] = (byte)data;
        }
        public static void K053245_clear_buffer(int chip)
        {
            int i, e;
            for (e = K053245_ramsize[chip] / 2, i = 0; i < e; i += 8)
            {
                K053245_buffer[chip][i] = 0;
            }
        }
        public static void K053245_update_buffer(int chip)
        {
            int i;
            for (i = 0; i < K053245_ramsize[chip] / 2; i++)
            {
                K053245_buffer[chip][i] = (ushort)(K053245_ram[chip][i * 2] * 0x100 + K053245_ram[chip][i * 2 + 1]);
            }
        }
        public static byte K053244_chip_r(int chip, int offset)
        {
            if ((K053244_regs[chip][5] & 0x10) != 0 && offset >= 0x0c && offset < 0x10)
            {
                int addr;
                addr = (K053244_rombank[chip] << 19) | ((K053244_regs[chip][11] & 0x7) << 18) | (K053244_regs[chip][8] << 10) | (K053244_regs[chip][9] << 2) | ((offset & 3) ^ 1);
                addr &= K053245_memory_region[chip].Length - 1;
                return K053245_memory_region[chip][addr];
            }
            else if (offset == 0x06)
            {
                K053245_update_buffer(chip);
                return 0;
            }
            else
            {
                return 0;
            }
        }
        public static byte K053244_r(int offset)
        {
            return K053244_chip_r(0, offset);
        }
        public static void K053244_chip_w(int chip, int offset, byte data)
        {
            K053244_regs[chip][offset] = data;
            switch (offset)
            {
                case 0x05:
                    {
                        break;
                    }
                case 0x06:
                    K053245_update_buffer(chip);
                    break;
            }
        }
        public static void K053244_w(int offset, byte data)
        {
            K053244_chip_w(0, offset, data);
        }
        public static ushort K053244_lsb_r(int offset)
        {
            return (ushort)K053244_r(offset);
        }
        public static void K053244_lsb_w(int offset, ushort data)
        {
            //if (ACCESSING_BITS_0_7)
            K053244_w(offset, (byte)(data & 0xff));
        }
        public static void K053244_lsb_w2(int offset, byte data)
        {
            K053244_w(offset, data);
        }
        public static void K053244_bankselect(int chip, int bank)
        {
            K053244_rombank[chip] = bank;
        }
        public static void K053245_sprites_draw(RECT cliprect)
        {
            int offs, pri_code, i;
            int[] sortedlist = new int[128];
            int flipscreenX, flipscreenY, spriteoffsX, spriteoffsY;
            flipscreenX = K053244_regs[0][5] & 0x01;
            flipscreenY = K053244_regs[0][5] & 0x02;
            spriteoffsX = (K053244_regs[0][0] << 8) | K053244_regs[0][1];
            spriteoffsY = (K053244_regs[0][2] << 8) | K053244_regs[0][3];
            for (offs = 0; offs < 128; offs++)
            {
                sortedlist[offs] = -1;
            }
            i = K053245_ramsize[0] / 2;
            for (offs = 0; offs < i; offs += 8)
            {
                pri_code = K053245_buffer[0][offs];
                if ((pri_code & 0x8000) != 0)
                {
                    pri_code &= 0x007f;
                    if (offs != 0 && pri_code == K05324x_z_rejection)
                    {
                        continue;
                    }
                    if (sortedlist[pri_code] == -1)
                    {
                        sortedlist[pri_code] = offs;
                    }
                }
            }
            for (pri_code = 127; pri_code >= 0; pri_code--)
            {
                int ox, oy, color, color2, code, code2, size, w, h, x, y, flipx, flipy, mirrorx, mirrory, shadow, zoomx, zoomy, pri, pri2;
                offs = sortedlist[pri_code];
                if (offs == -1)
                {
                    continue;
                }
                code = K053245_buffer[0][offs + 1];
                code = ((code & 0xffe1) + ((code & 0x0010) >> 2) + ((code & 0x0008) << 1) + ((code & 0x0004) >> 1) + ((code & 0x0002) << 2));
                color = K053245_buffer[0][offs + 6] & 0x00ff;
                pri = 0;
                K053245_callback(code, color, out code2, out color2, out pri2);
                size = (K053245_buffer[0][offs] & 0x0f00) >> 8;
                w = 1 << (size & 0x03);
                h = 1 << ((size >> 2) & 0x03);
                zoomy = K053245_buffer[0][offs + 4];
                if (zoomy > 0x2000)
                {
                    continue;
                }
                if (zoomy != 0)
                {
                    zoomy = (0x400000 + zoomy / 2) / zoomy;
                }
                else
                {
                    zoomy = 2 * 0x400000;
                }
                if ((K053245_buffer[0][offs] & 0x4000) == 0)
                {
                    zoomx = K053245_buffer[0][offs + 5];
                    if (zoomx > 0x2000)
                    {
                        continue;
                    }
                    if (zoomx != 0)
                    {
                        zoomx = (0x400000 + zoomx / 2) / zoomx;
                    }
                    else
                    {
                        zoomx = 2 * 0x400000;
                    }
                }
                else
                {
                    zoomx = zoomy;
                }
                ox = K053245_buffer[0][offs + 3] + spriteoffsX;
                oy = K053245_buffer[0][offs + 2];
                ox += K053245_dx[0];
                oy += K053245_dy[0];
                flipx = K053245_buffer[0][offs] & 0x1000;
                flipy = K053245_buffer[0][offs] & 0x2000;
                mirrorx = K053245_buffer[0][offs + 6] & 0x0100;
                if (mirrorx != 0)
                {
                    flipx = 0;
                }
                mirrory = K053245_buffer[0][offs + 6] & 0x0200;
                shadow = K053245_buffer[0][offs + 6] & 0x0080;
                if (flipscreenX != 0)
                {
                    ox = 512 - ox;
                    if (mirrorx == 0)
                    {
                        flipx = (flipx == 0) ? 1 : 0;
                    }
                }
                if (flipscreenY != 0)
                {
                    oy = -oy;
                    if (mirrory == 0)
                    {
                        flipy = (flipy == 0) ? 1 : 0;
                    }
                }
                ox = (ox + 0x5d) & 0x3ff;
                if (ox >= 768)
                {
                    ox -= 1024;
                }
                oy = (-(oy + spriteoffsY + 0x07)) & 0x3ff;
                if (oy >= 640)
                {
                    oy -= 1024;
                }
                ox -= (zoomx * w) >> 13;
                oy -= (zoomy * h) >> 13;
                for (y = 0; y < h; y++)
                {
                    int sx, sy, zw, zh;
                    sy = oy + ((zoomy * y + (1 << 11)) >> 12);
                    zh = (oy + ((zoomy * (y + 1) + (1 << 11)) >> 12)) - sy;
                    for (x = 0; x < w; x++)
                    {
                        int c, fx, fy;
                        sx = ox + ((zoomx * x + (1 << 11)) >> 12);
                        zw = (ox + ((zoomx * (x + 1) + (1 << 11)) >> 12)) - sx;
                        c = code2;
                        if (mirrorx != 0)
                        {
                            if ((flipx == 0) ^ (2 * x < w))
                            {
                                c += (w - x - 1);
                                fx = 1;
                            }
                            else
                            {
                                c += x;
                                fx = 0;
                            }
                        }
                        else
                        {
                            if (flipx != 0)
                            {
                                c += w - 1 - x;
                            }
                            else
                            {
                                c += x;
                            }
                            fx = flipx;
                        }
                        if (mirrory != 0)
                        {
                            if ((flipy == 0) ^ (2 * y >= h))
                            {
                                c += 8 * (h - y - 1);
                                fy = 1;
                            }
                            else
                            {
                                c += 8 * y;
                                fy = 0;
                            }
                        }
                        else
                        {
                            if (flipy != 0)
                            {
                                c += 8 * (h - 1 - y);
                            }
                            else
                            {
                                c += 8 * y;
                            }
                            fy = flipy;
                        }
                        c = (c & 0x3f) | (code2 & ~0x3f);
                        if (zoomx == 0x10000 && zoomy == 0x10000)
                        {
                            Drawgfx.common_drawgfx_konami(gfx2rom, c, color2, fx, fy, sx, sy, cliprect, shadow, (uint)(pri2 | (1 << 31)));
                        }
                        else
                        {
                            Drawgfx.common_drawgfxzoom_konami(gfx2rom, c, color2, fx, fy, sx, sy, cliprect, shadow, 0, (zw << 16) / 16, (zh << 16) / 16, (uint)(pri2 | 1 << 31));
                        }
                    }
                }
            }
        }
        public static void SaveStateBinary_K053245(BinaryWriter writer)
        {
            int i;
            writer.Write(K05324x_z_rejection);
            writer.Write(K053245_ram[0], 0, 0x800);
            for (i = 0; i < 0x400; i++)
            {
                writer.Write(K053245_buffer[0][i]);
            }
            for (i = 0; i < 2; i++)
            {
                writer.Write(K053244_rombank[i]);
            }
            for (i = 0; i < 2; i++)
            {
                writer.Write(K053245_dx[i]);
            }
            for (i = 0; i < 2; i++)
            {
                writer.Write(K053245_dy[i]);
            }
            writer.Write(K053244_regs[0], 0, 0x10);
            writer.Write(K054000_ram, 0, 0x20);
        }
        public static void LoadStateBinary_K053245(BinaryReader reader)
        {
            int i;
            K05324x_z_rejection = reader.ReadInt32();
            K053245_ram[0] = reader.ReadBytes(0x800);
            for (i = 0; i < 0x400; i++)
            {
                K053245_buffer[0][i] = reader.ReadUInt16();
            }
            for (i = 0; i < 2; i++)
            {
                K053244_rombank[i] = reader.ReadInt32();
            }
            for (i = 0; i < 2; i++)
            {
                K053245_dx[i] = reader.ReadInt32();
            }
            for (i = 0; i < 2; i++)
            {
                K053245_dy[i] = reader.ReadInt32();
            }
            K053244_regs[0] = reader.ReadBytes(0x10);
            K054000_ram = reader.ReadBytes(0x20);
        }
        public static int K053246_read_register(int regnum)
        {
            return K053246_regs[regnum];
        }
        public static int K053247_read_register(int regnum)
        {
            return K053247_regs[regnum];
        }
        public static void K053247_vh_start(int dx, int dy, int plane_order, K053247_delegate _K053247_callback)
        {
            int gfx_index, i;
            //gfx_drawmode_table[0] = DRAWMODE_NONE;
            //for (i = 1;i < 15;i++)
            //	gfx_drawmode_table[i] = DRAWMODE_SOURCE;
            //gfx_drawmode_table[15] = DRAWMODE_SHADOW;
            K053247_dx = dx;
            K053247_dy = dy;
            K053247_wraparound = 1;
            K05324x_z_rejection = -1;
            //K053247_memory_region = gfx_memory_region;
            //K053247_gfx = Machine.gfx[0];
            K053247_callback = _K053247_callback;
            K053246_OBJCHA_line = 0;
            K053247_ram = new ushort[0x800];
            K053246_regs = new byte[8];
            K053247_regs = new ushort[16];
            Array.Clear(K053247_ram, 0, 0x800);
            Array.Clear(K053246_regs, 0, 8);
            Array.Clear(K053247_regs, 0, 16);
        }
        public static void K055673_vh_start(int layout, int dx, int dy, K053247_delegate _K053247_callback)
        {
            int gfx_index;
            //UINT8 *s1, *s2, *d;
            long i, c;
            //UINT16 *K055673_rom;
            int size4;
            //K055673_rom = (UINT16 *)memory_region(machine, gfx_memory_region);
            /*switch(layout)
            {
            case K055673_LAYOUT_GX:
                size4 = (memory_region_length(machine, gfx_memory_region)/(1024*1024))/5;
                size4 *= 4*1024*1024;
                K055673_rom = auto_malloc(size4 * 5);
                d = (UINT8 *)K055673_rom;
                s1 = memory_region(machine, gfx_memory_region);
                s2 = s1 + (size4);	 // 1bpp area
                for (i = 0; i < size4; i+= 4)
                {
                    *d++ = *s1++;
                    *d++ = *s1++;
                    *d++ = *s1++;
                    *d++ = *s1++;
                    *d++ = *s2++;
                }

                total = size4 / 128;
                decode_gfx(machine, gfx_index, (UINT8 *)K055673_rom, total, &spritelayout, 4);
                break;

            case K055673_LAYOUT_RNG:
                total = memory_region_length(machine, gfx_memory_region) / (16*16/2);
                decode_gfx(machine, gfx_index, (UINT8 *)K055673_rom, total, &spritelayout2, 4);
                break;

            case K055673_LAYOUT_LE2:
                total = memory_region_length(machine, gfx_memory_region) / (16*16);
                decode_gfx(machine, gfx_index, (UINT8 *)K055673_rom, total, &spritelayout3, 4);
                break;

            case K055673_LAYOUT_GX6:
                total = memory_region_length(machine, gfx_memory_region) / (16*16*6/8);
                decode_gfx(machine, gfx_index, (UINT8 *)K055673_rom, total, &spritelayout4, 4);
                break;

            default:
                fatalerror("Unsupported layout");
            }


            c = machine->gfx[gfx_index]->color_granularity-1;
            gfx_drawmode_table[0] = DRAWMODE_NONE;
            for (i = 1;i < c;i++)
                gfx_drawmode_table[i] = DRAWMODE_SOURCE;
            gfx_drawmode_table[c] = DRAWMODE_SHADOW;*/

            K053247_dx = dx;
            K053247_dy = dy;
            K053247_wraparound = 1;
            K05324x_z_rejection = -1;
            //K053247_memory_region = gfx_memory_region;
            K053247_gfx = Machine.gfx[1];
            K053247_callback = _K053247_callback;
            K053246_OBJCHA_line = 0;
            K053247_ram = new ushort[0x800];
            K053246_regs = new byte[8];
            K053247_regs = new ushort[16];
        }
        public static void SaveStateBinary_K055673(BinaryWriter writer)
        {
            int i;
            writer.Write(K053247_dx);
            writer.Write(K053247_dy);
            writer.Write(K053247_wraparound);
            writer.Write(K05324x_z_rejection);
            writer.Write(K053246_OBJCHA_line);
            for (i = 0; i < 0x800; i++)
            {
                writer.Write(K053247_ram[i]);
            }
            writer.Write(K053246_regs, 0, 8);
            for (i = 0; i < 16; i++)
            {
                writer.Write(K053247_regs[i]);
            }
            writer.Write(K053246_OBJCHA_line);
        }
        public static void LoadStateBinary_K055673(BinaryReader reader)
        {
            int i;
            K053247_dx = reader.ReadInt32();
            K053247_dy = reader.ReadInt32();
            K053247_wraparound = reader.ReadInt32();
            K05324x_z_rejection = reader.ReadInt32();
            K053246_OBJCHA_line = reader.ReadByte();
            for (i = 0; i < 0x800; i++)
            {
                K053247_ram[i] = reader.ReadUInt16();
            }
            K053246_regs = reader.ReadBytes(8);
            for (i = 0; i < 16; i++)
            {
                K053247_regs[i] = reader.ReadUInt16();
            }
            K053246_OBJCHA_line = reader.ReadByte();
        }
        public static void K053247_reg_word_w(int offset, ushort data)
        {
            K053247_regs[offset] = data;
        }
        public static void K053247_reg_word_w1(int offset, byte data)
        {
            K053247_regs[offset] = (ushort)((data << 8) | (K053247_regs[offset] & 0xff));
        }
        public static void K053247_reg_word_w2(int offset, byte data)
        {
            K053247_regs[offset] = (ushort)((K053247_regs[offset] & 0xff00) | data);
        }
        public static ushort K053247_word_r(int offset)
        {
            return K053247_ram[offset];
        }
        public static void K053247_word_w(int offset, ushort data)
        {
            K053247_ram[offset] = data;
        }
        public static void K053247_word_w1(int offset, byte data)
        {
            K053247_ram[offset] = (ushort)((data << 8) | (K053247_ram[offset] & 0xff));
        }
        public static void K053247_word_w2(int offset, byte data)
        {
            K053247_ram[offset] = (ushort)((K053247_ram[offset] & 0xff00) | data);
        }
        public static ushort K055673_rom_word_r(int offset)
        {
            /*UINT8 *ROM8 = (UINT8 *)memory_region(machine, K053247_memory_region);
            UINT16 *ROM = (UINT16 *)memory_region(machine, K053247_memory_region);
            int size4 = (memory_region_length(machine, K053247_memory_region)/(1024*1024))/5;
            int romofs;

            size4 *= 4*1024*1024;	// get offset to 5th bit
            ROM8 += size4;
            romofs = K053246_regs[6]<<16 | K053246_regs[7]<<8 | K053246_regs[4];

            switch (offset)
            {
                case 0:	// 20k / 36u
                    return ROM[romofs+2];
                    break;
                case 1:	// 17k / 36y
                    return ROM[romofs+3];
                    break;
                case 2: // 10k / 32y
                case 3:
                    romofs /= 2;
                        return ROM8[romofs+1];
                    break;
                case 4:	// 22k / 34u
                    return ROM[romofs];
                    break;
                case 5:	// 19k / 34y
                    return ROM[romofs+1];
                    break;
                case 6:	// 12k / 29y
                case 7:
                    romofs /= 2;
                        return ROM8[romofs];
                    break;
                default:
                    break;
            }*/
            return 0;
        }
        public static byte K053246_r(int offset)
        {
            if (K053246_OBJCHA_line == 1)
            {
                int addr;
                addr = (K053246_regs[6] << 17) | (K053246_regs[7] << 9) | (K053246_regs[4] << 1) | ((offset & 1) ^ 1);
                //addr &= memory_region_length(machine, K053247_memory_region)-1;
                return 0;//memory_region(machine, K053247_memory_region)[addr];
            }
            else
            {
                return 0;
            }
        }
        public static void K053246_w(int offset, byte data)
        {
            K053246_regs[offset] = data;
        }
        public static ushort K053246_word_r(int offset)
        {
            return (ushort)(K053246_r(offset * 2 + 1) | (K053246_r(offset * 2) << 8));
        }

        public static void K053246_word_w(int offset, ushort data)
        {
            K053246_w(offset * 2, (byte)(data >> 8));
            K053246_w(offset * 2 + 1, (byte)(data & 0xff));
        }
        public static void K053246_set_OBJCHA_line(int state)
        {
            K053246_OBJCHA_line = (byte)state;
        }
        public static int K053246_is_IRQ_enabled()
        {
            return K053246_regs[5] & 0x10;
        }


        public static void K053936_zoom_draw(int chip, ushort[] ctrl, ushort[] linectrl, RECT cliprect, Tmap tmap, int flags, uint priority)
        {
            if ((ctrl[0x07] & 0x0040) != 0)
            {
                uint startx, starty;
                int incxx, incxy;
                RECT my_clip;
                int y, maxy;
                if (((ctrl[0x07] & 0x0002) != 0) && (ctrl[0x09] != 0))
                {
                    my_clip.min_x = ctrl[0x08] + K053936_offset[chip][0] + 2;
                    my_clip.max_x = ctrl[0x09] + K053936_offset[chip][0] + 2 - 1;
                    if (my_clip.min_x < cliprect.min_x)
                    {
                        my_clip.min_x = cliprect.min_x;
                    }
                    if (my_clip.max_x > cliprect.max_x)
                    {
                        my_clip.max_x = cliprect.max_x;
                    }
                    y = ctrl[0x0a] + K053936_offset[chip][1] - 2;
                    if (y < cliprect.min_y)
                    {
                        y = cliprect.min_y;
                    }
                    maxy = ctrl[0x0b] + K053936_offset[chip][1] - 2 - 1;
                    if (maxy > cliprect.max_y)
                    {
                        maxy = cliprect.max_y;
                    }
                }
                else
                {
                    my_clip.min_x = cliprect.min_x;
                    my_clip.max_x = cliprect.max_x;
                    y = cliprect.min_y;
                    maxy = cliprect.max_y;
                }
                while (y <= maxy)
                {
                    //UINT16 *lineaddr = linectrl + 4*((y - K053936_offset[chip][1]) & 0x1ff);
                    int lineaddr_offset = 4 * ((y - K053936_offset[chip][1]) & 0x1ff);
                    my_clip.min_y = my_clip.max_y = y;
                    startx = (uint)(256 * (short)(linectrl[lineaddr_offset] + ctrl[0x00]));
                    starty = (uint)(256 * (short)(linectrl[lineaddr_offset + 1] + ctrl[0x01]));
                    incxx = (short)(linectrl[lineaddr_offset + 2]);
                    incxy = (short)(linectrl[lineaddr_offset + 3]);
                    if ((ctrl[0x06] & 0x8000) != 0)
                    {
                        incxx *= 256;
                    }
                    if ((ctrl[0x06] & 0x0080) != 0)
                    {
                        incxy *= 256;
                    }
                    startx -= (uint)(K053936_offset[chip][0] * incxx);
                    starty -= (uint)(K053936_offset[chip][0] * incxy);
                    tmap.tilemap_draw_roz_primask(my_clip, (uint)(startx << 5), (uint)(starty << 5), incxx << 5, incxy << 5, 0, 0, K053936_wraparound[chip], flags, (byte)priority, 0xff);
                    y++;
                }
            }
            else
            {
                uint startx, starty;
                int incxx, incxy, incyx, incyy;
                startx = (uint)(256 * (short)(ctrl[0x00]));
                starty = (uint)(256 * (short)(ctrl[0x01]));
                incyx = (short)(ctrl[0x02]);
                incyy = (short)(ctrl[0x03]);
                incxx = (short)(ctrl[0x04]);
                incxy = (short)(ctrl[0x05]);
                if ((ctrl[0x06] & 0x4000) != 0)
                {
                    incyx *= 256; incyy *= 256;
                }
                if ((ctrl[0x06] & 0x0040) != 0)
                {
                    incxx *= 256; incxy *= 256;
                }
                startx -= (uint)(K053936_offset[chip][1] * incyx);
                starty -= (uint)(K053936_offset[chip][1] * incyy);
                startx -= (uint)(K053936_offset[chip][0] * incxx);
                starty -= (uint)(K053936_offset[chip][0] * incxy);
                tmap.tilemap_draw_roz_primask(cliprect, (uint)(startx << 5), (uint)(starty << 5), incxx << 5, incxy << 5, 0, 0, K053936_wraparound[chip], flags, (byte)priority, 0xff);
            }
        }
        public static void K053936_0_zoom_draw(RECT cliprect, Tmap tmap, int flags, uint priority)
        {
            K053936_zoom_draw(0, K053936_0_ctrl, K053936_0_linectrl, cliprect, tmap, flags, priority);
        }
        public static void K053936_wraparound_enable(int chip, int status)
        {
            K053936_wraparound[chip] = status;
        }
        public static void K053936_set_offset(int chip, int xoffs, int yoffs)
        {
            K053936_offset[chip][0] = xoffs;
            K053936_offset[chip][1] = yoffs;
        }
        public static void SaveStateBinary_K053936(BinaryWriter writer)
        {
            int i, j;
            for (i = 0; i < 0x10; i++)
            {
                writer.Write(K053936_0_ctrl[i]);
            }
            for (i = 0; i < 0x800; i++)
            {
                writer.Write(K053936_0_linectrl[i]);
            }
            for (i = 0; i < 2; i++)
            {
                for (j = 0; j < 2; j++)
                {
                    writer.Write(K053936_offset[i][j]);
                }
            }
            for (i = 0; i < 2; i++)
            {
                writer.Write(K053936_wraparound[i]);
            }
        }
        public static void LoadStateBinary_K053936(BinaryReader reader)
        {
            int i, j;
            for (i = 0; i < 0x10; i++)
            {
                K053936_0_ctrl[i] = reader.ReadUInt16();
            }
            for (i = 0; i < 0x800; i++)
            {
                K053936_0_linectrl[i] = reader.ReadUInt16();
            }
            for (i = 0; i < 2; i++)
            {
                for (j = 0; j < 2; j++)
                {
                    K053936_offset[i][j] = reader.ReadInt32();
                }
            }
            for (i = 0; i < 2; i++)
            {
                K053936_wraparound[i] = reader.ReadInt32();
            }
        }
        public static void K053251_vh_start()
        {
            K053251_tilemaps = new Tmap[5];
            K053251_set_tilemaps(null, null, null, null, null);
        }
        public static void K053251_set_tilemaps(Tmap ci0, Tmap ci1, Tmap ci2, Tmap ci3, Tmap ci4)
        {
            K053251_tilemaps[0] = ci0;
            K053251_tilemaps[1] = ci1;
            K053251_tilemaps[2] = ci2;
            K053251_tilemaps[3] = ci3;
            K053251_tilemaps[4] = ci4;
            if (ci0 == null && ci1 == null && ci2 == null && ci3 == null && ci4 == null)
            {
                K053251_tilemaps_set = 0;
            }
            else
            {
                K053251_tilemaps_set = 1;
            }
        }
        public static void K053251_w(int offset, byte data)
        {
            int i, newind;
            data &= 0x3f;
            if (K053251_ram[offset] != data)
            {
                K053251_ram[offset] = data;
                if (offset == 9)
                {
                    for (i = 0; i < 3; i++)
                    {
                        newind = 32 * ((data >> 2 * i) & 0x03);
                        if (K053251_palette_index[i] != newind)
                        {
                            K053251_palette_index[i] = newind;
                            if (K053251_tilemaps[i] != null)
                            {
                                K053251_tilemaps[i].all_tiles_dirty = true;
                            }
                        }
                    }
                    if (K053251_tilemaps_set == 0)
                    {
                        for (i = 0; i < 3; i++)
                        {
                            K052109_tilemap[i].all_tiles_dirty = true;
                        }
                    }
                }
                else if (offset == 10)
                {
                    for (i = 0; i < 2; i++)
                    {
                        newind = 16 * ((data >> 3 * i) & 0x07);
                        if (K053251_palette_index[3 + i] != newind)
                        {
                            K053251_palette_index[3 + i] = newind;
                            if (K053251_tilemaps[3 + i] != null)
                            {
                                K053251_tilemaps[3 + i].all_tiles_dirty = true;
                            }
                        }
                    }
                    if (K053251_tilemaps_set == 0)
                    {
                        for (i = 0; i < 3; i++)
                        {
                            K052109_tilemap[i].all_tiles_dirty = true;
                        }
                    }
                }
            }
        }
        public static void K053251_lsb_w(int offset, ushort data)
        {
            //if (ACCESSING_BITS_0_7)
            K053251_w(offset, (byte)(data & 0xff));
        }
        public static void K053251_lsb_w2(int offset, byte data)
        {
            K053251_w(offset, data);
        }
        public static void K053251_msb_w(int offset, ushort data)
        {
            //if (ACCESSING_BITS_8_15)
            K053251_w(offset, (byte)((data >> 8) & 0xff));
        }
        public static void K053251_msb_w1(int offset, byte data)
        {
            K053251_w(offset, data);
        }
        public static int K053251_get_priority(int ci)
        {
            return K053251_ram[ci];
        }
        public static int K053251_get_palette_index(int ci)
        {
            return K053251_palette_index[ci];
        }
        public static void SaveStateBinary_K053251(BinaryWriter writer)
        {
            int i;
            writer.Write(K053251_ram);
            for (i = 0; i < 5; i++)
            {
                writer.Write(K053251_palette_index[i]);
            }
            writer.Write(K053251_tilemaps_set);
        }
        public static void LoadStateBinary_K053251(BinaryReader reader)
        {
            int i;
            K053251_ram = reader.ReadBytes(0x10);
            for (i = 0; i < 5; i++)
            {
                K053251_palette_index[i] = reader.ReadInt32();
            }
            K053251_tilemaps_set = reader.ReadInt32();
        }
        public static void LoadStateBinary_K053251_2(BinaryReader reader)
        {
            int i;
            reader.ReadBytes(0x10);
            for (i = 0; i < 5; i++)
            {
                reader.ReadInt32();
            }
            reader.ReadInt32();
        }
        public static void K054000_w(int offset, byte data)
        {
            K054000_ram[offset] = data;
        }
        public static byte K054000_r(int offset)
        {
            int Acx, Acy, Aax, Aay;
            int Bcx, Bcy, Bax, Bay;
            if (offset != 0x18)
            {
                return 0;
            }
            Acx = (K054000_ram[0x01] << 16) | (K054000_ram[0x02] << 8) | K054000_ram[0x03];
            Acy = (K054000_ram[0x09] << 16) | (K054000_ram[0x0a] << 8) | K054000_ram[0x0b];
            if (K054000_ram[0x04] == 0xff)
            {
                Acx += 3;
            }
            if (K054000_ram[0x0c] == 0xff)
            {
                Acy += 3;
            }
            Aax = K054000_ram[0x06] + 1;
            Aay = K054000_ram[0x07] + 1;
            Bcx = (K054000_ram[0x15] << 16) | (K054000_ram[0x16] << 8) | K054000_ram[0x17];
            Bcy = (K054000_ram[0x11] << 16) | (K054000_ram[0x12] << 8) | K054000_ram[0x13];
            Bax = K054000_ram[0x0e] + 1;
            Bay = K054000_ram[0x0f] + 1;
            if (Acx + Aax < Bcx - Bax)
            {
                return 1;
            }
            if (Bcx + Bax < Acx - Aax)
            {
                return 1;
            }
            if (Acy + Aay < Bcy - Bay)
            {
                return 1;
            }
            if (Bcy + Bay < Acy - Aay)
            {
                return 1;
            }
            return 0;
        }
        public static ushort K054000_lsb_r(int offset)
        {
            return K054000_r(offset);
        }
        public static void K054000_lsb_w(int offset, ushort data)
        {
            //if (ACCESSING_BITS_0_7)
            K054000_w(offset, (byte)(data & 0xff));
        }
        public static void K054000_lsb_w2(int offset, byte data)
        {
            K054000_w(offset, data);
        }

        public static void K056832_mark_line_dirty(int P, int L)
        {
            if (L < 0x100)
            {
                K056832_LineDirty[P][L >> 5] |= (uint)(1 << (L & 0x1f));
            }
        }
        public static void K056832_mark_all_lines_dirty(int P)
        {
            K056832_AllLinesDirty[P] = 1;
        }
        public static void K056832_mark_page_dirty(int page)
        {
            if (K056832_PageTileMode[page] != 0)
            {
                K056832_tilemap[page].all_tiles_dirty = true;
            }
            else
            {
                K056832_mark_all_lines_dirty(page);
            }
        }
        public static void K056832_mark_plane_dirty(int layer)
        {
            byte tilemode;
            int i;
            tilemode = K056832_LayerTileMode[layer];
            for (i = 0; i < 16; i++)
            {
                if (K056832_LayerAssociatedWithPage[i] == layer)
                {
                    K056832_PageTileMode[i] = tilemode;
                    K056832_mark_page_dirty(i);
                }
            }
        }
        public static void K056832_MarkAllTilemapsDirty()
        {
            int i;

            for (i = 0; i < 16; i++)
            {
                if (K056832_LayerAssociatedWithPage[i] != -1)
                {
                    K056832_PageTileMode[i] = K056832_LayerTileMode[K056832_LayerAssociatedWithPage[i]];
                    K056832_mark_page_dirty(i);
                }
            }
        }
        public static void K056832_UpdatePageLayout()
        {
            int layer, rowstart, rowspan, colstart, colspan, r, c, pageIndex, setlayer;
            K056832_LayerAssociation = K056832_DefaultLayerAssociation;
            for (layer = 0; layer < 4; layer++)
            {
                if (K056832_Y[layer] == 0 && K056832_X[layer] == 0 && K056832_H[layer] == 3 && K056832_W[layer] == 3)
                {
                    K056832_LayerAssociation = 0;
                    break;
                }
            }
            for (pageIndex = 0; pageIndex < 16; pageIndex++)
            {
                K056832_LayerAssociatedWithPage[pageIndex] = -1;
            }
            for (layer = 0; layer < 4; layer++)
            {
                rowstart = K056832_Y[layer];
                colstart = K056832_X[layer];
                rowspan = K056832_H[layer] + 1;
                colspan = K056832_W[layer] + 1;
                setlayer = (K056832_LayerAssociation != 0) ? layer : K056832_ActiveLayer;
                for (r = 0; r < rowspan; r++)
                {
                    for (c = 0; c < colspan; c++)
                    {
                        pageIndex = (((rowstart + r) & 3) << 2) + ((colstart + c) & 3);
                        if (K056832_djmain_hack == 0 || K056832_LayerAssociatedWithPage[pageIndex] == -1)
                        {
                            K056832_LayerAssociatedWithPage[pageIndex] = setlayer;
                        }
                    }
                }
            }
            K056832_MarkAllTilemapsDirty();
        }
        public static void K056832_change_rambank()
        {
            int bank = K056832_regs[0x19];
            if ((K056832_regs[0] & 0x02) != 0)
            {
                K056832_SelectedPage = 16;
            }
            else
            {
                K056832_SelectedPage = ((bank >> 1) & 0xc) | (bank & 3);
            }
            K056832_SelectedPagex4096 = K056832_SelectedPage << 12;
            K056832_MarkAllTilemapsDirty();
        }
        public static void K056832_change_rombank()
        {
            int bank;
            if (K056832_uses_tile_banks != 0)
            {
                bank = (K056832_regs[0x1a] >> 8) | (K056832_regs[0x1b] << 4) | (K056832_cur_tile_bank << 6);
            }
            else
            {
                bank = K056832_regs[0x1a] | (K056832_regs[0x1b] << 16);
            }
            K056832_CurGfxBank = bank % K056832_NumGfxBanks;
        }


        public static void K056832_vh_start(K056832_delegate _K056832_callback, int djmain_hack)
        {
            int i, j;
            K053252_regs = new ushort[16];
            K056832_shiftmasks = new int[4][]{
                new int[]{6,0x3f,0,0x00},
                new int[]{4,0x0f,2,0x30},
                new int[]{2,0x03,2,0x3c},
                new int[]{0,0x00,2,0x3f}
            };
            K056832_LayerOffset = new int[8][];
            for (i = 0; i < 8; i++)
            {
                K056832_LayerOffset[i] = new int[2];
            }
            K056832_LSRAMPage = new int[8][];
            for (i = 0; i < 8; i++)
            {
                K056832_LSRAMPage[i] = new int[2];
            }

            K056832_X = new int[8];
            K056832_Y = new int[8];
            K056832_W = new int[8];
            K056832_H = new int[8];
            K056832_dx = new int[8];
            K056832_dy = new int[8];
            K056832_LayerTileMode = new byte[8];
            K056832_regs = new ushort[0x20];
            K056832_regsb = new ushort[4];
            K056832_LayerAssociatedWithPage = new int[16];
            K056832_callback = _K056832_callback;
            K056832_NumGfxBanks = gfx1rom.Length / 0x2000;
            K056832_CurGfxBank = 0;
            K056832_use_ext_linescroll = 0;
            K056832_uses_tile_banks = 0;
            K056832_djmain_hack = djmain_hack;
            for (i = 0; i < 4; i++)
            {
                K056832_LayerOffset[i][0] = 0;
                K056832_LayerOffset[i][1] = 0;
                K056832_LSRAMPage[i][0] = i;
                K056832_LSRAMPage[i][1] = i << 11;
                K056832_X[i] = 0;
                K056832_Y[i] = 0;
                K056832_W[i] = 0;
                K056832_H[i] = 0;
                K056832_dx[i] = 0;
                K056832_dy[i] = 0;
                K056832_LayerTileMode[i] = 1;
            }
            K056832_DefaultLayerAssociation = 1;
            K056832_ActiveLayer = 0;
            K056832_UpdateMode = 0;
            K056832_linemap_enabled = 0;
            K056832_LineDirty = new uint[16][];
            for (i = 0; i < 16; i++)
            {
                K056832_LineDirty[i] = new uint[8];
            }
            K056832_AllLinesDirty = new byte[16];
            K056832_PageTileMode = new byte[16];
            for (i = 0; i < 16; i++)
            {
                K056832_AllLinesDirty[i] = 0;
                K056832_PageTileMode[i] = 1;
            }
            K056832_videoram = new ushort[0x1000 * 17];
            K056832_tilemap = new Tmap[16];
            K056832_tilemap[0x0] = Tmap.tilemap_create(Tmap.tilemap_scan_rows, 8, 8, 64, 32);
            K056832_tilemap[0x1] = Tmap.tilemap_create(Tmap.tilemap_scan_rows, 8, 8, 64, 32);
            K056832_tilemap[0x2] = Tmap.tilemap_create(Tmap.tilemap_scan_rows, 8, 8, 64, 32);
            K056832_tilemap[0x3] = Tmap.tilemap_create(Tmap.tilemap_scan_rows, 8, 8, 64, 32);
            K056832_tilemap[0x4] = Tmap.tilemap_create(Tmap.tilemap_scan_rows, 8, 8, 64, 32);
            K056832_tilemap[0x5] = Tmap.tilemap_create(Tmap.tilemap_scan_rows, 8, 8, 64, 32);
            K056832_tilemap[0x6] = Tmap.tilemap_create(Tmap.tilemap_scan_rows, 8, 8, 64, 32);
            K056832_tilemap[0x7] = Tmap.tilemap_create(Tmap.tilemap_scan_rows, 8, 8, 64, 32);
            K056832_tilemap[0x8] = Tmap.tilemap_create(Tmap.tilemap_scan_rows, 8, 8, 64, 32);
            K056832_tilemap[0x9] = Tmap.tilemap_create(Tmap.tilemap_scan_rows, 8, 8, 64, 32);
            K056832_tilemap[0xa] = Tmap.tilemap_create(Tmap.tilemap_scan_rows, 8, 8, 64, 32);
            K056832_tilemap[0xb] = Tmap.tilemap_create(Tmap.tilemap_scan_rows, 8, 8, 64, 32);
            K056832_tilemap[0xc] = Tmap.tilemap_create(Tmap.tilemap_scan_rows, 8, 8, 64, 32);
            K056832_tilemap[0xd] = Tmap.tilemap_create(Tmap.tilemap_scan_rows, 8, 8, 64, 32);
            K056832_tilemap[0xe] = Tmap.tilemap_create(Tmap.tilemap_scan_rows, 8, 8, 64, 32);
            K056832_tilemap[0xf] = Tmap.tilemap_create(Tmap.tilemap_scan_rows, 8, 8, 64, 32);
            K056832_tilemap[0x0].tile_update3 = K056832_tilemap[0x0].tile_update_konami_k056832_0;
            K056832_tilemap[0x1].tile_update3 = K056832_tilemap[0x1].tile_update_konami_k056832_1;
            K056832_tilemap[0x2].tile_update3 = K056832_tilemap[0x2].tile_update_konami_k056832_2;
            K056832_tilemap[0x3].tile_update3 = K056832_tilemap[0x3].tile_update_konami_k056832_3;
            K056832_tilemap[0x4].tile_update3 = K056832_tilemap[0x4].tile_update_konami_k056832_4;
            K056832_tilemap[0x5].tile_update3 = K056832_tilemap[0x5].tile_update_konami_k056832_5;
            K056832_tilemap[0x6].tile_update3 = K056832_tilemap[0x6].tile_update_konami_k056832_6;
            K056832_tilemap[0x7].tile_update3 = K056832_tilemap[0x7].tile_update_konami_k056832_7;
            K056832_tilemap[0x8].tile_update3 = K056832_tilemap[0x8].tile_update_konami_k056832_8;
            K056832_tilemap[0x9].tile_update3 = K056832_tilemap[0x9].tile_update_konami_k056832_9;
            K056832_tilemap[0xa].tile_update3 = K056832_tilemap[0xa].tile_update_konami_k056832_a;
            K056832_tilemap[0xb].tile_update3 = K056832_tilemap[0xb].tile_update_konami_k056832_b;
            K056832_tilemap[0xc].tile_update3 = K056832_tilemap[0xc].tile_update_konami_k056832_c;
            K056832_tilemap[0xd].tile_update3 = K056832_tilemap[0xd].tile_update_konami_k056832_d;
            K056832_tilemap[0xe].tile_update3 = K056832_tilemap[0xe].tile_update_konami_k056832_e;
            K056832_tilemap[0xf].tile_update3 = K056832_tilemap[0xf].tile_update_konami_k056832_f;
            K056832_pixmap = new ushort[16][];
            for (i = 0; i < 16; i++)
            {
                K056832_tilemap[i].pen_to_flags = new byte[1, 32];
                K056832_tilemap[i].pen_to_flags[0, 0] = 0;
                for (j = 1; j < 32; j++)
                {
                    K056832_tilemap[i].pen_to_flags[0, j] = 0x10;
                }
                K056832_pixmap[i] = K056832_tilemap[i].tilemap_get_pixmap();
                K056832_tilemap[i].tilemap_draw_instance3 = K056832_tilemap[i].tilemap_draw_instance_konami_mystwarr;
            }            
            Array.Clear(K056832_videoram, 0, 0x10000);            
            K056832_UpdatePageLayout();
            K056832_change_rambank();
            K056832_change_rombank();
        }
        public static ushort K056832_rom_word_r(int offset)
        {
            int ofs16, ofs8;
            int ret;
            ofs16 = (offset / 8) * 5;
            ofs8 = (offset / 4) * 5;
            ofs16 += (K056832_CurGfxBank * 5 * 1024);
            ofs8 += (K056832_CurGfxBank * 10 * 1024);
            ret = (K056832_rom[ofs8 + 4] << 8);
            if ((offset % 8) >= 4)
            {
                ret |= (K056832_rom[ofs16 + 1] << 24) | (K056832_rom[ofs16 + 3] << 16);
            }
            else
            {
                ret |= (K056832_rom[ofs16] << 24) | (K056832_rom[ofs16 + 2] << 16);
            }
            return (ushort)ret;
        }
        public static ushort K056832_mw_rom_word_r(int offset)
        {
            int bank = 10240 * K056832_CurGfxBank;
            int addr;
            if ((K056832_regsb[2] & 0x8) != 0)
            {
                int bit;
                int res, temp;
                bit = offset % 4;
                addr = (offset / 4) * 5;
                temp = K056832_rom[addr + 4 + bank];
                switch (bit)
                {
                    default:
                    case 0:
                        res = (temp & 0x80) << 5;
                        res |= ((temp & 0x40) >> 2);
                        break;
                    case 1:
                        res = (temp & 0x20) << 7;
                        res |= (temp & 0x10);
                        break;
                    case 2:
                        res = (temp & 0x08) << 9;
                        res |= ((temp & 0x04) << 2);
                        break;
                    case 3:
                        res = (temp & 0x02) << 11;
                        res |= ((temp & 0x01) << 4);
                        break;
                }
                return (ushort)res;
            }
            else
            {
                addr = (offset >> 1) * 5;
                if ((offset & 1) != 0)
                {
                    addr += 2;
                }
                addr += bank;
                return (ushort)(K056832_rom[addr + 1] | (K056832_rom[addr] << 8));
            }
        }
        public static ushort K056832_ram_word_r(int offset)
        {
            K056832_rom_half = 0;
            return K056832_videoram[K056832_SelectedPagex4096 + offset];
        }
        public static void K056832_ram_word_w(int offset, ushort data)
        {
            ushort old_mask, old_data;
            ushort mem_mask = 0xffff;
            old_mask = (ushort)~mem_mask;
            old_data = K056832_videoram[K056832_SelectedPagex4096 + offset];
            data = (ushort)((data & mem_mask) | (old_data & old_mask));
            if (data != 0)
            {
                int i1 = 1;
            }
            if (data != old_data)
            {                
                K056832_videoram[K056832_SelectedPagex4096 + offset] = data;
                offset >>= 1;
                if (K056832_PageTileMode[K056832_SelectedPage] != 0)
                {
                    K056832_tilemap[K056832_SelectedPage].tilemap_mark_tile_dirty(offset);
                }
                else
                {
                    K056832_mark_line_dirty(K056832_SelectedPage, offset);
                }
            }
        }
        public static void K056832_ram_word_w1(int offset, byte data)
        {
            ushort old_data;
            old_data = K056832_videoram[K056832_SelectedPagex4096 + offset];
            if (data != 0)
            {
                int i1 = 1;
            }
            if (data != (old_data >> 8))
            {                
                K056832_videoram[K056832_SelectedPagex4096 + offset] = (ushort)((data << 8) | (old_data & 0xff));
                offset >>= 1;
                if (K056832_PageTileMode[K056832_SelectedPage] != 0)
                {
                    K056832_tilemap[K056832_SelectedPage].tilemap_mark_tile_dirty(offset);
                }
                else
                {
                    K056832_mark_line_dirty(K056832_SelectedPage, offset);
                }
            }
        }
        public static void K056832_ram_word_w2(int offset, byte data)
        {
            ushort old_data;
            old_data = K056832_videoram[K056832_SelectedPagex4096 + offset];
            if (data != 0)
            {
                int i1 = 1;
            }
            if (data != (old_data & 0xff))
            {                
                K056832_videoram[K056832_SelectedPagex4096 + offset] = (ushort)((old_data & 0xff00) | data);
                offset >>= 1;
                if (K056832_PageTileMode[K056832_SelectedPage] != 0)
                {
                    K056832_tilemap[K056832_SelectedPage].tilemap_mark_tile_dirty(offset);
                }
                else
                {
                    K056832_mark_line_dirty(K056832_SelectedPage, offset);
                }
            }
        }
        public static void K056832_word_w(int offset, ushort data)
        {
            int layer, flip, mask, i;
            uint old_data, new_data;
            old_data = K056832_regs[offset];
            K056832_regs[offset] = data;
            new_data = K056832_regs[offset];
            if (new_data != old_data)
            {
                switch (offset)
                {
                    case 0x00 / 2:
                        if ((new_data & 0x30) != (old_data & 0x30))
                        {
                            flip = 0;
                            if ((new_data & 0x20) != 0)
                            {
                                flip |= Tilemap.TILEMAP_FLIPY;
                            }
                            if ((new_data & 0x10) != 0)
                            {
                                flip |= Tilemap.TILEMAP_FLIPX;
                            }
                            for (i = 0; i < 16; i++)
                            {
                                Tmap.tilemap_set_flip(K056832_tilemap[i], (byte)flip);
                            }
                        }
                        if ((new_data & 0x02) != (old_data & 0x02))
                        {
                            K056832_change_rambank();
                        }
                        break;
                    case 0x08 / 2:
                        for (layer = 0; layer < 4; layer++)
                        {
                            mask = 1 << layer;
                            i = (int)(new_data & mask);
                            if (i != (old_data & mask))
                            {
                                K056832_LayerTileMode[layer] = (byte)i;
                                K056832_mark_plane_dirty(layer);
                            }
                        }
                        break;
                    case 0x32 / 2:
                        K056832_change_rambank();
                        break;
                    case 0x34 / 2:
                    case 0x36 / 2:
                        K056832_change_rombank();
                        break;
                    default:
                        layer = offset & 3;
                        if (offset >= 0x10 / 2 && offset <= 0x16 / 2)
                        {
                            K056832_Y[layer] = (int)((new_data & 0x18) >> 3);
                            K056832_H[layer] = (int)(new_data & 0x3);
                            K056832_ActiveLayer = layer;
                            K056832_UpdatePageLayout();
                        }
                        else if (offset >= 0x18 / 2 && offset <= 0x1e / 2)
                        {
                            K056832_X[layer] = (int)((new_data & 0x18) >> 3);
                            K056832_W[layer] = (int)(new_data & 0x03);
                            K056832_ActiveLayer = layer;
                            K056832_UpdatePageLayout();
                        }
                        else if (offset >= 0x20 / 2 && offset <= 0x26 / 2)
                        {
                            K056832_dy[layer] = (short)new_data;
                        }
                        else if (offset >= 0x28 / 2 && offset <= 0x2e / 2)
                        {
                            K056832_dx[layer] = (short)new_data;
                        }
                        break;
                }
            }
        }
        public static void K056832_word_w1(int offset, byte data)
        {
            int layer, flip, mask, i;
            uint old_data, new_data;
            old_data = K056832_regs[offset];
            K056832_regs[offset] = (ushort)((uint)(data << 8) | (old_data & 0xff));
            new_data = K056832_regs[offset];
            if (new_data != old_data)
            {
                switch (offset)
                {
                    case 0x00 / 2:
                        if ((new_data & 0x30) != (old_data & 0x30))
                        {
                            flip = 0;
                            if ((new_data & 0x20) != 0)
                            {
                                flip |= Tilemap.TILEMAP_FLIPY;
                            }
                            if ((new_data & 0x10) != 0)
                            {
                                flip |= Tilemap.TILEMAP_FLIPX;
                            }
                            for (i = 0; i < 16; i++)
                            {
                                Tmap.tilemap_set_flip(K056832_tilemap[i], (byte)flip);
                            }
                        }
                        if ((new_data & 0x02) != (old_data & 0x02))
                        {
                            K056832_change_rambank();
                        }
                        break;
                    case 0x08 / 2:
                        for (layer = 0; layer < 4; layer++)
                        {
                            mask = 1 << layer;
                            i = (int)(new_data & mask);
                            if (i != (old_data & mask))
                            {
                                K056832_LayerTileMode[layer] = (byte)i;
                                K056832_mark_plane_dirty(layer);
                            }
                        }
                        break;
                    case 0x32 / 2:
                        K056832_change_rambank();
                        break;
                    case 0x34 / 2:
                    case 0x36 / 2:
                        K056832_change_rombank();
                        break;
                    default:
                        layer = offset & 3;
                        if (offset >= 0x10 / 2 && offset <= 0x16 / 2)
                        {
                            K056832_Y[layer] = (int)((new_data & 0x18) >> 3);
                            K056832_H[layer] = (int)(new_data & 0x3);
                            K056832_ActiveLayer = layer;
                            K056832_UpdatePageLayout();
                        }
                        else if (offset >= 0x18 / 2 && offset <= 0x1e / 2)
                        {
                            K056832_X[layer] = (int)((new_data & 0x18) >> 3);
                            K056832_W[layer] = (int)(new_data & 0x03);
                            K056832_ActiveLayer = layer;
                            K056832_UpdatePageLayout();
                        }
                        else if (offset >= 0x20 / 2 && offset <= 0x26 / 2)
                        {
                            K056832_dy[layer] = (short)new_data;
                        }
                        else if (offset >= 0x28 / 2 && offset <= 0x2e / 2)
                        {
                            K056832_dx[layer] = (short)new_data;
                        }
                        break;
                }
            }
        }
        public static void K056832_word_w2(int offset, byte data)
        {
            int layer, flip, mask, i;
            uint old_data, new_data;
            old_data = K056832_regs[offset];
            K056832_regs[offset] = (ushort)((old_data & 0xff00) | data);
            new_data = K056832_regs[offset];
            if (new_data != old_data)
            {
                switch (offset)
                {
                    case 0x00 / 2:
                        if ((new_data & 0x30) != (old_data & 0x30))
                        {
                            flip = 0;
                            if ((new_data & 0x20) != 0)
                            {
                                flip |= Tilemap.TILEMAP_FLIPY;
                            }
                            if ((new_data & 0x10) != 0)
                            {
                                flip |= Tilemap.TILEMAP_FLIPX;
                            }
                            for (i = 0; i < 16; i++)
                            {
                                Tmap.tilemap_set_flip(K056832_tilemap[i], (byte)flip);
                            }
                        }
                        if ((new_data & 0x02) != (old_data & 0x02))
                        {
                            K056832_change_rambank();
                        }
                        break;
                    case 0x08 / 2:
                        for (layer = 0; layer < 4; layer++)
                        {
                            mask = 1 << layer;
                            i = (int)(new_data & mask);
                            if (i != (old_data & mask))
                            {
                                K056832_LayerTileMode[layer] = (byte)i;
                                K056832_mark_plane_dirty(layer);
                            }
                        }
                        break;
                    case 0x32 / 2:
                        K056832_change_rambank();
                        break;
                    case 0x34 / 2:
                    case 0x36 / 2:
                        K056832_change_rombank();
                        break;
                    default:
                        layer = offset & 3;
                        if (offset >= 0x10 / 2 && offset <= 0x16 / 2)
                        {
                            K056832_Y[layer] = (int)((new_data & 0x18) >> 3);
                            K056832_H[layer] = (int)(new_data & 0x3);
                            K056832_ActiveLayer = layer;
                            K056832_UpdatePageLayout();
                        }
                        else if (offset >= 0x18 / 2 && offset <= 0x1e / 2)
                        {
                            K056832_X[layer] = (int)((new_data & 0x18) >> 3);
                            K056832_W[layer] = (int)(new_data & 0x03);
                            K056832_ActiveLayer = layer;
                            K056832_UpdatePageLayout();
                        }
                        else if (offset >= 0x20 / 2 && offset <= 0x26 / 2)
                        {
                            K056832_dy[layer] = (short)new_data;
                        }
                        else if (offset >= 0x28 / 2 && offset <= 0x2e / 2)
                        {
                            K056832_dx[layer] = (short)new_data;
                        }
                        break;
                }
            }
        }
        public static void K056832_b_word_w(int offset, ushort data)
        {
            K056832_regsb[offset] = data;
        }
        public static void K056832_b_word_w1(int offset, byte data)
        {
            K056832_regsb[offset] = (ushort)((data<<8)|(K056832_regsb[offset]&0xff));
        }
        public static void K056832_b_word_w2(int offset, byte data)
        {
            K056832_regsb[offset] = (ushort)((K056832_regsb[offset] & 0xff00) | data);
        }


        public static int K056832_update_linemap(/*running_machine *machine, bitmap_t *bitmap,*/ int page, int flags)
        {
            if (K056832_PageTileMode[page] != 0)
            {
                return 0;
            }
            if (K056832_linemap_enabled == 0)
            {
                return 1;
            }
            RECT zerorect;
            uint[] dirty;
            int all_dirty;
            byte[,] xprmap;
            byte[] xprdata;
            xprmap = K056832_tilemap[page].tilemap_get_flagsmap();
            xprdata = K056832_tilemap[page].tilemap_get_tile_flags();
            dirty = K056832_LineDirty[page];
            all_dirty = K056832_AllLinesDirty[page];
            if (all_dirty != 0)
            {
                dirty[7] = dirty[6] = dirty[5] = dirty[4] = dirty[3] = dirty[2] = dirty[1] = dirty[0] = 0;
                K056832_AllLinesDirty[page] = 0;
                zerorect.min_x = zerorect.max_x = zerorect.min_y = zerorect.max_y = 0;
                K056832_tilemap[page].tilemap_draw_primask(zerorect, 0x10, 0);
                //fillbitmap(xprmap, 0, 0);
                Array.Copy(Tilemap.bb10, xprdata, 0x800);
            }
            else
            {
                if ((dirty[0] | dirty[1] | dirty[2] | dirty[3] | dirty[4] | dirty[5] | dirty[6] | dirty[7]) == 0)
                {
                    return 0;
                }
            }
            return 0;
        }
        public static void K056832_tilemap_draw(/*running_machine *machine, bitmap_t *bitmap,*/ RECT cliprect, int layer, int flags, uint priority)
        {
            int[] last_colorbase = new int[16];
            uint last_dx, last_visible, new_colorbase, last_active;
            int sx, sy, ay, tx, ty, width, height;
            int clipw, clipx, cliph, clipy, clipmaxy;
            int line_height, line_endy, line_starty, line_y;
            int sdat_start, sdat_walk, sdat_adv, sdat_wrapmask, sdat_offs;
            int pageIndex, flipx, flipy, corr, r, c;
            int cminy, cmaxy, cminx, cmaxx;
            int dminy, dmaxy, dminx, dmaxx;
            RECT drawrect=new RECT();
            //tilemap *tmap;
            //UINT16 *pScrollData;
            int ScrollData;
            int pScrollData_offset;
            ushort[] ram16 = new ushort[2];
            int rowstart = K056832_Y[layer];
            int colstart = K056832_X[layer];
            int rowspan = K056832_H[layer] + 1;
            int colspan = K056832_W[layer] + 1;
            int dy = K056832_dy[layer];
            int dx = K056832_dx[layer];
            int scrollbank = ((K056832_regs[0x18] >> 1) & 0xc) | (K056832_regs[0x18] & 3);
            int scrollmode = K056832_regs[0x05] >> (K056832_LSRAMPage[layer][0] << 1) & 3;
            if (K056832_use_ext_linescroll != 0)
            {
                scrollbank = 16;
            }
            height = rowspan * 0x100;
            width = colspan * 0x200;
            cminx = cliprect.min_x;
            cmaxx = cliprect.max_x;
            cminy = cliprect.min_y;
            cmaxy = cliprect.max_y;
            flipy = K056832_regs[0] & 0x20;
            if (flipy != 0)
            {
                corr = K056832_regs[0x3c / 2];
                if ((corr & 0x400) != 0)
                {
                    corr |= unchecked((int)0xfffff800);
                }
            }
            else
            {
                corr = 0;
            }
            dy += corr;
            ay = (int)((uint)(dy - K056832_LayerOffset[layer][1]) % height);
            flipx = K056832_regs[0] & 0x10;
            if (flipx != 0)
            {
                corr = K056832_regs[0x3a / 2];
                if ((corr & 0x800) != 0)
                {
                    corr |= unchecked((int)0xfffff000);
                }
            }
            else
            {
                corr = 0;
            }
            corr -= K056832_LayerOffset[layer][0];
            if (scrollmode == 0 && (flags & 0x80000000) != 0)
            {
                scrollmode = 3;
                flags &= (int)~0x80000000;
            }
            switch (scrollmode)
            {
                case 0: // linescroll
                    //pScrollData = &K056832_videoram[scrollbank << 12] + (K056832_LSRAMPage[layer][1] >> 1);
                    ScrollData = (K056832_videoram[(scrollbank << 12) + (K056832_LSRAMPage[layer][1] >> 1)] << 16) | K056832_videoram[(scrollbank << 12) + (K056832_LSRAMPage[layer][1] >> 1) + 1];
                    line_height = 1;
                    sdat_wrapmask = 0x3ff;
                    sdat_adv = 2;
                    break;
                case 2: // rowscroll
                    //pScrollData = &K056832_videoram[scrollbank << 12] + (K056832_LSRAMPage[layer][1] >> 1);
                    ScrollData = (K056832_videoram[(scrollbank << 12) + (K056832_LSRAMPage[layer][1] >> 1)] << 16) | K056832_videoram[(scrollbank << 12) + (K056832_LSRAMPage[layer][1] >> 1) + 1];
                    line_height = 8;
                    sdat_wrapmask = 0x3ff;
                    sdat_adv = 16;
                    break;
                default: // xyscroll
                    //pScrollData = ram16;
                    ScrollData = (ram16[0] << 16) | ram16[1];
                    line_height = 0x100;
                    sdat_wrapmask = 0;
                    sdat_adv = 0;
                    ram16[0] = 0;
                    ram16[1] = (ushort)dx;
                    ScrollData = ram16[1];
                    break;
            }
            if (flipy != 0)
            {
                sdat_adv = -sdat_adv;
            }
            last_active = (uint)K056832_ActiveLayer;
            new_colorbase = (uint)(K056832_UpdateMode != 0 ? K055555_get_palette_index(layer) : 0);
            for (r = 0; r < rowspan; r++)
            {
                if (rowspan > 1)
                {
                    sy = ay;
                    ty = r * 0x100;
                    if (flipy == 0)
                    {
                        if ((r == 0) && (sy > height - 0x100))
                        {
                            sy -= height;
                        }
                        if ((sy + 0x100 <= ty) || (sy - 0x100 >= ty))
                        {
                            continue;
                        }
                        if ((ty -= sy) >= 0)
                        {
                            cliph = 0x100 - ty;
                            clipy = line_starty = ty;
                            line_endy = 0x100;
                            sdat_start = 0;
                        }
                        else
                        {
                            cliph = 0x100 + ty;
                            ty = -ty;
                            clipy = line_starty = 0;
                            line_endy = cliph;
                            sdat_start = ty;
                            if (scrollmode == 2)
                            {
                                sdat_start &= ~7;
                                line_starty -= ty & 7;
                            }
                        }
                    }
                    else
                    {
                        ty += 0x100;
                        if ((r == rowspan - 1) && (sy < 0x100))
                        {
                            sy += height;
                        }
                        if ((sy + 0x100 <= ty) || (sy - 0x100 >= ty))
                        {
                            continue;
                        }
                        if ((ty -= sy) <= 0)
                        {
                            cliph = 0x100 + ty;
                            clipy = line_starty = -ty;
                            line_endy = 0x100;
                            sdat_start = 0x100 - 1;
                            if (scrollmode == 2) sdat_start &= ~7;
                        }
                        else
                        {
                            cliph = 0x100 - ty;
                            clipy = line_starty = 0;
                            line_endy = cliph;
                            sdat_start = cliph - 1;
                            if (scrollmode == 2)
                            {
                                sdat_start &= ~7;
                                line_starty -= ty & 7;
                            }
                        }
                    }
                }
                else
                {
                    cliph = line_endy = 0x100;
                    clipy = line_starty = 0;
                    if (flipy == 0)
                    {
                        sdat_start = dy;
                    }
                    else
                    {
                        sdat_start = 0x100 - 1;
                    }
                    if (scrollmode == 2)
                    {
                        sdat_start &= ~7;
                        line_starty -= dy & 7;
                    }
                }
                sdat_start += r * 0x100;
                sdat_start <<= 1;
                clipmaxy = clipy + cliph - 1;
                for (c = 0; c < colspan; c++)
                {
                    pageIndex = (((rowstart + r) & 3) << 2) + ((colstart + c) & 3);
                    if (K056832_LayerAssociation != 0)
                    {
                        if (K056832_LayerAssociatedWithPage[pageIndex] != layer)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if (K056832_LayerAssociatedWithPage[pageIndex] == -1)
                        {
                            continue;
                        }
                        K056832_ActiveLayer = layer;
                    }
                    if (K056832_UpdateMode != 0)
                    {
                        if (last_colorbase[pageIndex] != new_colorbase)
                        {
                            last_colorbase[pageIndex] = (int)new_colorbase;
                            K056832_mark_page_dirty(pageIndex);
                        }
                    }
                    else if (pageIndex == 0)
                    {
                        K056832_ActiveLayer = 0;
                    }
                    if (K056832_update_linemap(pageIndex, flags)!=0)
                    {
                        continue;
                    }
                    K056832_tilemap[pageIndex].tilemap_set_scrolly(0, ay);
                    last_dx = 0x100000;
                    last_visible = 0;
                    for (sdat_walk = sdat_start, line_y = line_starty; line_y < line_endy; sdat_walk += sdat_adv, line_y += line_height)
                    {
                        dminy = line_y;
                        dmaxy = line_y + line_height - 1;
                        if (dminy < clipy)
                        {
                            dminy = clipy;
                        }
                        if (dmaxy > clipmaxy)
                        {
                            dmaxy = clipmaxy;
                        }
                        if (dminy > cmaxy || dmaxy < cminy)
                        {
                            continue;
                        }
                        sdat_offs = sdat_walk & sdat_wrapmask;
                        drawrect.min_y = (dminy < cminy) ? cminy : dminy;
                        drawrect.max_y = (dmaxy > cmaxy) ? cmaxy : dmaxy;
                        //dx = ((int)pScrollData[sdat_offs] << 16 | (int)pScrollData[sdat_offs + 1]) + corr;
                        dx = ScrollData + corr;
                        //dx = 0;
                        if (last_dx == dx)
                        {
                            if (last_visible != 0)
                            {
                                goto LINE_SHORTCIRCUIT;
                            }
                            continue;
                        }
                        last_dx = (uint)dx;
                        if (colspan > 1)
                        {
                            sx = (int)((uint)dx & (width - 1));
                            tx = c << 9;
                            if (flipx == 0)
                            {
                                if ((c == 0) && (sx > width - 0x200))
                                {
                                    sx -= width;
                                }
                                if ((sx + 0x200 <= tx) || (sx - 0x200 >= tx))
                                {
                                    last_visible = 0;
                                    continue;
                                }
                                if ((tx -= sx) <= 0)
                                {
                                    clipw = 0x200 + tx;
                                    clipx = 0;
                                }
                                else
                                {
                                    clipw = 0x200 - tx;
                                    clipx = tx;
                                }
                            }
                            else
                            {
                                tx += 0x200;
                                if ((c == colspan - 1) && (sx < 0x200))
                                {
                                    sx += width;
                                }
                                if ((sx + 0x200 <= tx) || (sx - 0x200 >= tx))
                                {
                                    last_visible = 0; continue;
                                }
                                if ((tx -= sx) >= 0)
                                {
                                    clipw = 0x200 - tx;
                                    clipx = 0;
                                }
                                else
                                {
                                    clipw = 0x200 + tx;
                                    clipx = -tx;
                                }
                            }
                        }
                        else
                        {
                            clipw = 0x200;
                            clipx = 0;
                        }
                        last_visible = 1;
                        dminx = clipx;
                        dmaxx = clipx + clipw - 1;
                        drawrect.min_x = (dminx < cminx) ? cminx : dminx;
                        drawrect.max_x = (dmaxx > cmaxx) ? cmaxx : dmaxx;
                        K056832_tilemap[pageIndex].tilemap_set_scrollx(0, dx);
                    LINE_SHORTCIRCUIT:
                        if (flags == 0)
                        {
                            flags = 0x10;
                        }
                        K056832_tilemap[pageIndex].tilemap_draw_primask(drawrect, flags, (byte)priority);
                    }
                }
            }
            K056832_ActiveLayer = (int)last_active;
        }
        public static void K056832_set_LayerAssociation(int status)
        {
            K056832_DefaultLayerAssociation = status;
        }
        public static int K056832_get_LayerAssociation()
        {
            return K056832_LayerAssociation;
        }
        public static void K056832_set_LayerOffset(int layer, int offsx, int offsy)
        {
            K056832_LayerOffset[layer][0] = offsx;
            K056832_LayerOffset[layer][1] = offsy;
        }
        public static void SaveStateBinary_K056832(BinaryWriter writer)
        {
            int i, j;
            for (i = 0; i < 0x10000; i++)
            {
                writer.Write(K056832_videoram[i]);
            }
            for (i = 0; i < 32; i++)
            {
                writer.Write(K056832_regs[i]);
            }
            for (i = 0; i < 4; i++)
            {
                writer.Write(K056832_regsb[i]);
            }
            for (i = 0; i < 8; i++)
            {
                writer.Write(K056832_X[i]);
            }
            for (i = 0; i < 8; i++)
            {
                writer.Write(K056832_Y[i]);
            }
            for (i = 0; i < 8; i++)
            {
                writer.Write(K056832_W[i]);
            }
            for (i = 0; i < 8; i++)
            {
                writer.Write(K056832_H[i]);
            }
            for (i = 0; i < 8; i++)
            {
                writer.Write(K056832_dx[i]);
            }
            for (i = 0; i < 8; i++)
            {
                writer.Write(K056832_dy[i]);
            }
            writer.Write(K056832_NumGfxBanks);
            writer.Write(K056832_CurGfxBank);
            writer.Write(K056832_gfxnum);
            writer.Write(K056832_rom_half);
            for (i = 0; i < 0x10; i++)
            {
                writer.Write(K056832_LayerAssociatedWithPage[i]);
            }
            for (i = 0; i < 8; i++)
            {
                for (j = 0; j < 2; j++)
                {
                    writer.Write(K056832_LayerOffset[i][j]);
                }
            }
            for (i = 0; i < 8; i++)
            {
                for (j = 0; j < 2; j++)
                {
                    writer.Write(K056832_LSRAMPage[i][j]);
                }
            }
            for (i = 0; i < 16; i++)
            {
                for (j = 0; j < 8; j++)
                {
                    writer.Write(K056832_LineDirty[i][j]);
                }
            }
            writer.Write(K056832_AllLinesDirty, 0, 16);
            writer.Write(K056832_PageTileMode, 0, 16);
            writer.Write(K056832_LayerTileMode, 0, 8);
            writer.Write(K056832_DefaultLayerAssociation);
            writer.Write(K056832_LayerAssociation);
            writer.Write(K056832_ActiveLayer);
            writer.Write(K056832_SelectedPage);
            writer.Write(K056832_SelectedPagex4096);
            writer.Write(K056832_UpdateMode);
            writer.Write(K056832_linemap_enabled);
            writer.Write(K056832_use_ext_linescroll);
            writer.Write(K056832_uses_tile_banks);
            writer.Write(K056832_cur_tile_bank);
            writer.Write(K056832_djmain_hack);
        }
        public static void LoadStateBinary_K056832(BinaryReader reader)
        {
            int i, j;
            for (i = 0; i < 0x10000; i++)
            {
                K056832_videoram[i] = reader.ReadUInt16();
            }
            for (i = 0; i < 32; i++)
            {
                K056832_regs[i] = reader.ReadUInt16();
            }
            for (i = 0; i < 4; i++)
            {
                K056832_regsb[i] = reader.ReadUInt16();
            }
            for (i = 0; i < 8; i++)
            {
                K056832_X[i] = reader.ReadInt32();
            }
            for (i = 0; i < 8; i++)
            {
                K056832_Y[i] = reader.ReadInt32();
            }
            for (i = 0; i < 8; i++)
            {
                K056832_W[i] = reader.ReadInt32();
            }
            for (i = 0; i < 8; i++)
            {
                K056832_H[i] = reader.ReadInt32();
            }
            for (i = 0; i < 8; i++)
            {
                K056832_dx[i] = reader.ReadInt32();
            }
            for (i = 0; i < 8; i++)
            {
                K056832_dy[i] = reader.ReadInt32();
            }
            K056832_NumGfxBanks = reader.ReadInt32();
            K056832_CurGfxBank = reader.ReadInt32();
            K056832_gfxnum = reader.ReadInt32();
            K056832_rom_half = reader.ReadInt32();
            for (i = 0; i < 0x10; i++)
            {
                K056832_LayerAssociatedWithPage[i] = reader.ReadInt32();
            }
            for (i = 0; i < 8; i++)
            {
                for (j = 0; j < 2; j++)
                {
                    K056832_LayerOffset[i][j] = reader.ReadInt32();
                }
            }
            for (i = 0; i < 8; i++)
            {
                for (j = 0; j < 2; j++)
                {
                    K056832_LSRAMPage[i][j] = reader.ReadInt32();
                }
            }
            for (i = 0; i < 16; i++)
            {
                for (j = 0; j < 8; j++)
                {
                    K056832_LineDirty[i][j] = reader.ReadUInt32();
                }
            }
            K056832_AllLinesDirty = reader.ReadBytes(16);
            K056832_PageTileMode = reader.ReadBytes(16);
            K056832_LayerTileMode = reader.ReadBytes(8);
            K056832_DefaultLayerAssociation = reader.ReadInt32();
            K056832_LayerAssociation = reader.ReadInt32();
            K056832_ActiveLayer = reader.ReadInt32();
            K056832_SelectedPage = reader.ReadInt32();
            K056832_SelectedPagex4096 = reader.ReadInt32();
            K056832_UpdateMode = reader.ReadInt32();
            K056832_linemap_enabled = reader.ReadInt32();
            K056832_use_ext_linescroll = reader.ReadInt32();
            K056832_uses_tile_banks = reader.ReadInt32();
            K056832_cur_tile_bank = reader.ReadInt32();
            K056832_djmain_hack = reader.ReadInt32();
        }
        public static void K055555_vh_start()
        {
            k55555_regs = new byte[128];
            Array.Clear(k55555_regs, 0, 128);
        }
        public static void K055555_write_reg(int regnum, byte regdat)
        {
            k55555_regs[regnum] = regdat;
        }
        public static void K055555_word_w(int offset, ushort data)
        {
            K055555_write_reg(offset, (byte)(data >> 8));
        }
        public static void K055555_word_w(int offset, byte data)
        {
            K055555_write_reg(offset, data);
        }
        public static byte K055555_read_register(int regnum)
        {
            return k55555_regs[regnum];
        }
        public static int K055555_get_palette_index(int idx)
        {
            return k55555_regs[23 + idx];
        }
        public static void SaveStateBinary_K055555(BinaryWriter writer)
        {
            writer.Write(k55555_regs, 0, 128);
        }
        public static void LoadStateBinary_K055555(BinaryReader reader)
        {
            k55555_regs = reader.ReadBytes(128);
        }
        public static void K054338_vh_start()
        {
            k54338_regs = new ushort[32];
            Array.Clear(k54338_regs, 0, 32);
            K054338_shdRGB = new int[9];
            Array.Clear(K054338_shdRGB, 0, 9);
            K054338_alphainverted = 1;
        }
        public static void K054338_word_w(int offset, ushort data)
        {
            k54338_regs[offset] = data;
        }
        public static void K054338_word_w1(int offset, byte data)
        {
            k54338_regs[offset] = (ushort)((data<<8)|(k54338_regs[offset]&0xff));
        }
        public static void K054338_word_w2(int offset, byte data)
        {
            k54338_regs[offset] = (ushort)((k54338_regs[offset]&0xff00)|data);
        }
        public static int K054338_read_register(int reg)
        {
            return k54338_regs[reg];
        }
        public static void K054338_update_all_shadows()
        {
            int i, d;
            int noclip = k54338_regs[15] & 0x20;
            for (i = 0; i < 9; i++)
            {
                d = k54338_regs[2 + i] & 0x1ff;
                if (d >= 0x100)
                {
                    d -= 0x200;
                }
                K054338_shdRGB[i] = d;
            }
            Palette.palette_set_shadow_dRGB32(0, K054338_shdRGB[0], K054338_shdRGB[1], K054338_shdRGB[2], noclip);
            Palette.palette_set_shadow_dRGB32(1, K054338_shdRGB[3], K054338_shdRGB[4], K054338_shdRGB[5], noclip);
            Palette.palette_set_shadow_dRGB32(2, K054338_shdRGB[6], K054338_shdRGB[7], K054338_shdRGB[8], noclip);
        }
        public static void K054338_fill_backcolor(bitmap_t bitmap, int mode)
        {
            int clipx, clipy, clipw, cliph, i, dst_pitch;
            int offset, offsetx, offsety, paletteoffset;
            int BGC_CBLK, BGC_SET;
            int bgcolor;
            clipx = Video.screenstate.visarea.min_x & ~3;
            clipy = Video.screenstate.visarea.min_y;
            clipw = (Video.screenstate.visarea.max_x - clipx + 4) & ~3;
            cliph = Video.screenstate.visarea.max_y - clipy + 1;
            offsetx = 0;
            offsety = clipy;
            offsetx += clipx;
            offset = clipy * bitmap.width;
            offset += clipx;
            dst_pitch = bitmap.rowpixels;
            BGC_SET = 0;
            paletteoffset = 0;
            if (mode == 0)
            {
                bgcolor = (int)(k54338_regs[0] & 0xff) << 16 | (int)k54338_regs[1];
            }
            else
            {
                BGC_CBLK = K055555_read_register(0);
                BGC_SET = K055555_read_register(1);
                paletteoffset += BGC_CBLK << 9;
                if ((BGC_SET & 2) == 0)
                {
                    bgcolor = (int)Generic.paletteram32[paletteoffset];
                    mode = 0;
                }
                else
                {
                    bgcolor = 0;
                }
            }
            bgcolor = (int)(0xff000000 | (uint)bgcolor);
            if (mode == 0)
            {
                offset += clipw;
                i = clipw = -clipw;
                do
                {
                    do
                    {
                        bitmap.ui1[offset + i] = bitmap.ui1[offset + i + 1] = bitmap.ui1[offset + i + 2] = bitmap.ui1[offset + i + 3] = (uint)bgcolor;
                    }
                    while ((i += 4) != 0);
                    offset += dst_pitch;
                    i = clipw;
                }
                while (--cliph != 0);
            }
            else
            {
                if ((BGC_SET & 1) == 0)
                {
                    paletteoffset += clipy;
                    offset += clipw;
                    bgcolor = (int)Generic.paletteram32[paletteoffset++];
                    i = clipw = -clipw;
                    do
                    {
                        do
                        {
                            bitmap.ui1[offset + i] = bitmap.ui1[offset + i + 1] = bitmap.ui1[offset + i + 2] = bitmap.ui1[offset + i + 3] = (uint)bgcolor;
                        }
                        while ((i += 4) != 0);
                        offset += dst_pitch;
                        bgcolor = (int)Generic.paletteram32[paletteoffset++];
                        i = clipw;
                    }
                    while (--cliph != 0);
                }
                else
                {
                    paletteoffset += clipx;
                    clipw <<= 2;
                    do
                    {
                        Array.Copy(Generic.paletteram32, paletteoffset, bitmap.ui1, offset, clipw);
                        offset += dst_pitch;
                    }
                    while (--cliph != 0);
                }
            }
        }
        public static int K054338_set_alpha_level(int pblend)
        {
            int ctrl, mixpri, mixset, mixlv;
            if (pblend <= 0 || pblend > 3)
            {
                Drawgfx.alpha_set_level(255);
                return (255);
            }
            ctrl = k54338_regs[15];
            mixpri = ctrl & 0x02;
            mixset = k54338_regs[13 + (pblend >> 1 & 1)] >> (~pblend << 3 & 8);
            mixlv = mixset & 0x1f;
            if (K054338_alphainverted != 0)
            {
                mixlv = 0x1f - mixlv;
            }
            if ((mixset & 0x20) == 0)
            {
                mixlv = mixlv << 3 | mixlv >> 2;
                Drawgfx.alpha_set_level(mixlv);
            }
            else
            {
                if (mixlv != 0 && mixlv < 0x1f)
                {
                    mixlv = 0x10;
                }
                mixlv = mixlv << 3 | mixlv >> 2;
                Drawgfx.alpha_set_level(mixlv);
            }
            return mixlv;
        }
        public static void K054338_invert_alpha(int invert)
        {
            K054338_alphainverted = invert;
        }
        public static void SaveStateBinary_K054338(BinaryWriter writer)
        {
            int i;
            for (i = 0; i < 32; i++)
            {
                writer.Write(k54338_regs[i]);
            }
        }
        public static void LoadStateBinary_K054338(BinaryReader reader)
        {
            int i;
            for (i = 0; i < 32; i++)
            {
                k54338_regs[i] = reader.ReadUInt16();
            }
        }
        public static void K053250_set_LayerOffset(int chip, int offsx, int offsy)
        {
            K053250_info.chip[chip].offsx = offsx;
            K053250_info.chip[chip].offsy = offsy;
        }
        public static void K053250_dma(int chip, int limiter)
        {
            int last_frame, current_frame;
            current_frame = (int)Video.video_screen_get_frame_number();
            last_frame = K053250_info.chip[chip].frame;
            if (limiter != 0 && current_frame == last_frame)
            {
                return;
            }
            K053250_info.chip[chip].frame = current_frame;
            Array.Copy(K053250_info.chip[chip].ram, K053250_info.chip[chip].buffer[K053250_info.chip[chip].page[chip]], 0x1000);
            K053250_info.chip[chip].page[chip] ^= 1;
        }
        public static void K053250_vh_start(int chips, byte[] bb1)
        {
            ushort[] ram;
            int chip;
            K053250_info.chips = chips;
            for (chip = 0; chip < chips; chip++)
            {
                K053250_info.chip[chip].regs = new byte[8];
                K053250_info.chip[chip].page = new int[2];
                K053250_info.chip[chip].rom = bb1;
                ram = new ushort[0x3000];
                K053250_info.chip[chip].ram = ram;
                K053250_info.chip[chip].rammaxoffset = 0x800;
                K053250_info.chip[chip].buffer0offset = 0x2000;
                K053250_info.chip[chip].buffer1offset = 0x2800;
                Array.Clear(ram, 0x2000, 0x1000);
                K053250_info.chip[chip].rommask = (uint)bb1.Length;
                K053250_info.chip[chip].page[1] = K053250_info.chip[chip].page[0] = 0;
                K053250_info.chip[chip].offsy = K053250_info.chip[chip].offsx = 0;
                K053250_info.chip[chip].frame = -1;
            }
        }
        public static void K053250_0_w(int offset, ushort data)
        {
            //if (ACCESSING_BITS_0_7)
            {
                if (offset == 4 && (data & 2)==0 && (K053250_info.chip[0].regs[4] & 2)!=0)
                {
                    K053250_dma(0, 1);
                }
                K053250_info.chip[0].regs[offset] = (byte)data;
            }
        }
        public static void K053250_0_w2(int offset, byte data)
        {
            if (offset == 4 && (data & 2) == 0 && (K053250_info.chip[0].regs[4] & 2) != 0)
            {
                K053250_dma(0, 1);
            }
            K053250_info.chip[0].regs[offset] = data;
        }
        public static ushort K053250_0_r(int offset)
        {
            return K053250_info.chip[0].regs[offset];
        }
        public static void K053250_0_ram_w(int offset, ushort data)
        {
            K053250_info.chip[0].ram[offset] = data;
        }
        public static void K053250_0_ram_w1(int offset, byte data)
        {
            K053250_info.chip[0].ram[offset] = (ushort)((data << 8) | (K053250_info.chip[0].ram[offset] & 0xff)); ;
        }
        public static void K053250_0_ram_w2(int offset, byte data)
        {
            K053250_info.chip[0].ram[offset] = (ushort)((K053250_info.chip[0].ram[offset] & 0xff00) | data);
        }
        public static ushort K053250_0_ram_r(int offset)
        {
            return K053250_info.chip[0].ram[offset];
        }
        public static ushort K053250_0_rom_r(int offset)
        {
            return K053250_info.chip[0].rom[0x80000 * K053250_info.chip[0].regs[6] + 0x800 * K053250_info.chip[0].regs[7] + (offset >> 1)];
        }
        public static void SaveStateBinary_K053250(BinaryWriter writer)
        {
            int i;
            for (i = 0; i < 0x3000; i++)
            {
                writer.Write(K053250_info.chip[0].ram[i]);
            }
            writer.Write(K053250_info.chip[0].regs,0,8);
        }
        public static void LoadStateBinary_K053250(BinaryReader reader)
        {
            int i;
            for (i = 0; i < 0x3000; i++)
            {
                K053250_info.chip[0].ram[i] = reader.ReadUInt16();
            }
            K053250_info.chip[0].regs = reader.ReadBytes(8);
        }
        public static void K053252_word_w(int offset, ushort data)
        {
            K053252_regs[offset] = data;
        }
        public static void K053252_word_w1(int offset, byte data)
        {
            K053252_regs[offset] = (ushort)((data << 8) | (K053252_regs[offset] & 0xff));
        }
        public static void K053252_word_w2(int offset, byte data)
        {
            K053252_regs[offset] = (ushort)((K053252_regs[offset] & 0xff00) | data);
        }
    }
}
