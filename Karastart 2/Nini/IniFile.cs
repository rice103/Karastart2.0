using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Nini.Ini;
using System.Collections;

namespace OrmeSpace
{
	/// <summary>
	/// Create a New INI file to store or load data
	/// </summary>
	public class IniFile
	{
        private IniDocument nIniDoc;
		private String path;

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        //[DllImport("kernel32")]
        //private static extern int GetPrivateProfileString(string section,string key,string def,StringBuilder retVal,int size,string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileSection(string section, IntPtr lpReturnedString,
          int nSize, string lpFileName);


		/// <summary>
		/// INIFile Constructor.
		/// </summary>
		/// <param name="INIPath"></param>

		public IniFile(string INIPath)
		{
            setPath(INIPath);
            
		}
		/// <summary>
		/// Write Data to the INI File
		/// </summary>
		/// <param name="Section"></param>
		/// Section name
		/// <param name="Key"></param>
		/// Key Name
		/// <param name="Value"></param>
		/// Value Name
		public void IniWriteValue(string Section,string Key,string Value)
		{
			WritePrivateProfileString(Section,Key,Value,this.path);

            nIniDoc.Save(this.path);
		}
		
		/// <summary>
		/// Read Data Value From the Ini File
		/// </summary>
		/// <param name="Section"></param>
		/// <param name="Key"></param>
		/// <param name="Path"></param>
		/// <returns></returns>
        /// 

        public void setPath(String newPath){
            this.path = newPath;
            nIniDoc = new IniDocument(newPath);
        }

        public string IniReadValue(string Section, string Key, string defaultValue)
        {
            String finalRes = IniReadValue(Section, Key);
            if (finalRes==null || finalRes.Equals(""))
            {
                finalRes = defaultValue;
            }
            return finalRes;

        }

		public string IniReadValue(string Section,string Key)
		{
            String temp = "";
            IniSection tmpSection;
            foreach (Object o in nIniDoc.Sections)
            {
                tmpSection = ((IniSection)((DictionaryEntry)o).Value);

                if (tmpSection.Name.ToLower().Equals(Section.ToLower()))
                {
                    temp = tmpSection.GetValue(Key);
                    break;
                }
            }
			return temp;
		}


        /// <summary>
        /// Reads a whole section of the INI file.
        /// </summary>
        /// <param name="section">Section to read.</param>
        public string[] ReadSection(string section)
        {
            const int bufferSize = 2048;

            StringBuilder returnedString = new StringBuilder();

            IntPtr pReturnedString = Marshal.AllocCoTaskMem(bufferSize);
            try
            {
                int bytesReturned = GetPrivateProfileSection(section, pReturnedString, bufferSize, this.path);

                //bytesReturned -1 to remove trailing \0
                for (int i = 0; i < bytesReturned - 1; i++)
                    returnedString.Append((char)Marshal.ReadByte(new IntPtr((uint)pReturnedString + (uint)i)));
            }
            finally
            {
                Marshal.FreeCoTaskMem(pReturnedString);
            }

            string sectionData = returnedString.ToString();
            return sectionData.Split('\0');
        }

	}
}
