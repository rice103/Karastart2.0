﻿using System;
using System.Windows.Forms;

namespace AudioSwitch.Controls
{
    public sealed class CustomListView : ListView
    {
        public event ScrollEventHandler Scroll;

        public CustomListView()
        {
            DoubleBuffered = true;
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x020A)
            {
                try
                {
                    var bytes = BitConverter.GetBytes((int)m.WParam);
                    var y = BitConverter.ToInt16(bytes, 2);
                    Scroll(this, new ScrollEventArgs((ScrollEventType)(m.WParam.ToInt32() & 0xffff), y));
                }
                catch {}
            }
            else
                base.WndProc(ref m);
        }
    }
}
