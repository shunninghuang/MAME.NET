using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using cpu.z80;
using cpu.m6800;

namespace mame
{
    public partial class Seibu
    {
        public static void SaveStateBinary_kncljoe(BinaryWriter writer)
        {
            int i;
            writer.Write(dswa);
            writer.Write(dswb);
            writer.Write(port1);
            writer.Write(port2);
            writer.Write(tile_bank);
            writer.Write(sprite_bank);
            writer.Write(flipscreen);
            writer.Write(kncljoe_scrollregs, 0, 2);
            writer.Write(Generic.videoram, 0, 0x1000);
            writer.Write(Generic.spriteram, 0, 0x800);
            for (i = 0; i < 0x100; i++)
            {
                writer.Write(Palette.entry_color2[i]);
            }
            writer.Write(Memory.mainram, 0, 0x1000);
            writer.Write(Memory.audioram, 0, 0x80);
            Z80A.zz1[0].SaveStateBinary(writer);
            M6800.m1.SaveStateBinary(writer);
            Cpuint.SaveStateBinary(writer);
            writer.Write(Timer.global_basetime.seconds);
            writer.Write(Timer.global_basetime.attoseconds);
            writer.Write(Video.screenstate.vblank_start_time.seconds);
            writer.Write(Video.screenstate.vblank_start_time.attoseconds);
            writer.Write(Video.screenstate.frame_number);
            writer.Write(Sound.last_update_second);
            Cpuexec.SaveStateBinary(writer);
            Timer.SaveStateBinary(writer);
            AY8910.AA8910[0].SaveStateBinary(writer);
            SN76496.ss1[0].SaveStateBinary(writer);
            SN76496.ss1[1].SaveStateBinary(writer);
            writer.Write(Sound.latched_value[0]);
            writer.Write(Sound.utempdata[0]);
            writer.Write(AY8910.AA8910[0].stream.output_sampindex);
            writer.Write(AY8910.AA8910[0].stream.output_base_sampindex);
            writer.Write(SN76496.ss1[0].Channel.output_sampindex);
            writer.Write(SN76496.ss1[0].Channel.output_base_sampindex);
            writer.Write(SN76496.ss1[1].Channel.output_sampindex);
            writer.Write(SN76496.ss1[1].Channel.output_base_sampindex);
            writer.Write(Sound.mixerstream.output_sampindex);
            writer.Write(Sound.mixerstream.output_base_sampindex);
        }
        public static void LoadStateBinary_kncljoe(BinaryReader reader)
        {
            int i;
            dswa = reader.ReadByte();
            dswb = reader.ReadByte();
            port1 = reader.ReadByte();
            port2 = reader.ReadByte();
            tile_bank = reader.ReadInt32();
            sprite_bank = reader.ReadInt32();
            flipscreen = reader.ReadInt32();
            kncljoe_scrollregs = reader.ReadBytes(2);
            Generic.videoram = reader.ReadBytes(0x1000);
            Generic.spriteram = reader.ReadBytes(0x800);
            for (i = 0; i < 0x100; i++)
            {
                Palette.entry_color2[i] = reader.ReadUInt32();
            }
            Memory.mainram = reader.ReadBytes(0x1000);
            Memory.audioram = reader.ReadBytes(0x800);
            Z80A.zz1[0].LoadStateBinary(reader);
            M6800.m1.LoadStateBinary(reader);
            Cpuint.LoadStateBinary(reader);
            Timer.global_basetime.seconds = reader.ReadInt32();
            Timer.global_basetime.attoseconds = reader.ReadInt64();
            Video.screenstate.vblank_start_time.seconds = reader.ReadInt32();
            Video.screenstate.vblank_start_time.attoseconds = reader.ReadInt64();
            Video.screenstate.frame_number = reader.ReadInt64();
            Sound.last_update_second = reader.ReadInt32();
            Cpuexec.LoadStateBinary(reader);
            Timer.LoadStateBinary(reader);
            AY8910.AA8910[0].LoadStateBinary(reader);
            SN76496.ss1[0].LoadStateBinary(reader);
            SN76496.ss1[1].LoadStateBinary(reader);
            Sound.latched_value[0] = reader.ReadUInt16();
            Sound.utempdata[0] = reader.ReadUInt16();
            AY8910.AA8910[0].stream.output_sampindex = reader.ReadInt32();
            AY8910.AA8910[0].stream.output_base_sampindex = reader.ReadInt32();
            SN76496.ss1[0].Channel.output_sampindex = reader.ReadInt32();
            SN76496.ss1[0].Channel.output_base_sampindex = reader.ReadInt32();
            SN76496.ss1[1].Channel.output_sampindex = reader.ReadInt32();
            SN76496.ss1[1].Channel.output_base_sampindex = reader.ReadInt32();
            Sound.mixerstream.output_sampindex = reader.ReadInt32();
            Sound.mixerstream.output_base_sampindex = reader.ReadInt32();
        }
    }
}
