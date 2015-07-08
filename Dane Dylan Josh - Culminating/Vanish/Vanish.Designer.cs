namespace Vanish
{
    partial class Vanish
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Vanish));
            this.pbCanvas = new System.Windows.Forms.PictureBox();
            this.tmrMove = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pbCanvas)).BeginInit();
            this.SuspendLayout();
            // 
            // pbCanvas
            // 
            this.pbCanvas.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pbCanvas.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pbCanvas.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbCanvas.Location = new System.Drawing.Point(0, 0);
            this.pbCanvas.Name = "pbCanvas";
            this.pbCanvas.Size = new System.Drawing.Size(1280, 1024);
            this.pbCanvas.TabIndex = 0;
            this.pbCanvas.TabStop = false;
            this.pbCanvas.Paint += new System.Windows.Forms.PaintEventHandler(this.pbCanvas_Paint);
            // 
            // tmrMove
            // 
            this.tmrMove.Enabled = true;
            this.tmrMove.Interval = 1;
            this.tmrMove.Tick += new System.EventHandler(this.tmrMove_Tick);
            // 
            // Vanish
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(1264, 986);
            this.Controls.Add(this.pbCanvas);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Vanish";
            this.Text = "Vanish";
            this.Load += new System.EventHandler(this.Vanish_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Vanish_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Vanish_KeyUp);
            ((System.ComponentModel.ISupportInitialize)(this.pbCanvas)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pbCanvas;
        private System.Windows.Forms.Timer tmrMove;

    }
}

