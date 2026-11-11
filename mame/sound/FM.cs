using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace mame
{
    public class FM
    {
        public class FM_OPN
        {
            public int type;
            public FM_ST ST;
            public FM_3SLOT SL3;
            public FM_CH[] CH;
            public uint[] pan;
            public uint eg_cnt;
            public uint eg_timer;
            public uint eg_timer_add;
            public uint eg_timer_overflow;
            public uint[] fn_table;
            public int lfo_cnt;
            public int lfo_inc;
            public int[] lfo_freq;
            public int[,] idt_tab;
            public int[] iconnect1 = new int[8], iconnect2 = new int[8], iconnect3 = new int[8], iconnect4 = new int[6], imem = new int[13];
            public FM_OPN()
            {
                int i;
                CH = new FM_CH[6];
                for (i = 0; i < 6; i++)
                {
                    CH[i].SLOT = new FM_SLOT[4];
                }
                idt_tab = new int[6, 4];
                ST = new FM_ST();
                ST.dt_tab2 = new int[8, 32];
                SL3 = new FM_3SLOT();
                SL3.fc = new uint[3];
                SL3.kcode = new byte[3];
                SL3.block_fnum = new uint[3];
                pan = new uint[12];
                fn_table = new uint[4096];
                lfo_freq = new int[8];
                ST.timer_handler = null;
                ST.IRQ_Handler = null;
                ST.SSG.set_clock = null;
                ST.SSG.write = null;
                ST.SSG.read = null;
                ST.SSG.reset = null;
            }
            private void FM_STATUS_SET(byte flag)
            {
                ST.status |= flag;
                if ((ST.irq == 0) && ((ST.status & ST.irqmask) != 0))
                {
                    ST.irq = 1;
                    if (ST.IRQ_Handler != null)
                    {
                        ST.IRQ_Handler(1);
                    }
                }
            }
            public void FM_STATUS_RESET(byte flag)
            {
                ST.status &= (byte)~flag;
                if ((ST.irq != 0) && ((ST.status & ST.irqmask) == 0))
                {
                    ST.irq = 0;
                    if (ST.IRQ_Handler != null)
                    {
                        ST.IRQ_Handler(0);
                    }
                }
            }
            public void FM_IRQMASK_SET(byte flag)
            {
                ST.irqmask = flag;
                FM_STATUS_SET(0);
                FM_STATUS_RESET(0);
            }
            private void set_timers(byte v)
            {
                ST.mode = v;
                if ((v & 0x20) != 0)
                {
                    FM_STATUS_RESET(0x02);
                }
                if ((v & 0x10) != 0)
                {
                    FM_STATUS_RESET(0x01);
                }
                if ((v & 0x02) != 0)
                {
                    if (ST.TBC == 0)
                    {
                        ST.TBC = (256 - ST.TB) << 4;
                        ST.timer_handler(1, ST.TBC * ST.timer_prescaler, ST.clock);
                    }
                }
                else
                {
                    if (ST.TBC != 0)
                    {
                        ST.TBC = 0;
                        ST.timer_handler(1, 0, ST.clock);
                    }
                }
                if ((v & 0x01) != 0)
                {
                    if (ST.TAC == 0)
                    {
                        ST.TAC = (1024 - ST.TA);
                        ST.timer_handler(0, ST.TAC * ST.timer_prescaler, ST.clock);
                    }
                }
                else
                {
                    if (ST.TAC != 0)
                    {
                        ST.TAC = 0;
                        ST.timer_handler(0, 0, ST.clock);
                    }
                }
            }
            public void TimerAOver()
            {
                if ((ST.mode & 0x04) != 0)
                {
                    FM_STATUS_SET(0x01);
                }
                ST.TAC = (1024 - ST.TA);
                ST.timer_handler(0, ST.TAC * ST.timer_prescaler, ST.clock);
            }
            public void TimerBOver()
            {
                if ((ST.mode & 0x08) != 0)
                {
                    FM_STATUS_SET(0x02);
                }
                ST.TBC = (256 - ST.TB) << 4;
                ST.timer_handler(1, ST.TBC * ST.timer_prescaler, ST.clock);
            }
            public void FM_BUSY_CLEAR()
            {
                ST.busy_expiry_time = Attotime.ATTOTIME_ZERO;
            }
            public byte FM_STATUS_FLAG()
            {
                if (Attotime.attotime_compare(ST.busy_expiry_time, Attotime.ATTOTIME_ZERO) != 0)
                {
                    if (Attotime.attotime_compare(ST.busy_expiry_time, Timer.get_current_time()) > 0)
                    {
                        return (byte)(ST.status | 0x80);
                    }
                    FM_BUSY_CLEAR();
                }
                return ST.status;
            }
            public void FM_BUSY_SET(int busyclock)
            {
                Atime expiry_period = Attotime.attotime_mul(Attotime.ATTOTIME_IN_HZ(ST.clock), (uint)(busyclock * ST.timer_prescaler));
                ST.busy_expiry_time = Attotime.attotime_add(Timer.get_current_time(), expiry_period);
            }
            private void FM_KEYON(int type, int c, int s)
            {
                if (CH[c].SLOT[s].key == 0)
                {
                    CH[c].SLOT[s].key = 1;
                    CH[c].SLOT[s].phase = 0;
                    if ((type == TYPE_YM2612) || (type == TYPE_YM2608))
                    {
                        if ((CH[c].SLOT[s].ar + CH[c].SLOT[s].ksr) < 32 + 62)
                        {
                            CH[c].SLOT[s].state = 4;
                            CH[c].SLOT[s].volume = 1023;
                        }
                        else
                        {
                            CH[c].SLOT[s].state = 3;
                            CH[c].SLOT[s].volume = 0;
                        }
                    }
                    else
                    {
                        CH[c].SLOT[s].state = 4;
                    }
                }
            }
            private void FM_KEYOFF(int c, int s)
            {
                if (CH[c].SLOT[s].key != 0)
                {
                    CH[c].SLOT[s].key = 0;
                    if (CH[c].SLOT[s].state > 1)
                    {
                        CH[c].SLOT[s].state = 1;
                    }
                }
            }
            private void set_value1(int c)
            {
                if (iconnect1[c] == 12)
                {
                    out_fm[11] = out_fm[9] = out_fm[10] = CH[c].op1_out0;
                }
                else
                {
                    out_fm[iconnect1[c]] = CH[c].op1_out0;
                }
            }
            private void set_mem(int c)
            {
                if (imem[c] == 8 || imem[c] == 10 || imem[c] == 11)
                {
                    out_fm[imem[c]] = CH[c].mem_value;
                }
            }
            private void setup_connection(int ch)
            {
                switch (CH[ch].ALGO)
                {
                    case 0:
                        iconnect1[ch] = 9;
                        iconnect2[ch] = 11;
                        iconnect3[ch] = 10;
                        imem[ch] = 8;
                        break;
                    case 1:
                        iconnect1[ch] = 11;
                        iconnect2[ch] = 11;
                        iconnect3[ch] = 10;
                        imem[ch] = 8;
                        break;
                    case 2:
                        iconnect1[ch] = 10;
                        iconnect2[ch] = 11;
                        iconnect3[ch] = 10;
                        imem[ch] = 8;
                        break;
                    case 3:
                        iconnect1[ch] = 9;
                        iconnect2[ch] = 11;
                        iconnect3[ch] = 10;
                        imem[ch] = 10;
                        break;
                    case 4:
                        iconnect1[ch] = 9;
                        iconnect2[ch] = ch;
                        iconnect3[ch] = 10;
                        imem[ch] = 11;
                        break;
                    case 5:
                        iconnect1[ch] = 12;
                        iconnect2[ch] = ch;
                        iconnect3[ch] = ch;
                        imem[ch] = 8;
                        break;
                    case 6:
                        iconnect1[ch] = 9;
                        iconnect2[ch] = ch;
                        iconnect3[ch] = ch;
                        imem[ch] = 11;
                        break;
                    case 7:
                        iconnect1[ch] = ch;
                        iconnect2[ch] = ch;
                        iconnect3[ch] = ch;
                        imem[ch] = 11;
                        break;
                }
                iconnect4[ch] = ch;
            }
            private void set_det_mul(int c, int s, byte v)
            {
                CH[c].SLOT[s].mul = ((v & 0x0f) != 0) ? (v & 0x0f) * 2 : 1;
                idt_tab[c, s] = (v >> 4) & 7;
                CH[c].SLOT[0].Incr = -1;
            }
            private void set_tl(int c, int s, byte v)
            {
                CH[c].SLOT[s].tl = (v & 0x7f) << (10 - 7);
            }
            private void set_ar_ksr(int c, int s, byte v)
            {
                byte old_KSR = CH[c].SLOT[s].KSR;
                CH[c].SLOT[s].ar = ((v & 0x1f) != 0) ? 32 + ((v & 0x1f) << 1) : 0;
                CH[c].SLOT[s].KSR = (byte)(3 - (v >> 6));
                if (CH[c].SLOT[s].KSR != old_KSR)
                {
                    CH[c].SLOT[0].Incr = -1;
                }
                if ((CH[c].SLOT[s].ar + CH[c].SLOT[s].ksr) < 32 + 62)
                {
                    CH[c].SLOT[s].eg_sh_ar = eg_rate_shift[CH[c].SLOT[s].ar + CH[c].SLOT[s].ksr];
                    if ((type == TYPE_YM2612) || (type == TYPE_YM2608))
                    {
                        CH[c].SLOT[s].eg_sel_ar = eg_rate_select2612[CH[c].SLOT[s].ar + CH[c].SLOT[s].ksr];
                    }
                    else
                    {
                        CH[c].SLOT[s].eg_sel_ar = eg_rate_select[CH[c].SLOT[s].ar + CH[c].SLOT[s].ksr];
                    }
                }
                else
                {
                    CH[c].SLOT[s].eg_sh_ar = 0;
                    CH[c].SLOT[s].eg_sel_ar = 17 * 8;
                }
            }
            private void set_dr(int c, int s, byte v)
            {
                CH[c].SLOT[s].d1r = ((v & 0x1f) != 0) ? 32 + ((v & 0x1f) << 1) : 0;
                CH[c].SLOT[s].eg_sh_d1r = eg_rate_shift[CH[c].SLOT[s].d1r + CH[c].SLOT[s].ksr];
                if ((type == TYPE_YM2612) || (type == TYPE_YM2608))
                {
                    CH[c].SLOT[s].eg_sel_d1r = eg_rate_select2612[CH[c].SLOT[s].d1r + CH[c].SLOT[s].ksr];
                }
                else
                {
                    CH[c].SLOT[s].eg_sel_d1r = eg_rate_select[CH[c].SLOT[s].d1r + CH[c].SLOT[s].ksr];
                }
            }
            private void set_sr(int c, int s, byte v)
            {
                CH[c].SLOT[s].d2r = ((v & 0x1f) != 0) ? 32 + ((v & 0x1f) << 1) : 0;
                CH[c].SLOT[s].eg_sh_d2r = eg_rate_shift[CH[c].SLOT[s].d2r + CH[c].SLOT[s].ksr];
                if ((type == TYPE_YM2612) || (type == TYPE_YM2608))
                {
                    CH[c].SLOT[s].eg_sel_d2r = eg_rate_select2612[CH[c].SLOT[s].d2r + CH[c].SLOT[s].ksr];
                }
                else
                {
                    CH[c].SLOT[s].eg_sel_d2r = eg_rate_select[CH[c].SLOT[s].d2r + CH[c].SLOT[s].ksr];
                }
            }
            private void set_sl_rr(int c, int s, byte v)
            {
                CH[c].SLOT[s].sl = sl_table[v >> 4];
                CH[c].SLOT[s].rr = 34 + ((v & 0x0f) << 2);
                CH[c].SLOT[s].eg_sh_rr = eg_rate_shift[CH[c].SLOT[s].rr + CH[c].SLOT[s].ksr];
                if ((type == TYPE_YM2612) || (type == TYPE_YM2608))
                {
                    CH[c].SLOT[s].eg_sel_rr = eg_rate_select2612[CH[c].SLOT[s].rr + CH[c].SLOT[s].ksr];
                }
                else
                {
                    CH[c].SLOT[s].eg_sel_rr = eg_rate_select[CH[c].SLOT[s].rr + CH[c].SLOT[s].ksr];
                }
            }
            public void advance_lfo()
            {
                byte pos;
                byte prev_pos;
                if (lfo_inc != 0)
                {
                    prev_pos = (byte)(lfo_cnt >> 24 & 127);
                    lfo_cnt += lfo_inc;
                    pos = (byte)((lfo_cnt >> 24) & 127);
                    if (pos < 64)
                    {
                        LFO_AM = (pos & 63) * 2;
                    }
                    else
                    {
                        LFO_AM = 126 - ((pos & 63) * 2);
                    }
                    prev_pos >>= 2;
                    pos >>= 2;
                    LFO_PM = pos;
                }
                else
                {
                    LFO_AM = 0;
                    LFO_PM = 0;
                }
            }
            public void advance_eg_channel(int c)
            {
                uint out1;
                byte swap_flag = 0;
                int i;
                for (i = 0; i < 4; i++)
                {
                    switch (CH[c].SLOT[i].state)
                    {
                        case 4:
                            if ((eg_cnt & ((1 << CH[c].SLOT[i].eg_sh_ar) - 1)) == 0)
                            {
                                CH[c].SLOT[i].volume += (~CH[c].SLOT[i].volume * (eg_inc[CH[c].SLOT[i].eg_sel_ar + ((eg_cnt >> CH[c].SLOT[i].eg_sh_ar) & 7)])) >> 4;
                                if (CH[c].SLOT[i].volume <= 0)
                                {
                                    CH[c].SLOT[i].volume = 0;
                                    CH[c].SLOT[i].state = 3;
                                }
                            }
                            break;

                        case 3:
                            if ((CH[c].SLOT[i].ssg & 0x08) != 0)
                            {
                                if ((eg_cnt & ((1 << CH[c].SLOT[i].eg_sh_d1r) - 1)) == 0)
                                {
                                    CH[c].SLOT[i].volume += 4 * eg_inc[CH[c].SLOT[i].eg_sel_d1r + ((eg_cnt >> CH[c].SLOT[i].eg_sh_d1r) & 7)];
                                    if (CH[c].SLOT[i].volume >= CH[c].SLOT[i].sl)
                                    {
                                        CH[c].SLOT[i].state = 2;
                                    }
                                }
                            }
                            else
                            {
                                if ((eg_cnt & ((1 << CH[c].SLOT[i].eg_sh_d1r) - 1)) == 0)
                                {
                                    CH[c].SLOT[i].volume += eg_inc[CH[c].SLOT[i].eg_sel_d1r + ((eg_cnt >> CH[c].SLOT[i].eg_sh_d1r) & 7)];
                                    if (CH[c].SLOT[i].volume >= CH[c].SLOT[i].sl)
                                    {
                                        CH[c].SLOT[i].state = 2;
                                    }
                                }
                            }
                            break;
                        case 2:
                            if ((CH[c].SLOT[i].ssg & 0x08) != 0)
                            {
                                if ((eg_cnt & ((1 << CH[c].SLOT[i].eg_sh_d2r) - 1)) == 0)
                                {
                                    CH[c].SLOT[i].volume += 4 * eg_inc[CH[c].SLOT[i].eg_sel_d2r + ((eg_cnt >> CH[c].SLOT[i].eg_sh_d2r) & 7)];
                                    if (CH[c].SLOT[i].volume >= 512)
                                    {
                                        CH[c].SLOT[i].volume = 0x3ff;
                                        if ((CH[c].SLOT[i].ssg & 0x01) != 0)
                                        {
                                            if ((CH[c].SLOT[i].ssgn & 1) != 0)
                                            {

                                            }
                                            else
                                            {
                                                swap_flag = (byte)((CH[c].SLOT[i].ssg & 0x02) | 1);
                                            }
                                        }
                                        else
                                        {
                                            CH[c].SLOT[i].phase = 0;
                                            CH[c].SLOT[i].volume = 511;
                                            CH[c].SLOT[i].state = 4;
                                            swap_flag = (byte)(CH[c].SLOT[i].ssg & 0x02); /* bit 1 = alternate */
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if ((eg_cnt & ((1 << CH[c].SLOT[i].eg_sh_d2r) - 1)) == 0)
                                {
                                    CH[c].SLOT[i].volume += eg_inc[CH[c].SLOT[i].eg_sel_d2r + ((eg_cnt >> CH[c].SLOT[i].eg_sh_d2r) & 7)];
                                    if (CH[c].SLOT[i].volume >= 0x3ff)
                                    {
                                        CH[c].SLOT[i].volume = 0x3ff;
                                    }
                                }
                            }
                            break;
                        case 1:
                            if ((eg_cnt & ((1 << CH[c].SLOT[i].eg_sh_rr) - 1)) == 0)
                            {
                                CH[c].SLOT[i].volume += eg_inc[CH[c].SLOT[i].eg_sel_rr + ((eg_cnt >> CH[c].SLOT[i].eg_sh_rr) & 7)];
                                if (CH[c].SLOT[i].volume >= 0x3ff)
                                {
                                    CH[c].SLOT[i].volume = 0x3ff;
                                    CH[c].SLOT[i].state = 0;
                                }
                            }
                            break;
                    }
                    out1 = (uint)(CH[c].SLOT[i].tl + ((uint)CH[c].SLOT[i].volume));
                    if (((CH[c].SLOT[i].ssg & 0x08) != 0) && ((CH[c].SLOT[i].ssgn & 2) != 0) && (CH[c].SLOT[i].state != 0))
                    {
                        out1 ^= 511;
                    }
                    CH[c].SLOT[i].vol_out = out1;
                    CH[c].SLOT[i].ssgn ^= swap_flag;
                }
            }
            private uint volume_calc(int c, int s)
            {
                int AM = LFO_AM >> CH[c].ams;
                return (uint)(CH[c].SLOT[s].vol_out + (AM & CH[c].SLOT[s].AMmask));
            }
            private void update_phase_lfo_slot(int s, int pms, uint block_fnum)
            {
                uint fnum_lfo = ((block_fnum & 0x7f0) >> 4) * 32 * 8;
                int lfo_fn_table_index_offset = lfo_pm_table[fnum_lfo + pms + LFO_PM];
                if (lfo_fn_table_index_offset != 0)
                {
                    byte blk;
                    uint fn;
                    int kc, fc;
                    block_fnum = (uint)(block_fnum * 2 + lfo_fn_table_index_offset);
                    blk = (byte)((block_fnum & 0x7000) >> 12);
                    fn = block_fnum & 0xfff;
                    kc = (blk << 2) | opn_fktable[fn >> 8];
                    fc = (int)((fn_table[fn] >> (7 - blk)) + ST.dt_tab2[idt_tab[2, 0], kc]);
                    if (fc < 0)
                    {
                        fc += fn_max;
                    }
                    CH[2].SLOT[s].phase += (uint)((fc * CH[2].SLOT[s].mul) >> 1);
                }
                else
                {
                    CH[2].SLOT[s].phase += (uint)CH[2].SLOT[s].Incr;
                }
            }
            private void update_phase_lfo_channel(FM_CH[] CH, int c)
            {
                uint block_fnum = CH[c].block_fnum;
                uint fnum_lfo = ((block_fnum & 0x7f0) >> 4) * 32 * 8;
                int lfo_fn_table_index_offset = lfo_pm_table[fnum_lfo + CH[c].pms + LFO_PM];
                if (lfo_fn_table_index_offset != 0)
                {
                    byte blk;
                    uint fn;
                    int kc, fc, finc;
                    block_fnum = (uint)(block_fnum * 2 + lfo_fn_table_index_offset);
                    blk = (byte)((block_fnum & 0x7000) >> 12);
                    fn = block_fnum & 0xfff;
                    kc = (blk << 2) | opn_fktable[fn >> 8];
                    fc = (int)(fn_table[fn] >> (7 - blk));
                    finc = fc + ST.dt_tab2[idt_tab[c, 0], kc];
                    if (finc < 0)
                    {
                        finc += fn_max;
                    }
                    CH[c].SLOT[0].phase += (uint)((finc * CH[c].SLOT[0].mul) >> 1);
                    finc = fc + ST.dt_tab2[idt_tab[c, 2], kc];
                    if (finc < 0)
                    {
                        finc += fn_max;
                    }
                    CH[c].SLOT[2].phase += (uint)((finc * CH[c].SLOT[2].mul) >> 1);
                    finc = fc + ST.dt_tab2[idt_tab[c, 1], kc];
                    if (finc < 0)
                    {
                        finc += fn_max;
                    }
                    CH[c].SLOT[1].phase += (uint)((finc * CH[c].SLOT[1].mul) >> 1);
                    finc = fc + ST.dt_tab2[idt_tab[c, 3], kc];
                    if (finc < 0)
                    {
                        finc += fn_max;
                    }
                    CH[c].SLOT[3].phase += (uint)((finc * CH[c].SLOT[3].mul) >> 1);
                }
                else
                {
                    CH[c].SLOT[0].phase += (uint)CH[c].SLOT[0].Incr;
                    CH[c].SLOT[2].phase += (uint)CH[c].SLOT[2].Incr;
                    CH[c].SLOT[1].phase += (uint)CH[c].SLOT[1].Incr;
                    CH[c].SLOT[3].phase += (uint)CH[c].SLOT[3].Incr;
                }
            }
            public void chan_calc(int c, int chnum)
            {
                uint eg_out;
                out_fm[8] = out_fm[9] = out_fm[10] = out_fm[11] = 0;//m2 = c1 = c2 = mem = 0;
                set_mem(c);
                eg_out = volume_calc(c, 0);
                int out1 = CH[c].op1_out0 + CH[c].op1_out1;
                CH[c].op1_out0 = CH[c].op1_out1;
                set_value1(c);
                CH[c].op1_out1 = 0;
                if (eg_out < 832)
                {
                    if (CH[c].FB == 0)
                    {
                        out1 = 0;
                    }
                    CH[c].op1_out1 = op_calc1(CH[c].SLOT[0].phase, eg_out, (out1 << CH[c].FB));
                }
                eg_out = volume_calc(c, 1);
                if (eg_out < 832)
                {
                    out_fm[iconnect3[c]] += op_calc(CH[c].SLOT[1].phase, eg_out, out_fm[8]);
                }
                eg_out = volume_calc(c, 2);
                if (eg_out < 832)
                {
                    out_fm[iconnect2[c]] += op_calc(CH[c].SLOT[2].phase, eg_out, out_fm[9]);
                }
                eg_out = volume_calc(c, 3);
                if (eg_out < 832)
                {
                    out_fm[iconnect4[c]] += op_calc(CH[c].SLOT[3].phase, eg_out, out_fm[10]);
                }
                CH[c].mem_value = out_fm[11];//mem;
                if (CH[c].pms != 0)
                {
                    if (((ST.mode & 0xC0) != 0) && (chnum == 2))
                    {
                        update_phase_lfo_slot(0, CH[c].pms, SL3.block_fnum[1]);
                        update_phase_lfo_slot(2, CH[c].pms, SL3.block_fnum[2]);
                        update_phase_lfo_slot(1, CH[c].pms, SL3.block_fnum[0]);
                        update_phase_lfo_slot(3, CH[c].pms, CH[c].block_fnum);
                    }
                    else
                    {
                        update_phase_lfo_channel(CH, c);
                    }
                }
                else
                {
                    CH[c].SLOT[0].phase += (uint)CH[c].SLOT[0].Incr;
                    CH[c].SLOT[2].phase += (uint)CH[c].SLOT[2].Incr;
                    CH[c].SLOT[1].phase += (uint)CH[c].SLOT[1].Incr;
                    CH[c].SLOT[3].phase += (uint)CH[c].SLOT[3].Incr;
                }
            }
            public void refresh_fc_eg_slot(int type, int c, int s, int fc, int kc)
            {
                int ksr = kc >> CH[c].SLOT[s].KSR;
                fc += ST.dt_tab2[idt_tab[c, s], kc];
                if (fc < 0)
                {
                    fc += fn_max;
                }
                CH[c].SLOT[s].Incr = (fc * CH[c].SLOT[s].mul) >> 1;
                if (CH[c].SLOT[s].ksr != ksr)
                {
                    CH[c].SLOT[s].ksr = (byte)ksr;
                    if ((CH[c].SLOT[s].ar + CH[c].SLOT[s].ksr) < 32 + 62)
                    {
                        CH[c].SLOT[s].eg_sh_ar = eg_rate_shift[CH[c].SLOT[s].ar + CH[c].SLOT[s].ksr];
                        if ((type == TYPE_YM2612) || (type == TYPE_YM2608))
                        {
                            CH[c].SLOT[s].eg_sel_ar = eg_rate_select2612[CH[c].SLOT[s].ar + CH[c].SLOT[s].ksr];
                        }
                        else
                        {
                            CH[c].SLOT[s].eg_sel_ar = eg_rate_select[CH[c].SLOT[s].ar + CH[c].SLOT[s].ksr];
                        }
                    }
                    else
                    {
                        CH[c].SLOT[s].eg_sh_ar = 0;
                        CH[c].SLOT[s].eg_sel_ar = 17 * 8;
                    }
                    CH[c].SLOT[s].eg_sh_d1r = eg_rate_shift[CH[c].SLOT[s].d1r + CH[c].SLOT[s].ksr];
                    CH[c].SLOT[s].eg_sh_d2r = eg_rate_shift[CH[c].SLOT[s].d2r + CH[c].SLOT[s].ksr];
                    CH[c].SLOT[s].eg_sh_rr = eg_rate_shift[CH[c].SLOT[s].rr + CH[c].SLOT[s].ksr];
                    if ((type == TYPE_YM2612) || (type == TYPE_YM2608))
                    {
                        CH[c].SLOT[s].eg_sel_d1r = eg_rate_select2612[CH[c].SLOT[s].d1r + CH[c].SLOT[s].ksr];
                        CH[c].SLOT[s].eg_sel_d2r = eg_rate_select2612[CH[c].SLOT[s].d2r + CH[c].SLOT[s].ksr];
                        CH[c].SLOT[s].eg_sel_rr = eg_rate_select2612[CH[c].SLOT[s].rr + CH[c].SLOT[s].ksr];
                    }
                    else
                    {
                        CH[c].SLOT[s].eg_sel_d1r = eg_rate_select[CH[c].SLOT[s].d1r + CH[c].SLOT[s].ksr];
                        CH[c].SLOT[s].eg_sel_d2r = eg_rate_select[CH[c].SLOT[s].d2r + CH[c].SLOT[s].ksr];
                        CH[c].SLOT[s].eg_sel_rr = eg_rate_select[CH[c].SLOT[s].rr + CH[c].SLOT[s].ksr];
                    }
                }
            }
            public void refresh_fc_eg_chan(int type, int c)
            {
                if (CH[c].SLOT[0].Incr == -1)
                {
                    int fc = (int)CH[c].fc;
                    int kc = CH[c].kcode;
                    refresh_fc_eg_slot(type, c, 0, fc, kc);
                    refresh_fc_eg_slot(type, c, 2, fc, kc);
                    refresh_fc_eg_slot(type, c, 1, fc, kc);
                    refresh_fc_eg_slot(type, c, 3, fc, kc);
                }
            }
            private void init_timetables()
            {
                int i, d;
                double rate;
                for (d = 0; d <= 3; d++)
                {
                    for (i = 0; i <= 31; i++)
                    {
                        rate = ((double)dt_tab[d * 32 + i]) * 1024 * ST.freqbase * (1 << 16) / ((double)(1 << 20));
                        ST.dt_tab2[d, i] = (int)rate;
                        ST.dt_tab2[d + 4, i] = -ST.dt_tab2[d, i];
                    }
                }
            }
            public void reset_channels(int num)
            {
                int c, s;
                ST.mode = 0;	/* normal mode */
                ST.TA = 0;
                ST.TAC = 0;
                ST.TB = 0;
                ST.TBC = 0;
                for (c = 0; c < num; c++)
                {
                    CH[c].fc = 0;
                    for (s = 0; s < 4; s++)
                    {
                        CH[c].SLOT[s].ssg = 0;
                        CH[c].SLOT[s].ssgn = 0;
                        CH[c].SLOT[s].state = 0;
                        CH[c].SLOT[s].volume = 0x3ff;
                        CH[c].SLOT[s].vol_out = 0x3ff;
                    }
                }
            }
            public void CSMKeyControll()
            {
                FM_KEYON(type, 2, 0);
                FM_KEYON(type, 2, 2);
                FM_KEYON(type, 2, 1);
                FM_KEYON(type, 2, 3);
            }
            public void OPNSetPres(int pres, int timer_prescaler, int SSGpres)
            {
                int i;
                ST.freqbase = (ST.rate != 0) ? ((double)ST.clock / ST.rate) / pres : 0;
                eg_timer_add = (uint)((1 << 16) * ST.freqbase);
                eg_timer_overflow = (3) * (1 << 16);
                ST.timer_prescaler = timer_prescaler;
                if (SSGpres != 0)
                {
                    ST.SSG.set_clock(ST.clock * 2 / SSGpres);
                }
                init_timetables();
                for (i = 0; i < 4096; i++)
                {
                    fn_table[i] = (uint)((double)i * 32 * ST.freqbase * (1 << (16 - 10)));
                }
                fn_max = (int)((uint)(((double)fn_table[0x7ff * 2] / ST.freqbase)) >> 2);//2096127
                for (i = 0; i < 8; i++)
                {
                    lfo_freq[i] = (int)((1.0 / lfo_samples_per_step[i]) * (1 << 24) * ST.freqbase);
                }
            }
            public void OPNWriteMode(int r, byte v)
            {
                byte c;
                switch (r)
                {
                    case 0x21:
                        break;
                    case 0x22:
                        if ((type & TYPE_LFOPAN) != 0)
                        {
                            if ((v & 0x08) != 0)
                            {
                                lfo_inc = lfo_freq[v & 7];
                            }
                            else
                            {
                                lfo_inc = 0;
                            }
                        }
                        break;
                    case 0x24:
                        ST.TA = (ST.TA & 0x03) | (((int)v) << 2);
                        break;
                    case 0x25:
                        ST.TA = (ST.TA & 0x3fc) | (v & 3);
                        break;
                    case 0x26:
                        ST.TB = v;
                        break;
                    case 0x27:
                        set_timers(v);
                        break;
                    case 0x28:
                        c = (byte)(v & 0x03);
                        if (c == 3)
                            break;
                        if ((v & 0x04) != 0 && (type & TYPE_6CH) != 0)
                            c += 3;
                        if ((v & 0x10) != 0)
                            FM_KEYON(type, c, 0);
                        else
                            FM_KEYOFF(c, 0);
                        if ((v & 0x20) != 0)
                            FM_KEYON(type, c, 2);
                        else
                            FM_KEYOFF(c, 2);
                        if ((v & 0x40) != 0)
                            FM_KEYON(type, c, 1);
                        else
                            FM_KEYOFF(c, 1);
                        if ((v & 0x80) != 0)
                            FM_KEYON(type, c, 3);
                        else
                            FM_KEYOFF(c, 3);
                        break;
                }
            }
            public void OPNWriteReg(int r, byte v)
            {
                byte c = (byte)(r & 3);
                int s = (r >> 2) & 3;
                if (c == 3)
                    return;
                if (r >= 0x100)
                    c += 3;
                switch (r & 0xf0)
                {
                    case 0x30:
                        set_det_mul(c, s, v);
                        break;
                    case 0x40:
                        set_tl(c, s, v);
                        break;
                    case 0x50:
                        set_ar_ksr(c, s, v);
                        break;
                    case 0x60:
                        set_dr(c, s, v);
                        if ((type & TYPE_LFOPAN) != 0)
                        {
                            CH[c].SLOT[s].AMmask = ((v & 0x80) != 0) ? 0xffffffff : 0;
                        }
                        break;
                    case 0x70:
                        set_sr(c, s, v);
                        break;
                    case 0x80:
                        set_sl_rr(c, s, v);
                        break;
                    case 0x90:
                        CH[c].SLOT[s].ssg = (byte)(v & 0x0f);
                        CH[c].SLOT[s].ssgn = (byte)((v & 0x04) >> 1);
                        break;
                    case 0xa0:
                        switch ((r >> 2) & 3)
                        {
                            case 0:
                                {
                                    uint fn = (((uint)((ST.fn_h) & 7)) << 8) + v;
                                    byte blk = (byte)(ST.fn_h >> 3);
                                    CH[c].kcode = (byte)((blk << 2) | opn_fktable[fn >> 7]);
                                    CH[c].fc = fn_table[fn * 2] >> (7 - blk);
                                    CH[c].block_fnum = (uint)(blk << 11) | fn;
                                    CH[c].SLOT[0].Incr = -1;
                                }
                                break;
                            case 1:
                                ST.fn_h = (byte)(v & 0x3f);
                                break;
                            case 2:
                                if (r < 0x100)
                                {
                                    uint fn = (((uint)(SL3.fn_h & 7)) << 8) + v;
                                    byte blk = (byte)(SL3.fn_h >> 3);
                                    SL3.kcode[c] = (byte)((blk << 2) | opn_fktable[fn >> 7]);
                                    SL3.fc[c] = fn_table[fn * 2] >> (7 - blk);
                                    SL3.block_fnum[c] = (uint)(blk << 11) | fn;
                                    CH[c + 2].SLOT[0].Incr = -1;
                                }
                                break;
                            case 3:
                                if (r < 0x100)
                                    SL3.fn_h = (byte)(v & 0x3f);
                                break;
                        }
                        break;
                    case 0xb0:
                        switch ((r >> 2) & 3)
                        {
                            case 0:
                                {
                                    int feedback = (v >> 3) & 7;
                                    CH[c].ALGO = (byte)(v & 7);
                                    CH[c].FB = (feedback != 0) ? (byte)(feedback + 6) : (byte)0;
                                    setup_connection(c);
                                }
                                break;
                            case 1:
                                if ((type & TYPE_LFOPAN) != 0)
                                {
                                    CH[c].pms = (v & 7) * 32;
                                    CH[c].ams = lfo_ams_depth_shift[(v >> 4) & 0x03];
                                    pan[c * 2] = ((v & 0x80) != 0) ? 0xffffffff : 0;
                                    pan[c * 2 + 1] = ((v & 0x40) != 0) ? 0xffffffff : 0;
                                }
                                break;
                        }
                        break;
                }
            }
            public void OPNPrescaler_w(int addr, int pre_divider)
            {
                int[] opn_pres = new int[4] { 2 * 12, 2 * 12, 6 * 12, 3 * 12 };
                int[] ssg_pres = new int[4] { 1, 1, 4, 2 };
                int sel;
                switch (addr)
                {
                    case 0:
                        ST.prescaler_sel = 2;
                        break;
                    case 1:
                        break;
                    case 0x2d:
                        ST.prescaler_sel |= 0x02;
                        break;
                    case 0x2e:
                        ST.prescaler_sel |= 0x01;
                        break;
                    case 0x2f:
                        ST.prescaler_sel = 0;
                        break;
                }
                sel = ST.prescaler_sel & 3;
                OPNSetPres(opn_pres[sel] * pre_divider, opn_pres[sel] * pre_divider, ssg_pres[sel] * pre_divider);
            }
        }
        public struct FM_SLOT
        {
            public byte KSR;
            public int ar;
            public int d1r;
            public int d2r;
            public int rr;
            public byte ksr;
            public int mul;
            public uint phase;
            public int Incr;
            public byte state;
            public int tl;
            public int volume;
            public int sl;
            public uint vol_out;
            public byte eg_sh_ar;
            public byte eg_sel_ar;
            public byte eg_sh_d1r;
            public byte eg_sel_d1r;
            public byte eg_sh_d2r;
            public byte eg_sel_d2r;
            public byte eg_sh_rr;
            public byte eg_sel_rr;
            public byte ssg;
            public byte ssgn;
            public uint key;
            public uint AMmask;
        }
        public struct FM_CH
        {
            public FM_SLOT[] SLOT;
            public byte ALGO;
            public byte FB;
            public int op1_out0, op1_out1;
            public int mem_value;
            public int pms;
            public byte ams;
            public uint fc;
            public byte kcode;
            public uint block_fnum;
        }
        public struct FM_ST
        {
            public int clock;
            public int rate;
            public double freqbase;
            public int timer_prescaler;
            public Atime busy_expiry_time;
            public byte address;
            public byte irq;
            public byte irqmask;
            public byte status;
            public byte mode;
            public byte prescaler_sel;
            public byte fn_h;
            public int TA;
            public int TAC;
            public byte TB;
            public int TBC;
            public int[,] dt_tab2;
            public FM_TIMERHANDLER timer_handler;
            public FM_IRQHANDLER IRQ_Handler;
            public ssg_callbacks SSG;
        }
        public struct FM_3SLOT
        {
            public uint[] fc;
            public byte fn_h;
            public byte[] kcode;
            public uint[] block_fnum;
        }
        public struct ADPCM_CH
        {
            public byte flag;
            public byte flagMask;
            public byte now_data;
            public uint now_addr;
            public uint now_step;
            public uint step;
            public uint start;
            public uint end;
            public byte IL;
            public int adpcm_acc;
            public int adpcm_step;
            public int adpcm_out;
            public sbyte vol_mul;
            public byte vol_shift;
        }
        public struct ssg_callbacks
        {
            public set_clock_handler set_clock;
            public write_handler write;
            public read_handler read;
            public reset_handler reset;
        }
        public static int TYPE_SSG = 0x01, TYPE_LFOPAN = 0x02, TYPE_6CH = 0x04, TYPE_DAC = 0x08, TYPE_ADPCM = 0x10, TYPE_2610 = 0x20;
        public static int TYPE_YM2203 = (TYPE_SSG), TYPE_YM2608 = (TYPE_SSG | TYPE_LFOPAN | TYPE_6CH | TYPE_ADPCM), TYPE_YM2610 = (TYPE_SSG | TYPE_LFOPAN | TYPE_6CH | TYPE_ADPCM | TYPE_2610), TYPE_YM2612 = (TYPE_DAC | TYPE_LFOPAN | TYPE_6CH);
        public static int[] ipan = new int[6];
        private static int[] tl_tab = new int[6656];
        private static uint[] sin_tab = new uint[0x400];
        private static int[] sl_table = new int[16]{
         ( 0*32),( 1*32),( 2*32),(3*32 ),(4*32 ),(5*32 ),(6*32 ),( 7*32),
         ( 8*32),( 9*32),(10*32),(11*32),(12*32),(13*32),(14*32),(31*32)
        };
        private static byte[] eg_inc = new byte[19 * 8]{
            0,1, 0,1, 0,1, 0,1,
            0,1, 0,1, 1,1, 0,1,
            0,1, 1,1, 0,1, 1,1,
            0,1, 1,1, 1,1, 1,1,

            1,1, 1,1, 1,1, 1,1,
            1,1, 1,2, 1,1, 1,2,
            1,2, 1,2, 1,2, 1,2,
            1,2, 2,2, 1,2, 2,2,

            2,2, 2,2, 2,2, 2,2,
            2,2, 2,4, 2,2, 2,4,
            2,4, 2,4, 2,4, 2,4,
            2,4, 4,4, 2,4, 4,4,

            4,4, 4,4, 4,4, 4,4,
            4,4, 4,8, 4,4, 4,8,
            4,8, 4,8, 4,8, 4,8,
            4,8, 8,8, 4,8, 8,8,

            8,8, 8,8, 8,8, 8,8,
            16,16,16,16,16,16,16,16,
            0,0, 0,0, 0,0, 0,0,
        };
        public static byte[] eg_rate_select = new byte[32 + 64 + 32]{
            (18*8),(18*8),(18*8),(18*8),(18*8),(18*8),(18*8),(18*8),
            (18*8),(18*8),(18*8),(18*8),(18*8),(18*8),(18*8),(18*8),
            (18*8),(18*8),(18*8),(18*8),(18*8),(18*8),(18*8),(18*8),
            (18*8),(18*8),(18*8),(18*8),(18*8),(18*8),(18*8),(18*8),

            ( 0*8),( 1*8),( 2*8),( 3*8),
            ( 0*8),( 1*8),( 2*8),( 3*8),
            ( 0*8),( 1*8),( 2*8),( 3*8),
            ( 0*8),( 1*8),( 2*8),( 3*8),
            ( 0*8),( 1*8),( 2*8),( 3*8),
            ( 0*8),( 1*8),( 2*8),( 3*8),
            ( 0*8),( 1*8),( 2*8),( 3*8),
            ( 0*8),( 1*8),( 2*8),( 3*8),
            ( 0*8),( 1*8),( 2*8),( 3*8),
            ( 0*8),( 1*8),( 2*8),( 3*8),
            ( 0*8),( 1*8),( 2*8),( 3*8),
            ( 0*8),( 1*8),( 2*8),( 3*8),

            ( 4*8),( 5*8),( 6*8),( 7*8),

            ( 8*8),( 9*8),(10*8),(11*8),

            (12*8),(13*8),(14*8),(15*8),

            (16*8),(16*8),(16*8),(16*8),

            (16*8),(16*8),(16*8),(16*8),(16*8),(16*8),(16*8),(16*8),
            (16*8),(16*8),(16*8),(16*8),(16*8),(16*8),(16*8),(16*8),
            (16*8),(16*8),(16*8),(16*8),(16*8),(16*8),(16*8),(16*8),
            (16*8),(16*8),(16*8),(16*8),(16*8),(16*8),(16*8),(16*8)
        };
        public static byte[] eg_rate_select2612 = new byte[32 + 64 + 32]{
            (18*8),(18*8),(18*8),(18*8),(18*8),(18*8),(18*8),(18*8),
            (18*8),(18*8),(18*8),(18*8),(18*8),(18*8),(18*8),(18*8),
            (18*8),(18*8),(18*8),(18*8),(18*8),(18*8),(18*8),(18*8),
            (18*8),(18*8),(18*8),(18*8),(18*8),(18*8),(18*8),(18*8),

            ( 18*8),( 18*8),( 0*8),( 0*8),
            ( 0*8),( 0*8),( 2*8),( 2*8),

            ( 0*8),( 1*8),( 2*8),( 3*8),
            ( 0*8),( 1*8),( 2*8),( 3*8),
            ( 0*8),( 1*8),( 2*8),( 3*8),
            ( 0*8),( 1*8),( 2*8),( 3*8),
            ( 0*8),( 1*8),( 2*8),( 3*8),
            ( 0*8),( 1*8),( 2*8),( 3*8),
            ( 0*8),( 1*8),( 2*8),( 3*8),
            ( 0*8),( 1*8),( 2*8),( 3*8),
            ( 0*8),( 1*8),( 2*8),( 3*8),
            ( 0*8),( 1*8),( 2*8),( 3*8),

            ( 4*8),( 5*8),( 6*8),( 7*8),

            ( 8*8),( 9*8),(10*8),(11*8),

            (12*8),(13*8),(14*8),(15*8),

            (16*8),(16*8),(16*8),(16*8),

            (16*8),(16*8),(16*8),(16*8),(16*8),(16*8),(16*8),(16*8),
            (16*8),(16*8),(16*8),(16*8),(16*8),(16*8),(16*8),(16*8),
            (16*8),(16*8),(16*8),(16*8),(16*8),(16*8),(16*8),(16*8),
            (16*8),(16*8),(16*8),(16*8),(16*8),(16*8),(16*8),(16*8)
        };
        public static byte[] eg_rate_shift = new byte[32 + 64 + 32]{
            (0),(0),(0),(0),(0),(0),(0),(0),
            (0),(0),(0),(0),(0),(0),(0),(0),
            (0),(0),(0),(0),(0),(0),(0),(0),
            (0),(0),(0),(0),(0),(0),(0),(0),

            (11),(11),(11),(11),
            (10),(10),(10),(10),
            ( 9),( 9),( 9),( 9),
            ( 8),( 8),( 8),( 8),
            ( 7),( 7),( 7),( 7),
            ( 6),( 6),( 6),( 6),
            ( 5),( 5),( 5),( 5),
            ( 4),( 4),( 4),( 4),
            ( 3),( 3),( 3),( 3),
            ( 2),( 2),( 2),( 2),
            ( 1),( 1),( 1),( 1),
            ( 0),( 0),( 0),( 0),

            ( 0),( 0),( 0),( 0),

            ( 0),( 0),( 0),( 0),

            ( 0),( 0),( 0),( 0),

            ( 0),( 0),( 0),( 0),

            ( 0),( 0),( 0),( 0),( 0),( 0),( 0),( 0),
            ( 0),( 0),( 0),( 0),( 0),( 0),( 0),( 0),
            ( 0),( 0),( 0),( 0),( 0),( 0),( 0),( 0),
            ( 0),( 0),( 0),( 0),( 0),( 0),( 0),( 0)
        };
        public static byte[] dt_tab = new byte[4 * 32]{
	        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
	        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,

	        0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2,
	        2, 3, 3, 3, 4, 4, 4, 5, 5, 6, 6, 7, 8, 8, 8, 8,

            1, 1, 1, 1, 2, 2, 2, 2, 2, 3, 3, 3, 4, 4, 4, 5,
	        5, 6, 6, 7, 8, 8, 9,10,11,12,13,14,16,16,16,16,

            2, 2, 2, 2, 2, 3, 3, 3, 4, 4, 4, 5, 5, 6, 6, 7,
	        8 , 8, 9,10,11,12,13,14,16,17,19,20,22,22,22,22
        };
        private static byte[] opn_fktable = new byte[16] { 0, 0, 0, 0, 0, 0, 0, 1, 2, 3, 3, 3, 3, 3, 3, 3 };
        private static uint[] lfo_samples_per_step = new uint[8] { 108, 77, 71, 67, 62, 44, 8, 5 };
        private static byte[] lfo_ams_depth_shift = new byte[4] { 8, 3, 1, 0 };
        private static byte[,] lfo_pm_output = new byte[56, 8]{
            {0,   0,   0,   0,   0,   0,   0,   0},
            {0,   0,   0,   0,   0,   0,   0,   0},
            {0,   0,   0,   0,   0,   0,   0,   0},
            {0,   0,   0,   0,   0,   0,   0,   0},
            {0,   0,   0,   0,   0,   0,   0,   0},
            {0,   0,   0,   0,   0,   0,   0,   0},
            {0,   0,   0,   0,   0,   0,   0,   0},
            {0,   0,   0,   0,   1,   1,   1,   1},

            {0,   0,   0,   0,   0,   0,   0,   0},
            {0,   0,   0,   0,   0,   0,   0,   0},
            {0,   0,   0,   0,   0,   0,   0,   0},
            {0,   0,   0,   0,   0,   0,   0,   0},
            {0,   0,   0,   0,   0,   0,   0,   0},
            {0,   0,   0,   0,   0,   0,   0,   0},
            {0,   0,   0,   0,   1,   1,   1,   1},
            {0,   0,   1,   1,   2,   2,   2,   3},

            {0,   0,   0,   0,   0,   0,   0,   0},
            {0,   0,   0,   0,   0,   0,   0,   0},
            {0,   0,   0,   0,   0,   0,   0,   0},
            {0,   0,   0,   0,   0,   0,   0,   0},
            {0,   0,   0,   0,   0,   0,   0,   1},
            {0,   0,   0,   0,   1,   1,   1,   1},
            {0,   0,   1,   1,   2,   2,   2,   3},
            {0,   0,   2,   3,   4,   4,   5,   6},

            {0,   0,   0,   0,   0,   0,   0,   0},
            {0,   0,   0,   0,   0,   0,   0,   0},
            {0,   0,   0,   0,   0,   0,   1,   1},
            {0,   0,   0,   0,   1,   1,   1,   1},
            {0,   0,   0,   1,   1,   1,   1,   2},
            {0,   0,   1,   1,   2,   2,   2,   3},
            {0,   0,   2,   3,   4,   4,   5,   6},
            {0,   0,   4,   6,   8,   8, 0xa, 0xc},

            {0,   0,   0,   0,   0,   0,   0,   0},
            {0,   0,   0,   0,   1,   1,   1,   1},
            {0,   0,   0,   1,   1,   1,   2,   2},
            {0,   0,   1,   1,   2,   2,   3,   3},
            {0,   0,   1,   2,   2,   2,   3,   4},
            {0,   0,   2,   3,   4,   4,   5,   6},
            {0,   0,   4,   6,   8,   8, 0xa, 0xc},
            {0,   0,   8, 0xc,0x10,0x10,0x14,0x18},

            {0,   0,   0,   0,   0,   0,   0,   0},
            {0,   0,   0,   0,   2,   2,   2,   2},
            {0,   0,   0,   2,   2,   2,   4,   4},
            {0,   0,   2,   2,   4,   4,   6,   6},
            {0,   0,   2,   4,   4,   4,   6,   8},
            {0,   0,   4,   6,   8,   8, 0xa, 0xc},
            {0,   0,   8, 0xc,0x10,0x10,0x14,0x18},
            {0,   0,0x10,0x18,0x20,0x20,0x28,0x30},

            {0,   0,   0,   0,   0,   0,   0,   0},
            {0,   0,   0,   0,   4,   4,   4,   4},
            {0,   0,   0,   4,   4,   4,   8,   8},
            {0,   0,   4,   4,   8,   8, 0xc, 0xc},
            {0,   0,   4,   8,   8,   8, 0xc,0x10},
            {0,   0,   8, 0xc,0x10,0x10,0x14,0x18},
            {0,   0,0x10,0x18,0x20,0x20,0x28,0x30},
            {0,   0,0x20,0x30,0x40,0x40,0x50,0x60},
        };
        private static int[] steps = new int[49]
        {
	         16,  17,   19,   21,   23,   25,   28,
	         31,  34,   37,   41,   45,   50,   55,
	         60,  66,   73,   80,   88,   97,  107,
	        118, 130,  143,  157,  173,  190,  209,
	        230, 253,  279,  307,  337,  371,  408,
	        449, 494,  544,  598,  658,  724,  796,
	        876, 963, 1060, 1166, 1282, 1411, 1552
        };
        public static int[] step_inc = new int[8] { -1 * 16, -1 * 16, -1 * 16, -1 * 16, 2 * 16, 5 * 16, 7 * 16, 9 * 16 };
        public static int[] jedi_table = new int[49 * 16];
        public delegate void FM_TIMERHANDLER(int c, int cnt, int clock);
        public delegate void FM_IRQHANDLER(int irq);
        public delegate void set_clock_handler(int clock);
        public delegate void write_handler(int address, byte data);
        public delegate byte read_handler();
        public delegate void reset_handler();
        private static int[] lfo_pm_table = new int[128 * 8 * 32];
        public static int[] out_fm = new int[13];
        public static int[] out_adpcm = new int[4];
        public static int[] out_delta = new int[4];
        public static byte[] ymsndrom;
        public static int LFO_AM;
        public static int LFO_PM;
        private static int fn_max;
        public static void FM_init()
        {
            init_tables();
        }
        public static int Limit(int val, int max, int min)
        {
            if (val > max)
            {
                return max;
            }
            else if (val < min)
            {
                return min;
            }
            else
            {
                return val;
            }
        }
        private static int op_calc(uint phase, uint env, int pm)
        {
            uint p;
            p = (uint)((env << 3) + sin_tab[(((int)((phase & 0xffff0000) + (pm << 15))) >> 16) & 0x3ff]);
            if (p >= 6656)
            {
                return 0;
            }
            return tl_tab[p];
        }
        private static int op_calc1(uint phase, uint env, int pm)
        {
            uint p;
            p = (uint)((env << 3) + sin_tab[(((int)((phase & 0xffff0000) + pm)) >> 16) & 0x3ff]);
            if (p >= 6656)
            {
                return 0;
            }
            return tl_tab[p];
        }
        public static int init_tables()
        {
            int i, x, n;
            double o, m;
            for (x = 0; x < 0x100; x++)
            {
                m = (1 << 16) / Math.Pow(2, (x + 1) * (1.0 / 32) / 8.0);
                m = Math.Floor(m);
                n = (int)m;
                n >>= 4;
                if ((n & 1) != 0)
                {
                    n = (n >> 1) + 1;
                }
                else
                {
                    n = n >> 1;
                }
                n <<= 2;
                tl_tab[x * 2] = n;
                tl_tab[x * 2 + 1] = -tl_tab[x * 2];
                for (i = 1; i < 13; i++)
                {
                    tl_tab[x * 2 + i * 2 * 0x100] = tl_tab[x * 2] >> i;
                    tl_tab[x * 2 + 1 + i * 2 * 0x100] = -tl_tab[x * 2 + i * 2 * 0x100];
                }
            }
            for (i = 0; i < 0x400; i++)
            {
                m = Math.Sin(((i * 2) + 1) * Math.PI / 0x400);
                if (m > 0.0)
                {
                    o = 8 * Math.Log(1.0 / m) / Math.Log(2);
                }
                else
                {
                    o = 8 * Math.Log(-1.0 / m) / Math.Log(2);
                }
                o = o / (1.0 / 32);
                n = (int)(2.0 * o);
                if ((n & 1) != 0)
                {
                    n = (n >> 1) + 1;
                }
                else
                {
                    n = n >> 1;
                }
                sin_tab[i] = (uint)(n * 2 + (m >= 0.0 ? 0 : 1));
            }
            for (i = 0; i < 8; i++)
            {
                byte fnum;
                for (fnum = 0; fnum < 128; fnum++)
                {
                    byte value;
                    byte step;
                    int offset_depth = i;
                    int offset_fnum_bit;
                    int bit_tmp;
                    for (step = 0; step < 8; step++)
                    {
                        value = 0;
                        for (bit_tmp = 0; bit_tmp < 7; bit_tmp++)
                        {
                            if ((fnum & (1 << bit_tmp)) != 0)
                            {
                                offset_fnum_bit = bit_tmp * 8;
                                value += lfo_pm_output[offset_fnum_bit + offset_depth, step];
                            }
                        }
                        lfo_pm_table[(fnum * 32 * 8) + (i * 32) + step + 0] = value;
                        lfo_pm_table[(fnum * 32 * 8) + (i * 32) + (step ^ 7) + 8] = value;
                        lfo_pm_table[(fnum * 32 * 8) + (i * 32) + step + 16] = -value;
                        lfo_pm_table[(fnum * 32 * 8) + (i * 32) + (step ^ 7) + 24] = -value;
                    }
                }
            }
            return 1;
        }
        public static void Init_ADPCMATable()
        {
            int step, nib;
            for (step = 0; step < 49; step++)
            {
                for (nib = 0; nib < 16; nib++)
                {
                    int value = (2 * (nib & 0x07) + 1) * steps[step] / 8;
                    jedi_table[step * 16 + nib] = ((nib & 0x08) != 0) ? -value : value;
                }
            }
        }
    }
    public class YM2612
    {
        public byte[] REGS;
        public FM.FM_OPN OPN;
        public byte addr_A1;
        public int dacen;
        public int dacout;
        public Timer.emu_timer[] timer;
        public sound_stream stream;
        public static int dacena;
        public static YM2612[] FF2612 = new YM2612[2] { new YM2612(), new YM2612() };
        public void ym2612_update_one(int offset, int length)
        {
            int i;
            int dacouta = dacout;
            dacena = dacen;
            OPN.refresh_fc_eg_chan(OPN.type, 0);
            OPN.refresh_fc_eg_chan(OPN.type, 1);
            if ((OPN.ST.mode & 0xc0) != 0)
            {
                if (OPN.CH[2].SLOT[0].Incr == -1)
                {
                    OPN.refresh_fc_eg_slot(OPN.type, 2, 0, (int)OPN.SL3.fc[1], OPN.SL3.kcode[1]);
                    OPN.refresh_fc_eg_slot(OPN.type, 2, 2, (int)OPN.SL3.fc[2], OPN.SL3.kcode[2]);
                    OPN.refresh_fc_eg_slot(OPN.type, 2, 1, (int)OPN.SL3.fc[0], OPN.SL3.kcode[0]);
                    OPN.refresh_fc_eg_slot(OPN.type, 2, 3, (int)OPN.CH[2].fc, OPN.CH[2].kcode);
                }
            }
            else
            {
                OPN.refresh_fc_eg_chan(OPN.type, 2);
            }
            OPN.refresh_fc_eg_chan(OPN.type, 3);
            OPN.refresh_fc_eg_chan(OPN.type, 4);
            OPN.refresh_fc_eg_chan(OPN.type, 5);
            for (i = 0; i < length; i++)
            {
                OPN.advance_lfo();
                FM.out_fm[0] = 0;
                FM.out_fm[1] = 0;
                FM.out_fm[2] = 0;
                FM.out_fm[3] = 0;
                FM.out_fm[4] = 0;
                FM.out_fm[5] = 0;
                OPN.eg_timer += OPN.eg_timer_add;
                while (OPN.eg_timer >= OPN.eg_timer_overflow)
                {
                    OPN.eg_timer -= OPN.eg_timer_overflow;
                    OPN.eg_cnt++;
                    OPN.advance_eg_channel(0);
                    OPN.advance_eg_channel(1);
                    OPN.advance_eg_channel(2);
                    OPN.advance_eg_channel(3);
                    OPN.advance_eg_channel(4);
                    OPN.advance_eg_channel(5);
                }
                OPN.chan_calc(0, 0);
                OPN.chan_calc(1, 1);
                OPN.chan_calc(2, 2);
                OPN.chan_calc(3, 3);
                OPN.chan_calc(4, 4);
                if (dacena != 0)
                {
                    OPN.iconnect4[5] = dacouta;
                }
                else
                {
                    OPN.chan_calc(5, 5);
                }
                {
                    int lt, rt;
                    lt = (int)((FM.out_fm[0] >> 0) & OPN.pan[0]);
                    rt = (int)((FM.out_fm[0] >> 0) & OPN.pan[1]);
                    lt += (int)((FM.out_fm[1] >> 0) & OPN.pan[2]);
                    rt += (int)((FM.out_fm[1] >> 0) & OPN.pan[3]);
                    lt += (int)((FM.out_fm[2] >> 0) & OPN.pan[4]);
                    rt += (int)((FM.out_fm[2] >> 0) & OPN.pan[5]);
                    lt += (int)((FM.out_fm[3] >> 0) & OPN.pan[6]);
                    rt += (int)((FM.out_fm[3] >> 0) & OPN.pan[7]);
                    lt += (int)((FM.out_fm[4] >> 0) & OPN.pan[8]);
                    rt += (int)((FM.out_fm[4] >> 0) & OPN.pan[9]);
                    lt += (int)((FM.out_fm[5] >> 0) & OPN.pan[10]);
                    rt += (int)((FM.out_fm[5] >> 0) & OPN.pan[11]);
                    lt = FM.Limit(lt, 32767, -32768);
                    rt = FM.Limit(rt, 32767, -32768);
                    stream.streamoutput[0][offset + i] = lt;
                    stream.streamoutput[1][offset + i] = rt;
                }
            }
        }
        public void ym2612_postload()
        {
            int r;
            dacout = ((int)REGS[0x2a] - 0x80) << 6;
            dacen = REGS[0x2d] & 0x80;
            for (r = 0x30; r < 0x9e; r++)
            {
                if ((r & 3) != 3)
                {
                    OPN.OPNWriteReg(r, REGS[r]);
                    OPN.OPNWriteReg(r | 0x100, REGS[r | 0x100]);
                }
            }
            for (r = 0xb0; r < 0xb6; r++)
            {
                if ((r & 3) != 3)
                {
                    OPN.OPNWriteReg(r, REGS[r]);
                    OPN.OPNWriteReg(r | 0x100, REGS[r | 0x100]);
                }
            }
        }
        public static void ym2612_init(int index, int clock, int rate, FM.FM_TIMERHANDLER timer_handler, FM.FM_IRQHANDLER IRQHandler)
        {
            int i;
            FF2612[index].REGS = new byte[512];
            FF2612[index].OPN = new FM.FM_OPN();
            FF2612[index].addr_A1 = 0;
            FF2612[index].dacen = 0;
            FF2612[index].dacout = 0;
            FM.init_tables();
            FF2612[index].OPN.type = FM.TYPE_YM2612;
            FF2612[index].OPN.ST.clock = clock;
            FF2612[index].OPN.ST.rate = rate;
            FF2612[index].OPN.ST.timer_handler = timer_handler;
            FF2612[index].OPN.ST.IRQ_Handler = IRQHandler;
            FF2612[index].ym2612_reset_chip();
        }
        public void ym2612_reset_chip()
        {
            int i;
            OPN.OPNSetPres(6 * 24, 6 * 24, 0);
            OPN.FM_IRQMASK_SET(0x03);
            OPN.FM_BUSY_CLEAR();
            OPN.OPNWriteMode(0x27, 0x30);
            OPN.eg_timer = 0;
            OPN.eg_cnt = 0;
            OPN.FM_STATUS_RESET(0xff);
            OPN.reset_channels(6);
            for (i = 0xb6; i >= 0xb4; i--)
            {
                OPN.OPNWriteReg(i, 0xc0);
                OPN.OPNWriteReg(i | 0x100, 0xc0);
            }
            for (i = 0xb2; i >= 0x30; i--)
            {
                OPN.OPNWriteReg(i, 0);
                OPN.OPNWriteReg(i | 0x100, 0);
            }
            for (i = 0x26; i >= 0x20; i--)
            {
                OPN.OPNWriteReg(i, 0);
            }
            dacen = 0;
        }
        public void ym2612_write(int a, byte v)
        {
            int addr;
            v &= 0xff;
            switch (a & 3)
            {
                case 0:
                    OPN.ST.address = v;
                    addr_A1 = 0;
                    break;
                case 1:
                    if (addr_A1 != 0)
                    {
                        break;
                    }
                    addr = OPN.ST.address;
                    REGS[addr] = v;
                    switch (addr & 0xf0)
                    {
                        case 0x20:
                            switch (addr)
                            {
                                case 0x2a:
                                    stream.stream_update();
                                    dacout = ((int)v - 0x80) << 6;
                                    break;
                                case 0x2b:
                                    dacen = v & 0x80;
                                    break;
                                default:
                                    stream.stream_update();
                                    OPN.OPNWriteMode(addr, v);
                                    break;
                            }
                            break;
                        default:
                            stream.stream_update();
                            OPN.OPNWriteReg(addr, v);
                            break;
                    }
                    break;
                case 2:
                    OPN.ST.address = v;
                    addr_A1 = 1;
                    break;
                case 3:
                    if (addr_A1 != 1)
                    {
                        break;
                    }
                    addr = OPN.ST.address;
                    REGS[addr | 0x100] = v;
                    stream.stream_update();
                    OPN.OPNWriteReg(addr | 0x100, v);
                    break;
            }
        }
        public byte ym2612_read(int a)
        {
            switch (a & 3)
            {
                case 0:
                    return OPN.FM_STATUS_FLAG();
                case 1:
                case 2:
                case 3:
                    return OPN.FM_STATUS_FLAG();
            }
            return 0;
        }
        public void ym2612_timer_over(int c)
        {
            if (c != 0)
            {
                OPN.TimerBOver();
            }
            else
            {
                stream.stream_update();
                OPN.TimerAOver();
                if ((OPN.ST.mode & 0x80) != 0)
                {
                    OPN.CSMKeyControll();
                }
            }
        }
        public static void timer_callback_2612_0_0()
        {
            FF2612[0].ym2612_timer_over(0);
        }
        public static void timer_callback_2612_0_1()
        {
            FF2612[0].ym2612_timer_over(1);
        }
        public static void timer_callback_2612_1_0()
        {
            FF2612[1].ym2612_timer_over(0);
        }
        public static void timer_callback_2612_1_1()
        {
            FF2612[1].ym2612_timer_over(1);
        }
        public void timer_callback_2612_0()
        {
            ym2612_timer_over(0);
        }
        public void timer_callback_2612_1()
        {
            ym2612_timer_over(1);
        }
        public void timer_handler(int c, int count, int clock)
        {
            if (count == 0)
            {
                Timer.timer_enable(timer[c], false);
            }
            else
            {
                Atime period = Attotime.attotime_mul(Attotime.ATTOTIME_IN_HZ(clock), (uint)count);
                if (Timer.timer_enable(timer[c], true) == false)
                {
                    Timer.timer_adjust_periodic(timer[c], period, Attotime.ATTOTIME_NEVER);
                }
            }
        }
        public void SaveStateBinary(BinaryWriter writer)
        {
            int i, j;
            writer.Write(REGS, 0, 0x200);
            writer.Write(OPN.ST.freqbase);
            writer.Write(OPN.ST.timer_prescaler);
            writer.Write(OPN.ST.busy_expiry_time.seconds);
            writer.Write(OPN.ST.busy_expiry_time.attoseconds);
            writer.Write(OPN.ST.address);
            writer.Write(OPN.ST.irq);
            writer.Write(OPN.ST.irqmask);
            writer.Write(OPN.ST.status);
            writer.Write(OPN.ST.mode);
            writer.Write(OPN.ST.prescaler_sel);
            writer.Write(OPN.ST.fn_h);
            writer.Write(OPN.ST.TA);
            writer.Write(OPN.ST.TAC);
            writer.Write(OPN.ST.TB);
            writer.Write(OPN.ST.TBC);
            for (i = 0; i < 12; i++)
            {
                writer.Write(OPN.pan[i]);
            }
            writer.Write(OPN.eg_cnt);
            writer.Write(OPN.eg_timer);
            writer.Write(OPN.eg_timer_add);
            writer.Write(OPN.eg_timer_overflow);
            writer.Write(OPN.lfo_cnt);
            writer.Write(OPN.lfo_inc);
            for (i = 0; i < 8; i++)
            {
                writer.Write(OPN.lfo_freq[i]);
            }
            for (i = 0; i < 3; i++)
            {
                writer.Write(OPN.SL3.fc[i]);
            }
            writer.Write(OPN.SL3.fn_h);
            writer.Write(OPN.SL3.kcode, 0, 3);
            for (i = 0; i < 3; i++)
            {
                writer.Write(OPN.SL3.block_fnum[i]);
            }
            for (i = 0; i < 6; i++)
            {
                for (j = 0; j < 4; j++)
                {
                    writer.Write(OPN.CH[i].SLOT[j].KSR);
                    writer.Write(OPN.CH[i].SLOT[j].ar);
                    writer.Write(OPN.CH[i].SLOT[j].d1r);
                    writer.Write(OPN.CH[i].SLOT[j].d2r);
                    writer.Write(OPN.CH[i].SLOT[j].rr);
                    writer.Write(OPN.CH[i].SLOT[j].ksr);
                    writer.Write(OPN.CH[i].SLOT[j].mul);
                    writer.Write(OPN.CH[i].SLOT[j].phase);
                    writer.Write(OPN.CH[i].SLOT[j].Incr);
                    writer.Write(OPN.CH[i].SLOT[j].state);
                    writer.Write(OPN.CH[i].SLOT[j].tl);
                    writer.Write(OPN.CH[i].SLOT[j].volume);
                    writer.Write(OPN.CH[i].SLOT[j].sl);
                    writer.Write(OPN.CH[i].SLOT[j].vol_out);
                    writer.Write(OPN.CH[i].SLOT[j].eg_sh_ar);
                    writer.Write(OPN.CH[i].SLOT[j].eg_sel_ar);
                    writer.Write(OPN.CH[i].SLOT[j].eg_sh_d1r);
                    writer.Write(OPN.CH[i].SLOT[j].eg_sel_d1r);
                    writer.Write(OPN.CH[i].SLOT[j].eg_sh_d2r);
                    writer.Write(OPN.CH[i].SLOT[j].eg_sel_d2r);
                    writer.Write(OPN.CH[i].SLOT[j].eg_sh_rr);
                    writer.Write(OPN.CH[i].SLOT[j].eg_sel_rr);
                    writer.Write(OPN.CH[i].SLOT[j].ssg);
                    writer.Write(OPN.CH[i].SLOT[j].ssgn);
                    writer.Write(OPN.CH[i].SLOT[j].key);
                    writer.Write(OPN.CH[i].SLOT[j].AMmask);
                }
            }
            writer.Write(addr_A1);
            writer.Write(dacen);
            writer.Write(dacout);
        }
        public void LoadStateBinary(BinaryReader reader)
        {
            int i, j;
            REGS = reader.ReadBytes(0x200);
            OPN.ST.freqbase = reader.ReadDouble();
            OPN.ST.timer_prescaler = reader.ReadInt32();
            OPN.ST.busy_expiry_time.seconds = reader.ReadInt32();
            OPN.ST.busy_expiry_time.attoseconds = reader.ReadInt64();
            OPN.ST.address = reader.ReadByte();
            OPN.ST.irq = reader.ReadByte();
            OPN.ST.irqmask = reader.ReadByte();
            OPN.ST.status = reader.ReadByte();
            OPN.ST.mode = reader.ReadByte();
            OPN.ST.prescaler_sel = reader.ReadByte();
            OPN.ST.fn_h = reader.ReadByte();
            OPN.ST.TA = reader.ReadInt32();
            OPN.ST.TAC = reader.ReadInt32();
            OPN.ST.TB = reader.ReadByte();
            OPN.ST.TBC = reader.ReadInt32();
            for (i = 0; i < 12; i++)
            {
                OPN.pan[i] = reader.ReadUInt32();
            }
            OPN.eg_cnt = reader.ReadUInt32();
            OPN.eg_timer = reader.ReadUInt32();
            OPN.eg_timer_add = reader.ReadUInt32();
            OPN.eg_timer_overflow = reader.ReadUInt32();
            OPN.lfo_cnt = reader.ReadInt32();
            OPN.lfo_inc = reader.ReadInt32();
            for (i = 0; i < 8; i++)
            {
                OPN.lfo_freq[i] = reader.ReadInt32();
            }
            for (i = 0; i < 3; i++)
            {
                OPN.SL3.fc[i] = reader.ReadUInt32();
            }
            OPN.SL3.fn_h = reader.ReadByte();
            OPN.SL3.kcode = reader.ReadBytes(3);
            for (i = 0; i < 3; i++)
            {
                OPN.SL3.block_fnum[i] = reader.ReadUInt32();
            }
            for (i = 0; i < 6; i++)
            {
                for (j = 0; j < 4; j++)
                {
                    OPN.CH[i].SLOT[j].KSR = reader.ReadByte();
                    OPN.CH[i].SLOT[j].ar = reader.ReadInt32();
                    OPN.CH[i].SLOT[j].d1r = reader.ReadInt32();
                    OPN.CH[i].SLOT[j].d2r = reader.ReadInt32();
                    OPN.CH[i].SLOT[j].rr = reader.ReadInt32();
                    OPN.CH[i].SLOT[j].ksr = reader.ReadByte();
                    OPN.CH[i].SLOT[j].mul = reader.ReadInt32();
                    OPN.CH[i].SLOT[j].phase = reader.ReadUInt32();
                    OPN.CH[i].SLOT[j].Incr = reader.ReadInt32();
                    OPN.CH[i].SLOT[j].state = reader.ReadByte();
                    OPN.CH[i].SLOT[j].tl = reader.ReadInt32();
                    OPN.CH[i].SLOT[j].volume = reader.ReadInt32();
                    OPN.CH[i].SLOT[j].sl = reader.ReadInt32();
                    OPN.CH[i].SLOT[j].vol_out = reader.ReadUInt32();
                    OPN.CH[i].SLOT[j].eg_sh_ar = reader.ReadByte();
                    OPN.CH[i].SLOT[j].eg_sel_ar = reader.ReadByte();
                    OPN.CH[i].SLOT[j].eg_sh_d1r = reader.ReadByte();
                    OPN.CH[i].SLOT[j].eg_sel_d1r = reader.ReadByte();
                    OPN.CH[i].SLOT[j].eg_sh_d2r = reader.ReadByte();
                    OPN.CH[i].SLOT[j].eg_sel_d2r = reader.ReadByte();
                    OPN.CH[i].SLOT[j].eg_sh_rr = reader.ReadByte();
                    OPN.CH[i].SLOT[j].eg_sel_rr = reader.ReadByte();
                    OPN.CH[i].SLOT[j].ssg = reader.ReadByte();
                    OPN.CH[i].SLOT[j].ssgn = reader.ReadByte();
                    OPN.CH[i].SLOT[j].key = reader.ReadUInt32();
                    OPN.CH[i].SLOT[j].AMmask = reader.ReadUInt32();
                }
            }
            addr_A1 = reader.ReadByte();
            dacen = reader.ReadInt32();
            dacout = reader.ReadInt32();
        }
        public static void ym2612_start(int sndindex, int clock)
        {
            int rate = clock / 72;
            FF2612[sndindex] = new YM2612();
            FF2612[sndindex].timer = new Timer.emu_timer[2];
            if (sndindex == 0)
            {
                FF2612[0].timer[0] = Timer.timer_alloc_common(FF2612[0].timer_callback_2612_0, "timer_callback_2612_0_0", false);
                FF2612[0].timer[1] = Timer.timer_alloc_common(FF2612[0].timer_callback_2612_1, "timer_callback_2612_0_1", false);
            }
            else if (sndindex == 1)
            {
                FF2612[1].timer[0] = Timer.timer_alloc_common(FF2612[1].timer_callback_2612_0, "timer_callback_2612_1_0", false);
                FF2612[1].timer[1] = Timer.timer_alloc_common(FF2612[1].timer_callback_2612_1, "timer_callback_2612_1_1", false);
            }
            FF2612[sndindex].stream = new sound_stream(rate, 0, 2, FF2612[sndindex].ym2612_update_one);
            ym2612_init(sndindex, clock, rate, FF2612[sndindex].timer_handler, null);
        }
    }
}
