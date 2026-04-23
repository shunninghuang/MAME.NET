using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using cpu.z80;
using cpu.i8x41;

namespace mame
{
    public partial class Taito
    {
        public static byte[] tnzs_bank1, tnzs_bank2, tnzs_bank3, tnzs_objram, tnzs_sharedram, tnzs_vdcram, tnzs_scrollram, tnzs_objctrl;
        public static byte tnzs_bg_flag;
        public static int mcu_type, tnzs_input_select;
        public static int mcu_initializing, mcu_coinage_init, mcu_command, mcu_readcredits;
        public static int mcu_reportcoin, kageki_csport_sel;
        public static ushort up1x, up2x;
        public static byte[] mcu_coinage = new byte[4];
        public static short[][] sampledata = new short[0x2f][];
        public static int[] samplesize = new int[0x2f];
        public static byte mcu_coinsA, mcu_coinsB, mcu_credits;
        public static int insertcoin, tnzs_screenflip;
        public const int MCU_NONE_INSECTX = 1, MCU_NONE_KAGEKI = 2, MCU_NONE_TNZSB = 3, MCU_NONE_KABUKIZ = 4, MCU_EXTRMATN = 5, MCU_ARKANOID = 6, MCU_PLUMPOP = 7, MCU_DRTOPPEL = 8, MCU_CHUKATAI = 9, MCU_TNZS = 10;
        public static byte dswa_r(int offset)
        {
            return dswa;
        }
        public static byte dswb_r(int offset)
        {
            return dswb;
        }
        public static void kageki_init_samples()
        {
            int start, size;
            int i, j, n;
            int src_offset = 0x0090, scan_offset;
            for (i = 0; i < 0x2f; i++)
            {
                start = samplesrom[src_offset + i * 2 + 1] * 256 + samplesrom[src_offset + i * 2];
                scan_offset = src_offset + start;
                size = 0;
                for (j = 0; j < 0x10000; j++)
                {                    
                    if (samplesrom[scan_offset] == 0x00)
                    {
                        break;
                    }
                    else
                    {
                        scan_offset++;
                        size++;
                    }
                }
                sampledata[i] = new short[size];
                samplesize[i] = size;
                if (start < 0x100)
                {
                    start = size = 0;
                }
                for (n = 0; n < size; n++)
                {
                    sampledata[i][n] = (short)((sbyte)(samplesrom[start + n] ^ 0x80) * 256);
                }
            }
        }
        public static byte kageki_csport_r(int offset)
        {
            int dsw, dsw1, dsw2;
            dsw1 = dswa;
            dsw2 = dswb;
            switch (kageki_csport_sel)
            {
                case 0x00:			// DSW2 5,1 / DSW1 5,1
                    dsw = (((dsw2 & 0x10) >> 1) | ((dsw2 & 0x01) << 2) | ((dsw1 & 0x10) >> 3) | ((dsw1 & 0x01) >> 0));
                    break;
                case 0x01:			// DSW2 7,3 / DSW1 7,3
                    dsw = (((dsw2 & 0x40) >> 3) | ((dsw2 & 0x04) >> 0) | ((dsw1 & 0x40) >> 5) | ((dsw1 & 0x04) >> 2));
                    break;
                case 0x02:			// DSW2 6,2 / DSW1 6,2
                    dsw = (((dsw2 & 0x20) >> 2) | ((dsw2 & 0x02) << 1) | ((dsw1 & 0x20) >> 4) | ((dsw1 & 0x02) >> 1));
                    break;
                case 0x03:			// DSW2 8,4 / DSW1 8,4
                    dsw = (((dsw2 & 0x80) >> 4) | ((dsw2 & 0x08) >> 1) | ((dsw1 & 0x80) >> 6) | ((dsw1 & 0x08) >> 3));
                    break;
                default:
                    dsw = 0x00;
                    break;
            }
            return (byte)(dsw & 0xff);
        }
        public static void kageki_csport_w(int offset, byte data)
        {
            if (data > 0x3f)
            {
                kageki_csport_sel = (data & 0x03);
            }
            else
            {
                if (data > 0x2f)
                {
                    Sample.sample_stop(0);
                }
                else
                {
                    Sample.sample_start_raw(0, sampledata[data], samplesize[data], 7000, 0);
                }
            }
        }
        public static void kabukiz_sound_bank_w(int offset, byte data)
        {
            if (data != 0xff)
            {
                basebankaudio = 0x400 * (data & 0x07);
            }
        }
        public static void kabukiz_sample_w(int offset, byte data)
        {
            if (data != 0xff)
            {
                DAC.dac_0_data_w(0, data);
            }
        }
        public static byte mcu_tnzs_r(int offset)
        {
            byte data = 0;
            if (offset == 0)
            {
                I8x41.m1.STATE &= unchecked((byte)~I8x41.OBF);
                if ((I8x41.m1.ENABLE & I8x41.FLAGS)!=0)
                {
                    I8x41.m1.P2_HS &= 0xef;
                    if ((I8x41.m1.STATE & I8x41.IBF) != 0)
                    {
                        I8x41.m1.P2_HS |= 0x20;
                    }
                    else
                    {
                        I8x41.m1.P2_HS &= 0xdf;
                    }
                    I8x41.m1.WP(0x02, (byte)(I8x41.m1.P2 & I8x41.m1.P2_HS));
                }
                data = I8x41.m1.DBBO;
                Cpuexec.cpu_yield();
            }
            else
            {
                data = I8x41.m1.STATE;
                Cpuexec.cpu_yield();
            }
            return data;
        }
        public static void mcu_tnzs_w(int offset, byte data)
        {
            if (offset == 0)
            {
                I8x41.m1.DBBI = data;
                if (I8x41.m1.subtype == 8041)
                {
                    I8x41.m1.DBBO = data;
                }
                I8x41.m1.STATE &= unchecked((byte)~I8x41.F1);
                I8x41.m1.STATE |= I8x41.IBF;
                if ((I8x41.m1.ENABLE & I8x41.IBFI)!=0)
                {
                    I8x41.m1.CONTROL |= I8x41.IBFI_PEND;
                }
                if ((I8x41.m1.ENABLE & I8x41.FLAGS)!=0)
                {
                    I8x41.m1.P2_HS |= 0x20;
                    if (0 == (I8x41.m1.STATE & I8x41.OBF))
                    {
                        I8x41.m1.P2_HS |= 0x10;
                    }
                    else
                    {
                        I8x41.m1.P2_HS &= 0xef;
                    }
                    I8x41.m1.WP(0x02, (byte)(I8x41.m1.P2 & I8x41.m1.P2_HS));
                }
            }
            else
            {
                I8x41.m1.DBBI = data;
                if (I8x41.m1.subtype == 8041)
                {
                    I8x41.m1.DBBO = data;
                }
                I8x41.m1.STATE |= I8x41.F1;
                I8x41.m1.STATE |= I8x41.IBF;
                if ((I8x41.m1.ENABLE & I8x41.IBFI) != 0)
                {
                    I8x41.m1.CONTROL |= I8x41.IBFI_PEND;
                }
                if ((I8x41.m1.ENABLE & I8x41.FLAGS) != 0)
                {
                    I8x41.m1.P2_HS |= 0x20;
                    if (0 == (I8x41.m1.STATE & I8x41.OBF))
                    {
                        I8x41.m1.P2_HS |= 0x10;
                    }
                    else
                    {
                        I8x41.m1.P2_HS &= 0xef;
                    }
                    I8x41.m1.WP(0x02, (byte)(I8x41.m1.P2 & I8x41.m1.P2_HS));
                }
            }
        }
        public static byte tnzs_port1_r()
        {
            byte data = 0;
            switch (tnzs_input_select & 0x0f)
            {
                case 0x0a: data = (byte)sbyte2; break;
                case 0x0c: data = (byte)sbyte0; break;
                case 0x0d: data = (byte)sbyte1; break;
                default: data = 0xff; break;
            }
            return data;
        }
        public static byte tnzs_port2_r()
        {
            byte data = (byte)sbyte2;
            return data;
        }
        public static void tnzs_port2_w(byte data)
        {
            Generic.coin_lockout_w(0, (data & 0x40));
            Generic.coin_lockout_w(1, (data & 0x80));
            Generic.coin_counter_w(0, (~data & 0x10));
            Generic.coin_counter_w(1, (~data & 0x20));
            tnzs_input_select = data;
        }
        public static void tnzs_port2_w_2(byte data)
        {
            Generic.coin_lockout_w(0, (data & 0x40) != 0 ? 0 : 1);
            Generic.coin_lockout_w(1, (data & 0x80) != 0 ? 0 : 1);
            Generic.coin_counter_w(0, (~data & 0x10));
            Generic.coin_counter_w(1, (~data & 0x20));
            tnzs_input_select = data;
        }
        public static byte arknoid2_sh_f000_r(int offset)
        {
            int val;
            up1x = (ushort)Inptport.input_port_read_direct(Inptport.analog_p1x);
            up2x = (ushort)Inptport.input_port_read_direct(Inptport.analog_p2x);
            if (up1h != 0)
            {
                up1x |= up1h;
            }
            if (up2h != 0)
            {
                up2x |= up2h;
            }
            val = (offset / 2) != 0 ? up2x : up1x;
            if ((offset & 1) != 0)
            {
                return (byte)((val >> 8) & 0xff);
            }
            else
            {
                return (byte)(val & 0xff);
            }
        }
        public static void mcu_reset()
        {
            mcu_initializing = 3;
            mcu_coinage_init = 0;
            mcu_coinage[0] = 1;
            mcu_coinage[1] = 1;
            mcu_coinage[2] = 1;
            mcu_coinage[3] = 1;
            mcu_coinsA = 0;
            mcu_coinsB = 0;
            mcu_credits = 0;
            mcu_reportcoin = 0;
            mcu_command = 0;
        }
        static void mcu_handle_coins(int coin)
        {
            if ((coin & 0x08) != 0)
            {
                mcu_reportcoin = coin;
            }
            else if (coin != 0 && coin != insertcoin)
            {
                if ((coin & 0x01) != 0)
                {
                    Generic.coin_counter_w(0, 1);
                    Generic.coin_counter_w(0, 0);
                    mcu_coinsA++;
                    if (mcu_coinsA >= mcu_coinage[0])
                    {
                        mcu_coinsA -= mcu_coinage[0];
                        mcu_credits += mcu_coinage[1];
                        if (mcu_credits >= 9)
                        {
                            mcu_credits = 9;
                            Generic.coin_lockout_global_w(1);
                        }
                        else
                        {
                            Generic.coin_lockout_global_w(0);
                        }
                    }
                }
                if ((coin & 0x02) != 0)
                {
                    Generic.coin_counter_w(1, 1);
                    Generic.coin_counter_w(1, 0);
                    mcu_coinsB++;
                    if (mcu_coinsB >= mcu_coinage[2])
                    {
                        mcu_coinsB -= mcu_coinage[2];
                        mcu_credits += mcu_coinage[3];
                        if (mcu_credits >= 9)
                        {
                            mcu_credits = 9;
                            Generic.coin_lockout_global_w(1);
                        }
                        else
                        {
                            Generic.coin_lockout_global_w(0);
                        }
                    }
                }
                if ((coin & 0x04) != 0)
                {
                    mcu_credits++;
                }
                mcu_reportcoin = coin;
            }
            else
            {
                if (mcu_credits < 9)
                {
                    Generic.coin_lockout_global_w(0);
                }
                mcu_reportcoin = 0;
            }
            if (coin == 1)
            {
                int i1 = 1;
            }
            insertcoin = coin;
        }
        public static byte mcu_arknoid2_r(int offset)
        {
            byte[] mcu_startup = new byte[] { 0x55, 0xaa, 0x5a };
            if (offset == 0)
            {
                if (mcu_initializing != 0)
                {
                    mcu_initializing--;
                    return mcu_startup[2 - mcu_initializing];
                }
                switch (mcu_command)
                {
                    case 0x41:
                        return mcu_credits;
                    case 0xc1:
                        if (mcu_readcredits == 0)
                        {
                            mcu_readcredits = 1;
                            if ((mcu_reportcoin & 0x08) != 0)
                            {
                                mcu_initializing = 3;
                                return 0xee;
                            }
                            else
                            {
                                return mcu_credits;
                            }
                        }
                        else
                        {
                            return (byte)sbyte0;
                        }
                    default:
                        return 0xff;
                }
            }
            else
            {
                if ((mcu_reportcoin & 0x08) != 0)
                {
                    return 0xe1;
                }
                if ((mcu_reportcoin & 0x01) != 0)
                {
                    return 0x11;
                }
                if ((mcu_reportcoin & 0x02) != 0)
                {
                    return 0x21;
                }
                if ((mcu_reportcoin & 0x04) != 0)
                {
                    return 0x31;
                }
                return 0x01;
            }
        }
        public static void mcu_arknoid2_w(int offset, byte data)
        {
            if (offset == 0)
            {
                if (mcu_command == 0x41)
                {
                    mcu_credits = (byte)((mcu_credits + data) & 0xff);
                }
            }
            else
            {
                if (mcu_initializing != 0)
                {
                    mcu_coinage[mcu_coinage_init++] = data;
                    if (mcu_coinage_init == 4)
                    {
                        mcu_coinage_init = 0;
                    }
                }
                if (data == 0xc1)
                {
                    mcu_readcredits = 0;
                }
                if (data == 0x15)
                {
                    mcu_credits = (byte)((mcu_credits - 1) & 0xff);
                    if (mcu_credits == 0xff)
                    {
                        mcu_credits = 0;
                    }
                }
                mcu_command = data;
            }
        }
        public static byte mcu_extrmatn_r(int offset)
        {
            byte[] mcu_startup = new byte[] { 0x5a, 0xa5, 0x55 };
            /*if (Video.screenstate.frame_number >= 0x250 && Video.screenstate.frame_number <= 0x251)
            {
                bcoin1 = 0x01;
                sbyte2 = unchecked((sbyte)0xdf);
            }
            if (Video.screenstate.frame_number >= 0x2a0 && Video.screenstate.frame_number <= 0x2a1)
            {
                sbyte0 = 0x7f;
            }*/
            if (offset == 0)
            {
                if (mcu_initializing != 0)
                {
                    mcu_initializing--;
                    return mcu_startup[2 - mcu_initializing];
                }
                switch (mcu_command)
                {
                    case 0x01:
                        return (byte)(sbyte0 ^ 0xff);
                    case 0x02:
                        return (byte)(sbyte1 ^ 0xff);
                    case 0x1a:
                        return (byte)(bcoin1 | (bcoin2 << 1));
                    case 0x21:
                        return (byte)(sbyte2 & 0x0f);
                    case 0x41:
                        return mcu_credits;
                    case 0xa0:
                        if ((mcu_reportcoin & 0x08) != 0)
                        {
                            mcu_initializing = 3;
                            return 0xee;
                        }
                        else
                        {
                            return mcu_credits;
                        }
                    case 0xa1:
                        if (mcu_readcredits == 0)
                        {
                            mcu_readcredits = 1;
                            if ((mcu_reportcoin & 0x08) != 0)
                            {
                                mcu_initializing = 3;
                                return 0xee;
                            }
                            else
                            {
                                return mcu_credits;
                            }
                        }
                        else
                        {
                            return (byte)((((byte)sbyte0 & 0xf0) | ((byte)sbyte1 >> 4)) ^ 0xff);
                        }
                    default:
                        return 0xff;
                }
            }
            else
            {
                if ((mcu_reportcoin & 0x08) != 0)
                {
                    return 0xe1;
                }
                if ((mcu_reportcoin & 0x01) != 0)
                {
                    return 0x11;
                }
                if ((mcu_reportcoin & 0x02) != 0)
                {
                    return 0x21;
                }
                if ((mcu_reportcoin & 0x04) != 0)
                {
                    return 0x31;
                }
                return 0x01;
            }
        }
        public static void mcu_extrmatn_w(int offset, byte data)
        {
            if (offset == 0)
            {
                if (mcu_command == 0x41)
                {
                    mcu_credits = (byte)((mcu_credits + data) & 0xff);
                }
            }
            else
            {
                if (mcu_initializing != 0)
                {
                    mcu_coinage[mcu_coinage_init++] = data;
                    if (mcu_coinage_init == 4)
                    {
                        mcu_coinage_init = 0;
                    }
                }
                if (data == 0xa1)
                {
                    mcu_readcredits = 0;
                }
                if ((data == 0x09) && (mcu_type == MCU_DRTOPPEL || mcu_type == MCU_PLUMPOP))
                {
                    mcu_credits = (byte)((mcu_credits - 1) & 0xff);
                }
                if ((data == 0x18) && (mcu_type == MCU_DRTOPPEL || mcu_type == MCU_PLUMPOP))
                {
                    mcu_credits = (byte)((mcu_credits - 2) & 0xff);
                }
                mcu_command = data;
            }
        }
        public static byte tnzs_mcu_r(int offset)
        {
            switch (mcu_type)
            {
                case MCU_TNZS:
                case MCU_CHUKATAI:
                    return mcu_tnzs_r(offset);
                case MCU_ARKANOID:
                    return mcu_arknoid2_r(offset);
                case MCU_EXTRMATN:
                case MCU_DRTOPPEL:
                case MCU_PLUMPOP:
                    return mcu_extrmatn_r(offset);
                default:
                    return 0xff;
            }
        }
        public static void tnzs_mcu_w(int offset, byte data)
        {
            switch (mcu_type)
            {
                case MCU_TNZS:
                case MCU_CHUKATAI:
                    mcu_tnzs_w(offset, data);
                    break;
                case MCU_ARKANOID:
                    mcu_arknoid2_w(offset, data);
                    break;
                case MCU_EXTRMATN:
                case MCU_DRTOPPEL:
                case MCU_PLUMPOP:
                    mcu_extrmatn_w(offset, data);
                    break;
                default:
                    break;
            }
        }
        public static void arknoid2_interrupt()
        {
            int coin;
            /*if (Video.screenstate.frame_number >= 0x250 && Video.screenstate.frame_number <= 0x251)
            {
                bcoin1 = 0x01;
                sbyte2 = unchecked((sbyte)0xdf);
            }*/
            switch (mcu_type)
            {
                case MCU_ARKANOID:
                case MCU_EXTRMATN:
                case MCU_DRTOPPEL:
                case MCU_PLUMPOP:
                    coin = 0;
                    coin |= ((bcoin1 & 1) << 0);
                    coin |= ((bcoin2 & 1) << 1);
                    coin |= ((sbyte2 & 3) << 2);
                    coin ^= 0x0c;
                    mcu_handle_coins(coin);
                    break;
                default:
                    break;
            }
            Cpuint.cpunum_set_input_line(0, 0, LineState.HOLD_LINE);
        }
        public static void machine_reset_tnzs()
        {
            switch (mcu_type)
            {
                case MCU_ARKANOID:
                case MCU_EXTRMATN:
                case MCU_DRTOPPEL:
                case MCU_PLUMPOP:
                    mcu_reset();
                    break;
                default:
                    break;
            }
            //tnzs_input_select = 0;
            //tnzs_bg_flag = 0;
            //basebankmain = 0;// 0x8000;
            //basebanksub = 0;
        }
        public static byte tnzs_bank1_r(int offset)
        {
            return tnzs_bank1[basebankmain + offset];
        }
        public static void tnzs_bank1_w(int offset, byte data)
        {
            tnzs_bank1[basebankmain + offset] = data;
        }
        public static byte tnzs_bank2_r(int offset)
        {
            return tnzs_bank2[basebanksub + offset];
        }
        public static void tnzs_bank2_w(int offset, byte data)
        {
            tnzs_bank2[basebanksub + offset] = data;
        }
        public static byte tnzs_sharedram_r(int offset)
        {
            return tnzs_sharedram[offset];
        }
        public static void tnzs_sharedram_w(int offset, byte data)
        {
            tnzs_sharedram[offset] = data;
        }
        public static void tnzs_bankswitch_w(byte data)
        {
            if ((data & 0x10) != 0)
            {
                Cpuint.cpunum_set_input_line(1, (int)LineState.INPUT_LINE_RESET, LineState.CLEAR_LINE);
            }
            else
            {
                Cpuint.cpunum_set_input_line(1, (int)LineState.INPUT_LINE_RESET, LineState.ASSERT_LINE);
            }
            basebankmain = 0x4000 * (data & 0x07);
        }
        public static void tnzs_bankswitch1_w(byte data)
        {
            switch (mcu_type)
            {
                case MCU_TNZS:
                case MCU_CHUKATAI:
                    if ((data & 0x04) != 0)
                    {
                        if(Cpuexec.cpu[2] == I8x41.m1)
                        {
                            Cpuint.cpunum_set_input_line(2, (int)LineState.INPUT_LINE_RESET, LineState.PULSE_LINE);
                        }
                    }
                    break;
                case MCU_NONE_INSECTX:
                    Generic.coin_lockout_w(0, (~data & 0x04));
                    Generic.coin_lockout_w(1, (~data & 0x08));
                    Generic.coin_counter_w(0, (data & 0x10));
                    Generic.coin_counter_w(1, (data & 0x20));
                    break;
                case MCU_NONE_TNZSB:
                case MCU_NONE_KABUKIZ:
                    Generic.coin_lockout_w(0, (~data & 0x10));
                    Generic.coin_lockout_w(1, (~data & 0x20));
                    Generic.coin_counter_w(0, (data & 0x04));
                    Generic.coin_counter_w(1, (data & 0x08));
                    break;
                case MCU_NONE_KAGEKI:
                    Generic.coin_lockout_global_w((~data & 0x20));
                    Generic.coin_counter_w(0, (data & 0x04));
                    Generic.coin_counter_w(1, (data & 0x08));
                    break;
                case MCU_ARKANOID:
                case MCU_EXTRMATN:
                case MCU_DRTOPPEL:
                case MCU_PLUMPOP:
                    if ((data & 0x04) != 0)
                    {
                        mcu_reset();
                    }
                    break;
                default:
                    break;
            }
            basebanksub = 0x2000 * (data & 3);
        }
        public static void jpopnics_subbankswitch_w(byte data)
        {
            basebanksub = 0x2000 * (data & 3);
        }
        public static void irqhandler_tnzs(int irq)
        {
            Cpuint.cpunum_set_input_line(2, (int)LineState.INPUT_LINE_NMI, irq != 0 ? LineState.ASSERT_LINE : LineState.CLEAR_LINE);
        }
        public static void tnzsb_sound_command_w(byte data)
        {
            Sound.soundlatch_w(data);
            Cpuint.cpunum_set_input_line_and_vector2(2, 0, LineState.HOLD_LINE, 0xff);
        }
        public static void jpopnics_palette_w(int offset, byte data)
        {
            int r, g, b;
            ushort paldata;
            Generic.paletteram[offset] = data;
            offset = offset >> 1;            
            paldata = (ushort)((Generic.paletteram[offset * 2] << 8) | Generic.paletteram[(offset * 2 + 1)]);
            g = (paldata >> 12) & 0x000f;
            r = (paldata >> 4) & 0x000f;
            b = (paldata >> 8) & 0x000f;
            Palette.palette_entry_set_color2(offset, Palette.make_rgb(r << 4, g << 4, b << 4));
        }
        public static void palette_init_arknoid2(byte[] color_prom)
        {
            int i, col;
            for (i = 0; i < 0x200; i++)
            {
                col = (color_prom[i] << 8) + color_prom[i + 512];
                Palette.palette_entry_set_color2(i, Palette.make_rgb(Palette.pal5bit((byte)(col >> 10)), Palette.pal5bit((byte)(col >> 5)), Palette.pal5bit((byte)(col >> 0))));
            }
        }
        public static void draw_background(RECT cliprect, int m_offset)
        {
            int x, y, column, tot, flag;
            int scrollx, scrolly;
            int upperbits;
            int ctrl2 = tnzs_objctrl[1];
            if (((ctrl2 ^ (~ctrl2 << 1)) & 0x40) != 0)
            {
                m_offset += 0x800;
            }
            if ((tnzs_bg_flag & 0x80) != 0)
            {
                flag = 0;
            }
            else
            {
                flag = 1;
            }
            tot = tnzs_objctrl[1] & 0x1f;
            if (tot == 1)
            {
                tot = 16;
            }
            upperbits = tnzs_objctrl[2] + tnzs_objctrl[3] * 256;
            for (column = 0; column < tot; column++)
            {
                scrollx = tnzs_scrollram[column * 16 + 4] - ((upperbits & 0x01) * 256);
                if (tnzs_screenflip != 0)
                {
                    scrolly = tnzs_scrollram[column * 16] + 1 - 256;
                }
                else
                {
                    scrolly = -tnzs_scrollram[column * 16] + 1;
                }
                for (y = 0; y < 16; y++)
                {
                    for (x = 0; x < 2; x++)
                    {
                        int code, color, flipx, flipy, sx, sy;
                        int i = 32 * (column ^ 8) + 2 * y + x;
                        code = tnzs_objram[m_offset + i] + ((tnzs_objram[m_offset + i + 0x1000] & 0x3f) << 8);
                        color = (tnzs_objram[m_offset + i + 0x1200] & 0xf8) >> 3;
                        sx = x * 16;
                        sy = y * 16;
                        flipx = tnzs_objram[m_offset + i + 0x1000] & 0x80;
                        flipy = tnzs_objram[m_offset + i + 0x1000] & 0x40;
                        if (tnzs_screenflip != 0)
                        {
                            sy = 240 - sy;
                            flipx = flipx == 0 ? 1 : 0;
                            flipy = flipy == 0 ? 1 : 0;
                        }
                        /*if (Video.screenstate.frame_number == 0xf0)
                        {
                            StreamWriter sw1 = new StreamWriter(@"\VS2008\compare1\compare1\bin\Debug\2.txt", true);
                            sw1.WriteLine(code.ToString("x") + "\t" + color.ToString("x") + "\t" + flipx.ToString("x") + "\t" + flipy.ToString("x") + "\t" + sx.ToString("x") + "\t" + sy.ToString("x") + "\t" + scrollx.ToString("x") + "\t" + scrolly.ToString("x"));
                            sw1.Close();
                        }*/
                        Drawgfx.common_drawgfx_tnzs(gfx1rom, code, color, flipx, flipy, sx + scrollx, (sy + scrolly) & 0xff, cliprect);
                        Drawgfx.common_drawgfx_tnzs(gfx1rom, code, color, flipx, flipy, sx + 512 + scrollx, (sy + scrolly) & 0xff, cliprect);
                    }
                }
                upperbits >>= 1;
            }
        }
        public static void draw_foreground(RECT cliprect, int char_offset, int x_offset, int y_offset, int ctrl_offset, int color_offset)
        {
            int i;
            int ctrl2 = tnzs_objctrl[1];
            if (((ctrl2 ^ (~ctrl2 << 1)) & 0x40) != 0)
            {
                char_offset += 0x800;
                x_offset += 0x800;
                ctrl_offset += 0x800;
                color_offset += 0x800;
            }
            for (i = 0x1ff; i >= 0; i--)
            {
                int code, color, sx, sy, flipx, flipy;
                code = tnzs_objram[char_offset + i] + ((tnzs_objram[ctrl_offset + i] & 0x3f) << 8);
                color = (tnzs_objram[color_offset + i] & 0xf8) >> 3;
                sx = tnzs_objram[x_offset + i] - ((tnzs_objram[color_offset + i] & 1) << 8);
                sy = 240 - tnzs_vdcram[y_offset + i];
                flipx = tnzs_objram[ctrl_offset + i] & 0x80;
                flipy = tnzs_objram[ctrl_offset + i] & 0x40;
                if (tnzs_screenflip != 0)
                {
                    sy = 240 - sy;
                    flipx = flipx == 0 ? 1 : 0;
                    flipy = flipy == 0 ? 1 : 0;
                    if ((sy == 0) && (code == 0))
                    {
                        sy += 240;
                    }
                }
                Drawgfx.common_drawgfx_tnzs(gfx1rom, code, color, flipx, flipy, sx, sy + 2, cliprect);
                Drawgfx.common_drawgfx_tnzs(gfx1rom, code, color, flipx, flipy, sx + 512, sy + 2, cliprect);
            }
        }
        public static void video_update_tnzs()
        {
            tnzs_screenflip = (tnzs_objctrl[0] & 0x40) >> 6;
            //fillbitmap(bitmap, 0x1f0, cliprect);
            Array.Clear(Video.bitmapbase[Video.curbitmap], 0, 0x10000);
            draw_background(Video.new_clip, 0x400);
            draw_foreground(Video.new_clip, 0x0000, 0x0200, 0x0000, 0x1000, 0x1200);
        }
        public static void video_eof_tnzs()
        {
            int ctrl2 = tnzs_objctrl[1];
            if ((~ctrl2 & 0x20) != 0)
            {
                if ((ctrl2 & 0x40) != 0)
                {
                    Array.Copy(tnzs_objram, 0x8000, tnzs_objram, 0x0000, 0x0400);
                    Array.Copy(tnzs_objram, 0x1800, tnzs_objram, 0x1000, 0x0400);
                }
                else
                {
                    Array.Copy(tnzs_objram, 0x0000, tnzs_objram, 0x0800, 0x0400);
                    Array.Copy(tnzs_objram, 0x1000, tnzs_objram, 0x1800, 0x0400);
                }
                Array.Copy(tnzs_objram, 0x0c00, tnzs_objram, 0x0400, 0x0400);
                Array.Copy(tnzs_objram, 0x1c00, tnzs_objram, 0x1400, 0x0400);
            }
        }
    }
}
