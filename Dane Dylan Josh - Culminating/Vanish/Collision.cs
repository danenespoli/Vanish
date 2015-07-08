using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Vanish
{
    class Collision
    {
        Statics Stat = new Statics();
        public Collision() { }    //no constructor necessary

        //PLAYER AND EXIT
        public void PlayerExit(Player player, Exit exit, World w)
        {
            if (player.xPos >= exit.xPos - 10 && player.xPos + player.size <= exit.xPos + exit.width + 10 &&    //check if player intersects with exit
                player.yPos >= exit.yPos - 10 && player.yPos + player.size <= exit.yPos + exit.height + 10) //10 pixel buffer
            {
                w.gameWon = true;
            }
        }

        //PLAYER PULSE INTERCEPTING ENEMY EARSHOT
        public void PlayerPulseEnemyHearing(Player player, List<Enemy> enemy, World w)
        {
            double dx, dy;
            double radiiP;
            if (!w.gameWon)
            {
                foreach (Enemy e in enemy)
                {
                    if (player.moving && e.hearing) //if the player is moving and enemy can hear
                    {
                        dx = player.xPos - e.xPos;
                        dy = player.yPos - e.yPos;
                        radiiP = ((player.pulse / 2) + (e.hearDist / 2));

                        //pulse collision
                        if (Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2)) <= radiiP) //pulses collide
                        {
                            w.gameLost = true;
                            e.caughtPlayer = true;
                            break;
                        }
                    }
                }
            }
        }

        public void PlayerEnemySight(Player player, List<Enemy> enemy, World w)
        {
            bool noneUp = false;
            bool noneDown = false;
            bool noneLeft = false;
            bool noneRight = false;
            if (!w.gameWon)
            {
                foreach (Enemy e in enemy)
                {
                    if (player.mode != "vanish")    //player/enemy sight intersect does not apply to 'Vanish' mode
                    {
                        //for UP
                        if (e.sightDir == "up" &&
                            player.yPos + player.size > e.sightPoints[1].Y && player.yPos + player.size < e.sightPoints[0].Y && //in between enemy and end of sightline on the X
                            player.xPos < e.sightPoints[0].X - ((player.yPos - e.sightPoints[0].Y) / 3) && player.xPos > e.sightPoints[0].X + ((player.yPos - e.sightPoints[0].Y) / 3))
                        {
                            w.gameLost = true;
                            e.caughtPlayer = true;
                        }
                        //for DOWN
                        if (e.sightDir == "down" &&
                            player.yPos < e.sightPoints[1].Y && player.yPos > e.sightPoints[0].Y && //in between enemy and end of sightline on the X
                            player.xPos + player.size > e.sightPoints[0].X - ((player.yPos - e.sightPoints[0].Y) / 3) && player.xPos + player.size < e.sightPoints[0].X + ((player.yPos - e.sightPoints[0].Y) / 3))
                        {
                            w.gameLost = true;
                            e.caughtPlayer = true;
                        }

                        //for LEFT
                        if (e.sightDir == "left" &&
                            player.xPos + player.size > e.sightPoints[1].X && player.xPos + player.size < e.sightPoints[0].X && //in between enemy and end of sightline on the X
                            player.yPos + player.size < e.sightPoints[0].Y - ((player.xPos - e.sightPoints[0].X) / 3) && player.yPos + player.size > e.sightPoints[0].Y + ((player.xPos - e.sightPoints[0].X) / 3))
                        {
                            w.gameLost = true;
                            e.caughtPlayer = true;
                        }
                        //for RIGHT
                        if (e.sightDir == "right" &&
                            player.xPos < e.sightPoints[1].X && player.xPos > e.sightPoints[0].X && //in between enemy and end of sightline on the X
                            player.yPos > e.sightPoints[0].Y - ((player.xPos - e.sightPoints[0].X) / 3) && player.yPos < e.sightPoints[0].Y + ((player.xPos - e.sightPoints[0].X) / 3))
                        {
                            w.gameLost = true;
                            e.caughtPlayer = true;
                        }


                        if (noneUp)
                        {
                            e.sightDist = e.SIGHT_DIST;
                        }
                        if (noneDown)
                        {
                            e.sightDist = e.SIGHT_DIST;
                        }
                        if (noneLeft)
                        {
                            e.sightDist = e.SIGHT_DIST;
                        }
                        if (noneRight)
                        {
                            e.sightDist = e.SIGHT_DIST;
                        }

                        e.drawRect();
                    }
                }
            }
        }

        public void EnemySightTiles(Tile[,] grid, List<Enemy> enemy)    //stop cone at walls
        {
            bool noneUp = false;
            bool noneDown = false;
            bool noneLeft = false;
            bool noneRight = false;

            foreach (Tile t in grid)
            {
                if (t.initialized)  //if the tile is a wall
                {
                    foreach (Enemy e in enemy)
                    {
                        //for UP
                        if (e.sightDir == "up" &&
                            t.loc.Y + Stat.TileSize > e.sightPoints[1].Y && t.loc.Y + Stat.TileSize < e.sightPoints[0].Y && //in between enemy and end of sightline on the X
                            t.loc.X < e.sightPoints[0].X - ((t.loc.Y - e.sightPoints[0].Y) / 3) && t.loc.X > e.sightPoints[0].X + ((t.loc.Y - e.sightPoints[0].Y) / 3))
                        {
                            e.sightPoints[1].Y = t.loc.Y + Stat.TileSize;
                            e.sightPoints[2].Y = t.loc.Y + Stat.TileSize;
                            e.sightDist = e.sightPoints[0].Y - (t.loc.Y + Stat.TileSize); //sight dist should be positive even if being subtracted (accounted for in drawRect)
                            noneLeft = false;
                        }
                        //for DOWN
                        if (e.sightDir == "down" &&
                            t.loc.Y < e.sightPoints[1].Y && t.loc.Y > e.sightPoints[0].Y && //in between enemy and end of sightline on the X
                            t.loc.X > e.sightPoints[0].X - ((t.loc.Y - e.sightPoints[0].Y) / 3) && t.loc.X < e.sightPoints[0].X + ((t.loc.Y - e.sightPoints[0].Y) / 3))
                        {
                            e.sightPoints[1].Y = t.loc.Y;
                            e.sightPoints[2].Y = t.loc.Y;
                            e.sightDist = t.loc.Y - e.sightPoints[0].Y;
                            noneDown = false;
                        }

                        //for LEFT
                        if (e.sightDir == "left" &&
                            t.loc.X + Stat.TileSize > e.sightPoints[1].X && t.loc.X + Stat.TileSize < e.sightPoints[0].X && //in between enemy and end of sightline on the X
                            t.loc.Y < e.sightPoints[0].Y - ((t.loc.X - e.sightPoints[0].X) / 3) && t.loc.Y > e.sightPoints[0].Y + ((t.loc.X - e.sightPoints[0].X) / 3))
                        {
                            e.sightPoints[1].X = t.loc.X + Stat.TileSize;
                            e.sightPoints[2].X = t.loc.X + Stat.TileSize;
                            e.sightDist = e.sightPoints[0].X - (t.loc.X + Stat.TileSize); //sight dist should be positive
                            noneLeft = false;
                        }
                        //for RIGHT
                        if (e.sightDir == "right" &&
                            t.loc.X < e.sightPoints[1].X && t.loc.X > e.sightPoints[0].X && //in between enemy and end of sightline on the X
                            t.loc.Y > e.sightPoints[0].Y - ((t.loc.X - e.sightPoints[0].X) / 3) && t.loc.Y < e.sightPoints[0].Y + ((t.loc.X - e.sightPoints[0].X) / 3))
                        {
                            e.sightPoints[1].X = t.loc.X;
                            e.sightPoints[2].X = t.loc.X;
                            e.sightDist = t.loc.X - e.sightPoints[0].X;
                            noneRight = false;
                        }


                        if (noneUp)
                        {
                            e.sightDist = e.SIGHT_DIST;
                        }
                        if (noneDown)
                        {
                            e.sightDist = e.SIGHT_DIST;
                        }
                        if (noneLeft)
                        {
                            e.sightDist = e.SIGHT_DIST;
                        }
                        if (noneRight)
                        {
                            e.sightDist = e.SIGHT_DIST;
                        }

                        e.drawRect();
                    }
                }
            }
        }
        
        //PLAYER INTERCEPTING TILES/DOORS   (Doors are technically a workaround in which walls are drawn beneath locked doors so you cannot walk through them)
        public void PlayerTiles(Player player, Tile[,] grid)
        {   

            bool noneUp = true;
            bool noneDown = true;
            bool noneLeft = true;
            bool noneRight = true;
            foreach (Tile t in grid)
            {
                if (t.initialized)  //if a wall
                {
                    double xPosP = player.xPos + (player.size / 2); //new CENTRED position vars
                    double yPosP = player.yPos + (player.size / 2);
                    double xPosT = t.loc.X + (Stat.TileSize / 2);
                    double yPosT = t.loc.Y + (Stat.TileSize / 2);
                    double overcomeDist = (player.size + Stat.TileSize) / 2;

                    if (player.yPos <= 0 ||
                        yPosT - yPosP >= -overcomeDist && yPosT - yPosP <= 0 &&    //tile above
                        xPosT - xPosP <= (overcomeDist / 2) && xPosT - xPosP >= -(overcomeDist / 2))
                    {
                        player.upC = false;   //can't move up
                        noneUp = false;
                    }
                    if (player.yPos + player.size >= Stat.pbCanvasHeight ||
                        yPosT - yPosP <= overcomeDist && yPosT - yPosP >= 0 &&    //tile below
                        xPosT - xPosP <= (overcomeDist / 2) && xPosT - xPosP >= -(overcomeDist / 2))
                    {
                        player.downC = false;   //can't move down
                        noneDown = false;
                    }
                    if (player.xPos <= 0 ||
                        xPosT - xPosP >= -overcomeDist && xPosT - xPosP <= 0 &&    //tile on left
                        yPosT - yPosP <= (overcomeDist / 2) && yPosT - yPosP >= -(overcomeDist / 2))
                    {
                        player.leftC = false;   //can't move left
                        noneLeft = false;
                    }
                    if (player.xPos + player.size >= Stat.pbCanvasWidth ||
                        xPosT - xPosP <= overcomeDist && xPosT - xPosP >= 0 &&    //tile on right
                        yPosT - yPosP <= (overcomeDist / 2) && yPosT - yPosP >= -(overcomeDist / 2))
                    {
                        player.rightC = false;   //can't move right
                        noneRight = false;
                    }
                }
            }


            if (noneUp)     //if none in the way, let it pass through IF player.up == true also
            {
                player.upC = true;
            }
            if (noneDown)
            {
                player.downC = true;
            }
            if (noneLeft)
            {
                player.leftC = true;
            }
            if (noneRight)
            {
                player.rightC = true;
            }
        }





    }
}
