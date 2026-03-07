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
            // Reserved for runtime initialization if needed.
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