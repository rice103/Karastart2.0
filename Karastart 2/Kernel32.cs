using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.Win32.SafeHandles;
using System.Windows.Forms;
using System.Diagnostics;
using Microsoft.Win32;
using System.Reflection;

namespace KaraStart
{
    public class Kernel32
    {
        public static Guid GuidMd5SumByProcess(String file)
        {
            return Guid.Parse(Md5SumByProcess(file));
        }
        public static string Md5SumByProcess(string file)
        {
            var p = new Process();
            p.StartInfo.FileName = "md5sum.exe";
            p.StartInfo.Arguments = '\u0022' + file + '\u0022';
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            p.WaitForExit();
            string output = p.StandardOutput.ReadToEnd();
            return output.Substring(output.Length - 55, 32); //output.Split(' ')[0].Substring(1).ToUpper();
        }

        public static void searchIn(string criteria, string dir, bool extendInSubDir, ref Queue<String> queueFile,out int numFile,out int numDir,BoolBox boolBox)
        {
            numDir = 0;
            numFile = 0;
            if (Directory.Exists(dir))
            {
                Kernel32 k = new Kernel32();
                long s = k.RecurseDirectory("*" + criteria + "*", dir, extendInSubDir, out numFile, out numDir, ref queueFile,boolBox);
            }
        }

        private long RecurseDirectory(string criteria, string directory, bool searchInSubFolders, out int files, out int folders, ref Queue<String> queueFile, BoolBox boolBox)
        {
            IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);
            files = 0;
            folders = 0;
            Application.DoEvents();
            if (boolBox.val) return 0;
            Kernel32.WIN32_FIND_DATA findData;

