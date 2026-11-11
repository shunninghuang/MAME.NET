using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace mame
{
    public partial class sega
    {
        public static byte rom_board, mcu_data, lghost_value, lghost_select;
        public static byte[] misc_io_data, wwally_last_x, wwally_last_y;
        public static Func<int, ushort> custom_io_r;
        public static Action<int, ushort> custom_io_w;
        public static bitmap_t tempbitmap;
        private static ushort[] uu2000;
        public static byte grayscale_enable;
        public static byte vdp_enable, vdp_mixing;
        public static Action<byte> sound_w_callback;
        public static Func<byte> sound_r_callback;
        public static void sound_w_system18(byte data)
        {
            Sound.soundlatch_w((ushort)(data & 0xff));
            Cpuint.cpunum_set_input_line(1, (int)LineState.INPUT_LINE_NMI, LineState.PULSE_LINE);
        }
        public static byte sound_r_system18()
        {
            return mcu_data;
        }
        public static void system18_generic_init(int _rom_board)
        {
            rom_board = (byte)_rom_board;
            segaic16_spriteram_0 = new ushort[0x400];
            Generic.paletteram16 = new ushort[0x2000];
            segaic16_tileram_0 = new ushort[0x8000];
            segaic16_textram_0 = new ushort[0x800];
            Memory.mainram = new byte[0x4000];
            segaic16_memory_mapper_init();
            fd1094_driver_init(segaic16_memory_mapper_set_decrypted);
            custom_io_r = null;
            custom_io_w = null;
        }
        public static void boost_interleave()
        {
            Cpuexec.cpu_boost_interleave(Attotime.ATTOTIME_ZERO, Attotime.ATTOTIME_IN_MSEC(10));
        }
        public static void machine_reset_system18()
        {
            segaic16_memory_mapper_reset();
            segaic16_tilemap_reset(0);
            fd1094_machine_init();
            if (b8751)
            {
                Timer.timer_set_internal(boost_interleave, "boost_interleave");
            }
        }
        public static ushort io_chip_r(int offset)
        {
            offset &= 0x1f / 2;
            switch (offset)
            {
                case 0x00 / 2:
                case 0x02 / 2:
                case 0x04 / 2:
                case 0x06 / 2:
                case 0x08 / 2:
                case 0x0a / 2:
                case 0x0c / 2:
                case 0x0e / 2:
                    if ((misc_io_data[0x1e / 2] & (1 << offset)) != 0)
                    {
                        return misc_io_data[offset];
                    }
                    if (offset == 0)
                    {
                        return (byte)sbyte1;
                    }
                    else if (offset == 1)
                    {
                        return (byte)sbyte2;
                    }
                    else if (offset == 2)
                    {
                        return (byte)sbyte3;
                    }
                    else if (offset == 3)
                    {
                        return (byte)sbyte4;
                    }
                    else if(offset==4)
                    {
                        return (byte)sbytes;
                    }
                    else if (offset == 5)
                    {
                        return coinage;
                    }
                    else if (offset == 6)
                    {
                        return dsw;
                    }
                    else if (offset == 7)
                    {
                        return porth;
                    }
                    return 0;
                case 0x10 / 2:
                    return 'S';
                case 0x12 / 2:
                    return 'E';
                case 0x14 / 2:
                    return 'G';
                case 0x16 / 2:
                    return 'A';
                case 0x18 / 2:
                case 0x1c / 2:
                    return misc_io_data[0x1c / 2];
                case 0x1a / 2:
                case 0x1e / 2:
                    return misc_io_data[0x1e / 2];
            }
            return 0xffff;
        }
        public static void io_chip_w(int offset, ushort data)
        {
            byte old;
            offset &= 0x1f / 2;
            old = misc_io_data[offset];
            misc_io_data[offset] = (byte)data;
            switch (offset)
            {
                case 0x00 / 2:
                case 0x02 / 2:
                case 0x04 / 2:
                case 0x08 / 2:
                case 0x0a / 2:
                case 0x0c / 2:
                    break;
                case 0x06 / 2:
                    system18_set_grayscale(~data & 0x40);
                    segaic16_tilemap_set_flip(0, data & 0x20);
                    segaic16_sprites_set_flip(0, data & 0x20);
                    Generic.coin_counter_w(1, data & 0x02);
                    Generic.coin_counter_w(0, data & 0x01);
                    break;
                case 0x0e / 2:
                    if (rom_board == 2 || rom_board == 1)
                    {
                        int i;
                        for (i = 0; i < 4; i++)
                        {
                            segaic16_tilemap_set_bank(0, 0 + i, (data & 0xf) * 4 + i);
                            segaic16_tilemap_set_bank(0, 4 + i, ((data >> 4) & 0xf) * 4 + i);
                        }
                    }
                    break;
                case 0x1c / 2:
                    segaic16_set_display_enable(data & 2);
                    if (((old ^ data) & 4) != 0)
                    {
                        system18_set_vdp_enable(data & 4);
                    }
                    break;
            }
        }
        public static ushort misc_io_r(int offset)
        {
            offset &= 0x1fff;
            switch (offset & (0x3000 / 2))
            {
                case 0x0000 / 2:
                case 0x1000 / 2:
                    return io_chip_r(offset);
                case 0x2000 / 2:
                    if ((offset & 1) == 0)
                    {
                        return (byte)sbytes;
                    }
                    else if ((offset & 1) == 1)
                    {
                        return coinage;
                    }
                    break;
            }
            if (custom_io_r != null)
            {
                return custom_io_r(offset);
            }
            return segaic16_open_bus_r(0);
        }
        public static void misc_io_w(int offset, ushort data)
        {
            offset &= 0x1fff;
            switch (offset & (0x3000 / 2))
            {
                case 0x0000 / 2:
                case 0x1000 / 2:
                    //if (ACCESSING_BITS_0_7)
                    {
                        io_chip_w(offset, data);
                        return;
                    }
                    break;
                case 0x2000 / 2:
                    //if (ACCESSING_BITS_0_7)
                    {
                        system18_set_vdp_mixing(data & 0xff);
                        return;
                    }
                    break;
            }
            if (custom_io_w != null)
            {
                custom_io_w(offset, data);
                return;
            }
        }
        public static void misc_io_w1(int offset, byte data)
        {
            offset &= 0x1fff;
            if (custom_io_w != null)
            {
                custom_io_w(offset, data);
                return;
            }
        }
        public static void misc_io_w2(int offset, byte data)
        {
            offset &= 0x1fff;
            switch (offset & (0x3000 / 2))
            {
                case 0x0000 / 2:
                case 0x1000 / 2:
                    //if (ACCESSING_BITS_0_7)
                    {
                        io_chip_w(offset, data);
                        return;
                    }
                    break;
                case 0x2000 / 2:
                    //if (ACCESSING_BITS_0_7)
                    {
                        system18_set_vdp_mixing(data & 0xff);
                        return;
                    }
                    break;
            }
            if (custom_io_w != null)
            {
                custom_io_w(offset, data);
                return;
            }
        }
        public static void rom_5987_bank_w(int offset, ushort data)
        {
            //if (!ACCESSING_BITS_0_7)
            //	return;
            offset &= 0xf;
            data &= 0xff;
            if (offset < 8)
            {
                int maxbanks = (tilesrom.Length / 0x40) / 1024;
                if (data >= maxbanks)
                {
                    data %= (ushort)maxbanks;
                }
                segaic16_tilemap_set_bank(0, offset, data);
            }
            else
            {
                int maxbanks = spritesrom.Length / 0x20000;
                if (data >= maxbanks)
                {
                    data = 255;
                }
                segaic16_sprites_set_bank(0, (offset - 8) * 2 + 0, data * 2 + 0);
                segaic16_sprites_set_bank(0, (offset - 8) * 2 + 1, data * 2 + 1);
            }
        }
        public static void rom_5987_bank_w2(int offset, byte data)
        {
            offset &= 0xf;
            data &= 0xff;
            if (offset < 8)
            {
                int maxbanks = (tilesrom.Length / 0x40) / 1024;
                if (data >= maxbanks)
                {
                    data %= (byte)maxbanks;
                }
                segaic16_tilemap_set_bank(0, offset, data);
            }
            else
            {
                int maxbanks = spritesrom.Length / 0x20000;
                if (data >= maxbanks)
                {
                    data = 255;
                }
                segaic16_sprites_set_bank(0, (offset - 8) * 2 + 0, data * 2 + 0);
                segaic16_sprites_set_bank(0, (offset - 8) * 2 + 1, data * 2 + 1);
            }
        }
        public static void rom_837_7525_bank_w(int offset, ushort data)
        {
            //if (!ACCESSING_BITS_0_7)
            //	return;
            offset &= 0xf;
            data &= 0xff;
            if (offset < 8)
            {
                data &= 0x9f;
                if ((data & 0x80) != 0)
                {
                    data += 0x20;
                }
                data &= 0x3f;
                segaic16_tilemap_set_bank(0, offset, data);
            }
        }
        public static void rom_837_7525_bank_w2(int offset, byte data)
        {
            //if (!ACCESSING_BITS_0_7)
            //	return;
            offset &= 0xf;
            data &= 0xff;
            if (offset < 8)
            {
                data &= 0x9f;
                if ((data & 0x80) != 0)
                {
                    data += 0x20;
                }
                data &= 0x3f;
                segaic16_tilemap_set_bank(0, offset, data);
            }
        }
        public static ushort ddcrew_custom_io_r(int offset)
        {
            switch (offset)
            {
                case 0x3020 / 2:
                    return (byte)sbyte3;
                case 0x3022 / 2:
                    return (byte)sbyte4;
                case 0x3024 / 2:
                    return (byte)sbyte34;
            }
            return segaic16_open_bus_r(0);
        }
        public static ushort lghost_custom_io_r(int offset)
        {
            ushort result;
            switch (offset)
            {
                case 0x3010 / 2:
                case 0x3012 / 2:
                case 0x3014 / 2:
                case 0x3016 / 2:
                    result = (ushort)(lghost_value | 0x7f);
                    lghost_value <<= 1;
                    return result;
            }
            return segaic16_open_bus_r(0);
        }
        public static void lghost_custom_io_w(int offset, ushort data)
        {
            switch (offset)
            {
                case 0x3010 / 2:
                    //lghost_value = 255 - input_port_read(machine, "GUNY1");
                    break;
                case 0x3012 / 2:
                    //lghost_value = input_port_read(machine, "GUNX1");
                    break;
                case 0x3014 / 2:
                    //lghost_value = 255 - input_port_read(machine, lghost_select ? "GUNY3" : "GUNY2");
                    break;
                case 0x3016 / 2:
                    //lghost_value = input_port_read(machine, lghost_select ? "GUNX3" : "GUNX2");
                    break;
                case 0x3020 / 2:
                    lghost_select = (byte)(data & 1);
                    break;
            }
        }
        public static ushort wwally_custom_io_r(int offset)
        {
            switch (offset)
            {
                case 0x3000 / 2:
                    return (ushort)((Inptport.input_port_read_direct(Inptport.analog_p0) - wwally_last_x[0]) & 0xff);
                case 0x3004 / 2:
                    return (ushort)((Inptport.input_port_read_direct(Inptport.analog_p0) - wwally_last_y[0]) & 0xff);
                case 0x3008 / 2:
                    return 0;//(input_port_read(machine, "TRACKX2") - wwally_last_x[1]) & 0xff;
                case 0x300c / 2:
                    return 0;//(input_port_read(machine, "TRACKY2") - wwally_last_y[1]) & 0xff;
                case 0x3010 / 2:
                    return 0;//(input_port_read(machine, "TRACKX3") - wwally_last_x[2]) & 0xff;
                case 0x3014 / 2:
                    return 0;//(input_port_read(machine, "TRACKY3") - wwally_last_y[2]) & 0xff;
            }
            return segaic16_open_bus_r(0);
        }
        public static void wwally_custom_io_w(int offset, ushort data)
        {
            switch (offset)
            {
                case 0x3000 / 2:
                case 0x3004 / 2:
                    wwally_last_x[0] = (byte)Inptport.input_port_read_direct(Inptport.analog_p0);
                    wwally_last_y[0] = (byte)Inptport.input_port_read_direct(Inptport.analog_p1);
                    break;
                case 0x3008 / 2:
                case 0x300c / 2:
                    wwally_last_x[1] = 0;//input_port_read(machine, "TRACKX2");
                    wwally_last_y[1] = 0;//input_port_read(machine, "TRACKY2");
                    break;
                case 0x3010 / 2:
                case 0x3014 / 2:
                    wwally_last_x[2] = 0;//input_port_read(machine, "TRACKX3");
                    wwally_last_y[2] = 0;//input_port_read(machine, "TRACKY3");
                    break;
            }
        }

        public static void soundbank_w(byte data)
        {
            basebanksnd = 0x2000 * data;
        }
        public static void mcu_data_w(byte data)
        {
            mcu_data = data;
            Cpuint.cpunum_set_input_line(2, 1, LineState.PULSE_LINE);
        }

        public static void video_start_system18()
        {
            int i;
            int width, height;
            tempbitmap = null;
            grayscale_enable = 0;
            vdp_enable = 0;
            vdp_mixing = 0;
            segaic16_palette_init(0x800);
            segaic16_tilemap_init(0, 2, 0x000, 0, 8);
            segaic16_sprites_init(0, 2, 0x400, 0);
            uu2000 = new ushort[0x158 * 0x106];
            for (i = 0; i < 0x158 * 0x106; i++)
            {
                uu2000[i] = 0x2000;
            }
            system18_vdp_start();
            width = Video.screenstate.width;
            height = Video.screenstate.height;            
            tempbitmap = new bitmap_t();
            tempbitmap.width = width;
            tempbitmap.height = height;
            tempbitmap.rowpixels = width;
            tempbitmap.bpp = 16;
            tempbitmap.uu1 = new ushort[width * height];
        }

        public static void system18_set_grayscale(int enable)
        {
            enable = (enable != 0)?1:0;
            if (enable != grayscale_enable)
            {
                Video.video_screen_update_partial(Video.video_screen_get_vpos());
                grayscale_enable = (byte)enable;
            }
        }
        public static void system18_set_vdp_enable(int enable)
        {
            enable = (enable != 0)?1:0;
            if (enable != vdp_enable)
            {
                Video.video_screen_update_partial(Video.video_screen_get_vpos());
                vdp_enable = (byte)enable;
            }
        }
        public static void system18_set_vdp_mixing(int mixing)
        {
            if (mixing != vdp_mixing)
            {
                Video.video_screen_update_partial(Video.video_screen_get_vpos());
                vdp_mixing = (byte)mixing;
            }
        }
        public static void draw_vdp(bitmap_t bitmap, RECT cliprect, int priority)
        {
            int x, y;
            for (y = cliprect.min_y; y <= cliprect.max_y; y++)
            {
                for (x = cliprect.min_x; x <= cliprect.max_x; x++)
                {
                    ushort pix = tempbitmap.uu1[y * tempbitmap.rowpixels + x];
                    if (pix != 0xffff)
                    {
                        bitmap.uu1[y * bitmap.rowpixels + x] = pix;
                        Tilemap.priority_bitmap[y * Tilemap.screen_width + x] |= (byte)priority;
                    }
                }
            }
        }
        public static void video_update_system18()
        {
            int j;
            int vdppri, vdplayer;
            vdplayer = (vdp_mixing >> 1) & 3;
            vdppri = (vdp_mixing & 1) != 0 ? (1 << vdplayer) : 0;
            if (segaic16_display_enable == 0)
            {
                for (j = Video.new_clip.min_y; j <= Video.new_clip.max_y; j++)
                {
                    Array.Copy(uu2000, 0, Palette.bbitmap[Video.curbitmap].uu1, Palette.bbitmap[Video.curbitmap].rowpixels * j + Video.new_clip.min_x, Video.new_clip.max_x - Video.new_clip.min_x + 1);
                }
                return;
            }
            if (vdp_enable != 0)
            {
                system18_vdp_update(tempbitmap, Video.new_clip);
            }
            for (j = Video.new_clip.min_y; j <= Video.new_clip.max_y; j++)
            {
                Array.Copy(Tilemap.bb00, 0, Tilemap.priority_bitmap, Tilemap.screen_width * j + Video.new_clip.min_x, Video.new_clip.max_x - Video.new_clip.min_x + 1);
            }
            if (Video.screenstate.frame_number == 0x1b0)
            {
                int i1 = 1;
            }
            segaic16_tilemap_draw(0, Palette.bbitmap[Video.curbitmap], Video.new_clip, 1, 0 | 0, 0x00);//0x80
            segaic16_tilemap_draw(0, Palette.bbitmap[Video.curbitmap], Video.new_clip, 1, 1 | 0, 0x00);//0x80
            if (vdp_enable != 0 && vdplayer == 0)
            {
                draw_vdp(Palette.bbitmap[Video.curbitmap], Video.new_clip, vdppri);
            }
            segaic16_tilemap_draw(0, Palette.bbitmap[Video.curbitmap], Video.new_clip, 1, 0 | 0x10, 0x01);//
            segaic16_tilemap_draw(0, Palette.bbitmap[Video.curbitmap], Video.new_clip, 1, 1 | 0x10, 0x02);//
            if (vdp_enable != 0 && vdplayer == 1)
            {
                draw_vdp(Palette.bbitmap[Video.curbitmap], Video.new_clip, vdppri);
            }
            segaic16_tilemap_draw(0, Palette.bbitmap[Video.curbitmap], Video.new_clip, 0, 0 | 0x10, 0x02);
            segaic16_tilemap_draw(0, Palette.bbitmap[Video.curbitmap], Video.new_clip, 0, 1 | 0x10, 0x04);
            if (vdp_enable != 0 && vdplayer == 2)
            {
                draw_vdp(Palette.bbitmap[Video.curbitmap], Video.new_clip, vdppri);
            }
            segaic16_tilemap_draw(0, Palette.bbitmap[Video.curbitmap], Video.new_clip, 2, 0 | 0x10, 0x04);
            segaic16_tilemap_draw(0, Palette.bbitmap[Video.curbitmap], Video.new_clip, 2, 1 | 0x10, 0x08);
            if (vdp_enable != 0 && vdplayer == 3)
            {
                draw_vdp(Palette.bbitmap[Video.curbitmap], Video.new_clip, vdppri);
            }
            segaic16_sprites_draw(0, Palette.bbitmap[Video.curbitmap], Video.new_clip);
        }
    }
}
