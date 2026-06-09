using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mame
{
    public partial class Tmap
    {
        public void tile_update_seibu_kncljoe_bg(int logindex, int col, int row)
        {
            int x0 = tilewidth * col;
            int y0 = tileheight * row;
            int memindex;
            int code, color;
            byte flags;
            int pen_data_offset, palette_base;
            memindex = logical_to_memory[logindex];
            int attr = Generic.videoram[2 * memindex + 1];
            code = Generic.videoram[2 * memindex] + ((attr & 0xc0) << 2) + (Seibu.tile_bank << 10);
            color = attr & 0xf;
            flags = (byte)(Tmap.TILE_FLIPXY((attr & 0x30) >> 4) ^ (attributes & 0x03));
            pen_data_offset = code * 0x40;
            palette_base = 8 * color;
            tileflags[logindex] = tile_draw(Seibu.tilesrom, pen_data_offset, x0, y0, palette_base, 0, 0, flags);
        }
    }
}
