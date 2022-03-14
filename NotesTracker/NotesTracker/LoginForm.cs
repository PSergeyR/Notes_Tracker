using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace NotesTracker
{
    public partial class MainWindow : Form
    {
        SqlConnection sqlConnection;


        public MainWindow()
        {
            InitializeComponent();
        }

        private async void MainWindow_Load(object sender, EventArgs e)
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\UserS\source\repos\NotesTracker\NotesTracker\Database.mdf;Integrated Security=True";

            sqlConnection = new SqlConnection(connectionString);

            await sqlConnection.OpenAsync();

            SqlDataReader sqlReader = null;

            SqlCommand command = new SqlCommand("SELECT * FROM [Notes]", sqlConnection);

            try
            {
                sqlReader = await command.ExecuteReaderAsync();

                while (await sqlReader.ReadAsync())
                {



                    listBox1.Items.Add(Convert.ToString(sqlReader["Id"]) + "  " + Convert.ToString(sqlReader["Title"]) + "   " + "Автор: " + Convert.ToString(sqlReader["Author"]) + "   " + "Адресат: " + Convert.ToString(sqlReader["Adresat"]) + "   " + Convert.ToString(sqlReader["CreateDate"]));


                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), ex.Source.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {

                if (sqlReader != null)
                    sqlReader.Close();




            }


        }

        //Отключение от базы данных

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (sqlConnection != null && sqlConnection.State != ConnectionState.Closed)
                sqlConnection.Close();
        }

        private async void button1_Click(object sender, EventArgs e)        //Создание новой задачи
        {
            //Сообщение об незаполненных полях (изначально невидима)
            if (label9.Visible)     
                label9.Visible = false;
            //Проверки на пустые поля
            if (!string.IsNullOrEmpty(maskedTextBox1.Text) && !string.IsNullOrWhiteSpace(maskedTextBox1.Text) &&
                !string.IsNullOrEmpty(maskedTextBox2.Text) && !string.IsNullOrWhiteSpace(maskedTextBox2.Text) &&
                !string.IsNullOrEmpty(maskedTextBox3.Text) && !string.IsNullOrWhiteSpace(maskedTextBox3.Text))
            {
                //Заполнение данными из текстбоксов
                SqlCommand comand = new SqlCommand("INSERT INTO [Notes] (Title, Author, Adresat)VALUES(@Title, @Author, @Adresat )", sqlConnection);
                comand.Parameters.AddWithValue("Title", maskedTextBox1.Text);
                comand.Parameters.AddWithValue("Author", maskedTextBox3.Text);
                comand.Parameters.AddWithValue("Adresat", maskedTextBox2.Text);

                await comand.ExecuteNonQueryAsync();
                button5_Click(sender, e);
            }
            else
            {
                //Сообщение в случает незаполненных полей (красным цветом)
                label9.Visible = true;
                label9.Text = "Должны быть заполнены все поля";

            }
        }

        private async void button5_Click(object sender, EventArgs e)        //Обновление списка задач
        {
            listBox1.Items.Clear();     //Очистка списка

            SqlDataReader sqlReader = null;

            SqlCommand command = new SqlCommand("SELECT * FROM [Notes]", sqlConnection);        //Формирование запроса к базе данных

            try
            {
                sqlReader = await command.ExecuteReaderAsync();

                while (await sqlReader.ReadAsync())
                {
                    //Заполнение списка данными
                    listBox1.Items.Add(Convert.ToString(sqlReader["Id"]) + "  " + Convert.ToString(sqlReader["Title"]) + "   " +"Автор: " + Convert.ToString(sqlReader["Author"]) + "   " +"Адресат: " +Convert.ToString(sqlReader["Adresat"]) + "   " + Convert.ToString(sqlReader["CreateDate"]));
                }
            }
            catch (Exception ex)
            {
                //В случае ошибки появится окно с ее текстом
                MessageBox.Show(ex.Message.ToString(), ex.Source.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (sqlReader != null)
                    sqlReader.Close();


            }
        }

        private async void button2_Click(object sender, EventArgs e)        //Изменение
        {
            //Проверка на то, что строка выбрана
            if (listBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Не выбрана задача для редактирования!");
                return;
            }
            else
            {
                //Сообщение об незаполненных полях (изначально невидима)
                if (label10.Visible)
                    label10.Visible = false;

                //Проверки на пустые поля
                if (!string.IsNullOrEmpty(textBox1.Text) && !string.IsNullOrWhiteSpace(textBox1.Text) &&
                    !string.IsNullOrEmpty(textBox2.Text) && !string.IsNullOrWhiteSpace(textBox2.Text))
                {

                    string n = listBox1.SelectedItem.ToString();   //получаем строку со всей информацией
                    string n1 = Regex.Replace(n, @"\D+\d+", "");  //Регулярное выражение
                    int ID = Convert.ToInt32(n1);
                    label4.Text = n1;
                    SqlCommand command = new SqlCommand("UPDATE [Notes] SET [Title]=@Title, [Adresat]=@Adresat WHERE [Id]=@ID ", sqlConnection);        //Формирование запроса к базе данных
                                                                                                                                                        //Заполнение данными из текстбоксов
                    command.Parameters.AddWithValue("ID", label4.Text);
                    command.Parameters.AddWithValue("Title", textBox1.Text);
                    command.Parameters.AddWithValue("Adresat", textBox2.Text);

                    await command.ExecuteNonQueryAsync();
                    //Обновление списка
                    button5_Click(sender, e);
                }

                else
                {
                    //Сообщение в случает незаполненных полей (красным цветом)
                    label10.Visible = true;
                    label10.Text = "Не все поля заполнены!";
                }
            }
        }

        private async void button3_Click(object sender, EventArgs e)        //Удаление
        {
            //Вариант с индексом строки (альтернатива используемому)
            // int nom = listBox1.SelectedIndex;
            // long Id = ((Note)listBox1.Items[nom]).Id;


            // рабочий вариант:
            //Проверка на то, что строка выбрана
            if (listBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Не выбрана задача для удаления!");
                return;
            }
            else
            {
                string n = listBox1.SelectedItem.ToString();   //получаем строку со всей информацией
            string n1 = Regex.Replace(n, @"\D+\d+", "");  //Регулярное выражение
            int ID = Convert.ToInt32(n1);
            label4.Text = n1;


           
            

            SqlCommand command = new SqlCommand("DELETE FROM [Notes] WHERE [Id]=@ID", sqlConnection);
            command.Parameters.AddWithValue("ID", label4.Text);

            await command.ExecuteNonQueryAsync();
            


            button5_Click(sender, e);
                }
        }

        private void tabPage2_Click(object sender, EventArgs e)


        {

        }



        //Фильтр по адресату
        private async void button4_Click(object sender, EventArgs e)        //Поиск по адресату

        {
            
            listBox2.Items.Clear();     //Очистка списка

            SqlDataReader sqlReader = null;

            SqlCommand command = new SqlCommand("SELECT * FROM [Notes] WHERE [Adresat]=@Adresat", sqlConnection);       //Формирование запроса к базе данных

            command.Parameters.AddWithValue("Adresat", textBox3.Text);      //Заполнение данными из текстбокса

            try
            {
                sqlReader = await command.ExecuteReaderAsync();

                while (await sqlReader.ReadAsync())
                {
                    //Заполнение списка результатами
                    listBox2.Items.Add(Convert.ToString(sqlReader["Id"]) + "  " + Convert.ToString(sqlReader["Title"]) + "   " + Convert.ToString(sqlReader["Author"]) + "   " + Convert.ToString(sqlReader["Adresat"]) + "   " + Convert.ToString(sqlReader["CreateDate"]));
                }
            }
            catch (Exception ex)
            {
                //В случае ошибки появится окно с ее текстом
                MessageBox.Show(ex.Message.ToString(), ex.Source.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                //Закрытие подключения
                if (sqlReader != null)
                    sqlReader.Close();


            }
        }
    }
}

