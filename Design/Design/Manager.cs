using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using MetroFramework.Forms;

namespace Design
{
    public partial class Manager : MetroForm
    {
        public Manager()
        {
            InitializeComponent();
            updateUser();
            updateUser2();
        }

        private void updateUser2()
        {
            using (MySqlConnection connection = new MySqlConnection("Server = 192.168.1.225; Port = 3306; Database = dbtest; Uid = root1; Pwd = crew1207"))
            {
                try
                {
                    connection.Open();
                    string sql = $"SELECT * FROM MEMBER_INFO";

                    MySqlCommand cmd = new MySqlCommand(sql, connection);
                    MySqlDataReader table = cmd.ExecuteReader();

                    while (table.Read())
                    {
                        if (table["MEMBER_ID"].ToString() == "racoon")
                        {
                            continue;
                        }
                        checkedListBox2.Items.Add(table["MEMBER_ID"]);
                    }
                }
                catch
                {

                }
            }
        }
        private void updateUser()
        {
            using (MySqlConnection connection = new MySqlConnection("Server = 192.168.1.225; Port = 3306; Database = dbtest; Uid = root1; Pwd = crew1207"))
            {
                try
                {
                    connection.Open();
                    string sql = $"SELECT * FROM MEMBER_INFO WHERE MEMBER_APPROVAL = 0";

                    MySqlCommand cmd = new MySqlCommand(sql, connection);
                    MySqlDataReader table = cmd.ExecuteReader();

                    while(table.Read())
                    {
                        if (table["MEMBER_ID"].ToString() == "racoon")
                        {
                            continue;
                        }
                        checkedListBox1.Items.Add(table["MEMBER_ID"]);
                        
                    }
                }
                catch
                {

                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            List<string> list = new List<string>();
            String[] str = { };
            for (int x = 0; x < checkedListBox1.CheckedItems.Count; ++x)
            {
                list.Add(checkedListBox1.CheckedItems[x].ToString());
                str = list.ToArray();
            }
            using (MySqlConnection connection = new MySqlConnection("Server = 192.168.1.225; Port = 3306; Database = dbtest; Uid = root1; Pwd = crew1207"))
            {
                try
                {
                    connection.Open();
                    for (int x = 0; x < str.Count(); ++x)
                    {
                        string sql = $"UPDATE MEMBER_INFO SET MEMBER_APPROVAL = 1 WHERE MEMBER_ID = '{str[x]}'";
                        MySqlCommand cmd = new MySqlCommand(sql, connection);
                        MySqlDataReader table = cmd.ExecuteReader();
                        table.Close();
                    }
                    checkedListBox1.Items.Clear();
                    checkedListBox2.Items.Clear();
                    updateUser2();
                    updateUser();
                    MessageBox.Show("체크된 아이디를 승인하였습니다.");
                }
                catch
                {

                }
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            List<string> list = new List<string>();
            String[] str = { };
            for (int x = 0; x < checkedListBox2.CheckedItems.Count; ++x)
            {
                list.Add(checkedListBox2.CheckedItems[x].ToString());
                str = list.ToArray();
            }
            using (MySqlConnection connection = new MySqlConnection("Server = 192.168.1.225; Port = 3306; Database = dbtest; Uid = root1; Pwd = crew1207"))
            {
                try
                {
                    connection.Open();
                    for (int x = 0; x < str.Count(); ++x)
                    {
                        string sql = $"DELETE FROM MEMBER_INFO WHERE MEMBER_ID = '{str[x]}'";
                        MySqlCommand cmd = new MySqlCommand(sql, connection);
                        MySqlDataReader table = cmd.ExecuteReader();
                        table.Close();
                    }
                    checkedListBox1.Items.Clear();
                    checkedListBox2.Items.Clear();
                    updateUser2();
                    updateUser();
                    MessageBox.Show("선택된 아이디를 삭제하였습니다.");
                }
                catch
                {

                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ManagerUser frm = new ManagerUser();
            frm.ShowDialog();
        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }
    }
}
