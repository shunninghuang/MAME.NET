# MAME.NET
C# arcade emulator

This article looks at: How Main.NET loads ROMs, hacking ROMs with the M68000 debugger and Z80 debugger, and using a game example of how to do that.
## Introduction
MAME (Multiple Arcade Machine Emulator) is a free and open source emulator designed to recreate the hardware of arcade game system in software on modern personal computers and other platforms. MAME.NET is a C# based arcade emulator, and it maintains the same architecture of MAME. By using C# and the powerful integrated development environment -- Microsoft Visual Studio, there is no macro and you can debug the supported arcade game anywhere. There are some classic boards supported by now: M72, M92, Taito B, Capcom, CPS-1, CPS-1(Qsound), CPS2, Data East, Tehkan, Konami 68000, Neo Geo, Technos, Namco System 1, IGS011, PGM(PolyGame Master).

MAME.NET runs at the following steps: load the ROMs, initialize the machine, soft reset the machine, and loop "cpuexec_timeslice" operation. The "cpuexec_timeslice" operation means sequentially execute every CPU for a time slice, and execute timer callbacks. Timer callbacks contains: video update, soft reset, CPU interrupt, sound update, watchdog reset and other interrupts. By these steps, MAME.NET emulates the arcade board successfully. MAME.NET has more functions: save and load state, record and replay input, cheat, cheat search, IPS (patch main ROM), board debugger, CPU debugger.
## Load the ROMs
As an emulator, MAME.NET loads ROMs first.

There are two CPUs working in Irem M72 board: a NEC V30 CPU and a Zilog Z80 CPU. There are 20-bits address and 16-bits data in V30. There are 16-bits address and 8-bits data in Z80. The V30 CPU runs at 8MHz. The program loads the file "maincpu.rom" as V30 ROM. The Z80 CPU runs at 3579545 Hz. The program loads the file "soundcpu.rom" as Z80 ROM. There are two sound chips: Yamaha YM2151, DAC chip.

There are two CPUs working in Irem M92 board: a NEC V33 CPU and a NEC V30 CPU. There are 20-bits address and 16-bits data in both CPUs. The V33 CPU runs at 9MHz. The program loads the file "maincpu.rom" as V33 ROM. The V30 CPU runs at 7159090 Hz. The program loads the file "soundcpu.rom" as V30 ROM. There are two sound chips: Yamaha YM2151, Irem GA20.

For Taito board, there are various CPU and chip settings.

For Taito B board, the M68000 CPU runs at 12MHz (16Mhz for some games). The program loads the file "maincpu.rom" as M68000 ROM. The file "gfx1.rom" contains tile data to display on screen. The Z80 CPU runs at 4MHz. The program loads the file "audiocpu.rom" as Z80 ROM. There are two sound chips: General Instrument AY8910, Yamaha YM2610. The program loads the file "ymsnd.rom" and the optional file "ymsnddeltat.rom" as the Yamaha YM2610 ROM.

For Capcom board, there are various CPU and chip settings.

There are 2 CPUs working in CPS-1, CPS-1(Qsound), CPS2, Neo Geo, PGM boards: a Motorola M68000 CPU and a Zilog Z80 CPU. There are 24-bits address and 16-bits data in M68000. There are 16-bits address and 8-bits data in Z80.

For CPS-1 board, the M68000 CPU runs at 10MHz (12MHz for some games). The program loads the file "maincpu.rom" as M68000 ROM. Size of "maincpu.rom" should be no greater than 0x400000 bytes. The file "gfx.rom" contains tile data to display on screen. The Z80 CPU runs at 3579545 Hz. The program loads the file "audiocpu.rom" as Z80 ROM. Size of "audiocpu.rom" should be no greater than 0x18000 bytes. There are two sound chips: Yamaha YM2151, Oki MSM6295. The program loads the file "oki.rom" as the Oki MSM6295 ROM.

For CPS-1(QSound) board, the M68000 CPU runs at 12MHz. The program loads the file "maincpu.rom" as M68000 ROM. Size of "maincpu.rom" should be no greater than 0x400000 bytes. The file "gfx.rom" contains tile data to display on screen. The Z80 CPU runs at 8MHz with Kabuki encrypted code. The program loads the file "audiocpu.rom" as Z80 ROM for ReadMemory only, the file "audiocpuop.rom" as the Z80 ROM for ReadOp only. Size of "audiocpu.rom" should not be greater than 0x50000 bytes. There is a Q-Sound chip. The program loads the file "qsound.rom" as the Q-Sound chip ROM.

For CPS2 board, the M68000 CPU runs at 11.8MHz. The original M68000 ROM is encrypted. The program loads the file "maincpu.rom" as M68000 ROM for memory code, the file "maincpuop.rom" as M68000 ROM for opcode. The file "gfx.rom" contains tile data to display on screen. The Z80 CPU runs at 8MHz. The program loads the file "audiocpu.rom" as Z80 ROM. Size of "audiocpu.rom" should not be greater than 0x50000 bytes. There is a Q-Sound chip. The program loads the file "qsound.rom" as the Q-Sound chip ROM.

