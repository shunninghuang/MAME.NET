using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace mame
{
    public class SN76496
    {
        private const int MAX_OUTPUT = 0x7fff;
        private const int STEP = 0x10000;
        public sound_stream Channel;
        private int SampleRate;
        private int[] VolTable = new int[16];
        private int[] Register = new int[8];
        private int LastRegister;
        private int[] Volume = new int[4];
        private uint RNG;
        private int NoiseMode;
        private int FeedbackMask;
        private int WhitenoiseTaps;
        private int WhitenoiseInvert;
        private int[] Period = new int[4];
        private int[] Count = new int[4];
        private int[] Output = new int[4];
        public static SN76496[] ss1 = new SN76496[5];
        public SN76496(int clock, int feedbackMask, int noiseTaps, bool noiseInvert)
        {
            int i;
            SampleRate = clock / 16;
            Channel = new sound_stream(SampleRate, 0, 1, SN76496Update);
            SetGain(0);
            for (i = 0; i < 4; i++)
            {
                Volume[i] = 0;
            }
            LastRegister = 0;
            for (i = 0; i < 8; i += 2)
            {
                Register[i] = 0;
                Register[i + 1] = 0x0f;
            }
            for (i = 0; i < 4; i++)
            {
                Output[i] = 0;
                Period[i] = STEP;
                Count[i] = STEP;
            }
            FeedbackMask = feedbackMask;
            WhitenoiseTaps = noiseTaps;
            WhitenoiseInvert = noiseInvert ? 1 : 0;
            RNG = (uint)FeedbackMask;
            Output[3] = (int)(RNG & 1);
        }
        public void Write(int data)
        {
            int n, r, c;
            if ((data & 0x80) != 0)
            {
                r = (data & 0x70) >> 4;
                LastRegister = r;
                Register[r] = (Register[r] & 0x3f0) | (data & 0x0f);
            }
            else
            {
                r = LastRegister;
            }
            c = r / 2;
            switch (r)
            {
                case 0:
                case 2:
                case 4:
                    if ((data & 0x80) == 0)
                    {
                        Register[r] = (Register[r] & 0x0f) | ((data & 0x3f) << 4);
                    }
                    Period[c] = STEP * Register[r];
                    if (Period[c] == 0)
                    {
                        Period[c] = STEP;
                    }
                    if (r == 4)
                    {
                        if ((Register[6] & 0x03) == 0x03)
                        {
                            Period[3] = 2 * Period[2];
                        }
                    }
                    break;
                case 1:
                case 3:
                case 5:
                case 7:
                    Volume[c] = VolTable[data & 0x0f];
                    if ((data & 0x80) == 0)
                    {
                        Register[r] = (Register[r] & 0x3f0) | (data & 0x0f);
                    }
                    break;
                case 6:
                    if ((data & 0x80) == 0)
                    {
                        Register[r] = (Register[r] & 0x3f0) | (data & 0x0f);
                    }
                    n = Register[6];
                    NoiseMode = ((n & 4) != 0) ? 1 : 0;
                    Period[3] = ((n & 3) == 3) ? (2 * Period[2]) : (STEP << (5 + (n & 3)));
                    RNG = (uint)FeedbackMask;
                    Output[3] = (int)(RNG & 1);
                    break;
            }
        }
        public void SN76496Update(int offset, int length)
        {
            int i,j;
            int[] vol = new int[4];
            int left, nextevent;
            uint outValue;
            for (i = 0; i < 4; i++)
            {
                if (Volume[i] == 0)
                {
                    if (Count[i] <= length * STEP)
                    {
                        Count[i] += length * STEP;
                    }
                }
            }
            for(j=0;j<length;j++)
            {
                vol[0] = vol[1] = vol[2] = vol[3] = 0;
                for (i = 0; i < 3; i++)
                {
                    if (Output[i] != 0)
                    {
                        vol[i] += Count[i];
                    }
                    Count[i] -= STEP;
                    while (Count[i] <= 0)
                    {
                        Count[i] += Period[i];
                        if (Count[i] > 0)
                        {
                            Output[i] ^= 1;
                            if (Output[i] != 0)
                            {
                                vol[i] += Period[i];
                            }
                            break;
                        }
                        Count[i] += Period[i];
                        vol[i] += Period[i];
                    }
                    if (Output[i] != 0)
                    {
                        vol[i] -= Count[i];
                    }
                }
                left = STEP;
                do
                {
                    nextevent = Math.Min(Count[3], left);
                    if (Output[3] != 0)
                    {
                        vol[3] += Count[3];
                    }
                    Count[3] -= nextevent;
                    if (Count[3] <= 0)
                    {
                        if (NoiseMode == 1)
                        {
                            if (((RNG & WhitenoiseTaps) != WhitenoiseTaps) && ((RNG & WhitenoiseTaps) != 0))
                            {
                                RNG >>= 1;
                                RNG |= (uint)FeedbackMask;
                            }
                            else
                            {
                                RNG >>= 1;
                            }
                            Output[3] = (WhitenoiseInvert != 0) ? (int)((~RNG) & 1) : (int)(RNG & 1);
                        }
                        else
                        {
                            if ((RNG & 1) != 0)
                            {
                                RNG >>= 1;
                                RNG |= (uint)FeedbackMask;
                            }
                            else
                            {
                                RNG >>= 1;
                            }
                            Output[3] = (int)(RNG & 1);
                        }
                        Count[3] += Period[3];
                        if (Output[3] != 0)
                        {
                            vol[3] += Period[3];
                        }
                    }
                    if (Output[3] != 0)
                    {
                        vol[3] -= Count[3];
                    }
                    left -= nextevent;
                }while (left > 0);
                outValue = (uint)(vol[0] * Volume[0] + vol[1] * Volume[1] + vol[2] * Volume[2] + vol[3] * Volume[3]);
                if (outValue > MAX_OUTPUT * STEP)
                {
                    outValue = MAX_OUTPUT * STEP;
                }
                Channel.streamoutput[0][offset + j] = (int)(outValue / STEP);
            }
        }
        private void SetGain(int gain)
        {
            double outVal;
            gain &= 0xff;
            outVal = MAX_OUTPUT / 3;
            int i;
            for (i = 0; i < gain; i++)
            {
                outVal *= 1.023292992;
            }
            for (i = 0; i < 15; i++)
            {
                if (outVal > MAX_OUTPUT / 3)
                {
                    VolTable[i] = MAX_OUTPUT / 3;
                }
                else
                {
                    VolTable[i] = (int)outVal;
                }
                outVal /= 1.258925412;
            }
            VolTable[15] = 0;
        }
        public void SaveStateBinary(BinaryWriter writer)
        {
            int i;
            for (i = 0; i < 8; i++)
            {
                writer.Write(Register[i]);
            }
            writer.Write(LastRegister);
            for (i = 0; i < 4; i++)
            {
                writer.Write(Volume[i]);
            }
            writer.Write(RNG);
            writer.Write(NoiseMode);
            for (i = 0; i < 4; i++)
            {
                writer.Write(Period[i]);
            }
            for (i = 0; i < 4; i++)
            {
                writer.Write(Count[i]);
            }
            for (i = 0; i < 4; i++)
            {
                writer.Write(Output[i]);
            }
        }
        public void LoadStateBinary(BinaryReader reader)
        {
            int i;
            for (i = 0; i < 8; i++)
            {
                Register[i] = reader.ReadInt32();
            }
            LastRegister = reader.ReadInt32();
            for (i = 0; i < 4; i++)
            {
                Volume[i] = reader.ReadInt32();
            }
            RNG = reader.ReadUInt32();
            NoiseMode = reader.ReadInt32();
            for (i = 0; i < 4; i++)
            {
                Period[i] = reader.ReadInt32();
            }
            for (i = 0; i < 4; i++)
            {
                Count[i] = reader.ReadInt32();
            }
            for (i = 0; i < 4; i++)
            {
                Output[i] = reader.ReadInt32();
            }
        }
    }
}
