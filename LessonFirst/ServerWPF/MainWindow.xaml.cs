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
        private bool _buttonIsStart;
        private TcpListener _serverSock;
        private Thread _serverThread;
        private ManualResetEvent _eventStop;
        private List<ClientInfo> listClients;


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

            _buttonIsStart = false;
            _serverSock = null;
            _serverThread = null;
            _eventStop = new ManualResetEvent(false);
        }


        /// <summary>
        /// Кнопка включения выключения чего-то
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartButtonClick(object sender, RoutedEventArgs e)
        {
            if (!_buttonIsStart) //запуск сервера
            {
                try
                {
                    _serverSock = new TcpListener(IPAddress.Parse(IPAddresComboBox.Text), int.Parse(PortTextBox.Text));

                    _serverSock.Start();
                    _serverThread = new Thread(ServerThreadProcess);
                    _serverThread.Start(_serverSock);
                    _buttonIsStart = !_buttonIsStart;
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
                IAsyncResult asyncResult = _serverSock.BeginAcceptSocket(AsyncServerProc, _serverSock);
                asyncResult.AsyncWaitHandle.WaitOne();

                while (asyncResult.AsyncWaitHandle.WaitOne(200) == false)
                {
                    if (_eventStop.WaitOne(0) == true)
                    {
                        return;
                    }
                }
            }
        }

        private void AsyncServerProc(IAsyncResult ar)
        {
            TcpListener serverSock = (TcpListener)ar.AsyncState;

            TcpClient client = serverSock.EndAcceptTcpClient(ar);

            WriteToLog("Подключился клиент\r\n");
            WriteToLog($"IP адрес клиента: {client.Client.RemoteEndPoint.ToString()}\r\n");
            //Запуск потока клиента в пуле потоков
            ThreadPool.QueueUserWorkItem(ThreadClientProcess, client);

        }


        private void WriteToLog(string str)
        {
            logTextBox.Text += str;


            //
            Dispatcher.Invoke(() =>
            {
                logTextBox.AppendText(str);
            });
        }

        /// <summary>
        /// Поток обслуживания клиента
        /// </summary>
        /// <param name="obj"></param>
        private void ThreadClientProcess(object obj)
        {
            TcpClient client = (TcpClient)obj;
            //Работа с клиентом через объектов TcpClient
            WriteToLog("Рабочий поток клиентов запущен\r\n");

            byte[] buf = new byte[4 * 1024];//4Kb
            string clientName;//Имя клиента

            //Определить общение с клиентом
            //Ждем имя клиента, пользователя мессенджера
            int recSize = client.Client.Receive(buf);
            clientName = Encoding.UTF8.GetString(buf, 0, recSize);

            WriteToLog($"Клиент: {clientName}\r\n");
            //Ответ сервера клиенту - Welcome to ******

            client.Client.
                Send(Encoding.ASCII.GetBytes($"Welcome {clientName}"));
            //добавление клиента в список
            ClientInfo v = new ClientInfo
            {
                Client = client,
                Name = clientName
            };

            while (true)
            {
                recSize = client.Client.Receive(buf);
                string message = Encoding.UTF8.GetString(buf);
                //отправить сообщение всем подключенным клиентам
                client.Client.Send(Encoding.ASCII.GetBytes(message));

            }
        }

        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
