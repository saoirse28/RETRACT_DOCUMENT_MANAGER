using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using FyfeSoftware.Sketchy.Core;
using FyfeSoftware.Sketchy.Core.Shapes;
using FyfeSoftware.Sketchy.Design;

using AForge;
using AForge.Imaging.Filters;
using ImageMagick;
using System.Data;

namespace DocumentManager
{
    public partial class formPhotoEdit : Form
    {
        List<AForge.Point> controlPoints = new List<AForge.Point>();
        public string fileName;

        MagickImage originalBmp;
        MagickImage cropBmp;
        MagickReadSettings magickSettingsEnchiper = new MagickReadSettings();
        MagickReadSettings magickSettingsNoEnchiper = new MagickReadSettings();
        public string encipherDecipher = "";

        public DesignerCanvas m_canvas;
        private float[] zooms = {
                                    0.1f,
                                    0.2f,
                                    0.4f,
                                    0.5f,
                                    0.75f,
                                    1.0f,
                                    1.25f,
                                    1.5f,
                                    1.75f,
                                    2.0f,
                                    3.0f
                                };


        private enum EBUTTONSTATE
        {
            CROPSTART,
            CROPEND,
            TRANSFORMSTART,
            TRANSFORMEND
        }

        private void SetButtons(EBUTTONSTATE a_ebuttonstate)
        {
            switch (a_ebuttonstate)
            {
                default:
                case EBUTTONSTATE.CROPSTART:
                    toolStrip1.Items["toolStripButtonReset"].Enabled = false;
                    toolStrip1.Items["toolStripButtonRotateLeft"].Enabled = false;
                    toolStrip1.Items["toolStripButtonRotateRight"].Enabled = false;
                    toolStrip1.Items["toolStripButtonCrop"].Enabled = false;
                    toolStrip1.Items["toolStripButtonTransform"].Enabled = false;
                    toolStrip1.Items["toolStripButtonBrightness"].Enabled = false;

                    toolStrip1.Items["toolStripButtonAcceptCrop"].Visible = true;
                    toolStrip1.Items["toolStripButtonCancelCrop"].Visible = true;
                    toolStrip1.Items["toolStripButtonAcceptTransform"].Visible = false;
                    toolStrip1.Items["toolStripButtonCancelTransform"].Visible = false;
                    button1.Enabled = false;
                    button2.Enabled = false;
                    break;
                case EBUTTONSTATE.CROPEND:
                    toolStrip1.Items["toolStripButtonReset"].Enabled = true;
                    toolStrip1.Items["toolStripButtonRotateLeft"].Enabled = true;
                    toolStrip1.Items["toolStripButtonRotateRight"].Enabled = true;
                    toolStrip1.Items["toolStripButtonCrop"].Enabled = true;
                    toolStrip1.Items["toolStripButtonTransform"].Enabled = true;
                    toolStrip1.Items["toolStripButtonBrightness"].Enabled = true;

                    toolStrip1.Items["toolStripButtonAcceptCrop"].Visible = false;
                    toolStrip1.Items["toolStripButtonCancelCrop"].Visible = false;
                    toolStrip1.Items["toolStripButtonAcceptTransform"].Visible = false;
                    toolStrip1.Items["toolStripButtonCancelTransform"].Visible = false;
                    button1.Enabled = true;
                    button2.Enabled = true;
                    break;
                case EBUTTONSTATE.TRANSFORMSTART:
                    toolStrip1.Items["toolStripButtonReset"].Enabled = false;
                    toolStrip1.Items["toolStripButtonRotateLeft"].Enabled = false;
                    toolStrip1.Items["toolStripButtonRotateRight"].Enabled = false;
                    toolStrip1.Items["toolStripButtonCrop"].Enabled = false;
                    toolStrip1.Items["toolStripButtonTransform"].Enabled = false;
                    toolStrip1.Items["toolStripButtonBrightness"].Enabled = false;

                    toolStrip1.Items["toolStripButtonAcceptCrop"].Visible = false;
                    toolStrip1.Items["toolStripButtonCancelCrop"].Visible = false;
                    toolStrip1.Items["toolStripButtonAcceptTransform"].Visible = true;
                    toolStrip1.Items["toolStripButtonCancelTransform"].Visible = true;
                    button1.Enabled = false;
                    button2.Enabled = false;
                    break;
                case EBUTTONSTATE.TRANSFORMEND:
                    toolStrip1.Items["toolStripButtonReset"].Enabled = true;
                    toolStrip1.Items["toolStripButtonRotateLeft"].Enabled = true;
                    toolStrip1.Items["toolStripButtonRotateRight"].Enabled = true;
                    toolStrip1.Items["toolStripButtonCrop"].Enabled = true;
                    toolStrip1.Items["toolStripButtonBrightness"].Enabled = true;

                    toolStrip1.Items["toolStripButtonAcceptCrop"].Visible = false;
                    toolStrip1.Items["toolStripButtonCancelCrop"].Visible = false;
                    toolStrip1.Items["toolStripButtonTransform"].Enabled = true;
                    toolStrip1.Items["toolStripButtonAcceptTransform"].Visible = false;
                    toolStrip1.Items["toolStripButtonCancelTransform"].Visible = false;
                    button1.Enabled = true;
                    button2.Enabled = true;
                    break;
            }
        }
        public formPhotoEdit()
        {
            InitializeComponent();
        }

