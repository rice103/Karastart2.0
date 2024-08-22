﻿namespace AudioSwitch.Forms
{
    partial class FormSettings
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tabSettings = new System.Windows.Forms.TabControl();
            this.tabHotkeys = new System.Windows.Forms.TabPage();
            this.gridHotkeys = new System.Windows.Forms.DataGridView();
            this.Function = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Control = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Alt = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Shift = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Win = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.HotKey = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.ShowOSD = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.tabDevices = new System.Windows.Forms.TabPage();
            this.groupDevice = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.checkHideDevice = new System.Windows.Forms.CheckBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.buttonResetDevice = new System.Windows.Forms.Button();
            this.buttonSaveDevice = new System.Windows.Forms.Button();
            this.trackBrightness = new System.Windows.Forms.TrackBar();
            this.trackSaturation = new System.Windows.Forms.TrackBar();
            this.trackHue = new System.Windows.Forms.TrackBar();
            this.pictureModded = new System.Windows.Forms.PictureBox();
            this.buttonClose = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.linkManual = new System.Windows.Forms.LinkLabel();
            this.listDevices = new AudioSwitch.Controls.CustomListView();
            this.label6 = new System.Windows.Forms.Label();
            this.comboDefMode = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.trackTransparency = new System.Windows.Forms.TrackBar();
            this.numTimeout = new System.Windows.Forms.NumericUpDown();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.comboOSDSkin = new System.Windows.Forms.ComboBox();
            this.labelVersion = new System.Windows.Forms.Label();
            this.linkWebpage = new System.Windows.Forms.LinkLabel();
            this.labelAuthor = new System.Windows.Forms.Label();
            this.checkDefaultMultiAndComm = new System.Windows.Forms.CheckBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.comboScrollKey = new System.Windows.Forms.ComboBox();
            this.checkScrShowOSD = new System.Windows.Forms.CheckBox();
            this.checkVolScroll = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tabGeneral = new System.Windows.Forms.TabPage();
            this.tabSettings.SuspendLayout();
            this.tabHotkeys.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridHotkeys)).BeginInit();
            this.tabDevices.SuspendLayout();
            this.groupDevice.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBrightness)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackSaturation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackHue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureModded)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackTransparency)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTimeout)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabGeneral.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabSettings
            // 
            this.tabSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabSettings.Controls.Add(this.tabHotkeys);
            this.tabSettings.Controls.Add(this.tabDevices);
            this.tabSettings.Controls.Add(this.tabGeneral);
            this.tabSettings.HotTrack = true;
            this.tabSettings.Location = new System.Drawing.Point(12, 12);
            this.tabSettings.Name = "tabSettings";
            this.tabSettings.SelectedIndex = 0;
            this.tabSettings.Size = new System.Drawing.Size(536, 342);
            this.tabSettings.TabIndex = 0;
            // 
            // tabHotkeys
            // 
            this.tabHotkeys.Controls.Add(this.gridHotkeys);
            this.tabHotkeys.Location = new System.Drawing.Point(4, 22);
            this.tabHotkeys.Name = "tabHotkeys";
            this.tabHotkeys.Padding = new System.Windows.Forms.Padding(3);
            this.tabHotkeys.Size = new System.Drawing.Size(528, 316);
            this.tabHotkeys.TabIndex = 0;
            this.tabHotkeys.Text = "Hot Keys";
            this.tabHotkeys.UseVisualStyleBackColor = true;
            // 
            // gridHotkeys
            // 
            this.gridHotkeys.AllowUserToResizeColumns = false;
            this.gridHotkeys.AllowUserToResizeRows = false;
            this.gridHotkeys.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridHotkeys.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.gridHotkeys.BackgroundColor = System.Drawing.SystemColors.Window;
            this.gridHotkeys.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.gridHotkeys.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridHotkeys.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Function,
            this.Control,
            this.Alt,
            this.Shift,
            this.Win,
            this.HotKey,
            this.ShowOSD});
            this.gridHotkeys.GridColor = System.Drawing.SystemColors.Control;
            this.gridHotkeys.Location = new System.Drawing.Point(6, 6);
            this.gridHotkeys.Name = "gridHotkeys";
            this.gridHotkeys.RowHeadersWidth = 25;
            this.gridHotkeys.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(186)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            this.gridHotkeys.RowsDefaultCellStyle = dataGridViewCellStyle1;
            this.gridHotkeys.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.gridHotkeys.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridHotkeys.Size = new System.Drawing.Size(516, 304);
            this.gridHotkeys.TabIndex = 1;
            // 
            // Function
            // 
            this.Function.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Function.HeaderText = "Function";
            this.Function.MaxDropDownItems = 100;
            this.Function.MinimumWidth = 160;
            this.Function.Name = "Function";
            this.Function.Width = 160;
            // 
            // Control
            // 
            this.Control.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.Control.HeaderText = "Control";
            this.Control.Name = "Control";
            this.Control.Width = 46;
            // 
            // Alt
            // 
            this.Alt.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.Alt.HeaderText = "Alt";
            this.Alt.Name = "Alt";
            this.Alt.Width = 25;
            // 
            // Shift
            // 
            this.Shift.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.Shift.HeaderText = "Shift";
            this.Shift.Name = "Shift";
            this.Shift.Width = 34;
            // 
            // Win
            // 
            this.Win.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.Win.HeaderText = "Win";
            this.Win.Name = "Win";
            this.Win.Width = 32;
            // 
            // HotKey
            // 
            this.HotKey.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.HotKey.HeaderText = "Hot Key";
            this.HotKey.MaxDropDownItems = 100;
            this.HotKey.MinimumWidth = 120;
            this.HotKey.Name = "HotKey";
            // 
            // ShowOSD
            // 
            this.ShowOSD.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.ShowOSD.HeaderText = "Show OSD";
            this.ShowOSD.Name = "ShowOSD";
            this.ShowOSD.Width = 66;
            // 
            // tabDevices
            // 
            this.tabDevices.Controls.Add(this.groupDevice);
            this.tabDevices.Controls.Add(this.listDevices);
            this.tabDevices.Location = new System.Drawing.Point(4, 22);
            this.tabDevices.Name = "tabDevices";
            this.tabDevices.Padding = new System.Windows.Forms.Padding(3);
            this.tabDevices.Size = new System.Drawing.Size(528, 316);
            this.tabDevices.TabIndex = 1;
            this.tabDevices.Text = "Devices";
            this.tabDevices.UseVisualStyleBackColor = true;
            // 
            // groupDevice
            // 
            this.groupDevice.Controls.Add(this.label7);
            this.groupDevice.Controls.Add(this.checkHideDevice);
            this.groupDevice.Controls.Add(this.label9);
            this.groupDevice.Controls.Add(this.label10);
            this.groupDevice.Controls.Add(this.label11);
            this.groupDevice.Controls.Add(this.buttonResetDevice);
            this.groupDevice.Controls.Add(this.buttonSaveDevice);
            this.groupDevice.Controls.Add(this.trackBrightness);
            this.groupDevice.Controls.Add(this.trackSaturation);
            this.groupDevice.Controls.Add(this.trackHue);
            this.groupDevice.Controls.Add(this.pictureModded);
            this.groupDevice.Location = new System.Drawing.Point(268, 6);
            this.groupDevice.Name = "groupDevice";
            this.groupDevice.Size = new System.Drawing.Size(254, 304);
            this.groupDevice.TabIndex = 8;
            this.groupDevice.TabStop = false;
            this.groupDevice.Text = "Selected Device Settings";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(121, 27);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(79, 13);
            this.label7.TabIndex = 26;
            this.label7.Text = "Tray Icon Color";
            // 
            // checkHideDevice
            // 
            this.checkHideDevice.AutoSize = true;
            this.checkHideDevice.Location = new System.Drawing.Point(17, 237);
            this.checkHideDevice.Name = "checkHideDevice";
            this.checkHideDevice.Size = new System.Drawing.Size(154, 17);
            this.checkHideDevice.TabIndex = 25;
            this.checkHideDevice.Text = "Hide device from switch list";
            this.checkHideDevice.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(42, 63);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(27, 13);
            this.label9.TabIndex = 16;
            this.label9.Text = "Hue";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(14, 88);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(55, 13);
            this.label10.TabIndex = 19;
            this.label10.Text = "Saturation";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(13, 114);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(56, 13);
            this.label11.TabIndex = 21;
            this.label11.Text = "Brightness";
            // 
            // buttonResetDevice
            // 
            this.buttonResetDevice.Location = new System.Drawing.Point(16, 263);
            this.buttonResetDevice.Name = "buttonResetDevice";
            this.buttonResetDevice.Size = new System.Drawing.Size(102, 27);
            this.buttonResetDevice.TabIndex = 13;
            this.buttonResetDevice.Text = "Remove Settings";
            this.buttonResetDevice.UseVisualStyleBackColor = true;
            this.buttonResetDevice.Click += new System.EventHandler(this.buttonResetDevice_Click);
            // 
            // buttonSaveDevice
            // 
            this.buttonSaveDevice.Location = new System.Drawing.Point(137, 263);
            this.buttonSaveDevice.Name = "buttonSaveDevice";
            this.buttonSaveDevice.Size = new System.Drawing.Size(102, 27);
            this.buttonSaveDevice.TabIndex = 18;
            this.buttonSaveDevice.Text = "Save Settings";
            this.buttonSaveDevice.UseVisualStyleBackColor = true;
            this.buttonSaveDevice.Click += new System.EventHandler(this.buttonSaveDevice_Click);
            // 
            // trackBrightness
            // 
            this.trackBrightness.BackColor = System.Drawing.SystemColors.Window;
            this.trackBrightness.Location = new System.Drawing.Point(75, 112);
            this.trackBrightness.Maximum = 60;
            this.trackBrightness.Name = "trackBrightness";
            this.trackBrightness.Size = new System.Drawing.Size(164, 45);
            this.trackBrightness.TabIndex = 24;
            this.trackBrightness.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBrightness.Scroll += new System.EventHandler(this.trackBarsHSB_Scroll);
            // 
            // trackSaturation
            // 
            this.trackSaturation.BackColor = System.Drawing.SystemColors.Window;
            this.trackSaturation.Location = new System.Drawing.Point(75, 86);
            this.trackSaturation.Maximum = 100;
            this.trackSaturation.Name = "trackSaturation";
            this.trackSaturation.Size = new System.Drawing.Size(164, 45);
            this.trackSaturation.TabIndex = 23;
            this.trackSaturation.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackSaturation.Scroll += new System.EventHandler(this.trackBarsHSB_Scroll);
            // 
            // trackHue
            // 
            this.trackHue.BackColor = System.Drawing.SystemColors.Window;
            this.trackHue.Location = new System.Drawing.Point(75, 61);
            this.trackHue.Maximum = 360;
            this.trackHue.Name = "trackHue";
            this.trackHue.Size = new System.Drawing.Size(164, 45);
            this.trackHue.TabIndex = 22;
            this.trackHue.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackHue.Scroll += new System.EventHandler(this.trackBarsHSB_Scroll);
            // 
            // pictureModded
            // 
            this.pictureModded.Location = new System.Drawing.Point(209, 19);
            this.pictureModded.Name = "pictureModded";
            this.pictureModded.Size = new System.Drawing.Size(30, 30);
            this.pictureModded.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureModded.TabIndex = 20;
            this.pictureModded.TabStop = false;
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClose.Location = new System.Drawing.Point(445, 360);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(102, 30);
            this.buttonClose.TabIndex = 4;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font("Segoe UI Semilight", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(186)));
            this.label4.Location = new System.Drawing.Point(43, 364);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(126, 21);
            this.label4.TabIndex = 5;
            this.label4.Text = "AudioSwitch v2.0";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.Image = global::AudioSwitch.Properties.Resources.spkr;
            this.pictureBox1.Location = new System.Drawing.Point(9, 359);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(37, 34);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 6;
            this.pictureBox1.TabStop = false;
            // 
            // linkManual
            // 
            this.linkManual.AutoSize = true;
            this.linkManual.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.linkManual.LinkColor = System.Drawing.SystemColors.HotTrack;
            this.linkManual.Location = new System.Drawing.Point(474, 14);
            this.linkManual.Name = "linkManual";
            this.linkManual.Size = new System.Drawing.Size(75, 13);
            this.linkManual.TabIndex = 7;
            this.linkManual.TabStop = true;
            this.linkManual.Text = "Online Manual";
            this.linkManual.VisitedLinkColor = System.Drawing.SystemColors.HotTrack;
            this.linkManual.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkManual_LinkClicked);
            // 
            // listDevices
            // 
            this.listDevices.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listDevices.BackColor = System.Drawing.SystemColors.Window;
            this.listDevices.FullRowSelect = true;
            this.listDevices.HideSelection = false;
            this.listDevices.Location = new System.Drawing.Point(6, 6);
            this.listDevices.MultiSelect = false;
            this.listDevices.Name = "listDevices";
            this.listDevices.Scrollable = false;
            this.listDevices.Size = new System.Drawing.Size(256, 304);
            this.listDevices.TabIndex = 1;
            this.listDevices.TileSize = new System.Drawing.Size(222, 40);
            this.listDevices.UseCompatibleStateImageBehavior = false;
            this.listDevices.View = System.Windows.Forms.View.Tile;
            this.listDevices.SelectedIndexChanged += new System.EventHandler(this.listDevices_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(11, 24);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(108, 13);
            this.label6.TabIndex = 18;
            this.label6.Text = "Default GUI Devices:";
            // 
            // comboDefMode
            // 
            this.comboDefMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboDefMode.FormattingEnabled = true;
            this.comboDefMode.Location = new System.Drawing.Point(125, 21);
            this.comboDefMode.Name = "comboDefMode";
            this.comboDefMode.Size = new System.Drawing.Size(121, 21);
            this.comboDefMode.TabIndex = 19;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.groupBox3);
            this.groupBox1.Controls.Add(this.numTimeout);
            this.groupBox1.Controls.Add(this.trackTransparency);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(186)));
            this.groupBox1.Location = new System.Drawing.Point(9, 9);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(510, 154);
            this.groupBox1.TabIndex = 18;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "OSD Settings";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(314, 17);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(72, 13);
            this.label8.TabIndex = 8;
            this.label8.Text = "Transparency";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(308, 104);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(107, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Closing Timeout (ms):";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // trackTransparency
            // 
            this.trackTransparency.BackColor = System.Drawing.SystemColors.Window;
            this.trackTransparency.Location = new System.Drawing.Point(317, 37);
            this.trackTransparency.Maximum = 255;
            this.trackTransparency.Name = "trackTransparency";
            this.trackTransparency.Size = new System.Drawing.Size(160, 45);
            this.trackTransparency.TabIndex = 14;
            this.trackTransparency.Value = 255;
            this.trackTransparency.Scroll += new System.EventHandler(this.trackTransparency_ValueChanged);
            // 
            // numTimeout
            // 
            this.numTimeout.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numTimeout.Location = new System.Drawing.Point(421, 102);
            this.numTimeout.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numTimeout.Minimum = new decimal(new int[] {
            500,
            0,
            0,
            -2147483648});
            this.numTimeout.Name = "numTimeout";
            this.numTimeout.Size = new System.Drawing.Size(56, 20);
            this.numTimeout.TabIndex = 10;
            this.numTimeout.Value = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.numTimeout.ValueChanged += new System.EventHandler(this.numTimeout_ValueChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.labelAuthor);
            this.groupBox3.Controls.Add(this.linkWebpage);
            this.groupBox3.Controls.Add(this.labelVersion);
            this.groupBox3.Controls.Add(this.comboOSDSkin);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Location = new System.Drawing.Point(8, 16);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(273, 131);
            this.groupBox3.TabIndex = 8;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Skin";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(186)));
            this.label1.Location = new System.Drawing.Point(16, 65);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 12);
            this.label1.TabIndex = 8;
            this.label1.Text = "Version";
            // 
            // comboOSDSkin
            // 
            this.comboOSDSkin.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboOSDSkin.FormattingEnabled = true;
            this.comboOSDSkin.Location = new System.Drawing.Point(18, 22);
            this.comboOSDSkin.Name = "comboOSDSkin";
            this.comboOSDSkin.Size = new System.Drawing.Size(181, 21);
            this.comboOSDSkin.TabIndex = 12;
            this.comboOSDSkin.SelectedIndexChanged += new System.EventHandler(this.comboOSDSkin_SelectedIndexChanged);
            // 
            // labelVersion
            // 
            this.labelVersion.AutoSize = true;
            this.labelVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(186)));
            this.labelVersion.Location = new System.Drawing.Point(54, 65);
            this.labelVersion.Name = "labelVersion";
            this.labelVersion.Size = new System.Drawing.Size(29, 12);
            this.labelVersion.TabIndex = 16;
            this.labelVersion.Text = "<ver>";
            // 
            // linkWebpage
            // 
            this.linkWebpage.AutoSize = true;
            this.linkWebpage.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.linkWebpage.LinkColor = System.Drawing.SystemColors.HotTrack;
            this.linkWebpage.Location = new System.Drawing.Point(15, 81);
            this.linkWebpage.Name = "linkWebpage";
            this.linkWebpage.Size = new System.Drawing.Size(66, 13);
            this.linkWebpage.TabIndex = 15;
            this.linkWebpage.TabStop = true;
            this.linkWebpage.Text = "<Webpage>";
            this.linkWebpage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.linkWebpage.VisitedLinkColor = System.Drawing.SystemColors.HotTrack;
            this.linkWebpage.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkWebpage_LinkClicked);
            // 
            // labelAuthor
            // 
            this.labelAuthor.AutoSize = true;
            this.labelAuthor.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(186)));
            this.labelAuthor.Location = new System.Drawing.Point(15, 52);
            this.labelAuthor.Name = "labelAuthor";
            this.labelAuthor.Size = new System.Drawing.Size(58, 13);
            this.labelAuthor.TabIndex = 8;
            this.labelAuthor.Text = "<Author>";
            this.labelAuthor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // checkDefaultMultiAndComm
            // 
            this.checkDefaultMultiAndComm.AutoSize = true;
            this.checkDefaultMultiAndComm.Location = new System.Drawing.Point(14, 48);
            this.checkDefaultMultiAndComm.Name = "checkDefaultMultiAndComm";
            this.checkDefaultMultiAndComm.Size = new System.Drawing.Size(271, 17);
            this.checkDefaultMultiAndComm.TabIndex = 20;
            this.checkDefaultMultiAndComm.Text = "Switch both Multimedia and Communications device";
            this.checkDefaultMultiAndComm.UseVisualStyleBackColor = true;
            this.checkDefaultMultiAndComm.CheckedChanged += new System.EventHandler(this.checkDefaultMultiAndComm_CheckedChanged);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.checkVolScroll);
            this.groupBox4.Controls.Add(this.checkScrShowOSD);
            this.groupBox4.Controls.Add(this.comboScrollKey);
            this.groupBox4.Controls.Add(this.label2);
            this.groupBox4.Location = new System.Drawing.Point(9, 169);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(510, 53);
            this.groupBox4.TabIndex = 8;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Volume Scrolling";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(245, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(85, 13);
            this.label2.TabIndex = 23;
            this.label2.Text = " + Mouse Wheel";
            // 
            // comboScrollKey
            // 
            this.comboScrollKey.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboScrollKey.FormattingEnabled = true;
            this.comboScrollKey.Location = new System.Drawing.Point(85, 21);
            this.comboScrollKey.Name = "comboScrollKey";
            this.comboScrollKey.Size = new System.Drawing.Size(161, 21);
            this.comboScrollKey.TabIndex = 21;
            this.comboScrollKey.SelectedIndexChanged += new System.EventHandler(this.comboScrollKey_SelectedIndexChanged);
            // 
            // checkScrShowOSD
            // 
            this.checkScrShowOSD.AutoSize = true;
            this.checkScrShowOSD.Location = new System.Drawing.Point(390, 23);
            this.checkScrShowOSD.Name = "checkScrShowOSD";
            this.checkScrShowOSD.Size = new System.Drawing.Size(79, 17);
            this.checkScrShowOSD.TabIndex = 8;
            this.checkScrShowOSD.Text = "Show OSD";
            this.checkScrShowOSD.UseVisualStyleBackColor = true;
            this.checkScrShowOSD.CheckedChanged += new System.EventHandler(this.checkScrShowOSD_CheckedChanged);
            // 
            // checkVolScroll
            // 
            this.checkVolScroll.AutoSize = true;
            this.checkVolScroll.Location = new System.Drawing.Point(14, 23);
            this.checkVolScroll.Name = "checkVolScroll";
            this.checkVolScroll.Size = new System.Drawing.Size(65, 17);
            this.checkVolScroll.TabIndex = 22;
            this.checkVolScroll.Text = "Enabled";
            this.checkVolScroll.UseVisualStyleBackColor = true;
            this.checkVolScroll.CheckedChanged += new System.EventHandler(this.checkVolScroll_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.comboDefMode);
            this.groupBox2.Controls.Add(this.checkDefaultMultiAndComm);
            this.groupBox2.Location = new System.Drawing.Point(9, 228);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(510, 76);
            this.groupBox2.TabIndex = 21;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Misc";
            // 
            // tabGeneral
            // 
            this.tabGeneral.Controls.Add(this.groupBox2);
            this.tabGeneral.Controls.Add(this.groupBox4);
            this.tabGeneral.Controls.Add(this.groupBox1);
            this.tabGeneral.Location = new System.Drawing.Point(4, 22);
            this.tabGeneral.Name = "tabGeneral";
            this.tabGeneral.Size = new System.Drawing.Size(528, 316);
            this.tabGeneral.TabIndex = 2;
            this.tabGeneral.Text = "General";
            this.tabGeneral.UseVisualStyleBackColor = true;
            this.tabGeneral.Enter += new System.EventHandler(this.tabOSD_Enter);
            this.tabGeneral.Leave += new System.EventHandler(this.tabOSD_Leave);
            // 
            // FormSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(560, 399);
            this.Controls.Add(this.linkManual);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.tabSettings);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormSettings";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "AudioSwitch - Settings";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormSettings_FormClosing);
            this.Load += new System.EventHandler(this.FormSettings_Load);
            this.tabSettings.ResumeLayout(false);
            this.tabHotkeys.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridHotkeys)).EndInit();
            this.tabDevices.ResumeLayout(false);
            this.groupDevice.ResumeLayout(false);
            this.groupDevice.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBrightness)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackSaturation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackHue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureModded)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackTransparency)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTimeout)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tabGeneral.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabSettings;
        private System.Windows.Forms.TabPage tabHotkeys;
        private System.Windows.Forms.TabPage tabDevices;
        private System.Windows.Forms.DataGridView gridHotkeys;
        private System.Windows.Forms.Button buttonClose;
        private Controls.CustomListView listDevices;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.LinkLabel linkManual;
        private System.Windows.Forms.GroupBox groupDevice;
        private System.Windows.Forms.CheckBox checkHideDevice;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button buttonResetDevice;
        private System.Windows.Forms.Button buttonSaveDevice;
        private System.Windows.Forms.TrackBar trackBrightness;
        private System.Windows.Forms.TrackBar trackSaturation;
        private System.Windows.Forms.TrackBar trackHue;
        private System.Windows.Forms.PictureBox pictureModded;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.DataGridViewComboBoxColumn Function;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Control;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Alt;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Shift;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Win;
        private System.Windows.Forms.DataGridViewComboBoxColumn HotKey;
        private System.Windows.Forms.DataGridViewCheckBoxColumn ShowOSD;
        private System.Windows.Forms.TabPage tabGeneral;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox comboDefMode;
        private System.Windows.Forms.CheckBox checkDefaultMultiAndComm;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.CheckBox checkVolScroll;
        private System.Windows.Forms.CheckBox checkScrShowOSD;
        private System.Windows.Forms.ComboBox comboScrollKey;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label labelAuthor;
        private System.Windows.Forms.LinkLabel linkWebpage;
        private System.Windows.Forms.Label labelVersion;
        private System.Windows.Forms.ComboBox comboOSDSkin;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numTimeout;
        private System.Windows.Forms.TrackBar trackTransparency;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label8;
    }
}