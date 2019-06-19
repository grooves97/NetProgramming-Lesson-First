using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ServerWPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool buttonIsStart;
        private TcpListener serverSock;
        private Thread serverThread;
        private ManualResetEvent eventStop;

        public MainWindow()
        {
            InitializeComponent();

            //eventStop = new ManualResetEvent;
            IPAddresComboBox.Items.Add("0.0.0.0");
            IPAddresComboBox.Items.Add("127.0.0.1");
            IPHostEntry entryServer =  Dns.GetHostEntry(Dns.GetHostName());

            foreach (var i in entryServer.AddressList)
            {
                IPAddresComboBox.Items.Add(i.ToString());
            }

            buttonIsStart = false;
            serverSock = null;
            serverThread = null;
        }
        /// <summary>
        /// Кнопка включения выключения чего-то
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartButtonClick(object sender, RoutedEventArgs e)
        {
            if (!buttonIsStart) //запуск сервера
            {
                try
                {
                    serverSock = new TcpListener(IPAddress.Parse(IPAddresComboBox.Text), int.Parse(PortTextBox.Text));

                    serverSock.Start();
                    serverThread = new Thread(ServerThreadProcess);
                    serverThread.Start(serverSock);
                    buttonIsStart = !buttonIsStart;
                    StartButton.Content = "Stop";
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message,"Error", MessageBoxButton.OK, MessageBoxImage.Stop);
                }
            }
            else
            {
                //Нужен объект синхронизации с потоком сервера

            }

          
        }
        /// <summary>
        /// Поток сервера
        /// </summary>
        /// <param name="obj"></param>
        private void ServerThreadProcess(object obj)
        {
            TcpListener serverSocket = (TcpListener)obj;
            while (true)
            {
                //Вариант первый
                //TcpClient client = serverSocket.AcceptTcpClient();
                //Обработка клиента в поле потоков
                //ThreadPool.QueueUserWorkItem(ThreadClient, client);
                //Вариант второй
                IAsyncResult asyncResult = serverSock.BeginAcceptSocket(AsyncServerProc, serverSock);
                asyncResult.AsyncWaitHandle.WaitOne();

                //if (eventStop.Wa)
                //{

                //}
            }
        }

        private void AsyncServerProc(IAsyncResult ar)
        {
            TcpListener serverSock = (TcpListener)ar.AsyncState;

            TcpClient client = serverSock.EndAcceptTcpClient(ar);
            //Запуск потока клиента в пуле потоков
            //ThreadPool.QueueUserWorkItem(ThreadClient, client);

        }

        /// <summary>
        /// Поток обслуживания клиента
        /// </summary>
        /// <param name="obj"></param>
        private void ThreadClientProcess(object obj)
        {
            TcpClient client = (TcpClient)obj;
            //Работа с клиентом через объектов TcpClient
            //Определить общение с клиентом
        }
    }
}
