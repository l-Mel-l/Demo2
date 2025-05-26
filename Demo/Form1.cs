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
using System.Xml.Linq;

namespace Demo
{
    public partial class Form1 : Form
    {

        public string Connection = @"Data Source=.;Initial Catalog = Demo; Integrated Security = True";
        public Form1()
        {
            InitializeComponent();
            LoadPanels();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        public void LoadPanels()
        {
            flowLayoutPanel.Controls.Clear();

            string sql = @"
                Select [Тип_партнера],[Наименование_партнера],[Директор],[Телефон_партнера],[Электронная_почта_партнера],
                [Юридический_адрес_партнера],[Рейтинг],s.TotalSell from Partners_import as p
                LEFT JOIN (Select [Наименование_партнера] as Name, SUM([Количество_продукции]) as TotalSell
                FROM Partner_products_import GROUP BY [Наименование_партнера]) as s ON p.[Наименование_партнера] = s.Name;";

            using (var conn = new SqlConnection(Connection))
            using (var cmd = new SqlCommand(sql, conn))
            {
                conn.Open();
                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {   
                        string type = rdr.GetString(0);
                        string name = rdr.GetString(1);
                        string director = rdr.GetString(2);
                        string number = rdr.GetString(3);
                        string email = rdr.GetString(4);
                        string adress = rdr.GetString(5);
                        int rating = rdr.GetByte(6);
                        int sell = rdr.GetInt32(7);

                        int discount;
                        if (sell > 300000) discount = 15;
                        else if (sell > 50000) discount = 10;
                        else if (sell > 10000) discount = 5;
                        else discount = 0;

                        var panel = new Panel
                        {
                            Width = 740,
                            Height = 90,
                            BorderStyle = BorderStyle.FixedSingle,
                            Margin = new Padding(8),
                            Cursor = Cursors.Hand
                        };

                        panel.Controls.Add(new Label
                        {
                            Text = $"{type} | {name}",
                            Font = new Font("Segoe UI",12),
                            Location = new Point(10,10),
                            AutoSize = true,
                        });

                        panel.Controls.Add(new Label
                        {
                            Text = $"{director}",
                            Location = new Point(10, 30),
                            AutoSize = true,
                        });

                        panel.Controls.Add(new Label
                        {
                            Text = $"+7 {number}",
                            Location = new Point(10, 45),
                            AutoSize = true,
                        });

                        panel.Controls.Add(new Label
                        {
                            Text = $"Рейтинг: {rating}",
                            Location = new Point(10, 60),
                            AutoSize = true,
                        });

                        panel.Controls.Add(new Label
                        {
                            Text = $"{discount}%",
                            Location = new Point(650, 10),
                            Font = new Font("Segoe UI", 12),
                            AutoSize = true,
                        });


                        Action showDetails = () =>
                        {
                            flowLayoutPanel.Controls.Clear();

                            var pf = new PartnerForm(this,type,name,director,number,email,adress,rating);
                            pf.Show();
                            this.Hide();
                        };

                        panel.Click += (s, e) => showDetails();

                        foreach (Control ctl in panel.Controls)
                        {
                            ctl.Click += (s, e) => showDetails();
                        }

                        flowLayoutPanel.Controls.Add(panel);
                    }
                }
            }
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            flowLayoutPanel.Controls.Clear();
            var pf = new PartnerForm(this);
            pf.Show();
            this.Hide();
        }
    }
}
