using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;
using System.Configuration;

namespace CafeShopManagementSystem
{
    class AdminAddProductsData
    {
        public int ID { get; set; } // id
        public string ProductID { get; set; } // prod_id
        public string ProductName { get; set; } // prod_name
        public string Type { get; set; } // prod_type
        public int Stock { get; set; } // prod_stock
        public decimal Price { get; set; } // prod_price
        public string Status { get; set; } // prod_status
        public string Image { get; set; } // prod_image
        public string DateInsert { get; set; } // date_insert
        public string DateUpdate { get; set; } // date_update

        static string conn = ConfigurationManager.ConnectionStrings["myDatabaseConnection"].ConnectionString;
        NpgsqlConnection connect = new NpgsqlConnection(conn);

        public List<AdminAddProductsData> productsListData()
        {
            List<AdminAddProductsData> listData = new List<AdminAddProductsData>();

            if (connect.State == ConnectionState.Closed)
            {
                try
                {
                    connect.Open();

                    string selectData = "SELECT * FROM products WHERE date_delete IS NULL";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(selectData, connect))
                    {
                        NpgsqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            AdminAddProductsData apd = new AdminAddProductsData
                            {
                                ID = Convert.ToInt32(reader["id"]),
                                ProductID = reader["prod_id"].ToString(),
                                ProductName = reader["prod_name"].ToString(),
                                Type = reader["prod_type"].ToString(),
                                Stock = Convert.ToInt32(reader["prod_stock"]),
                                Price = Convert.ToDecimal(reader["prod_price"]),
                                Status = reader["prod_status"].ToString(),
                                Image = reader["prod_image"].ToString(),
                                DateInsert = Convert.ToDateTime(reader["date_insert"]).ToString("yyyy-MM-dd HH:mm:ss"),
                                DateUpdate = reader["date_update"] == DBNull.Value ? null :
                                             Convert.ToDateTime(reader["date_update"]).ToString("yyyy-MM-dd HH:mm:ss")
                            };

                            listData.Add(apd);
                        }

                        reader.Close();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed connection: " + ex.Message);
                }
                finally
                {
                    connect.Close();
                }
            }

            return listData;
        }

        // Получение только доступных продуктов
        public List<AdminAddProductsData> availableProductsData()
        {
            List<AdminAddProductsData> listData = new List<AdminAddProductsData>();

            if (connect.State == ConnectionState.Closed)
            {
                try
                {
                    connect.Open();

                    string selectData = "SELECT * FROM products WHERE prod_status = @status AND date_delete IS NULL";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(selectData, connect))
                    {
                        cmd.Parameters.AddWithValue("@status", "Available");

                        NpgsqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            AdminAddProductsData apd = new AdminAddProductsData
                            {
                                ID = Convert.ToInt32(reader["id"]),
                                ProductID = reader["prod_id"].ToString(),
                                ProductName = reader["prod_name"].ToString(),
                                Type = reader["prod_type"].ToString(),
                                Stock = Convert.ToInt32(reader["prod_stock"]),
                                Price = Convert.ToDecimal(reader["prod_price"]),
                                Status = reader["prod_status"].ToString(),
                                Image = reader["prod_image"].ToString()
                            };

                            listData.Add(apd);
                        }

                        reader.Close();
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
