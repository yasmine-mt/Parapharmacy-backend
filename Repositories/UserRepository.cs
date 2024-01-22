using backend.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace backend.Repositories
{
    public class UserRepository : IRepository<User>
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("UserAppCon");
        }

        public IEnumerable<User> GetAll()
        {
            string query = @"SELECT UserID, FirstName, LastName, Email, Phone, Password, Address, City, PostalCode FROM dbo.[Users]";

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

            var users = new List<User>();
            foreach (DataRow row in table.Rows)
            {
                users.Add(new User(
                    (int)row["UserID"],
                    row["FirstName"].ToString(),
                    row["LastName"].ToString(),
                    row["Email"].ToString(),
                    row["Phone"].ToString(),
                    row["Password"].ToString(),
                    row["Address"] != DBNull.Value ? row["Address"].ToString() : null,
                    row["City"] != DBNull.Value ? row["City"].ToString() : null,
                    row["PostalCode"] != DBNull.Value ? row["PostalCode"].ToString() : null
                ));
            }
            return users;
        }

        public User GetById(int userId)
        {
            string query = @"SELECT UserID, FirstName, LastName, Email, Phone, Password, Address, City, PostalCode FROM dbo.[Users] WHERE UserID = @UserID";

            User user = null;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", userId);

                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        user = new User(
                         reader.GetInt32(0),
                         reader.GetString(1),
                         reader.GetString(2),
                         reader.GetString(3),
                         reader.GetString(4),
                         reader.GetString(5),
                         reader["Address"] != DBNull.Value ? reader.GetString(6) : null,
                         reader["City"] != DBNull.Value ? reader.GetString(7) : null,
                         reader["PostalCode"] != DBNull.Value ? reader.GetString(8) : null
                     );
                    }

                    reader.Close();
                }

                connection.Close();
            }

            return user;
        }

        public bool Add(User user)
        {
            string query = @"INSERT INTO dbo.[Users] 
                    (FirstName, LastName, Email, Phone, Password, Address, City, PostalCode) 
                    VALUES (@FirstName, @LastName, @Email, @Phone, @Password, @Address, @City, @PostalCode)";

            try
            {
                using (SqlConnection myCon = new SqlConnection(_connectionString))
                {
                    myCon.Open();
                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        myCommand.Parameters.Add(new SqlParameter("@FirstName", SqlDbType.VarChar, 50)).Value = user.FirstName;
                        myCommand.Parameters.Add(new SqlParameter("@LastName", SqlDbType.VarChar, 50)).Value = user.LastName;
                        myCommand.Parameters.Add(new SqlParameter("@Email", SqlDbType.VarChar, 100)).Value = user.Email;
                        myCommand.Parameters.Add(new SqlParameter("@Phone", SqlDbType.VarChar, 20)).Value = user.Phone;
                        myCommand.Parameters.Add(new SqlParameter("@Password", SqlDbType.VarChar, 100)).Value = user.Password;
                        myCommand.Parameters.Add(new SqlParameter("@Address", SqlDbType.VarChar, 255)).Value = user.Address;
                        myCommand.Parameters.Add(new SqlParameter("@City", SqlDbType.VarChar, 50)).Value = user.City;
                        myCommand.Parameters.Add(new SqlParameter("@PostalCode", SqlDbType.VarChar, 20)).Value = user.PostalCode;

                        myCommand.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"SQL Error during user insertion: {ex.Message}");
                Console.WriteLine($"SQL Error Number: {ex.Number}");
                Console.WriteLine($"SQL Error Procedure: {ex.Procedure}");
                Console.WriteLine($"SQL Error Line Number: {ex.LineNumber}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during user insertion: {ex.Message}");
                return false;
            }
        }


        public bool Update(User user)
        {
            string query = @"UPDATE dbo.[Users] 
                             SET FirstName = @FirstName,
                             LastName = @LastName,
                             Email = @Email,
                             Phone = @Phone,
                             Password = @Password,
                             Address = @Address,
                             City = @City,
                             PostalCode = @PostalCode
                             WHERE UserID = @UserID";

            using (SqlConnection myCon = new SqlConnection(_connectionString))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@UserID", user.UserID);
                    myCommand.Parameters.AddWithValue("@FirstName", user.FirstName);
                    myCommand.Parameters.AddWithValue("@LastName", user.LastName);
                    myCommand.Parameters.AddWithValue("@Email", user.Email);
                    myCommand.Parameters.AddWithValue("@Phone", user.Phone);
                    myCommand.Parameters.AddWithValue("@Password", user.Password);
                    myCommand.Parameters.AddWithValue("@Address", user.Address);
                    myCommand.Parameters.AddWithValue("@City", user.City);
                    myCommand.Parameters.AddWithValue("@PostalCode", user.PostalCode);

                    int rowsAffected = myCommand.ExecuteNonQuery();
                    myCon.Close();

                    return rowsAffected > 0; 

                }
            }
        }

        public bool Delete(int userId)
        {
            string query = @"DELETE FROM dbo.[Users] 
                             WHERE UserID = @UserID";

            using (SqlConnection myCon = new SqlConnection(_connectionString))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@UserID", userId);
                    int rowsAffected = myCommand.ExecuteNonQuery();
                    myCon.Close();

                    return rowsAffected > 0;
                }
            }
        }
      
    }
}

