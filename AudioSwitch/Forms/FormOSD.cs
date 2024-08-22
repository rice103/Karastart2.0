﻿using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Windows.Forms;
using AudioSwitch.Classes;
using Timer = System.Windows.Forms.Timer;

namespace AudioSwitch.Forms
{
    public partial class FormOSD : Form
    {
        private readonly Timer CloseTimer = new Timer();
        private TimeSpan timeout;
        private DateTime timeopened;
        public OSDskin Skin;
        private Bitmap defBackImg;
        private Bitmap muteBackImg;
        private Bitmap volBarImg;
        public byte Transparency = 255;
        private Bitmap LastBMPApplied;

        private void FadeoutForm(byte transparency)
        {
            for (var i = transparency; i > 0; i--)
            {
                SetBitmap(new Bitmap(LastBMPApplied), i);
                Application.DoEvents();
            }
            Hide();
        }

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x00080000; // This form has to have the WS_EX_LAYERED extended style
                return cp;
            }
        }

        public FormOSD()
        {
            InitializeComponent();
            CloseTimer.Tick += CloseTimer_Tick;
            CloseTimer.Interval = 100;
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x0084 /*WM_NCHITTEST*/)
            {
                m.Result = (IntPtr)2;	// HTCLIENT
                Program.settings.OSD.Left = Left;
                Program.settings.OSD.Top = Top;
                return;
            }
            base.WndProc(ref m);
        }

        private void CloseTimer_Tick(object sender, EventArgs eventArgs)
        {
            if (DateTime.Now - timeopened < timeout) return;
            CloseTimer.Stop();
            FadeoutForm(Transparency);            
        }

        private void ResetTimer()
        {
            timeout = new TimeSpan(0, 0, 0, 0, Program.settings.OSD.ClosingTimeout);
            timeopened = DateTime.Now;
            CloseTimer.Stop();
            CloseTimer.Start();
        }

        public void LoadSkin()
        {
            Left = Program.settings.OSD.Left;
            Top = Program.settings.OSD.Top;
            Transparency = Program.settings.OSD.Transparency;

            if (defBackImg != null) defBackImg.Dispose();
            defBackImg = new Bitmap(Image.FromFile("Skins\\" + Program.settings.OSD.Skin + "\\back.png"));

            if (muteBackImg != null) muteBackImg.Dispose();
            muteBackImg = new Bitmap(Image.FromFile("Skins\\" + Program.settings.OSD.Skin + "\\mute.png"));

            if (volBarImg != null) volBarImg.Dispose();
            volBarImg = new Bitmap(Image.FromFile("Skins\\" + Program.settings.OSD.Skin + "\\meter.png"));

            Skin = OSDskin.Load(Program.settings.OSD.Skin);
        }

        public void SetVolImage(float volume)
        {
            Bitmap combinedImg;
            if (Skin.VolBar.Type == "horizontal")
            {
                var barWidth = Math.Round(volume / (1f / Skin.VolBar.Steps)) * ((double)volBarImg.Width / Skin.VolBar.Steps);
                combinedImg = OverlayPicture(defBackImg, volBarImg, Skin.VolBar.X, Skin.VolBar.Y, (int)(barWidth), volBarImg.Height);
            }
            else
            {
                var barHeight = volBarImg.Height / Skin.VolBar.Steps;
                var barTop = (int)Math.Round(volume / (1f / Skin.VolBar.Steps)) * barHeight;
                if (barTop > volBarImg.Height - barHeight)
                    barTop = volBarImg.Height - barHeight;

                var barImg = GetCroppedPicture(volBarImg, 0, barTop, volBarImg.Width, barHeight);
                combinedImg = OverlayPicture(defBackImg, barImg, Skin.VolBar.X, Skin.VolBar.Y, volBarImg.Width, barHeight);
            }
            
            SetBitmap(combinedImg, Transparency);
        }

        public void ChangeVolume(float volume)
        {
            if (Skin == null)
                LoadSkin();

            SetVolImage(volume);
            ResetTimer();
            Show();
        }

        public void ChangeMute(bool isMuted, float volume)
        {
            if (Skin == null)
                LoadSkin();

            if (isMuted)
                SetBitmap(new Bitmap(muteBackImg), Transparency);
            else
                SetVolImage(volume);

            ResetTimer();
            Show();
        }

        public void ChangeDevice(string changedTextBottom)
        {
            if (Skin == null)
                LoadSkin();

            SetBitmap(OverlayText(changedTextBottom), Transparency);
            ResetTimer();
            Show();
        }

        private Bitmap OverlayText(string text)
        {
            var newImg = new Bitmap(defBackImg);
            using (var graphics = Graphics.FromImage(newImg))
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.TextRenderingHint = TextRenderingHint.AntiAlias;

                graphics.DrawImage(defBackImg, 0, 0);
                graphics.DrawString(text,
                                    new Font(Skin.DeviceText.Font, Skin.DeviceText.FontSize),
                                    new SolidBrush(ColorTranslator.FromHtml(Skin.DeviceText.ColorHex)),
                                    new Rectangle(Skin.DeviceText.X, Skin.DeviceText.Y, Skin.DeviceText.MaxWidth, Skin.DeviceText.MaxHeight),
                                    new StringFormat(StringFormatFlags.LineLimit));
                return newImg;
            }
        }

        /// <para>Changes the current bitmap with a custom opacity level.  Here is where all happens!</para>
        private void SetBitmap(Bitmap bitmap, byte opacity)
        {
            if (LastBMPApplied != null)
                LastBMPApplied.Dispose();
            LastBMPApplied = bitmap;

            if (bitmap.PixelFormat != PixelFormat.Format32bppArgb)
                throw new ApplicationException("The bitmap must be 32ppp with alpha-channel.");

            // The ideia of this is very simple,
            // 1. Create a compatible DC with screen;
            // 2. Select the bitmap with 32bpp with alpha-channel in the compatible DC;
            // 3. Call the UpdateLayeredWindow.

            var screenDc = Win32.GetDC(IntPtr.Zero);
            var memDc = Win32.CreateCompatibleDC(screenDc);
            var hBitmap = IntPtr.Zero;
            var oldBitmap = IntPtr.Zero;

            try
            {
                hBitmap = bitmap.GetHbitmap(Color.FromArgb(0)); // grab a GDI handle from this GDI+ bitmap
                oldBitmap = Win32.SelectObject(memDc, hBitmap);

                var size = new Win32.Size(bitmap.Width, bitmap.Height);
                var pointSource = new Win32.Point(0, 0);
                var topPos = new Win32.Point(Left, Top);
                var blend = new Win32.BLENDFUNCTION
                {
                    BlendOp = Win32.AC_SRC_OVER,
                    BlendFlags = 0,
                    SourceConstantAlpha = opacity,
                    AlphaFormat = Win32.AC_SRC_ALPHA
                };

                try
                {
                    Win32.UpdateLayeredWindow(Handle, screenDc, ref topPos, ref size, memDc, ref pointSource, 0, ref blend,
                                          Win32.ULW_ALPHA);
                }
                catch { }
            }
            finally
            {
                Win32.ReleaseDC(IntPtr.Zero, screenDc);
                if (hBitmap != IntPtr.Zero)
                {
                    Win32.SelectObject(memDc, oldBitmap);
                    //Windows.DeleteObject(hBitmap); // The documentation says that we have to use the Windows.DeleteObject... but since there is no such method I use the normal DeleteObject from Win32 GDI and it's working fine without any resource leak.
                    Win32.DeleteObject(hBitmap);
                }
                Win32.DeleteDC(memDc);
            }
        }

        private Bitmap OverlayPicture(Bitmap first, Bitmap second, int x, int y, int width, int height)
        {
            var target = new Bitmap(first.Width, first.Height, PixelFormat.Format32bppArgb);
            using (var graphics = Graphics.FromImage(target))
            {
                graphics.DrawImage(first, 0, 0);

                if (width > 0 && height > 0)
                {
                    var rect = new Rectangle(0, 0, width, height);
                    using (var cropped = second.Clone(rect, second.PixelFormat))
                        graphics.DrawImage(cropped, x, y);
                }
            }
            return target;
        }

        private static Bitmap GetCroppedPicture(Bitmap picture, int x, int y, int width, int height)
        {
            return picture.Clone(new Rectangle(x, y, width, height), picture.PixelFormat);
        }
    }
}
