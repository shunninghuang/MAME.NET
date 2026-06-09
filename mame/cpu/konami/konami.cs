using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using mame;

namespace cpu.konami
{
    public sealed partial class KonamiCpu : cpuexec_data
    {
        public static KonamiCpu k1;
        public Action[] konami_main, konami_indexed, konami_direct, konami_extended;
        public RegisterPair PC;
        public RegisterPair PPC;
        public RegisterPair D;
        public RegisterPair DP;
        public RegisterPair U, S;
        public RegisterPair X, Y;
        public byte CC;
        public byte ireg;
        public LineState[] irq_state = new LineState[2];
        public int extra_cycles;
        public byte int_state;
        public LineState nmi_state;
        public byte CC_C = 0x01, CC_V = 0x02, CC_Z = 0x04, CC_N = 0x08, CC_II = 0x10, CC_H = 0x20, CC_IF = 0x40, CC_E = 0x80;
        public RegisterPair EA;
        public byte KONAMI_CWAI=8,KONAMI_SYNC=16, KONAMI_LDS=32;
        public byte KONAMI_IRQ_LINE = 0, KONAMI_FIRQ_LINE = 1;
        public Func<ushort, byte> RM;//ReadMemory;
        public Action<ushort, byte> WM;//WriteMemory;
        public Func<int, byte> RP;//ReadIO;
        public Action<int, byte> WP;//WriteIO;
        public Func<ushort, byte> ROP;
        public Func<ushort, byte> ROP_ARG;
        public Func<int, int> irq_callback;
        public Action<int> setlines_callback;
        public delegate void debug_delegate();
        public debug_delegate debugger_start_cpu_hook_callback, debugger_stop_cpu_hook_callback;
        private ulong totalExecutedCycles;
        private int pendingCycles;
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
        public byte[] flags8i = new byte[256]
{
0x04,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
0x08|0x02,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,
0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,
0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,
0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,
0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,
0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,
0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,
0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08
};
        public byte[] flags8d=new byte[256]
        {
0x04,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x02,
0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,
0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,
0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,
0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,
0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,
0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,
0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,
0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08
};
        public byte[] cycles1 =new byte[256]
{
	/*   0  1  2  3  4  5  6  7  8  9  A  B  C  D  E  F */
  /*0*/  1, 1, 1, 1, 1, 1, 1, 1, 4, 4, 4, 4, 5, 5, 5, 5,
  /*1*/  2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
  /*2*/  2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
  /*3*/  2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 3, 3, 7, 6,
  /*4*/  3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4, 4, 3, 3, 4, 4,
  /*5*/  4, 4, 4, 4, 4, 4, 4, 4, 3, 3, 3, 3, 3, 1, 1, 1,
  /*6*/  3, 3, 3, 3, 3, 3, 3, 3, 5, 5, 5, 5, 5, 5, 5, 5,
  /*7*/  3, 3, 3, 3, 3, 3, 3, 3, 5, 5, 5, 5, 5, 5, 5, 5,
  /*8*/  2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 5,
  /*9*/  2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 6,
  /*A*/  2, 2, 2, 4, 4, 4, 4, 4, 2, 2, 2, 2, 3, 3, 2, 1,
  /*B*/  3, 2, 2,11,22,11, 2, 4, 3, 3, 3, 3, 3, 3, 3, 3,
  /*C*/  3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 2, 2, 3, 2,
  /*D*/  2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
  /*E*/  1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
  /*F*/  1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1
};
        public KonamiCpu()
        {
            setlines_callback = null;
            konami_main = new Action[0x100] {
	            illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal,	/* 00 */
	            opcode2,opcode2,opcode2,opcode2,pshs   ,pshu   ,puls   ,pulu   ,
	            lda_im ,ldb_im ,opcode2,opcode2,adda_im,addb_im,opcode2,opcode2,	/* 10 */
	            adca_im,adcb_im,opcode2,opcode2,suba_im,subb_im,opcode2,opcode2,
	            sbca_im,sbcb_im,opcode2,opcode2,anda_im,andb_im,opcode2,opcode2,	/* 20 */
	            bita_im,bitb_im,opcode2,opcode2,eora_im,eorb_im,opcode2,opcode2,
	            ora_im ,orb_im ,opcode2,opcode2,cmpa_im,cmpb_im,opcode2,opcode2,	/* 30 */
	            setline_im,opcode2,opcode2,opcode2,andcc,orcc  ,exg    ,tfr    ,
	            ldd_im ,opcode2,ldx_im ,opcode2,ldy_im ,opcode2,ldu_im ,opcode2,	/* 40 */
	            lds_im ,opcode2,cmpd_im,opcode2,cmpx_im,opcode2,cmpy_im,opcode2,
	            cmpu_im,opcode2,cmps_im,opcode2,addd_im,opcode2,subd_im,opcode2,	/* 50 */
	            opcode2,opcode2,opcode2,opcode2,opcode2,illegal,illegal,illegal,
	            bra    ,bhi    ,bcc    ,bne    ,bvc    ,bpl    ,bge    ,bgt    ,	/* 60 */
	            lbra   ,lbhi   ,lbcc   ,lbne   ,lbvc   ,lbpl   ,lbge   ,lbgt   ,
	            brn    ,bls    ,bcs    ,beq    ,bvs    ,bmi    ,blt    ,ble    ,	/* 70 */
	            lbrn   ,lbls   ,lbcs   ,lbeq   ,lbvs   ,lbmi   ,lblt   ,lble   ,
	            clra   ,clrb   ,opcode2,coma   ,comb   ,opcode2,nega   ,negb   ,	/* 80 */
	            opcode2,inca   ,incb   ,opcode2,deca   ,decb   ,opcode2,rts    ,
	            tsta   ,tstb   ,opcode2,lsra   ,lsrb   ,opcode2,rora   ,rorb   ,	/* 90 */
	            opcode2,asra   ,asrb   ,opcode2,asla   ,aslb   ,opcode2,rti    ,
	            rola   ,rolb   ,opcode2,opcode2,opcode2,opcode2,opcode2,opcode2,	/* a0 */
	            opcode2,opcode2,bsr    ,lbsr   ,decbjnz,decxjnz,nop    ,illegal,
	            abx    ,daa	   ,sex    ,mul    ,lmul   ,divx   ,bmove  ,move   ,	/* b0 */
	            lsrd   ,opcode2,rord   ,opcode2,asrd   ,opcode2,asld   ,opcode2,
	            rold   ,opcode2,clrd   ,opcode2,negd   ,opcode2,incd   ,opcode2,	/* c0 */
	            decd   ,opcode2,tstd   ,opcode2,absa   ,absb   ,absd   ,bset   ,
	            bset2  ,illegal,illegal,illegal,illegal,illegal,illegal,illegal,	/* d0 */
	            illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal,
	            illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal,	/* e0 */
	            illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal,
	            illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal,	/* f0 */
	            illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal
            };
            konami_indexed=new Action[0x100]{
                illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal,	/* 00 */
	            leax   ,leay   ,leau   ,leas   ,illegal,illegal,illegal,illegal,
	            illegal,illegal,lda_ix ,ldb_ix ,illegal,illegal,adda_ix,addb_ix,	/* 10 */
	            illegal,illegal,adca_ix,adcb_ix,illegal,illegal,suba_ix,subb_ix,
	            illegal,illegal,sbca_ix,sbcb_ix,illegal,illegal,anda_ix,andb_ix,	/* 20 */
	            illegal,illegal,bita_ix,bitb_ix,illegal,illegal,eora_ix,eorb_ix,
	            illegal,illegal,ora_ix ,orb_ix ,illegal,illegal,cmpa_ix,cmpb_ix,	/* 30 */
	            illegal,setline_ix,sta_ix,stb_ix,illegal,illegal,illegal,illegal,
	            illegal,ldd_ix ,illegal,ldx_ix ,illegal,ldy_ix ,illegal,ldu_ix ,	/* 40 */
	            illegal,lds_ix ,illegal,cmpd_ix,illegal,cmpx_ix,illegal,cmpy_ix,
	            illegal,cmpu_ix,illegal,cmps_ix,illegal,addd_ix,illegal,subd_ix,	/* 50 */
	            std_ix ,stx_ix ,sty_ix ,stu_ix ,sts_ix ,illegal,illegal,illegal,
	            illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal,	/* 60 */
	            illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal,
	            illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal,	/* 70 */
	            illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal,
	            illegal,illegal,clr_ix ,illegal,illegal,com_ix ,illegal,illegal,	/* 80 */
	            neg_ix ,illegal,illegal,inc_ix ,illegal,illegal,dec_ix ,illegal,
	            illegal,illegal,tst_ix ,illegal,illegal,lsr_ix ,illegal,illegal,	/* 90 */
	            ror_ix ,illegal,illegal,asr_ix ,illegal,illegal,asl_ix ,illegal,
	            illegal,illegal,rol_ix ,lsrw_ix,rorw_ix,asrw_ix,aslw_ix,rolw_ix,	/* a0 */
	            jmp_ix ,jsr_ix ,illegal,illegal,illegal,illegal,illegal,illegal,
	            illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal,	/* b0 */
	            illegal,lsrd_ix,illegal,rord_ix,illegal,asrd_ix,illegal,asld_ix,
	            illegal,rold_ix,illegal,clrw_ix,illegal,negw_ix,illegal,incw_ix,	/* c0 */
	            illegal,decw_ix,illegal,tstw_ix,illegal,illegal,illegal,illegal,
	            illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal,	/* d0 */
	            illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal,
	            illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal,	/* e0 */
	            illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal,
	            illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal,	/* f0 */
	            illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal
            };
            konami_direct=new Action[0x100]{
                illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal,	/* 00 */
	            illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal,
	            illegal,illegal,lda_di ,ldb_di ,illegal,illegal,adda_di,addb_di,	/* 10 */
	            illegal,illegal,adca_di,adcb_di,illegal,illegal,suba_di,subb_di,
	            illegal,illegal,sbca_di,sbcb_di,illegal,illegal,anda_di,andb_di,	/* 20 */
	            illegal,illegal,bita_di,bitb_di,illegal,illegal,eora_di,eorb_di,
	            illegal,illegal,ora_di ,orb_di ,illegal,illegal,cmpa_di,cmpb_di,	/* 30 */
	            illegal,setline_di,sta_di,stb_di,illegal,illegal,illegal,illegal,
	            illegal,ldd_di ,illegal,ldx_di ,illegal,ldy_di ,illegal,ldu_di ,	/* 40 */
	            illegal,lds_di ,illegal,cmpd_di,illegal,cmpx_di,illegal,cmpy_di,
	            illegal,cmpu_di,illegal,cmps_di,illegal,addd_di,illegal,subd_di,	/* 50 */
	            std_di ,stx_di ,sty_di ,stu_di ,sts_di ,illegal,illegal,illegal,
	            illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal,	/* 60 */
	            illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal,
	            illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal,	/* 70 */
	            illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal,
	            illegal,illegal,clr_di ,illegal,illegal,com_di ,illegal,illegal,	/* 80 */
	            neg_di ,illegal,illegal,inc_di ,illegal,illegal,dec_di ,illegal,
	            illegal,illegal,tst_di ,illegal,illegal,lsr_di ,illegal,illegal,	/* 90 */
	            ror_di ,illegal,illegal,asr_di ,illegal,illegal,asl_di ,illegal,
	            illegal,illegal,rol_di ,lsrw_di,rorw_di,asrw_di,aslw_di,rolw_di,	/* a0 */
	            jmp_di ,jsr_di ,illegal,illegal,illegal,illegal,illegal,illegal,
	            illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal,	/* b0 */
	            illegal,lsrd_di,illegal,rord_di,illegal,asrd_di,illegal,asld_di,
	            illegal,rold_di,illegal,clrw_di,illegal,negw_di,illegal,incw_di,	/* c0 */
	            illegal,decw_di,illegal,tstw_di,illegal,illegal,illegal,illegal,
	            illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal,	/* d0 */
	            illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal,
	            illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal,	/* e0 */
	            illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal,
	            illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal,	/* f0 */
	            illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal
            };
            konami_extended = new Action[0x100]
            {
                illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal,	/* 00 */
	            illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal,
	            illegal,illegal,lda_ex ,ldb_ex ,illegal,illegal,adda_ex,addb_ex,	/* 10 */
	            illegal,illegal,adca_ex,adcb_ex,illegal,illegal,suba_ex,subb_ex,
	            illegal,illegal,sbca_ex,sbcb_ex,illegal,illegal,anda_ex,andb_ex,	/* 20 */
	            illegal,illegal,bita_ex,bitb_ex,illegal,illegal,eora_ex,eorb_ex,
	            illegal,illegal,ora_ex ,orb_ex ,illegal,illegal,cmpa_ex,cmpb_ex,	/* 30 */
	            illegal,setline_ex,sta_ex,stb_ex,illegal,illegal,illegal,illegal,
	            illegal,ldd_ex ,illegal,ldx_ex ,illegal,ldy_ex ,illegal,ldu_ex ,	/* 40 */
	            illegal,lds_ex ,illegal,cmpd_ex,illegal,cmpx_ex,illegal,cmpy_ex,
	            illegal,cmpu_ex,illegal,cmps_ex,illegal,addd_ex,illegal,subd_ex,	/* 50 */
	            std_ex ,stx_ex ,sty_ex ,stu_ex ,sts_ex ,illegal,illegal,illegal,
	            illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal,	/* 60 */
	            illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal,
	            illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal,	/* 70 */
	            illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal,
	            illegal,illegal,clr_ex ,illegal,illegal,com_ex ,illegal,illegal,	/* 80 */
	            neg_ex ,illegal,illegal,inc_ex ,illegal,illegal,dec_ex ,illegal,
	            illegal,illegal,tst_ex ,illegal,illegal,lsr_ex ,illegal,illegal,	/* 90 */
	            ror_ex ,illegal,illegal,asr_ex ,illegal,illegal,asl_ex ,illegal,
	            illegal,illegal,rol_ex ,lsrw_ex,rorw_ex,asrw_ex,aslw_ex,rolw_ex,	/* a0 */
	            jmp_ex ,jsr_ex ,illegal,illegal,illegal,illegal,illegal,illegal,
	            illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal,	/* b0 */
	            illegal,lsrd_ex,illegal,rord_ex,illegal,asrd_ex,illegal,asld_ex,
	            illegal,rold_ex,illegal,clrw_ex,illegal,negw_ex,illegal,incw_ex,	/* c0 */
	            illegal,decw_ex,illegal,tstw_ex,illegal,illegal,illegal,illegal,
	            illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal,	/* d0 */
	            illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal,
	            illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal,	/* e0 */
	            illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal,
	            illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal,	/* f0 */
	            illegal,illegal,illegal,illegal,illegal,illegal,illegal,illegal
            };
        }
        public override void cpunum_set_input_line_and_vector(int cpunum, int line, LineState state, int vector)
        {
            Timer.timer_set_internal(Cpuint.cpunum_empty_event_queue, "cpunum_empty_event_queue");
        }
        public void CHECK_IRQ_LINES()
        {
            if (irq_state[KONAMI_IRQ_LINE] != LineState.CLEAR_LINE || irq_state[KONAMI_FIRQ_LINE] != LineState.CLEAR_LINE)
            {
                int_state &= unchecked((byte)~KONAMI_SYNC);
            }
            if (irq_state[KONAMI_FIRQ_LINE] != LineState.CLEAR_LINE && (CC & CC_IF) == 0)
            {
                if ((int_state & KONAMI_CWAI) != 0)
                {
                    int_state &= unchecked((byte)~KONAMI_CWAI);
                    extra_cycles += 7;
                }
                else
                {
                    CC &= (byte)(~CC_E);
                    PUSHWORD(PC);
                    PUSHBYTE(CC);
                    extra_cycles += 10;
                }
                CC |= (byte)(CC_IF | CC_II);
                PC.LowWord = RM16(0xfff6);
                irq_callback(1);
            }
            else if (irq_state[KONAMI_IRQ_LINE] != LineState.CLEAR_LINE && (CC & CC_II)==0)
            {
                if ((int_state & KONAMI_CWAI)!=0)
                {
                    int_state &= unchecked((byte)~KONAMI_CWAI);
                    extra_cycles += 7;
                }
                else
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
                    extra_cycles += 19;
                }
                CC |= CC_II;
                PC.LowWord = RM16(0xfff8);
                irq_callback(0);
            }
        }
        private ushort SIGNED(byte a)
        {
            return (ushort)(short)(sbyte)a;
        }
        private byte IMMBYTE()
        {
            byte b = ROP_ARG((ushort)PC.LowWord);
            PC.LowWord++;
            return b;
        }
        private RegisterPair IMMWORD()
        {
            RegisterPair w = new RegisterPair();
            w.d = (uint)((ROP_ARG((ushort)PC.d) << 8) | ROP_ARG((ushort)((PC.d + 1) & 0xffff)));
            PC.d += 2;
            return w;
        }
        private void PUSHBYTE(byte b)
        {
            --S.LowWord;
            WM(S.LowWord, b);
        }
        private void PUSHWORD(RegisterPair w)
        {
            --S.LowWord;
            WM(S.LowWord, w.LowByte);
            --S.LowWord;
            WM(S.LowWord, w.HighByte);
        }
        private byte PULLBYTE()
        {
            byte b;
            b = RM(S.LowWord);
            S.LowWord++;
            return b;
        }
        private ushort PULLWORD()
        {
            ushort w;
            w = (ushort)(RM(S.LowWord) << 8);
            S.LowWord++;
            w |= RM(S.LowWord);
            S.LowWord++;
            return w;
        }
        private void PSHUBYTE(byte b)
        {
            --U.LowWord; WM(U.LowWord, b);
        }
        private void PSHUWORD(RegisterPair w)
        {
            --U.LowWord;
            WM(U.LowWord, w.LowByte);
            --U.LowWord;
            WM(U.LowWord, w.HighByte);
        }
        private byte PULUBYTE()
        {
            byte b;
            b = RM(U.LowWord);
            U.LowWord++;
            return b;
        }
        private ushort PULUWORD()
        {
            ushort w;
            w = (ushort)(RM(U.LowWord) << 8);
            U.LowWord++;
            w |= RM(U.LowWord);
            U.LowWord++;
            return w;
        }
        private void CLR_HNZVC()
        {
            CC &= (byte)(~(CC_H | CC_N | CC_Z | CC_V | CC_C));
        }
        private void CLR_NZV()
        {
            CC &= (byte)(~(CC_N | CC_Z | CC_V));
        }
        private void CLR_NZ()
        {
            CC &= (byte)(~(CC_N | CC_Z));
        }
        private void CLR_HNZC()
        {
            CC &= (byte)(~(CC_H | CC_N | CC_Z | CC_C));
        }
        private void CLR_NZVC()
        {
            CC &= (byte)(~(CC_N | CC_Z | CC_V | CC_C));
        }
        private void CLR_Z()
        {
            CC &= (byte)(~(CC_Z));
        }
        private void CLR_NZC()
        {
            CC &= (byte)(~(CC_N | CC_Z | CC_C));
        }
        private void CLR_ZC()
        {
            CC &= (byte)(~(CC_Z | CC_C));
        }
        private void SET_Z(uint a)
        {
            if (a == 0)
            {
                SEZ();
            }
        }
        private void SET_Z8(byte a)
        {
            if (a == 0)
            {
                SEZ();
            }
        }
        private void SET_Z16(ushort a)
        {
            if (a == 0)
            {
                SEZ();
            }
        }
        private void SET_N8(byte a)
        {
            CC |= (byte)((a & 0x80) >> 4);
        }
        private void SET_N16(ushort a)
        {
            CC |= (byte)((a & 0x8000) >> 12);
        }
        private void SET_H(byte a, byte b, byte r)
        {
            CC |= (byte)(((a ^ b ^ r) & 0x10) << 1);
        }
        private void SET_C8(ushort a)
        {
            CC |= (byte)((a & 0x100) >> 8);
        }
        private void SET_C16(uint a)
        {
            CC |= (byte)((a & 0x10000) >> 16);
        }
        private void SET_V8(byte a, ushort b, ushort r)
        {
            CC |= (byte)(((a ^ b ^ r ^ (r >> 1)) & 0x80) >> 6);
        }
        private void SET_V16(ushort a, ushort b, uint r)
        {
            CC |= (byte)(((a ^ b ^ r ^ (r >> 1)) & 0x8000) >> 14);
        }
        private void SET_FLAGS8I(byte a)
        {
            CC |= flags8i[(a) & 0xff];
        }
        private void SET_FLAGS8D(byte a)
        {
            CC |= flags8d[(a) & 0xff];
        }
        private void SET_NZ8(byte a)
        {
            SET_N8(a);
            SET_Z(a);
        }
        private void SET_NZ16(ushort a)
        {
            SET_N16(a);
            SET_Z(a);
        }
        private void SET_FLAGS8(byte a, ushort b, ushort r)
        {
            SET_N8((byte)r);
            SET_Z8((byte)r);
            SET_V8(a, b, r);
            SET_C8(r);
        }
        private void SET_FLAGS16(ushort a, ushort b, uint r)
        {
            SET_N16((ushort)r);
            SET_Z16((ushort)r);
            SET_V16(a, b, r);
            SET_C16(r);
        }
        private void DIRECT()
        {
            EA.d = DP.d;
            EA.LowByte = IMMBYTE();
        }
        private void IMM8()
        {
            EA.d = PC.d;
            PC.LowWord++;
        }
        private void IMM16()
        {
            EA.d = PC.d;
            PC.LowWord += 2;
        }
        private void EXTENDED()
        {
            EA = IMMWORD();
        }
        private void SEC()
        {
            CC |= CC_C;
        }
        private void CLC()
        {
            CC &= (byte)(~CC_C);
        }
        private void SEZ()
        {
            CC |= CC_Z;
        }
        private void CLZ()
        {
            CC &= (byte)(~CC_Z);
        }
        private void SEN()
        {
            CC |= CC_N;
        }
        private void CLN()
        {
            CC &= (byte)(~CC_N);
        }
        private void SEV()
        {
            CC |= CC_V;
        }
        private void CLV()
        {
            CC &= (byte)(~CC_V);
        }
        private void SEH()
        {
            CC |= CC_H;
        }
        private void CLH()
        {
            CC &= (byte)(~CC_H);
        }
        private byte DIRBYTE()
        {
            DIRECT();
            return RM(EA.LowWord);
        }
        private RegisterPair DIRWORD()
        {
            RegisterPair w = new RegisterPair();
            DIRECT();
            w.LowWord = RM16(EA.LowWord);
            return w;
        }
        private byte EXTBYTE()
        {
            EXTENDED();
            return RM(EA.LowWord);
        }
        private RegisterPair EXTWORD()
        {
            RegisterPair w = new RegisterPair();
            EXTENDED();
            w.LowWord = RM16(EA.LowWord);
            return w;
        }
        private void BRANCH(bool f)
        {
            byte t = IMMBYTE();
            if (f)
            {
                PC.LowWord += (ushort)SIGNED(t);
            }
        }
        private void LBRANCH(bool f)
        {
            RegisterPair t = IMMWORD();
            if (f)
            {
                pendingCycles -= 1;
                PC.LowWord += t.LowWord;
            }
        }
        private byte NXORV()
        {
            return (byte)((CC & CC_N) ^ ((CC & CC_V) << 2));
        }
        private ushort GETREG(int reg)
        {
            ushort val;
            switch (reg)
            {
                case 0: val = D.HighByte; break;
                case 1: val = D.LowByte; break;
                case 2: val = X.LowWord; break;
                case 3: val = Y.LowWord; break;
                case 4: val = S.LowWord; break;
                case 5: val = U.LowWord; break;
                default: val = 0xff; break;
            }
            return val;
        }
        private void SETREG(ushort val, int reg)
        {
            switch (reg)
            {
                case 0: D.HighByte = (byte)val; break;
                case 1: D.LowByte = (byte)val; break;
                case 2: X.LowWord = val; break;
                case 3: Y.LowWord = val; break;
                case 4: S.LowWord = val; break;
                case 5: U.LowWord = val; break;
                default: break;
            }
        }
        private ushort RM16(ushort Addr)
        {
            ushort result = (ushort)(RM(Addr) << 8);
            return (ushort)(result | RM((ushort)((Addr + 1) & 0xffff)));
        }
        private void WM16(ushort Addr, RegisterPair p)
        {
            WM(Addr, p.HighByte);
            WM((ushort)((Addr + 1) & 0xffff), p.LowByte);
        }
        private void konami_init(Func<int, int> _irqcallback)
        {
            irq_callback = _irqcallback;
        }
        public override void Reset()
        {
            int_state = 0;
            nmi_state = LineState.CLEAR_LINE;
            irq_state[0] = LineState.CLEAR_LINE;
            irq_state[0] = LineState.CLEAR_LINE;

            DP.d = 0;

            CC |= CC_II;
            CC |= CC_IF;
            PC.d = RM16(0xfffe);
        }
        public override void set_irq_line(int irqline, LineState state)
        {
            if (irqline ==(int)LineState.INPUT_LINE_NMI)
            {
                if (nmi_state == state)
                {
                    return;
                }
                nmi_state = state;
                if (state ==LineState.CLEAR_LINE)
                {
                    return;
                }
                if ((int_state & KONAMI_LDS) == 0)
                {
                    return;
                }
                int_state &= (byte)~KONAMI_SYNC;
                if ((int_state & KONAMI_CWAI) != 0)
                {
                    int_state &= (byte)~KONAMI_CWAI;
                    extra_cycles += 7;
                }
                else
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
                    extra_cycles += 19;
                }
                CC |= (byte)(CC_IF | CC_II);
                PC.LowWord = RM16(0xfffc);
            }
            else if (irqline < 2)
            {
                irq_state[irqline] =state;
                if (state == LineState.CLEAR_LINE)
                {
                    return;
                }
                CHECK_IRQ_LINES();
            }
        }
        public override int ExecuteCycles(int cycles)
        {
            pendingCycles = cycles - extra_cycles;
            extra_cycles = 0;
            if ((int_state & (KONAMI_CWAI | KONAMI_SYNC))!=0)
            {
                pendingCycles = 0;
            }
            else
            {
                do
                {
                    int prevCycles = pendingCycles;
                    PPC = PC;
                    //debugger_instruction_hook(Machine, PCD);
                    ireg = ROP(PC.LowWord);
                    PC.LowWord++;
                    konami_main[ireg]();
                    pendingCycles -= cycles1[ireg];
                    int delta = prevCycles - pendingCycles;
                    totalExecutedCycles += (ulong)delta;
                }
                while (pendingCycles > 0);
                pendingCycles -= extra_cycles;
                extra_cycles = 0;
            }
            return cycles - pendingCycles;
        }
        public override void SaveStateBinary(BinaryWriter writer)
        {
            writer.Write(PPC.LowWord);
            writer.Write(PC.LowWord);
            writer.Write(U.LowWord);
            writer.Write(S.LowWord);
            writer.Write(X.LowWord);
            writer.Write(Y.LowWord);
            writer.Write(D.LowWord);
            writer.Write(DP.LowWord);
            writer.Write(CC);
            writer.Write(int_state);
            writer.Write((byte)nmi_state);
            writer.Write((byte)irq_state[0]);
            writer.Write((byte)irq_state[1]);
            writer.Write(TotalExecutedCycles);
            writer.Write(PendingCycles);
        }
        public override void LoadStateBinary(BinaryReader reader)
        {
            PPC.LowWord = reader.ReadUInt16();
            PC.LowWord = reader.ReadUInt16();
            U.LowWord = reader.ReadUInt16();
            S.LowWord = reader.ReadUInt16();
            X.LowWord = reader.ReadUInt16();
            Y.LowWord = reader.ReadUInt16();
            D.LowWord = reader.ReadUInt16();
            DP.LowWord = reader.ReadUInt16();
            CC = reader.ReadByte();
            int_state = reader.ReadByte();
            nmi_state = (LineState)reader.ReadByte();
            irq_state[0] = (LineState)reader.ReadByte();
            irq_state[1] = (LineState)reader.ReadByte();
            TotalExecutedCycles = reader.ReadUInt64();
            PendingCycles = reader.ReadInt32();
        }
    }
}
