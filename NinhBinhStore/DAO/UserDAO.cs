using Microsoft.Data.SqlClient;
using NinhBinhStore.Models;

namespace NinhBinhStore.DAO
{
    public class UserDAO
    {
        private readonly DBContext _context;

        public UserDAO(DBContext context)
        {
            _context = context;
        }

        // 1. Kiểm tra đăng nhập
        public User CheckLogin(string username, string password)
        {
            string query = "SELECT * FROM taikhoan WHERE tendangnhap = @User AND matkhau = @Pass";
            using (SqlConnection conn = _context.GetConnection())
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@User", username);
                    cmd.Parameters.AddWithValue("@Pass", password);

                    using (SqlDataReader rs = cmd.ExecuteReader())
                    {
                        if (rs.Read())
                        {
                            return new User
                            {
                                Id = Convert.ToInt32(rs["id"]),
                                Hoten = rs["hoten"].ToString(),
                                Tendangnhap = rs["tendangnhap"].ToString(),
                                Matkhau = rs["matkhau"].ToString(),
                                Vaitro = rs["vaitro"].ToString(),
                                Email = rs["email"].ToString(),
                                Sodienthoai = rs["sodienthoai"].ToString(),
                                Diachi = rs["diachi"].ToString()
                            };
                        }
                    }
                }
            }
            return null;
        }

        // 2. Đăng ký user mới
        public bool Register(User u)
        {
            string query = "INSERT INTO taikhoan (hoten, tendangnhap, matkhau, email, sodienthoai, diachi, vaitro) VALUES (@Hoten, @User, @Pass, @Email, @Sdt, @Diachi, 'user')";
            using (SqlConnection conn = _context.GetConnection())
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Hoten", u.Hoten);
                    cmd.Parameters.AddWithValue("@User", u.Tendangnhap);
                    cmd.Parameters.AddWithValue("@Pass", u.Matkhau);
                    cmd.Parameters.AddWithValue("@Email", u.Email);
                    cmd.Parameters.AddWithValue("@Sdt", u.Sodienthoai ?? "");
                    cmd.Parameters.AddWithValue("@Diachi", u.Diachi ?? "");
                    try
                    {
                        return cmd.ExecuteNonQuery() > 0;
                    }
                    catch
                    {
                        return false; // Trùng tên đăng nhập hoặc lỗi khác
                    }
                }
            }
        }

        // [MỚI] 3. Lấy thông tin User theo ID (Dùng cho trang Tài khoản)
        public User GetUserById(int id)
        {
            string query = "SELECT * FROM taikhoan WHERE id = @Id";
            using (SqlConnection conn = _context.GetConnection())
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    using (SqlDataReader rs = cmd.ExecuteReader())
                    {
                        if (rs.Read())
                        {
                            return new User
                            {
                                Id = Convert.ToInt32(rs["id"]),
                                Hoten = rs["hoten"].ToString(),
                                Tendangnhap = rs["tendangnhap"].ToString(),
                                Email = rs["email"].ToString(),
                                Sodienthoai = rs["sodienthoai"].ToString(),
                                Diachi = rs["diachi"].ToString(),
                                Vaitro = rs["vaitro"].ToString()
                            };
                        }
                    }
                }
            }
            return null;
        }

        // [MỚI] 4. Cập nhật thông tin User
        public bool UpdateUserInfo(User u)
        {
            string query = "UPDATE taikhoan SET hoten=@Hoten, email=@Email, sodienthoai=@Phone, diachi=@Addr WHERE id=@Id";
            using (SqlConnection conn = _context.GetConnection())
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Hoten", u.Hoten);
                    cmd.Parameters.AddWithValue("@Email", u.Email);
                    cmd.Parameters.AddWithValue("@Phone", u.Sodienthoai ?? "");
                    cmd.Parameters.AddWithValue("@Addr", u.Diachi ?? "");
                    cmd.Parameters.AddWithValue("@Id", u.Id);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // [MỚI] 5. Lấy tất cả User (Cho Admin quản lý)
        public List<User> GetAllUsers()
        {
            List<User> list = new List<User>();
            string query = "SELECT * FROM taikhoan";
            using (SqlConnection conn = _context.GetConnection())
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader rs = cmd.ExecuteReader())
                {
                    while (rs.Read())
                    {
                        list.Add(new User
                        {
                            Id = Convert.ToInt32(rs["id"]),
                            Hoten = rs["hoten"].ToString(),
                            Tendangnhap = rs["tendangnhap"].ToString(),
                            Email = rs["email"].ToString(),
                            Vaitro = rs["vaitro"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        // Thêm vào UserDAO.cs

        // 1. Kiểm tra xem email có tồn tại trong hệ thống không
        public bool CheckEmailExists(string email)
        {
            using (var conn = _context.GetConnection())
            {
                conn.Open();
                string sql = "SELECT COUNT(*) FROM taikhoan WHERE email = @Email";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        // 2. Cập nhật mật khẩu mới theo email
        public bool UpdatePasswordByEmail(string email, string newPassword)
        {
            using (var conn = _context.GetConnection())
            {
                conn.Open();
                string sql = "UPDATE taikhoan SET matkhau = @Pass WHERE email = @Email";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Pass", newPassword);
                    cmd.Parameters.AddWithValue("@Email", email);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}
