using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using mame;

namespace cpu.i8x41
{
    public partial class I8x41 : cpuexec_data
    {
        public enum I8X41Type
        {
            TYPE_I8X41,
            TYPE_I8X42
        }
        public I8X41Type type;
        public static I8x41 m1;
        public const int CLOCK_DIVIDER = 15;

        // Interrupts
        public const int I8X41_INT_IBF = 0;
        public const int I8X41_INT_TEST1 = 1;

        public const int I8X41_p1 = 0x01;
        public const int I8X41_p2 = 0x02;
        public const int I8X41_t0 = 0x80;
        public const int I8X41_t1 = 0x81;
        public const int I8X41_ps = 0x82;

        public const int I8X41_RAM_MASK = 0x7f;
        public const int I8X42_RAM_MASK = 0xff;

        public const ushort V_RESET = 0x000;
        public const ushort V_IBF = 0x003;
        public const ushort V_TIMER = 0x007;

        // Internal Memory Map Offsets
        public const ushort M_IRAM = 0x800;
        public const ushort M_BANK0 = 0x800;
        public const ushort M_STACK = 0x808;
        public const ushort M_BANK1 = 0x818;
        public const ushort M_USER = 0x820;

        public const byte FC=0x80;
        public const byte FA=0x40;
        public const byte Ff0=0x20;
        public const byte BS=0x10;
        public const byte FU=0x08;
        public const byte SP=0x07;

        public const byte OBF=0x01;
        public const byte IBF=0x02;
        public const byte F0=0x04;
        public const byte F1=0x08;

        public const byte IBFI=	0x01;
        public const byte TCNTI=0x02;
        public const byte DMA=0x04;
        public const byte FLAGS=0x08;
        public const byte T=0x10;
        public const byte CNT = 0x20;

        public const byte IBFI_IGNR = 0x01;
        public const byte IBFI_PEND=0x02;
        public const byte TIRQ_IGNR=0x04;
        public const byte TIRQ_PEND=0x08;
        public const byte TEST1=0x10;
        public const byte TOVF = 0x20;

        public const byte IRQ_IGNR=0x05;
        public const byte IRQ_PEND=0x0a;

        public ushort PPC;
        public ushort PC;
        public byte timer;
        public byte PRESCALER;
        public ushort subtype;
        public byte A;
        public byte PSW;
        public byte STATE;
        public byte ENABLE;
        public byte CONTROL;
        public byte DBBI;
        public byte DBBO;
        public byte P1;
        public byte P2;
        public byte P2_HS;
        public byte ram_mask;
        protected ulong totalExecutedCycles;
        protected int pendingCycles;
        public override ulong TotalExecutedCycles
        {
            get
            {
                return totalExecutedCycles;
            }
            set
            {
                totalExecutedCycles = value;
            }
        }
        public override int PendingCycles
        {
            get
            {
                return pendingCycles;
            }
            set
            {
                pendingCycles = value;
            }
        }

        public Func<ushort, byte> RM;//ReadMemory;
        public Action<ushort, byte> WM;//WriteMemory;
        public Func<int, byte> RP;//ReadIO;
        public Action<int, byte> WP;//WriteIO;
        public Func<ushort, byte> ROP;
        public Func<ushort, byte> ROP_ARG;
        public Func<int, int> irq_callback;

