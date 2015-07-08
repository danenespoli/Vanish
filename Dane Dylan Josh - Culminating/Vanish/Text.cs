using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Vanish
{
    class Text
    {
        //UNUSED
        Statics Stat = new Statics();
        string text;
        Font font;

        public float posX, posY;

        public Text(string s, float tilePosX, float tilePosY, float size)
        {
            size = size / 16 * Stat.TileSize;
            font = new Font("Open Sans", size);
            text = s;

            posX = tilePosX * Stat.TileSize;
            posY = tilePosY * Stat.TileSize;
        }
        public void draw(PaintEventArgs e)
        {
            e.Graphics.DrawString(text, font, new SolidBrush(Stat.Black), new PointF(posX, posY));
        }


    }
}
