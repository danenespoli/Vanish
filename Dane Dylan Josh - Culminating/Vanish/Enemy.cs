using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Vanish
{
    class Enemy
    {
        Statics Stat = new Statics();
        //all enemies
        public SolidBrush brush = new SolidBrush(Color.Firebrick);
        public Rectangle rect;

        public int size;
        public double xPos, yPos;

        public string construct;
        public double xPosStart, yPosStart; //for Constructor A
        public double xPosEnd, yPosEnd;
        public List<double> pathPointX = new List<double>();    //for Constructor B
        public List<double> pathPointY = new List<double>();
        public int pathNum = 0;
        public int nextPath = 1;

        public double xAmt, yAmt;

        public double pace;

        
        //hearing
        public bool hearing = false;
        public SolidBrush hearBrush = new SolidBrush(Color.FromArgb(10, Color.Firebrick));
        public Rectangle hearRect;  //rectangle of ellipse
        public int hearDist;

        //seeing
        public bool seeing = false;
        public SolidBrush sightBrush = new SolidBrush(Color.FromArgb(35, Color.Firebrick));
        public Point[] sightPoints = new Point[3];  //point array of cone
        public int SIGHT_DIST;
        public int sightDist;
        public string sightDir;

        public bool caughtPlayer = false;


        //Constructor A *
        public Enemy(Point start, Point end, double speed, int hear, int see)
        {
            construct = "A";
            size = Stat.TileSize * 2;
            
            xPosStart = start.X * Stat.TileSize; //tile index * Stat.TileSize (tile size)
            yPosStart = start.Y * Stat.TileSize;
            xPosEnd = end.X * Stat.TileSize;
            yPosEnd = end.Y * Stat.TileSize;

            xPos = xPosStart;
            yPos = yPosStart;

            pace = (speed / 100) * Stat.TileSize * Stat.difficulty;   //* Stat.TileSize to keep things proportional for varying resolutions
                                                //(speed / 100) to prevent input speed from being very small annoying decimals
            drawRect();
            evalAmt(xPosStart, xPosEnd, yPosStart, yPosEnd);


            if (hear != 0)
            {
                hearDist = hear * Stat.TileSize;
                hearing = true;
            }
            if (see != 0)
            {
                SIGHT_DIST = see * Stat.TileSize;
                sightDist = SIGHT_DIST;
                seeing = true;
            }
        }

        //Constructor B **
        public Enemy(List<Point> points, double speed, int hear, int see)
        {
            construct = "B";
            size = Stat.TileSize * 2;
            
            foreach (Point point in points)
            {
                pathPointX.Add(point.X * Stat.TileSize);
                pathPointY.Add(point.Y * Stat.TileSize);
            }

            xPos = pathPointX[0];
            yPos = pathPointY[0];

            pace = (speed / 100) * Stat.TileSize * Stat.difficulty;   //* Stat.TileSize to keep things proportional for varying resolutions
                                                //(speed / 100) to prevent input speed from being very small annoying decimals

            drawRect();
            evalAmt(pathPointX[0], pathPointX[1], pathPointY[0], pathPointY[1]);

            if (hear != 0)
            {
                hearDist = hear * Stat.TileSize;
                hearing = true;
            }
            if (see != 0)
            {
                SIGHT_DIST = see * Stat.TileSize;
                sightDist = SIGHT_DIST;
                seeing = true;
            }
        }







        public void drawRect()
        {
            rect = new Rectangle((int)(xPos), (int)(yPos), size, size);

            if (hearing)
            {
                hearRect = new Rectangle((int)(xPos + ((size - hearDist) / 2)), (int)(yPos + ((size - hearDist) / 2)), hearDist, hearDist);
            }
            if (seeing)
            {
                sightPoints[0] = new Point((int)(xPos + (size / 2)), (int)(yPos + (size / 2)));


                //sight cone direction
                if (xAmt > 0)
                {
                    sightPoints[1].X = sightPoints[0].X + sightDist;
                    sightPoints[2].X = sightPoints[0].X + sightDist;
                    sightPoints[1].Y = sightPoints[0].Y - (sightDist / 3);
                    sightPoints[2].Y = sightPoints[0].Y + (sightDist / 3);
                    sightDir = "right";
                }
                else if (xAmt < 0)
                {
                    sightPoints[1].X = sightPoints[0].X - sightDist;
                    sightPoints[2].X = sightPoints[0].X - sightDist;
                    sightPoints[1].Y = sightPoints[0].Y - (sightDist / 3);
                    sightPoints[2].Y = sightPoints[0].Y + (sightDist / 3);
                    sightDir = "left";
                }
                else if (yAmt > 0)
                {
                    sightPoints[1].Y = sightPoints[0].Y + sightDist;
                    sightPoints[2].Y = sightPoints[0].Y + sightDist;
                    sightPoints[1].X = sightPoints[0].X - (sightDist / 3);
                    sightPoints[2].X = sightPoints[0].X + (sightDist / 3);
                    sightDir = "down";
                }
                else if (yAmt < 0)
                {
                    sightPoints[1].Y = sightPoints[0].Y - sightDist;
                    sightPoints[2].Y = sightPoints[0].Y - sightDist;
                    sightPoints[1].X = sightPoints[0].X - (sightDist / 3);
                    sightPoints[2].X = sightPoints[0].X + (sightDist / 3);
                    sightDir = "up";
                }

            }
        }

        public void evalAmt(double xStart, double xEnd, double yStart, double yEnd)
        {
                if (xEnd - xStart > 0)
                {
                    xAmt = pace;                    //determine speed axially
                }
                else if (xEnd - xStart < 0)
                {
                    xAmt = -pace;
                }
                else
                {
                    xAmt = 0;
                }

                if (yEnd - yStart > 0)
                {
                    yAmt = pace;
                }
                else if (yEnd - yStart < 0)
                {
                    yAmt = -pace;
                }
                else
                {
                    yAmt = 0;
                }

        }

        //for Constructor A only *
        public void reversePath()
        {
            double xTemp;
            double yTemp;

            xTemp = xPosEnd;
            yTemp = yPosEnd;

            xPosEnd = xPosStart;
            yPosEnd = yPosStart;

            xPosStart = xTemp;
            yPosStart = yTemp;

            xAmt *= -1;
            yAmt *= -1;

            if (seeing)
            {
                sightDist = SIGHT_DIST;
            }
        }
        //for Constructor B only **
        public void incrementPath()
        {
            pathNum++;
            nextPath++;

            if (pathNum == pathPointX.Count - 1)
            {
                nextPath = 0;
            }
            if (nextPath == 1)
            {
                pathNum = 0;
            }

            if (seeing)
            {
                sightDist = SIGHT_DIST;
            }
            evalAmt(pathPointX[pathNum], pathPointX[nextPath], pathPointY[pathNum], pathPointY[nextPath]);

        }



        public void EnemyMove()
        {
            if (construct == "A")
            {
                if (xPos <= xPosEnd + (pace / 2) && xPos >= xPosEnd - (pace / 2) &&   //buffer amount
                    yPos <= yPosEnd + (pace / 2) && yPos >= yPosEnd - (pace / 2))
                {
                    reversePath();
                }
                else if (xPos != xPosEnd ||
                         yPos != yPosEnd)
                {
                    xPos += xAmt;
                    yPos += yAmt;
                    drawRect();
                }

            }
            else if (construct == "B")
            {

                if (xPos <= pathPointX[nextPath] + (pace / 2) && xPos >= pathPointX[nextPath] - (pace / 2) &&   //buffer amount
                    yPos <= pathPointY[nextPath] + (pace / 2) && yPos >= pathPointY[nextPath] - (pace / 2))
                {
                    incrementPath(); //move onto next path if at the end of a path
                }
                else
                {
                    xPos += xAmt; //otherwise move as normal
                    yPos += yAmt;
                    drawRect();
                }
            }
        }

        //to draw enemies
        public void draw(PaintEventArgs e, World w)
        {
            e.Graphics.FillEllipse(brush, rect);
            if (hearing)
            {
                e.Graphics.FillEllipse(hearBrush, hearRect);
            }
            if (seeing)
            {
                w.collision.EnemySightTiles(w.grid, w.enemies); //check for cone size before drawing
                e.Graphics.FillPolygon(sightBrush, sightPoints);
            }
        }



    }
}
