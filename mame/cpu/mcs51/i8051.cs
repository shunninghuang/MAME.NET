using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using mame;

namespace cpu.mcs51
{
    public partial class I8051 : cpuexec_data
    {
        public struct I8051_UART
        {
            public byte timerbaud;
            public byte sending;
            public byte data_out;
            public byte bits_to_send;
            public byte bitcycles;
        }
        public I8051_UART uart;
        public ushort ppc;
        public ushort pc;
        public ushort subtype;
        public byte cur_irq;
        public byte irq_priority;
        public byte rwm;
        public byte prev_used_cycles;
        public byte last_int0;
        public byte last_int1;
        public byte int_vec;
        public byte priority_request;
        public byte po;
        public byte sp;
        public byte dpl;
        public byte dph;
        public byte pcon;
        public byte tcon;
        public byte tmod;
        public byte tl0;
        public byte tl1;
        public byte th0;
        public byte th1;
        public byte p1;
        public byte scon;
        public byte sbuf;
        public byte p2;
        public byte ie;
        public byte p3;
        public byte ip;
        public byte t2con;
        public byte rcap2l;
        public byte rcap2h;
        public byte tl2;
        public byte th2;
        public byte psw;
        public byte acc;
        public byte b;
        public static I8051 I1;
        public byte[] IntRam = new byte[0x100];
        public Func<ushort, byte> ReadMemory;
        public Func<ushort, byte> ReadData;
        public Action<ushort, byte> WriteData;
        public Func<ushort, byte> ReadOp;
        public Func<ushort, byte> ReadOpArg;
        public Func<ushort, byte> ReadIO;
        public Action<ushort, byte> WriteIO;
        public Func<int, int> irq_callback;
        public Action<int> serial_tx_callback;
        public Func<int> serial_rx_callback;
        public Func<int, byte> iram_iread;
        public Action<int, byte> iram_iwrite;
        public Func<int,uint, uint> eram_iaddr_callback;
        public Action<int> hold_serial_tx_callback;
        public Func<int> hold_serial_rx_callback;
        public Func<int, uint, uint> hold_eram_iaddr_callback;
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
        public override void set_irq_line(int irqline, LineState state)
        {
            switch (irqline)
            {
                case 0:
                    if (state != LineState.CLEAR_LINE)
                    {
                        if (GET_EX0()!=0)
                        {
                            if (GET_IT0()!=0)
                            {
                                if (last_int0 == 0)
                                    SET_IE0(1);
                            }
                            else
                                SET_IE0(1);
                        }
                    }
                    else
                    {
                        SET_IE0(0);
                    }
                    last_int0 = (byte)state;
                    if (GET_IE0() != 0)
                    {
                        pendingCycles -= check_interrupts();
                    }
                    break;
                case 1:
                    if (state != LineState.CLEAR_LINE)
                    {
                        if (GET_EX1()!=0)
                        {
                            if (GET_IT1()!=0)
                            {
                                if (last_int1 == 0)
                                {
                                    SET_IE1(1);
                                }
                            }
                            else
                            {
                                SET_IE1(1);
                            }
                        }
                    }
                    else
                    {
                        SET_IE1(0);
                    }
                    last_int1 = (byte)state;
                    if (GET_IE1() != 0)
                    {
                        pendingCycles -= check_interrupts();
                    }
                    break;
                case 4:
                    if (GET_ES()!=0 && GET_REN()!=0)
                    {
                        int data = 0;
                        if (serial_rx_callback != null)
                        {
                            data = serial_rx_callback();
                        }
                        sbuf =(byte)data;
                        SET_RI(1);
                    }
                    break;
            }
        }
        public override void cpunum_set_input_line_and_vector(int cpunum, int line, LineState state, int vector)
        {
            Timer.timer_set_internal(Cpuint.cpunum_empty_event_queue, "cpunum_empty_event_queue");
        }


        public static int P0 = 0x80, SP = 0x81, DPL = 0x82, DPH = 0x83, PCON = 0x87, TCON = 0x88, TMOD = 0x89, TL0 = 0x8a, TL1 = 0x8b, TH0 = 0x8c, TH1 = 0x8d, P1 = 0x90, SCON = 0x98, SBUF = 0x99, P2 = 0xa0, IE = 0xa8, P3 = 0xb0, IP = 0xb8, T2CON = 0xc8, RCAP2L = 0xca, RCAP2H = 0xcb, TL2 = 0xcc, TH2 = 0xcd, PSW = 0xd0, ACC = 0xe0, B = 0xf0, C = 0xd7, P = 0xd0, AC = 0xd6, OV = 0xd2, TF0 = 0x8d, TF1 = 0x8f, IE0 = 0x89, IE1 = 0x8b, TI = 0x99, RI = 0x98, TI_FLAG = 1, RI_FLAG = 2;

