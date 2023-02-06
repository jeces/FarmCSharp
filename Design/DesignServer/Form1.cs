using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DesignServer
{
    public partial class Form1 : Form
    {
        TcpListener server = null;
        TcpClient clientSocket = null;
        static int counter = 0;
        string date;

        public Dictionary<TcpClient, string> clientList = new Dictionary<TcpClient, string>();

        public Form1()
        {
            InitializeComponent();
            Thread t = new Thread(initSocket);
            t.IsBackground = true;      // 메인 스레드가 종료되었을 때 같이 종료되게 만들기 위함.
            t.Start();
        }

        private void initSocket()
        {
            server = new TcpListener(IPAddress.Any, 8080);
            clientSocket = default(TcpClient);
            server.Start();
            DisplayText(">> Server Started");

            while (true)
            {
                try
                {
                    counter++;  // 접속자 증가를 위해
                    clientSocket = server.AcceptTcpClient();    // 클라이언트 소켓 접속 허용
                    DisplayText(">> Accept connection from client");

                    NetworkStream stream = clientSocket.GetStream();    // NetworkStream을 얻어옴 
                    byte[] buffer = new byte[1024]; // 저장할 버퍼
                    int bytes = stream.Read(buffer, 0, buffer.Length);  // 클라이언트로 온 데이터를 byte로 읽음
                    string user_name = Encoding.Unicode.GetString(buffer, 0, bytes);
                    user_name = user_name.Substring(0, user_name.IndexOf("$"));

                    clientList.Add(clientSocket, user_name);
                    SendMessageAll(user_name + "님이 입장하셨습니다.", "", false);

                    handleClient h_client = new handleClient();
                    h_client.OnReceived += new handleClient.MessageDisplayHandler(OnReceived);
                    h_client.OnDisconnected += new handleClient.DisconnectedHandler(h_client_OnDisconnected);
                    h_client.startClient(clientSocket, clientList);
                }
                catch (SocketException se) { break; }
                catch (Exception ex) { break; }
            }
        }

        private void h_client_OnDisconnected(TcpClient clientSocket)
        {
            if (clientList.ContainsKey(clientSocket))
                clientList.Remove(clientSocket);
        }

        private void OnReceived(string message, string user_name)
        {
            if (message.Equals("leaveChat"))
            {
                string displayMessage = "leave user : " + user_name;
                DisplayText(displayMessage);
                SendMessageAll("leaveChat", user_name, true);
            }
            else
            {
                string displayMessage = "From client : " + user_name + " : " + message;
                DisplayText(displayMessage);
                SendMessageAll(message, user_name, true);
            }
        }

        private void SendMessageAll(string v1, string v2, bool v3)
        {
            foreach (var pair in clientList)
            {
                date = DateTime.Now.ToString("yyyy.MM.dd. HH:mm:ss");

                TcpClient client = pair.Key as TcpClient;
                NetworkStream stream = client.GetStream();
                byte[] buffer = null;

                if (v3)
                {
                    if (v1.Equals("leaveChat"))
                        buffer = Encoding.Unicode.GetBytes(v2 + "님 나갔습니다.");
                    else
                        buffer = Encoding.Unicode.GetBytes("[ " + date + " ]" + v2 + " : " + v1);
                }
                else
                {
                    buffer = Encoding.Unicode.GetBytes(v1);
                }

                stream.Write(buffer, 0, buffer.Length);
                stream.Flush();
            }
        }

        private void DisplayText(string v)
        {
            if (textBox1.InvokeRequired)
            {
                textBox1.BeginInvoke(new MethodInvoker(delegate
                {
                    textBox1.AppendText(v + Environment.NewLine);
                }));
            }
            else
                textBox1.AppendText(v + Environment.NewLine);

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
