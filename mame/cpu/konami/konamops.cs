using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mame;

namespace cpu.konami
{
    public partial class KonamiCpu
    {
        void illegal()
        {

        }
        void neg_di()
        {
            ushort r, t;
            t = DIRBYTE();
            r = (ushort)(-t);
            CLR_NZVC();
            SET_FLAGS8(0, t, r);
            WM(EA.LowWord, (byte)r);
        }
        void com_di()
        {
            byte t;
            t = DIRBYTE();
            t = (byte)(~t);
            CLR_NZV();
            SET_NZ8(t);
            SEC();
            WM(EA.LowWord, t);
        }
        void lsr_di()
        {
            byte t;
            t = DIRBYTE();
            CLR_NZC();
            CC |= (byte)(t & CC_C);
            t >>= 1;
            SET_Z8(t);
            WM(EA.LowWord, t);
        }
        void ror_di()
        {
            byte t, r;
            t = DIRBYTE();
            r = (byte)((CC & CC_C) << 7);
            CLR_NZC();
            CC |= (byte)(t & CC_C);
            r |= (byte)(t >> 1);
            SET_NZ8(r);
            WM(EA.LowWord, r);
        }
        void asr_di()
        {
            byte t;
            t = DIRBYTE();
            CLR_NZC();
            CC |= (byte)(t & CC_C);
            t = (byte)((t & 0x80) | (t >> 1));
            SET_NZ8(t);
            WM(EA.LowWord, t);
        }
        void asl_di()
        {
            ushort t, r;
            t = DIRBYTE();
            r = (ushort)(t << 1);
            CLR_NZVC();
            SET_FLAGS8((byte)t, t, r);
            WM(EA.LowWord, (byte)r);
        }
        void rol_di()
        {
            ushort t, r;
            t = DIRBYTE();
            r = (ushort)((CC & CC_C) | (t << 1));
            CLR_NZVC();
            SET_FLAGS8((byte)t, t, r);
            WM(EA.LowWord, (byte)r);
        }
        void dec_di()
        {
            byte t;
            t = DIRBYTE();
            --t;
            CLR_NZV();
            SET_FLAGS8D(t);
            WM(EA.LowWord, t);
        }
        void inc_di()
        {
            byte t;
            t = DIRBYTE();
            ++t;
            CLR_NZV();
            SET_FLAGS8I(t);
            WM(EA.LowWord, t);
        }
        void tst_di()
        {
            byte t;
            t = DIRBYTE();
            CLR_NZV();
            SET_NZ8(t);
        }
        void jmp_di()
        {
            DIRECT();
            PC.d = EA.d;
        }
        void clr_di()
        {
            DIRECT();
            WM(EA.LowWord, 0);
            CLR_NZVC();
            SEZ();
        }
        void nop()
        {

        }
        void sync()
        {
            int_state |= KONAMI_SYNC;
            CHECK_IRQ_LINES();
            if ((int_state & KONAMI_SYNC) != 0)
                if (pendingCycles > 0) pendingCycles = 0;
        }
        void lbra()
        {
            EA = IMMWORD();
            PC.LowWord += EA.LowWord;
            if (EA.LowWord == 0xfffd)
                if (pendingCycles > 0)
                    pendingCycles = 0;
        }
        void lbsr()
        {
            EA = IMMWORD();
            PUSHWORD(PC);
            PC.LowWord += EA.LowWord;
        }
        void daa()
        {
            byte msn, lsn;
            ushort t, cf = 0;
            msn = (byte)(D.HighByte & 0xf0);
            lsn = (byte)(D.HighByte & 0x0f);
            if (lsn > 0x09 || (CC & CC_H) != 0)
                cf |= 0x06;
            if (msn > 0x80 && lsn > 0x09)
                cf |= 0x60;
            if (msn > 0x90 || (CC & CC_C) != 0)
                cf |= 0x60;
            t = (ushort)(cf + D.HighByte);
            CLR_NZV();
            SET_NZ8((byte)t);
            SET_C8(t);
            D.HighByte = (byte)t;
        }
        void orcc()
        {
            byte t;
            t = IMMBYTE();
            CC |= t;
            CHECK_IRQ_LINES();
        }
        void andcc()
        {
            byte t;
            t = IMMBYTE();
            CC &= t;
            CHECK_IRQ_LINES();
        }
        void sex()
        {
            ushort t;
            t = SIGNED(D.LowByte);
            D.LowWord = t;
            CLR_NZ();
            SET_NZ16(t);
        }
        void exg()
        {
            ushort t1 = 0, t2 = 0;
            byte tb;
            tb = IMMBYTE();
            t1 = GETREG(tb >> 4);
            t2 = GETREG(tb & 0x0f);
            SETREG(t2, tb >> 4);
            SETREG(t1, tb & 0x0f);
        }
        void tfr()
        {
            byte tb;
            ushort t = 0;
            tb = IMMBYTE();
            t = GETREG(tb & 0x0f);
            SETREG(t, (tb >> 4) & 0x07);
        }
        void bra()
        {
            byte t;
            t = IMMBYTE();
            PC.LowWord += SIGNED(t);
            if (t == 0xfe)
                if (pendingCycles > 0)
                    pendingCycles = 0;
        }
        void brn()
        {
            byte t;
            t = IMMBYTE();
        }
        void lbrn()
        {
            EA = IMMWORD();
        }
        void bhi()
        {
            BRANCH((CC & (CC_Z | CC_C)) == 0);
        }
        void lbhi()
        {
            LBRANCH((CC & (CC_Z | CC_C)) == 0);
        }
        void bls()
        {
            BRANCH((CC & (CC_Z | CC_C)) != 0);
        }
        void lbls()
        {
            LBRANCH((CC & (CC_Z | CC_C)) != 0);
        }
        void bcc()
        {
            BRANCH((CC & CC_C) == 0);
        }
        void lbcc()
        {
            LBRANCH((CC & CC_C) == 0);
        }
        void bcs()
        {
            BRANCH((CC & CC_C) != 0);
        }
        void lbcs()
        {
            LBRANCH((CC & CC_C) != 0);
        }
        void bne()
        {
            BRANCH((CC & CC_Z) == 0);
        }
        void lbne()
        {
            LBRANCH((CC & CC_Z) == 0);
        }
        void beq()
        {
            BRANCH((CC & CC_Z) != 0);
        }
        void lbeq()
        {
            LBRANCH((CC & CC_Z) != 0);
        }
        void bvc()
        {
            BRANCH((CC & CC_V) == 0);
        }
        void lbvc()
        {
            LBRANCH((CC & CC_V) == 0);
        }
        void bvs()
        {
            BRANCH((CC & CC_V) != 0);
        }
        void lbvs()
        {
            LBRANCH((CC & CC_V) != 0);
        }
        void bpl()
        {
            BRANCH((CC & CC_N) == 0);
        }
        void lbpl()
        {
            LBRANCH((CC & CC_N) == 0);
        }
        void bmi()
        {
            BRANCH((CC & CC_N) != 0);
        }
        void lbmi()
        {
            LBRANCH((CC & CC_N) != 0);
        }
        void bge()
        {
            BRANCH(NXORV() == 0);
        }
        void lbge()
        {
            LBRANCH(NXORV() == 0);
        }
        void blt()
        {
            BRANCH(NXORV() != 0);
        }
        void lblt()
        {
            LBRANCH(NXORV() != 0);
        }
        void bgt()
        {
            BRANCH(!(NXORV() != 0 || (CC & CC_Z) != 0));
        }
        void lbgt()
        {
            LBRANCH(!(NXORV() != 0 || (CC & CC_Z) != 0));
        }
        void ble()
        {
            BRANCH(NXORV() != 0 || (CC & CC_Z) != 0);
        }
        void lble()
        {
            LBRANCH(NXORV() != 0 || (CC & CC_Z) != 0);
        }
        void leax()
        {
            X.LowWord = EA.LowWord;
            CLR_Z();
            SET_Z(X.LowWord);
        }
        void leay()
        {
            Y.LowWord = EA.LowWord;
            CLR_Z();
            SET_Z(Y.LowWord);
        }
        void leas()
        {
            S.LowWord = EA.LowWord;
            int_state |= KONAMI_LDS;
        }
        void leau()
        {
            U.LowWord = EA.LowWord;
        }
        void pshs()
        {
            byte t;
            t = IMMBYTE();
            if ((t & 0x80) != 0) { PUSHWORD(PC); pendingCycles -= 2; }
            if ((t & 0x40) != 0) { PUSHWORD(U); pendingCycles -= 2; }
            if ((t & 0x20) != 0) { PUSHWORD(Y); pendingCycles -= 2; }
            if ((t & 0x10) != 0) { PUSHWORD(X); pendingCycles -= 2; }
            if ((t & 0x08) != 0) { PUSHBYTE(DP.HighByte); pendingCycles -= 1; }
            if ((t & 0x04) != 0) { PUSHBYTE(D.LowByte); pendingCycles -= 1; }
            if ((t & 0x02) != 0) { PUSHBYTE(D.HighByte); pendingCycles -= 1; }
            if ((t & 0x01) != 0) { PUSHBYTE(CC); pendingCycles -= 1; }
        }
        void puls()
        {
            byte t;
            t = IMMBYTE();
            if ((t & 0x01) != 0) { CC = PULLBYTE(); pendingCycles -= 1; }
            if ((t & 0x02) != 0) { D.HighByte = PULLBYTE(); pendingCycles -= 1; }
            if ((t & 0x04) != 0) { D.LowByte = PULLBYTE(); pendingCycles -= 1; }
            if ((t & 0x08) != 0) { DP.HighByte = PULLBYTE(); pendingCycles -= 1; }
            if ((t & 0x10) != 0) { X.d = PULLWORD(); pendingCycles -= 2; }
            if ((t & 0x20) != 0) { Y.d = PULLWORD(); pendingCycles -= 2; }
            if ((t & 0x40) != 0) { U.d = PULLWORD(); pendingCycles -= 2; }
            if ((t & 0x80) != 0) { PC.d = PULLWORD(); pendingCycles -= 2; }
            if ((t & 0x01) != 0) { CHECK_IRQ_LINES(); }
        }
        void pshu()
        {
            byte t;
            t = IMMBYTE();
            if ((t & 0x80) != 0) { PSHUWORD(PC); pendingCycles -= 2; }
            if ((t & 0x40) != 0) { PSHUWORD(S); pendingCycles -= 2; }
            if ((t & 0x20) != 0) { PSHUWORD(Y); pendingCycles -= 2; }
            if ((t & 0x10) != 0) { PSHUWORD(X); pendingCycles -= 2; }
            if ((t & 0x08) != 0) { PSHUBYTE(DP.HighByte); pendingCycles -= 1; }
            if ((t & 0x04) != 0) { PSHUBYTE(D.LowByte); pendingCycles -= 1; }
            if ((t & 0x02) != 0) { PSHUBYTE(D.HighByte); pendingCycles -= 1; }
            if ((t & 0x01) != 0) { PSHUBYTE(CC); pendingCycles -= 1; }
        }
        void pulu()
        {
            byte t;
            t = IMMBYTE();
            if ((t & 0x01) != 0) { CC = PULUBYTE(); pendingCycles -= 1; }
            if ((t & 0x02) != 0) { D.HighByte = PULUBYTE(); pendingCycles -= 1; }
            if ((t & 0x04) != 0) { D.LowByte = PULUBYTE(); pendingCycles -= 1; }
            if ((t & 0x08) != 0) { DP.HighByte = PULUBYTE(); pendingCycles -= 1; }
            if ((t & 0x10) != 0) { X.d = PULUWORD(); pendingCycles -= 2; }
            if ((t & 0x20) != 0) { Y.d = PULUWORD(); pendingCycles -= 2; }
            if ((t & 0x40) != 0) { S.d = PULUWORD(); pendingCycles -= 2; }
            if ((t & 0x80) != 0) { PC.d = PULUWORD(); pendingCycles -= 2; }
            if ((t & 0x01) != 0) { CHECK_IRQ_LINES(); }
        }
        void rts()
        {
            PC.d = PULLWORD();
        }
        void abx()
        {
            X.LowWord += D.LowByte;
        }
        void rti()
        {
            byte t;
            CC = PULLBYTE();
            t = (byte)(CC & CC_E);
            if (t != 0)
            {
                pendingCycles -= 9;
                D.HighByte = PULLBYTE();
                D.LowByte = PULLBYTE();
                DP.HighByte = PULLBYTE();
                X.d = PULLWORD();
                Y.d = PULLWORD();
                U.d = PULLWORD();
            }
            PC.d = PULLWORD();
            CHECK_IRQ_LINES();
        }
        void cwai()
        {
            byte t;
            t = IMMBYTE();
            CC &= t;
            CC |= CC_E;
            PUSHWORD(PC);
            PUSHWORD(U);
            PUSHWORD(Y);
            PUSHWORD(X);
            PUSHBYTE(DP.HighByte);
            PUSHBYTE(D.LowByte);
            PUSHBYTE(D.HighByte);
            PUSHBYTE(CC);
            int_state |= KONAMI_CWAI;
            CHECK_IRQ_LINES();
            if ((int_state & KONAMI_CWAI) != 0)
                if (pendingCycles > 0)
                    pendingCycles = 0;
        }
        void mul()
        {
            ushort t;
            t = (ushort)(D.HighByte * D.LowByte);
            CLR_ZC();
            SET_Z16(t);
            if ((t & 0x80) != 0)
                SEC();
            D.LowWord = t;
        }
        void swi()
        {
            CC |= CC_E;
            PUSHWORD(PC);
            PUSHWORD(U);
            PUSHWORD(Y);
            PUSHWORD(X);
            PUSHBYTE(DP.HighByte);
            PUSHBYTE(D.LowByte);
            PUSHBYTE(D.HighByte);
            PUSHBYTE(CC);
            CC |= (byte)(CC_IF | CC_II);
            PC.d = RM16(0xfffa);
            //CHANGE_PC;
        }
        void swi2()
        {
            CC |= CC_E;
            PUSHWORD(PC);
            PUSHWORD(U);
            PUSHWORD(Y);
            PUSHWORD(X);
            PUSHBYTE(DP.HighByte);
            PUSHBYTE(D.LowByte);
            PUSHBYTE(D.HighByte);
            PUSHBYTE(CC);
            PC.d = RM16(0xfff4);
            //CHANGE_PC;
        }
        void swi3()
        {
            CC |= CC_E;
            PUSHWORD(PC);
            PUSHWORD(U);
            PUSHWORD(Y);
            PUSHWORD(X);
            PUSHBYTE(DP.HighByte);
            PUSHBYTE(D.LowByte);
            PUSHBYTE(D.HighByte);
            PUSHBYTE(CC);
            PC.d = RM16(0xfff2);
            //CHANGE_PC;
        }
        void nega()
        {
            ushort r;
            r = (ushort)(-D.HighByte);
            CLR_NZVC();
            SET_FLAGS8(0, D.HighByte, r);
            D.HighByte = (byte)r;
        }
        void coma()
        {
            D.HighByte = (byte)(~D.HighByte);
            CLR_NZV();
            SET_NZ8(D.HighByte);
            SEC();
        }
        void lsra()
        {
            CLR_NZC();
            CC |= (byte)(D.HighByte & CC_C);
            D.HighByte >>= 1;
            SET_Z8(D.HighByte);
        }
        void rora()
        {
            byte r;
            r = (byte)((CC & CC_C) << 7);
            CLR_NZC();
            CC |= (byte)(D.HighByte & CC_C);
            r |= (byte)(D.HighByte >> 1);
            SET_NZ8(r);
            D.HighByte = r;
        }
        void asra()
        {
            CLR_NZC();
            CC |= (byte)(D.HighByte & CC_C);
            D.HighByte = (byte)((D.HighByte & 0x80) | (D.HighByte >> 1));
            SET_NZ8(D.HighByte);
        }
        void asla()
        {
            ushort r;
            r = (ushort)(D.HighByte << 1);
            CLR_NZVC();
            SET_FLAGS8(D.HighByte, D.HighByte, r);
            D.HighByte = (byte)r;
        }
        void rola()
        {
            ushort t, r;
            t = D.HighByte;
            r = (ushort)((CC & CC_C) | (t << 1));
            CLR_NZVC();
            SET_FLAGS8((byte)t, t, r);
            D.HighByte = (byte)r;
        }
        void deca()
        {
            --D.HighByte;
            CLR_NZV();
            SET_FLAGS8D(D.HighByte);
        }
        void inca()
        {
            ++D.HighByte;
            CLR_NZV();
            SET_FLAGS8I(D.HighByte);
        }
        void tsta()
        {
            CLR_NZV();
            SET_NZ8(D.HighByte);
        }
        void clra()
        {
            D.HighByte = 0;
            CLR_NZVC();
            SEZ();
        }
        void negb()
        {
            ushort r;
            r = (ushort)(-D.LowByte);
            CLR_NZVC();
            SET_FLAGS8(0, D.LowByte, r);
            D.LowByte = (byte)r;
        }
        void comb()
        {
            D.LowByte = (byte)(~D.LowByte);
            CLR_NZV();
            SET_NZ8(D.LowByte);
            SEC();
        }
        void lsrb()
        {
            CLR_NZC();
            CC |= (byte)(D.LowByte & CC_C);
            D.LowByte >>= 1;
            SET_Z8(D.LowByte);
        }
        void rorb()
        {
            byte r;
            r = (byte)((CC & CC_C) << 7);
            CLR_NZC();
            CC |= (byte)(D.LowByte & CC_C);
            r |= (byte)(D.LowByte >> 1);
            SET_NZ8(r);
            D.LowByte = r;
        }
        void asrb()
        {
            CLR_NZC();
            CC |= (byte)(D.LowByte & CC_C);
            D.LowByte = (byte)((D.LowByte & 0x80) | (D.LowByte >> 1));
            SET_NZ8(D.LowByte);
        }
        void aslb()
        {
            ushort r;
            r = (ushort)(D.LowByte << 1);
            CLR_NZVC();
            SET_FLAGS8(D.LowByte, D.LowByte, r);
            D.LowByte = (byte)r;
        }
        void rolb()
        {
            ushort t, r;
            t = D.LowByte;
            r = (ushort)(CC & CC_C);
            r |= (ushort)(t << 1);
            CLR_NZVC();
            SET_FLAGS8((byte)t, t, r);
            D.LowByte = (byte)r;
        }
        void decb()
        {
            --D.LowByte;
            CLR_NZV();
            SET_FLAGS8D(D.LowByte);
        }
        void incb()
        {
            ++D.LowByte;
            CLR_NZV();
            SET_FLAGS8I(D.LowByte);
        }
        void tstb()
        {
            CLR_NZV();
            SET_NZ8(D.LowByte);
        }
        void clrb()
        {
            D.LowByte = 0;
            CLR_NZVC();
            SEZ();
        }
        void neg_ix()
        {
            ushort r, t;
            t = RM(EA.LowWord);
            r = (ushort)(-t);
            CLR_NZVC();
            SET_FLAGS8(0, t, r);
            WM(EA.LowWord, (byte)r);
        }
        void com_ix()
        {
            byte t;
            t = (byte)(~RM(EA.LowWord));
            CLR_NZV();
            SET_NZ8(t);
            SEC();
            WM(EA.LowWord, t);
        }
        void lsr_ix()
        {
            byte t;
            t = RM(EA.LowWord);
            CLR_NZC();
            CC |= (byte)(t & CC_C);
            t >>= 1;
            SET_Z8(t);
            WM(EA.LowWord, t);
        }
        void ror_ix()
        {
            byte t, r;
            t = RM(EA.LowWord);
            r = (byte)((CC & CC_C) << 7);
            CLR_NZC();
            CC |= (byte)(t & CC_C);
            r |= (byte)(t >> 1);
            SET_NZ8(r);
            WM(EA.LowWord, r);
        }
        void asr_ix()
        {
            byte t;
            t = RM(EA.LowWord);
            CLR_NZC();
            CC |= (byte)(t & CC_C);
            t = (byte)((t & 0x80) | (t >> 1));
            SET_NZ8(t);
            WM(EA.LowWord, t);
        }
        void asl_ix()
        {
            ushort t, r;
            t = RM(EA.LowWord);
            r = (ushort)(t << 1);
            CLR_NZVC();
            SET_FLAGS8((byte)t, t, r);
            WM(EA.LowWord, (byte)r);
        }
        void rol_ix()
        {
            ushort t, r;
            t = RM(EA.LowWord);
            r = (ushort)(CC & CC_C);
            r |= (ushort)(t << 1);
            CLR_NZVC();
            SET_FLAGS8((byte)t, t, r);
            WM(EA.LowWord, (byte)r);
        }
        void dec_ix()
        {
            byte t;
            t = (byte)(RM(EA.LowWord) - 1);
            CLR_NZV();
            SET_FLAGS8D(t);
            WM(EA.LowWord, t);
        }
        void inc_ix()
        {
            byte t;
            t = (byte)(RM(EA.LowWord) + 1);
            CLR_NZV();
            SET_FLAGS8I(t);
            WM(EA.LowWord, t);
        }
        void tst_ix()
        {
            byte t;
            t = RM(EA.LowWord);
            CLR_NZV();
            SET_NZ8(t);
        }
        void jmp_ix()
        {
            PC.d = EA.d;
        }
        void clr_ix()
        {
            RM(EA.LowWord);
            WM(EA.LowWord, 0);
            CLR_NZVC(); SEZ();
        }
        void neg_ex()
        {
            ushort r, t;
            t = EXTBYTE();
            r = (ushort)(-t);
            CLR_NZVC();
            SET_FLAGS8(0, t, r);
            WM(EA.LowWord, (byte)r);
        }
        void com_ex()
        {
            byte t;
            t = EXTBYTE();
            t = (byte)(~t);
            CLR_NZV();
            SET_NZ8(t);
            SEC();
            WM(EA.LowWord, t);
        }
        void lsr_ex()
        {
            byte t;
            t = EXTBYTE();
            CLR_NZC();
            CC |= (byte)(t & CC_C);
            t >>= 1;
            SET_Z8(t);
            WM(EA.LowWord, t);
        }
        void ror_ex()
        {
            byte t, r;
            t = EXTBYTE();
            r = (byte)((CC & CC_C) << 7);
            CLR_NZC();
            CC |= (byte)(t & CC_C);
            r |= (byte)(t >> 1);
            SET_NZ8(r);
            WM(EA.LowWord, r);
        }
        void asr_ex()
        {
            byte t;
            t = EXTBYTE();
            CLR_NZC();
            CC |= (byte)(t & CC_C);
            t = (byte)((t & 0x80) | (t >> 1));
            SET_NZ8(t);
            WM(EA.LowWord, t);
        }
        void asl_ex()
        {
            ushort t, r;
            t = EXTBYTE();
            r = (ushort)(t << 1);
            CLR_NZVC();
            SET_FLAGS8((byte)t, t, r);
            WM(EA.LowWord, (byte)r);
        }
        void rol_ex()
        {
            ushort t, r;
            t = EXTBYTE();
            r = (ushort)((CC & CC_C) | (t << 1));
            CLR_NZVC();
            SET_FLAGS8((byte)t, t, r);
            WM(EA.LowWord, (byte)r);
        }
        void dec_ex()
        {
            byte t;
            t = EXTBYTE();
            --t;
            CLR_NZV();
            SET_FLAGS8D(t);
            WM(EA.LowWord, t);
        }
        void inc_ex()
        {
            byte t;
            t = EXTBYTE();
            ++t;
            CLR_NZV();
            SET_FLAGS8I(t);
            WM(EA.LowWord, t);
        }
        void tst_ex()
        {
            byte t;
            t = EXTBYTE();
            CLR_NZV();
            SET_NZ8(t);
        }
        void jmp_ex()
        {
            EXTENDED();
            PC.d = EA.d;
        }
        void clr_ex()
        {
            EXTENDED();
            WM(EA.LowWord, 0);
            CLR_NZVC();
            SEZ();
        }
        void suba_im()
        {
            ushort t, r;
            t = IMMBYTE();
            r = (ushort)(D.HighByte - t);
            CLR_NZVC();
            SET_FLAGS8(D.HighByte, t, r);
            D.HighByte = (byte)r;
        }
        void cmpa_im()
        {
            ushort t, r;
            int i1, i2, i3;
            t = IMMBYTE();
            r = (ushort)(D.HighByte - t);
            i1 = CC;
            CLR_NZVC();
            i2 = CC;
            SET_FLAGS8(D.HighByte, t, r);
            i3 = CC;
        }
        void sbca_im()
        {
            ushort t, r;
            t = IMMBYTE();
            r = (ushort)(D.HighByte - t - (CC & CC_C));
            CLR_NZVC();
            SET_FLAGS8(D.HighByte, t, r);
            D.HighByte = (byte)r;
        }
        void subd_im()
        {
            uint r, d;
            RegisterPair b;
            b = IMMWORD();
            d = D.LowWord;
            r = d - b.d;
            CLR_NZVC();
            SET_FLAGS16((ushort)d, (ushort)b.d, r);
            D.LowWord = (ushort)r;
        }
        void cmpd_im()
        {
            uint r, d;
            RegisterPair b;
            b = IMMWORD();
            d = D.LowWord;
            r = d - b.d;
            CLR_NZVC();
            SET_FLAGS16((ushort)d, (ushort)b.d, r);
        }
        void cmpu_im()
        {
            uint r, d;
            RegisterPair b;
            b = IMMWORD();
            d = U.LowWord;
            r = d - b.d;
            CLR_NZVC();
            SET_FLAGS16((ushort)d, (ushort)b.d, r);
        }
        void anda_im()
        {
            byte t;
            t = IMMBYTE();
            D.HighByte &= t;
            CLR_NZV();
            SET_NZ8(D.HighByte);
        }
        void bita_im()
        {
            byte t, r;
            t = IMMBYTE();
            r = (byte)(D.HighByte & t);
            CLR_NZV();
            SET_NZ8(r);
        }
        void lda_im()
        {
            D.HighByte = IMMBYTE();
            CLR_NZV();
            SET_NZ8(D.HighByte);
        }
        void sta_im()
        {
            CLR_NZV();
            SET_NZ8(D.HighByte);
            IMM8();
            WM(EA.LowWord, D.HighByte);
        }
        void eora_im()
        {
            byte t;
            t = IMMBYTE();
            D.HighByte ^= t;
            CLR_NZV();
            SET_NZ8(D.HighByte);
        }
        void adca_im()
        {
            ushort t, r;
            t = IMMBYTE();
            r = (ushort)(D.HighByte + t + (CC & CC_C));
            CLR_HNZVC();
            SET_FLAGS8(D.HighByte, t, r);
            SET_H(D.HighByte, (byte)t, (byte)r);
            D.HighByte = (byte)r;
        }
        void ora_im()
        {
            byte t;
            t = IMMBYTE();
            D.HighByte |= t;
            CLR_NZV();
            SET_NZ8(D.HighByte);
        }
        void adda_im()
        {
            ushort t, r;
            t = IMMBYTE();
            r = (ushort)(D.HighByte + t);
            CLR_HNZVC();
            SET_FLAGS8(D.HighByte, t, r);
            SET_H(D.HighByte, (byte)t, (byte)r);
            D.HighByte = (byte)r;
        }
        void cmpx_im()
        {
            uint r, d;
            RegisterPair b;
            b = IMMWORD();
            d = X.LowWord;
            r = d - b.d;
            CLR_NZVC();
            SET_FLAGS16((ushort)d, (ushort)b.d, r);
        }
        void cmpy_im()
        {
            uint r, d;
            RegisterPair b;
            b = IMMWORD();
            d = Y.LowWord;
            r = d - b.d;
            CLR_NZVC();
            SET_FLAGS16((ushort)d, (ushort)b.d, r);
        }
        void cmps_im()
        {
            uint r, d;
            RegisterPair b;
            b = IMMWORD();
            d = S.LowWord;
            r = d - b.d;
            CLR_NZVC();
            SET_FLAGS16((ushort)d, (ushort)b.d, r);
        }
        void bsr()
        {
            byte t;
            t = IMMBYTE();
            PUSHWORD(PC);
            PC.LowWord += SIGNED(t);
            //CHANGE_PC;
        }
        void ldx_im()
        {
            X = IMMWORD();
            CLR_NZV();
            SET_NZ16(X.LowWord);
        }
        void ldy_im()
        {
            Y = IMMWORD();
            CLR_NZV();
            SET_NZ16(Y.LowWord);
        }
        void stx_im()
        {
            CLR_NZV();
            SET_NZ16(X.LowWord);
            IMM16();
            WM16(EA.LowWord, X);
        }
        void sty_im()
        {
            CLR_NZV();
            SET_NZ16(Y.LowWord);
            IMM16();
            WM16(EA.LowWord, Y);
        }
        void suba_di()
        {
            ushort t, r;
            t = DIRBYTE();
            r = (ushort)(D.HighByte - t);
            CLR_NZVC();
            SET_FLAGS8(D.HighByte, t, r);
            D.HighByte = (byte)r;
        }
        void cmpa_di()
        {
            ushort t, r;
            t = DIRBYTE();
            r = (ushort)(D.HighByte - t);
            CLR_NZVC();
            SET_FLAGS8(D.HighByte, t, r);
        }
        void sbca_di()
        {
            ushort t, r;
            t = DIRBYTE();
            r = (ushort)(D.HighByte - t - (CC & CC_C));
            CLR_NZVC();
            SET_FLAGS8(D.HighByte, t, r);
            D.HighByte = (byte)r;
        }
        void subd_di()
        {
            uint r, d;
            RegisterPair b;
            b = DIRWORD();
            d = D.LowWord;
            r = d - b.d;
            CLR_NZVC();
            SET_FLAGS16((ushort)d, (ushort)b.d, r);
            D.LowWord = (ushort)r;
        }
        void cmpd_di()
        {
            uint r, d;
            RegisterPair b;
            b = DIRWORD();
            d = D.LowWord;
            r = d - b.d;
            CLR_NZVC();
            SET_FLAGS16((ushort)d, (ushort)b.d, r);
        }
        void cmpu_di()
        {
            uint r, d;
            RegisterPair b;
            b = DIRWORD();
            d = U.LowWord;
            r = d - b.d;
            CLR_NZVC();
            SET_FLAGS16(U.LowWord, (ushort)b.d, r);
        }
        void anda_di()
        {
            byte t;
            t = DIRBYTE();
            D.HighByte &= t;
            CLR_NZV();
            SET_NZ8(D.HighByte);
        }
        void bita_di()
        {
            byte t, r;
            t = DIRBYTE();
            r = (byte)(D.HighByte & t);
            CLR_NZV();
            SET_NZ8(r);
        }
        void lda_di()
        {
            D.HighByte = DIRBYTE();
            CLR_NZV();
            SET_NZ8(D.HighByte);
        }
        void sta_di()
        {
            CLR_NZV();
            SET_NZ8(D.HighByte);
            DIRECT();
            WM(EA.LowWord, D.HighByte);
        }
        void eora_di()
        {
            byte t;
            t = DIRBYTE();
            D.HighByte ^= t;
            CLR_NZV();
            SET_NZ8(D.HighByte);
        }
        void adca_di()
        {
            ushort t, r;
            t = DIRBYTE();
            r = (ushort)(D.HighByte + t + (CC & CC_C));
            CLR_HNZVC();
            SET_FLAGS8(D.HighByte, t, r);
            SET_H(D.HighByte, (byte)t, (byte)r);
            D.HighByte = (byte)r;
        }
        void ora_di()
        {
            byte t;
            t = DIRBYTE();
            D.HighByte |= t;
            CLR_NZV();
            SET_NZ8(D.HighByte);
        }
        void adda_di()
        {
            ushort t, r;
            t = DIRBYTE();
            r = (ushort)(D.HighByte + t);
            CLR_HNZVC();
            SET_FLAGS8(D.HighByte, t, r);
            SET_H(D.HighByte, (byte)t, (byte)r);
            D.HighByte = (byte)r;
        }
        void cmpx_di()
        {
            uint r, d;
            RegisterPair b;
            b = DIRWORD();
            d = X.LowWord;
            r = d - b.d;
            CLR_NZVC();
            SET_FLAGS16((ushort)d, (ushort)b.d, r);
        }
        void cmpy_di()
        {
            uint r, d;
            RegisterPair b;
            b = DIRWORD();
            d = Y.LowWord;
            r = d - b.d;
            CLR_NZVC();
            SET_FLAGS16((ushort)d, (ushort)b.d, r);
        }
        void cmps_di()
        {
            uint r, d;
            RegisterPair b;
            b = DIRWORD();
            d = S.LowWord;
            r = d - b.d;
            CLR_NZVC();
            SET_FLAGS16((ushort)d, (ushort)b.d, r);
        }
        void jsr_di()
        {
            DIRECT();
            PUSHWORD(PC);
            PC.d = EA.d;
            //CHANGE_PC;
        }
        void ldx_di()
        {
            X = DIRWORD();
            CLR_NZV();
            SET_NZ16(X.LowWord);
        }
        void ldy_di()
        {
            Y = DIRWORD();
            CLR_NZV();
            SET_NZ16(Y.LowWord);
        }
        void stx_di()
        {
            CLR_NZV();
            SET_NZ16(X.LowWord);
            DIRECT();
            WM16(EA.LowWord, X);
        }
        void sty_di()
        {
            CLR_NZV();
            SET_NZ16(Y.LowWord);
            DIRECT();
            WM16(EA.LowWord, Y);
        }
        void suba_ix()
        {
            ushort t, r;
            t = RM(EA.LowWord);
            r = (ushort)(D.HighByte - t);
            CLR_NZVC();
            SET_FLAGS8(D.HighByte, t, r);
            D.HighByte = (byte)r;
        }
        void cmpa_ix()
        {
            ushort t, r;
            t = RM(EA.LowWord);
            r = (ushort)(D.HighByte - t);
            CLR_NZVC();
            SET_FLAGS8(D.HighByte, t, r);
        }
        void sbca_ix()
        {
            ushort t, r;
            t = RM(EA.LowWord);
            r = (ushort)(D.HighByte - t - (CC & CC_C));
            CLR_NZVC();
            SET_FLAGS8(D.HighByte, t, r);
            D.HighByte = (byte)r;
        }
        void subd_ix()
        {
            uint r, d;
            RegisterPair b;
            b.d = RM16(EA.LowWord);
            d = D.LowWord;
            r = d - b.d;
            CLR_NZVC();
            SET_FLAGS16((ushort)d, (ushort)b.d, r);
            D.LowWord = (ushort)r;
        }
        void cmpd_ix()
        {
            uint r, d;
            RegisterPair b;
            b.d = RM16(EA.LowWord);
            d = D.LowWord;
            r = d - b.d;
            CLR_NZVC();
            SET_FLAGS16((ushort)d, (ushort)b.d, r);
        }
        void cmpu_ix()
        {
            uint r;
            RegisterPair b = new RegisterPair();
            b.d = RM16(EA.LowWord);
            r = U.LowWord - b.d;
            CLR_NZVC();
            SET_FLAGS16(U.LowWord, b.LowWord, r);
        }
        void anda_ix()
        {
            D.HighByte &= RM(EA.LowWord);
            CLR_NZV();
            SET_NZ8(D.HighByte);
        }
        void bita_ix()
        {
            byte r;
            r = (byte)(D.HighByte & RM(EA.LowWord));
            CLR_NZV();
            SET_NZ8(r);
        }
        void lda_ix()
        {
            D.HighByte = RM(EA.LowWord);
            CLR_NZV();
            SET_NZ8(D.HighByte);
        }
        void sta_ix()
        {
            CLR_NZV();
            SET_NZ8(D.HighByte);
            WM(EA.LowWord, D.HighByte);
        }
        void eora_ix()
        {
            D.HighByte ^= RM(EA.LowWord);
            CLR_NZV();
            SET_NZ8(D.HighByte);
        }
        void adca_ix()
        {
            ushort t, r;
            t = RM(EA.LowWord);
            r = (ushort)(D.HighByte + t + (CC & CC_C));
            CLR_HNZVC();
            SET_FLAGS8(D.HighByte, t, r);
            SET_H(D.HighByte, (byte)t, (byte)r);
            D.HighByte = (byte)r;
        }
        void ora_ix()
        {
            D.HighByte |= RM(EA.LowWord);
            CLR_NZV();
            SET_NZ8(D.HighByte);
        }
        void adda_ix()
        {
            ushort t, r;
            t = RM(EA.LowWord);
            r = (ushort)(D.HighByte + t);
            CLR_HNZVC();
            SET_FLAGS8(D.HighByte, t, r);
            SET_H(D.HighByte, (byte)t, (byte)r);
            D.HighByte = (byte)r;
        }
        void cmpx_ix()
        {
            uint r, d;
            RegisterPair b;
            b.d = RM16(EA.LowWord);
            d = X.LowWord;
            r = d - b.d;
            CLR_NZVC();
            SET_FLAGS16((ushort)d, (ushort)b.d, r);
        }
        void cmpy_ix()
        {
            uint r, d;
            RegisterPair b;
            b.d = RM16(EA.LowWord);
            d = Y.LowWord;
            r = d - b.d;
            CLR_NZVC();
            SET_FLAGS16((ushort)d, (ushort)b.d, r);
        }
        void cmps_ix()
        {
            uint r, d;
            RegisterPair b;
            b.d = RM16(EA.LowWord);
            d = S.LowWord;
            r = d - b.d;
            CLR_NZVC();
            SET_FLAGS16((ushort)d, (ushort)b.d, r);
        }
        void jsr_ix()
        {
            PUSHWORD(PC);
            PC.d = EA.d;
        }
        void ldx_ix()
        {
            X.LowWord = RM16(EA.LowWord);
            CLR_NZV();
            SET_NZ16(X.LowWord);
        }
        void ldy_ix()
        {
            Y.LowWord = RM16(EA.LowWord);
            CLR_NZV();
            SET_NZ16(Y.LowWord);
        }
        void stx_ix()
        {
            CLR_NZV();
            SET_NZ16(X.LowWord);
            WM16(EA.LowWord, X);
        }
        void sty_ix()
        {
            CLR_NZV();
            SET_NZ16(Y.LowWord);
            WM16(EA.LowWord, Y);
        }
        void suba_ex()
        {
            ushort t, r;
            t = EXTBYTE();
            r = (ushort)(D.HighByte - t);
            CLR_NZVC();
            SET_FLAGS8(D.HighByte, t, r);
            D.HighByte = (byte)r;
        }
        void cmpa_ex()
        {
            ushort t, r;
            t = EXTBYTE();
            r = (ushort)(D.HighByte - t);
            CLR_NZVC();
            SET_FLAGS8(D.HighByte, t, r);
        }
        void sbca_ex()
        {
            ushort t, r;
            t = EXTBYTE();
            r = (ushort)(D.HighByte - t - (CC & CC_C));
            CLR_NZVC();
            SET_FLAGS8(D.HighByte, t, r);
            D.HighByte = (byte)r;
        }
        void subd_ex()
        {
            uint r, d;
            RegisterPair b;
            b = EXTWORD();
            d = D.LowWord;
            r = d - b.d;
            CLR_NZVC();
            SET_FLAGS16((ushort)d, (ushort)b.d, r);
            D.LowWord = (ushort)r;
        }
        void cmpd_ex()
        {
            uint r, d;
            RegisterPair b;
            b = EXTWORD();
            d = D.LowWord;
            r = d - b.d;
            CLR_NZVC();
            SET_FLAGS16((ushort)d, (ushort)b.d, r);
        }
        void cmpu_ex()
        {
            uint r, d;
            RegisterPair b;
            b = EXTWORD();
            d = U.LowWord;
            r = d - b.d;
            CLR_NZVC();
            SET_FLAGS16((ushort)d, (ushort)b.d, r);
        }
        void anda_ex()
        {
            byte t;
            t = EXTBYTE();
            D.HighByte &= t;
            CLR_NZV();
            SET_NZ8(D.HighByte);
        }
        void bita_ex()
        {
            byte t, r;
            t = EXTBYTE();
            r = (byte)(D.HighByte & t);
            CLR_NZV(); SET_NZ8(r);
        }
        void lda_ex()
        {
            D.HighByte = EXTBYTE();
            CLR_NZV();
            SET_NZ8(D.HighByte);
        }
        void sta_ex()
        {
            CLR_NZV();
            SET_NZ8(D.HighByte);
            EXTENDED();
            WM(EA.LowWord, D.HighByte);
        }
        void eora_ex()
        {
            byte t;
            t = EXTBYTE();
            D.HighByte ^= t;
            CLR_NZV();
            SET_NZ8(D.HighByte);
        }
        void adca_ex()
        {
            ushort t, r;
            t = EXTBYTE();
            r = (ushort)(D.HighByte + t + (CC & CC_C));
            CLR_HNZVC();
            SET_FLAGS8(D.HighByte, t, r);
            SET_H(D.HighByte, (byte)t, (byte)r);
            D.HighByte = (byte)r;
        }
        void ora_ex()
        {
            byte t;
            t = EXTBYTE();
            D.HighByte |= t;
            CLR_NZV();
            SET_NZ8(D.HighByte);
        }
        void adda_ex()
        {
            ushort t, r;
            t = EXTBYTE();
            r = (ushort)(D.HighByte + t);
            CLR_HNZVC();
            SET_FLAGS8(D.HighByte, t, r);
            SET_H(D.HighByte, (byte)t, (byte)r);
            D.HighByte = (byte)r;
        }
        void cmpx_ex()
        {
            uint r, d;
            RegisterPair b;
            b = EXTWORD();
            d = X.LowWord;
            r = d - b.d;
            CLR_NZVC();
            SET_FLAGS16((ushort)d, (ushort)b.d, r);
        }
        void cmpy_ex()
        {
            uint r, d;
            RegisterPair b;
            b = EXTWORD();
            d = Y.LowWord;
            r = d - b.d;
            CLR_NZVC();
            SET_FLAGS16((ushort)d, (ushort)b.d, r);
        }
        void cmps_ex()
        {
            uint r, d;
            RegisterPair b;
            b = EXTWORD();
            d = S.LowWord;
            r = d - b.d;
            CLR_NZVC();
            SET_FLAGS16((ushort)d, (ushort)b.d, r);
        }
        void jsr_ex()
        {
            EXTENDED();
            PUSHWORD(PC);
            PC.d = EA.d;
            //CHANGE_PC;
        }
        void ldx_ex()
        {
            X = EXTWORD();
            CLR_NZV();
            SET_NZ16(X.LowWord);
        }
        void ldy_ex()
        {
            Y = EXTWORD();
            CLR_NZV();
            SET_NZ16(Y.LowWord);
        }
        void stx_ex()
        {
            CLR_NZV();
            SET_NZ16(X.LowWord);
            EXTENDED();
            WM16(EA.LowWord, X);
        }
        void sty_ex()
        {
            CLR_NZV();
            SET_NZ16(Y.LowWord);
            EXTENDED();
            WM16(EA.LowWord, Y);
        }
        void subb_im()
        {
            ushort t, r;
            t = IMMBYTE();
            r = (ushort)(D.LowByte - t);
            CLR_NZVC();
            SET_FLAGS8(D.LowByte, t, r);
            D.LowByte = (byte)r;
        }
        void cmpb_im()
        {
            ushort t, r;
            t = IMMBYTE();
            r = (ushort)(D.LowByte - t);
            CLR_NZVC();
            SET_FLAGS8(D.LowByte, t, r);
        }
        void sbcb_im()
        {
            ushort t, r;
            t = IMMBYTE();
            r = (ushort)(D.LowByte - t - (CC & CC_C));
            CLR_NZVC();
            SET_FLAGS8(D.LowByte, t, r);
            D.LowByte = (byte)r;
        }
        void addd_im()
        {
            uint r, d;
            RegisterPair b;
            b = IMMWORD();
            d = D.LowWord;
            r = d + b.d;
            CLR_NZVC();
            SET_FLAGS16((ushort)d, (ushort)b.d, r);
            D.LowWord = (ushort)r;
        }
        void andb_im()
        {
            byte t;
            t = IMMBYTE();
            D.LowByte &= t;
            CLR_NZV();
            SET_NZ8(D.LowByte);
        }
        void bitb_im()
        {
            byte t, r;
            t = IMMBYTE();
            r = (byte)(D.LowByte & t);
            CLR_NZV();
            SET_NZ8(r);
        }
        void ldb_im()
        {
            D.LowByte = IMMBYTE();
            CLR_NZV();
            SET_NZ8(D.LowByte);
        }
        void stb_im()
        {
            CLR_NZV();
            SET_NZ8(D.LowByte);
            IMM8();
            WM(EA.LowWord, D.LowByte);
        }
        void eorb_im()
        {
            byte t;
            t = IMMBYTE();
            D.LowByte ^= t;
            CLR_NZV();
            SET_NZ8(D.LowByte);
        }
        void adcb_im()
        {
            ushort t, r;
            t = IMMBYTE();
            r = (ushort)(D.LowByte + t + (CC & CC_C));
            CLR_HNZVC();
            SET_FLAGS8(D.LowByte, t, r);
            SET_H(D.LowByte, (byte)t, (byte)r);
            D.LowByte = (byte)r;
        }
        void orb_im()
        {
            byte t;
            t = IMMBYTE();
            D.LowByte |= t;
            CLR_NZV();
            SET_NZ8(D.LowByte);
        }
        void addb_im()
        {
            ushort t, r;
            t = IMMBYTE();
            r = (ushort)(D.LowByte + t);
            CLR_HNZVC();
            SET_FLAGS8(D.LowByte, t, r);
            SET_H(D.LowByte, (byte)t, (byte)r);
            D.LowByte = (byte)r;
        }
        void ldd_im()
        {
            D = IMMWORD();
            CLR_NZV();
            SET_NZ16(D.LowWord);
        }
        void std_im()
        {
            CLR_NZV();
            SET_NZ16(D.LowWord);
            IMM16();
            WM16(EA.LowWord, D);
        }
        void ldu_im()
        {
            U = IMMWORD();
            CLR_NZV();
            SET_NZ16(U.LowWord);
        }
        void lds_im()
        {
            S = IMMWORD();
            CLR_NZV();
            SET_NZ16(S.LowWord);
            int_state |= KONAMI_LDS;
        }
        void stu_im()
        {
            CLR_NZV();
            SET_NZ16(U.LowWord);
            IMM16();
            WM16(EA.LowWord, U);
        }
        void sts_im()
        {
            CLR_NZV();
            SET_NZ16(S.LowWord);
            IMM16();
            WM16(EA.LowWord, S);
        }
        void subb_di()
        {
            ushort t, r;
            t = DIRBYTE();
            r = (ushort)(D.LowByte - t);
            CLR_NZVC();
            SET_FLAGS8(D.LowByte, t, r);
            D.LowByte = (byte)r;
        }
        void cmpb_di()
        {
            ushort t, r;
            t = DIRBYTE();
            r = (ushort)(D.LowByte - t);
            CLR_NZVC();
            SET_FLAGS8(D.LowByte, t, r);
        }
        void sbcb_di()
        {
            ushort t, r;
            t = DIRBYTE();
            r = (ushort)(D.LowByte - t - (CC & CC_C));
            CLR_NZVC();
            SET_FLAGS8(D.LowByte, t, r);
            D.LowByte = (byte)r;
        }
        void addd_di()
        {
            uint r, d;
            RegisterPair b;
            b = DIRWORD();
            d = D.LowWord;
            r = d + b.d;
            CLR_NZVC();
            SET_FLAGS16((ushort)d, (ushort)b.d, r);
            D.LowWord = (ushort)r;
        }
        void andb_di()
        {
            byte t;
            t = DIRBYTE();
            D.LowByte &= t;
            CLR_NZV();
            SET_NZ8(D.LowByte);
        }
        void bitb_di()
        {
            byte t, r;
            t = DIRBYTE();
            r = (byte)(D.LowByte & t);
            CLR_NZV();
            SET_NZ8(r);
        }
        void ldb_di()
        {
            D.LowByte = DIRBYTE();
            CLR_NZV();
            SET_NZ8(D.LowByte);
        }
        void stb_di()
        {
            CLR_NZV();
            SET_NZ8(D.LowByte);
            DIRECT();
            WM(EA.LowWord, D.LowByte);
        }
        void eorb_di()
        {
            byte t;
            t = DIRBYTE();
            D.LowByte ^= t;
            CLR_NZV();
            SET_NZ8(D.LowByte);
        }
        void adcb_di()
        {
            ushort t, r;
            t = DIRBYTE();
            r = (ushort)(D.LowByte + t + (CC & CC_C));
            CLR_HNZVC();
            SET_FLAGS8(D.LowByte, t, r);
            SET_H(D.LowByte, (byte)t, (byte)r);
            D.LowByte = (byte)r;
        }
        void orb_di()
        {
            byte t;
            t = DIRBYTE();
            D.LowByte |= t;
            CLR_NZV();
            SET_NZ8(D.LowByte);
        }
        void addb_di()
        {
            ushort t, r;
            t = DIRBYTE();
            r = (ushort)(D.LowByte + t);
            CLR_HNZVC();
            SET_FLAGS8(D.LowByte, t, r);
            SET_H(D.LowByte, (byte)t, (byte)r);
            D.LowByte = (byte)r;
        }
        void ldd_di()
        {
            D = DIRWORD();
            CLR_NZV();
            SET_NZ16(D.LowWord);
        }
        void std_di()
        {
            CLR_NZV();
            SET_NZ16(D.LowWord);
            DIRECT();
            WM16(EA.LowWord, D);
        }
        void ldu_di()
        {
            U = DIRWORD();
            CLR_NZV();
            SET_NZ16(U.LowWord);
        }
        void lds_di()
        {
            S = DIRWORD();
            CLR_NZV();
            SET_NZ16(S.LowWord);
            int_state |= KONAMI_LDS;
        }
        void stu_di()
        {
            CLR_NZV();
            SET_NZ16(U.LowWord);
            DIRECT();
            WM16(EA.LowWord, U);
        }
        void sts_di()
        {
            CLR_NZV();
            SET_NZ16(S.LowWord);
            DIRECT();
            WM16(EA.LowWord, S);
        }
        void subb_ix()
        {
            ushort t, r;
            t = RM(EA.LowWord);
            r = (ushort)(D.LowByte - t);
            CLR_NZVC();
            SET_FLAGS8(D.LowByte, t, r);
            D.LowByte = (byte)r;
        }
        void cmpb_ix()
        {
            ushort t, r;
            t = RM(EA.LowWord);
            r = (ushort)(D.LowByte - t);
            CLR_NZVC();
            SET_FLAGS8(D.LowByte, t, r);
        }
        void sbcb_ix()
        {
            ushort t, r;
            t = RM(EA.LowWord);
            r = (ushort)(D.LowByte - t - (CC & CC_C));
            CLR_NZVC();
            SET_FLAGS8(D.LowByte, t, r);
            D.LowByte = (byte)r;
        }
        void addd_ix()
        {
            uint r, d;
            RegisterPair b;
            b.d = RM16(EA.LowWord);
            d = D.LowWord;
            r = d + b.d;
            CLR_NZVC();
            SET_FLAGS16((ushort)d, (ushort)b.d, r);
            D.LowWord = (ushort)r;
        }
        void andb_ix()
        {
            D.LowByte &= RM(EA.LowWord);
            CLR_NZV();
            SET_NZ8(D.LowByte);
        }
        void bitb_ix()
        {
            byte r;
            r = (byte)(D.LowByte & RM(EA.LowWord));
            CLR_NZV();
            SET_NZ8(r);
        }
        void ldb_ix()
        {
            D.LowByte = RM(EA.LowWord);
            CLR_NZV();
            SET_NZ8(D.LowByte);
        }
        void stb_ix()
        {
            CLR_NZV();
            SET_NZ8(D.LowByte);
            WM(EA.LowWord, D.LowByte);
        }
        void eorb_ix()
        {
            D.LowByte ^= RM(EA.LowWord);
            CLR_NZV();
            SET_NZ8(D.LowByte);
        }
        void adcb_ix()
        {
            ushort t, r;
            t = RM(EA.LowWord);
            r = (ushort)(D.LowByte + t + (CC & CC_C));
            CLR_HNZVC();
            SET_FLAGS8(D.LowByte, t, r);
            SET_H(D.LowByte, (byte)t, (byte)r);
            D.LowByte = (byte)r;
        }
        void orb_ix()
        {
            D.LowByte |= RM(EA.LowWord);
            CLR_NZV();
            SET_NZ8(D.LowByte);
        }
        void addb_ix()
        {
            ushort t, r;
            t = RM(EA.LowWord);
            r = (ushort)(D.LowByte + t);
            CLR_HNZVC();
            SET_FLAGS8(D.LowByte, t, r);
            SET_H(D.LowByte, (byte)t, (byte)r);
            D.LowByte = (byte)r;
        }
        void ldd_ix()
        {
            D.LowWord = RM16(EA.LowWord);
            CLR_NZV();
            SET_NZ16(D.LowWord);
        }
        void std_ix()
        {
            CLR_NZV();
            SET_NZ16(D.LowWord);
            WM16(EA.LowWord, D);
        }
        void ldu_ix()
        {
            U.LowWord = RM16(EA.LowWord);
            CLR_NZV();
            SET_NZ16(U.LowWord);
        }
        void lds_ix()
        {
            S.LowWord = RM16(EA.LowWord);
            CLR_NZV();
            SET_NZ16(S.LowWord);
            int_state |= KONAMI_LDS;
        }
        void stu_ix()
        {
            CLR_NZV();
            SET_NZ16(U.LowWord);
            WM16(EA.LowWord, U);
        }
        void sts_ix()
        {
            CLR_NZV();
            SET_NZ16(S.LowWord);
            WM16(EA.LowWord, S);
        }
        void subb_ex()
        {
            ushort t, r;
            t = EXTBYTE();
            r = (ushort)(D.LowByte - t);
            CLR_NZVC();
            SET_FLAGS8(D.LowByte, t, r);
            D.LowByte = (byte)r;
        }
        void cmpb_ex()
        {
            ushort t, r;
            t = EXTBYTE();
            r = (ushort)(D.LowByte - t);
            CLR_NZVC();
            SET_FLAGS8(D.LowByte, t, r);
        }
        void sbcb_ex()
        {
            ushort t, r;
            t = EXTBYTE();
            r = (ushort)(D.LowByte - t - (CC & CC_C));
            CLR_NZVC();
            SET_FLAGS8(D.LowByte, t, r);
            D.LowByte = (byte)r;
        }
        void addd_ex()
        {
            uint r, d;
            RegisterPair b;
            b = EXTWORD();
            d = D.LowWord;
            r = d + b.d;
            CLR_NZVC();
            SET_FLAGS16((ushort)d, (ushort)b.d, r);
            D.LowWord = (ushort)r;
        }
        void andb_ex()
        {
            byte t;
            t = EXTBYTE();
            D.LowByte &= t;
            CLR_NZV();
            SET_NZ8(D.LowByte);
        }
        void bitb_ex()
        {
            byte t, r;
            t = EXTBYTE();
            r = (byte)(D.LowByte & t);
            CLR_NZV();
            SET_NZ8(r);
        }
        void ldb_ex()
        {
            D.LowByte = EXTBYTE();
            CLR_NZV();
            SET_NZ8(D.LowByte);
        }
        void stb_ex()
        {
            CLR_NZV();
            SET_NZ8(D.LowByte);
            EXTENDED();
            WM(EA.LowWord, D.LowByte);
        }
        void eorb_ex()
        {
            byte t;
            t = EXTBYTE();
            D.LowByte ^= t;
            CLR_NZV();
            SET_NZ8(D.LowByte);
        }
        void adcb_ex()
        {
            ushort t, r;
            t = EXTBYTE();
            r = (ushort)(D.LowByte + t + (CC & CC_C));
            CLR_HNZVC();
            SET_FLAGS8(D.LowByte, t, r);
            SET_H(D.LowByte, (byte)t, (byte)r);
            D.LowByte = (byte)r;
        }
        void orb_ex()
        {
            byte t;
            t = EXTBYTE();
            D.LowByte |= t;
            CLR_NZV();
            SET_NZ8(D.LowByte);
        }
        void addb_ex()
        {
            ushort t, r;
            t = EXTBYTE();
            r = (ushort)(D.LowByte + t);
            CLR_HNZVC();
            SET_FLAGS8(D.LowByte, t, r);
            SET_H(D.LowByte, (byte)t, (byte)r);
            D.LowByte = (byte)r;
        }
        void ldd_ex()
        {
            D = EXTWORD();
            CLR_NZV();
            SET_NZ16(D.LowWord);
        }
        void std_ex()
        {
            CLR_NZV();
            SET_NZ16(D.LowWord);
            EXTENDED();
            WM16(EA.LowWord, D);
        }
        void ldu_ex()
        {
            U = EXTWORD();
            CLR_NZV();
            SET_NZ16(U.LowWord);
        }
        void lds_ex()
        {
            S = EXTWORD();
            CLR_NZV();
            SET_NZ16(S.LowWord);
            int_state |= KONAMI_LDS;
        }
        void stu_ex()
        {
            CLR_NZV();
            SET_NZ16(U.LowWord);
            EXTENDED();
            WM16(EA.LowWord, U);
        }
        void sts_ex()
        {
            CLR_NZV();
            SET_NZ16(S.LowWord);
            EXTENDED();
            WM16(EA.LowWord, S);
        }
        void setline_im()
        {
            byte t;
            t = IMMBYTE();
            if (setlines_callback != null)
            {
                setlines_callback(t);
            }
        }
        void setline_ix()
        {
            byte t;
            t = RM(EA.LowWord);
            if (setlines_callback != null)
            {
                setlines_callback(t);
            }
        }
        void setline_di()
        {
            byte t;
            t = DIRBYTE();
            if (setlines_callback != null)
            {
                setlines_callback(t);
            }
        }
        void setline_ex()
        {
            byte t;
            t = EXTBYTE();
            if (setlines_callback != null)
            {
                setlines_callback(t);
            }
        }
        void bmove()
        {
            while (U.LowWord != 0)
            {
                byte t = RM(Y.LowWord);
                WM(X.LowWord, t);
                Y.LowWord++;
                X.LowWord++;
                U.LowWord--;
                pendingCycles -= 2;
            }
        }

