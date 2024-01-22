using backend.Models;
using System.Data;
using System.Data.SqlClient;

namespace backend.Repositories
{
    public class OrderRepository : IListRepository<Order>
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public OrderRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("UserAppCon");
        }

        public IEnumerable<Order> GetAll()
        {
            string query = @"SELECT OrderID, OrderDateTime, TotalPrice, OrderStatus, UserID FROM dbo.[Orders]";

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

            var orders = new List<Order>();
            foreach (DataRow row in table.Rows)
            {
                OrderStatus orderStatus = (OrderStatus)Enum.Parse(typeof(OrderStatus), row["OrderStatus"].ToString());
                orders.Add(new Order(
                    (int)row["OrderID"],
                    (DateTime)row["OrderDateTime"],
                    (decimal)row["TotalPrice"],
                    orderStatus,
                    (int)row["UserID"]
                ));
            }
            return orders;
        }

        public List<Order> GetById(int userId)
        {
            string query = @"SELECT OrderID, OrderDateTime, TotalPrice, OrderStatus, UserID FROM dbo.[Orders] WHERE UserID = @UserID";

            List<Order> orders = new List<Order>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", userId);

                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        Order order = new Order(
                            reader.GetInt32(0),
                            reader.GetDateTime(1),
                            reader.GetDecimal(2),
                            (OrderStatus)reader.GetInt32(3),
                            reader.GetInt32(4)
                        );

                        orders.Add(order);
                    }

                    reader.Close();
                }

                connection.Close();
            }

            return orders;
        }


        public bool Add(Order order)
        {
            string query = @"INSERT INTO dbo.Orders 
         (OrderDateTime, TotalPrice, OrderStatus, UserID) 
         VALUES (@OrderDateTime, @TotalPrice, @OrderStatus, @UserID)";

            try
            {
                using (SqlConnection myCon = new SqlConnection(_connectionString))
                {
                    myCon.Open();

                    // Log order details before processing
                    Console.WriteLine($"Order details before processing: OrderID={order.OrderID}, DateTime={order.DateTime}, TotalPrice={order.TotalPrice}, Status={order.Status}, UserID={order.UserID}");

                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        order.DateTime = DateTime.Now;

                        myCommand.Parameters.AddWithValue("@OrderDateTime", order.DateTime);
                        myCommand.Parameters.AddWithValue("@TotalPrice", order.TotalPrice);
                        myCommand.Parameters.AddWithValue("@OrderStatus", order.Status); 
                        myCommand.Parameters.AddWithValue("@UserID", order.UserID);

                        myCommand.ExecuteNonQuery();

                        Console.WriteLine($"Order details after processing: OrderID={order.OrderID}, DateTime={order.DateTime}, TotalPrice={order.TotalPrice}, Status={order.Status}, UserID={order.UserID}");

                        myCon.Close();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during order processing: {ex.Message}");
                return false;
            }
        }



        public bool Update(Order order)
        {
            string query = @"UPDATE dbo.[Orders] 
                    SET OrderStatus = @OrderStatus,
                    UserID = @UserID
                    WHERE OrderID = @OrderID";

            using (SqlConnection myCon = new SqlConnection(_connectionString))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@OrderID", order.OrderID);

                    myCommand.Parameters.AddWithValue("@OrderStatus", (int)order.Status);

                    myCommand.Parameters.AddWithValue("@UserID", order.UserID);

                    int rowsAffected = myCommand.ExecuteNonQuery();
                    myCon.Close();

                    return rowsAffected > 0;
                }
            }
        

    }

    public bool Delete(int orderId)
        {
            string query = @"DELETE FROM dbo.[Orders] 
                     WHERE OrderID = @OrderID";

            using (SqlConnection myCon = new SqlConnection(_connectionString))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@OrderID", orderId);
                    int rowsAffected = myCommand.ExecuteNonQuery();
                    myCon.Close();

                    return rowsAffected > 0;
                }
            }
        }

        public Order GetObjById(int id)
        {
            string query = @"SELECT OrderID, OrderDateTime, TotalPrice, OrderStatus, UserID
                    FROM dbo.[Orders]
                    WHERE OrderID = @OrderID";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@OrderID", id);

                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read()) { 
                        Order order = new Order(
                             reader.GetInt32(0),
                             reader.GetDateTime(1),
                             reader.GetDecimal(2),
                             (OrderStatus)reader.GetInt32(3),  
                             reader.GetInt32(4)
                         );


                    reader.Close();
                        connection.Close();

                        return order;
                    }
                }

                connection.Close();
            }

            return null; 
        }

    }
}
