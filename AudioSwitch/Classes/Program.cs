using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using AudioSwitch.CoreAudioApi;
using AudioSwitch.Forms;

namespace AudioSwitch.Classes
{
    public static class Program
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool AllocConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool FreeConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool AttachConsole(int dwProcessId);

        public static FormOSD frmOSD;
        public static FormSwitcher formSwitcher;
        private static bool isConsole;
        public static Settings settings;

        [STAThread]
        private static void Main(string[] args)
        {
            try
            {
                if (args.Length > 0)
                {
                    if (!AttachConsole(-1))
                        AllocConsole();
                    isConsole = true;
                    Console.WriteLine();

                    var cmdArgsJoined = string.Join(" ", args);
                    var cmdArgs = cmdArgsJoined.Split('-');
                    var willExit = false;

                    var hotkeyFunction = HotkeyFunction.SwitchPlaybackDevice;
                    var modifiers = HotModifierKeys.Win;
                    var hotKey = Keys.LWin;
                    var rType = EDataFlow.eAll;
                    var keysOK = 0;
                    
                    foreach (var arg in cmdArgs)
                    {
                        if (string.IsNullOrWhiteSpace(arg))
                            continue;

                        var purecmd = arg.Length > 1 ? arg.Substring(1, arg.Length - 1).Trim() : "";

                        switch (arg.Substring(0, 1))
                        {
                            case "m":
                                if (!Enum.TryParse(purecmd, true, out modifiers))
                                {
                                    Console.WriteLine("Error reading modifier key(s)!");
                                    return;
                                }
                                keysOK++;
                                break;

                            case "k":
                                if (!Enum.TryParse(purecmd, true, out hotKey))
                                {
                                    Console.WriteLine("Error reading hot key!");
                                    return;
                                }
                                keysOK++;
                                break;

                            case "f":
                                if (!Enum.TryParse(purecmd, true, out hotkeyFunction))
                                {
                                    Console.WriteLine("Error reading function name!");
                                    return;
                                }
                                keysOK++;
                                break;

                            case "i":
                                var devID = int.Parse(purecmd);
                                EndPoints.RefreshDeviceList(EDataFlow.eRender);
                                if (devID <= EndPoints.DeviceNames.Count - 1)
                                    EndPoints.SetDefaultDevice(devID);
                                break;

                            case "r":
                                switch (purecmd.ToLower())
                                {
                                    case "playback":
                                        rType = EDataFlow.eRender;
                                        break;
                                    case "recording":
                                        rType = EDataFlow.eCapture;
                                        break;
                                }

                                if (rType == EDataFlow.eAll)
                                {
                                    Console.WriteLine("Error reading render type!");
                                    return;
                                }
                                break;

                            case "l":
                                if (rType == EDataFlow.eAll)
                                {
                                    Console.WriteLine("You must first specify rendering type using '-r' flag!");
                                    return;
                                }
                                Console.WriteLine(" Devices available:");
                                EndPoints.RefreshDeviceList(rType);

                                for (var i = 0; i < EndPoints.DeviceNames.Count; i++)
                                {
                                    if (i == EndPoints.DefaultDeviceID)
                                        Console.WriteLine(" <" + i + "> " + EndPoints.DeviceNames[i]);
                                    else
                                        Console.WriteLine("  " + i + "  " + EndPoints.DeviceNames[i]);
                                }
                                break;

                            case "s":
                                Settings.settingsxml = purecmd;
                                break;

                            case "x":
                                willExit = true;
                                break;
                        }
                    }

                    Settings.Load();

                    if (keysOK == 3)
                    {
                        var hkey = GlobalHotkeys.AddOrFind(hotkeyFunction);
                        hkey.ModifierKeys = modifiers;
                        hkey.HotKey = hotKey;
                        Console.WriteLine("Hot key saved:  {0} => {1} + {2}", hotkeyFunction, modifiers, hotKey);
                        Settings.Save();
                    }

                    if (willExit) return;
                }

                if (settings == null)
                    Settings.Load();

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                frmOSD = new FormOSD();
                formSwitcher = new FormSwitcher();
                Application.Run();
            }
            finally
            {
                if (isConsole)
                {
                    FreeConsole();
                    SendKeys.SendWait("{ENTER}");
                }
            }
        }
    }
}

