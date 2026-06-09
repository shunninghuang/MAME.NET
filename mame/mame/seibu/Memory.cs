using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cpu.m6800;

namespace mame
{
    public partial class Seibu
    {
        public static byte byte1, byte2, bytes;
        public static byte byte1_old, byte2_old, bytes_old;
        public static byte Z0ReadOp_kncljoe(ushort address)
        {
            byte result = 0;
            if (address >= 0x0000 && address <= 0xbfff)
            {
                result = Memory.mainrom[address];
            }
            return result;
        }
        public static byte Z0ReadMemory_kncljoe(ushort address)
        {
            byte result = 0;
            if (address >= 0x0000 && address <= 0xbfff)
            {
                result = Memory.mainrom[address];
            }
            else if (address >= 0xc000 && address <= 0xcfff)
            {
                int offset = address - 0xc000;
                result = Generic.videoram[offset];
            }
            else if (address == 0xd800)
            {
                result = bytes;
            }
            else if (address == 0xd801)
            {
                result = byte1;
            }
            else if (address == 0xd802)
            {
                result = byte2;
            }
            else if (address == 0xd803)
            {
                result = dswa;
            }
            else if (address == 0xd804)
            {
                result = dswb;
            }
            else if (address == 0xd807)
            {
                result = 0;
            }
            else if (address == 0xd817)
            {
                result = 0;
            }
            else if (address >= 0xe800 && address <= 0xefff)
            {
                int offset = address - 0xe800;
                result = Generic.spriteram[offset];
            }
            else if (address >= 0xf000 && address <= 0xffff)
            {
                int offset = address - 0xf000;
                result = Memory.mainram[offset];
            }
            return result;
        }
        public static void Z0WriteMemory_kncljoe(ushort address, byte value)
        {
            if (address >= 0x0000 && address <= 0xbfff)
            {
                Memory.audiorom[address] = value;
            }
            else if (address >= 0xc000 && address <= 0xcfff)
            {
                int offset = address - 0xc000;
                kncljoe_videoram_w(offset, value);
            }
            else if (address >= 0xd000 && address <= 0xd001)
            {
                int offset = address - 0xd000;
                kncljoe_scroll_w(offset, value);
            }
            else if (address == 0xd800)
            {
                sound_cmd_w(value);
            }
            else if (address == 0xd801)
            {
                kncljoe_control_w(value);
            }
            else if (address == 0xd802)
            {
                SN76496.ss1[0].Write(value);
            }
            else if (address == 0xd803)
            {
                SN76496.ss1[1].Write(value);
            }
            else if (address >= 0xe800 && address <= 0xefff)
            {
                int offset = address - 0xe800;
                Generic.spriteram[offset] = value;
            }
            else if (address >= 0xf000 && address <= 0xffff)
            {
                int offset = address - 0xf000;
                Memory.mainram[offset] = value;
            }
        }
        public static byte M1ReadByte_kncljoe(ushort address)
        {
            byte result = 0;
            address &= 0x7fff;
            if (address >= 0x0000 && address <= 0x001f)
            {
                result = M6800.m1.m6803_internal_registers_r(address);
            }
            else if (address >= 0x0020 && address <= 0x007f)
            {
                result = 0;
            }
            else if (address >= 0x0080 && address <= 0x00ff)
            {
                int offset = address - 0x0080;
                result = Memory.audioram[offset];
            }
            else if (address >= 0x2000 && address <= 0x7fff)
            {
                result = Memory.audiorom[address];
            }
            return result;
        }
        public static void M1WriteByte_kncljoe(ushort address, byte value)
        {
            address &= 0x7fff;
            if (address >= 0x0000 && address <= 0x001f)
            {
                M6800.m1.m6803_internal_registers_w(address, value);
            }
            else if (address >= 0x0020 && address <= 0x007f)
            {
                
            }
            else if (address >= 0x0080 && address <= 0x00ff)
            {
                int offset = address - 0x0080;
                Memory.audioram[offset] = value;
            }
            else if (address >= 0x1000 && address <= 0x1fff)
            {
                sound_irq_ack_w();
            }
        }
        public static byte M1ReadIO_kncljoe(ushort address)
        {
            byte result = 0;
            if (address == 0x100)
            {
                result = m6803_port1_r();
            }
            else if (address == 0x101)
            {
                result = m6803_port2_r();
            }
            return result;
        }
        public static void M1WriteIO_kncljoe(ushort address, byte value)
        {
            if (address == 0x100)
            {
                m6803_port1_w(value);
            }
            else if (address == 0x101)
            {
                m6803_port2_w(value);
            }
        }
    }
}
