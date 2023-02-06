using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using MetroFramework.Forms;
using System.IO;

namespace Design
{
    public partial class Data : MetroForm
    {
        public Thread t_handler = null;
        public Thread t2 = null;
        public Thread t3 = null;
        public Thread test = null;
        public IPEndPoint ipep = null;
        public Socket client = null;
        public IPEndPoint senders = null;
        private string board_id_value;
        private string your_id_value;
        byte[] data = new byte[1024];
        byte[] datas = new byte[500000];
        private int count = 0;
        private int counttest = 0;
        private int count2 = 0;
        public string your_id
        {
            get { return this.your_id_value; }
            set { this.your_id_value = value; }
        }
        public string board_id
        {
            get { return this.board_id_value; }
            set { this.board_id_value = value; }
        }
        public int x1
        {
            get;
            private set;
        }
        public int y1
        {
            get;
            private set;
        }
        public int x2
        {
            get;
            private set;
        }
        public int y2
        {
            get;
            private set;
        }
        public int x3
        {
            get;
            private set;
        }
        public int y3
        {
            get;
            private set;
        }
        public string groundState1
        {
            get;
            private set;
        }
        public string groundState2
        {
            get;
            private set;
        }
        public string groundState3
        {
            get;
            private set;
        }
        public string searchTemp
        {
            get;
            private set;
        }
        public string searchHumi
        {
            get;
            private set;
        }
        public string searchLand
        {
            get;
            private set;
        }
        public string searchCo2
        {
            get;
            private set;
        }
        public string searchNutri
        {
            get;
            private set;
        }
        public string searchBoard
        {
            get;
            private set;
        }
        public Boolean logOut_button
        {
            get;
            private set;
        }
        Random random = new Random();
        public Data()
        {
            InitializeComponent();
            dataGridView7.AllowUserToAddRows = false;
            dataGridView6.AllowUserToAddRows = false;
            dataGridView5.AllowUserToAddRows = false;
            dataGridView4.AllowUserToAddRows = false;
            dataGridView3.AllowUserToAddRows = false;
            dataGridView2.AllowUserToAddRows = false;
            dataGridView1.AllowUserToAddRows = false;
            dbUpdateTemp();
            dbUpdateHumi();
            dbUpdateLand();
            dbUpdateCo2();
            dbUpdateNutri();
            dbUpdateBoard();
            searchTemp = "";
            searchHumi = "";
            searchLand = "";
            searchCo2 = "";
            searchNutri = "";
            searchBoard = "";
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            
            /*
            MemoryStream ms = null;
            Image img = null;

            EndPoint remote = (EndPoint)(senders);
            Console.WriteLine("이까지옴?");
            client.ReceiveFrom(datas, ref remote);
            Console.WriteLine($"이미지 : [{remote} Receive] : {Encoding.UTF8.GetString(datas).TrimEnd('\0')}");
            ms = new MemoryStream(datas);
            img = Image.FromStream(ms);

            pictureBox1.Image = (Image)img.Clone();
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            */

            // 한번 요청
            label2.Text = board_id;
            if (board_id == "racoon")
            {
                button3.Visible = true;
            }


            t_handler = new Thread(isChat);
            t_handler.IsBackground = true;
            t_handler.Start();

            

            /*test = new Thread(testThread);
            test.IsBackground = true;
            test.Start();*/
        }

