﻿using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using AudioSwitch.Classes;
using AudioSwitch.CoreAudioApi;

namespace AudioSwitch.Forms
{
    public partial class FormSettings : Form
    {
        private bool HotkeysChanged;

        public FormSettings()
        {
            InitializeComponent();

            FormSwitcher.SetWindowTheme(listDevices.Handle, "explorer", null);
            var tile = new Size(listDevices.ClientSize.Width, (int)(listDevices.TileSize.Height * FormSwitcher.DpiFactor));
            listDevices.TileSize = tile;

            var size = new Size((int)(32 * FormSwitcher.DpiFactor), (int)(32 * FormSwitcher.DpiFactor));
            listDevices.LargeImageList = new ImageList
            {
                ImageSize = size,
                ColorDepth = ColorDepth.Depth32Bit
            };

            Function.DataSource = Enum.GetNames(typeof(HotkeyFunction));
            Function.DataPropertyName = "HKFunction";

            HotKey.DataSource = Enum.GetNames(typeof(Keys));
            HotKey.DataPropertyName = "Key";

            Control.DataPropertyName = "Control";
            Alt.DataPropertyName = "Alt";
            Shift.DataPropertyName = "Shift";
            Win.DataPropertyName = "Win";
            ShowOSD.DataPropertyName = "ShowOSD";

            gridHotkeys.DataSource = Program.settings.Hotkey;
            gridHotkeys.CellValueChanged += gridHotkeys_CellValueChanged;

            var linkTip = new ToolTip();
            linkTip.SetToolTip(linkManual, "https://code.google.com/p/audioswitch/wiki/Manual");
        }

        private void FormSettings_Load(object sender, EventArgs e)
        {
            var devices = EndPoints.GetAllDeviceList();
            var cnt = 0;
            foreach (var dev in devices)
            {
                var devID = dev.Key.ID;
                var lvitem = new ListViewItem { Text = dev.Key.FriendlyName, ImageIndex = cnt, Tag = devID };

                var devSettings = Program.settings.Device.Find(x => x.DeviceID == devID);
                if (devSettings != null)
                {
                    lvitem.Font = new Font(lvitem.Font, FontStyle.Bold);

                    if (devSettings.HideFromList)
                        lvitem.Font = new Font(lvitem.Font, FontStyle.Italic);
                }

                listDevices.LargeImageList.Images.Add(dev.Value);
                listDevices.Items.Add(lvitem);
                cnt++;
            }
            pictureModded.Image = new Bitmap(Properties.Resources._66_100_highDPI);

            var OSDskins = Directory.GetDirectories("Skins");
            foreach (var skinDir in OSDskins)
                comboOSDSkin.Items.Add(skinDir.Substring(skinDir.IndexOf('\\') + 1));
            comboOSDSkin.Text = Program.settings.OSD.Skin;
            trackTransparency.Value = Program.settings.OSD.Transparency;

            comboDefMode.Items.Add("Playback");
            comboDefMode.Items.Add("Recording");
            comboDefMode.Text = Program.settings.DefaultDataFlow == EDataFlow.eCapture ? "Recording" : "Playback";
            checkDefaultMultiAndComm.Checked = Program.settings.DefaultMultimediaAndComm;
            
            checkVolScroll.Checked = Program.settings.VolumeScroll.Enabled;
            comboScrollKey.DataSource = Enum.GetNames(typeof(VolumeScrollKey));
            comboScrollKey.Text = Program.settings.VolumeScroll.Key.ToString();
            checkScrShowOSD.Checked = Program.settings.VolumeScroll.ShowOSD;
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void gridHotkeys_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            HotkeysChanged = true;
        }

        private void FormSettings_FormClosing(object sender, FormClosingEventArgs e)
        {
            tabOSD_Leave(sender, e);

            if (HotkeysChanged)
            {
                for (var i = 0; i < gridHotkeys.Rows.Count - 1; i++)
                {
                    var hasHotKeyErrors = Program.settings.Hotkey[i].Register();
                    gridHotkeys.Rows[i].DefaultCellStyle.BackColor = hasHotKeyErrors ? SystemColors.Window : Color.OrangeRed;
                }
            }
            Program.settings.DefaultDataFlow = comboDefMode.SelectedItem.ToString() == "Recording"
                                                   ? EDataFlow.eCapture
                                                   : EDataFlow.eRender;

            Program.settings.DefaultMultimediaAndComm = checkDefaultMultiAndComm.Checked;

            Settings.Save();
        }

        private void tabOSD_Enter(object sender, EventArgs e)
        {
            numTimeout.Value = Program.settings.OSD.ClosingTimeout;

            Program.frmOSD.Show();
            Program.frmOSD.LoadSkin();
            Program.frmOSD.SetVolImage(0.75f);
            Focus();
        }

        private void tabOSD_Leave(object sender, EventArgs e)
        {
            Program.frmOSD.Hide();
        }

        private void numTimeout_ValueChanged(object sender, EventArgs e)
        {
            Program.settings.OSD.ClosingTimeout = (int)numTimeout.Value;
        }

        private void checkDefaultMultiAndComm_CheckedChanged(object sender, EventArgs e)
        {
            Program.settings.DefaultMultimediaAndComm = checkDefaultMultiAndComm.Checked;
        }

