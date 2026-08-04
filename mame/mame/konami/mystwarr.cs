using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mame
{
    public partial class Konami
    {
        public static byte[] audioram2, audioram3;
        public static byte[] eepromrom;
        public static byte mw_irq_control;
        public static byte[] gx_workram;
        public static int oinprion, cbparam;
        public static int cur_sound_region;
        public static int sub1_colorbase, last_psac_colorbase, gametype;
        public static int roz_enable, roz_rombank;
        public static Tmap ult_936_tilemap;
        public static ushort clip;

        public static void nvram_handler_load_mystwarr()
        {
            Array.Copy(eepromrom, Eeprom.eeprom_data, 0x80);
        }
        public static ushort mweeprom_r()
        {
            int res = dsw1 | Eeprom.eeprom_read_bit();
            if (init_eeprom_count != 0)
            {
                init_eeprom_count--;
                res &= ~0x04;
            }
            return (ushort)res;
        }
        public static byte mweeprom_r2()
        {
            int res = dsw1 | Eeprom.eeprom_read_bit();
            if (init_eeprom_count != 0)
            {
                init_eeprom_count--;
                res &= ~0x04;
            }
            return (byte)res;
        }
        public static ushort vseeprom_r()
        {
            int res = dsw1 | Eeprom.eeprom_read_bit();
            if (init_eeprom_count != 0)
            {
                init_eeprom_count--;
                res &= ~0x08;
            }
            return (ushort)res;
        }
        public static byte vseeprom_r2()
        {
            int res = dsw1 | Eeprom.eeprom_read_bit();
            if (init_eeprom_count != 0)
            {
                init_eeprom_count--;
                res &= ~0x08;
            }
            return (byte)res;
        }
        public static void mweeprom_w(ushort data)
        {
            Eeprom.eeprom_write_bit((data & 0x0100) != 0 ? 1 : 0);
            Eeprom.eeprom_set_cs_line((data & 0x0200) != 0 ? LineState.CLEAR_LINE : LineState.ASSERT_LINE);
            Eeprom.eeprom_set_clock_line((data & 0x0400) != 0 ? LineState.ASSERT_LINE : LineState.CLEAR_LINE);
        }
        public static void mweeprom_w1(byte data)
        {
            Eeprom.eeprom_write_bit((data & 0x01) != 0 ? 1 : 0);
            Eeprom.eeprom_set_cs_line((data & 0x02) != 0 ? LineState.CLEAR_LINE : LineState.ASSERT_LINE);
            Eeprom.eeprom_set_clock_line((data & 0x04) != 0 ? LineState.ASSERT_LINE : LineState.CLEAR_LINE);
        }
        public static ushort dddeeprom_r()
        {
            return (ushort)((dsw1 | Eeprom.eeprom_read_bit()) << 8);
        }
        public static byte dddeeprom_r1()
        {
            return (byte)(dsw1 | Eeprom.eeprom_read_bit());
        }
        public static byte dddeeprom_r2()
        {
            return (byte)sbyte2;
        }
        public static void mmeeprom_w(ushort data)
        {
            Eeprom.eeprom_write_bit((data & 0x01) != 0 ? 1 : 0);
            Eeprom.eeprom_set_cs_line((data & 0x02) != 0 ? LineState.CLEAR_LINE : LineState.ASSERT_LINE);
            Eeprom.eeprom_set_clock_line((data & 0x04) != 0 ? LineState.ASSERT_LINE : LineState.CLEAR_LINE);
        }
        public static void mmeeprom_w2(byte data)
        {
            Eeprom.eeprom_write_bit((data & 0x01) != 0 ? 1 : 0);
            Eeprom.eeprom_set_cs_line((data & 0x02) != 0 ? LineState.CLEAR_LINE : LineState.ASSERT_LINE);
            Eeprom.eeprom_set_clock_line((data & 0x04) != 0 ? LineState.ASSERT_LINE : LineState.CLEAR_LINE);
        }
        public static void mystwarr_interrupt()
        {
            if ((mw_irq_control & 0x01) == 0)
            {
                return;
            }
            switch (Cpuexec.cpu[0].iloops)
            {
                case 0:
                    Cpuint.cpunum_set_input_line(0, 2, LineState.HOLD_LINE);
                    break;
                case 1:
                    Cpuint.cpunum_set_input_line(0, 4, LineState.HOLD_LINE);
                    break;
                case 2:
                    Cpuint.cpunum_set_input_line(0, 6, LineState.HOLD_LINE);
                    break;
            }
        }
        public static void metamrph_interrupt()
        {
            switch (Cpuexec.cpu[0].iloops)
            {
                case 0:
                    Cpuint.cpunum_set_input_line(0, 4, LineState.HOLD_LINE);
                    break;
                case 15:
                    Cpuint.cpunum_set_input_line(0, 6, LineState.HOLD_LINE);
                    break;
                case 39:
                    if (K053246_is_IRQ_enabled() != 0)
                    {
                        Cpuint.cpunum_set_input_line(0, 5, LineState.HOLD_LINE);
                    }
                    break;
            }
        }
        public static void mchamp_interrupt()
        {
            if ((mw_irq_control & 0x02) == 0)
            {
                return;
            }
            switch (Cpuexec.cpu[0].iloops)
            {
                case 0:
                    if (K053246_is_IRQ_enabled() != 0)
                    {
                        Cpuint.cpunum_set_input_line(0, 6, LineState.HOLD_LINE);
                    }
                    break;
                case 1:
                    Cpuint.cpunum_set_input_line(0, 2, LineState.HOLD_LINE);
                    break;
            }
        }
        public static void ddd_interrupt()
        {
            Cpuint.cpunum_set_input_line(0, 5, LineState.HOLD_LINE);
        }
        public static void sound_cmd1_w(ushort data)
        {
            Sound.soundlatch_w((ushort)(data & 0xff));
        }
        public static void sound_cmd1_msb_w(ushort data)
        {
            Sound.soundlatch_w((ushort)(data >> 8));
        }
        public static void sound_cmd2_w(ushort data)
        {
            Sound.soundlatch2_w((ushort)(data & 0xff));
        }
        public static void sound_cmd2_msb_w(ushort data)
        {
            Sound.soundlatch2_w((ushort)(data >> 8));
        }
        public static void sound_irq_w()
        {
            Cpuint.cpunum_set_input_line(1, 0, LineState.HOLD_LINE);
        }
        public static ushort sound_status_r()
        {
            int latch = Sound.soundlatch3_r();
            if ((latch & 0xf) == 0xe)
            {
                latch |= 1;
            }
            return (ushort)latch;
        }
        public static ushort sound_status_msb_r()
        {
            int latch = Sound.soundlatch3_r();
            if ((latch & 0xf) == 0xe)
            {
                latch |= 1;
            }
            return (ushort)(latch << 8);
        }
        public static void irq_ack_w(int offset, ushort data)
        {
            K056832_b_word_w(offset, data);
            if (offset == 3)
            {
                mw_irq_control = (byte)(data & 0xff);
            }
        }
        public static void irq_ack_w1(int offset, byte data)
        {
            K056832_b_word_w1(offset, data);
        }
        public static void irq_ack_w2(int offset, byte data)
        {
            K056832_b_word_w2(offset, data);
            if (offset == 3)
            {
                mw_irq_control = (byte)(data & 0xff);
            }
        }
        public static ushort mmcoins_r()
        {
            int res = sbyte0;
            if (init_eeprom_count != 0)
            {
                init_eeprom_count--;
                res &= ~0x10;
            }
            return (ushort)res;
        }
        public static ushort dddcoins_r()
        {
            int res = ((byte)sbyte0 << 8) | (byte)sbyte1;
            if (init_eeprom_count != 0)
            {
                init_eeprom_count--;
                res &= ~0x0800;
            }
            return (ushort)res;
        }
        public static ushort K053247_scattered_word_r(int offset)
        {
            if ((offset & 0x0078) != 0)
            {
                return Generic.spriteram16[offset];
            }
            else
            {
                offset = (offset & 0x0007) | ((offset & 0x7f80) >> 4);
                return K053247_word_r(offset);
            }
        }
        public static void K053247_scattered_word_w(int offset, ushort data)
        {
            if ((offset & 0x0078) != 0)
            {
                Generic.spriteram16[offset] = data;
            }
            else
            {
                offset = (offset & 0x0007) | ((offset & 0x7f80) >> 4);
                K053247_word_w(offset, data);
            }
        }
        public static void K053247_scattered_word_w1(int offset, byte data)
        {
            if ((offset & 0x0078) != 0)
            {
                Generic.spriteram16[offset] = (ushort)((data<<8)|(Generic.spriteram16[offset]&0xff));
            }
            else
            {
                offset = (offset & 0x0007) | ((offset & 0x7f80) >> 4);
                K053247_word_w1(offset, data);
            }
        }
        public static void K053247_scattered_word_w2(int offset, byte data)
        {
            if ((offset & 0x0078) != 0)
            {
                Generic.spriteram16[offset] = (ushort)((Generic.spriteram16[offset] & 0xff00) | data);
            }
            else
            {
                offset = (offset & 0x0007) | ((offset & 0x7f80) >> 4);
                K053247_word_w2(offset, data);
            }
        }
        public static ushort K053247_martchmp_word_r(int offset)
        {
            if ((offset & 0x0018) != 0)
            {
                return Generic.spriteram16[offset];
            }
            else
            {
                offset = (offset & 0x0007) | ((offset & 0x1fe0) >> 2);
                return K053247_word_r(offset);
            }
        }
        public static void K053247_martchmp_word_w(int offset, ushort data)
        {
            if ((offset & 0x0018) != 0)
            {
                Generic.spriteram16[offset] = data;
            }
            else
            {
                offset = (offset & 0x0007) | ((offset & 0x1fe0) >> 2);
                K053247_word_w(offset, data);
            }
        }
        public static void K053247_martchmp_word_w1(int offset, byte data)
        {
            if ((offset & 0x0018) != 0)
            {
                Generic.spriteram16[offset] = (ushort)((data << 8) | (Generic.spriteram16[offset] & 0xff));
            }
            else
            {
                offset = (offset & 0x0007) | ((offset & 0x1fe0) >> 2);
                K053247_word_w1(offset, data);
            }
        }
        public static void K053247_martchmp_word_w2(int offset, byte data)
        {
            if ((offset & 0x0018) != 0)
            {
                Generic.spriteram16[offset] = (ushort)((Generic.spriteram16[offset] & 0xff00) | data);
            }
            else
            {
                offset = (offset & 0x0007) | ((offset & 0x1fe0) >> 2);
                K053247_word_w2(offset, data);
            }
        }
        public static ushort mccontrol_r()
        {
            return (ushort)(mw_irq_control << 8);
        }
        public static void mccontrol_w(ushort data)
        {
            mw_irq_control = (byte)(data >> 8);
            K053246_set_OBJCHA_line((int)((data & 0x04) != 0 ? LineState.ASSERT_LINE : LineState.CLEAR_LINE));
        }
        public static void mccontrol_w1(byte data)
        {
            mw_irq_control = data;
            K053246_set_OBJCHA_line((int)LineState.CLEAR_LINE);
        }
        public static void reset_sound_region()
        {
            basebanksnd = 0x20000 + cur_sound_region * 0x4000;
        }
        public static void sound_ctrl_w(byte data)
        {
            cur_sound_region = (data & 0xf);
            reset_sound_region();
        }
        public static void machine_start_mystwarr()
        {
            cur_sound_region = 2;
            reset_sound_region();
            mw_irq_control = 0;
        }
        public static void machine_reset_mystwarr()
        {
            int i;
            for (i = 0; i <= 3; i++)
            {
                K054539.k054539_set_gain(0, i, 0.8);
                K054539.k054539_set_gain(0, i + 4, 2.0);
            }
            for (i = 0; i <= 7; i++)
            {
                K054539.k054539_set_gain(1, i, 0.5);
            }
        }
        public static void machine_reset_dadandrn()
        {
            int i;
            for (i = 4; i <= 7; i++)
            {
                K054539.k054539_set_gain(0, i, 2.0);
            }
        }
        public static void machine_reset_viostorm()
        {
            int i;
            for (i = 4; i <= 7; i++)
            {
                K054539.k054539_set_gain(0, i, 2.0);
            }
        }
        public static void machine_reset_metamrph()
        {
            int i;
            for (i = 0; i <= 3; i++)
            {
                K054539.k054539_set_gain(0, i, 0.8);
                K054539.k054539_set_gain(0, i + 4, 1.8);
                K054539.k054539_set_gain(1, i, 0.8);
                K054539.k054539_set_gain(1, i + 4, 0.8);
            }
        }
        public static void machine_reset_martchmp()
        {
            int i;
            K054539.k054539_init_flags(0, 1);
            for (i = 4; i <= 7; i++)
            {
                K054539.k054539_set_gain(0, i, 1.4);
            }
        }
        public static void machine_reset_gaiapols()
        {
            int i;
            for (i = 5; i <= 7; i++)
            {
                K054539.k054539_set_gain(0, i, 2.0);
            }
        }
        public static void mystwarr_tile_callback(int layer, int code, int color, int flags, out int code2, out int color2, out int flags2)
        {
            if (layer == 1)
            {
                if ((code & 0xff00) + (color) == 0x4101)
                {
                    cbparam++;
                }
                else
                {
                    cbparam--;
                }
            }
            code2 = code;
            color2 = layer_colorbase[layer] | (color >> 1 & 0x1f);
            flags2 = flags;
        }
        public static void game5bpp_tile_callback(int layer, int code, int color, int flags, out int code2, out int color2, out int flags2)
        {
            code2 = code;
            color2 = layer_colorbase[layer] | (color >> 1 & 0x1f);
            flags2 = flags;
        }
        public static void game4bpp_tile_callback(int layer, int code, int color, int flags, out int code2, out int color2, out int flags2)
        {
            code2 = code;
            color2 = layer_colorbase[layer] | (color >> 2 & 0x0f);
            flags2 = flags;
        }
        public static void mystwarr_sprite_callback(int code, int color, int priority, out int code2, out int color2, out int priority2)
        {
            int c = color;
            code2 = code;
            color2 = sprite_colorbase | (c & 0x001f);
            priority2 = c & 0x00f0;
        }
        public static void metamrph_sprite_callback(int code, int color, int priority, out int code2, out int color2, out int priority2)
        {
            int c = color;
            int attr = c;
            c = (c & 0x1f) | sprite_colorbase;
            code2 = code;
            if ((attr & 0x300) != 0x300)
            {
                color2 = c;
                priority2 = (attr & 0xe0) >> 2;
            }
            else
            {
                color2 = c | 3 << 16 | 0x40000000;
                priority2 = 0x1c;
            }
        }
        public static void gaiapols_sprite_callback(int code, int color, int priority, out int code2, out int color2, out int priority2)
        {
            int c = color;
            code2 = code;
            color2 = sprite_colorbase | (c >> 4 & 0x20) | (c & 0x001f);
            priority2 = c & 0x00e0;
        }
        public static void martchmp_sprite_callback(int code, int color, int priority, out int code2, out int color2, out int priority2)
        {
            int c = color;
            code2 = code;
            if ((c & 0x3ff) == 0x11f)
            {
                color2 = unchecked((int)0x80000000);
            }
            else
            {
                color2 = sprite_colorbase | (c & 0x1f);
            }
            if ((oinprion & 0xf0)!=0)
            {
                priority2 = cbparam;
            }
            else
            {
                priority2 = c & 0xf0;
            }
        }
        public static void video_start_gaiapols()
        {
            int i;
            K055555_vh_start();
            K054338_vh_start();
            gametype = 0;
            K056832_vh_start(game4bpp_tile_callback, 0);
            K055673_vh_start(1, -61, -22, gaiapols_sprite_callback);
            konamigx_mixer_init(0);
            K056832_set_LayerOffset(0, -2 + 2 - 1, 0 - 1);
            K056832_set_LayerOffset(1, 0 + 2, 0);
            K056832_set_LayerOffset(2, 2 + 2, 0);
            K056832_set_LayerOffset(3, 3 + 2, 0);
            K053936_wraparound_enable(0, 1);
            K053936GP_set_offset(0, -10, 0);
            ult_936_tilemap = Tmap.tilemap_create(Tmap.tilemap_scan_rows, 16, 16, 512, 512);
            ult_936_tilemap.pen_to_flags = new byte[1, 16];
            ult_936_tilemap.pen_to_flags[0, 0] = 0;
            for (i = 1; i < 16; i++)
            {
                ult_936_tilemap.pen_to_flags[0, i] = 0x10;
            }
        }
        public static void video_start_dadandrn()
        {
            int i;
            K055555_vh_start();
            K054338_vh_start();
            gametype = 1;
            K056832_vh_start(game5bpp_tile_callback, 0);
            K055673_vh_start(0, -42, -22, gaiapols_sprite_callback);
            konamigx_mixer_init(0);
            konamigx_mixer_primode(1);
            K056832_set_LayerOffset(0, -2 + 4, 0);
            K056832_set_LayerOffset(1, 0 + 4, 0);
            K056832_set_LayerOffset(2, 2 + 4, 0);
            K056832_set_LayerOffset(3, 3 + 4, 0);
            K053936_wraparound_enable(0, 1);
            K053936GP_set_offset(0, -8, 0);
            ult_936_tilemap = Tmap.tilemap_create(Tmap.tilemap_scan_rows, 16, 16, 512, 512);
            ult_936_tilemap.pen_to_flags = new byte[1, 16];
            ult_936_tilemap.pen_to_flags[0, 0] = 0;
            for (i = 1; i < 16; i++)
            {
                ult_936_tilemap.pen_to_flags[0, i] = 0x10;
            }
        }
        public static void video_start_mystwarr()
        {            
            K055555_vh_start();
            K054338_vh_start();
            gametype = 0;
            K056832_vh_start(mystwarr_tile_callback, 0);
            K055673_vh_start(0, -48, -24, mystwarr_sprite_callback);
            konamigx_mixer_init(0);
            K056832_set_LayerOffset(0, -2 - 3, 0);
            K056832_set_LayerOffset(1, 0 - 3, 0);
            K056832_set_LayerOffset(2, 2 - 3, 0);
            K056832_set_LayerOffset(3, 3 - 3, 0);
            cbparam = 0;
        }
        public static void video_start_metamrph()
        {
            gametype = 0;
            K055555_vh_start();
            K054338_vh_start();
            K053250_vh_start(1, gfx3rom);
            K056832_vh_start(game4bpp_tile_callback, 0);
            K055673_vh_start(1, -51, -22, metamrph_sprite_callback);
            konamigx_mixer_init(0);
            K056832_set_LayerOffset(0, -2 + 4, 0);
            K056832_set_LayerOffset(1, 0 + 4, 0);
            K056832_set_LayerOffset(2, 2 + 4, 0);
            K056832_set_LayerOffset(3, 3 + 4, 0);
            K053250_set_LayerOffset(0, -7, 0);
        }
        public static void video_start_viostorm()
        {
            gametype = 0;
            K055555_vh_start();
            K054338_vh_start();
            K056832_vh_start(game4bpp_tile_callback, 0);
            K055673_vh_start(1, -62, -23, metamrph_sprite_callback);
            konamigx_mixer_init(0);
            K056832_set_LayerOffset(0, -2 + 1, 0);
            K056832_set_LayerOffset(1, 0 + 1, 0);
            K056832_set_LayerOffset(2, 2 + 1, 0);
            K056832_set_LayerOffset(3, 3 + 1, 0);
        }
        public static void video_start_martchmp()
        {
            gametype = 0;
            K055555_vh_start();
            K054338_vh_start();
            K056832_vh_start(game5bpp_tile_callback, 0);
            K055673_vh_start(0, -58, -23, martchmp_sprite_callback);
            konamigx_mixer_init(0);
            K056832_set_LayerOffset(0, -2 - 4, 0);
            K056832_set_LayerOffset(1, 0 - 4, 0);
            K056832_set_LayerOffset(2, 2 - 4, 0);
            K056832_set_LayerOffset(3, 3 - 4, 0);
            K054338_invert_alpha(0);
        }
        public static void video_update_mystwarr()
        {
            int i, old, blendmode = 0;
            if (cbparam < 0)
            {
                cbparam = 0;
            }
            else if (cbparam >= 32)
            {
                blendmode = (1 << 16 | 3) << 2;
            }
            for (i = 0; i < 4; i++)
            {
                old = layer_colorbase[i];
                layer_colorbase[i] = K055555_get_palette_index(i) << 4;
                if (old != layer_colorbase[i])
                {
                    K056832_mark_plane_dirty(i);
                }
            }
            sprite_colorbase = K055555_get_palette_index(4) << 5;
            konamigx_mixer(Palette.bbitmap[Video.curbitmap], Video.new_clip, 0, 0, blendmode);
        }
        public static void video_update_metamrph()
        {
            int i, old;
            for (i = 0; i < 4; i++)
            {
                old = layer_colorbase[i];
                layer_colorbase[i] = K055555_get_palette_index(i) << 4;
                if (old != layer_colorbase[i])
                {
                    K056832_mark_plane_dirty(i);
                }
            }
            sprite_colorbase = K055555_get_palette_index(4) << 4;
            konamigx_mixer(Palette.bbitmap[Video.curbitmap], Video.new_clip, 0x14, 0, 0);
        }
        public static void video_update_martchmp()
        {
            int i, old, blendmode;
            for (i = 0; i < 4; i++)
            {
                old = layer_colorbase[i];
                layer_colorbase[i] = K055555_get_palette_index(i) << 4;
                if (old != layer_colorbase[i])
                {
                    K056832_mark_plane_dirty(i);
                }
            }
            sprite_colorbase = K055555_get_palette_index(4) << 5;
            cbparam = K055555_read_register(15);
            oinprion = K055555_read_register(19);
            blendmode = (oinprion == 0xef && K054338_read_register(13)!=0) ? ((1 << 16 | 3) << 2) : 0;
            konamigx_mixer(Palette.bbitmap[Video.curbitmap], Video.new_clip, 0, 0, blendmode);
        }
        public static void ddd_053936_enable_w(ushort data)
        {
            //if (ACCESSING_BITS_8_15)
            {
                roz_enable = data & 0x0100;
                roz_rombank = (data & 0xc000) >> 14;
            }
        }
        public static void ddd_053936_enable_w1(byte data)
        {
            roz_enable = (data<<8) & 0x0100;
            roz_rombank = ((data<<8) & 0xc000) >> 14;
        }
        public static void ddd_053936_clip_w(int offset, ushort data)
        {
            int old, clip_x, clip_y, size_x, size_y;
            int minx, maxx, miny, maxy;
            if (offset == 1)
            {
                //if (ACCESSING_BITS_8_15)
                {
                    K053936GP_clip_enable(0, data & 0x0100);
                }
            }
            else
            {
                old = clip;
                //COMBINE_DATA(&clip);
                clip = data;
                if (clip != old)
                {
                    clip_x = (clip & 0x003f) >> 0;
                    clip_y = (clip & 0x0fc0) >> 6;
                    size_x = (clip & 0x3000) >> 12;
                    size_y = (clip & 0xc000) >> 14;
                    switch (size_x)
                    {
                        case 0x3: size_x = 1; break;
                        case 0x2: size_x = 2; break;
                        default: size_x = 4; break;
                    }
                    switch (size_y)
                    {
                        case 0x3: size_y = 1; break;
                        case 0x2: size_y = 2; break;
                        default: size_y = 4; break;
                    }
                    minx = clip_x << 7;
                    maxx = ((clip_x + size_x) << 7) - 1;
                    miny = clip_y << 7;
                    maxy = ((clip_y + size_y) << 7) - 1;
                    K053936GP_set_cliprect(0, minx, maxx, miny, maxy);
                }
            }
        }
        public static void ddd_053936_clip_w1(int offset, byte data)
        {
            int old, clip_x, clip_y, size_x, size_y;
            int minx, maxx, miny, maxy;
            if (offset == 1)
            {
                K053936GP_clip_enable(0, (data<<8) & 0x0100);
            }
            else
            {
                old = clip;
                //COMBINE_DATA(&clip);
                clip = (ushort)((data<<8)|(clip&0xff));
                if (clip != old)
                {
                    clip_x = (clip & 0x003f) >> 0;
                    clip_y = (clip & 0x0fc0) >> 6;
                    size_x = (clip & 0x3000) >> 12;
                    size_y = (clip & 0xc000) >> 14;
                    switch (size_x)
                    {
                        case 0x3: size_x = 1; break;
                        case 0x2: size_x = 2; break;
                        default: size_x = 4; break;
                    }
                    switch (size_y)
                    {
                        case 0x3: size_y = 1; break;
                        case 0x2: size_y = 2; break;
                        default: size_y = 4; break;
                    }
                    minx = clip_x << 7;
                    maxx = ((clip_x + size_x) << 7) - 1;
                    miny = clip_y << 7;
                    maxy = ((clip_y + size_y) << 7) - 1;
                    K053936GP_set_cliprect(0, minx, maxx, miny, maxy);
                }
            }
        }
        public static void ddd_053936_clip_w2(int offset, byte data)
        {
            int old, clip_x, clip_y, size_x, size_y;
            int minx, maxx, miny, maxy;
            if (offset == 1)
            {

            }
            else
            {
                old = clip;
                //COMBINE_DATA(&clip);
                clip = (ushort)((clip & 0xff00) | data);
                if (clip != old)
                {
                    clip_x = (clip & 0x003f) >> 0;
                    clip_y = (clip & 0x0fc0) >> 6;
                    size_x = (clip & 0x3000) >> 12;
                    size_y = (clip & 0xc000) >> 14;
                    switch (size_x)
                    {
                        case 0x3: size_x = 1; break;
                        case 0x2: size_x = 2; break;
                        default: size_x = 4; break;
                    }
                    switch (size_y)
                    {
                        case 0x3: size_y = 1; break;
                        case 0x2: size_y = 2; break;
                        default: size_y = 4; break;
                    }
                    minx = clip_x << 7;
                    maxx = ((clip_x + size_x) << 7) - 1;
                    miny = clip_y << 7;
                    maxy = ((clip_y + size_y) << 7) - 1;
                    K053936GP_set_cliprect(0, minx, maxx, miny, maxy);
                }
            }
        }
        public static ushort gai_053936_tilerom_0_r(int offset)
        {
            return (ushort)((gfx4rom[0x20000 + offset] << 8) | gfx4rom[0x60000 + offset]);
        }
        public static ushort ddd_053936_tilerom_0_r(int offset)
        {
            return (ushort)((gfx4rom[offset] << 8) | gfx4rom[0x40000 + offset]);
        }
        public static ushort ddd_053936_tilerom_1_r(int offset)
        {
            return gfx4rom[offset / 2];
        }
        public static ushort gai_053936_tilerom_2_r(int offset)
        {
            offset += (roz_rombank * 0x100000);
            return (ushort)(gfx3rom[offset / 2] << 8);
        }
        public static ushort ddd_053936_tilerom_2_r(int offset)
        {
            offset += (roz_rombank * 0x100000);
            return (ushort)(gfx3rom[offset] << 8);
        }
        public static void video_update_dadandrn()
        {
            int i, newbase, dirty, rozmode;
            if (gametype == 0)
            {
                sprite_colorbase = (K055555_get_palette_index(4) << 4) & 0x7f;
                rozmode = 0x04;
            }
            else
            {
                sprite_colorbase = (K055555_get_palette_index(4) << 3) & 0x7f;
                rozmode = 0x08;
            }

            if (K056832_get_LayerAssociation()!=0)
            {
                for (i = 0; i < 4; i++)
                {
                    newbase = K055555_get_palette_index(i) << 4;
                    if (layer_colorbase[i] != newbase)
                    {
                        layer_colorbase[i] = newbase;
                        K056832_mark_plane_dirty(i);
                    }
                }
            }
            else
            {
                for (dirty = 0, i = 0; i < 4; i++)
                {
                    newbase = K055555_get_palette_index(i) << 4;
                    if (layer_colorbase[i] != newbase)
                    {
                        layer_colorbase[i] = newbase;
                        dirty = 1;
                    }
                }
                if (dirty != 0)
                {
                    K056832_MarkAllTilemapsDirty();
                }
            }
            last_psac_colorbase = sub1_colorbase;
            sub1_colorbase = K055555_get_palette_index(5);
            if (last_psac_colorbase != sub1_colorbase)
            {
                ult_936_tilemap.all_tiles_dirty = true;
            }
            if (roz_enable != 0)
            {
                konamigx_mixer(Palette.bbitmap[Video.curbitmap], Video.new_clip, ult_936_tilemap, rozmode, 0, 0);
            }
            else
            {
                konamigx_mixer(Palette.bbitmap[Video.curbitmap], Video.new_clip, rozmode, 0, 0);
            }
        }
    }
}
