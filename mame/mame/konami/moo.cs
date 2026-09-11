using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using cpu.m68000;

namespace mame
{
    public partial class Konami
    {
        public static int init_nosound_count;
        public static ushort[] protram;
        public static ushort cur_control2;
        public static int alpha_enabled, zmask;
        public static int[] K053251_CI = new int[4] { 1, 2, 3, 4 };
        public static ushort control1_r()
        {
            int res;
            res = Eeprom.eeprom_read_bit() | dsw1;
            if (init_eeprom_count != 0)
            {
                init_eeprom_count--;
                res &= 0xf7;
            }
            return (ushort)res;
        }
        public static ushort control2_r()
        {
            return cur_control2;
        }
        public static void control2_w(ushort data)
        {
            cur_control2 = data;
            Eeprom.eeprom_write_bit(cur_control2 & 0x01);
            Eeprom.eeprom_set_cs_line((cur_control2 & 0x02) != 0 ? LineState.CLEAR_LINE : LineState.ASSERT_LINE);
            Eeprom.eeprom_set_clock_line((cur_control2 & 0x04) != 0 ? LineState.ASSERT_LINE : LineState.CLEAR_LINE);
            if ((data & 0x100) != 0)
            {
                K053246_set_OBJCHA_line((int)LineState.ASSERT_LINE);
            }
            else
            {
                K053246_set_OBJCHA_line((int)LineState.CLEAR_LINE);
            }
        }
        public static void control2_w1(byte data)
        {
            cur_control2 = (ushort)((data << 8) | (cur_control2 & 0xff));
            Eeprom.eeprom_write_bit(cur_control2 & 0x01);
            Eeprom.eeprom_set_cs_line((cur_control2 & 0x02) != 0 ? LineState.CLEAR_LINE : LineState.ASSERT_LINE);
            Eeprom.eeprom_set_clock_line((cur_control2 & 0x04) != 0 ? LineState.ASSERT_LINE : LineState.CLEAR_LINE);
            if ((data & 0x100) != 0)
            {
                K053246_set_OBJCHA_line((int)LineState.ASSERT_LINE);
            }
            else
            {
                K053246_set_OBJCHA_line((int)LineState.CLEAR_LINE);
            }
        }
        public static void control2_w2(byte data)
        {
            cur_control2 = (ushort)((cur_control2 & 0xff00) | data);
            Eeprom.eeprom_write_bit(cur_control2 & 0x01);
            Eeprom.eeprom_set_cs_line((cur_control2 & 0x02) != 0 ? LineState.CLEAR_LINE : LineState.ASSERT_LINE);
            Eeprom.eeprom_set_clock_line((cur_control2 & 0x04) != 0 ? LineState.ASSERT_LINE : LineState.CLEAR_LINE);
            if ((data & 0x100) != 0)
            {
                K053246_set_OBJCHA_line((int)LineState.ASSERT_LINE);
            }
            else
            {
                K053246_set_OBJCHA_line((int)LineState.CLEAR_LINE);
            }
        }
        public static void moo_objdma()
        {
            int counter, num_inactive;
            int src_offset = 0, dst_offset = 0;
            num_inactive = counter = 256;
            do
            {
                if ((Generic.spriteram16[src_offset] & 0x8000) != 0 && (Generic.spriteram16[src_offset] & zmask) != 0)
                {
                    Array.Copy(Generic.spriteram16, src_offset, K053247_ram, dst_offset, 8);
                    dst_offset += 8;
                    num_inactive--;
                }
                src_offset += 0x80;
            }
            while (--counter != 0);
            if (num_inactive != 0)
            {
                do
                {
                    K053247_ram[dst_offset] = 0;
                    dst_offset += 8;
                }
                while (--num_inactive != 0);
            }
        }
        public static void dmaend_callback()
        {
            if ((cur_control2 & 0x800) != 0)
            {
                Cpuint.cpunum_set_input_line(0, 4, LineState.HOLD_LINE);
            }
        }
        public static void moo_interrupt()
        {
            if (K053246_is_IRQ_enabled() != 0)
            {
                moo_objdma();
                Timer.emu_timer timer = Timer.timer_alloc_common(dmaend_callback, "dmaend_callback", true);
                Timer.timer_adjust_periodic(timer, new Atime(0, (long)1e14), Attotime.ATTOTIME_NEVER);
            }
            if ((cur_control2 & 0x20) != 0)
            {
                Cpuint.cpunum_set_input_line(0, 5, LineState.HOLD_LINE);
            }
        }
        public static void moobl_interrupt()
        {
            moo_objdma();
            Timer.emu_timer timer = Timer.timer_alloc_common(dmaend_callback, "dmaend_callback", true);
            Timer.timer_adjust_periodic(timer, new Atime(0, (long)1e14), Attotime.ATTOTIME_NEVER);
            Cpuint.cpunum_set_input_line(0, 5, LineState.HOLD_LINE);
        }
        public static ushort sound_status_r_moo()
        {
            return Sound.soundlatch3_r();
        }
        public static void sound_bankswitch_w(byte data)
        {
            basebanksnd = (data & 0x0f) * 0x4000;
        }
        public static void moo_prot_w(int offset, ushort data)
        {
            uint src1, src2, dst, length, a, b, res;
            protram[offset] = data;
            if (offset == 0xc)
            {
                src1 = (uint)((protram[1] & 0xff) << 16 | protram[0]);
                src2 = (uint)((protram[3] & 0xff) << 16 | protram[2]);
                dst = (uint)((protram[5] & 0xff) << 16 | protram[4]);
                length = protram[0xf];
                while (length != 0)
                {
                    a = (uint)MC68000.mm1[0].ReadWord((int)src1);
                    b = (uint)MC68000.mm1[0].ReadWord((int)src2);
                    res = a + 2 * b;
                    MC68000.mm1[0].WriteWord((int)dst, (short)res);
                    src1 += 2;
                    src2 += 2;
                    dst += 2;
                    length--;
                }
            }
        }
        public static void moo_prot_w1(int offset, byte data)
        {
            uint src1, src2, dst, length, a, b, res;
            protram[offset] = (ushort)((data << 8) | (protram[offset] & 0xff));
            if (offset == 0xc)
            {
                src1 = (uint)((protram[1] & 0xff) << 16 | protram[0]);
                src2 = (uint)((protram[3] & 0xff) << 16 | protram[2]);
                dst = (uint)((protram[5] & 0xff) << 16 | protram[4]);
                length = protram[0xf];
                while (length != 0)
                {
                    a = (uint)MC68000.mm1[0].ReadWord((int)src1);
                    b = (uint)MC68000.mm1[0].ReadWord((int)src2);
                    res = a + 2 * b;
                    MC68000.mm1[0].WriteWord((int)dst, (short)res);
                    src1 += 2;
                    src2 += 2;
                    dst += 2;
                    length--;
                }
            }
        }
        public static void moo_prot_w2(int offset, byte data)
        {
            uint src1, src2, dst, length, a, b, res;
            protram[offset] = (ushort)((protram[offset] & 0xff00) | data);
            if (offset == 0xc)
            {
                src1 = (uint)((protram[1] & 0xff) << 16 | protram[0]);
                src2 = (uint)((protram[3] & 0xff) << 16 | protram[2]);
                dst = (uint)((protram[5] & 0xff) << 16 | protram[4]);
                length = protram[0xf];
                while (length != 0)
                {
                    a = (uint)MC68000.mm1[0].ReadWord((int)src1);
                    b = (uint)MC68000.mm1[0].ReadWord((int)src2);
                    res = a + 2 * b;
                    MC68000.mm1[0].WriteWord((int)dst, (short)res);
                    src1 += 2;
                    src2 += 2;
                    dst += 2;
                    length--;
                }
            }
        }
        public static void moobl_oki_bank_w(ushort data)
        {
            OKI6295.oo1[0].okim6295_set_bank_base((data & 0x0f) * 0x40000);
        }
        public static void machine_reset_moo()
        {
            init_nosound_count = 0;
        }
        public static void moo_sprite_callback(int code, int color, int priority, out int code2, out int color2, out int priority2)
        {
            int pri = (color & 0x03e0) >> 4;
            if (pri <= layerpri[2])
            {
                priority2 = 0;
            }
            else if (pri <= layerpri[1])
            {
                priority2 = 0xf0;
            }
            else if (pri <= layerpri[0])
            {
                priority2 = 0xf0 | 0xcc;
            }
            else
            {
                priority2 = 0xf0 | 0xcc | 0xaa;
            }
            code2 = code;
            color2 = sprite_colorbase | (color & 0x001f);
        }
        public static void moo_tile_callback(int layer, int code, int color, int flags, out int code2, out int color2, out int flags2)
        {
            code2 = code;
            color2 = layer_colorbase[layer] | (color >> 2 & 0x0f);
            flags2 = flags;
        }
        public static void video_start_moo()
        {
            int offsx, offsy;
            zmask = 0xffff;
            alpha_enabled = 0;
            K053251_vh_start();
            K054338_vh_start();
            K056832_vh_start(moo_tile_callback, 0);
            K056832_set_LayerOffset(0, -2 + 1, 0);
            K056832_set_LayerOffset(1, 2 + 1, 0);
            K056832_set_LayerOffset(2, 4 + 1, 0);
            K056832_set_LayerOffset(3, 6 + 1, 0);
            offsx = -48 + 1;
            offsy = 23;
            K053247_vh_start(offsx, offsy, 0x0123, moo_sprite_callback);
            K054338_invert_alpha(0);
        }
        public static void video_start_bucky()
        {
            int offsx, offsy;
            zmask = 0x00ff;
            alpha_enabled = 0;
            K053251_vh_start();
            K054338_vh_start();
            K056832_vh_start(moo_tile_callback, 0);
            K056832_set_LayerAssociation(0);
            K056832_set_LayerOffset(0, -2, 0);
            K056832_set_LayerOffset(1, 2, 0);
            K056832_set_LayerOffset(2, 4, 0);
            K056832_set_LayerOffset(3, 6, 0);
            offsx = -48;
            offsy = 23;
            K053247_vh_start(offsx, offsy, 0x0123, moo_sprite_callback);
            K054338_invert_alpha(0);
        }
        public static void video_update_moo()
        {
            int i, j;
            int[] layers = new int[3];
            int bg_colorbase, new_colorbase, plane, dirty, alpha;
            bg_colorbase = K053251_get_palette_index(1);
            sprite_colorbase = K053251_get_palette_index(0);
            layer_colorbase[0] = 0x70;
            if (K056832_get_LayerAssociation() != 0)
            {
                for (plane = 1; plane < 4; plane++)
                {
                    new_colorbase = K053251_get_palette_index(K053251_CI[plane]);
                    if (layer_colorbase[plane] != new_colorbase)
                    {
                        layer_colorbase[plane] = new_colorbase;
                        K056832_mark_plane_dirty(plane);
                    }
                }
            }
            else
            {
                for (dirty = 0, plane = 1; plane < 4; plane++)
                {
                    new_colorbase = K053251_get_palette_index(K053251_CI[plane]);
                    if (layer_colorbase[plane] != new_colorbase)
                    {
                        layer_colorbase[plane] = new_colorbase;
                        dirty = 1;
                    }
                }
                if (dirty != 0)
                {
                    K056832_MarkAllTilemapsDirty();
                }
            }
            layers[0] = 1;
            layerpri[0] = K053251_get_priority(2);
            layers[1] = 2;
            layerpri[1] = K053251_get_priority(3);
            layers[2] = 3;
            layerpri[2] = K053251_get_priority(4);
            sortlayers(layers, layerpri);
            K054338_update_all_shadows();
            K054338_fill_backcolor(Palette.bbitmap[Video.curbitmap], 0);
            for (j = Video.new_clip.min_y; j <= Video.new_clip.max_y; j++)
            {
                for (i = Video.new_clip.min_x; i <= Video.new_clip.max_x; i++)
                {
                    Tilemap.ppriority_bitmap[j * 0x200 + i] = 0;
                }
            }
            if (layerpri[0] < K053251_get_priority(1))
            {
                K056832_tilemap_draw(Video.new_clip, layers[0], 0, 1);
            }
            K056832_tilemap_draw(Video.new_clip, layers[1], 0, 2);
            alpha_enabled = K054338_read_register(15) & 0x02;
            alpha = (alpha_enabled != 0) ? K054338_set_alpha_level(1) : 255;
            if (alpha > 0)
            {
                K056832_tilemap_draw(Video.new_clip, layers[2], (alpha >= 255) ? 0 : 0x100, 4);
            }
            K053247_sprites_draw(Palette.bbitmap[Video.curbitmap], Video.new_clip);
            K056832_tilemap_draw(Video.new_clip, 0, 0, 0);
        }
    }
}
