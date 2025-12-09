using Microsoft.Data.SqlClient;
using System;
using System.Data;
using WebD_T.Models;
using WebDT.Database;

namespace WebD_T.DAL
{
    public class CustomerDAL
    {
        private readonly DbConnect connect = new DbConnect();

        // ==============================
        // 1. Lấy thông tin Customer theo Id
        // ==============================
        public Customer? GetCustomerById(int id)
        {
            connect.openConnection();

            Customer? customer = null;

            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connect.getConnecttion();
                command.CommandType = CommandType.Text;

                string query = @"SELECT * FROM customer WHERE id = @Id";

                command.CommandText = query;
                command.Parameters.AddWithValue("@Id", id);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        customer = new Customer
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            LastName = reader["LastName"]?.ToString() ?? string.Empty,
                            FirstName = reader["FirstName"]?.ToString() ?? string.Empty,
                            Address = reader["Address"]?.ToString() ?? string.Empty,
                            Email = reader["Email"]?.ToString() ?? string.Empty,
                            Phone = reader["Phone"]?.ToString() ?? string.Empty,

                            DateOfBirth = reader["DateOfBirth"] == DBNull.Value
                                ? (DateTime?)null
                                : Convert.ToDateTime(reader["DateOfBirth"]),

                            Img = reader["Img"] == DBNull.Value
                                ? null
                                : reader["Img"]?.ToString(),

                            Password = reader["Password"]?.ToString() ?? string.Empty,
                            RandomKey = reader["RandomKey"] == DBNull.Value
                                ? null
                                : reader["RandomKey"]?.ToString(),

                            IsActive = reader["IsActive"] != DBNull.Value
                                && Convert.ToBoolean(reader["IsActive"]),

                            Role = reader["Role"] != DBNull.Value
                                ? Convert.ToInt32(reader["Role"])
                                : 0,

                            RegisterAt = reader["RegisterAt"] != DBNull.Value
                                ? Convert.ToDateTime(reader["RegisterAt"])
                                : DateTime.MinValue,

                            UpdateAt = reader["UpdateAt"] != DBNull.Value
                                ? Convert.ToDateTime(reader["UpdateAt"])
                                : DateTime.MinValue
                        };
                    }
                }
            }

            connect.closeConnection();
            return customer;
        }

        // ==============================
        // 2. Cập nhật thông tin chi tiết Customer
        // ==============================
        public bool UpdateDetailCustomer(Customer customerUpdate, int id)
        {
            connect.openConnection();

            int isSuccess = 0;

            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connect.getConnecttion();
                command.CommandType = CommandType.Text;

                string query = @"
                    UPDATE customer
                    SET 
                        lastName    = @LastName,
                        firstName   = @FirstName,
                        email       = @Email,
                        phone       = @Phone,
                        img         = @Img,
                        address     = @Address,
                        dateOfBirth = @DateOfBirth,
                        updateAt    = @UpdateAt
                    WHERE id = @Id;
                ";

                command.CommandText = query;

                // Auto set UpdateAt nếu bạn chưa set từ ngoài
                if (customerUpdate.UpdateAt == default)
                {
                    customerUpdate.UpdateAt = DateTime.Now;
                }

                command.Parameters.AddWithValue("@Id", id);
                command.Parameters.AddWithValue("@LastName", customerUpdate.LastName);
                command.Parameters.AddWithValue("@FirstName", customerUpdate.FirstName);
                command.Parameters.AddWithValue("@Email", customerUpdate.Email);
                command.Parameters.AddWithValue("@Phone", customerUpdate.Phone ?? string.Empty);
                command.Parameters.AddWithValue("@Address", customerUpdate.Address);

                // Img nullable
                if (string.IsNullOrEmpty(customerUpdate.Img))
                    command.Parameters.AddWithValue("@Img", DBNull.Value);
                else
                    command.Parameters.AddWithValue("@Img", customerUpdate.Img);

                // DateOfBirth nullable
                if (customerUpdate.DateOfBirth.HasValue)
                    command.Parameters.AddWithValue("@DateOfBirth", customerUpdate.DateOfBirth.Value);
                else
                    command.Parameters.AddWithValue("@DateOfBirth", DBNull.Value);

                command.Parameters.AddWithValue("@UpdateAt", customerUpdate.UpdateAt);

                isSuccess = command.ExecuteNonQuery();
            }

            connect.closeConnection();
            return isSuccess > 0;
        }
    }
}
