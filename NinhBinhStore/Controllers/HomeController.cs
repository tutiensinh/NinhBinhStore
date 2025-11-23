using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using NinhBinhStore.Models;
using NinhBinhStore.DAO;
namespace NinhBinhStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ProductDAO _productDAO;
        private readonly ContactDAO _contactDAO;

        public HomeController(ProductDAO productDAO, ContactDAO contactDAO)
        {
            _productDAO = productDAO;
            _contactDAO = contactDAO;
        }

        // Tương ứng HomeServlet
        public IActionResult Index()
        {
            // Lấy dữ liệu hiển thị trang chủ
            ViewBag.NewestProducts = _productDAO.GetNewestProducts(4);
            ViewBag.KitchenProducts = _productDAO.GetProductsByCategory("Nhà bếp", 4);
            ViewBag.CleaningProducts = _productDAO.GetProductsByCategory("Làm sạch", 4);
            ViewBag.SaleProducts = _productDAO.GetProductsByCategory("Khuyến mãi", 4);
            ViewBag.LargeAppliances = _productDAO.GetProductsByCategory("Gia dụng lớn", 4);

            return View(); // View: Home/Index.cshtml (trangchu.jsp cũ)
        }

        // Tương ứng ContactServlet (GET)
        public IActionResult Contact()
        {
            return View();
        }

        // Tương ứng ContactServlet (POST)
        [HttpPost]
        public IActionResult Contact(string hoten, string email, string noidung)
        {
            bool success = _contactDAO.SaveContact(hoten, email, noidung);

            if (success)
            {
                TempData["SuccessMessage"] = "Gửi liên hệ thành công! Chúng tôi sẽ sớm phản hồi.";
            }
            else
            {
                TempData["ErrorMessage"] = "Gửi liên hệ thất bại. Vui lòng thử lại.";
            }

            return RedirectToAction("Contact");
        }
    }
}
