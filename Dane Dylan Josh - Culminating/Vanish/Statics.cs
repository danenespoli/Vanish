using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Vanish
{
    class Statics
    {
        public Rectangle resolution = Screen.PrimaryScreen.Bounds;
        
        public int tilesWide;
        public int tilesHigh;

        public double pbCanvasWidth;
        public double pbCanvasHeight;

        public double pbCanvasLocX;
        public double pbCanvasLocY;

        public int TileSize;


        public Color Black = Color.FromArgb(255, 30, 30, 30);    //off-black
        //key colors
        public Color Red = Color.FromArgb(255, 190, 30, 45);    //red     
        public Color Blue = Color.FromArgb(255, 55, 116, 186);  //blue
        public Color Green = Color.FromArgb(255, 37, 125, 29);  //green
        public Color Gold = Color.FromArgb(255, 252, 177, 22);  //gold

        public double difficulty;

        public Statics()    //all consts are stored here
        {
            //************USER INPUT NECESSARY*************
            difficulty = 1;
            //*********************************************


            resolution.Height -= 70;  //to account for taskbar

            tilesWide = 80;
            tilesHigh = 64;

            TileSize = (int)(resolution.Height / tilesHigh);

            pbCanvasWidth = tilesWide * TileSize;
            pbCanvasHeight = tilesHigh * TileSize;

            pbCanvasLocX = (resolution.Width - pbCanvasWidth) / 2;
            pbCanvasLocY = (resolution.Height - pbCanvasHeight) / 2;

            
        }


    }
}
