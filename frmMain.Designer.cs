namespace Idlorio
{
    partial class frmMain
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
            this.mapView1 = new Idlorio.MapView();
            this.SuspendLayout();
            // 
            // mapView1
            // 
            this.mapView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mapView1.Location = new System.Drawing.Point(0, 0);
            this.mapView1.Name = "mapView1";
            this.mapView1.Size = new System.Drawing.Size(2125, 1105);
            this.mapView1.TabIndex = 2;
            this.mapView1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.mapView1_MouseMove);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(2125, 1105);
            this.Controls.Add(this.mapView1);
            this.Name = "frmMain";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion
        private MapView mapView1;
    }
}

