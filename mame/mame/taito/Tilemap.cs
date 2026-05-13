using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mame
{
    public partial class Tmap
    {
        public void tile_update_taito_bg_opwolf(int logindex, int col, int row)
        {
            int x0 = tilewidth * col;
            int y0 = tileheight * row;
            byte flags;
            int memindex;
            int code, color, attr;
            int pen_data_offset, palette_base;
            //memindex = row * cols + col;
            memindex = logical_to_memory[logindex];
            if (Taito.PC080SN_dblwidth == 0)
            {
                code = (Taito.PC080SN_ram[0][Taito.PC080SN_bg_ram_offset[0][0] + 2 * memindex + 1] & 0x3fff);
                attr = Taito.PC080SN_ram[0][Taito.PC080SN_bg_ram_offset[0][0] + 2 * memindex];
            }
            else
            {
                code = (Taito.PC080SN_ram[0][Taito.PC080SN_bg_ram_offset[0][0] + memindex + 0x2000] & 0x3fff);
                attr = Taito.PC080SN_ram[0][Taito.PC080SN_bg_ram_offset[0][0] + memindex];
            }
            color = attr & 0x1ff;
            code = code % Taito.PC080SN_tilemap[0][0].total_elements;
            pen_data_offset = code * 0x40;
            palette_base = 0x10 * color;
            flags = (byte)((((attr & 0xc000) >> 14) & 3) ^ (attributes & 0x03));
            tileflags[logindex] = tile_draw(Taito.gfx1rom, pen_data_offset, x0, y0, palette_base, 0, 0, flags);
        }
        public void tile_update_taito_fg_opwolf(int logindex, int col, int row)
        {
            int x0 = tilewidth * col;
            int y0 = tileheight * row;
            byte flags;
            int memindex;
            int code, color, attr;
            int pen_data_offset, palette_base;
            //memindex = row * cols + col;
            memindex = logical_to_memory[logindex];
            if (Taito.PC080SN_dblwidth == 0)
            {
                code = (Taito.PC080SN_ram[0][Taito.PC080SN_bg_ram_offset[0][1] + 2 * memindex + 1] & 0x3fff);
                attr = Taito.PC080SN_ram[0][Taito.PC080SN_bg_ram_offset[0][1] + 2 * memindex];
            }
            else
            {
                code = (Taito.PC080SN_ram[0][Taito.PC080SN_bg_ram_offset[0][1] + memindex + 0x2000] & 0x3fff);
                attr = Taito.PC080SN_ram[0][Taito.PC080SN_bg_ram_offset[0][1] + memindex];
            }
            color = attr & 0x1ff;
            code = code % Taito.PC080SN_tilemap[0][1].total_elements;
            pen_data_offset = code * 0x40;
            palette_base = 0x10 * color;
            flags = (byte)((((attr & 0xc000) >> 14) & 3) ^ (attributes & 0x03));
            tileflags[logindex] = tile_draw(Taito.gfx1rom, pen_data_offset, x0, y0, palette_base, 0, 0, flags);
        }
    }
}
