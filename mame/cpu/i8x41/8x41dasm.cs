using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cpu.i8x41
{
    public static class I8X41Disassembler
    {
        public static int Disassemble(ushort pc, byte[] oprom, byte[] opram, out string instruction)
        {
            instruction = "";
            uint flags = 0;
            int PC = pc;
            byte op = oprom[PC - pc];
            PC++;

            switch (op)
            {
                case 0x00:
                    instruction = "nop";
                    break;
                case 0x02:
                    instruction = "out   dbb,a";
                    break;
                case 0x03:
                    instruction = "add   a,#$"+opram[PC - pc].ToString("X2");
                    PC++;
                    break;
                case 0x04: case 0x24: case 0x44: case 0x64: case 0x84: case 0xa4: case 0xc4: case 0xe4:
                    {
                        byte arg = opram[PC - pc];
                        PC++;
                        ushort jmpAddr = (ushort)(((op << 3) & 0x700) | arg);
                        instruction = "jmp   $"+jmpAddr.ToString("X4");
                    }
                    break;
                case 0x05:
                    instruction = "en    i";
                    break;
                case 0x07:
                    instruction = "dec   a";
                    break;
                case 0x08: case 0x09: case 0x0a: case 0x0b:
                    instruction = "in    a,p"+(op & 3).ToString();
                    break;
                // ... 必须为所有操作码实现反汇编格式字符串
                // 此示例仅展示少量指令，完整实现需遵循 8x41dasm.c 的逻辑
                case 0x83:
                    instruction = "ret";
                    flags = 2; // DASMFLAG_STEP_OUT
                    break;
                case 0x93:
                    instruction = "retr";
                    flags = 1; // DASMFLAG_STEP_OVER
                    break;
                default:
                    instruction = "db    $"+op.ToString("X2")+"h";
                    break;
            }

            int length = PC - pc;
            // 注意：此处将长度和标志位组合返回，与原C API一致。调用者需解包。
            return length | (int)flags;
        }
    }
}
