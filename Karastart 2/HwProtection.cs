using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Management;
using System.Collections;
using System.Security.Cryptography;
using System.IO;

namespace Sintec.Tool
{
    class HardDrive
    {
        private string model = null;
        private string type = null;
        private string serialNo = null;
        public string Model
        {
            get { return model; }
            set { model = value; }
        }
        public string Type
        {
            get { return type; }
            set { type = value; }
        }
        public string SerialNo
        {
            get { return serialNo; }
            set { serialNo = value; }
        }
    }
    public class HwProtection
    {
        //diskCodeMD5 = md5(codice disco + "sintecsrl")
        //password = md5("prefisso" + diskCodeMD5 + "suffisso")
        public static string diskCodeMD5()
        {
            return getHdMD5();
        }
        public static string diskCodeMD5(string untiledDiskCode)
        {
            return getHdMD5(untiledDiskCode);
        }
        private static string oldDllCode()
        {
            return HwProtection.GetMd5Hash(MD5.Create(), "prefisso" + getHdMD5(oldDiskCode()) + "suffisso");
        }
        public static string dllCode(string untiledDiskCode)
        {
            String oldCode=oldDllCode();
            if (oldCode == "f1017c67c6898fb4b7275428ec372914" || oldCode == "6cbc2106e8caf4208deb4dff122d46f4" || oldCode == "e9f566cec13b41de2b8bfde113147529")
                return oldCode;
            else
                return HwProtection.GetMd5Hash(MD5.Create(), "prefisso" + untiledDiskCode + "suffisso");
        }
        private static string dllCode()
        {
            return HwProtection.dllCode(diskCodeMD5());
        }
        private static string diskCode()
        {
            ArrayList hdCollection = new ArrayList();

            ManagementObjectSearcher searcher = new
                ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");

            foreach (ManagementObject wmi_HD in searcher.Get())
            {
                HardDrive hd = new HardDrive();
                hd.Model = wmi_HD["Model"].ToString();
                hd.Type = wmi_HD["InterfaceType"].ToString();
                if (wmi_HD["SerialNumber"] == null)
                    hd.SerialNo = "None";
                else
                    hd.SerialNo = wmi_HD["SerialNumber"].ToString();

                if (hd.Type!="USB")
                    hdCollection.Add(hd);
            }

            searcher = new
                ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMedia");

            foreach (ManagementObject wmi_HD in searcher.Get())
            {
                HardDrive hd = new HardDrive();
                hd.Model = "";// wmi_HD["Model"].ToString();
                hd.Type = "";//wmi_HD["InterfaceType"].ToString();
                if (wmi_HD["SerialNumber"] == null)
                    hd.SerialNo = "None";
                else
                    hd.SerialNo = wmi_HD["SerialNumber"].ToString();

                hdCollection.Add(hd);
            }
            // Display available hard drives
            //foreach (HardDrive hd in hdCollection)
            //{
            //Console.WriteLine("Model\t\t: " + hd.Model);
            //Console.WriteLine("Type\t\t: " + hd.Type);
            //Console.WriteLine("Serial No.\t: " + hd.SerialNo);
            //Console.WriteLine();
            //}
            return (((HardDrive)hdCollection[0]).Model.Trim() + "_" + ((HardDrive)hdCollection[0]).SerialNo.Trim());
        }
        private static string oldDiskCode()
        {
            ArrayList hdCollection = new ArrayList();

            ManagementObjectSearcher searcher = new
                ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");

            foreach (ManagementObject wmi_HD in searcher.Get())
            {
                HardDrive hd = new HardDrive();
                hd.Model = wmi_HD["Model"].ToString();
                hd.Type = wmi_HD["InterfaceType"].ToString();

                hdCollection.Add(hd);
            }

            searcher = new
                ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMedia");

            int i = 0;
            foreach (ManagementObject wmi_HD in searcher.Get())
            {
                HardDrive hd = (HardDrive)hdCollection[i];
                if (wmi_HD["SerialNumber"] == null)
                    hd.SerialNo = "None";
                else
                    hd.SerialNo = wmi_HD["SerialNumber"].ToString();
                ++i;
                break;
            }
            return ((HardDrive)hdCollection[0]).SerialNo;
        }

