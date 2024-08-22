using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace KaraStart
{
    public class MyCheckBox : CheckBox
    {
        public MyCheckBox()
            : base()
        {
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
        }

        protected override void OnPaintBackground(
    System.Windows.Forms.PaintEventArgs e)
        {
            base.OnPaintBackground(e);
            System.Drawing.Drawing2D.LinearGradientBrush brush
                = new System.Drawing.Drawing2D.LinearGradientBrush(
                this.ClientRectangle,
                System.Drawing.SystemColors.Highlight,
                System.Drawing.SystemColors.Window,
                System.Drawing.Drawing2D.LinearGradientMode.BackwardDiagonal);

            e.Graphics.FillRectangle(brush, this.ClientRectangle);
            brush.Dispose();
        }
    }
}
