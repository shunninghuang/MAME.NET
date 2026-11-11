using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace mame
{
    public class rf5c68
    {
        public struct pcm_channel
        {
            public byte enable;
            public byte env;
            public byte pan;
            public byte start;
            public uint addr;
            public ushort step;
            public ushort loopst;
        }
        public struct rf5c68pcm
        {
            public sound_stream stream;
            public pcm_channel[] chan;
            public byte cbank;
            public byte wbank;
            public byte enable;
            public byte[] data;
        }
        public static rf5c68pcm r1;
        public static void rf5c68_update(int offset, int length)
        {
            int i, j;
            for (i = 0; i < length; i++)
            {
                r1.stream.streamoutput[0][offset + i] = 0;
                r1.stream.streamoutput[1][offset + i] = 0;
            }
            if (r1.enable == 0)
            {
                return;
            }
            for (i = 0; i < 8; i++)
            {
                if (r1.chan[i].enable != 0)
                {
                    int lv = (r1.chan[i].pan & 0x0f) * r1.chan[i].env;
                    int rv = ((r1.chan[i].pan >> 4) & 0x0f) * r1.chan[i].env;
                    for (j = 0; j < length; j++)
                    {
                        int sample;
                        sample = r1.data[(r1.chan[i].addr >> 11) & 0xffff];
                        if (sample == 0xff)
                        {
                            r1.chan[i].addr = (uint)(r1.chan[i].loopst << 11);
                            sample = r1.data[(r1.chan[i].addr >> 11) & 0xffff];
                            if (sample == 0xff)
                            {
                                break;
                            }
                        }
                        r1.chan[i].addr += r1.chan[i].step;
                        if ((sample & 0x80) != 0)
                        {
                            sample &= 0x7f;
                            r1.stream.streamoutput[0][offset + j] += (sample * lv) >> 5;
                            r1.stream.streamoutput[1][offset + j] += (sample * rv) >> 5;
                        }
                        else
                        {
                            r1.stream.streamoutput[0][offset + j] -= (sample * lv) >> 5;
                            r1.stream.streamoutput[1][offset + j] -= (sample * rv) >> 5;
                        }
                    }
                }
            }
            for (j = 0; j < length; j++)
            {
                int temp;
                temp = r1.stream.streamoutput[0][offset + j];
                if (temp > 32767)
                {
                    temp = 32767;
                }
                else if (temp < -32768)
                {
                    temp = -32768;
                }
                r1.stream.streamoutput[0][offset + j] = temp & ~0x3f;
                temp = r1.stream.streamoutput[1][offset + j];
                if (temp > 32767)
                {
                    temp = 32767;
                }
                else if (temp < -32768)
                {
                    temp = -32768;
                }
                r1.stream.streamoutput[1][offset + j] = temp & ~0x3f;
            }
        }
        public static void rf5c68_start(int clock)
        {
            int i;
            r1 = new rf5c68pcm();
            r1.chan = new pcm_channel[8];
            for (i = 0; i < 8; i++)
            {
                r1.chan[i].enable = 0;
                r1.chan[i].env = 0;
                r1.chan[i].pan = 0;
                r1.chan[i].start = 0;
                r1.chan[i].addr = 0;
                r1.chan[i].step = 0;
                r1.chan[i].loopst = 0;
            }
            r1.cbank = 0;
            r1.wbank = 0;
            r1.enable = 0;
            r1.data = new byte[0x10000];
            for (i = 0; i < 0x10000; i++)
            {
                r1.data[i] = 0xff;
            }
            r1.stream = new sound_stream(clock / 384, 0, 2, rf5c68_update);
        }
        public static void rf5c68_reg_w(int offset, byte data)
        {
            int i;
            r1.stream.stream_update();
            switch (offset)
            {
                case 0x00:
                    r1.chan[r1.cbank].env = data;
                    break;
                case 0x01:
                    r1.chan[r1.cbank].pan = data;
                    break;
                case 0x02:
                    r1.chan[r1.cbank].step = (ushort)((r1.chan[r1.cbank].step & 0xff00) | (data & 0x00ff));
                    break;
                case 0x03:
                    r1.chan[r1.cbank].step = (ushort)((r1.chan[r1.cbank].step & 0x00ff) | ((data << 8) & 0xff00));
                    break;
                case 0x04:
                    r1.chan[r1.cbank].loopst = (ushort)((r1.chan[r1.cbank].loopst & 0xff00) | (data & 0x00ff));
                    break;
                case 0x05:
                    r1.chan[r1.cbank].loopst = (ushort)((r1.chan[r1.cbank].loopst & 0x00ff) | ((data << 8) & 0xff00));
                    break;
                case 0x06:
                    r1.chan[r1.cbank].start = data;
                    if (r1.chan[r1.cbank].enable == 0)
                    {
                        r1.chan[r1.cbank].addr = (uint)(r1.chan[r1.cbank].start << (8 + 11));
                    }
                    break;
                case 0x07:
                    r1.enable = (byte)((data >> 7) & 1);
                    if ((data & 0x40) != 0)
                    {
                        r1.cbank = (byte)(data & 7);
                    }
                    else
                    {
                        r1.wbank = (byte)(data & 15);
                    }
                    break;
                case 0x08:
                    for (i = 0; i < 8; i++)
                    {
                        r1.chan[i].enable = (byte)((~data >> i) & 1);
                        if (r1.chan[i].enable == 0)
                        {
                            r1.chan[i].addr = (uint)(r1.chan[i].start << (8 + 11));
                        }
                    }
                    break;
            }
        }
        public static byte rf5c68_r(int offset)
        {
            return r1.data[r1.wbank * 0x1000 + offset];
        }
        public static void rf5c68_w(int offset, byte data)
        {
            r1.data[r1.wbank * 0x1000 + offset] = data;
        }
        public static void SaveStateBinary(BinaryWriter writer)
        {
            int i;
            for (i = 0; i < 8; i++)
            {
                writer.Write(r1.chan[i].enable);
                writer.Write(r1.chan[i].env);
                writer.Write(r1.chan[i].pan);
                writer.Write(r1.chan[i].start);
                writer.Write(r1.chan[i].addr);
                writer.Write(r1.chan[i].step);
                writer.Write(r1.chan[i].loopst);
            }
            writer.Write(r1.cbank);
            writer.Write(r1.wbank);
            writer.Write(r1.enable);
            writer.Write(r1.data, 0, 0x10000);
        }
        public static void LoadStateBinary(BinaryReader reader)
        {
            int i;
            for (i = 0; i < 8; i++)
            {
                r1.chan[i].enable = reader.ReadByte();
                r1.chan[i].env = reader.ReadByte();
                r1.chan[i].pan = reader.ReadByte();
                r1.chan[i].start = reader.ReadByte();
                r1.chan[i].addr = reader.ReadUInt32();
                r1.chan[i].step = reader.ReadUInt16();
                r1.chan[i].loopst = reader.ReadUInt16();
            }
            r1.cbank = reader.ReadByte();
            r1.wbank = reader.ReadByte();
            r1.enable = reader.ReadByte();
            r1.data = reader.ReadBytes(0x10000);
        }
    }
}
