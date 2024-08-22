using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Sintec.Tool
{
    public class CsvTool
    {
        public static int appendOnCsv(String[][] output, String filePath)
        {
            int rowWrote = 0;
            string delimiter = ";";
            int length = output.GetLength(0);
            StringBuilder sb = new StringBuilder();
            for (int index = 0; index < length; index++)
            {
                sb.AppendLine(string.Join(delimiter, output[index]));
                rowWrote++;
            }
            try
            {
                if (File.Exists(filePath))
                    File.AppendAllText(filePath, sb.ToString());
                else
                    File.WriteAllText(filePath, sb.ToString());
            }
            catch { }
            return rowWrote;
        }
        public static int appendOnCsv(String[] output, String filePath)
        {
            int rowWrote = 0;
            string delimiter = ";";
            int length = output.GetLength(0);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Join(delimiter, output));
            rowWrote++;
            try
            {
                if (File.Exists(filePath))
                    File.AppendAllText(filePath, sb.ToString());
                else
                    File.WriteAllText(filePath, sb.ToString());
            }
            catch { }
            return rowWrote;
        }
    }
}
