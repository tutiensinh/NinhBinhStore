using System.Globalization;
namespace NinhBinhStore.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Hoten { get; set; }
        public string Tendangnhap { get; set; }
        public string Matkhau { get; set; }
        public string Email { get; set; }
        public string Sodienthoai { get; set; }
        public string Diachi { get; set; }
        public string Vaitro { get; set; } // 'admin' hoặc 'user'

        public User() { }

        public User(int id, string hoten, string tendangnhap, string matkhau, string vaitro)
        {
            Id = id;
            Hoten = hoten;
            Tendangnhap = tendangnhap;
            Matkhau = matkhau;
            Vaitro = vaitro;
        }

        // Kiểm tra Admin (Property thay vì Method)
        public bool IsAdmin => Vaitro?.ToLower() == "admin";
    }
}