        void move()
        {
            byte t = RM(Y.LowWord);
            WM(X.LowWord, t);
            Y.LowWord++;
            X.LowWord++;
            U.LowWord--;
        }

        void clrd()
        {
            D.LowWord = 0;
            CLR_NZVC();
            SEZ();
        }

        void clrw_ix()
        {
            RegisterPair t = new RegisterPair();
            t.d = 0;
            WM16(EA.LowWord, t);
            CLR_NZVC();
            SEZ();
        }

        void clrw_di()
        {
            RegisterPair t = new RegisterPair();
            t.d = 0;
            DIRECT();
            WM16(EA.LowWord, t);
            CLR_NZVC();
            SEZ();
        }

        void clrw_ex()
        {
            RegisterPair t = new RegisterPair();
            t.d = 0;
            EXTENDED();
            WM16(EA.LowWord, t);
            CLR_NZVC();
            SEZ();
        }

        void lsrd()
        {
            byte t = IMMBYTE();
            while (t-- > 0)
            {
                CLR_NZC();
                CC |= (byte)(D.LowWord & CC_C);
                D.LowWord >>= 1;
                SET_Z16(D.LowWord);
            }
        }

        void rord()
        {
            ushort r;
            byte t = IMMBYTE();
            while (t-- > 0)
            {
                r = (ushort)((CC & CC_C) << 15);
                CLR_NZC();
                CC |= (byte)(D.LowWord & CC_C);
                r |= (ushort)(D.LowWord >> 1);
                SET_NZ16(r);
                D.LowWord = r;
            }
        }

