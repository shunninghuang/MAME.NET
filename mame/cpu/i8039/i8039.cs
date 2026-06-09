using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mame;

namespace cpu.i8039
{
    public partial class I8039 : cpuexec_data
    {
        public const int I8039_p0 = 0x100;
        public const int I8039_p1 = 0x101;
        public const int I8039_p2 = 0x102;
        public const int I8039_p4 = 0x104;
        public const int I8039_p5 = 0x105;
        public const int I8039_p6 = 0x106;
        public const int I8039_p7 = 0x107;
        public const int I8039_t0 = 0x110;
        public const int I8039_t1 = 0x111;
        public const int I8039_ea = 0x112;
        public const int I8039_bus = 0x120;
        private const int I8039_NO_INT = 0;
        private const int I8039_EXTERNAL_INT = 1;
        private const int I8039_TIMCNT_INT = 2;
        private const byte FEATURE_M58715 = 1;
        private const byte C_FLAG = 0x80;
        private const byte A_FLAG = 0x40;
        private const byte F_FLAG = 0x20;
        private const byte B_FLAG = 0x10;
        public struct I8039_Regs
        {
            public RegisterPair PREVPC;
            public RegisterPair PC;
            public byte A, SP, PSW;
            public byte[] RAM;
            public byte bus, f1;
            public byte P1, P2;
            public byte EA;
            public byte cpu_feature;
            public byte ram_mask;
            public ushort int_rom_size;
            public byte pending_irq, irq_executing, masterClock, regPtr;
            public byte t_flag, timer, timerON, countON, xirq_en, tirq_en;
            public ushort A11;
            public byte irq_state, irq_extra_cycles;
            public int inst_cycles;
            public byte Old_T1;
        }
        public static I8039_Regs R = new I8039_Regs();
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
        public override void cpunum_set_input_line_and_vector(int cpunum, int line, LineState state, int vector)
        {
            Timer.timer_set_internal(Cpuint.cpunum_empty_event_queue, "cpunum_empty_event_queue");
        }
        public Func<ushort, byte> ReadMemory;
        public Action<ushort, byte> WriteMemory;
        public Func<ushort, byte> ReadOp;
        public Func<ushort, byte> ReadOpArg;
        public Func<int, byte> ReadIO;
        public Action<int, byte> WriteIO;
        public Func<int, int> IrqCallback;
        private byte port_r(byte A)
        {
            return ReadIO(I8039_p0 + A);
        }
        private void port_w(byte A,byte V)
        {
            WriteIO(I8039_p0 + A,V);
        }
        private byte test_r(byte A)
        {
            return ReadIO(I8039_t0 + A);
        }
        private void test_w(byte A,byte V)
        {
            WriteIO(I8039_t0 + A, V);
        }
        private byte bus_r()
        {
            return ReadIO(I8039_bus);
        }
        private void bus_w(byte V)
        {
            WriteIO(I8039_bus, V);
        }
        private byte INTRAM_R(int addr)
        {
            return R.RAM[addr & R.ram_mask];
        }
        private void INTRAM_W(int addr, byte value)
        {
            R.RAM[addr & R.ram_mask] = value;
        }
        private byte R0()
        {
            return R.RAM[R.regPtr];
        }
        private byte R1()
        {
            return R.RAM[R.regPtr + 1];
        }
        private byte R2()
        {
            return R.RAM[R.regPtr + 2];
        }
        private byte R3()
        {
            return R.RAM[R.regPtr + 3];
        }
        private byte R4()
        {
            return R.RAM[R.regPtr + 4];
        }
        private byte R5()
        {
            return R.RAM[R.regPtr + 5];
        }
        private byte R6()
        {
            return R.RAM[R.regPtr + 6];
        }
        private byte R7()
        {
            return R.RAM[R.regPtr + 7];
        }
        private bool M_Cy = ((R.PSW & C_FLAG) >>7)!=0;
        private bool M_Ay = (R.PSW & A_FLAG) != 0;
        private bool M_F0y = (R.PSW & F_FLAG) != 0;
        private bool M_By = (R.PSW & B_FLAG) != 0;
        private byte ea_r()
        {
            R.EA = ReadIO(I8039_ea);
            return R.EA;
        }
        private byte M_RDOP(ushort A)
        {
            if ((R.cpu_feature & FEATURE_M58715) != 0)
            {
                if ((A < R.int_rom_size) && (ea_r() == 0))
                {
                    return 0x00;
                }
            }
            return ReadOp(A);
        }
        private byte M_RDOP_ARG(ushort A)
        {
            if ((R.cpu_feature & FEATURE_M58715) != 0)
            {
                if ((A < R.int_rom_size) && (ea_r() == 0))
                {
                    return 0x00;
                }
            }
            return ReadOpArg(A);
        }
        private void CLR(byte flag)
        {
            R.PSW &= (byte)~flag;
        }
        private void SET(byte flag)
        {
            R.PSW |= flag;
        }
        private byte M_RDMEM_OPCODE()
        {
            byte retval = M_RDOP_ARG(R.PC.LowWord);
            R.PC.LowWord++;
            return retval;
        }
        private void push(byte d)
        {
            R.RAM[8 + R.SP++] = d;
            R.SP &= 0x0f;
            R.PSW &= 0xf8;
            R.PSW |= (byte)(R.SP >> 1);
        }
        private byte pull()
        {
            R.SP = (byte)((R.SP + 15) & 0x0f); // 相当于先减1，并处理下溢
            R.PSW &= 0xf8;
            R.PSW |= (byte)(R.SP >> 1);
            return R.RAM[8 + R.SP];
        }
        private void daa_a()
        {
            if ((R.A & 0x0f) > 0x09 || (R.PSW & A_FLAG) != 0)
            {
                R.A += 0x06;
                if ((R.A & 0xf0) == 0)
                {
                    SET(C_FLAG);
                }
            }
            if ((R.A & 0xf0) > 0x90 || (R.PSW & C_FLAG) != 0)
            {
                R.A += 0x60;
                SET(C_FLAG);
            }
            else
            {
                CLR(C_FLAG);
            }
        }
        private void M_ADD(byte dat)
        {
            CLR((byte)(C_FLAG | A_FLAG));
            if (((R.A & 0xf) + (dat & 0xf)) > 0xf)
            {
                SET(A_FLAG);
            }
            ushort temp = (ushort)(R.A + dat);
            if (temp > 0xff)
            {
                SET(C_FLAG);
            }
            R.A = (byte)temp;
        }
        private void M_ADDC(byte dat)
        {
            CLR(A_FLAG);
            byte carry = (byte)(M_Cy ? 1 : 0);
            if (((R.A & 0xf) + (dat & 0xf) + carry) > 0xf)
            {
                SET(A_FLAG);
            }
            ushort temp = (ushort)(R.A + dat + carry);
            CLR(C_FLAG);
            if (temp > 0xff)
            {
                SET(C_FLAG);
            }
            R.A = (byte)temp;
        }
        private void M_CALL(ushort addr)
        {
            push((byte)R.PC.LowByte); // 推入低字节
            push((byte)((R.PC.HighByte & 0x0f) | (R.PSW & 0xf0))); // 高4位 + PSW高4位
            R.PC.LowWord = addr;
        }
        private void M_XCHD(int addr)
        {
            byte dat = (byte)(R.A & 0x0f);
            byte val = INTRAM_R(addr);
            R.A &= 0xf0;
            R.A |= (byte)(val & 0x0f);
            val &= 0xf0;
            val |= dat;
            INTRAM_W(addr, val);
        }
        private void M_ILLEGAL()
        {
            
        }
        private void M_UNDEFINED()
        {
            
        }
        private void illegal() { M_ILLEGAL(); }
        private void add_a_n() { M_ADD(M_RDMEM_OPCODE()); }
        private void add_a_r0() { M_ADD(R0()); }
        private void add_a_r1()	{ M_ADD(R1()); }
        private void add_a_r2() { M_ADD(R2()); }
        private void add_a_r3() { M_ADD(R3()); }
        private void add_a_r4() { M_ADD(R4()); }
        private void add_a_r5() { M_ADD(R5()); }
        private void add_a_r6() { M_ADD(R6()); }
        private void add_a_r7() { M_ADD(R7()); }
        private void add_a_xr0() { M_ADD(INTRAM_R(R0())); }
        private void add_a_xr1() { M_ADD(INTRAM_R(R1())); }
        private void adc_a_n() { M_ADDC(M_RDMEM_OPCODE()); }
        private void adc_a_r0() { M_ADDC(R0()); }
        private void adc_a_r1() { M_ADDC(R1()); }
        private void adc_a_r2() { M_ADDC(R2()); }
        private void adc_a_r3() { M_ADDC(R3()); }
        private void adc_a_r4() { M_ADDC(R4()); }
        private void adc_a_r5() { M_ADDC(R5()); }
        private void adc_a_r6() { M_ADDC(R6()); }
        private void adc_a_r7() { M_ADDC(R7()); }
        private void adc_a_xr0() { M_ADDC(INTRAM_R(R0())); }
        private void adc_a_xr1() { M_ADDC(INTRAM_R(R1())); }
        private void anl_a_n() { R.A &= M_RDMEM_OPCODE(); }
        private void anl_a_r0() { R.A &= R0(); }
        private void anl_a_r1() { R.A &= R1(); }
        private void anl_a_r2() { R.A &= R2(); }
        private void anl_a_r3() { R.A &= R3(); }
        private void anl_a_r4() { R.A &= R4(); }
        private void anl_a_r5() { R.A &= R5(); }
        private void anl_a_r6() { R.A &= R6(); }
        private void anl_a_r7() { R.A &= R7(); }
        private void anl_a_xr0() { R.A &= INTRAM_R(R0()); }
        private void anl_a_xr1() { R.A &= INTRAM_R(R1()); }
        private void anl_bus_n() { bus_w((byte)(bus_r() & M_RDMEM_OPCODE())); }
        private void anl_p1_n()
        {
            R.P1 &= M_RDMEM_OPCODE();
            port_w(1, R.P1);
        }
        private void anl_p2_n()
        {
            R.P2 &= M_RDMEM_OPCODE();
            port_w(2, R.P2);
        }
        private void anld_p4_a() { port_w(4, (byte)((port_r(4) & M_RDMEM_OPCODE()) & 0x0f)); }
        private void anld_p5_a() { port_w(5, (byte)((port_r(5) & M_RDMEM_OPCODE()) & 0x0f)); }
        private void anld_p6_a() { port_w(6, (byte)((port_r(6) & M_RDMEM_OPCODE()) & 0x0f)); }
        private void anld_p7_a() { port_w(7, (byte)((port_r(7) & M_RDMEM_OPCODE()) & 0x0f)); }
        private void call() { byte i = M_RDMEM_OPCODE(); ushort a11 = (R.irq_executing == I8039_NO_INT) ? R.A11 : (ushort)0; M_CALL((ushort)(i | a11)); }
        private void call_1() { byte i = M_RDMEM_OPCODE(); ushort a11 = (R.irq_executing == I8039_NO_INT) ? R.A11 : (ushort)0; M_CALL((ushort)(i | 0x100 | a11)); }
        private void call_2() { byte i = M_RDMEM_OPCODE(); ushort a11 = (R.irq_executing == I8039_NO_INT) ? R.A11 : (ushort)0; M_CALL((ushort)(i | 0x200 | a11)); }
        private void call_3() { byte i = M_RDMEM_OPCODE(); ushort a11 = (R.irq_executing == I8039_NO_INT) ? R.A11 : (ushort)0; M_CALL((ushort)(i | 0x300 | a11)); }
        private void call_4() { byte i = M_RDMEM_OPCODE(); ushort a11 = (R.irq_executing == I8039_NO_INT) ? R.A11 : (ushort)0; M_CALL((ushort)(i | 0x400 | a11)); }
        private void call_5() { byte i = M_RDMEM_OPCODE(); ushort a11 = (R.irq_executing == I8039_NO_INT) ? R.A11 : (ushort)0; M_CALL((ushort)(i | 0x500 | a11)); }
        private void call_6() { byte i = M_RDMEM_OPCODE(); ushort a11 = (R.irq_executing == I8039_NO_INT) ? R.A11 : (ushort)0; M_CALL((ushort)(i | 0x600 | a11)); }
        private void call_7() { byte i = M_RDMEM_OPCODE(); ushort a11 = (R.irq_executing == I8039_NO_INT) ? R.A11 : (ushort)0; M_CALL((ushort)(i | 0x700 | a11)); }
        private void clr_a() { R.A = 0; }
        private void clr_c() { CLR(C_FLAG); }
        private void clr_f0() { CLR(F_FLAG); }
        private void clr_f1() { R.f1 = 0; }
        private void cpl_a() { R.A ^= 0xff; }
        private void cpl_c() { R.PSW ^= C_FLAG; }
        private void cpl_f0() { R.PSW ^= F_FLAG; }
        private void cpl_f1() { R.f1 ^= 1; }
        private void dec_a() { R.A--; }
        private void dec_r0() { R.RAM[R.regPtr]--; }
        private void dec_r1() { R.RAM[R.regPtr + 1]--; }
        private void dec_r2() { R.RAM[R.regPtr + 2]--; }
        private void dec_r3() { R.RAM[R.regPtr + 3]--; }
        private void dec_r4() { R.RAM[R.regPtr + 4]--; }
        private void dec_r5() { R.RAM[R.regPtr + 5]--; }
        private void dec_r6() { R.RAM[R.regPtr + 6]--; }
        private void dec_r7() { R.RAM[R.regPtr + 7]--; }
        private void dis_i() { R.xirq_en = 0; }
        private void dis_tcnti() { R.tirq_en = 0; R.pending_irq &= unchecked((byte)~I8039_TIMCNT_INT); }
        private void djnz_r0() { byte i = M_RDMEM_OPCODE(); R.RAM[R.regPtr]--; if (R0() != 0) { R.PC.LowWord = (ushort)(((R.PC.LowWord - 1) & 0xf00) | i); } }
        private void djnz_r1() { byte i = M_RDMEM_OPCODE(); R.RAM[R.regPtr + 1]--; if (R1() != 0) { R.PC.LowWord = (ushort)(((R.PC.LowWord - 1) & 0xf00) | i); } }
        private void djnz_r2() { byte i = M_RDMEM_OPCODE(); R.RAM[R.regPtr + 2]--; if (R2() != 0) { R.PC.LowWord = (ushort)(((R.PC.LowWord - 1) & 0xf00) | i); } }
        private void djnz_r3() { byte i = M_RDMEM_OPCODE(); R.RAM[R.regPtr + 3]--; if (R3() != 0) { R.PC.LowWord = (ushort)(((R.PC.LowWord - 1) & 0xf00) | i); } }
        private void djnz_r4() { byte i = M_RDMEM_OPCODE(); R.RAM[R.regPtr + 4]--; if (R4() != 0) { R.PC.LowWord = (ushort)(((R.PC.LowWord - 1) & 0xf00) | i); } }
        private void djnz_r5() { byte i = M_RDMEM_OPCODE(); R.RAM[R.regPtr + 5]--; if (R5() != 0) { R.PC.LowWord = (ushort)(((R.PC.LowWord - 1) & 0xf00) | i); } }
        private void djnz_r6() { byte i = M_RDMEM_OPCODE(); R.RAM[R.regPtr + 6]--; if (R6() != 0) { R.PC.LowWord = (ushort)(((R.PC.LowWord - 1) & 0xf00) | i); } }
        private void djnz_r7() { byte i = M_RDMEM_OPCODE(); R.RAM[R.regPtr + 7]--; if (R7() != 0) { R.PC.LowWord = (ushort)(((R.PC.LowWord - 1) & 0xf00) | i); } }
        private void en_i() { R.xirq_en = 1; if (R.irq_state == I8039_EXTERNAL_INT) { R.irq_extra_cycles += (byte)Ext_IRQ(); } }
        private void en_tcnti() { R.tirq_en = 1; }
        private void ento_clk() { M_UNDEFINED(); }
        private void in_a_p1() { R.A = (byte)(port_r(1) & R.P1); }
        private void in_a_p2() { R.A = (byte)(port_r(2) & R.P2); }
        private void ins_a_bus() { R.A = bus_r(); }
        private void inc_a() { R.A++; }
        private void inc_r0() { R.RAM[R.regPtr]++; }
        private void inc_r1() { R.RAM[R.regPtr + 1]++; }
        private void inc_r2() { R.RAM[R.regPtr + 2]++; }
        private void inc_r3() { R.RAM[R.regPtr + 3]++; }
        private void inc_r4() { R.RAM[R.regPtr + 4]++; }
        private void inc_r5() { R.RAM[R.regPtr + 5]++; }
        private void inc_r6() { R.RAM[R.regPtr + 6]++; }
        private void inc_r7() { R.RAM[R.regPtr + 7]++; }
        private void inc_xr0() { INTRAM_W(R0(), (byte)(INTRAM_R(R0()) + 1)); }
        private void inc_xr1() { INTRAM_W(R1(), (byte)(INTRAM_R(R1()) + 1)); }
        private void jmp()
        {
            byte i = M_RDOP(R.PC.LowWord);
            ushort oldpc, newpc;
            ushort a11 = (ushort)((R.irq_executing == I8039_NO_INT) ? R.A11 : 0);
            oldpc = (ushort)(R.PC.LowWord - 1);
            R.PC.LowWord = (ushort)(i | a11);
            newpc = R.PC.LowWord;
            if (newpc == oldpc)
            {
                if (pendingCycles > 0)
                {
                    pendingCycles = 0;
                }
                else if (newpc == oldpc - 1 && M_RDOP(newpc) == 0x00)
                {
                    if (pendingCycles > 0)
                    {
                        pendingCycles = 0;
                    }
                }
            }
        }
        private void jmp_1() { byte i = M_RDOP(R.PC.LowWord); ushort a11 = (R.irq_executing == I8039_NO_INT) ? R.A11 : (ushort)0; R.PC.LowWord = (ushort)(i | 0x100 | a11); }
        private void jmp_2() { byte i = M_RDOP(R.PC.LowWord); ushort a11 = (R.irq_executing == I8039_NO_INT) ? R.A11 : (ushort)0; R.PC.LowWord = (ushort)(i | 0x200 | a11); }
        private void jmp_3() { byte i = M_RDOP(R.PC.LowWord); ushort a11 = (R.irq_executing == I8039_NO_INT) ? R.A11 : (ushort)0; R.PC.LowWord = (ushort)(i | 0x300 | a11); }
        private void jmp_4() { byte i = M_RDOP(R.PC.LowWord); ushort a11 = (R.irq_executing == I8039_NO_INT) ? R.A11 : (ushort)0; R.PC.LowWord = (ushort)(i | 0x400 | a11); }
        private void jmp_5() { byte i = M_RDOP(R.PC.LowWord); ushort a11 = (R.irq_executing == I8039_NO_INT) ? R.A11 : (ushort)0; R.PC.LowWord = (ushort)(i | 0x500 | a11); }
        private void jmp_6() { byte i = M_RDOP(R.PC.LowWord); ushort a11 = (R.irq_executing == I8039_NO_INT) ? R.A11 : (ushort)0; R.PC.LowWord = (ushort)(i | 0x600 | a11); }
        private void jmp_7() { byte i = M_RDOP(R.PC.LowWord); ushort a11 = (R.irq_executing == I8039_NO_INT) ? R.A11 : (ushort)0; R.PC.LowWord = (ushort)(i | 0x700 | a11); }
        private void jmpp_xa() { ushort addr = (ushort)((R.PC.LowWord & 0xf00) | R.A); R.PC.LowWord = (ushort)((R.PC.LowWord & 0xf00) | ReadMemory(addr)); }
        private void jb_0() { byte i = M_RDMEM_OPCODE(); if ((R.A & 0x01) != 0) { R.PC.LowWord = (ushort)(((R.PC.LowWord - 1) & 0xf00) | i); } }
        private void jb_1() { byte i = M_RDMEM_OPCODE(); if ((R.A & 0x02) != 0) { R.PC.LowWord = (ushort)(((R.PC.LowWord - 1) & 0xf00) | i); } }
        private void jb_2() { byte i = M_RDMEM_OPCODE(); if ((R.A & 0x04) != 0) { R.PC.LowWord = (ushort)(((R.PC.LowWord - 1) & 0xf00) | i); } }
        private void jb_3() { byte i = M_RDMEM_OPCODE(); if ((R.A & 0x08) != 0) { R.PC.LowWord = (ushort)(((R.PC.LowWord - 1) & 0xf00) | i); } }
        private void jb_4() { byte i = M_RDMEM_OPCODE(); if ((R.A & 0x10) != 0) { R.PC.LowWord = (ushort)(((R.PC.LowWord - 1) & 0xf00) | i); } }
        private void jb_5() { byte i = M_RDMEM_OPCODE(); if ((R.A & 0x20) != 0) { R.PC.LowWord = (ushort)(((R.PC.LowWord - 1) & 0xf00) | i); } }
        private void jb_6() { byte i = M_RDMEM_OPCODE(); if ((R.A & 0x40) != 0) { R.PC.LowWord = (ushort)(((R.PC.LowWord - 1) & 0xf00) | i); } }
        private void jb_7() { byte i = M_RDMEM_OPCODE(); if ((R.A & 0x80) != 0) { R.PC.LowWord = (ushort)(((R.PC.LowWord - 1) & 0xf00) | i); } }
        private void jf0() { byte i = M_RDMEM_OPCODE(); if (M_F0y) { R.PC.LowWord = (ushort)(((R.PC.LowWord - 1) & 0xf00) | i); } }
        private void jf1() { byte i = M_RDMEM_OPCODE(); if (R.f1 != 0) { R.PC.LowWord = (ushort)(((R.PC.LowWord - 1) & 0xf00) | i); } }
        private void jnc() { byte i = M_RDMEM_OPCODE(); if (!M_Cy) { R.PC.LowWord = (ushort)(((R.PC.LowWord - 1) & 0xf00) | i); } }
        private void jc() { byte i = M_RDMEM_OPCODE(); if (M_Cy) { R.PC.LowWord = (ushort)(((R.PC.LowWord - 1) & 0xf00) | i); } }
        private void jni() { byte i = M_RDMEM_OPCODE(); if (R.irq_state == I8039_EXTERNAL_INT) { R.PC.LowWord = (ushort)(((R.PC.LowWord - 1) & 0xf00) | i); } }
        private void jnt_0() { byte i = M_RDMEM_OPCODE(); if (test_r(0) == 0) { R.PC.LowWord = (ushort)(((R.PC.LowWord - 1) & 0xf00) | i); } }
        private void jt_0() { byte i = M_RDMEM_OPCODE(); if (test_r(0) != 0) { R.PC.LowWord = (ushort)(((R.PC.LowWord - 1) & 0xf00) | i); } }
        private void jnt_1() { byte i = M_RDMEM_OPCODE(); if (test_r(1) == 0) { R.PC.LowWord = (ushort)(((R.PC.LowWord - 1) & 0xf00) | i); } }
        private void jt_1() { byte i = M_RDMEM_OPCODE(); if (test_r(1) != 0) { R.PC.LowWord = (ushort)(((R.PC.LowWord - 1) & 0xf00) | i); } }
        private void jnz() { byte i = M_RDMEM_OPCODE(); if (R.A != 0) { R.PC.LowWord = (ushort)(((R.PC.LowWord - 1) & 0xf00) | i); } }
        private void jz() { byte i = M_RDMEM_OPCODE(); if (R.A == 0) { R.PC.LowWord = (ushort)(((R.PC.LowWord - 1) & 0xf00) | i); } }
        private void jtf() { byte i = M_RDMEM_OPCODE(); if (R.t_flag != 0) { R.PC.LowWord = (ushort)(((R.PC.LowWord - 1) & 0xf00) | i); } }
        private void mov_a_n() { R.A = M_RDMEM_OPCODE(); }
        private void mov_a_r0() { R.A = R0(); }
        private void mov_a_r1() { R.A = R1(); }
        private void mov_a_r2() { R.A = R2(); }
        private void mov_a_r3() { R.A = R3(); }
        private void mov_a_r4() { R.A = R4(); }
        private void mov_a_r5() { R.A = R5(); }
        private void mov_a_r6() { R.A = R6(); }
        private void mov_a_r7() { R.A = R7(); }
        private void mov_a_psw() { R.A = R.PSW; }
        private void mov_a_xr0() { R.A = INTRAM_R(R0()); }
        private void mov_a_xr1() { R.A = INTRAM_R(R1()); }
        private void mov_r0_a() { R.RAM[R.regPtr] = R.A; }
        private void mov_r1_a() { R.RAM[R.regPtr + 1] = R.A; }
        private void mov_r2_a() { R.RAM[R.regPtr + 2] = R.A; }
        private void mov_r3_a() { R.RAM[R.regPtr + 3] = R.A; }
        private void mov_r4_a() { R.RAM[R.regPtr + 4] = R.A; }
        private void mov_r5_a() { R.RAM[R.regPtr + 5] = R.A; }
        private void mov_r6_a() { R.RAM[R.regPtr + 6] = R.A; }
        private void mov_r7_a() { R.RAM[R.regPtr + 7] = R.A; }
        private void mov_psw_a() { R.PSW = R.A; R.regPtr = (byte)(M_By ? 24 : 0); R.SP = (byte)((R.PSW & 7) << 1); }
        private void mov_r0_n() { R.RAM[R.regPtr] = M_RDMEM_OPCODE(); }
        private void mov_r1_n() { R.RAM[R.regPtr + 1] = M_RDMEM_OPCODE(); }
        private void mov_r2_n() { R.RAM[R.regPtr + 2] = M_RDMEM_OPCODE(); }
        private void mov_r3_n() { R.RAM[R.regPtr + 3] = M_RDMEM_OPCODE(); }
        private void mov_r4_n() { R.RAM[R.regPtr + 4] = M_RDMEM_OPCODE(); }
        private void mov_r5_n() { R.RAM[R.regPtr + 5] = M_RDMEM_OPCODE(); }
        private void mov_r6_n() { R.RAM[R.regPtr + 6] = M_RDMEM_OPCODE(); }
        private void mov_r7_n() { R.RAM[R.regPtr + 7] = M_RDMEM_OPCODE(); }
        private void mov_a_t() { R.A = R.timer; }
        private void mov_t_a() { R.timer = R.A; }
        private void mov_xr0_a() { INTRAM_W(R0(), R.A); }
        private void mov_xr1_a() { INTRAM_W(R1(), R.A); }
        private void mov_xr0_n() { INTRAM_W(R0(), M_RDMEM_OPCODE()); }
        private void mov_xr1_n() { INTRAM_W(R1(), M_RDMEM_OPCODE()); }
        private void movd_a_p4() { R.A = (byte)(port_r(4) & 0x0F); }
        private void movd_a_p5() { R.A = (byte)(port_r(5) & 0x0F); }
        private void movd_a_p6() { R.A = (byte)(port_r(6) & 0x0F); }
        private void movd_a_p7() { R.A = (byte)(port_r(7) & 0x0F); }
        private void movd_p4_a() { port_w(4, (byte)(R.A & 0x0F)); }
        private void movd_p5_a() { port_w(5, (byte)(R.A & 0x0F)); }
        private void movd_p6_a() { port_w(6, (byte)(R.A & 0x0F)); }
        private void movd_p7_a() { port_w(7, (byte)(R.A & 0x0F)); }
        private void movp_a_xa() { R.A = ReadMemory((ushort)((R.PC.LowWord & 0x0f00) | R.A)); }
        private void movp3_a_xa() { R.A = ReadMemory((ushort)(0x300 | R.A)); }
        private void movx_a_xr0() { R.A = ReadIO(R0()); }
        private void movx_a_xr1() { R.A = ReadIO(R1()); }
        private void movx_xr0_a() { WriteIO(R0(), R.A); }
        private void movx_xr1_a() { WriteIO(R1(), R.A); }
        private void nop() { }
        private void orl_a_n() { R.A |= M_RDMEM_OPCODE(); }
        private void orl_a_r0() { R.A |= R0(); }
        private void orl_a_r1() { R.A |= R1(); }
        private void orl_a_r2() { R.A |= R2(); }
        private void orl_a_r3() { R.A |= R3(); }
        private void orl_a_r4() { R.A |= R4(); }
        private void orl_a_r5() { R.A |= R5(); }
        private void orl_a_r6() { R.A |= R6(); }
        private void orl_a_r7() { R.A |= R7(); }
        private void orl_a_xr0() { R.A |= INTRAM_R(R0()); }
        private void orl_a_xr1() { R.A |= INTRAM_R(R1()); }
        private void orl_bus_n() { bus_w((byte)(bus_r() | M_RDMEM_OPCODE())); }
        private void orl_p1_n()
        {
            R.P1 |= M_RDMEM_OPCODE();
            port_w(1, R.P1);
        }
        private void orl_p2_n()
        {
            R.P2 |= M_RDMEM_OPCODE();
            port_w(2, R.P2);
        }
        private void orld_p4_a() { port_w(4, (byte)(port_r(4) | R.A)); }
        private void orld_p5_a() { port_w(5, (byte)(port_r(5) | R.A)); }
        private void orld_p6_a() { port_w(6, (byte)(port_r(6) | R.A)); }
        private void orld_p7_a() { port_w(7, (byte)(port_r(7) | R.A)); }
        private void outl_bus_a() { bus_w(R.A); }
        private void outl_p1_a() { port_w(1, R.A); R.P1 = R.A; }
        private void outl_p2_a() { port_w(2, R.A); R.P2 = R.A; }
        private void ret() { R.PC.LowWord = (ushort)(((pull() & 0x0f) << 8) | pull()); }
        private void retr()
        {
            byte i = pull();
            R.PC.LowWord = (ushort)(((i & 0x0f) << 8) | pull());
            R.PSW = (byte)((R.PSW & 0x0f) | (i & 0xf0));
            R.regPtr = (byte)((M_By) ? 24 : 0);
            R.irq_executing = I8039_NO_INT;
            if (R.irq_state == I8039_EXTERNAL_INT)
            {
                R.irq_extra_cycles += (byte)Ext_IRQ();
            }
            else if (R.pending_irq == I8039_TIMCNT_INT)
            {
                R.irq_extra_cycles += (byte)Timer_IRQ();
            }
        }
        private void rl_a() { byte i = (byte)(R.A & 0x80); R.A <<= 1; if (i != 0) R.A |= 0x01; else R.A &= 0xfe; }
        private void rlc_a() { byte i = (byte)(M_Cy ? 1 : 0); if ((R.A & 0x80) != 0) SET(C_FLAG); else CLR(C_FLAG); R.A <<= 1; if (i != 0) R.A |= 0x01; else R.A &= 0xfe; }
        private void rr_a() { byte i = (byte)(R.A & 1); R.A >>= 1; if (i != 0) R.A |= 0x80; else R.A &= 0x7f; }
        private void rrc_a() { byte i = (byte)(M_Cy ? 1 : 0); if ((R.A & 1) != 0) SET(C_FLAG); else CLR(C_FLAG); R.A >>= 1; if (i != 0) R.A |= 0x80; else R.A &= 0x7f; }
        private void sel_mb0() { R.A11 = 0x000; }
        private void sel_mb1() { R.A11 = 0x800; }
        private void sel_rb0() { CLR(B_FLAG); R.regPtr = 0; }
        private void sel_rb1() { SET(B_FLAG); R.regPtr = 24; }
        private void stop_tcnt() { R.timerON = 0; R.countON = 0; }
        private void strt_cnt() { R.countON = 1; R.timerON = 0; R.Old_T1 = test_r(1); }
        private void strt_t() { R.timerON = 1; R.countON = 0; R.masterClock = 0; }
        private void swap_a() { byte i = (byte)(R.A >> 4); R.A <<= 4; R.A |= i; }
        private void xch_a_r0() { byte i = R.A; R.A = R0(); R.RAM[R.regPtr] = i; }
        private void xch_a_r1() { byte i = R.A; R.A = R1(); R.RAM[R.regPtr + 1] = i; }
        private void xch_a_r2() { byte i = R.A; R.A = R2(); R.RAM[R.regPtr + 2] = i; }
        private void xch_a_r3() { byte i = R.A; R.A = R3(); R.RAM[R.regPtr + 3] = i; }
        private void xch_a_r4() { byte i = R.A; R.A = R4(); R.RAM[R.regPtr + 4] = i; }
        private void xch_a_r5() { byte i = R.A; R.A = R5(); R.RAM[R.regPtr + 5] = i; }
        private void xch_a_r6() { byte i = R.A; R.A = R6(); R.RAM[R.regPtr + 6] = i; }
        private void xch_a_r7() { byte i = R.A; R.A = R7(); R.RAM[R.regPtr + 7] = i; }
        private void xch_a_xr0() { byte i = R.A; R.A = INTRAM_R(R0()); INTRAM_W(R0(), i); }
        private void xch_a_xr1() { byte i = R.A; R.A = INTRAM_R(R1()); INTRAM_W(R1(), i); }
        private void xchd_a_xr0() { M_XCHD(R0()); }
        private void xchd_a_xr1() { M_XCHD(R1()); }
        private void xrl_a_n() { R.A ^= M_RDMEM_OPCODE(); }
        private void xrl_a_r0() { R.A ^= R0(); }
        private void xrl_a_r1() { R.A ^= R1(); }
        private void xrl_a_r2() { R.A ^= R2(); }
        private void xrl_a_r3() { R.A ^= R3(); }
        private void xrl_a_r4() { R.A ^= R4(); }
        private void xrl_a_r5() { R.A ^= R5(); }
        private void xrl_a_r6() { R.A ^= R6(); }
        private void xrl_a_r7() { R.A ^= R7(); }
        private void xrl_a_xr0() { R.A ^= INTRAM_R(R0()); }
        private void xrl_a_xr1() { R.A ^= INTRAM_R(R1()); }
        private struct s_opcode
        {
            public byte cycles;
            public Action function;
        }
        private s_opcode[] opcode_main = new s_opcode[256];

