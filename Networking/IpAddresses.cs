using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Sokoban.Lib;

namespace Sokoban.Networking
{
    public class IpAddresses
    {
        public static IPAddress[] GetLocalIpAddresses()
        {
            return Dns.GetHostAddresses(Dns.GetHostName());
        }

        public static bool IsLocalIpAddress(string host)
        {
            try
            {
                // get host IP addresses
                IPAddress[] hostIPs = Dns.GetHostAddresses(host);
                // get local IP addresses
                IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());

                // test if any host IP equals to any local IP or to localhost
                foreach (IPAddress hostIP in hostIPs)
                {
                    // is localhost
                    if (IPAddress.IsLoopback(hostIP)) return true;
                    // is local address
                    foreach (IPAddress localIP in localIPs)
                    {
                        if (hostIP.Equals(localIP)) return true;
                    }
                }
            }
            catch (SocketException e)
            {
                DebuggerIX.WriteLine("SocketException: " + e.Message);
            }
            catch (ArgumentOutOfRangeException e)
            {
                DebuggerIX.WriteLine("ArgumentOutOfRangeException: " + e.Message);
            }
            catch (ArgumentException e)
            {
                DebuggerIX.WriteLine("ArgumentException: " + e.Message);
            }
            catch (Exception e)
            {
                DebuggerIX.WriteLine("Exception: " + e.Message);
            }

            return false;
        }

        /// <summary>
        /// Get machine IPv4 address
        /// </summary>
        /// <returns></returns>
        public static string GetMyIPAddress()
        {
            IPHostEntry host;
            string localIP = "?";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                    break;
                }
            }
            return localIP;
        }
    }
}
