using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace mame
{
    public partial class Seibu
    {
        public static bool bBg, bSprite;
        public static void GDIInit()
        {

        }
        public static Bitmap GetBg()
        {
            int i1, i2, iOffset, iByte, i3, i4;
            int rows, cols, width, height;
            int tilewidth, tileheight;
            int logindex,memindex, attr, code, color, flags, pen_data_offset, palette_base;
            int x0 = 0, y0 = 0, dx0 = 0, dy0 = 0;
            tilewidth = 8;
            tileheight = tilewidth;
            rows = 0x20;
            cols = 0x40;
            width = tilewidth * cols;
            height = tileheight * rows;
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
                        logindex = i4 * cols + i3;
                        memindex = bg_tilemap.logical_to_memory[logindex];
                        attr = Generic.videoram[2 * memindex + 1];
                        code = Generic.videoram[2 * memindex] + ((attr & 0xc0) << 2) + (Seibu.tile_bank << 10);
                        color = attr & 0xf;
                        flags = (byte)(Tmap.TILE_FLIPXY((attr & 0x30) >> 4) ^ (bg_tilemap.attributes & 0x03));
                        pen_data_offset = code * 0x40;
                        palette_base = 8 * color;
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
                                iByte = Seibu.tilesrom[iOffset];
                                c1 = Color.FromArgb((int)Palette.entry_color2[palette_base + iByte]);
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
            int i,j,i1, i2,xdir,ydir,iByte;
            Color c1 = new Color();
            Bitmap bm1;
            bm1 = new Bitmap(0x100, 0x100);
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
                    if (flipx != 0)
                    {
                        xdir = -1;
                        sx += 15;
                    }
                    else
                    {
                        xdir = 1;
                    }
                    if (flipy != 0)
                    {
                        ydir = -1;
                        sy += 15;
                    }
                    else
                    {
                        ydir = 1;
                    }
                    code %= spritecount[sprite_bank];
                    for (i1 = 0; i1 < 0x10; i1++)
                    {
                        for (i2 = 0; i2 < 0x10; i2++)
                        {
                            iByte = spritesrom[sprite_bank][code * 0x100 + i1 + i2 * 0x10];
                            if (sx + xdir * i1 >= 0 && sx + xdir * i1 < 0x100 && sy + ydir * i2 >= 0x40 && sy + ydir * i2 < 0xc0)
                            {
                                c1 = Color.FromArgb((int)Palette.entry_color2[0x80 + 8 * color + iByte]);
                                if (iByte != 0)
                                {
                                    bm1.SetPixel(sx + xdir * i1, sy + ydir * i2, c1);
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
            Bitmap bm1 = new Bitmap(0x100, 0x100), bm2;
            Graphics g = Graphics.FromImage(bm1);
            g.Clear(Color.Transparent);
            if (bBg)
            {
                bm2 = GetBg();
                g.DrawImage(bm2, -bg_tilemap.rowscroll[0], -bg_tilemap.colscroll[0]);
                //g.DrawImage(bm2, 0x200, -bg_tilemap.colscroll[0]);
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
