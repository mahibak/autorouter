namespace Idlorio
{
    partial class MapView
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnCompute = new System.Windows.Forms.Button();
            this.txtMax = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtDesired = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtActual = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Location = new System.Drawing.Point(196, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1342, 1018);
            this.panel1.TabIndex = 1;
            // 
            // btnCompute
            // 
            this.btnCompute.Location = new System.Drawing.Point(3, 3);
            this.btnCompute.Name = "btnCompute";
            this.btnCompute.Size = new System.Drawing.Size(187, 23);
            this.btnCompute.TabIndex = 2;
            this.btnCompute.Text = "Compute";
            this.btnCompute.UseVisualStyleBackColor = true;
            this.btnCompute.Click += new System.EventHandler(this.btnCompute_Click);
            // 
            // txtMax
            // 
            this.txtMax.Location = new System.Drawing.Point(3, 51);
            this.txtMax.Name = "txtMax";
            this.txtMax.Size = new System.Drawing.Size(187, 20);
            this.txtMax.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Maximum";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 74);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Desired";
            // 
            // txtDesired
            // 
            this.txtDesired.Location = new System.Drawing.Point(3, 90);
            this.txtDesired.Name = "txtDesired";
            this.txtDesired.Size = new System.Drawing.Size(187, 20);
            this.txtDesired.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 113);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Actual";
            // 
            // txtActual
            // 
            this.txtActual.Location = new System.Drawing.Point(3, 129);
            this.txtActual.Name = "txtActual";
            this.txtActual.Size = new System.Drawing.Size(187, 20);
            this.txtActual.TabIndex = 8;
            // 
            // MapView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtActual);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtDesired);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtMax);
            this.Controls.Add(this.btnCompute);
            this.Controls.Add(this.panel1);
            this.Name = "MapView";
            this.Size = new System.Drawing.Size(1538, 1018);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnCompute;
        private System.Windows.Forms.TextBox txtMax;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtDesired;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtActual;
    }
}
