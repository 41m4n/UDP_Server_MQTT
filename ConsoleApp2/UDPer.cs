using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    public class UDPer
    {
        const int PORT_NUMBER = 27000;

        Thread t = null;
        public void Start()
        {
            if (t != null)
            {
                throw new Exception("Already started, stop first");
            }
            Console.WriteLine("Started listening");
            StartListening();
        }
        public void Stop()
        {
            try
            {
                udp.Close();
                Console.WriteLine("Stopped listening");
            }
            catch { /* don't care */ }
        }

        private readonly UdpClient udp = new UdpClient(PORT_NUMBER);
        IAsyncResult ar_ = null;

        private void StartListening()
        {
            ar_ = udp.BeginReceive(Receive, new object());
        }
        private void Receive(IAsyncResult ar)
        {
            IPEndPoint ip = new IPEndPoint(IPAddress.Any, PORT_NUMBER);
            byte[] bytes = udp.EndReceive(ar, ref ip);
            string message = Encoding.ASCII.GetString(bytes);
            Console.WriteLine("From {0} received: {1} ", ip.Address.ToString(), message);
            if (string.Equals(message, "GetIP"))
            {
                Send("Goddamn");
            }
            Thread.Sleep(3000);
            StartListening();
        }
        public void Send(string message)
        {
            //UdpClient client = new UdpClient();
            IPEndPoint ip = new IPEndPoint(IPAddress.Parse("255.255.255.255"), PORT_NUMBER);
            var host = Dns.GetHostEntry(Dns.GetHostName());
            Console.WriteLine("Before IP");
            foreach (var ips in host.AddressList)
            {
                Console.WriteLine("In IP");
                if (ips.AddressFamily == AddressFamily.InterNetwork)
                {
                    Console.WriteLine("In IP2");
                    message = ips.ToString();
                }
            }
            Console.WriteLine("After IP");
            JObject jsonReply = new JObject();
            jsonReply.Add("Command", "LocalIP");
            jsonReply.Add("ServerIP", message);

            byte[] bytes = Encoding.ASCII.GetBytes("Testa");
            udp.Send(bytes, bytes.Length, ip);
            //udp.Close();
            Console.WriteLine("Sent: {0} ", jsonReply.ToString());
        }
    }
}