        void asrd()
        {
            byte t = IMMBYTE();
            while (t-- > 0)
            {
                CLR_NZC();
                CC |= (byte)(D.LowWord & CC_C);
                D.LowWord = (ushort)((D.LowWord & 0x8000) | (D.LowWord >> 1));
                SET_NZ16(D.LowWord);
            }
        }

        void asld()
        {
            uint r;
            byte t = IMMBYTE();
            while (t-- > 0)
            {
                r = (uint)(D.LowWord << 1);
                CLR_NZVC();
                SET_FLAGS16(D.LowWord, D.LowWord, r);
                D.LowWord = (ushort)r;
            }
        }

        void rold()
        {
            ushort r;
            byte t = IMMBYTE();
            while (t-- > 0)
            {
                CLR_NZC();
                if ((D.LowWord & 0x8000) != 0) SEC();
                r = (ushort)(CC & CC_C);
                r |= (ushort)(D.LowWord << 1);
                SET_NZ16(r);
                D.LowWord = r;
            }
        }

        void decbjnz()
        {
            --D.LowByte;
            CLR_NZV();
            SET_FLAGS8D(D.LowByte);
            BRANCH((CC & CC_Z) == 0);
        }

        void decxjnz()
        {
            --X.LowWord;
            CLR_NZV();
            SET_NZ16(X.LowWord);
            BRANCH((CC & CC_Z) == 0);
        }

