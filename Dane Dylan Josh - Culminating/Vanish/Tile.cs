using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Vanish
{
    class Tile
    {
        Statics Stat = new Statics();
        
        public SolidBrush brush;
        public Rectangle rect;
        public Point loc;
        public int size;

        public bool initialized = false;


        public Tile(Point location)
        {
            brush = new SolidBrush(Stat.Black);
            size = Stat.TileSize;
            
            loc.X = location.X * size;  //creates location based on tile size
            loc.Y = location.Y * size;
            rect = new Rectangle(loc, new Size(size, size));
        }

        internal void toggle()
        {
            if (!initialized)
            {
                initialized = true;     //turns tile on
            }
            else
            {
                initialized = false;    //turns off
            }
        }
        public void initialize()
        {
            initialized = true; //uses initialization to determine whether the tile is drawn (ie, made a wall)
        }

        public void draw(PaintEventArgs e)
        {
            if (initialized)             //all unassigned tiles will not be drawn, making the 
            {                            //game *vastly* more efficient
                e.Graphics.FillRectangle(brush, rect);
            }
        }

    }
}