            // please note that the following line won't work if you try this on a network folder, like \\Machine\C$
            // simply remove the \\?\ part in this case or use \\?\UNC\ prefix
            //using (SafeFindHandle findHandle = Kernel32.FindFirstFile(@"\\?\" + directory + @"\*", out findData))
            using (SafeFindHandle findHandle = Kernel32.FindFirstFile(@"\\?\" + directory + @"\" + criteria , out findData))
            {
                if (!findHandle.IsInvalid)
                {

                    do
                    {
                        Application.DoEvents();
                        if (boolBox.val) return 0;
                        if ((findData.dwFileAttributes & FileAttributes.Directory) != 0)
                        {

                            if (findData.cFileName != "." && findData.cFileName != "..")
                            {
                                if (searchInSubFolders)
                                {
                                    System.Windows.Forms.Application.DoEvents();
                                    folders++;

                                    int subfiles, subfolders;
                                    string subdirectory = directory + (directory.EndsWith(@"\") ? "" : @"\") +
                                        findData.cFileName;
                                    if (subdirectory.ToLower().CompareTo(criteria) != 0)
                                    {
                                        RecurseDirectory("*", subdirectory, searchInSubFolders, out subfiles, out subfolders, ref queueFile,boolBox);
                                        folders += subfolders;
                                        files += subfiles;
                                    }
                                    else
                                    {
                                        RecurseDirectory(criteria, subdirectory, searchInSubFolders, out subfiles, out subfolders, ref queueFile, boolBox);
                                        folders += subfolders;
                                        files += subfiles;
                                    }
                                }
                            }
                        }
                        else
                        {
                            System.Windows.Forms.Application.DoEvents();
                            // File
                            files++;
                            if (!findData.dwFileAttributes.HasFlag(FileAttributes.Hidden))
                                queueFile.Enqueue(directory + "\\" + findData.cFileName);
                            //size += (long)findData.nFileSizeLow + (long)findData.nFileSizeHigh * 4294967296;
                        }
                    }
                    while (Kernel32.FindNextFile(findHandle, out findData));
                }

            }
            if (searchInSubFolders)
                foreach (String fo in Directory.GetDirectories(directory)){
                    RecurseDirectory(criteria,fo,searchInSubFolders,out files,out folders,ref queueFile,boolBox );
            }
            return 0;
        }
        

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern SafeFindHandle FindFirstFile(string lpFileName, out KaraStart.Kernel32.WIN32_FIND_DATA lpFindFileData);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool FindClose(SafeHandle hFindFile);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool FindNextFile(SafeHandle hFindFile, out KaraStart.Kernel32.WIN32_FIND_DATA lpFindFileData);

        internal sealed class SafeFindHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            // Methods
            [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
            internal SafeFindHandle()
                : base(true)
            {
            }

            public SafeFindHandle(IntPtr preExistingHandle, bool ownsHandle)
                : base(ownsHandle)
            {
                base.SetHandle(preExistingHandle);
            }

            protected override bool ReleaseHandle()
            {
                if (!(IsInvalid || IsClosed))
                {
                    return FindClose(this);
                }
                return (IsInvalid || IsClosed);
            }

            protected override void Dispose(bool disposing)
            {
                if (!(IsInvalid || IsClosed))
                {
                    FindClose(this);
                }
                base.Dispose(disposing);
            }
        }

        public static String getVersionFromRegistry()
        {
            String res = "";
            // Opens the registry key for the .NET Framework entry.
            using (RegistryKey ndpKey =
                RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, "").
                OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\"))
            {
                // As an alternative, if you know the computers you will query are running .NET Framework 4.5 
                // or later, you can use:
                // using (RegistryKey ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, 
                // RegistryView.Registry32).OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\"))
                foreach (string versionKeyName in ndpKey.GetSubKeyNames())
                {
                    if (versionKeyName.StartsWith("v"))
                    {
                        if (res != "")
                            res = res + "\n\r";
                        RegistryKey versionKey = ndpKey.OpenSubKey(versionKeyName);
                        string name = (string)versionKey.GetValue("Version", "");
                        string sp = versionKey.GetValue("SP", "").ToString();
                        string install = versionKey.GetValue("Install", "").ToString();
                        if (install == "") //no install info, must be later.
                            res+=(versionKeyName + "  " + name);
                        else
                        {
                            if (sp != "" && install == "1")
                            {
                                res += (versionKeyName + "  " + name + "  SP" + sp);
                            }

                        }
                        if (name != "")
                        {
                            continue;
                        }
                        foreach (string subKeyName in versionKey.GetSubKeyNames())
                        {
                            RegistryKey subKey = versionKey.OpenSubKey(subKeyName);
                            name = (string)subKey.GetValue("Version", "");
                            if (name != "")
                                sp = subKey.GetValue("SP", "").ToString();
                            install = subKey.GetValue("Install", "").ToString();
                            if (install == "") //no install info, must be later.
                                res += (versionKeyName + "  " + name);
                            else
                            {
                                if (sp != "" && install == "1")
                                {
                                    res += ("  " + subKeyName + "  " + name + "  SP" + sp);
                                }
                                else if (install == "1")
                                {
                                    res += ("  " + subKeyName + "  " + name);
                                }

                            }

                        }

                    }
                }
            }
            return res;
        }

        public static bool getSqlCeCompactInstalled()
        {
            try
            {
                System.Reflection.Assembly.Load("System.Data.SqlServerCe, Version=3.5.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91");
            }
            catch (System.IO.FileNotFoundException)
            {
                return false;
            }
            try
            {
                var factory = System.Data.Common.DbProviderFactories.GetFactory("System.Data.SqlServerCe.3.5");
            }
            catch (System.Configuration.ConfigurationException)
            {
                return false;
            }
            catch (System.ArgumentException)
            {
                return false;
            }
            return true;
        }

        public static String getAssemblyVersion(String name)
        {
            try
            {
                Assembly asm = Assembly.Load(name);
                return "Location: " + asm.Location + " version: " + FileVersionInfo.GetVersionInfo(asm.Location).FileVersion;
            }
            catch { return "No assembly found"; }
        }

        public static String getOsInfo()
        {
            //Get OperatingSystem information from the system namespace.
            System.OperatingSystem osInfo = System.Environment.OSVersion;

            //Determine the platform.
            switch (osInfo.Platform)
            {
                //Platform is Windows 95, Windows 98, Windows 98 Second Edition, 
                //or Windows Me.
                case System.PlatformID.Win32Windows:

                    switch (osInfo.Version.Minor)
                    {
                        case 0:
                            return ("Windows 95");
                            break;
                        case 10:
                            if (osInfo.Version.Revision.ToString() == "2222A")
                                return ("Windows 98 Second Edition");
                            else
                                return ("Windows 98");
                            break;
                        case 90:
                            return ("Windows Me");
                            break;
                    }
                    break;

                //Platform is Windows NT 3.51, Windows NT 4.0, Windows 2000, or Windows XP.
                case System.PlatformID.Win32NT:

                    switch (osInfo.Version.Major)
                    {
                        case 3:
                            return("Windows NT 3.51");
                            break;
                        case 4:
                            return ("Windows NT 4.0");
                            break;
                        case 5:
                            if (osInfo.Version.Minor == 0)
                                return ("Windows 2000");
                            else
                                return ("Windows XP");
                            break;
                        case 6:
                            if (osInfo.Version.Minor == 1)
                                return ("Windows 7");
                            else if (osInfo.Version.Minor == 0)
                                return ("Windows Vista");
                            else
                                return ("Windows 8");
                    } break;
            }
            return "";
        }

        public const int MAX_PATH = 260;
        public const int MAX_ALTERNATE = 14;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct WIN32_FIND_DATA
        {
            public FileAttributes dwFileAttributes;
            public FILETIME ftCreationTime;
            public FILETIME ftLastAccessTime;
            public FILETIME ftLastWriteTime;
            public uint nFileSizeHigh; //changed all to uint from int, otherwise you run into unexpected overflow
            public uint nFileSizeLow;  //| http://www.pinvoke.net/default.aspx/Structures/WIN32_FIND_DATA.html
            public uint dwReserved0;   //|
            public uint dwReserved1;   //v
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
            public string cFileName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_ALTERNATE)]
            public string cAlternate;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SHFILEINFO
    {
        public IntPtr hIcon;
        public IntPtr iIcon;
        public uint dwAttributes;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string szDisplayName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
        public string szTypeName;
    };

    class Win32
    {
        public const uint SHGFI_ICON = 0x100;
        public const uint SHGFI_LARGEICON = 0x0;    // 'Large icon
        public const uint SHGFI_SMALLICON = 0x1;    // 'Small icon

        [DllImport("shell32.dll")]
        public static extern IntPtr SHGetFileInfo(string pszPath,
                                    uint dwFileAttributes,
                                    ref SHFILEINFO psfi,
                                    uint cbSizeFileInfo,
                                    uint uFlags);
    }
}
