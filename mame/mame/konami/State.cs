using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using cpu.konami;
using cpu.m68000;
using cpu.z80;

namespace mame
{
    public partial class Konami
    {
        public static void SaveStateBinary_scontra(BinaryWriter writer)
        {
            int i;
            writer.Write(dsw1);
            writer.Write(dsw2);
            writer.Write(dsw3);
            writer.Write(palette_selected);
            writer.Write(m_priority);
            writer.Write(basebankmain);
            writer.Write(m_1f98_latch);
            writer.Write(ram, 0, 0x800);
            SaveStateBinary_K052109(writer);
            SaveStateBinary_K051960(writer);
            writer.Write(Generic.paletteram, 0, 0x800);
            for (i = 0; i < 0x400; i++)
            {
                writer.Write(Palette.entry_color[i]);
            }
            writer.Write(Memory.mainram, 0, 0x1800);
            writer.Write(Memory.audioram, 0, 0x800);
            KonamiCpu.k1.SaveStateBinary(writer);
            Z80A.zz1[0].SaveStateBinary(writer);
            Cpuint.SaveStateBinary(writer);
            writer.Write(Timer.global_basetime.seconds);
            writer.Write(Timer.global_basetime.attoseconds);
            Video.SaveStateBinary(writer);
            writer.Write(Sound.last_update_second);
            Cpuexec.SaveStateBinary(writer);
            Timer.SaveStateBinary(writer);
            YM2151.SaveStateBinary(writer);
            K007232.SaveStateBinary(writer);
            writer.Write(Sound.ym2151stream.output_sampindex);
            writer.Write(Sound.ym2151stream.output_base_sampindex);
            writer.Write(Sound.k007232stream.output_sampindex);
            writer.Write(Sound.k007232stream.output_base_sampindex);
            writer.Write(Sound.mixerstream.output_sampindex);
            writer.Write(Sound.mixerstream.output_base_sampindex);
        }
        public static void LoadStateBinary_scontra(BinaryReader reader)
        {
            int i;
            dsw1 = reader.ReadByte();
            dsw2 = reader.ReadByte();
            dsw3 = reader.ReadByte();
            palette_selected = reader.ReadInt32();
            m_priority = reader.ReadInt32();
            basebankmain = reader.ReadInt32();
            m_1f98_latch = reader.ReadByte();
            ram = reader.ReadBytes(0x800);
            LoadStateBinary_K052109(reader);
            LoadStateBinary_K051960(reader);
            Generic.paletteram = reader.ReadBytes(0x800);
            for (i = 0; i < 0x400; i++)
            {
                Palette.entry_color[i] = reader.ReadUInt32();
            }
            Memory.mainram = reader.ReadBytes(0x1800);
            Memory.audioram = reader.ReadBytes(0x800);
            KonamiCpu.k1.LoadStateBinary(reader);
            Z80A.zz1[0].LoadStateBinary(reader);
            Cpuint.LoadStateBinary(reader);
            Timer.global_basetime.seconds = reader.ReadInt32();
            Timer.global_basetime.attoseconds = reader.ReadInt64();
            Video.LoadStateBinary(reader);
            Sound.last_update_second = reader.ReadInt32();
            Cpuexec.LoadStateBinary(reader);
            Timer.LoadStateBinary(reader);
            YM2151.LoadStateBinary(reader);
            K007232.LoadStateBinary(reader);
            Sound.ym2151stream.output_sampindex = reader.ReadInt32();
            Sound.ym2151stream.output_base_sampindex = reader.ReadInt32();
            Sound.k007232stream.output_sampindex = reader.ReadInt32();
            Sound.k007232stream.output_base_sampindex = reader.ReadInt32();
            Sound.mixerstream.output_sampindex = reader.ReadInt32();
            Sound.mixerstream.output_base_sampindex = reader.ReadInt32();
        }
        public static void SaveStateBinary_thunderx(BinaryWriter writer)
        {
            int i;
            writer.Write(dsw1);
            writer.Write(dsw2);
            writer.Write(dsw3);
            writer.Write(palette_selected);
            writer.Write(m_priority);
            writer.Write(basebankmain);
            writer.Write(m_1f98_latch);
            writer.Write(rambank);
            writer.Write(pmcbank);
            writer.Write(ram, 0, 0x800);
            writer.Write(pmcram, 0, 0x800);
            SaveStateBinary_K052109(writer);
            SaveStateBinary_K051960(writer);
            writer.Write(Generic.paletteram, 0, 0x800);
            for (i = 0; i < 0x400; i++)
            {
                writer.Write(Palette.entry_color[i]);
            }
            writer.Write(Memory.mainram, 0, 0x1800);
            writer.Write(Memory.audioram, 0, 0x800);
            KonamiCpu.k1.SaveStateBinary(writer);
            Z80A.zz1[0].SaveStateBinary(writer);
            Cpuint.SaveStateBinary(writer);
            writer.Write(Timer.global_basetime.seconds);
            writer.Write(Timer.global_basetime.attoseconds);
            Video.SaveStateBinary(writer);
            writer.Write(Sound.last_update_second);
            Cpuexec.SaveStateBinary(writer);
            Timer.SaveStateBinary(writer);
            YM2151.SaveStateBinary(writer);
            writer.Write(Sound.ym2151stream.output_sampindex);
            writer.Write(Sound.ym2151stream.output_base_sampindex);
            writer.Write(Sound.mixerstream.output_sampindex);
            writer.Write(Sound.mixerstream.output_base_sampindex);
        }
        public static void LoadStateBinary_thunderx(BinaryReader reader)
        {
            int i;
            dsw1 = reader.ReadByte();
            dsw2 = reader.ReadByte();
            dsw3 = reader.ReadByte();
            palette_selected = reader.ReadInt32();
            m_priority = reader.ReadInt32();
            basebankmain = reader.ReadInt32();
            m_1f98_latch = reader.ReadByte();
            rambank = reader.ReadInt32();
            pmcbank = reader.ReadInt32();
            ram = reader.ReadBytes(0x800);
            pmcram = reader.ReadBytes(0x800);
            LoadStateBinary_K052109(reader);
            LoadStateBinary_K051960(reader);
            Generic.paletteram = reader.ReadBytes(0x800);
            for (i = 0; i < 0x400; i++)
            {
                Palette.entry_color[i] = reader.ReadUInt32();
            }
            Memory.mainram = reader.ReadBytes(0x1800);
            Memory.audioram = reader.ReadBytes(0x800);
            KonamiCpu.k1.LoadStateBinary(reader);
            Z80A.zz1[0].LoadStateBinary(reader);
            Cpuint.LoadStateBinary(reader);
            Timer.global_basetime.seconds = reader.ReadInt32();
            Timer.global_basetime.attoseconds = reader.ReadInt64();
            Video.LoadStateBinary(reader);
            Sound.last_update_second = reader.ReadInt32();
            Cpuexec.LoadStateBinary(reader);
            Timer.LoadStateBinary(reader);
            YM2151.LoadStateBinary(reader);
            Sound.ym2151stream.output_sampindex = reader.ReadInt32();
            Sound.ym2151stream.output_base_sampindex = reader.ReadInt32();
            Sound.mixerstream.output_sampindex = reader.ReadInt32();
            Sound.mixerstream.output_base_sampindex = reader.ReadInt32();
        }
        public static void SaveStateBinary_cuebrick(BinaryWriter writer)
        {
            int i;
            writer.Write(dsw1);
            writer.Write(dsw2);
            writer.Write(dsw3);
            writer.Write(cuebrick_snd_irqlatch);
            writer.Write(cuebrick_nvram_bank);
            for (i = 0; i < 0x8000; i++)
            {
                writer.Write(cuebrick_nvram[i]);
            }
            writer.Write(init_eeprom_count);
            writer.Write(toggle);
            writer.Write(dim_c);
            writer.Write(dim_v);
            writer.Write(lastdim);
            writer.Write(lasten);
            writer.Write(sprite_colorbase);
            writer.Write(bg_colorbase);
            for (i = 0; i < 3; i++)
            {
                writer.Write(layer_colorbase[i]);
            }
            SaveStateBinary_K053251(writer);
            SaveStateBinary_K052109(writer);
            SaveStateBinary_K051960(writer);
            for (i = 0; i < 0x800; i++)
            {
                writer.Write(Generic.paletteram16[i]);
            }
            for (i = 0; i < 0x2000; i++)
            {
                writer.Write(Generic.spriteram16[i]);
            }
            for (i = 0; i < 0x800; i++)
            {
                writer.Write(Palette.entry_color[i]);
            }
            writer.Write(Memory.mainram, 0, 0x4000);
            writer.Write(mainram2, 0, 0x4000);
            MC68000.mm1[0].SaveStateBinary(writer);
            writer.Write(Memory.audioram, 0, 0x800);
            Cpuint.SaveStateBinary(writer);
            writer.Write(Timer.global_basetime.seconds);
            writer.Write(Timer.global_basetime.attoseconds);
            Video.SaveStateBinary(writer);
            writer.Write(Sound.last_update_second);
            Cpuexec.SaveStateBinary(writer);
            Timer.SaveStateBinary(writer);
            YM2151.SaveStateBinary(writer);
            writer.Write(Sound.ym2151stream.output_sampindex);
            writer.Write(Sound.ym2151stream.output_base_sampindex);
            writer.Write(Sound.mixerstream.output_sampindex);
            writer.Write(Sound.mixerstream.output_base_sampindex);
        }
        public static void LoadStateBinary_cuebrick(BinaryReader reader)
        {
            int i;
            dsw1 = reader.ReadByte();
            dsw2 = reader.ReadByte();
            dsw3 = reader.ReadByte();
            cuebrick_snd_irqlatch = reader.ReadInt32();
            cuebrick_nvram_bank = reader.ReadInt32();
            for (i = 0; i < 0x8000; i++)
            {
                cuebrick_nvram[i] = reader.ReadUInt16();
            }
            init_eeprom_count = reader.ReadInt32();
            toggle = reader.ReadInt32();
            dim_c = reader.ReadInt32();
            dim_v = reader.ReadInt32();
            lastdim = reader.ReadInt32();
            lasten = reader.ReadInt32();
            sprite_colorbase = reader.ReadInt32();
            bg_colorbase = reader.ReadInt32();
            for (i = 0; i < 3; i++)
            {
                layer_colorbase[i] = reader.ReadInt32();
            }
            LoadStateBinary_K053251(reader);
            LoadStateBinary_K052109(reader);
            LoadStateBinary_K051960(reader);
            for (i = 0; i < 0x800; i++)
            {
                Generic.paletteram16[i] = reader.ReadUInt16();
            }
            for (i = 0; i < 0x2000; i++)
            {
                Generic.spriteram16[i] = reader.ReadUInt16();
            }
            for (i = 0; i < 0x800; i++)
            {
                Palette.entry_color[i] = reader.ReadUInt32();
            }
            Memory.mainram = reader.ReadBytes(0x4000);
            mainram2 = reader.ReadBytes(0x4000);
            MC68000.mm1[0].LoadStateBinary(reader);
            Memory.audioram = reader.ReadBytes(0x800);
            Cpuint.LoadStateBinary(reader);
            Timer.global_basetime.seconds = reader.ReadInt32();
            Timer.global_basetime.attoseconds = reader.ReadInt64();
            Video.LoadStateBinary(reader);
            Sound.last_update_second = reader.ReadInt32();
            Cpuexec.LoadStateBinary(reader);
            Timer.LoadStateBinary(reader);
            YM2151.LoadStateBinary(reader);
            Sound.ym2151stream.output_sampindex = reader.ReadInt32();
            Sound.ym2151stream.output_base_sampindex = reader.ReadInt32();
            Sound.mixerstream.output_sampindex = reader.ReadInt32();
            Sound.mixerstream.output_base_sampindex = reader.ReadInt32();
        }
        public static void SaveStateBinary_mia(BinaryWriter writer)
        {
            int i;
            writer.Write(dsw1);
            writer.Write(dsw2);
            writer.Write(dsw3);
            writer.Write(init_eeprom_count);
            writer.Write(toggle);
            writer.Write(dim_c);
            writer.Write(dim_v);
            writer.Write(lastdim);
            writer.Write(lasten);
            writer.Write(sprite_colorbase);
            writer.Write(bg_colorbase);
            for (i = 0; i < 3; i++)
            {
                writer.Write(layer_colorbase[i]);
            }
            SaveStateBinary_K053251(writer);
            SaveStateBinary_K052109(writer);
            SaveStateBinary_K051960(writer);
            for (i = 0; i < 0x800; i++)
            {
                writer.Write(Generic.paletteram16[i]);
            }
            for (i = 0; i < 0x2000; i++)
            {
                writer.Write(Generic.spriteram16[i]);
            }
            for (i = 0; i < 0x800; i++)
            {
                writer.Write(Palette.entry_color[i]);
            }
            writer.Write(Memory.mainram, 0, 0x4000);
            writer.Write(mainram2, 0, 0x4000);
            MC68000.mm1[0].SaveStateBinary(writer);
            writer.Write(Memory.audioram, 0, 0x800);
            Z80A.zz1[0].SaveStateBinary(writer);
            Cpuint.SaveStateBinary(writer);
            writer.Write(Timer.global_basetime.seconds);
            writer.Write(Timer.global_basetime.attoseconds);
            Video.SaveStateBinary(writer);
            writer.Write(Sound.last_update_second);
            Cpuexec.SaveStateBinary(writer);
            Timer.SaveStateBinary(writer);
            YM2151.SaveStateBinary(writer);
            K007232.SaveStateBinary(writer);
            for (i = 0; i < 1; i++)
            {
                writer.Write(Sound.latched_value[i]);
            }
            for (i = 0; i < 1; i++)
            {
                writer.Write(Sound.utempdata[i]);
            }
            writer.Write(Sound.ym2151stream.output_sampindex);
            writer.Write(Sound.ym2151stream.output_base_sampindex);
            writer.Write(Sound.k007232stream.output_sampindex);
            writer.Write(Sound.k007232stream.output_base_sampindex);
            writer.Write(Sound.mixerstream.output_sampindex);
            writer.Write(Sound.mixerstream.output_base_sampindex);
        }
        public static void LoadStateBinary_mia(BinaryReader reader)
        {
            int i;
            dsw1 = reader.ReadByte();
            dsw2 = reader.ReadByte();
            dsw3 = reader.ReadByte();
            init_eeprom_count = reader.ReadInt32();
            toggle = reader.ReadInt32();
            dim_c = reader.ReadInt32();
            dim_v = reader.ReadInt32();
            lastdim = reader.ReadInt32();
            lasten = reader.ReadInt32();
            sprite_colorbase = reader.ReadInt32();
            bg_colorbase = reader.ReadInt32();
            for (i = 0; i < 3; i++)
            {
                layer_colorbase[i] = reader.ReadInt32();
            }
            LoadStateBinary_K053251(reader);
            LoadStateBinary_K052109(reader);
            LoadStateBinary_K051960(reader);
            for (i = 0; i < 0x800; i++)
            {
                Generic.paletteram16[i] = reader.ReadUInt16();
            }
            for (i = 0; i < 0x2000; i++)
            {
                Generic.spriteram16[i] = reader.ReadUInt16();
            }
            for (i = 0; i < 0x800; i++)
            {
                Palette.entry_color[i] = reader.ReadUInt32();
            }
            Memory.mainram = reader.ReadBytes(0x4000);
            mainram2 = reader.ReadBytes(0x4000);
            MC68000.mm1[0].LoadStateBinary(reader);
            Memory.audioram = reader.ReadBytes(0x800);
            Z80A.zz1[0].LoadStateBinary(reader);
            Cpuint.LoadStateBinary(reader);
            Timer.global_basetime.seconds = reader.ReadInt32();
            Timer.global_basetime.attoseconds = reader.ReadInt64();
            Video.LoadStateBinary(reader);
            Sound.last_update_second = reader.ReadInt32();
            Cpuexec.LoadStateBinary(reader);
            Timer.LoadStateBinary(reader);
            YM2151.LoadStateBinary(reader);
            K007232.LoadStateBinary(reader);
            for (i = 0; i < 1; i++)
            {
                Sound.latched_value[i] = reader.ReadUInt16();
            }
            for (i = 0; i < 1; i++)
            {
                Sound.utempdata[i] = reader.ReadUInt16();
            }
            Sound.ym2151stream.output_sampindex = reader.ReadInt32();
            Sound.ym2151stream.output_base_sampindex = reader.ReadInt32();
            Sound.k007232stream.output_sampindex = reader.ReadInt32();
            Sound.k007232stream.output_base_sampindex = reader.ReadInt32();
            Sound.mixerstream.output_sampindex = reader.ReadInt32();
            Sound.mixerstream.output_base_sampindex = reader.ReadInt32();
        }
        public static void SaveStateBinary_tmnt(BinaryWriter writer)
        {
            int i;
            writer.Write(dsw1);
            writer.Write(dsw2);
            writer.Write(dsw3);
            writer.Write(tmnt_soundlatch);
            for (i = 0; i < 0x40000; i++)
            {
                writer.Write(sampledata[i]);
            }
            writer.Write(init_eeprom_count);
            writer.Write(toggle);
            writer.Write(dim_c);
            writer.Write(dim_v);
            writer.Write(lastdim);
            writer.Write(lasten);
            writer.Write(sprite_colorbase);
            writer.Write(bg_colorbase);
            for (i = 0; i < 3; i++)
            {
                writer.Write(layer_colorbase[i]);
            }
            SaveStateBinary_K053251(writer);
            SaveStateBinary_K052109(writer);
            SaveStateBinary_K051960(writer);
            for (i = 0; i < 0x800; i++)
            {
                writer.Write(Generic.paletteram16[i]);
            }
            for (i = 0; i < 0x2000; i++)
            {
                writer.Write(Generic.spriteram16[i]);
            }
            for (i = 0; i < 0x800; i++)
            {
                writer.Write(Palette.entry_color[i]);
            }
            writer.Write(Memory.mainram, 0, 0x4000);
            MC68000.mm1[0].SaveStateBinary(writer);
            writer.Write(Memory.audioram, 0, 0x800);
            Z80A.zz1[0].SaveStateBinary(writer);
            Cpuint.SaveStateBinary(writer);
            writer.Write(Timer.global_basetime.seconds);
            writer.Write(Timer.global_basetime.attoseconds);
            Video.SaveStateBinary(writer);
            writer.Write(Sound.last_update_second);
            Cpuexec.SaveStateBinary(writer);
            Timer.SaveStateBinary(writer);
            YM2151.SaveStateBinary(writer);
            K007232.SaveStateBinary(writer);
            Upd7759.SaveStateBinary(writer);
            Sample.SaveStateBinary(writer);
            for (i = 0; i < 1; i++)
            {
                writer.Write(Sound.latched_value[i]);
            }
            for (i = 0; i < 1; i++)
            {
                writer.Write(Sound.utempdata[i]);
            }
            writer.Write(Sound.ym2151stream.output_sampindex);
            writer.Write(Sound.ym2151stream.output_base_sampindex);
            writer.Write(Sound.k007232stream.output_sampindex);
            writer.Write(Sound.k007232stream.output_base_sampindex);
            writer.Write(Sound.upd7759stream.output_sampindex);
            writer.Write(Sound.upd7759stream.output_base_sampindex);
            writer.Write(Sound.samplestream.output_sampindex);
            writer.Write(Sound.samplestream.output_base_sampindex);
            writer.Write(Sound.mixerstream.output_sampindex);
            writer.Write(Sound.mixerstream.output_base_sampindex);
        }
        public static void LoadStateBinary_tmnt(BinaryReader reader)
        {
            int i;
            dsw1 = reader.ReadByte();
            dsw2 = reader.ReadByte();
            dsw3 = reader.ReadByte();
            tmnt_soundlatch = reader.ReadInt32();
            for (i = 0; i < 0x40000; i++)
            {
                sampledata[i] = reader.ReadInt16();
            }
            init_eeprom_count = reader.ReadInt32();
            toggle = reader.ReadInt32();
            dim_c = reader.ReadInt32();
            dim_v = reader.ReadInt32();
            lastdim = reader.ReadInt32();
            lasten = reader.ReadInt32();
            sprite_colorbase = reader.ReadInt32();
            bg_colorbase = reader.ReadInt32();
            for (i = 0; i < 3; i++)
            {
                layer_colorbase[i] = reader.ReadInt32();
            }
            LoadStateBinary_K053251(reader);
            LoadStateBinary_K052109(reader);
            LoadStateBinary_K051960(reader);
            for (i = 0; i < 0x800; i++)
            {
                Generic.paletteram16[i] = reader.ReadUInt16();
            }
            for (i = 0; i < 0x2000; i++)
            {
                Generic.spriteram16[i] = reader.ReadUInt16();
            }
            for (i = 0; i < 0x800; i++)
            {
                Palette.entry_color[i] = reader.ReadUInt32();
            }
            Memory.mainram = reader.ReadBytes(0x4000);
            MC68000.mm1[0].LoadStateBinary(reader);
            Memory.audioram = reader.ReadBytes(0x800);
            Z80A.zz1[0].LoadStateBinary(reader);
            Cpuint.LoadStateBinary(reader);
            Timer.global_basetime.seconds = reader.ReadInt32();
            Timer.global_basetime.attoseconds = reader.ReadInt64();
            Video.LoadStateBinary(reader);
            Sound.last_update_second = reader.ReadInt32();
            Cpuexec.LoadStateBinary(reader);
            Timer.LoadStateBinary(reader);
            YM2151.LoadStateBinary(reader);
            K007232.LoadStateBinary(reader);
            Upd7759.LoadStateBinary(reader);
            Sample.LoadStateBinary(reader);
            for (i = 0; i < 1; i++)
            {
                Sound.latched_value[i] = reader.ReadUInt16();
            }
            for (i = 0; i < 1; i++)
            {
                Sound.utempdata[i] = reader.ReadUInt16();
            }
            Sound.ym2151stream.output_sampindex = reader.ReadInt32();
            Sound.ym2151stream.output_base_sampindex = reader.ReadInt32();
            Sound.k007232stream.output_sampindex = reader.ReadInt32();
            Sound.k007232stream.output_base_sampindex = reader.ReadInt32();
            Sound.upd7759stream.output_sampindex = reader.ReadInt32();
            Sound.upd7759stream.output_base_sampindex = reader.ReadInt32();
            Sound.samplestream.output_sampindex = reader.ReadInt32();
            Sound.samplestream.output_base_sampindex = reader.ReadInt32();
            Sound.mixerstream.output_sampindex = reader.ReadInt32();
            Sound.mixerstream.output_base_sampindex = reader.ReadInt32();
        }
        public static void SaveStateBinary_punkshot(BinaryWriter writer)
        {
            int i;
            writer.Write(dsw1);
            writer.Write(dsw2);
            writer.Write(dsw3);
            writer.Write(init_eeprom_count);
            writer.Write(toggle);
            writer.Write(dim_c);
            writer.Write(dim_v);
            writer.Write(lastdim);
            writer.Write(lasten);
            writer.Write(sprite_colorbase);
            writer.Write(bg_colorbase);
            for (i = 0; i < 3; i++)
            {
                writer.Write(layer_colorbase[i]);
            }
            SaveStateBinary_K053251(writer);
            SaveStateBinary_K052109(writer);
            SaveStateBinary_K051960(writer);
            for (i = 0; i < 0x800; i++)
            {
                writer.Write(Generic.paletteram16[i]);
            }
            for (i = 0; i < 0x2000; i++)
            {
                writer.Write(Generic.spriteram16[i]);
            }
            for (i = 0; i < 0x800; i++)
            {
                writer.Write(Palette.entry_color[i]);
            }
            writer.Write(Memory.mainram, 0, 0x4000);
            MC68000.mm1[0].SaveStateBinary(writer);
            writer.Write(Memory.audioram, 0, 0x800);
            Z80A.zz1[0].SaveStateBinary(writer);
            Cpuint.SaveStateBinary(writer);
            writer.Write(Timer.global_basetime.seconds);
            writer.Write(Timer.global_basetime.attoseconds);
            Video.SaveStateBinary(writer);
            writer.Write(Sound.last_update_second);
            Cpuexec.SaveStateBinary(writer);
            Timer.SaveStateBinary(writer);
            YM2151.SaveStateBinary(writer);
            K053260.SaveStateBinary(writer);
            writer.Write(Sound.ym2151stream.output_sampindex);
            writer.Write(Sound.ym2151stream.output_base_sampindex);
            writer.Write(Sound.k053260stream.output_sampindex);
            writer.Write(Sound.k053260stream.output_base_sampindex);
            writer.Write(Sound.mixerstream.output_sampindex);
            writer.Write(Sound.mixerstream.output_base_sampindex);
        }
        public static void LoadStateBinary_punkshot(BinaryReader reader)
        {
            int i;
            dsw1 = reader.ReadByte();
            dsw2 = reader.ReadByte();
            dsw3 = reader.ReadByte();
            init_eeprom_count = reader.ReadInt32();
            toggle = reader.ReadInt32();
            dim_c = reader.ReadInt32();
            dim_v = reader.ReadInt32();
            lastdim = reader.ReadInt32();
            lasten = reader.ReadInt32();
            sprite_colorbase = reader.ReadInt32();
            bg_colorbase = reader.ReadInt32();
            for (i = 0; i < 3; i++)
            {
                layer_colorbase[i] = reader.ReadInt32();
            }
            LoadStateBinary_K053251(reader);
            LoadStateBinary_K052109(reader);
            LoadStateBinary_K051960(reader);
            for (i = 0; i < 0x800; i++)
            {
                Generic.paletteram16[i] = reader.ReadUInt16();
            }
            for (i = 0; i < 0x2000; i++)
            {
                Generic.spriteram16[i] = reader.ReadUInt16();
            }
            for (i = 0; i < 0x800; i++)
            {
                Palette.entry_color[i] = reader.ReadUInt32();
            }            
            Memory.mainram = reader.ReadBytes(0x4000);
            MC68000.mm1[0].LoadStateBinary(reader);
            Memory.audioram = reader.ReadBytes(0x800);
            Z80A.zz1[0].LoadStateBinary(reader);
            Cpuint.LoadStateBinary(reader);
            Timer.global_basetime.seconds = reader.ReadInt32();
            Timer.global_basetime.attoseconds = reader.ReadInt64();
            Video.LoadStateBinary(reader);
            Sound.last_update_second = reader.ReadInt32();
            Cpuexec.LoadStateBinary(reader);
            Timer.LoadStateBinary(reader);
            YM2151.LoadStateBinary(reader);
            K053260.LoadStateBinary(reader);
            Sound.ym2151stream.output_sampindex = reader.ReadInt32();
            Sound.ym2151stream.output_base_sampindex = reader.ReadInt32();
            Sound.k053260stream.output_sampindex = reader.ReadInt32();
            Sound.k053260stream.output_base_sampindex = reader.ReadInt32();
            Sound.mixerstream.output_sampindex = reader.ReadInt32();
            Sound.mixerstream.output_base_sampindex = reader.ReadInt32();
        }
        public static void SaveStateBinary_lgtnfght(BinaryWriter writer)
        {
            int i;
            writer.Write(dsw1);
            writer.Write(dsw2);
            writer.Write(dsw3);
            writer.Write(init_eeprom_count);
            writer.Write(toggle);
            writer.Write(dim_c);
            writer.Write(dim_v);
            writer.Write(lastdim);
            writer.Write(lasten);
            writer.Write(sprite_colorbase);
            writer.Write(bg_colorbase);
            for (i = 0; i < 3; i++)
            {
                writer.Write(layer_colorbase[i]);
            }
            SaveStateBinary_K053251(writer);
            SaveStateBinary_K052109(writer);
            SaveStateBinary_K053245(writer);
            for (i = 0; i < 0x800; i++)
            {
                writer.Write(Generic.paletteram16[i]);
            }
            for (i = 0; i < 0x2000; i++)
            {
                writer.Write(Generic.spriteram16[i]);
            }
            for (i = 0; i < 0x800; i++)
            {
                writer.Write(Palette.entry_color[i]);
            }
            writer.Write(Memory.mainram, 0, 0x4000);
            MC68000.mm1[0].SaveStateBinary(writer);
            writer.Write(Memory.audioram, 0, 0x800);
            Z80A.zz1[0].SaveStateBinary(writer);
            Cpuint.SaveStateBinary(writer);
            writer.Write(Timer.global_basetime.seconds);
            writer.Write(Timer.global_basetime.attoseconds);
            Video.SaveStateBinary(writer);
            writer.Write(Sound.last_update_second);
            Cpuexec.SaveStateBinary(writer);
            Timer.SaveStateBinary(writer);
            YM2151.SaveStateBinary(writer);
            K053260.SaveStateBinary(writer);
            writer.Write(Sound.ym2151stream.output_sampindex);
            writer.Write(Sound.ym2151stream.output_base_sampindex);
            writer.Write(Sound.k053260stream.output_sampindex);
            writer.Write(Sound.k053260stream.output_base_sampindex);
            writer.Write(Sound.mixerstream.output_sampindex);
            writer.Write(Sound.mixerstream.output_base_sampindex);
        }
        public static void LoadStateBinary_lgtnfght(BinaryReader reader)
        {
            int i;
            dsw1 = reader.ReadByte();
            dsw2 = reader.ReadByte();
            dsw3 = reader.ReadByte();
            init_eeprom_count = reader.ReadInt32();
            toggle = reader.ReadInt32();
            dim_c = reader.ReadInt32();
            dim_v = reader.ReadInt32();
            lastdim = reader.ReadInt32();
            lasten = reader.ReadInt32();
            sprite_colorbase = reader.ReadInt32();
            bg_colorbase = reader.ReadInt32();
            for (i = 0; i < 3; i++)
            {
                layer_colorbase[i] = reader.ReadInt32();
            }
            LoadStateBinary_K053251(reader);
            LoadStateBinary_K052109(reader);
            LoadStateBinary_K053245(reader);
            for (i = 0; i < 0x800; i++)
            {
                Generic.paletteram16[i] = reader.ReadUInt16();
            }
            for (i = 0; i < 0x2000; i++)
            {
                Generic.spriteram16[i] = reader.ReadUInt16();
            }
            for (i = 0; i < 0x800; i++)
            {
                Palette.entry_color[i] = reader.ReadUInt32();
            }
            Memory.mainram = reader.ReadBytes(0x4000);
            MC68000.mm1[0].LoadStateBinary(reader);
            Memory.audioram = reader.ReadBytes(0x800);
            Z80A.zz1[0].LoadStateBinary(reader);
            Cpuint.LoadStateBinary(reader);
            Timer.global_basetime.seconds = reader.ReadInt32();
            Timer.global_basetime.attoseconds = reader.ReadInt64();
            Video.LoadStateBinary(reader);
            Sound.last_update_second = reader.ReadInt32();
            Cpuexec.LoadStateBinary(reader);
            Timer.LoadStateBinary(reader);
            YM2151.LoadStateBinary(reader);
            K053260.LoadStateBinary(reader);
            Sound.ym2151stream.output_sampindex = reader.ReadInt32();
            Sound.ym2151stream.output_base_sampindex = reader.ReadInt32();
            Sound.k053260stream.output_sampindex = reader.ReadInt32();
            Sound.k053260stream.output_base_sampindex = reader.ReadInt32();
            Sound.mixerstream.output_sampindex = reader.ReadInt32();
            Sound.mixerstream.output_base_sampindex = reader.ReadInt32();
        }
        public static void SaveStateBinary_blswhstl(BinaryWriter writer)
        {
            int i;
            writer.Write(bytee);
            writer.Write(init_eeprom_count);
            writer.Write(toggle);
            writer.Write(dim_c);
            writer.Write(dim_v);
            writer.Write(lastdim);
            writer.Write(lasten);
            writer.Write(sprite_colorbase);
            writer.Write(bg_colorbase);
            for (i = 0; i < 3; i++)
            {
                writer.Write(layer_colorbase[i]);
            }
            SaveStateBinary_K053251(writer);
            SaveStateBinary_K052109(writer);
            SaveStateBinary_K053245(writer);
            for (i = 0; i < 0x800; i++)
            {
                writer.Write(Generic.paletteram16[i]);
            }
            for (i = 0; i < 0x2000; i++)
            {
                writer.Write(Generic.spriteram16[i]);
            }
            for (i = 0; i < 0x800; i++)
            {
                writer.Write(Palette.entry_color[i]);
            }
            writer.Write(Memory.mainram, 0, 0x4000);
            MC68000.mm1[0].SaveStateBinary(writer);
            writer.Write(Memory.audioram, 0, 0x800);
            Z80A.zz1[0].SaveStateBinary(writer);
            Cpuint.SaveStateBinary(writer);
            writer.Write(Timer.global_basetime.seconds);
            writer.Write(Timer.global_basetime.attoseconds);
            Video.SaveStateBinary(writer);
            writer.Write(Sound.last_update_second);
            Cpuexec.SaveStateBinary(writer);
            Timer.SaveStateBinary(writer);
            YM2151.SaveStateBinary(writer);
            K053260.SaveStateBinary(writer);
            writer.Write(Sound.ym2151stream.output_sampindex);
            writer.Write(Sound.ym2151stream.output_base_sampindex);
            writer.Write(Sound.k053260stream.output_sampindex);
            writer.Write(Sound.k053260stream.output_base_sampindex);
            writer.Write(Sound.mixerstream.output_sampindex);
            writer.Write(Sound.mixerstream.output_base_sampindex);
            Eeprom.SaveStateBinary(writer);
        }
        public static void LoadStateBinary_blswhstl(BinaryReader reader)
        {
            int i;
            bytee = reader.ReadByte();
            init_eeprom_count = reader.ReadInt32();
            toggle = reader.ReadInt32();
            dim_c = reader.ReadInt32();
            dim_v = reader.ReadInt32();
            lastdim = reader.ReadInt32();
            lasten = reader.ReadInt32();
            sprite_colorbase = reader.ReadInt32();
            bg_colorbase = reader.ReadInt32();
            for (i = 0; i < 3; i++)
            {
                layer_colorbase[i] = reader.ReadInt32();
            }
            LoadStateBinary_K053251(reader);
            LoadStateBinary_K052109(reader);
            LoadStateBinary_K053245(reader);
            for (i = 0; i < 0x800; i++)
            {
                Generic.paletteram16[i] = reader.ReadUInt16();
            }
            for (i = 0; i < 0x2000; i++)
            {
                Generic.spriteram16[i] = reader.ReadUInt16();
            }
            for (i = 0; i < 0x800; i++)
            {
                Palette.entry_color[i] = reader.ReadUInt32();
            }
            Memory.mainram = reader.ReadBytes(0x4000);
            MC68000.mm1[0].LoadStateBinary(reader);
            Memory.audioram = reader.ReadBytes(0x800);
            Z80A.zz1[0].LoadStateBinary(reader);
            Cpuint.LoadStateBinary(reader);
            Timer.global_basetime.seconds = reader.ReadInt32();
            Timer.global_basetime.attoseconds = reader.ReadInt64();
            Video.LoadStateBinary(reader);
            Sound.last_update_second = reader.ReadInt32();
            Cpuexec.LoadStateBinary(reader);
            Timer.LoadStateBinary(reader);
            YM2151.LoadStateBinary(reader);
            K053260.LoadStateBinary(reader);
            Sound.ym2151stream.output_sampindex = reader.ReadInt32();
            Sound.ym2151stream.output_base_sampindex = reader.ReadInt32();
            Sound.k053260stream.output_sampindex = reader.ReadInt32();
            Sound.k053260stream.output_base_sampindex = reader.ReadInt32();
            Sound.mixerstream.output_sampindex = reader.ReadInt32();
            Sound.mixerstream.output_base_sampindex = reader.ReadInt32();
            Eeprom.LoadStateBinary(reader);
        }        
        public static void SaveStateBinary_glfgreat(BinaryWriter writer)
        {
            int i;
            writer.Write(dsw1);
            writer.Write(dsw2);
            writer.Write(dsw3);
            writer.Write(init_eeprom_count);
            writer.Write(toggle);
            writer.Write(dim_c);
            writer.Write(dim_v);
            writer.Write(lastdim);
            writer.Write(lasten);
            writer.Write(sprite_colorbase);
            writer.Write(bg_colorbase);
            for (i = 0; i < 3; i++)
            {
                writer.Write(layer_colorbase[i]);
            }
            SaveStateBinary_K053251(writer);
            SaveStateBinary_K052109(writer);
            SaveStateBinary_K053245(writer);
            SaveStateBinary_K053936(writer);
            for (i = 0; i < 0x800; i++)
            {
                writer.Write(Generic.paletteram16[i]);
            }
            for (i = 0; i < 0x2000; i++)
            {
                writer.Write(Generic.spriteram16[i]);
            }
            for (i = 0; i < 0x800; i++)
            {
                writer.Write(Palette.entry_color[i]);
            }
            writer.Write(Memory.mainram, 0, 0x4000);
            MC68000.mm1[0].SaveStateBinary(writer);
            writer.Write(Memory.audioram, 0, 0x800);
            Z80A.zz1[0].SaveStateBinary(writer);
            Cpuint.SaveStateBinary(writer);
            writer.Write(Timer.global_basetime.seconds);
            writer.Write(Timer.global_basetime.attoseconds);
            Video.SaveStateBinary(writer);
            writer.Write(Sound.last_update_second);
            Cpuexec.SaveStateBinary(writer);
            Timer.SaveStateBinary(writer);
            K053260.SaveStateBinary(writer);
            writer.Write(Sound.k053260stream.output_sampindex);
            writer.Write(Sound.k053260stream.output_base_sampindex);
            writer.Write(Sound.mixerstream.output_sampindex);
            writer.Write(Sound.mixerstream.output_base_sampindex);
        }
        public static void LoadStateBinary_glfgreat(BinaryReader reader)
        {
            int i;
            dsw1 = reader.ReadByte();
            dsw2 = reader.ReadByte();
            dsw3 = reader.ReadByte();
            init_eeprom_count = reader.ReadInt32();
            toggle = reader.ReadInt32();
            dim_c = reader.ReadInt32();
            dim_v = reader.ReadInt32();
            lastdim = reader.ReadInt32();
            lasten = reader.ReadInt32();
            sprite_colorbase = reader.ReadInt32();
            bg_colorbase = reader.ReadInt32();
            for (i = 0; i < 3; i++)
            {
                layer_colorbase[i] = reader.ReadInt32();
            }
            LoadStateBinary_K053251(reader);
            LoadStateBinary_K052109(reader);
            LoadStateBinary_K053245(reader);
            LoadStateBinary_K053936(reader);
            for (i = 0; i < 0x800; i++)
            {
                Generic.paletteram16[i] = reader.ReadUInt16();
            }
            for (i = 0; i < 0x2000; i++)
            {
                Generic.spriteram16[i] = reader.ReadUInt16();
            }
            for (i = 0; i < 0x800; i++)
            {
                Palette.entry_color[i] = reader.ReadUInt32();
            }
            Memory.mainram = reader.ReadBytes(0x4000);
            MC68000.mm1[0].LoadStateBinary(reader);
            Memory.audioram = reader.ReadBytes(0x800);
            Z80A.zz1[0].LoadStateBinary(reader);
            Cpuint.LoadStateBinary(reader);
            Timer.global_basetime.seconds = reader.ReadInt32();
            Timer.global_basetime.attoseconds = reader.ReadInt64();
            Video.LoadStateBinary(reader);
            Sound.last_update_second = reader.ReadInt32();
            Cpuexec.LoadStateBinary(reader);
            Timer.LoadStateBinary(reader);
            K053260.LoadStateBinary(reader);
            Sound.k053260stream.output_sampindex = reader.ReadInt32();
            Sound.k053260stream.output_base_sampindex = reader.ReadInt32();
            Sound.mixerstream.output_sampindex = reader.ReadInt32();
            Sound.mixerstream.output_base_sampindex = reader.ReadInt32();
        }
        public static void SaveStateBinary_tmnt2(BinaryWriter writer)
        {
            int i;
            for (i = 0; i < 0x10; i++)
            {
                writer.Write(tmnt2_1c0800[i]);
            }
            writer.Write(init_eeprom_count);
            writer.Write(toggle);
            writer.Write(dim_c);
            writer.Write(dim_v);
            writer.Write(lastdim);
            writer.Write(lasten);
            writer.Write(sprite_colorbase);
            writer.Write(bg_colorbase);
            for (i = 0; i < 3; i++)
            {
                writer.Write(layer_colorbase[i]);
            }
            SaveStateBinary_K053251(writer);
            SaveStateBinary_K052109(writer);
            SaveStateBinary_K053245(writer);
            for (i = 0; i < 0x800; i++)
            {
                writer.Write(Generic.paletteram16[i]);
            }
            for (i = 0; i < 0x2000; i++)
            {
                writer.Write(Generic.spriteram16[i]);
            }
            for (i = 0; i < 0x800; i++)
            {
                writer.Write(Palette.entry_color[i]);
            }
            writer.Write(Memory.mainram, 0, 0x4000);
            writer.Write(mainram2, 0, 0x80);
            MC68000.mm1[0].SaveStateBinary(writer);
            writer.Write(Memory.audioram, 0, 0x800);
            Z80A.zz1[0].SaveStateBinary(writer);
            Cpuint.SaveStateBinary(writer);
            writer.Write(Timer.global_basetime.seconds);
            writer.Write(Timer.global_basetime.attoseconds);
            Video.SaveStateBinary(writer);
            writer.Write(Sound.last_update_second);
            Cpuexec.SaveStateBinary(writer);
            Timer.SaveStateBinary(writer);
            YM2151.SaveStateBinary(writer);
            K053260.SaveStateBinary(writer);
            writer.Write(Sound.ym2151stream.output_sampindex);
            writer.Write(Sound.ym2151stream.output_base_sampindex);
            writer.Write(Sound.k053260stream.output_sampindex);
            writer.Write(Sound.k053260stream.output_base_sampindex);
            writer.Write(Sound.mixerstream.output_sampindex);
            writer.Write(Sound.mixerstream.output_base_sampindex);
            Eeprom.SaveStateBinary(writer);
        }
        public static void LoadStateBinary_tmnt2(BinaryReader reader)
        {
            int i;
            for (i = 0; i < 0x10; i++)
            {
                tmnt2_1c0800[i] = reader.ReadUInt16();
            }
            init_eeprom_count = reader.ReadInt32();
            toggle = reader.ReadInt32();
            dim_c = reader.ReadInt32();
            dim_v = reader.ReadInt32();
            lastdim = reader.ReadInt32();
            lasten = reader.ReadInt32();
            sprite_colorbase = reader.ReadInt32();
            bg_colorbase = reader.ReadInt32();
            for (i = 0; i < 3; i++)
            {
                layer_colorbase[i] = reader.ReadInt32();
            }
            LoadStateBinary_K053251(reader);
            LoadStateBinary_K052109(reader);
            LoadStateBinary_K053245(reader);
            for (i = 0; i < 0x800; i++)
            {
                Generic.paletteram16[i] = reader.ReadUInt16();
            }
            for (i = 0; i < 0x2000; i++)
            {
                Generic.spriteram16[i] = reader.ReadUInt16();
            }
            for (i = 0; i < 0x800; i++)
            {
                Palette.entry_color[i] = reader.ReadUInt32();
            }
            Memory.mainram = reader.ReadBytes(0x4000);
            mainram2 = reader.ReadBytes(0x80);
            MC68000.mm1[0].LoadStateBinary(reader);
            Memory.audioram = reader.ReadBytes(0x800);
            Z80A.zz1[0].LoadStateBinary(reader);
            Cpuint.LoadStateBinary(reader);
            Timer.global_basetime.seconds = reader.ReadInt32();
            Timer.global_basetime.attoseconds = reader.ReadInt64();
            Video.LoadStateBinary(reader);
            Sound.last_update_second = reader.ReadInt32();
            Cpuexec.LoadStateBinary(reader);
            Timer.LoadStateBinary(reader);
            YM2151.LoadStateBinary(reader);
            K053260.LoadStateBinary(reader);
            Sound.ym2151stream.output_sampindex = reader.ReadInt32();
            Sound.ym2151stream.output_base_sampindex = reader.ReadInt32();
            Sound.k053260stream.output_sampindex = reader.ReadInt32();
            Sound.k053260stream.output_base_sampindex = reader.ReadInt32();
            Sound.mixerstream.output_sampindex = reader.ReadInt32();
            Sound.mixerstream.output_base_sampindex = reader.ReadInt32();
            Eeprom.LoadStateBinary(reader);
        }
        public static void SaveStateBinary_ssriders(BinaryWriter writer)
        {
            int i;
            writer.Write(init_eeprom_count);
            writer.Write(toggle);
            writer.Write(dim_c);
            writer.Write(dim_v);
            writer.Write(lastdim);
            writer.Write(lasten);
            writer.Write(sprite_colorbase);
            writer.Write(bg_colorbase);
            for (i = 0; i < 3; i++)
            {
                writer.Write(layer_colorbase[i]);
            }
            SaveStateBinary_K053251(writer);
            SaveStateBinary_K052109(writer);
            SaveStateBinary_K053245(writer);
            for (i = 0; i < 0x800; i++)
            {
                writer.Write(Generic.paletteram16[i]);
            }
            for (i = 0; i < 0x2000; i++)
            {
                writer.Write(Generic.spriteram16[i]);
            }
            for (i = 0; i < 0x800; i++)
            {
                writer.Write(Palette.entry_color[i]);
            }
            writer.Write(Memory.mainram, 0, 0x4000);
            writer.Write(mainram2, 0, 0x80);
            MC68000.mm1[0].SaveStateBinary(writer);
            writer.Write(Memory.audioram, 0, 0x800);
            Z80A.zz1[0].SaveStateBinary(writer);
            Cpuint.SaveStateBinary(writer);
            writer.Write(Timer.global_basetime.seconds);
            writer.Write(Timer.global_basetime.attoseconds);
            Video.SaveStateBinary(writer);
            Drawgfx.SaveStateBinary(writer);
            writer.Write(Sound.last_update_second);
            Cpuexec.SaveStateBinary(writer);
            Timer.SaveStateBinary(writer);
            YM2151.SaveStateBinary(writer);
            K053260.SaveStateBinary(writer);
            writer.Write(Sound.ym2151stream.output_sampindex);
            writer.Write(Sound.ym2151stream.output_base_sampindex);
            writer.Write(Sound.k053260stream.output_sampindex);
            writer.Write(Sound.k053260stream.output_base_sampindex);
            writer.Write(Sound.mixerstream.output_sampindex);
            writer.Write(Sound.mixerstream.output_base_sampindex);
            Eeprom.SaveStateBinary(writer);
        }
        public static void LoadStateBinary_ssriders(BinaryReader reader)
        {
            int i;
            init_eeprom_count = reader.ReadInt32();
            toggle = reader.ReadInt32();
            dim_c = reader.ReadInt32();
            dim_v = reader.ReadInt32();
            lastdim = reader.ReadInt32();
            lasten = reader.ReadInt32();
            sprite_colorbase = reader.ReadInt32();
            bg_colorbase = reader.ReadInt32();
            for (i = 0; i < 3; i++)
            {
                layer_colorbase[i] = reader.ReadInt32();
            }
            LoadStateBinary_K053251(reader);
            LoadStateBinary_K052109(reader);
            LoadStateBinary_K053245(reader);
            for (i = 0; i < 0x800; i++)
            {
                Generic.paletteram16[i] = reader.ReadUInt16();
            }
            for (i = 0; i < 0x2000; i++)
            {
                Generic.spriteram16[i] = reader.ReadUInt16();
            }
            for (i = 0; i < 0x800; i++)
            {
                Palette.entry_color[i] = reader.ReadUInt32();
            }
            Memory.mainram = reader.ReadBytes(0x4000);
            mainram2 = reader.ReadBytes(0x80);
            MC68000.mm1[0].LoadStateBinary(reader);
            Memory.audioram = reader.ReadBytes(0x800);
            Z80A.zz1[0].LoadStateBinary(reader);
            Cpuint.LoadStateBinary(reader);
            Timer.global_basetime.seconds = reader.ReadInt32();
            Timer.global_basetime.attoseconds = reader.ReadInt64();
            Video.LoadStateBinary(reader);
            Drawgfx.LoadStateBinary(reader);
            Sound.last_update_second = reader.ReadInt32();
            Cpuexec.LoadStateBinary(reader);
            Timer.LoadStateBinary(reader);
            YM2151.LoadStateBinary(reader);
            K053260.LoadStateBinary(reader);
            Sound.ym2151stream.output_sampindex = reader.ReadInt32();
            Sound.ym2151stream.output_base_sampindex = reader.ReadInt32();
            Sound.k053260stream.output_sampindex = reader.ReadInt32();
            Sound.k053260stream.output_base_sampindex = reader.ReadInt32();
            Sound.mixerstream.output_sampindex = reader.ReadInt32();
            Sound.mixerstream.output_base_sampindex = reader.ReadInt32();
            Eeprom.LoadStateBinary(reader);
        }
        public static void SaveStateBinary_thndrx2(BinaryWriter writer)
        {
            int i;
            writer.Write(bytee);
            writer.Write(init_eeprom_count);
            writer.Write(toggle);
            writer.Write(dim_c);
            writer.Write(dim_v);
            writer.Write(lastdim);
            writer.Write(lasten);
            writer.Write(sprite_colorbase);
            writer.Write(bg_colorbase);
            for (i = 0; i < 3; i++)
            {
                writer.Write(layer_colorbase[i]);
            }
            SaveStateBinary_K053251(writer);
            SaveStateBinary_K052109(writer);
            SaveStateBinary_K051960(writer);
            for (i = 0; i < 0x800; i++)
            {
                writer.Write(Generic.paletteram16[i]);
            }
            for (i = 0; i < 0x2000; i++)
            {
                writer.Write(Generic.spriteram16[i]);
            }
            for (i = 0; i < 0x800; i++)
            {
                writer.Write(Palette.entry_color[i]);
            }
            writer.Write(Memory.mainram, 0, 0x4000);
            MC68000.mm1[0].SaveStateBinary(writer);
            writer.Write(Memory.audioram, 0, 0x800);
            Z80A.zz1[0].SaveStateBinary(writer);
            Cpuint.SaveStateBinary(writer);
            writer.Write(Timer.global_basetime.seconds);
            writer.Write(Timer.global_basetime.attoseconds);
            Video.SaveStateBinary(writer);
            writer.Write(Sound.last_update_second);
            Cpuexec.SaveStateBinary(writer);
            Timer.SaveStateBinary(writer);
            YM2151.SaveStateBinary(writer);
            K053260.SaveStateBinary(writer);
            writer.Write(Sound.ym2151stream.output_sampindex);
            writer.Write(Sound.ym2151stream.output_base_sampindex);
            writer.Write(Sound.k053260stream.output_sampindex);
            writer.Write(Sound.k053260stream.output_base_sampindex);
            writer.Write(Sound.mixerstream.output_sampindex);
            writer.Write(Sound.mixerstream.output_base_sampindex);
            Eeprom.SaveStateBinary(writer);
        }
        public static void LoadStateBinary_thndrx2(BinaryReader reader)
        {
            int i;
            bytee = reader.ReadByte();
            init_eeprom_count = reader.ReadInt32();
            toggle = reader.ReadInt32();
            dim_c = reader.ReadInt32();
            dim_v = reader.ReadInt32();
            lastdim = reader.ReadInt32();
            lasten = reader.ReadInt32();
            sprite_colorbase = reader.ReadInt32();
            bg_colorbase = reader.ReadInt32();
            for (i = 0; i < 3; i++)
            {
                layer_colorbase[i] = reader.ReadInt32();
            }
            LoadStateBinary_K053251(reader);
            LoadStateBinary_K052109(reader);
            LoadStateBinary_K051960(reader);
            for (i = 0; i < 0x800; i++)
            {
                Generic.paletteram16[i] = reader.ReadUInt16();
            }
            for (i = 0; i < 0x2000; i++)
            {
                Generic.spriteram16[i] = reader.ReadUInt16();
            }
            for (i = 0; i < 0x800; i++)
            {
                Palette.entry_color[i] = reader.ReadUInt32();
            }
            Memory.mainram = reader.ReadBytes(0x4000);
            MC68000.mm1[0].LoadStateBinary(reader);
            Memory.audioram = reader.ReadBytes(0x800);
            Z80A.zz1[0].LoadStateBinary(reader);
            Cpuint.LoadStateBinary(reader);
            Timer.global_basetime.seconds = reader.ReadInt32();
            Timer.global_basetime.attoseconds = reader.ReadInt64();
            Video.LoadStateBinary(reader);
            Sound.last_update_second = reader.ReadInt32();
            Cpuexec.LoadStateBinary(reader);
            Timer.LoadStateBinary(reader);
            YM2151.LoadStateBinary(reader);
            K053260.LoadStateBinary(reader);
            Sound.ym2151stream.output_sampindex = reader.ReadInt32();
            Sound.ym2151stream.output_base_sampindex = reader.ReadInt32();
            Sound.k053260stream.output_sampindex = reader.ReadInt32();
            Sound.k053260stream.output_base_sampindex = reader.ReadInt32();
            Sound.mixerstream.output_sampindex = reader.ReadInt32();
            Sound.mixerstream.output_base_sampindex = reader.ReadInt32();
            Eeprom.LoadStateBinary(reader);
        }
        public static void SaveStateBinary_prmrsocr(BinaryWriter writer)
        {
            int i;
            writer.Write(basebanksnd);
            writer.Write(init_eeprom_count);
            writer.Write(toggle);
            writer.Write(dim_c);
            writer.Write(dim_v);
            writer.Write(lastdim);
            writer.Write(lasten);
            writer.Write(sprite_colorbase);
            writer.Write(bg_colorbase);
            for (i = 0; i < 3; i++)
            {
                writer.Write(layer_colorbase[i]);
            }
            SaveStateBinary_K053251(writer);
            SaveStateBinary_K052109(writer);
            SaveStateBinary_K053245(writer);
            SaveStateBinary_K053936(writer);
            for (i = 0; i < 0x800; i++)
            {
                writer.Write(Generic.paletteram16[i]);
            }
            for (i = 0; i < 0x2000; i++)
            {
                writer.Write(Generic.spriteram16[i]);
            }
            for (i = 0; i < 0x800; i++)
            {
                writer.Write(Palette.entry_color[i]);
            }
            writer.Write(Memory.mainram, 0, 0x4000);
            MC68000.mm1[0].SaveStateBinary(writer);
            writer.Write(Memory.audioram, 0, 0x2000);
            Z80A.zz1[0].SaveStateBinary(writer);
            Cpuint.SaveStateBinary(writer);
            writer.Write(Timer.global_basetime.seconds);
            writer.Write(Timer.global_basetime.attoseconds);
            Video.SaveStateBinary(writer);
            writer.Write(Sound.last_update_second);
            Cpuexec.SaveStateBinary(writer);
            Timer.SaveStateBinary(writer);
            K054539.kk1[0].SaveStateBinary(writer);
            for (i = 0; i < 3; i++)
            {
                writer.Write(Sound.latched_value[i]);
            }
            for (i = 0; i < 3; i++)
            {
                writer.Write(Sound.utempdata[i]);
            }
            writer.Write(K054539.kk1[0].info.stream.output_sampindex);
            writer.Write(K054539.kk1[0].info.stream.output_base_sampindex);
            writer.Write(Sound.mixerstream.output_sampindex);
            writer.Write(Sound.mixerstream.output_base_sampindex);
            Eeprom.SaveStateBinary(writer);
        }
        public static void LoadStateBinary_prmrsocr(BinaryReader reader)
        {
            int i;
            basebanksnd = reader.ReadInt32();
            init_eeprom_count = reader.ReadInt32();
            toggle = reader.ReadInt32();
            dim_c = reader.ReadInt32();
            dim_v = reader.ReadInt32();
            lastdim = reader.ReadInt32();
            lasten = reader.ReadInt32();
            sprite_colorbase = reader.ReadInt32();
            bg_colorbase = reader.ReadInt32();
            for (i = 0; i < 3; i++)
            {
                layer_colorbase[i] = reader.ReadInt32();
            }
            LoadStateBinary_K053251(reader);
            LoadStateBinary_K052109(reader);
            LoadStateBinary_K053245(reader);
            LoadStateBinary_K053936(reader);
            for (i = 0; i < 0x800; i++)
            {
                Generic.paletteram16[i] = reader.ReadUInt16();
            }
            for (i = 0; i < 0x2000; i++)
            {
                Generic.spriteram16[i] = reader.ReadUInt16();
            }
            for (i = 0; i < 0x800; i++)
            {
                Palette.entry_color[i] = reader.ReadUInt32();
            }
            Memory.mainram = reader.ReadBytes(0x4000);
            MC68000.mm1[0].LoadStateBinary(reader);
            Memory.audioram = reader.ReadBytes(0x2000);
            Z80A.zz1[0].LoadStateBinary(reader);
            Cpuint.LoadStateBinary(reader);
            Timer.global_basetime.seconds = reader.ReadInt32();
            Timer.global_basetime.attoseconds = reader.ReadInt64();
            Video.LoadStateBinary(reader);
            Sound.last_update_second = reader.ReadInt32();
            Cpuexec.LoadStateBinary(reader);
            Timer.LoadStateBinary(reader);
            K054539.kk1[0].LoadStateBinary(reader);
            for (i = 0; i < 3; i++)
            {
                Sound.latched_value[i] = reader.ReadUInt16();
            }
            for (i = 0; i < 3; i++)
            {
                Sound.utempdata[i] = reader.ReadUInt16();
            }
            K054539.kk1[0].info.stream.output_sampindex = reader.ReadInt32();
            K054539.kk1[0].info.stream.output_base_sampindex = reader.ReadInt32();
            Sound.mixerstream.output_sampindex = reader.ReadInt32();
            Sound.mixerstream.output_base_sampindex = reader.ReadInt32();
            Eeprom.LoadStateBinary(reader);
        }
        public static void SaveStateBinary_mystwarr(BinaryWriter writer)
        {
            int i;
            writer.Write(dsw1);
            writer.Write(mw_irq_control);
            writer.Write(oinprion);
            writer.Write(cbparam);
            writer.Write(cur_sound_region);
            writer.Write(sub1_colorbase);
            writer.Write(last_psac_colorbase);
            writer.Write(gametype);
            writer.Write(roz_enable);
            writer.Write(roz_rombank);
            writer.Write(clip);
            SaveStateBinary_K055555(writer);
            SaveStateBinary_K054338(writer);
            SaveStateBinary_K056832(writer);
            SaveStateBinary_K055673(writer);            
            SaveStateBinary_konamigx(writer);
            for (i = 0; i < 0x1000; i++)
            {
                writer.Write(Generic.paletteram16[i]);
            }
            for (i = 0; i < 0x800; i++)
            {
                writer.Write(Palette.entry_color[i]);
            }
            for (i = 0; i < 0x1800; i++)
            {
                writer.Write(Palette.entry_color2[i]);
            }
            writer.Write(gx_workram, 0, 0x10000);
            writer.Write(mainram2, 0, 0x20);
            writer.Write(Memory.audioram, 0, 0x2000);
            writer.Write(audioram2, 0, 0x1d0);
            writer.Write(audioram3, 0, 0x1d0);
            MC68000.mm1[0].SaveStateBinary(writer);
            Z80A.zz1[0].SaveStateBinary(writer);
            Cpuint.SaveStateBinary(writer);
            writer.Write(Timer.global_basetime.seconds);
            writer.Write(Timer.global_basetime.attoseconds);
            Video.SaveStateBinary(writer);
            writer.Write(Sound.last_update_second);
            Cpuexec.SaveStateBinary(writer);
            Timer.SaveStateBinary(writer);
            K054539.kk1[0].SaveStateBinary(writer);
            K054539.kk1[1].SaveStateBinary(writer);
            for (i = 0; i < 3; i++)
            {
                writer.Write(Sound.latched_value[i]);
            }
            for (i = 0; i < 3; i++)
            {
                writer.Write(Sound.utempdata[i]);
            }
            writer.Write(K054539.kk1[0].info.stream.output_sampindex);
            writer.Write(K054539.kk1[0].info.stream.output_base_sampindex);
            writer.Write(K054539.kk1[1].info.stream.output_sampindex);
            writer.Write(K054539.kk1[1].info.stream.output_base_sampindex);
            writer.Write(Sound.mixerstream.output_sampindex);
            writer.Write(Sound.mixerstream.output_base_sampindex);
            Eeprom.SaveStateBinary(writer);
        }
        public static void LoadStateBinary_mystwarr(BinaryReader reader)
        {
            int i;
            dsw1 = reader.ReadByte();
            mw_irq_control = reader.ReadByte();
            oinprion = reader.ReadInt32();
            cbparam = reader.ReadInt32();
            cur_sound_region = reader.ReadInt32();
            sub1_colorbase = reader.ReadInt32();
            last_psac_colorbase = reader.ReadInt32();
            gametype = reader.ReadInt32();
            roz_enable = reader.ReadInt32();
            roz_rombank = reader.ReadInt32();
            clip = reader.ReadUInt16();
            LoadStateBinary_K055555(reader);
            LoadStateBinary_K054338(reader);
            LoadStateBinary_K056832(reader);
            LoadStateBinary_K055673(reader);            
            LoadStateBinary_konamigx(reader);
            for (i = 0; i < 0x1000; i++)
            {
                Generic.paletteram16[i] = reader.ReadUInt16();
            }
            for (i = 0; i < 0x800; i++)
            {
                Palette.entry_color[i] = reader.ReadUInt32();
            }
            for (i = 0; i < 0x1800; i++)
            {
                Palette.entry_color2[i] = reader.ReadUInt32();
            }
            gx_workram = reader.ReadBytes(0x10000);
            mainram2 = reader.ReadBytes(0x20);
            Memory.audioram = reader.ReadBytes(0x2000);
            audioram2 = reader.ReadBytes(0x1d0);
            audioram3 = reader.ReadBytes(0x1d0);
            MC68000.mm1[0].LoadStateBinary(reader);
            Z80A.zz1[0].LoadStateBinary(reader);
            Cpuint.LoadStateBinary(reader);
            Timer.global_basetime.seconds = reader.ReadInt32();
            Timer.global_basetime.attoseconds = reader.ReadInt64();
            Video.LoadStateBinary(reader);
            Sound.last_update_second = reader.ReadInt32();
            Cpuexec.LoadStateBinary(reader);
            Timer.LoadStateBinary(reader);
            K054539.kk1[0].LoadStateBinary(reader);
            K054539.kk1[1].LoadStateBinary(reader);
            for (i = 0; i < 3; i++)
            {
                Sound.latched_value[i] = reader.ReadUInt16();
            }
            for (i = 0; i < 3; i++)
            {
                Sound.utempdata[i] = reader.ReadUInt16();
            }
            K054539.kk1[0].info.stream.output_sampindex = reader.ReadInt32();
            K054539.kk1[0].info.stream.output_base_sampindex= reader.ReadInt32();
            K054539.kk1[1].info.stream.output_sampindex= reader.ReadInt32();
            K054539.kk1[1].info.stream.output_base_sampindex = reader.ReadInt32();
            Sound.mixerstream.output_sampindex= reader.ReadInt32();
            Sound.mixerstream.output_base_sampindex = reader.ReadInt32();
            Eeprom.LoadStateBinary(reader);
        }
    }
}
