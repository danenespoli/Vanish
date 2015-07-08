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
    public partial class Vanish : Form
    {
        public Vanish(int l)
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized; 
            //Create new world, pick level
            level = l;
        }

        Statics Stat = new Statics();
        
        World w;
        int level;

        private void Vanish_Load(object sender, EventArgs e)
        {
            w = new World(level);   //create world based on level selected

            pbCanvas.Location = new Point((int)Stat.pbCanvasLocX, (int)Stat.pbCanvasLocY);
            pbCanvas.Width = (int)Stat.pbCanvasWidth;   //place pbCanvas in the middle of the form
            pbCanvas.Height = (int)Stat.pbCanvasHeight;
            this.MaximumSize = new Size((int)Stat.resolution.Width + 20, (int)Stat.resolution.Height + 40);
            this.MinimumSize = this.MaximumSize;
        }


        

        private void pbCanvas_Paint(object sender, PaintEventArgs e)
        {
            w.refresh(e);

            //END GAME CHECK
            if (w.gameWonDone)
            {
                this.Close();   //close if game won
            }
            if (w.gameLostDone)
            {
                w = new World(level);   //reset if game lost
            }
        }


        private void tmrMove_Tick(object sender, EventArgs e)
        {
            pbCanvas.Refresh();
        }




        private void Vanish_KeyDown(object sender, KeyEventArgs e)
        {
            w.player.KeyDown(e);

            if (e.KeyData == Keys.Space)
            {
                if (tmrMove.Enabled)
                {
                    tmrMove.Enabled = false;
                }
                else
                {
                    tmrMove.Enabled = true;
                }
            }
        }

        private void Vanish_KeyUp(object sender, KeyEventArgs e)
        {
            w.player.KeyUp(e);
        }


    }
}
