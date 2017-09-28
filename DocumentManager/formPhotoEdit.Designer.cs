namespace DocumentManager
{
    partial class formPhotoEdit
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(formPhotoEdit));
            this.skHost1 = new FyfeSoftware.Sketchy.WinForms.SketchyCanvasHost();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonReset = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonRotateLeft = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonRotateRight = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonCrop = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonTransform = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonBrightness = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonAcceptCrop = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonCancelCrop = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonAcceptTransform = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonCancelTransform = new System.Windows.Forms.ToolStripButton();
            this.lblZm = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tbZoom = new System.Windows.Forms.TrackBar();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbZoom)).BeginInit();
            this.SuspendLayout();
            // 
            // skHost1
            // 
            this.skHost1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.skHost1.AutoScroll = true;
            this.skHost1.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.skHost1.Canvas = null;
            this.skHost1.Location = new System.Drawing.Point(12, 54);
            this.skHost1.Name = "skHost1";
            this.skHost1.Size = new System.Drawing.Size(685, 347);
            this.skHost1.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(622, 407);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "&Close";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.Location = new System.Drawing.Point(541, 407);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 3;
            this.button2.Text = "&Done";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 412);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(220, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Move or resize the rectangular crop indicator.";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonReset,
            this.toolStripSeparator1,
            this.toolStripButtonRotateLeft,
            this.toolStripButtonRotateRight,
            this.toolStripSeparator2,
            this.toolStripButtonCrop,
            this.toolStripButtonTransform,
            this.toolStripButtonBrightness,
            this.toolStripButtonAcceptCrop,
            this.toolStripButtonCancelCrop,
            this.toolStripButtonAcceptTransform,
            this.toolStripButtonCancelTransform});
            this.toolStrip1.Location = new System.Drawing.Point(12, 20);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(475, 25);
            this.toolStrip1.TabIndex = 5;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButtonReset
            // 
            this.toolStripButtonReset.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonReset.Image")));
            this.toolStripButtonReset.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonReset.Name = "toolStripButtonReset";
            this.toolStripButtonReset.Size = new System.Drawing.Size(55, 22);
            this.toolStripButtonReset.Text = "Rese&t";
            this.toolStripButtonReset.Click += new System.EventHandler(this.toolStripButton4_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonRotateLeft
            // 
            this.toolStripButtonRotateLeft.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonRotateLeft.Image")));
            this.toolStripButtonRotateLeft.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonRotateLeft.Name = "toolStripButtonRotateLeft";
            this.toolStripButtonRotateLeft.Size = new System.Drawing.Size(84, 22);
            this.toolStripButtonRotateLeft.Text = "Rotate &Left";
            this.toolStripButtonRotateLeft.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolStripButtonRotateRight
            // 
            this.toolStripButtonRotateRight.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonRotateRight.Image")));
            this.toolStripButtonRotateRight.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonRotateRight.Name = "toolStripButtonRotateRight";
            this.toolStripButtonRotateRight.Size = new System.Drawing.Size(92, 22);
            this.toolStripButtonRotateRight.Text = "Rotate &Right";
            this.toolStripButtonRotateRight.Click += new System.EventHandler(this.toolStripButton2_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonCrop
            // 
            this.toolStripButtonCrop.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonCrop.Image")));
            this.toolStripButtonCrop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonCrop.Name = "toolStripButtonCrop";
            this.toolStripButtonCrop.Size = new System.Drawing.Size(53, 22);
            this.toolStripButtonCrop.Text = "C&rop";
            this.toolStripButtonCrop.Click += new System.EventHandler(this.toolStripButton3_Click);
            // 
            // toolStripButtonTransform
            // 
            this.toolStripButtonTransform.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonTransform.Image")));
            this.toolStripButtonTransform.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonTransform.Name = "toolStripButtonTransform";
            this.toolStripButtonTransform.Size = new System.Drawing.Size(63, 22);
            this.toolStripButtonTransform.Text = "&Flatten";
            this.toolStripButtonTransform.Click += new System.EventHandler(this.toolStripButton7_Click);
            // 
            // toolStripButtonBrightness
            // 
            this.toolStripButtonBrightness.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonBrightness.Image")));
            this.toolStripButtonBrightness.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonBrightness.Name = "toolStripButtonBrightness";
            this.toolStripButtonBrightness.Size = new System.Drawing.Size(113, 22);
            this.toolStripButtonBrightness.Text = "Lig&ht Correction";
            this.toolStripButtonBrightness.Click += new System.EventHandler(this.toolStripButtonBrightness_Click);
            // 
            // toolStripButtonAcceptCrop
            // 
            this.toolStripButtonAcceptCrop.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonAcceptCrop.Image")));
            this.toolStripButtonAcceptCrop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonAcceptCrop.Name = "toolStripButtonAcceptCrop";
            this.toolStripButtonAcceptCrop.Size = new System.Drawing.Size(93, 22);
            this.toolStripButtonAcceptCrop.Text = "&Accept Crop";
            this.toolStripButtonAcceptCrop.Visible = false;
            this.toolStripButtonAcceptCrop.Click += new System.EventHandler(this.toolStripButton5_Click);
            // 
            // toolStripButtonCancelCrop
            // 
            this.toolStripButtonCancelCrop.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonCancelCrop.Image")));
            this.toolStripButtonCancelCrop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonCancelCrop.Name = "toolStripButtonCancelCrop";
            this.toolStripButtonCancelCrop.Size = new System.Drawing.Size(92, 22);
            this.toolStripButtonCancelCrop.Text = "Ca&ncel Crop";
            this.toolStripButtonCancelCrop.Visible = false;
            this.toolStripButtonCancelCrop.Click += new System.EventHandler(this.toolStripButton6_Click);
            // 
            // toolStripButtonAcceptTransform
            // 
            this.toolStripButtonAcceptTransform.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonAcceptTransform.Image")));
            this.toolStripButtonAcceptTransform.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonAcceptTransform.Name = "toolStripButtonAcceptTransform";
            this.toolStripButtonAcceptTransform.Size = new System.Drawing.Size(103, 22);
            this.toolStripButtonAcceptTransform.Text = "&Accept Flatten";
            this.toolStripButtonAcceptTransform.Visible = false;
            this.toolStripButtonAcceptTransform.Click += new System.EventHandler(this.toolStripButtonAcceptTransform_Click);
            // 
            // toolStripButtonCancelTransform
            // 
            this.toolStripButtonCancelTransform.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonCancelTransform.Image")));
            this.toolStripButtonCancelTransform.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonCancelTransform.Name = "toolStripButtonCancelTransform";
            this.toolStripButtonCancelTransform.Size = new System.Drawing.Size(102, 22);
            this.toolStripButtonCancelTransform.Text = "Ca&ncel Flatten";
            this.toolStripButtonCancelTransform.Visible = false;
            this.toolStripButtonCancelTransform.Click += new System.EventHandler(this.toolStripButtonCancelTransform_Click);
            // 
            // lblZm
            // 
            this.lblZm.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblZm.AutoSize = true;
            this.lblZm.BackColor = System.Drawing.Color.Transparent;
            this.lblZm.Location = new System.Drawing.Point(500, 412);
            this.lblZm.Name = "lblZm";
            this.lblZm.Size = new System.Drawing.Size(33, 13);
            this.lblZm.TabIndex = 21;
            this.lblZm.Text = "100%";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label2.Location = new System.Drawing.Point(244, 412);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 13);
            this.label2.TabIndex = 20;
            this.label2.Text = "Zoom";
            // 
            // tbZoom
            // 
            this.tbZoom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.tbZoom.AutoSize = false;
            this.tbZoom.LargeChange = 1;
            this.tbZoom.Location = new System.Drawing.Point(281, 407);
            this.tbZoom.Margin = new System.Windows.Forms.Padding(0);
            this.tbZoom.Minimum = 1;
            this.tbZoom.Name = "tbZoom";
            this.tbZoom.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tbZoom.Size = new System.Drawing.Size(216, 22);
            this.tbZoom.TabIndex = 2;
            this.tbZoom.Value = 5;
            this.tbZoom.Scroll += new System.EventHandler(this.tbZoom_Scroll);
            // 
            // formPhotoEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(724, 439);
            this.ControlBox = false;
            this.Controls.Add(this.lblZm);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbZoom);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.skHost1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "formPhotoEdit";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Document Edit";
            this.Load += new System.EventHandler(this.formPhotoEdit_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbZoom)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private FyfeSoftware.Sketchy.WinForms.SketchyCanvasHost skHost1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButtonRotateLeft;
        private System.Windows.Forms.ToolStripButton toolStripButtonRotateRight;
        private System.Windows.Forms.ToolStripButton toolStripButtonCrop;
        private System.Windows.Forms.Label lblZm;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TrackBar tbZoom;
        private System.Windows.Forms.ToolStripButton toolStripButtonReset;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton toolStripButtonAcceptCrop;
        private System.Windows.Forms.ToolStripButton toolStripButtonCancelCrop;
        private System.Windows.Forms.ToolStripButton toolStripButtonTransform;
        private System.Windows.Forms.ToolStripButton toolStripButtonAcceptTransform;
        private System.Windows.Forms.ToolStripButton toolStripButtonCancelTransform;
        private System.Windows.Forms.ToolStripButton toolStripButtonBrightness;
    }
}