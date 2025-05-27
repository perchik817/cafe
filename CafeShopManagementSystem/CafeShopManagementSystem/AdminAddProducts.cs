using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Configuration;
using Npgsql;

namespace CafeShopManagementSystem
{
    public partial class AdminAddProducts : UserControl
    {
        static string conn = ConfigurationManager.ConnectionStrings["myDatabaseConnection"].ConnectionString;
        NpgsqlConnection  connect = new NpgsqlConnection (conn);

        public AdminAddProducts()
        {
            InitializeComponent();

            displayData();
        }

        public void refreshData()
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)refreshData);
                return;
            }
            displayData();
        }

        public bool emptyFields()
        {
            if(adminAddProducts_id.Text == "" || adminAddProducts_name.Text == ""
                || adminAddProducts_type.SelectedIndex == -1 || adminAddProducts_stock.Text == ""
                || adminAddProducts_price.Text == "" || adminAddProducts_status.SelectedIndex == -1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void displayData()
        {
            AdminAddProductsData prodData = new AdminAddProductsData();
            List<AdminAddProductsData> listData = prodData.productsListData();

            DataGridView1.DataSource = listData;
        }

        private void adminAddProducts_addBtn_Click(object sender, EventArgs e)
        {
            if (emptyFields())
            {
                MessageBox.Show("All fields are required to be filled.", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if(connect.State == ConnectionState.Closed)
                {
                    try
                    {
                        connect.Open();

                        // CHECKING IF THE PRODUCT ID IS EXISTING ALREADY
                        string selectProdID = "SELECT * FROM products WHERE prod_id = @prodID";

                        using (NpgsqlCommand selectPID = new NpgsqlCommand(selectProdID, connect))
                        {
                            selectPID.Parameters.AddWithValue("@prodID", adminAddProducts_id.Text.Trim());

                            NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(selectPID);
                            DataTable table = new DataTable();
                            adapter.Fill(table);

                            if(table.Rows.Count >= 1)
                            {
                                MessageBox.Show("Product ID: " + adminAddProducts_id.Text.Trim() + " is taken already", "Error Message"
                                    , MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                string insertData = "INSERT INTO products (prod_id, prod_name, prod_type, " +
                                    "prod_stock, prod_price, prod_status, prod_image, date_insert) VALUES(@prodID, @prodName" +
                                    ", @prodType, @prodStock, @prodPrice, @prodStatus, @prodImage, @dateInsert)";

                                DateTime today = DateTime.Today;

                                string path = Path.Combine(@"C:\Users\User\Desktop\cafe\products\"
                                    + adminAddProducts_id.Text.Trim() + ".jpg");

                                string directoryPath = Path.GetDirectoryName(path);

                                if (!Directory.Exists(directoryPath))
                                {
                                    Directory.CreateDirectory(directoryPath);
                                }

                                File.Copy(adminAddProducts_imageView.ImageLocation, path, true);

                                using (NpgsqlCommand cmd = new NpgsqlCommand(insertData, connect))
                                {
                                    cmd.Parameters.AddWithValue("@prodID", adminAddProducts_id.Text.Trim());
                                    cmd.Parameters.AddWithValue("@prodName", adminAddProducts_name.Text.Trim());
                                    cmd.Parameters.AddWithValue("@prodType", adminAddProducts_type.Text.Trim());
                                    cmd.Parameters.AddWithValue("@prodStock", int.Parse(adminAddProducts_stock.Text.Trim()));
                                    cmd.Parameters.AddWithValue("@prodPrice", decimal.Parse(adminAddProducts_price.Text.Trim()));
                                    cmd.Parameters.AddWithValue("@prodStatus", adminAddProducts_status.Text.Trim());
                                    cmd.Parameters.AddWithValue("@prodImage", path);
                                    cmd.Parameters.AddWithValue("@dateInsert", today);

                                    cmd.ExecuteNonQuery();
                                    clearFields();

                                    MessageBox.Show("Added successfully!", "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    displayData();
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Failed connection: " + ex, "Error Message"
                                    , MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        connect.Close();
                    }
                }
                
            }
        }

        private void adminAddProducts_importBtn_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "Image Files (*.jpg; *.png)|*.jpg;*.png";
                string imagePath = "";

                if(dialog.ShowDialog() == DialogResult.OK)
                {
                    imagePath = dialog.FileName;
                    adminAddProducts_imageView.ImageLocation = imagePath;
                }

            }catch(Exception ex)
            {
                MessageBox.Show("Error: " + ex, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void clearFields()
        {
            adminAddProducts_id.Text = "";
            adminAddProducts_name.Text = "";
            adminAddProducts_type.SelectedIndex = -1;
            adminAddProducts_stock.Text = "";
            adminAddProducts_price.Text = "";
            adminAddProducts_status.SelectedIndex = -1;
            adminAddProducts_imageView.Image = null;

        }

        private void adminAddProducts_clearBtn_Click(object sender, EventArgs e)
        {
            clearFields();
        }

        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            if(e.RowIndex != -1)
            {
                DataGridViewRow row = DataGridView1.Rows[e.RowIndex];
                adminAddProducts_id.Text = row.Cells[1].Value.ToString();
                adminAddProducts_name.Text = row.Cells[2].Value.ToString();
                adminAddProducts_type.Text = row.Cells[3].Value.ToString();
                adminAddProducts_stock.Text = row.Cells[4].Value.ToString();
                adminAddProducts_price.Text = row.Cells[5].Value.ToString();
                adminAddProducts_status.Text = row.Cells[6].Value.ToString();

                string imagepath = row.Cells[7].Value.ToString();
                try
                {
                    if (imagepath != null)
                    {
                        adminAddProducts_imageView.Image = Image.FromFile(imagepath);
                    }
                    else
                    {
                        adminAddProducts_imageView.Image = null;
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Error Image: " + ex, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                
            }
        }

        /*private void adminAddProducts_updateBtn_Click(object sender, EventArgs e)
        {
            if (emptyFields())
            {
                MessageBox.Show("All fields are requried to be filled", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                DialogResult check = MessageBox.Show("Are you sure you want to Update Product ID: " + adminAddProducts_id.Text.Trim() + "?"
                    , "Confirmation Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if(check == DialogResult.Yes)
                {
                    if(connect.State != ConnectionState.Open)
                    {
                        try
                        {
                            connect.Open();

                            string updateData = "UPDATE products SET prod_name = @prodName" +
                                ", prod_type = @prodType, prod_stock = @prodStock, prod_price = @prodPrice, prod_status = @prodStatus" +
                                ", date_update = @dateUpdate WHERE prod_id = @prodID";
                            DateTime today = DateTime.Today;

                            using (NpgsqlCommand updateD = new NpgsqlCommand(updateData, connect))
                            {
                                updateD.Parameters.AddWithValue("@prodName", adminAddProducts_name.Text.Trim());
                                updateD.Parameters.AddWithValue("@prodType", adminAddProducts_type.Text.Trim());
                                updateD.Parameters.AddWithValue("@prodStock", adminAddProducts_stock.Text.Trim());
                                updateD.Parameters.AddWithValue("@prodPrice", adminAddProducts_price.Text.Trim());
                                updateD.Parameters.AddWithValue("@prodStatus", adminAddProducts_status.Text.Trim());
                                updateD.Parameters.AddWithValue("@dateUpdate", today);
                                updateD.Parameters.AddWithValue("@prodID", adminAddProducts_id.Text.Trim());

                                updateD.ExecuteNonQuery();
                                clearFields();

                                MessageBox.Show("Updated successfully!", "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                displayData();

                            }
                        }catch(Exception ex)
                        {
                            MessageBox.Show("Connection failed: " + ex, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        finally
                        {
                            connect.Close();
                        }
                    }
                }
            }
        }
*/

        private void adminAddProducts_updateBtn_Click(object sender, EventArgs e)
        {
            if (emptyFields())
            {
                MessageBox.Show("All fields are required.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!int.TryParse(adminAddProducts_stock.Text.Trim(), out int stock))
            {
                MessageBox.Show("Stock must be a valid integer.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show(
                $"Update product ID «{adminAddProducts_id.Text.Trim()}»?",
                "Confirm Update",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            string imagePath = null;
            if (!string.IsNullOrEmpty(adminAddProducts_imageView.ImageLocation))
            {
                string destDir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    "cafe", "products");
                Directory.CreateDirectory(destDir);
                imagePath = Path.Combine(destDir, $"{adminAddProducts_id.Text.Trim()}.jpg");
                File.Copy(adminAddProducts_imageView.ImageLocation, imagePath, true);
            }

            using (connect)
            {
                connect.Open();
                string sql = @"
            UPDATE products
               SET prod_name   = @prodName,
                   prod_type   = @prodType,
                   prod_stock  = @prodStock,
                   prod_price  = @prodPrice,
                   prod_status = @prodStatus,
                   date_update = @dateUpdate"
                    + (imagePath != null ? ", prod_image = @prodImage" : "")
                    + " WHERE id = @id";

                using (var cmd = new NpgsqlCommand(sql, connect))
                {
                    cmd.Parameters.AddWithValue("prodName", adminAddProducts_name.Text.Trim());
                    cmd.Parameters.AddWithValue("prodType", adminAddProducts_type.Text.Trim());
                    cmd.Parameters.AddWithValue("prodStock", stock); // Преобразовано в int
                    cmd.Parameters.AddWithValue("prodPrice", decimal.Parse(adminAddProducts_price.Text.Trim()));
                    cmd.Parameters.AddWithValue("prodStatus", adminAddProducts_status.Text.Trim());
                    cmd.Parameters.AddWithValue("dateUpdate", DateTime.Now);

                    if (imagePath != null)
                        cmd.Parameters.AddWithValue("prodImage", imagePath);

                    object selectedId = DataGridView1.CurrentRow?.Cells["id"].Value;
                    if (selectedId == null || !int.TryParse(selectedId.ToString(), out int productId))
                    {
                        MessageBox.Show("Invalid product ID selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    cmd.Parameters.AddWithValue("id", productId);
                    int affected = cmd.ExecuteNonQuery();

                    if (affected > 0)
                    {
                        MessageBox.Show("Product updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        clearFields();
                        displayData();
                    }
                    else
                    {
                        MessageBox.Show("No product found with given ID.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }
        private void adminAddProducts_deleteBtn_Click(object sender, EventArgs e)
        {
            if (emptyFields())
            {
                MessageBox.Show("All fields are requried to be filled", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                DialogResult check = MessageBox.Show("Are you sure you want to Delete Product ID: " + adminAddProducts_id.Text.Trim() + "?"
                    , "Confirmation Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (check == DialogResult.Yes)
                {
                    if (connect.State != ConnectionState.Open)
                    {
                        try
                        {
                            connect.Open();

                            string updateData = "UPDATE products SET date_delete = @dateDelete WHERE prod_id = @prodID";
                            DateTime today = DateTime.Today;

                            using (NpgsqlCommand updateD = new NpgsqlCommand(updateData, connect))
                            {

                                updateD.Parameters.AddWithValue("@dateDelete", today);
                                updateD.Parameters.AddWithValue("@prodID", adminAddProducts_id.Text.Trim());

                                updateD.ExecuteNonQuery();
                                clearFields();

                                MessageBox.Show("Removed successfully!", "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                displayData();

                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Connection failed: " + ex, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        finally
                        {
                            connect.Close();
                        }
                    }
                }
            }
        }

        private void DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