        public byte[] i8051_cycles = new byte[]
        {
	        12,24,24,12,12,12,12,12,12,12,12,12,12,12,12,12,
	        24,24,24,12,12,12,12,12,12,12,12,12,12,12,12,12,
	        24,24,24,12,12,12,12,12,12,12,12,12,12,12,12,12,
	        24,24,24,12,12,12,12,12,12,12,12,12,12,12,12,12,
	        24,24,12,24,12,12,12,12,12,12,12,12,12,12,12,12,
	        24,24,12,24,12,12,12,12,12,12,12,12,12,12,12,12,
	        24,24,12,24,12,12,12,12,12,12,12,12,12,12,12,12,
	        24,24,24,24,12,24,12,12,12,12,12,12,12,12,12,12,
	        24,24,24,24,48,24,24,24,24,24,24,24,24,24,24,24,
	        24,24,24,24,12,12,12,12,12,12,12,12,12,12,12,12,
	        24,24,12,24,48,12,24,24,24,24,24,24,24,24,24,24,
	        24,24,12,12,24,24,24,24,24,24,24,24,24,24,24,24,
	        24,24,12,12,12,12,12,12,12,12,12,12,12,12,12,12,
	        24,24,12,12,12,24,12,12,24,24,24,24,24,24,24,24,
	        24,24,24,24,12,12,12,12,12,12,12,12,12,12,12,12,
	        24,24,24,24,12,12,12,12,12,12,12,12,12,12,12,12
        };
        private void SET_CY(int n)
        {
            psw = (byte)((psw & 0x7f) | (n << 7));
        }
        private void SET_AC(int n)
        {
            psw = (byte)((psw & 0xbf) | (n << 6));
        }
        private void SET_FO(int n)
        {
            psw = (byte)((psw & 0xdf) | (n << 5));
        }
        private void SET_RS(int n)
        {
            psw = (byte)((psw & 0xe7) | (n << 3));
        }
        private void SET_OV(int n)
        {
            psw = (byte)((psw & 0xfb) | (n << 2));
        }
        private void SET_P(int n)
        {
            psw = (byte)((psw & 0xfe) | (n << 0));
        }
        private void SET_EA(int n)
        {
            ie = (byte)((ie & 0x7f) | (n << 7));
        }
        private void SET_ET2(int n)
        {
            ie = (byte)((ie & 0xdf) | (n << 5));
        }
        private void SET_ES(int n)
        {
            ie = (byte)((ie & 0xef) | (n << 4));
        }
        private void SET_ET1(int n)
        {
            ie = (byte)((ie & 0xf7) | (n << 3));
        }
        private void SET_EX1(int n)
        {
            ie = (byte)((ie & 0xfb) | (n << 2));
        }
        private void SET_ET0(int n)
        {
            ie = (byte)((ie & 0xfd) | (n << 1));
        }
        private void SET_EX0(int n)
        {
            ie = (byte)((ie & 0xfe) | (n << 0));
        }
        private void SET_PT2(int n)
        {
            ip = (byte)((ip & 0xdf) | (n << 5));
        }
        private void SET_PS0(int n)
        {
            ip = (byte)((ip & 0xef) | (n << 4));
        }
        private void SET_PT1(int n)
        {
            ip = (byte)((ip & 0xf7) | (n << 3));
        }
        private void SET_PX1(int n)
        {
            ip = (byte)((ip & 0xfb) | (n << 2));
        }
        private void SET_PT0(int n)
        {
            ip = (byte)((ip & 0xfd) | (n << 1));
        }
        private void SET_PX0(int n)
        {
            ip = (byte)((ip & 0xfe) | (n << 0));
        }
        private void SET_TF1(int n)
        {
            tcon = (byte)((tcon & 0x7f) | (n << 7));
        }
        private void SET_TR1(int n)
        {
            tcon = (byte)((tcon & 0xbf) | (n << 6));
        }
        private void SET_TF0(int n)
        {
            tcon = (byte)((tcon & 0xdf) | (n << 5));
        }
        private void SET_TR0(int n)
        {
            tcon = (byte)((tcon & 0xef) | (n << 4));
        }
        private void SET_IE1(int n)
        {
            tcon = (byte)((tcon & 0xf7) | (n << 3));
        }
        private void SET_IT1(int n)
        {
            tcon = (byte)((tcon & 0xfb) | (n << 2));
        }
        private void SET_IE0(int n)
        {
            tcon = (byte)((tcon & 0xfd) | (n << 1));
        }
        private void SET_IT0(int n)
        {
            tcon = (byte)((tcon & 0xfe) | (n << 0));
        }
        private void SET_SM0(int n)
        {
            scon = (byte)((scon & 0x7f) | (n << 7));
        }
        private void SET_SM1(int n)
        {
            scon = (byte)((scon & 0xbf) | (n << 6));
        }
        private void SET_SM2(int n)
        {
            scon = (byte)((scon & 0xdf) | (n << 5));
        }
        private void SET_REN(int n)
        {
            scon = (byte)((scon & 0xef) | (n << 4));
        }
        private void SET_TB8(int n)
        {
            scon = (byte)((scon & 0xf7) | (n << 3));
        }
        private void SET_RB8(int n)
        {
            scon = (byte)((scon & 0xfb) | (n << 2));
        }
        private void SET_TI(int n)
        {
            scon = (byte)((scon & 0xfd) | (n << 1));
        }
        private void SET_RI(int n)
        {
            scon = (byte)((scon & 0xfe) | (n << 0));
        }
        private void SET_GATE1(int n)
        {
            tmod = (byte)((tmod & 0x7f) | (n << 7));
        }
        private void SET_CT1(int n)
        {
            tmod = (byte)((tmod & 0xbf) | (n << 6));
        }
        private void SET_M1_1(int n)
        {
            tmod = (byte)((tmod & 0xdf) | (n << 5));
        }
        private void SET_M1_0(int n)
        {
            tmod = (byte)((tmod & 0xef) | (n << 4));
        }
        private void SET_GATE0(int n)
        {
            tmod = (byte)((tmod & 0xf7) | (n << 3));
        }
        private void SET_CT0(int n)
        {
            tmod = (byte)((tmod & 0xfb) | (n << 2));
        }
        private void SET_M0_1(int n)
        {
            tmod = (byte)((tmod & 0xfd) | (n << 1));
        }
        private void SET_M0_0(int n)
        {
            tmod = (byte)((tmod & 0xfe) | (n << 0));
        }
        private void SET_TF2(int n)
        {
            t2con = (byte)((t2con & 0x7f) | (n << 7));
        }
        private void SET_EXF2(int n)
        {
            t2con = (byte)((t2con & 0xbf) | (n << 6));
        }
        private void SET_RCLK(int n)
        {
            t2con = (byte)((t2con & 0xdf) | (n << 5));
        }
        private void SET_TCLK(int n)
        {
            t2con = (byte)((t2con & 0xef) | (n << 4));
        }
        private void SET_EXEN2(int n)
        {
            t2con = (byte)((t2con & 0xf7) | (n << 3));
        }
        private void SET_TR2(int n)
        {
            t2con = (byte)((t2con & 0xfb) | (n << 2));
        }
        private void SET_CT2(int n)
        {
            t2con = (byte)((t2con & 0xfd) | (n << 1));
        }
        private void SET_CP(int n)
        {
            t2con = (byte)((t2con & 0xfe) | (n << 0));
        }
        private byte GET_CY()
        {
            return (byte)((psw & 0x80) >> 7);
        }
        private byte GET_AC()
        {
            return (byte)((psw & 0x40) >> 6);
        }
        private byte GET_FO()
        {
            return (byte)((psw & 0x20) >> 5);
        }
        private byte GET_RS()
        {
            return (byte)((psw & 0x18) >> 3);
        }
        private byte GET_OV()
        {
            return (byte)((psw & 0x04) >> 2);
        }
        private byte GET_P()
        {
            return (byte)((psw & 0x01) >> 0);
        }
        private byte GET_EA()
        {
            return (byte)((ie & 0x80) >> 7);
        }
        private byte GET_ET2()
        {
            return (byte)((ie & 0x20) >> 5);
        }
        private byte GET_ES()
        {
            return (byte)((ie & 0x10) >> 4);
        }
        private byte GET_ET1()
        {
            return (byte)((ie & 0x08) >> 3);
        }
        private byte GET_EX1()
        {
            return (byte)((ie & 0x04) >> 2);
        }
        private byte GET_ET0()
        {
            return (byte)((ie & 0x02) >> 1);
        }
        private byte GET_EX0()
        {
            return (byte)((ie & 0x01) >> 0);
        }
        private byte GET_PT2()
        {
            return (byte)((ip & 0x20) >> 5);
        }
        private byte GET_PS()
        {
            return (byte)((ip & 0x10) >> 4);
        }
        private byte GET_PT1()
        {
            return (byte)((ip & 0x08) >> 3);
        }
        private byte GET_PX1()
        {
            return (byte)((ip & 0x04) >> 2);
        }
        private byte GET_PT0()
        {
            return (byte)((ip & 0x02) >> 1);
        }
        private byte GET_PX0()
        {
            return (byte)((ip & 0x01) >> 0);
        }
        private byte GET_TF1()
        {
            return (byte)((tcon & 0x80) >> 7);
        }
        private byte GET_TR1()
        {
            return (byte)((tcon & 0x40) >> 6);
        }
        private byte GET_TF0()
        {
            return (byte)((tcon & 0x20) >> 5);
        }
        private byte GET_TR0()
        {
            return (byte)((tcon & 0x10) >> 4);
        }
        private byte GET_IE1()
        {
            return (byte)((tcon & 0x08) >> 3);
        }
        private byte GET_IT1()
        {
            return (byte)((tcon & 0x04) >> 2);
        }
        private byte GET_IE0()
        {
            return (byte)((tcon & 0x02) >> 1);
        }
        private byte GET_IT0()
        {
            return (byte)((tcon & 0x01) >> 0);
        }
        private byte GET_SM0()
        {
            return (byte)((scon & 0x80) >> 7);
        }
        private byte GET_SM1()
        {
            return (byte)((scon & 0x40) >> 6);
        }
        private byte GET_SM2()
        {
            return (byte)((scon & 0x20) >> 5);
        }
        private byte GET_REN()
        {
            return (byte)((scon & 0x10) >> 4);
        }
        private byte GET_TB8()
        {
            return (byte)((scon & 0x08) >> 3);
        }
        private byte GET_RB8()
        {
            return (byte)((scon & 0x04) >> 2);
        }
        private byte GET_TI()
        {
            return (byte)((scon & 0x02) >> 1);
        }
        private byte GET_RI()
        {
            return (byte)((scon & 0x01) >> 0);
        }
        private byte GET_GATE1()
        {
            return (byte)((tmod & 0x80) >> 7);
        }
        private byte GET_CT1()
        {
            return (byte)((tmod & 0x40) >> 6);
        }
        private byte GET_M1_1()
        {
            return (byte)((tmod & 0x20) >> 5);
        }
        private byte GET_M1_0()
        {
            return (byte)((tmod & 0x10) >> 4);
        }
        private byte GET_GATE0()
        {
            return (byte)((tmod & 0x08) >> 3);
        }
        private byte GET_CT0()
        {
            return (byte)((tmod & 0x04) >> 2);
        }
        private byte GET_M0_1()
        {
            return (byte)((tmod & 0x02) >> 1);
        }
        private byte GET_M0_0()
        {
            return (byte)((tmod & 0x01) >> 0);
        }
        private byte GET_TF2()
        {
            return (byte)((t2con & 0x80) >> 7);
        }
        private byte GET_EXF2()
        {
            return (byte)((t2con & 0x40) >> 6);
        }
        private byte GET_RCLK()
        {
            return (byte)((t2con & 0x20) >> 5);
        }
        private byte GET_TCLK()
        {
            return (byte)((t2con & 0x10) >> 4);
        }
        private byte GET_EXEN2()
        {
            return (byte)((t2con & 0x08) >> 3);
        }
        private byte GET_TR2()
        {
            return (byte)((t2con & 0x04) >> 2);
        }
        private byte GET_CT2()
        {
            return (byte)((t2con & 0x02) >> 1);
        }
        private byte GET_CP()
        {
            return (byte)((t2con & 0x01) >> 0);
        }
        private bool SERIALPORT_IRQ()
        {
            return ((scon & 0x03) != 0 && GET_ES() != 0);
        }
        private bool NO_PENDING_IRQ()
        {
            return ((tcon & 0xaa) == 0 && SERIALPORT_IRQ() == false);
        }

