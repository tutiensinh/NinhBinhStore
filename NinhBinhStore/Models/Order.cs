using System;
using System.Collections.Generic;
namespace NinhBinhStore.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int IdTaiKhoan { get; set; } // Thêm ID tài khoản để map dữ liệu dễ hơn
        public DateTime Ngaydat { get; set; }
        public decimal Tongtien { get; set; }
        public string Trangthai { get; set; }

        // Thông tin người dùng (Join từ bảng User)
        public string Hoten { get; set; }
        public string Sodienthoai { get; set; }
        public string Email { get; set; }
        public string Diachi { get; set; } // Thêm địa chỉ giao hàng

        // Danh sách sản phẩm trong đơn
        public List<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}