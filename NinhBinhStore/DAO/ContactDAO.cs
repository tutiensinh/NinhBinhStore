using Microsoft.Data.SqlClient;
using NinhBinhStore.Models;

namespace NinhBinhStore.DAO
{
    public class ContactDAO
    {
        private readonly DBContext _context;

        public ContactDAO(DBContext context)
        {
            _context = context;
        }

        public bool SaveContact(string hoten, string email, string noidung)
        {
            string query = "INSERT INTO lienhe (hoten, email, noidung, ngaygui) VALUES (@Hoten, @Email, @Noidung, GETDATE())";
            using (SqlConnection conn = _context.GetConnection())
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Hoten", hoten);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Noidung", noidung);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public List<Contact> GetAllContacts()
        {
            List<Contact> list = new List<Contact>();
            string query = "SELECT * FROM lienhe ORDER BY ngaygui DESC";
            using (SqlConnection conn = _context.GetConnection())
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader rs = cmd.ExecuteReader())
                    {
                        while (rs.Read())
                        {
                            list.Add(new Contact
                            {
                                Id = Convert.ToInt32(rs["id"]),
                                Hoten = rs["hoten"].ToString(),
                                Email = rs["email"].ToString(),
                                Noidung = rs["noidung"].ToString(),
                                Ngaygui = Convert.ToDateTime(rs["ngaygui"])
                            });
                        }
                    }
                }
            }
            return list;
        }

        // [MỚI] Xóa liên hệ (Cho Admin)
        public bool DeleteContact(int id)
        {
            string query = "DELETE FROM lienhe WHERE id = @Id";
            using (SqlConnection conn = _context.GetConnection())
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}
