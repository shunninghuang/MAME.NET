using System;

namespace cpu.z80
{
    public partial class Z80A
    {
        public ushort PC
        {
            get
            {
                return RegPC.LowWord;
            }
        }

        private bool RegFlagC
        {
            get { return (RegAF.LowByte & 0x01) != 0; }
            set { RegAF.LowByte = (byte)((RegAF.LowByte & ~0x01) | (value ? 0x01 : 0x00)); }
        }

        private bool RegFlagN
        {
            get { return (RegAF.LowByte & 0x02) != 0; }
            set { RegAF.LowByte = (byte)((RegAF.LowByte & ~0x02) | (value ? 0x02 : 0x00)); }
        }

        private bool RegFlagP
        {
            get { return (RegAF.LowByte & 0x04) != 0; }
            set { RegAF.LowByte = (byte)((RegAF.LowByte & ~0x04) | (value ? 0x04 : 0x00)); }
        }

        private bool RegFlag3
        {
            get { return (RegAF.LowByte & 0x08) != 0; }
            set { RegAF.LowByte = (byte)((RegAF.LowByte & ~0x08) | (value ? 0x08 : 0x00)); }
        }

        private bool RegFlagH
        {
            get { return (RegAF.LowByte & 0x10) != 0; }
            set { RegAF.LowByte = (byte)((RegAF.LowByte & ~0x10) | (value ? 0x10 : 0x00)); }
        }

        private bool RegFlag5
        {
            get { return (RegAF.LowByte & 0x20) != 0; }
            set { RegAF.LowByte = (byte)((RegAF.LowByte & ~0x20) | (value ? 0x20 : 0x00)); }
        }

        private bool RegFlagZ
        {
            get { return (RegAF.LowByte & 0x40) != 0; }
            set { RegAF.LowByte = (byte)((RegAF.LowByte & ~0x40) | (value ? 0x40 : 0x00)); }
        }

        private bool RegFlagS
        {
            get { return (RegAF.LowByte & 0x80) != 0; }
            set { RegAF.LowByte = (byte)((RegAF.LowByte & ~0x80) | (value ? 0x80 : 0x00)); }
        }

        private RegisterPair RegAF;
        private RegisterPair RegBC;
        private RegisterPair RegDE;
        private RegisterPair RegHL;

        private RegisterPair RegAltAF; // Shadow for A and F
        private RegisterPair RegAltBC; // Shadow for B and C
        private RegisterPair RegAltDE; // Shadow for D and E
        private RegisterPair RegAltHL; // Shadow for H and L

        private byte RegI; // I (interrupt vector)
        private byte RegR; // R (memory refresh)

        private byte RegR2;

        private RegisterPair RegIX; // IX (index register x)
        private RegisterPair RegIY; // IY (index register y)
        private RegisterPair RegWZ; // WZ

        private RegisterPair RegSP; // SP (stack pointer)
        private RegisterPair RegPC; // PC (program counter)

        private void ResetRegisters()
        {
            RegI = 0; RegR = 0; RegR2 = 0;
            RegPC.LowWord = 0;
            RegWZ.LowWord = 0;
        }

        public byte RegisterA
        {
            get { return RegAF.HighByte; }
            set { RegAF.HighByte = value; }
        }

        public byte RegisterF
        {
            get { return RegAF.LowByte; }
            set { RegAF.LowByte = value; }
        }

        public ushort RegisterAF
        {
            get { return RegAF.LowWord; }
            set { RegAF.LowWord = value; }
        }

        public byte RegisterB
        {
            get { return RegBC.HighByte; }
            set { RegBC.HighByte = value; }
        }

        public byte RegisterC
        {
            get { return RegBC.LowByte; }
            set { RegBC.LowByte = value; }
        }

        public ushort RegisterBC
        {
            get { return RegBC.LowWord; }
            set { RegBC.LowWord = value; }
        }

        public byte RegisterD
        {
            get { return RegDE.HighByte; }
            set { RegDE.HighByte = value; }
        }

        public byte RegisterE
        {
            get { return RegDE.LowByte; }
            set { RegDE.LowByte = value; }
        }
        public ushort RegisterDE
        {
            get { return RegDE.LowWord; }
            set { RegDE.LowWord = value; }
        }

        public byte RegisterH
        {
            get { return RegHL.HighByte; }
            set { RegHL.HighByte = value; }
        }

        public byte RegisterL
        {
            get { return RegHL.LowByte; }
            set { RegHL.LowByte = value; }
        }
        public ushort RegisterHL
        {
            get { return RegHL.LowWord; }
            set { RegHL.LowWord = value; }
        }

        public ushort RegisterPC
        {
            get { return RegPC.LowWord; }
            set { RegPC.LowWord = value; }
        }
        public ushort RegisterSP
        {
            get { return RegSP.LowWord; }
            set { RegSP.LowWord = value; }
        }
        public ushort RegisterIX
        {
            get { return RegIX.LowWord; }
            set { RegIX.LowWord = value; }
        }
        public ushort RegisterIY
        {
            get { return RegIY.LowWord; }
            set { RegIY.LowWord = value; }
        }
        public ushort RegisterWZ
        {
            get { return RegWZ.LowWord; }
            set { RegWZ.LowWord = value; }
        }
        public byte RegisterI
        {
            get { return RegI; }
            set { RegI = value; }
        }
        public byte RegisterR
        {
            get { return RegR; }
            set { RegR = value; }
        }
        public byte RegisterR2
        {
            get { return RegR2; }
            set { RegR2 = value; }
        }
        public ushort RegisterShadowAF
        {
            get { return RegAltAF.LowWord; }
            set { RegAltAF.LowWord = value; }
        }
        public ushort RegisterShadowBC
        {
            get { return RegAltBC.LowWord; }
            set { RegAltBC.LowWord = value; }
        }
        public ushort RegisterShadowDE
        {
            get { return RegAltDE.LowWord; }
            set { RegAltDE.LowWord = value; }
        }
        public ushort RegisterShadowHL
        {
            get { return RegAltHL.LowWord; }
            set { RegAltHL.LowWord = value; }
        }
    }
}