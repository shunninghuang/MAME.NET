using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mame
{
    public partial class Tmap
    {
        public void tile_update_ddragon_bg(int logindex, int col, int row)
        {
            int x0 = tilewidth * col;
            int y0 = tileheight * row;
            byte flags;
            int memindex;
            int code, attr, color;
            int pen_data_offset, palette_base;
            memindex = logical_to_memory[logindex];
            attr = Technos.ddragon_bgvideoram[2 * memindex];
            code = (Technos.ddragon_bgvideoram[2 * memindex + 1] + ((attr & 0x07) << 8)) % total_elements;
            color = (attr >> 3) & 0x07;
            flags = (byte)(((attr & 0xc0) >> 6) & 3);
            pen_data_offset = code * 0x100;
            palette_base = 0x100 + 0x10 * color;
            tileflags[logindex] = tile_draw(Technos.gfx3rom, pen_data_offset, x0, y0, palette_base, 0, 0, flags);
        }
        public void tile_update_ddragon_fg(int logindex, int col, int row)
        {
            int x0 = tilewidth * col;
            int y0 = tileheight * row;
            int memindex;
            int code, attr, color;
            int pen_data_offset, palette_base;
            memindex = logical_to_memory[logindex];
            attr = Technos.ddragon_fgvideoram[2 * memindex];
            code = (Technos.ddragon_fgvideoram[2 * memindex + 1] + ((attr & 0x07) << 8)) % total_elements;
            color = attr >> 5;
            pen_data_offset = code * 0x40;
            palette_base = 0x10 * color;
            tileflags[logindex] = tile_draw(Technos.gfx1rom, pen_data_offset, x0, y0, palette_base, 0, 0, 0);
        }
    }
}
