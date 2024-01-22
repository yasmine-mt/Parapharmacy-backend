using backend.Models;
using System.Data;
using System.Data.SqlClient;

namespace backend.Repositories
{
    public class ProductSizeRepository : IListRepository<ProductSize>
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public ProductSizeRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("UserAppCon");
        }

        public IEnumerable<ProductSize> GetAll()
        {
            string query = @"SELECT ProductSizeID, Size, Price, Quantity, ProductID FROM dbo.PRODUCTSIZES";

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

            var productSizes = new List<ProductSize>();
            foreach (DataRow row in table.Rows)
            {
                productSizes.Add(new ProductSize(
                    (int)row["ProductSizeID"],
                    (int)row["Size"],
                    (decimal)row["Price"],
                    (int)row["Quantity"],
                    (int)row["ProductID"]
                ));
            }
            return productSizes;
        }

        public ProductSize GetObjById(int productSizeId)
        {
            string query = @"SELECT * FROM dbo.PRODUCTSIZES WHERE ProductSizeID = @ProductSizeID";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ProductSizeID", productSizeId);

                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        ProductSize productSize = new ProductSize(
                            reader.GetInt32(0),
                            reader.GetInt32(1),
                            reader.GetDecimal(2),
                            reader.GetInt32(3),
                            reader.GetInt32(4)
                        );

                        reader.Close();
                        connection.Close();

                        return productSize;
                    }
                    else
                    {
                        reader.Close();
                        connection.Close();

                        return null; 
                    }
                }
            }
        }

        public List<ProductSize> GetById(int productId)
        {
            string query = @"SELECT * FROM dbo.PRODUCTSIZES WHERE ProductID = @ProductID";

            List<ProductSize> productSizes = new List<ProductSize>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ProductID", productId);

                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        ProductSize productSize = new ProductSize(
                            reader.GetInt32(0),
                            reader.GetInt32(1),
                            reader.GetDecimal(2),
                            reader.GetInt32(3),
                            reader.GetInt32(4)
                        );

                        productSizes.Add(productSize);
                    }

                    reader.Close();
                }

                connection.Close();
            }

            return productSizes;
        }


        public bool Add(ProductSize productSize)
        {
            string query = @"INSERT INTO dbo.ProductSizes 
                     (ProductSizeID, Size, Price, Quantity, ProductID) 
                     VALUES (@ProductSizeID, @Size, @Price, @Quantity, @ProductID)";

            try
            {
                using (SqlConnection myCon = new SqlConnection(_connectionString))
                {
                    myCon.Open();
                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        myCommand.Parameters.AddWithValue("@ProductSizeID", productSize.ProductSizeID);
                        myCommand.Parameters.AddWithValue("@Size", productSize.Size);
                        myCommand.Parameters.AddWithValue("@Price", productSize.Price);
                        myCommand.Parameters.AddWithValue("@Quantity", productSize.Quantity);
                        myCommand.Parameters.AddWithValue("@ProductID", productSize.ProductID);
                        myCommand.ExecuteNonQuery();
                        myCon.Close();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return false;
            }
        }


        public bool Update(ProductSize productSize)
        {
            string query = @"UPDATE dbo.PRODUCTSIZES 
                            SET Price = @Price, Quantity = @Quantity
                            WHERE ProductSizeID = @ProductSizeID;
                            ";

            using (SqlConnection myCon = new SqlConnection(_connectionString))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@ProductSizeID", productSize.ProductSizeID);
                    myCommand.Parameters.AddWithValue("@Price", productSize.Price);
                    myCommand.Parameters.AddWithValue("@Quantity", productSize.Quantity);

                    int rowsAffected = myCommand.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        public bool Delete(int productSizeID)
        {
            string query = @"DELETE FROM dbo.PRODUCTSIZES WHERE ProductSizeID = @ProductSizeID";

            using (SqlConnection myCon = new SqlConnection(_connectionString))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@ProductSizeID", productSizeID);

                    int rowsAffected = myCommand.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }
    }
}

