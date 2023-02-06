using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using MetroFramework.Forms;

namespace Design
{
    public partial class Chat : MetroForm
    {
        Data _data;
        delegate void Delegate_chat();
        Thread t_handler = null;
        public IPEndPoint ipep = null;
        public Socket client = null;
        public IPEndPoint senders = null;
        byte[] data = new byte[1024];
        byte[] datas = new byte[1024];
        private string your_id_value;
        string[] str = null;
        private bool isStop = false;
        public string your_id
        {
            get { return this.your_id_value; }
            set { this.your_id_value = value; }
        }
        public Chat()
        {
            InitializeComponent();
        }
        public Chat(Data fdata)
        {
            InitializeComponent();
            _data = fdata;
        }
        private void Chat_Load(object sender, EventArgs e)
        {
            this.Text = $"[{your_id}]님과의 채팅";
            t_handler = new Thread(chatMessage);
            t_handler.IsBackground = true;
            t_handler.Start();
            
        }
        private void chatMessage()
        {
            while (!isStop)
            {
                Array.Clear(datas, 0x0, datas.Length);
                Console.WriteLine("채팅기다림");
                EndPoint remote = (EndPoint)(senders);
                client.ReceiveFrom(datas, ref remote);
                Console.WriteLine($"채팅 [{remote} Receive] : {Encoding.UTF8.GetString(datas).TrimEnd('\0')}");
                str = Encoding.UTF8.GetString(datas).TrimEnd('\0').Split('#');
                if (str[0] == "receiveMessage")
                {
                    this.Invoke(new Delegate_chat(chatFun));
                } else
                {

                }
            }
        }

        private void chatFun()
        {
            if (richTextBox1.Text == "")
            {
                richTextBox1.SelectionAlignment = HorizontalAlignment.Left;
                richTextBox1.Text = $"[{your_id.TrimEnd('\0')}]님 : {str[1]}";
            }
            else
            {
                richTextBox1.AppendText("\r\n");
                richTextBox1.SelectionAlignment = HorizontalAlignment.Left;
                richTextBox1.AppendText($"[{your_id.TrimEnd('\0')}]님 : {str[1]}");
            }
            Array.Clear(datas, 0, datas.Length);
            richTextBox1.Select(richTextBox1.Text.Length - 1, 0);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            data = Encoding.UTF8.GetBytes($"ChatMessage#{your_id.TrimEnd('\0')}#{textBox2.Text.TrimEnd('\0')}");
            if(richTextBox1.Text == "")
            {
                richTextBox1.SelectionAlignment = HorizontalAlignment.Right;
                richTextBox1.Text = $"[나] : {textBox2.Text}";
                
            }else
            {
                richTextBox1.AppendText("\r\n");
                richTextBox1.SelectionAlignment = HorizontalAlignment.Right;
                richTextBox1.AppendText($"{textBox2.Text} [나");
            }
            client.SendTo(data, data.Length, SocketFlags.None, ipep);
            textBox2.Text = "";
            richTextBox1.Select(richTextBox1.Text.Length - 1, 0);
        }

        private void Chat_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void Chat_FormClosing(object sender, FormClosingEventArgs e)
        {
            isStop = true;
            t_handler.Interrupt();
            Console.WriteLine("chat interrupt");
            t_handler.Abort();
            Console.WriteLine("chat abort");

            // 남아있는 receive 처리를 위한 코드
            data = Encoding.UTF8.GetBytes($"emptyReceive#{your_id}");
            client.SendTo(data, data.Length, SocketFlags.None, ipep);

            Console.WriteLine("Waiting until Chat thread stops...");
            _data.Refresh();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            isStop = false;
            t_handler = new Thread(chatMessage);
            t_handler.IsBackground = true;
            t_handler.Start();
            Console.WriteLine("start");
        }
        private void button3_Click(object sender, EventArgs e)
        {
            isStop = true;
            t_handler.Interrupt();
            Console.WriteLine("chat interrupt");
            t_handler.Abort();
        }
    }
}