        public void Init(int clock, Func<int, int> _irqcallback)
        {
            irq_callback = _irqcallback;
        }
        public override void Reset()
        {
            int i;
            Func<int, int> save_irqcallback;
            save_irqcallback = irq_callback;
            ppc = 0;
            pc = 0;
            subtype = 0;
            cur_irq = 0;
            irq_priority = 0;
            rwm = 0;
            prev_used_cycles = 0;
            last_int0 = 0;
            last_int1 = 0;
            int_vec = 0;
            priority_request = 0;
            po = 0;
            sp = 0;
            dpl = 0;
            dph = 0;
            pcon = 0;
            tcon = 0;
            tmod = 0;
            tl0 = 0;
            tl1 = 0;
            th0 = 0;
            th1 = 0;
            p1 = 0;
            scon = 0;
            sbuf = 0;
            p2 = 0;
            ie = 0;
            p3 = 0;
            ip = 0;
            t2con = 0;
            rcap2l = 0;
            rcap2h = 0;
            tl2 = 0;
            th2 = 0;
            psw = 0;
            acc = 0;
            b = 0;
            irq_callback = save_irqcallback;
            uart.timerbaud = 0;
            uart.sending = 0;
            uart.data_out = 0;
            uart.bits_to_send = 0;
            uart.bitcycles = 0;
            subtype = 8051;
            iram_iread = internal_ram_read;
            iram_iwrite = internal_ram_write;
            serial_tx_callback = hold_serial_tx_callback;
            hold_serial_tx_callback = null;
            serial_rx_callback = hold_serial_rx_callback;
            hold_serial_rx_callback = null;
            eram_iaddr_callback = hold_eram_iaddr_callback;
            hold_eram_iaddr_callback = null;
            for (i = 0; i < 0x100; i++)
            {
                IntRam[i] = 0xff;
            }
            pc = 0;
            sfr_write(SP, 0x7);
            sfr_write(PSW, 0);
            sfr_write(DPH, 0);
            sfr_write(DPL, 0);
            sfr_write(ACC, 0);
            sfr_write(B, 0);
            sfr_write(IP, 0);
            sfr_write(IE, 0);
            sfr_write(SCON, 0);
            sfr_write(TCON, 0);
            sfr_write(TMOD, 0);
            sfr_write(TH1, 0);
            sfr_write(TH0, 0);
            sfr_write(TL1, 0);
            sfr_write(TL0, 0);
            sfr_write(P3, 0xff);
            sfr_write(P2, 0xff);
            sfr_write(P1, 0xff);
            sfr_write(P0, 0xff);
            CLEAR_CURRENT_IRQ();
        }
        public override int ExecuteCycles(int cycles)
        {
            pendingCycles = cycles;
            do
            {
                byte op = ReadOp(pc);
                ppc = pc;
                if (pc != ppc)
                {
                    op = ReadOp(pc);
                }
                if ((GET_TR0()!=0 && GET_ET0()!=0) || (GET_TR1()!=0 && GET_ET1()!=0))
                {
                    update_timer(prev_used_cycles);
                }
                if (uart.sending != 0)
                {
                    update_serial(prev_used_cycles);
                }
                pc += 1;
                pendingCycles -= (i8051_cycles[op]);
                switch (op)
                {
                    case 0x00:
                        nop();
                        break;
                    case 0x01:
                        ajmp();
                        break;
                    case 0x02:
                        ljmp();
                        break;
                    case 0x03:
                        rr_a();
                        break;
                    case 0x04:
                        inc_a();
                        break;
                    case 0x05:
                        rwm = 1;
                        inc_mem();
                        rwm = 0;
                        break;
                    case 0x06:
                    case 0x07:
                        inc_ir(op & 1);
                        break;
                    case 0x08:
                    case 0x09:
                    case 0x0a:
                    case 0x0b:
                    case 0x0c:
                    case 0x0d:
                    case 0x0e:
                    case 0x0f:
                        inc_r(op & 7);
                        break;
                    case 0x10:
                        rwm = 1;
                        jbc();
                        rwm = 0;
                        break;
                    case 0x11:
                        acall();
                        break;
                    case 0x12:
                        lcall();
                        break;
                    case 0x13:
                        rrc_a();
                        break;
                    case 0x14:
                        dec_a();
                        break;
                    case 0x15:
                        rwm = 1;
                        dec_mem();
                        rwm = 0;
                        break;
                    case 0x16:
                    case 0x17:
                        dec_ir(op & 1);
                        break;
                    case 0x18:
                    case 0x19:
                    case 0x1a:
                    case 0x1b:
                    case 0x1c:
                    case 0x1d:
                    case 0x1e:
                    case 0x1f:
                        dec_r(op & 7);
                        break;
                    case 0x20:
                        jb();
                        break;
                    case 0x21:
                        ajmp();
                        break;
                    case 0x22:
                        ret();
                        break;
                    case 0x23:
                        rl_a();
                        break;
                    case 0x24:
                        add_a_byte();
                        break;
                    case 0x25:
                        add_a_mem();
                        break;
                    case 0x26:
                    case 0x27:
                        add_a_ir(op & 1);
                        break;
                    case 0x28:
                    case 0x29:
                    case 0x2a:
                    case 0x2b:
                    case 0x2c:
                    case 0x2d:
                    case 0x2e:
                    case 0x2f:
                        add_a_r(op & 7);
                        break;
                    case 0x30:
                        jnb();
                        break;
                    case 0x31:
                        acall();
                        break;
                    case 0x32:
                        reti();
                        break;
                    case 0x33:
                        rlc_a();
                        break;
                    case 0x34:
                        addc_a_byte();
                        break;
                    case 0x35:
                        addc_a_mem();
                        break;
                    case 0x36:
                    case 0x37:
                        addc_a_ir(op & 1);
                        break;
                    case 0x38:
                    case 0x39:
                    case 0x3a:
                    case 0x3b:
                    case 0x3c:
                    case 0x3d:
                    case 0x3e:
                    case 0x3f:
                        addc_a_r(op & 7);
                        break;
                    case 0x40:
                        jc();
                        break;
                    case 0x41:
                        ajmp();
                        break;
                    case 0x42:
                        rwm = 1;
                        orl_mem_a();
                        rwm = 0;
                        break;
                    case 0x43:
                        rwm = 1;
                        orl_mem_byte();
                        rwm = 0;
                        break;
                    case 0x44:
                        orl_a_byte();
                        break;
                    case 0x45:
                        orl_a_mem();
                        break;
                    case 0x46:
                    case 0x47:
                        orl_a_ir(op & 1);
                        break;
                    case 0x48:
                    case 0x49:
                    case 0x4a:
                    case 0x4b:
                    case 0x4c:
                    case 0x4d:
                    case 0x4e:
                    case 0x4f:
                        orl_a_r(op & 7);
                        break;
                    case 0x50:
                        jnc();
                        break;
                    case 0x51:
                        acall();
                        break;
                    case 0x52:
                        rwm = 1;
                        anl_mem_a();
                        rwm = 0;
                        break;
                    case 0x53:
                        rwm = 1;
                        anl_mem_byte();
                        rwm = 0;
                        break;
                    case 0x54:
                        anl_a_byte();
                        break;
                    case 0x55:
                        anl_a_mem();
                        break;
                    case 0x56:
                    case 0x57:
                        anl_a_ir(op & 1);
                        break;
                    case 0x58:
                    case 0x59:
                    case 0x5a:
                    case 0x5b:
                    case 0x5c:
                    case 0x5d:
                    case 0x5e:
                    case 0x5f:
                        anl_a_r(op & 7);
                        break;
                    case 0x60:
                        jz();
                        break;
                    case 0x61:
                        ajmp();
                        break;
                    case 0x62:
                        rwm = 1;
                        xrl_mem_a();
                        rwm = 0;
                        break;
                    case 0x63:
                        rwm = 1;
                        xrl_mem_byte();
                        rwm = 0;
                        break;
                    case 0x64:
                        xrl_a_byte();
                        break;
                    case 0x65:
                        xrl_a_mem();
                        break;
                    case 0x66:
                    case 0x67:
                        xrl_a_ir(op & 1);
                        break;
                    case 0x68:
                    case 0x69:
                    case 0x6a:
                    case 0x6b:
                    case 0x6c:
                    case 0x6d:
                    case 0x6e:
                    case 0x6f:
                        xrl_a_r(op & 7);
                        break;
                    case 0x70:
                        jnz();
                        break;
                    case 0x71:
                        acall();
                        break;
                    case 0x72:
                        orl_c_bitaddr();
                        break;
                    case 0x73:
                        jmp_iadptr();
                        break;
                    case 0x74:
                        mov_a_byte();
                        break;
                    case 0x75:
                        mov_mem_byte();
                        break;
                    case 0x76:
                    case 0x77:
                        mov_ir_byte(op & 1);
                        break;
                    case 0x78:
                    case 0x79:
                    case 0x7a:
                    case 0x7b:
                    case 0x7c:
                    case 0x7d:
                    case 0x7e:
                    case 0x7f:
                        mov_r_byte(op & 7);
                        break;
                    case 0x80:
                        sjmp();
                        break;
                    case 0x81:
                        ajmp();
                        break;
                    case 0x82:
                        anl_c_bitaddr();
                        break;
                    case 0x83:
                        movc_a_iapc();
                        break;
                    case 0x84:
                        div_ab();
                        break;
                    case 0x85:
                        mov_mem_mem();
                        break;
                    case 0x86:
                    case 0x87:
                        mov_mem_ir(op & 1);
                        break;
                    case 0x88:
                    case 0x89:
                    case 0x8a:
                    case 0x8b:
                    case 0x8c:
                    case 0x8d:
                    case 0x8e:
                    case 0x8f:
                        mov_mem_r(op & 7);
                        break;
                    case 0x90:
                        mov_dptr_byte();
                        break;
                    case 0x91:
                        acall();
                        break;
                    case 0x92:
                        rwm = 1;
                        mov_bitaddr_c();
                        rwm = 0;
                        break;
                    case 0x93:
                        movc_a_iadptr();
                        break;
                    case 0x94:
                        subb_a_byte();
                        break;
                    case 0x95:
                        subb_a_mem();
                        break;
                    case 0x96:
                    case 0x97:
                        subb_a_ir(op & 1);
                        break;
                    case 0x98:
                    case 0x99:
                    case 0x9a:
                    case 0x9b:
                    case 0x9c:
                    case 0x9d:
                    case 0x9e:
                    case 0x9f:
                        subb_a_r(op & 7);
                        break;
                    case 0xa0:
                        orl_c_nbitaddr();
                        break;
                    case 0xa1:
                        ajmp();
                        break;
                    case 0xa2:
                        mov_c_bitaddr();
                        break;
                    case 0xa3:
                        inc_dptr();
                        break;
                    case 0xa4:
                        mul_ab();
                        break;
                    case 0xa5:
                        illegal();
                        break;
                    case 0xa6:
                    case 0xa7:
                        mov_ir_mem(op & 1);
                        break;
                    case 0xa8:
                    case 0xa9:
                    case 0xaa:
                    case 0xab:
                    case 0xac:
                    case 0xad:
                    case 0xae:
                    case 0xaf:
                        mov_r_mem(op & 7);
                        break;
                    case 0xb0:
                        anl_c_nbitaddr();
                        break;
                    case 0xb1:
                        acall();
                        break;
                    case 0xb2:
                        rwm = 1;
                        cpl_bitaddr();
                        rwm = 0;
                        break;
                    case 0xb3:
                        cpl_c();
                        break;
                    case 0xb4:
                        cjne_a_byte();
                        break;
                    case 0xb5:
                        cjne_a_mem();
                        break;
                    case 0xb6:
                    case 0xb7:
                        cjne_ir_byte(op & 1);
                        break;
                    case 0xb8:
                    case 0xb9:
                    case 0xba:
                    case 0xbb:
                    case 0xbc:
                    case 0xbd:
                    case 0xbe:
                    case 0xbf:
                        cjne_r_byte(op & 7);
                        break;
                    case 0xc0:
                        push();
                        break;
                    case 0xc1:
                        ajmp();
                        break;
                    case 0xc2:
                        rwm = 1;
                        clr_bitaddr();
                        rwm = 0;
                        break;
                    case 0xc3:
                        clr_c();
                        break;
                    case 0xc4:
                        swap_a();
                        break;
                    case 0xc5:
                        xch_a_mem();
                        break;
                    case 0xc6:
                    case 0xc7:
                        xch_a_ir(op & 1);
                        break;
                    case 0xc8:
                    case 0xc9:
                    case 0xca:
                    case 0xcb:
                    case 0xcc:
                    case 0xcd:
                    case 0xce:
                    case 0xcf:
                        xch_a_r(op & 7);
                        break;
                    case 0xd0:
                        pop();
                        break;
                    case 0xd1:
                        acall();
                        break;
                    case 0xd2:
                        rwm = 1;
                        setb_bitaddr();
                        rwm = 0;
                        break;
                    case 0xd3:
                        setb_c();
                        break;
                    case 0xd4:
                        da_a();
                        break;
                    case 0xd5:
                        rwm = 1;
                        djnz_mem();
                        rwm = 0;
                        break;
                    case 0xd6:
                    case 0xd7:
                        xchd_a_ir(op & 1);
                        break;
                    case 0xd8:
                    case 0xd9:
                    case 0xda:
                    case 0xdb:
                    case 0xdc:
                    case 0xdd:
                    case 0xde:
                    case 0xdf:
                        djnz_r(op & 7);
                        break;
                    case 0xe0:
                        movx_a_idptr();
                        break;
                    case 0xe1:
                        ajmp();
                        break;
                    case 0xe2:
                    case 0xe3:
                        movx_a_ir(op & 1);
                        break;
                    case 0xe4:
                        clr_a();
                        break;
                    case 0xe5:
                        mov_a_mem();
                        break;
                    case 0xe6:
                    case 0xe7:
                        mov_a_ir(op & 1);
                        break;
                    case 0xe8:
                    case 0xe9:
                    case 0xea:
                    case 0xeb:
                    case 0xec:
                    case 0xed:
                    case 0xee:
                    case 0xef:
                        mov_a_r(op & 7);
                        break;
                    case 0xf0:
                        movx_idptr_a();
                        break;
                    case 0xf1:
                        acall();
                        break;
                    case 0xf2:
                    case 0xf3:
                        movx_ir_a(op & 1);
                        break;
                    case 0xf4:
                        cpl_a();
                        break;
                    case 0xf5:
                        mov_mem_a();
                        break;
                    case 0xf6:
                    case 0xf7:
                        mov_ir_a(op & 1);
                        break;
                    case 0xf8:
                    case 0xf9:
                    case 0xfa:
                    case 0xfb:
                    case 0xfc:
                    case 0xfd:
                    case 0xfe:
                    case 0xff:
                        mov_r_a(op & 7);
                        break;
                    default:
                        illegal();
                        break;
                }
                prev_used_cycles = i8051_cycles[op];
                pendingCycles -= check_interrupts();
            }
            while (pendingCycles > 0);
            return cycles - pendingCycles;
        }
        public byte check_interrupts()
        {
            if (GET_EA() == 0)
            {
                return 0;
            }
            if (NO_PENDING_IRQ())
            {
                return 0;
            }
            if (irq_priority != 0)
            {
                return 0;
            }
            if (GET_IE0() != 0)
            {
                int_vec = 0x003;
                priority_request = GET_PX0();
            }
            if (priority_request == 0 && GET_TF0() != 0 && (int_vec == 0 || (int_vec != 0 && GET_PT0() != 0)))
            {
                int_vec = 0x00b;
                priority_request = GET_PT0();
            }
            if (priority_request == 0 && GET_IE1() != 0 && (int_vec == 0 || (int_vec != 0 && GET_PX1() != 0)))
            {
                int_vec = 0x013;
                priority_request = GET_PX1();
            }
            if (priority_request == 0 && GET_TF1() != 0 && (int_vec == 0 || (int_vec != 0 && GET_PT1() != 0)))
            {
                int_vec = 0x01b;
                priority_request = GET_PT1();
            }
            if (priority_request == 0 && GET_ES() != 0 && (GET_TI() != 0 || GET_RI() != 0) && (int_vec == 0 || (int_vec != 0 && GET_PS() != 0)))
            {
                int_vec = 0x023;
                priority_request = GET_PS();
            }
            if (priority_request == 0 && GET_ET2() != 0 && (GET_TF2() != 0 || GET_EXF2() != 0) && (int_vec == 0 || (int_vec != 0 && GET_PT2() != 0)))
            {
                int_vec = 0x02b; ;
                priority_request = GET_PT2();
            }
            if (cur_irq < 0xff && priority_request==0)
            {
                return 0;
            }
            push_pc();
            pc = int_vec;
            cur_irq = int_vec;
            irq_priority = priority_request;
            switch (cur_irq)
            {
                case 0x003:
                    SET_IE0(0);
                    break;
                case 0x00b:
                    SET_TF0(0);
                    break;
                case 0x013:
                    SET_IE1(0);
                    break;
                case 0x01b:
                    SET_TF1(0);
                    break;
                case 0x023:
                    break;
                case 0x02b:
                    break;
            }
            int_vec = 0;
            priority_request = 0;
            return 24;
        }
        public void sfr_write(int offset, byte data)
        {
            data &= 0xff;
            switch (offset)
            {
                case 0x80:
                    po = data;
                    WriteIO(0, data);
                    break;
                case 0x81:
                    sp = (byte)(data & 0xff);
                    break;
                case 0x82: dpl = data; break;
                case 0x83: dph = data; break;
                case 0x87: pcon = data; break;
                case 0x88: tcon = data; break;
                case 0x89: tmod = data; break;
                case 0x8a: tl0 = data; break;
                case 0x8b: tl1 = data; break;
                case 0x8c: th0 = data; break;
                case 0x8d: th1 = data; break;
                case 0x90:
                    p1 = data;
                    WriteIO(1, data);
                    break;
                case 0x98:
                    {
                        scon = data;
                        break;
                    }
                case 0x99:
                    serial_transmit(data);
                    break;
                case 0xa0:
                    p2 = data;
                    WriteIO(2, data);
                    break;
                case 0xa8: ie = data; break;
                case 0xb0:
                    p3 = data;
                    WriteIO(3, data);
                    break;
                case 0xb8: ip = data; break;
                case 0xc8: t2con = data; break;
                case 0xca: rcap2l = data; break;
                case 0xcb: rcap2h = data; break;
                case 0xcc: tl2 = data; break;
                case 0xcd: th2 = data; break;
                case 0xd0:
                    psw = data;
                    set_parity();
                    break;
                case 0xe0:
                    acc = data;
                    set_parity();
                    break;
                case 0xf0: b = data; break;
                default:
                    break;
            }
        }
        public byte sfr_read(int offset)
        {
            switch (offset)
            {
                case 0x80:
                    if (rwm != 0)
                        return po;
                    else
                        return ReadIO(0);
                case 0x81: return sp;
                case 0x82: return dpl;
                case 0x83: return dph;
                case 0x87: return pcon;
                case 0x88: return tcon;
                case 0x89: return tmod;
                case 0x8a: return tl0;
                case 0x8b: return tl1;
                case 0x8c: return th0;
                case 0x8d: return th1;
                case 0x90:
                    if (rwm != 0)
                        return p1;
                    else
                        return ReadIO(1);
                case 0x98: return scon;
                case 0x99: return sbuf;
                case 0xa0:
                    if (rwm != 0)
                        return p2;
                    else
                        return ReadIO(2);
                case 0xa8: return ie;
                case 0xb0:
                    if (rwm != 0)
                        return p3;
                    else
                        return ReadIO(3);
                case 0xb8: return ip;
                case 0xc8: return t2con;
                case 0xca: return rcap2l;
                case 0xcb: return rcap2h;
                case 0xcc: return tl2;
                case 0xcd: return th2;
                case 0xd0: return psw;
                case 0xe0: return acc;
                case 0xf0: return b;
                default:
                    break;
            }
            return 0xff;
        }
        private byte internal_ram_read(int offset)
        {
            if (offset < 0x80)
            {
                return IntRam[offset];
            }
            else
            {
                if (offset < 0x100)
                {
                    return sfr_read(offset);
                }
            }
            return 0xff;
        }
        private void internal_ram_write(int offset, byte data)
        {
            data &= 0xff;
            if (offset < 0x80)
            {
                IntRam[offset] = data;
            }
            else
            {
                if (offset < 0x100)
                {
                    sfr_write(offset, data);
                }
            }
        }
        private byte internal_ram_iread(int offset)
        {
            return iram_iread(offset);
        }
        private void internal_ram_iwrite(int offset, byte data)
        {
            iram_iwrite(offset, data);
        }
        private uint external_ram_iaddr(int offset,uint mem_mask)
        {
            if (eram_iaddr_callback != null)
            {
                return eram_iaddr_callback(offset,mem_mask);
            }
            return (uint)offset;
        }
        private void push_pc()
        {
            byte tmpSP = sp;
            tmpSP++;
            sfr_write(0x81, tmpSP);
            if (tmpSP == sp)
            {
                internal_ram_iwrite(tmpSP, (byte)(pc & 0xff));
            }
            tmpSP = sp;
            tmpSP++;
            sfr_write(0x81, tmpSP);
            if (tmpSP == sp)
            {
                internal_ram_iwrite(tmpSP, (byte)((pc & 0xff00) >> 8));
            }
        }
        private void pop_pc()
        {
            byte tmpSP = sp;
            pc = (ushort)((internal_ram_iread(tmpSP) & 0xff) << 8);
            tmpSP = (byte)(sp - 1);
            sfr_write(0x81, tmpSP);
            if (tmpSP == sp)
            {
                pc = (ushort)(pc | internal_ram_iread(tmpSP));
            }
            sfr_write(0x81, (byte)(tmpSP - 1));
        }
        private void CLEAR_CURRENT_IRQ()
        {
            cur_irq = 0xff;
            irq_priority = 0;
        }
        private void set_parity()
        {
            int i, p = 0;
            for (i = 1; i <= 128; i = i * 2)
            {
                if ((acc & i) != 0)
                {
                    p++;
                }
            }
            SET_P(p & 1);
        }
        private byte bit_address_r(int offset)
        {
            int word;
            int mask;
            int bit_pos;
            int base1;
            int distance;
            offset &= 0xff;
            if (offset < 0x80)
            {
                base1 = 0x20;
                distance = 1;
            }
            else
            {
                base1 = 0x80;
                distance = 8;
            }
            word = ((offset & 0x78) >> 3) * distance + base1;
            bit_pos = offset & 0x7;
            mask = 0x1 << bit_pos;
            return (byte)((internal_ram_read(word) & mask) >> bit_pos);
        }
        private void bit_address_w(int offset, byte data)
        {
            int word;
            int mask;
            int bit_pos;
            int result;
            int base1;
            int distance;
            offset &= 0xff;
            if (offset < 0x80)
            {
                base1 = 0x20;
                distance = 1;
            }
            else
            {
                base1 = 0x80;
                distance = 8;
            }
            word = ((offset & 0x78) >> 3) * distance + base1;
            bit_pos = offset & 0x7;
            data = (byte)((data & 0x1) << bit_pos);
            mask = ~(1 << bit_pos) & 0xff;
            result = internal_ram_read(word) & mask;
            result = result | data;
            internal_ram_write(word, (byte)result);
        }
        private byte R_R(int n)
        {
            return IntRam[(GET_RS() * 8) + n];
        }
        private ushort R_DPTR()
        {
            return (ushort)((dph << 8) | dpl);
        }
        private void DPTR_W(int n)
        {
            sfr_write(0x83, (byte)((n >> 8) & 0xff));
            sfr_write(0x82, (byte)(n & 0xff));
        }
        private void do_add_flags(byte a, byte data, byte c)
        {
            ushort result = (ushort)(a + data + c);
            short result1 = (short)((sbyte)a + (sbyte)data + c);
            int cy, ac, ov;
            cy = (result & 0x100) >> 8;
            result = (ushort)((a & 0x0f) + (data & 0x0f) + c);
            ac = (result & 0x10) >> 4;
            ov = (result1 < -128 || result1 > 127)?1:0;
            SET_CY(cy);
            SET_AC(ac);
            SET_OV(ov);
        }
        private void do_sub_flags(byte a, byte data, byte c)
        {
            ushort result = (ushort)(a - (data + c));
            short result1 = (short)((sbyte)a - (sbyte)(data + c));
            int cy, ac, ov;
            cy = (result & 0x100) >> 8;
            result = (ushort)((a & 0x0f) - ((data & 0x0f) + c));
            ac = (result & 0x10) >> 4;
            ov = (result1 < -128 || result1 > 127) ? 1 : 0;
            SET_CY(cy);
            SET_AC(ac);
            SET_OV(ov);
        }
        private void update_timer(int cyc)
        {
            if (GET_TR0() != 0)
            {
                int mode = (GET_M0_1() << 1) | GET_M0_0();
                uint count = 0;
                switch (mode)
                {
                    case 0:
                        count = (uint)((th0 << 5) | (tl0 & 0x1F));
                        if (GET_GATE0() != 0 && GET_IE0() != 0)
                        {
                            count += (uint)(cyc / 12);
                        }
                        if (GET_CT0() != 0)
                        {

                        }
                        if (GET_GATE0() == 0 && GET_CT0() == 0)
                        {
                            count += (uint)(cyc / 12);
                        }
                        if ((count & 0xffffe000) != 0)
                        {
                            SET_TF0(1);
                        }
                        th0 = (byte)((count >> 5) & 0xff);
                        tl0 = (byte)(count & 0x1f);
                        break;
                    case 1:
                        count = (uint)((th0 << 8) | tl0);
                        if (GET_GATE0() != 0 && GET_IE0() != 0)
                        {
                            count += (uint)(cyc / 12);
                        }
                        if (GET_CT0() != 0)
                        {

                        }
                        if (GET_GATE0() == 0 && GET_CT0() == 1)
                        {
                            count += (uint)(cyc / 12);
                        }
                        if ((count & 0xffff0000) != 0)
                        {
                            SET_TF0(1);
                        }
                        th0 = (byte)((count >> 8) & 0xff);
                        tl0 = (byte)(count & 0xff);
                        break;
                    case 2:
                        count = tl0;
                        count += (uint)(cyc / 12);
                        if ((count & 0xffffff00) != 0)
                        {
                            SET_TF0(1);
                            count += th0;
                        }
                        tl0 = (byte)(count & 0xff);
                        break;
                    case 3:
                        count = tl0;
                        count += (uint)(cyc / 12);
                        if ((count & 0xffffff00) != 0)
                        {
                            SET_TF0(1);
                        }
                        tl0 = (byte)(count & 0xff);
                        count = th0;
                        count += (uint)(cyc / 12);
                        if ((count & 0xffffff00) != 0)
                        {
                            SET_TF1(1);
                        }
                        th0 = (byte)(count & 0xff);
                        break;
                }
            }
            if (GET_TR1() != 0)
            {
                int mode = (GET_M1_1() << 1) | GET_M1_0();
                uint count = 0;
                switch (mode)
                {
                    case 0:
                        count = (uint)((th1 << 5) | (tl1 & 0x1F));
                        if (GET_GATE1() != 0 && GET_IE1() != 0)
                        {
                            count += (uint)(cyc / 12);
                        }
                        if (GET_CT1() != 0)
                        {

                        }
                        if (GET_GATE1() == 0 && GET_CT1() == 0)
                        {
                            count += (uint)(cyc / 12);
                        }
                        if ((count & 0xffffe000) != 0)
                        {
                            SET_TF1(1);
                        }
                        th1 = (byte)((count >> 5) & 0xff);
                        tl1 = (byte)(count & 0x1f);
                        break;
                    case 1:
                        count = (uint)((th1 << 8) | tl1);
                        if (GET_GATE1() != 0 && GET_IE1() != 0)
                        {
                            count += (uint)(cyc / 12);
                        }
                        if (GET_CT1() != 0)
                        {

                        }
                        if (GET_GATE1() == 0 && GET_CT1() == 0)
                        {
                            count += (uint)(cyc / 12);
                        }
                        if ((count & 0xffff0000) != 0)
                        {
                            SET_TF1(1);
                        }
                        th1 = (byte)((count >> 8) & 0xff);
                        tl1 = (byte)(count & 0xff);
                        break;
                    case 2:
                        count = tl1;
                        count += (uint)(cyc / 12);
                        if ((count & 0xffffff00) != 0)
                        {
                            SET_TF1(1);
                            count += th1;
                        }
                        tl1 = (byte)(count & 0xff);
                        break;
                }
            }
            if (GET_TR2() != 0)
            {
                uint count = (uint)((th2 << 8) | tl2);
                if (GET_CT2() != 0)
                {

                }
                else
                {
                    if (GET_TCLK() != 0 || GET_RCLK() != 0)
                    {
                        count += (uint)(cyc / 2);
                    }
                    else
                    {
                        count += (uint)(cyc / 12);
                    }
                    if ((count & 0xffff0000) != 0)
                    {
                        if (GET_TCLK() == 0 && GET_RCLK() == 0)
                        {
                            SET_TF2(1);
                        }
                        else
                        {
                            if (uart.sending != 0 && uart.bits_to_send != 0 && uart.timerbaud != 0)
                            {
                                uart.bits_to_send -= 1;
                            }
                        }
                        if (GET_CP() == 0)
                        {
                            count += (uint)((rcap2h << 8) | rcap2l);
                        }
                    }
                    th2 = (byte)((count >> 8) & 0xff);
                    tl2 = (byte)(count & 0xff);
                }
            }
        }
        private void serial_transmit(byte data)
        {
            int mode = (GET_SM0() << 1) | GET_SM1();
            uart.sending = 1;
            uart.data_out = data;
            switch (mode)
            {
                case 0:
                    uart.timerbaud = 0;
                    uart.bitcycles = 0;
                    uart.bits_to_send = 8 + 2;
                    break;
                case 1:
                    uart.timerbaud = 1;
                    uart.bits_to_send = 8 + 2;
                    break;
                case 2:
                case 3:
                    break;
            }
        }
        private void update_serial(int cyc)
        {
            if (uart.bits_to_send != 0)
            {
                if (uart.timerbaud != 0)
                {

                }
                else
                {
                    uart.bitcycles += (byte)cyc;
                    if (uart.bitcycles > 11)
                    {
                        int bits_sent = uart.bitcycles / 12;
                        int diff = uart.bitcycles % 12;
                        if (bits_sent > uart.bits_to_send)
                        {
                            bits_sent = uart.bits_to_send;
                            diff = 0;
                        }
                        uart.bits_to_send -= (byte)bits_sent;
                        uart.bitcycles = (byte)diff;
                    }
                }
            }
            if (uart.bits_to_send == 0)
            {
                uart.sending = 0;
                uart.bitcycles = 0;
                if (serial_tx_callback != null)
                {
                    serial_tx_callback(uart.data_out);
                }
                SET_TI(1);
            }
        }
        public override void SaveStateBinary(BinaryWriter writer)
        {
            writer.Write(ppc);
            writer.Write(pc);
            writer.Write(subtype);
            writer.Write(rwm);
            writer.Write(cur_irq);
            writer.Write(irq_priority);
            writer.Write(prev_used_cycles);
            writer.Write(last_int0);
            writer.Write(last_int1);
            writer.Write(int_vec);
            writer.Write(priority_request);
            writer.Write(po);
            writer.Write(sp);
            writer.Write(dpl);
            writer.Write(dph);
            writer.Write(pcon);
            writer.Write(tcon);
            writer.Write(tmod);
            writer.Write(tl0);
            writer.Write(tl1);
            writer.Write(th0);
            writer.Write(th1);
            writer.Write(p1);
            writer.Write(scon);
            writer.Write(sbuf);
            writer.Write(p2);
            writer.Write(ie);
            writer.Write(p3);
            writer.Write(ip);
            writer.Write(t2con);
            writer.Write(rcap2l);
            writer.Write(rcap2h);
            writer.Write(tl2);
            writer.Write(th2);
            writer.Write(psw);
            writer.Write(acc);
            writer.Write(b);
            writer.Write(IntRam, 0, 0x100);
        }
        public override void LoadStateBinary(BinaryReader reader)
        {
            ppc = reader.ReadUInt16();
            pc = reader.ReadUInt16();
            subtype = reader.ReadUInt16();
            rwm = reader.ReadByte();
            cur_irq = reader.ReadByte();
            irq_priority = reader.ReadByte();
            prev_used_cycles = reader.ReadByte();
            last_int0 = reader.ReadByte();
            last_int1 = reader.ReadByte();
            int_vec = reader.ReadByte();
            priority_request = reader.ReadByte();
            po = reader.ReadByte();
            sp = reader.ReadByte();
            dpl = reader.ReadByte();
            dph = reader.ReadByte();
            pcon = reader.ReadByte();
            tcon = reader.ReadByte();
            tmod = reader.ReadByte();
            tl0 = reader.ReadByte();
            tl1 = reader.ReadByte();
            th0 = reader.ReadByte();
            th1 = reader.ReadByte();
            p1 = reader.ReadByte();
            scon = reader.ReadByte();
            sbuf = reader.ReadByte();
            p2 = reader.ReadByte();
            ie = reader.ReadByte();
            p3 = reader.ReadByte();
            ip = reader.ReadByte();
            t2con = reader.ReadByte();
            rcap2l = reader.ReadByte();
            rcap2h = reader.ReadByte();
            tl2 = reader.ReadByte();
            th2 = reader.ReadByte();
            psw = reader.ReadByte();
            acc = reader.ReadByte();
            b = reader.ReadByte();
            IntRam = reader.ReadBytes(0x100);
        }
    }
}
