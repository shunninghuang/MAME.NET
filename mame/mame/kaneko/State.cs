using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using cpu.z80;

namespace mame
{
    public partial class Kaneko
    {
        public static void SaveStateBinary_airbustr(BinaryWriter writer)
        {
            int i;
            writer.Write(dsw1);
            writer.Write(dsw2);
            writer.Write(Mame.rand_seed);
            writer.Write(basebankmaster);
            writer.Write(basebankslave);
            writer.Write(basebankaudio);
            writer.Write(soundlatch_status);
            writer.Write(soundlatch2_status);
            writer.Write(master_addr);
            writer.Write(slave_addr);
            writer.Write(bg_scrollx);
            writer.Write(bg_scrolly);
            writer.Write(fg_scrollx);
            writer.Write(fg_scrolly);
            writer.Write(highbits);
            writer.Write(airbustr_bank1, 0, 0x20000);
            writer.Write(airbustr_bank2, 0, 0x20000);
            writer.Write(airbustr_bank3, 0, 0x20000);
            writer.Write(sharedram, 0, 0x1000);
            writer.Write(devram, 0, 0x1000);
            writer.Write(slaveram, 0, 0x1a00);
            writer.Write(airbustr_videoram2, 0, 0x400);
            writer.Write(airbustr_colorram2, 0, 0x400);
            writer.Write(Generic.videoram, 0, 0x400);
            writer.Write(Generic.colorram, 0, 0x400);
            writer.Write(Generic.paletteram, 0, 0x600);
            writer.Write(pandora_spriteram, 0, 0x1000);
            for (i = 0; i < 0x300; i++)
            {
                writer.Write(Palette.entry_color[i]);
            }
            writer.Write(Memory.mainram, 0, 0x1000);
            writer.Write(Memory.audioram, 0, 0x2000);
            for (i = 0; i < 3; i++)
            {
                Z80A.zz1[i].SaveStateBinary(writer);
            }
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
            YM2203.FF2203[0].SaveStateBinary(writer);
            OKI6295.oo1[0].SaveStateBinary(writer);
            for (i = 0; i < 2; i++)
            {
                writer.Write(Sound.latched_value[i]);
            }
            for (i = 0; i < 2; i++)
            {
                writer.Write(Sound.utempdata[i]);
            }
            writer.Write(AY8910.AA8910[0].stream.output_sampindex);
            writer.Write(AY8910.AA8910[0].stream.output_base_sampindex);
            writer.Write(YM2203.FF2203[0].stream.output_sampindex);
            writer.Write(YM2203.FF2203[0].stream.output_base_sampindex);
            writer.Write(OKI6295.oo1[0].OKI.stream.output_sampindex);
            writer.Write(OKI6295.oo1[0].OKI.stream.output_base_sampindex);
            writer.Write(Sound.mixerstream.output_sampindex);
            writer.Write(Sound.mixerstream.output_base_sampindex);
        }
        public static void LoadStateBinary_airbustr(BinaryReader reader)
        {
            int i;
            dsw1 = reader.ReadByte();
            dsw2 = reader.ReadByte();
            Mame.rand_seed = reader.ReadUInt32();
            basebankmaster = reader.ReadInt32();
            basebankslave = reader.ReadInt32();
            basebankaudio = reader.ReadInt32();
            soundlatch_status = reader.ReadInt32();
            soundlatch2_status = reader.ReadInt32();
            master_addr = reader.ReadInt32();
            slave_addr = reader.ReadInt32();
            bg_scrollx = reader.ReadInt32();
            bg_scrolly = reader.ReadInt32();
            fg_scrollx = reader.ReadInt32();
            fg_scrolly = reader.ReadInt32();
            highbits = reader.ReadInt32();
            airbustr_bank1 = reader.ReadBytes(0x20000);
            airbustr_bank2 = reader.ReadBytes(0x20000);
            airbustr_bank3 = reader.ReadBytes(0x20000);
            sharedram = reader.ReadBytes(0x1000);
            devram = reader.ReadBytes(0x1000);
            slaveram = reader.ReadBytes(0x1a00);
            airbustr_videoram2 = reader.ReadBytes(0x400);
            airbustr_colorram2 = reader.ReadBytes(0x400);
            Generic.videoram = reader.ReadBytes(0x400);
            Generic.colorram = reader.ReadBytes(0x400);
            Generic.paletteram = reader.ReadBytes(0x600);
            pandora_spriteram = reader.ReadBytes(0x1000);
            for (i = 0; i < 0x300; i++)
            {
                Palette.entry_color[i]=reader.ReadUInt32();
            }
            Memory.mainram = reader.ReadBytes(0x1000);
            Memory.audioram = reader.ReadBytes(0x2000);
            for (i = 0; i < 3; i++)
            {
                Z80A.zz1[i].LoadStateBinary(reader);
            }
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
            YM2203.FF2203[0].LoadStateBinary(reader);
            OKI6295.oo1[0].LoadStateBinary(reader);
            for (i = 0; i < 2; i++)
            {
                Sound.latched_value[i] = reader.ReadUInt16();
            }
            for (i = 0; i < 2; i++)
            {
                Sound.utempdata[i] = reader.ReadUInt16();
            }
            AY8910.AA8910[0].stream.output_sampindex = reader.ReadInt32();
            AY8910.AA8910[0].stream.output_base_sampindex = reader.ReadInt32();
            YM2203.FF2203[0].stream.output_sampindex = reader.ReadInt32();
            YM2203.FF2203[0].stream.output_base_sampindex = reader.ReadInt32();
            OKI6295.oo1[0].OKI.stream.output_sampindex = reader.ReadInt32();
            OKI6295.oo1[0].OKI.stream.output_base_sampindex = reader.ReadInt32();
            Sound.mixerstream.output_sampindex = reader.ReadInt32();
            Sound.mixerstream.output_base_sampindex = reader.ReadInt32();
        }
    }
}
