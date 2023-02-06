using MetroFramework.Forms;
using MySql.Data.MySqlClient;
using System;
using System.Windows.Forms;

namespace Design
{
    public partial class WriteBoard : MetroForm
    {
        private string board_id_value;
        public string board_id
        {
            get { return this.board_id_value; }
            set { this.board_id_value = value; }
        }
        public WriteBoard()
        {
            InitializeComponent();
        }

        private void Form7_Load(object sender, EventArgs e)
        {
            
        }
        private void writeBoard()
        {
            using (MySqlConnection connection = new MySqlConnection("Server = 192.168.1.225; Port = 3306; Database = dbtest; Uid = root1; Pwd = crew1207"))
            {
                try
                {
                    connection.Open();
                    string sql = $"INSERT INTO BOARD_INFO VALUES(0, '{board_id}', '{textBox1.Text}', '{textBox2.Text}', NOW())";
                    MySqlCommand cmd = new MySqlCommand(sql, connection);
                    MySqlDataReader table = cmd.ExecuteReader();
                    MessageBox.Show("정상적으로 작성되었습니다.");
                    table.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message.ToString());
                    MessageBox.Show("에러");
                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            using (MySqlConnection connection = new MySqlConnection("Server = 192.168.1.225; Port = 3306; Database = dbtest; Uid = root1; Pwd = crew1207"))
            {
                try
                {
                    connection.Open();
                    DialogResult dr = MessageBox.Show("작성하시겠습니까?", "알림", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                    if (dr == DialogResult.OK)
                    {
                        string sql = "SELECT * FROM BOARD_INFO";
                        MySqlCommand cmd = new MySqlCommand(sql, connection);
                        MySqlDataReader table = cmd.ExecuteReader();

                        while(table.Read())
                        {
                            if(table["BOARD_TITLE"].ToString() == textBox1.Text.ToString())
                            {
                                MessageBox.Show("이미 동일한 게시글 이름이 있습니다.");
                                table.Close();
                                return;
                            }
                        }
                        table.Close();
                        writeBoard();
                    }
                    else
                    {
                        MessageBox.Show("작성을 취소하였습니다.");
                        this.Close();
                    }
                }
                catch
                {

                }
                this.Close();
            }
        }

    }
}
