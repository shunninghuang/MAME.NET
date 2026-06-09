using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mame
{
    public partial class Kaneko
    {
        public static byte dsw1, dsw2;
        public static byte[] slaverom, devram, gfx1rom, gfx2rom, bank1, bank2, bank3, sharedram, slaveram;
        public static int basebankmaster, basebankslave, basebankaudio;
        public static int soundlatch_status, soundlatch2_status, master_addr, slave_addr;
        public static byte[] airbustr_videoram2, airbustr_colorram2;
        public static Tmap bg_tilemap, fg_tilemap;
        public static int bg_scrollx, bg_scrolly, fg_scrollx, fg_scrolly, highbits;
        public static void KanekoInit()
        {
            int i, n;
            Machine.bRom = true;
            switch (Machine.sName)
            {
                case "airbustr":
                case "airbustrj":
                case "airbustrb":
                    bank1 = new byte[0x20000];
                    bank2 = new byte[0x20000];
                    bank3 = new byte[0x20000];
                    Memory.mainram = new byte[0x1000];
                    sharedram = new byte[0x1000];
                    devram = new byte[0x1000];
                    slaveram = new byte[0x1a00];
                    Memory.audioram = new byte[0x2000];
                    airbustr_videoram2 = new byte[0x400];
                    airbustr_colorram2 = new byte[0x400];
                    Generic.videoram = new byte[0x400];
                    Generic.colorram = new byte[0x400];
                    Generic.paletteram = new byte[0x600];
                    Memory.mainrom = Machine.GetRom("master.rom");
                    slaverom = Machine.GetRom("slave.rom");
                    Memory.audiorom = Machine.GetRom("audiocpu.rom");
                    Array.Copy(Memory.mainrom, 0, bank1, 0, 0x20000);
                    Array.Copy(slaverom, 0, bank2, 0, 0x20000);
                    Array.Copy(Memory.audiorom, 0, bank3, 0, 0x20000);
                    gfx1rom = Machine.GetRom("gfx1.rom");
                    gfx2rom = Machine.GetRom("gfx2.rom");
                    n = gfx2rom.Length;
                    Drawgfx.spritecount = n / 0x100;
                    OKI6295.oo1[0] = new OKI6295();
                    OKI6295.oo1[0].okirom = Machine.GetRom("oki.rom");
                    if (Memory.mainrom == null || slaverom == null || Memory.audiorom == null || gfx1rom == null || gfx2rom == null || OKI6295.oo1[0].okirom == null)
                    {
                        Machine.bRom = false;
                    }
                    if (Machine.bRom)
                    {
                        dsw1 = 0xff;
                        dsw2 = 0xff;
                    }
                    break;
            }
        }
        public static byte dsw1_r(int offset)
        {
            return dsw1;
        }
        public static byte dsw2_r(int offset)
        {
            return dsw2;
        }
        public static byte devram_r(int offset)
        {
            switch (offset)
            {
                case 0xfe0:
                    return Generic.watchdog_reset_r();
                case 0xff2:
                case 0xff3:
                    {
                        int x = (devram[0xff0] + devram[0xff1] * 256) * (devram[0xff2] + devram[0xff3] * 256);
                        if (offset == 0xff2)
                        {
                            return (byte)((x & 0x00FF) >> 0);
                        }
                        else
                        {
                            return (byte)((x & 0xFF00) >> 8);
                        }
                    }
                    break;
                case 0xff4:
                    return (byte)Mame.mame_rand();
                default:
                    return devram[offset];
            }
        }
        public static void master_nmi_trigger_w()
        {
            Cpuint.cpunum_set_input_line(1, (int)LineState.INPUT_LINE_NMI, LineState.PULSE_LINE);
        }
        public static void master_bankswitch_w(byte data)
        {
            basebankmaster = 0x4000 * (data & 0x07);
        }
        public static void slave_bankswitch_w(byte data)
        {
            basebankslave = 0x4000 * (data & 0x07);
            Generic.flip_screen_set(data & 0x10);
            pandora_set_clear_bitmap(data & 0x20);
        }
        public static void sound_bankswitch_w(byte data)
        {
            basebankaudio = 0x4000 * (data & 0x07);
        }
        public static byte soundcommand_status_r()
        {
            return (byte)(4 + soundlatch_status * 2 + (1 - soundlatch2_status));
        }
        public static byte soundcommand_r()
        {
            soundlatch_status = 0;
            return (byte)Sound.soundlatch_r();
        }
        public static byte soundcommand2_r()
        {
            soundlatch2_status = 0;
            return (byte)Sound.soundlatch2_r();
        }
        public static void soundcommand_w(byte data)
        {
            Sound.soundlatch_w(data);
            soundlatch_status = 1;
            Cpuint.cpunum_set_input_line(2, (int)LineState.INPUT_LINE_NMI, LineState.PULSE_LINE);
        }
        public static void soundcommand2_w(byte data)
        {
            Sound.soundlatch2_w(data);
            soundlatch2_status = 1;
        }
        public static void airbustr_paletteram_w(int offset, byte data)
        {
            int val;
            Generic.paletteram[offset] = data;
            val = (Generic.paletteram[offset | 1] << 8) | Generic.paletteram[offset & ~1];
            Palette.palette_entry_set_color2(offset / 2, Palette.make_rgb(Palette.pal5bit((byte)(val >> 5)), Palette.pal5bit((byte)(val >> 10)), Palette.pal5bit((byte)(val >> 0))));
        }
        public static void airbustr_coin_counter_w(byte data)
        {
            Generic.coin_counter_w(0, data & 1);
            Generic.coin_counter_w(1, data & 2);
            Generic.coin_lockout_w(0, ~data & 4);
            Generic.coin_lockout_w(1, ~data & 8);
        }
        public static void master_interrupt()
        {
            master_addr ^= 0x02;
            Cpuint.cpunum_set_input_line_and_vector2(0, 0, LineState.HOLD_LINE, master_addr);
        }
        public static void slave_interrupt()
        {
            slave_addr ^= 0x02;
            Cpuint.cpunum_set_input_line_and_vector2(1, 0, LineState.HOLD_LINE, slave_addr);
        }
        public static void machine_reset_airbustr()
        {
            soundlatch_status = soundlatch2_status = 0;
            master_addr = 0xff;
            slave_addr = 0xfd;
            master_bankswitch_w(0x02);
            slave_bankswitch_w(0x02);
            sound_bankswitch_w(0x02);
        }
        public static void airbustr_videoram_w(int offset, byte data)
        {
            Generic.videoram[offset] = data;
            bg_tilemap.tilemap_mark_tile_dirty(offset);
        }
        public static void airbustr_colorram_w(int offset, byte data)
        {
            Generic.colorram[offset] = data;
            bg_tilemap.tilemap_mark_tile_dirty(offset);
        }
        public static void airbustr_videoram2_w(int offset, byte data)
        {
            airbustr_videoram2[offset] = data;
            fg_tilemap.tilemap_mark_tile_dirty(offset);
        }
        public static void airbustr_colorram2_w(int offset, byte data)
        {
            airbustr_colorram2[offset] = data;
            fg_tilemap.tilemap_mark_tile_dirty(offset);
        }
        public static void airbustr_scrollregs_w(int offset, byte data)
        {
            switch (offset)
            {
                case 0x00:
                    fg_scrolly = data;
                    break;
                case 0x02:
                    fg_scrollx = data;
                    break;
                case 0x04:
                    bg_scrolly = data;
                    break;
                case 0x06:
                    bg_scrollx = data;
                    break;
                case 0x08:
                    highbits = ~data;
                    break;
                default:
                    break;
            }
            bg_tilemap.tilemap_set_scrolly(0, ((highbits << 5) & 0x100) + bg_scrolly);
            bg_tilemap.tilemap_set_scrollx(0, ((highbits << 6) & 0x100) + bg_scrollx);
            fg_tilemap.tilemap_set_scrolly(0, ((highbits << 7) & 0x100) + fg_scrolly);
            fg_tilemap.tilemap_set_scrollx(0, ((highbits << 8) & 0x100) + fg_scrollx);
        }
        public static void video_start_airbustr()
        {
            int j;
            bitmap = new bitmap_t();
            bitmap.rowpixels = 0x100;
            bitmap.width = 0x100;
            bitmap.height = 0x100;
            bitmap.uu1 = new ushort[0x10000];
            bg_tilemap = Tmap.tilemap_create(Tmap.tilemap_scan_rows, 16, 16, 32, 32);
            fg_tilemap = Tmap.tilemap_create(Tmap.tilemap_scan_rows, 16, 16, 32, 32);
            bg_tilemap.pen_to_flags = new byte[1, 16];
            for (j = 0; j < 16; j++)
            {
                bg_tilemap.pen_to_flags[0, j] = 0x10;
            }
            bg_tilemap.tilemap_draw_instance3 = bg_tilemap.tilemap_draw_instance_capcom_sf;
            bg_tilemap.tile_update3 = bg_tilemap.tile_update_kaneko_bg;
            fg_tilemap.pen_to_flags = new byte[1, 16];
            fg_tilemap.pen_to_flags[0, 0] = 0;
            for (j = 1; j < 16; j++)
            {
                fg_tilemap.pen_to_flags[0, j] = 0x10;
            }
            fg_tilemap.tilemap_draw_instance3 = fg_tilemap.tilemap_draw_instance_capcom_sf;
            fg_tilemap.tile_update3 = fg_tilemap.tile_update_kaneko_fg;
            Tilemap.lsTmap = new List<Tmap>();
            Tilemap.lsTmap.Add(bg_tilemap);
            Tilemap.lsTmap.Add(fg_tilemap);
            pandora_start(1, 0, 0);
            bg_tilemap.tilemap_set_scrolldx(0x094, 0x06a);
            bg_tilemap.tilemap_set_scrolldy(0x100, 0x1ff);
            fg_tilemap.tilemap_set_scrolldx(0x094, 0x06a);
            fg_tilemap.tilemap_set_scrolldy(0x100, 0x1ff);
        }
        public static void video_update_airbustr()
        {
            bg_tilemap.tilemap_draw_primask(Video.new_clip, 0x10, 0);
            fg_tilemap.tilemap_draw_primask(Video.new_clip, 0x10, 0);
            pandora_update(Video.new_clip);
        }
        public static void video_eof_airbustr()
        {
            pandora_eof();
        }
    }
}
