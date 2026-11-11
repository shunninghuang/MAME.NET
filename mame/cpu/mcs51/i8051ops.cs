using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cpu.mcs51
{
    public partial class I8051
    {
        void acall()
        {
            byte op = ReadOp((ushort)(pc - 1));
            byte addr = ReadOpArg(pc++);
            push_pc();
            pc = (ushort)((pc & 0xf800) | ((op & 0xe0) << 3) | addr);
        }
        void add_a_byte()
        {
            byte data = ReadOpArg(pc++);
            byte result = (byte)(acc + data);
            do_add_flags(acc, data, 0);
            sfr_write(0xe0, result);
        }
        void add_a_mem()
        {
            byte addr = ReadOpArg(pc++);
            byte data = internal_ram_read(addr);
            byte result = (byte)(acc + data);
            do_add_flags(acc, data, 0);
            sfr_write(0xe0, result);
        }
        void add_a_ir(int r)
        {
            byte data = internal_ram_iread(R_R(r));
            byte result = (byte)(acc + data);
            do_add_flags(acc, data, 0);
            sfr_write(0xe0, result);
        }
        void add_a_r(int r)
        {
            byte data = R_R(r);
            byte result = (byte)(acc + data);
            do_add_flags(acc, data, 0);
            sfr_write(0xe0, result);
        }
        void addc_a_byte()
        {
            byte data = ReadOpArg(pc++);
            byte result = (byte)(acc + data + GET_CY());
            do_add_flags(acc, data, GET_CY());
            sfr_write(0xe0, result);
        }
        void addc_a_mem()
        {
            byte addr = ReadOpArg(pc++);
            byte data = internal_ram_read(addr);
            byte result = (byte)(acc + data + GET_CY());
            do_add_flags(acc, data, GET_CY());
            sfr_write(0xe0, result);
        }
        void addc_a_ir(int r)
        {
            byte data = internal_ram_iread(R_R(r));
            byte result = (byte)(acc + data + GET_CY());
            do_add_flags(acc, data, GET_CY());
            sfr_write(0xe0, result);
        }
        void addc_a_r(int r)
        {
            byte data = R_R(r);
            byte result = (byte)(acc + data + GET_CY());
            do_add_flags(acc, data, GET_CY());
            sfr_write(0xe0, result);
        }
        void ajmp()
        {
            byte op = ReadOp((ushort)(pc - 1));
            byte addr = ReadOpArg(pc++);
            pc = (ushort)((pc & 0xf800) | ((op & 0xe0) << 3) | addr);
        }
        void anl_mem_a()
        {
            byte addr = ReadOpArg(pc++);
            byte data = internal_ram_read(addr);
            internal_ram_write(addr, (byte)(data & acc));
        }
        void anl_mem_byte()
        {
            byte addr = ReadOpArg(pc++);
            byte data = ReadOpArg(pc++);
            byte src_data = internal_ram_read(addr);
            internal_ram_write(addr, (byte)(src_data & data));
        }
        void anl_a_byte()
        {
            byte data = ReadOpArg(pc++);
            sfr_write(0xe0, (byte)(acc & data));
        }
        void anl_a_mem()
        {
            byte addr = ReadOpArg(pc++);
            byte data = internal_ram_read(addr);
            sfr_write(0xe0, (byte)(acc & data));
        }
        void anl_a_ir(int r)
        {
            byte data = internal_ram_iread(R_R(r));
            sfr_write(0xe0, (byte)(acc & data));
        }
        void anl_a_r(int r)
        {
            byte data = R_R(r);
            sfr_write(0xe0, (byte)(acc & data));
        }
        void anl_c_bitaddr()
        {
            int cy = GET_CY();
            byte addr = ReadOpArg(pc++);
            byte bit = bit_address_r(addr);
            SET_CY(cy & bit);
        }
        void anl_c_nbitaddr()
        {
            int cy = GET_CY();
            byte addr = ReadOpArg(pc++);
            byte bit = bit_address_r(addr);
            bit = (byte)((~bit) & 1);
            SET_CY(cy & bit);
        }
        void cjne_a_byte()
        {
            byte data = ReadOpArg(pc++);
            sbyte rel_addr = (sbyte)ReadOpArg(pc++);
            if (acc != data)
            {
                pc = (ushort)(pc + rel_addr);
            }
            SET_CY(acc < data ? 1 : 0);
        }
        void cjne_a_mem()
        {
            byte addr = ReadOpArg(pc++);
            sbyte rel_addr = (sbyte)ReadOpArg(pc++);
            byte data = internal_ram_read(addr);
            if (acc != data)
            {
                pc = (ushort)(pc + rel_addr);
            }
            SET_CY(acc < data ? 1 : 0);
        }
        void cjne_ir_byte(int r)
        {
            byte data = ReadOpArg(pc++);
            sbyte rel_addr = (sbyte)ReadOpArg(pc++);
            byte src_data = internal_ram_iread(R_R(r));
            if (src_data != data)
            {
                pc = (ushort)(pc + rel_addr);
            }
            SET_CY(src_data < data ? 1 : 0);
        }
        void cjne_r_byte(int r)
        {
            byte data = ReadOpArg(pc++);
            sbyte rel_addr = (sbyte)ReadOpArg(pc++);
            byte src_data = R_R(r);
            if (src_data != data)
            {
                pc = (ushort)(pc + rel_addr);
            }
            SET_CY(src_data < data ? 1 : 0);
        }
        void clr_bitaddr()
        {
            byte addr = ReadOpArg(pc++);
            bit_address_w(addr, 0);
        }
        void clr_c()
        {
            SET_CY(0);
        }
        void clr_a()
        {
            sfr_write(0xe0, 0);
        }
        void cpl_bitaddr()
        {
            byte addr = ReadOpArg(pc++);
            byte data = (byte)((~bit_address_r(addr)) & 1);
            bit_address_w(addr, data);
        }
        void cpl_c()
        {
            byte bit = (byte)((~GET_CY()) & 1);
            SET_CY(bit);
        }
        void cpl_a()
        {
            byte data = (byte)((~acc) & 0xff);
            sfr_write(0xe0, data);
        }
        void da_a()
        {
            ushort new_acc = (ushort)(acc & 0xff);
            if (GET_AC() != 0 || (new_acc & 0x0f) > 0x09)
            {
                new_acc += 0x06;
            }
            if (GET_CY() != 0 || ((new_acc & 0xf0) > 0x90) || (new_acc & ~0xff) != 0)
            {
                new_acc += 0x60;
            }
            sfr_write(0xe0, (byte)(new_acc & 0xff));
            if ((new_acc & ~0xff) != 0)
            {
                SET_CY(1);
            }
        }
        void dec_a()
        {
            sfr_write(0xe0, (byte)(acc - 1));
        }
        void dec_mem()
        {
            byte addr = ReadOpArg(pc++);
            byte data = internal_ram_read(addr);
            internal_ram_write(addr, (byte)(data - 1));
        }
        void dec_ir(int r)
        {
            byte data = internal_ram_iread(R_R(r));
            internal_ram_write(R_R(r), (byte)(data - 1));
        }
        void dec_r(int r)
        {
            IntRam[(GET_RS() * 8) + r] = (byte)(R_R(r) - 1);
        }
        void div_ab()
        {
            if (b == 0)
            {
                SET_OV(1);
            }
            else
            {
                int quot = acc / b;
                int rem = acc % b;
                sfr_write(0xe0, (byte)quot);
                sfr_write(0xf0, (byte)rem);
                SET_OV(0);
            }
            SET_CY(0);
        }
        void djnz_mem()
        {
            byte addr = ReadOpArg(pc++);
            sbyte rel_addr = (sbyte)ReadOpArg(pc++);
            internal_ram_write(addr, (byte)(internal_ram_read(addr) - 1));
            if (internal_ram_read(addr) != 0)
            {
                pc = (ushort)(pc + rel_addr);
            }
        }
        void djnz_r(int r)
        {
            sbyte rel_addr = (sbyte)ReadOpArg(pc++);
            IntRam[(GET_RS() * 8) + r] = (byte)(R_R(r) - 1);
            if (R_R(r) != 0)
            {
                pc = (ushort)(pc + rel_addr);
            }
        }
        void inc_a()
        {
            sfr_write(0xe0, (byte)(acc + 1));
        }
        void inc_mem()
        {
            byte addr = ReadOpArg(pc++);
            byte data = internal_ram_read(addr);
            internal_ram_write(addr,(byte)(data + 1));
        }
        void inc_ir(int r)
        {
            byte data = internal_ram_iread(R_R(r));
            internal_ram_write(R_R(r), (byte)(data + 1));
        }
        void inc_r(int r)
        {
            byte data = R_R(r);
            IntRam[(GET_RS() * 8) + r] = (byte)(data + 1);
        }
        void inc_dptr()
        {
            ushort d = (ushort)(R_DPTR() + 1);
            DPTR_W(d);
        }
        void jb()
        {
            byte addr = ReadOpArg(pc++);
            sbyte rel_addr = (sbyte)ReadOpArg(pc++);
            if (bit_address_r(addr) != 0)
            {
                pc = (ushort)(pc + rel_addr);
            }
        }
        void jbc()
        {
            byte addr = ReadOpArg(pc++);
            sbyte rel_addr = (sbyte)ReadOpArg(pc++);
            if (bit_address_r(addr) != 0)
            {
                pc = (ushort)(pc + rel_addr);
                bit_address_w(addr, 0);
            }
        }
        void jc()
        {
            sbyte rel_addr = (sbyte)ReadOpArg(pc++);
            if (GET_CY() != 0)
            {
                pc = (ushort)(pc + rel_addr);
            }
        }
        void jmp_iadptr()
        {
            pc = (ushort)(acc + R_DPTR());
        }
        void jnb()
        {
            byte addr = ReadOpArg(pc++);
            sbyte rel_addr = (sbyte)ReadOpArg(pc++);
            if (bit_address_r(addr) == 0)
            {
                pc = (ushort)(pc + rel_addr);
            }
        }
        void jnc()
        {
            sbyte rel_addr = (sbyte)ReadOpArg(pc++);
            if (GET_CY() == 0)
            {
                pc = (ushort)(pc + rel_addr);
            }
        }
        void jnz()
        {
            sbyte rel_addr = (sbyte)ReadOpArg(pc++);
            if (acc != 0)
            {
                pc = (ushort)(pc + rel_addr);
            }
        }
        void jz()
        {
            sbyte rel_addr = (sbyte)ReadOpArg(pc++);
            if (acc == 0)
            {
                pc = (ushort)(pc + rel_addr);
            }
        }
        void lcall()
        {
            byte addr_hi = ReadOpArg(pc++);
            byte addr_lo = ReadOpArg(pc++);
            push_pc();
            pc = (ushort)((addr_hi << 8) | addr_lo);
        }
        void ljmp()
        {
            byte addr_hi = ReadOpArg(pc++);
            byte addr_lo = ReadOpArg(pc++);
            pc = (ushort)((addr_hi << 8) | addr_lo);
        }
        void mov_a_byte()
        {
            byte data = ReadOpArg(pc++);
            sfr_write(0xe0, data);
        }
        void mov_a_mem()
        {
            byte addr = ReadOpArg(pc++);
            sfr_write(0xe0, internal_ram_read(addr));
        }
        void mov_a_ir(int r)
        {
            sfr_write(0xe0, internal_ram_iread(R_R(r)));
        }
        void mov_a_r(int r)
        {
            sfr_write(0xe0, R_R(r));
        }
        void mov_mem_byte()
        {
            byte addr = ReadOpArg(pc++);
            byte data = ReadOpArg(pc++);
            internal_ram_write(addr, data);
        }
        void mov_mem_mem()
        {
            byte src = ReadOpArg(pc++);
            byte dst = ReadOpArg(pc++);
            internal_ram_write(dst, internal_ram_read(src));
        }
        void mov_ir_byte(int r)
        {
            byte data = ReadOpArg(pc++);
            internal_ram_iwrite(R_R(r), data);
        }
        void mov_r_byte(int r)
        {
            byte data = ReadOpArg(pc++);
            IntRam[(GET_RS() * 8) + r] = data;
        }
        void mov_mem_ir(int r)
        {
            byte addr = ReadOpArg(pc++);
            internal_ram_write(addr, internal_ram_iread(R_R(r)));
        }
        void mov_mem_r(int r)
        {
            byte addr = ReadOpArg(pc++);
            internal_ram_write(addr, R_R(r));
        }
        void mov_dptr_byte()
        {
            byte data_hi = ReadOpArg(pc++);
            byte data_lo = ReadOpArg(pc++);
            DPTR_W((data_hi << 8) | data_lo);
        }
        void mov_bitaddr_c()
        {
            byte addr = ReadOpArg(pc++);
            bit_address_w(addr, GET_CY());
        }
        void mov_ir_mem(int r)
        {
            byte addr = ReadOpArg(pc++);
            internal_ram_iwrite(R_R(r), internal_ram_read(addr));
        }
        void mov_r_mem(int r)
        {
            byte addr = ReadOpArg(pc++);
            IntRam[(GET_RS() * 8) + r] = internal_ram_read(addr);
        }
        void mov_mem_a()
        {
            byte addr = ReadOpArg(pc++);
            internal_ram_write(addr, acc);
        }
        void mov_ir_a(int r)
        {
            internal_ram_iwrite(R_R(r), acc);
        }
        void mov_r_a(int r)
        {
            IntRam[(GET_RS() * 8) + r] = acc;
        }
        void movc_a_iapc()
        {
            byte data = ReadMemory((ushort)(acc + pc));
            sfr_write(0xe0, data);
        }
        void mov_c_bitaddr()
        {
            byte addr = ReadOpArg(pc++);
            SET_CY(bit_address_r(addr));
        }
        void movc_a_iadptr()
        {
            byte data = ReadMemory((ushort)(acc + R_DPTR()));
            sfr_write(0xe0, data);
        }
        void movx_a_idptr()
        {
            uint addr = external_ram_iaddr(R_DPTR(), 0xFFFF);
            byte data = ReadData((ushort)addr);
            sfr_write(0xe0, data);
        }
        void movx_a_ir(int r)
        {
            uint addr = external_ram_iaddr(R_R(r), 0xFF);
            byte data = ReadData((ushort)addr);
            sfr_write(0xe0, data);
        }
        void movx_idptr_a()
        {
            uint addr = external_ram_iaddr(R_DPTR(), 0xFFFF);
            WriteData((ushort)addr, (byte)acc);
        }
        void movx_ir_a(int r)
        {
            uint addr = external_ram_iaddr(R_R(r), 0xFF);
            WriteData((ushort)addr, (byte)acc);
        }
        void mul_ab()
        {
            int result = acc * b;
            sfr_write(0xf0, (byte)((result & 0xFF00) >> 8));
            sfr_write(0xe0, (byte)(result & 0x00FF));
            SET_OV((result & 0x100) >> 8);
            SET_CY(0);
        }
        void nop()
        {

        }
        void orl_mem_a()
        {
            byte addr = ReadOpArg(pc++);
            byte data = internal_ram_read(addr);
            internal_ram_write(addr, (byte)(data | acc));
        }
        void orl_mem_byte()
        {
            byte addr = ReadOpArg(pc++);
            byte data = ReadOpArg(pc++);
            byte src_data = internal_ram_read(addr);
            internal_ram_write(addr, (byte)(src_data | data));
        }
        void orl_a_byte()
        {
            byte data = ReadOpArg(pc++);
            sfr_write(0xe0, (byte)(acc | data));
        }
        void orl_a_mem()
        {
            byte addr = ReadOpArg(pc++);
            byte data = internal_ram_read(addr);
            sfr_write(0xe0, (byte)(acc | data));
        }
        void orl_a_ir(int r)
        {
            byte data = internal_ram_iread(R_R(r));
            sfr_write(0xe0, (byte)(acc | data));
        }
        void orl_a_r(int r)
        {
            byte data = R_R(r);
            sfr_write(0xe0, (byte)(acc | data));
        }
        void orl_c_bitaddr()
        {
            int cy = GET_CY();
            byte addr = ReadOpArg(pc++);
            byte bit = bit_address_r(addr);
            SET_CY(cy | bit);
        }
        void orl_c_nbitaddr()
        {
            int cy = GET_CY();
            byte addr = ReadOpArg(pc++);
            byte bit = bit_address_r(addr);
            bit = (byte)((~bit) & 1);
            SET_CY(cy | bit);
        }
        void pop()
        {
            byte addr = ReadOpArg(pc++);
            internal_ram_write(addr, internal_ram_iread(sp));
            sfr_write(0x81, (byte)(sp - 1));
        }
        void push()
        {
            byte addr = ReadOpArg(pc++);
            byte tmp_sp = sp;
            tmp_sp++;
            sfr_write(0x81, tmp_sp);
            if (tmp_sp == sp)
            {
                internal_ram_iwrite(tmp_sp, internal_ram_read(addr));
            }
        }
        void ret()
        {
            pop_pc();
        }
        void reti()
        {
            pop_pc();
            CLEAR_CURRENT_IRQ();
        }
        void rl_a()
        {
            int carry = (acc & 0x80) >> 7;
            int data = (acc << 1) & 0xfe;
            sfr_write(0xe0, (byte)(data | carry));
        }
        void rlc_a()
        {
            int carry = (acc & 0x80) >> 7;
            int data = ((acc << 1) & 0xfe) | GET_CY();
            sfr_write(0xe0, (byte)data);
            SET_CY(carry);
        }
        void rr_a()
        {
            int carry = (acc & 1) << 7;
            int data = (acc >> 1) & 0x7f;
            sfr_write(0xe0, (byte)(data | carry));
        }
        void rrc_a()
        {
            int carry = acc & 1;
            int data = ((acc >> 1) & 0x7f) | (GET_CY() << 7);
            sfr_write(0xe0, (byte)data);
            SET_CY(carry);
        }
        void setb_c()
        {
            SET_CY(1);
        }
        void setb_bitaddr()
        {
            byte addr = ReadOpArg(pc++);
            bit_address_w(addr, 1);
        }
        void sjmp()
        {
            sbyte rel_addr = (sbyte)ReadOpArg(pc++);
            pc = (ushort)(pc + rel_addr);
        }
        void subb_a_byte()
        {
            byte data = ReadOpArg(pc++);
            byte result = (byte)(acc - data - GET_CY());
            do_sub_flags(acc, data, GET_CY());
            sfr_write(0xe0, result);
        }
        void subb_a_mem()
        {
            byte addr = ReadOpArg(pc++);
            byte data = internal_ram_read(addr);
            byte result = (byte)(acc - data - GET_CY());
            do_sub_flags(acc, data, GET_CY());
            sfr_write(0xe0, result);
        }
        void subb_a_ir(int r)
        {
            byte data = internal_ram_iread(R_R(r));
            byte result = (byte)(acc - data - GET_CY());
            do_sub_flags(acc, data, GET_CY());
            sfr_write(0xe0, result);
        }
        void subb_a_r(int r)
        {
            byte data = R_R(r);
            byte result = (byte)(acc - data - GET_CY());
            do_sub_flags(acc, data, GET_CY());
            sfr_write(0xe0, result);
        }
        void swap_a()
        {
            byte a_nib_hi = (byte)((acc & 0x0f) << 4);
            byte a_nib_lo = (byte)((acc & 0xf0) >> 4);
            sfr_write(0xe0, (byte)(a_nib_hi | a_nib_lo));
        }
        void xch_a_mem()
        {
            byte addr = ReadOpArg(pc++);
            byte data = internal_ram_read(addr);
            byte old_acc = acc;
            sfr_write(0xe0, data);
            internal_ram_write(addr, old_acc);
        }
        void xch_a_ir(int r)
        {
            byte data = internal_ram_iread(R_R(r));
            byte old_acc = acc;
            sfr_write(0xe0, data);
            internal_ram_write(R_R(r), old_acc);
        }
        void xch_a_r(int r)
        {
            byte data = R_R(r);
            byte old_acc = acc;
            sfr_write(0xe0, data);
            IntRam[(GET_RS() * 8) + r] = old_acc;
        }
        void xchd_a_ir(int r)
        {
            byte ir_data = internal_ram_iread(R_R(r));
            byte acc_val = acc;
            sfr_write(0xe0, (byte)((acc_val & 0xf0) | (ir_data & 0x0f)));
            internal_ram_write(R_R(r), (byte)((ir_data & 0xf0) | (acc_val & 0x0f)));
        }
        void xrl_mem_a()
        {
            byte addr = ReadOpArg(pc++);
            byte data = internal_ram_read(addr);
            internal_ram_write(addr, (byte)(data ^ acc));
        }
        void xrl_mem_byte()
        {
            byte addr = ReadOpArg(pc++);
            byte data = ReadOpArg(pc++);
            byte src_data = internal_ram_read(addr);
            internal_ram_write(addr, (byte)(src_data ^ data));
        }
        void xrl_a_byte()
        {
            byte data = ReadOpArg(pc++);
            sfr_write(0xe0, (byte)(acc ^ data));
        }
        void xrl_a_mem()
        {
            byte addr = ReadOpArg(pc++);
            byte data = internal_ram_read(addr);
            sfr_write(0xe0, (byte)(acc ^ data));
        }
        void xrl_a_ir(int r)
        {
            byte data = internal_ram_iread(R_R(r));
            sfr_write(0xe0, (byte)(acc ^ data));
        }
        void xrl_a_r(int r)
        {
            byte data = R_R(r);
            sfr_write(0xe0,(byte)( acc ^ data));
        }
        void illegal()
        {

        }
    }
}
