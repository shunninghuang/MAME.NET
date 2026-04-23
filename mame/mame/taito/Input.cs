using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX.DirectInput;

namespace mame
{
    public partial class Taito
    {
        public static void loop_inputports_taito_common()
        {

        }
        public static void loop_inputports_taito_bublbobl()
        {
            if (Keyboard.IsPressed(Key.D5))
            {
                sbyte0 |= 0x04;
            }
            else
            {
                sbyte0 &= ~0x04;
            }
            if (Keyboard.IsPressed(Key.D6))
            {
                sbyte0 |= 0x08;
            }
            else
            {
                sbyte0 &= ~0x08;
            }
            if (Keyboard.IsPressed(Key.D1))
            {
                sbyte1 &= ~0x40;
            }
            else
            {
                sbyte1 |= 0x40;
            }
            if (Keyboard.IsPressed(Key.D2))
            {
                sbyte2 &= ~0x40;
            }
            else
            {
                sbyte2 |= 0x40;
            }
            if (Keyboard.IsPressed(Key.D))
            {
                sbyte1 &= ~0x02;
            }
            else
            {
                sbyte1 |= 0x02;
            }
            if (Keyboard.IsPressed(Key.A))
            {
                sbyte1 &= ~0x01;
            }
            else
            {
                sbyte1 |= 0x01;
            }
            /*if (Keyboard.IsPressed(Key.S))
            {
                sbyte2 &= ~0x02;
            }
            else
            {
                sbyte2 |= 0x02;
            }
            if (Keyboard.IsPressed(Key.W))
            {
                sbyte2 &= ~0x01;
            }
            else
            {
                sbyte2 |= 0x01;
            }*/
            if (Keyboard.IsPressed(Key.J))
            {
                sbyte1 &= ~0x20;
            }
            else
            {
                sbyte1 |= 0x20;
            }
            if (Keyboard.IsPressed(Key.K))
            {
                sbyte1 &= ~0x10;
            }
            else
            {
                sbyte1 |= 0x10;
            }
            /*if (Keyboard.IsPressed(Key.L))
            {
                sbyte1 &= ~0x04;
            }
            else
            {
                sbyte1 |= 0x04;
            }*/
            if (Keyboard.IsPressed(Key.Right))
            {
                sbyte2 &= ~0x02;
            }
            else
            {
                sbyte2 |= 0x02;
            }
            if (Keyboard.IsPressed(Key.Left))
            {
                sbyte2 &= ~0x01;
            }
            else
            {
                sbyte2 |= 0x01;
            }
            /*if (Keyboard.IsPressed(Key.Down))
            {
                sbyte2 &= ~0x20;
            }
            else
            {
                sbyte2 |= 0x20;
            }
            if (Keyboard.IsPressed(Key.Up))
            {
                sbyte2 &= ~0x10;
            }
            else
            {
                sbyte2 |= 0x10;
            }*/
            if (Keyboard.IsPressed(Key.NumPad1))
            {
                sbyte2 &= ~0x20;
            }
            else
            {
                sbyte2 |= 0x20;
            }
            if (Keyboard.IsPressed(Key.NumPad2))
            {
                sbyte2 &= ~0x10;
            }
            else
            {
                sbyte2 |= 0x10;
            }
            /*if (Keyboard.IsPressed(Key.NumPad3))
            {
                sbyte1 &= ~0x40;
            }
            else
            {
                sbyte1 |= 0x40;
            }*/
            if (Keyboard.IsPressed(Key.R))
            {
                sbyte0 &= ~0x02;
            }
            else
            {
                sbyte0 |= 0x02;
            }
            /*if (Keyboard.IsPressed(Key.T))
            {
                sbyte0 &= ~0x02;
            }
            else
            {
                sbyte0 |= 0x02;
            }*/
        }
        public static void loop_inputports_taito_tokio()
        {
            if (Keyboard.IsPressed(Key.D5))
            {
                sbyte0 |= 0x04;
            }
            else
            {
                sbyte0 &= ~0x04;
            }
            if (Keyboard.IsPressed(Key.D6))
            {
                sbyte0 |= 0x08;
            }
            else
            {
                sbyte0 &= ~0x08;
            }
            if (Keyboard.IsPressed(Key.D1))
            {
                sbyte1 &= ~0x40;
            }
            else
            {
                sbyte1 |= 0x40;
            }
            if (Keyboard.IsPressed(Key.D2))
            {
                sbyte2 &= ~0x40;
            }
            else
            {
                sbyte2 |= 0x40;
            }
            if (Keyboard.IsPressed(Key.D))
            {
                sbyte1 &= ~0x02;
            }
            else
            {
                sbyte1 |= 0x02;
            }
            if (Keyboard.IsPressed(Key.A))
            {
                sbyte1 &= ~0x01;
            }
            else
            {
                sbyte1 |= 0x01;
            }
            if (Keyboard.IsPressed(Key.S))
            {
                sbyte1 &= ~0x04;
            }
            else
            {
                sbyte1 |= 0x04;
            }
            if (Keyboard.IsPressed(Key.W))
            {
                sbyte1 &= ~0x08;
            }
            else
            {
                sbyte1 |= 0x08;
            }
            if (Keyboard.IsPressed(Key.J))
            {
                sbyte1 &= ~0x20;
            }
            else
            {
                sbyte1 |= 0x20;
            }
            if (Keyboard.IsPressed(Key.K))
            {
                sbyte1 &= ~0x10;
            }
            else
            {
                sbyte1 |= 0x10;
            }
            /*if (Keyboard.IsPressed(Key.L))
            {
                sbyte1 &= ~0x04;
            }
            else
            {
                sbyte1 |= 0x04;
            }*/
            if (Keyboard.IsPressed(Key.Right))
            {
                sbyte2 &= ~0x02;
            }
            else
            {
                sbyte2 |= 0x02;
            }
            if (Keyboard.IsPressed(Key.Left))
            {
                sbyte2 &= ~0x01;
            }
            else
            {
                sbyte2 |= 0x01;
            }
            if (Keyboard.IsPressed(Key.Down))
            {
                sbyte2 &= ~0x04;
            }
            else
            {
                sbyte2 |= 0x04;
            }
            if (Keyboard.IsPressed(Key.Up))
            {
                sbyte2 &= ~0x08;
            }
            else
            {
                sbyte2 |= 0x08;
            }
            if (Keyboard.IsPressed(Key.NumPad1))
            {
                sbyte2 &= ~0x20;
            }
            else
            {
                sbyte2 |= 0x20;
            }
            if (Keyboard.IsPressed(Key.NumPad2))
            {
                sbyte2 &= ~0x10;
            }
            else
            {
                sbyte2 |= 0x10;
            }
            /*if (Keyboard.IsPressed(Key.NumPad3))
            {
                sbyte1 &= ~0x40;
            }
            else
            {
                sbyte1 |= 0x40;
            }*/
            if (Keyboard.IsPressed(Key.R))
            {
                sbyte0 &= ~0x02;
            }
            else
            {
                sbyte0 |= 0x02;
            }
            /*if (Keyboard.IsPressed(Key.T))
            {
                sbyte0 &= ~0x02;
            }
            else
            {
                sbyte0 |= 0x02;
            }*/
        }
        public static void loop_inputports_taito_boblbobl()
        {
            if (Keyboard.IsPressed(Key.D5))
            {
                sbyte0 |= 0x08;
            }
            else
            {
                sbyte0 &= ~0x08;
            }
            if (Keyboard.IsPressed(Key.D6))
            {
                sbyte0 |= 0x04;
            }
            else
            {
                sbyte0 &= ~0x04;
            }
            if (Keyboard.IsPressed(Key.D1))
            {
                sbyte0 &= ~0x40;
            }
            else
            {
                sbyte0 |= 0x40;
            }
            if (Keyboard.IsPressed(Key.D2))
            {
                sbyte1 &= ~0x40;
            }
            else
            {
                sbyte1 |= 0x40;
            }
            if (Keyboard.IsPressed(Key.D))
            {
                sbyte0 &= ~0x02;
            }
            else
            {
                sbyte0 |= 0x02;
            }
            if (Keyboard.IsPressed(Key.A))
            {
                sbyte0 &= ~0x01;
            }
            else
            {
                sbyte0 |= 0x01;
            }
            /*if (Keyboard.IsPressed(Key.S))
            {
                sbyte1 &= ~0x04;
            }
            else
            {
                sbyte1 |= 0x04;
            }
            if (Keyboard.IsPressed(Key.W))
            {
                sbyte1 &= ~0x08;
            }
            else
            {
                sbyte1 |= 0x08;
            }*/
            if (Keyboard.IsPressed(Key.J))
            {
                sbyte0 &= ~0x20;
            }
            else
            {
                sbyte0 |= 0x20;
            }
            if (Keyboard.IsPressed(Key.K))
            {
                sbyte0 &= ~0x10;
            }
            else
            {
                sbyte0 |= 0x10;
            }
            /*if (Keyboard.IsPressed(Key.L))
            {
                sbyte1 &= ~0x04;
            }
            else
            {
                sbyte1 |= 0x04;
            }*/
            if (Keyboard.IsPressed(Key.Right))
            {
                sbyte1 &= ~0x02;
            }
            else
            {
                sbyte1 |= 0x02;
            }
            if (Keyboard.IsPressed(Key.Left))
            {
                sbyte1 &= ~0x01;
            }
            else
            {
                sbyte1 |= 0x01;
            }
            /*if (Keyboard.IsPressed(Key.Down))
            {
                sbyte2 &= ~0x04;
            }
            else
            {
                sbyte2 |= 0x04;
            }
            if (Keyboard.IsPressed(Key.Up))
            {
                sbyte2 &= ~0x08;
            }
            else
            {
                sbyte2 |= 0x08;
            }*/
            if (Keyboard.IsPressed(Key.NumPad1))
            {
                sbyte1 &= ~0x20;
            }
            else
            {
                sbyte1 |= 0x20;
            }
            if (Keyboard.IsPressed(Key.NumPad2))
            {
                sbyte1 &= ~0x10;
            }
            else
            {
                sbyte1 |= 0x10;
            }
            /*if (Keyboard.IsPressed(Key.NumPad3))
            {
                sbyte1 &= ~0x40;
            }
            else
            {
                sbyte1 |= 0x40;
            }*/
            if (Keyboard.IsPressed(Key.R))
            {
                sbyte1 &= ~0x08;
            }
            else
            {
                sbyte1 |= 0x08;
            }
            /*if (Keyboard.IsPressed(Key.T))
            {
                sbyte0 &= ~0x02;
            }
            else
            {
                sbyte0 |= 0x02;
            }*/
        }
        public static void loop_inputports_taito_opwolf()
        {
            if (Keyboard.IsPressed(Key.D5))
            {
                sbyte0 |= 0x01;
            }
            else
            {
                sbyte0 &= ~0x01;
            }
            if (Keyboard.IsPressed(Key.D6))
            {
                sbyte0 |= 0x02;
            }
            else
            {
                sbyte0 &= ~0x02;
            }
            if (Keyboard.IsPressed(Key.D1))
            {
                sbyte1 &= ~0x10;
            }
            else
            {
                sbyte1 |= 0x10;
            }
            /*if (Keyboard.IsPressed(Key.D2))
            {
                sbyte2 &= ~0x40;
            }
            else
            {
                sbyte2 |= 0x40;
            }
            if (Keyboard.IsPressed(Key.D))
            {
                sbyte1 &= ~0x02;
            }
            else
            {
                sbyte1 |= 0x02;
            }
            if (Keyboard.IsPressed(Key.A))
            {
                sbyte1 &= ~0x01;
            }
            else
            {
                sbyte1 |= 0x01;
            }
            if (Keyboard.IsPressed(Key.S))
            {
                sbyte2 &= ~0x02;
            }
            else
            {
                sbyte2 |= 0x02;
            }
            if (Keyboard.IsPressed(Key.W))
            {
                sbyte2 &= ~0x01;
            }
            else
            {
                sbyte2 |= 0x01;
            }*/
            if (Keyboard.IsPressed(Key.J)|| Mouse.buttons[0] != 0)
            {
                sbyte1 &= ~0x01;
            }
            else
            {
                sbyte1 |= 0x01;
            }
            if (Keyboard.IsPressed(Key.K)|| Mouse.buttons[1] != 0)
            {
                sbyte1 &= ~0x02;
            }
            else
            {
                sbyte1 |= 0x02;
            }
            /*if (Keyboard.IsPressed(Key.L))
            {
                sbyte1 &= ~0x04;
            }
            else
            {
                sbyte1 |= 0x04;
            }
            if (Keyboard.IsPressed(Key.Right))
            {
                sbyte2 &= ~0x02;
            }
            else
            {
                sbyte2 |= 0x02;
            }
            if (Keyboard.IsPressed(Key.Left))
            {
                sbyte2 &= ~0x01;
            }
            else
            {
                sbyte2 |= 0x01;
            }
            if (Keyboard.IsPressed(Key.Down))
            {
                sbyte2 &= ~0x20;
            }
            else
            {
                sbyte2 |= 0x20;
            }
            if (Keyboard.IsPressed(Key.Up))
            {
                sbyte2 &= ~0x10;
            }
            else
            {
                sbyte2 |= 0x10;
            }
            if (Keyboard.IsPressed(Key.NumPad1))
            {
                sbyte2 &= ~0x20;
            }
            else
            {
                sbyte2 |= 0x20;
            }
            if (Keyboard.IsPressed(Key.NumPad2))
            {
                sbyte2 &= ~0x10;
            }
            else
            {
                sbyte2 |= 0x10;
            }
            if (Keyboard.IsPressed(Key.NumPad3))
            {
                sbyte1 &= ~0x40;
            }
            else
            {
                sbyte1 |= 0x40;
            }*/
            if (Keyboard.IsPressed(Key.R))
            {
                sbyte1 &= ~0x04;
            }
            else
            {
                sbyte1 |= 0x04;
            }
            if (Keyboard.IsPressed(Key.T))
            {
                sbyte1 &= ~0x08;
            }
            else
            {
                sbyte1 |= 0x08;
            }
            Inptport.frame_update_analog_field_opwolf_p1x(Inptport.analog_p1x);
            Inptport.frame_update_analog_field_opwolf_p1y(Inptport.analog_p1y);
            p1x = (byte)Inptport.input_port_read_direct(Inptport.analog_p1x);
            p1y = (byte)Inptport.input_port_read_direct(Inptport.analog_p1y);
        }
        public static void loop_inputports_taito_opwolfp()
        {
            if (Keyboard.IsPressed(Key.D5))
            {
                sbyte3 |= 0x02;
            }
            else
            {
                sbyte3 &= ~0x02;
            }
            if (Keyboard.IsPressed(Key.D6))
            {
                sbyte3 |= 0x04;
            }
            else
            {
                sbyte3 &= ~0x04;
            }
            if (Keyboard.IsPressed(Key.D1))
            {
                sbyte2 &= ~0x20;
            }
            else
            {
                sbyte2 |= 0x20;
            }
            if (Keyboard.IsPressed(Key.J) || Mouse.buttons[0] != 0)
            {
                sbyte2 &= ~0x02;
            }
            else
            {
                sbyte2 |= 0x02;
            }
            if (Keyboard.IsPressed(Key.K) || Mouse.buttons[1] != 0)
            {
                sbyte2 &= ~0x04;
            }
            else
            {
                sbyte2 |= 0x04;
            }
            if (Keyboard.IsPressed(Key.R))
            {
                sbyte2 &= ~0x08;
            }
            else
            {
                sbyte2 |= 0x08;
            }
            if (Keyboard.IsPressed(Key.T))
            {
                sbyte2 &= ~0x10;
            }
            else
            {
                sbyte2 |= 0x10;
            }
            Inptport.frame_update_analog_field_opwolf_p1x(Inptport.analog_p1x);
            Inptport.frame_update_analog_field_opwolf_p1y(Inptport.analog_p1y);
            p1x = (byte)Inptport.input_port_read_direct(Inptport.analog_p1x);
            p1y = (byte)Inptport.input_port_read_direct(Inptport.analog_p1y);
        }
        public static void loop_inputports_taito_plumppop()
        {
            if (Keyboard.IsPressed(Key.D5))
            {
                bcoin1 |= 0x01;
                sbyte2 |= 0x10;
            }
            else
            {
                bcoin1 &= unchecked((byte)~0x01);
                sbyte2 &= ~0x10;
            }
            if (Keyboard.IsPressed(Key.D6))
            {
                bcoin2 |= 0x01;
                sbyte2 |= 0x20;
            }
            else
            {
                bcoin2 &= unchecked((byte)~0x01);
                sbyte2 &= ~0x20;
            }
            if (Keyboard.IsPressed(Key.D1))
            {
                sbyte0 &= unchecked((sbyte)~0x80);
            }
            else
            {
                sbyte0 |= unchecked((sbyte)0x80);
            }
            if (Keyboard.IsPressed(Key.D2))
            {
                sbyte1 &= unchecked((sbyte)~0x80);
            }
            else
            {
                sbyte1 |= unchecked((sbyte)0x80);
            }
            if (Keyboard.IsPressed(Key.J))
            {
                sbyte0 &= ~0x10;
            }
            else
            {
                sbyte0 |= 0x10;
            }
            if (Keyboard.IsPressed(Key.K))
            {
                sbyte0 &= ~0x20;
            }
            else
            {
                sbyte0 |= 0x20;
            }
            if (Keyboard.IsPressed(Key.NumPad1))
            {
                sbyte1 &= ~0x10;
            }
            else
            {
                sbyte1 |= 0x10;
            }
            if (Keyboard.IsPressed(Key.NumPad2))
            {
                sbyte1 &= ~0x20;
            }
            else
            {
                sbyte1 |= 0x20;
            }
            if (Keyboard.IsPressed(Key.R))
            {
                sbyte2 &= ~0x01;
            }
            else
            {
                sbyte2 |= 0x01;
            }
            if (Keyboard.IsPressed(Key.T))
            {
                sbyte2 &= ~0x02;
            }
            else
            {
                sbyte2 |= 0x02;
            }
            Inptport.frame_update_analog_field_plumppop_p1x(Inptport.analog_p1x);
            Inptport.frame_update_analog_field_plumppop_p2x(Inptport.analog_p2x);
        }
        public static void loop_inputports_taito_jpopnics()
        {
            if (Keyboard.IsPressed(Key.D5))
            {
                sbyte1 &= ~0x40;
                
            }
            else
            {
                sbyte1 |= 0x40;
            }
            if (Keyboard.IsPressed(Key.D6))
            {
                sbyte2 &= ~0x40;
            }
            else
            {
                sbyte2 |= 0x20;
            }
            if (Keyboard.IsPressed(Key.D1))
            {
                sbyte1 &= unchecked((sbyte)~0x80);
            }
            else
            {
                sbyte1 |= unchecked((sbyte)0x80);
            }
            if (Keyboard.IsPressed(Key.D2))
            {
                sbyte2 &= unchecked((sbyte)~0x80);
            }
            else
            {
                sbyte2 |= unchecked((sbyte)0x80);
            }
            if (Keyboard.IsPressed(Key.J))
            {
                sbyte1 &= ~0x10;
            }
            else
            {
                sbyte1 |= 0x10;
            }
            if (Keyboard.IsPressed(Key.K))
            {
                sbyte1 &= ~0x20;
            }
            else
            {
                sbyte1 |= 0x20;
            }
            if (Keyboard.IsPressed(Key.NumPad1))
            {
                sbyte2 &= ~0x10;
            }
            else
            {
                sbyte2 |= 0x10;
            }
            if (Keyboard.IsPressed(Key.NumPad2))
            {
                sbyte2 &= ~0x20;
            }
            else
            {
                sbyte2 |= 0x20;
            }
            Inptport.frame_update_analog_field_plumppop_p1x(Inptport.analog_p1x);
            Inptport.frame_update_analog_field_plumppop_p2x(Inptport.analog_p2x);
        }
        public static void loop_inputports_taito_extrmatn()
        {
            if (Keyboard.IsPressed(Key.D5))
            {
                bcoin1 |= 0x01;                
            }
            else
            {
                bcoin1 &= unchecked((byte)~0x01);
            }
            if (Keyboard.IsPressed(Key.D6))
            {
                bcoin2 |= 0x01;                
            }
            else
            {
                bcoin2 &= unchecked((byte)~0x01);
            }
            if (Keyboard.IsPressed(Key.D1))
            {
                sbyte0 &= unchecked((sbyte)~0x80);
            }
            else
            {
                sbyte0 |= unchecked((sbyte)0x80);
            }
            if (Keyboard.IsPressed(Key.D2))
            {
                sbyte1 &= unchecked((sbyte)~0x80);
            }
            else
            {
                sbyte1 |= unchecked((sbyte)0x80);
            }
            if (Keyboard.IsPressed(Key.D))
            {
                sbyte0 &= ~0x02;
            }
            else
            {
                sbyte0 |= 0x02;
            }
            if (Keyboard.IsPressed(Key.A))
            {
                sbyte0 &= ~0x01;
            }
            else
            {
                sbyte0 |= 0x01;
            }
            if (Keyboard.IsPressed(Key.S))
            {
                sbyte0 &= ~0x08;
            }
            else
            {
                sbyte0 |= 0x08;
            }
            if (Keyboard.IsPressed(Key.W))
            {
                sbyte0 &= ~0x04;
            }
            else
            {
                sbyte0 |= 0x04;
            }
            if (Keyboard.IsPressed(Key.J))
            {
                sbyte0 &= ~0x10;
            }
            else
            {
                sbyte0 |= 0x10;
            }
            if (Keyboard.IsPressed(Key.K))
            {
                sbyte0 &= ~0x20;
            }
            else
            {
                sbyte0 |= 0x20;
            }
            if (Keyboard.IsPressed(Key.Right))
            {
                sbyte1 &= ~0x02;
            }
            else
            {
                sbyte1 |= 0x02;
            }
            if (Keyboard.IsPressed(Key.Left))
            {
                sbyte1 &= ~0x01;
            }
            else
            {
                sbyte1 |= 0x01;
            }
            if (Keyboard.IsPressed(Key.Down))
            {
                sbyte1 &= ~0x08;
            }
            else
            {
                sbyte1 |= 0x08;
            }
            if (Keyboard.IsPressed(Key.Up))
            {
                sbyte1 &= ~0x04;
            }
            else
            {
                sbyte1 |= 0x04;
            }
            if (Keyboard.IsPressed(Key.NumPad1))
            {
                sbyte1 &= ~0x10;
            }
            else
            {
                sbyte1 |= 0x10;
            }
            if (Keyboard.IsPressed(Key.NumPad2))
            {
                sbyte1 &= ~0x20;
            }
            else
            {
                sbyte1 |= 0x20;
            }
            if (Keyboard.IsPressed(Key.R))
            {
                sbyte2 &= ~0x01;
            }
            else
            {
                sbyte2 |= 0x01;
            }
            if (Keyboard.IsPressed(Key.T))
            {
                sbyte2 &= ~0x02;
            }
            else
            {
                sbyte2 |= 0x02;
            }
        }
        public static void loop_inputports_taito_arknoid2()
        {
            if (Keyboard.IsPressed(Key.D5))
            {
                bcoin1 |= 0x01;
                up1h |= 0x4000;
            }
            else
            {
                bcoin1 &= unchecked((byte)~0x01);
                up1h &= unchecked((ushort)~0x4000);
            }
            if (Keyboard.IsPressed(Key.D6))
            {
                bcoin2 |= 0x01;
                up1h |= 0x1000;
            }
            else
            {
                bcoin2 &= unchecked((byte)~0x01);
                up1h &= unchecked((ushort)~0x1000);
            }
            if (Keyboard.IsPressed(Key.D1))
            {
                sbyte0 &= unchecked((sbyte)~0x80);
            }
            else
            {
                sbyte0 |= unchecked((sbyte)0x80);
            }
            if (Keyboard.IsPressed(Key.D2))
            {
                sbyte1 &= unchecked((sbyte)~0x80);
            }
            else
            {
                sbyte1 |= unchecked((sbyte)0x80);
            }
            if (Keyboard.IsPressed(Key.J))
            {
                sbyte0 &= ~0x10;
            }
            else
            {
                sbyte0 |= 0x10;
            }
            if (Keyboard.IsPressed(Key.K))
            {
                sbyte0 &= ~0x20;
            }
            else
            {
                sbyte0 |= 0x20;
            }
            if (Keyboard.IsPressed(Key.NumPad1))
            {
                sbyte1 &= ~0x10;
            }
            else
            {
                sbyte1 |= 0x10;
            }
            if (Keyboard.IsPressed(Key.NumPad2))
            {
                sbyte1 &= ~0x20;
            }
            else
            {
                sbyte1 |= 0x20;
            }
            if (Keyboard.IsPressed(Key.R))
            {
                sbyte2 &= ~0x01;
                up1h |= 0x2000;
            }
            else
            {
                sbyte2 |= 0x01;
                up1h &= unchecked((ushort)~0x2000);
            }
            if (Keyboard.IsPressed(Key.T))
            {
                sbyte2 &= ~0x02;
            }
            else
            {
                sbyte2 |= 0x02;
            }
            Inptport.frame_update_analog_field_plumppop_p1x(Inptport.analog_p1x);
            Inptport.frame_update_analog_field_plumppop_p2x(Inptport.analog_p2x);
        }
        public static void loop_inputports_taito_tnzs()
        {
            if (Keyboard.IsPressed(Key.D5))
            {
                sbyte2 &= ~0x10;
            }
            else
            {
                sbyte2 |= 0x10;
            }
            if (Keyboard.IsPressed(Key.D6))
            {
                sbyte2 &= ~0x20;
            }
            else
            {
                sbyte2 |= 0x20;
            }
            if (Keyboard.IsPressed(Key.D1))
            {
                sbyte0 &= unchecked((sbyte)~0x80);
            }
            else
            {
                sbyte0 |= unchecked((sbyte)0x80);
            }
            if (Keyboard.IsPressed(Key.D2))
            {
                sbyte1 &= unchecked((sbyte)~0x80);
            }
            else
            {
                sbyte1 |= unchecked((sbyte)0x80);
            }
            if (Keyboard.IsPressed(Key.D))
            {
                sbyte0 &= ~0x02;
            }
            else
            {
                sbyte0 |= 0x02;
            }
            if (Keyboard.IsPressed(Key.A))
            {
                sbyte0 &= ~0x01;
            }
            else
            {
                sbyte0 |= 0x01;
            }
            if (Keyboard.IsPressed(Key.S))
            {
                sbyte0 &= ~0x08;
            }
            else
            {
                sbyte0 |= 0x08;
            }
            if (Keyboard.IsPressed(Key.W))
            {
                sbyte0 &= ~0x04;
            }
            else
            {
                sbyte0 |= 0x04;
            }
            if (Keyboard.IsPressed(Key.J))
            {
                sbyte0 &= ~0x10;
            }
            else
            {
                sbyte0 |= 0x10;
            }
            if (Keyboard.IsPressed(Key.K))
            {
                sbyte0 &= ~0x20;
            }
            else
            {
                sbyte0 |= 0x20;
            }
            /*if (Keyboard.IsPressed(Key.L))
            {
                sbyte1 &= ~0x04;
            }
            else
            {
                sbyte1 |= 0x04;
            }*/
            if (Keyboard.IsPressed(Key.Right))
            {
                sbyte1 &= ~0x02;
            }
            else
            {
                sbyte1 |= 0x02;
            }
            if (Keyboard.IsPressed(Key.Left))
            {
                sbyte1 &= ~0x01;
            }
            else
            {
                sbyte1 |= 0x01;
            }
            if (Keyboard.IsPressed(Key.Down))
            {
                sbyte1 &= ~0x08;
            }
            else
            {
                sbyte1 |= 0x08;
            }
            if (Keyboard.IsPressed(Key.Up))
            {
                sbyte1 &= ~0x04;
            }
            else
            {
                sbyte1 |= 0x04;
            }
            if (Keyboard.IsPressed(Key.NumPad1))
            {
                sbyte1 &= ~0x10;
            }
            else
            {
                sbyte1 |= 0x10;
            }
            if (Keyboard.IsPressed(Key.NumPad2))
            {
                sbyte1 &= ~0x20;
            }
            else
            {
                sbyte1 |= 0x20;
            }
            /*if (Keyboard.IsPressed(Key.NumPad3))
            {
                sbyte1 &= ~0x40;
            }
            else
            {
                sbyte1 |= 0x40;
            }*/
            if (Keyboard.IsPressed(Key.R))
            {
                sbyte2 &= ~0x01;
            }
            else
            {
                sbyte2 |= 0x01;
            }
            if (Keyboard.IsPressed(Key.T))
            {
                sbyte2 &= ~0x02;
            }
            else
            {
                sbyte2 |= 0x02;
            }
        }
        public static void loop_inputports_taito_tnzsjo()
        {
            if (Keyboard.IsPressed(Key.D5))
            {
                bcoin1 &= unchecked((byte)~0x01);
            }
            else
            {
                bcoin1 |= 0x01;
            }
            if (Keyboard.IsPressed(Key.D6))
            {
                bcoin2 &= unchecked((byte)~0x01);
            }
            else
            {
                bcoin2 |= 0x01;
            }
            if (Keyboard.IsPressed(Key.D1))
            {
                sbyte0 &= unchecked((sbyte)~0x80);
            }
            else
            {
                sbyte0 |= unchecked((sbyte)0x80);
            }
            if (Keyboard.IsPressed(Key.D2))
            {
                sbyte1 &= unchecked((sbyte)~0x80);
            }
            else
            {
                sbyte1 |= unchecked((sbyte)0x80);
            }
            if (Keyboard.IsPressed(Key.D))
            {
                sbyte0 &= ~0x02;
            }
            else
            {
                sbyte0 |= 0x02;
            }
            if (Keyboard.IsPressed(Key.A))
            {
                sbyte0 &= ~0x01;
            }
            else
            {
                sbyte0 |= 0x01;
            }
            if (Keyboard.IsPressed(Key.S))
            {
                sbyte0 &= ~0x08;
            }
            else
            {
                sbyte0 |= 0x08;
            }
            if (Keyboard.IsPressed(Key.W))
            {
                sbyte0 &= ~0x04;
            }
            else
            {
                sbyte0 |= 0x04;
            }
            if (Keyboard.IsPressed(Key.J))
            {
                sbyte0 &= ~0x10;
            }
            else
            {
                sbyte0 |= 0x10;
            }
            if (Keyboard.IsPressed(Key.K))
            {
                sbyte0 &= ~0x20;
            }
            else
            {
                sbyte0 |= 0x20;
            }
            /*if (Keyboard.IsPressed(Key.L))
            {
                sbyte1 &= ~0x04;
            }
            else
            {
                sbyte1 |= 0x04;
            }*/
            if (Keyboard.IsPressed(Key.Right))
            {
                sbyte1 &= ~0x02;
            }
            else
            {
                sbyte1 |= 0x02;
            }
            if (Keyboard.IsPressed(Key.Left))
            {
                sbyte1 &= ~0x01;
            }
            else
            {
                sbyte1 |= 0x01;
            }
            if (Keyboard.IsPressed(Key.Down))
            {
                sbyte1 &= ~0x08;
            }
            else
            {
                sbyte1 |= 0x08;
            }
            if (Keyboard.IsPressed(Key.Up))
            {
                sbyte1 &= ~0x04;
            }
            else
            {
                sbyte1 |= 0x04;
            }
            if (Keyboard.IsPressed(Key.NumPad1))
            {
                sbyte1 &= ~0x10;
            }
            else
            {
                sbyte1 |= 0x10;
            }
            if (Keyboard.IsPressed(Key.NumPad2))
            {
                sbyte1 &= ~0x20;
            }
            else
            {
                sbyte1 |= 0x20;
            }
            /*if (Keyboard.IsPressed(Key.NumPad3))
            {
                sbyte1 &= ~0x40;
            }
            else
            {
                sbyte1 |= 0x40;
            }*/
            if (Keyboard.IsPressed(Key.R))
            {
                sbyte2 &= ~0x01;
            }
            else
            {
                sbyte2 |= 0x01;
            }
            if (Keyboard.IsPressed(Key.T))
            {
                sbyte2 &= ~0x02;
            }
            else
            {
                sbyte2 |= 0x02;
            }
        }
        public static void loop_inputports_taito_kabukiz()
        {
            if (Keyboard.IsPressed(Key.D5))
            {
                sbyte2 &= ~0x10;
            }
            else
            {
                sbyte2 |= 0x10;
            }
            if (Keyboard.IsPressed(Key.D6))
            {
                sbyte2 &= ~0x20;
            }
            else
            {
                sbyte2 |= 0x20;
            }
            if (Keyboard.IsPressed(Key.D1))
            {
                sbyte0 &= unchecked((sbyte)~0x80);
            }
            else
            {
                sbyte0 |= unchecked((sbyte)0x80);
            }
            if (Keyboard.IsPressed(Key.D2))
            {
                sbyte1 &= unchecked((sbyte)~0x80);
            }
            else
            {
                sbyte1 |= unchecked((sbyte)0x80);
            }
            if (Keyboard.IsPressed(Key.D))
            {
                sbyte0 &= ~0x02;
            }
            else
            {
                sbyte0 |= 0x02;
            }
            if (Keyboard.IsPressed(Key.A))
            {
                sbyte0 &= ~0x01;
            }
            else
            {
                sbyte0 |= 0x01;
            }
            if (Keyboard.IsPressed(Key.S))
            {
                sbyte0 &= ~0x08;
            }
            else
            {
                sbyte0 |= 0x08;
            }
            if (Keyboard.IsPressed(Key.W))
            {
                sbyte0 &= ~0x04;
            }
            else
            {
                sbyte0 |= 0x04;
            }
            if (Keyboard.IsPressed(Key.J))
            {
                sbyte0 &= ~0x10;
            }
            else
            {
                sbyte0 |= 0x10;
            }
            if (Keyboard.IsPressed(Key.K))
            {
                sbyte0 &= ~0x20;
            }
            else
            {
                sbyte0 |= 0x20;
            }
            if (Keyboard.IsPressed(Key.Right))
            {
                sbyte1 &= ~0x02;
            }
            else
            {
                sbyte1 |= 0x02;
            }
            if (Keyboard.IsPressed(Key.Left))
            {
                sbyte1 &= ~0x01;
            }
            else
            {
                sbyte1 |= 0x01;
            }
            if (Keyboard.IsPressed(Key.Down))
            {
                sbyte1 &= ~0x08;
            }
            else
            {
                sbyte1 |= 0x08;
            }
            if (Keyboard.IsPressed(Key.Up))
            {
                sbyte1 &= ~0x04;
            }
            else
            {
                sbyte1 |= 0x04;
            }
            if (Keyboard.IsPressed(Key.NumPad1))
            {
                sbyte1 &= ~0x10;
            }
            else
            {
                sbyte1 |= 0x10;
            }
            if (Keyboard.IsPressed(Key.NumPad2))
            {
                sbyte1 &= ~0x20;
            }
            else
            {
                sbyte1 |= 0x20;
            }
            if (Keyboard.IsPressed(Key.R))
            {
                sbyte2 &= ~0x01;
            }
            else
            {
                sbyte2 |= 0x01;
            }
        }
        public static void loop_inputports_taito_insectx()
        {
            if (Keyboard.IsPressed(Key.D5))
            {
                sbyte2 &= ~0x08;
            }
            else
            {
                sbyte2 |= 0x08;
            }
            if (Keyboard.IsPressed(Key.D6))
            {
                sbyte2 &= ~0x04;
            }
            else
            {
                sbyte2 |= 0x04;
            }
            if (Keyboard.IsPressed(Key.D1))
            {
                sbyte0 &= unchecked((sbyte)~0x80);
            }
            else
            {
                sbyte0 |= unchecked((sbyte)0x80);
            }
            if (Keyboard.IsPressed(Key.D2))
            {
                sbyte1 &= unchecked((sbyte)~0x80);
            }
            else
            {
                sbyte1 |= unchecked((sbyte)0x80);
            }
            if (Keyboard.IsPressed(Key.D))
            {
                sbyte0 &= ~0x02;
            }
            else
            {
                sbyte0 |= 0x02;
            }
            if (Keyboard.IsPressed(Key.A))
            {
                sbyte0 &= ~0x01;
            }
            else
            {
                sbyte0 |= 0x01;
            }
            if (Keyboard.IsPressed(Key.S))
            {
                sbyte0 &= ~0x08;
            }
            else
            {
                sbyte0 |= 0x08;
            }
            if (Keyboard.IsPressed(Key.W))
            {
                sbyte0 &= ~0x04;
            }
            else
            {
                sbyte0 |= 0x04;
            }
            if (Keyboard.IsPressed(Key.J))
            {
                sbyte0 &= ~0x10;
            }
            else
            {
                sbyte0 |= 0x10;
            }
            if (Keyboard.IsPressed(Key.K))
            {
                sbyte0 &= ~0x20;
            }
            else
            {
                sbyte0 |= 0x20;
            }
            if (Keyboard.IsPressed(Key.Right))
            {
                sbyte1 &= ~0x02;
            }
            else
            {
                sbyte1 |= 0x02;
            }
            if (Keyboard.IsPressed(Key.Left))
            {
                sbyte1 &= ~0x01;
            }
            else
            {
                sbyte1 |= 0x01;
            }
            if (Keyboard.IsPressed(Key.Down))
            {
                sbyte1 &= ~0x08;
            }
            else
            {
                sbyte1 |= 0x08;
            }
            if (Keyboard.IsPressed(Key.Up))
            {
                sbyte1 &= ~0x04;
            }
            else
            {
                sbyte1 |= 0x04;
            }
            if (Keyboard.IsPressed(Key.NumPad1))
            {
                sbyte1 &= ~0x10;
            }
            else
            {
                sbyte1 |= 0x10;
            }
            if (Keyboard.IsPressed(Key.NumPad2))
            {
                sbyte1 &= ~0x20;
            }
            else
            {
                sbyte1 |= 0x20;
            }
            if (Keyboard.IsPressed(Key.R))
            {
                sbyte2 &= ~0x01;
            }
            else
            {
                sbyte2 |= 0x01;
            }
            if (Keyboard.IsPressed(Key.T))
            {
                sbyte2 &= ~0x02;
            }
            else
            {
                sbyte2 |= 0x02;
            }
        }
        public static void record_port_bublbobl()
        {
            if (sbyte0 != sbyte0_old || sbyte1 != sbyte1_old || sbyte2 != sbyte2_old)
            {
                sbyte0_old = sbyte0;
                sbyte1_old = sbyte1;
                sbyte2_old = sbyte2;
                Mame.bwRecord.Write(Video.screenstate.frame_number);
                Mame.bwRecord.Write(sbyte0);
                Mame.bwRecord.Write(sbyte1);
                Mame.bwRecord.Write(sbyte2);
            }
        }
        public static void replay_port_bublbobl()
        {
            if (Inptport.bReplayRead)
            {
                try
                {
                    Video.frame_number_obj = Mame.brRecord.ReadInt64();
                    sbyte0_old = Mame.brRecord.ReadSByte();
                    sbyte1_old = Mame.brRecord.ReadSByte();
                    sbyte2_old = Mame.brRecord.ReadSByte();
                }
                catch
                {
                    Mame.playState = Mame.PlayState.PLAY_REPLAYEND;
                    //Mame.mame_pause(true);
                }
                Inptport.bReplayRead = false;
            }
            if (Video.screenstate.frame_number == Video.frame_number_obj)
            {
                sbyte0 = sbyte0_old;
                sbyte1 = sbyte1_old;
                sbyte2 = sbyte2_old;
                Inptport.bReplayRead = true;
            }
            else
            {
                Inptport.bReplayRead = false;
            }
        }
        public static void record_port_opwolf()
        {
            if (sbyte0 != sbyte0_old || sbyte1 != sbyte1_old || Inptport.analog_p1x.accum != p1x_accum_old || Inptport.analog_p1x.previous != p1x_previous_old || Inptport.analog_p1y.accum != p1y_accum_old || Inptport.analog_p1y.previous != p1y_previous_old)
            {
                sbyte0_old = sbyte0;
                sbyte1_old = sbyte1;
                p1x_accum_old = Inptport.analog_p1x.accum;
                p1x_previous_old = Inptport.analog_p1x.previous;
                p1y_accum_old = Inptport.analog_p1y.accum;
                p1y_previous_old = Inptport.analog_p1y.previous;
                Mame.bwRecord.Write(Video.screenstate.frame_number);
                Mame.bwRecord.Write(sbyte0);
                Mame.bwRecord.Write(sbyte1);
                Mame.bwRecord.Write(Inptport.analog_p1x.accum);
                Mame.bwRecord.Write(Inptport.analog_p1x.previous);
                Mame.bwRecord.Write(Inptport.analog_p1y.accum);
                Mame.bwRecord.Write(Inptport.analog_p1y.previous);
            }
        }
        public static void replay_port_opwolf()
        {
            if (Inptport.bReplayRead)
            {
                try
                {
                    Video.frame_number_obj = Mame.brRecord.ReadInt64();
                    sbyte0_old = Mame.brRecord.ReadSByte();
                    sbyte1_old = Mame.brRecord.ReadSByte();
                    p1x_accum_old = Mame.brRecord.ReadInt32();
                    p1x_previous_old = Mame.brRecord.ReadInt32();
                    p1y_accum_old = Mame.brRecord.ReadInt32();
                    p1y_previous_old = Mame.brRecord.ReadInt32();
                }
                catch
                {
                    Mame.playState = Mame.PlayState.PLAY_REPLAYEND;
                }
                Inptport.bReplayRead = false;
            }
            if (Video.screenstate.frame_number == Video.frame_number_obj)
            {
                sbyte0 = sbyte0_old;
                sbyte1 = sbyte1_old;
                Inptport.analog_p1x.accum = p1x_accum_old;
                Inptport.analog_p1x.previous = p1x_previous_old;
                Inptport.analog_p1y.accum = p1y_accum_old;
                Inptport.analog_p1y.previous = p1y_previous_old;
                Inptport.bReplayRead = true;
            }
            else
            {
                Inptport.bReplayRead = false;
            }
        }
        public static void record_port_opwolfp()
        {
            if (sbyte2 != sbyte2_old || sbyte3 != sbyte3_old || Inptport.analog_p1x.accum != p1x_accum_old || Inptport.analog_p1x.previous != p1x_previous_old || Inptport.analog_p1y.accum != p1y_accum_old || Inptport.analog_p1y.previous != p1y_previous_old)
            {
                sbyte2_old = sbyte2;
                sbyte3_old = sbyte3;
                p1x_accum_old = Inptport.analog_p1x.accum;
                p1x_previous_old = Inptport.analog_p1x.previous;
                p1y_accum_old = Inptport.analog_p1y.accum;
                p1y_previous_old = Inptport.analog_p1y.previous;
                Mame.bwRecord.Write(Video.screenstate.frame_number);
                Mame.bwRecord.Write(sbyte2);
                Mame.bwRecord.Write(sbyte3);
                Mame.bwRecord.Write(Inptport.analog_p1x.accum);
                Mame.bwRecord.Write(Inptport.analog_p1x.previous);
                Mame.bwRecord.Write(Inptport.analog_p1y.accum);
                Mame.bwRecord.Write(Inptport.analog_p1y.previous);
            }
        }
        public static void replay_port_opwolfp()
        {
            if (Inptport.bReplayRead)
            {
                try
                {
                    Video.frame_number_obj = Mame.brRecord.ReadInt64();
                    sbyte2_old = Mame.brRecord.ReadSByte();
                    sbyte3_old = Mame.brRecord.ReadSByte();
                    p1x_accum_old = Mame.brRecord.ReadInt32();
                    p1x_previous_old = Mame.brRecord.ReadInt32();
                    p1y_accum_old = Mame.brRecord.ReadInt32();
                    p1y_previous_old = Mame.brRecord.ReadInt32();
                }
                catch
                {
                    Mame.playState = Mame.PlayState.PLAY_REPLAYEND;
                }
                Inptport.bReplayRead = false;
            }
            if (Video.screenstate.frame_number == Video.frame_number_obj)
            {
                sbyte2 = sbyte2_old;
                sbyte3 = sbyte3_old;
                Inptport.analog_p1x.accum = p1x_accum_old;
                Inptport.analog_p1x.previous = p1x_previous_old;
                Inptport.analog_p1y.accum = p1y_accum_old;
                Inptport.analog_p1y.previous = p1y_previous_old;
                Inptport.bReplayRead = true;
            }
            else
            {
                Inptport.bReplayRead = false;
            }
        }
        public static void record_port_plumppop()
        {
            if (bcoin1 != bcoin1_old || bcoin2 != bcoin2_old || sbyte0 != sbyte0_old || sbyte1 != sbyte1_old || sbyte2 != sbyte2_old || Inptport.analog_p1x.accum != p1x_accum_old || Inptport.analog_p1x.previous != p1x_previous_old || Inptport.analog_p2x.accum != p2x_accum_old || Inptport.analog_p2x.previous != p2x_previous_old)
            {
                bcoin1_old = bcoin1;
                bcoin2_old = bcoin2;
                sbyte0_old = sbyte0;
                sbyte1_old = sbyte1;
                sbyte2_old = sbyte2;
                p1x_accum_old = Inptport.analog_p1x.accum;
                p1x_previous_old = Inptport.analog_p1x.previous;
                p2x_accum_old = Inptport.analog_p2x.accum;
                p2x_previous_old = Inptport.analog_p2x.previous;
                Mame.bwRecord.Write(Video.screenstate.frame_number);
                Mame.bwRecord.Write(bcoin1);
                Mame.bwRecord.Write(bcoin2);
                Mame.bwRecord.Write(sbyte0);
                Mame.bwRecord.Write(sbyte1);
                Mame.bwRecord.Write(sbyte2);
                Mame.bwRecord.Write(Inptport.analog_p1x.accum);
                Mame.bwRecord.Write(Inptport.analog_p1x.previous);
                Mame.bwRecord.Write(Inptport.analog_p2x.accum);
                Mame.bwRecord.Write(Inptport.analog_p2x.previous);
            }
        }
        public static void replay_port_plumppop()
        {
            if (Inptport.bReplayRead)
            {
                try
                {
                    Video.frame_number_obj = Mame.brRecord.ReadInt64();
                    bcoin1_old = Mame.brRecord.ReadByte();
                    bcoin2_old = Mame.brRecord.ReadByte();
                    sbyte0_old = Mame.brRecord.ReadSByte();
                    sbyte1_old = Mame.brRecord.ReadSByte();
                    sbyte2_old = Mame.brRecord.ReadSByte();
                    p1x_accum_old = Mame.brRecord.ReadInt32();
                    p1x_previous_old = Mame.brRecord.ReadInt32();
                    p2x_accum_old = Mame.brRecord.ReadInt32();
                    p2x_previous_old = Mame.brRecord.ReadInt32();
                }
                catch
                {
                    Mame.playState = Mame.PlayState.PLAY_REPLAYEND;
                }
                Inptport.bReplayRead = false;
            }
            if (Video.screenstate.frame_number == Video.frame_number_obj)
            {
                bcoin1 = bcoin1_old;
                bcoin2 = bcoin2_old;
                sbyte0 = sbyte0_old;
                sbyte1 = sbyte1_old;
                sbyte2 = sbyte2_old;
                Inptport.analog_p1x.accum = p1x_accum_old;
                Inptport.analog_p1x.previous = p1x_previous_old;
                Inptport.analog_p2x.accum = p2x_accum_old;
                Inptport.analog_p2x.previous = p2x_previous_old;
                Inptport.bReplayRead = true;
            }
            else
            {
                Inptport.bReplayRead = false;
            }
        }
        public static void record_port_jpopnics()
        {
            if (sbyte1 != sbyte1_old || sbyte2 != sbyte2_old || Inptport.analog_p1x.accum != p1x_accum_old || Inptport.analog_p1x.previous != p1x_previous_old || Inptport.analog_p2x.accum != p2x_accum_old || Inptport.analog_p2x.previous != p2x_previous_old)
            {
                sbyte1_old = sbyte1;
                sbyte2_old = sbyte2;
                p1x_accum_old = Inptport.analog_p1x.accum;
                p1x_previous_old = Inptport.analog_p1x.previous;
                p2x_accum_old = Inptport.analog_p2x.accum;
                p2x_previous_old = Inptport.analog_p2x.previous;
                Mame.bwRecord.Write(Video.screenstate.frame_number);
                Mame.bwRecord.Write(sbyte1);
                Mame.bwRecord.Write(sbyte2);
                Mame.bwRecord.Write(Inptport.analog_p1x.accum);
                Mame.bwRecord.Write(Inptport.analog_p1x.previous);
                Mame.bwRecord.Write(Inptport.analog_p2x.accum);
                Mame.bwRecord.Write(Inptport.analog_p2x.previous);
            }
        }
        public static void replay_port_jpopnics()
        {
            if (Inptport.bReplayRead)
            {
                try
                {
                    Video.frame_number_obj = Mame.brRecord.ReadInt64();
                    sbyte1_old = Mame.brRecord.ReadSByte();
                    sbyte2_old = Mame.brRecord.ReadSByte();
                    p1x_accum_old = Mame.brRecord.ReadInt32();
                    p1x_previous_old = Mame.brRecord.ReadInt32();
                    p2x_accum_old = Mame.brRecord.ReadInt32();
                    p2x_previous_old = Mame.brRecord.ReadInt32();
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
                Inptport.analog_p1x.accum = p1x_accum_old;
                Inptport.analog_p1x.previous = p1x_previous_old;
                Inptport.analog_p2x.accum = p2x_accum_old;
                Inptport.analog_p2x.previous = p2x_previous_old;
                Inptport.bReplayRead = true;
            }
            else
            {
                Inptport.bReplayRead = false;
            }
        }
        public static void record_port_arknoid2()
        {
            if (up1h != up1h_old || bcoin1 != bcoin1_old || bcoin2 != bcoin2_old || sbyte0 != sbyte0_old || sbyte1 != sbyte1_old || sbyte2 != sbyte2_old || Inptport.analog_p1x.accum != p1x_accum_old || Inptport.analog_p1x.previous != p1x_previous_old || Inptport.analog_p2x.accum != p2x_accum_old || Inptport.analog_p2x.previous != p2x_previous_old)
            {
                up1h_old = up1h;
                bcoin1_old = bcoin1;
                bcoin2_old = bcoin2;
                sbyte0_old = sbyte0;
                sbyte1_old = sbyte1;
                sbyte2_old = sbyte2;
                p1x_accum_old = Inptport.analog_p1x.accum;
                p1x_previous_old = Inptport.analog_p1x.previous;
                p2x_accum_old = Inptport.analog_p2x.accum;
                p2x_previous_old = Inptport.analog_p2x.previous;
                Mame.bwRecord.Write(Video.screenstate.frame_number);
                Mame.bwRecord.Write(up1h);
                Mame.bwRecord.Write(bcoin1);
                Mame.bwRecord.Write(bcoin2);
                Mame.bwRecord.Write(sbyte0);
                Mame.bwRecord.Write(sbyte1);
                Mame.bwRecord.Write(sbyte2);
                Mame.bwRecord.Write(Inptport.analog_p1x.accum);
                Mame.bwRecord.Write(Inptport.analog_p1x.previous);
                Mame.bwRecord.Write(Inptport.analog_p2x.accum);
                Mame.bwRecord.Write(Inptport.analog_p2x.previous);
            }
        }
        public static void replay_port_arknoid2()
        {
            if (Inptport.bReplayRead)
            {
                try
                {
                    Video.frame_number_obj = Mame.brRecord.ReadInt64();
                    up1h_old = Mame.brRecord.ReadUInt16();
                    bcoin1_old = Mame.brRecord.ReadByte();
                    bcoin2_old = Mame.brRecord.ReadByte();
                    sbyte0_old = Mame.brRecord.ReadSByte();
                    sbyte1_old = Mame.brRecord.ReadSByte();
                    sbyte2_old = Mame.brRecord.ReadSByte();
                    p1x_accum_old = Mame.brRecord.ReadInt32();
                    p1x_previous_old = Mame.brRecord.ReadInt32();
                    p2x_accum_old = Mame.brRecord.ReadInt32();
                    p2x_previous_old = Mame.brRecord.ReadInt32();
                }
                catch
                {
                    Mame.playState = Mame.PlayState.PLAY_REPLAYEND;
                }
                Inptport.bReplayRead = false;
            }
            if (Video.screenstate.frame_number == Video.frame_number_obj)
            {
                up1h = up1h_old;
                bcoin1 = bcoin1_old;
                bcoin2 = bcoin2_old;
                sbyte0 = sbyte0_old;
                sbyte1 = sbyte1_old;
                sbyte2 = sbyte2_old;
                Inptport.analog_p1x.accum = p1x_accum_old;
                Inptport.analog_p1x.previous = p1x_previous_old;
                Inptport.analog_p2x.accum = p2x_accum_old;
                Inptport.analog_p2x.previous = p2x_previous_old;
                Inptport.bReplayRead = true;
            }
            else
            {
                Inptport.bReplayRead = false;
            }
        }
        public static void record_port_tnzs()
        {
            if (sbyte0 != sbyte0_old || sbyte1 != sbyte1_old || sbyte2 != sbyte2_old)
            {
                sbyte0_old = sbyte0;
                sbyte1_old = sbyte1;
                sbyte2_old = sbyte2;
                Mame.bwRecord.Write(Video.screenstate.frame_number);
                Mame.bwRecord.Write(sbyte0);
                Mame.bwRecord.Write(sbyte1);
                Mame.bwRecord.Write(sbyte2);
            }
        }
        public static void replay_port_tnzs()
        {
            if (Inptport.bReplayRead)
            {
                try
                {
                    Video.frame_number_obj = Mame.brRecord.ReadInt64();
                    sbyte0_old = Mame.brRecord.ReadSByte();
                    sbyte1_old = Mame.brRecord.ReadSByte();
                    sbyte2_old = Mame.brRecord.ReadSByte();
                }
                catch
                {
                    Mame.playState = Mame.PlayState.PLAY_REPLAYEND;
                }
                Inptport.bReplayRead = false;
            }
            if (Video.screenstate.frame_number == Video.frame_number_obj)
            {
                sbyte0 = sbyte0_old;
                sbyte1 = sbyte1_old;
                sbyte2 = sbyte2_old;
                Inptport.bReplayRead = true;
            }
            else
            {
                Inptport.bReplayRead = false;
            }
        }
        public static void record_port_tnzso()
        {
            if (bcoin1 != bcoin1_old || bcoin2 != bcoin2_old || sbyte0 != sbyte0_old || sbyte1 != sbyte1_old || sbyte2 != sbyte2_old)
            {
                bcoin1_old = bcoin1;
                bcoin2_old = bcoin2;
                sbyte0_old = sbyte0;
                sbyte1_old = sbyte1;
                sbyte2_old = sbyte2;
                Mame.bwRecord.Write(Video.screenstate.frame_number);
                Mame.bwRecord.Write(bcoin1);
                Mame.bwRecord.Write(bcoin2);
                Mame.bwRecord.Write(sbyte0);
                Mame.bwRecord.Write(sbyte1);
                Mame.bwRecord.Write(sbyte2);
            }
        }
        public static void replay_port_tnzso()
        {
            if (Inptport.bReplayRead)
            {
                try
                {
                    Video.frame_number_obj = Mame.brRecord.ReadInt64();
                    bcoin1_old = Mame.brRecord.ReadByte();
                    bcoin2_old = Mame.brRecord.ReadByte();
                    sbyte0_old = Mame.brRecord.ReadSByte();
                    sbyte1_old = Mame.brRecord.ReadSByte();
                    sbyte2_old = Mame.brRecord.ReadSByte();
                }
                catch
                {
                    Mame.playState = Mame.PlayState.PLAY_REPLAYEND;
                }
                Inptport.bReplayRead = false;
            }
            if (Video.screenstate.frame_number == Video.frame_number_obj)
            {
                bcoin1 = bcoin1_old;
                bcoin2 = bcoin2_old;
                sbyte0 = sbyte0_old;
                sbyte1 = sbyte1_old;
                sbyte2 = sbyte2_old;
                Inptport.bReplayRead = true;
            }
            else
            {
                Inptport.bReplayRead = false;
            }
        }
    }
}
