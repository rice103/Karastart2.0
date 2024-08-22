using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Microsoft.VisualBasic;

namespace KaraStart
{
    public partial class KstForm : Form
    {
        Dictionary<string,string> progNameExeAndName;
        public KstForm()
        {
            InitializeComponent();
        }

        private void KstForm_Load(object sender, EventArgs e)
        {

        }

        public KstResult getMeRes(string fileName)
        {
            FileInfo fi = new FileInfo(fileName);
            this.label1.Text  = fi.Name ;
            progNameExeAndName = Program.mainForm.getProgramEnabling(fi.Extension);
            foreach (KeyValuePair<string, string> k in progNameExeAndName)
            {
                comboBox1.Items.Add(k.Key );
            }
            comboBox1.SelectedIndex = 0;
            textBox2.Text = "0";
            textBox1.Text = "0";
            this.checkBox1.Checked = false;
            this.ShowDialog();
            if (this.Tag.ToString() != "-1")
                return new KstResult(fileName, comboBox1.Text, progNameExeAndName[comboBox1.Text] ,int.Parse(textBox1.Text), int.Parse(textBox2.Text), checkBox1.Checked);
            else
                return null;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Tag = "0";
            this.Hide();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox2.Text = "0";
            textBox2.Visible = comboBox1.Text.ToLower() == "vlc";
            label3.Visible = comboBox1.Text.ToLower() == "vlc";
            this.checkBox1.Visible = comboBox1.Text.ToLower() == "vlc";
            if (comboBox1.Text == "personalizzato")
            {
                openFileDialog1.Filter = "Programma (*.exe)|*.exe";
                openFileDialog1.Multiselect = false;
                openFileDialog1.FileName = "";
                openFileDialog1.Title = "Scegli il programma con cui avviare il file";
                openFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
                DialogResult result = openFileDialog1.ShowDialog();
                String name="";
                if (result == DialogResult.OK)
                {
                    name =openFileDialog1.FileName;
                }
                if (name == "")
                {
                    comboBox1.SelectedIndex = 0;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Tag = "-1";
            this.Hide();
        }

        private void KstForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.Tag == null)
            {
                this.Tag = "-1";
            }
        }
    }
    public class KstResult
    {
        public string fileName;
        public string programName;
        public string progPath;
        public int millisRitardo;
        public int sAnticipo;
        public bool mute;
        public KstResult(string fileName, string programName, string progPath, int millisRitardo, int sAnticipo, bool mute)
        {
            this.fileName = fileName;
            this.programName = programName;
            this.progPath = progPath;
            this.millisRitardo = millisRitardo;
            this.sAnticipo = sAnticipo;
            this.mute = mute;
        }
    }
}
