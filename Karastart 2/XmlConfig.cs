using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AMS.Profile;

namespace KaraStart
{
    public class XmlConfig
    {
        Profile p;
        public XmlConfig(String fileName)
        {
            p = new Xml(fileName);
        }
        public void IniWriteValue(String section, String key, String value)
        {
            p.SetValue(section, key, value);
        }
        public string IniReadValue(string Section, string Key, string defaultValue)
        {
            String finalRes = IniReadValue(Section, Key);
            if (finalRes == null || finalRes.Equals(""))
            {
                finalRes = defaultValue;
            }
            return finalRes;

        }
        public String IniReadValue(String section, String key)
        {
            String res = "";
            try
            {
                res = (String)p.GetValue(section, key);
            }
            catch { }
            return res;
        }
    }
}
