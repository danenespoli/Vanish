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
    public partial class IntroScreen : Form
    {
        public IntroScreen()
        {
            InitializeComponent();
            pbCanvas.BackgroundImage = Properties.Resources.VanishScreens_01;
        }

        int selected = 1;
        bool instr = false;
        bool play = false;
        int last;

        private void IntroScreen_KeyDown(object sender, KeyEventArgs e)
        {
            if (!instr && !play)
            {
                if (e.KeyData == Keys.Down)
                {
                    selected++;
                    if (selected >= 4)
                    {
                        selected = 1;
                    }
                }
                if (e.KeyData == Keys.Up)
                {
                    selected--;
                    if (selected <= 0)
                    {
                        selected = 3;
                    }
                }
                switch (selected)
                {
                    case 1:
                        pbCanvas.BackgroundImage = Properties.Resources.VanishScreens_01;
                        break;
                    case 2:
                        pbCanvas.BackgroundImage = Properties.Resources.VanishScreens_02;
                        break;
                    case 3:
                        pbCanvas.BackgroundImage = Properties.Resources.VanishScreens_03;
                        break;
                }
            }
            if (play)
            {
                if (selected != 0)
                {
                    if (e.KeyData == Keys.Left)
                    {
                        selected--;
                        if (selected == 0)
                        {
                            selected = 4;
                        }
                    }
                    if (e.KeyData == Keys.Right)
                    {
                        selected++;
                        if (selected == 5)
                        {
                            selected = 1;
                        }
                    }
                }
                if (e.KeyData == Keys.Up || e.KeyData == Keys.Down)
                {
                    if (selected == 0)
                    {
                        selected = last;
                    }
                    else
                    {
                        last = selected;
                        selected = 0;
                    }
                }
                switch (selected)
                {
                    case 0:
                        pbCanvas.BackgroundImage = Properties.Resources.VanishScreens_06;
                        break;
                    case 1:
                        pbCanvas.BackgroundImage = Properties.Resources.VanishScreens_07;
                        break;
                    case 2:
                        pbCanvas.BackgroundImage = Properties.Resources.VanishScreens_08;
                        break;
                    case 3:
                        pbCanvas.BackgroundImage = Properties.Resources.VanishScreens_09;
                        break;
                    case 4:
                        pbCanvas.BackgroundImage = Properties.Resources.VanishScreens_10;
                        break;
                }
            }

            if (e.KeyData == Keys.Enter)
            {
                select(selected);
            }
        }




        public void select(int s)
        {
            switch (s)
            {
                case 0:
                    pbCanvas.BackgroundImage = Properties.Resources.VanishScreens_01;
                    selected = 1;
                    play = false;
                    break;
                case 1:
                    if (play)
                    {
                        Vanish v = new Vanish(1);
                        v.Show();
                    }
                    else
                    {
                        pbCanvas.BackgroundImage = Properties.Resources.VanishScreens_07;
                        play = true;
                    }
                    break;
                case 2:
                    if (play)
                    {
                        Vanish v = new Vanish(2);
                        v.Show();
                    }
                    else
                    {
                        if (!instr)
                        {
                            pbCanvas.BackgroundImage = Properties.Resources.VanishScreens_04;
                            instr = true;
                        }
                        else
                        {
                            pbCanvas.BackgroundImage = Properties.Resources.VanishScreens_02;
                            instr = false;
                        }
                    }
                    break;
                case 3:
                    if (play)
                    {
                        Vanish v = new Vanish(3);
                        v.Show();
                    }
                    else
                    {
                        Application.Exit();
                    }
                    break;
                case 4:
                    if (play)
                    {
                        Vanish v = new Vanish(4);
                        v.Show();
                    }
                    break;
            }
        }




    }
}
