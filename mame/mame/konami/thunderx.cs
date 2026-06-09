using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mame
{
    public partial class Konami
    {
        public static int palette_selected, m_priority;
        public static int rambank, pmcbank, basebankmain;
        public static byte[] ram, pmcram, bank1;
        public static byte m_1f98_latch;
        public static void scontra_interrupt()
        {
            if (K052109_is_IRQ_enabled() != 0)
            {
                Cpuint.cpunum_set_input_line(0, 0, LineState.HOLD_LINE);
            }
        }
        public static void thunderx_firq_callback()
        {
            Cpuint.cpunum_set_input_line(0, 1, LineState.HOLD_LINE);
        }
        public static byte thunderx_bankedram_r(int offset)
        {
            if ((rambank & 0x01) != 0)
            {
                return ram[offset];
            }
            else if ((rambank & 0x10) != 0)
            {
                if (pmcbank != 0)
                {
                    return pmcram[offset];
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return Generic.paletteram[offset];
            }
        }
        public static void thunderx_bankedram_w(int offset, byte data)
        {
            if ((rambank & 0x01) != 0)
            {
                ram[offset] = data;
            }
            else if ((rambank & 0x10) != 0)
            {
                if (pmcbank != 0)
                {
                    pmcram[offset] = data;
                }
            }
            else
            {
                Generic.paletteram_xBBBBBGGGGGRRRRR_be_w(offset, data);
            }
        }
        public static byte bankedram_r(int offset)
        {
            if (palette_selected != 0)
            {
                return Generic.paletteram[offset];
            }
            else
            {
                return ram[offset];
            }
        }
        public static void bankedram_w(int offset, byte data)
        {
            if (palette_selected != 0)
            {
                Generic.paletteram_xBBBBBGGGGGRRRRR_be_w(offset, data);
            }
            else
            {
                ram[offset] = data;
            }
        }
        public static void gbusters_1f98_w(byte data)
        {
            K052109_set_RMRD_line((data & 0x01) != 0 ? LineState.ASSERT_LINE : LineState.CLEAR_LINE);
        }
        public static void gbusters_coin_counter_w(byte data)
        {
            palette_selected = ~data & 0x01;
            Generic.coin_counter_w(0, data & 0x02);
            Generic.coin_counter_w(1, data & 0x04);
            m_priority = data & 0x08;
        }
        public static void gbusters_unknown_w()
        {

        }
        public static void gbusters_snd_bankswitch_w(byte data)
        {
            int bank_B = ((data >> 2) & 0x01);
            int bank_A = ((data) & 0x01);
            K007232.k007232_set_bank(0, bank_A, bank_B);
        }
        public static void run_collisions(int s0, int e0, int s1, int e1, int cm, int hm)
        {
            int ii, jj;
            int p0_offset, p1_offset;
            p0_offset = 16 + 5 * s0;
            for (ii = s0; ii < e0; ii++, p0_offset += 5)
            {
                int l0, r0, b0, t0;
                if ((pmcram[p0_offset] & cm) == 0)
                {
                    continue;
                }
                l0 = pmcram[p0_offset + 3] - pmcram[p0_offset + 1];
                r0 = pmcram[p0_offset + 3] + pmcram[p0_offset + 1];
                t0 = pmcram[p0_offset + 4] - pmcram[p0_offset + 2];
                b0 = pmcram[p0_offset + 4] + pmcram[p0_offset + 2];
                p1_offset = 16 + 5 * s1;
                for (jj = s1; jj < e1; jj++, p1_offset += 5)
                {
                    int l1, r1, b1, t1;
                    if ((pmcram[p1_offset] & hm) == 0)
                    {
                        continue;
                    }
                    l1 = pmcram[p1_offset + 3] - pmcram[p1_offset + 1];
                    r1 = pmcram[p1_offset + 3] + pmcram[p1_offset + 1];
                    t1 = pmcram[p1_offset + 4] - pmcram[p1_offset + 2];
                    b1 = pmcram[p1_offset + 4] + pmcram[p1_offset + 2];
                    if (l1 >= r0)
                    {
                        continue;
                    }
                    if (l0 >= r1)
                    {
                        continue;
                    }
                    if (t1 >= b0)
                    {
                        continue;
                    }
                    if (t0 >= b1)
                    {
                        continue;
                    }
                    pmcram[p0_offset] = (byte)((pmcram[p0_offset] & 0x9f) | (pmcram[p1_offset] & 0x04) | 0x10);
                    pmcram[p1_offset] = (byte)((pmcram[p1_offset] & 0x9f) | 0x10);
                }
            }
        }
        public static void calculate_collisions()
        {
            int X0, Y0;
            int X1, Y1;
            int CM, HM;
            Y0 = pmcram[0];
            Y0 = (Y0 << 8) + pmcram[1];
            Y0 = (Y0 - 15) / 5;
            Y1 = (pmcram[2] - 15) / 5;
            if (pmcram[5] < 16)
            {
                X0 = pmcram[5];
                X0 = (X0 << 8) + pmcram[6];
                X0 = (X0 - 16) / 5;
                X1 = (pmcram[7] - 16) / 5;
            }
            else
            {
                X0 = (pmcram[5] - 16) / 5;
                X1 = (pmcram[6] - 16) / 5;
            }
            CM = pmcram[3];
            HM = pmcram[4];
            run_collisions(X0, Y0, X1, Y1, CM, HM);
        }
        public static byte thunderx_1f98_r()
        {
            return m_1f98_latch;
        }
        public static void thunderx_1f98_w(byte data)
        {
            K052109_set_RMRD_line((data & 0x01) != 0 ? LineState.ASSERT_LINE : LineState.CLEAR_LINE);
            pmcbank = (data & 0x02) >> 1;
            if (((data & 4) != 0) && ((m_1f98_latch & 4) == 0))
            {
                calculate_collisions();
                Timer.emu_timer timer = Timer.timer_alloc_common(thunderx_firq_callback, "vblank_interrupt2", true);
                Timer.timer_adjust_periodic(timer, new Atime(0, (long)(100 * Cpuexec.cpu[0].attoseconds_per_cycle)), Attotime.ATTOTIME_NEVER);
            }
            m_1f98_latch = data;
        }
        public static void scontra_bankswitch_w(byte data)
        {
            basebankmain = (data & 0x0f) * 0x2000;
            palette_selected = ~data & 0x10;
            Generic.coin_counter_w(0, data & 0x20);
            Generic.coin_counter_w(1, data & 0x40);
            m_priority = data & 0x80;
        }
        public static void thunderx_videobank_w(byte data)
        {
            rambank = data;
            Generic.coin_counter_w(0, data & 0x02);
            Generic.coin_counter_w(1, data & 0x04);
            m_priority = data & 0x08;
        }
        public static void thunderx_sh_irqtrigger_w()
        {
            Cpuint.cpunum_set_input_line_and_vector2(1, 0, LineState.HOLD_LINE, 0xff);
        }
        public static void scontra_snd_bankswitch_w(byte data)
        {
            int bank_A = (data & 0x03);
            int bank_B = ((data >> 2) & 0x03);
            K007232.k007232_set_bank(0, bank_A, bank_B);
        }
        public static void thunderx_banking(int lines)
        {
            int offs;
            offs = (((lines & 0x0f) ^ 0x08) * 0x2000);
            if (offs >= 0x18000)
            {
                offs -= 0x10000;
            }
            basebankmain = offs;
        }
        public static void gbusters_banking(int lines)
        {
            int offs;
            offs = (lines & 0x0f) * 0x2000;
            basebankmain = offs;
        }
        public static void scontra_tile_callback(int layer, int bank, int code, int color, int flags, int priority, out int code2, out int color2, out int flags2)
        {
            flags2 = flags;
            code2 = code | ((color & 0x1f) << 8) | (bank << 13);
            color2 = layer_colorbase[layer] + ((color & 0xe0) >> 5);
        }
        public static void gbusters_tile_callback(int layer, int bank, int code, int color, int flags, int priority, out int code2, out int color2, out int flags2)
        {
            flags2 = flags;
            code2 = code | ((color & 0x0d) << 8) | ((color & 0x10) << 5) | (bank << 12);
            color2 = layer_colorbase[layer] + ((color & 0xe0) >> 5);
        }
        public static void scontra_sprite_callback(int code, int color, int priority, int shadow, out int code2, out int color2, out int priority2)
        {
            code2 = code;
            priority2 = priority;
            switch (color & 0x30)
            {
                case 0x00: priority2 = 0xf0; break;
                case 0x10: priority2 = 0xf0 | 0xcc | 0xaa; break;
                case 0x20: priority2 = 0xf0 | 0xcc; break;
                case 0x30: priority2 = 0xffff; break;
            }
            color2 = sprite_colorbase + (color & 0x0f);
        }
        public static void gbusters_sprite_callback(int code, int color, int priority, int shadow, out int code2, out int color2, out int priority2)
        {
            code2 = code;
            priority2 = (color & 0x30) >> 4;
            color2 = sprite_colorbase + (color & 0x0f);
        }
        public static void video_start_scontra()
        {
            layer_colorbase[0] = 48;
            layer_colorbase[1] = 0;
            layer_colorbase[2] = 16;
            sprite_colorbase = 32;
            K052109_vh_start(scontra_tile_callback);
            K051960_vh_start(scontra_sprite_callback);
        }
        public static void video_update_scontra()
        {
            K052109_tilemap_update();
            Array.Clear(Tilemap.priority_bitmap, 0, 0x20000);
            if (m_priority!=0)
            {
                K052109_tilemap[2].tilemap_draw_primask(Video.screenstate.visarea, 0, 1);
                K052109_tilemap[1].tilemap_draw_primask(Video.screenstate.visarea, 0x10, 2);
            }
            else
            {
                K052109_tilemap[1].tilemap_draw_primask(Video.screenstate.visarea, 0, 1);
                K052109_tilemap[2].tilemap_draw_primask(Video.screenstate.visarea, 0x10, 2);
            }
            K052109_tilemap[0].tilemap_draw_primask(Video.screenstate.visarea, 0x10, 4);
            K051960_sprites_draw(Video.screenstate.visarea, -1, -1);
        }
        public static void video_start_gbusters()
        {
            layer_colorbase[0] = 48;
            layer_colorbase[1] = 0;
            layer_colorbase[2] = 16;
            sprite_colorbase = 32;
            K052109_vh_start(gbusters_tile_callback);
            K051960_vh_start(gbusters_sprite_callback);
        }
        public static void video_updata_gbusters()
        {
            K052109_tilemap_update();
            if (m_priority != 0)
            {
                K052109_tilemap[2].tilemap_draw_primask(Video.screenstate.visarea, 0, 0);
                K051960_sprites_draw(Video.screenstate.visarea, 2, 2);
                K052109_tilemap[1].tilemap_draw_primask(Video.screenstate.visarea, 0x10, 0);
                K051960_sprites_draw(Video.screenstate.visarea, 0, 0);
                K052109_tilemap[0].tilemap_draw_primask(Video.screenstate.visarea, 0x10, 0);
            }
            else
            {
                K052109_tilemap[1].tilemap_draw_primask(Video.screenstate.visarea, 0, 0);
                K051960_sprites_draw(Video.screenstate.visarea, 2, 2);
                K052109_tilemap[2].tilemap_draw_primask(Video.screenstate.visarea, 0x10, 0);
                K051960_sprites_draw(Video.screenstate.visarea, 0, 0);
                K052109_tilemap[0].tilemap_draw_primask(Video.screenstate.visarea, 0x10, 0);
            }
        }
    }
}
