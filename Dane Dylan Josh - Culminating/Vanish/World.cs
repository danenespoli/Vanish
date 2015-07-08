using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Vanish
{

    class World
    {
        public Statics Stat = new Statics();

        public Player player;
        public Color playerColor;

        public float scale = 2;
        public Tile[,] grid;
        public List<Enemy> enemies = new List<Enemy>(); //list of enemies
        public List<Door> doors = new List<Door>(); //list of doors
        public List<Key> keys = new List<Key>(); //list of enemies
        public List<List<Point>> enemyPoints = new List<List<Point>>(); //for the enemies with complex paths
        public List<Text> texts = new List<Text>(); //for instructions (only first level really)

        public Exit exit;   //single physical exit for player

        public int timeLimit;
        int time;   //for time limit
        Text timeText;
        public bool gameLost;   //triggering end game events (fade out, closing form, etc)
        public bool gameWon;

        public Collision collision = new Collision();

        //CONSTRUCTOR
        public World(int level)
        {
            playerColor = Stat.Black;   //initialize player colour

            switch (level)  //based on level, use individualized method
            {               //to initialize and create level
                case 1:
                    fillGrid();
                    Level_1();
                    break;
                case 2:
                    fillGrid();
                    Level_2();
                    break;
                case 3:
                    fillGrid();
                    Level_3();
                    break;
                case 4:
                    fillGrid();
                    Level_4();
                    break;
            }
        }
        void fillGrid()
        {
            grid = new Tile[Stat.tilesWide, Stat.tilesHigh];
            for (int y = 0; y < Stat.tilesHigh; y++)
            {
                for (int x = 0; x < Stat.tilesWide; x++)
                {
                    grid[x, y] = new Tile(new Point(x, y)); //create all tiles
                }
            }
        }

        int count = 1;
        public void refresh(PaintEventArgs e)
        {
            //Drawing

            //WORLD GRID *    
            foreach (Tile t in grid)
            {
                t.draw(e);
            }
            //DOORS **
            foreach (Door d in doors)
            {
                d.draw(e, player, grid);
            }
            //KEYS ***
            foreach (Key k in keys)
            {
                k.draw(e, player, this);
            }
            //TEXT ****
            foreach (Text te in texts)
            {
                te.draw(e);
            }

            //EXIT ******
            exit.draw(e);

            //ENEMIES *****
            foreach (Enemy en in enemies)
            {
                en.draw(e, this);
            }

            //PLAYER
            player.Draw(e);


            //Collision
            collision.PlayerTiles(player, grid);                        //player        |  walls
            collision.PlayerPulseEnemyHearing(player, enemies, this);   //player pulse  |  enemy earshot
            collision.EnemySightTiles(grid, enemies);                   //enemy sight   |  tiles
            collision.PlayerEnemySight(player, enemies, this);          //player        |  enemy sight
            collision.PlayerExit(player, exit, this);                   //player        |  exit

            //Movement
            player.PlayerMove(this);

            foreach (Enemy en in enemies)
            {
                en.EnemyMove();
            }

            if (gameLost || gameWon)    //for end game animations and events
            {                           //regardless of game won or lost (sorted out in method)
                if (!endGameStarted)
                {
                    endGameStart();
                }
                else
                {
                    endGameAnimate(e);
                }
            }
            else    //increment timer otherwise
            {
                if (count < 62) //not accurate to the second, timer lagging?
                {
                    count++;
                }
                else if (time != 0)
                {
                    time--;
                    count = 0;
                }
                if (time == 0)
                {
                    gameLost = true;
                }
            }
            timeText = new Text(time.ToString(), 0, 0, 40); //draw time text on
            timeText.draw(e);
        }

        bool endGameStarted;
        public void endGameStart()
        {
            endGameStarted = true;
            alpInc = 255 /lim;
        }
        double endAnimateTime = 0;
        double lim = 100;
        double alp = 0;
        double alpInc;
        public bool gameLostDone = false;
        public bool gameWonDone = false;

        public void endGameAnimate(PaintEventArgs e)
        {
            foreach (Enemy en in enemies)
            {
                en.xAmt = en.xAmt * 0.90;   //gradually decelerate 
                en.yAmt = en.yAmt * 0.90;
                ////if (en.caughtPlayer)  
                ////{
                //    en.xAmt = 0;   //stop completely so you can see collision point
                //    en.yAmt = 0;
                ////}
            }
            player.Speed = 0;

            if (endAnimateTime < lim)
            {
                if (gameLost)   //fade to black
                {
                    e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb((int)alp, Stat.Black)), new Rectangle(0, 0, (int)Stat.pbCanvasWidth, (int)Stat.pbCanvasHeight));
                }
                if (gameWon)    //fade to white
                {
                    e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb((int)alp, Color.WhiteSmoke)), new Rectangle(0, 0, (int)Stat.pbCanvasWidth, (int)Stat.pbCanvasHeight));
                }

                    alp += alpInc;
                    endAnimateTime++;
            }
            else
            {
                if (gameLost)
                {
                    gameLostDone = true;
                }
                if (gameWon)
                {
                    gameWonDone = true;
                }
                endGameStarted = false;
                endAnimateTime = 0;
            }
        }



        public void drawFrom(Point a, Point b)  //Point A must be smaller than Point B. This is too aid in level design
        {
            if (a.X == b.X)
            {
                for (int y = a.Y; y <= b.Y; y++) {
                    grid[a.X, y].initialize(); 
                }
            }
            else if (a.Y == b.Y)
            {
                for (int x = a.X; x <= b.X; x++)
                {
                    grid[x, a.Y].initialize();
                }
            }
        }
        public void exclude(Point a, Point b)   //can turn sections off
        {
            if (a.X == b.X)
            {
                for (int y = a.Y; y <= b.Y; y++)
                {
                    grid[a.X, y].toggle();
                }
            }
            else if (a.Y == b.Y)
            {
                for (int x = a.X; x <= b.X; x++)
                {
                    grid[x, a.Y].toggle();
                }
            }
        }

        //LEVEL 1 *
        public void Level_1()
        {
            timeLimit = (int)(30 / Stat.difficulty);
            time = timeLimit;

            //Initialize player *
            player = new Player(playerColor, new Point(16 * Stat.TileSize, 42 * Stat.TileSize));


            //Level generation (walls/doors/keys/enemies) **
            //starting zone
            drawFrom(new Point(13, 45), new Point(21, 45)); //bottom part
            drawFrom(new Point(13, 38), new Point(13, 45)); //left part
            drawFrom(new Point(21, 38), new Point(21, 45)); //right part
            doors.Add(new Door(14, 37, false, -1, 7, false));

            //outside square
            drawFrom(new Point(3, 37), new Point(31, 37));  //bottom part
            exclude(new Point(14, 37), new Point(20, 37));
            drawFrom(new Point(3, 9), new Point(31, 9));  //top part
            exclude(new Point(14, 9), new Point(20, 9));
            doors.Add(new Door(14, 9, false, -1, 7, false));
            drawFrom(new Point(3, 9), new Point(3, 37));  //left part
            drawFrom(new Point(31, 9), new Point(31, 48));  //right part    (also acts as additional part)

            enemyPoints.Add(new List<Point> { new Point(8, 31), new Point(8, 14), new Point(25, 14), new Point(25, 31) });  //RUSH Enemy
            enemies.Add(new Enemy(enemyPoints[enemyPoints.Count - 1], 40, 6, 16));

            //inside square
            drawFrom(new Point(14, 20), new Point(14, 26)); //left part
            drawFrom(new Point(14, 20), new Point(20, 20)); //top part
            drawFrom(new Point(20, 20), new Point(20, 26)); //right part
            drawFrom(new Point(14, 26), new Point(20, 26)); //bottom part
            exclude(new Point(16, 26), new Point(18, 26)); //hole for key


            //transition hall and stealth part
            drawFrom(new Point(13, 2), new Point(13, 9));   //left wall
            drawFrom(new Point(13, 2), new Point(57, 2));   //top of hall
            drawFrom(new Point(57, 2), new Point(57, 37));   //right of wall
            enemies.Add(new Enemy(new Point(35, 12), new Point(35, 35), 20, 8, 0));
            enemies.Add(new Enemy(new Point(52, 35), new Point(52, 12), 20, 8, 0));
            drawFrom(new Point(41, 12), new Point(41, 38));   //inside left wall
            drawFrom(new Point(47, 12), new Point(47, 38));  //inside right wall

            //transition hall to last hallway
            drawFrom(new Point(57, 38), new Point(75, 38));   //top
            drawFrom(new Point(75, 38), new Point(75, 59));   //right
            //door between
            doors.Add(new Door(66, 48, false, -1, 9, true));
            keys.Add(new Key(new PointF(16.8f, 22f), Stat.Red, doors));  //red key

            //last hallway enemy
            enemies.Add(new Enemy(new Point(23, 53), new Point(59, 53), 19, 0, 14));  //bottom hall  |  back and forth


            //around exit


            drawFrom(new Point(9, 48), new Point(65, 48));  //top part  (also top of last hall)
            drawFrom(new Point(9, 59), new Point(75, 59));  //bottom part   (also bottom of last hall)
            drawFrom(new Point(9, 48), new Point(9, 59));  //left part

            exit = new Exit(9, 49, 10, 10);
        }

        //LEVEL 2 **
        public void Level_2()
        {
            timeLimit = (int)(50 / Stat.difficulty);
            time = timeLimit;

            //Initialize player *
            player = new Player(playerColor, new Point(4 * Stat.TileSize, 12 * Stat.TileSize));



            //around entrance
            drawFrom(new Point(1, 5), new Point(8, 5));  //top part
            drawFrom(new Point(1, 15), new Point(8, 15)); //bottom
            drawFrom(new Point(1, 5), new Point(1, 15));  //left part
            drawFrom(new Point(8, 5), new Point(8, 15));  //right part
            exclude(new Point(8, 5), new Point(8, 10));  //hole in right

            //exit room
            drawFrom(new Point(11, 20), new Point(19, 20));  //top part
            drawFrom(new Point(11, 10), new Point(11, 20));  //top part
            drawFrom(new Point(19, 10), new Point(19, 20)); //left part
            doors.Add(new Door(12, 10, false, -1, 7, true));
            keys.Add(new Key(new PointF(45.5f, 57f), Stat.Gold, doors));  //Gold key

            exit = new Exit(12, 13, 7, 7);

            //transition hall
            drawFrom(new Point(8, 5), new Point(50, 5)); //top
            drawFrom(new Point(8, 10), new Point(50, 10)); //bottom
            drawFrom(new Point(50, 5), new Point(50, 52)); //right line
            exclude(new Point(50, 22), new Point(50, 28));
            exclude(new Point(22, 10), new Point(49, 10));

            //main room
            drawFrom(new Point(22, 10), new Point(22, 48)); // left
            drawFrom(new Point(22, 48), new Point(50, 48));//bottom
            exclude(new Point(22, 36), new Point(22, 44));
            exclude(new Point(42, 48), new Point(49, 48));

            enemyPoints.Add(new List<Point> { new Point(26, 9), new Point(26, 42), new Point(45, 42), new Point(45, 9) });
            enemies.Add(new Enemy(enemyPoints[enemyPoints.Count - 1], 30, 6, 16));

            enemyPoints.Add(new List<Point> { new Point(45, 42), new Point(45, 9), new Point(26, 9), new Point(26, 42) });
            enemies.Add(new Enemy(enemyPoints[enemyPoints.Count - 1], 30, 6, 16));

            //left room
            drawFrom(new Point(12, 36), new Point(22, 36)); //top
            drawFrom(new Point(12, 44), new Point(22, 44)); // bottom
            drawFrom(new Point(12, 36), new Point(12, 44)); //left
            doors.Add(new Door(22, 37, true, -1, 7, true));
            keys.Add(new Key(new PointF(54f, 24.5f), Stat.Red, doors));  //Red key

            //right room
            drawFrom(new Point(62, 12), new Point(78, 12));  //room
            drawFrom(new Point(62, 34), new Point(78, 34));
            drawFrom(new Point(62, 12), new Point(62, 34));
            drawFrom(new Point(78, 12), new Point(78, 34));

            drawFrom(new Point(50, 22), new Point(62, 22)); //corridor
            drawFrom(new Point(50, 29), new Point(62, 29));

            doors.Add(new Door(62, 23, true, -1, 6, true));
            keys.Add(new Key(new PointF(17f, 39f), Stat.Blue, doors));  //Blue key

            enemies.Add(new Enemy(new Point(29, 25), new Point(45, 25), 20, 6, 8));

            enemyPoints.Add(new List<Point> { new Point(65, 30), new Point(74, 30), new Point(74, 15), new Point(65, 15) });

            enemies.Add(new Enemy(enemyPoints[enemyPoints.Count - 1], 15, 6, 16));

            //bottom room
            drawFrom(new Point(42, 48), new Point(42, 52));  //entrance
            drawFrom(new Point(35, 52), new Point(56, 52));  //top
            drawFrom(new Point(35, 62), new Point(56, 62));  //bottom
            drawFrom(new Point(35, 52), new Point(35, 62));
            drawFrom(new Point(56, 52), new Point(56, 62));
            exclude(new Point(43, 52), new Point(49, 52));
            doors.Add(new Door(43, 52, false, -1, 7, true));
            keys.Add(new Key(new PointF(70f, 14f), Stat.Green, doors));  //Green key

            exclude(new Point(12, 10), new Point(18, 10));
            exclude(new Point(62, 23), new Point(62, 28)); //opening
        }

        //LEVEL 3 ***
        public void Level_3()
        {
            timeLimit = 35; //beat this level in 14 seconds (but that's Dylan
            time = timeLimit;
 
            //Initialize player *
            player = new Player(playerColor, new Point(39 * Stat.TileSize, 60 * Stat.TileSize));
 
            #region tiles
            //starting stuff
            drawFrom(new Point(36, 57), new Point(36, 62)); //west side
            drawFrom(new Point(44, 57), new Point(44, 62)); //east side
            drawFrom(new Point(36, 63), new Point(44, 63)); //south side      
            //first zone
            drawFrom(new Point(2, 57), new Point(70, 57));//bottom
            exclude(new Point(37, 57), new Point(43, 57)); //zone entrance
            drawFrom(new Point(2, 47), new Point(70, 47));//top
            drawFrom(new Point(2, 47), new Point(2, 57)); //left
            drawFrom(new Point(70, 47), new Point(70, 57)); //right
            exclude(new Point(44, 47), new Point(50, 47));//green door
            exclude(new Point(22, 47), new Point(28, 47));//yellow door
            //keyhole
            drawFrom(new Point(48, 62), new Point(53, 62));
            drawFrom(new Point(48, 57), new Point(48, 62));
            drawFrom(new Point(53, 57), new Point(53, 62));
            exclude(new Point(49, 57), new Point(52, 57));
            //second zone
            drawFrom(new Point(44, 42), new Point(44, 47)); //green door entrance
            drawFrom(new Point(51, 42), new Point(51, 47)); //
            drawFrom(new Point(21, 42), new Point(21, 47)); //yellow door entrance
            drawFrom(new Point(28, 42), new Point(28, 47)); //
            drawFrom(new Point(10, 42), new Point(70, 42)); //bottom
            exclude(new Point(45, 42), new Point(50, 42)); //greendoor blank
            exclude(new Point(22, 42), new Point(27, 42)); //yellowdoor blank
            drawFrom(new Point(10, 28), new Point(10, 42)); //left
            drawFrom(new Point(70, 28), new Point(70, 42)); //right
            drawFrom(new Point(10, 28), new Point(70, 28)); //top
            exclude(new Point(45, 28), new Point(50, 28)); //exit
                //middle divide
            drawFrom(new Point(40, 28), new Point(40, 42));
            exclude(new Point(40, 32), new Point(40, 38));
            doors.Add(new Door(40, 32, true, -1, 3, false));
            doors.Add(new Door(40, 35, true, 1, 4, false));
            //third zone
            drawFrom(new Point(44, 13), new Point(44, 28));//left
            drawFrom(new Point(51, 13), new Point(51, 28));//right
            drawFrom(new Point(44, 13), new Point(51, 13));//top
            exclude(new Point(44, 16), new Point(44, 21)); //left entrance
            exclude(new Point(51, 16), new Point(51, 21)); //right entrance
            doors.Add(new Door(45, 28, false, -1, 3, false));
            doors.Add(new Door(48, 28, false, 1, 3, false));
                //left side of third zone
            drawFrom(new Point(39, 15), new Point(43, 15)); //top entrance
            drawFrom(new Point(39, 22), new Point(43, 22)); //bottom entrance
            drawFrom(new Point(39, 11), new Point(39, 26)); //entrance
            exclude(new Point(39, 16), new Point(39, 21)); //
            drawFrom(new Point(17, 11), new Point(39, 11)); //top  
            drawFrom(new Point(17, 26), new Point(39, 26)); //bottom
            drawFrom(new Point(17, 11), new Point(17, 26)); //exit
            exclude(new Point(17, 16), new Point(17, 21));//
                //right side of third zone
            drawFrom(new Point(51, 15), new Point(75, 15)); //top
            drawFrom(new Point(51, 22), new Point(75, 22)); //bottom
            drawFrom(new Point(75, 15), new Point(75, 22)); //right right
            exclude(new Point(64,15), new Point(69,15)); // entrance
            drawFrom(new Point(63, 8), new Point(63, 15)); //left
            drawFrom(new Point(70, 2), new Point(70, 15)); //left
                    //long horizontal strip
            drawFrom(new Point(7, 2), new Point(70, 2)); //top
            drawFrom(new Point(13, 8), new Point(63, 8)); //bottom
                    //near endzone
            drawFrom(new Point(7, 2), new Point(7, 22));
            drawFrom(new Point(13, 8), new Point(13, 15));
            drawFrom(new Point(13, 15), new Point(17, 15));
            drawFrom(new Point(7, 22), new Point(17, 22));
            doors.Add(new Door(8, 15, false, 1, 5, false));
            doors.Add(new Door(8, 8, false, 1, 5, false));
            exit = new Exit(8, 9, 5, 6);
            #endregion      
            //all keys and doors
            //
            #region doors and keys
            doors.Add(new Door(22, 47, false, -1, 3, true)); //gold door 1 (left
            keys.Add(new Key(new PointF(5.0f, 51f), Stat.Gold, doors));
            doors.Add(new Door(25, 47, false, 1, 3, true));
            keys.Add(new Key(new PointF(5.0f, 51f), Stat.Gold, doors));
            doors.Add(new Door(49, 57, false, -1, 2, true)); //gold door 2 (bottom
            keys.Add(new Key(new PointF(5.0f, 51f), Stat.Gold, doors));
            doors.Add(new Door(51, 57, false, 1, 2, true));
            keys.Add(new Key(new PointF(5.0f, 51f), Stat.Gold, doors));
 
            doors.Add(new Door(45, 47, false, -1, 3, true)); //green door
            keys.Add(new Key(new PointF(50.2f, 58.6f), Stat.Green, doors));
            doors.Add(new Door(48, 47, false, 1, 3, true));
            keys.Add(new Key(new PointF(50.2f, 58.6f), Stat.Green, doors));
 
            doors.Add(new Door(44, 16, true, -1, 3, true)); //red door left
            keys.Add(new Key(new PointF(67f, 34f), Stat.Red, doors));
            doors.Add(new Door(44, 19, true, 1, 3, true));
            keys.Add(new Key(new PointF(67f, 34f), Stat.Red, doors));
 
            doors.Add(new Door(51, 16, true, -1, 3, true)); //red door right
            keys.Add(new Key(new PointF(67f, 34f), Stat.Red, doors));
            doors.Add(new Door(51, 19, true, 1, 3, true));
            keys.Add(new Key(new PointF(67f, 34f), Stat.Red, doors));
 
            doors.Add(new Door(63, 3, true, 1, 5, true)); //blue door
            keys.Add(new Key(new PointF(72.5f, 17.5f), Stat.Blue, doors));
            #endregion
 
            //first zone
            enemyPoints.Add(new List<Point> { new Point(5, 53), new Point(65, 53), new Point(65, 50), new Point(5, 50) });
            enemies.Add(new Enemy(enemyPoints[enemyPoints.Count - 1], 45, 0, 16));
            enemyPoints.Add(new List<Point> {new Point(65, 50), new Point(5, 50), new Point(5, 53), new Point(65, 53)});
            enemies.Add(new Enemy(enemyPoints[enemyPoints.Count - 1], 45, 0, 16));
            //second zone left
            enemyPoints.Add(new List<Point> { new Point(12, 38), new Point(37, 38), new Point(37, 31), new Point(12, 31)});
            enemies.Add(new Enemy(enemyPoints[enemyPoints.Count - 1], 25, 4, 12));
            enemyPoints.Add(new List<Point> { new Point(37, 31), new Point(12, 31), new Point(12, 38), new Point(37, 38), });
            enemies.Add(new Enemy(enemyPoints[enemyPoints.Count - 1], 25, 4, 12));
            //second zone right
            enemyPoints.Add(new List<Point> { new Point(43, 38), new Point(66, 38), new Point(66, 31), new Point(43, 31) });
            enemies.Add(new Enemy(enemyPoints[enemyPoints.Count - 1], 35, 4, 10));
            //third zone
            enemyPoints.Add(new List<Point> { new Point(18, 22), new Point(37, 22), new Point(37, 14), new Point(18, 14) });
            enemies.Add(new Enemy(enemyPoints[enemyPoints.Count - 1], 30, 6, 7));
            enemyPoints.Add(new List<Point> { new Point(37, 14), new Point(18, 14), new Point(18, 22), new Point(37, 22) });
            enemies.Add(new Enemy(enemyPoints[enemyPoints.Count - 1], 30, 6, 7));
        }

        //LEVEL 4 ****
        public void Level_4()
        {
            timeLimit = (int)(45 / Stat.difficulty); 
            time = timeLimit;

            //Initialize player *
            player = new Player(playerColor, new Point(6 * Stat.TileSize, 58 * Stat.TileSize));


            //start
            drawFrom(new Point(1, 62), new Point(12, 62));  //bottom part   (also bottom of last hall)
            drawFrom(new Point(1, 55), new Point(1, 61)); //left wall
            drawFrom(new Point(12, 55), new Point(12, 61));  //right wall
            drawFrom(new Point(1, 55), new Point(12, 55)); //top part
            exclude(new Point(3, 55), new Point(10, 55)); // opening in top
            doors.Add(new Door(3, 55, false, -1, 8, false));

            //start strip
            //vertical
            drawFrom(new Point(2, 16), new Point(2, 54));
            drawFrom(new Point(11, 16), new Point(11, 54));
            exclude(new Point(11, 24), new Point(11, 29));
            drawFrom(new Point(2, 16), new Point(11, 16));
            //horizontal
            drawFrom(new Point(11, 23), new Point(26, 23));
            drawFrom(new Point(11, 30), new Point(26, 30));
            doors.Add(new Door(26, 24, true, -1, 6, false));




            //middle zone
            //vert left
            drawFrom(new Point(26, 18), new Point(26, 35));
            exclude(new Point(26, 24), new Point(26, 29));
            //horizontal
            drawFrom(new Point(26, 18), new Point(58, 18));
            drawFrom(new Point(26, 36), new Point(58, 36));
            //vert right
            drawFrom(new Point(58, 18), new Point(58, 35));
            exclude(new Point(58, 24), new Point(58, 29));
            //middle block
            drawFrom(new Point(34, 26), new Point(49, 26));
            drawFrom(new Point(34, 27), new Point(49, 27));
            exclude(new Point(40, 26), new Point(43, 26));
            exclude(new Point(40, 27), new Point(43, 27));


            //hallway
            //middle zone zone exit
            drawFrom(new Point(58, 23), new Point(63, 23));
            drawFrom(new Point(58, 30), new Point(63, 30));
            //vert left
            drawFrom(new Point(63, 12), new Point(63, 44));
            exclude(new Point(63, 24), new Point(63, 29));
            //vert right
            drawFrom(new Point(73, 5), new Point(73, 55));
            exclude(new Point(73, 24), new Point(73, 29));
            //key hole
            drawFrom(new Point(73, 23), new Point(79, 23));
            drawFrom(new Point(73, 30), new Point(79, 30));
            drawFrom(new Point(79, 23), new Point(79, 30));

            //bottom zone
            //entrance
            drawFrom(new Point(58, 44), new Point(63, 44));
            drawFrom(new Point(58, 55), new Point(73, 55));
            //horizontal
            drawFrom(new Point(20, 38), new Point(58, 38));
            drawFrom(new Point(20, 62), new Point(58, 62));
            //vert right
            drawFrom(new Point(58, 38), new Point(58, 62));
            exclude(new Point(58, 45), new Point(58, 54));
            //vert left
            drawFrom(new Point(20, 38), new Point(20, 62));
            exclude(new Point(20, 45), new Point(20, 54));
            //pillar 1
            drawFrom(new Point(32, 38), new Point(32, 62));
            exclude(new Point(32, 45), new Point(32, 50));
            //pillar 2
            drawFrom(new Point(45, 38), new Point(45, 62));
            exclude(new Point(45, 50), new Point(45, 55));
            //key hole
            //key hole Left
            drawFrom(new Point(14, 45), new Point(14, 55));
            //horiztonal
            drawFrom(new Point(14, 45), new Point(20, 45));
            drawFrom(new Point(14, 55), new Point(20, 55));


            //top zone
            //entrance
            drawFrom(new Point(58, 12), new Point(63, 12));
            drawFrom(new Point(58, 5), new Point(73, 5));

            //vert right
            drawFrom(new Point(58, 2), new Point(58, 15));
            exclude(new Point(58, 6), new Point(58, 11));
            //horizontal
            drawFrom(new Point(24, 2), new Point(58, 2));
            drawFrom(new Point(24, 15), new Point(58, 15));
            //vert left
            drawFrom(new Point(24, 2), new Point(24, 15));
            exclude(new Point(24, 6), new Point(24, 11));

            //endzone
            //vert left
            drawFrom(new Point(18, 5), new Point(18, 12));
            //horizontal
            drawFrom(new Point(18, 5), new Point(24, 5));
            drawFrom(new Point(18, 12), new Point(24, 12));


            exit = new Exit(18, 6, 7, 6);

            //all of the keys
            //

            //GOLD KEY
            doors.Add(new Door(32, 45, true, 1, 6, true));//pillar left
            keys.Add(new Key(new PointF(75.50f, 25.5f), Stat.Gold, doors));

            //BLUE KEY
            doors.Add(new Door(45, 50, true, -1, 6, true));//pillar right
            keys.Add(new Key(new PointF(6.0f, 18.5f), Stat.Blue, doors));

            //GREEN KEY
            doors.Add(new Door(58, 6, true, -1, 3, true)); //top zone entrance
            keys.Add(new Key(new PointF(17.5f, 49.0f), Stat.Green, doors));
            doors.Add(new Door(58, 9, true, 1, 3, true));
            keys.Add(new Key(new PointF(17.5f, 49.0f), Stat.Green, doors));

            //RED KEY
            doors.Add(new Door(58, 45, true, -1, 5, true)); //bottom zone entrance
            keys.Add(new Key(new PointF(41.5f, 25.5f), Stat.Red, doors));
            doors.Add(new Door(58, 50, true, 1, 5, true));
            keys.Add(new Key(new PointF(41.5f, 25.5f), Stat.Red, doors));

            //all of the enemies
            //

            //enemies at bottom
            enemies.Add(new Enemy(new Point(38, 59), new Point(38, 41), 20, 0, 8));
            enemies.Add(new Enemy(new Point(51, 41), new Point(51, 59), 20, 0, 8));
            enemies.Add(new Enemy(new Point(26, 41), new Point(26, 59), 20, 0, 8));

            //enemies in middle
            enemyPoints.Add(new List<Point> { new Point(29, 21), new Point(54, 21), new Point(54, 31), new Point(29, 31) });
            enemies.Add(new Enemy(enemyPoints[enemyPoints.Count - 1], 20, 6, 16));
            enemyPoints.Add(new List<Point> { new Point(54, 31), new Point(29, 31), new Point(29, 21), new Point(54, 21) });
            enemies.Add(new Enemy(enemyPoints[enemyPoints.Count - 1], 20, 6, 16));

            //enemy in first corridor
            enemyPoints.Add(new List<Point> { new Point(6, 42), new Point(6, 26), new Point(16, 26), new Point(6, 26) });
            enemies.Add(new Enemy(enemyPoints[enemyPoints.Count - 1], 20, 6, 16));

            //enemy in far right corridor
            enemies.Add(new Enemy(new Point(68, 51), new Point(68, 7), 20, 6, 8));

            //enemy in top
            enemyPoints.Add(new List<Point> { new Point(55, 4), new Point(26, 4), new Point(26, 11), new Point(55, 11) });
            enemies.Add(new Enemy(enemyPoints[enemyPoints.Count - 1], 20, 6, 16));
        }



    }
}