For Data East board, the main M6502 CPU runs at 2MHz and the sound M6502 CPU runs at 1.5MHz. The file gfx1.rom, gfx2.rom contain tile data to display on screen. There are four sound chips: General Instrument AY8910, Yamaha YM2203, Yamaha YM3812, Oki MSM5205.

For Tehkan board, there are various CPU and chip settings.

For Konami 68000 board, there are various CPU and chip settings. There are Konami graphics chips: K052109, K051960, K053245, K054000, K053936 and K053251. There are relevant sound chips: Yamaha YM2151, Konami K007232, Konami K053260, Konami K054539, NEC UPD7759.

For Neo Geo board, the M68000 CPU runs at 12MHz. The program loads the file "maincpu.rom" as M68000 ROM. The file "sprites.rom" contains tile data to display on screen. The file "fixed.rom" is a graphic related ROM. The Z80 CPU runs at 4MHz. The program loads the file "audiocpu.rom" as Z80 ROM. There are two sound chips: General Instrument AY8910, Yamaha YM2610. The program loads the file "ymsnd.rom" and the optional file "ymsnddeltat.rom" as the Yamaha YM2610 ROM.

For Technos board, there are various CPU and chip settings.

For PGM board, the M68000 CPU runs at 20MHz. The program loads the file "maincpu.rom" as M68000 ROM. The files "sprcol.rom", "sprmask.rom", "tiles.rom" contain tile data to display on screen. The program dynamically writes the Z80 ROM. There is an ICS2115 sound chip.

There are four CPUs working in Namco System 1 board: three Motorola M6809 CPUs and one Hitachi HD63701 CPU. There are 16-bits address and 8-bits data in the four CPUs.

For Namco System 1, the four CPUs run at 1536000Hz. The Namco Custom 117 MMU provides a 23 bits virtual address for the first and second M6809 CPUs. The program loads the file "user1.rom" as the two CPU ROM. The files "gfx1.rom", "gfx2.rom", "gfx3.rom" contain tile data to display on screen. The program loads the file "audiocpu.rom" as the third M6809 CPU ROM. The program loads the file "voice.rom" as the HD63701 CPU ROM. There are three sound chips: Yamaha YM2151, Namco 8-voice stereo chip, DAC chip.

For IGS011 board, the M68000 CPU runs at 7333333Hz. The program loads the file "maincpu.rom" as M68000 ROM. The file "gfx1.rom" contains tile data to display on screen. There are two sound chips: Oki MSM6295 and Yamaha YM3812. The program loads the file "oki.rom" as the Oki MSM6295 ROM.

The ROM format of MAME.NET is the simplest. You can open and disassemble "maincpu.rom" and "audiocpu.rom" (only not opcode encrypted) with IDA Pro directly. There is no combination of multiple ROMs, no graphic effects decoding, no various decoding, and no byte swap. So the IPS file (.cht extension, the same as cheat file) is easy to understand. ROM hackers can focus on the real address-value pair and neglect ROM encoding and decoding. You can also disassemble CPU ROMs with the M68000 debugger and Z80 debugger functions in MAME.NET.

## Common Usage
Build environment: I only test on Windows 11 Professional, Microsoft Visual Studio 2008.

Operating environment: Microsoft .NET Framework 3.5 or higher.

### Hotkey:
- F3 -- soft reset,
- F7 – load state,
- Shift+F7 – save state,
- F8 – replay input,
- Shift+F8 – record input (start and stop),
- 0-9 and A-Z after state related hotkey – handle certain files,
- F10 – toggle global throttle,
- P – pause and continue,
- Shift+P - skip a frame.
### Control key:
- 1 -- P1 start,
- 2 -- P2 start,
- 5 -- P1 coin,
- 6 -- P2 coin,
- R -- Service 1,
- T – Service,
- W -- P1 up,
- S -- P1 down,
- A -- P1 left,
- D -- P1 right,
- J -- P1 button1,
- K -- P1 button 2,
- L -- P1 button 3,
- U -- P1 button 4,
- I -- P1 button 5,
- O -- P1 button 6,
- Up -- P2 up,
- Down -- P2 down,
- Left -- P2 left,
- Right -- P2 right,
- NumPad1 -- P2 button 1,
- NumPad2 -- P2 button 2,
- NumPad3 -- P2 button 3,
- NumPad4 -- P2 button 4,
- NumPad5 -- P2 button 5,
- NumPad6 -- P2 button 6.
Mouse supported games: Operation Wolf.

When the ROMs of a game are loaded, the emulator is auto paused. You can apply IPS and dip-switch now and press P to continue. You can get the proper dip-switch value from running MAME.

Occasionally GDI+ error occurs and a red cross is shown. You can click "File-Reset picturebox" to handle the error.

You can make the MAME.NET cheat file refer to cheat file of MAME or other emulators. You should make the ^1 operation to the cheat address for some emulators (for example: Winkawaks).

There are two files for record input. The .sta file records the initial state and the .inp file records the input key.
