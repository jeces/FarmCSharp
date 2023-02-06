using MySql.Data.MySqlClient;
using System;
using System.Windows.Forms;

namespace Design
{
    public partial class ManagerUser : Form
    {
        public ManagerUser()
        {
            InitializeComponent();
        }

        private void Form5_Load(object sender, EventArgs e)
        {
            dbUpdateUser();
        }

        private void dbUpdateUser()
        {
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
                        dataGridView2.Rows.Add(table["MEMBER_ID"], table["MEMBER_NAME"], table["MEMBER_PHONE"], table["MEMBER_EMAIL"], table["MEMBER_DATE"], table["MEMBER_APPROVAL"]);
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