        private static string diskCode1()
        {
            StringCollection propNames = new StringCollection();
            ManagementClass driveClass = new ManagementClass("Win32_DiskDrive");
            PropertyDataCollection props = driveClass.Properties;
            foreach (PropertyData driveProperty in props)
            {
                propNames.Add(driveProperty.Name);
            }
            string res = "";
            ManagementObjectCollection drives = driveClass.GetInstances();
            foreach (ManagementObject drv in drives)
            {
                foreach (string strProp in propNames)
                {
                    //Label2.Text+=drv[strProp];
                    res = res + strProp + "   =   " + drv[strProp] + "\r\n";
                }
            }
            return res;
        }
        public static bool verifyHdMD5(String codeToVerify)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                return (VerifyMd5Hash(md5Hash, codeToVerify, dllCode()));
            }
        }
        private static string getHdMD5()
        {
            return getHdMD5(diskCode());
        }


        private static readonly byte[] initVectorBytes = Encoding.ASCII.GetBytes("tu89ge6i340t89u2");
        // This constant is used to determine the keysize of the encryption algorithm.
        private const int keysize = 256;
        public static string Encrypt(string plainText)
        {
            string passPhrase = "sintecsalt";
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, null);
            {
                byte[] keyBytes = password.GetBytes(keysize / 8);
                using (RijndaelManaged symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.Mode = CipherMode.CBC;
                    using (ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes))
                    {
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                            {
                                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                                cryptoStream.FlushFinalBlock();
                                byte[] cipherTextBytes = memoryStream.ToArray();
                                return Convert.ToBase64String(cipherTextBytes);
                            }
                        }
                    }
                }
            }
        }

        public static string Decrypt(string cipherText)
        {
            string passPhrase = "sintecsalt";
            byte[] cipherTextBytes = Convert.FromBase64String(cipherText);
            PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, null);
            {
                byte[] keyBytes = password.GetBytes(keysize / 8);
                using(RijndaelManaged symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.Mode = CipherMode.CBC;
                    using(ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes))
                    {
                        using(MemoryStream memoryStream = new MemoryStream(cipherTextBytes))
                        {
                            using(CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                            {
                                byte[] plainTextBytes = new byte[cipherTextBytes.Length];
                                int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                                return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                            }
                        }
                    }
                }
            }
        }
    
        public static String SHAEncrypt(String stringa)
        {
            byte[] keyArray;
            SHA512CryptoServiceProvider hash = new SHA512CryptoServiceProvider();
            keyArray = hash.ComputeHash(UTF8Encoding.UTF8.GetBytes("sintecsalt"));
            byte[] trimmedBytes = new byte[24];
            Buffer.BlockCopy(keyArray, 0, trimmedBytes, 0, 24);
            keyArray = trimmedBytes;
            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.CBC;
            ICryptoTransform ic = tdes.CreateEncryptor();
            byte[] bytes = new byte[stringa.Length * sizeof(char)];
            System.Buffer.BlockCopy(stringa.ToCharArray(), 0, bytes, 0, bytes.Length);
            byte[] enc = ic.TransformFinalBlock(bytes, 0, bytes.Length);
            return (UTF8Encoding.UTF8.GetString(enc));
        }

        public static String SHADecrypt(String stringa)
        {
            byte[] keyArray;
            SHA512CryptoServiceProvider hash = new SHA512CryptoServiceProvider();
            keyArray = hash.ComputeHash(UTF8Encoding.UTF8.GetBytes("sintecsalt"));
            byte[] trimmedBytes = new byte[24];
            Buffer.BlockCopy(keyArray, 0, trimmedBytes, 0, 24);
            keyArray = trimmedBytes;
            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.CBC;
            ICryptoTransform ic = tdes.CreateDecryptor();
            byte[] bytes = new byte[stringa.Length * sizeof(char)];
            System.Buffer.BlockCopy(stringa.ToCharArray(), 0, bytes, 0, bytes.Length);
            byte[] enc = ic.TransformFinalBlock(bytes, 0, bytes.Length);
            return (UTF8Encoding.UTF8.GetString(enc));
        }

        private static string getHdMD5(String untiledDiskCode)
        {
            string source = untiledDiskCode + "sintecsrl";
            using (MD5 md5Hash = MD5.Create())
            {
                string hash = GetMd5Hash(md5Hash, source);

                //Console.WriteLine("The MD5 hash of " + source + " is: " + hash + ".");

                //Console.WriteLine("Verifying the hash...");

                //if (VerifyMd5Hash(md5Hash, source, hash))
                //{
                //    Console.WriteLine("The hashes are the same.");
                //}
                //else
                //{
                //    Console.WriteLine("The hashes are not same.");
                //}

                return hash;
            }
        }

        private static string GetMd5Hash(MD5 md5Hash, string input)
        {

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        public static string getMD5(String input)
        {
            byte[] data = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        // Verify a hash against a string.
        private static bool VerifyMd5Hash(MD5 md5Hash, string input, string hash)
        {
            // Hash the input.
            string hashOfInput = input;

            // Create a StringComparer an compare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hashOfInput, hash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
