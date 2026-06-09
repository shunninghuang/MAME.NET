using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace mame
{
    public partial class Kaneko
    {
        public static bool bBg, bFg, bSprite;
        public static void GDIInit()
        {

        }
        public static Bitmap GetBg()
        {
            int i1, i2, iOffset, iByte, i3, i4;
            int rows, cols, width, height;
            int tilewidth, tileheight;
            int tile_index, attr, code, color, pen_data_offset, palette_base;
            int x0 = 0, y0 = 0, dx0 = 0, dy0 = 0;
            tilewidth = 0x10;
            tileheight = tilewidth;
            rows = 0x20;
            cols = rows;
            width = tilewidth * cols;
            height = width;
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
                        attr = Generic.colorram[tile_index];
                        code = Generic.videoram[tile_index] + ((attr & 0x0f) << 8);
                        color = (attr >> 4) + 16;
                        pen_data_offset = code * 0x100;
                        palette_base = 0x10 * color;
                        x0 = tilewidth * i3;
                        y0 = tileheight * i4;
                        dx0 = 1;
                        dy0 = 1;
                        for (i1 = 0; i1 < tilewidth; i1++)
                        {
                            for (i2 = 0; i2 < tileheight; i2++)
                            {
                                iOffset = pen_data_offset + i2 * 0x10 + i1;
                                iByte = gfx1rom[iOffset];
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
            int i1, i2, iOffset, iByte, i3, i4;
            int rows, cols, width, height;
            int tilewidth, tileheight;
            int tile_index, attr, code, color, pen_data_offset, palette_base;
            int x0 = 0, y0 = 0, dx0 = 0, dy0 = 0;
            tilewidth = 0x10;
            tileheight = tilewidth;
            rows = 0x20;
            cols = rows;
            width = tilewidth * cols;
            height = width;
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
                        attr = Kaneko.airbustr_colorram2[tile_index];
                        code = Kaneko.airbustr_videoram2[tile_index] + ((attr & 0x0f) << 8);
                        color = attr >> 4;
                        pen_data_offset = code * 0x100;
                        palette_base = 0x10 * color;
                        x0 = tilewidth * i3;
                        y0 = tileheight * i4;
                        dx0 = 1;
                        dy0 = 1;
                        for (i1 = 0; i1 < tilewidth; i1++)
                        {
                            for (i2 = 0; i2 < tileheight; i2++)
                            {
                                iOffset = pen_data_offset + i2 * 0x10 + i1;
                                iByte = gfx1rom[iOffset];
                                if (iByte != 0)
                                {
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
            }
            bm1.UnlockBits(bmData);
            return bm1;
        }
        public static Bitmap GetSprite()
        {
            int i1, i2;
            Color c1 = new Color();
            ushort u1;
            Bitmap bm1;
            bm1 = new Bitmap(0x100, 0x100);
            for (i1 = 0; i1 < 0x100; i1++)
            {
                for (i2 = 0; i2 < 0x100; i2++)
                {
                    u1 = Kaneko.pandora_sprites_bitmap.uu1[i2 * 0x100 + i1];
                    if ((u1 & 0xf) != 0)
                    {
                        c1 = Color.FromArgb((int)Palette.entry_color[u1]);
                        bm1.SetPixel(i1, i2, c1);
                    }
                }
            }
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
                g.DrawImage(bm2, 0x94 - bg_tilemap.rowscroll[0], 0x100 - bg_tilemap.colscroll[0]);
                g.DrawImage(bm2, -0x200 + 0x94 - bg_tilemap.rowscroll[0], 0x100 - bg_tilemap.colscroll[0]);
            }
            if (bFg)
            {
                bm2 = GetFg();
                g.DrawImage(bm2, 0x94 - fg_tilemap.rowscroll[0], 0x100 - fg_tilemap.colscroll[0]);
                g.DrawImage(bm2, -0x200 + 0x94 - fg_tilemap.rowscroll[0], 0x100 - fg_tilemap.colscroll[0]);
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
