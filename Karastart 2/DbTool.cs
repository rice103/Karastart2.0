using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace KaraStart
{
    public class DbTool
    {
        public static String getHashOfFile(String fName)
        {
            FileInfo fi = new FileInfo(fName);
            //int hash = (fi.LastWriteTimeUtc.ToString() + fi.Length.ToString()).GetHashCode();
            return GetStringSha1Hash(fi.LastWriteTimeUtc.ToString() + fi.Length.ToString());
        }

        public static bool isSameFile(String f1, String f2)
        {
            return (getHashOfFile(f1) == getHashOfFile(f2)) ;
        }

        internal static string GetStringSha1Hash(string text)
        {
            if (String.IsNullOrEmpty(text))
                return String.Empty;

            using (var sha1 = new System.Security.Cryptography.SHA1Managed())
            {
                byte[] textData = Encoding.UTF8.GetBytes(text);

                byte[] hash = sha1.ComputeHash(textData);

                return BitConverter.ToString(hash).Replace("-", String.Empty);
            }
        }
    }
}
