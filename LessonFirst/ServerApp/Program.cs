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

        }

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
            //Ожидание вязи со стороны
            Socket clientSocket = serverSocket.Accept();
            //передать сообщение удаленному клиенту
            //clientSocket.Send();
            //принять сообщение от клиента
            //clientSocket.Receive();
        }
    }
}
