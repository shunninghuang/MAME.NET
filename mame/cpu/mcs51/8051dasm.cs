using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cpu.mcs51
{
    public partial class I8051
    {
        private static readonly string[] sfr =
        {
            "p0",     "sp",     "dpl",    "dph",    /* 80 - 83 */
            "adat",   "85h",    "86h",    "pcon",   /* 84 - 87 */
            "tcon",   "tmod",   "tl0",    "tl1",    /* 88 - 8b */
            "th0",    "th1",    "pwcm",   "pwmp",   /* 8a - 8f */
            "p1",     "91h",    "92h",    "93h",    /* 90 - 93 */
            "94h",    "95h",    "96h",    "97h",    /* 94 - 97 */
            "scon",   "sbuf",   "9ah",    "9bh",    /* 98 - 9b */
            "9ch",    "9dh",    "9eh",    "9fh",    /* 9c - 9f */
            "p2",     "0a1h",   "0a2h",   "0a3h",   /* a0 - a3 */
            "0a4h",   "0a5h",   "0a6h",   "0a7h",   /* a4 - a7 */
            "ie",     "cml0",   "cml1",   "cml2",   /* a8 - ab */
            "ctl0",   "ctl1",   "ctl2",   "ctl3",   /* ac - af */
            "p3",     "0b1h",   "0b2h",   "0b3h",   /* b0 - b3 */
            "0b4h",   "0b5h",   "0b6h",   "0b7h",   /* b4 - b7 */
            "ip",     "0b9h",   "0bah",   "0bbh",   /* b8 - bb */
            "0bch",   "0bdh",   "0beh",   "0bfh",   /* bc - bf */
            "p4",     "0c1h",   "0c2h",   "0c3h",   /* c0 - c3 */
            "p5",     "adcon",  "adch",   "0c7h",   /* c4 - c7 */
            "t2con",  "cmh0",   "rcap2l", "rcap2h", /* c8 - cb */
            "tl2",    "th2",    "cth2",   "cth3",   /* cc - cf */
            "psw",    "0d1h",   "0d2h",   "0d3h",   /* d0 - d3 */
            "0d4h",   "0d5h",   "0d6h",   "0d7h",   /* d4 - d7 */
            "i2cfg",  "s1sta",  "s1dat",  "s1adr",  /* d8 - db */
            "0dch",   "0ddh",   "0deh",   "0dfh",   /* dc - df */
            "acc",    "0e1h",   "0e2h",   "0e3h",   /* e0 - e3 */
            "0e4h",   "0e5h",   "0e6h",   "0e7h",   /* e4 - e7 */
            "csr",    "0e9h",   "tm2con", "ctcon",  /* e8 - eb */
            "tml2",   "tmh2",   "ste",    "rte",    /* ec - ef */
            "b",      "0f1h",   "0f2h",   "0f3h",   /* f0 - f3 */
            "0f4h",   "0f5h",   "0f6h",   "0f7h",   /* f4 - f7 */
            "i2sta",  "0f9h",   "0fah",   "0fbh",   /* f8 - fb */
            "pwm0",   "pwm1",   "pwena",  "t3"      /* fc - ff */
        };
        private static readonly string[] sfrbits =
        {
            "p0.0",   "p0.1",   "p0.2",   "p0.3",   /* 80 - 83 */
            "p0.4",   "p0.5",   "p0.6",   "p0.7",   /* 84 - 87 */
            "it0",    "ie0",    "it1",    "ie1",    /* 88 - 8b */
            "tr0",    "tf0",    "tr1",    "tf1",    /* 8c - 8f */
            "p1.0",   "p1.1",   "p1.2",   "p1.3",   /* 90 - 93 */
            "p1.4",   "p1.5",   "p1.6",   "p1.7",   /* 94 - 97 */
            "ri",     "ti",     "rb8",    "tb8",    /* 98 - 9b */
            "ren",    "sm2",    "sm1",    "sm0",    /* 9c - 9f */
            "p2.0",   "p2.1",   "p2.2",   "p2.3",   /* a0 - a3 */
            "p2.4",   "p2.5",   "p2.6",   "p2.7",   /* a4 - a7 */
            "ex0",    "et0",    "ex1",    "et1",    /* a8 - ab */
            "es",     "ie.5",   "ie.6",   "ea",     /* ac - af */
            "rxd",    "txd",    "int0",   "int1",   /* b0 - b3 */
            "t0",     "t1",     "wr",     "rd",     /* b4 - b7 */
            "px0",    "pt0",    "px1",    "pt1",    /* b8 - bb */
            "ps",     "ip.5",   "ip.6",   "ip.7",   /* bc - bf */
            "0c0h.0", "0c0h.1", "0c0h.2", "0c0h.3", /* c0 - c3 */
            "0c0h.4", "0c0h.5", "0c0h.6", "0c0h.7", /* c4 - c7 */
            "cprl2",  "ct2",    "tr2",    "exen2",  /* c8 - cb */
            "tclk",   "rclk",   "exf2",   "tf2",    /* cc - cf */
            "p",      "psw.1",  "ov",     "rs0",    /* d0 - d3 */
            "rs1",    "f0",     "ac",     "cy",     /* d4 - d7 */
            "ct0",    "ct1",    "i2cfg.2","i2cfg.3",/* d8 - db */
            "tirun",  "clrti",  "mastrq", "slaven", /* dc - df */
            "acc.0",  "acc.1",  "acc.2",  "acc.3",  /* e0 - e3 */
            "acc.4",  "acc.5",  "acc.6",  "acc.7",  /* e4 - e7 */
            "ibf",    "obf",    "idsm",   "obfc",   /* e8 - eb */
            "ma0",    "ma1",    "mb0",    "mb1",    /* ec - ef */
            "b.0",    "b.1",    "b.2",    "b.3",    /* f0 - f3 */
            "b.4",    "b.5",    "b.6",    "b.7",    /* f4 - f7 */
            "xstp",   "xstr",   "makstp", "makstr", /* f8 - fb */
            "xactv",  "xdata",  "idle",   "i2sta.7" /* fc - ff */
        };
        private static readonly string[] membits =
        {
            "20h.0",  "20h.1",  "20h.2",  "20h.3",
            "20h.4",  "20h.5",  "20h.6",  "20h.7",
            "21h.0",  "21h.1",  "21h.2",  "21h.3",
            "21h.4",  "21h.5",  "21h.6",  "21h.7",
            "22h.0",  "22h.1",  "22h.2",  "22h.3",
            "22h.4",  "22h.5",  "22h.6",  "22h.7",
            "23h.0",  "23h.1",  "23h.2",  "23h.3",
            "23h.4",  "23h.5",  "23h.6",  "23h.7",
            "24h.0",  "24h.1",  "24h.2",  "24h.3",
            "24h.4",  "24h.5",  "24h.6",  "24h.7",
            "25h.0",  "25h.1",  "25h.2",  "25h.3",
            "25h.4",  "25h.5",  "25h.6",  "25h.7",
            "26h.0",  "26h.1",  "26h.2",  "26h.3",
            "26h.4",  "26h.5",  "26h.6",  "26h.7",
            "27h.0",  "27h.1",  "27h.2",  "27h.3",
            "27h.4",  "27h.5",  "27h.6",  "27h.7",
            "28h.0",  "28h.1",  "28h.2",  "28h.3",
            "28h.4",  "28h.5",  "28h.6",  "28h.7",
            "29h.0",  "29h.1",  "29h.2",  "29h.3",
            "29h.4",  "29h.5",  "29h.6",  "29h.7",
            "2ah.0",  "2ah.1",  "2ah.2",  "2ah.3",
            "2ah.4",  "2ah.5",  "2ah.6",  "2ah.7",
            "2bh.0",  "2bh.1",  "2bh.2",  "2bh.3",
            "2bh.4",  "2bh.5",  "2bh.6",  "2bh.7",
            "2ch.0",  "2ch.1",  "2ch.2",  "2ch.3",
            "2ch.4",  "2ch.5",  "2ch.6",  "2ch.7",
            "2dh.0",  "2dh.1",  "2dh.2",  "2dh.3",
            "2dh.4",  "2dh.5",  "2dh.6",  "2dh.7",
            "2eh.0",  "2eh.1",  "2eh.2",  "2eh.3",
            "2eh.4",  "2eh.5",  "2eh.6",  "2eh.7",
            "2fh.0",  "2fh.1",  "2fh.2",  "2fh.3",
            "2fh.4",  "2fh.5",  "2fh.6",  "2fh.7"
        };
        private static readonly string[] regbank =
        {
            "rb0r0",  "rb0r1",  "rb0r2",  "rb0r3",
            "rb0r4",  "rb0r5",  "rb0r6",  "rb0r7",
            "rb1r0",  "rb1r1",  "rb1r2",  "rb1r3",
            "rb1r4",  "rb1r5",  "rb1r6",  "rb1r7",
            "rb2r0",  "rb2r1",  "rb2r2",  "rb2r3",
            "rb2r4",  "rb2r5",  "rb2r6",  "rb2r7",
            "rb3r0",  "rb3r1",  "rb3r2",  "rb3r3",
            "rb3r4",  "rb3r5",  "rb3r6",  "rb3r7"
        };
        public static string get_data_address(byte arg)
        {
            string buffer = "";
            if (arg < 0x80)
            {
                if (arg < 0x1f)
                {
                    buffer = regbank[arg];
                }
                else
                {
                    buffer = arg.ToString("X2");
                }
            }
            else
            {
                buffer = sfr[arg - 0x80];
            }
            return buffer;
        }
        public static string get_bit_address(byte arg)
        {
            string buffer = "";
            if (arg < 0x80)
            {

                if (arg < 0x7f)
                {
                    buffer = membits[arg];
                }
                else
                {
                    buffer = arg.ToString("X2");
                }
            }
            else
            {
                buffer = sfrbits[arg - 0x80];
            }
            return buffer;
        }
        public static uint i8051_dasm(out string dst, ushort pc, byte[] oprom, byte[] opram)
        {
            uint flags = 0;
            ushort PC = pc;
            string sym = "", sym2 = "";
            byte op, data;
            ushort addr;
            sbyte rel;
            dst = "";
            op = oprom[PC++ - pc];
            switch (op)
            {
                case 0x00:
                    dst = "nop";
                    break;
                case 0x01:
                case 0x21:
                case 0x41:
                case 0x61:
                case 0x81:
                case 0xa1:
                case 0xc1:
                case 0xe1:
                    addr = opram[PC++ - pc];
                    addr |= (ushort)((PC++ & 0xf800) | ((op & 0xe0) << 3));
                    dst = "ajmp " + addr.ToString("X4");
                    break;
                case 0x02:
                    addr = (ushort)((opram[PC++ - pc] << 8) & 0xff00);
                    addr |= opram[PC++ - pc];
                    dst = "ljmp" + addr.ToString("X4");
                    break;
                case 0x03:
                    dst = "rr    a";
                    break;
                case 0x04:
                    dst = "inc   a";
                    break;
                case 0x05:
                    sym = get_data_address(opram[PC++ - pc]);
                    dst = "inc   " + sym;
                    break;
                case 0x06:
                case 0x07:
                    dst = "inc   @r" + (op & 1).ToString();
                    break;
                case 0x08:
                case 0x09:
                case 0x0a:
                case 0x0b:
                case 0x0c:
                case 0x0d:
                case 0x0e:
                case 0x0f:
                    dst="inc   r"+(op & 7).ToString();
                    break;
                case 0x10:
                    sym = get_bit_address(opram[PC++ - pc]);
                    rel = (sbyte)opram[PC++ - pc];
                    dst = "jbc   " + sym +",$"+ (PC + rel).ToString("X4");
                    break;
                case 0x11:
                case 0x31:
                case 0x51:
                case 0x71:
                case 0x91:
                case 0xb1:
                case 0xd1:
                case 0xf1:
                    dst = "acall $" + ((PC & 0xf800) | ((op & 0xe0) << 3) | opram[PC - pc]).ToString("X4");
                    PC++;
                    flags = 0x20000000;
                    break;
                case 0x12:
                    addr = (ushort)((opram[PC++ - pc] << 8) & 0xff00);
                    addr |= opram[PC++ - pc];
                    dst = "lcall $" + addr.ToString("X4");
                    flags = 0x20000000;
                    break;
                case 0x13:
                    dst= "rrc   a";
                    break;
                case 0x14:
                    dst= "dec   a";
                    break;
                case 0x15:
                    sym = get_data_address(opram[PC++ - pc]);
                    dst = "dec   " + sym;
                    break;
                case 0x16:
                case 0x17:
                    dst = "dec   @r" + (op & 1).ToString();
                    break;
                case 0x18:
                case 0x19:
                case 0x1a:
                case 0x1b:
                case 0x1c:
                case 0x1d:
                case 0x1e:
                case 0x1f:
                    dst = "dec   r" + (op & 7).ToString();
                    break;
                case 0x20:
                    sym = get_bit_address(opram[PC++ - pc]);
                    rel = (sbyte)opram[PC++ - pc];
                    dst = "jb    " + sym + ",$" + (PC + rel).ToString("X4");
                    break;
                case 0x22:
                    dst = "ret";
                    flags = 0x40000000;
                    break;
                case 0x23:
                    dst= "rl    a";
                    break;
                case 0x24:
                    dst = "add   a,#$" + opram[PC++ - pc].ToString("X2");
                    break;
                case 0x25:
                    sym = get_data_address(opram[PC++ - pc]);
                    dst = "add   a," + sym;
                    break;
                case 0x26:
                case 0x27:
                    dst = "add   a,@r" + (op & 1).ToString();
                    break;
                case 0x28:
                case 0x29:
                case 0x2a:
                case 0x2b:
                case 0x2c:
                case 0x2d:
                case 0x2e:
                case 0x2f:
                    dst = "add   a,r" + (op & 7).ToString();
                    break;
                case 0x30:
                    sym = get_bit_address(opram[PC++ - pc]);
                    rel = (sbyte)opram[PC++ - pc];
                    dst = "jnb   " + sym + ",$" + (PC + rel).ToString("X4");
                    break;
                case 0x32:
                    dst = "reti";
                    flags = 0x40000000;
                    break;
                case 0x33:
                    dst="rlc   a";
                    break;
                case 0x34:
                    dst = "addc  a,#$" + opram[PC++ - pc].ToString("X2");
                    break;
                case 0x35:
                    sym = get_data_address(opram[PC++ - pc]);
                    dst = "addc  a," + sym;
                    break;
                case 0x36:
                case 0x37:
                    dst = "addc  a,@r" + (op & 1).ToString();
                    break;
                case 0x38:
                case 0x39:
                case 0x3a:
                case 0x3b:
                case 0x3c:
                case 0x3d:
                case 0x3e:
                case 0x3f:
                    dst = "addc  a,r" + (op & 7).ToString();
                    break;
                case 0x40:
                    rel = (sbyte)opram[PC++ - pc];
                    dst = "jc    $" + (PC + rel).ToString("X4");
                    break;
                case 0x42:
                    sym = get_data_address(opram[PC++ - pc]);
                    dst = "orl   " + sym + ",a";
                    break;
                case 0x43:
                    sym = get_data_address(opram[PC++ - pc]);
                    dst = "orl   " + sym + ",#$" + opram[PC++ - pc].ToString("X2");
                    break;
                case 0x44:
                    dst = "orl   a,#$" + opram[PC++ - pc].ToString("X2");
                    break;
                case 0x45:
                    sym = get_data_address(opram[PC++ - pc]);
                    dst = "orl   a," + sym;
                    break;
                case 0x46:
                case 0x47:
                    dst = "orl   a,@r" + (op & 1).ToString();
                    break;
                case 0x48:
                case 0x49:
                case 0x4a:
                case 0x4b:
                case 0x4c:
                case 0x4d:
                case 0x4e:
                case 0x4f:
                    dst = "orl   a,r" + (op & 7).ToString();
                    break;
                case 0x50:
                    rel = (sbyte)opram[PC++ - pc];
                    dst = "jnc   $" + (PC + rel).ToString("X4");
                    break;
                case 0x52:
                    sym = get_data_address(opram[PC++ - pc]);
                    dst = "anl   " + sym + ",a";
                    break;
                case 0x53:
                    sym = get_data_address(opram[PC++ - pc]);
                    dst = "anl   " + sym + ",#$" + opram[PC++ - pc].ToString("X2");
                    break;
                case 0x54:
                    dst = "anl   a,#$" + opram[PC++ - pc].ToString("X2");
                    break;
                case 0x55:
                    sym = get_data_address(opram[PC++ - pc]);
                    dst = "anl   a," + sym;
                    break;
                case 0x56:
                case 0x57:
                    dst = "anl   a,@r" + (op & 1).ToString();
                    break;
                case 0x58:
                case 0x59:
                case 0x5a:
                case 0x5b:
                case 0x5c:
                case 0x5d:
                case 0x5e:
                case 0x5f:
                    dst = "anl   a,r" + (op & 7).ToString();
                    break;
                case 0x60:
                    rel = (sbyte)opram[PC++ - pc];
                    dst = "jz    $" + (PC + rel).ToString("X4");
                    break;
                case 0x62:
                    sym = get_data_address(opram[PC++ - pc]);
                    dst = "xrl   " + sym + ",a";
                    break;
                case 0x63:
                    sym = get_data_address(opram[PC++ - pc]);
                    dst = "xrl   " + sym + ",#$" + opram[PC++ - pc].ToString("X2");
                    break;
                case 0x64:
                    dst = "xrl   a,#$" + opram[PC++ - pc].ToString("X2");
                    break;
                case 0x65:
                    sym = get_data_address(opram[PC++ - pc]);
                    dst = "xrl   a," + sym;
                    break;
                case 0x66:
                case 0x67:
                    dst = "xrl   a,@r" + (op & 1).ToString();
                    break;
                case 0x68:
                case 0x69:
                case 0x6a:
                case 0x6b:
                case 0x6c:
                case 0x6d:
                case 0x6e:
                case 0x6f:
                    dst = "xrl   a,r" + (op & 7).ToString();
                    break;
                case 0x70:
                    rel = (sbyte)opram[PC++ - pc];
                    dst = "jnz   $" + (PC + rel).ToString("X4");
                    break;
                case 0x72:
                    sym = get_bit_address(opram[PC++ - pc]);
                    dst = "orl   c," + sym;
                    break;
                case 0x73:
                    dst = "jmp   @a+dptr";
                    break;
                case 0x74:
                    dst = "mov   a,#$" + opram[PC++ - pc].ToString("X2");
                    break;
                case 0x75:
                    sym = get_data_address(opram[PC++ - pc]);
                    dst = "mov   " + sym + ",#$" + opram[PC++ - pc].ToString("X2");
                    break;
                case 0x76:
                case 0x77:
                    dst = "mov   @r" + (op & 1).ToString() + ",#$" + opram[PC++ - pc].ToString("X2");
                    break;
                case 0x78:
                case 0x79:
                case 0x7a:
                case 0x7b:
                case 0x7c:
                case 0x7d:
                case 0x7e:
                case 0x7f:
                    dst = "mov   r" + (op & 7).ToString() + ",#$" + opram[PC++ - pc].ToString("X2");
                    break;
                case 0x80:
                    rel = (sbyte)opram[PC++ - pc];
                    dst = "sjmp  $" + (PC + rel).ToString("X4");
                    break;
                case 0x82:
                    sym = get_bit_address(opram[PC++ - pc]);
                    dst = "anl   c," + sym;
                    break;
                case 0x83:
                    dst = "movc  a,@a+pc";
                    break;
                case 0x84:
                    dst = "div   ab";
                    break;
                case 0x85:
                    sym = get_data_address(opram[PC++ - pc]);
                    sym2 = get_data_address(opram[PC++ - pc]);
                    dst = "mov   " + sym2 + "," + sym;
                    break;
                case 0x86:
                case 0x87:
                    sym = get_data_address(opram[PC++ - pc]);
                    dst = "mov   " + sym + ",@r" + (op & 1).ToString();
                    break;
                case 0x88:
                case 0x89:
                case 0x8a:
                case 0x8b:
                case 0x8c:
                case 0x8d:
                case 0x8e:
                case 0x8f:
                    sym = get_data_address(opram[PC++ - pc]);
                    dst = "mov   " + sym + ",r" + (op & 7).ToString();
                    break;
                case 0x90:
                    addr = (ushort)((opram[PC++ - pc] << 8) & 0xff00);
                    addr |= opram[PC++ - pc];
                    dst = "mov   dptr,#$" + addr.ToString("X4");
                    break;
                case 0x92:
                    sym = get_bit_address(opram[PC++ - pc]);
                    dst = "mov   " + sym + ",c";
                    break;
                case 0x93:
                    dst = "movc  a,@a+dptr";
                    break;
                case 0x94:
                    dst = "subb  a,#$" + opram[PC++ - pc].ToString("X2");
                    break;
                case 0x95:
                    sym = get_data_address(opram[PC++ - pc]);
                    dst = "subb  a," + sym;
                    break;
                case 0x96:
                case 0x97:
                    dst = "subb  a,@r" + (op & 1).ToString();
                    break;
                case 0x98:
                case 0x99:
                case 0x9a:
                case 0x9b:
                case 0x9c:
                case 0x9d:
                case 0x9e:
                case 0x9f:
                    dst = "subb  a,r" + (op & 7).ToString();
                    break;
                case 0xa0:
                    sym = get_bit_address(opram[PC++ - pc]);
                    dst = "orl   c,/" + sym;
                    break;
                case 0xa2:
                    sym = get_bit_address(opram[PC++ - pc]);
                    dst = "mov   c," + sym;
                    break;
                case 0xa3:
                    dst = "inc   dptr";
                    break;
                case 0xa4:
                    dst = "mul   ab";
                    break;
                case 0xa5:
                    dst = "ill/rsv";
                    break;
                case 0xa6:
                case 0xa7:
                    sym = get_data_address(opram[PC++ - pc]);
                    dst = "mov   @r" + (op & 1).ToString() + "," + sym;
                    break;
                case 0xa8:
                case 0xa9:
                case 0xaa:
                case 0xab:
                case 0xac:
                case 0xad:
                case 0xae:
                case 0xaf:
                    sym = get_data_address(opram[PC++ - pc]);
                    dst = "mov   r" + (op & 7).ToString() + "," + sym;
                    break;
                case 0xb0:
                    sym = get_bit_address(opram[PC++ - pc]);
                    dst = "anl   c,/" + sym;
                    break;
                case 0xb2:
                    sym = get_bit_address(opram[PC++ - pc]);
                    dst = "cpl   " + sym;
                    break;
                case 0xb3:
                    dst = "cpl   c";
                    break;
                case 0xb4:
                    addr = opram[PC++ - pc];
                    rel = (sbyte)opram[PC++ - pc];
                    dst = "cjne  a,#$" + addr.ToString("X2") + ",$" + (PC + rel).ToString("X4");
                    break;
                case 0xb5:
                    sym = get_data_address(opram[PC++ - pc]);
                    rel = (sbyte)opram[PC++ - pc];
                    dst = "cjne  a," + sym + ",$" + (PC + rel).ToString("X4");
                    break;
                case 0xb6:
                case 0xb7:
                    addr = opram[PC++ - pc];
                    rel = (sbyte)opram[PC++ - pc];
                    dst = "cjne  @r" + (op & 1).ToString() + ",#$" + addr.ToString("X2") + ",$" + (PC + rel).ToString("X4");
                    break;
                case 0xb8:
                case 0xb9:
                case 0xba:
                case 0xbb:
                case 0xbc:
                case 0xbd:
                case 0xbe:
                case 0xbf:
                    addr = opram[PC++ - pc];
                    rel = (sbyte)opram[PC++ - pc];
                    dst = "cjne  r" + (op & 7).ToString() + ",#$" + addr.ToString("X2") + ",$" + (PC + rel).ToString("X4");
                    break;
                case 0xc0:
                    sym = get_data_address(opram[PC++ - pc]);
                    dst = "push  " + sym;
                    break;
                case 0xc2:
                    sym = get_bit_address(opram[PC++ - pc]);
                    dst = "clr   " + sym;
                    break;
                case 0xc3:
                    dst = "clr   c";
                    break;
                case 0xc4:
                    dst = "swap  a";
                    break;
                case 0xc5:
                    sym = get_data_address(opram[PC++ - pc]);
                    dst = "xch   a," + sym;
                    break;
                case 0xc6:
                case 0xc7:
                    dst = "xch   a,@r" + (op & 1).ToString();
                    break;
                case 0xc8:
                case 0xc9:
                case 0xca:
                case 0xcb:
                case 0xcc:
                case 0xcd:
                case 0xce:
                case 0xcf:
                    dst = "xch   a,r" + (op & 7).ToString();
                    break;
                case 0xd0:
                    sym = get_data_address(opram[PC++ - pc]);
                    dst = "pop   " + sym;
                    break;
                case 0xd2:
                    sym = get_bit_address(opram[PC++ - pc]);
                    dst = "setb  " + sym;
                    break;
                case 0xd3:
                    dst = "setb  c";
                    break;
                case 0xd4:
                    dst = "da    a";
                    break;
                case 0xd5:
                    sym = get_data_address(opram[PC++ - pc]);
                    rel = (sbyte)opram[PC++ - pc];
                    dst = "djnz  " + sym + ",$" + (PC + rel).ToString("X4");
                    flags = 0x20000000;
                    break;
                case 0xd6:
                case 0xd7:
                    dst = "xchd  a,@r" + (op & 1).ToString();
                    break;
                case 0xd8:
                case 0xd9:
                case 0xda:
                case 0xdb:
                case 0xdc:
                case 0xdd:
                case 0xde:
                case 0xdf:
                    rel = (sbyte)opram[PC++ - pc];
                    dst = "djnz  r" + (op & 7).ToString() + ",$" + (PC + rel).ToString("X4");
                    flags = 0x20000000;
                    break;
                case 0xe0:
                    dst = "movx  a,@dptr";
                    break;
                case 0xe2:
                case 0xe3:
                    dst = "movx  a,@r" + (op & 1).ToString();
                    break;
                case 0xe4:
                    dst = "clr   a";
                    break;
                case 0xe5:
                    sym = get_data_address(opram[PC++ - pc]);
                    dst = "mov   a," + sym;
                    break;
                case 0xe6:
                case 0xe7:
                    dst = "mov   a,@r" + (op & 1).ToString();
                    break;
                case 0xe8:
                case 0xe9:
                case 0xea:
                case 0xeb:
                case 0xec:
                case 0xed:
                case 0xee:
                case 0xef:
                    dst = "mov   a,r" + (op & 7).ToString();
                    break;
                case 0xf0:
                    dst = "movx  @dptr,a";
                    break;
                case 0xf2:
                case 0xf3:
                    dst = "movx  @r" + (op & 1).ToString() + ",a";
                    break;
                case 0xf4:
                    dst = "cpl   a";
                    break;
                case 0xf5:
                    sym = get_data_address(opram[PC++ - pc]);
                    dst = "mov   " + sym + ",a";
                    break;
                case 0xf6:
                case 0xf7:
                    dst = "mov   @r" + (op & 1).ToString() + ",a";
                    break;
                case 0xf8:
                case 0xf9:
                case 0xfa:
                case 0xfb:
                case 0xfc:
                case 0xfd:
                case 0xfe:
                case 0xff:
                    dst = "mov   r" + (op & 7).ToString() + ",a";
                    break;
                default:
                    dst = "illegal";
                    break;
            }
            return (uint)((uint)(PC - pc) | flags | 0x80000000);
        }
    }
}
