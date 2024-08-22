using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace KaraStart
{
    public partial class Settings : Form
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            checkBox1.Checked = Program.mainForm.useRamDisk;
            textBox2.Text = Program.mainForm.ramDiskPath;
            panel1.BackColor = Program.mainForm.secondScreen.textColor;
            numericUpDown1.Value = Program.mainForm.secondScreen.textVel;
            numericUpDown2.Value = Program.mainForm.secondScreen.textSize;
        }

        public void saveRamDiskPath(String path, bool useRamDisk)
        {
            Program.cfg.IniWriteValue("settings","useRamDisk",useRamDisk.ToString());
            Program.cfg.IniWriteValue("settings", "ramDiskPath", textBox2.Text);
            Program.mainForm.useRamDisk = useRamDisk;
            Program.mainForm.ramDiskPath = path;
        }

        private void button33_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = textBox2.Text;
            folderBrowserDialog1.Description = "Scegli la cartella temporanea dove depositare i file da avviare";
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBox2.Text = folderBrowserDialog1.SelectedPath;
                if (!Directory.Exists(textBox2.Text))
                {
                    textBox2.Text = "";
                }
                else
                {
                    saveRamDiskPath(textBox2.Text, true);
                } 
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            textBox2.Enabled = checkBox1.Checked;
            button33.Enabled = checkBox1.Checked;
            if (checkBox1.Checked)
            {
                if (!Directory.Exists(textBox2.Text))
                {
                    textBox2.Text = "";
                }
                else
                {
                    saveRamDiskPath(textBox2.Text, true);
                }
            }
            else
            {
                saveRamDiskPath(textBox2.Text, false);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //.Filter = "Playlist (*.txt)|*.txt";
            folderBrowserDialog1.Description = "Importa una playlista del vecchio Karastart";
            folderBrowserDialog1.SelectedPath  = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + @"\KaraStart\Playlist";
            //folderBrowserDialog1.RootFolder = "";
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                if (MessageBox.Show("Le playliste importate sovrascriranno quelle esistenti, continuare?", "Continuare?", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
                {
                    foreach (String s in Directory.GetFiles(folderBrowserDialog1.SelectedPath, "*.txt"))
                    {
                        FileInfo fi = new FileInfo(s);
                        //if (fi.Name.Replace(".xml", "") != "Default")
                        {

                            List<PlaylistItem> pItem = PlaylistManager.importPlayList(fi.FullName);
                            PlaylistManager.savePlayList(fi.Name.Replace(".txt", ""), pItem);
                        }
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "File di configurazione INI (*.ini)|*.ini";
            openFileDialog1.Title = "Importa il file gi configurazione del vecchio Karastart";
            openFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + @"\KaraStart";
            openFileDialog1.FileName = "Settings.ini";
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                if (MessageBox.Show("Le impostazioni importate sovrascriranno quelle esistenti, continuare?", "Continuare?", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
                {
                    Program.ini = new OrmeSpace.IniFile(openFileDialog1.FileName);
                    String[] activationCode = Program.ini.ReadSection("activation code");
                    String[] midiCommand = Program.ini.ReadSection("midi command");
                    String[] midiDevice = Program.ini.ReadSection("midi device");
                    String[] settings = Program.ini.ReadSection("settings");
                    String[] programs = Program.ini.ReadSection("programs");
                    String[] files = Program.ini.ReadSection("files");
                    String[] windows = Program.ini.ReadSection("windows");
                    foreach (String s in activationCode.ToList<String>())
                    {
                        try { Program.cfg.IniWriteValue("activation code", s.Split('=')[0], s.Split('=')[1]); }
                        catch { }
                    }
                    foreach (String s in midiCommand.ToList<String>())
                    {
                        try { Program.cfg.IniWriteValue("midi command", s.Split('=')[0], s.Split('=')[1]); }
                        catch { }
                    }
                    foreach (String s in midiDevice.ToList<String>())
                    {
                        try { Program.cfg.IniWriteValue("midi device", s.Split('=')[0], s.Split('=')[1]); }
                        catch { }
                    }
                    foreach (String s in settings.ToList<String>())
                    {
                        try { Program.cfg.IniWriteValue("settings", s.Split('=')[0], s.Split('=')[1]); }
                        catch { }
                    }
                    foreach (String s in programs.ToList<String>())
                    {
                        try { Program.cfg.IniWriteValue("programs", s.Split('=')[0], s.Split('=')[1]); }
                        catch { }
                    }
                    foreach (String s in files.ToList<String>())
                    {
                        try { Program.cfg.IniWriteValue("files", s.Split('=')[0], s.Split('=')[1]); }
                        catch { }
                    }
                    foreach (String s in windows.ToList<String>())
                    {
                        try { Program.cfg.IniWriteValue("windows", s.Split('=')[0], s.Split('=')[1]); }
                        catch { }
                    }
                }
            }
        }

        private void panel1_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = panel1.BackColor;
            if (colorDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                panel1.BackColor = colorDialog1.Color;
                Program.mainForm.secondScreen.textColor = colorDialog1.Color;
                Program.cfg.IniWriteValue("settings", "screenTextColor", colorDialog1.Color.ToKnownColor().ToString());
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string url = "http://www.ricecipriani.it/linkgenerator.asp?showAll=plugjack&codice=ramdiskKarastart";
            System.Diagnostics.Process.Start(url);
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            Program.cfg.IniWriteValue("settings", "screenTextVelocity", numericUpDown1.Value.ToString());
            Program.mainForm.secondScreen.textVel = (int)numericUpDown1.Value;
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            Program.cfg.IniWriteValue("settings", "screenTextSize", numericUpDown2.Value.ToString());
            Program.mainForm.secondScreen.textSize = (int)numericUpDown2.Value;
        }

    }
}
