using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace InventoryManagementSystem
{
    public partial class Form1 : Form
    {
        private const string ConnectionString = "Data Source=inventory.db;Version=3;";

        public Form1()
        {
            InitializeComponent();

            
            ApplyTheme();

            try
            {
                CreateTable();
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Initialization error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ApplyTheme()
        {
            // Palette
            var background = Color.FromArgb(250, 250, 250);
            var primary = Color.FromArgb(33, 150, 243); // blue
            var text = Color.FromArgb(33, 33, 33);

            // Form
            this.BackColor = background;
            this.ForeColor = text;
            this.Font = new Font("Segoe UI", 9F, FontStyle.Regular);

            
            foreach (var btn in this.Controls.OfType<Button>())
            {
                btn.FlatStyle = FlatStyle.Flat;
                btn.BackColor = primary;
                btn.ForeColor = Color.White;
                btn.FlatAppearance.BorderSize = 0;
                btn.Padding = new Padding(6, 3, 6, 3);
                btn.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            }

            foreach (var tb in this.Controls.OfType<TextBox>())
            {
                tb.BackColor = Color.White;
                tb.ForeColor = text;
                tb.BorderStyle = BorderStyle.FixedSingle;
                tb.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            }

            foreach (var lbl in this.Controls.OfType<Label>())
            {
                lbl.ForeColor = text;
                lbl.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            }

            // DataGridView styling (if present)
            if (this.Controls.Contains(dataGridView1) && dataGridView1 != null)
            {
                dataGridView1.EnableHeadersVisualStyles = false;
                dataGridView1.BackgroundColor = Color.White;
                dataGridView1.BorderStyle = BorderStyle.None;
                dataGridView1.GridColor = Color.FromArgb(230, 230, 230);

                dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = primary;
                dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                dataGridView1.RowTemplate.Height = 28;
                dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
                dataGridView1.DefaultCellStyle.SelectionBackColor = primary;
                dataGridView1.DefaultCellStyle.SelectionForeColor = Color.White;
                dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            }
        }

        private void CreateTable()
        {
            const string query = "CREATE TABLE IF NOT EXISTS Products (" +
                                 "ProductID INTEGER PRIMARY KEY AUTOINCREMENT," +
                                 "ProductName TEXT," +
                                 "Quantity INTEGER," +
                                 "Price REAL)";

            try
            {
                using (var con = new SQLiteConnection(ConnectionString))
                {
                    con.Open();
                    using (var cmd = new SQLiteCommand(query, con))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error creating table: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadData()
        {
            try
            {
                using (var con = new SQLiteConnection(ConnectionString))
                using (var da = new SQLiteDataAdapter("SELECT * FROM Products", con))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    dataGridView1.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtQuantity.Text, out var qty))
            {
                MessageBox.Show("Quantity must be an integer.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!double.TryParse(txtPrice.Text, out var price))
            {
                MessageBox.Show("Price must be a number.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                const string query = "UPDATE Products SET Quantity=@qty, Price=@price WHERE ProductName=@name";

                using (var con = new SQLiteConnection(ConnectionString))
                {
                    con.Open();
                    using (var cmd = new SQLiteCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@name", txtName.Text);
                        cmd.Parameters.AddWithValue("@qty", qty);
                        cmd.Parameters.AddWithValue("@price", price);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Product Updated", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating product: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Please enter a product name.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txtQuantity.Text, out var qty))
            {
                MessageBox.Show("Quantity must be an integer.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!double.TryParse(txtPrice.Text, out var price))
            {
                MessageBox.Show("Price must be a number.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                const string query = "INSERT INTO Products(ProductName,Quantity,Price) VALUES(@name,@qty,@price)";

                using (var con = new SQLiteConnection(ConnectionString))
                {
                    con.Open();
                    using (var cmd = new SQLiteCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@name", txtName.Text);
                        cmd.Parameters.AddWithValue("@qty", qty);
                        cmd.Parameters.AddWithValue("@price", price);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Product Added", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding product: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Please enter a product name to delete.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                const string query = "DELETE FROM Products WHERE ProductName=@name";

                using (var con = new SQLiteConnection(ConnectionString))
                {
                    con.Open();
                    using (var cmd = new SQLiteCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@name", txtName.Text);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Product Deleted", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting product: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}