        void bset()
        {
            while (U.LowWord != 0)
            {
                byte t = D.HighByte;
                WM(X.LowWord, t);
                X.LowWord++;
                U.LowWord--;
                pendingCycles -= 2;
            }
        }

        void bset2()
        {
            while (U.LowWord != 0)
            {
                WM16(X.LowWord, D);
                X.LowWord += 2;
                U.LowWord--;
                pendingCycles -= 3;
            }
        }

        void lmul()
        {
            uint t = (uint)(X.LowWord * Y.LowWord);
            X.LowWord = (ushort)(t >> 16);
            Y.LowWord = (ushort)(t & 0xffff);
            CLR_ZC();
            SET_Z(t);
            if ((t & 0x8000) != 0) SEC();
        }

        void divx()
        {
            ushort t;
            byte r;
            if (D.LowByte != 0)
            {
                t = (ushort)(X.LowWord / D.LowByte);
                r = (byte)(X.LowWord % D.LowByte);
            }
            else
            {
                t = 0;
                r = 0;
            }
            CLR_ZC();
            SET_Z16(t);
            if ((t & 0x80) != 0) SEC();
            X.LowWord = t;
            D.LowByte = r;
        }

        void incd()
        {
            uint r = (uint)(D.LowWord + 1);
            CLR_NZV();
            SET_FLAGS16(D.LowWord, D.LowWord, r);
            D.LowWord = (ushort)r;
        }

