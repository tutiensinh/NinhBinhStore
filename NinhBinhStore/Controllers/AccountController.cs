using Microsoft.AspNetCore.Mvc;
using NinhBinhStore.DAO;
using NinhBinhStore.Helpers; // Để dùng SessionExtensions nếu cần
using NinhBinhStore.Models;

namespace WebBanDoGiaDung.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserDAO _userDAO;
        private readonly OrderDAO _orderDAO;

        public AccountController(UserDAO userDAO, OrderDAO orderDAO)
        {
            _userDAO = userDAO;
            _orderDAO = orderDAO;
        }

        // --- 1. ĐĂNG NHẬP (LoginServlet) ---
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string tendangnhap, string matkhau)
        {
            var user = _userDAO.CheckLogin(tendangnhap, matkhau);
            if (user != null)
            {
                // Lưu session
                HttpContext.Session.SetInt32("id_taikhoan", user.Id);
                HttpContext.Session.SetString("hoten", user.Hoten);
                HttpContext.Session.SetString("vaitro", user.Vaitro);

                // Lưu object user full nếu cần (dùng helper SessionExtensions)
                // HttpContext.Session.SetObjectAsJson("user", user);

                // Phân quyền
                if (user.IsAdmin)
                {
                    return RedirectToAction("Dashboard", "Admin");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                ViewBag.ErrorMessage = "Tài khoản hoặc mật khẩu không đúng.";
                return View();
            }
        }

        // --- 2. ĐĂNG XUẤT (LogoutServlet) ---
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        // --- 3. ĐĂNG KÝ (RegisterServlet) ---
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(User u, string nhaplaimatkhau)
        {
            if (u.Matkhau != nhaplaimatkhau)
            {
                ViewBag.ErrorMessage = "Mật khẩu nhập lại không khớp!";
                return View();
            }

            // Kiểm tra username tồn tại? (Cần thêm hàm này vào UserDAO hoặc xử lý try-catch)
            // Ở đây tôi giả định bạn sẽ thêm hàm CheckUsernameExist vào UserDAO, 
            // hoặc logic insert sẽ trả về false nếu trùng unique constraint.

            // Tạm thời gọi hàm Register
            bool result = _userDAO.Register(u);

            if (result)
            {
                TempData["SuccessMessage"] = "Đăng ký thành công! Vui lòng đăng nhập.";
                return RedirectToAction("Login");
            }
            else
            {
                ViewBag.ErrorMessage = "Tên đăng nhập đã tồn tại hoặc lỗi hệ thống.";
                return View();
            }
        }

        // --- 4. QUÊN MẬT KHẨU (ForgotPasswordServlet) ---
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ForgotPassword(string email, string newPass, string confirmPass)
        {
            // 1. Kiểm tra nhập lại mật khẩu
            if (newPass != confirmPass)
            {
                ViewBag.Message = "Mật khẩu xác nhận không khớp!";
                ViewBag.MessageType = "error";
                return View();
            }

            // 2. Kiểm tra email có tồn tại không
            bool exists = _userDAO.CheckEmailExists(email);
            if (!exists)
            {
                ViewBag.Message = "Email này chưa được đăng ký!";
                ViewBag.MessageType = "error";
                return View();
            }

            // 3. Cập nhật mật khẩu
            bool result = _userDAO.UpdatePasswordByEmail(email, newPass);

            if (result)
            {
                ViewBag.Message = "Đổi mật khẩu thành công! Vui lòng đăng nhập lại.";
                ViewBag.MessageType = "success";
                // Có thể chuyển hướng về Login sau vài giây nếu muốn
            }
            else
            {
                ViewBag.Message = "Có lỗi xảy ra, vui lòng thử lại sau.";
                ViewBag.MessageType = "error";
            }

            return View();
        }

        // Xem thông tin tài khoản
        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetInt32("id_taikhoan");
            if (userId == null) return RedirectToAction("Index", "Login");

            // Lấy thông tin user
            var user = _userDAO.GetUserById(userId.Value);

            // Lấy lịch sử đơn hàng (Optional - nếu bạn muốn hiển thị luôn ở trang này)
            var orders = _orderDAO.GetOrdersByUserId(userId.Value);
            ViewBag.Orders = orders;

            return View(user); // View: Account.cshtml
        }

        // Cập nhật thông tin
        [HttpPost]
        public IActionResult UpdateInfo(User updatedInfo)
        {
            var userId = HttpContext.Session.GetInt32("id_taikhoan");
            if (userId == null) return RedirectToAction("Index", "Login");

            // Lấy user hiện tại từ DB để đảm bảo an toàn
            var currentUser = _userDAO.GetUserById(userId.Value);

            if (currentUser != null)
            {
                currentUser.Hoten = updatedInfo.Hoten;
                currentUser.Email = updatedInfo.Email;
                currentUser.Sodienthoai = updatedInfo.Sodienthoai;
                currentUser.Diachi = updatedInfo.Diachi;

                if (_userDAO.UpdateUserInfo(currentUser))
                {
                    // Cập nhật lại session nếu cần
                    HttpContext.Session.SetString("hoten", currentUser.Hoten);
                    TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Có lỗi xảy ra khi cập nhật!";
                }
            }

            return RedirectToAction("Index");
        }
    }
}