using Microsoft.Data.SqlClient;
using NinhBinhStore.Models;

namespace NinhBinhStore.DAO
{
    public class ProductDAO
    {
        private readonly DBContext _context;

        public ProductDAO(DBContext context)
        {
            _context = context;
        }

        // 1. Helper method để map dữ liệu (Giúp code gọn hơn)
        private Product ReadProduct(SqlDataReader rs)
        {
            return new Product
            {
                Id = Convert.ToInt32(rs["id"]),
                Tensp = rs["tensp"].ToString(),
                Mota = rs["mota"].ToString(),
                Dongia = Convert.ToDecimal(rs["dongia"]),
                Hinhanh = rs["hinhanh"].ToString(),
                Loai = rs["loai"].ToString(),
                Tonkho = Convert.ToInt32(rs["tonkho"])
            };
        }

        // 2. Lấy tất cả sản phẩm (Cho Admin & Trang chủ)
        public List<Product> GetAllProducts()
        {
            List<Product> list = new List<Product>();
            string query = "SELECT * FROM sanpham ORDER BY id DESC";
            using (SqlConnection conn = _context.GetConnection())
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader rs = cmd.ExecuteReader())
                {
                    while (rs.Read()) list.Add(ReadProduct(rs));
                }
            }
            return list;
        }

        // 3. Lấy sản phẩm mới nhất (Có giới hạn số lượng)
        public List<Product> GetNewestProducts(int limit)
        {
            List<Product> list = new List<Product>();
            string query = "SELECT TOP (@Limit) * FROM sanpham ORDER BY id DESC";

            using (SqlConnection conn = _context.GetConnection())
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Limit", limit);
                    using (SqlDataReader rs = cmd.ExecuteReader())
                    {
                        while (rs.Read()) list.Add(ReadProduct(rs));
                    }
                }
            }
            return list;
        }

        // 4. Lấy sản phẩm theo loại (Có giới hạn - dùng cho Trang chủ)
        public List<Product> GetProductsByCategory(string categoryName, int limit)
        {
            List<Product> list = new List<Product>();
            string query = "SELECT TOP (@Limit) * FROM sanpham WHERE loai = @Category";

            using (SqlConnection conn = _context.GetConnection())
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Category", categoryName);
                    cmd.Parameters.AddWithValue("@Limit", limit);
                    using (SqlDataReader rs = cmd.ExecuteReader())
                    {
                        while (rs.Read()) list.Add(ReadProduct(rs));
                    }
                }
            }
            return list;
        }

        // [MỚI] 5. Lấy sản phẩm theo loại (Không giới hạn - dùng cho trang Danh mục)
        public List<Product> GetProductsByCategory(string categoryName)
        {
            List<Product> list = new List<Product>();
            string query = "SELECT * FROM sanpham WHERE loai = @Category";

            using (SqlConnection conn = _context.GetConnection())
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Category", categoryName);
                    using (SqlDataReader rs = cmd.ExecuteReader())
                    {
                        while (rs.Read()) list.Add(ReadProduct(rs));
                    }
                }
            }
            return list;
        }

        // [MỚI] 6. Lấy sản phẩm theo loại VÀ khoảng giá (Filter)
        public List<Product> GetProductsByCategoryAndPrice(string categoryName, decimal min, decimal max)
        {
            List<Product> list = new List<Product>();
            string query = "SELECT * FROM sanpham WHERE loai = @Category AND dongia BETWEEN @Min AND @Max";

            using (SqlConnection conn = _context.GetConnection())
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Category", categoryName);
                    cmd.Parameters.AddWithValue("@Min", min);
                    cmd.Parameters.AddWithValue("@Max", max);
                    using (SqlDataReader rs = cmd.ExecuteReader())
                    {
                        while (rs.Read()) list.Add(ReadProduct(rs));
                    }
                }
            }
            return list;
        }

        // [MỚI] 7. Lấy danh sách các loại sản phẩm (Cho Menu)
        public List<string> GetAllCategories()
        {
            List<string> list = new List<string>();
            string query = "SELECT DISTINCT loai FROM sanpham";
            using (SqlConnection conn = _context.GetConnection())
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader rs = cmd.ExecuteReader())
                {
                    while (rs.Read()) list.Add(rs["loai"].ToString());
                }
            }
            return list;
        }

        // 8. Tìm kiếm sản phẩm
        public List<Product> SearchProducts(string searchQuery)
        {
            List<Product> list = new List<Product>();
            string query = "SELECT * FROM sanpham WHERE LOWER(tensp) LIKE @Query OR LOWER(mota) LIKE @Query";

            using (SqlConnection conn = _context.GetConnection())
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    string searchPattern = "%" + searchQuery.ToLower() + "%";
                    cmd.Parameters.AddWithValue("@Query", searchPattern);
                    using (SqlDataReader rs = cmd.ExecuteReader())
                    {
                        while (rs.Read()) list.Add(ReadProduct(rs));
                    }
                }
            }
            return list;
        }

        // 9. Lấy 1 sản phẩm bằng ID
        public Product GetProductById(int id)
        {
            string query = "SELECT * FROM sanpham WHERE id = @Id";
            using (SqlConnection conn = _context.GetConnection())
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    using (SqlDataReader rs = cmd.ExecuteReader())
                    {
                        if (rs.Read()) return ReadProduct(rs);
                    }
                }
            }
            return null;
        }

        // 10. [ADMIN] Thêm sản phẩm mới
        public bool InsertProduct(Product p)
        {
            string query = "INSERT INTO sanpham (tensp, mota, dongia, hinhanh, loai, tonkho) VALUES (@Tensp, @Mota, @Dongia, @Hinhanh, @Loai, @Tonkho)";
            using (SqlConnection conn = _context.GetConnection())
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Tensp", p.Tensp);
                    cmd.Parameters.AddWithValue("@Mota", p.Mota);
                    cmd.Parameters.AddWithValue("@Dongia", p.Dongia);
                    cmd.Parameters.AddWithValue("@Hinhanh", p.Hinhanh);
                    cmd.Parameters.AddWithValue("@Loai", p.Loai);
                    cmd.Parameters.AddWithValue("@Tonkho", p.Tonkho);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // [MỚI] 11. [ADMIN] Cập nhật sản phẩm
        public bool UpdateProduct(Product p)
        {
            string query = "UPDATE sanpham SET tensp=@Tensp, mota=@Mota, dongia=@Dongia, hinhanh=@Hinhanh, loai=@Loai, tonkho=@Tonkho WHERE id=@Id";
            using (SqlConnection conn = _context.GetConnection())
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Tensp", p.Tensp);
                    cmd.Parameters.AddWithValue("@Mota", p.Mota);
                    cmd.Parameters.AddWithValue("@Dongia", p.Dongia);
                    cmd.Parameters.AddWithValue("@Hinhanh", p.Hinhanh);
                    cmd.Parameters.AddWithValue("@Loai", p.Loai);
                    cmd.Parameters.AddWithValue("@Tonkho", p.Tonkho);
                    cmd.Parameters.AddWithValue("@Id", p.Id);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // [MỚI] 12. [ADMIN] Xóa sản phẩm
        public bool DeleteProduct(int id)
        {
            string query = "DELETE FROM sanpham WHERE id = @Id";
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