        private byte GETR(int n)
        {
            return RM((ushort)(((PSW & BS) != 0 ? M_BANK1 : M_BANK0) + n));
        }
        private void SETR(int n, byte v)
        {
            WM((ushort)(((PSW & BS)!=0 ? M_BANK1 : M_BANK0) + n), v);
        }
        private void PUSH_PC_TO_STACK()
        {
            WM((ushort)(M_STACK + (PSW & SP) * 2 + 0), (byte)(PC&0xff));
            WM((ushort)(M_STACK + (PSW & SP) * 2 + 1), (byte)(((PC >> 8) & 0x0f) | (PSW & 0xf0)));
            PSW = (byte)((PSW & ~SP) | ((PSW + 1) & SP));
        }
        private void illegal()
        {

        }
        private void add_r(int r)
        {
            byte res = (byte)(A + GETR(r));
            if (res < A)
            {
                PSW |= FC;
            }
            if ((res & 0x0f) < (A & 0x0f))
            {
                PSW |= FA;
            }
            A = res;
        }
        private void add_rm(int r)
        {
            byte res = (byte)(A + RM((ushort)(M_IRAM + (GETR(r) & ram_mask))));
            if (res < A)
            {
                PSW |= FC;
            }
            if ((res & 0x0f) < (A & 0x0f))
            {
                PSW |= FA;
            }
            A = res;
        }
        private void add_i()
        {
            byte res = (byte)(A + ROP_ARG(PC));
            PC++;
            if (res < A)
            {
                PSW |= FC;
            }
            if ((res & 0x0f) < (A & 0x0f))
            {
                PSW |= FA;
            }
            A = res;
        }
        private void addc_r(int r)
        {
            byte res = (byte)(A + GETR(r) + (PSW >> 7));
            if (res <= A)
            {
                PSW |= FC;
            }
            if ((res & 0x0f) <= (A & 0x0f))
            {
                PSW |= FA;
            }
            A = res;
        }
        private void addc_rm(int r)
        {
            byte res = (byte)(A + RM((ushort)(M_IRAM + (GETR(r) & ram_mask))) + (PSW >> 7));
            if (res <= A)
            {
                PSW |= FC;
            }
            if ((res & 0x0f) <= (A & 0x0f))
            {
                PSW |= FA;
            }
            A = res;
        }
        private void addc_i()
        {
            byte res = (byte)(A + ROP_ARG(PC) + (PSW >> 7));
            PC++;
            if (res <= A)
            {
                PSW |= FC;
            }
            if ((res & 0x0f) < (A & 0x0f))
            {
                PSW |= FA;
            }
            A = res;
        }
        private void anl_r(int r)
        {
            A = (byte)(A & GETR(r));
        }
        private void anl_rm(int r)
        {
            A = (byte)(A & RM((ushort)(M_IRAM + (GETR(r) & ram_mask))));
        }
        private void anl_i()
        {
            A = (byte)(A & ROP_ARG(PC));
            PC++;
        }
        private void anl_p_i(int p)
        {
            byte val = ROP_ARG(PC);
            PC++;
            switch (p)
            {
                case 00:
                    break;
                case 01:
                    P1 &= val;
                    WP(p, P1);
                    break;
                case 02:
                    P2 &= val;
                    WP(p, (byte)(P2 & P2_HS));
                    break;
                case 03:
                    break;
                default:
                    break;
            }
        }
        private void anld_p_a(int p)
        {
            WP(2, (byte)((P2 & 0xf0) | 0x0c | p));
            WP(I8X41_ps, 0);
            WP(2, (byte)(A & 0x0f));
            WP(I8X41_ps, 1);
        }
        private void call_i(int page)
        {
            byte adr = ROP_ARG(PC);
            PC++;
            PUSH_PC_TO_STACK();
            PC = (ushort)(page | adr);
        }

