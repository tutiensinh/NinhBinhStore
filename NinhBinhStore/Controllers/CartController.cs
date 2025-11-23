using Microsoft.AspNetCore.Mvc;
using NinhBinhStore.DAO;
using NinhBinhStore.Helpers; // Cần namespace chứa SessionExtensions
using NinhBinhStore.Models;

namespace WebBanDoGiaDung.Controllers
{
    public class CartController : Controller
    {
        private readonly ProductDAO _productDAO;

        public CartController(ProductDAO productDAO)
        {
            _productDAO = productDAO;
        }

        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetObjectFromJson<Cart>("Cart") ?? new Cart();
            return View(cart);
        }

        // Action thêm vào giỏ
        // GET/POST: /Cart/Add?id=1&quantity=1&action=buy
        public IActionResult Add(int id, int quantity = 1, string type = "add")
        {
            // 1. KIỂM TRA ĐĂNG NHẬP
            if (HttpContext.Session.GetString("hoten") == null)
            {
                // Trả về mã lỗi 401 (Unauthorized) để JS bắt và hiện popup
                return StatusCode(401, new { message = "Vui lòng đăng nhập" });
            }

            var product = _productDAO.GetProductById(id);
            if (product == null) return NotFound();

            var cart = HttpContext.Session.GetObjectFromJson<Cart>("Cart") ?? new Cart();
            cart.AddItem(new CartItem(product, quantity));
            HttpContext.Session.SetObjectAsJson("Cart", cart);

            if (type == "buy")
            {
                // Nếu Mua ngay -> Trả về JSON chứa URL chuyển hướng
                return Ok(new { success = true, redirect = "/Cart/Index" });
            }
            else
            {
                // Nếu Thêm vào giỏ -> Trả về JSON thành công
                return Ok(new { success = true, totalItems = cart.TotalItemCount });
            }
        }

        public IActionResult Remove(int id)
        {
            var cart = HttpContext.Session.GetObjectFromJson<Cart>("Cart");
            if (cart != null)
            {
                cart.RemoveItem(id);
                HttpContext.Session.SetObjectAsJson("Cart", cart);
            }
            return RedirectToAction("Index");
        }

        // Cập nhật số lượng (cho trang giỏ hàng)
        public IActionResult Update(int id, int quantity)
        {
            var cart = HttpContext.Session.GetObjectFromJson<Cart>("Cart");
            if (cart != null)
            {
                cart.UpdateQuantity(id, quantity);
                HttpContext.Session.SetObjectAsJson("Cart", cart);
                // Trả về JSON để cập nhật giá tiền mà không load lại trang
                return Ok(new
                {
                    itemTotal = cart.Items.FirstOrDefault(x => x.Product.Id == id)?.TotalPrice.ToString("N0") + " đ",
                    cartTotal = cart.TotalCartPrice.ToString("N0") + " đ",
                    totalItems = cart.TotalItemCount
                });
            }
            return BadRequest();
        }
    }
}