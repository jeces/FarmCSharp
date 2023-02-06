using Project_01.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Project_01
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            // 스레드로 전체 데이터베이스 출력
            Task.Run(() => DoListSometing());
        }
        // Refresh ListView1을 위한 메서드(전체 데이터베이스 출력)
        private void DoListSometing(int index = 0)
        {
            this.listView1.Items.Clear();
            this.listView3.Items.Clear();
            using (var db = new BooksDbContext())
            {
                // ListView 컬럼의 항목별 정렬
                List<Author> text = null;
                if (index == 0)
                    text = db.Authors.ToList();
                else if (index == 1)
                    text = db.Authors.OrderBy(n => n.Name).ToList();
                else if (index == 2)
                    text = db.Authors.OrderBy(n => n.Birthday.Year).ToList();
                else if (index == 3)
                    text = db.Authors.OrderBy(n => n.Gender).ToList();
                else if (index == 4)
                    text = db.Authors.OrderByDescending(n => n.Books.Count).ToList();
                foreach (var author in text)
                {
                    var str = string.Empty;
                    foreach (var book in author.Books)
                    {
                        str += $"[{book.Title}({book.PublishedYear}년)] ";
                    }
                    this.listView1.Items.Add(new ListViewItem(new string[] { author.Name, author.Birthday.ToString("d"), author.Gender, str }));
                    this.listView3.Items.Add(new ListViewItem(new string[] { author.Name }));
                }
            }
            listView1.Items[listView1.Items.Count - 1].EnsureVisible();
            listView3.Items[listView3.Items.Count - 1].EnsureVisible();
        }
        // Textbox의 전체 초기화를 위한 메서드
        private void Refresh_Textbox()
        {
            this.richTextBox1.Text = string.Empty;
            this.richTextBox2.Text = string.Empty;
            this.richTextBox3.Text = string.Empty;
            this.richTextBox4.Text = string.Empty;
            this.richTextBox6.Text = string.Empty;
            this.richTextBox7.Text = string.Empty;
        }
        // Author 삽입을 위한 이벤트
        private void button1_Click(object sender, EventArgs e)
        {
            if (this.richTextBox1.Text == "" || this.richTextBox2.Text == "" || this.richTextBox3.Text == "")
            {
                MessageBox.Show("저자, 출생년도, 성별을 확인해주세요.");
                return;
            }
            using (var db = new BooksDbContext())
            {
                foreach(var s in db.Authors.ToList())
                {
                    if(s.Name == this.richTextBox1.Text)
                    {
                        MessageBox.Show($"{s.Name}은 이미 가지고 있는 저자입니다.");
                        return;
                    }
                }
                var author = new Author
                {
                    Name = this.richTextBox1.Text,
                    Birthday = Convert.ToDateTime(this.richTextBox2.Text),
                    Gender = this.richTextBox3.Text,
                };
                db.Authors.Add(author);
                db.SaveChanges();
                MessageBox.Show($"{this.richTextBox1.Text}의 저자를 추가하였습니다.");
                DoListSometing();
            }
            Refresh_Textbox();
        }
        // Author 삭제를 위한 이벤트
        private void button2_Click(object sender, EventArgs e)
        {
            if (this.richTextBox1.Text == "")
            {
                MessageBox.Show("삭제할 저자를 입력해주세요.");
                return;
            }
            using (var db = new BooksDbContext())
            {
                var Count = 0;
                foreach (var s in db.Authors.ToList())
                {
                    if (s.Name == this.richTextBox1.Text)
                    {
                        ++Count;
                        break;
                    }
                }
                if(Count == 0)
                {
                    MessageBox.Show($"{this.richTextBox1.Text}은 데이터베이스에 없는 저자입니다.");
                    return;
                }
                var delet = db.Authors.SingleOrDefault(n => n.Name == this.richTextBox1.Text/* || n.Birthday == Convert.ToDateTime(this.richTextBox2.Text) || n.Gender == this.richTextBox3.Text*/);
                if(delet != null)
                {
                    var book_del = db.Books.Where(n => n.Author.Name == this.richTextBox1.Text).ToList();
                    foreach(var s in book_del)
                    {
                        db.Books.Remove(s);
                    }
                    db.Authors.Remove(delet);
                    db.SaveChanges();
                    DoListSometing();
                    MessageBox.Show($"{this.richTextBox1.Text}를 삭제하였습니다.");
                }
            }
            Refresh_Textbox();
        }
        // Author 수정을 위한 이벤트
        private void button3_Click(object sender, EventArgs e)
        {
            if (this.richTextBox1.Text == "")
            {
                MessageBox.Show("수정할 저자를 입력해주세요.");
                return;
            }
            using (var db = new BooksDbContext())
            {
                var Count = 0;
                foreach (var s in db.Authors.ToList())
                {
                    if (s.Name == this.richTextBox1.Text)
                    {
                        ++Count;
                        break;
                    }
                }
                if (Count == 0)
                {
                    MessageBox.Show($"{this.richTextBox1.Text}은 데이터베이스에 없는 저자입니다.");
                    return;
                }
                var change = db.Authors.SingleOrDefault(n => n.Name == this.richTextBox1.Text);
                change.Birthday = Convert.ToDateTime(this.richTextBox2.Text);
                change.Gender = this.richTextBox3.Text;
                db.SaveChanges();
                DoListSometing();
                MessageBox.Show($"{this.richTextBox1.Text}를 수정하였습니다.");
            }
            Refresh_Textbox();
        }
        // 검색을 위한 이벤트(radiocheck로 원하는 검색카테고리 설정)
        private void button4_Click(object sender, EventArgs e)
        {
            if(this.richTextBox4.Text == "")
            {
                MessageBox.Show("검색어를 입력해주세요.");
                return;
            }
            this.listView2.Items.Clear();
            using (var db = new BooksDbContext())
            {
                if(this.radioButton1.Checked)
                {
                    var searchList = db.Authors.Where(n => n.Name.Contains(this.richTextBox4.Text)).ToList();
                    if (DoNotSearch(this.richTextBox4.Text, searchList) > 0)
                        return;
                    DoSearch(searchList);
                }
                else if(this.radioButton2.Checked)
                {
                    int str = Convert.ToInt32(this.richTextBox4.Text);
                    var searchList = db.Authors.Where(n => n.Birthday.Year >= str).ToList();
                    if (DoNotSearch(this.richTextBox4.Text, searchList) > 0)
                        return;
                    DoSearch(searchList);
                }
                else if(this.radioButton3.Checked)
                {
                    var searchList = db.Authors.Where(n => n.Gender.Contains(this.richTextBox4.Text)).ToList();
                    if (DoNotSearch(this.richTextBox4.Text, searchList) > 0)
                        return;
                    DoSearch(searchList);
                }
                else if (this.radioButton4.Checked)
                {
                    //var searchList = db.Authors.Where(n => n.Books.Contains(Where(s => s.Title.ToString().Contains(str)).ToList().Count > 0).Distinct().ToList();
                    // 만약 한 author의 books 객체에 d, dd, ddd란 책이 있는데 이걸 contains에 포함시켜버리면 d의 books를 가져오고 그담 또 가져오고 겹쳐버리게 됨.
                    var a = db.Books.Where(n => n.Title.Contains(this.richTextBox4.Text)).ToList();
                    var b = new List<Author>();
                    foreach(var s in a)
                        b.Add(s.Author);
                    if (DoNotSearch(this.richTextBox4.Text, b.Distinct().ToList()) > 0)
                        return;
                    DoSearch(b.Distinct().ToList());
                }
                else if (this.radioButton5.Checked)
                {
                    int str = Convert.ToInt32(this.richTextBox4.Text);
                    var searchList = db.Authors.Where(n => n.Books.Where(s => s.PublishedYear >= str).ToList().Count > 0).ToList();
                    if (DoNotSearch(this.richTextBox4.Text, searchList) > 0)
                        return;
                    DoSearch(searchList);
                }
            }
            Refresh_Textbox();
        }
        // 검색 시 검색값이 없을 때 발생하는 메서드
        private int DoNotSearch(string text, List<Author> listSearch)
        {
            var count = 0;
            if (listSearch.Count == 0)
            {
                MessageBox.Show($"{text}는 없습니다.");
                return ++count;
            }
            return count;
        }
        // 검색의 원하는 List를 가져와서 ListView2에 추가해주는 메서드
        private void DoSearch(List<Author> SearchList)
        {
            using (var db = new BooksDbContext())
            {
                foreach (var author in SearchList)
                {
                    var str = string.Empty;
                    foreach (var book in author.Books)
                    {
                        str += $"[{book.Title}({book.PublishedYear})]";
                    }
                    this.listView2.Items.Add(new ListViewItem(new string[] { author.Name, author.Birthday.ToString("d"), author.Gender, str }));
                }
            }
            listView2.Items[listView2.Items.Count - 1].EnsureVisible();
        }
        // Book 삽입을 위한 이벤트
        private void button5_Click(object sender, EventArgs e)
        {
            if (this.label7.Text == "옆 저자목록 더블클릭" || this.richTextBox6.Text == "" || this.richTextBox7.Text == "")
            {
                MessageBox.Show("저자, 타이틀, 출판년도를 확인해주세요.");
                return;
            }
            using (var db = new BooksDbContext())
            {
                foreach (var s in db.Books.ToList())
                {
                    if (s.Title == this.richTextBox6.Text)
                    {
                        MessageBox.Show($"{s.Title}은 이미 가지고 있는 책입니다.");
                        return;
                    }
                }
                var author = db.Authors.FirstOrDefault(n => n.Name == this.label7.Text);
                var book = new Book
                {
                    Title = this.richTextBox6.Text,
                    PublishedYear = Convert.ToInt32(this.richTextBox7.Text),
                    Author = author,
                };
                db.Books.Add(book);
                db.SaveChanges();
                MessageBox.Show($"{this.label7.Text}의 책을 추가하였습니다.");
                DoListSometing();
            }
            Refresh_Textbox();
        }
        // Book 삭제를 위한 이벤트
        private void button6_Click(object sender, EventArgs e)
        {
            if (this.richTextBox6.Text == "")
            {
                MessageBox.Show("삭제할 타이틀을 입력해주세요.");
                return;
            }
            using (var db = new BooksDbContext())
            {
                var delet = db.Books.SingleOrDefault(n => n.Title == this.richTextBox6.Text);
                if (delet != null)
                {
                    db.Books.Remove(delet);
                    db.SaveChanges();
                    DoListSometing();
                    MessageBox.Show($"{this.richTextBox6.Text}{this.richTextBox7.Text}를 삭제하였습니다.");
                }
            }
            Refresh_Textbox();
        }
        // Book 수정을 위한 이벤트
        private void button7_Click(object sender, EventArgs e)
        {
            if (this.label7.Text == "")
            {
                MessageBox.Show("수정할 저자를 입력해주세요.(책을 수정)");
                return;
            }
            using (var db = new BooksDbContext())
            {
                var author = db.Authors.SingleOrDefault(n => n.Name == this.label7.Text);
                if(author == null)
                {
                    MessageBox.Show($"{this.label7.Text} 저자는 데이터베이스에 없습니다.");
                    return;
                }
                var change = db.Books.SingleOrDefault(n => n.Title == this.richTextBox6.Text);
                if(change == null)
                {
                    MessageBox.Show($"{this.richTextBox6.Text} 책은 데이터베이스에 없습니다.");
                    return;
                }
                change.PublishedYear = Convert.ToInt32(this.richTextBox7.Text);
                db.SaveChanges();
                DoListSometing();
            }
            Refresh_Textbox();
        }
        // 생년월일 포커스 떠날 시 발생 이벤트(19OO-OO-OO | 20OO-OO-OO 형태만 가능하도록)
        private void richTextBox2_Leave(object sender, EventArgs e)
        {
            Boolean ismatch = Regex.IsMatch(this.richTextBox2.Text, @"^(19|20)\d{2}-(0[1-9]|1[012])-(0[1-9]|[12][0-9]|3[0-1])$");
            if (!ismatch)
            {
                MessageBox.Show($"생년월일을 재대로 입력해주세요(ex : 19OO-OO-OO | 20OO-OO-OO)");
                this.richTextBox2.Text = string.Empty;
            }
        }
        // 성별의 포커스 떠날 시 발생 이벤트(남자, 여자만 입력 가능하도록)
        private void richTextBox3_Leave(object sender, EventArgs e)
        {
            if (!((this.richTextBox3.Text == "남자") || (this.richTextBox3.Text == "여자")))
            {
                MessageBox.Show("[남자] 과 [여자]로만 입력해주세요.");
                this.richTextBox3.Text = string.Empty;
            }
        }
        // 출판년도 richTextbox 포커스 떠날 시 발생 이벤트(4자리의 숫자만 기입 가능하도록)
        private void richTextBox7_Leave(object sender, EventArgs e)
        {
            Boolean ismatch = Regex.IsMatch(this.richTextBox7.Text, @"^(\d{4})$");
            if (!ismatch)
            {
                MessageBox.Show($"출판년도를 재대로 입력해주세요(ex : OOOO)");
                this.richTextBox7.Text = string.Empty;
            }
        }
        // ListView3의 더블클릭시 Label7로 옮기기 위한 이벤트
        private void listView3_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.label7.Text = listView3.FocusedItem.SubItems[0].Text;
        }
        // ListView1 컬럼의 항목클릭 시 정렬 이벤트
        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column == 0)
            {
                DoListSometing(1);
            }
            else if (e.Column == 1)
            {
                DoListSometing(2);
            }
            else if (e.Column == 2)
            {
                DoListSometing(3);
            }
            else if (e.Column == 3)
            {
                DoListSometing(4);
            }
        }
        // 리스트 더블 클릭시 저자TextBox로 자동입력을 위한 이벤트
        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.richTextBox1.Text = listView1.FocusedItem.SubItems[0].Text;
        }
    }
}