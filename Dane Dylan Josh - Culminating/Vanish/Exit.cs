using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Vanish
{
    class Exit
    {
        Statics Stat = new Statics();


        Rectangle rect;

        public double xPos, yPos;
        public int width, height;

        public Exit(int tilePosX, int tilePosY, int tileSizeX, int tileSizeY)
        {
            xPos = tilePosX * Stat.TileSize;
            yPos = tilePosY * Stat.TileSize;

            width = tileSizeX * Stat.TileSize;
            height = tileSizeY * Stat.TileSize;
            drawRect();
        }
        public void drawRect()
        {
            rect = new Rectangle((int)(xPos), (int)(yPos), width, height);
        }
        public void draw(PaintEventArgs e)
        {
            e.Graphics.DrawImage(Properties.Resources.Logo_05, rect);
        }
    }
}
