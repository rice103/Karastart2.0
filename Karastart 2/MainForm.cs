using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using OrmeSpace;
using System.IO;
using Microsoft.VisualBasic;
using NAudio.Wave;
using System.Diagnostics;
using DragNDrop;
using System.IO.Compression;
using System.Timers;
using System.Reflection;
using System.Net.Mail;
using Sintec.Tool;
using MCIDEMO;
using vlcmote;

namespace KaraStart
{
    public partial class MainForm : Form
    {
        bool inibSearch = true;
        bool inibSplittSaving = true;
        private String _mediaSourceForSecondScreen = "";
        public String mediaSourceForSecondScreen
        {
            get
            {
                return _mediaSourceForSecondScreen;
            }
            set
            {
                _mediaSourceForSecondScreen = value;
                secondScreen.bS = null;
            }
        }
        bool endSearch = true;
        BoolBox stopSearch = new BoolBox(false);
        List<Button> buttonList = new List<Button>();
        string dragItemTempFileName = string.Empty;
        private bool itemDragStart = false;
        public int minutsAfterDeleteFromRamdisk = 30;
        //public BoolBox stopSearch = new BoolBox();
        //public bool endSearch = true;
        public Dictionary<Guid, String> startedFile = new Dictionary<Guid, String>();
        public String realPathOfFile;
        public bool useRamDisk = false;
        public String ramDiskPath = "";
        public WaveStream mainOutputStream;
        public WaveChannel32 volumeStream;
        public WaveOutEvent player;
        int indexPlayed = 0;
        bool dragFromOut = false;
        private ListViewColumnSorter Sorter = new ListViewColumnSorter();
        public Queue<string> queueSearch = new Queue<string>();
        public bool firstTime = true;
        bool enableReceiveVolume = true;
        public int lastVolume;
        public Catcher catcher;
        public SecondScreen secondScreen;
        int oldSplitDist = 200;
        List<String> listMp3Back = new List<string>();
        private List<TimerAndTag> timers = new List<TimerAndTag>();
        private System.Timers.Timer m_timer0 = new System.Timers.Timer();
        private System.Timers.Timer m_timer1 = new System.Timers.Timer();
        private System.Timers.Timer m_timer2 = new System.Timers.Timer();
        private System.Timers.Timer m_timer3 = new System.Timers.Timer();
        private System.Timers.Timer m_timer4 = new System.Timers.Timer();
        private System.Timers.Timer m_timer5 = new System.Timers.Timer();
        private System.Timers.Timer m_timer6 = new System.Timers.Timer();
        private System.Timers.Timer m_timer7 = new System.Timers.Timer();
        private System.Timers.Timer m_timer8 = new System.Timers.Timer();
        private System.Timers.Timer m_timer9 = new System.Timers.Timer();
        public FileData[] infoFile = new FileData[2];

        public MainForm()
        {
            InitializeComponent();
            checkBox3.BackColor = Color.Transparent;
            catcher = new Catcher();
            secondScreen = new SecondScreen();
            //stopSearch.value = false;
            timers.Add(new TimerAndTag(m_timer0));
            timers.Add(new TimerAndTag(m_timer1));
            timers.Add(new TimerAndTag(m_timer2));
            timers.Add(new TimerAndTag(m_timer3));
            timers.Add(new TimerAndTag(m_timer4));
            timers.Add(new TimerAndTag(m_timer5));
            timers.Add(new TimerAndTag(m_timer6));
            timers.Add(new TimerAndTag(m_timer7));
            timers.Add(new TimerAndTag(m_timer8));
            timers.Add(new TimerAndTag(m_timer9));
            foreach (TimerAndTag t in timers)
            {
                t.t.Elapsed += new ElapsedEventHandler(Timer_Elapsed);
            }
            buttonList.Add(button4);
            buttonList.Add(button5);
            buttonList.Add(button6);
            buttonList.Add(button7);
            buttonList.Add(button8);
            buttonList.Add(button9);
            buttonList.Add(button10);
            buttonList.Add(button11);
            buttonList.Add(button12);
            buttonList.Add(button13);
            buttonList.Add(button14);
            buttonList.Add(button15);
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            ((System.Timers.Timer)sender).Stop();
            TimerAndTag tt = null;
            foreach (TimerAndTag t in timers)
            {
                if (t.t == ((System.Timers.Timer)sender))
                {
                    tt = t;
                }
            }
            if (tt.tag!=null)
                lauchProgram(tt.tag, tt.path);
            tt.t=null;
        }

        private TimerAndTag getFreeTimer(){
            TimerAndTag tt=null;
            foreach(TimerAndTag t in timers)
            {
                if (t.t == null)
                {
                    t.t = new System.Timers.Timer();
                    t.t.Elapsed += new ElapsedEventHandler(Timer_Elapsed);
                    tt = t;
                    break;
                }
                if (!t.t.Enabled)
                {
                    tt=t;
                    break;
                }
            }
            return tt;
        }

        private void lauchProgram(String s, String path){
#if !DEBUG
            try
#endif
            {
                String time = s.Split('[')[0];
                int timeInMs=int.Parse("0" + time);
                if (timeInMs==0)
                {
                    if (s.IndexOf("[int][pdf]") > -1)
                    {
                        s= "[ext]" + '\u0022' + "Acrord32.exe" + '\u0022' +" " +  s.Substring(10);
                    }
                    //s.Replace("[int][pdf]", "[ext]" + '\u0022' + "Acrord32.exe" + '\u0022');
                    if (s.Contains("[ext]"))
                    {
                        String prog="";
                        String param="";
                        String programAndParams = s.Replace("[ext]", "");
                        int iOfParams= programAndParams.IndexOf('\u0022' + " " + '\u0022');
                        if (iOfParams>0)
                        {
                            prog = programAndParams.Substring(1,iOfParams-1);
                            param = programAndParams.Substring(iOfParams+2);
                        }
                        else
                        {
                            prog = programAndParams.Replace("\u0022","");
                            param = "";                            
                        }
                        //cerca se esiste il programma con cui lanciare il file tra quelli predefiniti
                        foreach (Button b in buttonList)
                        {
                            String nameProg = prog;
                            if (prog.LastIndexOf('\u005c')>0)
                                nameProg = prog.Substring(prog.LastIndexOf('\u005c')+1);
                            if (b.Tag != null && b.Tag.ToString().ToLower().Contains(nameProg.ToLower()))
                            {
                                prog = b.Tag.ToString().Split('?')[0] ;
                                break;
                            }
                        }

                        if (param.Length > 0)
                        {
                            param = param.Substring(1, param.Length - 2);
                            if (!param.Contains("\\"))
                            {
                                param = path + "\\" + param;
                            }
                            if (param.Contains("l="))
                            {
                                param = param.Substring(0, param.IndexOf("l=") - 2);
                            }
                        }
                        else if (!prog.Contains("\\"))
                        {
                            prog = path + "\\" + prog;
                        }
                        System.Diagnostics.Process proc = new System.Diagnostics.Process();
                        proc.EnableRaisingEvents = false;
                        proc.StartInfo.FileName = '\u0022' + prog + '\u0022';
                        proc.StartInfo.Arguments = '\u0022' + param + '\u0022';
                        proc.Start();
                    }
                }
                else
                {
                    TimerAndTag tt = getFreeTimer();
                    if (tt!=null)
                    {
                        tt.tag = s.Substring(time.Length);
                        tt.t.Interval=timeInMs;
                        tt.path = path;
                        tt.t.Enabled=true;
                    }
                }
            }
#if !DEBUG
            catch (Exception e) { (new MailForm(e.Message)).ShowDialog(); }
#endif
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            refreshVisualOfInfoFile();
            button2.Tag = "";
            this.listView2.ListViewItemSorter = Sorter;
            this.listView1.ListViewItemSorter = Sorter;
            this.Left = Screen.PrimaryScreen.Bounds.Width - this.Width;
            this.Top = 0;
            this.Height = int.Parse(Program.cfg.IniReadValue("windows", "MainHeigth", Screen.PrimaryScreen.Bounds.Height.ToString()));
            this.Width = int.Parse(Program.cfg.IniReadValue("windows", "MainWidth", this.Width.ToString()));
            this.Location = new Point(int.Parse(Program.cfg.IniReadValue("windows", "positionL", (Screen.PrimaryScreen.Bounds.Right-this.Width).ToString())),int.Parse(Program.cfg.IniReadValue("windows", "positionT","0")));
            this.splitContainer1.SplitterDistance = (int)((Double)this.splitContainer1.Width / 100.0 * double.Parse(Program.cfg.IniReadValue("windows", "splitterPerc", "50")));
            inibSplittSaving = false;
            readIniBackground();
            readIniProgramSet();
            readIniFolderSet();
            readIniClone();
            String textOnClone = Program.cfg.IniReadValue("settings", "textOnClone", "-1");
            if (textOnClone != "-1")
                textBox5.Text = textOnClone;
            checkBox4.Checked = bool.Parse(Program.cfg.IniReadValue("settings", "textOnCloneOn", "False"));
            mediaSourceForSecondScreen = Program.cfg.IniReadValue("settings", "mediaSourceForSecondScreen", "");

            useRamDisk = bool.Parse(Program.cfg.IniReadValue("settings", "useRamDisk", "False"));
            ramDiskPath = Program.cfg.IniReadValue("settings", "ramDiskPath", "");
            checkObsoleteRamFile();

            ProgramsCheckForButton();

            listView2.Columns.Add("Titolo", 150, HorizontalAlignment.Left);
            listView2.Columns.Add("Ultima modifica", 110, HorizontalAlignment.Left);
            //listView2.Columns.Add("Icon", 5, HorizontalAlignment.Left);
            listView2.Columns.Add("tUltimaModifica", 0);

            listView1.Columns.Add("Titolo", 250, HorizontalAlignment.Left);
            listView1.Columns.Add("☺", 18, HorizontalAlignment.Left);
            //listView1.Columns.Add("Ultima modifica", 200, HorizontalAlignment.Left);
            //listView1.Columns.Add("tUltimaModifica", 0);

            comboBox1.Items.Add("Default");
            comboBox1.Text = "Default";

            this.Show();
            inibSearch = false;

            foreach (String s in Directory.GetFiles(AppSettingsManager.Istance.getAppSettings("PlaylistRooth"), "*.xml"))
            {
                FileInfo fi = new FileInfo(s);
                if (fi.Name.Replace(".xml", "") != "Default")
                    comboBox1.Items.Add(fi.Name.Replace(".xml", ""));
            }

            listView1.AllowDrop = true;
            listView1.DragDrop += new DragEventHandler(listView1_DragDrop);
            listView1.DragEnter += new DragEventHandler(listView1_DragEnter);

            refreshSearch();
            whileLoading = false;
        }


        private void button29_Click(object sender, EventArgs e)
        {
            if (catcher == null)
            {
                catcher = new Catcher();
            }
            if (firstTime)
            {
                catcher.Show();
            }
            firstTime = false;
            if (secondScreen == null)
                secondScreen = new SecondScreen();
            secondScreen.Show();
            secondScreen.StartTimer();

            button29.Visible = false;
            button30.Visible = true;
            button28.Visible = true;
            button2.Visible = true;
        }

