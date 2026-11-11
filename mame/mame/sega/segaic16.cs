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
        public static byte segaic16_display_enable;
        public static ushort[] segaic16_tileram_0;
        public static ushort[] segaic16_textram_0;
        public static ushort[] segaic16_spriteram_0;
        public static ushort[] segaic16_spriteram_1;
        public static ushort[] segaic16_roadram_0;
        public static ushort[] segaic16_rotateram_0;
        public static int[] resistances_normal = new int[6] { 3900, 2000, 1000, 1000 / 2, 1000 / 4, 0 };
        public static int[] resistances_sh = new int[6] { 3900, 2000, 1000, 1000 / 2, 1000 / 4, 470 };
        public static palette_info palette;
        public static tilemap_info[] bg_tilemap = new tilemap_info[1];
        public static sprite_info[] sprites;//2
        //public static road_info[] road;//1
        //public static rotate_info[] rotate;//1
        public static multiply_chip[] multiply = new multiply_chip[3];
        public static divide_chip[] divide = new divide_chip[3];
        public static compare_timer_chip[] compare_timer = new compare_timer_chip[2];
        public static byte recurse = 0;

        public struct palette_info
        {
            public int entries;
            public byte[] normal;
            public byte[] shadow;
            public byte[] hilight;
        }
        public struct tilemap_callback_info
        {
            public ushort[] rambase;
            public int rambase_offset;
            public byte[] bank;
            public ushort banksize;
        }
        public struct tilemap_info
        {
            public byte index;
            public byte type;
            public byte numpages;
            public byte flip;
            public byte rowscroll, colscroll;
            public byte[] bank;//[8];
            public ushort banksize;
            public ushort[] latched_xscroll;//[4];
            public ushort[] latched_yscroll;//[4];
            public ushort[] latched_pageselect;//[4];
            public int xoffs;
            public Tmap[] tilemaps;//16
            public Tmap textmap;
            public tilemap_callback_info[] tilemap_info1;//[16];
            public tilemap_callback_info textmap_info;
            public Action reset;
            public draw_layer_delegate draw_layer;
            public ushort[] textram;
            public ushort[] tileram;
        }
        public struct multiply_chip
        {
            public ushort[] regs;//4
        }
        public struct divide_chip
        {
            public ushort[] regs;//8
        }
        public struct compare_timer_chip
        {
            public ushort[] regs;//[16];
            public ushort counter;
            public byte bit;
            //void	(*sound_w)(UINT8);
            //void	(*timer_ack)(running_machine *);
        }
        public struct memory_mapper_chip
        {
            public byte[] regs;//[0x20];
            //const char *cpu;
            //const struct segaic16_memory_map_entry *map;
            //void	(*sound_w)(UINT8);
            //UINT8	(*sound_r)(void);
        }
        public class sprite_info
        {
            public byte index;
            public byte type;
            public byte flip;
            public byte shadow;
            public byte[] bank;//[16];
            public ushort colorbase;
            public int ramsize;
            public int xoffs;
            public Action<bitmap_t, RECT> draw;
            public ushort[] spriteram;
            public int spriteram_offset;
            public ushort[] buffer;
            public void system16b_draw_pixel(bitmap_t bitmap, RECT cliprect, int sprpri, int pix, int color, int x, int y)
            {
                if (x >= cliprect.min_x && x <= cliprect.max_x && pix != 0 && pix != 15)
                {
                    if (sprpri > Tilemap.priority_bitmap[y * Tilemap.screen_width + x])
                    {
                        if (color == colorbase + (0x3f << 4))
                        {
                            bitmap.uu1[y * bitmap.rowpixels + x] += (ushort)((Generic.paletteram16[bitmap.uu1[y * bitmap.rowpixels + x]] & 0x8000) != 0 ? palette.entries * 2 : palette.entries);
                        }
                        else
                        {
                            bitmap.uu1[y * bitmap.rowpixels + x] = (ushort)(pix | color);
                        }
                        if (bitmap.uu1[y * bitmap.rowpixels + x] == 0x2036)
                        {
                            int i1 = 1;
                        }
                    }
                    Tilemap.priority_bitmap[y * Tilemap.screen_width + x] = 0xff;
                }
            }
            public void segaic16_sprites_16b_draw(bitmap_t bitmap, RECT cliprect)
            {
                byte numbanks;
                ushort[] spritebase;
                int data_offset;
                spritebase = spritesrom;
                if (spritebase == null)
                {
                    return;
                }
                numbanks = (byte)(spritesrom.Length / 0x10000);
                for (data_offset = 0; data_offset < ramsize / 2; data_offset += 8)
                {
                    if ((spriteram[data_offset + 2] & 0x8000) != 0)
                    {
                        break;
                    }
                }
                for (data_offset -= 8; data_offset >= 0; data_offset -= 8)
                {
                    int bottom = spriteram[data_offset + 0] >> 8;
                    int top = spriteram[data_offset + 0] & 0xff;
                    int xpos = (spriteram[data_offset + 1] & 0x1ff) - 0xb8;
                    int hide = spriteram[data_offset + 2] & 0x4000;
                    int flip1 = spriteram[data_offset + 2] & 0x100;
                    int pitch = (sbyte)(spriteram[data_offset + 2] & 0xff);
                    ushort addr = spriteram[data_offset + 3];
                    int bank1 = bank[(spriteram[data_offset + 4] >> 8) & 0xf];
                    int sprpri = 1 << ((spriteram[data_offset + 4] >> 6) & 0x3);
                    int color = colorbase + ((spriteram[data_offset + 4] & 0x3f) << 4);
                    int vzoom = (spriteram[data_offset + 5] >> 5) & 0x1f;
                    int hzoom = spriteram[data_offset + 5] & 0x1f;
                    int spritedata_offset;
                    int x, y, pix, xdelta = 1;
                    spriteram[data_offset + 7] = addr;
                    if (hide != 0 || (top >= bottom) || bank1 == 255)
                    {
                        continue;
                    }
                    if (numbanks != 0)
                    {
                        bank1 %= numbanks;
                    }
                    spritedata_offset = 0x10000 * bank1;
                    spriteram[data_offset + 5] &= 0x03ff;
                    if (flip != 0)
                    {
                        int temp = top;
                        top = 224 - bottom;
                        bottom = 224 - temp;
                        xpos = 320 - xpos;
                        xdelta = -1;
                    }
                    for (y = top; y < bottom; y++)
                    {
                        addr += (ushort)pitch;
                        spriteram[data_offset + 5] += (ushort)(vzoom << 10);
                        if ((spriteram[data_offset + 5] & 0x8000) != 0)
                        {
                            addr += (ushort)pitch;
                            spriteram[data_offset + 5] &= unchecked((ushort)~0x8000);
                        }
                        if (y >= cliprect.min_y && y <= cliprect.max_y)
                        {
                            int xacc;
                            xacc = 4 * hzoom;
                            if (flip1 == 0)
                            {
                                spriteram[data_offset + 7] = (ushort)(addr - 1);
                                for (x = xpos; ((xpos - x) & 0x1ff) != 1; )
                                {
                                    ushort pixels = spritebase[spritedata_offset + (++spriteram[data_offset + 7])];
                                    pix = (pixels >> 12) & 0xf;
                                    xacc = (xacc & 0x3f) + hzoom;
                                    if (xacc < 0x40)
                                    {
                                        system16b_draw_pixel(bitmap, cliprect, sprpri, pix, color, x, y);
                                        x += xdelta;
                                    }
                                    pix = (pixels >> 8) & 0xf;
                                    xacc = (xacc & 0x3f) + hzoom;
                                    if (xacc < 0x40)
                                    {
                                        system16b_draw_pixel(bitmap, cliprect, sprpri, pix, color, x, y);
                                        x += xdelta;
                                    }
                                    pix = (pixels >> 4) & 0xf;
                                    xacc = (xacc & 0x3f) + hzoom;
                                    if (xacc < 0x40)
                                    {
                                        system16b_draw_pixel(bitmap, cliprect, sprpri, pix, color, x, y);
                                        x += xdelta;
                                    }
                                    pix = (pixels >> 0) & 0xf;
                                    xacc = (xacc & 0x3f) + hzoom;
                                    if (xacc < 0x40)
                                    {
                                        system16b_draw_pixel(bitmap, cliprect, sprpri, pix, color, x, y);
                                        x += xdelta;
                                    }
                                    if (pix == 15)
                                    {
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                spriteram[data_offset + 7] = (ushort)(addr + 1);
                                for (x = xpos; ((xpos - x) & 0x1ff) != 1; )
                                {
                                    ushort pixels = spritebase[spritedata_offset + (--spriteram[data_offset + 7])];
                                    pix = (pixels >> 0) & 0xf;
                                    xacc = (xacc & 0x3f) + hzoom;
                                    if (xacc < 0x40)
                                    {
                                        system16b_draw_pixel(bitmap, cliprect, sprpri, pix, color, x, y);
                                        x += xdelta;
                                    }
                                    pix = (pixels >> 4) & 0xf;
                                    xacc = (xacc & 0x3f) + hzoom;
                                    if (xacc < 0x40)
                                    {
                                        system16b_draw_pixel(bitmap, cliprect, sprpri, pix, color, x, y);
                                        x += xdelta;
                                    }
                                    pix = (pixels >> 8) & 0xf;
                                    xacc = (xacc & 0x3f) + hzoom;
                                    if (xacc < 0x40)
                                    {
                                        system16b_draw_pixel(bitmap, cliprect, sprpri, pix, color, x, y);
                                        x += xdelta;
                                    }
                                    pix = (pixels >> 12) & 0xf;
                                    xacc = (xacc & 0x3f) + hzoom;
                                    if (xacc < 0x40)
                                    {
                                        system16b_draw_pixel(bitmap, cliprect, sprpri, pix, color, x, y);
                                        x += xdelta;
                                    }
                                    if (pix == 15)
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        public delegate void draw_layer_delegate(tilemap_info info, bitmap_t bitmap, RECT cliprect, int which, int flags, int priority);
        public static void segaic16_set_display_enable(int enable)
        {
            enable = (enable != 0) ? 1 : 0;
            if (segaic16_display_enable != enable)
            {
                Video.video_screen_update_partial(Video.video_screen_get_vpos());
                segaic16_display_enable = (byte)enable;
            }
        }
        public static void segaic16_palette_init(int entries)
        {
            int i;
            palette.normal = new byte[32];
            palette.shadow = new byte[32];
            palette.hilight = new byte[32];
            double[][] weights = new double[2][];
            for (i = 0; i < 2; i++)
            {
                weights[i] = new double[6];
            }
            palette.entries = entries;
            resnet.compute_resistor_weights(0, 255, -1.0, 6, resistances_normal, weights[0], 0, 0, 0, null, null, 0, 0, 0, null, null, 0, 0);
            resnet.compute_resistor_weights(0, 255, -1.0, 6, resistances_sh, weights[1], 0, 0, 0, null, null, 0, 0, 0, null, null, 0, 0);
            for (i = 0; i < 32; i++)
            {
                int i4 = (i >> 4) & 1;
                int i3 = (i >> 3) & 1;
                int i2 = (i >> 2) & 1;
                int i1 = (i >> 1) & 1;
                int i0 = (i >> 0) & 1;
                palette.normal[i] = resnet.combine_6_weights(weights[0], i0, i1, i2, i3, i4, 0);
                palette.shadow[i] = resnet.combine_6_weights(weights[1], i0, i1, i2, i3, i4, 0);
                palette.hilight[i] = resnet.combine_6_weights(weights[1], i0, i1, i2, i3, i4, 1);
            }
        }
        public static void segaic16_paletteram_w(int offset, ushort data)
        {
            ushort newval;
            int r, g, b;
            newval = Generic.paletteram16[offset];
            newval = data;
            Generic.paletteram16[offset] = newval;
            r = ((newval >> 12) & 0x01) | ((newval << 1) & 0x1e);
            g = ((newval >> 13) & 0x01) | ((newval >> 3) & 0x1e);
            b = ((newval >> 14) & 0x01) | ((newval >> 7) & 0x1e);
            Palette.palette_set_callback(offset + 0 * palette.entries, (uint)(palette.normal[r] * 0x10000 + palette.normal[g] * 0x100 + palette.normal[b]));
            Palette.palette_set_callback(offset + 1 * palette.entries, (uint)(palette.shadow[r] * 0x10000 + palette.shadow[g] * 0x100 + palette.shadow[b]));
            Palette.palette_set_callback(offset + 2 * palette.entries, (uint)(palette.hilight[r] * 0x10000 + palette.hilight[g] * 0x100 + palette.hilight[b]));
        }
        public static void segaic16_paletteram_w1(int offset, byte data)
        {
            ushort newval;
            int r, g, b;
            newval = Generic.paletteram16[offset];
            newval = (ushort)((data << 8) | (newval & 0xff));
            Generic.paletteram16[offset] = newval;
            r = ((newval >> 12) & 0x01) | ((newval << 1) & 0x1e);
            g = ((newval >> 13) & 0x01) | ((newval >> 3) & 0x1e);
            b = ((newval >> 14) & 0x01) | ((newval >> 7) & 0x1e);
            Palette.palette_set_callback(offset + 0 * palette.entries, (uint)(palette.normal[r] * 0x10000 + palette.normal[g] * 0x100 + palette.normal[b]));
            Palette.palette_set_callback(offset + 1 * palette.entries, (uint)(palette.shadow[r] * 0x10000 + palette.shadow[g] * 0x100 + palette.shadow[b]));
            Palette.palette_set_callback(offset + 2 * palette.entries, (uint)(palette.hilight[r] * 0x10000 + palette.hilight[g] * 0x100 + palette.hilight[b]));
        }
        public static void segaic16_paletteram_w2(int offset, byte data)
        {
            ushort newval;
            int r, g, b;
            newval = Generic.paletteram16[offset];
            newval = (ushort)((newval & 0xff00) | data);
            Generic.paletteram16[offset] = newval;
            r = ((newval >> 12) & 0x01) | ((newval << 1) & 0x1e);
            g = ((newval >> 13) & 0x01) | ((newval >> 3) & 0x1e);
            b = ((newval >> 14) & 0x01) | ((newval >> 7) & 0x1e);
            Palette.palette_set_callback(offset + 0 * palette.entries, (uint)(palette.normal[r] * 0x10000 + palette.normal[g] * 0x100 + palette.normal[b]));
            Palette.palette_set_callback(offset + 1 * palette.entries, (uint)(palette.shadow[r] * 0x10000 + palette.shadow[g] * 0x100 + palette.shadow[b]));
            Palette.palette_set_callback(offset + 2 * palette.entries, (uint)(palette.hilight[r] * 0x10000 + palette.hilight[g] * 0x100 + palette.hilight[b]));
        }
        public static void segaic16_draw_virtual_tilemap(tilemap_info info, bitmap_t bitmap, RECT cliprect, ushort pages, ushort xscroll, ushort yscroll, uint flags, uint priority)
        {
            int leftmin = -1, leftmax = -1, rightmin = -1, rightmax = -1;
            int topmin = -1, topmax = -1, bottommin = -1, bottommax = -1;
            RECT pageclip;
            int page;
            int width = Video.screenstate.width;
            int height = Video.screenstate.height;
            if (xscroll < 64 * 8 - width)
            {
                leftmin = 0;
                leftmax = width - 1;
                rightmin = -1;
            }
            else if (xscroll < 64 * 8)
            {
                leftmin = 0;
                leftmax = 64 * 8 - xscroll - 1;
                rightmin = leftmax + 1;
                rightmax = width - 1;
            }
            else if (xscroll < 128 * 8 - width)
            {
                rightmin = 0;
                rightmax = width - 1;
                leftmin = -1;
            }
            else
            {
                rightmin = 0;
                rightmax = 128 * 8 - xscroll - 1;
                leftmin = rightmax + 1;
                leftmax = width - 1;
            }
            if (yscroll < 32 * 8 - height)
            {
                topmin = 0;
                topmax = height - 1;
                bottommin = -1;
            }
            else if (yscroll < 32 * 8)
            {
                topmin = 0;
                topmax = 32 * 8 - yscroll - 1;
                bottommin = topmax + 1;
                bottommax = height - 1;
            }
            else if (yscroll < 64 * 8 - height)
            {
                bottommin = 0;
                bottommax = height - 1;
                topmin = -1;
            }
            else
            {
                bottommin = 0;
                bottommax = 64 * 8 - yscroll - 1;
                topmin = bottommax + 1;
                topmax = height - 1;
            }
            if (info.flip != 0)
            {
                if (leftmin != -1)
                {
                    int temp = leftmin;
                    leftmin = width - 1 - leftmax;
                    leftmax = width - 1 - temp;
                }
                if (rightmin != -1)
                {
                    int temp = rightmin;
                    rightmin = width - 1 - rightmax;
                    rightmax = width - 1 - temp;
                }
                if (topmin != -1)
                {
                    int temp = topmin;
                    topmin = height - 1 - topmax;
                    topmax = height - 1 - temp;
                }
                if (bottommin != -1)
                {
                    int temp = bottommin;
                    bottommin = height - 1 - bottommax;
                    bottommax = height - 1 - temp;
                }
            }
            if (leftmin != -1 && topmin != -1)
            {
                pageclip.min_x = (leftmin < cliprect.min_x) ? cliprect.min_x : leftmin;
                pageclip.max_x = (leftmax > cliprect.max_x) ? cliprect.max_x : leftmax;
                pageclip.min_y = (topmin < cliprect.min_y) ? cliprect.min_y : topmin;
                pageclip.max_y = (topmax > cliprect.max_y) ? cliprect.max_y : topmax;
                if (pageclip.min_x <= pageclip.max_x && pageclip.min_y <= pageclip.max_y)
                {
                    page = (pages >> 0) & 0xf;
                    info.tilemaps[page].tilemap_set_scrollx(0, xscroll);
                    info.tilemaps[page].tilemap_set_scrolly(0, yscroll);
                    info.tilemaps[page].tilemap_draw_primask(pageclip, (int)flags, (byte)priority);
                }
            }
            if (rightmin != -1 && topmin != -1)
            {
                pageclip.min_x = (rightmin < cliprect.min_x) ? cliprect.min_x : rightmin;
                pageclip.max_x = (rightmax > cliprect.max_x) ? cliprect.max_x : rightmax;
                pageclip.min_y = (topmin < cliprect.min_y) ? cliprect.min_y : topmin;
                pageclip.max_y = (topmax > cliprect.max_y) ? cliprect.max_y : topmax;
                if (pageclip.min_x <= pageclip.max_x && pageclip.min_y <= pageclip.max_y)
                {
                    page = (pages >> 4) & 0xf;
                    info.tilemaps[page].tilemap_set_scrollx(0, xscroll);
                    info.tilemaps[page].tilemap_set_scrolly(0, yscroll);
                    info.tilemaps[page].tilemap_draw_primask(pageclip, (int)flags, (byte)priority);
                }
            }
            if (leftmin != -1 && bottommin != -1)
            {
                pageclip.min_x = (leftmin < cliprect.min_x) ? cliprect.min_x : leftmin;
                pageclip.max_x = (leftmax > cliprect.max_x) ? cliprect.max_x : leftmax;
                pageclip.min_y = (bottommin < cliprect.min_y) ? cliprect.min_y : bottommin;
                pageclip.max_y = (bottommax > cliprect.max_y) ? cliprect.max_y : bottommax;
                if (pageclip.min_x <= pageclip.max_x && pageclip.min_y <= pageclip.max_y)
                {
                    page = (pages >> 8) & 0xf;
                    info.tilemaps[page].tilemap_set_scrollx(0, xscroll);
                    info.tilemaps[page].tilemap_set_scrolly(0, yscroll);
                    info.tilemaps[page].tilemap_draw_primask(pageclip, (int)flags, (byte)priority);
                }
            }
            if (rightmin != -1 && bottommin != -1)
            {
                pageclip.min_x = (rightmin < cliprect.min_x) ? cliprect.min_x : rightmin;
                pageclip.max_x = (rightmax > cliprect.max_x) ? cliprect.max_x : rightmax;
                pageclip.min_y = (bottommin < cliprect.min_y) ? cliprect.min_y : bottommin;
                pageclip.max_y = (bottommax > cliprect.max_y) ? cliprect.max_y : bottommax;
                if (pageclip.min_x <= pageclip.max_x && pageclip.min_y <= pageclip.max_y)
                {
                    page = (pages >> 12) & 0xf;
                    info.tilemaps[page].tilemap_set_scrollx(0, xscroll);
                    info.tilemaps[page].tilemap_set_scrolly(0, yscroll);
                    info.tilemaps[page].tilemap_draw_primask(pageclip, (int)flags, (byte)priority);
                }
            }
        }


        public static void segaic16_tilemap_16b_draw_layer(tilemap_info info, bitmap_t bitmap, RECT cliprect, int which, int flags, int priority)
        {
            //UINT16 *textram = info->textram;
            ushort[] textram = info.textram;
            ushort xscroll, yscroll, pages;
            int x, y;
            xscroll = info.latched_xscroll[which];
            yscroll = info.latched_yscroll[which];
            pages = info.latched_pageselect[which];
            if ((yscroll & 0x8000) != 0)
            {
                for (y = cliprect.min_y & ~7; y <= cliprect.max_y; y += 8)
                {
                    int rowscrollindex = (info.flip != 0 ? (216 - y) : y) / 8;
                    RECT rowcolclip;
                    rowcolclip.min_y = (y < cliprect.min_y) ? cliprect.min_y : y;
                    rowcolclip.max_y = (y + 7 > cliprect.max_y) ? cliprect.max_y : y + 7;
                    for (x = ((cliprect.min_x + 8) & ~15) - 8; x <= cliprect.max_x; x += 16)
                    {
                        ushort effxscroll, effyscroll, rowscroll;
                        ushort effpages = pages;
                        rowcolclip.min_x = (x < cliprect.min_x) ? cliprect.min_x : x;
                        rowcolclip.max_x = (x + 15 > cliprect.max_x) ? cliprect.max_x : x + 15;
                        rowscroll = textram[0xf80 / 2 + 0x40 / 2 * which + rowscrollindex];
                        effxscroll = ((xscroll & 0x8000) != 0) ? rowscroll : xscroll;
                        effyscroll = textram[0xf16 / 2 + 0x40 / 2 * which + (x + 8) / 16];
                        if ((rowscroll & 0x8000) != 0)
                        {
                            effxscroll = info.latched_xscroll[which + 2];
                            effyscroll = info.latched_yscroll[which + 2];
                            effpages = info.latched_pageselect[which + 2];
                        }
                        effxscroll = (ushort)((0xc0 - effxscroll + info.xoffs) & 0x3ff);
                        effyscroll = (ushort)(effyscroll & 0x1ff);
                        segaic16_draw_virtual_tilemap(info, bitmap, rowcolclip, effpages, effxscroll, effyscroll, (uint)flags, (uint)priority);
                    }
                }
            }
            else
            {
                for (y = cliprect.min_y & ~7; y <= cliprect.max_y; y += 8)
                {
                    int rowscrollindex = (info.flip != 0 ? (216 - y) : y) / 8;
                    RECT rowclip = cliprect;
                    ushort effxscroll, effyscroll, rowscroll;
                    ushort effpages = pages;
                    rowclip.min_y = (y < cliprect.min_y) ? cliprect.min_y : y;
                    rowclip.max_y = (y + 7 > cliprect.max_y) ? cliprect.max_y : y + 7;
                    rowscroll = textram[0xf80 / 2 + 0x40 / 2 * which + rowscrollindex];
                    effxscroll = ((xscroll & 0x8000) != 0) ? rowscroll : xscroll;
                    effyscroll = yscroll;
                    if ((rowscroll & 0x8000) != 0)
                    {
                        effxscroll = info.latched_xscroll[which + 2];
                        effyscroll = info.latched_yscroll[which + 2];
                        effpages = info.latched_pageselect[which + 2];
                    }
                    effxscroll = (ushort)((0xc0 - effxscroll + info.xoffs) & 0x3ff);
                    effyscroll = (ushort)(effyscroll & 0x1ff);
                    segaic16_draw_virtual_tilemap(info, bitmap, rowclip, effpages, effxscroll, effyscroll, (uint)flags, (uint)priority);
                }
            }
        }
        public static void segaic16_tilemap_16b_latch_values()
        {
            ushort[] textram = bg_tilemap[0].textram;
            int i;
            for (i = 0; i < 4; i++)
            {
                bg_tilemap[0].latched_pageselect[i] = textram[0xe80 / 2 + i];
                bg_tilemap[0].latched_yscroll[i] = textram[0xe90 / 2 + i];
                bg_tilemap[0].latched_xscroll[i] = textram[0xe98 / 2 + i];
            }
            Timer.emu_timer timer = Timer.timer_alloc_common(segaic16_tilemap_16b_latch_values, "segaic16_tilemap_16b_latch_values", true);
            Timer.timer_adjust_periodic(timer, Video.video_screen_get_time_until_pos(261, 0), Attotime.ATTOTIME_NEVER);
        }
        public static void segaic16_tilemap_16b_reset()
        {
            Timer.emu_timer timer = Timer.timer_alloc_common(segaic16_tilemap_16b_latch_values, "segaic16_tilemap_16b_latch_values", true);
            Timer.timer_adjust_periodic(timer, Video.video_screen_get_time_until_pos(261, 0), Attotime.ATTOTIME_NEVER);
        }
        public static void segaic16_tilemap_init(int which, int type, int colorbase, int xoffs, int numbanks)
        {
            bg_tilemap = new tilemap_info[1];
            bg_tilemap[0].bank = new byte[8];
            bg_tilemap[0].latched_xscroll=new ushort[4];
            bg_tilemap[0].latched_yscroll = new ushort[4];
            bg_tilemap[0].latched_pageselect = new ushort[4];
            bg_tilemap[0].tilemap_info1 = new tilemap_callback_info[16];
            //tilemap_info info = bg_tilemap[which];
            //tile_get_info_func get_text_info;
            //tile_get_info_func get_tile_info;
            int pagenum;
            int i;
            //memset(info, 0, sizeof(*info));
            bg_tilemap[which].index = (byte)which;
            bg_tilemap[which].type = (byte)type;
            for (i = 0; i < numbanks; i++)
            {
                bg_tilemap[which].bank[i] = (byte)i;
            }
            bg_tilemap[which].banksize = (ushort)(0x2000 / numbanks);
            bg_tilemap[which].xoffs = xoffs;
            switch (which)
            {
                case 0:
                    bg_tilemap[which].textram = segaic16_textram_0;
                    bg_tilemap[which].tileram = segaic16_tileram_0;
                    break;
                default:
                    break;
            }
            switch (type)
            {
                /*case 0:
                    get_text_info = segaic16_tilemap_16a_text_info;
                    get_tile_info = segaic16_tilemap_16a_tile_info;
                    info.numpages = 4;
                    info.draw_layer = segaic16_tilemap_16a_draw_layer;
                    info.reset = null;
                    break;
                case 1:
                    get_text_info = segaic16_tilemap_16a_text_info;
                    get_tile_info = segaic16_tilemap_16a_tile_info;
                    info.numpages = 8;
                    info.draw_layer = segaic16_tilemap_16a_draw_layer;
                    info.reset = null;
                    break;*/
                case 2:
                    //get_text_info = segaic16_tilemap_16b_text_info;
                    //get_tile_info = segaic16_tilemap_16b_tile_info;
                    bg_tilemap[which].numpages = 16;
                    bg_tilemap[which].draw_layer = segaic16_tilemap_16b_draw_layer;
                    bg_tilemap[which].reset = segaic16_tilemap_16b_reset;
                    break;
                /*case 3:
                    get_text_info = segaic16_tilemap_16b_alt_text_info;
                    get_tile_info = segaic16_tilemap_16b_alt_tile_info;
                    info.numpages = 16;
                    info.draw_layer = segaic16_tilemap_16b_draw_layer;
                    info.reset = segaic16_tilemap_16b_reset;
                    break;
                default:
                    break;*/
            }
            bg_tilemap[which].textmap = Tmap.tilemap_create(Tmap.tilemap_scan_rows, 8, 8, 64, 28);//get_text_info
            bg_tilemap[which].textmap_info.rambase = bg_tilemap[which].textram;
            bg_tilemap[which].textmap_info.bank = bg_tilemap[which].bank;
            bg_tilemap[which].textmap_info.banksize = bg_tilemap[which].banksize;
            //tilemap_set_user_data(info.textmap, &info.textmap_info);
            bg_tilemap[which].textmap.tilemap_set_palette_offset(colorbase);
            bg_tilemap[which].textmap.pen_to_flags = new byte[1, 16];
            bg_tilemap[which].textmap.pen_to_flags[0, 0] = 0;
            for (i = 1; i < 16; i++)
            {
                bg_tilemap[which].textmap.pen_to_flags[0, i] = 0x10;
            }
            bg_tilemap[which].textmap.tile_update3 = bg_tilemap[which].textmap.tile_update_sega_16b_text;
            bg_tilemap[which].textmap.tilemap_draw_instance3 = bg_tilemap[which].textmap.tilemap_draw_instance_sega_system18;
            bg_tilemap[which].textmap.tilemap_set_scrolldx(-192 + xoffs, -170 + xoffs);
            bg_tilemap[which].textmap.tilemap_set_scrolldy(0, 38);
            Tilemap.lsTmap.Add(bg_tilemap[which].textmap);
            bg_tilemap[which].tilemap_info1 = new tilemap_callback_info[16];
            bg_tilemap[which].tilemaps = new Tmap[16];
            for (pagenum = 0; pagenum < bg_tilemap[which].numpages; pagenum++)
            {
                bg_tilemap[which].tilemaps[pagenum] = Tmap.tilemap_create(Tmap.tilemap_scan_rows, 8, 8, 64, 32);//get_tile_info
                bg_tilemap[which].tilemap_info1[pagenum].rambase = bg_tilemap[which].tileram;
                bg_tilemap[which].tilemap_info1[pagenum].rambase_offset = pagenum * 64 * 32;
                bg_tilemap[which].tilemap_info1[pagenum].bank = bg_tilemap[which].bank;
                bg_tilemap[which].tilemap_info1[pagenum].banksize = bg_tilemap[which].banksize;
                //tilemap_set_user_data(info.tilemaps[pagenum], &info.tilemap_info[pagenum]);
                bg_tilemap[which].tilemaps[pagenum].tilemap_set_palette_offset(colorbase);
                bg_tilemap[which].tilemaps[pagenum].pen_to_flags = new byte[1, 16];
                bg_tilemap[which].tilemaps[pagenum].pen_to_flags[0, 0] = 0;
                for (i = 1; i < 16; i++)
                {
                    bg_tilemap[which].tilemaps[pagenum].pen_to_flags[0, i] = 0x10;
                }
                bg_tilemap[which].tilemaps[pagenum].tilemap_draw_instance3 = bg_tilemap[which].tilemaps[pagenum].tilemap_draw_instance_sega_system18;
                bg_tilemap[which].tilemaps[pagenum].tilemap_set_scrolldx(0, 22);
                bg_tilemap[which].tilemaps[pagenum].tilemap_set_scrolldy(0, 38);
                Tilemap.lsTmap.Add(bg_tilemap[which].tilemaps[pagenum]);
            }
            bg_tilemap[which].tilemaps[0x0].tile_update3 = bg_tilemap[which].tilemaps[0x0].tile_update_sega_16b_tile_0;
            bg_tilemap[which].tilemaps[0x1].tile_update3 = bg_tilemap[which].tilemaps[0x1].tile_update_sega_16b_tile_1;
            bg_tilemap[which].tilemaps[0x2].tile_update3 = bg_tilemap[which].tilemaps[0x2].tile_update_sega_16b_tile_2;
            bg_tilemap[which].tilemaps[0x3].tile_update3 = bg_tilemap[which].tilemaps[0x3].tile_update_sega_16b_tile_3;
            bg_tilemap[which].tilemaps[0x4].tile_update3 = bg_tilemap[which].tilemaps[0x4].tile_update_sega_16b_tile_4;
            bg_tilemap[which].tilemaps[0x5].tile_update3 = bg_tilemap[which].tilemaps[0x5].tile_update_sega_16b_tile_5;
            bg_tilemap[which].tilemaps[0x6].tile_update3 = bg_tilemap[which].tilemaps[0x6].tile_update_sega_16b_tile_6;
            bg_tilemap[which].tilemaps[0x7].tile_update3 = bg_tilemap[which].tilemaps[0x7].tile_update_sega_16b_tile_7;
            bg_tilemap[which].tilemaps[0x8].tile_update3 = bg_tilemap[which].tilemaps[0x8].tile_update_sega_16b_tile_8;
            bg_tilemap[which].tilemaps[0x9].tile_update3 = bg_tilemap[which].tilemaps[0x9].tile_update_sega_16b_tile_9;
            bg_tilemap[which].tilemaps[0xa].tile_update3 = bg_tilemap[which].tilemaps[0xa].tile_update_sega_16b_tile_a;
            bg_tilemap[which].tilemaps[0xb].tile_update3 = bg_tilemap[which].tilemaps[0xb].tile_update_sega_16b_tile_b;
            bg_tilemap[which].tilemaps[0xc].tile_update3 = bg_tilemap[which].tilemaps[0xc].tile_update_sega_16b_tile_c;
            bg_tilemap[which].tilemaps[0xd].tile_update3 = bg_tilemap[which].tilemaps[0xd].tile_update_sega_16b_tile_d;
            bg_tilemap[which].tilemaps[0xe].tile_update3 = bg_tilemap[which].tilemaps[0xe].tile_update_sega_16b_tile_e;
            bg_tilemap[which].tilemaps[0xf].tile_update3 = bg_tilemap[which].tilemaps[0xf].tile_update_sega_16b_tile_f;
        }
        public static void segaic16_tilemap_draw(int which, bitmap_t bitmap, RECT cliprect, int map, int priority, int priority_mark)
        {
            if (map == 2)
            {
                bg_tilemap[which].textmap.tilemap_draw_primask(cliprect, priority, (byte)priority_mark);
            }
            else
            {
                bg_tilemap[which].draw_layer(bg_tilemap[which], bitmap, cliprect, map, priority, priority_mark);
            }
        }
        public static void segaic16_tilemap_reset(int which)
        {
            if (bg_tilemap[which].reset != null)
            {
                bg_tilemap[which].reset();
            }
        }
        public static void segaic16_tilemap_set_bank(int which, int banknum, int offset)
        {
            if (bg_tilemap[which].bank[banknum] != offset)
            {
                Video.video_screen_update_partial(Video.video_screen_get_vpos());
                bg_tilemap[which].bank[banknum] = (byte)offset;
                Tmap.tilemap_mark_all_tiles_dirty_alltilemap();
            }
        }
        public static void segaic16_tilemap_set_flip(int which, int flip)
        {
            int pagenum;
            flip = (flip != 0) ? 1 : 0;
            if (bg_tilemap[which].flip != flip)
            {
                Video.video_screen_update_partial(Video.video_screen_get_vpos());
                bg_tilemap[which].flip = (byte)flip;
                bg_tilemap[which].textmap.tilemap_set_flip((byte)(flip != 0 ? (Tilemap.TILEMAP_FLIPX | Tilemap.TILEMAP_FLIPY) : 0));
                for (pagenum = 0; pagenum < bg_tilemap[which].numpages; pagenum++)
                {
                    bg_tilemap[which].tilemaps[pagenum].tilemap_set_flip((byte)(flip != 0 ? (Tilemap.TILEMAP_FLIPX | Tilemap.TILEMAP_FLIPY) : 0));
                }
            }
        }
        public static void segaic16_tilemap_set_rowscroll(int which, int enable)
        {
            enable = (enable != 0) ? 1 : 0;
            if (bg_tilemap[which].rowscroll != enable)
            {
                Video.video_screen_update_partial(Video.video_screen_get_vpos());
                bg_tilemap[which].rowscroll = (byte)enable;
            }
        }
        public static void segaic16_tilemap_set_colscroll(int which, int enable)
        {
            enable = (enable != 0) ? 1 : 0;
            if (bg_tilemap[which].colscroll != enable)
            {
                Video.video_screen_update_partial(Video.video_screen_get_vpos());
                bg_tilemap[which].colscroll = (byte)enable;
            }
        }
        public static void segaic16_tileram_0_w(int offset, ushort data)
        {
            segaic16_tileram_0[offset] = data;
            bg_tilemap[0].tilemaps[offset / (64 * 32)].tilemap_mark_tile_dirty(offset % (64 * 32));
        }
        public static void segaic16_tileram_0_w1(int offset, byte data)
        {
            segaic16_tileram_0[offset] = (ushort)((data << 8) | (segaic16_tileram_0[offset] & 0xff));
            bg_tilemap[0].tilemaps[offset / (64 * 32)].tilemap_mark_tile_dirty(offset % (64 * 32));
        }
        public static void segaic16_tileram_0_w2(int offset, byte data)
        {
            segaic16_tileram_0[offset] = (ushort)((segaic16_tileram_0[offset] & 0xff00) | data);
            bg_tilemap[0].tilemaps[offset / (64 * 32)].tilemap_mark_tile_dirty(offset % (64 * 32));
        }
        public static void segaic16_textram_0_w(int offset, ushort data)
        {
            if (offset >= 0xe80 / 2)
            {
                Video.video_screen_update_partial(Video.video_screen_get_vpos());
            }
            segaic16_textram_0[offset] = data;
            bg_tilemap[0].textmap.tilemap_mark_tile_dirty(offset);
        }
        public static void segaic16_textram_0_w1(int offset, byte data)
        {
            if (offset >= 0xe80 / 2)
            {
                Video.video_screen_update_partial(Video.video_screen_get_vpos());
            }
            segaic16_textram_0[offset] = (ushort)((data << 8) | (segaic16_textram_0[offset] & 0xff));
            bg_tilemap[0].textmap.tilemap_mark_tile_dirty(offset);
        }
        public static void segaic16_textram_0_w2(int offset, byte data)
        {
            if (offset >= 0xe80 / 2)
            {
                Video.video_screen_update_partial(Video.video_screen_get_vpos());
            }
            segaic16_textram_0[offset] = (ushort)((segaic16_textram_0[offset] & 0xff00) | data);
            bg_tilemap[0].textmap.tilemap_mark_tile_dirty(offset);
        }


        public static void segaic16_sprites_init(int which, int type, int colorbase, int xoffs)
        {
            int i, buffer = 0;
            sprites = new sprite_info[2];
            sprites[which] = new sprite_info();
            sprites[which].bank = new byte[16];
            sprites[which].index = (byte)which;
            sprites[which].type = (byte)type;
            for (i = 0; i < 16; i++)
            {
                sprites[which].bank[i] = (byte)i;
            }
            sprites[which].colorbase = (ushort)colorbase;
            sprites[which].xoffs = xoffs;
            switch (which)
            {
                case 0:
                    sprites[which].spriteram = segaic16_spriteram_0;
                    break;
                case 1:
                    sprites[which].spriteram = segaic16_spriteram_1;
                    break;
                default:
                    break;
            }
            switch (type)
            {
                /*case 0:
                    sprites[which].draw = segaic16_sprites_hangon_draw;
                    sprites[which].ramsize = 0x800;
                    break;
                case 1:
                    sprites[which].draw = segaic16_sprites_16a_draw;
                    sprites[which].ramsize = 0x800;
                    break;*/
                case 2:
                    sprites[which].draw = sprites[which].segaic16_sprites_16b_draw;
                    sprites[which].ramsize = 0x800;
                    break;
                /*case 3:
                    sprites[which].draw = segaic16_sprites_sharrier_draw;
                    sprites[which].ramsize = 0x1000;
                    break;
                case 4:
                case 5:
                    sprites[which].draw = segaic16_sprites_outrun_draw;
                    sprites[which].ramsize = 0x1000;
                    buffer = 1;
                    break;
                case 6:
                    sprites[which].draw = segaic16_sprites_yboard_draw;
                    sprites[which].ramsize = 0x10000;
                    break;
                case 7:
                    sprites[which].draw = segaic16_sprites_yboard_16b_draw;
                    sprites[which].ramsize = 0x800;
                    break;
                default:
                    break;*/
            }
            if (buffer != 0)
            {
                sprites[which].buffer = new ushort[sprites[which].ramsize];
            }
        }
        public static void segaic16_sprites_draw(int which, bitmap_t bitmap, RECT cliprect)
        {
            sprites[which].draw(bitmap, cliprect);
        }
        public static void segaic16_sprites_set_bank(int which, int banknum, int offset)
        {
            if (sprites[which].bank[banknum] != offset)
            {
                Video.video_screen_update_partial(Video.video_screen_get_vpos());
                sprites[which].bank[banknum] = (byte)offset;
            }
        }
        public static void segaic16_sprites_set_flip(int which, int flip)
        {
            flip = (flip != 0) ? 1 : 0;
            if (sprites[which].flip != flip)
            {
                Video.video_screen_update_partial(Video.video_screen_get_vpos());
                sprites[which].flip = (byte)flip;
            }
        }
        public static void segaic16_sprites_set_shadow(int which, int shadow)
        {
            shadow = (shadow != 0) ? 1 : 0;
            if (sprites[which].shadow != shadow)
            {
                Video.video_screen_update_partial(Video.video_screen_get_vpos());
                sprites[which].shadow = (byte)shadow;
            }
        }
        public static void segaic16_sprites_buffer(sprite_info info)
        {
            if (info.buffer != null)
            {
                int i;
                ushort temp;
                for (i = 0; i < info.ramsize / 2; i++)
                {
                    temp=info.spriteram[i];
                    info.spriteram[i]=info.buffer[i];
                    info.buffer[i]=temp;
                }
                info.spriteram[0] = 0xffff;
            }
        }
        public static void segaic16_sprites_draw_0_w(int offset, ushort data)
        {
            segaic16_sprites_buffer(sprites[0]);
        }
        public static void segaic16_sprites_draw_1_w(int offset, ushort data)
        {
            segaic16_sprites_buffer(sprites[1]);
        }

        public static ushort segaic16_open_bus_r(int offset)
        {
            ushort result;
            if (recurse != 0)
            {
                return 0xffff;
            }
            recurse = 1;
            result = 0;//program_read_word_16be(activecpu_get_pc());
            recurse = 0;
            return result;
        }
        public static void segaic16_memory_mapper_init()
        {
            sound_w_callback = sound_w_system18;
            sound_r_callback = sound_r_system18;
            segaic16_memory_mapper_reset();
        }
        public static void segaic16_memory_mapper_reset()
        {
            int i;
            for (i = 0; i < 0x20; i++)
            {
                regs[i] = 0;
            }
        }
        public static void segaic16_memory_mapper_set_decrypted(ushort[] uu)
        {

        }
        public static void memory_mapper_w(int offset, byte data)
        {
            byte oldval;
            offset &= 0x1f;
            oldval = regs[offset];
            regs[offset] = data;
            switch (offset)
            {
                case 0x02:
                    if (((oldval ^ regs[offset]) & 3) != 0)
                    {
                        Cpuint.cpunum_set_input_line(0, (int)LineState.INPUT_LINE_RESET, (regs[offset] & 3) == 3 ? LineState.ASSERT_LINE : LineState.CLEAR_LINE);
                        if ((regs[offset] & 3) == 3)
                        {
                            fd1094_machine_init();
                        }
                    }
                    break;
                case 0x03:
                    if (sound_w_callback != null)
                    {
                        sound_w_callback(data);
                    }
                    break;
                case 0x04:
                    if ((regs[offset] & 7) != 7)
                    {
                        Cpuint.cpunum_set_input_line(0, (~regs[offset] & 7), LineState.HOLD_LINE);
                    }
                    break;
                case 0x05:
                    if (data == 0x01)
                    {
                        int addr = (regs[0x0a] << 17) | (regs[0x0b] << 9) | (regs[0x0c] << 1);
                        MC68000.mm1[0].WriteWord(addr, (short)((regs[0x00] << 8) | regs[0x01]));
                    }
                    else if (data == 0x02)
                    {
                        int addr = (regs[0x07] << 17) | (regs[0x08] << 9) | (regs[0x09] << 1);
                        ushort result;
                        result = (ushort)MC68000.mm1[0].ReadWord(addr);
                        regs[0x00] = (byte)(result >> 8);
                        regs[0x01] = (byte)result;
                    }
                    break;
                case 0x07:
                case 0x08:
                case 0x09:
                    break;
                case 0x0a:
                case 0x0b:
                case 0x0c:
                    break;
                case 0x10:
                case 0x11:
                case 0x12:
                case 0x13:
                case 0x14:
                case 0x15:
                case 0x16:
                case 0x17:
                case 0x18:
                case 0x19:
                case 0x1a:
                case 0x1b:
                case 0x1c:
                case 0x1d:
                case 0x1e:
                case 0x1f:
                    if (oldval != data)
                    {
                        //update_memory_mapping(machine, chip);
                    }
                    break;
                default:
                    break;
            }
        }
        public static ushort memory_mapper_r(int offset, ushort unmapped_val)
        {
            offset &= 0x1f;
            switch (offset)
            {
                case 0x00:
                case 0x01:
                    return regs[offset];
                case 0x02:
                    return (ushort)((regs[0x02] & 3) == 3 ? 0x00 : 0x0f);
                case 0x03:
                    if (sound_r_callback != null)
                    {
                        return sound_r_callback();
                    }
                    return 0xff;
                default:
                    break;
            }
            return unmapped_val;
        }
    }
}
