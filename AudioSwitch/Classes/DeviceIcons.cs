﻿using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using AudioSwitch.Properties;

namespace AudioSwitch.Classes
{
    public static class DeviceIcons
    {
        [DllImport("shell32.dll")]
        private static extern int ExtractIconEx(string libName, int iconIndex, IntPtr[] largeIcon, IntPtr[] smallIcon, int nIcons);

        public static ImageList ActiveIcons;
        public static ImageList NormalIcons;
        public static ImageList DefaultIcons;
        
        public static void InitImageLists(float dpifactor)
        {
            var size = new Size((int)(32 * dpifactor), (int)(32 * dpifactor));
            ActiveIcons = new ImageList
            {
                ImageSize = size,
                ColorDepth = ColorDepth.Depth32Bit
            };
            NormalIcons = new ImageList
            {
                ImageSize = size,
                ColorDepth = ColorDepth.Depth32Bit
            };
            DefaultIcons = new ImageList
            {
                ImageSize = size,
                ColorDepth = ColorDepth.Depth32Bit
            };
        }

        public static void Add(string iconPath)
        {
            var icon = GetIcon(iconPath);
            NormalIcons.Images.Add(icon);
            ActiveIcons.Images.Add(icon);
            DefaultIcons.Images.Add(AddOverlay(icon, Resources.defaultDevice));
        }

        public static Icon GetIcon(string iconPath)
        {
            var path = Environment.ExpandEnvironmentVariables(iconPath);
            var iconAdr = path.Split(',');

            Icon icon;
            if (iconAdr.Length > 1)
            {
                var hIconEx = new IntPtr[1];
                ExtractIconEx(iconAdr[0], int.Parse(iconAdr[1]), hIconEx, null, 1);
                icon = Icon.FromHandle(hIconEx[0]);
            }
            else
                icon = new Icon(iconAdr[0], NormalIcons.ImageSize.Width, NormalIcons.ImageSize.Height);
            return icon;
        }

        public static void Clear()
        {
            ActiveIcons.Images.Clear();
            NormalIcons.Images.Clear();
            DefaultIcons.Images.Clear();
        }

        private static Image AddOverlay(Icon originalIcon, Image overlay)
        {
            using (Image original = originalIcon.ToBitmap())
            {
                var bitmap = new Bitmap(originalIcon.Width, originalIcon.Height);
                using (var canvas = Graphics.FromImage(bitmap))
                {
                    canvas.DrawImage(original, 0, 0);
                    canvas.DrawImage(overlay, 0, 0, original.Width, original.Height);
                    canvas.Save();
                    return bitmap;
                }
            }
        }

        public static Bitmap ChangeColors(Bitmap bmp, int hue, float saturation, float brightness)
        {
            for (var y = 0; y < bmp.Height; y++)
                for (var x = 0; x < bmp.Width; x++)
                {
                    var p = bmp.GetPixel(x, y);
                    var pb = p.GetBrightness() + brightness;
                    pb = pb < 0 ? 0 : pb;
                    pb = pb > 1 ? 1 : pb;

                    var c = ColorFromAhsb(p.A, hue, p.GetSaturation() + saturation, pb);
                    bmp.SetPixel(x, y, c);
                }
            return bmp;
        }

        private static Color ColorFromAhsb(int alpha, float hue, float saturation, float brightness)
        {
            if (0 == saturation)
            {
                return Color.FromArgb(alpha, Convert.ToInt32(brightness * 255),
                  Convert.ToInt32(brightness * 255), Convert.ToInt32(brightness * 255));
            }

            float fMax, fMid, fMin;

            if (0.5 < brightness)
            {
                fMax = brightness - (brightness * saturation) + saturation;
                fMin = brightness + (brightness * saturation) - saturation;
            }
            else
            {
                fMax = brightness + (brightness * saturation);
                fMin = brightness - (brightness * saturation);
            }

            var iSextant = (int)Math.Floor(hue / 60f);
            if (300f <= hue)
                hue -= 360f;
            hue /= 60f;
            hue -= 2f * (float)Math.Floor(((iSextant + 1f) % 6f) / 2f);

            if (0 == iSextant % 2)
                fMid = hue * (fMax - fMin) + fMin;
            else
                fMid = fMin - hue * (fMax - fMin);

            var iMax = Convert.ToInt32(fMax * 255);
            var iMid = Convert.ToInt32(fMid * 255);
            var iMin = Convert.ToInt32(fMin * 255);

            switch (iSextant)
            {
                case 1:
                    return Color.FromArgb(alpha, iMid, iMax, iMin);
                case 2:
                    return Color.FromArgb(alpha, iMin, iMax, iMid);
                case 3:
                    return Color.FromArgb(alpha, iMin, iMid, iMax);
                case 4:
                    return Color.FromArgb(alpha, iMid, iMin, iMax);
                case 5:
                    return Color.FromArgb(alpha, iMax, iMin, iMid);
                default:
                    return Color.FromArgb(alpha, iMax, iMid, iMin);
            }
        }
    }
}
