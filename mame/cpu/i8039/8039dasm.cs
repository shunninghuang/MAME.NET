using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cpu.i8039
{
    public static class Disassembler8039
    {
        public const uint DASMFLAG_SUPPORTED = 0x80000000;
        public const uint DASMFLAG_STEP_OVER = 0x40000000;
        public const uint DASMFLAG_STEP_OUT = 0x20000000;
        private static readonly string[] Formats = {
            "00000011dddddddd", "add  a,#$%X",
            "01101rrr", "add  a,%R",
            "0110000r", "add  a,@%R",
            "00010011dddddddd", "adc  a,#$%X",
            "01111rrr", "adc  a,%R",
            "0111000r", "adc  a,@%R",
            "01010011dddddddd", "anl  a,#$%X",
            "01011rrr", "anl  a,%R",
            "0101000r", "anl  a,@%R",
            "10011000dddddddd", "anl  bus,#$%X",
            "10011001dddddddd", "anl  p1,#$%X",
            "10011010dddddddd", "anl  p2,#$%X",
            "100111pp", "anld %P,a",
            "aaa10100aaaaaaaa", "!call %A",
            "00100111", "clr  a",
            "10010111", "clr  c",
            "10100101", "clr  f1",
            "10000101", "clr  f0",
            "00110111", "cpl  a",
            "10100111", "cpl  c",
            "10010101", "cpl  f0",
            "10110101", "cpl  f1",
            "01010111", "da   a",
            "00000111", "dec  a",
            "11001rrr", "dec  %R",
            "00010101", "dis  i",
            "00110101", "dis  tcnti",
            "11101rrraaaaaaaa", "!djnz %R,%J",
            "00000101", "en   i",
            "00100101", "en   tcnti",
            "01110101", "ent0 clk",
            "00001001", "in   a,p1",
            "00001010", "in   a,p2",
            "00010111", "inc  a",
            "00011rrr", "inc  %R",
            "0001000r", "inc  @%R",
            "00001000", "ins  a,bus",
            "0001 0110aaaaaaaa", "jtf  %J",
            "0010 0110aaaaaaaa", "jnt0 %J",
            "0011 0110aaaaaaaa", "jt0  %J",
            "0100 0110aaaaaaaa", "jnt1 %J",
            "0101 0110aaaaaaaa", "jt1  %J",
            "0111 0110aaaaaaaa", "jf1  %J",
            "1000 0110aaaaaaaa", "jni  %J",
            "1001 0110aaaaaaaa", "jnz  %J",
            "1011 0110aaaaaaaa", "jf0  %J",
            "1100 0110aaaaaaaa", "jz   %J",
            "1110 0110aaaaaaaa", "jnc  %J",
            "1111 0110aaaaaaaa", "jc   %J",
            "bbb10010aaaaaaaa", "jb%B  %J",
            "aaa00100aaaaaaaa", "jmp  %A",
            "10110011", "jmpp @a",
            "00100011dddddddd", "mov  a,#$%X",
            "11111rrr", "mov  a,%R",
            "1111000r", "mov  a,@%R",
            "11000111", "mov  a,psw",
            "10111rrrdddddddd", "mov  %R,#$%X",
            "10101rrr", "mov  %R,a",
            "1010000r", "mov  @%R,a",
            "1011000rdddddddd", "mov  @%R,#$%X",
            "11010111", "mov  psw,a",
            "000011pp", "movd a,%P",
            "001111pp", "movd %P,a",
            "01000010", "mov  a,t",
            "01100010", "mov  t,a",
            "11100011", "movp3 a,@a",
            "10100011", "movp a,@a",
            "1000000r", "movx a,@%R",
            "1001000r", "movx @%R,a",
            "0100 1rrr", "orl  a,%R",
            "0100 000r", "orl  a,@%R",
            "0100 0011dddddddd", "orl  a,#$%X",
            "1000 1000dddddddd", "orl  bus,#$%X",
            "1000 1001dddddddd", "orl  p1,#$%X",
            "1000 1010dddddddd", "orl  p2,#$%X",
            "1000 11pp", "orld %P,a",
            "00000010", "outl bus,a",
            "001110pp", "outl %P,a",
            "10000011", "^ret",
            "10010011", "^retr",
            "11100111", "rl   a",
            "11110111", "rlc  a",
            "01110111", "rr   a",
            "01100111", "rrc  a",
            "11100101", "sel  mb0",
            "11110101", "sel  mb1",
            "11000101", "sel  rb0",
            "11010101", "sel  rb1",
            "01100101", "stop tcnt",
            "01000101", "strt cnt",
            "01010101", "strt t",
            "01000111", "swap a",
            "00101rrr", "xch  a,%R",
            "0010000r", "xch  a,@%R",
            "0011000r", "xchd a,@%R",
            "1101 0011dddddddd", "xrl  a,#$%X",
            "1101 1rrr", "xrl  a,%R",
            "1101 000r", "xrl  a,@%R",
            "00000000", "nop",
            null
        };
        private const int PTRS_PER_FORMAT = 2;
        private static readonly int MAX_OPS = (Formats.Length - 1) / PTRS_PER_FORMAT;
        private struct Opcode
        {
            public byte mask;
            public byte bits;
            public char extcode;
            public string parse;
            public string fmt;
            public uint flags;
        }
        private static Opcode[] Op = new Opcode[MAX_OPS + 1];
        private static bool OpInitialized = false;
        private static void InitDasm8039()
        {
            int i = 0;
            int index = 0;
            while (index < Formats.Length && Formats[index] != null)
            {
                string p = Formats[index];
                uint flags = 0;
                byte mask = 0, bits = 0;
                int bit = 7;
                for (int pos = 0; pos < p.Length && bit >= 0; pos++)
                {
                    char c = p[pos];
                    switch (c)
                    {
                        case '1':
                            mask |= (byte)(1 << bit);
                            bits |= (byte)(1 << bit);
                            bit--;
                            break;
                        case '0':
                            mask |= (byte)(1 << bit);
                            bit--;
                            break;
                        case ' ':
                            break;
                        case 'b':
                        case 'a':
                        case 'r':
                        case 'd':
                        case 'p':
                            bit--;
                            break;
                        default:
                            throw new InvalidOperationException("无效的指令编码 '"+Formats[index].ToString()+" "+Formats[index + 1].ToString()+"'");
                    }
                }
                if (bit != -1)
                {
                    throw new InvalidOperationException("编码 '"+Formats[index].ToString()+" "+Formats[index + 1].ToString()+"' 位数不足 "+bit.ToString());
                }
                char extcode = '\0';
                int fmtStart = index + 1;
                string fmt = Formats[fmtStart];
                if (fmt[0] == '!')
                {
                    flags |= DASMFLAG_STEP_OVER;
                    fmt = fmt.Substring(1);
                }
                else if (fmt[0] == '^')
                {
                    flags |= DASMFLAG_STEP_OUT;
                    fmt = fmt.Substring(1);
                }
                string parseStr = p;
                for (int pos = parseStr.Length - 1; pos >= 0; pos--)
                {
                    char ch = parseStr[pos];
                    if (ch != '0' && ch != '1' && ch != ' ')
                    {
                        extcode = ch;
                        break;
                    }
                }
                Op[i] = new Opcode
                {
                    mask = mask,
                    bits = bits,
                    extcode = extcode,
                    parse = parseStr,
                    fmt = fmt,
                    flags = flags
                };
                i++;
                index += PTRS_PER_FORMAT;
            }
            OpInitialized = true;
        }
        public static uint Dasm8039(StringBuilder buffer, uint pc, byte[] oprom)
        {
            if (!OpInitialized) InitDasm8039();

            byte code = oprom[0];
            int op = -1;
            for (int i = 0; i < MAX_OPS; i++)
            {
                if ((code & Op[i].mask) == Op[i].bits)
                {
                    if (op != -1)
                    {
                        System.Diagnostics.Debug.WriteLine("错误: 操作码 "+code.ToString("X2")+" 匹配 "+i.ToString()+" ("+ Op[i].fmt+") 和 "+op.ToString()+" ("+ Op[op].fmt+")");
                    }
                    op = i;
                }
            }

            if (op == -1)
            {
                buffer.Append("db   "+code.ToString("X2"));
                return 1 | DASMFLAG_SUPPORTED;
            }
            int cnt = 1;
            ushort fullcode = code;
            int bit = 7;
            if (Op[op].extcode != '\0')
            {
                cnt++;
                if (oprom.Length < 2)
                {
                    buffer.Append("<字节不足>");
                    return (uint)cnt;
                }
                fullcode <<= 8;
                fullcode |= oprom[1];
                bit = 15;
            }
            string cp = Op[op].parse;
            int b = 0, a = 0, d = 0, r = 0, p = 0;
            while (bit >= 0 && cp.Length > 0)
            {
                char ch = cp[0];
                cp = cp.Substring(1);
                switch (ch)
                {
                    case 'a':
                        a <<= 1;
                        a |= ((fullcode & (1 << bit)) != 0) ? 1 : 0;
                        bit--;
                        break;
                    case 'b':
                        b <<= 1;
                        b |= ((fullcode & (1 << bit)) != 0) ? 1 : 0;
                        bit--;
                        break;
                    case 'd':
                        d <<= 1;
                        d |= ((fullcode & (1 << bit)) != 0) ? 1 : 0;
                        bit--;
                        break;
                    case 'r':
                        r <<= 1;
                        r |= ((fullcode & (1 << bit)) != 0) ? 1 : 0;
                        bit--;
                        break;
                    case 'p':
                        p <<= 1;
                        p |= ((fullcode & (1 << bit)) != 0) ? 1 : 0;
                        bit--;
                        break;
                    case ' ':
                        break;
                    case '1':
                    case '0':
                        bit--;
                        break;
                    case '\0':
                        throw new InvalidOperationException("解析字符串过早结束，操作码 "+fullcode.ToString("X4")+", bit = "+ bit.ToString());
                }
            }
            string fmt = Op[op].fmt;
            //buffer.Clear();
            for (int i = 0; i < fmt.Length; i++)
            {
                if (fmt[i] == '%' && i + 1 < fmt.Length)
                {
                    i++;
                    char spec = fmt[i];
                    string numStr = "";
                    switch (spec)
                    {
                        case 'A':
                            numStr = string.Format("${0:X4}", a);
                            break;
                        case 'J':
                            numStr = string.Format("${0:X4}", ((pc + 1) & 0xF00) | a);
                            break;
                        case 'B':
                            numStr = b.ToString();
                            break;
                        case 'D':
                            numStr = d.ToString();
                            break;
                        case 'X':
                            numStr = string.Format("{0:X}", d);
                            break;
                        case 'R':
                            numStr = string.Format("r{0}", r);
                            break;
                        case 'P':
                            numStr = string.Format("p{0}", p);
                            break;
                        default:
                            throw new InvalidOperationException("格式字符串 '"+Op[op].fmt+"' 中的非法转义字符");
                    }
                    buffer.Append(numStr);
                }
                else
                {
                    buffer.Append(fmt[i]);
                }
            }
            return (uint)cnt | Op[op].flags | DASMFLAG_SUPPORTED;
        }
    }
}