        private void linkManual_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://code.google.com/p/audioswitch/wiki/Manual");
        }

        private void trackTransparency_ValueChanged(object sender, EventArgs e)
        {
            Program.settings.OSD.Transparency = Program.frmOSD.Transparency = (byte)trackTransparency.Value;
            Program.frmOSD.SetVolImage(0.75f);
        }

        private void trackBarsHSB_Scroll(object sender, EventArgs e)
        {
            if (pictureModded.Image != null)
                pictureModded.Image.Dispose();

            pictureModded.Image = DeviceIcons.ChangeColors(new Bitmap(Properties.Resources._66_100_highDPI), trackHue.Value, trackSaturation.Value / 100f,
                                               trackBrightness.Value/100f);
        }

        private void buttonResetDevice_Click(object sender, EventArgs e)
        {
            trackHue.Value = 0;
            trackSaturation.Value = 0;
            trackBrightness.Value = 0;
            pictureModded.Image = new Bitmap(Properties.Resources._66_100_highDPI);
            checkHideDevice.Checked = false;

            listDevices.SelectedItems[0].Font = new Font(listDevices.SelectedItems[0].Font, FontStyle.Regular);

            var devSettings = Program.settings.Device.Find(x => x.DeviceID == (string)listDevices.SelectedItems[0].Tag);
            if (devSettings != null)
                Program.settings.Device.Remove(devSettings);
        }

        private void buttonSaveDevice_Click(object sender, EventArgs e)
        {
            if (listDevices.SelectedItems.Count == 0) return;

            var devSettings = Program.settings.Device.Find(x => x.DeviceID == (string) listDevices.SelectedItems[0].Tag);

            listDevices.SelectedItems[0].Font = new Font(listDevices.SelectedItems[0].Font,
                                                         checkHideDevice.Checked ? FontStyle.Italic : FontStyle.Bold);

            if (devSettings != null)
            {
                devSettings.Brightness = trackBrightness.Value;
                devSettings.Hue = trackHue.Value;
                devSettings.Saturation = trackSaturation.Value;
                devSettings.HideFromList = checkHideDevice.Checked;
            }
            else
            {
                devSettings = new Settings.CDevice
                    {
                        DeviceID = (string) listDevices.SelectedItems[0].Tag,
                        HideFromList = checkHideDevice.Checked,
                        Brightness = trackBrightness.Value,
                        Hue = trackHue.Value,
                        Saturation = trackSaturation.Value
                    };
                Program.settings.Device.Add(devSettings);
            }
        }

        private void listDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listDevices.SelectedItems.Count == 0) return;

            var devSettings = Program.settings.Device.Find(x => x.DeviceID == (string)listDevices.SelectedItems[0].Tag);
            if (devSettings == null)
            {
                trackBrightness.Value = 0;
                trackHue.Value = 0;
                trackSaturation.Value = 0;
                pictureModded.Image = new Bitmap(Properties.Resources._66_100_highDPI);
                checkHideDevice.Checked = false;
                return;
            }

            trackBrightness.Value = devSettings.Brightness;
            trackHue.Value = devSettings.Hue;
            trackSaturation.Value = devSettings.Saturation;
            
            if (pictureModded.Image != null)
                pictureModded.Image.Dispose();

            pictureModded.Image = DeviceIcons.ChangeColors(new Bitmap(Properties.Resources._66_100_highDPI), trackHue.Value, trackSaturation.Value / 100f,
                                               trackBrightness.Value / 100f);
            checkHideDevice.Checked = devSettings.HideFromList;
        }

        private void comboOSDSkin_SelectedIndexChanged(object sender, EventArgs e)
        {
            Program.settings.OSD.Skin = comboOSDSkin.Text;
            Program.frmOSD.LoadSkin();
            Program.frmOSD.SetVolImage(0.75f);

            labelAuthor.Text = Program.frmOSD.Skin.Author;
            linkWebpage.Text = Program.frmOSD.Skin.Website;
            labelVersion.Text = Program.frmOSD.Skin.Version;
            Focus();
        }

        private void linkWebpage_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (linkWebpage.Links.Count > 0)
                Process.Start(linkWebpage.Text);
        }

        private void checkVolScroll_CheckedChanged(object sender, EventArgs e)
        {
            if (checkVolScroll.Checked)
            {
                comboScrollKey.Enabled = true;
                checkScrShowOSD.Enabled = true;
                Program.settings.VolumeScroll.Enabled = true;
            }
            else
            {
                comboScrollKey.Enabled = false;
                checkScrShowOSD.Enabled = false;
                Program.settings.VolumeScroll.Enabled = false;
            }
        }

        private void comboScrollKey_SelectedIndexChanged(object sender, EventArgs e)
        {
            Program.settings.VolumeScroll.Key =
                (VolumeScrollKey) Enum.Parse(typeof (VolumeScrollKey), comboScrollKey.Text);
        }

        private void checkScrShowOSD_CheckedChanged(object sender, EventArgs e)
        {
            Program.settings.VolumeScroll.ShowOSD = checkScrShowOSD.Checked;
        }
    }
}
