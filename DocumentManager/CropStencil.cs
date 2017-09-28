using FyfeSoftware.Sketchy.Core.Primitives.Stencils;
using FyfeSoftware.Sketchy.Core.Shapes;
using System;
using System.Drawing;
using FyfeSoftware.Sketchy.Core;

namespace DocumentManager
{
    [Serializable]
    class CropStencil : AbstractInteractiveStencil
    {
        public override SizeModeType AllowedSizing
        {
            get
            {
                return SizeModeType.Horizontal | SizeModeType.Vertical;
            }
        }

        public CropStencil()
        {
            Size = new SizeF(60, 60);
            this.Add(new RectangleShape()
            {
                FillBrush = new SolidBrush(Color.FromArgb(50, 255, 255, 255)),
                OutlineColor = Color.LightGreen,
                OutlineWidth = 3,
                OutlineStyle = System.Drawing.Drawing2D.DashStyle.Solid,
                Position = new PointF(0, 0),
                Size = this.Size
            });
        }

        public override IShapeEditor Editor
        {
            get { return null;}
        }

        public override void HandleEndEdit(ShapeEditEventArgs e)
        {
        }

    }

    [Serializable]
    class pointStencil : AbstractInteractiveStencil
    {
        public override SizeModeType AllowedSizing
        {
            get
            {
                return SizeModeType.MoveOnly;
            }
        }

        public pointStencil()
        {
            Size = new SizeF(60, 60);
            this.Add(new RectangleShape()
            {
                FillBrush = new SolidBrush(Color.FromArgb(50, 255, 0, 0)),
                OutlineColor = Color.Red,
                OutlineWidth = 0,
                OutlineStyle = System.Drawing.Drawing2D.DashStyle.Solid,
                Position = new PointF(0, 0),
                Size = this.Size
            });
        }

        public override IShapeEditor Editor
        {
            get { return null; }
        }

        public override void HandleEndEdit(ShapeEditEventArgs e)
        {
        }

    }

    [Serializable]
    public class BackgroundImageShape : AbstractStyledShape
    {
        public BackgroundImageShape()
        {
        }

        [NonSerialized]
        private Image m_image;
        public Image Image
        {
            get
            {
                return this.m_image;
            }
            set
            {
                this.m_image = value;
                this.Position = new PointF(0, 0);
                this.Size = this.m_image.Size;

                if (this.GetCanvas() != null)
                    this.GetCanvas().Size = new Size((int)this.Size.Width, (int)this.Size.Height);

                // Redraw on change
                if (this.GetCanvas() != null)
                    this.GetCanvas().Invalidate();
            }
        }

        public override SizeModeType AllowedSizing
        {
            get
            {
                return SizeModeType.None;
            }
        }

        public override bool DrawTo(Graphics g)
        {
            if (this.GetCanvas().Size.Width < this.Size.Width && 
                this.GetCanvas().Size.Height < this.Size.Height)
                this.GetCanvas().Size = new Size((int)this.Size.Width, (int)this.Size.Height);
            g.DrawImage(this.m_image, this.DrawPosition.X, this.DrawPosition.Y, this.DrawSize.Width, this.DrawSize.Height);
            return true;
        }
    }

    public class CornerAnchorShape : AbstractInteractiveStencil
    {

        /// <summary>
        /// Return null
        /// </summary>
        public override IShapeEditor Editor
        {
            get { return null; }
        }

        /// <summary>
        /// End of edit
        /// </summary>
        public override void HandleEndEdit(ShapeEditEventArgs e)
        {
            return;
        }

        /// <summary>
        /// Allowed sizing
        /// </summary>
        public override SizeModeType AllowedSizing
        {
            get
            {
                return SizeModeType.MoveOnly;
            }
        }

        // The template
        private PointF m_point;
        private RectangleShape m_markerBox;
        private TextShape m_labelShape;

        /// <summary>
        /// Scan area stencil
        /// </summary>
        public CornerAnchorShape(PointF point, String label)
        {
            this.Tag = this.m_point = point;
            this.Position = new PointF(point.X - 15, point.Y - 15);
            this.Size = new SizeF(30, 30);
            this.m_markerBox = new RectangleShape()
            {
                Size = new SizeF(24, 24),
                Position = new PointF(3, 3),
                FillBrush = new SolidBrush(Color.FromArgb(127, 255, 255, 0)),
                OutlineColor = Color.LightGreen,
                OutlineWidth = 5,
                OutlineStyle = System.Drawing.Drawing2D.DashStyle.Solid
            };
            this.m_labelShape = new TextShape()
            {
                FillBrush = Brushes.Black,
                Font = new Font(SystemFonts.CaptionFont, FontStyle.Bold),
                Position = new PointF(5, 5),
                Text = label
            };

            this.Add(this.m_markerBox);
            this.Add(this.m_labelShape);

        }


    }

}
