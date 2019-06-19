using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace ServerApp
{
    class Program
    {
        static void Main(string[] args)
        {
            IPAddress ip = IPAddress.Parse("0.0.0.0"); //мы принимаем всех user
            //IPAddress ip = IPAddress.Parse("127.0.0.1"); это локал хост /ping -4 localhost
            //IPAddress ip = IPAddress.Parse("192.168.56.1");//mask 255.255.0.0

            int port = 12345;
            Server(ip,port);
        }

        //0 - 1000 - ftp, http, dns
        //1000 - 10000 user ports
        // > 10000 - temporary port

        static void Server(IPAddress ipAddr, int port)
        {
            //Сокет сервера
            Socket serverSocket = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);
            //Связывание сокета сервера с конечной точкой:
            //IP-адрес + порт
            IPEndPoint serverEndPoint = new IPEndPoint(ipAddr, port);
            serverSocket.Bind(serverEndPoint);
            //Публикация сокета сервера в сети - открытие доступа к серверу
            serverSocket.Listen(100);

            while (true)
            {
                //Ожидание вязи со стороны
                Socket clientSocket = serverSocket.Accept();
                Console.WriteLine($"Client connected: {clientSocket.RemoteEndPoint.ToString()}");

                try
                {
                    while (clientSocket.Connected)
                    {
                        //Протокол работы эхо-сервера (порт 7 echo)
                        byte[] buf = new byte[1024];
                        //принять сообщение от клиента
                        int recSize = clientSocket.Receive(buf);

                        if (recSize == 0) break;

                        Console.WriteLine($"Received {recSize} bytes");
                        Console.WriteLine(Encoding.UTF8.GetString(buf));
                        //передать получное сообщение удаленному клиенту
                        int sendSize = clientSocket.Send(buf, SocketFlags.None);
                        Console.WriteLine($"Send to client {sendSize} bytes");

                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"Error: {exception.Message}");
                }

            }

        }
    }
}
