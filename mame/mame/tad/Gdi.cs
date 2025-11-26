using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace mame
{
    public partial class Tad
    {
        public static bool bText, bBg, bFg, bSprite;
        public delegate Bitmap getspriteDelegate();
        public static getspriteDelegate getsprite1;
        public static void GDIInit()
        {
            switch (Machine.sName)
            {
                case "toki":
                case "tokiu":
                case "tokip":
                case "tokia":
                case "tokiua":
                case "juju":
                case "jujuba":
                    getsprite1 = GetSprite_toki;
                    break;
                case "tokib":
                case "jujub":
                    getsprite1 = GetSprite_tokib;
                    break;
            }
        }
        public static Bitmap GetText()
        {
            int i1, i2, iOffset, i3, i4;
            int rows, cols, width, height;
            int tilewidth, tileheight;
            tilewidth = 8;
            tileheight = tilewidth;
            rows = 0x20;
            cols = rows;
            width = tilewidth * cols;
            height = width;
            int iByte;
            int tile_index = 0, color, code, flags;
            int pen_data_offset, palette_base;
            pen_data_offset = 0;
            palette_base = 0;            
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
                        tile_index = 0;
                        if (text_layer.attributes == 0)
                        {
                            tile_index = i4 * cols + i3;
                        }
                        else if (text_layer.attributes == 1)
                        {
                            tile_index = i4 * cols + cols - 1 - i3;
                        }
                        else if (text_layer.attributes == Tilemap.TILEMAP_FLIPY)
                        {
                            tile_index = (rows - 1 - i4) * cols + i3;
                        }
                        else if (text_layer.attributes == 3)
                        {
                            tile_index = (rows - 1 - i4) * cols + cols - 1 - i3;
                        }
                        code = Generic.videoram16[tile_index];
                        color = (code >> 12) & 0xf;
                        code = code & 0xfff;
                        flags = text_layer.attributes;
                        pen_data_offset = code * 0x40;
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
                                iOffset = pen_data_offset + i2 * 8 + i1;
                                iByte = gfx1rom[iOffset];
                                if (iByte == 0x0f)
                                {
                                    c1 = Color.Transparent;
                                }
                                else
                                {
                                    c1 = Color.FromArgb((int)Palette.entry_color[palette_base + iByte]);
                                }
                                if (c1 != Color.Transparent)
                                {
                                    int i11 = 1;
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
            int tile_index=0, color, code, flags;
            int pen_data_offset, palette_base;
            pen_data_offset = 0;
            palette_base = 0;            
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
                        tile_index = 0;
                        if (background_layer.attributes == 0)
                        {
                            tile_index = i4 * cols + i3;
                        }
                        else if (background_layer.attributes == 1)
                        {
                            tile_index = i4 * cols + cols - 1 - i3;
                        }
                        else if (background_layer.attributes == Tilemap.TILEMAP_FLIPY)
                        {
                            tile_index = (rows - 1 - i4) * cols + i3;
                        }
                        else if (background_layer.attributes == 3)
                        {
                            tile_index = (rows - 1 - i4) * cols + cols - 1 - i3;
                        }
                        code = toki_background1_videoram16[tile_index];
                        color = (code >> 12) & 0xf;
                        code = code & 0xfff;
                        flags = background_layer.attributes;
                        pen_data_offset = code * 0x100;
                        palette_base = 0x200 + 0x10 * color;
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
            tilewidth = 0x10;
            tileheight = tilewidth;
            rows = 0x20;
            cols = rows;
            width = tilewidth * cols;
            height = width;
            int iByte;
            int tile_index=0, color, code, flags;
            int pen_data_offset, palette_base;
            pen_data_offset = 0;
            palette_base = 0;            
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
                        tile_index = 0;
                        if (foreground_layer.attributes == 0)
                        {
                            tile_index = i4 * cols + i3;
                        }
                        else if (foreground_layer.attributes == 1)
                        {
                            tile_index = i4 * cols + cols - 1 - i3;
                        }
                        else if (foreground_layer.attributes == Tilemap.TILEMAP_FLIPY)
                        {
                            tile_index = (rows - 1 - i4) * cols + i3;
                        }
                        else if (foreground_layer.attributes == 3)
                        {
                            tile_index = (rows - 1 - i4) * cols + cols - 1 - i3;
                        }
                        tile_index = i4 * cols + i3;
                        code = toki_background2_videoram16[tile_index];
                        color = (code >> 12) & 0xf;
                        code = code & 0xfff;
                        flags = foreground_layer.attributes;
                        pen_data_offset = code * 0x100;
                        palette_base = 0x300 + 0x10 * color;
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
                                iByte = gfx4rom[iOffset];
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
        public static Bitmap GetSprite_toki()
        {
            int i5, i6, iOffset, iByte, x0, y0, dx0, dy0, iXin, iYin;
            Bitmap bm1, bm2;
            bm1 = new Bitmap(0x100, 0x100);
            Graphics g = Graphics.FromImage(bm1);
            Color c1 = new Color();
            int x, y, xoffs, yoffs, tile, flipx, flipy, color, offs;
            int sprite_word_offset;
            spriteram_size = 0x800;
            for (offs = (spriteram_size / 2) - 4; offs >= 0; offs -= 4)
            {
                sprite_word_offset = offs;
                if ((Generic.buffered_spriteram16[offs + 2] != 0xf000) && (Generic.buffered_spriteram16[offs] != 0xffff))
                {
                    xoffs = (Generic.buffered_spriteram16[offs] & 0xf0);
                    x = (Generic.buffered_spriteram16[offs + 2] + xoffs) & 0x1ff;
                    if (x > 256)
                    {
                        x -= 512;
                    }
                    yoffs = (Generic.buffered_spriteram16[offs] & 0xf) << 4;
                    y = (Generic.buffered_spriteram16[offs + 3] + yoffs) & 0x1ff;
                    if (y > 256)
                    {
                        y -= 512;
                    }
                    color = Generic.buffered_spriteram16[offs + 1] >> 12;
                    flipx = Generic.buffered_spriteram16[offs] & 0x100;
                    flipy = 0;
                    tile = (Generic.buffered_spriteram16[offs + 1] & 0xfff) + ((Generic.buffered_spriteram16[offs + 2] & 0x8000) >> 3);
                    if (Generic.flip_screen_get() != 0)
                    {
                        x = 240 - x;
                        y = 240 - y;
                        if (flipx != 0)
                        {
                            flipx = 0;
                        }
                        else
                        {
                            flipx = 1;
                        }
                        flipy = 1;
                    }
                    bm2 = new Bitmap(0x10, 0x10);
                    iOffset = tile * 0x100;
                    if (flipx != 0)
                    {
                        x0 = 0xf;
                        dx0 = -1;
                    }
                    else
                    {
                        x0 = 0;
                        dx0 = 1;
                    }
                    if (flipy != 0)
                    {
                        y0 = 0xf;
                        dy0 = -1;
                    }
                    else
                    {
                        y0 = 0;
                        dy0 = 1;
                    }
                    for (i5 = 0; i5 < 0x10; i5++)
                    {
                        for (i6 = 0; i6 < 0x10; i6++)
                        {
                            iByte = gfx2rom[iOffset + i5 + i6 * 0x10];
                            if (iByte == 0x0f)
                            {
                                c1 = Color.Transparent;
                            }
                            else
                            {
                                c1 = Color.FromArgb((int)Palette.entry_color[0x10 * color + iByte]);
                            }
                            iXin = (x0 + dx0 * i5) & 0xff;
                            iYin = (y0 + dy0 * i6) & 0xff;
                            if (iXin >= 0 && iXin <= 0x0f && iYin >= 0 && iYin <= 0x0f)
                            {
                                bm2.SetPixel(iXin, iYin, c1);
                            }
                        }
                    }
                    g.DrawImage(bm2, x, y);
                }
            }
            g.Dispose();
            return bm1;
        }
        public static Bitmap GetSprite_tokib()
        {
            int i5, i6, iOffset, iByte, x0, y0, dx0, dy0, iXin, iYin;
            Bitmap bm1, bm2;
            bm1 = new Bitmap(0x100, 0x100);
            Graphics g = Graphics.FromImage(bm1);
            Color c1 = new Color();
            int x, y, xoffs, yoffs, tile, flipx, flipy, color, offs;
            int sprite_word_offset;
            spriteram_size = 0x800;
            for (offs = 0; offs < spriteram_size / 2; offs += 4)
            {
                if (Generic.buffered_spriteram16[offs] == 0xf100)
                {
                    break;
                }
                if (Generic.buffered_spriteram16[offs + 2] != 0)
                {
                    x = Generic.buffered_spriteram16[offs + 3] & 0x1ff;
                    if (x > 256)
                    {
                        x -= 512;
                    }
                    y = Generic.buffered_spriteram16[offs] & 0x1ff;
                    if (y > 256)
                    {
                        y = (512 - y) + 240;
                    }
                    else
                    {
                        y = 240 - y;
                    }
                    flipx = Generic.buffered_spriteram16[offs + 1] & 0x4000;
                    tile = Generic.buffered_spriteram16[offs + 1] & 0x1fff;
                    color = Generic.buffered_spriteram16[offs + 2] >> 12;
                    bm2 = new Bitmap(0x10, 0x10);
                    iOffset = tile * 0x100;
                    if (flipx != 0)
                    {
                        x0 = 0xf;
                        dx0 = -1;
                    }
                    else
                    {
                        x0 = 0;
                        dx0 = 1;
                    }
                    y0 = 0;
                    dy0 = 1;       
                    for (i5 = 0; i5 < 0x10; i5++)
                    {
                        for (i6 = 0; i6 < 0x10; i6++)
                        {
                            iByte = gfx2rom[iOffset + i5 + i6 * 0x10];
                            if (iByte == 0x0f)
                            {
                                c1 = Color.Transparent;
                            }
                            else
                            {
                                c1 = Color.FromArgb((int)Palette.entry_color[0x10 * color + iByte]);
                            }
                            iXin = (x0 + dx0 * i5) & 0xff;
                            iYin = (y0 + dy0 * i6) & 0xff;
                            if (iXin >= 0 && iXin <= 0x0f && iYin >= 0 && iYin <= 0x0f)
                            {
                                bm2.SetPixel(iXin, iYin, c1);
                            }
                        }
                    }
                    g.DrawImage(bm2, x, y - 1);
                }
            }
            g.Dispose();
            return bm1;
        }
        public static Bitmap GetAllGDI()
        {
            Bitmap bm1 = new Bitmap(0x100, 0x100), bm2;
            Graphics g = Graphics.FromImage(bm1);
            g.Clear(Color.Transparent);
            if ((toki_scrollram16[0x28] & 0x100) != 0)
            {
                if (bBg)
                {
                    bm2 = GetBg();
                    g.DrawImage(bm2, 0, 0);
                }
                if (bFg)
                {
                    bm2 = GetFg();
                    g.DrawImage(bm2, 0, 0);
                }
                if (bSprite)
                {
                    bm2 = getsprite1();
                    g.DrawImage(bm2, 0, 0);
                }                
                if (bText)
                {
                    bm2 = GetText();
                    g.DrawImage(bm2, 0, 0);
                }
            }
            else
            {
                if (bFg)
                {
                    bm2 = GetFg();
                    switch (foreground_layer.attributes)
                    {
                        case 0:
                            break;
                        case 1:
                            bm2.RotateFlip(RotateFlipType.RotateNoneFlipX);
                            break;
                        case 2:
                            bm2.RotateFlip(RotateFlipType.RotateNoneFlipY);
                            break;
                        case 3:
                            bm2.RotateFlip(RotateFlipType.RotateNoneFlipXY);
                            break;
                    }
                    g.DrawImage(bm2, 0, 0);
                }
                if (bBg)
                {
                    bm2 = GetBg();
                    switch (background_layer.attributes)
                    {
                        case 0:
                            break;
                        case 1:
                            bm2.RotateFlip(RotateFlipType.RotateNoneFlipX);
                            break;
                        case 2:
                            bm2.RotateFlip(RotateFlipType.RotateNoneFlipY);
                            break;
                        case 3:
                            bm2.RotateFlip(RotateFlipType.RotateNoneFlipXY);
                            break;
                    }
                    g.DrawImage(bm2, 0, 0);
                }
                if (bSprite)
                {
                    bm2 = getsprite1();
                    switch (Machine.sDirection)
                    {
                        case "":
                            break;
                        case "90":
                            bm2.RotateFlip(RotateFlipType.Rotate90FlipNone);
                            break;
                        case "180":
                            bm2.RotateFlip(RotateFlipType.Rotate180FlipNone);
                            break;
                        case "270":
                            bm2.RotateFlip(RotateFlipType.Rotate270FlipNone);
                            break;
                    }
                    g.DrawImage(bm2, 0, 0);
                }                
                if (bText)
                {
                    bm2 = GetText();
                    switch (text_layer.attributes)
                    {
                        case 0:
                            break;
                        case 1:
                            bm2.RotateFlip(RotateFlipType.RotateNoneFlipX);
                            break;
                        case 2:
                            bm2.RotateFlip(RotateFlipType.RotateNoneFlipY);
                            break;
                        case 3:
                            bm2.RotateFlip(RotateFlipType.RotateNoneFlipXY);
                            break;
                    }
                    g.DrawImage(bm2, 0, 0);
                }
            }
            return bm1;
        }
    }
}
