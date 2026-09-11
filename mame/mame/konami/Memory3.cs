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
        public static sbyte MReadOpByte_moo(int address)
        {
            address &= 0xffffff;
            sbyte result = 0;
            if (address >= 0x000000 && address <= 0x07ffff)
            {
                result = (sbyte)Memory.mainrom[address];
            }
            else if (address >= 0x100000 && address <= 0x17ffff)
            {
                result = (sbyte)Memory.mainrom[address];
            }
            return result;
        }
        public static sbyte MReadByte_moo(int address)
        {
            address &= 0xffffff;
            sbyte result = 0;
            if (address >= 0x000000 && address <= 0x07ffff)
            {
                result = (sbyte)Memory.mainrom[address];
            }
            else if (address >= 0x0c4000 && address <= 0x0c4001)
            {
                if (address % 2 == 0)
                {
                    result = (sbyte)(K053246_word_r(0) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)K053246_word_r(0);
                }
            }
            else if (address >= 0x0d6014 && address <= 0x0d6015)
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
            else if (address >= 0x0d6000 && address <= 0x0d601f)
            {
                int offset = address - 0x0d6000;
                result = (sbyte)mainram2[offset];
            }
            else if (address >= 0x0da000 && address <= 0x0da001)
            {
                if (address % 2 == 0)
                {
                    result = sbyte3;
                }
                else if (address % 2 == 1)
                {
                    result = sbyte1;
                }
            }
            else if (address >= 0x0da002 && address <= 0x0da003)
            {
                if (address % 2 == 0)
                {
                    result = sbyte4;
                }
                else if (address % 2 == 1)
                {
                    result = sbyte2;
                }
            }
            else if (address >= 0x0dc000 && address <= 0x0dc001)
            {
                if (address % 2 == 0)
                {
                    result = 0;
                }
                else if (address % 2 == 1)
                {
                    result = sbyte0;
                }
            }
            else if (address >= 0x0dc002 && address <= 0x0dc003)
            {
                if (address % 2 == 0)
                {
                    result = (sbyte)(control1_r() >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)control1_r();
                }
            }
            else if (address >= 0x0de000 && address <= 0x0de001)
            {
                if (address % 2 == 0)
                {
                    result = (sbyte)(control2_r() >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)control2_r();
                }
            }
            else if (address >= 0x100000 && address <= 0x17ffff)
            {
                result = (sbyte)Memory.mainrom[address];
            }
            else if (address >= 0x180000 && address <= 0x18ffff)
            {
                int offset = address - 0x180000;
                result = (sbyte)Memory.mainram[offset];
            }
            else if (address >= 0x190000 && address <= 0x19ffff)
            {
                int offset = (address - 0x190000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(Generic.spriteram16[offset] >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)Generic.spriteram16[offset];
                }
            }
            else if (address >= 0x1a0000 && address <= 0x1a1fff)
            {
                int offset = (address - 0x1a0000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(K056832_ram_word_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)K056832_ram_word_r(offset);
                }
            }
            else if (address >= 0x1a2000 && address <= 0x1a3fff)
            {
                int offset = (address - 0x1a2000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(K056832_ram_word_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)K056832_ram_word_r(offset);
                }
            }
            else if (address >= 0x1b0000 && address <= 0x1b1fff)
            {
                int offset = (address - 0x1b0000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(K056832_rom_word_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)K056832_rom_word_r(offset);
                }
            }
            else if (address >= 0x1c0000 && address <= 0x1c1fff)
            {
                int offset = (address - 0x1c0000) / 2;
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
        public static short MReadOpWord_moo(int address)
        {
            address &= 0xffffff;
            short result = 0;
            if (address >= 0x000000 && address + 1 <= 0x07ffff)
            {
                result = (short)(Memory.mainrom[address] * 0x100 + Memory.mainrom[address + 1]);
            }
            else if (address >= 0x100000 && address + 1 <= 0x17ffff)
            {
                result = (short)(Memory.mainrom[address] * 0x100 + Memory.mainrom[address + 1]);
            }
            return result;
        }
        public static short MReadWord_moo(int address)
        {
            address &= 0xffffff;
            short result = 0;
            if (address >= 0x000000 && address + 1 <= 0x07ffff)
            {
                result = (short)(Memory.mainrom[address] * 0x100 + Memory.mainrom[address + 1]);
            }
            else if (address >= 0x0c4000 && address + 1 <= 0x0c4001)
            {
                result = (short)K053246_word_r(0);
            }
            else if (address >= 0x0d6014 && address + 1 <= 0x0d6015)
            {
                result = (short)sound_status_r();
            }
            else if (address >= 0x0d6000 && address + 1 <= 0x0d601f)
            {
                int offset = address - 0x0d6000;
                result = (short)(mainram2[offset] * 0x100 + mainram2[offset + 1]);
            }
            else if (address >= 0x0da000 && address + 1 <= 0x0da001)
            {
                result = (short)(((byte)sbyte3 << 8) | (byte)sbyte1);
            }
            else if (address >= 0x0da002 && address + 1 <= 0x0da003)
            {
                result = (short)(((byte)sbyte4 << 8) | (byte)sbyte2);
            }
            else if (address >= 0x0dc000 && address + 1 <= 0x0dc001)
            {
                result = (short)((byte)sbyte0);
            }
            else if (address >= 0x0dc002 && address + 1 <= 0x0dc003)
            {
                result = (short)control1_r();
            }
            else if (address >= 0x0de000 && address + 1 <= 0x0de001)
            {
                result = (short)control2_r();
            }
            else if (address >= 0x100000 && address + 1 <= 0x17ffff)
            {
                result = (short)(Memory.mainrom[address] * 0x100 + Memory.mainrom[address + 1]);
            }
            else if (address >= 0x180000 && address + 1 <= 0x18ffff)
            {
                int offset = address - 0x180000;
                result = (short)(Memory.mainram[offset] * 0x100 + Memory.mainram[offset + 1]);
            }
            else if (address >= 0x190000 && address + 1 <= 0x19ffff)
            {
                int offset = (address - 0x190000) / 2;
                result = (short)Generic.spriteram16[offset];
            }
            else if (address >= 0x1a0000 && address + 1 <= 0x1a1fff)
            {
                int offset = (address - 0x1a0000) / 2;
                result = (short)K056832_ram_word_r(offset);
            }
            else if (address >= 0x1a2000 && address + 1 <= 0x1a3fff)
            {
                int offset = (address - 0x1a2000) / 2;
                result = (short)K056832_ram_word_r(offset);
            }
            else if (address >= 0x1b0000 && address + 1 <= 0x1b1fff)
            {
                int offset = (address - 0x1b0000) / 2;
                result = (short)K056832_rom_word_r(offset);
            }
            else if (address >= 0x1c0000 && address + 1 <= 0x1c1fff)
            {
                int offset = (address - 0x1c0000) / 2;
                result = (short)Generic.paletteram16[offset];
            }
            return result;
        }
        public static int MReadOpLong_moo(int address)
        {
            int result = 0;
            result = (int)((ushort)MReadOpWord_moo(address) * 0x10000 + (ushort)MReadOpWord_moo(address + 2));
            return result;
        }
        public static int MReadLong_moo(int address)
        {
            int result = 0;
            result = (int)((ushort)MReadWord_moo(address) * 0x10000 + (ushort)MReadWord_moo(address + 2));
            return result;
        }
        public static void MWriteByte_moo(int address, sbyte value)
        {
            address &= 0xffffff;
            if (address >= 0x0c0000 && address <= 0x0c003f)
            {
                int offset = (address - 0x0c0000) / 2;
                if (address % 2 == 0)
                {
                    K056832_word_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    K056832_word_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x0c2000 && address <= 0x0c2007)
            {
                int offset = address - 0x0c2000;
                K053246_w(offset, (byte)value);
            }
            else if (address >= 0x0ca000 && address <= 0x0ca01f)
            {
                int offset = (address - 0x0ca000) / 2;
                if (address % 2 == 0)
                {
                    K054338_word_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    K054338_word_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x0cc000 && address <= 0x0cc01f)
            {
                int offset = (address - 0x0cc000) / 2;
                if (address % 2 == 1)
                {
                    K053251_lsb_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x0ce000 && address <= 0x0ce01f)
            {
                int offset = (address - 0x0ce000) / 2;
                if (address % 2 == 0)
                {
                    moo_prot_w1(offset, (byte)value);
                }
                else
                {
                    moo_prot_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x0d0000 && address <= 0x0d001f)
            {
                int offset = address - 0x0d0000;
                mainram3[offset] = (byte)value;
            }
            else if (address >= 0x0d4000 && address <= 0x0d4001)
            {
                sound_irq_w();
            }
            else if (address >= 0x0d600c && address <= 0x0d600d)
            {
                if (address % 2 == 0)
                {
                    sound_cmd1_w(0);
                }
                else if (address % 2 == 1)
                {
                    sound_cmd1_w((ushort)((byte)value));
                }
            }
            else if (address >= 0x0d600e && address <= 0x0d600f)
            {
                if (address % 2 == 0)
                {
                    sound_cmd2_w(0);
                }
                else if (address % 2 == 1)
                {
                    sound_cmd2_w((ushort)((byte)value));
                }
            }
            else if (address >= 0x0d6000 && address <= 0x0d601f)
            {
                int offset = address - 0x0d6000;
                mainram2[offset] = (byte)value;
            }
            else if (address >= 0x0d8000 && address <= 0x0d8007)
            {
                int offset = (address - 0x0d8000) / 2;
                if (address % 2 == 0)
                {
                    K056832_b_word_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    K056832_b_word_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x0de000 && address <= 0x0de001)
            {
                if (address % 2 == 0)
                {
                    control2_w1((byte)value);
                }
                else if (address % 2 == 1)
                {
                    control2_w2((byte)value);
                }
            }
            else if (address >= 0x180000 && address <= 0x18ffff)
            {
                int offset = address - 0x180000;
                Memory.mainram[offset] = (byte)value;
            }
            else if (address >= 0x190000 && address <= 0x19ffff)
            {
                int offset = (address - 0x190000) / 2;
                if (address % 2 == 0)
                {
                    Generic.spriteram16[offset] = (ushort)((value << 8) | (Generic.spriteram16[offset] & 0xff));
                }
                else if (address % 2 == 1)
                {
                    Generic.spriteram16[offset] = (ushort)((Generic.spriteram16[offset] & 0xff00) | (byte)value);
                }
            }
            else if (address >= 0x1a0000 && address <= 0x1a1fff)
            {
                int offset = (address - 0x1a0000) / 2;
                if (address % 2 == 0)
                {
                    K056832_ram_word_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    K056832_ram_word_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x1a2000 && address <= 0x1a3fff)
            {
                int offset = (address - 0x1a2000) / 2;
                if (address % 2 == 0)
                {
                    K056832_ram_word_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    K056832_ram_word_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x1c0000 && address <= 0x1c1fff)
            {
                int offset = (address - 0x1c0000) / 2;
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
        public static void MWriteWord_moo(int address, short value)
        {
            address &= 0xffffff;
            if (address >= 0x0c0000 && address + 1 <= 0x0c003f)
            {
                int offset = (address - 0x0c0000) / 2;
                K056832_word_w(offset, (ushort)value);
            }
            else if (address >= 0x0c2000 && address + 1 <= 0x0c2007)
            {
                int offset = (address - 0x0c2000) / 2;
                K053246_word_w(offset, (ushort)value);
            }
            else if (address >= 0x0ca000 && address + 1 <= 0x0ca01f)
            {
                int offset = (address - 0x0ca000) / 2;
                K054338_word_w(offset, (ushort)value);
            }
            else if (address >= 0x0cc000 && address + 1 <= 0x0cc01f)
            {
                int offset = (address - 0x0cc000) / 2;
                K053251_lsb_w(offset, (ushort)value);
            }
            else if (address >= 0x0ce000 && address + 1 <= 0x0ce01f)
            {
                int offset = (address - 0x0ce000) / 2;
                moo_prot_w(offset, (ushort)value);
            }
            else if (address >= 0x0d0000 && address + 1 <= 0x0d001f)
            {
                int offset = address - 0x0d0000;
                mainram3[offset] = (byte)(value >> 8);
                mainram3[offset + 1] = (byte)value;
            }
            else if (address >= 0x0d4000 && address + 1 <= 0x0d4001)
            {
                sound_irq_w();
            }
            else if (address >= 0x0d600c && address + 1 <= 0x0d600d)
            {
                sound_cmd1_w((ushort)value);
            }
            else if (address >= 0x0d600e && address + 1 <= 0x0d600f)
            {
                sound_cmd2_w((ushort)value);
            }
            else if (address >= 0x0d6000 && address + 1 <= 0x0d601f)
            {
                int offset = address - 0x0d6000;
                mainram2[offset] = (byte)(value >> 8);
                mainram2[offset + 1] = (byte)value;
            }
            else if (address >= 0x0d8000 && address + 1 <= 0x0d8007)
            {
                int offset = (address - 0x0d8000) / 2;
                K056832_b_word_w(offset, (ushort)value);
            }
            else if (address >= 0x0de000 && address + 1 <= 0x0de001)
            {
                control2_w((ushort)value);
            }
            else if (address >= 0x180000 && address + 1 <= 0x18ffff)
            {
                int offset = address - 0x180000;
                Memory.mainram[offset] = (byte)(value >> 8);
                Memory.mainram[offset + 1] = (byte)value;
            }
            else if (address >= 0x190000 && address + 1 <= 0x19ffff)
            {
                int offset = (address - 0x190000) / 2;
                Generic.spriteram16[offset] = (ushort)value;
            }
            else if (address >= 0x1a0000 && address + 1 <= 0x1a1fff)
            {
                int offset = (address - 0x1a0000) / 2;
                K056832_ram_word_w(offset, (ushort)value);
            }
            else if (address >= 0x1a2000 && address + 1 <= 0x1a3fff)
            {
                int offset = (address - 0x1a2000) / 2;
                K056832_ram_word_w(offset, (ushort)value);
            }
            else if (address >= 0x1c0000 && address + 1 <= 0x1c1fff)
            {
                int offset = (address - 0x1c0000) / 2;
                Generic.paletteram16_xrgb_word_be_w(offset, (ushort)value);
            }
        }
        public static void MWriteLong_moo(int address, int value)
        {
            MWriteWord_moo(address, (short)(value >> 16));
            MWriteWord_moo(address + 2, (short)value);
        }
        public static sbyte MReadOpByte_moobl(int address)
        {
            address &= 0xffffff;
            sbyte result = 0;
            if (address >= 0x000000 && address <= 0x07ffff)
            {
                result = (sbyte)Memory.mainrom[address];
            }
            else if (address >= 0x100000 && address <= 0x17ffff)
            {
                result = (sbyte)Memory.mainrom[address];
            }
            return result;
        }
        public static sbyte MReadByte_moobl(int address)
        {
            address &= 0xffffff;
            sbyte result = 0;
            if (address >= 0x000000 && address <= 0x07ffff)
            {
                result = (sbyte)Memory.mainrom[address];
            }
            else if (address >= 0x0c2f00 && address <= 0x0c2f01)
            {
                result = 0;
            }
            else if (address >= 0x0c4000 && address <= 0x0c4001)
            {
                if (address % 2 == 0)
                {
                    result = (sbyte)(K053246_word_r(0) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)K053246_word_r(0);
                }
            }
            else if (address >= 0x0d6ffe && address <= 0x0d6fff)
            {
                if (address % 2 == 0)
                {
                    result = (sbyte)(OKI6295.oo1[0].okim6295_status_r() >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)OKI6295.oo1[0].okim6295_status_r();
                }
            }
            else if (address >= 0x0da000 && address <= 0x0da001)
            {
                if (address % 2 == 0)
                {
                    result = sbyte3;
                }
                else if (address % 2 == 1)
                {
                    result = sbyte1;
                }
            }
            else if (address >= 0x0da002 && address <= 0x0da003)
            {
                if (address % 2 == 0)
                {
                    result = sbyte4;
                }
                else if (address % 2 == 1)
                {
                    result = sbyte2;
                }
            }
            else if (address >= 0x0dc000 && address <= 0x0dc001)
            {
                if (address % 2 == 0)
                {
                    result = 0;
                }
                else if (address % 2 == 1)
                {
                    result = sbyte0;
                }
            }
            else if (address >= 0x0dc002 && address <= 0x0dc003)
            {
                if (address % 2 == 0)
                {
                    result = (sbyte)(control1_r() >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)control1_r();
                }
            }
            else if (address >= 0x0de000 && address <= 0x0de001)
            {
                if (address % 2 == 0)
                {
                    result = (sbyte)(control2_r() >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)control2_r();
                }
            }
            else if (address >= 0x100000 && address <= 0x17ffff)
            {
                result = (sbyte)Memory.mainrom[address];
            }
            else if (address >= 0x180000 && address <= 0x18ffff)
            {
                int offset = address - 0x180000;
                result = (sbyte)Memory.mainram[offset];
            }
            else if (address >= 0x190000 && address <= 0x19ffff)
            {
                int offset = (address - 0x190000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(Generic.spriteram16[offset] >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)Generic.spriteram16[offset];
                }
            }
            else if (address >= 0x1a0000 && address <= 0x1a1fff)
            {
                int offset = (address - 0x1a0000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(K056832_ram_word_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)K056832_ram_word_r(offset);
                }
            }
            else if (address >= 0x1a2000 && address <= 0x1a3fff)
            {
                int offset = (address - 0x1a2000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(K056832_ram_word_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)K056832_ram_word_r(offset);
                }
            }
            else if (address >= 0x1b0000 && address <= 0x1b1fff)
            {
                int offset = (address - 0x1b0000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(K056832_rom_word_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)K056832_rom_word_r(offset);
                }
            }
            else if (address >= 0x1c0000 && address <= 0x1c1fff)
            {
                int offset = (address - 0x1c0000) / 2;
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
        public static short MReadOpWord_moobl(int address)
        {
            address &= 0xffffff;
            short result = 0;
            if (address >= 0x000000 && address + 1 <= 0x07ffff)
            {
                result = (short)(Memory.mainrom[address] * 0x100 + Memory.mainrom[address + 1]);
            }
            else if (address >= 0x100000 && address + 1 <= 0x17ffff)
            {
                result = (short)(Memory.mainrom[address] * 0x100 + Memory.mainrom[address + 1]);
            }
            return result;
        }
        public static short MReadWord_moobl(int address)
        {
            address &= 0xffffff;
            short result = 0;
            if (address >= 0x000000 && address + 1 <= 0x07ffff)
            {
                result = (short)(Memory.mainrom[address] * 0x100 + Memory.mainrom[address + 1]);
            }
            else if (address >= 0x0c4000 && address + 1 <= 0x0c4001)
            {
                result = (short)K053246_word_r(0);
            }
            else if (address >= 0x0d6ffe && address + 1 <= 0x0d6fff)
            {
                result = (short)OKI6295.oo1[0].okim6295_status_r();
            }
            else if (address >= 0x0da000 && address + 1 <= 0x0da001)
            {
                result = (short)(((byte)sbyte3 << 8) | (byte)sbyte1);
            }
            else if (address >= 0x0da002 && address + 1 <= 0x0da003)
            {
                result = (short)(((byte)sbyte4 << 8) | (byte)sbyte2);
            }
            else if (address >= 0x0dc000 && address + 1 <= 0x0dc001)
            {
                result = (short)((byte)sbyte0);
            }
            else if (address >= 0x0dc002 && address + 1 <= 0x0dc003)
            {
                result = (short)control1_r();
            }
            else if (address >= 0x0de000 && address + 1 <= 0x0de001)
            {
                result = (short)control2_r();
            }
            else if (address >= 0x100000 && address + 1 <= 0x17ffff)
            {
                result = (short)(Memory.mainrom[address] * 0x100 + Memory.mainrom[address + 1]);
            }
            else if (address >= 0x180000 && address + 1 <= 0x18ffff)
            {
                int offset = address - 0x180000;
                result = (short)(Memory.mainram[offset] * 0x100 + Memory.mainram[offset + 1]);
            }
            else if (address >= 0x190000 && address + 1 <= 0x19ffff)
            {
                int offset = (address - 0x190000) / 2;
                result = (short)Generic.spriteram16[offset];
            }
            else if (address >= 0x1a0000 && address + 1 <= 0x1a1fff)
            {
                int offset = (address - 0x1a0000) / 2;
                result = (short)K056832_ram_word_r(offset);
            }
            else if (address >= 0x1a2000 && address + 1 <= 0x1a3fff)
            {
                int offset = (address - 0x1a2000) / 2;
                result = (short)K056832_ram_word_r(offset);
            }
            else if (address >= 0x1b0000 && address + 1 <= 0x1b1fff)
            {
                int offset = (address - 0x1b0000) / 2;
                result = (short)K056832_rom_word_r(offset);
            }
            else if (address >= 0x1c0000 && address + 1 <= 0x1c1fff)
            {
                int offset = (address - 0x1c0000) / 2;
                result = (short)Generic.paletteram16[offset];
            }
            return result;
        }
        public static int MReadOpLong_moobl(int address)
        {
            int result = 0;
            result = (int)((ushort)MReadOpWord_moobl(address) * 0x10000 + (ushort)MReadOpWord_moobl(address + 2));
            return result;
        }
        public static int MReadLong_moobl(int address)
        {
            int result = 0;
            result = (int)((ushort)MReadWord_moobl(address) * 0x10000 + (ushort)MReadWord_moobl(address + 2));
            return result;
        }
        public static void MWriteByte_moobl(int address, sbyte value)
        {
            address &= 0xffffff;
            if (address >= 0x0c0000 && address <= 0x0c003f)
            {
                int offset = (address - 0x0c0000) / 2;
                if (address % 2 == 0)
                {
                    K056832_word_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    K056832_word_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x0c2000 && address <= 0x0c2007)
            {
                int offset = address - 0x0c2000;
                K053246_w(offset, (byte)value);
            }
            else if (address >= 0x0ca000 && address <= 0x0ca01f)
            {
                int offset = (address - 0x0ca000) / 2;
                if (address % 2 == 0)
                {
                    K054338_word_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    K054338_word_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x0cc000 && address <= 0x0cc01f)
            {
                int offset = (address - 0x0cc000) / 2;
                if (address % 2 == 0)
                {

                }
                else if (address % 2 == 1)
                {
                    K053251_lsb_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x0d0000 && address <= 0x0d001f)
            {
                int offset = address - 0x0d0000;
                mainram3[offset] = (byte)value;
            }
            else if (address >= 0x0d6ffc && address <= 0x0d6ffd)
            {
                if (address % 2 == 0)
                {
                    moobl_oki_bank_w((byte)0);
                }
                else if (address % 2 == 1)
                {
                    moobl_oki_bank_w((byte)value);
                }
            }
            else if (address >= 0x0d6ffe && address <= 0x0d6fff)
            {
                if (address % 2 == 0)
                {

                }
                else if (address % 2 == 1)
                {
                    OKI6295.oo1[0].okim6295_data_w((byte)value);
                }
            }
            else if (address >= 0x0d8000 && address <= 0x0d8007)
            {
                int offset = (address - 0x0d8000) / 2;
                if (address % 2 == 0)
                {
                    K056832_b_word_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    K056832_b_word_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x0de000 && address <= 0x0de001)
            {
                if (address % 2 == 0)
                {
                    control2_w1((byte)value);
                }
                else if (address % 2 == 1)
                {
                    control2_w2((byte)value);
                }
            }
            else if (address >= 0x180000 && address <= 0x18ffff)
            {
                int offset = address - 0x180000;
                Memory.mainram[offset] = (byte)value;
            }
            else if (address >= 0x190000 && address <= 0x19ffff)
            {
                int offset = (address - 0x190000) / 2;
                if (address % 2 == 0)
                {
                    Generic.spriteram16[offset] = (ushort)((value << 8) | (Generic.spriteram16[offset] & 0xff));
                }
                else if (address % 2 == 1)
                {
                    Generic.spriteram16[offset] = (ushort)((Generic.spriteram16[offset] & 0xff00) | (byte)value);
                }
            }
            else if (address >= 0x1a0000 && address <= 0x1a1fff)
            {
                int offset = (address - 0x1a0000) / 2;
                if (address % 2 == 0)
                {
                    K056832_ram_word_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    K056832_ram_word_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x1a2000 && address <= 0x1a3fff)
            {
                int offset = (address - 0x1a2000) / 2;
                if (address % 2 == 0)
                {
                    K056832_ram_word_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    K056832_ram_word_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x1c0000 && address <= 0x1c1fff)
            {
                int offset = (address - 0x1c0000) / 2;
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
        public static void MWriteWord_moobl(int address, short value)
        {
            address &= 0xffffff;
            if (address >= 0x0c0000 && address + 1 <= 0x0c003f)
            {
                int offset = (address - 0x0c0000) / 2;
                K056832_word_w(offset, (ushort)value);
            }
            else if (address >= 0x0c2000 && address + 1 <= 0x0c2007)
            {
                int offset = (address - 0x0c2000) / 2;
                K053246_word_w(offset, (ushort)value);
            }
            else if (address >= 0x0ca000 && address + 1 <= 0x0ca01f)
            {
                int offset = (address - 0x0ca000) / 2;
                K054338_word_w(offset, (ushort)value);
            }
            else if (address >= 0x0cc000 && address + 1 <= 0x0cc01f)
            {
                int offset = (address - 0x0cc000) / 2;
                K053251_lsb_w(offset, (ushort)value);
            }
            else if (address >= 0x0d0000 && address + 1 <= 0x0d001f)
            {
                int offset = address - 0x0d0000;
                mainram3[offset] = (byte)(value >> 8);
                mainram3[offset + 1] = (byte)value;
            }
            else if (address >= 0x0d6ffc && address + 1 <= 0x0d6ffd)
            {
                moobl_oki_bank_w((ushort)value);
            }
            else if (address >= 0x0d6ffe && address + 1 <= 0x0d6fff)
            {
                OKI6295.oo1[0].okim6295_data_w((ushort)value);
            }
            else if (address >= 0x0d8000 && address + 1 <= 0x0d8007)
            {
                int offset = (address - 0x0d8000) / 2;
                K056832_b_word_w(offset, (ushort)value);
            }
            else if (address >= 0x0de000 && address + 1 <= 0x0de001)
            {
                control2_w((ushort)value);
            }
            else if (address >= 0x180000 && address + 1 <= 0x18ffff)
            {
                int offset = address - 0x180000;
                Memory.mainram[offset] = (byte)(value >> 8);
                Memory.mainram[offset + 1] = (byte)value;
            }
            else if (address >= 0x190000 && address + 1 <= 0x19ffff)
            {
                int offset = (address - 0x190000) / 2;
                Generic.spriteram16[offset] = (ushort)value;
            }
            else if (address >= 0x1a0000 && address + 1 <= 0x1a1fff)
            {
                int offset = (address - 0x1a0000) / 2;
                K056832_ram_word_w(offset, (ushort)value);
            }
            else if (address >= 0x1a2000 && address + 1 <= 0x1a3fff)
            {
                int offset = (address - 0x1a2000) / 2;
                K056832_ram_word_w(offset, (ushort)value);
            }
            else if (address >= 0x1c0000 && address + 1 <= 0x1c1fff)
            {
                int offset = (address - 0x1c0000) / 2;
                Generic.paletteram16_xrgb_word_be_w(offset, (ushort)value);
            }
        }
        public static void MWriteLong_moobl(int address, int value)
        {
            MWriteWord_moobl(address, (short)(value >> 16));
            MWriteWord_moobl(address + 2, (short)value);
        }
        public static sbyte MReadOpByte_bucky(int address)
        {
            address &= 0xffffff;
            sbyte result = 0;
            if (address >= 0x000000 && address <= 0x07ffff)
            {
                result = (sbyte)Memory.mainrom[address];
            }
            else if (address >= 0x200000 && address <= 0x23ffff)
            {
                result = (sbyte)Memory.mainrom[address];
            }
            return result;
        }
        public static sbyte MReadByte_bucky(int address)
        {
            address &= 0xffffff;
            sbyte result = 0;
            if (address >= 0x000000 && address <= 0x07ffff)
            {
                result = (sbyte)Memory.mainrom[address];
            }
            else if (address >= 0x080000 && address <= 0x08ffff)
            {
                int offset = address - 0x080000;
                result = (sbyte)Memory.mainram[offset];
            }
            else if (address >= 0x090000 && address <= 0x09ffff)
            {
                int offset = (address - 0x090000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(Generic.spriteram16[offset] >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)Generic.spriteram16[offset];
                }
            }
            else if (address >= 0x0a0000 && address <= 0x0affff)
            {
                int offset = address - 0x0a0000;
                result = (sbyte)mainram2[offset];
            }
            else if (address >= 0x0c4000 && address <= 0x0c4001)
            {
                if (address % 2 == 0)
                {
                    result = (sbyte)(K053246_word_r(0) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)K053246_word_r(0);
                }
            }
            else if (address >= 0x0d2000 && address <= 0x0d20ff)
            {
                int offset = (address - 0x0d2000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(K054000_lsb_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)K054000_lsb_r(offset);
                }
            }
            else if (address >= 0x0d6014 && address <= 0x0d6015)
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
            else if (address >= 0x0d6000 && address <= 0x0d601f)
            {
                int offset = address - 0x0d6000;
                result = (sbyte)mainram4[offset];
            }
            else if (address >= 0x0da000 && address <= 0x0da001)
            {
                if (address % 2 == 0)
                {
                    result = sbyte3;
                }
                else if (address % 2 == 1)
                {
                    result = sbyte1;
                }
            }
            else if (address >= 0x0da002 && address <= 0x0da003)
            {
                if (address % 2 == 0)
                {
                    result = sbyte4;
                }
                else if (address % 2 == 1)
                {
                    result = sbyte2;
                }
            }
            else if (address >= 0x0dc000 && address <= 0x0dc001)
            {
                if (address % 2 == 0)
                {
                    result = 0;
                }
                else if (address % 2 == 1)
                {
                    result = sbyte0;
                }
            }
            else if (address >= 0x0dc002 && address <= 0x0dc003)
            {
                if (address % 2 == 0)
                {
                    result = (sbyte)(control1_r() >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)control1_r();
                }
            }
            else if (address >= 0x0de000 && address <= 0x0de001)
            {
                if (address % 2 == 0)
                {
                    result = (sbyte)(control2_r() >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)control2_r();
                }
            }
            else if (address >= 0x180000 && address <= 0x181fff)
            {
                int offset = (address - 0x180000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(K056832_ram_word_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)K056832_ram_word_r(offset);
                }
            }
            else if (address >= 0x182000 && address <= 0x183fff)
            {
                int offset = (address - 0x182000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(K056832_ram_word_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)K056832_ram_word_r(offset);
                }
            }
            else if (address >= 0x184000 && address <= 0x187fff)
            {
                int offset = address - 0x184000;
                result = (sbyte)mainram5[offset];
            }
            else if (address >= 0x190000 && address <= 0x191fff)
            {
                int offset = (address - 0x190000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(K056832_rom_word_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)K056832_rom_word_r(offset);
                }
            }
            else if (address >= 0x1b0000 && address <= 0x1b3fff)
            {
                int offset = (address - 0x1b0000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(Generic.paletteram16[offset] >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)Generic.paletteram16[offset];
                }
            }
            else if (address >= 0x200000 && address <= 0x23ffff)
            {
                result = (sbyte)Memory.mainrom[address];
            }
            return result;
        }
        public static short MReadOpWord_bucky(int address)
        {
            address &= 0xffffff;
            short result = 0;
            if (address >= 0x000000 && address + 1 <= 0x07ffff)
            {
                result = (short)(Memory.mainrom[address] * 0x100 + Memory.mainrom[address + 1]);
            }
            else if (address >= 0x200000 && address + 1 <= 0x23ffff)
            {
                result = (short)(Memory.mainrom[address] * 0x100 + Memory.mainrom[address + 1]);
            }
            return result;
        }
        public static short MReadWord_bucky(int address)
        {
            address &= 0xffffff;
            short result = 0;
            if (address >= 0x000000 && address + 1 <= 0x07ffff)
            {
                result = (short)(Memory.mainrom[address] * 0x100 + Memory.mainrom[address + 1]);
            }
            else if (address >= 0x080000 && address + 1 <= 0x08ffff)
            {
                int offset = address - 0x080000;
                result = (short)(Memory.mainram[offset] * 0x100 + Memory.mainram[offset + 1]);
            }
            else if (address >= 0x090000 && address + 1 <= 0x09ffff)
            {
                int offset = (address - 0x090000) / 2;
                result = (short)Generic.spriteram16[offset];
            }
            else if (address >= 0x0a0000 && address + 1 <= 0x0affff)
            {
                int offset = address - 0x0a0000;
                result = (short)(mainram2[offset] * 0x100 + mainram2[offset + 1]);
            }
            else if (address >= 0x0c4000 && address + 1 <= 0x0c4001)
            {
                result = (short)K053246_word_r(0);
            }
            else if (address >= 0x0d2000 && address + 1 <= 0x0d20ff)
            {
                int offset = (address - 0x0d2000) / 2;
                result = (short)K054000_lsb_r(offset);
            }
            else if (address >= 0x0d6014 && address + 1 <= 0x0d6015)
            {
                result = (short)sound_status_r();
            }
            else if (address >= 0x0d6000 && address + 1 <= 0x0d601f)
            {
                int offset = address - 0x0d6000;
                result = (short)(mainram4[offset] * 0x100 + mainram4[offset + 1]);
            }
            else if (address >= 0x0da000 && address + 1 <= 0x0da001)
            {
                result = (short)(((byte)sbyte3 << 8) | (byte)sbyte1);
            }
            else if (address >= 0x0da002 && address + 1 <= 0x0da003)
            {
                result = (short)(((byte)sbyte4 << 8) | (byte)sbyte2);
            }
            else if (address >= 0x0dc000 && address + 1 <= 0x0dc001)
            {
                result = (short)((byte)sbyte0);
            }
            else if (address >= 0x0dc002 && address + 1 <= 0x0dc003)
            {
                result = (short)control1_r();
            }
            else if (address >= 0x0de000 && address + 1 <= 0x0de001)
            {
                result = (short)control2_r();
            }
            else if (address >= 0x180000 && address + 1 <= 0x181fff)
            {
                int offset = (address - 0x180000) / 2;
                result = (short)K056832_ram_word_r(offset);
            }
            else if (address >= 0x182000 && address + 1 <= 0x183fff)
            {
                int offset = (address - 0x182000) / 2;
                result = (short)K056832_ram_word_r(offset);
            }
            else if (address >= 0x184000 && address + 1 <= 0x187fff)
            {
                int offset = address - 0x184000;
                result = (short)(mainram5[offset] * 0x100 + mainram5[offset + 1]);
            }
            else if (address >= 0x190000 && address + 1 <= 0x191fff)
            {
                int offset = (address - 0x190000) / 2;
                result = (short)K056832_rom_word_r(offset);
            }
            else if (address >= 0x1b0000 && address + 1 <= 0x1b3fff)
            {
                int offset = (address - 0x1b0000) / 2;
                result = (short)Generic.paletteram16[offset];
            }
            else if (address >= 0x200000 && address + 1 <= 0x23ffff)
            {
                result = (short)(Memory.mainrom[address] * 0x100 + Memory.mainrom[address + 1]);
            }
            return result;
        }
        public static int MReadOpLong_bucky(int address)
        {
            int result = 0;
            result = (int)((ushort)MReadOpWord_bucky(address) * 0x10000 + (ushort)MReadOpWord_bucky(address + 2));
            return result;
        }
        public static int MReadLong_bucky(int address)
        {
            int result = 0;
            result = (int)((ushort)MReadWord_bucky(address) * 0x10000 + (ushort)MReadWord_bucky(address + 2));
            return result;
        }
        public static void MWriteByte_bucky(int address, sbyte value)
        {
            address &= 0xffffff;
            if (address >= 0x080000 && address <= 0x08ffff)
            {
                int offset = address - 0x080000;
                Memory.mainram[offset] = (byte)value;
            }
            else if (address >= 0x090000 && address <= 0x09ffff)
            {
                int offset = (address - 0x090000) / 2;
                if (address % 2 == 0)
                {
                    Generic.spriteram16[offset] = (ushort)((value << 8) | (Generic.spriteram16[offset] & 0xff));
                }
                else if (address % 2 == 1)
                {
                    Generic.spriteram16[offset] = (ushort)((Generic.spriteram16[offset] & 0xff00) | (byte)value);
                }
            }
            else if (address >= 0x0a0000 && address <= 0x0affff)
            {
                int offset = address - 0x0a0000;
                mainram2[offset] = (byte)value;
            }
            else if (address >= 0x0c0000 && address <= 0x0c003f)
            {
                int offset = (address - 0x0c0000) / 2;
                if (address % 2 == 0)
                {
                    K056832_word_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    K056832_word_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x0c2000 && address <= 0x0c2007)
            {
                int offset = address - 0x0c2000;
                K053246_w(offset, (byte)value);
            }
            else if (address >= 0x0ca000 && address <= 0x0ca01f)
            {
                int offset = (address - 0x0ca000) / 2;
                if (address % 2 == 0)
                {
                    K054338_word_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    K054338_word_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x0cc000 && address <= 0x0cc01f)
            {
                int offset = (address - 0x0cc000) / 2;
                if (address % 2 == 0)
                {

                }
                else if (address % 2 == 1)
                {
                    K053251_lsb_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x0ce000 && address <= 0x0ce01f)
            {
                int offset = (address - 0x0ce000) / 2;
                if (address % 2 == 0)
                {
                    moo_prot_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    moo_prot_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x0d0000 && address <= 0x0d001f)
            {
                int offset = address - 0x0d0000;
                mainram3[offset] = (byte)value;
            }
            else if (address >= 0x0d2000 && address <= 0x0d20ff)
            {
                int offset = (address - 0x0d2000) / 2;
                if (address % 2 == 0)
                {

                }
                else if (address % 2 == 1)
                {
                    K054000_lsb_w(offset, (ushort)((byte)value));
                }
            }
            else if (address >= 0x0d4000 && address <= 0x0d4001)
            {
                sound_irq_w();
            }
            else if (address >= 0x0d600c && address <= 0x0d600d)
            {
                if (address % 2 == 0)
                {
                    sound_cmd1_w(0);
                }
                else if (address % 2 == 1)
                {
                    sound_cmd1_w((ushort)((byte)value));
                }
            }
            else if (address >= 0x0d600e && address <= 0x0d600f)
            {
                if (address % 2 == 0)
                {
                    sound_cmd2_w(0);
                }
                else if (address % 2 == 1)
                {
                    sound_cmd2_w((ushort)((byte)value));
                }
            }
            else if (address >= 0x0d6000 && address <= 0x0d601f)
            {
                int offset = address - 0x0d6000;
                mainram4[offset] = (byte)value;
            }
            else if (address >= 0x0d8000 && address <= 0x0d8007)
            {
                int offset = (address - 0x0d8000) / 2;
                if (address % 2 == 0)
                {
                    K056832_b_word_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    K056832_b_word_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x0de000 && address <= 0x0de001)
            {
                if (address % 2 == 0)
                {
                    control2_w1((byte)value);
                }
                else if (address % 2 == 1)
                {
                    control2_w2((byte)value);
                }
            }
            else if (address >= 0x180000 && address <= 0x181fff)
            {
                int offset = (address - 0x180000) / 2;
                if (address % 2 == 0)
                {
                    K056832_ram_word_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    K056832_ram_word_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x182000 && address <= 0x183fff)
            {
                int offset = (address - 0x182000) / 2;
                if (address % 2 == 0)
                {
                    K056832_ram_word_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    K056832_ram_word_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x184000 && address <= 0x187fff)
            {
                int offset = address - 0x184000;
                mainram5[offset] = (byte)value;
            }
            else if (address >= 0x1b0000 && address <= 0x1b3fff)
            {
                int offset = (address - 0x1b0000) / 2;
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
        public static void MWriteWord_bucky(int address, short value)
        {
            address &= 0xffffff;
            if (address >= 0x080000 && address + 1 <= 0x08ffff)
            {
                int offset = address - 0x080000;
                Memory.mainram[offset] = (byte)(value >> 8);
                Memory.mainram[offset + 1] = (byte)value;
            }
            else if (address >= 0x090000 && address + 1 <= 0x09ffff)
            {
                int offset = (address - 0x090000) / 2;
                Generic.spriteram16[offset] = (ushort)value;
            }
            else if (address >= 0x0a0000 && address + 1 <= 0x0affff)
            {
                int offset = address - 0x0a0000;
                mainram2[offset] = (byte)(value >> 8);
                mainram2[offset + 1] = (byte)value;
            }
            else if (address >= 0x0c0000 && address + 1 <= 0x0c003f)
            {
                int offset = (address - 0x0c0000) / 2;
                K056832_word_w(offset, (ushort)value);
            }
            else if (address >= 0x0c2000 && address + 1 <= 0x0c2007)
            {
                int offset = (address - 0x0c2000) / 2;
                K053246_word_w(offset, (ushort)value);
            }
            else if (address >= 0x0ca000 && address + 1 <= 0x0ca01f)
            {
                int offset = (address - 0x0ca000) / 2;
                K054338_word_w(offset, (ushort)value);
            }
            else if (address >= 0x0cc000 && address + 1 <= 0x0cc01f)
            {
                int offset = (address - 0x0cc000) / 2;
                K053251_lsb_w(offset, (ushort)value);
            }
            else if (address >= 0x0ce000 && address + 1 <= 0x0ce01f)
            {
                int offset = (address - 0x0ce000) / 2;
                moo_prot_w(offset, (ushort)value);
            }
            else if (address >= 0x0d0000 && address + 1 <= 0x0d001f)
            {
                int offset = address - 0x0d0000;
                mainram3[offset] = (byte)(value >> 8);
                mainram3[offset + 1] = (byte)value;
            }
            else if (address >= 0x0d2000 && address + 1 <= 0x0d20ff)
            {
                int offset = (address - 0x0d2000) / 2;
                K054000_lsb_w(offset, (ushort)value);
            }
            else if (address >= 0x0d4000 && address + 1 <= 0x0d4001)
            {
                sound_irq_w();
            }
            else if (address >= 0x0d600c && address + 1 <= 0x0d600d)
            {
                sound_cmd1_w((ushort)value);
            }
            else if (address >= 0x0d600e && address + 1 <= 0x0d600f)
            {
                sound_cmd2_w((ushort)value);
            }
            else if (address >= 0x0d6000 && address + 1 <= 0x0d601f)
            {
                int offset = address - 0x0d6000;
                mainram4[offset] = (byte)(value >> 8);
                mainram4[offset + 1] = (byte)value;
            }
            else if (address >= 0x0d8000 && address + 1 <= 0x0d8007)
            {
                int offset = (address - 0x0d8000) / 2;
                K056832_b_word_w(offset, (ushort)value);
            }
            else if (address >= 0x0de000 && address + 1 <= 0x0de001)
            {
                control2_w((ushort)value);
            }
            else if (address >= 0x180000 && address + 1 <= 0x181fff)
            {
                int offset = (address - 0x180000) / 2;
                K056832_ram_word_w(offset, (ushort)value);
            }
            else if (address >= 0x182000 && address + 1 <= 0x183fff)
            {
                int offset = (address - 0x182000) / 2;
                K056832_ram_word_w(offset, (ushort)value);
            }
            else if (address >= 0x184000 && address + 1 <= 0x187fff)
            {
                int offset = address - 0x184000;
                mainram5[offset] = (byte)(value >> 8);
                mainram5[offset + 1] = (byte)value;
            }
            else if (address >= 0x1b0000 && address + 1 <= 0x1b3fff)
            {
                int offset = (address - 0x1b0000) / 2;
                Generic.paletteram16_xrgb_word_be_w(offset, (ushort)value);
            }
        }
        public static void MWriteLong_bucky(int address, int value)
        {
            MWriteWord_bucky(address, (short)(value >> 16));
            MWriteWord_bucky(address + 2, (short)value);
        }
        public static sbyte MReadOpByte_mystwarr(int address)
        {
            address &= 0xffffff;
            sbyte result = 0;
            if (address >= 0x000000 && address <= 0x1fffff)
            {
                result = (sbyte)Memory.mainrom[address];
            }
            else if (address >= 0x200000 && address <= 0x20ffff)
            {
                int offset = address - 0x200000;
                result = (sbyte)Memory.mainram[offset];
            }
            return result;
        }
        public static sbyte MReadByte_mystwarr(int address)
        {
            address &= 0xffffff;
            sbyte result = 0;
            if (address >= 0x000000 && address <= 0x1fffff)
            {
                result = (sbyte)Memory.mainrom[address];
            }
            else if (address >= 0x200000 && address <= 0x20ffff)
            {
                int offset = address - 0x200000;
                result = (sbyte)Memory.mainram[offset];
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
            if (address >= 0x000000 && address + 1 <= 0x1fffff)
            {
                result = (short)(Memory.mainrom[address] * 0x100 + Memory.mainrom[address + 1]);
            }
            else if (address >= 0x200000 && address + 1 <= 0x20ffff)
            {
                int offset = address - 0x200000;
                result = (short)(Memory.mainram[offset] * 0x100 + Memory.mainram[offset + 1]);
            }
            return result;
        }
        public static short MReadWord_mystwarr(int address)
        {
            address &= 0xffffff;
            short result = 0;
            if (address >= 0x000000 && address + 1 <= 0x1fffff)
            {
                result = (short)(Memory.mainrom[address] * 0x100 + Memory.mainrom[address + 1]);
            }
            else if (address >= 0x200000 && address + 1 <= 0x20ffff)
            {
                int offset = address - 0x200000;
                result = (short)(Memory.mainram[offset] * 0x100 + Memory.mainram[offset + 1]);
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
                Memory.mainram[offset] = (byte)value;
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
                int offset = (address - 0x480000) / 2;
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
                Memory.mainram[offset] = (byte)(value >> 8);
                Memory.mainram[offset + 1] = (byte)value;
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
        public static sbyte MReadOpByte_dadandrn(int address)
        {
            address &= 0xffffff;
            sbyte result = 0;
            if (address >= 0x000000 && address <= 0x1fffff)
            {
                result = (sbyte)Memory.mainrom[address];
            }
            return result;
        }
        public static sbyte MReadByte_dadandrn(int address)
        {
            address &= 0xffffff;
            sbyte result = 0;
            if (address >= 0x000000 && address <= 0x1fffff)
            {
                result = (sbyte)Memory.mainrom[address];
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
            else if (address >= 0x410000 && address <= 0x413fff)
            {
                int offset = (address - 0x410000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(K056832_ram_word_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)K056832_ram_word_r(offset);
                }
            }
            else if (address >= 0x420000 && address <= 0x421fff)
            {
                int offset = (address - 0x420000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(Generic.paletteram16[offset] >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)Generic.paletteram16[offset];
                }
            }
            else if (address >= 0x440000 && address <= 0x443fff)
            {
                int offset = (address - 0x440000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(K056832_mw_rom_word_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)K056832_mw_rom_word_r(offset);
                }
            }
            else if (address >= 0x450000 && address <= 0x45000f)
            {
                int offset = (address - 0x450000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(K055673_rom_word_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)K055673_rom_word_r(offset);
                }
            }
            else if (address >= 0x470000 && address <= 0x470fff)
            {
                int offset = (address - 0x470000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(K053936_0_linectrl[offset] >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)K053936_0_linectrl[offset];
                }
            }
            else if (address >= 0x48a014 && address <= 0x48a015)
            {
                if (address % 2 == 0)
                {
                    result = (sbyte)(sound_status_msb_r() >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)sound_status_msb_r();
                }
            }
            else if (address >= 0x48a000 && address <= 0x48a01f)
            {
                int offset = address - 0x48a000;
                result = (sbyte)mainram2[offset];
            }
            else if (address >= 0x48e000 && address <= 0x48e001)
            {
                if (address % 2 == 0)
                {
                    result = (sbyte)(dddcoins_r() >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)dddcoins_r();
                }
            }
            else if (address >= 0x48e020 && address <= 0x48e021)
            {
                if (address % 2 == 0)
                {
                    result = (sbyte)dddeeprom_r1();
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)dddeeprom_r2();
                }
            }
            else if (address >= 0x600000 && address <= 0x60ffff)
            {
                int offset = address - 0x600000;
                result = (sbyte)Memory.mainram[offset];
            }
            else if (address >= 0x680000 && address <= 0x68003f)
            {
                int offset = (address - 0x680000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(K055550_word_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)K055550_word_r(offset);
                }
            }
            else if (address >= 0x800000 && address <= 0x87ffff)
            {
                int offset = (address - 0x800000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(ddd_053936_tilerom_0_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)ddd_053936_tilerom_0_r(offset);
                }
            }
            else if (address >= 0xa00000 && address <= 0xa7ffff)
            {
                int offset = (address - 0xa00000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(ddd_053936_tilerom_1_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)ddd_053936_tilerom_1_r(offset);
                }
            }
            else if (address >= 0xc00000 && address <= 0xdfffff)
            {
                int offset = (address - 0xc00000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(ddd_053936_tilerom_2_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)ddd_053936_tilerom_2_r(offset);
                }
            }
            return result;
        }
        public static short MReadOpWord_dadandrn(int address)
        {
            address &= 0xffffff;
            short result = 0;
            if (address >= 0x000000 && address + 1 <= 0x1fffff)
            {
                result = (short)(Memory.mainrom[address] * 0x100 + Memory.mainrom[address + 1]);
            }
            return result;
        }
        public static short MReadWord_dadandrn(int address)
        {
            address &= 0xffffff;
            short result = 0;
            if (address >= 0x000000 && address + 1 <= 0x1fffff)
            {
                result = (short)((Memory.mainrom[address] << 8) | (Memory.mainrom[address + 1] & 0xff));
            }
            else if (address >= 0x400000 && address <= 0x40ffff)
            {
                int offset = (address - 0x400000) / 2;
                result = (short)K053247_scattered_word_r(offset);
            }
            else if (address >= 0x410000 && address <= 0x413fff)
            {
                int offset = (address - 0x410000) / 2;
                result = (short)K056832_ram_word_r(offset);
            }
            else if (address >= 0x420000 && address + 1 <= 0x421fff)
            {
                int offset = (address - 0x420000) / 2;
                result = (short)Generic.paletteram16[offset];
            }
            else if (address >= 0x440000 && address <= 0x443fff)
            {
                int offset = (address - 0x440000) / 2;
                result = (short)K056832_mw_rom_word_r(offset);
            }
            else if (address >= 0x450000 && address <= 0x45000f)
            {
                int offset = (address - 0x450000) / 2;
                result = (short)K055673_rom_word_r(offset);
            }
            else if (address >= 0x470000 && address + 1 <= 0x470fff)
            {
                int offset = (address - 0x470000) / 2;
                result = (short)K053936_0_linectrl[offset];
            }
            else if (address >= 0x48a014 && address + 1 <= 0x48a015)
            {
                result = (short)sound_status_msb_r();
            }
            else if (address >= 0x48a000 && address + 1 <= 0x48a01f)
            {
                int offset = address - 0x48a000;
                result = (short)(mainram2[offset] * 0x100 + mainram2[offset + 1]);
            }
            else if (address >= 0x48e000 && address + 1 <= 0x48e001)
            {
                result = (short)dddcoins_r();
            }
            else if (address >= 0x48e020 && address + 1 <= 0x48e021)
            {
                result = (short)dddeeprom_r();
            }
            else if (address >= 0x600000 && address + 1 <= 0x60ffff)
            {
                int offset = address - 0x600000;
                result = (short)(Memory.mainram[offset] * 0x100 + Memory.mainram[offset + 1]);
            }
            else if (address >= 0x680000 && address + 1 <= 0x68003f)
            {
                int offset = (address - 0x680000) / 2;
                result = (short)K055550_word_r(offset);
            }
            else if (address >= 0x800000 && address + 1 <= 0x87ffff)
            {
                int offset = (address - 0x800000) / 2;
                result = (short)ddd_053936_tilerom_0_r(offset);
            }
            else if (address >= 0xa00000 && address + 1 <= 0xa7ffff)
            {
                int offset = (address - 0xa00000) / 2;
                result = (short)ddd_053936_tilerom_1_r(offset);
            }
            else if (address >= 0xc00000 && address / 2 <= 0xdfffff)
            {
                int offset = (address - 0xc00000) >> 1;
                result = (short)ddd_053936_tilerom_2_r(offset);
            }
            return result;
        }
        public static int MReadOpLong_dadandrn(int address)
        {
            int result = 0;
            result = (int)((ushort)MReadOpWord_dadandrn(address) * 0x10000 + (ushort)MReadOpWord_dadandrn(address + 2));
            return result;
        }
        public static int MReadLong_dadandrn(int address)
        {
            int result = 0;
            result = (int)((ushort)MReadWord_dadandrn(address) * 0x10000 + (ushort)MReadWord_dadandrn(address + 2));
            return result;
        }
        public static void MWriteByte_dadandrn(int address, sbyte value)
        {
            address &= 0xffffff;
            if (address >= 0x400000 && address <= 0x40ffff)
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
            else if (address >= 0x410000 && address <= 0x411fff)
            {
                int offset = (address - 0x410000) / 2;
                if (address % 2 == 0)
                {
                    K056832_ram_word_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    K056832_ram_word_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x412000 && address <= 0x413fff)
            {
                int offset = (address - 0x412000) / 2;
                if (address % 2 == 0)
                {
                    K056832_ram_word_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    K056832_ram_word_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x420000 && address <= 0x421fff)
            {
                int offset = (address - 0x420000) / 2;
                if (address % 2 == 0)
                {
                    Generic.paletteram16_xrgb_word_be_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    Generic.paletteram16_xrgb_word_be_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x430000 && address <= 0x430007)
            {
                int offset = address - 0x430000;
                K053246_w(offset, (byte)value);
            }
            else if (address >= 0x450010 && address <= 0x45001f)
            {
                int offset = (address - 0x450010) / 2;
                if (address % 2 == 0)
                {
                    K053247_reg_word_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    K053247_reg_word_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x460000 && address <= 0x46001f)
            {
                int offset = (address - 0x460000) / 2;
                if (address % 2 == 0)
                {
                    K053936_0_ctrl[offset] = (ushort)((value << 8) | (K053936_0_ctrl[offset] & 0xff));
                }
                else if (address % 2 == 1)
                {
                    K053936_0_ctrl[offset] = (ushort)((K053936_0_ctrl[offset] & 0xff00) | (byte)value);
                }
            }
            else if (address >= 0x470000 && address <= 0x470fff)
            {
                int offset = (address - 0x470000) / 2;
                if (address % 2 == 0)
                {
                    K053936_0_linectrl[offset] = (ushort)((value << 8) | (K053936_0_linectrl[offset] & 0xff));
                }
                else if (address % 2 == 1)
                {
                    K053936_0_linectrl[offset] = (ushort)((K053936_0_linectrl[offset] & 0xff00) | (byte)value);
                }
            }
            else if (address >= 0x480000 && address <= 0x48003f)
            {
                int offset = (address - 0x480000) / 2;
                if (address % 2 == 0)
                {
                    K056832_word_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    K056832_word_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x482000 && address <= 0x482007)
            {
                int offset = (address - 0x482000) / 2;
                if (address % 2 == 0)
                {
                    K056832_b_word_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    K056832_b_word_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x484000 && address <= 0x484003)
            {
                int offset = (address - 0x484000) / 2;
                if (address % 2 == 0)
                {
                    ddd_053936_clip_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    ddd_053936_clip_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x486000 && address <= 0x48601f)
            {
                int offset = (address - 0x486000) / 2;
                if (address % 2 == 0)
                {
                    K053252_word_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    K053252_word_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x488000 && address <= 0x4880ff)
            {
                int offset = (address - 0x488000) / 2;
                K055555_word_w(offset, (byte)value);
            }
            else if (address >= 0x48a00c && address <= 0x48a00d)
            {
                if (address % 2 == 0)
                {
                    sound_cmd1_msb_w1((byte)value);
                }
            }
            else if (address >= 0x48a00e && address <= 0x48a00f)
            {
                if (address % 2 == 0)
                {
                    sound_cmd2_msb_w1((byte)value);
                }
            }
            else if (address >= 0x48a000 && address <= 0x48a01f)
            {
                int offset = address - 0x48a000;
                mainram2[offset] = (byte)value;
            }
            else if (address >= 0x48c000 && address <= 0x48c01f)
            {
                int offset = (address - 0x48c000) / 2;
                if (address % 2 == 0)
                {
                    K054338_word_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    K054338_word_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x600000 && address <= 0x60ffff)
            {
                int offset = address - 0x600000;
                Memory.mainram[offset] = (byte)value;
            }
            else if (address >= 0x680000 && address <= 0x68003f)
            {
                int offset = (address - 0x680000) / 2;
                if (address % 2 == 0)
                {
                    K055550_word_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    K055550_word_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x6a0000 && address <= 0x6a0001)
            {
                if (address % 2 == 1)
                {
                    mmeeprom_w2((byte)value);
                }
            }
            else if (address >= 0x6c0000 && address <= 0x6c0001)
            {
                if (address % 2 == 0)
                {
                    ddd_053936_enable_w1((byte)value);
                }
            }
            else if (address >= 0x6e0000 && address <= 0x6e0001)
            {
                sound_irq_w();
            }
            else if (address >= 0xe00000 && address <= 0xe00001)
            {

            }
        }
        public static void MWriteWord_dadandrn(int address, short value)
        {
            address &= 0xffffff;
            if (address >= 0x400000 && address + 1 <= 0x40ffff)
            {
                int offset = (address - 0x400000) / 2;
                K053247_scattered_word_w(offset, (ushort)value);
            }
            else if (address >= 0x410000 && address + 1 <= 0x411fff)
            {
                int offset = (address - 0x410000) / 2;
                K056832_ram_word_w(offset, (ushort)value);
            }
            else if (address >= 0x412000 && address + 1 <= 0x413fff)
            {
                int offset = (address - 0x412000) / 2;
                K056832_ram_word_w(offset, (ushort)value);
            }
            else if (address >= 0x420000 && address + 1 <= 0x421fff)
            {
                int offset = (address - 0x420000) / 2;
                Generic.paletteram16_xrgb_word_be_w(offset, (ushort)value);
            }
            else if (address >= 0x430000 && address + 1 <= 0x430007)
            {
                int offset = (address - 0x430000) / 2;
                K053246_word_w(offset, (ushort)value);
            }
            else if (address >= 0x450010 && address + 1 <= 0x45001f)
            {
                int offset = (address - 0x450010) / 2;
                K053247_reg_word_w(offset, (ushort)value);
            }
            else if (address >= 0x460000 && address + 1 <= 0x46001f)
            {
                int offset = (address - 0x460000) / 2;
                K053936_0_ctrl[offset] = (ushort)value;
            }
            else if (address >= 0x470000 && address + 1 <= 0x470fff)
            {
                int offset = (address - 0x470000) / 2;
                K053936_0_linectrl[offset] = (ushort)value;
            }
            else if (address >= 0x480000 && address + 1 <= 0x48003f)
            {
                int offset = (address - 0x480000) / 2;
                K056832_word_w(offset, (ushort)value);
            }
            else if (address >= 0x482000 && address + 1 <= 0x482007)
            {
                int offset = (address - 0x482000) / 2;
                K056832_b_word_w(offset, (ushort)value);
            }
            else if (address >= 0x484000 && address + 1 <= 0x484003)
            {
                int offset = (address - 0x484000) / 2;
                ddd_053936_clip_w(offset, (ushort)value);
            }
            else if (address >= 0x486000 && address + 1 <= 0x48601f)
            {
                int offset = (address - 0x486000) / 2;
                K053252_word_w(offset, (ushort)value);
            }
            else if (address >= 0x488000 && address + 1 <= 0x4880ff)
            {
                int offset = (address - 0x488000) / 2;
                K055555_word_w(offset, (ushort)value);
            }
            else if (address >= 0x48a00c && address + 1 <= 0x48a00d)
            {
                sound_cmd1_msb_w((ushort)value);
            }
            else if (address >= 0x48a00e && address + 1 <= 0x48a00f)
            {
                sound_cmd2_msb_w((ushort)value);
            }
            else if (address >= 0x48a000 && address + 1 <= 0x48a01f)
            {
                int offset = address - 0x48a000;
                mainram2[offset] = (byte)(value >> 8);
                mainram2[offset + 1] = (byte)value;
            }
            else if (address >= 0x48c000 && address + 1 <= 0x48c01f)
            {
                int offset = (address - 0x48c000) / 2;
                K054338_word_w(offset, (ushort)value);
            }
            else if (address >= 0x600000 && address + 1 <= 0x60ffff)
            {
                int offset = address - 0x600000;
                Memory.mainram[offset] = (byte)(value >> 8);
                Memory.mainram[offset + 1] = (byte)value;
            }
            else if (address >= 0x680000 && address + 1 <= 0x68003f)
            {
                int offset = (address - 0x680000) / 2;
                K055550_word_w(offset, (ushort)value);
            }
            else if (address >= 0x6a0000 && address + 1 <= 0x6a0001)
            {
                mmeeprom_w((ushort)value);
            }
            else if (address >= 0x6c0000 && address + 1 <= 0x6c0001)
            {
                ddd_053936_enable_w((ushort)value);
            }
            else if (address >= 0x6e0000 && address + 1 <= 0x6e0001)
            {
                sound_irq_w();
            }
            else if (address >= 0xe00000 && address + 1 <= 0xe00001)
            {

            }
        }
        public static void MWriteLong_dadandrn(int address, int value)
        {
            MWriteWord_dadandrn(address, (short)(value >> 16));
            MWriteWord_dadandrn(address + 2, (short)value);
        }
        public static sbyte MReadOpByte_viostorm(int address)
        {
            address &= 0xffffff;
            sbyte result = 0;
            if (address >= 0x000000 && address <= 0x1fffff)
            {
                result = (sbyte)Memory.mainrom[address];
            }
            return result;
        }
        public static sbyte MReadByte_viostorm(int address)
        {
            address &= 0xffffff;
            sbyte result = 0;
            if (address >= 0x000000 && address <= 0x1fffff)
            {
                result = (sbyte)Memory.mainrom[address];
            }
            else if (address >= 0x200000 && address <= 0x20ffff)
            {
                int offset = address - 0x200000;
                result = (sbyte)Memory.mainram[offset];
            }
            else if (address >= 0x210000 && address <= 0x210fff)
            {
                int offset = (address - 0x210000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(K053247_word_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)K053247_word_r(offset);
                }
            }
            else if (address >= 0x211000 && address <= 0x21ffff)
            {
                int offset = address - 0x211000;
                result = (sbyte)mainram2[offset];
            }
            else if (address >= 0x244000 && address <= 0x24400f)
            {
                int offset = (address - 0x244000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(K055673_rom_word_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)K055673_rom_word_r(offset);
                }
            }
            else if (address >= 0x24c000 && address <= 0x24ffff)
            {
                int offset = address - 0x24c000;
                result = (sbyte)mainram3[offset];
            }
            else if (address >= 0x250000 && address <= 0x25000f)
            {
                int offset = address - 0x250000;
                result = (sbyte)mainram4[offset];
            }
            else if (address >= 0x25c000 && address <= 0x25c03f)
            {
                int offset = (address - 0x25c000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(K055550_word_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)K055550_word_r(offset);
                }
            }
            else if (address >= 0x268014 && address <= 0x268015)
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
            else if (address >= 0x268000 && address <= 0x26801f)
            {
                int offset = address - 0x268000;
                result = (sbyte)mainram5[offset];
            }
            else if (address >= 0x274000 && address <= 0x274001)
            {
                if (address % 2 == 0)
                {
                    result = (sbyte)sbyte3;
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)sbyte1;
                }
            }
            else if (address >= 0x274002 && address <= 0x274003)
            {
                if (address % 2 == 0)
                {
                    result = (sbyte)sbyte4;
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)sbyte2;
                }
            }
            else if (address >= 0x278000 && address <= 0x278001)
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
            else if (address >= 0x278002 && address <= 0x278003)
            {
                if (address % 2 == 0)
                {
                    result = 0;
                }
                else
                {
                    result = (sbyte)vseeprom_r2();
                }
            }
            else if (address >= 0x27c000 && address <= 0x27c001)
            {
                result = 0;
            }
            else if (address >= 0x300000 && address <= 0x301fff)
            {
                int offset = (address - 0x300000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(K056832_ram_word_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)K056832_ram_word_r(offset);
                }
            }
            else if (address >= 0x302000 && address <= 0x303fff)
            {
                int offset = (address - 0x302000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(K056832_ram_word_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)K056832_ram_word_r(offset);
                }
            }
            else if (address >= 0x304000 && address <= 0x3041ff)
            {
                int offset = address - 0x304000;
                result = (sbyte)mainram6[offset];
            }
            else if (address >= 0x310000 && address <= 0x311fff)
            {
                int offset = (address - 0x310000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(K056832_mw_rom_word_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)K056832_mw_rom_word_r(offset);
                }
            }
            else if (address >= 0x330000 && address <= 0x331fff)
            {
                int offset = (address - 0x330000) / 2;
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
        public static short MReadOpWord_viostorm(int address)
        {
            address &= 0xffffff;
            short result = 0;
            if (address >= 0x000000 && address + 1 <= 0x1fffff)
            {
                result = (short)(Memory.mainrom[address] * 0x100 + Memory.mainrom[address + 1]);
            }
            return result;
        }
        public static short MReadWord_viostorm(int address)
        {
            address &= 0xffffff;
            short result = 0;
            if (address >= 0x000000 && address + 1 <= 0x1fffff)
            {
                result = (short)(Memory.mainrom[address] * 0x100 + Memory.mainrom[address + 1]);
            }
            else if (address >= 0x200000 && address + 1 <= 0x20ffff)
            {
                int offset = address - 0x200000;
                result = (short)(Memory.mainram[offset] * 0x100 + Memory.mainram[offset + 1]);
            }
            else if (address >= 0x210000 && address + 1 <= 0x210fff)
            {
                int offset = (address - 0x210000) / 2;
                result = (short)K053247_word_r(offset);
            }
            else if (address >= 0x211000 && address + 1 <= 0x21ffff)
            {
                int offset = address - 0x211000;
                result = (short)(mainram2[offset] * 0x100 + mainram2[offset + 1]);
            }
            else if (address >= 0x244000 && address + 1 <= 0x24400f)
            {
                int offset = (address - 0x244000) / 2;
                result = (short)K055673_rom_word_r(offset);
            }
            else if (address >= 0x24c000 && address + 1 <= 0x24ffff)
            {
                int offset = address - 0x24c000;
                result = (short)(mainram3[offset] * 0x100 + mainram3[offset + 1]);
            }
            else if (address >= 0x250000 && address + 1 <= 0x25000f)
            {
                int offset = address - 0x250000;
                result = (short)(mainram4[offset] * 0x100 + mainram4[offset + 1]);
            }
            else if (address >= 0x25c000 && address + 1 <= 0x25c03f)
            {
                int offset = (address - 0x25c000) / 2;
                result = (short)K055550_word_r(offset);
            }
            else if (address >= 0x268014 && address + 1 <= 0x268015)
            {
                result = (short)sound_status_r();
            }
            else if (address >= 0x268000 && address + 1 <= 0x26801f)
            {
                int offset = address - 0x268000;
                result = (short)(mainram5[offset] * 0x100 + mainram5[offset + 1]);
            }
            else if (address >= 0x274000 && address + 1 <= 0x274001)
            {
                result = (short)((sbyte3 << 8) | (byte)sbyte1);
            }
            else if (address >= 0x274002 && address + 1 <= 0x274003)
            {
                result = (short)((sbyte4 << 8) | (byte)sbyte2);
            }
            else if (address >= 0x278000 && address + 1 <= 0x278001)
            {
                result = (short)mmcoins_r();
            }
            else if (address >= 0x278002 && address + 1 <= 0x278003)
            {
                result = (short)vseeprom_r();
            }
            else if (address >= 0x27c000 && address + 1 <= 0x27c001)
            {
                result = 0;
            }
            else if (address >= 0x300000 && address + 1 <= 0x301fff)
            {
                int offset = (address - 0x300000) / 2;
                result = (short)K056832_ram_word_r(offset);
            }
            else if (address >= 0x302000 && address + 1 <= 0x303fff)
            {
                int offset = (address - 0x302000) / 2;
                result = (short)K056832_ram_word_r(offset);
            }
            else if (address >= 0x304000 && address + 1 <= 0x3041ff)
            {
                int offset = address - 0x304000;
                result = (short)(mainram6[offset] * 0x100 + mainram6[offset + 1]);
            }
            else if (address >= 0x310000 && address + 1 <= 0x311fff)
            {
                int offset = (address - 0x310000) / 2;
                result = (short)K056832_mw_rom_word_r(offset);
            }
            else if (address >= 0x330000 && address + 1 <= 0x331fff)
            {
                int offset = (address - 0x330000) / 2;
                result = (short)Generic.paletteram16[offset];
            }
            return result;
        }
        public static int MReadOpLong_viostorm(int address)
        {
            int result = 0;
            result = (int)((ushort)MReadOpWord_viostorm(address) * 0x10000 + (ushort)MReadOpWord_viostorm(address + 2));
            return result;
        }
        public static int MReadLong_viostorm(int address)
        {
            int result = 0;
            result = (int)((ushort)MReadWord_viostorm(address) * 0x10000 + (ushort)MReadWord_viostorm(address + 2));
            return result;
        }
        public static void MWriteByte_viostorm(int address, sbyte value)
        {
            address &= 0xffffff;
            if (address >= 0x200000 && address <= 0x20ffff)
            {
                int offset = address - 0x200000;
                Memory.mainram[offset] = (byte)value;
            }
            else if (address >= 0x210000 && address <= 0x210fff)
            {
                int offset = (address - 0x210000) / 2;
                if (address % 2 == 0)
                {
                    K053247_word_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    K053247_word_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x211000 && address <= 0x21ffff)
            {
                int offset = address - 0x211000;
                mainram2[offset] = (byte)value;
            }
            else if (address >= 0x240000 && address <= 0x240007)
            {
                int offset = address - 0x240000;
                K053246_w(offset, (byte)value);
            }
            else if (address >= 0x244010 && address <= 0x24401f)
            {
                int offset = (address - 0x244010) / 2;
                if (address % 2 == 0)
                {
                    K053247_reg_word_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    K053247_reg_word_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x24c000 && address <= 0x24ffff)
            {
                int offset = address - 0x24c000;
                mainram3[offset] = (byte)value;
            }
            else if (address >= 0x250000 && address <= 0x25000f)
            {
                int offset = address - 0x250000;
                mainram4[offset] = (byte)value;
            }
            else if (address >= 0x254000 && address <= 0x25401f)
            {
                int offset = (address - 0x254000) / 2;
                if (address % 2 == 0)
                {
                    K054338_word_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    K054338_word_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x258000 && address <= 0x2580ff)
            {
                int offset = (address - 0x258000) / 2;
                K055555_word_w(offset, (byte)value);
            }
            else if (address >= 0x25c000 && address <= 0x25c03f)
            {
                int offset = (address - 0x25c000) / 2;
                if (address % 2 == 0)
                {
                    K055550_word_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    K055550_word_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x260000 && address <= 0x26001f)
            {
                int offset = (address - 0x260000) / 2;
                if (address % 2 == 0)
                {
                    K053252_word_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    K053252_word_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x264000 && address <= 0x264001)
            {
                sound_irq_w();
            }
            else if (address >= 0x26800c && address <= 0x26800d)
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
            else if (address >= 0x26800e && address <= 0x26800f)
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
            else if (address >= 0x268000 && address <= 0x26801f)
            {
                int offset = address - 0x268000;
                mainram5[offset] = (byte)value;
            }
            else if (address >= 0x26C000 && address <= 0x26C007)
            {
                int offset = (address - 0x26C000) / 2;
                if (address % 2 == 0)
                {
                    K056832_b_word_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    K056832_b_word_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x270000 && address <= 0x27003f)
            {
                int offset = (address - 0x270000) / 2;
                if (address % 2 == 0)
                {
                    K056832_word_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    K056832_word_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x27C000 && address <= 0x27C001)
            {
                if (address % 2 == 1)
                {
                    mmeeprom_w2((byte)value);
                }
            }
            else if (address >= 0x300000 && address <= 0x301fff)
            {
                int offset = (address - 0x300000) / 2;
                if (address % 2 == 0)
                {
                    K056832_ram_word_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    K056832_ram_word_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x302000 && address <= 0x303fff)
            {
                int offset = (address - 0x302000) / 2;
                if (address % 2 == 0)
                {
                    K056832_ram_word_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    K056832_ram_word_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x304000 && address <= 0x3041ff)
            {
                int offset = address - 0x304000;
                mainram6[offset] = (byte)value;
            }
            else if (address >= 0x330000 && address <= 0x331fff)
            {
                int offset = (address - 0x330000) / 2;
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
        public static void MWriteWord_viostorm(int address, short value)
        {
            address &= 0xffffff;
            if (address >= 0x200000 && address + 1 <= 0x20ffff)
            {
                int offset = address - 0x200000;
                Memory.mainram[offset] = (byte)(value >> 8);
                Memory.mainram[offset + 1] = (byte)value;
            }
            else if (address >= 0x210000 && address + 1 <= 0x210fff)
            {
                int offset = (address - 0x210000) / 2;
                K053247_word_w(offset, (ushort)value);
            }
            else if (address >= 0x211000 && address + 1 <= 0x21ffff)
            {
                int offset = address - 0x211000;
                mainram2[offset] = (byte)(value >> 8);
                mainram2[offset + 1] = (byte)value;
            }
            else if (address >= 0x240000 && address + 1 <= 0x240007)
            {
                int offset = (address - 0x240000) / 2;
                K053246_word_w(offset, (ushort)value);
            }
            else if (address >= 0x244010 && address + 1 <= 0x24401f)
            {
                int offset = (address - 0x244010) / 2;
                K053247_reg_word_w(offset, (ushort)value);
            }
            else if (address >= 0x24c000 && address + 1 <= 0x24ffff)
            {
                int offset = address - 0x24c000;
                mainram3[offset] = (byte)(value >> 8);
                mainram3[offset + 1] = (byte)value;
            }
            else if (address >= 0x250000 && address + 1 <= 0x25000f)
            {
                int offset = address - 0x250000;
                mainram4[offset] = (byte)(value >> 8);
                mainram4[offset + 1] = (byte)value;
            }
            else if (address >= 0x254000 && address + 1 <= 0x25401f)
            {
                int offset = (address - 0x254000) / 2;
                K054338_word_w(offset, (ushort)value);
            }
            else if (address >= 0x258000 && address + 1 <= 0x2580ff)
            {
                int offset = (address - 0x258000) / 2;
                K055555_word_w(offset, (ushort)value);
            }
            else if (address >= 0x25c000 && address + 1 <= 0x25c03f)
            {
                int offset = (address - 0x25c000) / 2;
                K055550_word_w(offset, (ushort)value);
            }
            else if (address >= 0x260000 && address + 1 <= 0x26001f)
            {
                int offset = (address - 0x260000) / 2;
                K053252_word_w(offset, (ushort)value);
            }
            else if (address >= 0x264000 && address + 1 <= 0x264001)
            {
                sound_irq_w();
            }
            else if (address >= 0x26800c && address + 1 <= 0x26800d)
            {
                sound_cmd1_w((ushort)value);
            }
            else if (address >= 0x26800e && address + 1 <= 0x26800f)
            {
                sound_cmd2_w((ushort)value);
            }
            else if (address >= 0x268000 && address + 1 <= 0x26801f)
            {
                int offset = address - 0x268000;
                mainram5[offset] = (byte)(value >> 8);
                mainram5[offset + 1] = (byte)value;
            }
            else if (address >= 0x26C000 && address + 1 <= 0x26C007)
            {
                int offset = (address - 0x26C000) / 2;
                K056832_b_word_w(offset, (ushort)value);
            }
            else if (address >= 0x270000 && address + 1 <= 0x27003f)
            {
                int offset = (address - 0x270000) / 2;
                K056832_word_w(offset, (ushort)value);
            }
            else if (address >= 0x27C000 && address + 1 <= 0x27C001)
            {
                mmeeprom_w((ushort)value);
            }
            else if (address >= 0x300000 && address + 1 <= 0x301fff)
            {
                int offset = (address - 0x300000) / 2;
                K056832_ram_word_w(offset, (ushort)value);
            }
            else if (address >= 0x302000 && address + 1 <= 0x303fff)
            {
                int offset = (address - 0x302000) / 2;
                K056832_ram_word_w(offset, (ushort)value);
            }
            else if (address >= 0x304000 && address + 1 <= 0x3041ff)
            {
                int offset = address - 0x304000;
                mainram6[offset] = (byte)(value >> 8);
                mainram6[offset + 1] = (byte)value;
            }
            else if (address >= 0x330000 && address + 1 <= 0x331fff)
            {
                int offset = (address - 0x330000) / 2;
                Generic.paletteram16_xrgb_word_be_w(offset, (ushort)value);
            }
        }
        public static void MWriteLong_viostorm(int address, int value)
        {
            MWriteWord_viostorm(address, (short)(value >> 16));
            MWriteWord_viostorm(address + 2, (short)value);
        }
        public static sbyte MReadOpByte_metamrph(int address)
        {
            address &= 0xffffff;
            sbyte result = 0;
            if (address >= 0x000000 && address <= 0x1fffff)
            {
                result = (sbyte)Memory.mainrom[address];
            }
            return result;
        }
        public static sbyte MReadByte_metamrph(int address)
        {
            address &= 0xffffff;
            sbyte result = 0;
            if (address >= 0x000000 && address <= 0x1fffff)
            {
                result = (sbyte)Memory.mainrom[address];
            }
            else if (address >= 0x200000 && address <= 0x20ffff)
            {
                int offset = address - 0x200000;
                result = (sbyte)Memory.mainram[offset];
            }
            else if (address >= 0x210000 && address <= 0x210fff)
            {
                int offset = (address - 0x210000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(K053247_word_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)K053247_word_r(offset);
                }
            }
            else if (address >= 0x211000 && address <= 0x21ffff)
            {
                int offset = address - 0x211000;
                result = (sbyte)mainram2[offset];
            }
            else if (address >= 0x244000 && address <= 0x24400f)
            {
                int offset = (address - 0x244000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(K055673_rom_word_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)K055673_rom_word_r(offset);
                }
            }
            else if (address >= 0x24c000 && address <= 0x24ffff)
            {
                int offset = (address - 0x24c000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(K053250_0_ram_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)K053250_0_ram_r(offset);
                }
            }
            else if (address >= 0x250000 && address <= 0x25000f)
            {
                int offset = (address - 0x250000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(K053250_0_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)K053250_0_r(offset);
                }
            }
            else if (address >= 0x268014 && address <= 0x268015)
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
            else if (address >= 0x268000 && address <= 0x26801f)
            {
                int offset = address - 0x268000;
                result = (sbyte)mainram3[offset];
            }
            else if (address >= 0x274000 && address <= 0x274001)
            {
                if (address % 2 == 0)
                {
                    result = (sbyte)sbyte3;
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)sbyte1;
                }
            }
            else if (address >= 0x274002 && address <= 0x274003)
            {
                if (address % 2 == 0)
                {
                    result = (sbyte)(sbyte4 >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)sbyte2;
                }
            }
            else if (address >= 0x278000 && address <= 0x278001)
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
            else if (address >= 0x278002 && address <= 0x278003)
            {
                if (address % 2 == 0)
                {
                    result = (sbyte)(vseeprom_r() >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)vseeprom_r();
                }
            }
            else if (address >= 0x27c000 && address <= 0x27c001)
            {
                result = 0;
            }
            else if (address >= 0x300000 && address <= 0x301fff)
            {
                int offset = (address - 0x300000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(K056832_ram_word_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)K056832_ram_word_r(offset);
                }
            }
            else if (address >= 0x302000 && address <= 0x303fff)
            {
                int offset = (address - 0x302000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(K056832_ram_word_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)K056832_ram_word_r(offset);
                }
            }
            else if (address >= 0x310000 && address <= 0x311fff)
            {
                int offset = (address - 0x310000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(K056832_mw_rom_word_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)K056832_mw_rom_word_r(offset);
                }
            }
            else if (address >= 0x320000 && address <= 0x321fff)
            {
                int offset = (address - 0x320000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(K053250_0_rom_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)K053250_0_rom_r(offset);
                }
            }
            else if (address >= 0x330000 && address <= 0x331fff)
            {
                int offset = (address - 0x330000) / 2;
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
        public static short MReadOpWord_metamrph(int address)
        {
            address &= 0xffffff;
            short result = 0;
            if (address >= 0x000000 && address + 1 <= 0x1fffff)
            {
                result = (short)(Memory.mainrom[address] * 0x100 + Memory.mainrom[address + 1]);
            }
            return result;
        }
        public static short MReadWord_metamrph(int address)
        {
            address &= 0xffffff;
            short result = 0;
            if (address >= 0x000000 && address <= 0x1fffff)
            {
                result = (short)(Memory.mainrom[address] * 0x100 + Memory.mainrom[address + 1]);
            }
            else if (address >= 0x200000 && address + 1 <= 0x20ffff)
            {
                int offset = address - 0x200000;
                result = (short)(Memory.mainram[offset] * 0x100 + Memory.mainram[offset + 1]);
            }
            else if (address >= 0x210000 && address + 1 <= 0x210fff)
            {
                int offset = (address - 0x210000) / 2;
                result = (short)K053247_word_r(offset);
            }
            else if (address >= 0x211000 && address + 1 <= 0x21ffff)
            {
                int offset = address - 0x211000;
                result = (short)(mainram2[offset] * 0x100 + mainram2[offset + 1]);
            }
            else if (address >= 0x244000 && address + 1 <= 0x24400f)
            {
                int offset = (address - 0x244000) / 2;
                result = (short)K055673_rom_word_r(offset);
            }
            else if (address >= 0x24c000 && address + 1 <= 0x24ffff)
            {
                int offset = (address - 0x24c000) / 2;
                result = (short)K053250_0_ram_r(offset);
            }
            else if (address >= 0x250000 && address + 1 <= 0x25000f)
            {
                int offset = (address - 0x250000) / 2;
                result = (short)K053250_0_r(offset);
            }
            else if (address >= 0x268014 && address + 1 <= 0x268015)
            {
                result = (short)sound_status_r();
            }
            else if (address >= 0x268000 && address + 1 <= 0x26801f)
            {
                int offset = address - 0x268000;
                result = (short)(mainram3[offset] * 0x100 + mainram3[offset + 1]);
            }
            else if (address >= 0x274000 && address + 1 <= 0x274001)
            {
                result = (short)((sbyte3 << 8) | (byte)sbyte1);
            }
            else if (address >= 0x274002 && address + 1 <= 0x274003)
            {
                result = (short)((sbyte4 << 8) | (byte)sbyte2);
            }
            else if (address >= 0x278000 && address + 1 <= 0x278001)
            {
                result = (short)mmcoins_r();
            }
            else if (address >= 0x278002 && address + 1 <= 0x278003)
            {
                result = (short)vseeprom_r();
            }
            else if (address >= 0x27c000 && address + 1 <= 0x27c001)
            {
                result = 0;
            }
            else if (address >= 0x300000 && address + 1 <= 0x301fff)
            {
                int offset = (address - 0x300000) / 2;
                result = (short)K056832_ram_word_r(offset);
            }
            else if (address >= 0x302000 && address + 1 <= 0x303fff)
            {
                int offset = (address - 0x302000) / 2;
                result = (short)K056832_ram_word_r(offset);
            }
            else if (address >= 0x310000 && address + 1 <= 0x311fff)
            {
                int offset = (address - 0x310000) / 2;
                result = (short)K056832_mw_rom_word_r(offset);
            }
            else if (address >= 0x320000 && address + 1 <= 0x321fff)
            {
                int offset = (address - 0x320000) / 2;
                result = (short)K053250_0_rom_r(offset);
            }
            else if (address >= 0x330000 && address + 1 <= 0x331fff)
            {
                int offset = (address - 0x330000) / 2;
                result = (short)Generic.paletteram16[offset];
            }
            return result;
        }
        public static int MReadOpLong_metamrph(int address)
        {
            int result = 0;
            result = (int)((ushort)MReadOpWord_metamrph(address) * 0x10000 + (ushort)MReadOpWord_metamrph(address + 2));
            return result;
        }
        public static int MReadLong_metamrph(int address)
        {
            int result = 0;
            result = (int)((ushort)MReadWord_metamrph(address) * 0x10000 + (ushort)MReadWord_metamrph(address + 2));
            return result;
        }
        public static void MWriteByte_metamrph(int address, sbyte value)
        {
            address &= 0xffffff;
            if (address >= 0x200000 && address <= 0x20ffff)
            {
                int offset = address - 0x200000;
                Memory.mainram[offset] = (byte)value;
            }
            else if (address >= 0x210000 && address <= 0x210fff)
            {
                int offset = (address - 0x210000) / 2;
                if (address % 2 == 0)
                {
                    K053247_word_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    K053247_word_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x211000 && address <= 0x21ffff)
            {
                int offset = address - 0x211000;
                mainram2[offset] = (byte)value;
            }
            else if (address >= 0x240000 && address <= 0x240007)
            {
                int offset = address - 0x240000;
                K053246_w(offset, (byte)value);
            }
            else if (address >= 0x244010 && address <= 0x24401f)
            {
                int offset = (address - 0x244010) / 2;
                if (address % 2 == 0)
                {
                    K053247_reg_word_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    K053247_reg_word_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x24c000 && address <= 0x24ffff)
            {
                int offset = (address - 0x24c000) / 2;
                if (address % 2 == 0)
                {
                    K053250_0_ram_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    K053250_0_ram_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x250000 && address <= 0x25000f)
            {
                int offset = (address - 0x250000) / 2;
                if (address % 2 == 1)
                {
                    K053250_0_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x254000 && address <= 0x25401f)
            {
                int offset = (address - 0x254000) / 2;
                if (address % 2 == 0)
                {
                    K054338_word_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    K054338_word_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x258000 && address <= 0x2580ff)
            {
                int offset = (address - 0x258000) / 2;
                K055555_word_w(offset, (byte)value);
            }
            else if (address >= 0x260000 && address <= 0x26001f)
            {
                int offset = (address - 0x260000) / 2;
                if (address % 2 == 0)
                {
                    K053252_word_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    K053252_word_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x264000 && address <= 0x264001)
            {
                sound_irq_w();
            }
            else if (address >= 0x26800c && address <= 0x26800d)
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
            else if (address >= 0x26800e && address <= 0x26800f)
            {
                if (address % 2 == 0)
                {
                    sound_cmd2_w(0);
                }
                else
                {
                    sound_cmd2_w((byte)value);
                }
            }
            else if (address >= 0x268000 && address <= 0x26801f)
            {
                int offset = address - 0x268000;
                mainram3[offset] = (byte)value;
            }
            else if (address >= 0x26C000 && address <= 0x26C007)
            {
                int offset = (address - 0x26C000) / 2;
                if (address % 2 == 0)
                {
                    K056832_b_word_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    K056832_b_word_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x270000 && address <= 0x27003f)
            {
                int offset = (address - 0x270000) / 2;
                if (address % 2 == 0)
                {
                    K056832_word_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    K056832_word_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x27C000 && address <= 0x27C001)
            {
                if (address % 2 == 1)
                {
                    mmeeprom_w2((byte)value);
                }
            }
            else if (address >= 0x300000 && address <= 0x301fff)
            {
                int offset = (address - 0x300000) / 2;
                if (address % 2 == 0)
                {
                    K056832_ram_word_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    K056832_ram_word_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x302000 && address <= 0x303fff)
            {
                int offset = (address - 0x302000) / 2;
                if (address % 2 == 0)
                {
                    K056832_ram_word_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    K056832_ram_word_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x330000 && address <= 0x331fff)
            {
                int offset = (address - 0x330000) / 2;
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
        public static void MWriteWord_metamrph(int address, short value)
        {
            address &= 0xffffff;
            if (address >= 0x200000 && address + 1 <= 0x20ffff)
            {
                int offset = address - 0x200000;
                Memory.mainram[offset] = (byte)(value >> 8);
                Memory.mainram[offset + 1] = (byte)value;
            }
            else if (address >= 0x210000 && address + 1 <= 0x210fff)
            {
                int offset = (address - 0x210000) / 2;
                K053247_word_w(offset, (ushort)value);
            }
            else if (address >= 0x211000 && address + 1 <= 0x21ffff)
            {
                int offset = address - 0x211000;
                mainram2[offset] = (byte)(value >> 8);
                mainram2[offset + 1] = (byte)value;
            }
            else if (address >= 0x240000 && address + 1 <= 0x240007)
            {
                int offset = (address - 0x240000) / 2;
                K053246_word_w(offset, (ushort)value);
            }
            else if (address >= 0x244010 && address + 1 <= 0x24401f)
            {
                int offset = (address - 0x244010) / 2;
                K053247_reg_word_w(offset, (ushort)value);
            }
            else if (address >= 0x24c000 && address + 1 <= 0x24ffff)
            {
                int offset = (address - 0x24c000) / 2;
                K053250_0_ram_w(offset, (ushort)value);
            }
            else if (address >= 0x250000 && address + 1 <= 0x25000f)
            {
                int offset = (address - 0x250000) / 2;
                K053250_0_w(offset, (ushort)value);
            }
            else if (address >= 0x254000 && address + 1 <= 0x25401f)
            {
                int offset = (address - 0x254000) / 2;
                K054338_word_w(offset, (ushort)value);
            }
            else if (address >= 0x258000 && address + 1 <= 0x2580ff)
            {
                int offset = (address - 0x258000) / 2;
                K055555_word_w(offset, (ushort)value);
            }
            else if (address >= 0x260000 && address + 1 <= 0x26001f)
            {
                int offset = (address - 0x260000) / 2;
                K053252_word_w(offset, (ushort)value);
            }
            else if (address >= 0x264000 && address + 1 <= 0x264001)
            {
                sound_irq_w();
            }
            else if (address >= 0x26800c && address + 1 <= 0x26800d)
            {
                sound_cmd1_w((ushort)value);
            }
            else if (address >= 0x26800e && address + 1 <= 0x26800f)
            {
                sound_cmd2_w((ushort)value);
            }
            else if (address >= 0x268000 && address + 1 <= 0x26801f)
            {
                int offset = address - 0x268000;
                mainram3[offset] = (byte)(value >> 8);
                mainram3[offset + 1] = (byte)value;
            }
            else if (address >= 0x26C000 && address + 1 <= 0x26C007)
            {
                int offset = (address - 0x26C000) / 2;
                K056832_b_word_w(offset, (ushort)value);
            }
            else if (address >= 0x270000 && address + 1 <= 0x27003f)
            {
                int offset = (address - 0x270000) / 2;
                K056832_word_w(offset, (ushort)value);
            }
            else if (address >= 0x27C000 && address + 1 <= 0x27C001)
            {
                mmeeprom_w((ushort)value);
            }
            else if (address >= 0x300000 && address + 1 <= 0x301fff)
            {
                int offset = (address - 0x300000) / 2;
                K056832_ram_word_w(offset, (ushort)value);
            }
            else if (address >= 0x302000 && address + 1 <= 0x303fff)
            {
                int offset = (address - 0x302000) / 2;
                K056832_ram_word_w(offset, (ushort)value);
            }
            else if (address >= 0x330000 && address + 1 <= 0x331fff)
            {
                int offset = (address - 0x330000) / 2;
                Generic.paletteram16_xrgb_word_be_w(offset, (ushort)value);
            }
        }
        public static void MWriteLong_metamrph(int address, int value)
        {
            MWriteWord_metamrph(address, (short)(value >> 16));
            MWriteWord_metamrph(address + 2, (short)value);
        }
        public static sbyte MReadOpByte_martchmp(int address)
        {
            address &= 0xffffff;
            sbyte result = 0;
            if (address >= 0x000000 && address <= 0x0fffff)
            {
                result = (sbyte)Memory.mainrom[address];
            }
            return result;
        }
        public static sbyte MReadByte_martchmp(int address)
        {
            address &= 0xffffff;
            sbyte result = 0;
            if (address >= 0x000000 && address <= 0x0fffff)
            {
                result = (sbyte)Memory.mainrom[address];
            }
            else if (address >= 0x100000 && address <= 0x10ffff)
            {
                int offset = address - 0x100000;
                result = (sbyte)Memory.mainram[offset];
            }
            else if (address >= 0x300000 && address <= 0x3fffff)
            {
                result = (sbyte)Memory.mainrom[address];
            }
            else if (address >= 0x402000 && address <= 0x40200f)
            {
                int offset = (address - 0x402000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(K055673_rom_word_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)K055673_rom_word_r(offset);
                }
            }
            else if (address >= 0x412000 && address <= 0x412001)
            {
                if (address % 2 == 0)
                {
                    result = (sbyte)(mccontrol_r() >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)mccontrol_r();
                }
            }
            else if (address >= 0x414000 && address <= 0x414001)
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
            else if (address >= 0x414002 && address <= 0x414003)
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
            else if (address >= 0x416000 && address <= 0x416001)
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
            else if (address >= 0x416002 && address <= 0x416003)
            {
                if (address % 2 == 1)
                {
                    result = (sbyte)mweeprom_r2();
                }
            }
            else if (address >= 0x418014 && address <= 0x418015)
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
            else if (address >= 0x418000 && address <= 0x41801f)
            {
                int offset = address - 0x418000;
                result = (sbyte)mainram2[offset];
            }
            else if (address >= 0x480000 && address <= 0x483fff)
            {
                int offset = (address - 0x480000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(K053247_martchmp_word_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)K053247_martchmp_word_r(offset);
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
            else if (address >= 0x680000 && address <= 0x681fff)
            {
                int offset = (address - 0x680000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(K056832_ram_word_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)K056832_ram_word_r(offset);
                }
            }
            else if (address >= 0x682000 && address <= 0x683fff)
            {
                int offset = (address - 0x682000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(K056832_ram_word_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)K056832_ram_word_r(offset);
                }
            }
            else if (address >= 0x700000 && address <= 0x703fff)
            {
                int offset = (address - 0x700000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(K056832_mw_rom_word_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)K056832_mw_rom_word_r(offset);
                }
            }
            return result;
        }
        public static short MReadOpWord_martchmp(int address)
        {
            address &= 0xffffff;
            short result = 0;
            if (address >= 0x000000 && address + 1 <= 0x0fffff)
            {
                result = (short)(Memory.mainrom[address] * 0x100 + Memory.mainrom[address + 1]);
            }
            return result;
        }
        public static short MReadWord_martchmp(int address)
        {
            address &= 0xffffff;
            short result = 0;
            if (address >= 0x000000 && address + 1 <= 0x0fffff)
            {
                result = (short)(Memory.mainrom[address] * 0x100 + Memory.mainrom[address + 1]);
            }
            else if (address >= 0x100000 && address + 1 <= 0x10ffff)
            {
                int offset = address - 0x100000;
                result = (short)(Memory.mainram[offset] * 0x100 + Memory.mainram[offset + 1]);
            }
            else if (address >= 0x300000 && address + 1 <= 0x3fffff)
            {
                result = (short)(Memory.mainrom[address] * 0x100 + Memory.mainrom[address + 1]);
            }
            else if (address >= 0x402000 && address + 1 <= 0x40200f)
            {
                int offset = (address - 0x402000) / 2;
                result = (short)K055673_rom_word_r(offset);
            }
            else if (address >= 0x412000 && address + 1 <= 0x412001)
            {
                result = (short)mccontrol_r();
            }
            else if (address >= 0x414000 && address + 1 <= 0x414001)
            {
                result = (short)((sbyte2 << 8) | (byte)sbyte1);
            }
            else if (address >= 0x414002 && address + 1 <= 0x414003)
            {
                result = (short)((sbyte4 << 8) | (byte)sbyte3);
            }
            else if (address >= 0x416000 && address + 1 <= 0x416001)
            {
                result = (short)mmcoins_r();
            }
            else if (address >= 0x416002 && address + 1 <= 0x416003)
            {
                result = (short)mweeprom_r();
            }
            else if (address >= 0x418014 && address + 1 <= 0x418015)
            {
                result = (short)sound_status_r();
            }
            else if (address >= 0x418000 && address + 1 <= 0x41801f)
            {
                int offset = address - 0x418000;
                result = (short)(mainram2[offset] * 0x100 + mainram2[offset + 1]);
            }
            else if (address >= 0x480000 && address + 1 <= 0x483fff)
            {
                int offset = (address - 0x480000) / 2;
                result = (short)K053247_martchmp_word_r(offset);
            }
            else if (address >= 0x600000 && address + 1 <= 0x601fff)
            {
                int offset = (address - 0x600000) / 2;
                result = (short)Generic.paletteram16[offset];
            }
            else if (address >= 0x680000 && address + 1 <= 0x681fff)
            {
                int offset = (address - 0x680000) / 2;
                result = (short)K056832_ram_word_r(offset);
            }
            else if (address >= 0x682000 && address + 1 <= 0x683fff)
            {
                int offset = (address - 0x682000) / 2;
                result = (short)K056832_ram_word_r(offset);
            }
            else if (address >= 0x700000 && address + 1 <= 0x703fff)
            {
                int offset = (address - 0x700000) / 2;
                result = (short)K056832_mw_rom_word_r(offset);
            }
            return result;
        }
        public static int MReadOpLong_martchmp(int address)
        {
            int result = 0;
            result = (int)((ushort)MReadOpWord_martchmp(address) * 0x10000 + (ushort)MReadOpWord_martchmp(address + 2));
            return result;
        }
        public static int MReadLong_martchmp(int address)
        {
            int result = 0;
            result = (int)((ushort)MReadWord_martchmp(address) * 0x10000 + (ushort)MReadWord_martchmp(address + 2));
            return result;
        }
        public static void MWriteByte_martchmp(int address, sbyte value)
        {
            address &= 0xffffff;
            if (address >= 0x100000 && address <= 0x10ffff)
            {
                int offset = address - 0x100000;
                Memory.mainram[offset] = (byte)value;
            }
            else if (address >= 0x400000 && address <= 0x4000ff)
            {
                int offset = (address - 0x400000) / 2;
                K055555_word_w(offset, (byte)value);
            }
            else if (address >= 0x402010 && address <= 0x40201f)
            {
                int offset = (address - 0x402010) / 2;
                if (address % 2 == 0)
                {
                    K053247_reg_word_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    K053247_reg_word_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x404000 && address <= 0x404007)
            {
                int offset = address - 0x404000;
                K053246_w(offset, (byte)value);
            }
            else if (address >= 0x40a000 && address <= 0x40a01f)
            {
                int offset = (address - 0x40a000) / 2;
                if (address % 2 == 0)
                {
                    K054338_word_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    K054338_word_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x40c000 && address <= 0x40c03f)
            {
                int offset = (address - 0x40c000) / 2;
                if (address % 2 == 0)
                {
                    K056832_word_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    K056832_word_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x40e000 && address <= 0x40e03f)
            {
                int offset = (address - 0x40e000) / 2;
                if (address % 2 == 0)
                {
                    K053990_martchmp_word_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    K053990_martchmp_word_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x410000 && address <= 0x410001)
            {
                if (address % 2 == 0)
                {
                    mweeprom_w1((byte)value);
                }
            }
            else if (address >= 0x412000 && address <= 0x412001)
            {
                if (address % 2 == 0)
                {
                    mccontrol_w1((byte)value);
                }
            }
            else if (address >= 0x41c000 && address <= 0x41c01f)
            {
                int offset = (address - 0x41c000) / 2;
                if (address % 2 == 0)
                {
                    K053252_word_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    K053252_word_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x41e000 && address <= 0x41e007)
            {
                int offset = (address - 0x41e000) / 2;
                if (address % 2 == 0)
                {
                    K056832_b_word_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    K056832_b_word_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x41800c && address <= 0x41800d)
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
            else if (address >= 0x41800e && address <= 0x41800f)
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
            else if (address >= 0x418000 && address <= 0x41801f)
            {
                int offset = address - 0x418000;
                mainram2[offset] = (byte)value;
            }
            else if (address >= 0x41a000 && address <= 0x41a001)
            {
                sound_irq_w();
            }
            else if (address >= 0x480000 && address <= 0x483fff)
            {
                int offset = (address - 0x480000) / 2;
                if (address % 2 == 0)
                {
                    K053247_martchmp_word_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    K053247_martchmp_word_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x600000 && address <= 0x601fff)
            {
                int offset = (address - 0x600000) / 2;
                if (address % 2 == 0)
                {
                    Generic.paletteram16_xrgb_word_be_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    Generic.paletteram16_xrgb_word_be_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x680000 && address <= 0x681fff)
            {
                int offset = (address - 0x680000) / 2;
                if (address % 2 == 0)
                {
                    K056832_ram_word_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    K056832_ram_word_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x682000 && address <= 0x683fff)
            {
                int offset = (address - 0x682000) / 2;
                if (address % 2 == 0)
                {
                    K056832_ram_word_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    K056832_ram_word_w2(offset, (byte)value);
                }
            }
        }
        public static void MWriteWord_martchmp(int address, short value)
        {
            address &= 0xffffff;
            if (address >= 0x100000 && address + 1 <= 0x10ffff)
            {
                int offset = address - 0x100000;
                Memory.mainram[offset] = (byte)(value >> 8);
                Memory.mainram[offset + 1] = (byte)value;
            }
            else if (address >= 0x400000 && address + 1 <= 0x4000ff)
            {
                int offset = (address - 0x400000) / 2;
                K055555_word_w(offset, (ushort)value);
            }
            else if (address >= 0x402010 && address + 1 <= 0x40201f)
            {
                int offset = (address - 0x402010) / 2;
                K053247_reg_word_w(offset, (ushort)value);
            }
            else if (address >= 0x404000 && address + 1 <= 0x404007)
            {
                int offset = (address - 0x404000) / 2;
                K053246_word_w(offset, (ushort)value);
            }
            else if (address >= 0x40a000 && address + 1 <= 0x40a01f)
            {
                int offset = (address - 0x40a000) / 2;
                K054338_word_w(offset, (ushort)value);
            }
            else if (address >= 0x40c000 && address + 1 <= 0x40c03f)
            {
                int offset = (address - 0x40c000) / 2;
                K056832_word_w(offset, (ushort)value);
            }
            else if (address >= 0x40e000 && address + 1 <= 0x40e03f)
            {
                int offset = (address - 0x40e000) / 2;
                K053990_martchmp_word_w(offset, (ushort)value);
            }
            else if (address >= 0x410000 && address + 1 <= 0x410001)
            {
                mweeprom_w((ushort)value);
            }
            else if (address >= 0x412000 && address + 1 <= 0x412001)
            {
                mccontrol_w((ushort)value);
            }
            else if (address >= 0x41c000 && address + 1 <= 0x41c01f)
            {
                int offset = (address - 0x41c000) / 2;
                K053252_word_w(offset, (ushort)value);
            }
            else if (address >= 0x41e000 && address + 1 <= 0x41e007)
            {
                int offset = (address - 0x41e000) / 2;
                K056832_b_word_w(offset, (ushort)value);
            }
            else if (address >= 0x41800c && address + 1 <= 0x41800d)
            {
                sound_cmd1_w((ushort)value);
            }
            else if (address >= 0x41800e && address + 1 <= 0x41800f)
            {
                sound_cmd2_w((ushort)value);
            }
            else if (address >= 0x418000 && address + 1 <= 0x41801f)
            {
                int offset = address - 0x418000;
                mainram2[offset] = (byte)(value >> 8);
                mainram2[offset + 1] = (byte)value;
            }
            else if (address >= 0x41a000 && address + 1 <= 0x41a001)
            {
                sound_irq_w();
            }
            else if (address >= 0x480000 && address + 1 <= 0x483fff)
            {
                int offset = (address - 0x480000) / 2;
                K053247_martchmp_word_w(offset, (ushort)value);
            }
            else if (address >= 0x600000 && address + 1 <= 0x601fff)
            {
                int offset = (address - 0x600000) / 2;
                Generic.paletteram16_xrgb_word_be_w(offset, (ushort)value);
            }
            else if (address >= 0x680000 && address + 1 <= 0x681fff)
            {
                int offset = (address - 0x680000) / 2;
                K056832_ram_word_w(offset, (ushort)value);
            }
            else if (address >= 0x682000 && address + 1 <= 0x683fff)
            {
                int offset = (address - 0x682000) / 2;
                K056832_ram_word_w(offset, (ushort)value);
            }
        }
        public static void MWriteLong_martchmp(int address, int value)
        {
            MWriteWord_martchmp(address, (short)(value >> 16));
            MWriteWord_martchmp(address + 2, (short)value);
        }
        public static sbyte MReadOpByte_gaiapols(int address)
        {
            address &= 0xffffff;
            sbyte result = 0;
            if (address >= 0x000000 && address <= 0x2fffff)
            {
                result = (sbyte)Memory.mainrom[address];
            }
            return result;
        }
        public static sbyte MReadByte_gaiapols(int address)
        {
            address &= 0xffffff;
            sbyte result = 0;
            if (address >= 0x000000 && address <= 0x2fffff)
            {
                result = (sbyte)Memory.mainrom[address];
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
            else if (address >= 0x410000 && address <= 0x411fff)
            {
                int offset = (address - 0x410000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(K056832_ram_word_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)K056832_ram_word_r(offset);
                }
            }
            else if (address >= 0x412000 && address <= 0x413fff)
            {
                int offset = (address - 0x412000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(K056832_ram_word_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)K056832_ram_word_r(offset);
                }
            }
            else if (address >= 0x420000 && address <= 0x421fff)
            {
                int offset = (address - 0x420000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(Generic.paletteram16[offset] >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)Generic.paletteram16[offset];
                }
            }
            else if (address >= 0x440000 && address <= 0x441fff)
            {
                int offset = (address - 0x440000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(K056832_mw_rom_word_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)K056832_mw_rom_word_r(offset);
                }
            }
            else if (address >= 0x450000 && address <= 0x45000f)
            {
                int offset = (address - 0x450000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(K055673_rom_word_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)K055673_rom_word_r(offset);
                }
            }
            else if (address >= 0x470000 && address <= 0x470fff)
            {
                int offset = (address - 0x470000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(K053936_0_linectrl[offset] >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)K053936_0_linectrl[offset];
                }
            }
            else if (address >= 0x48a014 && address <= 0x48a015)
            {
                if (address % 2 == 0)
                {
                    result = (sbyte)(sound_status_msb_r() >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)sound_status_msb_r();
                }
            }
            else if (address >= 0x48a000 && address <= 0x48a01f)
            {
                int offset = address - 0x48a000;
                result = (sbyte)mainram2[offset];
            }
            else if (address >= 0x48e000 && address <= 0x48e001)
            {
                if (address % 2 == 0)
                {
                    result = (sbyte)(dddcoins_r() >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)dddcoins_r();
                }
            }
            else if (address >= 0x48e020 && address <= 0x48e021)
            {
                ushort val = dddeeprom_r();
                if (address % 2 == 0)
                {
                    result = (sbyte)dddeeprom_r1();
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)dddeeprom_r2();
                }
            }
            else if (address >= 0x600000 && address <= 0x60ffff)
            {
                int offset = address - 0x600000;
                result = (sbyte)Memory.mainram[offset];
            }
            else if (address >= 0x660000 && address <= 0x6600ff)
            {
                int offset = (address - 0x660000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(K054000_lsb_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)K054000_lsb_r(offset);
                }
            }
            else if (address >= 0x800000 && address <= 0x87ffff)
            {
                int offset = (address - 0x800000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(gai_053936_tilerom_0_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)gai_053936_tilerom_0_r(offset);
                }
            }
            else if (address >= 0xa00000 && address <= 0xa7ffff)
            {
                int offset = (address - 0xa00000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(ddd_053936_tilerom_1_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)ddd_053936_tilerom_1_r(offset);
                }
            }
            else if (address >= 0xc00000 && address <= 0xdfffff)
            {
                int offset = (address - 0xc00000) / 2;
                if (address % 2 == 0)
                {
                    result = (sbyte)(gai_053936_tilerom_2_r(offset) >> 8);
                }
                else if (address % 2 == 1)
                {
                    result = (sbyte)gai_053936_tilerom_2_r(offset);
                }
            }
            return result;
        }
        public static short MReadOpWord_gaiapols(int address)
        {
            address &= 0xffffff;
            short result = 0;
            if (address >= 0x000000 && address + 1 <= 0x2fffff)
            {
                result = (short)(Memory.mainrom[address] * 0x100 + Memory.mainrom[address + 1]);
            }
            return result;
        }
        public static short MReadWord_gaiapols(int address)
        {
            address &= 0xffffff;
            short result = 0;
            if (address >= 0x000000 && address + 1 <= 0x2fffff)
            {
                result = (short)(Memory.mainrom[address] * 0x100 + Memory.mainrom[address + 1]);
            }
            else if (address >= 0x400000 && address + 1 <= 0x40ffff)
            {
                int offset = (address - 0x400000) / 2;
                result = (short)K053247_scattered_word_r(offset);
            }
            else if (address >= 0x410000 && address + 1 <= 0x411fff)
            {
                int offset = (address - 0x410000) / 2;
                result = (short)K056832_ram_word_r(offset);
            }
            else if (address >= 0x412000 && address + 1 <= 0x413fff)
            {
                int offset = (address - 0x412000) / 2;
                result = (short)K056832_ram_word_r(offset);
            }
            else if (address >= 0x420000 && address + 1 <= 0x421fff)
            {
                int offset = (address - 0x420000) / 2;
                result = (short)Generic.paletteram16[offset];
            }
            else if (address >= 0x440000 && address + 1 <= 0x441fff)
            {
                int offset = (address - 0x440000) / 2;
                result = (short)K056832_mw_rom_word_r(offset);
            }
            else if (address >= 0x450000 && address + 1 <= 0x45000f)
            {
                int offset = (address - 0x450000) / 2;
                result = (short)K055673_rom_word_r(offset);
            }
            else if (address >= 0x470000 && address + 1 <= 0x470fff)
            {
                int offset = (address - 0x470000) / 2;
                result = (short)K053936_0_linectrl[offset];
            }
            else if (address >= 0x48a014 && address + 1 <= 0x48a015)
            {
                result = (short)sound_status_msb_r();
            }
            else if (address >= 0x48a000 && address + 1 <= 0x48a01f)
            {
                int offset = address - 0x48a000;
                result = (short)(mainram2[offset] * 0x100 + mainram2[offset + 1]);
            }
            else if (address >= 0x48e000 && address + 1 <= 0x48e001)
            {
                result = (short)dddcoins_r();
            }
            else if (address >= 0x48e020 && address + 1 <= 0x48e021)
            {
                result = (short)dddeeprom_r();
            }
            else if (address >= 0x600000 && address + 1 <= 0x60ffff)
            {
                int offset = address - 0x600000;
                result = (short)(Memory.mainram[offset] * 0x100 + Memory.mainram[offset + 1]);
            }
            else if (address >= 0x660000 && address + 1 <= 0x6600ff)
            {
                int offset = (address - 0x660000) / 2;
                result = (short)K054000_lsb_r(offset);
            }
            else if (address >= 0x800000 && address + 1 <= 0x87ffff)
            {
                int offset = (address - 0x800000) / 2;
                result = (short)gai_053936_tilerom_0_r(offset);
            }
            else if (address >= 0xa00000 && address + 1 <= 0xa7ffff)
            {
                int offset = (address - 0xa00000) / 2;
                result = (short)ddd_053936_tilerom_1_r(offset);
            }
            else if (address >= 0xc00000 && address + 1 <= 0xdfffff)
            {
                int offset = (address - 0xc00000) / 2;
                result = (short)gai_053936_tilerom_2_r(offset);
            }
            return result;
        }
        public static int MReadOpLong_gaiapols(int address)
        {
            int result = 0;
            result = (int)((ushort)MReadOpWord_gaiapols(address) * 0x10000 + (ushort)MReadOpWord_gaiapols(address + 2));
            return result;
        }
        public static int MReadLong_gaiapols(int address)
        {
            int result = 0;
            result = (int)((ushort)MReadWord_gaiapols(address) * 0x10000 + (ushort)MReadWord_gaiapols(address + 2));
            return result;
        }
        public static void MWriteByte_gaiapols(int address, sbyte value)
        {
            address &= 0xffffff;
            if (address >= 0x400000 && address <= 0x40ffff)
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
            else if (address >= 0x410000 && address <= 0x411fff)
            {
                int offset = (address - 0x410000) / 2;
                if (address % 2 == 0)
                {
                    K056832_ram_word_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    K056832_ram_word_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x412000 && address <= 0x413fff)
            {
                int offset = (address - 0x412000) / 2;
                if (address % 2 == 0)
                {
                    K056832_ram_word_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    K056832_ram_word_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x420000 && address <= 0x421fff)
            {
                int offset = (address - 0x420000) / 2;
                if (address % 2 == 0)
                {
                    Generic.paletteram16_xrgb_word_be_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    Generic.paletteram16_xrgb_word_be_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x430000 && address <= 0x430007)
            {
                int offset = address - 0x430000;
                K053246_w(offset, (byte)value);
            }
            else if (address >= 0x450010 && address <= 0x45001f)
            {
                int offset = (address - 0x450010) / 2;
                if (address % 2 == 0)
                {
                    K053247_reg_word_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    K053247_reg_word_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x460000 && address <= 0x46001f)
            {
                int offset = (address - 0x460000) / 2;
                if (address % 2 == 0)
                {
                    K053936_0_ctrl[offset] = (ushort)((value << 8) | (K053936_0_ctrl[offset] & 0xff));
                }
                else if (address % 2 == 1)
                {
                    K053936_0_ctrl[offset] = (ushort)((K053936_0_ctrl[offset] & 0xff00) | (byte)value);
                }
            }
            else if (address >= 0x470000 && address <= 0x470fff)
            {
                int offset = (address - 0x470000) / 2;
                if (address % 2 == 0)
                {
                    K053936_0_linectrl[offset] = (ushort)((value << 8) | (K053936_0_linectrl[offset] & 0xff));
                }
                else if (address % 2 == 1)
                {
                    K053936_0_linectrl[offset] = (ushort)((K053936_0_linectrl[offset] & 0xff00) | (byte)value);
                }
            }
            else if (address >= 0x480000 && address <= 0x48003f)
            {
                int offset = (address - 0x480000) / 2;
                if (address % 2 == 0)
                {
                    K056832_word_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    K056832_word_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x482000 && address <= 0x482007)
            {
                int offset = (address - 0x482000) / 2;
                if (address % 2 == 0)
                {
                    K056832_b_word_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    K056832_b_word_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x484000 && address <= 0x484003)
            {
                int offset = (address - 0x484000) / 2;
                if (address % 2 == 0)
                {
                    ddd_053936_clip_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    ddd_053936_clip_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x486000 && address <= 0x48601f)
            {
                int offset = (address - 0x486000) / 2;
                if (address % 2 == 0)
                {
                    K053252_word_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    K053252_word_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x488000 && address <= 0x4880ff)
            {
                int offset = (address - 0x488000) / 2;
                K055555_word_w(offset, (byte)value);
            }
            else if (address >= 0x48a00c && address <= 0x48a00d)
            {
                if (address % 2 == 0)
                {
                    sound_cmd1_msb_w1((byte)value);
                }
            }
            else if (address >= 0x48a00e && address <= 0x48a00f)
            {
                if (address % 2 == 0)
                {
                    sound_cmd2_msb_w1((byte)value);
                }
            }
            else if (address >= 0x48a000 && address <= 0x48a01f)
            {
                int offset = address - 0x48a000;
                mainram2[offset] = (byte)value;
            }
            else if (address >= 0x48c000 && address <= 0x48c01f)
            {
                int offset = (address - 0x48c000) / 2;
                if (address % 2 == 0)
                {
                    K054338_word_w1(offset, (byte)value);
                }
                else if (address % 2 == 1)
                {
                    K054338_word_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x600000 && address <= 0x60ffff)
            {
                int offset = address - 0x600000;
                Memory.mainram[offset] = (byte)value;
            }
            else if (address >= 0x660000 && address <= 0x6600ff)
            {
                int offset = (address - 0x660000) / 2;
                if (address % 2 == 1)
                {
                    K054000_lsb_w2(offset, (byte)value);
                }
            }
            else if (address >= 0x6a0000 && address <= 0x6a0001)
            {
                if (address % 2 == 1)
                {
                    mmeeprom_w2((byte)value);
                }
            }
            else if (address >= 0x6c0000 && address <= 0x6c0001)
            {
                if (address % 2 == 0)
                {
                    ddd_053936_enable_w1((byte)value);
                }
            }
            else if (address >= 0x6e0000 && address <= 0x6e0001)
            {
                sound_irq_w();
            }
            else if (address >= 0xe00000 && address <= 0xe00001)
            {

            }
        }
        public static void MWriteWord_gaiapols(int address, short value)
        {
            address &= 0xffffff;
            if (address >= 0x400000 && address + 1 <= 0x40ffff)
            {
                int offset = (address - 0x400000) / 2;
                K053247_scattered_word_w(offset, (ushort)value);
            }
            else if (address >= 0x410000 && address + 1 <= 0x411fff)
            {
                int offset = (address - 0x410000) / 2;
                K056832_ram_word_w(offset, (ushort)value);
            }
            else if (address >= 0x412000 && address + 1 <= 0x413fff)
            {
                int offset = (address - 0x412000) / 2;
                K056832_ram_word_w(offset, (ushort)value);
            }
            else if (address >= 0x420000 && address + 1 <= 0x421fff)
            {
                int offset = (address - 0x420000) / 2;
                Generic.paletteram16_xrgb_word_be_w(offset, (ushort)value);
            }
            else if (address >= 0x430000 && address + 1 <= 0x430007)
            {
                int offset = (address - 0x430000) / 2;
                K053246_word_w(offset, (ushort)value);
            }
            else if (address >= 0x450010 && address + 1 <= 0x45001f)
            {
                int offset = (address - 0x450010) / 2;
                K053247_reg_word_w(offset, (ushort)value);
            }
            else if (address >= 0x460000 && address + 1 <= 0x46001f)
            {
                int offset = (address - 0x460000) / 2;
                K053936_0_ctrl[offset] = (ushort)value;
            }
            else if (address >= 0x470000 && address + 1 <= 0x470fff)
            {
                int offset = (address - 0x470000) / 2;
                K053936_0_linectrl[offset] = (ushort)value;
            }
            else if (address >= 0x480000 && address + 1 <= 0x48003f)
            {
                int offset = (address - 0x480000) / 2;
                K056832_word_w(offset, (ushort)value);
            }
            else if (address >= 0x482000 && address + 1 <= 0x482007)
            {
                int offset = (address - 0x482000) / 2;
                K056832_b_word_w(offset, (ushort)value);
            }
            else if (address >= 0x484000 && address + 1 <= 0x484003)
            {
                int offset = (address - 0x484000) / 2;
                ddd_053936_clip_w(offset, (ushort)value);
            }
            else if (address >= 0x486000 && address + 1 <= 0x48601f)
            {
                int offset = (address - 0x486000) / 2;
                K053252_word_w(offset, (ushort)value);
            }
            else if (address >= 0x488000 && address + 1 <= 0x4880ff)
            {
                int offset = (address - 0x488000) / 2;
                K055555_word_w(offset, (ushort)value);
            }
            else if (address >= 0x48a00c && address + 1 <= 0x48a00d)
            {
                sound_cmd1_msb_w((ushort)value);
            }
            else if (address >= 0x48a00e && address + 1 <= 0x48a00f)
            {
                sound_cmd2_msb_w((ushort)value);
            }
            else if (address >= 0x48a000 && address + 1 <= 0x48a01f)
            {
                int offset = address - 0x48a000;
                mainram2[offset] = (byte)(value >> 8);
                mainram2[offset + 1] = (byte)value;
            }
            else if (address >= 0x48c000 && address + 1 <= 0x48c01f)
            {
                int offset = (address - 0x48c000) / 2;
                K054338_word_w(offset, (ushort)value);
            }
            else if (address >= 0x600000 && address + 1 <= 0x60ffff)
            {
                int offset = address - 0x600000;
                Memory.mainram[offset] = (byte)(value >> 8);
                Memory.mainram[offset + 1] = (byte)value;
            }
            else if (address >= 0x660000 && address + 1 <= 0x6600ff)
            {
                int offset = (address - 0x660000) / 2;
                K054000_lsb_w(offset, (ushort)value);
            }
            else if (address >= 0x6a0000 && address + 1 <= 0x6a0001)
            {
                mmeeprom_w((ushort)value);
            }
            else if (address >= 0x6c0000 && address + 1 <= 0x6c0001)
            {
                ddd_053936_enable_w((ushort)value);
            }
            else if (address >= 0x6e0000 && address + 1 <= 0x6e0001)
            {
                sound_irq_w();
            }
            else if (address >= 0xe00000 && address + 1 <= 0xe00001)
            {

            }
        }
        public static void MWriteLong_gaiapols(int address, int value)
        {
            MWriteWord_gaiapols(address, (short)(value >> 16));
            MWriteWord_gaiapols(address + 2, (short)value);
        }
        public static byte ZReadOp_moo(ushort address)
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
        public static byte ZReadMemory_moo(ushort address)
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
            else if (address == 0xec01)
            {
                result = YM2151.ym2151_status_port_0_r();
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
        public static void ZWriteMemory_moo(ushort address, byte value)
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
            else if (address == 0xec00)
            {
                YM2151.ym2151_register_port_0_w(value);
            }
            else if (address == 0xec01)
            {
                YM2151.ym2151_data_port_0_w(value);
            }
            else if (address == 0xf000)
            {
                Sound.soundlatch3_w(value);
            }
            else if (address == 0xf800)
            {
                sound_bankswitch_w(value);
            }
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
