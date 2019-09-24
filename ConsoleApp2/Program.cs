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
    class Program
    {
        static UdpClient udpClient = new UdpClient();
        static IPEndPoint from;
        static void Main(string[] args)
        {
            //=============================================================UDPER
            //UDPer udp = new UDPer();
            //udp.Start();

            //ConsoleKeyInfo cki;
            //do
            //{
            //    if (Console.KeyAvailable)
            //    {
            //        cki = Console.ReadKey(true);
            //        switch (cki.KeyChar)
            //        {
            //            case 's':
            //                udp.Send(new Random().Next().ToString());
            //                break;
            //            case 'x':
            //                udp.Stop();
            //                return;
            //        }
            //    }
            //    Thread.Sleep(10);
            //} while (true);
            //=============================================================UDPER


            //=============================================================UDPSocket
            //UDPSocket s = new UDPSocket();
            //s.Server("127.0.0.1", 27000);

            //Console.ReadKey();
            //=============================================================UDPSocket

            //=============================================================UDPClient
            int PORT = 27000;
            //UdpClient udpClient = new UdpClient();
            udpClient = new UdpClient();
            udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, PORT));

            from = new IPEndPoint(0, 0);
            Task.Run(() =>
            {
                while (true)
                {
                    var recvBuffer = udpClient.Receive(ref from);
                    processMessage(Encoding.UTF8.GetString(recvBuffer), from);
                    Console.WriteLine(Encoding.UTF8.GetString(recvBuffer));                    
                }
            });

            //var data = Encoding.UTF8.GetBytes("ABCD");
            //udpClient.Send(data, data.Length, from);


            //=============================================================UDPClient

            Console.ReadKey();

        }

        public static void processMessage(string message, IPEndPoint targetUdp) {
            try
            {
                if (string.Equals(message, "GetIP"))
                {
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
                    var data = Encoding.UTF8.GetBytes(jsonReply.ToString());
                    udpClient.Send(data, data.Length, targetUdp);
                }
            }
            catch (Exception ex) {
                Console.WriteLine("Error When Processing Message:" + ex.Message);
            }
        }
    }
}
