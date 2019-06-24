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

namespace ClientWpf
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TcpClient _client;

        public MainWindow()
        {
            InitializeComponent();

            _client = new TcpClient();
        }

        private void SendButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                _client.Client.Send(Encoding.UTF8.GetBytes(userTextBox.Text));
            }
            catch (Exception exception)
            {
                MessageBox.Show($"Error: {exception.Message}");
            }
        }

        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ConnectButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                _client.Connect(IPAddress.Parse(ipTextBox.Text), int.Parse(portTextBox.Text));
                ThreadPool.QueueUserWorkItem(ServerClientThread, _client);
                MessageBox.Show("Связь с сервером установлена");
            }
            catch (Exception exception)
            {
                MessageBox.Show($"Error: {exception.Message}");
            }
        }

        private void ServerClientThread(object obj)
        {
            TcpClient client = (TcpClient)obj;
            WriteToLog("Рабочий поток клиент-сервер запущен\r\n");

            client.Client.Send(Encoding.UTF8.GetBytes("adads"));
            byte[] buf = new byte[4 * 1024];//4Kb
            int recSize = client.Client.Receive(buf);
            WriteToLog(Encoding.UTF8.GetString(buf, 0, recSize));

            while (true)
            {
                recSize = client.Client.Receive(buf);
                WriteToLog(Encoding.UTF8.GetString(buf,0,recSize));
                //client.Client.Send(Encoding.ASCII.GetBytes(message));

            }
        }

        private void SaveToBlock(string str)
        {
            Dispatcher.Invoke(() =>
            {
                serverTextBox.AppendText(str);
            });
        }

        private void WriteToLog(string str)
        {
            Dispatcher.Invoke(() =>
            {
                logTextBox.AppendText(str);
            });
        }
    }
}

/*https://test.nizarium.com/httpserver/source.html*/
