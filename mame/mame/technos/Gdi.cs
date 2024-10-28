using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace mame
{
    public partial class Technos
    {
        public static bool bBg, bFg, bSprite;
        public static void GDIInit()
        {

        }
        public static Bitmap GetBg()
        {
            int i1, i2, iOffset, i3, i4;
            int rows, cols, width, height;
            int tilewidth, tileheight;
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
                        tile_index = background_scan(i3, i4);
                        attr = ddragon_bgvideoram[2 * tile_index];
                        code = (ddragon_bgvideoram[2 * tile_index + 1] + ((attr & 0x07) << 8)) % bg_tilemap.total_elements;
                        color = (attr >> 3) & 0x07;
                        flags = ((attr & 0xc0) >> 6) & 3;
                        pen_data_offset = code * 0x100;
                        palette_base = 0x100 + 0x10 * color;
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
                                iByte = gfx3rom[iOffset];
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
        public static Bitmap GetFg()
        {
            int i1, i2, iOffset, i3, i4;
            int rows, cols, width, height;
            int tilewidth, tileheight;
            tilewidth = 0x8;
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
                        tile_index = i4 * 0x20 + i3;
                        attr = Technos.ddragon_fgvideoram[2 * tile_index];
                        code = (Technos.ddragon_fgvideoram[2 * tile_index + 1] + ((attr & 0x07) << 8)) % fg_tilemap.total_elements;
                        color = attr >> 5;
                        flags = 0;
                        pen_data_offset = code * 0x40;
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
                                iOffset = pen_data_offset + i2 * 8 + i1;
                                iByte = gfx1rom[iOffset];
                                if (iByte == 0)
                                {
                                    c1 = Color.Transparent;
                                }
                                else
                                {
                                    c1 = Color.FromArgb((int)Palette.entry_color[palette_base + iByte]);
                                }
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
            int i,i1, j1, offsetx, offsety, xdir, ydir, code = 0, color = 0;
            Bitmap bm1;
            Color c1 = new Color();
            ushort c;
            bm1 = new Bitmap(0x100, 0x100);
            int offs;
            BitmapData bmData;
            bmData = bm1.LockBits(new Rectangle(0, 0, bm1.Width, bm1.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            unsafe
            {
                byte* ptr = (byte*)(bmData.Scan0);
                byte* ptr2 = (byte*)0;
                byte[] srcbyte = new byte[0x800];                
                if (technos_video_hw == 1)
                {
                    Array.Copy(Generic.spriteram, srcbyte, 0x800);
                }
                else
                {
                    Array.Copy(ddragon_spriteram, 0x800, srcbyte, 0, 0x800);
                }
                for (i = 0; i < (64 * 5); i += 5)
                {
                    int attr = srcbyte[i + 1];
                    if ((attr & 0x80) != 0)
                    {
                        int sx = 240 - srcbyte[i + 4] + ((attr & 2) << 7);
                        int sy = 232 - srcbyte[i + 0] + ((attr & 1) << 8);
                        int size = (attr & 0x30) >> 4;
                        int flipx = (attr & 8);
                        int flipy = (attr & 4);
                        int dx = -16, dy = -16;
                        int which;
                        if (technos_video_hw == 2)
                        {
                            color = (srcbyte[i + 2] >> 5);
                            which = srcbyte[i + 3] + ((srcbyte[i + 2] & 0x1f) << 8);
                        }
                        else
                        {
                            if (technos_video_hw == 1)
                            {
                                if ((sx < -7) && (sx > -16))
                                {
                                    sx += 256;
                                }
                                if ((sy < -7) && (sy > -16))
                                {
                                    sy += 256;
                                }
                            }
                            color = (srcbyte[i + 2] >> 4) & 0x07;
                            which = srcbyte[i + 3] + ((srcbyte[i + 2] & 0x0f) << 8);
                        }
                        if (Generic.flip_screen_get() != 0)
                        {
                            sx = 240 - sx;
                            sy = 256 - sy;
                            flipx = (flipx == 0 ? 1 : 0);
                            flipy = (flipy == 0 ? 1 : 0);
                            dx = -dx;
                            dy = -dy;
                        }
                        which &= ~size;
                        if (flipx != 0)
                        {
                            offsetx = 0x1f;
                            xdir = -1;
                        }
                        else
                        {
                            offsetx = 0;
                            xdir = 1;
                        }
                        if (flipy != 0)
                        {
                            offsety = 0x1f;
                            ydir = -1;
                        }
                        else
                        {
                            offsety = 0;
                            ydir = 1;
                        }
                        switch (size)
                        {
                            case 0:
                                for (i1 = 0; i1 < 0x10; i1++)
                                {
                                    for (j1 = 0; j1 < 0x10; j1++)
                                    {
                                        if (sx + offsetx + xdir * j1 >= 0 && sx + offsetx + xdir * j1 < 0x100 && sy + offsety + ydir * i1 >= 0 && sy + offsety + ydir * i1 < 0x100)
                                        {
                                            c = gfx2rom[which * 0x100 + 0x10 * i1 + j1];
                                            if (c != 0)
                                            {
                                                c1 = Color.FromArgb((int)Palette.entry_color[0x80 + color * 10 + c]);
                                                ptr2 = ptr + ((sy + offsety + ydir * i1) * 0x100 + (sx + offsetx + xdir * j1)) * 4;
                                                *ptr2 = c1.B;
                                                *(ptr2 + 1) = c1.G;
                                                *(ptr2 + 2) = c1.R;
                                                *(ptr2 + 3) = c1.A;
                                            }
                                        }
                                    }
                                }
                                break;
                            case 1:
                                for (i1 = 0; i1 < 0x10; i1++)
                                {
                                    for (j1 = 0; j1 < 0x10; j1++)
                                    {
                                        if (sx + offsetx + xdir * j1 >= 0 && sx + offsetx + xdir * j1 < 0x100 && sy + dy + offsety + ydir * i1 >= 0 && sy + dy + offsety + ydir * i1 < 0x100)
                                        {
                                            c = gfx2rom[which * 0x100 + 0x10 * i1 + j1];
                                            if (c != 0)
                                            {
                                                c1 = Color.FromArgb((int)Palette.entry_color[0x80 + color * 10 + c]);
                                                ptr2 = ptr + ((sy + dy + offsety + ydir * i1) * 0x100 + (sx + offsetx + xdir * j1)) * 4;
                                                *ptr2 = c1.B;
                                                *(ptr2 + 1) = c1.G;
                                                *(ptr2 + 2) = c1.R;
                                                *(ptr2 + 3) = c1.A;
                                            }
                                        }
                                        if (sx + offsetx + xdir * j1 >= 0 && sx + offsetx + xdir * j1 < 0x100 && sy + offsety + ydir * i1 >= 0 && sy + offsety + ydir * i1 < 0x100)
                                        {
                                            c = gfx2rom[(which +1)* 0x100 + 0x10 * i1 + j1];
                                            if (c != 0)
                                            {
                                                c1 = Color.FromArgb((int)Palette.entry_color[0x80 + color * 10 + c]);
                                                ptr2 = ptr + ((sy + offsety + ydir * i1) * 0x100 + (sx + offsetx + xdir * j1)) * 4;
                                                *ptr2 = c1.B;
                                                *(ptr2 + 1) = c1.G;
                                                *(ptr2 + 2) = c1.R;
                                                *(ptr2 + 3) = c1.A;
                                            }
                                        }
                                    }
                                }
                                break;
                            case 2:
                                for (i1 = 0; i1 < 0x10; i1++)
                                {
                                    for (j1 = 0; j1 < 0x10; j1++)
                                    {
                                        if (sx + dx + offsetx + xdir * j1 >= 0 && sx + dx + offsetx + xdir * j1 < 0x100 && sy + offsety + ydir * i1 >= 0 && sy + offsety + ydir * i1 < 0x100)
                                        {
                                            c = gfx2rom[which * 0x100 + 0x10 * i1 + j1];
                                            if (c != 0)
                                            {
                                                c1 = Color.FromArgb((int)Palette.entry_color[0x80 + color * 10 + c]);
                                                ptr2 = ptr + ((sy + offsety + ydir * i1) * 0x100 + (sx + dx + offsetx + xdir * j1)) * 4;
                                                *ptr2 = c1.B;
                                                *(ptr2 + 1) = c1.G;
                                                *(ptr2 + 2) = c1.R;
                                                *(ptr2 + 3) = c1.A;
                                            }
                                        }
                                        if (sx + offsetx + xdir * j1 >= 0 && sx + offsetx + xdir * j1 < 0x100 && sy + offsety + ydir * i1 >= 0 && sy + offsety + ydir * i1 < 0x100)
                                        {
                                            c = gfx2rom[(which+2) * 0x100 + 0x10 * i1 + j1];
                                            if (c != 0)
                                            {
                                                c1 = Color.FromArgb((int)Palette.entry_color[0x80 + color * 10 + c]);
                                                ptr2 = ptr + ((sy + offsety + ydir * i1) * 0x100 + (sx + offsetx + xdir * j1)) * 4;
                                                *ptr2 = c1.B;
                                                *(ptr2 + 1) = c1.G;
                                                *(ptr2 + 2) = c1.R;
                                                *(ptr2 + 3) = c1.A;
                                            }
                                        }
                                    }
                                }
                                break;
                            case 3:
                                for (i1 = 0; i1 < 0x10; i1++)
                                {
                                    for (j1 = 0; j1 < 0x10; j1++)
                                    {
                                        if (sx + dx + offsetx + xdir * j1 >= 0 && sx + dx + offsetx + xdir * j1 < 0x100 && sy + dy + offsety + ydir * i1 >= 0 && sy + dy + offsety + ydir * i1 < 0x100)
                                        {
                                            c = gfx2rom[which * 0x100 + 0x10 * i1 + j1];
                                            if (c != 0)
                                            {
                                                c1 = Color.FromArgb((int)Palette.entry_color[0x80 + color * 10 + c]);
                                                ptr2 = ptr + ((sy + dy + offsety + ydir * i1) * 0x100 + (sx + dx + offsetx + xdir * j1)) * 4;
                                                *ptr2 = c1.B;
                                                *(ptr2 + 1) = c1.G;
                                                *(ptr2 + 2) = c1.R;
                                                *(ptr2 + 3) = c1.A;
                                            }
                                        }
                                        if (sx + dx + offsetx + xdir * j1 >= 0 && sx + dx + offsetx + xdir * j1 < 0x100 && sy + offsety + ydir * i1 >= 0 && sy + offsety + ydir * i1 < 0x100)
                                        {
                                            c = gfx2rom[(which+1) * 0x100 + 0x10 * i1 + j1];
                                            if (c != 0)
                                            {
                                                c1 = Color.FromArgb((int)Palette.entry_color[0x80 + color * 10 + c]);
                                                ptr2 = ptr + ((sy + offsety + ydir * i1) * 0x100 + (sx + dx + offsetx + xdir * j1)) * 4;
                                                *ptr2 = c1.B;
                                                *(ptr2 + 1) = c1.G;
                                                *(ptr2 + 2) = c1.R;
                                                *(ptr2 + 3) = c1.A;
                                            }
                                        }
                                        if (sx + offsetx + xdir * j1 >= 0 && sx + offsetx + xdir * j1 < 0x100 && sy + dy + offsety + ydir * i1 >= 0 && sy + dy + offsety + ydir * i1 < 0x100)
                                        {
                                            c = gfx2rom[(which+2) * 0x100 + 0x10 * i1 + j1];
                                            if (c != 0)
                                            {
                                                c1 = Color.FromArgb((int)Palette.entry_color[0x80 + color * 10 + c]);
                                                ptr2 = ptr + ((sy + dy + offsety + ydir * i1) * 0x100 + (sx + offsetx + xdir * j1)) * 4;
                                                *ptr2 = c1.B;
                                                *(ptr2 + 1) = c1.G;
                                                *(ptr2 + 2) = c1.R;
                                                *(ptr2 + 3) = c1.A;
                                            }
                                        }
                                        if (sx + offsetx + xdir * j1 >= 0 && sx + offsetx + xdir * j1 < 0x100 && sy + offsety + ydir * i1 >= 0 && sy + offsety + ydir * i1 < 0x100)
                                        {
                                            c = gfx2rom[(which+3) * 0x100 + 0x10 * i1 + j1];
                                            if (c != 0)
                                            {
                                                c1 = Color.FromArgb((int)Palette.entry_color[0x80 + color * 10 + c]);
                                                ptr2 = ptr + ((sy + offsety + ydir * i1) * 0x100 + (sx + offsetx + xdir * j1)) * 4;
                                                *ptr2 = c1.B;
                                                *(ptr2 + 1) = c1.G;
                                                *(ptr2 + 2) = c1.R;
                                                *(ptr2 + 3) = c1.A;
                                            }
                                        }
                                    }
                                }
                                break;
                        }
                    }
                }                
            }
            bm1.UnlockBits(bmData);
            return bm1;
        }
        public static Bitmap GetAllGDI()
        {
            Bitmap bm1 = new Bitmap(0x100, 0x100), bm2;
            Graphics g = Graphics.FromImage(bm1);
            g.Clear(Color.Transparent);
            if (bBg)
            {
                bm2 = GetBg();
                g.DrawImage(bm2, 0, 0);
            }
            if (bSprite)
            {
                bm2 = GetSprite();
                g.DrawImage(bm2, 0, 0);
            }
            if (bFg)
            {
                bm2 = GetFg();
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
