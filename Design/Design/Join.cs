using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace Design
{
    public partial class Join : Form
    {
        public IPEndPoint ipep = null;
        public Socket client = null;
        public IPEndPoint senders = null;
        byte[] data = new byte[1024];
        byte[] datas = new byte[1024];
        public int flag
        {
            get;
            private set;
        }
        public Join()
        {
            InitializeComponent();
            flag = 1;
        }
        // Join
        private void button2_Click(object sender, EventArgs e)  //가입
        {
            // NAME, PHONE, E-MAIL
            if (!(textBox1.Text == ""))
            {
                if (!(textBox2.Text == ""))
                {
                    if (!(textBox3.Text == ""))
                    {
                    }
                    else
                    {
                        textBox3.Focus();
                        MessageBox.Show("Input E-mail");
                        return;
                    }
                }
                else
                {
                    textBox2.Focus();
                    MessageBox.Show("Input Phone");
                    return;
                }
            }
            else
            {
                textBox1.Focus();
                MessageBox.Show("Input Name");
                return;
            }
            // ID, PW, DUPLICATE
            if (!(idbox.Text == ""))
            {
                if (!((psbox.Text == "") || (repsbox.Text == "")))
                {
                    if (flag == 0)
                    {
                        if(psbox.Text != repsbox.Text)
                        {
                            MessageBox.Show("Check Password");
                            return;
                        }
                        
                        // 서버로 보냄
                        data = Encoding.UTF8.GetBytes($"JoinCheck#{idbox.Text}#{psbox.Text}#{textBox1.Text}#{textBox2.Text}#{textBox3.Text}");
                        client.SendTo(data, data.Length, SocketFlags.None, ipep);
                        // 서버에서 받음

                        Array.Clear(datas, 0x0, datas.Length);
                        EndPoint remote = (EndPoint)(senders);
                        client.ReceiveFrom(datas, ref remote);
                        Console.WriteLine($"[{remote} Receive] : {Encoding.UTF8.GetString(datas)}");


                        if (Encoding.UTF8.GetString(datas).TrimEnd('\0') == "successJoin")
                        {
                            MessageBox.Show("Success Join");
                            this.Close();
                            return;
                        }
                        /*
                        //아이디 생성 
                        using (MySqlConnection connection = new MySqlConnection("Server = 192.168.1.225; Port = 3306; Database = dbtest; Uid = root1; Pwd = crew1207"))
                        {
                            try
                            {   // ROOT도 idx 넣기
                                connection.Open();
                                string sql = $"INSERT INTO MEMBER_INFO VALUES ('{idbox.Text}', '{psbox.Text}', '{textBox1.Text}', '{textBox2.Text}', '{textBox3.Text}', NOW(), 0, 'X')";

                                MySqlCommand cmd = new MySqlCommand(sql, connection);
                                MySqlDataReader table = cmd.ExecuteReader();
                                MessageBox.Show("가입되었습니다.");
                                this.Close();
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("실패");
                                Console.WriteLine(ex.ToString());
                            }
                        }
                        */
                    }
                    else
                    {
                        MessageBox.Show("Check Duplicate");
                    }
                }
                else
                {
                    MessageBox.Show("Input Password");
                }
            }
            else
            {
                MessageBox.Show("Input ID");
            }
            
        }
        // Id Duplicate
        private void button1_Click_1(object sender, EventArgs e)
        {
            if (idbox.Text == "")
            {
                MessageBox.Show("아이디를 입력해주세요.");
                return;
            }
            // 서버로 보냄
            data = Encoding.UTF8.GetBytes($"duplicat#{idbox.Text}");
            client.SendTo(data, data.Length, SocketFlags.None, ipep);

            // 서버에서 받음
            EndPoint remote = (EndPoint)(senders);
            client.ReceiveFrom(datas, ref remote);
            Console.WriteLine($"[{remote} Receive] : {Encoding.UTF8.GetString(datas)}");
            if(Encoding.UTF8.GetString(datas).TrimEnd('\0') == "notDuplicate")
            {
                flag = 0;
                MessageBox.Show("Enabled ID");
                idbox.ReadOnly = true;
            } 
            else
            {
                idbox.Text = "";
                flag = 1;
                MessageBox.Show($"[{idbox.Text}] is Duplicate ID");
            }

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
                        if(idbox.Text == "")
                        {
                            MessageBox.Show("아이디를 입력해주세요.");
                            return;
                        }
                        if (idbox.Text == table["MEMBER_ID"].ToString())
                        {
                            MessageBox.Show($"{idbox.Text}는 중복된 아이디입니다.");
                            idbox.Text = "";
                            psbox.Text = "";
                            flag = 1;
                            return;
                        }
                    }
                    MessageBox.Show("사용가능한 아이디입니다.");
                    flag = 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("실패");
                    Console.WriteLine(ex.ToString());
                }
            }
            */
        }
    }
}