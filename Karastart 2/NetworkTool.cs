using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Net;

namespace Sintec.Tool
{
    public class NetworkTool
    {
        public static bool PingHost(string nameOrAddress)
        {
            bool pingable = false;
            Ping pinger = new Ping();
            try
            {
                PingReply reply = pinger.Send(nameOrAddress);
                pingable = reply.Status == IPStatus.Success;
            }
            catch (PingException)
            {
                // Discard PingExceptions and return false;
            }
            return pingable;
        }

        [DllImport("iphlpapi.dll", ExactSpelling = true)]
        public static extern int SendARP(IPAddress DestIP, int SrcIP, byte[] pMacAddr, ref uint PhyAddrLen);

        public static bool PingArpHost(IPAddress address)
        {
            bool res = false;
            byte[] macAddr = new byte[6];
            uint macAddrLen = (uint)macAddr.Length;
            res = (SendARP(address, 0, macAddr, ref macAddrLen) == 0);
            return res;
        }
    }
}
