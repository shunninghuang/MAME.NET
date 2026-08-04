using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mame
{
    public partial class Palette
    {
        public class shadow_table_data
        {
            public uint[] data;
            public short dr;
            public short dg;
            public short db;
            public byte noclip;
        }
        public static shadow_table_data[] shadow_table;
        public static bitmap_t bitmap;
        public static bitmap_t[] bbitmap;
        public enum VIDEOATTRIBUTE
        {
            VIDEO_UPDATE_AFTER_VBLANK = 0x0004,
            VIDEO_HAS_SHADOWS = 0x0010,
            VIDEO_HAS_HIGHLIGHTS = 0x0020,
            VIDEO_BUFFERS_SPRITERAM = 0x40,
        }        
        public static void palette_set_shadow_mode(int mode)
        {
            Drawgfx.imode = mode;
        }
        public static void palette_set_shadow_dRGB32(int mode, int dr, int dg, int db, int noclip)
        {
            int i;
            if (dr < -0xff)
            {
                dr = -0xff;
            }
            else if (dr > 0xff)
            {
                dr = 0xff;
            }
            if (dg < -0xff)
            {
                dg = -0xff;
            }
            else if (dg > 0xff)
            {
                dg = 0xff;
            }
            if (db < -0xff)
            {
                db = -0xff;
            }
            else if (db > 0xff)
            {
                db = 0xff;
            }
            if (dr == shadow_table[mode].dr && dg == shadow_table[mode].dg && db == shadow_table[mode].db && noclip == shadow_table[mode].noclip)
            {
                return;
            }
            shadow_table[mode].dr = (short)dr;
            shadow_table[mode].dg = (short)dg;
            shadow_table[mode].db = (short)db;
            shadow_table[mode].noclip = (byte)noclip;
            for (i = 0; i < 32768; i++)
            {
                int r = pal5bit((byte)(i >> 10)) + dr;
                int g = pal5bit((byte)(i >> 5)) + dg;
                int b = pal5bit((byte)(i >> 0)) + db;
                uint final;
                if (noclip == 0)
                {
                    r = rgb_clamp(r);
                    g = rgb_clamp(g);
                    b = rgb_clamp(b);
                }
                final = make_rgb(r, g, b);
                shadow_table[mode].data[i] = final;
            }
        }
        public static void allocate_palette()
        {
            if ((Video.video_attributes & (int)VIDEOATTRIBUTE.VIDEO_HAS_SHADOWS) != 0)
            {
                palette_group_set_contrast(1, (float)0.6);
            }
            if ((Video.video_attributes & (int)VIDEOATTRIBUTE.VIDEO_HAS_HIGHLIGHTS) != 0)
            {
                palette_group_set_contrast(2, (float)(1 / 0.6));
            }
        }
        public static void allocate_shadow_tables()
        {
            uint[] table = new uint[0x10000];
            int i;
            Palette.shadow_table = new shadow_table_data[4];
            for (i = 0; i < 4; i++)
            {
                shadow_table[i] = new shadow_table_data();
                shadow_table[i].data = new uint[0x10000];
            }
            if ((Video.video_attributes & (int)VIDEOATTRIBUTE.VIDEO_HAS_SHADOWS) != 0)
            {
                if (format == bitmap_format.BITMAP_FORMAT_INDEXED16)
                {
                    for (i = 0; i < 65536; i++)
                    {
                        table[i] = (uint)((i < numcolors) ? (i + numcolors) : i);
                    }
                    Array.Copy(table, Palette.shadow_table[0].data, 0x10000);
                    Array.Copy(table, Palette.shadow_table[2].data, 0x10000);
                }
                else
                {
                    configure_rgb_shadows(0, (float)0.6);
                }                
            }
            if ((Video.video_attributes & (int)VIDEOATTRIBUTE.VIDEO_HAS_HIGHLIGHTS) != 0)
            {
                if (format == bitmap_format.BITMAP_FORMAT_INDEXED16)
                {
                    for (i = 0; i < 65536; i++)
                    {
                        table[i] = (uint)((i < numcolors) ? (i + 2 * numcolors) : i);
                    }
                    Array.Copy(table, Palette.shadow_table[1].data, 0x10000);
                    Array.Copy(table, Palette.shadow_table[3].data, 0x10000);
                }
                else
                {
                    configure_rgb_shadows(1, (float)(1 / 0.6));
                }
            }
        }
        public static void palette_set_brightness(int pen, double bright)
        {
            palette_entry_set_contrast(pen, (float)bright);
        }
        public static void configure_rgb_shadows(int mode, float factor)
        {
            int ifactor = (int)(factor * 256.0f);
            int i;
            for (i = 0; i < 32768; i++)
            {
                byte r = rgb_clamp((pal5bit((byte)(i >> 10)) * ifactor) >> 8);
                byte g = rgb_clamp((pal5bit((byte)(i >> 5)) * ifactor) >> 8);
                byte b = rgb_clamp((pal5bit((byte)(i >> 0)) * ifactor) >> 8);
                uint final = make_rgb(r, g, b);
                if (format == bitmap_format.BITMAP_FORMAT_RGB32)
                {
                    shadow_table[mode].data[i] = final;
                }
                else
                {
                    shadow_table[mode].data[i] = rgb_to_rgb15(final);
                }
            }
        }
    }
}
