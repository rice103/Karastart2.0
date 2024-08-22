using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.Windows.Forms;
using System.Security.AccessControl;

namespace QEyeCommon.Tool
{
    public class RegistryTool
    {
        public static bool setValue(String key, string value)
        {
            //non fuzniona senza impostare avvio da amministratore
            RegistrySecurity rs = new RegistrySecurity(); // it is right string for this code
            string currentUserStr = Environment.UserDomainName + "\\" + Environment.UserName;
            rs.AddAccessRule(new RegistryAccessRule(currentUserStr, RegistryRights.WriteKey | RegistryRights.ReadKey | RegistryRights.Delete | RegistryRights.FullControl, AccessControlType.Allow));
            try
            {
                RegistryKey regKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("Software\\" + Application.CompanyName + "\\" + Application.ProductName + "\\");
                if (regKey == null)
                {
                    regKey = Microsoft.Win32.Registry.LocalMachine.CreateSubKey("Software\\" + Application.CompanyName + "\\" + Application.ProductName + "\\", RegistryKeyPermissionCheck.ReadWriteSubTree );
                }
                if (regKey != null)
                {
                    regKey.SetValue(key, value, RegistryValueKind.String);
                    return true;
                }
                else
                {
                    return false;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static string getValue(String key)
        {
            try
            {
                RegistryKey regKey = Registry.LocalMachine;
                regKey = regKey.OpenSubKey("Software\\" + Application.CompanyName + "\\" + Application.ProductName + "\\");

                if (regKey != null)
                {
                    return regKey.GetValue(key).ToString();
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                return "";
            }
        }
    }
}
