using System;
using System.Windows.Forms;
using AudioSwitch.GlobalHook;

namespace AudioSwitch.Classes
{
    public static class ScrollVolume
    {
        public static EventHandler VolumeScroll;
        public static bool IsEnabled;

        private static bool Control;
        private static bool Alt;
        private static bool Shift;
        private static bool LWin;
        private static bool RWin;
        private static bool LeftMouseButton;
        private static bool RightMouseButton;

        private static void VolumeIsScrolling(object sender, MouseEventArgs mouseEventArgs)
        {
            switch (Program.settings.VolumeScroll.Key)
            {
                case VolumeScrollKey.Alt:
                    if (Alt)
                        if (VolumeScroll != null)
                            VolumeScroll(null, mouseEventArgs);
                    break;

                case VolumeScrollKey.Control:
                    if (Control)
                        if (VolumeScroll != null)
                            VolumeScroll(null, mouseEventArgs);
                    break;

                case VolumeScrollKey.Shift:
                    if (Shift)
                        if (VolumeScroll != null)
                            VolumeScroll(null, mouseEventArgs);
                    break;

                case VolumeScrollKey.Win:
                    if (LWin || RWin)
                        if (VolumeScroll != null)
                            VolumeScroll(null, mouseEventArgs);
                    break;

                case VolumeScrollKey.LeftMouseButton:
                    if (LeftMouseButton)
                        if (VolumeScroll != null)
                            VolumeScroll(null, mouseEventArgs);
                    break;

                case VolumeScrollKey.RightMouseButton:
                    if (RightMouseButton)
                        if (VolumeScroll != null)
                            VolumeScroll(null, mouseEventArgs);
                    break;
            }
        }

        private static void KeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            if (keyEventArgs.Alt)
                Alt = true;
            else if (keyEventArgs.Control)
                Control = true;
            else if (keyEventArgs.Shift)
                Shift = true;
            else if (keyEventArgs.KeyCode == Keys.LWin)
                LWin = true;
            else if (keyEventArgs.KeyCode == Keys.RWin)
                RWin = true;
        }

        private static void KeyUp(object sender, KeyEventArgs keyEventArgs)
        {
            if (keyEventArgs.Alt)
                Alt = false;
            else if (keyEventArgs.Control)
                Control = false;
            else if (keyEventArgs.Shift)
                Shift = false;
            else if (keyEventArgs.KeyCode == Keys.LWin)
                LWin = false;
            else if (keyEventArgs.KeyCode == Keys.RWin)
                RWin = false;
        }

        private static void MouseDown(object sender, MouseEventArgs mouseEventArgs)
        {
            switch (mouseEventArgs.Button)
            {
                case MouseButtons.Left:
                    LeftMouseButton = true;
                    break;
                case MouseButtons.Right:
                    RightMouseButton = true;
                    break;
            }
        }

        private static void MouseUp(object sender, MouseEventArgs mouseEventArgs)
        {
            switch (mouseEventArgs.Button)
            {
                case MouseButtons.Left:
                    LeftMouseButton = false;
                    break;
                case MouseButtons.Right:
                    RightMouseButton = false;
                    break;
            }
        }

        public static void RegisterVolScroll(bool Enable)
        {
            if (Enable)
            {
                if (IsEnabled)
                    return;

                HookManager.MouseDown += MouseDown;
                HookManager.MouseUp += MouseUp;
                HookManager.KeyDown += KeyDown;
                HookManager.KeyUp += KeyUp;
                HookManager.MouseWheel += VolumeIsScrolling;
                IsEnabled = true;
            }
            else
            {
                if (!IsEnabled)
                    return;

                HookManager.MouseDown -= MouseDown;
                HookManager.MouseUp -= MouseUp;
                HookManager.KeyDown -= KeyDown;
                HookManager.KeyUp -= KeyUp;
                HookManager.MouseWheel -= VolumeIsScrolling;
                IsEnabled = false;
            }
        }
    }
}
