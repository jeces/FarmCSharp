using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using MetroFramework.Forms;

namespace Design
{
    public partial class Login : MetroForm
    {
        // 기본 소켓 버전
        public IPEndPoint ipep = new IPEndPoint(IPAddress.Parse("192.168.1.225"), 4000);
        public Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        
        
        //public UdpClient client = new UdpClient("192.168.1.225", 4000); - UdpClient 버전

        byte[] data = new byte[1024];
        byte[] datas = new byte[5000000];
        public IPEndPoint senders = new IPEndPoint(IPAddress.Parse("192.168.1.225"), 0);
        
        public Login()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            serverCon();

            
        }

        private void serverCon()
        {
            data = Encoding.UTF8.GetBytes("LoginImage#");
            
            try
            {
                Console.WriteLine("Connecting Server");
                //data = Encoding.UTF8.GetBytes("I`m Client. ask Connect");
                client.SendTo(data, data.Length, SocketFlags.None, ipep);
                MemoryStream ms = null;
                Image img = null;

                // 사진 보냄
                EndPoint remote = (EndPoint)(senders);
                client.ReceiveFrom(datas, ref remote);
                //Console.WriteLine($"로그인 사진 : [{remote} Receive] : {Encoding.UTF8.GetString(datas).TrimEnd('\0')}");
                ms = new MemoryStream(datas);
                img = Image.FromStream(ms);

                pictureBox1.Image = (Image)img.Clone();
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            }
            catch
            {
                Console.WriteLine("Disconnect Server");
            }
        }
        // 로그인
        private void button1_Click(object sender, EventArgs e)
        {
            if(idbox.Text == "")
            {
                MessageBox.Show("Input Id");
                idbox.Focus();
                return;
            }
            if(psbox.Text == "")
            {
                MessageBox.Show("Input Password");
                psbox.Focus();
                return;
            }
            LoginDB();
            /*
            using (MySqlConnection connection = new MySqlConnection("Server = 192.168.1.225; Port = 3306; Database = dbtest; Uid = root1; Pwd = crew1207"))
            {
                try
                {
                    connection.Open();
                    string sql = "SELECT * FROM MEMBER_INFO";
                    MySqlCommand cmd = new MySqlCommand(sql, connection);
                    MySqlDataReader table = cmd.ExecuteReader();

                    while (table.Read())
                    {
                        if ((idbox.Text == table["MEMBER_ID"].ToString()) && (psbox.Text == table["MEMBER_PW"].ToString())) {
                            if(table["MEMBER_APPROVAL"].ToString() == 0.ToString())
                            {
                                MessageBox.Show("미승인된 아이디 입니다.");
                                psbox.Text = "";
                                return;
                            }
                            if(table["MEMBER_LOGIN"].ToString() == "O")
                            {
                                MessageBox.Show("이미 접속된 아이디 입니다.");
                                idbox.Text = "";
                                psbox.Text = "";
                                return;
                            }
                            this.Hide();
                            data = Encoding.UTF8.GetBytes($"{idbox.Text}");
                            client.SendTo(data, data.Length, SocketFlags.None, ipep);
                            MessageBox.Show("로그인 성공");

                            EndPoint remote = (EndPoint)(senders);
                            client.ReceiveFrom(datas, ref remote);
                            Console.WriteLine($"[{remote} Receive] : {Encoding.UTF8.GetString(datas)}");

                            //IP옮기기
                            Data frm1 = new Data();
                            Chat cha = new Chat();
                            frm1.ipep = ipep;
                            frm1.client = client;
                            frm1.senders = senders;
                            cha.ipep = ipep;
                            cha.client = client;
                            cha.senders = senders;
                            //LoginUpdate();
                            frm1.board_id = idbox.Text.ToString();
                            frm1.ShowDialog();
                            //this.Close();
                        }
                    }
                    MessageBox.Show("존재하지 않는 아이디이거나 비밀번호가 틀렸습니다.");
                    idbox.Text = "";
                    psbox.Text = "";
                    table.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("실패");
                    Console.WriteLine(ex.ToString());
                }
            }
            */
        }
        private void LoginDB()
        {
            // 서버로 보냄
            data = Encoding.UTF8.GetBytes($"LoginCheck#{idbox.Text}#{psbox.Text}");
            client.SendTo(data, data.Length, SocketFlags.None, ipep);

            // 서버에서 받음
            Array.Clear(datas, 0x0, datas.Length);
            EndPoint remote = (EndPoint)(senders);
            client.ReceiveFrom(datas, ref remote);
            Console.WriteLine($"로그인 [{remote} Receive] : {Encoding.UTF8.GetString(datas).TrimEnd('\0')}");
            if (Encoding.UTF8.GetString(datas).TrimEnd('\0') == "NoId")
            {
                MessageBox.Show("Non-existent ID");
                idbox.Text = "";
                psbox.Text = "";
                idbox.Focus();
            }
            else if (Encoding.UTF8.GetString(datas).TrimEnd('\0') == "notPassword")
            {
                MessageBox.Show("Different Password");
                psbox.Text = "";
                psbox.Focus();
            }
            else if(Encoding.UTF8.GetString(datas).TrimEnd('\0') == "notApproval")
            {
                MessageBox.Show("NotApproval ID. Get approved");
                psbox.Text = "";
                psbox.Focus();
            }
            else if(Encoding.UTF8.GetString(datas).TrimEnd('\0') == "loginDupli")
            {
                MessageBox.Show("Id is already login");
                psbox.Text = "";
                psbox.Focus();
            }
            else
            {

                // 채팅중인 사람에게 비워주기 위해 보내자.
                Array.Clear(data, 0x0, data.Length);
                data = Encoding.UTF8.GetBytes($"emptyReceive#{idbox.Text}");
                client.SendTo(data, data.Length, SocketFlags.None, ipep);

                //IP옮기기
                

                MessageBox.Show("Login Success");
                
                Console.WriteLine($"로그인 아이디 : {idbox.Text}");
                this.Hide();
                Data frm1 = new Data();
                frm1.ipep = ipep;
                frm1.client = client;
                frm1.senders = senders;
                Console.WriteLine($"로그인 아이디 : {idbox.Text}");
                frm1.board_id = idbox.Text;
                frm1.ShowDialog();

                

            }
        }
        private void button2_Click(object sender, EventArgs e)  //가입
        {
            Join frm = new Join();
            frm.ipep = ipep;
            frm.client = client;
            frm.senders = senders;
            frm.ShowDialog();
        }
        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            MessageBox.Show("Pragram Exit");
            client.Close();
        }

        private void Panel_Size(object sender, EventArgs e)
        {

        } 
    }
}