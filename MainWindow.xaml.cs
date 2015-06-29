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
        private int port=80;
        private String username="Anonymous";
        private String IpAdress="127.0.0.1";
        private TcpClient otherUser;
        private Thread serverTCP=null;
        private Thread clientTCP=null;
        private Thread serverUDP=null;
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
            if (serverUDP != null && serverUDP.IsAlive)
                serverUDP.Abort();
            serverTCP=new Thread(testConnectionAsClient);
            clientTCP=new Thread(testConnectionAsServer);
            serverUDP = new Thread(UdpServer);
            serverTCP.Start();
            clientTCP.Start();
            serverUDP.Start();
        }
        
        private void UdpServer()
        {
            UdpClient server= new UdpClient(port); ;
            IPEndPoint ip = new IPEndPoint(IPAddress.Parse(IpAdress), port);
            while (true)
            {
                try
                {
                    
                    Byte[] data = server.Receive(ref ip);
                    String dataParse = Encoding.ASCII.GetString(data);
                    Dispatcher.BeginInvoke(new Action(delegate()
                    {
                        MessagesTextBlock.Text += dataParse+"\n";
                    }));
                }
                catch (Exception E)
                {
                    MessageBox.Show(E.Message);
                }
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
            MessagesTextBlock.Text += username + "("+System.DateTime.Now.Hour + ":" + System.DateTime.Now.Minute + ":" + System.DateTime.Now.Second + ")"+" - " + new TextRange(MessageTextBox.Document.ContentStart, MessageTextBox.Document.ContentEnd).Text+"\n";
            try{
                 UdpClient client = new UdpClient(IpAdress, port);

                 Byte[] dane = Encoding.ASCII.GetBytes(username + "("+System.DateTime.Now.Hour + ":" + System.DateTime.Now.Minute + ":" + System.DateTime.Now.Second + ")"+" - " + new TextRange(MessageTextBox.Document.ContentStart, MessageTextBox.Document.ContentEnd).Text);
                 client.Send(dane, dane.Length);
                 client.Close();
                }
            catch(Exception ex){
                 MessageBox.Show(ex.Message);
                } 
        }

        private void MessageTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SendButton_Click(sender, e);
                MessageTextBox.Document.Blocks.Clear();
            }
        }
    }
}
