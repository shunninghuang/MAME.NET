using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace mame
{
    public partial class Seibu
    {
        public static byte port1, port2, dswa, dswb;
        public static Tmap bg_tilemap;
        public static int tile_bank, sprite_bank;
        public static int flipscreen;
        public static int[] spritecount;
        public static int[] pribase = new int[4] { 0x0180, 0x0080, 0x0100, 0x0000 };
        public static byte[] kncljoe_scrollregs, tilesrom, promsrom;
        public static byte[][] spritesrom;
        public static bitmap_t bitmap;
        public static void SeibuInit()
        {
            int i, n;
            Machine.bRom = true;
            switch (Machine.sName)
            {
                case "kncljoe":
                case "kncljoea":
                case "bcrusher":
                    kncljoe_scrollregs = new byte[2];
                    spritecount = new int[2];
                    spritesrom = new byte[2][];
                    Memory.mainram = new byte[0x1000];
                    Memory.audioram = new byte[0x80];
                    Generic.videoram = new byte[0x1000];
                    Generic.spriteram = new byte[0x800];
                    Memory.mainrom = Machine.GetRom("maincpu.rom");
                    Memory.audiorom = Machine.GetRom("soundcpu.rom");
                    tilesrom = Machine.GetRom("tiles.rom");
                    spritesrom[0] = Machine.GetRom("sprites1.rom");
                    spritesrom[1] = Machine.GetRom("sprites2.rom");
                    spritecount[0] = spritesrom[0].Length / 0x100;
                    spritecount[1] = spritesrom[1].Length / 0x100;
                    promsrom = Machine.GetRom("proms.rom");
                    if (Memory.mainrom == null || Memory.audiorom == null || tilesrom == null || spritesrom[0] == null || spritesrom[1] == null || promsrom == null)
                    {
                        Machine.bRom = false;
                    }
                    if (Machine.bRom)
                    {
                        dswa = 0xff;
                        dswb = 0x7f;
                    }
                    break;
            }
        }
        public static void sound_cmd_w(byte data)
        {
            if ((data & 0x80) == 0)
            {
                Sound.soundlatch_w((ushort)(data & 0x7f));
            }
            else
            {
                Cpuint.cpunum_set_input_line(1, 0, LineState.ASSERT_LINE);
            }
        }
        public static void m6803_port1_w(byte data)
        {
            port1 = data;
        }
        public static void m6803_port2_w(byte data)
        {
            /*StreamWriter sw1 = new StreamWriter(@"\VS2008\compare1\compare1\bin\Debug\m2.txt", true);
            sw1.WriteLine(Sound.iCount.ToString("x") + "\t" + port1.ToString("x") + "\t" + port2.ToString("x") + "\t" + data.ToString("x"));
            sw1.Close();*/
            if (((port2 & 0x01) != 0) && ((data & 0x01) == 0))
            {
                if ((port2 & 0x04) != 0)
                {
                    if ((port2 & 0x08) != 0)
                    {
                        AY8910.AA8910[0].ay8910_write_ym(0, port1);
                    }
                }
                else
                {
                    if ((port2 & 0x08) != 0)
                    {
                        AY8910.AA8910[0].ay8910_write_ym(1, port1);
                    }
                }
            }
            port2 = data;
        }
        public static byte m6803_port1_r()
        {
            if ((port2 & 0x08) != 0)
            {
                return AY8910.AA8910[0].ay8910_read_ym();
            }
            return 0xff;
        }
        public static byte m6803_port2_r()
        {
            return 0;
        }
        public static void sound_irq_ack_w()
        {
            Cpuint.cpunum_set_input_line(1, 0, LineState.CLEAR_LINE);
        }
        public static void unused_w(int offset, byte data)
        {

        }
        public static void sound_nmi()
        {
            Cpuint.cpunum_set_input_line(1, (int)LineState.INPUT_LINE_NMI, LineState.PULSE_LINE);
        }
        public static void palette_init_kncljoe(byte[] color_prom)
        {
            int i, r, g, b, bit0, bit1, bit2;
            for (i = 0; i < 0x80; i++)
            {
                r = Palette.pal4bit(color_prom[i + 0x000]);
                g = Palette.pal4bit(color_prom[i + 0x100]);
                b = Palette.pal4bit(color_prom[i + 0x200]);
                Palette.palette_entry_set_color2(i, Palette.make_rgb(r, g, b));
            }
            for (i = 0x80; i < 0x90; i++)
            {
                bit0 = 0;
                bit1 = (color_prom[(i - 0x80) + 0x300] >> 6) & 0x01;
                bit2 = (color_prom[(i - 0x80) + 0x300] >> 7) & 0x01;
                r = 0x21 * bit0 + 0x47 * bit1 + 0x97 * bit2;
                bit0 = (color_prom[(i - 0x80) + 0x300] >> 3) & 0x01;
                bit1 = (color_prom[(i - 0x80) + 0x300] >> 4) & 0x01;
                bit2 = (color_prom[(i - 0x80) + 0x300] >> 5) & 0x01;
                g = 0x21 * bit0 + 0x47 * bit1 + 0x97 * bit2;
                bit0 = (color_prom[(i - 0x80) + 0x300] >> 0) & 0x01;
                bit1 = (color_prom[(i - 0x80) + 0x300] >> 1) & 0x01;
                bit2 = (color_prom[(i - 0x80) + 0x300] >> 2) & 0x01;
                b = 0x21 * bit0 + 0x47 * bit1 + 0x97 * bit2;
                Palette.palette_entry_set_color2(i, Palette.make_rgb(r, g, b));
            }
            for (i = 0; i < 0x80; i++)
            {
                Palette.entry_color2[i] = Palette.entry_color[i];
            }
            for (i = 0x80; i < 0x100; i++)
            {
                int ctabentry = (color_prom[0x320 + i - 0x80] & 0x0f) | 0x80;
                Palette.entry_color2[i] = Palette.entry_color[ctabentry];
            }
        }
        public static void video_start_kncljoe()
        {
            int j;
            bitmap = new bitmap_t();
            bitmap.rowpixels = 0x100;
            bitmap.width = 0x100;
            bitmap.height = 0x100;
            bg_tilemap = Tmap.tilemap_create(Tmap.tilemap_scan_rows, 8, 8, 64, 32);
            bg_tilemap.tilemap_set_scroll_rows(4);
            bg_tilemap.pen_to_flags = new byte[1, 16];
            for (j = 0; j < 16; j++)
            {
                bg_tilemap.pen_to_flags[0, j] = 0x10;
            }
            bg_tilemap.tilemap_draw_instance3 = bg_tilemap.tilemap_draw_instance_capcom_sf;
            bg_tilemap.tile_update3 = bg_tilemap.tile_update_seibu_kncljoe_bg;
            tile_bank = sprite_bank = flipscreen = 0;
            Tilemap.lsTmap = new List<Tmap>();
            Tilemap.lsTmap.Add(bg_tilemap);
        }
        public static void kncljoe_videoram_w(int offset, byte data)
        {
            Generic.videoram[offset] = data;
            bg_tilemap.tilemap_mark_tile_dirty(offset / 2);
        }
        public static void kncljoe_control_w(byte data)
        {
            int i;
            flipscreen = data & 0x01;
            Tmap.tilemap_set_flip(bg_tilemap, flipscreen != 0 ? Tilemap.TILEMAP_FLIPX : Tilemap.TILEMAP_FLIPY);
            Generic.coin_counter_w(0, data & 0x02);
            Generic.coin_counter_w(1, data & 0x20);
            i = (data & 0x10) >> 4;
            if (tile_bank != i)
            {
                tile_bank = i;
                Tmap.tilemap_mark_all_tiles_dirty(bg_tilemap);
            }
            i = (data & 0x04) >> 2;
            if (sprite_bank != i)
            {
                sprite_bank = i;
                Array.Clear(Memory.mainrom, 0xf100, 0x180);
            }
        }
        public static void kncljoe_scroll_w(int offset, byte data)
        {
            int scrollx;
            kncljoe_scrollregs[offset] = data;
            scrollx = kncljoe_scrollregs[0] | kncljoe_scrollregs[1] << 8;
            bg_tilemap.tilemap_set_scrollx(0, scrollx);
            bg_tilemap.tilemap_set_scrollx(1, scrollx);
            bg_tilemap.tilemap_set_scrollx(2, scrollx);
            bg_tilemap.tilemap_set_scrollx(3, 0);
        }
        public static void draw_sprites(RECT cliprect)
        {
            RECT clip = cliprect;
            int i, j;            
            if (flipscreen != 0)
            {
                if (clip.max_y > Video.screenstate.visarea.max_y - 64)
                {
                    clip.max_y = Video.screenstate.visarea.max_y - 64;
                }
            }
            else
            {
                if (clip.min_y < Video.screenstate.visarea.min_y + 64)
                {
                    clip.min_y = Video.screenstate.visarea.min_y + 64;
                }
            }
            for (i = 0; i < 4; i++)
            {
                for (j = 0x7c; j >= 0; j -= 4)
                {
                    int offs = pribase[i] + j;
                    int sy = Generic.spriteram[offs];
                    int sx = Generic.spriteram[offs + 3];
                    int code = Generic.spriteram[offs + 2];
                    int attr = Generic.spriteram[offs + 1];
                    int flipx = attr & 0x40;
                    int flipy = (attr & 0x80) == 0 ? 1 : 0;
                    int color = attr & 0x0f;
                    if ((attr & 0x10) != 0)
                    {
                        code += 512;
                    }
                    if ((attr & 0x20) != 0)
                    {
                        code += 256;
                    }
                    if (flipscreen != 0)
                    {
                        flipx = flipx == 0 ? 1 : 0;
                        flipy = flipy == 0 ? 1 : 0;
                        sx = 240 - sx;
                        sy = 240 - sy;
                    }
                    if (sx >= 256 - 8)
                    {
                        sx -= 256;
                    }
                    Drawgfx.common_drawgfx_kncljoe(bitmap, spritesrom[sprite_bank], code, color, flipx, flipy, sx, sy, clip);
                }
            }
        }
        public static void video_update_kncljoe()
        {
            bg_tilemap.tilemap_draw_primask(Video.new_clip, 0x10, 0);
            draw_sprites(Video.new_clip);
        }
    }
}
