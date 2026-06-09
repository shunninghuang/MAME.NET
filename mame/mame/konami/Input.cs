using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX.DirectInput;

namespace mame
{
    public partial class Konami
    {
        public static void loop_inputports_konami_scontra()
        {
            if (Keyboard.IsPressed(Key.D5))
            {
                bytes &= unchecked((byte)~0x01);
            }
            else
            {
                bytes |= 0x01;
            }
            if (Keyboard.IsPressed(Key.D6))
            {
                bytes &= unchecked((byte)~0x02);
            }
            else
            {
                bytes |= 0x02;
            }
            if (Keyboard.IsPressed(Key.D1))
            {
                bytes &= unchecked((byte)~0x08);
            }
            else
            {
                bytes |= 0x08;
            }
            if (Keyboard.IsPressed(Key.D2))
            {
                bytes &= unchecked((byte)~0x10);
            }
            else
            {
                bytes |= 0x10;
            }
            if (Keyboard.IsPressed(Key.A))
            {
                byte1 &= unchecked((byte)~0x01);
            }
            else
            {
                byte1 |= 0x01;
            }
            if (Keyboard.IsPressed(Key.D))
            {
                byte1 &= unchecked((byte)~0x02);
            }
            else
            {
                byte1 |= 0x02;
            }
            if (Keyboard.IsPressed(Key.W))
            {
                byte1 &= unchecked((byte)~0x04);
            }
            else
            {
                byte1 |= 0x04;
            }
            if (Keyboard.IsPressed(Key.S))
            {
                byte1 &= unchecked((byte)~0x08);
            }
            else
            {
                byte1 |= 0x08;
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
            if (Keyboard.IsPressed(Key.Left))
            {
                byte2 &= unchecked((byte)~0x01);
            }
            else
            {
                byte2 |= 0x01;
            }
            if (Keyboard.IsPressed(Key.Right))
            {
                byte2 &= unchecked((byte)~0x02);
            }
            else
            {
                byte2 |= 0x02;
            }
            if (Keyboard.IsPressed(Key.Up))
            {
                byte2 &= unchecked((byte)~0x04);
            }
            else
            {
                byte2 |= 0x04;
            }
            if (Keyboard.IsPressed(Key.Down))
            {
                byte2 &= unchecked((byte)~0x08);
            }
            else
            {
                byte2 |= 0x08;
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
            if (Keyboard.IsPressed(Key.R))
            {
                bytes &= unchecked((byte)~0x04);
            }
            else
            {
                bytes |= 0x04;
            }
        }
        public static void loop_inputports_konami_cuebrick()
        {
            if (Keyboard.IsPressed(Key.D5))
            {
                sbyte0 &= ~0x01;
            }
            else
            {
                sbyte0 |= 0x01;
            }
            if (Keyboard.IsPressed(Key.D6))
            {
                sbyte0 &= ~0x02;
            }
            else
            {
                sbyte0 |= 0x02;
            }
            if (Keyboard.IsPressed(Key.D1))
            {
                sbyte0 &= ~0x08;
            }
            else
            {
                sbyte0 |= 0x08;
            }
            if (Keyboard.IsPressed(Key.D2))
            {
                sbyte0 &= ~0x10;
            }
            else
            {
                sbyte0 |= 0x10;
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
                sbyte1 &= ~0x08;
            }
            else
            {
                sbyte1 |= 0x08;
            }
            if (Keyboard.IsPressed(Key.W))
            {
                sbyte1 &= ~0x04;
            }
            else
            {
                sbyte1 |= 0x04;
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
            if (Keyboard.IsPressed(Key.L))
            {
                sbyte1 &= ~0x40;
            }
            else
            {
                sbyte1 |= 0x40;
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
                sbyte2 &= ~0x08;
            }
            else
            {
                sbyte2 |= 0x08;
            }
            if (Keyboard.IsPressed(Key.Up))
            {
                sbyte2 &= ~0x04;
            }
            else
            {
                sbyte2 |= 0x04;
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
            if (Keyboard.IsPressed(Key.NumPad3))
            {
                sbyte2 &= ~0x40;
            }
            else
            {
                sbyte2 |= 0x40;
            }
            if (Keyboard.IsPressed(Key.R))
            {
                sbyte0 &= ~0x40;
            }
            else
            {
                sbyte0 |= 0x40;
            }
            if (Keyboard.IsPressed(Key.T))
            {
                //sbyte0 &= ~0x20;
            }
            else
            {
                //sbyte0 |= 0x20;
            }
        }
        public static void loop_inputports_konami_tmnt()
        {
            if (Keyboard.IsPressed(Key.D5))
            {
                sbyte0 &= ~0x01;
            }
            else
            {
                sbyte0 |= 0x01;
            }
            if (Keyboard.IsPressed(Key.D6))
            {
                sbyte0 &= ~0x02;
            }
            else
            {
                sbyte0 |= 0x02;
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
                sbyte1 &= ~0x08;
            }
            else
            {
                sbyte1 |= 0x08;
            }
            if (Keyboard.IsPressed(Key.W))
            {
                sbyte1 &= ~0x04;
            }
            else
            {
                sbyte1 |= 0x04;
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
                sbyte2 &= ~0x08;
            }
            else
            {
                sbyte2 |= 0x08;
            }
            if (Keyboard.IsPressed(Key.Up))
            {
                sbyte2 &= ~0x04;
            }
            else
            {
                sbyte2 |= 0x04;
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
            if (Keyboard.IsPressed(Key.R))
            {
                sbyte0 &= ~0x10;
            }
            else
            {
                sbyte0 |= 0x10;
            }
            if (Keyboard.IsPressed(Key.T))
            {
                sbyte0 &= ~0x20;
            }
            else
            {
                sbyte0 |= 0x20;
            }
        }
        public static void loop_inputports_konami_punkshot2()
        {
            if (Keyboard.IsPressed(Key.D5))
            {
                sbyte0 &= ~0x01;
            }
            else
            {
                sbyte0 |= 0x01;
            }
            if (Keyboard.IsPressed(Key.D6))
            {
                sbyte0 &= ~0x02;
            }
            else
            {
                sbyte0 |= 0x02;
            }
            if (Keyboard.IsPressed(Key.D1))
            {
                dsw3 &= unchecked((byte)~0x01);
            }
            else
            {
                dsw3 |= 0x01;
            }
            if (Keyboard.IsPressed(Key.D2))
            {
                dsw3 &= unchecked((byte)~0x02);
            }
            else
            {
                dsw3 |= 0x02;
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
                sbyte1 &= ~0x08;
            }
            else
            {
                sbyte1 |= 0x08;
            }
            if (Keyboard.IsPressed(Key.W))
            {
                sbyte1 &= ~0x04;
            }
            else
            {
                sbyte1 |= 0x04;
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
                sbyte2 &= ~0x08;
            }
            else
            {
                sbyte2 |= 0x08;
            }
            if (Keyboard.IsPressed(Key.Up))
            {
                sbyte2 &= ~0x04;
            }
            else
            {
                sbyte2 |= 0x04;
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
            if (Keyboard.IsPressed(Key.R))
            {
                sbyte0 &= ~0x10;
            }
            else
            {
                sbyte0 |= 0x10;
            }
        }
        public static void loop_inputports_konami_blswhstl()
        {
            if (Keyboard.IsPressed(Key.D5))
            {
                sbyte0 &= ~0x01;
            }
            else
            {
                sbyte0 |= 0x01;
            }
            if (Keyboard.IsPressed(Key.D6))
            {
                sbyte0 &= ~0x02;
            }
            else
            {
                sbyte0 |= 0x02;
            }
            if(Keyboard.IsPressed(Key.D1))
            {
                sbyte0 &= ~0x10;
            }
            else
            {
                sbyte0 |= 0x10;
            }
            if (Keyboard.IsPressed(Key.D2))
            {
                sbyte0 &= ~0x20;
            }
            else
            {
                sbyte0 |= 0x20;
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
                sbyte1 &= ~0x08;
            }
            else
            {
                sbyte1 |= 0x08;
            }
            if (Keyboard.IsPressed(Key.W))
            {
                sbyte1 &= ~0x04;
            }
            else
            {
                sbyte1 |= 0x04;
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
            if (Keyboard.IsPressed(Key.L))
            {
                //sbyte1 &= ~0x04;
            }
            else
            {
                //sbyte1 |= 0x04;
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
                sbyte2 &= ~0x08;
            }
            else
            {
                sbyte2 |= 0x08;
            }
            if (Keyboard.IsPressed(Key.Up))
            {
                sbyte2 &= ~0x04;
            }
            else
            {
                sbyte2 |= 0x04;
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
            if (Keyboard.IsPressed(Key.NumPad3))
            {
                //sbyte1 &= ~0x40;
            }
            else
            {
                //sbyte1 |= 0x40;
            }
            if (Keyboard.IsPressed(Key.R))
            {
                sbyte0 &= ~0x04;
            }
            else
            {
                sbyte0 |= 0x04;
            }
            if (Keyboard.IsPressed(Key.T))
            {
                //sbyte0 &= ~0x20;
            }
            else
            {
                //sbyte0 |= 0x20;
            }
        }
        public static void loop_inputports_konami_glfgreat()
        {
            if (Keyboard.IsPressed(Key.D5))
            {
                sbyte0 &= ~0x01;
            }
            else
            {
                sbyte0 |= 0x01;
            }
            if (Keyboard.IsPressed(Key.D6))
            {
                sbyte0 &= ~0x02;
            }
            else
            {
                sbyte0 |= 0x02;
            }
            if (Keyboard.IsPressed(Key.D1))
            {
                dsw3 &= unchecked((byte)~0x01);
            }
            else
            {
                dsw3 |= 0x01;
            }
            if (Keyboard.IsPressed(Key.D2))
            {
                dsw3 &= unchecked((byte)~0x02);
            }
            else
            {
                dsw3 |= 0x02;
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
                sbyte1 &= ~0x08;
            }
            else
            {
                sbyte1 |= 0x08;
            }
            if (Keyboard.IsPressed(Key.W))
            {
                sbyte1 &= ~0x04;
            }
            else
            {
                sbyte1 |= 0x04;
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
            if (Keyboard.IsPressed(Key.L))
            {
                sbyte1 &= ~0x40;
            }
            else
            {
                sbyte1 |= 0x40;
            }
            if (Keyboard.IsPressed(Key.U))
            {
                sbyte1 &= unchecked((sbyte)~0x80);
            }
            else
            {
                sbyte1 |= unchecked((sbyte)0x80);
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
                sbyte2 &= ~0x08;
            }
            else
            {
                sbyte2 |= 0x08;
            }
            if (Keyboard.IsPressed(Key.Up))
            {
                sbyte2 &= ~0x04;
            }
            else
            {
                sbyte2 |= 0x04;
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
            if (Keyboard.IsPressed(Key.NumPad3))
            {
                sbyte2 &= ~0x40;
            }
            else
            {
                sbyte2 |= 0x40;
            }
            if(Keyboard.IsPressed(Key.NumPad4))
            {
                sbyte2 &= unchecked((sbyte)~0x80);
            }
            else
            {
                sbyte2 |= unchecked((sbyte)0x80);
            }
            if (Keyboard.IsPressed(Key.R))
            {
                sbyte0 &= ~0x10;
            }
            else
            {
                sbyte0 |= 0x10;
            }
            if (Keyboard.IsPressed(Key.T))
            {
                //sbyte0 &= ~0x20;
            }
            else
            {
                //sbyte0 |= 0x20;
            }
        }
        public static void loop_inputports_konami_qgakumon()
        {
            if (Keyboard.IsPressed(Key.D5))
            {
                sbyte0 &= ~0x01;
            }
            else
            {
                sbyte0 |= 0x01;
            }
            if (Keyboard.IsPressed(Key.D6))
            {
                sbyte0 &= ~0x02;
            }
            else
            {
                sbyte0 |= 0x02;
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
                sbyte1 &= ~0x08;
            }
            else
            {
                sbyte1 |= 0x08;
            }
            if (Keyboard.IsPressed(Key.W))
            {
                sbyte1 &= ~0x04;
            }
            else
            {
                sbyte1 |= 0x04;
            }
            if (Keyboard.IsPressed(Key.J))
            {
                sbyte1 &= ~0x10;
            }
            else
            {
                sbyte1 |= 0x10;
            }
            /*if (Keyboard.IsPressed(Key.K))
            {
                sbyte1 &= ~0x20;
            }
            else
            {
                sbyte1 |= 0x20;
            }
            if (Keyboard.IsPressed(Key.L))
            {
                sbyte1 &= ~0x40;
            }
            else
            {
                sbyte1 |= 0x40;
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
                sbyte2 &= ~0x08;
            }
            else
            {
                sbyte2 |= 0x08;
            }
            if (Keyboard.IsPressed(Key.Up))
            {
                sbyte2 &= ~0x04;
            }
            else
            {
                sbyte2 |= 0x04;
            }
            if (Keyboard.IsPressed(Key.NumPad1))
            {
                sbyte2 &= ~0x10;
            }
            else
            {
                sbyte2 |= 0x10;
            }
            /*if (Keyboard.IsPressed(Key.NumPad2))
            {
                sbyte2 &= ~0x20;
            }
            else
            {
                sbyte2 |= 0x20;
            }
            if (Keyboard.IsPressed(Key.NumPad3))
            {
                sbyte2 &= ~0x40;
            }
            else
            {
                sbyte2 |= 0x40;
            }*/
            if (Keyboard.IsPressed(Key.R))
            {
                //sbyte0 &= ~0x40;
            }
            else
            {
                //sbyte0 |= 0x40;
            }
            if (Keyboard.IsPressed(Key.T))
            {
                //sbyte0 &= ~0x20;
            }
            else
            {
                //sbyte0 |= 0x20;
            }
        }
        public static void loop_inputports_konami_ssriders()
        {
            if (Keyboard.IsPressed(Key.D5))
            {
                sbyte0 &= ~0x01;
            }
            else
            {
                sbyte0 |= 0x01;
            }
            if (Keyboard.IsPressed(Key.D6))
            {
                sbyte0 &= ~0x02;
            }
            else
            {
                sbyte0 |= 0x02;
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
                sbyte1 &= ~0x08;
            }
            else
            {
                sbyte1 |= 0x08;
            }
            if (Keyboard.IsPressed(Key.W))
            {
                sbyte1 &= ~0x04;
            }
            else
            {
                sbyte1 |= 0x04;
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
            if (Keyboard.IsPressed(Key.L))
            {
                //sbyte1 &= ~0x04;
            }
            else
            {
                //sbyte1 |= 0x04;
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
                sbyte2 &= ~0x08;
            }
            else
            {
                sbyte2 |= 0x08;
            }
            if (Keyboard.IsPressed(Key.Up))
            {
                sbyte2 &= ~0x04;
            }
            else
            {
                sbyte2 |= 0x04;
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
            if (Keyboard.IsPressed(Key.NumPad3))
            {
                //sbyte1 &= ~0x40;
            }
            else
            {
                //sbyte1 |= 0x40;
            }
            if (Keyboard.IsPressed(Key.R))
            {
                sbyte0 &= ~0x10;
            }
            else
            {
                sbyte0 |= 0x10;
            }
            if (Keyboard.IsPressed(Key.T))
            {
                sbyte0 &= ~0x20;
            }
            else
            {
                sbyte0 |= 0x20;
            }
        }
        public static void loop_inputports_konami_thndrx2()
        {
            if (Keyboard.IsPressed(Key.D5))
            {
                sbyte0 &= ~0x01;
            }
            else
            {
                sbyte0 |= 0x01;
            }
            if (Keyboard.IsPressed(Key.D6))
            {
                sbyte0 &= ~0x02;
            }
            else
            {
                sbyte0 |= 0x02;
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
                sbyte1 &= ~0x08;
            }
            else
            {
                sbyte1 |= 0x08;
            }
            if (Keyboard.IsPressed(Key.W))
            {
                sbyte1 &= ~0x04;
            }
            else
            {
                sbyte1 |= 0x04;
            }
            if (((byte)sbyte1 & 0x03) == 0)
            {
                sbyte1 |= 0x03;
            }
            if (((byte)sbyte1 & 0x0c) == 0)
            {
                sbyte1 |= 0x0c;
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
            if (Keyboard.IsPressed(Key.L))
            {
                //sbyte1 &= ~0x04;
            }
            else
            {
                //sbyte1 |= 0x04;
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
                sbyte2 &= ~0x08;
            }
            else
            {
                sbyte2 |= 0x08;
            }
            if (Keyboard.IsPressed(Key.Up))
            {
                sbyte2 &= ~0x04;
            }
            else
            {
                sbyte2 |= 0x04;
            }
            if (((byte)sbyte2 & 0x03) == 0)
            {
                sbyte2 |= 0x03;
            }
            if (((byte)sbyte2 & 0x0c) == 0)
            {
                sbyte2 |= 0x0c;
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
            if (Keyboard.IsPressed(Key.NumPad3))
            {
                //sbyte1 &= ~0x40;
            }
            else
            {
                //sbyte1 |= 0x40;
            }
            if (Keyboard.IsPressed(Key.R))
            {
                sbyte0 &= ~0x04;
            }
            else
            {
                sbyte0 |= 0x04;
            }
            if (Keyboard.IsPressed(Key.T))
            {
                //sbyte0 &= ~0x20;
            }
            else
            {
                //sbyte0 |= 0x20;
            }
        }
        public static void loop_inputports_konami_prmrsocr()
        {
            if (Keyboard.IsPressed(Key.D5))
            {
                sbyte0 &= ~0x04;
            }
            else
            {
                sbyte0 |= 0x04;
            }
            if (Keyboard.IsPressed(Key.D6))
            {
                bytee &= unchecked((byte)~0x04);
            }
            else
            {
                bytee |= 0x04;
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
                sbyte1 &= ~0x08;
            }
            else
            {
                sbyte1 |= 0x08;
            }
            if (Keyboard.IsPressed(Key.W))
            {
                sbyte1 &= ~0x04;
            }
            else
            {
                sbyte1 |= 0x04;
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
            if (Keyboard.IsPressed(Key.L))
            {
                //sbyte1 &= ~0x04;
            }
            else
            {
                //sbyte1 |= 0x04;
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
                sbyte2 &= ~0x08;
            }
            else
            {
                sbyte2 |= 0x08;
            }
            if (Keyboard.IsPressed(Key.Up))
            {
                sbyte2 &= ~0x04;
            }
            else
            {
                sbyte2 |= 0x04;
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
            if (Keyboard.IsPressed(Key.NumPad3))
            {
                //sbyte1 &= ~0x40;
            }
            else
            {
                //sbyte1 |= 0x40;
            }
            if (Keyboard.IsPressed(Key.R))
            {
                sbyte0 &= ~0x01;
            }
            else
            {
                sbyte0 |= 0x01;
            }
            if (Keyboard.IsPressed(Key.T))
            {
                //sbyte0 &= ~0x20;
            }
            else
            {
                //sbyte0 |= 0x20;
            }
        }
        public static void record_port_scontra()
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
        public static void replay_port_scontra()
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
        public static void record_port_cuebrick()
        {
            if (sbyte0 != sbyte0_old || sbyte1 != sbyte1_old || sbyte2 != sbyte2_old || sbyte3 != sbyte3_old || sbyte4 != sbyte4_old)
            {
                sbyte0_old = sbyte0;
                sbyte1_old = sbyte1;
                sbyte2_old = sbyte2;
                sbyte3_old = sbyte3;
                sbyte4_old = sbyte4;
                Mame.bwRecord.Write(Video.screenstate.frame_number);
                Mame.bwRecord.Write(sbyte0);
                Mame.bwRecord.Write(sbyte1);
                Mame.bwRecord.Write(sbyte2);
                Mame.bwRecord.Write(sbyte3);
                Mame.bwRecord.Write(sbyte4);
            }
        }
        public static void replay_port_cuebrick()
        {
            if (Inptport.bReplayRead)
            {
                try
                {
                    Video.frame_number_obj = Mame.brRecord.ReadInt64();
                    sbyte0_old = Mame.brRecord.ReadSByte();
                    sbyte1_old = Mame.brRecord.ReadSByte();
                    sbyte2_old = Mame.brRecord.ReadSByte();
                    sbyte3_old = Mame.brRecord.ReadSByte();
                    sbyte4_old = Mame.brRecord.ReadSByte();
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
                sbyte3 = sbyte3_old;
                sbyte4 = sbyte4_old;
                Inptport.bReplayRead = true;
            }
            else
            {
                Inptport.bReplayRead = false;
            }
        }
        public static void record_port_punkshot2()
        {
            if (sbyte0 != sbyte0_old || sbyte1 != sbyte1_old || sbyte2 != sbyte2_old || dsw3 != dsw3_old)
            {
                sbyte0_old = sbyte0;
                sbyte1_old = sbyte1;
                sbyte2_old = sbyte2;
                dsw3_old = dsw3;
                Mame.bwRecord.Write(Video.screenstate.frame_number);
                Mame.bwRecord.Write(sbyte0);
                Mame.bwRecord.Write(sbyte1);
                Mame.bwRecord.Write(sbyte2);
                Mame.bwRecord.Write(dsw3);
            }
        }
        public static void replay_port_punkshot2()
        {
            if (Inptport.bReplayRead)
            {
                try
                {
                    Video.frame_number_obj = Mame.brRecord.ReadInt64();
                    sbyte0_old = Mame.brRecord.ReadSByte();
                    sbyte1_old = Mame.brRecord.ReadSByte();
                    sbyte2_old = Mame.brRecord.ReadSByte();
                    dsw3_old = Mame.brRecord.ReadByte();
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
                dsw3 = dsw3_old;
                Inptport.bReplayRead = true;
            }
            else
            {
                Inptport.bReplayRead = false;
            }
        }
        public static void record_port_prmrsocr()
        {
            if (sbyte0 != sbyte0_old || sbyte1 != sbyte1_old || sbyte2 != sbyte2_old || sbyte3 != sbyte3_old || sbyte4 != sbyte4_old||bytee!=bytee_old)
            {
                sbyte0_old = sbyte0;
                sbyte1_old = sbyte1;
                sbyte2_old = sbyte2;
                sbyte3_old = sbyte3;
                sbyte4_old = sbyte4;
                bytee_old = bytee;
                Mame.bwRecord.Write(Video.screenstate.frame_number);
                Mame.bwRecord.Write(sbyte0);
                Mame.bwRecord.Write(sbyte1);
                Mame.bwRecord.Write(sbyte2);
                Mame.bwRecord.Write(sbyte3);
                Mame.bwRecord.Write(sbyte4);
                Mame.bwRecord.Write(bytee);
            }
        }
        public static void replay_port_prmrsocr()
        {
            if (Inptport.bReplayRead)
            {
                try
                {
                    Video.frame_number_obj = Mame.brRecord.ReadInt64();
                    sbyte0_old = Mame.brRecord.ReadSByte();
                    sbyte1_old = Mame.brRecord.ReadSByte();
                    sbyte2_old = Mame.brRecord.ReadSByte();
                    sbyte3_old = Mame.brRecord.ReadSByte();
                    sbyte4_old = Mame.brRecord.ReadSByte();
                    bytee_old = Mame.brRecord.ReadByte();
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
                sbyte3 = sbyte3_old;
                sbyte4 = sbyte4_old;
                bytee = bytee_old;
                Inptport.bReplayRead = true;
            }
            else
            {
                Inptport.bReplayRead = false;
            }
        }
    }
}
