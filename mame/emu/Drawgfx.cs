using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace mame
{
    public class bitmap_t
    {
        //void *alloc;
        public ushort[] uu1;
        public uint[] ui1;
        public int rowpixels;
        public int width;
        public int height;
        //public bitmap_format format;
        public int bpp;
        //palette_t *palette;
    }
    public enum bitmap_format
    {
        BITMAP_FORMAT_INVALID = 0,
        BITMAP_FORMAT_INDEXED8,
        BITMAP_FORMAT_INDEXED16,
        BITMAP_FORMAT_INDEXED32,
        BITMAP_FORMAT_RGB15,
        BITMAP_FORMAT_RGB32,
        BITMAP_FORMAT_ARGB32,
        BITMAP_FORMAT_YUY16,
        BITMAP_FORMAT_LAST
    }
    public struct alpha_cache
    {
        public int alphas;
        public int alphad;
    }
    public struct gfx_element
    {
        public ushort width;
        public ushort height;
        public byte flags;
        public uint total_elements;
        public uint color_base;
        public ushort color_depth;
        public ushort color_granularity;
        public uint total_colors;
        //UINT32 *		pen_usage;
        public byte[] gfxdata;
        public uint line_modulo;
        public uint char_modulo;
        //gfx_layout		layout;
    }    
    public partial class Drawgfx
    {
        public static alpha_cache drawgfx_alpha_cache;

        public static int[] gfx_drawmode_table = new int[256];
        public static int afterdrawmask;        
        public static int imode;
        public static int spritecount;
        public static void alpha_set_level(int level)
        {
            drawgfx_alpha_cache.alphas = level;
            drawgfx_alpha_cache.alphad = 256 - level;
        }
        public static uint alpha_blend32(uint d, uint s)
        {
            return (uint)(((((s & 0x0000ff) * drawgfx_alpha_cache.alphas + (d & 0x0000ff) * drawgfx_alpha_cache.alphad) >> 8)) |
                   ((((s & 0x00ff00) * drawgfx_alpha_cache.alphas + (d & 0x00ff00) * drawgfx_alpha_cache.alphad) >> 8) & 0x00ff00) |
                   ((((s & 0xff0000) * drawgfx_alpha_cache.alphas + (d & 0xff0000) * drawgfx_alpha_cache.alphad) >> 8) & 0xff0000));
        }
        public static void copybitmap_core16(ushort[] uu1, bitmap_t dest, bitmap_t src, int flipx, int flipy, int sx, int sy, RECT clip, int transparency, int transparent_color)
        {
            int ox;
            int oy;
            int ex;
            int ey;
            int offsetx, offsety, xdir, ydir;
            ox = sx;
            oy = sy;
            ex = sx + src.width - 1;
            if (sx < 0)
            {
                sx = 0;
            }
            if (sx < clip.min_x)
            {
                sx = clip.min_x;
            }
            if (ex >= dest.width)
            {
                ex = dest.width - 1;
            }
            if (ex > clip.max_x)
            {
                ex = clip.max_x;
            }
            if (sx > ex)
            {
                return;
            }
            ey = sy + src.height - 1;
            if (sy < 0)
            {
                sy = 0;
            }
            if (sy < clip.min_y)
            {
                sy = clip.min_y;
            }
            if (ey >= dest.height)
            {
                ey = dest.height - 1;
            }
            if (ey > clip.max_y)
            {
                ey = clip.max_y;
            }
            if (sy > ey)
            {
                return;
            }
            int sw = ex - sx + 1;
            int sh = ey - sy + 1;
            int sm = src.rowpixels;
            int dm = dest.rowpixels;
            if (flipx!=0)
            {
                offsetx = src.width - 1 - (sx - ox);
                xdir = -1;
            }
            else
            {
                offsetx = sx - ox;
                xdir = 1;
            }
            if (flipy!=0)
            {
                offsety = (src.height - 1 - (sy - oy));
                ydir = -1;
            }
            else
            {
                offsety = sy - oy;
                ydir = 1;
            }
            switch (transparency)
            {
                case 0:
                    break;
                case 1:
                    int i, j;
                    for (i = 0; i < sh; i++)
                    {
                        for (j = 0; j < sw; j++)
                        {
                            int col;
                            col = uu1[(offsety + ydir * i) * sm + offsetx + xdir * j];
                            if (col != transparent_color)
                            {
                                Video.bitmapbase[Video.curbitmap][(sy + i) * dm + sx + j] = (ushort)col;
                            }
                        }
                    }
                    break;
            }
        }
        public static void copybitmap_common(ushort[] uu1, bitmap_t dest, bitmap_t src, int flipx, int flipy, int sx, int sy, RECT clip, int transparency, int transparent_color)
        {
            copybitmap_core16(uu1, dest, src, flipx, flipy, sx, sy, clip, transparency, transparent_color);
        }
        public static void copybitmap_trans(ushort[] uu1, bitmap_t dest, bitmap_t src, int flipx, int flipy, int sx, int sy, RECT clip, int transparent_pen)
        {
            copybitmap_common(uu1, dest, src, flipx, flipy, sx, sy, clip, 1, transparent_pen);
        }
        public static void SaveStateBinary(BinaryWriter writer)
        {
            writer.Write(imode);
        }
        public static void LoadStateBinary(BinaryReader reader)
        {
            imode = reader.ReadInt32();
        }
    }
}
