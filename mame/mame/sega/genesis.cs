using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using cpu.m68000;

namespace mame
{
    public partial class sega
    {
        public static byte[] vdp_init=new byte[24]
	    {
		    0x04, 0x44, 0x30, 0x3C, 0x07, 0x6C, 0x00, 0x00,
		    0x00, 0x00, 0xFF, 0x00, 0x01, 0x37, 0x00, 0x02,
		    0x01, 0x00, 0x00, 0xFF, 0xFF, 0x00, 0x00, 0x80,
	    };
        public static byte[] is_important = new byte[32] { 0, 0, 1, 1, 1, 1, 0, 1, 0, 0, 0, 1, 0, 1, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public static ushort[] mask_table = new ushort[4] { 0x000, 0x007, 0xff8, 0xfff };
        public static byte[] size_table = new byte[4] { 32, 64, 128, 128 };
        public static ushort[] genesis_bg_pal_lookup = new ushort[4], genesis_sp_pal_lookup=new ushort[4];
        public static byte[] genesis_vdp_regs = new byte[0x20];

        public static byte[] vdp_vram, vdp_vsram;
        public static byte display_enable;
        public static int genesis_palette_base;
        public static ushort[] transparent_lookup;
        public static uint vdp_scrollabase, vdp_scrollbbase, vdp_windowbase, vdp_spritebase, vdp_hscrollbase;
        public static int vdp_hscrollmask;
        public static uint vdp_hscrollsize, vdp_address,window_vpos,window_hpos;
        public static byte vdp_vscrollmode, vdp_cmdpart, vdp_code, vdp_dmafill, scrollheight, scrollwidth, bgcol, window_down, window_right, window_width;


        public static int EXTRACT_PIXEL(uint x, int i)
        {
            return (int)(((x) >> (((i) ^ 1) * 4)) & 0x0f);
        }

        public static void start_genesis_vdp()
        {
            int i;
            vdp_vram = new byte[0x10000];
            vdp_vsram = new byte[0x80];
            transparent_lookup = new ushort[0x1000];
            for (i = 0; i < 0x1000; i++)
            {
                int orig_color = i & 0x7ff;
                int half_bright = i & 0x800;
                if ((orig_color & 0x100) != 0)
                {
                    transparent_lookup[i] = (ushort)orig_color;
                }
                else if (half_bright != 0)
                {
                    transparent_lookup[i] = (ushort)(orig_color | 0x800);
                }
                else
                {
                    transparent_lookup[i] = (ushort)(orig_color | 0x1000);
                }
            }
            genesis_palette_base = 0;
            genesis_bg_pal_lookup[0] = genesis_sp_pal_lookup[0] = 0x00;
            genesis_bg_pal_lookup[1] = genesis_sp_pal_lookup[1] = 0x10;
            genesis_bg_pal_lookup[2] = genesis_sp_pal_lookup[2] = 0x20;
            genesis_bg_pal_lookup[3] = genesis_sp_pal_lookup[3] = 0x30;
            for (i = 0; i < 24; i++)
            {
                vdp_register_w(0x8000 | (i << 8) | vdp_init[i], 1);
            }
            vdp_cmdpart = 0;
            vdp_code = 0;
            vdp_address = 0;
        }
        public static void video_start_genesis()
        {
            start_genesis_vdp();
        }
        public static void video_start_segac2()
        {
            start_genesis_vdp();
            genesis_sp_pal_lookup[0] = 0x100;
            genesis_sp_pal_lookup[1] = 0x110;
            genesis_sp_pal_lookup[2] = 0x120;
            genesis_sp_pal_lookup[3] = 0x130;
        }
        public static void system18_vdp_start()
        {
            start_genesis_vdp();
            genesis_palette_base = 0x1800;
            genesis_bg_pal_lookup[0] = genesis_sp_pal_lookup[0] = 0x1800;
            genesis_bg_pal_lookup[1] = genesis_sp_pal_lookup[1] = 0x1810;
            genesis_bg_pal_lookup[2] = genesis_sp_pal_lookup[2] = 0x1820;
            genesis_bg_pal_lookup[3] = genesis_sp_pal_lookup[3] = 0x1830;
        }

        public static void system18_vdp_update(bitmap_t bitmap, RECT cliprect)
        {
            int y;
            for (y = cliprect.min_y; y <= cliprect.max_y; y++)
            {
                drawline(bitmap, y, 0xffff);
            }
        }

        public static ushort genesis_vdp_r(int offset)
        {
            switch (offset)
            {
                case 0x00:
                case 0x01:
                    return (ushort)vdp_data_r();
                case 0x02:
                case 0x03:
                    return (ushort)vdp_control_r();
                case 0x04:
                case 0x05:
                case 0x06:
                case 0x07:
                    {
                        int xpos = Video.video_screen_get_hpos();
                        int ypos = Video.video_screen_get_vpos();
                        if (xpos > 0xe9)
                        {
                            xpos -= (342 - 0x100);
                        }
                        if (ypos > 0xea)
                        {
                            ypos -= (262 - 0x100);
                        }
                        if (ypos > 0)
                        {
                            ypos -= 2;
                        }
                        return (ushort)((ypos << 8) | xpos);
                    }
            }
            return 0;
        }
        public static void genesis_vdp_w(int offset, ushort data)
        {
            switch (offset)
            {
                case 0x00:
                case 0x01:
                    /*if (mem_mask != 0xffff)
                    {
                        data &= mem_mask;
                        if (ACCESSING_BITS_8_15)
                        {
                            data |= data >> 8;
                        }
                        else
                        {
                            data |= data << 8;
                        }
                    }*/
                    vdp_data_w(data);
                    break;
                case 0x02:
                case 0x03:
                    /*if (mem_mask != 0xffff)
                    {
                        data &= mem_mask;
                        if (ACCESSING_BITS_8_15)
                        {
                            data |= data >> 8;
                        }
                        else
                        {
                            data |= data << 8;
                        }
                    }*/
                    vdp_control_w(data);
                    break;
                case 0x08:
                case 0x09:
                case 0x0a:
                case 0x0b:
                    /*if (ACCESSING_BITS_0_7 && sndti_exists(SOUND_SN76496, 0))
                    {
                        sn76496_0_w(machine, 0, data & 0xff);
                    }*/
                    break;
            }
        }
        public static void genesis_vdp_w1(int offset, byte data)
        {
            int data2;
            switch (offset)
            {
                case 0x00:
                case 0x01:
                    data2 = data * 0x101;
                    vdp_data_w(data2);
                    break;
                case 0x02:
                case 0x03:
                    data2 = data * 0x101;
                    vdp_control_w(data2);
                    break;
                case 0x08:
                case 0x09:
                case 0x0a:
                case 0x0b:
                    /*if (ACCESSING_BITS_0_7 && sndti_exists(SOUND_SN76496, 0))
                    {
                        sn76496_0_w(machine, 0, data & 0xff);
                    }*/
                    break;
            }
        }
        public static void genesis_vdp_w2(int offset, byte data)
        {
            int data2;
            switch (offset)
            {
                case 0x00:
                case 0x01:
                    data2 = data * 0x101;
                    vdp_data_w(data2);
                    break;
                case 0x02:
                case 0x03:
                    data2 = data * 0x101;
                    vdp_control_w(data2);
                    break;
                case 0x08:
                case 0x09:
                case 0x0a:
                case 0x0b:
                    /*if (ACCESSING_BITS_0_7 && sndti_exists(SOUND_SN76496, 0))
                    {
                        sn76496_0_w(machine, 0, data & 0xff);
                    }*/
                    break;
            }
        }
        public static int vdp_data_r()
        {
            int read = 0;
            vdp_cmdpart = 0;
            switch (vdp_code & 0x0f)
            {
                case 0x00:
                    read = (vdp_vram[(vdp_address & ~1) & 0xffff] << 8) | vdp_vram[((vdp_address & ~1) + 1) & 0xffff];
                    break;
                case 0x04:
                    read = (vdp_vsram[(vdp_address & ~1) & 0x7f] << 8) | vdp_vsram[((vdp_address & ~1) + 1) & 0x7f];
                    break;
                default:
                    read = 0x00;
                    break;
            }
            vdp_address += genesis_vdp_regs[15];
            return read;
        }
        public static void vdp_data_w(int data)
        {
            vdp_cmdpart = 0;
            if (vdp_dmafill != 0)
            {
                vdp_dma_fill(data);
                vdp_dmafill = 0;
                return;
            }
            switch (vdp_code & 0x0f)
            {
                case 0x01:
                    if (vdp_address >= vdp_hscrollbase && vdp_address < vdp_hscrollbase + vdp_hscrollsize)
                    {
                        Video.video_screen_update_partial(Video.video_screen_get_vpos());
                    }
                    if ((vdp_address & 1) != 0)
                    {
                        data = ((data & 0xff) << 8) | ((data >> 8) & 0xff);
                    }
                    vdp_vram[(vdp_address & ~1) & 0xffff] = (byte)(data >> 8);
                    vdp_vram[(vdp_address | 1) & 0xffff] = (byte)data;
                    break;
                case 0x03:
                    {
                        int offset = (int)((vdp_address / 2) % 0x40);
                        Palette.palette_set_color_rgb(offset + genesis_palette_base, Palette.pal3bit((byte)(data >> 1)), Palette.pal3bit((byte)(data >> 5)), Palette.pal3bit((byte)(data >> 9)));
                    }
                    break;

                case 0x05:
                    Video.video_screen_update_partial(Video.video_screen_get_vpos());
                    if ((vdp_address & 1) != 0)
                    {
                        data = ((data & 0xff) << 8) | ((data >> 8) & 0xff);
                    }
                    vdp_vsram[(vdp_address & ~1) & 0x7f] = (byte)(data >> 8);
                    vdp_vsram[(vdp_address | 1) & 0x7f] = (byte)data;
                    break;
                default:
                    break;
            }
            vdp_address += genesis_vdp_regs[15];
        }
        public static int vdp_control_r()
        {
            int status = 0x3600;
            vdp_cmdpart = 0;
            if (Video.video_screen_get_vblank())
            {
                status |= 0x0008;
            }
            if (Video.video_screen_get_hblank())
            {
                status |= 0x0004;
            }
            return (status);
        }
        public static void vdp_control_w(int data)
        {
            if (vdp_cmdpart==0)
            {
                if ((data & 0xc000) == 0x8000)
                {
                    vdp_register_w(data, Video.video_screen_get_vblank()?1:0);
                }
                else
                {
                    vdp_code = (byte)((vdp_code & 0x3c) | ((data >> 14) & 0x03));
                    vdp_address = (uint)((vdp_address & 0xc000) | (uint)(data & 0x3fff));
                    vdp_cmdpart = 1;
                }
            }
            else
            {
                vdp_code = (byte)((vdp_code & 0x03) | ((data >> 2) & 0x3c));
                vdp_address = (uint)((vdp_address & 0x3fff) | (uint)((data << 14) & 0xc000));
                vdp_cmdpart = 0;
                vdp_control_dma(data);
            }
        }
        public static void vdp_register_w(int data, int vblank)
        {
            int scrwidth = 0;
            byte regnum = (byte)((data & 0x1f00) >> 8);
            byte regdat = (byte)(data & 0x00ff);
            genesis_vdp_regs[regnum] = regdat;
            if (is_important[regnum] != 0)
            {
                Video.video_screen_update_partial(Video.video_screen_get_vpos());
            }
            switch (regnum)
            {
                case 0x01:
                    break;
                case 0x02:
                    vdp_scrollabase = (uint)((regdat & 0x38) << 10);
                    break;
                case 0x03:
                    vdp_windowbase = (uint)((regdat & 0x3e) << 10);
                    break;
                case 0x04:
                    vdp_scrollbbase = (uint)((regdat & 0x07) << 13);
                    break;
                case 0x05:
                    vdp_spritebase = (uint)((regdat & 0x7e) << 9);
                    break;
                case 0x07:
                    bgcol = (byte)(regdat & 0x3f);
                    break;
                case 0x0b:
                    {
                        vdp_vscrollmode = (byte)((regdat & 0x04) >> 2);
                        vdp_hscrollmask = mask_table[regdat & 3];
                        vdp_hscrollsize = (uint)(4 * ((vdp_hscrollmask < 224) ? (vdp_hscrollmask + 1) : 224));
                        break;
                    }
                case 0x0c:
                    switch (regdat & 0x81)
                    {
                        case 0x00:
                            scrwidth = 32;
                            window_width = 32;
                            break;
                        case 0x01:
                            scrwidth = 40;
                            window_width = 64;
                            break;
                        case 0x80:
                            scrwidth = 40;
                            window_width = 64;
                            break;
                        case 0x81:
                            scrwidth = 40;
                            window_width = 64;
                            break;
                    }
                    {
                        int height = Video.screenstate.height;
                        RECT visarea = Video.screenstate.visarea;
                        long refresh = Video.screenstate.frame_period;
                        visarea.max_x = scrwidth * 8 - 1;
                        Video.video_screen_configure(scrwidth * 8, height, visarea, refresh);
                    }
                    break;
                case 0x0d:
                    vdp_hscrollbase = (uint)((regdat & 0x3f) << 10);
                    break;
                case 0x10:
                    {
                        scrollwidth = size_table[regdat & 0x03];
                        scrollheight = size_table[(regdat & 0x30) >> 4];
                        break;
                    }
                case 0x11:
                    window_right =(byte)(regdat & 0x80);
                    window_hpos = (uint)((regdat & 0x1f) << 4);
                    break;
                case 0x12:
                    window_down = (byte)(regdat & 0x80);
                    window_vpos = (uint)((regdat & 0x1f) << 3);
                    break;
            }
        }
        public static void vdp_control_dma(int data)
        {
            if ((vdp_code & 0x20)!=0 && (genesis_vdp_regs[1] & 0x10)!=0)
            {
                switch (genesis_vdp_regs[23] & 0xc0)
                {
                    case 0x00:
                    case 0x40:
                        vdp_dma_68k();
                        break;
                    case 0x80:
                        vdp_dmafill = 1;
                        break;
                    case 0xC0:
                        vdp_dma_copy();
                        break;
                }
            }
        }
        public static void vdp_dma_68k()
        {
            int length = genesis_vdp_regs[19] | (genesis_vdp_regs[20] << 8);
            int source = (genesis_vdp_regs[21] << 1) | (genesis_vdp_regs[22] << 9) | ((genesis_vdp_regs[23] & 0x7f) << 17);
            int count;
            if (length==0)
            {
                length = 0xffff;
            }
            for (count = 0; count < length; count++)
            {
                vdp_data_w(MC68000.mm1[0].ReadWord(source));
                source += 2;
            }
        }
        public static void vdp_dma_copy()
        {
            int length = genesis_vdp_regs[19] | (genesis_vdp_regs[20] << 8);
            int source = genesis_vdp_regs[21] | (genesis_vdp_regs[22] << 8);
            int count;
            if (length == 0)
            {
                length = 0xffff;
            }
            for (count = 0; count < length; count++)
            {
                vdp_vram[vdp_address & 0xffff] = vdp_vram[source & 0xffff];
                source++;
                vdp_address += genesis_vdp_regs[15];
            }
        }
        public static void vdp_dma_fill(int data)
        {
            int length = genesis_vdp_regs[19] | (genesis_vdp_regs[20] << 8);
            int count;
            if (length == 0)
            {
                length = 0xffff;
            }
            vdp_vram[vdp_address & 0xffff] = (byte)data;
            data >>= 8;
            for (count = 0; count < length; count++)
            {
                vdp_vram[(vdp_address ^ 1) & 0xffff] = (byte)data;
                vdp_address += genesis_vdp_regs[15];
            }
        }
        public static int vdp_gethscroll(int plane, int line)
        {
            line &= vdp_hscrollmask;
            return 0x400 - (((vdp_vram[(vdp_hscrollbase + (4 * line) + plane) & 0xffff] << 8) | vdp_vram[(vdp_hscrollbase + (4 * line) + plane + 1) & 0xffff]) & 0x3ff);
        }
        public static int vdp_getvscroll(int plane, int column)
        {
            uint vsramoffset;
            switch (vdp_vscrollmode)
            {
                case 0x00:
                    return ((vdp_vsram[plane & 0x7f] << 8) | vdp_vsram[(plane + 1) & 0x7f]) & 0x7ff;
                case 0x01:
                    if (column == 40)
                    {
                        column = 39;
                    }
                    vsramoffset = (uint)((4 * (column >> 1)) + plane);
                    return ((vdp_vsram[vsramoffset & 0x7f] << 8) | vdp_vsram[(vsramoffset + 1) & 0x7f]) & 0x7ff;
            }
            return 0;
        }

        public static void drawline(bitmap_t bitmap, int line, int bgfill)
        {
            int lowsprites, highsprites, link;
            uint[] scrolla_tiles = new uint[41], scrollb_tiles = new uint[41], window_tiles = new uint[41];
            int scrolla_offset, scrollb_offset;
            byte[] lowlist = new byte[81], highlist = new byte[81];
            int[] lowlist_offset = new int[81], highlist_offset = new int[81];
            int bgcolor = bgfill != 0 ? bgfill : genesis_bg_pal_lookup[0];
            int window_lclip, window_rclip;
            int scrolla_lclip, scrolla_rclip;
            int column, sprite;
            for (column = 0; column < 320; column++)
            {
                bitmap.uu1[line * bitmap.rowpixels + column] = (ushort)bgcolor;
            }
            if ((genesis_vdp_regs[1] & 0x40) == 0)
            {
                return;
            }
            link = lowsprites = highsprites = 0;
            for (sprite = 0; sprite < 80; sprite++)
            {
                int spritebase_offset = (int)((vdp_spritebase + 8 * link) & 0xffff);
                if ((vdp_vram[spritebase_offset + 4] & 0x0080) != 0)
                {
                    highlist_offset[++highsprites] = spritebase_offset;
                }
                else
                {
                    lowlist_offset[++lowsprites] = spritebase_offset;
                }
                link = vdp_vram[spritebase_offset+3] & 0x7F;
                if (link == 0)
                {
                    break;
                }
            }
            get_scroll_tiles(line, 2, vdp_scrollbbase, scrollb_tiles, out scrollb_offset);
            get_scroll_tiles(line, 0, vdp_scrollabase, scrolla_tiles, out scrolla_offset);
            get_window_tiles(line, vdp_windowbase, window_tiles);
            if ((window_down!=0 && line >= window_vpos) || (window_down==0 && line < window_vpos))
            {
                window_lclip = 0;
                window_rclip = 320 - 1;
            }
            else if (window_right!=0)
            {
                window_lclip = (int)window_hpos;
                window_rclip = 320 - 1;
            }
            else
            {
                window_lclip = 0;
                window_rclip = (int)(window_hpos - 1);
            }
            if (window_lclip == 0)
            {
                scrolla_lclip = window_rclip + 1;
                scrolla_rclip = 320 - 1;
            }
            else
            {
                scrolla_lclip = 0;
                scrolla_rclip = window_lclip - 1;
            }
            drawline_tiles(scrollb_tiles, bitmap, line, 0, scrollb_offset, 0, 320 - 1);
            drawline_tiles(scrolla_tiles, bitmap, line, 0, scrolla_offset, scrolla_lclip, scrolla_rclip);
            drawline_tiles(window_tiles, bitmap, line, 0, 0, window_lclip, window_rclip);
            for (sprite = lowsprites; sprite > 0; sprite--)
            {
                drawline_sprite(line, bitmap, 0, lowlist_offset[sprite]);
            }
            drawline_tiles(scrollb_tiles, bitmap, line, 1, scrollb_offset, 0, 320 - 1);
            drawline_tiles(scrolla_tiles, bitmap, line, 1, scrolla_offset, scrolla_lclip, scrolla_rclip);
            drawline_tiles(window_tiles, bitmap, line, 1, 0, window_lclip, window_rclip);
            for (sprite = highsprites; sprite > 0; sprite--)
            {
                drawline_sprite(line, bitmap, 1, highlist_offset[sprite]);
            }
        }
        public static void get_scroll_tiles(int line, int scrollnum, uint scrollbase, uint[] tiles, out int offset)
        {
            int linehscroll = vdp_gethscroll(scrollnum, line);
            int column, tiles_index = 0;
            offset = -(linehscroll % 8);
            linehscroll /= 8;
            for (column = 0; column < 41; column++)
            {
                int columnvscroll = vdp_getvscroll(scrollnum, (column - (linehscroll & 1)) & 0x3f) + line;
                int temp = ((columnvscroll / 8) & (scrollheight - 1)) * scrollwidth;
                int tilebase = (int)(scrollbase + 2 * temp);
                temp = (linehscroll + column) & (scrollwidth - 1);
                tilebase += 2 * temp;
                tiles[tiles_index++] = (uint)(((columnvscroll % 8) << 16) | ((vdp_vram[tilebase & 0xffff] << 8) | vdp_vram[(tilebase + 1) & 0xffff]));
            }
        }
        public static void get_window_tiles(int line, uint scrollbase, uint[] tiles)
        {
            int column, tiles_index = 0;
            for (column = 0; column < 40; column++)
            {
                int temp = (line / 8) * window_width + column;
                int tilebase = (int)(scrollbase + 2 * temp);
                tiles[tiles_index++] = (uint)(((line % 8) << 16) | ((vdp_vram[tilebase & 0xffff] << 8) | vdp_vram[(tilebase + 1) & 0xffff]));
            }
        }
        public static void drawline_tiles(uint[] tiles, bitmap_t bmap, int y, int pri, int offset, int lclip, int rclip)
        {
            int tiles_index = 0;
            uint tile;
            int bmap_offset = y * bmap.rowpixels;
            bmap_offset += offset;
            if (lclip > rclip)
            {
                return;
            }
            for (; offset < 320; offset += 8, bmap_offset += 8)
            {
                tile = tiles[tiles_index++];
                if (((tile >> 15) & 1) == pri && offset < 320)
                {
                    int colbase = genesis_bg_pal_lookup[(tile & 0x6000) >> 13];
                    int tp_offset = (int)((tile & 0x7ff) * 32);
                    uint mytile;
                    int col;
                    if ((tile & 0x1000) == 0)
                    {
                        mytile = (uint)(vdp_vram[tp_offset + (tile >> 16) * 4 + 3] * 0x1000000 + vdp_vram[tp_offset + (tile >> 16) * 4 + 2] * 0x10000 + vdp_vram[tp_offset + (tile >> 16) * 4 + 1] * 0x100 + vdp_vram[tp_offset + (tile >> 16) * 4]);
                    }
                    else
                    {
                        mytile = (uint)(vdp_vram[tp_offset + ((tile >> 16) ^ 7) * 4 + 3] * 0x1000000 + vdp_vram[tp_offset + ((tile >> 16) ^ 7) * 4 + 2] * 0x10000 + vdp_vram[tp_offset + ((tile >> 16) ^ 7) * 4 + 1] * 0x100 + vdp_vram[tp_offset + ((tile >> 16) ^ 7) * 4]);
                    }
                    if (mytile == 0)
                    {
                        continue;
                    }
                    if (offset >= lclip && offset <= rclip - 7)
                    {
                        if ((tile & 0x0800) == 0)
                        {
                            col = EXTRACT_PIXEL(mytile, 0); if (col != 0) bmap.uu1[bmap_offset + 0] = (ushort)(colbase + col);
                            col = EXTRACT_PIXEL(mytile, 1); if (col != 0) bmap.uu1[bmap_offset + 1] = (ushort)(colbase + col);
                            col = EXTRACT_PIXEL(mytile, 2); if (col != 0) bmap.uu1[bmap_offset + 2] = (ushort)(colbase + col);
                            col = EXTRACT_PIXEL(mytile, 3); if (col != 0) bmap.uu1[bmap_offset + 3] = (ushort)(colbase + col);
                            col = EXTRACT_PIXEL(mytile, 4); if (col != 0) bmap.uu1[bmap_offset + 4] = (ushort)(colbase + col);
                            col = EXTRACT_PIXEL(mytile, 5); if (col != 0) bmap.uu1[bmap_offset + 5] = (ushort)(colbase + col);
                            col = EXTRACT_PIXEL(mytile, 6); if (col != 0) bmap.uu1[bmap_offset + 6] = (ushort)(colbase + col);
                            col = EXTRACT_PIXEL(mytile, 7); if (col != 0) bmap.uu1[bmap_offset + 7] = (ushort)(colbase + col);
                        }
                        else
                        {
                            col = EXTRACT_PIXEL(mytile, 7); if (col != 0) bmap.uu1[bmap_offset + 0] = (ushort)(colbase + col);
                            col = EXTRACT_PIXEL(mytile, 6); if (col != 0) bmap.uu1[bmap_offset + 1] = (ushort)(colbase + col);
                            col = EXTRACT_PIXEL(mytile, 5); if (col != 0) bmap.uu1[bmap_offset + 2] = (ushort)(colbase + col);
                            col = EXTRACT_PIXEL(mytile, 4); if (col != 0) bmap.uu1[bmap_offset + 3] = (ushort)(colbase + col);
                            col = EXTRACT_PIXEL(mytile, 3); if (col != 0) bmap.uu1[bmap_offset + 4] = (ushort)(colbase + col);
                            col = EXTRACT_PIXEL(mytile, 2); if (col != 0) bmap.uu1[bmap_offset + 5] = (ushort)(colbase + col);
                            col = EXTRACT_PIXEL(mytile, 1); if (col != 0) bmap.uu1[bmap_offset + 6] = (ushort)(colbase + col);
                            col = EXTRACT_PIXEL(mytile, 0); if (col != 0) bmap.uu1[bmap_offset + 7] = (ushort)(colbase + col);
                        }
                    }
                    else if (offset >= lclip - 8 && offset <= rclip)
                    {
                        if ((tile & 0x0800) == 0)
                        {
                            col = EXTRACT_PIXEL(mytile, 0); if (col != 0 && (offset + 0) >= lclip && (offset + 0) <= rclip) bmap.uu1[bmap_offset + 0] = (ushort)(colbase + col);
                            col = EXTRACT_PIXEL(mytile, 1); if (col != 0 && (offset + 1) >= lclip && (offset + 1) <= rclip) bmap.uu1[bmap_offset + 1] = (ushort)(colbase + col);
                            col = EXTRACT_PIXEL(mytile, 2); if (col != 0 && (offset + 2) >= lclip && (offset + 2) <= rclip) bmap.uu1[bmap_offset + 2] = (ushort)(colbase + col);
                            col = EXTRACT_PIXEL(mytile, 3); if (col != 0 && (offset + 3) >= lclip && (offset + 3) <= rclip) bmap.uu1[bmap_offset + 3] = (ushort)(colbase + col);
                            col = EXTRACT_PIXEL(mytile, 4); if (col != 0 && (offset + 4) >= lclip && (offset + 4) <= rclip) bmap.uu1[bmap_offset + 4] = (ushort)(colbase + col);
                            col = EXTRACT_PIXEL(mytile, 5); if (col != 0 && (offset + 5) >= lclip && (offset + 5) <= rclip) bmap.uu1[bmap_offset + 5] = (ushort)(colbase + col);
                            col = EXTRACT_PIXEL(mytile, 6); if (col != 0 && (offset + 6) >= lclip && (offset + 6) <= rclip) bmap.uu1[bmap_offset + 6] = (ushort)(colbase + col);
                            col = EXTRACT_PIXEL(mytile, 7); if (col != 0 && (offset + 7) >= lclip && (offset + 7) <= rclip) bmap.uu1[bmap_offset + 7] = (ushort)(colbase + col);
                        }
                        else
                        {
                            col = EXTRACT_PIXEL(mytile, 7); if (col != 0 && (offset + 0) >= lclip && (offset + 0) <= rclip) bmap.uu1[bmap_offset + 0] = (ushort)(colbase + col);
                            col = EXTRACT_PIXEL(mytile, 6); if (col != 0 && (offset + 1) >= lclip && (offset + 1) <= rclip) bmap.uu1[bmap_offset + 1] = (ushort)(colbase + col);
                            col = EXTRACT_PIXEL(mytile, 5); if (col != 0 && (offset + 2) >= lclip && (offset + 2) <= rclip) bmap.uu1[bmap_offset + 2] = (ushort)(colbase + col);
                            col = EXTRACT_PIXEL(mytile, 4); if (col != 0 && (offset + 3) >= lclip && (offset + 3) <= rclip) bmap.uu1[bmap_offset + 3] = (ushort)(colbase + col);
                            col = EXTRACT_PIXEL(mytile, 3); if (col != 0 && (offset + 4) >= lclip && (offset + 4) <= rclip) bmap.uu1[bmap_offset + 4] = (ushort)(colbase + col);
                            col = EXTRACT_PIXEL(mytile, 2); if (col != 0 && (offset + 5) >= lclip && (offset + 5) <= rclip) bmap.uu1[bmap_offset + 5] = (ushort)(colbase + col);
                            col = EXTRACT_PIXEL(mytile, 1); if (col != 0 && (offset + 6) >= lclip && (offset + 6) <= rclip) bmap.uu1[bmap_offset + 6] = (ushort)(colbase + col);
                            col = EXTRACT_PIXEL(mytile, 0); if (col != 0 && (offset + 7) >= lclip && (offset + 7) <= rclip) bmap.uu1[bmap_offset + 7] = (ushort)(colbase + col);
                        }
                    }
                }
            }
        }
        public static void draw8pixs(bitmap_t bmap,int line, int sxpos, int patno, int priority, int colbase, int patline)
        {
            //UINT32 tile = *(UINT32*)&VDP_VRAM_BYTE(patno * 32 + 4 * patline);
            uint tile = (uint)(vdp_vram[patno * 32 + 4 * patline + 3] * 0x1000000 + vdp_vram[patno * 32 + 4 * patline + 2] * 0x10000 + vdp_vram[patno * 32 + 4 * patline + 1] * 0x100 + vdp_vram[patno * 32 + 4 * patline]);
            int col;
            int bmap_offset = line * bmap.rowpixels;
            if (tile == 0)
            {
                return;
            }
            if ((colbase & 0x30) != 0x30 || (genesis_vdp_regs[12] & 0x08)==0)
            {
                if (sxpos >= 0 && sxpos < 320) { col = EXTRACT_PIXEL(tile, 0); if (col != 0) bmap.uu1[bmap_offset + sxpos] = (ushort)(colbase + col); } sxpos++;
                if (sxpos >= 0 && sxpos < 320) { col = EXTRACT_PIXEL(tile, 1); if (col != 0) bmap.uu1[bmap_offset + sxpos] = (ushort)(colbase + col); } sxpos++;
                if (sxpos >= 0 && sxpos < 320) { col = EXTRACT_PIXEL(tile, 2); if (col != 0) bmap.uu1[bmap_offset + sxpos] = (ushort)(colbase + col); } sxpos++;
                if (sxpos >= 0 && sxpos < 320) { col = EXTRACT_PIXEL(tile, 3); if (col != 0) bmap.uu1[bmap_offset + sxpos] = (ushort)(colbase + col); } sxpos++;
                if (sxpos >= 0 && sxpos < 320) { col = EXTRACT_PIXEL(tile, 4); if (col != 0) bmap.uu1[bmap_offset + sxpos] = (ushort)(colbase + col); } sxpos++;
                if (sxpos >= 0 && sxpos < 320) { col = EXTRACT_PIXEL(tile, 5); if (col != 0) bmap.uu1[bmap_offset + sxpos] = (ushort)(colbase + col); } sxpos++;
                if (sxpos >= 0 && sxpos < 320) { col = EXTRACT_PIXEL(tile, 6); if (col != 0) bmap.uu1[bmap_offset + sxpos] = (ushort)(colbase + col); } sxpos++;
                if (sxpos >= 0 && sxpos < 320) { col = EXTRACT_PIXEL(tile, 7); if (col != 0) bmap.uu1[bmap_offset + sxpos] = (ushort)(colbase + col); } sxpos++;
            }
            else
            {
                if (sxpos >= 0 && sxpos < 320) { col = EXTRACT_PIXEL(tile, 0); if (col != 0) { if (col < 0x0e) bmap.uu1[bmap_offset + sxpos] = (ushort)(colbase + col); else bmap.uu1[bmap_offset + sxpos] = transparent_lookup[((col & 1) << 11) | (bmap.uu1[bmap_offset + sxpos] & 0x7ff)]; } } sxpos++;
                if (sxpos >= 0 && sxpos < 320) { col = EXTRACT_PIXEL(tile, 1); if (col != 0) { if (col < 0x0e) bmap.uu1[bmap_offset + sxpos] = (ushort)(colbase + col); else bmap.uu1[bmap_offset + sxpos] = transparent_lookup[((col & 1) << 11) | (bmap.uu1[bmap_offset + sxpos] & 0x7ff)]; } } sxpos++;
                if (sxpos >= 0 && sxpos < 320) { col = EXTRACT_PIXEL(tile, 2); if (col != 0) { if (col < 0x0e) bmap.uu1[bmap_offset + sxpos] = (ushort)(colbase + col); else bmap.uu1[bmap_offset + sxpos] = transparent_lookup[((col & 1) << 11) | (bmap.uu1[bmap_offset + sxpos] & 0x7ff)]; } } sxpos++;
                if (sxpos >= 0 && sxpos < 320) { col = EXTRACT_PIXEL(tile, 3); if (col != 0) { if (col < 0x0e) bmap.uu1[bmap_offset + sxpos] = (ushort)(colbase + col); else bmap.uu1[bmap_offset + sxpos] = transparent_lookup[((col & 1) << 11) | (bmap.uu1[bmap_offset + sxpos] & 0x7ff)]; } } sxpos++;
                if (sxpos >= 0 && sxpos < 320) { col = EXTRACT_PIXEL(tile, 4); if (col != 0) { if (col < 0x0e) bmap.uu1[bmap_offset + sxpos] = (ushort)(colbase + col); else bmap.uu1[bmap_offset + sxpos] = transparent_lookup[((col & 1) << 11) | (bmap.uu1[bmap_offset + sxpos] & 0x7ff)]; } } sxpos++;
                if (sxpos >= 0 && sxpos < 320) { col = EXTRACT_PIXEL(tile, 5); if (col != 0) { if (col < 0x0e) bmap.uu1[bmap_offset + sxpos] = (ushort)(colbase + col); else bmap.uu1[bmap_offset + sxpos] = transparent_lookup[((col & 1) << 11) | (bmap.uu1[bmap_offset + sxpos] & 0x7ff)]; } } sxpos++;
                if (sxpos >= 0 && sxpos < 320) { col = EXTRACT_PIXEL(tile, 6); if (col != 0) { if (col < 0x0e) bmap.uu1[bmap_offset + sxpos] = (ushort)(colbase + col); else bmap.uu1[bmap_offset + sxpos] = transparent_lookup[((col & 1) << 11) | (bmap.uu1[bmap_offset + sxpos] & 0x7ff)]; } } sxpos++;
                if (sxpos >= 0 && sxpos < 320) { col = EXTRACT_PIXEL(tile, 7); if (col != 0) { if (col < 0x0e) bmap.uu1[bmap_offset + sxpos] = (ushort)(colbase + col); else bmap.uu1[bmap_offset + sxpos] = transparent_lookup[((col & 1) << 11) | (bmap.uu1[bmap_offset + sxpos] & 0x7ff)]; } } sxpos++;
            }
        }
        public static void draw8pixs_hflip(bitmap_t bmap,int line, int sxpos, int patno, int priority, int colbase, int patline)
        {
            //UINT32 tile = *(UINT32*)&VDP_VRAM_BYTE(patno * 32 + 4 * patline);
            uint tile = (uint)(vdp_vram[patno * 32 + 4 * patline + 3] * 0x1000000 + vdp_vram[patno * 32 + 4 * patline + 2] * 0x10000 + vdp_vram[patno * 32 + 4 * patline + 1] * 0x100 + vdp_vram[patno * 32 + 4 * patline]);
            int col;
            int bmap_offset = line * bmap.rowpixels;
            if (tile == 0)
            {
                return;
            }
            if ((colbase & 0x30) != 0x30 || (genesis_vdp_regs[12] & 0x08)==0)
            {
                if (sxpos >= 0 && sxpos < 320) { col = EXTRACT_PIXEL(tile, 7); if (col != 0) bmap.uu1[bmap_offset + sxpos] = (ushort)(colbase + col); } sxpos++;
                if (sxpos >= 0 && sxpos < 320) { col = EXTRACT_PIXEL(tile, 6); if (col != 0) bmap.uu1[bmap_offset + sxpos] = (ushort)(colbase + col); } sxpos++;
                if (sxpos >= 0 && sxpos < 320) { col = EXTRACT_PIXEL(tile, 5); if (col != 0) bmap.uu1[bmap_offset + sxpos] = (ushort)(colbase + col); } sxpos++;
                if (sxpos >= 0 && sxpos < 320) { col = EXTRACT_PIXEL(tile, 4); if (col != 0) bmap.uu1[bmap_offset + sxpos] = (ushort)(colbase + col); } sxpos++;
                if (sxpos >= 0 && sxpos < 320) { col = EXTRACT_PIXEL(tile, 3); if (col != 0) bmap.uu1[bmap_offset + sxpos] = (ushort)(colbase + col); } sxpos++;
                if (sxpos >= 0 && sxpos < 320) { col = EXTRACT_PIXEL(tile, 2); if (col != 0) bmap.uu1[bmap_offset + sxpos] = (ushort)(colbase + col); } sxpos++;
                if (sxpos >= 0 && sxpos < 320) { col = EXTRACT_PIXEL(tile, 1); if (col != 0) bmap.uu1[bmap_offset + sxpos] = (ushort)(colbase + col); } sxpos++;
                if (sxpos >= 0 && sxpos < 320) { col = EXTRACT_PIXEL(tile, 0); if (col != 0) bmap.uu1[bmap_offset + sxpos] = (ushort)(colbase + col); } sxpos++;
            }
            else
            {
                if (sxpos >= 0 && sxpos < 320) { col = EXTRACT_PIXEL(tile, 7); if (col != 0) { if (col < 0x0e) bmap.uu1[bmap_offset + sxpos] = (ushort)(colbase + col); else bmap.uu1[bmap_offset + sxpos] = transparent_lookup[((col & 1) << 11) | (bmap.uu1[bmap_offset + sxpos] & 0x7ff)]; } } sxpos++;
                if (sxpos >= 0 && sxpos < 320) { col = EXTRACT_PIXEL(tile, 6); if (col != 0) { if (col < 0x0e) bmap.uu1[bmap_offset + sxpos] = (ushort)(colbase + col); else bmap.uu1[bmap_offset + sxpos] = transparent_lookup[((col & 1) << 11) | (bmap.uu1[bmap_offset + sxpos] & 0x7ff)]; } } sxpos++;
                if (sxpos >= 0 && sxpos < 320) { col = EXTRACT_PIXEL(tile, 5); if (col != 0) { if (col < 0x0e) bmap.uu1[bmap_offset + sxpos] = (ushort)(colbase + col); else bmap.uu1[bmap_offset + sxpos] = transparent_lookup[((col & 1) << 11) | (bmap.uu1[bmap_offset + sxpos] & 0x7ff)]; } } sxpos++;
                if (sxpos >= 0 && sxpos < 320) { col = EXTRACT_PIXEL(tile, 4); if (col != 0) { if (col < 0x0e) bmap.uu1[bmap_offset + sxpos] = (ushort)(colbase + col); else bmap.uu1[bmap_offset + sxpos] = transparent_lookup[((col & 1) << 11) | (bmap.uu1[bmap_offset + sxpos] & 0x7ff)]; } } sxpos++;
                if (sxpos >= 0 && sxpos < 320) { col = EXTRACT_PIXEL(tile, 3); if (col != 0) { if (col < 0x0e) bmap.uu1[bmap_offset + sxpos] = (ushort)(colbase + col); else bmap.uu1[bmap_offset + sxpos] = transparent_lookup[((col & 1) << 11) | (bmap.uu1[bmap_offset + sxpos] & 0x7ff)]; } } sxpos++;
                if (sxpos >= 0 && sxpos < 320) { col = EXTRACT_PIXEL(tile, 2); if (col != 0) { if (col < 0x0e) bmap.uu1[bmap_offset + sxpos] = (ushort)(colbase + col); else bmap.uu1[bmap_offset + sxpos] = transparent_lookup[((col & 1) << 11) | (bmap.uu1[bmap_offset + sxpos] & 0x7ff)]; } } sxpos++;
                if (sxpos >= 0 && sxpos < 320) { col = EXTRACT_PIXEL(tile, 1); if (col != 0) { if (col < 0x0e) bmap.uu1[bmap_offset + sxpos] = (ushort)(colbase + col); else bmap.uu1[bmap_offset + sxpos] = transparent_lookup[((col & 1) << 11) | (bmap.uu1[bmap_offset + sxpos] & 0x7ff)]; } } sxpos++;
                if (sxpos >= 0 && sxpos < 320) { col = EXTRACT_PIXEL(tile, 0); if (col != 0) { if (col < 0x0e) bmap.uu1[bmap_offset + sxpos] = (ushort)(colbase + col); else bmap.uu1[bmap_offset + sxpos] = transparent_lookup[((col & 1) << 11) | (bmap.uu1[bmap_offset + sxpos] & 0x7ff)]; } } sxpos++;
            }
        }
        public static void drawline_sprite(int line, bitmap_t bmap, int priority, int spritebase_offset)
        {
            int spriteypos = (((vdp_vram[spritebase_offset + 0] & 0x01) << 8) | vdp_vram[spritebase_offset + 1]) - 0x80;
            int spritexpos = (((vdp_vram[spritebase_offset + 6] & 0x01) << 8) | vdp_vram[spritebase_offset + 7]) - 0x80;
            int spriteheight = ((vdp_vram[spritebase_offset + 2] & 0x03) + 1) * 8;
            int spritewidth = (((vdp_vram[spritebase_offset + 2] & 0x0c) >> 2) + 1) * 8;
            int spriteattr, patno, patflip, patline, colbase, x;
            if (line < spriteypos || line >= spriteypos + spriteheight)
            {
                return;
            }
            if (spritexpos + spritewidth < 0 || spritexpos >= 320)
            {
                return;
            }
            spriteattr = (vdp_vram[spritebase_offset + 4] << 8) | vdp_vram[spritebase_offset + 5];
            patno = spriteattr & 0x07FF;
            patflip = (spriteattr & 0x1800) >> 11;
            patline = line - spriteypos;
            colbase = genesis_sp_pal_lookup[(spriteattr & 0x6000) >> 13];
            spritewidth >>= 3;
            spriteheight >>= 3;
            switch (patflip)
            {
                case 0x00:
                    for (x = 0; x < spritewidth; x++)
                    {
                        if (spritexpos >= -7 && spritexpos < 320)
                        {
                            draw8pixs(bmap, line, spritexpos, patno, priority, colbase, patline);
                        }
                        spritexpos += 8;
                        patno += spriteheight;
                    }
                    break;
                case 0x01:
                    patno += spriteheight * (spritewidth - 1);
                    for (x = 0; x < spritewidth; x++)
                    {
                        if (spritexpos >= -7 && spritexpos < 320)
                        {
                            draw8pixs_hflip(bmap, line, spritexpos, patno, priority, colbase, patline);
                        }
                        spritexpos += 8;
                        patno -= spriteheight;
                    }
                    break;
                case 0x02:
                    patline = 8 * spriteheight - patline - 1;
                    for (x = 0; x < spritewidth; x++)
                    {
                        if (spritexpos >= -7 && spritexpos < 320)
                        {
                            draw8pixs(bmap,line, spritexpos, patno, priority, colbase, patline);
                        }
                        spritexpos += 8;
                        patno += spriteheight;
                    }
                    break;
                case 0x03:
                    patno += spriteheight * (spritewidth - 1);
                    patline = 8 * spriteheight - patline - 1;
                    for (x = 0; x < spritewidth; x++)
                    {
                        if (spritexpos >= -7 && spritexpos < 320)
                        {
                            draw8pixs_hflip(bmap, line, spritexpos, patno, priority, colbase, patline);
                        }
                        spritexpos += 8;
                        patno -= spriteheight;
                    }
                    break;
            }
        }
        public static void SaveStateBinary_genesis(BinaryWriter writer)
        {
            int i;
            writer.Write(genesis_vdp_regs, 0, 0x20);
            writer.Write(vdp_vram, 0, 0x10000);
            writer.Write(vdp_vsram, 0, 0x80);
            writer.Write(display_enable);
            writer.Write(genesis_palette_base);
            for (i = 0; i < 0x1000; i++)
            {
                writer.Write(transparent_lookup[i]);
            }
            writer.Write(vdp_scrollabase);
            writer.Write(vdp_scrollbbase);
            writer.Write(vdp_windowbase);
            writer.Write(vdp_spritebase);
            writer.Write(vdp_hscrollbase);
            writer.Write(vdp_hscrollmask);
            writer.Write(vdp_hscrollsize);
            writer.Write(vdp_address);
            writer.Write(window_vpos);
            writer.Write(window_hpos);
            writer.Write(vdp_vscrollmode);
            writer.Write(vdp_cmdpart);
            writer.Write(vdp_code);
            writer.Write(vdp_dmafill);
            writer.Write(scrollheight);
            writer.Write(scrollwidth);
            writer.Write(bgcol);
            writer.Write(window_down);
            writer.Write(window_right);
            writer.Write(window_width);
        }
        public static void LoadStateBinary_genesis(BinaryReader reader)
        {
            int i;
            genesis_vdp_regs = reader.ReadBytes(0x20);
            vdp_vram = reader.ReadBytes(0x10000);
            vdp_vsram = reader.ReadBytes(0x80);
            display_enable = reader.ReadByte();
            genesis_palette_base = reader.ReadInt32();
            for (i = 0; i < 0x1000; i++)
            {
                transparent_lookup[i] = reader.ReadUInt16();
            }
            vdp_scrollabase = reader.ReadUInt32();
            vdp_scrollbbase = reader.ReadUInt32();
            vdp_windowbase = reader.ReadUInt32();
            vdp_spritebase = reader.ReadUInt32();
            vdp_hscrollbase = reader.ReadUInt32();
            vdp_hscrollmask = reader.ReadInt32();
            vdp_hscrollsize = reader.ReadUInt32();
            vdp_address = reader.ReadUInt32();
            window_vpos = reader.ReadUInt32();
            window_hpos = reader.ReadUInt32();
            vdp_vscrollmode = reader.ReadByte();
            vdp_cmdpart = reader.ReadByte();
            vdp_code = reader.ReadByte();
            vdp_dmafill = reader.ReadByte();
            scrollheight = reader.ReadByte();
            scrollwidth = reader.ReadByte();
            bgcol = reader.ReadByte();
            window_down = reader.ReadByte();
            window_right = reader.ReadByte();
            window_width = reader.ReadByte();
        }
    }
}
