using System.Data;
using System.Data.SqlClient;
using WebD_T.Models;

namespace WebD_T.DAL
{
    public class UserDAL
    {
        private readonly DbConnectionHelper _db;

        public UserDAL(DbConnectionHelper db)
        {
            _db = db;
        }

        public UserAccount? GetByUsername(string username)
        {
            _db.Open();
            UserAccount? user = null;

            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = _db.GetConnection();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"SELECT * FROM users WHERE username = @username";
                cmd.Parameters.AddWithValue("@username", username);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        user = new UserAccount
                        {
                            Id = (int)reader["id"],
                            Username = reader["username"].ToString() ?? "",
                            Email = reader["email"].ToString() ?? "",
                            Password = reader["password"].ToString() ?? "",
                            FullName = reader["full_name"] as string,
                            PhoneNumber = reader["phone_number"] as string,
                            Role = reader["role"].ToString() ?? "customer",
                            CreatedAt = (DateTime)reader["created_at"]
                        };
                    }
                }
            }

            _db.Close();
            return user;
        }

        public bool CreateUser(UserAccount user)
        {
            _db.Open();
            int rows = 0;

            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = _db.GetConnection();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"
                    INSERT INTO users (username, email, password, full_name, phone_number, role)
                    VALUES (@username, @email, @password, @full_name, @phone_number, @role)";

                cmd.Parameters.AddWithValue("@username", user.Username);
                cmd.Parameters.AddWithValue("@email", user.Email);
                cmd.Parameters.AddWithValue("@password", user.Password);
                cmd.Parameters.AddWithValue("@full_name", (object?)user.FullName ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@phone_number", (object?)user.PhoneNumber ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@role", user.Role);

                rows = cmd.ExecuteNonQuery();
            }

            _db.Close();
            return rows > 0;
        }
    }
}
