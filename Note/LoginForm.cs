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
    public partial class LoginForm : Form
    {
        SqlConnection sqlConnection;
        bool login = false;

        public  LoginForm()
        {
            InitializeComponent();
            string connectionstring = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\User\source\repos\Note\DatabaseNote.mdf;Integrated Security=True";
            sqlConnection = new SqlConnection(connectionstring);
        }        

        private async void LoginForm_Load(object sender, EventArgs e)
        {
            await sqlConnection.OpenAsync();
        }

        private void LoginForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (!login)
            {
            Application.Exit();
            }
            sqlConnection.Close();          
        }

        private async void button1_Click(object sender, EventArgs e) // REGISTRATION
        {
            if (!string.IsNullOrEmpty(textBox1.Text) && !string.IsNullOrWhiteSpace(textBox1.Text) &&
                    !string.IsNullOrEmpty(textBox2.Text) && !string.IsNullOrWhiteSpace(textBox2.Text))
            {
                SqlCommand command1 = new SqlCommand("SELECT  [UserName] FROM [LogUsers] WHERE UserName LIKE @UserName", sqlConnection);

                command1.Parameters.AddWithValue("UserName", textBox1.Text.ToLowerInvariant());

                SqlDataReader sqlDataReader = command1.ExecuteReader();

                if (!sqlDataReader.HasRows)
                {
                    SqlCommand command = new SqlCommand("INSERT INTO [LogUsers] (UserName, Password) VALUES(@UserName, @Password)", sqlConnection);

                    command.Parameters.AddWithValue("UserName", textBox1.Text.ToLowerInvariant());

                    command.Parameters.AddWithValue("Password", textBox2.Text);

                    sqlDataReader.Close();

                    await command.ExecuteNonQueryAsync();

                    MessageBox.Show("Успешная Регистрация!", "Регистрация", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
                else
                {
                    MessageBox.Show("Такой пользователь уже есть!", "Регистрация", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    sqlDataReader.Close();
                }
            }

            else
            {
                MessageBox.Show("Неправильно заполнены поля", "Регистрация", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void button2_Click(object sender, EventArgs e) // LOGON
        {
            if (!string.IsNullOrEmpty(textBox1.Text) && !string.IsNullOrWhiteSpace(textBox1.Text) && !string.IsNullOrEmpty(textBox2.Text) && !string.IsNullOrWhiteSpace(textBox2.Text))
            {
                SqlCommand command3 = new SqlCommand("SELECT  [UserName],[Password],[Id] FROM [LogUsers] WHERE UserName LIKE @UserName", sqlConnection);

                command3.Parameters.AddWithValue("UserName", textBox1.Text.ToLowerInvariant());

                SqlDataReader sqlDataReader34 = command3.ExecuteReader();

                // UserId = Convert.ToInt32(sqlDataReader34["Id"]);
                if (!sqlDataReader34.HasRows) MessageBox.Show("Неверный Пользователь", "Вход", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (sqlDataReader34.Read())
                {
                    if (Convert.ToString(sqlDataReader34["Password"]) == textBox2.Text)
                    {
                        sqlDataReader34.Close();
                        login = true;
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Неверный пароль", "Вход", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        sqlDataReader34.Close();
                    }
                }
            }
            else
            {
                MessageBox.Show("Неправильно заполнены поля", "Вход", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
    }

}

