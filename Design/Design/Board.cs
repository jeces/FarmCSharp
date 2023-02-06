using MetroFramework.Forms;
using MySql.Data.MySqlClient;
using System;
using System.Windows.Forms;


namespace Design
{
    public partial class Board : MetroForm
    {
        private string board_idx_value;
        private string board_id_value;
        private string board_title_value;
        private string board_content_value;
        private string board_date_value;
        private string board_flag_value;
        public string board_idx
        {
            get { return this.board_idx_value; }
            set { this.board_idx_value = value; }
        }
        public string board_id
        {
            get { return this.board_id_value; }
            set { this.board_id_value = value; }
        }
        public string board_title
        {
            get { return this.board_title_value; }
            set { this.board_title_value = value; }
        }
        public string board_content
        {
            get { return this.board_content_value; }
            set { this.board_content_value = value; }
        }
        public string board_date
        {
            get { return this.board_date_value; }
            set { this.board_date_value = value; }
        }
        public string board_flag
        {
            get { return this.board_flag_value; }
            set { this.board_flag_value = value; }
        }
        public Board()
        {
            InitializeComponent();
        }

        private void Form6_Load(object sender, EventArgs e)
        {
            textBox1.Text = board_title;
            label1.Text = board_id;
            textBox2.Text = board_content;
            label3.Text = board_date;
            this.Text = board_title;
            if (board_flag == "TRUE")
            {
                button1.Visible = true;
                button2.Visible = true;
            }
        }
        // 수정
        private void button2_Click(object sender, EventArgs e)
        {
            button3.Visible = true;
            button2.Visible = false;
            button1.Visible = false;
            if (board_flag == "TRUE")
            {
                textBox1.ReadOnly = false;
                textBox2.ReadOnly = false;
            }
        }
        // 삭제
        private void button1_Click(object sender, EventArgs e)
        {
            using (MySqlConnection connection = new MySqlConnection("Server = 192.168.1.225; Port = 3306; Database = dbtest; Uid = root1; Pwd = crew1207"))
            {
                try
                {
                    connection.Open();
                    string sql = $"DELETE FROM BOARD_INFO WHERE BOARD_IDX = {board_idx}";
                    MySqlCommand cmd = new MySqlCommand(sql, connection);
                    MySqlDataReader table = cmd.ExecuteReader();
                    MessageBox.Show("삭제되었습니다.");
                    table.Close();
                    this.Close();
                }
                catch
                {

                }
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            using (MySqlConnection connection = new MySqlConnection("Server = 192.168.1.225; Port = 3306; Database = dbtest; Uid = root1; Pwd = crew1207"))
            {
                try
                {
                    connection.Open();
                    DialogResult dr = MessageBox.Show("수정하시겠습니까?", "알림", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                    if (dr == DialogResult.OK)
                    {
                        string sql = "SELECT * FROM BOARD_INFO";
                        MySqlCommand cmd = new MySqlCommand(sql, connection);
                        MySqlDataReader table = cmd.ExecuteReader();

                        while (table.Read())
                        {
                            if (table["BOARD_TITLE"].ToString() == textBox1.Text.ToString())
                            {
                                MessageBox.Show("이미 동일한 게시글 이름이 있습니다.");
                                table.Close();
                                return;
                            }
                        }
                        table.Close();
                        updateBoard();
                    }
                    else
                    {
                        MessageBox.Show("수정을 취소하였습니다.");
                        this.Close();
                    }
                }
                catch
                {

                }
                this.Close();
            }
        }
        private void updateBoard()
        {
            using (MySqlConnection connection = new MySqlConnection("Server = 192.168.1.225; Port = 3306; Database = dbtest; Uid = root1; Pwd = crew1207"))
            {
                try
                {
                    connection.Open();
                    string sql = $"UPDATE BOARD_INFO SET BOARD_TITLE = {textBox1.Text}, BOARD_CONTENT = {textBox2.Text}, BOARD_DATE = NOW() WHERE BOARD_IDX = {board_idx}";
                    MySqlCommand cmd = new MySqlCommand(sql, connection);
                    MySqlDataReader table = cmd.ExecuteReader();
                    MessageBox.Show("수정완료되었습니다.");
                    table.Close();
                    this.Close();
                }
                catch
                {

                }
            }
        }
    }
}