        private void BuildOpcodeTable()
        {
            opcode_main[0x00] = new s_opcode { cycles = 1, function = nop };
            opcode_main[0x01] = new s_opcode { cycles = 0, function = illegal };
            opcode_main[0x02] = new s_opcode { cycles = 2, function = outl_bus_a };
            opcode_main[0x03] = new s_opcode { cycles = 2, function = add_a_n };
            opcode_main[0x04] = new s_opcode { cycles = 2, function = jmp };
            opcode_main[0x05] = new s_opcode { cycles = 1, function = en_i };
            opcode_main[0x06] = new s_opcode { cycles = 0, function = illegal };
            opcode_main[0x07] = new s_opcode { cycles = 1, function = dec_a };
            opcode_main[0x08] = new s_opcode { cycles = 2, function = ins_a_bus };
            opcode_main[0x09] = new s_opcode { cycles = 2, function = in_a_p1 };
            opcode_main[0x0A] = new s_opcode { cycles = 2, function = in_a_p2 };
            opcode_main[0x0B] = new s_opcode { cycles = 0, function = illegal };
            opcode_main[0x0C] = new s_opcode { cycles = 2, function = movd_a_p4 };
            opcode_main[0x0D] = new s_opcode { cycles = 2, function = movd_a_p5 };
            opcode_main[0x0E] = new s_opcode { cycles = 2, function = movd_a_p6 };
            opcode_main[0x0F] = new s_opcode { cycles = 2, function = movd_a_p7 };
            opcode_main[0x10] = new s_opcode { cycles = 1, function = inc_xr0 };
            opcode_main[0x11] = new s_opcode { cycles = 1, function = inc_xr1 };
            opcode_main[0x12] = new s_opcode { cycles = 2, function = jb_0 };
            opcode_main[0x13] = new s_opcode { cycles = 2, function = adc_a_n };
            opcode_main[0x14] = new s_opcode { cycles = 2, function = call };
            opcode_main[0x15] = new s_opcode { cycles = 1, function = dis_i };
            opcode_main[0x16] = new s_opcode { cycles = 2, function = jtf };
            opcode_main[0x17] = new s_opcode { cycles = 1, function = inc_a };
            opcode_main[0x18] = new s_opcode { cycles = 1, function = inc_r0 };
            opcode_main[0x19] = new s_opcode { cycles = 1, function = inc_r1 };
            opcode_main[0x1A] = new s_opcode { cycles = 1, function = inc_r2 };
            opcode_main[0x1B] = new s_opcode { cycles = 1, function = inc_r3 };
            opcode_main[0x1C] = new s_opcode { cycles = 1, function = inc_r4 };
            opcode_main[0x1D] = new s_opcode { cycles = 1, function = inc_r5 };
            opcode_main[0x1E] = new s_opcode { cycles = 1, function = inc_r6 };
            opcode_main[0x1F] = new s_opcode { cycles = 1, function = inc_r7 };
            opcode_main[0x20] = new s_opcode { cycles = 1, function = xch_a_xr0 };
            opcode_main[0x21] = new s_opcode { cycles = 1, function = xch_a_xr1 };
            opcode_main[0x22] = new s_opcode { cycles = 0, function = illegal };
            opcode_main[0x23] = new s_opcode { cycles = 2, function = mov_a_n };
            opcode_main[0x24] = new s_opcode { cycles = 2, function = jmp_1 };
            opcode_main[0x25] = new s_opcode { cycles = 1, function = en_tcnti };
            opcode_main[0x26] = new s_opcode { cycles = 2, function = jnt_0 };
            opcode_main[0x27] = new s_opcode { cycles = 1, function = clr_a };
            opcode_main[0x28] = new s_opcode { cycles = 1, function = xch_a_r0 };
            opcode_main[0x29] = new s_opcode { cycles = 1, function = xch_a_r1 };
            opcode_main[0x2A] = new s_opcode { cycles = 1, function = xch_a_r2 };
            opcode_main[0x2B] = new s_opcode { cycles = 1, function = xch_a_r3 };
            opcode_main[0x2C] = new s_opcode { cycles = 1, function = xch_a_r4 };
            opcode_main[0x2D] = new s_opcode { cycles = 1, function = xch_a_r5 };
            opcode_main[0x2E] = new s_opcode { cycles = 1, function = xch_a_r6 };
            opcode_main[0x2F] = new s_opcode { cycles = 1, function = xch_a_r7 };
            opcode_main[0x30] = new s_opcode { cycles = 1, function = xchd_a_xr0 };
            opcode_main[0x31] = new s_opcode { cycles = 1, function = xchd_a_xr1 };
            opcode_main[0x32] = new s_opcode { cycles = 2, function = jb_1 };
            opcode_main[0x33] = new s_opcode { cycles = 0, function = illegal };
            opcode_main[0x34] = new s_opcode { cycles = 2, function = call_1 };
            opcode_main[0x35] = new s_opcode { cycles = 1, function = dis_tcnti };
            opcode_main[0x36] = new s_opcode { cycles = 2, function = jt_0 };
            opcode_main[0x37] = new s_opcode { cycles = 1, function = cpl_a };
            opcode_main[0x38] = new s_opcode { cycles = 0, function = illegal };
            opcode_main[0x39] = new s_opcode { cycles = 2, function = outl_p1_a };
            opcode_main[0x3A] = new s_opcode { cycles = 2, function = outl_p2_a };
            opcode_main[0x3B] = new s_opcode { cycles = 0, function = illegal };
            opcode_main[0x3C] = new s_opcode { cycles = 2, function = movd_p4_a };
            opcode_main[0x3D] = new s_opcode { cycles = 2, function = movd_p5_a };
            opcode_main[0x3E] = new s_opcode { cycles = 2, function = movd_p6_a };
            opcode_main[0x3F] = new s_opcode { cycles = 2, function = movd_p7_a };
            opcode_main[0x40] = new s_opcode { cycles = 1, function = orl_a_xr0 };
            opcode_main[0x41] = new s_opcode { cycles = 1, function = orl_a_xr1 };
            opcode_main[0x42] = new s_opcode { cycles = 1, function = mov_a_t };
            opcode_main[0x43] = new s_opcode { cycles = 2, function = orl_a_n };
            opcode_main[0x44] = new s_opcode { cycles = 2, function = jmp_2 };
            opcode_main[0x45] = new s_opcode { cycles = 1, function = strt_cnt };
            opcode_main[0x46] = new s_opcode { cycles = 2, function = jnt_1 };
            opcode_main[0x47] = new s_opcode { cycles = 1, function = swap_a };
            opcode_main[0x48] = new s_opcode { cycles = 1, function = orl_a_r0 };
            opcode_main[0x49] = new s_opcode { cycles = 1, function = orl_a_r1 };
            opcode_main[0x4A] = new s_opcode { cycles = 1, function = orl_a_r2 };
            opcode_main[0x4B] = new s_opcode { cycles = 1, function = orl_a_r3 };
            opcode_main[0x4C] = new s_opcode { cycles = 1, function = orl_a_r4 };
            opcode_main[0x4D] = new s_opcode { cycles = 1, function = orl_a_r5 };
            opcode_main[0x4E] = new s_opcode { cycles = 1, function = orl_a_r6 };
            opcode_main[0x4F] = new s_opcode { cycles = 1, function = orl_a_r7 };
            opcode_main[0x50] = new s_opcode { cycles = 1, function = anl_a_xr0 };
            opcode_main[0x51] = new s_opcode { cycles = 1, function = anl_a_xr1 };
            opcode_main[0x52] = new s_opcode { cycles = 2, function = jb_2 };
            opcode_main[0x53] = new s_opcode { cycles = 2, function = anl_a_n };
            opcode_main[0x54] = new s_opcode { cycles = 2, function = call_2 };
            opcode_main[0x55] = new s_opcode { cycles = 1, function = strt_t };
            opcode_main[0x56] = new s_opcode { cycles = 2, function = jt_1 };
            opcode_main[0x57] = new s_opcode { cycles = 1, function = daa_a };
            opcode_main[0x58] = new s_opcode { cycles = 1, function = anl_a_r0 };
            opcode_main[0x59] = new s_opcode { cycles = 1, function = anl_a_r1 };
            opcode_main[0x5A] = new s_opcode { cycles = 1, function = anl_a_r2 };
            opcode_main[0x5B] = new s_opcode { cycles = 1, function = anl_a_r3 };
            opcode_main[0x5C] = new s_opcode { cycles = 1, function = anl_a_r4 };
            opcode_main[0x5D] = new s_opcode { cycles = 1, function = anl_a_r5 };
            opcode_main[0x5E] = new s_opcode { cycles = 1, function = anl_a_r6 };
            opcode_main[0x5F] = new s_opcode { cycles = 1, function = anl_a_r7 };
            opcode_main[0x60] = new s_opcode { cycles = 1, function = add_a_xr0 };
            opcode_main[0x61] = new s_opcode { cycles = 1, function = add_a_xr1 };
            opcode_main[0x62] = new s_opcode { cycles = 1, function = mov_t_a };
            opcode_main[0x63] = new s_opcode { cycles = 0, function = illegal };
            opcode_main[0x64] = new s_opcode { cycles = 2, function = jmp_3 };
            opcode_main[0x65] = new s_opcode { cycles = 1, function = stop_tcnt };
            opcode_main[0x66] = new s_opcode { cycles = 0, function = illegal };
            opcode_main[0x67] = new s_opcode { cycles = 1, function = rrc_a };
            opcode_main[0x68] = new s_opcode { cycles = 1, function = add_a_r0 };
            opcode_main[0x69] = new s_opcode { cycles = 1, function = add_a_r1 };
            opcode_main[0x6A] = new s_opcode { cycles = 1, function = add_a_r2 };
            opcode_main[0x6B] = new s_opcode { cycles = 1, function = add_a_r3 };
            opcode_main[0x6C] = new s_opcode { cycles = 1, function = add_a_r4 };
            opcode_main[0x6D] = new s_opcode { cycles = 1, function = add_a_r5 };
            opcode_main[0x6E] = new s_opcode { cycles = 1, function = add_a_r6 };
            opcode_main[0x6F] = new s_opcode { cycles = 1, function = add_a_r7 };
            opcode_main[0x70] = new s_opcode { cycles = 1, function = adc_a_xr0 };
            opcode_main[0x71] = new s_opcode { cycles = 1, function = adc_a_xr1 };
            opcode_main[0x72] = new s_opcode { cycles = 2, function = jb_3 };
            opcode_main[0x73] = new s_opcode { cycles = 0, function = illegal };
            opcode_main[0x74] = new s_opcode { cycles = 2, function = call_3 };
            opcode_main[0x75] = new s_opcode { cycles = 1, function = ento_clk };
            opcode_main[0x76] = new s_opcode { cycles = 2, function = jf1 };
            opcode_main[0x77] = new s_opcode { cycles = 1, function = rr_a };
            opcode_main[0x78] = new s_opcode { cycles = 1, function = adc_a_r0 };
            opcode_main[0x79] = new s_opcode { cycles = 1, function = adc_a_r1 };
            opcode_main[0x7A] = new s_opcode { cycles = 1, function = adc_a_r2 };
            opcode_main[0x7B] = new s_opcode { cycles = 1, function = adc_a_r3 };
            opcode_main[0x7C] = new s_opcode { cycles = 1, function = adc_a_r4 };
            opcode_main[0x7D] = new s_opcode { cycles = 1, function = adc_a_r5 };
            opcode_main[0x7E] = new s_opcode { cycles = 1, function = adc_a_r6 };
            opcode_main[0x7F] = new s_opcode { cycles = 1, function = adc_a_r7 };
            opcode_main[0x80] = new s_opcode { cycles = 2, function = movx_a_xr0 };
            opcode_main[0x81] = new s_opcode { cycles = 2, function = movx_a_xr1 };
            opcode_main[0x82] = new s_opcode { cycles = 0, function = illegal };
            opcode_main[0x83] = new s_opcode { cycles = 2, function = ret };
            opcode_main[0x84] = new s_opcode { cycles = 2, function = jmp_4 };
            opcode_main[0x85] = new s_opcode { cycles = 1, function = clr_f0 };
            opcode_main[0x86] = new s_opcode { cycles = 2, function = jni };
            opcode_main[0x87] = new s_opcode { cycles = 0, function = illegal };
            opcode_main[0x88] = new s_opcode { cycles = 2, function = orl_bus_n };
            opcode_main[0x89] = new s_opcode { cycles = 2, function = orl_p1_n };
            opcode_main[0x8A] = new s_opcode { cycles = 2, function = orl_p2_n };
            opcode_main[0x8B] = new s_opcode { cycles = 0, function = illegal };
            opcode_main[0x8C] = new s_opcode { cycles = 2, function = orld_p4_a };
            opcode_main[0x8D] = new s_opcode { cycles = 2, function = orld_p5_a };
            opcode_main[0x8E] = new s_opcode { cycles = 2, function = orld_p6_a };
            opcode_main[0x8F] = new s_opcode { cycles = 2, function = orld_p7_a };
            opcode_main[0x90] = new s_opcode { cycles = 2, function = movx_xr0_a };
            opcode_main[0x91] = new s_opcode { cycles = 2, function = movx_xr1_a };
            opcode_main[0x92] = new s_opcode { cycles = 2, function = jb_4 };
            opcode_main[0x93] = new s_opcode { cycles = 2, function = retr };
            opcode_main[0x94] = new s_opcode { cycles = 2, function = call_4 };
            opcode_main[0x95] = new s_opcode { cycles = 1, function = cpl_f0 };
            opcode_main[0x96] = new s_opcode { cycles = 2, function = jnz };
            opcode_main[0x97] = new s_opcode { cycles = 1, function = clr_c };
            opcode_main[0x98] = new s_opcode { cycles = 2, function = anl_bus_n };
            opcode_main[0x99] = new s_opcode { cycles = 2, function = anl_p1_n };
            opcode_main[0x9A] = new s_opcode { cycles = 2, function = anl_p2_n };
            opcode_main[0x9B] = new s_opcode { cycles = 0, function = illegal };
            opcode_main[0x9C] = new s_opcode { cycles = 2, function = anld_p4_a };
            opcode_main[0x9D] = new s_opcode { cycles = 2, function = anld_p5_a };
            opcode_main[0x9E] = new s_opcode { cycles = 2, function = anld_p6_a };
            opcode_main[0x9F] = new s_opcode { cycles = 2, function = anld_p7_a };
            opcode_main[0xA0] = new s_opcode { cycles = 1, function = mov_xr0_a };
            opcode_main[0xA1] = new s_opcode { cycles = 1, function = mov_xr1_a };
            opcode_main[0xA2] = new s_opcode { cycles = 0, function = illegal };
            opcode_main[0xA3] = new s_opcode { cycles = 2, function = movp_a_xa };
            opcode_main[0xA4] = new s_opcode { cycles = 2, function = jmp_5 };
            opcode_main[0xA5] = new s_opcode { cycles = 1, function = clr_f1 };
            opcode_main[0xA6] = new s_opcode { cycles = 0, function = illegal };
            opcode_main[0xA7] = new s_opcode { cycles = 1, function = cpl_c };
            opcode_main[0xA8] = new s_opcode { cycles = 1, function = mov_r0_a };
            opcode_main[0xA9] = new s_opcode { cycles = 1, function = mov_r1_a };
            opcode_main[0xAA] = new s_opcode { cycles = 1, function = mov_r2_a };
            opcode_main[0xAB] = new s_opcode { cycles = 1, function = mov_r3_a };
            opcode_main[0xAC] = new s_opcode { cycles = 1, function = mov_r4_a };
            opcode_main[0xAD] = new s_opcode { cycles = 1, function = mov_r5_a };
            opcode_main[0xAE] = new s_opcode { cycles = 1, function = mov_r6_a };
            opcode_main[0xAF] = new s_opcode { cycles = 1, function = mov_r7_a };
            opcode_main[0xB0] = new s_opcode { cycles = 2, function = mov_xr0_n };
            opcode_main[0xB1] = new s_opcode { cycles = 2, function = mov_xr1_n };
            opcode_main[0xB2] = new s_opcode { cycles = 2, function = jb_5 };
            opcode_main[0xB3] = new s_opcode { cycles = 2, function = jmpp_xa };
            opcode_main[0xB4] = new s_opcode { cycles = 2, function = call_5 };
            opcode_main[0xB5] = new s_opcode { cycles = 1, function = cpl_f1 };
            opcode_main[0xB6] = new s_opcode { cycles = 2, function = jf0 };
            opcode_main[0xB7] = new s_opcode { cycles = 0, function = illegal };
            opcode_main[0xB8] = new s_opcode { cycles = 2, function = mov_r0_n };
            opcode_main[0xB9] = new s_opcode { cycles = 2, function = mov_r1_n };
            opcode_main[0xBA] = new s_opcode { cycles = 2, function = mov_r2_n };
            opcode_main[0xBB] = new s_opcode { cycles = 2, function = mov_r3_n };
            opcode_main[0xBC] = new s_opcode { cycles = 2, function = mov_r4_n };
            opcode_main[0xBD] = new s_opcode { cycles = 2, function = mov_r5_n };
            opcode_main[0xBE] = new s_opcode { cycles = 2, function = mov_r6_n };
            opcode_main[0xBF] = new s_opcode { cycles = 2, function = mov_r7_n };
            opcode_main[0xC0] = new s_opcode { cycles = 0, function = illegal };
            opcode_main[0xC1] = new s_opcode { cycles = 0, function = illegal };
            opcode_main[0xC2] = new s_opcode { cycles = 0, function = illegal };
            opcode_main[0xC3] = new s_opcode { cycles = 0, function = illegal };
            opcode_main[0xC4] = new s_opcode { cycles = 2, function = jmp_6 };
            opcode_main[0xC5] = new s_opcode { cycles = 1, function = sel_rb0 };
            opcode_main[0xC6] = new s_opcode { cycles = 2, function = jz };
            opcode_main[0xC7] = new s_opcode { cycles = 1, function = mov_a_psw };
            opcode_main[0xC8] = new s_opcode { cycles = 1, function = dec_r0 };
            opcode_main[0xC9] = new s_opcode { cycles = 1, function = dec_r1 };
            opcode_main[0xCA] = new s_opcode { cycles = 1, function = dec_r2 };
            opcode_main[0xCB] = new s_opcode { cycles = 1, function = dec_r3 };
            opcode_main[0xCC] = new s_opcode { cycles = 1, function = dec_r4 };
            opcode_main[0xCD] = new s_opcode { cycles = 1, function = dec_r5 };
            opcode_main[0xCE] = new s_opcode { cycles = 1, function = dec_r6 };
            opcode_main[0xCF] = new s_opcode { cycles = 1, function = dec_r7 };
            opcode_main[0xD0] = new s_opcode { cycles = 1, function = xrl_a_xr0 };
            opcode_main[0xD1] = new s_opcode { cycles = 1, function = xrl_a_xr1 };
            opcode_main[0xD2] = new s_opcode { cycles = 2, function = jb_6 };
            opcode_main[0xD3] = new s_opcode { cycles = 2, function = xrl_a_n };
            opcode_main[0xD4] = new s_opcode { cycles = 2, function = call_6 };
            opcode_main[0xD5] = new s_opcode { cycles = 1, function = sel_rb1 };
            opcode_main[0xD6] = new s_opcode { cycles = 0, function = illegal };
            opcode_main[0xD7] = new s_opcode { cycles = 1, function = mov_psw_a };
            opcode_main[0xD8] = new s_opcode { cycles = 1, function = xrl_a_r0 };
            opcode_main[0xD9] = new s_opcode { cycles = 1, function = xrl_a_r1 };
            opcode_main[0xDA] = new s_opcode { cycles = 1, function = xrl_a_r2 };
            opcode_main[0xDB] = new s_opcode { cycles = 1, function = xrl_a_r3 };
            opcode_main[0xDC] = new s_opcode { cycles = 1, function = xrl_a_r4 };
            opcode_main[0xDD] = new s_opcode { cycles = 1, function = xrl_a_r5 };
            opcode_main[0xDE] = new s_opcode { cycles = 1, function = xrl_a_r6 };
            opcode_main[0xDF] = new s_opcode { cycles = 1, function = xrl_a_r7 };
            opcode_main[0xE0] = new s_opcode { cycles = 0, function = illegal };
            opcode_main[0xE1] = new s_opcode { cycles = 0, function = illegal };
            opcode_main[0xE2] = new s_opcode { cycles = 0, function = illegal };
            opcode_main[0xE3] = new s_opcode { cycles = 2, function = movp3_a_xa };
            opcode_main[0xE4] = new s_opcode { cycles = 2, function = jmp_7 };
            opcode_main[0xE5] = new s_opcode { cycles = 1, function = sel_mb0 };
            opcode_main[0xE6] = new s_opcode { cycles = 2, function = jnc };
            opcode_main[0xE7] = new s_opcode { cycles = 1, function = rl_a };
            opcode_main[0xE8] = new s_opcode { cycles = 2, function = djnz_r0 };
            opcode_main[0xE9] = new s_opcode { cycles = 2, function = djnz_r1 };
            opcode_main[0xEA] = new s_opcode { cycles = 2, function = djnz_r2 };
            opcode_main[0xEB] = new s_opcode { cycles = 2, function = djnz_r3 };
            opcode_main[0xEC] = new s_opcode { cycles = 2, function = djnz_r4 };
            opcode_main[0xED] = new s_opcode { cycles = 2, function = djnz_r5 };
            opcode_main[0xEE] = new s_opcode { cycles = 2, function = djnz_r6 };
            opcode_main[0xEF] = new s_opcode { cycles = 2, function = djnz_r7 };
            opcode_main[0xF0] = new s_opcode { cycles = 1, function = mov_a_xr0 };
            opcode_main[0xF1] = new s_opcode { cycles = 1, function = mov_a_xr1 };
            opcode_main[0xF2] = new s_opcode { cycles = 2, function = jb_7 };
            opcode_main[0xF3] = new s_opcode { cycles = 0, function = illegal };
            opcode_main[0xF4] = new s_opcode { cycles = 2, function = call_7 };
            opcode_main[0xF5] = new s_opcode { cycles = 1, function = sel_mb1 };
            opcode_main[0xF6] = new s_opcode { cycles = 2, function = jc };
            opcode_main[0xF7] = new s_opcode { cycles = 1, function = rlc_a };
            opcode_main[0xF8] = new s_opcode { cycles = 1, function = mov_a_r0 };
            opcode_main[0xF9] = new s_opcode { cycles = 1, function = mov_a_r1 };
            opcode_main[0xFA] = new s_opcode { cycles = 1, function = mov_a_r2 };
            opcode_main[0xFB] = new s_opcode { cycles = 1, function = mov_a_r3 };
            opcode_main[0xFC] = new s_opcode { cycles = 1, function = mov_a_r4 };
            opcode_main[0xFD] = new s_opcode { cycles = 1, function = mov_a_r5 };
            opcode_main[0xFE] = new s_opcode { cycles = 1, function = mov_a_r6 };
            opcode_main[0xFF] = new s_opcode { cycles = 1, function = mov_a_r7 };
            
        }
        private int Ext_IRQ()
        {
            int extra_cycles = 0;
            if (R.xirq_en != 0 && R.irq_executing == I8039_NO_INT)
            {
                R.irq_executing = I8039_EXTERNAL_INT;
                push((byte)R.PC.LowByte);
                push((byte)(((R.PC.HighByte >> 8) & 0x0f) | (R.PSW & 0xf0)));
                R.PC.LowWord = 0x03;
                extra_cycles = 2;
                if (R.timerON != 0)
                {
                    R.masterClock += (byte)extra_cycles;
                }
                if (IrqCallback != null)
                {
                    IrqCallback(0);
                }
            }
            return extra_cycles;
        }
        private int Timer_IRQ()
        {
            int extra_cycles = 0;
            if (R.tirq_en != 0)
            {
                if (R.irq_executing == I8039_NO_INT)
                {
                    R.irq_executing = I8039_TIMCNT_INT;
                    R.pending_irq &= unchecked((byte)~I8039_TIMCNT_INT);
                    push((byte)R.PC.LowByte);
                    push((byte)(((R.PC.HighByte >> 8) & 0x0f) | (R.PSW & 0xf0)));
                    R.PC.LowWord = 0x07;
                    extra_cycles = 2;
                    if (R.timerON != 0)
                    {
                        R.masterClock += (byte)extra_cycles;
                    }
                }
                else if (R.irq_executing == I8039_EXTERNAL_INT)
                {
                    R.pending_irq |= I8039_TIMCNT_INT;
                }
            }
            R.t_flag = 1;
            return extra_cycles;
        }
        public void Init(int clock, Func<int,int> irqcallback)
        {
            IrqCallback = irqcallback;
            R.cpu_feature = 0;
            R.ram_mask = 0x7F;
            R.int_rom_size = 0x800;
            R.RAM = new byte[128];
            R.timer = 0;
            BuildOpcodeTable();
        }
        public override void Reset()
        {
            R.PC.LowWord = 0;
            R.SP = 0;
            R.A = 0;
            R.PSW = 0x08;
            R.P1 = 0xff;
            R.P2 = 0xff;
            R.bus = 0;
            R.irq_executing = I8039_NO_INT;
            R.pending_irq = I8039_NO_INT;
            R.A11 = 0;
            R.tirq_en = R.xirq_en = 0;
            R.timerON = R.countON = 0;
            R.irq_extra_cycles = 0;
            R.masterClock = 0;
            R.regPtr = 0;
        }
        public override int ExecuteCycles(int cycles)
        {
            byte opcode,T1;
            int timerInt,c;
            pendingCycles = cycles - R.irq_extra_cycles;
            R.irq_extra_cycles = 0;
            do
            {
                R.PREVPC = R.PC;
                opcode = M_RDOP(R.PC.LowWord);
                R.PC.LowWord++;
                R.inst_cycles = opcode_main[opcode].cycles;
                opcode_main[opcode].function();
                pendingCycles -= R.inst_cycles;
                timerInt = 0;
                if (R.countON != 0)
                {
                    for (c = 0; c < R.inst_cycles; c++)
                    {
                        T1 = (byte)ReadIO(0x111);
                        if ((T1 - R.Old_T1) > 0)
                        {
                            R.timer++;
                            if (R.timer == 0)
                            {
                                timerInt = 1;
                            }
                        }
                        R.Old_T1 = T1;
                    }
                }
                if (R.timerON != 0)
                {
                    R.masterClock += (byte)R.inst_cycles;
                    while (R.masterClock >= 32)
                    {
                        R.masterClock -= 32;
                        R.timer++;
                        if (R.timer == 0)
                        {
                            timerInt = 1;
                        }
                    }
                }
                if (timerInt != 0)
                {
                    int count = Timer_IRQ();
                    pendingCycles -= count;
                }
            }
            while (pendingCycles > 0);
            pendingCycles -= R.irq_extra_cycles;
            R.irq_extra_cycles = 0;
            return cycles - pendingCycles;
        }
        public override void set_irq_line(int irqline, LineState state)
        {
            if (state != LineState.CLEAR_LINE)
            {
                R.irq_state = I8039_EXTERNAL_INT;
                R.irq_extra_cycles += (byte)Ext_IRQ();
            }
            else
            {
                R.irq_state = I8039_NO_INT;
            }
        }
    }
}
