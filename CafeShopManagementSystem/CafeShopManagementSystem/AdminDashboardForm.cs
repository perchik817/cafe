using System;
using System.Data;
using System.Windows.Forms;
using Npgsql;
using System.Configuration;

namespace CafeShopManagementSystem
{
    public partial class AdminDashboardForm : UserControl
    {
        static string conn = ConfigurationManager.ConnectionStrings["myDatabaseConnection"].ConnectionString;
        NpgsqlConnection connect = new NpgsqlConnection(conn);

        public AdminDashboardForm()
        {
            InitializeComponent();

            displayTotalCashier();
            displayTotalCustomers();
            displayTodaysIncome();
            displayTotalIncome();
        }

        public void refreshData()
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)refreshData);
                return;
            }
            displayTotalCashier();
            displayTotalCustomers();
            displayTodaysIncome();
            displayTotalIncome();
        }

        public void displayTotalCashier()
        {
            if (connect.State == ConnectionState.Closed)
            {
                try
                {
                    connect.Open();

                    string selectData = "SELECT COUNT(id) FROM users WHERE role = @role AND status = @status";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(selectData, connect))
                    {
                        cmd.Parameters.AddWithValue("@role", "Cashier");
                        cmd.Parameters.AddWithValue("@status", "Active");

                        NpgsqlDataReader reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            object value = reader[0];
                            if (value != DBNull.Value)
                            {
                                int count = Convert.ToInt32(value);
                                dashboard_TC.Text = count.ToString();
                            }
                            else
                            {
                                dashboard_TC.Text = "0";
                            }
                        }

                        reader.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed connection: " + ex, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    connect.Close();
                }
            }
        }

        public void displayTotalCustomers()
        {
            if (connect.State == ConnectionState.Closed)
            {
                try
                {
                    connect.Open();

                    string selectData = "SELECT COUNT(customer_id) FROM customers";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(selectData, connect))
                    {
                        NpgsqlDataReader reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            object value = reader[0];
                            if (value != DBNull.Value)
                            {
                                int count = Convert.ToInt32(value);
                                dashboard_TCust.Text = count.ToString();
                            }
                            else
                            {
                                dashboard_TCust.Text = "0";
                            }
                        }

                        reader.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed connection: " + ex, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    connect.Close();
                }
            }
        }

        public void displayTodaysIncome()
        {
            if (connect.State == ConnectionState.Closed)
            {
                try
                {
                    connect.Open();

                    string selectData = "SELECT SUM(total_price) FROM customers WHERE DATE = @date";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(selectData, connect))
                    {
                        DateTime today = DateTime.Today;
                        cmd.Parameters.AddWithValue("@date", today.Date);

                        NpgsqlDataReader reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            object value = reader[0];
                            if (value != DBNull.Value)
                            {
                                decimal total = Convert.ToDecimal(value);
                                dashboard_TI.Text = "$" + total.ToString("0.00");
                            }
                            else
                            {
                                dashboard_TI.Text = "$0.00";
                            }
                        }

                        reader.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed connection: " + ex, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    connect.Close();
                }
            }
        }

        public void displayTotalIncome()
        {
            if (connect.State == ConnectionState.Closed)
            {
                try
                {
                    connect.Open();

                    string selectData = "SELECT SUM(total_price) FROM customers";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(selectData, connect))
                    {
                        NpgsqlDataReader reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            object value = reader[0];
                            if (value != DBNull.Value)
                            {
                                decimal total = Convert.ToDecimal(value);
                                dashboard_TIn.Text = "$" + total.ToString("0.00");
                            }
                            else
                            {
                                dashboard_TIn.Text = "$0.00";
                            }
                        }

                        reader.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed connection: " + ex, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    connect.Close();
                }
            }
        }
    }
}