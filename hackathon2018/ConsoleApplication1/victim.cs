using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace victim
{
    class victim
    {
        private static Random random = new Random();
        private static List<DateTime> timesList = new List<DateTime>();

        static void Main(string[] args)
        {
            try
            {
                int port = random.Next(49152, 65535);
                Console.Write("\nport: " + port);
                string pass = RandomString(6);
                //const int PORT_NO = 5000;
                const string SERVER_IP = "127.0.0.1";

                Console.Write("\npassword: " + pass + "\n");

                //---listen at the specified IP and port no.---
                IPAddress localAdd = IPAddress.Parse(SERVER_IP);
                TcpListener listener = new TcpListener(localAdd, port);
                Console.WriteLine("\nListening...");
                listener.Start();
                while (true) { connection(listener, pass); }
            }
            catch { }
        }

        public static void connection(TcpListener listener, string pass)
        {
            //---incoming client connected---
            TcpClient client = listener.AcceptTcpClient();

            //---get the incoming data through a network stream---
            NetworkStream nwStream = client.GetStream();
            byte[] buffer = new byte[client.ReceiveBufferSize];

            //---write back the text to the client---
            string strMess = "Please enter your password\r\n";
            byte[] mess = Encoding.ASCII.GetBytes(strMess);
            nwStream.Write(mess, 0, mess.Length);

            //---read incoming stream---
            int bytesRead = nwStream.Read(buffer, 0, buffer.Length);

            //---convert the data received into a string---
            string passRecieved = Encoding.ASCII.GetString(buffer, 0, bytesRead);

            if (pass == passRecieved)
            {
                strMess = "Access granted";
                mess = Encoding.ASCII.GetBytes(strMess);
                nwStream.Write(mess, 0, mess.Length);
                bytesRead = nwStream.Read(buffer, 0, buffer.Length);
                string recieve = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                if (addToTimesList() >= 10)
                    Console.Write(recieve);
            }
            client.Close();
            nwStream.Close();
        }

        public static int addToTimesList()
        {
            DateTime time = DateTime.Now;
            int i = 0;
            while (i < timesList.Count)
            {
                if (timesList[i].Equals(time))
                    timesList.RemoveAt(i);
                else ++i;
            }
            if (timesList.Count < 10)
                timesList.Add(time);
            return timesList.Count;
        }

        public static string RandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