        void incw_di()
        {
            RegisterPair t = DIRWORD();
            RegisterPair r = t;
            r.d++;
            CLR_NZV();
            SET_FLAGS16(t.LowWord, t.LowWord, r.LowWord);
            WM16(EA.LowWord, r);
        }

        void incw_ix()
        {
            RegisterPair t = new RegisterPair();
            t.d = RM16(EA.LowWord);
            RegisterPair r = t;
            r.d++;
            CLR_NZV();
            SET_FLAGS16(t.LowWord, t.LowWord, r.LowWord);
            WM16(EA.LowWord, r);
        }

        void incw_ex()
        {
            RegisterPair t = EXTWORD();
            RegisterPair r = t;
            r.d++;
            CLR_NZV();
            SET_FLAGS16(t.LowWord, t.LowWord, r.LowWord);
            WM16(EA.LowWord, r);
        }

        void decd()
        {
            uint r = (uint)(D.LowWord - 1);
            CLR_NZV();
            SET_FLAGS16(D.LowWord, D.LowWord, r);
            D.LowWord = (ushort)r;
        }

        void decw_di()
        {
            RegisterPair t = DIRWORD();
            RegisterPair r = t;
            r.d--;
            CLR_NZV();
            SET_FLAGS16(t.LowWord, t.LowWord, r.LowWord);
            WM16(EA.LowWord, r);
        }

