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

namespace Demo
{
    public partial class PartnerForm : Form
    {
        private Form1 Parent;
        private readonly bool Isnew;
        private string OriginalName;

        public PartnerForm(Form1 parent)
        {
            InitializeComponent();
            Parent = parent;
            Isnew = true;
            OriginalName = null;
        }

        public PartnerForm(Form1 parent, string type, string name, string director, string number, string email, string adress, int rating): this(parent)
        {
            Isnew = false;
            OriginalName = name;
            setData(type, name, director, number, email, adress, rating);
        }

        public void setData(string type,string name,string director, string number, string email, string adress, int rating)
        {
            comboBoxType.Text = type;
            textBoxName.Text = name;
            textBoxDirector.Text = director;
            textBoxNumber.Text = number;
            textBoxEmail.Text = email;
            textBoxAdress.Text = adress;
            textBoxRating.Text = rating.ToString();
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            Parent.LoadPanels();
            Parent.Show();
            this.Close();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            string newType = comboBoxType.Text.Trim();
            string newName = textBoxName.Text.Trim();
            string newNumber = textBoxNumber.Text.Trim();
            string newDirector = textBoxDirector.Text.Trim();
            string newEmail = textBoxEmail.Text.Trim();
            string newAdress = textBoxAdress.Text.Trim();
            string newRating = textBoxRating.Text.Trim();

            if (newType == "" || newName == "" || newDirector == "" ||
                newNumber == "" || newEmail == "" || newAdress == "" || newRating == "")
            {
                MessageBox.Show("Заполните все поля", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            };

            var validTypes = new[] { "ООО", "ЗАО", "ПАО", "ОАО" };
            if (!validTypes.Contains(newType))
            {
                MessageBox.Show("Тип партнёра должен быть одним из: ООО, ЗАО, ПАО, ОАО", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(newRating, out int intRating) || intRating < 0 || intRating > 10)
            {
                MessageBox.Show("Рейтинг партнёра должен быть целым числом от 0 до 10", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try {
                using (var conn = new SqlConnection(Parent.Connection))
                using (var cmd = conn.CreateCommand())
                {
                    if (Isnew)
                    {
                        cmd.CommandText = @"
                        INSERT INTO Partners_import ([Директор], [Тип_партнера],
                        [Наименование_партнера], [Электронная_почта_партнера],
                        [Телефон_партнера], [Юридический_адрес_партнера], [Рейтинг])
                        VALUES (@director, @type, @name, @email, @number,@adress, @rating)";
                    }
                    else
                    {
                        cmd.CommandText = @"
                        UPDATE Partners_import Set [Директор] = @director, [Тип_партнера] = @type,
                        [Наименование_партнера] = @OriginalName, [Электронная_почта_партнера] = @email,
                        [Телефон_партнера] = @number, [Юридический_адрес_партнера] = @adress, [Рейтинг] = @rating
                        Where [Наименование_партнера] = @OriginalName";
                        cmd.Parameters.AddWithValue("@OriginalName", OriginalName);
                    }
                    cmd.Parameters.AddWithValue("@name", newName);
                    cmd.Parameters.AddWithValue("@director", newDirector);
                    cmd.Parameters.AddWithValue("@type", newType);
                    cmd.Parameters.AddWithValue("@email", newEmail);
                    cmd.Parameters.AddWithValue("@number", newNumber);
                    cmd.Parameters.AddWithValue("@adress", newAdress);
                    cmd.Parameters.AddWithValue("@rating", newRating);

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Данные сохранены", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Parent.LoadPanels();
                    Parent.Show();
                    this.Close();
                }
            }catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении в базу данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error) ;
                return;
            }
        }
    }
}
