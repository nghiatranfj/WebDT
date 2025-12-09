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

        // ==========================================================
        // 1. LẤY THÔNG TIN CUSTOMER THEO ID
        // ==========================================================
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
                        customer = MapReaderToCustomer(reader);
                    }
                }
            }

            connect.closeConnection();
            return customer;
        }

        // ==========================================================
        // 2. LẤY CUSTOMER THEO EMAIL (dùng để LOGIN)
        // ==========================================================
        public Customer? GetCustomerByEmail(string email)
        {
            connect.openConnection();

            Customer? customer = null;

            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connect.getConnecttion();
                command.CommandType = CommandType.Text;
                command.CommandText = @"SELECT * FROM customer WHERE email = @Email";

                command.Parameters.AddWithValue("@Email", email);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        customer = MapReaderToCustomer(reader);
                    }
                }
            }

            connect.closeConnection();
            return customer;
        }

        // ==========================================================
        // 3. ĐĂNG KÝ CUSTOMER MỚI
        // ==========================================================
        public bool CreateCustomer(Customer customer)
        {
            connect.openConnection();
            int isSuccess = 0;

            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = connect.getConnecttion();
                cmd.CommandType = CommandType.Text;

                string query = @"
                    INSERT INTO customer
                    (lastName, firstName, email, phone, address, img,
                     password, randomKey, isActive, role, registerAt, updateAt)
                    VALUES
                    (@LastName, @FirstName, @Email, @Phone, @Address, @Img,
                     @Password, @RandomKey, @IsActive, @Role, @RegisterAt, @UpdateAt)
                ";

                cmd.CommandText = query;

                cmd.Parameters.AddWithValue("@LastName", customer.LastName);
                cmd.Parameters.AddWithValue("@FirstName", customer.FirstName);
                cmd.Parameters.AddWithValue("@Email", customer.Email);
                cmd.Parameters.AddWithValue("@Phone", (object?)customer.Phone ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Address", (object?)customer.Address ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Img", (object?)customer.Img ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@Password", customer.Password);
                cmd.Parameters.AddWithValue("@RandomKey", customer.RandomKey ?? string.Empty);

                cmd.Parameters.AddWithValue("@IsActive", customer.IsActive);
                cmd.Parameters.AddWithValue("@Role", customer.Role);

                cmd.Parameters.AddWithValue("@RegisterAt", customer.RegisterAt == default ? DateTime.Now : customer.RegisterAt);
                cmd.Parameters.AddWithValue("@UpdateAt", customer.UpdateAt == default ? DateTime.Now : customer.UpdateAt);

                isSuccess = cmd.ExecuteNonQuery();
            }

            connect.closeConnection();
            return isSuccess > 0;
        }

        // ==========================================================
        // 4. CẬP NHẬT THÔNG TIN CUSTOMER
        // ==========================================================
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

                command.Parameters.AddWithValue("@Id", id);
                command.Parameters.AddWithValue("@LastName", customerUpdate.LastName);
                command.Parameters.AddWithValue("@FirstName", customerUpdate.FirstName);
                command.Parameters.AddWithValue("@Email", customerUpdate.Email);
                command.Parameters.AddWithValue("@Phone", (object?)customerUpdate.Phone ?? DBNull.Value);
                command.Parameters.AddWithValue("@Address", (object?)customerUpdate.Address ?? DBNull.Value);

                // Ảnh
                if (string.IsNullOrEmpty(customerUpdate.Img))
                    command.Parameters.AddWithValue("@Img", DBNull.Value);
                else
                    command.Parameters.AddWithValue("@Img", customerUpdate.Img);

                // Ngày sinh
                if (customerUpdate.DateOfBirth.HasValue)
                    command.Parameters.AddWithValue("@DateOfBirth", customerUpdate.DateOfBirth.Value);
                else
                    command.Parameters.AddWithValue("@DateOfBirth", DBNull.Value);

                // UpdateAt
                command.Parameters.AddWithValue("@UpdateAt", DateTime.Now);

                isSuccess = command.ExecuteNonQuery();
            }

            connect.closeConnection();
            return isSuccess > 0;
        }


        // ==========================================================
        // ============ HÀM MAP DATAREADER → CUSTOMER ===============
        // ==========================================================
        private Customer MapReaderToCustomer(SqlDataReader reader)
        {
            return new Customer
            {
                Id = Convert.ToInt32(reader["Id"]),
                LastName = reader["LastName"]?.ToString() ?? "",
                FirstName = reader["FirstName"]?.ToString() ?? "",
                Address = reader["Address"]?.ToString() ?? "",
                Email = reader["Email"]?.ToString() ?? "",
                Phone = reader["Phone"]?.ToString() ?? "",

                DateOfBirth = reader["DateOfBirth"] == DBNull.Value
                    ? null
                    : Convert.ToDateTime(reader["DateOfBirth"]),

                Img = reader["Img"] == DBNull.Value
                    ? null
                    : reader["Img"]?.ToString(),

                Password = reader["Password"]?.ToString() ?? "",
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

        public bool SignUp(Customer customer)
        {
            connect.openConnection();
            int rows = 0;

            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = connect.getConnecttion();
                cmd.CommandType = CommandType.Text;

                cmd.CommandText = @"
            INSERT INTO customer
            (lastName, firstName, email, phone, address, img,
             password, randomKey, isActive, role, registerAt, updateAt)
            VALUES
            (@LastName, @FirstName, @Email, @Phone, @Address, @Img,
             @Password, @RandomKey, @IsActive, @Role, @RegisterAt, @UpdateAt)
        ";

                cmd.Parameters.AddWithValue("@LastName", customer.LastName);
                cmd.Parameters.AddWithValue("@FirstName", customer.FirstName);
                cmd.Parameters.AddWithValue("@Email", customer.Email);
                cmd.Parameters.AddWithValue("@Phone", (object?)customer.Phone ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Address", (object?)customer.Address ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Img", (object?)customer.Img ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@Password", customer.Password);
                cmd.Parameters.AddWithValue("@RandomKey", customer.RandomKey ?? "");

                cmd.Parameters.AddWithValue("@IsActive", customer.IsActive);
                cmd.Parameters.AddWithValue("@Role", customer.Role);

                cmd.Parameters.AddWithValue("@RegisterAt",
                    customer.RegisterAt == default ? DateTime.Now : customer.RegisterAt);

                cmd.Parameters.AddWithValue("@UpdateAt",
                    customer.UpdateAt == default ? DateTime.Now : customer.UpdateAt);

                rows = cmd.ExecuteNonQuery();
            }

            connect.closeConnection();
            return rows > 0;
        }
    }
}
