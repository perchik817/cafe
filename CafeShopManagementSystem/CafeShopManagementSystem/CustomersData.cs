using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Npgsql;

namespace CafeShopManagementSystem
{
    class CustomersData
    {
        NpgsqlConnection  connect = new NpgsqlConnection (@"Host=localhost;Username=postgres;Password=postgres;Database=cafe;Timeout=30");
    
        public int CustomerID { set; get; }
        public string TotalPrice { set; get; }
        public string Amount { set; get; }
        public string Change { set; get; }
        public string Date { set; get; }

        public List<CustomersData> allCustomersData()
        {
            List<CustomersData> listData = new List<CustomersData>();

            if(connect.State == ConnectionState.Closed)
            {
                try
                {
                    connect.Open();

                    string selectData = "SELECT * FROM customers";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(selectData, connect))
                    {
                        NpgsqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            CustomersData cData = new CustomersData();

                            cData.CustomerID = (int)reader["customer_id"];
                            cData.TotalPrice = reader["total_price"].ToString();
                            cData.Amount = reader["amount"].ToString();
                            cData.Change = reader["change"].ToString();
                            cData.Date = reader["date"].ToString();

                            listData.Add(cData);
                        }
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Failed connection: " + ex);
                }
                finally
                {
                    connect.Close();
                }
            }

            return listData;
        }

    }
}
