using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mame
{
    public partial class Gaelco
    {
        public static int lastpc, lastoffset, lastencword, lastdecword;
        public static int decrypt(int param1, int param2, int enc_prev_word, int dec_prev_word, int enc_word)
        {
            int swap = (Generic.BIT(dec_prev_word, 8) << 1) | Generic.BIT(dec_prev_word, 7);
            int type = (Generic.BIT(dec_prev_word, 12) << 1) | Generic.BIT(dec_prev_word, 2);
            int res = 0;
            int k = 0;
            switch (swap)
            {
                case 0: res = Generic.BITSWAP16(enc_word, 1, 2, 0, 14, 12, 15, 4, 8, 13, 7, 3, 6, 11, 5, 10, 9); break;
                case 1: res = Generic.BITSWAP16(enc_word, 14, 10, 4, 15, 1, 6, 12, 11, 8, 0, 9, 13, 7, 3, 5, 2); break;
                case 2: res = Generic.BITSWAP16(enc_word, 2, 13, 15, 1, 12, 8, 14, 4, 6, 0, 9, 5, 10, 7, 3, 11); break;
                case 3: res = Generic.BITSWAP16(enc_word, 3, 8, 1, 13, 14, 4, 15, 0, 10, 2, 7, 12, 6, 11, 9, 5); break;
            }
            res ^= param2;
            switch (type)
            {
                case 0:
                    k = (0 << 0) |
                        (1 << 1) |
                        (0 << 2) |
                        (1 << 3) |
                        (1 << 4) |
                        (1 << 5);
                    break;
                case 1:
                    k = (Generic.BIT(dec_prev_word, 0) << 0) |
                        (Generic.BIT(dec_prev_word, 1) << 1) |
                        (Generic.BIT(dec_prev_word, 1) << 2) |
                        (Generic.BIT(enc_prev_word, 3) << 3) |
                        (Generic.BIT(enc_prev_word, 8) << 4) |
                        (Generic.BIT(enc_prev_word, 15) << 5);
                    break;
                case 2:
                    k = (Generic.BIT(enc_prev_word, 5) << 0) |
                        (Generic.BIT(dec_prev_word, 5) << 1) |
                        (Generic.BIT(enc_prev_word, 7) << 2) |
                        (Generic.BIT(enc_prev_word, 3) << 3) |
                        (Generic.BIT(enc_prev_word, 13) << 4) |
                        (Generic.BIT(enc_prev_word, 14) << 5);
                    break;
                case 3:
                    k = (Generic.BIT(enc_prev_word, 0) << 0) |
                        (Generic.BIT(enc_prev_word, 9) << 1) |
                        (Generic.BIT(enc_prev_word, 6) << 2) |
                        (Generic.BIT(dec_prev_word, 4) << 3) |
                        (Generic.BIT(enc_prev_word, 2) << 4) |
                        (Generic.BIT(dec_prev_word, 11) << 5);
                    break;
            }
            k ^= param1;
            res = (res & 0xffc0) | ((res + k) & 0x003f);
            res ^= param1;
            switch (type)
            {
                case 0:
                    k = (Generic.BIT(enc_word, 9) << 0) |
                        (Generic.BIT(res, 2) << 1) |
                        (Generic.BIT(enc_word, 5) << 2) |
                        (Generic.BIT(res, 5) << 3) |
                        (Generic.BIT(res, 4) << 4);
                    break;
                case 1:
                    k = (Generic.BIT(dec_prev_word, 2) << 0) |	// always 1
                        (Generic.BIT(enc_prev_word, 4) << 1) |
                        (Generic.BIT(dec_prev_word, 14) << 2) |
                        (Generic.BIT(res, 1) << 3) |
                        (Generic.BIT(dec_prev_word, 12) << 4);	// always 0
                    break;
                case 2:
                    k = (Generic.BIT(enc_prev_word, 6) << 0) |
                        (Generic.BIT(dec_prev_word, 6) << 1) |
                        (Generic.BIT(dec_prev_word, 15) << 2) |
                        (Generic.BIT(res, 0) << 3) |
                        (Generic.BIT(dec_prev_word, 7) << 4);
                    break;
                case 3:
                    k = (Generic.BIT(dec_prev_word, 2) << 0) |	// always 1
                        (Generic.BIT(dec_prev_word, 9) << 1) |
                        (Generic.BIT(enc_prev_word, 5) << 2) |
                        (Generic.BIT(dec_prev_word, 1) << 3) |
                        (Generic.BIT(enc_prev_word, 10) << 4);
                    break;
            }
            k ^= param1;
            res = (res & 0x003f) |
                    ((res + (k << 6)) & 0x07c0) |
                    ((res + (k << 11)) & 0xf800);
            res ^= (param1 << 6) | (param1 << 11);
            return Generic.BITSWAP16(res, 2, 6, 0, 11, 14, 12, 7, 10, 5, 4, 8, 3, 9, 1, 13, 15);
        }
        public static ushort gaelco_decrypt(int offset, int data, int param1, int param2)
        {
            ushort data2;
            int thispc = Cpuexec.activecpu;
            if (lastpc == thispc && offset == lastoffset + 1)
            {
                lastpc = 0;
                data2 = (ushort)decrypt(param1, param2, lastencword, lastdecword, data);
            }
            else
            {
                lastpc = thispc;
                lastoffset = offset;
                lastencword = data;
                data2 = (ushort)decrypt(param1, param2, 0, 0, data);
                lastdecword = data2;
            }
            return data2;
        }
        
    }
}
