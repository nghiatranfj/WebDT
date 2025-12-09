using Microsoft.Data.SqlClient;
using WebDT.Models;
using WebDT.Database;

namespace WebDT.DAL
{
    public class CategoryDAL
    {
        DbConnect connect = new DbConnect();
        public List<CategoryMenu> getAllWithCount()
        {
            connect.openConnection();
            List<CategoryMenu> list = new List<CategoryMenu>();
            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connect.getConnecttion();
                command.CommandType = System.Data.CommandType.Text;
                string query = @"select c.id, c.name, c.description, count(p.id) as
                        soluong
                         from categories c left join products p
                        on c.id = p.category_id
                        group by c.name,c.title, c.description
                        order by soluong desc
                        ";
                command.CommandText = query;
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    CategoryMenu category = new CategoryMenu()
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Name = reader["Name"].ToString() ?? "",
                        Description = reader["description"].ToString() ?? "",
                        Count = Convert.ToInt32(reader["soluong"].ToString())
                    };
                    list.Add(category);
                }
            }
            connect.closeConnection();
            return list;
        }

    }

}
