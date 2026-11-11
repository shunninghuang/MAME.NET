using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using cpu.m68000;
using cpu.z80;
using cpu.mcs51;

namespace mame
{
    public partial class sega
    {
        public static void SaveStateBinary_system18(BinaryWriter writer)
        {
            int i;
            writer.Write(dsw);
            writer.Write(porth);
            writer.Write(coinage);
            writer.Write(regs, 0, 0x20);
            writer.Write(grayscale_enable);
            writer.Write(vdp_enable);
            writer.Write(vdp_mixing);
            writer.Write(misc_io_data, 0, 0x10);
            for (i = 0; i < 0x8000; i++)
            {
                writer.Write(segaic16_tileram_0[i]);
            }
            for (i = 0; i < 0x800; i++)
            {
                writer.Write(segaic16_textram_0[i]);
            }
            for (i = 0; i < 0x400; i++)
            {
                writer.Write(segaic16_spriteram_0[i]);
            }
            SaveStateBinary_genesis(writer);
            for (i = 0; i < 0x1000; i++)
            {
                writer.Write(Generic.paletteram16[i]);
            }
            writer.Write(basebanksnd);
            for (i = 0; i < 0x2000; i++)
            {
                writer.Write(Palette.entry_color[i]);
            }
            writer.Write(Memory.mainram, 0, 0x4000);
            writer.Write(Memory.audioram, 0, 0x2000);
            MC68000.mm1[0].SaveStateBinary(writer);
            Z80A.zz1[0].SaveStateBinary(writer);
            Cpuint.SaveStateBinary(writer);
            writer.Write(Timer.global_basetime.seconds);
            writer.Write(Timer.global_basetime.attoseconds);
            Video.SaveStateBinary(writer);
            writer.Write(Sound.last_update_second);
            Cpuexec.SaveStateBinary(writer);
            Timer.SaveStateBinary(writer);
            YM2612.FF2612[0].SaveStateBinary(writer);
            YM2612.FF2612[1].SaveStateBinary(writer);
            rf5c68.SaveStateBinary(writer);
            writer.Write(YM2612.FF2612[0].stream.output_sampindex);
            writer.Write(YM2612.FF2612[0].stream.output_base_sampindex);
            writer.Write(YM2612.FF2612[1].stream.output_sampindex);
            writer.Write(YM2612.FF2612[1].stream.output_base_sampindex);
            writer.Write(rf5c68.r1.stream.output_sampindex);
            writer.Write(rf5c68.r1.stream.output_base_sampindex);
            writer.Write(Sound.mixerstream.output_sampindex);
            writer.Write(Sound.mixerstream.output_base_sampindex);
        }
        public static void LoadStateBinary_system18(BinaryReader reader)
        {
            int i;
            dsw = reader.ReadByte();
            porth = reader.ReadByte();
            coinage = reader.ReadByte();
            regs = reader.ReadBytes(0x20);
            grayscale_enable = reader.ReadByte();
            vdp_enable = reader.ReadByte();
            vdp_mixing = reader.ReadByte();
            misc_io_data = reader.ReadBytes(0x10);
            for (i = 0; i < 0x8000; i++)
            {
                segaic16_tileram_0[i] = reader.ReadUInt16();
            }
            for (i = 0; i < 0x800; i++)
            {
                segaic16_textram_0[i] = reader.ReadUInt16();
            }
            for (i = 0; i < 0x400; i++)
            {
                segaic16_spriteram_0[i] = reader.ReadUInt16();
            }
            LoadStateBinary_genesis(reader);
            for (i = 0; i < 0x1000; i++)
            {
                Generic.paletteram16[i] = reader.ReadUInt16();
            }
            basebanksnd = reader.ReadInt32();
            for (i = 0; i < 0x2000; i++)
            {
                Palette.entry_color[i] = reader.ReadUInt32();
            }
            Memory.mainram = reader.ReadBytes(0x4000);
            Memory.audioram = reader.ReadBytes(0x2000);
            MC68000.mm1[0].LoadStateBinary(reader);
            Z80A.zz1[0].LoadStateBinary(reader);
            Cpuint.LoadStateBinary(reader);
            Timer.global_basetime.seconds = reader.ReadInt32();
            Timer.global_basetime.attoseconds = reader.ReadInt64();
            Video.LoadStateBinary(reader);
            Sound.last_update_second = reader.ReadInt32();
            Cpuexec.LoadStateBinary(reader);
            Timer.LoadStateBinary(reader);
            YM2612.FF2612[0].LoadStateBinary(reader);
            YM2612.FF2612[1].LoadStateBinary(reader);
            rf5c68.LoadStateBinary(reader);
            YM2612.FF2612[0].stream.output_sampindex = reader.ReadInt32();
            YM2612.FF2612[0].stream.output_base_sampindex = reader.ReadInt32();
            YM2612.FF2612[1].stream.output_sampindex = reader.ReadInt32();
            YM2612.FF2612[1].stream.output_base_sampindex = reader.ReadInt32();
            rf5c68.r1.stream.output_sampindex = reader.ReadInt32();
            rf5c68.r1.stream.output_base_sampindex = reader.ReadInt32();
            Sound.mixerstream.output_sampindex = reader.ReadInt32();
            Sound.mixerstream.output_base_sampindex = reader.ReadInt32();
        }
        public static void SaveStateBinary_astorm(BinaryWriter writer)
        {
            int i;
            writer.Write(dsw);
            writer.Write(porth);
            writer.Write(coinage);
            writer.Write(regs, 0, 0x20);
            writer.Write(grayscale_enable);
            writer.Write(vdp_enable);
            writer.Write(vdp_mixing);
            writer.Write(misc_io_data, 0, 0x10);
            for (i = 0; i < 0x8000; i++)
            {
                writer.Write(segaic16_tileram_0[i]);
            }
            for (i = 0; i < 0x800; i++)
            {
                writer.Write(segaic16_textram_0[i]);
            }
            for (i = 0; i < 0x400; i++)
            {
                writer.Write(segaic16_spriteram_0[i]);
            }
            SaveStateBinary_genesis(writer);
            SaveStateBinary_s16fd(writer);
            for (i = 0; i < 0x1000; i++)
            {
                writer.Write(Generic.paletteram16[i]);
            }
            writer.Write(basebanksnd);
            for (i = 0; i < 0x2000; i++)
            {
                writer.Write(Palette.entry_color[i]);
            }
            writer.Write(Memory.mainram, 0, 0x4000);
            writer.Write(Memory.audioram, 0, 0x2000);
            MC68000.mm1[0].SaveStateBinary(writer);
            Z80A.zz1[0].SaveStateBinary(writer);
            Cpuint.SaveStateBinary(writer);
            writer.Write(Timer.global_basetime.seconds);
            writer.Write(Timer.global_basetime.attoseconds);
            Video.SaveStateBinary(writer);
            writer.Write(Sound.last_update_second);
            Cpuexec.SaveStateBinary(writer);
            Timer.SaveStateBinary(writer);
            YM2612.FF2612[0].SaveStateBinary(writer);
            YM2612.FF2612[1].SaveStateBinary(writer);
            rf5c68.SaveStateBinary(writer);
            writer.Write(YM2612.FF2612[0].stream.output_sampindex);
            writer.Write(YM2612.FF2612[0].stream.output_base_sampindex);
            writer.Write(YM2612.FF2612[1].stream.output_sampindex);
            writer.Write(YM2612.FF2612[1].stream.output_base_sampindex);
            writer.Write(rf5c68.r1.stream.output_sampindex);
            writer.Write(rf5c68.r1.stream.output_base_sampindex);
            writer.Write(Sound.mixerstream.output_sampindex);
            writer.Write(Sound.mixerstream.output_base_sampindex);
        }
        public static void LoadStateBinary_astorm(BinaryReader reader)
        {
            int i;
            dsw = reader.ReadByte();
            porth = reader.ReadByte();
            coinage = reader.ReadByte();
            regs = reader.ReadBytes(0x20);
            grayscale_enable = reader.ReadByte();
            vdp_enable = reader.ReadByte();
            vdp_mixing = reader.ReadByte();
            misc_io_data = reader.ReadBytes(0x10);
            for (i = 0; i < 0x8000; i++)
            {
                segaic16_tileram_0[i] = reader.ReadUInt16();
            }
            for (i = 0; i < 0x800; i++)
            {
                segaic16_textram_0[i] = reader.ReadUInt16();
            }
            for (i = 0; i < 0x400; i++)
            {
                segaic16_spriteram_0[i] = reader.ReadUInt16();
            }
            LoadStateBinary_genesis(reader);
            LoadStateBinary_s16fd(reader);
            for (i = 0; i < 0x1000; i++)
            {
                Generic.paletteram16[i] = reader.ReadUInt16();
            }
            basebanksnd = reader.ReadInt32();
            for (i = 0; i < 0x2000; i++)
            {
                Palette.entry_color[i] = reader.ReadUInt32();
            }
            Memory.mainram = reader.ReadBytes(0x4000);
            Memory.audioram = reader.ReadBytes(0x2000);
            MC68000.mm1[0].LoadStateBinary(reader);
            Z80A.zz1[0].LoadStateBinary(reader);            
            Cpuint.LoadStateBinary(reader);
            Timer.global_basetime.seconds = reader.ReadInt32();
            Timer.global_basetime.attoseconds = reader.ReadInt64();
            Video.LoadStateBinary(reader);
            Sound.last_update_second = reader.ReadInt32();
            Cpuexec.LoadStateBinary(reader);
            Timer.LoadStateBinary(reader);
            YM2612.FF2612[0].LoadStateBinary(reader);
            YM2612.FF2612[1].LoadStateBinary(reader);
            rf5c68.LoadStateBinary(reader);
            YM2612.FF2612[0].stream.output_sampindex = reader.ReadInt32();
            YM2612.FF2612[0].stream.output_base_sampindex = reader.ReadInt32();
            YM2612.FF2612[1].stream.output_sampindex = reader.ReadInt32();
            YM2612.FF2612[1].stream.output_base_sampindex = reader.ReadInt32();
            rf5c68.r1.stream.output_sampindex = reader.ReadInt32();
            rf5c68.r1.stream.output_base_sampindex = reader.ReadInt32();
            Sound.mixerstream.output_sampindex = reader.ReadInt32();
            Sound.mixerstream.output_base_sampindex = reader.ReadInt32();
        }
        public static void SaveStateBinary_mwalk(BinaryWriter writer)
        {
            int i;
            writer.Write(dsw);
            writer.Write(porth);
            writer.Write(coinage);
            writer.Write(regs, 0, 0x20);
            writer.Write(grayscale_enable);
            writer.Write(vdp_enable);
            writer.Write(vdp_mixing);
            writer.Write(misc_io_data, 0, 0x10);
            writer.Write(mcu_data);
            for (i = 0; i < 0x8000; i++)
            {
                writer.Write(segaic16_tileram_0[i]);
            }
            for (i = 0; i < 0x800; i++)
            {
                writer.Write(segaic16_textram_0[i]);
            }
            for (i = 0; i < 0x400; i++)
            {
                writer.Write(segaic16_spriteram_0[i]);
            }
            SaveStateBinary_genesis(writer);
            SaveStateBinary_s16fd(writer);
            for (i = 0; i < 0x1000; i++)
            {
                writer.Write(Generic.paletteram16[i]);
            }
            writer.Write(basebanksnd);
            for (i = 0; i < 0x2000; i++)
            {
                writer.Write(Palette.entry_color[i]);
            }
            writer.Write(Memory.mainram, 0, 0x4000);
            writer.Write(Memory.audioram, 0, 0x2000);
            MC68000.mm1[0].SaveStateBinary(writer);
            Z80A.zz1[0].SaveStateBinary(writer);
            I8051.I1.SaveStateBinary(writer);
            Cpuint.SaveStateBinary(writer);
            writer.Write(Timer.global_basetime.seconds);
            writer.Write(Timer.global_basetime.attoseconds);
            Video.SaveStateBinary(writer);
            writer.Write(Sound.last_update_second);
            Cpuexec.SaveStateBinary(writer);
            Timer.SaveStateBinary(writer);
            YM2612.FF2612[0].SaveStateBinary(writer);
            YM2612.FF2612[1].SaveStateBinary(writer);
            rf5c68.SaveStateBinary(writer);
            writer.Write(YM2612.FF2612[0].stream.output_sampindex);
            writer.Write(YM2612.FF2612[0].stream.output_base_sampindex);
            writer.Write(YM2612.FF2612[1].stream.output_sampindex);
            writer.Write(YM2612.FF2612[1].stream.output_base_sampindex);
            writer.Write(rf5c68.r1.stream.output_sampindex);
            writer.Write(rf5c68.r1.stream.output_base_sampindex);
            writer.Write(Sound.mixerstream.output_sampindex);
            writer.Write(Sound.mixerstream.output_base_sampindex);
        }
        public static void LoadStateBinary_mwalk(BinaryReader reader)
        {
            int i;
            dsw = reader.ReadByte();
            porth = reader.ReadByte();
            coinage = reader.ReadByte();
            regs = reader.ReadBytes(0x20);
            grayscale_enable = reader.ReadByte();
            vdp_enable = reader.ReadByte();
            vdp_mixing = reader.ReadByte();
            misc_io_data = reader.ReadBytes(0x10);
            mcu_data = reader.ReadByte();
            for (i = 0; i < 0x8000; i++)
            {
                segaic16_tileram_0[i] = reader.ReadUInt16();
            }
            for (i = 0; i < 0x800; i++)
            {
                segaic16_textram_0[i] = reader.ReadUInt16();
            }
            for (i = 0; i < 0x400; i++)
            {
                segaic16_spriteram_0[i] = reader.ReadUInt16();
            }
            LoadStateBinary_genesis(reader);
            LoadStateBinary_s16fd(reader);
            for (i = 0; i < 0x1000; i++)
            {
                Generic.paletteram16[i] = reader.ReadUInt16();
            }
            basebanksnd = reader.ReadInt32();
            for (i = 0; i < 0x2000; i++)
            {
                Palette.entry_color[i] = reader.ReadUInt32();
            }
            Memory.mainram = reader.ReadBytes(0x4000);
            Memory.audioram = reader.ReadBytes(0x2000);
            MC68000.mm1[0].LoadStateBinary(reader);
            Z80A.zz1[0].LoadStateBinary(reader);
            I8051.I1.LoadStateBinary(reader);
            Cpuint.LoadStateBinary(reader);
            Timer.global_basetime.seconds = reader.ReadInt32();
            Timer.global_basetime.attoseconds = reader.ReadInt64();
            Video.LoadStateBinary(reader);
            Sound.last_update_second = reader.ReadInt32();
            Cpuexec.LoadStateBinary(reader);
            Timer.LoadStateBinary(reader);
            YM2612.FF2612[0].LoadStateBinary(reader);
            YM2612.FF2612[1].LoadStateBinary(reader);
            rf5c68.LoadStateBinary(reader);
            YM2612.FF2612[0].stream.output_sampindex = reader.ReadInt32();
            YM2612.FF2612[0].stream.output_base_sampindex = reader.ReadInt32();
            YM2612.FF2612[1].stream.output_sampindex = reader.ReadInt32();
            YM2612.FF2612[1].stream.output_base_sampindex = reader.ReadInt32();
            rf5c68.r1.stream.output_sampindex = reader.ReadInt32();
            rf5c68.r1.stream.output_base_sampindex = reader.ReadInt32();
            Sound.mixerstream.output_sampindex = reader.ReadInt32();
            Sound.mixerstream.output_base_sampindex = reader.ReadInt32();
        }
        public static void SaveStateBinary_mwalkd(BinaryWriter writer)
        {
            int i;
            writer.Write(dsw);
            writer.Write(porth);
            writer.Write(coinage);
            writer.Write(regs, 0, 0x20);
            writer.Write(grayscale_enable);
            writer.Write(vdp_enable);
            writer.Write(vdp_mixing);
            writer.Write(misc_io_data, 0, 0x10);
            writer.Write(mcu_data);
            for (i = 0; i < 0x8000; i++)
            {
                writer.Write(segaic16_tileram_0[i]);
            }
            for (i = 0; i < 0x800; i++)
            {
                writer.Write(segaic16_textram_0[i]);
            }
            for (i = 0; i < 0x400; i++)
            {
                writer.Write(segaic16_spriteram_0[i]);
            }
            SaveStateBinary_genesis(writer);
            for (i = 0; i < 0x1000; i++)
            {
                writer.Write(Generic.paletteram16[i]);
            }
            writer.Write(basebanksnd);
            for (i = 0; i < 0x2000; i++)
            {
                writer.Write(Palette.entry_color[i]);
            }
            writer.Write(Memory.mainram, 0, 0x4000);
            writer.Write(Memory.audioram, 0, 0x2000);
            MC68000.mm1[0].SaveStateBinary(writer);
            Z80A.zz1[0].SaveStateBinary(writer);
            I8051.I1.SaveStateBinary(writer);
            Cpuint.SaveStateBinary(writer);
            writer.Write(Timer.global_basetime.seconds);
            writer.Write(Timer.global_basetime.attoseconds);
            Video.SaveStateBinary(writer);
            writer.Write(Sound.last_update_second);
            Cpuexec.SaveStateBinary(writer);
            Timer.SaveStateBinary(writer);
            YM2612.FF2612[0].SaveStateBinary(writer);
            YM2612.FF2612[1].SaveStateBinary(writer);
            rf5c68.SaveStateBinary(writer);
            writer.Write(YM2612.FF2612[0].stream.output_sampindex);
            writer.Write(YM2612.FF2612[0].stream.output_base_sampindex);
            writer.Write(YM2612.FF2612[1].stream.output_sampindex);
            writer.Write(YM2612.FF2612[1].stream.output_base_sampindex);
            writer.Write(rf5c68.r1.stream.output_sampindex);
            writer.Write(rf5c68.r1.stream.output_base_sampindex);
            writer.Write(Sound.mixerstream.output_sampindex);
            writer.Write(Sound.mixerstream.output_base_sampindex);
        }
        public static void LoadStateBinary_mwalkd(BinaryReader reader)
        {
            int i;
            dsw = reader.ReadByte();
            porth = reader.ReadByte();
            coinage = reader.ReadByte();
            regs = reader.ReadBytes(0x20);
            grayscale_enable = reader.ReadByte();
            vdp_enable = reader.ReadByte();
            vdp_mixing = reader.ReadByte();
            misc_io_data = reader.ReadBytes(0x10);
            mcu_data = reader.ReadByte();
            for (i = 0; i < 0x8000; i++)
            {
                segaic16_tileram_0[i] = reader.ReadUInt16();
            }
            for (i = 0; i < 0x800; i++)
            {
                segaic16_textram_0[i] = reader.ReadUInt16();
            }
            for (i = 0; i < 0x400; i++)
            {
                segaic16_spriteram_0[i] = reader.ReadUInt16();
            }
            LoadStateBinary_genesis(reader);
            for (i = 0; i < 0x1000; i++)
            {
                Generic.paletteram16[i] = reader.ReadUInt16();
            }
            basebanksnd = reader.ReadInt32();
            for (i = 0; i < 0x2000; i++)
            {
                Palette.entry_color[i] = reader.ReadUInt32();
            }
            Memory.mainram = reader.ReadBytes(0x4000);
            Memory.audioram = reader.ReadBytes(0x2000);
            MC68000.mm1[0].LoadStateBinary(reader);
            Z80A.zz1[0].LoadStateBinary(reader);
            I8051.I1.LoadStateBinary(reader);
            Cpuint.LoadStateBinary(reader);
            Timer.global_basetime.seconds = reader.ReadInt32();
            Timer.global_basetime.attoseconds = reader.ReadInt64();
            Video.LoadStateBinary(reader);
            Sound.last_update_second = reader.ReadInt32();
            Cpuexec.LoadStateBinary(reader);
            Timer.LoadStateBinary(reader);
            YM2612.FF2612[0].LoadStateBinary(reader);
            YM2612.FF2612[1].LoadStateBinary(reader);
            rf5c68.LoadStateBinary(reader);
            YM2612.FF2612[0].stream.output_sampindex = reader.ReadInt32();
            YM2612.FF2612[0].stream.output_base_sampindex = reader.ReadInt32();
            YM2612.FF2612[1].stream.output_sampindex = reader.ReadInt32();
            YM2612.FF2612[1].stream.output_base_sampindex = reader.ReadInt32();
            rf5c68.r1.stream.output_sampindex = reader.ReadInt32();
            rf5c68.r1.stream.output_base_sampindex = reader.ReadInt32();
            Sound.mixerstream.output_sampindex = reader.ReadInt32();
            Sound.mixerstream.output_base_sampindex = reader.ReadInt32();
        }
    }
}
