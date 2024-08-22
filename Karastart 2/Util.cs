using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Microsoft.Win32;

namespace DragNDrop
{
    internal static class Util
    {
        internal static void CreateDragItemTempFile(string dragItemTempFileName,string sourceFile)
        {
            File.Copy(sourceFile,dragItemTempFileName,true);
        }

        public static Version getVersion()
        {
            System.Version AppVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            string MyVersion = AppVersion.Major.ToString()
            + "." + AppVersion.Minor.ToString()
            + "." + AppVersion.Build.ToString()
            + "." + AppVersion.Revision.ToString();
            return AppVersion;
        }
        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        public static IEnumerable<string> RecommendedPrograms(string ext)
        {
            List<string> progs = new List<string>();

            string baseKey = @"Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts\." + ext;

            using (RegistryKey rk = Registry.CurrentUser.OpenSubKey(baseKey + @"\OpenWithList"))
            {
                if (rk != null)
                {
                    string mruList = (string)rk.GetValue("MRUList");
                    if (mruList != null)
                    {
                        foreach (char c in mruList.ToString())
                            progs.Add(rk.GetValue(c.ToString()).ToString());
                    }
                }
            }

            using (RegistryKey rk = Registry.CurrentUser.OpenSubKey(baseKey + @"\OpenWithProgids"))
            {
                if (rk != null)
                {
                    foreach (string item in rk.GetValueNames())
                        progs.Add(item);
                }
                //TO DO: Convert ProgID to ProgramName, etc.
            }

            return progs;
        }
    }

}
