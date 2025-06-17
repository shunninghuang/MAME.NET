using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace mame
{
    public partial class Gaelco
    {
        public static bool bMap0, bMap1, bSprite;
        public static void GDIInit()
        {

        }
        public static Bitmap GetMap0()
        {
            int i1, i2, iOffset, i3, i4;
            int rows, cols, width, height;
            int tilewidth, tileheight;
            int data, data2;
            tilewidth = 0x10;
            tileheight = tilewidth;
            rows = 0x20;
            cols = rows;
            width = tilewidth * cols;
            height = width;
            int iByte;
            int tile_index, attr, color, flipyx, code, flags;
            int pen_data_offset, palette_base;
            int x0 = 0, y0 = 0, dx0 = 0, dy0 = 0;
            Color c1 = new Color();
            Bitmap bm1;
            bm1 = new Bitmap(width, height);
            BitmapData bmData;
            bmData = bm1.LockBits(new Rectangle(0, 0, bm1.Width, bm1.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            unsafe
            {
                byte* ptr = (byte*)(bmData.Scan0);
                byte* ptr2 = (byte*)0;
                for (i3 = 0; i3 < cols; i3++)
                {
                    for (i4 = 0; i4 < rows; i4++)
                    {
                        tile_index = i4 * cols + i3;
                        data = Gaelco.gaelco_videoram[tile_index << 1];
                        data2 = Gaelco.gaelco_videoram[(tile_index << 1) + 1];
                        code = ((data & 0xfffc) >> 2);
                        color = data2 & 0x3f;
                        flags = data & 0x03;
                        pen_data_offset = (0x4000 + code) * 0x100;
                        palette_base = 0x10 * color;
                        if (flags == 0)
                        {
                            x0 = tilewidth * i3;
                            y0 = tileheight * i4;
                            dx0 = 1;
                            dy0 = 1;
                        }
                        else if (flags == 1)
                        {
                            x0 = tilewidth * i3 + tilewidth - 1;
                            y0 = tileheight * i4;
                            dx0 = -1;
                            dy0 = 1;
                        }
                        else if (flags == 2)
                        {
                            x0 = tilewidth * i3;
                            y0 = tileheight * i4 + tileheight - 1;
                            dx0 = 1;
                            dy0 = -1;
                        }
                        else if (flags == 3)
                        {
                            x0 = tilewidth * i3 + tilewidth - 1;
                            y0 = tileheight * i4 + tileheight - 1;
                            dx0 = -1;
                            dy0 = -1;
                        }
                        for (i1 = 0; i1 < tilewidth; i1++)
                        {
                            for (i2 = 0; i2 < tileheight; i2++)
                            {
                                iOffset = pen_data_offset + i2 * 0x10 + i1;
                                iByte = gfx2rom[iOffset];
                                c1 = Color.FromArgb((int)Palette.entry_color[palette_base + iByte]);
                                ptr2 = ptr + ((y0 + dy0 * i2) * width + (x0 + dx0 * i1)) * 4;
                                *ptr2 = c1.B;
                                *(ptr2 + 1) = c1.G;
                                *(ptr2 + 2) = c1.R;
                                *(ptr2 + 3) = c1.A;
                            }
                        }
                    }
                }
            }
            bm1.UnlockBits(bmData);
            return bm1;
        }
        public static Bitmap GetMap1()
        {
            int i1, i2, iOffset, i3, i4;
            int rows, cols, width, height;
            int tilewidth, tileheight;
            int data, data2;
            tilewidth = 0x10;
            tileheight = tilewidth;
            rows = 0x20;
            cols = rows;
            width = tilewidth * cols;
            height = width;
            int iByte;
            int tile_index, attr, color, flipyx, code, flags;
            int pen_data_offset, palette_base;
            int x0 = 0, y0 = 0, dx0 = 0, dy0 = 0;
            Color c1 = new Color();
            Bitmap bm1;
            bm1 = new Bitmap(width, height);
            BitmapData bmData;
            bmData = bm1.LockBits(new Rectangle(0, 0, bm1.Width, bm1.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            unsafe
            {
                byte* ptr = (byte*)(bmData.Scan0);
                byte* ptr2 = (byte*)0;
                for (i3 = 0; i3 < cols; i3++)
                {
                    for (i4 = 0; i4 < rows; i4++)
                    {
                        tile_index = i4 * cols + i3;
                        data = Gaelco.gaelco_videoram[(0x1000 / 2) + (tile_index << 1)];
                        data2 = Gaelco.gaelco_videoram[(0x1000 / 2) + (tile_index << 1) + 1];
                        code = ((data & 0xfffc) >> 2);
                        color = data2 & 0x3f;
                        flags = data & 0x03;
                        pen_data_offset = (0x4000 + code) * 0x100;
                        palette_base = 0x10 * color;
                        if (flags == 0)
                        {
                            x0 = tilewidth * i3;
                            y0 = tileheight * i4;
                            dx0 = 1;
                            dy0 = 1;
                        }
                        else if (flags == 1)
                        {
                            x0 = tilewidth * i3 + tilewidth - 1;
                            y0 = tileheight * i4;
                            dx0 = -1;
                            dy0 = 1;
                        }
                        else if (flags == 2)
                        {
                            x0 = tilewidth * i3;
                            y0 = tileheight * i4 + tileheight - 1;
                            dx0 = 1;
                            dy0 = -1;
                        }
                        else if (flags == 3)
                        {
                            x0 = tilewidth * i3 + tilewidth - 1;
                            y0 = tileheight * i4 + tileheight - 1;
                            dx0 = -1;
                            dy0 = -1;
                        }
                        for (i1 = 0; i1 < tilewidth; i1++)
                        {
                            for (i2 = 0; i2 < tileheight; i2++)
                            {
                                iOffset = pen_data_offset + i2 * 0x10 + i1;
                                iByte = gfx2rom[iOffset];
                                c1 = Color.FromArgb((int)Palette.entry_color[palette_base + iByte]);
                                ptr2 = ptr + ((y0 + dy0 * i2) * width + (x0 + dx0 * i1)) * 4;
                                *ptr2 = c1.B;
                                *(ptr2 + 1) = c1.G;
                                *(ptr2 + 2) = c1.R;
                                *(ptr2 + 3) = c1.A;
                            }
                        }
                    }
                }
            }
            bm1.UnlockBits(bmData);
            return bm1;
        }
        public static Bitmap GetSprite()
        {
            Bitmap bm1;
            int offs;
            int i, i5, i6;
            int xdir, ydir, offx, offy;
            int x, y, ex, ey;
            int iByte1, iByte2, iByte3, iByte4;
            Color c11 = new Color(), c12 = new Color(), c13 = new Color(), c14 = new Color();
            bm1 = new Bitmap(512, 512);
            for (i = 0x800 - 4 - 1; i >= 3; i -= 4)
            {
                int sx = gaelco_spriteram[i + 2] & 0x01ff;
                int sy = (240 - (gaelco_spriteram[i] & 0x00ff)) & 0x00ff;
                int number = gaelco_spriteram[i + 3];
                int color = (gaelco_spriteram[i + 2] & 0x7e00) >> 9;
                int attr = (gaelco_spriteram[i] & 0xfe00) >> 9;
                int priority = (gaelco_spriteram[i] & 0x3000) >> 12;
                int xflip = attr & 0x20;
                int yflip = attr & 0x40;
                int spr_size, pri_mask;
                if (color >= 0x38)
                {
                    priority = 4;
                }
                switch (priority)
                {
                    case 0: pri_mask = 0xff00; break;
                    case 1: pri_mask = 0xff00 | 0xf0f0; break;
                    case 2: pri_mask = 0xff00 | 0xf0f0 | 0xcccc; break;
                    case 3: pri_mask = 0xff00 | 0xf0f0 | 0xcccc | 0xaaaa; break;
                    default:
                    case 4: pri_mask = 0; break;
                }
                if ((attr & 0x04) != 0)
                {
                    spr_size = 1;
                }
                else
                {
                    spr_size = 2;
                    number &= (~3);
                }
                for (y = 0; y < spr_size; y++)
                {
                    for (x = 0; x < spr_size; x++)
                    {
                        ex = xflip != 0 ? (spr_size - 1 - x) : x;
                        ey = yflip != 0 ? (spr_size - 1 - y) : y;
                        if (xflip != 0)
                        {
                            offx = 8;
                            xdir = -1;
                        }
                        else
                        {
                            offx = 0;
                            xdir = 1;
                        }
                        if (yflip != 0)
                        {
                            offy = 8;
                            ydir = -1;
                        }
                        else
                        {
                            offy = 0;
                            ydir = 1;
                        }
                        for (i5 = 0; i5 < 8; i5++)
                        {
                            for (i6 = 0; i6 < 8; i6++)
                            {
                                iByte1 = gfx1rom[(number + x_offset[ex] + y_offset[ey]) * 0x40 + i5 + i6 * 8];
                                if (iByte1 != 0)
                                {
                                    c11 = Color.FromArgb((int)Palette.entry_color[0x10 * color + iByte1]);
                                    bm1.SetPixel(sx - 0x0f + x * 8 + offx + xdir * i5, sy + y * 8 + offy + ydir * i6, c11);
                                }
                            }
                        }
                    }
                }
            }
            return bm1;
        }
        public static Bitmap GetAllGDI()
        {
            Bitmap bm1 = new Bitmap(0x200, 0x200), bm2;
            Graphics g = Graphics.FromImage(bm1);
            g.Clear(Color.Transparent);
            if (bMap0)
            {
                bm2 = GetMap0();
                g.DrawImage(bm2, -gaelco_vregs[1] - 4, -gaelco_vregs[0]);
            }
            if (bMap1)
            {
                bm2 = GetMap1();
                g.DrawImage(bm2, -gaelco_vregs[3], -gaelco_vregs[2]);
                g.DrawImage(bm2, 0x200 - gaelco_vregs[3], -gaelco_vregs[2]);
            }
            if (bSprite)
            {
                bm2 = GetSprite();
                g.DrawImage(bm2, 0, 0);
            }
            switch (Machine.sDirection)
            {
                case "":
                    break;
                case "90":
                    bm1.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    break;
            }
            return bm1;
        }
    }
}
