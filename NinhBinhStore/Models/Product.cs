using System.Globalization;
namespace NinhBinhStore.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Tensp { get; set; }
        public string Mota { get; set; }
        public decimal Dongia { get; set; } // Dùng decimal cho tiền tệ
        public string Hinhanh { get; set; }
        public string Loai { get; set; }
        public int Tonkho { get; set; }

        // Constructor mặc định
        public Product() { }

        // Constructor đầy đủ
        public Product(int id, string tensp, string mota, decimal dongia, string hinhanh, string loai, int tonkho)
        {
            Id = id;
            Tensp = tensp;
            Mota = mota;
            Dongia = dongia;
            Hinhanh = hinhanh;
            Loai = loai;
            Tonkho = tonkho;
        }

        // Property chỉ đọc để format giá tiền hiển thị ra View
        // Ví dụ: 1,000,000 đ
        public string GiaFormatted
        {
            get
            {
                return string.Format(new CultureInfo("vi-VN"), "{0:N0} đ", Dongia);
            }
        }
    }
}
