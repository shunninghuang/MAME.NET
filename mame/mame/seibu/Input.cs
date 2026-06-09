using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX.DirectInput;

namespace mame
{
    public partial class Seibu
    {
        public static void loop_inputports_kncljoe()
        {
            if (Keyboard.IsPressed(Key.D5))
            {
                bytes &= unchecked((byte)~0x04);
            }
            else
            {
                bytes |= 0x04;
            }
            if (Keyboard.IsPressed(Key.D6))
            {
                bytes &= unchecked((byte)~0x08);
            }
            else
            {
                bytes |= 0x08;
            }
            if (Keyboard.IsPressed(Key.D1))
            {
                bytes &= unchecked((byte)~0x01);
            }
            else
            {
                bytes |= 0x01;
            }
            if (Keyboard.IsPressed(Key.D2))
            {
                bytes &= unchecked((byte)~0x02);
            }
            else
            {
                bytes |= 0x02;
            }
            if (Keyboard.IsPressed(Key.D))
            {
                byte1 &= unchecked((byte)~0x08);
            }
            else
            {
                byte1 |= 0x08;
            }
            if (Keyboard.IsPressed(Key.A))
            {
                byte1 &= unchecked((byte)~0x04);
            }
            else
            {
                byte1 |= 0x04;
            }
            if (Keyboard.IsPressed(Key.S))
            {
                byte1 &= unchecked((byte)~0x02);
            }
            else
            {
                byte1 |= 0x02;
            }
            if (Keyboard.IsPressed(Key.W))
            {
                byte1 &= unchecked((byte)~0x01);
            }
            else
            {
                byte1 |= 0x01;
            }
            if (Keyboard.IsPressed(Key.J))
            {
                byte1 &= unchecked((byte)~0x10);
            }
            else
            {
                byte1 |= 0x10;
            }
            if (Keyboard.IsPressed(Key.K))
            {
                byte1 &= unchecked((byte)~0x20);
            }
            else
            {
                byte1 |= 0x20;
            }
            if (Keyboard.IsPressed(Key.Right))
            {
                byte2 &= unchecked((byte)~0x08);
            }
            else
            {
                byte2 |= 0x08;
            }
            if (Keyboard.IsPressed(Key.Left))
            {
                byte2 &= unchecked((byte)~0x04);
            }
            else
            {
                byte2 |= 0x04;
            }
            if (Keyboard.IsPressed(Key.Down))
            {
                byte2 &= unchecked((byte)~0x02);
            }
            else
            {
                byte2 |= 0x02;
            }
            if (Keyboard.IsPressed(Key.Up))
            {
                byte2 &= unchecked((byte)~0x01);
            }
            else
            {
                byte2 |= 0x01;
            }
            if (Keyboard.IsPressed(Key.NumPad1))
            {
                byte2 &= unchecked((byte)~0x10);
            }
            else
            {
                byte2 |= 0x10;
            }
            if (Keyboard.IsPressed(Key.NumPad2))
            {
                byte2 &= unchecked((byte)~0x20);
            }
            else
            {
                byte2 |= 0x20;
            }
        }
        public static void record_port_kncljoe()
        {
            if (bytes != bytes_old || byte1 != byte1_old || byte2 != byte2_old)
            {
                bytes_old = bytes;
                byte1_old = byte1;
                byte2_old = byte2;
                Mame.bwRecord.Write(Video.screenstate.frame_number);
                Mame.bwRecord.Write(bytes);
                Mame.bwRecord.Write(byte1);
                Mame.bwRecord.Write(byte2);
            }
        }
        public static void replay_port_kncljoe()
        {
            if (Inptport.bReplayRead)
            {
                try
                {
                    Video.frame_number_obj = Mame.brRecord.ReadInt64();
                    bytes_old = Mame.brRecord.ReadByte();
                    byte1_old = Mame.brRecord.ReadByte();
                    byte2_old = Mame.brRecord.ReadByte();
                }
                catch
                {
                    Mame.playState = Mame.PlayState.PLAY_REPLAYEND;
                }
                Inptport.bReplayRead = false;
            }
            if (Video.screenstate.frame_number == Video.frame_number_obj)
            {
                bytes = bytes_old;
                byte1 = byte1_old;
                byte2 = byte2_old;
                Inptport.bReplayRead = true;
            }
            else
            {
                Inptport.bReplayRead = false;
            }
        }
    }
}