        private void formPhotoEdit_Load(object sender, EventArgs e)
        {
            m_canvas = new DesignerCanvas();
            this.skHost1.Canvas = this.m_canvas;

            magickSettingsNoEnchiper.CompressionMethod = CompressionMethod.JPEG;
            magickSettingsNoEnchiper.Format = MagickFormat.Jpeg;

            magickSettingsEnchiper.CompressionMethod = CompressionMethod.LosslessJPEG;
            magickSettingsEnchiper.Format = MagickFormat.Png;

            byte[] imageBytes = File.ReadAllBytes(fileName);

            if (encipherDecipher != "")
            {
                originalBmp = new MagickImage(imageBytes, magickSettingsEnchiper);
                cropBmp = new MagickImage(imageBytes, magickSettingsEnchiper);

                originalBmp.Decipher(encipherDecipher);
                cropBmp.Decipher(encipherDecipher);
                
            }
            else
            {
                originalBmp = new MagickImage(imageBytes, magickSettingsNoEnchiper);
                cropBmp = new MagickImage(imageBytes, magickSettingsNoEnchiper);
            }
                this.m_canvas.Add(new BackgroundImageShape() { Image = cropBmp.ToBitmap() }, "Image");

        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(encipherDecipher != "")
            {
                cropBmp.Format = MagickFormat.Png;
                cropBmp.Encipher(encipherDecipher);
            }

            File.Delete(fileName);
            cropBmp.Write(fileName);

            DialogResult = DialogResult.OK;
            Close();
        }

        private void tbZoom_Scroll(object sender, EventArgs e)
        {
            lblZm.Text = String.Format("{0:##}%", zooms[tbZoom.Value] * 100);
            this.m_canvas.Zoom = zooms[tbZoom.Value];
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            this.m_canvas.Clear();
            cropBmp = new MagickImage(originalBmp);
            this.m_canvas.Add(new BackgroundImageShape() { Image = cropBmp.ToBitmap() }, "Image");
            SetButtons(EBUTTONSTATE.CROPEND);
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            IShape itm = m_canvas.FindShape("Crop");

            if (itm != null)
            { 
                PointF position = itm.Position;
                SizeF size = itm.Size;
                cropBmp.Crop((int)position.X, (int)position.Y, (int)size.Width, (int)size.Height);
                this.m_canvas.Clear();
                this.m_canvas.Add(new BackgroundImageShape() { Image = cropBmp.ToBitmap() }, "Image");
                SetButtons(EBUTTONSTATE.CROPEND);
            }
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            this.m_canvas.Clear();
            this.m_canvas.Add(new BackgroundImageShape() { Image = cropBmp.ToBitmap() }, "Image");
            SetButtons(EBUTTONSTATE.CROPEND);
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            m_canvas.Clear();
            cropBmp.Rotate(270);
            this.m_canvas.Add(new BackgroundImageShape() { Image = cropBmp.ToBitmap() }, "Image");
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            m_canvas.Clear();
            cropBmp.Rotate(90);
            this.m_canvas.Add(new BackgroundImageShape() { Image = cropBmp.ToBitmap() }, "Image");
        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            if (m_canvas.FindShape("TL") != null)
            {
                return;
            }

            this.m_canvas.Add(new CornerAnchorShape(new PointF(0, 0), "TL"), "TL");
            this.m_canvas.Add(new CornerAnchorShape(new PointF(cropBmp.Width, 0), "TR"), "TR");
            this.m_canvas.Add(new CornerAnchorShape(new PointF(0, cropBmp.Height), "BL"), "BL");
            this.m_canvas.Add(new CornerAnchorShape(new PointF(cropBmp.Width, cropBmp.Height), "BR"), "BR");

            // Join the canvas stuff
            this.m_canvas.Add(new ConnectionLineShape()
            {
                Source = this.m_canvas.FindShape("TL"),
                Target = this.m_canvas.FindShape("TR"),
                OutlineWidth = 3,
                OutlineColor = Color.OrangeRed,
                OutlineStyle = System.Drawing.Drawing2D.DashStyle.Dot
            }, "TLTR");
            this.m_canvas.Add(new ConnectionLineShape()
            {
                Source = this.m_canvas.FindShape("TR"),
                Target = this.m_canvas.FindShape("BR"),
                OutlineWidth = 3,
                OutlineColor = Color.OrangeRed,
                OutlineStyle = System.Drawing.Drawing2D.DashStyle.Dot
            }, "TRBR");
            this.m_canvas.Add(new ConnectionLineShape()
            {
                Source = this.m_canvas.FindShape("BL"),
                Target = this.m_canvas.FindShape("BR"),
                OutlineWidth = 3,
                OutlineColor = Color.OrangeRed,
                OutlineStyle = System.Drawing.Drawing2D.DashStyle.Dot
            }, "BLBR");
            this.m_canvas.Add(new ConnectionLineShape()
            {
                Source = this.m_canvas.FindShape("TL"),
                Target = this.m_canvas.FindShape("BL"),
                OutlineWidth = 3,
                OutlineColor = Color.OrangeRed,
                OutlineStyle = System.Drawing.Drawing2D.DashStyle.Dot
            }, "TLBL");

            SetButtons(EBUTTONSTATE.TRANSFORMSTART);
        }

