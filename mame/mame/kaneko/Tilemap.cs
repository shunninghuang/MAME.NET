using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mame
{
    public partial class Tmap
    {
        public void tile_update_kaneko_bg(int logindex, int col, int row)
        {
            int x0 = tilewidth * col;
            int y0 = tileheight * row;
            int memindex;
            int code, color;
            int pen_data_offset, palette_base;
            memindex = logical_to_memory[logindex];
            int attr = Generic.colorram[memindex];
            code = Generic.videoram[memindex] + ((attr & 0x0f) << 8);
            color = (attr >> 4) + 16;
            pen_data_offset = code * 0x100;
            palette_base = 0x10 * color;
            tileflags[logindex] = tile_draw(Kaneko.gfx1rom, pen_data_offset, x0, y0, palette_base, 0, 0, 0);
        }
        public void tile_update_kaneko_fg(int logindex, int col, int row)
        {
            int x0 = tilewidth * col;
            int y0 = tileheight * row;
            int memindex;
            int code, color;
            int pen_data_offset, palette_base;
            memindex = logical_to_memory[logindex];
            int attr = Kaneko.airbustr_colorram2[memindex];
            code = Kaneko.airbustr_videoram2[memindex] + ((attr & 0x0f) << 8);
            color = attr >> 4;
            pen_data_offset = code * 0x100;
            palette_base = 0x10 * color;
            tileflags[logindex] = tile_draw(Kaneko.gfx1rom, pen_data_offset, x0, y0, palette_base, 0, 0, 0);
        }
    }
}
