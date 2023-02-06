using MySql.Data.MySqlClient;
using System;
using System.Windows.Forms;

namespace DesignClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (MySqlConnection connection = new MySqlConnection("Server = 127.0.0.1; Port = 3306; Database = dbtest; Uid = root; Pwd = ntek@123"))
            {
                try
                {
                    connection.Open();
                    string sql = "SELECT * FROM ROOT";
                    MySqlCommand cmd = new MySqlCommand(sql, connection);
                    MySqlDataReader table = cmd.ExecuteReader();

                    while (table.Read())
                    {
                        if ((idbox.Text == table["rootId"].ToString()) && (psbox.Text == table["password"].ToString()))
                        {
                            this.Hide();
                            MessageBox.Show("로그인 성공");
                            Form1 frm = new Form1();
                            frm.ShowDialog();
                            this.Close();

                        }
                        else
                        {
                            MessageBox.Show("로그인 실패");
                            idbox.Text = "";
                            psbox.Text = "";
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
    }
}
