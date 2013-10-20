using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Net; 

namespace NodeObserver
{
    class NetworkSensor
    {

        public string ip;
        public string hostname;
        public string mac;
        public string status;
        public string date;
        public string time;
        public string day;
        public Thread sense;
        
        public NetworkSensor(string ip) {
            this.ip = ip;
            sense = new Thread(new ThreadStart(sensorThread));
            sense.IsBackground = true;
            sense.Start();
        }

        public void sensorThread() {

            pingServer();
            this.mac = ARP();
            this.hostname = resolveHostName();

            if (this.mac != null)
            {
                this.status = "UP";
            }
            else
            {
                if (this.hostname != null)
                {
                    this.status = "DOWN";
                }
                else {
                    this.status = "UNKNOWN";
                }
            }

            this.day = System.DateTime.Now.DayOfWeek.ToString();
            this.date = System.DateTime.Now.ToShortDateString();
            this.time = System.DateTime.Now.ToString("HH:mm tt");
                       
            //Database mydb = new Database();
            //mydb.insertRecord(this.day, this.date, this.time, this.ip, this.mac, this.hostname, this.status);
        }


        public string resolveHostName() {
            try
            {
                System.Net.IPHostEntry IP = System.Net.Dns.GetHostEntry(ip);
                return IP.HostName;              
            }
            catch (Exception e)
            {
                e.Message.ToString();
                return null;
            }
        }

        public bool pingServer() {

            try
            {
                Ping pingreq = new Ping();
                PingReply rep = pingreq.Send(ip);
                //Console.WriteLine("Pinging {0} [{1}]", mac, rep.Address.ToString());
                if (rep.Status == IPStatus.Success)
                {
                    //Console.WriteLine("Reply From {0} : time={1} TTL={2}", rep.Address.ToString(), rep.RoundtripTime, rep.Options.Ttl);
                    return true;
                }
                else
                {
                    //Console.WriteLine("Ping: " + IPStatus.DestinationHostUnreachable.ToString());
                    return false;
                }
            }
            catch (Exception e) {
                System.Console.WriteLine(e.Message.ToString());
                return false;
            }
        }

        [DllImport("iphlpapi.dll", ExactSpelling = true)]
        public static extern int SendARP(int DestIP, int SrcIP, byte[] pMacAddr, ref uint PhyAddrLen);

        public string ARP() {
            IPAddress dst = IPAddress.Parse(ip);
            int intAddress = BitConverter.ToInt32(dst.GetAddressBytes(), 0);

            byte[] macAddr = new byte[6];
            uint macAddrLen = (uint)macAddr.Length;
            if (SendARP(intAddress, 0, macAddr, ref macAddrLen) != 0)
            {
                return null;
            }

            string[] str = new string[(int)macAddrLen];
            for (int i = 0; i < macAddrLen; i++)
                str[i] = macAddr[i].ToString("x2");

            return string.Join(":", str);
        }

    }
}
