﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using cpu.m68000;
using cpu.z80;

namespace mame
{
    public partial class Taitob
    {
        public static void SaveStateBinary_masterw(BinaryWriter writer)
        {
            int i;
            writer.Write(Taito.dswa);
            writer.Write(Taito.dswb);
            writer.Write(basebanksnd);
            writer.Write(eep_latch);
            writer.Write(coin_word);
            for (i = 0; i < 0x1000; i++)
            {
                writer.Write(Generic.paletteram16[i]);
            }
            for (i = 0; i < 0x400; i++)
            {
                writer.Write(taitob_scroll[i]);
            }
            for (i = 0; i < 0x8000; i++)
            {
                writer.Write(TC0180VCU_ram[i]);
            }
            for (i = 0; i < 0x10; i++)
            {
                writer.Write(TC0180VCU_ctrl[i]);
            }
            writer.Write(TC0220IOC_regs, 0, 8);
            writer.Write(TC0220IOC_port);
            writer.Write(TC0640FIO_regs, 0, 8);
            for (i = 0; i < 0xcc0; i++)
            {
                writer.Write(taitob_spriteram[i]);
            }
            for (i = 0; i < 2; i++)
            {
                writer.Write(bg_rambank[i]);
            }
            for (i = 0; i < 2; i++)
            {
                writer.Write(fg_rambank[i]);
            }
            writer.Write(tx_rambank);
            writer.Write(video_control);
            for (i = 0; i < 0x1000; i++)
            {
                writer.Write(Palette.entry_color[i]);
            }
            writer.Write(Memory.mainram, 0, 0x10000);
            writer.Write(mainram2, 0, 0x1e80);
            MC68000.m1.SaveStateBinary(writer);
            writer.Write(Memory.audioram, 0, 0x2000);
            Z80A.zz1[0].SaveStateBinary(writer);
            Cpuint.SaveStateBinary(writer);
            writer.Write(Timer.global_basetime.seconds);
            writer.Write(Timer.global_basetime.attoseconds);
            Video.SaveStateBinary(writer);
            writer.Write(Sound.last_update_second);
            Cpuexec.SaveStateBinary(writer);
            Timer.SaveStateBinary(writer);
            AY8910.AA8910[0].SaveStateBinary(writer);
            YM2203.FF2203[0].SaveStateBinary(writer);
            for (i = 0; i < 2; i++)
            {
                writer.Write(Sound.latched_value[i]);
            }
            for (i = 0; i < 2; i++)
            {
                writer.Write(Sound.utempdata[i]);
            }
            writer.Write(AY8910.AA8910[0].stream.sample_rate);
            writer.Write(AY8910.AA8910[0].stream.new_sample_rate);
            writer.Write(AY8910.AA8910[0].stream.gain);
            writer.Write(AY8910.AA8910[0].stream.output_sampindex);
            writer.Write(AY8910.AA8910[0].stream.output_base_sampindex);
            writer.Write(YM2203.FF2203[0].stream.output_sampindex);
            writer.Write(YM2203.FF2203[0].stream.output_base_sampindex);
            writer.Write(Sound.mixerstream.output_sampindex);
            writer.Write(Sound.mixerstream.output_base_sampindex);
            Eeprom.SaveStateBinary(writer);
            Taitosnd.SaveStateBinary(writer);
        }
        public static void LoadStateBinary_masterw(BinaryReader reader)
        {
            int i;
            Taito.dswa = reader.ReadByte();
            Taito.dswb = reader.ReadByte();
            basebanksnd = reader.ReadInt32();
            eep_latch = reader.ReadUInt16();
            coin_word = reader.ReadUInt16();
            for (i = 0; i < 0x1000; i++)
            {
                Generic.paletteram16[i] = reader.ReadUInt16();
            }
            for (i = 0; i < 0x400; i++)
            {
                taitob_scroll[i] = reader.ReadUInt16();
            }
            for (i = 0; i < 0x8000; i++)
            {
                TC0180VCU_ram[i] = reader.ReadUInt16();
            }
            for (i = 0; i < 0x10; i++)
            {
                TC0180VCU_ctrl[i] = reader.ReadUInt16();
            }
            TC0220IOC_regs = reader.ReadBytes(8);
            TC0220IOC_port = reader.ReadByte();
            TC0640FIO_regs = reader.ReadBytes(8);
            for (i = 0; i < 0xcc0; i++)
            {
                taitob_spriteram[i] = reader.ReadUInt16();
            }
            for (i = 0; i < 2; i++)
            {
                bg_rambank[i] = reader.ReadUInt16();
            }
            for (i = 0; i < 2; i++)
            {
                fg_rambank[i] = reader.ReadUInt16();
            }
            tx_rambank = reader.ReadUInt16();
            video_control = reader.ReadByte();
            for (i = 0; i < 0x1000; i++)
            {
                Palette.entry_color[i] = reader.ReadUInt32();
            }
            Memory.mainram = reader.ReadBytes(0x10000);
            mainram2 = reader.ReadBytes(0x1e80);
            MC68000.m1.LoadStateBinary(reader);
            Memory.audioram = reader.ReadBytes(0x2000);
            Z80A.zz1[0].LoadStateBinary(reader);
            Cpuint.LoadStateBinary(reader);
            Timer.global_basetime.seconds = reader.ReadInt32();
            Timer.global_basetime.attoseconds = reader.ReadInt64();
            Video.LoadStateBinary(reader);
            Sound.last_update_second = reader.ReadInt32();
            Cpuexec.LoadStateBinary(reader);
            Timer.LoadStateBinary(reader);
            AY8910.AA8910[0].LoadStateBinary(reader);
            YM2203.FF2203[0].LoadStateBinary(reader);
            for (i = 0; i < 2; i++)
            {
                Sound.latched_value[i] = reader.ReadUInt16();
            }
            for (i = 0; i < 2; i++)
            {
                Sound.utempdata[i] = reader.ReadUInt16();
            }
            AY8910.AA8910[0].stream.sample_rate = reader.ReadInt32();
            AY8910.AA8910[0].stream.new_sample_rate = reader.ReadInt32();
            AY8910.AA8910[0].stream.gain = reader.ReadInt32();
            AY8910.AA8910[0].stream.output_sampindex = reader.ReadInt32();
            AY8910.AA8910[0].stream.output_base_sampindex = reader.ReadInt32();
            YM2203.FF2203[0].stream.output_sampindex = reader.ReadInt32();
            YM2203.FF2203[0].stream.output_base_sampindex = reader.ReadInt32();
            Sound.mixerstream.output_sampindex = reader.ReadInt32();
            Sound.mixerstream.output_base_sampindex = reader.ReadInt32();
            Eeprom.LoadStateBinary(reader);
            Taitosnd.LoadStateBinary(reader);
        }
        public static void SaveStateBinary_nastar(BinaryWriter writer)
        {
            int i;
            writer.Write(Taito.dswa);
            writer.Write(Taito.dswb);
            writer.Write(basebanksnd);
            writer.Write(eep_latch);
            writer.Write(coin_word);
            for (i = 0; i < 0x1000; i++)
            {
                writer.Write(Generic.paletteram16[i]);
            }
            for (i = 0; i < 0x400; i++)
            {
                writer.Write(taitob_scroll[i]);
            }
            for (i = 0; i < 0x8000; i++)
            {
                writer.Write(TC0180VCU_ram[i]);
            }
            for (i = 0; i < 0x10; i++)
            {
                writer.Write(TC0180VCU_ctrl[i]);
            }
            writer.Write(TC0220IOC_regs, 0, 8);
            writer.Write(TC0220IOC_port);
            writer.Write(TC0640FIO_regs, 0, 8);
            for (i = 0; i < 0xcc0; i++)
            {
                writer.Write(taitob_spriteram[i]);
            }
            for (i = 0; i < 2; i++)
            {
                writer.Write(bg_rambank[i]);
            }
            for (i = 0; i < 2; i++)
            {
                writer.Write(fg_rambank[i]);
            }
            writer.Write(tx_rambank);
            writer.Write(video_control);
            for (i = 0; i < 0x1000; i++)
            {
                writer.Write(Palette.entry_color[i]);
            }
            writer.Write(Memory.mainram, 0, 0x10000);
            writer.Write(mainram2, 0, 0x1e80);
            MC68000.m1.SaveStateBinary(writer);
            writer.Write(Memory.audioram, 0, 0x2000);
            Z80A.zz1[0].SaveStateBinary(writer);
            Cpuint.SaveStateBinary(writer);
            writer.Write(Timer.global_basetime.seconds);
            writer.Write(Timer.global_basetime.attoseconds);
            Video.SaveStateBinary(writer);
            writer.Write(Sound.last_update_second);
            Cpuexec.SaveStateBinary(writer);
            Timer.SaveStateBinary(writer);
            AY8910.AA8910[0].SaveStateBinary(writer);
            YM2610.F2610.SaveStateBinary(writer);
            for (i = 0; i < 2; i++)
            {
                writer.Write(Sound.latched_value[i]);
            }
            for (i = 0; i < 2; i++)
            {
                writer.Write(Sound.utempdata[i]);
            }
            writer.Write(AY8910.AA8910[0].stream.sample_rate);
            writer.Write(AY8910.AA8910[0].stream.new_sample_rate);
            writer.Write(AY8910.AA8910[0].stream.gain);
            writer.Write(AY8910.AA8910[0].stream.output_sampindex);
            writer.Write(AY8910.AA8910[0].stream.output_base_sampindex);
            writer.Write(Sound.ym2610stream.output_sampindex);
            writer.Write(Sound.ym2610stream.output_base_sampindex);
            writer.Write(Sound.mixerstream.output_sampindex);
            writer.Write(Sound.mixerstream.output_base_sampindex);
            Eeprom.SaveStateBinary(writer);
            Taitosnd.SaveStateBinary(writer);
        }
        public static void LoadStateBinary_nastar(BinaryReader reader)
        {
            int i;
            Taito.dswa = reader.ReadByte();
            Taito.dswb = reader.ReadByte();
            basebanksnd = reader.ReadInt32();
            eep_latch = reader.ReadUInt16();
            coin_word = reader.ReadUInt16();
            for (i = 0; i < 0x1000; i++)
            {
                Generic.paletteram16[i] = reader.ReadUInt16();
            }
            for (i = 0; i < 0x400; i++)
            {
                taitob_scroll[i] = reader.ReadUInt16();
            }
            for (i = 0; i < 0x8000; i++)
            {
                TC0180VCU_ram[i] = reader.ReadUInt16();
            }
            for (i = 0; i < 0x10; i++)
            {
                TC0180VCU_ctrl[i] = reader.ReadUInt16();
            }
            TC0220IOC_regs = reader.ReadBytes(8);
            TC0220IOC_port = reader.ReadByte();
            TC0640FIO_regs = reader.ReadBytes(8);
            for (i = 0; i < 0xcc0; i++)
            {
                taitob_spriteram[i] = reader.ReadUInt16();
            }
            for (i = 0; i < 2; i++)
            {
                bg_rambank[i] = reader.ReadUInt16();
            }
            for (i = 0; i < 2; i++)
            {
                fg_rambank[i] = reader.ReadUInt16();
            }
            tx_rambank = reader.ReadUInt16();
            video_control = reader.ReadByte();
            for (i = 0; i < 0x1000; i++)
            {
                Palette.entry_color[i] = reader.ReadUInt32();
            }
            Memory.mainram = reader.ReadBytes(0x10000);
            mainram2 = reader.ReadBytes(0x1e80);
            MC68000.m1.LoadStateBinary(reader);
            Memory.audioram = reader.ReadBytes(0x2000);
            Z80A.zz1[0].LoadStateBinary(reader);
            Cpuint.LoadStateBinary(reader);
            Timer.global_basetime.seconds = reader.ReadInt32();
            Timer.global_basetime.attoseconds = reader.ReadInt64();
            Video.LoadStateBinary(reader);
            Sound.last_update_second = reader.ReadInt32();
            Cpuexec.LoadStateBinary(reader);
            Timer.LoadStateBinary(reader);
            AY8910.AA8910[0].LoadStateBinary(reader);
            YM2610.F2610.LoadStateBinary(reader);
            for (i = 0; i < 2; i++)
            {
                Sound.latched_value[i] = reader.ReadUInt16();
            }
            for (i = 0; i < 2; i++)
            {
                Sound.utempdata[i] = reader.ReadUInt16();
            }
            AY8910.AA8910[0].stream.sample_rate = reader.ReadInt32();
            AY8910.AA8910[0].stream.new_sample_rate = reader.ReadInt32();
            AY8910.AA8910[0].stream.gain = reader.ReadInt32();
            AY8910.AA8910[0].stream.output_sampindex = reader.ReadInt32();
            AY8910.AA8910[0].stream.output_base_sampindex = reader.ReadInt32();
            Sound.ym2610stream.output_sampindex = reader.ReadInt32();
            Sound.ym2610stream.output_base_sampindex = reader.ReadInt32();
            Sound.mixerstream.output_sampindex = reader.ReadInt32();
            Sound.mixerstream.output_base_sampindex = reader.ReadInt32();
            Eeprom.LoadStateBinary(reader);
            Taitosnd.LoadStateBinary(reader);
        }
        public static void SaveStateBinary_viofight(BinaryWriter writer)
        {
            int i;
            writer.Write(Taito.dswa);
            writer.Write(Taito.dswb);
            writer.Write(basebanksnd);
            writer.Write(eep_latch);
            writer.Write(coin_word);
            for (i = 0; i < 0x1000; i++)
            {
                writer.Write(Generic.paletteram16[i]);
            }
            for (i = 0; i < 0x400; i++)
            {
                writer.Write(taitob_scroll[i]);
            }
            for (i = 0; i < 0x8000; i++)
            {
                writer.Write(TC0180VCU_ram[i]);
            }
            for (i = 0; i < 0x10; i++)
            {
                writer.Write(TC0180VCU_ctrl[i]);
            }
            writer.Write(TC0220IOC_regs, 0, 8);
            writer.Write(TC0220IOC_port);
            writer.Write(TC0640FIO_regs, 0, 8);
            for (i = 0; i < 0xcc0; i++)
            {
                writer.Write(taitob_spriteram[i]);
            }
            for (i = 0; i < 2; i++)
            {
                writer.Write(bg_rambank[i]);
            }
            for (i = 0; i < 2; i++)
            {
                writer.Write(fg_rambank[i]);
            }
            writer.Write(tx_rambank);
            writer.Write(video_control);
            for (i = 0; i < 0x1000; i++)
            {
                writer.Write(Palette.entry_color[i]);
            }
            writer.Write(Memory.mainram, 0, 0x10000);
            writer.Write(mainram2, 0, 0x1e80);
            MC68000.m1.SaveStateBinary(writer);
            writer.Write(Memory.audioram, 0, 0x2000);
            Z80A.zz1[0].SaveStateBinary(writer);
            Cpuint.SaveStateBinary(writer);
            writer.Write(Timer.global_basetime.seconds);
            writer.Write(Timer.global_basetime.attoseconds);
            Video.SaveStateBinary(writer);
            writer.Write(Sound.last_update_second);
            Cpuexec.SaveStateBinary(writer);
            Timer.SaveStateBinary(writer);
            AY8910.AA8910[0].SaveStateBinary(writer);
            YM2203.FF2203[0].SaveStateBinary(writer);
            OKI6295.SaveStateBinary(writer);
            for (i = 0; i < 2; i++)
            {
                writer.Write(Sound.latched_value[i]);
            }
            for (i = 0; i < 2; i++)
            {
                writer.Write(Sound.utempdata[i]);
            }
            writer.Write(AY8910.AA8910[0].stream.sample_rate);
            writer.Write(AY8910.AA8910[0].stream.new_sample_rate);
            writer.Write(AY8910.AA8910[0].stream.gain);
            writer.Write(AY8910.AA8910[0].stream.output_sampindex);
            writer.Write(AY8910.AA8910[0].stream.output_base_sampindex);
            writer.Write(YM2203.FF2203[0].stream.output_sampindex);
            writer.Write(YM2203.FF2203[0].stream.output_base_sampindex);
            writer.Write(Sound.okistream.output_sampindex);
            writer.Write(Sound.okistream.output_base_sampindex);
            writer.Write(Sound.mixerstream.output_sampindex);
            writer.Write(Sound.mixerstream.output_base_sampindex);
            Eeprom.SaveStateBinary(writer);
            Taitosnd.SaveStateBinary(writer);
        }
        public static void LoadStateBinary_viofight(BinaryReader reader)
        {
            int i;
            Taito.dswa = reader.ReadByte();
            Taito.dswb = reader.ReadByte();
            basebanksnd = reader.ReadInt32();
            eep_latch = reader.ReadUInt16();
            coin_word = reader.ReadUInt16();
            for (i = 0; i < 0x1000; i++)
            {
                Generic.paletteram16[i] = reader.ReadUInt16();
            }
            for (i = 0; i < 0x400; i++)
            {
                taitob_scroll[i] = reader.ReadUInt16();
            }
            for (i = 0; i < 0x8000; i++)
            {
                TC0180VCU_ram[i] = reader.ReadUInt16();
            }
            for (i = 0; i < 0x10; i++)
            {
                TC0180VCU_ctrl[i] = reader.ReadUInt16();
            }
            TC0220IOC_regs = reader.ReadBytes(8);
            TC0220IOC_port = reader.ReadByte();
            TC0640FIO_regs = reader.ReadBytes(8);
            for (i = 0; i < 0xcc0; i++)
            {
                taitob_spriteram[i] = reader.ReadUInt16();
            }
            for (i = 0; i < 2; i++)
            {
                bg_rambank[i] = reader.ReadUInt16();
            }
            for (i = 0; i < 2; i++)
            {
                fg_rambank[i] = reader.ReadUInt16();
            }
            tx_rambank = reader.ReadUInt16();
            video_control = reader.ReadByte();
            for (i = 0; i < 0x1000; i++)
            {
                Palette.entry_color[i] = reader.ReadUInt32();
            }
            Memory.mainram = reader.ReadBytes(0x10000);
            mainram2 = reader.ReadBytes(0x1e80);
            MC68000.m1.LoadStateBinary(reader);
            Memory.audioram = reader.ReadBytes(0x2000);
            Z80A.zz1[0].LoadStateBinary(reader);
            Cpuint.LoadStateBinary(reader);
            Timer.global_basetime.seconds = reader.ReadInt32();
            Timer.global_basetime.attoseconds = reader.ReadInt64();
            Video.LoadStateBinary(reader);
            Sound.last_update_second = reader.ReadInt32();
            Cpuexec.LoadStateBinary(reader);
            Timer.LoadStateBinary(reader);
            AY8910.AA8910[0].LoadStateBinary(reader);
            YM2203.FF2203[0].LoadStateBinary(reader);
            OKI6295.LoadStateBinary(reader);
            for (i = 0; i < 2; i++)
            {
                Sound.latched_value[i] = reader.ReadUInt16();
            }
            for (i = 0; i < 2; i++)
            {
                Sound.utempdata[i] = reader.ReadUInt16();
            }
            AY8910.AA8910[0].stream.sample_rate = reader.ReadInt32();
            AY8910.AA8910[0].stream.new_sample_rate = reader.ReadInt32();
            AY8910.AA8910[0].stream.gain = reader.ReadInt32();
            AY8910.AA8910[0].stream.output_sampindex = reader.ReadInt32();
            AY8910.AA8910[0].stream.output_base_sampindex = reader.ReadInt32();
            YM2203.FF2203[0].stream.output_sampindex = reader.ReadInt32();
            YM2203.FF2203[0].stream.output_base_sampindex = reader.ReadInt32();
            Sound.okistream.output_sampindex = reader.ReadInt32();
            Sound.okistream.output_base_sampindex = reader.ReadInt32();
            Sound.mixerstream.output_sampindex = reader.ReadInt32();
            Sound.mixerstream.output_base_sampindex = reader.ReadInt32();
            Eeprom.LoadStateBinary(reader);
            Taitosnd.LoadStateBinary(reader);
        }
        public static void SaveStateBinary_hitice(BinaryWriter writer)
        {
            int i;
            writer.Write(Taito.dswa);
            writer.Write(Taito.dswb);
            for (i = 0; i < 0x40000; i++)
            {
                writer.Write(taitob_pixelram[i]);
            }
            writer.Write(basebanksnd);
            writer.Write(eep_latch);
            writer.Write(coin_word);
            for (i = 0; i < 0x1000; i++)
            {
                writer.Write(Generic.paletteram16[i]);
            }
            for (i = 0; i < 0x400; i++)
            {
                writer.Write(taitob_scroll[i]);
            }
            for (i = 0; i < 0x8000; i++)
            {
                writer.Write(TC0180VCU_ram[i]);
            }
            for (i = 0; i < 0x10; i++)
            {
                writer.Write(TC0180VCU_ctrl[i]);
            }
            writer.Write(TC0220IOC_regs, 0, 8);
            writer.Write(TC0220IOC_port);
            writer.Write(TC0640FIO_regs, 0, 8);
            for (i = 0; i < 0xcc0; i++)
            {
                writer.Write(taitob_spriteram[i]);
            }
            for (i = 0; i < 2; i++)
            {
                writer.Write(bg_rambank[i]);
            }
            for (i = 0; i < 2; i++)
            {
                writer.Write(fg_rambank[i]);
            }
            writer.Write(tx_rambank);
            writer.Write(video_control);
            for (i = 0; i < 0x1000; i++)
            {
                writer.Write(Palette.entry_color[i]);
            }
            writer.Write(Memory.mainram, 0, 0x10000);
            writer.Write(mainram2, 0, 0x1e80);
            MC68000.m1.SaveStateBinary(writer);
            writer.Write(Memory.audioram, 0, 0x2000);
            Z80A.zz1[0].SaveStateBinary(writer);
            Cpuint.SaveStateBinary(writer);
            writer.Write(Timer.global_basetime.seconds);
            writer.Write(Timer.global_basetime.attoseconds);
            Video.SaveStateBinary(writer);
            writer.Write(Sound.last_update_second);
            Cpuexec.SaveStateBinary(writer);
            Timer.SaveStateBinary(writer);
            AY8910.AA8910[0].SaveStateBinary(writer);
            YM2203.FF2203[0].SaveStateBinary(writer);
            OKI6295.SaveStateBinary(writer);
            for (i = 0; i < 2; i++)
            {
                writer.Write(Sound.latched_value[i]);
            }
            for (i = 0; i < 2; i++)
            {
                writer.Write(Sound.utempdata[i]);
            }
            writer.Write(AY8910.AA8910[0].stream.sample_rate);
            writer.Write(AY8910.AA8910[0].stream.new_sample_rate);
            writer.Write(AY8910.AA8910[0].stream.gain);
            writer.Write(AY8910.AA8910[0].stream.output_sampindex);
            writer.Write(AY8910.AA8910[0].stream.output_base_sampindex);
            writer.Write(YM2203.FF2203[0].stream.output_sampindex);
            writer.Write(YM2203.FF2203[0].stream.output_base_sampindex);
            writer.Write(Sound.okistream.output_sampindex);
            writer.Write(Sound.okistream.output_base_sampindex);
            writer.Write(Sound.mixerstream.output_sampindex);
            writer.Write(Sound.mixerstream.output_base_sampindex);
            Eeprom.SaveStateBinary(writer);
            Taitosnd.SaveStateBinary(writer);
        }
        public static void LoadStateBinary_hitice(BinaryReader reader)
        {
            int i;
            Taito.dswa = reader.ReadByte();
            Taito.dswb = reader.ReadByte();
            for (i = 0; i < 0x40000; i++)
            {
                taitob_pixelram[i] = reader.ReadUInt16();
            }
            basebanksnd = reader.ReadInt32();
            eep_latch = reader.ReadUInt16();
            coin_word = reader.ReadUInt16();
            for (i = 0; i < 0x1000; i++)
            {
                Generic.paletteram16[i] = reader.ReadUInt16();
            }
            for (i = 0; i < 0x400; i++)
            {
                taitob_scroll[i] = reader.ReadUInt16();
            }
            for (i = 0; i < 0x8000; i++)
            {
                TC0180VCU_ram[i] = reader.ReadUInt16();
            }
            for (i = 0; i < 0x10; i++)
            {
                TC0180VCU_ctrl[i] = reader.ReadUInt16();
            }
            TC0220IOC_regs = reader.ReadBytes(8);
            TC0220IOC_port = reader.ReadByte();
            TC0640FIO_regs = reader.ReadBytes(8);
            for (i = 0; i < 0xcc0; i++)
            {
                taitob_spriteram[i] = reader.ReadUInt16();
            }
            for (i = 0; i < 2; i++)
            {
                bg_rambank[i] = reader.ReadUInt16();
            }
            for (i = 0; i < 2; i++)
            {
                fg_rambank[i] = reader.ReadUInt16();
            }
            tx_rambank = reader.ReadUInt16();
            video_control = reader.ReadByte();
            for (i = 0; i < 0x1000; i++)
            {
                Palette.entry_color[i] = reader.ReadUInt32();
            }
            Memory.mainram = reader.ReadBytes(0x10000);
            mainram2 = reader.ReadBytes(0x1e80);
            MC68000.m1.LoadStateBinary(reader);
            Memory.audioram = reader.ReadBytes(0x2000);
            Z80A.zz1[0].LoadStateBinary(reader);
            Cpuint.LoadStateBinary(reader);
            Timer.global_basetime.seconds = reader.ReadInt32();
            Timer.global_basetime.attoseconds = reader.ReadInt64();
            Video.LoadStateBinary(reader);
            Sound.last_update_second = reader.ReadInt32();
            Cpuexec.LoadStateBinary(reader);
            Timer.LoadStateBinary(reader);
            AY8910.AA8910[0].LoadStateBinary(reader);
            YM2203.FF2203[0].LoadStateBinary(reader);
            OKI6295.LoadStateBinary(reader);
            for (i = 0; i < 2; i++)
            {
                Sound.latched_value[i] = reader.ReadUInt16();
            }
            for (i = 0; i < 2; i++)
            {
                Sound.utempdata[i] = reader.ReadUInt16();
            }
            AY8910.AA8910[0].stream.sample_rate = reader.ReadInt32();
            AY8910.AA8910[0].stream.new_sample_rate = reader.ReadInt32();
            AY8910.AA8910[0].stream.gain = reader.ReadInt32();
            AY8910.AA8910[0].stream.output_sampindex = reader.ReadInt32();
            AY8910.AA8910[0].stream.output_base_sampindex = reader.ReadInt32();
            YM2203.FF2203[0].stream.output_sampindex = reader.ReadInt32();
            YM2203.FF2203[0].stream.output_base_sampindex = reader.ReadInt32();
            Sound.okistream.output_sampindex = reader.ReadInt32();
            Sound.okistream.output_base_sampindex = reader.ReadInt32();
            Sound.mixerstream.output_sampindex = reader.ReadInt32();
            Sound.mixerstream.output_base_sampindex = reader.ReadInt32();
            Eeprom.LoadStateBinary(reader);
            Taitosnd.LoadStateBinary(reader);
        }
        public static void SaveStateBinary_silentd(BinaryWriter writer)
        {
            int i;
            writer.Write(Taito.dswa);
            writer.Write(Taito.dswb);
            writer.Write(basebanksnd);
            writer.Write(eep_latch);
            writer.Write(coin_word);
            for (i = 0; i < 0x1000; i++)
            {
                writer.Write(Generic.paletteram16[i]);
            }
            for (i = 0; i < 0x400; i++)
            {
                writer.Write(taitob_scroll[i]);
            }
            for (i = 0; i < 0x8000; i++)
            {
                writer.Write(TC0180VCU_ram[i]);
            }
            for (i = 0; i < 0x10; i++)
            {
                writer.Write(TC0180VCU_ctrl[i]);
            }
            writer.Write(TC0220IOC_regs, 0, 8);
            writer.Write(TC0220IOC_port);
            writer.Write(TC0640FIO_regs, 0, 8);
            for (i = 0; i < 0xcc0; i++)
            {
                writer.Write(taitob_spriteram[i]);
            }
            for (i = 0; i < 2; i++)
            {
                writer.Write(bg_rambank[i]);
            }
            for (i = 0; i < 2; i++)
            {
                writer.Write(fg_rambank[i]);
            }
            writer.Write(tx_rambank);
            writer.Write(video_control);
            for (i = 0; i < 0x1000; i++)
            {
                writer.Write(Palette.entry_color[i]);
            }
            writer.Write(Memory.mainram, 0, 0x10000);
            writer.Write(mainram2, 0, 0x1e80);
            MC68000.m1.SaveStateBinary(writer);
            writer.Write(Memory.audioram, 0, 0x2000);
            Z80A.zz1[0].SaveStateBinary(writer);
            Cpuint.SaveStateBinary(writer);
            writer.Write(Timer.global_basetime.seconds);
            writer.Write(Timer.global_basetime.attoseconds);
            Video.SaveStateBinary(writer);
            writer.Write(Sound.last_update_second);
            Cpuexec.SaveStateBinary(writer);
            Timer.SaveStateBinary(writer);
            AY8910.AA8910[0].SaveStateBinary(writer);
            YM2610.F2610.SaveStateBinary(writer);
            for (i = 0; i < 2; i++)
            {
                writer.Write(Sound.latched_value[i]);
            }
            for (i = 0; i < 2; i++)
            {
                writer.Write(Sound.utempdata[i]);
            }
            writer.Write(AY8910.AA8910[0].stream.sample_rate);
            writer.Write(AY8910.AA8910[0].stream.new_sample_rate);
            writer.Write(AY8910.AA8910[0].stream.gain);
            writer.Write(AY8910.AA8910[0].stream.output_sampindex);
            writer.Write(AY8910.AA8910[0].stream.output_base_sampindex);
            writer.Write(Sound.ym2610stream.output_sampindex);
            writer.Write(Sound.ym2610stream.output_base_sampindex);
            writer.Write(Sound.mixerstream.output_sampindex);
            writer.Write(Sound.mixerstream.output_base_sampindex);
            SaveStateBinary_MB87078(writer);
            Eeprom.SaveStateBinary(writer);
            Taitosnd.SaveStateBinary(writer);
        }
        public static void LoadStateBinary_silentd(BinaryReader reader)
        {
            int i;
            Taito.dswa = reader.ReadByte();
            Taito.dswb = reader.ReadByte();
            basebanksnd = reader.ReadInt32();
            eep_latch = reader.ReadUInt16();
            coin_word = reader.ReadUInt16();
            for (i = 0; i < 0x1000; i++)
            {
                Generic.paletteram16[i] = reader.ReadUInt16();
            }
            for (i = 0; i < 0x400; i++)
            {
                taitob_scroll[i] = reader.ReadUInt16();
            }
            for (i = 0; i < 0x8000; i++)
            {
                TC0180VCU_ram[i] = reader.ReadUInt16();
            }
            for (i = 0; i < 0x10; i++)
            {
                TC0180VCU_ctrl[i] = reader.ReadUInt16();
            }
            TC0220IOC_regs = reader.ReadBytes(8);
            TC0220IOC_port = reader.ReadByte();
            TC0640FIO_regs = reader.ReadBytes(8);
            for (i = 0; i < 0xcc0; i++)
            {
                taitob_spriteram[i] = reader.ReadUInt16();
            }
            for (i = 0; i < 2; i++)
            {
                bg_rambank[i] = reader.ReadUInt16();
            }
            for (i = 0; i < 2; i++)
            {
                fg_rambank[i] = reader.ReadUInt16();
            }
            tx_rambank = reader.ReadUInt16();
            video_control = reader.ReadByte();
            for (i = 0; i < 0x1000; i++)
            {
                Palette.entry_color[i] = reader.ReadUInt32();
            }
            Memory.mainram = reader.ReadBytes(0x10000);
            mainram2 = reader.ReadBytes(0x1e80);
            MC68000.m1.LoadStateBinary(reader);
            Memory.audioram = reader.ReadBytes(0x2000);
            Z80A.zz1[0].LoadStateBinary(reader);
            Cpuint.LoadStateBinary(reader);
            Timer.global_basetime.seconds = reader.ReadInt32();
            Timer.global_basetime.attoseconds = reader.ReadInt64();
            Video.LoadStateBinary(reader);
            Sound.last_update_second = reader.ReadInt32();
            Cpuexec.LoadStateBinary(reader);
            Timer.LoadStateBinary(reader);
            AY8910.AA8910[0].LoadStateBinary(reader);
            YM2610.F2610.LoadStateBinary(reader);
            for (i = 0; i < 2; i++)
            {
                Sound.latched_value[i] = reader.ReadUInt16();
            }
            for (i = 0; i < 2; i++)
            {
                Sound.utempdata[i] = reader.ReadUInt16();
            }
            AY8910.AA8910[0].stream.sample_rate = reader.ReadInt32();
            AY8910.AA8910[0].stream.new_sample_rate = reader.ReadInt32();
            AY8910.AA8910[0].stream.gain = reader.ReadInt32();
            AY8910.AA8910[0].stream.output_sampindex = reader.ReadInt32();
            AY8910.AA8910[0].stream.output_base_sampindex = reader.ReadInt32();
            Sound.ym2610stream.output_sampindex = reader.ReadInt32();
            Sound.ym2610stream.output_base_sampindex = reader.ReadInt32();
            Sound.mixerstream.output_sampindex = reader.ReadInt32();
            Sound.mixerstream.output_base_sampindex = reader.ReadInt32();
            LoadStateBinary_MB87078(reader);
            Eeprom.LoadStateBinary(reader);
            Taitosnd.LoadStateBinary(reader);
        }
    }
}