        void decw_ix()
        {
            RegisterPair t = new RegisterPair();
            t.d = RM16(EA.LowWord);
            RegisterPair r = t;
            r.d--;
            CLR_NZV();
            SET_FLAGS16(t.LowWord, t.LowWord, r.LowWord);
            WM16(EA.LowWord, r);
        }

        void decw_ex()
        {
            RegisterPair t = EXTWORD();
            RegisterPair r = t;
            r.d--;
            CLR_NZV();
            SET_FLAGS16(t.LowWord, t.LowWord, r.LowWord);
            WM16(EA.LowWord, r);
        }

        void tstd()
        {
            CLR_NZV();
            SET_NZ16(D.LowWord);
        }

        void tstw_di()
        {
            CLR_NZV();
            RegisterPair t = DIRWORD();
            SET_NZ16(t.LowWord);
        }

        void tstw_ix()
        {
            CLR_NZV();
            RegisterPair t = new RegisterPair();
            t.d = RM16(EA.LowWord);
            SET_NZ16(t.LowWord);
        }

        void tstw_ex()
        {
            CLR_NZV();
            RegisterPair t = EXTWORD();
            SET_NZ16(t.LowWord);
        }

        void lsrw_di()
        {
            RegisterPair t = DIRWORD();
            CLR_NZC();
            CC |= (byte)(t.LowWord & CC_C);
            t.LowWord >>= 1;
            SET_Z16(t.LowWord);
            WM16(EA.LowWord, t);
        }

        void lsrw_ix()
        {
            RegisterPair t = new RegisterPair();
            t.d = RM16(EA.LowWord);
            CLR_NZC();
            CC |= (byte)(t.LowWord & CC_C);
            t.LowWord >>= 1;
            SET_Z16(t.LowWord);
            WM16(EA.LowWord, t);
        }

        void lsrw_ex()
        {
            RegisterPair t = EXTWORD();
            CLR_NZC();
            CC |= (byte)(t.LowWord & CC_C);
            t.LowWord >>= 1;
            SET_Z16(t.LowWord);
            WM16(EA.LowWord, t);
        }

        void rorw_di()
        {
            RegisterPair t = DIRWORD();
            RegisterPair r = new RegisterPair();
            r.d = (ushort)((CC & CC_C) << 15);
            CLR_NZC();
            CC |= (byte)(t.LowWord & CC_C);
            r.d |= (ushort)(t.LowWord >> 1);
            SET_NZ16(r.LowWord);
            WM16(EA.LowWord, r);
        }

        void rorw_ix()
        {
            RegisterPair t = new RegisterPair();
            t.d = RM16(EA.LowWord);
            RegisterPair r = new RegisterPair();
            r.d = (ushort)((CC & CC_C) << 15);
            CLR_NZC();
            CC |= (byte)(t.LowWord & CC_C);
            r.d |= (ushort)(t.LowWord >> 1);
            SET_NZ16(r.LowWord);
            WM16(EA.LowWord, r);
        }

        void rorw_ex()
        {
            RegisterPair t = EXTWORD();
            RegisterPair r = new RegisterPair();
            r.d = (ushort)((CC & CC_C) << 15);
            CLR_NZC();
            CC |= (byte)(t.LowWord & CC_C);
            r.d |= (ushort)(t.LowWord >> 1);
            SET_NZ16(r.LowWord);
            WM16(EA.LowWord, r);
        }

        void asrw_di()
        {
            RegisterPair t = DIRWORD();
            CLR_NZC();
            CC |= (byte)(t.LowWord & CC_C);
            t.LowWord = (ushort)((t.LowWord & 0x8000) | (t.LowWord >> 1));
            SET_NZ16(t.LowWord);
            WM16(EA.LowWord, t);
        }

        void asrw_ix()
        {
            RegisterPair t = new RegisterPair();
            t.d = RM16(EA.LowWord);
            CLR_NZC();
            CC |= (byte)(t.LowWord & CC_C);
            t.LowWord = (ushort)((t.LowWord & 0x8000) | (t.LowWord >> 1));
            SET_NZ16(t.LowWord);
            WM16(EA.LowWord, t);
        }

        void asrw_ex()
        {
            RegisterPair t = EXTWORD();
            CLR_NZC();
            CC |= (byte)(t.LowWord & CC_C);
            t.LowWord = (ushort)((t.LowWord & 0x8000) | (t.LowWord >> 1));
            SET_NZ16(t.LowWord);
            WM16(EA.LowWord, t);
        }

        void aslw_di()
        {
            RegisterPair t = DIRWORD();
            RegisterPair r = new RegisterPair();
            r.d = (ushort)(t.LowWord << 1);
            CLR_NZVC();
            SET_FLAGS16(t.LowWord, t.LowWord, r.LowWord);
            WM16(EA.LowWord, r);
        }

        void aslw_ix()
        {
            RegisterPair t = new RegisterPair();
            t.d = RM16(EA.LowWord);
            RegisterPair r = new RegisterPair();
            r.d = (ushort)(t.LowWord << 1);
            CLR_NZVC();
            SET_FLAGS16(t.LowWord, t.LowWord, r.LowWord);
            WM16(EA.LowWord, r);
        }

        void aslw_ex()
        {
            RegisterPair t = EXTWORD();
            RegisterPair r = new RegisterPair();
            r.d = (ushort)(t.LowWord << 1);
            CLR_NZVC();
            SET_FLAGS16(t.LowWord, t.LowWord, r.LowWord);
            WM16(EA.LowWord, r);
        }

        void rolw_di()
        {
            RegisterPair t = DIRWORD();
            RegisterPair r = new RegisterPair();
            r.d = (ushort)((CC & CC_C) | (t.LowWord << 1));
            CLR_NZVC();
            SET_FLAGS16(t.LowWord, t.LowWord, r.LowWord);
            WM16(EA.LowWord, r);
        }

        void rolw_ix()
        {
            RegisterPair t = new RegisterPair();
            t.d = RM16(EA.LowWord);
            RegisterPair r = new RegisterPair();
            r.d = (ushort)((CC & CC_C) | (t.LowWord << 1));
            CLR_NZVC();
            SET_FLAGS16(t.LowWord, t.LowWord, r.LowWord);
            WM16(EA.LowWord, r);
        }

        void rolw_ex()
        {
            RegisterPair t = EXTWORD();
            RegisterPair r = new RegisterPair();
            r.d = (ushort)((CC & CC_C) | (t.LowWord << 1));
            CLR_NZVC();
            SET_FLAGS16(t.LowWord, t.LowWord, r.LowWord);
            WM16(EA.LowWord, r);
        }

        void negd()
        {
            uint r = (uint)(-D.LowWord);
            CLR_NZVC();
            SET_FLAGS16(0, D.LowWord, r);
            D.LowWord = (ushort)r;
        }

        void negw_di()
        {
            RegisterPair t = DIRWORD();
            RegisterPair r = new RegisterPair();
            r.d = (ushort)(-t.LowWord);
            CLR_NZVC();
            SET_FLAGS16(0, t.LowWord, r.LowWord);
            WM16(EA.LowWord, r);
        }

