using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Note
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();            
            button1.Visible = false;
            button2.Visible = false;
            textBox1.Visible = false;
            textBox2.Visible = false;           
        }
    

        SqlConnection sqlConnection;

        string username;
        int userid;
        int noteuserid;
        int noteid;

        private async void Form1_Load(object sender, EventArgs e)
        {
            LoginForm loginForm = new LoginForm();
            loginForm.ShowDialog();
            userid = loginForm.UserId;
            username = loginForm.UserName;

            var directory = System.IO.Directory.GetCurrentDirectory();
            var connectionstring = @"Data Source=(localdb)\mssqllocaldb;AttachDbFilename=" + directory + @"\NoteDataBase.mdf;Integrated Security=True;Connect Timeout=30;";
            //string connectionstring = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\User\source\repos\Note\Note\NoteDataBase.mdf;Integrated Security=True";
            sqlConnection = new SqlConnection(connectionstring);
            label1.Text = $" Добро пожаловать {username} Ваш айди {userid}";
            await sqlConnection.OpenAsync();

            refresh();
        }
        
        async void refresh() // ОБНОВЛЯЕТ СПИСОК ЗАМЕТОК
        {
            listBox1.Items.Clear();
            SqlDataReader sqlDataReader = null;

            SqlCommand command1 = new SqlCommand("SELECT  [Title] FROM [Note] ", sqlConnection);

            try
            {
                sqlDataReader = await command1.ExecuteReaderAsync();

                while (await sqlDataReader.ReadAsync())
                {
                    listBox1.Items.Add(Convert.ToString(sqlDataReader["Title"]));
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.Source,MessageBoxButtons.OK, MessageBoxIcon.Error);                
            }
            finally
            {
                if (sqlDataReader != null)
                sqlDataReader.Close();
            }
        }


        private async void listBox1_SelectedIndexChanged(object sender, EventArgs e) // ВЫБОР В ЛИСТБОКСЕ
        {
            textBox1.Visible = true;
            textBox2.Visible = true;           
            SqlDataReader sqlDataReader = null;

            if (listBox1.SelectedItem != null) 
            {
                string curItem = listBox1.SelectedItem.ToString();

                SqlCommand command35 = new SqlCommand("SELECT  [Text], [UserId], [NoteId] FROM [Note] WHERE Title LIKE @Title", sqlConnection);

                command35.Parameters.AddWithValue("Title", curItem);
                try
                {
                    sqlDataReader = await command35.ExecuteReaderAsync();

                    if (sqlDataReader.Read())
                    {
                        textBox1.Text = sqlDataReader["Text"].ToString();
                        textBox2.Text = curItem;
                        noteuserid = Convert.ToInt32(sqlDataReader["UserId"]);
                        noteid = Convert.ToInt32(sqlDataReader["NoteId"]);

                        if (noteuserid == userid)
                        {
                            button1.Visible = true;
                            button2.Visible = true;
                            label2.Visible = true;
                        }
                        else
                        {
                            button1.Visible = false;
                            button2.Visible = false;
                            label2.Visible = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, ex.Source, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    if( sqlDataReader!=null )
                    sqlDataReader.Close();
                }
            }
        }

        private async void button1_Click(object sender, EventArgs e) // КНОПКА ИЗМЕНИТЬ ЗАМЕТКУ
        {
            if (textBox2.Text == "Пожалуйста переименуйте заголовок" || string.IsNullOrEmpty(textBox2.Text) || string.IsNullOrWhiteSpace(textBox2.Text)) // !!
            {
                MessageBox.Show("ПЕРЕИМЕНУЙТЕ ЗАГОЛОВОК", "CRITICAL ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                SqlDataReader sqlDataReader = null;
                SqlCommand commandupsel = new SqlCommand("SELECT  [Title] FROM [Note] WHERE [Noteid] = @Noteid", sqlConnection);
                commandupsel.Parameters.AddWithValue("Noteid", noteid);
                try
                {
                    sqlDataReader = await commandupsel.ExecuteReaderAsync();
                    string title;
                    if (sqlDataReader.Read())
                    {
                        title = sqlDataReader["Title"].ToString();
                        sqlDataReader.Close();

                        if (!listBox1.Items.Contains(textBox2.Text) || textBox2.Text == title)
                        {
                            SqlCommand commandup = new SqlCommand("UPDATE [Note] SET [UserId] =@UserId, [Title] = @Title, [Text]=@Text WHERE [NoteId] = @NoteId", sqlConnection);
                            commandup.Parameters.AddWithValue("UserId", userid);
                            commandup.Parameters.AddWithValue("Title", textBox2.Text);
                            commandup.Parameters.AddWithValue("Text", textBox1.Text);
                            commandup.Parameters.AddWithValue("NoteId", noteid);
                            await commandup.ExecuteNonQueryAsync();
                            refresh();
                        }
                        else
                        {
                            MessageBox.Show("Такая заметка уже существует");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, ex.Source, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    sqlDataReader.Close();
                }               
            }
        }

        private async void button2_Click(object sender, EventArgs e) // КНОПКА УДАЛИТЬ
        {
            SqlCommand commanddel = new SqlCommand("DELETE FROM [NOTE] WHERE [Noteid] = @Noteid", sqlConnection);
            commanddel.Parameters.AddWithValue("NoteId", noteid);
            await commanddel.ExecuteNonQueryAsync();
            refresh();
            textBox1.Visible = false;
            textBox2.Visible = false;
            label2.Visible = false;           
        }

        private async void button3_Click(object sender, EventArgs e) // НОВАЯ ЗАМЕТКА
        {
            textBox1.Visible = false;
            textBox2.Visible = false;
            label2.Visible = false;            
            textBox1.Text = "Новая заметка";
            textBox2.Text = "Заметка " + (listBox1.Items.Count + 1);
            if (listBox1.Items.Contains(textBox2.Text))
            {
                textBox2.Text = "Пожалуйста переименуйте заголовок";
            }
            SqlCommand commandnew = new SqlCommand("INSERT INTO [Note] (UserId, Title, Text) VALUES(@UserId,@Title, @Text)", sqlConnection);
            commandnew.Parameters.AddWithValue("UserId", userid);
            commandnew.Parameters.AddWithValue("Title", textBox2.Text);
            commandnew.Parameters.AddWithValue("Text", textBox1.Text);
            await commandnew.ExecuteNonQueryAsync();
            refresh();
            label5.Text = "";
            MessageBox.Show("Заметка создана");
        }
    }
   
}

