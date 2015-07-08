using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
 
namespace Vanish
{
    class Player
    {
        Statics Stat = new Statics();

        PointF PrevLoc;
        public Color PlayerColorBrush;
        public int transparency = 255;
        public double xPos = 50.0f, yPos = 50.0f; //position
        public static double speedR, speedS; //rush speed
 
        public bool toggle = false;
        public bool toggleV = false;
 
        public double Speed;
 
        public int size;
 
        public double pulse = 0; // pulse size for rush
        public double pulseLim; //pulse limit for rush
        public double pulseChange;
        public double pulseDiv;
 
       
 
        public bool up = false, down = false, left = false, right = false, moving = false; //movement booleans
        public bool upC = false, downC = false, leftC = false, rightC = false, movingC = false; //movement booleans for use in collision (separate bools are used
                                                                                                //so that if you are not blocked suddenly you can continue in that direction
        public String mode;                                                                     //without having to repress the directional key
        public String lastM;
 
        public enum m
        {
            rush,
            stealth,
            vanish
        }

        public Player(Color PlayerColor, Point Position)
        {
            PlayerColorBrush = PlayerColor;
            xPos = Position.X;
            yPos = Position.Y;
            mode = m.rush.ToString();

            speedR = 0.25 * Stat.TileSize;
            speedS = 0.125 * Stat.TileSize;
            Speed = 0.25 * Stat.TileSize;

            size = Stat.TileSize * 2;

            pulseLim = 15 * Stat.TileSize;
            pulseChange = 0.375 * Stat.TileSize;
            pulseDiv = 0.25 * Stat.TileSize;
        }
 
        public void Draw(PaintEventArgs e)
        {
            e.Graphics.FillEllipse(new SolidBrush(Color.FromArgb((int)(60 - (pulse / pulseDiv)), Stat.Black)), Convert.ToSingle((xPos - ((pulse - size) / 2))), Convert.ToSingle((yPos - ((pulse - size) / 2))), Convert.ToSingle(pulse), Convert.ToSingle(pulse));
            e.Graphics.FillEllipse(new SolidBrush(Color.FromArgb(transparency, PlayerColorBrush)), (int)xPos, (int)yPos, size, size);
        }
 
        public void PlayerMove(World w)
        {
 
            PrevLoc = new PointF(Convert.ToSingle(xPos), Convert.ToSingle(yPos));
 
            if (up && upC)
            {
                yPos -= Speed;
            }
            if (down && downC)
            {
                yPos += Speed;
            }
            if (left && leftC)
            {
                xPos -= Speed;
            }
            if (right && rightC)
            {
                xPos += Speed;
            }
 
            if (xPos != PrevLoc.X || yPos != PrevLoc.Y)
            {
                moving = true;
            }
            else
            {
                moving = false;
            }
 
            if ((mode == "rush" || mode == "stealth") && moving && !w.gameLost)
            {
                if (pulse >= 0 && pulse <= pulseLim)
                {
                    pulse += pulseChange;
                }
                if (pulse >= pulseLim)
                {
                    pulse = 0;
                }
            }

            if (!moving && !w.gameLost)
            {
                pulse = 0;
            }
        }
 
        private void Vanish_MODE()
        {
            transparency = 100;
            mode = Player.m.vanish.ToString();
            Speed = 0;
            pulse = 0;
        }
 
        public void KeyDown(KeyEventArgs e)
        {
            if (e.KeyData == Keys.A && !toggle)
            {
                toggleRushStealth();
            }
 
            if (e.KeyData == Keys.S && !toggleV)
            {
                toggleVanishModes();
            }
 
            if (e.KeyData == Keys.Up)
            {
                up = true;
                upC = true;
            }
            if (e.KeyData == Keys.Down)
            {
                down = true;
                downC = true;
            }
            if (e.KeyData == Keys.Left)
            {
                left = true;
                leftC = true;
            }
            if (e.KeyData == Keys.Right)
            {
                right = true;
                rightC = true;
            }
        }
        public void KeyUp(KeyEventArgs e)
        {
            if (toggle)
                toggle = false;
 
            if (toggleV)
                toggleV = false;
 
            if (e.KeyData == Keys.Up)
            {
                up = false;
            }
            if (e.KeyData == Keys.Down)
            {
                down = false;
            }
            if (e.KeyData == Keys.Left)
            {
                left = false;
            }
            if (e.KeyData == Keys.Right)
            {
                right = false;
            }
        }
 
        public void toggleRushStealth()
        {
            if (mode == Player.m.rush.ToString())
            {
                transparency = 200;
                mode = Player.m.stealth.ToString();
                Speed = Player.speedS;
                pulse = 0;
                pulseLim = 7.5 * Stat.TileSize;
                pulseChange = 0.125 * Stat.TileSize;
                pulseDiv = 0.125 * Stat.TileSize;
            }
            else if (mode == Player.m.stealth.ToString() || mode == Player.m.vanish.ToString())
            {
                transparency = 255;
                mode = Player.m.rush.ToString();
                Speed = Player.speedR;
                pulse = 0;
                pulseLim = 15 * Stat.TileSize;
                pulseChange = 0.375 * Stat.TileSize;
                pulseDiv = 0.25 * Stat.TileSize;
            }
            toggle = true;
        }
 
        public void toggleVanishModes()
        {
            if (mode != "vanish")
            {
                lastM = mode;
                Vanish_MODE();
            }
            else if (mode == "vanish")
            {
                if (lastM == Player.m.stealth.ToString())
                {
                    mode = "stealth";
                    transparency = 200;
                    mode = Player.m.stealth.ToString();
                    Speed = Player.speedS;
                    pulse = 0;
                    pulseLim = 7.5 * Stat.TileSize;
                    pulseChange = 0.125 * Stat.TileSize;
                    pulseDiv = 0.125 * Stat.TileSize;
                }
                else if (lastM == Player.m.rush.ToString())
                {
                    mode = "rush";
                    transparency = 255;
                    mode = Player.m.rush.ToString();
                    Speed = Player.speedR;
                    pulse = 0;
                    pulseLim = 15 * Stat.TileSize;
                    pulseChange = 0.375 * Stat.TileSize;
                    pulseDiv = 0.25 * Stat.TileSize;
                }
            }
            toggleV = true;
 
        }
 
    }
}