        void negw_ix()
        {
            RegisterPair t = new RegisterPair();
            t.d = RM16(EA.LowWord);
            RegisterPair r = new RegisterPair();
            r.d = (ushort)(-t.LowWord);
            CLR_NZVC();
            SET_FLAGS16(0, t.LowWord, r.LowWord);
            WM16(EA.LowWord, r);
        }

        void negw_ex()
        {
            RegisterPair t = EXTWORD();
            RegisterPair r = new RegisterPair();
            r.d = (ushort)(-t.LowWord);
            CLR_NZVC();
            SET_FLAGS16(0, t.LowWord, r.LowWord);
            WM16(EA.LowWord, r);
        }

        void absa()
        {
            ushort r;
            if ((D.HighByte & 0x80) != 0)
                r = (ushort)(-D.HighByte);
            else
                r = D.HighByte;
            CLR_NZVC();
            SET_FLAGS8(0, D.HighByte, r);
            D.HighByte = (byte)r;
        }

        void absb()
        {
            ushort r;
            if ((D.LowByte & 0x80) != 0)
                r = (ushort)(-D.LowByte);
            else
                r = D.LowByte;
            CLR_NZVC();
            SET_FLAGS8(0, D.LowByte, r);
            D.LowByte = (byte)r;
        }

        void absd()
        {
            uint r;
            if ((D.LowWord & 0x8000) != 0)
                r = (uint)(-D.LowWord);
            else
                r = D.LowWord;
            CLR_NZVC();
            SET_FLAGS16(0, D.LowWord, r);
            D.LowWord = (ushort)r;
        }

        void lsrd_di()
        {
            byte t = DIRBYTE();
            while (t-- > 0)
            {
                CLR_NZC();
                CC |= (byte)(D.LowWord & CC_C);
                D.LowWord >>= 1;
                SET_Z16(D.LowWord);
            }
        }

        void rord_di()
        {
            ushort r;
            byte t = DIRBYTE();
            while (t-- > 0)
            {
                r = (ushort)((CC & CC_C) << 15);
                CLR_NZC();
                CC |= (byte)(D.LowWord & CC_C);
                r |= (ushort)(D.LowWord >> 1);
                SET_NZ16(r);
                D.LowWord = r;
            }
        }

        void asrd_di()
        {
            byte t = DIRBYTE();
            while (t-- > 0)
            {
                CLR_NZC();
                CC |= (byte)(D.LowWord & CC_C);
                D.LowWord = (ushort)((D.LowWord & 0x8000) | (D.LowWord >> 1));
                SET_NZ16(D.LowWord);
            }
        }

        void asld_di()
        {
            uint r;
            byte t = DIRBYTE();
            while (t-- > 0)
            {
                r = (uint)(D.LowWord << 1);
                CLR_NZVC();
                SET_FLAGS16(D.LowWord, D.LowWord, r);
                D.LowWord = (ushort)r;
            }
        }

        void rold_di()
        {
            ushort r;
            byte t = DIRBYTE();
            while (t-- > 0)
            {
                CLR_NZC();
                if ((D.LowWord & 0x8000) != 0) SEC();
                r = (ushort)(CC & CC_C);
                r |= (ushort)(D.LowWord << 1);
                SET_NZ16(r);
                D.LowWord = r;
            }
        }

        void lsrd_ix()
        {
            byte t = RM(EA.LowWord);
            while (t-- > 0)
            {
                CLR_NZC();
                CC |= (byte)(D.LowWord & CC_C);
                D.LowWord >>= 1;
                SET_Z16(D.LowWord);
            }
        }

        void rord_ix()
        {
            ushort r;
            byte t = RM(EA.LowWord);
            while (t-- > 0)
            {
                r = (ushort)((CC & CC_C) << 15);
                CLR_NZC();
                CC |= (byte)(D.LowWord & CC_C);
                r |= (ushort)(D.LowWord >> 1);
                SET_NZ16(r);
                D.LowWord = r;
            }
        }

        void asrd_ix()
        {
            byte t = RM(EA.LowWord);
            while (t-- > 0)
            {
                CLR_NZC();
                CC |= (byte)(D.LowWord & CC_C);
                D.LowWord = (ushort)((D.LowWord & 0x8000) | (D.LowWord >> 1));
                SET_NZ16(D.LowWord);
            }
        }

        void asld_ix()
        {
            uint r;
            byte t = RM(EA.LowWord);
            while (t-- > 0)
            {
                r = (uint)(D.LowWord << 1);
                CLR_NZVC();
                SET_FLAGS16(D.LowWord, D.LowWord, r);
                D.LowWord = (ushort)r;
            }
        }

        void rold_ix()
        {
            ushort r;
            byte t = RM(EA.LowWord);
            while (t-- > 0)
            {
                CLR_NZC();
                if ((D.LowWord & 0x8000) != 0) SEC();
                r = (ushort)(CC & CC_C);
                r |= (ushort)(D.LowWord << 1);
                SET_NZ16(r);
                D.LowWord = r;
            }
        }

        void lsrd_ex()
        {
            byte t = EXTBYTE();
            while (t-- > 0)
            {
                CLR_NZC();
                CC |= (byte)(D.LowWord & CC_C);
                D.LowWord >>= 1;
                SET_Z16(D.LowWord);
            }
        }

        void rord_ex()
        {
            ushort r;
            byte t = EXTBYTE();
            while (t-- > 0)
            {
                r = (ushort)((CC & CC_C) << 15);
                CLR_NZC();
                CC |= (byte)(D.LowWord & CC_C);
                r |= (ushort)(D.LowWord >> 1);
                SET_NZ16(r);
                D.LowWord = r;
            }
        }

        void asrd_ex()
        {
            byte t = EXTBYTE();
            while (t-- > 0)
            {
                CLR_NZC();
                CC |= (byte)(D.LowWord & CC_C);
                D.LowWord = (ushort)((D.LowWord & 0x8000) | (D.LowWord >> 1));
                SET_NZ16(D.LowWord);
            }
        }

        void asld_ex()
        {
            uint r;
            byte t = EXTBYTE();
            while (t-- > 0)
            {
                r = (uint)(D.LowWord << 1);
                CLR_NZVC();
                SET_FLAGS16(D.LowWord, D.LowWord, r);
                D.LowWord = (ushort)r;
            }
        }

        void rold_ex()
        {
            ushort r;
            byte t = EXTBYTE();
            while (t-- > 0)
            {
                CLR_NZC();
                if ((D.LowWord & 0x8000) != 0) SEC();
                r = (ushort)(CC & CC_C);
                r |= (ushort)(D.LowWord << 1);
                SET_NZ16(r);
                D.LowWord = r;
            }
        }

