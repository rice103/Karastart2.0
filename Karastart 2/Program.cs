/*
 * Created by SharpDevelop.
 * User: Rice Cipriani
 * Date: 07/05/2012
 * Time: 21:39
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Forms;
using System.Collections;
using System.IO;
using OrmeSpace;
using Sintec.Tool;
using AMS.Profile;

namespace KaraStart
{
	/// <summary>
	/// Class with program entry point.
	/// </summary>
	public sealed class Program
	{
        public static bool usingVlcMote = false;
        public static MainForm mainForm;
        //A prefix used in filename to identify a drag from the application
        //Make it more unique
        internal const string DRAG_SOURCE_PREFIX = "__DragNDrop__Temp__";
        //The item which we drag is being stored here
        //could be a path to a location or a filestream
        internal static object objDragItem;
        //A FileSystemWatcher used to monitor the System's Temp Directory for a drag from the application
        private static FileSystemWatcher tempDirectoryWatcher;
        //A Hashtable to keep multiple FileSystemWatchers 
        public static Hashtable watchers = null;

        public static XmlConfig cfg;
        public static IniFile ini;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
#if !DEBUG
            try
#endif
            {
                //Setting the tempDirectoryWatcher to monitor the creation of a file from our application
                tempDirectoryWatcher = new FileSystemWatcher();
                tempDirectoryWatcher.Path = Path.GetTempPath();
                tempDirectoryWatcher.Filter = string.Format("{0}*.tmp", DRAG_SOURCE_PREFIX);
                tempDirectoryWatcher.NotifyFilter = NotifyFilters.FileName;
                tempDirectoryWatcher.IncludeSubdirectories = false;
                tempDirectoryWatcher.EnableRaisingEvents = true;
                tempDirectoryWatcher.Created += new FileSystemEventHandler(TempDirectoryWatcherCreated);


                if (!Directory.Exists(AppSettingsManager.Istance.getAppDataDirectory()))
                    Directory.CreateDirectory(AppSettingsManager.Istance.getAppDataDirectory());
                if (!File.Exists(AppSettingsManager.Istance.getAppSettings("dataBase")))
                    File.Copy("data.sdf", AppSettingsManager.Istance.getAppSettings("dataBase"));
                //if (!File.Exists(AppSettingsManager.Istance.getAppSettings("ConfigFile")))
                //    File.Copy("karastart.config", AppSettingsManager.Istance.getAppSettings("ConfigFile"));

                if (!Directory.Exists(AppSettingsManager.Istance.getAppSettings("PlaylistRooth")))
                    Directory.CreateDirectory(AppSettingsManager.Istance.getAppSettings("PlaylistRooth"));

                //if (!File.Exists(AppSettingsManager.Istance.getAppSettings("ConfigFile")))
                //{
                    //StreamWriter sw = new StreamWriter(AppSettingsManager.Istance.getAppSettings("ConfigFile"));
                    //sw.Write("");
                    //sw.Close();
                //}

                cfg = new XmlConfig(AppSettingsManager.Istance.getAppSettings("ConfigFile"));
                //ini = new IniFile(@"./settings2.ini");

                //if (cfg.IniReadValue("settings", "checkUpdate", "True") == "True")
                //{
                //    if (DateTime.Parse(cfg.IniReadValue("settings", "lastUpdate", DateTime.MinValue.ToShortDateString())).CompareTo(DateTime.Today.AddDays(-1)) < 0)
                //    {
                //        cfg.IniWriteValue("settings", "lastUpdate", DateTime.Now.ToShortDateString());
                //        checkUpdate();
                //    }
                //}

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                mainForm = new MainForm();
                Application.Run(mainForm);
            }
#if !DEBUG
            catch (Exception e) { (new MailForm(e.Message)).ShowDialog(); }
#endif
        }

        public static void checkUpdate(){
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.EnableRaisingEvents = false;
            proc.StartInfo.FileName = @".\Karastart updater.exe";
            proc.StartInfo.Arguments = "Italiano";
            proc.Start();
            Environment.Exit(0);
        }

        #region FileSystemWatcher Methods
        /// <summary>
        /// A temp file is created in the temp folder with the prefix wen a drag is initiated
        /// At that point we will add a filesystem watch to all the Logical Drives present.
        /// And will clear the watchers when the drag is completed or canceled
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private static void TempDirectoryWatcherCreated(object sender, FileSystemEventArgs e)
        {
            try
            {
                CreateFileWatchers();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "DragNDrop Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        /// <summary>
        /// This Event is triggered when the temp file created is Dropped into the target folder.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void FileWatcherCreated(object sender, FileSystemEventArgs e)
        {
            try
            {
                OnFileDropPathFound(e.FullPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "DragNDrop Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        /// <summary>
        /// Methodd Called when the Dropped Path is found
        /// </summary>
        /// <param name="dropedFilePath"></param>
        private static void OnFileDropPathFound(string dropedFilePath)
        {
            try
            {
                if (dropedFilePath.Trim() != string.Empty && objDragItem != null)
                {
                    string dropPath = dropedFilePath.Substring(0, dropedFilePath.LastIndexOf('\\'));

                    if (File.Exists(dropedFilePath))
                        File.Delete(dropedFilePath);

                    //Use your Download Code here
                    MessageBox.Show(String.Format("{0} dropped to {1}",objDragItem.ToString(),dropPath));                    
                }
                objDragItem = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "DragNDrop Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Creating File Watchers for all Logical Drives in the System
        /// </summary>
        public static void CreateFileWatchers()
        {
            try
            {
                if (watchers == null)
                {
                    int i = 0;
                    Hashtable tempWatchers = new Hashtable();
                    FileSystemWatcher watcher;
                    //Adding FileSystemWatchers and adding Created event to it 
                    foreach (string driveName in Directory.GetLogicalDrives())
                    {
                        //this checking may sound absurd to you.
                        //but in OS like Windows 2000,
                        //even if there is no floppy drive present
                        //An "A:/" will be shown in My Computer.
                        //To avoid exceptions like this I've added this check.
                        if (Directory.Exists(driveName))
                        {
                            watcher = new FileSystemWatcher();
                            watcher.Filter = string.Format("{0}*.tmp", DRAG_SOURCE_PREFIX);
                            watcher.NotifyFilter = NotifyFilters.FileName;
                            watcher.Created += new FileSystemEventHandler(FileWatcherCreated);
                            watcher.IncludeSubdirectories = true;
                            watcher.Path = driveName;
                            watcher.EnableRaisingEvents = true;
                            tempWatchers.Add("file_watcher" + i.ToString(), watcher);
                            i = i + 1;
                        }
                    }
                    watchers = tempWatchers;
                    tempWatchers = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "DragNDrop Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        /// <summary>
        /// Removes the FileSystemWatchers watching the Logical Drives
        /// </summary>
        public static void ClearFileWatchers()
        {
            try
            {
                if (watchers != null && watchers.Count > 0)
                {
                    for (int i = 0; i < watchers.Count; i++)
                    {
                        ((FileSystemWatcher)watchers["file_watcher" + i.ToString()]).Dispose();
                    }
                    watchers.Clear();
                    watchers = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "DragNDrop Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion
    }
}
