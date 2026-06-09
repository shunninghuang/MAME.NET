using System.IO;
using mame;

namespace cpu.z80
{
    public partial class Z80A
    {
        public ushort PPC = 0;
        public ushort OP;
        /// <summary>
        /// Runs the CPU for a particular number of clock cycles.
        /// </summary>
        /// <param name="cycles">The number of cycles to run the CPU emulator for. Specify -1 to run for a single instruction.</param>
        public override int ExecuteCycles(int cycles)
        {
            pendingCycles = cycles;
            sbyte Displacement;

            bool TBOOL; byte TB; byte TBH; byte TBL; byte TB1; byte TB2; sbyte TSB; ushort TUS; int TI1; int TI2; int TIR;

            // Process interrupt requests.
            if (nonMaskableInterruptPending)
            {
                if (halted)
                {
                    halted = false;
                    RegPC.LowWord++;
                }
                totalExecutedCycles += 11; pendingCycles -= 11;
                nonMaskableInterruptPending = false;
                //iff2 = iff1;
                iff1 = false;
                WriteMemory(--RegSP.LowWord, RegPC.HighByte); WriteMemory(--RegSP.LowWord, RegPC.LowByte);
                RegPC.LowWord = 0x66;
                RegWZ.LowWord = RegPC.LowWord;
                NMICallback();
            }
            do
            {
                if (interrupt && iff1 && Interruptable) //Z80.irq_state iff1 !Z80.after_ei
                {
                    if (halted)
                    {
                        Halted = false;
                        RegPC.LowWord++;
                    }
                    iff1 = iff2 = false;
                    int irq_vector = irq_callback(0);
                    switch (interruptMode)
                    {
                        case 0:
                            switch (irq_vector & 0xff0000)
                            {
                                case 0xcd0000:
                                    WriteMemory(--RegSP.LowWord, RegPC.HighByte); WriteMemory(--RegSP.LowWord, RegPC.LowByte);
                                    RegPC.LowWord = (ushort)(irq_vector & 0xffff);
                                    totalExecutedCycles += 19; pendingCycles -= 19;
                                    break;
                                case 0xc30000:
                                    RegPC.LowWord = (ushort)(irq_vector & 0xffff);
                                    totalExecutedCycles += 12; pendingCycles -= 12;
                                    break;
                                default:
                                    WriteMemory(--RegSP.LowWord, RegPC.HighByte); WriteMemory(--RegSP.LowWord, RegPC.LowByte);
                                    RegPC.LowWord = (ushort)(irq_vector & 0x0038);
                                    totalExecutedCycles += (ulong)(cc_op[RegPC.LowByte] + cc_ex[RegPC.LowByte]); pendingCycles -= cc_op[RegPC.LowByte] + cc_ex[RegPC.LowByte];
                                    break;
                            }
                            break;
                        case 1:
                            WriteMemory(--RegSP.LowWord, RegPC.HighByte); WriteMemory(--RegSP.LowWord, RegPC.LowByte);
                            RegPC.LowWord = 0x38;
                            totalExecutedCycles += 13; pendingCycles -= 13;
                            break;
                        case 2:
                            TUS = (ushort)(RegI * 256 + irq_vector);
                            WriteMemory(--RegSP.LowWord, RegPC.HighByte); WriteMemory(--RegSP.LowWord, RegPC.LowByte);
                            RegPC.LowByte = ReadMemory(TUS++); RegPC.HighByte = ReadMemory(TUS);
                            totalExecutedCycles += 17; pendingCycles -= 17;
                            break;
                    }
                    RegWZ.LowWord = RegPC.LowWord;
                }

                {
                    Interruptable = true;
                    ++RegR;
                    PPC = RegPC.LowWord;
                    OP = ReadOp(PPC);
                    RegPC.LowWord++;
                    debugger_start_cpu_hook_callback();
                    switch (OP)//ReadMemory(RegPC.Word++))
                    {
                        case 0x00: // NOP
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x01: // LD BC, nn
                            RegBC.LowWord = (ushort)(ReadOpArg(RegPC.LowWord++) + ReadOpArg(RegPC.LowWord++) * 256);
                            totalExecutedCycles += 10; pendingCycles -= 10;
                            break;
                        case 0x02: // LD (BC), A
                            WriteMemory(RegBC.LowWord, RegAF.HighByte);
                            RegWZ.LowByte = (byte)((RegBC.LowWord + 1) & 0xFF);
                            RegWZ.HighByte = RegAF.HighByte;
                            totalExecutedCycles += 7; pendingCycles -= 7;
                            break;
                        case 0x03: // INC BC
                            ++RegBC.LowWord;
                            totalExecutedCycles += 6; pendingCycles -= 6;
                            break;
                        case 0x04: // INC B
                            RegAF.LowByte = (byte)(TableInc[++RegBC.HighByte] | (RegAF.LowByte & 1));
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x05: // DEC B
                            RegAF.LowByte = (byte)(TableDec[--RegBC.HighByte] | (RegAF.LowByte & 1));
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x06: // LD B, n
                            RegBC.HighByte = ReadOpArg(RegPC.LowWord++);
                            totalExecutedCycles += 7; pendingCycles -= 7;
                            break;
                        case 0x07: // RLCA
                            RegAF.LowWord = TableRotShift[0, 0, RegAF.LowWord];
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x08: // EX AF, AF'
                            TUS = RegAF.LowWord; RegAF.LowWord = RegAltAF.LowWord; RegAltAF.LowWord = TUS;
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x09: // ADD HL, BC
                            TI1 = (short)RegHL.LowWord; TI2 = (short)RegBC.LowWord; TIR = TI1 + TI2;
                            TUS = (ushort)TIR;
                            RegWZ.LowWord = (ushort)(RegHL.LowWord + 1);
                            RegFlagH = ((TI1 & 0xFFF) + (TI2 & 0xFFF)) > 0xFFF;
                            RegFlagN = false;
                            RegFlagC = ((ushort)TI1 + (ushort)TI2) > 0xFFFF;
                            RegHL.LowWord = TUS;
                            RegFlag3 = (TUS & 0x0800) != 0;
                            RegFlag5 = (TUS & 0x2000) != 0;
                            totalExecutedCycles += 11; pendingCycles -= 11;
                            break;
                        case 0x0A: // LD A, (BC)
                            RegAF.HighByte = ReadMemory(RegBC.LowWord);
                            RegWZ.LowWord = (ushort)(RegBC.LowWord + 1);
                            totalExecutedCycles += 7; pendingCycles -= 7;
                            break;
                        case 0x0B: // DEC BC
                            --RegBC.LowWord;
                            totalExecutedCycles += 6; pendingCycles -= 6;
                            break;
                        case 0x0C: // INC C
                            RegAF.LowByte = (byte)(TableInc[++RegBC.LowByte] | (RegAF.LowByte & 1));
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x0D: // DEC C
                            RegAF.LowByte = (byte)(TableDec[--RegBC.LowByte] | (RegAF.LowByte & 1));
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x0E: // LD C, n
                            RegBC.LowByte = ReadOpArg(RegPC.LowWord++);
                            totalExecutedCycles += 7; pendingCycles -= 7;
                            break;
                        case 0x0F: // RRCA
                            RegAF.LowWord = TableRotShift[0, 1, RegAF.LowWord];
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x10: // DJNZ d
                            TSB = (sbyte)ReadOpArg(RegPC.LowWord++);
                            if (--RegBC.HighByte != 0)
                            {
                                RegPC.LowWord = (ushort)(RegPC.LowWord + TSB);
                                RegWZ.LowWord = RegPC.LowWord;
                                totalExecutedCycles += 13; pendingCycles -= 13;
                            }
                            else
                            {
                                totalExecutedCycles += 8; pendingCycles -= 8;
                            }
                            break;
                        case 0x11: // LD DE, nn
                            RegDE.LowWord = (ushort)(ReadOpArg(RegPC.LowWord++) + ReadOpArg(RegPC.LowWord++) * 256);
                            totalExecutedCycles += 10; pendingCycles -= 10;
                            break;
                        case 0x12: // LD (DE), A
                            WriteMemory(RegDE.LowWord, RegAF.HighByte);
                            RegWZ.LowByte = (byte)((RegDE.LowWord + 1) & 0xFF);
                            RegWZ.HighByte = RegAF.HighByte;
                            totalExecutedCycles += 7; pendingCycles -= 7;
                            break;
                        case 0x13: // INC DE
                            ++RegDE.LowWord;
                            totalExecutedCycles += 6; pendingCycles -= 6;
                            break;
                        case 0x14: // INC D
                            RegAF.LowByte = (byte)(TableInc[++RegDE.HighByte] | (RegAF.LowByte & 1));
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x15: // DEC D
                            RegAF.LowByte = (byte)(TableDec[--RegDE.HighByte] | (RegAF.LowByte & 1));
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x16: // LD D, n
                            RegDE.HighByte = ReadOpArg(RegPC.LowWord++);
                            totalExecutedCycles += 7; pendingCycles -= 7;
                            break;
                        case 0x17: // RLA
                            RegAF.LowWord = TableRotShift[0, 2, RegAF.LowWord];
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x18: // JR d
                            TSB = (sbyte)ReadOpArg(RegPC.LowWord++);
                            RegPC.LowWord = (ushort)(RegPC.LowWord + TSB);
                            RegWZ.LowWord = RegPC.LowWord;
                            totalExecutedCycles += 12; pendingCycles -= 12;
                            break;
                        case 0x19: // ADD HL, DE
                            TI1 = (short)RegHL.LowWord; TI2 = (short)RegDE.LowWord; TIR = TI1 + TI2;
                            TUS = (ushort)TIR;
                            RegWZ.LowWord = (ushort)(RegHL.LowWord + 1);
                            RegFlagH = ((TI1 & 0xFFF) + (TI2 & 0xFFF)) > 0xFFF;
                            RegFlagN = false;
                            RegFlagC = ((ushort)TI1 + (ushort)TI2) > 0xFFFF;
                            RegHL.LowWord = TUS;
                            RegFlag3 = (TUS & 0x0800) != 0;
                            RegFlag5 = (TUS & 0x2000) != 0;
                            totalExecutedCycles += 11; pendingCycles -= 11;
                            break;
                        case 0x1A: // LD A, (DE)
                            RegAF.HighByte = ReadMemory(RegDE.LowWord);
                            RegWZ.LowWord = (ushort)(RegDE.LowWord + 1);
                            totalExecutedCycles += 7; pendingCycles -= 7;
                            break;
                        case 0x1B: // DEC DE
                            --RegDE.LowWord;
                            totalExecutedCycles += 6; pendingCycles -= 6;
                            break;
                        case 0x1C: // INC E
                            RegAF.LowByte = (byte)(TableInc[++RegDE.LowByte] | (RegAF.LowByte & 1));
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x1D: // DEC E
                            RegAF.LowByte = (byte)(TableDec[--RegDE.LowByte] | (RegAF.LowByte & 1));
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x1E: // LD E, n
                            RegDE.LowByte = ReadOpArg(RegPC.LowWord++);
                            totalExecutedCycles += 7; pendingCycles -= 7;
                            break;
                        case 0x1F: // RRA
                            RegAF.LowWord = TableRotShift[0, 3, RegAF.LowWord];
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x20: // JR NZ, d
                            TSB = (sbyte)ReadOpArg(RegPC.LowWord++);
                            if (!RegFlagZ)
                            {
                                RegPC.LowWord = (ushort)(RegPC.LowWord + TSB);
                                RegWZ.LowWord = RegPC.LowWord;
                                totalExecutedCycles += 12; pendingCycles -= 12;
                            }
                            else
                            {
                                totalExecutedCycles += 7; pendingCycles -= 7;
                            }
                            break;
                        case 0x21: // LD HL, nn
                            RegHL.LowWord = (ushort)(ReadOpArg(RegPC.LowWord++) + ReadOpArg(RegPC.LowWord++) * 256);
                            totalExecutedCycles += 10; pendingCycles -= 10;
                            break;
                        case 0x22: // LD (nn), HL
                            TUS = (ushort)(ReadOpArg(RegPC.LowWord++) + ReadOpArg(RegPC.LowWord++) * 256);
                            WriteMemory(TUS++, RegHL.LowByte);
                            WriteMemory(TUS, RegHL.HighByte);
                            RegWZ.LowWord = TUS;
                            totalExecutedCycles += 16; pendingCycles -= 16;
                            break;
                        case 0x23: // INC HL
                            ++RegHL.LowWord;
                            totalExecutedCycles += 6; pendingCycles -= 6;
                            break;
                        case 0x24: // INC H
                            RegAF.LowByte = (byte)(TableInc[++RegHL.HighByte] | (RegAF.LowByte & 1));
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x25: // DEC H
                            RegAF.LowByte = (byte)(TableDec[--RegHL.HighByte] | (RegAF.LowByte & 1));
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x26: // LD H, n
                            RegHL.HighByte = ReadOpArg(RegPC.LowWord++);
                            totalExecutedCycles += 7; pendingCycles -= 7;
                            break;
                        case 0x27: // DAA
                            RegAF.LowWord = TableDaa[RegAF.LowWord];
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x28: // JR Z, d
                            TSB = (sbyte)ReadOpArg(RegPC.LowWord++);
                            if (RegFlagZ)
                            {
                                RegPC.LowWord = (ushort)(RegPC.LowWord + TSB);
                                RegWZ.LowWord = RegPC.LowWord;
                                totalExecutedCycles += 12; pendingCycles -= 12;
                            }
                            else
                            {
                                totalExecutedCycles += 7; pendingCycles -= 7;
                            }
                            break;
                        case 0x29: // ADD HL, HL
                            TI1 = (short)RegHL.LowWord; TI2 = (short)RegHL.LowWord; TIR = TI1 + TI2;
                            TUS = (ushort)TIR;
                            RegWZ.LowWord = (ushort)(RegHL.LowWord + 1);
                            RegFlagH = ((TI1 & 0xFFF) + (TI2 & 0xFFF)) > 0xFFF;
                            RegFlagN = false;
                            RegFlagC = ((ushort)TI1 + (ushort)TI2) > 0xFFFF;
                            RegHL.LowWord = TUS;
                            RegFlag3 = (TUS & 0x0800) != 0;
                            RegFlag5 = (TUS & 0x2000) != 0;
                            totalExecutedCycles += 11; pendingCycles -= 11;
                            break;
                        case 0x2A: // LD HL, (nn)
                            TUS = (ushort)(ReadOpArg(RegPC.LowWord++) + ReadOpArg(RegPC.LowWord++) * 256);
                            RegHL.LowByte = ReadMemory(TUS++); RegHL.HighByte = ReadMemory(TUS);
                            RegWZ.LowWord = TUS;
                            totalExecutedCycles += 16; pendingCycles -= 16;
                            break;
                        case 0x2B: // DEC HL
                            --RegHL.LowWord;
                            totalExecutedCycles += 6; pendingCycles -= 6;
                            break;
                        case 0x2C: // INC L
                            RegAF.LowByte = (byte)(TableInc[++RegHL.LowByte] | (RegAF.LowByte & 1));
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x2D: // DEC L
                            RegAF.LowByte = (byte)(TableDec[--RegHL.LowByte] | (RegAF.LowByte & 1));
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x2E: // LD L, n
                            RegHL.LowByte = ReadOpArg(RegPC.LowWord++);
                            totalExecutedCycles += 7; pendingCycles -= 7;
                            break;
                        case 0x2F: // CPL
                            RegAF.HighByte ^= 0xFF; RegFlagH = true; RegFlagN = true; RegFlag3 = (RegAF.HighByte & 0x08) != 0; RegFlag5 = (RegAF.HighByte & 0x20) != 0;
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x30: // JR NC, d
                            TSB = (sbyte)ReadOpArg(RegPC.LowWord++);
                            if (!RegFlagC)
                            {
                                RegPC.LowWord = (ushort)(RegPC.LowWord + TSB);
                                RegWZ.LowWord = RegPC.LowWord;
                                totalExecutedCycles += 12; pendingCycles -= 12;
                            }
                            else
                            {
                                totalExecutedCycles += 7; pendingCycles -= 7;
                            }
                            break;
                        case 0x31: // LD SP, nn
                            RegSP.LowWord = (ushort)(ReadOpArg(RegPC.LowWord++) + ReadOpArg(RegPC.LowWord++) * 256);
                            totalExecutedCycles += 10; pendingCycles -= 10;
                            break;
                        case 0x32: // LD (nn), A
                            TUS = (ushort)(ReadOpArg(RegPC.LowWord++) + ReadOpArg(RegPC.LowWord++) * 256);
                            WriteMemory(TUS, RegAF.HighByte);
                            RegWZ.LowByte = (byte)((TUS + 1) & 0xFF);
                            RegWZ.HighByte = RegAF.HighByte;
                            totalExecutedCycles += 13; pendingCycles -= 13;
                            break;
                        case 0x33: // INC SP
                            ++RegSP.LowWord;
                            totalExecutedCycles += 6; pendingCycles -= 6;
                            break;
                        case 0x34: // INC (HL)
                            TB = ReadMemory(RegHL.LowWord); RegAF.LowByte = (byte)(TableInc[++TB] | (RegAF.LowByte & 1)); WriteMemory(RegHL.LowWord, TB);
                            totalExecutedCycles += 11; pendingCycles -= 11;
                            break;
                        case 0x35: // DEC (HL)
                            TB = ReadMemory(RegHL.LowWord); RegAF.LowByte = (byte)(TableDec[--TB] | (RegAF.LowByte & 1)); WriteMemory(RegHL.LowWord, TB);
                            totalExecutedCycles += 11; pendingCycles -= 11;
                            break;
                        case 0x36: // LD (HL), n
                            WriteMemory(RegHL.LowWord, ReadOpArg(RegPC.LowWord++));
                            totalExecutedCycles += 10; pendingCycles -= 10;
                            break;
                        case 0x37: // SCF
                            RegFlagH = false; RegFlagN = false; RegFlagC = true; RegFlag3 = (RegAF.HighByte & 0x08) != 0; RegFlag5 = (RegAF.HighByte & 0x20) != 0;
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x38: // JR C, d
                            TSB = (sbyte)ReadOpArg(RegPC.LowWord++);
                            if (RegFlagC)
                            {
                                RegPC.LowWord = (ushort)(RegPC.LowWord + TSB);
                                RegWZ.LowWord = RegPC.LowWord;
                                totalExecutedCycles += 12; pendingCycles -= 12;
                            }
                            else
                            {
                                totalExecutedCycles += 7; pendingCycles -= 7;
                            }
                            break;
                        case 0x39: // ADD HL, SP
                            TI1 = (short)RegHL.LowWord; TI2 = (short)RegSP.LowWord; TIR = TI1 + TI2;
                            TUS = (ushort)TIR;
                            RegWZ.LowWord = (ushort)(RegHL.LowWord + 1);
                            RegFlagH = ((TI1 & 0xFFF) + (TI2 & 0xFFF)) > 0xFFF;
                            RegFlagN = false;
                            RegFlagC = ((ushort)TI1 + (ushort)TI2) > 0xFFFF;
                            RegHL.LowWord = TUS;
                            RegFlag3 = (TUS & 0x0800) != 0;
                            RegFlag5 = (TUS & 0x2000) != 0;
                            totalExecutedCycles += 11; pendingCycles -= 11;
                            break;
                        case 0x3A: // LD A, (nn)
                            TUS = (ushort)(ReadOpArg(RegPC.LowWord++) + ReadOpArg(RegPC.LowWord++) * 256);
                            RegAF.HighByte = ReadMemory(TUS);
                            RegWZ.LowWord = (ushort)(TUS + 1);
                            totalExecutedCycles += 13; pendingCycles -= 13;//
                            break;
                        case 0x3B: // DEC SP
                            --RegSP.LowWord;
                            totalExecutedCycles += 6; pendingCycles -= 6;
                            break;
                        case 0x3C: // INC A
                            RegAF.LowByte = (byte)(TableInc[++RegAF.HighByte] | (RegAF.LowByte & 1));
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x3D: // DEC A
                            RegAF.LowByte = (byte)(TableDec[--RegAF.HighByte] | (RegAF.LowByte & 1));
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x3E: // LD A, n
                            RegAF.HighByte = ReadOpArg(RegPC.LowWord++);
                            totalExecutedCycles += 7; pendingCycles -= 7;
                            break;
                        case 0x3F: // CCF
                            RegFlagH = RegFlagC; RegFlagN = false; RegFlagC ^= true; RegFlag3 = (RegAF.HighByte & 0x08) != 0; RegFlag5 = (RegAF.HighByte & 0x20) != 0;
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x40: // LD B, B
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x41: // LD B, C
                            RegBC.HighByte = RegBC.LowByte;
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x42: // LD B, D
                            RegBC.HighByte = RegDE.HighByte;
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x43: // LD B, E
                            RegBC.HighByte = RegDE.LowByte;
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x44: // LD B, H
                            RegBC.HighByte = RegHL.HighByte;
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x45: // LD B, L
                            RegBC.HighByte = RegHL.LowByte;
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x46: // LD B, (HL)
                            RegBC.HighByte = ReadMemory(RegHL.LowWord);
                            totalExecutedCycles += 7; pendingCycles -= 7;
                            break;
                        case 0x47: // LD B, A
                            RegBC.HighByte = RegAF.HighByte;
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x48: // LD C, B
                            RegBC.LowByte = RegBC.HighByte;
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x49: // LD C, C
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x4A: // LD C, D
                            RegBC.LowByte = RegDE.HighByte;
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x4B: // LD C, E
                            RegBC.LowByte = RegDE.LowByte;
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x4C: // LD C, H
                            RegBC.LowByte = RegHL.HighByte;
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x4D: // LD C, L
                            RegBC.LowByte = RegHL.LowByte;
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x4E: // LD C, (HL)
                            RegBC.LowByte = ReadMemory(RegHL.LowWord);
                            totalExecutedCycles += 7; pendingCycles -= 7;
                            break;
                        case 0x4F: // LD C, A
                            RegBC.LowByte = RegAF.HighByte;
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x50: // LD D, B
                            RegDE.HighByte = RegBC.HighByte;
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x51: // LD D, C
                            RegDE.HighByte = RegBC.LowByte;
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x52: // LD D, D
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x53: // LD D, E
                            RegDE.HighByte = RegDE.LowByte;
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x54: // LD D, H
                            RegDE.HighByte = RegHL.HighByte;
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x55: // LD D, L
                            RegDE.HighByte = RegHL.LowByte;
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x56: // LD D, (HL)
                            RegDE.HighByte = ReadMemory(RegHL.LowWord);
                            totalExecutedCycles += 7; pendingCycles -= 7;
                            break;
                        case 0x57: // LD D, A
                            RegDE.HighByte = RegAF.HighByte;
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x58: // LD E, B
                            RegDE.LowByte = RegBC.HighByte;
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x59: // LD E, C
                            RegDE.LowByte = RegBC.LowByte;
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x5A: // LD E, D
                            RegDE.LowByte = RegDE.HighByte;
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x5B: // LD E, E
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x5C: // LD E, H
                            RegDE.LowByte = RegHL.HighByte;
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x5D: // LD E, L
                            RegDE.LowByte = RegHL.LowByte;
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x5E: // LD E, (HL)
                            RegDE.LowByte = ReadMemory(RegHL.LowWord);
                            totalExecutedCycles += 7; pendingCycles -= 7;
                            break;
                        case 0x5F: // LD E, A
                            RegDE.LowByte = RegAF.HighByte;
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x60: // LD H, B
                            RegHL.HighByte = RegBC.HighByte;
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x61: // LD H, C
                            RegHL.HighByte = RegBC.LowByte;
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x62: // LD H, D
                            RegHL.HighByte = RegDE.HighByte;
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x63: // LD H, E
                            RegHL.HighByte = RegDE.LowByte;
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x64: // LD H, H
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x65: // LD H, L
                            RegHL.HighByte = RegHL.LowByte;
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x66: // LD H, (HL)
                            RegHL.HighByte = ReadMemory(RegHL.LowWord);
                            totalExecutedCycles += 7; pendingCycles -= 7;
                            break;
                        case 0x67: // LD H, A
                            RegHL.HighByte = RegAF.HighByte;
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x68: // LD L, B
                            RegHL.LowByte = RegBC.HighByte;
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x69: // LD L, C
                            RegHL.LowByte = RegBC.LowByte;
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x6A: // LD L, D
                            RegHL.LowByte = RegDE.HighByte;
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x6B: // LD L, E
                            RegHL.LowByte = RegDE.LowByte;
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x6C: // LD L, H
                            RegHL.LowByte = RegHL.HighByte;
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x6D: // LD L, L
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x6E: // LD L, (HL)
                            RegHL.LowByte = ReadMemory(RegHL.LowWord);
                            totalExecutedCycles += 7; pendingCycles -= 7;
                            break;
                        case 0x6F: // LD L, A
                            RegHL.LowByte = RegAF.HighByte;
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x70: // LD (HL), B
                            WriteMemory(RegHL.LowWord, RegBC.HighByte);
                            totalExecutedCycles += 7; pendingCycles -= 7;
                            break;
                        case 0x71: // LD (HL), C
                            WriteMemory(RegHL.LowWord, RegBC.LowByte);
                            totalExecutedCycles += 7; pendingCycles -= 7;
                            break;
                        case 0x72: // LD (HL), D
                            WriteMemory(RegHL.LowWord, RegDE.HighByte);
                            totalExecutedCycles += 7; pendingCycles -= 7;
                            break;
                        case 0x73: // LD (HL), E
                            WriteMemory(RegHL.LowWord, RegDE.LowByte);
                            totalExecutedCycles += 7; pendingCycles -= 7;
                            break;
                        case 0x74: // LD (HL), H
                            WriteMemory(RegHL.LowWord, RegHL.HighByte);
                            totalExecutedCycles += 7; pendingCycles -= 7;
                            break;
                        case 0x75: // LD (HL), L
                            WriteMemory(RegHL.LowWord, RegHL.LowByte);
                            totalExecutedCycles += 7; pendingCycles -= 7;
                            break;
                        case 0x76: // HALT
                            Halt();
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x77: // LD (HL), A
                            WriteMemory(RegHL.LowWord, RegAF.HighByte);
                            totalExecutedCycles += 7; pendingCycles -= 7;
                            break;
                        case 0x78: // LD A, B
                            RegAF.HighByte = RegBC.HighByte;
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x79: // LD A, C
                            RegAF.HighByte = RegBC.LowByte;
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x7A: // LD A, D
                            RegAF.HighByte = RegDE.HighByte;
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x7B: // LD A, E
                            RegAF.HighByte = RegDE.LowByte;
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x7C: // LD A, H
                            RegAF.HighByte = RegHL.HighByte;
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x7D: // LD A, L
                            RegAF.HighByte = RegHL.LowByte;
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x7E: // LD A, (HL)
                            RegAF.HighByte = ReadMemory(RegHL.LowWord);
                            totalExecutedCycles += 7; pendingCycles -= 7;
                            break;
                        case 0x7F: // LD A, A
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x80: // ADD A, B
                            RegAF.LowWord = TableALU[0, RegAF.HighByte, RegBC.HighByte, 0];
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x81: // ADD A, C
                            RegAF.LowWord = TableALU[0, RegAF.HighByte, RegBC.LowByte, 0];
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x82: // ADD A, D
                            RegAF.LowWord = TableALU[0, RegAF.HighByte, RegDE.HighByte, 0];
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x83: // ADD A, E
                            RegAF.LowWord = TableALU[0, RegAF.HighByte, RegDE.LowByte, 0];
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x84: // ADD A, H
                            RegAF.LowWord = TableALU[0, RegAF.HighByte, RegHL.HighByte, 0];
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x85: // ADD A, L
                            RegAF.LowWord = TableALU[0, RegAF.HighByte, RegHL.LowByte, 0];
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x86: // ADD A, (HL)
                            RegAF.LowWord = TableALU[0, RegAF.HighByte, ReadMemory(RegHL.LowWord), 0];
                            totalExecutedCycles += 7; pendingCycles -= 7;
                            break;
                        case 0x87: // ADD A, A
                            RegAF.LowWord = TableALU[0, RegAF.HighByte, RegAF.HighByte, 0];
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x88: // ADC A, B
                            RegAF.LowWord = TableALU[1, RegAF.HighByte, RegBC.HighByte, RegFlagC ? 1 : 0];
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x89: // ADC A, C
                            RegAF.LowWord = TableALU[1, RegAF.HighByte, RegBC.LowByte, RegFlagC ? 1 : 0];
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x8A: // ADC A, D
                            RegAF.LowWord = TableALU[1, RegAF.HighByte, RegDE.HighByte, RegFlagC ? 1 : 0];
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x8B: // ADC A, E
                            RegAF.LowWord = TableALU[1, RegAF.HighByte, RegDE.LowByte, RegFlagC ? 1 : 0];
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x8C: // ADC A, H
                            RegAF.LowWord = TableALU[1, RegAF.HighByte, RegHL.HighByte, RegFlagC ? 1 : 0];
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x8D: // ADC A, L
                            RegAF.LowWord = TableALU[1, RegAF.HighByte, RegHL.LowByte, RegFlagC ? 1 : 0];
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x8E: // ADC A, (HL)
                            RegAF.LowWord = TableALU[1, RegAF.HighByte, ReadMemory(RegHL.LowWord), RegFlagC ? 1 : 0];
                            totalExecutedCycles += 7; pendingCycles -= 7;
                            break;
                        case 0x8F: // ADC A, A
                            RegAF.LowWord = TableALU[1, RegAF.HighByte, RegAF.HighByte, RegFlagC ? 1 : 0];
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x90: // SUB B
                            RegAF.LowWord = TableALU[2, RegAF.HighByte, RegBC.HighByte, 0];
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x91: // SUB C
                            RegAF.LowWord = TableALU[2, RegAF.HighByte, RegBC.LowByte, 0];
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x92: // SUB D
                            RegAF.LowWord = TableALU[2, RegAF.HighByte, RegDE.HighByte, 0];
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x93: // SUB E
                            RegAF.LowWord = TableALU[2, RegAF.HighByte, RegDE.LowByte, 0];
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x94: // SUB H
                            RegAF.LowWord = TableALU[2, RegAF.HighByte, RegHL.HighByte, 0];
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x95: // SUB L
                            RegAF.LowWord = TableALU[2, RegAF.HighByte, RegHL.LowByte, 0];
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x96: // SUB (HL)
                            RegAF.LowWord = TableALU[2, RegAF.HighByte, ReadMemory(RegHL.LowWord), 0];
                            totalExecutedCycles += 7; pendingCycles -= 7;
                            break;
                        case 0x97: // SUB A, A
                            RegAF.LowWord = TableALU[2, RegAF.HighByte, RegAF.HighByte, 0];
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x98: // SBC A, B
                            RegAF.LowWord = TableALU[3, RegAF.HighByte, RegBC.HighByte, RegFlagC ? 1 : 0];
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x99: // SBC A, C
                            RegAF.LowWord = TableALU[3, RegAF.HighByte, RegBC.LowByte, RegFlagC ? 1 : 0];
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x9A: // SBC A, D
                            RegAF.LowWord = TableALU[3, RegAF.HighByte, RegDE.HighByte, RegFlagC ? 1 : 0];
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x9B: // SBC A, E
                            RegAF.LowWord = TableALU[3, RegAF.HighByte, RegDE.LowByte, RegFlagC ? 1 : 0];
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x9C: // SBC A, H
                            RegAF.LowWord = TableALU[3, RegAF.HighByte, RegHL.HighByte, RegFlagC ? 1 : 0];
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x9D: // SBC A, L
                            RegAF.LowWord = TableALU[3, RegAF.HighByte, RegHL.LowByte, RegFlagC ? 1 : 0];
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0x9E: // SBC A, (HL)
                            RegAF.LowWord = TableALU[3, RegAF.HighByte, ReadMemory(RegHL.LowWord), RegFlagC ? 1 : 0];
                            totalExecutedCycles += 7; pendingCycles -= 7;
                            break;
                        case 0x9F: // SBC A, A
                            RegAF.LowWord = TableALU[3, RegAF.HighByte, RegAF.HighByte, RegFlagC ? 1 : 0];
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0xA0: // AND B
                            RegAF.LowWord = TableALU[4, RegAF.HighByte, RegBC.HighByte, 0];
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0xA1: // AND C
                            RegAF.LowWord = TableALU[4, RegAF.HighByte, RegBC.LowByte, 0];
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0xA2: // AND D
                            RegAF.LowWord = TableALU[4, RegAF.HighByte, RegDE.HighByte, 0];
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0xA3: // AND E
                            RegAF.LowWord = TableALU[4, RegAF.HighByte, RegDE.LowByte, 0];
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0xA4: // AND H
                            RegAF.LowWord = TableALU[4, RegAF.HighByte, RegHL.HighByte, 0];
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0xA5: // AND L
                            RegAF.LowWord = TableALU[4, RegAF.HighByte, RegHL.LowByte, 0];
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0xA6: // AND (HL)
                            RegAF.LowWord = TableALU[4, RegAF.HighByte, ReadMemory(RegHL.LowWord), 0];
                            totalExecutedCycles += 7; pendingCycles -= 7;
                            break;
                        case 0xA7: // AND A
                            RegAF.LowWord = TableALU[4, RegAF.HighByte, RegAF.HighByte, 0];
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0xA8: // XOR B
                            RegAF.LowWord = TableALU[5, RegAF.HighByte, RegBC.HighByte, 0];
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0xA9: // XOR C
                            RegAF.LowWord = TableALU[5, RegAF.HighByte, RegBC.LowByte, 0];
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0xAA: // XOR D
                            RegAF.LowWord = TableALU[5, RegAF.HighByte, RegDE.HighByte, 0];
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0xAB: // XOR E
                            RegAF.LowWord = TableALU[5, RegAF.HighByte, RegDE.LowByte, 0];
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0xAC: // XOR H
                            RegAF.LowWord = TableALU[5, RegAF.HighByte, RegHL.HighByte, 0];
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0xAD: // XOR L
                            RegAF.LowWord = TableALU[5, RegAF.HighByte, RegHL.LowByte, 0];
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0xAE: // XOR (HL)
                            RegAF.LowWord = TableALU[5, RegAF.HighByte, ReadMemory(RegHL.LowWord), 0];
                            totalExecutedCycles += 7; pendingCycles -= 7;
                            break;
                        case 0xAF: // XOR A
                            RegAF.LowWord = TableALU[5, RegAF.HighByte, RegAF.HighByte, 0];
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0xB0: // OR B
                            RegAF.LowWord = TableALU[6, RegAF.HighByte, RegBC.HighByte, 0];
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0xB1: // OR C
                            RegAF.LowWord = TableALU[6, RegAF.HighByte, RegBC.LowByte, 0];
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0xB2: // OR D
                            RegAF.LowWord = TableALU[6, RegAF.HighByte, RegDE.HighByte, 0];
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0xB3: // OR E
                            RegAF.LowWord = TableALU[6, RegAF.HighByte, RegDE.LowByte, 0];
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0xB4: // OR H
                            RegAF.LowWord = TableALU[6, RegAF.HighByte, RegHL.HighByte, 0];
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0xB5: // OR L
                            RegAF.LowWord = TableALU[6, RegAF.HighByte, RegHL.LowByte, 0];
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0xB6: // OR (HL)
                            RegAF.LowWord = TableALU[6, RegAF.HighByte, ReadMemory(RegHL.LowWord), 0];
                            totalExecutedCycles += 7; pendingCycles -= 7;
                            break;
                        case 0xB7: // OR A
                            RegAF.LowWord = TableALU[6, RegAF.HighByte, RegAF.HighByte, 0];
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0xB8: // CP B
                            RegAF.LowWord = TableALU[7, RegAF.HighByte, RegBC.HighByte, 0];
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0xB9: // CP C
                            RegAF.LowWord = TableALU[7, RegAF.HighByte, RegBC.LowByte, 0];
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0xBA: // CP D
                            RegAF.LowWord = TableALU[7, RegAF.HighByte, RegDE.HighByte, 0];
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0xBB: // CP E
                            RegAF.LowWord = TableALU[7, RegAF.HighByte, RegDE.LowByte, 0];
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0xBC: // CP H
                            RegAF.LowWord = TableALU[7, RegAF.HighByte, RegHL.HighByte, 0];
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0xBD: // CP L
                            RegAF.LowWord = TableALU[7, RegAF.HighByte, RegHL.LowByte, 0];
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0xBE: // CP (HL)
                            RegAF.LowWord = TableALU[7, RegAF.HighByte, ReadMemory(RegHL.LowWord), 0];
                            totalExecutedCycles += 7; pendingCycles -= 7;
                            break;
                        case 0xBF: // CP A
                            RegAF.LowWord = TableALU[7, RegAF.HighByte, RegAF.HighByte, 0];
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0xC0: // RET NZ
                            if (!RegFlagZ)
                            {
                                RegPC.LowByte = ReadMemory(RegSP.LowWord++); RegPC.HighByte = ReadMemory(RegSP.LowWord++);
                                RegWZ.LowWord = RegPC.LowWord;
                                totalExecutedCycles += 11; pendingCycles -= 11;
                            }
                            else
                            {
                                totalExecutedCycles += 5; pendingCycles -= 5;
                            }
                            break;
                        case 0xC1: // POP BC
                            RegBC.LowByte = ReadMemory(RegSP.LowWord++); RegBC.HighByte = ReadMemory(RegSP.LowWord++);
                            totalExecutedCycles += 10; pendingCycles -= 10;
                            break;
                        case 0xC2: // JP NZ, nn
                            RegWZ.LowWord = (ushort)(ReadOpArg(RegPC.LowWord++) + ReadOpArg(RegPC.LowWord++) * 256);
                            if (!RegFlagZ)
                            {
                                RegPC.LowWord = RegWZ.LowWord;
                            }
                            totalExecutedCycles += 10; pendingCycles -= 10;
                            break;
                        case 0xC3: // JP nn
                            RegPC.LowWord = (ushort)(ReadOpArg(RegPC.LowWord++) + ReadOpArg(RegPC.LowWord++) * 256);
                            RegWZ.LowWord = RegPC.LowWord;
                            totalExecutedCycles += 10; pendingCycles -= 10;
                            break;
                        case 0xC4: // CALL NZ, nn
                            RegWZ.LowWord = (ushort)(ReadOpArg(RegPC.LowWord++) + ReadOpArg(RegPC.LowWord++) * 256);
                            if (!RegFlagZ)
                            {
                                totalExecutedCycles += 17; pendingCycles -= 17;
                                WriteMemory(--RegSP.LowWord, RegPC.HighByte); WriteMemory(--RegSP.LowWord, RegPC.LowByte);
                                RegPC.LowWord = RegWZ.LowWord;
                            }
                            else
                            {
                                totalExecutedCycles += 10; pendingCycles -= 10;
                            }
                            break;
                        case 0xC5: // PUSH BC
                            WriteMemory(--RegSP.LowWord, RegBC.HighByte); WriteMemory(--RegSP.LowWord, RegBC.LowByte);
                            totalExecutedCycles += 11; pendingCycles -= 11;
                            break;
                        case 0xC6: // ADD A, n
                            RegAF.LowWord = TableALU[0, RegAF.HighByte, ReadOpArg(RegPC.LowWord++), 0];
                            totalExecutedCycles += 7; pendingCycles -= 7;
                            break;
                        case 0xC7: // RST $00
                            WriteMemory(--RegSP.LowWord, RegPC.HighByte); WriteMemory(--RegSP.LowWord, RegPC.LowByte);
                            RegPC.LowWord = 0x00;
                            totalExecutedCycles += 11; pendingCycles -= 11;
                            break;
                        case 0xC8: // RET Z
                            if (RegFlagZ)
                            {
                                RegPC.LowByte = ReadMemory(RegSP.LowWord++); RegPC.HighByte = ReadMemory(RegSP.LowWord++);
                                RegWZ.LowWord = RegPC.LowWord;
                                totalExecutedCycles += 11; pendingCycles -= 11;
                            }
                            else
                            {
                                totalExecutedCycles += 5; pendingCycles -= 5;
                            }
                            break;
                        case 0xC9: // RET
                            RegPC.LowByte = ReadMemory(RegSP.LowWord++); RegPC.HighByte = ReadMemory(RegSP.LowWord++);
                            RegWZ.LowWord = RegPC.LowWord;
                            totalExecutedCycles += 10; pendingCycles -= 10;
                            break;
                        case 0xCA: // JP Z, nn
                            RegWZ.LowWord = (ushort)(ReadOpArg(RegPC.LowWord++) + ReadOpArg(RegPC.LowWord++) * 256);
                            if (RegFlagZ)
                            {
                                RegPC.LowWord = RegWZ.LowWord;
                            }
                            totalExecutedCycles += 10; pendingCycles -= 10;
                            break;
                        case 0xCB: // (Prefix)
                            ++RegR;
                            switch (ReadOp(RegPC.LowWord++))
                            {
                                case 0x00: // RLC B
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    TUS = TableRotShift[1, 0, RegAF.LowByte + 256 * RegBC.HighByte];
                                    RegBC.HighByte = (byte)(TUS >> 8);
                                    RegAF.LowByte = (byte)TUS;
                                    break;
                                case 0x01: // RLC C
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    TUS = TableRotShift[1, 0, RegAF.LowByte + 256 * RegBC.LowByte];
                                    RegBC.LowByte = (byte)(TUS >> 8);
                                    RegAF.LowByte = (byte)TUS;
                                    break;
                                case 0x02: // RLC D
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    TUS = TableRotShift[1, 0, RegAF.LowByte + 256 * RegDE.HighByte];
                                    RegDE.HighByte = (byte)(TUS >> 8);
                                    RegAF.LowByte = (byte)TUS;
                                    break;
                                case 0x03: // RLC E
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    TUS = TableRotShift[1, 0, RegAF.LowByte + 256 * RegDE.LowByte];
                                    RegDE.LowByte = (byte)(TUS >> 8);
                                    RegAF.LowByte = (byte)TUS;
                                    break;
                                case 0x04: // RLC H
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    TUS = TableRotShift[1, 0, RegAF.LowByte + 256 * RegHL.HighByte];
                                    RegHL.HighByte = (byte)(TUS >> 8);
                                    RegAF.LowByte = (byte)TUS;
                                    break;
                                case 0x05: // RLC L
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    TUS = TableRotShift[1, 0, RegAF.LowByte + 256 * RegHL.LowByte];
                                    RegHL.LowByte = (byte)(TUS >> 8);
                                    RegAF.LowByte = (byte)TUS;
                                    break;
                                case 0x06: // RLC (HL)
                                    totalExecutedCycles += 15; pendingCycles -= 15;
                                    TUS = TableRotShift[1, 0, RegAF.LowByte + 256 * ReadMemory(RegHL.LowWord)];
                                    WriteMemory(RegHL.LowWord, (byte)(TUS >> 8));
                                    RegAF.LowByte = (byte)TUS;
                                    break;
                                case 0x07: // RLC A
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    TUS = TableRotShift[1, 0, RegAF.LowByte + 256 * RegAF.HighByte];
                                    RegAF.HighByte = (byte)(TUS >> 8);
                                    RegAF.LowByte = (byte)TUS;                                    
                                    break;
                                case 0x08: // RRC B
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    TUS = TableRotShift[1, 1, RegAF.LowByte + 256 * RegBC.HighByte];
                                    RegBC.HighByte = (byte)(TUS >> 8);
                                    RegAF.LowByte = (byte)TUS;                                    
                                    break;
                                case 0x09: // RRC C
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    TUS = TableRotShift[1, 1, RegAF.LowByte + 256 * RegBC.LowByte];
                                    RegBC.LowByte = (byte)(TUS >> 8);
                                    RegAF.LowByte = (byte)TUS;                                    
                                    break;
                                case 0x0A: // RRC D
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    TUS = TableRotShift[1, 1, RegAF.LowByte + 256 * RegDE.HighByte];
                                    RegDE.HighByte = (byte)(TUS >> 8);
                                    RegAF.LowByte = (byte)TUS;                                    
                                    break;
                                case 0x0B: // RRC E
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    TUS = TableRotShift[1, 1, RegAF.LowByte + 256 * RegDE.LowByte];
                                    RegDE.LowByte = (byte)(TUS >> 8);
                                    RegAF.LowByte = (byte)TUS;                                    
                                    break;
                                case 0x0C: // RRC H
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    TUS = TableRotShift[1, 1, RegAF.LowByte + 256 * RegHL.HighByte];
                                    RegHL.HighByte = (byte)(TUS >> 8);
                                    RegAF.LowByte = (byte)TUS;                                    
                                    break;
                                case 0x0D: // RRC L
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    TUS = TableRotShift[1, 1, RegAF.LowByte + 256 * RegHL.LowByte];
                                    RegHL.LowByte = (byte)(TUS >> 8);
                                    RegAF.LowByte = (byte)TUS;                                    
                                    break;
                                case 0x0E: // RRC (HL)
                                    totalExecutedCycles += 15; pendingCycles -= 15;
                                    TUS = TableRotShift[1, 1, RegAF.LowByte + 256 * ReadMemory(RegHL.LowWord)];
                                    WriteMemory(RegHL.LowWord, (byte)(TUS >> 8));
                                    RegAF.LowByte = (byte)TUS;
                                    break;
                                case 0x0F: // RRC A
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    TUS = TableRotShift[1, 1, RegAF.LowByte + 256 * RegAF.HighByte];
                                    RegAF.HighByte = (byte)(TUS >> 8);
                                    RegAF.LowByte = (byte)TUS;                                    
                                    break;
                                case 0x10: // RL B
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    TUS = TableRotShift[1, 2, RegAF.LowByte + 256 * RegBC.HighByte];
                                    RegBC.HighByte = (byte)(TUS >> 8);
                                    RegAF.LowByte = (byte)TUS;                                    
                                    break;
                                case 0x11: // RL C
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    TUS = TableRotShift[1, 2, RegAF.LowByte + 256 * RegBC.LowByte];
                                    RegBC.LowByte = (byte)(TUS >> 8);
                                    RegAF.LowByte = (byte)TUS;                                    
                                    break;
                                case 0x12: // RL D
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    TUS = TableRotShift[1, 2, RegAF.LowByte + 256 * RegDE.HighByte];
                                    RegDE.HighByte = (byte)(TUS >> 8);
                                    RegAF.LowByte = (byte)TUS;                                    
                                    break;
                                case 0x13: // RL E
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    TUS = TableRotShift[1, 2, RegAF.LowByte + 256 * RegDE.LowByte];
                                    RegDE.LowByte = (byte)(TUS >> 8);
                                    RegAF.LowByte = (byte)TUS;                                    
                                    break;
                                case 0x14: // RL H
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    TUS = TableRotShift[1, 2, RegAF.LowByte + 256 * RegHL.HighByte];
                                    RegHL.HighByte = (byte)(TUS >> 8);
                                    RegAF.LowByte = (byte)TUS;
                                    break;
                                case 0x15: // RL L
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    TUS = TableRotShift[1, 2, RegAF.LowByte + 256 * RegHL.LowByte];
                                    RegHL.LowByte = (byte)(TUS >> 8);
                                    RegAF.LowByte = (byte)TUS;
                                    break;
                                case 0x16: // RL (HL)
                                    totalExecutedCycles += 15; pendingCycles -= 15;
                                    TUS = TableRotShift[1, 2, RegAF.LowByte + 256 * ReadMemory(RegHL.LowWord)];
                                    WriteMemory(RegHL.LowWord, (byte)(TUS >> 8));
                                    RegAF.LowByte = (byte)TUS;
                                    break;
                                case 0x17: // RL A
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    TUS = TableRotShift[1, 2, RegAF.LowByte + 256 * RegAF.HighByte];
                                    RegAF.HighByte = (byte)(TUS >> 8);
                                    RegAF.LowByte = (byte)TUS;
                                    break;
                                case 0x18: // RR B
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    TUS = TableRotShift[1, 3, RegAF.LowByte + 256 * RegBC.HighByte];
                                    RegBC.HighByte = (byte)(TUS >> 8);
                                    RegAF.LowByte = (byte)TUS;
                                    break;
                                case 0x19: // RR C
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    TUS = TableRotShift[1, 3, RegAF.LowByte + 256 * RegBC.LowByte];
                                    RegBC.LowByte = (byte)(TUS >> 8);
                                    RegAF.LowByte = (byte)TUS;
                                    break;
                                case 0x1A: // RR D
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    TUS = TableRotShift[1, 3, RegAF.LowByte + 256 * RegDE.HighByte];
                                    RegDE.HighByte = (byte)(TUS >> 8);
                                    RegAF.LowByte = (byte)TUS;
                                    break;
                                case 0x1B: // RR E
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    TUS = TableRotShift[1, 3, RegAF.LowByte + 256 * RegDE.LowByte];
                                    RegDE.LowByte = (byte)(TUS >> 8);
                                    RegAF.LowByte = (byte)TUS;
                                    break;
                                case 0x1C: // RR H
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    TUS = TableRotShift[1, 3, RegAF.LowByte + 256 * RegHL.HighByte];
                                    RegHL.HighByte = (byte)(TUS >> 8);
                                    RegAF.LowByte = (byte)TUS;
                                    break;
                                case 0x1D: // RR L
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    TUS = TableRotShift[1, 3, RegAF.LowByte + 256 * RegHL.LowByte];
                                    RegHL.LowByte = (byte)(TUS >> 8);
                                    RegAF.LowByte = (byte)TUS;
                                    break;
                                case 0x1E: // RR (HL)
                                    totalExecutedCycles += 15; pendingCycles -= 15;
                                    TUS = TableRotShift[1, 3, RegAF.LowByte + 256 * ReadMemory(RegHL.LowWord)];
                                    WriteMemory(RegHL.LowWord, (byte)(TUS >> 8));
                                    RegAF.LowByte = (byte)TUS;
                                    break;
                                case 0x1F: // RR A
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    TUS = TableRotShift[1, 3, RegAF.LowByte + 256 * RegAF.HighByte];
                                    RegAF.HighByte = (byte)(TUS >> 8);
                                    RegAF.LowByte = (byte)TUS;
                                    break;
                                case 0x20: // SLA B
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    TUS = TableRotShift[1, 4, RegAF.LowByte + 256 * RegBC.HighByte];
                                    RegBC.HighByte = (byte)(TUS >> 8);
                                    RegAF.LowByte = (byte)TUS;
                                    break;
                                case 0x21: // SLA C
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    TUS = TableRotShift[1, 4, RegAF.LowByte + 256 * RegBC.LowByte];
                                    RegBC.LowByte = (byte)(TUS >> 8);
                                    RegAF.LowByte = (byte)TUS;
                                    break;
                                case 0x22: // SLA D
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    TUS = TableRotShift[1, 4, RegAF.LowByte + 256 * RegDE.HighByte];
                                    RegDE.HighByte = (byte)(TUS >> 8);
                                    RegAF.LowByte = (byte)TUS;
                                    break;
                                case 0x23: // SLA E
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    TUS = TableRotShift[1, 4, RegAF.LowByte + 256 * RegDE.LowByte];
                                    RegDE.LowByte = (byte)(TUS >> 8);
                                    RegAF.LowByte = (byte)TUS;
                                    break;
                                case 0x24: // SLA H
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    TUS = TableRotShift[1, 4, RegAF.LowByte + 256 * RegHL.HighByte];
                                    RegHL.HighByte = (byte)(TUS >> 8);
                                    RegAF.LowByte = (byte)TUS;
                                    break;
                                case 0x25: // SLA L
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    TUS = TableRotShift[1, 4, RegAF.LowByte + 256 * RegHL.LowByte];
                                    RegHL.LowByte = (byte)(TUS >> 8);
                                    RegAF.LowByte = (byte)TUS;
                                    break;
                                case 0x26: // SLA (HL)
                                    totalExecutedCycles += 15; pendingCycles -= 15;
                                    TUS = TableRotShift[1, 4, RegAF.LowByte + 256 * ReadMemory(RegHL.LowWord)];
                                    WriteMemory(RegHL.LowWord, (byte)(TUS >> 8));
                                    RegAF.LowByte = (byte)TUS;
                                    break;
                                case 0x27: // SLA A
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    TUS = TableRotShift[1, 4, RegAF.LowByte + 256 * RegAF.HighByte];
                                    RegAF.HighByte = (byte)(TUS >> 8);
                                    RegAF.LowByte = (byte)TUS;
                                    break;
                                case 0x28: // SRA B
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    TUS = TableRotShift[1, 5, RegAF.LowByte + 256 * RegBC.HighByte];
                                    RegBC.HighByte = (byte)(TUS >> 8);
                                    RegAF.LowByte = (byte)TUS;
                                    break;
                                case 0x29: // SRA C
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    TUS = TableRotShift[1, 5, RegAF.LowByte + 256 * RegBC.LowByte];
                                    RegBC.LowByte = (byte)(TUS >> 8);
                                    RegAF.LowByte = (byte)TUS;
                                    break;
                                case 0x2A: // SRA D
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    TUS = TableRotShift[1, 5, RegAF.LowByte + 256 * RegDE.HighByte];
                                    RegDE.HighByte = (byte)(TUS >> 8);
                                    RegAF.LowByte = (byte)TUS;
                                    break;
                                case 0x2B: // SRA E
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    TUS = TableRotShift[1, 5, RegAF.LowByte + 256 * RegDE.LowByte];
                                    RegDE.LowByte = (byte)(TUS >> 8);
                                    RegAF.LowByte = (byte)TUS;
                                    break;
                                case 0x2C: // SRA H
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    TUS = TableRotShift[1, 5, RegAF.LowByte + 256 * RegHL.HighByte];
                                    RegHL.HighByte = (byte)(TUS >> 8);
                                    RegAF.LowByte = (byte)TUS;
                                    break;
                                case 0x2D: // SRA L
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    TUS = TableRotShift[1, 5, RegAF.LowByte + 256 * RegHL.LowByte];
                                    RegHL.LowByte = (byte)(TUS >> 8);
                                    RegAF.LowByte = (byte)TUS;
                                    break;
                                case 0x2E: // SRA (HL)
                                    totalExecutedCycles += 15; pendingCycles -= 15;
                                    TUS = TableRotShift[1, 5, RegAF.LowByte + 256 * ReadMemory(RegHL.LowWord)];
                                    WriteMemory(RegHL.LowWord, (byte)(TUS >> 8));
                                    RegAF.LowByte = (byte)TUS;
                                    break;
                                case 0x2F: // SRA A
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    TUS = TableRotShift[1, 5, RegAF.LowByte + 256 * RegAF.HighByte];
                                    RegAF.HighByte = (byte)(TUS >> 8);
                                    RegAF.LowByte = (byte)TUS;
                                    break;
                                case 0x30: // SL1 B
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    TUS = TableRotShift[1, 6, RegAF.LowByte + 256 * RegBC.HighByte];
                                    RegBC.HighByte = (byte)(TUS >> 8);
                                    RegAF.LowByte = (byte)TUS;
                                    break;
                                case 0x31: // SL1 C
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    TUS = TableRotShift[1, 6, RegAF.LowByte + 256 * RegBC.LowByte];
                                    RegBC.LowByte = (byte)(TUS >> 8);
                                    RegAF.LowByte = (byte)TUS;
                                    break;
                                case 0x32: // SL1 D
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    TUS = TableRotShift[1, 6, RegAF.LowByte + 256 * RegDE.HighByte];
                                    RegDE.HighByte = (byte)(TUS >> 8);
                                    RegAF.LowByte = (byte)TUS;
                                    break;
                                case 0x33: // SL1 E
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    TUS = TableRotShift[1, 6, RegAF.LowByte + 256 * RegDE.LowByte];
                                    RegDE.LowByte = (byte)(TUS >> 8);
                                    RegAF.LowByte = (byte)TUS;
                                    break;
                                case 0x34: // SL1 H
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    TUS = TableRotShift[1, 6, RegAF.LowByte + 256 * RegHL.HighByte];
                                    RegHL.HighByte = (byte)(TUS >> 8);
                                    RegAF.LowByte = (byte)TUS;
                                    break;
                                case 0x35: // SL1 L
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    TUS = TableRotShift[1, 6, RegAF.LowByte + 256 * RegHL.LowByte];
                                    RegHL.LowByte = (byte)(TUS >> 8);
                                    RegAF.LowByte = (byte)TUS;
                                    break;
                                case 0x36: // SL1 (HL)
                                    totalExecutedCycles += 15; pendingCycles -= 15;
                                    TUS = TableRotShift[1, 6, RegAF.LowByte + 256 * ReadMemory(RegHL.LowWord)];
                                    WriteMemory(RegHL.LowWord, (byte)(TUS >> 8));
                                    RegAF.LowByte = (byte)TUS;
                                    break;
                                case 0x37: // SL1 A
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    TUS = TableRotShift[1, 6, RegAF.LowByte + 256 * RegAF.HighByte];
                                    RegAF.HighByte = (byte)(TUS >> 8);
                                    RegAF.LowByte = (byte)TUS;
                                    break;
                                case 0x38: // SRL B
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    TUS = TableRotShift[1, 7, RegAF.LowByte + 256 * RegBC.HighByte];
                                    RegBC.HighByte = (byte)(TUS >> 8);
                                    RegAF.LowByte = (byte)TUS;
                                    break;
                                case 0x39: // SRL C
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    TUS = TableRotShift[1, 7, RegAF.LowByte + 256 * RegBC.LowByte];
                                    RegBC.LowByte = (byte)(TUS >> 8);
                                    RegAF.LowByte = (byte)TUS;
                                    break;
                                case 0x3A: // SRL D
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    TUS = TableRotShift[1, 7, RegAF.LowByte + 256 * RegDE.HighByte];
                                    RegDE.HighByte = (byte)(TUS >> 8);
                                    RegAF.LowByte = (byte)TUS;
                                    break;
                                case 0x3B: // SRL E
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    TUS = TableRotShift[1, 7, RegAF.LowByte + 256 * RegDE.LowByte];
                                    RegDE.LowByte = (byte)(TUS >> 8);
                                    RegAF.LowByte = (byte)TUS;
                                    break;
                                case 0x3C: // SRL H
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    TUS = TableRotShift[1, 7, RegAF.LowByte + 256 * RegHL.HighByte];
                                    RegHL.HighByte = (byte)(TUS >> 8);
                                    RegAF.LowByte = (byte)TUS;
                                    break;
                                case 0x3D: // SRL L
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    TUS = TableRotShift[1, 7, RegAF.LowByte + 256 * RegHL.LowByte];
                                    RegHL.LowByte = (byte)(TUS >> 8);
                                    RegAF.LowByte = (byte)TUS;
                                    break;
                                case 0x3E: // SRL (HL)
                                    totalExecutedCycles += 15; pendingCycles -= 15;
                                    TUS = TableRotShift[1, 7, RegAF.LowByte + 256 * ReadMemory(RegHL.LowWord)];
                                    WriteMemory(RegHL.LowWord, (byte)(TUS >> 8));
                                    RegAF.LowByte = (byte)TUS;
                                    break;
                                case 0x3F: // SRL A
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    TUS = TableRotShift[1, 7, RegAF.LowByte + 256 * RegAF.HighByte];
                                    RegAF.HighByte = (byte)(TUS >> 8);
                                    RegAF.LowByte = (byte)TUS;
                                    break;
                                case 0x40: // BIT 0, B
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegFlagZ = (RegBC.HighByte & 0x01) == 0;
                                    RegFlagP = RegFlagZ;
                                    RegFlagS = false;
                                    RegFlag3 = false;
                                    RegFlag5 = false;
                                    RegFlagH = true;
                                    RegFlagN = false;
                                    break;
                                case 0x41: // BIT 0, C
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegFlagZ = (RegBC.LowByte & 0x01) == 0;
                                    RegFlagP = RegFlagZ;
                                    RegFlagS = false;
                                    RegFlag3 = false;
                                    RegFlag5 = false;
                                    RegFlagH = true;
                                    RegFlagN = false;
                                    break;
                                case 0x42: // BIT 0, D
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegFlagZ = (RegDE.HighByte & 0x01) == 0;
                                    RegFlagP = RegFlagZ;
                                    RegFlagS = false;
                                    RegFlag3 = false;
                                    RegFlag5 = false;
                                    RegFlagH = true;
                                    RegFlagN = false;
                                    break;
                                case 0x43: // BIT 0, E
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegFlagZ = (RegDE.LowByte & 0x01) == 0;
                                    RegFlagP = RegFlagZ;
                                    RegFlagS = false;
                                    RegFlag3 = false;
                                    RegFlag5 = false;
                                    RegFlagH = true;
                                    RegFlagN = false;
                                    break;
                                case 0x44: // BIT 0, H
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegFlagZ = (RegHL.HighByte & 0x01) == 0;
                                    RegFlagP = RegFlagZ;
                                    RegFlagS = false;
                                    RegFlag3 = false;
                                    RegFlag5 = false;
                                    RegFlagH = true;
                                    RegFlagN = false;
                                    break;
                                case 0x45: // BIT 0, L
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegFlagZ = (RegHL.LowByte & 0x01) == 0;
                                    RegFlagP = RegFlagZ;
                                    RegFlagS = false;
                                    RegFlag3 = false;
                                    RegFlag5 = false;
                                    RegFlagH = true;
                                    RegFlagN = false;
                                    break;
                                case 0x46: // BIT 0, (HL)
                                    totalExecutedCycles += 12; pendingCycles -= 12;
                                    RegFlagZ = (ReadMemory(RegHL.LowWord) & 0x01) == 0;
                                    RegFlagP = RegFlagZ;
                                    RegFlagS = false;
                                    RegFlag3 = (RegWZ.HighByte & 0x08) != 0;
                                    RegFlag5 = (RegWZ.HighByte & 0x20) != 0;
                                    RegFlagH = true;
                                    RegFlagN = false;
                                    break;
                                case 0x47: // BIT 0, A
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegFlagZ = (RegAF.HighByte & 0x01) == 0;
                                    RegFlagP = RegFlagZ;
                                    RegFlagS = false;
                                    RegFlag3 = false;
                                    RegFlag5 = false;
                                    RegFlagH = true;
                                    RegFlagN = false;
                                    break;
                                case 0x48: // BIT 1, B
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegFlagZ = (RegBC.HighByte & 0x02) == 0;
                                    RegFlagP = RegFlagZ;
                                    RegFlagS = false;
                                    RegFlag3 = false;
                                    RegFlag5 = false;
                                    RegFlagH = true;
                                    RegFlagN = false;
                                    break;
                                case 0x49: // BIT 1, C
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegFlagZ = (RegBC.LowByte & 0x02) == 0;
                                    RegFlagP = RegFlagZ;
                                    RegFlagS = false;
                                    RegFlag3 = false;
                                    RegFlag5 = false;
                                    RegFlagH = true;
                                    RegFlagN = false;
                                    break;
                                case 0x4A: // BIT 1, D
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegFlagZ = (RegDE.HighByte & 0x02) == 0;
                                    RegFlagP = RegFlagZ;
                                    RegFlagS = false;
                                    RegFlag3 = false;
                                    RegFlag5 = false;
                                    RegFlagH = true;
                                    RegFlagN = false;
                                    break;
                                case 0x4B: // BIT 1, E
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegFlagZ = (RegDE.LowByte & 0x02) == 0;
                                    RegFlagP = RegFlagZ;
                                    RegFlagS = false;
                                    RegFlag3 = false;
                                    RegFlag5 = false;
                                    RegFlagH = true;
                                    RegFlagN = false;
                                    break;
                                case 0x4C: // BIT 1, H
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegFlagZ = (RegHL.HighByte & 0x02) == 0;
                                    RegFlagP = RegFlagZ;
                                    RegFlagS = false;
                                    RegFlag3 = false;
                                    RegFlag5 = false;
                                    RegFlagH = true;
                                    RegFlagN = false;
                                    break;
                                case 0x4D: // BIT 1, L
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegFlagZ = (RegHL.LowByte & 0x02) == 0;
                                    RegFlagP = RegFlagZ;
                                    RegFlagS = false;
                                    RegFlag3 = false;
                                    RegFlag5 = false;
                                    RegFlagH = true;
                                    RegFlagN = false;
                                    break;
                                case 0x4E: // BIT 1, (HL)
                                    totalExecutedCycles += 12; pendingCycles -= 12;
                                    RegFlagZ = (ReadMemory(RegHL.LowWord) & 0x02) == 0;
                                    RegFlagP = RegFlagZ;
                                    RegFlagS = false;
                                    RegFlag3 = (RegWZ.HighByte & 0x08) != 0;
                                    RegFlag5 = (RegWZ.HighByte & 0x20) != 0;
                                    RegFlagH = true;
                                    RegFlagN = false;
                                    break;
                                case 0x4F: // BIT 1, A
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegFlagZ = (RegAF.HighByte & 0x02) == 0;
                                    RegFlagP = RegFlagZ;
                                    RegFlagS = false;
                                    RegFlag3 = false;
                                    RegFlag5 = false;
                                    RegFlagH = true;
                                    RegFlagN = false;
                                    break;
                                case 0x50: // BIT 2, B
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegFlagZ = (RegBC.HighByte & 0x04) == 0;
                                    RegFlagP = RegFlagZ;
                                    RegFlagS = false;
                                    RegFlag3 = false;
                                    RegFlag5 = false;
                                    RegFlagH = true;
                                    RegFlagN = false;
                                    break;
                                case 0x51: // BIT 2, C
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegFlagZ = (RegBC.LowByte & 0x04) == 0;
                                    RegFlagP = RegFlagZ;
                                    RegFlagS = false;
                                    RegFlag3 = false;
                                    RegFlag5 = false;
                                    RegFlagH = true;
                                    RegFlagN = false;
                                    break;
                                case 0x52: // BIT 2, D
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegFlagZ = (RegDE.HighByte & 0x04) == 0;
                                    RegFlagP = RegFlagZ;
                                    RegFlagS = false;
                                    RegFlag3 = false;
                                    RegFlag5 = false;
                                    RegFlagH = true;
                                    RegFlagN = false;
                                    break;
                                case 0x53: // BIT 2, E
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegFlagZ = (RegDE.LowByte & 0x04) == 0;
                                    RegFlagP = RegFlagZ;
                                    RegFlagS = false;
                                    RegFlag3 = false;
                                    RegFlag5 = false;
                                    RegFlagH = true;
                                    RegFlagN = false;
                                    break;
                                case 0x54: // BIT 2, H
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegFlagZ = (RegHL.HighByte & 0x04) == 0;
                                    RegFlagP = RegFlagZ;
                                    RegFlagS = false;
                                    RegFlag3 = false;
                                    RegFlag5 = false;
                                    RegFlagH = true;
                                    RegFlagN = false;
                                    break;
                                case 0x55: // BIT 2, L
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegFlagZ = (RegHL.LowByte & 0x04) == 0;
                                    RegFlagP = RegFlagZ;
                                    RegFlagS = false;
                                    RegFlag3 = false;
                                    RegFlag5 = false;
                                    RegFlagH = true;
                                    RegFlagN = false;
                                    break;
                                case 0x56: // BIT 2, (HL)
                                    totalExecutedCycles += 12; pendingCycles -= 12;
                                    RegFlagZ = (ReadMemory(RegHL.LowWord) & 0x04) == 0;
                                    RegFlagP = RegFlagZ;
                                    RegFlagS = false;
                                    RegFlag3 = (RegWZ.HighByte & 0x08) != 0;
                                    RegFlag5 = (RegWZ.HighByte & 0x20) != 0;
                                    RegFlagH = true;
                                    RegFlagN = false;
                                    break;
                                case 0x57: // BIT 2, A
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegFlagZ = (RegAF.HighByte & 0x04) == 0;
                                    RegFlagP = RegFlagZ;
                                    RegFlagS = false;
                                    RegFlag3 = false;
                                    RegFlag5 = false;
                                    RegFlagH = true;
                                    RegFlagN = false;
                                    break;
                                case 0x58: // BIT 3, B
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegFlagZ = (RegBC.HighByte & 0x08) == 0;
                                    RegFlagP = RegFlagZ;
                                    RegFlagS = false;
                                    RegFlag3 = !RegFlagZ;
                                    RegFlag5 = false;
                                    RegFlagH = true;
                                    RegFlagN = false;
                                    break;
                                case 0x59: // BIT 3, C
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegFlagZ = (RegBC.LowByte & 0x08) == 0;
                                    RegFlagP = RegFlagZ;
                                    RegFlagS = false;
                                    RegFlag3 = !RegFlagZ;
                                    RegFlag5 = false;
                                    RegFlagH = true;
                                    RegFlagN = false;
                                    break;
                                case 0x5A: // BIT 3, D
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegFlagZ = (RegDE.HighByte & 0x08) == 0;
                                    RegFlagP = RegFlagZ;
                                    RegFlagS = false;
                                    RegFlag3 = !RegFlagZ;
                                    RegFlag5 = false;
                                    RegFlagH = true;
                                    RegFlagN = false;
                                    break;
                                case 0x5B: // BIT 3, E
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegFlagZ = (RegDE.LowByte & 0x08) == 0;
                                    RegFlagP = RegFlagZ;
                                    RegFlagS = false;
                                    RegFlag3 = !RegFlagZ;
                                    RegFlag5 = false;
                                    RegFlagH = true;
                                    RegFlagN = false;
                                    break;
                                case 0x5C: // BIT 3, H
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegFlagZ = (RegHL.HighByte & 0x08) == 0;
                                    RegFlagP = RegFlagZ;
                                    RegFlagS = false;
                                    RegFlag3 = !RegFlagZ;
                                    RegFlag5 = false;
                                    RegFlagH = true;
                                    RegFlagN = false;
                                    break;
                                case 0x5D: // BIT 3, L
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegFlagZ = (RegHL.LowByte & 0x08) == 0;
                                    RegFlagP = RegFlagZ;
                                    RegFlagS = false;
                                    RegFlag3 = !RegFlagZ;
                                    RegFlag5 = false;
                                    RegFlagH = true;
                                    RegFlagN = false;
                                    break;
                                case 0x5E: // BIT 3, (HL)
                                    totalExecutedCycles += 12; pendingCycles -= 12;
                                    RegFlagZ = (ReadMemory(RegHL.LowWord) & 0x08) == 0;
                                    RegFlagP = RegFlagZ;
                                    RegFlagS = false;
                                    RegFlag3 = (RegWZ.HighByte & 0x08) != 0;
                                    RegFlag5 = (RegWZ.HighByte & 0x20) != 0;
                                    RegFlagH = true;
                                    RegFlagN = false;
                                    break;
                                case 0x5F: // BIT 3, A
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegFlagZ = (RegAF.HighByte & 0x08) == 0;
                                    RegFlagP = RegFlagZ;
                                    RegFlagS = false;
                                    RegFlag3 = !RegFlagZ;
                                    RegFlag5 = false;
                                    RegFlagH = true;
                                    RegFlagN = false;
                                    break;
                                case 0x60: // BIT 4, B
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegFlagZ = (RegBC.HighByte & 0x10) == 0;
                                    RegFlagP = RegFlagZ;
                                    RegFlagS = false;
                                    RegFlag3 = false;
                                    RegFlag5 = false;
                                    RegFlagH = true;
                                    RegFlagN = false;
                                    break;
                                case 0x61: // BIT 4, C
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegFlagZ = (RegBC.LowByte & 0x10) == 0;
                                    RegFlagP = RegFlagZ;
                                    RegFlagS = false;
                                    RegFlag3 = false;
                                    RegFlag5 = false;
                                    RegFlagH = true;
                                    RegFlagN = false;
                                    break;
                                case 0x62: // BIT 4, D
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegFlagZ = (RegDE.HighByte & 0x10) == 0;
                                    RegFlagP = RegFlagZ;
                                    RegFlagS = false;
                                    RegFlag3 = false;
                                    RegFlag5 = false;
                                    RegFlagH = true;
                                    RegFlagN = false;
                                    break;
                                case 0x63: // BIT 4, E
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegFlagZ = (RegDE.LowByte & 0x10) == 0;
                                    RegFlagP = RegFlagZ;
                                    RegFlagS = false;
                                    RegFlag3 = false;
                                    RegFlag5 = false;
                                    RegFlagH = true;
                                    RegFlagN = false;
                                    break;
                                case 0x64: // BIT 4, H
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegFlagZ = (RegHL.HighByte & 0x10) == 0;
                                    RegFlagP = RegFlagZ;
                                    RegFlagS = false;
                                    RegFlag3 = false;
                                    RegFlag5 = false;
                                    RegFlagH = true;
                                    RegFlagN = false;
                                    break;
                                case 0x65: // BIT 4, L
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegFlagZ = (RegHL.LowByte & 0x10) == 0;
                                    RegFlagP = RegFlagZ;
                                    RegFlagS = false;
                                    RegFlag3 = false;
                                    RegFlag5 = false;
                                    RegFlagH = true;
                                    RegFlagN = false;
                                    break;
                                case 0x66: // BIT 4, (HL)
                                    totalExecutedCycles += 12; pendingCycles -= 12;
                                    RegFlagZ = (ReadMemory(RegHL.LowWord) & 0x10) == 0;
                                    RegFlagP = RegFlagZ;
                                    RegFlagS = false;
                                    RegFlag3 = (RegWZ.HighByte & 0x08) != 0;
                                    RegFlag5 = (RegWZ.HighByte & 0x20) != 0;
                                    RegFlagH = true;
                                    RegFlagN = false;
                                    break;
                                case 0x67: // BIT 4, A
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegFlagZ = (RegAF.HighByte & 0x10) == 0;
                                    RegFlagP = RegFlagZ;
                                    RegFlagS = false;
                                    RegFlag3 = false;
                                    RegFlag5 = false;
                                    RegFlagH = true;
                                    RegFlagN = false;
                                    break;
                                case 0x68: // BIT 5, B
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegFlagZ = (RegBC.HighByte & 0x20) == 0;
                                    RegFlagP = RegFlagZ;
                                    RegFlagS = false;
                                    RegFlag3 = false;
                                    RegFlag5 = !RegFlagZ;
                                    RegFlagH = true;
                                    RegFlagN = false;
                                    break;
                                case 0x69: // BIT 5, C
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegFlagZ = (RegBC.LowByte & 0x20) == 0;
                                    RegFlagP = RegFlagZ;
                                    RegFlagS = false;
                                    RegFlag3 = false;
                                    RegFlag5 = !RegFlagZ;
                                    RegFlagH = true;
                                    RegFlagN = false;
                                    break;
                                case 0x6A: // BIT 5, D
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegFlagZ = (RegDE.HighByte & 0x20) == 0;
                                    RegFlagP = RegFlagZ;
                                    RegFlagS = false;
                                    RegFlag3 = false;
                                    RegFlag5 = !RegFlagZ;
                                    RegFlagH = true;
                                    RegFlagN = false;
                                    break;
                                case 0x6B: // BIT 5, E
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegFlagZ = (RegDE.LowByte & 0x20) == 0;
                                    RegFlagP = RegFlagZ;
                                    RegFlagS = false;
                                    RegFlag3 = false;
                                    RegFlag5 = !RegFlagZ;
                                    RegFlagH = true;
                                    RegFlagN = false;
                                    break;
                                case 0x6C: // BIT 5, H
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegFlagZ = (RegHL.HighByte & 0x20) == 0;
                                    RegFlagP = RegFlagZ;
                                    RegFlagS = false;
                                    RegFlag3 = false;
                                    RegFlag5 = !RegFlagZ;
                                    RegFlagH = true;
                                    RegFlagN = false;
                                    break;
                                case 0x6D: // BIT 5, L
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegFlagZ = (RegHL.LowByte & 0x20) == 0;
                                    RegFlagP = RegFlagZ;
                                    RegFlagS = false;
                                    RegFlag3 = false;
                                    RegFlag5 = !RegFlagZ;
                                    RegFlagH = true;
                                    RegFlagN = false;
                                    break;
                                case 0x6E: // BIT 5, (HL)
                                    totalExecutedCycles += 12; pendingCycles -= 12;
                                    RegFlagZ = (ReadMemory(RegHL.LowWord) & 0x20) == 0;
                                    RegFlagP = RegFlagZ;
                                    RegFlagS = false;
                                    RegFlag3 = (RegWZ.HighByte & 0x08) != 0;
                                    RegFlag5 = (RegWZ.HighByte & 0x20) != 0;
                                    RegFlagH = true;
                                    RegFlagN = false;
                                    break;
                                case 0x6F: // BIT 5, A
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegFlagZ = (RegAF.HighByte & 0x20) == 0;
                                    RegFlagP = RegFlagZ;
                                    RegFlagS = false;
                                    RegFlag3 = false;
                                    RegFlag5 = !RegFlagZ;
                                    RegFlagH = true;
                                    RegFlagN = false;
                                    break;
                                case 0x70: // BIT 6, B
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegFlagZ = (RegBC.HighByte & 0x40) == 0;
                                    RegFlagP = RegFlagZ;
                                    RegFlagS = false;
                                    RegFlag3 = false;
                                    RegFlag5 = false;
                                    RegFlagH = true;
                                    RegFlagN = false;
                                    break;
                                case 0x71: // BIT 6, C
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegFlagZ = (RegBC.LowByte & 0x40) == 0;
                                    RegFlagP = RegFlagZ;
                                    RegFlagS = false;
                                    RegFlag3 = false;
                                    RegFlag5 = false;
                                    RegFlagH = true;
                                    RegFlagN = false;
                                    break;
                                case 0x72: // BIT 6, D
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegFlagZ = (RegDE.HighByte & 0x40) == 0;
                                    RegFlagP = RegFlagZ;
                                    RegFlagS = false;
                                    RegFlag3 = false;
                                    RegFlag5 = false;
                                    RegFlagH = true;
                                    RegFlagN = false;
                                    break;
                                case 0x73: // BIT 6, E
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegFlagZ = (RegDE.LowByte & 0x40) == 0;
                                    RegFlagP = RegFlagZ;
                                    RegFlagS = false;
                                    RegFlag3 = false;
                                    RegFlag5 = false;
                                    RegFlagH = true;
                                    RegFlagN = false;
                                    break;
                                case 0x74: // BIT 6, H
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegFlagZ = (RegHL.HighByte & 0x40) == 0;
                                    RegFlagP = RegFlagZ;
                                    RegFlagS = false;
                                    RegFlag3 = false;
                                    RegFlag5 = false;
                                    RegFlagH = true;
                                    RegFlagN = false;
                                    break;
                                case 0x75: // BIT 6, L
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegFlagZ = (RegHL.LowByte & 0x40) == 0;
                                    RegFlagP = RegFlagZ;
                                    RegFlagS = false;
                                    RegFlag3 = false;
                                    RegFlag5 = false;
                                    RegFlagH = true;
                                    RegFlagN = false;
                                    break;
                                case 0x76: // BIT 6, (HL)
                                    totalExecutedCycles += 12; pendingCycles -= 12;
                                    RegFlagZ = (ReadMemory(RegHL.LowWord) & 0x40) == 0;
                                    RegFlagP = RegFlagZ;
                                    RegFlagS = false;
                                    RegFlag3 = (RegWZ.HighByte & 0x08) != 0;
                                    RegFlag5 = (RegWZ.HighByte & 0x20) != 0;
                                    RegFlagH = true;
                                    RegFlagN = false;
                                    break;
                                case 0x77: // BIT 6, A
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegFlagZ = (RegAF.HighByte & 0x40) == 0;
                                    RegFlagP = RegFlagZ;
                                    RegFlagS = false;
                                    RegFlag3 = false;
                                    RegFlag5 = false;
                                    RegFlagH = true;
                                    RegFlagN = false;
                                    break;
                                case 0x78: // BIT 7, B
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegFlagZ = (RegBC.HighByte & 0x80) == 0;
                                    RegFlagP = RegFlagZ;
                                    RegFlagS = !RegFlagZ;
                                    RegFlag3 = false;
                                    RegFlag5 = false;
                                    RegFlagH = true;
                                    RegFlagN = false;
                                    break;
                                case 0x79: // BIT 7, C
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegFlagZ = (RegBC.LowByte & 0x80) == 0;
                                    RegFlagP = RegFlagZ;
                                    RegFlagS = !RegFlagZ;
                                    RegFlag3 = false;
                                    RegFlag5 = false;
                                    RegFlagH = true;
                                    RegFlagN = false;
                                    break;
                                case 0x7A: // BIT 7, D
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegFlagZ = (RegDE.HighByte & 0x80) == 0;
                                    RegFlagP = RegFlagZ;
                                    RegFlagS = !RegFlagZ;
                                    RegFlag3 = false;
                                    RegFlag5 = false;
                                    RegFlagH = true;
                                    RegFlagN = false;
                                    break;
                                case 0x7B: // BIT 7, E
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegFlagZ = (RegDE.LowByte & 0x80) == 0;
                                    RegFlagP = RegFlagZ;
                                    RegFlagS = !RegFlagZ;
                                    RegFlag3 = false;
                                    RegFlag5 = false;
                                    RegFlagH = true;
                                    RegFlagN = false;
                                    break;
                                case 0x7C: // BIT 7, H
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegFlagZ = (RegHL.HighByte & 0x80) == 0;
                                    RegFlagP = RegFlagZ;
                                    RegFlagS = !RegFlagZ;
                                    RegFlag3 = false;
                                    RegFlag5 = false;
                                    RegFlagH = true;
                                    RegFlagN = false;
                                    break;
                                case 0x7D: // BIT 7, L
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegFlagZ = (RegHL.LowByte & 0x80) == 0;
                                    RegFlagP = RegFlagZ;
                                    RegFlagS = !RegFlagZ;
                                    RegFlag3 = false;
                                    RegFlag5 = false;
                                    RegFlagH = true;
                                    RegFlagN = false;
                                    break;
                                case 0x7E: // BIT 7, (HL)
                                    totalExecutedCycles += 12; pendingCycles -= 12;
                                    RegFlagZ = (ReadMemory(RegHL.LowWord) & 0x80) == 0;
                                    RegFlagP = RegFlagZ;
                                    RegFlagS = !RegFlagZ;
                                    RegFlag3 = (RegWZ.HighByte & 0x08) != 0;
                                    RegFlag5 = (RegWZ.HighByte & 0x20) != 0;
                                    RegFlagH = true;
                                    RegFlagN = false;
                                    break;
                                case 0x7F: // BIT 7, A
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegFlagZ = (RegAF.HighByte & 0x80) == 0;
                                    RegFlagP = RegFlagZ;
                                    RegFlagS = !RegFlagZ;
                                    RegFlag3 = false;
                                    RegFlag5 = false;
                                    RegFlagH = true;
                                    RegFlagN = false;
                                    break;
                                case 0x80: // RES 0, B
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegBC.HighByte &= unchecked((byte)~0x01);                                    
                                    break;
                                case 0x81: // RES 0, C
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegBC.LowByte &= unchecked((byte)~0x01);
                                    break;
                                case 0x82: // RES 0, D
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegDE.HighByte &= unchecked((byte)~0x01);
                                    break;
                                case 0x83: // RES 0, E
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegDE.LowByte &= unchecked((byte)~0x01);
                                    break;
                                case 0x84: // RES 0, H
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegHL.HighByte &= unchecked((byte)~0x01);
                                    break;
                                case 0x85: // RES 0, L
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegHL.LowByte &= unchecked((byte)~0x01);
                                    break;
                                case 0x86: // RES 0, (HL)
                                    totalExecutedCycles += 15; pendingCycles -= 15;
                                    WriteMemory(RegHL.LowWord, (byte)(ReadMemory(RegHL.LowWord) & unchecked((byte)~0x01)));
                                    break;
                                case 0x87: // RES 0, A
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegAF.HighByte &= unchecked((byte)~0x01);
                                    break;
                                case 0x88: // RES 1, B
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegBC.HighByte &= unchecked((byte)~0x02);
                                    break;
                                case 0x89: // RES 1, C
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegBC.LowByte &= unchecked((byte)~0x02);
                                    break;
                                case 0x8A: // RES 1, D
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegDE.HighByte &= unchecked((byte)~0x02);
                                    break;
                                case 0x8B: // RES 1, E
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegDE.LowByte &= unchecked((byte)~0x02);
                                    break;
                                case 0x8C: // RES 1, H
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegHL.HighByte &= unchecked((byte)~0x02);
                                    break;
                                case 0x8D: // RES 1, L
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegHL.LowByte &= unchecked((byte)~0x02);
                                    break;
                                case 0x8E: // RES 1, (HL)
                                    totalExecutedCycles += 15; pendingCycles -= 15;
                                    WriteMemory(RegHL.LowWord, (byte)(ReadMemory(RegHL.LowWord) & unchecked((byte)~0x02)));
                                    break;
                                case 0x8F: // RES 1, A
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegAF.HighByte &= unchecked((byte)~0x02);
                                    break;
                                case 0x90: // RES 2, B
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegBC.HighByte &= unchecked((byte)~0x04);
                                    break;
                                case 0x91: // RES 2, C
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegBC.LowByte &= unchecked((byte)~0x04);
                                    break;
                                case 0x92: // RES 2, D
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegDE.HighByte &= unchecked((byte)~0x04);
                                    break;
                                case 0x93: // RES 2, E
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegDE.LowByte &= unchecked((byte)~0x04);
                                    break;
                                case 0x94: // RES 2, H
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegHL.HighByte &= unchecked((byte)~0x04);
                                    break;
                                case 0x95: // RES 2, L
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegHL.LowByte &= unchecked((byte)~0x04);
                                    break;
                                case 0x96: // RES 2, (HL)
                                    totalExecutedCycles += 15; pendingCycles -= 15;
                                    WriteMemory(RegHL.LowWord, (byte)(ReadMemory(RegHL.LowWord) & unchecked((byte)~0x04)));
                                    break;
                                case 0x97: // RES 2, A
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegAF.HighByte &= unchecked((byte)~0x04);
                                    break;
                                case 0x98: // RES 3, B
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegBC.HighByte &= unchecked((byte)~0x08);
                                    break;
                                case 0x99: // RES 3, C
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegBC.LowByte &= unchecked((byte)~0x08);
                                    break;
                                case 0x9A: // RES 3, D
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegDE.HighByte &= unchecked((byte)~0x08);
                                    break;
                                case 0x9B: // RES 3, E
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegDE.LowByte &= unchecked((byte)~0x08);
                                    break;
                                case 0x9C: // RES 3, H
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegHL.HighByte &= unchecked((byte)~0x08);
                                    break;
                                case 0x9D: // RES 3, L
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegHL.LowByte &= unchecked((byte)~0x08);
                                    break;
                                case 0x9E: // RES 3, (HL)
                                    totalExecutedCycles += 15; pendingCycles -= 15;
                                    WriteMemory(RegHL.LowWord, (byte)(ReadMemory(RegHL.LowWord) & unchecked((byte)~0x08)));
                                    break;
                                case 0x9F: // RES 3, A
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegAF.HighByte &= unchecked((byte)~0x08);
                                    break;
                                case 0xA0: // RES 4, B
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegBC.HighByte &= unchecked((byte)~0x10);
                                    break;
                                case 0xA1: // RES 4, C
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegBC.LowByte &= unchecked((byte)~0x10);
                                    break;
                                case 0xA2: // RES 4, D
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegDE.HighByte &= unchecked((byte)~0x10);
                                    break;
                                case 0xA3: // RES 4, E
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegDE.LowByte &= unchecked((byte)~0x10);
                                    break;
                                case 0xA4: // RES 4, H
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegHL.HighByte &= unchecked((byte)~0x10);
                                    break;
                                case 0xA5: // RES 4, L
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegHL.LowByte &= unchecked((byte)~0x10);
                                    break;
                                case 0xA6: // RES 4, (HL)
                                    totalExecutedCycles += 15; pendingCycles -= 15;
                                    WriteMemory(RegHL.LowWord, (byte)(ReadMemory(RegHL.LowWord) & unchecked((byte)~0x10)));
                                    break;
                                case 0xA7: // RES 4, A
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegAF.HighByte &= unchecked((byte)~0x10);
                                    break;
                                case 0xA8: // RES 5, B
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegBC.HighByte &= unchecked((byte)~0x20);
                                    break;
                                case 0xA9: // RES 5, C
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegBC.LowByte &= unchecked((byte)~0x20);
                                    break;
                                case 0xAA: // RES 5, D
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegDE.HighByte &= unchecked((byte)~0x20);
                                    break;
                                case 0xAB: // RES 5, E
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegDE.LowByte &= unchecked((byte)~0x20);
                                    break;
                                case 0xAC: // RES 5, H
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegHL.HighByte &= unchecked((byte)~0x20);
                                    break;
                                case 0xAD: // RES 5, L
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegHL.LowByte &= unchecked((byte)~0x20);
                                    break;
                                case 0xAE: // RES 5, (HL)
                                    totalExecutedCycles += 15; pendingCycles -= 15;
                                    WriteMemory(RegHL.LowWord, (byte)(ReadMemory(RegHL.LowWord) & unchecked((byte)~0x20)));
                                    break;
                                case 0xAF: // RES 5, A
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegAF.HighByte &= unchecked((byte)~0x20);
                                    break;
                                case 0xB0: // RES 6, B
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegBC.HighByte &= unchecked((byte)~0x40);
                                    break;
                                case 0xB1: // RES 6, C
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegBC.LowByte &= unchecked((byte)~0x40);
                                    break;
                                case 0xB2: // RES 6, D
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegDE.HighByte &= unchecked((byte)~0x40);
                                    break;
                                case 0xB3: // RES 6, E
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegDE.LowByte &= unchecked((byte)~0x40);
                                    break;
                                case 0xB4: // RES 6, H
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegHL.HighByte &= unchecked((byte)~0x40);
                                    break;
                                case 0xB5: // RES 6, L
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegHL.LowByte &= unchecked((byte)~0x40);
                                    break;
                                case 0xB6: // RES 6, (HL)
                                    totalExecutedCycles += 15; pendingCycles -= 15;
                                    WriteMemory(RegHL.LowWord, (byte)(ReadMemory(RegHL.LowWord) & unchecked((byte)~0x40)));
                                    break;
                                case 0xB7: // RES 6, A
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegAF.HighByte &= unchecked((byte)~0x40);
                                    break;
                                case 0xB8: // RES 7, B
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegBC.HighByte &= unchecked((byte)~0x80);
                                    break;
                                case 0xB9: // RES 7, C
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegBC.LowByte &= unchecked((byte)~0x80);
                                    break;
                                case 0xBA: // RES 7, D
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegDE.HighByte &= unchecked((byte)~0x80);
                                    break;
                                case 0xBB: // RES 7, E
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegDE.LowByte &= unchecked((byte)~0x80);
                                    break;
                                case 0xBC: // RES 7, H
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegHL.HighByte &= unchecked((byte)~0x80);
                                    break;
                                case 0xBD: // RES 7, L
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegHL.LowByte &= unchecked((byte)~0x80);
                                    break;
                                case 0xBE: // RES 7, (HL)
                                    totalExecutedCycles += 15; pendingCycles -= 15;
                                    WriteMemory(RegHL.LowWord, (byte)(ReadMemory(RegHL.LowWord) & unchecked((byte)~0x80)));
                                    break;
                                case 0xBF: // RES 7, A
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegAF.HighByte &= unchecked((byte)~0x80);
                                    break;
                                case 0xC0: // SET 0, B
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegBC.HighByte |= unchecked((byte)0x01);
                                    break;
                                case 0xC1: // SET 0, C
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegBC.LowByte |= unchecked((byte)0x01);
                                    break;
                                case 0xC2: // SET 0, D
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegDE.HighByte |= unchecked((byte)0x01);
                                    break;
                                case 0xC3: // SET 0, E
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegDE.LowByte |= unchecked((byte)0x01);
                                    break;
                                case 0xC4: // SET 0, H
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegHL.HighByte |= unchecked((byte)0x01);
                                    break;
                                case 0xC5: // SET 0, L
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegHL.LowByte |= unchecked((byte)0x01);
                                    break;
                                case 0xC6: // SET 0, (HL)
                                    totalExecutedCycles += 15; pendingCycles -= 15;
                                    WriteMemory(RegHL.LowWord, (byte)(ReadMemory(RegHL.LowWord) | unchecked((byte)0x01)));
                                    break;
                                case 0xC7: // SET 0, A
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegAF.HighByte |= unchecked((byte)0x01);
                                    break;
                                case 0xC8: // SET 1, B
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegBC.HighByte |= unchecked((byte)0x02);
                                    break;
                                case 0xC9: // SET 1, C
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegBC.LowByte |= unchecked((byte)0x02);
                                    break;
                                case 0xCA: // SET 1, D
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegDE.HighByte |= unchecked((byte)0x02);
                                    break;
                                case 0xCB: // SET 1, E
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegDE.LowByte |= unchecked((byte)0x02);
                                    break;
                                case 0xCC: // SET 1, H
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegHL.HighByte |= unchecked((byte)0x02);
                                    break;
                                case 0xCD: // SET 1, L
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegHL.LowByte |= unchecked((byte)0x02);
                                    break;
                                case 0xCE: // SET 1, (HL)
                                    totalExecutedCycles += 15; pendingCycles -= 15;
                                    WriteMemory(RegHL.LowWord, (byte)(ReadMemory(RegHL.LowWord) | unchecked((byte)0x02)));
                                    break;
                                case 0xCF: // SET 1, A
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegAF.HighByte |= unchecked((byte)0x02);
                                    break;
                                case 0xD0: // SET 2, B
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegBC.HighByte |= unchecked((byte)0x04);
                                    break;
                                case 0xD1: // SET 2, C
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegBC.LowByte |= unchecked((byte)0x04);
                                    break;
                                case 0xD2: // SET 2, D
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegDE.HighByte |= unchecked((byte)0x04);
                                    break;
                                case 0xD3: // SET 2, E
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegDE.LowByte |= unchecked((byte)0x04);
                                    break;
                                case 0xD4: // SET 2, H
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegHL.HighByte |= unchecked((byte)0x04);
                                    break;
                                case 0xD5: // SET 2, L
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegHL.LowByte |= unchecked((byte)0x04);
                                    break;
                                case 0xD6: // SET 2, (HL)
                                    totalExecutedCycles += 15; pendingCycles -= 15;
                                    WriteMemory(RegHL.LowWord, (byte)(ReadMemory(RegHL.LowWord) | unchecked((byte)0x04)));
                                    break;
                                case 0xD7: // SET 2, A
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegAF.HighByte |= unchecked((byte)0x04);
                                    break;
                                case 0xD8: // SET 3, B
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegBC.HighByte |= unchecked((byte)0x08);
                                    break;
                                case 0xD9: // SET 3, C
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegBC.LowByte |= unchecked((byte)0x08);
                                    break;
                                case 0xDA: // SET 3, D
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegDE.HighByte |= unchecked((byte)0x08);
                                    break;
                                case 0xDB: // SET 3, E
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegDE.LowByte |= unchecked((byte)0x08);
                                    break;
                                case 0xDC: // SET 3, H
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegHL.HighByte |= unchecked((byte)0x08);
                                    break;
                                case 0xDD: // SET 3, L
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegHL.LowByte |= unchecked((byte)0x08);
                                    break;
                                case 0xDE: // SET 3, (HL)
                                    totalExecutedCycles += 15; pendingCycles -= 15;
                                    WriteMemory(RegHL.LowWord, (byte)(ReadMemory(RegHL.LowWord) | unchecked((byte)0x08)));
                                    break;
                                case 0xDF: // SET 3, A
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegAF.HighByte |= unchecked((byte)0x08);
                                    break;
                                case 0xE0: // SET 4, B
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegBC.HighByte |= unchecked((byte)0x10);
                                    break;
                                case 0xE1: // SET 4, C
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegBC.LowByte |= unchecked((byte)0x10);
                                    break;
                                case 0xE2: // SET 4, D
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegDE.HighByte |= unchecked((byte)0x10);
                                    break;
                                case 0xE3: // SET 4, E
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegDE.LowByte |= unchecked((byte)0x10);
                                    break;
                                case 0xE4: // SET 4, H
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegHL.HighByte |= unchecked((byte)0x10);
                                    break;
                                case 0xE5: // SET 4, L
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegHL.LowByte |= unchecked((byte)0x10);
                                    break;
                                case 0xE6: // SET 4, (HL)
                                    totalExecutedCycles += 15; pendingCycles -= 15;
                                    WriteMemory(RegHL.LowWord, (byte)(ReadMemory(RegHL.LowWord) | unchecked((byte)0x10)));
                                    break;
                                case 0xE7: // SET 4, A
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegAF.HighByte |= unchecked((byte)0x10);
                                    break;
                                case 0xE8: // SET 5, B
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegBC.HighByte |= unchecked((byte)0x20);
                                    break;
                                case 0xE9: // SET 5, C
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegBC.LowByte |= unchecked((byte)0x20);
                                    break;
                                case 0xEA: // SET 5, D
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegDE.HighByte |= unchecked((byte)0x20);
                                    break;
                                case 0xEB: // SET 5, E
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegDE.LowByte |= unchecked((byte)0x20);
                                    break;
                                case 0xEC: // SET 5, H
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegHL.HighByte |= unchecked((byte)0x20);
                                    break;
                                case 0xED: // SET 5, L
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegHL.LowByte |= unchecked((byte)0x20);
                                    break;
                                case 0xEE: // SET 5, (HL)
                                    totalExecutedCycles += 15; pendingCycles -= 15;
                                    WriteMemory(RegHL.LowWord, (byte)(ReadMemory(RegHL.LowWord) | unchecked((byte)0x20)));
                                    break;
                                case 0xEF: // SET 5, A
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegAF.HighByte |= unchecked((byte)0x20);
                                    break;
                                case 0xF0: // SET 6, B
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegBC.HighByte |= unchecked((byte)0x40);
                                    break;
                                case 0xF1: // SET 6, C
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegBC.LowByte |= unchecked((byte)0x40);
                                    break;
                                case 0xF2: // SET 6, D
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegDE.HighByte |= unchecked((byte)0x40);
                                    break;
                                case 0xF3: // SET 6, E
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegDE.LowByte |= unchecked((byte)0x40);
                                    break;
                                case 0xF4: // SET 6, H
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegHL.HighByte |= unchecked((byte)0x40);
                                    break;
                                case 0xF5: // SET 6, L
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegHL.LowByte |= unchecked((byte)0x40);
                                    break;
                                case 0xF6: // SET 6, (HL)
                                    totalExecutedCycles += 15; pendingCycles -= 15;
                                    WriteMemory(RegHL.LowWord, (byte)(ReadMemory(RegHL.LowWord) | unchecked((byte)0x40)));
                                    break;
                                case 0xF7: // SET 6, A
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegAF.HighByte |= unchecked((byte)0x40);
                                    break;
                                case 0xF8: // SET 7, B
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegBC.HighByte |= unchecked((byte)0x80);
                                    break;
                                case 0xF9: // SET 7, C
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegBC.LowByte |= unchecked((byte)0x80);
                                    break;
                                case 0xFA: // SET 7, D
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegDE.HighByte |= unchecked((byte)0x80);
                                    break;
                                case 0xFB: // SET 7, E
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegDE.LowByte |= unchecked((byte)0x80);
                                    break;
                                case 0xFC: // SET 7, H
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegHL.HighByte |= unchecked((byte)0x80);
                                    break;
                                case 0xFD: // SET 7, L
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegHL.LowByte |= unchecked((byte)0x80);
                                    break;
                                case 0xFE: // SET 7, (HL)
                                    totalExecutedCycles += 15; pendingCycles -= 15;
                                    WriteMemory(RegHL.LowWord, (byte)(ReadMemory(RegHL.LowWord) | unchecked((byte)0x80)));
                                    break;
                                case 0xFF: // SET 7, A
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegAF.HighByte |= unchecked((byte)0x80);
                                    break;
                            }
                            break;
                        case 0xCC: // CALL Z, nn
                            RegWZ.LowWord = (ushort)(ReadOpArg(RegPC.LowWord++) + ReadOpArg(RegPC.LowWord++) * 256);
                            if (RegFlagZ)
                            {
                                WriteMemory(--RegSP.LowWord, RegPC.HighByte); WriteMemory(--RegSP.LowWord, RegPC.LowByte);
                                RegPC.LowWord = RegWZ.LowWord;
                                totalExecutedCycles += 17; pendingCycles -= 17;
                            }
                            else
                            {
                                totalExecutedCycles += 10; pendingCycles -= 10;
                            }
                            break;
                        case 0xCD: // CALL nn
                            RegWZ.LowWord = (ushort)(ReadOpArg(RegPC.LowWord++) + ReadOpArg(RegPC.LowWord++) * 256);
                            WriteMemory(--RegSP.LowWord, RegPC.HighByte); WriteMemory(--RegSP.LowWord, RegPC.LowByte);
                            RegPC.LowWord = RegWZ.LowWord;
                            totalExecutedCycles += 17; pendingCycles -= 17;
                            break;
                        case 0xCE: // ADC A, n
                            RegAF.LowWord = TableALU[1, RegAF.HighByte, ReadOpArg(RegPC.LowWord++), RegFlagC ? 1 : 0];
                            totalExecutedCycles += 7; pendingCycles -= 7;
                            break;
                        case 0xCF: // RST $08
                            WriteMemory(--RegSP.LowWord, RegPC.HighByte); WriteMemory(--RegSP.LowWord, RegPC.LowByte);
                            RegPC.LowWord = 0x08;
                            totalExecutedCycles += 11; pendingCycles -= 11;
                            break;
                        case 0xD0: // RET NC
                            if (!RegFlagC)
                            {
                                RegPC.LowByte = ReadMemory(RegSP.LowWord++); RegPC.HighByte = ReadMemory(RegSP.LowWord++);
                                RegWZ.LowWord = RegPC.LowWord;
                                totalExecutedCycles += 11; pendingCycles -= 11;
                            }
                            else
                            {
                                totalExecutedCycles += 5; pendingCycles -= 5;
                            }
                            break;
                        case 0xD1: // POP DE
                            RegDE.LowByte = ReadMemory(RegSP.LowWord++); RegDE.HighByte = ReadMemory(RegSP.LowWord++);
                            totalExecutedCycles += 10; pendingCycles -= 10;
                            break;
                        case 0xD2: // JP NC, nn
                            RegWZ.LowWord = (ushort)(ReadOpArg(RegPC.LowWord++) + ReadOpArg(RegPC.LowWord++) * 256);
                            if (!RegFlagC)
                            {
                                RegPC.LowWord = RegWZ.LowWord;
                            }
                            totalExecutedCycles += 10; pendingCycles -= 10;
                            break;
                        case 0xD3: // OUT n, A
                            TUS = (ushort)(ReadOpArg(RegPC.LowWord++) | (RegAF.HighByte << 8));
                            WriteHardware(TUS, RegAF.HighByte);
                            RegWZ.LowByte = (byte)(((TUS & 0xFF) + 1) & 0xFF);
                            RegWZ.HighByte = RegAF.HighByte;
                            totalExecutedCycles += 11; pendingCycles -= 11;
                            break;
                        case 0xD4: // CALL NC, nn
                            RegWZ.LowWord = (ushort)(ReadOpArg(RegPC.LowWord++) + ReadOpArg(RegPC.LowWord++) * 256);
                            if (!RegFlagC)
                            {
                                WriteMemory(--RegSP.LowWord, RegPC.HighByte); WriteMemory(--RegSP.LowWord, RegPC.LowByte);
                                RegPC.LowWord = RegWZ.LowWord;
                                totalExecutedCycles += 17; pendingCycles -= 17;
                            }
                            else
                            {
                                totalExecutedCycles += 10; pendingCycles -= 10;
                            }
                            break;
                        case 0xD5: // PUSH DE
                            WriteMemory(--RegSP.LowWord, RegDE.HighByte); WriteMemory(--RegSP.LowWord, RegDE.LowByte);
                            totalExecutedCycles += 11; pendingCycles -= 11;
                            break;
                        case 0xD6: // SUB n
                            RegAF.LowWord = TableALU[2, RegAF.HighByte, ReadOpArg(RegPC.LowWord++), 0];
                            totalExecutedCycles += 7; pendingCycles -= 7;
                            break;
                        case 0xD7: // RST $10
                            WriteMemory(--RegSP.LowWord, RegPC.HighByte); WriteMemory(--RegSP.LowWord, RegPC.LowByte);
                            RegPC.LowWord = 0x10;
                            totalExecutedCycles += 11; pendingCycles -= 11;
                            break;
                        case 0xD8: // RET C
                            if (RegFlagC)
                            {
                                RegPC.LowByte = ReadMemory(RegSP.LowWord++); RegPC.HighByte = ReadMemory(RegSP.LowWord++);
                                RegWZ.LowWord = RegPC.LowWord;
                                totalExecutedCycles += 11; pendingCycles -= 11;
                            }
                            else
                            {
                                totalExecutedCycles += 5; pendingCycles -= 5;
                            }
                            break;
                        case 0xD9: // EXX
                            TUS = RegBC.LowWord; RegBC.LowWord = RegAltBC.LowWord; RegAltBC.LowWord = TUS;
                            TUS = RegDE.LowWord; RegDE.LowWord = RegAltDE.LowWord; RegAltDE.LowWord = TUS;
                            TUS = RegHL.LowWord; RegHL.LowWord = RegAltHL.LowWord; RegAltHL.LowWord = TUS;
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0xDA: // JP C, nn
                            RegWZ.LowWord = (ushort)(ReadOpArg(RegPC.LowWord++) + ReadOpArg(RegPC.LowWord++) * 256);
                            if (RegFlagC)
                            {
                                RegPC.LowWord = RegWZ.LowWord;
                            }
                            totalExecutedCycles += 10; pendingCycles -= 10;
                            break;
                        case 0xDB: // IN A, n
                            TUS = (ushort)(ReadOpArg(RegPC.LowWord++) | (RegAF.HighByte << 8));
                            RegAF.HighByte = ReadHardware(TUS);
                            RegWZ.LowWord = (ushort)(TUS + 1);
                            totalExecutedCycles += 11; pendingCycles -= 11;
                            break;
                        case 0xDC: // CALL C, nn
                            RegWZ.LowWord = (ushort)(ReadOpArg(RegPC.LowWord++) + ReadOpArg(RegPC.LowWord++) * 256);
                            if (RegFlagC)
                            {
                                WriteMemory(--RegSP.LowWord, RegPC.HighByte); WriteMemory(--RegSP.LowWord, RegPC.LowByte);
                                RegPC.LowWord = RegWZ.LowWord;
                                totalExecutedCycles += 17; pendingCycles -= 17;
                            }
                            else
                            {
                                totalExecutedCycles += 10; pendingCycles -= 10;
                            }
                            break;
                        case 0xDD: // (Prefix)
                            ++RegR;
                            switch (ReadOp(RegPC.LowWord++))
                            {
                                case 0x00: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x01: // LD BC, nn
                                    totalExecutedCycles += 10; pendingCycles -= 10;
                                    RegBC.LowWord = (ushort)(ReadMemory(RegPC.LowWord++) + ReadMemory(RegPC.LowWord++) * 256);
                                    break;
                                case 0x02: // LD (BC), A
                                    totalExecutedCycles += 7; pendingCycles -= 7;
                                    WriteMemory(RegBC.LowWord, RegAF.HighByte);
                                    break;
                                case 0x03: // INC BC
                                    totalExecutedCycles += 6; pendingCycles -= 6;
                                    ++RegBC.LowWord;                                    
                                    break;
                                case 0x04: // INC B
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowByte = (byte)(TableInc[++RegBC.HighByte] | (RegAF.LowByte & 1));                                    
                                    break;
                                case 0x05: // DEC B
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowByte = (byte)(TableDec[--RegBC.HighByte] | (RegAF.LowByte & 1));                                    
                                    break;
                                case 0x06: // LD B, n
                                    totalExecutedCycles += 7; pendingCycles -= 7;
                                    RegBC.HighByte = ReadMemory(RegPC.LowWord++);                                    
                                    break;
                                case 0x07: // RLCA
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableRotShift[0, 0, RegAF.LowWord];                                    
                                    break;
                                case 0x08: // EX AF, AF'
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    TUS = RegAF.LowWord; RegAF.LowWord = RegAltAF.LowWord; RegAltAF.LowWord = TUS;                                    
                                    break;
                                case 0x09: // ADD IX, BC
                                    totalExecutedCycles += 15; pendingCycles -= 15;
                                    TI1 = (short)RegIX.LowWord; TI2 = (short)RegBC.LowWord; TIR = TI1 + TI2;
                                    TUS = (ushort)TIR;
                                    RegWZ.LowWord = (ushort)(RegIX.LowWord + 1);
                                    RegFlagH = ((TI1 & 0xFFF) + (TI2 & 0xFFF)) > 0xFFF;
                                    RegFlagN = false;
                                    RegFlagC = ((ushort)TI1 + (ushort)TI2) > 0xFFFF;
                                    RegIX.LowWord = TUS;
                                    RegFlag3 = (TUS & 0x0800) != 0;
                                    RegFlag5 = (TUS & 0x2000) != 0;                                    
                                    break;
                                case 0x0A: // LD A, (BC)
                                    totalExecutedCycles += 7; pendingCycles -= 7;
                                    RegAF.HighByte = ReadMemory(RegBC.LowWord);
                                    break;
                                case 0x0B: // DEC BC
                                    totalExecutedCycles += 6; pendingCycles -= 6;
                                    --RegBC.LowWord;                                    
                                    break;
                                case 0x0C: // INC C
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowByte = (byte)(TableInc[++RegBC.LowByte] | (RegAF.LowByte & 1));                                    
                                    break;
                                case 0x0D: // DEC C
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowByte = (byte)(TableDec[--RegBC.LowByte] | (RegAF.LowByte & 1));
                                    break;
                                case 0x0E: // LD C, n
                                    totalExecutedCycles += 7; pendingCycles -= 7;
                                    RegBC.LowByte = ReadMemory(RegPC.LowWord++);
                                    break;
                                case 0x0F: // RRCA
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableRotShift[0, 1, RegAF.LowWord];
                                    break;
                                case 0x10: // DJNZ d
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    TSB = (sbyte)ReadMemory(RegPC.LowWord++);
                                    if (--RegBC.HighByte != 0)
                                    {
                                        RegPC.LowWord = (ushort)(RegPC.LowWord + TSB);
                                        totalExecutedCycles += 5; pendingCycles -= 5;
                                    }
                                    break;
                                case 0x11: // LD DE, nn
                                    totalExecutedCycles += 10; pendingCycles -= 10;
                                    RegDE.LowWord = (ushort)(ReadMemory(RegPC.LowWord++) + ReadMemory(RegPC.LowWord++) * 256);
                                    break;
                                case 0x12: // LD (DE), A
                                    totalExecutedCycles += 7; pendingCycles -= 7;
                                    WriteMemory(RegDE.LowWord, RegAF.HighByte);
                                    break;
                                case 0x13: // INC DE
                                    totalExecutedCycles += 6; pendingCycles -= 6;
                                    ++RegDE.LowWord;                                    
                                    break;
                                case 0x14: // INC D
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowByte = (byte)(TableInc[++RegDE.HighByte] | (RegAF.LowByte & 1));                                    
                                    break;
                                case 0x15: // DEC D
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowByte = (byte)(TableDec[--RegDE.HighByte] | (RegAF.LowByte & 1));
                                    break;
                                case 0x16: // LD D, n
                                    totalExecutedCycles += 7; pendingCycles -= 7;
                                    RegDE.HighByte = ReadMemory(RegPC.LowWord++);
                                    break;
                                case 0x17: // RLA
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableRotShift[0, 2, RegAF.LowWord];
                                    break;
                                case 0x18: // JR d
                                    totalExecutedCycles += 12; pendingCycles -= 12;
                                    TSB = (sbyte)ReadMemory(RegPC.LowWord++);
                                    RegPC.LowWord = (ushort)(RegPC.LowWord + TSB);
                                    break;
                                case 0x19: // ADD IX, DE
                                    totalExecutedCycles += 15; pendingCycles -= 15;
                                    TI1 = (short)RegIX.LowWord; TI2 = (short)RegDE.LowWord; TIR = TI1 + TI2;
                                    TUS = (ushort)TIR;
                                    RegWZ.LowWord = (ushort)(RegIX.LowWord + 1);
                                    RegFlagH = ((TI1 & 0xFFF) + (TI2 & 0xFFF)) > 0xFFF;
                                    RegFlagN = false;
                                    RegFlagC = ((ushort)TI1 + (ushort)TI2) > 0xFFFF;
                                    RegIX.LowWord = TUS;
                                    RegFlag3 = (TUS & 0x0800) != 0;
                                    RegFlag5 = (TUS & 0x2000) != 0;                                    
                                    break;
                                case 0x1A: // LD A, (DE)
                                    totalExecutedCycles += 7; pendingCycles -= 7;
                                    RegAF.HighByte = ReadMemory(RegDE.LowWord);
                                    break;
                                case 0x1B: // DEC DE
                                    totalExecutedCycles += 6; pendingCycles -= 6;
                                    --RegDE.LowWord;                                    
                                    break;
                                case 0x1C: // INC E
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowByte = (byte)(TableInc[++RegDE.LowByte] | (RegAF.LowByte & 1));                                    
                                    break;
                                case 0x1D: // DEC E
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowByte = (byte)(TableDec[--RegDE.LowByte] | (RegAF.LowByte & 1));
                                    break;
                                case 0x1E: // LD E, n
                                    totalExecutedCycles += 7; pendingCycles -= 7;
                                    RegDE.LowByte = ReadMemory(RegPC.LowWord++);                                    
                                    break;
                                case 0x1F: // RRA
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableRotShift[0, 3, RegAF.LowWord];                                    
                                    break;
                                case 0x20: // JR NZ, d
                                    totalExecutedCycles += 7; pendingCycles -= 7;
                                    TSB = (sbyte)ReadMemory(RegPC.LowWord++);
                                    if (!RegFlagZ)
                                    {
                                        RegPC.LowWord = (ushort)(RegPC.LowWord + TSB);
                                        totalExecutedCycles += 5; pendingCycles -= 5;
                                    }
                                    break;
                                case 0x21: // LD IX, nn
                                    totalExecutedCycles += 14; pendingCycles -= 14;
                                    RegIX.LowWord = (ushort)(ReadOpArg(RegPC.LowWord++) + ReadOpArg(RegPC.LowWord++) * 256);                                    
                                    break;
                                case 0x22: // LD (nn), IX
                                    totalExecutedCycles += 20; pendingCycles -= 20;
                                    TUS = (ushort)(ReadOpArg(RegPC.LowWord++) + ReadOpArg(RegPC.LowWord++) * 256);
                                    WriteMemory(TUS++, RegIX.LowByte);
                                    WriteMemory(TUS, RegIX.HighByte);
                                    RegWZ.LowWord = TUS;
                                    break;
                                case 0x23: // INC IX
                                    totalExecutedCycles += 10; pendingCycles -= 10;
                                    ++RegIX.LowWord;                                    
                                    break;
                                case 0x24: // INC IXH
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegAF.LowByte = (byte)(TableInc[++RegIX.HighByte] | (RegAF.LowByte & 1));                                    
                                    break;
                                case 0x25: // DEC IXH
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegAF.LowByte = (byte)(TableDec[--RegIX.HighByte] | (RegAF.LowByte & 1));                                    
                                    break;
                                case 0x26: // LD IXH, n
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegIX.HighByte = ReadOpArg(RegPC.LowWord++);
                                    break;
                                case 0x27: // DAA
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableDaa[RegAF.LowWord];                                    
                                    break;
                                case 0x28: // JR Z, d
                                    totalExecutedCycles += 7; pendingCycles -= 7;
                                    TSB = (sbyte)ReadMemory(RegPC.LowWord++);
                                    if (RegFlagZ)
                                    {
                                        RegPC.LowWord = (ushort)(RegPC.LowWord + TSB);
                                        totalExecutedCycles += 5; pendingCycles -= 5;
                                    }
                                    break;
                                case 0x29: // ADD IX, IX
                                    totalExecutedCycles += 15; pendingCycles -= 15;
                                    TI1 = (short)RegIX.LowWord; TI2 = (short)RegIX.LowWord; TIR = TI1 + TI2;
                                    TUS = (ushort)TIR;
                                    RegWZ.LowWord = (ushort)(RegIX.LowWord + 1);
                                    RegFlagH = ((TI1 & 0xFFF) + (TI2 & 0xFFF)) > 0xFFF;
                                    RegFlagN = false;
                                    RegFlagC = ((ushort)TI1 + (ushort)TI2) > 0xFFFF;
                                    RegIX.LowWord = TUS;
                                    RegFlag3 = (TUS & 0x0800) != 0;
                                    RegFlag5 = (TUS & 0x2000) != 0;                                    
                                    break;
                                case 0x2A: // LD IX, (nn)
                                    totalExecutedCycles += 20; pendingCycles -= 20;
                                    TUS = (ushort)(ReadOpArg(RegPC.LowWord++) + ReadOpArg(RegPC.LowWord++) * 256);
                                    RegIX.LowByte = ReadMemory(TUS++); RegIX.HighByte = ReadMemory(TUS);
                                    RegWZ.LowWord = TUS;
                                    break;
                                case 0x2B: // DEC IX
                                    totalExecutedCycles += 10; pendingCycles -= 10;
                                    --RegIX.LowWord;                                    
                                    break;
                                case 0x2C: // INC IXL
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegAF.LowByte = (byte)(TableInc[++RegIX.LowByte] | (RegAF.LowByte & 1));                                    
                                    break;
                                case 0x2D: // DEC IXL
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegAF.LowByte = (byte)(TableDec[--RegIX.LowByte] | (RegAF.LowByte & 1));
                                    break;
                                case 0x2E: // LD IXL, n
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegIX.LowByte = ReadOpArg(RegPC.LowWord++);
                                    break;
                                case 0x2F: // CPL
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.HighByte ^= 0xFF; RegFlagH = true; RegFlagN = true; RegFlag3 = (RegAF.HighByte & 0x08) != 0; RegFlag5 = (RegAF.HighByte & 0x20) != 0;                                    
                                    break;
                                case 0x30: // JR NC, d
                                    totalExecutedCycles += 7; pendingCycles -= 7;
                                    TSB = (sbyte)ReadMemory(RegPC.LowWord++);
                                    if (!RegFlagC)
                                    {
                                        RegPC.LowWord = (ushort)(RegPC.LowWord + TSB);
                                        totalExecutedCycles += 5; pendingCycles -= 5;
                                    }
                                    break;
                                case 0x31: // LD SP, nn
                                    totalExecutedCycles += 10; pendingCycles -= 10;
                                    RegSP.LowWord = (ushort)(ReadMemory(RegPC.LowWord++) + ReadMemory(RegPC.LowWord++) * 256);
                                    break;
                                case 0x32: // LD (nn), A
                                    totalExecutedCycles += 13; pendingCycles -= 13;
                                    WriteMemory((ushort)(ReadMemory(RegPC.LowWord++) + ReadMemory(RegPC.LowWord++) * 256), RegAF.HighByte);
                                    break;
                                case 0x33: // INC SP
                                    totalExecutedCycles += 6; pendingCycles -= 6;
                                    ++RegSP.LowWord;                                    
                                    break;
                                case 0x34: // INC (IX+d)
                                    totalExecutedCycles += 23; pendingCycles -= 23;
                                    Displacement = (sbyte)ReadOpArg(RegPC.LowWord++);
                                    RegWZ.LowWord = (ushort)(RegIX.LowWord + Displacement);
                                    TB = ReadMemory(RegWZ.LowWord); RegAF.LowByte = (byte)(TableInc[++TB] | (RegAF.LowByte & 1)); WriteMemory(RegWZ.LowWord, TB);
                                    break;
                                case 0x35: // DEC (IX+d)
                                    totalExecutedCycles += 23; pendingCycles -= 23;
                                    Displacement = (sbyte)ReadOpArg(RegPC.LowWord++);
                                    RegWZ.LowWord = (ushort)(RegIX.LowWord + Displacement);
                                    TB = ReadMemory(RegWZ.LowWord); RegAF.LowByte = (byte)(TableDec[--TB] | (RegAF.LowByte & 1)); WriteMemory(RegWZ.LowWord, TB);
                                    break;
                                case 0x36: // LD (IX+d), n
                                    totalExecutedCycles += 19; pendingCycles -= 19;
                                    Displacement = (sbyte)ReadOpArg(RegPC.LowWord++);
                                    RegWZ.LowWord = (ushort)(RegIX.LowWord + Displacement);
                                    WriteMemory(RegWZ.LowWord, ReadOpArg(RegPC.LowWord++));
                                    break;
                                case 0x37: // SCF
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegFlagH = false; RegFlagN = false; RegFlagC = true; RegFlag3 = (RegAF.HighByte & 0x08) != 0; RegFlag5 = (RegAF.HighByte & 0x20) != 0;                                    
                                    break;
                                case 0x38: // JR C, d
                                    totalExecutedCycles += 7; pendingCycles -= 7;
                                    TSB = (sbyte)ReadMemory(RegPC.LowWord++);
                                    if (RegFlagC)
                                    {
                                        RegPC.LowWord = (ushort)(RegPC.LowWord + TSB);
                                        totalExecutedCycles += 5; pendingCycles -= 5;
                                    }
                                    break;
                                case 0x39: // ADD IX, SP
                                    totalExecutedCycles += 15; pendingCycles -= 15;
                                    TI1 = (short)RegIX.LowWord; TI2 = (short)RegSP.LowWord; TIR = TI1 + TI2;
                                    TUS = (ushort)TIR;
                                    RegWZ.LowWord = (ushort)(RegIX.LowWord + 1);
                                    RegFlagH = ((TI1 & 0xFFF) + (TI2 & 0xFFF)) > 0xFFF;
                                    RegFlagN = false;
                                    RegFlagC = ((ushort)TI1 + (ushort)TI2) > 0xFFFF;
                                    RegIX.LowWord = TUS;
                                    RegFlag3 = (TUS & 0x0800) != 0;
                                    RegFlag5 = (TUS & 0x2000) != 0;                                    
                                    break;
                                case 0x3A: // LD A, (nn)
                                    totalExecutedCycles += 13; pendingCycles -= 13;
                                    RegAF.HighByte = ReadMemory((ushort)(ReadMemory(RegPC.LowWord++) + ReadMemory(RegPC.LowWord++) * 256));
                                    break;
                                case 0x3B: // DEC SP
                                    totalExecutedCycles += 6; pendingCycles -= 6;
                                    --RegSP.LowWord;                                    
                                    break;
                                case 0x3C: // INC A
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowByte = (byte)(TableInc[++RegAF.HighByte] | (RegAF.LowByte & 1));                                    
                                    break;
                                case 0x3D: // DEC A
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowByte = (byte)(TableDec[--RegAF.HighByte] | (RegAF.LowByte & 1));
                                    break;
                                case 0x3E: // LD A, n
                                    totalExecutedCycles += 7; pendingCycles -= 7;
                                    RegAF.HighByte = ReadMemory(RegPC.LowWord++);
                                    break;
                                case 0x3F: // CCF
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegFlagH = RegFlagC; RegFlagN = false; RegFlagC ^= true; RegFlag3 = (RegAF.HighByte & 0x08) != 0; RegFlag5 = (RegAF.HighByte & 0x20) != 0;
                                    break;
                                case 0x40: // LD B, B
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x41: // LD B, C
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegBC.HighByte = RegBC.LowByte;                                    
                                    break;
                                case 0x42: // LD B, D
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegBC.HighByte = RegDE.HighByte;
                                    break;
                                case 0x43: // LD B, E
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegBC.HighByte = RegDE.LowByte;
                                    break;
                                case 0x44: // LD B, IXH
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegBC.HighByte = RegIX.HighByte;                                    
                                    break;
                                case 0x45: // LD B, IXL
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegBC.HighByte = RegIX.LowByte;
                                    break;
                                case 0x46: // LD B, (IX+d)
                                    totalExecutedCycles += 19; pendingCycles -= 19;
                                    Displacement = (sbyte)ReadOpArg(RegPC.LowWord++);
                                    RegWZ.LowWord = (ushort)(RegIX.LowWord + Displacement);
                                    RegBC.HighByte = ReadMemory(RegWZ.LowWord);
                                    break;
                                case 0x47: // LD B, A
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegBC.HighByte = RegAF.HighByte;                                    
                                    break;
                                case 0x48: // LD C, B
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegBC.LowByte = RegBC.HighByte;
                                    break;
                                case 0x49: // LD C, C
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x4A: // LD C, D
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegBC.LowByte = RegDE.HighByte;
                                    break;
                                case 0x4B: // LD C, E
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegBC.LowByte = RegDE.LowByte;
                                    break;
                                case 0x4C: // LD C, IXH
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegBC.LowByte = RegIX.HighByte;                                    
                                    break;
                                case 0x4D: // LD C, IXL
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegBC.LowByte = RegIX.LowByte;
                                    break;
                                case 0x4E: // LD C, (IX+d)
                                    totalExecutedCycles += 19; pendingCycles -= 19;
                                    Displacement = (sbyte)ReadOpArg(RegPC.LowWord++);
                                    RegWZ.LowWord = (ushort)(RegIX.LowWord + Displacement);
                                    RegBC.LowByte = ReadMemory(RegWZ.LowWord);
                                    break;
                                case 0x4F: // LD C, A
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegBC.LowByte = RegAF.HighByte;                                    
                                    break;
                                case 0x50: // LD D, B
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegDE.HighByte = RegBC.HighByte;
                                    break;
                                case 0x51: // LD D, C
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegDE.HighByte = RegBC.LowByte;
                                    break;
                                case 0x52: // LD D, D
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x53: // LD D, E
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegDE.HighByte = RegDE.LowByte;
                                    break;
                                case 0x54: // LD D, IXH
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegDE.HighByte = RegIX.HighByte;                                    
                                    break;
                                case 0x55: // LD D, IXL
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegDE.HighByte = RegIX.LowByte;
                                    break;
                                case 0x56: // LD D, (IX+d)
                                    totalExecutedCycles += 19; pendingCycles -= 19;
                                    Displacement = (sbyte)ReadOpArg(RegPC.LowWord++);
                                    RegWZ.LowWord = (ushort)(RegIX.LowWord + Displacement);
                                    RegDE.HighByte = ReadMemory(RegWZ.LowWord);
                                    break;
                                case 0x57: // LD D, A
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegDE.HighByte = RegAF.HighByte;                                    
                                    break;
                                case 0x58: // LD E, B
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegDE.LowByte = RegBC.HighByte;
                                    break;
                                case 0x59: // LD E, C
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegDE.LowByte = RegBC.LowByte;
                                    break;
                                case 0x5A: // LD E, D
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegDE.LowByte = RegDE.HighByte;
                                    break;
                                case 0x5B: // LD E, E
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x5C: // LD E, IXH
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegDE.LowByte = RegIX.HighByte;                                    
                                    break;
                                case 0x5D: // LD E, IXL
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegDE.LowByte = RegIX.LowByte;
                                    break;
                                case 0x5E: // LD E, (IX+d)
                                    totalExecutedCycles += 19; pendingCycles -= 19;
                                    Displacement = (sbyte)ReadOpArg(RegPC.LowWord++);
                                    RegWZ.LowWord = (ushort)(RegIX.LowWord + Displacement);
                                    RegDE.LowByte = ReadMemory(RegWZ.LowWord);
                                    break;
                                case 0x5F: // LD E, A
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegDE.LowByte = RegAF.HighByte;                                    
                                    break;
                                case 0x60: // LD IXH, B
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegIX.HighByte = RegBC.HighByte;                                    
                                    break;
                                case 0x61: // LD IXH, C
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegIX.HighByte = RegBC.LowByte;
                                    break;
                                case 0x62: // LD IXH, D
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegIX.HighByte = RegDE.HighByte;
                                    break;
                                case 0x63: // LD IXH, E
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegIX.HighByte = RegDE.LowByte;
                                    break;
                                case 0x64: // LD IXH, IXH
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    break;
                                case 0x65: // LD IXH, IXL
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegIX.HighByte = RegIX.LowByte;
                                    break;
                                case 0x66: // LD H, (IX+d)
                                    totalExecutedCycles += 19; pendingCycles -= 19;
                                    Displacement = (sbyte)ReadOpArg(RegPC.LowWord++);
                                    RegWZ.LowWord = (ushort)(RegIX.LowWord + Displacement);
                                    RegHL.HighByte = ReadMemory(RegWZ.LowWord);
                                    break;
                                case 0x67: // LD IXH, A
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegIX.HighByte = RegAF.HighByte;
                                    break;
                                case 0x68: // LD IXL, B
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegIX.LowByte = RegBC.HighByte;
                                    break;
                                case 0x69: // LD IXL, C
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegIX.LowByte = RegBC.LowByte;
                                    break;
                                case 0x6A: // LD IXL, D
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegIX.LowByte = RegDE.HighByte;
                                    break;
                                case 0x6B: // LD IXL, E
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegIX.LowByte = RegDE.LowByte;
                                    break;
                                case 0x6C: // LD IXL, IXH
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegIX.LowByte = RegIX.HighByte;
                                    break;
                                case 0x6D: // LD IXL, IXL
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    break;
                                case 0x6E: // LD L, (IX+d)
                                    totalExecutedCycles += 19; pendingCycles -= 19;
                                    Displacement = (sbyte)ReadOpArg(RegPC.LowWord++);
                                    RegWZ.LowWord = (ushort)(RegIX.LowWord + Displacement);
                                    RegHL.LowByte = ReadMemory(RegWZ.LowWord);
                                    break;
                                case 0x6F: // LD IXL, A
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegIX.LowByte = RegAF.HighByte;
                                    break;
                                case 0x70: // LD (IX+d), B
                                    totalExecutedCycles += 19; pendingCycles -= 19;
                                    Displacement = (sbyte)ReadOpArg(RegPC.LowWord++);
                                    RegWZ.LowWord = (ushort)(RegIX.LowWord + Displacement);
                                    WriteMemory(RegWZ.LowWord, RegBC.HighByte);
                                    break;
                                case 0x71: // LD (IX+d), C
                                    totalExecutedCycles += 19; pendingCycles -= 19;
                                    Displacement = (sbyte)ReadOpArg(RegPC.LowWord++);
                                    RegWZ.LowWord = (ushort)(RegIX.LowWord + Displacement);
                                    WriteMemory(RegWZ.LowWord, RegBC.LowByte);
                                    break;
                                case 0x72: // LD (IX+d), D
                                    totalExecutedCycles += 19; pendingCycles -= 19;
                                    Displacement = (sbyte)ReadOpArg(RegPC.LowWord++);
                                    RegWZ.LowWord = (ushort)(RegIX.LowWord + Displacement);
                                    WriteMemory(RegWZ.LowWord, RegDE.HighByte);
                                    break;
                                case 0x73: // LD (IX+d), E
                                    totalExecutedCycles += 19; pendingCycles -= 19;
                                    Displacement = (sbyte)ReadOpArg(RegPC.LowWord++);
                                    RegWZ.LowWord = (ushort)(RegIX.LowWord + Displacement);
                                    WriteMemory(RegWZ.LowWord, RegDE.LowByte);
                                    break;
                                case 0x74: // LD (IX+d), H
                                    totalExecutedCycles += 19; pendingCycles -= 19;
                                    Displacement = (sbyte)ReadOpArg(RegPC.LowWord++);
                                    RegWZ.LowWord = (ushort)(RegIX.LowWord + Displacement);
                                    WriteMemory(RegWZ.LowWord, RegHL.HighByte);
                                    break;
                                case 0x75: // LD (IX+d), L
                                    totalExecutedCycles += 19; pendingCycles -= 19;
                                    Displacement = (sbyte)ReadOpArg(RegPC.LowWord++);
                                    RegWZ.LowWord = (ushort)(RegIX.LowWord + Displacement);
                                    WriteMemory(RegWZ.LowWord, RegHL.LowByte);
                                    break;
                                case 0x76: // HALT
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    Halt();                                    
                                    break;
                                case 0x77: // LD (IX+d), A
                                    totalExecutedCycles += 19; pendingCycles -= 19;
                                    Displacement = (sbyte)ReadOpArg(RegPC.LowWord++);
                                    RegWZ.LowWord = (ushort)(RegIX.LowWord + Displacement);
                                    WriteMemory(RegWZ.LowWord, RegAF.HighByte);
                                    break;
                                case 0x78: // LD A, B
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.HighByte = RegBC.HighByte;                                    
                                    break;
                                case 0x79: // LD A, C
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.HighByte = RegBC.LowByte;
                                    break;
                                case 0x7A: // LD A, D
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.HighByte = RegDE.HighByte;
                                    break;
                                case 0x7B: // LD A, E
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.HighByte = RegDE.LowByte;
                                    break;
                                case 0x7C: // LD A, IXH
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegAF.HighByte = RegIX.HighByte;                                    
                                    break;
                                case 0x7D: // LD A, IXL
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegAF.HighByte = RegIX.LowByte;
                                    break;
                                case 0x7E: // LD A, (IX+d)
                                    totalExecutedCycles += 19; pendingCycles -= 19;
                                    Displacement = (sbyte)ReadOpArg(RegPC.LowWord++);
                                    RegWZ.LowWord = (ushort)(RegIX.LowWord + Displacement);
                                    RegAF.HighByte = ReadMemory(RegWZ.LowWord);
                                    break;
                                case 0x7F: // LD A, A
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x80: // ADD A, B
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableALU[0, RegAF.HighByte, RegBC.HighByte, 0];                                    
                                    break;
                                case 0x81: // ADD A, C
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableALU[0, RegAF.HighByte, RegBC.LowByte, 0];
                                    break;
                                case 0x82: // ADD A, D
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableALU[0, RegAF.HighByte, RegDE.HighByte, 0];
                                    break;
                                case 0x83: // ADD A, E
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableALU[0, RegAF.HighByte, RegDE.LowByte, 0];
                                    break;
                                case 0x84: // ADD A, IXH
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegAF.LowWord = TableALU[0, RegAF.HighByte, RegIX.HighByte, 0];                                    
                                    break;
                                case 0x85: // ADD A, IXL
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegAF.LowWord = TableALU[0, RegAF.HighByte, RegIX.LowByte, 0];
                                    break;
                                case 0x86: // ADD A, (IX+d)
                                    totalExecutedCycles += 19; pendingCycles -= 19;
                                    Displacement = (sbyte)ReadOpArg(RegPC.LowWord++);
                                    RegWZ.LowWord = (ushort)(RegIX.LowWord + Displacement);
                                    RegAF.LowWord = TableALU[0, RegAF.HighByte, ReadMemory(RegWZ.LowWord), 0];
                                    break;
                                case 0x87: // ADD A, A
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableALU[0, RegAF.HighByte, RegAF.HighByte, 0];                                    
                                    break;
                                case 0x88: // ADC A, B
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableALU[1, RegAF.HighByte, RegBC.HighByte, RegFlagC ? 1 : 0];
                                    break;
                                case 0x89: // ADC A, C
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableALU[1, RegAF.HighByte, RegBC.LowByte, RegFlagC ? 1 : 0];
                                    break;
                                case 0x8A: // ADC A, D
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableALU[1, RegAF.HighByte, RegDE.HighByte, RegFlagC ? 1 : 0];
                                    break;
                                case 0x8B: // ADC A, E
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableALU[1, RegAF.HighByte, RegDE.LowByte, RegFlagC ? 1 : 0];
                                    break;
                                case 0x8C: // ADC A, IXH
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegAF.LowWord = TableALU[1, RegAF.HighByte, RegIX.HighByte, RegFlagC ? 1 : 0];                                    
                                    break;
                                case 0x8D: // ADC A, IXL
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegAF.LowWord = TableALU[1, RegAF.HighByte, RegIX.LowByte, RegFlagC ? 1 : 0];
                                    break;
                                case 0x8E: // ADC A, (IX+d)
                                    totalExecutedCycles += 19; pendingCycles -= 19;
                                    Displacement = (sbyte)ReadOpArg(RegPC.LowWord++);
                                    RegWZ.LowWord = (ushort)(RegIX.LowWord + Displacement);
                                    RegAF.LowWord = TableALU[1, RegAF.HighByte, ReadMemory(RegWZ.LowWord), RegFlagC ? 1 : 0];
                                    break;
                                case 0x8F: // ADC A, A
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableALU[1, RegAF.HighByte, RegAF.HighByte, RegFlagC ? 1 : 0];                                    
                                    break;
                                case 0x90: // SUB B
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableALU[2, RegAF.HighByte, RegBC.HighByte, 0];
                                    break;
                                case 0x91: // SUB C
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableALU[2, RegAF.HighByte, RegBC.LowByte, 0];
                                    break;
                                case 0x92: // SUB D
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableALU[2, RegAF.HighByte, RegDE.HighByte, 0];
                                    break;
                                case 0x93: // SUB E
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableALU[2, RegAF.HighByte, RegDE.LowByte, 0];
                                    break;
                                case 0x94: // SUB IXH
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegAF.LowWord = TableALU[2, RegAF.HighByte, RegIX.HighByte, 0];                                    
                                    break;
                                case 0x95: // SUB IXL
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegAF.LowWord = TableALU[2, RegAF.HighByte, RegIX.LowByte, 0];
                                    break;
                                case 0x96: // SUB (IX+d)
                                    totalExecutedCycles += 19; pendingCycles -= 19;
                                    Displacement = (sbyte)ReadOpArg(RegPC.LowWord++);
                                    RegWZ.LowWord = (ushort)(RegIX.LowWord + Displacement);
                                    RegAF.LowWord = TableALU[2, RegAF.HighByte, ReadMemory(RegWZ.LowWord), 0];
                                    break;
                                case 0x97: // SUB A, A
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableALU[2, RegAF.HighByte, RegAF.HighByte, 0];                                    
                                    break;
                                case 0x98: // SBC A, B
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableALU[3, RegAF.HighByte, RegBC.HighByte, RegFlagC ? 1 : 0];
                                    break;
                                case 0x99: // SBC A, C
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableALU[3, RegAF.HighByte, RegBC.LowByte, RegFlagC ? 1 : 0];
                                    break;
                                case 0x9A: // SBC A, D
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableALU[3, RegAF.HighByte, RegDE.HighByte, RegFlagC ? 1 : 0];
                                    break;
                                case 0x9B: // SBC A, E
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableALU[3, RegAF.HighByte, RegDE.LowByte, RegFlagC ? 1 : 0];
                                    break;
                                case 0x9C: // SBC A, IXH
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegAF.LowWord = TableALU[3, RegAF.HighByte, RegIX.HighByte, RegFlagC ? 1 : 0];                                    
                                    break;
                                case 0x9D: // SBC A, IXL
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegAF.LowWord = TableALU[3, RegAF.HighByte, RegIX.LowByte, RegFlagC ? 1 : 0];
                                    break;
                                case 0x9E: // SBC A, (IX+d)
                                    totalExecutedCycles += 19; pendingCycles -= 19;
                                    Displacement = (sbyte)ReadOpArg(RegPC.LowWord++);
                                    RegWZ.LowWord = (ushort)(RegIX.LowWord + Displacement);
                                    RegAF.LowWord = TableALU[3, RegAF.HighByte, ReadMemory(RegWZ.LowWord), RegFlagC ? 1 : 0];
                                    break;
                                case 0x9F: // SBC A, A
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableALU[3, RegAF.HighByte, RegAF.HighByte, RegFlagC ? 1 : 0];                                    
                                    break;
                                case 0xA0: // AND B
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableALU[4, RegAF.HighByte, RegBC.HighByte, 0];
                                    break;
                                case 0xA1: // AND C
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableALU[4, RegAF.HighByte, RegBC.LowByte, 0];
                                    break;
                                case 0xA2: // AND D
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableALU[4, RegAF.HighByte, RegDE.HighByte, 0];
                                    break;
                                case 0xA3: // AND E
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableALU[4, RegAF.HighByte, RegDE.LowByte, 0];
                                    break;
                                case 0xA4: // AND IXH
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegAF.LowWord = TableALU[4, RegAF.HighByte, RegIX.HighByte, 0];                                    
                                    break;
                                case 0xA5: // AND IXL
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegAF.LowWord = TableALU[4, RegAF.HighByte, RegIX.LowByte, 0];
                                    break;
                                case 0xA6: // AND (IX+d)
                                    totalExecutedCycles += 19; pendingCycles -= 19;
                                    Displacement = (sbyte)ReadOpArg(RegPC.LowWord++);
                                    RegWZ.LowWord = (ushort)(RegIX.LowWord + Displacement);
                                    RegAF.LowWord = TableALU[4, RegAF.HighByte, ReadMemory(RegWZ.LowWord), 0];
                                    break;
                                case 0xA7: // AND A
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableALU[4, RegAF.HighByte, RegAF.HighByte, 0];                                    
                                    break;
                                case 0xA8: // XOR B
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableALU[5, RegAF.HighByte, RegBC.HighByte, 0];
                                    break;
                                case 0xA9: // XOR C
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableALU[5, RegAF.HighByte, RegBC.LowByte, 0];
                                    break;
                                case 0xAA: // XOR D
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableALU[5, RegAF.HighByte, RegDE.HighByte, 0];
                                    break;
                                case 0xAB: // XOR E
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableALU[5, RegAF.HighByte, RegDE.LowByte, 0];
                                    break;
                                case 0xAC: // XOR IXH
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegAF.LowWord = TableALU[5, RegAF.HighByte, RegIX.HighByte, 0];                                    
                                    break;
                                case 0xAD: // XOR IXL
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegAF.LowWord = TableALU[5, RegAF.HighByte, RegIX.LowByte, 0];
                                    break;
                                case 0xAE: // XOR (IX+d)
                                    totalExecutedCycles += 19; pendingCycles -= 19;
                                    Displacement = (sbyte)ReadOpArg(RegPC.LowWord++);
                                    RegWZ.LowWord = (ushort)(RegIX.LowWord + Displacement);
                                    RegAF.LowWord = TableALU[5, RegAF.HighByte, ReadMemory(RegWZ.LowWord), 0];
                                    break;
                                case 0xAF: // XOR A
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableALU[5, RegAF.HighByte, RegAF.HighByte, 0];                                    
                                    break;
                                case 0xB0: // OR B
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableALU[6, RegAF.HighByte, RegBC.HighByte, 0];
                                    break;
                                case 0xB1: // OR C
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableALU[6, RegAF.HighByte, RegBC.LowByte, 0];
                                    break;
                                case 0xB2: // OR D
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableALU[6, RegAF.HighByte, RegDE.HighByte, 0];
                                    break;
                                case 0xB3: // OR E
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableALU[6, RegAF.HighByte, RegDE.LowByte, 0];
                                    break;
                                case 0xB4: // OR IXH
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegAF.LowWord = TableALU[6, RegAF.HighByte, RegIX.HighByte, 0];                                    
                                    break;
                                case 0xB5: // OR IXL
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegAF.LowWord = TableALU[6, RegAF.HighByte, RegIX.LowByte, 0];
                                    break;
                                case 0xB6: // OR (IX+d)
                                    totalExecutedCycles += 19; pendingCycles -= 19;
                                    Displacement = (sbyte)ReadOpArg(RegPC.LowWord++);
                                    RegWZ.LowWord = (ushort)(RegIX.LowWord + Displacement);
                                    RegAF.LowWord = TableALU[6, RegAF.HighByte, ReadMemory(RegWZ.LowWord), 0];
                                    break;
                                case 0xB7: // OR A
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableALU[6, RegAF.HighByte, RegAF.HighByte, 0];                                    
                                    break;
                                case 0xB8: // CP B
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableALU[7, RegAF.HighByte, RegBC.HighByte, 0];
                                    break;
                                case 0xB9: // CP C
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableALU[7, RegAF.HighByte, RegBC.LowByte, 0];
                                    break;
                                case 0xBA: // CP D
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableALU[7, RegAF.HighByte, RegDE.HighByte, 0];
                                    break;
                                case 0xBB: // CP E
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableALU[7, RegAF.HighByte, RegDE.LowByte, 0];
                                    break;
                                case 0xBC: // CP IXH
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegAF.LowWord = TableALU[7, RegAF.HighByte, RegIX.HighByte, 0];                                    
                                    break;
                                case 0xBD: // CP IXL
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegAF.LowWord = TableALU[7, RegAF.HighByte, RegIX.LowByte, 0];
                                    break;
                                case 0xBE: // CP (IX+d)
                                    totalExecutedCycles += 19; pendingCycles -= 19;
                                    Displacement = (sbyte)ReadOpArg(RegPC.LowWord++);
                                    RegWZ.LowWord = (ushort)(RegIX.LowWord + Displacement);
                                    RegAF.LowWord = TableALU[7, RegAF.HighByte, ReadMemory(RegWZ.LowWord), 0];
                                    break;
                                case 0xBF: // CP A
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableALU[7, RegAF.HighByte, RegAF.HighByte, 0];                                    
                                    break;
                                case 0xC0: // RET NZ
                                    totalExecutedCycles += 5; pendingCycles -= 5;
                                    if (!RegFlagZ)
                                    {
                                        RegPC.LowByte = ReadMemory(RegSP.LowWord++); RegPC.HighByte = ReadMemory(RegSP.LowWord++);
                                        totalExecutedCycles += 6; pendingCycles -= 6;
                                    }
                                    break;
                                case 0xC1: // POP BC
                                    totalExecutedCycles += 10; pendingCycles -= 10;
                                    RegBC.LowByte = ReadMemory(RegSP.LowWord++); RegBC.HighByte = ReadMemory(RegSP.LowWord++);
                                    break;
                                case 0xC2: // JP NZ, nn
                                    totalExecutedCycles += 10; pendingCycles -= 10;
                                    TUS = (ushort)(ReadMemory(RegPC.LowWord++) + ReadMemory(RegPC.LowWord++) * 256);
                                    if (!RegFlagZ)
                                    {
                                        RegPC.LowWord = TUS;
                                    }
                                    break;
                                case 0xC3: // JP nn
                                    totalExecutedCycles += 10; pendingCycles -= 10;
                                    RegPC.LowWord = (ushort)(ReadMemory(RegPC.LowWord++) + ReadMemory(RegPC.LowWord++) * 256);
                                    break;
                                case 0xC4: // CALL NZ, nn
                                    totalExecutedCycles += 10; pendingCycles -= 10;
                                    TUS = (ushort)(ReadMemory(RegPC.LowWord++) + ReadMemory(RegPC.LowWord++) * 256);
                                    if (!RegFlagZ)
                                    {
                                        totalExecutedCycles += 7; pendingCycles -= 7;
                                        WriteMemory(--RegSP.LowWord, RegPC.HighByte); WriteMemory(--RegSP.LowWord, RegPC.LowByte);
                                        RegPC.LowWord = TUS;
                                    }
                                    break;
                                case 0xC5: // PUSH BC
                                    totalExecutedCycles += 11; pendingCycles -= 11;
                                    WriteMemory(--RegSP.LowWord, RegBC.HighByte); WriteMemory(--RegSP.LowWord, RegBC.LowByte);
                                    break;
                                case 0xC6: // ADD A, n
                                    totalExecutedCycles += 7; pendingCycles -= 7;
                                    RegAF.LowWord = TableALU[0, RegAF.HighByte, ReadMemory(RegPC.LowWord++), 0];
                                    break;
                                case 0xC7: // RST $00
                                    totalExecutedCycles += 11; pendingCycles -= 11;
                                    WriteMemory(--RegSP.LowWord, RegPC.HighByte); WriteMemory(--RegSP.LowWord, RegPC.LowByte);
                                    RegPC.LowWord = 0x00;
                                    break;
                                case 0xC8: // RET Z
                                    totalExecutedCycles += 5; pendingCycles -= 5;
                                    if (RegFlagZ)
                                    {
                                        RegPC.LowByte = ReadMemory(RegSP.LowWord++); RegPC.HighByte = ReadMemory(RegSP.LowWord++);
                                        totalExecutedCycles += 6; pendingCycles -= 6;
                                    }
                                    break;
                                case 0xC9: // RET
                                    totalExecutedCycles += 10; pendingCycles -= 10;
                                    RegPC.LowByte = ReadMemory(RegSP.LowWord++); RegPC.HighByte = ReadMemory(RegSP.LowWord++);
                                    break;
                                case 0xCA: // JP Z, nn
                                    totalExecutedCycles += 10; pendingCycles -= 10;
                                    TUS = (ushort)(ReadMemory(RegPC.LowWord++) + ReadMemory(RegPC.LowWord++) * 256);
                                    if (RegFlagZ)
                                    {
                                        RegPC.LowWord = TUS;
                                    }
                                    break;
                                case 0xCB: // (Prefix)
                                    Displacement = (sbyte)ReadOpArg(RegPC.LowWord++);
                                    //++RegR;
                                    RegWZ.LowWord = (ushort)(RegIX.LowWord + Displacement);
                                    switch (ReadOpArg(RegPC.LowWord++))
                                    {
                                        case 0x00: // RLC (IX+d)→B
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 0, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            RegBC.HighByte = (byte)TUS;
                                            break;
                                        case 0x01: // RLC (IX+d)→C
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 0, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            RegBC.LowByte = (byte)TUS;
                                            break;
                                        case 0x02: // RLC (IX+d)→D
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 0, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            RegDE.HighByte = (byte)TUS;
                                            break;
                                        case 0x03: // RLC (IX+d)→E
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 0, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            RegDE.LowByte = (byte)TUS;
                                            break;
                                        case 0x04: // RLC (IX+d)→H
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 0, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            RegHL.HighByte = (byte)TUS;
                                            break;
                                        case 0x05: // RLC (IX+d)→L
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 0, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            RegHL.LowByte = (byte)TUS;
                                            break;
                                        case 0x06: // RLC (IX+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 0, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            break;
                                        case 0x07: // RLC (IX+d)→A
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 0, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            RegAF.HighByte = (byte)TUS;
                                            break;
                                        case 0x08: // RRC (IX+d)→B
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 1, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            RegBC.HighByte = (byte)TUS;
                                            break;
                                        case 0x09: // RRC (IX+d)→C
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 1, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            RegBC.LowByte = (byte)TUS;
                                            break;
                                        case 0x0A: // RRC (IX+d)→D
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 1, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            RegDE.HighByte = (byte)TUS;
                                            break;
                                        case 0x0B: // RRC (IX+d)→E
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 1, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            RegDE.LowByte = (byte)TUS;
                                            break;
                                        case 0x0C: // RRC (IX+d)→H
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 1, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            RegHL.HighByte = (byte)TUS;
                                            break;
                                        case 0x0D: // RRC (IX+d)→L
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 1, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            RegHL.LowByte = (byte)TUS;
                                            break;
                                        case 0x0E: // RRC (IX+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 1, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            break;
                                        case 0x0F: // RRC (IX+d)→A
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 1, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            RegAF.HighByte = (byte)TUS;
                                            break;
                                        case 0x10: // RL (IX+d)→B
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 2, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            RegBC.HighByte = (byte)TUS;
                                            break;
                                        case 0x11: // RL (IX+d)→C
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 2, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            RegBC.LowByte = (byte)TUS;
                                            break;
                                        case 0x12: // RL (IX+d)→D
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 2, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            RegDE.HighByte = (byte)TUS;
                                            break;
                                        case 0x13: // RL (IX+d)→E
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 2, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            RegDE.LowByte = (byte)TUS;
                                            break;
                                        case 0x14: // RL (IX+d)→H
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 2, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            RegHL.HighByte = (byte)TUS;
                                            break;
                                        case 0x15: // RL (IX+d)→L
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 2, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            RegHL.LowByte = (byte)TUS;
                                            break;
                                        case 0x16: // RL (IX+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 2, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            break;
                                        case 0x17: // RL (IX+d)→A
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 2, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            RegAF.HighByte = (byte)TUS;
                                            break;
                                        case 0x18: // RR (IX+d)→B
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 3, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            RegBC.HighByte = (byte)TUS;
                                            break;
                                        case 0x19: // RR (IX+d)→C
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 3, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            RegBC.LowByte = (byte)TUS;
                                            break;
                                        case 0x1A: // RR (IX+d)→D
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 3, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            RegDE.HighByte = (byte)TUS;
                                            break;
                                        case 0x1B: // RR (IX+d)→E
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 3, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            RegDE.LowByte = (byte)TUS;
                                            break;
                                        case 0x1C: // RR (IX+d)→H
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 3, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            RegHL.HighByte = (byte)TUS;
                                            break;
                                        case 0x1D: // RR (IX+d)→L
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 3, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            RegHL.LowByte = (byte)TUS;
                                            break;
                                        case 0x1E: // RR (IX+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 3, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            break;
                                        case 0x1F: // RR (IX+d)→A
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 3, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            RegAF.HighByte = (byte)TUS;
                                            break;
                                        case 0x20: // SLA (IX+d)→B
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 4, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            RegBC.HighByte = (byte)TUS;
                                            break;
                                        case 0x21: // SLA (IX+d)→C
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 4, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            RegBC.LowByte = (byte)TUS;
                                            break;
                                        case 0x22: // SLA (IX+d)→D
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 4, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            RegDE.HighByte = (byte)TUS;
                                            break;
                                        case 0x23: // SLA (IX+d)→E
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 4, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            RegDE.LowByte = (byte)TUS;
                                            break;
                                        case 0x24: // SLA (IX+d)→H
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 4, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            RegHL.HighByte = (byte)TUS;
                                            break;
                                        case 0x25: // SLA (IX+d)→L
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 4, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            RegHL.LowByte = (byte)TUS;
                                            break;
                                        case 0x26: // SLA (IX+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 4, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            break;
                                        case 0x27: // SLA (IX+d)→A
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 4, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            RegAF.HighByte = (byte)TUS;
                                            break;
                                        case 0x28: // SRA (IX+d)→B
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 5, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            RegBC.HighByte = (byte)TUS;
                                            break;
                                        case 0x29: // SRA (IX+d)→C
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 5, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            RegBC.LowByte = (byte)TUS;
                                            break;
                                        case 0x2A: // SRA (IX+d)→D
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 5, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            RegDE.HighByte = (byte)TUS;
                                            break;
                                        case 0x2B: // SRA (IX+d)→E
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 5, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            RegDE.LowByte = (byte)TUS;
                                            break;
                                        case 0x2C: // SRA (IX+d)→H
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 5, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            RegHL.HighByte = (byte)TUS;
                                            break;
                                        case 0x2D: // SRA (IX+d)→L
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 5, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            RegHL.LowByte = (byte)TUS;
                                            break;
                                        case 0x2E: // SRA (IX+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 5, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            break;
                                        case 0x2F: // SRA (IX+d)→A
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 5, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            RegAF.HighByte = (byte)TUS;
                                            break;
                                        case 0x30: // SL1 (IX+d)→B
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 6, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            RegBC.HighByte = (byte)TUS;
                                            break;
                                        case 0x31: // SL1 (IX+d)→C
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 6, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            RegBC.LowByte = (byte)TUS;
                                            break;
                                        case 0x32: // SL1 (IX+d)→D
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 6, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            RegDE.HighByte = (byte)TUS;
                                            break;
                                        case 0x33: // SL1 (IX+d)→E
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 6, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            RegDE.LowByte = (byte)TUS;
                                            break;
                                        case 0x34: // SL1 (IX+d)→H
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 6, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            RegHL.HighByte = (byte)TUS;
                                            break;
                                        case 0x35: // SL1 (IX+d)→L
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 6, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            RegHL.LowByte = (byte)TUS;
                                            break;
                                        case 0x36: // SL1 (IX+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 6, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            break;
                                        case 0x37: // SL1 (IX+d)→A
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 6, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            RegAF.HighByte = (byte)TUS;
                                            break;
                                        case 0x38: // SRL (IX+d)→B
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 7, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            RegBC.HighByte = (byte)TUS;
                                            break;
                                        case 0x39: // SRL (IX+d)→C
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 7, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            RegBC.LowByte = (byte)TUS;
                                            break;
                                        case 0x3A: // SRL (IX+d)→D
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 7, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            RegDE.HighByte = (byte)TUS;
                                            break;
                                        case 0x3B: // SRL (IX+d)→E
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 7, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            RegDE.LowByte = (byte)TUS;
                                            break;
                                        case 0x3C: // SRL (IX+d)→H
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 7, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            RegHL.HighByte = (byte)TUS;
                                            break;
                                        case 0x3D: // SRL (IX+d)→L
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 7, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            RegHL.LowByte = (byte)TUS;
                                            break;
                                        case 0x3E: // SRL (IX+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 7, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            break;
                                        case 0x3F: // SRL (IX+d)→A
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 7, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            RegAF.HighByte = (byte)TUS;
                                            break;
                                        case 0x40: // BIT 0, (IX+d)
                                        case 0x41: // BIT 0, (IX+d)
                                        case 0x42: // BIT 0, (IX+d)
                                        case 0x43: // BIT 0, (IX+d)
                                        case 0x44: // BIT 0, (IX+d)
                                        case 0x45: // BIT 0, (IX+d)
                                        case 0x46: // BIT 0, (IX+d)
                                        case 0x47: // BIT 0, (IX+d)
                                            totalExecutedCycles += 20; pendingCycles -= 20;
                                            TBOOL = (ReadMemory(RegWZ.LowWord) & 0x01) == 0;
                                            RegFlagN = false;
                                            RegFlagP = TBOOL;
                                            RegFlag3 = ((RegWZ.LowWord >> 8) & 0x08) != 0;
                                            RegFlagH = true;
                                            RegFlag5 = ((RegWZ.LowWord >> 8) & 0x20) != 0;
                                            RegFlagZ = TBOOL;
                                            RegFlagS = false;
                                            break;
                                        case 0x48: // BIT 1, (IX+d)
                                        case 0x49: // BIT 1, (IX+d)
                                        case 0x4A: // BIT 1, (IX+d)
                                        case 0x4B: // BIT 1, (IX+d)
                                        case 0x4C: // BIT 1, (IX+d)
                                        case 0x4D: // BIT 1, (IX+d)
                                        case 0x4E: // BIT 1, (IX+d)
                                        case 0x4F: // BIT 1, (IX+d)
                                            totalExecutedCycles += 20; pendingCycles -= 20;
                                            TBOOL = (ReadMemory(RegWZ.LowWord) & 0x02) == 0;
                                            RegFlagN = false;
                                            RegFlagP = TBOOL;
                                            RegFlag3 = ((RegWZ.LowWord >> 8) & 0x08) != 0;
                                            RegFlagH = true;
                                            RegFlag5 = ((RegWZ.LowWord >> 8) & 0x20) != 0;
                                            RegFlagZ = TBOOL;
                                            RegFlagS = false;
                                            break;
                                        case 0x50: // BIT 2, (IX+d)
                                        case 0x51: // BIT 2, (IX+d)
                                        case 0x52: // BIT 2, (IX+d)
                                        case 0x53: // BIT 2, (IX+d)
                                        case 0x54: // BIT 2, (IX+d)
                                        case 0x55: // BIT 2, (IX+d)
                                        case 0x56: // BIT 2, (IX+d)
                                        case 0x57: // BIT 2, (IX+d)
                                            totalExecutedCycles += 20; pendingCycles -= 20;
                                            TBOOL = (ReadMemory(RegWZ.LowWord) & 0x04) == 0;
                                            RegFlagN = false;
                                            RegFlagP = TBOOL;
                                            RegFlag3 = ((RegWZ.LowWord >> 8) & 0x08) != 0;
                                            RegFlagH = true;
                                            RegFlag5 = ((RegWZ.LowWord >> 8) & 0x20) != 0;
                                            RegFlagZ = TBOOL;
                                            RegFlagS = false;
                                            break;
                                        case 0x58: // BIT 3, (IX+d)
                                        case 0x59: // BIT 3, (IX+d)
                                        case 0x5A: // BIT 3, (IX+d)
                                        case 0x5B: // BIT 3, (IX+d)
                                        case 0x5C: // BIT 3, (IX+d)
                                        case 0x5D: // BIT 3, (IX+d)
                                        case 0x5E: // BIT 3, (IX+d)
                                        case 0x5F: // BIT 3, (IX+d)
                                            totalExecutedCycles += 20; pendingCycles -= 20;
                                            TBOOL = (ReadMemory(RegWZ.LowWord) & 0x08) == 0;
                                            RegFlagN = false;
                                            RegFlagP = TBOOL;
                                            RegFlag3 = ((RegWZ.LowWord >> 8) & 0x08) != 0;
                                            RegFlagH = true;
                                            RegFlag5 = ((RegWZ.LowWord >> 8) & 0x20) != 0;
                                            RegFlagZ = TBOOL;
                                            RegFlagS = false;
                                            break;
                                        case 0x60: // BIT 4, (IX+d)
                                        case 0x61: // BIT 4, (IX+d)
                                        case 0x62: // BIT 4, (IX+d)
                                        case 0x63: // BIT 4, (IX+d)
                                        case 0x64: // BIT 4, (IX+d)
                                        case 0x65: // BIT 4, (IX+d)
                                        case 0x66: // BIT 4, (IX+d)
                                        case 0x67: // BIT 4, (IX+d)
                                            totalExecutedCycles += 20; pendingCycles -= 20;
                                            TBOOL = (ReadMemory(RegWZ.LowWord) & 0x10) == 0;
                                            RegFlagN = false;
                                            RegFlagP = TBOOL;
                                            RegFlag3 = ((RegWZ.LowWord >> 8) & 0x08) != 0;
                                            RegFlagH = true;
                                            RegFlag5 = ((RegWZ.LowWord >> 8) & 0x20) != 0;
                                            RegFlagZ = TBOOL;
                                            RegFlagS = false;
                                            break;
                                        case 0x68: // BIT 5, (IX+d)
                                        case 0x69: // BIT 5, (IX+d)
                                        case 0x6A: // BIT 5, (IX+d)
                                        case 0x6B: // BIT 5, (IX+d)
                                        case 0x6C: // BIT 5, (IX+d)
                                        case 0x6D: // BIT 5, (IX+d)
                                        case 0x6E: // BIT 5, (IX+d)
                                        case 0x6F: // BIT 5, (IX+d)
                                            totalExecutedCycles += 20; pendingCycles -= 20;
                                            TBOOL = (ReadMemory(RegWZ.LowWord) & 0x20) == 0;
                                            RegFlagN = false;
                                            RegFlagP = TBOOL;
                                            RegFlag3 = ((RegWZ.LowWord >> 8) & 0x08) != 0;
                                            RegFlagH = true;
                                            RegFlag5 = ((RegWZ.LowWord >> 8) & 0x20) != 0;
                                            RegFlagZ = TBOOL;
                                            RegFlagS = false;
                                            break;
                                        case 0x70: // BIT 6, (IX+d)
                                        case 0x71: // BIT 6, (IX+d)
                                        case 0x72: // BIT 6, (IX+d)
                                        case 0x73: // BIT 6, (IX+d)
                                        case 0x74: // BIT 6, (IX+d)
                                        case 0x75: // BIT 6, (IX+d)
                                        case 0x76: // BIT 6, (IX+d)
                                        case 0x77: // BIT 6, (IX+d)
                                            totalExecutedCycles += 20; pendingCycles -= 20;
                                            TBOOL = (ReadMemory(RegWZ.LowWord) & 0x40) == 0;
                                            RegFlagN = false;
                                            RegFlagP = TBOOL;
                                            RegFlag3 = ((RegWZ.LowWord >> 8) & 0x08) != 0;
                                            RegFlagH = true;
                                            RegFlag5 = ((RegWZ.LowWord >> 8) & 0x20) != 0;
                                            RegFlagZ = TBOOL;
                                            RegFlagS = false;
                                            break;
                                        case 0x78: // BIT 7, (IX+d)
                                        case 0x79: // BIT 7, (IX+d)
                                        case 0x7A: // BIT 7, (IX+d)
                                        case 0x7B: // BIT 7, (IX+d)
                                        case 0x7C: // BIT 7, (IX+d)
                                        case 0x7D: // BIT 7, (IX+d)
                                        case 0x7E: // BIT 7, (IX+d)
                                        case 0x7F: // BIT 7, (IX+d)
                                            totalExecutedCycles += 20; pendingCycles -= 20;
                                            TBOOL = (ReadMemory(RegWZ.LowWord) & 0x80) == 0;
                                            RegFlagN = false;
                                            RegFlagP = TBOOL;
                                            RegFlag3 = ((RegWZ.LowWord >> 8) & 0x08) != 0;
                                            RegFlagH = true;
                                            RegFlag5 = ((RegWZ.LowWord >> 8) & 0x20) != 0;
                                            RegFlagZ = TBOOL;
                                            RegFlagS = !TBOOL;
                                            break;
                                        case 0x80: // RES 0, (IX+d)→B
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegBC.HighByte = (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x01));
                                            WriteMemory(RegWZ.LowWord, RegBC.HighByte);
                                            break;
                                        case 0x81: // RES 0, (IX+d)→C
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegBC.LowByte = (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x01));
                                            WriteMemory(RegWZ.LowWord, RegBC.LowByte);
                                            break;
                                        case 0x82: // RES 0, (IX+d)→D
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegDE.HighByte = (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x01));
                                            WriteMemory(RegWZ.LowWord, RegDE.HighByte);
                                            break;
                                        case 0x83: // RES 0, (IX+d)→E
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegDE.LowByte = (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x01));
                                            WriteMemory(RegWZ.LowWord, RegDE.LowByte);
                                            break;
                                        case 0x84: // RES 0, (IX+d)→H
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegHL.HighByte = (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x01));
                                            WriteMemory(RegWZ.LowWord, RegHL.HighByte);
                                            break;
                                        case 0x85: // RES 0, (IX+d)→L
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegHL.LowByte = (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x01));
                                            WriteMemory(RegWZ.LowWord, RegHL.LowByte);
                                            break;
                                        case 0x86: // RES 0, (IX+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x01)));
                                            break;
                                        case 0x87: // RES 0, (IX+d)→A
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegAF.HighByte = (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x01));
                                            WriteMemory(RegWZ.LowWord, RegAF.HighByte);
                                            break;
                                        case 0x88: // RES 1, (IX+d)→B
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegBC.HighByte = (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x02));
                                            WriteMemory(RegWZ.LowWord, RegBC.HighByte);
                                            break;
                                        case 0x89: // RES 1, (IX+d)→C
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegBC.LowByte = (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x02));
                                            WriteMemory(RegWZ.LowWord, RegBC.LowByte);
                                            break;
                                        case 0x8A: // RES 1, (IX+d)→D
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegDE.HighByte = (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x02));
                                            WriteMemory(RegWZ.LowWord, RegDE.HighByte);
                                            break;
                                        case 0x8B: // RES 1, (IX+d)→E
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegDE.LowByte = (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x02));
                                            WriteMemory(RegWZ.LowWord, RegDE.LowByte);
                                            break;
                                        case 0x8C: // RES 1, (IX+d)→H
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegHL.HighByte = (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x02));
                                            WriteMemory(RegWZ.LowWord, RegHL.HighByte);
                                            break;
                                        case 0x8D: // RES 1, (IX+d)→L
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegHL.LowByte = (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x02));
                                            WriteMemory(RegWZ.LowWord, RegHL.LowByte);
                                            break;
                                        case 0x8E: // RES 1, (IX+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x02)));
                                            break;
                                        case 0x8F: // RES 1, (IX+d)→A
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegAF.HighByte = (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x02));
                                            WriteMemory(RegWZ.LowWord, RegAF.HighByte);
                                            break;
                                        case 0x90: // RES 2, (IX+d)→B
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegBC.HighByte = (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x04));
                                            WriteMemory(RegWZ.LowWord, RegBC.HighByte);
                                            break;
                                        case 0x91: // RES 2, (IX+d)→C
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegBC.LowByte = (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x04));
                                            WriteMemory(RegWZ.LowWord, RegBC.LowByte);
                                            break;
                                        case 0x92: // RES 2, (IX+d)→D
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegDE.HighByte = (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x04));
                                            WriteMemory(RegWZ.LowWord, RegDE.HighByte);
                                            break;
                                        case 0x93: // RES 2, (IX+d)→E
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegDE.LowByte = (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x04));
                                            WriteMemory(RegWZ.LowWord, RegDE.LowByte);
                                            break;
                                        case 0x94: // RES 2, (IX+d)→H
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegHL.HighByte = (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x04));
                                            WriteMemory(RegWZ.LowWord, RegHL.HighByte);
                                            break;
                                        case 0x95: // RES 2, (IX+d)→L
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegHL.LowByte = (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x04));
                                            WriteMemory(RegWZ.LowWord, RegHL.LowByte);
                                            break;
                                        case 0x96: // RES 2, (IX+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x04)));
                                            break;
                                        case 0x97: // RES 2, (IX+d)→A
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegAF.HighByte = (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x04));
                                            WriteMemory(RegWZ.LowWord, RegAF.HighByte);
                                            break;
                                        case 0x98: // RES 3, (IX+d)→B
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegBC.HighByte = (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x08));
                                            WriteMemory(RegWZ.LowWord, RegBC.HighByte);
                                            break;
                                        case 0x99: // RES 3, (IX+d)→C
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegBC.LowByte = (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x08));
                                            WriteMemory(RegWZ.LowWord, RegBC.LowByte);
                                            break;
                                        case 0x9A: // RES 3, (IX+d)→D
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegDE.HighByte = (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x08));
                                            WriteMemory(RegWZ.LowWord, RegDE.HighByte);
                                            break;
                                        case 0x9B: // RES 3, (IX+d)→E
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegDE.LowByte = (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x08));
                                            WriteMemory(RegWZ.LowWord, RegDE.LowByte);
                                            break;
                                        case 0x9C: // RES 3, (IX+d)→H
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegHL.HighByte = (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x08));
                                            WriteMemory(RegWZ.LowWord, RegHL.HighByte);
                                            break;
                                        case 0x9D: // RES 3, (IX+d)→L
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegHL.LowByte = (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x08));
                                            WriteMemory(RegWZ.LowWord, RegHL.LowByte);
                                            break;
                                        case 0x9E: // RES 3, (IX+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x08)));
                                            break;
                                        case 0x9F: // RES 3, (IX+d)→A
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegAF.HighByte = (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x08));
                                            WriteMemory(RegWZ.LowWord, RegAF.HighByte);
                                            break;
                                        case 0xA0: // RES 4, (IX+d)→B
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegBC.HighByte = (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x10));
                                            WriteMemory(RegWZ.LowWord, RegBC.HighByte);
                                            break;
                                        case 0xA1: // RES 4, (IX+d)→C
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegBC.LowByte = (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x10));
                                            WriteMemory(RegWZ.LowWord, RegBC.LowByte);
                                            break;
                                        case 0xA2: // RES 4, (IX+d)→D
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegDE.HighByte = (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x10));
                                            WriteMemory(RegWZ.LowWord, RegDE.HighByte);
                                            break;
                                        case 0xA3: // RES 4, (IX+d)→E
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegDE.LowByte = (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x10));
                                            WriteMemory(RegWZ.LowWord, RegDE.LowByte);
                                            break;
                                        case 0xA4: // RES 4, (IX+d)→H
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegHL.HighByte = (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x10));
                                            WriteMemory(RegWZ.LowWord, RegHL.HighByte);
                                            break;
                                        case 0xA5: // RES 4, (IX+d)→L
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegHL.LowByte = (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x10));
                                            WriteMemory(RegWZ.LowWord, RegHL.LowByte);
                                            break;
                                        case 0xA6: // RES 4, (IX+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x10)));
                                            break;
                                        case 0xA7: // RES 4, (IX+d)→A
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegAF.HighByte = (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x10));
                                            WriteMemory(RegWZ.LowWord, RegAF.HighByte);
                                            break;
                                        case 0xA8: // RES 5, (IX+d)→B
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegBC.HighByte = (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x20));
                                            WriteMemory(RegWZ.LowWord, RegBC.HighByte);
                                            break;
                                        case 0xA9: // RES 5, (IX+d)→C
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegBC.LowByte = (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x20));
                                            WriteMemory(RegWZ.LowWord, RegBC.LowByte);
                                            break;
                                        case 0xAA: // RES 5, (IX+d)→D
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegDE.HighByte = (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x20));
                                            WriteMemory(RegWZ.LowWord, RegDE.HighByte);
                                            break;
                                        case 0xAB: // RES 5, (IX+d)→E
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegDE.LowByte = (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x20));
                                            WriteMemory(RegWZ.LowWord, RegDE.LowByte);
                                            break;
                                        case 0xAC: // RES 5, (IX+d)→H
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegHL.HighByte = (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x20));
                                            WriteMemory(RegWZ.LowWord, RegHL.HighByte);
                                            break;
                                        case 0xAD: // RES 5, (IX+d)→L
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegHL.LowByte = (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x20));
                                            WriteMemory(RegWZ.LowWord, RegHL.LowByte);
                                            break;
                                        case 0xAE: // RES 5, (IX+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x20)));
                                            break;
                                        case 0xAF: // RES 5, (IX+d)→A
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegAF.HighByte = (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x20));
                                            WriteMemory(RegWZ.LowWord, RegAF.HighByte);
                                            break;
                                        case 0xB0: // RES 6, (IX+d)→B
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegBC.HighByte = (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x40));
                                            WriteMemory(RegWZ.LowWord, RegBC.HighByte);
                                            break;
                                        case 0xB1: // RES 6, (IX+d)→C
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegBC.LowByte = (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x40));
                                            WriteMemory(RegWZ.LowWord, RegBC.LowByte);
                                            break;
                                        case 0xB2: // RES 6, (IX+d)→D
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegDE.HighByte = (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x40));
                                            WriteMemory(RegWZ.LowWord, RegDE.HighByte);
                                            break;
                                        case 0xB3: // RES 6, (IX+d)→E
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegDE.LowByte = (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x40));
                                            WriteMemory(RegWZ.LowWord, RegDE.LowByte);
                                            break;
                                        case 0xB4: // RES 6, (IX+d)→H
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegHL.HighByte = (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x40));
                                            WriteMemory(RegWZ.LowWord, RegHL.HighByte);
                                            break;
                                        case 0xB5: // RES 6, (IX+d)→L
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegHL.LowByte = (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x40));
                                            WriteMemory(RegWZ.LowWord, RegHL.LowByte);
                                            break;
                                        case 0xB6: // RES 6, (IX+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x40)));
                                            break;
                                        case 0xB7: // RES 6, (IX+d)→A
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegAF.HighByte = (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x40));
                                            WriteMemory(RegWZ.LowWord, RegAF.HighByte);
                                            break;
                                        case 0xB8: // RES 7, (IX+d)→B
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegBC.HighByte = (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x80));
                                            WriteMemory(RegWZ.LowWord, RegBC.HighByte);
                                            break;
                                        case 0xB9: // RES 7, (IX+d)→C
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegBC.LowByte = (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x80));
                                            WriteMemory(RegWZ.LowWord, RegBC.LowByte);
                                            break;
                                        case 0xBA: // RES 7, (IX+d)→D
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegDE.HighByte = (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x80));
                                            WriteMemory(RegWZ.LowWord, RegDE.HighByte);
                                            break;
                                        case 0xBB: // RES 7, (IX+d)→E
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegDE.LowByte = (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x80));
                                            WriteMemory(RegWZ.LowWord, RegDE.LowByte);
                                            break;
                                        case 0xBC: // RES 7, (IX+d)→H
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegHL.HighByte = (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x80));
                                            WriteMemory(RegWZ.LowWord, RegHL.HighByte);
                                            break;
                                        case 0xBD: // RES 7, (IX+d)→L
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegHL.LowByte = (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x80));
                                            WriteMemory(RegWZ.LowWord, RegHL.LowByte);
                                            break;
                                        case 0xBE: // RES 7, (IX+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x80)));
                                            break;
                                        case 0xBF: // RES 7, (IX+d)→A
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegAF.HighByte = (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x80));
                                            WriteMemory(RegWZ.LowWord, RegAF.HighByte);
                                            break;
                                        case 0xC0: // SET 0, (IX+d)→B
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegBC.HighByte = (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x01));
                                            WriteMemory(RegWZ.LowWord, RegBC.HighByte);
                                            break;
                                        case 0xC1: // SET 0, (IX+d)→C
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegBC.LowByte = (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x01));
                                            WriteMemory(RegWZ.LowWord, RegBC.LowByte);
                                            break;
                                        case 0xC2: // SET 0, (IX+d)→D
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegDE.HighByte = (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x01));
                                            WriteMemory(RegWZ.LowWord, RegDE.HighByte);
                                            break;
                                        case 0xC3: // SET 0, (IX+d)→E
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegDE.LowByte = (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x01));
                                            WriteMemory(RegWZ.LowWord, RegDE.LowByte);
                                            break;
                                        case 0xC4: // SET 0, (IX+d)→H
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegHL.HighByte = (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x01));
                                            WriteMemory(RegWZ.LowWord, RegHL.HighByte);
                                            break;
                                        case 0xC5: // SET 0, (IX+d)→L
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegHL.LowByte = (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x01));
                                            WriteMemory(RegWZ.LowWord, RegHL.LowByte);
                                            break;
                                        case 0xC6: // SET 0, (IX+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x01)));
                                            break;
                                        case 0xC7: // SET 0, (IX+d)→A
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegAF.HighByte = (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x01));
                                            WriteMemory(RegWZ.LowWord, RegAF.HighByte);
                                            break;
                                        case 0xC8: // SET 1, (IX+d)→B
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegBC.HighByte = (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x02));
                                            WriteMemory(RegWZ.LowWord, RegBC.HighByte);
                                            break;
                                        case 0xC9: // SET 1, (IX+d)→C
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegBC.LowByte = (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x02));
                                            WriteMemory(RegWZ.LowWord, RegBC.LowByte);
                                            break;
                                        case 0xCA: // SET 1, (IX+d)→D
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegDE.HighByte = (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x02));
                                            WriteMemory(RegWZ.LowWord, RegDE.HighByte);
                                            break;
                                        case 0xCB: // SET 1, (IX+d)→E
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegDE.LowByte = (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x02));
                                            WriteMemory(RegWZ.LowWord, RegDE.LowByte);
                                            break;
                                        case 0xCC: // SET 1, (IX+d)→H
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegHL.HighByte = (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x02));
                                            WriteMemory(RegWZ.LowWord, RegHL.HighByte);
                                            break;
                                        case 0xCD: // SET 1, (IX+d)→L
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegHL.LowByte = (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x02));
                                            WriteMemory(RegWZ.LowWord, RegHL.LowByte);
                                            break;
                                        case 0xCE: // SET 1, (IX+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x02)));
                                            break;
                                        case 0xCF: // SET 1, (IX+d)→A
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegAF.HighByte = (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x02));
                                            WriteMemory(RegWZ.LowWord, RegAF.HighByte);
                                            break;
                                        case 0xD0: // SET 2, (IX+d)→B
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegBC.HighByte = (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x04));
                                            WriteMemory(RegWZ.LowWord, RegBC.HighByte);
                                            break;
                                        case 0xD1: // SET 2, (IX+d)→C
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegBC.LowByte = (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x04));
                                            WriteMemory(RegWZ.LowWord, RegBC.LowByte);
                                            break;
                                        case 0xD2: // SET 2, (IX+d)→D
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegDE.HighByte = (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x04));
                                            WriteMemory(RegWZ.LowWord, RegDE.HighByte);
                                            break;
                                        case 0xD3: // SET 2, (IX+d)→E
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegDE.LowByte = (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x04));
                                            WriteMemory(RegWZ.LowWord, RegDE.LowByte);
                                            break;
                                        case 0xD4: // SET 2, (IX+d)→H
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegHL.HighByte = (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x04));
                                            WriteMemory(RegWZ.LowWord, RegHL.HighByte);
                                            break;
                                        case 0xD5: // SET 2, (IX+d)→L
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegHL.LowByte = (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x04));
                                            WriteMemory(RegWZ.LowWord, RegHL.LowByte);
                                            break;
                                        case 0xD6: // SET 2, (IX+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x04)));
                                            break;
                                        case 0xD7: // SET 2, (IX+d)→A
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegAF.HighByte = (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x04));
                                            WriteMemory(RegWZ.LowWord, RegAF.HighByte);
                                            break;
                                        case 0xD8: // SET 3, (IX+d)→B
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegBC.HighByte = (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x08));
                                            WriteMemory(RegWZ.LowWord, RegBC.HighByte);
                                            break;
                                        case 0xD9: // SET 3, (IX+d)→C
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegBC.LowByte = (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x08));
                                            WriteMemory(RegWZ.LowWord, RegBC.LowByte);
                                            break;
                                        case 0xDA: // SET 3, (IX+d)→D
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegDE.HighByte = (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x08));
                                            WriteMemory(RegWZ.LowWord, RegDE.HighByte);
                                            break;
                                        case 0xDB: // SET 3, (IX+d)→E
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegDE.LowByte = (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x08));
                                            WriteMemory(RegWZ.LowWord, RegDE.LowByte);
                                            break;
                                        case 0xDC: // SET 3, (IX+d)→H
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegHL.HighByte = (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x08));
                                            WriteMemory(RegWZ.LowWord, RegHL.HighByte);
                                            break;
                                        case 0xDD: // SET 3, (IX+d)→L
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegHL.LowByte = (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x08));
                                            WriteMemory(RegWZ.LowWord, RegHL.LowByte);
                                            break;
                                        case 0xDE: // SET 3, (IX+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x08)));
                                            break;
                                        case 0xDF: // SET 3, (IX+d)→A
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegAF.HighByte = (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x08));
                                            WriteMemory(RegWZ.LowWord, RegAF.HighByte);
                                            break;
                                        case 0xE0: // SET 4, (IX+d)→B
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegBC.HighByte = (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x10));
                                            WriteMemory(RegWZ.LowWord, RegBC.HighByte);
                                            break;
                                        case 0xE1: // SET 4, (IX+d)→C
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegBC.LowByte = (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x10));
                                            WriteMemory(RegWZ.LowWord, RegBC.LowByte);
                                            break;
                                        case 0xE2: // SET 4, (IX+d)→D
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegDE.HighByte = (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x10));
                                            WriteMemory(RegWZ.LowWord, RegDE.HighByte);
                                            break;
                                        case 0xE3: // SET 4, (IX+d)→E
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegDE.LowByte = (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x10));
                                            WriteMemory(RegWZ.LowWord, RegDE.LowByte);
                                            break;
                                        case 0xE4: // SET 4, (IX+d)→H
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegHL.HighByte = (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x10));
                                            WriteMemory(RegWZ.LowWord, RegHL.HighByte);
                                            break;
                                        case 0xE5: // SET 4, (IX+d)→L
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegHL.LowByte = (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x10));
                                            WriteMemory(RegWZ.LowWord, RegHL.LowByte);
                                            break;
                                        case 0xE6: // SET 4, (IX+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x10)));
                                            break;
                                        case 0xE7: // SET 4, (IX+d)→A
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegAF.HighByte = (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x10));
                                            WriteMemory(RegWZ.LowWord, RegAF.HighByte);
                                            break;
                                        case 0xE8: // SET 5, (IX+d)→B
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegBC.HighByte = (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x20));
                                            WriteMemory(RegWZ.LowWord, RegBC.HighByte);
                                            break;
                                        case 0xE9: // SET 5, (IX+d)→C
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegBC.LowByte = (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x20));
                                            WriteMemory(RegWZ.LowWord, RegBC.LowByte);
                                            break;
                                        case 0xEA: // SET 5, (IX+d)→D
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegDE.HighByte = (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x20));
                                            WriteMemory(RegWZ.LowWord, RegDE.HighByte);
                                            break;
                                        case 0xEB: // SET 5, (IX+d)→E
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegDE.LowByte = (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x20));
                                            WriteMemory(RegWZ.LowWord, RegDE.LowByte);
                                            break;
                                        case 0xEC: // SET 5, (IX+d)→H
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegHL.HighByte = (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x20));
                                            WriteMemory(RegWZ.LowWord, RegHL.HighByte);
                                            break;
                                        case 0xED: // SET 5, (IX+d)→L
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegHL.LowByte = (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x20));
                                            WriteMemory(RegWZ.LowWord, RegHL.LowByte);
                                            break;
                                        case 0xEE: // SET 5, (IX+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x20)));
                                            break;
                                        case 0xEF: // SET 5, (IX+d)→A
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegAF.HighByte = (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x20));
                                            WriteMemory(RegWZ.LowWord, RegAF.HighByte);
                                            break;
                                        case 0xF0: // SET 6, (IX+d)→B
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegBC.HighByte = (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x40));
                                            WriteMemory(RegWZ.LowWord, RegBC.HighByte);
                                            break;
                                        case 0xF1: // SET 6, (IX+d)→C
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegBC.LowByte = (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x40));
                                            WriteMemory(RegWZ.LowWord, RegBC.LowByte);
                                            break;
                                        case 0xF2: // SET 6, (IX+d)→D
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegDE.HighByte = (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x40));
                                            WriteMemory(RegWZ.LowWord, RegDE.HighByte);
                                            break;
                                        case 0xF3: // SET 6, (IX+d)→E
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegDE.LowByte = (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x40));
                                            WriteMemory(RegWZ.LowWord, RegDE.LowByte);
                                            break;
                                        case 0xF4: // SET 6, (IX+d)→H
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegHL.HighByte = (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x40));
                                            WriteMemory(RegWZ.LowWord, RegHL.HighByte);
                                            break;
                                        case 0xF5: // SET 6, (IX+d)→L
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegHL.LowByte = (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x40));
                                            WriteMemory(RegWZ.LowWord, RegHL.LowByte);
                                            break;
                                        case 0xF6: // SET 6, (IX+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x40)));
                                            break;
                                        case 0xF7: // SET 6, (IX+d)→A
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegAF.HighByte = (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x40));
                                            WriteMemory(RegWZ.LowWord, RegAF.HighByte);
                                            break;
                                        case 0xF8: // SET 7, (IX+d)→B
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegBC.HighByte = (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x80));
                                            WriteMemory(RegWZ.LowWord, RegBC.HighByte);
                                            break;
                                        case 0xF9: // SET 7, (IX+d)→C
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegBC.LowByte = (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x80));
                                            WriteMemory(RegWZ.LowWord, RegBC.LowByte);
                                            break;
                                        case 0xFA: // SET 7, (IX+d)→D
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegDE.HighByte = (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x80));
                                            WriteMemory(RegWZ.LowWord, RegDE.HighByte);
                                            break;
                                        case 0xFB: // SET 7, (IX+d)→E
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegDE.LowByte = (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x80));
                                            WriteMemory(RegWZ.LowWord, RegDE.LowByte);
                                            break;
                                        case 0xFC: // SET 7, (IX+d)→H
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegHL.HighByte = (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x80));
                                            WriteMemory(RegWZ.LowWord, RegHL.HighByte);
                                            break;
                                        case 0xFD: // SET 7, (IX+d)→L
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegHL.LowByte = (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x80));
                                            WriteMemory(RegWZ.LowWord, RegHL.LowByte);
                                            break;
                                        case 0xFE: // SET 7, (IX+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x80)));
                                            break;
                                        case 0xFF: // SET 7, (IX+d)→A
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            RegAF.HighByte = (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x80));
                                            WriteMemory(RegWZ.LowWord, RegAF.HighByte);
                                            break;
                                    }
                                    break;
                                case 0xCC: // CALL Z, nn
                                    totalExecutedCycles += 10; pendingCycles -= 10;
                                    TUS = (ushort)(ReadMemory(RegPC.LowWord++) + ReadMemory(RegPC.LowWord++) * 256);
                                    if (RegFlagZ)
                                    {
                                        totalExecutedCycles += 7; pendingCycles -= 7;
                                        WriteMemory(--RegSP.LowWord, RegPC.HighByte); WriteMemory(--RegSP.LowWord, RegPC.LowByte);
                                        RegPC.LowWord = TUS;
                                    }
                                    break;
                                case 0xCD: // CALL nn
                                    totalExecutedCycles += 17; pendingCycles -= 17;
                                    TUS = (ushort)(ReadMemory(RegPC.LowWord++) + ReadMemory(RegPC.LowWord++) * 256);
                                    WriteMemory(--RegSP.LowWord, RegPC.HighByte); WriteMemory(--RegSP.LowWord, RegPC.LowByte);
                                    RegPC.LowWord = TUS;
                                    break;
                                case 0xCE: // ADC A, n
                                    totalExecutedCycles += 7; pendingCycles -= 7;
                                    RegAF.LowWord = TableALU[1, RegAF.HighByte, ReadMemory(RegPC.LowWord++), RegFlagC ? 1 : 0];
                                    break;
                                case 0xCF: // RST $08
                                    totalExecutedCycles += 11; pendingCycles -= 11;
                                    WriteMemory(--RegSP.LowWord, RegPC.HighByte); WriteMemory(--RegSP.LowWord, RegPC.LowByte);
                                    RegPC.LowWord = 0x08;
                                    break;
                                case 0xD0: // RET NC
                                    totalExecutedCycles += 5; pendingCycles -= 5;
                                    if (!RegFlagC)
                                    {
                                        totalExecutedCycles += 6; pendingCycles -= 6;
                                        RegPC.LowByte = ReadMemory(RegSP.LowWord++); RegPC.HighByte = ReadMemory(RegSP.LowWord++);
                                    }
                                    break;
                                case 0xD1: // POP DE
                                    totalExecutedCycles += 10; pendingCycles -= 10;
                                    RegDE.LowByte = ReadMemory(RegSP.LowWord++); RegDE.HighByte = ReadMemory(RegSP.LowWord++);
                                    break;
                                case 0xD2: // JP NC, nn
                                    totalExecutedCycles += 10; pendingCycles -= 10;
                                    TUS = (ushort)(ReadMemory(RegPC.LowWord++) + ReadMemory(RegPC.LowWord++) * 256);
                                    if (!RegFlagC)
                                    {
                                        RegPC.LowWord = TUS;
                                    }
                                    break;
                                case 0xD3: // OUT n, A
                                    totalExecutedCycles += 11; pendingCycles -= 11;
                                    WriteHardware(ReadMemory(RegPC.LowWord++), RegAF.HighByte);
                                    break;
                                case 0xD4: // CALL NC, nn
                                    totalExecutedCycles += 10; pendingCycles -= 10;
                                    TUS = (ushort)(ReadMemory(RegPC.LowWord++) + ReadMemory(RegPC.LowWord++) * 256);
                                    if (!RegFlagC)
                                    {
                                        totalExecutedCycles += 7; pendingCycles -= 7;
                                        WriteMemory(--RegSP.LowWord, RegPC.HighByte); WriteMemory(--RegSP.LowWord, RegPC.LowByte);
                                        RegPC.LowWord = TUS;
                                    }
                                    break;
                                case 0xD5: // PUSH DE
                                    totalExecutedCycles += 11; pendingCycles -= 11;
                                    WriteMemory(--RegSP.LowWord, RegDE.HighByte); WriteMemory(--RegSP.LowWord, RegDE.LowByte);
                                    break;
                                case 0xD6: // SUB n
                                    totalExecutedCycles += 7; pendingCycles -= 7;
                                    RegAF.LowWord = TableALU[2, RegAF.HighByte, ReadMemory(RegPC.LowWord++), 0];
                                    break;
                                case 0xD7: // RST $10
                                    totalExecutedCycles += 11; pendingCycles -= 11;
                                    WriteMemory(--RegSP.LowWord, RegPC.HighByte); WriteMemory(--RegSP.LowWord, RegPC.LowByte);
                                    RegPC.LowWord = 0x10;
                                    break;
                                case 0xD8: // RET C
                                    totalExecutedCycles += 5; pendingCycles -= 5;
                                    if (RegFlagC)
                                    {
                                        totalExecutedCycles += 6; pendingCycles -= 6;
                                        RegPC.LowByte = ReadMemory(RegSP.LowWord++); RegPC.HighByte = ReadMemory(RegSP.LowWord++);
                                    }
                                    break;
                                case 0xD9: // EXX
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    TUS = RegBC.LowWord; RegBC.LowWord = RegAltBC.LowWord; RegAltBC.LowWord = TUS;
                                    TUS = RegDE.LowWord; RegDE.LowWord = RegAltDE.LowWord; RegAltDE.LowWord = TUS;
                                    TUS = RegHL.LowWord; RegHL.LowWord = RegAltHL.LowWord; RegAltHL.LowWord = TUS;
                                    break;
                                case 0xDA: // JP C, nn
                                    totalExecutedCycles += 10; pendingCycles -= 10;
                                    TUS = (ushort)(ReadMemory(RegPC.LowWord++) + ReadMemory(RegPC.LowWord++) * 256);
                                    if (RegFlagC)
                                    {
                                        RegPC.LowWord = TUS;
                                    }
                                    break;
                                case 0xDB: // IN A, n
                                    totalExecutedCycles += 11; pendingCycles -= 11;
                                    TUS = (ushort)(ReadMemory(RegPC.LowWord++) | (RegAF.HighByte << 8));
                                    RegAF.HighByte = ReadHardware(TUS);
                                    RegWZ.LowWord = (ushort)(TUS + 1);
                                    break;
                                case 0xDC: // CALL C, nn
                                    totalExecutedCycles += 10; pendingCycles -= 10;
                                    TUS = (ushort)(ReadMemory(RegPC.LowWord++) + ReadMemory(RegPC.LowWord++) * 256);
                                    if (RegFlagC)
                                    {
                                        totalExecutedCycles += 7; pendingCycles -= 7;
                                        WriteMemory(--RegSP.LowWord, RegPC.HighByte); WriteMemory(--RegSP.LowWord, RegPC.LowByte);
                                        RegPC.LowWord = TUS;
                                    }
                                    break;
                                case 0xDD: // <-
                                    // Invalid sequence.
                                    totalExecutedCycles += 1337; pendingCycles -= 1337;
                                    break;
                                case 0xDE: // SBC A, n
                                    totalExecutedCycles += 7; pendingCycles -= 7;
                                    RegAF.LowWord = TableALU[3, RegAF.HighByte, ReadMemory(RegPC.LowWord++), RegFlagC ? 1 : 0];
                                    break;
                                case 0xDF: // RST $18
                                    totalExecutedCycles += 11; pendingCycles -= 11;
                                    WriteMemory(--RegSP.LowWord, RegPC.HighByte); WriteMemory(--RegSP.LowWord, RegPC.LowByte);
                                    RegPC.LowWord = 0x18;
                                    break;
                                case 0xE0: // RET PO
                                    totalExecutedCycles += 5; pendingCycles -= 5;
                                    if (!RegFlagP)
                                    {
                                        RegPC.LowByte = ReadMemory(RegSP.LowWord++); RegPC.HighByte = ReadMemory(RegSP.LowWord++);
                                        totalExecutedCycles += 6; pendingCycles -= 6;
                                    }
                                    break;
                                case 0xE1: // POP IX
                                    totalExecutedCycles += 14; pendingCycles -= 14;
                                    RegIX.LowByte = ReadMemory(RegSP.LowWord++); RegIX.HighByte = ReadMemory(RegSP.LowWord++);
                                    break;
                                case 0xE2: // JP PO, nn
                                    totalExecutedCycles += 10; pendingCycles -= 10;
                                    TUS = (ushort)(ReadMemory(RegPC.LowWord++) + ReadMemory(RegPC.LowWord++) * 256);
                                    if (!RegFlagP)
                                    {
                                        RegPC.LowWord = TUS;
                                    }
                                    break;
                                case 0xE3: // EX (SP), IX
                                    totalExecutedCycles += 23; pendingCycles -= 23;
                                    TUS = RegSP.LowWord; TBL = ReadMemory(TUS++); TBH = ReadMemory(TUS--);
                                    WriteMemory(TUS++, RegIX.LowByte); WriteMemory(TUS, RegIX.HighByte);
                                    RegIX.LowByte = TBL; RegIX.HighByte = TBH;
                                    RegWZ.LowWord = RegIX.LowWord;
                                    break;
                                case 0xE4: // CALL C, nn
                                    totalExecutedCycles += 10; pendingCycles -= 10;
                                    TUS = (ushort)(ReadMemory(RegPC.LowWord++) + ReadMemory(RegPC.LowWord++) * 256);
                                    if (RegFlagC)
                                    {
                                        totalExecutedCycles += 7; pendingCycles -= 7;
                                        WriteMemory(--RegSP.LowWord, RegPC.HighByte); WriteMemory(--RegSP.LowWord, RegPC.LowByte);
                                        RegPC.LowWord = TUS;
                                    }
                                    break;
                                case 0xE5: // PUSH IX
                                    totalExecutedCycles += 15; pendingCycles -= 15;
                                    WriteMemory(--RegSP.LowWord, RegIX.HighByte); WriteMemory(--RegSP.LowWord, RegIX.LowByte);
                                    break;
                                case 0xE6: // AND n
                                    totalExecutedCycles += 7; pendingCycles -= 7;
                                    RegAF.LowWord = TableALU[4, RegAF.HighByte, ReadMemory(RegPC.LowWord++), 0];
                                    break;
                                case 0xE7: // RST $20
                                    totalExecutedCycles += 11; pendingCycles -= 11;
                                    WriteMemory(--RegSP.LowWord, RegPC.HighByte); WriteMemory(--RegSP.LowWord, RegPC.LowByte);
                                    RegPC.LowWord = 0x20;
                                    break;
                                case 0xE8: // RET PE
                                    totalExecutedCycles += 5; pendingCycles -= 5;
                                    if (RegFlagP)
                                    {
                                        totalExecutedCycles += 6; pendingCycles -= 6;
                                        RegPC.LowByte = ReadMemory(RegSP.LowWord++); RegPC.HighByte = ReadMemory(RegSP.LowWord++);
                                    }
                                    break;
                                case 0xE9: // JP IX
                                    RegPC.LowWord = RegIX.LowWord;
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    break;
                                case 0xEA: // JP PE, nn
                                    totalExecutedCycles += 10; pendingCycles -= 10;
                                    TUS = (ushort)(ReadMemory(RegPC.LowWord++) + ReadMemory(RegPC.LowWord++) * 256);
                                    if (RegFlagP)
                                    {
                                        RegPC.LowWord = TUS;
                                    }
                                    break;
                                case 0xEB: // EX DE, HL
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    TUS = RegDE.LowWord; RegDE.LowWord = RegHL.LowWord; RegHL.LowWord = TUS;                                    
                                    break;
                                case 0xEC: // CALL PE, nn
                                    totalExecutedCycles += 10; pendingCycles -= 10;
                                    TUS = (ushort)(ReadMemory(RegPC.LowWord++) + ReadMemory(RegPC.LowWord++) * 256);
                                    if (RegFlagP)
                                    {
                                        totalExecutedCycles += 7; pendingCycles -= 7;
                                        WriteMemory(--RegSP.LowWord, RegPC.HighByte); WriteMemory(--RegSP.LowWord, RegPC.LowByte);
                                        RegPC.LowWord = TUS;
                                    }
                                    break;
                                case 0xED: // (Prefix)
                                    ++RegR;
                                    switch (ReadOp(RegPC.LowWord++))
                                    {
                                        case 0x00: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x01: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x02: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x03: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x04: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x05: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x06: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x07: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x08: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x09: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x0A: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x0B: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x0C: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x0D: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x0E: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x0F: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x10: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x11: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x12: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x13: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x14: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x15: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x16: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x17: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x18: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x19: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x1A: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x1B: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x1C: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x1D: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x1E: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x1F: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x20: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x21: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x22: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x23: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x24: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x25: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x26: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x27: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x28: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x29: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x2A: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x2B: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x2C: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x2D: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x2E: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x2F: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x30: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x31: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x32: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x33: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x34: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x35: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x36: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x37: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x38: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x39: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x3A: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x3B: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x3C: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x3D: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x3E: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x3F: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x40: // IN B, C
                                            totalExecutedCycles += 12; pendingCycles -= 12;
                                            RegBC.HighByte = ReadHardware((ushort)RegBC.LowWord);
                                            RegFlagS = RegBC.HighByte > 127;
                                            RegFlagZ = RegBC.HighByte == 0;
                                            RegFlag5 = (RegBC.HighByte & 0x20) != 0;
                                            RegFlagH = false;
                                            RegFlag3 = (RegBC.HighByte & 0x08) != 0;
                                            RegFlagP = TableParity[RegBC.HighByte];
                                            RegFlagN = false;
                                            break;
                                        case 0x41: // OUT C, B
                                            totalExecutedCycles += 12; pendingCycles -= 12;
                                            WriteHardware(RegBC.LowByte, RegBC.HighByte);
                                            break;
                                        case 0x42: // SBC HL, BC
                                            totalExecutedCycles += 15; pendingCycles -= 15;
                                            TI1 = (short)RegHL.LowWord; TI2 = (short)RegBC.LowWord; TIR = TI1 - TI2;
                                            if (RegFlagC) { --TIR; ++TI2; }
                                            TUS = (ushort)TIR;
                                            RegFlagH = ((RegHL.LowWord ^ RegBC.LowWord ^ TUS) & 0x1000) != 0;
                                            RegFlagN = true;
                                            RegFlagC = (((int)RegHL.LowWord - (int)RegBC.LowWord - (RegFlagC ? 1 : 0)) & 0x10000) != 0;
                                            RegFlagP = TIR > 32767 || TIR < -32768;
                                            RegFlagS = TUS > 32767;
                                            RegFlagZ = TUS == 0;
                                            RegHL.LowWord = TUS;
                                            RegFlag3 = (TUS & 0x0800) != 0;
                                            RegFlag5 = (TUS & 0x2000) != 0;                                            
                                            break;
                                        case 0x43: // LD (nn), BC
                                            totalExecutedCycles += 20; pendingCycles -= 20;
                                            TUS = (ushort)(ReadMemory(RegPC.LowWord++) + ReadMemory(RegPC.LowWord++) * 256);
                                            WriteMemory(TUS++, RegBC.LowByte);
                                            WriteMemory(TUS, RegBC.HighByte);
                                            break;
                                        case 0x44: // NEG
                                            totalExecutedCycles += 8; pendingCycles -= 8;
                                            RegAF.LowWord = TableNeg[RegAF.LowWord];                                            
                                            break;
                                        case 0x45: // RETN
                                            totalExecutedCycles += 14; pendingCycles -= 14;
                                            RegPC.LowByte = ReadMemory(RegSP.LowWord++); RegPC.HighByte = ReadMemory(RegSP.LowWord++);
                                            IFF1 = IFF2;
                                            break;
                                        case 0x46: // IM $0
                                            totalExecutedCycles += 8; pendingCycles -= 8;
                                            interruptMode = 0;                                            
                                            break;
                                        case 0x47: // LD I, A
                                            totalExecutedCycles += 9; pendingCycles -= 9;
                                            RegI = RegAF.HighByte;                                            
                                            break;
                                        case 0x48: // IN C, C
                                            totalExecutedCycles += 12; pendingCycles -= 12;
                                            RegBC.LowByte = ReadHardware((ushort)RegBC.LowWord);
                                            RegFlagS = RegBC.LowByte > 127;
                                            RegFlagZ = RegBC.LowByte == 0;
                                            RegFlag5 = (RegBC.LowByte & 0x20) != 0;
                                            RegFlagH = false;
                                            RegFlag3 = (RegBC.LowByte & 0x08) != 0;
                                            RegFlagP = TableParity[RegBC.LowByte];
                                            RegFlagN = false;
                                            break;
                                        case 0x49: // OUT C, C
                                            totalExecutedCycles += 12; pendingCycles -= 12;
                                            WriteHardware(RegBC.LowByte, RegBC.LowByte);
                                            break;
                                        case 0x4A: // ADC HL, BC
                                            totalExecutedCycles += 15; pendingCycles -= 15;
                                            TI1 = (short)RegHL.LowWord; TI2 = (short)RegBC.LowWord; TIR = TI1 + TI2;
                                            if (RegFlagC) { ++TIR; ++TI2; }
                                            TUS = (ushort)TIR;
                                            RegFlagH = ((TI1 & 0xFFF) + (TI2 & 0xFFF)) > 0xFFF;
                                            RegFlagN = false;
                                            RegFlagC = ((ushort)TI1 + (ushort)TI2) > 0xFFFF;
                                            RegFlagP = TIR > 32767 || TIR < -32768;
                                            RegFlagS = TUS > 32767;
                                            RegFlagZ = TUS == 0;
                                            RegHL.LowWord = TUS;
                                            RegFlag3 = (TUS & 0x0800) != 0;
                                            RegFlag5 = (TUS & 0x2000) != 0;                                            
                                            break;
                                        case 0x4B: // LD BC, (nn)
                                            totalExecutedCycles += 20; pendingCycles -= 20;
                                            TUS = (ushort)(ReadMemory(RegPC.LowWord++) + ReadMemory(RegPC.LowWord++) * 256);
                                            RegBC.LowByte = ReadMemory(TUS++); RegBC.HighByte = ReadMemory(TUS);
                                            break;
                                        case 0x4C: // NEG
                                            totalExecutedCycles += 8; pendingCycles -= 8;
                                            RegAF.LowWord = TableNeg[RegAF.LowWord];                                            
                                            break;
                                        case 0x4D: // RETI
                                            totalExecutedCycles += 14; pendingCycles -= 14;
                                            RegPC.LowByte = ReadMemory(RegSP.LowWord++); RegPC.HighByte = ReadMemory(RegSP.LowWord++);
                                            break;
                                        case 0x4E: // IM $0
                                            interruptMode = 0;
                                            totalExecutedCycles += 8; pendingCycles -= 8;
                                            break;
                                        case 0x4F: // LD R, A
                                            RegR = RegAF.HighByte;
                                            totalExecutedCycles += 9; pendingCycles -= 9;
                                            break;
                                        case 0x50: // IN D, C
                                            totalExecutedCycles += 12; pendingCycles -= 12;
                                            RegDE.HighByte = ReadHardware((ushort)RegBC.LowWord);
                                            RegFlagS = RegDE.HighByte > 127;
                                            RegFlagZ = RegDE.HighByte == 0;
                                            RegFlag5 = (RegDE.HighByte & 0x20) != 0;
                                            RegFlagH = false;
                                            RegFlag3 = (RegDE.HighByte & 0x08) != 0;
                                            RegFlagP = TableParity[RegDE.HighByte];
                                            RegFlagN = false;
                                            break;
                                        case 0x51: // OUT C, D
                                            totalExecutedCycles += 12; pendingCycles -= 12;
                                            WriteHardware(RegBC.LowByte, RegDE.HighByte);
                                            break;
                                        case 0x52: // SBC HL, DE
                                            totalExecutedCycles += 15; pendingCycles -= 15;
                                            TI1 = (short)RegHL.LowWord; TI2 = (short)RegDE.LowWord; TIR = TI1 - TI2;
                                            if (RegFlagC) { --TIR; ++TI2; }
                                            TUS = (ushort)TIR;
                                            RegFlagH = ((RegHL.LowWord ^ RegDE.LowWord ^ TUS) & 0x1000) != 0;
                                            RegFlagN = true;
                                            RegFlagC = (((int)RegHL.LowWord - (int)RegDE.LowWord - (RegFlagC ? 1 : 0)) & 0x10000) != 0;
                                            RegFlagP = TIR > 32767 || TIR < -32768;
                                            RegFlagS = TUS > 32767;
                                            RegFlagZ = TUS == 0;
                                            RegHL.LowWord = TUS;
                                            RegFlag3 = (TUS & 0x0800) != 0;
                                            RegFlag5 = (TUS & 0x2000) != 0;                                            
                                            break;
                                        case 0x53: // LD (nn), DE
                                            totalExecutedCycles += 20; pendingCycles -= 20;
                                            TUS = (ushort)(ReadMemory(RegPC.LowWord++) + ReadMemory(RegPC.LowWord++) * 256);
                                            WriteMemory(TUS++, RegDE.LowByte);
                                            WriteMemory(TUS, RegDE.HighByte);
                                            break;
                                        case 0x54: // NEG
                                            totalExecutedCycles += 8; pendingCycles -= 8;
                                            RegAF.LowWord = TableNeg[RegAF.LowWord];                                            
                                            break;
                                        case 0x55: // RETN
                                            totalExecutedCycles += 14; pendingCycles -= 14;
                                            RegPC.LowByte = ReadMemory(RegSP.LowWord++); RegPC.HighByte = ReadMemory(RegSP.LowWord++);
                                            IFF1 = IFF2;
                                            break;
                                        case 0x56: // IM $1
                                            totalExecutedCycles += 8; pendingCycles -= 8;
                                            interruptMode = 1;                                            
                                            break;
                                        case 0x57: // LD A, I
                                            totalExecutedCycles += 9; pendingCycles -= 9;
                                            RegAF.HighByte = RegI;
                                            RegFlagS = RegI > 127;
                                            RegFlagZ = RegI == 0;
                                            RegFlag5 = ((RegI & 0x20) != 0);
                                            RegFlagH = false;
                                            RegFlag3 = ((RegI & 0x08) != 0);
                                            RegFlagN = false;
                                            RegFlagP = IFF2;                                            
                                            break;
                                        case 0x58: // IN E, C
                                            totalExecutedCycles += 12; pendingCycles -= 12;
                                            RegDE.LowByte = ReadHardware((ushort)RegBC.LowWord);
                                            RegFlagS = RegDE.LowByte > 127;
                                            RegFlagZ = RegDE.LowByte == 0;
                                            RegFlag5 = (RegDE.LowByte & 0x20) != 0;
                                            RegFlagH = false;
                                            RegFlag3 = (RegDE.LowByte & 0x08) != 0;
                                            RegFlagP = TableParity[RegDE.LowByte];
                                            RegFlagN = false;
                                            break;
                                        case 0x59: // OUT C, E
                                            totalExecutedCycles += 12; pendingCycles -= 12;
                                            WriteHardware(RegBC.LowByte, RegDE.LowByte);
                                            break;
                                        case 0x5A: // ADC HL, DE
                                            totalExecutedCycles += 15; pendingCycles -= 15;
                                            TI1 = (short)RegHL.LowWord; TI2 = (short)RegDE.LowWord; TIR = TI1 + TI2;
                                            if (RegFlagC) { ++TIR; ++TI2; }
                                            TUS = (ushort)TIR;
                                            RegFlagH = ((TI1 & 0xFFF) + (TI2 & 0xFFF)) > 0xFFF;
                                            RegFlagN = false;
                                            RegFlagC = ((ushort)TI1 + (ushort)TI2) > 0xFFFF;
                                            RegFlagP = TIR > 32767 || TIR < -32768;
                                            RegFlagS = TUS > 32767;
                                            RegFlagZ = TUS == 0;
                                            RegHL.LowWord = TUS;
                                            RegFlag3 = (TUS & 0x0800) != 0;
                                            RegFlag5 = (TUS & 0x2000) != 0;                                            
                                            break;
                                        case 0x5B: // LD DE, (nn)
                                            totalExecutedCycles += 20; pendingCycles -= 20;
                                            TUS = (ushort)(ReadMemory(RegPC.LowWord++) + ReadMemory(RegPC.LowWord++) * 256);
                                            RegDE.LowByte = ReadMemory(TUS++); RegDE.HighByte = ReadMemory(TUS);
                                            break;
                                        case 0x5C: // NEG
                                            totalExecutedCycles += 8; pendingCycles -= 8;
                                            RegAF.LowWord = TableNeg[RegAF.LowWord];                                            
                                            break;
                                        case 0x5D: // RETI
                                            totalExecutedCycles += 14; pendingCycles -= 14;
                                            RegPC.LowByte = ReadMemory(RegSP.LowWord++); RegPC.HighByte = ReadMemory(RegSP.LowWord++);
                                            break;
                                        case 0x5E: // IM $2
                                            totalExecutedCycles += 8; pendingCycles -= 8;
                                            interruptMode = 2;                                            
                                            break;
                                        case 0x5F: // LD A, R
                                            totalExecutedCycles += 9; pendingCycles -= 9;
                                            RegAF.HighByte = (byte)((RegR & 0x7F) | RegR2);
                                            RegFlagS = (RegR2 == 0x80);
                                            RegFlagZ = (byte)((RegR & 0x7F) | RegR2) == 0;
                                            RegFlagH = false;
                                            RegFlag5 = ((RegR & 0x20) != 0);
                                            RegFlagN = false;
                                            RegFlag3 = ((RegR & 0x08) != 0);
                                            RegFlagP = IFF2;                                            
                                            break;
                                        case 0x60: // IN H, C
                                            totalExecutedCycles += 12; pendingCycles -= 12;
                                            RegHL.HighByte = ReadHardware((ushort)RegBC.LowWord);
                                            RegFlagS = RegHL.HighByte > 127;
                                            RegFlagZ = RegHL.HighByte == 0;
                                            RegFlagH = false;
                                            RegFlagP = TableParity[RegHL.HighByte];
                                            RegFlagN = false;
                                            RegFlag3 = (RegHL.HighByte & 0x08) != 0;
                                            RegFlag5 = (RegHL.HighByte & 0x20) != 0;
                                            break;
                                        case 0x61: // OUT C, H
                                            totalExecutedCycles += 12; pendingCycles -= 12;
                                            WriteHardware(RegBC.LowByte, RegHL.HighByte);
                                            break;
                                        case 0x62: // SBC HL, HL
                                            totalExecutedCycles += 15; pendingCycles -= 15;
                                            TI1 = (short)RegHL.LowWord; TI2 = (short)RegHL.LowWord; TIR = TI1 - TI2;
                                            if (RegFlagC) { --TIR; ++TI2; }
                                            TUS = (ushort)TIR;
                                            RegFlagH = ((RegHL.LowWord ^ RegHL.LowWord ^ TUS) & 0x1000) != 0;
                                            RegFlagN = true;
                                            RegFlagC = (((int)RegHL.LowWord - (int)RegHL.LowWord - (RegFlagC ? 1 : 0)) & 0x10000) != 0;
                                            RegFlagP = TIR > 32767 || TIR < -32768;
                                            RegFlagS = TUS > 32767;
                                            RegFlagZ = TUS == 0;
                                            RegHL.LowWord = TUS;
                                            RegFlag3 = (TUS & 0x0800) != 0;
                                            RegFlag5 = (TUS & 0x2000) != 0;                                            
                                            break;
                                        case 0x63: // LD (nn), HL
                                            totalExecutedCycles += 16; pendingCycles -= 16;
                                            TUS = (ushort)(ReadMemory(RegPC.LowWord++) + ReadMemory(RegPC.LowWord++) * 256);
                                            WriteMemory(TUS++, RegHL.LowByte);
                                            WriteMemory(TUS, RegHL.HighByte);
                                            break;
                                        case 0x64: // NEG
                                            totalExecutedCycles += 8; pendingCycles -= 8;
                                            RegAF.LowWord = TableNeg[RegAF.LowWord];                                            
                                            break;
                                        case 0x65: // RETN
                                            totalExecutedCycles += 14; pendingCycles -= 14;
                                            RegPC.LowByte = ReadMemory(RegSP.LowWord++); RegPC.HighByte = ReadMemory(RegSP.LowWord++);
                                            IFF1 = IFF2;
                                            break;
                                        case 0x66: // IM $0
                                            totalExecutedCycles += 8; pendingCycles -= 8;
                                            interruptMode = 0;                                            
                                            break;
                                        case 0x67: // RRD
                                            totalExecutedCycles += 18; pendingCycles -= 18;
                                            TB1 = RegAF.HighByte; TB2 = ReadMemory(RegHL.LowWord);
                                            WriteMemory(RegHL.LowWord, (byte)((TB2 >> 4) + (TB1 << 4)));
                                            RegAF.HighByte = (byte)((TB1 & 0xF0) + (TB2 & 0x0F));
                                            RegFlagS = RegAF.HighByte > 127;
                                            RegFlagZ = RegAF.HighByte == 0;
                                            RegFlagH = false;
                                            RegFlagP = TableParity[RegAF.HighByte];
                                            RegFlagN = false;
                                            RegFlag3 = (RegAF.HighByte & 0x08) != 0;
                                            RegFlag5 = (RegAF.HighByte & 0x20) != 0;
                                            break;
                                        case 0x68: // IN L, C
                                            totalExecutedCycles += 12; pendingCycles -= 12;
                                            RegHL.LowByte = ReadHardware((ushort)RegBC.LowWord);
                                            RegFlagS = RegHL.LowByte > 127;
                                            RegFlagZ = RegHL.LowByte == 0;
                                            RegFlagH = false;
                                            RegFlagP = TableParity[RegHL.LowByte];
                                            RegFlagN = false;
                                            RegFlag3 = (RegHL.LowByte & 0x08) != 0;
                                            RegFlag5 = (RegHL.LowByte & 0x20) != 0;
                                            break;
                                        case 0x69: // OUT C, L
                                            totalExecutedCycles += 12; pendingCycles -= 12;
                                            WriteHardware(RegBC.LowByte, RegHL.LowByte);
                                            break;
                                        case 0x6A: // ADC HL, HL
                                            totalExecutedCycles += 15; pendingCycles -= 15;
                                            TI1 = (short)RegHL.LowWord; TI2 = (short)RegHL.LowWord; TIR = TI1 + TI2;
                                            if (RegFlagC) { ++TIR; ++TI2; }
                                            TUS = (ushort)TIR;
                                            RegFlagH = ((TI1 & 0xFFF) + (TI2 & 0xFFF)) > 0xFFF;
                                            RegFlagN = false;
                                            RegFlagC = ((ushort)TI1 + (ushort)TI2) > 0xFFFF;
                                            RegFlagP = TIR > 32767 || TIR < -32768;
                                            RegFlagS = TUS > 32767;
                                            RegFlagZ = TUS == 0;
                                            RegHL.LowWord = TUS;
                                            RegFlag3 = (TUS & 0x0800) != 0;
                                            RegFlag5 = (TUS & 0x2000) != 0;                                            
                                            break;
                                        case 0x6B: // LD HL, (nn)
                                            totalExecutedCycles += 16; pendingCycles -= 16;
                                            TUS = (ushort)(ReadMemory(RegPC.LowWord++) + ReadMemory(RegPC.LowWord++) * 256);
                                            RegHL.LowByte = ReadMemory(TUS++); RegHL.HighByte = ReadMemory(TUS);
                                            break;
                                        case 0x6C: // NEG
                                            totalExecutedCycles += 8; pendingCycles -= 8;
                                            RegAF.LowWord = TableNeg[RegAF.LowWord];                                            
                                            break;
                                        case 0x6D: // RETI
                                            totalExecutedCycles += 14; pendingCycles -= 14;
                                            RegPC.LowByte = ReadMemory(RegSP.LowWord++); RegPC.HighByte = ReadMemory(RegSP.LowWord++);
                                            break;
                                        case 0x6E: // IM $0
                                            totalExecutedCycles += 8; pendingCycles -= 8;
                                            interruptMode = 0;                                            
                                            break;
                                        case 0x6F: // RLD
                                            totalExecutedCycles += 18; pendingCycles -= 18;
                                            TB1 = RegAF.HighByte; TB2 = ReadMemory(RegHL.LowWord);
                                            WriteMemory(RegHL.LowWord, (byte)((TB1 & 0x0F) + (TB2 << 4)));
                                            RegAF.HighByte = (byte)((TB1 & 0xF0) + (TB2 >> 4));
                                            RegFlagS = RegAF.HighByte > 127;
                                            RegFlagZ = RegAF.HighByte == 0;
                                            RegFlagH = false;
                                            RegFlagP = TableParity[RegAF.HighByte];
                                            RegFlagN = false;
                                            RegFlag3 = (RegAF.HighByte & 0x08) != 0;
                                            RegFlag5 = (RegAF.HighByte & 0x20) != 0;
                                            break;
                                        case 0x70: // IN 0, C
                                            totalExecutedCycles += 12; pendingCycles -= 12;
                                            TB = ReadHardware((ushort)RegBC.LowWord);
                                            RegFlagS = TB > 127;
                                            RegFlagZ = TB == 0;
                                            RegFlagH = false;
                                            RegFlagP = TableParity[TB];
                                            RegFlagN = false;
                                            RegFlag3 = (TB & 0x08) != 0;
                                            RegFlag5 = (TB & 0x20) != 0;
                                            break;
                                        case 0x71: // OUT C, 0
                                            totalExecutedCycles += 12; pendingCycles -= 12;
                                            WriteHardware(RegBC.LowByte, 0);
                                            break;
                                        case 0x72: // SBC HL, SP
                                            totalExecutedCycles += 15; pendingCycles -= 15;
                                            TI1 = (short)RegHL.LowWord; TI2 = (short)RegSP.LowWord; TIR = TI1 - TI2;
                                            if (RegFlagC) { --TIR; ++TI2; }
                                            TUS = (ushort)TIR;
                                            RegFlagH = ((RegHL.LowWord ^ RegSP.LowWord ^ TUS) & 0x1000) != 0;
                                            RegFlagN = true;
                                            RegFlagC = (((int)RegHL.LowWord - (int)RegSP.LowWord - (RegFlagC ? 1 : 0)) & 0x10000) != 0;
                                            RegFlagP = TIR > 32767 || TIR < -32768;
                                            RegFlagS = TUS > 32767;
                                            RegFlagZ = TUS == 0;
                                            RegHL.LowWord = TUS;
                                            RegFlag3 = (TUS & 0x0800) != 0;
                                            RegFlag5 = (TUS & 0x2000) != 0;
                                            break;
                                        case 0x73: // LD (nn), SP
                                            totalExecutedCycles += 20; pendingCycles -= 20;
                                            TUS = (ushort)(ReadOpArg(RegPC.LowWord++) + ReadOpArg(RegPC.LowWord++) * 256);
                                            WriteMemory(TUS++, RegSP.LowByte);
                                            WriteMemory(TUS, RegSP.HighByte);
                                            break;
                                        case 0x74: // NEG
                                            totalExecutedCycles += 8; pendingCycles -= 8;
                                            RegAF.LowWord = TableNeg[RegAF.LowWord];                                            
                                            break;
                                        case 0x75: // RETN
                                            totalExecutedCycles += 14; pendingCycles -= 14;
                                            RegPC.LowByte = ReadMemory(RegSP.LowWord++); RegPC.HighByte = ReadMemory(RegSP.LowWord++);
                                            IFF1 = IFF2;
                                            break;
                                        case 0x76: // IM $1
                                            totalExecutedCycles += 8; pendingCycles -= 8;
                                            interruptMode = 1;                                            
                                            break;
                                        case 0x77: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x78: // IN A, C
                                            totalExecutedCycles += 12; pendingCycles -= 12;
                                            RegAF.HighByte = ReadHardware((ushort)RegBC.LowWord);
                                            RegFlagS = RegAF.HighByte > 127;
                                            RegFlagZ = RegAF.HighByte == 0;
                                            RegFlagH = false;
                                            RegFlagP = TableParity[RegAF.HighByte];
                                            RegFlagN = false;
                                            RegFlag3 = (RegAF.HighByte & 0x08) != 0;
                                            RegFlag5 = (RegAF.HighByte & 0x20) != 0;
                                            RegWZ.LowWord = (ushort)(RegBC.LowWord + 1);
                                            break;
                                        case 0x79: // OUT C, A
                                            totalExecutedCycles += 12; pendingCycles -= 12;
                                            WriteHardware(RegBC.LowByte, RegAF.HighByte);
                                            break;
                                        case 0x7A: // ADC HL, SP
                                            totalExecutedCycles += 15; pendingCycles -= 15;
                                            TI1 = (short)RegHL.LowWord; TI2 = (short)RegSP.LowWord; TIR = TI1 + TI2;
                                            if (RegFlagC) { ++TIR; ++TI2; }
                                            TUS = (ushort)TIR;
                                            RegFlagH = ((TI1 & 0xFFF) + (TI2 & 0xFFF)) > 0xFFF;
                                            RegFlagN = false;
                                            RegFlagC = ((ushort)TI1 + (ushort)TI2) > 0xFFFF;
                                            RegFlagP = TIR > 32767 || TIR < -32768;
                                            RegFlagS = TUS > 32767;
                                            RegFlagZ = TUS == 0;
                                            RegHL.LowWord = TUS;
                                            RegFlag3 = (TUS & 0x0800) != 0;
                                            RegFlag5 = (TUS & 0x2000) != 0;                                            
                                            break;
                                        case 0x7B: // LD SP, (nn)
                                            totalExecutedCycles += 20; pendingCycles -= 20;
                                            TUS = (ushort)(ReadMemory(RegPC.LowWord++) + ReadMemory(RegPC.LowWord++) * 256);
                                            RegSP.LowByte = ReadMemory(TUS++); RegSP.HighByte = ReadMemory(TUS);
                                            break;
                                        case 0x7C: // NEG
                                            totalExecutedCycles += 8; pendingCycles -= 8;
                                            RegAF.LowWord = TableNeg[RegAF.LowWord];                                            
                                            break;
                                        case 0x7D: // RETI
                                            totalExecutedCycles += 14; pendingCycles -= 14;
                                            RegPC.LowByte = ReadMemory(RegSP.LowWord++); RegPC.HighByte = ReadMemory(RegSP.LowWord++);
                                            break;
                                        case 0x7E: // IM $2
                                            totalExecutedCycles += 8; pendingCycles -= 8;
                                            interruptMode = 2;                                            
                                            break;
                                        case 0x7F: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x80: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x81: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x82: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x83: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x84: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x85: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x86: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x87: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x88: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x89: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x8A: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x8B: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x8C: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x8D: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x8E: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x8F: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x90: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x91: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x92: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x93: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x94: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x95: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x96: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x97: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x98: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x99: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x9A: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x9B: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x9C: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x9D: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x9E: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x9F: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xA0: // LDI
                                            totalExecutedCycles += 16; pendingCycles -= 16;
                                            WriteMemory(RegDE.LowWord++, TB1 = ReadMemory(RegHL.LowWord++));
                                            TB1 += RegAF.HighByte; RegFlag5 = (TB1 & 0x02) != 0; RegFlag3 = (TB1 & 0x08) != 0;
                                            --RegBC.LowWord;
                                            RegFlagP = RegBC.LowWord != 0;
                                            RegFlagH = false;
                                            RegFlagN = false;
                                            break;
                                        case 0xA1: // CPI
                                            totalExecutedCycles += 16; pendingCycles -= 16;
                                            TB1 = ReadMemory(RegHL.LowWord++); TB2 = (byte)(RegAF.HighByte - TB1);
                                            RegFlagN = true;
                                            RegFlagH = TableHalfBorrow[RegAF.HighByte, TB1];
                                            RegFlagZ = TB2 == 0;
                                            RegFlagS = TB2 > 127;
                                            TB1 = (byte)(RegAF.HighByte - TB1 - (RegFlagH ? 1 : 0)); RegFlag5 = (TB1 & 0x02) != 0; RegFlag3 = (TB1 & 0x08) != 0;
                                            --RegBC.LowWord;
                                            RegFlagP = RegBC.LowWord != 0;
                                            break;
                                        case 0xA2: // INI
                                            totalExecutedCycles += 16; pendingCycles -= 16;
                                            TB = ReadHardware(RegBC.LowWord);
                                            RegWZ.LowWord = (ushort)(RegBC.LowWord + 1);
                                            --RegBC.HighByte;
                                            WriteMemory(RegHL.LowWord++, TB);
                                            RegFlagZ = RegBC.HighByte == 0;
                                            TUS = (ushort)(((RegBC.LowByte + 1) & 0xff) + TB);
                                            if ((TB & 0x80) != 0)
                                            {
                                                RegFlagN = true;
                                            }
                                            if ((TUS & 0x100) != 0)
                                            {
                                                RegFlagH = RegFlagC = true;
                                            }
                                            RegFlagP = TableParity[(TUS & 0x07) ^ RegBC.HighByte];
                                            break;
                                        case 0xA3: // OUTI
                                            totalExecutedCycles += 16; pendingCycles -= 16;
                                            TB = ReadMemory(RegHL.LowWord++);
                                            --RegBC.HighByte;
                                            RegWZ.LowWord = (ushort)(RegBC.LowWord + 1);
                                            WriteHardware(RegBC.LowWord, TB);
                                            RegFlagC = false;
                                            RegFlagN = false;
                                            RegFlag3 = IsX(RegBC.HighByte);
                                            RegFlagH = false;
                                            RegFlag5 = IsY(RegBC.HighByte);
                                            RegFlagZ = RegBC.HighByte == 0;
                                            RegFlagS = IsS(RegBC.HighByte);
                                            TUS = (ushort)(RegHL.LowByte + TB);
                                            if (IsS(TB))
                                            {
                                                RegFlagN = true;
                                            }
                                            if ((TUS & 0x100) != 0)
                                            {
                                                RegFlagC = true;
                                                RegFlagH = true;
                                            }
                                            RegFlagP = TableParity[(TUS & 0x07) ^ RegBC.HighByte];
                                            break;
                                        case 0xA4: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xA5: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xA6: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xA7: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xA8: // LDD
                                            totalExecutedCycles += 16; pendingCycles -= 16;
                                            WriteMemory(RegDE.LowWord--, TB1 = ReadMemory(RegHL.LowWord--));
                                            TB1 += RegAF.HighByte; RegFlag5 = (TB1 & 0x02) != 0; RegFlag3 = (TB1 & 0x08) != 0;
                                            --RegBC.LowWord;
                                            RegFlagP = RegBC.LowWord != 0;
                                            RegFlagH = false;
                                            RegFlagN = false;
                                            break;
                                        case 0xA9: // CPD
                                            totalExecutedCycles += 16; pendingCycles -= 16;
                                            TB1 = ReadMemory(RegHL.LowWord--); TB2 = (byte)(RegAF.HighByte - TB1);
                                            RegWZ.LowWord--;
                                            RegFlagN = true;
                                            RegFlagH = TableHalfBorrow[RegAF.HighByte, TB1];
                                            RegFlagZ = TB2 == 0;
                                            RegFlagS = TB2 > 127;
                                            TB1 = (byte)(RegAF.HighByte - TB1 - (RegFlagH ? 1 : 0)); RegFlag5 = (TB1 & 0x02) != 0; RegFlag3 = (TB1 & 0x08) != 0;
                                            --RegBC.LowWord;
                                            RegFlagP = RegBC.LowWord != 0;
                                            break;
                                        case 0xAA: // IND
                                            totalExecutedCycles += 16; pendingCycles -= 16;
                                            TB = ReadHardware(RegBC.LowWord);
                                            RegWZ.LowWord = (ushort)(RegBC.LowWord - 1);
                                            --RegBC.HighByte;
                                            WriteMemory(RegHL.LowWord--, TB);
                                            RegFlagZ = RegBC.HighByte == 0;
                                            TUS = (ushort)(((RegBC.LowByte - 1) & 0xff) + TB);
                                            if ((TB & 0x80) != 0)
                                            {
                                                RegFlagN = true;
                                            }
                                            if ((TUS & 0x100) != 0)
                                            {
                                                RegFlagH = RegFlagC = true;
                                            }
                                            RegFlagP = TableParity[(TUS & 0x07) ^ RegBC.HighByte];
                                            break;
                                        case 0xAB: // OUTD
                                            totalExecutedCycles += 16; pendingCycles -= 16;
                                            TB = ReadMemory(RegHL.LowWord--);
                                            WriteHardware(RegBC.LowWord, TB);
                                            --RegBC.HighByte;
                                            RegWZ.LowWord = (ushort)(RegBC.LowWord - 1);
                                            TUS = (ushort)(RegHL.LowByte + TB);
                                            RegFlagZ = RegBC.HighByte == 0;
                                            if ((TB & 0x80) != 0)
                                            {
                                                RegFlagN = true;
                                            }
                                            if ((TUS & 0x100) != 0)
                                            {
                                                RegFlagH = RegFlagC = true;
                                            }
                                            RegFlagP = TableParity[(TUS & 0x07) ^ RegBC.HighByte];
                                            break;
                                        case 0xAC: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xAD: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xAE: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xAF: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xB0: // LDIR
                                            totalExecutedCycles += 16; pendingCycles -= 16;
                                            WriteMemory(RegDE.LowWord++, TB1 = ReadMemory(RegHL.LowWord++));
                                            TB1 += RegAF.HighByte; RegFlag5 = (TB1 & 0x02) != 0; RegFlag3 = (TB1 & 0x08) != 0;
                                            --RegBC.LowWord;
                                            RegFlagP = RegBC.LowWord != 0;
                                            RegFlagH = false;
                                            RegFlagN = false;
                                            if (RegBC.LowWord != 0)
                                            {
                                                RegPC.LowWord -= 2;
                                                RegWZ.LowWord = (ushort)(RegPC.LowWord + 1);
                                                totalExecutedCycles += 5; pendingCycles -= 5;
                                            }
                                            break;
                                        case 0xB1: // CPIR
                                            totalExecutedCycles += 16; pendingCycles -= 16;
                                            TB1 = ReadMemory(RegHL.LowWord++); TB2 = (byte)(RegAF.HighByte - TB1);
                                            RegFlagN = true;
                                            RegFlagH = TableHalfBorrow[RegAF.HighByte, TB1];
                                            RegFlagZ = TB2 == 0;
                                            RegFlagS = TB2 > 127;
                                            TB1 = (byte)(RegAF.HighByte - TB1 - (RegFlagH ? 1 : 0)); RegFlag5 = (TB1 & 0x02) != 0; RegFlag3 = (TB1 & 0x08) != 0;
                                            --RegBC.LowWord;
                                            RegFlagP = RegBC.LowWord != 0;
                                            if (RegBC.LowWord != 0 && !RegFlagZ)
                                            {
                                                RegPC.LowWord -= 2;
                                                RegWZ.LowWord = (ushort)(RegPC.LowWord + 1);
                                                totalExecutedCycles += 5; pendingCycles -= 5;
                                            }
                                            break;
                                        case 0xB2: // INIR
                                            totalExecutedCycles += 16; pendingCycles -= 16;
                                            TB = ReadHardware(RegBC.LowWord);
                                            RegWZ.LowWord = (ushort)(RegBC.LowWord + 1);
                                            --RegBC.HighByte;
                                            WriteMemory(RegHL.LowWord++, TB);
                                            RegFlagZ = RegBC.HighByte == 0;
                                            TUS = (ushort)(((RegBC.LowByte + 1) & 0xff) + TB);
                                            if ((TB & 0x80) != 0)
                                            {
                                                RegFlagN = true;
                                            }
                                            if ((TUS & 0x100) != 0)
                                            {
                                                RegFlagH = RegFlagC = true;
                                            }
                                            RegFlagP = TableParity[(TUS & 0x07) ^ RegBC.HighByte];
                                            if (RegBC.HighByte != 0)
                                            {
                                                RegPC.LowWord -= 2;
                                                totalExecutedCycles += 5; pendingCycles -= 5;
                                            }
                                            break;
                                        case 0xB3: // OTIR
                                            totalExecutedCycles += 16; pendingCycles -= 16;
                                            TB = ReadMemory(RegHL.LowWord++);
                                            --RegBC.HighByte;
                                            RegWZ.LowWord = (ushort)(RegBC.LowWord + 1);
                                            WriteHardware(RegBC.LowWord, TB);
                                            RegFlagC = false;
                                            RegFlagN = false;
                                            RegFlag3 = IsX(RegBC.HighByte);
                                            RegFlagH = false;
                                            RegFlag5 = IsY(RegBC.HighByte);
                                            RegFlagZ = RegBC.HighByte == 0;
                                            RegFlagS = IsS(RegBC.HighByte);
                                            TUS = (ushort)(RegHL.LowByte + TB);
                                            if (IsS(TB))
                                            {
                                                RegFlagN = true;
                                            }
                                            if ((TUS & 0x100) != 0)
                                            {
                                                RegFlagC = true;
                                                RegFlagH = true;
                                            }
                                            RegFlagP = TableParity[(TUS & 0x07) ^ RegBC.HighByte];
                                            if (RegBC.HighByte != 0)
                                            {
                                                RegPC.LowWord -= 2;
                                                totalExecutedCycles += 5; pendingCycles -= 5;
                                            }
                                            break;
                                        case 0xB4: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xB5: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xB6: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xB7: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xB8: // LDDR
                                            totalExecutedCycles += 16; pendingCycles -= 16;
                                            WriteMemory(RegDE.LowWord--, TB1 = ReadMemory(RegHL.LowWord--));
                                            TB1 += RegAF.HighByte; RegFlag5 = (TB1 & 0x02) != 0; RegFlag3 = (TB1 & 0x08) != 0;
                                            --RegBC.LowWord;
                                            RegFlagP = RegBC.LowWord != 0;
                                            RegFlagH = false;
                                            RegFlagN = false;
                                            if (RegBC.LowWord != 0)
                                            {
                                                RegPC.LowWord -= 2;
                                                RegWZ.LowWord = (ushort)(RegPC.LowWord + 1);
                                                totalExecutedCycles += 5; pendingCycles -= 5;
                                            }
                                            break;
                                        case 0xB9: // CPDR
                                            totalExecutedCycles += 16; pendingCycles -= 16;
                                            TB1 = ReadMemory(RegHL.LowWord--); TB2 = (byte)(RegAF.HighByte - TB1);
                                            RegFlagN = true;
                                            RegFlagH = TableHalfBorrow[RegAF.HighByte, TB1];
                                            RegFlagZ = TB2 == 0;
                                            RegFlagS = TB2 > 127;
                                            TB1 = (byte)(RegAF.HighByte - TB1 - (RegFlagH ? 1 : 0)); RegFlag5 = (TB1 & 0x02) != 0; RegFlag3 = (TB1 & 0x08) != 0;
                                            --RegBC.LowWord;
                                            RegFlagP = RegBC.LowWord != 0;
                                            if (RegBC.LowWord != 0 && !RegFlagZ)
                                            {
                                                RegPC.LowWord -= 2;
                                                RegWZ.LowWord = (ushort)(RegPC.LowWord + 1);
                                                totalExecutedCycles += 5; pendingCycles -= 5;
                                            }
                                            break;
                                        case 0xBA: // INDR
                                            totalExecutedCycles += 16; pendingCycles -= 16;
                                            TB = ReadHardware(RegBC.LowWord);
                                            RegWZ.LowWord = (ushort)(RegBC.LowWord - 1);
                                            --RegBC.HighByte;
                                            WriteMemory(RegHL.LowWord--, TB);
                                            RegFlagZ = RegBC.HighByte == 0;
                                            TUS = (ushort)(((RegBC.LowByte - 1) & 0xff) + TB);
                                            if ((TB & 0x80) != 0)
                                            {
                                                RegFlagN = true;
                                            }
                                            if ((TUS & 0x100) != 0)
                                            {
                                                RegFlagH = RegFlagC = true;
                                            }
                                            RegFlagP = TableParity[(TUS & 0x07) ^ RegBC.HighByte];
                                            if (RegBC.HighByte != 0)
                                            {
                                                RegPC.LowWord -= 2;
                                                totalExecutedCycles += 5; pendingCycles -= 5;
                                            }
                                            break;
                                        case 0xBB: // OTDR
                                            totalExecutedCycles += 16; pendingCycles -= 16;
                                            TB = ReadMemory(RegHL.LowWord--);
                                            WriteHardware(RegBC.LowWord, TB);
                                            --RegBC.HighByte;
                                            RegWZ.LowWord = (ushort)(RegBC.LowWord - 1);
                                            TUS = (ushort)(RegHL.LowByte + TB);
                                            RegFlagZ = RegBC.HighByte == 0;
                                            if ((TB & 0x80) != 0)
                                            {
                                                RegFlagN = true;
                                            }
                                            if ((TUS & 0x100) != 0)
                                            {
                                                RegFlagH = RegFlagC = true;
                                            }
                                            RegFlagP = TableParity[(TUS & 0x07) ^ RegBC.HighByte];
                                            if (RegBC.HighByte != 0)
                                            {
                                                RegPC.LowWord -= 2;
                                                totalExecutedCycles += 5; pendingCycles -= 5;
                                            }
                                            break;
                                        case 0xBC: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xBD: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xBE: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xBF: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xC0: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xC1: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xC2: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xC3: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xC4: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xC5: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xC6: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xC7: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xC8: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xC9: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xCA: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xCB: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xCC: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xCD: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xCE: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xCF: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xD0: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xD1: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xD2: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xD3: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xD4: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xD5: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xD6: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xD7: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xD8: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xD9: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xDA: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xDB: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xDC: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xDD: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xDE: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xDF: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xE0: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xE1: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xE2: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xE3: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xE4: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xE5: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xE6: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xE7: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xE8: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xE9: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xEA: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xEB: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xEC: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xED: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xEE: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xEF: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xF0: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xF1: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xF2: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xF3: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xF4: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xF5: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xF6: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xF7: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xF8: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xF9: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xFA: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xFB: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xFC: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xFD: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xFE: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xFF: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                    }
                                    break;
                                case 0xEE: // XOR n
                                    totalExecutedCycles += 7; pendingCycles -= 7;
                                    RegAF.LowWord = TableALU[5, RegAF.HighByte, ReadMemory(RegPC.LowWord++), 0];
                                    break;
                                case 0xEF: // RST $28
                                    totalExecutedCycles += 11; pendingCycles -= 11;
                                    WriteMemory(--RegSP.LowWord, RegPC.HighByte); WriteMemory(--RegSP.LowWord, RegPC.LowByte);
                                    RegPC.LowWord = 0x28;
                                    break;
                                case 0xF0: // RET P
                                    totalExecutedCycles += 5; pendingCycles -= 5;
                                    if (!RegFlagS)
                                    {
                                        RegPC.LowByte = ReadMemory(RegSP.LowWord++); RegPC.HighByte = ReadMemory(RegSP.LowWord++);
                                        totalExecutedCycles += 6; pendingCycles -= 6;
                                    }
                                    break;
                                case 0xF1: // POP AF
                                    totalExecutedCycles += 10; pendingCycles -= 10;
                                    RegAF.LowByte = ReadMemory(RegSP.LowWord++); RegAF.HighByte = ReadMemory(RegSP.LowWord++);
                                    break;
                                case 0xF2: // JP P, nn
                                    totalExecutedCycles += 10; pendingCycles -= 10;
                                    TUS = (ushort)(ReadMemory(RegPC.LowWord++) + ReadMemory(RegPC.LowWord++) * 256);
                                    if (!RegFlagS)
                                    {
                                        RegPC.LowWord = TUS;
                                    }
                                    break;
                                case 0xF3: // DI
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    IFF1 = IFF2 = false;                                    
                                    break;
                                case 0xF4: // CALL P, nn
                                    totalExecutedCycles += 10; pendingCycles -= 10;
                                    TUS = (ushort)(ReadMemory(RegPC.LowWord++) + ReadMemory(RegPC.LowWord++) * 256);
                                    if (!RegFlagS)
                                    {
                                        totalExecutedCycles += 7; pendingCycles -= 7;
                                        WriteMemory(--RegSP.LowWord, RegPC.HighByte); WriteMemory(--RegSP.LowWord, RegPC.LowByte);
                                        RegPC.LowWord = TUS;
                                    }
                                    break;
                                case 0xF5: // PUSH AF
                                    totalExecutedCycles += 11; pendingCycles -= 11;
                                    WriteMemory(--RegSP.LowWord, RegAF.HighByte); WriteMemory(--RegSP.LowWord, RegAF.LowByte);
                                    break;
                                case 0xF6: // OR n
                                    totalExecutedCycles += 7; pendingCycles -= 7;
                                    RegAF.LowWord = TableALU[6, RegAF.HighByte, ReadMemory(RegPC.LowWord++), 0];
                                    break;
                                case 0xF7: // RST $30
                                    totalExecutedCycles += 11; pendingCycles -= 11;
                                    WriteMemory(--RegSP.LowWord, RegPC.HighByte); WriteMemory(--RegSP.LowWord, RegPC.LowByte);
                                    RegPC.LowWord = 0x30;
                                    break;
                                case 0xF8: // RET M
                                    totalExecutedCycles += 5; pendingCycles -= 5;
                                    if (RegFlagS)
                                    {
                                        RegPC.LowByte = ReadMemory(RegSP.LowWord++); RegPC.HighByte = ReadMemory(RegSP.LowWord++);
                                        totalExecutedCycles += 6; pendingCycles -= 6;
                                    }
                                    break;
                                case 0xF9: // LD SP, IX
                                    totalExecutedCycles += 10; pendingCycles -= 10;
                                    RegSP.LowWord = RegIX.LowWord;                                    
                                    break;
                                case 0xFA: // JP M, nn
                                    totalExecutedCycles += 10; pendingCycles -= 10;
                                    TUS = (ushort)(ReadMemory(RegPC.LowWord++) + ReadMemory(RegPC.LowWord++) * 256);
                                    if (RegFlagS)
                                    {
                                        RegPC.LowWord = TUS;
                                    }
                                    break;
                                case 0xFB: // EI
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    IFF1 = IFF2 = true;
                                    Interruptable = false;                                    
                                    break;
                                case 0xFC: // CALL M, nn
                                    totalExecutedCycles += 10; pendingCycles -= 10;
                                    TUS = (ushort)(ReadMemory(RegPC.LowWord++) + ReadMemory(RegPC.LowWord++) * 256);
                                    if (RegFlagS)
                                    {
                                        totalExecutedCycles += 7; pendingCycles -= 7;
                                        WriteMemory(--RegSP.LowWord, RegPC.HighByte); WriteMemory(--RegSP.LowWord, RegPC.LowByte);
                                        RegPC.LowWord = TUS;
                                    }
                                    break;
                                case 0xFD: // <-
                                    // Invalid sequence.
                                    totalExecutedCycles += 1337; pendingCycles -= 1337;
                                    break;
                                case 0xFE: // CP n
                                    totalExecutedCycles += 7; pendingCycles -= 7;
                                    RegAF.LowWord = TableALU[7, RegAF.HighByte, ReadMemory(RegPC.LowWord++), 0];                                    
                                    break;
                                case 0xFF: // RST $38
                                    totalExecutedCycles += 11; pendingCycles -= 11;
                                    WriteMemory(--RegSP.LowWord, RegPC.HighByte); WriteMemory(--RegSP.LowWord, RegPC.LowByte);
                                    RegPC.LowWord = 0x38;
                                    break;
                            }
                            break;
                        case 0xDE: // SBC A, n
                            RegAF.LowWord = TableALU[3, RegAF.HighByte, ReadOpArg(RegPC.LowWord++), RegFlagC ? 1 : 0];
                            totalExecutedCycles += 7; pendingCycles -= 7;
                            break;
                        case 0xDF: // RST $18
                            WriteMemory(--RegSP.LowWord, RegPC.HighByte); WriteMemory(--RegSP.LowWord, RegPC.LowByte);
                            RegPC.LowWord = 0x18;
                            totalExecutedCycles += 11; pendingCycles -= 11;
                            break;
                        case 0xE0: // RET PO
                            if (!RegFlagP)
                            {
                                RegPC.LowByte = ReadMemory(RegSP.LowWord++); RegPC.HighByte = ReadMemory(RegSP.LowWord++);
                                RegWZ.LowWord = RegPC.LowWord;
                                totalExecutedCycles += 11; pendingCycles -= 11;
                            }
                            else
                            {
                                totalExecutedCycles += 5; pendingCycles -= 5;
                            }
                            break;
                        case 0xE1: // POP HL
                            RegHL.LowByte = ReadMemory(RegSP.LowWord++); RegHL.HighByte = ReadMemory(RegSP.LowWord++);
                            totalExecutedCycles += 10; pendingCycles -= 10;
                            break;
                        case 0xE2: // JP PO, nn
                            RegWZ.LowWord = (ushort)(ReadOpArg(RegPC.LowWord++) + ReadOpArg(RegPC.LowWord++) * 256);
                            if (!RegFlagP)
                            {
                                RegPC.LowWord = RegWZ.LowWord;
                            }
                            totalExecutedCycles += 10; pendingCycles -= 10;
                            break;
                        case 0xE3: // EX (SP), HL
                            TUS = RegSP.LowWord; TBL = ReadMemory(TUS++); TBH = ReadMemory(TUS--);
                            WriteMemory(TUS++, RegHL.LowByte); WriteMemory(TUS, RegHL.HighByte);
                            RegHL.LowByte = TBL; RegHL.HighByte = TBH;
                            RegWZ.LowWord = RegHL.LowWord;
                            totalExecutedCycles += 19; pendingCycles -= 19;
                            break;
                        case 0xE4: // CALL C, nn
                            RegWZ.LowWord = (ushort)(ReadOpArg(RegPC.LowWord++) + ReadOpArg(RegPC.LowWord++) * 256);
                            if (RegFlagC)
                            {
                                WriteMemory(--RegSP.LowWord, RegPC.HighByte); WriteMemory(--RegSP.LowWord, RegPC.LowByte);
                                RegPC.LowWord = RegWZ.LowWord;
                                totalExecutedCycles += 17; pendingCycles -= 17;
                            }
                            else
                            {
                                totalExecutedCycles += 10; pendingCycles -= 10;
                            }
                            break;
                        case 0xE5: // PUSH HL
                            WriteMemory(--RegSP.LowWord, RegHL.HighByte); WriteMemory(--RegSP.LowWord, RegHL.LowByte);
                            totalExecutedCycles += 11; pendingCycles -= 11;
                            break;
                        case 0xE6: // AND n
                            RegAF.LowWord = TableALU[4, RegAF.HighByte, ReadOpArg(RegPC.LowWord++), 0];
                            totalExecutedCycles += 7; pendingCycles -= 7;
                            break;
                        case 0xE7: // RST $20
                            WriteMemory(--RegSP.LowWord, RegPC.HighByte); WriteMemory(--RegSP.LowWord, RegPC.LowByte);
                            RegPC.LowWord = 0x20;
                            totalExecutedCycles += 11; pendingCycles -= 11;
                            break;
                        case 0xE8: // RET PE
                            if (RegFlagP)
                            {
                                RegPC.LowByte = ReadMemory(RegSP.LowWord++); RegPC.HighByte = ReadMemory(RegSP.LowWord++);
                                RegWZ.LowWord = RegPC.LowWord;
                                totalExecutedCycles += 11; pendingCycles -= 11;
                            }
                            else
                            {
                                totalExecutedCycles += 5; pendingCycles -= 5;
                            }
                            break;
                        case 0xE9: // JP HL
                            RegPC.LowWord = RegHL.LowWord;
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0xEA: // JP PE, nn
                            RegWZ.LowWord = (ushort)(ReadOpArg(RegPC.LowWord++) + ReadOpArg(RegPC.LowWord++) * 256);
                            if (RegFlagP)
                            {
                                RegPC.LowWord = RegWZ.LowWord;
                            }
                            totalExecutedCycles += 10; pendingCycles -= 10;
                            break;
                        case 0xEB: // EX DE, HL
                            TUS = RegDE.LowWord; RegDE.LowWord = RegHL.LowWord; RegHL.LowWord = TUS;
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0xEC: // CALL PE, nn
                            RegWZ.LowWord = (ushort)(ReadOpArg(RegPC.LowWord++) + ReadOpArg(RegPC.LowWord++) * 256);
                            if (RegFlagP)
                            {
                                WriteMemory(--RegSP.LowWord, RegPC.HighByte); WriteMemory(--RegSP.LowWord, RegPC.LowByte);
                                RegPC.LowWord = RegWZ.LowWord;
                                totalExecutedCycles += 17; pendingCycles -= 17;
                            }
                            else
                            {
                                totalExecutedCycles += 10; pendingCycles -= 10;
                            }
                            break;
                        case 0xED: // (Prefix)
                            ++RegR;
                            switch (ReadOp(RegPC.LowWord++))
                            {
                                case 0x00: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x01: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x02: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x03: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x04: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x05: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x06: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x07: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x08: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x09: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x0A: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x0B: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x0C: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x0D: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x0E: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x0F: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x10: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x11: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x12: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x13: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x14: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x15: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x16: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x17: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x18: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x19: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x1A: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x1B: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x1C: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x1D: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x1E: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x1F: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x20: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x21: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x22: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x23: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x24: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x25: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x26: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x27: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x28: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x29: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x2A: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x2B: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x2C: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x2D: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x2E: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x2F: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x30: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x31: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x32: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x33: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x34: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x35: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x36: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x37: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x38: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x39: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x3A: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x3B: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x3C: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x3D: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x3E: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x3F: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x40: // IN B, C
                                    totalExecutedCycles += 12; pendingCycles -= 12;
                                    RegBC.HighByte = ReadHardware((ushort)RegBC.LowWord);
                                    RegFlagS = RegBC.HighByte > 127;
                                    RegFlagZ = RegBC.HighByte == 0;
                                    RegFlag5 = (RegBC.HighByte & 0x20) != 0;
                                    RegFlagH = false;
                                    RegFlag3 = (RegBC.HighByte & 0x08) != 0;
                                    RegFlagP = TableParity[RegBC.HighByte];
                                    RegFlagN = false;
                                    break;
                                case 0x41: // OUT C, B
                                    totalExecutedCycles += 12; pendingCycles -= 12;
                                    WriteHardware(RegBC.LowWord, RegBC.HighByte);
                                    break;
                                case 0x42: // SBC HL, BC
                                    totalExecutedCycles += 15; pendingCycles -= 15;
                                    TI1 = (short)RegHL.LowWord; TI2 = (short)RegBC.LowWord; TIR = TI1 - TI2;
                                    if (RegFlagC) { --TIR; ++TI2; }
                                    TUS = (ushort)TIR;
                                    RegWZ.LowWord = (ushort)(RegHL.LowWord + 1);
                                    RegFlagH = ((RegHL.LowWord ^ RegBC.LowWord ^ TUS) & 0x1000) != 0;
                                    RegFlagN = true;
                                    RegFlagC = (((int)RegHL.LowWord - (int)RegBC.LowWord - (RegFlagC ? 1 : 0)) & 0x10000) != 0;
                                    RegFlagP = TIR > 32767 || TIR < -32768;
                                    RegFlagS = TUS > 32767;
                                    RegFlagZ = TUS == 0;
                                    RegHL.LowWord = TUS;
                                    RegFlag3 = (TUS & 0x0800) != 0;
                                    RegFlag5 = (TUS & 0x2000) != 0;
                                    break;
                                case 0x43: // LD (nn), BC
                                    totalExecutedCycles += 20; pendingCycles -= 20;
                                    TUS = (ushort)(ReadOpArg(RegPC.LowWord++) + ReadOpArg(RegPC.LowWord++) * 256);
                                    WriteMemory(TUS++, RegBC.LowByte);
                                    WriteMemory(TUS, RegBC.HighByte);
                                    RegWZ.LowWord = TUS;
                                    break;
                                case 0x44: // NEG
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegAF.LowWord = TableNeg[RegAF.LowWord];
                                    break;
                                case 0x45: // RETN
                                    totalExecutedCycles += 14; pendingCycles -= 14;
                                    RegPC.LowByte = ReadMemory(RegSP.LowWord++); RegPC.HighByte = ReadMemory(RegSP.LowWord++);
                                    RegWZ.LowWord = RegPC.LowWord;
                                    IFF1 = IFF2;
                                    break;
                                case 0x46: // IM $0
                                    interruptMode = 0;
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    break;
                                case 0x47: // LD I, A
                                    RegI = RegAF.HighByte;
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    break;
                                case 0x48: // IN C, C
                                    totalExecutedCycles += 12; pendingCycles -= 12;
                                    RegBC.LowByte = ReadHardware((ushort)RegBC.LowWord);
                                    RegFlagS = RegBC.LowByte > 127;
                                    RegFlagZ = RegBC.LowByte == 0;
                                    RegFlag5 = (RegBC.LowByte & 0x20) != 0;
                                    RegFlagH = false;
                                    RegFlag3 = (RegBC.LowByte & 0x08) != 0;
                                    RegFlagP = TableParity[RegBC.LowByte];
                                    RegFlagN = false;
                                    break;
                                case 0x49: // OUT C, C
                                    totalExecutedCycles += 12; pendingCycles -= 12;
                                    WriteHardware(RegBC.LowWord, RegBC.LowByte);
                                    break;
                                case 0x4A: // ADC HL, BC
                                    totalExecutedCycles += 15; pendingCycles -= 15;
                                    TI1 = (short)RegHL.LowWord; TI2 = (short)RegBC.LowWord; TIR = TI1 + TI2;
                                    if (RegFlagC) { ++TIR; ++TI2; }
                                    TUS = (ushort)TIR;
                                    RegWZ.LowWord = (ushort)(RegHL.LowWord + 1);
                                    RegFlagH = ((TI1 & 0xFFF) + (TI2 & 0xFFF)) > 0xFFF;
                                    RegFlagN = false;
                                    RegFlagC = ((ushort)TI1 + (ushort)TI2) > 0xFFFF;
                                    RegFlagP = TIR > 32767 || TIR < -32768;
                                    RegFlagS = TUS > 32767;
                                    RegFlagZ = TUS == 0;
                                    RegHL.LowWord = TUS;
                                    RegFlag3 = (TUS & 0x0800) != 0;
                                    RegFlag5 = (TUS & 0x2000) != 0;
                                    break;
                                case 0x4B: // LD BC, (nn)
                                    totalExecutedCycles += 20; pendingCycles -= 20;
                                    TUS = (ushort)(ReadOpArg(RegPC.LowWord++) + ReadOpArg(RegPC.LowWord++) * 256);
                                    RegBC.LowByte = ReadMemory(TUS++); RegBC.HighByte = ReadMemory(TUS);
                                    RegWZ.LowWord = TUS;
                                    break;
                                case 0x4C: // NEG
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegAF.LowWord = TableNeg[RegAF.LowWord];                                    
                                    break;
                                case 0x4D: // RETI
                                    totalExecutedCycles += 14; pendingCycles -= 14;
                                    RegPC.LowByte = ReadMemory(RegSP.LowWord++); RegPC.HighByte = ReadMemory(RegSP.LowWord++);
                                    RegWZ.LowWord = RegPC.LowWord;
                                    IFF1 = IFF2;
                                    break;
                                case 0x4E: // IM $0
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    interruptMode = 0;                                    
                                    break;
                                case 0x4F: // LD R, A
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegR = RegAF.HighByte;
                                    RegR2 = (byte)(RegAF.HighByte & 0x80);                                    
                                    break;
                                case 0x50: // IN D, C
                                    totalExecutedCycles += 12; pendingCycles -= 12;
                                    RegDE.HighByte = ReadHardware((ushort)RegBC.LowWord);
                                    RegFlagS = RegDE.HighByte > 127;
                                    RegFlagZ = RegDE.HighByte == 0;
                                    RegFlag5 = (RegDE.HighByte & 0x20) != 0;
                                    RegFlagH = false;
                                    RegFlag3 = (RegDE.HighByte & 0x08) != 0;
                                    RegFlagP = TableParity[RegDE.HighByte];
                                    RegFlagN = false;
                                    break;
                                case 0x51: // OUT C, D
                                    totalExecutedCycles += 12; pendingCycles -= 12;
                                    WriteHardware(RegBC.LowWord, RegDE.HighByte);
                                    break;
                                case 0x52: // SBC HL, DE
                                    totalExecutedCycles += 15; pendingCycles -= 15;
                                    TI1 = (short)RegHL.LowWord; TI2 = (short)RegDE.LowWord; TIR = TI1 - TI2;
                                    if (RegFlagC) { --TIR; ++TI2; }
                                    TUS = (ushort)TIR;
                                    RegWZ.LowWord = (ushort)(RegHL.LowWord + 1);
                                    RegFlagH = ((RegHL.LowWord ^ RegDE.LowWord ^ TUS) & 0x1000) != 0;
                                    RegFlagN = true;
                                    RegFlagC = (((int)RegHL.LowWord - (int)RegDE.LowWord - (RegFlagC ? 1 : 0)) & 0x10000) != 0;
                                    RegFlagP = TIR > 32767 || TIR < -32768;
                                    RegFlagS = TUS > 32767;
                                    RegFlagZ = TUS == 0;
                                    RegHL.LowWord = TUS;
                                    RegFlag3 = (TUS & 0x0800) != 0;
                                    RegFlag5 = (TUS & 0x2000) != 0;
                                    break;
                                case 0x53: // LD (nn), DE
                                    totalExecutedCycles += 20; pendingCycles -= 20;
                                    TUS = (ushort)(ReadOpArg(RegPC.LowWord++) + ReadOpArg(RegPC.LowWord++) * 256);
                                    WriteMemory(TUS++, RegDE.LowByte);
                                    WriteMemory(TUS, RegDE.HighByte);
                                    RegWZ.LowWord = TUS;
                                    break;
                                case 0x54: // NEG
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegAF.LowWord = TableNeg[RegAF.LowWord];                                    
                                    break;
                                case 0x55: // RETN
                                    totalExecutedCycles += 14; pendingCycles -= 14;
                                    RegPC.LowByte = ReadMemory(RegSP.LowWord++); RegPC.HighByte = ReadMemory(RegSP.LowWord++);
                                    RegWZ.LowWord = RegPC.LowWord;
                                    IFF1 = IFF2;
                                    break;
                                case 0x56: // IM $1
                                    totalExecutedCycles += 8; pendingCycles -= 8; 
                                    interruptMode = 1;                                    
                                    break;
                                case 0x57: // LD A, I
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegAF.HighByte = RegI;
                                    RegFlagS = RegI > 127;
                                    RegFlagZ = RegI == 0;
                                    RegFlag5 = ((RegI & 0x20) != 0);
                                    RegFlagH = false;
                                    RegFlag3 = ((RegI & 0x08) != 0);
                                    RegFlagN = false;
                                    RegFlagP = IFF2;                                    
                                    break;
                                case 0x58: // IN E, C
                                    totalExecutedCycles += 12; pendingCycles -= 12;
                                    RegDE.LowByte = ReadHardware((ushort)RegBC.LowWord);
                                    RegFlagS = RegDE.LowByte > 127;
                                    RegFlagZ = RegDE.LowByte == 0;
                                    RegFlag5 = (RegDE.LowByte & 0x20) != 0;
                                    RegFlagH = false;
                                    RegFlag3 = (RegDE.LowByte & 0x08) != 0;
                                    RegFlagP = TableParity[RegDE.LowByte];
                                    RegFlagN = false;
                                    break;
                                case 0x59: // OUT C, E
                                    totalExecutedCycles += 12; pendingCycles -= 12;
                                    WriteHardware(RegBC.LowWord, RegDE.LowByte);
                                    break;
                                case 0x5A: // ADC HL, DE
                                    totalExecutedCycles += 15; pendingCycles -= 15;
                                    TI1 = (short)RegHL.LowWord; TI2 = (short)RegDE.LowWord; TIR = TI1 + TI2;
                                    if (RegFlagC) { ++TIR; ++TI2; }
                                    TUS = (ushort)TIR;
                                    RegWZ.LowWord = (ushort)(RegHL.LowWord + 1);
                                    RegFlagH = ((TI1 & 0xFFF) + (TI2 & 0xFFF)) > 0xFFF;
                                    RegFlagN = false;
                                    RegFlagC = ((ushort)TI1 + (ushort)TI2) > 0xFFFF;
                                    RegFlagP = TIR > 32767 || TIR < -32768;
                                    RegFlagS = TUS > 32767;
                                    RegFlagZ = TUS == 0;
                                    RegHL.LowWord = TUS;
                                    RegFlag3 = (TUS & 0x0800) != 0;
                                    RegFlag5 = (TUS & 0x2000) != 0;                                    
                                    break;
                                case 0x5B: // LD DE, (nn)
                                    totalExecutedCycles += 20; pendingCycles -= 20;
                                    TUS = (ushort)(ReadOpArg(RegPC.LowWord++) + ReadOpArg(RegPC.LowWord++) * 256);
                                    RegDE.LowByte = ReadMemory(TUS++); RegDE.HighByte = ReadMemory(TUS);
                                    RegWZ.LowWord = TUS;
                                    break;
                                case 0x5C: // NEG
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegAF.LowWord = TableNeg[RegAF.LowWord];                                    
                                    break;
                                case 0x5D: // RETI
                                    totalExecutedCycles += 14; pendingCycles -= 14;
                                    RegPC.LowByte = ReadMemory(RegSP.LowWord++); RegPC.HighByte = ReadMemory(RegSP.LowWord++);
                                    RegWZ.LowWord = RegPC.LowWord;
                                    IFF1 = IFF2;
                                    break;
                                case 0x5E: // IM $2
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    interruptMode = 2;                                    
                                    break;
                                case 0x5F: // LD A, R
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegAF.HighByte = (byte)((RegR & 0x7F) | RegR2);
                                    RegFlagS = (RegR2 == 0x80);
                                    RegFlagZ = (byte)((RegR & 0x7F) | RegR2) == 0;
                                    RegFlagH = false;
                                    RegFlag5 = ((RegR & 0x20) != 0);
                                    RegFlagN = false;
                                    RegFlag3 = ((RegR & 0x08) != 0);
                                    RegFlagP = IFF2;
                                    break;
                                case 0x60: // IN H, C
                                    totalExecutedCycles += 12; pendingCycles -= 12;
                                    RegHL.HighByte = ReadHardware(RegBC.LowWord);
                                    RegFlagS = RegHL.HighByte > 127;
                                    RegFlagZ = RegHL.HighByte == 0;
                                    RegFlagH = false;
                                    RegFlagP = TableParity[RegHL.HighByte];
                                    RegFlagN = false;
                                    RegFlag3 = (RegHL.HighByte & 0x08) != 0;
                                    RegFlag5 = (RegHL.HighByte & 0x20) != 0;
                                    break;
                                case 0x61: // OUT C, H
                                    totalExecutedCycles += 12; pendingCycles -= 12;
                                    WriteHardware(RegBC.LowWord, RegHL.HighByte);
                                    break;
                                case 0x62: // SBC HL, HL
                                    totalExecutedCycles += 15; pendingCycles -= 15;
                                    TI1 = (short)RegHL.LowWord; TI2 = (short)RegHL.LowWord; TIR = TI1 - TI2;
                                    if (RegFlagC) { --TIR; ++TI2; }
                                    TUS = (ushort)TIR;
                                    RegWZ.LowWord = (ushort)(RegHL.LowWord + 1);
                                    RegFlagH = ((RegHL.LowWord ^ RegHL.LowWord ^ TUS) & 0x1000) != 0;
                                    RegFlagN = true;
                                    RegFlagC = (((int)RegHL.LowWord - (int)RegHL.LowWord - (RegFlagC ? 1 : 0)) & 0x10000) != 0;
                                    RegFlagP = TIR > 32767 || TIR < -32768;
                                    RegFlagS = TUS > 32767;
                                    RegFlagZ = TUS == 0;
                                    RegHL.LowWord = TUS;
                                    RegFlag3 = (TUS & 0x0800) != 0;
                                    RegFlag5 = (TUS & 0x2000) != 0;
                                    break;
                                case 0x63: // LD (nn), HL
                                    totalExecutedCycles += 16; pendingCycles -= 16;
                                    TUS = (ushort)(ReadOpArg(RegPC.LowWord++) + ReadOpArg(RegPC.LowWord++) * 256);
                                    WriteMemory(TUS++, RegHL.LowByte);
                                    WriteMemory(TUS, RegHL.HighByte);
                                    RegWZ.LowWord = TUS;
                                    break;
                                case 0x64: // NEG
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegAF.LowWord = TableNeg[RegAF.LowWord];                                    
                                    break;
                                case 0x65: // RETN
                                    totalExecutedCycles += 14; pendingCycles -= 14;
                                    RegPC.LowByte = ReadMemory(RegSP.LowWord++); RegPC.HighByte = ReadMemory(RegSP.LowWord++);
                                    RegWZ.LowWord = RegPC.LowWord;
                                    IFF1 = IFF2;
                                    break;
                                case 0x66: // IM $0
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    interruptMode = 0;                                    
                                    break;
                                case 0x67: // RRD
                                    totalExecutedCycles += 18; pendingCycles -= 18;
                                    TB1 = RegAF.HighByte; TB2 = ReadMemory(RegHL.LowWord);
                                    RegWZ.LowWord = (ushort)(RegHL.LowWord + 1);
                                    WriteMemory(RegHL.LowWord, (byte)((TB2 >> 4) + (TB1 << 4)));
                                    RegAF.HighByte = (byte)((TB1 & 0xF0) + (TB2 & 0x0F));
                                    RegFlagS = RegAF.HighByte > 127;
                                    RegFlagZ = RegAF.HighByte == 0;
                                    RegFlagH = false;
                                    RegFlagP = TableParity[RegAF.HighByte];
                                    RegFlagN = false;
                                    RegFlag3 = (RegAF.HighByte & 0x08) != 0;
                                    RegFlag5 = (RegAF.HighByte & 0x20) != 0;
                                    break;
                                case 0x68: // IN L, C
                                    totalExecutedCycles += 12; pendingCycles -= 12;
                                    RegHL.LowByte = ReadHardware((ushort)RegBC.LowWord);
                                    RegFlagS = RegHL.LowByte > 127;
                                    RegFlagZ = RegHL.LowByte == 0;
                                    RegFlagH = false;
                                    RegFlagP = TableParity[RegHL.LowByte];
                                    RegFlagN = false;
                                    RegFlag3 = (RegHL.LowByte & 0x08) != 0;
                                    RegFlag5 = (RegHL.LowByte & 0x20) != 0;
                                    break;
                                case 0x69: // OUT C, L
                                    totalExecutedCycles += 12; pendingCycles -= 12;
                                    WriteHardware(RegBC.LowWord, RegHL.LowByte);
                                    break;
                                case 0x6A: // ADC HL, HL
                                    totalExecutedCycles += 15; pendingCycles -= 15;
                                    TI1 = (short)RegHL.LowWord; TI2 = (short)RegHL.LowWord; TIR = TI1 + TI2;
                                    if (RegFlagC) { ++TIR; ++TI2; }
                                    TUS = (ushort)TIR;
                                    RegWZ.LowWord = (ushort)(RegHL.LowWord + 1);
                                    RegFlagH = ((TI1 & 0xFFF) + (TI2 & 0xFFF)) > 0xFFF;
                                    RegFlagN = false;
                                    RegFlagC = ((ushort)TI1 + (ushort)TI2) > 0xFFFF;
                                    RegFlagP = TIR > 32767 || TIR < -32768;
                                    RegFlagS = TUS > 32767;
                                    RegFlagZ = TUS == 0;
                                    RegHL.LowWord = TUS;
                                    RegFlag3 = (TUS & 0x0800) != 0;
                                    RegFlag5 = (TUS & 0x2000) != 0;
                                    break;
                                case 0x6B: // LD HL, (nn)
                                    totalExecutedCycles += 16; pendingCycles -= 16;
                                    TUS = (ushort)(ReadOpArg(RegPC.LowWord++) + ReadOpArg(RegPC.LowWord++) * 256);
                                    RegHL.LowByte = ReadMemory(TUS++); RegHL.HighByte = ReadMemory(TUS);
                                    RegWZ.LowWord = TUS;
                                    break;
                                case 0x6C: // NEG
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegAF.LowWord = TableNeg[RegAF.LowWord];                                    
                                    break;
                                case 0x6D: // RETI
                                    totalExecutedCycles += 14; pendingCycles -= 14;
                                    RegPC.LowByte = ReadMemory(RegSP.LowWord++); RegPC.HighByte = ReadMemory(RegSP.LowWord++);
                                    RegWZ.LowWord = RegPC.LowWord;
                                    IFF1 = IFF2;
                                    break;
                                case 0x6E: // IM $0
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    interruptMode = 0;                                    
                                    break;
                                case 0x6F: // RLD
                                    totalExecutedCycles += 18; pendingCycles -= 18;
                                    TB1 = RegAF.HighByte; TB2 = ReadMemory(RegHL.LowWord);
                                    RegWZ.LowWord = (ushort)(RegHL.LowWord + 1);
                                    WriteMemory(RegHL.LowWord, (byte)((TB1 & 0x0F) + (TB2 << 4)));
                                    RegAF.HighByte = (byte)((TB1 & 0xF0) + (TB2 >> 4));
                                    RegFlagS = RegAF.HighByte > 127;
                                    RegFlagZ = RegAF.HighByte == 0;
                                    RegFlagH = false;
                                    RegFlagP = TableParity[RegAF.HighByte];
                                    RegFlagN = false;
                                    RegFlag3 = (RegAF.HighByte & 0x08) != 0;
                                    RegFlag5 = (RegAF.HighByte & 0x20) != 0;
                                    break;
                                case 0x70: // IN 0, C
                                    totalExecutedCycles += 12; pendingCycles -= 12;
                                    TB = ReadHardware((ushort)RegBC.LowWord);
                                    RegFlagS = TB > 127;
                                    RegFlagZ = TB == 0;
                                    RegFlagH = false;
                                    RegFlagP = TableParity[TB];
                                    RegFlagN = false;
                                    RegFlag3 = (TB & 0x08) != 0;
                                    RegFlag5 = (TB & 0x20) != 0;
                                    break;
                                case 0x71: // OUT C, 0
                                    totalExecutedCycles += 12; pendingCycles -= 12;
                                    WriteHardware(RegBC.LowWord, 0);
                                    break;
                                case 0x72: // SBC HL, SP
                                    totalExecutedCycles += 15; pendingCycles -= 15;
                                    TI1 = (short)RegHL.LowWord; TI2 = (short)RegSP.LowWord; TIR = TI1 - TI2;
                                    if (RegFlagC) { --TIR; ++TI2; }
                                    TUS = (ushort)TIR;
                                    RegWZ.LowWord = (ushort)(RegHL.LowWord + 1);
                                    RegFlagH = ((RegHL.LowWord ^ RegSP.LowWord ^ TUS) & 0x1000) != 0;
                                    RegFlagN = true;
                                    RegFlagC = (((int)RegHL.LowWord - (int)RegSP.LowWord - (RegFlagC ? 1 : 0)) & 0x10000) != 0;
                                    RegFlagP = TIR > 32767 || TIR < -32768;
                                    RegFlagS = TUS > 32767;
                                    RegFlagZ = TUS == 0;
                                    RegHL.LowWord = TUS;
                                    RegFlag3 = (TUS & 0x0800) != 0;
                                    RegFlag5 = (TUS & 0x2000) != 0;
                                    break;
                                case 0x73: // LD (nn), SP
                                    totalExecutedCycles += 20; pendingCycles -= 20;
                                    TUS = (ushort)(ReadOpArg(RegPC.LowWord++) + ReadOpArg(RegPC.LowWord++) * 256);
                                    WriteMemory(TUS++, RegSP.LowByte);
                                    WriteMemory(TUS, RegSP.HighByte);
                                    RegWZ.LowWord = TUS;
                                    break;
                                case 0x74: // NEG
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegAF.LowWord = TableNeg[RegAF.LowWord];                                    
                                    break;
                                case 0x75: // RETN
                                    totalExecutedCycles += 14; pendingCycles -= 14;
                                    RegPC.LowByte = ReadMemory(RegSP.LowWord++); RegPC.HighByte = ReadMemory(RegSP.LowWord++);
                                    RegWZ.LowWord = RegPC.LowWord;
                                    IFF1 = IFF2;
                                    break;
                                case 0x76: // IM $1
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    interruptMode = 1;                                    
                                    break;
                                case 0x77: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x78: // IN A, C
                                    totalExecutedCycles += 12; pendingCycles -= 12;
                                    RegAF.HighByte = ReadHardware(RegBC.LowWord);
                                    RegFlagS = RegAF.HighByte > 127;
                                    RegFlagZ = RegAF.HighByte == 0;
                                    RegFlagH = false;
                                    RegFlagP = TableParity[RegAF.HighByte];
                                    RegFlagN = false;
                                    RegFlag3 = (RegAF.HighByte & 0x08) != 0;
                                    RegFlag5 = (RegAF.HighByte & 0x20) != 0;
                                    RegWZ.LowWord = (ushort)(RegBC.LowWord + 1);
                                    break;
                                case 0x79: // OUT C, A
                                    totalExecutedCycles += 12; pendingCycles -= 12;
                                    WriteHardware(RegBC.LowWord, RegAF.HighByte);
                                    RegWZ.LowWord = (ushort)(RegBC.LowWord + 1);
                                    break;
                                case 0x7A: // ADC HL, SP
                                    totalExecutedCycles += 15; pendingCycles -= 15;
                                    TI1 = (short)RegHL.LowWord; TI2 = (short)RegSP.LowWord; TIR = TI1 + TI2;
                                    if (RegFlagC) { ++TIR; ++TI2; }
                                    TUS = (ushort)TIR;
                                    RegWZ.LowWord = (ushort)(RegHL.LowWord + 1);
                                    RegFlagH = ((TI1 & 0xFFF) + (TI2 & 0xFFF)) > 0xFFF;
                                    RegFlagN = false;
                                    RegFlagC = ((ushort)TI1 + (ushort)TI2) > 0xFFFF;
                                    RegFlagP = TIR > 32767 || TIR < -32768;
                                    RegFlagS = TUS > 32767;
                                    RegFlagZ = TUS == 0;
                                    RegHL.LowWord = TUS;
                                    RegFlag3 = (TUS & 0x0800) != 0;
                                    RegFlag5 = (TUS & 0x2000) != 0;
                                    break;
                                case 0x7B: // LD SP, (nn)
                                    totalExecutedCycles += 20; pendingCycles -= 20;
                                    TUS = (ushort)(ReadOpArg(RegPC.LowWord++) + ReadOpArg(RegPC.LowWord++) * 256);
                                    RegSP.LowByte = ReadMemory(TUS++); RegSP.HighByte = ReadMemory(TUS);
                                    RegWZ.LowWord = TUS;
                                    break;
                                case 0x7C: // NEG
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegAF.LowWord = TableNeg[RegAF.LowWord];                                    
                                    break;
                                case 0x7D: // RETI
                                    totalExecutedCycles += 14; pendingCycles -= 14;
                                    RegPC.LowByte = ReadMemory(RegSP.LowWord++); RegPC.HighByte = ReadMemory(RegSP.LowWord++);
                                    RegWZ.LowWord = RegPC.LowWord;
                                    IFF1 = IFF2;
                                    break;
                                case 0x7E: // IM $2
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    interruptMode = 2;                                    
                                    break;
                                case 0x7F: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x80: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x81: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x82: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x83: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x84: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x85: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x86: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x87: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x88: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x89: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x8A: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x8B: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x8C: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x8D: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x8E: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x8F: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x90: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x91: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x92: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x93: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x94: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x95: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x96: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x97: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x98: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x99: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x9A: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x9B: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x9C: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x9D: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x9E: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x9F: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0xA0: // LDI
                                    totalExecutedCycles += 16; pendingCycles -= 16;
                                    WriteMemory(RegDE.LowWord++, TB1 = ReadMemory(RegHL.LowWord++));
                                    TB1 += RegAF.HighByte; RegFlag5 = (TB1 & 0x02) != 0; RegFlag3 = (TB1 & 0x08) != 0;
                                    --RegBC.LowWord;
                                    RegFlagP = RegBC.LowWord != 0;
                                    RegFlagH = false;
                                    RegFlagN = false;
                                    break;
                                case 0xA1: // CPI
                                    totalExecutedCycles += 16; pendingCycles -= 16;
                                    TB1 = ReadMemory(RegHL.LowWord++); TB2 = (byte)(RegAF.HighByte - TB1);
                                    RegWZ.LowWord++;
                                    RegFlagN = true;
                                    RegFlagH = TableHalfBorrow[RegAF.HighByte, TB1];
                                    RegFlagZ = TB2 == 0;
                                    RegFlagS = TB2 > 127;
                                    TB1 = (byte)(RegAF.HighByte - TB1 - (RegFlagH ? 1 : 0)); RegFlag5 = (TB1 & 0x02) != 0; RegFlag3 = (TB1 & 0x08) != 0;
                                    --RegBC.LowWord;
                                    RegFlagP = RegBC.LowWord != 0;
                                    break;
                                case 0xA2: // INI
                                    totalExecutedCycles += 16; pendingCycles -= 16;
                                    TB = ReadHardware(RegBC.LowWord);
                                    RegWZ.LowWord = (ushort)(RegBC.LowWord + 1);
                                    --RegBC.HighByte;
                                    WriteMemory(RegHL.LowWord++, TB);
                                    RegFlagZ = RegBC.HighByte == 0;
                                    TUS = (ushort)(((RegBC.LowByte + 1) & 0xff) + TB);
                                    if ((TB & 0x80) != 0)
                                    {
                                        RegFlagN = true;
                                    }
                                    if ((TUS & 0x100) != 0)
                                    {
                                        RegFlagH = RegFlagC = true;
                                    }
                                    RegFlagP = TableParity[(TUS & 0x07) ^ RegBC.HighByte];
                                    break;
                                case 0xA3: // OUTI
                                    totalExecutedCycles += 16; pendingCycles -= 16;
                                    TB = ReadMemory(RegHL.LowWord++);
                                    --RegBC.HighByte;
                                    RegWZ.LowWord = (ushort)(RegBC.LowWord + 1);
                                    WriteHardware(RegBC.LowWord, TB);
                                    RegFlagC = false;
                                    RegFlagN = false;
                                    RegFlag3 = IsX(RegBC.HighByte);
                                    RegFlagH = false;
                                    RegFlag5 = IsY(RegBC.HighByte);
                                    RegFlagZ = RegBC.HighByte == 0;
                                    RegFlagS = IsS(RegBC.HighByte);
                                    TUS = (ushort)(RegHL.LowByte + TB);
                                    if (IsS(TB))
                                    {
                                        RegFlagN = true;
                                    }
                                    if ((TUS & 0x100) != 0)
                                    {
                                        RegFlagC = true;
                                        RegFlagH = true;
                                    }
                                    RegFlagP = TableParity[(TUS & 0x07) ^ RegBC.HighByte];
                                    break;
                                case 0xA4: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0xA5: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0xA6: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0xA7: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0xA8: // LDD
                                    totalExecutedCycles += 16; pendingCycles -= 16;
                                    WriteMemory(RegDE.LowWord--, TB1 = ReadMemory(RegHL.LowWord--));
                                    TB1 += RegAF.HighByte; RegFlag5 = (TB1 & 0x02) != 0; RegFlag3 = (TB1 & 0x08) != 0;
                                    --RegBC.LowWord;
                                    RegFlagP = RegBC.LowWord != 0;
                                    RegFlagH = false;
                                    RegFlagN = false;
                                    break;
                                case 0xA9: // CPD
                                    totalExecutedCycles += 16; pendingCycles -= 16;
                                    TB1 = ReadMemory(RegHL.LowWord--); TB2 = (byte)(RegAF.HighByte - TB1);
                                    RegWZ.LowWord--;
                                    RegFlagN = true;
                                    RegFlagH = TableHalfBorrow[RegAF.HighByte, TB1];
                                    RegFlagZ = TB2 == 0;
                                    RegFlagS = TB2 > 127;
                                    TB1 = (byte)(RegAF.HighByte - TB1 - (RegFlagH ? 1 : 0)); RegFlag5 = (TB1 & 0x02) != 0; RegFlag3 = (TB1 & 0x08) != 0;
                                    --RegBC.LowWord;
                                    RegFlagP = RegBC.LowWord != 0;
                                    break;
                                case 0xAA: // IND
                                    totalExecutedCycles += 16; pendingCycles -= 16;
                                    TB = ReadHardware(RegBC.LowWord);
                                    RegWZ.LowWord = (ushort)(RegBC.LowWord - 1);
                                    --RegBC.HighByte;
                                    WriteMemory(RegHL.LowWord--, TB);
                                    RegFlagZ = RegBC.HighByte == 0;
                                    TUS = (ushort)(((RegBC.LowByte - 1) & 0xff) + TB);
                                    if ((TB & 0x80) != 0)
                                    {
                                        RegFlagN = true;
                                    }
                                    if ((TUS & 0x100) != 0)
                                    {
                                        RegFlagH = RegFlagC = true;
                                    }
                                    RegFlagP = TableParity[(TUS & 0x07) ^ RegBC.HighByte];
                                    break;
                                case 0xAB: // OUTD
                                    totalExecutedCycles += 16; pendingCycles -= 16;
                                    TB = ReadMemory(RegHL.LowWord--);
                                    WriteHardware(RegBC.LowWord, TB);
                                    --RegBC.HighByte;
                                    RegWZ.LowWord = (ushort)(RegBC.LowWord - 1);
                                    TUS = (ushort)(RegHL.LowByte + TB);
                                    RegFlagZ = RegBC.HighByte == 0;
                                    if ((TB & 0x80) != 0)
                                    {
                                        RegFlagN = true;
                                    }
                                    if ((TUS & 0x100) != 0)
                                    {
                                        RegFlagH = RegFlagC = true;
                                    }
                                    RegFlagP = TableParity[(TUS & 0x07) ^ RegBC.HighByte];
                                    break;
                                case 0xAC: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0xAD: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0xAE: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0xAF: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0xB0: // LDIR
                                    totalExecutedCycles += 16; pendingCycles -= 16;
                                    WriteMemory(RegDE.LowWord++, TB1 = ReadMemory(RegHL.LowWord++));
                                    TB1 += RegAF.HighByte; RegFlag5 = (TB1 & 0x02) != 0; RegFlag3 = (TB1 & 0x08) != 0;
                                    --RegBC.LowWord;
                                    RegFlagP = RegBC.LowWord != 0;
                                    RegFlagH = false;
                                    RegFlagN = false;
                                    if (RegBC.LowWord != 0)
                                    {
                                        RegPC.LowWord -= 2;
                                        RegWZ.LowWord = (ushort)(RegPC.LowWord + 1);
                                        totalExecutedCycles += 5; pendingCycles -= 5;
                                    }
                                    break;
                                case 0xB1: // CPIR
                                    totalExecutedCycles += 16; pendingCycles -= 16;
                                    TB1 = ReadMemory(RegHL.LowWord++); TB2 = (byte)(RegAF.HighByte - TB1);
                                    RegWZ.LowWord++;
                                    RegFlagN = true;
                                    RegFlagH = TableHalfBorrow[RegAF.HighByte, TB1];
                                    RegFlagZ = TB2 == 0;
                                    RegFlagS = TB2 > 127;
                                    TB1 = (byte)(RegAF.HighByte - TB1 - (RegFlagH ? 1 : 0)); RegFlag5 = (TB1 & 0x02) != 0; RegFlag3 = (TB1 & 0x08) != 0;
                                    --RegBC.LowWord;
                                    RegFlagP = RegBC.LowWord != 0;
                                    if (RegBC.LowWord != 0 && !RegFlagZ)
                                    {
                                        RegPC.LowWord -= 2;
                                        RegWZ.LowWord = (ushort)(RegPC.LowWord + 1);
                                        totalExecutedCycles += 5; pendingCycles -= 5;
                                    }
                                    break;
                                case 0xB2: // INIR
                                    totalExecutedCycles += 16; pendingCycles -= 16;
                                    TB = ReadHardware(RegBC.LowWord);
                                    RegWZ.LowWord = (ushort)(RegBC.LowWord + 1);
                                    --RegBC.HighByte;
                                    WriteMemory(RegHL.LowWord++, TB);
                                    RegFlagZ = RegBC.HighByte == 0;
                                    TUS = (ushort)(((RegBC.LowByte + 1) & 0xff) + TB);
                                    if ((TB & 0x80) != 0)
                                    {
                                        RegFlagN = true;
                                    }
                                    if ((TUS & 0x100) != 0)
                                    {
                                        RegFlagH = RegFlagC = true;
                                    }
                                    RegFlagP = TableParity[(TUS & 0x07) ^ RegBC.HighByte];
                                    if (RegBC.HighByte != 0)
                                    {
                                        RegPC.LowWord -= 2;
                                        totalExecutedCycles += 5; pendingCycles -= 5;
                                    }
                                    break;
                                case 0xB3: // OTIR
                                    totalExecutedCycles += 16; pendingCycles -= 16;
                                    TB = ReadMemory(RegHL.LowWord++);
                                    --RegBC.HighByte;
                                    RegWZ.LowWord = (ushort)(RegBC.LowWord + 1);
                                    WriteHardware(RegBC.LowWord, TB);
                                    RegFlagC = false;
                                    RegFlagN = false;
                                    RegFlag3 = IsX(RegBC.HighByte);
                                    RegFlagH = false;
                                    RegFlag5 = IsY(RegBC.HighByte);
                                    RegFlagZ = RegBC.HighByte == 0;
                                    RegFlagS = IsS(RegBC.HighByte);
                                    TUS = (ushort)(RegHL.LowByte + TB);
                                    if (IsS(TB))
                                    {
                                        RegFlagN = true;
                                    }
                                    if ((TUS & 0x100) != 0)
                                    {
                                        RegFlagC = true;
                                        RegFlagH = true;
                                    }
                                    RegFlagP = TableParity[(TUS & 0x07) ^ RegBC.HighByte];
                                    if (RegBC.HighByte != 0)
                                    {
                                        RegPC.LowWord -= 2;
                                        totalExecutedCycles += 5; pendingCycles -= 5;
                                    }
                                    break;
                                case 0xB4: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0xB5: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0xB6: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0xB7: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0xB8: // LDDR
                                    totalExecutedCycles += 16; pendingCycles -= 16;
                                    WriteMemory(RegDE.LowWord--, TB1 = ReadMemory(RegHL.LowWord--));
                                    TB1 += RegAF.HighByte; RegFlag5 = (TB1 & 0x02) != 0; RegFlag3 = (TB1 & 0x08) != 0;
                                    --RegBC.LowWord;
                                    RegFlagP = RegBC.LowWord != 0;
                                    RegFlagH = false;
                                    RegFlagN = false;
                                    if (RegBC.LowWord != 0)
                                    {
                                        RegPC.LowWord -= 2;
                                        RegWZ.LowWord = (ushort)(RegPC.LowWord + 1);
                                        totalExecutedCycles += 5; pendingCycles -= 5;
                                    }
                                    break;
                                case 0xB9: // CPDR
                                    totalExecutedCycles += 16; pendingCycles -= 16;
                                    TB1 = ReadMemory(RegHL.LowWord--); TB2 = (byte)(RegAF.HighByte - TB1);
                                    RegWZ.LowWord--;
                                    RegFlagN = true;
                                    RegFlagH = TableHalfBorrow[RegAF.HighByte, TB1];
                                    RegFlagZ = TB2 == 0;
                                    RegFlagS = TB2 > 127;
                                    TB1 = (byte)(RegAF.HighByte - TB1 - (RegFlagH ? 1 : 0)); RegFlag5 = (TB1 & 0x02) != 0; RegFlag3 = (TB1 & 0x08) != 0;
                                    --RegBC.LowWord;
                                    RegFlagP = RegBC.LowWord != 0;
                                    if (RegBC.LowWord != 0 && !RegFlagZ)
                                    {
                                        RegPC.LowWord -= 2;
                                        RegWZ.LowWord = (ushort)(RegPC.LowWord + 1);
                                        totalExecutedCycles += 5; pendingCycles -= 5;
                                    }
                                    break;
                                case 0xBA: // INDR
                                    totalExecutedCycles += 16; pendingCycles -= 16;
                                    TB = ReadHardware(RegBC.LowWord);
                                    RegWZ.LowWord = (ushort)(RegBC.LowWord - 1);
                                    --RegBC.HighByte;
                                    WriteMemory(RegHL.LowWord--, TB);
                                    RegFlagZ = RegBC.HighByte == 0;
                                    TUS = (ushort)(((RegBC.LowByte - 1) & 0xff) + TB);
                                    if ((TB & 0x80) != 0)
                                    {
                                        RegFlagN = true;
                                    }
                                    if ((TUS & 0x100) != 0)
                                    {
                                        RegFlagH = RegFlagC = true;
                                    }
                                    RegFlagP = TableParity[(TUS & 0x07) ^ RegBC.HighByte];
                                    if (RegBC.HighByte != 0)
                                    {
                                        RegPC.LowWord -= 2;
                                        totalExecutedCycles += 5; pendingCycles -= 5;
                                    }
                                    break;
                                case 0xBB: // OTDR
                                    totalExecutedCycles += 16; pendingCycles -= 16;
                                    TB = ReadMemory(RegHL.LowWord--);
                                    WriteHardware(RegBC.LowWord, TB);
                                    --RegBC.HighByte;
                                    RegWZ.LowWord = (ushort)(RegBC.LowWord - 1);
                                    TUS = (ushort)(RegHL.LowByte + TB);
                                    RegFlagZ = RegBC.HighByte == 0;
                                    if ((TB & 0x80) != 0)
                                    {
                                        RegFlagN = true;
                                    }
                                    if ((TUS & 0x100) != 0)
                                    {
                                        RegFlagH = RegFlagC = true;
                                    }
                                    RegFlagP = TableParity[(TUS & 0x07) ^ RegBC.HighByte];
                                    if (RegBC.HighByte != 0)
                                    {
                                        RegPC.LowWord -= 2;
                                        totalExecutedCycles += 5; pendingCycles -= 5;
                                    }
                                    break;
                                case 0xBC: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0xBD: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0xBE: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0xBF: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0xC0: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0xC1: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0xC2: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0xC3: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0xC4: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0xC5: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0xC6: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0xC7: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0xC8: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0xC9: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0xCA: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0xCB: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0xCC: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0xCD: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0xCE: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0xCF: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0xD0: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0xD1: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0xD2: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0xD3: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0xD4: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0xD5: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0xD6: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0xD7: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0xD8: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0xD9: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0xDA: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0xDB: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0xDC: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0xDD: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0xDE: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0xDF: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0xE0: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0xE1: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0xE2: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0xE3: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0xE4: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0xE5: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0xE6: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0xE7: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0xE8: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0xE9: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0xEA: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0xEB: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0xEC: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0xED: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0xEE: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0xEF: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0xF0: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0xF1: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0xF2: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0xF3: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0xF4: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0xF5: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0xF6: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0xF7: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0xF8: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0xF9: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0xFA: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0xFB: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0xFC: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0xFD: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0xFE: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0xFF: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                            }
                            break;
                        case 0xEE: // XOR n
                            RegAF.LowWord = TableALU[5, RegAF.HighByte, ReadOpArg(RegPC.LowWord++), 0];
                            totalExecutedCycles += 7; pendingCycles -= 7;
                            break;
                        case 0xEF: // RST $28
                            WriteMemory(--RegSP.LowWord, RegPC.HighByte); WriteMemory(--RegSP.LowWord, RegPC.LowByte);
                            RegPC.LowWord = 0x28;
                            totalExecutedCycles += 11; pendingCycles -= 11;
                            break;
                        case 0xF0: // RET P
                            if (!RegFlagS)
                            {
                                RegPC.LowByte = ReadMemory(RegSP.LowWord++); RegPC.HighByte = ReadMemory(RegSP.LowWord++);
                                RegWZ.LowWord = RegPC.LowWord;
                                totalExecutedCycles += 11; pendingCycles -= 11;
                            }
                            else
                            {
                                totalExecutedCycles += 5; pendingCycles -= 5;
                            }
                            break;
                        case 0xF1: // POP AF
                            RegAF.LowByte = ReadMemory(RegSP.LowWord++); RegAF.HighByte = ReadMemory(RegSP.LowWord++);
                            totalExecutedCycles += 10; pendingCycles -= 10;
                            break;
                        case 0xF2: // JP P, nn
                            RegWZ.LowWord = (ushort)(ReadOpArg(RegPC.LowWord++) + ReadOpArg(RegPC.LowWord++) * 256);
                            if (!RegFlagS)
                            {
                                RegPC.LowWord = RegWZ.LowWord;
                            }
                            totalExecutedCycles += 10; pendingCycles -= 10;
                            break;
                        case 0xF3: // DI
                            IFF1 = IFF2 = false;
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0xF4: // CALL P, nn
                            RegWZ.LowWord = (ushort)(ReadOpArg(RegPC.LowWord++) + ReadOpArg(RegPC.LowWord++) * 256);
                            if (!RegFlagS)
                            {
                                WriteMemory(--RegSP.LowWord, RegPC.HighByte); WriteMemory(--RegSP.LowWord, RegPC.LowByte);
                                RegPC.LowWord = RegWZ.LowWord;
                                totalExecutedCycles += 17; pendingCycles -= 17;
                            }
                            else
                            {
                                totalExecutedCycles += 10; pendingCycles -= 10;
                            }
                            break;
                        case 0xF5: // PUSH AF
                            WriteMemory(--RegSP.LowWord, RegAF.HighByte); WriteMemory(--RegSP.LowWord, RegAF.LowByte);
                            totalExecutedCycles += 11; pendingCycles -= 11;
                            break;
                        case 0xF6: // OR n
                            RegAF.LowWord = TableALU[6, RegAF.HighByte, ReadOpArg(RegPC.LowWord++), 0];
                            totalExecutedCycles += 7; pendingCycles -= 7;
                            break;
                        case 0xF7: // RST $30
                            WriteMemory(--RegSP.LowWord, RegPC.HighByte); WriteMemory(--RegSP.LowWord, RegPC.LowByte);
                            RegPC.LowWord = 0x30;
                            totalExecutedCycles += 11; pendingCycles -= 11;
                            break;
                        case 0xF8: // RET M
                            if (RegFlagS)
                            {
                                RegPC.LowByte = ReadMemory(RegSP.LowWord++); RegPC.HighByte = ReadMemory(RegSP.LowWord++);
                                RegWZ.LowWord = RegPC.LowWord;
                                totalExecutedCycles += 11; pendingCycles -= 11;
                            }
                            else
                            {
                                totalExecutedCycles += 5; pendingCycles -= 5;
                            }
                            break;
                        case 0xF9: // LD SP, HL
                            RegSP.LowWord = RegHL.LowWord;
                            totalExecutedCycles += 6; pendingCycles -= 6;
                            break;
                        case 0xFA: // JP M, nn
                            RegWZ.LowWord = (ushort)(ReadOpArg(RegPC.LowWord++) + ReadOpArg(RegPC.LowWord++) * 256);
                            if (RegFlagS)
                            {
                                RegPC.LowWord = RegWZ.LowWord;
                            }
                            totalExecutedCycles += 10; pendingCycles -= 10;
                            break;
                        case 0xFB: // EI
                            IFF1 = IFF2 = true;
                            Interruptable = false;
                            totalExecutedCycles += 4; pendingCycles -= 4;
                            break;
                        case 0xFC: // CALL M, nn
                            RegWZ.LowWord = (ushort)(ReadOpArg(RegPC.LowWord++) + ReadOpArg(RegPC.LowWord++) * 256);
                            if (RegFlagS)
                            {
                                WriteMemory(--RegSP.LowWord, RegPC.HighByte); WriteMemory(--RegSP.LowWord, RegPC.LowByte);
                                RegPC.LowWord = RegWZ.LowWord;
                                totalExecutedCycles += 17; pendingCycles -= 17;
                            }
                            else
                            {
                                totalExecutedCycles += 10; pendingCycles -= 10;
                            }
                            break;
                        case 0xFD: // (Prefix)
                            ++RegR;
                            switch (ReadOp(RegPC.LowWord++))
                            {
                                case 0x00: // NOP
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x01: // LD BC, nn
                                    totalExecutedCycles += 10; pendingCycles -= 10;
                                    RegBC.LowWord = (ushort)(ReadMemory(RegPC.LowWord++) + ReadMemory(RegPC.LowWord++) * 256);
                                    break;
                                case 0x02: // LD (BC), A
                                    totalExecutedCycles += 7; pendingCycles -= 7;
                                    WriteMemory(RegBC.LowWord, RegAF.HighByte);
                                    break;
                                case 0x03: // INC BC
                                    totalExecutedCycles += 6; pendingCycles -= 6;
                                    ++RegBC.LowWord;                                    
                                    break;
                                case 0x04: // INC B
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowByte = (byte)(TableInc[++RegBC.HighByte] | (RegAF.LowByte & 1));                                    
                                    break;
                                case 0x05: // DEC B
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowByte = (byte)(TableDec[--RegBC.HighByte] | (RegAF.LowByte & 1));
                                    break;
                                case 0x06: // LD B, n
                                    totalExecutedCycles += 7; pendingCycles -= 7;
                                    RegBC.HighByte = ReadMemory(RegPC.LowWord++);
                                    break;
                                case 0x07: // RLCA
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableRotShift[0, 0, RegAF.LowWord];
                                    break;
                                case 0x08: // EX AF, AF'
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    TUS = RegAF.LowWord; RegAF.LowWord = RegAltAF.LowWord; RegAltAF.LowWord = TUS;
                                    break;
                                case 0x09: // ADD IY, BC
                                    totalExecutedCycles += 15; pendingCycles -= 15;
                                    TI1 = (short)RegIY.LowWord; TI2 = (short)RegBC.LowWord; TIR = TI1 + TI2;
                                    TUS = (ushort)TIR;
                                    RegWZ.LowWord = (ushort)(RegIY.LowWord + 1);
                                    RegFlagH = ((TI1 & 0xFFF) + (TI2 & 0xFFF)) > 0xFFF;
                                    RegFlagN = false;
                                    RegFlagC = ((ushort)TI1 + (ushort)TI2) > 0xFFFF;
                                    RegIY.LowWord = TUS;
                                    RegFlag3 = (TUS & 0x0800) != 0;
                                    RegFlag5 = (TUS & 0x2000) != 0;                                    
                                    break;
                                case 0x0A: // LD A, (BC)
                                    totalExecutedCycles += 7; pendingCycles -= 7;
                                    RegAF.HighByte = ReadMemory(RegBC.LowWord);
                                    break;
                                case 0x0B: // DEC BC
                                    --RegBC.LowWord;
                                    totalExecutedCycles += 6; pendingCycles -= 6;
                                    break;
                                case 0x0C: // INC C
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowByte = (byte)(TableInc[++RegBC.LowByte] | (RegAF.LowByte & 1));                                    
                                    break;
                                case 0x0D: // DEC C
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowByte = (byte)(TableDec[--RegBC.LowByte] | (RegAF.LowByte & 1));
                                    break;
                                case 0x0E: // LD C, n
                                    totalExecutedCycles += 7; pendingCycles -= 7;
                                    RegBC.LowByte = ReadMemory(RegPC.LowWord++);
                                    break;
                                case 0x0F: // RRCA
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableRotShift[0, 1, RegAF.LowWord];
                                    break;
                                case 0x10: // DJNZ d
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    TSB = (sbyte)ReadMemory(RegPC.LowWord++);
                                    if (--RegBC.HighByte != 0)
                                    {
                                        RegPC.LowWord = (ushort)(RegPC.LowWord + TSB);
                                        totalExecutedCycles += 5; pendingCycles -= 5;
                                    }
                                    break;
                                case 0x11: // LD DE, nn
                                    totalExecutedCycles += 10; pendingCycles -= 10;
                                    RegDE.LowWord = (ushort)(ReadMemory(RegPC.LowWord++) + ReadMemory(RegPC.LowWord++) * 256);
                                    break;
                                case 0x12: // LD (DE), A
                                    totalExecutedCycles += 7; pendingCycles -= 7;
                                    WriteMemory(RegDE.LowWord, RegAF.HighByte);
                                    break;
                                case 0x13: // INC DE
                                    totalExecutedCycles += 6; pendingCycles -= 6;
                                    ++RegDE.LowWord;                                    
                                    break;
                                case 0x14: // INC D
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowByte = (byte)(TableInc[++RegDE.HighByte] | (RegAF.LowByte & 1));                                    
                                    break;
                                case 0x15: // DEC D
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowByte = (byte)(TableDec[--RegDE.HighByte] | (RegAF.LowByte & 1));
                                    break;
                                case 0x16: // LD D, n
                                    totalExecutedCycles += 7; pendingCycles -= 7;
                                    RegDE.HighByte = ReadMemory(RegPC.LowWord++);
                                    break;
                                case 0x17: // RLA
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableRotShift[0, 2, RegAF.LowWord];
                                    break;
                                case 0x18: // JR d
                                    totalExecutedCycles += 12; pendingCycles -= 12;
                                    TSB = (sbyte)ReadMemory(RegPC.LowWord++);
                                    RegPC.LowWord = (ushort)(RegPC.LowWord + TSB);
                                    break;
                                case 0x19: // ADD IY, DE
                                    totalExecutedCycles += 15; pendingCycles -= 15;
                                    TI1 = (short)RegIY.LowWord; TI2 = (short)RegDE.LowWord; TIR = TI1 + TI2;
                                    TUS = (ushort)TIR;
                                    RegWZ.LowWord = (ushort)(RegIY.LowWord + 1);
                                    RegFlagH = ((TI1 & 0xFFF) + (TI2 & 0xFFF)) > 0xFFF;
                                    RegFlagN = false;
                                    RegFlagC = ((ushort)TI1 + (ushort)TI2) > 0xFFFF;
                                    RegIY.LowWord = TUS;
                                    RegFlag3 = (TUS & 0x0800) != 0;
                                    RegFlag5 = (TUS & 0x2000) != 0;                                    
                                    break;
                                case 0x1A: // LD A, (DE)
                                    totalExecutedCycles += 7; pendingCycles -= 7;
                                    RegAF.HighByte = ReadMemory(RegDE.LowWord);
                                    break;
                                case 0x1B: // DEC DE
                                    totalExecutedCycles += 6; pendingCycles -= 6;
                                    --RegDE.LowWord;                                    
                                    break;
                                case 0x1C: // INC E
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowByte = (byte)(TableInc[++RegDE.LowByte] | (RegAF.LowByte & 1));                                    
                                    break;
                                case 0x1D: // DEC E
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowByte = (byte)(TableDec[--RegDE.LowByte] | (RegAF.LowByte & 1));
                                    break;
                                case 0x1E: // LD E, n
                                    totalExecutedCycles += 7; pendingCycles -= 7;
                                    RegDE.LowByte = ReadMemory(RegPC.LowWord++);
                                    break;
                                case 0x1F: // RRA
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableRotShift[0, 3, RegAF.LowWord];
                                    break;
                                case 0x20: // JR NZ, d
                                    totalExecutedCycles += 7; pendingCycles -= 7;
                                    TSB = (sbyte)ReadMemory(RegPC.LowWord++);
                                    if (!RegFlagZ)
                                    {
                                        RegPC.LowWord = (ushort)(RegPC.LowWord + TSB);
                                        totalExecutedCycles += 5; pendingCycles -= 5;
                                    }
                                    break;
                                case 0x21: // LD IY, nn
                                    totalExecutedCycles += 14; pendingCycles -= 14;
                                    RegIY.LowWord = (ushort)(ReadOpArg(RegPC.LowWord++) + ReadOpArg(RegPC.LowWord++) * 256);                                    
                                    break;
                                case 0x22: // LD (nn), IY
                                    totalExecutedCycles += 20; pendingCycles -= 20;
                                    TUS = (ushort)(ReadOpArg(RegPC.LowWord++) + ReadOpArg(RegPC.LowWord++) * 256);
                                    WriteMemory(TUS++, RegIY.LowByte);
                                    WriteMemory(TUS, RegIY.HighByte);
                                    RegWZ.LowWord = TUS;
                                    break;
                                case 0x23: // INC IY
                                    totalExecutedCycles += 10; pendingCycles -= 10;
                                    ++RegIY.LowWord;                                    
                                    break;
                                case 0x24: // INC IYH
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegAF.LowByte = (byte)(TableInc[++RegIY.HighByte] | (RegAF.LowByte & 1));                                    
                                    break;
                                case 0x25: // DEC IYH
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegAF.LowByte = (byte)(TableDec[--RegIY.HighByte] | (RegAF.LowByte & 1));
                                    break;
                                case 0x26: // LD IYH, n
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegIY.HighByte = ReadOpArg(RegPC.LowWord++);
                                    break;
                                case 0x27: // DAA
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableDaa[RegAF.LowWord];                                    
                                    break;
                                case 0x28: // JR Z, d
                                    totalExecutedCycles += 7; pendingCycles -= 7;
                                    TSB = (sbyte)ReadMemory(RegPC.LowWord++);
                                    if (RegFlagZ)
                                    {
                                        RegPC.LowWord = (ushort)(RegPC.LowWord + TSB);
                                        totalExecutedCycles += 5; pendingCycles -= 5;
                                    }
                                    break;
                                case 0x29: // ADD IY, IY
                                    totalExecutedCycles += 15; pendingCycles -= 15;
                                    TI1 = (short)RegIY.LowWord; TI2 = (short)RegIY.LowWord; TIR = TI1 + TI2;
                                    TUS = (ushort)TIR;
                                    RegWZ.LowWord = (ushort)(RegIY.LowWord + 1);
                                    RegFlagH = ((TI1 & 0xFFF) + (TI2 & 0xFFF)) > 0xFFF;
                                    RegFlagN = false;
                                    RegFlagC = ((ushort)TI1 + (ushort)TI2) > 0xFFFF;
                                    RegIY.LowWord = TUS;
                                    RegFlag3 = (TUS & 0x0800) != 0;
                                    RegFlag5 = (TUS & 0x2000) != 0;                                    
                                    break;
                                case 0x2A: // LD IY, (nn)
                                    totalExecutedCycles += 20; pendingCycles -= 20;
                                    TUS = (ushort)(ReadOpArg(RegPC.LowWord++) + ReadOpArg(RegPC.LowWord++) * 256);
                                    RegIY.LowByte = ReadMemory(TUS++); RegIY.HighByte = ReadMemory(TUS);
                                    RegWZ.LowWord = TUS;
                                    break;
                                case 0x2B: // DEC IY
                                    totalExecutedCycles += 10; pendingCycles -= 10;
                                    --RegIY.LowWord;                                    
                                    break;
                                case 0x2C: // INC IYL
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegAF.LowByte = (byte)(TableInc[++RegIY.LowByte] | (RegAF.LowByte & 1));                                    
                                    break;
                                case 0x2D: // DEC IYL
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegAF.LowByte = (byte)(TableDec[--RegIY.LowByte] | (RegAF.LowByte & 1));
                                    break;
                                case 0x2E: // LD IYL, n
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegIY.LowByte = ReadOpArg(RegPC.LowWord++);
                                    break;
                                case 0x2F: // CPL
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.HighByte ^= 0xFF; RegFlagH = true; RegFlagN = true; RegFlag3 = (RegAF.HighByte & 0x08) != 0; RegFlag5 = (RegAF.HighByte & 0x20) != 0;                                    
                                    break;
                                case 0x30: // JR NC, d
                                    totalExecutedCycles += 7; pendingCycles -= 7;
                                    TSB = (sbyte)ReadMemory(RegPC.LowWord++);
                                    if (!RegFlagC)
                                    {
                                        RegPC.LowWord = (ushort)(RegPC.LowWord + TSB);
                                        totalExecutedCycles += 5; pendingCycles -= 5;
                                    }
                                    break;
                                case 0x31: // LD SP, nn
                                    totalExecutedCycles += 10; pendingCycles -= 10;
                                    RegSP.LowWord = (ushort)(ReadMemory(RegPC.LowWord++) + ReadMemory(RegPC.LowWord++) * 256);
                                    break;
                                case 0x32: // LD (nn), A
                                    totalExecutedCycles += 13; pendingCycles -= 13;
                                    WriteMemory((ushort)(ReadMemory(RegPC.LowWord++) + ReadMemory(RegPC.LowWord++) * 256), RegAF.HighByte);
                                    break;
                                case 0x33: // INC SP
                                    totalExecutedCycles += 6; pendingCycles -= 6;
                                    ++RegSP.LowWord;                                    
                                    break;
                                case 0x34: // INC (IY+d)
                                    totalExecutedCycles += 23; pendingCycles -= 23;
                                    Displacement = (sbyte)ReadOpArg(RegPC.LowWord++);
                                    RegWZ.LowWord = (ushort)(RegIY.LowWord + Displacement);
                                    TB = ReadMemory(RegWZ.LowWord); RegAF.LowByte = (byte)(TableInc[++TB] | (RegAF.LowByte & 1)); WriteMemory(RegWZ.LowWord, TB);
                                    break;
                                case 0x35: // DEC (IY+d)
                                    totalExecutedCycles += 23; pendingCycles -= 23;
                                    Displacement = (sbyte)ReadOpArg(RegPC.LowWord++);
                                    RegWZ.LowWord = (ushort)(RegIY.LowWord + Displacement);
                                    TB = ReadMemory(RegWZ.LowWord); RegAF.LowByte = (byte)(TableDec[--TB] | (RegAF.LowByte & 1)); WriteMemory(RegWZ.LowWord, TB);
                                    break;
                                case 0x36: // LD (IY+d), n
                                    totalExecutedCycles += 19; pendingCycles -= 19;
                                    Displacement = (sbyte)ReadOpArg(RegPC.LowWord++);
                                    RegWZ.LowWord = (ushort)(RegIY.LowWord + Displacement);
                                    WriteMemory(RegWZ.LowWord, ReadOpArg(RegPC.LowWord++));
                                    break;
                                case 0x37: // SCF
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegFlagH = false; RegFlagN = false; RegFlagC = true; RegFlag3 = (RegAF.HighByte & 0x08) != 0; RegFlag5 = (RegAF.HighByte & 0x20) != 0;                                    
                                    break;
                                case 0x38: // JR C, d
                                    totalExecutedCycles += 7; pendingCycles -= 7;
                                    TSB = (sbyte)ReadMemory(RegPC.LowWord++);
                                    if (RegFlagC)
                                    {
                                        RegPC.LowWord = (ushort)(RegPC.LowWord + TSB);
                                        totalExecutedCycles += 5; pendingCycles -= 5;
                                    }
                                    break;
                                case 0x39: // ADD IY, SP
                                    totalExecutedCycles += 15; pendingCycles -= 15;
                                    TI1 = (short)RegIY.LowWord; TI2 = (short)RegSP.LowWord; TIR = TI1 + TI2;
                                    TUS = (ushort)TIR;
                                    RegWZ.LowWord = (ushort)(RegIY.LowWord + 1);
                                    RegFlagH = ((TI1 & 0xFFF) + (TI2 & 0xFFF)) > 0xFFF;
                                    RegFlagN = false;
                                    RegFlagC = ((ushort)TI1 + (ushort)TI2) > 0xFFFF;
                                    RegIY.LowWord = TUS;
                                    RegFlag3 = (TUS & 0x0800) != 0;
                                    RegFlag5 = (TUS & 0x2000) != 0;                                    
                                    break;
                                case 0x3A: // LD A, (nn)
                                    totalExecutedCycles += 13; pendingCycles -= 13;
                                    RegAF.HighByte = ReadMemory((ushort)(ReadMemory(RegPC.LowWord++) + ReadMemory(RegPC.LowWord++) * 256));
                                    break;
                                case 0x3B: // DEC SP
                                    totalExecutedCycles += 6; pendingCycles -= 6;
                                    --RegSP.LowWord;                                    
                                    break;
                                case 0x3C: // INC A
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowByte = (byte)(TableInc[++RegAF.HighByte] | (RegAF.LowByte & 1));                                    
                                    break;
                                case 0x3D: // DEC A
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowByte = (byte)(TableDec[--RegAF.HighByte] | (RegAF.LowByte & 1));
                                    break;
                                case 0x3E: // LD A, n
                                    totalExecutedCycles += 7; pendingCycles -= 7;
                                    RegAF.HighByte = ReadMemory(RegPC.LowWord++);
                                    break;
                                case 0x3F: // CCF
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegFlagH = RegFlagC; RegFlagN = false; RegFlagC ^= true; RegFlag3 = (RegAF.HighByte & 0x08) != 0; RegFlag5 = (RegAF.HighByte & 0x20) != 0;
                                    break;
                                case 0x40: // LD B, B
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x41: // LD B, C
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegBC.HighByte = RegBC.LowByte;                                    
                                    break;
                                case 0x42: // LD B, D
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegBC.HighByte = RegDE.HighByte;
                                    break;
                                case 0x43: // LD B, E
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegBC.HighByte = RegDE.LowByte;
                                    break;
                                case 0x44: // LD B, IYH
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegBC.HighByte = RegIY.HighByte;                                    
                                    break;
                                case 0x45: // LD B, IYL
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegBC.HighByte = RegIY.LowByte;
                                    break;
                                case 0x46: // LD B, (IY+d)
                                    totalExecutedCycles += 19; pendingCycles -= 19;
                                    Displacement = (sbyte)ReadOpArg(RegPC.LowWord++);
                                    RegWZ.LowWord = (ushort)(RegIY.LowWord + Displacement);
                                    RegBC.HighByte = ReadMemory(RegWZ.LowWord);
                                    break;
                                case 0x47: // LD B, A
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegBC.HighByte = RegAF.HighByte;                                    
                                    break;
                                case 0x48: // LD C, B
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegBC.LowByte = RegBC.HighByte;
                                    break;
                                case 0x49: // LD C, C
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x4A: // LD C, D
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegBC.LowByte = RegDE.HighByte;
                                    break;
                                case 0x4B: // LD C, E
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegBC.LowByte = RegDE.LowByte;
                                    break;
                                case 0x4C: // LD C, IYH
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegBC.LowByte = RegIY.HighByte;                                    
                                    break;
                                case 0x4D: // LD C, IYL
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegBC.LowByte = RegIY.LowByte;
                                    break;
                                case 0x4E: // LD C, (IY+d)
                                    totalExecutedCycles += 19; pendingCycles -= 19;
                                    Displacement = (sbyte)ReadOpArg(RegPC.LowWord++);
                                    RegWZ.LowWord = (ushort)(RegIY.LowWord + Displacement);
                                    RegBC.LowByte = ReadMemory(RegWZ.LowWord);
                                    break;
                                case 0x4F: // LD C, A
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegBC.LowByte = RegAF.HighByte;                                    
                                    break;
                                case 0x50: // LD D, B
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegDE.HighByte = RegBC.HighByte;
                                    break;
                                case 0x51: // LD D, C
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegDE.HighByte = RegBC.LowByte;
                                    break;
                                case 0x52: // LD D, D
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x53: // LD D, E
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegDE.HighByte = RegDE.LowByte;
                                    break;
                                case 0x54: // LD D, IYH
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegDE.HighByte = RegIY.HighByte;                                    
                                    break;
                                case 0x55: // LD D, IYL
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegDE.HighByte = RegIY.LowByte;
                                    break;
                                case 0x56: // LD D, (IY+d)
                                    totalExecutedCycles += 19; pendingCycles -= 19;
                                    Displacement = (sbyte)ReadOpArg(RegPC.LowWord++);
                                    RegWZ.LowWord = (ushort)(RegIY.LowWord + Displacement);
                                    RegDE.HighByte = ReadMemory(RegWZ.LowWord);
                                    break;
                                case 0x57: // LD D, A
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegDE.HighByte = RegAF.HighByte;                                    
                                    break;
                                case 0x58: // LD E, B
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegDE.LowByte = RegBC.HighByte;
                                    break;
                                case 0x59: // LD E, C
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegDE.LowByte = RegBC.LowByte;
                                    break;
                                case 0x5A: // LD E, D
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegDE.LowByte = RegDE.HighByte;
                                    break;
                                case 0x5B: // LD E, E
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x5C: // LD E, IYH
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegDE.LowByte = RegIY.HighByte;                                    
                                    break;
                                case 0x5D: // LD E, IYL
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegDE.LowByte = RegIY.LowByte;                                    
                                    break;
                                case 0x5E: // LD E, (IY+d)
                                    totalExecutedCycles += 19; pendingCycles -= 19;
                                    Displacement = (sbyte)ReadOpArg(RegPC.LowWord++);
                                    RegWZ.LowWord = (ushort)(RegIY.LowWord + Displacement);
                                    RegDE.LowByte = ReadMemory(RegWZ.LowWord);
                                    break;
                                case 0x5F: // LD E, A
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegDE.LowByte = RegAF.HighByte;                                    
                                    break;
                                case 0x60: // LD IYH, B
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegIY.HighByte = RegBC.HighByte;                                    
                                    break;
                                case 0x61: // LD IYH, C
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegIY.HighByte = RegBC.LowByte;
                                    break;
                                case 0x62: // LD IYH, D
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegIY.HighByte = RegDE.HighByte;
                                    break;
                                case 0x63: // LD IYH, E
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegIY.HighByte = RegDE.LowByte;
                                    break;
                                case 0x64: // LD IYH, IYH
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    break;
                                case 0x65: // LD IYH, IYL
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegIY.HighByte = RegIY.LowByte;
                                    break;
                                case 0x66: // LD H, (IY+d)
                                    totalExecutedCycles += 19; pendingCycles -= 19;
                                    Displacement = (sbyte)ReadOpArg(RegPC.LowWord++);
                                    RegWZ.LowWord = (ushort)(RegIY.LowWord + Displacement);
                                    RegHL.HighByte = ReadMemory(RegWZ.LowWord);
                                    break;
                                case 0x67: // LD IYH, A
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegIY.HighByte = RegAF.HighByte;
                                    break;
                                case 0x68: // LD IYL, B
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegIY.LowByte = RegBC.HighByte;
                                    break;
                                case 0x69: // LD IYL, C
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegIY.LowByte = RegBC.LowByte;
                                    break;
                                case 0x6A: // LD IYL, D
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegIY.LowByte = RegDE.HighByte;
                                    break;
                                case 0x6B: // LD IYL, E
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegIY.LowByte = RegDE.LowByte;
                                    break;
                                case 0x6C: // LD IYL, IYH
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegIY.LowByte = RegIY.HighByte;
                                    break;
                                case 0x6D: // LD IYL, IYL
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    break;
                                case 0x6E: // LD L, (IY+d)
                                    totalExecutedCycles += 19; pendingCycles -= 19;
                                    Displacement = (sbyte)ReadOpArg(RegPC.LowWord++);
                                    RegWZ.LowWord = (ushort)(RegIY.LowWord + Displacement);
                                    RegHL.LowByte = ReadMemory(RegWZ.LowWord);
                                    break;
                                case 0x6F: // LD IYL, A
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegIY.LowByte = RegAF.HighByte;
                                    break;
                                case 0x70: // LD (IY+d), B
                                    totalExecutedCycles += 19; pendingCycles -= 19;
                                    Displacement = (sbyte)ReadOpArg(RegPC.LowWord++);
                                    RegWZ.LowWord = (ushort)(RegIY.LowWord + Displacement);
                                    WriteMemory(RegWZ.LowWord, RegBC.HighByte);
                                    break;
                                case 0x71: // LD (IY+d), C
                                    totalExecutedCycles += 19; pendingCycles -= 19;
                                    Displacement = (sbyte)ReadOpArg(RegPC.LowWord++);
                                    RegWZ.LowWord = (ushort)(RegIY.LowWord + Displacement);
                                    WriteMemory(RegWZ.LowWord, RegBC.LowByte);
                                    break;
                                case 0x72: // LD (IY+d), D
                                    totalExecutedCycles += 19; pendingCycles -= 19;
                                    Displacement = (sbyte)ReadOpArg(RegPC.LowWord++);
                                    RegWZ.LowWord = (ushort)(RegIY.LowWord + Displacement);
                                    WriteMemory(RegWZ.LowWord, RegDE.HighByte);
                                    break;
                                case 0x73: // LD (IY+d), E
                                    totalExecutedCycles += 19; pendingCycles -= 19;
                                    Displacement = (sbyte)ReadOpArg(RegPC.LowWord++);
                                    RegWZ.LowWord = (ushort)(RegIY.LowWord + Displacement);
                                    WriteMemory(RegWZ.LowWord, RegDE.LowByte);
                                    break;
                                case 0x74: // LD (IY+d), H
                                    totalExecutedCycles += 19; pendingCycles -= 19;
                                    Displacement = (sbyte)ReadOpArg(RegPC.LowWord++);
                                    RegWZ.LowWord = (ushort)(RegIY.LowWord + Displacement);
                                    WriteMemory(RegWZ.LowWord, RegHL.HighByte);
                                    break;
                                case 0x75: // LD (IY+d), L
                                    totalExecutedCycles += 19; pendingCycles -= 19;
                                    Displacement = (sbyte)ReadOpArg(RegPC.LowWord++);
                                    RegWZ.LowWord = (ushort)(RegIY.LowWord + Displacement);
                                    WriteMemory(RegWZ.LowWord, RegHL.LowByte);
                                    break;
                                case 0x76: // HALT
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    Halt();                                    
                                    break;
                                case 0x77: // LD (IY+d), A
                                    totalExecutedCycles += 19; pendingCycles -= 19;
                                    Displacement = (sbyte)ReadOpArg(RegPC.LowWord++);
                                    RegWZ.LowWord = (ushort)(RegIY.LowWord + Displacement);
                                    WriteMemory(RegWZ.LowWord, RegAF.HighByte);
                                    break;
                                case 0x78: // LD A, B
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.HighByte = RegBC.HighByte;                                    
                                    break;
                                case 0x79: // LD A, C
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.HighByte = RegBC.LowByte;
                                    break;
                                case 0x7A: // LD A, D
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.HighByte = RegDE.HighByte;
                                    break;
                                case 0x7B: // LD A, E
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.HighByte = RegDE.LowByte;
                                    break;
                                case 0x7C: // LD A, IYH
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegAF.HighByte = RegIY.HighByte;                                    
                                    break;
                                case 0x7D: // LD A, IYL
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegAF.HighByte = RegIY.LowByte;
                                    break;
                                case 0x7E: // LD A, (IY+d)
                                    totalExecutedCycles += 19; pendingCycles -= 19;
                                    Displacement = (sbyte)ReadOpArg(RegPC.LowWord++);
                                    RegWZ.LowWord = (ushort)(RegIY.LowWord + Displacement);
                                    RegAF.HighByte = ReadMemory(RegWZ.LowWord);
                                    break;
                                case 0x7F: // LD A, A
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    break;
                                case 0x80: // ADD A, B
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableALU[0, RegAF.HighByte, RegBC.HighByte, 0];                                    
                                    break;
                                case 0x81: // ADD A, C
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableALU[0, RegAF.HighByte, RegBC.LowByte, 0];
                                    break;
                                case 0x82: // ADD A, D
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableALU[0, RegAF.HighByte, RegDE.HighByte, 0];
                                    break;
                                case 0x83: // ADD A, E
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableALU[0, RegAF.HighByte, RegDE.LowByte, 0];
                                    break;
                                case 0x84: // ADD A, IYH
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegAF.LowWord = TableALU[0, RegAF.HighByte, RegIY.HighByte, 0];                                    
                                    break;
                                case 0x85: // ADD A, IYL
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegAF.LowWord = TableALU[0, RegAF.HighByte, RegIY.LowByte, 0];
                                    break;
                                case 0x86: // ADD A, (IY+d)
                                    totalExecutedCycles += 19; pendingCycles -= 19;//16
                                    Displacement = (sbyte)ReadOpArg(RegPC.LowWord++);
                                    RegWZ.LowWord = (ushort)(RegIY.LowWord + Displacement);
                                    RegAF.LowWord = TableALU[0, RegAF.HighByte, ReadMemory(RegWZ.LowWord), 0];
                                    break;
                                case 0x87: // ADD A, A
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableALU[0, RegAF.HighByte, RegAF.HighByte, 0];                                    
                                    break;
                                case 0x88: // ADC A, B
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableALU[1, RegAF.HighByte, RegBC.HighByte, RegFlagC ? 1 : 0];
                                    break;
                                case 0x89: // ADC A, C
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableALU[1, RegAF.HighByte, RegBC.LowByte, RegFlagC ? 1 : 0];
                                    break;
                                case 0x8A: // ADC A, D
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableALU[1, RegAF.HighByte, RegDE.HighByte, RegFlagC ? 1 : 0];
                                    break;
                                case 0x8B: // ADC A, E
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableALU[1, RegAF.HighByte, RegDE.LowByte, RegFlagC ? 1 : 0];
                                    break;
                                case 0x8C: // ADC A, IYH
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegAF.LowWord = TableALU[1, RegAF.HighByte, RegIY.HighByte, RegFlagC ? 1 : 0];                                    
                                    break;
                                case 0x8D: // ADC A, IYL
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegAF.LowWord = TableALU[1, RegAF.HighByte, RegIY.LowByte, RegFlagC ? 1 : 0];
                                    break;
                                case 0x8E: // ADC A, (IY+d)
                                    totalExecutedCycles += 19; pendingCycles -= 19;
                                    Displacement = (sbyte)ReadOpArg(RegPC.LowWord++);
                                    RegWZ.LowWord = (ushort)(RegIY.LowWord + Displacement);
                                    RegAF.LowWord = TableALU[1, RegAF.HighByte, ReadMemory(RegWZ.LowWord), RegFlagC ? 1 : 0];
                                    break;
                                case 0x8F: // ADC A, A
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableALU[1, RegAF.HighByte, RegAF.HighByte, RegFlagC ? 1 : 0];                                    
                                    break;
                                case 0x90: // SUB B
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableALU[2, RegAF.HighByte, RegBC.HighByte, 0];
                                    break;
                                case 0x91: // SUB C
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableALU[2, RegAF.HighByte, RegBC.LowByte, 0];
                                    break;
                                case 0x92: // SUB D
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableALU[2, RegAF.HighByte, RegDE.HighByte, 0];
                                    break;
                                case 0x93: // SUB E
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableALU[2, RegAF.HighByte, RegDE.LowByte, 0];
                                    break;
                                case 0x94: // SUB IYH
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegAF.LowWord = TableALU[2, RegAF.HighByte, RegIY.HighByte, 0];                                    
                                    break;
                                case 0x95: // SUB IYL
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegAF.LowWord = TableALU[2, RegAF.HighByte, RegIY.LowByte, 0];
                                    break;
                                case 0x96: // SUB (IY+d)
                                    totalExecutedCycles += 19; pendingCycles -= 19;
                                    Displacement = (sbyte)ReadOpArg(RegPC.LowWord++);
                                    RegWZ.LowWord = (ushort)(RegIY.LowWord + Displacement);
                                    RegAF.LowWord = TableALU[2, RegAF.HighByte, ReadMemory(RegWZ.LowWord), 0];
                                    break;
                                case 0x97: // SUB A, A
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableALU[2, RegAF.HighByte, RegAF.HighByte, 0];                                    
                                    break;
                                case 0x98: // SBC A, B
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableALU[3, RegAF.HighByte, RegBC.HighByte, RegFlagC ? 1 : 0];
                                    break;
                                case 0x99: // SBC A, C
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableALU[3, RegAF.HighByte, RegBC.LowByte, RegFlagC ? 1 : 0];
                                    break;
                                case 0x9A: // SBC A, D
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableALU[3, RegAF.HighByte, RegDE.HighByte, RegFlagC ? 1 : 0];
                                    break;
                                case 0x9B: // SBC A, E
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableALU[3, RegAF.HighByte, RegDE.LowByte, RegFlagC ? 1 : 0];
                                    break;
                                case 0x9C: // SBC A, IYH
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegAF.LowWord = TableALU[3, RegAF.HighByte, RegIY.HighByte, RegFlagC ? 1 : 0];                                    
                                    break;
                                case 0x9D: // SBC A, IYL
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegAF.LowWord = TableALU[3, RegAF.HighByte, RegIY.LowByte, RegFlagC ? 1 : 0];                                    
                                    break;
                                case 0x9E: // SBC A, (IY+d)
                                    totalExecutedCycles += 19; pendingCycles -= 19;
                                    Displacement = (sbyte)ReadOpArg(RegPC.LowWord++);
                                    RegWZ.LowWord = (ushort)(RegIY.LowWord + Displacement);
                                    RegAF.LowWord = TableALU[3, RegAF.HighByte, ReadMemory(RegWZ.LowWord), RegFlagC ? 1 : 0];
                                    break;
                                case 0x9F: // SBC A, A
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableALU[3, RegAF.HighByte, RegAF.HighByte, RegFlagC ? 1 : 0];                                    
                                    break;
                                case 0xA0: // AND B
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableALU[4, RegAF.HighByte, RegBC.HighByte, 0];                                    
                                    break;
                                case 0xA1: // AND C
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableALU[4, RegAF.HighByte, RegBC.LowByte, 0];
                                    break;
                                case 0xA2: // AND D
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableALU[4, RegAF.HighByte, RegDE.HighByte, 0];
                                    break;
                                case 0xA3: // AND E
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableALU[4, RegAF.HighByte, RegDE.LowByte, 0];
                                    break;
                                case 0xA4: // AND IYH
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegAF.LowWord = TableALU[4, RegAF.HighByte, RegIY.HighByte, 0];                                    
                                    break;
                                case 0xA5: // AND IYL
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegAF.LowWord = TableALU[4, RegAF.HighByte, RegIY.LowByte, 0];
                                    break;
                                case 0xA6: // AND (IY+d)
                                    totalExecutedCycles += 19; pendingCycles -= 19;
                                    Displacement = (sbyte)ReadOpArg(RegPC.LowWord++);
                                    RegWZ.LowWord = (ushort)(RegIY.LowWord + Displacement);
                                    RegAF.LowWord = TableALU[4, RegAF.HighByte, ReadMemory(RegWZ.LowWord), 0];
                                    break;
                                case 0xA7: // AND A
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableALU[4, RegAF.HighByte, RegAF.HighByte, 0];                                    
                                    break;
                                case 0xA8: // XOR B
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableALU[5, RegAF.HighByte, RegBC.HighByte, 0];
                                    break;
                                case 0xA9: // XOR C
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableALU[5, RegAF.HighByte, RegBC.LowByte, 0];
                                    break;
                                case 0xAA: // XOR D
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableALU[5, RegAF.HighByte, RegDE.HighByte, 0];
                                    break;
                                case 0xAB: // XOR E
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableALU[5, RegAF.HighByte, RegDE.LowByte, 0];
                                    break;
                                case 0xAC: // XOR IYH
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegAF.LowWord = TableALU[5, RegAF.HighByte, RegIY.HighByte, 0];                                    
                                    break;
                                case 0xAD: // XOR IYL
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegAF.LowWord = TableALU[5, RegAF.HighByte, RegIY.LowByte, 0];
                                    break;
                                case 0xAE: // XOR (IY+d)
                                    totalExecutedCycles += 19; pendingCycles -= 19;
                                    Displacement = (sbyte)ReadOpArg(RegPC.LowWord++);
                                    RegWZ.LowWord = (ushort)(RegIY.LowWord + Displacement);
                                    RegAF.LowWord = TableALU[5, RegAF.HighByte, ReadMemory(RegWZ.LowWord), 0];
                                    break;
                                case 0xAF: // XOR A
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableALU[5, RegAF.HighByte, RegAF.HighByte, 0];                                    
                                    break;
                                case 0xB0: // OR B
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableALU[6, RegAF.HighByte, RegBC.HighByte, 0];
                                    break;
                                case 0xB1: // OR C
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableALU[6, RegAF.HighByte, RegBC.LowByte, 0];
                                    break;
                                case 0xB2: // OR D
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableALU[6, RegAF.HighByte, RegDE.HighByte, 0];
                                    break;
                                case 0xB3: // OR E
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableALU[6, RegAF.HighByte, RegDE.LowByte, 0];
                                    break;
                                case 0xB4: // OR IYH
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegAF.LowWord = TableALU[6, RegAF.HighByte, RegIY.HighByte, 0];                                    
                                    break;
                                case 0xB5: // OR IYL
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegAF.LowWord = TableALU[6, RegAF.HighByte, RegIY.LowByte, 0];
                                    break;
                                case 0xB6: // OR (IY+d)
                                    totalExecutedCycles += 19; pendingCycles -= 19;
                                    Displacement = (sbyte)ReadOpArg(RegPC.LowWord++);
                                    RegWZ.LowWord = (ushort)(RegIY.LowWord + Displacement);
                                    RegAF.LowWord = TableALU[6, RegAF.HighByte, ReadMemory(RegWZ.LowWord), 0];
                                    break;
                                case 0xB7: // OR A
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableALU[6, RegAF.HighByte, RegAF.HighByte, 0];                                    
                                    break;
                                case 0xB8: // CP B
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableALU[7, RegAF.HighByte, RegBC.HighByte, 0];
                                    break;
                                case 0xB9: // CP C
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableALU[7, RegAF.HighByte, RegBC.LowByte, 0];
                                    break;
                                case 0xBA: // CP D
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableALU[7, RegAF.HighByte, RegDE.HighByte, 0];
                                    break;
                                case 0xBB: // CP E
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableALU[7, RegAF.HighByte, RegDE.LowByte, 0];
                                    break;
                                case 0xBC: // CP IYH
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegAF.LowWord = TableALU[7, RegAF.HighByte, RegIY.HighByte, 0];                                    
                                    break;
                                case 0xBD: // CP IYL
                                    totalExecutedCycles += 9; pendingCycles -= 9;
                                    RegAF.LowWord = TableALU[7, RegAF.HighByte, RegIY.LowByte, 0];
                                    break;
                                case 0xBE: // CP (IY+d)
                                    totalExecutedCycles += 19; pendingCycles -= 19;
                                    Displacement = (sbyte)ReadOpArg(RegPC.LowWord++);
                                    RegWZ.LowWord = (ushort)(RegIY.LowWord + Displacement);
                                    RegAF.LowWord = TableALU[7, RegAF.HighByte, ReadMemory(RegWZ.LowWord), 0];
                                    break;
                                case 0xBF: // CP A
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    RegAF.LowWord = TableALU[7, RegAF.HighByte, RegAF.HighByte, 0];                                    
                                    break;
                                case 0xC0: // RET NZ
                                    totalExecutedCycles += 5; pendingCycles -= 5;
                                    if (!RegFlagZ)
                                    {
                                        RegPC.LowByte = ReadMemory(RegSP.LowWord++); RegPC.HighByte = ReadMemory(RegSP.LowWord++);
                                        totalExecutedCycles += 6; pendingCycles -= 6;
                                    }
                                    break;
                                case 0xC1: // POP BC
                                    totalExecutedCycles += 10; pendingCycles -= 10;
                                    RegBC.LowByte = ReadMemory(RegSP.LowWord++); RegBC.HighByte = ReadMemory(RegSP.LowWord++);
                                    break;
                                case 0xC2: // JP NZ, nn
                                    totalExecutedCycles += 10; pendingCycles -= 10;
                                    TUS = (ushort)(ReadMemory(RegPC.LowWord++) + ReadMemory(RegPC.LowWord++) * 256);
                                    if (!RegFlagZ)
                                    {
                                        RegPC.LowWord = TUS;
                                    }
                                    break;
                                case 0xC3: // JP nn
                                    totalExecutedCycles += 10; pendingCycles -= 10;
                                    RegPC.LowWord = (ushort)(ReadMemory(RegPC.LowWord++) + ReadMemory(RegPC.LowWord++) * 256);
                                    break;
                                case 0xC4: // CALL NZ, nn
                                    totalExecutedCycles += 10; pendingCycles -= 10;
                                    TUS = (ushort)(ReadMemory(RegPC.LowWord++) + ReadMemory(RegPC.LowWord++) * 256);
                                    if (!RegFlagZ)
                                    {
                                        totalExecutedCycles += 7; pendingCycles -= 7;
                                        WriteMemory(--RegSP.LowWord, RegPC.HighByte); WriteMemory(--RegSP.LowWord, RegPC.LowByte);
                                        RegPC.LowWord = TUS;
                                    }
                                    break;
                                case 0xC5: // PUSH BC
                                    totalExecutedCycles += 11; pendingCycles -= 11;
                                    WriteMemory(--RegSP.LowWord, RegBC.HighByte); WriteMemory(--RegSP.LowWord, RegBC.LowByte);
                                    break;
                                case 0xC6: // ADD A, n
                                    totalExecutedCycles += 7; pendingCycles -= 7;
                                    RegAF.LowWord = TableALU[0, RegAF.HighByte, ReadMemory(RegPC.LowWord++), 0];
                                    break;
                                case 0xC7: // RST $00
                                    totalExecutedCycles += 11; pendingCycles -= 11;
                                    WriteMemory(--RegSP.LowWord, RegPC.HighByte); WriteMemory(--RegSP.LowWord, RegPC.LowByte);
                                    RegPC.LowWord = 0x00;
                                    break;
                                case 0xC8: // RET Z
                                    totalExecutedCycles += 5; pendingCycles -= 5;
                                    if (RegFlagZ)
                                    {
                                        RegPC.LowByte = ReadMemory(RegSP.LowWord++); RegPC.HighByte = ReadMemory(RegSP.LowWord++);
                                        totalExecutedCycles += 6; pendingCycles -= 6;
                                    }
                                    break;
                                case 0xC9: // RET
                                    totalExecutedCycles += 10; pendingCycles -= 10;
                                    RegPC.LowByte = ReadMemory(RegSP.LowWord++); RegPC.HighByte = ReadMemory(RegSP.LowWord++);
                                    break;
                                case 0xCA: // JP Z, nn
                                    totalExecutedCycles += 10; pendingCycles -= 10;
                                    TUS = (ushort)(ReadMemory(RegPC.LowWord++) + ReadMemory(RegPC.LowWord++) * 256);
                                    if (RegFlagZ)
                                    {
                                        RegPC.LowWord = TUS;
                                    }
                                    break;
                                case 0xCB: // (Prefix)
                                    Displacement = (sbyte)ReadOpArg(RegPC.LowWord++);
                                    //++RegR;
                                    RegWZ.LowWord = (ushort)(RegIY.LowWord + Displacement);
                                    switch (ReadOpArg(RegPC.LowWord++))
                                    {
                                        case 0x00: // RLC (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 0, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            break;
                                        case 0x01: // RLC (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 0, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            break;
                                        case 0x02: // RLC (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 0, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            break;
                                        case 0x03: // RLC (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 0, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            break;
                                        case 0x04: // RLC (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 0, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            break;
                                        case 0x05: // RLC (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 0, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            break;
                                        case 0x06: // RLC (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 0, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            break;
                                        case 0x07: // RLC (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 0, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            break;
                                        case 0x08: // RRC (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 1, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            break;
                                        case 0x09: // RRC (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 1, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            break;
                                        case 0x0A: // RRC (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 1, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            break;
                                        case 0x0B: // RRC (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 1, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            break;
                                        case 0x0C: // RRC (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 1, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            break;
                                        case 0x0D: // RRC (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 1, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            break;
                                        case 0x0E: // RRC (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 1, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            break;
                                        case 0x0F: // RRC (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 1, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            break;
                                        case 0x10: // RL (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 2, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            break;
                                        case 0x11: // RL (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 2, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            break;
                                        case 0x12: // RL (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 2, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            break;
                                        case 0x13: // RL (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 2, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            break;
                                        case 0x14: // RL (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 2, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            break;
                                        case 0x15: // RL (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 2, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            break;
                                        case 0x16: // RL (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 2, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            break;
                                        case 0x17: // RL (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 2, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            break;
                                        case 0x18: // RR (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 3, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            break;
                                        case 0x19: // RR (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 3, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            break;
                                        case 0x1A: // RR (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 3, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            break;
                                        case 0x1B: // RR (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 3, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            break;
                                        case 0x1C: // RR (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 3, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            break;
                                        case 0x1D: // RR (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 3, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            break;
                                        case 0x1E: // RR (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 3, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            break;
                                        case 0x1F: // RR (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 3, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            break;
                                        case 0x20: // SLA (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 4, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            break;
                                        case 0x21: // SLA (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 4, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            break;
                                        case 0x22: // SLA (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 4, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            break;
                                        case 0x23: // SLA (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 4, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            break;
                                        case 0x24: // SLA (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 4, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            break;
                                        case 0x25: // SLA (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 4, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            break;
                                        case 0x26: // SLA (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 4, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            break;
                                        case 0x27: // SLA (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 4, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            break;
                                        case 0x28: // SRA (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 5, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            break;
                                        case 0x29: // SRA (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 5, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            break;
                                        case 0x2A: // SRA (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 5, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            break;
                                        case 0x2B: // SRA (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 5, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            break;
                                        case 0x2C: // SRA (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 5, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            break;
                                        case 0x2D: // SRA (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 5, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            break;
                                        case 0x2E: // SRA (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 5, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            break;
                                        case 0x2F: // SRA (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 5, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            break;
                                        case 0x30: // SL1 (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 6, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            break;
                                        case 0x31: // SL1 (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 6, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            break;
                                        case 0x32: // SL1 (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 6, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            break;
                                        case 0x33: // SL1 (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 6, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            break;
                                        case 0x34: // SL1 (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 6, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            break;
                                        case 0x35: // SL1 (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 6, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            break;
                                        case 0x36: // SL1 (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 6, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            break;
                                        case 0x37: // SL1 (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 6, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            break;
                                        case 0x38: // SRL (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 7, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            break;
                                        case 0x39: // SRL (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 7, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            break;
                                        case 0x3A: // SRL (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 7, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            break;
                                        case 0x3B: // SRL (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 7, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            break;
                                        case 0x3C: // SRL (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 7, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            break;
                                        case 0x3D: // SRL (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 7, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            break;
                                        case 0x3E: // SRL (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 7, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            break;
                                        case 0x3F: // SRL (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            TUS = TableRotShift[1, 7, RegAF.LowByte + 256 * ReadMemory(RegWZ.LowWord)];
                                            WriteMemory(RegWZ.LowWord, (byte)(TUS >> 8));
                                            RegAF.LowByte = (byte)TUS;
                                            break;
                                        case 0x40: // BIT 0, (IY+d)
                                        case 0x41: // BIT 0, (IY+d)
                                        case 0x42: // BIT 0, (IY+d)
                                        case 0x43: // BIT 0, (IY+d)
                                        case 0x44: // BIT 0, (IY+d)
                                        case 0x45: // BIT 0, (IY+d)
                                        case 0x46: // BIT 0, (IY+d)
                                        case 0x47: // BIT 0, (IY+d)
                                            totalExecutedCycles += 20; pendingCycles -= 20;
                                            TBOOL = (ReadMemory(RegWZ.LowWord) & 0x01) == 0;
                                            RegFlagN = false;
                                            RegFlagP = TBOOL;
                                            RegFlag3 = ((RegWZ.LowWord >> 8) & 0x08) != 0;
                                            RegFlagH = true;
                                            RegFlag5 = ((RegWZ.LowWord >> 8) & 0x20) != 0;
                                            RegFlagZ = TBOOL;
                                            RegFlagS = false;
                                            break;
                                        case 0x48: // BIT 1, (IY+d)
                                        case 0x49: // BIT 1, (IY+d)
                                        case 0x4A: // BIT 1, (IY+d)
                                        case 0x4B: // BIT 1, (IY+d)
                                        case 0x4C: // BIT 1, (IY+d)
                                        case 0x4D: // BIT 1, (IY+d)
                                        case 0x4E: // BIT 1, (IY+d)
                                        case 0x4F: // BIT 1, (IY+d)
                                            totalExecutedCycles += 20; pendingCycles -= 20;
                                            TBOOL = (ReadMemory(RegWZ.LowWord) & 0x02) == 0;
                                            RegFlagN = false;
                                            RegFlagP = TBOOL;
                                            RegFlag3 = ((RegWZ.LowWord >> 8) & 0x08) != 0;
                                            RegFlagH = true;
                                            RegFlag5 = ((RegWZ.LowWord >> 8) & 0x20) != 0;
                                            RegFlagZ = TBOOL;
                                            RegFlagS = false;
                                            break;
                                        case 0x50: // BIT 2, (IY+d)
                                        case 0x51: // BIT 2, (IY+d)
                                        case 0x52: // BIT 2, (IY+d)
                                        case 0x53: // BIT 2, (IY+d)
                                        case 0x54: // BIT 2, (IY+d)
                                        case 0x55: // BIT 2, (IY+d)
                                        case 0x56: // BIT 2, (IY+d)
                                        case 0x57: // BIT 2, (IY+d)
                                            totalExecutedCycles += 20; pendingCycles -= 20;
                                            TBOOL = (ReadMemory(RegWZ.LowWord) & 0x04) == 0;
                                            RegFlagN = false;
                                            RegFlagP = TBOOL;
                                            RegFlag3 = ((RegWZ.LowWord >> 8) & 0x08) != 0;
                                            RegFlagH = true;
                                            RegFlag5 = ((RegWZ.LowWord >> 8) & 0x20) != 0;
                                            RegFlagZ = TBOOL;
                                            RegFlagS = false;
                                            break;
                                        case 0x58: // BIT 3, (IY+d)
                                        case 0x59: // BIT 3, (IY+d)
                                        case 0x5A: // BIT 3, (IY+d)
                                        case 0x5B: // BIT 3, (IY+d)
                                        case 0x5C: // BIT 3, (IY+d)
                                        case 0x5D: // BIT 3, (IY+d)
                                        case 0x5E: // BIT 3, (IY+d)
                                        case 0x5F: // BIT 3, (IY+d)
                                            totalExecutedCycles += 20; pendingCycles -= 20;
                                            TBOOL = (ReadMemory(RegWZ.LowWord) & 0x08) == 0;
                                            RegFlagN = false;
                                            RegFlagP = TBOOL;
                                            RegFlag3 = ((RegWZ.LowWord >> 8) & 0x08) != 0;
                                            RegFlagH = true;
                                            RegFlag5 = ((RegWZ.LowWord >> 8) & 0x20) != 0;
                                            RegFlagZ = TBOOL;
                                            RegFlagS = false;
                                            break;
                                        case 0x60: // BIT 4, (IY+d)
                                        case 0x61: // BIT 4, (IY+d)
                                        case 0x62: // BIT 4, (IY+d)
                                        case 0x63: // BIT 4, (IY+d)
                                        case 0x64: // BIT 4, (IY+d)
                                        case 0x65: // BIT 4, (IY+d)
                                        case 0x66: // BIT 4, (IY+d)
                                        case 0x67: // BIT 4, (IY+d)
                                            totalExecutedCycles += 20; pendingCycles -= 20;
                                            TBOOL = (ReadMemory(RegWZ.LowWord) & 0x10) == 0;
                                            RegFlagN = false;
                                            RegFlagP = TBOOL;
                                            RegFlag3 = ((RegWZ.LowWord >> 8) & 0x08) != 0;
                                            RegFlagH = true;
                                            RegFlag5 = ((RegWZ.LowWord >> 8) & 0x20) != 0;
                                            RegFlagZ = TBOOL;
                                            RegFlagS = false;
                                            break;
                                        case 0x68: // BIT 5, (IY+d)
                                        case 0x69: // BIT 5, (IY+d)
                                        case 0x6A: // BIT 5, (IY+d)
                                        case 0x6B: // BIT 5, (IY+d)
                                        case 0x6C: // BIT 5, (IY+d)
                                        case 0x6D: // BIT 5, (IY+d)
                                        case 0x6E: // BIT 5, (IY+d)
                                        case 0x6F: // BIT 5, (IY+d)
                                            totalExecutedCycles += 20; pendingCycles -= 20;
                                            TBOOL = (ReadMemory(RegWZ.LowWord) & 0x20) == 0;
                                            RegFlagN = false;
                                            RegFlagP = TBOOL;
                                            RegFlag3 = ((RegWZ.LowWord >> 8) & 0x08) != 0;
                                            RegFlagH = true;
                                            RegFlag5 = ((RegWZ.LowWord >> 8) & 0x20) != 0;
                                            RegFlagZ = TBOOL;
                                            RegFlagS = false;
                                            break;
                                        case 0x70: // BIT 6, (IY+d)
                                        case 0x71: // BIT 6, (IY+d)
                                        case 0x72: // BIT 6, (IY+d)
                                        case 0x73: // BIT 6, (IY+d)
                                        case 0x74: // BIT 6, (IY+d)
                                        case 0x75: // BIT 6, (IY+d)
                                        case 0x76: // BIT 6, (IY+d)
                                        case 0x77: // BIT 6, (IY+d)
                                            totalExecutedCycles += 20; pendingCycles -= 20;
                                            TBOOL = (ReadMemory(RegWZ.LowWord) & 0x40) == 0;
                                            RegFlagN = false;
                                            RegFlagP = TBOOL;
                                            RegFlag3 = ((RegWZ.LowWord >> 8) & 0x08) != 0;
                                            RegFlagH = true;
                                            RegFlag5 = ((RegWZ.LowWord >> 8) & 0x20) != 0;
                                            RegFlagZ = TBOOL;
                                            RegFlagS = false;
                                            break;
                                        case 0x78: // BIT 7, (IY+d)
                                        case 0x79: // BIT 7, (IY+d)
                                        case 0x7A: // BIT 7, (IY+d)
                                        case 0x7B: // BIT 7, (IY+d)
                                        case 0x7C: // BIT 7, (IY+d)
                                        case 0x7D: // BIT 7, (IY+d)
                                        case 0x7E: // BIT 7, (IY+d)
                                        case 0x7F: // BIT 7, (IY+d)
                                            totalExecutedCycles += 20; pendingCycles -= 20;
                                            TBOOL = (ReadMemory(RegWZ.LowWord) & 0x80) == 0;
                                            RegFlagN = false;
                                            RegFlagP = TBOOL;
                                            RegFlag3 = ((RegWZ.LowWord >> 8) & 0x08) != 0;
                                            RegFlagH = true;
                                            RegFlag5 = ((RegWZ.LowWord >> 8) & 0x20) != 0;
                                            RegFlagZ = TBOOL;
                                            RegFlagS = !TBOOL;
                                            break;
                                        case 0x80: // RES 0, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x01)));
                                            break;
                                        case 0x81: // RES 0, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x01)));
                                            break;
                                        case 0x82: // RES 0, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x01)));
                                            break;
                                        case 0x83: // RES 0, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x01)));
                                            break;
                                        case 0x84: // RES 0, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x01)));
                                            break;
                                        case 0x85: // RES 0, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x01)));
                                            break;
                                        case 0x86: // RES 0, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x01)));
                                            break;
                                        case 0x87: // RES 0, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x01)));
                                            break;
                                        case 0x88: // RES 1, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x02)));
                                            break;
                                        case 0x89: // RES 1, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x02)));
                                            break;
                                        case 0x8A: // RES 1, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x02)));
                                            break;
                                        case 0x8B: // RES 1, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x02)));
                                            break;
                                        case 0x8C: // RES 1, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x02)));
                                            break;
                                        case 0x8D: // RES 1, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x02)));
                                            break;
                                        case 0x8E: // RES 1, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x02)));
                                            break;
                                        case 0x8F: // RES 1, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x02)));
                                            break;
                                        case 0x90: // RES 2, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x04)));
                                            break;
                                        case 0x91: // RES 2, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x04)));
                                            break;
                                        case 0x92: // RES 2, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x04)));
                                            break;
                                        case 0x93: // RES 2, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x04)));
                                            break;
                                        case 0x94: // RES 2, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x04)));
                                            break;
                                        case 0x95: // RES 2, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x04)));
                                            break;
                                        case 0x96: // RES 2, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x04)));
                                            break;
                                        case 0x97: // RES 2, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x04)));
                                            break;
                                        case 0x98: // RES 3, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x08)));
                                            break;
                                        case 0x99: // RES 3, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x08)));
                                            break;
                                        case 0x9A: // RES 3, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x08)));
                                            break;
                                        case 0x9B: // RES 3, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x08)));
                                            break;
                                        case 0x9C: // RES 3, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x08)));
                                            break;
                                        case 0x9D: // RES 3, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x08)));
                                            break;
                                        case 0x9E: // RES 3, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x08)));
                                            break;
                                        case 0x9F: // RES 3, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x08)));
                                            break;
                                        case 0xA0: // RES 4, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x10)));
                                            break;
                                        case 0xA1: // RES 4, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x10)));
                                            break;
                                        case 0xA2: // RES 4, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x10)));
                                            break;
                                        case 0xA3: // RES 4, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x10)));
                                            break;
                                        case 0xA4: // RES 4, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x10)));
                                            break;
                                        case 0xA5: // RES 4, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x10)));
                                            break;
                                        case 0xA6: // RES 4, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x10)));
                                            break;
                                        case 0xA7: // RES 4, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x10)));
                                            break;
                                        case 0xA8: // RES 5, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x20)));
                                            break;
                                        case 0xA9: // RES 5, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x20)));
                                            break;
                                        case 0xAA: // RES 5, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x20)));
                                            break;
                                        case 0xAB: // RES 5, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x20)));
                                            break;
                                        case 0xAC: // RES 5, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x20)));
                                            break;
                                        case 0xAD: // RES 5, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x20)));
                                            break;
                                        case 0xAE: // RES 5, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x20)));
                                            break;
                                        case 0xAF: // RES 5, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x20)));
                                            break;
                                        case 0xB0: // RES 6, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x40)));
                                            break;
                                        case 0xB1: // RES 6, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x40)));
                                            break;
                                        case 0xB2: // RES 6, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x40)));
                                            break;
                                        case 0xB3: // RES 6, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x40)));
                                            break;
                                        case 0xB4: // RES 6, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x40)));
                                            break;
                                        case 0xB5: // RES 6, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x40)));
                                            break;
                                        case 0xB6: // RES 6, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x40)));
                                            break;
                                        case 0xB7: // RES 6, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x40)));
                                            break;
                                        case 0xB8: // RES 7, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x80)));
                                            break;
                                        case 0xB9: // RES 7, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x80)));
                                            break;
                                        case 0xBA: // RES 7, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x80)));
                                            break;
                                        case 0xBB: // RES 7, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x80)));
                                            break;
                                        case 0xBC: // RES 7, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x80)));
                                            break;
                                        case 0xBD: // RES 7, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x80)));
                                            break;
                                        case 0xBE: // RES 7, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x80)));
                                            break;
                                        case 0xBF: // RES 7, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) & unchecked((byte)~0x80)));
                                            break;
                                        case 0xC0: // SET 0, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x01)));
                                            break;
                                        case 0xC1: // SET 0, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x01)));
                                            break;
                                        case 0xC2: // SET 0, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x01)));
                                            break;
                                        case 0xC3: // SET 0, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x01)));
                                            break;
                                        case 0xC4: // SET 0, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x01)));
                                            break;
                                        case 0xC5: // SET 0, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x01)));
                                            break;
                                        case 0xC6: // SET 0, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x01)));
                                            break;
                                        case 0xC7: // SET 0, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x01)));
                                            break;
                                        case 0xC8: // SET 1, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x02)));
                                            break;
                                        case 0xC9: // SET 1, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x02)));
                                            break;
                                        case 0xCA: // SET 1, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x02)));
                                            break;
                                        case 0xCB: // SET 1, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x02)));
                                            break;
                                        case 0xCC: // SET 1, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x02)));
                                            break;
                                        case 0xCD: // SET 1, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x02)));
                                            break;
                                        case 0xCE: // SET 1, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x02)));
                                            break;
                                        case 0xCF: // SET 1, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x02)));
                                            break;
                                        case 0xD0: // SET 2, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x04)));
                                            break;
                                        case 0xD1: // SET 2, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x04)));
                                            break;
                                        case 0xD2: // SET 2, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x04)));
                                            break;
                                        case 0xD3: // SET 2, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x04)));
                                            break;
                                        case 0xD4: // SET 2, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x04)));
                                            break;
                                        case 0xD5: // SET 2, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x04)));
                                            break;
                                        case 0xD6: // SET 2, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x04)));
                                            break;
                                        case 0xD7: // SET 2, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x04)));
                                            break;
                                        case 0xD8: // SET 3, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x08)));
                                            break;
                                        case 0xD9: // SET 3, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x08)));
                                            break;
                                        case 0xDA: // SET 3, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x08)));
                                            break;
                                        case 0xDB: // SET 3, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x08)));
                                            break;
                                        case 0xDC: // SET 3, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x08)));
                                            break;
                                        case 0xDD: // SET 3, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x08)));
                                            break;
                                        case 0xDE: // SET 3, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x08)));
                                            break;
                                        case 0xDF: // SET 3, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x08)));
                                            break;
                                        case 0xE0: // SET 4, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x10)));
                                            break;
                                        case 0xE1: // SET 4, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x10)));
                                            break;
                                        case 0xE2: // SET 4, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x10)));
                                            break;
                                        case 0xE3: // SET 4, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x10)));
                                            break;
                                        case 0xE4: // SET 4, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x10)));
                                            break;
                                        case 0xE5: // SET 4, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x10)));
                                            break;
                                        case 0xE6: // SET 4, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x10)));
                                            break;
                                        case 0xE7: // SET 4, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x10)));
                                            break;
                                        case 0xE8: // SET 5, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x20)));
                                            break;
                                        case 0xE9: // SET 5, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x20)));
                                            break;
                                        case 0xEA: // SET 5, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x20)));
                                            break;
                                        case 0xEB: // SET 5, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x20)));
                                            break;
                                        case 0xEC: // SET 5, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x20)));
                                            break;
                                        case 0xED: // SET 5, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x20)));
                                            break;
                                        case 0xEE: // SET 5, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x20)));
                                            break;
                                        case 0xEF: // SET 5, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x20)));
                                            break;
                                        case 0xF0: // SET 6, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x40)));
                                            break;
                                        case 0xF1: // SET 6, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x40)));
                                            break;
                                        case 0xF2: // SET 6, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x40)));
                                            break;
                                        case 0xF3: // SET 6, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x40)));
                                            break;
                                        case 0xF4: // SET 6, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x40)));
                                            break;
                                        case 0xF5: // SET 6, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x40)));
                                            break;
                                        case 0xF6: // SET 6, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x40)));
                                            break;
                                        case 0xF7: // SET 6, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x40)));
                                            break;
                                        case 0xF8: // SET 7, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x80)));
                                            break;
                                        case 0xF9: // SET 7, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x80)));
                                            break;
                                        case 0xFA: // SET 7, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x80)));
                                            break;
                                        case 0xFB: // SET 7, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x80)));
                                            break;
                                        case 0xFC: // SET 7, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x80)));
                                            break;
                                        case 0xFD: // SET 7, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x80)));
                                            break;
                                        case 0xFE: // SET 7, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x80)));
                                            break;
                                        case 0xFF: // SET 7, (IY+d)
                                            totalExecutedCycles += 23; pendingCycles -= 23;
                                            WriteMemory(RegWZ.LowWord, (byte)(ReadMemory(RegWZ.LowWord) | unchecked((byte)0x80)));
                                            break;
                                    }
                                    break;
                                case 0xCC: // CALL Z, nn
                                    totalExecutedCycles += 10; pendingCycles -= 10;
                                    TUS = (ushort)(ReadMemory(RegPC.LowWord++) + ReadMemory(RegPC.LowWord++) * 256);
                                    if (RegFlagZ)
                                    {
                                        totalExecutedCycles += 7; pendingCycles -= 7;
                                        WriteMemory(--RegSP.LowWord, RegPC.HighByte); WriteMemory(--RegSP.LowWord, RegPC.LowByte);
                                        RegPC.LowWord = TUS;
                                    }
                                    break;
                                case 0xCD: // CALL nn
                                    totalExecutedCycles += 17; pendingCycles -= 17;
                                    TUS = (ushort)(ReadMemory(RegPC.LowWord++) + ReadMemory(RegPC.LowWord++) * 256);
                                    WriteMemory(--RegSP.LowWord, RegPC.HighByte); WriteMemory(--RegSP.LowWord, RegPC.LowByte);
                                    RegPC.LowWord = TUS;
                                    break;
                                case 0xCE: // ADC A, n
                                    totalExecutedCycles += 7; pendingCycles -= 7;
                                    RegAF.LowWord = TableALU[1, RegAF.HighByte, ReadMemory(RegPC.LowWord++), RegFlagC ? 1 : 0];
                                    break;
                                case 0xCF: // RST $08
                                    totalExecutedCycles += 11; pendingCycles -= 11;
                                    WriteMemory(--RegSP.LowWord, RegPC.HighByte); WriteMemory(--RegSP.LowWord, RegPC.LowByte);
                                    RegPC.LowWord = 0x08;
                                    break;
                                case 0xD0: // RET NC
                                    totalExecutedCycles += 5; pendingCycles -= 5;
                                    if (!RegFlagC)
                                    {
                                        RegPC.LowByte = ReadMemory(RegSP.LowWord++); RegPC.HighByte = ReadMemory(RegSP.LowWord++);
                                        totalExecutedCycles += 6; pendingCycles -= 6;
                                    }
                                    break;
                                case 0xD1: // POP DE
                                    totalExecutedCycles += 10; pendingCycles -= 10;
                                    RegDE.LowByte = ReadMemory(RegSP.LowWord++); RegDE.HighByte = ReadMemory(RegSP.LowWord++);
                                    break;
                                case 0xD2: // JP NC, nn
                                    totalExecutedCycles += 10; pendingCycles -= 10;
                                    TUS = (ushort)(ReadMemory(RegPC.LowWord++) + ReadMemory(RegPC.LowWord++) * 256);
                                    if (!RegFlagC)
                                    {
                                        RegPC.LowWord = TUS;
                                    }
                                    break;
                                case 0xD3: // OUT n, A
                                    totalExecutedCycles += 11; pendingCycles -= 11;
                                    WriteHardware(ReadMemory(RegPC.LowWord++), RegAF.HighByte);
                                    break;
                                case 0xD4: // CALL NC, nn
                                    totalExecutedCycles += 10; pendingCycles -= 10;
                                    TUS = (ushort)(ReadMemory(RegPC.LowWord++) + ReadMemory(RegPC.LowWord++) * 256);
                                    if (!RegFlagC)
                                    {
                                        totalExecutedCycles += 7; pendingCycles -= 7;
                                        WriteMemory(--RegSP.LowWord, RegPC.HighByte); WriteMemory(--RegSP.LowWord, RegPC.LowByte);
                                        RegPC.LowWord = TUS;
                                    }
                                    break;
                                case 0xD5: // PUSH DE
                                    totalExecutedCycles += 11; pendingCycles -= 11;
                                    WriteMemory(--RegSP.LowWord, RegDE.HighByte); WriteMemory(--RegSP.LowWord, RegDE.LowByte);
                                    break;
                                case 0xD6: // SUB n
                                    totalExecutedCycles += 7; pendingCycles -= 7;
                                    RegAF.LowWord = TableALU[2, RegAF.HighByte, ReadMemory(RegPC.LowWord++), 0];
                                    break;
                                case 0xD7: // RST $10
                                    totalExecutedCycles += 11; pendingCycles -= 11;
                                    WriteMemory(--RegSP.LowWord, RegPC.HighByte); WriteMemory(--RegSP.LowWord, RegPC.LowByte);
                                    RegPC.LowWord = 0x10;
                                    break;
                                case 0xD8: // RET C
                                    totalExecutedCycles += 5; pendingCycles -= 5;
                                    if (RegFlagC)
                                    {
                                        RegPC.LowByte = ReadMemory(RegSP.LowWord++); RegPC.HighByte = ReadMemory(RegSP.LowWord++);
                                        totalExecutedCycles += 6; pendingCycles -= 6;
                                    }
                                    break;
                                case 0xD9: // EXX
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    TUS = RegBC.LowWord; RegBC.LowWord = RegAltBC.LowWord; RegAltBC.LowWord = TUS;
                                    TUS = RegDE.LowWord; RegDE.LowWord = RegAltDE.LowWord; RegAltDE.LowWord = TUS;
                                    TUS = RegHL.LowWord; RegHL.LowWord = RegAltHL.LowWord; RegAltHL.LowWord = TUS;                                    
                                    break;
                                case 0xDA: // JP C, nn
                                    totalExecutedCycles += 10; pendingCycles -= 10;
                                    TUS = (ushort)(ReadMemory(RegPC.LowWord++) + ReadMemory(RegPC.LowWord++) * 256);
                                    if (RegFlagC)
                                    {
                                        RegPC.LowWord = TUS;
                                    }
                                    break;
                                case 0xDB: // IN A, n
                                    totalExecutedCycles += 11; pendingCycles -= 11;
                                    TUS = (ushort)(ReadOpArg(RegPC.LowWord++) | (RegAF.HighByte << 8));
                                    RegAF.HighByte = ReadHardware(TUS);
                                    RegWZ.LowWord = (ushort)(TUS + 1);
                                    break;
                                case 0xDC: // CALL C, nn
                                    totalExecutedCycles += 10; pendingCycles -= 10;
                                    TUS = (ushort)(ReadMemory(RegPC.LowWord++) + ReadMemory(RegPC.LowWord++) * 256);
                                    if (RegFlagC)
                                    {
                                        totalExecutedCycles += 7; pendingCycles -= 7;
                                        WriteMemory(--RegSP.LowWord, RegPC.HighByte); WriteMemory(--RegSP.LowWord, RegPC.LowByte);
                                        RegPC.LowWord = TUS;
                                    }
                                    break;
                                case 0xDD: // <-
                                    // Invalid sequence.
                                    totalExecutedCycles += 1337; pendingCycles -= 1337;
                                    break;
                                case 0xDE: // SBC A, n
                                    totalExecutedCycles += 7; pendingCycles -= 7;
                                    RegAF.LowWord = TableALU[3, RegAF.HighByte, ReadMemory(RegPC.LowWord++), RegFlagC ? 1 : 0];
                                    break;
                                case 0xDF: // RST $18
                                    totalExecutedCycles += 11; pendingCycles -= 11;
                                    WriteMemory(--RegSP.LowWord, RegPC.HighByte); WriteMemory(--RegSP.LowWord, RegPC.LowByte);
                                    RegPC.LowWord = 0x18;
                                    break;
                                case 0xE0: // RET PO
                                    totalExecutedCycles += 5; pendingCycles -= 5;
                                    if (!RegFlagP)
                                    {
                                        RegPC.LowByte = ReadMemory(RegSP.LowWord++); RegPC.HighByte = ReadMemory(RegSP.LowWord++);
                                        totalExecutedCycles += 6; pendingCycles -= 6;
                                    }
                                    break;
                                case 0xE1: // POP IY
                                    totalExecutedCycles += 14; pendingCycles -= 14;
                                    RegIY.LowByte = ReadMemory(RegSP.LowWord++); RegIY.HighByte = ReadMemory(RegSP.LowWord++);
                                    break;
                                case 0xE2: // JP PO, nn
                                    totalExecutedCycles += 10; pendingCycles -= 10;
                                    TUS = (ushort)(ReadMemory(RegPC.LowWord++) + ReadMemory(RegPC.LowWord++) * 256);
                                    if (!RegFlagP)
                                    {
                                        RegPC.LowWord = TUS;
                                    }
                                    break;
                                case 0xE3: // EX (SP), IY
                                    totalExecutedCycles += 23; pendingCycles -= 23;
                                    TUS = RegSP.LowWord; TBL = ReadMemory(TUS++); TBH = ReadMemory(TUS--);
                                    WriteMemory(TUS++, RegIY.LowByte); WriteMemory(TUS, RegIY.HighByte);
                                    RegIY.LowByte = TBL; RegIY.HighByte = TBH;
                                    RegWZ.LowWord = RegIY.LowWord;
                                    break;
                                case 0xE4: // CALL C, nn
                                    totalExecutedCycles += 10; pendingCycles -= 10;
                                    TUS = (ushort)(ReadMemory(RegPC.LowWord++) + ReadMemory(RegPC.LowWord++) * 256);
                                    if (RegFlagC)
                                    {
                                        totalExecutedCycles += 7; pendingCycles -= 7;
                                        WriteMemory(--RegSP.LowWord, RegPC.HighByte); WriteMemory(--RegSP.LowWord, RegPC.LowByte);
                                        RegPC.LowWord = TUS;
                                    }
                                    break;
                                case 0xE5: // PUSH IY
                                    totalExecutedCycles += 15; pendingCycles -= 15;
                                    WriteMemory(--RegSP.LowWord, RegIY.HighByte); WriteMemory(--RegSP.LowWord, RegIY.LowByte);
                                    break;
                                case 0xE6: // AND n
                                    totalExecutedCycles += 7; pendingCycles -= 7;
                                    RegAF.LowWord = TableALU[4, RegAF.HighByte, ReadMemory(RegPC.LowWord++), 0];
                                    break;
                                case 0xE7: // RST $20
                                    totalExecutedCycles += 11; pendingCycles -= 11;
                                    WriteMemory(--RegSP.LowWord, RegPC.HighByte); WriteMemory(--RegSP.LowWord, RegPC.LowByte);
                                    RegPC.LowWord = 0x20;
                                    break;
                                case 0xE8: // RET PE
                                    totalExecutedCycles += 5; pendingCycles -= 5;
                                    if (RegFlagP)
                                    {
                                        RegPC.LowByte = ReadMemory(RegSP.LowWord++); RegPC.HighByte = ReadMemory(RegSP.LowWord++);
                                        totalExecutedCycles += 6; pendingCycles -= 6;
                                    }
                                    break;
                                case 0xE9: // JP IY
                                    totalExecutedCycles += 8; pendingCycles -= 8;
                                    RegPC.LowWord = RegIY.LowWord;                                    
                                    break;
                                case 0xEA: // JP PE, nn
                                    totalExecutedCycles += 10; pendingCycles -= 10;
                                    TUS = (ushort)(ReadMemory(RegPC.LowWord++) + ReadMemory(RegPC.LowWord++) * 256);
                                    if (RegFlagP)
                                    {
                                        RegPC.LowWord = TUS;
                                    }
                                    break;
                                case 0xEB: // EX DE, HL
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    TUS = RegDE.LowWord; RegDE.LowWord = RegHL.LowWord; RegHL.LowWord = TUS;                                    
                                    break;
                                case 0xEC: // CALL PE, nn
                                    totalExecutedCycles += 10; pendingCycles -= 10;
                                    TUS = (ushort)(ReadMemory(RegPC.LowWord++) + ReadMemory(RegPC.LowWord++) * 256);
                                    if (RegFlagP)
                                    {
                                        totalExecutedCycles += 7; pendingCycles -= 7;
                                        WriteMemory(--RegSP.LowWord, RegPC.HighByte); WriteMemory(--RegSP.LowWord, RegPC.LowByte);
                                        RegPC.LowWord = TUS;
                                    }
                                    break;
                                case 0xED: // (Prefix)
                                    ++RegR;
                                    switch (ReadOp(RegPC.LowWord++))
                                    {
                                        case 0x00: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x01: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x02: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x03: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x04: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x05: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x06: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x07: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x08: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x09: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x0A: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x0B: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x0C: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x0D: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x0E: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x0F: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x10: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x11: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x12: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x13: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x14: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x15: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x16: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x17: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x18: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x19: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x1A: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x1B: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x1C: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x1D: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x1E: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x1F: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x20: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x21: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x22: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x23: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x24: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x25: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x26: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x27: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x28: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x29: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x2A: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x2B: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x2C: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x2D: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x2E: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x2F: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x30: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x31: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x32: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x33: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x34: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x35: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x36: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x37: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x38: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x39: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x3A: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x3B: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x3C: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x3D: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x3E: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x3F: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x40: // IN B, C
                                            totalExecutedCycles += 12; pendingCycles -= 12;
                                            RegBC.HighByte = ReadHardware(RegBC.LowWord);
                                            RegFlagS = RegBC.HighByte > 127;
                                            RegFlagZ = RegBC.HighByte == 0;
                                            RegFlag5 = (RegBC.HighByte & 0x20) != 0;
                                            RegFlagH = false;
                                            RegFlag3 = (RegBC.HighByte & 0x08) != 0;
                                            RegFlagP = TableParity[RegBC.HighByte];
                                            RegFlagN = false;
                                            break;
                                        case 0x41: // OUT C, B
                                            totalExecutedCycles += 12; pendingCycles -= 12;
                                            WriteHardware(RegBC.LowByte, RegBC.HighByte);
                                            break;
                                        case 0x42: // SBC HL, BC
                                            totalExecutedCycles += 15; pendingCycles -= 15;
                                            TI1 = (short)RegHL.LowWord; TI2 = (short)RegBC.LowWord; TIR = TI1 - TI2;
                                            if (RegFlagC) { --TIR; ++TI2; }
                                            TUS = (ushort)TIR;
                                            RegFlagH = ((RegHL.LowWord ^ RegBC.LowWord ^ TUS) & 0x1000) != 0;
                                            RegFlagN = true;
                                            RegFlagC = (((int)RegHL.LowWord - (int)RegBC.LowWord - (RegFlagC ? 1 : 0)) & 0x10000) != 0;
                                            RegFlagP = TIR > 32767 || TIR < -32768;
                                            RegFlagS = TUS > 32767;
                                            RegFlagZ = TUS == 0;
                                            RegHL.LowWord = TUS;
                                            RegFlag3 = (TUS & 0x0800) != 0;
                                            RegFlag5 = (TUS & 0x2000) != 0;                                            
                                            break;
                                        case 0x43: // LD (nn), BC
                                            totalExecutedCycles += 20; pendingCycles -= 20;
                                            TUS = (ushort)(ReadMemory(RegPC.LowWord++) + ReadMemory(RegPC.LowWord++) * 256);
                                            WriteMemory(TUS++, RegBC.LowByte);
                                            WriteMemory(TUS, RegBC.HighByte);
                                            RegWZ.LowWord = TUS;
                                            break;
                                        case 0x44: // NEG
                                            totalExecutedCycles += 8; pendingCycles -= 8;
                                            RegAF.LowWord = TableNeg[RegAF.LowWord];                                            
                                            break;
                                        case 0x45: // RETN
                                            totalExecutedCycles += 14; pendingCycles -= 14;
                                            RegPC.LowByte = ReadMemory(RegSP.LowWord++); RegPC.HighByte = ReadMemory(RegSP.LowWord++);
                                            RegWZ.LowWord = RegPC.LowWord;
                                            IFF1 = IFF2;
                                            break;
                                        case 0x46: // IM $0
                                            totalExecutedCycles += 8; pendingCycles -= 8;
                                            interruptMode = 0;                                            
                                            break;
                                        case 0x47: // LD I, A
                                            totalExecutedCycles += 9; pendingCycles -= 9;
                                            RegI = RegAF.HighByte;                                            
                                            break;
                                        case 0x48: // IN C, C
                                            totalExecutedCycles += 12; pendingCycles -= 12;
                                            RegBC.LowByte = ReadHardware(RegBC.LowWord);
                                            RegFlagS = RegBC.LowByte > 127;
                                            RegFlagZ = RegBC.LowByte == 0;
                                            RegFlag5 = (RegBC.LowByte & 0x20) != 0;
                                            RegFlagH = false;
                                            RegFlag3 = (RegBC.LowByte & 0x08) != 0;
                                            RegFlagP = TableParity[RegBC.LowByte];
                                            RegFlagN = false;
                                            break;
                                        case 0x49: // OUT C, C
                                            totalExecutedCycles += 12; pendingCycles -= 12;
                                            WriteHardware(RegBC.LowByte, RegBC.LowByte);
                                            break;
                                        case 0x4A: // ADC HL, BC
                                            totalExecutedCycles += 15; pendingCycles -= 15;
                                            TI1 = (short)RegHL.LowWord; TI2 = (short)RegBC.LowWord; TIR = TI1 + TI2;
                                            if (RegFlagC) { ++TIR; ++TI2; }
                                            TUS = (ushort)TIR;
                                            RegFlagH = ((TI1 & 0xFFF) + (TI2 & 0xFFF)) > 0xFFF;
                                            RegFlagN = false;
                                            RegFlagC = ((ushort)TI1 + (ushort)TI2) > 0xFFFF;
                                            RegFlagP = TIR > 32767 || TIR < -32768;
                                            RegFlagS = TUS > 32767;
                                            RegFlagZ = TUS == 0;
                                            RegHL.LowWord = TUS;
                                            RegFlag3 = (TUS & 0x0800) != 0;
                                            RegFlag5 = (TUS & 0x2000) != 0;                                            
                                            break;
                                        case 0x4B: // LD BC, (nn)
                                            totalExecutedCycles += 20; pendingCycles -= 20;
                                            TUS = (ushort)(ReadMemory(RegPC.LowWord++) + ReadMemory(RegPC.LowWord++) * 256);
                                            RegBC.LowByte = ReadMemory(TUS++); RegBC.HighByte = ReadMemory(TUS);
                                            RegWZ.LowWord = TUS;
                                            break;
                                        case 0x4C: // NEG
                                            totalExecutedCycles += 8; pendingCycles -= 8;
                                            RegAF.LowWord = TableNeg[RegAF.LowWord];                                            
                                            break;
                                        case 0x4D: // RETI
                                            totalExecutedCycles += 14; pendingCycles -= 14;
                                            RegPC.LowByte = ReadMemory(RegSP.LowWord++); RegPC.HighByte = ReadMemory(RegSP.LowWord++);
                                            break;
                                        case 0x4E: // IM $0
                                            totalExecutedCycles += 8; pendingCycles -= 8;
                                            interruptMode = 0;                                            
                                            break;
                                        case 0x4F: // LD R, A
                                            totalExecutedCycles += 9; pendingCycles -= 9;
                                            RegR = RegAF.HighByte;                                            
                                            break;
                                        case 0x50: // IN D, C
                                            totalExecutedCycles += 12; pendingCycles -= 12;
                                            RegDE.HighByte = ReadHardware(RegBC.LowWord);
                                            RegFlagS = RegDE.HighByte > 127;
                                            RegFlagZ = RegDE.HighByte == 0;
                                            RegFlag5 = (RegDE.HighByte & 0x20) != 0;
                                            RegFlagH = false;
                                            RegFlag3 = (RegDE.HighByte & 0x08) != 0;
                                            RegFlagP = TableParity[RegDE.HighByte];
                                            RegFlagN = false;
                                            break;
                                        case 0x51: // OUT C, D
                                            totalExecutedCycles += 12; pendingCycles -= 12;
                                            WriteHardware(RegBC.LowByte, RegDE.HighByte);
                                            break;
                                        case 0x52: // SBC HL, DE
                                            totalExecutedCycles += 15; pendingCycles -= 15;
                                            TI1 = (short)RegHL.LowWord; TI2 = (short)RegDE.LowWord; TIR = TI1 - TI2;
                                            if (RegFlagC) { --TIR; ++TI2; }
                                            TUS = (ushort)TIR;
                                            RegWZ.LowWord = (ushort)(RegHL.LowWord + 1);
                                            RegFlagH = ((RegHL.LowWord ^ RegDE.LowWord ^ TUS) & 0x1000) != 0;
                                            RegFlagN = true;
                                            RegFlagC = (((int)RegHL.LowWord - (int)RegDE.LowWord - (RegFlagC ? 1 : 0)) & 0x10000) != 0;
                                            RegFlagP = TIR > 32767 || TIR < -32768;
                                            RegFlagS = TUS > 32767;
                                            RegFlagZ = TUS == 0;
                                            RegHL.LowWord = TUS;
                                            RegFlag3 = (TUS & 0x0800) != 0;
                                            RegFlag5 = (TUS & 0x2000) != 0;                                            
                                            break;
                                        case 0x53: // LD (nn), DE
                                            totalExecutedCycles += 20; pendingCycles -= 20;
                                            TUS = (ushort)(ReadMemory(RegPC.LowWord++) + ReadMemory(RegPC.LowWord++) * 256);
                                            WriteMemory(TUS++, RegDE.LowByte);
                                            WriteMemory(TUS, RegDE.HighByte);
                                            RegWZ.LowWord = TUS;
                                            break;
                                        case 0x54: // NEG
                                            totalExecutedCycles += 8; pendingCycles -= 8;
                                            RegAF.LowWord = TableNeg[RegAF.LowWord];                                            
                                            break;
                                        case 0x55: // RETN
                                            totalExecutedCycles += 14; pendingCycles -= 14;
                                            RegPC.LowByte = ReadMemory(RegSP.LowWord++); RegPC.HighByte = ReadMemory(RegSP.LowWord++);
                                            IFF1 = IFF2;
                                            break;
                                        case 0x56: // IM $1
                                            totalExecutedCycles += 8; pendingCycles -= 8;
                                            interruptMode = 1;                                            
                                            break;
                                        case 0x57: // LD A, I
                                            totalExecutedCycles += 9; pendingCycles -= 9;
                                            RegAF.HighByte = RegI;
                                            RegFlagS = RegI > 127;
                                            RegFlagZ = RegI == 0;
                                            RegFlag5 = ((RegI & 0x20) != 0);
                                            RegFlagH = false;
                                            RegFlag3 = ((RegI & 0x08) != 0);
                                            RegFlagN = false;
                                            RegFlagP = IFF2;                                            
                                            break;
                                        case 0x58: // IN E, C
                                            totalExecutedCycles += 12; pendingCycles -= 12;
                                            RegDE.LowByte = ReadHardware(RegBC.LowWord);
                                            RegFlagS = RegDE.LowByte > 127;
                                            RegFlagZ = RegDE.LowByte == 0;
                                            RegFlag5 = (RegDE.LowByte & 0x20) != 0;
                                            RegFlagH = false;
                                            RegFlag3 = (RegDE.LowByte & 0x08) != 0;
                                            RegFlagP = TableParity[RegDE.LowByte];
                                            RegFlagN = false;
                                            break;
                                        case 0x59: // OUT C, E
                                            totalExecutedCycles += 12; pendingCycles -= 12;
                                            WriteHardware(RegBC.LowByte, RegDE.LowByte);
                                            break;
                                        case 0x5A: // ADC HL, DE
                                            totalExecutedCycles += 15; pendingCycles -= 15;
                                            TI1 = (short)RegHL.LowWord; TI2 = (short)RegDE.LowWord; TIR = TI1 + TI2;
                                            if (RegFlagC) { ++TIR; ++TI2; }
                                            TUS = (ushort)TIR;
                                            RegFlagH = ((TI1 & 0xFFF) + (TI2 & 0xFFF)) > 0xFFF;
                                            RegFlagN = false;
                                            RegFlagC = ((ushort)TI1 + (ushort)TI2) > 0xFFFF;
                                            RegFlagP = TIR > 32767 || TIR < -32768;
                                            RegFlagS = TUS > 32767;
                                            RegFlagZ = TUS == 0;
                                            RegHL.LowWord = TUS;
                                            RegFlag3 = (TUS & 0x0800) != 0;
                                            RegFlag5 = (TUS & 0x2000) != 0;                                            
                                            break;
                                        case 0x5B: // LD DE, (nn)
                                            totalExecutedCycles += 20; pendingCycles -= 20;
                                            TUS = (ushort)(ReadMemory(RegPC.LowWord++) + ReadMemory(RegPC.LowWord++) * 256);
                                            RegDE.LowByte = ReadMemory(TUS++); RegDE.HighByte = ReadMemory(TUS);
                                            break;
                                        case 0x5C: // NEG
                                            totalExecutedCycles += 8; pendingCycles -= 8;
                                            RegAF.LowWord = TableNeg[RegAF.LowWord];                                            
                                            break;
                                        case 0x5D: // RETI
                                            totalExecutedCycles += 14; pendingCycles -= 14;
                                            RegPC.LowByte = ReadMemory(RegSP.LowWord++); RegPC.HighByte = ReadMemory(RegSP.LowWord++);
                                            break;
                                        case 0x5E: // IM $2
                                            totalExecutedCycles += 8; pendingCycles -= 8;
                                            interruptMode = 2;                                            
                                            break;
                                        case 0x5F: // LD A, R
                                            totalExecutedCycles += 9; pendingCycles -= 9;
                                            RegAF.HighByte = (byte)((RegR & 0x7F) | RegR2);
                                            RegFlagS = (RegR2 == 0x80);
                                            RegFlagZ = (byte)((RegR & 0x7F) | RegR2) == 0;
                                            RegFlagH = false;
                                            RegFlag5 = ((RegR & 0x20) != 0);
                                            RegFlagN = false;
                                            RegFlag3 = ((RegR & 0x08) != 0);
                                            RegFlagP = IFF2;                                            
                                            break;
                                        case 0x60: // IN H, C
                                            totalExecutedCycles += 12; pendingCycles -= 12;
                                            RegHL.HighByte = ReadHardware(RegBC.LowWord);
                                            RegFlagS = RegHL.HighByte > 127;
                                            RegFlagZ = RegHL.HighByte == 0;
                                            RegFlagH = false;
                                            RegFlagP = TableParity[RegHL.HighByte];
                                            RegFlagN = false;
                                            RegFlag3 = (RegHL.HighByte & 0x08) != 0;
                                            RegFlag5 = (RegHL.HighByte & 0x20) != 0;
                                            break;
                                        case 0x61: // OUT C, H
                                            totalExecutedCycles += 12; pendingCycles -= 12;
                                            WriteHardware(RegBC.LowByte, RegHL.HighByte);
                                            break;
                                        case 0x62: // SBC HL, HL
                                            totalExecutedCycles += 15; pendingCycles -= 15;
                                            TI1 = (short)RegHL.LowWord; TI2 = (short)RegHL.LowWord; TIR = TI1 - TI2;
                                            if (RegFlagC) { --TIR; ++TI2; }
                                            TUS = (ushort)TIR;
                                            RegFlagH = ((RegHL.LowWord ^ RegHL.LowWord ^ TUS) & 0x1000) != 0;
                                            RegFlagN = true;
                                            RegFlagC = (((int)RegHL.LowWord - (int)RegHL.LowWord - (RegFlagC ? 1 : 0)) & 0x10000) != 0;
                                            RegFlagP = TIR > 32767 || TIR < -32768;
                                            RegFlagS = TUS > 32767;
                                            RegFlagZ = TUS == 0;
                                            RegHL.LowWord = TUS;
                                            RegFlag3 = (TUS & 0x0800) != 0;
                                            RegFlag5 = (TUS & 0x2000) != 0;                                            
                                            break;
                                        case 0x63: // LD (nn), HL
                                            totalExecutedCycles += 16; pendingCycles -= 16;
                                            TUS = (ushort)(ReadMemory(RegPC.LowWord++) + ReadMemory(RegPC.LowWord++) * 256);
                                            WriteMemory(TUS++, RegHL.LowByte);
                                            WriteMemory(TUS, RegHL.HighByte);
                                            break;
                                        case 0x64: // NEG
                                            totalExecutedCycles += 8; pendingCycles -= 8;
                                            RegAF.LowWord = TableNeg[RegAF.LowWord];                                            
                                            break;
                                        case 0x65: // RETN
                                            totalExecutedCycles += 14; pendingCycles -= 14;
                                            RegPC.LowByte = ReadMemory(RegSP.LowWord++); RegPC.HighByte = ReadMemory(RegSP.LowWord++);
                                            IFF1 = IFF2;
                                            break;
                                        case 0x66: // IM $0
                                            totalExecutedCycles += 8; pendingCycles -= 8;
                                            interruptMode = 0;                                            
                                            break;
                                        case 0x67: // RRD
                                            totalExecutedCycles += 18; pendingCycles -= 18;
                                            TB1 = RegAF.HighByte; TB2 = ReadMemory(RegHL.LowWord);
                                            WriteMemory(RegHL.LowWord, (byte)((TB2 >> 4) + (TB1 << 4)));
                                            RegAF.HighByte = (byte)((TB1 & 0xF0) + (TB2 & 0x0F));
                                            RegFlagS = RegAF.HighByte > 127;
                                            RegFlagZ = RegAF.HighByte == 0;
                                            RegFlagH = false;
                                            RegFlagP = TableParity[RegAF.HighByte];
                                            RegFlagN = false;
                                            RegFlag3 = (RegAF.HighByte & 0x08) != 0;
                                            RegFlag5 = (RegAF.HighByte & 0x20) != 0;
                                            break;
                                        case 0x68: // IN L, C
                                            totalExecutedCycles += 12; pendingCycles -= 12;
                                            RegHL.LowByte = ReadHardware(RegBC.LowWord);
                                            RegFlagS = RegHL.LowByte > 127;
                                            RegFlagZ = RegHL.LowByte == 0;
                                            RegFlagH = false;
                                            RegFlagP = TableParity[RegHL.LowByte];
                                            RegFlagN = false;
                                            RegFlag3 = (RegHL.LowByte & 0x08) != 0;
                                            RegFlag5 = (RegHL.LowByte & 0x20) != 0;
                                            break;
                                        case 0x69: // OUT C, L
                                            totalExecutedCycles += 12; pendingCycles -= 12;
                                            WriteHardware(RegBC.LowByte, RegHL.LowByte);
                                            break;
                                        case 0x6A: // ADC HL, HL
                                            totalExecutedCycles += 15; pendingCycles -= 15;
                                            TI1 = (short)RegHL.LowWord; TI2 = (short)RegHL.LowWord; TIR = TI1 + TI2;
                                            if (RegFlagC) { ++TIR; ++TI2; }
                                            TUS = (ushort)TIR;
                                            RegFlagH = ((TI1 & 0xFFF) + (TI2 & 0xFFF)) > 0xFFF;
                                            RegFlagN = false;
                                            RegFlagC = ((ushort)TI1 + (ushort)TI2) > 0xFFFF;
                                            RegFlagP = TIR > 32767 || TIR < -32768;
                                            RegFlagS = TUS > 32767;
                                            RegFlagZ = TUS == 0;
                                            RegHL.LowWord = TUS;
                                            RegFlag3 = (TUS & 0x0800) != 0;
                                            RegFlag5 = (TUS & 0x2000) != 0;                                            
                                            break;
                                        case 0x6B: // LD HL, (nn)
                                            totalExecutedCycles += 16; pendingCycles -= 16;
                                            TUS = (ushort)(ReadMemory(RegPC.LowWord++) + ReadMemory(RegPC.LowWord++) * 256);
                                            RegHL.LowByte = ReadMemory(TUS++); RegHL.HighByte = ReadMemory(TUS);
                                            break;
                                        case 0x6C: // NEG
                                            totalExecutedCycles += 8; pendingCycles -= 8;
                                            RegAF.LowWord = TableNeg[RegAF.LowWord];                                            
                                            break;
                                        case 0x6D: // RETI
                                            totalExecutedCycles += 14; pendingCycles -= 14;
                                            RegPC.LowByte = ReadMemory(RegSP.LowWord++); RegPC.HighByte = ReadMemory(RegSP.LowWord++);
                                            break;
                                        case 0x6E: // IM $0
                                            totalExecutedCycles += 8; pendingCycles -= 8;
                                            interruptMode = 0;                                            
                                            break;
                                        case 0x6F: // RLD
                                            totalExecutedCycles += 18; pendingCycles -= 18;
                                            TB1 = RegAF.HighByte; TB2 = ReadMemory(RegHL.LowWord);
                                            WriteMemory(RegHL.LowWord, (byte)((TB1 & 0x0F) + (TB2 << 4)));
                                            RegAF.HighByte = (byte)((TB1 & 0xF0) + (TB2 >> 4));
                                            RegFlagS = RegAF.HighByte > 127;
                                            RegFlagZ = RegAF.HighByte == 0;
                                            RegFlagH = false;
                                            RegFlagP = TableParity[RegAF.HighByte];
                                            RegFlagN = false;
                                            RegFlag3 = (RegAF.HighByte & 0x08) != 0;
                                            RegFlag5 = (RegAF.HighByte & 0x20) != 0;
                                            break;
                                        case 0x70: // IN 0, C
                                            totalExecutedCycles += 12; pendingCycles -= 12;
                                            TB = ReadHardware(RegBC.LowWord);
                                            RegFlagS = TB > 127;
                                            RegFlagZ = TB == 0;
                                            RegFlagH = false;
                                            RegFlagP = TableParity[TB];
                                            RegFlagN = false;
                                            RegFlag3 = (TB & 0x08) != 0;
                                            RegFlag5 = (TB & 0x20) != 0;
                                            break;
                                        case 0x71: // OUT C, 0
                                            totalExecutedCycles += 12; pendingCycles -= 12;
                                            WriteHardware(RegBC.LowByte, 0);
                                            break;
                                        case 0x72: // SBC HL, SP
                                            totalExecutedCycles += 15; pendingCycles -= 15;
                                            TI1 = (short)RegHL.LowWord; TI2 = (short)RegSP.LowWord; TIR = TI1 - TI2;
                                            if (RegFlagC) { --TIR; ++TI2; }
                                            TUS = (ushort)TIR;
                                            RegWZ.LowWord = (ushort)(RegHL.LowWord + 1);
                                            RegFlagH = ((RegHL.LowWord ^ RegSP.LowWord ^ TUS) & 0x1000) != 0;
                                            RegFlagN = true;
                                            RegFlagC = (((int)RegHL.LowWord - (int)RegSP.LowWord - (RegFlagC ? 1 : 0)) & 0x10000) != 0;
                                            RegFlagP = TIR > 32767 || TIR < -32768;
                                            RegFlagS = TUS > 32767;
                                            RegFlagZ = TUS == 0;
                                            RegHL.LowWord = TUS;
                                            RegFlag3 = (TUS & 0x0800) != 0;
                                            RegFlag5 = (TUS & 0x2000) != 0;                                            
                                            break;
                                        case 0x73: // LD (nn), SP
                                            totalExecutedCycles += 20; pendingCycles -= 20;
                                            TUS = (ushort)(ReadMemory(RegPC.LowWord++) + ReadMemory(RegPC.LowWord++) * 256);
                                            WriteMemory(TUS++, RegSP.LowByte);
                                            WriteMemory(TUS, RegSP.HighByte);
                                            RegWZ.LowWord = TUS;
                                            break;
                                        case 0x74: // NEG
                                            totalExecutedCycles += 8; pendingCycles -= 8;
                                            RegAF.LowWord = TableNeg[RegAF.LowWord];                                            
                                            break;
                                        case 0x75: // RETN
                                            totalExecutedCycles += 14; pendingCycles -= 14;
                                            RegPC.LowByte = ReadMemory(RegSP.LowWord++); RegPC.HighByte = ReadMemory(RegSP.LowWord++);
                                            RegWZ.LowWord = RegPC.LowWord;
                                            IFF1 = IFF2;
                                            break;
                                        case 0x76: // IM $1
                                            totalExecutedCycles += 8; pendingCycles -= 8;
                                            interruptMode = 1;                                            
                                            break;
                                        case 0x77: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x78: // IN A, C
                                            totalExecutedCycles += 12; pendingCycles -= 12;
                                            RegAF.HighByte = ReadHardware(RegBC.LowWord);
                                            RegFlagS = RegAF.HighByte > 127;
                                            RegFlagZ = RegAF.HighByte == 0;
                                            RegFlagH = false;
                                            RegFlagP = TableParity[RegAF.HighByte];
                                            RegFlagN = false;
                                            RegFlag3 = (RegAF.HighByte & 0x08) != 0;
                                            RegFlag5 = (RegAF.HighByte & 0x20) != 0;
                                            RegWZ.LowWord = (ushort)(RegBC.LowWord + 1);
                                            break;
                                        case 0x79: // OUT C, A
                                            totalExecutedCycles += 12; pendingCycles -= 12;
                                            WriteHardware(RegBC.LowByte, RegAF.HighByte);
                                            RegWZ.LowWord = (ushort)(RegBC.LowWord + 1);
                                            break;
                                        case 0x7A: // ADC HL, SP
                                            totalExecutedCycles += 15; pendingCycles -= 15;
                                            TI1 = (short)RegHL.LowWord; TI2 = (short)RegSP.LowWord; TIR = TI1 + TI2;
                                            if (RegFlagC) { ++TIR; ++TI2; }
                                            TUS = (ushort)TIR;
                                            RegWZ.LowWord = (ushort)(RegHL.LowWord + 1);
                                            RegFlagH = ((TI1 & 0xFFF) + (TI2 & 0xFFF)) > 0xFFF;
                                            RegFlagN = false;
                                            RegFlagC = ((ushort)TI1 + (ushort)TI2) > 0xFFFF;
                                            RegFlagP = TIR > 32767 || TIR < -32768;
                                            RegFlagS = TUS > 32767;
                                            RegFlagZ = TUS == 0;
                                            RegHL.LowWord = TUS;
                                            RegFlag3 = (TUS & 0x0800) != 0;
                                            RegFlag5 = (TUS & 0x2000) != 0;                                            
                                            break;
                                        case 0x7B: // LD SP, (nn)
                                            totalExecutedCycles += 20; pendingCycles -= 20;
                                            TUS = (ushort)(ReadMemory(RegPC.LowWord++) + ReadMemory(RegPC.LowWord++) * 256);
                                            RegSP.LowByte = ReadMemory(TUS++); RegSP.HighByte = ReadMemory(TUS);
                                            RegWZ.LowWord = TUS;
                                            break;
                                        case 0x7C: // NEG
                                            totalExecutedCycles += 8; pendingCycles -= 8;
                                            RegAF.LowWord = TableNeg[RegAF.LowWord];                                            
                                            break;
                                        case 0x7D: // RETI
                                            totalExecutedCycles += 14; pendingCycles -= 14;
                                            RegPC.LowByte = ReadMemory(RegSP.LowWord++); RegPC.HighByte = ReadMemory(RegSP.LowWord++);
                                            RegWZ.LowWord = RegPC.LowWord;
                                            break;
                                        case 0x7E: // IM $2
                                            totalExecutedCycles += 8; pendingCycles -= 8;
                                            interruptMode = 2;                                            
                                            break;
                                        case 0x7F: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x80: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x81: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x82: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x83: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x84: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x85: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x86: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x87: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x88: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x89: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x8A: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x8B: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x8C: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x8D: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x8E: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x8F: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x90: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x91: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x92: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x93: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x94: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x95: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x96: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x97: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x98: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x99: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x9A: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x9B: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x9C: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x9D: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x9E: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0x9F: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xA0: // LDI
                                            totalExecutedCycles += 16; pendingCycles -= 16;
                                            WriteMemory(RegDE.LowWord++, TB1 = ReadMemory(RegHL.LowWord++));
                                            TB1 += RegAF.HighByte; RegFlag5 = (TB1 & 0x02) != 0; RegFlag3 = (TB1 & 0x08) != 0;
                                            --RegBC.LowWord;
                                            RegFlagP = RegBC.LowWord != 0;
                                            RegFlagH = false;
                                            RegFlagN = false;
                                            break;
                                        case 0xA1: // CPI
                                            totalExecutedCycles += 16; pendingCycles -= 16;
                                            TB1 = ReadMemory(RegHL.LowWord++); TB2 = (byte)(RegAF.HighByte - TB1);
                                            RegFlagN = true;
                                            RegFlagH = TableHalfBorrow[RegAF.HighByte, TB1];
                                            RegFlagZ = TB2 == 0;
                                            RegFlagS = TB2 > 127;
                                            TB1 = (byte)(RegAF.HighByte - TB1 - (RegFlagH ? 1 : 0)); RegFlag5 = (TB1 & 0x02) != 0; RegFlag3 = (TB1 & 0x08) != 0;
                                            --RegBC.LowWord;
                                            RegFlagP = RegBC.LowWord != 0;
                                            break;
                                        case 0xA2: // INI
                                            totalExecutedCycles += 16; pendingCycles -= 16;
                                            TB = ReadHardware(RegBC.LowWord);
                                            RegWZ.LowWord = (ushort)(RegBC.LowWord + 1);
                                            --RegBC.HighByte;
                                            WriteMemory(RegHL.LowWord++, TB);
                                            RegFlagZ = RegBC.HighByte == 0;
                                            TUS = (ushort)(((RegBC.LowByte + 1) & 0xff) + TB);
                                            if ((TB & 0x80) != 0)
                                            {
                                                RegFlagN = true;
                                            }
                                            if ((TUS & 0x100) != 0)
                                            {
                                                RegFlagH = RegFlagC = true;
                                            }
                                            RegFlagP = TableParity[(TUS & 0x07) ^ RegBC.HighByte];
                                            break;
                                        case 0xA3: // OUTI
                                            totalExecutedCycles += 16; pendingCycles -= 16;
                                            TB = ReadMemory(RegHL.LowWord++);
                                            --RegBC.HighByte;
                                            RegWZ.LowWord = (ushort)(RegBC.LowWord + 1);
                                            WriteHardware(RegBC.LowWord, TB);
                                            RegFlagC = false;
                                            RegFlagN = false;
                                            RegFlag3 = IsX(RegBC.HighByte);
                                            RegFlagH = false;
                                            RegFlag5 = IsY(RegBC.HighByte);
                                            RegFlagZ = RegBC.HighByte == 0;
                                            RegFlagS = IsS(RegBC.HighByte);
                                            TUS = (ushort)(RegHL.LowByte + TB);
                                            if (IsS(TB))
                                            {
                                                RegFlagN = true;
                                            }
                                            if ((TUS & 0x100) != 0)
                                            {
                                                RegFlagC = true;
                                                RegFlagH = true;
                                            }
                                            RegFlagP = TableParity[(TUS & 0x07) ^ RegBC.HighByte];
                                            break;
                                        case 0xA4: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xA5: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xA6: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xA7: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xA8: // LDD
                                            totalExecutedCycles += 16; pendingCycles -= 16;
                                            WriteMemory(RegDE.LowWord--, TB1 = ReadMemory(RegHL.LowWord--));
                                            TB1 += RegAF.HighByte; RegFlag5 = (TB1 & 0x02) != 0; RegFlag3 = (TB1 & 0x08) != 0;
                                            --RegBC.LowWord;
                                            RegFlagP = RegBC.LowWord != 0;
                                            RegFlagH = false;
                                            RegFlagN = false;
                                            break;
                                        case 0xA9: // CPD
                                            totalExecutedCycles += 16; pendingCycles -= 16;
                                            TB1 = ReadMemory(RegHL.LowWord--); TB2 = (byte)(RegAF.HighByte - TB1);
                                            RegFlagN = true;
                                            RegFlagH = TableHalfBorrow[RegAF.HighByte, TB1];
                                            RegFlagZ = TB2 == 0;
                                            RegFlagS = TB2 > 127;
                                            TB1 = (byte)(RegAF.HighByte - TB1 - (RegFlagH ? 1 : 0)); RegFlag5 = (TB1 & 0x02) != 0; RegFlag3 = (TB1 & 0x08) != 0;
                                            --RegBC.LowWord;
                                            RegFlagP = RegBC.LowWord != 0;
                                            break;
                                        case 0xAA: // IND
                                            totalExecutedCycles += 16; pendingCycles -= 16;
                                            TB = ReadHardware(RegBC.LowWord);
                                            RegWZ.LowWord = (ushort)(RegBC.LowWord - 1);
                                            --RegBC.HighByte;
                                            WriteMemory(RegHL.LowWord--, TB);
                                            RegFlagZ = RegBC.HighByte == 0;
                                            TUS = (ushort)(((RegBC.LowByte - 1) & 0xff) + TB);
                                            if ((TB & 0x80) != 0)
                                            {
                                                RegFlagN = true;
                                            }
                                            if ((TUS & 0x100) != 0)
                                            {
                                                RegFlagH = RegFlagC = true;
                                            }
                                            RegFlagP = TableParity[(TUS & 0x07) ^ RegBC.HighByte];
                                            break;
                                        case 0xAB: // OUTD
                                            totalExecutedCycles += 16; pendingCycles -= 16;
                                            TB = ReadMemory(RegHL.LowWord--);
                                            WriteHardware(RegBC.LowWord, TB);
                                            --RegBC.HighByte;
                                            RegWZ.LowWord = (ushort)(RegBC.LowWord - 1);
                                            TUS = (ushort)(RegHL.LowByte + TB);
                                            RegFlagZ = RegBC.HighByte == 0;
                                            if ((TB & 0x80) != 0)
                                            {
                                                RegFlagN = true;
                                            }
                                            if ((TUS & 0x100) != 0)
                                            {
                                                RegFlagH = RegFlagC = true;
                                            }
                                            RegFlagP = TableParity[(TUS & 0x07) ^ RegBC.HighByte];
                                            break;
                                        case 0xAC: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xAD: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xAE: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xAF: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xB0: // LDIR
                                            totalExecutedCycles += 16; pendingCycles -= 16;
                                            WriteMemory(RegDE.LowWord++, TB1 = ReadMemory(RegHL.LowWord++));
                                            TB1 += RegAF.HighByte; RegFlag5 = (TB1 & 0x02) != 0; RegFlag3 = (TB1 & 0x08) != 0;
                                            --RegBC.LowWord;
                                            RegFlagP = RegBC.LowWord != 0;
                                            RegFlagH = false;
                                            RegFlagN = false;
                                            if (RegBC.LowWord != 0)
                                            {
                                                RegPC.LowWord -= 2;
                                                RegWZ.LowWord = (ushort)(RegPC.LowWord + 1);
                                                totalExecutedCycles += 5; pendingCycles -= 5;
                                            }
                                            break;
                                        case 0xB1: // CPIR
                                            totalExecutedCycles += 16; pendingCycles -= 16;
                                            TB1 = ReadMemory(RegHL.LowWord++); TB2 = (byte)(RegAF.HighByte - TB1);
                                            RegWZ.LowWord++;
                                            RegFlagN = true;
                                            RegFlagH = TableHalfBorrow[RegAF.HighByte, TB1];
                                            RegFlagZ = TB2 == 0;
                                            RegFlagS = TB2 > 127;
                                            TB1 = (byte)(RegAF.HighByte - TB1 - (RegFlagH ? 1 : 0)); RegFlag5 = (TB1 & 0x02) != 0; RegFlag3 = (TB1 & 0x08) != 0;
                                            --RegBC.LowWord;
                                            RegFlagP = RegBC.LowWord != 0;
                                            if (RegBC.LowWord != 0 && !RegFlagZ)
                                            {
                                                RegPC.LowWord -= 2;
                                                RegWZ.LowWord = (ushort)(RegPC.LowWord + 1);
                                                totalExecutedCycles += 5; pendingCycles -= 5;
                                            }
                                            break;
                                        case 0xB2: // INIR
                                            totalExecutedCycles += 16; pendingCycles -= 16;
                                            TB = ReadHardware(RegBC.LowWord);
                                            RegWZ.LowWord = (ushort)(RegBC.LowWord + 1);
                                            --RegBC.HighByte;
                                            WriteMemory(RegHL.LowWord++, TB);
                                            RegFlagZ = RegBC.HighByte == 0;
                                            TUS = (ushort)(((RegBC.LowByte + 1) & 0xff) + TB);
                                            if ((TB & 0x80) != 0)
                                            {
                                                RegFlagN = true;
                                            }
                                            if ((TUS & 0x100) != 0)
                                            {
                                                RegFlagH = RegFlagC = true;
                                            }
                                            RegFlagP = TableParity[(TUS & 0x07) ^ RegBC.HighByte];
                                            if (RegBC.HighByte != 0)
                                            {
                                                RegPC.LowWord -= 2;
                                                totalExecutedCycles += 5; pendingCycles -= 5;
                                            }
                                            break;
                                        case 0xB3: // OTIR
                                            totalExecutedCycles += 16; pendingCycles -= 16;
                                            TB = ReadMemory(RegHL.LowWord++);
                                            --RegBC.HighByte;
                                            RegWZ.LowWord = (ushort)(RegBC.LowWord + 1);
                                            WriteHardware(RegBC.LowWord, TB);
                                            RegFlagC = false;
                                            RegFlagN = false;
                                            RegFlag3 = IsX(RegBC.HighByte);
                                            RegFlagH = false;
                                            RegFlag5 = IsY(RegBC.HighByte);
                                            RegFlagZ = RegBC.HighByte == 0;
                                            RegFlagS = IsS(RegBC.HighByte);
                                            TUS = (ushort)(RegHL.LowByte + TB);
                                            if (IsS(TB))
                                            {
                                                RegFlagN = true;
                                            }
                                            if ((TUS & 0x100) != 0)
                                            {
                                                RegFlagC = true;
                                                RegFlagH = true;
                                            }
                                            RegFlagP = TableParity[(TUS & 0x07) ^ RegBC.HighByte];
                                            if (RegBC.HighByte != 0)
                                            {
                                                RegPC.LowWord -= 2;
                                                totalExecutedCycles += 5; pendingCycles -= 5;
                                            }
                                            break;
                                        case 0xB4: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xB5: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xB6: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xB7: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xB8: // LDDR
                                            totalExecutedCycles += 16; pendingCycles -= 16;
                                            WriteMemory(RegDE.LowWord--, TB1 = ReadMemory(RegHL.LowWord--));
                                            TB1 += RegAF.HighByte; RegFlag5 = (TB1 & 0x02) != 0; RegFlag3 = (TB1 & 0x08) != 0;
                                            --RegBC.LowWord;
                                            RegFlagP = RegBC.LowWord != 0;
                                            RegFlagH = false;
                                            RegFlagN = false;
                                            if (RegBC.LowWord != 0)
                                            {
                                                RegPC.LowWord -= 2;
                                                RegWZ.LowWord = (ushort)(RegPC.LowWord + 1);
                                                totalExecutedCycles += 5; pendingCycles -= 5;
                                            }
                                            break;
                                        case 0xB9: // CPDR
                                            totalExecutedCycles += 16; pendingCycles -= 16;
                                            TB1 = ReadMemory(RegHL.LowWord--); TB2 = (byte)(RegAF.HighByte - TB1);
                                            RegWZ.LowWord--;
                                            RegFlagN = true;
                                            RegFlagH = TableHalfBorrow[RegAF.HighByte, TB1];
                                            RegFlagZ = TB2 == 0;
                                            RegFlagS = TB2 > 127;
                                            TB1 = (byte)(RegAF.HighByte - TB1 - (RegFlagH ? 1 : 0)); RegFlag5 = (TB1 & 0x02) != 0; RegFlag3 = (TB1 & 0x08) != 0;
                                            --RegBC.LowWord;
                                            RegFlagP = RegBC.LowWord != 0;
                                            if (RegBC.LowWord != 0 && !RegFlagZ)
                                            {
                                                RegPC.LowWord -= 2;
                                                RegWZ.LowWord = (ushort)(RegPC.LowWord + 1);
                                                totalExecutedCycles += 5; pendingCycles -= 5;
                                            }
                                            break;
                                        case 0xBA: // INDR
                                            totalExecutedCycles += 16; pendingCycles -= 16;
                                            TB = ReadHardware(RegBC.LowWord);
                                            RegWZ.LowWord = (ushort)(RegBC.LowWord - 1);
                                            --RegBC.HighByte;
                                            WriteMemory(RegHL.LowWord--, TB);
                                            RegFlagZ = RegBC.HighByte == 0;
                                            TUS = (ushort)(((RegBC.LowByte - 1) & 0xff) + TB);
                                            if ((TB & 0x80) != 0)
                                            {
                                                RegFlagN = true;
                                            }
                                            if ((TUS & 0x100) != 0)
                                            {
                                                RegFlagH = RegFlagC = true;
                                            }
                                            RegFlagP = TableParity[(TUS & 0x07) ^ RegBC.HighByte];
                                            if (RegBC.HighByte != 0)
                                            {
                                                RegPC.LowWord -= 2;
                                                totalExecutedCycles += 5; pendingCycles -= 5;
                                            }
                                            break;
                                        case 0xBB: // OTDR
                                            totalExecutedCycles += 16; pendingCycles -= 16;
                                            TB = ReadMemory(RegHL.LowWord--);
                                            WriteHardware(RegBC.LowWord, TB);
                                            --RegBC.HighByte;
                                            RegWZ.LowWord = (ushort)(RegBC.LowWord - 1);
                                            TUS = (ushort)(RegHL.LowByte + TB);
                                            RegFlagZ = RegBC.HighByte == 0;
                                            if ((TB & 0x80) != 0)
                                            {
                                                RegFlagN = true;
                                            }
                                            if ((TUS & 0x100) != 0)
                                            {
                                                RegFlagH = RegFlagC = true;
                                            }
                                            RegFlagP = TableParity[(TUS & 0x07) ^ RegBC.HighByte];
                                            if (RegBC.HighByte != 0)
                                            {
                                                RegPC.LowWord -= 2;
                                                totalExecutedCycles += 5; pendingCycles -= 5;
                                            }
                                            break;
                                        case 0xBC: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xBD: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xBE: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xBF: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xC0: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xC1: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xC2: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xC3: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xC4: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xC5: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xC6: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xC7: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xC8: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xC9: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xCA: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xCB: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xCC: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xCD: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xCE: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xCF: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xD0: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xD1: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xD2: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xD3: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xD4: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xD5: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xD6: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xD7: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xD8: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xD9: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xDA: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xDB: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xDC: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xDD: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xDE: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xDF: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xE0: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xE1: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xE2: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xE3: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xE4: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xE5: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xE6: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xE7: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xE8: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xE9: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xEA: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xEB: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xEC: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xED: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xEE: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xEF: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xF0: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xF1: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xF2: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xF3: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xF4: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xF5: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xF6: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xF7: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xF8: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xF9: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xFA: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xFB: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xFC: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xFD: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xFE: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                        case 0xFF: // NOP
                                            totalExecutedCycles += 4; pendingCycles -= 4;
                                            break;
                                    }
                                    break;
                                case 0xEE: // XOR n
                                    totalExecutedCycles += 7; pendingCycles -= 7;
                                    RegAF.LowWord = TableALU[5, RegAF.HighByte, ReadMemory(RegPC.LowWord++), 0];
                                    break;
                                case 0xEF: // RST $28
                                    totalExecutedCycles += 11; pendingCycles -= 11;
                                    WriteMemory(--RegSP.LowWord, RegPC.HighByte); WriteMemory(--RegSP.LowWord, RegPC.LowByte);
                                    RegPC.LowWord = 0x28;
                                    break;
                                case 0xF0: // RET P
                                    totalExecutedCycles += 5; pendingCycles -= 5;
                                    if (!RegFlagS)
                                    {
                                        RegPC.LowByte = ReadMemory(RegSP.LowWord++); RegPC.HighByte = ReadMemory(RegSP.LowWord++);
                                        totalExecutedCycles += 6; pendingCycles -= 6;
                                    }
                                    break;
                                case 0xF1: // POP AF
                                    totalExecutedCycles += 10; pendingCycles -= 10;
                                    RegAF.LowByte = ReadMemory(RegSP.LowWord++); RegAF.HighByte = ReadMemory(RegSP.LowWord++);
                                    break;
                                case 0xF2: // JP P, nn
                                    totalExecutedCycles += 10; pendingCycles -= 10;
                                    TUS = (ushort)(ReadMemory(RegPC.LowWord++) + ReadMemory(RegPC.LowWord++) * 256);
                                    if (!RegFlagS)
                                    {
                                        RegPC.LowWord = TUS;
                                    }
                                    break;
                                case 0xF3: // DI
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    IFF1 = IFF2 = false;                                    
                                    break;
                                case 0xF4: // CALL P, nn
                                    totalExecutedCycles += 10; pendingCycles -= 10;
                                    TUS = (ushort)(ReadMemory(RegPC.LowWord++) + ReadMemory(RegPC.LowWord++) * 256);
                                    if (!RegFlagS)
                                    {
                                        totalExecutedCycles += 7; pendingCycles -= 7;
                                        WriteMemory(--RegSP.LowWord, RegPC.HighByte); WriteMemory(--RegSP.LowWord, RegPC.LowByte);
                                        RegPC.LowWord = TUS;
                                    }
                                    break;
                                case 0xF5: // PUSH AF
                                    totalExecutedCycles += 11; pendingCycles -= 11;
                                    WriteMemory(--RegSP.LowWord, RegAF.HighByte); WriteMemory(--RegSP.LowWord, RegAF.LowByte);
                                    break;
                                case 0xF6: // OR n
                                    totalExecutedCycles += 7; pendingCycles -= 7;
                                    RegAF.LowWord = TableALU[6, RegAF.HighByte, ReadMemory(RegPC.LowWord++), 0];
                                    break;
                                case 0xF7: // RST $30
                                    totalExecutedCycles += 11; pendingCycles -= 11;
                                    WriteMemory(--RegSP.LowWord, RegPC.HighByte); WriteMemory(--RegSP.LowWord, RegPC.LowByte);
                                    RegPC.LowWord = 0x30;
                                    break;
                                case 0xF8: // RET M
                                    totalExecutedCycles += 5; pendingCycles -= 5;
                                    if (RegFlagS)
                                    {
                                        RegPC.LowByte = ReadMemory(RegSP.LowWord++); RegPC.HighByte = ReadMemory(RegSP.LowWord++);
                                        totalExecutedCycles += 6; pendingCycles -= 6;
                                    }
                                    break;
                                case 0xF9: // LD SP, IY
                                    totalExecutedCycles += 10; pendingCycles -= 10;
                                    RegSP.LowWord = RegIY.LowWord;                                    
                                    break;
                                case 0xFA: // JP M, nn
                                    totalExecutedCycles += 10; pendingCycles -= 10;
                                    TUS = (ushort)(ReadMemory(RegPC.LowWord++) + ReadMemory(RegPC.LowWord++) * 256);
                                    if (RegFlagS)
                                    {
                                        RegPC.LowWord = TUS;
                                    }
                                    break;
                                case 0xFB: // EI
                                    totalExecutedCycles += 4; pendingCycles -= 4;
                                    IFF1 = IFF2 = true;
                                    Interruptable = false;                                    
                                    break;
                                case 0xFC: // CALL M, nn
                                    totalExecutedCycles += 10; pendingCycles -= 10;
                                    TUS = (ushort)(ReadMemory(RegPC.LowWord++) + ReadMemory(RegPC.LowWord++) * 256);
                                    if (RegFlagS)
                                    {
                                        totalExecutedCycles += 7; pendingCycles -= 7;
                                        WriteMemory(--RegSP.LowWord, RegPC.HighByte); WriteMemory(--RegSP.LowWord, RegPC.LowByte);
                                        RegPC.LowWord = TUS;
                                    }
                                    break;
                                case 0xFD: // <-
                                    // Invalid sequence.
                                    totalExecutedCycles += 1337; pendingCycles -= 1337;
                                    break;
                                case 0xFE: // CP n
                                    totalExecutedCycles += 7; pendingCycles -= 7;
                                    RegAF.LowWord = TableALU[7, RegAF.HighByte, ReadMemory(RegPC.LowWord++), 0];
                                    break;
                                case 0xFF: // RST $38
                                    totalExecutedCycles += 11; pendingCycles -= 11;
                                    WriteMemory(--RegSP.LowWord, RegPC.HighByte); WriteMemory(--RegSP.LowWord, RegPC.LowByte);
                                    RegPC.LowWord = 0x38;
                                    break;
                            }
                            break;
                        case 0xFE: // CP n
                            RegAF.LowWord = TableALU[7, RegAF.HighByte, ReadOpArg(RegPC.LowWord++), 0];
                            totalExecutedCycles += 7; pendingCycles -= 7;
                            break;
                        case 0xFF: // RST $38
                            WriteMemory(--RegSP.LowWord, RegPC.HighByte); WriteMemory(--RegSP.LowWord, RegPC.LowByte);
                            RegPC.LowWord = 0x38;
                            totalExecutedCycles += 11; pendingCycles -= 11;
                            break;
                    }
                    debugger_stop_cpu_hook_callback();
                }
            }
            while (pendingCycles > 0);
            return cycles - pendingCycles;
        }
    }
}
