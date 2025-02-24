﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ui;
using cpu.m68000;
using cpu.z80;

namespace mame
{
    public partial class CPS
    {
        public static short short0, short1, short2;
        public static sbyte sbyte0, sbyte3;
        public static short short0_old, short1_old, short2_old;
        public static sbyte sbyte0_old, sbyte3_old;
        public static int iMStatus = 0;
        public static sbyte MReadOpByte_cps1(int address)
        {
            address &= 0xffffff;
            sbyte result = 0;
            if (address <= 0x3fffff)
            {
                if (address < Memory.mainrom.Length)
                {
                    result = (sbyte)(Memory.mainrom[address]);
                }
                else
                {
                    result = 0;
                }
            }
            /*else if (address >= 0x900000 && address <= 0x92ffff)
            {
                result = (sbyte)gfxram[(address & 0x3ffff)];
            }
            else if (address >= 0xff0000 && address <= 0xffffff)
            {
                result = (sbyte)Memory.mainram[address & 0xffff];
            }*/
            return result;
        }
        public static sbyte MReadByte_cps1(int address)
        {
            address &= 0xffffff;
            sbyte result = 0;
            if (address <= 0x3fffff)
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
            else if (address >= 0x800000 && address <= 0x800007)
            {
                if (address == 0x800000)
                {
                    result = (sbyte)(short1 >> 8);
                }
                else if (address == 0x800001)
                {
                    result = (sbyte)(short1);
                }
                else
                {
                    result = -1;
                }
            }
            else if (address >= 0x800018 && address <= 0x80001f)
            {
                int offset = (address - 0x800018) / 2;
                result = (sbyte)cps1_dsw_r(offset);
            }
            else if (address >= 0x800020 && address <= 0x800021)
            {
                result = 0;
            }
            else if (address >= 0x800140 && address <= 0x80017f)
            {
                int offset = (address - 0x800140) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(cps1_cps_b_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)cps1_cps_b_r(offset);
                }
            }
            else if (address >= 0x900000 && address <= 0x92ffff)
            {
                result = (sbyte)gfxram[(address & 0x3ffff)];
            }
            else if (address >= 0xff0000 && address <= 0xffffff)
            {
                result = (sbyte)Memory.mainram[address & 0xffff];
            }
            return result;
        }
        public static short MReadOpWord_cps1(int address)
        {
            address &= 0xffffff;
            short result = 0;
            if (address <= 0x3fffff)
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
            else if (address >= 0x900000 && address + 1 <= 0x92ffff)
            {
                result = (short)(gfxram[(address & 0x3ffff)] * 0x100 + gfxram[(address & 0x3ffff) + 1]);
            }
            else if (address >= 0xff0000 && address + 1 <= 0xffffff)
            {
                result = (short)(Memory.mainram[(address & 0xffff)] * 0x100 + Memory.mainram[(address & 0xffff) + 1]);
            }
            return result;
        }
        public static short MReadWord_cps1(int address)
        {
            address &= 0xffffff;
            short result = 0;
            if (address <= 0x3fffff)
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
            else if (address >= 0x800000 && address + 1 <= 0x800007)
            {
                result = short1;// input_port_4_word_r
            }
            else if (address >= 0x800018 && address + 1 <= 0x80001f)
            {
                int offset = (address - 0x800018) / 2;
                result = (short)(((byte)(cps1_dsw_r(offset)) << 8) | (byte)cps1_dsw_r(offset));
            }
            else if (address >= 0x800020 && address + 1 <= 0x800021)
            {
                result = 0;
            }
            else if (address >= 0x800140 && address + 1 <= 0x80017f)
            {
                int offset = (address - 0x800140) / 2;
                result = (short)cps1_cps_b_r(offset);
            }
            else if (address >= 0x900000 && address + 1 <= 0x92ffff)
            {
                result = (short)(gfxram[(address & 0x3ffff)] * 0x100 + gfxram[(address & 0x3ffff) + 1]);
            }
            else if (address >= 0xff0000 && address + 1 <= 0xffffff)
            {
                result = (short)(Memory.mainram[(address & 0xffff)] * 0x100 + Memory.mainram[(address & 0xffff) + 1]);
            }
            return result;
        }
        public static int MReadOpLong_cps1(int address)
        {
            int result = 0;
            result = (int)((ushort)MReadOpWord_cps1(address) * 0x10000 + (ushort)MReadOpWord_cps1(address + 2));
            return result;
        }
        public static int MReadLong_cps1(int address)
        {
            int result = 0;
            result = (int)((ushort)MReadWord_cps1(address) * 0x10000 + (ushort)MReadWord_cps1(address + 2));
            return result;
        }
        public static void MWriteByte_cps1(int address, sbyte value)
        {
            address &= 0xffffff;
            if (address >= 0x800030 && address <= 0x800037)
            {
                if (address % 2 == 0)
                {
                    cps1_coinctrl_w((ushort)(value * 0x100));
                }
                else
                {
                    return;
                }
            }
            else if (address >= 0x800100 && address <= 0x80013f)
            {
                return;
            }
            else if (address >= 0x800140 && address <= 0x80017f)
            {
                return;
            }
            else if (address >= 0x800180 && address <= 0x800187)
            {
                Sound.soundlatch_w((ushort)value);
            }
            else if (address >= 0x800188 && address <= 0x80018f)
            {
                Sound.soundlatch2_w((ushort)value);
            }
            else if (address >= 0x900000 && address <= 0x92ffff)
            {
                gfxram[(address & 0x3ffff)] = (byte)(value);
                cps1_gfxram_w((address & 0x3ffff) / 2);
            }
            else if (address >= 0xff0000 && address <= 0xffffff)
            {
                Memory.mainram[(address & 0xffff)] = (byte)(value);
            }
            else
            {
                int i1 = 1;
            }
        }
        public static void MWriteWord_cps1(int address, short value)
        {
            address &= 0xffffff;
            if (address >= 0x800030 && address + 1 <= 0x800037)
            {
                return;
            }
            else if (address >= 0x800100 && address + 1 <= 0x80013f)
            {
                int offset = (address - 0x800100) / 2;
                cps1_cps_a_w(offset, (ushort)value);
            }
            else if (address >= 0x800140 && address + 1 <= 0x80017f)
            {
                int offset = (address - 0x800140) / 2;
                cps1_cps_b_w(offset, (ushort)value);
            }
            else if (address >= 0x800180 && address + 1 <= 0x800187)
            {
                Sound.soundlatch_w((ushort)value);
            }
            else if (address >= 0x800188 && address + 1 <= 0x80018f)
            {
                Sound.soundlatch2_w((ushort)value);
            }
            else if (address >= 0x900000 && address + 1 <= 0x92ffff)
            {
                gfxram[(address & 0x3ffff)] = (byte)(value >> 8);
                gfxram[(address & 0x3ffff) + 1] = (byte)value;
                cps1_gfxram_w((address & 0x3ffff) / 2);
            }
            else if (address >= 0xff0000 && address + 1 <= 0xffffff)
            {
                Memory.mainram[(address & 0xffff)] = (byte)(value >> 8);
                Memory.mainram[(address & 0xffff) + 1] = (byte)(value);
            }
            else
            {
                int i1 = 1;
            }
        }
        public static void MWriteLong_cps1(int address, int value)
        {
            MWriteWord_cps1(address, (short)(value >> 16));
            MWriteWord_cps1(address + 2, (short)value);
        }
        public static byte ZReadOp_cps1(ushort address)
        {
            byte result = 0;
            if (address <= 0x7fff)
            {
                result = Memory.audiorom[address & 0x7fff];
            }
            else
            {
                result = 0;
            }
            return result;
        }
        public static byte ZReadMemory_cps1(ushort address)
        {
            byte result = 0;
            if (address < 0x8000)
            {
                result = Memory.audiorom[address & 0x7fff];
            }
            else if (address >= 0x8000 && address <= 0xbfff)
            {
                result = Memory.audiorom[basebanksnd + (address & 0x3fff)];
            }
            else if (address >= 0xd000 && address <= 0xd7ff)
            {
                result = Memory.audioram[address & 0x7ff];
            }
            else if (address == 0xf001)
            {
                result = YM2151.ym2151_status_port_0_r();
            }
            else if (address == 0xf002)
            {
                result = OKI6295.okim6295_status_0_r();
            }
            else if (address == 0xf008)
            {
                result = (byte)Sound.soundlatch_r();
            }
            else if (address == 0xf00a)
            {
                result = (byte)Sound.soundlatch2_r();
            }
            else
            {
                result = 0;
            }
            return result;
        }
        public static void ZWriteMemory_cps1(ushort address, byte value)
        {
            if (address >= 0xd000 && address <= 0xd7ff)
            {
                Memory.audioram[address & 0x7ff] = value;
            }
            else if (address == 0xf000)
            {
                YM2151.ym2151_register_port_0_w(value);
            }
            else if (address == 0xf001)
            {
                YM2151.ym2151_data_port_0_w(value);
            }
            else if (address == 0xf002)
            {
                OKI6295.okim6295_data_0_w(value);
            }
            else if (address == 0xf004)
            {
                cps1_snd_bankswitch_w(value);
            }
            else if (address == 0xf006)
            {
                cps1_oki_pin7_w(value);
            }
            else
            {

            }
        }
        public static sbyte MReadOpByte_qsound(int address)
        {
            address &= 0xffffff;
            sbyte result = 0;
            if (address <= 0x3fffff)
            {
                if (address < Memory.mainrom.Length)
                {
                    result = (sbyte)(Memory.mainrom[address]);
                }
                else
                {
                    result = 0;
                }
            }
            else if (address >= 0x900000 && address <= 0x92ffff)
            {
                result = (sbyte)gfxram[(address & 0x3ffff)];
            }
            return result;
        }
        public static sbyte MReadByte_qsound(int address)
        {
            address &= 0xffffff;
            sbyte result = 0;
            if (address <= 0x3fffff)
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
            else if (address >= 0x800000 && address <= 0x800007)
            {
                if (address == 0x800000)//IN1
                {
                    result = (sbyte)(short1 >> 8);
                }
                else if (address == 0x800001)
                {
                    result = (sbyte)(short1);
                }
                else
                {
                    result = -1;
                }
            }
            else if (address >= 0x800018 && address <= 0x80001f)
            {
                int offset = (address - 0x800018) / 2;
                result = (sbyte)cps1_dsw_r(offset);
            }
            else if (address >= 0x800140 && address <= 0x80017f)
            {
                int offset = (address - 0x800140) / 2;
                result = (sbyte)cps1_cps_b_r(offset);
            }
            else if (address >= 0x900000 && address <= 0x92ffff)
            {
                int offset = address - 0x900000;
                result = (sbyte)gfxram[offset];
            }
            else if (address >= 0xf00000 && address <= 0xf0ffff)
            {
                int offset = (address - 0xf00000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(qsound_rom_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)qsound_rom_r(offset);
                }
            }
            else if (address >= 0xf18000 && address <= 0xf19fff)
            {
                int offset = (address - 0xf18000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(qsound_sharedram1_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)qsound_sharedram1_r(offset);
                }
            }
            else if (address >= 0xf1c000 && address <= 0xf1c001)
            {
                result = (sbyte)short2;
            }
            else if (address >= 0xf1c002 && address <= 0xf1c003)
            {
                result = sbyte3;
            }
            else if (address >= 0xf1c006 && address <= 0xf1c007)
            {
                result = (sbyte)cps1_eeprom_port_r();
            }
            else if (address >= 0xf1e000 && address <= 0xf1ffff)
            {
                int offset = (address - 0xf1e000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(qsound_sharedram2_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)qsound_sharedram2_r(offset);
                }
            }
            else if (address >= 0xff0000 && address <= 0xffffff)
            {
                int offset = address - 0xff0000;
                result = (sbyte)Memory.mainram[offset];
            }
            return result;
        }
        public static short MReadOpWord_qsound(int address)
        {
            address &= 0xffffff;
            short result = 0;
            if (address <= 0x3fffff)
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
            else if (address >= 0x900000 && address + 1 <= 0x92ffff)
            {
                int offset = address - 0x900000;
                result = (short)(gfxram[offset] * 0x100 + gfxram[offset + 1]);
            }
            else if (address >= 0xff0000 && address + 1 <= 0xffffff)
            {
                int offset = address - 0xff0000;
                result = (short)(Memory.mainram[offset] * 0x100 + Memory.mainram[offset + 1]);
            }
            return result;
        }
        public static short MReadWord_qsound(int address)
        {
            address &= 0xffffff;
            short result = 0;
            if (address <= 0x3fffff)
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
            else if (address >= 0x800000 && address + 1 <= 0x800007)
            {
                result = short1;//input_port_4_word_r
            }
            else if (address >= 0x800018 && address + 1 <= 0x80001f)
            {
                int offset = (address - 0x800018) / 2;
                result = cps1_dsw_r(offset);
            }
            else if (address >= 0x800140 && address + 1 <= 0x80017f)
            {
                int offset = (address - 0x800140) / 2;
                result = (short)cps1_cps_b_r(offset);
            }
            else if (address >= 0x900000 && address + 1 <= 0x92ffff)
            {
                int offset = address - 0x900000;
                result = (short)(gfxram[offset] * 0x100 + gfxram[offset + 1]);
            }
            else if (address >= 0xf00000 && address + 1 <= 0xf0ffff)
            {
                int offset = (address - 0xf00000) / 2;
                result = qsound_rom_r(offset);
            }
            else if (address >= 0xf18000 && address + 1 <= 0xf19fff)
            {
                int offset = (address - 0xf18000) / 2;
                result = qsound_sharedram1_r(offset);
            }
            else if (address >= 0xf1c000 && address + 1 <= 0xf1c001)
            {
                result = (short)((int)short2 & 0xff);
            }
            else if (address >= 0xf1c002 && address + 1 <= 0xf1c003)
            {
                result = (short)((int)sbyte3 & 0xff);
            }
            else if (address >= 0xf1c006 && address + 1 <= 0xf1c007)
            {
                result = (short)cps1_eeprom_port_r();
            }
            else if (address >= 0xf1e000 && address + 1 <= 0xf1ffff)
            {
                int offset = (address - 0xf1e000) / 2;
                result = qsound_sharedram2_r(offset);
            }
            else if (address >= 0xff0000 && address + 1 <= 0xffffff)
            {
                int offset = address - 0xff0000;
                result = (short)(Memory.mainram[offset] * 0x100 + Memory.mainram[offset + 1]);
            }
            return result;
        }
        public static int MReadOpLong_qsound(int address)
        {
            int result = 0;
            result = (int)((ushort)MReadOpWord_qsound(address) * 0x10000 + (ushort)MReadOpWord_qsound(address + 2));
            return result;
        }
        public static int MReadLong_qsound(int address)
        {
            int result = 0;
            result = (int)((ushort)MReadWord_qsound(address) * 0x10000 + (ushort)MReadWord_qsound(address + 2));
            return result;
        }
        public static void MWriteByte_qsound(int address, sbyte value)
        {
            address &= 0xffffff;
            if (address >= 0x800030 && address <= 0x800037)
            {
                if (address % 2 == 0)
                {
                    cps1_coinctrl_w((ushort)(value * 0x100));
                }
                else
                {
                    int i11 = 1;
                }
            }
            else if (address >= 0x800100 && address <= 0x80013f)
            {
                int i11 = 1;//cps1_cps_a_w
            }
            else if (address >= 0x800140 && address <= 0x80017f)
            {
                int i11 = 1;//cps1_cps_b_w
            }
            else if (address >= 0x900000 && address <= 0x92ffff)
            {
                int offset = address - 0x900000;
                gfxram[offset] = (byte)(value);
                cps1_gfxram_w(offset / 2);
            }
            else if (address >= 0xf18000 && address <= 0xf19fff)
            {
                int offset = (address - 0xf18000) / 2;
                if ((address & 1) == 1)
                {
                    qsound_sharedram1_w(offset, (byte)value);
                }
                else
                {
                    int i1 = 1;
                }
            }
            else if (address >= 0xf1c004 && address <= 0xf1c005)
            {
                int i11 = 1;//cpsq_coinctrl2_w
            }
            else if (address >= 0xf1c006 && address <= 0xf1c007)
            {
                if ((address & 1) == 1)
                {
                    cps1_eeprom_port_w(value);
                }
            }
            else if (address >= 0xf1e000 && address <= 0xf1ffff)
            {
                int offset = (address - 0xf1e000) / 2;
                if ((address & 1) == 1)
                {
                    qsound_sharedram2_w(offset, (byte)value);
                }
                else
                {
                    int i1 = 1;
                }
            }
            else if (address >= 0xff0000 && address <= 0xffffff)
            {
                int offset = address - 0xff0000;
                Memory.mainram[offset] = (byte)(value);
            }
            else
            {
                int i11 = 1;
            }
        }
        public static void MWriteWord_qsound(int address, short value)
        {
            address &= 0xffffff;
            if (address >= 0x800030 && address + 1 <= 0x800037)
            {
                if (address % 2 == 0)
                {
                    cps1_coinctrl_w((ushort)(value * 0x100));
                }
                else
                {
                    int i11 = 1;
                }
            }
            else if (address >= 0x800100 && address + 1 <= 0x80013f)
            {
                cps1_cps_a_w((address & 0x3f) / 2, (ushort)value);
            }
            else if (address >= 0x800140 && address + 1 <= 0x80017f)
            {
                cps1_cps_b_w((address & 0x3f) / 2, (ushort)value);
            }
            else if (address >= 0x900000 && address + 1 <= 0x92ffff)
            {
                gfxram[(address & 0x3ffff)] = (byte)(value >> 8);
                gfxram[(address & 0x3ffff) + 1] = (byte)value;
                cps1_gfxram_w((address & 0x3ffff) / 2);
            }
            else if (address >= 0xf18000 && address + 1 <= 0xf19fff)
            {
                qsound_sharedram1_w((address - 0xf18000) >> 1, (byte)value);
            }
            else if (address >= 0xf1c004 && address + 1 <= 0xf1c005)
            {
                cpsq_coinctrl2_w((ushort)value);
            }
            else if (address >= 0xf1c006 && address + 1 <= 0xf1c007)
            {
                cps1_eeprom_port_w(value);
            }
            else if (address >= 0xf1e000 && address + 1 <= 0xf1ffff)
            {
                qsound_sharedram2_w((address - 0xf1e000) >> 1, (byte)value);
            }
            else if (address >= 0xff0000 && address + 1 <= 0xffffff)
            {
                Memory.mainram[(address & 0xffff)] = (byte)(value >> 8);
                Memory.mainram[(address & 0xffff) + 1] = (byte)(value);
            }
            else
            {
                int i11 = 1;
            }
        }
        public static void MWriteLong_qsound(int address, int value)
        {
            MWriteWord_qsound(address, (short)(value >> 16));
            MWriteWord_qsound(address + 2, (short)value);
        }
        public static sbyte MReadOpByte_cps2(int address)
        {
            address &= 0xffffff;
            sbyte result = 0;
            if (address <= 0x3fffff)
            {
                if (address < mainromop.Length)
                {
                    result = (sbyte)(mainromop[address]);
                }
                else
                {
                    result = 0;
                }
            }
            return result;
        }
        public static sbyte MReadPcrelByte_cps2(int address)
        {
            address &= 0xffffff;
            sbyte result = 0;
            if (address <= 0x3fffff)
            {
                if (address < mainromop.Length)
                {
                    result = (sbyte)mainromop[address];
                }
                else
                {
                    result = 0;
                }
            }
            else
            {
                result = MReadByte_cps2(address);
            }
            return result;
        }
        public static sbyte MReadByte_cps2(int address)
        {
            address &= 0xffffff;
            sbyte result = 0;
            if (address <= 0x3fffff)
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
            else if (address >= 0x400000 && address <= 0x40000b)
            {
                int offset = (address - 0x400000) / 2;
                result = (sbyte)cps2_output[offset];
            }
            else if (address >= 0x618000 && address <= 0x619fff)
            {
                int offset = (address - 0x618000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(qsound_sharedram1_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)qsound_sharedram1_r(offset);
                }
            }
            else if (address >= 0x662000 && address <= 0x662001)
            {
                result = 0;
            }
            else if (address >= 0x662008 && address <= 0x662009)
            {
                result = 0;
            }
            else if (address >= 0x662020 && address <= 0x662021)
            {
                result = 0;
            }
            else if (address >= 0x660000 && address <= 0x663fff)
            {
                result = 0;
            }
            else if (address >= 0x664000 && address <= 0x664001)
            {
                result = 0;
            }
            else if (address >= 0x708000 && address <= 0x709fff)
            {
                int offset = (address - 0x708000) / 2;
                result = (sbyte)cps2_objram2_r(offset);
            }
            else if (address >= 0x70a000 && address <= 0x70bfff)
            {
                int offset = (address - 0x70a000) / 2;
                result = (sbyte)cps2_objram2_r(offset);
            }
            else if (address >= 0x70c000 && address <= 0x70dfff)
            {
                int offset = (address - 0x70c000) / 2;
                result = (sbyte)cps2_objram2_r(offset);
            }
            else if (address >= 0x70e000 && address <= 0x70ffff)
            {
                int offset = (address - 0x70e000) / 2;
                result = (sbyte)cps2_objram2_r(offset);
            }
            else if (address >= 0x800140 && address <= 0x80017f)
            {
                int offset = (address - 0x800140) / 2;
                result = (sbyte)cps1_cps_b_r(offset);
            }
            else if (address >= 0x804000 && address <= 0x804001)
            {
                if (address % 2 == 0)
                {
                    result = (sbyte)(short0 >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)short0;
                }
            }
            else if (address >= 0x804010 && address <= 0x804011)
            {
                if (address % 2 == 0)
                {
                    result = (sbyte)(short1 >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)short1;
                }
            }
            else if (address >= 0x804020 && address <= 0x804021)
            {
                if (address % 2 == 0)
                {
                    result = (sbyte)(short2 >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)(short2 & (Eeprom.eeprom_bit_r() - 2));
                }
            }
            else if (address >= 0x804030 && address <= 0x804031)
            {
                if (address % 2 == 0)
                {
                    result = (sbyte)(cps2_qsound_volume_r() >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)cps2_qsound_volume_r();
                }
            }
            else if (address >= 0x8040b0 && address <= 0x8040b3)
            {
                if (address % 2 == 0)
                {
                    result = (sbyte)(kludge_r() >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)kludge_r();
                }
            }
            else if (address >= 0x804140 && address <= 0x80417f)
            {
                int offset = (address - 0x804140) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(cps1_cps_b_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)cps1_cps_b_r(offset);
                }
            }
            else if (address >= 0x900000 && address <= 0x92ffff)
            {
                int offset = address - 0x900000;
                result = (sbyte)gfxram[offset];
            }
            else if (address >= 0xff0000 && address <= 0xffffff)
            {
                result = (sbyte)Memory.mainram[address & 0xffff];
            }
            return result;
        }
        public static short MReadOpWord_cps2(int address)
        {
            address &= 0xffffff;
            short result = 0;
            if (address <= 0x3fffff)
            {
                if (address + 1 < mainromop.Length)
                {
                    result = (short)(mainromop[address] * 0x100 + mainromop[address + 1]);
                }
                else
                {
                    result = 0;
                }
            }
            return result;
        }
        public static short MReadPcrelWord_cps2(int address)
        {
            address &= 0xffffff;
            short result = 0;
            if (address <= 0x3fffff)
            {
                if (address + 1 < mainromop.Length)
                {
                    result = (short)(mainromop[address] * 0x100 + mainromop[address + 1]);
                }
                else
                {
                    result = 0;
                }
            }
            else
            {
                result = MReadWord_cps2(address);
            }
            return result;
        }
        public static short MReadWord_cps2(int address)
        {
            address &= 0xffffff;
            short result = 0;
            if (address <= 0x3fffff)
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
            else if (address >= 0x400000 && address <= 0x40000b)
            {
                int offset = (address - 0x400000) / 2;
                result = (short)cps2_output[offset];
            }
            else if (address >= 0x618000 && address <= 0x619fff)
            {
                int offset = (address - 0x618000) / 2;
                result = qsound_sharedram1_r(offset);
            }
            else if (address >= 0x662000 && address <= 0x662001)
            {
                result = 0;
            }
            else if (address >= 0x662008 && address <= 0x662009)
            {
                result = 0;
            }
            else if (address >= 0x662020 && address <= 0x662021)
            {
                result = 0;
            }
            else if (address >= 0x660000 && address <= 0x663fff)
            {
                result = 0;
            }
            else if (address >= 0x664000 && address <= 0x664001)
            {
                result = 0;
            }
            else if (address >= 0x708000 && address <= 0x709fff)
            {
                int offset = (address - 0x708000) / 2;
                result = cps2_objram2_r(offset);
            }
            else if (address >= 0x70a000 && address <= 0x70bfff)
            {
                int offset = (address - 0x70a000) / 2;
                result = cps2_objram2_r(offset);
            }
            else if (address >= 0x70c000 && address <= 0x70dfff)
            {
                int offset = (address - 0x70c000) / 2;
                result = cps2_objram2_r(offset);
            }
            else if (address >= 0x70e000 && address <= 0x70ffff)
            {
                int offset = (address - 0x70e000) / 2;
                result = cps2_objram2_r(offset);
            }
            else if (address >= 0x800140 && address <= 0x80017f)
            {
                int offset = (address - 0x800140) / 2;
                result = (short)cps1_cps_b_r(offset);
            }
            else if (address >= 0x804000 && address <= 0x804001)
            {
                result = short0;
            }
            else if (address >= 0x804010 && address <= 0x804011)
            {
                result = short1;
            }
            else if (address >= 0x804020 && address <= 0x804021)
            {
                result = (short)(short2 & (Eeprom.eeprom_bit_r() - 2));
            }
            else if (address >= 0x804030 && address <= 0x804031)
            {
                result = cps2_qsound_volume_r();
            }
            else if (address >= 0x8040b0 && address <= 0x8040b3)
            {
                result = kludge_r();
            }
            else if (address >= 0x804140 && address <= 0x80417f)
            {
                int offset = (address - 0x804140) / 2;
                result = (short)cps1_cps_b_r(offset);
            }
            else if (address >= 0x900000 && address + 1 <= 0x92ffff)
            {
                int offset = address - 0x900000;
                result = (short)(gfxram[offset] * 0x100 + gfxram[offset + 1]);
            }
            else if (address >= 0xff0000 && address + 1 <= 0xffffff)
            {
                int offset = address - 0xff0000;
                result = (short)(Memory.mainram[offset] * 0x100 + Memory.mainram[offset + 1]);
            }
            return result;
        }
        public static int MReadOpLong_cps2(int address)
        {
            int result = 0;
            result = (int)((ushort)MReadOpWord_cps2(address) * 0x10000 + (ushort)MReadOpWord_cps2(address + 2));
            return result;
        }
        public static int MReadPcrelLong_cps2(int address)
        {
            int result = 0;
            result = (int)((ushort)MReadPcrelWord_cps2(address) * 0x10000 + (ushort)MReadPcrelWord_cps2(address + 2));
            return result;
        }
        public static int MReadLong_cps2(int address)
        {
            int result = 0;
            result = (int)((ushort)MReadWord_cps2(address) * 0x10000 + (ushort)MReadWord_cps2(address + 2));
            return result;
        }
        public static void MWriteByte_cps2(int address, sbyte value)
        {
            address &= 0xffffff;
            if (address <= 0x3fffff)
            {
                int i11 = 1;
            }
            if (address >= 0x400000 && address <= 0x40000b)
            {
                int offset = (address - 0x400000) / 2;
                cps2_output[offset] = (ushort)value;
            }
            else if (address >= 0x618000 && address <= 0x619fff)
            {
                int offset = (address - 0x618000) / 2;
                qsound_sharedram1_w(offset, (byte)value);
            }
            else if (address >= 0x662000 && address <= 0x662001)
            {
                int i11 = 1;
            }
            else if (address >= 0x662008 && address <= 0x662009)
            {
                int i11 = 1;
            }
            else if (address >= 0x662020 && address <= 0x662021)
            {
                int i11 = 1;
            }
            else if (address >= 0x660000 && address <= 0x663fff)
            {
                int i11 = 1;
            }
            else if (address >= 0x664000 && address <= 0x664001)
            {
                int i11 = 1;
            }
            else if (address >= 0x700000 && address <= 0x701fff)
            {
                int offset = (address - 0x700000) / 2;
                cps2_objram1_w(offset, (ushort)value);
            }
            else if (address >= 0x708000 && address <= 0x709fff)
            {
                int offset = (address - 0x708000) / 2;
                cps2_objram2_w(offset, (ushort)value);
            }
            else if (address >= 0x70a000 && address <= 0x70bfff)
            {
                int i1 = 1;
                //int offset = (add - 0x70a000) / 2;
                //cps2_objram2_w(offset, (ushort)value);
            }
            else if (address >= 0x70c000 && address <= 0x70dfff)
            {
                int i1 = 1;
                //int offset = (add - 0x70c000) / 2;
                //cps2_objram2_w(offset, (ushort)value);
            }
            else if (address >= 0x70e000 && address <= 0x70ffff)
            {
                int i1 = 1;
                //int offset = (add - 0x70e000) / 2;
                //cps2_objram2_w(offset, (ushort)value);
            }
            else if (address >= 0x800100 && address <= 0x80013f)
            {
                int i11 = 1;//cps1_cps_a_w
            }
            else if (address >= 0x800140 && address <= 0x80017f)
            {
                int i11 = 1;//cps1_cps_b_w
            }
            else if (address >= 0x804040 && address <= 0x804041)
            {
                if (address == 0x804040)
                {
                    cps2_eeprom_port_bh(value);
                }
                else if (address == 0x804041)
                {
                    cps2_eeprom_port_bl(value);
                }
            }
            else if (address >= 0x8040a0 && address <= 0x8040a1)
            {
                int i11 = 1;//nop
            }
            else if (address >= 0x8040e0 && address <= 0x8040e1)
            {
                cps2_objram_bank_w(value);
            }
            else if (address >= 0x804100 && address <= 0x80413f)
            {
                int i11 = 1;//cps1_cps_a_w
            }
            else if (address >= 0x804140 && address <= 0x80417f)
            {
                int i11 = 1;//cps1_cps_b_w
            }
            else if (address >= 0x900000 && address <= 0x92ffff)
            {
                int offset = address - 0x900000;
                gfxram[offset] = (byte)(value);
                cps1_gfxram_w(offset / 2);
            }
            else if (address >= 0xff0000 && address <= 0xffffff)
            {
                int offset = address - 0xff0000;
                Memory.mainram[offset] = (byte)(value);
            }
            else
            {
                int i11 = 1;
            }
        }
        public static void MWriteWord_cps2(int address, short value)
        {
            address &= 0xffffff;
            if (address <= 0x3fffff)
            {
                int i11 = 1;
            }
            if (address >= 0x400000 && address + 1 <= 0x40000b)
            {
                int offset = (address - 0x400000) / 2;
                cps2_output[offset] = (ushort)value;
            }
            else if (address >= 0x618000 && address <= 0x619fff)
            {
                int offset = (address - 0x618000) / 2;
                qsound_sharedram1_w(offset, (byte)value);
            }
            else if (address >= 0x662000 && address <= 0x662001)
            {
                int i11 = 1;
            }
            else if (address >= 0x662008 && address <= 0x662009)
            {
                int i11 = 1;
            }
            else if (address >= 0x662020 && address <= 0x662021)
            {
                int i11 = 1;
            }
            else if (address >= 0x660000 && address <= 0x663fff)
            {
                int i11 = 1;
            }
            else if (address >= 0x664000 && address <= 0x664001)
            {
                int i11 = 1;
            }
            else if (address >= 0x700000 && address <= 0x701fff)
            {
                int offset = (address - 0x700000) / 2;
                cps2_objram1_w(offset, (ushort)value);
            }
            else if (address >= 0x708000 && address <= 0x709fff)
            {
                int offset = (address - 0x708000) / 2;
                cps2_objram2_w(offset, (ushort)value);
            }
            else if (address >= 0x70a000 && address <= 0x70bfff)
            {
                int offset = (address - 0x70a000) / 2;
                cps2_objram2_w(offset, (ushort)value);
            }
            else if (address >= 0x70c000 && address <= 0x70dfff)
            {
                int offset = (address - 0x70c000) / 2;
                cps2_objram2_w(offset, (ushort)value);
            }
            else if (address >= 0x70e000 && address <= 0x70ffff)
            {
                int offset = (address - 0x70e000) / 2;
                cps2_objram2_w(offset, (ushort)value);
            }
            else if (address >= 0x800100 && address <= 0x80013f)
            {
                int offset = (address - 0x800100) / 2;
                cps1_cps_a_w(offset, (ushort)value);
            }
            else if (address >= 0x800140 && address + 1 <= 0x80017f)
            {
                int offset = (address - 0x800140) / 2;
                cps1_cps_b_w(offset, (ushort)value);
            }
            else if (address >= 0x804040 && address <= 0x804041)
            {
                cps2_eeprom_port_w(value);
            }
            else if (address >= 0x8040a0 && address <= 0x8040a1)
            {
                int i11 = 1;//nop
            }
            else if (address >= 0x8040e0 && address <= 0x8040e1)
            {
                cps2_objram_bank_w(value);
            }
            else if (address >= 0x804100 && address + 1 <= 0x80413f)
            {
                int offset = (address - 0x804100) / 2;
                cps1_cps_a_w(offset, (ushort)value);
            }
            else if (address >= 0x804140 && address <= 0x80417f)
            {
                int offset = (address - 0x804140) / 2;
                cps1_cps_b_w(offset, (ushort)value);
            }
            else if (address >= 0x900000 && address + 1 <= 0x92ffff)
            {
                int offset = address - 0x900000;
                gfxram[offset] = (byte)(value >> 8);
                gfxram[offset + 1] = (byte)value;
                cps1_gfxram_w(offset / 2);
            }
            else if (address >= 0xff0000 && address + 1 <= 0xffffff)
            {
                int offset = address - 0xff0000;
                Memory.mainram[offset] = (byte)(value >> 8);
                Memory.mainram[offset + 1] = (byte)value;
            }
            else
            {
                int i11 = 1;
            }
        }
        public static void MWriteLong_cps2(int address, int value)
        {
            MWriteWord_cps2(address, (short)(value >> 16));
            MWriteWord_cps2(address + 2, (short)value);
        }
        public static sbyte MReadOpByte_cps2turbo(int address)
        {
            address &= 0xffffff;
            sbyte result = 0;
            if (address <= 0x5fffff)
            {
                if (address < Memory.mainrom.Length)
                {
                    result = (sbyte)(Memory.mainrom[address]);
                }
                else
                {
                    result = 0;
                }
            }
            return result;
        }
        public static sbyte MReadByte_cps2turbo(int address)
        {
            address &= 0xffffff;
            sbyte result = 0;
            if (address <= 0x5fffff)
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
            else if (address >= 0x618000 && address <= 0x619fff)
            {
                int offset = (address - 0x618000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(qsound_sharedram1_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)qsound_sharedram1_r(offset);
                }
            }
            else if (address >= 0x660000 && address <= 0x663fff)
            {
                result = 0;
            }
            else if (address >= 0x664000 && address <= 0x664001)
            {
                result = 0;
            }
            else if (address >= 0x665000 && address <= 0x66500b)
            {
                int offset = (address - 0x665000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(cps2_output[offset] >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)cps2_output[offset];
                }
            }
            else if (address >= 0x708000 && address <= 0x709fff)
            {
                int offset = (address - 0x708000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(cps2_objram2_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)cps2_objram2_r(offset);
                }
            }
            else if (address >= 0x70a000 && address <= 0x70bfff)
            {
                int offset = (address - 0x70a000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(cps2_objram2_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)cps2_objram2_r(offset);
                }
            }
            else if (address >= 0x70c000 && address <= 0x70dfff)
            {
                int offset = (address - 0x70c000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(cps2_objram2_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)cps2_objram2_r(offset);
                }
            }
            else if (address >= 0x70e000 && address <= 0x70ffff)
            {
                int offset = (address - 0x70e000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(cps2_objram2_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)cps2_objram2_r(offset);
                }
            }
            else if (address >= 0x800140 & address <= 0x80017f)
            {
                int offset = (address - 0x800140) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(cps1_cps_b_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)cps1_cps_b_r(offset);
                }
            }
            else if (address >= 0x804000 && address <= 0x804001)
            {
                if (address % 2 == 0)
                {
                    result = (sbyte)(short0 >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)short0;
                }
            }
            else if (address >= 0x804010 && address <= 0x804011)
            {
                if (address % 2 == 0)
                {
                    result = (sbyte)(short1 >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)short1;
                }
            }
            else if (address >= 0x804020 && address <= 0x804021)
            {
                if (address % 2 == 0)
                {
                    result = (sbyte)(short2 >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)(short2 & (Eeprom.eeprom_bit_r() - 2));
                }
            }
            else if (address >= 0x804030 && address <= 0x804031)
            {
                if (address % 2 == 0)
                {
                    result = (sbyte)(cps2_qsound_volume_r() >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)cps2_qsound_volume_r();
                }
            }
            else if (address >= 0x8040b0 && address <= 0x8040b3)
            {
                if (address % 2 == 0)
                {
                    result = (sbyte)(kludge_r() >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)kludge_r();
                }
            }
            else if (address >= 0x804140 && address <= 0x80417f)
            {
                int offset = (address - 0x804140) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(cps1_cps_b_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)cps1_cps_b_r(offset);
                }
            }
            else if (address >= 0x900000 && address <= 0x92ffff)
            {
                int offset = address - 0x900000;
                result = (sbyte)gfxram[offset];
            }
            else if (address >= 0xff0000 && address <= 0xffffef)
            {
                int offset = address - 0xff0000;
                result = (sbyte)Memory.mainram[offset];
            }
            else if (address >= 0xfffff0 && address <= 0xfffffb)
            {
                int offset = (address - 0xfffff0) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(cps2_output[offset] >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)cps2_output[offset];
                }
            }
            else if (address >= 0xfffffc && address <= 0xffffff)
            {
                int offset = address - 0xff0000;
                result = (sbyte)Memory.mainram[offset];
            }
            return result;
        }
        public static short MReadOpWord_cps2turbo(int address)
        {
            address &= 0xffffff;
            short result = 0;
            if (address <= 0x5fffff)
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
            return result;
        }
        public static short MReadWord_cps2turbo(int address)
        {
            address &= 0xffffff;
            short result = 0;
            if (address <= 0x5fffff)
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
            else if (address >= 0x618000 && address + 1 <= 0x619fff)
            {
                int offset = (address - 0x618000) / 2;
                result = qsound_sharedram1_r(offset);
            }
            else if (address >= 0x660000 && address + 1 <= 0x663fff)
            {
                result = 0;
            }
            else if (address >= 0x664000 && address + 1 <= 0x664001)
            {
                result = 0;
            }
            else if (address >= 0x665000 && address + 1 <= 0x66500b)
            {
                int offset = (address - 0x665000) / 2;
                result = (short)cps2_output[offset];
            }
            else if (address >= 0x708000 && address <= 0x709fff)
            {
                int offset = (address - 0x708000) / 2;
                result = cps2_objram2_r(offset);
            }
            else if (address >= 0x70a000 && address <= 0x70bfff)
            {
                int offset = (address - 0x70a000) / 2;
                result = cps2_objram2_r(offset);
            }
            else if (address >= 0x70c000 && address <= 0x70dfff)
            {
                int offset = (address - 0x70c000) / 2;
                result = cps2_objram2_r(offset);
            }
            else if (address >= 0x70e000 && address <= 0x70ffff)
            {
                int offset = (address - 0x70e000) / 2;
                result = cps2_objram2_r(offset);
            }
            else if (address >= 0x800140 && address <= 0x80017f)
            {
                int offset = (address - 0x800140) / 2;
                result = (short)cps1_cps_b_r(offset);
            }
            else if (address >= 0x804000 && address <= 0x804001)
            {
                result = short0;
            }
            else if (address >= 0x804010 && address <= 0x804011)
            {
                result = short1;
            }
            else if (address >= 0x804020 && address <= 0x804021)
            {
                result = (short)(short2 & (Eeprom.eeprom_bit_r() - 2));
            }
            else if (address >= 0x804030 && address <= 0x804031)
            {
                result = cps2_qsound_volume_r();
            }
            else if (address >= 0x8040b0 && address <= 0x8040b3)
            {
                result = kludge_r();
            }
            else if (address >= 0x804140 && address <= 0x80417f)
            {
                int offset = (address - 0x804140) / 2;
                result = (short)cps1_cps_b_r(offset);
            }
            else if (address >= 0x900000 && address + 1 <= 0x92ffff)
            {
                int offset = address - 0x900000;
                result = (short)(gfxram[offset] * 0x100 + gfxram[offset + 1]);
            }
            else if (address >= 0xff0000 && address + 1 <= 0xffffef)
            {
                int offset = address - 0xff0000;
                result = (short)(Memory.mainram[offset] * 0x100 + Memory.mainram[offset + 1]);
            }
            else if (address >= 0xfffff0 && address + 1 <= 0xfffffb)
            {
                int offset = (address - 0xfffff0) / 2;
                result = (short)cps2_output[offset];
            }
            else if (address >= 0xfffffc && address + 1 <= 0xffffff)
            {
                int offset = address - 0xff0000;
                result = (short)(Memory.mainram[offset] * 0x100 + Memory.mainram[offset + 1]);
            }
            return result;
        }
        public static int MReadOpLong_cps2turbo(int address)
        {
            int result = 0;
            result = (int)((ushort)MReadOpWord_cps2turbo(address) * 0x10000 + (ushort)MReadOpWord_cps2turbo(address + 2));
            return result;
        }
        public static int MReadLong_cps2turbo(int address)
        {
            int result = 0;
            result = (int)((ushort)MReadWord_cps2turbo(address) * 0x10000 + (ushort)MReadWord_cps2turbo(address + 2));
            return result;
        }
        public static void MWriteByte_cps2turbo(int address, sbyte value)
        {
            address &= 0xffffff;
            if (address >= 0x618000 && address <= 0x619fff)
            {
                int offset = (address - 0x618000) / 2;
                if (address % 2 == 1)
                {
                    qsound_sharedram1_samples_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x660000 && address <= 0x663fff)
            {
                int i1 = 1;
            }
            else if (address >= 0x664000 && address <= 0x664001)
            {
                int i1 = 1;
            }
            else if (address >= 0x665000 && address <= 0x66500b)
            {
                int offset = (address - 0x665000) / 2;
                if (address % 2 == 0)
                {
                    cps2_output[offset] = (ushort)((value << 8) | (cps2_output[offset] & 0xff));
                }
                else if (address % 2 == 1)
                {
                    cps2_output[offset] = (ushort)((cps2_output[offset] & 0xff00) | (byte)value);
                }
            }
            else if (address >= 0x700000 && address <= 0x701fff)
            {
                int offset = (address - 0x700000) / 2;
                if (address % 2 == 0)
                {
                    cps2_objram1_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    cps2_objram1_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x708000 && address <= 0x709fff)
            {
                int offset = (address - 0x708000) / 2;
                if (address % 2 == 0)
                {
                    cps2_objram2_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    cps2_objram2_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x70a000 && address <= 0x70bfff)
            {
                int offset = (address - 0x70a000) / 2;
                if (address % 2 == 0)
                {
                    cps2_objram2_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    cps2_objram2_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x70c000 && address <= 0x70dfff)
            {
                int offset = (address - 0x70c000) / 2;
                if (address % 2 == 0)
                {
                    cps2_objram2_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    cps2_objram2_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x70e000 && address <= 0x70ffff)
            {
                int offset = (address - 0x70e000) / 2;
                if (address % 2 == 0)
                {
                    cps2_objram2_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    cps2_objram2_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x800100 && address <= 0x80013f)
            {
                int offset = (address - 0x800100) / 2;
                if (address % 2 == 0)
                {
                    cps1_cps_a_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    cps1_cps_a_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x800140 && address <= 0x80017f)
            {
                int offset = (address - 0x800140) / 2;
                if (address % 2 == 0)
                {
                    cps1_cps_b_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    cps1_cps_b_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x804040 && address <= 0x804041)
            {
                cps2_eeprom_port_w(value);
            }
            else if (address >= 0x8040a0 && address <= 0x8040a1)
            {
                int i1 = 1;
            }
            else if (address >= 0x8040e0 && address <= 0x8040e1)
            {
                cps2_objram_bank_w(value);
            }
            else if (address >= 0x804100 && address <= 0x80413f)
            {
                int offset = (address - 0x804100) / 2;
                if (address % 2 == 0)
                {
                    cps1_cps_a_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    cps1_cps_a_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x804140 && address <= 0x80417f)
            {
                int offset = (address - 0x804140) / 2;
                if (address % 2 == 0)
                {
                    cps1_cps_b_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    cps1_cps_b_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x900000 && address <= 0x92ffff)
            {
                int offset = address - 0x900000;
                gfxram[offset] = (byte)(value);
                cps1_gfxram_w(offset / 2);
            }
            else if (address >= 0xff0000 && address <= 0xffffef)
            {
                int offset = address - 0xff0000;
                Memory.mainram[offset] = (byte)value;
            }
            else if (address >= 0xfffff0 && address <= 0xfffffb)
            {
                int offset = (address - 0xfffff0) / 2;
                if (address % 2 == 0)
                {
                    cps2_output[offset] = (ushort)((value << 8) | (cps2_output[offset] & 0xff));
                }
                else if (address % 2 == 1)
                {
                    cps2_output[offset] = (ushort)((cps2_output[offset] & 0xff00) | (byte)value);
                }
            }
            else if (address >= 0xfffffc && address <= 0xffffff)
            {
                int offset = address - 0xff0000;
                Memory.mainram[offset] = (byte)value;
            }
        }
        public static void MWriteWord_cps2turbo(int address, short value)
        {
            address &= 0xffffff;
            if (address >= 0x618000 && address + 1 <= 0x619fff)
            {
                int offset = (address - 0x618000) / 2;
                qsound_sharedram1_samples_w(offset, (ushort)value);
            }
            else if (address >= 0x660000 && address + 1 <= 0x663fff)
            {

            }
            else if (address >= 0x664000 && address + 1 <= 0x664001)
            {

            }
            else if (address >= 0x665000 && address + 1 <= 0x66500b)
            {
                int offset = (address - 0x665000) / 2;
                cps2_output[offset] = (ushort)value;
            }
            else if (address >= 0x700000 && address + 1 <= 0x701fff)
            {
                int offset = (address - 0x700000) / 2;
                cps2_objram1_w(offset, (ushort)value);
            }
            else if (address >= 0x708000 && address <= 0x709fff)
            {
                int offset = (address - 0x708000) / 2;
                cps2_objram2_w(offset, (ushort)value);
            }
            else if (address >= 0x70a000 && address <= 0x70bfff)
            {
                int offset = (address - 0x70a000) / 2;
                cps2_objram2_w(offset, (ushort)value);
            }
            else if (address >= 0x70c000 && address <= 0x70dfff)
            {
                int offset = (address - 0x70c000) / 2;
                cps2_objram2_w(offset, (ushort)value);
            }
            else if (address >= 0x70e000 && address <= 0x70ffff)
            {
                int offset = (address - 0x70e000) / 2;
                cps2_objram2_w(offset, (ushort)value);
            }
            else if (address >= 0x800100 && address <= 0x80013f)
            {
                int offset = (address - 0x800100) / 2;
                cps1_cps_a_w(offset, (ushort)value);
            }
            else if (address >= 0x800140 && address + 1 <= 0x80017f)
            {
                int offset = (address - 0x800140) / 2;
                cps1_cps_b_w(offset, (ushort)value);
            }
            else if (address >= 0x804040 && address <= 0x804041)
            {
                cps2_eeprom_port_w(value);
            }
            else if (address >= 0x8040a0 && address <= 0x8040a1)
            {
                int i11 = 1;//nop
            }
            else if (address >= 0x8040e0 && address <= 0x8040e1)
            {
                cps2_objram_bank_w(value);
            }
            else if (address >= 0x804100 && address + 1 <= 0x80413f)
            {
                int offset = (address - 0x804100) / 2;
                cps1_cps_a_w(offset, (ushort)value);
            }
            else if (address >= 0x804140 && address <= 0x80417f)
            {
                int offset = (address - 0x804140) / 2;
                cps1_cps_b_w(offset, (ushort)value);
            }
            else if (address >= 0x900000 && address + 1 <= 0x92ffff)
            {
                int offset = address - 0x900000;
                gfxram[offset] = (byte)(value >> 8);
                gfxram[offset + 1] = (byte)value;
                cps1_gfxram_w(offset / 2);
            }
            else if (address >= 0xff0000 && address + 1 <= 0xffffef)
            {
                int offset = address - 0xff0000;
                Memory.mainram[offset] = (byte)(value >> 8);
                Memory.mainram[offset + 1] = (byte)value;
            }
            else if (address >= 0xfffff0 && address + 1 <= 0xfffffb)
            {
                int offset = (address - 0xfffff0) / 2;
                cps2_output[offset] = (ushort)value;
            }
            else if (address >= 0xfffffc && address + 1 <= 0xffffff)
            {
                int offset = address - 0xff0000;
                Memory.mainram[offset] = (byte)(value >> 8);
                Memory.mainram[offset + 1] = (byte)value;
            }
        }
        public static void MWriteLong_cps2turbo(int address, int value)
        {
            MWriteWord_cps2turbo(address, (short)(value >> 16));
            MWriteWord_cps2turbo(address + 2, (short)value);
        }
        public static byte ZReadOp_qsound(ushort address)
        {
            byte result = 0;
            if (address <= 0x7fff)
            {
                result = audioromop[address & 0x7fff];
            }
            else
            {
                result = 0;
            }
            return result;
        }
        public static byte ZReadMemory_qsound(ushort address)
        {
            byte result = 0;
            if (address <= 0x7fff)
            {
                result = Memory.audiorom[address & 0x7fff];
            }
            else if (address >= 0x8000 && address <= 0xbfff)
            {
                result = Memory.audiorom[basebanksnd + (address & 0x3fff)];
            }
            else if (address >= 0xc000 && address <= 0xcfff)
            {
                result = qsound_sharedram1[address & 0xfff];
            }
            else if (address == 0xd007)
            {
                result = QSound.qsound_status_r();
            }
            else if (address >= 0xf000 && address <= 0xffff)
            {
                result = qsound_sharedram2[address & 0xfff];
            }
            else
            {
                result = 0;
            }
            return result;
        }
        public static void ZWriteMemory_qsound(ushort address, byte value)
        {
            if (address >= 0xc000 && address <= 0xcfff)
            {
                qsound_sharedram1[address & 0xfff] = value;
            }
            else if (address == 0xd000)
            {
                QSound.qsound_data_h_w(value);
            }
            else if (address == 0xd001)
            {
                QSound.qsound_data_l_w(value);
            }
            else if (address == 0xd002)
            {
                QSound.qsound_cmd_w(value);
            }
            else if (address == 0xd003)
            {
                qsound_banksw_w(value);
            }
            else if (address >= 0xf000 && address <= 0xffff)
            {
                qsound_sharedram2[address & 0xfff] = value;
            }
            else
            {

            }
        }
        public static byte ZReadHardware(ushort address)
        {
            return 0;
        }
        public static void ZWriteHardware(ushort address, byte value)
        {

        }
    }
}
