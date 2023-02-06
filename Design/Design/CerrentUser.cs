using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Design
{
    public partial class CerrentUser : Form
    {
        public IPEndPoint ipep = null;
        public Socket client = null;
        public IPEndPoint senders = null;
        byte[] data = new byte[1024];
        byte[] datas = new byte[5000000];
        private string board_id_value;
        private string your_id_value;
        private int count = 0;
        public string board_id
        {
            get { return this.board_id_value; }
            set { this.board_id_value = value; }
        }
        public string your_id
        {
            get { return this.your_id_value; }
            set { this.your_id_value = value; }
        }
        public CerrentUser()
        {
            InitializeComponent();
        }

        private void Form8_Load(object sender, EventArgs e)
        {
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.AllowUserToAddRows = false;
            // 이거 스레드 돌려야됨
            Thread t_handler = new Thread(updateLogin);
            t_handler.IsBackground = true;
            t_handler.Start();
        }

        private void updateLogin()
        {
            while (true)
            {
                
                /*
                
                */
                // 서버에서 받음
                
            }
        }

        private void updateLogin2()
        {
            using (MySqlConnection connection = new MySqlConnection("Server = 192.168.1.225; Port = 3306; Database = dbtest; Uid = root1; Pwd = crew1207"))
            {
                try
                {
                    connection.Open();
                    string sql = $"SELECT * FROM (SELECT * FROM MEMBER_INFO WHERE MEMBER_LOGIN ='O') A WHERE MEMBER_ID NOT IN('{board_id}')";
                    MySqlCommand cmd = new MySqlCommand(sql, connection);
                    MySqlDataReader table = cmd.ExecuteReader();

                    while (table.Read())
                    {
                        dataGridView1.Rows.Add(table["MEMBER_ID"], table["MEMBER_NAME"]);
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

        private void dataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            data = Encoding.UTF8.GetBytes($"Chat#{dataGridView1.Rows[e.RowIndex].Cells[0].FormattedValue}#{board_id}");
            your_id = (string)dataGridView1.Rows[e.RowIndex].Cells[0].FormattedValue;
            MessageBox.Show($"{your_id}님에게 수락요청을 보냈습니다.");
            client.SendTo(data, data.Length, SocketFlags.None, ipep);
            //this.Close();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            EndPoint remote = (EndPoint)(senders);
            string json = null;
            List<MyObject> member = null;
            if (count == 0)
            {
                Console.WriteLine("여긴한번만들어와야됨");
                data = Encoding.UTF8.GetBytes($"currentUser#{board_id}");
                client.SendTo(data, data.Length, SocketFlags.None, ipep);
                Console.WriteLine("현재 유저 기다리는중");
                Array.Clear(datas, 0x0, datas.Length);

                Console.WriteLine(Encoding.UTF8.GetString(datas).TrimEnd('\0'));
                client.ReceiveFrom(datas, ref remote);
                Console.WriteLine("데이터받음");
                json = $"{Encoding.UTF8.GetString(datas).TrimEnd('\0')}";
                if (json.TrimStart('[').TrimEnd(']') == "")
                {
                    Console.WriteLine("비어있음");
                    count++;
                    return;
                }
                member = JsonConvert.DeserializeObject<List<MyObject>>(json);
                Console.WriteLine($" ID : {member[0].MEMBER_ID}");
                dataGridView1.Rows.Clear();
                for (int i = 0; i < member.Count; ++i)
                {
                    dataGridView1.Rows.Add(member[i].MEMBER_ID, member[i].MEMBER_NAME);
                }
                count++;
            }
            else
            {
                Console.WriteLine("현재 유저 기다리는중");
                Array.Clear(datas, 0x0, datas.Length);

                Console.WriteLine(Encoding.UTF8.GetString(datas).TrimEnd('\0'));
                client.ReceiveFrom(datas, ref remote);
                Console.WriteLine("데이터받음");
                json = $"{Encoding.UTF8.GetString(datas).TrimEnd('\0')}";
                if (json.TrimStart('[').TrimEnd(']') == "")
                {
                    Console.WriteLine("비어있음");
                    count++;
                    return;
                }
                member = JsonConvert.DeserializeObject<List<MyObject>>(json);
                Console.WriteLine($" ID : {member[0].MEMBER_ID}");
                dataGridView1.Rows.Clear();
                for (int i = 0; i < member.Count; ++i)
                {
                    dataGridView1.Rows.Add(member[i].MEMBER_ID, member[i].MEMBER_NAME);
                }
                count++;
            }
        }
    }
}