        void opcode2()
        {
            byte ireg2 = ROP_ARG(PC.LowWord);
            PC.LowWord++;

            switch (ireg2)
            {
                case 0x07:
                    EA.d = 0;
                    if (konami_extended[ireg] != null)
                        konami_extended[ireg]();
                    pendingCycles -= 2;
                    return;
                case 0x0f:
                    EA = IMMWORD();
                    EA.d = RM16(EA.LowWord);
                    pendingCycles -= 4;
                    break;
                case 0x20:
                    EA.d = X.LowWord;
                    X.LowWord++;
                    pendingCycles -= 2;
                    break;
                case 0x21:
                    EA.d = X.LowWord;
                    X.LowWord += 2;
                    pendingCycles -= 3;
                    break;
                case 0x22:
                    X.LowWord--;
                    EA.d = X.LowWord;
                    pendingCycles -= 2;
                    break;
                case 0x23:
                    X.LowWord -= 2;
                    EA.d = X.LowWord;
                    pendingCycles -= 3;
                    break;
                case 0x24:
                    byte temp = IMMBYTE();
                    EA.d = (ushort)(X.LowWord + SIGNED(temp));
                    pendingCycles -= 2;
                    break;
                case 0x25:
                    EA = IMMWORD();
                    EA.d += X.LowWord;
                    pendingCycles -= 4;
                    break;
                case 0x26:
                    EA.d = X.LowWord;
                    break;
                case 0x28:
                    EA.d = X.LowWord;
                    X.LowWord++;
                    EA.d = RM16(EA.LowWord);
                    pendingCycles -= 5;
                    break;
                case 0x29:
                    EA.d = X.LowWord;
                    X.LowWord += 2;
                    EA.d = RM16(EA.LowWord);
                    pendingCycles -= 6;
                    break;
                case 0x2a:
                    X.LowWord--;
                    EA.d = X.LowWord;
                    EA.d = RM16(EA.LowWord);
                    pendingCycles -= 5;
                    break;
                case 0x2b:
                    X.LowWord -= 2;
                    EA.d = X.LowWord;
                    EA.d = RM16(EA.LowWord);
                    pendingCycles -= 6;
                    break;
                case 0x2c:
                    temp = IMMBYTE();
                    EA.d = (ushort)(X.LowWord + SIGNED(temp));
                    EA.d = RM16(EA.LowWord);
                    pendingCycles -= 4;
                    break;
                case 0x2d:
                    EA = IMMWORD();
                    EA.d += X.LowWord;
                    EA.d = RM16(EA.LowWord);
                    pendingCycles -= 7;
                    break;
                case 0x2e:
                    EA.d = X.LowWord;
                    EA.d = RM16(EA.LowWord);
                    pendingCycles -= 3;
                    break;
                case 0x30:
                    EA.d = Y.LowWord;
                    Y.LowWord++;
                    pendingCycles -= 2;
                    break;
                case 0x31:
                    EA.d = Y.LowWord;
                    Y.LowWord += 2;
                    pendingCycles -= 3;
                    break;
                case 0x32:
                    Y.LowWord--;
                    EA.d = Y.LowWord;
                    pendingCycles -= 2;
                    break;
                case 0x33:
                    Y.LowWord -= 2;
                    EA.d = Y.LowWord;
                    pendingCycles -= 3;
                    break;
                case 0x34:
                    temp = IMMBYTE();
                    EA.d = (ushort)(Y.LowWord + SIGNED(temp));
                    pendingCycles -= 2;
                    break;
                case 0x35:
                    EA = IMMWORD();
                    EA.d += Y.LowWord;
                    pendingCycles -= 4;
                    break;
                case 0x36:
                    EA.d = Y.LowWord;
                    break;
                case 0x38:
                    EA.d = Y.LowWord;
                    Y.LowWord++;
                    EA.d = RM16(EA.LowWord);
                    pendingCycles -= 5;
                    break;
                case 0x39:
                    EA.d = Y.LowWord;
                    Y.LowWord += 2;
                    EA.d = RM16(EA.LowWord);
                    pendingCycles -= 6;
                    break;
                case 0x3a:
                    Y.LowWord--;
                    EA.d = Y.LowWord;
                    EA.d = RM16(EA.LowWord);
                    pendingCycles -= 5;
                    break;
                case 0x3b:
                    Y.LowWord -= 2;
                    EA.d = Y.LowWord;
                    EA.d = RM16(EA.LowWord);
                    pendingCycles -= 6;
                    break;
                case 0x3c:
                    temp = IMMBYTE();
                    EA.d = (ushort)(Y.LowWord + SIGNED(temp));
                    EA.d = RM16(EA.LowWord);
                    pendingCycles -= 4;
                    break;
                case 0x3d:
                    EA = IMMWORD();
                    EA.d += Y.LowWord;
                    EA.d = RM16(EA.LowWord);
                    pendingCycles -= 7;
                    break;
                case 0x3e:
                    EA.d = Y.LowWord;
                    EA.d = RM16(EA.LowWord);
                    pendingCycles -= 3;
                    break;
                case 0x50:
                    EA.d = U.LowWord;
                    U.LowWord++;
                    pendingCycles -= 2;
                    break;
                case 0x51:
                    EA.d = U.LowWord;
                    U.LowWord += 2;
                    pendingCycles -= 3;
                    break;
                case 0x52:
                    U.LowWord--;
                    EA.d = U.LowWord;
                    pendingCycles -= 2;
                    break;
                case 0x53:
                    U.LowWord -= 2;
                    EA.d = U.LowWord;
                    pendingCycles -= 3;
                    break;
                case 0x54:
                    temp = IMMBYTE();
                    EA.d = (ushort)(U.LowWord + SIGNED(temp));
                    pendingCycles -= 2;
                    break;
                case 0x55:
                    EA = IMMWORD();
                    EA.d += U.LowWord;
                    pendingCycles -= 4;
                    break;
                case 0x56:
                    EA.d = U.LowWord;
                    break;
                case 0x58:
                    EA.d = U.LowWord;
                    U.LowWord++;
                    EA.d = RM16(EA.LowWord);
                    pendingCycles -= 5;
                    break;
                case 0x59:
                    EA.d = U.LowWord;
                    U.LowWord += 2;
                    EA.d = RM16(EA.LowWord);
                    pendingCycles -= 6;
                    break;
                case 0x5a:
                    U.LowWord--;
                    EA.d = U.LowWord;
                    EA.d = RM16(EA.LowWord);
                    pendingCycles -= 5;
                    break;
                case 0x5b:
                    U.LowWord -= 2;
                    EA.d = U.LowWord;
                    EA.d = RM16(EA.LowWord);
                    pendingCycles -= 6;
                    break;
                case 0x5c:
                    temp = IMMBYTE();
                    EA.d = (ushort)(U.LowWord + SIGNED(temp));
                    EA.d = RM16(EA.LowWord);
                    pendingCycles -= 4;
                    break;
                case 0x5d:
                    EA = IMMWORD();
                    EA.d += U.LowWord;
                    EA.d = RM16(EA.LowWord);
                    pendingCycles -= 7;
                    break;
                case 0x5e:
                    EA.d = U.LowWord;
                    EA.d = RM16(EA.LowWord);
                    pendingCycles -= 3;
                    break;
                case 0x60:
                    EA.d = S.LowWord;
                    S.LowWord++;
                    pendingCycles -= 2;
                    break;
                case 0x61:
                    EA.d = S.LowWord;
                    S.LowWord += 2;
                    pendingCycles -= 3;
                    break;
                case 0x62:
                    S.LowWord--;
                    EA.d = S.LowWord;
                    pendingCycles -= 2;
                    break;
                case 0x63:
                    S.LowWord -= 2;
                    EA.d = S.LowWord;
                    pendingCycles -= 3;
                    break;
                case 0x64:
                    temp = IMMBYTE();
                    EA.d = (ushort)(S.LowWord + SIGNED(temp));
                    pendingCycles -= 2;
                    break;
                case 0x65:
                    EA = IMMWORD();
                    EA.d += S.LowWord;
                    pendingCycles -= 4;
                    break;
                case 0x66:
                    EA.d = S.LowWord;
                    break;
                case 0x68:
                    EA.d = S.LowWord;
                    S.LowWord++;
                    EA.d = RM16(EA.LowWord);
                    pendingCycles -= 5;
                    break;
                case 0x69:
                    EA.d = S.LowWord;
                    S.LowWord += 2;
                    EA.d = RM16(EA.LowWord);
                    pendingCycles -= 6;
                    break;
                case 0x6a:
                    S.LowWord--;
                    EA.d = S.LowWord;
                    EA.d = RM16(EA.LowWord);
                    pendingCycles -= 5;
                    break;
                case 0x6b:
                    S.LowWord -= 2;
                    EA.d = S.LowWord;
                    EA.d = RM16(EA.LowWord);
                    pendingCycles -= 6;
                    break;
                case 0x6c:
                    temp = IMMBYTE();
                    EA.d = (ushort)(S.LowWord + SIGNED(temp));
                    EA.d = RM16(EA.LowWord);
                    pendingCycles -= 4;
                    break;
                case 0x6d:
                    EA = IMMWORD();
                    EA.d += S.LowWord;
                    EA.d = RM16(EA.LowWord);
                    pendingCycles -= 7;
                    break;
                case 0x6e:
                    EA.d = S.LowWord;
                    EA.d = RM16(EA.LowWord);
                    pendingCycles -= 3;
                    break;
                case 0x70:
                    EA.d = PC.LowWord;
                    PC.LowWord++;
                    pendingCycles -= 2;
                    break;
                case 0x71:
                    EA.d = PC.LowWord;
                    PC.LowWord += 2;
                    pendingCycles -= 3;
                    break;
                case 0x72:
                    PC.LowWord--;
                    EA.d = PC.LowWord;
                    pendingCycles -= 2;
                    break;
                case 0x73:
                    PC.LowWord -= 2;
                    EA.d = PC.LowWord;
                    pendingCycles -= 3;
                    break;
                case 0x74:
                    temp = IMMBYTE();
                    EA.d = (ushort)(PC.LowWord - 1 + SIGNED(temp));
                    pendingCycles -= 2;
                    break;
                case 0x75:
                    EA = IMMWORD();
                    EA.d += (ushort)(PC.LowWord - 2);
                    pendingCycles -= 4;
                    break;
                case 0x76:
                    EA.d = PC.LowWord;
                    break;
                case 0x78:
                    EA.d = PC.LowWord;
                    PC.LowWord++;
                    EA.d = RM16(EA.LowWord);
                    pendingCycles -= 5;
                    break;
                case 0x79:
                    EA.d = PC.LowWord;
                    PC.LowWord += 2;
                    EA.d = RM16(EA.LowWord);
                    pendingCycles -= 6;
                    break;
                case 0x7a:
                    PC.LowWord--;
                    EA.d = PC.LowWord;
                    EA.d = RM16(EA.LowWord);
                    pendingCycles -= 5;
                    break;
                case 0x7b:
                    PC.LowWord -= 2;
                    EA.d = PC.LowWord;
                    EA.d = RM16(EA.LowWord);
                    pendingCycles -= 6;
                    break;
                case 0x7c:
                    temp = IMMBYTE();
                    EA.d = (ushort)(PC.LowWord - 1 + SIGNED(temp));
                    EA.d = RM16(EA.LowWord);
                    pendingCycles -= 4;
                    break;
                case 0x7d:
                    EA = IMMWORD();
                    EA.d += (ushort)(PC.LowWord - 2);
                    EA.d = RM16(EA.LowWord);
                    pendingCycles -= 7;
                    break;
                case 0x7e:
                    EA.d = PC.LowWord;
                    EA.d = RM16(EA.LowWord);
                    pendingCycles -= 3;
                    break;
                case 0xa0:
                    EA.d = (ushort)(X.LowWord + SIGNED(D.HighByte));
                    pendingCycles -= 1;
                    break;
                case 0xa1:
                    EA.d = (ushort)(X.LowWord + SIGNED(D.LowByte));
                    pendingCycles -= 1;
                    break;
                case 0xa7:
                    EA.d = (ushort)(X.LowWord + D.LowWord);
                    pendingCycles -= 4;
                    break;
                case 0xa8:
                    EA.d = (ushort)(X.LowWord + SIGNED(D.HighByte));
                    EA.d = RM16(EA.LowWord);
                    pendingCycles -= 4;
                    break;
                case 0xa9:
                    EA.d = (ushort)(X.LowWord + SIGNED(D.LowByte));
                    EA.d = RM16(EA.LowWord);
                    pendingCycles -= 4;
                    break;
                case 0xaf:
                    EA.d = (ushort)(X.LowWord + D.LowWord);
                    EA.d = RM16(EA.LowWord);
                    pendingCycles -= 7;
                    break;
                case 0xb0:
                    EA.d = (ushort)(Y.LowWord + SIGNED(D.HighByte));
                    pendingCycles -= 1;
                    break;
                case 0xb1:
                    EA.d = (ushort)(Y.LowWord + SIGNED(D.LowByte));
                    pendingCycles -= 1;
                    break;
                case 0xb7:
                    EA.d = (ushort)(Y.LowWord + D.LowWord);
                    pendingCycles -= 4;
                    break;
                case 0xb8:
                    EA.d = (ushort)(Y.LowWord + SIGNED(D.HighByte));
                    EA.d = RM16(EA.LowWord);
                    pendingCycles -= 4;
                    break;
                case 0xb9:
                    EA.d = (ushort)(Y.LowWord + SIGNED(D.LowByte));
                    EA.d = RM16(EA.LowWord);
                    pendingCycles -= 4;
                    break;
                case 0xbf:
                    EA.d = (ushort)(Y.LowWord + D.LowWord);
                    EA.d = RM16(EA.LowWord);
                    pendingCycles -= 7;
                    break;
                case 0xc4:
                    EA.d = 0;
                    if (konami_direct[ireg] != null)
                        konami_direct[ireg]();
                    pendingCycles -= 1;
                    return;
                case 0xcc:
                    EA = DIRWORD();
                    pendingCycles -= 4;
                    break;
                case 0xd0:
                    EA.d = (ushort)(U.LowWord + SIGNED(D.HighByte));
                    pendingCycles -= 1;
                    break;
                case 0xd1:
                    EA.d = (ushort)(U.LowWord + SIGNED(D.LowByte));
                    pendingCycles -= 1;
                    break;
                case 0xd7:
                    EA.d = (ushort)(U.LowWord + D.LowWord);
                    pendingCycles -= 4;
                    break;
                case 0xd8:
                    EA.d = (ushort)(U.LowWord + SIGNED(D.HighByte));
                    EA.d = RM16(EA.LowWord);
                    pendingCycles -= 4;
                    break;
                case 0xd9:
                    EA.d = (ushort)(U.LowWord + SIGNED(D.LowByte));
                    EA.d = RM16(EA.LowWord);
                    pendingCycles -= 4;
                    break;
                case 0xdf:
                    EA.d = (ushort)(U.LowWord + D.LowWord);
                    EA.d = RM16(EA.LowWord);
                    pendingCycles -= 7;
                    break;
                case 0xe0:
                    EA.d = (ushort)(S.LowWord + SIGNED(D.HighByte));
                    pendingCycles -= 1;
                    break;
                case 0xe1:
                    EA.d = (ushort)(S.LowWord + SIGNED(D.LowByte));
                    pendingCycles -= 1;
                    break;
                case 0xe7:
                    EA.d = (ushort)(S.LowWord + D.LowWord);
                    pendingCycles -= 4;
                    break;
                case 0xe8:
                    EA.d = (ushort)(S.LowWord + SIGNED(D.HighByte));
                    EA.d = RM16(EA.LowWord);
                    pendingCycles -= 4;
                    break;
                case 0xe9:
                    EA.d = (ushort)(S.LowWord + SIGNED(D.LowByte));
                    EA.d = RM16(EA.LowWord);
                    pendingCycles -= 4;
                    break;
                case 0xef:
                    EA.d = (ushort)(S.LowWord + D.LowWord);
                    EA.d = RM16(EA.LowWord);
                    pendingCycles -= 7;
                    break;
                case 0xf0:
                    EA.d = (ushort)(PC.LowWord + SIGNED(D.HighByte));
                    pendingCycles -= 1;
                    break;
                case 0xf1:
                    EA.d = (ushort)(PC.LowWord + SIGNED(D.LowByte));
                    pendingCycles -= 1;
                    break;
                case 0xf7:
                    EA.d = (ushort)(PC.LowWord + D.LowWord);
                    pendingCycles -= 4;
                    break;
                case 0xf8:
                    EA.d = (ushort)(PC.LowWord + SIGNED(D.HighByte));
                    EA.d = RM16(EA.LowWord);
                    pendingCycles -= 4;
                    break;
                case 0xf9:
                    EA.d = (ushort)(PC.LowWord + SIGNED(D.LowByte));
                    EA.d = RM16(EA.LowWord);
                    pendingCycles -= 4;
                    break;
                case 0xff:
                    EA.d = (ushort)(PC.LowWord + D.LowWord);
                    EA.d = RM16(EA.LowWord);
                    pendingCycles -= 7;
                    break;
                default:
                    EA.d = 0;
                    break;
            }
            if (konami_indexed[ireg] != null)
                konami_indexed[ireg]();
        }

    }
}
