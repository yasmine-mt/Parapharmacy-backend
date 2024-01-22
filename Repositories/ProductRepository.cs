using backend.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace backend.Repositories
{
    public class ProductRepository : IRepository<Product>
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public ProductRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("UserAppCon");
        }

     

        public IEnumerable<Product> GetAll()
        {
            string query = @"SELECT ProductID, Name, Brand, Description, ImageURL, Category FROM dbo.Products";

            DataTable table = new DataTable();
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(_connectionString))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            var products = new List<Product>();
            foreach (DataRow row in table.Rows)
            {
                products.Add(new Product(
                    (int)row["ProductID"],
                    row["Name"].ToString(),
                    row["Brand"].ToString(),
                    row["Description"].ToString(),
                    row["ImageURL"].ToString(),
                    row["Category"].ToString()
                ));
            }
            return products;
        }

        public Product GetById(int productId)
        {
            string query = @"SELECT ProductID, Name, Brand, Description, ImageURL, Category FROM dbo.Products WHERE ProductID = @ProductID";

            Product product = null;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ProductID", productId);

                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        product = new Product(
                            reader.GetInt32(0),
                            reader.GetString(1),
                            reader.GetString(2),
                            reader.GetString(3),
                            reader.GetString(4),
                            reader.GetString(5)
                        );
                    }

                    reader.Close();
                }

                connection.Close();
            }

            return product;
        }

        public bool Add(Product product)
        {
            string query = @"INSERT INTO dbo.Products 
                     (ProductID, Name, Brand, Description, ImageURL, Category) 
                     VALUES (@ProductID, @Name, @Brand, @Description, @ImageURL, @Category)";

            try
            {
                using (SqlConnection myCon = new SqlConnection(_connectionString))
                {
                    myCon.Open();
                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        // Assuming ProductID is an integer, adjust the type accordingly
                        myCommand.Parameters.AddWithValue("@ProductID", product.ProductID);
                        myCommand.Parameters.AddWithValue("@Name", product.Name);
                        myCommand.Parameters.AddWithValue("@Brand", product.Brand);
                        myCommand.Parameters.AddWithValue("@Description", product.Description);
                        myCommand.Parameters.AddWithValue("@ImageURL", product.ImageURL);
                        myCommand.Parameters.AddWithValue("@Category", product.Category);
                        myCommand.ExecuteNonQuery();
                        myCon.Close();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                Console.WriteLine($"Exception: {ex.Message}");
                return false;
            }
        }

        public bool Update(Product product)
        {
            string query = @"UPDATE dbo.[Products] 
                             SET Name = @Name,
                             Brand = @Brand,
                             Description = @Description,
                             ImageURL = @ImageURL,
                             Category = @Category
                             WHERE ProductID = @ProductID";

            using (SqlConnection myCon = new SqlConnection(_connectionString))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@ProductID", product.ProductID);
                    myCommand.Parameters.AddWithValue("@Name", product.Name);
                    myCommand.Parameters.AddWithValue("@Brand", product.Brand);
                    myCommand.Parameters.AddWithValue("@Description", product.Description);
                    myCommand.Parameters.AddWithValue("@ImageURL", product.ImageURL);
                    myCommand.Parameters.AddWithValue("@Category", product.Category);

                    int rowsAffected = myCommand.ExecuteNonQuery();
                    myCon.Close();

                    return rowsAffected > 0;
                }
            }
        }

        public bool Delete(int productId)
        {
            string query = @"DELETE FROM dbo.Products 
                             WHERE ProductID = @ProductID";

            using (SqlConnection myCon = new SqlConnection(_connectionString))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@ProductID", productId);
                    int rowsAffected = myCommand.ExecuteNonQuery();
                    myCon.Close();

                    return rowsAffected > 0;
                }
            }
        }


       
    }
}







