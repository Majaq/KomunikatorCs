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
using System.Threading;
using System.Net.Sockets;
using System.Net;

namespace messengerCs
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int port=1000;
        private String username="Anonymous";
        private String IpAdress="127.0.0.1";
        private TcpClient otherUser;
        private Thread serverTCP=null;
        private Thread clientTCP=null;
        private Thread serverThread=null;
        private Thread clientThread=null;
        public MainWindow()
        {
            InitializeComponent();
        }
        private void testConnectionAsServer()
        {
            TcpClient client=null;
            TcpListener listener=null;
            IPAddress adress = IPAddress.Parse(IpAdress);
            while (true)
            {
                try
                {
                    listener = new TcpListener(adress, port);
                    listener.Start();
                    client = listener.AcceptTcpClient();
                    client.Close();
                    listener.Stop();
                }
                catch (Exception E)
                {

                }
            }
        }
        private void testConnectionAsClient()
        {
            while (true)
            {
                try
                {
                    otherUser = new TcpClient(IpAdress, port);
                    Dispatcher.BeginInvoke(new Action(delegate() 
                      {
                          ConnectedRadioButton.IsChecked = true;
                          MessageTextBox.IsEnabled = true;
                          SendButton.IsEnabled = true;
                          ConnectButton.IsEnabled = false;
                      }));
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                    Dispatcher.BeginInvoke(new Action(delegate()
                    {
                        ConnectedRadioButton.IsChecked = false;
                        MessageTextBox.IsEnabled = false;
                        SendButton.IsEnabled = false;
                        ConnectButton.IsEnabled = true;
                    }));
                }
                System.Threading.Thread.Sleep(1000);
            }
        }
        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            IpAdress=IpTextBox.Text;
            if (serverTCP != null && serverTCP.IsAlive)
                serverTCP.Abort();
            if (clientTCP != null && clientTCP.IsAlive)
                clientTCP.Abort();
            
            serverTCP=new Thread(testConnectionAsClient);
            clientTCP=new Thread(testConnectionAsServer);
            serverTCP.Start();
            clientTCP.Start();
        }
        
        private void UdpServer()
        {
            UdpClient server;
            IPEndPoint ip = new IPEndPoint(IPAddress.Parse(IpAdress), port);
            try
            {
                server = new UdpClient(port);
                Byte[] data = server.Receive(ref ip);
                String dataParse = Encoding.ASCII.GetString(data);
                Dispatcher.BeginInvoke(new Action(delegate()
                {
                    MessagesTextBlock.Text += "\n\n" + dataParse;
                }));
            }
            catch (Exception E)
            {
                MessageBox.Show(E.Message);
            }
        }
        private void Window_Closing_1(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(Environment.ExitCode);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            username = LoginTextBox.Text;
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
