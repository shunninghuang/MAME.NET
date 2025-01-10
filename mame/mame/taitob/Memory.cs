using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using cpu.z80;

namespace mame
{
    public partial class Taitob
    {
        public static sbyte MReadOpByte_masterw(int address)
        {
            address &= 0xffffff;
            sbyte result = 0;
            if (address <= 0x07ffff)
            {
                result = (sbyte)(Memory.mainrom[address]);
            }
            else if (address >= 0x200000 && address <= 0x203fff)
            {
                int offset = address - 0x200000;
                result=(sbyte)Memory.mainram[offset];
            }
            return result;
        }
        public static sbyte MReadByte_masterw(int address)
        {
            address &= 0xffffff;
            sbyte result = 0;
            if (address <= 0x07ffff)
            {
                if (address < Memory.mainrom.Length)
                {
                    result = (sbyte)Memory.mainrom[address];
                }
                else
                {
                    result = 0;
                }
            }
            else if(address>=0x200000&&address<=0x203fff)
            {
                int offset = address - 0x200000;
                result = (sbyte)Memory.mainram[offset];
            }
            else if (address >= 0x400000 && address <= 0x40ffff)
            {
                int offset = (address - 0x400000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(TC0180VCU_word_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)TC0180VCU_word_r(offset);
                }
            }
            else if (address >= 0x410000 && address <= 0x41197f)
            {
                int offset = (address - 0x410000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(taitob_spriteram[offset] >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)taitob_spriteram[offset];
                }
            }
            else if (address >= 0x411980 && address <= 0x4137ff)
            {
                result = (sbyte)mainram2[address - 0x411980];
            }
            else if (address >= 0x413800 && address <= 0x413fff)
            {
                int offset = (address - 0x413800) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(taitob_scroll[offset] >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)taitob_scroll[offset];
                }
            }
            else if (address >= 0x418000 && address <= 0x41801f)
            {
                int offset = (address - 0x418000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(taitob_v_control_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)taitob_v_control_r(offset);
                }
            }
            else if (address >= 0x440000 && address <= 0x47ffff)
            {
                int offset = (address - 0x440000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(TC0180VCU_framebuffer_word_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)TC0180VCU_framebuffer_word_r(offset);
                }
            }
            else if (address >= 0x600000 && address <= 0x601fff)
            {
                int offset = (address - 0x600000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(Generic.paletteram16[offset] >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)Generic.paletteram16[offset];
                }
            }
            else if (address >= 0x800000 && address <= 0x800001)
            {
                if (address % 2 == 0)
                {
                    result = (sbyte)(Taito.TC0220IOC_halfword_byteswap_portreg_r() >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)Taito.TC0220IOC_halfword_byteswap_portreg_r();
                }
            }
            else if (address >= 0x800002 && address <= 0x800003)
            {
                if (address % 2 == 0)
                {
                    result = (sbyte)(Taito.TC0220IOC_halfword_byteswap_port_r() >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)Taito.TC0220IOC_halfword_byteswap_port_r();
                }
            }
            else if (address >= 0xa00000 && address <= 0xa00001)
            {
                result = 0;
            }
            else if (address >= 0xa00002 && address <= 0xa00003)
            {
                if (address % 2 == 0)
                {
                    result = (sbyte)(Taitosnd.taitosound_comm16_msb_r() >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)Taitosnd.taitosound_comm16_msb_r();
                }
            }
            return result;
        }
        public static short MReadOpWord_masterw(int address)
        {
            address &= 0xffffff;
            short result = 0;
            if (address <= 0x07ffff)
            {
                if (address + 1 < Memory.mainrom.Length)
                {
                    result = (short)(Memory.mainrom[address] * 0x100 + Memory.mainrom[address + 1]);
                }
                else
                {
                    result = 0;
                }
            }
            else if (address >= 0x200000 && address <= 0x203fff)
            {
                int offset = address - 0x200000;
                result = (short)(Memory.mainram[offset] * 0x100 + Memory.mainram[offset + 1]);
            }
            return result;
        }
        public static short MReadWord_masterw(int address)
        {
            address &= 0xffffff;
            short result = 0;
            if (address <= 0x07ffff)
            {
                if (address + 1 < Memory.mainrom.Length)
                {
                    result = (short)(Memory.mainrom[address] * 0x100 + Memory.mainrom[address + 1]);
                }
                else
                {
                    result = 0;
                }
            }
            else if (address >= 0x200000 && address + 1 <= 0x203fff)
            {
                int offset = address - 0x200000;
                result = (short)(Memory.mainram[offset] * 0x100 + Memory.mainram[offset + 1]);
            }
            else if (address >= 0x400000 && address + 1 <= 0x40ffff)
            {
                int offset = (address - 0x400000) / 2;
                result = (short)TC0180VCU_word_r(offset);
            }
            else if (address >= 0x410000 && address + 1 <= 0x41197f)
            {
                int offset = (address - 0x410000) / 2;
                result = (short)taitob_spriteram[offset];
            }
            else if (address >= 0x411980 && address + 1 <= 0x4137ff)
            {
                int offset = address - 0x410000;
                result = (short)(mainram2[offset] * 0x100 + mainram2[offset + 1]);
            }
            else if (address >= 0x413800 && address <= 0x413fff)
            {
                int offset = (address - 0x413800) / 2;
                result = (short)(taitob_scroll[offset]);
            }
            else if (address >= 0x418000 && address + 1 <= 0x41801f)
            {
                int offset = (address - 0x418000) / 2;
                result = (short)taitob_v_control_r(offset);
            }
            else if (address >= 0x440000 && address + 1 <= 0x47ffff)
            {
                int offset = (address - 0x440000) / 2;
                result = (short)TC0180VCU_framebuffer_word_r(offset);
            }
            else if (address >= 0x600000 && address + 1 <= 0x601fff)
            {
                int offset = (address - 0x600000) / 2;
                result = (short)Generic.paletteram16[offset];
            }
            else if (address >= 0x800000 && address + 1 <= 0x800001)
            {
                result = (short)Taito.TC0220IOC_halfword_byteswap_portreg_r();
            }
            else if (address >= 0x800002 && address + 1 <= 0x800003)
            {
                result = (short)Taito.TC0220IOC_halfword_byteswap_port_r();
            }
            else if (address >= 0xa00000 && address + 1 <= 0xa00001)
            {
                result = 0;
            }
            else if (address >= 0xa00002 && address + 1 <= 0xa00003)
            {
                result = (short)Taitosnd.taitosound_comm16_msb_r();
            }
            return result;
        }
        public static int MReadOpLong_masterw(int address)
        {
            address &= 0xffffff;
            int result = 0;
            if (address <= 0x07ffff)
            {
                if (address + 3 < Memory.mainrom.Length)
                {
                    result = (int)(Memory.mainrom[address] * 0x1000000 + Memory.mainrom[address + 1] * 0x10000 + Memory.mainrom[address + 2] * 0x100 + Memory.mainrom[address + 3]);
                }
                else
                {
                    result = 0;
                }
            }
            else if(address>=0x200000&&address+3<=0x203fff)
            {
                int offset=address-0x200000;
                result = (int)(Memory.mainram[offset] * 0x1000000 + Memory.mainram[offset + 1] * 0x10000 + Memory.mainram[offset + 2] * 0x100 + Memory.mainram[offset + 3]);
            }
            return result;
        }
        public static int MReadLong_masterw(int address)
        {
            address &= 0xffffff;
            int result = 0;
            if (address <= 0x07ffff)
            {
                if (address + 3 < Memory.mainrom.Length)
                {
                    result = (int)(Memory.mainrom[address] * 0x1000000 + Memory.mainrom[address + 1] * 0x10000 + Memory.mainrom[address + 2] * 0x100 + Memory.mainrom[address + 3]);
                }
                else
                {
                    result = 0;
                }
            }
            else if (address >= 0x200000 && address + 3 <= 0x203fff)
            {
                int offset = address - 0x200000;
                result = (int)(Memory.mainram[offset] * 0x1000000 + Memory.mainram[offset + 1] * 0x10000 + Memory.mainram[offset + 2] * 0x100 + Memory.mainram[offset + 3]);
            }
            else if (address >= 0x400000 && address + 3 <= 0x40ffff)
            {
                int offset = (address - 0x400000) / 2;
                result = (int)(TC0180VCU_word_r(offset) * 0x10000 + TC0180VCU_word_r(offset + 1));
            }
            else if (address >= 0x410000 && address + 3 <= 0x41197f)
            {
                int offset = (address - 0x410000) / 2;
                result = (int)(taitob_spriteram[offset] * 0x10000 + taitob_spriteram[offset + 1]);
            }
            else if (address >= 0x411980 && address + 3 <= 0x4137ff)
            {
                int offset = address - 0x411980;
                result = (int)(mainram2[offset] * 0x1000000 + mainram2[offset + 1] * 0x10000 + mainram2[offset + 2] * 0x100 + mainram2[offset + 3]);
            }
            else if (address >= 0x413800 && address + 3 <= 0x413fff)
            {
                int offset = (address - 0x413800) / 2;
                result = (int)(taitob_scroll[offset] * 0x10000 + taitob_scroll[offset + 1]);
            }
            else if (address >= 0x418000 && address + 3 <= 0x41801f)
            {
                int offset = (address - 0x418000) / 2;
                result = (int)(taitob_v_control_r(offset) * 0x10000 + taitob_v_control_r(offset + 1));
            }
            else if (address >= 0x440000 && address + 3 <= 0x47ffff)
            {
                int offset = (address - 0x440000) / 2;
                result = (int)(TC0180VCU_framebuffer_word_r(offset) * 0x10000 + TC0180VCU_framebuffer_word_r(offset + 1));
            }
            else if (address >= 0x600000 && address + 3 <= 0x601fff)
            {
                int offset = (address - 0x600000) / 2;
                result = (int)(Generic.paletteram16[offset] * 0x10000 + Generic.paletteram16[offset + 1]);
            }
            return result;
        }
        public static void MWriteByte_masterw(int address, sbyte value)
        {
            address &= 0xffffff;
            if (address >= 0x000000 && address <= 0x07ffff)
            {
                if (address < Memory.mainrom.Length)
                {
                    Memory.mainrom[address] = (byte)value;
                }
            }
            else if (address >= 0x200000 && address <= 0x203fff)
            {
                int offset = address - 0x200000;
                Memory.mainram[offset] = (byte)value;
            }
            else if (address >= 0x400000 && address <= 0x40ffff)
            {
                int offset = (address - 0x400000) / 2;
                if (address % 2 == 0)
                {
                    TC0180VCU_word_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    TC0180VCU_word_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x410000 && address <= 0x41197f)
            {
                int offset = (address - 0x410000) / 2;
                if (address % 2 == 0)
                {
                    taitob_spriteram[offset] = (ushort)((value << 8) | (taitob_spriteram[offset] & 0xff));
                }
                else if (address % 2 == 1)
                {
                    taitob_spriteram[offset] = (ushort)((taitob_spriteram[offset] & 0xff00) | (byte)value);
                }
            }
            else if (address >= 0x411980 && address <= 0x4137ff)
            {
                int offset = address - 0x411980;
                mainram2[offset] = (byte)value;
            }
            else if (address >= 0x413800 && address <= 0x413fff)
            {
                int offset = (address - 0x413800) / 2;
                if (address % 2 == 0)
                {
                    taitob_scroll[offset] = (ushort)((value << 8) | (taitob_scroll[offset] & 0xff));
                }
                else if (address % 2 == 1)
                {
                    taitob_scroll[offset] = (ushort)((taitob_scroll[offset] & 0xff00) | (byte)value);
                }
            }
            else if (address >= 0x418000 && address <= 0x41801f)
            {
                int offset = (address - 0x418000) / 2;
                if (address % 2 == 0)
                {
                    taitob_v_control_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    taitob_v_control_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x440000 && address <= 0x47ffff)
            {
                int offset = (address - 0x440000) / 2;
                if (address % 2 == 0)
                {
                    TC0180VCU_framebuffer_word_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    TC0180VCU_framebuffer_word_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x600000 && address <= 0x601fff)
            {
                int offset = (address - 0x600000) / 2;
                if (address % 2 == 0)
                {
                    Generic.paletteram16_RRRRGGGGBBBBxxxx_word_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    Generic.paletteram16_RRRRGGGGBBBBxxxx_word_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x800000 && address <= 0x800001)
            {
                if (address % 2 == 0)
                {
                    Taito.TC0220IOC_halfword_byteswap_portreg_w1((byte)value);
                }
            }
            else if (address >= 0x800002 && address <= 0x800003)
            {
                if (address % 2 == 0)
                {
                    Taito.TC0220IOC_halfword_byteswap_port_w1((byte)value);
                }
            }
            else if (address >= 0xa00000 && address <= 0xa00001)
            {
                if (address % 2 == 0)
                {
                    Taitosnd.taitosound_port16_msb_w1((byte)value);
                }
            }
            else if (address >= 0xa00002 && address <= 0xa00003)
            {
                if (address % 2 == 0)
                {
                    Taitosnd.taitosound_comm16_msb_w1((byte)value);
                }
            }
        }
        public static void MWriteWord_masterw(int address, short value)
        {
            address &= 0xffffff;
            if (address >= 0x000000 && address + 1 <= 0x07ffff)
            {
                if (address + 1 < Memory.mainrom.Length)
                {
                    Memory.mainrom[address] = (byte)(value >> 8);
                    Memory.mainrom[address + 1] = (byte)value;
                }
            }
            else if (address >= 0x200000 && address + 1 <= 0x203fff)
            {
                int offset = address - 0x200000;
                Memory.mainram[offset] = (byte)(value >> 8);
                Memory.mainram[offset + 1] = (byte)value;
            }
            else if (address >= 0x400000 && address + 1 <= 0x40ffff)
            {
                int offset = (address - 0x400000) / 2;
                TC0180VCU_word_w(offset, (ushort)value);
            }
            else if (address >= 0x410000 && address + 1 <= 0x41197f)
            {
                int offset = (address - 0x410000) / 2;
                taitob_spriteram[offset] = (ushort)value;
            }
            else if (address >= 0x411980 && address + 1 <= 0x4137ff)
            {
                int offset = address - 0x411980;
                mainram2[offset] = (byte)(value >> 8);
                mainram2[offset + 1] = (byte)value;
            }
            else if (address >= 0x413800 && address + 1 <= 0x413fff)
            {
                int offset = (address - 0x413800) / 2;
                taitob_scroll[offset] = (ushort)value;
            }
            else if (address >= 0x418000 && address + 1 <= 0x41801f)
            {
                int offset = (address - 0x418000) / 2;
                taitob_v_control_w(offset, (ushort)value);
            }
            else if (address >= 0x440000 && address + 1 <= 0x47ffff)
            {
                int offset = (address - 0x440000) / 2;
                TC0180VCU_framebuffer_word_w(offset, (ushort)value);
            }
            else if (address >= 0x600000 && address + 1 <= 0x601fff)
            {
                int offset = (address - 0x600000) / 2;
                Generic.paletteram16_RRRRGGGGBBBBxxxx_word_w(offset, (ushort)value);
            }
            else if (address >= 0x800000 && address + 1 <= 0x800001)
            {
                Taito.TC0220IOC_halfword_byteswap_portreg_w((ushort)value);
            }
            else if (address >= 0x800002 && address + 1 <= 0x800003)
            {
                Taito.TC0220IOC_halfword_byteswap_port_w((ushort)value);
            }
            else if (address >= 0xa00000 && address + 1 <= 0xa00001)
            {
                Taitosnd.taitosound_port16_msb_w((ushort)value);
            }
            else if (address >= 0xa00002 && address + 1 <= 0xa00003)
            {
                Taitosnd.taitosound_comm16_msb_w((ushort)value);
            }
        }
        public static void MWriteLong_masterw(int address, int value)
        {
            MWriteWord_masterw(address, (short)(value >> 16));
            MWriteWord_masterw(address + 2, (short)value);
        }
        public static sbyte MReadOpByte_rastsag2(int address)
        {
            address &= 0xffffff;
            sbyte result = 0;
            if (address <= 0x07ffff)
            {
                result = (sbyte)(Memory.mainrom[address]);
            }
            else if (address >= 0x600000 && address <= 0x607fff)
            {
                int offset = address - 0x600000;
                result=(sbyte)Memory.mainram[offset];
            }
            return result;
        }
        public static sbyte MReadByte_rastsag2(int address)
        {
            address &= 0xffffff;
            sbyte result = 0;
            if (address <= 0x07ffff)
            {
                result = (sbyte)(Memory.mainrom[address]);
            }
            else if (address >= 0x200000 && address <= 0x201fff)
            {
                int offset = (address - 0x200000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(Generic.paletteram16[offset] >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)Generic.paletteram16[offset];
                }
            }
            else if (address >= 0x400000 && address <= 0x40ffff)
            {
                int offset = (address - 0x400000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(TC0180VCU_word_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)TC0180VCU_word_r(offset);
                }
            }
            else if (address >= 0x410000 && address <= 0x41197f)
            {
                int offset = (address - 0x410000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(taitob_spriteram[offset] >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)taitob_spriteram[offset];
                }
            }
            else if (address >= 0x411980 && address <= 0x4137ff)
            {
                result = (sbyte)mainram2[address - 0x411980];
            }
            else if (address >= 0x413800 && address <= 0x413fff)
            {
                int offset = (address - 0x413800) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(taitob_scroll[offset] >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)taitob_scroll[offset];
                }
            }
            else if (address >= 0x418000 && address <= 0x41801f)
            {
                int offset = (address - 0x418000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(taitob_v_control_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)taitob_v_control_r(offset);
                }
            }
            else if (address >= 0x440000 && address <= 0x47ffff)
            {
                int offset = (address - 0x440000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(TC0180VCU_framebuffer_word_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)TC0180VCU_framebuffer_word_r(offset);
                }
            }
            else if (address >= 0x600000 && address <= 0x607fff)
            {
                int offset = address - 0x600000;
                result = (sbyte)Memory.mainram[offset];
            }
            else if (address >= 0x800000 && address <= 0x800001)
            {
                result = 0;
            }
            else if (address >= 0x800002 && address <= 0x800003)
            {
                if (address % 2 == 0)
                {
                    result = (sbyte)(Taitosnd.taitosound_comm16_msb_r() >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)Taitosnd.taitosound_comm16_msb_r();
                }
            }
            else if (address >= 0xa00000 && address <= 0xa0000f)
            {
                int offset = (address - 0xa00000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(Taito.TC0220IOC_halfword_byteswap_r(offset) >> 8);
                }
                else
                {
                    result = (sbyte)Taito.TC0220IOC_halfword_byteswap_r(offset);
                }
            }
            return result;
        }
        public static short MReadOpWord_rastsag2(int address)
        {
            address &= 0xffffff;
            short result = 0;
            if (address <= 0x07ffff)
            {
                if (address + 1 < Memory.mainrom.Length)
                {
                    result = (short)(Memory.mainrom[address] * 0x100 + Memory.mainrom[address + 1]);
                }
                else
                {
                    result = 0;
                }
            }
            else if (address >= 0x600000 && address + 1 <= 0x607fff)
            {
                int offset = address - 0x600000;
                result = (short)(Memory.mainram[offset] * 0x100 + Memory.mainram[offset + 1]);
            }
            return result;
        }
        public static short MReadWord_rastsag2(int address)
        {
            address &= 0xffffff;
            short result = 0;
            if (address <= 0x07ffff)
            {
                if (address + 1 < Memory.mainrom.Length)
                {
                    result = (short)(Memory.mainrom[address] * 0x100 + Memory.mainrom[address + 1]);
                }
                else
                {
                    result = 0;
                }
            }
            else if (address >= 0x200000 && address + 1 <= 0x201fff)
            {
                int offset = (address - 0x200000) / 2;
                result = (short)Generic.paletteram16[offset];
            }
            else if (address >= 0x400000 && address + 1 <= 0x40ffff)
            {
                int offset = (address - 0x400000) / 2;
                result = (short)TC0180VCU_word_r(offset);
            }
            else if (address >= 0x410000 && address + 1 <= 0x41197f)
            {
                int offset = (address - 0x410000) / 2;
                result = (short)taitob_spriteram[offset];
            }
            else if (address >= 0x411980 && address + 1 <= 0x4137ff)
            {
                int offset = address - 0x410000;
                result = (short)(mainram2[offset] * 0x100 + mainram2[offset + 1]);
            }
            else if (address >= 0x413800 && address + 1 <= 0x413fff)
            {
                int offset = (address - 0x413800) / 2;
                result = (short)(taitob_scroll[offset]);
            }
            else if (address >= 0x418000 && address + 1 <= 0x41801f)
            {
                int offset = (address - 0x418000) / 2;
                result = (short)taitob_v_control_r(offset);
            }
            else if (address >= 0x440000 && address + 1 <= 0x47ffff)
            {
                int offset = (address - 0x440000) / 2;
                result = (short)TC0180VCU_framebuffer_word_r(offset);
            }
            else if (address >= 0x600000 && address + 1 <= 0x607fff)
            {
                int offset = address - 0x600000;
                result = (short)(Memory.mainram[offset] * 0x100 + Memory.mainram[offset + 1]);
            }
            else if (address >= 0x800000 && address + 1 <= 0x800001)
            {
                result = 0;
            }
            else if (address >= 0x800002 && address + 1 <= 0x800003)
            {
                result = (short)Taitosnd.taitosound_comm16_msb_r();
            }
            else if (address >= 0xa00000 && address + 1 <= 0xa0000f)
            {
                int offset = (address - 0xa0000) / 2;
                result = (short)Taito.TC0220IOC_halfword_byteswap_r(offset);
            }
            return result;
        }
        public static int MReadOpLong_rastsag2(int address)
        {
            address &= 0xffffff;
            int result = 0;
            if (address <= 0x07ffff)
            {
                if (address + 3 < Memory.mainrom.Length)
                {
                    result = (int)(Memory.mainrom[address] * 0x1000000 + Memory.mainrom[address + 1] * 0x10000 + Memory.mainrom[address + 2] * 0x100 + Memory.mainrom[address + 3]);
                }
                else
                {
                    result = 0;
                }
            }
            else if (address >= 0x600000 && address + 3 <= 0x607fff)
            {
                int offset = address - 0x600000;
                result = (int)(Memory.mainram[offset] * 0x1000000 + Memory.mainram[offset + 1] * 0x10000 + Memory.mainram[offset + 2] * 0x100 + Memory.mainram[offset + 3]);
            }
            return result;
        }
        public static int MReadLong_rastsag2(int address)
        {
            address &= 0xffffff;
            int result = 0;
            if (address <= 0x07ffff)
            {
                if (address + 3 < Memory.mainrom.Length)
                {
                    result = (int)(Memory.mainrom[address] * 0x1000000 + Memory.mainrom[address + 1] * 0x10000 + Memory.mainrom[address + 2] * 0x100 + Memory.mainrom[address + 3]);
                }
                else
                {
                    result = 0;
                }
            }
            else if (address >= 0x200000 && address + 3 <= 0x201fff)
            {
                int offset = (address - 0x200000) / 2;
                result = (int)(Generic.paletteram16[offset] * 0x10000 + Generic.paletteram16[offset + 1]);
            }
            else if (address >= 0x400000 && address + 3 <= 0x40ffff)
            {
                int offset = (address - 0x400000) / 2;
                result = (int)(TC0180VCU_word_r(offset) * 0x10000 + TC0180VCU_word_r(offset + 1));
            }
            else if (address >= 0x410000 && address + 3 <= 0x41197f)
            {
                int offset = (address - 0x410000) / 2;
                result = (int)(taitob_spriteram[offset] * 0x10000 + taitob_spriteram[offset + 1]);
            }
            else if (address >= 0x411980 && address + 3 <= 0x4137ff)
            {
                int offset = address - 0x411980;
                result = (int)(mainram2[offset] * 0x1000000 + mainram2[offset + 1] * 0x10000 + mainram2[offset + 2] * 0x100 + mainram2[offset + 3]);
            }
            else if (address >= 0x413800 && address + 3 <= 0x413fff)
            {
                int offset = (address - 0x413800) / 2;
                result = (int)(taitob_scroll[offset] * 0x10000 + taitob_scroll[offset + 1]);
            }
            else if (address >= 0x418000 && address + 3 <= 0x41801f)
            {
                int offset = (address - 0x418000) / 2;
                result = (int)(taitob_v_control_r(offset) * 0x10000 + taitob_v_control_r(offset + 1));
            }
            else if (address >= 0x440000 && address + 3 <= 0x47ffff)
            {
                int offset = (address - 0x440000) / 2;
                result = (int)(TC0180VCU_framebuffer_word_r(offset) * 0x10000 + TC0180VCU_framebuffer_word_r(offset + 1));
            }
            else if (address >= 0x600000 && address + 3 <= 0x607fff)
            {
                int offset = address - 0x600000;
                result = (int)(Memory.mainram[offset] * 0x1000000 + Memory.mainram[offset + 1] * 0x10000 + Memory.mainram[offset + 2] * 0x100 + Memory.mainram[offset + 3]);
            }
            else if (address >= 0xa00000 && address + 3 <= 0xa0000f)
            {
                int offset = (address - 0xa00000) / 2;
                result = (int)(Taito.TC0220IOC_halfword_byteswap_r(offset) * 0x10000 + Taito.TC0220IOC_halfword_byteswap_r(offset + 1));
            }
            return result;
        }
        public static void MWriteByte_rastsag2(int address, sbyte value)
        {
            address &= 0xffffff;
            if (address >= 0x000000 && address <= 0x07ffff)
            {
                if (address < Memory.mainrom.Length)
                {
                    Memory.mainrom[address] = (byte)value;
                }
            }
            else if (address >= 0x200000 && address <= 0x201fff)
            {
                int offset = (address - 0x200000) / 2;
                if (address % 2 == 0)
                {
                    Generic.paletteram16_RRRRGGGGBBBBxxxx_word_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    Generic.paletteram16_RRRRGGGGBBBBxxxx_word_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x400000 && address <= 0x40ffff)
            {
                int offset = (address - 0x400000) / 2;
                if (address % 2 == 0)
                {
                    TC0180VCU_word_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    TC0180VCU_word_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x410000 && address <= 0x41197f)
            {
                int offset = (address - 0x410000) / 2;
                if (address % 2 == 0)
                {
                    taitob_spriteram[offset] = (ushort)((value << 8) | (taitob_spriteram[offset] & 0xff));
                }
                else if (address % 2 == 1)
                {
                    taitob_spriteram[offset] = (ushort)((taitob_spriteram[offset] & 0xff00) | (byte)value);
                }
            }
            else if (address >= 0x411980 && address <= 0x4137ff)
            {
                int offset = address - 0x411980;
                mainram2[offset] = (byte)value;
            }
            else if (address >= 0x413800 && address <= 0x413fff)
            {
                int offset = (address - 0x413800) / 2;
                if (address % 2 == 0)
                {
                    taitob_scroll[offset] = (ushort)((value << 8) | (taitob_scroll[offset] & 0xff));
                }
                else if (address % 2 == 1)
                {
                    taitob_scroll[offset] = (ushort)((taitob_scroll[offset] & 0xff00) | (byte)value);
                }
            }
            else if (address >= 0x418000 && address <= 0x41801f)
            {
                int offset = (address - 0x418000) / 2;
                if (address % 2 == 0)
                {
                    taitob_v_control_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    taitob_v_control_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x440000 && address <= 0x47ffff)
            {
                int offset = (address - 0x440000) / 2;
                if (address % 2 == 0)
                {
                    TC0180VCU_framebuffer_word_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    TC0180VCU_framebuffer_word_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x600000 && address <= 0x607fff)
            {
                int offset = address - 0x600000;
                Memory.mainram[offset] = (byte)value;
            }
            else if (address >= 0x800000 && address <= 0x800001)
            {
                if (address % 2 == 0)
                {
                    Taitosnd.taitosound_port16_msb_w1((byte)value);
                }
            }
            else if (address >= 0x800002 && address <= 0x800003)
            {
                if (address % 2 == 0)
                {
                    Taitosnd.taitosound_comm16_msb_w1((byte)value);
                }
            }
            else if (address >= 0xa00000 && address <= 0xa0000f)
            {
                int offset = (address - 0xa00000) / 2;
                Taito.TC0220IOC_halfword_byteswap_w1(offset, (byte)value);
            }
        }
        public static void MWriteWord_rastsag2(int address, short value)
        {
            address &= 0xffffff;
            if (address >= 0x000000 && address + 1 <= 0x07ffff)
            {
                if (address + 1 < Memory.mainrom.Length)
                {
                    Memory.mainrom[address] = (byte)(value >> 8);
                    Memory.mainrom[address + 1] = (byte)value;
                }
            }
            else if (address >= 0x200000 && address + 1 <= 0x201fff)
            {
                int offset = (address - 0x200000) / 2;
                Generic.paletteram16_RRRRGGGGBBBBxxxx_word_w(offset, (ushort)value);
            }
            else if (address >= 0x400000 && address + 1 <= 0x40ffff)
            {
                int offset = (address - 0x400000) / 2;
                TC0180VCU_word_w(offset, (ushort)value);
            }
            else if (address >= 0x410000 && address + 1 <= 0x41197f)
            {
                int offset = (address - 0x410000) / 2;
                taitob_spriteram[offset] = (ushort)value;
            }
            else if (address >= 0x411980 && address + 1 <= 0x4137ff)
            {
                int offset = address - 0x411980;
                mainram2[offset] = (byte)(value >> 8);
                mainram2[offset + 1] = (byte)value;
            }
            else if (address >= 0x413800 && address + 1 <= 0x413fff)
            {
                int offset = (address - 0x413800) / 2;
                taitob_scroll[offset] = (ushort)value;
            }
            else if (address >= 0x418000 && address + 1 <= 0x41801f)
            {
                int offset = (address - 0x418000) / 2;
                taitob_v_control_w(offset, (ushort)value);
            }
            else if (address >= 0x440000 && address + 1 <= 0x47ffff)
            {
                int offset = (address - 0x440000) / 2;
                TC0180VCU_framebuffer_word_w(offset, (ushort)value);
            }
            else if (address >= 0x600000 && address + 1 <= 0x607fff)
            {
                int offset = address - 0x600000;
                Memory.mainram[offset] = (byte)(value >> 8);
                Memory.mainram[offset + 1] = (byte)value;
            }
            else if (address >= 0x800000 && address + 1 <= 0x800001)
            {
                Taitosnd.taitosound_port16_msb_w((ushort)value);
            }
            else if (address >= 0x800002 && address + 1 <= 0x800003)
            {
                Taitosnd.taitosound_comm16_msb_w((ushort)value);
            }
            else if (address >= 0xa00000 && address + 1 <= 0xa0000f)
            {
                int offset = (address - 0xa00000) / 2;
                Taito.TC0220IOC_halfword_byteswap_w(offset, (ushort)value);
            }
        }
        public static void MWriteLong_rastsag2(int address, int value)
        {
            address &= 0xffffff;
            if (address >= 0x000000 && address + 3 <= 0x07ffff)
            {
                if (address + 3 < Memory.mainrom.Length)
                {
                    Memory.mainrom[address] = (byte)(value >> 24);
                    Memory.mainrom[address + 1] = (byte)(value >> 16);
                    Memory.mainrom[address + 2] = (byte)(value >> 8);
                    Memory.mainrom[address + 3] = (byte)value;
                }
            }
            else if (address >= 0x200000 && address + 3 <= 0x201fff)
            {
                int offset = (address - 0x200000) / 2;
                Generic.paletteram16_RRRRGGGGBBBBxxxx_word_w(offset, (ushort)(value >> 16));
                Generic.paletteram16_RRRRGGGGBBBBxxxx_word_w(offset + 1, (ushort)value);
            }
            else if (address >= 0x400000 && address + 3 <= 0x40ffff)
            {
                int offset = (address - 0x400000) / 2;
                TC0180VCU_word_w(offset, (ushort)(value >> 16));
                TC0180VCU_word_w(offset + 1, (ushort)value);
            }
            else if (address >= 0x410000 && address + 3 <= 0x41197f)
            {
                int offset = (address - 0x410000) / 2;
                taitob_spriteram[offset] = (ushort)(value >> 16);
                taitob_spriteram[offset + 1] = (ushort)value;
            }
            else if (address >= 0x411980 && address + 3 <= 0x4137ff)
            {
                int offset = address - 0x411980;
                mainram2[offset] = (byte)(value >> 24);
                mainram2[offset + 1] = (byte)(value >> 16);
                mainram2[offset + 2] = (byte)(value >> 8);
                mainram2[offset + 3] = (byte)value;
            }
            else if (address >= 0x413800 && address + 3 <= 0x413fff)
            {
                int offset = (address - 0x413800) / 2;
                taitob_scroll[offset] = (ushort)(value >> 16);
                taitob_scroll[offset + 1] = (ushort)value;
            }
            else if (address >= 0x418000 && address + 3 <= 0x41801f)
            {
                int offset = (address - 0x418000) / 2;
                taitob_v_control_w(offset, (ushort)(value >> 16));
                taitob_v_control_w(offset + 1, (ushort)value);
            }
            else if (address >= 0x440000 && address + 3 <= 0x47ffff)
            {
                int offset = (address - 0x440000) / 2;
                TC0180VCU_framebuffer_word_w(offset, (ushort)(value >> 16));
                TC0180VCU_framebuffer_word_w(offset + 1, (ushort)value);
            }
            else if (address >= 0x600000 && address + 3 <= 0x607fff)
            {
                int offset = address - 0x600000;
                Memory.mainram[offset] = (byte)(value >> 24);
                Memory.mainram[offset + 1] = (byte)(value >> 16);
                Memory.mainram[offset + 2] = (byte)(value >> 8);
                Memory.mainram[offset + 3] = (byte)value;
            }
            else if (address >= 0xa00000 && address + 3 <= 0xa0000f)
            {
                int offset = (address - 0xa00000) / 2;
                Taito.TC0220IOC_halfword_byteswap_w(offset, (ushort)(value >> 16));
                Taito.TC0220IOC_halfword_byteswap_w(offset + 1, (ushort)value);
            }
        }
        public static sbyte MReadOpByte_rambo3(int address)
        {
            address &= 0xffffff;
            sbyte result = 0;
            if (address <= 0x07ffff)
            {
                result = (sbyte)(Memory.mainrom[address]);
            }
            else if (address >= 0x800000 && address <= 0x803fff)
            {
                int offset = address - 0x800000;
                result = (sbyte)Memory.mainram[offset];
            }
            return result;
        }
        public static sbyte MReadByte_rambo3(int address)
        {
            address &= 0xffffff;
            sbyte result = 0;
            if (address <= 0x07ffff)
            {
                result = (sbyte)(Memory.mainrom[address]);
            }
            else if (address >= 0x200000 && address <= 0x200001)
            {
                result = 0;
            }
            else if (address >= 0x200002 && address <= 0x200003)
            {
                if (address % 2 == 0)
                {
                    result = (sbyte)(Taitosnd.taitosound_comm16_msb_r() >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)Taitosnd.taitosound_comm16_msb_r();
                }
            }
            else if (address >= 0x400000 && address <= 0x40ffff)
            {
                int offset = (address - 0x400000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(TC0180VCU_word_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)TC0180VCU_word_r(offset);
                }
            }
            else if (address >= 0x410000 && address <= 0x41197f)
            {
                int offset = (address - 0x410000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(taitob_spriteram[offset] >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)taitob_spriteram[offset];
                }
            }
            else if (address >= 0x411980 && address <= 0x4137ff)
            {
                result = (sbyte)mainram2[address - 0x411980];
            }
            else if (address >= 0x413800 && address <= 0x413fff)
            {
                int offset = (address - 0x413800) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(taitob_scroll[offset] >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)taitob_scroll[offset];
                }
            }
            else if (address >= 0x418000 && address <= 0x41801f)
            {
                int offset = (address - 0x418000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(taitob_v_control_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)taitob_v_control_r(offset);
                }
            }
            else if (address >= 0x440000 && address <= 0x47ffff)
            {
                int offset = (address - 0x440000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(TC0180VCU_framebuffer_word_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)TC0180VCU_framebuffer_word_r(offset);
                }
            }
            else if (address >= 0x600000 && address <= 0x60000f)
            {
                int offset = (address - 0x600000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(Taito.TC0220IOC_halfword_byteswap_r(offset) >> 8);
                }
                else
                {
                    result = (sbyte)Taito.TC0220IOC_halfword_byteswap_r(offset);
                }
            }
            else if (address >= 0x600010 && address <= 0x600011)
            {
                result = (sbyte)tracky1_lo_r();
            }
            else if (address >= 0x600012 && address <= 0x600013)
            {
                result = (sbyte)tracky1_hi_r();
            }
            else if (address >= 0x600014 && address <= 0x600015)
            {
                result = (sbyte)trackx1_lo_r();
            }
            else if (address >= 0x600016 && address <= 0x600017)
            {
                result = (sbyte)trackx1_hi_r();
            }
            else if (address >= 0x600018 && address <= 0x600019)
            {
                result = (sbyte)tracky2_lo_r();
            }
            else if (address >= 0x60001a && address <= 0x60001b)
            {
                result = (sbyte)tracky2_hi_r();
            }
            else if (address >= 0x60001c && address <= 0x60001d)
            {
                result = (sbyte)trackx2_lo_r();
            }
            else if (address >= 0x60001e && address <= 0x60001f)
            {
                result = (sbyte)trackx2_hi_r();
            }
            else if (address >= 0x800000 && address <= 0x803fff)
            {
                int offset = address - 0x800000;
                result = (sbyte)Memory.mainram[offset];
            }
            else if (address >= 0xa00000 && address <= 0xa01fff)
            {
                int offset = (address - 0xa00000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(Generic.paletteram16[offset] >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)Generic.paletteram16[offset];
                }
            }
            return result;
        }
        public static short MReadOpWord_rambo3(int address)
        {
            address &= 0xffffff;
            short result = 0;
            if (address <= 0x07ffff)
            {
                if (address + 1 < Memory.mainrom.Length)
                {
                    result = (short)(Memory.mainrom[address] * 0x100 + Memory.mainrom[address + 1]);
                }
                else
                {
                    result = 0;
                }
            }
            else if (address >= 0x800000 && address + 1 <= 0x803fff)
            {
                int offset = address - 0x800000;
                result = (short)(Memory.mainram[offset] * 0x100 + Memory.mainram[offset + 1]);
            }
            return result;
        }
        public static short MReadWord_rambo3(int address)
        {
            address &= 0xffffff;
            short result = 0;
            if (address <= 0x07ffff)
            {
                if (address + 1 < Memory.mainrom.Length)
                {
                    result = (short)(Memory.mainrom[address] * 0x100 + Memory.mainrom[address + 1]);
                }
                else
                {
                    result = 0;
                }
            }
            else if (address >= 0x200000 && address + 1 <= 0x200001)
            {
                result = 0;
            }
            else if (address >= 0x200002 && address + 1 <= 0x200003)
            {
                result = (short)Taitosnd.taitosound_comm16_msb_r();
            }
            else if (address >= 0x400000 && address + 1 <= 0x40ffff)
            {
                int offset = (address - 0x400000) / 2;
                result = (short)TC0180VCU_word_r(offset);
            }
            else if (address >= 0x410000 && address + 1 <= 0x41197f)
            {
                int offset = (address - 0x410000) / 2;
                result = (short)taitob_spriteram[offset];
            }
            else if (address >= 0x411980 && address + 1 <= 0x4137ff)
            {
                int offset = address - 0x410000;
                result = (short)(mainram2[offset] * 0x100 + mainram2[offset + 1]);
            }
            else if (address >= 0x413800 && address + 1 <= 0x413fff)
            {
                int offset = (address - 0x413800) / 2;
                result = (short)(taitob_scroll[offset]);
            }
            else if (address >= 0x418000 && address + 1 <= 0x41801f)
            {
                int offset = (address - 0x418000) / 2;
                result = (short)taitob_v_control_r(offset);
            }
            else if (address >= 0x440000 && address + 1 <= 0x47ffff)
            {
                int offset = (address - 0x440000) / 2;
                result = (short)TC0180VCU_framebuffer_word_r(offset);
            }
            else if (address >= 0x600000 && address + 1 <= 0x60000f)
            {
                int offset = (address - 0x600000) / 2;
                result = (short)Taito.TC0220IOC_halfword_byteswap_r(offset);
            }
            else if (address >= 0x600010 && address + 1 <= 0x600011)
            {
                result = (sbyte)tracky1_lo_r();
            }
            else if (address >= 0x600012 && address + 1 <= 0x600013)
            {
                result = (sbyte)tracky1_hi_r();
            }
            else if (address >= 0x600014 && address + 1 <= 0x600015)
            {
                result = (sbyte)trackx1_lo_r();
            }
            else if (address >= 0x600016 && address + 1 <= 0x600017)
            {
                result = (sbyte)trackx1_hi_r();
            }
            else if (address >= 0x600018 && address + 1 <= 0x600019)
            {
                result = (sbyte)tracky2_lo_r();
            }
            else if (address >= 0x60001a && address + 1 <= 0x60001b)
            {
                result = (sbyte)tracky2_hi_r();
            }
            else if (address >= 0x60001c && address + 1 <= 0x60001d)
            {
                result = (sbyte)trackx2_lo_r();
            }
            else if (address >= 0x60001e && address + 1 <= 0x60001f)
            {
                result = (sbyte)trackx2_hi_r();
            }
            else if (address >= 0x800000 && address + 1 <= 0x803fff)
            {
                int offset = address - 0x800000;
                result = (short)(Memory.mainram[offset] * 0x100 + Memory.mainram[offset + 1]);
            }
            else if (address >= 0xa00000 && address + 1 <= 0xa01fff)
            {
                int offset = (address - 0xa00000) / 2;
                result = (short)Generic.paletteram16[offset];
            }
            return result;
        }
        public static int MReadOpLong_rambo3(int address)
        {
            int result = 0;
            result = (int)((ushort)MReadOpWord_rambo3(address) * 0x10000 + (ushort)MReadOpWord_rambo3(address + 2));
            return result;
        }
        public static int MReadLong_rambo3(int address)
        {
            int result = 0;
            result = (int)((ushort)MReadWord_rambo3(address) * 0x10000 + (ushort)MReadWord_rambo3(address + 2));
            return result;
        }
        public static void MWriteByte_rambo3(int address, sbyte value)
        {
            address &= 0xffffff;
            if (address >= 0x000000 && address <= 0x07ffff)
            {
                if (address < Memory.mainrom.Length)
                {
                    Memory.mainrom[address] = (byte)value;
                }
            }
            else if (address >= 0x200000 && address <= 0x200001)
            {
                if (address % 2 == 0)
                {
                    Taitosnd.taitosound_port16_msb_w1((byte)value);
                }
            }
            else if (address >= 0x200002 && address <= 0x200003)
            {
                if (address % 2 == 0)
                {
                    Taitosnd.taitosound_comm16_msb_w1((byte)value);
                }
            }
            else if (address >= 0x400000 && address <= 0x40ffff)
            {
                int offset = (address - 0x400000) / 2;
                if (address % 2 == 0)
                {
                    TC0180VCU_word_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    TC0180VCU_word_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x410000 && address <= 0x41197f)
            {
                int offset = (address - 0x410000) / 2;
                if (address % 2 == 0)
                {
                    taitob_spriteram[offset] = (ushort)((value << 8) | (taitob_spriteram[offset] & 0xff));
                }
                else if (address % 2 == 1)
                {
                    taitob_spriteram[offset] = (ushort)((taitob_spriteram[offset] & 0xff00) | (byte)value);
                }
            }
            else if (address >= 0x411980 && address <= 0x4137ff)
            {
                int offset = address - 0x411980;
                mainram2[offset] = (byte)value;
            }
            else if (address >= 0x413800 && address <= 0x413fff)
            {
                int offset = (address - 0x413800) / 2;
                if (address % 2 == 0)
                {
                    taitob_scroll[offset] = (ushort)((value << 8) | (taitob_scroll[offset] & 0xff));
                }
                else if (address % 2 == 1)
                {
                    taitob_scroll[offset] = (ushort)((taitob_scroll[offset] & 0xff00) | (byte)value);
                }
            }
            else if (address >= 0x418000 && address <= 0x41801f)
            {
                int offset = (address - 0x418000) / 2;
                if (address % 2 == 0)
                {
                    taitob_v_control_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    taitob_v_control_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x440000 && address <= 0x47ffff)
            {
                int offset = (address - 0x440000) / 2;
                if (address % 2 == 0)
                {
                    TC0180VCU_framebuffer_word_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    TC0180VCU_framebuffer_word_w2(offset, (byte)value);
                }
            }
            else if(address>=0x600000&&address<=0x60000f)
            {
                int offset = (address - 0x600000) / 2;
                Taito.TC0220IOC_halfword_byteswap_w1(offset, (byte)value);
            }
            else if (address >= 0x800000 && address <= 0x803fff)
            {
                int offset = address - 0x800000;
                Memory.mainram[offset] = (byte)value;
            }
            else if (address >= 0xa00000 && address <= 0xa01fff)
            {
                int offset = (address - 0xa00000) / 2;
                if (address % 2 == 0)
                {
                    Generic.paletteram16_RRRRGGGGBBBBxxxx_word_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    Generic.paletteram16_RRRRGGGGBBBBxxxx_word_w2(offset, (byte)value);
                }
            }
        }
        public static void MWriteWord_rambo3(int address, short value)
        {
            address &= 0xffffff;
            if (address >= 0x000000 && address + 1 <= 0x07ffff)
            {
                if (address + 1 < Memory.mainrom.Length)
                {
                    Memory.mainrom[address] = (byte)(value >> 8);
                    Memory.mainrom[address + 1] = (byte)value;
                }
            }
            else if (address >= 0x200000 && address + 1 <= 0x200001)
            {
                Taitosnd.taitosound_port16_msb_w((ushort)value);
            }
            else if (address >= 0x200002 && address + 1 <= 0x200003)
            {
                Taitosnd.taitosound_comm16_msb_w((ushort)value);
            }
            else if (address >= 0x400000 && address + 1 <= 0x40ffff)
            {
                int offset = (address - 0x400000) / 2;
                TC0180VCU_word_w(offset, (ushort)value);
            }
            else if (address >= 0x410000 && address + 1 <= 0x41197f)
            {
                int offset = (address - 0x410000) / 2;
                taitob_spriteram[offset] = (ushort)value;
            }
            else if (address >= 0x411980 && address + 1 <= 0x4137ff)
            {
                int offset = address - 0x411980;
                mainram2[offset] = (byte)(value >> 8);
                mainram2[offset + 1] = (byte)value;
            }
            else if (address >= 0x413800 && address + 1 <= 0x413fff)
            {
                int offset = (address - 0x413800) / 2;
                taitob_scroll[offset] = (ushort)value;
            }
            else if (address >= 0x418000 && address + 1 <= 0x41801f)
            {
                int offset = (address - 0x418000) / 2;
                taitob_v_control_w(offset, (ushort)value);
            }
            else if (address >= 0x440000 && address + 1 <= 0x47ffff)
            {
                int offset = (address - 0x440000) / 2;
                TC0180VCU_framebuffer_word_w(offset, (ushort)value);
            }
            else if (address >= 0x600000 && address + 1 <= 0x60000f)
            {
                int offset = (address - 0x600000) / 2;
                Taito.TC0220IOC_halfword_byteswap_w(offset, (ushort)value);
            }
            else if (address >= 0x800000 && address + 1 <= 0x803fff)
            {
                int offset = address - 0x800000;
                Memory.mainram[offset] = (byte)(value >> 8);
                Memory.mainram[offset + 1] = (byte)value;
            }
            else if (address >= 0xa00000 && address + 1 <= 0xa01fff)
            {
                int offset = (address - 0xa00000) / 2;
                Generic.paletteram16_RRRRGGGGBBBBxxxx_word_w(offset, (ushort)value);
            }
        }
        public static void MWriteLong_rambo3(int address, int value)
        {
            MWriteWord_rambo3(address, (short)(value >> 16));
            MWriteWord_rambo3(address + 2, (short)value);
        }
        public static sbyte MReadOpByte_crimec(int address)
        {
            address &= 0xffffff;
            sbyte result = 0;
            if (address <= 0x07ffff)
            {
                result = (sbyte)(Memory.mainrom[address]);
            }
            else if (address >= 0xa00000 && address <= 0xa0ffff)
            {
                int offset = address - 0xa00000;
                result = (sbyte)Memory.mainram[offset];
            }
            return result;
        }
        public static sbyte MReadByte_crimec(int address)
        {
            address &= 0xffffff;
            sbyte result = 0;
            if (address <= 0x07ffff)
            {
                result = (sbyte)(Memory.mainrom[address]);
            }
            else if(address>=0x200000&&address<=0x20000f)
            {
                int offset=(address-0x200000)/2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(Taito.TC0220IOC_halfword_byteswap_r(offset) >> 8);
                }
                else
                {
                    result = (sbyte)Taito.TC0220IOC_halfword_byteswap_r(offset);
                }
            }
            else if (address >= 0x400000 && address <= 0x40ffff)
            {
                int offset = (address - 0x400000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(TC0180VCU_word_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)TC0180VCU_word_r(offset);
                }
            }
            else if (address >= 0x410000 && address <= 0x41197f)
            {
                int offset = (address - 0x410000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(taitob_spriteram[offset] >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)taitob_spriteram[offset];
                }
            }
            else if (address >= 0x411980 && address <= 0x4137ff)
            {
                result = (sbyte)mainram2[address - 0x411980];
            }
            else if (address >= 0x413800 && address <= 0x413fff)
            {
                int offset = (address - 0x413800) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(taitob_scroll[offset] >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)taitob_scroll[offset];
                }
            }
            else if (address >= 0x418000 && address <= 0x41801f)
            {
                int offset = (address - 0x418000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(taitob_v_control_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)taitob_v_control_r(offset);
                }
            }
            else if (address >= 0x440000 && address <= 0x47ffff)
            {
                int offset = (address - 0x440000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(TC0180VCU_framebuffer_word_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)TC0180VCU_framebuffer_word_r(offset);
                }
            }
            else if (address >= 0x600000 && address <= 0x600001)
            {
                result = 0;
            }
            else if (address >= 0x600002 && address <= 0x600003)
            {
                if (address % 2 == 0)
                {
                    result = (sbyte)(Taitosnd.taitosound_comm16_msb_r() >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)Taitosnd.taitosound_comm16_msb_r();
                }
            }
            else if (address >= 0x800000 && address <= 0x801fff)
            {
                int offset = (address - 0x800000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(Generic.paletteram16[offset] >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)Generic.paletteram16[offset];
                }
            }
            else if (address >= 0xa00000 && address <= 0xa0ffff)
            {
                int offset = address - 0xa00000;
                result = (sbyte)Memory.mainram[offset];
            }
            return result;
        }
        public static short MReadOpWord_crimec(int address)
        {
            address &= 0xffffff;
            short result = 0;
            if (address <= 0x07ffff)
            {
                if (address + 1 < Memory.mainrom.Length)
                {
                    result = (short)(Memory.mainrom[address] * 0x100 + Memory.mainrom[address + 1]);
                }
                else
                {
                    result = 0;
                }
            }
            else if (address >= 0xa00000 && address <= 0xa0ffff)
            {
                int offset = address - 0xa00000;
                result = (short)(Memory.mainram[offset] * 0x100 + Memory.mainram[offset + 1]);
            }
            return result;
        }
        public static short MReadWord_crimec(int address)
        {
            address &= 0xffffff;
            short result = 0;
            if (address <= 0x07ffff)
            {
                if (address + 1 < Memory.mainrom.Length)
                {
                    result = (short)(Memory.mainrom[address] * 0x100 + Memory.mainrom[address + 1]);
                }
                else
                {
                    result = 0;
                }
            }
            else if (address >= 0x200000 && address + 1 <= 0x20000f)
            {
                int offset = (address - 0x200000) / 2;
                result = (short)Taito.TC0220IOC_halfword_byteswap_r(offset);
            }
            else if (address >= 0x400000 && address + 1 <= 0x40ffff)
            {
                int offset = (address - 0x400000) / 2;
                result = (short)TC0180VCU_word_r(offset);
            }
            else if (address >= 0x410000 && address + 1 <= 0x41197f)
            {
                int offset = (address - 0x410000) / 2;
                result = (short)taitob_spriteram[offset];
            }
            else if (address >= 0x411980 && address + 1 <= 0x4137ff)
            {
                int offset = address - 0x410000;
                result = (short)(mainram2[offset] * 0x100 + mainram2[offset + 1]);
            }
            else if (address >= 0x413800 && address + 1 <= 0x413fff)
            {
                int offset = (address - 0x413800) / 2;
                result = (short)(taitob_scroll[offset]);
            }
            else if (address >= 0x418000 && address + 1 <= 0x41801f)
            {
                int offset = (address - 0x418000) / 2;
                result = (short)taitob_v_control_r(offset);
            }
            else if (address >= 0x440000 && address + 1 <= 0x47ffff)
            {
                int offset = (address - 0x440000) / 2;
                result = (short)TC0180VCU_framebuffer_word_r(offset);
            }
            else if (address >= 0x600000 && address + 1 <= 0x600001)
            {
                result = 0;
            }
            else if (address >= 0x600002 && address + 1 <= 0x600003)
            {
                result = (short)Taitosnd.taitosound_comm16_msb_r();
            }
            else if (address >= 0x800000 && address + 1 <= 0x801fff)
            {
                int offset = (address - 0x800000) / 2;
                result = (short)Generic.paletteram16[offset];
            }
            else if (address >= 0xa00000 && address <= 0xa0ffff)
            {
                int offset = address - 0xa00000;
                result = (short)(Memory.mainram[offset] * 0x100 + Memory.mainram[offset + 1]);
            }
            return result;
        }
        public static int MReadOpLong_crimec(int address)
        {
            int result = 0;
            result = (int)((ushort)MReadOpWord_crimec(address) * 0x10000 + (ushort)MReadOpWord_crimec(address + 2));
            return result;
        }
        public static int MReadLong_crimec(int address)
        {
            int result = 0;
            result = (int)((ushort)MReadWord_crimec(address) * 0x10000 + (ushort)MReadWord_crimec(address + 2));
            return result;
        }
        public static void MWriteByte_crimec(int address, sbyte value)
        {
            address &= 0xffffff;
            if (address >= 0x000000 && address <= 0x07ffff)
            {
                if (address < Memory.mainrom.Length)
                {
                    Memory.mainrom[address] = (byte)value;
                }
            }
            else if (address >= 0x200000 && address <= 0x20000f)
            {
                int offset = (address - 0x200000) / 2;
                Taito.TC0220IOC_halfword_byteswap_w1(offset, (byte)value);
            }
            else if (address >= 0x400000 && address <= 0x40ffff)
            {
                int offset = (address - 0x400000) / 2;
                if (address % 2 == 0)
                {
                    TC0180VCU_word_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    TC0180VCU_word_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x410000 && address <= 0x41197f)
            {
                int offset = (address - 0x410000) / 2;
                if (address % 2 == 0)
                {
                    taitob_spriteram[offset] = (ushort)((value << 8) | (taitob_spriteram[offset] & 0xff));
                }
                else if (address % 2 == 1)
                {
                    taitob_spriteram[offset] = (ushort)((taitob_spriteram[offset] & 0xff00) | (byte)value);
                }
            }
            else if (address >= 0x411980 && address <= 0x4137ff)
            {
                int offset = address - 0x411980;
                mainram2[offset] = (byte)value;
            }
            else if (address >= 0x413800 && address <= 0x413fff)
            {
                int offset = (address - 0x413800) / 2;
                if (address % 2 == 0)
                {
                    taitob_scroll[offset] = (ushort)((value << 8) | (taitob_scroll[offset] & 0xff));
                }
                else if (address % 2 == 1)
                {
                    taitob_scroll[offset] = (ushort)((taitob_scroll[offset] & 0xff00) | (byte)value);
                }
            }
            else if (address >= 0x418000 && address <= 0x41801f)
            {
                int offset = (address - 0x418000) / 2;
                if (address % 2 == 0)
                {
                    taitob_v_control_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    taitob_v_control_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x440000 && address <= 0x47ffff)
            {
                int offset = (address - 0x440000) / 2;
                if (address % 2 == 0)
                {
                    TC0180VCU_framebuffer_word_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    TC0180VCU_framebuffer_word_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x600000 && address <= 0x600001)
            {
                if (address % 2 == 0)
                {
                    Taitosnd.taitosound_port16_msb_w1((byte)value);
                }
            }
            else if (address >= 0x600002 && address <= 0x600003)
            {
                if (address % 2 == 0)
                {
                    Taitosnd.taitosound_comm16_msb_w1((byte)value);
                }
            }
            else if (address >= 0x800000 && address <= 0x801fff)
            {
                int offset = (address - 0x800000) / 2;
                if (address % 2 == 0)
                {
                    Generic.paletteram16_RRRRGGGGBBBBxxxx_word_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    Generic.paletteram16_RRRRGGGGBBBBxxxx_word_w2(offset, (byte)value);
                }
            }
            else if (address >= 0xa00000 && address <= 0xa0ffff)
            {
                int offset = address - 0xa00000;
                Memory.mainram[offset] = (byte)value;
            }
        }
        public static void MWriteWord_crimec(int address, short value)
        {
            address &= 0xffffff;
            if (address >= 0x000000 && address + 1 <= 0x07ffff)
            {
                if (address + 1 < Memory.mainrom.Length)
                {
                    Memory.mainrom[address] = (byte)(value >> 8);
                    Memory.mainrom[address + 1] = (byte)value;
                }
            }
            else if (address >= 0x200000 && address + 1 <= 0x20000f)
            {
                int offset = (address - 0x200000) / 2;
                Taito.TC0220IOC_halfword_byteswap_w(offset, (ushort)value);
            }
            else if (address >= 0x400000 && address + 1 <= 0x40ffff)
            {
                int offset = (address - 0x400000) / 2;
                TC0180VCU_word_w(offset, (ushort)value);
            }
            else if (address >= 0x410000 && address + 1 <= 0x41197f)
            {
                int offset = (address - 0x410000) / 2;
                taitob_spriteram[offset] = (ushort)value;
            }
            else if (address >= 0x411980 && address + 1 <= 0x4137ff)
            {
                int offset = address - 0x411980;
                mainram2[offset] = (byte)(value >> 8);
                mainram2[offset + 1] = (byte)value;
            }
            else if (address >= 0x413800 && address + 1 <= 0x413fff)
            {
                int offset = (address - 0x413800) / 2;
                taitob_scroll[offset] = (ushort)value;
            }
            else if (address >= 0x418000 && address + 1 <= 0x41801f)
            {
                int offset = (address - 0x418000) / 2;
                taitob_v_control_w(offset, (ushort)value);
            }
            else if (address >= 0x440000 && address + 1 <= 0x47ffff)
            {
                int offset = (address - 0x440000) / 2;
                TC0180VCU_framebuffer_word_w(offset, (ushort)value);
            }
            else if (address >= 0x600000 && address + 1 <= 0x600001)
            {
                Taitosnd.taitosound_port16_msb_w((ushort)value);
            }
            else if (address >= 0x600002 && address + 1 <= 0x600003)
            {
                Taitosnd.taitosound_comm16_msb_w((ushort)value);
            }
            else if (address >= 0x800000 && address + 1 <= 0x801fff)
            {
                int offset = (address - 0x800000) / 2;
                Generic.paletteram16_RRRRGGGGBBBBxxxx_word_w(offset, (ushort)value);
            }
            else if (address >= 0xa00000 && address + 1 <= 0xa0ffff)
            {
                int offset = address - 0xa00000;
                Memory.mainram[offset] = (byte)(value >> 8);
                Memory.mainram[offset + 1] = (byte)value;
            }
        }
        public static void MWriteLong_crimec(int address,int value)
        {
            MWriteWord_crimec(address, (short)(value >> 16));
            MWriteWord_crimec(address + 2, (short)value);
        }
        public static sbyte MReadOpByte_pbobble(int address)
        {
            address &= 0xffffff;
            sbyte result = 0;
            if (address <= 0x07ffff)
            {
                result = (sbyte)(Memory.mainrom[address]);
            }
            else if (address >= 0x800000 && address <= 0x801fff)
            {
                int offset = (address - 0x800000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(Generic.paletteram16[offset] >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)Generic.paletteram16[offset];
                }
            }
            else if (address >= 0x900000 && address <= 0x90ffff)
            {
                int offset=address-0x900000;
                result = (sbyte)Memory.mainram[offset];
            }
            return result;
        }
        public static sbyte MReadByte_pbobble(int address)
        {
            address &= 0xffffff;
            sbyte result = 0;
            if (address <= 0x07ffff)
            {
                if (address < Memory.mainrom.Length)
                {
                    result = (sbyte)Memory.mainrom[address];
                }
                else
                {
                    result = 0;
                }
            }
            else if (address >= 0x400000 && address <= 0x40ffff)
            {
                int offset = (address - 0x400000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(TC0180VCU_word_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)TC0180VCU_word_r(offset);
                }
            }
            else if (address >= 0x410000 && address <= 0x41197f)
            {
                int offset = (address - 0x410000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(taitob_spriteram[offset] >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)taitob_spriteram[offset];
                }
            }
            else if (address >= 0x411980 && address <= 0x4137ff)
            {
                result = (sbyte)mainram2[address - 0x411980];
            }
            else if (address >= 0x413800 && address <= 0x413fff)
            {
                int offset = (address - 0x413800) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(taitob_scroll[offset] >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)taitob_scroll[offset];
                }
            }
            else if (address >= 0x418000 && address <= 0x41801f)
            {
                int offset = (address - 0x418000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(taitob_v_control_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)taitob_v_control_r(offset);
                }
            }
            else if (address >= 0x440000 && address <= 0x47ffff)
            {
                int offset = (address - 0x440000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(TC0180VCU_framebuffer_word_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)TC0180VCU_framebuffer_word_r(offset);
                }
            }
            else if (address >= 0x500000 && address <= 0x50000f)
            {
                int offset = (address - 0x500000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(pbobble_input_bypass_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = 0;
                }
            }
            else if (address >= 0x500024 && address <= 0x500025)
            {
                if (address % 2 == 0)
                {
                    result = 0;
                }
                else if (address % 2 == 1)
                {
                    result = Taito.sbyte3;
                }
            }
            else if (address >= 0x500026 && address <= 0x500027)
            {
                if (address % 2 == 0)
                {
                    result = (sbyte)(eep_latch_r() >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)eep_latch_r();
                }
            }
            else if (address >= 0x50002e && address <= 0x50002f)
            {
                if (address % 2 == 0)
                {
                    result = 0;
                }
                else if (address % 2 == 1)
                {
                    result = Taito.sbyte4;
                }
            }
            else if (address >= 0x700000 && address <= 0x700001)
            {
                result = 0;//NOP
            }
            else if (address >= 0x700002 && address <= 0x700003)
            {
                if (address % 2 == 0)
                {
                    result = (sbyte)(Taitosnd.taitosound_comm16_msb_r() >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)Taitosnd.taitosound_comm16_msb_r();
                }
            }
            else if (address >= 0x800000 && address <= 0x801fff)
            {
                int offset = (address - 0x800000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(Generic.paletteram16[offset] >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)Generic.paletteram16[offset];
                }
            }
            else if (address >= 0x900000 && address <= 0x90ffff)
            {
                result = (sbyte)Memory.mainram[address - 0x900000];
            }
            return result;
        }
        public static short MReadOpWord_pbobble(int address)
        {
            address &= 0xffffff;
            short result = 0;
            if (address <= 0x07ffff)
            {
                if (address + 1 < Memory.mainrom.Length)
                {
                    result = (short)(Memory.mainrom[address] * 0x100 + Memory.mainrom[address + 1]);
                }
                else
                {
                    result = 0;
                }
            }
            else if (address >= 0x800000 && address + 1 <= 0x801fff)
            {
                int offset = (address - 0x800000) / 2;
                result = (short)Generic.paletteram16[offset];
            }
            else if (address >= 0x900000 && address + 1 <= 0x90ffff)
            {
                int offset = address - 0x900000;
                result = (short)(Memory.mainram[offset] * 0x100 + Memory.mainram[offset + 1]);
            }
            return result;
        }
        public static short MReadWord_pbobble(int address)
        {
            address &= 0xffffff;
            short result = 0;
            if (address <= 0x07ffff)
            {
                if (address + 1 < Memory.mainrom.Length)
                {
                    result = (short)(Memory.mainrom[address] * 0x100 + Memory.mainrom[address + 1]);
                }
                else
                {
                    result = 0;
                }
            }
            else if (address >= 0x400000 && address + 1 <= 0x40ffff)
            {
                int offset = (address - 0x400000) / 2;
                result = (short)TC0180VCU_word_r(offset);
            }
            else if (address >= 0x410000 && address + 1 <= 0x41197f)
            {
                int offset = (address - 0x410000) / 2;
                result = (short)taitob_spriteram[offset];
            }
            else if (address >= 0x411980 && address + 1 <= 0x4137ff)
            {
                int offset = address - 0x410000;
                result = (short)(mainram2[offset] * 0x100 + mainram2[offset + 1]);
            }
            else if (address >= 0x413800 && address + 1 <= 0x413fff)
            {
                int offset = (address - 0x413800) / 2;
                result = (short)(taitob_scroll[offset]);
            }
            else if (address >= 0x418000 && address + 1 <= 0x41801f)
            {
                int offset = (address - 0x418000) / 2;
                result = (short)taitob_v_control_r(offset);
            }
            else if (address >= 0x440000 && address + 1 <= 0x47ffff)
            {
                int offset = (address - 0x440000) / 2;
                result = (short)TC0180VCU_framebuffer_word_r(offset);
            }
            else if (address >= 0x500000 && address + 1 <= 0x50000f)
            {
                int offset = (address - 0x500000) / 2;
                result = (short)pbobble_input_bypass_r(offset);
            }
            else if (address >= 0x500024 && address + 1 <= 0x500025)
            {
                result = (short)Taito.sbyte3;
            }
            else if (address >= 0x500026 && address + 1 <= 0x500027)
            {
                result = (short)eep_latch_r();
            }
            else if (address >= 0x50002e && address + 1 <= 0x50002f)
            {
                result = (short)Taito.sbyte4;
            }
            else if (address >= 0x700000 && address + 1 <= 0x700001)
            {
                result = 0;//NOP
            }
            else if (address >= 0x700002 && address + 1 <= 0x700003)
            {
                result = (short)Taitosnd.taitosound_comm16_msb_r();
            }
            else if (address >= 0x800000 && address + 1 <= 0x801fff)
            {
                int offset = (address - 0x800000) / 2;
                result = (short)Generic.paletteram16[offset];
            }
            else if (address >= 0x900000 && address + 1 <= 0x90ffff)
            {
                int offset=address-0x900000;
                result = (short)(Memory.mainram[offset] * 0x100 + Memory.mainram[offset + 1]);
            }
            return result;
        }
        public static int MReadOpLong_pbobble(int address)
        {
            address &= 0xffffff;
            int result = 0;
            if (address <= 0x07ffff)
            {
                if (address + 3 < Memory.mainrom.Length)
                {
                    result = (int)(Memory.mainrom[address] * 0x1000000 + Memory.mainrom[address + 1] * 0x10000 + Memory.mainrom[address + 2] * 0x100 + Memory.mainrom[address + 3]);
                }
                else
                {
                    result = 0;
                }
            }
            else if (address >= 0x800000 && address + 3 <= 0x801fff)
            {
                int offset = (address - 0x800000) / 2;
                result = (int)(Generic.paletteram16[offset] * 0x10000 + Generic.paletteram16[offset + 1]);
            }
            else if (address >= 0x900000 && address + 3 <= 0x90ffff)
            {
                int offset = address - 0x900000;
                result = (int)(Memory.mainram[offset] * 0x1000000 + Memory.mainram[offset + 1] * 0x10000 + Memory.mainram[offset + 2] * 0x100 + Memory.mainram[offset + 3]);
            }
            return result;
        }
        public static int MReadLong_pbobble(int address)
        {
            address &= 0xffffff;
            int result = 0;
            if (address <= 0x07ffff)
            {
                if (address + 3 < Memory.mainrom.Length)
                {
                    result = (int)(Memory.mainrom[address] * 0x1000000 + Memory.mainrom[address + 1] * 0x10000 + Memory.mainrom[address + 2] * 0x100 + Memory.mainrom[address + 3]);
                }
                else
                {
                    result = 0;
                }
            }
            else if (address >= 0x400000 && address + 3 <= 0x40ffff)
            {
                int offset = (address - 0x400000) / 2;
                result = (int)(TC0180VCU_word_r(offset) * 0x10000 + TC0180VCU_word_r(offset + 1));
            }
            else if (address >= 0x410000 && address + 3 <= 0x41197f)
            {
                int offset=(address-0x410000)/2;
                result=(int)(taitob_spriteram[offset]*0x10000+taitob_spriteram[offset+1]);
            }
            else if (address >= 0x411980 && address + 3 <= 0x4137ff)
            {
                int offset=address-0x411980;
                result = (int)(mainram2[offset] * 0x1000000 + mainram2[offset + 1] * 0x10000 + mainram2[offset + 2] * 0x100 + mainram2[offset + 3]);
            }
            else if (address >= 0x413800 && address + 3 <= 0x413fff)
            {
                int offset = (address - 0x413800) / 2;
                result = (int)(taitob_scroll[offset] * 0x10000 + taitob_scroll[offset + 1]);
            }
            else if (address >= 0x418000 && address + 3 <= 0x41801f)
            {
                int offset = (address - 0x418000) / 2;
                result = (int)(taitob_v_control_r(offset) * 0x10000 + taitob_v_control_r(offset + 1));
            }
            else if (address >= 0x440000 && address + 3 <= 0x47ffff)
            {
                int offset = (address - 0x440000) / 2;
                result = (int)(TC0180VCU_framebuffer_word_r(offset) * 0x10000 + TC0180VCU_framebuffer_word_r(offset + 1));
            }
            else if (address >= 0x500000 && address + 3 <= 0x50000f)
            {
                int offset = (address - 0x500000) / 2;
                result = (int)(pbobble_input_bypass_r(offset) * 0x10000 + pbobble_input_bypass_r(offset + 1));
            }
            else if (address >= 0x800000 && address + 3 <= 0x801fff)
            {
                int offset = (address - 0x800000) / 2;
                result = (int)(Generic.paletteram16[offset] * 0x10000 + Generic.paletteram16[offset + 1]);
            }
            else if (address >= 0x900000 && address + 3 <= 0x90ffff)
            {
                int offset = address - 0x900000;
                result = (int)(Memory.mainram[offset] * 0x1000000 + Memory.mainram[offset + 1] * 0x10000 + Memory.mainram[offset + 2] * 0x100 + Memory.mainram[offset + 3]);
            }
            return result;
        }
        
        public static void MWriteByte_pbobble(int address, sbyte value)
        {
            address &= 0xffffff;
            if (address >= 0x000000 && address <= 0x07ffff)
            {
                if (address < Memory.mainrom.Length)
                {
                    Memory.mainrom[address] = (byte)value;
                }
            }
            else if (address >= 0x400000 && address <= 0x40ffff)
            {
                int offset = (address - 0x400000)/2;
                if (address % 2 == 0)
                {
                    TC0180VCU_word_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    TC0180VCU_word_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x410000 && address <= 0x41197f)
            {
                int offset = (address - 0x410000) / 2;
                if (address % 2 == 0)
                {
                    taitob_spriteram[offset] = (ushort)((value << 8) | (taitob_spriteram[offset] & 0xff));
                }
                else if (address % 2 == 1)
                {
                    taitob_spriteram[offset] = (ushort)((taitob_spriteram[offset] & 0xff00) | (byte)value);
                }
            }
            else if (address >= 0x411980 && address <= 0x4137ff)
            {
                int offset = address - 0x411980;
                mainram2[offset] = (byte)value;
            }
            else if (address >= 0x413800 && address <= 0x413fff)
            {
                int offset = (address - 0x413800) / 2;
                if (address % 2 == 0)
                {
                    taitob_scroll[offset] = (ushort)((value << 8) | (taitob_scroll[offset] & 0xff));
                }
                else if (address % 2 == 1)
                {
                    taitob_scroll[offset] = (ushort)((taitob_scroll[offset] & 0xff00) | (byte)value);
                }
            }
            else if (address >= 0x418000 && address <= 0x41801f)
            {
                int offset = (address - 0x418000) / 2;
                if (address % 2 == 0)
                {
                    taitob_v_control_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    taitob_v_control_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x440000 && address <= 0x47ffff)
            {
                int offset = (address - 0x440000) / 2;
                if (address % 2 == 0)
                {
                    TC0180VCU_framebuffer_word_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    TC0180VCU_framebuffer_word_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x500000 && address <= 0x50000f)
            {
                int offset = (address - 0x500000) / 2;
                Taito.TC0640FIO_halfword_byteswap_w1(offset, (byte)value);
            }
            else if (address >= 0x500026 && address <= 0x500027)
            {
                if (address % 2 == 0)
                {
                    eeprom_w1((byte)value);
                }
                else if (address % 2 == 1)
                {
                    eeprom_w2((byte)value);
                }
            }
            else if (address >= 0x500028 && address <= 0x500029)
            {
                player_34_coin_ctrl_w((ushort)value);
            }
            else if (address >= 0x600000 && address <= 0x600003)
            {
                int offset = (address - 0x600000) / 2;
                if (address % 2 == 0)
                {
                    gain_control_w1(offset, (byte)value);
                }
            }
            else if (address >= 0x700000 && address <= 0x700001)
            {                
                if (address % 2 == 0)
                {
                    Taitosnd.taitosound_port16_msb_w1((byte)value);
                }
            }
            else if (address >= 0x700002 && address <= 0x700003)
            {
                if (address % 2 == 0)
                {
                    Taitosnd.taitosound_comm16_msb_w1((byte)value);
                }
            }
            else if (address >= 0x800000 && address <= 0x801fff)
            {
                int offset = (address - 0x800000) / 2;
                if (address % 2 == 0)
                {
                    Generic.paletteram16_RRRRGGGGBBBBRGBx_word_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    Generic.paletteram16_RRRRGGGGBBBBRGBx_word_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x900000 && address <= 0x90ffff)
            {
                int offset = address - 0x900000;
                Memory.mainram[offset] = (byte)value;
            }
        }
        public static void MWriteWord_pbobble(int address, short value)
        {
            address &= 0xffffff;
            if (address >= 0x000000 && address + 1 <= 0x07ffff)
            {
                if (address + 1 < Memory.mainrom.Length)
                {
                    Memory.mainrom[address] = (byte)(value >> 8);
                    Memory.mainrom[address + 1] = (byte)value;
                }
            }
            else if (address >= 0x400000 && address + 1 <= 0x40ffff)
            {
                int offset = (address - 0x400000) / 2;
                TC0180VCU_word_w(offset, (ushort)value);
            }
            else if (address >= 0x410000 && address + 1 <= 0x41197f)
            {
                int offset = (address - 0x410000) / 2;
                taitob_spriteram[offset] = (ushort)value;
            }
            else if (address >= 0x411980 && address + 1 <= 0x4137ff)
            {
                int offset = address - 0x411980;
                mainram2[offset] = (byte)(value >> 8);
                mainram2[offset + 1] = (byte)value;
            }
            else if (address >= 0x413800 && address + 1 <= 0x413fff)
            {
                int offset = (address - 0x413800) / 2;
                taitob_scroll[offset] = (ushort)value;
            }
            else if (address >= 0x418000 && address + 1 <= 0x41801f)
            {
                int offset = (address - 0x418000) / 2;
                taitob_v_control_w(offset, (ushort)value);
            }
            else if (address >= 0x440000 && address + 1 <= 0x47ffff)
            {
                int offset = (address - 0x440000) / 2;
                TC0180VCU_framebuffer_word_w(offset, (ushort)value);
            }
            else if (address >= 0x500000 && address + 1 <= 0x50000f)
            {
                int offset = (address - 0x500000) / 2;
                Taito.TC0640FIO_halfword_byteswap_w(offset, (ushort)value);
            }
            else if (address >= 0x500026 && address + 1 <= 0x500027)
            {
                eeprom_w((ushort)value);
            }
            else if (address >= 0x500028 && address + 1 <= 0x500029)
            {
                player_34_coin_ctrl_w((ushort)value);
            }
            else if (address >= 0x600000 && address + 1 <= 0x600003)
            {
                int offset = (address - 0x600000) / 2;
                gain_control_w(offset, (ushort)value);
            }
            else if (address >= 0x700000 && address + 1 <= 0x700001)
            {
                Taitosnd.taitosound_port16_msb_w((ushort)value);
            }
            else if (address >= 0x700002 && address + 1 <= 0x700003)
            {
                Taitosnd.taitosound_comm16_msb_w((ushort)value);
            }
            else if (address >= 0x800000 && address + 1 <= 0x801fff)
            {
                int offset = (address - 0x800000) / 2;
                Generic.paletteram16_RRRRGGGGBBBBRGBx_word_w(offset, (ushort)value);
            }
            else if (address >= 0x900000 && address + 1 <= 0x90ffff)
            {
                int offset = address - 0x900000;
                Memory.mainram[offset] = (byte)(value >> 8);
                Memory.mainram[offset + 1] = (byte)value;
            }
        }
        public static void MWriteLong_pbobble(int address, int value)
        {
            address &= 0xffffff;
            if (address >= 0x000000 && address + 3 <= 0x07ffff)
            {
                if (address + 3 < Memory.mainrom.Length)
                {
                    Memory.mainrom[address] = (byte)(value >> 24);
                    Memory.mainrom[address + 1] = (byte)(value>>16);
                    Memory.mainrom[address + 2] = (byte)(value >> 8);
                    Memory.mainrom[address + 3] = (byte)value;
                }
            }
            else if (address >= 0x400000 && address + 3 <= 0x40ffff)
            {
                int offset = (address - 0x400000) / 2;
                TC0180VCU_word_w(offset, (ushort)(value>>16));
                TC0180VCU_word_w(offset + 1, (ushort)value);
            }
            else if (address >= 0x410000 && address + 3 <= 0x41197f)
            {
                int offset = (address - 0x410000) / 2;
                taitob_spriteram[offset] = (ushort)(value >> 16);
                taitob_spriteram[offset + 1] = (ushort)value;
            }
            else if (address >= 0x411980 && address + 3 <= 0x4137ff)
            {
                int offset = address - 0x411980;
                mainram2[offset] = (byte)(value >> 24);
                mainram2[offset + 1] = (byte)(value>>16);
                mainram2[offset + 2] = (byte)(value >> 8);
                mainram2[offset + 3] = (byte)value;
            }
            else if (address >= 0x413800 && address + 3 <= 0x413fff)
            {
                int offset = (address - 0x413800) / 2;
                taitob_scroll[offset] = (ushort)(value>>16);
                taitob_scroll[offset + 1] = (ushort)value;
            }
            else if (address >= 0x418000 && address + 3 <= 0x41801f)
            {
                int offset = (address - 0x418000) / 2;
                taitob_v_control_w(offset, (ushort)(value>>16));
                taitob_v_control_w(offset + 1, (ushort)value);
            }
            else if (address >= 0x440000 && address + 3 <= 0x47ffff)
            {
                int offset = (address - 0x440000) / 2;
                TC0180VCU_framebuffer_word_w(offset, (ushort)(value>>16));
                TC0180VCU_framebuffer_word_w(offset + 1, (ushort)value);
            }
            else if (address >= 0x500000 && address + 3 <= 0x50000f)
            {
                int offset = (address - 0x500000) / 2;
                Taito.TC0640FIO_halfword_byteswap_w(offset, (ushort)(value >> 16));
                Taito.TC0640FIO_halfword_byteswap_w(offset + 1, (ushort)value);
            }
            else if (address >= 0x600000 && address + 3 <= 0x600003)
            {
                int offset = (address - 0x600000) / 2;
                gain_control_w(offset, (ushort)(value>>16));
                gain_control_w(offset + 1, (ushort)value);
            }
            else if (address >= 0x800000 && address + 3 <= 0x801fff)
            {
                int offset = (address - 0x800000) / 2;
                Generic.paletteram16_RRRRGGGGBBBBRGBx_word_w(offset, (ushort)(value>>16));
                Generic.paletteram16_RRRRGGGGBBBBRGBx_word_w(offset + 1, (ushort)value);
            }
            else if (address >= 0x900000 && address + 3 <= 0x90ffff)
            {
                int offset = address - 0x900000;
                Memory.mainram[offset] = (byte)(value >> 24);
                Memory.mainram[offset + 1] = (byte)(value>>16);
                Memory.mainram[offset + 2] = (byte)(value >> 8);
                Memory.mainram[offset + 3] = (byte)value;
            }
        }
        public static byte ZReadOp_masterw(ushort address)
        {
            byte result = 0;
            if (address <= 0x3fff)
            {
                result = Memory.audiorom[address & 0x7fff];
            }
            else if (address >= 0x4000 && address <= 0x7fff)
            {
                int offset = address - 0x4000;
                result = Memory.audiorom[basebanksnd + offset];
            }
            else
            {
                result = 0;
            }
            return result;
        }
        public static byte ZReadMemory_masterw(ushort address)
        {
            byte result = 0;
            if (address <= 0x3fff)
            {
                result = Memory.audiorom[address & 0x7fff];
            }
            else if (address >= 0x4000 && address <= 0x7fff)
            {
                int offset = address - 0x4000;
                result = Memory.audiorom[basebanksnd + offset];
            }
            else if (address >= 0x8000 && address <= 0x8fff)
            {
                int offset = address - 0x8000;
                result = Memory.audioram[offset];
            }
            else if (address == 0x9000)
            {
                result = YM2203.ym2203_status_port_0_r();
            }
            else if (address == 0xa001)
            {
                result = Taitosnd.taitosound_slave_comm_r();
            }
            return result;
        }
        public static void ZWriteMemory_masterw(ushort address, byte value)
        {
            if (address <= 0x7fff)
            {
                Memory.audiorom[address] = value;
            }
            else if (address >= 0x8000 && address <= 0x8fff)
            {
                int offset = address - 0x8000;
                Memory.audioram[offset] = value;
            }
            else if (address == 0x9000)
            {
                YM2203.ym2203_control_port_0_w(value);
            }
            else if (address == 0x9001)
            {
                YM2203.ym2203_write_port_0_w(value);
            }
            else if (address == 0xa000)
            {
                Taitosnd.taitosound_slave_port_w(value);
            }
            else if (address == 0xa001)
            {
                Taitosnd.taitosound_slave_comm_w(value);
            }
        }
        public static byte ZReadOp_pbobble(ushort address)
        {
            byte result = 0;
            if (address <= 0x3fff)
            {
                result = Memory.audiorom[address & 0x7fff];
            }
            else
            {
                result = 0;
            }
            return result;
        }
        public static byte ZReadMemory_pbobble(ushort address)
        {
            byte result = 0;
            if (address <= 0x3fff)
            {
                result = Memory.audiorom[address & 0x7fff];
            }
            else if (address >= 0x4000 && address <= 0x7fff)
            {
                result = Memory.audiorom[basebanksnd + (address & 0x3fff)];
            }
            else if (address >= 0xc000 && address <= 0xdfff)
            {
                result = Memory.audioram[address - 0xc000];
            }
            else if (address >= 0xe000 && address <= 0xe000)
            {
                result = YM2610.F2610.ym2610_read(0);
            }
            else if (address >= 0xe001 && address <= 0xe001)
            {
                result = YM2610.F2610.ym2610_read(1);
            }
            else if (address >= 0xe002 && address <= 0xe002)
            {
                result = YM2610.F2610.ym2610_read(2);
            }
            else if (address >= 0xe200 && address <= 0xe200)
            {

            }
            else if (address >= 0xe201 && address <= 0xe201)
            {
                result = Taitosnd.taitosound_slave_comm_r();
            }
            else if (address >= 0xea00 && address <= 0xea00)
            {

            }
            return result;
        }
        public static void ZWriteMemory_pbobble(ushort address, byte value)
        {
            if (address <= 0x7fff)
            {
                Memory.audiorom[address] = value;
            }
            else if (address >= 0xc000 && address <= 0xdfff)
            {
                Memory.audioram[address - 0xc000] = value;
            }
            else if (address >= 0xe000 && address <= 0xe000)
            {
                YM2610.F2610.ym2610_write(0, value);
            }
            else if (address >= 0xe001 && address <= 0xe001)
            {
                YM2610.F2610.ym2610_write(1, value);
            }
            else if (address >= 0xe002 && address <= 0xe002)
            {
                YM2610.F2610.ym2610_write(2, value);
            }
            else if (address >= 0xe003 && address <= 0xe003)
            {
                YM2610.F2610.ym2610_write(3, value);
            }
            else if (address >= 0xe200 && address <= 0xe200)
            {
                Taitosnd.taitosound_slave_port_w(value);
            }
            else if (address >= 0xe201 && address <= 0xe201)
            {
                Taitosnd.taitosound_slave_comm_w(value);
            }
            else if (address >= 0xe400 && address <= 0xe403)
            {

            }
            else if (address >= 0xe600 && address <= 0xe600)
            {

            }
            else if (address >= 0xee00 && address <= 0xee00)
            {

            }
            else if (address >= 0xf000 && address <= 0xf000)
            {

            }
            else if (address >= 0xf200 && address <= 0xf200)
            {
                bankswitch_w(0, value);
            }
        }
        public static byte ZReadHardware(ushort address)
        {
            byte result = 0;
            return result;
        }
        public static void ZWriteHardware(ushort address, byte value)
        {
            
        }
    }
}
