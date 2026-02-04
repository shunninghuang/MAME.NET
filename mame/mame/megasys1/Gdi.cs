using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace mame
{
    public partial class Megasys1
    {
        public static bool bSprite;
        public static void GDIInit()
        {

        }
        public static Bitmap GetSprite()
        {
            Bitmap bm1;
            bm1 = new Bitmap(0x200, 0x200);
            int color, code, sx, sy, flipx, flipy, attr, sprite, offs, color_mask;
            int objectdata_offset, spritedata_offset;
            int xdir,ydir,i5, i6,iByte;
            Color c1 = new Color();
            if (hardware_type_z == 0)
            {
                color_mask = (megasys1_sprite_flag & 0x100) != 0 ? 0x07 : 0x0f;
                for (offs = (0x800 - 8) / 2; offs >= 0; offs -= 8 / 2)
                {
                    for (sprite = 0; sprite < 4; sprite++)
                    {
                        objectdata_offset = offs + 0x400 * sprite;
                        spritedata_offset = 0x4000 + (megasys1_objectram[objectdata_offset] & 0x7f) * 8;
                        attr = Memory.mainram[mainram_offset + spritedata_offset * 2 + 8] * 0x100 + Memory.mainram[mainram_offset + spritedata_offset * 2 + 9];
                        if (((attr & 0xc0) >> 6) != sprite)
                        {
                            continue;
                        }
                        sx = (Memory.mainram[mainram_offset + spritedata_offset * 2 + 0x0a] * 0x100 + Memory.mainram[mainram_offset + spritedata_offset * 2 + 0x0b] + megasys1_objectram[objectdata_offset + 0x02 / 2]) % 0x200;
                        sy = (Memory.mainram[mainram_offset + spritedata_offset * 2 + 0x0c] * 0x100 + Memory.mainram[mainram_offset + spritedata_offset * 2 + 0x0d] + megasys1_objectram[objectdata_offset + 0x04 / 2]) % 0x200;
                        if (sx > 256 - 1)
                        {
                            sx -= 512;
                        }
                        if (sy > 256 - 1)
                        {
                            sy -= 512;
                        }
                        flipx = attr & 0x40;
                        flipy = attr & 0x80;
                        if ((megasys1_screen_flag & 1) != 0)
                        {
                            flipx = (flipx == 0 ? 1 : 0);
                            flipy = (flipy == 0 ? 1 : 0);
                            sx = 240 - sx;
                            sy = 240 - sy;
                        }
                        code = Memory.mainram[mainram_offset + spritedata_offset * 2 + 0x0e] * 0x100 + Memory.mainram[mainram_offset + spritedata_offset * 2 + 0x0f] + megasys1_objectram[objectdata_offset + 0x06 / 2];
                        color = (attr & color_mask);
                        code = (code & 0xfff) + ((megasys1_sprite_bank & 1) << 12);
                        if (flipx != 0)
                        {
                            xdir = -1;
                        }
                        else
                        {
                            xdir = 1;
                        }
                        if (flipy != 0)
                        {
                            ydir = -1;
                        }
                        else
                        {
                            ydir = 1;
                        }
                        for (i5 = 0; i5 < 0x10; i5++)
                        {
                            for (i6 = 0; i6 < 0x10; i6++)
                            {
                                iByte = spritesrom[code * 0x100 + i5 + i6 * 0x10];
                                if (iByte != 0x0f)
                                {
                                    c1 = Color.FromArgb((int)Palette.entry_color[0x300+ 0x10 * color + iByte]);
                                    bm1.SetPixel(sx + xdir * i5, sy + ydir * i6, c1);
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
            if (bSprite)
            {
                bm2 = GetSprite();
                g.DrawImage(bm2, 0, 0);
            }
            return bm1;
        }
    }
}