        private void clr_a()
        {
            A = 0;
        }
        private void clr_c()
        {
            PSW &= unchecked((byte)~FC);
        }
        private void clr_f0()
        {
            PSW &= unchecked((byte)~Ff0);
            STATE &= unchecked((byte)~F0);
        }
        private void clr_f1()
        {
            STATE &= unchecked((byte)~F1);
        }
        private void cpl_a()
        {
            A = (byte)~A;
        }
        private void cpl_c()
        {
            PSW ^= FC;
        }
        private void cpl_f0()
        {
            PSW ^= Ff0;
            STATE ^= F0;
        }
        private void cpl_f1()
        {
            STATE ^= F1;
        }
        private void da_a()
        {
            byte res = (byte)(A + (((PSW & FA) != 0 || ((A & 0x0f) > 0x09)) ? 0x06 : 0x00));
            if ((PSW & FC) != 0 || ((res & 0xf0) > 0x90))
            {
                res += 0x60;
            }
            if (res < A)
            {
                PSW |= FC;
            }
            else
            {
                PSW &= unchecked((byte)~FC);
            }
            A = res;
        }
        private void dec_a()
        {
            A -= 1;
        }
        private void dec_r(int r)
        {
            SETR(r, (byte)(GETR(r) - 1));
        }
        private void dis_i()
        {
            ENABLE &= unchecked((byte)~IBFI);
        }
        private void dis_tcnti()
        {
            ENABLE &= unchecked((byte)~TCNTI);
        }
        private void djnz_r_i(int r)
        {
            byte adr = ROP_ARG(PC);
            PC++;
            SETR(r, (byte)(GETR(r) - 1));
            if (GETR(r) != 0)
            {
                PC = (ushort)((PC & 0x700) | adr);
            }
        }
        private void en_dma()
        {
            ENABLE |= DMA;
            P2_HS &= 0xbf;
            WP(0x02, (byte)(P2 & P2_HS));
        }
        private void en_flags()
        {
            if (0 == (ENABLE & FLAGS))
            {
                ENABLE |= FLAGS;
                if ((STATE & OBF) != 0)
                {
                    P2_HS |= 0x10;
                }
                else
                {
                    P2_HS &= 0xef;
                }
                if ((STATE & IBF) != 0)
                {
                    P2_HS |= 0x20;
                }
                else
                {
                    P2_HS &= 0xdf;
                }
                WP(0x02, (byte)(P2 & P2_HS));
            }
        }
        private void en_i()
        {
            if (0 == (ENABLE & IBFI))
            {
                ENABLE |= IBFI;
                CONTROL &= unchecked((byte)~IBFI_IGNR);
                if ((STATE & IBF) != 0)
                {
                    set_irq_line(I8X41_INT_IBF,LineState.HOLD_LINE);
                }
            }
        }
        private void en_tcnti()
        {
            ENABLE |= TCNTI;
            CONTROL &= unchecked((byte)~TIRQ_IGNR);
        }
        private void in_a_dbb()
        {
            if (irq_callback != null)
            {
                irq_callback(I8X41_INT_IBF);
            }
            STATE &= unchecked((byte)~IBF);
            if ((ENABLE & FLAGS) != 0)
            {
                P2_HS &= 0xdf;
                if ((STATE & OBF) != 0)
                {
                    P2_HS |= 0x10;
                }
                else
                {
                    P2_HS &= 0xef;
                }
                WP(0x02, (byte)(P2 & P2_HS));
            }
            A = DBBI;
        }
        private void in_a_p(int p)
        {
            switch (p)
            {
                case 00:
                    break;
                case 01:
                    A = (byte)(RP(p) & P1);
                    break;
                case 02:
                    A = (byte)(RP(p) & P2);
                    break;
                case 03:
                    break;
                default:
                    break;
            }
        }
        private void inc_a()
        {
            A += 1;
        }
        private void inc_r(int r)
        {
            SETR(r, (byte)(GETR(r) + 1));
        }
        private void inc_rm(int r)
        {
            ushort addr = (ushort)(M_IRAM + (GETR(r) & ram_mask));
            WM(addr, (byte)(RM(addr) + 1));
        }
        private void jbb_i(int bit)
        {
            byte adr = ROP_ARG(PC);
            PC += 1;
            if ((A & (1 << bit)) != 0)
            {
                PC = (ushort)((PC & 0x700) | adr);
            }
        }
        private void jc_i()
        {
            byte adr = ROP_ARG(PC);
            PC += 1;
            if ((PSW & FC) != 0)
            {
                PC = (ushort)((PC & 0x700) | adr);
            }
        }
        private void jf0_i()
        {
            byte adr = ROP_ARG(PC);
            PC += 1;
            if ((STATE & F0) != 0)
            {
                PC = (ushort)((PC & 0x700) | adr);
            }
        }
        private void jf1_i()
        {
            byte adr = ROP_ARG(PC);
            PC += 1;
            if ((STATE & F1) != 0)
            {
                PC = (ushort)((PC & 0x700) | adr);
            }
        }
        private void jmp_i(int page)
        {
            byte adr = ROP_ARG(PC);
            PC = (ushort)(page | adr);
        }
        private void jmpp_a()
        {
            ushort adr = (ushort)((PC & 0x700) | A);
            PC = (ushort)((PC & 0x700) | RM(adr));
        }
        private void jnc_i()
        {
            byte adr = ROP_ARG(PC);
            PC += 1;
            if ((PSW & FC) == 0)
            {
                PC = (ushort)((PC & 0x700) | adr);
            }
        }
        private void jnibf_i()
        {
            byte adr = ROP_ARG(PC);
            PC += 1;
            if (0 == (STATE & IBF))
            {
                PC = (ushort)((PC & 0x700) | adr);
            }
        }
        private void jnt0_i()
        {
            byte adr = ROP_ARG(PC);
            PC += 1;
            if (0 == RP(I8X41_t0))
            {
                PC = (ushort)((PC & 0x700) | adr);
            }
        }
        private void jnt1_i()
        {
            byte adr = ROP_ARG(PC);
            PC += 1;
            if ((ENABLE & CNT) == 0)
            {
                byte level = RP(I8X41_t1);
                if (level != 0)
                {
                    CONTROL |= TEST1;
                }
                else
                {
                    CONTROL &= unchecked((byte)~TEST1);
                }
            }
            if ((CONTROL & TEST1) == 0)
            {
                PC = (ushort)((PC & 0x700) | adr);
            }
        }
        private void jnz_i()
        {
            byte adr = ROP_ARG(PC);
            PC += 1;
            if (A != 0)
            {
                PC = (ushort)((PC & 0x700) | adr);
            }
        }
        private void jobf_i()
        {
            byte adr = ROP_ARG(PC);
            PC += 1;
            if ((STATE & OBF) != 0)
            {
                PC = (ushort)((PC & 0x700) | adr);
            }
        }
        private void jtf_i()
        {
            byte adr = ROP_ARG(PC);
            PC += 1;
            if ((CONTROL & TOVF) != 0)
            {
                PC = (ushort)((PC & 0x700) | adr);
            }
            CONTROL &= unchecked((byte)~TOVF);
        }
        private void jt0_i()
        {
            byte adr = ROP_ARG(PC);
            PC += 1;
            if (RP(I8X41_t0) != 0)
            {
                PC = (ushort)((PC & 0x700) | adr);
            }
        }
        private void jt1_i()
        {
            byte adr = ROP_ARG(PC);
            PC += 1;
            if ((ENABLE & CNT) == 0)
            {
                byte level = RP(I8X41_t1);
                if (level != 0)
                {
                    CONTROL |= TEST1;
                }
                else
                {
                    CONTROL &= unchecked((byte)~TEST1);
                }
            }
            if ((CONTROL & TEST1) != 0)
            {
                PC = (ushort)((PC & 0x700) | adr);
            }
        }
        private void jz_i()
        {
            byte adr = ROP_ARG(PC);
            PC += 1;
            if (A == 0)
            {
                PC = (ushort)((PC & 0x700) | adr);
            }
        }
        private void mov_a_i()
        {
            A = ROP(PC);
            PC += 1;
        }
        private void mov_a_psw()
        {
            A = PSW;
        }
        private void mov_a_r(int r)
        {
            A = GETR(r);
        }
        private void mov_a_rm(int r)
        {
            A = RM((ushort)(M_IRAM + (GETR(r) & ram_mask)));
        }
        private void mov_a_t()
        {
            A = timer;
        }
        private void mov_psw_a()
        {
            PSW = A;
        }
        private void mov_r_a(int r)
        {
            SETR(r, A);
        }
        private void mov_r_i(int r)
        {
            byte val = ROP_ARG(PC);
            PC += 1;
            SETR(r, val);
        }
        private void mov_rm_a(int r)
        {
            WM((ushort)(M_IRAM + (GETR(r) & ram_mask)), A);
        }
        private void mov_rm_i(int r)
        {
            byte val = ROP_ARG(PC);
            PC += 1;
            WM((ushort)(M_IRAM + (GETR(r) & ram_mask)), val);
        }
        private void mov_sts_a()
        {
            STATE = (byte)((STATE & 0x0f) | (A & 0xf0));
        }
        private void mov_t_a()
        {
            timer = A;
        }
        private void movd_a_p(int p)
        {
            WP(2, (byte)((P2 & 0xf0) | 0x00 | p));
            WP(I8X41_ps, 0);
            A = (byte)(RP(2) & 0xf);
            WP(I8X41_ps, 1);
        }
        private void movd_p_a(int p)
        {
            WP(2, (byte)((P2 & 0xf0) | 0x04 | p));
            WP(I8X41_ps, 0);
            WP(2, (byte)(A & 0x0f));
            WP(I8X41_ps, 1);
        }
        private void movp_a_am()
        {
            ushort addr = (ushort)((PC & 0x700) | A);
            A = RM(addr);
        }
        private void movp3_a_am()
        {
            ushort addr = (ushort)(0x300 | A);
            A = RM(addr);
        }
        private void nop()
        {

        }
        private void orl_r(int r)
        {
            A = (byte)(A | GETR(r));
        }
        private void orl_rm(int r)
        {
            A = (byte)(A | RM((ushort)(M_IRAM + (GETR(r) & ram_mask))));
        }
        private void orl_i()
        {
            byte val = ROP_ARG(PC);
            PC++;
            A = (byte)(A | val);
        }
        private void orl_p_i(int p)
        {
            byte val = ROP_ARG(PC);
            PC++;
            switch (p)
            {
                case 00:
                    break;
                case 01:
                    P1 |= val;
                    WP(p, P1);
                    break;
                case 02:
                    P2 |= val;
                    WP(p, P2);
                    break;
                case 03:
                    break;
                default:
                    break;
            }
        }
        private void orld_p_a(int p)
        {
            WP(2, (byte)((P2 & 0xf0) | 0x08 | p));
            WP(I8X41_ps, 0);
            WP(2, (byte)(A & 0x0f));
            WP(I8X41_ps, 1);
        }
        private void out_dbb_a()
        {
            DBBO = A;
            STATE |= OBF;
            if ((ENABLE & FLAGS) != 0)
            {
                P2_HS |= 0x10;
                if ((STATE & IBF) != 0)
                {
                    P2_HS |= 0x20;
                }
                else
                {
                    P2_HS &= 0xdf;
                }
                WP(0x02, (byte)(P2 & P2_HS));
            }
        }
        private void out_p_a(int p)
        {
            switch (p)
            {
                case 00:
                    break;
                case 01:
                    WP(p, A);
                    P1 = A;
                    break;
                case 02:
                    WP(p, A);
                    P2 = A;
                    break;
                case 03:
                    break;
                default:
                    break;
            }
        }
        private void ret()
        {
            byte msb;
            PSW = (byte)((PSW & ~SP) | ((PSW - 1) & SP));
            msb = RM((ushort)(M_STACK + (PSW & SP) * 2 + 1));
            PC = RM((ushort)(M_STACK + (PSW & SP) * 2 + 0));
            PC = (ushort)(PC | ((msb << 8) & 0x700));
        }
        private void retr()
        {
            byte msb;
            PSW = (byte)((PSW & ~SP) | ((PSW - 1) & SP));
            msb = RM((ushort)(M_STACK + (PSW & SP) * 2 + 1));
            PC = RM((ushort)(M_STACK + (PSW & SP) * 2 + 0));
            PC = (ushort)(PC | ((msb << 8) & 0x700));
            PSW = (byte)((PSW & 0x0f) | (msb & 0xf0));
            CONTROL &= unchecked((byte)~IRQ_IGNR);
        }
        private void rl_a()
        {
            A = (byte)((A << 1) | (A >> 7));
        }
        private void rlc_a()
        {
            byte c = (byte)(PSW >> 7);
            PSW = (byte)((PSW & ~FC) | (A & FC));
            A = (byte)((A << 1) | c);
        }
        private void rr_a()
        {
            A = (byte)((A >> 1) | (A << 7));
        }
        private void rrc_a()
        {
            byte c = (byte)(PSW & 0x80);
            PSW = (byte)((PSW & ~FC) | (A << 7));
            A = (byte)((A >> 1) | c);
        }
        private void sel_rb0()
        {
            PSW &= unchecked((byte)~BS);
        }
        private void sel_rb1()
        {
            PSW |= BS;
        }
        private void stop_tcnt()
        {
            ENABLE &= unchecked((byte)~(T | CNT));
        }
        private void strt_cnt()
        {
            ENABLE |= CNT;
            ENABLE &= unchecked((byte)~T);
        }
        private void strt_t()
        {
            ENABLE |= T;
            ENABLE &= unchecked((byte)~CNT);
        }
        private void swap_a()
        {
            A = (byte)((A << 4) | (A >> 4));
        }
        private void xch_a_r(int r)
        {
            byte tmp = GETR(r);
            SETR(r, A);
            A = tmp;
        }
        private void xch_a_rm(int r)
        {
            ushort addr = (ushort)(M_IRAM + (GETR(r) & ram_mask));
            byte tmp = RM(addr);
            WM(addr, A);
            A = tmp;
        }
        private void xchd_a_rm(int r)
        {
            ushort addr = (ushort)(M_IRAM + (GETR(r) & ram_mask));
            byte tmp = RM(addr);
            WM(addr, (byte)((tmp & 0xf0) | (A & 0x0f)));
            A = (byte)((A & 0xf0) | (tmp & 0x0f));
        }
        private void xrl_r(int r)
        {
            A = (byte)(A ^ GETR(r));
        }
        private void xrl_rm(int r)
        {
            A = (byte)(A ^ RM((ushort)(M_IRAM + (GETR(r) & ram_mask))));
        }
        private void xrl_i()
        {
            byte val = ROP_ARG(PC);
            PC++;
            A = (byte)(A ^ val);
        }
        private static readonly byte[] i8x41_cycles = new byte[]
        {
            1,1,1,2,2,1,1,1,2,2,2,2,2,2,2,2,
            1,1,2,2,2,1,2,1,1,1,1,1,1,1,1,1,
            1,1,1,2,2,1,2,1,1,1,1,1,1,1,1,1,
            1,1,2,1,2,1,2,1,2,2,2,2,2,2,2,2,
            1,1,1,2,2,1,2,1,1,1,1,1,1,1,1,1,
            1,1,2,2,2,1,2,1,1,1,1,1,1,1,1,1,
            1,1,1,1,2,1,1,1,1,1,1,1,1,1,1,1,
            1,1,2,1,2,1,2,1,1,1,1,1,1,1,1,1,
            1,1,1,2,2,1,2,1,2,2,2,2,2,2,2,2,
            1,1,2,2,1,1,2,1,2,2,2,2,2,2,2,2,
            1,1,1,2,2,1,1,1,1,1,1,1,1,1,1,1,
            2,2,2,2,2,1,2,1,2,2,2,2,2,2,2,2,
            1,1,1,1,2,1,2,1,1,1,1,1,1,1,1,1,
            1,1,2,1,2,1,2,1,1,1,1,1,1,1,1,1,
            1,1,1,2,2,1,2,1,2,2,2,2,2,2,2,2,
            1,1,2,1,2,1,2,1,2,2,2,2,2,2,2,2
        };
        public static void i8x41_init(I8X41Type _type)
        {
            m1 = new I8x41();
            m1.type = _type;
        }
        public override void cpunum_set_input_line_and_vector(int cpunum, int line, LineState state, int vector)
        {
            Timer.timer_set_internal(Cpuint.cpunum_empty_event_queue, "cpunum_empty_event_queue");
        }
        public override void Reset()
        {
            var saveIrqCallback = irq_callback;
            PPC = 0; PC = 0; timer = 0; PRESCALER = 0; A = 0; PSW = 0; STATE = 0;
            ENABLE = 0; CONTROL = 0; DBBI = 0xFF; DBBO = 0xFF; P1 = 0xFF; P2 = 0xFF; P2_HS = 0xFF;
            irq_callback = saveIrqCallback;
            subtype = 8041;
            ram_mask = I8X41_RAM_MASK;
            if (type == I8X41Type.TYPE_I8X42)
            {
                subtype = 8042;
                ram_mask = I8X42_RAM_MASK;
            }
            ENABLE = IBFI | TCNTI;
        }
        public override int ExecuteCycles(int cycles)
        {
            int inst_cycles, T1_level;
            byte op;
            pendingCycles = cycles;
            do
            {
                PPC = PC;
                op = ROP(PC);
                PC++;
                pendingCycles -= i8x41_cycles[op];
                switch (op)
                {
                    case 0x00:
                        nop();
                        break;
                    case 0x01:
                        illegal();
                        break;
                    case 0x02:
                        out_dbb_a();
                        break;
                    case 0x03:
                        add_i();
                        break;
                    case 0x04:
                        jmp_i(0x000);
                        break;
                    case 0x05:
                        en_i();
                        break;
                    case 0x06:
                        illegal();
                        break;
                    case 0x07:
                        dec_a();
                        break;
                    case 0x08:
                    case 0x09:
                    case 0x0a:
                    case 0x0b:
                        in_a_p(op & 3);
                        break;
                    case 0x0c:
                    case 0x0d:
                    case 0x0e:
                    case 0x0f:
                        movd_a_p(op & 3);
                        break;
                    case 0x10:
                        inc_rm(0);
                        break;
                    case 0x11:
                        inc_rm(1);
                        break;
                    case 0x12:
                        jbb_i(0);
                        break;
                    case 0x13:
                        addc_i();
                        break;
                    case 0x14:
                        call_i(0x000);
                        break;
                    case 0x15:
                        dis_i();
                        break;
                    case 0x16:
                        jtf_i();
                        break;
                    case 0x17:
                        inc_a();
                        break;
                    case 0x18:
                    case 0x19:
                    case 0x1a:
                    case 0x1b:
                    case 0x1c:
                    case 0x1d:
                    case 0x1e:
                    case 0x1f:
                        inc_r(op & 7);
                        break;
                    case 0x20:
                        xch_a_rm(0);
                        break;
                    case 0x21:
                        xch_a_rm(1);
                        break;
                    case 0x22:
                        in_a_dbb();
                        break;
                    case 0x23:
                        mov_a_i();
                        break;
                    case 0x24:
                        jmp_i(0x100);
                        break;
                    case 0x25:
                        en_tcnti();
                        break;
                    case 0x26:
                        jnt0_i();
                        break;
                    case 0x27:
                        clr_a();
                        break;
                    case 0x28:
                    case 0x29:
                    case 0x2a:
                    case 0x2b:
                    case 0x2c:
                    case 0x2d:
                    case 0x2e:
                    case 0x2f:
                        xch_a_r(op & 7);
                        break;
                    case 0x30:
                        xchd_a_rm(0);
                        break;
                    case 0x31:
                        xchd_a_rm(1);
                        break;
                    case 0x32:
                        jbb_i(1);
                        break;
                    case 0x33:
                        illegal();
                        break;
                    case 0x34:
                        call_i(0x100);
                        break;
                    case 0x35:
                        dis_tcnti();
                        break;
                    case 0x36:
                        jt0_i();
                        break;
                    case 0x37:
                        cpl_a();
                        break;
                    case 0x38:
                    case 0x39:
                    case 0x3a:
                    case 0x3b:
                        out_p_a(op & 3);
                        break;
                    case 0x3c:
                    case 0x3d:
                    case 0x3e:
                    case 0x3f:
                        movd_p_a(op & 3);
                        break;
                    case 0x40:
                        orl_rm(0);
                        break;
                    case 0x41:
                        orl_rm(1);
                        break;
                    case 0x42:
                        mov_a_t();
                        break;
                    case 0x43:
                        orl_i();
                        break;
                    case 0x44:
                        jmp_i(0x200);
                        break;
                    case 0x45:
                        strt_cnt();
                        break;
                    case 0x46:
                        jnt1_i();
                        break;
                    case 0x47:
                        swap_a();
                        break;
                    case 0x48:
                    case 0x49:
                    case 0x4a:
                    case 0x4b:
                    case 0x4c:
                    case 0x4d:
                    case 0x4e:
                    case 0x4f:
                        orl_r(op & 7);
                        break;
                    case 0x50:
                        anl_rm(0);
                        break;
                    case 0x51:
                        anl_rm(1);
                        break;
                    case 0x52:
                        jbb_i(2);
                        break;
                    case 0x53:
                        anl_i();
                        break;
                    case 0x54:
                        call_i(0x200);
                        break;
                    case 0x55:
                        strt_t();
                        break;
                    case 0x56:
                        jt1_i();
                        break;
                    case 0x57:
                        da_a();
                        break;
                    case 0x58:
                    case 0x59:
                    case 0x5a:
                    case 0x5b:
                    case 0x5c:
                    case 0x5d:
                    case 0x5e:
                    case 0x5f:
                        anl_r(op & 7);
                        break;
                    case 0x60:
                        add_rm(0);
                        break;
                    case 0x61:
                        add_rm(1);
                        break;
                    case 0x62:
                        mov_t_a();
                        break;
                    case 0x63:
                        illegal();
                        break;
                    case 0x64:
                        jmp_i(0x300);
                        break;
                    case 0x65:
                        stop_tcnt();
                        break;
                    case 0x66:
                        illegal();
                        break;
                    case 0x67:
                        rrc_a();
                        break;
                    case 0x68:
                    case 0x69:
                    case 0x6a:
                    case 0x6b:
                    case 0x6c:
                    case 0x6d:
                    case 0x6e:
                    case 0x6f:
                        add_r(op & 7);
                        break;
                    case 0x70:
                        addc_rm(0);
                        break;
                    case 0x71:
                        addc_rm(1);
                        break;
                    case 0x72:
                        jbb_i(3);
                        break;
                    case 0x73:
                        illegal();
                        break;
                    case 0x74:
                        call_i(0x300);
                        break;
                    case 0x75:
                        illegal();
                        break;
                    case 0x76:
                        jf1_i();
                        break;
                    case 0x77:
                        rr_a();
                        break;
                    case 0x78:
                    case 0x79:
                    case 0x7a:
                    case 0x7b:
                    case 0x7c:
                    case 0x7d:
                    case 0x7e:
                    case 0x7f:
                        addc_r(op & 7);
                        break;
                    case 0x80:
                        illegal();
                        break;
                    case 0x81:
                        illegal();
                        break;
                    case 0x82:
                        illegal();
                        break;
                    case 0x83:
                        ret();
                        break;
                    case 0x84:
                        jmp_i(0x400);
                        break;
                    case 0x85:
                        clr_f0();
                        break;
                    case 0x86:
                        jobf_i();
                        break;
                    case 0x87:
                        illegal();
                        break;
                    case 0x88:
                    case 0x89:
                    case 0x8a:
                    case 0x8b:
                        orl_p_i(op & 3);
                        break;
                    case 0x8c:
                    case 0x8d:
                    case 0x8e:
                    case 0x8f:
                        orld_p_a(op & 7);
                        break;
                    case 0x90:
                        mov_sts_a();
                        break;
                    case 0x91:
                        illegal();
                        break;
                    case 0x92:
                        jbb_i(4);
                        break;
                    case 0x93:
                        retr();
                        break;
                    case 0x94:
                        call_i(0x400);
                        break;
                    case 0x95:
                        cpl_f0();
                        break;
                    case 0x96:
                        jnz_i();
                        break;
                    case 0x97:
                        clr_c();
                        break;
                    case 0x98:
                    case 0x99:
                    case 0x9a:
                    case 0x9b:
                        anl_p_i(op & 3);
                        break;
                    case 0x9c:
                    case 0x9d:
                    case 0x9e:
                    case 0x9f:
                        anld_p_a(op & 7);
                        break;
                    case 0xa0:
                        mov_rm_a(0);
                        break;
                    case 0xa1:
                        mov_rm_a(1);
                        break;
                    case 0xa2:
                        illegal();
                        break;
                    case 0xa3:
                        movp_a_am();
                        break;
                    case 0xa4:
                        jmp_i(0x500);
                        break;
                    case 0xa5:
                        clr_f1();
                        break;
                    case 0xa6:
                        illegal();
                        break;
                    case 0xa7:
                        cpl_c();
                        break;
                    case 0xa8:
                    case 0xa9:
                    case 0xaa:
                    case 0xab:
                    case 0xac:
                    case 0xad:
                    case 0xae:
                    case 0xaf:
                        mov_r_a(op & 7);
                        break;
                    case 0xb0:
                        mov_rm_i(0);
                        break;
                    case 0xb1:
                        mov_rm_i(1);
                        break;
                    case 0xb2:
                        jbb_i(5);
                        break;
                    case 0xb3:
                        jmpp_a();
                        break;
                    case 0xb4:
                        call_i(0x500);
                        break;
                    case 0xb5:
                        cpl_f1();
                        break;
                    case 0xb6:
                        jf0_i();
                        break;
                    case 0xb7:
                        illegal();
                        break;
                    case 0xb8:
                    case 0xb9:
                    case 0xba:
                    case 0xbb:
                    case 0xbc:
                    case 0xbd:
                    case 0xbe:
                    case 0xbf:
                        mov_r_i(op & 7);
                        break;
                    case 0xc0:
                        illegal();
                        break;
                    case 0xc1:
                        illegal();
                        break;
                    case 0xc2:
                        illegal();
                        break;
                    case 0xc3:
                        illegal();
                        break;
                    case 0xc4:
                        jmp_i(0x600);
                        break;
                    case 0xc5:
                        sel_rb0();
                        break;
                    case 0xc6:
                        jz_i();
                        break;
                    case 0xc7:
                        mov_a_psw();
                        break;
                    case 0xc8:
                    case 0xc9:
                    case 0xca:
                    case 0xcb:
                    case 0xcc:
                    case 0xcd:
                    case 0xcf:
                        dec_r(op & 7);
                        break;
                    case 0xd0:
                        xrl_rm(0);
                        break;
                    case 0xd1:
                        xrl_rm(1);
                        break;
                    case 0xd2:
                        jbb_i(6);
                        break;
                    case 0xd3:
                        xrl_i();
                        break;
                    case 0xd4:
                        call_i(0x600);
                        break;
                    case 0xd5:
                        sel_rb1();
                        break;
                    case 0xd6:
                        jnibf_i();
                        break;
                    case 0xd7:
                        mov_psw_a();
                        break;
                    case 0xd8:
                    case 0xd9:
                    case 0xda:
                    case 0xdb:
                    case 0xdc:
                    case 0xdd:
                    case 0xde:
                    case 0xdf:
                        xrl_r(op & 7);
                        break;
                    case 0xe0:
                        illegal();
                        break;
                    case 0xe1:
                        illegal();
                        break;
                    case 0xe2:
                        illegal();
                        break;
                    case 0xe3:
                        movp3_a_am();
                        break;
                    case 0xe4:
                        jmp_i(0x700);
                        break;
                    case 0xe5:
                        en_dma();
                        break;
                    case 0xe6:
                        jnc_i();
                        break;
                    case 0xe7:
                        rl_a();
                        break;
                    case 0xe8:
                    case 0xe9:
                    case 0xea:
                    case 0xeb:
                    case 0xec:
                    case 0xed:
                    case 0xee:
                    case 0xef:
                        djnz_r_i(op & 7);
                        break;
                    case 0xf0:
                        mov_a_rm(0);
                        break;
                    case 0xf1:
                        mov_a_rm(1);
                        break;
                    case 0xf2:
                        jbb_i(7);
                        break;
                    case 0xf3:
                        illegal();
                        break;
                    case 0xf4:
                        call_i(0x700);
                        break;
                    case 0xf5:
                        en_flags();
                        break;
                    case 0xf6:
                        jc_i();
                        break;
                    case 0xf7:
                        rlc_a();
                        break;
                    case 0xf8:
                    case 0xf9:
                    case 0xfa:
                    case 0xfb:
                    case 0xfc:
                    case 0xfd:
                    case 0xfe:
                    case 0xff:
                        mov_a_r(op & 7);
                        break;
                }
                if ((ENABLE & CNT) != 0)
                {
                    inst_cycles = i8x41_cycles[op];
                    for (; inst_cycles > 0; inst_cycles--)
                    {
                        T1_level = RP(I8X41_t1);
                        if (((CONTROL & TEST1)!=0) && (T1_level == 0))
                        {
                            timer++;
                            if (timer == 0)
                            {
                                CONTROL |= TOVF;
                                if ((ENABLE & TCNTI)!=0)
                                {
                                    CONTROL |= TIRQ_PEND;
                                }
                            }
                        }
                        if (T1_level!=0)
                        {
                            CONTROL |= TEST1;
                        }
                        else
                        {
                            CONTROL &= unchecked((byte)~TEST1);
                        }
                    }
                }
                if ((ENABLE & T) != 0)
                {
                    PRESCALER += i8x41_cycles[op];
                    if (PRESCALER >= 32)
                    {
                        PRESCALER -= 32;
                        timer++;
                        if (timer == 0)
                        {
                            CONTROL |= TOVF;
                            if ((ENABLE & TCNTI) != 0)
                            {
                                CONTROL |= TIRQ_PEND;
                            }
                        }
                    }
                }
                if ((CONTROL & IRQ_PEND)!=0)
                {
                    if (0 == (CONTROL & IBFI_IGNR))
                    {
                        if (((ENABLE & IBFI)!=0) && ((CONTROL & IBFI_PEND)!=0))
                        {
                            PUSH_PC_TO_STACK();
                            PC = V_IBF;
                            CONTROL &= unchecked((byte)~IBFI_PEND);
                            CONTROL |= IBFI_IGNR;
                            pendingCycles -= 2;
                        }
                    }
                    if (0 == (CONTROL & TIRQ_IGNR))
                    {
                        if (((ENABLE & TCNTI)!=0) && ((CONTROL & TIRQ_PEND)!=0))
                        {
                            PUSH_PC_TO_STACK();
                            PC = V_TIMER;
                            CONTROL &= unchecked((byte)~TIRQ_PEND);
                            CONTROL |= IRQ_IGNR;
                            if ((ENABLE & T)!=0)
                            {
                                PRESCALER += 2;
                            }
                            pendingCycles -= 2;
                        }
                    }
                }
            }
            while (pendingCycles > 0);
            return cycles - pendingCycles;
        }
        public override void SaveStateBinary(BinaryWriter writer)
        {
            writer.Write(PPC);
            writer.Write(PC);
            writer.Write(timer);
            writer.Write(PRESCALER);
            writer.Write(subtype);
            writer.Write(A);
            writer.Write(PSW);
            writer.Write(STATE);
            writer.Write(ENABLE);
            writer.Write(CONTROL);
            writer.Write(DBBI);
            writer.Write(DBBO);
            writer.Write(P1);
            writer.Write(P2);
            writer.Write(P2_HS);
        }
        public override void LoadStateBinary(BinaryReader reader)
        {
            PPC = reader.ReadUInt16();
            PC = reader.ReadUInt16();
            timer = reader.ReadByte();
            PRESCALER = reader.ReadByte();
            subtype = reader.ReadUInt16();
            A = reader.ReadByte();
            PSW = reader.ReadByte();
            STATE = reader.ReadByte();
            ENABLE = reader.ReadByte();
            CONTROL = reader.ReadByte();
            DBBI = reader.ReadByte();
            DBBO = reader.ReadByte();
            P1 = reader.ReadByte();
            P2 = reader.ReadByte();
            P2_HS = reader.ReadByte();
        }
    }
}
