using System;
using System.Drawing;
using System.Windows.Forms;
using AudioSwitch.Classes;
using AudioSwitch.CoreAudioApi;
using AudioSwitch.Properties;

namespace AudioSwitch.Controls
{
    public partial class VolumeBar : UserControl
    {
        public EventHandler VolumeMuteChanged;
        public MMDevice Device;
        public bool Stereo;

        private VolEventsHandler volEvents;
        private static DateTime LastScroll = DateTime.Now;
        private static readonly TimeSpan ShortInterval = new TimeSpan(0, 0, 0, 0, 80);
        
        private Point pMousePosition = Point.Empty;
        private bool Moving;
        private readonly ToolTip handleTip = new ToolTip();

        private bool _mute;
        public bool Mute
        {
            get { return _mute; }
            set
            {
                _mute = value;
                Thumb.BackgroundImage.Dispose();
                Thumb.BackgroundImage = value ? Resources.ThumbMute : Resources.ThumbNormal;
                if (VolumeMuteChanged != null)
                    VolumeMuteChanged(this, null);
            }
        }

        private float _value;
        public float Value
        {
            get { return _value; }
            set
            {
                _value = value;
                MoveThumb();
                if (VolumeMuteChanged != null)
                    VolumeMuteChanged(this, null);
            }
        }

        public VolumeBar()
        {
            InitializeComponent();
            handleTip.SetToolTip(Thumb, "Master Volume");
        }

        public void ChangeMute()
        {
            Mute = !Mute;
            Device.AudioEndpointVolume.Mute = Mute;
        }

        public void ChangeVolume(float value)
        {
            _value = value;
            Device.AudioEndpointVolume.MasterVolumeLevelScalar = value;
            if (VolumeMuteChanged != null)
                VolumeMuteChanged(this, null);
        }

        private void MoveThumb()
        {
            var trackStep = (double)(ClientSize.Width - Thumb.Width);
            Thumb.Left = (int)(_value * trackStep);
        }

        private void Thumb_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                pMousePosition = Thumb.PointToClient(MousePosition);
                Moving = true;
            }
            else
            {
                ChangeMute();
            }
        }

        private void Thumb_MouseUp(object sender, MouseEventArgs e)
        {
            Moving = false;
        }

        private void Thumb_MouseMove(object sender, MouseEventArgs e)
        {
            if (Moving && e.Button == MouseButtons.Left)
            {
                var theFormPosition = PointToClient(MousePosition);
                theFormPosition.X -= pMousePosition.X;

                if (theFormPosition.X > Width - Thumb.Width)
                    theFormPosition.X = Width - Thumb.Width;

                if (theFormPosition.X < 0)
                    theFormPosition.X = 0;

                Thumb.Left = theFormPosition.X;
                Thumb.Refresh();

                var trackStep = (float)(ClientSize.Width - Thumb.Width);
                ChangeVolume(Thumb.Left / trackStep);
            }
        }
        
        private void lblGraph_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Moving = true;
                var theFormPosition = PointToClient(MousePosition);
                theFormPosition.X -= Thumb.Width / 2;

                if (theFormPosition.X > Width - Thumb.Width)
                    theFormPosition.X = Width - Thumb.Width;

                if (theFormPosition.X < 0)
                    theFormPosition.X = 0;

                Thumb.Left = theFormPosition.X;
            }
            else
            {
                ChangeMute();
            }
        }

        private void lblGraph_MouseMove(object sender, MouseEventArgs e)
        {
            if (Moving && e.Button == MouseButtons.Left)
            {
                var theFormPosition = PointToClient(MousePosition);
                theFormPosition.X -= Thumb.Width / 2;

                if (theFormPosition.X > Width - Thumb.Width)
                    theFormPosition.X = Width - Thumb.Width;

                if (theFormPosition.X < 0)
                    theFormPosition.X = 0;

                Thumb.Left = theFormPosition.X;

                var trackStep = (float)(ClientSize.Width - Thumb.Width);
                ChangeVolume(Thumb.Left / trackStep);
            }
        }

        private void lblGraph_MouseUp(object sender, MouseEventArgs e)
        {
            Moving = false;
        }

        private void Thumb_Move(object sender, EventArgs e)
        {
            Thumb.Refresh();
            lblGraph.Refresh();
        }

        private void Thumb_MouseEnter(object sender, EventArgs e)
        {
            if (_mute) return;
            Thumb.BackgroundImage.Dispose();
            Thumb.BackgroundImage = Resources.ThumbHover;
        }

        private void Thumb_MouseLeave(object sender, EventArgs e)
        {
            if (_mute) return;
            Thumb.BackgroundImage.Dispose();
            Thumb.BackgroundImage = Resources.ThumbNormal;
        }

        public void DoScroll(object sender, ScrollEventArgs e)
        {
            var amount = DateTime.Now - LastScroll <= ShortInterval ? 0.04f : 0.02f;
            LastScroll = DateTime.Now;

            ChangeVolumeSafe(e.NewValue, amount);
        }

        private void ChangeVolumeSafe(int direction, float amount)
        {
            if (direction > 0)
                if (Value <= 1 - amount)
                    ChangeVolume(Value + amount);
                else
                    ChangeVolume(1);

            else if (direction < 0)
                if (Value >= amount)
                    ChangeVolume(Value - amount);
                else
                    ChangeVolume(0);
        }

        private void VolNotify(AudioVolumeNotificationData data)
        {
            if (InvokeRequired)
                Invoke(new AudioEndpointVolumeNotificationDelegate(VolNotify),
                       new object[] {data});
            else
            {
                if (!Moving)
                    Value = data.MasterVolume;
                Mute = data.Muted;
            }
        }

        public void RegisterDevice(EDataFlow RenderType)
        {
            if (Device != null)
            {
                try
                {
                    Device.AudioEndpointVolume.OnVolumeNotification -= VolNotify;
                    if (volEvents != null)
                        Device.AudioSessionManager.Sessions[0].UnregisterAudioSessionNotification(volEvents);
                    volEvents = null;
                }
                catch { }
                Device = null;
            }

            Device = EndPoints.DeviceEnumerator.GetDefaultAudioEndpoint(RenderType, ERole.eMultimedia);
            Value = Device.AudioEndpointVolume.MasterVolumeLevelScalar;
            Mute = Device.AudioEndpointVolume.Mute;
            Stereo = Device.AudioMeterInformation.Channels.GetCount() > 1;
            volEvents = new VolEventsHandler(this);
            Device.AudioSessionManager.Sessions[0].RegisterAudioSessionNotification(volEvents);
            Device.AudioEndpointVolume.OnVolumeNotification += VolNotify;
        }
    }
}
