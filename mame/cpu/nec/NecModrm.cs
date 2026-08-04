using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cpu.nec
{
    partial class Nec
    {
        ushort RegWord(uint ModRM)
        {
            return (ushort)(I.regs.b[mod_RM.regw[ModRM] * 2] + I.regs.b[mod_RM.regw[ModRM] * 2 + 1] * 0x100);// I.regs.w[mod_RM.regw[ModRM]];
        }
        byte RegByte(uint ModRM)
        {
            return I.regs.b[mod_RM.regb[ModRM]];
        }
        ushort GetRMWord(uint ModRM)
        {
            return (ushort)(ModRM >= 0xc0 ? I.regs.b[mod_RM.RMw[ModRM] * 2] + I.regs.b[mod_RM.RMw[ModRM] * 2 + 1] * 0x100 : ReadWord(GetEA[ModRM]()));
        }
        void PutbackRMWord(uint ModRM, ushort val)
        {
            if (ModRM >= 0xc0)
            {
                //I.regs.w[mod_RM.RMw[ModRM]] = val;
                I.regs.b[mod_RM.RMw[ModRM] * 2] = (byte)(val % 0x100);
                I.regs.b[mod_RM.RMw[ModRM] * 2 + 1] = (byte)(val / 0x100);
            }
            else
            {
                WriteWord(EA, val);
            }
        }
        ushort GetnextRMWord()
        {
            return ReadWord((EA & 0xf0000) | ((EA + 2) & 0xffff));
        }
        void PutRMWord(uint ModRM, ushort val)
        {
            if (ModRM >= 0xc0)
            {
                //I.regs.w[mod_RM.RMw[ModRM]] = val;
                I.regs.b[mod_RM.RMw[ModRM] * 2] = (byte)(val % 0x100);
                I.regs.b[mod_RM.RMw[ModRM] * 2 + 1] = (byte)(val / 0x100);
            }
            else
            {
                WriteWord(GetEA[ModRM](), val);
            }
        }
        void PutImmRMWord(uint ModRM)
        {
            ushort val;
            if (ModRM >= 0xc0)
            {
                //I.regs.w[mod_RM.RMw[ModRM]] = FETCHWORD();
                ushort w = FETCHWORD();
                I.regs.b[mod_RM.RMw[ModRM] * 2] = (byte)(w % 0x100);
                I.regs.b[mod_RM.RMw[ModRM] * 2+1] = (byte)(w / 0x100);
            }
            else
            {
                EA = GetEA[ModRM]();
                val = FETCHWORD();
                WriteWord(EA, val);
            }
        }
        byte GetRMByte(uint ModRM)
        {
            return ((ModRM) >= 0xc0 ? I.regs.b[mod_RM.RMb[ModRM]] : ReadByte(GetEA[ModRM]()));
        }
        void PutRMByte(uint ModRM, byte val)
        {
            if (ModRM >= 0xc0)
            {
                I.regs.b[mod_RM.RMb[ModRM]] = val;
            }
            else
            {
                WriteByte(GetEA[ModRM](), val);
            }
        }
        void PutImmRMByte(uint ModRM)
        {
            if (ModRM >= 0xc0)
            {
                I.regs.b[mod_RM.RMb[ModRM]] = FETCH();
            }
            else
            {
                EA = GetEA[ModRM]();
                WriteByte(EA, FETCH());
            }
        }
        void PutbackRMByte(uint ModRM, byte val)
        {
            if (ModRM >= 0xc0)
            {
                I.regs.b[mod_RM.RMb[ModRM]] = val;
            }
            else
            {
                WriteByte(EA, val);
            }
        }
        void DEF_br8(out uint ModRM,out uint src, out uint dst)
        {
            ModRM = FETCH();
            src = RegByte(ModRM);
            dst = GetRMByte(ModRM);
        }
        void DEF_wr16(out uint ModRM, out uint src, out uint dst)
        {
            ModRM = FETCH();
            src = RegWord(ModRM);
            dst = GetRMWord(ModRM);
        }
        void DEF_r8b(out uint ModRM, out uint src, out uint dst)
        {
            ModRM = FETCH();
            dst = RegByte(ModRM);
            src = GetRMByte(ModRM);
        }
        void DEF_r16w(out uint ModRM,out uint src,out uint dst)
        {
	        ModRM = FETCH();
	        dst = RegWord(ModRM);
            src = GetRMWord(ModRM);
        }
        void DEF_ald8(out uint src, out uint dst)
        {
	         src = FETCH();
	         dst = I.regs.b[0];
        }
        void DEF_axd16(out uint src, out uint dst)
        {
            src = FETCH();
            dst = (ushort)(I.regs.b[0] + I.regs.b[1] * 0x100);// I.regs.w[0];
            src += (ushort)(FETCH() << 8);
        }
    }
}
