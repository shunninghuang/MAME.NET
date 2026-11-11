using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mame
{
    public class fd1094
    {
        public static int selected_state, irq_mode;
        public static int global_key1, global_key2, global_key3;        
        public static ushort[] masked_opcodes =
	{
	0x013a,0x033a,0x053a,0x073a,0x083a,0x093a,0x0b3a,0x0d3a,0x0f3a,

	0x103a,       0x10ba,0x10fa,	0x113a,0x117a,0x11ba,0x11fa,
	0x123a,       0x12ba,0x12fa,	0x133a,0x137a,0x13ba,0x13fa,
	0x143a,       0x14ba,0x14fa,	0x153a,0x157a,0x15ba,
	0x163a,       0x16ba,0x16fa,	0x173a,0x177a,0x17ba,
	0x183a,       0x18ba,0x18fa,	0x193a,0x197a,0x19ba,
	0x1a3a,       0x1aba,0x1afa,	0x1b3a,0x1b7a,0x1bba,
	0x1c3a,       0x1cba,0x1cfa,	0x1d3a,0x1d7a,0x1dba,
	0x1e3a,       0x1eba,0x1efa,	0x1f3a,0x1f7a,0x1fba,

	0x203a,0x207a,0x20ba,0x20fa,	0x213a,0x217a,0x21ba,0x21fa,
	0x223a,0x227a,0x22ba,0x22fa,	0x233a,0x237a,0x23ba,0x23fa,
	0x243a,0x247a,0x24ba,0x24fa,	0x253a,0x257a,0x25ba,
	0x263a,0x267a,0x26ba,0x26fa,	0x273a,0x277a,0x27ba,
	0x283a,0x287a,0x28ba,0x28fa,	0x293a,0x297a,0x29ba,
	0x2a3a,0x2a7a,0x2aba,0x2afa,	0x2b3a,0x2b7a,0x2bba,
	0x2c3a,0x2c7a,0x2cba,0x2cfa,	0x2d3a,0x2d7a,0x2dba,
	0x2e3a,0x2e7a,0x2eba,0x2efa,	0x2f3a,0x2f7a,0x2fba,

	0x303a,0x307a,0x30ba,0x30fa,	0x313a,0x317a,0x31ba,0x31fa,
	0x323a,0x327a,0x32ba,0x32fa,	0x333a,0x337a,0x33ba,0x33fa,
	0x343a,0x347a,0x34ba,0x34fa,	0x353a,0x357a,0x35ba,
	0x363a,0x367a,0x36ba,0x36fa,	0x373a,0x377a,0x37ba,
	0x383a,0x387a,0x38ba,0x38fa,	0x393a,0x397a,0x39ba,
	0x3a3a,0x3a7a,0x3aba,0x3afa,	0x3b3a,0x3b7a,0x3bba,
	0x3c3a,0x3c7a,0x3cba,0x3cfa,	0x3d3a,0x3d7a,0x3dba,
	0x3e3a,0x3e7a,0x3eba,0x3efa,	0x3f3a,0x3f7a,0x3fba,

	0x41ba,0x43ba,0x44fa,0x45ba,0x46fa,0x47ba,0x49ba,0x4bba,0x4cba,0x4cfa,0x4dba,0x4fba,

	0x803a,0x807a,0x80ba,0x80fa,	0x81fa,
	0x823a,0x827a,0x82ba,0x82fa,	0x83fa,
	0x843a,0x847a,0x84ba,0x84fa,	0x85fa,
	0x863a,0x867a,0x86ba,0x86fa,	0x87fa,
	0x883a,0x887a,0x88ba,0x88fa,	0x89fa,
	0x8a3a,0x8a7a,0x8aba,0x8afa,	0x8bfa,
	0x8c3a,0x8c7a,0x8cba,0x8cfa,	0x8dfa,
	0x8e3a,0x8e7a,0x8eba,0x8efa,	0x8ffa,

	0x903a,0x907a,0x90ba,0x90fa,	0x91fa,
	0x923a,0x927a,0x92ba,0x92fa,	0x93fa,
	0x943a,0x947a,0x94ba,0x94fa,	0x95fa,
	0x963a,0x967a,0x96ba,0x96fa,	0x97fa,
	0x983a,0x987a,0x98ba,0x98fa,	0x99fa,
	0x9a3a,0x9a7a,0x9aba,0x9afa,	0x9bfa,
	0x9c3a,0x9c7a,0x9cba,0x9cfa,	0x9dfa,
	0x9e3a,0x9e7a,0x9eba,0x9efa,	0x9ffa,

	0xb03a,0xb07a,0xb0ba,0xb0fa,	0xb1fa,
	0xb23a,0xb27a,0xb2ba,0xb2fa,	0xb3fa,
	0xb43a,0xb47a,0xb4ba,0xb4fa,	0xb5fa,
	0xb63a,0xb67a,0xb6ba,0xb6fa,	0xb7fa,
	0xb83a,0xb87a,0xb8ba,0xb8fa,	0xb9fa,
	0xba3a,0xba7a,0xbaba,0xbafa,	0xbbfa,
	0xbc3a,0xbc7a,0xbcba,0xbcfa,	0xbdfa,
	0xbe3a,0xbe7a,0xbeba,0xbefa,	0xbffa,

	0xc03a,0xc07a,0xc0ba,0xc0fa,	0xc1fa,
	0xc23a,0xc27a,0xc2ba,0xc2fa,	0xc3fa,
	0xc43a,0xc47a,0xc4ba,0xc4fa,	0xc5fa,
	0xc63a,0xc67a,0xc6ba,0xc6fa,	0xc7fa,
	0xc83a,0xc87a,0xc8ba,0xc8fa,	0xc9fa,
	0xca3a,0xca7a,0xcaba,0xcafa,	0xcbfa,
	0xcc3a,0xcc7a,0xccba,0xccfa,	0xcdfa,
	0xce3a,0xce7a,0xceba,0xcefa,	0xcffa,

	0xd03a,0xd07a,0xd0ba,0xd0fa,	0xd1fa,
	0xd23a,0xd27a,0xd2ba,0xd2fa,	0xd3fa,
	0xd43a,0xd47a,0xd4ba,0xd4fa,	0xd5fa,
	0xd63a,0xd67a,0xd6ba,0xd6fa,	0xd7fa,
	0xd83a,0xd87a,0xd8ba,0xd8fa,	0xd9fa,
	0xda3a,0xda7a,0xdaba,0xdafa,	0xdbfa,
	0xdc3a,0xdc7a,0xdcba,0xdcfa,	0xddfa,
	0xde3a,0xde7a,0xdeba,0xdefa,	0xdffa
};
        public static byte[][] masked_opcodes_lookup = new byte[2][] { new byte[65536 / 8 / 2], new byte[65536 / 8 / 2] };
        public static bool masked_opcodes_created = false;


        static int final_decrypt(int i, int moreffff)
        {
            int j;
            int dec = i;
            if ((i & 0xf080) == 0x8000)
            {
                dec ^= 0x0080;
            }
            if ((i & 0xf080) == 0xc080)
            {
                dec ^= 0x0080;
            }
            if ((i & 0xb080) == 0x8000)
            {
                dec ^= 0x4000;
            }
            if ((i & 0xb100) == 0x0000)
            {
                dec ^= 0x4000;
            }
            if (!masked_opcodes_created)
            {
                masked_opcodes_created = true;
                for (j = 0; j < masked_opcodes.Length; j++)
                {
                    ushort opcode = masked_opcodes[j];
                    masked_opcodes_lookup[0][opcode >> 4] |= (byte)(1 << ((opcode >> 1) & 7));
                    masked_opcodes_lookup[1][opcode >> 4] |= (byte)(1 << ((opcode >> 1) & 7));
                }
                for (j = 0; j < 65536; j += 2)
                {
                    if ((j & 0xff80) == 0x4e80 || (j & 0xf0f8) == 0x50c8 || (j & 0xf000) == 0x6000)
                    {
                        masked_opcodes_lookup[1][j >> 4] |= (byte)(1 << ((j >> 1) & 7));
                    }
                }
            }
            if (((masked_opcodes_lookup[moreffff][dec >> 4] >> ((dec >> 1) & 7)) & 1) != 0)
            {
                dec = -1;
            }
            return dec;
        }
        public static int decode(int address, int val, byte[] main_key, int gkey1, int gkey2, int gkey3, int vector_fetch)
        {
            int mainkey, key_F, key_6a, key_7a, key_6b;
            int key_0a, key_0b, key_0c;
            int key_1a, key_1b, key_2a, key_2b, key_3a, key_3b, key_4a, key_4b, key_5a, key_5b;
            int global_xor0, global_xor1;
            int global_swap0a, global_swap1, global_swap2, global_swap3, global_swap4;
            int global_swap0b;
            if ((address & 0x0ffc) == 0 && address >= 4)
            {
                mainkey = main_key[(address & 0x1fff) | 0x1000];
            }
            else
            {
                mainkey = main_key[address & 0x1fff];
            }
            if ((address & 0x1000) != 0)
            {
                key_F = Generic.BIT(mainkey, 7);
            }
            else
            {
                key_F = Generic.BIT(mainkey, 6);
            }
            if (vector_fetch != 0)
            {
                if (address <= 3)
                {
                    gkey3 = 0x00;
                }
                if (address <= 2)
                {
                    gkey2 = 0x00;
                }
                if (address <= 1)
                {
                    gkey1 = 0x00;
                }
                if (address <= 1)
                {
                    key_F = 0;
                }
            }
            global_xor0 = 1 ^ Generic.BIT(gkey1, 5);
            global_xor1 = 1 ^ Generic.BIT(gkey1, 2);
            global_swap2 = 1 ^ Generic.BIT(gkey1, 0);
            global_swap0a = 1 ^ Generic.BIT(gkey2, 5);
            global_swap0b = 1 ^ Generic.BIT(gkey2, 2);
            global_swap3 = 1 ^ Generic.BIT(gkey3, 6);
            global_swap1 = 1 ^ Generic.BIT(gkey3, 4);
            global_swap4 = 1 ^ Generic.BIT(gkey3, 2);
            key_0a = Generic.BIT(mainkey, 0) ^ Generic.BIT(gkey3, 1);
            key_0b = Generic.BIT(mainkey, 0) ^ Generic.BIT(gkey1, 7);
            key_0c = Generic.BIT(mainkey, 0) ^ Generic.BIT(gkey1, 1);
            key_1a = Generic.BIT(mainkey, 1) ^ Generic.BIT(gkey2, 7);
            key_1b = Generic.BIT(mainkey, 1) ^ Generic.BIT(gkey1, 3);
            key_2a = Generic.BIT(mainkey, 2) ^ Generic.BIT(gkey3, 7);
            key_2b = Generic.BIT(mainkey, 2) ^ Generic.BIT(gkey1, 4);
            key_3a = Generic.BIT(mainkey, 3) ^ Generic.BIT(gkey2, 0);
            key_3b = Generic.BIT(mainkey, 3) ^ Generic.BIT(gkey3, 3);
            key_4a = Generic.BIT(mainkey, 4) ^ Generic.BIT(gkey2, 3);
            key_4b = Generic.BIT(mainkey, 4) ^ Generic.BIT(gkey3, 0);
            key_5a = Generic.BIT(mainkey, 5) ^ Generic.BIT(gkey3, 5);
            key_5b = Generic.BIT(mainkey, 5) ^ Generic.BIT(gkey1, 6);
            key_6a = Generic.BIT(mainkey, 6) ^ Generic.BIT(gkey2, 1);
            key_6b = Generic.BIT(mainkey, 6) ^ Generic.BIT(gkey2, 6);
            key_7a = Generic.BIT(mainkey, 7) ^ Generic.BIT(gkey2, 4);
            if ((val & 0xe000) == 0x0000)
            {
                val = Generic.BITSWAP16(val, 12, 15, 14, 13, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0);
            }
            else
            {
                if ((val & 0x8000) != 0)
                {
                    if (global_xor1 == 0)
                    {
                        if ((~val & 0x0008) != 0)
                        {
                            val ^= 0x2410;
                        }
                    }
                    if ((~val & 0x0004) != 0)
                    {
                        val ^= 0x0022;
                    }
                    if (key_1b == 0)
                    {
                        if ((~val & 0x1000) != 0)
                        {
                            val ^= 0x0848;
                        }
                    }
                    if (global_swap2 == 0)
                    {
                        if (key_0c == 0)
                        {
                            val ^= 0x4101;
                        }
                    }
                    if (key_2b == 0)
                    {
                        val = Generic.BITSWAP16(val, 15, 14, 13, 9, 11, 10, 12, 8, 2, 6, 5, 4, 3, 7, 1, 0);
                    }
                    val = 0x6561 ^ Generic.BITSWAP16(val, 15, 9, 10, 13, 3, 12, 0, 14, 6, 5, 2, 11, 8, 1, 4, 7);
                }
                if ((val & 0x4000) != 0)
                {
                    if (global_xor0 == 0)
                    {
                        if ((val & 0x0800) != 0)
                        {
                            val ^= 0x9048;
                        }
                    }
                    if (key_3a == 0)
                    {
                        if ((val & 0x0004) != 0)
                        {
                            val ^= 0x0202;
                        }
                    }
                    if (key_6a == 0)
                    {
                        if ((val & 0x0400) != 0)
                        {
                            val ^= 0x0004;
                        }
                    }
                    if (key_5b == 0)
                    {
                        if (key_0b == 0)
                        {
                            val ^= 0x08a1;
                        }
                    }
                    if (global_swap0b == 0)
                    {
                        val = Generic.BITSWAP16(val, 15, 14, 10, 12, 11, 13, 9, 4, 7, 6, 5, 8, 3, 2, 1, 0);
                    }
                    val = 0x3523 ^ Generic.BITSWAP16(val, 13, 14, 7, 0, 8, 6, 4, 2, 1, 15, 3, 11, 12, 10, 5, 9);
                }
                if ((val & 0x2000) != 0)
                {
                    if (key_4a == 0)
                    {
                        if ((val & 0x0100) != 0)
                        {
                            val ^= 0x4210;
                        }
                    }
                    if (key_1a == 0)
                    {
                        if ((val & 0x0040) != 0)
                        {
                            val ^= 0x0080;
                        }
                    }
                    if (key_7a == 0)
                    {
                        if ((val & 0x0001) != 0)
                        {
                            val ^= 0x110a;
                        }
                    }
                    if (key_4b == 0)
                    {
                        if (key_0a == 0)
                        {
                            val ^= 0x0040;
                        }
                    }
                    if (global_swap0a == 0)
                    {
                        if (key_6b == 0)
                        {
                            val ^= 0x0404;
                        }
                    }
                    if (key_5b == 0)
                    {
                        val = Generic.BITSWAP16(val, 0, 14, 13, 12, 15, 10, 9, 8, 7, 6, 11, 4, 3, 2, 1, 5);
                    }
                    val = 0x99a5 ^ Generic.BITSWAP16(val, 10, 2, 13, 7, 8, 0, 3, 14, 6, 15, 1, 11, 9, 4, 5, 12);
                }
                val = 0x87ff ^ Generic.BITSWAP16(val, 5, 15, 13, 14, 6, 0, 9, 10, 4, 11, 1, 2, 12, 3, 7, 8);
                if (global_swap4 == 0)
                {
                    val = Generic.BITSWAP16(val, 6, 14, 13, 12, 11, 10, 9, 5, 7, 15, 8, 4, 3, 2, 1, 0);
                }
                if (global_swap3 == 0)
                {
                    val = Generic.BITSWAP16(val, 15, 12, 14, 13, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0);
                }
                if (global_swap2 == 0)
                {
                    val = Generic.BITSWAP16(val, 15, 14, 13, 12, 11, 2, 9, 8, 10, 6, 5, 4, 3, 0, 1, 7);
                }
                if (key_3b == 0)
                {
                    val = Generic.BITSWAP16(val, 15, 14, 13, 12, 11, 10, 4, 8, 7, 6, 5, 9, 1, 2, 3, 0);
                }
                if (key_2a == 0)
                {
                    val = Generic.BITSWAP16(val, 15, 12, 13, 14, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0);
                }
                if (global_swap1 == 0)
                {
                    val = Generic.BITSWAP16(val, 15, 14, 13, 12, 9, 8, 11, 10, 7, 6, 5, 4, 3, 2, 1, 0);
                }
                if (key_5a == 0)
                {
                    val = Generic.BITSWAP16(val, 15, 14, 13, 12, 11, 10, 9, 8, 4, 5, 7, 6, 3, 2, 1, 0);
                }
                if (global_swap0a == 0)
                {
                    val = Generic.BITSWAP16(val, 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 0, 3, 2, 1);
                }
            }
            return final_decrypt(val, key_F);
        }
        public static int fd1094_decode(int address, int val, byte[] key, int vector_fetch)
        {
            if (key == null)
            {
                return 0;
            }
            return decode(address, val, key, global_key1, global_key2, global_key3, vector_fetch);
        }
        public static int fd1094_set_state(byte[] key, int state)
        {
            if (key == null)
            {
                return 0;
            }
            if (state == -1)
            {
                state = selected_state;
            }
            switch (state & 0x300)
            {
                case 0x0000:
                    selected_state = state & 0xff;
                    break;
                case 0x0100:
                    selected_state = state & 0xff;
                    irq_mode = 0;
                    break;
                case 0x0200:
                    irq_mode = 1;
                    break;
                case 0x0300:
                    irq_mode = 0;
                    break;
            }
            if (irq_mode != 0)
            {
                state = key[0];
            }
            else
            {
                state = selected_state;
            }
            global_key1 = key[1];
            global_key2 = key[2];
            global_key3 = key[3];
            if ((state & 0x0001) != 0)
            {
                global_key1 ^= 0x04;
                global_key2 ^= 0x80;
                global_key3 ^= 0x80;
            }
            if ((state & 0x0002) != 0)
            {
                global_key1 ^= 0x01;
                global_key2 ^= 0x10;
                global_key3 ^= 0x01;
            }
            if ((state & 0x0004) != 0)
            {
                global_key1 ^= 0x80;
                global_key2 ^= 0x40;
                global_key3 ^= 0x04;
            }
            if ((state & 0x0008) != 0)
            {
                global_key1 ^= 0x20;
                global_key2 ^= 0x02;
                global_key3 ^= 0x20;
            }
            if ((state & 0x0010) != 0)
            {
                global_key1 ^= 0x02;
                global_key1 ^= 0x40;
                global_key2 ^= 0x08;
            }
            if ((state & 0x0020) != 0)
            {
                global_key1 ^= 0x08;
                global_key3 ^= 0x08;
                global_key3 ^= 0x10;
            }
            if ((state & 0x0040) != 0)
            {
                global_key1 ^= 0x10;
                global_key2 ^= 0x20;
                global_key2 ^= 0x04;
            }
            if ((state & 0x0080) != 0)
            {
                global_key2 ^= 0x01;
                global_key3 ^= 0x02;
                global_key3 ^= 0x40;
            }
            return (state & 0xff) | ((irq_mode != 0) ? 0x0200 : 0x0100);
        }
        
    }
}
