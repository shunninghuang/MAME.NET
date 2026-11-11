using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX.DirectInput;

namespace mame
{
    public partial class sega
    {
        public static void loop_inputports_system18()
        {
            if (Keyboard.IsPressed(Key.D5))
            {
                sbytes &= ~0x01;
            }
            else
            {
                sbytes |= 0x01;
            }
            if (Keyboard.IsPressed(Key.D6))
            {
                sbytes &= ~0x02;
            }
            else
            {
                sbytes |= 0x02;
            }
            if (Keyboard.IsPressed(Key.D1))
            {
                sbytes &= ~0x10;
            }
            else
            {
                sbytes |= 0x10;
            }
            if (Keyboard.IsPressed(Key.D2))
            {
                sbytes &= ~0x20;
            }
            else
            {
                sbytes |= 0x20;
            }
            if (Keyboard.IsPressed(Key.A))
            {
                sbyte1 &= unchecked((sbyte)~0x80);
            }
            else
            {
                sbyte1 |= unchecked((sbyte)0x80);
            }
            if (Keyboard.IsPressed(Key.D))
            {
                sbyte1 &= ~0x40;
            }
            else
            {
                sbyte1 |= 0x40;
            }
            if (Keyboard.IsPressed(Key.W))
            {
                sbyte1 &= ~0x20;
            }
            else
            {
                sbyte1 |= 0x20;
            }
            if (Keyboard.IsPressed(Key.S))
            {
                sbyte1 &= ~0x10;
            }
            else
            {
                sbyte1 |= 0x10;
            }
            if (Keyboard.IsPressed(Key.J))
            {
                sbyte1 &= ~0x01;
            }
            else
            {
                sbyte1 |= 0x01;
            }
            if (Keyboard.IsPressed(Key.K))
            {
                sbyte1 &= ~0x02;
            }
            else
            {
                sbyte1 |= 0x02;
            }
            if (Keyboard.IsPressed(Key.L))
            {
                sbyte1 &= ~0x04;
            }
            else
            {
                sbyte1 |= 0x04;
            }
            if (Keyboard.IsPressed(Key.Left))
            {
                sbyte2 &= unchecked((sbyte)~0x80);
            }
            else
            {
                sbyte2 |= unchecked((sbyte)0x80);
            }
            if (Keyboard.IsPressed(Key.Right))
            {
                sbyte2 &= ~0x40;
            }
            else
            {
                sbyte2 |= 0x40;
            }
            if (Keyboard.IsPressed(Key.Up))
            {
                sbyte2 &= ~0x20;
            }
            else
            {
                sbyte2 |= 0x20;
            }
            if (Keyboard.IsPressed(Key.Down))
            {
                sbyte2 &= ~0x10;
            }
            else
            {
                sbyte2 |= 0x10;
            }
            if (Keyboard.IsPressed(Key.NumPad1))
            {
                sbyte2 &= ~0x01;
            }
            else
            {
                sbyte2 |= 0x01;
            }
            if (Keyboard.IsPressed(Key.NumPad2))
            {
                sbyte2 &= ~0x02;
            }
            else
            {
                sbyte2 |= 0x02;
            }
            if (Keyboard.IsPressed(Key.NumPad3))
            {
                sbyte2 &= ~0x04;
            }
            else
            {
                sbyte2 |= 0x04;
            }
            if (Keyboard.IsPressed(Key.R))
            {
                sbytes &= ~0x08;
            }
            else
            {
                sbytes |= 0x08;
            }
            if (Keyboard.IsPressed(Key.T))
            {
                sbytes &= ~0x40;
            }
            else
            {
                sbytes |= 0x40;
            }
        }
        public static void loop_inputports_astorm()
        {
            if (Keyboard.IsPressed(Key.D5))
            {
                sbytes &= unchecked((sbyte)~0x80);
            }
            else
            {
                sbytes |= unchecked((sbyte)0x80);
            }
            if (Keyboard.IsPressed(Key.D6))
            {
                sbytes &= ~0x02;
            }
            else
            {
                sbytes |= 0x02;
            }
            if (Keyboard.IsPressed(Key.D1))
            {
                sbytes &= ~0x10;
            }
            else
            {
                sbytes |= 0x10;
            }
            if (Keyboard.IsPressed(Key.D2))
            {
                sbytes &= ~0x20;
            }
            else
            {
                sbytes |= 0x20;
            }
            if (Keyboard.IsPressed(Key.A))
            {
                sbyte1 &= unchecked((sbyte)~0x80);
            }
            else
            {
                sbyte1 |= unchecked((sbyte)0x80);
            }
            if (Keyboard.IsPressed(Key.D))
            {
                sbyte1 &= ~0x40;
            }
            else
            {
                sbyte1 |= 0x40;
            }
            if (Keyboard.IsPressed(Key.W))
            {
                sbyte1 &= ~0x20;
            }
            else
            {
                sbyte1 |= 0x20;
            }
            if (Keyboard.IsPressed(Key.S))
            {
                sbyte1 &= ~0x10;
            }
            else
            {
                sbyte1 |= 0x10;
            }
            if (Keyboard.IsPressed(Key.J))
            {
                sbyte1 &= ~0x01;
            }
            else
            {
                sbyte1 |= 0x01;
            }
            if (Keyboard.IsPressed(Key.K))
            {
                sbyte1 &= ~0x02;
            }
            else
            {
                sbyte1 |= 0x02;
            }
            if (Keyboard.IsPressed(Key.L))
            {
                sbyte1 &= ~0x04;
            }
            else
            {
                sbyte1 |= 0x04;
            }
            if (Keyboard.IsPressed(Key.Left))
            {
                sbyte2 &= unchecked((sbyte)~0x80);
            }
            else
            {
                sbyte2 |= unchecked((sbyte)0x80);
            }
            if (Keyboard.IsPressed(Key.Right))
            {
                sbyte2 &= ~0x40;
            }
            else
            {
                sbyte2 |= 0x40;
            }
            if (Keyboard.IsPressed(Key.Up))
            {
                sbyte2 &= ~0x20;
            }
            else
            {
                sbyte2 |= 0x20;
            }
            if (Keyboard.IsPressed(Key.Down))
            {
                sbyte2 &= ~0x10;
            }
            else
            {
                sbyte2 |= 0x10;
            }
            if (Keyboard.IsPressed(Key.NumPad1))
            {
                sbyte2 &= ~0x01;
            }
            else
            {
                sbyte2 |= 0x01;
            }
            if (Keyboard.IsPressed(Key.NumPad2))
            {
                sbyte2 &= ~0x02;
            }
            else
            {
                sbyte2 |= 0x02;
            }
            if (Keyboard.IsPressed(Key.NumPad3))
            {
                sbyte2 &= ~0x04;
            }
            else
            {
                sbyte2 |= 0x04;
            }
            if (Keyboard.IsPressed(Key.R))
            {
                sbytes &= ~0x08;
            }
            else
            {
                sbytes |= 0x08;
            }
        }
        public static void loop_inputports_lghost()
        {
            if (Keyboard.IsPressed(Key.D5))
            {
                sbytes &= ~0x01;
            }
            else
            {
                sbytes |= 0x01;
            }
            if (Keyboard.IsPressed(Key.D6))
            {
                sbytes &= ~0x02;
            }
            else
            {
                sbytes |= 0x02;
            }
            if (Keyboard.IsPressed(Key.D1))
            {
                sbytes &= ~0x10;
            }
            else
            {
                sbytes |= 0x10;
            }
            if (Keyboard.IsPressed(Key.D2))
            {
                sbytes &= ~0x20;
            }
            else
            {
                sbytes |= 0x20;
            }
            if (Keyboard.IsPressed(Key.J))
            {
                sbyte1 &= ~0x01;
            }
            else
            {
                sbyte1 |= 0x01;
            }
            if (Keyboard.IsPressed(Key.K))
            {
                sbyte1 &= ~0x02;
            }
            else
            {
                sbyte1 |= 0x02;
            }
            if (Keyboard.IsPressed(Key.NumPad1))
            {
                sbyte1 &= ~0x04;
            }
            else
            {
                sbyte1 |= 0x04;
            }
            if (Keyboard.IsPressed(Key.NumPad2))
            {
                sbyte1 &= ~0x08;
            }
            else
            {
                sbyte1 |= 0x08;
            }
            if (Keyboard.IsPressed(Key.R))
            {
                sbytes &= ~0x10;
            }
            else
            {
                sbytes |= 0x10;
            }
            if (Keyboard.IsPressed(Key.T))
            {
                sbytes &= ~0x20;
            }
            else
            {
                sbytes |= 0x20;
            }
        }
        public static void loop_inputports_wwallyj()
        {
            if (Keyboard.IsPressed(Key.D5))
            {
                sbytes &= ~0x01;
            }
            else
            {
                sbytes |= 0x01;
            }
            if (Keyboard.IsPressed(Key.D6))
            {
                sbytes &= ~0x02;
            }
            else
            {
                sbytes |= 0x02;
            }
            if (Keyboard.IsPressed(Key.D1))
            {
                sbytes &= ~0x10;
            }
            else
            {
                sbytes |= 0x10;
            }
            if (Keyboard.IsPressed(Key.D2))
            {
                sbytes &= ~0x20;
            }
            else
            {
                sbytes |= 0x20;
            }
            frame_update_analog_field_wwallyj_p0(Inptport.analog_p0);
            frame_update_analog_field_wwallyj_p1(Inptport.analog_p1);
        }
        public static void frame_update_analog_field_wwallyj_p0(analog_field_state analog)
        {
            bool keypressed = false;
            int delta = 0;
            int value2;
            value2 = Inptport.apply_analog_min_max(analog, analog.accum);
            analog.previous = analog.accum = value2;
            if (Keyboard.IsPressed(Key.A))
            {
                keypressed = true;
                delta -= analog.delta * 0x200;
                analog.lastdigital = 1;
            }
            if (Keyboard.IsPressed(Key.D))
            {
                keypressed = true;
                delta += analog.delta * 0x200;
                analog.lastdigital = 2;
            }
            analog.accum += delta;
            if (!keypressed)
            {
                analog.lastdigital = 0;
            }
        }
        public static void frame_update_analog_field_wwallyj_p1(analog_field_state analog)
        {
            bool keypressed = false;
            int delta = 0;
            int value2;
            value2 = Inptport.apply_analog_min_max(analog, analog.accum);
            analog.previous = analog.accum = value2;
            if (Keyboard.IsPressed(Key.S))
            {
                keypressed = true;
                delta -= analog.delta * 0x200;
                analog.lastdigital = 1;
            }
            if (Keyboard.IsPressed(Key.W))
            {
                keypressed = true;
                delta += analog.delta * 0x200;
                analog.lastdigital = 2;
            }
            analog.accum += delta;
            if (!keypressed)
            {
                analog.lastdigital = 0;
            }
        }
        public static void record_port()
        {
            if (sbyte1 != sbyte1_old || sbyte2 != sbyte2_old || sbyte3 != sbyte3_old || sbyte4 != sbyte4_old || sbytes != sbytes_old)
            {
                sbyte1_old = sbyte1;
                sbyte2_old = sbyte2;
                sbyte3_old = sbyte3;
                sbyte4_old = sbyte4;
                sbytes_old = sbytes;
                Mame.bwRecord.Write(Video.screenstate.frame_number);
                Mame.bwRecord.Write(sbyte1);
                Mame.bwRecord.Write(sbyte2);
                Mame.bwRecord.Write(sbyte3);
                Mame.bwRecord.Write(sbyte4);
                Mame.bwRecord.Write(sbytes);
            }
        }
        public static void replay_port()
        {
            if (Inptport.bReplayRead)
            {
                try
                {
                    Video.frame_number_obj = Mame.brRecord.ReadInt64();
                    sbyte1_old = Mame.brRecord.ReadSByte();
                    sbyte2_old = Mame.brRecord.ReadSByte();
                    sbyte3_old = Mame.brRecord.ReadSByte();
                    sbyte4_old = Mame.brRecord.ReadSByte();
                    sbytes_old = Mame.brRecord.ReadSByte();
                }
                catch
                {
                    Mame.playState = Mame.PlayState.PLAY_REPLAYEND;
                }
                Inptport.bReplayRead = false;
            }
            if (Video.screenstate.frame_number == Video.frame_number_obj)
            {
                sbyte1 = sbyte1_old;
                sbyte2 = sbyte2_old;
                sbyte3 = sbyte3_old;
                sbyte4 = sbyte4_old;
                sbytes = sbytes_old;
                Inptport.bReplayRead = true;
            }
            else
            {
                Inptport.bReplayRead = false;
            }
        }
    }
}
