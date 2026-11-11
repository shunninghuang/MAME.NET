using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mame
{
    public partial class sega
    {
        public static byte[] tilesrom, regs, rom1, rom2, maincpukeyrom, mcurom;
        public static ushort[] spritesrom, mainrom2;
        public static int basebanksnd;
        public static byte porth, coinage, dsw;
        public static bool b8751;
        public static void segaInit()
        {
            int i, n1, n;
            b8751 = false;
            misc_io_data = new byte[0x10];
            wwally_last_x = new byte[3];
            wwally_last_y = new byte[3];
            Memory.mainram = new byte[0x4000];
            Memory.audioram = new byte[0x2000];
            Generic.paletteram16 = new ushort[0x1000];
            segaic16_tileram_0 = new ushort[0x8000];
            segaic16_textram_0 = new ushort[0x800];
            segaic16_spriteram_0 = new ushort[0x400];
            regs = new byte[0x20];
            Machine.bRom = true;            
            Memory.audiorom = Machine.GetRom("audiocpu.rom");
            tilesrom = Machine.GetRom("tiles.rom");
            rom2 = Machine.GetRom("sprites.rom");
            n = rom2.Length;
            spritesrom = new ushort[n / 2];
            for (i = 0; i < n / 2; i++)
            {
                spritesrom[i] = (ushort)(rom2[i * 2] * 0x100 + rom2[i * 2 + 1]);
            }
            switch (Machine.sName)
            {
                case "astorm":
                case "astorm3":
                case "astormu":
                case "astormj":
                case "pontoon":
                case "pontoona":
                    Memory.mainrom = Machine.GetRom("maincpu.rom");
                    n = Memory.mainrom.Length;
                    mainrom2 = new ushort[n / 2];
                    for (i = 0; i < n / 2; i++)
                    {
                        mainrom2[i] = (ushort)(Memory.mainrom[i * 2] * 0x100 + Memory.mainrom[i * 2 + 1]);
                    }
                    maincpukeyrom = Machine.GetRom("maincpukey.rom");
                    porth = 0xff;
                    coinage = 0xff;
                    dsw = 0xfd;
                    if (maincpukeyrom == null || Memory.mainrom == null || Memory.audiorom == null || tilesrom == null || spritesrom == null)
                    {
                        Machine.bRom = false;
                    }
                    break;
                case "bloxeed":
                case "bloxeeda":
                    Memory.mainrom = Machine.GetRom("maincpu.rom");
                    n = Memory.mainrom.Length;
                    mainrom2 = new ushort[n / 2];
                    for (i = 0; i < n / 2; i++)
                    {
                        mainrom2[i] = (ushort)(Memory.mainrom[i * 2] * 0x100 + Memory.mainrom[i * 2 + 1]);
                    }
                    maincpukeyrom = Machine.GetRom("maincpukey.rom");
                    porth = 0xff;
                    coinage = 0xff;
                    dsw = 0x77;
                    if (maincpukeyrom == null || Memory.mainrom == null || Memory.audiorom == null || tilesrom == null || spritesrom == null)
                    {
                        Machine.bRom = false;
                    }
                    break;
                case "cltchitr":
                case "cltchitrj":
                case "desertbr":
                case "desertbrj":
                case "lghost":
                case "lghostu":
                case "lghostj":
                case "wwallyj":
                case "wwallyja":
                case "wwallyja3p":                
                    Memory.mainrom = Machine.GetRom("maincpu.rom");
                    n = Memory.mainrom.Length;
                    mainrom2 = new ushort[n / 2];
                    for (i = 0; i < n / 2; i++)
                    {
                        mainrom2[i] = (ushort)(Memory.mainrom[i * 2] * 0x100 + Memory.mainrom[i * 2 + 1]);
                    }
                    maincpukeyrom = Machine.GetRom("maincpukey.rom");
                    porth = 0xff;
                    coinage = 0xff;
                    dsw = 0xfd;
                    if (maincpukeyrom == null || Memory.mainrom == null || Memory.audiorom == null || tilesrom == null || spritesrom == null)
                    {
                        Machine.bRom = false;
                    }
                    break;
                case "ddcrew":
                case "ddcrewu":
                case "ddcrew2":
                case "ddcrew1":
                case "ddcrewj":
                case "ddcrewj2":
                    Memory.mainrom = Machine.GetRom("maincpu.rom");
                    n = Memory.mainrom.Length;
                    mainrom2 = new ushort[n / 2];
                    for (i = 0; i < n / 2; i++)
                    {
                        mainrom2[i] = (ushort)(Memory.mainrom[i * 2] * 0x100 + Memory.mainrom[i * 2 + 1]);
                    }
                    maincpukeyrom = Machine.GetRom("maincpukey.rom");
                    porth = 0xff;
                    coinage = 0xff;
                    dsw = 0xf7;
                    if (maincpukeyrom == null || Memory.mainrom == null || Memory.audiorom == null || tilesrom == null || spritesrom == null)
                    {
                        Machine.bRom = false;
                    }
                    break;
                case "hamaway":
                    Memory.mainrom = Machine.GetRom("maincpu.rom");
                    n = Memory.mainrom.Length;
                    mainrom2 = new ushort[n / 2];
                    for (i = 0; i < n / 2; i++)
                    {
                        mainrom2[i] = (ushort)(Memory.mainrom[i * 2] * 0x100 + Memory.mainrom[i * 2 + 1]);
                    }
                    porth = 0xff;
                    coinage = 0xff;
                    dsw = 0xbf;
                    if (Memory.mainrom == null || Memory.audiorom == null || tilesrom == null || spritesrom == null)
                    {
                        Machine.bRom = false;
                    }
                    break;
                case "mwalk":
                case "mwalkj":
                    b8751 = true;
                    Memory.mainrom = Machine.GetRom("maincpu.rom");
                    n = Memory.mainrom.Length;
                    mainrom2 = new ushort[n / 2];
                    for (i = 0; i < n / 2; i++)
                    {
                        mainrom2[i] = (ushort)(Memory.mainrom[i * 2] * 0x100 + Memory.mainrom[i * 2 + 1]);
                    }
                    maincpukeyrom = Machine.GetRom("maincpukey.rom");
                    mcurom = Machine.GetRom("mcu.rom");
                    porth = 0xff;
                    coinage = 0xff;
                    dsw = 0x41;
                    if (maincpukeyrom == null || Memory.mainrom == null || Memory.audiorom == null || tilesrom == null || spritesrom == null || mcurom == null)
                    {
                        Machine.bRom = false;
                    }
                    break;
                case "mwalku":
                    b8751 = true;
                    Memory.mainrom = Machine.GetRom("maincpu.rom");
                    n = Memory.mainrom.Length;
                    mainrom2 = new ushort[n / 2];
                    for (i = 0; i < n / 2; i++)
                    {
                        mainrom2[i] = (ushort)(Memory.mainrom[i * 2] * 0x100 + Memory.mainrom[i * 2 + 1]);
                    }
                    maincpukeyrom = Machine.GetRom("maincpukey.rom");
                    mcurom = Machine.GetRom("mcu.rom");
                    porth = 0xff;
                    coinage = 0xff;
                    dsw = 0x71;
                    if (maincpukeyrom == null || Memory.mainrom == null || Memory.audiorom == null || tilesrom == null || spritesrom == null || mcurom == null)
                    {
                        Machine.bRom = false;
                    }
                    break;
                case "shdancer":
                case "shdancerj":
                case "shdancer1":
                case "astorm3d":
                case "astormud":
                case "astormjd":
                case "cltchitrd":
                case "cltchitrjd":
                case "desertbrd":
                case "desertbrjd":
                case "lghostd":
                case "lghostud":
                case "wwallyjd":
                case "wwallyjad":
                    Memory.mainrom = Machine.GetRom("maincpu.rom");
                    n = Memory.mainrom.Length;
                    mainrom2 = new ushort[n / 2];
                    for (i = 0; i < n / 2; i++)
                    {
                        mainrom2[i] = (ushort)(Memory.mainrom[i * 2] * 0x100 + Memory.mainrom[i * 2 + 1]);
                    }
                    porth = 0xff;
                    coinage = 0xff;
                    dsw = 0xfd;
                    if (Memory.mainrom == null || Memory.audiorom == null || tilesrom == null || spritesrom == null)
                    {
                        Machine.bRom = false;
                    }
                    if (Machine.bRom)
                    {
                        porth = 0xff;
                        coinage = 0xff;
                        dsw = 0xfd;
                    }
                    break;
                case "bloxeedd":
                    Memory.mainrom = Machine.GetRom("maincpu.rom");
                    n = Memory.mainrom.Length;
                    mainrom2 = new ushort[n / 2];
                    for (i = 0; i < n / 2; i++)
                    {
                        mainrom2[i] = (ushort)(Memory.mainrom[i * 2] * 0x100 + Memory.mainrom[i * 2 + 1]);
                    }
                    if (Memory.mainrom == null || Memory.audiorom == null || tilesrom == null || spritesrom == null)
                    {
                        Machine.bRom = false;
                    }
                    if (Machine.bRom)
                    {
                        porth = 0xff;
                        coinage = 0xff;
                        dsw = 0x77;
                    }
                    break;
                case "ddcrewd":
                case "ddcrewud":
                case "ddcrew2d":
                case "ddcrew1d":
                case "ddcrewjd":
                case "ddcrewj2d":
                    Memory.mainrom = Machine.GetRom("maincpu.rom");
                    n = Memory.mainrom.Length;
                    mainrom2 = new ushort[n / 2];
                    for (i = 0; i < n / 2; i++)
                    {
                        mainrom2[i] = (ushort)(Memory.mainrom[i * 2] * 0x100 + Memory.mainrom[i * 2 + 1]);
                    }
                    if (Memory.mainrom == null || Memory.audiorom == null || tilesrom == null || spritesrom == null)
                    {
                        Machine.bRom = false;
                    }
                    if (Machine.bRom)
                    {
                        porth = 0xff;
                        coinage = 0xff;
                        dsw = 0x77;
                    }
                    break;
                case "mwalkd":                
                case "mwalkjd":
                    b8751 = true;
                    Memory.mainrom = Machine.GetRom("maincpu.rom");
                    n = Memory.mainrom.Length;
                    mainrom2 = new ushort[n / 2];
                    for (i = 0; i < n / 2; i++)
                    {
                        mainrom2[i] = (ushort)(Memory.mainrom[i * 2] * 0x100 + Memory.mainrom[i * 2 + 1]);
                    }
                    mcurom = Machine.GetRom("mcu.rom");
                    if (Memory.mainrom == null || Memory.audiorom == null || tilesrom == null || spritesrom == null || mcurom == null)
                    {
                        Machine.bRom = false;
                    }
                    if (Machine.bRom)
                    {
                        porth = 0xff;
                        coinage = 0xff;
                        dsw = 0x41;
                    }
                    break;
                case "mwalkud":
                    b8751 = true;
                    Memory.mainrom = Machine.GetRom("maincpu.rom");
                    n = Memory.mainrom.Length;
                    mainrom2 = new ushort[n / 2];
                    for (i = 0; i < n / 2; i++)
                    {
                        mainrom2[i] = (ushort)(Memory.mainrom[i * 2] * 0x100 + Memory.mainrom[i * 2 + 1]);
                    }
                    mcurom = Machine.GetRom("mcu.rom");
                    if (Memory.mainrom == null || Memory.audiorom == null || tilesrom == null || spritesrom == null || mcurom == null)
                    {
                        Machine.bRom = false;
                    }
                    if (Machine.bRom)
                    {
                        porth = 0xff;
                        coinage = 0xff;
                        dsw = 0x71;
                    }
                    break;
            }
        }
        public static void driver_init()
        {
            switch (Machine.sName)
            {
                case "astorm":
                case "astorm3":
                case "astormu":
                case "astormj":
                case "bloxeed":
                case "bloxeeda":
                case "mwalk":
                case "mwalku":
                case "mwalkj":
                case "pontoon":
                case "pontoona":
                case "astorm3d":
                case "astormud":
                case "astormjd":
                case "bloxeedd":
                case "mwalkd":
                case "mwalkud":
                case "mwalkjd":
                    system18_generic_init(2);
                    break;
                case "cltchitr":
                case "cltchitrj":
                case "desertbr":
                case "desertbrj":
                case "cltchitrd":
                case "cltchitrjd":
                case "desertbrd":
                case "desertbrjd":
                    system18_generic_init(3);
                    break;
                case "ddcrew":
                case "ddcrewu":
                case "ddcrew2":
                case "ddcrew1":
                case "ddcrewj":
                case "ddcrewj2":
                case "ddcrewd":
                case "ddcrewud":
                case "ddcrew2d":
                case "ddcrew1d":
                case "ddcrewjd":
                case "ddcrewj2d":
                    system18_generic_init(3);
                    custom_io_r = ddcrew_custom_io_r;
                    break;
                case "hamaway":
                    system18_generic_init(4);
                    break;
                case "lghost":
                case "lghostu":
                case "lghostj":
                case "lghostd":
                case "lghostud":
                    system18_generic_init(3);
                    custom_io_r = lghost_custom_io_r;
                    custom_io_w = lghost_custom_io_w;
                    break;
                case "shdancer":
                case "shdancerj":
                case "shdancer1":
                    system18_generic_init(1);
                    break;
                case "wwallyj":
                case "wwallyja":
                case "wwallyja3p":                
                case "wwallyjd":
                case "wwallyjad":
                    system18_generic_init(3);
                    wwally_last_x = new byte[3];
                    wwally_last_y = new byte[3];
                    custom_io_r = wwally_custom_io_r;
                    custom_io_w = wwally_custom_io_w;
                    break;
            }
        }
    }
}
