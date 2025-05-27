using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;
using System.Configuration;

namespace CafeShopManagementSystem
{
    class CashierOrderFormProdData
    {
        public int ID { set; get; }                 // 0
        public string ProductID { set; get; }       // 1
        public string ProductName { set; get; }     // 2
        public string Type { set; get; }            // 3
        public int Stock { set; get; }              // 4
        public string Price { set; get; }           // 5
        public string Status { set; get; }          // 6

        static string connString = ConfigurationManager.ConnectionStrings["myDatabaseConnection"].ConnectionString;
        NpgsqlConnection connect = new NpgsqlConnection(connString);

        public List<CashierOrderFormProdData> availableProductsData()
        {
            List<CashierOrderFormProdData> listData = new List<CashierOrderFormProdData>();

            if (connect.State == ConnectionState.Closed)
            {
                try
                {
                    connect.Open();

                    string selectData = "SELECT * FROM products WHERE prod_status = @stats AND date_delete IS NULL";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(selectData, connect))
                    {
                        cmd.Parameters.AddWithValue("@stats", "Available");

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                CashierOrderFormProdData apd = new CashierOrderFormProdData
                                {
                                    ID = Convert.ToInt32(reader["id"]),
                                    ProductID = reader["prod_id"].ToString(),
                                    ProductName = reader["prod_name"].ToString(),
                                    Type = reader["prod_type"].ToString(),
                                    Stock = Convert.ToInt32(reader["prod_stock"]),
                                    Price = reader["prod_price"].ToString(),
                                    Status = reader["prod_status"].ToString()
                                };

                                listData.Add(apd);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed Connection: " + ex.Message);
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