        private void sendImage()
        {
            data = Encoding.UTF8.GetBytes($"DataImage#{board_id}");
            client.SendTo(data, data.Length, SocketFlags.None, ipep);
            //Console.WriteLine($"보냈음{board_id}");

            MemoryStream ms = null;
            Image img = null;

            Array.Clear(datas, 0x0, datas.Length);
            EndPoint remote = (EndPoint)(senders);
            //Console.WriteLine("받을 준비");
            client.ReceiveFrom(datas, ref remote);
            //Console.WriteLine($"[{remote} Receive] : {Encoding.UTF8.GetString(datas).TrimEnd('\0')}");
            ms = new MemoryStream(datas);
            img = Image.FromStream(ms);

            pictureBox1.Image = (Image)img.Clone();
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        // 채팅
        private void isChat()
        {
            

            string json = null;
            List<MyObject> member = null;
            while (true)
            {
                if(count2 == 0)
                {
                    Array.Clear(data, 0x0, data.Length);
                    data = Encoding.UTF8.GetBytes($"currentUser#{board_id}");
                    client.SendTo(data, data.Length, SocketFlags.None, ipep);
                    count2 = 1;
                }
                
                Array.Clear(datas, 0x0, datas.Length);
                Console.WriteLine($"데이터 : {Encoding.UTF8.GetString(datas).TrimEnd('\0')}기다리는중");
                EndPoint remote = (EndPoint)(senders);
                client.ReceiveFrom(datas, ref remote);
                Console.WriteLine($"데이터 [{remote} Receive] : {Encoding.UTF8.GetString(datas).TrimEnd('\0')}");
                string[] s = Encoding.UTF8.GetString(datas).TrimEnd('\0').Split('#');
                Console.WriteLine($"{s[0]}, {s[1]}");
                
                if (s[0] == "acceptM")
                {
                    t2 = new Thread(new ParameterizedThreadStart(accept));
                    t2.IsBackground = true;
                    t2.Start(s[1]);
                    t2.Join();
                    // 채팅중 들어온 유저 다시 뿌려주기
                    data = Encoding.UTF8.GetBytes("currentUser#");
                    client.SendTo(data, data.Length, SocketFlags.None, ipep);
                    this.Refresh();
                }
                else if(s[0] == "acc")
                {
                    Console.WriteLine("acc");
                    if (MessageBox.Show($"[{s[1]}]의 채팅을 수락하시겠습니까?", "Chatting", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        string a = Encoding.UTF8.GetString(datas).TrimEnd('\0');
                        data = Encoding.UTF8.GetBytes($"accept#{s[1]}#{board_id}");
                        Console.WriteLine(a.Length);
                        client.SendTo(data, data.Length, SocketFlags.None, ipep);
                        MessageBox.Show("채팅방에 참여합니다.");

                        t3 = new Thread(new ParameterizedThreadStart(accept2));
                        t3.IsBackground = true;
                        t3.Start(s[1]);
                        t3.Join();
                        // 채팅중 들어온 유저 다시 뿌려주기
                        data = Encoding.UTF8.GetBytes("currentUser#");
                        client.SendTo(data, data.Length, SocketFlags.None, ipep);
                        this.Refresh();
                    }
                    else
                    {
                        MessageBox.Show("채팅방에 거절하였습니다.");
                    }
                    //MessageBox.Show($"{Encoding.UTF8.GetString(datas)}");
                    Console.WriteLine("Come in");
                }
                else if(s[0] == "currUse" && count == 0)
                {
                    Console.WriteLine("여긴 처음 유저상황");
                    //Array.Clear(datas, 0x0, datas.Length);
                    Console.WriteLine(s[1]);
                    //client.ReceiveFrom(datas, ref remote);
                    //json = $"{Encoding.UTF8.GetString(datas).TrimEnd('\0')}";
                    json = s[1];
                    if (json.TrimStart('[').TrimEnd(']') == "")
                    {
                        Console.WriteLine("비어있음");
                        count++;
                        return;
                    }
                    member = JsonConvert.DeserializeObject<List<MyObject>>(json);
                    dataGridView7.Rows.Clear();
                    for (int i = 0; i < member.Count; ++i)
                    {
                        if(member[i].MEMBER_ID == board_id)
                        {
                            
                        } else
                        {
                            dataGridView7.Rows.Add(member[i].MEMBER_ID, member[i].MEMBER_NAME, member[i].MEMBER_LOGIN);
                        }
                    }
                    count++;
                }
                else if(s[0] == "currUse" && count != 0)
                {
                    Console.WriteLine("여긴 두번째부터 유저상황");
                    //Array.Clear(datas, 0x0, datas.Length);
                    Console.WriteLine(s[1]);
                    //client.ReceiveFrom(datas, ref remote);
                    //json = $"{Encoding.UTF8.GetString(datas).TrimEnd('\0')}";
                    json = s[1];
                    if (json.TrimStart('[').TrimEnd(']') == "")
                    {
                        Console.WriteLine("비어있음");
                        count++;
                        return;
                    }
                    member = JsonConvert.DeserializeObject<List<MyObject>>(json);
                    Console.WriteLine($" ID : {member[0].MEMBER_ID}");
                    dataGridView7.Rows.Clear();
                    for (int i = 0; i < member.Count; ++i)
                    {
                        if (member[i].MEMBER_ID == board_id)
                        {

                        }
                        else
                        {
                            dataGridView7.Rows.Add(member[i].MEMBER_ID, member[i].MEMBER_NAME, member[i].MEMBER_LOGIN);
                        }
                    }
                    count++;
                }
                if(count2 == 1)
                {
                    sendImage();
                    count2 = 2;
                }
                
                Console.WriteLine("while 빠져나감");
            }
            
        }

        private void testThread()
        {
            while(true)
            {
                //Array.Clear(datas, 0x0, datas.Length);
                Console.WriteLine($"데이터2 : {Encoding.UTF8.GetString(datas).TrimEnd('\0')}기다리는중");
                Console.WriteLine($"count2 : {++counttest}");
                EndPoint remote = (EndPoint)(senders);
                client.ReceiveFrom(datas, ref remote);
                Console.WriteLine($"데이터2 [{remote} Receive] : {Encoding.UTF8.GetString(datas).TrimEnd('\0')}");
                string[] s = Encoding.UTF8.GetString(datas).TrimEnd('\0').Split('#');
                Console.WriteLine($"2 {s[0]}, {s[1]}");
            }
            
        }
        private void accept(Object s)
        {
            Console.WriteLine("AcceptM");
            MessageBox.Show("수락되었습니다.");
            //t_handler.Interrupt();
            //this.Hide();
            Chat cha = new Chat(this);
            cha.ipep = ipep;
            cha.client = client;
            cha.senders = senders;
            cha.your_id = (string)s;
            //MessageBox.Show($"난 요청한쪽 {s[1]}");  // 요청을 한쪽을 못가져옴
            cha.ShowDialog();
            //t_handler.Abort();
            //t_handler.Join();
            /*            t2.Interrupt(); //이게 실행이 안됨
                        t2.Abort();*/
            Console.WriteLine("스레드 종료?");
        }
        private void accept2(Object s)
        {
            //this.Hide();
            //t_handler.Interrupt();
            Chat cha = new Chat(this);
            cha.ipep = ipep;
            cha.client = client;
            cha.senders = senders;
            cha.your_id = (string)s;
            cha.ShowDialog();

            
            /*            t3.Interrupt(); //이게 실행이 안됨
                        t3.Abort();*/
            //t_handler.Abort();
            Console.WriteLine("채팅창 종료시 스레드 종료");
        }
        //온도
        private void dbUpdateTemp()
        {
            using (MySqlConnection connection = new MySqlConnection("Server = 192.168.1.225; Port = 3306; Database = dbtest; Uid = root1; Pwd = crew1207"))
            {
                try
                {
                    connection.Open();
                    string sql = "SELECT * FROM TEMP_INFO";
                    MySqlCommand cmd = new MySqlCommand(sql, connection);
                    MySqlDataReader table = cmd.ExecuteReader();

                    while (table.Read())
                    {
                        dataGridView1.Rows.Add(table["TEMP_LINE"], table["TEMP_TEMP"], table["TEMP_DATE"], table["TEMP_CONTROL"]);
                        if (Convert.ToDouble(table["TEMP_TEMP"]) < 40.0)
                        {
                            //제어기 온
                            //표옆에 빨간색으로 표시
                        }
                    }
                    table.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("실패");
                    Console.WriteLine(ex.ToString());
                }
            }
        }
        //습도
        private void dbUpdateHumi()
        {
            using (MySqlConnection connection = new MySqlConnection("Server = 192.168.1.225; Port = 3306; Database = dbtest; Uid = root1; Pwd = crew1207"))
            {
                try
                {
                    connection.Open();
                    string sql = "SELECT * FROM HUMI_INFO";
                    MySqlCommand cmd = new MySqlCommand(sql, connection);
                    MySqlDataReader table = cmd.ExecuteReader();

                    while (table.Read())
                    {
                        dataGridView3.Rows.Add(table["HUMI_LINE"], table["HUMI_HUMI"], table["HUMI_DATE"], table["HUMI_CONTROL"]);
                        if (Convert.ToDouble(table["HUMI_HUMI"]) > 70.0)
                        {
                            //제어기 온
                            //표옆에 빨간색으로 표시
                        }
                    }
                    table.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("실패");
                    Console.WriteLine(ex.ToString());
                }
            }
        }
        //데이터 엑셀화
        //토양
        private void dbUpdateLand()
        {
            using (MySqlConnection connection = new MySqlConnection("Server = 192.168.1.225; Port = 3306; Database = dbtest; Uid = root1; Pwd = crew1207"))
            {
                try
                {
                    connection.Open();
                    string sql = "SELECT * FROM LAND_INFO";
                    MySqlCommand cmd = new MySqlCommand(sql, connection);
                    MySqlDataReader table = cmd.ExecuteReader();

                    while (table.Read())
                    {
                        dataGridView2.Rows.Add(table["LAND_LINE"], table["LAND_INFO"], table["LAND_DATE"], table["LAND_CONTROL"]);
                        if (table["LAND_INFO"].ToString() == "오염")
                        {
                            //제어기 온
                            //표옆에 빨간색으로 표시
                        }
                    }
                    table.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("실패");
                    Console.WriteLine(ex.ToString());
                }
            }
        }
        //Co2
        private void dbUpdateCo2()
        {
            using (MySqlConnection connection = new MySqlConnection("Server = 192.168.1.225; Port = 3306; Database = dbtest; Uid = root1; Pwd = crew1207"))
            {
                try
                {
                    connection.Open();
                    string sql = "SELECT * FROM CO2_INFO";
                    MySqlCommand cmd = new MySqlCommand(sql, connection);
                    MySqlDataReader table = cmd.ExecuteReader();

                    while (table.Read())
                    {
                        dataGridView4.Rows.Add(table["CO2_LINE"], table["CO2_INFO"], table["CO2_DATE"], table["CO2_CONTROL"]);
                        if (Convert.ToDouble(table["CO2_INFO"]) > 20.0)
                        {
                            //제어기 온
                            //표옆에 빨간색으로 표시
                        }
                    }
                    table.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("실패");
                    Console.WriteLine(ex.ToString());
                }
            }
        }
        //양액
        private void dbUpdateNutri()
        {
            using (MySqlConnection connection = new MySqlConnection("Server = 192.168.1.225; Port = 3306; Database = dbtest; Uid = root1; Pwd = crew1207"))
            {
                try
                {
                    connection.Open();
                    string sql = "SELECT * FROM NUTRI_INFO";
                    MySqlCommand cmd = new MySqlCommand(sql, connection);
                    MySqlDataReader table = cmd.ExecuteReader();

                    while (table.Read())
                    {
                        dataGridView5.Rows.Add(table["NUTRI_LINE"], table["NUTRI_INFO"], table["NUTRI_DATE"], table["NUTRI_CONTROL"]);
                        if (table["NUTRI_INFO"].ToString() == "부족")
                        {
                            //제어기 온
                            //표옆에 빨간색으로 표시
                        }
                    }
                    table.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("실패");
                    Console.WriteLine(ex.ToString());
                }
            }
        }
        //게시판
        private void dbUpdateBoard()
        {
            using (MySqlConnection connection = new MySqlConnection("Server = 192.168.1.225; Port = 3306; Database = dbtest; Uid = root1; Pwd = crew1207"))
            {
                try
                {
                    // 쿼리문 두번써서 => 
                    connection.Open();
                    string sql = "SELECT * FROM BOARD_INFO WHERE BOARD_ID = 'racoon'";
                    MySqlCommand cmd = new MySqlCommand(sql, connection);
                    MySqlDataReader table = cmd.ExecuteReader();
                    dataGridView6.Rows.Clear();
                    while (table.Read())
                    {
                        dataGridView6.Rows.Add(table["BOARD_TITLE"], table["BOARD_ID"], table["BOARD_DATE"]);
                    }
                    table.Close();

                    string sql2 = "SELECT * FROM BOARD_INFO WHERE BOARD_ID NOT IN ('racoon')";
                    MySqlCommand cmd2 = new MySqlCommand(sql2, connection);
                    MySqlDataReader table2 = cmd2.ExecuteReader();
                    while(table2.Read())
                    {
                        dataGridView6.Rows.Add(table2["BOARD_TITLE"], table2["BOARD_ID"], table2["BOARD_DATE"]);
                    }
                    table2.Close();

                }
                catch (Exception ex)
                {
                    Console.WriteLine("실패");
                    Console.WriteLine(ex.ToString());
                }
            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            
        }

        //온도 검색
        private void button4_Click(object sender, EventArgs e)
        {
            using (MySqlConnection connection = new MySqlConnection("Server = 192.168.1.225; Port = 3306; Database = dbtest; Uid = root1; Pwd = crew1207"))
            {
                dataGridView1.Rows.Clear();
                try
                {
                    connection.Open();
                    string sql = "";
                    if (searchTemp == "TEMP_DATE")
                    {
                        if (!Regex.IsMatch(textBox3.Text.ToString(), @"^(19|20)\d{2}-(0[1-9]|1[012])-(0[1-9]|[12][0-9]|3[0-1])$"))
                        {
                            MessageBox.Show("0000-00-00 으로 입력해주세요.");
                            return;
                        }
                        sql = $"SELECT * FROM TEMP_INFO WHERE DATE > '{textBox3.Text}'";
                    }else if(searchTemp == "")
                    {
                        sql = $"SELECT * FROM TEMP_INFO";
                    }
                    else
                    {
                        sql = $"SELECT * FROM TEMP_INFO WHERE {searchTemp} = '{textBox3.Text}'";
                    }

                    MySqlCommand cmd = new MySqlCommand(sql, connection);
                    MySqlDataReader table = cmd.ExecuteReader();

                    while (table.Read())
                    {
                        dataGridView1.Rows.Add(table["TEMP_LINE"], table["TEMP_TEMP"], table["TEMP_DATE"], table["TEMP_CONTROL"]);
                    }
                    table.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("실패");
                    Console.WriteLine(ex.ToString());
                }
            }
        }
        //온도 검색
        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {
            searchTemp = "";
            textBox3.Text = "";
        }

        private void radioButton7_CheckedChanged(object sender, EventArgs e)
        {
            searchTemp = "TEMP_LINE";
            textBox3.Text = "";
        }

        private void radioButton8_CheckedChanged(object sender, EventArgs e)
        {
            searchTemp = "TEMP_TEMP";
            textBox3.Text = "";
        }

        private void radioButton9_CheckedChanged(object sender, EventArgs e)
        {
            searchTemp = "TEMP_DATE";
            textBox3.Text = "";
        }

        private void radioButton10_CheckedChanged(object sender, EventArgs e)
        {
            searchTemp = "TEMP_CONTROL";
            textBox3.Text = "";
        }
        //습도 검색
        private void button5_Click(object sender, EventArgs e)
        {
            using (MySqlConnection connection = new MySqlConnection("Server = 192.168.1.225; Port = 3306; Database = dbtest; Uid = root1; Pwd = crew1207"))
            {
                dataGridView3.Rows.Clear();
                try
                {
                    connection.Open();
                    string sql = "";
                    if (searchHumi == "HUMI_DATE")
                    {
                        if (!Regex.IsMatch(textBox4.Text.ToString(), @"^(19|20)\d{2}-(0[1-9]|1[012])-(0[1-9]|[12][0-9]|3[0-1])$"))
                        {
                            MessageBox.Show("0000-00-00 으로 입력해주세요.");
                            return;
                        }
                        sql = $"SELECT * FROM HUMI_INFO WHERE DATE > '{textBox4.Text}'";
                    }
                    else if (searchHumi == "")
                    {
                        sql = $"SELECT * FROM HUMI_INFO";
                    }
                    else
                    {
                        sql = $"SELECT * FROM HUMI_INFO WHERE {searchHumi} = '{textBox4.Text}'";
                    }

                    MySqlCommand cmd = new MySqlCommand(sql, connection);
                    MySqlDataReader table = cmd.ExecuteReader();

                    while (table.Read())
                    {
                        dataGridView3.Rows.Add(table["HUMI_LINE"], table["HUMI_HUMI"], table["HUMI_DATE"], table["HUMI_CONTROL"]);
                    }
                    table.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("실패");
                    Console.WriteLine(ex.ToString());
                }
            }
        }
        //습도 검색
        private void radioButton15_CheckedChanged(object sender, EventArgs e)
        {
            searchTemp = "";
            textBox4.Text = "";
        }

        private void radioButton14_CheckedChanged(object sender, EventArgs e)
        {
            searchTemp = "HUNI_LINE";
            textBox4.Text = "";
        }

        private void radioButton13_CheckedChanged(object sender, EventArgs e)
        {
            searchTemp = "HUMI_HUMI";
            textBox4.Text = "";
        }

        private void radioButton12_CheckedChanged(object sender, EventArgs e)
        {
            searchTemp = "HUMI_DATE";
            textBox4.Text = "";
        }

        private void radioButton11_CheckedChanged(object sender, EventArgs e)
        {
            searchTemp = "HUMI_CONTROL";
            textBox4.Text = "";
        }
        //토양 검색
        private void button1_Click_1(object sender, EventArgs e)
        {
            using (MySqlConnection connection = new MySqlConnection("Server = 192.168.1.225; Port = 3306; Database = dbtest; Uid = root1; Pwd = crew1207"))
            {
                dataGridView2.Rows.Clear();
                try
                {
                    connection.Open();
                    string sql = "";
                    if (searchLand == "LAND_DATE")
                    {
                        if (!Regex.IsMatch(textBox1.Text.ToString(), @"^(19|20)\d{2}-(0[1-9]|1[012])-(0[1-9]|[12][0-9]|3[0-1])$"))
                        {
                            MessageBox.Show("0000-00-00 으로 입력해주세요.");
                            return;
                        }
                        sql = $"SELECT * FROM LAND_INFO WHERE DATE > '{textBox1.Text}'";
                    }
                    else if (searchLand == "")
                    {
                        sql = $"SELECT * FROM LAND_INFO";
                    }
                    else
                    {
                        sql = $"SELECT * FROM LAND_INFO WHERE {searchLand} = '{textBox1.Text}'";
                    }

                    MySqlCommand cmd = new MySqlCommand(sql, connection);
                    MySqlDataReader table = cmd.ExecuteReader();

                    while (table.Read())
                    {
                        dataGridView2.Rows.Add(table["LAND_LINE"], table["LAND_INFO"], table["LAND_DATE"], table["LAND_CONTROL"]);
                    }
                    table.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("실패");
                    Console.WriteLine(ex.ToString());
                }
            }
        }
        //토양
        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            searchLand = "";
            textBox1.Text = "";
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            searchLand = "LAND_LINE";
            textBox1.Text = "";
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            searchLand = "LAND_INFO";
            textBox1.Text = "";
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            searchLand = "LAND_DATE";
            textBox1.Text = "";
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            searchLand = "LAND_CONTROL";
            textBox1.Text = "";
        }
        //CO2
        private void button6_Click(object sender, EventArgs e)
        {
            using (MySqlConnection connection = new MySqlConnection("Server = 192.168.1.225; Port = 3306; Database = dbtest; Uid = root1; Pwd = crew1207"))
            {
                dataGridView4.Rows.Clear();
                try
                {
                    connection.Open();
                    string sql = "";
                    if (searchCo2 == "CO2_DATE")
                    {
                        if (!Regex.IsMatch(textBox5.Text.ToString(), @"^(19|20)\d{2}-(0[1-9]|1[012])-(0[1-9]|[12][0-9]|3[0-1])$"))
                        {
                            MessageBox.Show("0000-00-00 으로 입력해주세요.");
                            return;
                        }
                        sql = $"SELECT * FROM CO2_INFO WHERE DATE > '{textBox5.Text}'";
                    }
                    else if (searchCo2 == "")
                    {
                        sql = $"SELECT * FROM CO2_INFO";
                    }
                    else
                    {
                        sql = $"SELECT * FROM CO2_INFO WHERE {searchCo2} = '{textBox5.Text}'";
                    }

                    MySqlCommand cmd = new MySqlCommand(sql, connection);
                    MySqlDataReader table = cmd.ExecuteReader();

                    while (table.Read())
                    {
                        dataGridView4.Rows.Add(table["CO2_LINE"], table["CO2_INFO"], table["CO2_DATE"], table["CO2_CONTROL"]);
                    }
                    table.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("실패");
                    Console.WriteLine(ex.ToString());
                }
            }
        }
        //CO2
        private void radioButton20_CheckedChanged(object sender, EventArgs e)
        {
            searchCo2 = "";
            textBox5.Text = "";
        }

        private void radioButton19_CheckedChanged(object sender, EventArgs e)
        {
            searchCo2 = "CO2_LINE";
            textBox5.Text = "";
        }

        private void radioButton18_CheckedChanged(object sender, EventArgs e)
        {
            searchCo2 = "CO2_INFO";
            textBox5.Text = "";
        }

        private void radioButton17_CheckedChanged(object sender, EventArgs e)
        {
            searchCo2 = "CO2_DATE";
            textBox5.Text = "";
        }

        private void radioButton16_CheckedChanged(object sender, EventArgs e)
        {
            searchCo2 = "CO2_CONTROL";
            textBox5.Text = "";
        }

        private void button7_Click(object sender, EventArgs e)
        {
            using (MySqlConnection connection = new MySqlConnection("Server = 192.168.1.225; Port = 3306; Database = dbtest; Uid = root1; Pwd = crew1207"))
            {
                dataGridView5.Rows.Clear();
                try
                {
                    connection.Open();
                    string sql = "";
                    if (searchNutri == "NUTRI_DATE")
                    {
                        if (!Regex.IsMatch(textBox6.Text.ToString(), @"^(19|20)\d{2}-(0[1-9]|1[012])-(0[1-9]|[12][0-9]|3[0-1])$"))
                        {
                            MessageBox.Show("0000-00-00 으로 입력해주세요.");
                            return;
                        }
                        sql = $"SELECT * FROM NUTRI_INFO WHERE DATE > '{textBox6.Text}'";
                    }
                    else if (searchNutri == "")
                    {
                        sql = $"SELECT * FROM NUTRI_INFO";
                    }
                    else
                    {
                        sql = $"SELECT * FROM NUTRI_INFO WHERE {searchNutri} = '{textBox6.Text}'";
                    }

                    MySqlCommand cmd = new MySqlCommand(sql, connection);
                    MySqlDataReader table = cmd.ExecuteReader();

                    while (table.Read())
                    {
                        dataGridView5.Rows.Add(table["NUTRI_LINE"], table["NUTRI_INFO"], table["NUTRI_DATE"], table["NUTRI_CONTROL"]);
                    }
                    table.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("실패");
                    Console.WriteLine(ex.ToString());
                }
            }
        }
        //양액
        private void radioButton25_CheckedChanged(object sender, EventArgs e)
        {
            searchNutri = "";
            textBox6.Text = "";
        }

        private void radioButton24_CheckedChanged(object sender, EventArgs e)
        {
            searchNutri = "NUTRI_LINE";
            textBox6.Text = "";
        }

        private void radioButton23_CheckedChanged(object sender, EventArgs e)
        {
            searchNutri = "NUTRI_INFO";
            textBox6.Text = "";
        }

        private void radioButton22_CheckedChanged(object sender, EventArgs e)
        {
            searchNutri = "NUTRI_DATE";
            textBox6.Text = "";
        }

        private void radioButton21_CheckedChanged(object sender, EventArgs e)
        {
            searchNutri = "NUTRI_CONTROL";
            textBox6.Text = "";
        }

        //게시판
        private void button8_Click(object sender, EventArgs e)
        {
            using (MySqlConnection connection = new MySqlConnection("Server = 192.168.1.225; Port = 3306; Database = dbtest; Uid = root1; Pwd = crew1207"))
            {
                dataGridView6.Rows.Clear();
                try
                {
                    connection.Open();
                    string sql = "";
                    if (searchBoard == "BOARD_DATE")
                    {
                        if (!Regex.IsMatch(textBox7.Text.ToString(), @"^(19|20)\d{2}-(0[1-9]|1[012])-(0[1-9]|[12][0-9]|3[0-1])$"))
                        {
                            MessageBox.Show("0000-00-00 으로 입력해주세요.");
                            return;
                        }
                        sql = $"SELECT * FROM BOARD_INFO WHERE DATE > '{textBox7.Text}'";
                    }
                    else if (searchBoard == "")
                    {
                        sql = $"SELECT * FROM BOARD_INFO";
                    }
                    else
                    {
                        sql = $"SELECT * FROM BOARD_INFO WHERE {searchBoard} = '{textBox7.Text}'";
                    }

                    MySqlCommand cmd = new MySqlCommand(sql, connection);
                    MySqlDataReader table = cmd.ExecuteReader();

                    while (table.Read())
                    {
                        dataGridView6.Rows.Add(table["BOARD_TITLE"], table["BOARD_ID"], table["BOARD_DATE"]);
                    }
                    table.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("실패");
                    Console.WriteLine(ex.ToString());
                }
            }
        }
        private void radioButton30_CheckedChanged(object sender, EventArgs e)
        {
            searchBoard = "";
            textBox7.Text = "";
        }

        private void radioButton29_CheckedChanged(object sender, EventArgs e)
        {
            searchBoard = "BOARD_TITLE";
            textBox7.Text = "";
        }

        private void radioButton28_CheckedChanged(object sender, EventArgs e)
        {
            searchBoard = "BOARD_ID";
            textBox7.Text = "";
        }

        private void radioButton27_CheckedChanged(object sender, EventArgs e)
        {
            searchBoard = "BOARD_DATE";
            textBox7.Text = "";
        }
        // 게시판 클릭
        private void dataGridView6_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            // 내글[관리자 or 자신] 수정, 삭제 / 
            using (MySqlConnection connection = new MySqlConnection("Server = 192.168.1.225; Port = 3306; Database = dbtest; Uid = root1; Pwd = crew1207"))
            {
                try
                {
                    connection.Open();
                    string sql = "SELECT * FROM BOARD_INFO";
                    MySqlCommand cmd = new MySqlCommand(sql, connection);
                    MySqlDataReader table = cmd.ExecuteReader();
                    while (table.Read())
                    {
                        // 타이틀과 아이디가 맞는 글 선택
                        if((dataGridView6.Rows[e.RowIndex].Cells[1].FormattedValue.ToString() == table["BOARD_ID"].ToString()) && (dataGridView6.Rows[e.RowIndex].Cells[0].FormattedValue.ToString() == table["BOARD_TITLE"].ToString()))
                        {
                            Board frm = new Board();
                            if (dataGridView6.Rows[e.RowIndex].Cells[1].FormattedValue.ToString() == board_id || board_id == "racoon") 
                            {
                                frm.board_flag = "TRUE";
                            } else
                            {
                                frm.board_flag = "FALSE";
                            }
                            frm.board_idx = table["BOARD_IDX"].ToString();
                            frm.board_id = table["BOARD_ID"].ToString();
                            frm.board_title = table["BOARD_TITLE"].ToString();
                            frm.board_content = table["BOARD_CONTENT"].ToString();
                            frm.board_date = table["BOARD_DATE"].ToString();
                            frm.ShowDialog();
                        }
                    }
                    table.Close();
                }
                catch
                {

                }
            }
            dbUpdateBoard();
        }
        // 게시글 작성
        private void button2_Click(object sender, EventArgs e)
        {
            WriteBoard frm = new WriteBoard();
            frm.board_id = board_id;
            frm.ShowDialog();
            dbUpdateBoard();
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            Manager frm = new Manager();
            frm.ShowDialog();
        }
        // 로그아웃
        private void button9_Click(object sender, EventArgs e)
        {
            logOut();
        }
        // 로그아웃
        private void logOut()
        {
            this.Hide();
            byte[] data1 = new byte[1024];
            data1 = Encoding.UTF8.GetBytes($"Logout#{board_id}");
            logOut_button = true;
            Login frm = new Login();
            client.SendTo(data1, 0, data1.Length, SocketFlags.None, frm.ipep);
            MessageBox.Show("로그아웃 되었습니다.");
            data1 = Encoding.UTF8.GetBytes($"currentUser#{board_id}");
            client.SendTo(data1, 0, data1.Length, SocketFlags.None, frm.ipep);   // 데이터 삭제
            frm.ShowDialog();
            this.Close();
        }
        // 접속유저
        private void button10_Click(object sender, EventArgs e)
        {
            //byte[] data = new byte[1024];
            //data = Encoding.UTF8.GetBytes("Check");
            //Login frm = new Login();
            //frm.client.SendTo(data, data.Length, SocketFlags.None, frm.ipep);

            CerrentUser frm8 = new CerrentUser();
            frm8.ipep = ipep;
            frm8.client = client;
            frm8.senders = senders;
            frm8.board_id = board_id;
            frm8.ShowDialog();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (logOut_button == true)
            {
                return;
            }
            logOut();
            
            //t_handler.Abort();
            Console.WriteLine("Data Waiting until thread stops...");
            t_handler.Interrupt();
            Console.WriteLine("interrupt");
            t_handler.Join();
            Console.WriteLine("finish");
        }
        //임시 업로드
        private void button11_Click(object sender, EventArgs e)
        {
            OpenFileDialog pFileDlg = new OpenFileDialog();
            pFileDlg.Filter = "Text Files(*.txt)|*.txt|All Files(*.*)|*.*";
            pFileDlg.Title = "파일을 선택해주세요.";
            if (pFileDlg.ShowDialog() == DialogResult.OK)
            {
                String strFullPathFile = pFileDlg.FileName;
                // ToDo
            }
        }

        private void dataGridView7_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if(dataGridView7.Rows[e.RowIndex].Cells[2].FormattedValue.ToString() == "A")
            {
                MessageBox.Show("안드로이드 접속자는 채팅을 이용할 수 없습니다.");
            } else
            {
                data = Encoding.UTF8.GetBytes($"Chat#{dataGridView7.Rows[e.RowIndex].Cells[0].FormattedValue}#{board_id}");
                your_id = (string)dataGridView7.Rows[e.RowIndex].Cells[0].FormattedValue;
                MessageBox.Show($"{your_id}님에게 수락요청을 보냈습니다.");
                client.SendTo(data, data.Length, SocketFlags.None, ipep);

                Console.WriteLine($"{Encoding.UTF8.GetString(data)}를 보냈습니다.");
            }
            
        }
    }
}