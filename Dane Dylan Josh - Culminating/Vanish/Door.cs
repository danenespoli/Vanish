using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Vanish
{
    class Door
    {
        Statics Stat = new Statics();
        
        public double xPos, yPos;
        public double xPosClosed, yPosClosed;
        public double size;
        public bool vert;

        public SolidBrush brush;
        public Rectangle rect;

        public int open = -1;   //1 is open, -1 is closed
        public int openPositive;    //1 is openPositive, -1 is openNegative
        public bool sliding = false;

        public int doorProximity;  //proximity required to open an unlocked door
        public double slideDuration = 20;
        public double slideCount = 0;
        public double amt; //amount to slide per tick

        public bool locked;


        public Door(int tilePosX, int tilePosY, bool vertical, int openPos, int tileSize, bool isLocked)
        {
            doorProximity = 8 * Stat.TileSize;

            xPos = tilePosX * Stat.TileSize;
            yPos = tilePosY * Stat.TileSize;
              
            xPosClosed = xPos;  //initialize all variables
            yPosClosed = yPos;

            size = tileSize * Stat.TileSize;
            vert = vertical;
            openPositive = openPos;

            locked = isLocked;

            drawRect();
        }
        public void drawRect()
        {
            if (vert)
            { //most intuitive way to do it for world generation (as opposed to height/width)
                rect = new Rectangle((int)(xPos), (int)(yPos), Stat.TileSize, (int)size);
            }
            else
            {
                rect = new Rectangle((int)(xPos), (int)(yPos), (int)size, Stat.TileSize);
            }
        }
        public void draw(PaintEventArgs e, Player player, Tile[,] grid)
        {

            if (!locked)    //if unlocked, use default gray door colour
            {
                brush = new SolidBrush(Color.FromArgb(175, Stat.Black));
            }
            e.Graphics.FillRectangle(brush, rect);


            if (sliding)
            {
                slide();  //continue slide animation every timer tick...
            } //Check Door Proximity
            else if (shouldSlide(player, grid))  //if it should slide
            {
                startSlide(); //start slide animation
            }
        }



        //for locked only
        public void toggleLocked(World w, Key k)
        {
            if (locked && !k.initially)
            {
                locked = false;
            }
            else
            {
                locked = true;
            }

            if (vert) //to make sure the player can't walk through a door that won't open
            {                   //initialize tiles under the door so it's like a wall
                for (int i = (int)(yPos / Stat.TileSize); i < (int)(yPos / Stat.TileSize) + (int)(size / Stat.TileSize); i++)
                {
                    w.grid[(int)(xPos / Stat.TileSize), i].toggle();
                }
            }
            else
            {
                for (int i = (int)(xPos / Stat.TileSize); i < (int)(xPos / Stat.TileSize) + (int)(size / Stat.TileSize); i++)
                {
                    w.grid[i, (int)(yPos / Stat.TileSize)].toggle();
                }
            }
        }



        //OPENING ANIMATION
        public bool shouldSlide(Player player, Tile[,] grid)
        {
            bool noneBetween = true;   //referring to wall tiles in the way of door (don't want the door opening if there's a wall separating the door from the player)
            foreach (Tile t in grid)
            {
                if (vert)
                {
                    if (t.initialized &&
                        ((t.loc.X > player.xPos && t.loc.X < xPosClosed) ||     //Tile X in between Player and Door (horizontally)
                        (t.loc.X < player.xPos && t.loc.X > xPosClosed))
                        &&                                                  //AND
                         (t.loc.Y > yPosClosed && t.loc.Y < yPosClosed + size))     //Tile Y in between door top and bottom
                    {
                        noneBetween = false;
                    }
                }
                else if (!vert)
                {
                    if (t.initialized &&
                        ((t.loc.Y > player.yPos && t.loc.Y < yPosClosed) ||     //Tile Y in between Player and Door (horizontally)
                        (t.loc.Y < player.yPos && t.loc.Y > yPosClosed))
                        &&                                                  //AND
                         (t.loc.X > xPosClosed && t.loc.X < xPosClosed + size))     //Tile X in between door top and bottom
                    {
                        noneBetween = false;
                    }
                }
            }

                //if there are none inbetween then:

            if (player.xPos + doorProximity > xPosClosed && player.xPos - doorProximity < xPosClosed &&    //proximal to door
                player.yPos + doorProximity > yPosClosed && player.yPos - doorProximity < yPosClosed &&
                noneBetween)    //FIX Door not working
            {
                if (open == -1 && locked == false)   //unopened and unlocked 
                {
                    return true;  //open
                }
            }
            else
            {
                if (open == 1)  //if open
                {
                    return true;  //close
                }
            }

            return false;
        }
        public void startSlide()
        {
            amt = size / slideDuration;
            sliding = true;

            if (open == 1)
            {
                amt *= -1;  //determine positive/negative directional amount
            }
            if (openPositive == -1)
            {
                amt *= -1;
            }
        }
        public void slide()
        {
            if (slideCount < slideDuration)
            {
                if (vert)
                {
                    yPos += amt;    //add amount on timer tick
                }
                else
                {
                    xPos += amt;
                }

                slideCount++;
            }
            else   //until finished
            {
                slideCount = 0; //reset counter
                sliding = false;

                if (open == -1) //if was closed
                {
                    open = 1;   //now open
                }
                else
                {
                    open = -1;  //else closed
                }
            }
            drawRect();
        }



    }
}
