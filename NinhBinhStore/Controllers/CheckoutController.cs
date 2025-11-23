using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Web; // Cần cài NuGet System.Web.HttpUtility nếu muốn Encode URL giống Java
using NinhBinhStore.DAO;
using NinhBinhStore.Helpers;
using NinhBinhStore.Models;

namespace WebBanDoGiaDung.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly DBContext _dbContext;
        // Thông tin ngân hàng (Hardcode như trong Servlet cũ)
        private const string BANK_ID = "970407";
        private const string ACCOUNT_NO = "19039593941013";
        private const string ACCOUNT_NAME = "NGUYEN NGOC TU";

        public CheckoutController(DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        // POST: /Checkout/Process
        [HttpPost]
        public IActionResult Process(string action, string fullName, string email, string phone, string address, string note)
        {
            // Lấy giỏ hàng
            var cart = HttpContext.Session.GetObjectFromJson<Cart>("Cart");
            var userId = HttpContext.Session.GetInt32("id_taikhoan");

            if (cart == null || cart.Items.Count == 0 || userId == null)
            {
                return RedirectToAction("Index", "Cart");
            }

            if (action == "confirm_shipping")
            {
                // Lưu thông tin ship vào session (tạm thời)
                HttpContext.Session.SetString("ship_fullName", fullName ?? "");
                HttpContext.Session.SetString("ship_email", email ?? "");
                HttpContext.Session.SetString("ship_phone", phone ?? "");
                HttpContext.Session.SetString("ship_address", address ?? "");
                HttpContext.Session.SetString("ship_note", note ?? "");

                // Tạo mã QR
                long amount = (long)cart.TotalCartPrice;
                string description = "DH" + DateTimeOffset.Now.ToUnixTimeMilliseconds();

                // Encode URL (Dùng System.Uri.EscapeDataString thay cho URLEncoder của Java)
                string encodedName = Uri.EscapeDataString(ACCOUNT_NAME);
                string encodedDesc = Uri.EscapeDataString(description);

                string qrUrl = $"https://img.vietqr.io/image/{BANK_ID}-{ACCOUNT_NO}-compact2.png?amount={amount}&addInfo={encodedDesc}&accountName={encodedName}";

                ViewBag.QrUrl = qrUrl;
                ViewBag.Description = description;
                ViewBag.Amount = amount;
                ViewBag.BankName = "Techcombank";
                ViewBag.AccountNo = ACCOUNT_NO;
                ViewBag.AccountName = ACCOUNT_NAME;

                return View("Checkout"); // Trả về View checkout.cshtml để quét mã
            }
            else if (action == "complete_order")
            {
                // Kiểm tra xem đã nhập địa chỉ chưa
                string shipAddress = HttpContext.Session.GetString("ship_address");
                string shipEmail = HttpContext.Session.GetString("ship_email");

                if (string.IsNullOrEmpty(shipAddress))
                {
                    return RedirectToAction("Index", "Cart");
                }

                // --- BẮT ĐẦU GIAO DỊCH (TRANSACTION) ---
                using (SqlConnection conn = _dbContext.GetConnection())
                {
                    conn.Open();
                    SqlTransaction transaction = conn.BeginTransaction();

                    try
                    {
                        // 1. Insert DonHang
                        string sqlOrder = "INSERT INTO donhang (id_taikhoan, ngaydat, diachi, email, tongtien, trangthai) VALUES (@Uid, GETDATE(), @Addr, @Email, @Total, N'Chờ xử lý'); SELECT SCOPE_IDENTITY();";
                        int orderId = 0;

                        using (SqlCommand cmdOrder = new SqlCommand(sqlOrder, conn, transaction))
                        {
                            cmdOrder.Parameters.AddWithValue("@Uid", userId);
                            cmdOrder.Parameters.AddWithValue("@Addr", shipAddress);
                            cmdOrder.Parameters.AddWithValue("@Email", shipEmail);
                            cmdOrder.Parameters.AddWithValue("@Total", cart.TotalCartPrice);

                            // Lấy ID vừa tạo
                            orderId = Convert.ToInt32(cmdOrder.ExecuteScalar());
                        }

                        // 2. Insert ChiTiet & Update Kho
                        string sqlDetail = "INSERT INTO chitietdonhang (id_donhang, id_sanpham, soluong, dongia) VALUES (@Oid, @Pid, @Qty, @Price)";
                        string sqlStock = "UPDATE sanpham SET tonkho = tonkho - @Qty WHERE id = @Pid";

                        foreach (var item in cart.Items)
                        {
                            // Insert Detail
                            using (SqlCommand cmdDetail = new SqlCommand(sqlDetail, conn, transaction))
                            {
                                cmdDetail.Parameters.AddWithValue("@Oid", orderId);
                                cmdDetail.Parameters.AddWithValue("@Pid", item.Product.Id);
                                cmdDetail.Parameters.AddWithValue("@Qty", item.Quantity);
                                cmdDetail.Parameters.AddWithValue("@Price", item.Product.Dongia);
                                cmdDetail.ExecuteNonQuery();
                            }

                            // Update Stock
                            using (SqlCommand cmdStock = new SqlCommand(sqlStock, conn, transaction))
                            {
                                cmdStock.Parameters.AddWithValue("@Qty", item.Quantity);
                                cmdStock.Parameters.AddWithValue("@Pid", item.Product.Id);
                                cmdStock.ExecuteNonQuery();
                            }
                        }

                        // Commit Transaction
                        transaction.Commit();

                        // Xử lý sau khi thành công
                        HttpContext.Session.SetInt32("last_order_id", orderId);
                        HttpContext.Session.Remove("Cart"); // Xóa giỏ hàng
                        HttpContext.Session.Remove("ship_address"); // Xóa session tạm
                        // ... xóa các session ship khác ...

                        return View("OrderSuccess"); // Chuyển sang trang thành công
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        // Log lỗi ex
                        return RedirectToAction("Index", "Cart", new { error = "order_failed" });
                    }
                }
            }
            return RedirectToAction("Index", "Cart");
        }

        // GET: /Checkout/Shipping
        public IActionResult Shipping()
        {
            // Kiểm tra đăng nhập
            if (HttpContext.Session.GetString("hoten") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Kiểm tra giỏ hàng
            var cart = HttpContext.Session.GetObjectFromJson<Cart>("Cart");
            if (cart == null || cart.Items.Count == 0)
            {
                return RedirectToAction("Index", "Cart");
            }

            // Lấy thông tin User để điền sẵn vào form (nếu có)
            var userId = HttpContext.Session.GetInt32("id_taikhoan");
            // Giả sử bạn có UserDAO để lấy thông tin chi tiết
            // var user = _userDAO.GetUserById(userId.Value); 
            // ViewBag.User = user; 

            // Tạm thời lấy từ Session nếu bạn đã lưu lúc login, hoặc để trống
            return View();
        }
    }
}