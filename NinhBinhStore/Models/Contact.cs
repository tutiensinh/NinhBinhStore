namespace NinhBinhStore.Models
{
    public class Contact
    {
        public int Id { get; set; }
        public string Hoten { get; set; }
        public string Email { get; set; }
        public string Noidung { get; set; }
        public DateTime Ngaygui { get; set; }

        public Contact() { }

        public Contact(int id, string hoten, string email, string noidung, DateTime ngaygui)
        {
            Id = id;
            Hoten = hoten;
            Email = email;
            Noidung = noidung;
            Ngaygui = ngaygui;
        }
    }
}
