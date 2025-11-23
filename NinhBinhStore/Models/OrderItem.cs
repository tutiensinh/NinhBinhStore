namespace NinhBinhStore.Models
{
    public class OrderItem
    {
        public int Id { get; set; } // Id của dòng chi tiết (từ bảng chitietdonhang)
        public int IdSanpham { get; set; }

        public string Tensp { get; set; }
        public string Hinhanh { get; set; }
        public int Soluong { get; set; }
        public decimal Dongia { get; set; }

        // Tính thành tiền
        public decimal TotalPrice => Soluong * Dongia;
    }
}
