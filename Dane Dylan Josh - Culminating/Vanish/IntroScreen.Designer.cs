namespace Vanish
{
    partial class IntroScreen
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IntroScreen));
            this.pbCanvas = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbCanvas)).BeginInit();
            this.SuspendLayout();
            // 
            // pbCanvas
            // 
            this.pbCanvas.BackColor = System.Drawing.Color.Transparent;
            this.pbCanvas.Location = new System.Drawing.Point(0, 0);
            this.pbCanvas.Name = "pbCanvas";
            this.pbCanvas.Size = new System.Drawing.Size(800, 600);
            this.pbCanvas.TabIndex = 0;
            this.pbCanvas.TabStop = false;
            // 
            // IntroScreen
            // 
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.Controls.Add(this.pbCanvas);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(816, 638);
            this.MinimumSize = new System.Drawing.Size(816, 638);
            this.Name = "IntroScreen";
            this.Text = "Vanish";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.IntroScreen_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.pbCanvas)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pbBlurred;
        private System.Windows.Forms.Timer tmrAnimate;
        private System.Windows.Forms.PictureBox pbCanvas;

    }
}