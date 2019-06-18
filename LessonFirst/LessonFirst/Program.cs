using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace LessonFirst
{
    class Program
    {
        static void Main(string[] args)
        {
            string compName = Dns.GetHostName();

            Console.WriteLine($"This host name: {compName}");

            IPHostEntry ipList = Dns.GetHostByName(compName);

            foreach (var a in ipList.AddressList)
            {
                Console.WriteLine($"Address list {a.ToString()}");
            }

            Console.WriteLine("Введите имя сетевого ресурса");

            compName = Console.ReadLine();
            ipList = Dns.GetHostByName(compName);

            foreach (var item in ipList.AddressList)
            {
                Console.WriteLine(item.ToString());
            }

            Socket socket = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp);
            try
            {
                socket.Connect(ipList.AddressList[0], 443);
                if (socket.Connected)
                {
                    string httpRequest = "GET/HTTP 1.0/";
                    socket.Send(Encoding.ASCII.GetBytes(httpRequest));
                    byte[] buf = new byte[1024*4];
                    socket.Receive(buf);
                    Console.WriteLine($"Received: {Encoding.UTF8.GetString(buf)}");
                }
                else
                {
                    Console.WriteLine("Error: connected fail!");
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Упс что-то пошло не так{exception.Message}");
            }

            Console.ReadLine();
        }
    }
}
