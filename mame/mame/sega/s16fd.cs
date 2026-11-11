using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using cpu.m68000;

namespace mame
{
    public partial class sega
    {
        public static byte[] fd1094_key;
        public static ushort[] fd1094_cpuregion;
        public static int fd1094_cpuregionsize;
        public static ushort[] fd1094_userregion;
        public static ushort[][] fd1094_cacheregion = new ushort[8][];
        public static int[] fd1094_cached_states = new int[8];
        public static int fd1094_current_cacheposition;
        public static int fd1094_state;
        public static int fd1094_selected_state;
        public static Action<ushort[]> fd1094_set_decrypted;
        public static ushort[] fd1094_get_decrypted_base()
        {
            if (fd1094_key == null)
            {
                return null;
            }
            return fd1094_userregion;
        }
        public static void set_decrypted_region()
        {
            if (fd1094_set_decrypted != null)
            {
                fd1094_set_decrypted(fd1094_userregion);
            }
            else
            {
                //memory_set_decrypted_region(0, 0, fd1094_cpuregionsize - 1, fd1094_userregion);
            }
        }
        public static void fd1094_setstate_and_decrypt(int state)
        {
            int i;
            int addr;
            switch (state & 0x300)
            {
                case 0x000:
                case 0x0100:
                    fd1094_selected_state = state & 0xff;
                    break;
            }
            fd1094_state = state;
            //cpunum_set_info_int(0, CPUINFO_INT_REGISTER + M68K_PREF_ADDR, 0x0010);
            state = fd1094.fd1094_set_state(fd1094_key, state) & 0xff;
            for (i = 0; i < 8; i++)
            {
                if (fd1094_cached_states[i] == state)
                {
                    fd1094_userregion = fd1094_cacheregion[i];
                    set_decrypted_region();
                    //m68k_set_encrypted_opcode_range(0, 0, fd1094_cpuregionsize);
                    return;
                }
            }
            fd1094_cached_states[fd1094_current_cacheposition] = state;
            for (addr = 0; addr < fd1094_cpuregionsize / 2; addr++)
            {
                ushort dat;
                dat = (ushort)fd1094.fd1094_decode(addr, fd1094_cpuregion[addr], fd1094_key, 0);
                fd1094_cacheregion[fd1094_current_cacheposition][addr] = dat;
            }
            fd1094_userregion = fd1094_cacheregion[fd1094_current_cacheposition];
            set_decrypted_region();
            //m68k_set_encrypted_opcode_range(0, 0, fd1094_cpuregionsize);
            fd1094_current_cacheposition++;
            if (fd1094_current_cacheposition >= 8)
            {
                fd1094_current_cacheposition = 0;
            }
        }
        public static void fd1094_cmp_callback(uint val, int reg)
        {
            if (reg == 0 && (val & 0x0000ffff) == 0x0000ffff)
            {
                fd1094_setstate_and_decrypt((int)((val & 0xffff0000) >> 16));
            }
        }
        public static int fd1094_int_callback(int irqline)
        {
            fd1094_setstate_and_decrypt(0x0200);
            return (0x60 + irqline * 4) / 4;
        }
        public static void fd1094_rte_callback()
        {
            fd1094_setstate_and_decrypt(0x0300);
        }
        public static void fd1094_kludge_reset_values()
        {
            int i;
            for (i = 0; i < 4; i++)
            {
                fd1094_userregion[i] = (ushort)fd1094.fd1094_decode(i, fd1094_cpuregion[i], fd1094_key, 1);
            }
        }
        public static void fd1094_machine_init()
        {
            if (fd1094_key == null)
            {
                return;
            }
            fd1094_setstate_and_decrypt(0x0100);
            fd1094_kludge_reset_values();
            MC68000.mm1[0].cmpild_instr_callback = fd1094_cmp_callback;
            MC68000.mm1[0].rte_instr_callback = fd1094_rte_callback;
            Cpuint.cpunum_set_irq_callback(0, fd1094_int_callback);
        }
        public static void fd1094_postload()
        {
            if (fd1094_state != -1)
            {
                int selected_state = fd1094_selected_state;
                int state = fd1094_state;
                fd1094_machine_init();
                fd1094_setstate_and_decrypt(selected_state);
                fd1094_setstate_and_decrypt(state);
            }
        }
        public static void key_changed()
        {
            int addr;
            for (addr = 0; addr < fd1094_cpuregionsize / 2; addr++)
            {
                ushort dat;
                dat = (ushort)fd1094.fd1094_decode(addr, fd1094_cpuregion[addr], fd1094_key, 0);
                fd1094_cacheregion[0][addr] = dat;
            }
            fd1094_userregion = fd1094_cacheregion[0];
            set_decrypted_region();
            fd1094_current_cacheposition = 1;
            //cpunum_set_info_int(0, CPUINFO_INT_REGISTER + M68K_PREF_ADDR, 0x0010);
        }
        public static void fd1094_driver_init(Action<ushort[]> set_decrypted)
        {
            int i;
            fd1094_cpuregion = mainrom2;
            fd1094_cpuregionsize = Memory.mainrom.Length;
            fd1094_key = maincpukeyrom;
            fd1094_set_decrypted = set_decrypted;
            if (fd1094_key == null)
            {
                return;
            }
            for (i = 0; i < 8; i++)
            {
                fd1094_cacheregion[i] = new ushort[fd1094_cpuregionsize / 2];
                fd1094_cached_states[i] = -1;
            }
            fd1094_current_cacheposition = 0;
            fd1094_state = -1;
            /*if ((machine.debug_flags & DEBUG_FLAG_ENABLED) != 0 && memory_region(machine, "soundcpu:key") != null)
            {
                fd1094_init_debugging(machine, "maincpu", "maincpu:key", "soundcpu:key", key_changed);
            }
            state_save_register_global(nameof(fd1094_selected_state));
            state_save_register_global(nameof(fd1094_state));
            state_save_register_postload(machine, fd1094_postload);*/
        }
        public static void SaveStateBinary_s16fd(BinaryWriter writer)
        {
            writer.Write(fd1094_selected_state);
            writer.Write(fd1094_state);
        }
        public static void LoadStateBinary_s16fd(BinaryReader reader)
        {
            fd1094_selected_state = reader.ReadInt32();
            fd1094_state = reader.ReadInt32();
        }
    }
}
