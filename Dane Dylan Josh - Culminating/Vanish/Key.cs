using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Vanish
{
    class Key
    {
        Statics Stat = new Statics();
        
        public double xPos, yPos;
        public double width;
        public double height;
        Color color;
        public Rectangle rect;

        public SolidBrush brush;

        public Door doorForKey;

        public bool initially = true;


        public Key(PointF tilePos, Color c, List<Door> d)
        {
            xPos = tilePos.X * Stat.TileSize;  // (size of a tile)
            yPos = tilePos.Y * Stat.TileSize;

            width = 1.5 * Stat.TileSize;
            height = 3 * Stat.TileSize;

            doorForKey = d[d.Count-1];   //assign (last) door to key
            color = c;

            brush = new SolidBrush(color);
            doorForKey.brush = brush;

            drawRect();
        }

        public void drawRect()
        {
            rect = new Rectangle((int)(xPos), (int)(yPos), (int)width, (int)height);
        }

        public void draw(PaintEventArgs e, Player player, World w)
        {
            if (doorForKey.locked)
            {
                if (color == Stat.Red)  //depending on key colour
                {
                    e.Graphics.DrawImage(Properties.Resources.keyRed, rect);    //Draw different Key Image
                }
                if (color == Stat.Blue)
                {
                    e.Graphics.DrawImage(Properties.Resources.keyBlue, rect);
                }
                if (color == Stat.Green)
                {
                    e.Graphics.DrawImage(Properties.Resources.keyGreen, rect);
                }
                if (color == Stat.Gold)
                {
                    e.Graphics.DrawImage(Properties.Resources.keyGold, rect);
                }

                if (player.xPos + player.size > xPos && player.xPos < xPos + width &&  //Player collides with key
                    player.yPos + player.size > yPos && player.yPos < yPos + height ||
                    initially)
                {
                    doorForKey.toggleLocked(w, this);
                    initially = false;
                }
            }






        }
    }
}
