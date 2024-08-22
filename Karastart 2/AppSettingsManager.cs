using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Windows.Forms;
using System.Collections;
using System.IO;

namespace Sintec.Tool
{
    public class AppSettingsManager
    {
        System.Configuration.Configuration config;
        public AppSettingsManager() : this("App.config") { }
        public AppSettingsManager(String filePath)
        {
            string appPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string configFile = System.IO.Path.Combine(appPath, filePath);
            ExeConfigurationFileMap configFileMap = new ExeConfigurationFileMap();
            configFileMap.ExeConfigFilename = configFile;
            config = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);
        }
        private static AppSettingsManager _istance;
        public static AppSettingsManager Istance
        {
            get
            {
                if (_istance == null)
                    _istance = new AppSettingsManager();
                return _istance;
            }
        }
        public String getAppSettings(String key)
        {
            String res = config.AppSettings.Settings[key].Value;
            if (res.StartsWith("~App_Data"))
                res = res.Replace("~App_Data", getAppDataDirectory());

                    //System.Windows.Forms.Application.UserAppDataPath;
            return res;
        }
        public String getAppDataDirectory()
        {
            if (!Directory.Exists(Environment.SpecialFolder.ApplicationData + "\\" + Application.CompanyName))
                Directory.CreateDirectory(Environment.SpecialFolder.ApplicationData + "\\" + Application.CompanyName);
            if (!Directory.Exists(Environment.SpecialFolder.ApplicationData + "\\" + Application.CompanyName + "\\" + Application.ProductName))
                Directory.CreateDirectory(Environment.SpecialFolder.ApplicationData + "\\" + Application.CompanyName + "\\" + Application.ProductName);
            return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + Application.CompanyName + "\\" + Application.ProductName;
        }
        public void setAppSettings(String key, String value)
        {
            config.AppSettings.Settings[key].Value = value;
            config.Save();
        }
    }
}