        private void readIniBackground(){
            textBox2.Text = Program.cfg.IniReadValue("settings", "backgroundMusicFolder", Environment.GetFolderPath(Environment.SpecialFolder.MyMusic));
            trackBar1.Value = int.Parse(Program.cfg.IniReadValue("settings", "backgroundVolume", ((int)Math.Round(trackBar1.Maximum * 0.30)).ToString()));
            refreshBackMusic();
        }

        private void refreshBackMusic()
        {
            if (player!=null && player.PlaybackState == PlaybackState.Paused)
                player.Stop();
            if (Directory.Exists(textBox2.Text))
                listMp3Back = searchMp3InFolderAndSubfolder(textBox2.Text);
            if (listMp3Back.Count > 0)
            {
                button31.Enabled = true;
                button31.Image = global::KaraStart.Properties.Resources.Play;
                this.button31.Image.Tag = "play";
            }
        }

        private void Play()
        {
            timer6.Enabled = false;
            Application.DoEvents();
            try
            {
                if (player == null)
                {
                    player = new WaveOutEvent();
                    //player.PlaybackStopped += new EventHandler<StoppedEventArgs>(player_PlaybackStopped);
                }
                if (player.PlaybackState == PlaybackState.Stopped)
                {
                    FileInfo fin = new FileInfo(listMp3Back[indexPlayed]);
                    mainOutputStream = new NAudio.Wave.Mp3FileReader(listMp3Back[indexPlayed]);
                    volumeStream = new WaveChannel32(mainOutputStream);
                    indexPlayed = (indexPlayed + 1) % listMp3Back.Count;
                    player.Init(volumeStream);
                    TimeSpan time = mainOutputStream.TotalTime;
                    int millis = (int)time.TotalMilliseconds;
                    timer6.Interval = millis;
                    groupBox1.Text = "Sottofondo - " + fin.Name;
                }
                volumeStream.Volume = 0;
                player.Play();
                timer6.Enabled = true;
                timer3.Tag = "1";
                timer3.Enabled = true;
            }
            catch
            {
                this.button31.Image.Tag = "play";
            }
        }

        private void player_PlaybackStopped(object sender, StoppedEventArgs se)
        {
            if ((player != null && player.PlaybackState == PlaybackState.Stopped) || player == null)
            {
                mainOutputStream = new NAudio.Wave.Mp3FileReader(listMp3Back[indexPlayed]);
                volumeStream = new WaveChannel32(mainOutputStream);
                indexPlayed = (indexPlayed + 1) % listMp3Back.Count;
                player.Init(volumeStream);
            }
            volumeStream.Volume = 0;
            player.Play();
            timer3.Tag = "1";
            timer3.Enabled = true;
        }

        private void Pause()
        {
            timer3.Tag = "-1";
            timer3.Enabled = true;
            timer6.Enabled = false;
        }

        private void Next()
        {
            timer3.Tag = "0";
            timer3.Enabled = true;
        }

        private List<String> searchMp3InFolderAndSubfolder(String folder)
        {
            Random rng = new Random();
            List<String> res= new List<string>();
            foreach (String f in "*.mp3|*.m4a".Split('|').SelectMany(filter => System.IO.Directory.GetFiles(folder, filter, SearchOption.AllDirectories)))
            {
                res.Insert(rng.Next(0, res.Count ),f);
            }
            return res;
        }

        public void writeIniBackground()
        {
            Program.cfg.IniWriteValue("settings", "backgroundMusicFolder", textBox2.Text);
            Program.cfg.IniWriteValue("settings", "backgroundVolume", trackBar1.Value.ToString());
        }

        private void readIniFolderSet(){
            textBox3.Text = Program.cfg.IniReadValue("settings", "finalSearchFolder", Environment.GetFolderPath(Environment.SpecialFolder.Personal));
            textBox1.Text = Program.cfg.IniReadValue("settings", "rootSearchFolder", "C:");
            button17.Text = Program.cfg.IniReadValue("settings", "folder_name_0", "-nd-");
            button17.Tag = Program.cfg.IniReadValue("settings", "folder_path_0", "");
            button18.Text = Program.cfg.IniReadValue("settings", "folder_name_1", "-nd-");
            button18.Tag = Program.cfg.IniReadValue("settings", "folder_path_1", "");
            button16.Text = Program.cfg.IniReadValue("settings", "folder_name_2", "-nd-");
            button16.Tag = Program.cfg.IniReadValue("settings", "folder_path_2", "");
            button19.Text = Program.cfg.IniReadValue("settings", "folder_name_3", "-nd-");
            button19.Tag = Program.cfg.IniReadValue("settings", "folder_path_3", "");
            button20.Text = Program.cfg.IniReadValue("settings", "folder_name_4", "-nd-");
            button20.Tag = Program.cfg.IniReadValue("settings", "folder_path_4", "");
            button21.Text = Program.cfg.IniReadValue("settings", "folder_name_5", "-nd-");
            button21.Tag = Program.cfg.IniReadValue("settings", "folder_path_5", "");
            button22.Text = Program.cfg.IniReadValue("settings", "folder_name_6", "-nd-");
            button22.Tag = Program.cfg.IniReadValue("settings", "folder_path_6", "");
            button23.Text = Program.cfg.IniReadValue("settings", "folder_name_7", "-nd-");
            button23.Tag = Program.cfg.IniReadValue("settings", "folder_path_7", "");
            button24.Text = Program.cfg.IniReadValue("settings", "folder_name_8", "-nd-");
            button24.Tag = Program.cfg.IniReadValue("settings", "folder_path_8", "");
            button25.Text = Program.cfg.IniReadValue("settings", "folder_name_9", "-nd-");
            button25.Tag = Program.cfg.IniReadValue("settings", "folder_path_9", "");
            button26.Text = Program.cfg.IniReadValue("settings", "folder_name_10", "-nd-");
            button26.Tag = Program.cfg.IniReadValue("settings", "folder_path_10", "");
            button27.Text = Program.cfg.IniReadValue("settings", "folder_name_11", "-nd-");
            button27.Tag = Program.cfg.IniReadValue("settings", "folder_path_11", "");
        }

        private void readIniClone()
        {
            int x;
            int y;
            x = int.Parse(Program.cfg.IniReadValue("settings", "CLONE_location_X", int.MinValue.ToString()));
            y= int.Parse(Program.cfg.IniReadValue("settings", "CLONE_location_Y", int.MinValue.ToString()));
            catcher.location=new Point(x,y);
            x= int.Parse(Program.cfg.IniReadValue("settings", "CLONE_size_X", int.MinValue.ToString()));
            y= int.Parse(Program.cfg.IniReadValue("settings", "CLONE_size_Y", int.MinValue.ToString()));
            catcher.size=new Size(x,y);
        }

        public void writeIniClone(Point pos, Size size)
        {
            Program.cfg.IniWriteValue("settings", "CLONE_location_X",pos.X.ToString());
            Program.cfg.IniWriteValue("settings", "CLONE_location_Y",pos.Y.ToString());
            Program.cfg.IniWriteValue("settings", "CLONE_size_X",size.Width.ToString());
            Program.cfg.IniWriteValue("settings", "CLONE_size_Y",size.Height.ToString());
        }

        private void button30_Click(object sender, EventArgs e)
        {
            secondScreen.Hide();
            secondScreen.StopTimer();
            catcher.Hide();
            button29.Visible = true;
            button30.Visible = false;
            button28.Visible = false;
            button2.Visible = false;
        }

