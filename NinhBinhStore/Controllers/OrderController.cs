using Microsoft.AspNetCore.Mvc;
using NinhBinhStore.DAO;
namespace NinhBinhStore.Controllers
{
    public class OrderController : Controller
    {
        private readonly OrderDAO _orderDAO;

        public OrderController(OrderDAO orderDAO)
        {
            _orderDAO = orderDAO;
        }

        // --- 1. TRA CỨU ĐƠN HÀNG (OrderLookupServlet) ---
        public IActionResult Lookup()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Lookup(string email)
        {
            var orders = _orderDAO.FindOrdersByEmail(email);

            // Lấy chi tiết cho từng đơn
            foreach (var order in orders)
            {
                order.Items = _orderDAO.GetOrderDetails(order.Id);
            }

            ViewBag.SearchTerm = email;
            return View("Results", orders); // View: Order/Results.cshtml
        }

        // --- 2. LỊCH SỬ ĐƠN HÀNG (OrderHistoryServlet) ---
        public IActionResult History()
        {
            var userId = HttpContext.Session.GetInt32("id_taikhoan");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account", new { error = "Vui lòng đăng nhập để xem lịch sử" });
            }

            var orders = _orderDAO.GetOrdersByUserId(userId.Value);
            foreach (var order in orders)
            {
                order.Items = _orderDAO.GetOrderDetails(order.Id);
            }

            return View(orders);
        }
    }
}
