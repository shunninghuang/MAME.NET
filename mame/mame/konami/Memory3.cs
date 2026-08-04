using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mame
{
    public partial class Konami
    {
        public static byte KReadOp_scontra(ushort address)
        {
            byte result = 0;
            if (address >= 0x4000 && address <= 0x57ff)
            {
                int offset = address - 0x4000;
                result = Memory.mainram[offset];
            }
            else if (address >= 0x6000 && address <= 0x7fff)
            {
                int offset = address - 0x6000;
                result = bank1[basebankmain + offset];
            }
            else if (address >= 0x8000 && address <= 0xffff)
            {
                result = Memory.mainrom[address];
            }
            return result;
        }
        public static byte KReadMemory_scontra(ushort address)
        {
            byte result = 0;
            if (address == 0x1f90)
            {
                result = bytes;
            }
            else if (address == 0x1f91)
            {
                result = byte1;
            }
            else if (address == 0x1f92)
            {
                result = byte2;
            }
            else if (address == 0x1f93)
            {
                result = dsw3;
            }
            else if (address == 0x1f94)
            {
                result = dsw1;
            }
            else if (address == 0x1f95)
            {
                result = dsw2;
            }
            else if (address >= 0x0000 && address <= 0x3fff)
            {
                result = K052109_051960_r(address);
            }
            else if (address >= 0x4000 && address <= 0x57ff)
            {
                int offset = address - 0x4000;
                result = Memory.mainram[offset];
            }
            else if (address >= 0x5800 && address <= 0x5fff)
            {
                int offset = address - 0x5800;
                result = bankedram_r(offset);
            }
            else if (address >= 0x6000 && address <= 0x7fff)
            {
                int offset = address - 0x6000;
                result = bank1[basebankmain + offset];
            }
            else if (address >= 0x8000 && address <= 0xffff)
            {
                result = Memory.mainrom[address];
            }
            return result;
        }
        public static void KWriteMemory_scontra(ushort address, byte value)
        {
            if (address == 0x1f80)
            {
                scontra_bankswitch_w(value);
            }
            else if (address == 0x1f84)
            {
                Sound.soundlatch_w(value);
            }
            else if (address == 0x1f88)
            {
                thunderx_sh_irqtrigger_w();
            }
            else if (address == 0x1f8c)
            {
                Generic.watchdog_reset_w();
            }
            else if (address == 0x1f98)
            {
                thunderx_1f98_w(value);
            }
            else if (address >= 0x0000 && address <= 0x3fff)
            {
                K052109_051960_w(address, value);
            }
            else if (address >= 0x4000 && address <= 0x57ff)
            {
                int offset = address - 0x4000;
                Memory.mainram[offset] = value;
            }
            else if (address >= 0x5800 && address <= 0x5fff)
            {
                int offset = address - 0x5800;
                bankedram_w(offset, value);
            }
            else if (address >= 0x6000 && address <= 0xffff)
            {

            }
        }
        public static byte KReadMemory_thunderx(ushort address)
        {
            byte result = 0;
            if (address == 0x1f90)
            {
                result = bytes;
            }
            else if (address == 0x1f91)
            {
                result = byte1;
            }
            else if (address == 0x1f92)
            {
                result = byte2;
            }
            else if (address == 0x1f93)
            {
                result = dsw3;
            }
            else if (address == 0x1f94)
            {
                result = dsw1;
            }
            else if (address == 0x1f95)
            {
                result = dsw2;
            }
            else if (address == 0x1f98)
            {
                result = thunderx_1f98_r();
            }
            else if (address >= 0x0000 && address <= 0x3fff)
            {
                result = K052109_051960_r(address);
            }
            else if (address >= 0x4000 && address <= 0x57ff)
            {
                int offset = address - 0x4000;
                result = Memory.mainram[offset];
            }
            else if (address >= 0x5800 && address <= 0x5fff)
            {
                int offset = address - 0x5800;
                result = thunderx_bankedram_r(offset);
            }
            else if (address >= 0x6000 && address <= 0x7fff)
            {
                int offset = address - 0x6000;
                result = bank1[basebankmain + offset];
            }
            else if (address >= 0x8000 && address <= 0xffff)
            {
                result = Memory.mainrom[address];
            }
            return result;
        }
        public static void KWriteMemory_thunderx(ushort address, byte value)
        {
            if (address == 0x1f80)
            {
                thunderx_videobank_w(value);
            }
            else if (address == 0x1f84)
            {
                Sound.soundlatch_w(value);
            }
            else if (address == 0x1f88)
            {
                thunderx_sh_irqtrigger_w();
            }
            else if (address == 0x1f8c)
            {
                Generic.watchdog_reset_w();
            }
            else if (address == 0x1f98)
            {
                thunderx_1f98_w(value);
            }
            else if (address >= 0x0000 && address <= 0x3fff)
            {
                K052109_051960_w(address, value);
            }
            else if (address >= 0x4000 && address <= 0x57ff)
            {
                int offset = address - 0x4000;
                Memory.mainram[offset] = value;
            }
            else if (address >= 0x5800 && address <= 0x5fff)
            {
                int offset = address - 0x5800;
                thunderx_bankedram_w(offset, value);
            }
            else if (address >= 0x6000 && address <= 0xffff)
            {

            }
        }
        public static void KWriteMemory_gbusters(ushort address, byte value)
        {
            if (address == 0x1f80)
            {
                gbusters_coin_counter_w(value);
            }
            else if (address == 0x1f84)
            {
                Sound.soundlatch_w(value);
            }
            else if (address == 0x1f88)
            {
                thunderx_sh_irqtrigger_w();
            }
            else if (address == 0x1f8c)
            {
                Generic.watchdog_reset_w();
            }
            else if (address == 0x1f98)
            {
                gbusters_1f98_w(value);
            }
            else if (address == 0x1f9c)
            {
                gbusters_unknown_w();
            }
            else if (address >= 0x0000 && address <= 0x3fff)
            {
                K052109_051960_w(address, value);
            }
            else if (address >= 0x4000 && address <= 0x57ff)
            {
                int offset = address - 0x4000;
                Memory.mainram[offset] = value;
            }
            else if (address >= 0x5800 && address <= 0x5fff)
            {
                int offset = address - 0x5800;
                bankedram_w(offset, value);
            }
            else if (address >= 0x6000 && address <= 0xffff)
            {

            }
        }
        public static byte ZReadOp_scontra(ushort address)
        {
            byte result = 0;
            if (address >= 0x0000 && address <= 0x7fff)
            {
                result = Memory.audiorom[address];
            }
            else if (address >= 0x8000 && address <= 0x87ff)
            {
                int offset = address - 0x8000;
                result = Memory.audioram[offset];
            }
            return result;
        }
        public static byte ZReadMemory_scontra(ushort address)
        {
            byte result = 0;
            if (address >= 0x0000 && address <= 0x7fff)
            {
                result = Memory.audiorom[address];
            }
            else if (address >= 0x8000 && address <= 0x87ff)
            {
                int offset = address - 0x8000;
                result = Memory.audioram[offset];
            }
            else if (address == 0xa000)
            {
                result = (byte)Sound.soundlatch_r();
            }
            else if (address >= 0xb000 && address <= 0xb00d)
            {
                int offset = address - 0xb000;
                result = K007232.k007232_read_port_0_r(offset);
            }
            else if (address == 0xc001)
            {
                result = YM2151.ym2151_status_port_0_r();
            }
            return result;
        }
        public static void ZWriteMemory_scontra(ushort address, byte value)
        {
            if (address >= 0x0000 && address <= 0x7fff)
            {

            }
            else if (address >= 0x8000 && address <= 0x87ff)
            {
                int offset = address - 0x8000;
                Memory.audioram[offset] = value;
            }
            else if (address >= 0xb000 && address <= 0xb00d)
            {
                int offset = address - 0xb000;
                K007232.k007232_write_port_0_w(offset, value);
            }
            else if (address == 0xc000)
            {
                YM2151.ym2151_register_port_0_w(value);
            }
            else if (address == 0xc001)
            {
                YM2151.ym2151_data_port_0_w(value);
            }
            else if (address == 0xf000)
            {
                scontra_snd_bankswitch_w(value);
            }
        }
        public static byte ZReadMemory_thunderx(ushort address)
        {
            byte result = 0;
            if (address >= 0x0000 && address <= 0x7fff)
            {
                result = Memory.audiorom[address];
            }
            else if (address >= 0x8000 && address <= 0x87ff)
            {
                int offset = address - 0x8000;
                result = Memory.audioram[offset];
            }
            else if (address == 0xa000)
            {
                result = (byte)Sound.soundlatch_r();
            }
            else if (address == 0xc001)
            {
                result = YM2151.ym2151_status_port_0_r();
            }
            return result;
        }
        public static void ZWriteMemory_thunderx(ushort address, byte value)
        {
            if (address >= 0x0000 && address <= 0x7fff)
            {

            }
            else if (address >= 0x8000 && address <= 0x87ff)
            {
                int offset = address - 0x8000;
                Memory.audioram[offset] = value;
            }
            else if (address == 0xc000)
            {
                YM2151.ym2151_register_port_0_w(value);
            }
            else if (address == 0xc001)
            {
                YM2151.ym2151_data_port_0_w(value);
            }
        }
        public static void ZWriteMemory_gbusters(ushort address, byte value)
        {
            if (address >= 0x0000 && address <= 0x7fff)
            {

            }
            else if (address >= 0x8000 && address <= 0x87ff)
            {
                int offset = address - 0x8000;
                Memory.audioram[offset] = value;
            }
            else if (address >= 0xb000 && address <= 0xb00d)
            {
                int offset = address - 0xb000;
                K007232.k007232_write_port_0_w(offset, value);
            }
            else if (address == 0xc000)
            {
                YM2151.ym2151_register_port_0_w(value);
            }
            else if (address == 0xc001)
            {
                YM2151.ym2151_data_port_0_w(value);
            }
            else if (address == 0xf000)
            {
                gbusters_snd_bankswitch_w(value);
            }
        }
        public static sbyte MReadOpByte_mystwarr(int address)
        {
            address &= 0xffffff;
            sbyte result = 0;
            if (address <= 0x1fffff)
            {
                result = (sbyte)Memory.mainrom[address];
            }
            else if (address >= 0x200000 && address <= 0x20ffff)
            {
                int offset = address - 0x200000;
                result = (sbyte)gx_workram[offset];
            }
            return result;
        }
        public static sbyte MReadByte_mystwarr(int address)
        {
            address &= 0xffffff;
            sbyte result = 0;
            if (address <= 0x1fffff)
            {
                result = (sbyte)Memory.mainrom[address];
            }
            else if (address >= 0x200000 && address <= 0x20ffff)
            {
                int offset = address - 0x200000;
                result = (sbyte)gx_workram[offset];
            }
            else if (address >= 0x400000 && address <= 0x40ffff)
            {
                int offset = (address - 0x400000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(K053247_scattered_word_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)K053247_scattered_word_r(offset);
                }
            }
            else if (address >= 0x482000 && address <= 0x48200f)
            {
                int offset = (address - 0x482000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(K055673_rom_word_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)K055673_rom_word_r(offset);
                }
            }
            else if (address >= 0x494000 && address <= 0x494001)
            {
                if (address % 2 == 0)
                {
                    result = sbyte2;
                }
                else if (address % 2 == 1)
                {
                    result = sbyte1;
                }
            }
            else if (address >= 0x494002 && address <= 0x494003)
            {
                if (address % 2 == 0)
                {
                    result = sbyte4;
                }
                else if (address % 2 == 1)
                {
                    result = sbyte3;
                }
            }
            else if (address >= 0x496000 && address <= 0x496001)
            {
                if (address % 2 == 0)
                {
                    result = (sbyte)(mmcoins_r() >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)mmcoins_r();
                }
            }
            else if (address >= 0x496002 && address <= 0x496003)
            {
                if (address % 2 == 0)
                {
                    result = (sbyte)(mweeprom_r() >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)mweeprom_r();
                }
            }
            else if (address >= 0x498014 && address <= 0x498015)
            {
                if (address % 2 == 0)
                {
                    result = (sbyte)(sound_status_r() >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)sound_status_r();
                }
            }
            else if (address >= 0x498000 && address <= 0x49801f)
            {
                int offset = address - 0x498000;
                result = (sbyte)mainram2[offset];
            }
            else if (address >= 0x600000 && address <= 0x601fff)
            {
                int offset = (address - 0x600000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(K056832_ram_word_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)K056832_ram_word_r(offset);
                }
            }
            else if (address >= 0x602000 && address <= 0x603fff)
            {
                int offset = (address - 0x602000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(K056832_ram_word_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)K056832_ram_word_r(offset);
                }
            }
            else if (address >= 0x680000 && address <= 0x683fff)
            {
                int offset = (address - 0x680000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(K056832_mw_rom_word_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)K056832_mw_rom_word_r(offset);
                }
            }
            else if (address >= 0x700000 && address <= 0x701fff)
            {
                int offset = (address - 0x700000) / 2;
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
        public static short MReadOpWord_mystwarr(int address)
        {
            address &= 0xffffff;
            short result = 0;
            if (address <= 0x1fffff)
            {
                if (address + 1 < Memory.mainrom.Length)
                    result = (short)(Memory.mainrom[address] * 0x100 + Memory.mainrom[address + 1]);
            }
            else if (address >= 0x200000 && address + 1 <= 0x20ffff)
            {
                int offset = address - 0x200000;
                result = (short)((gx_workram[offset] << 8) | gx_workram[offset + 1]);
            }
            return result;
        }
        public static short MReadWord_mystwarr(int address)
        {
            address &= 0xffffff;
            short result = 0;
            if (address <= 0x1fffff)
            {
                result = (short)(Memory.mainrom[address] * 0x100 + Memory.mainrom[address + 1]);
            }
            else if (address >= 0x200000 && address + 1 <= 0x20ffff)
            {
                int offset = address - 0x200000;
                result = (short)(gx_workram[offset] * 0x100 + gx_workram[offset + 1]);
            }
            else if (address >= 0x400000 && address + 1 <= 0x40ffff)
            {
                int offset = (address - 0x400000) / 2;
                result = (short)K053247_scattered_word_r(offset);
            }
            else if (address >= 0x482000 && address + 1 <= 0x48200f)
            {
                int offset = (address - 0x482000) / 2;
                result = (short)K055673_rom_word_r(offset);
            }
            else if (address >= 0x494000 && address + 1 <= 0x494001)
            {
                result = (short)(((byte)sbyte2 << 8) | (byte)sbyte1);
            }
            else if (address >= 0x494002 && address + 1 <= 0x494003)
            {
                result = (short)(((byte)sbyte4 << 8) | (byte)sbyte3);
            }
            else if (address >= 0x496000 && address + 1 <= 0x496001)
            {
                result = (short)mmcoins_r();
            }
            else if (address >= 0x496002 && address + 1 <= 0x496003)
            {
                result = (short)mweeprom_r();
            }
            else if (address >= 0x498014 && address + 1 <= 0x498015)
            {
                result = (short)sound_status_r();
            }
            else if (address >= 0x498000 && address + 1 <= 0x49801f)
            {
                int offset = address - 0x498000;
                result = (short)(mainram2[offset] * 0x100 + mainram2[offset + 1]);
            }
            else if (address >= 0x600000 && address + 1 <= 0x601fff)
            {
                int offset = (address - 0x600000) / 2;
                result = (short)K056832_ram_word_r(offset);
            }
            else if (address >= 0x602000 && address + 1 <= 0x603fff)
            {
                int offset = (address - 0x602000) / 2;
                result = (short)K056832_ram_word_r(offset);
            }
            else if (address >= 0x680000 && address + 1 <= 0x683fff)
            {
                int offset = (address - 0x680000) / 2;
                result = (short)K056832_mw_rom_word_r(offset);
            }
            else if (address >= 0x700000 && address + 1 <= 0x701fff)
            {
                int offset = (address - 0x700000) / 2;
                result = (short)Generic.paletteram16[offset];
            }
            return result;
        }
        public static int MReadOpLong_mystwarr(int address)
        {
            int result = 0;
            result = (int)((ushort)MReadOpWord_mystwarr(address) * 0x10000 + (ushort)MReadOpWord_mystwarr(address + 2));
            return result;
        }
        public static int MReadLong_mystwarr(int address)
        {
            int result = 0;
            result = (int)((ushort)MReadWord_mystwarr(address) * 0x10000 + (ushort)MReadWord_mystwarr(address + 2));
            return result;
        }
        public static void MWriteByte_mystwarr(int address, sbyte value)
        {
            address &= 0xffffff;
            if (address >= 0x200000 && address <= 0x20ffff)
            {
                int offset = address - 0x200000;
                gx_workram[offset] = (byte)value;
            }
            else if (address >= 0x400000 && address <= 0x40ffff)
            {
                int offset = (address - 0x400000) / 2;
                if (address % 2 == 0)
                {
                    K053247_scattered_word_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    K053247_scattered_word_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x480000 && address <= 0x4800ff)
            {
                int offset = (address - 0x480000)/2;
                K055555_word_w(offset, (byte)value);
            }
            else if (address >= 0x482010 && address <= 0x48201f)
            {
                int offset = (address - 0x482010) / 2;
                if (address % 2 == 0)
                {
                    K053247_reg_word_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    K053247_reg_word_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x484000 && address <= 0x484007)
            {
                int offset = address - 0x484000;
                K053246_w(offset, (byte)value);
            }
            else if (address >= 0x48a000 && address <= 0x48a01f)
            {
                int offset = (address - 0x48a000) / 2;
                if (address % 2 == 0)
                {
                    K054338_word_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    K054338_word_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x48c000 && address <= 0x48c03f)
            {
                int offset = (address - 0x48c000) / 2;
                if (address % 2 == 0)
                {
                    K056832_word_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    K056832_word_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x490000 && address <= 0x490001)
            {
                if (address % 2 == 0)
                {
                    mweeprom_w1((byte)value);
                }
            }
            else if (address >= 0x492000 && address <= 0x492001)
            {

            }
            else if (address >= 0x49800c && address <= 0x49800d)
            {
                if (address % 2 == 0)
                {
                    sound_cmd1_w(0);
                }
                else if (address % 2 == 1)
                {
                    sound_cmd1_w((byte)value);
                }
            }
            else if (address >= 0x49800e && address <= 0x49800f)
            {
                if (address % 2 == 0)
                {
                    sound_cmd2_w(0);
                }
                else if (address % 2 == 1)
                {
                    sound_cmd2_w((byte)value);
                }
            }
            else if (address >= 0x498000 && address <= 0x49801f)
            {
                int offset = address - 0x498000;
                mainram2[offset] = (byte)value;
            }
            else if (address >= 0x49a000 && address <= 0x49a001)
            {
                sound_irq_w();
            }
            else if (address >= 0x49c000 && address <= 0x49c01f)
            {
                int offset = (address - 0x49c000) / 2;
                if (address % 2 == 0)
                {
                    K053252_word_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    K053252_word_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x49e000 && address <= 0x49e007)
            {
                int offset = (address - 0x49e000) / 2;
                if (address % 2 == 0)
                {
                    irq_ack_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    irq_ack_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x600000 && address <= 0x601fff)
            {
                int offset = (address - 0x600000) / 2;
                if (address % 2 == 0)
                {
                    K056832_ram_word_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    K056832_ram_word_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x602000 && address <= 0x603fff)
            {
                int offset = (address - 0x602000) / 2;
                if (address % 2 == 0)
                {
                    K056832_ram_word_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    K056832_ram_word_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x700000 && address <= 0x701fff)
            {
                int offset = (address - 0x700000) / 2;
                if (address % 2 == 0)
                {
                    Generic.paletteram16_xrgb_word_be_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    Generic.paletteram16_xrgb_word_be_w2(offset, (byte)value);
                }
            }
        }
        public static void MWriteWord_mystwarr(int address, short value)
        {
            address &= 0xffffff;
            if (address >= 0x200000 && address + 1 <= 0x20ffff)
            {
                int offset = address - 0x200000;
                gx_workram[offset] = (byte)(value >> 8);
                gx_workram[offset + 1] = (byte)value;
            }
            else if (address >= 0x400000 && address + 1 <= 0x40ffff)
            {
                int offset = (address - 0x400000) / 2;
                K053247_scattered_word_w(offset, (ushort)value);
            }
            else if (address >= 0x480000 && address + 1 <= 0x4800ff)
            {
                int offset = (address - 0x480000) / 2;
                K055555_word_w(offset, (ushort)value);
            }
            else if (address >= 0x482010 && address + 1 <= 0x48201f)
            {
                int offset = (address - 0x482010) / 2;
                K053247_reg_word_w(offset, (ushort)value);
            }
            else if (address >= 0x484000 && address + 1 <= 0x484007)
            {
                int offset = (address - 0x484000) / 2;
                K053246_word_w(offset, (ushort)value);
            }
            else if (address >= 0x48a000 && address + 1 <= 0x48a01f)
            {
                int offset = (address - 0x48a000) / 2;
                K054338_word_w(offset, (ushort)value);
            }
            else if (address >= 0x48c000 && address + 1 <= 0x48c03f)
            {
                int offset = (address - 0x48c000) / 2;
                K056832_word_w(offset, (ushort)value);
            }
            else if (address >= 0x490000 && address + 1 <= 0x490001)
            {
                mweeprom_w((ushort)value);
            }
            else if (address >= 0x492000 && address + 1 <= 0x492001)
            {

            }
            else if (address >= 0x49800c && address + 1 <= 0x49800d)
            {
                sound_cmd1_w((ushort)value);
            }
            else if (address >= 0x49800e && address + 1 <= 0x49800f)
            {
                sound_cmd2_w((ushort)value);
            }
            else if (address >= 0x498000 && address + 1 <= 0x49801f)
            {
                int offset = address - 0x498000;
                mainram2[offset] = (byte)(value >> 8);
                mainram2[offset + 1] = (byte)value;
            }
            else if (address >= 0x49a000 && address + 1 <= 0x49a001)
            {
                sound_irq_w();
            }
            else if (address >= 0x49c000 && address + 1 <= 0x49c01f)
            {
                int offset = (address - 0x49c000) / 2;
                K053252_word_w(offset, (ushort)value);
            }
            else if (address >= 0x49e000 && address + 1 <= 0x49e007)
            {
                int offset = (address - 0x49e000) / 2;
                irq_ack_w(offset, (ushort)value);
            }
            else if (address >= 0x600000 && address + 1 <= 0x601fff)
            {
                int offset = (address - 0x600000) / 2;
                K056832_ram_word_w(offset, (ushort)value);
            }
            else if (address >= 0x602000 && address + 1 <= 0x603fff)
            {
                int offset = (address - 0x602000) / 2;
                K056832_ram_word_w(offset, (ushort)value);
            }
            else if (address >= 0x700000 && address + 1 <= 0x701fff)
            {
                int offset = (address - 0x700000) / 2;
                Generic.paletteram16_xrgb_word_be_w(offset, (ushort)value);
            }
        }
        public static void MWriteLong_mystwarr(int address, int value)
        {
            MWriteWord_mystwarr(address, (short)(value >> 16));
            MWriteWord_mystwarr(address + 2, (short)value);
        }
        public static byte ZReadOp_mystwarr(ushort address)
        {
            byte result = 0;
            if (address >= 0x0000 && address <= 0x7fff)
            {
                result = Memory.audiorom[address];
            }
            else if (address >= 0x8000 && address <= 0xbfff)
            {
                int offset = address - 0x8000;
                result = Memory.audiorom[basebanksnd + offset];
            }
            return result;
        }
        public static byte ZReadMemory_mystwarr(ushort address)
        {
            byte result = 0;
            if (address >= 0x0000 && address <= 0x7fff)
            {
                result = Memory.audiorom[address];
            }
            else if (address >= 0x8000 && address <= 0xbfff)
            {
                int offset = address - 0x8000;
                result = Memory.audiorom[basebanksnd + offset];
            }
            else if (address >= 0xc000 && address <= 0xdfff)
            {
                int offset = address - 0xc000;
                result = Memory.audioram[offset];
            }
            else if (address >= 0xe000 && address <= 0xe22f)
            {
                int offset = address - 0xe000;
                result = K054539.kk1[0].k054539_r(offset);
            }
            else if (address >= 0xe230 && address <= 0xe3ff)
            {
                int offset = address - 0xe230;
                result = audioram2[offset];
            }
            else if (address >= 0xe400 && address <= 0xe62f)
            {
                int offset = address - 0xe400;
                result = K054539.kk1[1].k054539_r(offset);
            }
            else if (address >= 0xe630 && address <= 0xe7ff)
            {
                int offset = address - 0xe630;
                result = audioram3[offset];
            }
            else if (address == 0xf002)
            {
                result = (byte)Sound.soundlatch_r();
            }
            else if (address == 0xf003)
            {
                result = (byte)Sound.soundlatch2_r();
            }
            return result;
        }
        public static void ZWriteMemory_mystwarr(ushort address, byte value)
        {
            if (address >= 0x0000 && address <= 0xbfff)
            {
                
            }
            else if (address >= 0xc000 && address <= 0xdfff)
            {
                int offset = address - 0xc000;
                Memory.audioram[offset] = value;
            }
            else if (address >= 0xe000 && address <= 0xe22f)
            {
                int offset = address - 0xe000;
                K054539.kk1[0].k054539_w(offset, value);
            }
            else if (address >= 0xe230 && address <= 0xe3ff)
            {
                int offset = address - 0xe230;
                audioram2[offset] = value;
            }
            else if (address >= 0xe400 && address <= 0xe62f)
            {
                int offset = address - 0xe400;
                K054539.kk1[1].k054539_w(offset, value);
            }
            else if (address >= 0xe630 && address <= 0xe7ff)
            {
                int offset = address - 0xe630;
                audioram3[offset] = value;
            }
            else if (address == 0xf000)
            {
                Sound.soundlatch3_w(value);
            }
            else if (address == 0xf800)
            {
                sound_ctrl_w(value);
            }
            else if (address >= 0xfff0 && address <= 0xfff3)
            {
                
            }
        }
    }
}
