using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Windows.Forms;
using DragNDrop;

namespace KaraStart
{
    public class PlaylistItem
    {
        public string name;
        public string path;
        public double rank;
        public Guid guid = Guid.Empty;
    }
    public class PlaylistManager
    {
        public static String xmlPath = Sintec.Tool.AppSettingsManager.Istance.getAppSettings("PlaylistRooth") + "\\";
        public static Encoding e = System.Text.Encoding.BigEndianUnicode;
        public static List<PlaylistItem> importPlayList(String path)
        {
            List<PlaylistItem> res = new List<PlaylistItem>();
            String[] lines = System.IO.File.ReadAllLines(path, System.Text.Encoding.Default);
            int k =0;
            while (k < lines.Length)
            {
                double rank = 0;
                PlaylistItem i = new PlaylistItem();
                i.name = lines[k++];
                i.path = lines[k++];
                i.rank = rank;
                rank = rank + 0.001;
                i.guid = Guid.Empty;
                res.Add(i);
            }
            return res;
        }
        public static void deletePlayList(String name)
        {
            String xmlFile = PlaylistManager.xmlPath + name + ".xml";
            File.Delete(xmlFile);
        }
        public static void savePlayList(String name, List<PlaylistItem> playlist)
        {
            if (!Directory.Exists(PlaylistManager.xmlPath))
                Directory.CreateDirectory(PlaylistManager.xmlPath);

            String xmlFile = PlaylistManager.xmlPath + name + ".xml";

            XmlWriterSettings settings = new XmlWriterSettings { Indent = true, NewLineOnAttributes = false, OmitXmlDeclaration = true, IndentChars = "\t" };
            XmlWriter xmlWriter = XmlWriter.Create(xmlFile, settings);

            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("KarastartPlaylist");

            xmlWriter.WriteStartElement("applicationInfo");
            xmlWriter.WriteStartElement("Karastart");
            xmlWriter.WriteAttributeString("version", Util.getVersion().Major.ToString() + "." + Util.getVersion().MajorRevision.ToString() + "." + Util.getVersion().Minor);
            xmlWriter.WriteAttributeString("productName", Application.ProductName);
            xmlWriter.WriteAttributeString("companyName", Application.CompanyName);
            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("PlylistItems");
            foreach (PlaylistItem pli in playlist)
            {
                xmlWriter.WriteStartElement("Item");
                xmlWriter.WriteStartElement("FilePath");
                xmlWriter.WriteString(pli.path);
                xmlWriter.WriteEndElement();
                xmlWriter.WriteStartElement("Name");
                xmlWriter.WriteAttributeString("Rank", pli.rank.ToString("0.00"));
                xmlWriter.WriteAttributeString("Guid", pli.guid.ToString());
                xmlWriter.WriteString(pli.name);
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndElement();
            }
            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndDocument();
            xmlWriter.Close();
        }
        public static List<PlaylistItem> openPlayList(String name)
        {
            List<PlaylistItem> res = new List<PlaylistItem>();
            String xmlFile = PlaylistManager.xmlPath + name + ".xml";
            if (File.Exists(xmlFile))
            {
                XmlDocument xmlDoc = new XmlDocument(); //* create an xml document object.
                if (!File.Exists(xmlFile))
                {
                    XmlWriterSettings settings = new XmlWriterSettings { Indent = true, NewLineOnAttributes = false, OmitXmlDeclaration = true, IndentChars = "\t" };
                    XmlWriter xmlWriter = XmlWriter.Create(xmlFile, settings);
                }
                xmlDoc.Load(xmlFile); //* load the XML document from the specified file.

                XmlNodeList appInfo = xmlDoc.GetElementsByTagName("applicationInfo");
                foreach (XmlElement e in appInfo[0].ChildNodes)
                {
                    if (e.Name == "Karastart")
                    {
                        foreach (XmlAttribute a in e.Attributes)
                        {
                            if (a.Name == "version")
                            {
                                if (a.Value != Util.getVersion().Major.ToString() + "." + Util.getVersion().MajorRevision.ToString() + "." + Util.getVersion().Minor)
                                {
                                    //MessageBox.Show("La versione che si sta utilizzando è diversa da quella con cui è stato creto il file, potrebbero esserci problemi nel caricamento dei dati");
                                }
                            }

                        }
                    }
                }
                XmlNodeList workData = xmlDoc.GetElementsByTagName("PlylistItems");
                foreach (XmlElement e in workData[0].ChildNodes)
                {
                    if (e.Name == "Item")
                    {
                        PlaylistItem pli = new PlaylistItem();
                        foreach (XmlElement f in e.ChildNodes)
                        {
                            if (f.Name == "Name")
                            {
                                foreach (XmlAttribute b in f.Attributes)
                                {
                                    if (b.Name == "Rank")
                                        pli.rank = double.Parse("0" + b.Value);
                                    if (b.Name == "Guid")
                                        pli.guid = Guid.Parse(b.Value);
                                }
                                pli.name  = f.InnerText;
                            }
                            if (f.Name == "FilePath")
                            {
                                pli.path = f.InnerText;
                            }
                        }
                        res.Add(pli);
                    }
                }

                //StreamReader fr = new StreamReader(@"./Playlist/" + name + ".txt", e);
                //while (!fr.EndOfStream)
                //{
                //    PlaylistItem i = new PlaylistItem();
                //    String nameAndRank = fr.ReadLine();
                //    i.name = nameAndRank.Split('!')[0];
                //    i.path = fr.ReadLine();
                //    i.rank = 0;// double.Parse(nameAndRank.Split('!')[1]);
                //    res.Add(i);
                //}
                //fr.Close();
            }
            else
            {
                //(File.OpenWrite(@"./Playlist/" + name + ".txt")).Close();
            }
            return res;
        }
    }
    
}
