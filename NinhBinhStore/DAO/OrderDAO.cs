using Microsoft.Data.SqlClient;
using NinhBinhStore.Models;

namespace NinhBinhStore.DAO
{
    public class OrderDAO
    {
        private readonly DBContext _context;

        public OrderDAO(DBContext context)
        {
            _context = context;
        }

        // 1. Lấy đơn hàng theo User ID (Lịch sử mua hàng)
        public List<Order> GetOrdersByUserId(int userId)
        {
            List<Order> orders = new List<Order>();
            string query = @"SELECT d.id, d.ngaydat, d.tongtien, d.trangthai, t.hoten, t.email, d.diachi 
                             FROM donhang d 
                             JOIN taikhoan t ON d.id_taikhoan = t.id
                             WHERE d.id_taikhoan = @UserId AND d.trangthai = N'Hoàn thành'
                             ORDER BY d.ngaydat DESC";

            using (SqlConnection conn = _context.GetConnection())
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    using (SqlDataReader rs = cmd.ExecuteReader())
                    {
                        while (rs.Read())
                        {
                            orders.Add(new Order
                            {
                                Id = Convert.ToInt32(rs["id"]),
                                Ngaydat = Convert.ToDateTime(rs["ngaydat"]),
                                Tongtien = Convert.ToDecimal(rs["tongtien"]),
                                Trangthai = rs["trangthai"].ToString(),
                                Hoten = rs["hoten"].ToString(),
                                Email = rs["email"].ToString(),
                                Diachi = rs["diachi"].ToString()
                            });
                        }
                    }
                }
            }
            return orders;
        }

        // 2. Sửa hàm GetAllOrders (Cho Admin - Lấy thêm số điện thoại)
        public List<Order> GetAllOrders()
        {
            List<Order> orders = new List<Order>();
            // Lấy thêm sodienthoai từ bảng taikhoan
            string query = @"SELECT d.id, d.ngaydat, d.tongtien, d.trangthai, d.diachi, d.email, 
                            t.hoten, t.sodienthoai 
                     FROM donhang d 
                     JOIN taikhoan t ON d.id_taikhoan = t.id 
                     ORDER BY d.ngaydat DESC";

            using (var conn = _context.GetConnection())
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader rs = cmd.ExecuteReader())
                {
                    while (rs.Read())
                    {
                        var order = new Order
                        {
                            Id = Convert.ToInt32(rs["id"]),
                            Ngaydat = Convert.ToDateTime(rs["ngaydat"]),
                            Tongtien = Convert.ToDecimal(rs["tongtien"]),
                            Trangthai = rs["trangthai"].ToString(),
                            Hoten = rs["hoten"].ToString(),
                            Email = rs["email"].ToString(),
                            Diachi = rs["diachi"].ToString()
                        };
                        // Lưu SĐT vào object (Bạn cần thêm thuộc tính Sodienthoai vào Model Order nếu chưa có)
                        // order.Sodienthoai = rs["sodienthoai"].ToString(); 
                        orders.Add(order);
                    }
                }
            }
            return orders;
        }

        // 3. Sửa hàm GetOrderDetails (Lấy ảnh sản phẩm)
        public List<OrderItem> GetOrderDetails(int orderId)
        {
            var list = new List<OrderItem>();
            // Đảm bảo lấy cột hinhanh từ bảng sanpham
            string sql = @"SELECT c.*, s.tensp, s.hinhanh 
                   FROM chitietdonhang c 
                   JOIN sanpham s ON c.id_sanpham = s.id 
                   WHERE c.id_donhang = @Oid";
            using (var conn = _context.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Oid", orderId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new OrderItem
                            {
                                Id = (int)reader["id"],
                                IdSanpham = (int)reader["id_sanpham"],
                                Tensp = reader["tensp"].ToString(),
                                Hinhanh = reader["hinhanh"].ToString(), // <-- Đã lấy ảnh
                                Soluong = (int)reader["soluong"],
                                Dongia = Convert.ToDecimal(reader["dongia"])
                            });
                        }
                    }
                }
            }
            return list;
        }

        // 1. Sửa hàm FindOrdersByEmail (Cho tra cứu khách hàng)
        public List<Order> FindOrdersByEmail(string email)
        {
            var list = new List<Order>();
            using (var conn = _context.GetConnection())
            {
                conn.Open();
                // JOIN với bảng taikhoan để lấy họ tên người nhận
                string sql = @"SELECT d.*, t.hoten 
                       FROM donhang d 
                       JOIN taikhoan t ON d.id_taikhoan = t.id 
                       WHERE d.email = @Email 
                       ORDER BY d.ngaydat DESC";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Order
                            {
                                Id = (int)reader["id"],
                                Ngaydat = Convert.ToDateTime(reader["ngaydat"]),
                                Tongtien = Convert.ToDecimal(reader["tongtien"]),
                                Trangthai = reader["trangthai"].ToString(),
                                Diachi = reader["diachi"].ToString(),
                                Email = reader["email"].ToString(),
                                Hoten = reader["hoten"].ToString() // <-- Đã lấy được tên người nhận
                            });
                        }
                    }
                }
            }
            return list;
        }

        // [MỚI] 5. Cập nhật trạng thái đơn hàng (Admin)
        public bool UpdateOrderStatus(int orderId, string status)
        {
            string query = "UPDATE donhang SET trangthai = @Status WHERE id = @Id";
            using (SqlConnection conn = _context.GetConnection())
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Status", status);
                    cmd.Parameters.AddWithValue("@Id", orderId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // Thêm hàm này vào OrderDAO
        public List<RevenueViewModel> GetRevenueStats()
        {
            var list = new List<RevenueViewModel>();
            using (var conn = _context.GetConnection())
            {
                conn.Open();
                // Câu lệnh SQL: Nhóm theo ngày, tính tổng tiền và đếm số đơn HOÀN THÀNH
                string sql = @"
            SELECT 
                FORMAT(ngaydat, 'yyyy-MM-dd') as Ngay, 
                SUM(tongtien) as TongTien, 
                COUNT(id) as SoDon 
            FROM donhang 
            WHERE trangthai = N'Hoàn thành' 
            GROUP BY FORMAT(ngaydat, 'yyyy-MM-dd')
            ORDER BY Ngay DESC"; // Lấy ngày gần nhất trước

                using (var cmd = new SqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new RevenueViewModel
                        {
                            Ngay = Convert.ToDateTime(reader["Ngay"]).ToString("dd/MM/yyyy"),
                            DoanhThu = Convert.ToDecimal(reader["TongTien"]),
                            SoDonHang = Convert.ToInt32(reader["SoDon"])
                        });
                    }
                }
            }
            return list;
        }

        public Order GetOrderById(int id)
        {
            Order order = null;
            using (var conn = _context.GetConnection())
            {
                conn.Open();

                // 1. Lấy thông tin đơn hàng (Kèm tên và SĐT người mua từ bảng taikhoan)
                string sql = @"SELECT d.id, d.ngaydat, d.tongtien, d.diachi, d.email, t.hoten, t.sodienthoai 
                       FROM donhang d 
                       JOIN taikhoan t ON d.id_taikhoan = t.id 
                       WHERE d.id = @Id";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            order = new Order
                            {
                                Id = Convert.ToInt32(reader["id"]),
                                Ngaydat = Convert.ToDateTime(reader["ngaydat"]),
                                Tongtien = Convert.ToDecimal(reader["tongtien"]),
                                Diachi = reader["diachi"].ToString(),
                                Email = reader["email"].ToString(),
                                Hoten = reader["hoten"].ToString(),
                                // Map thêm số điện thoại nếu Model Order có thuộc tính này
                                // Sodienthoai = reader["sodienthoai"].ToString() 
                            };
                        }
                    }
                }

                // 2. Nếu tìm thấy đơn, lấy tiếp danh sách sản phẩm
                if (order != null)
                {
                    string sqlItems = @"SELECT c.*, s.tensp 
                                FROM chitietdonhang c 
                                JOIN sanpham s ON c.id_sanpham = s.id 
                                WHERE c.id_donhang = @Id";
                    using (var cmd = new SqlCommand(sqlItems, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                order.Items.Add(new OrderItem
                                {
                                    Id = Convert.ToInt32(reader["id"]),
                                    IdSanpham = Convert.ToInt32(reader["id_sanpham"]),
                                    Tensp = reader["tensp"].ToString(),
                                    Soluong = Convert.ToInt32(reader["soluong"]),
                                    Dongia = Convert.ToDecimal(reader["dongia"])
                                });
                            }
                        }
                    }
                }
            }
            return order;
        }

    }
}
