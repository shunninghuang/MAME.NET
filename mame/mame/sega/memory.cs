using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mame
{
    public partial class sega
    {
        public static sbyte sbyte1, sbyte2, sbyte3, sbyte4, sbyte34 = -1, sbytes;
        public static sbyte sbyte1_old, sbyte2_old,sbyte3_old,sbyte4_old, sbytes_old;
        public static sbyte MReadOpByte_5874d(int address)
        {
            address &= 0xffffff;
            sbyte result = 0;
            if (address >= regs[0x11] * 0x10000 && address <= regs[0x11] * 0x10000 + 0x7ffff)
            {
                int offset = (address - regs[0x11] * 0x10000)/2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(fd1094_userregion[offset] >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)fd1094_userregion[offset];
                }
            }
            else if (address >= regs[0x13] * 0x10000 && address <= regs[0x13] * 0x10000 + 0x7ffff)
            {
                int offset = (address - regs[0x13] * 0x10000 + 0x80000)/2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(fd1094_userregion[offset] >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)fd1094_userregion[offset];
                }
            }
            return result;
        }
        public static sbyte MReadOpByte_5874(int address)
        {
            address &= 0xffffff;
            sbyte result = 0;
            if (address >= regs[0x11] * 0x10000 && address <= regs[0x11] * 0x10000 + 0x7ffff)
            {
                int offset = address - regs[0x11] * 0x10000;
                result = (sbyte)Memory.mainrom[offset];
            }
            else if (address >= regs[0x13] * 0x10000 && address <= regs[0x13] * 0x10000 + 0x7ffff)
            {
                int offset = address - regs[0x13] * 0x10000 + 0x80000;
                result = (sbyte)Memory.mainrom[offset];
            }
            return result;
        }
        public static sbyte MReadByte_5874(int address)
        {
            address &= 0xffffff;
            sbyte result = 0;
            if (address >= regs[0x11] * 0x10000 && address <= regs[0x11] * 0x10000 + 0x7ffff)
            {
                int offset = address - regs[0x11] * 0x10000;
                result = (sbyte)Memory.mainrom[offset];
            }
            else if (address >= regs[0x13] * 0x10000 && address <= regs[0x13] * 0x10000 + 0x7ffff)
            {
                int offset = address - regs[0x13] * 0x10000 + 0x80000;
                result = (sbyte)Memory.mainrom[offset];
            }
            else if (address >= regs[0x15] * 0x10000 && address <= regs[0x15] * 0x10000 + 0x10)
            {
                int offset = (address - regs[0x15] * 0x10000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(genesis_vdp_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)genesis_vdp_r(offset);
                }
            }
            else if (address >= regs[0x17] * 0x10000 && address <= regs[0x17] * 0x10000 + 0xffff)
            {
                int offset = (address - regs[0x17] * 0x10000) & 0x3fff;
                result = (sbyte)Memory.mainram[offset];
            }
            else if (address >= regs[0x19] * 0x10000 && address <= regs[0x19] * 0x10000 + 0x7ff)
            {
                int offset = (address - regs[0x19] * 0x10000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(segaic16_spriteram_0[offset] >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)segaic16_spriteram_0[offset];
                }
            }
            else if (address >= regs[0x1b] * 0x10000 + 0x10000 && address <= regs[0x1b] * 0x10000 + 0x10fff)
            {
                int offset = (address - regs[0x1b] * 0x10000 - 0x10000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(segaic16_textram_0[offset] >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)segaic16_textram_0[offset];
                }
            }
            else if (address >= regs[0x1b] * 0x10000 + 0x0000 && address <= regs[0x1b] * 0x10000 + 0xffff)
            {
                int offset = (address - regs[0x1b] * 0x10000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(segaic16_tileram_0[offset] >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)segaic16_tileram_0[offset];
                }
            }
            else if (address >= regs[0x1d] * 0x10000 + 0x0000 && address <= regs[0x1d] * 0x10000 + 0x1fff)
            {
                int offset = (address - regs[0x1d] * 0x10000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(Generic.paletteram16[offset] >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)Generic.paletteram16[offset];
                }
            }
            else if (address >= regs[0x1f] * 0x10000 + 0x0000 && address <= regs[0x1f] * 0x10000 + 0x3fff)
            {
                int offset = (address - regs[0x1f] * 0x10000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(misc_io_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)misc_io_r(offset);
                }
            }
            else
            {
                int offset = address / 2;
                result = 0;
            }
            return result;
        }
        public static short MReadOpWord_5874d(int address)
        {
            address &= 0xffffff;
            short result = 0;
            if (address >= regs[0x11] * 0x10000 && address + 1 <= regs[0x11] * 0x10000 + 0x7ffff)
            {
                int offset = (address - regs[0x11] * 0x10000)/2;
                result = (short)fd1094_userregion[offset];
            }
            else if (address >= regs[0x13] * 0x10000 && address + 1 <= regs[0x13] * 0x10000 + 0x7ffff)
            {
                int offset = (address - regs[0x13] * 0x10000 + 0x80000)/2;
                result = (short)fd1094_userregion[offset];
            }
            return result;
        }
        public static short MReadOpWord_5874(int address)
        {
            address &= 0xffffff;
            short result = 0;
            if (address >= regs[0x11] * 0x10000 && address + 1 <= regs[0x11] * 0x10000 + 0x7ffff)
            {
                int offset = address - regs[0x11] * 0x10000;
                result = (short)(Memory.mainrom[offset] * 0x100 + Memory.mainrom[offset + 1]);
            }
            else if (address >= regs[0x13] * 0x10000 && address + 1 <= regs[0x13] * 0x10000 + 0x7ffff)
            {
                int offset = address - regs[0x13] * 0x10000 + 0x80000;
                result = (short)(Memory.mainrom[offset] * 0x100 + Memory.mainrom[offset + 1]);
            }
            return result;
        }
        public static short MReadWord_5874(int address)
        {
            address &= 0xffffff;
            short result = 0;
            if (address >= regs[0x11] * 0x10000 && address + 1 <= regs[0x11] * 0x10000 + 0x7ffff)
            {
                int offset = address - regs[0x11] * 0x10000;
                result = (short)(Memory.mainrom[offset] * 0x100 + Memory.mainrom[offset + 1]);
            }
            else if (address >= regs[0x13] * 0x10000 && address + 1 <= regs[0x13] * 0x10000 + 0x7ffff)
            {
                int offset = address - regs[0x13] * 0x10000 + 0x80000;
                result = (short)(Memory.mainrom[offset] * 0x100 + Memory.mainrom[offset + 1]);
            }
            else if (address >= regs[0x15] * 0x10000 && address + 1 <= regs[0x15] * 0x10000 + 0x10)
            {
                int offset = (address - regs[0x15] * 0x10000) / 2;
                result = (short)genesis_vdp_r(offset);
            }
            else if (address >= regs[0x17] * 0x10000 && address <= regs[0x17] * 0x10000 + 0xffff)
            {
                int offset = (address - regs[0x17] * 0x10000) & 0x3fff;
                result = (short)(Memory.mainram[offset] * 0x100 + Memory.mainram[offset + 1]);
            }
            else if (address >= regs[0x19] * 0x10000 && address <= regs[0x19] * 0x10000 + 0x7ff)
            {
                int offset = (address - regs[0x19] * 0x10000) / 2;
                result = (short)segaic16_spriteram_0[offset];
            }
            else if (address >= regs[0x1b] * 0x10000 + 0x0000 && address <= regs[0x1b] * 0x10000 + 0xffff)
            {
                int offset = (address - regs[0x1b] * 0x10000) / 2;
                result = (short)segaic16_tileram_0[offset];
            }
            else if (address >= regs[0x1b] * 0x10000 + 0x10000 && address <= regs[0x1b] * 0x10000 + 0x10fff)
            {
                int offset = (address - regs[0x1b] * 0x10000 - 0x10000) / 2;
                result = (short)segaic16_textram_0[offset];
            }
            else if (address >= regs[0x1d] * 0x10000 + 0x0000 && address <= regs[0x1d] * 0x10000 + 0x1fff)
            {
                int offset = (address - regs[0x1d] * 0x10000) / 2;
                result = (short)Generic.paletteram16[offset];
            }
            else if (address >= regs[0x1f] * 0x10000 + 0x0000 && address <= regs[0x1f] * 0x10000 + 0x3fff)
            {
                int offset = (address - regs[0x1f] * 0x10000) / 2;
                result = (short)misc_io_r(offset);
            }
            else
            {
                int offset = address / 2;
                result = 0;
            }
            return result;
        }
        public static int MReadOpLong_5874d(int address)
        {
            int result = 0;
            result = (int)((ushort)MReadOpWord_5874d(address) * 0x10000 + (ushort)MReadOpWord_5874d(address + 2));
            return result;
        }
        public static int MReadOpLong_5874(int address)
        {
            int result = 0;
            result = (int)((ushort)MReadOpWord_5874(address) * 0x10000 + (ushort)MReadOpWord_5874(address + 2));
            return result;
        }
        public static int MReadLong_5874(int address)
        {
            int result = 0;
            result = (int)((ushort)MReadWord_5874(address) * 0x10000 + (ushort)MReadWord_5874(address + 2));
            return result;
        }
        public static void MWriteByte_5874(int address, sbyte value)
        {
            address &= 0xffffff;
            if (address >= regs[0x15] * 0x10000 && address <= regs[0x15] * 0x10000 + 0x10)
            {
                int offset = (address - regs[0x15] * 0x10000) / 2;
                if (address % 2 == 0)
                {
                    genesis_vdp_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    genesis_vdp_w2(offset, (byte)value);
                }
            }
            else if (address >= regs[0x17] * 0x10000 && address <= regs[0x17] * 0x10000 + 0xffff)
            {
                int offset = (address - regs[0x17] * 0x10000) & 0x3fff;
                Memory.mainram[offset] = (byte)value;
            }
            else if (address >= regs[0x19] * 0x10000 && address <= regs[0x19] * 0x10000 + 0x7ff)
            {
                int offset = (address - regs[0x19] * 0x10000) / 2;
                if (address % 2 == 0)
                {
                    segaic16_spriteram_0[offset] = (ushort)(((byte)value << 8) | (segaic16_spriteram_0[offset] & 0xff));
                }
                else if (address % 2 == 1)
                {
                    segaic16_spriteram_0[offset] = (ushort)((segaic16_spriteram_0[offset] & 0xff00) | (byte)value);
                }
            }
            else if (address >= regs[0x1b] * 0x10000 + 0x0000 && address <= regs[0x1b] * 0x10000 + 0xffff)
            {
                int offset = (address - regs[0x1b] * 0x10000) / 2;
                if (address % 2 == 0)
                {
                    segaic16_tileram_0_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    segaic16_tileram_0_w2(offset, (byte)value);
                }
            }
            else if (address >= regs[0x1b] * 0x10000 + 0x10000 && address <= regs[0x1b] * 0x10000 + 0x10fff)
            {
                int offset = (address - regs[0x1b] * 0x10000 - 0x10000) / 2;
                if (address % 2 == 0)
                {
                    segaic16_textram_0_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    segaic16_textram_0_w2(offset, (byte)value);
                }
            }
            else if (address >= regs[0x1d] * 0x10000 + 0x0000 && address <= regs[0x1d] * 0x10000 + 0x1fff)
            {
                int offset = (address - regs[0x1d] * 0x10000) / 2;
                if (address % 2 == 0)
                {
                    segaic16_paletteram_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    segaic16_paletteram_w2(offset, (byte)value);
                }
            }
            else if (address >= regs[0x1f] * 0x10000 + 0x0000 && address <= regs[0x1f] * 0x10000 + 0x3fff)
            {
                int offset = (address - regs[0x1f] * 0x10000) / 2;
                if (address % 2 == 0)
                {
                    misc_io_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    misc_io_w2(offset, (byte)value);
                }
            }
            else
            {
                int offset = address / 2;
                if (address % 2 == 0)
                {

                }
                else if (address % 2 == 1)
                {
                    memory_mapper_w(offset, (byte)value);
                }
            }
        }
        public static void MWriteWord_5874(int address, short value)
        {
            address &= 0xffffff;
            if (address >= regs[0x15] * 0x10000 && address + 1 <= regs[0x15] * 0x10000 + 0x10)
            {
                int offset = (address - regs[0x15] * 0x10000) / 2;
                genesis_vdp_w(offset, (ushort)value);
            }
            else if (address >= regs[0x17] * 0x10000 && address + 1 <= regs[0x17] * 0x10000 + 0xffff)
            {
                int offset = (address - regs[0x17] * 0x10000) & 0x3fff;
                Memory.mainram[offset] = (byte)(value >> 8);
                Memory.mainram[offset + 1] = (byte)value;
            }
            else if (address >= regs[0x19] * 0x10000 && address + 1 <= regs[0x19] * 0x10000 + 0x7ff)
            {
                int offset = (address - regs[0x19] * 0x10000) / 2;
                segaic16_spriteram_0[offset] = (ushort)value;
            }
            else if (address >= regs[0x1b] * 0x10000 + 0x0000 && address + 1 <= regs[0x1b] * 0x10000 + 0xffff)
            {
                int offset = (address - regs[0x1b] * 0x10000) / 2;
                segaic16_tileram_0_w(offset, (ushort)value);
            }
            else if (address >= regs[0x1b] * 0x10000 + 0x10000 && address + 1 <= regs[0x1b] * 0x10000 + 0x10fff)
            {
                int offset = (address - regs[0x1b] * 0x10000 - 0x10000) / 2;
                segaic16_textram_0_w(offset, (ushort)value);
            }
            else if (address >= regs[0x1d] * 0x10000 + 0x0000 && address + 1 <= regs[0x1d] * 0x10000 + 0x1fff)
            {
                int offset = (address - regs[0x1d] * 0x10000) / 2;
                segaic16_paletteram_w(offset, (ushort)value);
            }
            else if (address >= regs[0x1f] * 0x10000 + 0x0000 && address + 1 <= regs[0x1f] * 0x10000 + 0x3fff)
            {
                int offset = (address - regs[0x1f] * 0x10000) / 2;
                misc_io_w(offset, (ushort)value);
            }
            else
            {
                int offset = address / 2;
                memory_mapper_w(offset, (byte)value);
            }
        }
        public static void MWriteLong_5874(int address, int value)
        {
            MWriteWord_5874(address, (short)(value >> 16));
            MWriteWord_5874(address + 2, (short)value);
        }
        public static sbyte MReadOpByte_5987d(int address)
        {
            address &= 0xffffff;
            sbyte result = 0;
            if (address >= regs[0x11] * 0x10000 && address <= regs[0x11] * 0x10000 + 0xfffff)
            {
                int offset = (address - regs[0x11] * 0x10000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(fd1094_userregion[offset] >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)fd1094_userregion[offset];
                }
            }
            else if (address >= regs[0x13] * 0x10000 && address <= regs[0x13] * 0x10000 + 0x7ffff)
            {
                int offset = (address - regs[0x13] * 0x10000 + 0x80000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(fd1094_userregion[offset] >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)fd1094_userregion[offset];
                }
            }
            return result;
        }
        public static sbyte MReadOpByte_5987(int address)
        {
            address &= 0xffffff;
            sbyte result = 0;
            if (address >= regs[0x11] * 0x10000 && address <= regs[0x11] * 0x10000 + 0xfffff)
            {
                int offset = address - regs[0x11] * 0x10000;
                result = (sbyte)Memory.mainrom[offset];
            }
            else if (address >= regs[0x13] * 0x10000 && address <= regs[0x13] * 0x10000 + 0x7ffff)
            {
                int offset = address - regs[0x13] * 0x10000 + 0x80000;
                result = (sbyte)Memory.mainrom[offset];
            }
            return result;
        }
        public static sbyte MReadByte_5987(int address)
        {
            address &= 0xffffff;
            sbyte result = 0;
            if (address >= regs[0x11] * 0x10000 && address <= regs[0x11] * 0x10000 + 0xfffff)
            {
                int offset = address - regs[0x11] * 0x10000;
                result = (sbyte)Memory.mainrom[offset];
            }
            else if (address >= regs[0x13] * 0x10000 && address <= regs[0x13] * 0x10000 + 0x1fffff)
            {
                int offset = address - regs[0x13] * 0x10000 + 0x80000;
                result = (sbyte)Memory.mainrom[offset];
            }
            else if (address >= regs[0x15] * 0x10000 && address <= regs[0x15] * 0x10000 + 0x10)
            {
                int offset = (address - regs[0x15] * 0x10000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(genesis_vdp_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)genesis_vdp_r(offset);
                }
            }
            else if (address >= regs[0x17] * 0x10000 && address <= regs[0x17] * 0x10000 + 0xffff)
            {
                int offset = (address - regs[0x17] * 0x10000) & 0x3fff;
                result = (sbyte)Memory.mainram[offset];
            }
            else if (address >= regs[0x19] * 0x10000 && address <= regs[0x19] * 0x10000 + 0x7ff)
            {
                int offset = (address - regs[0x19] * 0x10000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(segaic16_spriteram_0[offset] >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)segaic16_spriteram_0[offset];
                }
            }
            else if (address >= regs[0x1b] * 0x10000 + 0x10000 && address <= regs[0x1b] * 0x10000 + 0x10fff)
            {
                int offset = (address - regs[0x1b] * 0x10000 - 0x10000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(segaic16_textram_0[offset] >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)segaic16_textram_0[offset];
                }
            }
            else if (address >= regs[0x1b] * 0x10000 + 0x0000 && address <= regs[0x1b] * 0x10000 + 0xffff)
            {
                int offset = (address - regs[0x1b] * 0x10000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(segaic16_tileram_0[offset] >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)segaic16_tileram_0[offset];
                }
            }
            else if (address >= regs[0x1d] * 0x10000 + 0x0000 && address <= regs[0x1d] * 0x10000 + 0x1fff)
            {
                int offset = (address - regs[0x1d] * 0x10000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(Generic.paletteram16[offset] >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)Generic.paletteram16[offset];
                }
            }
            else if (address >= regs[0x1f] * 0x10000 + 0x0000 && address <= regs[0x1f] * 0x10000 + 0x3fff)
            {
                int offset = (address - regs[0x1f] * 0x10000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(misc_io_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)misc_io_r(offset);
                }
            }
            else
            {
                int offset = address / 2;
                result = 0;
            }
            return result;
        }
        public static short MReadOpWord_5987d(int address)
        {
            address &= 0xffffff;
            short result = 0;
            if (address >= regs[0x11] * 0x10000 && address + 1 <= regs[0x11] * 0x10000 + 0xfffff)
            {
                int offset = (address - regs[0x11] * 0x10000) / 2;
                result = (short)fd1094_userregion[offset];
            }
            else if (address >= regs[0x13] * 0x10000 && address + 1 <= regs[0x13] * 0x10000 + 0x7ffff)
            {
                int offset = (address - regs[0x13] * 0x10000 + 0x80000) / 2;
                result = (short)fd1094_userregion[offset];
            }
            return result;
        }
        public static short MReadOpWord_5987(int address)
        {
            address &= 0xffffff;
            short result = 0;
            if (address >= regs[0x11] * 0x10000 && address + 1 <= regs[0x11] * 0x10000 + 0xfffff)
            {
                int offset = address - regs[0x11] * 0x10000;
                result = (short)(Memory.mainrom[offset] * 0x100 + Memory.mainrom[offset + 1]);
            }
            else if (address >= regs[0x13] * 0x10000 && address + 1 <= regs[0x13] * 0x10000 + 0x7ffff)
            {
                int offset = address - regs[0x13] * 0x10000 + 0x80000;
                result = (short)(Memory.mainrom[offset] * 0x100 + Memory.mainrom[offset + 1]);
            }
            return result;
        }
        public static short MReadWord_5987(int address)
        {
            address &= 0xffffff;
            short result = 0;
            if (address >= regs[0x11] * 0x10000 && address + 1 <= regs[0x11] * 0x10000 + 0xfffff)
            {
                int offset = address - regs[0x11] * 0x10000;
                result = (short)(Memory.mainrom[offset] * 0x100 + Memory.mainrom[offset + 1]);
            }
            else if (address >= regs[0x13] * 0x10000 && address + 1 <= regs[0x13] * 0x10000 + 0x1fffff)
            {
                int offset = address - regs[0x13] * 0x10000 + 0x80000;
                result = (short)(Memory.mainrom[offset] * 0x100 + Memory.mainrom[offset + 1]);
            }
            else if (address >= regs[0x15] * 0x10000 && address + 1 <= regs[0x15] * 0x10000 + 0x10)
            {
                int offset = (address - regs[0x15] * 0x10000) / 2;
                result = (short)genesis_vdp_r(offset);
            }
            else if (address >= regs[0x17] * 0x10000 && address <= regs[0x17] * 0x10000 + 0xffff)
            {
                int offset = (address - regs[0x17] * 0x10000) & 0x3fff;
                result = (short)(Memory.mainram[offset] * 0x100 + Memory.mainram[offset + 1]);
            }
            else if (address >= regs[0x19] * 0x10000 && address <= regs[0x19] * 0x10000 + 0x7ff)
            {
                int offset = (address - regs[0x19] * 0x10000) / 2;
                result = (short)segaic16_spriteram_0[offset];
            }
            else if (address >= regs[0x1b] * 0x10000 + 0x0000 && address <= regs[0x1b] * 0x10000 + 0xffff)
            {
                int offset = (address - regs[0x1b] * 0x10000) / 2;
                result = (short)segaic16_tileram_0[offset];
            }
            else if (address >= regs[0x1b] * 0x10000 + 0x10000 && address <= regs[0x1b] * 0x10000 + 0x10fff)
            {
                int offset = (address - regs[0x1b] * 0x10000 - 0x10000) / 2;
                result = (short)segaic16_textram_0[offset];
            }
            else if (address >= regs[0x1d] * 0x10000 + 0x0000 && address <= regs[0x1d] * 0x10000 + 0x1fff)
            {
                int offset = (address - regs[0x1d] * 0x10000) / 2;
                result = (short)Generic.paletteram16[offset];
            }
            else if (address >= regs[0x1f] * 0x10000 + 0x0000 && address <= regs[0x1f] * 0x10000 + 0x3fff)
            {
                int offset = (address - regs[0x1f] * 0x10000) / 2;
                result = (short)misc_io_r(offset);
            }
            else
            {
                int offset = address / 2;
                result = 0;
            }
            return result;
        }
        public static int MReadOpLong_5987d(int address)
        {
            int result = 0;
            result = (int)((ushort)MReadOpWord_5987d(address) * 0x10000 + (ushort)MReadOpWord_5987d(address + 2));
            return result;
        }
        public static int MReadOpLong_5987(int address)
        {
            int result = 0;
            result = (int)((ushort)MReadOpWord_5987(address) * 0x10000 + (ushort)MReadOpWord_5987(address + 2));
            return result;
        }
        public static int MReadLong_5987(int address)
        {
            int result = 0;
            result = (int)((ushort)MReadWord_5987(address) * 0x10000 + (ushort)MReadWord_5987(address + 2));
            return result;
        }
        public static void MWriteByte_5987(int address, sbyte value)
        {
            address &= 0xffffff;
            if (address >= regs[0x13] * 0x10000 && address <= regs[0x13] * 0x10000 + 0x1fffff)
            {
                int offset = (address - regs[0x13] * 0x10000) / 2;
                if (address % 2 == 0)
                {

                }
                else if (address % 2 == 1)
                {
                    rom_5987_bank_w2(offset, (byte)value);
                }
            }
            else if (address >= regs[0x15] * 0x10000 && address <= regs[0x15] * 0x10000 + 0x10)
            {
                int offset = (address - regs[0x15] * 0x10000) / 2;
                if (address % 2 == 0)
                {
                    genesis_vdp_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    genesis_vdp_w2(offset, (byte)value);
                }
            }
            else if (address >= regs[0x17] * 0x10000 && address <= regs[0x17] * 0x10000 + 0xffff)
            {
                int offset = (address - regs[0x17] * 0x10000) & 0x3fff;
                Memory.mainram[offset] = (byte)value;
            }
            else if (address >= regs[0x19] * 0x10000 && address <= regs[0x19] * 0x10000 + 0x7ff)
            {
                int offset = (address - regs[0x19] * 0x10000) / 2;
                if (address % 2 == 0)
                {
                    segaic16_spriteram_0[offset] = (ushort)(((byte)value << 8) | (segaic16_spriteram_0[offset] & 0xff));
                }
                else if (address % 2 == 1)
                {
                    segaic16_spriteram_0[offset] = (ushort)((segaic16_spriteram_0[offset] & 0xff00) | (byte)value);
                }
            }
            else if (address >= regs[0x1b] * 0x10000 + 0x0000 && address <= regs[0x1b] * 0x10000 + 0xffff)
            {
                int offset = (address - regs[0x1b] * 0x10000) / 2;
                if (address % 2 == 0)
                {
                    segaic16_tileram_0_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    segaic16_tileram_0_w2(offset, (byte)value);
                }
            }
            else if (address >= regs[0x1b] * 0x10000 + 0x10000 && address <= regs[0x1b] * 0x10000 + 0x10fff)
            {
                int offset = (address - regs[0x1b] * 0x10000 - 0x10000) / 2;
                if (address % 2 == 0)
                {
                    segaic16_textram_0_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    segaic16_textram_0_w2(offset, (byte)value);
                }
            }
            else if (address >= regs[0x1d] * 0x10000 + 0x0000 && address <= regs[0x1d] * 0x10000 + 0x1fff)
            {
                int offset = (address - regs[0x1d] * 0x10000) / 2;
                if (address % 2 == 0)
                {
                    segaic16_paletteram_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    segaic16_paletteram_w2(offset, (byte)value);
                }
            }
            else if (address >= regs[0x1f] * 0x10000 + 0x0000 && address <= regs[0x1f] * 0x10000 + 0x3fff)
            {
                int offset = (address - regs[0x1f] * 0x10000) / 2;
                if (address % 2 == 0)
                {
                    misc_io_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    misc_io_w2(offset, (byte)value);
                }
            }
            else
            {
                int offset = address / 2;
                if (address % 2 == 0)
                {

                }
                else if (address % 2 == 1)
                {
                    memory_mapper_w(offset, (byte)value);
                }
            }
        }
        public static void MWriteWord_5987(int address, short value)
        {
            address &= 0xffffff;
            if (address >= regs[0x13] * 0x10000 && address + 1 <= regs[0x13] * 0x10000 + 0x1fffff)
            {
                int offset = (address - regs[0x13] * 0x10000) / 2;
                rom_5987_bank_w(offset, (ushort)value);
            }
            else if (address >= regs[0x15] * 0x10000 && address + 1 <= regs[0x15] * 0x10000 + 0x10)
            {
                int offset = (address - regs[0x15] * 0x10000) / 2;
                genesis_vdp_w(offset, (ushort)value);
            }
            else if (address >= regs[0x17] * 0x10000 && address + 1 <= regs[0x17] * 0x10000 + 0xffff)
            {
                int offset = (address - regs[0x17] * 0x10000) & 0x3fff;
                Memory.mainram[offset] = (byte)(value >> 8);
                Memory.mainram[offset + 1] = (byte)value;
            }
            else if (address >= regs[0x19] * 0x10000 && address + 1 <= regs[0x19] * 0x10000 + 0x7ff)
            {
                int offset = (address - regs[0x19] * 0x10000) / 2;
                segaic16_spriteram_0[offset] = (ushort)value;
            }
            else if (address >= regs[0x1b] * 0x10000 + 0x0000 && address + 1 <= regs[0x1b] * 0x10000 + 0xffff)
            {
                int offset = (address - regs[0x1b] * 0x10000) / 2;
                segaic16_tileram_0_w(offset, (ushort)value);
            }
            else if (address >= regs[0x1b] * 0x10000 + 0x10000 && address + 1 <= regs[0x1b] * 0x10000 + 0x10fff)
            {
                int offset = (address - regs[0x1b] * 0x10000 - 0x10000) / 2;
                segaic16_textram_0_w(offset, (ushort)value);
            }
            else if (address >= regs[0x1d] * 0x10000 + 0x0000 && address + 1 <= regs[0x1d] * 0x10000 + 0x1fff)
            {
                int offset = (address - regs[0x1d] * 0x10000) / 2;
                segaic16_paletteram_w(offset, (ushort)value);
            }
            else if (address >= regs[0x1f] * 0x10000 + 0x0000 && address + 1 <= regs[0x1f] * 0x10000 + 0x3fff)
            {
                int offset = (address - regs[0x1f] * 0x10000) / 2;
                misc_io_w(offset, (ushort)value);
            }
            else
            {
                int offset = address / 2;
                memory_mapper_w(offset, (byte)value);
            }
        }
        public static void MWriteLong_5987(int address, int value)
        {
            MWriteWord_5987(address, (short)(value >> 16));
            MWriteWord_5987(address + 2, (short)value);
        }
        public static sbyte MReadOpByte_5987d_desertbr(int address)
        {
            address &= 0xffffff;
            sbyte result = 0;
            if (address >= regs[0x11] * 0x10000 && address <= regs[0x11] * 0x10000 + 0xfffff)
            {
                int offset = (address - regs[0x11] * 0x10000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(fd1094_userregion[offset] >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)fd1094_userregion[offset];
                }
            }
            else if (address >= regs[0x13] * 0x10000 && address <= regs[0x13] * 0x10000 + 0x7ffff)
            {
                int offset = (address - regs[0x13] * 0x10000 + 0x100000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(fd1094_userregion[offset] >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)fd1094_userregion[offset];
                }
            }
            return result;
        }
        public static sbyte MReadOpByte_5987_desertbr(int address)
        {
            address &= 0xffffff;
            sbyte result = 0;
            if (address >= regs[0x11] * 0x10000 && address <= regs[0x11] * 0x10000 + 0xfffff)
            {
                int offset = address - regs[0x11] * 0x10000;
                result = (sbyte)Memory.mainrom[offset];
            }
            else if (address >= regs[0x13] * 0x10000 && address <= regs[0x13] * 0x10000 + 0x1fffff)
            {
                int offset = address - regs[0x13] * 0x10000 + 0x100000;
                result = (sbyte)Memory.mainrom[offset];
            }
            return result;
        }
        public static sbyte MReadByte_5987_desertbr(int address)
        {
            address &= 0xffffff;
            sbyte result = 0;
            if (address >= regs[0x11] * 0x10000 && address <= regs[0x11] * 0x10000 + 0xfffff)
            {
                int offset = address - regs[0x11] * 0x10000;
                result = (sbyte)Memory.mainrom[offset];
            }
            else if (address >= regs[0x13] * 0x10000 && address <= regs[0x13] * 0x10000 + 0x1fffff)
            {
                int offset = address - regs[0x13] * 0x10000 + 0x100000;
                result = (sbyte)Memory.mainrom[offset];
            }
            else if (address >= regs[0x15] * 0x10000 && address <= regs[0x15] * 0x10000 + 0x10)
            {
                int offset = (address - regs[0x15] * 0x10000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(genesis_vdp_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)genesis_vdp_r(offset);
                }
            }
            else if (address >= regs[0x17] * 0x10000 && address <= regs[0x17] * 0x10000 + 0xffff)
            {
                int offset = (address - regs[0x17] * 0x10000) & 0x3fff;
                result = (sbyte)Memory.mainram[offset];
            }
            else if (address >= regs[0x19] * 0x10000 && address <= regs[0x19] * 0x10000 + 0x7ff)
            {
                int offset = (address - regs[0x19] * 0x10000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(segaic16_spriteram_0[offset] >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)segaic16_spriteram_0[offset];
                }
            }
            else if (address >= regs[0x1b] * 0x10000 + 0x10000 && address <= regs[0x1b] * 0x10000 + 0x10fff)
            {
                int offset = (address - regs[0x1b] * 0x10000 - 0x10000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(segaic16_textram_0[offset] >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)segaic16_textram_0[offset];
                }
            }
            else if (address >= regs[0x1b] * 0x10000 + 0x0000 && address <= regs[0x1b] * 0x10000 + 0xffff)
            {
                int offset = (address - regs[0x1b] * 0x10000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(segaic16_tileram_0[offset] >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)segaic16_tileram_0[offset];
                }
            }
            else if (address >= regs[0x1d] * 0x10000 + 0x0000 && address <= regs[0x1d] * 0x10000 + 0x1fff)
            {
                int offset = (address - regs[0x1d] * 0x10000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(Generic.paletteram16[offset] >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)Generic.paletteram16[offset];
                }
            }
            else if (address >= regs[0x1f] * 0x10000 + 0x0000 && address <= regs[0x1f] * 0x10000 + 0x3fff)
            {
                int offset = (address - regs[0x1f] * 0x10000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(misc_io_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)misc_io_r(offset);
                }
            }
            else
            {
                int offset = address / 2;
                result = 0;
            }
            return result;
        }
        public static short MReadOpWord_5987d_desertbr(int address)
        {
            address &= 0xffffff;
            short result = 0;
            if (address >= regs[0x11] * 0x10000 && address + 1 <= regs[0x11] * 0x10000 + 0xfffff)
            {
                int offset = (address - regs[0x11] * 0x10000) / 2;
                result = (short)fd1094_userregion[offset];
            }
            else if (address >= regs[0x13] * 0x10000 && address + 1 <= regs[0x13] * 0x10000 + 0x1fffff)
            {
                int offset = (address - regs[0x13] * 0x10000 + 0x100000) / 2;
                result = (short)fd1094_userregion[offset];
            }
            return result;
        }
        public static short MReadOpWord_5987_desertbr(int address)
        {
            address &= 0xffffff;
            short result = 0;
            if (address >= regs[0x11] * 0x10000 && address + 1 <= regs[0x11] * 0x10000 + 0xfffff)
            {
                int offset = address - regs[0x11] * 0x10000;
                result = (short)(Memory.mainrom[offset] * 0x100 + Memory.mainrom[offset + 1]);
            }
            else if (address >= regs[0x13] * 0x10000 && address + 1 <= regs[0x13] * 0x10000 + 0x1fffff)
            {
                int offset = address - regs[0x13] * 0x10000 + 0x100000;
                result = (short)(Memory.mainrom[offset] * 0x100 + Memory.mainrom[offset + 1]);
            }
            return result;
        }
        public static short MReadWord_5987_desertbr(int address)
        {
            address &= 0xffffff;
            short result = 0;
            if (address >= regs[0x11] * 0x10000 && address + 1 <= regs[0x11] * 0x10000 + 0xfffff)
            {
                int offset = address - regs[0x11] * 0x10000;
                result = (short)(Memory.mainrom[offset] * 0x100 + Memory.mainrom[offset + 1]);
            }
            else if (address >= regs[0x13] * 0x10000 && address + 1 <= regs[0x13] * 0x10000 + 0x1fffff)
            {
                int offset = address - regs[0x13] * 0x10000 + 0x100000;
                result = (short)(Memory.mainrom[offset] * 0x100 + Memory.mainrom[offset + 1]);
            }
            else if (address >= regs[0x15] * 0x10000 && address + 1 <= regs[0x15] * 0x10000 + 0x10)
            {
                int offset = (address - regs[0x15] * 0x10000) / 2;
                result = (short)genesis_vdp_r(offset);
            }
            else if (address >= regs[0x17] * 0x10000 && address <= regs[0x17] * 0x10000 + 0xffff)
            {
                int offset = (address - regs[0x17] * 0x10000) & 0x3fff;
                result = (short)(Memory.mainram[offset] * 0x100 + Memory.mainram[offset + 1]);
            }
            else if (address >= regs[0x19] * 0x10000 && address <= regs[0x19] * 0x10000 + 0x7ff)
            {
                int offset = (address - regs[0x19] * 0x10000) / 2;
                result = (short)segaic16_spriteram_0[offset];
            }
            else if (address >= regs[0x1b] * 0x10000 + 0x0000 && address <= regs[0x1b] * 0x10000 + 0xffff)
            {
                int offset = (address - regs[0x1b] * 0x10000) / 2;
                result = (short)segaic16_tileram_0[offset];
            }
            else if (address >= regs[0x1b] * 0x10000 + 0x10000 && address <= regs[0x1b] * 0x10000 + 0x10fff)
            {
                int offset = (address - regs[0x1b] * 0x10000 - 0x10000) / 2;
                result = (short)segaic16_textram_0[offset];
            }
            else if (address >= regs[0x1d] * 0x10000 + 0x0000 && address <= regs[0x1d] * 0x10000 + 0x1fff)
            {
                int offset = (address - regs[0x1d] * 0x10000) / 2;
                result = (short)Generic.paletteram16[offset];
            }
            else if (address >= regs[0x1f] * 0x10000 + 0x0000 && address <= regs[0x1f] * 0x10000 + 0x3fff)
            {
                int offset = (address - regs[0x1f] * 0x10000) / 2;
                result = (short)misc_io_r(offset);
            }
            else
            {
                int offset = address / 2;
                result = 0;
            }
            return result;
        }
        public static int MReadOpLong_5987d_desertbr(int address)
        {
            int result = 0;
            result = (int)((ushort)MReadOpWord_5987d_desertbr(address) * 0x10000 + (ushort)MReadOpWord_5987d_desertbr(address + 2));
            return result;
        }
        public static int MReadOpLong_5987_desertbr(int address)
        {
            int result = 0;
            result = (int)((ushort)MReadOpWord_5987_desertbr(address) * 0x10000 + (ushort)MReadOpWord_5987_desertbr(address + 2));
            return result;
        }
        public static int MReadLong_5987_desertbr(int address)
        {
            int result = 0;
            result = (int)((ushort)MReadWord_5987_desertbr(address) * 0x10000 + (ushort)MReadWord_5987_desertbr(address + 2));
            return result;
        }
        public static sbyte MReadOpByte_7525(int address)
        {
            address &= 0xffffff;
            sbyte result = 0;
            if (address >= regs[0x11] * 0x10000 && address <= regs[0x11] * 0x10000 + 0xfffff)
            {
                int offset = address - regs[0x11] * 0x10000;
                result = (sbyte)Memory.mainrom[offset];
            }
            else if (address >= regs[0x13] * 0x10000 && address <= regs[0x13] * 0x10000 + 0x7ffff)
            {
                int offset = address - regs[0x13] * 0x10000 + 0x80000;
                result = (sbyte)Memory.mainrom[offset];
            }
            else if (address >= regs[0x13] * 0x10000 + 0x80000&&address<=regs[0x13] * 0x10000 +0x1fffff)
            {
                result = 0;
            }
            return result;
        }
        public static sbyte MReadByte_7525(int address)
        {
            address &= 0xffffff;
            sbyte result = 0;
            if (address >= regs[0x11] * 0x10000 && address <= regs[0x11] * 0x10000 + 0xfffff)
            {
                int offset = address - regs[0x11] * 0x10000;
                result = (sbyte)Memory.mainrom[offset];
            }
            else if (address >= regs[0x13] * 0x10000 && address <= regs[0x13] * 0x10000 + 0x7ffff)
            {
                int offset = address - regs[0x13] * 0x10000 + 0x80000;
                result = (sbyte)Memory.mainrom[offset];
            }
            else if (address >= regs[0x13] * 0x10000 + 0x80000 && address <= regs[0x13] * 0x10000 + 0x1fffff)
            {
                result = 0;
            }
            else if (address >= regs[0x15] * 0x10000 && address <= regs[0x15] * 0x10000 + 0x10)
            {
                int offset = (address - regs[0x15] * 0x10000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(genesis_vdp_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)genesis_vdp_r(offset);
                }
            }
            else if (address >= regs[0x17] * 0x10000 && address <= regs[0x17] * 0x10000 + 0xffff)
            {
                int offset = (address - regs[0x17] * 0x10000) & 0x3fff;
                result = (sbyte)Memory.mainram[offset];
            }
            else if (address >= regs[0x19] * 0x10000 && address <= regs[0x19] * 0x10000 + 0x7ff)
            {
                int offset = (address - regs[0x19] * 0x10000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(segaic16_spriteram_0[offset] >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)segaic16_spriteram_0[offset];
                }
            }
            else if (address >= regs[0x1b] * 0x10000 + 0x10000 && address <= regs[0x1b] * 0x10000 + 0x10fff)
            {
                int offset = (address - regs[0x1b] * 0x10000 - 0x10000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(segaic16_textram_0[offset] >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)segaic16_textram_0[offset];
                }
            }
            else if (address >= regs[0x1b] * 0x10000 + 0x0000 && address <= regs[0x1b] * 0x10000 + 0xffff)
            {
                int offset = (address - regs[0x1b] * 0x10000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(segaic16_tileram_0[offset] >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)segaic16_tileram_0[offset];
                }
            }
            else if (address >= regs[0x1d] * 0x10000 + 0x0000 && address <= regs[0x1d] * 0x10000 + 0x1fff)
            {
                int offset = (address - regs[0x1d] * 0x10000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(Generic.paletteram16[offset] >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)Generic.paletteram16[offset];
                }
            }
            else if (address >= regs[0x1f] * 0x10000 + 0x0000 && address <= regs[0x1f] * 0x10000 + 0x3fff)
            {
                int offset = (address - regs[0x1f] * 0x10000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(misc_io_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)misc_io_r(offset);
                }
            }
            else
            {
                int offset = address / 2;
                result = 0;
            }
            return result;
        }
        public static short MReadOpWord_7525(int address)
        {
            address &= 0xffffff;
            short result = 0;
            if (address >= regs[0x11] * 0x10000 && address + 1 <= regs[0x11] * 0x10000 + 0xfffff)
            {
                int offset = address - regs[0x11] * 0x10000;
                result = (short)(Memory.mainrom[offset] * 0x100 + Memory.mainrom[offset + 1]);
            }
            else if (address >= regs[0x13] * 0x10000 && address + 1 <= regs[0x13] * 0x10000 + 0x7ffff)
            {
                int offset = address - regs[0x13] * 0x10000 + 0x80000;
                result = (short)(Memory.mainrom[offset] * 0x100 + Memory.mainrom[offset + 1]);
            }
            else if (address >= regs[0x13] * 0x10000 + 0x80000 && address + 1 <= regs[0x13] * 0x10000 + 0x1fffff)
            {
                result = 0;
            }
            return result;
        }
        public static short MReadWord_7525(int address)
        {
            address &= 0xffffff;
            short result = 0;
            if (address >= regs[0x11] * 0x10000 && address + 1 <= regs[0x11] * 0x10000 + 0xfffff)
            {
                int offset = address - regs[0x11] * 0x10000;
                result = (short)(Memory.mainrom[offset] * 0x100 + Memory.mainrom[offset + 1]);
            }
            else if (address >= regs[0x13] * 0x10000 && address + 1 <= regs[0x13] * 0x10000 + 0x7ffff)
            {
                int offset = address - regs[0x13] * 0x10000 + 0x80000;
                result = (short)(Memory.mainrom[offset] * 0x100 + Memory.mainrom[offset + 1]);
            }
            else if (address >= regs[0x13] * 0x10000 + 0x80000 && address + 1 <= regs[0x13] * 0x10000 + 0x1fffff)
            {
                result = 0;
            }
            else if (address >= regs[0x15] * 0x10000 && address + 1 <= regs[0x15] * 0x10000 + 0x10)
            {
                int offset = (address - regs[0x15] * 0x10000) / 2;
                result = (short)genesis_vdp_r(offset);
            }
            else if (address >= regs[0x17] * 0x10000 && address <= regs[0x17] * 0x10000 + 0xffff)
            {
                int offset = (address - regs[0x17] * 0x10000) & 0x3fff;
                result = (short)(Memory.mainram[offset] * 0x100 + Memory.mainram[offset + 1]);
            }
            else if (address >= regs[0x19] * 0x10000 && address <= regs[0x19] * 0x10000 + 0x7ff)
            {
                int offset = (address - regs[0x19] * 0x10000) / 2;
                result = (short)segaic16_spriteram_0[offset];
            }
            else if (address >= regs[0x1b] * 0x10000 + 0x0000 && address <= regs[0x1b] * 0x10000 + 0xffff)
            {
                int offset = (address - regs[0x1b] * 0x10000) / 2;
                result = (short)segaic16_tileram_0[offset];
            }
            else if (address >= regs[0x1b] * 0x10000 + 0x10000 && address <= regs[0x1b] * 0x10000 + 0x10fff)
            {
                int offset = (address - regs[0x1b] * 0x10000 - 0x10000) / 2;
                result = (short)segaic16_textram_0[offset];
            }
            else if (address >= regs[0x1d] * 0x10000 + 0x0000 && address <= regs[0x1d] * 0x10000 + 0x1fff)
            {
                int offset = (address - regs[0x1d] * 0x10000) / 2;
                result = (short)Generic.paletteram16[offset];
            }
            else if (address >= regs[0x1f] * 0x10000 + 0x0000 && address <= regs[0x1f] * 0x10000 + 0x3fff)
            {
                int offset = (address - regs[0x1f] * 0x10000) / 2;
                result = (short)misc_io_r(offset);
            }
            else
            {
                int offset = address / 2;
                result = 0;
            }
            return result;
        }
        public static int MReadOpLong_7525(int address)
        {
            int result = 0;
            result = (int)((ushort)MReadOpWord_7525(address) * 0x10000 + (ushort)MReadOpWord_7525(address + 2));
            return result;
        }
        public static int MReadLong_7525(int address)
        {
            int result = 0;
            result = (int)((ushort)MReadWord_5987(address) * 0x10000 + (ushort)MReadWord_5987(address + 2));
            return result;
        }
        public static void MWriteByte_7525(int address, sbyte value)
        {
            address &= 0xffffff;
            if (address >= regs[0x13] * 0x10000 && address <= regs[0x13] * 0x10000 + 0x7ffff)
            {
                int offset = (address - regs[0x13] * 0x10000) / 2;
                if (address % 2 == 0)
                {

                }
                else if (address % 2 == 1)
                {
                    rom_837_7525_bank_w2(offset, (byte)value);
                }
            }
            else if (address >= regs[0x15] * 0x10000 && address <= regs[0x15] * 0x10000 + 0x10)
            {
                int offset = (address - regs[0x15] * 0x10000) / 2;
                if (address % 2 == 0)
                {
                    genesis_vdp_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    genesis_vdp_w2(offset, (byte)value);
                }
            }
            else if (address >= regs[0x17] * 0x10000 && address <= regs[0x17] * 0x10000 + 0xffff)
            {
                int offset = (address - regs[0x17] * 0x10000) & 0x3fff;
                Memory.mainram[offset] = (byte)value;
            }
            else if (address >= regs[0x19] * 0x10000 && address <= regs[0x19] * 0x10000 + 0x7ff)
            {
                int offset = (address - regs[0x19] * 0x10000) / 2;
                if (address % 2 == 0)
                {
                    segaic16_spriteram_0[offset] = (ushort)(((byte)value << 8) | (segaic16_spriteram_0[offset] & 0xff));
                }
                else if (address % 2 == 1)
                {
                    segaic16_spriteram_0[offset] = (ushort)((segaic16_spriteram_0[offset] & 0xff00) | (byte)value);
                }
            }
            else if (address >= regs[0x1b] * 0x10000 + 0x0000 && address <= regs[0x1b] * 0x10000 + 0xffff)
            {
                int offset = (address - regs[0x1b] * 0x10000) / 2;
                if (address % 2 == 0)
                {
                    segaic16_tileram_0_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    segaic16_tileram_0_w2(offset, (byte)value);
                }
            }
            else if (address >= regs[0x1b] * 0x10000 + 0x10000 && address <= regs[0x1b] * 0x10000 + 0x10fff)
            {
                int offset = (address - regs[0x1b] * 0x10000 - 0x10000) / 2;
                if (address % 2 == 0)
                {
                    segaic16_textram_0_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    segaic16_textram_0_w2(offset, (byte)value);
                }
            }
            else if (address >= regs[0x1d] * 0x10000 + 0x0000 && address <= regs[0x1d] * 0x10000 + 0x1fff)
            {
                int offset = (address - regs[0x1d] * 0x10000) / 2;
                if (address % 2 == 0)
                {
                    segaic16_paletteram_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    segaic16_paletteram_w2(offset, (byte)value);
                }
            }
            else if (address >= regs[0x1f] * 0x10000 + 0x0000 && address <= regs[0x1f] * 0x10000 + 0x3fff)
            {
                int offset = (address - regs[0x1f] * 0x10000) / 2;
                if (address % 2 == 0)
                {
                    misc_io_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    misc_io_w2(offset, (byte)value);
                }
            }
            else
            {
                int offset = address / 2;
                if (address % 2 == 0)
                {

                }
                else if (address % 2 == 1)
                {
                    memory_mapper_w(offset, (byte)value);
                }
            }
        }
        public static void MWriteWord_7525(int address, short value)
        {
            address &= 0xffffff;
            if (address >= regs[0x13] * 0x10000 && address + 1 <= regs[0x13] * 0x10000 + 0x7ffff)
            {
                int offset = (address - regs[0x13] * 0x10000) / 2;
                rom_837_7525_bank_w(offset, (ushort)value);
            }
            else if (address >= regs[0x15] * 0x10000 && address + 1 <= regs[0x15] * 0x10000 + 0x10)
            {
                int offset = (address - regs[0x15] * 0x10000) / 2;
                genesis_vdp_w(offset, (ushort)value);
            }
            else if (address >= regs[0x17] * 0x10000 && address + 1 <= regs[0x17] * 0x10000 + 0xffff)
            {
                int offset = (address - regs[0x17] * 0x10000) & 0x3fff;
                Memory.mainram[offset] = (byte)(value >> 8);
                Memory.mainram[offset + 1] = (byte)value;
            }
            else if (address >= regs[0x19] * 0x10000 && address + 1 <= regs[0x19] * 0x10000 + 0x7ff)
            {
                int offset = (address - regs[0x19] * 0x10000) / 2;
                segaic16_spriteram_0[offset] = (ushort)value;
            }
            else if (address >= regs[0x1b] * 0x10000 + 0x0000 && address + 1 <= regs[0x1b] * 0x10000 + 0xffff)
            {
                int offset = (address - regs[0x1b] * 0x10000) / 2;
                segaic16_tileram_0_w(offset, (ushort)value);
            }
            else if (address >= regs[0x1b] * 0x10000 + 0x10000 && address + 1 <= regs[0x1b] * 0x10000 + 0x10fff)
            {
                int offset = (address - regs[0x1b] * 0x10000 - 0x10000) / 2;
                segaic16_textram_0_w(offset, (ushort)value);
            }
            else if (address >= regs[0x1d] * 0x10000 + 0x0000 && address + 1 <= regs[0x1d] * 0x10000 + 0x1fff)
            {
                int offset = (address - regs[0x1d] * 0x10000) / 2;
                segaic16_paletteram_w(offset, (ushort)value);
            }
            else if (address >= regs[0x1f] * 0x10000 + 0x0000 && address + 1 <= regs[0x1f] * 0x10000 + 0x3fff)
            {
                int offset = (address - regs[0x1f] * 0x10000) / 2;
                misc_io_w(offset, (ushort)value);
            }
            else
            {
                int offset = address / 2;
                memory_mapper_w(offset, (byte)value);
            }
        }
        public static void MWriteLong_7525(int address, int value)
        {
            MWriteWord_7525(address, (short)(value >> 16));
            MWriteWord_7525(address + 2, (short)value);
        }
        public static sbyte MReadOpByte_shdancer(int address)
        {
            address &= 0xffffff;
            sbyte result = 0;
            if (address >= regs[0x11] * 0x10000 && address <= regs[0x11] * 0x10000 + 0x7ffff)
            {
                int offset = address - regs[0x11] * 0x10000;
                result = (sbyte)Memory.mainrom[offset];
            }
            return result;
        }
        public static sbyte MReadByte_shdancer(int address)
        {
            address &= 0xffffff;
            sbyte result = 0;
            if (address >= regs[0x11] * 0x10000 && address <= regs[0x11] * 0x10000 + 0x7ffff)
            {
                int offset = address - regs[0x11] * 0x10000;
                result = (sbyte)Memory.mainrom[offset];
            }
            else if (address >= regs[0x13] * 0x10000 && address <= regs[0x13] * 0x10000 + 0x10)
            {
                int offset = (address - regs[0x13] * 0x10000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(genesis_vdp_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)genesis_vdp_r(offset);
                }
            }
            else if (address >= regs[0x17] * 0x10000 && address <= regs[0x17] * 0x10000 + 0xffff)
            {
                int offset = (address - regs[0x17] * 0x10000) & 0x3fff;
                result = (sbyte)Memory.mainram[offset];
            }
            else if (address >= regs[0x19] * 0x10000 && address <= regs[0x19] * 0x10000 + 0x7ff)
            {
                int offset = (address - regs[0x19] * 0x10000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(segaic16_spriteram_0[offset] >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)segaic16_spriteram_0[offset];
                }
            }
            else if (address >= regs[0x1b] * 0x10000 + 0x10000 && address <= regs[0x1b] * 0x10000 + 0x10fff)
            {
                int offset = (address - regs[0x1b] * 0x10000 - 0x10000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(segaic16_textram_0[offset] >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)segaic16_textram_0[offset];
                }
            }
            else if (address >= regs[0x1b] * 0x10000 + 0x0000 && address <= regs[0x1b] * 0x10000 + 0xffff)
            {
                int offset = (address - regs[0x1b] * 0x10000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(segaic16_tileram_0[offset] >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)segaic16_tileram_0[offset];
                }
            }
            else if (address >= regs[0x1d] * 0x10000 + 0x0000 && address <= regs[0x1d] * 0x10000 + 0x1fff)
            {
                int offset = (address - regs[0x1d] * 0x10000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(Generic.paletteram16[offset] >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)Generic.paletteram16[offset];
                }
            }
            else if (address >= regs[0x1f] * 0x10000 + 0x0000 && address <= regs[0x1f] * 0x10000 + 0x3fff)
            {
                int offset = (address - regs[0x1f] * 0x10000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(misc_io_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)misc_io_r(offset);
                }
            }
            else
            {
                int offset = address / 2;
                result = 0;
            }
            return result;
        }
        public static short MReadOpWord_shdancer(int address)
        {
            address &= 0xffffff;
            short result = 0;
            if (address >= regs[0x11] * 0x10000 && address + 1 <= regs[0x11] * 0x10000 + 0x7ffff)
            {
                int offset = address - regs[0x11] * 0x10000;
                result = (short)(Memory.mainrom[offset] * 0x100 + Memory.mainrom[offset + 1]);
            }
            return result;
        }
        public static short MReadWord_shdancer(int address)
        {
            address &= 0xffffff;
            short result = 0;
            if (address >= regs[0x11] * 0x10000 && address+1 <= regs[0x11] * 0x10000 + 0x7ffff)
            {
                int offset = address - regs[0x11] * 0x10000;
                result = (short)(Memory.mainrom[offset] * 0x100 + Memory.mainrom[offset + 1]);
            }
            else if (address >= regs[0x13] * 0x10000 && address + 1 <= regs[0x13] * 0x10000 + 0x10)
            {
                int offset = (address - regs[0x13] * 0x10000) / 2;
                result = (short)genesis_vdp_r(offset);
            }
            else if (address >= regs[0x17] * 0x10000 && address <= regs[0x17] * 0x10000 + 0xffff)
            {
                int offset = (address - regs[0x17] * 0x10000) & 0x3fff;
                result = (short)(Memory.mainram[offset] * 0x100 + Memory.mainram[offset + 1]);
            }
            else if (address >= regs[0x19] * 0x10000 && address <= regs[0x19] * 0x10000 + 0x7ff)
            {
                int offset = (address - regs[0x19] * 0x10000) / 2;
                result = (short)segaic16_spriteram_0[offset];
            }
            else if (address >= regs[0x1b] * 0x10000 + 0x0000 && address <= regs[0x1b] * 0x10000 + 0xffff)
            {
                int offset = (address - regs[0x1b] * 0x10000) / 2;
                result = (short)segaic16_tileram_0[offset];
            }
            else if (address >= regs[0x1b] * 0x10000 + 0x10000 && address <= regs[0x1b] * 0x10000 + 0x10fff)
            {
                int offset = (address - regs[0x1b] * 0x10000 - 0x10000) / 2;
                result = (short)segaic16_textram_0[offset];
            }
            else if (address >= regs[0x1d] * 0x10000 + 0x0000 && address <= regs[0x1d] * 0x10000 + 0x1fff)
            {
                int offset = (address - regs[0x1d] * 0x10000) / 2;
                result = (short)Generic.paletteram16[offset];
            }
            else if (address >= regs[0x1f] * 0x10000 + 0x0000 && address <= regs[0x1f] * 0x10000 + 0x3fff)
            {
                int offset = (address - regs[0x1f] * 0x10000) / 2;
                result = (short)misc_io_r(offset);
            }
            else
            {
                int offset = address / 2;
                result = 0;
            }
            return result;
        }
        public static int MReadOpLong_shdancer(int address)
        {
            int result = 0;
            result = (int)((ushort)MReadOpWord_shdancer(address) * 0x10000 + (ushort)MReadOpWord_shdancer(address + 2));
            return result;
        }
        public static int MReadLong_shdancer(int address)
        {
            int result = 0;
            result = (int)((ushort)MReadWord_shdancer(address) * 0x10000 + (ushort)MReadWord_shdancer(address + 2));
            return result;
        }
        public static void MWriteByte_shdancer(int address, sbyte value)
        {
            address &= 0xffffff;
            if (address >= regs[0x13] * 0x10000 && address <= regs[0x13] * 0x10000 + 0x10)
            {
                int offset = (address - regs[0x13] * 0x10000) / 2;
                if (address % 2 == 0)
                {
                    genesis_vdp_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    genesis_vdp_w2(offset, (byte)value);
                }
            }
            else if (address >= regs[0x17] * 0x10000 && address <= regs[0x17] * 0x10000 + 0xffff)
            {
                int offset = (address - regs[0x17] * 0x10000) & 0x3fff;
                Memory.mainram[offset] = (byte)value;
            }
            else if (address >= regs[0x19] * 0x10000 && address <= regs[0x19] * 0x10000 + 0x7ff)
            {
                int offset = (address - regs[0x19] * 0x10000) / 2;
                if (address % 2 == 0)
                {
                    segaic16_spriteram_0[offset] = (ushort)(((byte)value << 8) | (segaic16_spriteram_0[offset] & 0xff));
                }
                else if (address % 2 == 1)
                {
                    segaic16_spriteram_0[offset] = (ushort)((segaic16_spriteram_0[offset] & 0xff00) | (byte)value);
                }
            }
            else if (address >= regs[0x1b] * 0x10000 + 0x0000 && address <= regs[0x1b] * 0x10000 + 0xffff)
            {
                int offset = (address - regs[0x1b] * 0x10000) / 2;
                if (address % 2 == 0)
                {
                    segaic16_tileram_0_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    segaic16_tileram_0_w2(offset, (byte)value);
                }
            }
            else if (address >= regs[0x1b] * 0x10000 + 0x10000 && address <= regs[0x1b] * 0x10000 + 0x10fff)
            {
                int offset = (address - regs[0x1b] * 0x10000 - 0x10000) / 2;
                if (address % 2 == 0)
                {
                    segaic16_textram_0_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    segaic16_textram_0_w2(offset, (byte)value);
                }
            }
            else if (address >= regs[0x1d] * 0x10000 + 0x0000 && address <= regs[0x1d] * 0x10000 + 0x1fff)
            {
                int offset = (address - regs[0x1d] * 0x10000) / 2;
                if (address % 2 == 0)
                {
                    segaic16_paletteram_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    segaic16_paletteram_w2(offset, (byte)value);
                }
            }
            else if (address >= regs[0x1f] * 0x10000 + 0x0000 && address <= regs[0x1f] * 0x10000 + 0x3fff)
            {
                int offset = (address - regs[0x1f] * 0x10000) / 2;
                if (address % 2 == 0)
                {
                    misc_io_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    misc_io_w2(offset, (byte)value);
                }
            }
            else
            {
                int offset = address / 2;
                if (address % 2 == 0)
                {

                }
                else if (address % 2 == 1)
                {
                    memory_mapper_w(offset, (byte)value);
                }
            }
        }
        public static void MWriteWord_shdancer(int address, short value)
        {
            address &= 0xffffff;
            if (address >= regs[0x13] * 0x10000 && address + 1 <= regs[0x13] * 0x10000 + 0x10)
            {
                int offset = (address - regs[0x13] * 0x10000) / 2;
                genesis_vdp_w(offset, (ushort)value);
            }
            else if (address >= regs[0x17] * 0x10000 && address + 1 <= regs[0x17] * 0x10000 + 0xffff)
            {
                int offset = (address - regs[0x17] * 0x10000) & 0x3fff;
                Memory.mainram[offset] = (byte)(value >> 8);
                Memory.mainram[offset + 1] = (byte)value;
            }
            else if (address >= regs[0x19] * 0x10000 && address + 1 <= regs[0x19] * 0x10000 + 0x7ff)
            {
                int offset = (address - regs[0x19] * 0x10000) / 2;
                segaic16_spriteram_0[offset] = (ushort)value;
            }
            else if (address >= regs[0x1b] * 0x10000 + 0x0000 && address + 1 <= regs[0x1b] * 0x10000 + 0xffff)
            {
                int offset = (address - regs[0x1b] * 0x10000) / 2;
                segaic16_tileram_0_w(offset, (ushort)value);
            }
            else if (address >= regs[0x1b] * 0x10000 + 0x10000 && address + 1 <= regs[0x1b] * 0x10000 + 0x10fff)
            {
                int offset = (address - regs[0x1b] * 0x10000 - 0x10000) / 2;
                segaic16_textram_0_w(offset, (ushort)value);
            }
            else if (address >= regs[0x1d] * 0x10000 + 0x0000 && address + 1 <= regs[0x1d] * 0x10000 + 0x1fff)
            {
                int offset = (address - regs[0x1d] * 0x10000) / 2;
                segaic16_paletteram_w(offset, (ushort)value);
            }
            else if (address >= regs[0x1f] * 0x10000 + 0x0000 && address + 1 <= regs[0x1f] * 0x10000 + 0x3fff)
            {
                int offset = (address - regs[0x1f] * 0x10000) / 2;
                misc_io_w(offset, (ushort)value);
            }
            else
            {
                int offset = address / 2;
                memory_mapper_w(offset, (byte)value);
            }
        }
        public static void MWriteLong_shdancer(int address, int value)
        {
            MWriteWord_shdancer(address, (short)(value >> 16));
            MWriteWord_shdancer(address + 2, (short)value);
        }

        
        public static byte ZReadOp_system18(ushort address)
        {
            byte result = 0;
            if (address >= 0x0000 && address <= 0x9fff)
            {
                result = Memory.audiorom[address];
            }
            else if (address >= 0xa000 && address <= 0xbfff)
            {
                int offset = address - 0xa000;
                result = Memory.audiorom[basebanksnd + offset];
            }
            return result;
        }
        public static byte ZReadMemory_system18(ushort address)
        {
            byte result = 0;
            if (address >= 0x0000 && address <= 0x9fff)
            {
                result = Memory.audiorom[address];
            }
            else if (address >= 0xa000 && address <= 0xbfff)
            {
                int offset = address - 0xa000;
                result = Memory.audiorom[basebanksnd + offset];
            }
            else if (address >= 0xc000 && address <= 0xcfff)
            {
                int offset = address & 0xf;
                result = rf5c68.rf5c68_r(offset);
            }
            else if (address >= 0xd000 && address <= 0xdfff)
            {
                int offset = address - 0xd000;
                result = rf5c68.rf5c68_r(offset);
            }
            else if (address >= 0xe000 && address <= 0xffff)
            {
                int offset = address - 0xe000;
                result = Memory.audioram[offset];
            }
            return result;
        }
        public static void ZWriteMemory_system18(ushort address, byte value)
        {
            if (address >= 0xc000 && address <= 0xc00f)
            {
                int offset = address - 0xc000;
                rf5c68.rf5c68_reg_w(offset, value);
            }
            else if (address >= 0xd000 && address <= 0xdfff)
            {
                int offset = address - 0xd000;
                rf5c68.rf5c68_w(offset, value);
            }
            else if (address >= 0xe000 && address <= 0xffff)
            {
                int offset = address - 0xe000;
                Memory.audioram[offset] = value;
            }
        }
        public static byte ZReadHardware_system18(ushort address)
        {
            byte result = 0;
            address &= 0xff;
            if (address >= 0x80 && address <= 0x8f)
            {
                int offset = (address - 0x80) & 3;
                if (offset == 0)
                {
                    result = YM2612.FF2612[0].ym2612_read(0);
                }
            }
            else if (address >= 0x90 && address <= 0x9f)
            {
                int offset = (address - 0x90) & 3;
                if (offset == 0)
                {
                    result = YM2612.FF2612[1].ym2612_read(0);
                }
            }
            else if (address == 0xc0)
            {
                result = (byte)Sound.soundlatch_r();
            }
            return result;
        }
        public static void ZWriteHardware_system18(ushort address, byte value)
        {
            address &= 0xff;
            if (address >= 0x80 && address <= 0x8f)
            {
                int offset = (address - 0x80) & 3;
                YM2612.FF2612[0].ym2612_write(offset, value);
            }
            else if (address >= 0x90 && address <= 0x9f)
            {
                int offset = (address - 0x90) & 3;
                YM2612.FF2612[1].ym2612_write(offset, value);
            }
            else if (address == 0xa0)
            {
                soundbank_w(value);
            }
            else if (address == 0xc0)
            {
                mcu_data_w(value);
            }
        }
        public static byte IReadOp_system18(ushort address)
        {
            byte result = 0;
            address &= 0xfff;
            if (address >= 0x0000 && address <= 0x0fff)
            {
                result = mcurom[address];
            }
            return result;
        }
        public static byte IReadMemory_system18(ushort address)
        {
            byte result = 0;
            address &= 0xfff;
            if (address >= 0x0000 && address <= 0x0fff)
            {
                result = mcurom[address];
            }
            return result;
        }
        public static byte IReadData_system18(ushort address)
        {
            byte result = 0;
            address &= 0x1f;
            if (address >= 0x00 && address <= 0x1f)
            {
                result = (byte)memory_mapper_r(address, 0xff);
            }
            return result;
        }
        public static void IWriteData_system18(ushort address, byte data)
        {
            address &= 0x1f;
            memory_mapper_w(address, data);
        }
        public static byte IReadIO_system18(ushort address)
        {
            return 0;
        }
        public static void IWriteIO_system18(ushort address, byte data)
        {

        }
    }
}
