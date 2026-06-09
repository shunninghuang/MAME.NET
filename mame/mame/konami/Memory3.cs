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
    }
}