        private void button28_Click(object sender, EventArgs e)
        {
            if (catcher.Visible == false)
            {
                catcher.Visible = true;
            }
            else
            {
                catcher.Hide();
            }
        }

        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            int val = (int)Math.Ceiling((double)(vScrollBar1.Maximum - vScrollBar1.Value)/10.0);
            vScrollBar1.Tag = val.ToString();
            if (infoFile[0] != null)
            {
                timer5.Enabled = false;
                Application.DoEvents();
                timer5.Enabled = true;
            }
            SistemVolumChanger.SetVolume(val);
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                lastVolume = SistemVolumChanger.GetVolume();
                this.checkBox2.BackgroundImage = global::KaraStart.Properties.Resources.mute;
                //SistemVolumChanger.SetVolume(0);
            }
            else
            {
                this.checkBox2.BackgroundImage = global::KaraStart.Properties.Resources.unmute;
                //SistemVolumChanger.SetVolume(lastVolume);
            }
            SistemVolumChanger.SetMuteUnmute(checkBox2.Checked);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            if (enableReceiveVolume)
            {
                //vScrollBar1.Value = (int)Math.Ceiling((double)(91.0 / 100.0 * (double)(vScrollBar1.Maximum - (SistemVolumChanger.GetVolume()))));
                vScrollBar1.Value = (vScrollBar1.Maximum - (SistemVolumChanger.GetVolume() * 10));
                checkBox2.Checked = SistemVolumChanger.isMuted();
                vScrollBar1.Tag = SistemVolumChanger.GetVolume();
            }
            if (queueSearch.Count==0) endSearch = true;
            while (queueSearch.Count > 0)
            {
                if (stopSearch.val)
                {
                    stopSearch.val = false;
                    queueSearch.Clear();
                    endSearch = true;
                    break;
                }
                String fName = queueSearch.Dequeue();
                if (listView2.Items.Count < 1000)
                {
                    System.IO.FileInfo fi = new System.IO.FileInfo(fName);
                    listView2.SmallImageList = imageList2;
                    IntPtr hIcon = IntPtr.Zero;
                    String imageKey = "";
                    if (fi.Exists)
                    {
                        IntPtr hImgSmall;
                        SHFILEINFO shinfo = new SHFILEINFO();
                        hImgSmall = Win32.SHGetFileInfo(fName, 0, ref shinfo,
                                                       (uint)Marshal.SizeOf(shinfo),
                                                        Win32.SHGFI_ICON |
                                                        Win32.SHGFI_SMALLICON);
                        hIcon = shinfo.hIcon;
                        imageKey = fName;
                        if (hIcon != IntPtr.Zero)
                        {
                            System.Drawing.Icon myIcon = System.Drawing.Icon.FromHandle(hIcon);
                            imageList2.Images.Add(fName, myIcon);
                        }
                    }
                    else
                    {
                        imageKey = "exlamation";
                    }//Add file name and icon to listview
                    ListViewItem item = new ListViewItem(fi.Name, imageKey);
                    item.Name = fName;
                    item.SubItems.Add(fi.LastWriteTime.ToShortDateString() + " " + fi.LastWriteTime.ToShortTimeString());
                    item.SubItems.Add(fi.LastWriteTimeUtc.Ticks.ToString());
                    listView2.Items.Add(item);
                }
                Application.DoEvents();
            }
            timer1.Enabled = true;
        }

        private void vScrollBar1_MouseEnter(object sender, EventArgs e)
        {
            enableReceiveVolume = false;
        }

        private void vScrollBar1_MouseLeave(object sender, EventArgs e)
        {
            enableReceiveVolume = true;
        }

        private void button33_Click(object sender, EventArgs e)
        {
            if (this.button31.Image.Tag!=null && this.button31.Image.Tag.ToString() == "pause")
            {
                button31.PerformClick();
                button33.Tag = "1";
            }
            else
            {
                folderBrowserDialog1.SelectedPath = textBox2.Text;
                folderBrowserDialog1.Description = "Seleziona la cartella contenente i file audio di sottofondo";

                DialogResult result = folderBrowserDialog1.ShowDialog();
                if (result == DialogResult.OK)
                {
                    textBox2.Text = folderBrowserDialog1.SelectedPath;
                    writeIniBackground();
                    refreshBackMusic();
                }
            }
        }

        private void textBox2_DoubleClick(object sender, EventArgs e)
        {
            button33.PerformClick();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = textBox1.Text; 
            folderBrowserDialog1.Description = "Scegli la cartella contenente TUTTE le basi";
            
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBox1.Text = folderBrowserDialog1.SelectedPath;
                Program.cfg.IniWriteValue("settings", "rootSearchFolder", textBox1.Text);
            }
        }

        private void textBox1_DoubleClick(object sender, EventArgs e)
        {
            button3.PerformClick();
        }

        private void button34_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = textBox3.Text; 
            folderBrowserDialog1.Description = "Scegli la cartella dove effettuare la ricerca";
            
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBox3.Text = folderBrowserDialog1.SelectedPath;
                Program.cfg.IniWriteValue("settings", "finalSearchFolder", textBox3.Text);
            }
        }

        private void textBox3_DoubleClick(object sender, EventArgs e)
        {
            button34.PerformClick();
        }

        private void refreshSearch()
        {
            if (inibSearch) return;
            if (!endSearch)
            {
                stopSearch.val = true;
                timer1_Tick(null, null);
            }
            listView2.Items.Clear();
            imageList2.Images.Clear();
            System.Drawing.Icon myIcon = System.Drawing.Icon.FromHandle(global::KaraStart.Properties.Resources.exclamation.GetHicon());
            imageList2.Images.Add("exlamation", myIcon);
            int numFile; int numDir;
            endSearch = false;
            Kernel32.searchIn(textBox4.Text, textBox3.Text, checkBox1.Checked, ref queueSearch, out numFile, out numDir, stopSearch);
        }
        private void textBox4_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyValue == 13)
            {
                refreshSearch();
            }
        }

        private void listView2_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            ListView myListView = (ListView)sender;
            int colCliked = e.Column;
            if (colCliked==1) colCliked=2;

            if (Sorter.SortColumn == colCliked)
            {
                if (myListView.Sorting == SortOrder.Ascending)
                    myListView.Sorting = SortOrder.Descending;
                else
                    myListView.Sorting = SortOrder.Ascending;
            }
            else
            {
                myListView.Sorting = SortOrder.Descending;
            }

            Sorter.SortColumn = colCliked;
            myListView.Sort();
        }

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            ListView myListView = (ListView)sender;
            int colCliked = e.Column;

            if (Sorter.SortColumn == colCliked)
            {
                if (myListView.Tag!=null && (SortOrder)myListView.Tag == SortOrder.Ascending)
                    myListView.Sorting = SortOrder.Descending;
                else
                    myListView.Sorting = SortOrder.Ascending;
            }
            else
            {
                myListView.Sorting = SortOrder.Descending;
            }
            myListView.Tag = myListView.Sorting;

            Sorter.SortColumn = colCliked;
            myListView.Sort();
            myListView.Sorting = SortOrder.None;
        }

        private void updatePlaylist(){
            List<PlaylistItem> list = new List<PlaylistItem>();
            for (int i = 0; i < listView1.Items.Count; i++)
            {
                PlaylistItem p = new PlaylistItem();
                p.rank = i * 0.001;
                p.path = relativePath(listView1.Items[i].Name);
                p.name = listView1.Items[i].Text;
                p.guid = listView1.Items[i].SubItems[0].Tag == null ? Guid.Empty : Guid.Parse(listView1.Items[i].SubItems[0].Tag.ToString());
                list.Add(p);
            }
            PlaylistManager.savePlayList(comboBox1.Text, list);
        }

        private void button37_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                openFileDialog1.Filter = "Playlist (*.txt)|*.txt";
                openFileDialog1.Title = "Importa una playlista del vecchio Karastart";
                openFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + @"\KaraStart\Playlist";
                openFileDialog1.FileName = "";
                DialogResult result = openFileDialog1.ShowDialog();
                if (result == DialogResult.OK)
                {
                    List<PlaylistItem> pItem = PlaylistManager.importPlayList(openFileDialog1.FileName);
                    PlaylistManager.savePlayList(openFileDialog1.SafeFileName.Replace(".txt",""), pItem);
                }
            }
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            writeIniBackground();
        }

        void listView1_DragEnter(object sender, DragEventArgs e)
        {
            if (privateDrag)
            { 
                e.Effect = e.AllowedEffect; 
            }
            else
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        void listView1_DragDrop(object sender, DragEventArgs e)
        {
            if (!privateDrag)
            {
                if (e.Data.GetData("FileNameW") != null && e.Data.GetData("FileNameW").ToString() != "")
                {
                    dragFromOut = true;
                    String[] vect = (String[])e.Data.GetData("FileNameW");
                    for (int k = 0; k < vect.Length; k++)
                    {
                        refreshInformationOfFile(vect[k]);
                        addToPlaylistAnItem(vect[k]);
                    }
                    updatePlaylistWithTimout();
                    dragFromOut = false;
                }
            }
        }

        bool inibhiteReorder = false;

        private void addToPlaylistAnItem(String itemFullName)
        {
            String resFile = itemFullName;
            if (File.Exists(itemFullName))
            {
                FileInfo fe = new FileInfo(itemFullName);
                if (fe.Extension.ToLower() == ".lnk")
                    resFile = LinkResolver.ResolveShortcut(itemFullName);
                if (File.Exists(resFile))
                {
                    ListViewItem iExist = null;
                    foreach (ListViewItem i in listView1.Items)
                    {
                        //if (infoFile[1]!=null && infoFile[1].
                        if (i.SubItems[0].Tag!=null && i.SubItems[0].Tag.ToString() == infoFile[1].idFile.ToString())
                        {
                            iExist = i;
                            break;
                        }
                    }
                    if (iExist!=null) //listView1.Items.ContainsKey(relativePath(resFile)))
                    {
                        MessageBox.Show("Il file che si vuole aggiungere già esiste nella playlist");
                        listView1.SelectedIndices.Clear();
                        listView1.SelectedIndices.Add(iExist.Index);
                        listView1.Focus();
                    }
                    else
                    {
                        FileInfo fi = new FileInfo(resFile);
                        IntPtr hImgSmall;
                        SHFILEINFO shinfo = new SHFILEINFO();

                        listView1.SmallImageList = imageList1;
                        hImgSmall = Win32.SHGetFileInfo(fi.FullName, 0, ref shinfo,
                                                       (uint)Marshal.SizeOf(shinfo),
                                                        Win32.SHGFI_ICON |
                                                        Win32.SHGFI_SMALLICON);
                        System.Drawing.Icon myIcon =
                               System.Drawing.Icon.FromHandle(shinfo.hIcon);
                        imageList1.Images.Add(fi.FullName,myIcon);

                        //Add file name and icon to listview
                        ListViewItem item = new ListViewItem();
                        item.Name = relativePath(fi.FullName);
                        item.Text = fe.Name;
                        item.ImageKey = fi.FullName;
                        Guid res = Kernel32.GuidMd5SumByProcess(fi.FullName);
                        item.SubItems[0].Tag = res;
                        FileData f = Database.fileDataOf(res);
                        item.SubItems.Add((5.0 * f.rating).ToString("0"));
                        item.SubItems.Add(fi.LastWriteTimeUtc.Ticks.ToString());
                        if (startedFile.Count > 0 && startedFile.Keys.Last<Guid>() == res)
                        {
                            item.ForeColor = Color.Green;
                        }
                        else if (f.lastStartDate.AddHours(24).Subtract(DateTime.Now).TotalDays > 0)
                        {
                            item.ForeColor = Color.Blue;
                        }

                        listView1.Items.Add(item);
                        //item.SubItems.Add(fi.LastWriteTime.ToShortDateString() + " " + fi.LastWriteTime.ToShortTimeString());
                        //item.SubItems.Add(fi.LastWriteTimeUtc.Ticks.ToString());
                        updatePlaylistWithTimout();
                    }
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadPlaylistInListView();
            
        }

        private void loadPlaylistInListView()
        {
            imageList1.Images.Clear();
            listView1.Items.Clear();
            System.Drawing.Icon myIcon = System.Drawing.Icon.FromHandle(global::KaraStart.Properties.Resources.exclamation_red.GetHicon());
            imageList1.Images.Add("exlamation", myIcon);
            foreach (PlaylistItem p in PlaylistManager.openPlayList(comboBox1.Text))
            {
                if (File.Exists(veryPath(p.path)))
                {
                    FileInfo fi = new FileInfo(veryPath(p.path));
                    IntPtr hImgSmall;
                    SHFILEINFO shinfo = new SHFILEINFO();
                    String keyImage = "";
                    listView1.SmallImageList = imageList1;
                    hImgSmall = Win32.SHGetFileInfo(fi.FullName, 0, ref shinfo,
                                                    (uint)Marshal.SizeOf(shinfo),
                                                    Win32.SHGFI_ICON |
                                                    Win32.SHGFI_SMALLICON);
                    myIcon = System.Drawing.Icon.FromHandle(shinfo.hIcon);
                    imageList1.Images.Add(fi.FullName, myIcon);
                    keyImage = fi.FullName;
                    //Add file name and icon to listview

                    ListViewItem item = new ListViewItem();
                    item.Name = fi.FullName;
                    item.Text = p.name;
                    item.ImageKey = fi.FullName;
                    Guid res = p.guid;
                    if (p.guid == Guid.Empty)
                        res = Kernel32.GuidMd5SumByProcess(fi.FullName);
                    item.SubItems[0].Tag = res.ToString();
                    FileData f = Database.fileDataOf(res);
                    item.SubItems.Add((5.0 * f.rating).ToString("0"));
                    item.SubItems.Add(fi.LastWriteTimeUtc.Ticks.ToString());
                    if (startedFile.Count > 0 && startedFile.Keys.Last<Guid>() == res)
                    {
                        item.ForeColor = Color.Green;
                    }
                    else if (f.lastStartDate.AddHours(24).Subtract(DateTime.Now).TotalDays > 0)
                    {
                        item.ForeColor = Color.Blue;
                    }
                    listView1.Items.Add(item);
                }
                else
                {
                    ListViewItem item = listView1.Items.Add(p.path, p.name, "exlamation");
                    //item.SubItems.Add("0");
                    item.ForeColor = Color.Gray;
                }
            }
        }

        private void button37_Click(object sender, EventArgs e)
        {
            String name ="-rice";
            name = Interaction.InputBox("Inserisci il nome della Playlist", "Nuova playlist");
            if (name != "")
            {
                comboBox1.Items.Add(name);
                comboBox1.Text = name;
                imageList1.Images.Clear();
                listView1.Items.Clear();
                updatePlaylistWithTimout();
            }
        }

        private void button38_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text == "Default")
            {
                MessageBox.Show("Non si può cancellare la playlist predefinita");
            }
            else
            {
                if (MessageBox.Show("Vuoi veramente canellare la playlist " + comboBox1.Text + "?", "Cancella la playlist", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    imageList1.Images.Clear();
                    listView1.Items.Clear();
                    PlaylistManager.deletePlayList(comboBox1.Text);
                    comboBox1.Items.Remove(comboBox1.Text);
                    comboBox1.Text = "Default";
                    loadPlaylistInListView();
                }
            }
        }

        bool privateDrag;
        private void listView1_ItemDrag(object sender, ItemDragEventArgs e)
        {
            if (MouseButtons == System.Windows.Forms.MouseButtons.Left)
            {
                if (e.Button == MouseButtons.None)
                    return;
                privateDrag = true;
                ClearDragData();
                Program.objDragItem = veryPath(listView1.SelectedItems[0].Name);
                itemDragStart = true;
                //DoDragDrop(e.Item, DragDropEffects.Copy);

                if (itemDragStart && Program.objDragItem != null)
                {
                    FileInfo fi = new FileInfo(veryPath(listView1.SelectedItems[0].Name));
                    dragItemTempFileName = string.Format("{0}{1}{2}", Path.GetTempPath(), "", fi.Name);
                    try
                    {
                        //Util.CreateDragItemTempFile(dragItemTempFileName,fi.FullName);

                        string[] fileList = new string[] { fi.FullName };
                        DataObject fileDragData = new DataObject(DataFormats.FileDrop, fileList);
                        DoDragDrop(fileDragData, DragDropEffects.All);

                        ClearDragData();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "DragNDrop Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                privateDrag = false;
            }
        }

        private void listView1_DragOver(object sender, DragEventArgs e)
        {
            if (!dragFromOut && privateDrag)// && e.Data.GetData("FileNameW") == null)
            {
                var pos = listView1.PointToClient(new Point(e.X, e.Y));
                var hit = listView1.HitTest(pos);
                if (hit.Item != null)
                {
                    try
                    {
                        var dragItem = listView1.Items[((String[])e.Data.GetData("FileNameW"))[0]];
                        hit.Item.Tag = dragItem;
                        int k = hit.Item.Index;
                        if (dragItem.Index != k)
                        {
                            listView1.BeginUpdate();
                            listView1.Items.RemoveAt(dragItem.Index);
                            listView1.Items.Insert(k, dragItem);
                            updatePlaylistWithTimout();
                            listView1.EndUpdate();
                        }
                    }
                    catch { }
                }
            }
        }

        private void updatePlaylistWithTimout()
        {
            timer2.Enabled = false;
            timer2.Interval = 500;
            timer2.Enabled = true;
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            timer2.Enabled = false;
            updatePlaylist();
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void listView2_DoubleClick(object sender, EventArgs e)
        {
            if (listView2.SelectedItems.Count > 0)
            {
                if (button1.Text == "<")
                {
                    addToPlaylistAnItem(listView2.SelectedItems[0].Name);
                }
                else
                {
                    if (button15.Enabled)
                        startFile(button15);
                }
            }
        }

        private void startFile(Button b)
        {
            if (this.button31.Image.Tag!=null && this.button31.Image.Tag.ToString() == "pause")
                pauseBack();
#if !DEBUG
            try
#endif
            {
                String programForInfoFile = "";
                label4.Text = lblFileName.Text;
                Guid id = (lblFileName.Tag!=null)? refreshInfoFileIn(0, veryPath(lblFileName.Tag.ToString())):Guid.Empty;
                String program = "";
                String vPath = "";
                if (lblFileName.Tag != null && lblFileName.Tag.ToString() != "")
                {
                    SistemVolumChanger.SetVolume((int)Math.Round(infoFile[0].volume));
                    String fName = lblFileName.Tag.ToString();
                    if (File.Exists(veryPath(fName)))
                    {
                        vPath = veryPath(fName);
                    }
                }
                if (b == button15)
                {
                    program = vPath;
                    vPath = "";
                    //se trovi tra i button tag il programma ricavato dall'infofile
                    Button bFound = null;
                    if (infoFile[0].programNameLastStart.ToLower() == "karastart.exe")
                    {
                        bFound = button14;
                    }
                    else
                    {
                        foreach (Button bTemp in buttonList)
                        {
                            if (bTemp.Tag!=null && (bTemp.Tag.ToString().Split('?')[0]).Split('\\').Last().ToLower() == infoFile[0].programNameLastStart.ToLower())
                            {
                                bFound = bTemp;
                                break;
                            }
                        }
                    }
                    if (bFound!=null)
                    {
                        startFile(bFound);
                        return;
                    }
                }
                else if (b != button14)
                {
                    program = '\u0022' + b.Tag.ToString().Split('?')[0] + '\u0022';
                    programForInfoFile = b.Tag.ToString().Split('?')[0].Split('\\').Last();
                }

                if (b == button4)
                {
                    if (Program.usingVlcMote)
                    {
                        VlcRemote vlcR = new VlcRemote();
                        vlcR.Add(vPath);
                        vlcR.Play();
                        vlcR.Volume(checkBox3.Checked ? 100 : 0);
                    }
                    else
                    {
                        double ratio = 1.0;
                        if (vPath != "")
                        {
                            vPath = vPath + '\u0022' + " " + '\u0022';
                            //Size s = JockerSoft.Media.FrameGrabber.GetVideoSize(vPath);
                            //if (s.Height > 0 && s.Width > 0)
                            //{
                            //    ratio = 1.0;
                            //    if (((double)s.Width / (double)catcher.Width) > ((double)s.Height / (double)catcher.Height))
                            //    {
                            //        ratio = (double)catcher.Width / (double)s.Width;
                            //    }
                            //    else if (((double)s.Width / (double)catcher.Width) < ((double)s.Height / (double)catcher.Height))
                            //    {
                            //        ratio = (double)catcher.Height / (double)s.Height;
                            //    }
                            //    //                         vv ratio
                            //    vPath = vPath + "--zoom=" + (1.0).ToString("0.0") + '\u0022' + " " + '\u0022' + "--autoscale" + '\u0022' + " " + '\u0022';
                            //}
                            //else
                            {
                                vPath = vPath + "--zoom=" + (1.0).ToString("0.0") + '\u0022' + " " + '\u0022' + "--autoscale" + '\u0022' + " " + '\u0022';
                            }
                        }
                        vPath = vPath + "--width=" + (int)Math.Round(catcher.Width * 1.0) + '\u0022' + " " + '\u0022' + "--height=" + (int)Math.Round(catcher.Height * 1.0) + '\u0022' + " " + '\u0022' +
                           "--qt-max-volume=300";
                        if (checkBox3.Checked)
                        {
                            vPath = vPath + '\u0022' + " " + '\u0022' + "--no-audio";
                        }
                    }
                }
                if ((b != button14) && !Program.usingVlcMote)
                {
                    vPath = '\u0022' + vPath + '\u0022';
                    //MessageBox.Show(vPath);

                    System.Diagnostics.Process proc = new System.Diagnostics.Process();
                    proc.EnableRaisingEvents = false;
                    proc.StartInfo.FileName = program;
                    proc.StartInfo.Arguments = vPath;
                    proc.Start();
                }
                else
                {
                    // riga per riga fai launch..
                    FileInfo fi = new FileInfo(vPath);
                    String dir = fi.Directory.FullName;
                    if (useRamDisk && realPathOfFile.ToLower().EndsWith(".kst"))
                    {
                        dir = (new FileInfo(realPathOfFile)).Directory.FullName;
                    }
                    StreamReader sr = new StreamReader(vPath);
                    try
                    {
                        while (!sr.EndOfStream)
                        {
                            String r = sr.ReadLine();
                            lauchProgram(r, dir);
                        }
                    }
                    catch { }
                    sr.Close();
                }
                if (id != Guid.Empty)
                {
                    if (startedFile.ContainsKey(id))
                        startedFile.Remove(id);
                    startedFile.Add(id, realPathOfFile);
                    saveStartedInfoFile(id);
                    if (b == button14)
                        saveProgramNameLastStartFile("karastart.exe", id);
                    else if (b.Tag == null)
                        saveProgramNameLastStartFile("", id);
                    else
                        saveProgramNameLastStartFile(b.Tag.ToString().Split('?')[0].Split('\\').Last(), id);
                }
                if (listView1.TopItem != null && listView1.SelectedItems.Count > 0)
                {
                    ListViewItem lvi = listView1.TopItem;
                    ListViewItem lvs = listView1.SelectedItems[0];
                    loadPlaylistInListView();
                    foreach (ListViewItem l in listView1.Items)
                    {
                        if (l.Name == lvi.Name)
                            listView1.TopItem = l;
                        if (l.Name == lvs.Name)
                            l.Selected = true;
                    }
                    listView1.Select();
                }
            }
#if !DEBUG
            catch (Exception e) { (new MailForm(e.Message)).ShowDialog(); }
#endif
        }

        private string veryPath(String fPath)
        {
            String veryPath = fPath;
            if (fPath.StartsWith("\\"))
            {
                veryPath = (textBox1.Text + veryPath).Replace(@"\\", @"\");
            }
            return veryPath;
        }

        private void listView1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                if (e.KeyValue == 13 && new FileInfo(veryPath(listView1.SelectedItems[0].Name)).Exists)
                {
                    startFile(button15);
                }
                else if (e.KeyValue == 46)
                {
                    listView1.Items.RemoveByKey(listView1.SelectedItems[0].Name);
                    updatePlaylistWithTimout();
                    //loadPlaylistInListView();
                }
            }
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0 && new FileInfo(veryPath(listView1.SelectedItems[0].Name)).Exists)
            {
                if (button15.Enabled)
                    startFile(button15);
            }
        }

        private void listView2_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (listView2.SelectedItems.Count > 0)
            {
                if (e.KeyValue == 13)
                {
                    addToPlaylistAnItem(listView2.SelectedItems[0].Name);
                }
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            Program.cfg.IniWriteValue("settings", "finalSearchFolder", textBox3.Text);
            refreshSearch();
        }

        private void listView1_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count>0)
            {
                refreshInformationOfFile(veryPath(listView1.SelectedItems[0].Name));
            }
        }

        private void listView2_Click(object sender, EventArgs e)
        {
            if (listView2.SelectedItems.Count > 0)
            {
                refreshInformationOfFile(listView2.SelectedItems[0].Name);
            }
        }

        private void refreshInformationOfFile(String fName)
        {
            if (File.Exists(fName))
            {
                FileInfo fi = new FileInfo(fName);
                refreshInformationOfFile(fName, fi.Name);
            }
        }

        private void refreshInformationOfFile(String fName, string toShowInLabel)
        {
            UseWaitCursor = true;
            Application.DoEvents();
            if (fName != realPathOfFile) // se ho già cliccato lo stesso file salto
            {
                if (File.Exists(fName))
                {
                    FileInfo fi = new FileInfo(fName);
                    String finalPathofZipOrKstUnpacked = fName;
                    if (fi.Extension.ToLower() == ".zip")
                    {
                        if (useRamDisk)
                        {
                            finalPathofZipOrKstUnpacked = addFileToRamDisk(fName);
                        }
                        else
                        {
                            finalPathofZipOrKstUnpacked = unpackedKst(fName);
                        }
                        realPathOfFile = fName;
                        lblFileName.Text = toShowInLabel.Replace((fi.Extension), "");
                        fi = new FileInfo(finalPathofZipOrKstUnpacked);
                        lblFileName.Tag = fi.FullName;
                        lblExt.Text = fi.Extension.Replace(".", "");
                        fName = finalPathofZipOrKstUnpacked;
                    }
                    else
                    {
                        lblFileName.Text = toShowInLabel.Replace((fi.Extension), "");
                        lblFileName.Tag = fName;
                        if (useRamDisk)
                        {
                            lblFileName.Tag = addFileToRamDisk(fName);
                        }
                        realPathOfFile = fName;
                        lblExt.Text = fi.Extension.Replace(".", "");
                    }
                    setProgramEnabling(fName);
                }
            }
            UseWaitCursor = false;
            Application.DoEvents();
            refreshInfoFileIn(1, fName);
            label7.Text = lblFileName.Text;
        }

        private Guid refreshInfoFileIn(int started0OrSelected1, string fileName)
        {
            Guid res = Kernel32.GuidMd5SumByProcess(fileName);
            infoFile[started0OrSelected1] = Database.fileDataOf(res);
            refreshVisualOfInfoFile();
            return res;
        }

        private void refreshVisualOfInfoFile()
        {
            if (infoFile[0] == null)
            {
                label4.Text = "";
                label5.Text = "";
                label6.Text = "";
                rate0_0.BorderStyle = BorderStyle.None;
                rate0_1.BorderStyle = BorderStyle.None;
                rate0_2.BorderStyle = BorderStyle.None;
                rate0_3.BorderStyle = BorderStyle.None;
                rate0_4.BorderStyle = BorderStyle.None;
            }
            else
            {
                label5.Text = "vol: " + infoFile[0].volume.ToString() + ", lanciato " + infoFile[0].usedNumberTimes + " volte";
                String with = "";
                with = infoFile[0].programNameLastStart;
                if (infoFile[0].lastStartDate != DateTime.MinValue)
                    label6.Text = "il " + infoFile[0].lastStartDate.ToShortDateString();
                else
                    label6.Text = "mai lanciato prima";
                if (with != "")
                    label6.Text = label6.Text + " con " + with;
                rate0_0.BorderStyle = BorderStyle.None;
                rate0_1.BorderStyle = BorderStyle.None;
                rate0_2.BorderStyle = BorderStyle.None;
                rate0_3.BorderStyle = BorderStyle.None;
                rate0_4.BorderStyle = BorderStyle.None;
                if (Math.Round(infoFile[0].rating/0.2)==0)
                    rate0_0.BorderStyle = BorderStyle.FixedSingle;
                if (Math.Round(infoFile[0].rating / 0.2) == 1)
                    rate0_1.BorderStyle = BorderStyle.FixedSingle;
                if (Math.Round(infoFile[0].rating / 0.2) == 2)
                    rate0_2.BorderStyle = BorderStyle.FixedSingle;
                if (Math.Round(infoFile[0].rating / 0.2) == 3)
                    rate0_3.BorderStyle = BorderStyle.FixedSingle;
                if (Math.Round(infoFile[0].rating / 0.2) == 4)
                    rate0_4.BorderStyle = BorderStyle.FixedSingle;

            }
            if (infoFile[1] == null)
            {
                label7.Text = "";
                label8.Text = "";
                label9.Text = "";
                rate0_0.BorderStyle = BorderStyle.None;
                rate0_1.BorderStyle = BorderStyle.None;
                rate0_2.BorderStyle = BorderStyle.None;
                rate0_3.BorderStyle = BorderStyle.None;
                rate0_4.BorderStyle = BorderStyle.None;
            }
            else
            {
                String with = "";
                with = infoFile[1].programNameLastStart; 
                label8.Text = "vol: " + infoFile[1].volume.ToString() + ", lanciato " + infoFile[1].usedNumberTimes + " volte";
                if (infoFile[1].lastStartDate != DateTime.MinValue)
                    label9.Text = "il " + infoFile[1].lastStartDate.ToShortDateString();
                else
                    label9.Text = "mai lanciato prima";
                if (with != "")
                    label9.Text = label9.Text + " con " + with;
                rate1_0.BorderStyle = BorderStyle.None;
                rate1_1.BorderStyle = BorderStyle.None;
                rate1_2.BorderStyle = BorderStyle.None;
                rate1_3.BorderStyle = BorderStyle.None;
                rate1_4.BorderStyle = BorderStyle.None;
                if (Math.Round(infoFile[1].rating / 0.2) == 0)
                    rate1_0.BorderStyle = BorderStyle.FixedSingle;
                if (Math.Round(infoFile[1].rating / 0.2) == 1)
                    rate1_1.BorderStyle = BorderStyle.FixedSingle;
                if (Math.Round(infoFile[1].rating / 0.2) == 2)
                    rate1_2.BorderStyle = BorderStyle.FixedSingle;
                if (Math.Round(infoFile[1].rating / 0.2) == 3)
                    rate1_3.BorderStyle = BorderStyle.FixedSingle;
                if (Math.Round(infoFile[1].rating / 0.2) == 4)
                    rate1_4.BorderStyle = BorderStyle.FixedSingle;
            }
        }

        private void saveRatingInfoFile(double rating, Guid id)
        {
            Database.saveRating(id, rating);
            if (infoFile[0]!= null && infoFile[0].idFile == id)
            {
                infoFile[0].rating = (float)rating;
            }
            if (infoFile[1]!= null && infoFile[1].idFile == id)
            {
                infoFile[1].rating = (float)rating;
            }
            refreshVisualOfInfoFile();
        }

        private void saveStartedInfoFile(Guid id)
        {
            DateTime now = DateTime.Now;
            long startedTimes = Database.saveLastStarting(id, now );
            if (infoFile[0]!= null && infoFile[0].idFile == id)
            {
                infoFile[0].lastStartDate = now;
                infoFile[0].usedNumberTimes = startedTimes;
            }
            if (infoFile[1]!= null && infoFile[1].idFile == id)
            {
                infoFile[1].lastStartDate = now;
                infoFile[1].usedNumberTimes = startedTimes;
            }
            refreshVisualOfInfoFile();
        }

        private void saveProgramNameLastStartFile(string progName, Guid id)
        {
            Database.saveProgramName(id, progName);
            if (infoFile[0]!= null && infoFile[0].idFile == id)
            {
                infoFile[0].programNameLastStart = progName;
            }
            if (infoFile[1]!= null && infoFile[1].idFile == id)
            {
                infoFile[1].programNameLastStart = progName;
            }
            refreshVisualOfInfoFile();
        }

        private void saveVolumeInfoFile(Guid id)
        {
            Database.saveVolume(id, float.Parse(vScrollBar1.Tag.ToString()));
            if (infoFile[0]!= null && infoFile[0].idFile == id)
            {
                infoFile[0].volume = float.Parse(vScrollBar1.Tag.ToString());
            }
            if (infoFile[1]!= null && infoFile[1].idFile == id)
            {
                infoFile[1].volume = float.Parse(vScrollBar1.Tag.ToString());
            }
            refreshVisualOfInfoFile();
        }

        private string unpackedKst(String realPath)
        {
            String name = realPath;
            if (name.ToLower().EndsWith(".zip"))
            {
                ZipStorer zi = ZipStorer.Open(realPath, FileAccess.Read);
                bool existKst = false;
                foreach (ZipStorer.ZipFileEntry ze in zi.ReadCentralDir())
                {
                    if (ze.FilenameInZip.ToLower().EndsWith(".kst"))
                    {
                        existKst = true;
                        break;
                    }
                }
                if (existKst)
                {
                    FileInfo fo= new FileInfo(realPath);
                    string dir = GetTemporaryDirectory() + "\\" + fo.Name.Replace(fo.Extension, "") ;
                    if (Directory.Exists(dir))
                    {
                        Directory.Delete(dir, true);
                    }
                    Directory.CreateDirectory(dir);
                    foreach (ZipStorer.ZipFileEntry ze in zi.ReadCentralDir())
                    {
                        zi.ExtractFile(ze, dir + "\\" + ze.FilenameInZip);
                        if (ze.FilenameInZip.ToLower().EndsWith(".kst"))
                        {
                            name = dir + "\\" + ze.FilenameInZip;
                        }
                    }
                }
            }
            return name;
        }

        public string addFileToRamDisk(String realPath)
        {
            String name = realPath;
            try
            {
                // copia nella ramdisk il file
                name = (ramDiskPath + "\\" + (new FileInfo(realPath)).Name).Replace("\\\\", "\\");
                if (File.Exists(name))
                {
                    FileInfo fi = new FileInfo(name);
                    name = name.Replace(fi.Extension, "_" + DateTime.Now.Millisecond + fi.Extension);
                }
                try
                {
                    File.Copy(realPath, name, true);
                }
                catch
                {
                    name = realPath;
                }
                if (name.ToLower().EndsWith(".zip"))
                {
                    ZipStorer zi = ZipStorer.Open(realPath, FileAccess.Read);
                    bool existKst = false;
                    foreach (ZipStorer.ZipFileEntry ze in zi.ReadCentralDir())
                    {
                        if (ze.FilenameInZip.ToLower().EndsWith(".kst"))
                        {
                            existKst = true;
                            break;
                        }
                    }
                    if (existKst)
                    {
                        FileInfo fo = new FileInfo(realPath);
                        string dir = ramDiskPath + "\\" + fo.Name.Replace(fo.Extension, "");
                        if (Directory.Exists(dir))
                        {
                            Directory.Delete(dir, true);
                        }
                        Directory.CreateDirectory(dir);
                        foreach (ZipStorer.ZipFileEntry ze in zi.ReadCentralDir())
                        {
                            zi.ExtractFile(ze, dir + "\\" + ze.FilenameInZip);
                            if (ze.FilenameInZip.ToLower().EndsWith(".kst"))
                            {
                                name = dir + "\\" + ze.FilenameInZip;
                            }
                        }
                    }
                }
            }
            catch { }
            // prova a cancellare i vecchi file (avviati da un'ora)
            timer4.Tag = (minutsAfterDeleteFromRamdisk * 60).ToString(); //mezz'ora
            timer4.Enabled = true;

            return name;
        }

        public string GetTemporaryDirectory()
        {
            string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDirectory);
            return tempDirectory;
        }
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button31_Click(object sender, EventArgs e)
        {
            if (this.button31.Image.Tag != null && this.button31.Image.Tag.ToString() == "play")
            {
                playBack();
            }
            else
            {
                pauseBack();
            }

        }

        private void playBack()
        {
            this.button31.Image = global::KaraStart.Properties.Resources.Pause;
            this.button31.Image.Tag = "pause";
            button31.Enabled = false;
            button32.Enabled = false; 
            Play();
        }

        private void pauseBack()
        {
            this.button31.Image = global::KaraStart.Properties.Resources.Play;
            this.button31.Image.Tag = "play";
            button31.Enabled = false;
            button32.Enabled = false;
            Pause();
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            double stepNum= 10.0;
            if (timer3.Tag.ToString() == "-1")
            {
                if (volumeStream.Volume > (double)trackBar1.Value / 100.0 / stepNum)
                {
                    volumeStream.Volume = volumeStream.Volume - (float)((double)trackBar1.Value / 100.0 / stepNum);
                }
                else
                {
                    timer3.Enabled = false;
                    volumeStream.Volume = 0;
                    timer3.Tag = "";
                    player.Pause();
                    button31.Enabled = true;
                    button32.Enabled = false;
                    if (button33.Tag !=null && button33.Tag.ToString() == "1")
                    {
                        button33.PerformClick();
                        button33.Tag = "";
                    }
                }
            }
            if (timer3.Tag.ToString() == "0")
            {
                stepNum = 5.0;
                if (volumeStream.Volume > (double)trackBar1.Value / 100.0 / stepNum)
                {
                    volumeStream.Volume = volumeStream.Volume - (float)((double)trackBar1.Value / 100.0 / stepNum);
                }
                else
                {
                    timer3.Enabled = false;
                    volumeStream.Volume = 0;
                    timer3.Tag = "";
                    player.Stop();
                    button31.Enabled = true;
                    button32.Enabled = false;
                    Play();
                }
            }
            if (timer3.Tag.ToString() == "1")
            {
                if (volumeStream.Volume <= (double)trackBar1.Value / 100.0 * 0.95)
                {
                    volumeStream.Volume = volumeStream.Volume + (float)((double)trackBar1.Value / 100.0 / stepNum);
                }
                else
                {
                    timer3.Enabled = false;
                    volumeStream.Volume = (float)((Double)trackBar1.Value / 100.0);
                    timer3.Tag = "";
                    button31.Enabled = true;
                    button32.Enabled = true;
                }
            }
        }

        private void button32_Click(object sender, EventArgs e)
        {
            Next();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            if (volumeStream != null)
            {
                volumeStream.Volume = (float)trackBar1.Value / 100.0F;
            }
        }

        private void button17_MouseDown(object sender, MouseEventArgs e)
        {
            clickInFolder(0, button17, e);
        }

        private String relativePath(String path, String root)
        {
            return path.Replace(root, "");
        }
        private String relativePath(String path)
        {
            String res = relativePath(path, textBox1.Text);
            if (res.EndsWith(":\\"))
                res=res.Replace(":\\",":");
            if (res=="")
                res = "\\";
            return res;
        }

        private void clickInFolder(int indexOfButton, object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                folderBrowserDialog1.SelectedPath = textBox1.Text == "" ? Environment.GetFolderPath(Environment.SpecialFolder.MyMusic) : textBox1.Text;
                folderBrowserDialog1.Description = "Scegli la cartella rapida per la ricerca dei file";
                DialogResult result = folderBrowserDialog1.ShowDialog();
                if (result == DialogResult.OK)
                {
                    ((Button)sender).Tag = relativePath(folderBrowserDialog1.SelectedPath);
                    string name = (new DirectoryInfo(folderBrowserDialog1.SelectedPath)).Name;
                    name = Interaction.InputBox("Inserisci il nome della cartella " + folderBrowserDialog1.SelectedPath, "Nome della cartella", name);
                    if (name == "") name = (new DirectoryInfo(folderBrowserDialog1.SelectedPath)).Name;
                    ((Button)sender).Text = name;
                    writeIniFolderSet();
                }
            }
            else
            {
                if ((((Button)sender).Tag!=null) && ((Button)sender).Tag.ToString() !="")
                    textBox3.Text = veryPath(((Button)sender).Tag.ToString());
            }
        }

        private void button18_MouseDown(object sender, MouseEventArgs e)
        {
            clickInFolder(1, button18, e);
        }

        private void button16_MouseDown(object sender, MouseEventArgs e)
        {
            clickInFolder(2, button16, e);
        }

        private void button19_MouseDown(object sender, MouseEventArgs e)
        {
            clickInFolder(3, button19, e);
        }

        private void button20_MouseDown(object sender, MouseEventArgs e)
        {
            clickInFolder(4, button20, e);
        }

        private void button21_MouseDown(object sender, MouseEventArgs e)
        {
            clickInFolder(5, button21, e);
        }

        private void button22_MouseDown(object sender, MouseEventArgs e)
        {
            clickInFolder(6, button22, e);
        }

        private void button23_MouseDown(object sender, MouseEventArgs e)
        {
            clickInFolder(7, button23, e);
        }

        private void button24_MouseDown(object sender, MouseEventArgs e)
        {
            clickInFolder(8, button24, e);
        }

        private void button25_MouseDown(object sender, MouseEventArgs e)
        {
            clickInFolder(9, button25, e);
        }

        private void button26_MouseDown(object sender, MouseEventArgs e)
        {
            clickInFolder(10, button26, e);
        }

        private void button27_MouseDown(object sender, MouseEventArgs e)
        {
            clickInFolder(11, button27, e);
        }

        private void writeIniFolderSet()
        {
            Program.cfg.IniWriteValue("settings", "folder_name_0", button17.Text);
            Program.cfg.IniWriteValue("settings", "folder_path_0", button17.Tag.ToString());
            Program.cfg.IniWriteValue("settings", "folder_name_1", button18.Text);
            Program.cfg.IniWriteValue("settings", "folder_path_1", button18.Tag.ToString());
            Program.cfg.IniWriteValue("settings", "folder_name_2", button16.Text);
            Program.cfg.IniWriteValue("settings", "folder_path_2", button16.Tag.ToString());
            Program.cfg.IniWriteValue("settings", "folder_name_3", button19.Text);
            Program.cfg.IniWriteValue("settings", "folder_path_3", button19.Tag.ToString());
            Program.cfg.IniWriteValue("settings", "folder_name_4", button20.Text);
            Program.cfg.IniWriteValue("settings", "folder_path_4", button20.Tag.ToString());
            Program.cfg.IniWriteValue("settings", "folder_name_5", button21.Text);
            Program.cfg.IniWriteValue("settings", "folder_path_5", button21.Tag.ToString());
            Program.cfg.IniWriteValue("settings", "folder_name_6", button22.Text);
            Program.cfg.IniWriteValue("settings", "folder_path_6", button22.Tag.ToString());
            Program.cfg.IniWriteValue("settings", "folder_name_7", button23.Text);
            Program.cfg.IniWriteValue("settings", "folder_path_7", button23.Tag.ToString());
            Program.cfg.IniWriteValue("settings", "folder_name_8", button24.Text);
            Program.cfg.IniWriteValue("settings", "folder_path_8", button24.Tag.ToString());
            Program.cfg.IniWriteValue("settings", "folder_name_9", button25.Text);
            Program.cfg.IniWriteValue("settings", "folder_path_9", button25.Tag.ToString());
            Program.cfg.IniWriteValue("settings", "folder_name_10", button26.Text);
            Program.cfg.IniWriteValue("settings", "folder_path_10", button26.Tag.ToString());
            Program.cfg.IniWriteValue("settings", "folder_name_11", button27.Text);
            Program.cfg.IniWriteValue("settings", "folder_path_11", button27.Tag.ToString());

        }

        private void readIniProgramSet()
        {
            button10.Text = Program.cfg.IniReadValue("settings", "program_name_0", "-nd-");
            button10.Tag = Program.cfg.IniReadValue("settings", "program_path_0", "-nd-");
            if (button10.Tag != "-nd-" && File.Exists(AppSettingsManager.Istance.getAppDataDirectory() + "\\Icon\\0.bmp"))
            {
                try
                {
                    FileInfo fi = new FileInfo(button10.Tag.ToString().Split('?')[0]);
                    Icon ico = Icon.ExtractAssociatedIcon(fi.FullName);
                    Bitmap b = new Bitmap(ico.ToBitmap(), new Size(button10.Height - 10, button10.Height - 10));
                    button10.Text = "";
                    button10.BackgroundImage = b;
                    button10.Text = "";
                }
                catch { }
            }
            //button10.Enabled = (button10.Tag != null && Directory.Exists(button10.Tag.ToString()));

            button11.Text = Program.cfg.IniReadValue("settings", "program_name_1", "-nd-");
            button11.Tag = Program.cfg.IniReadValue("settings", "program_path_1", "-nd-");
            if (button11.Tag != "-nd-" && File.Exists(AppSettingsManager.Istance.getAppDataDirectory() + "\\Icon\\1.bmp"))
            {
                try
                {
                    FileInfo fi = new FileInfo(button11.Tag.ToString().Split('?')[0]);
                    Icon ico = Icon.ExtractAssociatedIcon(fi.FullName);
                    Bitmap b = new Bitmap(ico.ToBitmap(), new Size(button11.Height - 10, button11.Height - 10));
                    button11.Text = "";
                    button11.BackgroundImage = b;
                    button11.Text = "";
                }
                catch { }
            }
            //button11.Enabled = (button11.Tag != null && Directory.Exists(button11.Tag.ToString()));
        }

        private void writeIniProgramSet()
        {
            Program.cfg.IniWriteValue("settings", "program_name_0", button10.Text);
            Program.cfg.IniWriteValue("settings", "program_path_0", button10.Tag.ToString());
            Program.cfg.IniWriteValue("settings", "program_name_1", button11.Text);
            Program.cfg.IniWriteValue("settings", "program_path_1", button11.Tag.ToString());
        }

        private void esciToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void cosaCèDiNuovoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(@"http://karastart.myblog.it/2011/03/05/cosa-c-e-di-nuovo/");
        }

        private void ilBlogDiKarastartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(@"http://karastart.myblog.it/");
        }

        private void infoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (new AboutBox1()).ShowDialog();
        }

        private void impostazioniToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (new Settings()).ShowDialog();
        }

        private void timer4_Tick(object sender, EventArgs e)
        {
            if (int.Parse(timer4.Tag.ToString()) > 0)
            {
                timer4.Tag = int.Parse(timer4.Tag.ToString()) - 1;
            }
            else
            {
                checkObsoleteRamFile();
                timer4.Enabled = false;
            }
        }

        private void checkObsoleteRamFile()
        {
            if (useRamDisk)
            {
                foreach (String fName in Directory.GetFiles(ramDiskPath, "*", SearchOption.AllDirectories))
                {
                    FileInfo fi = new FileInfo(fName);
                    if (DateTime.Now.Subtract(fi.CreationTime).TotalMinutes > (double)minutsAfterDeleteFromRamdisk)
                    {
                        double min = DateTime.Now.Subtract(fi.CreationTime).TotalMinutes;
                        fi.Delete();
                    }
                }
                foreach (String fName in Directory.GetDirectories(ramDiskPath, "*", SearchOption.AllDirectories))
                {
                    DirectoryInfo di = new DirectoryInfo(fName);
                    if (di.GetFiles().Length == 0)
                    {
                        di.Delete();
                    }
                }
            }
        }

        private void utilizzoDelCLONASCHERMOToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(@"http://karastart.myblog.it/utilizzo-dello-schermo-clone/");
        }

        private void setProgramEnabling(String fName)
        {
            FileInfo fi = new FileInfo(fName);
            enableButtonOnExtension(button4, fi.Extension);
            enableButtonOnExtension(button5, fi.Extension);
            enableButtonOnExtension(button6, fi.Extension);
            enableButtonOnExtension(button7, fi.Extension);
            enableButtonOnExtension(button8, fi.Extension);
            enableButtonOnExtension(button9, fi.Extension);
            enableButtonOnExtension(button10, fi.Extension);
            enableButtonOnExtension(button11, fi.Extension);
            enableButtonOnExtension(button12, fi.Extension);
            enableButtonOnExtension(button13, fi.Extension);
            enableButtonOnExtension(button14, fi.Extension);
            button15.Enabled = true;
        }

        public Dictionary<string, string> getProgramEnabling(String extension)
        {
            Dictionary<string, string> progNameExeAndName = new Dictionary<string, string>();
            if (canExecuteWithButton(button4, extension))
            {
                String progName = new FileInfo(button4.Tag.ToString().Split('?')[0]).FullName;
                progNameExeAndName.Add("vlc", progName);
            }
            if (canExecuteWithButton(button5, extension))
            {
                String progName = new FileInfo(button5.Tag.ToString().Split('?')[0]).FullName;
                progNameExeAndName.Add("karaoke5", progName);
            }
            if (canExecuteWithButton(button6, extension))
            {
                String progName = new FileInfo(button6.Tag.ToString().Split('?')[0]).FullName;
                progNameExeAndName.Add("karafun", progName);
            }
            if (canExecuteWithButton(button7, extension))
            {
                String progName = new FileInfo(button7.Tag.ToString().Split('?')[0]).FullName;
                progNameExeAndName.Add("karabox", progName);
            }
            if (canExecuteWithButton(button8, extension))
            {
                String progName = new FileInfo(button8.Tag.ToString().Split('?')[0]).FullName;
                progNameExeAndName.Add("van basco", progName);
            }
            if (canExecuteWithButton(button9, extension))
            {
                String progName = new FileInfo(button9.Tag.ToString().Split('?')[0]).FullName;
                progNameExeAndName.Add("wmedia player", progName);
            }
            if (canExecuteWithButton(button10, extension))
            {
                String progName = new FileInfo(button10.Tag.ToString().Split('?')[0]).FullName;
                progNameExeAndName.Add(button10.Text, progName);
            }
            if (canExecuteWithButton(button11, extension))
            {
                String progName = new FileInfo(button11.Tag.ToString().Split('?')[0]).FullName;
                progNameExeAndName.Add(button11.Text, progName);
            }
            if (canExecuteWithButton(button12, extension))
            {
                String progName = new FileInfo(button12.Tag.ToString().Split('?')[0]).FullName;
                progNameExeAndName.Add("acrobat pdf reader", progName);
            }
            if (canExecuteWithButton(button13, extension))
            {
                String progName = new FileInfo(button13.Tag.ToString().Split('?')[0]).FullName;
                progNameExeAndName.Add("ms word", progName);
            }
            if (canExecuteWithButton(button15, extension))
            {
                progNameExeAndName.Add("jolly", "");
            }
            progNameExeAndName.Add("personalizzato", "personalizzato");
            return progNameExeAndName;
        }

        private bool canExecuteWithButton(Button b, String extension)
        {
            extension= extension.ToLower();
            if (b == button15) return true;
            if (b == button14 && extension == ".kst") return true;
            if (b.Tag != null && b.Tag.ToString() != "" && b.Tag.ToString() != "-nd-")
            {
                bool exist = false;
                String[] extVect = b.Tag.ToString().Split('?')[1].Split('|');
                for (int i = 1; i < extVect.Length; i++)
                {
                    exist = (extVect[i].Replace("*", "") == extension);
                    if (exist) break;
                }
                return exist;
            }
            else
            {
                return false;
            }
        }

        private void enableButtonOnExtension(Button b, String extension)
        {
            b.Enabled = canExecuteWithButton(b, extension);
        }

        private void ProgramsCheckForButton()
        {
            String pName = @"\VideoLAN\VLC\vlc.exe?|*.mp3|*.wav|*.mp4|*.avi|*.mpg|*.mpeg|*.m4a|*.wmw|*.asf|*.wma|*.mp3+g|*.k5|*.m5";
            Button b = button4;
            checkSingolProgram(b, pName);
            b = button5;
            pName = @"\Karaoke5\karaoke.exe?|*.mp3|*.wav|*.wma|*.mp3+g|*.m5|*.mid|*.midi|*.kar|*.k5";
            checkSingolProgram(b, pName);
            b = button6;
            pName = @"\KaraFun\KaraFun.exe?|*.kfn";
            checkSingolProgram(b, pName);
            b = button7;
            pName = @"\Karabox\KaraBoxPlus.exe?|*.mkf";
            checkSingolProgram(b, pName);
            b = button8;
            pName = @"\vanBasco's Karaoke Player\vmidi.exe?|*.mid|*.midi|*.kar";
            checkSingolProgram(b, pName);
            b = button9;
            pName = @"\Windows Media Player\wmplayer.exe?|*.mp3|*.wav|*.mp4|*.avi|*.mpg|*.mpeg|*.m4a|*.wmw|*.asf|*.wma|*.mp3+g|*.m5|*.mid|*.midi|*.kar|*.k5";
            checkSingolProgram(b, pName);
            checkSingolProgram(button12, @"\Adobe\Reader 9.0\Reader\AcroRd32.exe?|*.pdf");
            checkSingolProgram(button13, @"\Microsoft Office\Office14\WINWORD.EXE?|*.doc|*.txt|*.rtf|*.docx");
            if (button13.Tag == null || button13.Tag.ToString() == "")
                checkSingolProgram(button13, @"\Microsoft Office\Office13\WINWORD.EXE?|*.doc|*.txt|*.rtf|*.docx");
            if (button13.Tag == null || button13.Tag.ToString() == "")
                checkSingolProgram(button13, @"\Microsoft Office\Office12\WINWORD.EXE?|*.doc|*.txt|*.rtf|*.docx");
            if (button13.Tag == null || button13.Tag.ToString() == "")
                checkSingolProgram(button13, @"\Microsoft Office\Office11\WINWORD.EXE?|*.doc|*.txt|*.rtf|*.docx");
            if (button13.Tag == null || button13.Tag.ToString() == "")
                checkSingolProgram(button13, @"\Microsoft Office\Office10\WINWORD.EXE?|*.doc|*.txt|*.rtf|*.docx");
        }

        private void checkSingolProgram(Button b, String path)
        {
            String pName = path;
            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + pName.Split('?')[0]))
                b.Tag = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + pName;
            else if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + pName.Split('?')[0]))
                b.Tag = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + pName;
            if (b.Tag!=null && b.Tag.ToString() != "")
                b.Enabled = true;
        }

        private void sostieniKarastartComprandoUnCaffèARiceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(@"http://karastart.myblog.it/fai-donazione/");
        }

        private void button10_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                setNewProgram(sender);
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                startFile((Button)sender);
            }
        }

        private void setNewProgram(object sender)
        {
            openFileDialog1.Filter = "Programma (*.exe)|*.exe";
            openFileDialog1.Title = "Scegli il programma associato al pulsante";
            openFileDialog1.FileName = "";
            openFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                string name = (new FileInfo(openFileDialog1.FileName)).Name;
                Icon ico = Icon.ExtractAssociatedIcon(openFileDialog1.FileName);
                Bitmap b = new Bitmap(ico.ToBitmap(),new Size(button10.Height-10, button10.Height-10));
                //if (!Directory.Exists(AppSettingsManager.Istance.getAppDataDirectory() + "\\Icon\\"))
                //    Directory.CreateDirectory(AppSettingsManager.Istance.getAppDataDirectory() + "\\Icon\\");
                //String iconName = ((Button)sender).Name == "button10" ? "0" : "1";
                //File.Delete(AppSettingsManager.Istance.getAppDataDirectory() + "\\Icon\\" + iconName + ".bmp");
                //b.Save(AppSettingsManager.Istance.getAppDataDirectory() + "\\Icon\\" + iconName + ".bmp");
                //name = Interaction.InputBox("Inserisci il nome del programma " + name, "Nome del pulsante", name);
                //if (name == "") name = (new FileInfo(openFileDialog1.FileName)).Name;
                //((Button)sender).Text = name;
                ((Button)sender).Text = "";
                ((Button)sender).BackgroundImage = b;
                //((Button)sender).BackgroundImageLayout = ImageLayout.Center;
                String ext = "";
                MessageBox.Show("Potrai scegliere le estensioni dei file da poter avviare con " + name + ". (Ad esempio puoi aggiungere 'mp3'). Potrai inserirne quante ne vuoi, ogni volta che ti viene chiesto inseriscine una e premi invio. Quando avrai finito premi invio senza inserire niente", "Assegna estensioni");
                String inse = Interaction.InputBox("Nuova estensione");
                while (inse != "")
                {
                    ext = ext + "|*." + inse;
                    inse = Interaction.InputBox("Nuova estensione");
                }
                ((Button)sender).Tag = openFileDialog1.FileName + "?" + ext;
                writeIniProgramSet();
            }
        }

        private void MainForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                var p = PointToClient(Cursor.Position);
                Control c = GetChildAtPoint(p);
                if (c != null && c.Enabled == false)
                {
                    if (c == button10 || c == button11)
                    {
                        setNewProgram(c);
                    }
                }
            }
        }

        private void buttonOfProgram_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                startFile((Button)sender);
        }

        private void button11_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                setNewProgram(sender);
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                startFile((Button)sender);
            }
        }

        private void listView2_KeyUp(object sender, KeyEventArgs e)
        {
            if (listView2.SelectedItems.Count > 0)
            {
                if (e.KeyValue == 40 || e.KeyValue == 38)
                {
                    refreshInformationOfFile(listView2.SelectedItems[0].Name);
                }
            }
        }

        private void listView1_KeyUp(object sender, KeyEventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                if (e.KeyValue == 40 || e.KeyValue == 38)
                {
                    refreshInformationOfFile(veryPath(listView1.SelectedItems[0].Name));
                }
            }
        }
        #region DragMethods
        private void ClearDragData()
        {
            try
            {
                if (File.Exists(dragItemTempFileName))
                    File.Delete(dragItemTempFileName);
                Program.objDragItem = null;
                dragItemTempFileName = string.Empty;
                itemDragStart = false;
                Program.ClearFileWatchers();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "DragNDrop Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        private void creaFileCombinatoConIFileSelezionatiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                String inse = "-1";
                string folder = "-1";
                folderBrowserDialog1.Description = "Scegli dove salvare il file combinato";
                folderBrowserDialog1.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
                DialogResult result = folderBrowserDialog1.ShowDialog();
                if (result == DialogResult.OK)
                {
                    folder = folderBrowserDialog1.SelectedPath;
                    String tempKst = Path.GetTempFileName();
                    foreach (ListViewItem l in listView1.SelectedItems)
                    {
                        try
                        {
                            if (File.Exists(l.Name))
                            {
                                String s = l.Name;
                                FileInfo fi = new FileInfo(s);
                                KstResult res = (new KstForm()).getMeRes(fi.Name);
                                if (res != null)
                                {
                                    if (res.millisRitardo != 0)
                                        File.AppendAllText(tempKst, res.millisRitardo.ToString());
                                    if (res.progPath == "")
                                        File.AppendAllText(tempKst, "[ext]" + '\u0022' + fi.Name + '\u0022');
                                    else
                                        File.AppendAllText(tempKst, "[ext]" + '\u0022' + res.progPath + '\u0022' + " " + '\u0022' + fi.Name + '\u0022');
                                    if (res.programName == "vlc")
                                        File.AppendAllText(tempKst, " " + '\u0022' + "--noaudio" + '\u0022');
                                    if (res.programName == "vlc" && res.sAnticipo != 0)
                                        File.AppendAllText(tempKst, " " + '\u0022' + "--start-time=" + res.sAnticipo + '\u0022');
                                    File.AppendAllText(tempKst, "\r\n");
                                }
                            }
                        }
                        catch (Exception ex) { (new MailForm(ex.Message)).ShowDialog(); }
                    }
                    try
                    {
                        FileInfo fin = new FileInfo(veryPath(listView1.SelectedItems[0].Name));
                        inse = Interaction.InputBox("Inserisci il nome del file zip combinato", "Nome del file combinato", fin.Name.Replace(fin.Extension, ""));
                        if (inse != "")
                        {
                            UseWaitCursor = true;
                            Application.DoEvents();
                            ZipStorer zip = ZipStorer.Create(folder + "\\" + inse + ".zip", "file creato con Karastart 2.0");
                            zip.AddFile(ZipStorer.Compression.Deflate, tempKst, "file combo di Karastart (il file zip può essere lanciato da Karastart).kst", "file creato da Karastart 2.0");
                            foreach (ListViewItem l in listView1.SelectedItems)
                            {
                                fin = new FileInfo(l.Name);
                                zip.AddFile(ZipStorer.Compression.Deflate, fin.FullName, fin.Name, "file creato da Karastart 2.0");
                                Application.DoEvents();
                            }
                            zip.Close();
                            UseWaitCursor = false;
                            Application.DoEvents();
                        }
                    }
                    catch (Exception ex) { (new MailForm(ex.Message)).ShowDialog(); }
                }
                if (inse != "-1" && inse != "")
                {
                    DialogResult result2 = MessageBox.Show("Aggiungere il file combinato " + inse + " alla playlist?","Aggiungi alla playlist", MessageBoxButtons.OKCancel);
                    if (result2 == DialogResult.OK)
                    {
                        //foreach (ListViewItem i in listView1.SelectedItems)
                        //{
                        //    i.Remove();
                        //}
                        refreshInformationOfFile(folder + "\\" + inse + ".zip");
                        addToPlaylistAnItem(folder + "\\" + inse + ".zip");
                        listView1.SelectedItems.Clear();
                        listView1.Items[listView1.Items.Count - 1].Selected = true;
                    }
                }
            }
        }

        private void eliminaFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                listView1.Items.RemoveByKey(listView1.SelectedItems[0].Name);
                updatePlaylistWithTimout();
            }
        }

        private void apriCartellaContenenteIlFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                System.Diagnostics.Process.Start(
                    "explorer.exe",
                    string.Format("/select, \"{0}\"", veryPath(listView1.SelectedItems[0].Name)));
            }
        }

        private void apriPercorsoFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView2.SelectedItems.Count > 0)
            {
                System.Diagnostics.Process.Start(
                    "explorer.exe",
                    string.Format("/select, \"{0}\"", listView2.SelectedItems[0].Name));
            }
        }

        private void avviaFileConProgrammaJollyusatoPerUltimoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (button15.Enabled && lblFileName.Tag.ToString() != "" && listView2.SelectedItems.Count > 0)
                startFile(button15);
        }

        private void aggiungiAPlaylistCorrenteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView2.SelectedItems.Count > 0)
            {
                addToPlaylistAnItem(listView2.SelectedItems[0].Name);
            }
        }

        private void button2_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (mediaSourceForSecondScreen == "")
                {
                    button2_MouseDown(sender, new MouseEventArgs(System.Windows.Forms.MouseButtons.Right, e.Clicks, e.X, e.Y, e.Delta));
                }
                else
                {
                    button2.Tag = 1 - int.Parse("0" + button2.Tag.ToString());
                    secondScreen.useBitmap = button2.Tag.ToString() == "1";
                }
            }
            else
            {
                String filePath ="";
                openFileDialog1.Filter = "Jpeg (*.jpg)|*.jpg|Bitmap (*.bmp)|*.bmp|Video avi (*.avi)|*.avi|Video mp4 (*.mp4)|*.mp4|"
                         + "File multimediali|*.bmp;*.jpg;*.avi;*.mp4|Tutti i file|*.*";
                openFileDialog1.FilterIndex = 5;
                openFileDialog1.Title = "Scegli un file da proiettare nella finestra clone";
                openFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                openFileDialog1.FileName = "";
                DialogResult result = openFileDialog1.ShowDialog();
                if (result == DialogResult.OK)
                {
                    filePath = openFileDialog1.FileName;
                    try
                    {
                        Cursor.Current = Cursors.WaitCursor;
                        if (filePath.EndsWith(".jpg") || filePath.EndsWith(".bmp"))
                        {
                            Bitmap b = new Bitmap(filePath);
                            b.Dispose();
                            GC.SuppressFinalize(b);
                        }
                        else if (filePath.EndsWith(".mp4") || filePath.EndsWith(".avi"))
                        {
                            MciPlayer m = new MciPlayer();

                            String Pcommand = "close aviWitkKarastart";
                            m.MCISendString(Pcommand);
                            Pcommand = "close aviWitkKarastart0";
                            m.MCISendString(Pcommand);

                            FileInfo fi = new FileInfo(filePath);
                            String relocatedfile = Application.StartupPath + "\\unicodenamesupport\\0" + fi.Extension;
                            if (File.Exists(relocatedfile))
                                File.Delete(relocatedfile);
                            if (!Directory.Exists("unicodenamesupport"))
                                Directory.CreateDirectory("unicodenamesupport");
                            File.Copy(filePath, relocatedfile);

                            //open command with assigned alias and window handle
                            Pcommand = "open mpegvideo!\"" +
                                       "unicodenamesupport\\0" + fi.Extension + "\" alias aviWitkKarastart0" +
                                       " parent " + panel1.Handle.ToString() + " style child";

                            String res = m.MCISendString(Pcommand);
                            if (res!= "1")
                                throw new Exception("Media loading error");

                            Pcommand = "close aviWitkKarastart0";
                            m.MCISendString(Pcommand);
                        }
                        secondScreen.useBitmap = false;
                        Application.DoEvents();
                        mediaSourceForSecondScreen = filePath;
                        Program.cfg.IniWriteValue("settings", "mediaSourceForSecondScreen", filePath);
                        button2.Tag = "1";
                        secondScreen.useBitmap = bool.Parse(button2.Tag.ToString().Replace("1", "True").Replace("0", "False"));
                        Cursor.Current = Cursors.Default;
                    }
                    catch
                    {
                        MessageBox.Show("Immagine o video non validi, scegli un altro file o se vuoi visualizzare un video installa i codec necessari (Klite Codec)");
                        button2_MouseDown(sender, new MouseEventArgs(System.Windows.Forms.MouseButtons.Right, e.Clicks, e.X, e.Y, e.Delta));
                    }
                    
                }
            }
            button2.Image = (button2.Tag!=null && button2.Tag.ToString() == "0") ? global::KaraStart.Properties.Resources.videoimage : global::KaraStart.Properties.Resources.videoimage_active;
        }

        private void button1_MouseDown(object sender, MouseEventArgs e)
        {
            if (splitContainer1.SplitterDistance > 25)
            {
                oldSplitDist = splitContainer1.SplitterDistance;
                splitContainer1.SplitterDistance = 0;
                button1.Text = ">";
            }
            else
            {
                splitContainer1.SplitterDistance = oldSplitDist;
                button1.Text = "<";
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (infoFile[0] != null)
            {
                saveRatingInfoFile(0.2 * (double)int.Parse(((PictureBox)sender).Name.Replace("rate0_","")), infoFile[0].idFile);
            }
        }

        private void pictureBox10_Click(object sender, EventArgs e)
        {
            if (infoFile[1] != null)
            {
                saveRatingInfoFile(0.2 * (double)int.Parse(((PictureBox)sender).Name.Replace("rate1_", "")), infoFile[1].idFile);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void timer5_Tick(object sender, EventArgs e)
        {
            timer5.Enabled = false;
            saveVolumeInfoFile(infoFile[0].idFile);
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            textBox5.ForeColor = Color.Black;
            textBox5.Font = new Font(textBox5.Font, FontStyle.Regular);
        }

        private void textBox5_Click(object sender, EventArgs e)
        {
            textBox5.SelectAll();
        }

        private void textBox5_Leave(object sender, EventArgs e)
        {
            Program.cfg.IniWriteValue("settings", "textOnClone", textBox5.Text);
            resetTextOnClone();
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            resetTextOnClone();
            Program.cfg.IniWriteValue("settings", "textOnCloneOn", checkBox4.Checked.ToString());
        }

        public void resetTextOnClone()
        {
            if (secondScreen != null)
            {
                secondScreen.text = textBox5.Text;
                secondScreen.useText = checkBox4.Checked;
            }
        }

        private void textBox5_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode ==  Keys.Enter)
            {
                textBox5_Leave(sender, null);
            }
        }

        private void suggerimentoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (new MailForm("")).ShowDialog();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            stopSearch.val = true;
        }

        private void aggiornamentoAutomaticoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.cfg.IniWriteValue("settings", "checkUpdate", ((ToolStripMenuItem)sender).Checked.ToString());
        }

        private void controllaAggiornamentiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.checkUpdate();
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift && (e.KeyCode >= Keys.F1 && e.KeyCode <= Keys.F24))
            {
                int index = (e.KeyCode - Keys.F1);
                if (index < comboBox1.Items.Count)
                    comboBox1.SelectedIndex = index;
            }
            if (e.KeyCode == Keys.Escape && secondScreen.WindowState == FormWindowState.Maximized)
            {
                secondScreen.panel1_MouseDoubleClick(null, null);
                secondScreen.Location = this.Location;
            }
        }

        private void scalettaFileAvviatiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String temp = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".txt");
            using (StreamWriter sw = File.CreateText(temp))
            {
                foreach (String s in startedFile.Values)
                {
                    FileInfo fi = new FileInfo(s);
                    sw.WriteLine(fi.Name);
                }
                sw.Close();
            }
            System.Diagnostics.Process.Start(temp);
        }

        bool whileLoading = true;
        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (!whileLoading && this.WindowState!= FormWindowState.Minimized)
            {
                Program.cfg.IniWriteValue("windows", "positionL", this.Location.X.ToString());
                Program.cfg.IniWriteValue("windows", "positionT", this.Location.Y.ToString());
                Program.cfg.IniWriteValue("windows", "MainHeigth", this.Height.ToString());
                Program.cfg.IniWriteValue("windows", "MainWidth", this.Width.ToString());
            }
        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem15_Click(object sender, EventArgs e)
        {
            toolStripMenuItem14_Click(sender, e);
        }

        private void toolStripMenuItem14_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(@"http://karastart.myblog.it/guida");
        }

        private void toolStripMenuItem13_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(@"http://karastart.myblog.it/vlc");
        }

        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (!inibSplittSaving)
                Program.cfg.IniWriteValue("windows", "splitterPerc", ((Double)splitContainer1.Panel1.Width / (Double)splitContainer1.Width * 100.0).ToString("0.00"));
        }

        private void timer6_Tick(object sender, EventArgs e)
        {
            timer6.Enabled = false;
            button32.PerformClick();
        }

    }
    public class TimerAndTag
    {
        public System.Timers.Timer t;
        public String tag;
        public String path;
        public TimerAndTag(System.Timers.Timer t)
        {
            this.path = "";
            this.t = t;
            this.tag = "";
        }
    }
}
