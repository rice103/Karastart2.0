using System.Collections.Generic;
using System.Drawing;
using AudioSwitch.CoreAudioApi;

namespace AudioSwitch.Classes
{
    public static class EndPoints
    {
        public static readonly List<string> DeviceNames = new List<string>();
        public static int DefaultDeviceID;

        public static readonly MMDeviceEnumerator DeviceEnumerator = new MMDeviceEnumerator();
        private static readonly Dictionary<int, string> DeviceIDs = new Dictionary<int, string>(); 
        private static readonly PolicyConfigClient pPolicyConfig = new PolicyConfigClient();

        public static void RefreshDeviceList(EDataFlow renderType)
        {
            DeviceNames.Clear();
            DeviceIDs.Clear();
            
            var pDevices = DeviceEnumerator.EnumerateAudioEndPoints(renderType, EDeviceState.Active);
            var defDeviceID = DeviceEnumerator.GetDefaultAudioEndpoint(renderType, ERole.eMultimedia).ID;
            var devCount = pDevices.Count;

            for (var i = 0; i < devCount; i++)
            {
                var device = pDevices[i];
                var devID = device.ID;
                DeviceNames.Add(device.FriendlyName);
                DeviceIDs.Add(i, devID);

                if (devID == defDeviceID)
                    DefaultDeviceID = i;
            }
        }

        public static string SetNextDefault(EDataFlow rType)
        {
            RefreshDeviceList(rType);

            if (DefaultDeviceID == DeviceNames.Count - 1)
                DefaultDeviceID = 0;
            else
                DefaultDeviceID++;

            SetDefaultDevice(DefaultDeviceID);
            return DeviceNames[DefaultDeviceID];
        }

        public static MMDevice GetDefaultMMDevice(EDataFlow renderType)
        {
            return DeviceEnumerator.GetDefaultAudioEndpoint(renderType, ERole.eMultimedia);
        }
        
        public static Dictionary<MMDevice, Icon> GetAllDeviceList()
        {
            var devices = new Dictionary<MMDevice, Icon>();
            var pDevices = DeviceEnumerator.EnumerateAudioEndPoints(EDataFlow.eAll, EDeviceState.Active);
            var devCount = pDevices.Count;

            for (var i = 0; i < devCount; i++)
            {
                var device = pDevices[i];
                devices.Add(device, DeviceIcons.GetIcon(device.IconPath));
            }

            return devices;
        }

        public static void SetDefaultDevice(int devID)
        {
            SetDefaultDevice(DeviceIDs[devID]);
        }

        public static void SetDefaultDevice(string devID)
        {
            try
            {
                pPolicyConfig.SetDefaultEndpoint(devID, ERole.eMultimedia);
                if (Program.settings.DefaultMultimediaAndComm)
                    pPolicyConfig.SetDefaultEndpoint(devID, ERole.eCommunications);
            }
            catch
            {
                if (!Program.settings.IgnoreErrors)
                    throw;
            }
        }
    }
}
