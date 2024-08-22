using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Timers;
using DragNDrop;
using MCIDEMO;
using System.IO;

namespace KaraStart
{
    public partial class SecondScreen : Form
    {
        public int textSize = Int32.Parse(Program.cfg.IniReadValue("settings", "screenTextSize", "6"));
        public Color textColor = Color.FromName(Program.cfg.IniReadValue("settings", "screenTextColor", "White"));
        public int textVel = Int32.Parse(Program.cfg.IniReadValue("settings", "screenTextVelocity", "4"));
        private double actualPositionOfText = 0;
        public String text = "";
        private bool _useText = false;
        public bool useText
        {
            get
            {
                return _useText;
            }
            set
            {
                _useText = value;
                actualPositionOfText = panel1.Width;
            }
        }
        Point offset;
        bool moving = false;
        public bool useBitmap = false;
        public SecondScreen()
        {
            InitializeComponent();
            timer1.Enabled = false;
        }

        private void OnTimedEvent(object sender, EventArgs e)
        {
            aTimer.Enabled = false;
            panel1.Refresh();
            aTimer.Enabled = true;
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
                this.Left = - offset.X + MousePosition.X;
                this.Top = - offset.Y + MousePosition.Y ;
            }
        }

        public void panel1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                panel2.Visible = true;
                this.WindowState = FormWindowState.Normal;
                this.Size = new Size(320, 240);
            }
            else{
                panel2.Visible = false;
                this.WindowState = FormWindowState.Maximized;
                MessageBox.Show("Per ripristinare la finestra clone premere ESC");
            }
            timer2.Enabled = true;
        }

        private void panel1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                Program.mainForm.button30.PerformClick();
            }
        }

        private void SecondScreen_FormClosed(object sender, FormClosedEventArgs e)
        {
            Program.mainForm.button30.PerformClick();
        }

        System.Timers.Timer aTimer;
        public void StartTimer()
        {
            millis = DateTime.Now;
            timer1.Interval = 40;
            timer1.Enabled = true;
            
            //aTimer = new System.Timers.Timer(10000);
            ////GC.KeepAlive(aTimer);

            //// Hook up the Elapsed event for the timer.
            //aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);

            //// Set the Interval to 2 seconds (2000 milliseconds).
            //aTimer.Interval = 30;
            //aTimer.Enabled = true;
        }

        public void StopTimer()
        {
            if (m != null)
            {
                String Pcommand = "close aviWitkKarastart";
                m.MCISendString(Pcommand);
                m = null;
            }
            //aTimer.Enabled = false;
            timer1.Enabled = false;
        }

        public Bitmap bS = null;
        DateTime millis = DateTime.Now;
        MciPlayer m = null;
        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            //panel1.Refresh();
            Bitmap b = new Bitmap(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height);
            if (!useBitmap)
            {
                if (m != null)
                {
                    String Pcommand = "close aviWitkKarastart";
                    m.MCISendString(Pcommand);
                    m = null;
                }
                b = ScreenshotCaptureWithMouse.ScreenCapture.CaptureScreen.CaptureDesktop(Program.mainForm.catcher.location.X, Program.mainForm.catcher.location.Y, Program.mainForm.catcher.size.Width, Program.mainForm.catcher.size.Height);
            }
            else
            {
                using (Graphics g = Graphics.FromImage(b))
                {
                    if (Program.mainForm.mediaSourceForSecondScreen.EndsWith(".jpg") || Program.mainForm.mediaSourceForSecondScreen.EndsWith(".bmp"))
                    {
                        if (bS == null)
                            bS = Util.ResizeImage(new Bitmap(Program.mainForm.mediaSourceForSecondScreen), Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height);
                        g.DrawImage(bS, new Rectangle(0, 0, b.Width, b.Height), new Rectangle(0, 0, bS.Width, bS.Height), GraphicsUnit.Pixel);
                        ((IDisposable)g).Dispose();
                        GC.SuppressFinalize(g);
                    }
                    else
                    {
                        if (m == null)
                        {
                            panel1.CreateGraphics().Clear(Color.Black);

                            string Pcommand = "";
                            m = new MciPlayer();

                            //close any files using the alias with the value of _index
                            Pcommand = "close aviWitkKarastart";
                            m.MCISendString(Pcommand);

                            FileInfo fi = new FileInfo(Program.mainForm.mediaSourceForSecondScreen);

                            //open command with assigned alias and window handle
                            Pcommand = "open mpegvideo!\"" +
                                       "unicodenamesupport\\0" + fi.Extension + "\" alias aviWitkKarastart" +
                                       " parent " + panel1.Handle.ToString() + " style child";

                            m.MCISendString(Pcommand);

                            //find the size of he source rectangle 
                            Pcommand = "where aviWitkKarastart source";
                            string s2 = m.MCISendString(Pcommand);

                            //maximum size of video to filt into a sub-area slot
                            int width = panel1.Width;
                            int height = panel1.Height;

                            //get the aspect ratio of width/height to be same as source but within our area allocated
                            MciPlayer.getAspectRatioSize(s2, ref width, ref height);

                            int l = (int)(panel1.Width - width) / 2;
                            int t = (int)(panel1.Height - height) / 2;

                            //set the active window
                            Pcommand = "put aviWitkKarastart window at " + l + " " + t + " " + width + " " + height;
                            m.MCISendString(Pcommand);

                            Pcommand = "setaudio aviWitkKarastart off";
                            m.MCISendString(Pcommand);

                            Pcommand = "play aviWitkKarastart repeat";
                            m.MCISendString(Pcommand);
                        }
                    }
                }

            }
            if (m == null)
            {
                using (Graphics g = panel1.CreateGraphics())
                {
                    g.DrawImage(b, new Rectangle(0, 0, panel1.Width, panel1.Height), new Rectangle(0, 0, b.Width, b.Height), GraphicsUnit.Pixel);
                    Font f = new Font(FontFamily.GenericSerif, (int)Math.Round((double)(textSize * panel1.Height) / 50.0));
                    //Font f2 = new Font(FontFamily.GenericSerif, 2 + (int)Math.Round((double)panel1.Height / 8.0));
                    double v = (double)panel1.Width / 5.0;
                    int millisTrascorsi = -(int)millis.Subtract(DateTime.Now).TotalMilliseconds;
                    actualPositionOfText = actualPositionOfText - (v * (double)(textVel * millisTrascorsi) / 4000.0);
                    millis = DateTime.Now;
                    Point pos = new Point((int)Math.Round(actualPositionOfText), panel1.Height - (int)Math.Round(f.GetHeight()) - 2);
                    if (g.MeasureString(text, f).Width + actualPositionOfText < 0)
                        actualPositionOfText = panel1.Width;
                    if (useText)
                    {
                        //g.DrawString(text, f2, Brushes.Black, pos.X - (int)((g.MeasureString(text, f2).Width - g.MeasureString(text, f).Width) / 2.0), pos.Y - (int)((g.MeasureString(text, f2).Height - g.MeasureString(text, f).Height) / 2.0));
                        g.DrawString(text, f, new SolidBrush(textColor), pos.X, pos.Y);
                    }
                    ((IDisposable)g).Dispose();
                    GC.SuppressFinalize(g);
                }
            }
            ((IDisposable)b).Dispose();
            GC.SuppressFinalize(b);
            GC.WaitForPendingFinalizers();
            timer1.Enabled = true;
        }

        private void SecondScreen_Resize(object sender, EventArgs e)
        {
            if (m != null)
            {
                String Pcommand = "where aviWitkKarastart source";
                string s2 = m.MCISendString(Pcommand);

                //maximum size of video to filt into a sub-area slot
                int width = panel1.Width;
                int height = panel1.Height;

                //get the aspect ratio of width/height to be same as source but within our area allocated
                MciPlayer.getAspectRatioSize(s2, ref width, ref height);

                int l = (int)(panel1.Width - width) / 2;
                int t = (int)(panel1.Height - height) / 2;

                //set the active window
                Pcommand = "put aviWitkKarastart window at " + l + " " + t + " " + width + " " + height;
                m.MCISendString(Pcommand);

                panel1.CreateGraphics().Clear(Color.Black);
            }
        }

        private void panel2_MouseClick(object sender, MouseEventArgs e)
        {
            panel1_MouseClick(sender, e);
        }

        private void panel2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            panel1_MouseDoubleClick(sender, e);
        }

        private void SecondScreen_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape && this.WindowState == FormWindowState.Maximized)
            {
                panel1_MouseDoubleClick(null, null);
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            timer2.Enabled = false;
            if (m != null)
            {
                panel1.CreateGraphics().Clear(Color.Black);

                //String Pcommand = "where aviWitkKarastart source";
                //string s2 = m.MCISendString(Pcommand);

                ////maximum size of video to filt into a sub-area slot
                //int width = panel1.Width;
                //int height = panel1.Height;

                ////get the aspect ratio of width/height to be same as source but within our area allocated
                //MciPlayer.getAspectRatioSize(s2, ref width, ref height);

                //int l = (int)(panel1.Width - width) / 2;
                //int t = (int)(panel1.Height - height) / 2;

                ////set the active window
                //Pcommand = "put aviWitkKarastart window at " + l + " " + t + " " + width + " " + height;
                //m.MCISendString(Pcommand);
            }
        }

    }
}
