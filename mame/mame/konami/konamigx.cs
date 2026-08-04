using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace mame
{
    public partial class Konami
    {
        public static int counta;
        public static RECT[] K053936_cliprect;
        public static int[] K053936_clip_enabled;
        public static int[] colormask = new int[8] { 1, 3, 7, 0xf, 0x1f, 0x3f, 0x7f, 0xff };


        public struct GX_OBJ
        {
            public int order, offs, code, color;
        }
        public static GX_OBJ[] gx_objpool;
        public static byte[] gx_shdzbuf;
        public static int gx_objptr_offset;
        public static ushort[] gx_spriteram;
        public static int gx_objdma, gx_primode;

        public static byte konamigx_wrport1_0, konamigx_wrport1_1;
        public static ushort konamigx_wrport2;
        
        public static int K053246_objset1;
        public static int[] K053247_vrcbk;
        public static int K053247_coreg, K053247_coregshift, K053247_opset;
        public static int opri;
        public static int[] vcblk;
        public static int ocblk;
        public static int vinmix, vmixon, osinmix, osmixon;

        public static void SaveStateBinary_konamigx(BinaryWriter writer)
        {
            int i;
            writer.Write(konamigx_wrport1_0);
            writer.Write(konamigx_wrport1_1);
            writer.Write(konamigx_wrport2);
            writer.Write(K053246_objset1);
            for (i = 0; i < 4; i++)
            {
                writer.Write(K053247_vrcbk[i]);
            }
            writer.Write(K053247_coreg);
            writer.Write(K053247_coregshift);
            writer.Write(K053247_opset);
            writer.Write(opri);
            for (i = 0; i < 6; i++)
            {
                writer.Write(vcblk[i]);
            }
            writer.Write(ocblk);
            writer.Write(vinmix);
            writer.Write(vmixon);
            writer.Write(osinmix);
            writer.Write(osmixon);

        }
        public static void LoadStateBinary_konamigx(BinaryReader reader)
        {
            int i;
            konamigx_wrport1_0=reader.ReadByte();
            konamigx_wrport1_1=reader.ReadByte();
            konamigx_wrport2 = reader.ReadUInt16();
            K053246_objset1 = reader.ReadInt32();
            for (i = 0; i < 4; i++)
            {
                K053247_vrcbk[i] = reader.ReadInt32();
            }
            K053247_coreg = reader.ReadInt32();
            K053247_coregshift = reader.ReadInt32();
            K053247_opset = reader.ReadInt32();
            opri = reader.ReadInt32();
            for (i = 0; i < 6; i++)
            {
                vcblk[i] = reader.ReadInt32();
            }
            ocblk = reader.ReadInt32();
            vinmix = reader.ReadInt32();
            vmixon = reader.ReadInt32();
            osinmix = reader.ReadInt32();
            osmixon = reader.ReadInt32();
        }
        public static void K053936GP_set_offset(int chip, int xoffs, int yoffs)
        {
            K053936_offset[chip][0] = xoffs;
            K053936_offset[chip][1] = yoffs;
        }
        public static void K053936GP_clip_enable(int chip, int status)
        {
            K053936_clip_enabled[chip] = status;
        }
        public static void K053936GP_set_cliprect(int chip, int minx, int maxx, int miny, int maxy)
        {
            K053936_cliprect[chip].min_x = minx;
            K053936_cliprect[chip].max_x = maxx;
            K053936_cliprect[chip].min_y = miny;
            K053936_cliprect[chip].max_y = maxy;
        }
        public static unsafe void K053936GP_copyroz32clip(bitmap_t dst_bitmap,int src_bitmap_rowpixels, ushort[] src_bitmap_data,RECT *dst_cliprect,RECT *src_cliprect, uint _startx,uint _starty,int _incxx,int _incxy,int _incyx,int _incyy,int tilebpp, int blend, int clip)
        {
	        int cy, cx;
	        int ecx;
	        int src_pitch, incxy, incxx;
	        int src_minx, src_maxx, src_miny, src_maxy, cmask;
            int dst_base_offset;
	        int tx, dst_pitch;
	        int starty, incyy, startx, incyx, ty, sx, sy;
	        incxy = _incxy; incxx = _incxx; incyy = _incyy; incyx = _incyx;
	        starty = (int)_starty;
            startx = (int)_startx;
	        if (src_cliprect!=null && clip!=0)
	        {
		        src_minx = src_cliprect->min_x;
		        src_maxx = src_cliprect->max_x;
		        src_miny = src_cliprect->min_y;
		        src_maxy = src_cliprect->max_y;
	        }
	        else
            {
                src_minx = src_miny = -0x10000;
                src_maxx = src_maxy = 0x10000;
            }
	        if (dst_cliprect!=null)
	        {
		        sx = dst_cliprect->min_x;
		        tx = dst_cliprect->max_x - sx + 1;
		        sy = dst_cliprect->min_y;
		        ty = dst_cliprect->max_y - sy + 1;
		        startx += sx * incxx + sy * incyx;
		        starty += sx * incxy + sy * incyy;
	        }
	        else
            {
                sx = sy = 0;
                tx = dst_bitmap.width;
                ty = dst_bitmap.height;
            }
	        dst_pitch = dst_bitmap.rowpixels;
            dst_base_offset = sy * dst_pitch + sx + tx;
	        ecx = tx = -tx;
	        tilebpp = (tilebpp-1) & 7;
	        cmask = colormask[tilebpp];
	        src_pitch = src_bitmap_rowpixels;
	        cy = starty;
	        cx = startx;
	        if (blend > 0)
	        {
                dst_base_offset += dst_pitch;
		        starty += incyy;
		        startx += incyx;
		        do
                {
			        do
                    {
				        int srcx = (cx >> 16) & 0x1fff;
				        int srcy = (cy >> 16) & 0x1fff;
				        int pixel;
				        cx += incxx;
				        cy += incxy;
				        if (srcx < src_minx || srcx > src_maxx || srcy < src_miny || srcy > src_maxy)
                        {
					        continue;
                        }
                        pixel = src_bitmap_data[srcy * src_pitch + srcx];
				        if ((pixel & cmask)==0)
                        {
					        continue;
                        }
                        dst_bitmap.uu1[dst_base_offset + ecx] = (ushort)Drawgfx.alpha_blend32(0, (uint)dst_bitmap.uu1[dst_base_offset + ecx]);
			        }
			        while (++ecx!=0);
			        ecx = tx;
                    dst_base_offset += dst_pitch;
			        cy = starty;
                    starty += incyy;
			        cx = startx;
                    startx += incyx;
		        }
                while (--ty!=0);
	        }
	        else
	        {
		        if (blend == 0)
		        {
                    dst_base_offset += dst_pitch;
			        starty += incyy;
			        startx += incyx;
		        }
		        else
		        {
			        if (((sy & 1) ^ (blend & 1))!=0)
			        {
				        if (ty <= 1)
                        {
                            return;
                        }
                        dst_base_offset += dst_pitch;
				        cy += incyy;
				        cx += incyx;
			        }
			        if (ty > 1)
			        {
				        ty >>= 1;
				        dst_pitch <<= 1;
				        incyy <<= 1;
				        incyx <<= 1;
                        dst_base_offset += dst_pitch;
				        starty = cy + incyy;
				        startx = cx + incyx;
			        }
		        }
		        do
                {
			        do
                    {
				        int srcx = (cx >> 16) & 0x1fff;
				        int srcy = (cy >> 16) & 0x1fff;
				        int pixel;
				        cx += incxx;
				        cy += incxy;
				        if (srcx < src_minx || srcx > src_maxx || srcy < src_miny || srcy > src_maxy)
                        {
					        continue;
                        }
                        pixel = src_bitmap_data[srcy * src_pitch + srcx];
				        if ((pixel & cmask)==0)
                        {
					        continue;
                        }
                        dst_bitmap.uu1[dst_base_offset + ecx] = 0;
			        }
			        while (++ecx!=0);
			        ecx = tx;
                    dst_base_offset += dst_pitch;
			        cy = starty; starty += incyy;
			        cx = startx; startx += incyx;
		        }
                while (--ty!=0);
	        }
        }
        public static unsafe void K053936GP_zoom_draw(int chip, ushort[] ctrl, ushort[] linectrl, bitmap_t bitmap, RECT cliprect, Tmap tmap, int tilebpp, int blend)
        {
            ushort[] src_bitmap_data;
            RECT src_cliprect;
            int lineaddr_offset;
            int src_bitmap_rowpixels;
            RECT my_clip;
            uint startx, starty;
            int incxx, incxy, incyx, incyy, y, maxy, clip;
            src_bitmap_data = tmap.tilemap_get_pixmap();
            src_bitmap_rowpixels = tmap.width;
            src_cliprect = K053936_cliprect[chip];
            clip = K053936_clip_enabled[chip];
            if ((ctrl[0x07] & 0x0040) != 0)
            {
                my_clip.min_x = cliprect.min_x;
                my_clip.max_x = cliprect.max_x;
                y = cliprect.min_y;
                maxy = cliprect.max_y;
                while (y <= maxy)
                {
                    lineaddr_offset = (((y - K053936_offset[chip][1]) & 0x1ff) << 2);
                    my_clip.min_y = my_clip.max_y = y;
                    startx = (uint)((short)(linectrl[lineaddr_offset + 0] + ctrl[0x00]) << 8);
                    starty = (uint)((short)(linectrl[lineaddr_offset + 1] + ctrl[0x01]) << 8);
                    incxx = (int)((short)(linectrl[lineaddr_offset + 2]));
                    incxy = (int)((short)(linectrl[lineaddr_offset + 3]));
                    if ((ctrl[0x06] & 0x8000) != 0)
                    {
                        incxx <<= 8;
                    }
                    if ((ctrl[0x06] & 0x0080) != 0)
                    {
                        incxy <<= 8;
                    }
                    startx -= (uint)(K053936_offset[chip][0] * incxx);
                    starty -= (uint)(K053936_offset[chip][0] * incxy);
                    K053936GP_copyroz32clip(bitmap, src_bitmap_rowpixels, src_bitmap_data, &my_clip, &src_cliprect, startx << 5, starty << 5, incxx << 5, incxy << 5, 0, 0, tilebpp, blend, clip);
                    y++;
                }
            }
            else
            {
                startx = (uint)((short)(ctrl[0x00]) << 8);
                starty = (uint)((short)(ctrl[0x01]) << 8);
                incyx = (short)(ctrl[0x02]);
                incyy = (short)(ctrl[0x03]);
                incxx = (short)(ctrl[0x04]);
                incxy = (short)(ctrl[0x05]);

                if ((ctrl[0x06] & 0x4000) != 0)
                {
                    incyx <<= 8;
                    incyy <<= 8;
                }
                if ((ctrl[0x06] & 0x0040) != 0)
                {
                    incxx <<= 8;
                    incxy <<= 8;
                }
                startx -= (uint)(K053936_offset[chip][1] * incyx);
                starty -= (uint)(K053936_offset[chip][1] * incyy);
                startx -= (uint)(K053936_offset[chip][0] * incxx);
                starty -= (uint)(K053936_offset[chip][0] * incxy);
                K053936GP_copyroz32clip(bitmap, src_bitmap_rowpixels, src_bitmap_data, &cliprect, &src_cliprect, startx << 5, starty << 5, incxx << 5, incxy << 5, incyx << 5, incyy << 5, tilebpp, blend, clip);
            }
        }
        public static void K053936GP_0_zoom_draw(bitmap_t bitmap, RECT cliprect, Tmap tmap, int tilebpp, int blend)
        {
            K053936GP_zoom_draw(0, K053936_0_ctrl, K053936_0_linectrl, bitmap, cliprect, tmap, tilebpp, blend);
        }
        public static void K053936GP_1_zoom_draw(bitmap_t bitmap, RECT cliprect, Tmap tmap, int tilebpp, int blend)
        {
            K053936GP_zoom_draw(1, K053936_1_ctrl, K053936_1_linectrl, bitmap, cliprect, tmap, tilebpp, blend);
        }
        public static void zdrawgfxzoom32GP(bitmap_t bitmap, gfx_element gfx, RECT cliprect, uint code, uint color, int flipx, int flipy, int sx, int sy, int scalex, int scaley, int alpha, int drawmode, int zcode, int pri)
        {
            int src_ptr_offset;
            int src_x;
            int eax, ecx;
            int src_fx, src_fdx;
            int shdpen;
            byte z8, db0, p8, db1;
            int ozbuf_ptr_offset;
            int szbuf_ptr_offset;
            int pal_base_offset;
            int dst_ptr_offset;
            int src_fby, src_fdy, src_fbx;
            int src_base_offset;
            int dst_w, dst_h;
            int nozoom, granularity;
            int src_fw, src_fh;
            int dst_minx, dst_maxx, dst_miny, dst_maxy;
            int dst_skipx, dst_skipy, dst_x, dst_y, dst_lastx, dst_lasty;
            int src_pitch, dst_pitch;
            if (scalex == 0 || scaley == 0)
            {
                return;
            }
            granularity = shdpen = gfx.color_granularity;
            shdpen--;
            if (zcode >= 0)
            {
                if (drawmode == 5)
                {
                    drawmode = 4;
                    shdpen = 1;
                }
            }
            else if (drawmode >= 4)
            {
                return;
            }
            if ((drawmode & 2) != 0)
            {
                if (alpha <= 0)
                {
                    return;
                }
                if (alpha >= 255)
                {
                    drawmode &= ~2;
                }
            }
            ozbuf_ptr_offset = 0;
            szbuf_ptr_offset = 0;
            src_pitch = 16;
            src_fw = 16;
            src_fh = 16;
            src_base_offset = (int)((code % gfx.total_elements) * gfx.char_modulo);
            pal_base_offset = (int)(gfx.color_base + (color % gfx.total_colors) * granularity);
            dst_ptr_offset = 0;
            dst_pitch = bitmap.rowpixels;
            dst_minx = cliprect.min_x;
            dst_maxx = cliprect.max_x;
            dst_miny = cliprect.min_y;
            dst_maxy = cliprect.max_y;
            dst_x = sx;
            dst_y = sy;
            if (dst_x > dst_maxx || dst_y > dst_maxy)
            {
                return;
            }
            nozoom = (scalex == 0x10000 && scaley == 0x10000) ? 1 : 0;
            if (nozoom != 0)
            {
                dst_h = dst_w = 16;
                src_fdy = src_fdx = 1;
            }
            else
            {
                dst_w = ((scalex << 4) + 0x8000) >> 16;
                dst_h = ((scaley << 4) + 0x8000) >> 16;
                if (dst_w == 0 || dst_h == 0)
                {
                    return;
                }
                src_fw <<= 19;
                src_fh <<= 19;
                src_fdx = src_fw / dst_w;
                src_fdy = src_fh / dst_h;
            }
            dst_lastx = dst_x + dst_w - 1;
            if (dst_lastx < dst_minx)
            {
                return;
            }
            dst_lasty = dst_y + dst_h - 1;
            if (dst_lasty < dst_miny)
            {
                return;
            }
            dst_skipx = 0;
            eax = dst_minx;
            if ((eax -= dst_x) > 0)
            {
                dst_skipx = eax;
                dst_w -= eax;
                dst_x = dst_minx;
            }
            eax = dst_lastx;
            if ((eax -= dst_maxx) > 0)
            {
                dst_w -= eax;
            }
            dst_skipy = 0;
            eax = dst_miny;
            if ((eax -= dst_y) > 0)
            {
                dst_skipy = eax;
                dst_h -= eax;
                dst_y = dst_miny;
            }
            eax = dst_lasty;
            if ((eax -= dst_maxy) > 0)
            {
                dst_h -= eax;
            }
            if (nozoom != 0)
            {
                if (flipx == 0)
                {
                    src_fbx = 0;
                }
                else
                {
                    src_fbx = src_fw - 1;
                    src_fdx = -src_fdx;
                }
                if (flipy == 0)
                {
                    src_fby = 0;
                }
                else
                {
                    src_fby = src_fh - 1;
                    src_fdy = -src_fdy;
                    src_pitch = -src_pitch;
                }
            }
            else
            {
                if (flipx == 0)
                {
                    src_fbx = 0;
                }
                else
                {
                    src_fbx = src_fw - 0 - 1;
                    src_fdx = -src_fdx;
                }
                if (flipy == 0)
                {
                    src_fby = 0;
                }
                else
                {
                    src_fby = src_fh - 0 - 1;
                    src_fdy = -src_fdy;
                }
            }
            src_fbx += dst_skipx * src_fdx;
            src_fby += dst_skipy * src_fdy;
            eax = (dst_y - dst_miny) * 384 + (dst_x - dst_minx) + dst_w;
            db0 = z8 = (byte)zcode;
            db1 = p8 = (byte)pri;
            ozbuf_ptr_offset += eax;
            szbuf_ptr_offset += eax << 1;
            dst_ptr_offset += dst_y * dst_pitch + dst_x + dst_w;
            dst_w = -dst_w;
            if (nozoom == 0)
            {
                ecx = src_fby;
                src_fby += src_fdy;
                ecx >>= 19;
                src_fx = src_fbx;
                src_x = src_fbx;
                src_fx += src_fdx;
                ecx <<= 4;
                src_ptr_offset = src_base_offset;
                src_x >>= 19;
                src_ptr_offset += ecx;
                ecx = dst_w;
                if (zcode < 0)
                {
                    do
                    {
                        do
                        {
                            eax = gfx.gfxdata[src_ptr_offset + src_x];
                            src_x = src_fx;
                            src_fx += src_fdx;
                            src_x >>= 19;
                            if (eax == 0 || eax >= shdpen)
                            {
                                continue;
                            }
                            bitmap.ui1[ecx] = Palette.entry_color2[pal_base_offset + eax];
                        }
                        while (++ecx != 0);
                        ecx = src_fby;
                        src_fby += src_fdy;
                        dst_ptr_offset += dst_pitch;
                        ecx >>= 19;
                        src_fx = src_fbx;
                        src_x = src_fbx;
                        src_fx += src_fdx;
                        ecx <<= 4;
                        src_ptr_offset = src_base_offset;
                        src_x >>= 19;
                        src_ptr_offset += ecx;
                        ecx = dst_w;
                    }
                    while (--dst_h != 0);
                }
                else
                {
                    switch (drawmode)
                    {
                        case 0:
                            do
                            {
                                do
                                {
                                    eax = gfx.gfxdata[src_ptr_offset + src_x];
                                    src_x = src_fx;
                                    src_fx += src_fdx;
                                    src_x >>= 19;
                                    if (eax == 0 || Tilemap.ppriority_bitmap[ozbuf_ptr_offset + ecx] < z8)
                                    {
                                        continue;
                                    }
                                    eax = (int)Palette.entry_color2[pal_base_offset + eax];
                                    Tilemap.ppriority_bitmap[ozbuf_ptr_offset] = z8;
                                    bitmap.ui1[dst_ptr_offset + ecx] = (uint)eax;
                                }
                                while (++ecx != 0);
                                ecx = src_fby;
                                src_fby += src_fdy;
                                ozbuf_ptr_offset += 384;
                                dst_ptr_offset += dst_pitch;
                                ecx >>= 19;
                                src_fx = src_fbx;
                                src_x = src_fbx;
                                src_fx += src_fdx;
                                ecx <<= 4;
                                src_ptr_offset = src_base_offset;
                                src_x >>= 19;
                                src_ptr_offset += ecx;
                                ecx = dst_w;
                            }
                            while (--dst_h != 0);
                            break;
                        case 1:
                            do
                            {
                                do
                                {
                                    eax = gfx.gfxdata[src_ptr_offset + src_x];
                                    src_x = src_fx;
                                    src_fx += src_fdx;
                                    src_x >>= 19;
                                    if (eax == 0 || eax >= shdpen || Tilemap.ppriority_bitmap[ozbuf_ptr_offset + ecx] < z8)
                                    {
                                        continue;
                                    }
                                    eax = (int)Palette.entry_color2[pal_base_offset + eax];
                                    Tilemap.ppriority_bitmap[ozbuf_ptr_offset + ecx] = z8;
                                    bitmap.ui1[dst_ptr_offset + ecx] = (uint)eax;
                                }
                                while (++ecx != 0);
                                ecx = src_fby;
                                src_fby += src_fdy;
                                ozbuf_ptr_offset += 384;
                                dst_ptr_offset += dst_pitch;
                                ecx >>= 19;
                                src_fx = src_fbx;
                                src_x = src_fbx;
                                src_fx += src_fdx;
                                ecx <<= 4;
                                src_ptr_offset = src_base_offset;
                                src_x >>= 19;
                                src_ptr_offset += ecx;
                                ecx = dst_w;
                            }
                            while (--dst_h != 0);
                            break;
                        case 2:
                            do
                            {
                                do
                                {
                                    eax = gfx.gfxdata[src_ptr_offset + src_x];
                                    src_x = src_fx;
                                    src_fx += src_fdx;
                                    src_x >>= 19;
                                    if (eax == 0 || Tilemap.ppriority_bitmap[ozbuf_ptr_offset + ecx] < z8)
                                    {
                                        continue;
                                    }
                                    Tilemap.ppriority_bitmap[ozbuf_ptr_offset + ecx] = z8;
                                    bitmap.ui1[dst_ptr_offset + ecx] = Drawgfx.alpha_blend32(Palette.entry_color2[pal_base_offset + eax], bitmap.ui1[dst_ptr_offset + ecx]);
                                }
                                while (++ecx != 0);
                                ecx = src_fby;
                                src_fby += src_fdy;
                                ozbuf_ptr_offset += 384;
                                dst_ptr_offset += dst_pitch;
                                ecx >>= 19;
                                src_fx = src_fbx;
                                src_x = src_fbx;
                                src_fx += src_fdx;
                                ecx <<= 4;
                                src_ptr_offset = src_base_offset;
                                src_x >>= 19;
                                src_ptr_offset += ecx;
                                ecx = dst_w;
                            }
                            while (--dst_h != 0);
                            break;
                        case 3:
                            do
                            {
                                do
                                {
                                    eax = gfx.gfxdata[src_ptr_offset + src_x];
                                    src_x = src_fx;
                                    src_fx += src_fdx;
                                    src_x >>= 19;
                                    if (eax == 0 || eax >= shdpen || Tilemap.ppriority_bitmap[ozbuf_ptr_offset + ecx] < z8)
                                    {
                                        continue;
                                    }
                                    Tilemap.ppriority_bitmap[ozbuf_ptr_offset + ecx] = z8;
                                    bitmap.ui1[dst_ptr_offset + ecx] = Drawgfx.alpha_blend32(Palette.entry_color2[pal_base_offset + eax], bitmap.ui1[dst_ptr_offset + ecx]);
                                }
                                while (++ecx != 0);
                                ecx = src_fby;
                                src_fby += src_fdy;
                                ozbuf_ptr_offset += 384;
                                dst_ptr_offset += dst_pitch;
                                ecx >>= 19;
                                src_fx = src_fbx;
                                src_x = src_fbx;
                                src_fx += src_fdx;
                                ecx <<= 4;
                                src_ptr_offset = src_base_offset;
                                src_x >>= 19;
                                src_ptr_offset += ecx;
                                ecx = dst_w;
                            }
                            while (--dst_h != 0);
                            break;
                        case 4:
                            do
                            {
                                do
                                {
                                    eax = gfx.gfxdata[src_ptr_offset + src_x];
                                    src_x = src_fx;
                                    src_fx += src_fdx;
                                    src_x >>= 19;
                                    if (eax < shdpen || gx_shdzbuf[szbuf_ptr_offset + ecx * 2] < z8 || gx_shdzbuf[szbuf_ptr_offset + ecx * 2 + 1] <= p8)
                                    {
                                        continue;
                                    }
                                    eax = (int)bitmap.ui1[dst_ptr_offset + ecx];
                                    gx_shdzbuf[szbuf_ptr_offset + ecx * 2] = z8;
                                    gx_shdzbuf[szbuf_ptr_offset + ecx * 2 + 1] = p8;
                                    eax = (eax >> 9 & 0x7c00) | (eax >> 6 & 0x03e0) | (eax >> 3 & 0x001f);
                                    bitmap.ui1[dst_ptr_offset + ecx] = Palette.shadow_table[0].data[eax];
                                }
                                while (++ecx != 0);
                                ecx = src_fby;
                                src_fby += src_fdy;
                                szbuf_ptr_offset += (384 << 1);
                                dst_ptr_offset += dst_pitch;
                                ecx >>= 19;
                                src_fx = src_fbx;
                                src_x = src_fbx;
                                src_fx += src_fdx;
                                ecx <<= 4;
                                src_ptr_offset = src_base_offset;
                                src_x >>= 19;
                                src_ptr_offset += ecx;
                                ecx = dst_w;
                            }
                            while (--dst_h != 0);
                            break;
                    }
                }
            }
            else
            {
                src_ptr_offset = src_base_offset + (src_fby << 4) + src_fbx;
                src_fdy = src_fdx * dst_w + src_pitch;
                ecx = dst_w;
                if (zcode < 0)
                {
                    do
                    {
                        do
                        {
                            eax = gfx.gfxdata[src_ptr_offset];
                            src_ptr_offset += src_fdx;
                            if (eax == 0 || eax >= shdpen)
                            {
                                continue;
                            }
                            bitmap.ui1[dst_ptr_offset + ecx] = Palette.entry_color2[pal_base_offset + eax];
                        }
                        while (++ecx != 0);
                        src_ptr_offset += src_fdy;
                        dst_ptr_offset += dst_pitch;
                        ecx = dst_w;
                    }
                    while (--dst_h != 0);
                }
                else
                {
                    switch (drawmode)
                    {
                        case 0:
                            do
                            {
                                do
                                {
                                    eax = gfx.gfxdata[src_ptr_offset];
                                    src_ptr_offset += src_fdx;
                                    if (eax == 0 || Tilemap.ppriority_bitmap[ozbuf_ptr_offset + ecx] < z8)
                                    {
                                        continue;
                                    }
                                    eax = (int)Palette.entry_color2[pal_base_offset + eax];
                                    Tilemap.ppriority_bitmap[ozbuf_ptr_offset + ecx] = z8;
                                    if (eax != 0)
                                    {
                                        int i1 = 1;
                                    }
                                    bitmap.ui1[dst_ptr_offset + ecx] = (uint)eax;
                                }
                                while (++ecx != 0);
                                src_ptr_offset += src_fdy;
                                ozbuf_ptr_offset += 384;
                                dst_ptr_offset += dst_pitch;
                                ecx = dst_w;
                            }
                            while (--dst_h != 0);
                            break;
                        case 1:
                            do
                            {
                                do
                                {
                                    eax = gfx.gfxdata[src_ptr_offset];
                                    src_ptr_offset += src_fdx;
                                    if (eax == 0 || eax >= shdpen || Tilemap.ppriority_bitmap[ozbuf_ptr_offset + ecx] < z8)
                                    {
                                        continue;
                                    }
                                    eax = (int)Palette.entry_color2[pal_base_offset + eax];
                                    Tilemap.ppriority_bitmap[ozbuf_ptr_offset + ecx] = z8;
                                    bitmap.ui1[dst_ptr_offset + ecx] = (uint)eax;
                                }
                                while (++ecx != 0);
                                src_ptr_offset += src_fdy;
                                ozbuf_ptr_offset += 384;
                                dst_ptr_offset += dst_pitch;
                                ecx = dst_w;
                            }
                            while (--dst_h != 0);
                            break;
                        case 2:
                            do
                            {
                                do
                                {
                                    eax = gfx.gfxdata[src_ptr_offset];
                                    src_ptr_offset += src_fdx;
                                    if (eax == 0 || Tilemap.ppriority_bitmap[ozbuf_ptr_offset + ecx] < z8)
                                    {
                                        continue;
                                    }
                                    Tilemap.ppriority_bitmap[ozbuf_ptr_offset + ecx] = z8;
                                    bitmap.ui1[dst_ptr_offset + ecx] = Drawgfx.alpha_blend32(Palette.entry_color2[pal_base_offset + eax], bitmap.ui1[dst_ptr_offset + ecx]);
                                }
                                while (++ecx != 0);
                                src_ptr_offset += src_fdy;
                                ozbuf_ptr_offset += 384;
                                dst_ptr_offset += dst_pitch;
                                ecx = dst_w;
                            }
                            while (--dst_h != 0);
                            break;
                        case 3:
                            do
                            {
                                do
                                {
                                    eax = gfx.gfxdata[src_ptr_offset];
                                    src_ptr_offset += src_fdx;
                                    if (eax == 0 || eax >= shdpen || Tilemap.ppriority_bitmap[ozbuf_ptr_offset + ecx] < z8)
                                    {
                                        continue;
                                    }
                                    Tilemap.ppriority_bitmap[ozbuf_ptr_offset + ecx] = z8;
                                    bitmap.ui1[dst_ptr_offset + ecx] = Drawgfx.alpha_blend32(Palette.entry_color2[pal_base_offset + eax], bitmap.ui1[dst_ptr_offset + ecx]);
                                }
                                while (++ecx != 0);
                                src_ptr_offset += src_fdy;
                                ozbuf_ptr_offset += 384;
                                dst_ptr_offset += dst_pitch;
                                ecx = dst_w;
                            }
                            while (--dst_h != 0);
                            break;
                        case 4:
                            do
                            {
                                do
                                {
                                    eax = gfx.gfxdata[src_ptr_offset];
                                    src_ptr_offset += src_fdx;
                                    if (eax < shdpen || gx_shdzbuf[szbuf_ptr_offset + ecx * 2] < z8 || gx_shdzbuf[szbuf_ptr_offset + ecx * 2 + 1] <= p8)
                                    {
                                        continue;
                                    }
                                    eax = (int)bitmap.ui1[dst_ptr_offset + ecx];
                                    gx_shdzbuf[szbuf_ptr_offset + ecx * 2] = z8;
                                    gx_shdzbuf[szbuf_ptr_offset + ecx * 2 + 1] = p8;
                                    eax = (eax >> 9 & 0x7c00) | (eax >> 6 & 0x03e0) | (eax >> 3 & 0x001f);
                                    bitmap.ui1[dst_ptr_offset + ecx] = Palette.shadow_table[0].data[eax];
                                }
                                while (++ecx != 0);
                                src_ptr_offset += src_fdy;
                                szbuf_ptr_offset += (384 << 1);
                                dst_ptr_offset += dst_pitch;
                                ecx = dst_w;
                            }
                            while (--dst_h != 0);
                            break;
                    }
                }
            }
        }
        public static void konamigx_precache_registers()
        {
            int[] coregmasks = new int[5] { 0xf, 0xe, 0xc, 0x8, 0x0 };
            int[] coregshifts = new int[5] { 4, 5, 6, 7, 8 };
            int i;
            K053246_objset1 = K053246_read_register(5);
            i = K053247_read_register(0x8 / 2);
            K053247_vrcbk[0] = (i & 0x000f) << 14;
            K053247_vrcbk[1] = (i & 0x0f00) << 6;
            i = K053247_read_register(0xa / 2);
            K053247_vrcbk[2] = (i & 0x000f) << 14;
            K053247_vrcbk[3] = (i & 0x0f00) << 6;
            K053247_opset = K053247_read_register(0xc / 2);
            i = K053247_opset & 7;
            if (i > 4)
            {
                i = 4;
            }
            K053247_coreg = K053247_read_register(0xc / 2) >> 8 & 0xf;
            K053247_coreg = (K053247_coreg & coregmasks[i]) << 12;
            K053247_coregshift = coregshifts[i];
            opri = K055555_read_register(15);
            oinprion = K055555_read_register(19);
            vcblk[0] = K055555_read_register(23);
            vcblk[1] = K055555_read_register(24);
            vcblk[2] = K055555_read_register(25);
            vcblk[3] = K055555_read_register(26);
            vcblk[4] = K055555_read_register(28);
            vcblk[5] = K055555_read_register(29);
            ocblk = K055555_read_register(27);
            vinmix = K055555_read_register(33);
            vmixon = K055555_read_register(34);
            osinmix = K055555_read_register(35);
            osmixon = K055555_read_register(36);
        }
        public static void gx_wipezbuf(int noshadow)
        {
            int w = Video.screenstate.visarea.max_x - Video.screenstate.visarea.min_x + 1;
            int h = Video.screenstate.visarea.max_y - Video.screenstate.visarea.min_y + 1;
            int offsetx,offsety,offset;
            offsetx = Video.screenstate.visarea.min_x;
            offsety = Video.screenstate.visarea.min_y;
            offset = 0;
            int ecx = h;
            int i;
            do
            {
                for (i = 0; i < w; i++)
                {
                    Tilemap.ppriority_bitmap[offset + i] = 0xff;
                }
                offset += 384;
            }
            while (--ecx!=0);
            if (noshadow == 0)
            {
                offset = 0;
                w <<= 1;
                ecx = h;
                do
                {
                    for (i = 0; i < w; i++)
                    {
                        gx_shdzbuf[offset + i] = 0xff;
                    }
                    offset += 384 << 1;
                }
                while (--ecx != 0);
            }
        }
        public static void konamigx_mixer_init(int objdma)
        {
            K053247_vrcbk = new int[4];
            vcblk = new int[6];
            gx_objdma = 0;
            gx_primode = 0;
            //gx_objzbuf = (UINT8 *)priority_bitmap->base;
            gx_shdzbuf = new byte[0x2a0000];
            gx_objpool = new GX_OBJ[518];
            //K053247_export_config(&K053247_ram, &K053247_gfx, &K053247_callback, &K053247_dx, &K053247_dy);
            //K054338_export_config(&K054338_shdRGB);
            if (objdma != 0)
            {
                gx_spriteram = new ushort[0x800];
                gx_objdma = 1;
            }
            else
            {
                gx_spriteram = K053247_ram;
            }
            Palette.palette_set_shadow_dRGB32(3, -80, -80, -80, 0);
            K054338_invert_alpha(1);
        }
        public static void konamigx_mixer_primode(int mode)
        {
            gx_primode = mode;
        }
        public static void konamigx_mixer(bitmap_t bitmap, RECT cliprect, int sub1flags, int sub2flags, int mixerflags)
        {
            int[] xoffset = new int[8] { 0, 1, 4, 5, 16, 17, 20, 21 };
            int[] yoffset = new int[8] { 0, 2, 8, 10, 32, 34, 40, 42 };
            int parity = 0;
            int[] objbuf = new int[518];
            int[] shadowon = new int[3], shdpri = new int[3], layerid = new int[6], layerpri = new int[6];
            int wrapsize, xwraplim, ywraplim, cltc_shdpri, prflp, disp;
            int xa, ya, ox, oy, zw, zh, flipx, flipy, mirrorx, mirrory, zoomx, zoomy, scalex, scaley, nozoom;
            int screenwidth, flipscreenx, flipscreeny, offx, offy;
            int nobj, i, j, k, l, temp, temp1, temp2, temp3, temp4, count;
            int order, offs, code, color, zcode, pri = 0, spri, spri_min, shdprisel, shadow, alpha, drawmode;
            int code2, color2, pri2;
            K054338_fill_backcolor(bitmap, konamigx_wrport1_0 & 0x20);
            parity ^= 1;
            disp = K055555_read_register(45);
            if (disp == 0)
            {
                return;
            }
            cltc_shdpri = K054338_read_register(15);
            if ((cltc_shdpri & 0x01) == 0)
            {
                return;
            }
            cltc_shdpri &= 0x04;
            if (Video.screenstate.frame_number == 0x3b2)
            {
                int i1 = 1;
            }
            if ((mixerflags & 0x20000000) != 0)
            {
                mixerflags |= 0x10000000;
            }
            else
            {
                gx_wipezbuf(mixerflags & 0x10000000);
            }
            konamigx_precache_registers();
            flipscreenx = K053246_objset1 & 1;
            flipscreeny = K053246_objset1 & 2;
            offx = (K053246_read_register(0) << 8 | K053246_read_register(1)) & 0x3ff;
            offy = (K053246_read_register(2) << 8 | K053246_read_register(3)) & 0x3ff;
            layerid[0] = 0;
            layerid[1] = 1;
            layerid[2] = 2;
            layerid[3] = 3;
            layerid[4] = 4;
            layerid[5] = 5;
            if ((K053247_opset & 0x40) != 0)
            {
                wrapsize = 512;
                xwraplim = 512 - 64;
                ywraplim = 512 - 128;
            }
            else
            {
                wrapsize = 1024;
                xwraplim = 1024 - 384;
                ywraplim = 1024 - 512;
            }
            prflp = K055555_read_register(1) & 0x04;
            layerpri[0] = K055555_read_register(7);
            layerpri[1] = K055555_read_register(10);
            layerpri[3] = K055555_read_register(14);
            layerpri[4] = K055555_read_register(16);
            layerpri[5] = K055555_read_register(17);
            if (gx_primode == -1)
            {
                layerpri[2] = K055555_read_register(10) + 0x20;
                shdprisel = 0x3f;
            }
            else
            {
                layerpri[2] = K055555_read_register(13);
                shdprisel = K055555_read_register(41);
            }
            if ((shdprisel & 0x03) == 0)
            {
                shadowon[0] = 0;
            }
            if ((shdprisel & 0x0c) == 0)
            {
                shadowon[1] = 0;
            }
            if ((shdprisel & 0x30) == 0)
            {
                shadowon[2] = 0;
            }
            shdpri[0] = K055555_read_register(37);
            shdpri[1] = K055555_read_register(38);
            shdpri[2] = K055555_read_register(39);
            spri_min = 0;
            shadowon[2] = shadowon[1] = shadowon[0] = 0;
            if ((mixerflags & 0x10000000) == 0)
            {
                for (j = 0, i = 0; i < 3; j += 3, i++)
                {
                    k = K054338_shdRGB[j];
                    if (k < -7 || k > 7)
                    {
                        shadowon[i] = 1;
                        continue;
                    }
                    k = K054338_shdRGB[j + 1];
                    if (k < -7 || k > 7)
                    {
                        shadowon[i] = 1;
                        continue;
                    }
                    k = K054338_shdRGB[j + 2];
                    if (k < -7 || k > 7)
                    {
                        shadowon[i] = 1;
                    }
                }
                temp = K055555_read_register(40);
                for (i = 0; i < 4; i++)
                {
                    if ((temp >> i & 1) == 0 && spri_min < layerpri[i])
                    {
                        spri_min = layerpri[i];
                    }
                }
                K054338_update_all_shadows();
            }
            for (j = 0; j < 5; j++)
            {
                temp1 = layerpri[j];
                for (i = j + 1; i < 6; i++)
                {
                    temp2 = layerpri[i];
                    if ((uint)temp1 <= (uint)temp2)
                    {
                        layerpri[i] = temp1;
                        layerpri[j] = temp1 = temp2;
                        temp2 = layerid[i];
                        layerid[i] = layerid[j];
                        layerid[j] = temp2;
                    }
                }
            }
            gx_objptr_offset = 0;
            nobj = 0;
            for (i = 5; i >= 0; i--)
            {
                code = layerid[i];
                switch (code)
                {
                    case 4:
                        offs = -128;
                        if ((sub1flags & 0xf) != 0)
                        {
                            if ((sub1flags & 0x10) != 0)
                            {
                                offs = -4;
                            }
                            /*else if (sub1 != 0)
                            {
                                offs = -2;
                            }*/
                        }
                        break;
                    case 5:
                        offs = -128;
                        if ((sub2flags & 0xf) != 0)
                        {
                            if ((sub2flags & 0x10) != 0)
                            {
                                offs = -5;
                            }
                            /*else if (sub2 != 0)
                            {
                                offs = -3;
                            }*/
                        }
                        break;
                    default:
                        offs = -1;
                        break;
                }
                if (offs != -128)
                {
                    gx_objpool[gx_objptr_offset].order = layerpri[i] << 24;
                    gx_objpool[gx_objptr_offset].code = code;
                    gx_objpool[gx_objptr_offset].offs = offs;
                    gx_objptr_offset++;
                    objbuf[nobj] = nobj;
                    nobj++;
                }
            }
            i = j = 0xff;
            for (offs = 0; offs < 0x800; offs += 8)
            {
                if ((gx_spriteram[offs] & 0x8000) == 0)
                {
                    continue;
                }
                zcode = gx_spriteram[offs] & 0xff;
                if ((K053247_opset & 0x10) != 0)
                {
                    zcode = 0xff - zcode;
                }
                code = gx_spriteram[offs + 1];
                color = k = gx_spriteram[offs + 6];
                l = gx_spriteram[offs + 7];
                K053247_callback(code, color, pri, out code2, out color2, out pri2);
                code = code2;
                color = color2;
                pri = pri2;
                temp4 = temp3 = temp2 = temp1 = spri = shadow = 0;
                if ((color & 0x80000000) != 0)
                {
                    shadow = 3;
                    spri = pri;
                    temp3 = 1;
                    temp4 = 5;
                }
                else
                {
                    shadow = k >> 10 & 3;
                    if (shadow != 0)
                    {
                        if (shadow != 1 || (K053246_objset1 & 0x20) != 0)
                        {
                            shadow--;
                            temp1 = 1;
                            temp2 = 1;
                            if (shadowon[shadow] != 0)
                            {
                                temp3 = 1;
                                temp4 = 4;
                            }
                        }
                        else
                        {
                            shadow = 0;
                            if (shadowon[0] == 0)
                            {
                                continue;
                            }
                            temp3 = 1;
                            temp4 = 5;
                        }
                    }
                    else
                    {
                        temp1 = 1;
                        temp2 = 0;
                    }
                    if (temp1 != 0)
                    {
                        if ((color >> 16 & 3) != 0)
                        {
                            temp2 |= 2;
                        }
                    }
                    if (temp3 != 0)
                    {
                        spri = (K053247_opset & 0x20) != 0 ? pri : shdpri[shadow];
                    }
                }
                switch (gx_primode & 0xf)
                {
                    case 1:
                        zcode = 0;
                        break;
                    case 4:
                        if ((k & 0x3000) != 0 || k == 0x0800)
                        {
                            continue;
                        }
                        break;
                    case 5:
                        if (spri < spri_min) spri = spri_min;
                        break;
                }
                if (temp1 != 0)
                {
                    order = pri << 24 | zcode << 16 | offs << (8 - 3) | temp2 << 4;
                    gx_objpool[gx_objptr_offset].order = order;
                    gx_objpool[gx_objptr_offset].offs = offs;
                    gx_objpool[gx_objptr_offset].code = code;
                    gx_objpool[gx_objptr_offset].color = color;
                    gx_objptr_offset++;
                    objbuf[nobj] = nobj;
                    nobj++;
                }
                if (temp3 != 0 && (color & 0x40000000) == 0 && (mixerflags & 0x10000000) == 0)
                {
                    order = spri << 24 | zcode << 16 | offs << (8 - 3) | temp4 << 4 | shadow;
                    gx_objpool[gx_objptr_offset].order = order;
                    gx_objpool[gx_objptr_offset].offs = offs;
                    gx_objpool[gx_objptr_offset].code = code;
                    gx_objpool[gx_objptr_offset].color = color;
                    gx_objptr_offset++;
                    objbuf[nobj] = nobj;
                    nobj++;
                }
            }
            k = nobj;
            l = nobj - 1;
            for (j = 0; j < l; j++)
            {
                temp1 = objbuf[j];
                temp2 = gx_objpool[temp1].order;
                for (i = j + 1; i < k; i++)
                {
                    temp3 = objbuf[i];
                    temp4 = gx_objpool[temp3].order;
                    if ((uint)temp2 <= (uint)temp4)
                    {
                        temp2 = temp4;
                        objbuf[i] = temp1;
                        objbuf[j] = temp1 = temp3;
                    }
                }
            }
            screenwidth = Video.screenstate.width;
            for (count = 0; count < nobj; count++)
            {
                order = gx_objpool[objbuf[count]].order;
                offs = gx_objpool[objbuf[count]].offs;
                code = gx_objpool[objbuf[count]].code;
                color = gx_objpool[objbuf[count]].color;
                if (offs >= 0)
                {
                    if ((disp & 0x10) == 0)
                    {
                        continue;
                    }
                }
                else
                {
                    i = code << 1;
                    j = mixerflags >> i & 3;
                    k = 0;
                    switch (offs)
                    {
                        case -1:
                            if ((disp & (1 << code)) != 0)
                            {
                                if (j == 1)
                                {
                                    temp1 = 0xff;
                                    temp2 = temp3 = 0;
                                }
                                else
                                    if (j == 3)
                                    {
                                        temp1 = 0x00;
                                        temp2 = mixerflags >> (i + 16);
                                        temp3 = 3;
                                    }
                                    else
                                    {
                                        temp1 = vinmix;
                                        temp2 = vinmix >> i & 3;
                                        temp3 = vmixon >> i & 3;
                                    }
                                if (temp1 != 0xff && temp2 != 0)
                                {
                                    temp4 = K054338_set_alpha_level(temp2);
                                    if (temp4 <= 0)
                                    {
                                        continue;
                                    }
                                    if (temp4 < 255)
                                    {
                                        k = 0x100;
                                    }
                                }
                                if ((mixerflags & 1 << (code + 12)) != 0)
                                {
                                    k |= unchecked((int)0x80000000);
                                }
                                if (Video.screenstate.frame_number==0x3b2&& count == 0x19)
                                {
                                    int i1 = 1;
                                }
                                counta = count;
                                K056832_tilemap_draw(cliprect, code, k, 0);
                            }
                            continue;
                        case -2:
                        case -4:
                            if ((disp & 0x20) != 0)
                            {
                                if (j == 1)
                                {
                                    temp1 = 0xff;
                                    temp2 = temp3 = 0;
                                }
                                else if (j == 3)
                                {
                                    temp1 = 0x00;
                                    temp2 = mixerflags >> 24;
                                    temp3 = 3;
                                }
                                else
                                {
                                    temp1 = osinmix;
                                    temp2 = osinmix >> 2 & 3;
                                    temp3 = osmixon >> 2 & 3;
                                }
                                if (temp1 != 0xff && temp2 != 0)
                                {
                                    temp4 = K054338_set_alpha_level(temp2);
                                    if (temp4 <= 0) continue;
                                    if (temp4 < 255) k = (j == 2) ? ~parity : 1;
                                }
                                l = sub1flags & 0xf;
                                if (offs == -2)
                                {
                                    //K053936GP_0_zoom_draw(bitmap, cliprect, l, k);
                                }
                                else
                                {
                                    //K053250_draw(machine, bitmap, cliprect, 0, vcblk[4] << l, 0, 0);
                                }
                            }
                            continue;
                        case -3:
                        case -5:
                            if ((disp & 0x40) != 0)
                            {
                                if (j == 1) { temp1 = 0xff; temp2 = temp3 = 0; }
                                else
                                    if (j == 3) { temp1 = 0x00; temp2 = mixerflags >> 26; temp3 = 3; }
                                    else
                                    {
                                        temp1 = osinmix;
                                        temp2 = osinmix >> 4 & 3;
                                        temp3 = osmixon >> 4 & 3;
                                    }
                                if (temp1 != 0xff && temp2 != 0)
                                {
                                    temp4 = K054338_set_alpha_level(temp2);
                                    if (temp4 <= 0)
                                    {
                                        continue;
                                    }
                                    if (temp4 < 255)
                                    {
                                        k = (j == 2) ? ~parity : 1;
                                    }
                                }
                                l = sub2flags & 0xf;
                                if (offs == -3)
                                {
                                    //K053936GP_1_zoom_draw(machine, bitmap, cliprect, sub2, l, k);
                                }
                                else
                                {
                                    //K053250_draw(machine, bitmap, cliprect, 1, vcblk[5] << l, 0, 0);
                                }
                            }
                            continue;
                    }
                    continue;
                }
                drawmode = order >> 4 & 0xf;
                alpha = 255;
                if ((drawmode & 2) != 0)
                {
                    alpha = color >> 16 & 3;
                    if (alpha != 0)
                    {
                        alpha = K054338_set_alpha_level(alpha);
                    }
                    if (alpha <= 0)
                    {
                        continue;
                    }
                }
                color &= 0x0000ffff;
                if (drawmode >= 4)
                {
                    Palette.palette_set_shadow_mode(order & 0x0f);
                }
                if ((mixerflags & 0x20000000) == 0)
                {
                    zcode = order >> 16 & 0xff;
                    pri = order >> 24 & 0xff;
                }
                else
                {
                    zcode = -1;
                }
                xa = ya = 0;
                if ((code & 0x01) != 0)
                {
                    xa += 1;
                }
                if ((code & 0x02) != 0)
                {
                    ya += 1;
                }
                if ((code & 0x04) != 0)
                {
                    xa += 2;
                }
                if ((code & 0x08) != 0)
                {
                    ya += 2;
                }
                if ((code & 0x10) != 0)
                {
                    xa += 4;
                }
                if ((code & 0x20) != 0)
                {
                    ya += 4;
                }
                code &= ~0x3f;
                temp4 = gx_spriteram[offs];
                oy = gx_spriteram[offs + 2] & 0x3ff;
                ox = gx_spriteram[offs + 3] & 0x3ff;
                scaley = zoomy = gx_spriteram[offs + 4] & 0x3ff;
                if (zoomy != 0)
                {
                    zoomy = (0x400000 + (zoomy >> 1)) / zoomy;
                }
                else
                {
                    zoomy = 0x800000;
                }
                if ((temp4 & 0x4000) == 0)
                {
                    scalex = zoomx = gx_spriteram[offs + 5] & 0x3ff;
                    if (zoomx != 0)
                    {
                        zoomx = (0x400000 + (zoomx >> 1)) / zoomx;
                    }
                    else
                    {
                        zoomx = 0x800000;
                    }
                }
                else
                {
                    zoomx = zoomy;
                    scalex = scaley;
                }
                nozoom = (scalex == 0x40 && scaley == 0x40) ? 1 : 0;
                flipx = temp4 & 0x1000;
                flipy = temp4 & 0x2000;
                temp = gx_spriteram[offs + 6];
                mirrorx = temp & 0x4000;
                if (mirrorx != 0)
                {
                    flipx = 0;
                }
                mirrory = temp & 0x8000;
                if ((K053246_objset1 & 8) != 0)
                {
                    zoomx = zoomx >> 1;
                    ox = (ox >> 1) + 1;
                    if (flipscreenx != 0)
                    {
                        ox += screenwidth;
                    }
                }
                if (flipscreenx != 0)
                {
                    ox = -ox;
                    if (mirrorx == 0)
                    {
                        flipx = flipx == 0 ? 1 : 0;
                    }
                }
                if (flipscreeny != 0)
                {
                    oy = -oy;
                    if (mirrory == 0)
                    {
                        flipy = flipy == 0 ? 1 : 0;
                    }
                }
                temp = wrapsize - 1;
                ox = (ox - offx) & temp;
                oy = (-oy - offy) & temp;
                if (ox >= xwraplim) ox -= wrapsize;
                if (oy >= ywraplim) oy -= wrapsize;
                ox += K053247_dx;
                oy += K053247_dy;
                temp = temp4 >> 8 & 0x0f;
                k = 1 << (temp & 3);
                l = 1 << (temp >> 2 & 3);
                ox -= (zoomx * k) >> 13;
                oy -= (zoomy * l) >> 13;
                for (j = 0; j < l; j++)
                {
                    temp4 = oy + ((zoomy * j + (1 << 11)) >> 12);
                    zh = (oy + ((zoomy * (j + 1) + (1 << 11)) >> 12)) - temp4;
                    for (i = 0; i < k; i++)
                    {
                        temp3 = ox + ((zoomx * i + (1 << 11)) >> 12);
                        zw = (ox + ((zoomx * (i + 1) + (1 << 11)) >> 12)) - temp3;
                        temp = code;
                        if (mirrorx != 0)
                        {
                            if ((flipx == 0) ^ ((i << 1) < k))
                            {
                                temp += xoffset[(k - 1 - i + xa) & 7];
                                temp1 = 1;
                            }
                            else
                            {
                                temp += xoffset[(i + xa) & 7];
                                temp1 = 0;
                            }
                        }
                        else
                        {
                            if (flipx != 0)
                            {
                                temp += xoffset[(k - 1 - i + xa) & 7];
                            }
                            else
                            {
                                temp += xoffset[(i + xa) & 7];
                            }
                            temp1 = flipx;
                        }
                        if (mirrory != 0)
                        {
                            if ((flipy == 0) ^ ((j << 1) >= l))
                            {
                                temp += yoffset[(l - 1 - j + ya) & 7];
                                temp2 = 1;
                            }
                            else
                            {
                                temp += yoffset[(j + ya) & 7];
                                temp2 = 0;
                            }
                        }
                        else
                        {
                            if (flipy != 0)
                            {
                                temp += yoffset[(l - 1 - j + ya) & 7];
                            }
                            else
                            {
                                temp += yoffset[(j + ya) & 7];
                            }
                            temp2 = flipy;
                        }
                        if (nozoom != 0)
                        {
                            scaley = scalex = 0x10000;
                        }
                        else
                        {
                            scalex = zw << 12;
                            scaley = zh << 12;
                        };
                        zdrawgfxzoom32GP(Palette.bbitmap[Video.curbitmap], K053247_gfx, cliprect, (uint)temp, (uint)color, temp1, temp2, temp3, temp4, scalex, scaley, alpha, drawmode, zcode, pri);
                    }
                }
            }
        }
        public static void konamigx_mixer(bitmap_t bitmap, RECT cliprect, Tmap sub1, int sub1flags, int sub2flags, int mixerflags)
        {
            int[] xoffset = new int[8] { 0, 1, 4, 5, 16, 17, 20, 21 };
            int[] yoffset = new int[8] { 0, 2, 8, 10, 32, 34, 40, 42 };
            int parity = 0;
            int[] objbuf = new int[518];
            int[] shadowon = new int[3], shdpri = new int[3], layerid = new int[6], layerpri = new int[6];
            int wrapsize, xwraplim, ywraplim, cltc_shdpri, prflp, disp;
            int xa, ya, ox, oy, zw, zh, flipx, flipy, mirrorx, mirrory, zoomx, zoomy, scalex, scaley, nozoom;
            int screenwidth, flipscreenx, flipscreeny, offx, offy;
            int nobj, i, j, k, l, temp, temp1, temp2, temp3, temp4, count;
            int order, offs, code, color, zcode, pri = 0, spri, spri_min, shdprisel, shadow, alpha, drawmode;
            int code2, color2, pri2;
            K054338_fill_backcolor(bitmap, konamigx_wrport1_0 & 0x20);
            parity ^= 1;
            disp = K055555_read_register(45);
            if (disp == 0)
            {
                return;
            }
            cltc_shdpri = K054338_read_register(15);
            if ((cltc_shdpri & 0x01) == 0)
            {
                return;
            }
            cltc_shdpri &= 0x04;
            if (Video.screenstate.frame_number == 0x3b2)
            {
                int i1 = 1;
            }
            if ((mixerflags & 0x20000000) != 0)
            {
                mixerflags |= 0x10000000;
            }
            else
            {
                gx_wipezbuf(mixerflags & 0x10000000);
            }
            konamigx_precache_registers();
            flipscreenx = K053246_objset1 & 1;
            flipscreeny = K053246_objset1 & 2;
            offx = (K053246_read_register(0) << 8 | K053246_read_register(1)) & 0x3ff;
            offy = (K053246_read_register(2) << 8 | K053246_read_register(3)) & 0x3ff;
            layerid[0] = 0;
            layerid[1] = 1;
            layerid[2] = 2;
            layerid[3] = 3;
            layerid[4] = 4;
            layerid[5] = 5;
            if ((K053247_opset & 0x40) != 0)
            {
                wrapsize = 512;
                xwraplim = 512 - 64;
                ywraplim = 512 - 128;
            }
            else
            {
                wrapsize = 1024;
                xwraplim = 1024 - 384;
                ywraplim = 1024 - 512;
            }
            prflp = K055555_read_register(1) & 0x04;
            layerpri[0] = K055555_read_register(7);
            layerpri[1] = K055555_read_register(10);
            layerpri[3] = K055555_read_register(14);
            layerpri[4] = K055555_read_register(16);
            layerpri[5] = K055555_read_register(17);
            if (gx_primode == -1)
            {
                layerpri[2] = K055555_read_register(10) + 0x20;
                shdprisel = 0x3f;
            }
            else
            {
                layerpri[2] = K055555_read_register(13);
                shdprisel = K055555_read_register(41);
            }
            if ((shdprisel & 0x03) == 0)
            {
                shadowon[0] = 0;
            }
            if ((shdprisel & 0x0c) == 0)
            {
                shadowon[1] = 0;
            }
            if ((shdprisel & 0x30) == 0)
            {
                shadowon[2] = 0;
            }
            shdpri[0] = K055555_read_register(37);
            shdpri[1] = K055555_read_register(38);
            shdpri[2] = K055555_read_register(39);
            spri_min = 0;
            shadowon[2] = shadowon[1] = shadowon[0] = 0;
            if ((mixerflags & 0x10000000) == 0)
            {
                for (j = 0, i = 0; i < 3; j += 3, i++)
                {
                    k = K054338_shdRGB[j];
                    if (k < -7 || k > 7)
                    {
                        shadowon[i] = 1;
                        continue;
                    }
                    k = K054338_shdRGB[j + 1];
                    if (k < -7 || k > 7)
                    {
                        shadowon[i] = 1;
                        continue;
                    }
                    k = K054338_shdRGB[j + 2];
                    if (k < -7 || k > 7)
                    {
                        shadowon[i] = 1;
                    }
                }
                temp = K055555_read_register(40);
                for (i = 0; i < 4; i++)
                {
                    if ((temp >> i & 1) == 0 && spri_min < layerpri[i])
                    {
                        spri_min = layerpri[i];
                    }
                }
                K054338_update_all_shadows();
            }
            for (j = 0; j < 5; j++)
            {
                temp1 = layerpri[j];
                for (i = j + 1; i < 6; i++)
                {
                    temp2 = layerpri[i];
                    if ((uint)temp1 <= (uint)temp2)
                    {
                        layerpri[i] = temp1;
                        layerpri[j] = temp1 = temp2;
                        temp2 = layerid[i];
                        layerid[i] = layerid[j];
                        layerid[j] = temp2;
                    }
                }
            }
            gx_objptr_offset = 0;
            nobj = 0;
            for (i = 5; i >= 0; i--)
            {
                code = layerid[i];
                switch (code)
                {
                    case 4:
                        offs = -128;
                        if ((sub1flags & 0xf) != 0)
                        {
                            if ((sub1flags & 0x10) != 0)
                            {
                                offs = -4;
                            }
                            else //if (sub1 != null)
                            {
                                offs = -2;
                            }
                        }
                        break;
                    case 5:
                        offs = -128;
                        if ((sub2flags & 0xf) != 0)
                        {
                            if ((sub2flags & 0x10) != 0)
                            {
                                offs = -5;
                            }
                            /*else if (sub2 != 0)
                            {
                                offs = -3;
                            }*/
                        }
                        break;
                    default:
                        offs = -1;
                        break;
                }
                if (offs != -128)
                {
                    gx_objpool[gx_objptr_offset].order = layerpri[i] << 24;
                    gx_objpool[gx_objptr_offset].code = code;
                    gx_objpool[gx_objptr_offset].offs = offs;
                    gx_objptr_offset++;
                    objbuf[nobj] = nobj;
                    nobj++;
                }
            }
            i = j = 0xff;
            for (offs = 0; offs < 0x800; offs += 8)
            {
                if ((gx_spriteram[offs] & 0x8000) == 0)
                {
                    continue;
                }
                zcode = gx_spriteram[offs] & 0xff;
                if ((K053247_opset & 0x10) != 0)
                {
                    zcode = 0xff - zcode;
                }
                code = gx_spriteram[offs + 1];
                color = k = gx_spriteram[offs + 6];
                l = gx_spriteram[offs + 7];
                K053247_callback(code, color, pri, out code2, out color2, out pri2);
                code = code2;
                color = color2;
                pri = pri2;
                temp4 = temp3 = temp2 = temp1 = spri = shadow = 0;
                if ((color & 0x80000000) != 0)
                {
                    shadow = 3;
                    spri = pri;
                    temp3 = 1;
                    temp4 = 5;
                }
                else
                {
                    shadow = k >> 10 & 3;
                    if (shadow != 0)
                    {
                        if (shadow != 1 || (K053246_objset1 & 0x20) != 0)
                        {
                            shadow--;
                            temp1 = 1;
                            temp2 = 1;
                            if (shadowon[shadow] != 0)
                            {
                                temp3 = 1;
                                temp4 = 4;
                            }
                        }
                        else
                        {
                            shadow = 0;
                            if (shadowon[0] == 0)
                            {
                                continue;
                            }
                            temp3 = 1;
                            temp4 = 5;
                        }
                    }
                    else
                    {
                        temp1 = 1;
                        temp2 = 0;
                    }
                    if (temp1 != 0)
                    {
                        if ((color >> 16 & 3) != 0)
                        {
                            temp2 |= 2;
                        }
                    }
                    if (temp3 != 0)
                    {
                        spri = (K053247_opset & 0x20) != 0 ? pri : shdpri[shadow];
                    }
                }
                switch (gx_primode & 0xf)
                {
                    case 1:
                        zcode = 0;
                        break;
                    case 4:
                        if ((k & 0x3000) != 0 || k == 0x0800)
                        {
                            continue;
                        }
                        break;
                    case 5:
                        if (spri < spri_min) spri = spri_min;
                        break;
                }
                if (temp1 != 0)
                {
                    order = pri << 24 | zcode << 16 | offs << (8 - 3) | temp2 << 4;
                    gx_objpool[gx_objptr_offset].order = order;
                    gx_objpool[gx_objptr_offset].offs = offs;
                    gx_objpool[gx_objptr_offset].code = code;
                    gx_objpool[gx_objptr_offset].color = color;
                    gx_objptr_offset++;
                    objbuf[nobj] = nobj;
                    nobj++;
                }
                if (temp3 != 0 && (color & 0x40000000) == 0 && (mixerflags & 0x10000000) == 0)
                {
                    order = spri << 24 | zcode << 16 | offs << (8 - 3) | temp4 << 4 | shadow;
                    gx_objpool[gx_objptr_offset].order = order;
                    gx_objpool[gx_objptr_offset].offs = offs;
                    gx_objpool[gx_objptr_offset].code = code;
                    gx_objpool[gx_objptr_offset].color = color;
                    gx_objptr_offset++;
                    objbuf[nobj] = nobj;
                    nobj++;
                }
            }
            k = nobj;
            l = nobj - 1;
            for (j = 0; j < l; j++)
            {
                temp1 = objbuf[j];
                temp2 = gx_objpool[temp1].order;
                for (i = j + 1; i < k; i++)
                {
                    temp3 = objbuf[i];
                    temp4 = gx_objpool[temp3].order;
                    if ((uint)temp2 <= (uint)temp4)
                    {
                        temp2 = temp4;
                        objbuf[i] = temp1;
                        objbuf[j] = temp1 = temp3;
                    }
                }
            }
            screenwidth = Video.screenstate.width;
            for (count = 0; count < nobj; count++)
            {
                order = gx_objpool[objbuf[count]].order;
                offs = gx_objpool[objbuf[count]].offs;
                code = gx_objpool[objbuf[count]].code;
                color = gx_objpool[objbuf[count]].color;
                if (offs >= 0)
                {
                    if ((disp & 0x10) == 0)
                    {
                        continue;
                    }
                }
                else
                {
                    i = code << 1;
                    j = mixerflags >> i & 3;
                    k = 0;
                    switch (offs)
                    {
                        case -1:
                            if ((disp & (1 << code)) != 0)
                            {
                                if (j == 1)
                                {
                                    temp1 = 0xff;
                                    temp2 = temp3 = 0;
                                }
                                else
                                    if (j == 3)
                                    {
                                        temp1 = 0x00;
                                        temp2 = mixerflags >> (i + 16);
                                        temp3 = 3;
                                    }
                                    else
                                    {
                                        temp1 = vinmix;
                                        temp2 = vinmix >> i & 3;
                                        temp3 = vmixon >> i & 3;
                                    }
                                if (temp1 != 0xff && temp2 != 0)
                                {
                                    temp4 = K054338_set_alpha_level(temp2);
                                    if (temp4 <= 0)
                                    {
                                        continue;
                                    }
                                    if (temp4 < 255)
                                    {
                                        k = 0x100;
                                    }
                                }
                                if ((mixerflags & 1 << (code + 12)) != 0)
                                {
                                    k |= unchecked((int)0x80000000);
                                }
                                counta = count;
                                K056832_tilemap_draw(cliprect, code, k, 0);
                            }
                            continue;
                        case -2:
                        case -4:
                            if ((disp & 0x20) != 0)
                            {
                                if (j == 1)
                                {
                                    temp1 = 0xff;
                                    temp2 = temp3 = 0;
                                }
                                else if (j == 3)
                                {
                                    temp1 = 0x00;
                                    temp2 = mixerflags >> 24;
                                    temp3 = 3;
                                }
                                else
                                {
                                    temp1 = osinmix;
                                    temp2 = osinmix >> 2 & 3;
                                    temp3 = osmixon >> 2 & 3;
                                }
                                if (temp1 != 0xff && temp2 != 0)
                                {
                                    temp4 = K054338_set_alpha_level(temp2);
                                    if (temp4 <= 0) continue;
                                    if (temp4 < 255) k = (j == 2) ? ~parity : 1;
                                }
                                l = sub1flags & 0xf;
                                if (offs == -2)
                                {
                                    K053936GP_0_zoom_draw(bitmap, cliprect, sub1, l, k);
                                }
                                else
                                {
                                    //K053250_draw(machine, bitmap, cliprect, 0, vcblk[4] << l, 0, 0);
                                }
                            }
                            continue;
                        case -3:
                        case -5:
                            if ((disp & 0x40) != 0)
                            {
                                if (j == 1) { temp1 = 0xff; temp2 = temp3 = 0; }
                                else
                                    if (j == 3) { temp1 = 0x00; temp2 = mixerflags >> 26; temp3 = 3; }
                                    else
                                    {
                                        temp1 = osinmix;
                                        temp2 = osinmix >> 4 & 3;
                                        temp3 = osmixon >> 4 & 3;
                                    }
                                if (temp1 != 0xff && temp2 != 0)
                                {
                                    temp4 = K054338_set_alpha_level(temp2);
                                    if (temp4 <= 0)
                                    {
                                        continue;
                                    }
                                    if (temp4 < 255)
                                    {
                                        k = (j == 2) ? ~parity : 1;
                                    }
                                }
                                l = sub2flags & 0xf;
                                if (offs == -3)
                                {
                                    //K053936GP_1_zoom_draw(machine, bitmap, cliprect, sub2, l, k);
                                }
                                else
                                {
                                    //K053250_draw(machine, bitmap, cliprect, 1, vcblk[5] << l, 0, 0);
                                }
                            }
                            continue;
                    }
                    continue;
                }
                drawmode = order >> 4 & 0xf;
                alpha = 255;
                if ((drawmode & 2) != 0)
                {
                    alpha = color >> 16 & 3;
                    if (alpha != 0)
                    {
                        alpha = K054338_set_alpha_level(alpha);
                    }
                    if (alpha <= 0)
                    {
                        continue;
                    }
                }
                color &= 0x0000ffff;
                if (drawmode >= 4)
                {
                    Palette.palette_set_shadow_mode(order & 0x0f);
                }
                if ((mixerflags & 0x20000000) == 0)
                {
                    zcode = order >> 16 & 0xff;
                    pri = order >> 24 & 0xff;
                }
                else
                {
                    zcode = -1;
                }
                xa = ya = 0;
                if ((code & 0x01) != 0)
                {
                    xa += 1;
                }
                if ((code & 0x02) != 0)
                {
                    ya += 1;
                }
                if ((code & 0x04) != 0)
                {
                    xa += 2;
                }
                if ((code & 0x08) != 0)
                {
                    ya += 2;
                }
                if ((code & 0x10) != 0)
                {
                    xa += 4;
                }
                if ((code & 0x20) != 0)
                {
                    ya += 4;
                }
                code &= ~0x3f;
                temp4 = gx_spriteram[offs];
                oy = gx_spriteram[offs + 2] & 0x3ff;
                ox = gx_spriteram[offs + 3] & 0x3ff;
                scaley = zoomy = gx_spriteram[offs + 4] & 0x3ff;
                if (zoomy != 0)
                {
                    zoomy = (0x400000 + (zoomy >> 1)) / zoomy;
                }
                else
                {
                    zoomy = 0x800000;
                }
                if ((temp4 & 0x4000) == 0)
                {
                    scalex = zoomx = gx_spriteram[offs + 5] & 0x3ff;
                    if (zoomx != 0)
                    {
                        zoomx = (0x400000 + (zoomx >> 1)) / zoomx;
                    }
                    else
                    {
                        zoomx = 0x800000;
                    }
                }
                else
                {
                    zoomx = zoomy;
                    scalex = scaley;
                }
                nozoom = (scalex == 0x40 && scaley == 0x40) ? 1 : 0;
                flipx = temp4 & 0x1000;
                flipy = temp4 & 0x2000;
                temp = gx_spriteram[offs + 6];
                mirrorx = temp & 0x4000;
                if (mirrorx != 0)
                {
                    flipx = 0;
                }
                mirrory = temp & 0x8000;
                if ((K053246_objset1 & 8) != 0)
                {
                    zoomx = zoomx >> 1;
                    ox = (ox >> 1) + 1;
                    if (flipscreenx != 0)
                    {
                        ox += screenwidth;
                    }
                }
                if (flipscreenx != 0)
                {
                    ox = -ox;
                    if (mirrorx == 0)
                    {
                        flipx = flipx == 0 ? 1 : 0;
                    }
                }
                if (flipscreeny != 0)
                {
                    oy = -oy;
                    if (mirrory == 0)
                    {
                        flipy = flipy == 0 ? 1 : 0;
                    }
                }
                temp = wrapsize - 1;
                ox = (ox - offx) & temp;
                oy = (-oy - offy) & temp;
                if (ox >= xwraplim) ox -= wrapsize;
                if (oy >= ywraplim) oy -= wrapsize;
                ox += K053247_dx;
                oy += K053247_dy;
                temp = temp4 >> 8 & 0x0f;
                k = 1 << (temp & 3);
                l = 1 << (temp >> 2 & 3);
                ox -= (zoomx * k) >> 13;
                oy -= (zoomy * l) >> 13;
                for (j = 0; j < l; j++)
                {
                    temp4 = oy + ((zoomy * j + (1 << 11)) >> 12);
                    zh = (oy + ((zoomy * (j + 1) + (1 << 11)) >> 12)) - temp4;
                    for (i = 0; i < k; i++)
                    {
                        temp3 = ox + ((zoomx * i + (1 << 11)) >> 12);
                        zw = (ox + ((zoomx * (i + 1) + (1 << 11)) >> 12)) - temp3;
                        temp = code;
                        if (mirrorx != 0)
                        {
                            if ((flipx == 0) ^ ((i << 1) < k))
                            {
                                temp += xoffset[(k - 1 - i + xa) & 7];
                                temp1 = 1;
                            }
                            else
                            {
                                temp += xoffset[(i + xa) & 7];
                                temp1 = 0;
                            }
                        }
                        else
                        {
                            if (flipx != 0)
                            {
                                temp += xoffset[(k - 1 - i + xa) & 7];
                            }
                            else
                            {
                                temp += xoffset[(i + xa) & 7];
                            }
                            temp1 = flipx;
                        }
                        if (mirrory != 0)
                        {
                            if ((flipy == 0) ^ ((j << 1) >= l))
                            {
                                temp += yoffset[(l - 1 - j + ya) & 7];
                                temp2 = 1;
                            }
                            else
                            {
                                temp += yoffset[(j + ya) & 7];
                                temp2 = 0;
                            }
                        }
                        else
                        {
                            if (flipy != 0)
                            {
                                temp += yoffset[(l - 1 - j + ya) & 7];
                            }
                            else
                            {
                                temp += yoffset[(j + ya) & 7];
                            }
                            temp2 = flipy;
                        }
                        if (nozoom != 0)
                        {
                            scaley = scalex = 0x10000;
                        }
                        else
                        {
                            scalex = zw << 12;
                            scaley = zh << 12;
                        };
                        zdrawgfxzoom32GP(Palette.bbitmap[Video.curbitmap], K053247_gfx, cliprect, (uint)temp, (uint)color, temp1, temp2, temp3, temp4, scalex, scaley, alpha, drawmode, zcode, pri);
                    }
                }
            }
        }
    }
}
