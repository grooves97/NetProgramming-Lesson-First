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
using System.Windows.Shapes;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ServerWPF
{
    /// <summary>
    /// Логика взаимодействия для ClientWindow.xaml
    /// </summary>
    public partial class ClientWindow : Window
    {
        private TcpClient _client;

        public ClientWindow()
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
                MessageBox.Show("Связь с сервером установлена");
                ThreadPool.QueueUserWorkItem(ServerClientThread, _client);
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
            Encoding.UTF8.GetString(buf,0, recSize);

            while (true)
            {
                recSize = client.Client.Receive(buf);
                string message = Encoding.UTF8.GetString(buf);
                serverTextBox.Text = message;
                //client.Client.Send(Encoding.ASCII.GetBytes(message));

            }
        }

        private void SaveToBlock(string str)
        {
            serverTextBox.Text = str;

            Dispatcher.Invoke(() =>
            {
                serverTextBox.AppendText(str);
            });
        }

        private void WriteToLog(string str)
        {
            logTextBox.Text += str;
            
            Dispatcher.Invoke(() =>
            {
                logTextBox.AppendText(str);
            });
        }
    }
}

/*https://test.nizarium.com/httpserver/source.html*/