        private void toolStripButtonAcceptTransform_Click(object sender, EventArgs e)
        {
            IShape itm = m_canvas.FindShape("TL");

            if (itm == null)
            {
                return;
            }

            IntPoint TL = new IntPoint((int)this.m_canvas.FindShape("TL").Position.X + 15,
                (int)this.m_canvas.FindShape("TL").Position.Y + 15);
            IntPoint TR = new IntPoint((int)this.m_canvas.FindShape("TR").Position.X + 15,
                (int)this.m_canvas.FindShape("TR").Position.Y + 15);
            IntPoint BR = new IntPoint((int)this.m_canvas.FindShape("BR").Position.X + 15,
                (int)this.m_canvas.FindShape("BR").Position.Y + 15);
            IntPoint BL = new IntPoint((int)this.m_canvas.FindShape("BL").Position.X + 15,
                (int)this.m_canvas.FindShape("BL").Position.Y + 15);

            // define quadrilateral's corners
            List<IntPoint> corners = new List<IntPoint>();
            corners.Add(TL);
            corners.Add(TR);
            corners.Add(BR);
            corners.Add(BL);

            double width = Math.Sqrt(Math.Pow(TR.X - TL.X, 2) + Math.Pow(TR.Y - TL.Y, 2));
            double height = Math.Sqrt(Math.Pow(BR.X - TR.X, 2) + Math.Pow(BR.Y - TR.Y, 2));

            double w = cropBmp.Width;
            double h = cropBmp.Height;

            // create filter
            QuadrilateralTransformation filter =
                new QuadrilateralTransformation(corners, (int)width, (int)height);
            // apply the filter

            cropBmp = new MagickImage(filter.Apply(cropBmp.ToBitmap()));
            this.m_canvas.Clear();
            this.m_canvas.Add(new BackgroundImageShape() { Image = cropBmp.ToBitmap() }, "Image");
            SetButtons(EBUTTONSTATE.TRANSFORMEND);
        }

        private void toolStripButtonCancelTransform_Click(object sender, EventArgs e)
        {
            this.m_canvas.Clear();
            this.m_canvas.Add(new BackgroundImageShape() { Image = cropBmp.ToBitmap() }, "Image");
            SetButtons(EBUTTONSTATE.TRANSFORMEND);
        }

        private void toolStripButtonBrightness_Click(object sender, EventArgs e)
        {
            formAdjustLight frm = new formAdjustLight();
            frm.bmp = new Bitmap(cropBmp.ToBitmap());
            DialogResult result = frm.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                cropBmp = new MagickImage(frm.imgLight);
            }
            this.m_canvas.Clear();
            this.m_canvas.Add(new BackgroundImageShape() { Image = cropBmp.ToBitmap() }, "Image");
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {

            IShape itm = m_canvas.FindShape("Crop");

            if (itm == null)
            {
                this.m_canvas.Add(new CropStencil()
                {
                    Size = new SizeF(60, 60),
                    Position = new PointF(this.skHost1.HorizontalScroll.Value, this.skHost1.VerticalScroll.Value)
                }, "Crop");
                SetButtons(EBUTTONSTATE.CROPSTART);
            }            
        }
    }
}
