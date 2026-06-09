using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mame
{
    public partial class Kaneko
    {
        public static byte byte1, byte2, bytes;
        public static byte byte1_old, byte2_old, bytes_old;
        public static byte Z0ReadOp_airbustr(ushort address)
        {
            byte result = 0;
            if (address >= 0x0000 && address <= 0x7fff)
            {
                result = Memory.mainrom[address];
            }
            else if (address >= 0x8000 && address <= 0xbfff)
            {
                int offset = address - 0x8000;
                result = bank1[basebankmaster + offset];
            }
            return result;
        }
        public static byte Z0ReadMemory_airbustr(ushort address)
        {
            byte result = 0;
            if (address >= 0x0000 && address <= 0x7fff)
            {
                result = Memory.mainrom[address];
            }
            else if (address >= 0x8000 && address <= 0xbfff)
            {
                int offset = address - 0x8000;
                result = bank1[basebankmaster + offset];
            }
            else if (address >= 0xc000 && address <= 0xcfff)
            {
                int offset = address - 0xc000;
                result = pandora_spriteram_r(offset);
            }
            else if (address >= 0xd000 && address <= 0xdfff)
            {
                int offset = address - 0xd000;
                result = Memory.mainram[offset];
            }
            else if (address >= 0xe000 && address <= 0xefff)
            {
                int offset = address - 0xe000;
                result = devram_r(offset);
            }
            else if (address >= 0xf000 && address <= 0xffff)
            {
                int offset = address - 0xf000;
                result = sharedram[offset];
            }
            return result;
        }
        public static byte Z0ReadMemory_airbustrb(ushort address)
        {
            byte result = 0;
            if (address >= 0x0000 && address <= 0x7fff)
            {
                result = Memory.mainrom[address];
            }
            else if (address >= 0x8000 && address <= 0xbfff)
            {
                int offset = address - 0x8000;
                result = bank1[basebankmaster + offset];
            }
            else if (address >= 0xc000 && address <= 0xcfff)
            {
                int offset = address - 0xc000;
                result = pandora_spriteram_r(offset);
            }
            else if (address >= 0xd000 && address <= 0xdfff)
            {
                int offset = address - 0xd000;
                result = Memory.mainram[offset];
            }
            else if (address >= 0xe000 && address <= 0xefff)
            {
                int offset = address - 0xe000;
                result = devram[offset];
            }
            else if (address >= 0xf000 && address <= 0xffff)
            {
                int offset = address - 0xf000;
                result = sharedram[offset];
            }
            return result;
        }
        public static void Z0WriteMemory_airbustr(ushort address, byte value)
        {
            if (address >= 0x0000 && address <= 0x7fff)
            {
                Memory.audiorom[address] = value;
            }
            else if (address >= 0x8000 && address <= 0xbfff)
            {
                int offset = address - 0x8000;
                bank1[basebankmaster + offset] = value;
            }
            else if (address >= 0xc000 && address <= 0xcfff)
            {
                int offset = address - 0xc000;
                pandora_spriteram_w(offset, value);
            }
            else if (address >= 0xd000 && address <= 0xdfff)
            {
                int offset = address - 0xd000;
                Memory.mainram[offset] = value;
            }
            else if (address >= 0xe000 && address <= 0xefff)
            {
                int offset = address - 0xe000;
                devram[offset]= value;
            }
            else if (address >= 0xf000 && address <= 0xffff)
            {
                int offset = address - 0xf000;
                sharedram[offset] = value;
            }
        }
        public static byte Z0ReadHardware(ushort address)
        {
            return 0;
        }
        public static void Z0WriteHardware(ushort address, byte value)
        {
            address &= 0xff;
            if (address == 0x00)
            {
                master_bankswitch_w(value);
            }
            else if (address == 0x01)
            {

            }
            else if (address == 0x02)
            {
                master_nmi_trigger_w();
            }
        }
        public static byte Z1ReadOp_airbustr(ushort address)
        {
            byte result = 0;
            if (address >= 0x0000 && address <= 0x7fff)
            {
                result = slaverom[address];
            }
            else if (address >= 0x8000 && address <= 0xbfff)
            {
                int offset = address - 0x8000;
                result = bank2[basebankslave + offset];
            }
            return result;
        }
        public static byte Z1ReadMemory_airbustr(ushort address)
        {
            byte result = 0;
            if (address >= 0x0000 && address <= 0x7fff)
            {
                result = slaverom[address];
            }
            else if (address >= 0x8000 && address <= 0xbfff)
            {
                int offset = address - 0x8000;
                result = bank2[basebankslave + offset];
            }
            else if (address >= 0xc000 && address <= 0xc3ff)
            {
                int offset = address - 0xc000;
                result = airbustr_videoram2[offset];
            }
            else if (address >= 0xc400 && address <= 0xc7ff)
            {
                int offset = address - 0xc400;
                result = airbustr_colorram2[offset];
            }
            else if (address >= 0xc800 && address <= 0xcbff)
            {
                int offset = address - 0xc800;
                result = Generic.videoram[offset];
            }
            else if (address >= 0xcc00 && address <= 0xcfff)
            {
                int offset = address - 0xcc00;
                result = Generic.colorram[offset];
            }
            else if (address >= 0xd000 && address <= 0xd5ff)
            {
                int offset = address - 0xd000;
                result = Generic.paletteram[offset];
            }
            else if (address >= 0xd600 && address <= 0xefff)
            {
                int offset = address - 0xd600;
                result = slaveram[offset];
            }
            else if (address >= 0xf000 && address <= 0xffff)
            {
                int offset = address - 0xf000;
                result = sharedram[offset];
            }
            return result;
        }
        public static void Z1WriteMemory_airbustr(ushort address, byte value)
        {
            if (address >= 0x0000 && address <= 0x7fff)
            {
                slaverom[address] = value;
            }
            else if (address >= 0x8000 && address <= 0xbfff)
            {
                int offset = address - 0x8000;
                bank2[basebankslave + offset] = value;
            }
            else if (address >= 0xc000 && address <= 0xc3ff)
            {
                int offset = address - 0xc000;
                airbustr_videoram2_w(offset, value);
            }
            else if (address >= 0xc400 && address <= 0xc7ff)
            {
                int offset = address - 0xc400;
                airbustr_colorram2_w(offset, value);
            }
            else if (address >= 0xc800 && address <= 0xcbff)
            {
                int offset = address - 0xc800;
                airbustr_videoram_w(offset, value);
            }
            else if (address >= 0xcc00 && address <= 0xcfff)
            {
                int offset = address - 0xcc00;
                airbustr_colorram_w(offset, value);
            }
            else if (address >= 0xd000 && address <= 0xd5ff)
            {
                int offset = address - 0xd000;
                airbustr_paletteram_w(offset, value);
            }
            else if (address >= 0xd600 && address <= 0xefff)
            {
                int offset = address - 0xd600;
                slaveram[offset] = value;
            }
            else if (address >= 0xf000 && address <= 0xffff)
            {
                int offset = address - 0xf000;
                sharedram[offset] = value;
            }
        }
        public static byte Z1ReadHardware(ushort address)
        {
            byte result = 0;
            address &= 0xff;
            if (address == 0x02)
            {
                result = soundcommand2_r();
            }
            else if (address == 0x0e)
            {
                result = soundcommand_status_r();
            }
            else if (address == 0x20)
            {
                result = byte1;
            }
            else if (address == 0x22)
            {
                result = byte2;
            }
            else if (address == 0x24)
            {
                result = bytes;
            }
            return result;
        }
        public static void Z1WriteHardware(ushort address, byte value)
        {
            address &= 0xff;
            if (address == 0x00)
            {
                slave_bankswitch_w(value);
            }            
            else if (address == 0x02)
            {
                soundcommand_w(value);
            }
            else if (address >= 0x04 && address <= 0x0c)
            {
                int offset = address - 0x04;
                airbustr_scrollregs_w(offset, value);
            }
            else if (address == 0x28)
            {
                airbustr_coin_counter_w(value);
            }
            else if (address == 0x38)
            {

            }
        }
        public static byte Z2ReadOp_airbustr(ushort address)
        {
            byte result = 0;
            if (address >= 0x0000 && address <= 0x7fff)
            {
                result = Memory.audiorom[address];
            }
            else if (address >= 0x8000 && address <= 0xbfff)
            {
                int offset = address - 0x8000;
                result = bank3[basebankaudio + offset];
            }
            return result;
        }
        public static byte Z2ReadMemory_airbustr(ushort address)
        {
            byte result = 0;
            if (address >= 0x0000 && address <= 0x7fff)
            {
                result = Memory.audiorom[address];
            }
            else if (address >= 0x8000 && address <= 0xbfff)
            {
                int offset = address - 0x8000;
                result = bank3[basebankaudio + offset];
            }
            else if (address >= 0xc000 && address <= 0xdfff)
            {
                int offset = address - 0xc000;
                result = Memory.audioram[offset];
            }
            return result;
        }
        public static void Z2WriteMemory_airbustr(ushort address, byte value)
        {
            if (address >= 0x0000 && address <= 0x7fff)
            {
                Memory.audiorom[address] = value;
            }
            else if (address >= 0x8000 && address <= 0xbfff)
            {
                int offset = address - 0x8000;
                bank3[basebankaudio + offset] = value;
            }
            else if (address >= 0xc000 && address <= 0xdfff)
            {
                int offset = address - 0xc000;
                Memory.audioram[offset] = value;
            }
        }
        public static byte Z2ReadHardware(ushort address)
        {
            byte result = 0;
            address &= 0xff;
            if (address == 0x02)
            {
                result = YM2203.ym2203_status_port_0_r();
            }
            else if (address == 0x03)
            {
                result = YM2203.ym2203_read_port_0_r();
            }
            else if (address == 0x04)
            {
                result = (byte)OKI6295.oo1[0].okim6295_status_r();
            }
            else if (address == 0x06)
            {
                result = soundcommand_r();
            }
            return result;
        }
        public static void Z2WriteHardware(ushort address, byte value)
        {
            address &= 0xff;
            if (address == 0x00)
            {
                sound_bankswitch_w(value);
            }
            else if (address == 0x02)
            {
                YM2203.ym2203_control_port_0_w(value);
            }
            else if (address == 0x03)
            {
                YM2203.ym2203_write_port_0_w(value);
            }
            else if (address == 0x04)
            {
                OKI6295.oo1[0].okim6295_data_w(value);
            }
            else if (address == 0x06)
            {
                soundcommand2_w(value);
            }
        }
    }
}
