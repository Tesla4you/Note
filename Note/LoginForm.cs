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

        public LoginForm()
        {
            InitializeComponent();
            string connectionstring = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\User\source\repos\Note\Note\DatabaseNote.mdf;Integrated Security=True";
            sqlConnection = new SqlConnection(connectionstring);
        }        

        private void LoginForm_Load(object sender, EventArgs e)
        {
            
        }

        private void LoginForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (!login)
            {
            Application.Exit();
            }
            if (sqlConnection != null && sqlConnection.State != ConnectionState.Closed)
            {
                sqlConnection.Close();
            }
        }

        private async void button1_Click(object sender, EventArgs e) // REGISTRATION
        {
            if (!string.IsNullOrEmpty(textBox1.Text) && !string.IsNullOrWhiteSpace(textBox1.Text) &&
                    !string.IsNullOrEmpty(textBox2.Text) && !string.IsNullOrWhiteSpace(textBox2.Text))
            {
                SqlCommand command1 = new SqlCommand("SELECT  [Name] FROM [User] WHERE Name LIKE @Name", sqlConnection);

                command1.Parameters.AddWithValue("Name", textBox1.Text.ToLowerInvariant());

                SqlDataReader sqlDataReader = command1.ExecuteReader();

                if (!sqlDataReader.HasRows)
                {
                    SqlCommand command = new SqlCommand("INSERT INTO [User] (Name, Password) VALUES(@Name, @Password)", sqlConnection);

                    command.Parameters.AddWithValue("Name", textBox1.Text.ToLowerInvariant());

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
            
        }

    }
}
