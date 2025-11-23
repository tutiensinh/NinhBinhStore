namespace NinhBinhStore.Models
{
    public class RevenueViewModel
    {
        public string Ngay { get; set; } // Ngày tháng (dd/MM/yyyy)
        public decimal DoanhThu { get; set; } // Tổng tiền bán được
        public int SoDonHang { get; set; } // Số lượng đơn
    }
}