using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace KaraStart
{
    public partial class Catcher : Form
    {
        public Point location;
        public Size size;
        bool moving = false;
        Point offset;
        bool resizing = false;
        Point offsetR;
        public Catcher()
        {
            InitializeComponent();
        }

        private void ScreenCatcher_Load(object sender, EventArgs e)
        {
            this.SetStyle(System.Windows.Forms.ControlStyles.SupportsTransparentBackColor, true);
            this.BackColor = System.Drawing.Color.Transparent;
            if (location.X == int.MinValue) location = this.Location;
            if (size.Width  == int.MinValue) size = this.Size;
            this.Location = location;
            this.Size = size;
        }

        private void panel2_MouseDown(object sender, MouseEventArgs e)
        {
            moving = true;
            offset = new Point(MousePosition.X - this.Left, MousePosition.Y - this.Top);
        }

        private void panel2_MouseUp(object sender, MouseEventArgs e)
        {
            moving = false;
        }

        private void panel2_MouseMove(object sender, MouseEventArgs e)
        {
            if (moving)
            {
                this.Left = -offset.X + MousePosition.X;
                this.Top = -offset.Y + MousePosition.Y;
                location = this.Location;
                Program.mainForm.writeIniClone(location, size);
            }
        }

        private void panel6_MouseUp(object sender, MouseEventArgs e)
        {
            resizing = false;
        }

        private void panel6_MouseMove(object sender, MouseEventArgs e)
        {
            if (resizing)
            {
                this.Width  = -offsetR.X + MousePosition.X;
                this.Height = -offsetR.Y + MousePosition.Y;
                size = this.Size;
                Program.mainForm.writeIniClone(location, size);
            }
        }

        private void panel6_MouseDown(object sender, MouseEventArgs e)
        {
            resizing = true;
            offsetR = new Point(MousePosition.X - this.Width , MousePosition.Y - this.Height );
        }

    }
}
