using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace KaraStart
{
    public class RamDiskFile
    {
        String filePath;
        String originalFilePath;
        String fileName;
        public static string ramDiskRoot;
        public RamDiskFile(String originalFilePath)
        {
            bool sameFileExists = false;
            this.originalFilePath = originalFilePath;
            this.fileName = Path.GetFileName(originalFilePath);
            this.filePath = ramDiskRoot + "/" + fileName;
            if (File.Exists(filePath))
            {
                if (new FileInfo(filePath).Length == new FileInfo(originalFilePath).Length)
                {
                    sameFileExists = true;
                }
            }
            if (!sameFileExists)
            {
                File.Copy(originalFilePath, filePath, true);
            }
        }
        public string getKstStarter(String zipFile)
        {
            string res="";

            return res;
        }
    }
}
